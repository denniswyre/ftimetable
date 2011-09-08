<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Flex.Master" CodeBehind="assignSubject.aspx.vb" Inherits="FlexTimeTable.assignSubject" %>
<%@ Register src="../userControl/getDepartment.ascx" tagname="getDepartment" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h3>
        Assign Subjects to Departments</h3>
    <p>
        <uc1:getDepartment ID="getDepartment1" runat="server" />
    </p>
    <p>
        <table>
            <tr>
                <td>
                    Unassigned Subjects<br />
                    <asp:ListBox ID="lstUnassigned" runat="server" Height="300px" Width="300px"></asp:ListBox>
                </td>
                <td align="center">
                    <asp:Button ID="btnAssigned" runat="server" Text="Assign" Width="100px" />
                    <br />
                    <asp:Button ID="btnUnassigned" runat="server" Text="Remove" Width="100px" />
                </td>
                <td>
                    Assigned Subjects<br />
                    <asp:ListBox ID="lstAssigned" runat="server" Height="300px" Width="300px"></asp:ListBox>
                </td>
            </tr>
        </table>
    </p>
 
</asp:Content>
