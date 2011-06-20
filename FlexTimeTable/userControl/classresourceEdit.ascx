<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="classresourceEdit.ascx.vb" Inherits="FlexTimeTable.classresourceEdit" %>
<asp:Panel ID="pnlAction" runat="server">
    <table style="width: 100%;">
        <tr>
            <td  colspan="2">
                <asp:Label ID="lblMessage" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <td >
                ID:
            </td>
            <td>
                <asp:Label ID="lblID" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <td >
                Name:
            </td>
            <td>
                <asp:TextBox ID="txtName" runat="server"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td >
                Number of Students:
            </td>
            <td>
                <asp:TextBox ID="txtNoOfParticipants" runat="server"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td >
                Resource Type:
            </td>
            <td>
                <asp:DropDownList ID="cboResourceType" runat="server">
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td >
                Number of Time Slots:
            </td>
            <td>
                <asp:DropDownList ID="cboAmtTimeSlots" runat="server">
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td >
                Max Merged Timeslots:
            </td>
            <td>
                <asp:DropDownList ID="cboMergedTimeSlots" runat="server">
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td >
                </td>
            <td>
                </td>
        </tr>
        <tr>
            <td colspan="2" align="center">
               </td>
        </tr>
        <tr>
            <td align="center" colspan="2">
                &nbsp;</td>
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