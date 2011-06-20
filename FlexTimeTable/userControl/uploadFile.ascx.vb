Public Class uploadFile
    Inherits System.Web.UI.UserControl
    Public Event UploadComplete As EventHandler

    Property Enabled As Boolean

    Public Property filetotable() As clsfiletotableconversion
        Get
            Return CType(Session(" filetotable"), clsfiletotableconversion)
        End Get
        Set(ByVal value As clsfiletotableconversion)
            Session(" filetotable") = value
        End Set
    End Property

    Public WriteOnly Property errorlist() As ListItem
        Set(ByVal value As ListItem)
            lstError.Items.Add(value)
        End Set
    End Property

    Public WriteOnly Property header() As String
        Set(ByVal value As String)
            pnlUpload.GroupingText = value
        End Set
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            WizLoadCVS.ActiveStepIndex = 0
            WizLoadCVS.StepNextButtonText = "Load File"
            WizLoadCVS.WizardSteps.Item(1).AllowReturn = False
        End If
    End Sub

    Public Sub Initialize()
        Dim vfiletotable As clsfiletotableconversion = filetotable
        WizLoadCVS.ActiveStepIndex = 0
        lstError.Items.Clear()
        litFields.Text = vfiletotable.displayMessage()
    End Sub

    Protected Sub WizLoadCVS_NextButtonClick(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.WizardNavigationEventArgs) Handles WizLoadCVS.NextButtonClick
        litMessage.Text = ""
        Try
            Dim vfiletotable As clsfiletotableconversion = filetotable
            Select Case e.CurrentStepIndex
                Case 0 'start
                    '''''upload file to grid
                    UploadFile()
                    With GridView1
                        .DataSource = vfiletotable.pDatatable
                        .DataBind()
                    End With
                    lstError.Items.Clear()
                    WizLoadCVS.StepNextButtonText = "Process"
                Case 1 ' confirm
                    For Each fld As ListItem In vfiletotable.fields
                        If CBool(fld.Value) = False Then
                            Throw New Exception("Field:<b>""" + fld.Text + """</b> does not exists. Ensure that the header is spelled correctly")
                        End If
                    Next
                    RaiseEvent UploadComplete(Me, New System.EventArgs)
                    litMessage.Text = "<p>" + clsGeneral.displaymessage("File has been loaded!", False) + "</p>"
                    WizLoadCVS.StepNextButtonText = "Complete"
            End Select
        Catch ex As Exception
            litMessage.Text = clsGeneral.displaymessage(ex.Message, True) + "<br/>"
            e.Cancel = True
        End Try
    End Sub


    Protected Sub WizLoadCVS_PreviousButtonClick(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.WizardNavigationEventArgs) Handles WizLoadCVS.PreviousButtonClick
        litMessage.Text = ""
        If e.CurrentStepIndex = 1 Then
            WizLoadCVS.StepNextButtonText = "Load File"
        End If
    End Sub

    Private Sub UploadFile()
        Dim path As String = Server.MapPath("~/")
        Dim fileOK As Boolean = False
        Dim fileLen As Integer
        Dim csvString As String = ""
        Dim fileExtension As String
        Dim allowedExtensions As String() = {".txt", ".csv"}
        Dim FileLenMax As Integer = 999999
        Dim vSuccess As Boolean = False
        Dim Input() As Byte

        If Not FileUpload.HasFile Then
            Throw New OverflowException("No Path Selected!!!")
        End If
        ''''''make sure file extension is ok
        fileExtension = System.IO.Path. _
            GetExtension(FileUpload.FileName).ToLower()
        For i As Integer = 0 To allowedExtensions.Length - 1
            If fileExtension = allowedExtensions(i) Then
                fileOK = True
            End If
        Next
        If Not fileOK Then
            Throw New OverflowException("Wrong File Format!!!")
        End If
        '''''''''''''''''''''''''
        ' Get the length of the file.
        fileLen = FileUpload.PostedFile.ContentLength
        If fileLen > FileLenMax Then
            Throw New OverflowException("File is Too Big!!!")
        End If
        ' Create a byte array to hold the contents of the file.
        ReDim Input(fileLen)
        Input = FileUpload.FileBytes
        ' Copy the byte array to a string.
        For loop1 As Integer = 0 To fileLen - 1
            csvString = csvString & Chr(CInt(Input(loop1).ToString()))
        Next loop1

        Dim vfiletotable As clsfiletotableconversion = filetotable
        vfiletotable.cvFileToTable(csvString, Chr(13))
        filetotable = vfiletotable
    End Sub

End Class