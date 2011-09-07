Public Class uploadVenues
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            PrepareFile()
        End If
    End Sub

    Protected Sub PrepareFile()
        'create valid fields
        uploadFile1.filetotable = New clsfiletotableconversion
        'Dim vfiletotable As New clsfiletotableconversion
        With uploadFile1.filetotable
            .fields = New ListItemCollection()
            ' Add items to the collection.
            .fields.Add(New ListItem("cluster", "0"))
            .fields.Add(New ListItem("site", "0"))
            .fields.Add(New ListItem("buildingname", "0"))
            .fields.Add(New ListItem("buildingcode", "0"))
            .fields.Add(New ListItem("roomno", "0"))
            .fields.Add(New ListItem("spaceuse", "0"))
            .fields.Add(New ListItem("size", "0"))
            .displayMessage()
        End With
        uploadFile1.header = "Upload Venue File"
        uploadFile1.Initialize()
    End Sub

    Private Sub uploadFile1_UploadComplete(ByVal sender As Object, ByVal e As System.EventArgs) Handles uploadFile1.UploadComplete
        processFile(uploadFile1.filetotable.pDatatable)
    End Sub


    Function generateClusterID() As Integer
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


    Function generateSiteID() As Integer
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

    Protected Sub processFile(ByVal vDatatable As DataTable)
        Dim vContext As timetableEntities = New timetableEntities()
        Dim defaultlabType = 7
        Dim defaultlecturetype = 8
        Dim rowindex = 0
        For Each dr As DataRow In vDatatable.Rows
            Try
                ''''get delivery siteid
                'check site  
                Dim vclusterid = CInt(dr("cluster"))
                Dim vsitename = Trim(CStr(dr("site")))
                Dim vbuildingcode = Trim(CStr(dr("buildingcode")))
                Dim vbuildingname = Trim(CStr(dr("buildingname")))
                Dim vspaceuse = CStr(dr("spaceuse"))
                Dim vroomno = CStr(dr("roomno"))
                Dim vsize = 0
                Try
                    vsize = CInt(dr("size"))
                Catch ex As Exception
                    vsize = 20
                End Try

                'set cluster
                Dim vCluster = (From p In vContext.siteclusters Where p.ID = vclusterid Select p).Single

                'set site
                Dim vSite = (From p In vContext.sites Where p.longName = vsitename And p.SiteClusterID = vCluster.ID Select p).SingleOrDefault
                If IsNothing(vSite) Then
                    vSite = New site With {
                        .ID = generateSiteID(),
                        .longName = vsitename,
                        .shortName = vsitename,
                        .Address = vsitename,
                        .SiteClusterID = vCluster.ID}
                    vContext.sites.AddObject(vSite)
                    vContext.SaveChanges()
                End If


                'start with building
                Dim vBuilding = (From p In vContext.buildings
                                   Where p.shortName = vbuildingcode And
                                         p.SiteID = vSite.ID Select p).FirstOrDefault
                If IsNothing(vBuilding) Then
                    ''create new building
                    vBuilding = New building With {
                        .longName = vbuildingname,
                        .shortName = vbuildingcode,
                        .SiteID = vSite.ID}
                    vContext.buildings.AddObject(vBuilding)
                    vContext.SaveChanges()
                End If

                'verify resourcetype
                Dim vResourceType = (From p In vContext.resourcetypes Where p.Description = vspaceuse Select p).FirstOrDefault
                If IsNothing(vResourceType) Then
                    If InStr(LCase(vspaceuse), "computer") > 0 Then
                        vResourceType = (From p In vContext.resourcetypes Where p.ID = defaultlabType Select p).Single
                    Else
                        vResourceType = (From p In vContext.resourcetypes Where p.ID = defaultlecturetype Select p).Single
                    End If
                End If

                'check venue
                Dim vVenue = (From p In vContext.venues Where p.BuildingID = vBuilding.ID And p.Code = vroomno Select p).FirstOrDefault
                If IsNothing(vVenue) Then
                    vVenue = New venue With {
                        .BuildingID = vBuilding.ID,
                        .TypeID = vResourceType.ID,
                        .Capacity = vsize,
                        .Description = vroomno,
                        .Code = vroomno}
                    vContext.venues.AddObject(vVenue)
                    vContext.SaveChanges()
                End If
            Catch ex As Exception
                uploadFile1.errorlist = New ListItem("Row No:" + CStr(rowindex + 1) + "-->err:" + ex.Message)
            End Try
            rowindex += 1
        Next
    End Sub

End Class