Public Class VenueAllocation
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            LoadCampuses()
            loadUsers()
            loadGrid()
        End If
    End Sub

    Sub LoadCampuses()
        Dim vContext As timetableEntities = New timetableEntities()
        Me.cboCampuses.DataSource = (From p In vContext.campus Order By p.longName Select p.longName, p.ID)
        Me.cboCampuses.DataTextField = "longName"
        Me.cboCampuses.DataValueField = "ID"
        Me.cboCampuses.DataBind()
    End Sub


    Sub loadUsers()
        Me.cboQualifiedUsers.DataSource = Roles.GetUsersInRole("venueAdmin")
        Me.cboQualifiedUsers.DataBind()
    End Sub

    Sub loadGrid()
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vCampusUsers = (From p In vContext.campususers Where _
                       p.CampusID = CType(Me.cboCampuses.SelectedValue, Double) Select p.Username)
        Me.grdCampusUsers.DataSource = vCampusUsers
        Me.grdCampusUsers.DataBind()
    End Sub

    Protected Sub btnAddUser_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnAddUser.Click
        Try
            Dim vContext As timetableEntities = New timetableEntities()
            Dim vCampus = (From p In vContext.campus Where _
                               p.ID = CType(Me.cboCampuses.SelectedValue, Double) Select p).First
            Dim Officer = (From p In vContext.officers _
                                   Where p.my_aspnet_users.name = Me.cboQualifiedUsers.SelectedItem.Text).First
            Officer.campus.Add(vCampus)
            vContext.SaveChanges()
            ' Display a status message
            ActionStatus.Text = String.Format("User {0} was added to Campus {1}.", Me.cboQualifiedUsers.SelectedItem.Text, cboCampuses.SelectedItem.Text)
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

    Protected Sub cboCampuses_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles cboCampuses.SelectedIndexChanged
        ActionStatus.Text = ""
        loadGrid()
    End Sub

    Private Sub grdCampusUsers_RowDeleting(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewDeleteEventArgs) Handles grdCampusUsers.RowDeleting
        ' Reference the UserNameLabel
        Dim UserNameLabel As Label = CType(grdCampusUsers.Rows(e.RowIndex).FindControl("UserNameLabel"), Label)

        ' Remove the user from Campus
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vCampus = (From p In vContext.campus Where _
                           p.ID = CType(Me.cboCampuses.SelectedValue, Double) Select p).First

        Dim vOfficer = (From p In vContext.officers Where p.my_aspnet_users.name = UserNameLabel.Text Select p).First

        vCampus.officers.Remove(vOfficer)
        vContext.SaveChanges()

        ' Refresh the GridView
        loadGrid()

        ' Display a status message
        ActionStatus.Text = String.Format("User {0} was removed from Campus {1}.", UserNameLabel.Text, cboCampuses.SelectedItem.Text)
        Me.ActionStatus.ForeColor = Drawing.Color.DarkSeaGreen
    End Sub
End Class