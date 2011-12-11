<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Flex.Master"
    CodeBehind="assignSubject.aspx.vb" Inherits="FlexTimeTable.assignSubject" %>

<%@ Register Src="../userControl/getDepartment.ascx" TagName="getDepartment" TagPrefix="uc1" %>
<%@ Register src="../userControl/logButton.ascx" tagname="logButton" tagprefix="uc2" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h3>
        Assign Subjects to Departments</h3>
    <div>
        <uc1:getDepartment ID="getDepartment1" runat="server" />
    </div>
    <asp:Literal ID="errorMessage" runat="server"></asp:Literal>
    <table align="center">
        <tr>
            <td>
                Unassigned Subjects<br />
                Search: <asp:TextBox ID="txtSubjectSearch" runat="server"></asp:TextBox><asp:Button ID="btnSearch"
                runat="server" Text="Search" />
                <asp:ListBox ID="lstUnassigned" runat="server" Height="300px" Font-Size="Smaller" Width="400px"></asp:ListBox>
            </td>
           
            <td align="center">
                <uc2:logButton ID="logAssigned" runat="server" /><br />
                <uc2:logButton ID="logUnassigned" runat="server" /><br />
                <asp:Button ID="btnRefresh" runat="server" Text="Refresh" Width="100px" />
            </td>
            <td>
                Assigned Subjects<br />
                <asp:ListBox ID="lstAssigned" runat="server" Height="300px" Font-Size="Smaller" Width="400px"></asp:ListBox>
            </td>
        </tr>
    </table>
</asp:Content>
