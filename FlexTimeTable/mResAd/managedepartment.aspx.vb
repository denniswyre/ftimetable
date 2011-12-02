Public Class managedepartment
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            loadFaculty()
            logSave.Text = "Save"
            logDelete.Text = "Delete"
        End If
    End Sub

    Sub loadFaculty()
        Dim vContext As timetableEntities = New timetableEntities()
        Dim OfficerID As Integer = clsOfficer.getOfficer(User.Identity.Name).ID
        Me.cboFaculty.DataSource = (From p In vContext.facultyusers _
                                     Where p.OfficerID = OfficerID _
                                       Select p.FacultyName, p.FacultyID)
        Me.cboFaculty.DataTextField = "FacultyName"
        Me.cboFaculty.DataValueField = "FacultyID"
        Me.cboFaculty.DataBind()

        loadSchool()
    End Sub

    Sub loadSchool()
        Dim vContext As timetableEntities = New timetableEntities
        If cboFaculty.SelectedIndex >= 0 Then
            Dim FacultyID = CType(Me.cboFaculty.SelectedValue, Integer)
            Me.cboSchool.DataSource = (From p In vContext.schools _
                                Where p.facultyID = FacultyID _
                                Order By p.longName _
                                  Select p.longName, p.id)
        Else
            Me.cboSchool.DataSource = Nothing
        End If
        Me.cboSchool.DataTextField = "longName"
        Me.cboSchool.DataValueField = "ID"
        Me.cboSchool.DataBind()
        loadDepartments()
    End Sub

    Sub loadDepartments()
        Dim vContext As timetableEntities = New timetableEntities()
        If cboSchool.SelectedIndex >= 0 Then
            Dim schoolID = CType(cboSchool.SelectedValue, Integer)
            Me.grdDepartment.DataSource = (From p In vContext.departments Order By p.longName Where p.SchoolID = schoolID _
                                           Select p)
            litSchool.Text = "School:"
            lnkCreate.Visible = True
        Else
            Me.grdDepartment.DataSource = Nothing
            litSchool.Text = "No School Created for this Faculty:"
            lnkCreate.Visible = False
        End If
        Me.grdDepartment.DataBind()
        mvDept.SetActiveView(vwGrid)
        lblMessage.Text = ""
    End Sub

    Private Sub grdDepartment_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles grdDepartment.SelectedIndexChanged
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vID = CType(grdDepartment.SelectedRow.Cells(0).Text, Integer)
        Dim vDepart As department = _
                   (From p In vContext.departments _
                       Where p.ID = vID Select p).First
        With vDepart
            Me.lblID.Text = .ID.ToString
            Me.txtCode.Text = .code
            Me.txtShortName.Text = .shortName
            Me.txtLongName.Text = .longName
            changeMode(eMode.edit)
        End With
        lblMessage.Text = ""
    End Sub

    Enum eMode
        edit
        delete
        create
    End Enum

    Sub changeMode(ByVal vMode As eMode)
        mvDept.SetActiveView(vwEdit)
        Select Case vMode
            Case eMode.edit
                Me.logDelete.Visible = True
                Me.logSave.Visible = True
                logSave.Text = "Update"
                litEdit.Text = "Edit Department"
            Case eMode.create
                Me.lblID.Text = ""
                Me.txtCode.Text = ""
                Me.txtShortName.Text = "" '.shortName
                Me.txtLongName.Text = "" '.longName
                Me.logDelete.Visible = False
                Me.logSave.Visible = True
                logSave.Text = "Save"
                litEdit.Text = "Create Department"
        End Select
    End Sub

    Private Sub lnkCreate_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles lnkCreate.Click
        changeMode(eMode.create)
        lblMessage.Text = ""
    End Sub

    Protected Sub btnCancel_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnCancel.Click
        mvDept.SetActiveView(vwGrid)
        lblMessage.Text = ""
    End Sub

    Protected Sub CreateDepartment()
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vDepartment = New department With {
            .longName = Me.txtShortName.Text,
            .shortName = Me.txtCode.Text,
            .code = txtCode.Text,
            .SchoolID = CType(Me.cboSchool.SelectedItem.Value, Integer)}
        logSave.Function = "Create Department"
        logSave.Description = txtCode.Text + "---" + txtLongName.Text
        vContext.departments.AddObject(vDepartment)
        vContext.SaveChanges()
    End Sub

    Protected Sub UpdateDepartment()
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vDepartment As department = _
            (From p In vContext.departments _
                Where p.ID = CType(lblID.Text, Integer) _
                    Select p).First
        logSave.Function = "Update Department"
        logSave.Description = "From:" + vDepartment.code + "---" + vDepartment.longName + "---To:" + txtCode.Text + "---" + txtLongName.Text
        With vDepartment
            .code = Me.txtCode.Text
            .longName = Me.txtLongName.Text
            .shortName = Me.txtShortName.Text
        End With
        vContext.SaveChanges()
    End Sub

    Protected Sub DeleteDepartment()
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vDepartment = (From p In vContext.departments _
                            Where p.ID = CType(Me.lblID.Text, Integer) _
                            Select p).First
        logSave.Function = "Delete Department"
        logSave.Description = vDepartment.code + "---" + vDepartment.longName
        Dim DummyFacultyID = CType(ConfigurationManager.AppSettings("dummyfaculty"), Integer)
        If vDepartment.school.facultyID = DummyFacultyID Then
            Throw New Exception("You cannot delete this department!!!")
        End If
        vContext.DeleteObject(vDepartment)
        vContext.SaveChanges()
    End Sub

    Private Sub logSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles logSave.Click

        Try
            If Me.lblID.Text = "" Then
                CreateDepartment()
            Else
                UpdateDepartment()
            End If
            loadDepartments()
            lblMessage.Text = ""
        Catch ex As Exception
            lblMessage.Text = ex.Message
        End Try
    End Sub

    Private Sub logDelete_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles logDelete.Click
        Try
            DeleteDepartment()
            loadDepartments()
            lblMessage.Text = ""
        Catch ex As Exception
            lblMessage.Text = ex.Message
        End Try
    End Sub

    Private Sub cboFaculty_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cboFaculty.SelectedIndexChanged
        loadSchool()
    End Sub

   

    Private Sub cboSchool_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles cboSchool.SelectedIndexChanged
        loadDepartments()
    End Sub
End Class