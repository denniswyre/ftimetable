<%@ Page Title="Home Page" Language="vb" MasterPageFile="~/Flex.Master" AutoEventWireup="false"
    CodeBehind="Default.aspx.vb" Inherits="FlexTimeTable._Default" %>

<%@ Register Src="userControl/qualificationsearch.ascx" TagName="qualificationsearch"
    TagPrefix="uc1" %>
<%@ Register Src="userControl/subjectsearch.ascx" TagName="subjectsearch" TagPrefix="uc2" %>
<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <h2>
        WSU TimeTable System
    </h2>
    <asp:MultiView ID="mvGeneral" runat="server" ActiveViewIndex="0">
        <asp:View ID="vwMain" runat="server">
            <asp:Literal ID="litMessage" runat="server"></asp:Literal>
            <div>
                <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="~/Changes.aspx" Target="_blank">Click Here to View Summary of Changes Made</asp:HyperLink>
            </div>
            <table>
                <tr>
                    <td>
                        <asp:Label ID="lblStatus" runat="server"></asp:Label>
                    </td>
                    <td>
                        <asp:Label ID="lblCluster" runat="server"></asp:Label>
                        <asp:Label ID="lblObject0" runat="server"></asp:Label>
                        <asp:Label ID="lblObject1" runat="server"></asp:Label>
                        <asp:HiddenField ID="hdnType" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td valign="top" style="margin-left: 120px">
                        <asp:Calendar ID="Calendar1" runat="server" BackColor="White" BorderColor="#999999"
                            CellPadding="4" DayNameFormat="Shortest" Font-Names="Verdana" Font-Size="8pt"
                            ForeColor="Black" Height="180px" Width="200px">
                            <DayHeaderStyle BackColor="#CCCCCC" Font-Bold="True" Font-Size="7pt" />
                            <NextPrevStyle VerticalAlign="Bottom" />
                            <OtherMonthDayStyle ForeColor="#808080" />
                            <SelectedDayStyle BackColor="#666666" Font-Bold="True" ForeColor="White" />
                            <SelectorStyle BackColor="#CCCCCC" />
                            <TitleStyle BackColor="#999999" BorderColor="Black" Font-Bold="True" />
                            <TodayDayStyle BackColor="#CCCCCC" ForeColor="Black" />
                            <WeekendDayStyle BackColor="#FFFFCC" />
                        </asp:Calendar>
                        <br />
                        <table>
                            <tr>
                                <td valign="top" align="left">
                                    Type:
                                </td>
                                <td valign="top" align="left">
                                    <asp:DropDownList ID="cboType" runat="server" AutoPostBack="True" Width="100px">
                                        <asp:ListItem>Qualification</asp:ListItem>
                                        <asp:ListItem>Subject</asp:ListItem>
                                        <asp:ListItem>Venue</asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                            </tr>
                            <tr>
                                <td valign="top" align="left">
                                    Cluster:
                                </td>
                                <td valign="top" align="left">
                                    <asp:DropDownList ID="cboCluster" runat="server" AutoPostBack="True" Width="100px">
                                    </asp:DropDownList>
                                </td>
                            </tr>
                            <asp:PlaceHolder ID="phQual" runat="server">
                                <tr>
                                    <td valign="top" align="left">
                                        Qual:
                                    </td>
                                    <td valign="top" align="left">
                                        <asp:Label ID="lblQualCode" runat="server" Font-Bold="true" Text=" " Height="18px"
                                            Width="80px" BorderWidth="1" BorderStyle="Solid"></asp:Label>
                                        <asp:Button ID="btnQual" runat="server" Text="" Height="18px" Width="20px" />
                                    </td>
                                </tr>
                                <tr>
                                    <td valign="top" align="left">
                                        Level:
                                    </td>
                                    <td valign="top" align="left">
                                        <asp:DropDownList ID="cboLevel" runat="server" Width="100px">
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                            </asp:PlaceHolder>
                            <asp:PlaceHolder ID="phClass" runat="server">
                                <tr>
                                    <td valign="top" align="left">
                                        Subject:
                                    </td>
                                    <td valign="top" align="left">
                                        <asp:Label ID="lblSubjectCode" runat="server" Text="" Font-Bold="true" Height="18px"
                                            Width="80px" BorderWidth="1" BorderStyle="Solid"></asp:Label>
                                        <asp:Button ID="btnSubject" runat="server" Height="18px" Text="" Width="20px" />
                                    </td>
                                </tr>
                                <tr>
                                    <td valign="top" align="left">
                                        Class:
                                    </td>
                                    <td valign="top" align="left">
                                        <asp:DropDownList ID="cboClassgroup" runat="server" Width="100px">
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                            </asp:PlaceHolder>
                            <asp:PlaceHolder ID="phVenue" runat="server">
                                <tr>
                                    <td valign="top" align="left">
                                        Site:
                                    </td>
                                    <td valign="top" align="left">
                                        <asp:DropDownList ID="cboSite" runat="server" AutoPostBack="True" Width="100px">
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                                <tr>
                                    <td valign="top" align="left">
                                        Room:
                                    </td>
                                    <td valign="top" align="left">
                                        <asp:DropDownList ID="cboRoom" runat="server" Width="100px" Font-Strikeout="False">
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                            </asp:PlaceHolder>
                            <tr>
                                <td valign="top" align="left">
                                </td>
                                <td valign="top" align="left">
                                    <asp:Button ID="btnControl" runat="server" Text="Display" />
                                </td>
                            </tr>
                        </table>
                    </td>
                    <td valign="top" align="left">
                        <asp:Panel ID="pnlDisplay" Width="600px" runat="server">
                            <asp:MultiView ID="mvTimetable" runat="server">
                                <asp:View ID="vwDisplay" runat="server">
                                    <asp:GridView ID="grdTimeTable" runat="server" DataKeyNames="TimeSlotID" AutoGenerateColumns="False"
                                        BackColor="#CCCCCC" BorderColor="#999999" BorderStyle="Solid" BorderWidth="3px"
                                        CellPadding="4" CellSpacing="2" ForeColor="Black">
                                        <Columns>
                                            <asp:TemplateField HeaderText="Time">
                                                <ItemTemplate>
                                                    <%# DisplayTimeHeader(Eval("TimeSlotID"))%>
                                                </ItemTemplate>
                                                <HeaderStyle Font-Size="XX-Small" />
                                                <ItemStyle Font-Size="XX-Small" />
                                            </asp:TemplateField>
                                            <asp:ButtonField DataTextField="slot1" CommandName="1" ItemStyle-HorizontalAlign="Center"
                                                HeaderText="Sun" SortExpression="slot1">
                                                <HeaderStyle HorizontalAlign="Center" />
                                                <ItemStyle HorizontalAlign="Center" Font-Size="X-Small" />
                                            </asp:ButtonField>
                                            <asp:ButtonField DataTextField="slot2" CommandName="2" ItemStyle-HorizontalAlign="Center"
                                                HeaderText="Mon" SortExpression="slot2">
                                                <HeaderStyle HorizontalAlign="Center" />
                                                <ItemStyle HorizontalAlign="Center" Font-Size="X-Small" />
                                            </asp:ButtonField>
                                            <asp:ButtonField DataTextField="slot3" CommandName="3" ItemStyle-HorizontalAlign="Center"
                                                HeaderText="Tue" SortExpression="slot3">
                                                <HeaderStyle HorizontalAlign="Center" />
                                                <ItemStyle HorizontalAlign="Center" Font-Size="X-Small" />
                                            </asp:ButtonField>
                                            <asp:ButtonField DataTextField="slot4" CommandName="4" ItemStyle-HorizontalAlign="Center"
                                                HeaderText="Wed" SortExpression="slot4">
                                                <HeaderStyle HorizontalAlign="Center" />
                                                <ItemStyle HorizontalAlign="Center" Font-Size="X-Small" />
                                            </asp:ButtonField>
                                            <asp:ButtonField DataTextField="slot5" CommandName="5" ItemStyle-HorizontalAlign="Center"
                                                HeaderText="Thu" SortExpression="slot5">
                                                <HeaderStyle HorizontalAlign="Center" />
                                                <ItemStyle HorizontalAlign="Center" Font-Size="X-Small" />
                                            </asp:ButtonField>
                                            <asp:ButtonField DataTextField="slot6" CommandName="6" ItemStyle-HorizontalAlign="Center"
                                                HeaderText="Fri" SortExpression="slot6">
                                                <HeaderStyle HorizontalAlign="Center" />
                                                <ItemStyle HorizontalAlign="Center" Font-Size="X-Small" />
                                            </asp:ButtonField>
                                            <asp:ButtonField DataTextField="slot7" CommandName="7" ItemStyle-HorizontalAlign="Center"
                                                HeaderText="Sat" SortExpression="slot7">
                                                <HeaderStyle HorizontalAlign="Center" />
                                                <ItemStyle HorizontalAlign="Center" Font-Size="X-Small" />
                                            </asp:ButtonField>
                                        </Columns>
                                        <FooterStyle BackColor="#CCCCCC" />
                                        <HeaderStyle BackColor="Black" Font-Bold="True" ForeColor="White" />
                                        <PagerStyle BackColor="#CCCCCC" ForeColor="Black" HorizontalAlign="Left" />
                                        <RowStyle BackColor="White" />
                                        <SelectedRowStyle BackColor="#000099" Font-Bold="True" ForeColor="White" />
                                        <SortedAscendingCellStyle BackColor="#F1F1F1" />
                                        <SortedAscendingHeaderStyle BackColor="#808080" />
                                        <SortedDescendingCellStyle BackColor="#CAC9C9" />
                                        <SortedDescendingHeaderStyle BackColor="#383838" />
                                    </asp:GridView>
                                </asp:View>
                                <asp:View ID="vwDetail" runat="server">
                                    <asp:Panel ID="pnlDetails" GroupingText="Slot Details" runat="server">
                                        <asp:Button ID="btnReturn" runat="server" Text="Return" />
                                        <asp:Literal ID="litHeader" runat="server"></asp:Literal>
                                        <asp:Repeater ID="repDetails" runat="server">
                                            <ItemTemplate>
                                                <table border="1" width="100%">
                                                    <tr>
                                                        <td>
                                                            Class:
                                                        </td>
                                                        <td>
                                                            <%# Container.DataItem("class")%>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            Subject:
                                                        </td>
                                                        <td>
                                                            <%# Container.DataItem("subject")%>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            Venue:
                                                        </td>
                                                        <td>
                                                            <%# Container.DataItem("venue")%>
                                                        </td>
                                                    </tr>
                                                    <asp:PlaceHolder ID="phLecturer" runat="server">
                                                        <tr>
                                                            <td>
                                                                Lecturer:
                                                            </td>
                                                            <td>
                                                                <%# Container.DataItem("lecturer")%>
                                                            </td>
                                                        </tr>
                                                    </asp:PlaceHolder>
                                                </table>
                                                <br />
                                            </ItemTemplate>
                                        </asp:Repeater>
                                        <asp:Literal ID="litFooter" runat="server"></asp:Literal>
                                    </asp:Panel>
                                </asp:View>
                            </asp:MultiView>
                        </asp:Panel>
                    </td>
                </tr>
            </table>
        </asp:View>
        <asp:View ID="vwQualification" runat="server">
            <uc1:qualificationsearch ID="ucQqualificationSearch" runat="server" />
        </asp:View>
        <asp:View ID="vwSubject" runat="server">
            <uc2:subjectsearch ID="ucSubjectSearch" runat="server" />
        </asp:View>
    </asp:MultiView>
</asp:Content>
