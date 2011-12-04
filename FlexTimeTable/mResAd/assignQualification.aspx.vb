Public Class assignQualification
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            ucGetDepartment.loadFaculty(User.Identity.Name)
            loadDummyQualifications()
            loadAssigneQualifications()
            logAssigned.Text = "Add"
            logUnassigned.Text = "Remove"
            logAssigned.Width = btnRefresh.Width
            logUnassigned.Width = btnRefresh.Width
        End If
    End Sub



    Sub loadDummyQualifications()
        Dim vContext As timetableEntities = New timetableEntities()
        Dim DummyFacultyID = CType(ConfigurationManager.AppSettings("dummyfaculty"), Integer)
        Dim vQualification = (From p In vContext.qualifications Where p.department.school.facultyID = DummyFacultyID Order By p.longName Select name = (p.longName + ", " + p.Code), id = p.ID).ToList
        With lstUnassigned
            .DataSource = vQualification
            .DataTextField = "name"
            .DataValueField = "id"
            .DataBind()
        End With
    End Sub

    Sub loadAssigneQualifications()
        Dim vContext As timetableEntities = New timetableEntities()
        Dim DepartID = ucGetDepartment.getID
        Dim vQualification = (From p In vContext.qualifications Where p.DepartmentID = DepartID Order By p.longName Select name = (p.longName + ", " + p.Code), id = p.ID).ToList
        With lstAssigned
            .DataSource = vQualification
            .DataTextField = "name"
            .DataValueField = "id"
            .DataBind()
        End With
    End Sub

    Private Sub ucGetDepartment_DepartmentClick(E As Object, Args As clsDepartmentEvent) Handles ucGetDepartment.DepartmentClick
        loadDummyQualifications()
        loadAssigneQualifications()
        errorMessage.Text = ""
    End Sub

    Private Sub logAssigned_Click(sender As Object, e As System.EventArgs) Handles logAssigned.Click
        Try
            Dim vContext As timetableEntities = New timetableEntities()
            Dim Qualification = (From p In vContext.qualifications Where p.ID = CInt(lstUnassigned.SelectedValue) Select p).Single
            Qualification.DepartmentID = ucGetDepartment.getID
            vContext.SaveChanges()
            logAssigned.Function = "Assign"
            logAssigned.Description = Qualification.Code + "---Dept:" + Qualification.department.code
            loadDummyQualifications()
            loadAssigneQualifications()
            errorMessage.Text = ""
        Catch ex As Exception
            errorMessage.Text = clsGeneral.displaymessage(ex.Message, True)
        End Try
    End Sub

    Private Sub logUnassigned_Click(sender As Object, e As System.EventArgs) Handles logUnassigned.Click
        Try
            Dim DummyFacultyID = CType(ConfigurationManager.AppSettings("dummyfaculty"), Integer)
            Dim vContext As timetableEntities = New timetableEntities()
            Dim Qualification = (From p In vContext.qualifications Where p.ID = CInt(lstAssigned.SelectedValue) Select p).Single
            Qualification.DepartmentID = ((From p In vContext.departments Where p.school.facultyID = DummyFacultyID Select p).First).ID
            vContext.SaveChanges()
            logAssigned.Function = "Unassign"
            logAssigned.Description = Qualification.Code + "---Dept:" + Qualification.department.code
            loadDummyQualifications()
            loadAssigneQualifications()
            errorMessage.Text = ""
        Catch ex As Exception
            errorMessage.Text = clsGeneral.displaymessage(ex.Message, True)
        End Try
    End Sub

    Private Sub btnRefresh_Click(sender As Object, e As System.EventArgs) Handles btnRefresh.Click
        Try
            loadDummyQualifications()
            loadAssigneQualifications()
        Catch ex As Exception
            errorMessage.Text = clsGeneral.displaymessage(ex.Message, True)
        End Try
    End Sub
End Class