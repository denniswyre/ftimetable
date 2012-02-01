<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Flex.Master"
    CodeBehind="generateTimetable.aspx.vb" Inherits="FlexTimeTable.generateTimetable" %>
  
<%@ Register src="../userControl/purgeData.ascx" tagname="purgeData" tagprefix="uc1" %>
<%@ Register src="../userControl/createResources.ascx" tagname="createResources" tagprefix="uc2" %>
  
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h1>
        Generate Time Table
    </h1>
    <asp:Literal ID="litError" runat="server"></asp:Literal>
       <uc2:createResources ID="createResources1" runat="server" />
    <uc1:purgeData ID="purgeData1" runat="server" />
       <asp:Panel ID="pnlGenerate" runat="server" GroupingText="Generate TimeTable">
        <table style="width: 100%;">
            <tr>
                <td>
                    Year:</td>
                <td>
                    <asp:DropDownList ID="cboYear" AutoPostBack="true" runat="server">
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td>
                    Block:</td>
                <td>
                    <asp:DropDownList ID="cboBlock" runat="server">
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td>
                    Cluster(s):</td>
                <td>
                    <asp:CheckBox ID="chkAllCluster" runat="server" Text="All" />
                    <asp:DropDownList ID="cboCluster" runat="server">
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td>
                    Faculty:</td>
                <td>
                    <asp:CheckBox ID="chkAllFaculty" runat="server" Text="All" />
                    <asp:DropDownList ID="cboFaculty" runat="server">
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
            <td colspan="2" align="center">
                <asp:Button ID="btnGenerate" runat="server" Text="Generate" Width="150px" />    
            </td>
            </tr>
        </table>
    </asp:Panel>
</asp:Content>
