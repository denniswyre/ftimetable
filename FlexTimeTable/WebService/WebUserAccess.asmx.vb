Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.ComponentModel
Imports System.DirectoryServices
Imports System.DirectoryServices.Protocols
Imports System.Security.Cryptography.X509Certificates
Imports System.Net
Imports Microsoft.VisualBasic

' To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line.
' <System.Web.Script.Services.ScriptService()> _
<System.Web.Services.WebService(Namespace:="http://wsu.ac.za/")> _
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
<ToolboxItem(False)> _
Public Class WebUserAccess
    Inherits System.Web.Services.WebService
    Private mLdapServer As String = ConfigurationManager.AppSettings("LdapServer")
    Private mAdminDN As String = ConfigurationManager.AppSettings("LdapAdminDN")
    Private mAdminPwd As String = ConfigurationManager.AppSettings("LdapAdminPwd")
    Private mLdapResponse As SearchResponse

    Public Structure sLdapOfficer
        Public ID As Integer
        Public DistinquishedName As String
        Public Email As String
        Public username As String
        Public Surname As String
        Public Firstname As String
        Public Initials As String
        Public Title As String
    End Structure

    <WebMethod()> _
    Public Function getUserDetails(ByVal username As String) As sLdapOfficer
        Dim vConn As New LdapConnection(New LdapDirectoryIdentifier(mLdapServer, False, False))
        vConn.AuthType = AuthType.Basic
        vConn.SessionOptions.SecureSocketLayer = False
        vConn.Credential = New NetworkCredential(mAdminDN, mAdminPwd)
        vConn.Bind()
        ''''set LDAP Search Request
        Dim sr As New SearchRequest("o=WSU", "cn=" + username, System.DirectoryServices.Protocols.SearchScope.Subtree)
        ''''get LDAP Search Response
        mLdapResponse = CType(vConn.SendRequest(sr), SearchResponse) ' srp is search response
        Try
            ''''get first response entry
            Dim entry As SearchResultEntry = mLdapResponse.Entries(0)
            Return New sLdapOfficer With {
                .DistinquishedName = entry.DistinguishedName,
                .Email = getLdapValue("mail"),
                .Firstname = getLdapValue("givenname"),
                .Initials = getLdapValue("initials"),
                .Surname = getLdapValue("sn"),
                .username = username}
        Catch ex As ArgumentOutOfRangeException
            Return Nothing
        End Try
    End Function


    <WebMethod()> _
    Public Function authenticateUser(ByVal distinquishedName As String, ByVal password As String) As Boolean
        Dim vConn As New LdapConnection(New LdapDirectoryIdentifier(mLdapServer, False, False))
        vConn.AuthType = AuthType.Basic
        vConn.SessionOptions.SecureSocketLayer = False
        vConn.Credential = New NetworkCredential(distinquishedName, password)
        Try
            vConn.Bind()
            Return True
        Catch ex As LdapException
            Return False
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

End Class