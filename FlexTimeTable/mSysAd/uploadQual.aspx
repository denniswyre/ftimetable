<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/NoAjaxFlex.Master"
    CodeBehind="uploadQual.aspx.vb" Inherits="FlexTimeTable.uploadQual" %>

<%@ Register Src="../userControl/uploadFile.ascx" TagName="uploadFile" TagPrefix="uc2" %>
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
        Upload Qualification Data</h3>
    <asp:Literal ID="litMessage" runat="server"></asp:Literal>
    <br />
    <asp:Literal ID="litFaculty" runat="server">Faculty:</asp:Literal>
    <asp:DropDownList ID="cboFaculty" AutoPostBack="true" runat="server">
    </asp:DropDownList>
    <br />
    <asp:Literal ID="Literal1" runat="server">School:</asp:Literal>
    <asp:DropDownList ID="cboSchool" runat="server">
    </asp:DropDownList>
    <asp:Panel ID="pnlUpload" runat="server">
        <uc2:uploadFile ID="uploadFile1" runat="server" />
    </asp:Panel>
</asp:Content>
