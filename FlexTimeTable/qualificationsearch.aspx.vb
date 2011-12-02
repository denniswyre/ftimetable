Public Class qualificationsearch
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            mvQual.SetActiveView(vwGrid)
            optSearchType.SelectedIndex = 0
        End If
    End Sub


    Sub loadQualifications()
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vQuals As New List(Of qualification)
        Select Case optSearchType.SelectedIndex
            Case 0   ' new code
                vQuals = (From p In vContext.qualifications Where p.Code.Contains(txtSearchValue.Text) Select p).ToList
            Case 1   ' old code
                Dim newSearchCode = clsGeneral.correctCode(txtSearchValue.Text)
                Dim oldcodes = (From p In vContext.oldqualificationcodes Where p.oldCode.Contains(newSearchCode) Select p)
                For Each x In oldcodes.ToList
                    vQuals.Add(x.qualification)
                Next
            Case 2   ' name
                vQuals = (From p In vContext.qualifications Where p.longName.Contains(txtSearchValue.Text) Select p).ToList
        End Select
        Me.grdQualification.DataSource = vQuals
        Me.grdQualification.DataBind()
        mvQual.SetActiveView(vwGrid)
        litMessage.Text = ""
    End Sub

    Function displayOldCodes(ByVal QualID As Object) As String
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vQualID = CType(QualID, Integer)
        Dim oldcodes = (From p In vContext.oldqualificationcodes Where p.QualID = vQualID Select p).ToList
        Dim vText = ""
        For Each x In oldcodes
            If vText = "" Then
                vText = x.OldCode
            Else
                vText = vText + ", " + x.OldCode
            End If
        Next
        Return vText
    End Function

    Private Sub grdQualification_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles grdQualification.SelectedIndexChanged
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vID = CType(grdQualification.SelectedRow.Cells(0).Text, Integer)
        Dim vQual As qualification = _
                   (From p In vContext.qualifications _
                       Where p.ID = vID Select p).First
        With vQual
            litQual.Text = "<b>New Code:</b>" + .Code +
                           "<br/><b>Old Code:</b>" + displayOldCodes(.ID) +
                           "<br/><b>Short Name:</b>" + .shortName +
                           "<br/><b>Long Name:</b>" + .longName
        End With
        mvQual.SetActiveView(vwView)
        litMessage.Text = ""
    End Sub


    Protected Sub btnCancel_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnCancel.Click
        mvQual.SetActiveView(vwGrid)
        litMessage.Text = ""
    End Sub


    Protected Sub btnGet_Click(sender As Object, e As EventArgs) Handles btnGet.Click
        loadQualifications()
    End Sub
End Class