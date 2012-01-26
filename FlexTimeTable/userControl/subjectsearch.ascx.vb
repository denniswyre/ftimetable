Imports System.Web.HttpContext
Partial Class subjectsearch
    Inherits System.Web.UI.UserControl

    Public Event SubjectClick(ByVal E As Object, ByVal Args As clsSubjectEvent)
    Public Event ExitClick(ByVal E As Object, ByVal Args As System.EventArgs)
    Private Sub btnReturn_Click(sender As Object, e As System.EventArgs) Handles btnReturn.Click
        RaiseEvent ExitClick(Me, e)
    End Sub
    Public Sub activateExit(ByVal Activate As Boolean)
        btnReturn.Visible = Activate
    End Sub


    Public Sub getSubjects()
        If optSearchType.SelectedIndex = 1 Then
            LoadSubjectsBySearch()
        Else
            loadByDepartment(ucDepartment.getID)
        End If
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            ucDepartment.loadFaculty(Nothing)
            optSearchType.SelectedIndex = 0
            displaySearchType()
            btnReturn.Visible = False
        End If
    End Sub

    Private Sub optSearchType_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles optSearchType.SelectedIndexChanged
        displaySearchType()
    End Sub

    Private Sub displaySearchType()
        If optSearchType.SelectedIndex = 1 Then
            pnlSearch.Visible = True
            ucDepartment.Visible = False
            loadByDepartment(0)
        Else
            pnlSearch.Visible = False
            ucDepartment.Visible = True
            loadByDepartment(ucDepartment.getID)
        End If
    End Sub


    Sub loadByDepartment(ByVal DepartmentID As Integer)
        Dim vContext As timetableEntities = New timetableEntities()
        If DepartmentID > 0 Then
            Me.grdsubject.DataSource = (From p In vContext.subjects Order By p.Code Where p.DepartmentID = DepartmentID Select p)
        Else
            Me.grdsubject.DataSource = Nothing
        End If
        Me.grdsubject.DataBind()
    End Sub


    Sub LoadSubjectsBySearch()
        Dim searchStr = txtSearchValue.Text
        If Len(Trim(searchStr)) = 0 Then
            loadByDepartment(0)
            Exit Sub
        End If
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vSubjects As New List(Of subject)
        'add new code search
        vSubjects = (From p In vContext.subjects Where p.Code.Contains(searchStr) Select p).ToList
        Dim ProperOldCodeText = clsGeneral.correctCode(searchStr)
        Dim oldcodes = (From p In vContext.oldsubjectcodes Where p.OldCode.Contains(ProperOldCodeText) Select p)
        'add old code search
        For Each x In oldcodes.ToList
            vSubjects.Add(x.subject)
        Next
        'add name search
        Dim NameSubjectSearch = (From p In vContext.subjects Where p.longName.Contains(searchStr) Select p).ToList
        For Each x In NameSubjectSearch
            vSubjects.Add(x)
        Next

        Me.grdsubject.DataSource = (From p In vSubjects Distinct Order By p.Code Select p).ToList
        Me.grdsubject.DataBind()
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
        Dim Args As New clsSubjectEvent(vID)
        RaiseEvent SubjectClick(Me, Args)
        lblMessage.Text = ""
    End Sub


    Protected Sub btnGet_Click(sender As Object, e As EventArgs) Handles btnGet.Click
        getSubjects()  'LoadSubjectsBySearch()
    End Sub


    Private Sub ucDepartment_DepartmentClick(E As Object, Args As clsDepartmentEvent) Handles ucDepartment.DepartmentClick
        getSubjects() '   loadByDepartment(Args.mDepartmentID)
    End Sub

    
End Class