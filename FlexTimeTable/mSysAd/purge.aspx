<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Flex.Master"
    CodeBehind="purge.aspx.vb" Inherits="FlexTimeTable.purge" %>

<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h3>
        Purge Time Table Data</h3>
    <asp:Literal ID="Message" runat="server"></asp:Literal><br />
    <asp:Panel ID="Panel1" runat="server" GroupingText="Purge All Time Table entries">
    <p>Note:This is a dangerous operation.&nbsp;Purging is necessary when you want to 
        generate the Time Table</p>
    <asp:Button ID="btnPurge" runat="server" Text="Purge" />
    <asp:ConfirmButtonExtender ID="btnPurge_ConfirmButtonExtender" runat="server" 
        ConfirmText="Are you sure?" Enabled="True" TargetControlID="btnPurge">
    </asp:ConfirmButtonExtender></asp:Panel>
</asp:Content>
