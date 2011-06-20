Public Class managefaculty
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            loadfaculty()
            btnSave.Text = "Save"
            btnDelete.Text = "Delete"
            Me.pnlAction.Visible = False
        End If
    End Sub


    Sub loadfaculty()
        Dim vContext As timetableEntities = New timetableEntities()
        Grdfaculty.DataSource = (From p In vContext.faculties _
                                    Select p.shortName, p.longName, p.ID)
        Me.Grdfaculty.DataBind()
    End Sub

    Private Sub grdfaculty_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles Grdfaculty.SelectedIndexChanged
        ''populate field
        With Grdfaculty
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
                Me.pnlAction.GroupingText = "Edit faculty"
            Case eMode.create
                Me.txtID.Enabled = True
                Me.txtID.Text = ""
                Me.txtLongName.Text = ""
                Me.txtShortName.Text = ""
                Me.btnDelete.Visible = False
                Me.btnSave.Visible = True
                btnSave.Text = "Save"
                Me.pnlAction.GroupingText = "Site faculty"
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

    Protected Sub Createfaculty()
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vfaculty = New faculty With {
            .ID = CType(Me.txtID.Text, Integer),
            .shortName = Me.txtShortName.Text,
            .longName = Me.txtLongName.Text}
        vContext.faculties.AddObject(vfaculty)
        vContext.SaveChanges()
    End Sub

    Protected Sub Updatefaculty()
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vfaculty As faculty = _
            (From p In vContext.faculties _
                Where p.ID = CType(txtID.Text, Integer) _
                    Select p).First
        With vfaculty
            .longName = Me.txtLongName.Text
            .shortName = Me.txtShortName.Text
        End With
        vContext.SaveChanges()
    End Sub

    Protected Sub Deletefaculty()
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vfaculty = (From p In vContext.faculties _
                            Where p.ID = CType(Me.txtID.Text, Integer) _
                            Select p).First
        vContext.DeleteObject(vfaculty)
        vContext.SaveChanges()
    End Sub

    Private Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
        Try
            If Me.txtID.Enabled Then
                Createfaculty()
            Else
                Updatefaculty()
            End If
            loadfaculty()
            Me.pnlAction.Visible = False
            lblMessage.Text = ""
        Catch ex As Exception
            lblMessage.Text = ex.Message
        End Try
    End Sub

    Private Sub btnDelete_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnDelete.Click
        Try
            Deletefaculty()
            loadfaculty()
            Me.pnlAction.Visible = False
            lblMessage.Text = ""
        Catch ex As Exception
            lblMessage.Text = ex.Message
        End Try
    End Sub


End Class