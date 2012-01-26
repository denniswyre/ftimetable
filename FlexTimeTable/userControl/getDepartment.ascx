<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="getDepartment.ascx.vb"
    Inherits="FlexTimeTable.getDepartment" %>
<hr />
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
    <asp:PlaceHolder ID="phUpdate" runat="server">
        <tr>
            <td width="200px">
            </td>
            <td>
                <asp:Button ID="btnUpdate" runat="server" Text="Update Department" />
            </td>
        </tr>
    </asp:PlaceHolder>
</table>
<hr />
