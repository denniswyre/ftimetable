<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Flex.Master"
    CodeBehind="AcademicBlock.aspx.vb" Inherits="FlexTimeTable.AcademicBlock1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style type="text/css">
    .style1
    {
        height: 29px;
    }
</style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h3>
        Academic block</h3>
    <asp:Literal ID="litMessage" runat="server"></asp:Literal><br />
    <asp:LinkButton ID="lnkCreate" runat="server">New Academic Block</asp:LinkButton>
    <asp:GridView ID="GrdBlock" runat="server" AutoGenerateColumns="False" DataKeyNames="ID">
        <Columns>
            <asp:BoundField DataField="ID" HeaderText="ID" SortExpression="ID" />
            <asp:BoundField DataField="Name" HeaderText="Name" SortExpression="Name" />
            <asp:BoundField DataField="startWeek" HeaderText="Start" SortExpression="startWeek" />
            <asp:BoundField DataField="endWeek" HeaderText="End" SortExpression="endWeek" />
            <asp:CheckBoxField DataField="yearBlock" HeaderText="Year" 
                SortExpression="yearBlock" />
            <asp:CommandField ShowSelectButton="True" />
        </Columns>
    </asp:GridView>
    <asp:Panel ID="pnlAction" runat="server">
        <table>
            <tr>
                <td>
                    <asp:Label ID="Label1" runat="server" Text="ID:"></asp:Label>
                </td>
                <td>
                     <asp:Label ID="lblID" runat="server" Text="ID:"></asp:Label>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="Label2" runat="server" Text="Name:"></asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="txtName" runat="server"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td class="style1">
                    <asp:Label ID="Label6" runat="server" Text="Year Block:"></asp:Label>
                </td>
                <td class="style1">
                    <asp:CheckBox ID="chkYearBlock" runat="server" AutoPostBack="True" />
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="Label4" runat="server" Text="Start Week:"></asp:Label>
                </td>
                <td>
                    <asp:DropDownList ID="cboStart" runat="server">
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="Label5" runat="server" Text="End week:"></asp:Label>
                </td>
                <td>
                    <asp:DropDownList ID="cboEnd" runat="server">
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td align="center" colspan="2">
                    <asp:Button ID="btnSave" runat="server" Text="Save" />
                    <asp:Button ID="btnDelete" runat="server" Text="Delete" />
                    <asp:Button ID="btnCancel" runat="server" Text="Cancel" />
                </td>
            </tr>
        </table>
    </asp:Panel>
</asp:Content>
