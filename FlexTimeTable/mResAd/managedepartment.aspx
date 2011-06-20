<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Flex.Master"
    CodeBehind="managedepartment.aspx.vb" Inherits="FlexTimeTable.managedepartment" %>

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
        Manage Department</h3>
    <asp:Label ID="lblMessage" runat="server" Text="Label"></asp:Label><br />
    <asp:Literal ID="litSchool" runat="server"></asp:Literal>
    <asp:DropDownList ID="cboSchool" AutoPostBack="true" runat="server">
    </asp:DropDownList>
    <asp:DropDownList ID="cboFaculty" AutoPostBack="true" runat="server">
    </asp:DropDownList>
    <br />
    <asp:MultiView ID="mvDept" runat="server">
        <asp:View ID="vwGrid" runat="server">
            <asp:LinkButton ID="lnkCreate" runat="server">New Department</asp:LinkButton>
            <asp:GridView ID="grdDepartment" runat="server" AutoGenerateColumns="False" DataKeyNames="ID">
                <Columns>
                    <asp:BoundField DataField="ID" HeaderText="ID" SortExpression="ID" />
                    <asp:BoundField DataField="code" HeaderText="Code" SortExpression="code" />
                    <asp:BoundField DataField="longName" HeaderText="Name" SortExpression="longName" />
                    <asp:CommandField ShowSelectButton="True" />
                </Columns>
            </asp:GridView>
        </asp:View>
        <asp:View ID="vwEdit" runat="server">
            <p>
                <b>
                    <asp:Literal ID="litEdit" runat="server"></asp:Literal></b></p>
            <table style="width: 38%;">
                <tr>
                    <td class="style1">
                        <asp:Label ID="Label4" runat="server" AssociatedControlID="lblID" Text="ID:"></asp:Label>
                    </td>
                    <td>
                        <asp:Label ID="lblID" runat="server"></asp:Label>
                    </td>
                </tr>
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
                        <asp:Button ID="btnSave" runat="server" Text="Save" />
                        <asp:Button ID="btnDelete" runat="server" Text="Delete" />
                        <asp:Button ID="btnCancel" runat="server" Text="Cancel" />
                    </td>
                </tr>
            </table>
        </asp:View>
    </asp:MultiView>
</asp:Content>
