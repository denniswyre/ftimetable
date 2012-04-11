<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Flex.Master"
    CodeBehind="ClassSummary.aspx.vb" Inherits="FlexTimeTable.ClassSummary" %>

<%@ Register Src="../userControl/getDepartment.ascx" TagName="getDepartment" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h3>
        Class Summary</h3>
    <div>
        <asp:Literal ID="litMessage" runat="server"></asp:Literal>
        <br />
        Campus:<asp:DropDownList ID="cboCampus" AutoPostBack="true" runat="server"> </asp:DropDownList>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
        Academic Block:<asp:DropDownList ID="cboBlock" AutoPostBack="true" runat="server">
        </asp:DropDownList>
        <uc1:getDepartment ID="ucDepartment" runat="server" />
        <asp:Literal ID="litSummary" runat="server"></asp:Literal><br />
        <asp:GridView ID="grdClasses" runat="server">
        </asp:GridView>
        <br />
        <br />
    </div>
</asp:Content>
