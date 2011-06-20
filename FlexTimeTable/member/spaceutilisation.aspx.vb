Public Class spaceutilisation
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            getcampus()
            getResourceType()
            getOffering()
            GetUtilisation()
        End If
    End Sub

    Sub getcampus()
        Dim vContext As timetableEntities = New timetableEntities()
        Dim campuses = (From p In vContext.campus Select name = p.longName, id = p.ID).ToList
        With cboCampus
            .DataSource = campuses
            .DataTextField = "name"
            .DataValueField = "id"
            .DataBind()
        End With
    End Sub

    Sub getOffering()
        Dim vContext As timetableEntities = New timetableEntities()
        Dim voffering = (From p In vContext.offeringtypes Select name = p.name, id = p.ID).ToList
        With cboOfferingType
            .DataSource = voffering
            .DataTextField = "name"
            .DataValueField = "id"
            .DataBind()
        End With
    End Sub


    Sub getResourceType()
        Dim vContext As timetableEntities = New timetableEntities()
        Dim restypes = (From p In vContext.resourcetypes Select name = p.code, id = p.ID).ToList
        With cboType
            .DataSource = restypes
            .DataTextField = "name"
            .DataValueField = "id"
            .DataBind()
        End With
    End Sub


    Sub GetUtilisation()
        Dim vContext As timetableEntities = New timetableEntities()
        Dim dt As DataTable = New DataTable()
        ' Create Columns
        dt.Columns.Add("Room", System.Type.GetType("System.String"))
        dt.Columns.Add("Type", System.Type.GetType("System.String"))
        dt.Columns.Add("Allocated", System.Type.GetType("System.Int32"))
        dt.Columns.Add("Available", System.Type.GetType("System.Int32"))
        dt.Columns.Add("Percent", System.Type.GetType("System.Int32"))
        Dim dr As DataRow
        Dim CampuID = CInt(cboCampus.SelectedValue)
        Dim TypeID = CInt(cboType.SelectedValue)
        Dim OfferingId = CInt(cboOfferingType.SelectedValue)
        Dim vOffering = (From p In vContext.offeringtypes Where p.ID = OfferingId Select p).First
        Dim vvenues = (From p In vContext.venues
                        Order By p.Code, p.building.shortName, p.building.site.shortName
                        Where p.building.site.sitecluster.CampusID = CampuID And
                              p.resourcetype.ID = TypeID
                              Select p).ToList
        Dim vYear = Year(Now)
        Dim vWeek = DatePart(DateInterval.WeekOfYear, Now)
        Dim startslot = vOffering.startTimeSlot
        Dim endslot = vOffering.endTimeSlot
        Dim TotalSlots = ((vOffering.endTimeSlot - vOffering.startTimeSlot) + 1) * 5
        Dim slotCount = 0
        Dim Usedslots = 0
        Dim novenues = 0
        For Each x In vvenues
            novenues = novenues + 1
            With x
                Dim slots = Aggregate p In x.resourceschedules
                               Where p.Year = vYear And p.Week = vWeek And
                                     p.timeSlotID >= startslot And p.timeSlotID <= endslot
                                Into Count()
                dr = dt.NewRow
                dr("Room") = x.Code + "," + x.building.shortName + "," + x.building.site.shortName
                dr("Type") = x.resourcetype.code
                dr("Allocated") = slots
                dr("Available") = (TotalSlots - slots)
                dr("Percent") = CInt(slots * 100 / TotalSlots)
                dt.Rows.Add(dr)
                slotCount = slotCount + slots
                novenues = novenues + 1
            End With
        Next
        Try
            litSummary.Text = "No of venues:" + novenues.ToString + "<br/>" +
                              "Timeslots Per Venue:" + TotalSlots.ToString + "<br/>" +
                              "Total TimeSlots:" + (novenues * TotalSlots).ToString + "<br/>" +
                              "Utilisation:=" + CInt(slotCount / (TotalSlots * novenues)).ToString + "%"
        Catch ex As Exception
            litSummary.Text = ""
        End Try
        Dim dv = New DataView(dt, "", "Percent Desc", DataViewRowState.CurrentRows)
        With grdUtilize
            .DataSource = dv
            '  .Sort("Percent", SortDirection.Ascending)
            .DataBind()
        End With

    End Sub


    Protected Sub cboCampus_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles cboCampus.SelectedIndexChanged
        GetUtilisation()
    End Sub

    Protected Sub cboType_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles cboType.SelectedIndexChanged
        GetUtilisation()
    End Sub

    Protected Sub cboOfferingType_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles cboOfferingType.SelectedIndexChanged
        GetUtilisation()
    End Sub
End Class