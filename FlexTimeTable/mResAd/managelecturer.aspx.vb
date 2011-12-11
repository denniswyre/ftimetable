﻿Imports System.Net
Imports System.DirectoryServices
Imports System.DirectoryServices.Protocols
Public Class managelecturer
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            getDepartment1.loadFaculty(User.Identity.Name)
            loadRosterVariables()
            setcontrol(True)
        End If
    End Sub

#Region "general"


    Sub loadlecturers()
        Dim vContext As timetableEntities = New timetableEntities()
        Dim DepartmentID = getDepartment1.getID
        Dim vLecturers = (From p In vContext.lecturers
                            Order By p.officer.Surname, p.officer.FirstName
                                    Where p.DepartmentID = DepartmentID
                                       Select surname = p.officer.Surname,
                                              firstname = p.officer.FirstName,
                                              initials = p.officer.Initials,
                                              title = p.officer.title,
                                              username = p.officer.my_aspnet_users.name,
                                              id = p.LecturerID).ToList
        pnlLecturers.GroupingText = "Lecturers" '+ cboDepartments.SelectedItem.Text
        grdLecturers.DataSource = vLecturers
        grdLecturers.DataBind()
        pnlgetLecturer.Enabled = True
        loadLecturerDetails()
    End Sub



    Sub loadSearchSubjects()
        Dim vContext As timetableEntities = New timetableEntities()
        Dim SearchSubjects = (From p In vContext.subjects Where p.longName.Contains(txtSubjectSearch.Text) Select p)
        With lstAvailableSubjects
            .Items.Clear()
            For Each x In SearchSubjects
                Dim ostr = clsGeneral.getOldStr(x.ID)
                .Items.Add(New ListItem(x.longName + " [" + x.Code + "]" + ostr, CStr(x.ID)))
            Next
        End With
    End Sub


    Sub loadLecturerDetails()
        Dim Lecturername As String = ""
        Try
            Dim LecturerID As Integer = CInt(ViewState("lecturerID"))
            Dim vContext As timetableEntities = New timetableEntities()
            Dim vlecturer = (From p In vContext.lecturers Where p.LecturerID = LecturerID Select p).FirstOrDefault
            ''  list selected subject
            Lecturername = vlecturer.officer.Surname + "," + _
                         vlecturer.officer.FirstName + " " + _
                         vlecturer.officer.Initials
            'list subjects
            loadSelectedsubjects(vlecturer.subjects.ToList)
            'list roster
            grdRoster.DataSource = (From p In vlecturer.lecturersiteclusteravailabilities
                                        Order By p.DayOfWeek, p.timeslot.StartTime
                                        Select clusterID = p.SiteClusterID,
                                          ClusterName = p.sitecluster.longName,
                                          Day = p.DayOfWeek,
                                          startid = p.StartTimeSlot,
                                          endid = p.EndTimeSlot)
            grdRoster.DataBind()
        Catch ex As Exception
            lstSelectedSubjects.DataSource = Nothing
            lstSelectedSubjects.DataBind()
            grdRoster.DataSource = Nothing
            grdRoster.DataBind()
        End Try
        ''headers
    End Sub

    Sub loadSelectedsubjects(ByVal vSubjects As List(Of subject))
        Dim vContext As timetableEntities = New timetableEntities()
        With lstSelectedSubjects
            .Items.Clear()
            For Each x In vSubjects
                Dim ostr = clsGeneral.getOldStr(x.ID)
                Dim vItem As New ListItem(x.longName + " [" + x.Code + "]" + ostr, CStr(x.ID))
                .Items.Add(vItem)
            Next
        End With
    End Sub


#End Region

#Region "lecturer"
    Protected Sub btnVerify_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnGet.Click
        litErrorMessage.Text = ""
        Try
            If (txtUsername.Text).Trim().Length = 0 Then
                litErrorMessage.Text = clsGeneral.displaymessage("You must enter a username in the textbox.", True)
                Exit Sub
            End If

            Dim vOfficer As clsOfficer.sOfficer = ldap1.getUserDetails(txtUsername.Text)
            If vOfficer.username = "" Then
                Throw New Exception("lecturer not found!")
            End If

            Session("officer") = vOfficer
            With vOfficer
                lblproposedlectuer.Text = String.Format("Name:{0} {1} {2},  email:{3}, Username:{4}", .Firstname, .Initials, .Surname, .Email, .username)
            End With
            setcontrol(False)
        Catch ex As Exception
            litErrorMessage.Text = clsGeneral.displaymessage(ex.Message, True)
        End Try
    End Sub

    Sub setcontrol(ByVal vNormal As Boolean)
        lblUsername.Visible = vNormal
        lblproposedlectuer.Visible = Not vNormal
        btnGet.Visible = vNormal
        btnAddLecturer.Visible = Not vNormal
        btnCancelLecturer.Enabled = Not vNormal
        txtUsername.Visible = vNormal
    End Sub

    Protected Sub btnAddLecturer_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnAddLecturer.Click
        litErrorMessage.Text = ""
        Try
            'get Ldap user from saved session
            Dim vLdapUser As clsOfficer.sOfficer = CType(Session("officer"), clsOfficer.sOfficer)

            ''''add to membership first
            ' check if the user exists in membership
            Dim userInfo As MembershipUser = Membership.GetUser(vLdapUser.username)
            If userInfo Is Nothing Then
                'create new membership
                Dim dummyPassword As String = clsGeneral.getRandomStr(12)
                userInfo = Membership.CreateUser(txtUsername.Text, dummyPassword, vLdapUser.Email)
                userInfo.IsApproved = True
                Membership.UpdateUser(userInfo)
            End If

            'define role for lecturer
            Dim ModeratorRole As String = "moderator"
            ' add user to role
            If Not Roles.IsUserInRole(vLdapUser.username, ModeratorRole) Then
                Roles.AddUserToRole(vLdapUser.username, ModeratorRole)
            End If

            Dim vReturn As FlexTimeTable.clsOfficer.sOfficerStatus = _
                           FlexTimeTable.clsOfficer.getOfficerID(vLdapUser)

            Select Case vReturn.Status
                Case FlexTimeTable.clsOfficer.eOfficerStatus.matched
                    'nothing to be done. Everything is in order
                Case FlexTimeTable.clsOfficer.eOfficerStatus.unmatched
                    'create officer record
                    vReturn.ID = FlexTimeTable.clsOfficer.CreateOfficer(vLdapUser)
                Case FlexTimeTable.clsOfficer.eOfficerStatus.unlinked
                    ' link record to membership
                    FlexTimeTable.clsOfficer.LinkOfficer(vReturn.ID, vLdapUser.username)
                Case FlexTimeTable.clsOfficer.eOfficerStatus.linked
                    'this is an error. need to alert
                    Throw New Exception("Lecturer might exist with a different username!")
            End Select

            Dim vContext As timetableEntities = New timetableEntities()
            Dim vlecturer As lecturer = (From p In vContext.lecturers Where p.LecturerID = vReturn.ID Select p).SingleOrDefault
            'check if lecturer exists
            If IsNothing(vlecturer) Then
                'create lecturer and assigned to this department
                vlecturer = New lecturer With {.DepartmentID = getDepartment1.getID, .LecturerID = vReturn.ID}
                vContext.lecturers.AddObject(vlecturer)
                vContext.SaveChanges()
            Else
                Dim DummyFacultyID = CType(ConfigurationManager.AppSettings("dummyfaculty"), Integer)
                'check if lecturer already assigned to the correct department
                If vlecturer.DepartmentID = getDepartment1.getID Then
                ElseIf vlecturer.department.school.facultyID = DummyFacultyID Then
                    'if lecturer belongs to dummy faculty move to this department
                    vlecturer.DepartmentID = getDepartment1.getID
                    vContext.SaveChanges()
                Else
                    Throw New Exception("Lecturer belongs to department:" + vlecturer.department.longName + "; school" + vlecturer.department.school.longName + ", " + vlecturer.department.school.faculty.code)
                End If
            End If
            loadlecturers()
        Catch ex As Exception
            litErrorMessage.Text = clsGeneral.displaymessage(ex.Message, True)
        End Try
        setcontrol(True)
    End Sub

    Protected Sub btnCancelLecturer_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnCancelLecturer.Click
        litErrorMessage.Text = ""
        setcontrol(True)
    End Sub


    Private Sub grdLecturers_RowDeleting(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewDeleteEventArgs) Handles grdLecturers.RowDeleting
        Try
            Dim vLecturerID As Integer = CInt(grdLecturers.DataKeys(e.RowIndex).Values(0))
            Dim vContext As timetableEntities = New timetableEntities()
            Dim vlecturer = (From p In vContext.lecturers Where p.LecturerID = vLecturerID Select p).First
            vContext.lecturers.DeleteObject(vlecturer)
            vContext.SaveChanges()
            loadlecturers()
            litErrorMessage.Text = ""
        Catch ex As Exception
            litErrorMessage.Text = clsGeneral.displaymessage(ex.Message, True)
        End Try
        loadLecturerDetails()
    End Sub

    Private Sub grdLecturers_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles grdLecturers.SelectedIndexChanged
        litErrorMessage.Text = ""
        Dim vlecturerID = CInt(grdLecturers.SelectedDataKey.Values(0))
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vlecturer = (From p In vContext.lecturers Where p.LecturerID = vlecturerID Select p).First
        With vlecturer
            litLecturer.Text = "<b>Lecturer:</b><span class=""lecturer"">" + CType(IIf(.officer.title = "", "", .officer.title + "&nbsp;"), String) + .officer.Surname +
                                                             ", " + .officer.FirstName +
                                                             " " + .officer.Initials +
                                                             " [" + .officer.my_aspnet_users.name + "]</span>"
        End With
        ViewState("lecturerID") = vlecturerID
        mvLecturer.SetActiveView(vwDetails)
        loadLecturerDetails()
    End Sub

#End Region

#Region "subject"

    Protected Sub btnAdd_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnAdd.Click
        litErrorMessage.Text = ""
        Try
            Dim LecturerID As Integer = CInt(ViewState("lecturerID"))
            Dim vContext As timetableEntities = New timetableEntities()
            Dim vSubject = (From p In vContext.subjects Where p.ID = CInt(lstAvailableSubjects.SelectedValue) Select p).FirstOrDefault
            Dim vlecturer = (From p In vContext.lecturers Where p.LecturerID = LecturerID Select p).FirstOrDefault
            vlecturer.subjects.Add(vSubject)
            vContext.SaveChanges()
            loadLecturerDetails()
        Catch ex As Exception
            litErrorMessage.Text = clsGeneral.displaymessage(ex.Message, True)
        End Try
    End Sub

    Protected Sub btnRemove_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnRemove.Click
        litErrorMessage.Text = ""
        Try
            Dim LecturerID As Integer = CInt(ViewState("lecturerID"))
            Dim vContext As timetableEntities = New timetableEntities()
            Dim vSubject = (From p In vContext.subjects Where p.ID = CInt(lstSelectedSubjects.SelectedValue) Select p).FirstOrDefault
            Dim vlecturer = (From p In vContext.lecturers Where p.LecturerID = LecturerID Select p).FirstOrDefault
            vlecturer.subjects.Remove(vSubject)
            vContext.SaveChanges()
            loadLecturerDetails()
        Catch ex As Exception
            litErrorMessage.Text = clsGeneral.displaymessage(ex.Message, True)
        End Try
    End Sub

#End Region

#Region "Roster"
    Sub loadRosterVariables()
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vCluster = ""
        With cboCluster
            .DataSource = (From p In vContext.siteclusters Select p).ToList
            .DataTextField = "longName"
            .DataValueField = "ID"
            .DataBind()
        End With
        With cboDay
            For i = 1 To 7
                .Items.Add(WeekdayName(i, False, vbSunday))
            Next
        End With
        For Each timeslot In (From p In vContext.timeslots Order By p.StartTime Select p).ToList
            With timeslot
                cboStart.Items.Add(New ListItem(DisplayTime(.ID, 0), .ID.ToString))
                cboEnd.Items.Add(New ListItem(DisplayTime(.ID, 1), .ID.ToString))
            End With
        Next
    End Sub

    Function displayday(ByVal i As Integer) As String
        Return WeekdayName(i, False, vbSunday)
    End Function

    Function DisplayTime(ByVal TimeSlotID As Integer, ByVal IsEnd As Integer) As String
        Dim vContext As timetableEntities = New timetableEntities()
        Dim TimeSlot = (From p In vContext.timeslots Where p.ID = TimeSlotID Select p).First
        With TimeSlot
            If IsEnd = 0 Then
                Return Format(.StartTime, "HH:mm")
            Else
                Return Format(DateAdd(DateInterval.Minute, .Duration, .StartTime), "HH:mm")
            End If
        End With
    End Function

    Protected Sub btnAddRoster_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnAddRoster.Click
        litErrorMessage.Text = ""
        Try
            If DateDiff(DateInterval.Minute, CDate(cboStart.SelectedItem.Text), CDate(cboEnd.SelectedItem.Text)) <= 0 Then
                Throw New Exception("The end time must be greater than the start time!!!")
            End If
            Dim vContext As timetableEntities = New timetableEntities()
            Dim LecturerID As Integer = CInt(ViewState("lecturerID"))
            Dim vClusterID = CInt(cboCluster.SelectedValue)
            Dim vDay = cboDay.SelectedIndex + 1
            Dim StartID = CInt(cboStart.SelectedValue)
            Dim EndID = CInt(cboEnd.SelectedValue)

            Dim StartSlot = (From p In vContext.timeslots Where p.ID = StartID Select p).FirstOrDefault
            Dim EndSlot = (From p In vContext.timeslots Where p.ID = EndID Select p).FirstOrDefault

            Dim vlecturer = (From p In vContext.lecturers Where p.LecturerID = LecturerID Select p).FirstOrDefault
            Dim vCurrentRoster = (From p In vContext.lecturersiteclusteravailabilities
                        Where p.LecturerID = LecturerID Select p).ToList
            '''''''''check conflicts
            For Each x In vCurrentRoster
                If x.DayOfWeek = vDay And x.SiteClusterID = vClusterID Then
                    Throw New Exception("Site Cluster already exists for the selected day!!")
                End If
                If x.DayOfWeek = vDay And _
                   DateDiff(DateInterval.Minute, x.timeslot.StartTime, StartSlot.StartTime) <= 0 And _
                   DateDiff(DateInterval.Minute, x.timeslot.StartTime, EndSlot.StartTime) >= 0 Then
                    Throw New Exception("There is a time conflict!!")
                End If
                If x.DayOfWeek = vDay And _
                   DateDiff(DateInterval.Minute, x.timeslot1.StartTime, StartSlot.StartTime) <= 0 And _
                   DateDiff(DateInterval.Minute, x.timeslot1.StartTime, EndSlot.StartTime) >= 0 Then
                    Throw New Exception("There is a time conflict!!")
                End If
            Next
            '''''''''''''''''''''''''''

            Dim vRoster = New lecturersiteclusteravailability With {
                        .LecturerID = LecturerID,
                        .SiteClusterID = vClusterID,
                        .DayOfWeek = vDay,
                        .StartTimeSlot = StartID,
                        .EndTimeSlot = EndID}
            vlecturer.lecturersiteclusteravailabilities.Add(vRoster)
            vContext.SaveChanges()
            loadLecturerDetails()
        Catch ex As Exception
            litErrorMessage.Text = clsGeneral.displaymessage(ex.Message, True)
        End Try
        loadLecturerDetails()
    End Sub

    Private Sub grdRoster_RowDeleting(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewDeleteEventArgs) Handles grdRoster.RowDeleting
        Try
            Dim vLecturerID = CInt(ViewState("lecturerID"))
            Dim vClusterID As Integer = CInt(grdRoster.DataKeys(e.RowIndex).Values(0))
            Dim vDay As Integer = CInt(grdRoster.DataKeys(e.RowIndex).Values(1))
            Dim vStartID As Integer = CInt(grdRoster.DataKeys(e.RowIndex).Values(2))
            Dim vContext As timetableEntities = New timetableEntities()
            Dim vRoster = (From p In vContext.lecturersiteclusteravailabilities
                             Where p.LecturerID = vLecturerID And
                                   p.SiteClusterID = vClusterID And
                                   p.DayOfWeek = vDay And
                                   p.StartTimeSlot = vStartID
                             Select p).First
            vContext.lecturersiteclusteravailabilities.DeleteObject(vRoster)
            vContext.SaveChanges()
            litErrorMessage.Text = ""
        Catch ex As Exception
            litErrorMessage.Text = clsGeneral.displaymessage(ex.Message, True)
        End Try
        loadLecturerDetails()
    End Sub

#End Region

    Private Sub lnkReturn_Click(sender As Object, e As System.EventArgs) Handles lnkReturn.Click
        mvLecturer.SetActiveView(vwSelect)
    End Sub

    Private Sub getDepartment1_DepartmentClick(E As Object, Args As clsDepartmentEvent) Handles getDepartment1.DepartmentClick
        litErrorMessage.Text = ""
        mvLecturer.SetActiveView(vwSelect)
        loadlecturers()
        lstAvailableSubjects.Items.Clear()
    End Sub

    Private Sub btnSubjectSearch_Click(sender As Object, e As System.EventArgs) Handles btnSubjectSearch.Click
        loadSearchSubjects()
    End Sub
End Class