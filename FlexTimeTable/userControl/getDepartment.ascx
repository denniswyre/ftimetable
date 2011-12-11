<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="getDepartment.ascx.vb"
    Inherits="FlexTimeTable.getDepartment" %>
<table>
    <tr>
        <td width="200px">
            Faculty:
        </td>
        <td>
            <asp:DropDownList ID="cboFaculty" AutoPostBack="true" runat="server">
            </asp:DropDownList>
        </td>
    </tr>
    <tr>
        <td width="200px">
            School:
        </td>
        <td>
            <asp:DropDownList ID="cboSchool" AutoPostBack="true" runat="server">
            </asp:DropDownList>
        </td>
    </tr>
    <tr>
        <td width="200px">
            Department:
        </td>
        <td>
            <asp:DropDownList ID="cboDepartment" AutoPostBack="true" runat="server">
            </asp:DropDownList>
        </td>
    </tr>
</table>
