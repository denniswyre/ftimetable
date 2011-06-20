<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Flex.Master"
    CodeBehind="spaceutilisation.aspx.vb" Inherits="FlexTimeTable.spaceutilisation" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h3>
        Space Utilisation</h3>
    <div>
        Campus:<asp:DropDownList ID="cboCampus" AutoPostBack="true" runat="server">
        </asp:DropDownList>
        <br />
        Resource Type:<asp:DropDownList ID="cboType" AutoPostBack="true" runat="server">
        </asp:DropDownList>
        <br />
        Offering Type:<asp:DropDownList ID="cboOfferingType" AutoPostBack="true" runat="server">
        </asp:DropDownList>
        <br />
        <asp:Literal ID="litSummary" runat="server"></asp:Literal><br />
        <asp:GridView ID="grdUtilize" runat="server">
        </asp:GridView>
    </div>
</asp:Content>
