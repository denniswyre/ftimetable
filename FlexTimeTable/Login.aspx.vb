Public Class Login
    Inherits System.Web.UI.Page
    Const cntAdminUser As String = "flexTimeTableAdministrator"
    Const cntAdminRole As String = "SystemAdmin"

    Private Sub UserLogin()
        Dim vIsNewUser As Boolean = False
        Dim vOfficer As New clsOfficer.sOfficer

        Try
            'Dim vLdap As New WebUserAccess()
            '            GoTo tempLine

            vOfficer = ldap1.getUserDetails(Me.UserName.Text)
            If vOfficer.username = "" Then
                Throw New Exception("Invalid Credentials!")
            End If
            If IsNothing(ldap1.BindLdap(vOfficer.DistinquishedName, Me.Password.Text)) Then
                Throw New Exception("Invalid Credentials!")
            End If
            'tempLine:


            Dim vUser As MembershipUser = Membership.GetUser(Me.UserName.Text)
            If vUser Is Nothing Then
                'create new membership
                Dim dummyPassword As String = clsGeneral.getRandomStr(12)
                vIsNewUser = True
                vUser = Membership.CreateUser(UserName.Text, dummyPassword, vOfficer.Email)
                vUser.IsApproved = True
                Membership.UpdateUser(vUser)
            End If

            If vUser.IsLockedOut Then
                Throw New OverflowException("Your Account is disabled! Contact the IT Department")
            End If
            FormsAuthentication.RedirectFromLoginPage(UserName.Text, Me.RememberMe.Checked)
        Catch ex As Exception
            FailureText.Text = ex.Message
        End Try

    End Sub

    Sub adminLogin()
        Try
            If Not FormsAuthentication.Authenticate(cntAdminUser, Me.Password.Text) Then
                Throw New Exception("Invalid Password!!")
            End If
            '''''''''''check role
            If Not Roles.IsUserInRole(cntAdminUser, cntAdminRole) Then
                Roles.AddUserToRole(cntAdminUser, cntAdminRole)
            End If
            FormsAuthentication.RedirectFromLoginPage(UserName.Text, False)
        Catch ex As Exception
            FailureText.Text = ex.Message
        End Try

    End Sub

    Private Sub chkAdmin_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkAdmin.CheckedChanged
        Me.UserName.Enabled = Not chkAdmin.Checked
        UserName.Text = CType(IIf(chkAdmin.Checked, cntAdminUser, String.Empty), String)
        Me.RememberMe.Visible = Not chkAdmin.Checked
    End Sub

    Protected Sub LoginButton_Click(ByVal sender As Object, ByVal e As EventArgs) Handles LoginButton.Click
        If Page.IsValid Then
            If Me.chkAdmin.Checked Then
                adminLogin()
            Else
                UserLogin()
            End If
        End If
    End Sub

End Class