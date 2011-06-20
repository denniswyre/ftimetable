Public Class managecluster
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            loadCampus()
            btnSave.Text = "Save"
            btnDelete.Text = "Delete"
            mvCluster.SetActiveView(vwGrid)
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
        loadClusters()
    End Sub

    Sub loadClusters()
        If cboCampus.SelectedIndex >= 0 Then
            Dim vContext As timetableEntities = New timetableEntities()
            GrdClusters.DataSource = (From p In vContext.siteclusters _
                                        Where p.CampusID = CInt(cboCampus.SelectedValue) _
                                          Select p.shortName, p.longName, p.ID)
            lnkCreate.Visible = True
        Else
            GrdClusters.DataSource = Nothing
            lnkCreate.Visible = False
        End If
        Me.GrdClusters.DataBind()
        mvCluster.SetActiveView(vwGrid)
    End Sub

    Private Sub grdClusters_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles GrdClusters.SelectedIndexChanged
        ''populate field
        With grdClusters
            Me.lblID.Text = .SelectedRow.Cells(0).Text
            Me.txtLongName.Text = .SelectedRow.Cells(1).Text
            Me.txtShortName.Text = .SelectedRow.Cells(2).Text
            changeMode(eMode.edit)
        End With

    End Sub

    Enum eMode
        edit
        delete
        create
    End Enum

    Sub changeMode(ByVal vMode As eMode)
        mvCluster.SetActiveView(vwEdit)
        Select Case vMode
            Case eMode.edit
                Me.btnDelete.Visible = True
                Me.btnSave.Visible = True
                btnSave.Text = "Update"
                litEdit.Text = "Edit Site Cluster"
            Case eMode.create
                Me.lblID.Text = ""
                Me.txtLongName.Text = ""
                Me.txtShortName.Text = ""
                Me.btnDelete.Visible = False
                Me.btnSave.Visible = True
                btnSave.Text = "Save"
                litEdit.Text = "Create Site Cluster"
        End Select
    End Sub

    Private Sub lnkCreate_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles lnkCreate.Click
        changeMode(eMode.create)
    End Sub

    Protected Sub btnCancel_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnCancel.Click
        mvCluster.SetActiveView(vwGrid)
    End Sub

    Protected Sub CreateSiteCluster()
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vSiteCluster = New sitecluster With {
            .ID = generateNewID(),
            .longName = Me.txtLongName.Text,
            .shortName = Me.txtShortName.Text,
            .CampusID = CType(Me.cboCampus.SelectedItem.Value, Integer)}
        vContext.siteclusters.AddObject(vSiteCluster)
        vContext.SaveChanges()
    End Sub

    Function generateNewID() As Integer
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vSiteCluster = (From p In vContext.siteclusters Select p).ToList
        For i = 1 To 999
            Dim fd = False
            For Each x In vSiteCluster
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

    Protected Sub UpdateSiteCluster()
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vSiteCluster As sitecluster = _
            (From p In vContext.siteclusters _
                Where p.ID = CType(lblID.Text, Integer) _
                    Select p).First
        With vSiteCluster
            .longName = Me.txtLongName.Text
            .shortName = Me.txtShortName.Text
        End With
        vContext.SaveChanges()
    End Sub

    Protected Sub DeleteSiteCluster()
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vSiteCluster = (From p In vContext.siteclusters _
                            Where p.ID = CType(Me.lblID.Text, Integer) _
                            Select p).First
        vContext.DeleteObject(vSiteCluster)
        vContext.SaveChanges()
        loadClusters()
    End Sub

    Private Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
        If Me.lblID.Text = "" Then
            'ucSave.Function = "Create SiteCluster"
            'ucSave.Description = Me.txtLongName.Text
            CreateSiteCluster()
        Else
            'ucSave.Function = "Update SiteCluster"
            'ucSave.Description = "Cluster Site ID:" + Me.lblID.Text + _
            '" New Campus:" + Me.cboCampus.SelectedValue + " Previous Campus:" + grdClusters.SelectedRow.Cells(1).Text + _
            '" New Name:" + Me.txtLongName.Text + " Previous Name:" + grdClusters.SelectedRow.Cells(3).Text
            UpdateSiteCluster()
        End If
        loadClusters()
    End Sub

    Private Sub btnDelete_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnDelete.Click
        'ucDelete.Function = "Delete SiteCluster"
        'ucDelete.Description = "Cluster Site ID:" + Me.lblID.Text + _
        '   " Campus:" + grdClusters.SelectedRow.Cells(1).Text + _
        '  " Name:" + grdClusters.SelectedRow.Cells(3).Text
        DeleteSiteCluster()
    End Sub

    Private Sub cboCampus_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cboCampus.SelectedIndexChanged
        loadClusters()
    End Sub
End Class