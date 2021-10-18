<%@ Page Title="MyMembership" Language="C#" MasterPageFile="~/mTSoar.Master" AutoEventWireup="true" CodeBehind="MyMembership.aspx.cs" Inherits="TSoar.MemberPages.MyMembership.MyMembership" %>

<asp:Content ID="HeaderContent" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="server">
    <asp:Label runat="server" Text="Inspect a club member's data stored on the PSSA Website" Font-Italic="true" />
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="ContentPlaceHolderMain" runat="server">
    <div class="gvclass">
    <ajaxToolkit:Accordion
        ID="AccordionDBMaint"
        runat="Server"
        SelectedIndex="0"
        HeaderCssClass="accordionHeader"
        HeaderSelectedCssClass="accordionHeaderSelected"
        ContentCssClass="accordionContent"
        AutoSize="None"
        FadeTransitions="true"
        TransitionDuration="250"
        FramesPerSecond="40"
        RequireOpenedPane="false">
        <Panes>
            <ajaxToolkit:AccordionPane runat="server" >
                <Header> Club membership: member category and dates </Header>
                <Content>
                    <asp:GridView ID="gvClubMembership" runat="server" AutoGenerateColumns="False"
                        GridLines="None" CssClass="SoarNPGridStyle" HorizontalAlign="Center"
                        EmptyDataText="-=> No Data Found <=-" ShowHeaderWhenEmpty="true">
                        <Columns>
                            <asp:TemplateField>
                                <HeaderTemplate>Member Display Name</HeaderTemplate>
                                <ItemTemplate><asp:Label ID="lblDispName" runat="server" Text='<%# Eval("Name") %>' /></ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="Category" HeaderText="Club Membership Category" />
                            <asp:BoundField DataField="Begin" HeaderText="Begin Date" DataFormatString="{0:yyyy/MM/dd}" />
                            <asp:BoundField DataField="End" HeaderText="End Date" DataFormatString="{0:yyyy/MM/dd}" />
                            <asp:TemplateField>
                                <HeaderTemplate>Notes</HeaderTemplate>
                                <ItemTemplate><asp:Label ID="lblNotes" runat="server" Text='<%# Eval("Note") %>' /></ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </Content>
            </ajaxToolkit:AccordionPane>        
            <ajaxToolkit:AccordionPane runat="server" >
                <Header> Soaring Society of America (SSA) membership: member category and dates </Header>
                <Content>
                        <asp:GridView ID="gvSSAMembership" runat="server" AutoGenerateColumns="False"
                            GridLines="None" CssClass="SoarNPGridStyle" HorizontalAlign="Center"
                            EmptyDataText="-=> No Data Found <=-" ShowHeaderWhenEmpty="true">
                            <Columns>
                            <asp:TemplateField>
                                <HeaderTemplate>Member Display Name</HeaderTemplate>
                                <ItemTemplate><asp:Label ID="lblDispName" runat="server" Text='<%# Eval("Name") %>' /></ItemTemplate>
                            </asp:TemplateField>
                                <asp:BoundField DataField="SSA_ID" HeaderText="SSA Member Identifier" />
                                <asp:BoundField DataField="Category" HeaderText="SSA Membership Category" />
                                <asp:BoundField DataField="Begin" HeaderText="Begin Date" DataFormatString="{0:yyyy/MM/dd}" />
                                <asp:BoundField DataField="End" HeaderText="End Date" DataFormatString="{0:yyyy/MM/dd}" />
                                <asp:BoundField DataField="Expires" HeaderText="Expires on" DataFormatString="{0:yyyy/MM/dd}" />
                                <asp:BoundField DataField="RenewsWithChapter" HeaderText="Renews Membership with Chapter" />
                            <asp:TemplateField>
                                <HeaderTemplate>Name of SSA Chapter of Affiliation</HeaderTemplate>
                                <ItemTemplate><asp:Label ID="lblAffil" runat="server" Text='<%# Eval("ChapterAffiliation") %>' /></ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField>
                                <HeaderTemplate>Notes</HeaderTemplate>
                                <ItemTemplate><asp:Label ID="lblNotes" runat="server" Text='<%# Eval("Notes") %>' /></ItemTemplate>
                            </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                </Content>
            </ajaxToolkit:AccordionPane>
            <ajaxToolkit:AccordionPane runat="server" >
                <Header> Member contact data </Header>
                <Content>
                    <h4>Simple Contact Data</h4>
                        <asp:GridView ID="gvContacts" runat="server" AutoGenerateColumns="False"
                            GridLines="None" CssClass="SoarNPGridStyle" HorizontalAlign="Center"
                            EmptyDataText="-=> No Data Found <=-" ShowHeaderWhenEmpty="true">
                            <Columns>
                            <asp:TemplateField>
                                <HeaderTemplate>Member Display Name</HeaderTemplate>
                                <ItemTemplate><asp:Label ID="lblDispName" runat="server" Text='<%# Eval("Name") %>' /></ItemTemplate>
                            </asp:TemplateField>
                                <asp:BoundField DataField="ContactType" HeaderText="Contact Type" />
                                <asp:BoundField DataField="Begin" HeaderText="Begin Date" DataFormatString="{0:yyyy/MM/dd}" />
                                <asp:BoundField DataField="End" HeaderText="End Date" DataFormatString="{0:yyyy/MM/dd}" />
                                <asp:BoundField DataField="Info" HeaderText="Contact Data" />
                            </Columns>
                        </asp:GridView>
                    <h4>Contact Data with Physical Addresses</h4>
                        <asp:GridView ID="gvPhysAddr" runat="server" AutoGenerateColumns="False"
                            GridLines="None" CssClass="SoarNPGridStyle" HorizontalAlign="Center"
                            EmptyDataText="-=> No Data Found <=-" ShowHeaderWhenEmpty="true">
                            <Columns>
                            <asp:TemplateField>
                                <HeaderTemplate>Member Display Name</HeaderTemplate>
                                <ItemTemplate><asp:Label ID="lblDispName" runat="server" Text='<%# Eval("Name") %>' /></ItemTemplate>
                            </asp:TemplateField>
                                <asp:BoundField DataField="ContactType" HeaderText="Contact Type" />
                                <asp:BoundField DataField="Begin" HeaderText="Begin Date" DataFormatString="{0:yyyy/MM/dd}" />
                                <asp:BoundField DataField="End" HeaderText="End Date" DataFormatString="{0:yyyy/MM/dd}" />
                            <asp:TemplateField>
                                <HeaderTemplate>Address Line 1</HeaderTemplate>
                                <ItemTemplate><asp:Label ID="lblAddr1" runat="server" Text='<%# Eval("Address_1") %>' /></ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField>
                                <HeaderTemplate>Address Line 2</HeaderTemplate>
                                <ItemTemplate><asp:Label ID="lblAddr2" runat="server" Text='<%# Eval("Address_2") %>' /></ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField>
                                <HeaderTemplate>City</HeaderTemplate>
                                <ItemTemplate><asp:Label ID="lblCity" runat="server" Text='<%# Eval("City") %>' /></ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField>
                                <HeaderTemplate>State/Province</HeaderTemplate>
                                <ItemTemplate><asp:Label ID="lblAddr2" runat="server" Text='<%# Eval("State_Prov") %>' /></ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField>
                                <HeaderTemplate>ZIP or Postal Code</HeaderTemplate>
                                <ItemTemplate><asp:Label ID="lblCity" runat="server" Text='<%# Eval("PostalCode") %>' /></ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField>
                                <HeaderTemplate>Country</HeaderTemplate>
                                <ItemTemplate><asp:Label ID="lblAddr2" runat="server" Text='<%# Eval("Country") %>' /></ItemTemplate>
                            </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                </Content>
            </ajaxToolkit:AccordionPane>        
            <ajaxToolkit:AccordionPane runat="server" >
                <Header> Certifications</Header>
                <Content>
                        <asp:GridView ID="gvCertifics" runat="server" AutoGenerateColumns="False"
                            GridLines="None" CssClass="SoarNPGridStyle" HorizontalAlign="Center"
                            EmptyDataText="-=> No Data Found <=-" ShowHeaderWhenEmpty="true">
                            <Columns>
                            <asp:TemplateField>
                                <HeaderTemplate>Member Display Name</HeaderTemplate>
                                <ItemTemplate><asp:Label ID="lblDispName" runat="server" Text='<%# Eval("Name") %>' /></ItemTemplate>
                            </asp:TemplateField>
                                <asp:BoundField DataField="Q_R" HeaderText="Qual/Cert or Rating?" />
                                <asp:BoundField DataField="Qu_Rat" HeaderText="Qual/Certification/Rating Name" />
                                <asp:BoundField DataField="Since" HeaderText="Date Obtained" DataFormatString="{0:yyyy/MM/dd}" />
                                <asp:BoundField DataField="Expires" HeaderText="Date Expires" DataFormatString="{0:yyyy/MM/dd}" />
                                <asp:BoundField DataField="Notes" HeaderText="Notes" />
                            </Columns>
                        </asp:GridView>
                </Content>
            </ajaxToolkit:AccordionPane>
            <ajaxToolkit:AccordionPane runat="server" >
                <Header> Ratings</Header>
                <Content>
                        <asp:GridView ID="gvRatings" runat="server" AutoGenerateColumns="False"
                            GridLines="None" CssClass="SoarNPGridStyle" HorizontalAlign="Center"
                            EmptyDataText="-=> No Data Found <=-" ShowHeaderWhenEmpty="true">
                            <Columns>
                            <asp:TemplateField>
                                <HeaderTemplate>Member Display Name</HeaderTemplate>
                                <ItemTemplate><asp:Label ID="lblDispName" runat="server" Text='<%# Eval("Name") %>' /></ItemTemplate>
                            </asp:TemplateField>
                                <asp:BoundField DataField="Q_R" HeaderText="Qual/Cert or Rating?" />
                                <asp:BoundField DataField="Qu_Rat" HeaderText="Qual/Certification/Rating Name" />
                                <asp:BoundField DataField="Since" HeaderText="Date Obtained" DataFormatString="{0:yyyy/MM/dd}" />
                                <asp:BoundField DataField="Expires" HeaderText="Date Expires" DataFormatString="{0:yyyy/MM/dd}" />
                                <asp:BoundField DataField="Notes" HeaderText="Notes" />
                            </Columns>
                        </asp:GridView>
                </Content>
            </ajaxToolkit:AccordionPane>
            <ajaxToolkit:AccordionPane runat="server" >
                <Header>Qualifications</Header>
                <Content>
                        <asp:GridView ID="gvQualifics" runat="server" AutoGenerateColumns="False"
                            GridLines="None" CssClass="SoarNPGridStyle" HorizontalAlign="Center"
                            EmptyDataText="-=> No Data Found <=-" ShowHeaderWhenEmpty="true">
                            <Columns>
                            <asp:TemplateField>
                                <HeaderTemplate>Member Display Name</HeaderTemplate>
                                <ItemTemplate><asp:Label ID="lblDispName" runat="server" Text='<%# Eval("Name") %>' /></ItemTemplate>
                            </asp:TemplateField>
                                <asp:BoundField DataField="Q_R" HeaderText="Qual/Cert or Rating?" />
                                <asp:BoundField DataField="Qu_Rat" HeaderText="Qual/Certification/Rating Name" />
                                <asp:BoundField DataField="Since" HeaderText="Date Obtained" DataFormatString="{0:yyyy/MM/dd}" />
                                <asp:BoundField DataField="Expires" HeaderText="Date Expires" DataFormatString="{0:yyyy/MM/dd}" />
                                <asp:BoundField DataField="Notes" HeaderText="Notes" />
                            </Columns>
                        </asp:GridView>
                </Content>
            </ajaxToolkit:AccordionPane>
            <ajaxToolkit:AccordionPane runat="server" >
                <Header> Board positions and dates </Header>
                <Content>
                        <asp:GridView ID="gvBoardPos" runat="server" AutoGenerateColumns="False"
                            GridLines="None" CssClass="SoarNPGridStyle" HorizontalAlign="Center"
                            EmptyDataText="-=> No Data Found <=-" ShowHeaderWhenEmpty="true">
                            <Columns>
                            <asp:TemplateField>
                                <HeaderTemplate>Member Display Name</HeaderTemplate>
                                <ItemTemplate><asp:Label ID="lblDispName" runat="server" Text='<%# Eval("Name") %>' /></ItemTemplate>
                            </asp:TemplateField>
                                <asp:BoundField DataField="Office" HeaderText="Board Position" />
                                <asp:BoundField DataField="Begin" HeaderText="Begin Date" DataFormatString="{0:yyyy/MM/dd}" />
                                <asp:BoundField DataField="End" HeaderText="End Date" DataFormatString="{0:yyyy/MM/dd}" />
                                <asp:BoundField DataField="Notes" HeaderText="Notes" />
                            </Columns>
                        </asp:GridView>
                </Content>
            </ajaxToolkit:AccordionPane>
            <ajaxToolkit:AccordionPane runat="server" >
                <Header> Financial details </Header>
                <Content>
                    Not yet operational
                </Content>
            </ajaxToolkit:AccordionPane>        
            <ajaxToolkit:AccordionPane runat="server" >
                <Header> Tracking of flying charges and their minima </Header>
                <Content>
                    <a href="../Fin/MemberFinStat.aspx">Track flying charges</a>
                </Content>
            </ajaxToolkit:AccordionPane>
            <ajaxToolkit:AccordionPane runat="server" >
                <Header> Equity share ownership </Header>
                <Content>
                        <asp:GridView ID="gvEquityShares" runat="server" AutoGenerateColumns="False"
                            GridLines="None" CssClass="SoarNPGridStyle" HorizontalAlign="Center"
                            EmptyDataText="-=> No Data Found <=-" ShowHeaderWhenEmpty="true">
                            <Columns>
                                <asp:BoundField DataField="Date" HeaderText="Date of Transaction" DataFormatString="{0:yyyy/MM/dd}" />
                                <asp:BoundField DataField="Qual" HeaderText="Date Quality" />
                                <asp:BoundField DataField="NumberOfShares" HeaderText="Number of Shares" ItemStyle-HorizontalAlign="Right" />
                                <asp:BoundField DataField="Type" HeaderText="Type of Transaction" />
                                <asp:BoundField DataField="Info" HeaderText="Source of Information" />
                                <asp:BoundField DataField="Notes" HeaderText="Notes" />
                            </Columns>
                        </asp:GridView>
                    <div class="text-center">
                        Sum of Shares Transactions = <asp:Label ID="lblSum" runat="server" Text="TBD"></asp:Label> Shares
                    </div>
                </Content>
            </ajaxToolkit:AccordionPane>
        </Panes>            
    </ajaxToolkit:Accordion>
    </div>

    <div>
        <%-- ModalPopupExtender, popping up MPE_Panel and dynamically populating lblPopupText --%>
        <asp:LinkButton ID="Target" runat="server" Text="T" CssClass="displayNone" />
        <ajaxToolkit:ModalPopupExtender ID="ModalPopExt" runat="server"
            TargetControlID="Target" PopupControlID="MPE_Panel"
            BackgroundCssClass="background"
             />
        <asp:Panel ID="MPE_Panel" runat="server" CssClass="popup" style="display:none;" HorizontalAlign="Center">
            <asp:Label ID="lblPopupText" runat="server" BackColor="#eeb5a2" Font-Bold="true" />
            <br /><br />
            <p> <asp:Button ID="OkButton" runat="server" Text="OK" OnClick="Button_Click" />&nbsp;&nbsp;
                <asp:Button ID="CancelButton" runat="server" Text="Cancel" OnClick="Button_Click" />&nbsp;&nbsp;
                <asp:Button ID="NoButton" runat="server" Text="No" OnClick="Button_Click" />&nbsp;&nbsp;
                <asp:Button ID="YesButton" runat="server" Text="Yes" OnClick="Button_Click"/></p>
        </asp:Panel>
    </div>

</asp:Content>

<asp:Content ID="FooterContent" ContentPlaceHolderID="ContentPlaceHolderFooter" runat="server">
    <%: Title %>
</asp:Content>
