<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Flex.Master"
    CodeBehind="assignQualification.aspx.vb" Inherits="FlexTimeTable.assignQualification" %>

<%@ Register Src="../userControl/getDepartment.ascx" TagName="getDepartment" TagPrefix="uc1" %>
<%@ Register src="../userControl/logButton.ascx" tagname="logButton" tagprefix="uc2" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h3>
        Assign Qualifications to Departments</h3>
    <div>
        <uc1:getDepartment ID="ucGetDepartment" runat="server" />
    </div>
    <asp:Literal ID="errorMessage" runat="server"></asp:Literal>
    <table align="center">
        <tr>
            <td>
                Unassigned Qualifications<br />
                <asp:ListBox ID="lstUnassigned" runat="server" Height="300px" Font-Size="Smaller" Width="400px"></asp:ListBox>
            </td>
            <td align="center">
                <uc2:logButton ID="logUnassigned" runat="server" /><br />
                <uc2:logButton ID="logAssigned" runat="server" /><br />
                <asp:Button ID="btnRefresh" runat="server" Text="Refresh" Width="100px" />
            </td>
            <td>
                Assigned Qualifications<br />
                <asp:ListBox ID="lstAssigned" runat="server" Height="300px" Font-Size="Smaller" Width="400px"></asp:ListBox>
            </td>
        </tr>
    </table>
</asp:Content>
