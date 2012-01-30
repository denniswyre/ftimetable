<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="EditTimeslot.ascx.vb"
    Inherits="FlexTimeTable.EditTimeslot" %>
<asp:Panel ID="pnlMain" GroupingText="Edit Time Slot" runat="server">
    <div style="float: right; margin-top: -15px">
        <asp:LinkButton ID="btnReturn" runat="server">Return</asp:LinkButton>
    </div>
    <div style="clear: both">
    </div>
    <div style="margin-top: -10px">
        <asp:Literal ID="errorMessage" runat="server"></asp:Literal>
        <table>
            
            <tr>
                <td>
                    Subject:
                </td>
                <td>
                    <asp:DropDownList ID="cboSubject" runat="server" AutoPostBack="True">
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td>
                    Class:
                </td>
                <td>
                    <asp:DropDownList ID="cboClass" runat="server" AutoPostBack="True">
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td>
                    Resource:
                </td>
                <td>
                    <asp:DropDownList ID="cboResource" runat="server" AutoPostBack="True">
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td>
                    Cluster:
                </td>
                <td>
                    <asp:DropDownList ID="cboCluster" runat="server">
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td>
                    Site:
                </td>
                <td>
                    <asp:DropDownList ID="cboSite" runat="server" AutoPostBack="True">
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td>
                    Venue:
                </td>
                <td>
                    <asp:DropDownList ID="cboVenue" runat="server">
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td>
                    Day of Week:
                </td>
                <td>
                    <asp:DropDownList ID="cboDay" runat="server">
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td>
                    Start Time:
                </td>
                <td>
                    <asp:DropDownList ID="cboStartSlot" runat="server">
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td>
                    No of Slots:
                </td>
                <td>
                    <asp:DropDownList ID="cboNoSlots" runat="server">
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td>
                    Strict Venue:</td>
                <td>
                    <asp:CheckBox ID="chkStrictVenue" runat="server" AutoPostBack="True" />
                </td>
            </tr>
            <tr>
                <td>
                </td>
                <td>
                    <asp:Button ID="btnSave" runat="server" Text="Save" />
                </td>
            </tr>
        </table>
    </div>
</asp:Panel>
