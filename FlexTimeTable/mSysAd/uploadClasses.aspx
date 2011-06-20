<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Flex.Master"
    CodeBehind="uploadClasses.aspx.vb" Inherits="FlexTimeTable.uploadClasses" %>

<%@ Register Src="../userControl/uploadFile.ascx" TagName="uploadFile" TagPrefix="uc2" %>
<%@ Register Src="../userControl/ldap.ascx" TagName="ldap" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h3>
        Upload CLass Group Data</h3>
    <asp:Panel ID="pnlUpload" runat="server">
        <uc2:uploadFile ID="uploadFile1" runat="server" />
    </asp:Panel>
    <uc1:ldap ID="ldap1" runat="server" />
</asp:Content>
