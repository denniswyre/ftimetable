'Imports OpenPop.Pop3.Pop3Client
'Imports OpenPop.Mime.Message
Imports System.Net.Mail

Public Class BulkEmail
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

    Protected Sub btnSend_Click(sender As Object, e As EventArgs) Handles btnSend.Click
        Try
            Dim vContext As timetableEntities = New timetableEntities()
            Dim users = (From p In vContext.officers Select p).ToList
            For Each x In users
                '   x.my_aspnet_users.name
                Dim toAdd As New MailAddress(x.my_aspnet_users.name + "@wsu.ac.za")
                Dim frAdd As New MailAddress("sms@wsu.ac.za")
                Dim MailMsg As New MailMessage(frAdd, toAdd)
                With MailMsg
                    .BodyEncoding = Encoding.Default
                    .Subject = txtSubject.Text
                    .Body = txtBody.Text
                    .Priority = MailPriority.High
                    .IsBodyHtml = False
                End With
                'Smtpclient to send the mail message
                Dim SmtpMail As New SmtpClient
                With SmtpMail
                    .Host = "pop3.wsu.ac.za"
                    .Send(MailMsg)
                End With
                ErrorMessage.Text = ""
            Next
        Catch ex As Exception
            ErrorMessage.Text = clsGeneral.displaymessage(ex.Message, True)
        End Try

    End Sub
End Class