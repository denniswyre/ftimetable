Public Class uploadSubject
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            PrepareFile()
        End If
    End Sub



    Private Sub uploadFile1_UploadComplete(ByVal sender As Object, ByVal e As System.EventArgs) Handles uploadFile1.UploadComplete
        processFile(uploadFile1.filetotable.pDatatable)
    End Sub

    Protected Sub PrepareFile()
        'create valid fields
        uploadFile1.filetotable = New clsfiletotableconversion
        'Dim vfiletotable As New clsfiletotableconversion
        With uploadFile1.filetotable
            .fields = New ListItemCollection()
            ' Add items to the collection.
            .fields.Add(New ListItem("subjectcode", "0"))
            .fields.Add(New ListItem("subjectname", "0"))
            .fields.Add(New ListItem("blockcode", "0"))
            .fields.Add(New ListItem("level", "0"))
            ' .fields.Add(New ListItem("newsubjectcode", "0"))
        End With
        uploadFile1.header = "Upload Subject File"
        uploadFile1.Initialize()
    End Sub



    Protected Sub processFile(ByVal vDataTable As DataTable)
        Dim RowIndex As Integer = 0
        Dim MaxPeriods As Integer = CType(ConfigurationManager.AppSettings("maxperiods"), Integer)
        Dim MaxClassSize As Integer = CType(ConfigurationManager.AppSettings("maxclasssize"), Integer)

        Dim vContext As timetableEntities = New timetableEntities()
        Dim DummyFacultyID = 999
        'create dummy faculty, school and dept for subjects
        Dim vFaculty = (From p In vContext.faculties Where p.ID = DummyFacultyID Select p).FirstOrDefault
        If IsNothing(vFaculty) Then
            vFaculty = New faculty With {
                .ID = DummyFacultyID,
                .code = "dummy",
                .longName = "dummy faculty",
                .shortName = "dummy faculty"}
            vContext.faculties.AddObject(vFaculty)
            vContext.SaveChanges()
        End If
        Dim vDummySchool = (From p In vContext.schools Where p.facultyID = DummyFacultyID Select p).FirstOrDefault
        If IsNothing(vDummySchool) Then
            vDummySchool = New school With {
                .facultyID = DummyFacultyID,
                .code = "dummy001",
                .longName = "Dummy school",
                .shortName = "dum sch"}
            vContext.schools.AddObject(vDummySchool)
            vContext.SaveChanges()
        End If

        Dim vDummyDept = (From p In vContext.departments Where p.SchoolID = vDummySchool.id Select p).FirstOrDefault
        If IsNothing(vDummyDept) Then
            vDummyDept = New department With {
                .SchoolID = vDummySchool.id,
                .code = "dummy001",
                .longName = "Dummy Department",
                .shortName = "dumDept"}
            vContext.departments.AddObject(vDummyDept)
            vContext.SaveChanges()
        End If

        Dim prevError As String = ""
        For Each dr As DataRow In vDataTable.Rows
            'essential fields
            Dim vsubjectcode As String ' correctsubjcode(CType(dr("subjectcode"), String))
            Dim vsubjectName As String ' Trim(CType(dr("subjectname"), String))
            Dim vblockCode As Boolean '= CBool(IIf(CDbl(Trim(CType(dr("blockcode"), String))) = 0, True, False))
            Dim vlevel As Integer ' correctlevel(CType(dr("level"), String))
            Try
                vsubjectcode = clsGeneral.correctCode(CType(dr("subjectcode"), String))
                vsubjectName = Trim(CType(dr("subjectname"), String))
                vblockCode = CBool(IIf(CDbl(Trim(CType(dr("blockcode"), String))) = 0, True, False))
                vlevel = clsGeneral.correctSubjectLevel(CType(dr("level"), String))

                'set subject
                Dim vSubj = (From p In vContext.subjects Where p.Code = vsubjectcode Select p).FirstOrDefault
                If IsNothing(vSubj) Then
                    vSubj = New subject With {
                        .DepartmentID = vDummyDept.ID,
                        .Code = vsubjectcode,
                        .longName = Left(vsubjectName, 250),
                        .shortName = vsubjectcode,
                        .Level = vlevel,
                        .yearBlock = vblockCode}
                    vContext.subjects.AddObject(vSubj)
                    vContext.SaveChanges()
                End If
            Catch ex As Exception
                'do not show the last error as it is necessary.
                If RowIndex > 0 Then
                    uploadFile1.errorlist = New ListItem(prevError)
                End If
                prevError = "Row No:" + CStr(RowIndex) + "-->err:" + ex.Message
            End Try
            RowIndex += 1
        Next
    End Sub

End Class