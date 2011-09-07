Imports Microsoft.VisualBasic
Public Class _Default
    Inherits System.Web.UI.Page

    Const cBreak = "<br/>"

    Structure sslot
        Dim year As Integer
        Dim week As Integer
        Dim day As Integer
        Dim timeslot As Integer
    End Structure

    Structure sSlotDetails
        Dim [class] As String
        Dim subject As String
        Dim venue As String
        Dim lecturer As String
    End Structure

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            Try
                mvTimetable.SetActiveView(vwDisplay)
                mvType.SetActiveView(vwQual)
                Calendar1.SelectedDate = Now
                showStatus()
                getlevels()
                getCluster()
                loadobjects()
                setTimeTableType()
                litMessage.Text = ""
            Catch ex As Exception
                litMessage.Text = clsGeneral.displaymessage(ex.Message, True)
            End Try
           
        End If
    End Sub

    Sub loadobjects()
        mvType.ActiveViewIndex = cboType.SelectedIndex
        Select Case mvType.ActiveViewIndex
            Case 0  'qual
                getQualification()
            Case 1   'class
                getSubjects()
            Case 2   'site
                getSite()
        End Select
    End Sub

    Sub getCluster()
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vCluster = (From p In vContext.siteclusters Select p).ToList
        With cboCluster
            .DataSource = vCluster
            .DataTextField = "shortName"
            .DataValueField = "ID"
            .DataBind()
        End With
    End Sub

    Sub getQualification()
        If cboCluster.SelectedIndex < 0 Then
            Throw New OverflowException("No Cluster available")
        End If
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vClusterID = CInt(cboCluster.SelectedValue)
        Dim vCluster = (From p In vContext.siteclusters Where p.ID = vClusterID Select p).Single
        Dim vQual = vCluster.qualifications
        With cboQual
            .DataSource = vQual
            .DataTextField = "shortname"
            .DataValueField = "ID"
            .DataBind()
        End With
    End Sub

    Sub getSubjects()
        If cboCluster.SelectedIndex < 0 Then
            Throw New OverflowException("No Cluster available")
        End If
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vClusterID = CInt(cboCluster.SelectedValue)
        Dim vCluster = (From p In vContext.siteclusters Where p.ID = vClusterID Select p).Single
        Dim vSubject = (From p In vContext.siteclustersubjects Where p.SiteClusterID = vClusterID Select code = (p.subject.Code), id = p.SubjectID).ToList
        With cboSubject
            .DataSource = vSubject
            .DataTextField = "code"
            .DataValueField = "id"
            .DataBind()
        End With
        getClasses()
    End Sub

    Sub getSite()
        If cboCluster.SelectedIndex < 0 Then
            Throw New OverflowException("No Cluster available")
        End If
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vClusterID = CInt(cboCluster.SelectedValue)
        Dim vSites = (From p In vContext.sites Where p.SiteClusterID = vClusterID Select p).ToList
        With cboSite
            .DataSource = vSites
            .DataTextField = "shortName"
            .DataValueField = "id"
            .DataBind()
        End With
        getRooms()
    End Sub

    Sub getClasses()
        If cboSubject.SelectedIndex > -1 Then
            Dim vContext As timetableEntities = New timetableEntities()
            Dim vSubjectID = CInt(cboSubject.SelectedValue)
            Dim vClusterID = CInt(cboCluster.SelectedValue)
            Dim vClasses = (From p In vContext.classgroups
                             Where p.SubjectID = vSubjectID And
                                   p.SiteClusterID = vClusterID Select p).ToList
            With cboClassgroup
                .DataSource = vClasses
                .DataTextField = "code"
                .DataValueField = "ID"
                .DataBind()
            End With
        Else
            cboClassgroup.Items.Clear()
        End If
    End Sub

    Sub getRooms()
        If cboSite.SelectedIndex > -1 Then
            Dim vContext As timetableEntities = New timetableEntities()
            Dim vSiteID = CInt(cboSite.SelectedValue)
            Dim vRooms = (From p In vContext.venues
                           Where p.building.SiteID = vSiteID Select code = p.Code, id = p.ID).ToList
            With cboRoom
                .DataSource = vRooms
                .DataTextField = "code"
                .DataValueField = "id"
                .DataBind()
            End With
        Else
            cboRoom.Items.Clear()
        End If
    End Sub

    Sub getlevels()
        For i = 1 To 6
            cboLevel.Items.Add(i.ToString)
        Next
    End Sub

    Function DisplayTimeHeader(ByVal TimeSlotID As Integer) As String
        Dim vContext As timetableEntities = New timetableEntities()
        Dim TimeSlot = (From p In vContext.timeslots Where p.ID = TimeSlotID Select p).First
        With TimeSlot
            If .LunchPeriod = True Then
                Return "Lunch"
            Else
                Return Format(.StartTime, "HH:mm") + "-" + _
                       Format(DateAdd(DateInterval.Minute, .Duration, .StartTime), "HH:mm")
            End If
        End With
    End Function

    Private Sub grdTimeTable_RowCommand(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewCommandEventArgs) Handles grdTimeTable.RowCommand
        Dim vContext As timetableEntities = New timetableEntities()
        Dim currentRowIndex As Integer = Int32.Parse(CStr(e.CommandArgument))
        Dim vSlot As New sslot With
            {
                .timeslot = CInt(grdTimeTable.DataKeys(currentRowIndex).Value),
                .day = CInt(e.CommandName),
                .week = DatePart(DateInterval.WeekOfYear, Calendar1.SelectedDate),
                .year = DatePart(DateInterval.Year, Calendar1.SelectedDate)
            }
        Select Case CInt(hdnType.Value)
            Case 0 'qual
                Dim vClusterID = CInt(lblCluster.ToolTip)
                Dim vQualID = CInt(lblObject0.ToolTip)
                Dim vLevel = CInt(lblObject1.ToolTip)
                displayQualDetails(vQualID, vLevel, vClusterID, vSlot)
            Case 1 'class
                Dim vClassID = CInt(lblObject0.ToolTip)
                displayClassDetails(vClassID, vSlot)
            Case 2 'venue
                Dim vVenueID = CInt(lblObject0.ToolTip)
                displayVenueDetails(vVenueID, vSlot)
        End Select
        mvTimetable.SetActiveView(vwDetail)
    End Sub


    Private Sub displayQualDetails(ByVal vQualID As Integer, ByVal vlevel As Integer, ByVal vClusterID As Integer, ByVal vSlot As sslot)
        Dim vContext As timetableEntities = New timetableEntities()

        Dim vQual = (From p In vContext.qualifications
                          Where p.ID = vQualID Select p).First
        
        Dim vVenue As New venue
        Dim vResource = New resource
        Dim vClass = New classgroup
        Dim vDetails As New HashSet(Of sSlotDetails)
        For Each p In vQual.programmesubjects
            Dim vSiteSubjects = (From q In p.subject.siteclustersubjects
                                  Where q.subject.Level = vlevel And
                                        q.SiteClusterID = vClusterID
                                        Select q).ToList

            For Each r In vSiteSubjects
                For Each s In r.classgroups
                    For Each x In s.resources
                        For Each y In x.resourceschedules
                            If (y.timeSlotID = vSlot.timeslot And
                                y.Day = vSlot.day And
                                y.Week = vSlot.week And
                                y.Year = vSlot.year) Then
                                vClass = s
                                vResource = x
                                vVenue = y.venues.First
                                Dim SubjectDetail = "Subject:" + vClass.siteclustersubject.subject.Code + " [" + vClass.siteclustersubject.subject.longName + "]"
                                Dim ClassDetail = "Class Group:" + vClass.code + " Resource Type:" + vResource.resourcetype.code
                                Dim LecturerDetail = vClass.lecturer.officer.Surname + "," + vClass.lecturer.officer.FirstName
                                Dim VenueDetail = "Room:" + vVenue.Code + " Building:" + vVenue.building.shortName + " Site:" + vVenue.building.site.shortName
                                vDetails.Add(New sSlotDetails With {
                                             .subject = SubjectDetail,
                                             .class = ClassDetail,
                                             .venue = VenueDetail,
                                             .lecturer = LecturerDetail})
                            End If
                        Next
                    Next
                Next
            Next
        Next

        Dim vTimeslot = (From p In vContext.timeslots Where p.ID = vSlot.timeslot Select p).First
        litHeader.Text = "<p>" + WeekdayName(vSlot.day, False, FirstDayOfWeek.Sunday) +
                          "  " + Format(vTimeslot.StartTime, "HH:mm") + "-" +
                          Format(DateAdd(DateInterval.Minute, vTimeslot.Duration, vTimeslot.StartTime), "HH:mm") +
                          "</p><p> Qualification:" + vQual.Code + " [" + vQual.longName + "]</p>"
        litFooter.Text = ""
        DisplayDetails(vDetails)
        mvTimetable.SetActiveView(vwDetail)
    End Sub


    Private Sub displayClassDetails(ByVal vClassID As Integer, ByVal vSlot As sslot)
        Dim vContext As timetableEntities = New timetableEntities()

        Dim vClass = (From p In vContext.classgroups
                          Where p.ID = vClassID Select p).First

        Dim vVenue As New venue
        Dim ResFd = False 'resource found 
        Dim vResource = New resource
        Dim vDetails As New HashSet(Of sSlotDetails)
        For Each x In vClass.resources
            For Each y In x.resourceschedules
                If (y.timeSlotID = vSlot.timeslot And
                    y.Day = vSlot.day And
                    y.Week = vSlot.week And
                    y.Year = vSlot.year) Then
                    vResource = x
                    vVenue = y.venues.First
                    Dim SubjectDetail = "Subject:" + vClass.siteclustersubject.subject.Code + " [" + vClass.siteclustersubject.subject.longName + "]"
                    Dim ClassDetail = "Class Group:" + vClass.code + " Resource Type:" + vResource.resourcetype.code
                    Dim LecturerDetail = vClass.lecturer.officer.Surname + "," + vClass.lecturer.officer.FirstName
                    Dim VenueDetail = "Room:" + vVenue.Code + " Building:" + vVenue.building.shortName + " Site:" + vVenue.building.site.shortName
                    vDetails.Add(New sSlotDetails With {
                                 .subject = SubjectDetail,
                                 .class = ClassDetail,
                                 .venue = VenueDetail,
                                 .lecturer = LecturerDetail})
                    ResFd = True
                End If
                If ResFd Then Exit For
            Next
            If ResFd Then Exit For
        Next

        Dim vTimeslot = (From p In vContext.timeslots Where p.ID = vSlot.timeslot Select p).First
        Dim vQuals = (From p In vContext.programmesubjects Where p.SubjectID = vClass.SubjectID Select p.qualification).ToList
        Dim vQualStr = ""
        Dim InLoop = False
        For Each x In vQuals
            Dim vStr = x.Code + " [" + x.longName + "]"
            If InLoop Then
                vQualStr = vQualStr + cBreak + vStr
            Else
                vQualStr = vStr
                InLoop = True
            End If
        Next
        litHeader.Text = "<p>" + WeekdayName(vSlot.day, False, FirstDayOfWeek.Sunday) +
                       "  " + Format(vTimeslot.StartTime, "HH:mm") + "-" +
                       Format(DateAdd(DateInterval.Minute, vTimeslot.Duration, vTimeslot.StartTime), "HH:mm") +
                       "</p>"
        litFooter.Text = "<b>Qualification(s)</b><br/>" + vQualStr
        DisplayDetails(vDetails)

       ' lblSubject.Text = "Subject:" + vClass.siteclustersubject.subject.Code + " [" + vClass.siteclustersubject.subject.longName + "]"
        'lblClass.Text = "Class Group:" + vClass.code + " Resource Type:" + vResource.resourcetype.code
        'lblVenue.Text = "Room:" + vVenue.Code + " Building:" + vVenue.building.shortName + " Site:" + vVenue.building.site.shortName
        mvTimetable.SetActiveView(vwDetail)
    End Sub


    Private Sub displayVenueDetails(ByVal vVenueID As Integer, ByVal vSlot As sslot)
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vVenue = (From p In vContext.venues
                          Where p.ID = vVenueID
                            Select p).First

        Dim vResource = New resource
        Dim vClass = New classgroup
        Dim vDetails As New HashSet(Of sSlotDetails)
        For Each y In vVenue.resourceschedules
            If (y.timeSlotID = vSlot.timeslot And
                y.Day = vSlot.day And
                y.Week = vSlot.week And
                y.Year = vSlot.year) Then
                vResource = y.resource
                vClass = vResource.classgroups.First
                Dim SubjectDetail = "Subject:" + vClass.siteclustersubject.subject.Code + " [" + vClass.siteclustersubject.subject.longName + "]"
                Dim ClassDetail = "Class Group:" + vClass.code + " Resource Type:" + vResource.resourcetype.code
                Dim VenueDetail = "Room:" + vVenue.Code + " Building:" + vVenue.building.shortName + " Site:" + vVenue.building.site.shortName
                Dim LecturerDetail = vClass.lecturer.officer.Surname + "," + vClass.lecturer.officer.FirstName
                vDetails.Add(New sSlotDetails With {
                             .subject = SubjectDetail,
                             .class = ClassDetail,
                             .venue = VenueDetail,
                             .lecturer = LecturerDetail})
            End If
        Next
       
        ' Dim vClass = vResource.classgroups.First
        Dim vTimeslot = (From p In vContext.timeslots Where p.ID = vSlot.timeslot Select p).First
        Dim vQuals = (From p In vContext.programmesubjects Where p.SubjectID = vClass.SubjectID Select p.qualification).ToList
        Dim vQualStr = ""
        Dim InLoop = False
        For Each x In vQuals
            Dim vStr = x.Code + " [" + x.longName + "]"
            If InLoop Then
                vQualStr = vQualStr + cBreak + vStr
            Else
                vQualStr = vStr
                InLoop = True
            End If
        Next

        litHeader.Text = "<p>" + WeekdayName(vSlot.day, False, FirstDayOfWeek.Sunday) +
                    "  " + Format(vTimeslot.StartTime, "HH:mm") + "-" +
                    Format(DateAdd(DateInterval.Minute, vTimeslot.Duration, vTimeslot.StartTime), "HH:mm") +
                    "</p>"
        litFooter.Text = "<b>Qualification(s)</b><br/>" + vQualStr
        DisplayDetails(vDetails)


        mvTimetable.SetActiveView(vwDetail)
    End Sub


    Private Sub btnReturn_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnReturn.Click
        mvTimetable.SetActiveView(vwDisplay)
    End Sub

    Protected Sub cboType_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles cboType.SelectedIndexChanged
        loadobjects()
    End Sub

    Protected Sub Calendar1_SelectionChanged(ByVal sender As Object, ByVal e As EventArgs) Handles Calendar1.SelectionChanged
        Try
            showStatus()
            setTimeTable()
            litMessage.Text = ""
        Catch ex As Exception
            litMessage.Text = clsGeneral.displaymessage(ex.Message, True)
        End Try
    End Sub

    Sub showStatus()
        Dim vWeek = DatePart(DateInterval.WeekOfYear, Calendar1.SelectedDate)
        Dim vYear = DatePart(DateInterval.Year, Calendar1.SelectedDate)
        lblStatus.Text = "Week:" + vWeek.ToString + " Year:" + vYear.ToString
    End Sub


    Private Sub cboCluster_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cboCluster.SelectedIndexChanged
        Try
            loadobjects()
            litMessage.Text = ""
        Catch ex As Exception
            litMessage.Text = clsGeneral.displaymessage(ex.Message, True)
        End Try
    End Sub

    Private Sub cboSubject_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cboSubject.SelectedIndexChanged
        Try
            getClasses()
            litMessage.Text = ""
        Catch ex As Exception
            litMessage.Text = clsGeneral.displaymessage(ex.Message, True)
        End Try
    End Sub

    Private Sub cboSite_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cboSite.SelectedIndexChanged
        Try
            getRooms()
            litMessage.Text = ""
        Catch ex As Exception
            litMessage.Text = clsGeneral.displaymessage(ex.Message, True)
        End Try
    End Sub

    Protected Sub btnControl_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnControl.Click
        Try
            setTimeTableType()
            litMessage.Text = ""
        Catch ex As Exception
            litMessage.Text = clsGeneral.displaymessage(ex.Message, True)
        End Try
    End Sub

    Sub setTimeTableType()
        If cboCluster.SelectedIndex < 0 Then
            Throw New OverflowException("No Cluster available")
        End If
        lblCluster.Text = "Cluster:" + cboCluster.SelectedItem.Text
        lblCluster.ToolTip = cboCluster.SelectedItem.Value
        hdnType.Value = CStr(cboType.SelectedIndex)
        Select Case cboType.SelectedIndex
            Case 0 'qual
                lblObject0.Text = "Qual:" + cboQual.SelectedItem.Text
                lblObject0.ToolTip = cboQual.SelectedItem.Value
                lblObject1.Text = "Level:" + cboLevel.SelectedItem.Text
                lblObject1.ToolTip = cboLevel.SelectedItem.Text
            Case 1 'class
                lblObject0.Text = "Subject:" + cboSubject.SelectedItem.Text + " Class:" + cboClassgroup.SelectedItem.Text
                lblObject0.ToolTip = cboClassgroup.SelectedItem.Value
                lblObject1.Text = ""
                lblObject1.ToolTip = ""
            Case 2 'venue
                lblObject0.Text = "Site:" + cboSite.SelectedItem.Text + " Venue:" + cboRoom.SelectedItem.Text
                lblObject0.ToolTip = cboRoom.SelectedItem.Value
                lblObject1.Text = ""
                lblObject1.ToolTip = ""
        End Select
        setTimeTable()
    End Sub

    Sub setTimeTable()
        If cboCluster.SelectedIndex < 0 Then
            Throw New OverflowException("No Cluster available")
        End If
        Dim vContext As timetableEntities = New timetableEntities()
        Dim dt As DataTable = FormatTimeTable()
        Select Case CInt(hdnType.Value)
            Case 0 'qual
                setQualTimetable(dt, CInt(lblObject0.ToolTip), CInt(lblObject1.ToolTip), CInt(lblCluster.ToolTip))
            Case 1 'class
                setClassTimetable(dt, CInt(lblObject0.ToolTip))
            Case 2 'venue
                setVenueTimetable(dt, CInt(lblObject0.ToolTip))
        End Select
        grdTimeTable.DataSource = dt
        grdTimeTable.DataBind()
        mvTimetable.SetActiveView(vwDisplay)
    End Sub

    Sub setQualTimetable(ByRef dt As DataTable, ByVal vQualID As Integer, ByVal vLevel As Integer, ByVal vClusterID As Integer)
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vSubjects = (From p In vContext.programmesubjects
                          Where p.QualID = vQualID And
                                p.subject.Level = vLevel
                                Select p.subject).ToList
        Dim vTimeSlots = (From p In vContext.timeslots Select p).ToList
        Dim dr As DataRow
        Dim vDate = Calendar1.SelectedDate
        Dim vWeek = DatePart(DateInterval.WeekOfYear, vDate)
        Dim vYear = DatePart(DateInterval.Year, vDate)
        For Each x In vTimeSlots
            Dim vSlotID = x.ID
            ' Populate the datatable with your data (put this in appropriate loop)
            dr = dt.NewRow
            dr("TimeSlotID") = vSlotID
            Dim slot1 As String = ""
            Dim slot2 As String = ""
            Dim slot3 As String = ""
            Dim slot4 As String = ""
            Dim slot5 As String = ""
            Dim slot6 As String = ""
            Dim slot7 As String = ""
            Dim inCell As Boolean = False
            For Each y In vSubjects
                Dim vSiteSubjects = (From p In y.siteclustersubjects
                                 Where p.SiteClusterID = vClusterID Select p).ToList
                For Each z In vSiteSubjects
                    For Each k In z.classgroups
                        For Each j In k.resources
                            Dim nslot1 = (From p In j.resourceschedules Where p.Year = vYear And p.Week = vWeek And p.Day = 1 And p.timeSlotID = vSlotID Select p.resource.Name).FirstOrDefault
                            Dim nslot2 = (From p In j.resourceschedules Where p.Year = vYear And p.Week = vWeek And p.Day = 2 And p.timeSlotID = vSlotID Select p.resource.Name).FirstOrDefault
                            Dim nslot3 = (From p In j.resourceschedules Where p.Year = vYear And p.Week = vWeek And p.Day = 3 And p.timeSlotID = vSlotID Select p.resource.Name).FirstOrDefault
                            Dim nslot4 = (From p In j.resourceschedules Where p.Year = vYear And p.Week = vWeek And p.Day = 4 And p.timeSlotID = vSlotID Select p.resource.Name).FirstOrDefault
                            Dim nslot5 = (From p In j.resourceschedules Where p.Year = vYear And p.Week = vWeek And p.Day = 5 And p.timeSlotID = vSlotID Select p.resource.Name).FirstOrDefault
                            Dim nslot6 = (From p In j.resourceschedules Where p.Year = vYear And p.Week = vWeek And p.Day = 6 And p.timeSlotID = vSlotID Select p.resource.Name).FirstOrDefault
                            Dim nslot7 = (From p In j.resourceschedules Where p.Year = vYear And p.Week = vWeek And p.Day = 7 And p.timeSlotID = vSlotID Select p.resource.Name).FirstOrDefault
                            If inCell Then
                                slot1 = slot1 + CStr(IIf(IsNothing(nslot1), "", cBreak + nslot1))
                                slot2 = slot2 + CStr(IIf(IsNothing(nslot2), "", cBreak + nslot2))
                                slot3 = slot3 + CStr(IIf(IsNothing(nslot3), "", cBreak + nslot3))
                                slot4 = slot4 + CStr(IIf(IsNothing(nslot4), "", cBreak + nslot4))
                                slot5 = slot5 + CStr(IIf(IsNothing(nslot5), "", cBreak + nslot5))
                                slot6 = slot6 + CStr(IIf(IsNothing(nslot6), "", cBreak + nslot6))
                                slot7 = slot7 + CStr(IIf(IsNothing(nslot7), "", cBreak + nslot7))
                            Else
                                slot1 = CStr(IIf(IsNothing(nslot1), "", nslot1))
                                slot2 = CStr(IIf(IsNothing(nslot2), "", nslot2))
                                slot3 = CStr(IIf(IsNothing(nslot3), "", nslot3))
                                slot4 = CStr(IIf(IsNothing(nslot4), "", nslot4))
                                slot5 = CStr(IIf(IsNothing(nslot5), "", nslot5))
                                slot6 = CStr(IIf(IsNothing(nslot6), "", nslot6))
                                slot7 = CStr(IIf(IsNothing(nslot7), "", nslot7))
                                inCell = True
                            End If
                        Next
                    Next
                Next
            Next
            ' Add the row
            If Len(slot1) > 0 Then
                dr("slot1") = trimstring(slot1)
            End If
            If Len(slot2) > 0 Then
                dr("slot2") = trimstring(slot2)
            End If
            If Len(slot3) > 0 Then
                dr("slot3") = trimstring(slot3)
            End If
            If Len(slot4) > 0 Then
                dr("slot4") = trimstring(slot4)
            End If
            If Len(slot5) > 0 Then
                dr("slot5") = trimstring(slot5)
            End If
            If Len(slot6) > 0 Then
                dr("slot6") = trimstring(slot6)
            End If
            If Len(slot7) > 0 Then
                dr("slot7") = trimstring(slot7)
            End If
            dt.Rows.Add(dr)
        Next
        dt.AcceptChanges()
    End Sub

    Function trimstring(ByVal value As String) As String
        If InStr(value, cBreak) = 1 Then
            Return Mid(value, Len(cBreak) + 1)
        Else
            Return value
        End If
    End Function


    Sub setClassTimetable(ByRef dt As DataTable, ByVal vClassID As Integer)
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vTimeSlots = (From p In vContext.timeslots Select p).ToList
        Dim dr As DataRow
        Dim vDate = Calendar1.SelectedDate
        Dim vWeek = DatePart(DateInterval.WeekOfYear, vDate)
        Dim vYear = DatePart(DateInterval.Year, vDate)
        Dim vClassgroup = (From p In vContext.classgroups Where p.ID = vClassID Select p).First

        For Each x In vTimeSlots
            Dim vSlotID = x.ID
            dr = dt.NewRow
            dr("TimeSlotID") = x.ID
            Dim slot1 As String = ""
            Dim slot2 As String = ""
            Dim slot3 As String = ""
            Dim slot4 As String = ""
            Dim slot5 As String = ""
            Dim slot6 As String = ""
            Dim slot7 As String = ""
            Dim inCell As Boolean = False
            For Each y In vClassgroup.resources
                Dim nslot1 = (From p In y.resourceschedules Where p.Year = vYear And p.Week = vWeek And p.Day = 1 And p.timeSlotID = vSlotID Select p.resource.Name).FirstOrDefault
                Dim nslot2 = (From p In y.resourceschedules Where p.Year = vYear And p.Week = vWeek And p.Day = 2 And p.timeSlotID = vSlotID Select p.resource.Name).FirstOrDefault
                Dim nslot3 = (From p In y.resourceschedules Where p.Year = vYear And p.Week = vWeek And p.Day = 3 And p.timeSlotID = vSlotID Select p.resource.Name).FirstOrDefault
                Dim nslot4 = (From p In y.resourceschedules Where p.Year = vYear And p.Week = vWeek And p.Day = 4 And p.timeSlotID = vSlotID Select p.resource.Name).FirstOrDefault
                Dim nslot5 = (From p In y.resourceschedules Where p.Year = vYear And p.Week = vWeek And p.Day = 5 And p.timeSlotID = vSlotID Select p.resource.Name).FirstOrDefault
                Dim nslot6 = (From p In y.resourceschedules Where p.Year = vYear And p.Week = vWeek And p.Day = 6 And p.timeSlotID = vSlotID Select p.resource.Name).FirstOrDefault
                Dim nslot7 = (From p In y.resourceschedules Where p.Year = vYear And p.Week = vWeek And p.Day = 7 And p.timeSlotID = vSlotID Select p.resource.Name).FirstOrDefault
                If inCell Then
                    slot1 = slot1 + CStr(IIf(IsNothing(nslot1), "", cBreak + nslot1))
                    slot2 = slot2 + CStr(IIf(IsNothing(nslot2), "", cBreak + nslot2))
                    slot3 = slot3 + CStr(IIf(IsNothing(nslot3), "", cBreak + nslot3))
                    slot4 = slot4 + CStr(IIf(IsNothing(nslot4), "", cBreak + nslot4))
                    slot5 = slot5 + CStr(IIf(IsNothing(nslot5), "", cBreak + nslot5))
                    slot6 = slot6 + CStr(IIf(IsNothing(nslot6), "", cBreak + nslot6))
                    slot7 = slot7 + CStr(IIf(IsNothing(nslot7), "", cBreak + nslot7))
                Else
                    slot1 = CStr(IIf(IsNothing(nslot1), "", nslot1))
                    slot2 = CStr(IIf(IsNothing(nslot2), "", nslot2))
                    slot3 = CStr(IIf(IsNothing(nslot3), "", nslot3))
                    slot4 = CStr(IIf(IsNothing(nslot4), "", nslot4))
                    slot5 = CStr(IIf(IsNothing(nslot5), "", nslot5))
                    slot6 = CStr(IIf(IsNothing(nslot6), "", nslot6))
                    slot7 = CStr(IIf(IsNothing(nslot7), "", nslot7))
                    inCell = True
                End If
            Next
            ' Add the row
            If Len(slot1) > 0 Then
                dr("slot1") = trimstring(slot1)
            End If
            If Len(slot2) > 0 Then
                dr("slot2") = trimstring(slot2)
            End If
            If Len(slot3) > 0 Then
                dr("slot3") = trimstring(slot3)
            End If
            If Len(slot4) > 0 Then
                dr("slot4") = trimstring(slot4)
            End If
            If Len(slot5) > 0 Then
                dr("slot5") = trimstring(slot5)
            End If
            If Len(slot6) > 0 Then
                dr("slot6") = trimstring(slot6)
            End If
            If Len(slot7) > 0 Then
                dr("slot7") = trimstring(slot7)
            End If
            dt.Rows.Add(dr)
        Next
        dt.AcceptChanges()
    End Sub

    Sub setVenueTimetable(ByRef dt As DataTable, ByVal vVenueID As Integer)
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vTimeSlots = (From p In vContext.timeslots Select p).ToList
        Dim dr As DataRow
        Dim vDate = Calendar1.SelectedDate
        Dim vWeek = DatePart(DateInterval.WeekOfYear, vDate)
        Dim vYear = DatePart(DateInterval.Year, vDate)
        Dim vVenue = (From p In vContext.venues Where p.ID = vVenueID Select p).First
        Dim vResourceschedules = vVenue.resourceschedules
        For Each x In vTimeSlots
            ' Populate the datatable with your data (put this in appropriate loop)
            dr = dt.NewRow
            dr("TimeSlotID") = x.ID
            Dim vSlotID = x.ID
            Dim slot1 = (From p In vResourceschedules Where p.Year = vYear And p.Week = vWeek And p.Day = 1 And p.timeSlotID = vSlotID Select p.resource.Name).FirstOrDefault
            Dim slot2 = (From p In vResourceschedules Where p.Year = vYear And p.Week = vWeek And p.Day = 2 And p.timeSlotID = vSlotID Select p.resource.Name).FirstOrDefault
            Dim slot3 = (From p In vResourceschedules Where p.Year = vYear And p.Week = vWeek And p.Day = 3 And p.timeSlotID = vSlotID Select p.resource.Name).FirstOrDefault
            Dim slot4 = (From p In vResourceschedules Where p.Year = vYear And p.Week = vWeek And p.Day = 4 And p.timeSlotID = vSlotID Select p.resource.Name).FirstOrDefault
            Dim slot5 = (From p In vResourceschedules Where p.Year = vYear And p.Week = vWeek And p.Day = 5 And p.timeSlotID = vSlotID Select p.resource.Name).FirstOrDefault
            Dim slot6 = (From p In vResourceschedules Where p.Year = vYear And p.Week = vWeek And p.Day = 6 And p.timeSlotID = vSlotID Select p.resource.Name).FirstOrDefault
            Dim slot7 = (From p In vResourceschedules Where p.Year = vYear And p.Week = vWeek And p.Day = 7 And p.timeSlotID = vSlotID Select p.resource.Name).FirstOrDefault

            If Len(slot1) > 0 Then
                dr("slot1") = slot1
            End If
            If Len(slot2) > 0 Then
                dr("slot2") = slot2
            End If
            If Len(slot3) > 0 Then
                dr("slot3") = slot3
            End If
            If Len(slot4) > 0 Then
                dr("slot4") = slot4
            End If
            If Len(slot5) > 0 Then
                dr("slot5") = slot5
            End If
            If Len(slot6) > 0 Then
                dr("slot6") = slot6
            End If
            If Len(slot7) > 0 Then
                dr("slot7") = slot7
            End If
            ' Add the row
            dt.Rows.Add(dr)
        Next
        dt.AcceptChanges()
    End Sub


    Protected Sub DisplayDetails(ByVal vDetail As HashSet(Of sSlotDetails))
        Dim dt As DataTable = New DataTable()
        dt.Columns.Add("qualification", System.Type.GetType("System.String"))
        dt.Columns.Add("class", System.Type.GetType("System.String"))
        dt.Columns.Add("subject", System.Type.GetType("System.String"))
        dt.Columns.Add("venue", System.Type.GetType("System.String"))
        dt.Columns.Add("lecturer", System.Type.GetType("System.String"))
        Dim dr As DataRow
        For Each vSlot In vDetail
            With vSlot
                dr = dt.NewRow
                dr("class") = .class
                dr("subject") = .subject
                dr("venue") = .venue
                dr("lecturer") = .lecturer
                dt.Rows.Add(dr)
            End With
        Next
        With repDetails
            .DataSource = dt
            .DataBind()
        End With
    End Sub

    Protected Function FormatTimeTable() As DataTable
        Dim dt As DataTable = New DataTable()
        ' Create Columns
        dt.Columns.Add("TimeSlotID", System.Type.GetType("System.Int32"))
        dt.Columns.Add("slot1", System.Type.GetType("System.String"))
        dt.Columns.Add("slot2", System.Type.GetType("System.String"))
        dt.Columns.Add("slot3", System.Type.GetType("System.String"))
        dt.Columns.Add("slot4", System.Type.GetType("System.String"))
        dt.Columns.Add("slot5", System.Type.GetType("System.String"))
        dt.Columns.Add("slot6", System.Type.GetType("System.String"))
        dt.Columns.Add("slot7", System.Type.GetType("System.String"))
        Return dt
    End Function

    Protected Sub cboQual_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles cboQual.SelectedIndexChanged

    End Sub

    Protected Sub cboClassgroup_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles cboClassgroup.SelectedIndexChanged

    End Sub

    Private Sub repDetails_ItemCreated(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles repDetails.ItemCreated
        Dim plh = CType(e.Item.FindControl("phLecturer"), PlaceHolder)
        plh.Visible = User.Identity.IsAuthenticated
    End Sub


End Class