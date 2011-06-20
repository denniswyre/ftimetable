Public Class ResourceAllocation
    Inherits System.Web.UI.Page
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            LoadFaculties()
            loadUsers()
            loadGrid()
        End If
    End Sub

    Sub LoadFaculties()
        Dim vContext As timetableEntities = New timetableEntities()
        Me.cboFaculties.DataSource = (From p In vContext.faculties Order By p.longName Select p.longName, p.ID)
        Me.cboFaculties.DataTextField = "longName"
        Me.cboFaculties.DataValueField = "ID"
        Me.cboFaculties.DataBind()
    End Sub


    Sub loadUsers()
        Me.cboQualifiedUsers.DataSource = Roles.GetUsersInRole("resourceAdmin")
        Me.cboQualifiedUsers.DataBind()
    End Sub

    Sub loadGrid()
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vFacultyUsers = (From p In vContext.Facultyusers Where _
                       p.FacultyID = CType(Me.cboFaculties.SelectedValue, Double) Select p.Username)
        Me.grdFacultyUsers.DataSource = vFacultyUsers
        Me.grdFacultyUsers.DataBind()
    End Sub

    Protected Sub btnAddUser_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnAddUser.Click
        Try
            Dim vContext As timetableEntities = New timetableEntities()
            Dim vFaculty = (From p In vContext.faculties Where _
                               p.ID = CType(Me.cboFaculties.SelectedValue, Double) Select p).First
            Dim Officer = (From p In vContext.officers _
                                   Where p.my_aspnet_users.name = Me.cboQualifiedUsers.SelectedItem.Text).First
            Officer.faculties.Add(vFaculty)
            vContext.SaveChanges()
            ' Display a status message
            ActionStatus.Text = String.Format("User {0} was added to Faculty {1}.", Me.cboQualifiedUsers.SelectedItem.Text, cboFaculties.SelectedItem.Text)
            Me.ActionStatus.ForeColor = Drawing.Color.DarkSeaGreen
        Catch ex As System.InvalidOperationException
            Me.ActionStatus.Text = "brokenlink detected! Update User in User Management"
        Catch ex As MySql.Data.MySqlClient.MySqlException
            Me.ActionStatus.Text = "2:duplicate:" + ex.Message
        Catch ex As UpdateException
            Me.ActionStatus.Text = "Already exists"
        End Try
        loadGrid()
    End Sub

    Protected Sub cboFaculties_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles cboFaculties.SelectedIndexChanged
        ActionStatus.Text = ""
        loadGrid()
    End Sub

    Private Sub grdFacultyUsers_RowDeleting(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewDeleteEventArgs) Handles grdFacultyUsers.RowDeleting
        ' Reference the UserNameLabel
        Dim UserNameLabel As Label = CType(grdFacultyUsers.Rows(e.RowIndex).FindControl("UserNameLabel"), Label)

        ' Remove the user from Faculty
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vFaculty = (From p In vContext.faculties Where _
                           p.ID = CType(Me.cboFaculties.SelectedValue, Double) Select p).First

        Dim vOfficer = (From p In vContext.officers Where p.my_aspnet_users.name = UserNameLabel.Text Select p).First

        vFaculty.officers.Remove(vOfficer)
        vContext.SaveChanges()

        ' Refresh the GridView
        loadGrid()

        ' Display a status message
        ActionStatus.Text = String.Format("User {0} was removed from Faculty {1}.", UserNameLabel.Text, cboFaculties.SelectedItem.Text)
        Me.ActionStatus.ForeColor = Drawing.Color.DarkSeaGreen
    End Sub
End Class