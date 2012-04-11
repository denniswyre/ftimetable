Public Class ClassSummary
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            loadcampus()
            loadAcademicBlock()
            ucDepartment.setUpdate(False)
            ucDepartment.loadFaculty("")
        End If
    End Sub

    Sub loadcampus()
        Dim vContext As timetableEntities = New timetableEntities()
        With cboCampus
            .DataSource = (From p In vContext.campus
                                            Select p.longName, p.ID)
            .DataTextField = "longName"
            .DataValueField = "ID"
            .DataBind()
        End With
    End Sub

    Sub loadAcademicBlock()
        Dim vContext As timetableEntities = New timetableEntities()
        With cboBlock
            .DataSource = (From p In vContext.academicblocks
                                            Select p.Name, p.ID)
            .DataTextField = "Name"
            .DataValueField = "ID"
            .DataBind()
        End With
    End Sub

    Function getBlockID() As Integer
        Dim vContext As timetableEntities = New timetableEntities()
        'get last week in selected block
        Dim vweek = (From p In vContext.academicblocks Where p.ID = CInt(cboBlock.SelectedValue) Select p).Single.endWeek
        'use first block from all blocks with last week
        Return (From p In vContext.academicblocks Where p.startWeek <= vweek And p.endWeek >= vweek
                           Order By p.startWeek Descending, p.endWeek Ascending
                              Select p).First.ID
    End Function


    Sub GetUtilisation()
        Dim vContext As timetableEntities = New timetableEntities()
        Dim dt As DataTable = New DataTable()
        ' Create Columns
        dt.Columns.Add("Class", System.Type.GetType("System.String"))
        dt.Columns.Add("Resource", System.Type.GetType("System.String"))
        dt.Columns.Add("Timeslots", System.Type.GetType("System.Int32"))
        dt.Columns.Add("Assigned", System.Type.GetType("System.Int32"))
        dt.Columns.Add("Required", System.Type.GetType("System.Int32"))
        dt.Columns.Add("Reasons", System.Type.GetType("System.String"))
        Dim dr As DataRow
        Dim DepartmentID = ucDepartment.getID
        Dim CampusID = CInt(cboCampus.SelectedValue)
        Dim vClasses = (From p In vContext.classgroups
                            Order By p.siteclustersubject.subject.longName
        Where (p.siteclustersubject.subject.DepartmentID = DepartmentID) And
              (p.siteclustersubject.sitecluster.CampusID = CampusID)
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
                Dim resourceID = z.ID
                Dim reasons = (From p In vContext.timetablelogs
                                Where p.ResourceID = resourceID And
                                      p.AcademicYear = Year(Date.Now) And
                                      p.AcademicBlockID = CInt(cboBlock.SelectedValue)
                                      Select p).FirstOrDefault
                assigned = assigned + slots
                needed = needed + z.AmtTimeSlots
                dr = dt.NewRow
                dr("Class") = z.Name
                dr("Resource") = z.resourcetype.code
                dr("Timeslots") = z.AmtTimeSlots
                dr("Assigned") = slots
                dr("Required") = z.AmtTimeSlots - slots
                If IsNothing(reasons) Then
                    dr("Reasons") = ""
                Else
                    dr("Reasons") = clsGeneral.getlogTypeDescription(CType(reasons.ReasonID, clsGeneral.eLogType))
                End If
                dt.Rows.Add(dr)
            Next
        Next
        Dim dv = New DataView(dt, "", "Required Desc", DataViewRowState.CurrentRows)
        With grdClasses
            .DataSource = dv
            .DataBind()
        End With
        litSummary.Text = "Timeslots Required:____" + needed.ToString + "<br/>" +
                          "Assigned Timeslots:____" + assigned.ToString + "<br/>" +
                          "Unscheduled Timeslots:_" + Format(needed - assigned, "#,###") + "<br/>" +
                          "Percent Allocated:_____" + Format((assigned * 100) / needed, "#,###.0")

    End Sub


    Private Sub ucDepartment_DepartmentClick(E As Object, Args As clsDepartmentEvent) Handles ucDepartment.DepartmentClick
        litMessage.Text = ""
        GetUtilisation()
    End Sub

    Private Sub cboBlock_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles cboBlock.SelectedIndexChanged
        litMessage.Text = ""
        GetUtilisation()
    End Sub

    Private Sub cboCampus_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles cboCampus.SelectedIndexChanged
        litMessage.Text = ""
        GetUtilisation()
    End Sub
End Class