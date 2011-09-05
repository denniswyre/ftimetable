Public Class managesubject
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            getDepartment1.loadFaculty(User.Identity.Name)
            btnSave.Text = "Save"
            btnDelete.Text = "Delete"
        End If
    End Sub

    

    Sub loadsubjects(ByVal DepartmentID As Integer)
        Dim vContext As timetableEntities = New timetableEntities()
        Me.grdsubject.DataSource = (From p In vContext.subjects Where p.DepartmentID = DepartmentID Select p)
        Me.grdsubject.DataBind()
        mvSubject.SetActiveView(vwGrid)
        lblMessage.Text = ""
    End Sub

    Private Sub grdsubject_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles grdsubject.SelectedIndexChanged
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vID = CType(grdsubject.SelectedRow.Cells(0).Text, Integer)
        Dim vQual As subject = _
                   (From p In vContext.subjects _
                       Where p.ID = vID Select p).First
        With vQual
            Me.lblID.Text = .ID.ToString
            Me.txtCode.Text = .Code
            chkYearBlock.Checked = .yearBlock
            Me.txtShortName.Text = .shortName
            Me.txtLongName.Text = .longName
            Me.txtOldCode.Text = .OldCode
            changeMode(eMode.edit)
        End With
        lblMessage.Text = ""
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
                Me.btnDelete.Visible = True
                Me.btnSave.Visible = True
                btnSave.Text = "Update"
                litEdit.Text = "Edit subject"
            Case eMode.create
                Me.lblID.Text = ""
                Me.txtCode.Text = ""
                Me.txtShortName.Text = "" '.shortName
                Me.txtLongName.Text = "" '.longName
                Me.txtOldCode.Text = "" '.OldCode
                Me.btnDelete.Visible = False
                Me.btnSave.Visible = True
                btnSave.Text = "Save"
                litEdit.Text = "Create subject"
        End Select
    End Sub

    Private Sub lnkCreate_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles lnkCreate.Click
        changeMode(eMode.create)
        lblMessage.Text = ""
    End Sub

    Protected Sub btnCancel_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnCancel.Click
        mvSubject.SetActiveView(vwGrid)
        lblMessage.Text = ""
    End Sub

    Protected Sub Createsubject()
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vsubject = New subject With {
            .longName = Me.txtShortName.Text,
            .shortName = Me.txtCode.Text,
            .Code = txtCode.Text,
            .oldCode = txtOldCode.Text,
            .yearBlock = chkYearBlock.Checked,
            .Level = CType(Me.cboLevel.SelectedItem.Value, Integer),
            .DepartmentID = getDepartment1.getID}
        vContext.subjects.AddObject(vsubject)
        vContext.SaveChanges()
    End Sub

    Protected Sub Updatesubject()
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vsubject As subject = _
            (From p In vContext.subjects _
                Where p.ID = CType(lblID.Text, Integer) _
                    Select p).First
        With vsubject
            .longName = Me.txtShortName.Text
            .shortName = Me.txtCode.Text
            .Code = txtCode.Text
            .oldCode = txtOldCode.Text
            .yearBlock = chkYearBlock.Checked
            .Level = CType(Me.cboLevel.SelectedItem.Value, Integer)
        End With
        vContext.SaveChanges()
    End Sub

    Protected Sub Deletesubject()
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vsubject = (From p In vContext.subjects _
                            Where p.ID = CType(Me.lblID.Text, Integer) _
                            Select p).First
        vContext.DeleteObject(vsubject)
        vContext.SaveChanges()
    End Sub

    Private Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click

        Try
            If Me.lblID.Text = "" Then
                Createsubject()
            Else
                Updatesubject()
            End If
            loadsubjects(getDepartment1.getID)
            lblMessage.Text = ""
        Catch ex As Exception
            lblMessage.Text = ex.Message
        End Try
    End Sub

    Private Sub btnDelete_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnDelete.Click
        Try
            Deletesubject()
            loadsubjects(getDepartment1.getID)
            lblMessage.Text = ""
        Catch ex As Exception
            lblMessage.Text = ex.Message
        End Try
    End Sub


    Private Sub getDepartment1_DepartmentClick(E As Object, Args As clsDepartmentEvent) Handles getDepartment1.DepartmentClick
        loadsubjects(Args.mDepartmentID)
    End Sub
End Class