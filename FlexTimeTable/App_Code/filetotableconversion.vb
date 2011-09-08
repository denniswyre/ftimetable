Imports System.Data
Public Class clsfiletotableconversion
    Private mfields As ListItemCollection
    Private mDatatable As DataTable

    'table fields
    Public Property fields() As ListItemCollection
        Get
            Return mfields
        End Get
        Set(ByVal value As ListItemCollection)
            mfields = value
        End Set
    End Property

    'uploaded datatable
    Public Property pDatatable() As DataTable
        Get
            Return mDatatable
        End Get
        Set(ByVal value As DataTable)
            mDatatable = value
        End Set
    End Property

    Public Sub New()
        mfields = New ListItemCollection
        mDatatable = New DataTable
    End Sub

    Public Function displayMessage() As String
        Dim fieldstring As String = ""
        Dim j = 0
        For Each fld As ListItem In mfields
            If j = 0 Then
                fieldstring = fld.Text
            ElseIf j Mod 3 = 0 Then
                fieldstring = fieldstring + ",<br/>" + fld.Text
            Else
                fieldstring = fieldstring + ", " + fld.Text
            End If
            j = j + 1
        Next
        Return "<l>Make sure the headers spelled according to the list below:</l><p><b>" + _
                            fieldstring + "</b></p>"
    End Function

    Private Sub SetTableColumns(ByVal LineArray() As String)
        mDatatable = New DataTable
        Dim vFD As Boolean = True
        Dim k As Integer = 0
        For j = 0 To mfields.Count - 1
            mfields.Item(j).Value = CStr(0)
        Next
        For i As Integer = LineArray.GetLowerBound(0) To LineArray.GetUpperBound(0)
            Dim LineField As String = LCase(Trim(LineArray(i)))
            vFD = False
            For j = 0 To mfields.Count - 1
                If LineField = mfields.Item(j).Text And mfields.Item(j).Value = "0" Then
                    mDatatable.Columns.Add(mfields.Item(j).Text)
                    mfields.Item(j).Value = "1"
                    vFD = True
                    Exit For
                End If
            Next
            If Not vFD Then
                k = k + 1
                mDatatable.Columns.Add("Unknown" + k.ToString)
            End If
        Next
    End Sub

    Public Sub cvFileToTable(ByVal csvStr As String, ByVal vDelimiter As Char)
        Dim dr As DataRow
        Dim StrArray() As String = Split(csvStr, vDelimiter, -1)
        Dim LineArray() As String
        Dim CurCell As String = ""

        Dim isFirstRow As Boolean = True
        For j As Integer = StrArray.GetLowerBound(0) To (StrArray.GetUpperBound(0) - 1)
            LineArray = LineToArray(StrArray(j))

            If isFirstRow Then
                SetTableColumns(LineArray)
                isFirstRow = False
            Else
                dr = mDatatable.NewRow()
                For i As Integer = LineArray.GetLowerBound(0) To LineArray.GetUpperBound(0)
                    Try
                        CurCell = Trim(LineArray(i))
                    Catch ex As IndexOutOfRangeException
                        ' insertError("UploadCourse:CSV File Record " + j.ToString, "Column " + (i + 1).ToString + " is Blank!!!")
                    End Try
                    InsertCell(dr, i, CurCell)
                Next
                Try
                    mDatatable.Rows.Add(dr)
                Catch ex As Exception
                End Try
            End If
        Next
    End Sub

    Public Sub cvFileToBigTable(ByVal csvStr As String, ByVal vDelimiter As Char)
        Dim dr As DataRow
        Dim StrArray() As String = Split(csvStr, vDelimiter, -1)
        Dim LineArray() As String
        Dim CurCell As String = ""

        Dim isFirstRow As Boolean = True
        For j As Integer = StrArray.GetLowerBound(0) To (StrArray.GetUpperBound(0) - 1)
            LineArray = LineToArray(StrArray(j))

            If isFirstRow Then
                SetTableColumns(LineArray)
                isFirstRow = False
            Else
                dr = mDatatable.NewRow()
                For i As Integer = LineArray.GetLowerBound(0) To LineArray.GetUpperBound(0)
                    Try
                        CurCell = Trim(LineArray(i))
                    Catch ex As IndexOutOfRangeException
                        ' insertError("UploadCourse:CSV File Record " + j.ToString, "Column " + (i + 1).ToString + " is Blank!!!")
                    End Try
                    InsertCell(dr, i, CurCell)
                Next
                Try
                    mDatatable.Rows.Add(dr)
                Catch ex As Exception
                End Try
            End If
        Next
    End Sub



    Private Shared Function LineToArray(ByVal line As String) As String()
        Dim pattern As String = ",(?=(?:[^\" + Chr(34) + "]*\" + Chr(34) + "[^\" + Chr(34) + "]*\" + Chr(34) + ")*(?![^\" + Chr(34) + "]*\" + Chr(34) + "))"
        Dim r As New Regex(pattern)
        Return r.Split(line)
    End Function

    Private Sub InsertCell(ByRef dr As DataRow, ByVal iCol As Integer, ByVal cellValue As String)
        Try
            With mDatatable
                If .Columns(iCol).DataType Is System.Type.GetType("System.String") Then
                    dr(iCol) = IIf(cellValue Is Nothing, "", cellValue)
                ElseIf .Columns(iCol).DataType Is System.Type.GetType("System.DateTime") Then
                    dr(iCol) = IIf(cellValue Is Nothing, DateTime.UtcNow, Convert.ToDateTime(cellValue))
                ElseIf .Columns(iCol).DataType Is System.Type.GetType("System.Boolean") Then
                    Select Case Trim(UCase(cellValue))
                        Case "YES", "TRUE", "1"
                            dr(iCol) = True
                        Case "NO", "FALSE", "0"
                            dr(iCol) = False
                        Case Else
                            dr(iCol) = False
                    End Select
                Else
                    dr(iCol) = IIf(cellValue Is Nothing, 0, Convert.ToDecimal(cellValue))
                End If
            End With
        Catch ex As Exception
        End Try
    End Sub

End Class
