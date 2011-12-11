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


    Public Shared Function getOldStr(ByVal subjectid As Integer) As String
        Dim vContext As timetableEntities = New timetableEntities()
        Dim oldcodes = (From p In vContext.oldsubjectcodes Where p.SubjectID = subjectid Select p).ToList
        If oldcodes.Count = 0 Then
            Return ""
        End If
        Dim oStr As String = ""
        For Each ox In oldcodes
            If oStr = "" Then
                oStr = " -->" + ox.OldCode
            Else
                oStr = oStr + ", " + ox.OldCode
            End If
        Next
        Return oStr
    End Function


    Public Shared Function correctSubjectLevel(ByVal oldlevel As String) As Integer
        Try
            Return CInt(oldlevel)
        Catch ex As Exception
            Return 1
        End Try
    End Function

    Public Shared Function correctCode(ByVal oldcode As String) As String
        Dim xCode As String = ""
        For Each i As Char In Trim(oldcode)
            Select Case Asc(i)
                Case 32 'space
                Case 48 To 57  'numbers
                    xCode = xCode + i
                Case 65 To 90 'uppercase letters
                    xCode = xCode + i
                Case 97 To 122  'lowercase letters
                    xCode = xCode + UCase(i)
                Case Else
                    xCode = xCode + "_"
            End Select
        Next
        Return xCode
    End Function


    Public Shared Function DisplayOldCode(ByVal oldcode As String) As String
        Dim xCode As String = ""
        For Each i As Char In Trim(oldcode)
            Select Case i
                Case CChar("_")
                    xCode = xCode + "/"
                Case Else
                    xCode = xCode + i
            End Select
        Next
        Return xCode
    End Function


    Public Shared Function NormalizeString(ByVal oldStr As String) As String
        Dim xStr As String = ""
        For Each i As Char In Trim(oldStr)
            Select Case Asc(i)
                Case 32 'space
                    ''''''if previous char is a space remove it
                    Dim vlen = xStr.Length
                    If vlen > 0 Then
                        If Asc(CChar(Right(xStr, 1))) <> 32 Then
                            xStr = xStr + i
                        End If
                    End If
                Case 34
                Case 48 To 57  'numbers
                    xStr = xStr + i
                Case 65 To 90 'uppercase letters
                    xStr = xStr + i
                Case Asc(","), Asc("&"), Asc(".")
                    xStr = xStr + i
                Case 97 To 122  'lowercase letters
                    xStr = xStr + UCase(i)
                Case Else
                    If xStr.Length > 0 Then
                        xStr = xStr + "_"
                    End If
            End Select
        Next
        Return xStr
    End Function


    Public Shared Function createusername(ByVal firstname As String, ByVal surname As String) As String
        Dim username As String = LCase(Left(Trim(firstname), 1))
        For Each i As Char In Trim(surname)
            Select Case Asc(i)
                Case 48 To 57  'numbers
                    username = username + i
                Case 65 To 90 'uppercase letters
                    username = username + LCase(i)
                Case 97 To 122  'lowercase letters
                    username = username + i
                Case Else
            End Select
        Next
        Return username
    End Function


End Class
