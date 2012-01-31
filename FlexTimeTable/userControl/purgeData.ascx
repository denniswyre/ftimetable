<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="purgeData.ascx.vb" Inherits="FlexTimeTable.purgeData" %>


<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>


<asp:Panel ID="Panel1" GroupingText="Purge TimeTable" runat="server">
    <asp:Literal ID="litMessage" runat="server"></asp:Literal>
<p>This is a dangerous operation. All TimeTable Data will be lost.</p>
    <asp:Button ID="btnPurge" runat="server" Text="Press to Purge Data" />
    <asp:ConfirmButtonExtender ID="btnPurge_ConfirmButtonExtender" runat="server" 
        ConfirmText="Are you sure?" Enabled="True" TargetControlID="btnPurge">
    </asp:ConfirmButtonExtender>
</asp:Panel>
