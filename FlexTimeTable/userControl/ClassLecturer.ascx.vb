Public Class ClassLecturer
    Inherits System.Web.UI.UserControl

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

    Public Property mSubjectID() As Integer
        Set(ByVal value As Integer)
            ViewState("subjectid") = CStr(value)
        End Set
        Get
            Return CInt(ViewState("subjectid"))
        End Get
    End Property

    Public Property mClassID() As Integer
        Set(ByVal value As Integer)
            ViewState("classid") = CStr(value)
        End Set
        Get
            Return CInt(ViewState("classid"))
        End Get
    End Property

    Public ReadOnly Property LecturerID() As Integer
        Get
            Return LecturerIDfromLabel()
        End Get
    End Property

    Private Function LecturerIDfromLabel() As Integer
        Try
            Return CInt(lblLecturer.ToolTip)
        Catch ex As Exception
            Return 0
        End Try
    End Function

    Public Sub LoadLecturer()
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vSubject = (From p In vContext.subjects
                                   Where p.ID = mSubjectID
                                          Select p).FirstOrDefault
        '''''''
        Try
            cboLecturer.DataSource = (From p In vSubject.lecturers Select name = (p.officer.Surname + "," + p.officer.FirstName), ID = p.LecturerID)
        Catch ex As Exception
            cboLecturer.DataSource = Nothing
        End Try
        cboLecturer.DataTextField = "name"
        cboLecturer.DataValueField = "ID"
        cboLecturer.DataBind()
        Try
            Dim vClass = (From p In vContext.classgroups Where p.ID = mClassID Select p).FirstOrDefault
            setLecturer(vClass.lecturer)
        Catch ex As Exception
            setLecturer(0, "")
        End Try

    End Sub

    Sub setLecturer(ByVal vID As Integer, ByVal vName As String)
        If vID > 0 Then
            lblLecturer.Text = vName
            lblLecturer.ToolTip = vID.ToString
        Else
            lblLecturer.Text = "Unassigned"
            lblLecturer.ToolTip = ""
        End If
    End Sub

    Sub setLecturer(ByRef x As lecturer)
        If Not IsNothing(x) Then
            setLecturer(x.LecturerID, x.officer.Surname + "," + x.officer.FirstName)
        Else
            setLecturer(0, "")
        End If
    End Sub

    Public Sub SaveLecturer()
        Try
            Dim NewlecturerID As Integer = LecturerIDfromLabel()
            Dim vContext As timetableEntities = New timetableEntities()
            Dim vClass = (From p In vContext.classgroups Where p.ID = mClassID Select p).First
            Dim OldLecturerID As Integer
            Try
                OldLecturerID = vClass.lecturer.LecturerID
            Catch ex As Exception
                OldLecturerID = 0
            End Try
            ''''get existing lecturer attached to class
            Dim vOldLecturer = (From p In vContext.lecturers Where p.LecturerID = OldLecturerID Select p).FirstOrDefault
            Dim vNewLecturer = (From p In vContext.lecturers Where p.LecturerID = NewlecturerID Select p).FirstOrDefault
            If Not IsNothing(vOldLecturer) AndAlso OldLecturerID <> NewlecturerID Then
                vOldLecturer.classgroups.Remove(vClass)
            End If
            If OldLecturerID <> NewlecturerID Then
                vNewLecturer.classgroups.Add(vClass)
            End If
            vContext.SaveChanges()
        Catch ex As Exception
        End Try
    End Sub

    Protected Sub btnLecturer_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnLecturer.Click
        setLecturer(CInt(cboLecturer.SelectedItem.Value), cboLecturer.SelectedItem.Text)
        SaveLecturer()
    End Sub

End Class