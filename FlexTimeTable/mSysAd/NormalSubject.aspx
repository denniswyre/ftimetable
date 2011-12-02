<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Flex.Master" CodeBehind="NormalSubject.aspx.vb" Inherits="FlexTimeTable.NormalSubject" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<h3>Normalize Subjects</h3>
    <asp:Button ID="btnNormize" runat="server" Text="Normalize Subject Names" /><br /><br /><br />
    Enter Text:
    <asp:TextBox ID="txtValue" runat="server" Width="400px"></asp:TextBox>
     <asp:Button ID="btnTest" runat="server" Text="test" />
     <br />
    <asp:Literal ID="litResult" runat="server"></asp:Literal>
</asp:Content>
