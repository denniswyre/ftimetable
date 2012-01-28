<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="classgroups.ascx.vb"
    Inherits="FlexTimeTable.classgroups" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register Src="~/userControl/ClassLecturer.ascx" TagName="ClassLecturer" TagPrefix="uc2" %>
<%@ Register Src="~/userControl/classresource.ascx" TagName="classresource" TagPrefix="uc3" %>
<asp:Panel ID="pnlMain" runat="server">
    <asp:Literal ID="litMessage" runat="server"></asp:Literal>
    <asp:Panel ID="pnlQualification" runat="server" GroupingText="Associated Qualifications"
        Width="100%">
        <asp:ListBox ID="lstQualification" runat="server" Width="100%" Height="60px"></asp:ListBox>
    </asp:Panel>
    <asp:Panel ID="pnlCluster" runat="server" GroupingText="Delivery Clusters" Width="100%">
        <asp:MultiView ID="mvCluster" runat="server" ActiveViewIndex="0">
            <asp:View ID="vwClusterView" runat="server">
                <div style="margin-top: -20px">
                    <asp:LinkButton ID="btnClusterEdit" runat="server">Edit</asp:LinkButton></div>
                <asp:DropDownList ID="cboCluster" runat="server" AutoPostBack="True" Width="100%">
                </asp:DropDownList>
            </asp:View>
            <asp:View ID="vwClusterEdit" runat="server">
                <div style="float: right; margin-top: -20px">
                    <asp:LinkButton ID="btnClusterReturn" runat="server">Close</asp:LinkButton>
                </div>
                <div style="clear: both">
                </div>
                <div style="margin-top: -10px">
                    <table>
                        <tr>
                            <td>
                                <asp:Label ID="lblAllCluster" runat="server" Text="ALL Clusters" Font-Bold="True"
                                    Font-Underline="True" Width="200px" Font-Size="Small"></asp:Label><br />
                                <asp:ListBox ID="lstAllClusters" runat="server" Width="200px" Height="100px"></asp:ListBox>
                            </td>
                            <td>
                                <asp:Button ID="btnAddCluster" runat="server" Text="Add" Width="100px" /><br />
                                <asp:Button ID="btnRemCluster" runat="server" Text="Remove" Width="100px" />
                            </td>
                            <td>
                                <asp:Label ID="lblSelCluster" runat="server" Text="Selected Clusters" Font-Bold="True"
                                    Font-Underline="True" Width="200px" Font-Size="Small"></asp:Label><br />
                                <asp:ListBox ID="lstSelClusters" runat="server" Width="200px" Height="100px"></asp:ListBox>
                            </td>
                        </tr>
                    </table>
                </div>
            </asp:View>
        </asp:MultiView>
    </asp:Panel>
    <asp:Panel ID="pnlClass" Width="100%" GroupingText="Class Groups" runat="server">
        <asp:MultiView ID="mvClass" ActiveViewIndex="0" runat="server">
            <asp:View ID="vwClassList" runat="server">
                <asp:LinkButton ID="btnCreateClass" runat="server">New Class Group</asp:LinkButton>
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
            </asp:View>
            <asp:View ID="vwClassDetail" runat="server">
                <div style="float: right">
                    <a href="#top">Top</a>&nbsp;&nbsp; <a href="#bottom">Bottom</a> &nbsp;&nbsp;
                    <asp:LinkButton ID="btnEdit" runat="server">Edit</asp:LinkButton>&nbsp;&nbsp;
                    <asp:LinkButton ID="btnReturn" runat="server">Close</asp:LinkButton>
                </div>
                <div style="clear: both">
                </div>
                <asp:Panel ID="pnlClassDetail" GroupingText="Class Detail" Width="100%" runat="server">
                    <table>
                        <tr>
                            <td>
                                ID:
                            </td>
                            <td>
                                <asp:Label ID="lblID" runat="server"></asp:Label>
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
                                Class Size:
                            </td>
                            <td>
                                <asp:TextBox ID="txtSize" runat="server"></asp:TextBox>
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
                                <asp:Label ID="Label1" runat="server" Text="TimeSlots:" ToolTip="Number of TimeSlots per week"></asp:Label>
                            </td>
                            <td>
                                <asp:DropDownList ID="cboTimeSlots" runat="server">
                                </asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2">
                                <asp:Panel ID="pnlControl" runat="server">
                                    <asp:Button ID="btnSave" runat="server" Text="Save"></asp:Button>
                                    <asp:Button ID="btnDelete" runat="server" Text="Delete"></asp:Button>
                                    <asp:Button ID="btnCancel" runat="server" Text="Cancel"></asp:Button>
                                </asp:Panel>
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
                <a name="bottom"></a>
                <uc2:ClassLecturer ID="ucClassLecturer" runat="server" />
                <uc3:classresource id="ucClassResource" runat="server" />
            </asp:View>
        </asp:MultiView>
    </asp:Panel>
</asp:Panel>
