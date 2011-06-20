Public Class managedepartment
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            loadFaculty()
            btnSave.Text = "Save"
            btnDelete.Text = "Delete"
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
                Me.btnDelete.Visible = True
                Me.btnSave.Visible = True
                btnSave.Text = "Update"
                litEdit.Text = "Edit Department"
            Case eMode.create
                Me.lblID.Text = ""
                Me.txtCode.Text = ""
                Me.txtShortName.Text = "" '.shortName
                Me.txtLongName.Text = "" '.longName
                Me.btnDelete.Visible = False
                Me.btnSave.Visible = True
                btnSave.Text = "Save"
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
        vContext.Departments.AddObject(vDepartment)
        vContext.SaveChanges()
    End Sub

    Protected Sub UpdateDepartment()
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vDepartment As Department = _
            (From p In vContext.Departments _
                Where p.ID = CType(lblID.Text, Integer) _
                    Select p).First
        With vDepartment
            .code = Me.txtCode.Text
            .longName = Me.txtLongName.Text
            .shortName = Me.txtShortName.Text
        End With
        vContext.SaveChanges()
    End Sub

    Protected Sub DeleteDepartment()
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vDepartment = (From p In vContext.Departments _
                            Where p.ID = CType(Me.lblID.Text, Integer) _
                            Select p).First
        vContext.DeleteObject(vDepartment)
        vContext.SaveChanges()
    End Sub

    Private Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click

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

    Private Sub btnDelete_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnDelete.Click
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

   

End Class