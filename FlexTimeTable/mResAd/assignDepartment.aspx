<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Flex.Master" CodeBehind="assignDepartment.aspx.vb" Inherits="FlexTimeTable.assignDepartment" %>
<%@ Register src="../userControl/logButton.ascx" tagname="logButton" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
 <h3>
        Move Departments between schools</h3>

    Faculty:<asp:DropDownList ID="cboFaculty" AutoPostBack="true" runat="server">
    </asp:DropDownList><br />
    <asp:Literal ID="ErrorMessage" runat="server"></asp:Literal>
    <table align="center">
        <tr>
            <td>Unknown Departments<br />
                <asp:ListBox ID="lstUnknownDepart" runat="server" Height="300px" Font-Size="Smaller" Width="400px"></asp:ListBox>
            </td>
            <td align="center">
                <uc1:logButton ID="logAdd" runat="server" /><br />
                <uc1:logButton ID="logDel" runat="server" />
                <asp:Button ID="btnRefresh" runat="server" Text="Refresh" Width="100px" />
            </td>
            <td>School:<asp:DropDownList ID="cboSchool" runat="server" AutoPostBack="True" 
                    Width="345px">
                </asp:DropDownList><br />
                <asp:ListBox ID="lstSelectedDepart" runat="server" Height="300px" Font-Size="Smaller" Width="400px"></asp:ListBox>
             </td>
        </tr>
       
    </table>

</asp:Content>
