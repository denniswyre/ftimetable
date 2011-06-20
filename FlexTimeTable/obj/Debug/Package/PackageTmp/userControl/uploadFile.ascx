<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="uploadFile.ascx.vb"
    Inherits="FlexTimeTable.uploadFile" %>
<asp:Panel ID="pnlUpload" runat="server" GroupingText="Upload File">
    <asp:Literal ID="litMessage" runat="server"></asp:Literal>
    <asp:Wizard ID="WizLoadCVS" runat="server" ActiveStepIndex="0" SideBarStyle-Wrap="false"
        SideBarStyle-HorizontalAlign="Left" SideBarStyle-VerticalAlign="Top" DisplaySideBar="false"
        EnableTheming="True" FinishCompleteButtonText="" FinishCompleteButtonType="Link"
        FinishPreviousButtonText="" FinishPreviousButtonType="Link">
        <WizardSteps>
            <asp:WizardStep ID="WizStart" runat="server" Title="Select CSV File">
                <asp:FileUpload ID="FileUpload" runat="server" Width="300px" BorderStyle="Inset"
                    BorderWidth="2px" />
                <br />
                <br />
                <asp:Literal ID="litFields" runat="server"></asp:Literal>
            </asp:WizardStep>
            <asp:WizardStep ID="WizConfirm" runat="server" Title="Map Fields">
                <asp:Panel ID="Panel2" runat="server" Width="400px" ScrollBars="Auto">
                    <h4>Verify file Columns</h4>
                    <asp:GridView ID="GridView1" runat="server" PageSize="5" Width="100%" AllowPaging="True">
                        <PagerTemplate>
                            <table width="100%">
                                <tr>
                                    <td>
                                        <asp:Literal ID="litNull" runat="server"></asp:Literal>
                                    </td>
                                </tr>
                            </table>
                        </PagerTemplate>
                    </asp:GridView>
                                   </asp:Panel>
            </asp:WizardStep>
            <asp:WizardStep ID="WizComplete" runat="server" Title="Final">
            <h4>List of Errors Found!!</h4>
                <asp:ListBox ID="lstError" runat="server"></asp:ListBox>
            </asp:WizardStep>
        </WizardSteps>
    </asp:Wizard>
</asp:Panel>
