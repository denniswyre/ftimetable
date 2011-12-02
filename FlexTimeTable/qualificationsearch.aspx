<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Flex.Master"
    CodeBehind="qualificationsearch.aspx.vb" Inherits="FlexTimeTable.qualificationsearch" %>

<%@ Register Src="~/userControl/logButton.ascx" TagName="logButton" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h3>
        Manage Qualifications</h3>
    <asp:Literal ID="litMessage" runat="server"></asp:Literal>
    <asp:MultiView ID="mvQual" runat="server">
        <asp:View ID="vwGrid" runat="server">
            Search by:<asp:RadioButtonList ID="optSearchType" runat="server" RepeatDirection="Horizontal">
                <asp:ListItem>New Code</asp:ListItem>
                <asp:ListItem>Old Code</asp:ListItem>
                <asp:ListItem>Name</asp:ListItem>
            </asp:RadioButtonList>
            <br />
            <asp:TextBox ID="txtSearchValue" runat="server"></asp:TextBox><asp:Button ID="btnGet"
                runat="server" Text="Search" />
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
        </asp:View>
        <asp:View ID="vwView" runat="server">
            <div><asp:Literal ID="litQual" runat="server"></asp:Literal><</div>
            <div>
                <asp:LinkButton ID="btnCancel" runat="server">Cancel</asp:LinkButton>
            </div>                
        </asp:View>
    </asp:MultiView>
</asp:Content>
