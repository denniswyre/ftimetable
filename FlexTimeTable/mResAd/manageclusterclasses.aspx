<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Flex.Master"
    CodeBehind="manageclusterclasses.aspx.vb" Inherits="FlexTimeTable.manageclusterclasses" %>

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
                <td>
                    <asp:Literal ID="litDepartment" runat="server"></asp:Literal>
                </td>
                <td>
                    <asp:DropDownList ID="cboDepartments" runat="server" AutoPostBack="true">
                    </asp:DropDownList>
                    <asp:DropDownList ID="cboFaculty" runat="server" AutoPostBack="true">
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Literal ID="litSiteCluster" runat="server"></asp:Literal>
                </td>
                <td>
                    <asp:DropDownList ID="cboCluster" runat="server" AutoPostBack="True">
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Literal ID="litSubject" runat="server"></asp:Literal>
                </td>
                <td>
                    <asp:DropDownList ID="cboSubject" runat="server" AutoPostBack="True">
                    </asp:DropDownList>
                </td>
            </tr>
        </table>
    </asp:Panel>
    <br />
    <asp:Panel ID="pnlSubjectDetail" runat="server">
        <table>
            <tr>
                <th class="style1">
                    Subject Details
                </th>
                <th>
                    Associated Qualifications
                </th>
            </tr>
            <tr>
                <td class="style1">
                    <asp:Literal ID="litSubjectDetails" runat="server"></asp:Literal>
                </td>
                <td>
                    <asp:Literal ID="litQual" runat="server"></asp:Literal>
                    <asp:ListBox ID="lstQualification" runat="server"></asp:ListBox>
                </td>
            </tr>
        </table>
    </asp:Panel>
    <asp:MultiView ID="Pages" runat="server">
        <asp:View ID="PageClasses" runat="server">
            <asp:Panel ID="pnlDetails" runat="server" GroupingText="Class Groups">
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
            </asp:Panel>
        </asp:View>
        <asp:View ID="PageClassDetails" runat="server">
            <asp:Panel ID="Panel1" GroupingText="Class View" runat="server">
                <asp:LinkButton ID="btnClassEdit2" runat="server">Edit</asp:LinkButton>&nbsp;&nbsp;
                <asp:LinkButton ID="btnClassCancel" runat="server">Cancel</asp:LinkButton>
                <br />
                <asp:Literal ID="litClassDetails" runat="server"></asp:Literal>
            </asp:Panel>
            <uc2:ClassLecturer ID="ClassLecturer1" runat="server" />
            <uc3:classresource ID="classresource1" runat="server" />
        </asp:View>
        <asp:View ID="pageClassEdit" runat="server">
            <asp:Panel ID="pnlClass" GroupingText="Edit Class" runat="server">
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
                                <asp:Button ID="btnCancel" runat="server" Text="Cancel" />
                            </asp:Panel>
                        </td>
                    </tr>
                </table>
            </asp:Panel>
        </asp:View>
    </asp:MultiView>
</asp:Content>
