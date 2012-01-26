Imports System.Web.HttpContext
Partial Class qualificationsearch
    Inherits System.Web.UI.UserControl

    Public Event QualClick(ByVal E As Object, ByVal Args As clsQualEvent)
    Public Event ExitClick(ByVal E As Object, ByVal Args As System.EventArgs)
    Private Sub btnReturn_Click(sender As Object, e As System.EventArgs) Handles btnReturn.Click
        RaiseEvent ExitClick(Me, e)
    End Sub

    Public Sub activateExit(ByVal Activate As Boolean)
        btnReturn.Visible = Activate
    End Sub

    Public Sub getQualifications()
        If optSearchType.SelectedIndex = 1 Then
            loadQualificationsBySearch()
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
        Me.grdQualification.DataSource = (From p In vContext.qualifications Order By p.Code Where p.DepartmentID = DepartmentID Select p)
        Me.grdQualification.DataBind()
    End Sub



    Sub loadQualificationsBySearch()
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vQuals As New List(Of qualification)
        'search  new code
        vQuals = (From p In vContext.qualifications Where p.Code.Contains(txtSearchValue.Text) Select p).ToList
        'search old code
        Dim newSearchCode = clsGeneral.correctCode(txtSearchValue.Text)
        Dim oldcodes = (From p In vContext.oldqualificationcodes Where p.oldCode.Contains(newSearchCode) Select p)
        For Each x In oldcodes.ToList
            vQuals.Add(x.qualification)
        Next
        'search by name
        Dim vNameQualsSearch = (From p In vContext.qualifications Where p.longName.Contains(txtSearchValue.Text) Select p).ToList
        For Each x In vNameQualsSearch
            vQuals.Add(x)
        Next

        Me.grdQualification.DataSource = (From p In vQuals Distinct Order By p.Code Select p).ToList
        Me.grdQualification.DataBind()
        litMessage.Text = ""
    End Sub

    Function displayOldCodes(ByVal QualID As Object) As String
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vQualID = CType(QualID, Integer)
        Dim oldcodes = (From p In vContext.oldqualificationcodes Where p.QualID = vQualID Select p).ToList
        Dim vText = ""
        For Each x In oldcodes
            If vText = "" Then
                vText = x.oldCode
            Else
                vText = vText + ", " + x.oldCode
            End If
        Next
        Return vText
    End Function

    Private Sub grdQualification_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles grdQualification.SelectedIndexChanged
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vID = CType(grdQualification.SelectedRow.Cells(0).Text, Integer)
        Dim Args As New clsQualEvent(vID)
        RaiseEvent QualClick(Me, Args)
        litMessage.Text = ""
    End Sub


    Protected Sub btnGet_Click(sender As Object, e As EventArgs) Handles btnGet.Click
        loadQualificationsBySearch()
    End Sub

    Private Sub ucDepartment_DepartmentClick(E As Object, Args As clsDepartmentEvent) Handles ucDepartment.DepartmentClick
        loadByDepartment(Args.mDepartmentID)
    End Sub
End Class