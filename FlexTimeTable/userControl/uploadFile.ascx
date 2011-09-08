<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="uploadFile.ascx.vb"
    Inherits="FlexTimeTable.uploadFile" %>
<asp:Panel ID="pnlUpload" runat="server" GroupingText="Upload File">
    <asp:Literal ID="litMessage" runat="server"></asp:Literal>
    <asp:Wizard ID="WizLoadCVS" runat="server" ActiveStepIndex="0" SideBarStyle-Wrap="false"
        SideBarStyle-HorizontalAlign="Left" SideBarStyle-VerticalAlign="Top" DisplaySideBar="false"
        EnableTheming="True" FinishCompleteButtonText="" FinishCompleteButtonType="Link"
        FinishPreviousButtonText="" FinishPreviousButtonType="Link">
        <SideBarStyle HorizontalAlign="Left" VerticalAlign="Top" Wrap="False" />
        <WizardSteps>
            <asp:WizardStep ID="WizStart" runat="server" Title="Select CSV File">
                <asp:FileUpload ID="FileUpload" runat="server" BorderStyle="Inset" 
                    BorderWidth="2px" Width="600px" />
                <br />
                <br />
                <asp:Literal ID="litFields" runat="server"></asp:Literal>
            </asp:WizardStep>
            <asp:WizardStep ID="WizConfirm" runat="server" Title="Map Fields">
                <asp:Panel ID="Panel2" runat="server" ScrollBars="Auto" Width="400px">
                    <h4>
                        Verify file Columns</h4>
                    <asp:GridView ID="GridView1" runat="server" AllowPaging="True" PageSize="5" 
                        Width="100%">
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
                <h4>
                    List of Errors Found!!</h4>
                <asp:ListBox ID="lstError" runat="server"></asp:ListBox>
            </asp:WizardStep>
        </WizardSteps>
    </asp:Wizard>
</asp:Panel>
