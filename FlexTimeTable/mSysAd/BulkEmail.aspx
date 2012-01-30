<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Flex.Master" CodeBehind="BulkEmail.aspx.vb" Inherits="FlexTimeTable.BulkEmail" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<h2>
        Bulk Email
    </h2>
    <asp:Literal ID="ErrorMessage" runat="server"></asp:Literal><br />
    Subject:<asp:TextBox ID="txtSubject" runat="server" Width="305px"></asp:TextBox>
    <asp:Button ID="btnSend" runat="server" Text="Send" Width="98px" /><br /><br />
    <asp:TextBox ID="txtBody" runat="server" TextMode="MultiLine" Height="228px" 
        Width="461px"></asp:TextBox>

</asp:Content>
