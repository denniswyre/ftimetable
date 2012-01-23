Imports System.Web.HttpContext
Partial Class departmentSearch
    Inherits System.Web.UI.UserControl

    Public Event DepartmentClick(ByVal E As Object, ByVal Args As clsDepartmentEvent)

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            ucGetSchool.loadFaculty("")
            optSearchType.SelectedIndex = 0
            displaySearchType()
        End If
    End Sub

    Private Sub optSearchType_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles optSearchType.SelectedIndexChanged
        displaySearchType()
    End Sub

    Private Sub displaySearchType()
        If optSearchType.SelectedIndex = 1 Then
            pnlSearch.Visible = True
            ucGetSchool.Visible = False
            loadBySchool(0)
        Else
            pnlSearch.Visible = False
            ucGetSchool.Visible = True
            loadBySchool(ucGetSchool.getID)
        End If
    End Sub

    
    Sub loadBySchool(ByVal schoolID As Integer)
        Dim vContext As timetableEntities = New timetableEntities()
        Me.grdDepartment.DataSource = (From p In vContext.departments Order By p.code
                                          Where p.SchoolID = schoolID _
                                            Select p)
        Me.grdDepartment.DataBind()
        lblMessage.Text = ""
    End Sub

    Sub LoadDepartmentsBySearch()
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vDepartments As New List(Of department)
        ' code search
        vDepartments = (From p In vContext.departments Where p.code.Contains(txtSearchValue.Text) Select p).ToList

        'add name search
        Dim NameDepartmentSearch = (From p In vContext.departments Where p.longName.Contains(txtSearchValue.Text) Select p).ToList
        For Each x In NameDepartmentSearch
            vDepartments.Add(x)
        Next

        Me.grdDepartment.DataSource = (From p In vDepartments Distinct Order By p.code Select p).ToList
        Me.grdDepartment.DataBind()
        lblMessage.Text = ""
    End Sub


    Private Sub grdDepartment_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles grdDepartment.SelectedIndexChanged
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vID = CType(grdDepartment.SelectedRow.Cells(0).Text, Integer)
        Dim Args As New clsDepartmentEvent(vID)
        RaiseEvent DepartmentClick(Me, Args)
        lblMessage.Text = ""
    End Sub


    Protected Sub btnGet_Click(sender As Object, e As EventArgs) Handles btnGet.Click
        LoadDepartmentsBySearch()
    End Sub


    Private Sub ucGetSchool_SchoolClick(E As Object, Args As clsSchoolEvent) Handles ucGetSchool.SchoolClick
        loadBySchool(Args.mSchoolD)
    End Sub
End Class