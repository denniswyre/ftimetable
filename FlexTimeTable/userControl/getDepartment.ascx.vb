Public Class getDepartment
    Inherits System.Web.UI.UserControl

    Public Event DepartmentClick(ByVal E As Object, ByVal Args As clsDepartmentEvent)
    Sub generateEvent()
        Dim Args As New clsDepartmentEvent(CInt(cboDepartments.SelectedValue))
        RaiseEvent DepartmentClick(Me, Args)
    End Sub

    Public Function getID() As Integer
        Return CInt(cboDepartments.SelectedValue)
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

        loadDepartments()
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
        If AlignVertical Then
            LitStart.Text = "<table><tr><td " + stylewidth + ">Department:</td><td>"
            LitMiddle.Text = "</td><tr><td>Campus:</td><td>"
            LitEnd.Text = "</td></tr></table>"
        Else
            LitStart.Text = "<table><tr><td " + stylewidth + ">Department:</td><td>"
            LitMiddle.Text = ""
            LitEnd.Text = "</td></tr></table>"
        End If

    End Sub

    Sub loadDepartments()
        Dim vContext As timetableEntities = New timetableEntities
        If cboFaculty.SelectedIndex >= 0 Then
            Dim FacultyID = CType(Me.cboFaculty.SelectedValue, Integer)
            Me.cboDepartments.DataSource = (From p In vContext.departments _
                                Where p.school.facultyID = FacultyID _
                                Order By p.school.longName, p.longName _
                                  Select longName = (p.longName + "," + p.school.shortName), p.ID)
        Else
            Me.cboDepartments.DataSource = Nothing
        End If
        Me.cboDepartments.DataTextField = "longName"
        Me.cboDepartments.DataValueField = "ID"
        Me.cboDepartments.DataBind()
        generateEvent()
    End Sub

    Private Sub cboFaculty_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cboFaculty.SelectedIndexChanged
        loadDepartments()
    End Sub


    Private Sub cboDepartments_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles cboDepartments.SelectedIndexChanged
        generateEvent()
    End Sub

   
End Class