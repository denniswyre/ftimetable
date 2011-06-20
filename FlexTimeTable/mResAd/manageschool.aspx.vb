Public Class manageschool
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            loadFaculty()
            btnSave.Text = "Save"
            btnDelete.Text = "Delete"
        End If
    End Sub

    Sub loadFaculty()
        Dim vContext As timetableEntities = New timetableEntities()
        Dim OfficerID As Integer = clsOfficer.getOfficer(User.Identity.Name).ID
        Me.cboFaculty.DataSource = (From p In vContext.Facultyusers _
                                     Where p.OfficerID = OfficerID _
                                       Select p.FacultyName, p.FacultyID)
        Me.cboFaculty.DataTextField = "FacultyName"
        Me.cboFaculty.DataValueField = "FacultyID"
        Me.cboFaculty.DataBind()
        loadschools()
    End Sub

    Sub loadschools()
        If cboFaculty.SelectedIndex >= 0 Then
            Dim vContext As timetableEntities = New timetableEntities()
            Grdschools.DataSource = (From p In vContext.schools _
                                        Where p.facultyID = CInt(cboFaculty.SelectedValue) _
                                          Select p)
            lnkCreate.Visible = True
        Else
            Grdschools.DataSource = Nothing
            lnkCreate.Visible = False
        End If
        Me.Grdschools.DataBind()
        mvSchool.SetActiveView(vwGrid)
    End Sub

    Private Sub grdschools_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles Grdschools.SelectedIndexChanged
        ''populate field
        Dim vContext As timetableEntities = New timetableEntities()

        Dim vID As Integer = CType(Grdschools.SelectedRow.Cells(0).Text, Integer)
        Dim vSchool = (From p In vContext.schools _
                          Where p.id = vID _
                            Select p).First
        With vSchool
            Me.lblID.Text = Grdschools.SelectedRow.Cells(0).Text
            Me.txtCode.Text = .code
            Me.txtLongName.Text = .longName
            Me.txtShortName.Text = .shortName
            changeMode(eMode.edit)
        End With

    End Sub

    Enum eMode
        edit
        delete
        create
    End Enum

    Sub changeMode(ByVal vMode As eMode)
        mvSchool.SetActiveView(vwEdit)
        Select Case vMode
            Case eMode.edit
                Me.btnDelete.Visible = True
                Me.btnSave.Visible = True
                btnSave.Text = "Update"
                litEdit.Text = "Edit School"
            Case eMode.create
                Me.lblID.Text = ""
                Me.txtLongName.Text = ""
                Me.txtShortName.Text = ""
                Me.btnDelete.Visible = False
                Me.btnSave.Visible = True
                btnSave.Text = "Save"
                litEdit.Text = "Create School"
        End Select
    End Sub

    Private Sub lnkCreate_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles lnkCreate.Click
        changeMode(eMode.create)
    End Sub

    Protected Sub btnCancel_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnCancel.Click
        mvSchool.SetActiveView(vwGrid)
    End Sub

    Protected Sub Createschool()
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vschool = New school With {
            .code = Me.txtCode.Text,
            .longName = Me.txtLongName.Text,
            .shortName = Me.txtShortName.Text,
            .facultyID = CType(cboFaculty.SelectedValue, Integer)}
        vContext.schools.AddObject(vschool)
        vContext.SaveChanges()
    End Sub

    Protected Sub Updateschool()
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vschool As school = _
            (From p In vContext.schools _
                Where p.ID = CType(lblID.Text, Integer) _
                    Select p).First
        With vschool
            .code = Me.txtCode.Text
            .longName = Me.txtLongName.Text
            .shortName = Me.txtShortName.Text
        End With
        vContext.SaveChanges()
    End Sub

    Protected Sub Deleteschool()
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vschool = (From p In vContext.schools _
                            Where p.ID = CType(Me.lblID.Text, Integer) _
                            Select p).First
        vContext.DeleteObject(vschool)
        vContext.SaveChanges()
        loadschools()
    End Sub

    Private Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
        If Me.lblID.Text = "" Then
            Createschool()
        Else
            Updateschool()
        End If
        loadschools()
    End Sub

    Private Sub btnDelete_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnDelete.Click
        Deleteschool()
    End Sub

    Private Sub cboFaculty_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cboFaculty.SelectedIndexChanged
        loadschools()
    End Sub
End Class