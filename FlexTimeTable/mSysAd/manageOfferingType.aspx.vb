Public Class manageOfferingType
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            loadTimeSlots()
            loadOfferingType()
            btnSave.Text = "Save"
            btnDelete.Text = "Delete"
            Me.pnlAction.Visible = False
        End If
    End Sub


    Sub loadTimeSlots()
        Dim vContext As timetableEntities = New timetableEntities()
        Dim TimeList = (From p In vContext.timeslots Order By p.StartTime Select p).ToList
        For Each vtimeslot As timeslot In TimeList
            With vtimeslot
                Dim vStartItem As New ListItem(DisplayTime(.ID, 0), .ID.ToString)
                Dim vEndItem As New ListItem(DisplayTime(.ID, 1), .ID.ToString)
                cboStartTime.Items.Add(vStartItem)
                cboEndTime.Items.Add(vEndItem)
                cboSabStart.Items.Add(vStartItem)
                cboSabEnd.Items.Add(vEndItem)
            End With
        Next
    End Sub

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

    Sub loadOfferingType()
        Dim vContext As timetableEntities = New timetableEntities()
        grdOffering.DataSource = (From p In vContext.offeringtypes
                                    Order By p.ID
                                      Select p)
        Me.grdOffering.DataBind()
    End Sub

    Private Sub grdOffering_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles grdOffering.SelectedIndexChanged
        ''populate field
        Dim vID As Integer = CType(grdOffering.SelectedRow.Cells(0).Text, Integer)
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vOfferingType As offeringtype = (From p In vContext.offeringtypes Where p.ID = vID Select p).FirstOrDefault
        With vOfferingType
            lblID.Text = .ID.ToString
            txtCode.Text = .code
            txtName.Text = .name.ToString
            chkSabbathClasses.Checked = .SabbathClasses
            cboStartTime.SelectedValue = .timeslot.ID.ToString
            cboEndTime.SelectedValue = .timeslot1.ID.ToString
            cboSabStart.SelectedValue = .timeslot2.ID.ToString
            cboSabEnd.SelectedValue = .timeslot3.ID.ToString
        End With
        changeMode(eMode.edit)
        litMessage.Text = ""
    End Sub

    Enum eMode
        edit
        delete
        create
    End Enum

    Sub changeMode(ByVal vMode As eMode)
        Me.pnlAction.Visible = True
        Select Case vMode
            Case eMode.edit
                Me.btnDelete.Visible = True
                Me.btnSave.Visible = True
                btnSave.Text = "Update"
                Me.pnlAction.GroupingText = "Edit OfferingType"
            Case eMode.create
                lblID.Text = ""
                Me.txtCode.Text = ""
                Me.txtName.Text = ""
                Me.btnDelete.Visible = False
                Me.btnSave.Visible = True
                btnSave.Text = "Save"
                Me.pnlAction.GroupingText = "Site OfferingType"
        End Select
    End Sub

    Function generateNewID() As Integer
        If grdOffering.Rows.Count = 0 Then
            Return 1
        Else
            Return CInt(grdOffering.Rows(grdOffering.Rows.Count - 1).Cells(0).Text) + 1
        End If
    End Function

    Private Sub lnkCreate_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles lnkCreate.Click
        changeMode(eMode.create)
        litMessage.Text = ""
    End Sub

    Protected Sub btnCancel_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnCancel.Click
        Me.pnlAction.Visible = False
        litMessage.Text = ""
    End Sub

    Sub verifyvalues()
        If Trim(txtCode.Text) = "" Then
            Throw New Exception("Code cannot be blank")
        End If
        If Trim(txtName.Text) = "" Then
            Throw New Exception("Name cannot be blank")
        End If
        If DateDiff(DateInterval.Minute, _
                     CDate(cboStartTime.SelectedItem.Text), _
                     CDate(cboEndTime.SelectedItem.Text)) < 0 Then
            Throw New Exception("End Time must be greater than Start Time")
        End If
        If chkSabbathClasses.Checked AndAlso _
             DateDiff(DateInterval.Minute, _
                       CDate(cboSabStart.SelectedItem.Text), _
                       CDate(cboSabEnd.SelectedItem.Text)) < 0 Then
            Throw New Exception("Saturday End Time must be greater than Start Time")
        End If
    End Sub

    Protected Sub CreateOfferingType()
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vOfferingType = New offeringtype With {
            .ID = generateNewID(),
            .code = Me.txtCode.Text,
            .name = txtName.Text,
            .SabbathClasses = chkSabbathClasses.Checked,
            .startTimeSlot = CInt(cboStartTime.SelectedValue),
             .endTimeSlot = CInt(cboEndTime.SelectedValue),
             .sabStartTimeSlot = CInt(cboSabStart.SelectedValue),
             .sabEndTimeSlot = CInt(cboSabEnd.SelectedValue)}
        vContext.offeringtypes.AddObject(vOfferingType)
        vContext.SaveChanges()
    End Sub

    Protected Sub UpdateOfferingType()
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vOfferingType As offeringtype = _
            (From p In vContext.offeringtypes _
                Where p.ID = CInt(lblID.Text) _
                    Select p).First
        With vOfferingType
            .code = Me.txtCode.Text
            .name = txtName.Text
            .SabbathClasses = chkSabbathClasses.Checked
            .startTimeSlot = CInt(cboStartTime.SelectedValue)
            .endTimeSlot = CInt(cboEndTime.SelectedValue)
            .sabStartTimeSlot = CInt(cboSabStart.SelectedValue)
            .sabEndTimeSlot = CInt(cboSabEnd.SelectedValue)
        End With
        vContext.SaveChanges()
    End Sub

    Protected Sub DeleteOfferingType()
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vOfferingType = (From p In vContext.offeringtypes _
                            Where p.ID = CInt(Me.lblID.Text) _
                            Select p).First
        vContext.DeleteObject(vOfferingType)
        vContext.SaveChanges()
    End Sub

    Private Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
        Try
            verifyvalues()
            If Me.lblID.Text = "" Then
                CreateOfferingType()
            Else
                UpdateOfferingType()
            End If
            litMessage.Text = ""
            loadOfferingType()
            Me.pnlAction.Visible = False
        Catch ex As Exception
            litMessage.Text = ex.Message
        End Try
    End Sub

    Private Sub btnDelete_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnDelete.Click
        Try
            DeleteOfferingType()
            loadOfferingType()
            Me.pnlAction.Visible = False
            litMessage.Text = ""
        Catch ex As Exception
            litMessage.Text = ex.Message
        End Try
    End Sub


End Class