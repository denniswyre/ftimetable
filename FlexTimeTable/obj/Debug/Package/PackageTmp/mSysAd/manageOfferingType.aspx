<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Flex.Master"
    CodeBehind="manageOfferingType.aspx.vb" Inherits="FlexTimeTable.manageOfferingType" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style type="text/css">
        .style1
        {
            height: 33px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h3>
        Offering types</h3>
    <asp:Literal ID="litMessage" runat="server"></asp:Literal><br />
    <asp:LinkButton ID="lnkCreate" runat="server">New Offering Type</asp:LinkButton>
    <asp:GridView ID="grdOffering" runat="server" AutoGenerateColumns="False" DataKeyNames="ID">
        <Columns>
            <asp:BoundField DataField="ID" HeaderText="ID" SortExpression="ID" />
            <asp:BoundField DataField="Code" HeaderText="Code" SortExpression="Code" />
            <asp:BoundField DataField="Name" HeaderText="Name" SortExpression="Name" />
            <asp:TemplateField HeaderText="Start" SortExpression="StartTime">
                <ItemTemplate>
                    <%# DisplayTime(Eval("startTimeSlot"), 0)%>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="End" SortExpression="EndTime">
                <ItemTemplate>
                    <%# DisplayTime(Eval("endTimeSlot"), 1)%>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Sat. Classes" SortExpression="SabbathClasses">
                <ItemTemplate>
                    <%# IIf(Boolean.Parse(Eval("SabbathClasses").ToString()), "Yes", "No")%>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Start" SortExpression="SabStart">
                <ItemTemplate>
                    <%# DisplayTime(Eval("sabStartTimeSlot"), 0)%>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="End" SortExpression="SabEnd">
                <ItemTemplate>
                    <%# DisplayTime(Eval("sabEndTimeSlot"), 1)%>
                </ItemTemplate>
            </asp:TemplateField>
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
                    <asp:Label ID="lblID" runat="server"></asp:Label>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="Label2" runat="server" Text="Code:"></asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="txtCode" runat="server"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td class="style1">
                    <asp:Label ID="Label4" runat="server" Text="Name:"></asp:Label>
                </td>
                <td class="style1">
                    <asp:TextBox ID="txtName" runat="server"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td class="style1">
                    <asp:Label ID="Label5" runat="server" Text="Start Time:"></asp:Label>
                </td>
                <td class="style1">
                    <asp:DropDownList ID="cboStartTime" runat="server">
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td class="style1">
                    <asp:Label ID="Label6" runat="server" Text="End Time:"></asp:Label>
                </td>
                <td class="style1">
                    <asp:DropDownList ID="cboEndTime" runat="server">
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td class="style1">
                    <asp:Label ID="Label9" runat="server" Text="Saturday Classes?"></asp:Label>
                </td>
                <td class="style1">
                    <asp:CheckBox ID="chkSabbathClasses" runat="server" />
                </td>
            </tr>
            <tr>
                <td class="style1">
                    <asp:Label ID="Label7" runat="server" Text="Saturday Start Time:"></asp:Label>
                </td>
                <td class="style1">
                    <asp:DropDownList ID="cboSabStart" runat="server">
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td class="style1">
                    <asp:Label ID="Label8" runat="server" Text="Saturday End Time:"></asp:Label>
                </td>
                <td class="style1">
                    <asp:DropDownList ID="cboSabEnd" runat="server">
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
