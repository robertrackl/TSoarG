<%@ Page Title="Club Roster" Language="C#" MasterPageFile="~/mTSoar.Master" AutoEventWireup="true" CodeBehind="ClubRoster.aspx.cs" Inherits="TSoar.MemberPages.Roster.ClubRoster" %>

<asp:Content ID="HeaderContent" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="server">
    <asp:Label runat="server" Text="Show Several Lists of Club Members" Font-Italic="true" />
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="ContentPlaceHolderMain" runat="server">
    <ajaxToolkit:Accordion
        ID="AccordionRoster"
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
                <Header> Telephone Numbers </Header>
                <Content>
                    <asp:GridView ID="gvTelNumbers" runat="server" AutoGenerateColumns="False"
                        GridLines="None" CssClass="SoarNPGridStyle" HorizontalAlign="Center"
                        OnPageIndexChanging="gvTelNumbers_PageIndexChanging" AllowPaging="true" PageSize="20">
                    <PagerSettings Mode="NextPreviousFirstLast" Position="TopAndBottom" Visible="true" FirstPageText="&nbsp;First&nbsp;"
                        LastPageText="&nbsp;Last&nbsp;" NextPageText="&nbsp;Next&nbsp;" PreviousPageText="&nbsp;Previous&nbsp;" />
                    <PagerStyle CssClass="SoarNPpaging" />
                        <Columns>
                            <asp:TemplateField>
                                <HeaderTemplate>Member Display Name</HeaderTemplate>
                                <ItemTemplate><asp:Label ID="lblDispName" runat="server" Text='<%# Eval("sDisplayName") %>' /></ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="sContactInfo" HeaderText="Telephone #" />
                            <asp:BoundField DataField="sPeopleContactType" HeaderText="Type" />
                        </Columns>
                    </asp:GridView>
                </Content>
            </ajaxToolkit:AccordionPane>        
            <ajaxToolkit:AccordionPane runat="server" >
                <Header> Email Addresses </Header>
                <Content>
                    <asp:GridView ID="gvEmails" runat="server" AutoGenerateColumns="False"
                        GridLines="None" CssClass="SoarNPGridStyle" HorizontalAlign="Center"
                        OnPageIndexChanging="gvEmails_PageIndexChanging" AllowPaging="true" PageSize="20">
                    <PagerSettings Mode="NextPreviousFirstLast" Position="TopAndBottom" Visible="true" FirstPageText="&nbsp;First&nbsp;"
                        LastPageText="&nbsp;Last&nbsp;" NextPageText="&nbsp;Next&nbsp;" PreviousPageText="&nbsp;Previous&nbsp;" />
                    <PagerStyle CssClass="SoarNPpaging" />
                        <Columns>
                            <asp:TemplateField>
                                <HeaderTemplate>Member Display Name</HeaderTemplate>
                                <ItemTemplate><asp:Label ID="lblDispName" runat="server" Text='<%# Eval("sDisplayName") %>' /></ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField>
                                <HeaderTemplate>Email Address</HeaderTemplate>
                                <ItemTemplate><asp:Label ID="lblContInf" runat="server" Text='<%# Eval("sContactInfo") %>' /></ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </Content>
            </ajaxToolkit:AccordionPane>        
            <ajaxToolkit:AccordionPane runat="server" >
                <Header> Physical Addresses </Header>
                <Content>
                    <asp:GridView ID="gvPhysAddr" runat="server" AutoGenerateColumns="False"
                        GridLines="None" CssClass="SoarNPGridStyle" HorizontalAlign="Center"
                        OnPageIndexChanging="gvPhysAddr_PageIndexChanging" AllowPaging="true" PageSize="20">
                    <PagerSettings Mode="NextPreviousFirstLast" Position="TopAndBottom" Visible="true" FirstPageText="&nbsp;First&nbsp;"
                        LastPageText="&nbsp;Last&nbsp;" NextPageText="&nbsp;Next&nbsp;" PreviousPageText="&nbsp;Previous&nbsp;" />
                    <PagerStyle CssClass="SoarNPpaging" />
                        <Columns>
                            <asp:TemplateField>
                                <HeaderTemplate>Member Display Name</HeaderTemplate>
                                <ItemTemplate><asp:Label ID="lblDispName" runat="server" Text='<%# Eval("sDisplayName") %>' /></ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField>
                                <HeaderTemplate>Address 1</HeaderTemplate>
                                <ItemTemplate><asp:Label ID="lblAddr1" runat="server" Text='<%# Eval("sAddress1") %>' /></ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField>
                                <HeaderTemplate>Address 2</HeaderTemplate>
                                <ItemTemplate><asp:Label ID="lblAddr2" runat="server" Text='<%# Eval("sAddress2") %>' /></ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField>
                                <HeaderTemplate>City</HeaderTemplate>
                                <ItemTemplate><asp:Label ID="lblCity" runat="server" Text='<%# Eval("sCity") %>' /></ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField>
                                <HeaderTemplate>Zip/Postal Codes</HeaderTemplate>
                                <ItemTemplate><asp:Label ID="lblZIP" runat="server" Text='<%# Eval("sZipPostal") %>' /></ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField>
                                <HeaderTemplate>Country</HeaderTemplate>
                                <ItemTemplate><asp:Label ID="lblCountry" runat="server" Text='<%# Eval("sCountry") %>' /></ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="sPeopleContactType" HeaderText="Type" />
                        </Columns>
                    </asp:GridView>
                </Content>
            </ajaxToolkit:AccordionPane>        
            <ajaxToolkit:AccordionPane runat="server" >
                <Header> Qualifications </Header>
                <Content>
                    <asp:GridView ID="gvQualifs" runat="server" AutoGenerateColumns="False"
                        GridLines="None" CssClass="SoarNPGridStyle" HorizontalAlign="Center"
                        OnPageIndexChanging="gvQualifs_PageIndexChanging" AllowPaging="true" PageSize="20">
                    <PagerSettings Mode="NextPreviousFirstLast" Position="TopAndBottom" Visible="true" FirstPageText="&nbsp;First&nbsp;"
                        LastPageText="&nbsp;Last&nbsp;" NextPageText="&nbsp;Next&nbsp;" PreviousPageText="&nbsp;Previous&nbsp;" />
                    <PagerStyle CssClass="SoarNPpaging" />
                        <Columns>
                            <asp:TemplateField>
                                <HeaderTemplate>Member Display Name</HeaderTemplate>
                                <ItemTemplate><asp:Label ID="lblDispName" runat="server" Text='<%# Eval("sDisplayName") %>' /></ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="Certification" HeaderText="Certification" />
                            <asp:BoundField DataField="Rating" HeaderText="Rating" />
                            <asp:BoundField DataField="Qualification" HeaderText="Qualification" />
                            <asp:BoundField DataField="Since" HeaderText="Since" />
                            <asp:BoundField DataField="Expires" HeaderText="Expires" />
                        </Columns>
                    </asp:GridView>
                </Content>
            </ajaxToolkit:AccordionPane>        
        </Panes>            
    </ajaxToolkit:Accordion>

</asp:Content>

<asp:Content ID="FooterContent" ContentPlaceHolderID="ContentPlaceHolderFooter" runat="server">
    <%: Title %>
</asp:Content>
