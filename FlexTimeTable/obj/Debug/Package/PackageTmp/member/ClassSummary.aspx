<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Flex.Master"
    CodeBehind="ClassSummary.aspx.vb" Inherits="FlexTimeTable.ClassSummary" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h3>
        Class Summary</h3>
    <div>
        <asp:Literal ID="litMessage" runat="server"></asp:Literal>
        <br />
        Faculty:<asp:DropDownList ID="cboFaculty" AutoPostBack="true" runat="server">
        </asp:DropDownList>
        <br />
        Department:
        <asp:DropDownList ID="cboDepartments" AutoPostBack="true" runat="server">
        </asp:DropDownList>
        <br />
        <asp:Literal ID="litSummary" runat="server"></asp:Literal><br />
        <asp:GridView ID="grdClasses" runat="server">
        </asp:GridView>
        <br />
        <br />
    </div>
</asp:Content>
