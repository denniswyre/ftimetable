<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Flex.Master"
    CodeBehind="managesubject.aspx.vb" Inherits="FlexTimeTable.managesubject" %>

<%@ Register Src="~/userControl/logButton.ascx" TagName="logButton" TagPrefix="uc1" %>
<%@ Register Src="~/userControl/getDepartment.ascx" TagName="getDepartment" TagPrefix="uc2" %>
<%@ Register Src="~/userControl/subjectsearch.ascx" TagName="subjectsearch" TagPrefix="uc3" %>
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
    <asp:Literal ID="errorMessage" runat="server"></asp:Literal><br />
    <asp:MultiView ID="mvSubject" runat="server" ActiveViewIndex="0">
        <asp:View ID="vwGrid" runat="server">
            <asp:LinkButton ID="lnkCreate" runat="server">Create New Subject</asp:LinkButton>
            <uc3:subjectsearch ID="ucSubjectSearch" runat="server" />
        </asp:View>
        <asp:View ID="vwEdit" runat="server">
            <asp:LinkButton ID="btnCancel" runat="server">Return</asp:LinkButton>&nbsp;&nbsp;
            <asp:PlaceHolder ID="phAccess" runat="server">
                <asp:LinkButton ID="lnkEdit" runat="server">Edit</asp:LinkButton>&nbsp;&nbsp;
            </asp:PlaceHolder>
            <asp:Panel ID="pnlDetail" GroupingText="" runat="server">
                <asp:Literal ID="litOldDepartment" runat="server"></asp:Literal>
                <uc2:getDepartment ID="ucGetDepartment" runat="server" />
                <table>
                    <asp:PlaceHolder ID="phID" runat="server">
                        <tr>
                            <td class="style1">
                                <asp:Label ID="Label4" runat="server" AssociatedControlID="lblID" Text="Subject ID:"></asp:Label>
                            </td>
                            <td>
                                <asp:Label ID="lblID" runat="server"></asp:Label>
                            </td>
                        </tr>
                    </asp:PlaceHolder>
                    <tr>
                        <td class="style1">
                            <asp:Label ID="Label2" runat="server" AssociatedControlID="txtCode" Text="Subject Code:"></asp:Label>
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
                    <asp:PlaceHolder ID="phOldCodes" runat="server">
                        <tr>
                            <td class="style1">
                                <asp:Label ID="Label6" runat="server" Text="Old Subject Codes:"></asp:Label>
                            </td>
                            <td>
                                <asp:Label ID="lblOldCodes" runat="server" Text="Old Codes:"></asp:Label>
                            </td>
                        </tr>
                    </asp:PlaceHolder>
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
