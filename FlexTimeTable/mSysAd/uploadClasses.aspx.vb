Partial Class uploadClasses
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            PrepareFile()
        End If
    End Sub

    Protected Sub PrepareFile()
        'create valid fields
        uploadFile1.filetotable = New clsfiletotableconversion
        'Dim vfiletotable As New clsfiletotableconversion
        With uploadFile1.filetotable
            .fields = New ListItemCollection()
            ' Add items to the collection.
            .fields.Add(New ListItem("subjectcode", "0"))
            .fields.Add(New ListItem("level", "0"))
            .fields.Add(New ListItem("blockcode", "0"))
            .fields.Add(New ListItem("qualcode", "0"))
            .fields.Add(New ListItem("classgroup", "0"))
            .fields.Add(New ListItem("classsize", "0"))
            .fields.Add(New ListItem("timeslots", "0"))
            .fields.Add(New ListItem("venuetype", "0"))
            .fields.Add(New ListItem("lusername", "0"))
            .fields.Add(New ListItem("deliverysite", "0"))
            .fields.Add(New ListItem("periodtype", "0"))
            .fields.Add(New ListItem("practicals", "0"))
            .fields.Add(New ListItem("pvenuetype", "0"))
            .fields.Add(New ListItem("prefvenue", "0"))
        End With
        uploadFile1.header = "Upload Class Groups File"
        uploadFile1.Initialize()
    End Sub

    Private Sub uploadFile1_UploadComplete(ByVal sender As Object, ByVal e As System.EventArgs) Handles uploadFile1.UploadComplete
        processFile(uploadFile1.filetotable.pDatatable)
    End Sub

    Protected Sub processFile(ByVal vDataTable As DataTable)
        Dim RowIndex As Integer = 0
        Dim MaxPeriods As Integer = CType(ConfigurationManager.AppSettings("maxperiods"), Integer)
        Dim MaxClassSize As Integer = CType(ConfigurationManager.AppSettings("maxclasssize"), Integer)

        Dim vContext As timetableEntities = New timetableEntities()
        'get data table from user control

        Dim prevError As String = ""
        For Each dr As DataRow In vDataTable.Rows
            'essential fields
            Dim vSubjectcode As String
            Dim vLevel As Integer
            Dim vBlockID As Integer
            Dim vQualcode As String
            Dim vClassCode As String
            Dim vClasssize As Integer
            Dim vTimeslots As Integer
            Dim vVenuetype As String
            Dim vDeliverysite As String
            ' non essential fields
            Dim vLusername As String
            Dim vPeriodType As String
            Dim vPTimeSlots As Integer
            Dim vPVenueType As String
            Dim vPrefVenue As String
            Dim vOfferingTypeID As Integer = getOfferingTypeID("")  'need to look at this
            Try
                'essential fields
                vSubjectcode = clsGeneral.correctCode(CType(dr("subjectcode"), String))
                vLevel = clsGeneral.correctSubjectLevel(CType(dr("level"), String))
                vBlockID = getBlockID(CType(dr("blockcode"), String))
                vQualcode = Trim(CType(dr("qualcode"), String))
                vClassCode = clsGeneral.correctCode(CType(dr("classgroup"), String))
                vClasssize = CType(dr("classsize"), Integer)
                vTimeslots = CType(dr("timeslots"), Integer)
                vVenuetype = CType(dr("venuetype"), String)
                vDeliverysite = CType(dr("deliverysite"), String)
                ' non essential fields
                Try
                    vLusername = CType(dr("lusername"), String)
                Catch ex As Exception
                    vLusername = Nothing
                End Try
                Try
                    vPeriodType = CType(dr("periodtype"), String)
                Catch ex As Exception
                    vPeriodType = Nothing
                End Try
                Try
                    vPTimeSlots = CType(dr("practicals"), Integer)
                Catch ex As Exception
                    vPTimeSlots = 0
                End Try
                Try
                    vPVenueType = CType(dr("pvenuetype"), String)
                Catch ex As Exception
                    vPVenueType = Nothing
                End Try
                Try
                    vPrefVenue = CType(dr("prefvenue"), String)
                Catch ex As Exception
                    vPrefVenue = Nothing
                End Try

                'get key IDs
                Dim vSite As site
                Try
                    vSite = (From p In vContext.sites Where p.longName = vDeliverysite Select p).First
                Catch ex As Exception
                    Throw New Exception("DeliverySite Error:" + ex.Message)
                End Try
                Dim vSiteCluster = (From p In vContext.sites Where p.longName = vDeliverysite Select p.sitecluster).First

                'qualification
                Dim vQual As qualification
                Try
                    vQual = (From p In vContext.qualifications Where p.Code.Contains(vQualcode) Select p).First
                Catch ex As Exception
                    Throw New Exception("Qualification Error:" + ex.Message)
                End Try
                'subject
                Dim vSubject As subject
                Try
                    vSubject = (From p In vContext.subjects Where p.Code = vSubjectcode Select p).First
                Catch ex As Exception
                    Throw New Exception("Subject Error:" + ex.Message)
                End Try
                ''''''set programmesubjects
                Dim vProgrammeSubject = (From p In vContext.programmesubjects
                                         Where p.QualID = vQual.ID And
                                         p.SubjectID = vSubject.ID Select p).FirstOrDefault
                If IsNothing(vProgrammeSubject) Then
                    vProgrammeSubject = New programmesubject With {
                        .QualID = vQual.ID,
                        .SubjectID = vSubject.ID,
                        .Level = vSubject.Level}
                    vContext.programmesubjects.AddObject(vProgrammeSubject)
                    vContext.SaveChanges()
                End If
                'set siteclusterprogramme
                If Not vSiteCluster.qualifications.Contains(vQual) Then
                    vSiteCluster.qualifications.Add(vQual)
                    vContext.SaveChanges()
                End If
                'set siteclustersubject
                Dim vClusterSubject = (From p In vContext.siteclustersubjects
                                       Where p.SiteClusterID = vSite.SiteClusterID And
                                       p.SubjectID = vSubject.ID Select p).FirstOrDefault
                If IsNothing(vClusterSubject) Then
                    vClusterSubject = New siteclustersubject With {
                        .SiteClusterID = vSite.SiteClusterID,
                        .SubjectID = vSubject.ID}
                    vContext.siteclustersubjects.AddObject(vClusterSubject)
                    vContext.SaveChanges()
                End If
                'class group
                Dim vClassgrp = (From p In vContext.classgroups
                                 Where p.code = vClassCode And
                                       p.SiteClusterID = vSite.SiteClusterID And
                                       p.SubjectID = vSubject.ID Select p).FirstOrDefault
                If IsNothing(vClassgrp) Then
                    vClassgrp = New classgroup With {
                        .code = vClassCode,
                        .SiteClusterID = vSite.SiteClusterID,
                        .SubjectID = vSubject.ID,
                        .AcademicBlockID = vBlockID,
                        .classSize = vClasssize,
                        .OfferingTypeID = vOfferingTypeID,
                        .TimeSlotTotal = vTimeslots}
                    vContext.classgroups.AddObject(vClassgrp)
                    vContext.SaveChanges()
                End If
                ' set lecturer
                Dim vLecturerID = getLecturer(vLusername)
                If vLecturerID = 0 Then
                    uploadFile1.errorlist = New ListItem("Row No:" + CStr(RowIndex + 1) + "-->err:" + "Lecturer does not exist")
                Else
                    Dim vLecturer = (From p In vContext.lecturers Where p.LecturerID = vLecturerID Select p).FirstOrDefault
                    Try
                        If IsNothing(vLecturer) Then
                            vLecturer = New lecturer With {
                                       .LecturerID = vLecturerID,
                                       .DepartmentID = vSubject.DepartmentID}
                            vContext.lecturers.AddObject(vLecturer)
                        End If
                        vLecturer.subjects.Add(vSubject)
                        vLecturer.classgroups.Add(vClassgrp)
                        vContext.SaveChanges()
                    Catch ex As Exception
                        uploadFile1.errorlist = New ListItem("Row No:" + CStr(RowIndex + 1) + "-->err:Lecturer Error:" + ex.Message)
                    End Try
                End If
                ' resources
                Dim vResourceCnt = getGetResourceCount(vClassgrp.ID)
                Dim vResourceTypeID = getResourceTypeID(vVenuetype)
                Dim vMaxMergedTimeSlots = GetMaxMergedTimeSlots(vPeriodType, vTimeslots)
                Dim vResourceName = vSubjectcode + "[" + vClassgrp.code + "]"
                If vPTimeSlots > 0 And vResourceCnt = 0 Then
                    'only if practical(lab) details exists
                    CreateResource(vClassgrp.ID, vResourceName, vResourceTypeID, vClasssize, vTimeslots, vMaxMergedTimeSlots)
                    ''deal with practicals later
                    Dim vPResourceTypeID = getResourceTypeID(vPVenueType)
                    ' Dim LabPeriods As Integer = vPTimeSlots
                    If vPResourceTypeID > 0 And vPTimeSlots > 0 Then
                        Dim nlabsize As Integer = CType(ConfigurationManager.AppSettings("nlabsize"), Integer)
                        ''''Get Number of lab groups
                        Dim NoOfLabs As Integer = CType(vClasssize / nlabsize, Integer) + 1
                        For i As Integer = 1 To NoOfLabs
                            Dim LabClassSize As Integer
                            vResourceName = vResourceName + "-lab" + i.ToString
                            If i = NoOfLabs Then
                                LabClassSize = vClasssize - (NoOfLabs - 1) * CType(vClasssize / NoOfLabs, Integer)
                            Else
                                LabClassSize = CType(vClasssize / NoOfLabs, Integer)
                            End If
                            CreateResource(vClassgrp.ID, vResourceName, vPResourceTypeID, LabClassSize, vPTimeSlots, 2)
                        Next
                    End If
                ElseIf vPTimeSlots = 0 Then
                    ' no practical(lab) details exist
                    vResourceName = vResourceName + "-" + CStr(vResourceCnt)
                    Dim TotalNoOfResources = getTotalSubGroup(vDataTable, vSubjectcode, vClassCode, vDeliverysite, vLevel)
                    If vResourceCnt < TotalNoOfResources Then
                        CreateResource(vClassgrp.ID, vResourceName, vResourceTypeID, vClasssize, vTimeslots, vMaxMergedTimeSlots)
                    End If
                End If
            Catch ex As Exception
                uploadFile1.errorlist = New ListItem("Row No:" + CStr(RowIndex + 1) + "-->err:" + ex.Message)
            End Try
            RowIndex += 1
        Next
    End Sub

    Function getGetResourceCount(ByVal vClassID As Integer) As Integer
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vClassgrp = (From p In vContext.classgroups
                             Where p.ID = vClassID Select p).First
        Return vClassgrp.resources.Count
    End Function

    Function getLecturer(ByVal lusername As String) As Integer
        Dim vContext As timetableEntities = New timetableEntities()
        'lecturer, officer user ldap
        Dim vOfficer As clsOfficer.sOfficer = clsOfficer.getOfficer(lusername)
        If vOfficer.ID = 0 Then
            vOfficer = ldap1.getUserDetails(lusername)
            If vOfficer.DistinquishedName <> "" Then
                'check if user exists 
                Dim userInfo As MembershipUser = Membership.GetUser(lusername)
                If userInfo Is Nothing Then
                    'create new membership
                    Dim dummyPassword As String = clsGeneral.getRandomStr(12)
                    userInfo = Membership.CreateUser(lusername, dummyPassword, vOfficer.Email)
                    userInfo.IsApproved = True
                    Membership.UpdateUser(userInfo)
                End If
                Return clsOfficer.CreateOfficer(vOfficer, True)
            Else
                Return 0
            End If
        Else
            Return vOfficer.ID
        End If
    End Function

    Function getOfferingTypeID(ByVal OfferingCode As String) As Integer
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vOfferingType = (From p In vContext.offeringtypes Select p).First
        'need to imporve
        Return vOfferingType.ID
    End Function

    Function getBlockID(ByVal BlockCode As String) As Integer
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vBlock = (From p In vContext.academicblocks Select p).First
        'need to imporve
        Return vBlock.ID
    End Function

    Function GetMaxMergedTimeSlots(ByVal TypeField As String, ByVal vPeriods As Integer) As Integer
        Dim isNum As Boolean = False
        Dim vPeriodType As Integer
        Try
            vPeriodType = CType(Trim(LCase(TypeField)), Integer)
            isNum = True
        Catch ex As Exception
            vPeriodType = 0
        End Try
        If Not isNum Then
            Select Case Trim(LCase(TypeField))
                Case "single"
                    vPeriodType = 1
                Case "double"
                    vPeriodType = 2
                Case "treble"
                    vPeriodType = 3
                Case Else
                    vPeriodType = 0
            End Select
        End If
        If vPeriodType = 0 Then
            If vPeriods <= 3 Then
                Return 1
            Else
                Return 2
            End If
        Else
            Return vPeriodType
        End If
    End Function

    Function getResourceYear(ByVal startweek As Integer, ByVal endweek As Integer) As Integer
        Dim vCurWeek = DatePart(DateInterval.WeekOfYear, Now)
        If vCurWeek > endweek Then
            Return Year(Now) + 1
        Else
            Return Year(Now)
        End If
    End Function

    Function CreateResource(ByVal vClassgroupID As Integer, ByVal vName As String, ByVal vResourceTypeID As Integer, ByVal vAmtParticipants As Integer, ByVal vAmtTimeSlots As Integer, ByVal vMaxMergedTimeSlots As Integer) As Integer
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vClassgroup = (From p In vContext.classgroups Where p.ID = vClassgroupID Select p).First
        Dim vResource = New resource With {
          .Name = vName,
          .AmtParticipants = vAmtParticipants,
          .AmtTimeSlots = vAmtTimeSlots,
          .MaxMergedTimeSlots = vMaxMergedTimeSlots,
          .year = getResourceYear(vClassgroup.academicblock.startWeek, vClassgroup.academicblock.endWeek),
          .startWeek = vClassgroup.academicblock.startWeek,
          .endWeek = vClassgroup.academicblock.endWeek,
          .classgrouplinked = True,
          .ResourceTypeID = vResourceTypeID}
        vContext.resources.AddObject(vResource)
        vClassgroup.resources.Add(vResource)
        Try
            vContext.SaveChanges()
        Catch ex As Exception
            Throw New Exception("Resource Error:" + ex.Message)
        End Try
        Return vResource.ID
    End Function


    Function getRoomID(ByVal vRoomNo As String, ByVal vSiteID As Integer) As Integer
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vVenueList = (From p In vContext.venues
                            Where p.Code = vRoomNo And
                                  p.building.SiteID = vSiteID
                                     Select p)
        If vVenueList.Count = 1 Then
            Return vVenueList.First.ID
        Else
            Return 0
        End If
    End Function

    Function getResourceTypeID(ByVal vResourceDescript As String) As Integer
        Dim vContext As timetableEntities = New timetableEntities()
        If Trim(vResourceDescript) = "" Then
            Return 0
        End If
        Dim vResourceType = (From p In vContext.resourcetypes Where p.Description = vResourceDescript Select p).FirstOrDefault
        If IsNothing(vResourceType) Then
            Return 0
        End If
        Return vResourceType.ID
    End Function

    Function getTotalSubGroup(ByVal dt As DataTable, ByVal vSubjectcode As String, ByVal vClassgroup As String, ByVal vSite As String, ByVal vLevel As Integer) As Integer
        Dim vCount As Integer = 0
        For Each dr As DataRow In dt.Rows
            Try
                Dim xClassgroup = CStr(dr("classgroup"))
                Dim xsubjcode = CStr(dr("subjectcode"))
                Dim xSite = CStr(dr("deliverysite"))
                Dim xLevel = clsGeneral.correctSubjectLevel(CStr(dr("level")))

                If vClassgroup = xClassgroup And _
                   vSubjectcode = xsubjcode And _
                   vSite = xSite And _
                   vLevel = xLevel Then
                    vCount = vCount + 1
                End If
            Catch ex As Exception
            End Try
        Next
        Return vCount
    End Function

End Class