<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Flex.Master"
    CodeBehind="managelecturer.aspx.vb" Inherits="FlexTimeTable.managelecturer" %>

<%@ Register Src="../userControl/ldap.ascx" TagName="ldap" TagPrefix="uc1" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register src="../userControl/getDepartment.ascx" tagname="getDepartment" tagprefix="uc2" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h3>
        Manage lecturers</h3>
    <asp:Literal ID="litErrorMessage" runat="server"></asp:Literal>
    <br />
    <uc2:getDepartment ID="getDepartment1" runat="server" />
    <br />
    <asp:MultiView ID="mvLecturer" runat="server" ActiveViewIndex="0">
        <asp:View ID="vwSelect" runat="server">
            <asp:Panel ID="pnlgetLecturer" GroupingText="Add Lecturer to Department" runat="server"
                Width="100%">
                <asp:Label ID="lblUsername" runat="server" Text="Username:"></asp:Label>
                <asp:TextBox ID="txtUsername" Width="300px" runat="server"></asp:TextBox>
                <asp:Label ID="lblproposedlectuer" Width="300px" runat="server"></asp:Label>
                <asp:Button ID="btnGet" runat="server" Text="Get" Width="80px" />
                <asp:Button ID="btnAddLecturer" runat="server" Text="Confirm" Width="80px" />
                <asp:Button ID="btnCancelLecturer" runat="server" Text="Cancel" Width="80px" />
            </asp:Panel>
            <asp:Panel ID="pnlLecturers" GroupingText="Lecturers" runat="server">
                <asp:GridView ID="grdLecturers" runat="server" DataKeyNames="ID" Width="100%" AutoGenerateColumns="False">
                    <Columns>
                        <asp:BoundField DataField="ID" HeaderText="ID" Visible="False" SortExpression="ID" />
                        <asp:BoundField DataField="surname" HeaderText="Surname" SortExpression="surname" />
                        <asp:BoundField DataField="firstname" HeaderText="First Name" SortExpression="firstname" />
                        <asp:BoundField DataField="initials" HeaderText="M.I." SortExpression="initials" />
                        <asp:BoundField DataField="title" HeaderText="Title" SortExpression="title" />
                        <asp:BoundField DataField="username" HeaderText="Username" SortExpression="username" />
                        <asp:CommandField ShowSelectButton="True" />
                        <asp:CommandField DeleteText="Remove" ShowDeleteButton="True" />
                    </Columns>
                </asp:GridView>
            </asp:Panel>
        </asp:View>
        <asp:View ID="vwDetails" runat="server">
            <br />
            <br />
            <table width="100%">
                <tr>
                    <td>
                        <asp:Literal ID="litLecturer" runat="server"></asp:Literal>
                    </td>
                    <td align="right">
                        <asp:LinkButton ID="lnkReturn" runat="server">Return</asp:LinkButton>
                    </td>
                </tr>
            </table>
            <br />
            <asp:TabContainer ID="TabContainer1" runat="server" ActiveTabIndex="0">
                <asp:TabPanel runat="server" HeaderText="Subject" ID="TabSubject">
                    <ContentTemplate>
                        <asp:Panel ID="pnlSubject" GroupingText="Subjects" runat="server">
                            <table width="100%">
                                <tr>
                                    <td width="40%">
                                        Available Subjects
                                        <asp:ListBox ID="lstAvailableSubjects" runat="server" Width="100%"></asp:ListBox>
                                    </td>
                                    <td valign="middle" align="center" width="20%">
                                        <asp:Button ID="btnAdd" runat="server" Text="Add" Width="100px" /><br />
                                        <asp:Button ID="btnRemove" runat="server" Text="Remove" Width="100px" />
                                    </td>
                                    <td width="40%">
                                        Selected Subjects
                                        <asp:ListBox ID="lstSelectedSubjects" runat="server" Width="100%"></asp:ListBox>
                                    </td>
                                </tr>
                            </table>
                        </asp:Panel>
                    </ContentTemplate>
                </asp:TabPanel>
                <asp:TabPanel runat="server" HeaderText="Roster" ID="TabRoster">
                    <ContentTemplate>
                        <asp:Panel ID="pnlRoster" GroupingText="Site Roster" runat="server">
                            <asp:GridView ID="grdRoster" runat="server" DataKeyNames="ClusterID, day, startid"
                                AutoGenerateColumns="false" Width="100%">
                                <Columns>
                                    <asp:BoundField DataField="ClusterID" HeaderText="ClusterID" Visible="false" SortExpression="ClusterID" />
                                    <asp:BoundField DataField="day" HeaderText="day" Visible="false" SortExpression="day" />
                                    <asp:BoundField DataField="startid" HeaderText="startid" Visible="false" SortExpression="startid" />
                                    <asp:BoundField DataField="ClusterName" HeaderText="Site Cluster" SortExpression="ClusterName" />
                                    <asp:TemplateField HeaderText="Day" SortExpression="Day">
                                        <ItemTemplate>
                                            <%# displayday(Eval("day"))%>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Start" SortExpression="StartTime">
                                        <ItemTemplate>
                                            <%# DisplayTime(Eval("startid"), 0)%>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="End" SortExpression="EndTime">
                                        <ItemTemplate>
                                            <%# DisplayTime(Eval("endid"), 1)%>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:CommandField ShowDeleteButton="True" />
                                </Columns>
                            </asp:GridView>
                            <asp:DropDownList ID="cboCluster" runat="server">
                            </asp:DropDownList>
                            <asp:DropDownList ID="cboDay" runat="server">
                            </asp:DropDownList>
                            <asp:DropDownList ID="cboStart" runat="server">
                            </asp:DropDownList>
                            <asp:DropDownList ID="cboEnd" runat="server">
                            </asp:DropDownList>
                            <asp:Button ID="btnAddRoster" runat="server" Text="Add" />
                        </asp:Panel>
                    </ContentTemplate>
                </asp:TabPanel>
            </asp:TabContainer>
        </asp:View>
    </asp:MultiView>
    <uc1:ldap ID="ldap1" runat="server" />
</asp:Content>
