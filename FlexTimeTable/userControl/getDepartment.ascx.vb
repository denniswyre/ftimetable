Public Class getDepartment
    Inherits System.Web.UI.UserControl

    Public Event DepartmentClick(ByVal E As Object, ByVal Args As clsDepartmentEvent)

    Sub generateEvent()
        Dim DepartID As Integer
        If cboDepartment.SelectedIndex < 0 Then
            DepartID = 0
        Else
            DepartID = CInt(cboDepartment.SelectedValue)
        End If
        Dim Args As New clsDepartmentEvent(DepartID)
        RaiseEvent DepartmentClick(Me, Args)
    End Sub

    Public Function getID() As Integer
        Return CInt(cboDepartment.SelectedValue)
    End Function

    Public Sub loadFaculty(ByVal username As String)
        Dim vContext As timetableEntities = New timetableEntities()
        Dim OfficerID As Integer = clsOfficer.getOfficer(username).ID
        Me.cboFaculty.DataSource = (From p In vContext.facultyusers _
                                     Where p.OfficerID = OfficerID _
                                       Select p.FacultyCode, p.FacultyID)
        Me.cboFaculty.DataTextField = "FacultyCode"
        Me.cboFaculty.DataValueField = "FacultyID"
        Me.cboFaculty.DataBind()
        loadSchools()
    End Sub

    Private Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            SetLabel(False)
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


    Sub loadSchools()
        Dim vContext As timetableEntities = New timetableEntities
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
        loadDepartments()
    End Sub

    Sub loadDepartments()
        Dim vContext As timetableEntities = New timetableEntities
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
        generateEvent()
    End Sub

    Private Sub cboFaculty_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cboFaculty.SelectedIndexChanged
        loadSchools()
    End Sub


    Private Sub cboDepartment_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles cboDepartment.SelectedIndexChanged
        generateEvent()
    End Sub

   
    Private Sub cboSchool_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles cboSchool.SelectedIndexChanged
        loadDepartments()
    End Sub
End Class