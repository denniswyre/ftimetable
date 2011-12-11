Public Class assignSubject
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            getDepartment1.loadFaculty(User.Identity.Name)
            loadDummySubjects()
            loadAssigneSubjects()
            logAssigned.Text = "Add"
            logUnassigned.Text = "Remove"
            logAssigned.Width = btnRefresh.Width
            logUnassigned.Width = btnRefresh.Width
        End If
    End Sub



    Sub loadDummySubjects()
        Dim vContext As timetableEntities = New timetableEntities()
        Dim DummyFacultyID = CType(ConfigurationManager.AppSettings("dummyfaculty"), Integer)
        Dim vSubject = (From p In vContext.subjects Where p.department.school.facultyID = DummyFacultyID And p.longName.Contains(txtSubjectSearch.Text) Order By p.longName Select p).ToList
        With lstUnassigned.Items
            .Clear()
            For Each x In vSubject
                .Add(New ListItem(x.longName + " [" + x.Code + "]", CStr(x.ID)))
            Next
        End With
    End Sub

    Sub loadAssigneSubjects()
        Dim vContext As timetableEntities = New timetableEntities()
        Dim DepartID = getDepartment1.getID
        Dim vsubject = (From p In vContext.subjects Where p.DepartmentID = DepartID Order By p.longName Select p).ToList
        With lstAssigned.Items
            .Clear()
            For Each x In vsubject
                .Add(New ListItem(x.longName + " [" + x.Code + "]", CStr(x.id)))
            Next
        End With
    End Sub

    Private Sub getDepartment1_DepartmentClick(E As Object, Args As clsDepartmentEvent) Handles getDepartment1.DepartmentClick
        loadDummySubjects()
        loadAssigneSubjects()
        errorMessage.Text = ""
    End Sub

    Private Sub logAssigned_Click(sender As Object, e As System.EventArgs) Handles logAssigned.Click
        Try
            Dim vContext As timetableEntities = New timetableEntities()
            Dim subject = (From p In vContext.subjects Where p.ID = CInt(lstUnassigned.SelectedValue) Select p).Single
            subject.DepartmentID = getDepartment1.getID
            logAssigned.Function = "Assigned"
            logAssigned.Description = subject.Code + "  Dept:" + subject.department.code
            vContext.SaveChanges()
            loadDummySubjects()
            loadAssigneSubjects()
            errorMessage.Text = ""
        Catch ex As Exception
            errorMessage.Text = clsGeneral.displaymessage(ex.Message, True)
        End Try
    End Sub

    Private Sub logUnassigned_Click(sender As Object, e As System.EventArgs) Handles logUnassigned.Click
        Try
            Dim DummyFacultyID = CType(ConfigurationManager.AppSettings("dummyfaculty"), Integer)
            Dim vContext As timetableEntities = New timetableEntities()
            Dim subject = (From p In vContext.subjects Where p.ID = CInt(lstAssigned.SelectedValue) Select p).Single
            subject.DepartmentID = ((From p In vContext.departments Where p.school.facultyID = DummyFacultyID Select p).First).ID
            logUnassigned.Function = "Unassigned"
            logUnassigned.Description = subject.Code + "  Dept:" + subject.department.code
            vContext.SaveChanges()
            loadDummySubjects()
            loadAssigneSubjects()
            errorMessage.Text = ""
        Catch ex As Exception
            errorMessage.Text = clsGeneral.displaymessage(ex.Message, True)
        End Try
    End Sub

    Private Sub btnRefresh_Click(sender As Object, e As System.EventArgs) Handles btnRefresh.Click
        loadDummySubjects()
        loadAssigneSubjects()
        errorMessage.Text = ""
    End Sub

    Private Sub btnSearch_Click(sender As Object, e As System.EventArgs) Handles btnSearch.Click
        loadDummySubjects()
    End Sub
End Class