Public Class managebuilding
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            loadCampus()
            btnSave.Text = "Save"
            btnDelete.Text = "Delete"
        End If
    End Sub

    Sub loadCampus()
        Dim vContext As timetableEntities = New timetableEntities()
        Dim OfficerID As Integer = clsOfficer.getOfficer(User.Identity.Name).ID
        Me.cboCampus.DataSource = (From p In vContext.campususers _
                                     Where p.OfficerID = OfficerID _
                                       Select p.CampusName, p.CampusID)
        Me.cboCampus.DataTextField = "CampusName"
        Me.cboCampus.DataValueField = "CampusID"
        Me.cboCampus.DataBind()

        loadSites()
    End Sub

    Sub loadSites()
        Dim vContext As timetableEntities = New timetableEntities
        If cboCampus.SelectedIndex >= 0 Then
            Dim CampusID = CType(Me.cboCampus.SelectedValue, Integer)
            Me.cboSites.DataSource = (From p In vContext.sites _
                                Where p.sitecluster.CampusID = CampusID _
                                Order By p.sitecluster.longName, p.longName _
                                  Select longName = (p.longName + "," + p.sitecluster.longName), p.ID)
        Else
            Me.cboSites.DataSource = Nothing
        End If
        Me.cboSites.DataTextField = "longName"
        Me.cboSites.DataValueField = "ID"
        Me.cboSites.DataBind()
        loadBuildings()
    End Sub

    Sub loadBuildings()
        Dim vContext As timetableEntities = New timetableEntities()
        If cboSites.SelectedIndex >= 0 Then
            Dim SiteID = CType(cboSites.SelectedValue, Integer)
            Me.grdBuilding.DataSource = (From p In vContext.buildings Where p.SiteID = SiteID _
                                           Select p)
            litSite.Text = "Site:"
            lnkCreate.Visible = True
        Else
            Me.grdBuilding.DataSource = Nothing
            litSite.Text = "No Site Created for this Campus:"
            lnkCreate.Visible = False
        End If
        Me.grdBuilding.DataBind()
        mvBuilding.SetActiveView(vwGrid)
    End Sub

    Private Sub grdBuilding_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles grdBuilding.SelectedIndexChanged
        ''populate field
        With grdBuilding
            Me.lblID.Text = .SelectedRow.Cells(0).Text
            Me.txtShortName.Text = .SelectedRow.Cells(1).Text
            Me.txtLongName.Text = .SelectedRow.Cells(2).Text
            changeMode(eMode.edit)
        End With

    End Sub

    Enum eMode
        edit
        delete
        create
    End Enum

    Sub changeMode(ByVal vMode As eMode)
        mvBuilding.SetActiveView(vwEdit)
        Select Case vMode
            Case eMode.edit
                Me.btnDelete.Visible = True
                Me.btnSave.Visible = True
                btnSave.Text = "Update"
                litEdit.Text = "Edit Building"
            Case eMode.create
                Me.lblID.Text = ""
                Me.txtLongName.Text = ""
                Me.txtShortName.Text = ""
                Me.btnDelete.Visible = False
                Me.btnSave.Visible = True
                btnSave.Text = "Save"
                litEdit.Text = "Create Building"
        End Select
    End Sub

    Private Sub lnkCreate_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles lnkCreate.Click
        changeMode(eMode.create)
    End Sub

    Protected Sub btnCancel_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnCancel.Click
        mvBuilding.SetActiveView(vwGrid)
    End Sub

    Protected Sub Createbuilding()
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vBuilding = New building With {
            .longName = Me.txtLongName.Text,
            .shortName = Me.txtShortName.Text,
            .SiteID = CType(Me.cboSites.SelectedItem.Value, Integer)}
        vContext.buildings.AddObject(vBuilding)
        vContext.SaveChanges()
    End Sub

    Protected Sub Updatebuilding()
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vbuilding As building = _
            (From p In vContext.buildings _
                Where p.ID = CType(lblID.Text, Integer) _
                    Select p).First
        With vbuilding
            .longName = Me.txtLongName.Text
            .shortName = Me.txtShortName.Text
        End With
        vContext.SaveChanges()
    End Sub

    Protected Sub Deletebuilding()
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vbuilding = (From p In vContext.buildings _
                            Where p.ID = CType(Me.lblID.Text, Integer) _
                            Select p).First
        vContext.DeleteObject(vbuilding)
        vContext.SaveChanges()
    End Sub

    Private Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
        If Me.lblID.Text = "" Then
            'ucSave.Function = "Create SiteCluster"
            'ucSave.Description = Me.txtLongName.Text
            Createbuilding()
        Else
            'ucSave.Function = "Update SiteCluster"
            'ucSave.Description = "Cluster Site ID:" + Me.lblID.Text + _
            '" New Campus:" + Me.cboCampus.SelectedValue + " Previous Campus:" + grdBuilding.SelectedRow.Cells(1).Text + _
            '" New Name:" + Me.txtLongName.Text + " Previous Name:" + grdBuilding.SelectedRow.Cells(3).Text
            Updatebuilding()
        End If
        loadBuildings()
    End Sub

    Private Sub btnDelete_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnDelete.Click
        'ucDelete.Function = "Delete SiteCluster"
        'ucDelete.Description = "Cluster Site ID:" + Me.lblID.Text + _
        '   " Campus:" + grdBuilding.SelectedRow.Cells(1).Text + _
        '   " Name:" + grdBuilding.SelectedRow.Cells(3).Text
        Deletebuilding()
        loadBuildings()
    End Sub

    Private Sub cboCampus_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cboCampus.SelectedIndexChanged
        loadSites()
    End Sub

    Private Sub cboSites_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cboSites.SelectedIndexChanged
        loadBuildings()
    End Sub
End Class