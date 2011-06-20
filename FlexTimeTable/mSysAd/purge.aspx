<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Flex.Master"
    CodeBehind="purge.aspx.vb" Inherits="FlexTimeTable.purge" %>

<%@ Register src="../userControl/ldap.ascx" tagname="ldap" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h3>
        Purge Data</h3>
    <asp:RadioButtonList ID="optOption" runat="server">
        <asp:ListItem>Sites</asp:ListItem>
        <asp:ListItem>Academic</asp:ListItem>
        <asp:ListItem>Class</asp:ListItem>
        <asp:ListItem>TimeTable</asp:ListItem>
    </asp:RadioButtonList>
    <asp:Literal ID="Message" runat="server"></asp:Literal><br />
    <asp:CheckBox ID="CheckBox1" runat="server" Text="Are you sure you want to purge the data?" />&nbsp;
    <br />
    This is a dangerous operation.&nbsp; If you are really certain Enter your 
    password
    <br />
    and then Press the Purge Button.&nbsp; Purging is necessary when you want to 
    upload fresh data<br />
    Password:<asp:TextBox ID="txtwordpass" runat="server" TextMode="Password" 
        AutoCompleteType="Disabled"></asp:TextBox>
    <br />
    <asp:Button ID="btnPurge" runat="server" Text="Purge" />
    <uc1:ldap ID="ldap1" runat="server" />
    <br />
    </asp:Content>
