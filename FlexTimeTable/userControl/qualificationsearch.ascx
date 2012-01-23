<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="qualificationsearch.ascx.vb"
    Inherits="FlexTimeTable.qualificationsearch" %>
<%@ Register Src="~/userControl/logButton.ascx" TagName="logButton" TagPrefix="uc1" %>
<%@ Register Src="getDepartment.ascx" TagName="getDepartment" TagPrefix="uc2" %>
<asp:Panel ID="pnlQual" runat="server" GroupingText="Browse/Search Qualifications">
    <asp:Literal ID="litMessage" runat="server"></asp:Literal>
    <asp:RadioButtonList ID="optSearchType" runat="server" AutoPostBack="True" RepeatDirection="Horizontal">
        <asp:ListItem>Browse</asp:ListItem>
        <asp:ListItem>Search</asp:ListItem>
    </asp:RadioButtonList>
    <asp:Panel ID="pnlSearch" runat="server">
        <hr />
        Text:<asp:TextBox ID="txtSearchValue" runat="server"></asp:TextBox><asp:Button ID="btnGet"
            runat="server" Text="Search" />
    </asp:Panel>
    <uc2:getDepartment ID="ucDepartment" runat="server" />
    <asp:GridView ID="grdQualification" runat="server" AutoGenerateColumns="False" DataKeyNames="ID">
        <Columns>
            <asp:BoundField DataField="ID" HeaderText="ID" SortExpression="ID" />
            <asp:BoundField DataField="code" HeaderText="Code" SortExpression="code" />
            <asp:TemplateField HeaderText="Old Code(s)">
                <ItemTemplate>
                    <asp:Label ID="Label1" runat="server" Text='<%# DisplayOldCodes(Eval("ID")) %>'></asp:Label>
                </ItemTemplate>
                <ItemStyle HorizontalAlign="Left" />
                <HeaderStyle HorizontalAlign="Left" />
            </asp:TemplateField>
            <asp:BoundField DataField="longName" HeaderText="Name" SortExpression="longName" />
            <asp:CommandField ShowSelectButton="True" />
        </Columns>
    </asp:GridView>
</asp:Panel>
