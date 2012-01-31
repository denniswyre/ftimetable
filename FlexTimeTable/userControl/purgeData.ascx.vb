Public Class purgeData
    Inherits System.Web.UI.UserControl

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub


    Private Sub PurgeAcademic()
        ' delete resourceschedules
        Dim vContext As timetableEntities = New timetableEntities()

        'purge class first
        PurgeClass()

        ' delete subject
        Dim vsubject = (From d In vContext.subjects Select d).ToList
        For Each x In vsubject
            For Each y In x.siteclustersubjects.ToList
                x.siteclustersubjects.Remove(y)
            Next
            For Each y In x.programmesubjects.ToList
                x.programmesubjects.Remove(y)
            Next
            vContext.subjects.DeleteObject(x)
        Next
        vContext.SaveChanges()

        ' delete qual
        Dim vqual = (From d In vContext.qualifications Select d).ToList
        For Each x In vqual
            For Each y In x.siteclusters.ToList
                x.siteclusters.Remove(y)
            Next
            vContext.qualifications.DeleteObject(x)
        Next
        vContext.SaveChanges()

        ' delete depart
        Dim vdepart = (From d In vContext.departments Select d).ToList
        For Each x In vdepart
            For Each y In x.venues.ToList
                x.venues.Remove(y)
            Next
            vContext.departments.DeleteObject(x)
        Next
        vContext.SaveChanges()

        ' delete school
        Dim vschool = (From d In vContext.schools Select d).ToList
        For Each x In vschool
            vContext.schools.DeleteObject(x)
        Next
        vContext.SaveChanges()

    End Sub


    Private Sub PurgeSite()
        Dim vContext As timetableEntities = New timetableEntities()
        'delete venues
        Dim vVenue = (From d In vContext.venues Select d).ToList
        For Each x In vVenue
            vContext.venues.DeleteObject(x)
        Next
        vContext.SaveChanges()

        ' delete buildings and venues
        Dim vbuildings = (From d In vContext.buildings Select d).ToList
        For Each x In vbuildings
            vContext.buildings.DeleteObject(x)
        Next
        vContext.SaveChanges()

        'delete sites
        Dim vsites = (From d In vContext.sites Select d).ToList
        For Each x In vsites
            vContext.sites.DeleteObject(x)
        Next
        vContext.SaveChanges()

        'delete cluster sites
        Dim vsiteclusters = (From d In vContext.siteclusters Select d).ToList
        For Each x In vsiteclusters
            vContext.siteclusters.DeleteObject(x)
        Next
        vContext.SaveChanges()

    End Sub


    Private Sub PurgeClass()
        ' delete resourceschedules
        Dim vContext As timetableEntities = New timetableEntities()

        'purge schedule first
        PurgeSchedule()

        ' delete resources
        Dim vResource = (From d In vContext.resources Select d).ToList
        For Each x In vResource
            For Each y In x.classgroups.ToList
                x.classgroups.Remove(y)
            Next
            For Each y In x.resourcepreferredvenues.ToList
                x.resourcepreferredvenues.Remove(y)
            Next
            vContext.resources.DeleteObject(x)
        Next
        vContext.SaveChanges()

        ' delete lecturer
        Dim vLecturer = (From d In vContext.lecturers Select d).ToList
        For Each x In vLecturer
            For Each y In x.lecturersiteclusteravailabilities.ToList
                x.lecturersiteclusteravailabilities.Remove(y)
            Next
            For Each y In x.subjects.ToList
                x.subjects.Remove(y)
            Next
            For Each y In x.classgroups.ToList
                x.classgroups.Remove(y)
            Next
            vContext.lecturers.DeleteObject(x)
        Next
        vContext.SaveChanges()

        ' delete class
        Dim vClass = (From d In vContext.classgroups Select d).ToList
        For Each x In vClass
            vContext.classgroups.DeleteObject(x)
        Next
        vContext.SaveChanges()

        ' delete subject associations
        Dim vsubject = (From d In vContext.subjects Select d).ToList
        For Each x In vsubject
            For Each y In x.siteclustersubjects.ToList
                x.siteclustersubjects.Remove(y)
            Next
            For Each y In x.programmesubjects.ToList
                x.programmesubjects.Remove(y)
            Next
        Next
        vContext.SaveChanges()

        ' delete qual associations
        Dim vqual = (From d In vContext.qualifications Select d).ToList
        For Each x In vqual
            For Each y In x.siteclusters.ToList
                x.siteclusters.Remove(y)
            Next
        Next
        vContext.SaveChanges()

    End Sub


    Private Sub PurgeSchedule()
        ' delete resourceschedules
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vResSche = (From d In vContext.resourceschedules Select d).ToList
        For Each x In vResSche
            Dim xvenue = x.venues.FirstOrDefault
            Do While Not IsNothing(x.venues.FirstOrDefault)
                x.venues.Remove(xvenue)
                xvenue = x.venues.FirstOrDefault
            Loop
            vContext.resourceschedules.DeleteObject(x)
        Next
        vContext.SaveChanges()
    End Sub

    Private Sub btnPurge_Click(sender As Object, e As System.EventArgs) Handles btnPurge.Click
        Try
            PurgeSchedule()
            clsGeneral.logAction(Request.Path, Request.UserHostAddress, "Purge TimeTable Data", Context.User.Identity.Name)
            litMessage.Text = clsGeneral.displaymessage("Data Purged!!", False)
        Catch ex As Exception
            litMessage.Text = clsGeneral.displaymessage(ex.Message, True)
        End Try
    End Sub
End Class