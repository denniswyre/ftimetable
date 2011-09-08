Public Class assignSubject
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            getDepartment1.loadFaculty(User.Identity.Name)
            loadDummySubjects()
            loadAssigneSubjects()
        End If
    End Sub



    Sub loadDummySubjects()
        Dim vContext As timetableEntities = New timetableEntities()
        Dim DummyFacultyID = CType(ConfigurationManager.AppSettings("dummyfaculty"), Integer)
        Dim vSubject = (From p In vContext.subjects Where p.department.school.facultyID = DummyFacultyID Order By p.longName Select name = (p.longName + ", " + p.Code), id = p.ID).ToList
        With lstUnassigned
            .DataSource = vSubject
            .DataTextField = "name"
            .DataValueField = "id"
            .DataBind()
        End With
    End Sub

    Sub loadAssigneSubjects()
        Dim vContext As timetableEntities = New timetableEntities()
        Dim DepartID = getDepartment1.getID
        Dim vsubject = (From p In vContext.subjects Where p.DepartmentID = DepartID Order By p.longName Select name = (p.longName + ", " + p.Code), id = p.ID).ToList
        With lstAssigned
            .DataSource = vsubject
            .DataTextField = "name"
            .DataValueField = "id"
            .DataBind()
        End With
    End Sub

    Private Sub getDepartment1_DepartmentClick(E As Object, Args As clsDepartmentEvent) Handles getDepartment1.DepartmentClick
        loadDummySubjects()
        loadAssigneSubjects()
    End Sub

    Private Sub btnAssigned_Click(sender As Object, e As System.EventArgs) Handles btnAssigned.Click
        Try
            Dim vContext As timetableEntities = New timetableEntities()
            Dim subject = (From p In vContext.subjects Where p.ID = CInt(lstUnassigned.SelectedValue) Select p).Single
            subject.DepartmentID = getDepartment1.getID
            vContext.SaveChanges()
            loadDummySubjects()
            loadAssigneSubjects()
        Catch ex As Exception

        End Try
    End Sub

    Private Sub btnUnassigned_Click(sender As Object, e As System.EventArgs) Handles btnUnassigned.Click
        Try
            Dim DummyFacultyID = CType(ConfigurationManager.AppSettings("dummyfaculty"), Integer)
            Dim vContext As timetableEntities = New timetableEntities()
            Dim subject = (From p In vContext.subjects Where p.ID = CInt(lstAssigned.SelectedValue) Select p).Single
            subject.DepartmentID = ((From p In vContext.departments Where p.school.facultyID = DummyFacultyID Select p).Single).ID
            vContext.SaveChanges()
            loadDummySubjects()
            loadAssigneSubjects()
        Catch ex As Exception

        End Try
    End Sub
End Class