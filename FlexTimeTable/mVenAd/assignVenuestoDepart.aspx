<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Flex.Master"
    CodeBehind="assignVenuestoDepart.aspx.vb" Inherits="FlexTimeTable.assignVenuestoDepart" %>

<%@ Register Src="~/userControl/logButton.ascx" TagName="logButton" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h3>
        Assign Venues</h3>
    <asp:Label ID="lblMessage" runat="server" Text=""></asp:Label><br />
    <asp:Literal ID="litCluster" runat="server"></asp:Literal>
    <asp:DropDownList ID="cboCluster" AutoPostBack="true" runat="server">
    </asp:DropDownList>
    <asp:DropDownList ID="cboCampus" AutoPostBack="true" runat="server">
    </asp:DropDownList>
    <br />
    <br />
    Department:<asp:DropDownList ID="cboDepartment" runat="server" 
    AutoPostBack="True">
    </asp:DropDownList>
       <br />
    <table><tr><td>
    Available Rooms<br />
     <asp:ListBox ID="lstVenues" runat="server" Width="300px"></asp:ListBox></td><td>
    <asp:Button ID="btnAdd" runat="server" Text="Add"  Width="80px" /><br />
     <asp:Button ID="btnDelete" runat="server" Text="Remove"  Width="80px"/></td><td>
    Assigned Rooms<br />
    <asp:ListBox ID="lstDepartVenues" runat="server" Width="300px"></asp:ListBox>
   </td></tr></table>
   
    </asp:Content>
