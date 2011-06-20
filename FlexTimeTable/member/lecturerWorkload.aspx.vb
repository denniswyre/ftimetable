Public Class lecturerWorkload
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            loadFaculty()
        End If
    End Sub


    Sub loadFaculty()
        Dim vContext As timetableEntities = New timetableEntities()
        Dim OfficerID As Integer = clsOfficer.getOfficer(User.Identity.Name).ID
        Me.cboFaculty.DataSource = (From p In vContext.facultyusers _
                                     Where p.OfficerID = OfficerID _
                                       Select p.FacultyName, p.FacultyID)
        Me.cboFaculty.DataTextField = "FacultyName"
        Me.cboFaculty.DataValueField = "FacultyID"
        Me.cboFaculty.DataBind()

        loadDepartments()
    End Sub

    Sub loadDepartments()
        Dim vContext As timetableEntities = New timetableEntities
        If cboFaculty.SelectedIndex >= 0 Then
            Dim FacultyID = CType(Me.cboFaculty.SelectedValue, Integer)
            Me.cboDepartments.DataSource = (From p In vContext.departments _
                                Where p.school.facultyID = FacultyID _
                                Order By p.school.longName, p.longName _
                                  Select longName = (p.longName + "," + p.school.longName), p.ID)
        Else
            Me.cboDepartments.DataSource = Nothing
        End If
        Me.cboDepartments.DataTextField = "longName"
        Me.cboDepartments.DataValueField = "ID"
        Me.cboDepartments.DataBind()
        GetUtilisation()
    End Sub

    Sub GetUtilisation()
        Dim vContext As timetableEntities = New timetableEntities()
        Dim dt As DataTable = New DataTable()
        ' Create Columns
        dt.Columns.Add("Lecturer", System.Type.GetType("System.String"))
        dt.Columns.Add("Assigned", System.Type.GetType("System.Int32"))
        Dim dr As DataRow
        Dim DepartmentID = CType(cboDepartments.SelectedValue, Integer)
        Dim vLecturers = (From p In vContext.lecturers
                            Order By p.officer.Surname, p.officer.FirstName
                                    Where p.DepartmentID = DepartmentID
                                       Select p).ToList

        Dim vYear = Year(Now)
        Dim vWeek = DatePart(DateInterval.WeekOfYear, Now)
        Dim vcount = vLecturers.Count
        Dim noSlots = 0

        For Each x In vLecturers
            Dim slotCount = 0
            For Each y In x.classgroups
                For Each z In y.resources
                    With z
                        Dim slots = Aggregate p In z.resourceschedules
                                       Where p.Year = vYear And p.Week = vWeek
                                            Into Count()
                        slotCount = slotCount + slots
                    End With
                Next
            Next
            noSlots = noSlots + slotCount
            dr = dt.NewRow
            dr("Lecturer") = x.officer.Surname + "," + x.officer.FirstName
            dr("Assigned") = slotCount
            dt.Rows.Add(dr)
        Next
        Dim dv = New DataView(dt, "", "Assigned Desc", DataViewRowState.CurrentRows)
        With grdLecturer
            .DataSource = dv
            .DataBind()
        End With
        litSummary.Text = " No of Lecturers:" + vcount.ToString + " Avg Workload:" + Format(noSlots / vcount, "#,###")

    End Sub

    Private Sub cboFaculty_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cboFaculty.SelectedIndexChanged
        litMessage.Text = ""
        loadDepartments()
    End Sub

    Private Sub cboDepartments_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cboDepartments.SelectedIndexChanged
        litMessage.Text = ""
        GetUtilisation()
    End Sub
End Class