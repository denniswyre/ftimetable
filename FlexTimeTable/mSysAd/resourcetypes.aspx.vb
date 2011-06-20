Public Class resourcetypes
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            loadresourcetypes()
            btnSave.Text = "Save"
            btnDelete.Text = "Delete"
            mvResType.SetActiveView(vwGrid)
        End If
    End Sub

    Sub loadresourcetypes()
        Dim vContext As timetableEntities = New timetableEntities()
        grdresourcetype.DataSource = (From p In vContext.resourcetypes _
                                     Select p.isClassRoom, p.code, p.Description, p.ID)
        Me.grdresourcetype.DataBind()
        mvResType.SetActiveView(vwGrid)
    End Sub

    Private Sub grdresourcetype_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles grdresourcetype.SelectedIndexChanged
        ''populate field
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vID = CType(grdresourcetype.SelectedRow.Cells(0).Text, Integer)
        Dim VType = (From p In vContext.resourcetypes Where p.ID = vID _
                                     Select p).First
        With VType
            Me.lblID.Text = .ID.ToString
            Me.txtCode.Text = .code.ToString
            Me.txtDescription.Text = .Description
            chkClassroom.Checked = .isClassRoom
            changeMode(eMode.edit)
        End With
    End Sub

    Enum eMode
        edit
        delete
        create
    End Enum

    Sub changeMode(ByVal vMode As eMode)
        mvResType.SetActiveView(vwEdit)
        Select Case vMode
            Case eMode.edit
                Me.btnDelete.Visible = True
                Me.btnSave.Visible = True
                btnSave.Text = "Update"
                litEdit.Text = "Edit Resource Type"
            Case eMode.create
                Me.lblID.Text = ""
                Me.txtCode.Text = ""
                Me.txtDescription.Text = ""
                Me.btnDelete.Visible = False
                Me.btnSave.Visible = True
                btnSave.Text = "Save"
                litEdit.Text = "Create Resource Type"
        End Select
    End Sub

    Private Sub lnkCreate_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles lnkCreate.Click
        changeMode(eMode.create)
    End Sub

    Protected Sub btnCancel_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnCancel.Click
        mvResType.SetActiveView(vwGrid)
    End Sub

    Protected Sub CreateSiteresourcetype()
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vresourcetype = New resourcetype With {
            .code = Me.txtCode.Text,
            .Description = Me.txtDescription.Text,
            .isClassRoom = chkClassroom.Checked}
        vContext.resourcetypes.AddObject(vresourcetype)
        vContext.SaveChanges()
    End Sub

    Protected Sub UpdateSiteresourcetype()
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vresourcetype As resourcetype = _
            (From p In vContext.resourcetypes _
                Where p.ID = CType(lblID.Text, Integer) _
                    Select p).First
        With vresourcetype
            .code = Me.txtCode.Text
            .Description = Me.txtDescription.Text
            .isClassRoom = chkClassroom.Checked
        End With
        vContext.SaveChanges()
    End Sub

    Protected Sub DeleteSiteresourcetype()
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vresourcetype = (From p In vContext.resourcetypes _
                            Where p.ID = CType(Me.lblID.Text, Integer) _
                            Select p).First
        vContext.DeleteObject(vresourcetype)
        vContext.SaveChanges()
    End Sub

    Private Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
        Try
            If Me.lblID.Text = "" Then
                CreateSiteresourcetype()
            Else
                UpdateSiteresourcetype()
            End If
            loadresourcetypes()
        Catch ex As Exception
            lblMessage.Text = ex.Message
        End Try
    End Sub

    Private Sub btnDelete_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnDelete.Click
        Try
            DeleteSiteresourcetype()
            loadresourcetypes()
        Catch ex As Exception
            lblMessage.Text = ex.Message
        End Try
    End Sub

End Class
