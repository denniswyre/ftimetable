<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Flex.Master"
    CodeBehind="managelecturer.aspx.vb" Inherits="FlexTimeTable.managelecturer" %>

<%@ Register Src="../userControl/ldap.ascx" TagName="ldap" TagPrefix="uc1" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register Src="../userControl/getDepartment.ascx" TagName="getDepartment" TagPrefix="uc2" %>
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
                <asp:Button ID="btnAddLecturer" runat="server" Text="Continue" Width="80px" />
                <asp:Button ID="btnCancelLecturer" runat="server" Text="Cancel" Width="80px" />
            </asp:Panel>
            <table width="100%">
                <tr>
                    <td width="50%" valign="top" align="left">
                        <asp:Panel ID="pnlLecturers" GroupingText="Department Lecturers" runat="server">
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
                    </td>
                    <td width="50%" valign="top" align="left">
                        <asp:Panel ID="pnlExtLecturers" GroupingText="External Lecturers" runat="server">
                            <asp:GridView ID="grdExtLecturer" runat="server" DataKeyNames="ID" Width="100%" AutoGenerateColumns="False">
                                <Columns>
                                    <asp:BoundField DataField="ID" HeaderText="ID" Visible="False" SortExpression="ID" />
                                    <asp:BoundField DataField="surname" HeaderText="Surname" SortExpression="surname" />
                                    <asp:BoundField DataField="firstname" HeaderText="First Name" SortExpression="firstname" />
                                    <asp:BoundField DataField="initials" HeaderText="M.I." SortExpression="initials" />
                                    <asp:BoundField DataField="title" HeaderText="Title" SortExpression="title" />
                                    <asp:BoundField DataField="username" HeaderText="Username" SortExpression="username" />
                                    <asp:ButtonField CommandName="transfer" Text="Transfer" />
                                    <asp:ButtonField CommandName="select" Text="Select" />
                                </Columns>
                            </asp:GridView>
                        </asp:Panel>
                    </td>
                </tr>
            </table>
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
                        <table width="100%" valign="top">
                            <tr>
                                <td>
                                    Search:<asp:TextBox ID="txtSubjectSearch" runat="server"></asp:TextBox><asp:Button
                                        ID="btnSubjectSearch" runat="server" Text="Search" /><br />
                                    <div id='hello' style="z-index: 102; left: 13px; overflow: auto; width: 350px; height: 300px">
                                        <asp:ListBox ID="lstAvailableSubjects" runat="server" Font-Size="Smaller" Width="700px"
                                            Height="600px"></asp:ListBox>
                                    </div>
                                </td>
                                <td align="center" valign="top">
                                    <br />
                                    <br />
                                    <br />
                                    <br />
                                    <asp:Button ID="btnAdd" runat="server" Text="Add" Width="80px" /><br />
                                    <asp:Button ID="btnRemove" runat="server" Text="Remove" Width="80px" />
                                </td>
                                <td valign="top">
                                    Selected Subjects
                                    <div id='Div1' style="z-index: 102; left: 13px; overflow: auto; width: 350px; height: 150px">
                                        <asp:ListBox ID="lstSelectedSubjects" runat="server" Font-Size="Smaller" Width="700px"
                                            Height="600px"></asp:ListBox>
                                    </div>
                                    External Subjects
                                    <div id='Div2' style="z-index: 102; left: 13px; overflow: auto; width: 350px; height: 150px">
                                        <asp:ListBox ID="lstExtSelectedSubjects" runat="server" Font-Size="Smaller" Width="700px"
                                            Height="600px"></asp:ListBox>
                                    </div>
                                </td>
                            </tr>
                        </table>
                    </ContentTemplate>
                </asp:TabPanel>
                <asp:TabPanel runat="server" HeaderText="Roster" ID="TabRoster">
                    <ContentTemplate>
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
                        <asp:Panel ID="pnlRosterButtons" GroupingText="Site Roster" runat="server">
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
