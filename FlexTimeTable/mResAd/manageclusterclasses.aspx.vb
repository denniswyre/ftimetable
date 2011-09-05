Partial Class manageclusterclasses
    Inherits System.Web.UI.Page
    Private TabHeader() As String = {"Class Groups", "Class", "Class Resources"}
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            getDepartment1.loadFaculty(User.Identity.Name)
            loadCluster()
            loadOffering()
            loadTimeSlots()
            litSiteCluster.Text = "Site Cluster:"
            btnSave.Text = "Save"
            btnDelete.Text = "Delete"
            changeMode(eMode.reset)
        End If
    End Sub

    Private Sub manageclusterclasses_PreRender(sender As Object, e As System.EventArgs) Handles Me.PreRender
        getDepartment1.SetLabel(False, 200)
    End Sub

    Sub loadCluster()
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vClusters = (From p In vContext.siteclusters Order By p.campu.shortName, p.longName Select name = (p.longName + ", " + p.campu.shortName), p.ID).ToList
        With cboCluster
            .DataSource = vClusters
            .DataTextField = "name"
            .DataValueField = "ID"
            .DataBind()
        End With
        loadSubjects()
    End Sub


    Sub loadSubjects()
        If cboCluster.SelectedIndex = -1 Or getDepartment1.getID <= 0 Then
            Exit Sub
        End If
        Dim vContext As timetableEntities = New timetableEntities()
        cboSubject.Items.Clear()
        Dim vSiteclusterID = CType(cboCluster.SelectedValue, Integer)
        Dim vDepartmentID = getDepartment1.getID
        Dim vSiteCluster = (From p In vContext.siteclusters Where p.ID = vSiteclusterID Select p).First
        For Each x In (From p In vContext.programmesubjects Where p.subject.DepartmentID = vDepartmentID Select p).ToList
            Dim vQualID = x.QualID
            For Each y In (From o In vContext.qualifications Where o.ID = vQualID Select o).ToList
                For Each z In y.siteclusters
                    If z.ID = vSiteclusterID Then
                        Dim xvalue = x.subject.ID
                        Dim isRedun = False
                        'check for redundancy
                        For Each i As ListItem In cboSubject.Items
                            If xvalue = CInt(i.Value) Then
                                isRedun = True
                            End If
                        Next
                        If Not isRedun Then
                            'insert alphabetically
                            Dim xInserted = False
                            Dim xIndex = 0
                            Dim xtext = x.subject.longName + "[" + x.subject.Code + "]"
                            For Each i As ListItem In cboSubject.Items
                                If i.Text > xtext Then
                                    cboSubject.Items.Insert(xIndex, New ListItem(xtext, CType(x.SubjectID, String)))
                                    xInserted = True
                                    Exit For
                                End If
                                xIndex = xIndex + 1
                            Next
                            If xInserted = False Then
                                cboSubject.Items.Add(New ListItem(xtext, CType(x.SubjectID, String)))
                            End If
                        End If
                    End If
                Next
            Next
        Next
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
                litSubjectDetails.Text = "Code:<b>" + .Code + "</b>&nbsp;&nbsp; Short name:<b>" + .shortName + "</b><br/>" +
                                         "Name:<b>" + .longName + "</b><br/>" +
                                         "Level:<b>" + CStr(.Level) + "</b>&nbsp;&nbsp;Full Year:<b>" + CStr(IIf(.yearBlock, "Yes", "No")) + "</b>"
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
                TabClass.ActiveTabIndex = 1 ' ' .ActiveViewIndex = 2
                tab0.Enabled = False
                tab1.Enabled = True
                Tab2.Enabled = False
                Tab3.Enabled = False
                tab1.HeaderText = "Class Group " + CStr(lblID.Text) + " Details"
                Tab2.HeaderText = "Lecturer For Class Group->" + CStr(lblID.Text)
                Tab3.HeaderText = "Class Group->" + CStr(lblID.Text) + " Resources"
                txtCode.Enabled = True
                cboOffering.Enabled = True
                cboBlock.Enabled = True
                txtSize.Enabled = True
                cboTimeSlots.Enabled = True
                btnReturn.Visible = True
                btnEdit.Visible = False
                btnDelete.Visible = True
                btnSave.Visible = True
                btnSave.Text = "Update"
            Case eMode.create
                TabClass.ActiveTabIndex = 1
                tab0.Enabled = False
                tab1.Enabled = True
                Tab2.Enabled = False
                Tab3.Enabled = False
                tab1.HeaderText = "New Class Group"
                Tab2.HeaderText = "Lecturer"
                Tab3.HeaderText = "Class Group Resources"
                lblID.Text = ""
                txtCode.Text = ""
                txtSize.Text = "" '.shortName
                cboTimeSlots.SelectedIndex = 0
                cboOffering.SelectedIndex = 0
                cboBlock.SelectedIndex = 0
                txtCode.Enabled = True
                cboOffering.Enabled = True
                cboBlock.Enabled = True
                txtSize.Enabled = True
                cboTimeSlots.Enabled = True
                btnReturn.Visible = True
                btnEdit.Visible = False
                btnDelete.Visible = False
                btnSave.Visible = True
                btnSave.Text = "Save"
            Case eMode.reset
                LoadClasses()
                TabClass.ActiveTabIndex = 0
                tab0.Enabled = True
                tab1.Enabled = False
                Tab2.Enabled = False
                Tab3.Enabled = False
                tab1.HeaderText = "Class Group"
                Tab2.HeaderText = "Lecturer"
                Tab3.HeaderText = "Class Group Resources"
            Case eMode.view
                displayclass()
                TabClass.ActiveTabIndex = 1
                tab0.Enabled = True
                tab1.Enabled = True
                Tab2.Enabled = True
                Tab3.Enabled = True
                tab1.HeaderText = "Class Group->" + CStr(lblID.Text) + " Details"
                Tab2.HeaderText = "Lecturer For Class Group->" + CStr(lblID.Text)
                Tab3.HeaderText = "Class Group->" + CStr(lblID.Text) + " Resources"
                txtCode.Enabled = False
                cboOffering.Enabled = False
                cboBlock.Enabled = False
                txtSize.Enabled = False
                cboTimeSlots.Enabled = False
                btnReturn.Visible = False
                btnEdit.Visible = True
                btnDelete.Visible = False
                btnSave.Visible = False
                btnSave.Text = "Save"
        End Select
    End Sub

    Private Sub lnkClass_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles lnkClass.Click
        changeMode(eMode.create)
        ClassLecturer1.mClassID = 0
        ClassLecturer1.mSubjectID = CInt(cboSubject.SelectedValue)
        ClassLecturer1.LoadLecturer()
        litMessage.Text = ""
    End Sub


    Private Sub cboSubject_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cboSubject.SelectedIndexChanged
        displaySubject()
    End Sub

    Protected Sub cboCluster_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles cboCluster.SelectedIndexChanged
        loadSubjects()
    End Sub


    Private Sub grdClasses_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles grdClasses.SelectedIndexChanged
        Dim vid = CInt(grdClasses.SelectedDataKey.Values(0))
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vClass As classgroup = _
                   (From p In vContext.classgroups _
                       Where p.ID = vid Select p).First
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
        classresource1.ClassID = vid
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
        Dim templitClassViewText = tableStart + _
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
            Dim vSubjectID = CType(cboSubject.SelectedItem.Value, Integer)
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
        Dim vSubjectID = CType(cboSubject.SelectedItem.Value, Integer)
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
            .SubjectID = CType(cboSubject.SelectedItem.Value, Integer)}
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
            LoadClasses()
            changeMode(eMode.view)
            litMessage.Text = ""
        Catch ex As Exception
            If IsNothing(ex.InnerException) Then
                litMessage.Text = clsGeneral.displaymessage(ex.Message, True)
            Else
                litMessage.Text = clsGeneral.displaymessage(ex.InnerException.Message, True)
            End If
        End Try
    End Sub

    Private Sub btnDelete_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnDelete.Click
        Try
            DeleteClass()
            LoadClasses()
            changeMode(eMode.reset)
            litMessage.Text = ""
        Catch ex As Exception
            If IsNothing(ex.InnerException) Then
                litMessage.Text = clsGeneral.displaymessage(ex.Message, True)
            Else
                litMessage.Text = clsGeneral.displaymessage(ex.InnerException.Message, True)
            End If
        End Try
    End Sub


    Private Sub btnReturn_Click(sender As Object, e As System.EventArgs) Handles btnReturn.Click
        litMessage.Text = ""
        Try
            changeMode(eMode.view)
        Catch ex As Exception
            changeMode(eMode.reset)
        End Try
    End Sub

    Private Sub btnEdit_Click(sender As Object, e As System.EventArgs) Handles btnEdit.Click
        changeMode(eMode.edit)
    End Sub

    Private Sub getDepartment1_DepartmentClick(E As Object, Args As clsDepartmentEvent) Handles getDepartment1.DepartmentClick
        loadSubjects()
    End Sub

   
End Class