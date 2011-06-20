Imports System.Net
Imports System.DirectoryServices
Imports System.DirectoryServices.Protocols
'Imports System.Security.Cryptography.X509Certificates
Public Class ldap
    Inherits System.Web.UI.UserControl

    ' Public Class WebUserAccess
    Private mLdapServer As String = ConfigurationManager.AppSettings("LdapServer")
    Private mAdminDN As String = ConfigurationManager.AppSettings("LdapAdminDN")
    Private mAdminPwd As String = ConfigurationManager.AppSettings("LdapAdminPwd")
    Private mBaseDN As String = ConfigurationManager.AppSettings("LdapBaseDN")
    Private mPort As Integer = CInt(ConfigurationManager.AppSettings("LdapPort"))
    Private mSecurity As Boolean = CBool(ConfigurationManager.AppSettings("LdapSSL"))
    Private mVersion As Integer = CInt(ConfigurationManager.AppSettings("LdapVersion"))
    Private mLdapResponse As SearchResponse


    Public Function BindLdap(ByVal distinquishedName As String, ByVal password As String) As LdapConnection
        Dim vConn As New LdapConnection(New LdapDirectoryIdentifier(mLdapServer, mPort, False, False))
        vConn.SessionOptions.SecureSocketLayer = mSecurity
        vConn.SessionOptions.ProtocolVersion = mVersion
        If IsNothing(distinquishedName) Then
            vConn.AuthType = AuthType.Anonymous
        Else
            vConn.AuthType = AuthType.Basic
            vConn.Credential = New NetworkCredential(distinquishedName, password)
        End If
        Try
            vConn.Bind()
            Return vConn
        Catch ex As LdapException
            Return Nothing
        End Try
    End Function



    Public Function getUserDetails(ByVal username As String) As clsOfficer.sOfficer
        'Dim vConn = BindLdap(mAdminDN, mAdminPwd)
        Dim vConn = BindLdap(Nothing, Nothing)
        ''''set LDAP Search Request
        Dim sr As New SearchRequest(mBaseDN, "cn=" + username, System.DirectoryServices.Protocols.SearchScope.Subtree)
        ''''get LDAP Search Response
        mLdapResponse = CType(vConn.SendRequest(sr), SearchResponse) ' srp is search response
        Try
            ''''get first response entry
            Dim entry As SearchResultEntry = mLdapResponse.Entries(0)
            Return New clsOfficer.sOfficer With {
                .ID = 0,
                .DistinquishedName = entry.DistinguishedName,
                .Email = getLdapValue("mail"),
                .Firstname = getLdapValue("givenname"),
                .Initials = getLdapValue("initials"),
                .Surname = getLdapValue("sn"),
                .username = username}
        Catch ex As ArgumentOutOfRangeException
            Return New clsOfficer.sOfficer With {
                 .id = 0,
                 .DistinquishedName = "",
                 .Email = "",
                 .Firstname = "",
                 .Initials = "",
                 .Surname = "",
                 .username = ""}
        End Try
    End Function


    Private Function getLdapValue(ByVal vKey As String) As String
        Dim srpe As SearchResultEntry, srecol As SearchResultEntryCollection
        Dim srattcol As SearchResultAttributeCollection 'srattrib As System.Collections.DictionaryEntry
        'Dim srattrib As System.DirectoryServices.Protocols.DirectoryAttribute
        Dim DE As DictionaryEntry, vals() As String
        Dim key As String, strToDisplay As String = ""

        srecol = mLdapResponse.Entries 'srpecol is search result entries collection
        For Each srpe In srecol ' srpe is SearchResultEntry
            srattcol = srpe.Attributes  'srattcol is a  SearchResultAttributeCollection
            For Each DE In srattcol
                key = DE.Key.ToString
                vals = DirectCast(srpe.Attributes(key).GetValues(GetType(String)), String())
                If vKey = key Then
                    Return vals(0)
                End If
            Next
        Next
        Return String.Empty
    End Function
    'End Class

End Class