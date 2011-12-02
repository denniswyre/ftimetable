Public Class NormalSubject
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

    Private Sub btnNormize_Click(sender As Object, e As System.EventArgs) Handles btnNormize.Click
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vsubjectList = (From p In vContext.subjects Select p).ToList
        For Each x In vsubjectList
            x.longName = clsGeneral.NormalizeString(x.longName)
            x.shortName = clsGeneral.NormalizeString(x.shortName)
            vContext.SaveChanges()
        Next
    End Sub

    Private Sub btnTest_Click(sender As Object, e As System.EventArgs) Handles btnTest.Click
        litResult.Text = clsGeneral.NormalizeString(txtValue.Text)
    End Sub
End Class