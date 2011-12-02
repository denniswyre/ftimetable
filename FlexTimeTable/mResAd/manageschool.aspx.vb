Public Class manageschool
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            loadFaculty()
            logSave.Text = "Save"
            logDelete.Text = "Delete"
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
        ErrorMessage.Text = ""
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
                Me.logDelete.Visible = True
                logSave.Visible = True
                logSave.Text = "Update"
                litEdit.Text = "Edit School"
            Case eMode.create
                Me.lblID.Text = ""
                Me.txtLongName.Text = ""
                Me.txtShortName.Text = ""
                Me.logDelete.Visible = False
                logSave.Visible = True
                logSave.Text = "Save"
                litEdit.Text = "Create School"
        End Select
    End Sub

    Private Sub lnkCreate_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles lnkCreate.Click
        changeMode(eMode.create)
        ErrorMessage.Text = ""
    End Sub

    Protected Sub btnCancel_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnCancel.Click
        ErrorMessage.Text = ""
        mvSchool.SetActiveView(vwGrid)
    End Sub

    Protected Sub Createschool()
        logSave.Function = "Create School"
        logSave.Description = "Create:" + txtCode.Text + "---" + txtLongName.Text
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
        logSave.Function = "Update School"
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vschool As school = _
            (From p In vContext.schools _
                Where p.ID = CType(lblID.Text, Integer) _
                    Select p).First
        logSave.Description = "changed from:" + vschool.code + "---" + vschool.longName + "---changed to:" + txtCode.Text + "----" + txtLongName.Text
        With vschool
            .code = Me.txtCode.Text
            .longName = Me.txtLongName.Text
            .shortName = Me.txtShortName.Text
        End With
        vContext.SaveChanges()
    End Sub

    Protected Sub Deleteschool()
        logDelete.Function = "Delete School"
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vschool = (From p In vContext.schools _
                            Where p.id = CType(Me.lblID.Text, Integer) _
                            Select p).First
        logDelete.Description = "Delete:" + vschool.code + "---" + vschool.longName
        Dim DummyFacultyID = CType(ConfigurationManager.AppSettings("dummyfaculty"), Integer)
        If vschool.facultyID = DummyFacultyID Then
            Throw New Exception("You cannot delete this school!!!")
        End If
        vContext.DeleteObject(vschool)
        vContext.SaveChanges()
        loadschools()
    End Sub


    Private Sub logDelete_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles logDelete.Click
        Try
            Deleteschool()
            ErrorMessage.Text = ""
        Catch ex As Exception
            ErrorMessage.Text = clsGeneral.displaymessage(ex.Message, True)
        End Try
    End Sub

    Private Sub cboFaculty_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cboFaculty.SelectedIndexChanged
        loadschools()
        ErrorMessage.Text = ""
    End Sub

    Private Sub logSave_Click(sender As Object, e As System.EventArgs) Handles logSave.Click
        Try
            If Me.lblID.Text = "" Then
                Createschool()
            Else
                Updateschool()
            End If
            loadschools()
            ErrorMessage.Text = ""
        Catch ex As Exception
            ErrorMessage.Text = clsGeneral.displaymessage(ex.Message, True)
        End Try
        
    End Sub
End Class