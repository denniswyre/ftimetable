<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Flex.Master"
    CodeBehind="managesubject.aspx.vb" Inherits="FlexTimeTable.managesubject" %>

<%@ Register Src="~/userControl/logButton.ascx" TagName="logButton" TagPrefix="uc1" %>
<%@ Register src="../userControl/getDepartment.ascx" tagname="getDepartment" tagprefix="uc2" %>
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
        Manage subjects</h3>
    <asp:Label ID="lblMessage" runat="server" Text="Label"></asp:Label><br />
    <uc2:getDepartment ID="getDepartment1" runat="server" />
    <br />
    <asp:MultiView ID="mvSubject" runat="server">
        <asp:View ID="vwGrid" runat="server">
            <asp:LinkButton ID="lnkCreate" runat="server">New subject</asp:LinkButton>
            <asp:GridView ID="grdsubject" runat="server" AutoGenerateColumns="False" DataKeyNames="ID">
                <Columns>
                    <asp:BoundField DataField="ID" HeaderText="ID" SortExpression="ID" />
                    <asp:BoundField DataField="code" HeaderText="Code" SortExpression="code" />
                    <asp:BoundField DataField="longName" HeaderText="Long Name" SortExpression="longName" />
                    <asp:BoundField DataField="shortName" HeaderText="Short Name" SortExpression="shortName" />
                    <asp:BoundField DataField="level" HeaderText="Level" SortExpression="level" />
                    <asp:CheckBoxField DataField="YearBlock" HeaderText="Year" SortExpression="YearBlock" />
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
                    <td class="style1">
                        <asp:Label ID="Label6" runat="server" Text="Old Codes:"></asp:Label>
                    </td>
                    <td>
                        <asp:ListBox ID="lstOldCodes" runat="server"></asp:ListBox>
                    </td>
                </tr>
                <tr>
                    <td class="style1">
                        <asp:Label ID="Label3" runat="server" Text="Level:"></asp:Label>
                    </td>
                    <td>
                        <asp:DropDownList ID="cboLevel" runat="server">
                            <asp:ListItem>1</asp:ListItem>
                            <asp:ListItem>2</asp:ListItem>
                            <asp:ListItem>3</asp:ListItem>
                            <asp:ListItem>4</asp:ListItem>
                            <asp:ListItem>5</asp:ListItem>
                            <asp:ListItem>6</asp:ListItem>
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td class="style1">
                        <asp:Label ID="Label7" runat="server" Text="Year Block?"></asp:Label>
                    </td>
                    <td>
                        <asp:CheckBox ID="chkYearBlock" runat="server" />
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
