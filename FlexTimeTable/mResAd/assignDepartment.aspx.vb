Public Class assignDepartment
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            loadFaculty()
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
        With cboSchoolA
            .DataSource = (From p In vContext.schools Where p.facultyID = CInt(cboFaculty.SelectedValue) Select name = (p.longName), id = p.id).ToList
            .DataTextField = "name"
            .DataValueField = "id"
            .DataBind()
        End With
        With cboSchoolB
            .DataSource = (From p In vContext.schools Where p.facultyID = CInt(cboFaculty.SelectedValue) Select name = (p.longName), id = p.id).ToList
            .DataTextField = "name"
            .DataValueField = "id"
            .DataBind()
        End With
        loadDepartmentA()
        loadDepartmentB()
    End Sub

    Sub loadDepartmentA()
        Dim vContext As timetableEntities = New timetableEntities()
        With lstDepartmentA
            .DataSource = (From p In vContext.departments Where p.SchoolID = CInt(cboSchoolA.SelectedValue) Select name = (p.longName), id = p.ID).ToList
            .DataTextField = "name"
            .DataValueField = "id"
            .DataBind()
        End With
    End Sub

    Sub loadDepartmentB()
        Dim vContext As timetableEntities = New timetableEntities()
        With lstDepartmentB
            .DataSource = (From p In vContext.departments Where p.SchoolID = CInt(cboSchoolB.SelectedValue) Select name = (p.longName), id = p.ID).ToList
            .DataTextField = "name"
            .DataValueField = "id"
            .DataBind()
        End With
    End Sub



    Private Sub cboSchoolA_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles cboSchoolA.SelectedIndexChanged
        loadDepartmentA()
    End Sub

    Private Sub cboSchoolB_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles cboSchoolB.SelectedIndexChanged
        loadDepartmentB()
    End Sub

    Private Sub btnAddto_Click(sender As Object, e As System.EventArgs) Handles btnAddto.Click
        Try
            Dim vContext As timetableEntities = New timetableEntities()
            Dim vDepartment = (From p In vContext.departments Where p.ID = CInt(lstDepartmentA.SelectedValue) Select p).Single
            vDepartment.SchoolID = CInt(cboSchoolB.SelectedValue)
            vContext.SaveChanges()
            loadDepartmentA()
            loadDepartmentB()
        Catch ex As Exception
            
        End Try
    End Sub


    Private Sub btnAddfr_Click(sender As Object, e As System.EventArgs) Handles btnAddfr.Click
        Try
            Dim vContext As timetableEntities = New timetableEntities()
            Dim vDepartment = (From p In vContext.departments Where p.ID = CInt(lstDepartmentB.SelectedValue) Select p).Single
            vDepartment.SchoolID = CInt(cboSchoolA.SelectedValue)
            vContext.SaveChanges()
            loadDepartmentA()
            loadDepartmentB()
        Catch ex As Exception

        End Try
    End Sub

    Private Sub cboFaculty_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles cboFaculty.SelectedIndexChanged
        loadSchools()
    End Sub
End Class