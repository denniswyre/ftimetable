<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Flex.Master"
    CodeBehind="managetimeslot.aspx.vb" Inherits="FlexTimeTable.managetimeslot" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h3>
        Time Slots</h3>
    <asp:Literal ID="litMessage" runat="server"></asp:Literal><br />
    <asp:LinkButton ID="lnkCreate" runat="server">New Time Slot</asp:LinkButton>
    <table>
        <tr>
            <td valign="top">
                <asp:GridView ID="GrdTimeslot" runat="server" AutoGenerateColumns="False" DataKeyNames="ID">
                    <Columns>
                        <asp:BoundField DataField="ID" HeaderText="ID" SortExpression="ID" />
                        <asp:BoundField DataField="StartTime" HeaderText="StartTime" DataFormatString="{0:HH:mm}"
                            SortExpression="StartTime" />
                        <asp:BoundField DataField="Duration" HeaderText="Duration" SortExpression="Duration" />
                        <asp:BoundField DataField="LunchPeriod" HeaderText="LunchPeriod" DataFormatString="{0:Yes;No}" SortExpression="LunchPeriod" />
                        <asp:CommandField ShowSelectButton="True" />
                    </Columns>
                </asp:GridView>
            </td>
            <td valign="top">
                <asp:Panel ID="pnlAction" runat="server">
                    <table>
                        <tr>
                            <td>
                                <asp:Label ID="Label1" runat="server" Text="ID:"></asp:Label>
                            </td>
                            <td>
                                <asp:Label ID="lblID" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Lunch Period:</td>
                            <td>
                                <asp:CheckBox ID="chkLunchPeriod" runat="server" />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Label ID="Label2" runat="server" Text="StartTime:"></asp:Label>
                            </td>
                            <td>
                                <asp:TextBox ID="txtStart" runat="server"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Label ID="Label4" runat="server" Text="Duration:"></asp:Label>
                            </td>
                            <td>
                                <asp:TextBox ID="txtDuration" runat="server"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td align="center" colspan="2">
                                <asp:Button ID="btnSave" runat="server" Text="Save" />
                                <asp:Button ID="btnDelete" runat="server" Text="Delete" />
                                <asp:Button ID="btnCancel" runat="server" Text="Cancel" />
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
            </td>
        </tr>
    </table>
</asp:Content>
