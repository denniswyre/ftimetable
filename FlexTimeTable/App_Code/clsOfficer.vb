Imports System.Linq
Imports System.Data.Linq
Imports MySql.Data
Imports MySql.Data.MySqlClient

Public Class clsOfficer

    Public Enum eOfficerStatus
        matched = 0
        linked = -1
        unlinked = -2
        unmatched = -3
    End Enum

    Public Structure sOfficerStatus
        Public ID As Integer
        Public Status As eOfficerStatus
    End Structure

    Public Structure sOfficer
        Public ID As Integer
        Public DistinquishedName As String
        Public Email As String
        Public username As String
        Public Surname As String
        Public Firstname As String
        Public Initials As String
        Public Title As String
    End Structure

    Public Shared Function CreateOfficer(ByVal vOfficer As sOfficer) As Integer
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vUser As my_aspnet_users = _
            (From p In vContext.my_aspnet_users _
             Where p.name = vOfficer.username Select p).FirstOrDefault
        If Not IsNothing(vUser) Then
            Dim NewOfficer As New officer
            NewOfficer.FirstName = vOfficer.Firstname
            NewOfficer.Initials = vOfficer.Initials
            NewOfficer.Surname = vOfficer.Surname
            NewOfficer.title = ""
            NewOfficer.my_aspnet_users = vUser
            vContext.officers.AddObject(NewOfficer)
            vContext.SaveChanges()
            Return NewOfficer.ID
        Else
            Return 0
        End If
    End Function

    Public Shared Sub LinkOfficer(ByVal OfficerID As Integer, ByVal Username As String)
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vUser As my_aspnet_users = _
             (From p In vContext.my_aspnet_users _
                    Where p.name = Username Select p).First
        Dim vOfficer As officer = _
             (From p In vContext.officers _
                    Where p.ID = OfficerID Select p).First
        vOfficer.my_aspnet_users = vUser
        vContext.SaveChanges()
    End Sub

    Public Shared Function getOfficer(ByVal username As String) As sOfficer
        Dim vContext As timetableEntities = New timetableEntities()
        Dim dsOfficer As New sOfficer
        Try
            Dim vOfficer As officer = (From p In vContext.officers _
                        Where p.my_aspnet_users.name = username Select p).First
            With dsOfficer
                .Firstname = vOfficer.FirstName
                .Initials = vOfficer.Initials
                .Surname = vOfficer.Surname
                .Title = vOfficer.title
                .ID = vOfficer.ID
                .DistinquishedName = ""
                .username = username
            End With
        Catch ex As System.InvalidOperationException
            With dsOfficer
                .Firstname = ""
                .Initials = ""
                .Surname = ""
                .Title = ""
                .ID = 0
                .DistinquishedName = ""
                .username = ""
            End With
        End Try
        Return dsOfficer
    End Function

    Public Shared Function getOfficerID(ByVal vLdapOfficer As sOfficer) As sOfficerStatus
        Dim vOfficerStatus As New sOfficerStatus
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vOfficer As officer = Nothing
        Try
            vOfficer = (From p In vContext.officers _
                        Where p.my_aspnet_users.name = vLdapOfficer.username Select p).First
            vOfficerStatus.ID = vOfficer.ID
            vOfficerStatus.Status = eOfficerStatus.matched
        Catch NoUserName As Exception
            Try
                vOfficer = _
                 (From p In vContext.officers _
                        Where p.Surname = vLdapOfficer.Surname And _
                              p.FirstName = vLdapOfficer.Firstname And _
                              p.Initials = vLdapOfficer.Initials Select p).First
                Try
                    Dim actualUsername As String = vOfficer.my_aspnet_users.name
                    vOfficerStatus.ID = vOfficer.ID
                    vOfficerStatus.Status = eOfficerStatus.linked
                Catch ex As NullReferenceException
                    vOfficerStatus.ID = vOfficer.ID
                    vOfficerStatus.Status = eOfficerStatus.unlinked
                End Try
            Catch NoMatchingName As System.InvalidOperationException
                vOfficerStatus.ID = 0
                vOfficerStatus.Status = eOfficerStatus.unmatched
            End Try
        End Try
        Return vOfficerStatus
    End Function

    Public Shared Function getOfficer(ByVal ID As Integer) As sOfficer
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vOfficer As officer = _
             (From p In vContext.officers _
                    Where p.ID = ID Select p).First
        Dim dsOfficer As New sOfficer
        With dsOfficer
            .Firstname = vOfficer.FirstName
            .Initials = vOfficer.Initials
            .Surname = vOfficer.Surname
            .Title = vOfficer.title
            .ID = ID
            .DistinquishedName = ""
            Try
                .username = vOfficer.my_aspnet_users.name
            Catch ex As NullReferenceException
                .username = ""
            End Try
        End With
        Return dsOfficer
    End Function

End Class

