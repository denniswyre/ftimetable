Imports System.Net
Imports System.DirectoryServices
Imports System.DirectoryServices.Protocols
'Imports System.Security.Cryptography.X509Certificates
Public Class userManagement
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            ' Bind the users and roles
            BindRolesToList()

            'Display those users belonging to the currently selected role
            DisplayUsersBelongingToRole()

            Me.UpdateUser.Enabled = False
            Me.CancelUser.Enabled = False
        End If
    End Sub

    Private Sub BindRolesToList()
        ' Get all of the roles
        Dim roleNames() As String = Roles.GetAllRoles()
        RoleList.DataSource = roleNames
        RoleList.DataBind()

        Me.UserRoleList.DataSource = roleNames
        Me.UserRoleList.DataBind()
    End Sub

#Region "Roles Tasks"

    Protected Sub RoleList_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles RoleList.SelectedIndexChanged
        DisplayUsersBelongingToRole()
    End Sub

    Private Sub DisplayUsersBelongingToRole()
        ' Get the selected role
        Dim selectedRoleName As String = RoleList.SelectedValue

        ' Get the list of usernames that belong to the role
        Dim usersBelongingToRole() As String = Roles.GetUsersInRole(selectedRoleName)

        ' Bind the list of users to the GridView
        RolesUserList.DataSource = usersBelongingToRole
        RolesUserList.DataBind()
    End Sub

    Protected Sub RolesUserList_RowDeleting(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewDeleteEventArgs) Handles RolesUserList.RowDeleting
        ' Get the selected role
        Dim selectedRoleName As String = RoleList.SelectedValue

        ' Reference the UserNameLabel
        Dim UserNameLabel As Label = CType(RolesUserList.Rows(e.RowIndex).FindControl("UserNameLabel"), Label)

        ' Remove the user from the role
        Try
            Roles.RemoveUserFromRole(UserNameLabel.Text, selectedRoleName)

            ' Refresh the GridView
            DisplayUsersBelongingToRole()

            ' Display a status message
            ActionStatus.Text = String.Format("User {0} was removed from role {1}.", UserNameLabel.Text, selectedRoleName)
            Me.ActionStatus.ForeColor = Drawing.Color.DarkSeaGreen
        Catch ex As Exception

        End Try


    End Sub
#End Region


#Region "User task List"

    Private Sub CheckRolesForSelectedUser(ByVal selectedUserName As String)
        ' Determine what roles the selected user belongs to
        Dim selectedUsersRoles() As String = Roles.GetRolesForUser(selectedUserName)

        ' Loop through the Repeater's Items and check or uncheck the checkbox as needed
        For Each ri As ListItem In Me.UserRoleList.Items
            ' See if RoleCheckBox.Text is in selectedUsersRoles
            If System.Linq.Enumerable.Contains(Of String)(selectedUsersRoles, ri.Text) Then
                ri.Selected = True
            Else
                ri.Selected = False
            End If
        Next
    End Sub

    Private Sub unCheckRoles()
        For Each ri As ListItem In Me.UserRoleList.Items
            ri.Selected = False
        Next
    End Sub

    Protected Sub GetUser_Click(ByVal sender As Object, ByVal e As EventArgs) Handles GetUser.Click
        'get details and roles
        Try
            If (UserName.Text).Trim().Length = 0 Then
                ActionStatus.Text = "You must enter a username in the textbox."
                Exit Sub
            End If

            Dim vOfficer As clsOfficer.sOfficer = ldap1.getUserDetails(UserName.Text)
            If vOfficer.username = "" Then
                Throw New Exception("User not found!")
            End If
            'save session
            Session("officer") = vOfficer

            With vOfficer
                CheckRolesForSelectedUser(Me.UserName.Text)
                Me.UserDetails.Text = String.Format("Name:{0} {1} {2},  email:{3}.", .Firstname, .Initials, .Surname, .Email)
            End With
            UserName.Enabled = False
            Me.UpdateUser.Enabled = True
            Me.CancelUser.Enabled = True
            Me.GetUser.Enabled = False
        Catch ex As Exception
            ActionStatus.Text = ex.Message
            Me.ActionStatus.ForeColor = Drawing.Color.Red
            Me.UserDetails.Text = ""
        End Try
    End Sub

    Protected Sub SaveUser_Click(ByVal sender As Object, ByVal e As EventArgs) Handles UpdateUser.Click
        Try
            'get Ldap user from saved session
            Dim vLdapUser As clsOfficer.sOfficer = CType(Session("officer"), FlexTimeTable.clsOfficer.sOfficer)

            ''''add to membership first
            ' check if the user exists in membership
            Dim userInfo As MembershipUser = Membership.GetUser(vLdapUser.username)
            If userInfo Is Nothing Then
                'create new membership
                Dim dummyPassword As String = clsGeneral.getRandomStr(12)
                userInfo = Membership.CreateUser(UserName.Text, dummyPassword, vLdapUser.Email)
                userInfo.IsApproved = True
                Membership.UpdateUser(userInfo)
            End If

            ' update roles
            For Each ri As ListItem In Me.UserRoleList.Items
                If ri.Selected Then
                    ' add user to role
                    If Not Roles.IsUserInRole(vLdapUser.username, ri.Text) Then
                        Roles.AddUserToRole(vLdapUser.username, ri.Text)
                    End If
                Else
                    ' Remove the user from the role
                    If Roles.IsUserInRole(vLdapUser.username, ri.Text) Then
                        Roles.RemoveUserFromRole(vLdapUser.username, ri.Text)
                    End If
                End If
            Next

            Dim vReturn As FlexTimeTable.clsOfficer.sOfficerStatus = _
                           FlexTimeTable.clsOfficer.getOfficerID(vLdapUser)
            Select Case vReturn.Status
                Case FlexTimeTable.clsOfficer.eOfficerStatus.matched
                    'nothing to be done. Everything is in order
                Case FlexTimeTable.clsOfficer.eOfficerStatus.unmatched
                    'create officer record
                    FlexTimeTable.clsOfficer.CreateOfficer(vLdapUser)
                Case FlexTimeTable.clsOfficer.eOfficerStatus.unlinked
                    ' link record to membership
                    FlexTimeTable.clsOfficer.LinkOfficer(vReturn.ID, vLdapUser.username)
                Case FlexTimeTable.clsOfficer.eOfficerStatus.linked
                    'this is an error. need to alert
                    Throw New Exception("User might exist with a different username!")
            End Select

            'refresh list box
            DisplayUsersBelongingToRole()

            'message
            Me.ActionStatus.ForeColor = Drawing.Color.DarkSeaGreen
            Me.ActionStatus.Text = UserName.Text + " updated"
        Catch ex As Exception
            Me.ActionStatus.ForeColor = Drawing.Color.Red
            Me.ActionStatus.Text = ex.Message
        End Try

    End Sub

    Protected Sub CancelUser_Click(ByVal sender As Object, ByVal e As EventArgs) Handles CancelUser.Click
        unCheckRoles()
        Me.UserDetails.Text = ""
        UserName.Enabled = True
        GetUser.Enabled = True
        UpdateUser.Enabled = False
        CancelUser.Enabled = False
    End Sub
#End Region


End Class