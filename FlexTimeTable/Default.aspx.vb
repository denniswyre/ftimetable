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

    Enum eDisplayType
        qualification = 0
        classgroup = 1
        venue = 2
    End Enum

    Property pType As eDisplayType
        Set(value As eDisplayType)
            hdnType.Value = CStr(value)
        End Set
        Get
            Return CType(hdnType.Value, eDisplayType)
        End Get
    End Property

    Property pClusterID As Integer
        Set(value As Integer)
            Try
                If value = 0 Then
                    Throw New Exception("")
                End If
                Dim vContext As timetableEntities = New timetableEntities()
                Dim vClu = (From p In vContext.siteclusters Where p.ID = value Select p).Single
                litCluster.Text = "<b>Cluster:</b>" + vClu.shortName
                ViewState("cluster") = CStr(value)
            Catch ex As Exception
                litCluster.Text = ""
                ViewState("cluster") = "0"
            End Try
        End Set
        Get
            Return CInt(ViewState("cluster"))
        End Get
    End Property

    Property pQualID As Integer
        Set(value As Integer)
            Try
                If value = 0 Then
                    Throw New Exception("")
                End If
                Dim vContext As timetableEntities = New timetableEntities()

                Dim vQua = (From p In vContext.qualifications Where p.ID = value Select p).SingleOrDefault

                'load clusters
                With cboCluster
                    .DataSource = (From p In vQua.siteclusters Select p).ToList
                    .DataTextField = "longName"
                    .DataValueField = "ID"
                    .DataBind()
                End With

                ViewState("QualCode") = CStr(value)
                lblQualCode.Text = vQua.Code
                lblQualCode.ToolTip = vQua.longName
                litMessage.Text = ""
            Catch ex As Exception
                litMessage.Text = clsGeneral.displaymessage(ex.Message, True)
                ViewState("QualCode") = "0"
                lblQualCode.Text = ""
                lblQualCode.ToolTip = ""
                cboCluster.DataSource = Nothing
                cboCluster.DataBind()
            End Try
        End Set
        Get
            Return CInt(ViewState("QualCode"))
        End Get
    End Property

    Property pSubjectID As Integer
        Set(value As Integer)
            Try
                If value = 0 Then
                    Throw New Exception("")
                End If

                Dim vContext As timetableEntities = New timetableEntities()
                Dim vSub = (From p In vContext.subjects Where p.ID = value Select p).Single

                'load clusters
                With cboCluster
                    .DataSource = (From p In vSub.siteclustersubjects Select p.sitecluster).ToList
                    .DataTextField = "longName"
                    .DataValueField = "ID"
                    .DataBind()
                End With

                getClasses()

                ViewState("subjectID") = CStr(value)
                lblSubjectCode.Text = vSub.Code
                lblSubjectCode.ToolTip = vSub.longName
                litMessage.Text = ""
            Catch ex As Exception
                litMessage.Text = clsGeneral.displaymessage(ex.Message, True)
                ViewState("subjectID") = "0"
                lblSubjectCode.Text = ""
                lblSubjectCode.ToolTip = ""
                cboCluster.DataSource = Nothing
                cboCluster.DataBind()
                cboClassgroup.DataSource = Nothing
                cboClassgroup.DataBind()
            End Try
        End Set
        Get
            Return CInt(ViewState("subjectID"))
        End Get
    End Property



    Property pObject0 As Integer
        Set(value As Integer)
            If value > 0 Then
                Dim vContext As timetableEntities = New timetableEntities()
                Select Case pType
                    Case eDisplayType.qualification
                        Dim vqu = (From p In vContext.qualifications Where p.ID = value Select p).Single
                        litObject0.Text = "<b>Qual:</b>" + vqu.longName
                    Case eDisplayType.classgroup
                        Dim vcla = (From p In vContext.classgroups Where p.ID = value Select p).Single
                        litObject0.Text = "<b>Subject:</b>" + vcla.siteclustersubject.subject.longName + " <b>Class:</b>" + vcla.code
                    Case eDisplayType.venue
                        Dim ven = (From p In vContext.venues Where p.ID = value Select p).Single
                        litObject0.Text = "<b>Venue:</b>" + ven.Code + "," + ven.building.shortName + "," + ven.building.site.shortName
                End Select
            Else
                litObject0.Text = ""
            End If
            ViewState("object0") = CStr(value)
        End Set
        Get
            Return CInt(ViewState("object0"))
        End Get
    End Property

    Property pObject1 As Integer
        Set(value As Integer)
            If value > 0 Then
                Select Case pType
                    Case eDisplayType.qualification
                        litObject1.Text = "<b>Level:</b>" + CStr(value)
                    Case eDisplayType.classgroup
                        litObject1.Text = ""
                    Case eDisplayType.venue
                        litObject1.Text = ""
                End Select
            Else
                litObject1.Text = ""
            End If
            ViewState("object1") = CStr(value)
        End Set
        Get
            Return CInt(ViewState("object1"))
        End Get
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            Try
                pSubjectID = 0
                pQualID = 0
                ucQqualificationSearch.activateExit(True)
                ucSubjectSearch.activateExit(True)
                mvTimetable.SetActiveView(vwDisplay)
                cboType.SelectedIndex = 0
                setDisplayType(eDisplayType.qualification)
                Calendar1.SelectedDate = Now
                showStatus()
                getlevels()
                loadTypeProperties()
                setTimeTableType()
                litMessage.Text = ""
            Catch ex As Exception
                litMessage.Text = clsGeneral.displaymessage(ex.Message, True)
            End Try

        End If
    End Sub

    Sub setDisplayType(ByVal vType As eDisplayType)
        cboCluster.Items.Clear()
        pQualID = 0
        pSubjectID = 0
        Select Case vType
            Case eDisplayType.qualification
                phQual.Visible = True
                phClass.Visible = False
                phVenue.Visible = False
            Case eDisplayType.classgroup
                phQual.Visible = False
                phClass.Visible = True
                phVenue.Visible = False
            Case eDisplayType.venue
                phQual.Visible = False
                phClass.Visible = False
                phVenue.Visible = True
                getAllClusters()
        End Select
    End Sub

    Sub loadTypeProperties()
        Select Case CType(cboType.SelectedIndex, eDisplayType)
            Case eDisplayType.qualification

            Case eDisplayType.classgroup
                getClasses()
            Case eDisplayType.venue
                getSites()
        End Select
    End Sub


    Sub getAllClusters()
        Dim vContext As timetableEntities = New timetableEntities()
        With cboCluster
            .DataSource = (From p In vContext.siteclusters Select p).ToList
            .DataTextField = "shortname"
            .DataValueField = "id"
            .DataBind()
        End With
        getSites()
    End Sub

    Sub getClasses()
        If pSubjectID > 0 And cboCluster.SelectedIndex >= 0 Then
            Dim vContext As timetableEntities = New timetableEntities()
            Dim vClusterID = CInt(cboCluster.SelectedValue)
            Dim vClasses = (From p In vContext.classgroups
                             Where p.SubjectID = pSubjectID And
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
        Select Case pType
            Case eDisplayType.qualification 'qual
                displayQualDetails(pObject0, pObject1, pClusterID, vSlot)
            Case eDisplayType.classgroup 'class
                displayClassDetails(pObject0, vSlot)
            Case eDisplayType.venue 'venue
                displayVenueDetails(pObject0, vSlot)
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
                                Dim LecturerDetail = ""
                                If Not IsNothing(vClass.lecturer) Then
                                    LecturerDetail = vClass.lecturer.officer.Surname + "," + vClass.lecturer.officer.FirstName
                                End If
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
        setDisplayType(CType(cboType.SelectedIndex, eDisplayType))
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
            loadTypeProperties()
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
        Try
            pClusterID = CInt(cboCluster.SelectedItem.Value)
            pType = CType(cboType.SelectedIndex, eDisplayType)
            Select Case pType
                Case eDisplayType.qualification 'qual
                    pObject0 = pQualID
                    pObject1 = CInt(cboLevel.SelectedItem.Text)
                Case eDisplayType.classgroup 'class
                    pObject0 = CInt(cboClassgroup.SelectedItem.Value)
                    pObject1 = 0
                Case eDisplayType.venue 'venue
                    pObject0 = CInt(cboRoom.SelectedItem.Value)
                    pObject1 = 0
            End Select
            setTimeTable()
        Catch ex As Exception
            pClusterID = 0
            pObject0 = 0
            pObject1 = 0
            setBlankTimetable()
        End Try
    End Sub

    Sub setTimeTable()
        Dim vContext As timetableEntities = New timetableEntities()
        Dim dt As DataTable = FormatTimeTable()
        Select Case pType ' CType(CInt(hdnType.Value),
            Case eDisplayType.qualification ' 0 'qual
                setQualTimetable(dt, pObject0, pObject1, pClusterID)
            Case eDisplayType.classgroup '1 'class
                setClassTimetable(dt, pObject0)
            Case eDisplayType.venue '2 'venue
                setVenueTimetable(dt, pObject0)
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



    Sub setBlankTimetable()
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vTimeSlots = (From p In vContext.timeslots Select p).ToList
        Dim vDate = Calendar1.SelectedDate
        Dim vWeek = DatePart(DateInterval.WeekOfYear, vDate)
        Dim vYear = DatePart(DateInterval.Year, vDate)
        Dim dt As DataTable = FormatTimeTable()
        Dim dr As DataRow
        For Each x In vTimeSlots
            ' Populate the datatable with your data (put this in appropriate loop)
            dr = dt.NewRow
            dr("TimeSlotID") = x.ID
            ' Add the row
            dt.Rows.Add(dr)
        Next
        dt.AcceptChanges()
        grdTimeTable.DataSource = dt
        grdTimeTable.DataBind()
        mvTimetable.SetActiveView(vwDisplay)
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

    Protected Sub cboClassgroup_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles cboClassgroup.SelectedIndexChanged

    End Sub

    Private Sub repDetails_ItemCreated(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles repDetails.ItemCreated
        Dim plh = CType(e.Item.FindControl("phLecturer"), PlaceHolder)
        plh.Visible = User.Identity.IsAuthenticated
    End Sub

    Private Sub ucQqualificationSearch_ExitClick(E As Object, Args As System.EventArgs) Handles ucQqualificationSearch.ExitClick
        mvGeneral.ActiveViewIndex = 0
    End Sub

    Private Sub ucQqualificationSearch_QualClick(E As Object, Args As clsQualEvent) Handles ucQqualificationSearch.QualClick
        pQualID = Args.mQualID
        mvGeneral.ActiveViewIndex = 0
    End Sub

    Private Sub ucSubjectSearch_ExitClick(E As Object, Args As System.EventArgs) Handles ucSubjectSearch.ExitClick
        mvGeneral.ActiveViewIndex = 0
    End Sub

    Private Sub ucSubjectSearch_SubjectClick(E As Object, Args As clsSubjectEvent) Handles ucSubjectSearch.SubjectClick
        pSubjectID = Args.mSubjectID
        loadTypeProperties()
        mvGeneral.ActiveViewIndex = 0
    End Sub

    Private Sub btnQual_Click(sender As Object, e As System.EventArgs) Handles btnQual.Click
        ucQqualificationSearch.activateExit(True)
        mvGeneral.ActiveViewIndex = 1
    End Sub

    Private Sub btnSubject_Click(sender As Object, e As System.EventArgs) Handles btnSubject.Click
        ucSubjectSearch.activateExit(True)
        mvGeneral.ActiveViewIndex = 2
    End Sub

    Private Sub getSites()
        If cboCluster.SelectedIndex >= 0 Then
            Dim vContext As timetableEntities = New timetableEntities()
            Dim vclusterID = CInt(cboCluster.SelectedItem.Value)
            Dim vSites = (From p In vContext.sites Where p.SiteClusterID = vclusterID Select p).ToList
            With cboSite
                .DataSource = vSites
                .DataTextField = "shortName"
                .DataValueField = "id"
                .DataBind()
            End With
            getRooms()
        End If
    End Sub
End Class