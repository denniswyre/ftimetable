Public Class managetimeslot
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            loadTimeslot()
            btnSave.Text = "Save"
            btnDelete.Text = "Delete"
            Me.pnlAction.Visible = False
        End If
    End Sub


    Sub loadTimeslot()
        Dim vContext As timetableEntities = New timetableEntities()
        GrdTimeslot.DataSource = (From p In vContext.timeslots
                                    Order By p.ID
                                      Select p)
        Me.GrdTimeslot.DataBind()
    End Sub

    Private Sub GrdTimeslot_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles GrdTimeslot.SelectedIndexChanged
        ''populate field
        Dim vID As Integer = CType(GrdTimeslot.SelectedRow.Cells(0).Text, Integer)
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vTimeslot As Timeslot = (From p In vContext.Timeslots Where p.ID = vID Select p).FirstOrDefault
        With vTimeslot
            lblID.Text = .ID.ToString
            txtStart.Text = Format(.StartTime, "HH:mm")
            txtDuration.Text = .Duration.ToString
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
                Me.pnlAction.GroupingText = "Edit Timeslot"
            Case eMode.create
                lblID.Text = ""
                Me.txtStart.Text = ""
                Me.txtDuration.Text = ""
                Me.btnDelete.Visible = False
                Me.btnSave.Visible = True
                btnSave.Text = "Save"
                Me.pnlAction.GroupingText = "Site Timeslot"
        End Select
    End Sub

    Function generateNewID() As Integer
        If GrdTimeslot.Rows.Count = 0 Then
            Return 1
        Else
            Return CInt(GrdTimeslot.Rows(GrdTimeslot.Rows.Count - 1).Cells(0).Text) + 1
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

    Function LunchPeriodIsFree(ByVal vID As Integer) As Boolean
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vTimeslot As timeslot
        If vID = 0 Then
            vTimeslot = (From p In vContext.timeslots
                Where p.LunchPeriod = True Select p).FirstOrDefault
        Else
            vTimeslot = (From p In vContext.timeslots _
                Where p.ID <> vID And
                      p.LunchPeriod = True Select p).FirstOrDefault
        End If
        Return IsNothing(vTimeslot)
    End Function

    Protected Sub CreateTimeslot()
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vTimeslot = New timeslot With {
            .ID = generateNewID(),
            .StartTime = CDate(Me.txtStart.Text),
            .Duration = CInt(txtDuration.Text),
            .LunchPeriod = chkLunchPeriod.Checked
           }
        vContext.Timeslots.AddObject(vTimeslot)
        vContext.SaveChanges()
    End Sub

    Protected Sub UpdateTimeslot()
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vTimeslot As timeslot = _
            (From p In vContext.timeslots _
                Where p.ID = CInt(lblID.Text) _
                    Select p).First
        With vTimeslot
            .StartTime = CDate(Me.txtStart.Text)
            .Duration = CInt(txtDuration.Text)
            .LunchPeriod = chkLunchPeriod.Checked
        End With
        vContext.SaveChanges()
    End Sub

    Protected Sub DeleteTimeslot()
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vTimeslot = (From p In vContext.timeslots _
                            Where p.ID = CInt(Me.lblID.Text) _
                            Select p).First
        vContext.DeleteObject(vTimeslot)
        vContext.SaveChanges()
    End Sub

    Private Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
        Dim vID = CInt(IIf(Me.lblID.Text = "", 0, lblID.Text))
        Try
            If Not LunchPeriodIsFree(vID) Then
                Throw New Exception("Lunch Period already exists!!")
            End If
            If vID = 0 Then
                CreateTimeslot()
            Else
                UpdateTimeslot()
            End If
            litMessage.Text = ""
            loadTimeslot()
            Me.pnlAction.Visible = False
        Catch ex As Exception
            litMessage.Text = ex.Message
        End Try
    End Sub

    Private Sub btnDelete_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnDelete.Click
        Try
            DeleteTimeslot()
            loadTimeslot()
            Me.pnlAction.Visible = False
            litMessage.Text = ""
        Catch ex As Exception
            litMessage.Text = ex.Message
        End Try
    End Sub

End Class