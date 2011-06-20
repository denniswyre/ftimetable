<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Flex.Master"
    CodeBehind="manageprogramme.aspx.vb" Inherits="FlexTimeTable.manageprogramme" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h3>
        Programme Management</h3>
    <asp:Label ID="lblMessage" runat="server" Text=""></asp:Label>
    <table>
        <tr>
            <td style="width: 220">
                <asp:Literal ID="litFaculty" runat="server" Text="Faculty:"></asp:Literal>
            </td>
            <td>
                <asp:DropDownList ID="cboFaculty" runat="server" AutoPostBack="True">
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td style="width: 220">
                <asp:Literal ID="litDepartment" runat="server"></asp:Literal>
            </td>
            <td>
                <asp:DropDownList ID="cboDepartment" AutoPostBack="true" runat="server">
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td style="width: 220">
                <asp:Literal ID="litQualification" runat="server" Text="Qualification:"></asp:Literal>
            </td>
            <td>
                <asp:DropDownList ID="cboQualification" AutoPostBack="true" runat="server">
                </asp:DropDownList>
            </td>
        </tr>
        <asp:PlaceHolder ID="phLevel" runat="server">
            <tr>
                <td style="width: 220">
                    <asp:Literal ID="litLevel" runat="server" Text="Level:"></asp:Literal>
                </td>
                <td>
                    <asp:DropDownList ID="cboLevel" AutoPostBack="true" runat="server">
                        <asp:ListItem>1</asp:ListItem>
                        <asp:ListItem>2</asp:ListItem>
                        <asp:ListItem>3</asp:ListItem>
                        <asp:ListItem>4</asp:ListItem>
                        <asp:ListItem>5</asp:ListItem>
                        <asp:ListItem>6</asp:ListItem>
                        <asp:ListItem>7</asp:ListItem>
                    </asp:DropDownList>
                </td>
            </tr>
        </asp:PlaceHolder>
    </table>
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <asp:TabContainer ID="TabContainer1" runat="server" ActiveTabIndex="0">
        <asp:TabPanel runat="server" HeaderText="Core Subjects" ID="TabCore">
            <HeaderTemplate>
                Core Subjects
            </HeaderTemplate>
            <ContentTemplate>
                <div style="border: 2px">
                    <table>
                        <tr>
                            <td valign="top">
                                <asp:TextBox ID="txtCoreSearch" runat="server"></asp:TextBox>
                                <asp:Button ID="btnCoreSearch" runat="server" ToolTip="Search for subjects" Text="Find" /><br />
                                <asp:ListBox ID="lstCoreSubjects" runat="server" Width="300px" Height="300px"></asp:ListBox>
                            </td>
                            <td align="center" valign="middle" style="width: 120">
                                <asp:Button ID="btnCoreAdd" runat="server" Text="Add" Width="100px" /><br />
                                <asp:Button ID="btnCoreRemove" runat="server" Text="Remove" Width="100px" />
                            </td>
                            <td valign="top">
                                <strong>Selected Subjects</strong><br />
                                <asp:ListBox ID="lstSelectedCoreSubjects" runat="server" Width="300px" Height="300px">
                                </asp:ListBox>
                            </td>
                        </tr>
                    </table>
                </div>
            </ContentTemplate>
        </asp:TabPanel>
        <asp:TabPanel runat="server" HeaderText="Service Subjects" ID="TabService">
            <HeaderTemplate>
                Service Subjects
            </HeaderTemplate>
            <ContentTemplate>
                <div style="border: 2px">
                    <table>
                        <tr>
                            <td>
                                <asp:TextBox ID="txtServiceSearch" runat="server"></asp:TextBox><asp:Button ID="btnServiceSearch"
                                    runat="server" ToolTip="Search for a subject" Text="Find" /><br />
                                <asp:ListBox ID="lstServiceSubjects" runat="server" Width="300px" Height="300px">
                                </asp:ListBox>
                            </td>
                            <td align="center" valign="middle" style="width: 120">
                                <asp:Button ID="btnServiceAdd" runat="server" Text="Add" Width="100px" /><br />
                                <asp:Button ID="btnServiceRemove" runat="server" Text="Remove" Width="100px" />
                            </td>
                            <td>
                                <strong>Selected Subjects</strong><br />
                                <asp:ListBox ID="lstSelectedServiceSubject" runat="server" Width="300px" Height="300px">
                                </asp:ListBox>
                            </td>
                        </tr>
                    </table>
                </div>
            </ContentTemplate>
        </asp:TabPanel>
        <asp:TabPanel runat="server" HeaderText="Clusters" ID="TabClusters">
            <ContentTemplate>
                <table>
                    <tr>
                        <td>
                            <strong>All Site Clusters</strong><br />
                            <asp:ListBox ID="lstAllClusters" runat="server" Width="300px"></asp:ListBox>
                        </td>
                        <td align="center" valign="middle" style="width: 120">
                            <asp:Button ID="btnClusterAdd" runat="server" Text="Add" Width="100px" /><br />
                            <asp:Button ID="btnClusterRemove" runat="server" Text="Remove" Width="100px" />
                        </td>
                        <td>
                            <strong>Selected Site Clusters</strong><br />
                            <asp:ListBox ID="lstSelectedClusters" runat="server" Width="300px"></asp:ListBox>
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
        </asp:TabPanel>
    </asp:TabContainer>
</asp:Content>
