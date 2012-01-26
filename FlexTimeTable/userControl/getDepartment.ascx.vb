Public Class getDepartment
    Inherits System.Web.UI.UserControl

    Public Event DepartmentClick(ByVal E As Object, ByVal Args As clsDepartmentEvent)
    Public Event UpdateDepartment(ByVal e As Object, ByVal Args As System.EventArgs)

    Public WriteOnly Property enabled As Boolean
        Set(value As Boolean)
            cboFaculty.Enabled = value
            cboSchool.Enabled = value
            cboDepartment.Enabled = value
        End Set
    End Property

    Public Sub setUpdate(ByVal activateButton As Boolean)
        phUpdate.Visible = activateButton
    End Sub

    Sub generateEvent()
        Dim DepartID = 0
        Try
            DepartID = CInt(cboDepartment.SelectedValue)
        Catch ex As Exception
        End Try
        Session("departmentID") = CStr(DepartID)
        Dim Args As New clsDepartmentEvent(DepartID)
        RaiseEvent DepartmentClick(Me, Args)
    End Sub

    Enum eType
        faculty
        school
        department
    End Enum

    Private Sub SelectType(ByVal vType As eType)
        Try
            Dim DepartID = CInt(Session("departmentID"))
            Dim vContext As timetableEntities = New timetableEntities()
            Dim Depart = (From p In vContext.departments Where p.ID = DepartID Select p).Single
            If Not IsNothing(Depart) Then
                Select Case vType
                    Case eType.faculty
                        Dim vFacItem = cboFaculty.Items.FindByValue(CStr(Depart.school.facultyID))
                        If Not IsNothing(vFacItem) Then
                            cboFaculty.SelectedValue = vFacItem.Value
                        End If
                    Case eType.school
                        Dim vSchItem = cboSchool.Items.FindByValue(CStr(Depart.SchoolID))
                        If Not IsNothing(vSchItem) Then
                            cboSchool.SelectedValue = vSchItem.Value
                        End If
                    Case eType.department
                        Dim vDepItem = cboDepartment.Items.FindByValue(CStr(Depart.ID))
                        If Not IsNothing(vDepItem) Then
                            cboDepartment.SelectedValue = vDepItem.Value
                        End If
                End Select
            End If
        Catch ex As Exception
        End Try
    End Sub

    Public Sub setID(ByVal vID As Integer)
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vDepart = (From p In vContext.departments Where p.ID = vID Select p).Single
        cboFaculty.SelectedValue = CStr(vDepart.school.facultyID)
        cboSchool.SelectedValue = CStr(vDepart.SchoolID)
        cboDepartment.SelectedValue = CStr(vDepart.ID)
    End Sub

    Public Function getID() As Integer
        Dim vID = 0
        Try
            vID = CInt(cboDepartment.SelectedValue)
        Catch ex As Exception
            vID = 0
        End Try
        Return vID
    End Function



    Public Sub loadFaculty(ByVal username As String)
        Dim vContext As timetableEntities = New timetableEntities()
        If Len(username) = 0 Then
            Me.cboFaculty.DataSource = (From p In vContext.faculties
                                           Select code = p.code, ID = p.ID)
        Else

            Dim OfficerID As Integer = clsOfficer.getOfficer(username).ID
            Me.cboFaculty.DataSource = (From p In vContext.facultyusers _
                                       Where p.OfficerID = OfficerID _
                                         Select code = p.FacultyCode, id = p.FacultyID)
        End If
        Me.cboFaculty.DataTextField = "code"
        Me.cboFaculty.DataValueField = "id"
        Me.cboFaculty.DataBind()
        SelectType(eType.faculty)
        loadSchools(True)
    End Sub


    Private Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            SetLabel(False)
            phUpdate.Visible = False
        End If
    End Sub

    Public Sub SetLabel(ByVal AlignVertical As Boolean, Optional ByVal LabelWidth As Integer = 0)
        Dim stylewidth As String
        If LabelWidth = 0 Then
            stylewidth = ""
        Else
            stylewidth = "style=""" + "width: " + CStr(LabelWidth) + """"
        End If
    End Sub


    Sub loadSchools(ByVal SetDefault As Boolean)
        Dim vContext As timetableEntities = New timetableEntities
        Me.cboSchool.Items.Clear()
        If cboFaculty.SelectedIndex >= 0 Then
            Dim FacultyID = CType(Me.cboFaculty.SelectedValue, Integer)
            Me.cboSchool.DataSource = (From p In vContext.schools
                                Where p.facultyID = FacultyID
                                    Order By p.longName
                                        Select longName = p.longName, p.id)
        Else
            Me.cboSchool.DataSource = Nothing
        End If
        Me.cboSchool.DataTextField = "longName"
        Me.cboSchool.DataValueField = "ID"
        Me.cboSchool.DataBind()
        If SetDefault Then
            SelectType(eType.school)
        End If
        loadDepartments(SetDefault)
    End Sub

    Sub loadDepartments(ByVal SetDefault As Boolean)
        Dim vContext As timetableEntities = New timetableEntities
        cboDepartment.Items.Clear()
        If cboSchool.SelectedIndex >= 0 Then
            Dim SchoolID = CType(Me.cboSchool.SelectedValue, Integer)
            Me.cboDepartment.DataSource = (From p In vContext.departments
                                Where p.SchoolID = SchoolID
                                    Order By p.longName
                                        Select longName = p.longName, p.ID)
        Else
            Me.cboDepartment.DataSource = Nothing
        End If
        Me.cboDepartment.DataTextField = "longName"
        Me.cboDepartment.DataValueField = "ID"
        Me.cboDepartment.DataBind()
        If SetDefault Then
            SelectType(eType.department)
        End If
        generateEvent()
    End Sub

    Private Sub cboFaculty_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cboFaculty.SelectedIndexChanged
        loadSchools(False)
    End Sub


    Private Sub cboDepartment_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles cboDepartment.SelectedIndexChanged
        generateEvent()
    End Sub

    Private Sub cboSchool_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles cboSchool.SelectedIndexChanged
        loadDepartments(False)
    End Sub

    Private Sub btnUpdate_Click(sender As Object, e As System.EventArgs) Handles btnUpdate.Click
        RaiseEvent UpdateDepartment(Me, e)
    End Sub
End Class