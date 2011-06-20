Public Class managevenues
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            loadCampus()
            loadresourcetype()
            mvVenue.SetActiveView(vwGrid)
        End If
    End Sub

    Sub loadresourcetype()
        Dim vContext As timetableEntities = New timetableEntities()
        cboType.DataSource = (From p In vContext.resourcetypes _
                                     Select p.Description, p.ID)
        With Me.cboType
            .DataTextField = "Description"
            .DataValueField = "ID"
            .DataBind()
        End With
    End Sub
    Sub loadCampus()
        Dim vContext As timetableEntities = New timetableEntities()
        Dim OfficerID As Integer = clsOfficer.getOfficer(User.Identity.Name).ID
        Me.cboCampus.DataSource = (From p In vContext.campususers _
                                     Where p.OfficerID = OfficerID _
                                       Select p.CampusName, p.CampusID)
        Me.cboCampus.DataTextField = "CampusName"
        Me.cboCampus.DataValueField = "CampusID"
        Me.cboCampus.DataBind()
        loadSites()
    End Sub

    Sub loadSites()
        Dim vContext As timetableEntities = New timetableEntities
        cboSite.Items.Clear()
        If cboCampus.SelectedIndex >= 0 Then
            Dim CampusID = CType(Me.cboCampus.SelectedValue, Integer)
            Me.cboSite.DataSource = (From p In vContext.sites _
                                Where p.sitecluster.CampusID = CampusID _
                                Order By p.sitecluster.longName, p.longName _
                                  Select longName = (p.longName + "," + p.sitecluster.longName), p.ID)
        Else
            Me.cboSite.DataSource = Nothing
        End If
        With Me.cboSite
            .DataTextField = "longName"
            .DataValueField = "ID"
            .DataBind()
        End With
        loadBuildings()
    End Sub

    Sub loadBuildings()
        cboBuilding.Items.Clear()
        Dim vContext As timetableEntities = New timetableEntities()
        If cboSite.SelectedIndex >= 0 Then
            Dim SiteID = CType(cboSite.SelectedValue, Integer)
            Me.cboBuilding.DataSource = (From p In vContext.buildings Where p.SiteID = SiteID _
                                           Select p)
            Me.litSite.Text = "Site:"
        Else
            Me.cboBuilding.DataSource = Nothing
            Me.litSite.Text = "No Site Found:"
        End If
        With Me.cboBuilding
            .DataTextField = "longName"
            .DataValueField = "ID"
            .DataBind()
        End With
        loadVenue()
    End Sub

    Sub loadVenue()
        Dim vContext As timetableEntities = New timetableEntities()
        Dim BuildingID As Integer = -1
        With Me.grdVenue
            .Columns(1).Visible = True
            If cboBuilding.SelectedIndex > -1 Then
                BuildingID = CType(cboBuilding.SelectedValue, Integer)
                .DataSource = (From p In vContext.venues Where p.BuildingID = BuildingID _
                             Select TypeDescription = p.resourcetype.Description, TypeID = p.resourcetype.ID, p.Capacity, p.Description, p.Code, p.ID)
                Me.lnkCreate.Visible = True
                Me.litBuilding.Text = "Building:"
            Else
                .DataSource = Nothing
                Me.lnkCreate.Visible = False
                Me.litBuilding.Text = "No Building found:"
            End If
            .DataBind()
            .Columns(1).Visible = False
        End With
        litMessage.Text = ""
        mvVenue.SetActiveView(vwGrid)
    End Sub

    Private Sub grdVenue_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles grdVenue.SelectedIndexChanged
        With grdVenue
            Me.lblID.Text = .SelectedRow.Cells(0).Text
            Me.cboType.SelectedValue = .SelectedRow.Cells(1).Text
            Me.txtCode.Text = .SelectedRow.Cells(3).Text
            Me.txtDescription.Text = .SelectedRow.Cells(4).Text
            Me.txtCapacity.Text = .SelectedRow.Cells(5).Text
            changeMode(eMode.edit)
            litMessage.Text = ""
        End With
    End Sub

    Enum eMode
        edit
        delete
        create
    End Enum

    Sub changeMode(ByVal vMode As eMode)
        mvVenue.SetActiveView(vwEdit)
        Select Case vMode
            Case eMode.edit
                Me.btnDelete.Visible = True
                Me.btnSave.Visible = True
                btnSave.Text = "Update"
                Me.litEdit.Text = "Edit Building"
            Case eMode.create
                Me.lblID.Text = ""
                Me.txtCode.Text = ""
                Me.txtDescription.Text = ""
                Me.txtCapacity.Text = ""
                Me.cboType.SelectedIndex = 0
                Me.btnDelete.Visible = False
                Me.btnSave.Visible = True
                btnSave.Text = "Save"
                Me.litEdit.Text = "Create Building"
        End Select
    End Sub

    Private Sub lnkCreate_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles lnkCreate.Click
        changeMode(eMode.create)
    End Sub

    Protected Sub btnCancel_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnCancel.Click
        litMessage.Text = ""
        mvVenue.SetActiveView(vwGrid)
    End Sub

    Protected Sub Createvenue()
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vvenue = New venue With {
            .Code = Me.txtCode.Text,
            .Description = Me.txtDescription.Text,
            .Capacity = CInt(Me.txtCapacity.Text),
            .BuildingID = CType(Me.cboBuilding.SelectedItem.Value, Integer),
            .TypeID = CType(Me.cboType.SelectedItem.Value, Integer)}
        vContext.venues.AddObject(vvenue)
        vContext.SaveChanges()
    End Sub

    Protected Sub Updatevenue()
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vvenue As venue = _
            (From p In vContext.venues _
                Where p.ID = CType(lblID.Text, Integer) _
                    Select p).First
        With vvenue
            .Code = Me.txtCode.Text
            .Description = Me.txtDescription.Text
            .Capacity = CInt(Me.txtCapacity.Text)
            .TypeID = CType(Me.cboType.SelectedItem.Value, Integer)
        End With
        vContext.SaveChanges()
    End Sub

    Protected Sub Deletevenue()
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vvenue = (From p In vContext.venues _
                            Where p.ID = CType(Me.lblID.Text, Integer) _
                            Select p).First
        vContext.DeleteObject(vvenue)
        vContext.SaveChanges()
    End Sub

    Private Sub cboCampus_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cboCampus.SelectedIndexChanged
        loadSites()
    End Sub

    Private Sub cboSite_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cboSite.SelectedIndexChanged
        loadBuildings()
    End Sub

    Private Sub cboBuilding_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cboBuilding.SelectedIndexChanged
        loadVenue()
    End Sub

    Private Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
        Try
            If Me.lblID.Text = "" Then
                Createvenue()
                litMessage.Text = clsGeneral.displaymessage("Venue Created!!", False)
            Else
                Updatevenue()
                litMessage.Text = clsGeneral.displaymessage("Venue Created!!", False)
            End If
            loadVenue()
        Catch ex As Exception
            litMessage.Text = clsGeneral.displaymessage(ex.Message, True)
        End Try
    End Sub

    Private Sub btnDelete_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnDelete.Click
        Try
            Deletevenue()
            loadVenue()
            litMessage.Text = clsGeneral.displaymessage("Venue deleted!!", False)
        Catch ex As Exception
            litMessage.Text = clsGeneral.displaymessage(ex.Message, True)
        End Try
    End Sub
End Class