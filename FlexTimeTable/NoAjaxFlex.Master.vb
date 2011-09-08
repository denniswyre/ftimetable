Imports System.Web.HttpContext
Partial Class NoAjaxFlex
    Inherits System.Web.UI.MasterPage

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            'log entry
            If (Current.User.Identity.IsAuthenticated) Then
                Dim vContext As timetableEntities = New timetableEntities()
                Dim vLogEntry As New userlog With {
                    .page = Request.Path,
                    .ipaddress = Request.UserHostAddress,
                    .description = "Loading Page",
                    .fdatetime = DateTime.Now,
                    .function = "Loading Page",
                    .user = Current.User.Identity.Name}
                vContext.userlogs.AddObject(vLogEntry)
                vContext.SaveChanges()

            End If
        End If
    End Sub

End Class