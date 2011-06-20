<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Flex.Master"
    CodeBehind="managesites.aspx.vb" Inherits="FlexTimeTable.managesites" %>

<%@ Register Src="~/userControl/logButton.ascx" TagName="logButton" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style type="text/css">
        .style1
        {
            width: 92px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h3>
        Manage Sites</h3>
    <asp:Literal ID="litCluster" runat="server"></asp:Literal><asp:DropDownList ID="cboCluster" AutoPostBack="true" runat="server">
    </asp:DropDownList>
    <asp:DropDownList ID="cboCampus" runat="server" AutoPostBack="True">
    </asp:DropDownList>
    <br />
    <asp:MultiView ID="mvSite" runat="server">
        <asp:View ID="vwGrid" runat="server">
       
       
    <asp:LinkButton ID="lnkCreate" runat="server">New Site</asp:LinkButton>
    <asp:GridView ID="grdSite" runat="server" AutoGenerateColumns="False" DataKeyNames="ID">
        <Columns>
            <asp:BoundField DataField="ID" HeaderText="ID" SortExpression="ID" />
            <asp:BoundField DataField="shortName" HeaderText="Code" SortExpression="shortName" />
            <asp:BoundField DataField="longName" HeaderText="Name" SortExpression="longName" />
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
                    <asp:Label ID="Label2" runat="server" AssociatedControlID="txtShortName" Text="Code:"></asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="txtShortName" runat="server"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td class="style1">
                    <asp:Label ID="Label1" runat="server" AssociatedControlID="txtLongName" Text="Name:"></asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="txtLongName" runat="server"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td class="style1">
                    <asp:Label ID="Label3" runat="server" AssociatedControlID="txtAddress" Text="Address:"></asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="txtAddress" runat="server"></asp:TextBox>
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
