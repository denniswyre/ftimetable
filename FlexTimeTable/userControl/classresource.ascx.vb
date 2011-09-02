Public Class classresource
    Inherits System.Web.UI.UserControl

    Public Property ClassID() As Integer
        Get
            Return CInt(ViewState("classid"))
        End Get
        Set(ByVal value As Integer)
            ViewState("classid") = CStr(value)
        End Set
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            LoadResourceTypes()
            loadTimeSlots()
            btnSave.Text = "Save"
            btnDelete.Text = "Delete"
        End If
    End Sub

    Private Sub loadTimeSlots()
        For i = 1 To 30
            cboAmtTimeSlots.Items.Add(CStr(i))
        Next
        For i = 1 To 3
            cboMergedTimeSlots.Items.Add(CStr(i))
        Next
    End Sub

    Private Sub LoadResourceTypes()
        Dim vContext As timetableEntities = New timetableEntities()
        With cboResourceType
            .DataSource = (From p In vContext.resourcetypes Order By p.code Select name = (p.code + "(" + p.Description + ")"), p.ID)
            .DataTextField = "name"
            .DataValueField = "ID"
            .DataBind()
        End With
    End Sub

    Public Sub LoadResources()
        Dim vContext As timetableEntities = New timetableEntities()

        Dim vresources = (From p In vContext.classgroups
                                     Where p.ID = ClassID
                                      Select p.resources).FirstOrDefault
        grdResource.DataSource = (From p In vresources
                                      Select ID = p.ID,
                                             name = p.Name,
                                             TypeName = p.resourcetype.code,
                                             slots = p.AmtTimeSlots,
                                             size = p.AmtParticipants)
        grdResource.DataBind()
        loadAvailableVenues()
        Pages.ActiveViewIndex = 0
    End Sub

    Sub loadAvailableVenues()
        With cboAvailVenue
            Dim vContext As timetableEntities = New timetableEntities()
            Dim vClusterID = (From p In vContext.classgroups Where p.ID = ClassID Select p.SiteClusterID).First
            Dim vVenues = (From p In vContext.venues
                             Where p.building.site.SiteClusterID = vClusterID And
                                   p.resourcetype.ID = CInt(cboResourceType.SelectedValue)
                               Select name = (p.Code + "," + p.Description + p.building.shortName), p.ID).ToList
            .DataSource = vVenues
            .DataTextField = "name"
            .DataValueField = "ID"
            .DataBind()
        End With
    End Sub

    Sub loadSelectedVenues(ByVal vResourceID As Integer)
        With lstSelVenues
            Dim vContext As timetableEntities = New timetableEntities()
            Dim vVenues = (From p In vContext.resourcepreferredvenues
                             Where p.ResourceID = vResourceID
                               Select name = (p.venue.Code + "," + p.venue.Description + p.venue.building.shortName), ID = p.VenueID).ToList
            .DataSource = vVenues
            .DataTextField = "name"
            .DataValueField = "ID"
            .DataBind()
        End With
    End Sub


    Private Sub grdResource_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles grdResource.SelectedIndexChanged
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vResourceID = CType(grdResource.SelectedDataKey.Values(0), Integer)
        Dim vResource As resource = _
                   (From p In vContext.resources _
                       Where p.ID = vResourceID Select p).First
        With vResource
            lblID.Text = .ID.ToString
            txtName.Text = .Name
            txtNoOfParticipants.Text = CStr(.AmtParticipants)
            cboAmtTimeSlots.SelectedIndex = .AmtTimeSlots - 1
            cboMergedTimeSlots.SelectedIndex = .MaxMergedTimeSlots - 1
            cboResourceType.SelectedValue = CStr(.ResourceTypeID)
            changeMode(eMode.edit)
        End With
        loadSelectedVenues(vResourceID)
        lblMessage.Text = ""
    End Sub

    Private Sub lnkCreate_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles lnkCreate.Click
        changeMode(eMode.create)
        lblMessage.Text = ""
    End Sub

    Enum eMode
        edit
        delete
        create
    End Enum

    Sub changeMode(ByVal vMode As eMode)
        Select Case vMode
            Case eMode.edit
                Me.btnDelete.Visible = True
                Me.btnSave.Visible = True
                btnSave.Text = "Update"
                Me.pnlAction.GroupingText = "Edit Resource"
                Pages.ActiveViewIndex = 1
            Case eMode.create
                Me.lblID.Text = ""
                txtName.Text = ""
                txtNoOfParticipants.Text = ""
                cboAmtTimeSlots.SelectedIndex = 0
                cboMergedTimeSlots.SelectedIndex = 0
                cboResourceType.SelectedIndex = 0
                Me.btnDelete.Visible = False
                Me.btnSave.Visible = True
                btnSave.Text = "Save"
                Me.pnlAction.GroupingText = "Create Resource"
                Pages.ActiveViewIndex = 1
        End Select
    End Sub

    Protected Sub btnCancel_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnCancel.Click
        Pages.ActiveViewIndex = 0
    End Sub

 

    Function getResourceYear(ByVal startweek As Integer, ByVal endweek As Integer) As Integer
        Dim vCurWeek = DatePart(DateInterval.WeekOfYear, Now)
        If vCurWeek > endweek Then
            Return Year(Now) + 1
        Else
            Return Year(Now)
        End If
    End Function

    Structure sResource
        Dim ID As Integer
        Dim Name As String
        Dim year As Integer
        Dim startweek As Integer
        Dim endweek As Integer
        Dim ResourceTypeID As Integer
        Dim classgrouplinked As Boolean
        Dim AmtParticipants As Integer
        Dim AmtTimeSlots As Integer
        Dim MaxMergedTimeSlots As Integer
    End Structure

    Function prepareClassResource() As sResource
        Dim vContext As timetableEntities = New timetableEntities()
        Dim classgroup = (From p In vContext.classgroups Where p.ID = ClassID Select p).First
        Dim vRes As New sResource With {
            .Name = txtName.Text,
            .AmtParticipants = CInt(txtNoOfParticipants.Text),
            .AmtTimeSlots = cboAmtTimeSlots.SelectedIndex + 1,
            .MaxMergedTimeSlots = cboMergedTimeSlots.SelectedIndex + 1,
            .ResourceTypeID = CInt(cboResourceType.SelectedValue),
            .year = getResourceYear(classgroup.academicblock.startWeek, classgroup.academicblock.endWeek),
            .startWeek = classgroup.academicblock.startWeek,
            .endWeek = classgroup.academicblock.endWeek,
            .classgrouplinked = True}
        Return vRes
    End Function

    'auto generate first resource for newly created classgroup
    Public Function createFirstResource() As Integer
        Dim vContext As timetableEntities = New timetableEntities()
        Dim classgroup = (From p In vContext.classgroups Where p.ID = ClassID Select p).First
        Dim vRes As New sResource With {
            .Name = "Main",
            .AmtParticipants = classgroup.classSize,
            .AmtTimeSlots = classgroup.TimeSlotTotal,
            .MaxMergedTimeSlots = CInt(ConfigurationManager.AppSettings("defaultClassMaxMergedSlots")),
            .ResourceTypeID = CInt(ConfigurationManager.AppSettings("defaultClassResourceType")),
            .year = getResourceYear(classgroup.academicblock.startWeek, classgroup.academicblock.endWeek),
            .startWeek = classgroup.academicblock.startWeek,
            .endWeek = classgroup.academicblock.endWeek,
            .classgrouplinked = True}
        Return CreateResource(vRes)
    End Function

    Protected Function CreateResource() As Integer
        Return CreateResource(prepareClassResource())
    End Function

    Protected Function CreateResource(ByVal vRes As sResource) As Integer
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vclassgroup = (From p In vContext.classgroups Where p.ID = ClassID Select p).First
        Dim vResource = New resource With {
          .Name = vRes.Name,
          .AmtParticipants = vRes.AmtParticipants,
          .AmtTimeSlots = vRes.AmtTimeSlots,
          .MaxMergedTimeSlots = vRes.MaxMergedTimeSlots,
          .year = vRes.year,
          .startWeek = vRes.startweek,
          .endWeek = vRes.endweek,
          .classgrouplinked = vRes.classgrouplinked,
          .ResourceTypeID = vRes.ResourceTypeID}
        vContext.resources.AddObject(vResource)
        vContext.SaveChanges()
        vResource.classgroups.Add(vclassgroup)
        vContext.SaveChanges()
        saveVenues(vResource)
        Return vResource.ID
    End Function

    Protected Sub UpdateResource()
        UpdateResource(prepareClassResource())
    End Sub

    Protected Sub UpdateResource(ByVal vRes As sResource)
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vResource As resource = _
            (From p In vContext.resources _
                Where p.ID = CType(lblID.Text, Integer) _
                    Select p).First
        With vResource
            .Name = vRes.Name
            .AmtParticipants = vRes.AmtParticipants
            .AmtTimeSlots = vRes.AmtTimeSlots
            .MaxMergedTimeSlots = vRes.MaxMergedTimeSlots
            .year = vRes.year
            .startWeek = vRes.startweek
            .endWeek = vRes.endweek
            .classgrouplinked = vRes.classgrouplinked
            .ResourceTypeID = vRes.ResourceTypeID
        End With
        vContext.SaveChanges()
        saveVenues(vResource)
    End Sub


    Protected Sub DeleteResource()
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vResource = (From p In vContext.resources _
                            Where p.ID = CType(Me.lblID.Text, Integer) _
                            Select p).First
        vContext.DeleteObject(vResource)
        vContext.SaveChanges()
    End Sub

    Private Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
        Try
            If Me.lblID.Text = "" Then
                CreateResource()
            Else
                UpdateResource()
            End If
            LoadResources()
        Catch ex As Exception
            lblMessage.Text = ex.Message
        End Try
    End Sub

    Private Sub btnDelete_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnDelete.Click
        Try
            DeleteResource()
            LoadResources()
        Catch ex As Exception
            lblMessage.Text = ex.Message
        End Try
    End Sub

    Private Sub cboResourceType_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cboResourceType.SelectedIndexChanged
        loadAvailableVenues()
    End Sub

    Protected Sub btnVenueAdd_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnVenueAdd.Click
        '''''see if item already in list
        lstSelVenues.SelectedIndex = -1
        Try
            If lstSelVenues.Items.IndexOf(cboAvailVenue.SelectedItem) < 0 Then
                lstSelVenues.Items.Add(cboAvailVenue.SelectedItem)
            End If
        Catch ex As Exception
        End Try
    End Sub

    Protected Sub btnVenueRem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnVenueRem.Click
        If lstSelVenues.SelectedIndex > -1 Then
            lstSelVenues.Items.RemoveAt(lstSelVenues.SelectedIndex)
        End If
    End Sub

    Sub saveVenues(ByVal vRes As resource)
        Dim vContext As timetableEntities = New timetableEntities()
        Dim VenueDelArr As New ArrayList
        Dim VenueAddArr As New ArrayList
        'add venues not found in table
        For Each y As ListItem In lstSelVenues.Items
            Dim yFound As Boolean = False
            For Each x In vRes.resourcepreferredvenues
                If CInt(y.Value) = x.VenueID Then
                    yFound = True
                End If
            Next
            If Not yFound Then
                VenueAddArr.Add(CInt(y.Value))
            End If
        Next
        ' delete records not found in list
        For Each x In vRes.resourcepreferredvenues
            Dim xFound As Boolean = False
            For Each y As ListItem In lstSelVenues.Items
                If CInt(y.Value) = x.VenueID Then
                    xFound = True
                End If
            Next
            If Not xFound Then
                VenueDelArr.Add(x.VenueID)
            End If
        Next
        ''add venues
        For Each x As Integer In VenueAddArr
            Dim y As New resourcepreferredvenue With {
                .VenueID = x,
                .ResourceID = vRes.ID,
                .Rank = 0}
            vContext.resourcepreferredvenues.AddObject(y)
            vContext.SaveChanges()
        Next
        'delete venue
        For Each x As Integer In VenueDelArr
            Dim xVenueID = x
            Dim y = (From p In vContext.resourcepreferredvenues
                          Where p.VenueID = xVenueID And p.ResourceID = vRes.ID
                           Select p).First
            vContext.DeleteObject(y)
            vContext.SaveChanges()
        Next

    End Sub
End Class