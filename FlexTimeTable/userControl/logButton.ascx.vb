Imports System.Web.HttpContext
Partial Class logButton
    Inherits System.Web.UI.UserControl

    Public Event Click As System.EventHandler

    Public WriteOnly Property Description() As String
        Set(ByVal value As String)
            ViewState("Description") = value
        End Set
    End Property

    Public WriteOnly Property [Function]() As String
        Set(ByVal value As String)
            ViewState("Function") = value
        End Set
    End Property

    Public Property Enabled() As Boolean
        Get
            Return Button.Enabled
        End Get
        Set(ByVal value As Boolean)
            Button.Enabled = value
        End Set
    End Property

    Public Property Width() As Unit
        Get
            Return Button.Width
        End Get
        Set(ByVal value As Unit)
            Button.Width = value
        End Set
    End Property

    Public Property Height() As Unit
        Get
            Return Button.Height
        End Get
        Set(ByVal value As Unit)
            Button.Height = value
        End Set
    End Property

    Public WriteOnly Property Text() As String
        Set(ByVal value As String)
            Button.Text = value
        End Set
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            ViewState("Description") = ""
            ViewState("Function") = ""
        End If
    End Sub


    Private Sub Button_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Button.Click
        RaiseEvent Click(Me, New EventArgs())
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vLogEntry As New userlog With {
            .page = Request.Path,
            .ipaddress = Request.UserHostAddress,
            .description = CType(ViewState("Description"), String),
            .fdatetime = DateTime.Now,
            .function = CType(ViewState("Function"), String),
            .user = Current.User.Identity.Name}
        vContext.userlogs.AddObject(vLogEntry)
        vContext.SaveChanges()
    End Sub
End Class