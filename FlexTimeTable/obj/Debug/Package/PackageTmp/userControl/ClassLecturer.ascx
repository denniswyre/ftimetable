<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ClassLecturer.ascx.vb" Inherits="FlexTimeTable.ClassLecturer" %>
    <table>
        <tr>
            <td>
                Assigned:
            </td>
            <td>
                <asp:Label ID="lblLecturer" runat="server" ></asp:Label>
            </td>
        </tr>
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
    </table>