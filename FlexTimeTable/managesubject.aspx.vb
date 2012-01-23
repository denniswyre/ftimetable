Public Class managesubject
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            ucGetDepartment.loadFaculty(User.Identity.Name)
            logSave.Text = "Save"
            logDelete.Text = "Delete"

           lnkCreate.Visible = clsOfficer.isAccessValid(User.Identity.Name, 0)
        End If
    End Sub



    Private Sub DisplaySubject(ByVal vID As Integer)
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vSubject As subject = _
                   (From p In vContext.subjects _
                       Where p.ID = vID Select p).First

        With vSubject
            Me.lblID.Text = .ID.ToString
            Me.txtCode.Text = .Code
            chkYearBlock.Checked = .yearBlock
            Me.txtShortName.Text = .shortName
            Me.txtLongName.Text = .longName

            'list old codes
            lblOldCodes.Text = ""
            For Each x In (From p In vContext.oldsubjectcodes Where p.SubjectID = vID Select p.OldCode, p.ID).ToList
                If lblOldCodes.Text = "" Then
                    lblOldCodes.Text = x.OldCode
                Else
                    lblOldCodes.Text = lblOldCodes.Text + " , " + x.OldCode
                End If
            Next

            Dim DummyFacultyID = CType(ConfigurationManager.AppSettings("dummyfaculty"), Integer)
            Dim vDepartID = .DepartmentID
            Dim vDepart = (From p In vContext.departments Where p.ID = vDepartID Select p).Single
            If vDepart.school.faculty.ID = DummyFacultyID Then
                litOldDepartment.Text = "<b>Linked Department:None</b>" 
            Else
                litOldDepartment.Text = "<b>Linked Department:</b>" + vDepart.longName +
                                                 " <b>School:</b>" + vDepart.school.longName +
                                                " <b>Faculty:</b>" + vDepart.school.faculty.code
            End If
            Dim vFacultyID = .department.school.facultyID
            If vFacultyID = DummyFacultyID Then
                phAccess.Visible = clsOfficer.isAccessValid(User.Identity.Name, 0)
            Else
                phAccess.Visible = clsOfficer.isAccessValid(User.Identity.Name, vFacultyID)
            End If
            logDelete.Visible = clsOfficer.isAccessValid(User.Identity.Name, vFacultyID)
        End With

        changeMode(eMode.view)
        mvSubject.SetActiveView(vwEdit)
        errorMessage.Text = ""
    End Sub

    Enum eMode
        view
        edit
        delete
        create
    End Enum

    Sub changeMode(ByVal vMode As eMode)
        mvSubject.SetActiveView(vwEdit)
        Select Case vMode
            Case eMode.view
                ucGetDepartment.Visible = False
                logSave.Text = "Update"
                btnCancelEdit.Visible = False
                pnlDetail.Enabled = False
                pnlDetail.GroupingText = "Subject Details"
                phID.Visible = True
                phOldCodes.Visible = True
            Case eMode.edit
                ucGetDepartment.Visible = True
                pnlDetail.Enabled = True
                logSave.Text = "Update"
                pnlDetail.GroupingText = "Edit subject"
                phAccess.Visible = False
                btnCancelEdit.Visible = True
                phID.Visible = True
                phOldCodes.Visible = True
            Case eMode.create
                lblOldCodes.Text = ""
                litOldDepartment.Text = ""
                pnlDetail.Enabled = True
                ucGetDepartment.Visible = True
                Me.lblID.Text = ""
                Me.txtCode.Text = ""
                Me.txtShortName.Text = "" '.shortName
                Me.txtLongName.Text = "" '.longName
                Me.logDelete.Visible = False
                Me.logSave.Visible = True
                logSave.Text = "Save"
                pnlDetail.GroupingText = "Create subject"
                phAccess.Visible = False
                btnCancelEdit.Visible = True
                phID.Visible = False
                phOldCodes.Visible = False
        End Select
    End Sub

    Private Sub lnkCreate_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles lnkCreate.Click
        changeMode(eMode.create)
        errorMessage.Text = ""
    End Sub

    Protected Sub btnCancel_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnCancel.Click
        mvSubject.SetActiveView(vwGrid)
        errorMessage.Text = ""
    End Sub

    Protected Function Createsubject() As Integer
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vsubject = New subject With {
            .longName = Me.txtLongName.Text,
            .shortName = Me.txtShortName.Text,
            .Code = txtCode.Text,
            .yearBlock = chkYearBlock.Checked,
            .Level = CType(Me.cboLevel.SelectedItem.Value, Integer),
            .DepartmentID = ucGetDepartment.getID}
        logSave.Function = "Create Subject"
        logSave.Description = vsubject.Code + ":" + vsubject.longName
        vContext.subjects.AddObject(vsubject)
        vContext.SaveChanges()
        Return vsubject.ID
    End Function

    Protected Sub Updatesubject()
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vsubject As subject = _
            (From p In vContext.subjects _
                Where p.ID = CType(lblID.Text, Integer) _
                    Select p).First
        Dim pSubject = vsubject
        With vsubject
            .longName = Me.txtLongName.Text
            .shortName = Me.txtShortName.Text
            .Code = txtCode.Text
            .yearBlock = chkYearBlock.Checked
            .Level = CType(Me.cboLevel.SelectedItem.Value, Integer)
            .DepartmentID = ucGetDepartment.getID
        End With
        logSave.Function = "Update Subject"
        logSave.Description = "Previous:" + pSubject.Code + ":" + pSubject.longName + "  to:" + vsubject.Code + ":" + vsubject.longName
        vContext.SaveChanges()
    End Sub

    Protected Sub Deletesubject()
        Dim vContext As timetableEntities = New timetableEntities()
        Dim DummyFacultyID = CType(ConfigurationManager.AppSettings("dummyfaculty"), Integer)
        Dim DummyDepartID = CType(ConfigurationManager.AppSettings("dummyDepartment"), Integer)

        Dim vSubject = (From p In vContext.subjects _
                            Where p.ID = CType(Me.lblID.Text, Integer) _
                            Select p).Single
        If vSubject.department.school.facultyID = DummyFacultyID Then
            logDelete.Function = "Delete Subject"
            logDelete.Description = "Delete:" + vSubject.Code + ":" + vSubject.longName
            vContext.DeleteObject(vSubject)
            vContext.SaveChanges()
        Else
            vSubject.DepartmentID = DummyDepartID
            logDelete.Function = "Unassigned"
            logDelete.Description = vSubject.Code + "  Dept:" + vSubject.department.code
            vContext.SaveChanges()
        End If
    End Sub

    Private Sub logSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles logSave.Click
        Try
            Dim vID As Integer
            If Me.lblID.Text = "" Then
                vID = Createsubject()
            Else
                vID = CInt(Me.lblID.Text)
                Updatesubject()
            End If
            errorMessage.Text = ""
            DisplaySubject(vID)
        Catch ex As Exception
            errorMessage.Text = clsGeneral.displaymessage(ex.Message, True)
        End Try
    End Sub

    Private Sub logDelete_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles logDelete.Click
        Try
            Deletesubject()
            errorMessage.Text = ""
            mvSubject.SetActiveView(vwGrid)
        Catch ex As Exception
            errorMessage.Text = clsGeneral.displaymessage(ex.Message, True)
        End Try
    End Sub


    Private Sub ucSubjectSearch_SubjectClick(E As Object, Args As clsSubjectEvent) Handles ucSubjectSearch.SubjectClick
        DisplaySubject(Args.mSubjectID)
    End Sub

    Private Sub lnkEdit_Click(sender As Object, e As System.EventArgs) Handles lnkEdit.Click
        changeMode(eMode.edit)
    End Sub

    Private Sub btnCancelEdit_Click(sender As Object, e As System.EventArgs) Handles btnCancelEdit.Click
        If Me.lblID.Text = "" Then
            mvSubject.SetActiveView(vwGrid)
        Else
            DisplaySubject(CInt(Me.lblID.Text))
        End If
        errorMessage.Text = ""
    End Sub
End Class