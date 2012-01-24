<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Flex.Master"
    CodeBehind="managedepartment.aspx.vb" Inherits="FlexTimeTable.managedepartment" %>

<%@ Register Src="~/userControl/logButton.ascx" TagName="logButton" TagPrefix="uc1" %>
<%@ Register Src="~/userControl/departmentSearch.ascx" TagName="departmentSearch"
    TagPrefix="uc2" %>
<%@ Register Src="~/userControl/getSchool.ascx" TagName="getSchool" TagPrefix="uc3" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style type="text/css">
        .style1
        {
            width: 200px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h3>
        Manage Department</h3>
    <asp:Label ID="lblMessage" runat="server" Text=""></asp:Label>
    <asp:MultiView ID="mvDept" runat="server" ActiveViewIndex="0">
        <asp:View ID="vwGrid" runat="server">
            <asp:LinkButton ID="lnkCreate" runat="server">New Department</asp:LinkButton>
            <uc2:departmentSearch ID="ucDepartmentSearch" runat="server" />
        </asp:View>
        <asp:View ID="vwEdit" runat="server">
            <asp:LinkButton ID="btnCancel" runat="server">Return</asp:LinkButton>
            <asp:PlaceHolder ID="phAccess" runat="server">&nbsp;&nbsp;
                <asp:LinkButton ID="lnkEdit" runat="server">Edit</asp:LinkButton>
                &nbsp;&nbsp;</asp:PlaceHolder>
            <asp:Panel ID="pnlDetail" GroupingText="" runat="server">
                <asp:Literal ID="litOldSchool" runat="server"></asp:Literal>
                <uc3:getSchool ID="ucSchool" runat="server" />
                <table>
                    <asp:PlaceHolder ID="phID" runat="server">
                        <tr>
                            <td class="style1">
                                <asp:Label ID="Label4" runat="server" AssociatedControlID="lblID" Text="ID:"></asp:Label>
                            </td>
                            <td>
                                <asp:Label ID="lblID" runat="server"></asp:Label>
                            </td>
                        </tr>
                    </asp:PlaceHolder>
                    <tr>
                        <td class="style1">
                            <asp:Label ID="Label2" runat="server" AssociatedControlID="txtCode" Text="Code:"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtCode" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="style1">
                            <asp:Label ID="Label1" runat="server" AssociatedControlID="txtShortName" Text="Short Name:"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtShortName" runat="server" Width="400px"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="style1">
                            <asp:Label ID="Label5" runat="server" AssociatedControlID="txtLongName" Text="Long Name:"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtLongName" runat="server" Width="400px"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2" align="center">
                            <uc1:logButton ID="logSave" runat="server" />
                            <uc1:logButton ID="logDelete" runat="server" />
                            <asp:Button ID="btnCancelEdit" runat="server" Text="Cancel" />
                        </td>
                    </tr>
                </table>
            </asp:Panel>
        </asp:View>
    </asp:MultiView>
</asp:Content>
