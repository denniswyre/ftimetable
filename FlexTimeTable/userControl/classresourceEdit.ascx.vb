Public Class classresourceEdit
    Inherits System.Web.UI.UserControl

    Public Event Terminate As System.EventHandler

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            LoadResourceTypes()
            loadTimeSlots()
            btnSave.Text = "Save"
            btnDelete.Text = "Delete"
        End If
    End Sub
    Public Property ClassID() As Integer
        Get
            Return CInt(ViewState("classid"))
        End Get
        Set(ByVal value As Integer)
            ViewState("classid") = CStr(value)
        End Set
    End Property

    Public Sub loadTimeSlots()
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


    Public Sub setFields(ByVal vResourceID As Integer)
        lblMessage.Text = ""
        If vResourceID = 0 Then
            changeMode(eMode.create)
        Else
            Dim vContext As timetableEntities = New timetableEntities()
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
        End If
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
        End Select
    End Sub

    Protected Sub btnCancel_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnCancel.Click
        RaiseEvent Terminate(sender, e)
    End Sub

    Protected Sub CreateResource()
        Dim ClassID As Integer = CInt(ViewState("classid"))
        Dim vContext As timetableEntities = New timetableEntities()
        'Dim vClass = (From p In vContext.classgroups Where p.ID = ClassID Select p).First
        'Dim vlecturer = (From p In vContext.lecturers Where p.LecturerID = CInt(lblLecturer.ToolTip) Select p).FirstOrDefault
        Dim vResource = New resource With {
          .Name = txtName.Text,
          .AmtParticipants = CInt(txtNoOfParticipants.Text),
          .AmtTimeSlots = cboAmtTimeSlots.SelectedIndex + 1,
          .MaxMergedTimeSlots = cboMergedTimeSlots.SelectedIndex + 1,
          .ResourceTypeID = CInt(cboResourceType.SelectedValue)}
        vContext.resources.AddObject(vResource)
        vContext.SaveChanges()
        Dim vClassResource = New classgroupresource With {
       .classGroupID = ClassID,
       .ResourceID = vResource.ID}
        vContext.classgroupresources.AddObject(vClassResource)
        vContext.SaveChanges()
    End Sub

    Protected Sub UpdateResource()
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vResource As resource = _
            (From p In vContext.resources _
                Where p.ID = CType(lblID.Text, Integer) _
                    Select p).First
        With vResource
            .Name = txtName.Text
            .AmtParticipants = CInt(txtNoOfParticipants.Text)
            .AmtTimeSlots = cboAmtTimeSlots.SelectedIndex + 1
            .MaxMergedTimeSlots = cboMergedTimeSlots.SelectedIndex + 1
            .ResourceTypeID = CInt(cboResourceType.SelectedValue)
        End With
        vContext.SaveChanges()
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
            RaiseEvent Terminate(sender, e)
        Catch ex As Exception
            lblMessage.Text = ex.Message
        End Try
    End Sub

    Private Sub btnDelete_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnDelete.Click
        Try
            DeleteResource()
            RaiseEvent Terminate(sender, e)
        Catch ex As Exception
            lblMessage.Text = ex.Message
        End Try
    End Sub
End Class