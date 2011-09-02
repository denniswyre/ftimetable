<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="classresource.ascx.vb"
    Inherits="FlexTimeTable.classresource" %>
<asp:MultiView ID="Pages" runat="server">
    <asp:View ID="ViewGrid" runat="server">
            <asp:LinkButton ID="lnkCreate" runat="server">New Resource</asp:LinkButton>
            <asp:GridView ID="grdResource" runat="server" DataKeyNames="ID" AutoGenerateColumns="False">
                <Columns>
                    <asp:BoundField DataField="ID" HeaderText="ID" SortExpression="ID" />
                    <asp:BoundField DataField="Name" HeaderText="Name" SortExpression="Name" />
                    <asp:BoundField DataField="TypeName" HeaderText="Type" SortExpression="TypeName" />
                    <asp:BoundField DataField="slots" HeaderText="Slots" SortExpression="slots" />
                    <asp:BoundField DataField="size" HeaderText="Size" SortExpression="size" />
                    <asp:CommandField ShowSelectButton="True" />
                </Columns>
            </asp:GridView>
    </asp:View>
    <asp:View ID="ViewEdit" runat="server">
        <asp:Label ID="lblMessage" runat="server"></asp:Label>
        <asp:Panel ID="pnlAction" runat="server">
            <table>
                <tr>
                    <td>
                        ID:
                    </td>
                    <td>
                        <asp:Label ID="lblID" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td>
                        Name:
                    </td>
                    <td>
                        <asp:TextBox ID="txtName" runat="server"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>
                        Number of Students:
                    </td>
                    <td>
                        <asp:TextBox ID="txtNoOfParticipants" runat="server"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>
                        Number of Time Slots:
                    </td>
                    <td>
                        <asp:DropDownList ID="cboAmtTimeSlots" runat="server">
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td>
                        Max Merged Timeslots:
                    </td>
                    <td>
                        <asp:DropDownList ID="cboMergedTimeSlots" runat="server">
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td>
                        Resource Type:
                    </td>
                    <td>
                        <asp:DropDownList ID="cboResourceType" runat="server" AutoPostBack="True">
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td>
                        Preferred Venue(s):
                    </td>
                    <td>
                        <asp:ListBox ID="lstSelVenues" runat="server" Width="300px"></asp:ListBox>
                        <div>
                            <asp:DropDownList ID="cboAvailVenue" runat="server" Width="200px">
                            </asp:DropDownList>
                            <asp:Button ID="btnVenueAdd" runat="server" Text="add" />
                            <asp:Button ID="btnVenueRem" runat="server" Text="rem" /></div>
                    </td>
                </tr>
                <tr>
                    <td colspan="2" align="center">
                    </td>
                </tr>
                <tr>
                    <td align="center" colspan="2">
                        &nbsp;
                    </td>
                </tr>
                <tr>
                    <td align="center" class="style1" colspan="2">
                        <asp:Button ID="btnSave" runat="server" Text="Save" />
                        <asp:Button ID="btnDelete" runat="server" Text="Delete" />
                        <asp:Button ID="btnCancel" runat="server" Text="Cancel" />
                    </td>
                </tr>
            </table>
        </asp:Panel>
    </asp:View>
</asp:MultiView>
