Partial Class uploadClasses
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then

        End If
    End Sub

   

    Protected Sub btnProcess_Click(sender As Object, e As EventArgs) Handles btnProcess.Click
        processFile()
    End Sub

    Protected Sub processFile()
        Dim RowIndex As Integer = 0
        Dim MaxPeriods As Integer = CType(ConfigurationManager.AppSettings("maxperiods"), Integer)
        Dim MaxClassSize As Integer = CType(ConfigurationManager.AppSettings("maxclasssize"), Integer)

        Dim vContext As timetableEntities = New timetableEntities()
        'get data table from user control
        Dim defaultlecturetype = 8
        Dim prevError As String = ""
        For Each x In (From p In vContext.transaction_upload Select p).ToList
            Dim fields As New sTablefields
            With fields
                .schoolcode = x.Schoolcode
                .schoolname = x.SchoolName
                .deptcode = x.DeptCode
                .deptname = x.DeptName
                .OLDQualcode = x.oldQualCode
                .OLDSubjectcode = clsGeneral.correctCode(x.oldSubjectCode)
                .offeringString = x.Offering
                .ClassCode = clsGeneral.correctCode(x.ClassGroup)
                .Classsize = CType(x.classsize, Integer)
                .cluster = x.clusterCode
                .lname = x.lname
                .lsurname = x.lsurname
                .Timeslots = 4
                .RowIndex = RowIndex
            End With
            ' non essential fields
            processRow(fields)
            RowIndex += 1
        Next
    End Sub




    Structure sTablefields
        Dim schoolcode As String '= Trim(CType(dr("schoolcode"), String))
        Dim schoolname As String '=' Trim(CType(dr("schoolname"), String))
        Dim deptcode As String '=' Trim(CType(dr("deptcode"), String))
        Dim deptname As String '=' Trim(CType(dr("deptname"), String))
        Dim OLDQualcode As String '= Trim(CType(dr("oldqualcode"), String))
        Dim OLDSubjectcode As String '= clsGeneral.correctCode(CType(dr("oldsubjectcode"), String))
        Dim offeringString As String '= Trim(CType(dr("offering"), String))
        Dim ClassCode As String '= clsGeneral.correctCode(CType(dr("classgroup"), String))
        Dim Classsize As Integer '= CType(dr("classsize"), Integer)
        Dim cluster As String '= Trim(CType(dr("clustercode"), String))
        Dim lname As String '= Trim(CType(dr("lname"), String))
        Dim lsurname As String '= Trim(CType(dr("lsurname"), String))
        Dim Timeslots As Integer
        Dim RowIndex As Integer
    End Structure

    Protected Sub processRow(ByVal fields As sTablefields)
        'essential fields
        With fields
            Try
                Dim vContext As timetableEntities = New timetableEntities()

                'get data table from user control
                Dim defaultlecturetype = 8
                Dim vsitecluster As sitecluster = Nothing
                Dim vclusternum = CInt(.cluster)
                Dim vSiteClusterlist = (From p In vContext.siteclusters Select p).ToList
                For Each x In vSiteClusterlist
                    Dim testnum = CInt(x.code)
                    If vclusternum = testnum Then
                        vsitecluster = x
                        Exit For
                    End If
                Next

                If IsNothing(vsitecluster) Then
                    Throw New OverflowException("cluster " + .cluster + " does not exist")
                End If

                'qualification
                Dim vQual As qualification = (From p In vContext.oldqualificationcodes Where p.oldCode.Contains(.OLDQualcode) Select p.qualification).FirstOrDefault
                If IsNothing(vQual) Then
                    Throw New Exception("Qualification " + .OLDQualcode + " not found ")
                End If
                vQual = (From p In vContext.oldqualificationcodes Where p.oldCode.Contains(.OLDQualcode) Select p.qualification).First

                'subject
                Dim vSubject As subject = (From p In vContext.oldsubjectcodes Where p.OldCode.Contains(.OLDSubjectcode) Select p.subject).FirstOrDefault
                If IsNothing(vSubject) Then
                    Throw New Exception("Subject Error:" + .OLDSubjectcode + " not found")
                End If

                ''''adjust department
                Dim correctdepartment = (From p In vContext.departments Where p.code = .deptcode Select p).FirstOrDefault
                If Not IsNothing(correctdepartment) AndAlso vSubject.DepartmentID <> correctdepartment.ID Then
                    vSubject.DepartmentID = correctdepartment.ID
                    vContext.SaveChanges()
                End If

                ''''adjust school
                Dim vfaculty As faculty = Nothing
                Dim vschool = (From p In vContext.schools Where p.code = .schoolcode Select p).FirstOrDefault
                If IsNothing(vschool) Then
                    Dim facultyid = getfacultyid(.schoolname)
                    vfaculty = (From p In vContext.faculties Where p.ID = facultyid Select p).SingleOrDefault
                    If Not IsNothing(vfaculty) Then
                        ''create new school
                        vschool = New school With {
                            .code = fields.schoolcode,
                            .facultyID = facultyid,
                            .longName = fields.schoolname,
                            .shortName = fields.schoolcode}
                        vContext.schools.AddObject(vschool)
                        vContext.SaveChanges()
                    End If
                Else
                    vfaculty = vschool.faculty
                End If
                If Not IsNothing(vschool) Then
                    If correctdepartment.SchoolID <> vschool.id Then
                        'adjust department school 
                        correctdepartment.SchoolID = vschool.id
                        vContext.SaveChanges()
                    End If
                End If

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
                If Not vsitecluster.qualifications.Contains(vQual) Then
                    vsitecluster.qualifications.Add(vQual)
                    vContext.SaveChanges()
                End If
                'set siteclustersubject
                Dim vClusterSubject = (From p In vContext.siteclustersubjects
                                       Where p.SiteClusterID = vsitecluster.ID And
                                       p.SubjectID = vSubject.ID Select p).FirstOrDefault
                If IsNothing(vClusterSubject) Then
                    vClusterSubject = New siteclustersubject With {
                        .SiteClusterID = vsitecluster.ID,
                        .SubjectID = vSubject.ID}
                    vContext.siteclustersubjects.AddObject(vClusterSubject)
                    vContext.SaveChanges()
                End If

                'get offering type
                Dim vOfferingTypeID As Integer = getOfferingTypeID(.offeringString)  'need to look at this
                Dim vblockid = getBlockID("")
                'class group
                Dim vClassgrp = (From p In vContext.classgroups
                                 Where p.code = .ClassCode And
                                       p.SiteClusterID = vsitecluster.ID And
                                       p.SubjectID = vSubject.ID Select p).FirstOrDefault
                If IsNothing(vClassgrp) Then
                    vClassgrp = New classgroup With {
                        .code = fields.ClassCode,
                        .SiteClusterID = vsitecluster.ID,
                        .SubjectID = vSubject.ID,
                        .AcademicBlockID = vblockid,
                        .classSize = fields.Classsize,
                        .OfferingTypeID = vOfferingTypeID,
                        .TimeSlotTotal = CInt(fields.Timeslots)}
                    vContext.classgroups.AddObject(vClassgrp)
                    vContext.SaveChanges()
                End If

                ' set lecturer
                Dim vlusername = clsGeneral.createusername(.lname, .lsurname)
                Dim vLecturerID = getLecturer(vlusername)
                If vLecturerID = 0 Then
                    lstError.Items.Add(New ListItem("Row No:" + CStr(.RowIndex + 1) + "-->err:" + "Lecturer does not exist"))
                Else
                    Dim vLecturer = (From p In vContext.lecturers Where p.LecturerID = vLecturerID Select p).FirstOrDefault
                    Try
                        If IsNothing(vLecturer) Then
                            vLecturer = New lecturer With {
                                       .LecturerID = vLecturerID,
                                       .DepartmentID = vSubject.DepartmentID}
                            vContext.lecturers.AddObject(vLecturer)
                        End If
                        Dim dosave As Boolean = False
                        'check if lecturer has subject
                        If (From p In vLecturer.subjects Where p.ID = vSubject.ID Select p).Count = 0 Then
                            dosave = True
                            vLecturer.subjects.Add(vSubject)
                        End If
                        If (From p In vLecturer.classgroups Where p.ID = vClassgrp.ID Select p).Count = 0 Then
                            dosave = True
                            vLecturer.classgroups.Add(vClassgrp)
                        End If
                        If dosave = True Then
                            vContext.SaveChanges()
                        End If
                    Catch ex As Exception
                        lstError.Items.Add(New ListItem("Row No:" + CStr(.RowIndex + 1) + "-->err:Lecturer Error:" + ex.Message))
                    End Try
                End If
                ' resources
                Dim vResourceCnt = getGetResourceCount(vClassgrp.ID)
                Dim vResourceType = (From p In vContext.resourcetypes Where p.ID = defaultlecturetype Select p).First
                Dim vMaxMergedTimeSlots = GetMaxMergedTimeSlots("0", CInt(.Timeslots))
                Dim vResourceName = vSubject.Code + "[" + vClassgrp.code + "]"
                CreateResource(vClassgrp.ID, vResourceName, vResourceType.ID, CInt(.Classsize), CInt(.Timeslots), vMaxMergedTimeSlots)
            Catch ex As OverflowException
                lstError.Items.Add(New ListItem("Row No:" + CStr(.RowIndex + 1) + "-->err:" + ex.Message))
            Catch ex As Exception
                lstError.Items.Add(New ListItem("Row No:" + CStr(.RowIndex + 1) + "-->err:" + ex.Message))
            End Try
        End With
    End Sub

    Function getfacultyid(ByVal searchstring As String) As Integer
        Dim vContext As timetableEntities = New timetableEntities()
        If InStr(UCase(searchstring), "FACULTY OF EDUCATION") > 0 Then
            Return 1
        End If
        If InStr(UCase(searchstring), "FACULTY OF HEALTH SCIENCES") > 0 Then
            Return 2
        End If
        If InStr(UCase(searchstring), "BUS- MAN SCIENCES") > 0 Then
            Return 3
        End If
        If InStr(UCase(searchstring), "INFO & COMMUNIC TECHNOLOGY") > 0 Then
            Return 4
        End If
        If InStr(UCase(searchstring), "FACULTY OF ENG AND SCIENCE") > 0 Then
            Return 4
        End If
        Return 0
    End Function

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

    Function getOfferingTypeID(ByVal OfferingString As String) As Integer
        If InStr(LCase(OfferingString), "part") > 0 Then
            Return 2
        Else
            Return 1
        End If
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
        Dim vResource = (From p In vClassgroup.resources Where p.ResourceTypeID = vResourceTypeID Select p).FirstOrDefault
        If IsNothing(vResource) Then
            vResource = New resource With {
              .Name = vName,
              .AmtParticipants = vAmtParticipants,
              .AmtTimeSlots = vAmtTimeSlots,
              .MaxMergedTimeSlots = vMaxMergedTimeSlots,
              .year = getResourceYear(vClassgroup.academicblock.startWeek, vClassgroup.academicblock.endWeek),
              .startWeek = vClassgroup.academicblock.startWeek,
              .endWeek = vClassgroup.academicblock.endWeek,
              .classgrouplinked = True,
              .TimeslotsArrangement = "",
              .ResourceTypeID = vResourceTypeID}
            vContext.resources.AddObject(vResource)
            vClassgroup.resources.Add(vResource)
            Try
                vContext.SaveChanges()
            Catch ex As Exception
                Throw New Exception("Resource Error:" + ex.Message)
            End Try
        End If
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