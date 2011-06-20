Public Class manageprogramme
    Inherits System.Web.UI.Page
    Private TabHeader() As String = {"Core Subjects", "Service Subjects", "Site Clusters"}

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            loadFaculty()
            loadAllClusters()
            TabContainer1.ActiveTabIndex = 0
            setTabHeader(0)
        End If
    End Sub


    Sub loadFaculty()
        Dim vContext As timetableEntities = New timetableEntities()
        Dim OfficerID As Integer = clsOfficer.getOfficer(User.Identity.Name).ID
        cboFaculty.DataSource = (From p In vContext.facultyusers _
                                     Where p.OfficerID = OfficerID _
                                       Select p.FacultyName, p.FacultyID)
        cboFaculty.DataTextField = "FacultyName"
        cboFaculty.DataValueField = "FacultyID"
        cboFaculty.DataBind()

        loadDepartments()
    End Sub

    Sub loadDepartments()
        cboDepartment.Items.Clear()
        Dim vContext As timetableEntities = New timetableEntities()
        If cboFaculty.SelectedIndex >= 0 Then
            Dim FacultyID As Integer = CType(cboFaculty.SelectedValue, Integer)
            cboDepartment.DataSource = (From p In vContext.departments Where p.school.facultyID = FacultyID Select longName = (p.longName + "," + p.school.shortName), p.ID)
            litDepartment.Text = "Department:"
        Else
            cboDepartment.DataSource = Nothing
            litDepartment.Text = "No Department Found:"
        End If
        cboDepartment.DataTextField = "longName"
        cboDepartment.DataValueField = "ID"
        cboDepartment.DataBind()

        loadQualification()
       
    End Sub

    Sub loadQualification()
        cboQualification.Items.Clear()
        Dim vContext As timetableEntities = New timetableEntities()
        If cboDepartment.SelectedIndex >= 0 Then
            Dim DepartID As Integer = CType(cboDepartment.SelectedValue, Integer)
            cboQualification.DataSource = (From p In vContext.qualifications Where p.DepartmentID = DepartID Select p.longName, p.ID)
        Else
            cboQualification.DataSource = Nothing
        End If
        cboQualification.DataTextField = "longName"
        cboQualification.DataValueField = "ID"
        cboQualification.DataBind()

       displayQualificationDetails

    End Sub

    Sub setcontrol(ByVal vEnabled As Boolean)
        btnClusterAdd.Enabled = vEnabled
        btnClusterRemove.Enabled = vEnabled
        btnCoreAdd.Enabled = vEnabled
        btnServiceAdd.Enabled = vEnabled
        btnCoreRemove.Enabled = vEnabled
        btnServiceRemove.Enabled = vEnabled
    End Sub

    Sub displayQualificationDetails()
        lstCoreSubjects.Items.Clear()
        lstServiceSubjects.Items.Clear()
        lstSelectedCoreSubjects.Items.Clear()
        lstSelectedServiceSubject.Items.Clear()
        loadCoreSubjects("")
        loadServiceSubjects("")
        If cboQualification.Items.Count > 0 Then
            LoadQualificationSubjects()
            LoadQualificationClusters()
            setcontrol(True)
        Else
            setcontrol(False)
        End If

    End Sub

    Sub LoadQualificationSubjects()
        lstSelectedCoreSubjects.Items.Clear()
        lstSelectedServiceSubject.Items.Clear()
        Dim vContext As timetableEntities = New timetableEntities()
        Dim programmesubjectList = (From p In vContext.programmesubjects
                                      Order By p.subject.longName
                                        Where p.QualID = CInt(cboQualification.SelectedValue) And
                                              p.Level = CInt(cboLevel.SelectedValue)
                                               Select p).ToList
        For Each prog As programmesubject In programmesubjectList
            Dim vItem As New ListItem(prog.subject.longName, CStr(prog.SubjectID))
            If prog.subject.DepartmentID = CInt(cboDepartment.SelectedValue) Then
                'core subject
                lstSelectedCoreSubjects.Items.Add(vItem)
                lstSelectedCoreSubjects.SelectedIndex = -1
            Else
                'service subject
                lstSelectedServiceSubject.Items.Add(vItem)
                lstSelectedServiceSubject.SelectedIndex = -1
            End If
        Next
    End Sub

    Sub LoadQualificationClusters()
        lstSelectedClusters.Items.Clear()
        Dim vContext As timetableEntities = New timetableEntities()
        Dim QualClusters = (From p In vContext.qualifications
                            Where p.ID = CInt(cboQualification.SelectedValue)
                               Select p.siteclusters).FirstOrDefault
        For Each prog As sitecluster In QualClusters
            With prog
                Dim vItem As New ListItem(.longName, CStr(.ID))
                lstSelectedClusters.Items.Add(vItem)
                lstSelectedClusters.SelectedIndex = -1
            End With
        Next
    End Sub

    Sub setTabHeader(ByVal index As Integer)
        TabContainer1.ActiveTabIndex = index
        phLevel.Visible = CBool(IIf(index = 2, False, True))
    End Sub

    Private Sub cboFaculty_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cboFaculty.SelectedIndexChanged
        loadDepartments()
    End Sub


    Private Sub cboDepartment_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cboDepartment.SelectedIndexChanged
        loadQualification()
        loadCoreSubjects("")
    End Sub

    Private Sub cboQualification_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cboQualification.SelectedIndexChanged
       displayQualificationDetails
    End Sub

    Protected Sub cboLevel_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles cboLevel.SelectedIndexChanged
        displayQualificationDetails()
    End Sub


#Region "Subjects"

    Protected Sub SaveSubjects()
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vProgrammeSubject = (From p In vContext.programmesubjects Where p.QualID = CInt(cboQualification.SelectedValue) Select p)
        If vProgrammeSubject.Count > 0 Then
            For Each prog As programmesubject In vProgrammeSubject
                vContext.DeleteObject(prog)
            Next
        End If
        For Each vSubject As ListItem In lstSelectedCoreSubjects.Items
            Dim ProgSubject As New programmesubject With {
                .QualID = CInt(cboQualification.SelectedValue),
                .SubjectID = CInt(vSubject.Value),
                .Level = CInt(cboLevel.SelectedValue)}
            vContext.programmesubjects.AddObject(ProgSubject)
        Next
        For Each vSubject As ListItem In lstSelectedServiceSubject.Items
            Dim ProgSubject As New programmesubject With {
                .QualID = CInt(cboQualification.SelectedValue),
                .SubjectID = CInt(vSubject.Value),
                .Level = CInt(cboLevel.SelectedValue)}
            vContext.programmesubjects.AddObject(ProgSubject)
        Next
        vContext.SaveChanges()
    End Sub

#End Region


#Region "Core Subjects"
    Sub loadCoreSubjects(ByVal vSearch As String)
        lstCoreSubjects.Items.Clear()
        Dim vContext As timetableEntities = New timetableEntities()
        If cboDepartment.SelectedIndex >= 0 Then
            Dim DepartID As Integer = CType(cboDepartment.SelectedValue, Integer)
            Dim QualExist As Boolean = CBool(IIf(cboQualification.SelectedIndex > -1, True, False))
            lstCoreSubjects.DataSource = (From p In vContext.subjects
                                           Order By p.longName
                                             Where QualExist And p.DepartmentID = DepartID And p.longName.Contains(vSearch)
                                                Select p.longName, p.ID)
        Else
            lstCoreSubjects.DataSource = Nothing
        End If
        lstCoreSubjects.DataTextField = "longName"
        lstCoreSubjects.DataValueField = "ID"
        lstCoreSubjects.DataBind()

    End Sub

    Protected Sub btnCoreAdd_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnCoreAdd.Click
        If lstCoreSubjects.SelectedIndex > -1 Then
            lstSelectedCoreSubjects.SelectedIndex = -1
            If lstSelectedCoreSubjects.Items.IndexOf(lstCoreSubjects.SelectedItem) = -1 Then
                lstSelectedCoreSubjects.Items.Add(lstCoreSubjects.SelectedItem)
                SaveSubjects()
            End If
        End If
    End Sub

    Protected Sub btnCoreRemove_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnCoreRemove.Click
        If lstSelectedCoreSubjects.SelectedIndex > -1 Then
            lstSelectedCoreSubjects.Items.RemoveAt(lstSelectedCoreSubjects.SelectedIndex)
            SaveSubjects()
        End If
    End Sub
#End Region

#Region "Service Subjects"


    Sub loadServiceSubjects(ByVal vSearch As String)
        lstServiceSubjects.Items.Clear()
        Dim vContext As timetableEntities = New timetableEntities()
        If cboDepartment.SelectedIndex >= 0 Then
            Dim DepartID As Integer = CType(cboDepartment.SelectedValue, Integer)
            Dim QualExist As Boolean = CBool(IIf(cboQualification.SelectedIndex > -1, True, False))
            lstServiceSubjects.DataSource = (From p In vContext.subjects
                                                Order By p.longName
                                                    Where QualExist And p.DepartmentID <> DepartID And
                                                          p.longName.Contains(vSearch)
                                                        Select p.longName, p.ID)
        Else
            lstServiceSubjects.DataSource = Nothing
        End If
        lstServiceSubjects.DataTextField = "longName"
        lstServiceSubjects.DataValueField = "ID"
        lstServiceSubjects.DataBind()

    End Sub

    Protected Sub btnServiceAdd_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnServiceAdd.Click
        If lstServiceSubjects.SelectedIndex > -1 Then
            lstSelectedServiceSubject.SelectedIndex = -1
            If lstSelectedServiceSubject.Items.IndexOf(lstServiceSubjects.SelectedItem) = -1 Then
                lstSelectedServiceSubject.Items.Add(lstServiceSubjects.SelectedItem)
                SaveSubjects()
            End If
        End If
    End Sub

    Protected Sub btnServiceRemove_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnServiceRemove.Click
        If lstSelectedServiceSubject.SelectedIndex > -1 Then
            lstSelectedServiceSubject.Items.RemoveAt(lstSelectedServiceSubject.SelectedIndex)
            SaveSubjects()
        End If
    End Sub
#End Region

#Region "Cluster Site"
    Sub loadAllClusters()
        lstAllClusters.Items.Clear()
        Dim vContext As timetableEntities = New timetableEntities()
        lstAllClusters.DataSource = (From p In vContext.siteclusters
                                       Order By p.longName
                                           Select p.longName, p.ID)
        lstAllClusters.DataTextField = "longName"
        lstAllClusters.DataValueField = "ID"
        lstAllClusters.DataBind()
    End Sub

    Protected Sub btnClusterAdd_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnClusterAdd.Click
        If lstAllClusters.SelectedIndex > -1 Then
            lstSelectedClusters.SelectedIndex = -1
            If lstSelectedClusters.Items.IndexOf(lstAllClusters.SelectedItem) = -1 Then
                lstSelectedClusters.Items.Add(lstAllClusters.SelectedItem)
                SaveSiteProgramme()
            End If
        End If
    End Sub

    Protected Sub btnClusterRemove_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnClusterRemove.Click
        If lstSelectedClusters.SelectedIndex > -1 Then
            lstSelectedClusters.Items.RemoveAt(lstSelectedClusters.SelectedIndex)
            SaveSiteProgramme()
        End If
    End Sub



    Protected Sub SaveSiteProgramme()
        Try
            Dim vContext As timetableEntities = New timetableEntities()
            Dim vQual = (From p In vContext.qualifications Where p.ID = CInt(cboQualification.SelectedValue) Select p).First
            ' delete all those that are not selected
            Dim ClusterDelArr As New ArrayList
            If vQual.siteclusters.Count > 0 Then
                'mark those that need to be deleted
                For Each vCluster As sitecluster In vQual.siteclusters
                    Dim ClusterFound As Boolean = False
                    For Each vSelCluster As ListItem In lstSelectedClusters.Items
                        If vCluster.ID = CInt(vSelCluster.Value) Then
                            ClusterFound = True
                            Exit For
                        End If
                    Next
                    If Not ClusterFound Then
                        ClusterDelArr.Add(vCluster.ID)
                    End If
                Next
                'delete those that are not in the selected list
                For Each vclu As Integer In ClusterDelArr
                    Dim ClusterID As Integer = vclu
                    Dim cluster As sitecluster = (From p In vQual.siteclusters Where p.ID = ClusterID Select p).First
                    vQual.siteclusters.Remove(cluster)
                Next
            End If
            For Each vCluster As ListItem In lstSelectedClusters.Items
                'check if cluster exists 
                Dim vID As Integer = CInt(vCluster.Value)
                Dim clusterSearch = From p In vQual.siteclusters Where p.ID = vID Select p
                If clusterSearch.Count = 0 Then
                    Dim vNewCluster As sitecluster = (From p In vContext.siteclusters
                                                        Where p.ID = vID
                                                            Select p).First
                    vQual.siteclusters.Add(vNewCluster)
                End If
            Next
            vContext.SaveChanges()
            lblMessage.Text = "Updated!!"
        Catch ex As Exception
            lblMessage.Text = ex.Message
        End Try
    End Sub

#End Region

    Protected Sub btnCoreSearch_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnCoreSearch.Click
        loadCoreSubjects(txtCoreSearch.Text)
    End Sub

    Private Sub btnServiceSearch_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnServiceSearch.Click
        loadServiceSubjects(txtServiceSearch.Text)
    End Sub
End Class