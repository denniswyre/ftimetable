Public Class uploadQual
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            loadFaculty()
            PrepareFile()
        End If
    End Sub


    Sub loadFaculty()
        Dim vContext As timetableEntities = New timetableEntities()
        Dim OfficerID As Integer = clsOfficer.getOfficer(User.Identity.Name).ID
        Me.cboFaculty.DataSource = (From p In vContext.facultyusers _
                                     Where p.OfficerID = OfficerID _
                                       Select p.FacultyName, p.FacultyID)
        Me.cboFaculty.DataTextField = "FacultyName"
        Me.cboFaculty.DataValueField = "FacultyID"
        Me.cboFaculty.DataBind()
        loadSchool()
    End Sub

    Sub loadSchool()
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vFacultyID As Integer = CInt(cboFaculty.SelectedValue)
        With Me.cboSchool
            .DataSource = (From p In vContext.schools _
                                     Where p.facultyID = vFacultyID _
                                       Select p)
            .DataTextField = "LongName"
            .DataValueField = "ID"
            .DataBind()
        End With
        PrepareFile()
        pnlUpload.Enabled = CBool(IIf(cboSchool.SelectedIndex > -1, True, False))
    End Sub

    Private Sub uploadFile1_UploadComplete(ByVal sender As Object, ByVal e As System.EventArgs) Handles uploadFile1.UploadComplete
        processFile(uploadFile1.filetotable.pDatatable)
    End Sub

    Private Sub cboFaculty_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cboFaculty.SelectedIndexChanged
        loadSchool()
    End Sub

    Protected Sub PrepareFile()
        'create valid fields
        uploadFile1.filetotable = New clsfiletotableconversion
        'Dim vfiletotable As New clsfiletotableconversion
        With uploadFile1.filetotable
            .fields = New ListItemCollection()
            ' Add items to the collection.
            .fields.Add(New ListItem("qualificationcode", "0"))
            .fields.Add(New ListItem("qualificationname", "0"))
            .fields.Add(New ListItem("deptcode", "0"))
            .fields.Add(New ListItem("departmentname", "0"))
            .fields.Add(New ListItem("newqualcode", "0"))
        End With
        uploadFile1.header = "Upload Class Groups File"
        uploadFile1.Initialize()
    End Sub



    Protected Sub processFile(ByVal vDataTable As DataTable)
        Dim RowIndex As Integer = 0
        Dim MaxPeriods As Integer = CType(ConfigurationManager.AppSettings("maxperiods"), Integer)
        Dim MaxClassSize As Integer = CType(ConfigurationManager.AppSettings("maxclasssize"), Integer)



        Dim vContext As timetableEntities = New timetableEntities()
        'get school ID
        Dim vSchoolID = CInt(cboSchool.SelectedValue)
        Dim prevError As String = ""
        For Each dr As DataRow In vDataTable.Rows
            Try
                'essential fields
                Dim vNewQualcode = Trim(CType(dr("newqualcode"), String))
                Dim vQualCode = Trim(CType(dr("qualificationcode"), String))
                Dim vQualName = Trim(CType(dr("qualificationname"), String))
                Dim vDeptCode = Trim(CType(dr("deptcode"), String))
                Dim vDeptName = Trim(CType(dr("departmentname"), String))

                'get department
                Dim vDept = (From p In vContext.departments Where p.code = vDeptCode Select p).FirstOrDefault
                If IsNothing(vDept) Then
                    'create new  dept 
                    vDept = New department With {
                        .SchoolID = vSchoolID,
                        .code = vDeptCode,
                        .shortName = vDeptName,
                        .longName = vDeptName}
                    vContext.departments.AddObject(vDept)
                    vContext.SaveChanges()
                End If
                'set qualifications
                Dim vQual = (From p In vContext.qualifications Where p.Code = vQualCode Select p).FirstOrDefault
                If IsNothing(vQual) Then
                    vQual = New qualification With {
                        .DepartmentID = vDept.ID,
                        .Code = vQualCode,
                        .longName = vQualName,
                        .shortName = vQualName}
                    vContext.qualifications.AddObject(vQual)
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