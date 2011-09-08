<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Flex.Master" CodeBehind="assignDepartment.aspx.vb" Inherits="FlexTimeTable.assignDepartment" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
 <h3>
        Move Departments between schools</h3>
    Faculty:<asp:DropDownList ID="cboFaculty" AutoPostBack="true" runat="server">
    </asp:DropDownList><br />
    <table>
        <tr>
            <td>School A:<asp:DropDownList ID="cboSchoolA" runat="server" Width="245px" 
                    AutoPostBack="True">
                </asp:DropDownList><br />
                <asp:ListBox ID="lstDepartmentA" runat="server" Height="300px" Width="300px"></asp:ListBox>
            </td>
            <td>
                <asp:Button ID="btnAddto" runat="server" Text="--->" /><br />
                 <asp:Button ID="btnAddfr" runat="server" Text="<---" />
            </td>
            <td>School B:<asp:DropDownList ID="cboSchoolB" runat="server" AutoPostBack="True" 
                    Width="245px">
                </asp:DropDownList><br />
                <asp:ListBox ID="lstDepartmentB" runat="server" Height="300px" Width="300px"></asp:ListBox>
             </td>
        </tr>
       
    </table>

</asp:Content>
