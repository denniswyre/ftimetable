Public Class managequalification
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            getDepartment1.loadFaculty(User.Identity.Name)
            logSave.Text = "Save"
            logDelete.Text = "Delete"
        End If
    End Sub


    Sub loadQualifications(ByVal DepartmentID As Integer)
        Dim vContext As timetableEntities = New timetableEntities()
        Me.grdQualification.DataSource = (From p In vContext.qualifications Where p.DepartmentID = DepartmentID Select p)
        Me.grdQualification.DataBind()
        mvQual.SetActiveView(vwGrid)
        litMessage.Text = ""
    End Sub

    Private Sub grdQualification_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles grdQualification.SelectedIndexChanged
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vID = CType(grdQualification.SelectedRow.Cells(0).Text, Integer)
        Dim vQual As qualification = _
                   (From p In vContext.qualifications _
                       Where p.ID = vID Select p).First
        With vQual
            Me.lblID.Text = .ID.ToString
            Me.txtCode.Text = .Code
            Me.txtShortName.Text = .shortName
            Me.txtLongName.Text = .longName
            changeMode(eMode.edit)
            With lstOldCodes
                .DataSource = (From p In vContext.oldqualificationcodes Where p.QualID = vID Select p.oldCode, p.ID).ToList
                .DataTextField = "oldCode"
                .DataValueField = "ID"
                .DataBind()
            End With
        End With
        litMessage.Text = ""
    End Sub

    Enum eMode
        edit
        delete
        create
    End Enum

    Sub changeMode(ByVal vMode As eMode)
        mvQual.SetActiveView(vwEdit)
        Select Case vMode
            Case eMode.edit
                Me.logDelete.Visible = True
                Me.logSave.Visible = True
                logSave.Text = "Update"
                litEdit.Text = "Edit Qualification"
            Case eMode.create
                Me.lblID.Text = ""
                Me.txtCode.Text = ""
                Me.txtShortName.Text = "" '.shortName
                Me.txtLongName.Text = "" '.longName
                Me.logDelete.Visible = False
                Me.logSave.Visible = True
                logSave.Text = "Save"
                litEdit.Text = "Create Qualification"
        End Select
    End Sub

    Private Sub lnkCreate_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles lnkCreate.Click
        changeMode(eMode.create)
        litMessage.Text = ""
    End Sub

    Protected Sub btnCancel_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnCancel.Click
        mvQual.SetActiveView(vwGrid)
        litMessage.Text = ""
    End Sub

    Protected Sub CreateQualification()
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vQualification = New qualification With {
            .longName = Me.txtShortName.Text,
            .shortName = Me.txtCode.Text,
            .Code = txtCode.Text,
            .DepartmentID = getDepartment1.getID}
        logSave.Function = "Create Qualification"
        logSave.Description = "New:" + vQualification.Code + ":" + vQualification.longName
        vContext.qualifications.AddObject(vQualification)
        vContext.SaveChanges()
    End Sub

    Protected Sub UpdateQualification()
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vQualification As qualification = _
            (From p In vContext.qualifications _
                Where p.ID = CType(lblID.Text, Integer) _
                    Select p).First
        Dim pQualification = vQualification
        With vQualification
            .longName = Me.txtShortName.Text
            .shortName = Me.txtCode.Text
        End With
        logSave.Function = "Update Qualification"
        logSave.Description = "From:" + pQualification.Code + ":" + pQualification.longName + "-----" + vQualification.Code + ":" + vQualification.longName
        vContext.SaveChanges()
    End Sub

    Protected Sub DeleteQualification()
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vQualification = (From p In vContext.qualifications _
                            Where p.ID = CType(Me.lblID.Text, Integer) _
                            Select p).First
        logDelete.Function = "Delete Qualification"
        logDelete.Description = "Delete:" + vQualification.Code + ":" + vQualification.longName
        vContext.DeleteObject(vQualification)
        vContext.SaveChanges()
    End Sub

    Private Sub logSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles logSave.Click

        Try
            If Me.lblID.Text = "" Then
                CreateQualification()
            Else
                UpdateQualification()
            End If
            loadQualifications((getDepartment1.getID))
            litMessage.Text = clsGeneral.displaymessage("Save", False)
        Catch ex As Exception
            litMessage.Text = clsGeneral.displaymessage(ex.Message, True)
        End Try
    End Sub

    Private Sub logDelete_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles logDelete.Click
        Try
            DeleteQualification()
            loadQualifications(getDepartment1.getID)
            litMessage.Text = clsGeneral.displaymessage("Deleted", False)
        Catch ex As Exception
            litMessage.Text = clsGeneral.displaymessage(ex.Message, True)
        End Try
    End Sub

    Private Sub getDepartment1_DepartmentClick(E As Object, Args As clsDepartmentEvent) Handles getDepartment1.DepartmentClick
        loadQualifications(Args.mDepartmentID)
    End Sub
End Class