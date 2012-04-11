Imports System.IO

Public Class generateTimetable
    Inherits System.Web.UI.Page


    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            getyear()
            getfaculties()
            getClusters()
            If CType(Application("timetablegeneration"), Boolean) = True Then
                litErrorMessage.Text = clsGeneral.displaymessage("Timetable is being generated already!!", True)
                btnGenerate.Enabled = False
            End If
        End If
    End Sub


    Sub getyear()
        cboYear.Items.Clear()
        cboYear.Items.Add(CStr(Year(Now)))
        cboYear.Items.Add(CStr(Year(Now) + 1))
        getBlock()
    End Sub

    Sub getBlock()
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vBlocks As List(Of academicblock) '= (From p In vContext.academicblocks Select p).ToList
        If CInt(cboYear.SelectedItem.Text) = Year(Now) Then
            Dim vCurWeek = DatePart(DateInterval.WeekOfYear, Now)
            vBlocks = (From p In vContext.academicblocks Where p.endWeek > vCurWeek Select p).ToList
        Else
            vBlocks = (From p In vContext.academicblocks Select p).ToList
        End If
        With cboBlock.Items
            .Clear()
            For Each x In vBlocks
                .Add(New ListItem(x.Name + " [" + CStr(x.startWeek) + "-" + CStr(x.endWeek) + "]", CStr(x.ID)))
            Next
        End With
    End Sub

    Sub getClusters()
        Dim vContext As timetableEntities = New timetableEntities()
        With Me.cboCluster
            .DataSource = (From p In vContext.siteclusters Select p).ToList
            .DataValueField = "ID"
            .DataTextField = "longName"
            .DataBind()
        End With
    End Sub

    Sub getfaculties()
        Dim vContext As timetableEntities = New timetableEntities()
        With cboFaculty
            .DataSource = (From p In vContext.faculties Select name = p.longName, id = p.ID).ToList
            .DataTextField = "name"
            .DataValueField = "id"
            .DataBind()
        End With
    End Sub


    Structure sSlot
        Dim Year As Integer
        Dim Week As Integer
        Dim Day As Integer
        Dim timeslot As Integer
    End Structure


  


    Sub WriteResourceLog(ByVal vResourceID As Integer, ByVal BlockID As Integer, ByVal AcademicYear As Integer, ByVal vLogType As clsGeneral.eLogType)
        Dim vContext As timetableEntities = New timetableEntities()
        Dim newResourceLog As New timetablelog With {
            .ResourceID = vResourceID,
            .AcademicBlockID = BlockID,
            .AcademicYear = AcademicYear,
            .DateGenerated = Date.Now,
            .ReasonID = CType(vLogType, Integer)}
        vContext.timetablelogs.AddObject(newResourceLog)
        vContext.SaveChanges()
    End Sub

    Sub getQualifiedRooms(ByVal vResource As resource, ByRef venueList As ArrayList, ByRef vlogType As clsGeneral.eLogType)
        Dim vResourceSize = vResource.AmtParticipants

        Dim resourceTypefound = False
        'get preferred venue
        For Each x In vResource.resourcepreferredvenues
            venueList.Add(x.VenueID)
        Next

        ' get classgroup linked to resource (for now only one)
        Dim ResourceClassgroup = vResource.classgroups.FirstOrDefault
        If Not IsNothing(ResourceClassgroup) Then
            'get all venues assigned to department

            Dim DepartVenues = ResourceClassgroup.siteclustersubject.subject.department.venues
            For Each y In (From p In DepartVenues Order By p.Capacity
                                 Where p.resourcetype.ID = vResource.ResourceTypeID And
                                       p.Capacity >= vResourceSize Select p).ToList
                venueList.Add(y.ID)
            Next

            ''get all qualified venues in site cluster
            Dim vsites = ResourceClassgroup.siteclustersubject.sitecluster.sites
            For Each v In vsites
                Dim y = v.buildings
                For Each z In y
                    For Each x In (From p In z.venues Order By p.Capacity
                                     Where p.resourcetype.ID = vResource.ResourceTypeID Select p).ToList
                        resourceTypefound = True
                        If x.Capacity >= vResourceSize Then
                            venueList.Add(x.ID)
                        End If
                    Next
                Next
            Next

        End If

        If venueList.Count = 0 Then
            If resourceTypefound Then
                vlogType = clsGeneral.eLogType.SizeNotAvailable
            Else
                vlogType = clsGeneral.eLogType.NoResourceType
            End If
        End If
    End Sub

    Protected Sub btnGenerate_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnGenerate.Click
        If CType(Application("timetablegeneration"), Boolean) = True Then
            litErrorMessage.Text = clsGeneral.displaymessage("Timetable is being generated already!!", True)
            btnGenerate.Enabled = False
            Exit Sub
        End If
        Application("timetablegeneration") = "1"
        Dim vContext As timetableEntities = New timetableEntities()
        ''''''''''''''delete log'''''''''''''''''''''''''''''''''''''''''''''

        Dim logss = (From p In vContext.timetablelogs Select p).ToList
        For Each x In logss
            vContext.DeleteObject(x)
        Next
        vContext.SaveChanges()
        ''''''''''''''''''''''''''''''''''
        Dim vClassGroups As List(Of classgroup)
        If chkAllCluster.Checked And chkAllFaculty.Checked Then
            vClassGroups = (From p In vContext.classgroups Select p).ToList
        ElseIf chkAllCluster.Checked And Not chkAllFaculty.Checked Then
            vClassGroups = (From p In vContext.classgroups
                                Where p.siteclustersubject.subject.department.school.facultyID =
                                    CInt(cboFaculty.SelectedValue) Select p).ToList
        ElseIf Not chkAllCluster.Checked And chkAllFaculty.Checked Then
            vClassGroups = (From p In vContext.classgroups
                                Where p.SiteClusterID = CInt(cboCluster.SelectedValue) Select p).ToList
        Else
            vClassGroups = (From p In vContext.classgroups
                                Where p.siteclustersubject.subject.department.school.facultyID = CInt(cboFaculty.SelectedValue) And
                                      p.SiteClusterID = CInt(cboCluster.SelectedValue) Select p).ToList
        End If

        Dim vBlock = (From p In vContext.academicblocks Where p.ID = CInt(cboBlock.SelectedValue) Select p).First
        Dim vYear = CInt(cboYear.SelectedItem.Text)
        Dim vCurWeek = DatePart(DateInterval.WeekOfYear, Now)

        Dim vStartWeek As Integer
        If Year(Now) = vYear And vCurWeek > vBlock.startWeek Then
            vStartWeek = vCurWeek
        Else  ''''greater
            vStartWeek = vBlock.startWeek
        End If
        Dim weekBoundaries = getBlockBoundaries(vStartWeek)



        Dim AssignedClassList As New List(Of Integer)
        Dim unAssignedClassList As New List(Of Integer)

        Dim OrderedResources = orderResources(vClassGroups, vYear, vStartWeek)
        For Each x In OrderedResources
            Dim xResourceID = CInt(x)
            Dim vResource = (From p In vContext.resources Where p.ID = xResourceID And p.year = vYear Select p).FirstOrDefault
            If Not IsNothing(vResource) Then
                scheduleResource(vResource, weekBoundaries)
            End If
        Next
        clsGeneral.logAction(Request.Path, Request.UserHostAddress, "Generate TimeTable", Context.User.Identity.Name)
        Application("timetablegeneration") = "0"
    End Sub


    Function orderResources(ByVal vClassgroupList As List(Of classgroup), ByVal vYear As Integer, vWeek As Integer) As List(Of Integer)
        Dim vContext As timetableEntities = New timetableEntities()
        ''Give priority to those who have preferrred rooms

        Dim PriorityAssignedList As New List(Of Integer)
        Dim PriorityUnAssignedList As New List(Of Integer)
        Dim OtherAssignedList As New List(Of Integer)
        Dim OtherUnAssignedList As New List(Of Integer)
        Dim i = 0

        For Each x In vClassgroupList
            Dim ClusterID = x.SiteClusterID
            Dim IsVenueAssigned = False
            For Each y In x.siteclustersubject.subject.department.venues
                If ClusterID = y.building.site.SiteClusterID Then
                    IsVenueAssigned = True
                    Exit For
                End If
            Next
            Dim HasManyResources As Boolean = False
            If x.resources.Count > 1 Then
                HasManyResources = True
            End If

            Dim UnscheduledResources = (From p In x.resources
                                          Where p.AmtTimeSlots > p.resourceschedules.Count And
                                                p.endWeek >= vWeek And
                                                p.startWeek <= vWeek And
                                                p.year = vYear
                                            Select p).ToList
            '''''''substitute for x.resources
            For Each y In UnscheduledResources
                i = i + 1
                If HasManyResources AndAlso y.resourcetype.isClassRoom Then
                    If IsVenueAssigned Then
                        PriorityAssignedList.Add(y.ID)
                    Else
                        PriorityUnAssignedList.Add(y.ID)
                    End If
                Else
                    If IsVenueAssigned Then
                        OtherAssignedList.Add(y.ID)
                    Else
                        OtherUnAssignedList.Add(y.ID)
                    End If
                End If

            Next
        Next
        litErrorMessage.Text = clsGeneral.displaymessage("number of resources" + CStr(i), False)
        Return PriorityAssignedList.Concat(PriorityUnAssignedList).Concat(OtherAssignedList).Concat(OtherUnAssignedList).ToList
    End Function



    'get the endweeks of all atomic blocks from startweek to end of year 
    Function getBlockBoundaries(ByVal vStartweek As Integer) As HashSet(Of Integer)
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vBoundaries As New HashSet(Of Integer)
        vBoundaries.Add(vStartweek)
        Dim vBlocks = (From p In vContext.academicblocks Where p.endWeek > vStartweek Order By p.endWeek Select p).ToList
        For Each x In vBlocks
            vBoundaries.Add(x.endWeek)
        Next
        Return vBoundaries
    End Function

    'get Minor Academic Block ID for A SPECIFIED WEEK 
    Function getMinorBlockID(ByVal vWeek As Integer) As Integer
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vBlocks = (From p In vContext.academicblocks Where p.startWeek <= vWeek And p.endWeek >= vWeek
                           Order By p.startWeek Descending, p.endWeek Ascending
                              Select p).FirstOrDefault
        If IsNothing(vBlocks) Then
            Return 0
        Else
            Return vBlocks.ID
        End If
    End Function


    Protected Sub scheduleResource(ByVal vResource As resource, ByVal vWeekBoundaries As HashSet(Of Integer))
        Dim vContext As timetableEntities = New timetableEntities()

        Dim vClassGroup = vResource.classgroups.First
        Dim vOfferingType = vClassGroup.offeringtype

        Dim morningSlots As New List(Of Integer)
        Dim afternoonSlots As New List(Of Integer)
        Dim weekEndSlots As New List(Of Integer)
        Dim lunchTimeSlot = (From p In vContext.timeslots
                                Where p.LunchPeriod = True
                                      Select p).FirstOrDefault
        For Each x In (From p In vContext.timeslots
                                Where p.ID >= vOfferingType.startTimeSlot And
                                      p.ID <= vOfferingType.endTimeSlot
                                      Select p).ToList
            If x.ID < lunchTimeSlot.ID Then
                morningSlots.Add(x.ID)
            ElseIf x.ID > lunchTimeSlot.ID Then
                afternoonSlots.Add(x.ID)
            End If
        Next
        If vOfferingType.SabbathClasses Then
            For Each x In (From p In vContext.timeslots
                            Where p.ID >= vOfferingType.sabStartTimeSlot And
                                  p.ID >= vOfferingType.sabEndTimeSlot
                                  Select p).ToList
                weekEndSlots.Add(x.ID)
            Next
        End If



        Dim vWorkDays() As Integer = {2, 4, 3, 5, 6}
        Dim vWeekEnd() As Integer = {7}
        ' Try

        With vResource
            For i As Integer = 0 To vWeekBoundaries.Count - 2
                Dim iStartWeek = CInt(IIf(i = 0, vWeekBoundaries(i), vWeekBoundaries(i) + 1))
                Dim iEndWeek = vWeekBoundaries(i + 1)
                'exit resource if it doesn't contain this week
                If Not (iStartWeek >= .startWeek And iStartWeek <= .endWeek) Then Continue For
                '''''''''''''''''''''''''''Get Existing Booked Periods ''''''''''''''''''''''''''

                Dim savedSlots As Integer = (From p In .resourceschedules Where p.Year = .year And p.Week = iStartWeek Select p).Count
                '''''get the required periods
                '''''do not genertate if timetable already created
                If savedSlots >= .AmtTimeSlots Then Continue For

                ''get all used resource slots
                Dim vUsedResourceSlots As New HashSet(Of sSlot)
                setUsedResourceSlots(vUsedResourceSlots, .ID, .year, iStartWeek)
                Dim vlecturerAvailable = False

                Dim vVenueList As New ArrayList
                Dim vlogType As clsGeneral.eLogType = clsGeneral.eLogType.noLog
                getQualifiedRooms(vResource, vVenueList, vlogType)

                For Each xVenueID In vVenueList
                    If savedSlots >= .AmtTimeSlots Then Exit For
                    Dim vVenueID = CInt(xVenueID)
                    ''''''''''''''''''''''''''START Qualified ROOMs Loop''''''''''''''''

                    '''''''''''set used venue slots 
                    Dim vUsedVenueSlots As New HashSet(Of sSlot)
                    setUsedVenueSlots(vUsedVenueSlots, CInt(vVenueID), .year, iStartWeek)

                    '''''''initialize mark resources to be saved
                    Dim MarkedSlots As New HashSet(Of sSlot)

                    ''''''''''''''''schedule morning slots first
                    scheduleSlots(vUsedResourceSlots,
                        vUsedVenueSlots,
                        MarkedSlots,
                        vOfferingType,
                        vResource,
                        vClassGroup.ID,
                        vVenueID,
                        savedSlots,
                        vWorkDays,
                        morningSlots,
                        .AmtTimeSlots,
                        .year,
                        iStartWeek,
                        vlecturerAvailable)

                    '''''''''''''''schedule afternoon slots
                    scheduleSlots(vUsedResourceSlots,
                       vUsedVenueSlots,
                       MarkedSlots,
                       vOfferingType,
                       vResource,
                       vClassGroup.ID,
                       vVenueID,
                       savedSlots,
                       vWorkDays,
                       afternoonSlots,
                      .AmtTimeSlots,
                       .year,
                       iStartWeek,
                       vlecturerAvailable)

                    ''''''''''''''schedule weekend 
                    scheduleSlots(vUsedResourceSlots,
                        vUsedVenueSlots,
                        MarkedSlots,
                        vOfferingType,
                        vResource,
                        vClassGroup.ID,
                        vVenueID,
                        savedSlots,
                        vWeekEnd,
                        weekEndSlots,
                        .AmtTimeSlots,
                        .year,
                        iStartWeek,
                        vlecturerAvailable)

                    ''''''''''''''''''''''''''END Qualified ROOMs Loop''''''''''''''
                    BulkSaveSlots(MarkedSlots, .ID, vVenueID, iStartWeek, iEndWeek)
                Next

                '''''''''write log
                If vlogType = clsGeneral.eLogType.noLog And vVenueList.Count > 0 Then
                    If Not vlecturerAvailable Then
                        vlogType = clsGeneral.eLogType.LecturerNotAvailable
                    ElseIf (From p In .resourceschedules Where p.Year = .year And p.Week = iStartWeek Select p).Count = 0 Then
                        vlogType = clsGeneral.eLogType.NoSpaceFoundInCluster
                    End If
                End If

                If vlogType <> clsGeneral.eLogType.noLog Then
                    WriteResourceLog(vResource.ID, getMinorBlockID(iStartWeek), vResource.year, vlogType)
                End If
                ''''''''''''''''''''''''Block Period'''''''''''''''''''''''''''''''''''''''
            Next


        End With  'vSubGrDRow
    End Sub



    Sub BulkSaveSlots(ByVal MarkedSlots As HashSet(Of sSlot), ByVal vResourceID As Integer, ByVal vVenueID As Integer, ByVal vStartWeek As Integer, ByVal vEndWeek As Integer)
        Dim vContext As timetableEntities = New timetableEntities()
        Dim xVenue = (From p In vContext.venues Where p.ID = vVenueID Select p).First
        For Each x In MarkedSlots
            For jWeek = vStartWeek To vEndWeek
                Dim vResourceSchedule = New resourceschedule With {
                    .ResourceID = vResourceID,
                    .timeSlotID = x.timeslot,
                    .Day = x.Day,
                    .Week = jWeek,
                    .Year = x.Year}
                vContext.resourceschedules.AddObject(vResourceSchedule)
                vResourceSchedule.venues.Add(xVenue)
                vContext.SaveChanges()
            Next
        Next
    End Sub

    Sub scheduleSlots(ByRef vUsedResourceSlots As HashSet(Of sSlot),
                      ByRef vUsedVenueSlots As HashSet(Of sSlot),
                      ByRef MarkedSlots As HashSet(Of sSlot),
                      ByVal vOfferingType As offeringtype,
                      ByVal vResource As resource,
                      ByVal vClassID As Integer,
                      ByVal vVenueID As Integer,
                      ByRef savedSlots As Integer,
                      ByVal vDays() As Integer,
                      ByVal vTimeSlots As List(Of Integer),
                      ByVal vAmtTimeSlots As Integer,
                      ByVal vYear As Integer,
                      ByVal iStartWeek As Integer,
                      ByRef vLecturerNotRostered As Boolean)
        With vResource
            If vTimeSlots.Count = 0 Then
                Exit Sub
            End If
            Dim vEndTimeSlot = vTimeSlots.Last
            For Each vday As Integer In vDays
                For Each vSlot In vTimeSlots
                    '''''''''''''''''''''''''''Time Slots ''''''''''''''''''''''''''''''''''
                    If savedSlots >= vAmtTimeSlots Then
                        Exit For
                    End If

                    If vSlot > vEndTimeSlot Then
                        Exit For
                    End If

                    'only schedule once for a day. goto next day if already scheduled on this day
                    If Not isResourceDayFree(vYear, iStartWeek, vday, .ID) Then
                        Exit For
                    End If

                    'create time slot structure
                    Dim xSlot As New sSlot With {.Year = vYear, .Week = iStartWeek, .Day = vday, .timeslot = vSlot}

                    'check if lecturer is rostered
                    If Not IsLecturerRostered(xSlot, vClassID) Then
                        'need to roster lecturers first.
                        Continue For
                    Else
                        vLecturerNotRostered = True
                    End If

                    '''''''''''''''''''''''''''''''''''''TIME PERIOD''''''''''''''''''''''''
                    Dim RemSlots = vAmtTimeSlots - savedSlots
                    If IsSlotFree(vUsedResourceSlots,
                                  vUsedVenueSlots,
                                  xSlot,
                                  vEndTimeSlot,
                                  .MaxMergedTimeSlots,
                                  RemSlots) Then
                        'createSlot(vUsedResourceSlots, vUsedVenueSlots, savedSlots, vVenueID, .ID, xSlot, iEndWeek, .MaxMergedTimeSlots, RemSlots)
                        MarkSlots(vUsedResourceSlots, vUsedVenueSlots, MarkedSlots, savedSlots, vVenueID, .ID, xSlot, .MaxMergedTimeSlots, RemSlots)
                    End If
                    '''''''''''''''''''''''''''''''''''''END TIME PERIOD''''''''''''''''''''''''
                Next
                ''''''''''''''''''''''''''''''''''END DAY''''''''''''''''''''''''
            Next
        End With
    End Sub



    'get slots of venue that are already assigned
    Sub setUsedVenueSlots(ByRef UsedVenueSlots As HashSet(Of sSlot), ByVal vvenueID As Integer, ByVal xYear As Integer, ByVal xWeek As Integer)
        Dim vContext As timetableEntities = New timetableEntities()
        '''''''''''''''''''''''
        'get venue  Timetable
        Dim xVenueSchedules = (From p In vContext.venues Where p.ID = vvenueID Select p.resourceschedules).FirstOrDefault

        If Not IsNothing(xVenueSchedules) Then
            For Each x In xVenueSchedules
                If x.Year = xYear And x.Week = xWeek Then
                    Dim vSlot As New sSlot With {
                        .Year = xYear,
                        .Week = xWeek,
                        .timeslot = x.timeSlotID,
                        .Day = x.Day}
                    UsedVenueSlots.Add(vSlot)
                End If
            Next
        End If
    End Sub


    Function IsLecturerRostered(ByVal vSlot As sSlot, ByVal vClassID As Integer) As Boolean
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vClass = (From p In vContext.classgroups Where p.ID = vClassID Select p).First
        Dim vClusterID = vClass.siteclustersubject.sitecluster.ID
        'Dim vRoster = vClass.lecturer.lecturersiteclusteravailabilities
        'see if the lecturer roster is setup
        'check for Null
        Try
            '''''''return true if there is no roster
            If IsNothing(vClass.lecturer.lecturersiteclusteravailabilities) Then
                Return True
            End If
        Catch ex As NullReferenceException
            Return True
        Catch ex As Exception
            Throw ex
        End Try
        Dim vSlotRoster = (From p In vClass.lecturer.lecturersiteclusteravailabilities
                    Where p.SiteClusterID = vClusterID And
                          p.DayOfWeek = vSlot.Day And
                          p.StartTimeSlot <= vSlot.timeslot And
                          p.EndTimeSlot >= vSlot.timeslot
                          Select p).ToList
        If vSlotRoster.Count = 0 Then
            Return False
        Else
            Return True
        End If
    End Function

    'get slots of resource that are already assigned by same qualification, lecturer, classgroup etc
    Sub setUsedResourceSlots(ByRef UsedResourceSlots As HashSet(Of sSlot), ByVal vResourceID As Integer, ByVal vYear As Integer, ByVal vWeek As Integer)
        Dim vContext As timetableEntities = New timetableEntities()

        Dim vResource = (From p In vContext.resources Where p.ID = vResourceID Select p).First

        'add all resource slots
        Dim vResourceSchedules = (From p In vResource.resourceschedules Where p.Year = vYear And p.Week = vWeek Select p).ToList
        For Each x In vResourceSchedules
            Dim vSlot As New sSlot With {
                .Year = vYear,
                .Week = vWeek,
                .timeslot = x.timeSlotID,
                .Day = x.Day}
            UsedResourceSlots.Add(vSlot)
        Next

        'get list of all associtaed classgroups
        For Each x In vResource.classgroups
            Dim vClassGroup = x
            'get each vclassgroup schedules
            Dim xResources = (From p In vClassGroup.resources Select p).ToList
            For Each i In xResources
                Dim iSchedules = (From p In i.resourceschedules Where p.Year = vYear And p.Week = vWeek Select p).ToList
                For Each k In iSchedules
                    Dim vSlot As New sSlot With {
                        .Year = vYear,
                        .Week = vWeek,
                        .timeslot = k.timeSlotID,
                        .Day = k.Day}
                    UsedResourceSlots.Add(vSlot)
                Next
            Next

            'get lecturer Timetable
            Dim vlecturer = vClassGroup.lecturer
            If Not IsNothing(vlecturer) Then
                Dim vLecturerID = vlecturer.LecturerID
                Dim lclasses = (From p In vContext.classgroups Where p.lecturer.LecturerID = vLecturerID Select p).ToList
                For Each j In lclasses
                    Dim jClass = j
                    Dim jResources = jClass.resources
                    For Each i In jResources
                        Dim iSchedules = (From p In i.resourceschedules Where p.Year = vYear And p.Week = vWeek Select p).ToList
                        For Each k In iSchedules
                            Dim vSlot As New sSlot With {
                                .Year = vYear,
                                .Week = vWeek,
                                .timeslot = k.timeSlotID,
                                .Day = k.Day}
                            UsedResourceSlots.Add(vSlot)
                        Next
                    Next
                Next
            End If

            'get class group subject
            Dim vSubjectID = vClassGroup.SubjectID
            'get qualifications TimeTable
            Dim vQualList = (From p In vContext.programmesubjects Where p.SubjectID = vSubjectID Select p).ToList
            For Each y In vQualList
                Dim vQual = y
                'get subject list except classgroup subject
                Dim SubjectList = (From p In vContext.programmesubjects
                                     Where p.QualID = vQual.QualID And
                                           p.SubjectID <> vSubjectID Select p).ToList
                For Each z In SubjectList
                    Dim vSubj = z
                    Dim vClassList = (From p In vContext.classgroups Where p.SubjectID = vSubj.SubjectID Select p).ToList
                    For Each j In vClassList
                        Dim jClass = j
                        Dim jResources = jClass.resources
                        For Each i In jResources
                            Dim iSchedules = (From p In i.resourceschedules Where p.Year = vYear And p.Week = vWeek Select p).ToList
                            For Each k In iSchedules
                                Dim vSlot As New sSlot With {
                                    .Year = vYear,
                                    .Week = vWeek,
                                    .timeslot = k.timeSlotID,
                                    .Day = k.Day}
                                UsedResourceSlots.Add(vSlot)
                            Next
                        Next
                    Next
                Next
            Next
        Next
    End Sub


    ''check of the resource already set for the specified day
    Function isResourceDayFree(ByVal vYear As Integer, ByVal vWeek As Integer, ByVal vDay As Integer, ByVal vResourceID As Integer) As Boolean
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vDayCount = (From p In vContext.resourceschedules
                           Where p.Year = vYear And
                                 p.Week = vWeek And
                                 p.Day = vDay And
                                 p.ResourceID = vResourceID Select p).Count
        If vDayCount = 0 Then
            Return True
        Else
            Return False
        End If
    End Function


    Function getLunchPeriod() As Integer
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vTimeslot = (From p In vContext.timeslots
                Where p.LunchPeriod = True Select p).FirstOrDefault
        Return CInt(IIf(IsNothing(vTimeslot), 0, vTimeslot.ID))
    End Function

    Function IsSlotFree(ByVal vUsedResourceSlots As HashSet(Of sSlot),
                        ByVal vUsedVenueSlots As HashSet(Of sSlot),
                        ByVal vSlot As sSlot,
                        ByVal vOfferingEndSlot As Integer,
                        ByVal vMaxMergedTimeSlots As Integer,
                        ByVal vRemTimeSlots As Integer) As Boolean
        Dim vLunchPeriod As Integer = getLunchPeriod()

        Dim vAllocatedSlots = CInt(IIf(vRemTimeSlots > vMaxMergedTimeSlots, vMaxMergedTimeSlots, vRemTimeSlots))

        ''''''ensure that consecutive periods completed before end of offering Timeslot
        If vSlot.timeslot + vAllocatedSlots - 1 > vOfferingEndSlot Then
            Return False
        End If

        ''''''ensure that periods are not interrupted by lunch
        If vLunchPeriod >= vSlot.timeslot And
           vLunchPeriod <= vAllocatedSlots + vSlot.timeslot - 1 Then
            Return False
        End If


        ''''check if resources are available 
        For jTimeSlot As Integer = vSlot.timeslot To vSlot.timeslot + vAllocatedSlots - 1
            Dim jSlot = New sSlot With {.Year = vSlot.Year, .Week = vSlot.Week, .timeslot = jTimeSlot, .Day = vSlot.Day}
            If vUsedResourceSlots.Contains(jSlot) Then
                Return False
            End If
            If vUsedVenueSlots.Contains(jSlot) Then
                Return False
            End If
        Next

        Return True
    End Function


    Sub MarkSlots(ByRef UsedResourceSlots As HashSet(Of sSlot), ByRef UsedVenueSlots As HashSet(Of sSlot), ByRef MarkedSlots As HashSet(Of sSlot), ByRef vSavedSlots As Integer, ByVal vVenueID As Integer, ByVal vResourceID As Integer, ByVal vSlot As sSlot, ByVal vMaxMergedTimeSlot As Integer, ByVal vRemTimeSlots As Integer)
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vVenue = (From p In vContext.venues Where p.ID = vVenueID Select p).First
        Dim vAllocatedSlots As Integer

        'determine how smuch slots can be saved
        If vRemTimeSlots > vMaxMergedTimeSlot Then
            vAllocatedSlots = vMaxMergedTimeSlot
        Else
            vAllocatedSlots = vRemTimeSlots
        End If

        For i As Integer = 0 To vAllocatedSlots - 1
            'mark slots
            Dim newSlot As New sSlot With {
                .timeslot = vSlot.timeslot + i,
                .Day = vSlot.Day,
                .Week = vSlot.Week,
                .Year = vSlot.Year}
            UsedResourceSlots.Add(newSlot)
            UsedVenueSlots.Add(newSlot)
            MarkedSlots.Add(newSlot)
            vSavedSlots = vSavedSlots + 1
        Next
    End Sub


    Sub createSlot(ByRef UsedResourceSlots As HashSet(Of sSlot), ByRef UsedVenueSlots As HashSet(Of sSlot), ByRef vSavedSlots As Integer, ByVal vVenueID As Integer, ByVal vResourceID As Integer, ByVal vSlot As sSlot, ByVal iEndWeek As Integer, ByVal vMaxMergedTimeSlot As Integer, ByVal vRemTimeSlots As Integer)
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vVenue = (From p In vContext.venues Where p.ID = vVenueID Select p).First
        Dim vAllocatedSlots As Integer

        'determine how smuch slots can be saved
        If vRemTimeSlots > vMaxMergedTimeSlot Then
            vAllocatedSlots = vMaxMergedTimeSlot
        Else
            vAllocatedSlots = vRemTimeSlots
        End If

        For i As Integer = 0 To vAllocatedSlots - 1
            'Try
            For jWeek = vSlot.Week To iEndWeek
                Dim vResourceSchedule = New resourceschedule With {
                    .ResourceID = vResourceID,
                    .timeSlotID = vSlot.timeslot + i,
                    .Day = vSlot.Day,
                    .Week = jWeek,
                    .Year = vSlot.Year}
                vContext.resourceschedules.AddObject(vResourceSchedule)
                vResourceSchedule.venues.Add(vVenue)
                vContext.SaveChanges()
            Next
            ''''''set new UsedResourceSlots
            Dim newSlot As New sSlot With {.timeslot = vSlot.timeslot + i, .Day = vSlot.Day, .Week = vSlot.Week, .Year = vSlot.Year}
            UsedResourceSlots.Add(newSlot)
            UsedVenueSlots.Add(newSlot)
            vSavedSlots = vSavedSlots + 1
        Next
    End Sub

    Private Sub cboYear_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cboYear.SelectedIndexChanged
        getBlock()
    End Sub
End Class