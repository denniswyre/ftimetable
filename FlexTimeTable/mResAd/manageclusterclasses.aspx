<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Flex.Master"
    CodeBehind="manageclusterclasses.aspx.vb" Inherits="FlexTimeTable.manageclusterclasses" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register Src="../userControl/ClassLecturer.ascx" TagName="ClassLecturer" TagPrefix="uc2" %>
<%@ Register Src="../userControl/classresource.ascx" TagName="classresource" TagPrefix="uc3" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h3>
        Manage Subject Classes</h3>
    <asp:Literal ID="litMessage" runat="server"></asp:Literal>
    <br />
    <asp:Panel ID="pnlMain" runat="server">
        <table>
            <tr>
                <td width="200px">
                    <asp:Literal ID="litDepartment" runat="server"></asp:Literal>
                </td>
                <td>
                    <asp:DropDownList ID="cboDepartments" runat="server" AutoPostBack="true" Width="300px">
                    </asp:DropDownList>
                    <asp:DropDownList ID="cboFaculty" runat="server" AutoPostBack="true" Width="200px">
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td width="200px">
                    <asp:Literal ID="litSiteCluster" runat="server"></asp:Literal>
                </td>
                <td>
                    <asp:DropDownList ID="cboCluster" runat="server" AutoPostBack="True" Width="500px">
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td width="200px">
                    <asp:Literal ID="litSubject" runat="server"></asp:Literal>
                </td>
                <td>
                    <asp:DropDownList ID="cboSubject" runat="server" AutoPostBack="True" Width="500px">
                    </asp:DropDownList>
                </td>
            </tr>
        </table>
    </asp:Panel>
    <br />
    <asp:Panel ID="pnlSubjectDetail" runat="server" Width="100%">
        <table width="100%">
            <tr>
                <th width="50%" align="left">
                    Subject Details
                </th>
                <th width="50%"  align="left">
                    Associated Qualifications
                </th>
            </tr>
            <tr>
                <td width="50%">
                    <asp:Literal ID="litSubjectDetails" runat="server"></asp:Literal>
                </td>
                <td width="50%">
                    <asp:Literal ID="litQual" runat="server"></asp:Literal>
                    <asp:ListBox ID="lstQualification" runat="server"></asp:ListBox>
                </td>
            </tr>
        </table>
    </asp:Panel>
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <asp:TabContainer ID="TabClass" runat="server" ActiveTabIndex="0">
        <asp:TabPanel ID="tab0" runat="server" HeaderText="Class Groups">
            <ContentTemplate>
                <asp:LinkButton ID="lnkClass" runat="server">New Class Group</asp:LinkButton>
                <asp:GridView ID="grdClasses" runat="server" DataKeyNames="ID" AutoGenerateColumns="False">
                    <Columns>
                        <asp:BoundField DataField="ID" HeaderText="ID" Visible="false" SortExpression="ID" />
                        <asp:BoundField DataField="Code" HeaderText="Code" SortExpression="Code" />
                        <asp:BoundField DataField="classSize" HeaderText="Size" SortExpression="classSize" />
                        <asp:BoundField DataField="TimeSlotTotal" HeaderText="Slots" SortExpression="TimeSlotTotal" />
                        <asp:TemplateField HeaderText="Block" SortExpression="AcademicBlockID">
                            <ItemTemplate>
                                <%# displayBlock(Eval("AcademicBlockID"))%>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Offering" SortExpression="OfferingTypeID">
                            <ItemTemplate>
                                <%# DisplayOffering(Eval("OfferingTypeID"))%>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:CommandField ShowSelectButton="True" />
                    </Columns>
                </asp:GridView>
            </ContentTemplate>
        </asp:TabPanel>
        <asp:TabPanel ID="tab1" runat="server" HeaderText="Class Details">
            <ContentTemplate>
                   <table>
                    <tr>
                        <td>
                            ID:
                        </td>
                        <td>
                            <asp:Label ID="lblID" runat="server" Text=""></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Code:
                        </td>
                        <td>
                            <asp:TextBox ID="txtCode" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Offering Type:
                        </td>
                        <td>
                            <asp:DropDownList ID="cboOffering" runat="server">
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Academic Block:
                        </td>
                        <td>
                            <asp:DropDownList ID="cboBlock" runat="server">
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Class Size:
                        </td>
                        <td>
                            <asp:TextBox ID="txtSize" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="Label1" runat="server" Text="TimeSlots:" ToolTip="Number of TimeSlots per week"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="cboTimeSlots" runat="server">
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2" align="center">
                            <asp:Panel ID="pnlControl" runat="server">
                                <asp:Button ID="btnSave" runat="server" Text="Save" />
                                <asp:Button ID="btnDelete" runat="server" Text="Delete" />
                                <asp:Button ID="btnReturn" runat="server" Text="Return" />
                                <asp:Button ID="btnEdit" runat="server" Text="Edit" />
                              </asp:Panel>
                        </td>
                    </tr>
                </table>
                
            </ContentTemplate>
        </asp:TabPanel>
        <asp:TabPanel ID="Tab2" runat="server" HeaderText="Class Lecturer">
            <ContentTemplate>
                <uc2:ClassLecturer ID="ClassLecturer1" runat="server" />
            </ContentTemplate>
        </asp:TabPanel>
        <asp:TabPanel ID="Tab3" runat="server" HeaderText="Class Resources">
            <ContentTemplate>
                <uc3:classresource id="classresource1" runat="server" />
            </ContentTemplate>
        </asp:TabPanel>
    </asp:TabContainer>
</asp:Content>
