Public Class managedepartment
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            ucSchool.loadFaculty(User.Identity.Name)
            logSave.Text = "Save"
            logDelete.Text = "Delete"
            lnkCreate.Visible = clsOfficer.isAccessValid(User.Identity.Name, 0)
        End If
    End Sub

    

    Enum eMode
        view
        edit
        delete
        create
    End Enum

    Sub changeMode(ByVal vMode As eMode)
        mvDept.SetActiveView(vwEdit)
        Select Case vMode
            Case eMode.view
                ucSchool.Visible = False
                '   Me.logDelete.Visible = False
                '  Me.logSave.Visible = False
                logSave.Text = "Update"
                btnCancelEdit.Visible = False
                pnlDetail.Enabled = False
                pnlDetail.GroupingText = "Department Details"
                phID.Visible = True
            Case eMode.edit
                ucSchool.Visible = True
                pnlDetail.Enabled = True
                ' Me.logDelete.Visible = True
                'Me.logSave.Visible = True
                logSave.Text = "Update"
                pnlDetail.GroupingText = "Edit Department"
                phAccess.Visible = False
                btnCancelEdit.Visible = True
                phID.Visible = True
            Case eMode.create
                litOldSchool.Text = ""
                pnlDetail.Enabled = True
                ucSchool.Visible = True
                Me.lblID.Text = ""
                Me.txtCode.Text = ""
                Me.txtShortName.Text = "" '.shortName
                Me.txtLongName.Text = "" '.longName
                Me.logDelete.Visible = False
                Me.logSave.Visible = True
                logSave.Text = "Save"
                pnlDetail.GroupingText = "Create Department"
                phAccess.Visible = False
                btnCancelEdit.Visible = True
                phID.Visible = False
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

    Protected Function CreateDepartment() As Integer
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vSchooID = ucSchool.getID
        Dim vDepartment = New department With {
            .longName = Me.txtLongName.Text,
            .shortName = Me.txtShortName.Text,
            .code = txtCode.Text,
            .SchoolID = vSchooID}
        logSave.Function = "Create Department"
        logSave.Description = txtCode.Text + "---" + txtLongName.Text
        vContext.departments.AddObject(vDepartment)
        vContext.SaveChanges()
        Return vDepartment.ID
    End Function

    Protected Sub UpdateDepartment()
        Dim DummyDepartID = CType(ConfigurationManager.AppSettings("dummyDepartment"), Integer)
        Dim vDepartID = CType(lblID.Text, Integer)
        If DummyDepartID = vDepartID Then
            Throw New OverflowException("This is a reserved department. It cannot be updated")
        End If
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vDepartment As department = _
                   (From p In vContext.departments
                       Where p.ID = vDepartID
                           Select p).First
        Dim vSchooID = ucSchool.getID
        logSave.Function = "Update Department"
        logSave.Description = "From:" + vDepartment.code + "---" + vDepartment.longName + "---To:" + txtCode.Text + "---" + txtLongName.Text
        With vDepartment
            .code = Me.txtCode.Text
            .longName = Me.txtLongName.Text
            .shortName = Me.txtShortName.Text
            .SchoolID = vSchooID
        End With
        vContext.SaveChanges()
    End Sub

    Protected Sub DeleteDepartment()
        Dim vContext As timetableEntities = New timetableEntities()
        Dim DummyDepartID = CType(ConfigurationManager.AppSettings("dummyDepartment"), Integer)
        Dim DummyDepartment = (From p In vContext.departments
                            Where p.ID = DummyDepartID
                            Select p).First


        Dim vDepartID = CType(Me.lblID.Text, Integer)
        Dim vDepartment = (From p In vContext.departments _
                            Where p.ID = vDepartID
                            Select p).First
        If vDepartID = DummyDepartID Then
            'forbid deletion
            Throw New OverflowException("This is a reserved department. It cannot be Deleted!")
        ElseIf vDepartment.school.facultyID = DummyDepartment.school.facultyID Then
            'delete 
            EraseDepartment(vDepartID)
            logDelete.Function = "Erase Dummy Department"
            logDelete.Description = vDepartment.code + "---" + vDepartment.longName
        Else
            'move to dummy department
            logDelete.Function = "Move Department to Dummy"
            logDelete.Description = vDepartment.code + " school:" + vDepartment.school.code
            vDepartment.SchoolID = DummyDepartment.SchoolID
            vContext.SaveChanges()
        End If

    End Sub


    Private Sub EraseDepartment(ByVal DepartID As Integer)
        Dim vDummyDepartID = CType(ConfigurationManager.AppSettings("dummyDepartment"), Integer)
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vSelDepart = (From p In vContext.departments Where p.ID = DepartID Select p).Single

        'move all subjects to dummy department
        Do While vSelDepart.subjects.Count > 0
            vSelDepart.subjects.First()
            vSelDepart.subjects.First.DepartmentID = vDummyDepartID
            vContext.SaveChanges()
        Loop
        'move all qualifications to dummy department
        Do While vSelDepart.qualifications.Count > 0
            vSelDepart.qualifications.First()
            vSelDepart.qualifications.First.DepartmentID = vDummyDePartID
            vContext.SaveChanges()
        Loop

        'move lecturers to dummy department
        Do While vSelDepart.lecturers.Count > 0
            vSelDepart.lecturers.First()
            vSelDepart.lecturers.First.DepartmentID = vDummyDePartID
            vContext.SaveChanges()
        Loop

        'delete department venues 
        Do While vSelDepart.venues.Count > 0
            vContext.DeleteObject(vSelDepart.venues.First())
            vContext.SaveChanges()
        Loop

        'delete departement
        vContext.DeleteObject(vSelDepart)
        vContext.SaveChanges()
    End Sub


    Private Sub logSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles logSave.Click
        Try
            Dim vID As Integer
            If Me.lblID.Text = "" Then
                vID = CreateDepartment()
            Else
                vID = CInt(Me.lblID.Text)
                UpdateDepartment()
            End If
            lblMessage.Text = ""
            DisplayDepartment(vID)
        Catch ex As Exception
            lblMessage.Text = ex.Message
        End Try
    End Sub

    Private Sub logDelete_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles logDelete.Click
        Try
            DeleteDepartment()
            lblMessage.Text = ""
            mvDept.SetActiveView(vwGrid)
        Catch ex As Exception
            lblMessage.Text = ex.Message
        End Try
    End Sub

    Private Sub ucDepartmentSearch_DepartmentClick(E As Object, Args As clsDepartmentEvent) Handles ucDepartmentSearch.DepartmentClick
        DisplayDepartment(Args.mDepartmentID)
    End Sub


    Sub DisplayDepartment(ByVal vID As Integer)
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vDepart As department = _
                   (From p In vContext.departments _
                       Where p.ID = vID Select p).First
        With vDepart
            Me.lblID.Text = .ID.ToString
            Me.txtCode.Text = .code
            Me.txtShortName.Text = .shortName
            Me.txtLongName.Text = .longName

            Dim DummyFacultyID = CType(ConfigurationManager.AppSettings("dummyfaculty"), Integer)
            Dim vSchoolID = .SchoolID
            If .school.facultyID = DummyFacultyID Then
                litOldSchool.Text = "<b>Linked School:None</b>"
            Else
                litOldSchool.Text = "<b>Linked School:</b>" + .school.longName +
                                   " <b>Faculty:</b>" + .school.faculty.code
            End If
            changeMode(eMode.view)

            Dim vFacultyID = .school.faculty.ID
            If vFacultyID = DummyFacultyID Then
                phAccess.Visible = clsOfficer.isAccessValid(User.Identity.Name, 0)
            Else
                phAccess.Visible = clsOfficer.isAccessValid(User.Identity.Name, .school.faculty.ID)
            End If
            logDelete.Visible = clsOfficer.isAccessValid(User.Identity.Name, .school.faculty.ID)
            lblMessage.Text = ""
            mvDept.SetActiveView(vwEdit)
        End With
    End Sub

    Private Sub lnkEdit_Click(sender As Object, e As System.EventArgs) Handles lnkEdit.Click
        changeMode(eMode.edit)
    End Sub

    Private Sub btnCancelEdit_Click(sender As Object, e As System.EventArgs) Handles btnCancelEdit.Click
        If Me.lblID.Text = "" Then
            mvDept.SetActiveView(vwGrid)
        Else
            DisplayDepartment(CInt(Me.lblID.Text))
        End If
        lblMessage.Text = ""
    End Sub
End Class