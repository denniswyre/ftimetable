Public Class managecampus
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            loadCampus()
            btnSave.Text = "Save"
            btnDelete.Text = "Delete"
            Me.pnlAction.Visible = False
        End If
    End Sub


    Sub loadCampus()
        Dim vContext As timetableEntities = New timetableEntities()
        GrdCampus.DataSource = (From p In vContext.campus _
                                    Select p.shortName, p.longName, p.ID)
        Me.GrdCampus.DataBind()
    End Sub

    Private Sub grdCampus_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles GrdCampus.SelectedIndexChanged
        ''populate field
        With GrdCampus
            Me.txtID.Text = .SelectedRow.Cells(0).Text
            Me.txtLongName.Text = .SelectedRow.Cells(1).Text
            Me.txtShortName.Text = .SelectedRow.Cells(2).Text
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
        Me.pnlAction.Visible = True
        Select Case vMode
            Case eMode.edit
                Me.txtID.Enabled = False
                Me.btnDelete.Visible = True
                Me.btnSave.Visible = True
                btnSave.Text = "Update"
                Me.pnlAction.GroupingText = "Edit Campus"
            Case eMode.create
                Me.txtID.Enabled = True
                Me.txtID.Text = ""
                Me.txtLongName.Text = ""
                Me.txtShortName.Text = ""
                Me.btnDelete.Visible = False
                Me.btnSave.Visible = True
                btnSave.Text = "Save"
                Me.pnlAction.GroupingText = "Site Campus"
        End Select
    End Sub

    Private Sub lnkCreate_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles lnkCreate.Click
        changeMode(eMode.create)
        lblMessage.Text = ""
    End Sub

    Protected Sub btnCancel_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnCancel.Click
        Me.pnlAction.Visible = False
        lblMessage.Text = ""
    End Sub

    Protected Sub CreateCampus()
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vCampus = New campu With {
            .ID = CType(Me.txtID.Text, Integer),
            .shortName = Me.txtShortName.Text,
            .longName = Me.txtLongName.Text}
        vContext.campus.AddObject(vCampus)
        vContext.SaveChanges()
    End Sub

    Protected Sub UpdateCampus()
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vCampus As campu = _
            (From p In vContext.campus _
                Where p.ID = CType(txtID.Text, Integer) _
                    Select p).First
        With vCampus
            .longName = Me.txtLongName.Text
            .shortName = Me.txtShortName.Text
        End With
        vContext.SaveChanges()
    End Sub

    Protected Sub DeleteCampus()
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vCampus = (From p In vContext.campus _
                            Where p.ID = CType(Me.txtID.Text, Integer) _
                            Select p).First
        vContext.DeleteObject(vCampus)
        vContext.SaveChanges()
    End Sub

    Private Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
        Try
            If Me.txtID.Enabled Then
                CreateCampus()
            Else
                UpdateCampus()
            End If
            lblMessage.Text = ""
        Catch ex As Exception
            lblMessage.Text = ex.Message
        End Try
    End Sub

    Private Sub btnDelete_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnDelete.Click
        Try
            DeleteCampus()
            loadCampus()
            Me.pnlAction.Visible = False
            lblMessage.Text = ""
        Catch ex As Exception
            lblMessage.Text = ex.Message
        End Try
    End Sub

   
End Class