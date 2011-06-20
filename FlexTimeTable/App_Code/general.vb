Public Class clsGeneral
    Public Shared Function getRandomNumber(ByVal lowerbound As Integer, ByVal upperbound As Integer) As Integer
        Randomize()
        Return CInt(Int((upperbound - lowerbound + 1) * Rnd() + lowerbound))
    End Function

    Public Shared Function getRandomStr(ByVal vLen As Integer) As String
        Dim VerificationStr As String = ""
        Dim i As Integer
        For i = 0 To vLen
            VerificationStr += Chr(getRandomNumber(65, 90))
        Next
        Return VerificationStr
    End Function

    Public Shared Function displaymessage(ByVal vtext As String, ByVal isErrorMessage As Boolean) As String
        If isErrorMessage Then
            Return "<span class=""errormessage"">" + vtext + "</span>"
        Else
            Return "<span class=""okmessage"">" + vtext + "</span>"
        End If
    End Function


    Public Shared Function correctSubjectLevel(ByVal oldlevel As String) As Integer
        Try
            Return CInt(oldlevel)
        Catch ex As Exception
            Return 1
        End Try
    End Function

    Public Shared Function correctCode(ByVal oldcode As String) As String
        Dim newcode As String = ""
        For Each i As Char In Trim(oldcode)
            Select Case Asc(i)
                Case 32 'space
                Case 48 To 57  'numbers
                    newcode = newcode + i
                Case 65 To 90 'uppercase letters
                    newcode = newcode + i
                Case 97 To 122  'lowercase letters
                    newcode = newcode + UCase(i)
                Case Else
                    newcode = newcode + "_"
            End Select
        Next
        Return newcode
    End Function

End Class
