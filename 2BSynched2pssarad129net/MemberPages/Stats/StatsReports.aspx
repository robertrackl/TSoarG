<%@ Page Title="Statistics Reports" Language="C#" MasterPageFile="~/mTSoar.Master" AutoEventWireup="true" CodeBehind="StatsReports.aspx.cs"
    Inherits="TSoar.MemberPages.Stats.StatsReports" %>

<asp:Content ID="HeaderContent" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="server">
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="ContentPlaceHolderMain" runat="server">
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
            <ajaxToolkit:AccordionPane ID="AccPCaveats" runat="server" >
                <Header> Explanations, Caveats </Header>
                <Content>
                    <div class="HelpText" ><%-- SCR 213 --%>
                        <p>Two kinds of reports are available here: click on "Report by Aviator" or "Report by Equipment" below. In either case, you need to 
                            also click on the "Generate Report" button. There is no filtering available here; if you need that please navigate
                            <a href="ClubStats.aspx">here.</a>
                        </p>
                        <p>The reports available here only contain data that were reported to your glider club's statistician: all flights by club gliders,
                            but privately owned gliders only to the extent that the flights were reported to the statistician.
                            That does include all flights where a club towplane was involved because we expect all such flights to be reported to the statistician.
                        </p>
                        <p>Reports by Aviator may show more flying hours than for gliders because double occupancy is counted separately, i.e.,
                        a flight in a two-seat glider results in hours of flight time for both first and second pilot.
                        </p>
                        <p>Reports on this page only show glider flights, both by aviator and by equipment. Tow plane flights are ignored.
                            However, they are available <a href="ClubStats.aspx">here</a>
                            although flight durations are only a rough estimate as tow plane landing times are not tracked.
                        </p>
                    </div><%-- SCR 213 --%>
                </Content>
            </ajaxToolkit:AccordionPane>
            <ajaxToolkit:AccordionPane ID="AccPPilot" runat="server" >
                <Header> Report by Aviator </Header>
                <Content>
                    <asp:Button ID="pbByPilot" runat="server" Text="Generate Report" OnClick="pbByPilot_Click" />
                    <asp:GridView ID="gvByPilot" runat="server" AutoGenerateColumns="False"
                        GridLines="None" CssClass="SoarNPGridStyle" EmptyDataText="No Data Yet" ShowHeaderWhenEmpty="true"
                        HorizontalAlign="Center" AllowPaging="true" PageSize="40" OnPageIndexChanging="gvByPilot_PageIndexChanging">
                        <PagerSettings Mode="NextPreviousFirstLast" Position="TopAndBottom" Visible="true" FirstPageText="&nbsp;First&nbsp;"
                            LastPageText="&nbsp;Last&nbsp;" NextPageText="&nbsp;Next&nbsp;" PreviousPageText="&nbsp;Previous&nbsp;" />
                        <PagerStyle CssClass="SoarNPpaging" />
                        <Columns>
                            <asp:TemplateField>
                                <HeaderTemplate>Aviator</HeaderTemplate>
                                <ItemTemplate><asp:Label ID="lblAviator" runat="server" Text='<%# Eval("Aviator") %>' /></ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="Year" HeaderText="Year" HeaderStyle-HorizontalAlign="Center" />
                            <asp:TemplateField>
                                <HeaderTemplate>Glider</HeaderTemplate>
                                <ItemTemplate><asp:Label ID="lblGlider" runat="server" Text='<%# Eval("Glider") %>' /></ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="Role" HeaderText="Role" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" />
                            <asp:BoundField DataField="Flight_Hours" HeaderText="Flight Hours" DataFormatString="{0:F2}" ItemStyle-HorizontalAlign="Right" HeaderStyle-HorizontalAlign="Center" />
                            <asp:BoundField DataField="SubGlider" HeaderText="SubT Glider" ItemStyle-HorizontalAlign="Right" HeaderStyle-HorizontalAlign="Center" />
                            <asp:BoundField DataField="SubYear" HeaderText="SubT Year" ItemStyle-HorizontalAlign="Right" HeaderStyle-HorizontalAlign="Center" />
                            <asp:BoundField DataField="SubAviator" HeaderText="SubT Aviator" ItemStyle-HorizontalAlign="Right" HeaderStyle-HorizontalAlign="Center" />
                            <asp:BoundField DataField="Total" HeaderText="Total" ItemStyle-HorizontalAlign="Right" HeaderStyle-HorizontalAlign="Center" />
                        </Columns>
                    </asp:GridView>
                </Content>
            </ajaxToolkit:AccordionPane>
            <ajaxToolkit:AccordionPane ID="AccPGlider" runat="server" >
                <Header> Report by Equipment </Header>
                <Content>
                    <asp:Button ID="pbByAircraft" runat="server" Text="Generate Report" OnClick="pbByAircraft_Click" /></li>
                    <asp:GridView ID="gvByGlider" runat="server" AutoGenerateColumns="False"
                        GridLines="None" CssClass="SoarNPGridStyle" EmptyDataText="No Data Yet" ShowHeaderWhenEmpty="true"
                        HorizontalAlign="Center" AllowPaging="true" PageSize="40" OnPageIndexChanging="gvByGlider_PageIndexChanging">
                        <PagerSettings Mode="NextPreviousFirstLast" Position="TopAndBottom" Visible="true" FirstPageText="&nbsp;First&nbsp;"
                            LastPageText="&nbsp;Last&nbsp;" NextPageText="&nbsp;Next&nbsp;" PreviousPageText="&nbsp;Previous&nbsp;" />
                        <PagerStyle CssClass="SoarNPpaging" />
                        <Columns>
                            <asp:TemplateField HeaderStyle-HorizontalAlign="Center">
                                <HeaderTemplate>Glider - Owner</HeaderTemplate>
                                <ItemTemplate><asp:Label ID="lblGliderOwner" runat="server" Text='<%# Eval("Glider___Owner") %>' /></ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="Year" HeaderText="Year" HeaderStyle-HorizontalAlign="Center" />
                            <asp:BoundField DataField="Flight_Hours" HeaderText="Flight Hours" DataFormatString="{0:F2}" ItemStyle-HorizontalAlign="Right" HeaderStyle-HorizontalAlign="Center" />
                            <asp:BoundField DataField="SubGlider" HeaderText="SubT Glider" ItemStyle-HorizontalAlign="Right" HeaderStyle-HorizontalAlign="Center" />
                            <asp:BoundField DataField="Total" HeaderText="Total" ItemStyle-HorizontalAlign="Right" HeaderStyle-HorizontalAlign="Center" />
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