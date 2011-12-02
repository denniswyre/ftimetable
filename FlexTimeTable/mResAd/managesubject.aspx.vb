Public Class managesubject
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            getDepartment1.loadFaculty(User.Identity.Name)
            logSave.Text = "Save"
            logDelete.Text = "Delete"
        End If
    End Sub



    Sub loadsubjects(ByVal DepartmentID As Integer)
        Dim vContext As timetableEntities = New timetableEntities()
        Me.grdsubject.DataSource = (From p In vContext.subjects Where p.DepartmentID = DepartmentID Select p)
        Me.grdsubject.DataBind()
        mvSubject.SetActiveView(vwGrid)
        errorMessage.Text = ""
    End Sub

    Private Sub grdsubject_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles grdsubject.SelectedIndexChanged
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vID = CType(grdsubject.SelectedRow.Cells(0).Text, Integer)
        Dim vSubject As subject = _
                   (From p In vContext.subjects _
                       Where p.ID = vID Select p).First
        With vSubject
            Me.lblID.Text = .ID.ToString
            Me.txtCode.Text = .Code
            chkYearBlock.Checked = .yearBlock
            Me.txtShortName.Text = .shortName
            Me.txtLongName.Text = .longName
            changeMode(eMode.edit)
            'list old codes
            With lstOldCodes
                .DataSource = (From p In vContext.oldsubjectcodes Where p.SubjectID = vID Select p.OldCode, p.ID).ToList
                .DataTextField = "oldCode"
                .DataValueField = "ID"
                .DataBind()
            End With
        End With
        errorMessage.Text = ""
    End Sub

    Enum eMode
        edit
        delete
        create
    End Enum

    Sub changeMode(ByVal vMode As eMode)
        mvSubject.SetActiveView(vwEdit)
        Select Case vMode
            Case eMode.edit
                Me.logDelete.Visible = True
                Me.logSave.Visible = True
                logSave.Text = "Update"
                litEdit.Text = "Edit subject"
            Case eMode.create
                Me.lblID.Text = ""
                Me.txtCode.Text = ""
                Me.txtShortName.Text = "" '.shortName
                Me.txtLongName.Text = "" '.longName
                Me.logDelete.Visible = False
                Me.logSave.Visible = True
                logSave.Text = "Save"
                litEdit.Text = "Create subject"
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

    Protected Sub Createsubject()
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vsubject = New subject With {
            .longName = Me.txtShortName.Text,
            .shortName = Me.txtCode.Text,
            .Code = txtCode.Text,
            .yearBlock = chkYearBlock.Checked,
            .Level = CType(Me.cboLevel.SelectedItem.Value, Integer),
            .DepartmentID = getDepartment1.getID}
        logSave.Function = "Create Subject"
        logSave.Description = vsubject.Code + ":" + vsubject.longName
        vContext.subjects.AddObject(vsubject)
        vContext.SaveChanges()
    End Sub

    Protected Sub Updatesubject()
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vsubject As subject = _
            (From p In vContext.subjects _
                Where p.ID = CType(lblID.Text, Integer) _
                    Select p).First
        Dim pSubject = vsubject
        With vsubject
            .longName = Me.txtShortName.Text
            .shortName = Me.txtCode.Text
            .Code = txtCode.Text
            .yearBlock = chkYearBlock.Checked
            .Level = CType(Me.cboLevel.SelectedItem.Value, Integer)
        End With
        logSave.Function = "Update Subject"
        logSave.Description = "Previous:" + pSubject.Code + ":" + pSubject.longName + "  to:" + vsubject.Code + ":" + vsubject.longName
        vContext.SaveChanges()
    End Sub

    Protected Sub Deletesubject()
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vsubject = (From p In vContext.subjects _
                            Where p.ID = CType(Me.lblID.Text, Integer) _
                            Select p).First
        logDelete.Function = "Delete Subject"
        logDelete.Description = "Delete:" + vsubject.Code + ":" + vsubject.longName
        vContext.DeleteObject(vsubject)
        vContext.SaveChanges()
    End Sub

    Private Sub logSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles logSave.Click

        Try
            If Me.lblID.Text = "" Then
                Createsubject()
            Else
                Updatesubject()
            End If
            loadsubjects(getDepartment1.getID)
            errorMessage.Text = ""
        Catch ex As Exception
            errorMessage.Text = clsGeneral.displaymessage(ex.Message, True)
        End Try
    End Sub

    Private Sub logDelete_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles logDelete.Click
        Try
            Deletesubject()
            loadsubjects(getDepartment1.getID)
            errorMessage.Text = ""
        Catch ex As Exception
            errorMessage.Text = clsGeneral.displaymessage(ex.Message, True)
        End Try
    End Sub


    Private Sub getDepartment1_DepartmentClick(E As Object, Args As clsDepartmentEvent) Handles getDepartment1.DepartmentClick
        loadsubjects(Args.mDepartmentID)
    End Sub
End Class