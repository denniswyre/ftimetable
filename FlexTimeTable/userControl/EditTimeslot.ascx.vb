Public Class EditTimeslot
    Inherits System.Web.UI.UserControl
    Public Event ExitClick(ByVal e As Object, ByVal Args As System.EventArgs)
    Public Event ErrorDetected(ByVal e As Object, ByVal Args As System.EventArgs)

    Private Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            loadTimeParameters()

        End If
    End Sub


    Property pType As _Default.eDisplayType
        Set(value As _Default.eDisplayType)
            ViewState("displayType") = CStr(CInt(value))
        End Set
        Get
            Return CType(CInt(ViewState("displayType")), _Default.eDisplayType)
        End Get
    End Property


    Sub loadTimeParameters()
        With cboDay
            For i = 1 To 7
                .Items.Add(New ListItem(WeekdayName(i, False, Microsoft.VisualBasic.FirstDayOfWeek.Sunday), CStr(i)))
            Next
        End With

        With cboStartSlot
            Dim vContext As timetableEntities = New timetableEntities()
            Dim vSlots = (From p In vContext.timeslots Order By p.StartTime Select p).ToList
            With cboStartSlot
                For Each x In vSlots
                    Dim vtime As String = Format(x.StartTime, "HH:mm")
                    Dim vItem As New ListItem(vtime, CStr(x.ID))
                    .Items.Add(vItem)
                Next
            End With
        End With

        cboNoSlots.Items.Add(New ListItem("1"))
        cboNoSlots.Items.Add(New ListItem("2"))
        cboNoSlots.Items.Add(New ListItem("3"))
    End Sub


    Sub clearParameters()
        errorMessage.Text = ""
        cboClass.Items.Clear()
        cboCluster.Items.Clear()
        cboResource.Items.Clear()
        cboSite.Items.Clear()
        cboSubject.Items.Clear()
        cboVenue.Items.Clear()
        chkStrictVenue.Checked = False
    End Sub


    Public Sub SetClassParameters(ByVal vClassID As Integer)
        clearParameters()
        pType = _Default.eDisplayType.classgroup
        'specify cluster,subject and class group and list resources
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vClass = (From p In vContext.classgroups Where p.ID = vClassID Select p).ToList
        Dim vSubjectID As Integer = vClass.First.SubjectID
        Dim vClusterID As Integer = vClass.First.SiteClusterID
        setSubjects((From p In vContext.subjects Order By p.Code Where p.ID = vSubjectID Select p).ToList)
        setClasses(vClass)
        setResources(vClass.First.resources.ToList)

        '''''''
        Dim vSites = (From p In vClass.First.siteclustersubject.sitecluster.sites Select p).ToList
        setClusters((From p In vContext.siteclusters Where p.ID = vClusterID Select p).ToList)
        setSite(vSites)
        setVenues(False)
    End Sub

    Public Sub SetVenueParameters(ByVal vVenueID As Integer, ByVal Username As String)
        clearParameters()
        'specify venue
        pType = _Default.eDisplayType.venue
        'list subjects, class groups and resources


        Dim vContext As timetableEntities = New timetableEntities()
        Dim vVenues = (From p In vContext.venues Order By p.building.shortName, p.Code Where p.ID = vVenueID Select p).ToList
        Dim vSiteID = vVenues.First.building.SiteID
        Dim vSites = (From p In vContext.sites Where p.ID = vSiteID Select p).ToList
        Dim vClusterId = vSites.First.SiteClusterID
        Dim vCluster = (From p In vContext.siteclusters Where p.ID = vClusterId Select p).ToList
        setClusters(vCluster)
        setSite(vSites)
        setVenues(vVenues)
        '''''''''''''''''''
        ''''''''''''''''''
        Dim sUser = clsOfficer.getOfficer(Username)
        Dim fac = (From p In vContext.facultyusers Where p.OfficerID = sUser.ID Select p).ToList

        ''''''''
        Dim vSubjects As New List(Of subject)
        For Each x In (From p In vCluster.First.siteclustersubjects Select p.subject).ToList
            For Each y In fac
                If x.department.school.facultyID = y.FacultyID Then
                    vSubjects.Add(x)
                End If
            Next
        Next
        setSubjects((From p In vSubjects Order By p.Code Select p).ToList)
        '''''''
        Dim vSubjectID = CInt(cboSubject.SelectedValue)
        setClasses((From p In vContext.classgroups Order By p.code Where p.SubjectID = vSubjectID Select p).ToList)
        Dim vClassID = CInt(cboClass.SelectedValue)
        Dim vResources = (From p In vContext.classgroups Where p.ID = vClassID Select p.resources).First
        setResources(vResources.ToList)
    End Sub

    Public Sub SetQualParameters(ByVal vQualID As Integer, vLevelID As Integer, vClusterID As Integer)
        clearParameters()
        pType = _Default.eDisplayType.qualification
        'set list of subjects and specify cluster
        'list subjects, class groups and resources
        'list venues
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vQuals = (From p In vContext.qualifications Where p.ID = vQualID Select p).First
        setSubjects((From p In vQuals.programmesubjects Order By p.subject.Code Select p.subject).ToList)
        Dim vSubjectID = CInt(cboSubject.SelectedValue)
        setClasses((From p In vContext.classgroups Order By p.code Where p.SubjectID = vSubjectID Select p).ToList)
        Dim vClassID = CInt(cboClass.SelectedValue)
        Dim vResources = (From p In vContext.classgroups Where p.ID = vClassID Select p.resources).First
        setResources(vResources.ToList)
        '''''''''''''''''''''''''
        setClusters((From p In vQuals.siteclusters Where p.ID = vClusterID Select p).ToList)
        Dim vSites = (From p In vContext.sites Where p.SiteClusterID = vClusterID Select p).ToList
        setSite(vSites)
        '''''''''''''''
        setVenues(False)
    End Sub


    Private Sub btnReturn_Click(sender As Object, e As System.EventArgs) Handles btnReturn.Click
        RaiseEvent ExitClick(Me, e)
    End Sub


    Sub setClusters(ByVal vClusters As List(Of sitecluster))
        With cboCluster
            .DataSource = vClusters
            .DataTextField = "longName"
            .DataValueField = "ID"
            .DataBind()
        End With
    End Sub

    Sub setSubjects(ByVal vSubject As List(Of subject))
        With cboSubject
            .DataSource = vSubject
            .DataTextField = "Code"
            .DataValueField = "ID"
            .DataBind()
        End With
    End Sub


    Sub setClasses(ByVal vClasses As List(Of classgroup))
        With cboClass
            .DataSource = vClasses
            .DataTextField = "Code"
            .DataValueField = "ID"
            .DataBind()
        End With
    End Sub

    Sub setSite(ByVal vSites As List(Of site))
        With cboSite
            .DataSource = vSites
            .DataTextField = "longName"
            .DataValueField = "ID"
            .DataBind()
        End With
    End Sub

    Sub setVenues(ByVal vVenues As List(Of venue))
        With cboVenue
            .Items.Clear()
            For Each x In vVenues
                Dim vItem As New ListItem(x.Code + ", " + x.building.shortName, CStr(x.ID))
                .Items.Add(vItem)
            Next
        End With
    End Sub


    Sub setVenues(ByVal ChangeResourceOnly As Boolean)
        Dim vContext As timetableEntities = New timetableEntities()

        If ChangeResourceOnly Then
            If cboVenue.SelectedIndex < 0 Then
                Throw New OverflowException("No Venue Found!!!")
            End If
            Dim vVenueID = CInt(cboVenue.SelectedValue)
            Dim vvenue = (From p In vContext.venues Where p.ID = vVenueID Select p).First
            Dim vResourceTypeID = vvenue.resourcetype.ID

            Dim vClassID = CInt(cboClass.SelectedValue)
            Dim vResources = (From p In vContext.classgroups Where p.ID = vClassID Select p.resources).First
            setResources((From p In vResources Where p.ResourceTypeID = vResourceTypeID Select p).ToList)
        Else
            If cboResource.SelectedIndex < 0 Then
                Throw New OverflowException("No Resource Found!!!")
            End If
            Dim vResourceID = CInt(cboResource.SelectedValue)
            Dim vResource = (From p In vContext.resources Where p.ID = vResourceID Select p).First
            Dim vsiteID = CInt(cboSite.SelectedValue)
            Dim vBuildings = (From p In vContext.buildings Where p.SiteID = vsiteID Select p).ToList
            Dim vVenues As New List(Of venue)
            For Each y In vBuildings
                For Each z In y.venues
                    If chkStrictVenue.Checked Then
                        If z.resourcetype.ID = vResource.ResourceTypeID Then
                            vVenues.Add(z)
                        End If
                    Else
                        vVenues.Add(z)
                    End If
                Next
            Next
            setVenues((From p In vVenues Order By p.building.shortName, p.Code Select p).ToList)
        End If

    End Sub


    Sub setResources(ByVal vResources As List(Of resource))
        With cboResource
            .DataSource = vResources
            .DataTextField = "Name"
            .DataValueField = "ID"
            .DataBind()
        End With
    End Sub


    Protected Sub cboClass_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboClass.SelectedIndexChanged
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vClassID = CInt(cboClass.SelectedValue)
        Dim vResources = (From p In vContext.classgroups Where p.ID = vClassID Select p.resources).First
        setResources(vResources.ToList)
    End Sub

    Protected Sub cboResource_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboResource.SelectedIndexChanged
        Try
            setVenues(False)
        Catch ex As Exception
            errorMessage.Text = clsGeneral.displaymessage(ex.Message, True)
        End Try
    End Sub

    Protected Sub cboSite_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboSite.SelectedIndexChanged
        Try
            setVenues(False)
        Catch ex As Exception
            errorMessage.Text = clsGeneral.displaymessage(ex.Message, True)
        End Try
    End Sub

    Protected Sub cboSubject_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboSubject.SelectedIndexChanged
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vSubjectID = CInt(cboSubject.SelectedValue)
        setClasses((From p In vContext.classgroups Order By p.code Where p.SubjectID = vSubjectID Select p).ToList)
        Dim vClassID = CInt(cboClass.SelectedValue)
        Dim vResources = (From p In vContext.classgroups Where p.ID = vClassID Select p.resources).First
        setResources(vResources.ToList)
    End Sub

    Private Sub chkStrictVenue_CheckedChanged(sender As Object, e As System.EventArgs) Handles chkStrictVenue.CheckedChanged
        Try
            setVenues(True)
        Catch ex As Exception
            errorMessage.Text = clsGeneral.displaymessage(ex.Message, True)
        End Try
    End Sub

    Private Sub btnSave_Click(sender As Object, e As System.EventArgs) Handles btnSave.Click
        Try

            Dim vContext As timetableEntities = New timetableEntities()
            Dim vResourceID = CInt(cboResource.SelectedValue)
            Dim vResource = (From p In vContext.resources Where p.ID = vResourceID Select p).First

            Dim vClass = (From p In vContext.classgroups Where p.ID = CInt(cboClass.SelectedValue) Select p).First
            Dim vBlock = vClass.academicblock

            Dim vNowWeek = DatePart(DateInterval.WeekOfYear, Date.Now)
            Dim vNowYear = DatePart(DateInterval.Year, Date.Now)
            Dim vGenStartWeek As Integer
            Dim vGenEndWeek As Integer

            If vNowWeek > vBlock.startWeek And vNowWeek <= vBlock.endWeek Then
                ''''generate from Vnoweek to vblock.endweek
                vGenStartWeek = vNowWeek
                vGenEndWeek = vBlock.endWeek
            ElseIf vNowWeek <= vBlock.startWeek Then
                '''''generate from vblock. Start and End
                vGenStartWeek = vBlock.startWeek
                vGenEndWeek = vBlock.endWeek
            Else
                '''''give error message
                Throw New OverflowException("Resource cannot be scheduled at this time. Check the Academic Block.")
            End If

            Dim vResourcSlots = (From p In vContext.resourceschedules Where p.Week = vNowWeek And p.Year = vNowYear And p.ResourceID = vResourceID Select p).ToList
            If vResource.AmtTimeSlots <= vResourcSlots.Count Then
                '''''''''''give error message
                Throw New OverflowException("Resource has already been fully scheduled!")
            End If
            Dim remSlots = vResource.AmtTimeSlots - vResourcSlots.Count
            Dim actSlots As Integer
            If CInt(cboNoSlots.SelectedItem.Text) > remSlots Then
                actSlots = remSlots
            Else
                actSlots = CInt(cboNoSlots.SelectedItem.Text)
            End If
            ' check if there is any clash

            Dim vFirstWeekSlot As New _Default.sslot With {
                .day = CInt(cboDay.SelectedValue),
                .timeslot = CInt(cboStartSlot.SelectedValue),
                .week = vGenStartWeek,
                .year = vNowYear}



            Dim vVenueID = CInt(cboVenue.SelectedValue)
            ''check venue
            '''' check venue availability for the first week
            Dim venueSchedule = (From p In vContext.venues Where p.ID = vVenueID Select p.resourceschedules).FirstOrDefault
            If Not IsNothing(venueSchedule) Then
                For i = 0 To actSlots - 1
                    Dim iSlot = vFirstWeekSlot.timeslot + i
                    Dim VenueAvail = (From p In venueSchedule
                                     Where p.Year = vFirstWeekSlot.year And
                                            p.Week = vFirstWeekSlot.week And
                                            p.Day = vFirstWeekSlot.day And
                                            p.timeSlotID = iSlot
                                            Select p).FirstOrDefault
                    If Not IsNothing(VenueAvail) Then
                        Throw New OverflowException("The selected Venue is not available!!")
                    End If
                Next
            End If

            '''''''check Class Clashes
            Dim ClassResources = vClass.resources.ToList
            If Not IsNothing(ClassResources) Then
                For Each x In ClassResources
                    For i = 0 To actSlots - 1
                        Dim iSlot = vFirstWeekSlot.timeslot + i
                        Dim ClassAvail = (From p In x.resourceschedules
                                         Where p.Year = vFirstWeekSlot.year And
                                                p.Week = vFirstWeekSlot.week And
                                                p.Day = vFirstWeekSlot.day And
                                                p.timeSlotID = iSlot
                                                Select p).FirstOrDefault
                        If Not IsNothing(ClassAvail) Then
                            Throw New OverflowException("Class clashes with this period!")
                        End If
                    Next
                Next
            End If


            '''''''check lecturer availability
            If Not IsNothing(vClass.lecturer) Then
                Dim vlecturerID = vClass.lecturer.LecturerID
                Dim LecturerResources = (From p In vContext.classgroups Where p.lecturer.LecturerID = vlecturerID Select p.resources).FirstOrDefault
                If Not IsNothing(LecturerResources) Then
                    For Each x In LecturerResources
                        For i = 0 To actSlots - 1
                            Dim iSlot = vFirstWeekSlot.timeslot + i
                            Dim LecturerAvail = (From p In x.resourceschedules
                                             Where p.Year = vFirstWeekSlot.year And
                                                    p.Week = vFirstWeekSlot.week And
                                                    p.Day = vFirstWeekSlot.day And
                                                    p.timeSlotID = iSlot
                                                    Select p).FirstOrDefault
                            If Not IsNothing(LecturerAvail) Then
                                Throw New OverflowException("The Lecturer has other Classes during this period!")
                            End If
                        Next
                    Next
                End If


                Dim vClusterID = vClass.SiteClusterID
                'check lecturer roster
                For i = 0 To actSlots - 1
                    Dim iSlot = vFirstWeekSlot.timeslot + i
                    Dim lecturerRoster = (From p In vContext.lecturersiteclusteravailabilities
                                            Where p.DayOfWeek = vFirstWeekSlot.day And
                                                  p.StartTimeSlot <= iSlot And
                                                  p.EndTimeSlot >= iSlot And
                                                  p.SiteClusterID = vClusterID
                                                  Select p).FirstOrDefault
                    If IsNothing(lecturerRoster) Then
                        Throw New OverflowException("The Lecturer is not rostered for this Cluster and Period!")
                    End If
                Next
            End If

            Dim vVenue = (From p In vContext.venues Where p.ID = vVenueID Select p).First
            For iWeek = vGenStartWeek To vGenEndWeek
                For i = 0 To actSlots - 1
                    ''check lecturer
                    Dim iSlot = vFirstWeekSlot.timeslot + i
                    Dim jWeek = iWeek
                    Dim vResourceSchedule = New resourceschedule With {
                       .ResourceID = vResourceID,
                       .timeSlotID = iSlot,
                       .Day = vFirstWeekSlot.day,
                       .Week = jWeek,
                       .Year = vFirstWeekSlot.year}
                    vContext.resourceschedules.AddObject(vResourceSchedule)
                    vResourceSchedule.venues.Add(vVenue)
                    vContext.SaveChanges()
                Next
            Next
            Dim vLogEntry As New userlog With {
              .page = Request.Path,
              .ipaddress = Request.UserHostAddress,
              .description = "Class:" + cboClass.SelectedValue + " Venue:" + cboVenue.SelectedValue,
              .fdatetime = DateTime.Now,
              .function = "Create TimeTable",
              .user = Context.User.Identity.Name}
            vContext.userlogs.AddObject(vLogEntry)
            vContext.SaveChanges()
            RaiseEvent ExitClick(Me, New System.EventArgs)
        Catch ex As Exception
            errorMessage.Text = clsGeneral.displaymessage(ex.Message, True)
        End Try
    End Sub
End Class