Public Class managequalification
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            ucGetDepartment.loadFaculty(User.Identity.Name)
            logSave.Text = "Save"
            logDelete.Text = "Delete"
            lnkCreate.Visible = clsOfficer.isAccessValid(User.Identity.Name, 0)
        End If
    End Sub



    Private Sub DisplayQualification(ByVal vID As Integer)
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vQualification As qualification = _
                   (From p In vContext.qualifications _
                       Where p.ID = vID Select p).First

        With vQualification
            Me.lblID.Text = .ID.ToString
            Me.txtCode.Text = .Code
            Me.txtShortName.Text = .shortName
            Me.txtLongName.Text = .longName
            changeMode(eMode.edit)

            lblOldCodes.Text = ""
            For Each x In (From p In vContext.oldqualificationcodes Where p.QualID = vID Select p.oldCode, p.ID).ToList
                If lblOldCodes.Text = "" Then
                    lblOldCodes.Text = x.oldCode
                Else
                    lblOldCodes.Text = lblOldCodes.Text + " , " + x.oldCode
                End If
            Next

            Dim DummyFacultyID = CType(ConfigurationManager.AppSettings("dummyFaculty"), Integer)
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
            Dim FacAccess = clsOfficer.isAccessValid(User.Identity.Name, vFacultyID)
            If vFacultyID = DummyFacultyID Then
                phAccess.Visible = clsOfficer.isAccessValid(User.Identity.Name, 0)
            Else
                phAccess.Visible = FacAccess
            End If
            logDelete.Visible = FacAccess
            ucQualDetail.setQualification(vID, FacAccess)
        End With

        changeMode(eMode.view)
        litMessage.Text = ""
    End Sub

    Enum eMode
        view
        edit
        delete
        create
    End Enum

    Sub changeMode(ByVal vMode As eMode)
        mvQual.SetActiveView(vwEdit)
        Select Case vMode
            Case eMode.view
                ucGetDepartment.Visible = False
                logSave.Text = "Update"
                pnlControl.Visible = False
                pnlDetail.Enabled = False
                pnlDetail.GroupingText = "Qualification Details"
                phID.Visible = True
                phOldCodes.Visible = True
                ucQualDetail.Visible = True
                ucGetDepartment.setUpdate(False)
            Case eMode.edit
                ucGetDepartment.Visible = True
                pnlDetail.Enabled = True
                logSave.Text = "Update"
                pnlDetail.GroupingText = "Edit Qualification"
                phAccess.Visible = False
                pnlControl.Visible = True
                phID.Visible = True
                phOldCodes.Visible = True
                ucQualDetail.Visible = False
                ucGetDepartment.setUpdate(True)
                btnCancelEdit.Visible = True
            Case eMode.create
                lblOldCodes.Text = ""
                litOldDepartment.Text = ""
                pnlDetail.Enabled = True
                ucGetDepartment.Visible = True
                Me.lblID.Text = ""
                Me.txtCode.Text = ""
                Me.txtShortName.Text = "" '.shortName
                Me.txtLongName.Text = "" '.longName
                logSave.Text = "Save"
                Me.logDelete.Visible = False
                Me.logSave.Visible = True
                pnlDetail.GroupingText = "Create Qualification"
                phAccess.Visible = False
                pnlControl.Visible = True
                phID.Visible = False
                phOldCodes.Visible = False
                ucQualDetail.Visible = False
                ucGetDepartment.setUpdate(False)
                btnCancelEdit.Visible = False
        End Select
    End Sub

    Private Sub lnkCreate_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles lnkCreate.Click
        changeMode(eMode.create)
        litMessage.Text = ""
    End Sub

    Protected Sub btnCancel_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnCancel.Click
        ucQualificationSearch.getQualifications()
        mvQual.SetActiveView(vwGrid)
        litMessage.Text = ""
    End Sub

    Protected Function CreateQualification() As Integer
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vQualification = New qualification With {
            .longName = Me.txtLongName.Text,
            .shortName = Me.txtShortName.Text,
            .Code = txtCode.Text,
            .DepartmentID = ucGetDepartment.getID}
        logSave.Function = "Create Qualification"
        logSave.Description = "New:" + vQualification.Code + ":" + vQualification.longName
        vContext.qualifications.AddObject(vQualification)
        vContext.SaveChanges()
        Return vQualification.ID
    End Function

    Protected Sub UpdateQualification()
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vQualification As qualification = _
            (From p In vContext.qualifications _
                Where p.ID = CType(lblID.Text, Integer) _
                    Select p).First
        Dim pQualification = vQualification
        With vQualification
            .longName = Me.txtLongName.Text
            .shortName = Me.txtShortName.Text
            .Code = txtCode.Text
        End With
        logSave.Function = "Update Qualification"
        logSave.Description = "From:" + pQualification.Code + ":" + pQualification.longName + "-----" + vQualification.Code + ":" + vQualification.longName
        vContext.SaveChanges()
    End Sub

    Protected Sub DeleteQualification()
        Dim vContext As timetableEntities = New timetableEntities()
        Dim DummyFacultyID = CType(ConfigurationManager.AppSettings("dummyfaculty"), Integer)
        Dim DummyDepartID = CType(ConfigurationManager.AppSettings("dummyDepartment"), Integer)


        Dim vQualification = (From p In vContext.qualifications _
                            Where p.ID = CType(Me.lblID.Text, Integer) _
                            Select p).First
        If vQualification.department.school.facultyID = DummyFacultyID Then
            logDelete.Function = "Delete Qualification"
            logDelete.Description = "Delete:" + vQualification.Code + ":" + vQualification.longName
            vContext.DeleteObject(vQualification)
            vContext.SaveChanges()
        Else
            vQualification.DepartmentID = DummyDepartID
            vContext.SaveChanges()
            logDelete.Function = "Unassign"
            logDelete.Description = vQualification.Code + "---Dept:" + vQualification.department.code
        End If
    End Sub

    Private Sub ucGetDepartment_UpdateDepartment(e As Object, Args As System.EventArgs) Handles ucGetDepartment.UpdateDepartment
        Try
            Dim vContext As timetableEntities = New timetableEntities()
            Dim vQualification As qualification = _
                (From p In vContext.qualifications _
                    Where p.ID = CType(lblID.Text, Integer) _
                        Select p).First
            Dim pQualification = vQualification
            With vQualification
                .DepartmentID = ucGetDepartment.getID
            End With
            logSave.Function = "Update Qualification"
            logSave.Description = "From:" + CStr(pQualification.ID) + ":" + CStr(pQualification.DepartmentID) + "-----" + ":" + CStr(ucGetDepartment.getID)
            vContext.SaveChanges()
            DisplayQualification(vQualification.ID)
            litMessage.Text = ""
        Catch ex As Exception
            litMessage.Text = clsGeneral.displaymessage(ex.Message, True)
        End Try
    End Sub


    Private Sub logSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles logSave.Click
        Try
            Dim vID As Integer
            If Me.lblID.Text = "" Then
                vID = CreateQualification()
            Else
                vID = CInt(Me.lblID.Text)
                UpdateQualification()
            End If
            DisplayQualification(vID)
            litMessage.Text = ""
        Catch ex As Exception
            litMessage.Text = clsGeneral.displaymessage(ex.Message, True)
        End Try
    End Sub

    Private Sub logDelete_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles logDelete.Click
        Try
            DeleteQualification()
            ucQualificationSearch.getQualifications()
            mvQual.SetActiveView(vwGrid)
            litMessage.Text = ""
        Catch ex As Exception
            litMessage.Text = clsGeneral.displaymessage(ex.Message, True)
        End Try
    End Sub

    Private Sub ucQualificationSearch_QualClick(E As Object, Args As clsQualEvent) Handles ucQualificationSearch.QualClick
        DisplayQualification(Args.mQualID)
        mvQual.SetActiveView(vwEdit)
    End Sub

    Private Sub lnkEdit_Click(sender As Object, e As System.EventArgs) Handles lnkEdit.Click
        changeMode(eMode.edit)
    End Sub

    Private Sub btnCancelEdit_Click(sender As Object, e As System.EventArgs) Handles btnCancelEdit.Click
        DisplayQualification(CInt(Me.lblID.Text))
        litMessage.Text = ""
    End Sub
   
   
End Class