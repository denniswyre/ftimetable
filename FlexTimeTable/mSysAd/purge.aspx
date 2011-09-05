<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Flex.Master"
    CodeBehind="purge.aspx.vb" Inherits="FlexTimeTable.purge" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h3>
        Purge Data</h3>
    <asp:RadioButtonList ID="optOption" runat="server">
        <asp:ListItem>Sites</asp:ListItem>
        <asp:ListItem>Academic</asp:ListItem>
        <asp:ListItem>Class</asp:ListItem>
        <asp:ListItem>TimeTable</asp:ListItem>
    </asp:RadioButtonList>
    <asp:Literal ID="Message" runat="server"></asp:Literal><br />
    <asp:CheckBox ID="CheckBox1" runat="server" Text="Are you sure you want to purge the data?" />
    <p>Note:This is a dangerous operation.&nbsp;Purging is necessary when you want to upload
    fresh data</p>
    <asp:Button ID="btnPurge" runat="server" Text="Purge" />
</asp:Content>
