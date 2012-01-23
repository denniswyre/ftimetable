Imports System.Net
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

    Sub LoadLecturers()
        Dim vContext As timetableEntities = New timetableEntities()
        Dim DepartmentID = getDepartment1.getID

        'load departmental lecturers
        grdLecturers.DataSource = (From p In vContext.lecturers
                            Order By p.officer.Surname, p.officer.FirstName
                                    Where p.DepartmentID = DepartmentID
                                     Select surname = p.officer.Surname,
                                              firstname = p.officer.FirstName,
                                              initials = p.officer.Initials,
                                              title = p.officer.title,
                                              username = p.officer.my_aspnet_users.name,
                                              id = p.LecturerID).ToList
        grdLecturers.DataBind()

        'load external lecturers who lecture departmental subjects
        Dim UniqueExtLecturers As New List(Of lecturer)

        'loop through all subjects belonging to department
        For Each x In (From p In vContext.subjects Where p.DepartmentID = DepartmentID Select p).ToList
            For Each y In (From q In x.lecturers Where q.DepartmentID <> DepartmentID Select q).ToList
                Dim yLecturer = y.LecturerID
                'ensure that there is no duplicate
                If (From r In UniqueExtLecturers Where r.LecturerID = yLecturer Select r).Count = 0 Then
                    UniqueExtLecturers.Add(y)
                End If
            Next
        Next
        grdExtLecturer.DataSource = (From p In UniqueExtLecturers
                                       Order By p.officer.Surname, p.officer.FirstName
                                              Select surname = p.officer.Surname,
                                                    firstname = p.officer.FirstName,
                                                    initials = p.officer.Initials,
                                                    title = p.officer.title,
                                                    username = p.officer.my_aspnet_users.name,
                                                    id = p.LecturerID).ToList

        grdExtLecturer.DataBind()
        pnlgetLecturer.Enabled = True
    End Sub



    Sub loadSearchSubjects()
        Dim vContext As timetableEntities = New timetableEntities()
        Dim DepartmentID = getDepartment1.getID
        With lstAvailableSubjects
            .Items.Clear()
            For Each x In (From p In vContext.subjects
                                 Where p.DepartmentID = DepartmentID And p.longName.Contains(txtSubjectSearch.Text)
                                   Order By p.longName Select p).ToList
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
            Dim vDepartID = CInt(getDepartment1.getID)
            Dim departSubjects = (From p In vlecturer.subjects Where p.DepartmentID = vDepartID Select p).ToList
            Dim otherSubjects = (From p In vlecturer.subjects Where p.DepartmentID <> vDepartID Select p).ToList
            'list departmental subjects
            loadSelectedsubjects(departSubjects, lstSelectedSubjects.Items)
            'list other subjects
            loadSelectedsubjects(otherSubjects, lstExtSelectedSubjects.Items)
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

    Sub loadSelectedsubjects(ByVal vSubjects As List(Of subject), ByRef vItems As ListItemCollection)
        Dim vContext As timetableEntities = New timetableEntities()
        vItems.Clear()
        For Each x In vSubjects
            Dim ostr = clsGeneral.getOldStr(x.ID)
            Dim vItem As New ListItem(x.longName + " [" + x.Code + "]" + ostr, CStr(x.ID))
            vItems.Add(vItem)
        Next
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

            Dim DummyFacultyID = CType(ConfigurationManager.AppSettings("dummyfaculty"), Integer)
            Dim vContext As timetableEntities = New timetableEntities()
            Dim vlecturer As lecturer = (From p In vContext.lecturers Where p.LecturerID = vReturn.ID Select p).SingleOrDefault
            'create lecturer if it does not exists
            If IsNothing(vlecturer) Then
                'create lecturer and assigned to this department
                vlecturer = New lecturer With {.DepartmentID = getDepartment1.getID, .LecturerID = vReturn.ID}
                vContext.lecturers.AddObject(vlecturer)
                vContext.SaveChanges()
            ElseIf vlecturer.department.school.facultyID = DummyFacultyID Then
                'if lecturer belongs to dummy faculty move to this department
                vlecturer.DepartmentID = getDepartment1.getID
                vContext.SaveChanges()
            End If
            'select lecturer 
            If vlecturer.DepartmentID = getDepartment1.getID Then
                '''''''select int lecturer  
                selectLecturer(vlecturer.LecturerID, False)
            Else
                '''''''select ext lecturer
                selectLecturer(vlecturer.LecturerID, True)
             End If
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
            Dim vlecturer = (From p In vContext.lecturers Where p.LecturerID = vLecturerID Select p).Single
            Dim DummyFacultyID = CType(ConfigurationManager.AppSettings("dummyfaculty"), Integer)
            Dim DummyDepartName = "Dummy Department"
            Dim vDummyDePartID = (From p In vContext.departments Where p.school.facultyID = DummyFacultyID And p.longName = DummyDepartName Select p).Single.ID
            vlecturer.DepartmentID = vDummyDePartID
            vContext.SaveChanges()
            LoadLecturers()
            litErrorMessage.Text = ""
        Catch ex As Exception
            litErrorMessage.Text = clsGeneral.displaymessage(ex.Message, True)
        End Try
    End Sub

    Private Sub grdLecturers_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles grdLecturers.SelectedIndexChanged
        litErrorMessage.Text = ""
        selectLecturer(CInt(grdLecturers.SelectedDataKey.Values(0)), False)
    End Sub


    Sub selectLecturer(ByVal vID As Integer, ByVal IsExternal As Boolean)
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vlecturer = (From p In vContext.lecturers Where p.LecturerID = vID Select p).First
        With vlecturer
            Dim DepartInfo = CStr(IIf(IsExternal, "---->" + .department.longName + "," + .department.school.longName + ", " + vlecturer.department.school.faculty.code, ""))
            litLecturer.Text = "<b>Lecturer:</b><span class=""lecturer"">" +
                                CType(IIf(.officer.title = "", "", .officer.title + "&nbsp;"), String) + .officer.Surname +
                                ", " + .officer.FirstName + " " + .officer.Initials + " [" + .officer.my_aspnet_users.name +
                                DepartInfo + "]</span>"
        End With
        ViewState("lecturerID") = vID
        mvLecturer.SetActiveView(vwDetails)
        pnlRosterButtons.Enabled = Not IsExternal
        grdRoster.Columns(7).Visible = Not IsExternal
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
        LoadLecturers()
        mvLecturer.SetActiveView(vwSelect)
    End Sub

    Private Sub getDepartment1_DepartmentClick(E As Object, Args As clsDepartmentEvent) Handles getDepartment1.DepartmentClick
        litErrorMessage.Text = ""
        mvLecturer.SetActiveView(vwSelect)
        LoadLecturers()
        lstAvailableSubjects.Items.Clear()
    End Sub

    Private Sub btnSubjectSearch_Click(sender As Object, e As System.EventArgs) Handles btnSubjectSearch.Click
        loadSearchSubjects()
    End Sub

    Private Sub grdExtLecturer_RowCommand(sender As Object, e As System.Web.UI.WebControls.GridViewCommandEventArgs) Handles grdExtLecturer.RowCommand
        litErrorMessage.Text = ""
        Dim RowNo = CInt(e.CommandArgument)
        Dim vlecturerID = CInt(grdExtLecturer.DataKeys(RowNo).Value)
        Try
            Select Case LCase(e.CommandName)
                Case "select"
                    selectLecturer(vlecturerID, True)
                Case "transfer"
                    Dim vContext As timetableEntities = New timetableEntities()
                    Dim vlecturer = (From p In vContext.lecturers Where p.LecturerID = vlecturerID Select p).Single
                    Dim DummyFacultyID = CType(ConfigurationManager.AppSettings("dummyfaculty"), Integer)
                    If vlecturer.department.school.facultyID = DummyFacultyID Then
                        'if lecturer belongs to dummy faculty move to this department
                        vlecturer.DepartmentID = getDepartment1.getID
                        vContext.SaveChanges()
                    Else
                        Throw New Exception("Lecturer belongs to department:" + vlecturer.department.longName + "; school" + vlecturer.department.school.longName + ", " + vlecturer.department.school.faculty.code)
                    End If
                    LoadLecturers()
            End Select
        Catch ex As Exception
            litErrorMessage.Text = clsGeneral.displaymessage(ex.Message, True)
        End Try
    End Sub

End Class