<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Flex.Master"
    CodeBehind="userManagement.aspx.vb" Inherits="FlexTimeTable.userManagement" %>

<%@ Register Src="../userControl/ldap.ascx" TagName="ldap" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2>
        User Management</h2>
    <p>
        <asp:Label ID="ActionStatus" runat="server" CssClass="Important" Font-Bold="True"
            ForeColor="Red"></asp:Label>
    </p>
    <div>
        <table>
            <tr>
                <td align="right" valign="top">
                    <b>Role:</b>
                    <asp:DropDownList ID="RoleList" runat="server" AutoPostBack="true" Width="305px">
                    </asp:DropDownList>
                    <asp:GridView ID="RolesUserList" runat="server" AutoGenerateColumns="False" EmptyDataText="No users belong to this role."
                        Width="290px">
                        <Columns>
                            <asp:TemplateField>
                                <HeaderTemplate>
                                    Users
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <asp:Label runat="server" Width="250px" ID="UserNameLabel" Text='<%# Container.DataItem %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:CommandField DeleteText="Remove" ShowDeleteButton="True" />
                        </Columns>
                    </asp:GridView>
                </td>
                <td align="center" valign="top">
                    <b>Username:</b><asp:TextBox ID="UserName" runat="server" Width="205px"></asp:TextBox>
                    <asp:Button ID="GetUser" runat="server" Text="Get" />
                    <br />
                    <br />
                    <asp:Label ID="UserDetails" runat="server"></asp:Label>
                    <asp:CheckBoxList ID="UserRoleList" runat="server">
                    </asp:CheckBoxList>
                    <br />
                    <asp:Button ID="UpdateUser" runat="server" Text="Update" />
                    <asp:Button ID="CancelUser" runat="server" Text="Cancel" />
                </td>
            </tr>
        </table>
    </div>
    <uc1:ldap ID="ldap1" runat="server" />
</asp:Content>
