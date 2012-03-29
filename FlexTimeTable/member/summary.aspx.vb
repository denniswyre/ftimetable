Public Class summary
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            cboYear.Items.Add(CStr(Date.Now.Year))
            cboYear.Items.Add(CStr(Date.Now.Year + 1))
            SetBlock()
        End If
    End Sub

    Sub AddReportLine(vline As String)
        litReport.Text = litReport.Text + "<br/>" + vline
    End Sub


    Sub getSubjectSummary()
        Dim vContext As timetableEntities = New timetableEntities()
        Dim DummyFacultyID = CType(ConfigurationManager.AppSettings("dummyfaculty"), Integer)

        'all faculties
        AddReportLine("Faculty: ALL")
        AddReportLine(" Total Qualifications:" + CStr(getQual(0)))
        AddReportLine(" Total Subjects:" + CStr(getSubjects(0)))
        AddReportLine(" Subjects without classes:" + CStr(getSubjwoClasses(0)))
        '''''log 
        getResourceLog()
        'per faculty
        For Each x In (From p In vContext.faculties).ToList
            AddReportLine("===========================================")
            Dim xID = x.ID
            If xID = DummyFacultyID Then
                AddReportLine("Faculty:" + "Unassigned")
            Else
                AddReportLine("Faculty:" + x.code)
            End If

            AddReportLine(" Total Qualifications:" + CStr(getQual(xID)))
            AddReportLine(" Total Subjects:" + CStr(getSubjects(xID)))
            AddReportLine(" Subjects without classes:" + CStr(getSubjwoClasses(xID)))
            For Each y In (From p In vContext.campus).ToList
                Dim yID = y.ID
                AddReportLine("___________________________________________")
                AddReportLine("Campus:" + y.longName + "  (" + x.code + ")")
                getLogCount(y.ID, xID)
            Next
        Next


    End Sub


    Function getSubjects(ByVal vFaculty As Integer) As Integer
        Dim vContext As timetableEntities = New timetableEntities()
        If vFaculty = 0 Then
            Return (From p In vContext.subjects Select p).Count
        Else
            Return (From p In vContext.subjects Where p.department.school.facultyID = vFaculty Select p).Count
        End If
    End Function


    Function getQual(ByVal vFaculty As Integer) As Integer
        Dim vContext As timetableEntities = New timetableEntities()
        If vFaculty = 0 Then
            Return (From p In vContext.qualifications Select p).Count
        Else
            Return (From p In vContext.qualifications Where p.department.school.facultyID = vFaculty Select p).Count
        End If
    End Function

    Function getSubjwoClasses(ByVal vFaculty As Integer) As Integer
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vSubj As List(Of subject)
        If vFaculty = 0 Then
            vSubj = (From p In vContext.subjects Select p).ToList
        Else
            vSubj = (From p In vContext.subjects Where p.department.school.facultyID = vFaculty Select p).ToList
        End If
        Dim iCount = 0
        For Each x In vSubj
            Dim InLoop = False
            For Each y In x.siteclustersubjects
                For Each z In y.classgroups
                    InLoop = True
                    Exit For
                Next
                If InLoop Then
                    Exit For
                End If
            Next
            If Not InLoop Then
                iCount = iCount + 1
            End If
        Next
        Return iCount
    End Function

    Sub getResourceLog()
        Dim vContext As timetableEntities = New timetableEntities()
        AddReportLine(" Lecturers Not Available:" + CStr((From p In vContext.timetablelogs Where p.ReasonID = clsGeneral.eLogType.LecturerNotAvailable Select p).Count))
        AddReportLine(" Room Type Not Available:" + CStr((From p In vContext.timetablelogs Where p.ReasonID = clsGeneral.eLogType.NoResourceType Select p).Count))
        AddReportLine(" No Room Available:" + CStr((From p In vContext.timetablelogs Where p.ReasonID = clsGeneral.eLogType.NoSpaceFoundInCluster Select p).Count))
        AddReportLine(" Rooms too Small:" + CStr((From p In vContext.timetablelogs Where p.ReasonID = clsGeneral.eLogType.SizeNotAvailable Select p).Count))
    End Sub


    Sub getLogCount(ByVal vCampus As Integer, ByVal vFaculty As Integer)
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vClasses = (From p In vContext.classgroups
                            Order By p.siteclustersubject.subject.longName
                                    Where p.siteclustersubject.subject.department.school.facultyID = vFaculty And
                                          p.siteclustersubject.sitecluster.CampusID = vCampus
                                       Select p).ToList

        Dim BlockID = getBlockID()
        Dim vYear = CInt(cboYear.SelectedValue)
        Dim vWeek = (From p In vContext.academicblocks Where p.ID = CInt(cboBlock.SelectedValue) Select p).Single.endWeek
        Dim needed = 0
        Dim assigned = 0
        Dim NoResourceType = 0
        Dim SizeNotAvailable = 0
        Dim NoSpaceFoundInCluster = 0
        Dim LecturerNotAvailable = 0


        For Each x In vClasses
            For Each z In x.resources
                Dim zID = z.ID
                Dim slots = Aggregate p In z.resourceschedules
                                   Where p.Year = vYear And p.Week = vWeek
                                        Into Count()
                assigned = assigned + slots
                needed = needed + z.AmtTimeSlots

                If (From p In vContext.timetablelogs Where p.ReasonID = clsGeneral.eLogType.LecturerNotAvailable And p.ResourceID = zID And p.AcademicBlockID = BlockID Select p).Count > 0 Then
                    LecturerNotAvailable = LecturerNotAvailable + 1
                End If
                If (From p In vContext.timetablelogs Where p.ReasonID = clsGeneral.eLogType.NoResourceType And p.ResourceID = zID And p.AcademicBlockID = BlockID Select p).Count > 0 Then
                    NoResourceType = NoResourceType + 1
                End If
                If (From p In vContext.timetablelogs Where p.ReasonID = clsGeneral.eLogType.NoSpaceFoundInCluster And p.ResourceID = zID And p.AcademicBlockID = BlockID Select p).Count > 0 Then
                    NoSpaceFoundInCluster = NoSpaceFoundInCluster + 1
                End If
                If (From p In vContext.timetablelogs Where p.ReasonID = clsGeneral.eLogType.SizeNotAvailable And p.ResourceID = zID And p.AcademicBlockID = BlockID Select p).Count > 0 Then
                    SizeNotAvailable = SizeNotAvailable + 1
                End If
            Next
        Next
        AddReportLine(" Number of Timeslots Required: " + needed.ToString)
        AddReportLine(" Assigned Timeslots:" + assigned.ToString)
        AddReportLine(" Amount Required:" + Format(needed - assigned, "#,###"))
        AddReportLine(" Percent Allocated:" + Format((assigned * 100) / needed, "#,###.0"))
        AddReportLine(" Lecturers Not Available:" + CStr(LecturerNotAvailable))
        AddReportLine(" Room Type Not Available:" + CStr(NoResourceType))
        AddReportLine(" No Room Available:" + CStr(NoSpaceFoundInCluster))
        AddReportLine(" Rooms too Small:" + CStr(SizeNotAvailable))
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

    Sub SetBlock()
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vBlocks = (From p In vContext.academicblocks
                           Order By p.startWeek, p.endWeek Descending
                              Select p).ToList
        For Each x In vBlocks
            cboBlock.Items.Add(New ListItem(x.Name, CStr(x.ID)))
        Next
    End Sub

    Protected Sub btnReport_Click(sender As Object, e As EventArgs) Handles btnReport.Click
        Try
            errormessage.Text = ""
            litReport.Text = ""
            getSubjectSummary()
        Catch ex As Exception
            errormessage.Text = clsGeneral.displaymessage(ex.Message, True) + "<br/>"
        End Try
    End Sub
End Class