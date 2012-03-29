<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Flex.Master" CodeBehind="summary.aspx.vb" Inherits="FlexTimeTable.summary" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<h2>Summary</h2>
    <asp:Literal ID="errormessage" runat="server"></asp:Literal><br />
    Year:<asp:DropDownList ID="cboYear" runat="server">
    </asp:DropDownList>&nbsp;&nbsp;
    Academic Block:<asp:DropDownList ID="cboBlock" runat="server">
    </asp:DropDownList>
    <asp:Button ID="btnReport" runat="server" Text="Get Report" />
    <asp:Panel ID="pnlReport" Width="762px" BorderWidth="1px" ScrollBars="Vertical" runat="server" 
        Height="468px">
        <asp:Literal ID="litReport" runat="server"></asp:Literal>
    </asp:Panel>
</asp:Content>
