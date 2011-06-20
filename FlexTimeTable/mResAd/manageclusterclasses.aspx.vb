Public Class manageclusterclasses
    Inherits System.Web.UI.Page
    Private TabHeader() As String = {"Class Groups", "Class", "Class Resources"}
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            loadCluster()
            loadFaculty()
            loadOffering()
            loadTimeSlots()
            litSiteCluster.Text = "Site Cluster:"
            btnSave.Text = "Save"
            btnDelete.Text = "Delete"
            changeMode(eMode.reset)
        End If
    End Sub

    Sub loadCluster()
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vClusters = (From p In vContext.siteclusters Order By p.campu.shortName, p.longName Select p).ToList
        For Each x In vClusters
            cboCluster.Items.Add(New ListItem(x.longName + ", " + x.campu.shortName, CStr(x.ID)))
        Next
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
        Dim vContext As timetableEntities = New timetableEntities

        cboDepartments.Items.Clear()
        If cboFaculty.SelectedIndex >= 0 Then
            Dim FacultyID = CType(cboFaculty.SelectedValue, Integer)
            cboDepartments.DataSource = (From p In vContext.departments _
                                Where p.school.facultyID = FacultyID _
                                Order By p.school.longName, p.longName _
                                  Select longName = (p.longName + "," + p.school.longName), p.ID)
        Else
            cboDepartments.DataSource = Nothing
        End If
        cboDepartments.DataTextField = "longName"
        cboDepartments.DataValueField = "ID"
        cboDepartments.DataBind()
        loadSubjects()
    End Sub

    Sub loadSubjects()
        Dim vContext As timetableEntities = New timetableEntities()
        cboSubject.Items.Clear()
        If cboDepartments.SelectedIndex >= 0 Then
            litDepartment.Text = "Department:"
            Dim DepartmentID = CType(cboDepartments.SelectedValue, Integer)
            cboSubject.DataSource = (From p In vContext.subjects Where p.DepartmentID = DepartmentID _
                                           Select lname = (p.longName + "[" + p.Code + "]"), p.ID).ToList
        Else
            cboSubject.DataSource = Nothing
            litDepartment.Text = "No Department Created for this Faculty:"
        End If
        cboSubject.DataTextField = "lname"
        cboSubject.DataValueField = "ID"
        cboSubject.DataBind()
        displaySubject()
    End Sub

    Sub displaySubject()
        lstQualification.Items.Clear()
        Dim vContext As timetableEntities = New timetableEntities()
        If cboSubject.SelectedIndex >= 0 Then
            Dim SubjectID = CType(cboSubject.SelectedValue, Integer)
            Dim ClusterID = CType(cboCluster.SelectedValue, Integer)
            Dim vSubject = (From p In vContext.subjects Where p.ID = SubjectID _
                                           Select p).First
            litSubject.Text = "Subject:"
            ''determine the qualified qualifications for this subject
            Dim IsQualListed = False
            Dim ProgrammeList = (From p In vContext.programmesubjects Where p.SubjectID = SubjectID Select p.qualification).ToList
            For Each x In ProgrammeList
                Dim vClusterList = (From p In x.siteclusters Where p.ID = ClusterID Select p).ToList
                If vClusterList.Count > 0 Then
                    IsQualListed = True
                    'add qualification to list box
                    lstQualification.Items.Add(New ListItem(x.longName, CStr(x.ID)))
                End If
            Next
            With vSubject
                litSubjectDetails.Text = "<table>" + _
                                         "<tr><td>Code:</td><td><b>" + .Code + "</b></td>" + _
                                         "  <td>Short name:</td><td><b>" + .shortName + "</b></td></tr>" + _
                                         "<tr><td>Long Name:</td><td><b>" + .longName + "</b></td>" + _
                                         "   <td>Level:</td><td><b>" + CStr(.Level) + "</b></td></tr>" + _
                                         "<tr><td>Old Codes:</td><td><b>" + .oldCode + "</b></td>" + _
                                         "   <td>Full Year:</td><td><b>" + CStr(IIf(.yearBlock, "Yes", "No")) + "</b></td></tr>" + _
                                         "</table>"

            End With
            If IsQualListed Then
                lstQualification.Visible = True
                litQual.Text = ""
                loadBlock(vSubject.yearBlock)
            Else
                lstQualification.Visible = False
                litQual.Text = "<p>This Subject is not listed for any qualification for this Site Cluster!!</p>"
            End If
        Else
            litSubject.Text = "No Subject for this Cluster:"
            litSubjectDetails.Text = ""
            lstQualification.Visible = False
            litQual.Text = ""
        End If
        changeMode(eMode.reset)
    End Sub

    Enum eMode
        edit
        reset
        create
        view
    End Enum

    Sub changeMode(ByVal vMode As eMode)
        Select Case vMode
            Case eMode.edit
                Pages.ActiveViewIndex = 2
                btnDelete.Visible = True
                btnSave.Visible = True
                btnSave.Text = "Update"
            Case eMode.create
                Pages.ActiveViewIndex = 2
                lblID.Text = ""
                txtCode.Text = ""
                txtSize.Text = "" '.shortName
                cboTimeSlots.SelectedIndex = 0
                cboOffering.SelectedIndex = 0
                cboBlock.SelectedIndex = 0
                btnDelete.Visible = False
                btnSave.Visible = True
                btnSave.Text = "Save"
            Case eMode.reset
                LoadClasses()
                Pages.ActiveViewIndex = 0
            Case eMode.view
                displayclass()
                Pages.ActiveViewIndex = 1
        End Select
    End Sub

    Private Sub lnkClass_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles lnkClass.Click
        changeMode(eMode.create)
        ClassLecturer1.mClassID = 0
        ClassLecturer1.mSubjectID = CInt(cboSubject.SelectedValue)
        ClassLecturer1.LoadLecturer()
        litMessage.Text = ""
    End Sub

    Private Sub cboFaculty_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cboFaculty.SelectedIndexChanged
        loadDepartments()
    End Sub

    Private Sub cboDepartments_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cboDepartments.SelectedIndexChanged
        loadSubjects()
    End Sub

    Private Sub cboSubject_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cboSubject.SelectedIndexChanged
        displaySubject()
    End Sub

    Protected Sub cboCluster_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles cboCluster.SelectedIndexChanged
        displaySubject()
    End Sub


    Private Sub grdClasses_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles grdClasses.SelectedIndexChanged
        Dim vid = CInt(grdClasses.SelectedDataKey.Values(0))
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vClass As classgroup = _
                   (From p In vContext.classgroups _
                       Where p.ID = vID Select p).First
        With vClass
            lblID.Text = .ID.ToString
            txtCode.Text = .code
            txtSize.Text = .classSize.ToString
            cboBlock.SelectedValue = .AcademicBlockID.ToString
            cboOffering.SelectedValue = .OfferingTypeID.ToString
            cboTimeSlots.SelectedIndex = .TimeSlotTotal - 1
        End With
        changeMode(eMode.view)
        litMessage.Text = ""
    End Sub

    Sub loadUserControls(ByVal vid As Integer)
        'load class resource
        classresource1.ClassID = vID
        classresource1.LoadResources()
        'load Lectuerer
        ClassLecturer1.mClassID = vid
        ClassLecturer1.mSubjectID = CInt(cboSubject.SelectedValue)
        ClassLecturer1.LoadLecturer()
    End Sub

    Sub displayclass()
        Dim tableStart = "<table>"
        Dim tableend = "</table>"
        Dim rowstart = "<tr><td>"
        Dim rowmiddle = "</td><td><b>"
        Dim rowend = "</b></td></tr>"
        litClassDetails.Text = tableStart + _
                                rowstart + "code:" + rowmiddle + txtCode.Text + rowend + _
                                rowstart + "Size:" + rowmiddle + txtSize.Text + rowend + _
                                rowstart + "Block:" + rowmiddle + cboBlock.SelectedItem.Text + rowend + _
                                rowstart + "Offering:" + rowmiddle + cboOffering.SelectedItem.Text + rowend + _
                                rowstart + "TimeSlots:" + rowmiddle + cboTimeSlots.SelectedItem.Text + rowend + _
                                tableend
        loadUserControls(CInt(lblID.Text))
    End Sub

    Sub LoadClasses()
        litMessage.Text = ""
        grdClasses.DataSource = Nothing
        If lstQualification.Visible Then
            lnkClass.Enabled = True
            Dim vContext As timetableEntities = New timetableEntities()
            Dim vSiteClusterID = CType(cboCluster.SelectedItem.Value, Integer)
            Dim vSubjectID = CType(cboDepartments.SelectedItem.Value, Integer)
            Dim vClasses = (From p In vContext.siteclustersubjects
                                Where p.SubjectID = vSubjectID And
                                      p.SiteClusterID = vSiteClusterID
                                      Select p.classgroups).FirstOrDefault
            If Not IsNothing(vClasses) Then
                grdClasses.DataSource = From p In vClasses Select p
            End If
        Else
            lnkClass.Enabled = False
        End If
        grdClasses.DataBind()
    End Sub

    Function displayBlock(ByVal vBLockID As Integer) As String
        Dim vContext As timetableEntities = New timetableEntities()
        Dim Block = (From p In vContext.academicblocks Where p.ID = vBLockID Select p).First
        Return Block.Name
    End Function

    Function displayOffering(ByVal vID As Integer) As String
        Dim vContext As timetableEntities = New timetableEntities()
        Dim Offering = (From p In vContext.offeringtypes Where p.ID = vID Select p).First
        Return Offering.name
    End Function

    Sub loadBlock(ByVal isYear As Boolean)
        Dim vContext As timetableEntities = New timetableEntities()
        With cboBlock
            '.DataSource = (From p In vContext.academicblocks Where p.yearBlock = isYear Select p.Name, p.ID).ToList
            .DataSource = (From p In vContext.academicblocks Select p.Name, p.ID).ToList
            .DataTextField = "Name"
            .DataValueField = "ID"
            .DataBind()
        End With
    End Sub

    Sub loadOffering()
        Dim vContext As timetableEntities = New timetableEntities()
        With cboOffering
            .DataSource = (From p In vContext.offeringtypes Select p.name, p.ID).ToList
            .DataTextField = "Name"
            .DataValueField = "ID"
            .DataBind()
        End With
    End Sub

    Sub loadTimeSlots()
        With cboTimeSlots
            For i = 1 To 12
                .Items.Add(i.ToString)
            Next
        End With
    End Sub

    Protected Function CreateClass() As Integer
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vSiteClusterID = CType(cboCluster.SelectedItem.Value, Integer)
        Dim vSubjectID = CType(cboDepartments.SelectedItem.Value, Integer)
        Dim vSiteSubject = (From p In vContext.siteclustersubjects
                            Where p.SubjectID = vSubjectID And
                                  p.SiteClusterID = vSiteClusterID
                                  Select p).FirstOrDefault
        If IsNothing(vSiteSubject) Then
            'create 
            vSiteSubject = New siteclustersubject With {
                            .SiteClusterID = vSiteClusterID,
                            .SubjectID = vSubjectID}
            vContext.siteclustersubjects.AddObject(vSiteSubject)
            vContext.SaveChanges()
        End If
        Dim vClass = New classgroup With {
            .code = txtCode.Text,
            .classSize = CInt(txtSize.Text),
            .AcademicBlockID = CInt(cboBlock.SelectedValue),
            .OfferingTypeID = CInt(cboOffering.SelectedValue),
            .TimeSlotTotal = cboTimeSlots.SelectedIndex + 1,
            .SiteClusterID = CType(cboCluster.SelectedItem.Value, Integer),
            .SubjectID = CType(cboDepartments.SelectedItem.Value, Integer)}
        vSiteSubject.classgroups.Add(vClass)
        vContext.SaveChanges()
        classresource1.ClassID = vClass.ID
        classresource1.createFirstResource()
        Return vClass.ID
    End Function

    Protected Sub UpdateClass()
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vClass As classgroup = _
            (From p In vContext.classgroups _
                Where p.ID = CType(lblID.Text, Integer) _
                    Select p).First
        With vClass
            .code = txtCode.Text
            .classSize = CInt(txtSize.Text)
            .AcademicBlockID = CInt(cboBlock.SelectedValue)
            .OfferingTypeID = CInt(cboOffering.SelectedValue)
            .TimeSlotTotal = cboTimeSlots.SelectedIndex + 1
            .SiteClusterID = CType(cboCluster.SelectedItem.Value, Integer)
            .SubjectID = CType(cboDepartments.SelectedItem.Value, Integer)
        End With
        vContext.SaveChanges()
    End Sub

    Protected Sub DeleteClass()
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vClass As classgroup = _
             (From p In vContext.classgroups _
                 Where p.ID = CType(lblID.Text, Integer) _
                     Select p).First
        vContext.DeleteObject(vClass)
        vContext.SaveChanges()
    End Sub

    Private Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
        Try
            If lblID.Text = "" Then
                lblID.Text = CStr(CreateClass())
            Else
                UpdateClass()
            End If
            changeMode(eMode.view)
            litMessage.Text = ""
        Catch ex As Exception
            litMessage.Text = ex.Message
        End Try
    End Sub

    Private Sub btnDelete_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnDelete.Click
        Try
            DeleteClass()
            changeMode(eMode.reset)
            litMessage.Text = ""
        Catch ex As Exception
            litMessage.Text = ex.Message
        End Try
    End Sub

    Protected Sub btnCancel_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnCancel.Click
        litMessage.Text = ""
        changeMode(eMode.view)
    End Sub

    Protected Sub btnClassEdit2_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnClassEdit2.Click
        changeMode(eMode.edit)
    End Sub

    Protected Sub btnClassCancel_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnClassCancel.Click
        changeMode(eMode.reset)
    End Sub
End Class