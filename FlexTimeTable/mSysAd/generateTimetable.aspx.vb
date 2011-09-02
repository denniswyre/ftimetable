Public Class generateTimetable
    Inherits System.Web.UI.Page


    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            getyear()
            getfaculties()
            getClusters()
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
        Dim WeekDay As Integer
        Dim timeslot As Integer
    End Structure

    Function getQualifiedRooms(ByVal vResourceID As Integer) As ArrayList
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vResource = (From p In vContext.resources Where p.ID = vResourceID Select p).First
        Dim vResourceSize = vResource.AmtParticipants
        Dim venueList As New ArrayList
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
                                     Where p.resourcetype.ID = vResource.ResourceTypeID And
                                           p.Capacity >= vResourceSize Select p).ToList
                        venueList.Add(x.ID)
                    Next
                Next
            Next
        End If
        ' If venueList.Count = 0 Then
        'Throw New Exception("No Available venue. Check Resource Type!!!")
        'End If
        Return venueList 'As New ArrayList
    End Function

    Protected Sub btnGenerate_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnGenerate.Click
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vClassGroups As List(Of classgroup)
        If chkAllCluster.Checked And chkAllFaculty.Checked Then
            vClassGroups = (From p In vContext.classgroups Select p).ToList
        ElseIf chkAllCluster.Checked And Not chkAllFaculty.Checked Then
            vClassGroups = (From p In vContext.classgroups
                                Where p.siteclustersubject.subject.department.school.facultyID =
                                    CInt(cboFaculty.SelectedValue)
                                        Select p).ToList
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
        Dim vEndWeek = vBlock.endWeek
        Dim vStartWeek As Integer
        If Year(Now) = vYear And vCurWeek > vBlock.startWeek Then
            vStartWeek = vCurWeek
        Else  ''''greater
            vStartWeek = vBlock.startWeek
        End If
        Dim weekBoundaries = getBlockBoundaries(vStartWeek)

        Dim AssignedClassList As New List(Of Integer)
        Dim unAssignedClassList As New List(Of Integer)


        separateClassGroups(vClassGroups, AssignedClassList, unAssignedClassList)

        ' schedule those with assigned venues first
        For Each i In AssignedClassList
            Dim iClassID = i
            Dim iClass = (From p In vContext.classgroups Where p.ID = iClassID Select p).Single
            For Each x In (From p In iClass.resources
                           Where p.year = vYear Select p).ToList
                scheduleResource(x.ID, iClass, vYear, vStartWeek, vEndWeek, weekBoundaries)
            Next
        Next

        ' schedule those with unassigned venues last
        For Each i In unAssignedClassList
            Dim iClassID = i
            Dim iClass = (From p In vContext.classgroups Where p.ID = iClassID Select p).Single
            For Each x In (From p In iClass.resources
                           Where p.year = vYear Select p).ToList
                scheduleResource(x.ID, iClass, vYear, vStartWeek, vEndWeek, weekBoundaries)
            Next
        Next

    End Sub


    Sub separateClassGroups(ByVal vClassgroupList As List(Of classgroup), ByRef AssignedClassList As List(Of Integer), ByRef UnAssignedClassList As List(Of Integer))
        Dim vContext As timetableEntities = New timetableEntities()
        ''Give priority to those who have preferrred rooms
        For Each x In vClassgroupList
            Dim ClusterID = x.SiteClusterID
            Dim IsVenueAssigned = False
            For Each y In x.siteclustersubject.subject.department.venues
                If ClusterID = y.building.site.SiteClusterID Then
                    IsVenueAssigned = True
                    Exit For
                End If
            Next
            If IsVenueAssigned Then
                AssignedClassList.Add(x.ID)
            Else
                UnAssignedClassList.Add(x.ID)
            End If
        Next
    End Sub


    'get the endweeks of all atomic blocks from startweek to end of year 
    Function getBlockBoundaries(ByVal vStartweek As Integer) As HashSet(Of Integer)
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vBlocks = (From p In vContext.academicblocks Order By p.endWeek Select p).ToList
        Dim vBoundaries As New HashSet(Of Integer)
        Dim StartInserted As Boolean = False
        For Each x In vBlocks
            If vStartweek < x.endWeek Then
                If Not StartInserted Then
                    vBoundaries.Add(vStartweek)
                End If
                vBoundaries.Add(x.endWeek)
            End If
        Next
        Return vBoundaries
    End Function


    Protected Sub scheduleResource(ByVal vResourceID As Integer, ByVal vClassgroup As classgroup, ByVal vYear As Integer, ByVal vBlockStartWeek As Integer, ByVal vBlockEndWeek As Integer, ByVal vWeekBoundaries As HashSet(Of Integer))
        Dim vContext As timetableEntities = New timetableEntities()
        'Dim vClassgroup = (From p In vContext.classgroups Where p.ID = vClassID Select p).First
        Dim vOfferingType = vClassgroup.offeringtype
        Dim vResource = (From p In vContext.resources Where p.ID = vResourceID Select p).First
        Dim vDays() As Integer = {2, 4, 3, 5, 6, 7}
        ' Try
        With vResource
            For i As Integer = 0 To vWeekBoundaries.Count - 2
                Dim iStartWeek = CInt(IIf(i = 0, vWeekBoundaries(i), vWeekBoundaries(i) + 1))
                Dim iEndWeek = vWeekBoundaries(i + 1)
                'exit resource if it doesn't contain this week
                If Not (iStartWeek >= .startWeek And iStartWeek <= .endWeek) Then Continue For
                '''''''''''''''''''''''''''Get Existing Booked Periods ''''''''''''''''''''''''''
                Dim vBookedTimeSlots As Integer = (From p In vContext.resourceschedules Where p.ResourceID = .ID And p.Year = vYear And p.Week = iStartWeek Select p).Count
                '''''get the required periods
                Dim vReqTimeSlots As Integer = .AmtTimeSlots - vBookedTimeSlots
                '''''do not genertate if timetable already created
                If vReqTimeSlots = 0 Then Continue For

                'start index
                Dim vSlotIndex As Integer = 0

                ''get all used resource slots
                Dim vUsedResourceSlots As New HashSet(Of sSlot)
                setUsedResourceSlots(vUsedResourceSlots, .ID, vYear, iStartWeek)

                If vSlotIndex >= vReqTimeSlots Then Continue For
                For Each vVenueID In getQualifiedRooms(.ID)
                    ''''''''''''''''''''''''''START Qualified ROOMs Loop''''''''''''''''
                    Dim vUsedVenueSlots As New HashSet(Of sSlot)
                    setUsedVenueSlots(vUsedVenueSlots, CInt(vVenueID), .year, iStartWeek)
                    ''get timeslots
                    For Each vday As Integer In vDays
                        ''''''''''''''''''''''''''DAY OF WEEK '''''''''''''''''''''''''''''
                        'set timeslots according to offering type
                        Dim vTimeslots = (From p In vContext.timeslots
                                                Where p.ID >= vOfferingType.startTimeSlot And
                                                      p.ID <= vOfferingType.endTimeSlot
                                                      Select p).ToList
                        Dim vEndTimeSlot = vOfferingType.endTimeSlot
                        If vday = 7 Then
                            'schedule on saturday only if offering type allows this
                            If vOfferingType.SabbathClasses Then
                                vTimeslots = (From p In vContext.timeslots
                                                Where p.ID >= vOfferingType.sabStartTimeSlot And
                                                      p.ID >= vOfferingType.sabEndTimeSlot
                                                      Select p).ToList
                                vEndTimeSlot = vOfferingType.sabEndTimeSlot
                            Else
                                Continue For
                            End If
                        End If
                        For Each vSlot In vTimeslots
                            If vSlot.ID > vEndTimeSlot Then
                                Exit For
                            End If
                            If vSlotIndex >= vReqTimeSlots Then
                                Exit For
                            End If
                            'only schedule once for a day. goto next day if already scheduled on this day
                            If Not isResourceDayFree(vYear, iStartWeek, vday, .ID) Then
                                Exit For
                            End If
                            'create time slot structure
                            Dim xSlot As New sSlot With {.Year = vYear, .Week = iStartWeek, .WeekDay = vday, .timeslot = vSlot.ID}
                            'check if lecturer is rostered
                            If Not IsLecturerRostered(xSlot, vClassgroup.ID) Then
                                'need to roster lecturers first.
                                'Continue For 
                            End If
                            '''''''''''''''''''''''''''''''''''''TIME PERIOD''''''''''''''''''''''''
                            If IsSlotFree(vUsedResourceSlots,
                                          vUsedVenueSlots,
                                          xSlot,
                                          vEndTimeSlot,
                                          .MaxMergedTimeSlots,
                                          vReqTimeSlots - vSlotIndex) Then
                                createSlot(vUsedResourceSlots, vSlotIndex, CInt(vVenueID), .ID, xSlot, iEndWeek, .MaxMergedTimeSlots, vReqTimeSlots - vSlotIndex)
                            End If
                        Next
                        '''''''''''''''''''''''''''''''''''''END TIME PERIOD''''''''''''''''''''''''
                    Next
                    ''''''''''''''''''''''''''END Qualified ROOMs Loop''''''''''''''
                Next
                ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            Next
        End With  'vSubGrDRow
    End Sub



    'get slots of venue that are already assigned
    Sub setUsedVenueSlots(ByRef UsedSlots As HashSet(Of sSlot), ByVal vvenueID As Integer, ByVal xYear As Integer, ByVal xWeek As Integer)
        Dim vContext As timetableEntities = New timetableEntities()
        'get venue  Timetable
        Dim xVenueSchedules = (From p In vContext.venues Where p.ID = vvenueID Select p.resourceschedules).FirstOrDefault
        '  Dim xVenueSchedules = (From p In vContext.resourceschedules
        '                          Where p.venues..Contains(vVenue) Select p)
        If Not IsNothing(xVenueSchedules) Then
            For Each x In xVenueSchedules
                If x.Year = xYear And x.Week = xWeek Then
                    Dim vSlot As New sSlot With {
                        .timeslot = x.timeSlotID,
                        .WeekDay = x.Day}
                    AddUsedSlots(UsedSlots, vSlot)
                End If
            Next
        End If
    End Sub

    Function IsLecturerRostered(ByVal vSlot As sSlot, ByVal vClassID As Integer) As Boolean
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vClass = (From p In vContext.classgroups Where p.ID = vClassID Select p).First
        Dim vClusterID = vClass.siteclustersubject.sitecluster.ID
        'Dim vRoster = vClass.lecturer.lecturersiteclusteravailabilities
        Dim vAvail = (From p In vClass.lecturer.lecturersiteclusteravailabilities
                         Where p.SiteClusterID = vClusterID And
                               p.DayOfWeek = vSlot.WeekDay And
                               p.StartTimeSlot <= vSlot.timeslot And
                               p.EndTimeSlot >= vSlot.timeslot
                               Select p).FirstOrDefault
        If IsNothing(vAvail) Then
            Return False
        Else
            Return True
        End If
    End Function

    'get slots of resource that are already assigned by same qualification, lecturer, classgroup etc
    Sub setUsedResourceSlots(ByRef UsedSlots As HashSet(Of sSlot), ByVal vResourceID As Integer, ByVal vYear As Integer, ByVal vWeek As Integer)
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vResource = (From p In vContext.resources Where p.ID = vResourceID Select p).First
        Dim vResourceSchedules = (From p In vResource.resourceschedules Where p.Year = vYear And p.Week = vWeek Select p).ToList
        For Each x In vResourceSchedules
            Dim vSlot As New sSlot With {
                .timeslot = x.timeSlotID,
                .WeekDay = x.Day}
            AddUsedSlots(UsedSlots, vSlot)
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
                        .timeslot = k.timeSlotID,
                        .WeekDay = k.Day}
                    AddUsedSlots(UsedSlots, vSlot)
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
                                .timeslot = k.timeSlotID,
                                .WeekDay = k.Day}
                            AddUsedSlots(UsedSlots, vSlot)
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
                    Dim vClassList = (From p In vContext.classgroups Where p.SubjectID = vSubj.QualID Select p).ToList
                    For Each j In vClassList
                        Dim jClass = j
                        Dim jResources = jClass.resources
                        For Each i In jResources
                            Dim iSchedules = (From p In i.resourceschedules Where p.Year = vYear And p.Week = vWeek Select p).ToList
                            For Each k In iSchedules
                                Dim vSlot As New sSlot With {
                                    .timeslot = k.timeSlotID,
                                    .WeekDay = k.Day}
                                AddUsedSlots(UsedSlots, vSlot)
                            Next
                        Next
                    Next
                Next
            Next
        Next
    End Sub


    Sub AddUsedSlots(ByRef UsedSlots As HashSet(Of sSlot), ByVal NewSlot As sSlot)
        If Not UsedSlots.Contains(NewSlot) Then
            UsedSlots.Add(NewSlot)
        End If
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
        If vAllocatedSlots + vSlot.timeslot - 1 > vOfferingEndSlot Then
            Return False
        End If

        ''''''ensure that periods are not interrupted by lunch
        If vLunchPeriod >= vSlot.timeslot And
           vLunchPeriod <= vAllocatedSlots + vSlot.timeslot - 1 Then
            Return False
        End If


        ''''check if resources are available 
        For j As Integer = 0 To vAllocatedSlots - 1
            Dim jSlot = New sSlot With {.timeslot = vSlot.timeslot + j, .WeekDay = vSlot.WeekDay}
            If vUsedResourceSlots.Contains(jSlot) Then
                Return False
            End If
            If vUsedVenueSlots.Contains(jSlot) Then
                Return False
            End If
        Next

        Return True
    End Function

    Sub createSlot(ByRef UsedSlots As HashSet(Of sSlot), ByRef vIndex As Integer, ByVal vVenueID As Integer, ByVal vResourceID As Integer, ByVal vSlot As sSlot, ByVal iEndWeek As Integer, ByVal vMaxMergedTimeSlot As Integer, ByVal vRemTimeSlots As Integer)
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vVenue = (From p In vContext.venues Where p.ID = vVenueID Select p).First
        Dim vAllocatedSlots As Integer
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
                    .Day = vSlot.WeekDay,
                    .Week = jWeek,
                    .Year = vSlot.Year}
                vContext.resourceschedules.AddObject(vResourceSchedule)
                vResourceSchedule.venues.Add(vVenue)
                vContext.SaveChanges()
            Next
            ''''''set new usedslots
            Dim newSlot As New sSlot With {.timeslot = vSlot.timeslot + i, .WeekDay = vSlot.WeekDay, .Week = vSlot.Week, .Year = vSlot.Year}
            AddUsedSlots(UsedSlots, newSlot)
            vIndex = vIndex + 1
        Next
    End Sub

    Private Sub cboYear_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cboYear.SelectedIndexChanged
        getBlock()
    End Sub
End Class