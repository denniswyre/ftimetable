Public Class assignDepartment
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            loadFaculty()
            logAdd.Text = "Add"
            logDel.Text = "Remove"
            logErase.Text = "Erase"
            logAdd.Width = btnRefresh.Width
            logDel.Width = btnRefresh.Width
            logErase.Width = btnRefresh.Width
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
        loadSchools()
    End Sub

    Sub loadSchools()
        Dim vContext As timetableEntities = New timetableEntities()
        With cboSchool
            .DataSource = (From p In vContext.schools Where p.facultyID = CInt(cboFaculty.SelectedValue) Select name = (p.longName), id = p.id).ToList
            .DataTextField = "name"
            .DataValueField = "id"
            .DataBind()
        End With
        loadUnknownDepartments()
        loadDepartments()
    End Sub

    Sub loadUnknownDepartments()
        Dim vContext As timetableEntities = New timetableEntities()
        Dim DummyFacultyID = CType(ConfigurationManager.AppSettings("dummyfaculty"), Integer)
        With lstUnknownDepart
            .DataSource = (From p In vContext.departments Where p.school.facultyID = DummyFacultyID Select name = (p.longName), id = p.ID).ToList
            .DataTextField = "name"
            .DataValueField = "id"
            .DataBind()
        End With
    End Sub

    Sub loadDepartments()
        Dim vContext As timetableEntities = New timetableEntities()
        With lstSelectedDepart
            .DataSource = (From p In vContext.departments Where p.SchoolID = CInt(cboSchool.SelectedValue) Select name = (p.longName), id = p.ID).ToList
            .DataTextField = "name"
            .DataValueField = "id"
            .DataBind()
        End With
    End Sub



    Private Sub cboSchool_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles cboSchool.SelectedIndexChanged
        loadDepartments()
    End Sub

    Private Sub logAdd_Click(sender As Object, e As System.EventArgs) Handles logAdd.Click
        Try
            If lstUnknownDepart.SelectedIndex >= 0 Then
                Dim vContext As timetableEntities = New timetableEntities()
                Dim vDepartment = (From p In vContext.departments Where p.ID = CInt(lstUnknownDepart.SelectedValue) Select p).Single
                If vDepartment.longName = "Dummy Department" Then
                    Throw New Exception("Cannot Move this Dummy Department!!")
                End If
                vDepartment.SchoolID = CInt(cboSchool.SelectedValue)
                logAdd.Function = "Add Department from dummy"
                logAdd.Description = vDepartment.code + " school:" + vDepartment.school.code
                vContext.SaveChanges()
                loadUnknownDepartments()
                loadDepartments()
            Else
                Throw New Exception("You must select an unknown department!")
            End If
        Catch ex As Exception
            ErrorMessage.Text = clsGeneral.displaymessage(ex.Message, True)
        End Try
    End Sub


    Private Sub logDel_Click(sender As Object, e As System.EventArgs) Handles logDel.Click
        Try
            If lstSelectedDepart.SelectedIndex >= 0 Then
                Dim vContext As timetableEntities = New timetableEntities()
                Dim DummyFacultyID = CType(ConfigurationManager.AppSettings("dummyfaculty"), Integer)
                Dim DummySchool = (From p In vContext.schools Where p.facultyID = DummyFacultyID Select p).First
                Dim vDepartment = (From p In vContext.departments Where p.ID = CInt(lstSelectedDepart.SelectedValue) Select p).Single
                logDel.Function = "Move Department to Dummy"
                logDel.Description = vDepartment.code + " school:" + vDepartment.school.code
                vDepartment.SchoolID = DummySchool.id
                vContext.SaveChanges()
                loadUnknownDepartments()
                loadDepartments()
            Else
                Throw New Exception("You must select a department!")
            End If
        Catch ex As Exception
            ErrorMessage.Text = clsGeneral.displaymessage(ex.Message, True)
        End Try
    End Sub

    Private Sub cboFaculty_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles cboFaculty.SelectedIndexChanged
        loadSchools()
    End Sub

    Private Sub logErase_Click(sender As Object, e As System.EventArgs) Handles logErase.Click
        Try
            If lstUnknownDepart.SelectedIndex >= 0 Then
                EraseDepartment(CInt(lstUnknownDepart.SelectedValue))
                logAdd.Function = "Erase Department from Dummy"
                logAdd.Description = lstUnknownDepart.SelectedItem.Text
                loadUnknownDepartments()
                loadDepartments()
            Else
                Throw New Exception("You must select an unknown department!")
            End If
        Catch ex As Exception
            ErrorMessage.Text = clsGeneral.displaymessage(ex.Message, True)
        End Try
    End Sub



    Private Sub EraseDepartment(ByVal DepartID As Integer)
        Dim vContext As timetableEntities = New timetableEntities()
        Dim DummyDepartName = "Dummy Department"
        Dim vDummyDePartID = (From p In vContext.departments Where p.longName = DummyDepartName Select p).Single.ID
        Dim vSelDepart = (From p In vContext.departments Where p.ID = DepartID Select p).Single

        If vSelDepart.ID = vDummyDePartID Then
            Throw New Exception("Cannot Erase this Dummy Department!!")
        End If

        'move all subjects to dummy department
        Do While vSelDepart.subjects.Count > 0
            vSelDepart.subjects.First()
            vSelDepart.subjects.First.DepartmentID = vDummyDePartID
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
End Class