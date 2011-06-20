Public Class managesites
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            loadCampus()
            btnSave.Text = "Save"
            btnDelete.Text = "Delete"
            mvSite.SetActiveView(vwGrid)
        End If
    End Sub

    Sub loadCampus()
        Dim vContext As timetableEntities = New timetableEntities()
        Dim OfficerID As Integer = clsOfficer.getOfficer(User.Identity.Name).ID
        cboCampus.DataSource = (From p In vContext.campususers _
                                     Where p.OfficerID = OfficerID _
                                       Select p.CampusName, p.CampusID)
        cboCampus.DataTextField = "CampusName"
        cboCampus.DataValueField = "CampusID"
        cboCampus.DataBind()
        loadCluster()
    End Sub

    Sub loadCluster()
        If cboCampus.SelectedIndex >= 0 Then
            Dim vContext As timetableEntities = New timetableEntities()
            Dim CampusID As Integer = CType(cboCampus.SelectedValue, Integer)
            cboCluster.DataSource = (From p In vContext.siteclusters _
                                          Where p.CampusID = CampusID _
                                            Order By p.longName _
                                               Select p.longName, p.ID)
        Else
            cboCluster.DataSource = Nothing
        End If
        cboCluster.DataTextField = "longName"
        cboCluster.DataValueField = "ID"
        cboCluster.DataBind()
        loadSites()
    End Sub

    Sub loadSites()
        If cboCluster.SelectedIndex >= 0 Then
            Dim vClusterID = CType(cboCluster.SelectedValue, Integer)
            Dim vContext As timetableEntities = New timetableEntities()
            grdSite.DataSource = (From p In vContext.sites Where p.SiteClusterID = vClusterID Select p.shortName, p.longName, p.ID)
            litCluster.Text = "Cluster:"
            lnkCreate.Visible = True
        Else
            grdSite.DataSource = Nothing
            litCluster.Text = "No Site Cluster Found:"
            lnkCreate.Visible = False
        End If
        grdSite.DataBind()
        mvSite.SetActiveView(vwGrid)
    End Sub

    Private Sub grdSite_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles grdSite.SelectedIndexChanged
        ''populate field
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vID = CType(grdSite.SelectedRow.Cells(0).Text, Integer)
        Dim vSite As site = _
                   (From p In vContext.sites _
                       Where p.ID = vID Select p).First
        With vSite
            lblID.Text = .ID.ToString
            txtLongName.Text = .longName
            txtShortName.Text = .shortName
            txtAddress.Text = .Address
        End With
        changeMode(eMode.edit)

    End Sub

    Enum eMode
        edit
        delete
        create
    End Enum

    Sub changeMode(ByVal vMode As eMode)
        mvSite.SetActiveView(vwEdit)
        Select Case vMode
            Case eMode.edit
                btnDelete.Visible = True
                btnSave.Visible = True
                btnSave.Text = "Update"
                litEdit.Text = "Edit Site Cluster"
            Case eMode.create
                lblID.Text = ""
                txtLongName.Text = ""
                txtShortName.Text = ""
                btnDelete.Visible = False
                btnSave.Visible = True
                btnSave.Text = "Save"
                litEdit.Text = "Create Site Cluster"
        End Select
    End Sub

    Private Sub lnkCreate_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles lnkCreate.Click
        changeMode(eMode.create)
    End Sub

    Protected Sub btnCancel_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnCancel.Click
        mvSite.SetActiveView(vwGrid)
    End Sub

    Function generateNewID() As Integer
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vSite = (From p In vContext.sites Select p).ToList
        For i = 1 To 999
            Dim fd = False
            For Each x In vSite
                If x.ID = i Then
                    fd = True
                End If
            Next
            If Not fd Then
                Return i
            End If
        Next
        Return 0
    End Function

    Protected Sub CreateSite()
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vSite = New site With {
            .ID = generateNewID(),
            .longName = txtLongName.Text,
            .shortName = txtShortName.Text,
            .Address = txtAddress.Text,
            .SiteClusterID = CType(cboCluster.SelectedItem.Value, Integer)}
        vContext.sites.AddObject(vSite)
        vContext.SaveChanges()
    End Sub

    Protected Sub UpdateSite()
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vSite As site = _
            (From p In vContext.sites _
                Where p.ID = CType(lblID.Text, Integer) _
                    Select p).First
        With vSite
            .longName = txtLongName.Text
            .shortName = txtShortName.Text
            .Address = txtAddress.Text
        End With
        vContext.SaveChanges()
    End Sub

    Protected Sub DeleteSite()
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vSite = (From p In vContext.sites _
                            Where p.ID = CType(lblID.Text, Integer) _
                            Select p).First
        vContext.DeleteObject(vSite)
        vContext.SaveChanges()
        loadSites()
    End Sub

    Private Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
        If lblID.Text = "" Then
            CreateSite()
        Else
            UpdateSite()
        End If
        loadSites()
    End Sub

    Private Sub btnDelete_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnDelete.Click
        DeleteSite()
    End Sub

    Private Sub cboCluster_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cboCluster.SelectedIndexChanged
        loadSites()
    End Sub

    Protected Sub cboCampus_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles cboCampus.SelectedIndexChanged
        loadCluster()
    End Sub
End Class