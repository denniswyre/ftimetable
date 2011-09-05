<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Flex.Master"
    CodeBehind="ResourceAllocation.aspx.vb" Inherits="FlexTimeTable.ResourceAllocation" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2>
        Academic Administration</h2>
    <p>
        <asp:Label ID="ActionStatus" runat="server" CssClass="Important" Font-Bold="True"
            ForeColor="Red"></asp:Label>
    </p>
    <div>
        Faculty:<asp:DropDownList ID="cboFaculties" runat="server" AutoPostBack="True" Width="243px">
        </asp:DropDownList>
        <asp:GridView ID="grdFacultyUsers" runat="server" AutoGenerateColumns="False" EmptyDataText="No users belong to this role."
            Width="290px">
            <Columns>
                <asp:TemplateField>
                    <HeaderTemplate>Administrators</HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label runat="server" Width="250px" ID="UserNameLabel" Text='<%# Container.DataItem %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:CommandField DeleteText="Remove" ShowDeleteButton="True" />
            </Columns>
        </asp:GridView>
        <asp:DropDownList ID="cboQualifiedUsers" runat="server" Width="240px">
        </asp:DropDownList>
        <asp:Button ID="btnAddUser" runat="server" Text="Add" Width="50px" />
    </div>
</asp:Content>
