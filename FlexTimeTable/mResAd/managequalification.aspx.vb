Public Class managequalification
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            getDepartment1.loadFaculty(User.Identity.Name)
            btnSave.Text = "Save"
            btnDelete.Text = "Delete"
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
            Me.txtOldCode.Text = .OldCode
            changeMode(eMode.edit)
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
                Me.btnDelete.Visible = True
                Me.btnSave.Visible = True
                btnSave.Text = "Update"
                litEdit.Text = "Edit Qualification"
            Case eMode.create
                Me.lblID.Text = ""
                Me.txtCode.Text = ""
                Me.txtShortName.Text = "" '.shortName
                Me.txtLongName.Text = "" '.longName
                Me.txtOldCode.Text = "" '.OldCode
                Me.btnDelete.Visible = False
                Me.btnSave.Visible = True
                btnSave.Text = "Save"
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
            .OldCode = txtOldCode.Text,
            .DepartmentID = getDepartment1.getID}
        vContext.qualifications.AddObject(vQualification)
        vContext.SaveChanges()
    End Sub

    Protected Sub UpdateQualification()
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vQualification As qualification = _
            (From p In vContext.qualifications _
                Where p.ID = CType(lblID.Text, Integer) _
                    Select p).First
        With vQualification
            .longName = Me.txtShortName.Text
            .shortName = Me.txtCode.Text
        End With
        vContext.SaveChanges()
    End Sub

    Protected Sub DeleteQualification()
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vQualification = (From p In vContext.qualifications _
                            Where p.ID = CType(Me.lblID.Text, Integer) _
                            Select p).First
        vContext.DeleteObject(vQualification)
        vContext.SaveChanges()
    End Sub

    Private Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
       
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

    Private Sub btnDelete_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnDelete.Click
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