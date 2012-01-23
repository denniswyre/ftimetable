Partial Class getSchool
    Inherits System.Web.UI.UserControl

    Public Event SchoolClick(ByVal E As Object, ByVal Args As clsSchoolEvent)


    Public WriteOnly Property enabled As Boolean
        Set(value As Boolean)
            cboFaculty.Enabled = value
            cboSchool.Enabled = value
        End Set
    End Property



    Sub generateEvent()
        Dim SchoolID = 0
        Try
            SchoolID = CInt(cboSchool.SelectedValue)
        Catch ex As Exception
        End Try
        Session("SchoolID") = CStr(SchoolID)
        Dim Args As New clsSchoolEvent(SchoolID)
        RaiseEvent SchoolClick(Me, Args)
    End Sub

    Enum eType
        faculty
        school
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
                End Select
            End If
        Catch ex As Exception
        End Try
    End Sub

    Public Sub setID(ByVal vID As Integer)
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vSchool = (From p In vContext.schools Where p.id = vID Select p).Single
        cboFaculty.SelectedValue = CStr(vSchool.facultyID)
        cboSchool.SelectedValue = CStr(vSchool.id)
    End Sub

    Public Function getID() As Integer
        Dim vID = 0
        Try
            vID = CInt(cboSchool.SelectedValue)
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
        generateEvent()
    End Sub

    Private Sub cboFaculty_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cboFaculty.SelectedIndexChanged
        loadSchools(False)
    End Sub


    Private Sub cboSchool_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles cboSchool.SelectedIndexChanged
        generateEvent()
    End Sub
End Class