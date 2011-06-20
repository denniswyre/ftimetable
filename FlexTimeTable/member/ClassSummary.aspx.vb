Public Class ClassSummary
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
        dt.Columns.Add("Class", System.Type.GetType("System.String"))
        dt.Columns.Add("Resource", System.Type.GetType("System.String"))
        dt.Columns.Add("Periods", System.Type.GetType("System.Int32"))
        dt.Columns.Add("Assigned", System.Type.GetType("System.Int32"))
        dt.Columns.Add("Required", System.Type.GetType("System.Int32"))
        Dim dr As DataRow
        Dim DepartmentID = CType(cboDepartments.SelectedValue, Integer)
        Dim vClasses = (From p In vContext.classgroups
                            Order By p.siteclustersubject.subject.longName
                                    Where p.siteclustersubject.subject.DepartmentID = DepartmentID
                                       Select p).ToList

        Dim vYear = Year(Now)
        Dim vWeek = DatePart(DateInterval.WeekOfYear, Now)
        Dim vcount = 0
        Dim needed = 0
        Dim assigned = 0
        For Each x In vClasses
            For Each z In x.resources
                Dim slots = Aggregate p In z.resourceschedules
                                   Where p.Year = vYear And p.Week = vWeek
                                        Into Count()
                assigned = assigned + slots
                needed = needed + z.AmtTimeSlots
                dr = dt.NewRow
                dr("Class") = z.Name
                dr("Resource") = z.resourcetype.code
                dr("Periods") = z.AmtTimeSlots
                dr("Assigned") = slots
                dr("Required") = z.AmtTimeSlots - slots
                dt.Rows.Add(dr)
            Next
        Next
        Dim dv = New DataView(dt, "", "Required Desc", DataViewRowState.CurrentRows)
        With grdClasses
            .DataSource = dv
            .DataBind()
        End With
        litSummary.Text = "Number of Timeslots Required: " + needed.ToString + "<br/>Assigned Timeslots:" + assigned.ToString + "<br/>Amount Required:" + Format(needed - assigned, "#,###") + "<br/>Percent Allocated:" + Format((assigned * 100) / needed, "#,###.0")

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