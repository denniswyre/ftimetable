<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Flex.Master"
    CodeBehind="resourcetypes.aspx.vb" Inherits="FlexTimeTable.resourcetypes" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h3>
        Resource Types</h3>
    <asp:Label ID="lblMessage" runat="server" Text=""></asp:Label><br />
    <asp:MultiView ID="mvResType" runat="server">
        <asp:View ID="vwGrid" runat="server">
            <asp:LinkButton ID="lnkCreate" runat="server">Create Resource Type</asp:LinkButton>
            <asp:GridView ID="grdresourcetype" DataKeyNames="ID" AutoGenerateColumns="False"
                runat="server">
                <Columns>
                    <asp:BoundField DataField="ID" HeaderText="ID" SortExpression="ID" />
                    <asp:BoundField DataField="Code" HeaderText="Code" SortExpression="Code" />
                    <asp:BoundField DataField="Description" HeaderText="Description" SortExpression="Description" />
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <asp:Label ID="Label5" runat="server" Text="Lecture Room"></asp:Label>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:CheckBox ID="CheckBox1" Enabled="false" runat="server" Checked='<%# Eval("isClassroom")  %>' />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:CommandField ShowSelectButton="True" />
                </Columns>
            </asp:GridView>
        </asp:View>
        <asp:View ID="vwEdit" runat="server">
            <p>
                <b>
                    <asp:Literal ID="litEdit" runat="server"></asp:Literal></b></p>
            <table style="width: 38%;">
                <tr>
                    <td class="style1">
                        <asp:Label ID="Label4" runat="server" AssociatedControlID="lblID" Text="ID:"></asp:Label>
                    </td>
                    <td>
                        <asp:Label ID="lblID" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="style1">
                        <asp:Label ID="Label1" runat="server" AssociatedControlID="txtCode" Text="Code:"></asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="txtCode" runat="server"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="style1">
                        <asp:Label ID="Label2" runat="server" AssociatedControlID="txtDescription" Text="Description:"></asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="txtDescription" runat="server"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="style1">
                        <asp:Label ID="Label3" runat="server" AssociatedControlID="chkClassroom" Text="Lecture Room:"></asp:Label>
                    </td>
                    <td>
                        <asp:CheckBox ID="chkClassroom" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td colspan="2" align="center">
                        <asp:Button ID="btnSave" runat="server" Text="Save" />
                        <asp:Button ID="btnDelete" runat="server" Text="Delete" />
                        <asp:Button ID="btnCancel" runat="server" Text="Cancel" />
                    </td>
                </tr>
            </table>
        </asp:View>
    </asp:MultiView>
</asp:Content>
