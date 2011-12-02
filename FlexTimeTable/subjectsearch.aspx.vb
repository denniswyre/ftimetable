Public Class subjectsearch
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            optSearchType.SelectedIndex = 0
            mvSubject.SetActiveView(vwGrid)
        End If
    End Sub


    Sub loadsubjects()
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vSubjects As New List(Of subject)
        Select Case optSearchType.SelectedIndex
            Case 0   ' new code
                vSubjects = (From p In vContext.subjects Where p.Code.Contains(txtSearchValue.Text) Select p).ToList
            Case 1   ' old code
                Dim newSearchCode = clsGeneral.correctCode(txtSearchValue.Text)
                Dim oldcodes = (From p In vContext.oldsubjectcodes Where p.OldCode.Contains(newSearchCode) Select p)
                For Each x In oldcodes.ToList
                    vSubjects.Add(x.subject)
                Next
            Case 2   ' name
                vSubjects = (From p In vContext.subjects Where p.longName.Contains(txtSearchValue.Text) Select p).ToList
        End Select
        Me.grdsubject.DataSource = vSubjects
        Me.grdsubject.DataBind()
        mvSubject.SetActiveView(vwGrid)
        lblMessage.Text = ""
    End Sub


    Function displayOldCodes(ByVal SubjectID As Object) As String
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vSubID = CType(SubjectID, Integer)
        Dim oldcodes = (From p In vContext.oldsubjectcodes Where p.SubjectID = vSubID Select p).ToList
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

    Private Sub grdsubject_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles grdsubject.SelectedIndexChanged
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vID = CType(grdsubject.SelectedRow.Cells(0).Text, Integer)
        Dim vSubject As subject = _
                   (From p In vContext.subjects _
                       Where p.ID = vID Select p).First
        With vSubject
            litsubjectdetails.Text = "<br/><b>New Subject Code:</b>" + .Code +
                                     "<br/><b>Old codes:</b>" + displayOldCodes(.ID) +
                                     "<br/><b>Short Name:</b>" + .shortName +
                                     "<br/><b>Long Name:</b>" + .longName +
                                     "<br/><b>Block:</b>" + CType(IIf(.yearBlock, "Year", "Semester"), String) +
                                     "<br/><b>Department:</b>" + .department.longName +
                                     "<br/><b>School:</b>" + .department.school.longName +
                                     "<br/><b>Faculty:</b>" + .department.school.faculty.code
        End With
        mvSubject.SetActiveView(vwView)
        lblMessage.Text = ""
    End Sub

    Enum eMode
        edit
        delete
        create
    End Enum


    Protected Sub btnCancel_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnCancel.Click
        mvSubject.SetActiveView(vwGrid)
        lblMessage.Text = ""
    End Sub


    Protected Sub btnGet_Click(sender As Object, e As EventArgs) Handles btnGet.Click
        loadsubjects()
    End Sub
End Class