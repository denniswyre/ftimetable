Imports System.Data
Public Class assignVenuestoDepart
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            loadCampus()
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
        Dim vContext As timetableEntities = New timetableEntities
        If cboCampus.SelectedIndex >= 0 Then
            Dim CampusID = CType(Me.cboCampus.SelectedValue, Integer)
            Me.cboCluster.DataSource = (From p In vContext.siteclusters _
                                Where p.CampusID = CampusID _
                                Order By p.longName _
                                  Select p.longName, p.ID)
        Else
            Me.cboCluster.DataSource = Nothing
        End If
        Me.cboCluster.DataTextField = "longName"
        Me.cboCluster.DataValueField = "ID"
        Me.cboCluster.DataBind()
        loadDepartments()
        loadVenues()
    End Sub

    Sub loadAssignedVenues()
        lstDepartVenues.Items.Clear()
        If cboDepartment.SelectedIndex > -1 Then
            Dim vContext As timetableEntities = New timetableEntities()
            Dim vDepartID = CInt(cboDepartment.SelectedValue)
            Dim vDepart = (From p In vContext.departments Where p.ID = vDepartID Select p).First
            Dim vVenues = (From p In vDepart.venues Where p.building.site.SiteClusterID = CInt(cboCluster.SelectedValue) Select name = (p.Code + "," + p.building.shortName), id = p.ID).ToList
            With lstDepartVenues
                .DataSource = vVenues
                .DataTextField = "name"
                .DataValueField = "id"
                .DataBind()
            End With
        Else
        End If
    End Sub

    Sub loadVenues()
        Dim vContext As timetableEntities = New timetableEntities()
        lstVenues.Items.Clear()
        If cboCluster.SelectedIndex >= 0 Then
            Dim clusterID = CType(cboCluster.SelectedValue, Integer)
            Me.lstVenues.DataSource = (From p In vContext.venues Where p.building.site.SiteClusterID = clusterID
                                           Select name = (p.Code + "," + p.building.shortName), p.ID)
            litCluster.Text = "Cluster:"
        Else
            Me.lstVenues.DataSource = Nothing
            litCluster.Text = "No Cluster found for this Campus:"
        End If
        lstVenues.DataTextField = "Name"
        lstVenues.DataValueField = "ID"
        Me.lstVenues.DataBind()
    End Sub

    Sub loadDepartments()
        Dim vContext As timetableEntities = New timetableEntities()
        'get avaliable qualifications for link site cluster
        Dim vSiteCluster = (From p In vContext.siteclusters Where p.ID = CInt(cboCluster.SelectedValue) Select p).FirstOrDefault
        'get qualifications
        cboDepartment.Items.Clear()
        If Not IsNothing(vSiteCluster) Then
            For Each x In vSiteCluster.qualifications
                Dim DepartName As String = x.department.longName + "," + x.department.school.shortName
                Dim vItem As New ListItem(DepartName, CStr(x.DepartmentID))
                If cboDepartment.Items.IndexOf(vItem) < 0 Then
                    cboDepartment.Items.Add(vItem)
                End If
            Next
        End If
        loadAssignedVenues()
    End Sub

    Private Sub cboCampus_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cboCampus.SelectedIndexChanged
        loadClusters()
    End Sub

    Private Sub cboCluster_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cboCluster.SelectedIndexChanged
        loadVenues()
        loadDepartments()
    End Sub


    Protected Sub btnAdd_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnAdd.Click
        Dim vContext As timetableEntities = New timetableEntities()
        Try
            Dim vDept = (From p In vContext.departments Where p.ID = CInt(cboDepartment.SelectedValue) Select p).First
            Dim vVenue = (From p In vContext.venues Where p.ID = CInt(lstVenues.SelectedValue) Select p).First
            vVenue.departments.Add(vDept)
            vContext.SaveChanges()
            loadAssignedVenues()
            lblMessage.Text = ""
        Catch ex As UpdateException
            lblMessage.Text = "Entry already exists"
        Catch ex As Exception
            lblMessage.Text = "System Error!!! Contact ICT Administrator!!"
        End Try
    End Sub

    Protected Sub btnDelete_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnDelete.Click
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vDept = (From p In vContext.departments Where p.ID = CInt(cboDepartment.SelectedValue) Select p).First
        Dim vDeptVenue = (From p In vContext.venues Where p.ID = CInt(lstDepartVenues.SelectedValue) Select p).First
        vDeptVenue.departments.Remove(vDept)
        vContext.SaveChanges()
        loadAssignedVenues()
    End Sub

    Private Sub cboDepartment_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cboDepartment.SelectedIndexChanged
        loadAssignedVenues()
    End Sub
End Class