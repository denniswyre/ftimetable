<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="subjectsearch.ascx.vb"
    Inherits="FlexTimeTable.subjectsearch" %>
<%@ Register Src="~/userControl/logButton.ascx" TagName="logButton" TagPrefix="uc1" %>
<%@ Register Src="getDepartment.ascx" TagName="getDepartment" TagPrefix="uc2" %>
<asp:Panel ID="Panel1" GroupingText="Browse/Search Subjects" runat="server">
    <div style="float: right">
        <asp:LinkButton ID="btnReturn" runat="server">Close</asp:LinkButton>
    </div>
    <div style="clear: both">
    </div>
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
    <uc2:getDepartment ID="ucDepartment" runat="server" />
    <asp:GridView ID="grdsubject" runat="server" AutoGenerateColumns="False" DataKeyNames="ID">
        <Columns>
            <asp:BoundField DataField="ID" HeaderText="ID" SortExpression="ID" />
            <asp:BoundField DataField="code" HeaderText="New Code" SortExpression="code" />
            <asp:TemplateField HeaderText="Old Code(s)">
                <ItemTemplate>
                    <asp:Label ID="Label1" runat="server" Text='<%# DisplayOldCodes(Eval("ID")) %>'></asp:Label>
                </ItemTemplate>
                <ItemStyle HorizontalAlign="Left" />
                <HeaderStyle HorizontalAlign="Left" />
            </asp:TemplateField>
            <asp:BoundField DataField="longName" HeaderText="Long Name" SortExpression="longName" />
            <asp:BoundField DataField="level" HeaderText="Level" SortExpression="level" />
            <asp:CheckBoxField DataField="YearBlock" HeaderText="Year" SortExpression="YearBlock" />
            <asp:CommandField ShowSelectButton="True" />
        </Columns>
    </asp:GridView>
</asp:Panel>
