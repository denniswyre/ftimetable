<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Flex.Master"
    CodeBehind="subjectsearch.aspx.vb" Inherits="FlexTimeTable.subjectsearch" %>

<%@ Register Src="~/userControl/logButton.ascx" TagName="logButton" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h3>
        Subject Search</h3>
    <asp:Label ID="lblMessage" runat="server" Text="Label"></asp:Label><br />
    <asp:MultiView ID="mvSubject" runat="server">
        <asp:View ID="vwGrid" runat="server">
            Search by:<asp:RadioButtonList ID="optSearchType" runat="server" RepeatDirection="Horizontal">
                <asp:ListItem>New Code</asp:ListItem>
                <asp:ListItem>Old Code</asp:ListItem>
                <asp:ListItem>Name</asp:ListItem>
            </asp:RadioButtonList>
            <br />
            <asp:TextBox ID="txtSearchValue" runat="server"></asp:TextBox><asp:Button ID="btnGet"
                runat="server" Text="Search" />
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
        </asp:View>
        <asp:View ID="vwView" runat="server">
            <div>
                <asp:Literal ID="litsubjectdetails" runat="server"></asp:Literal>
            </div>
            <div>
                <asp:LinkButton ID="btnCancel" runat="server">Return</asp:LinkButton>
            </div>
        </asp:View>
    </asp:MultiView>
</asp:Content>
