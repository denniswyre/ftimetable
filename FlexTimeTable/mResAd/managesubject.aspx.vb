Public Class managesubject
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
        Me.cboFaculty.DataSource = (From p In vContext.Facultyusers _
                                     Where p.OfficerID = OfficerID _
                                       Select p.FacultyName, p.FacultyID)
        Me.cboFaculty.DataTextField = "FacultyName"
        Me.cboFaculty.DataValueField = "FacultyID"
        Me.cboFaculty.DataBind()

        loadDepartments()
    End Sub

    Sub loadDepartments()
        Dim vContext As timetableEntities = New timetableEntities
        If cboFaculty.SelectedIndex >= 0 Then
            Dim FacultyID = CType(Me.cboFaculty.SelectedValue, Integer)
            Me.cboDepartments.DataSource = (From p In vContext.Departments _
                                Where p.school.FacultyID = FacultyID _
                                Order By p.school.longName, p.longName _
                                  Select longName = (p.longName + "," + p.school.longName), p.ID)
        Else
            Me.cboDepartments.DataSource = Nothing
        End If
        Me.cboDepartments.DataTextField = "longName"
        Me.cboDepartments.DataValueField = "ID"
        Me.cboDepartments.DataBind()
        loadsubjects()
    End Sub

    Sub loadsubjects()
        Dim vContext As timetableEntities = New timetableEntities()
        If cboDepartments.SelectedIndex >= 0 Then
            Dim DepartmentID = CType(cboDepartments.SelectedValue, Integer)
            Me.grdsubject.DataSource = (From p In vContext.subjects Where p.DepartmentID = DepartmentID _
                                           Select p)
            litDepartment.Text = "Department:"
            lnkCreate.Visible = True
        Else
            Me.grdsubject.DataSource = Nothing
            litDepartment.Text = "No Department Created for this Faculty:"
            lnkCreate.Visible = False
        End If
        Me.grdsubject.DataBind()
        mvSubject.SetActiveView(vwGrid)
        lblMessage.Text = ""
    End Sub

    Private Sub grdsubject_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles grdsubject.SelectedIndexChanged
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vID = CType(grdsubject.SelectedRow.Cells(0).Text, Integer)
        Dim vQual As subject = _
                   (From p In vContext.subjects _
                       Where p.ID = vID Select p).First
        With vQual
            Me.lblID.Text = .ID.ToString
            Me.txtCode.Text = .Code
            chkYearBlock.Checked = .yearBlock
            Me.txtShortName.Text = .shortName
            Me.txtLongName.Text = .longName
            Me.txtOldCode.Text = .OldCode
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
        mvSubject.SetActiveView(vwEdit)
        Select Case vMode
            Case eMode.edit
                Me.btnDelete.Visible = True
                Me.btnSave.Visible = True
                btnSave.Text = "Update"
                litEdit.Text = "Edit subject"
            Case eMode.create
                Me.lblID.Text = ""
                Me.txtCode.Text = ""
                Me.txtShortName.Text = "" '.shortName
                Me.txtLongName.Text = "" '.longName
                Me.txtOldCode.Text = "" '.OldCode
                Me.btnDelete.Visible = False
                Me.btnSave.Visible = True
                btnSave.Text = "Save"
                litEdit.Text = "Create subject"
        End Select
    End Sub

    Private Sub lnkCreate_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles lnkCreate.Click
        changeMode(eMode.create)
        lblMessage.Text = ""
    End Sub

    Protected Sub btnCancel_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnCancel.Click
        mvSubject.SetActiveView(vwGrid)
        lblMessage.Text = ""
    End Sub

    Protected Sub Createsubject()
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vsubject = New subject With {
            .longName = Me.txtShortName.Text,
            .shortName = Me.txtCode.Text,
            .Code = txtCode.Text,
            .oldCode = txtOldCode.Text,
            .yearBlock = chkYearBlock.Checked,
            .Level = CType(Me.cboLevel.SelectedItem.Value, Integer),
            .DepartmentID = CType(Me.cboDepartments.SelectedItem.Value, Integer)}
        vContext.subjects.AddObject(vsubject)
        vContext.SaveChanges()
    End Sub

    Protected Sub Updatesubject()
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vsubject As subject = _
            (From p In vContext.subjects _
                Where p.ID = CType(lblID.Text, Integer) _
                    Select p).First
        With vsubject
            .longName = Me.txtShortName.Text
            .shortName = Me.txtCode.Text
            .Code = txtCode.Text
            .oldCode = txtOldCode.Text
            .yearBlock = chkYearBlock.Checked
            .Level = CType(Me.cboLevel.SelectedItem.Value, Integer)
        End With
        vContext.SaveChanges()
    End Sub

    Protected Sub Deletesubject()
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vsubject = (From p In vContext.subjects _
                            Where p.ID = CType(Me.lblID.Text, Integer) _
                            Select p).First
        vContext.DeleteObject(vsubject)
        vContext.SaveChanges()
    End Sub

    Private Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click

        Try
            If Me.lblID.Text = "" Then
                Createsubject()
            Else
                Updatesubject()
            End If
            loadsubjects()
            lblMessage.Text = ""
        Catch ex As Exception
            lblMessage.Text = ex.Message
        End Try
    End Sub

    Private Sub btnDelete_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnDelete.Click
        Try
            Deletesubject()
            loadsubjects()
            lblMessage.Text = ""
        Catch ex As Exception
            lblMessage.Text = ex.Message
        End Try
    End Sub

    Private Sub cboFaculty_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cboFaculty.SelectedIndexChanged
        loadDepartments()
    End Sub

    Private Sub cboDepartments_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cboDepartments.SelectedIndexChanged
        loadsubjects()
    End Sub

End Class