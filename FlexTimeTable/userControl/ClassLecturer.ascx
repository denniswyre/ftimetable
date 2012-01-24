<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ClassLecturer.ascx.vb"
    Inherits="FlexTimeTable.ClassLecturer" %>
<asp:Panel ID="pnlLecturer" GroupingText="Lecturer" runat="server">
    <table>
        <tr>
            <td>
                <asp:Literal ID="litLabel" runat="server"></asp:Literal>
            </td>
            <td>
                <asp:Label ID="lblLecturer" runat="server"></asp:Label>
            </td>
        </tr>
        <asp:PlaceHolder ID="phEdit" runat="server">
            <tr>
                <td>
                    Candidates:
                </td>
                <td>
                    <asp:DropDownList ID="cboLecturer" runat="server">
                    </asp:DropDownList>
                    <asp:Button ID="btnLecturer" runat="server" Text="Assign" />
                </td>
            </tr>
        </asp:PlaceHolder>
    </table>
</asp:Panel>
