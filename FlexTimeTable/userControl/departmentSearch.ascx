<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="departmentSearch.ascx.vb"
    Inherits="FlexTimeTable.departmentSearch" %>
<%@ Register Src="~/userControl/logButton.ascx" TagName="logButton" TagPrefix="uc1" %>
<%@ Register Src="getSchool.ascx" TagName="getSchool" TagPrefix="uc2" %>
<asp:Panel ID="Panel1" GroupingText="Browse/Search Department" runat="server">
    <asp:Label ID="lblMessage" runat="server" Text=""></asp:Label>
    <asp:RadioButtonList ID="optSearchType" runat="server" AutoPostBack="True" RepeatDirection="Horizontal">
        <asp:ListItem>Browse</asp:ListItem>
        <asp:ListItem>Search</asp:ListItem>
    </asp:RadioButtonList>
    <asp:Panel ID="pnlSearch" runat="server">
        <hr />
        Text:<asp:TextBox ID="txtSearchValue" runat="server"></asp:TextBox><asp:Button ID="btnGet"
            runat="server" Text="Search" />
    </asp:Panel>
    <uc2:getSchool ID="ucGetSchool" runat="server" />
    <asp:GridView ID="grdDepartment" runat="server" AutoGenerateColumns="False" DataKeyNames="ID">
        <Columns>
            <asp:BoundField DataField="ID" HeaderText="ID" SortExpression="ID" />
            <asp:BoundField DataField="code" HeaderText="Code" SortExpression="code" />
            <asp:BoundField DataField="longName" HeaderText="Name" SortExpression="longName" />
            <asp:CommandField ShowSelectButton="True" />
        </Columns>
    </asp:GridView>
</asp:Panel>
