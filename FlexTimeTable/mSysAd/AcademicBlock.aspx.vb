Public Class AcademicBlock1
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            loadAcademicBlock()
            ''''''load weeks
            For i As Integer = 1 To 52
                cboStart.Items.Add("Week " + i.ToString)
                cboEnd.Items.Add("Week " + i.ToString)
            Next
            btnSave.Text = "Save"
            btnDelete.Text = "Delete"
            Me.pnlAction.Visible = False
        End If
    End Sub


    Sub loadAcademicBlock()
        Dim vContext As timetableEntities = New timetableEntities()
        GrdBlock.DataSource = (From p In vContext.academicblocks _
                                    Select p)
        Me.GrdBlock.DataBind()
    End Sub

    Private Sub GrdBlock_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles GrdBlock.SelectedIndexChanged
        ''populate field
        Dim vID As Integer = CType(GrdBlock.SelectedRow.Cells(0).Text, Integer)
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vBlock As academicblock = (From p In vContext.academicblocks Where p.ID = vID Select p).FirstOrDefault
        With vBlock
            lblID.Text = .ID.ToString
            txtName.Text = .Name
            chkYearBlock.Checked = .yearBlock
            cboStart.SelectedIndex = .startWeek - 1
            cboEnd.SelectedIndex = .endWeek - 1
        End With
        YearBlock()
        changeMode(eMode.edit)
        litMessage.Text = ""
    End Sub

    Enum eMode
        edit
        delete
        create
    End Enum

    Sub changeMode(ByVal vMode As eMode)
        Me.pnlAction.Visible = True
        Select Case vMode
            Case eMode.edit
                Me.btnDelete.Visible = True
                Me.btnSave.Visible = True
                btnSave.Text = "Update"
                Me.pnlAction.GroupingText = "Edit AcademicBlock"
            Case eMode.create
                Me.lblID.Text = ""
                Me.txtName.Text = ""
                cboStart.SelectedIndex = 0
                cboEnd.SelectedIndex = cboEnd.Items.Count - 1
                Me.btnDelete.Visible = False
                Me.btnSave.Visible = True
                btnSave.Text = "Save"
                Me.pnlAction.GroupingText = "Site AcademicBlock"
        End Select
    End Sub

    Private Sub lnkCreate_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles lnkCreate.Click
        changeMode(eMode.create)
        litMessage.Text = ""
    End Sub

    Protected Sub btnCancel_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnCancel.Click
        Me.pnlAction.Visible = False
        litMessage.Text = ""
    End Sub

    Function generateNewID() As Integer
        If GrdBlock.Rows.Count = 0 Then
            Return 1
        Else
            Return CInt(GrdBlock.Rows(GrdBlock.Rows.Count - 1).Cells(0).Text) + 1
        End If
    End Function

    Protected Sub CreateAcademicBlock()
        Dim vContext As timetableEntities = New timetableEntities()
        validateAcademicBlock(vContext)
        Dim vAcademicBlock = New academicblock With {
            .ID = generateNewID(),
            .Name = Me.txtName.Text,
            .yearBlock = chkYearBlock.Checked,
            .startWeek = cboStart.SelectedIndex + 1,
            .endWeek = cboEnd.SelectedIndex + 1}
        vContext.academicblocks.AddObject(vAcademicBlock)
        vContext.SaveChanges()
    End Sub

    Sub validateAcademicBlock(ByVal vContext As timetableEntities)
        Dim vCourseMinlength = CType(ConfigurationManager.AppSettings("CourseMinLength"), Integer)
        If chkYearBlock.Checked Then
            Dim vAcademicBlock = (From p In vContext.academicblocks
                                    Where p.yearBlock = True
                                        Select p).ToList
            If vAcademicBlock.Count > 0 Then
                Throw New Exception("Only one entry can have the Year Block Checked!!")
            End If
        Else
            If cboEnd.SelectedIndex < cboStart.SelectedIndex + vCourseMinlength Then
                Throw New Exception("The Block must be at least " + CStr(vCourseMinlength) + " weeks!!")
            End If
        End If
    End Sub

    Protected Sub UpdateAcademicBlock()
        Dim vContext As timetableEntities = New timetableEntities()
        validateAcademicBlock(vContext)
        Dim vAcademicBlock As academicblock = _
            (From p In vContext.academicblocks _
                Where p.ID = CInt(lblID.Text) _
                    Select p).First
        With vAcademicBlock
            .Name = Me.txtName.Text
            .yearBlock = chkYearBlock.Checked
            .startWeek = cboStart.SelectedIndex + 1
            .endWeek = cboEnd.SelectedIndex + 1
        End With
        vContext.SaveChanges()
    End Sub

    Protected Sub DeleteAcademicBlock()
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vAcademicBlock = (From p In vContext.academicblocks _
                            Where p.ID = CInt(Me.lblID.Text) _
                            Select p).First
        vContext.DeleteObject(vAcademicBlock)
        vContext.SaveChanges()
    End Sub

    Private Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
        Try
            If Me.lblID.Text = "" Then
                CreateAcademicBlock()
            Else
                UpdateAcademicBlock()
            End If
            litMessage.Text = ""
            loadAcademicBlock()
            Me.pnlAction.Visible = False
        Catch ex As Exception
            litMessage.Text = ex.Message
        End Try
    End Sub

    Sub YearBlock()
        If chkYearBlock.Checked Then
            cboStart.SelectedIndex = 0
            cboEnd.SelectedIndex = cboEnd.Items.Count - 1
            cboEnd.Enabled = False
            cboStart.Enabled = False
        Else
            cboEnd.Enabled = True
            cboStart.Enabled = True
        End If
    End Sub

    Private Sub btnDelete_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnDelete.Click
        Try
            DeleteAcademicBlock()
            loadAcademicBlock()
            Me.pnlAction.Visible = False
            litMessage.Text = ""
        Catch ex As Exception
            litMessage.Text = ex.Message
        End Try
    End Sub


    Protected Sub chkYearBlock_CheckedChanged(ByVal sender As Object, ByVal e As EventArgs) Handles chkYearBlock.CheckedChanged
        YearBlock()
    End Sub
End Class