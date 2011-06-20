<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Flex.Master"
    CodeBehind="managevenues.aspx.vb" Inherits="FlexTimeTable.managevenues" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style type="text/css">
        .style1
        {
            width: 92px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h3>
        Manage Venues</h3>
    <asp:Literal ID="litMessage" runat="server"></asp:Literal>
    <table>
        <tr>
            <td>
                Campus:
            </td>
            <td>
                <asp:DropDownList ID="cboCampus" AutoPostBack="true" runat="server">
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td>
                <asp:Literal ID="litSite" runat="server"></asp:Literal>
            </td>
            <td>
                <asp:DropDownList ID="cboSite" AutoPostBack="true" runat="server">
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td>
                <asp:Literal ID="litBuilding" runat="server"></asp:Literal>
            </td>
            <td>
                <asp:DropDownList ID="cboBuilding" AutoPostBack="true" runat="server">
                </asp:DropDownList>
            </td>
        </tr>
    </table>
    <asp:MultiView ID="mvVenue" runat="server">
        <asp:View ID="vwGrid" runat="server">
            <asp:LinkButton ID="lnkCreate" runat="server">New Venue</asp:LinkButton>
            <asp:GridView ID="grdVenue" runat="server" AutoGenerateColumns="False" DataKeyNames="ID">
                <Columns>
                    <asp:BoundField DataField="ID" HeaderText="ID" SortExpression="ID" />
                    <asp:BoundField DataField="TypeID" HeaderText="TypeID" SortExpression="TypeID" />
                    <asp:BoundField DataField="TypeDescription" HeaderText="Type" SortExpression="TypeDescription" />
                    <asp:BoundField DataField="Code" HeaderText="Code" SortExpression="Code" />
                    <asp:BoundField DataField="Description" HeaderText="Description" SortExpression="Description" />
                    <asp:BoundField DataField="Capacity" HeaderText="Capacity" SortExpression="Capacity" />
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
                        <asp:Label ID="Label3" runat="server" AssociatedControlID="txtCapacity" Text="Capacity:"></asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="txtCapacity" runat="server"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="style1">
                        <asp:Label ID="Label5" runat="server" AssociatedControlID="cboType" Text="Type:"></asp:Label>
                    </td>
                    <td>
                        <asp:DropDownList ID="cboType" runat="server">
                        </asp:DropDownList>
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
