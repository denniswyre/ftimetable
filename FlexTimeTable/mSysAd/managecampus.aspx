<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Flex.Master"
    CodeBehind="managecampus.aspx.vb" Inherits="FlexTimeTable.managecampus" %>

<%@ Register Src="~/userControl/logButton.ascx" TagName="logButton" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style type="text/css">
        .style1
        {
            width: 92px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h3>
        Campus Managment</h3>
    <br />
    <asp:Label ID="lblMessage" runat="server" Text=""></asp:Label><br />
    <asp:LinkButton ID="lnkCreate" runat="server">New Campus</asp:LinkButton>
    <asp:GridView ID="GrdCampus" runat="server" AutoGenerateColumns="False" DataKeyNames="ID">
        <Columns>
            <asp:BoundField DataField="ID" HeaderText="ID" SortExpression="ID" />
            <asp:BoundField DataField="shortName" HeaderText="Code" SortExpression="shortName" />
            <asp:BoundField DataField="longName" HeaderText="Name" SortExpression="longName" />
            <asp:CommandField ShowSelectButton="True" />
        </Columns>
    </asp:GridView>
    <asp:Panel ID="pnlAction" runat="server" GroupingText="">
        <table style="width: 38%;">
            <tr>
                <td class="style1">
                    <asp:Label ID="Label4" runat="server" AssociatedControlID="txtID" Text="ID:"></asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="txtID" runat="server"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td class="style1">
                    <asp:Label ID="Label2" runat="server" AssociatedControlID="txtShortName" Text="Code:"></asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="txtShortName" runat="server"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td class="style1">
                    <asp:Label ID="Label1" runat="server" AssociatedControlID="txtLongName" Text="Name:"></asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="txtLongName" runat="server"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td colspan="2" align="center">
                    <asp:Button ID="btnSave" runat="server" Text="Save" />
                    <asp:Button ID="btnDelete" runat="server" Text="Delete" />
                    <asp:Button ID="btnCancel" runat="server" Text="Cancel" />
                </td>
            </tr>
        </table>
        <br />
        <br />
        <br />
        <br />
    </asp:Panel>
</asp:Content>
