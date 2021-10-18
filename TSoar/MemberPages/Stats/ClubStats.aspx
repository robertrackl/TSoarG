<%@ Page Title="Club Flying Statistics [ClubStats]" Language="C#" MasterPageFile="~/mTSoar.Master" AutoEventWireup="true"
    CodeBehind="ClubStats.aspx.cs" Inherits="TSoar.MemberPages.Stats.ClubStats" %>

<asp:Content ID="HeaderContent" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="server">
    <asp:Label runat="server" Text="The PSSA Flight Statistics Page" Font-Italic="true" />
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="ContentPlaceHolderMain" runat="server">
    <div class="HelpText" ><%-- SCR 213 --%>
        <p>
            <asp:Label runat="server" ID="lbl_filter" Text="." />
        </p>
            <asp:Label ID="lblVersionUpdate" runat="server"
                Text="Your previous filter settings had to be deleted because of a filter settings data table version update; sorry about that!"
                Font-Bold="true" ForeColor="Red" Visible="false" />
        <p>Change filters for flight operations:
            <asp:Button ID="pbStdOpsFilters" Text="Standard" runat="server" OnClick="pbStdOpsFilters_Click" />
            <asp:Button ID="pbAdvOpsFilters" Text="Advanced" runat="server" OnClick="pbAdvOpsFilters_Click" /></p>
        <p>Re Tow Plane Statistics: We do not keep track of the tow plane's landing time, nor of tow plane flights not on tow. If you include tow plane statistics below, an average flight duration of
            <asp:Label ID="lblTowPlaneFltDur" runat="server" Text="10" /> minutes is assumed. Non-towing flights do not show up here.</p>
        <h4>Aggregate statistics for the flights contained in the details tree view or table view below:</h4>
        <asp:GridView ID="gvStatsByEqR" runat="server" AutoGenerateColumns="false" CssClass="SoarNPGridStyle">
            <Columns>
                <asp:BoundField DataField="sEquipmentRole" HeaderText="Equipment Role" ItemStyle-CssClass="text-center" />
                <asp:BoundField DataField="NumFl" HeaderText="Number of Flights" ItemStyle-CssClass="text-center" />
                <asp:BoundField DataField="TotalTimeHrs" HeaderText="Total Hours Flown" ItemStyle-CssClass="text-center" />
                <asp:BoundField DataField="AvgFltDur" HeaderText="Avg. Number of Minutes per Flight" ItemStyle-CssClass="text-center" />
            </Columns>
        </asp:GridView>
    </div><%-- SCR 213 --%>
    <ajaxToolkit:TabContainer ID="TabC_MFC" runat="server">
        <ajaxToolkit:TabPanel runat="server" HeaderText="Tree View">
            <ContentTemplate>
                Tree View LEGEND: <span style='color:red; font-weight:bold'> T </span> = Takeoff, <span style='color:red; font-weight:bold'> L </span> = Landing, 
                    <span style='color:red; font-weight:bold'> D </span> = Duration in minutes, <span style='color:red; font-weight:bold'> CC </span> = Charge Code
                <asp:UpdatePanel ID="UpdatePanelTrV" UpdateMode="Conditional" runat="server">
                    <ContentTemplate>
                        <asp:TreeView ID="trv_Ops" runat="server" MaxDataBindDepth="6" PathSeparator="|"
                            OnTreeNodePopulate="trv_Ops_TreeNodePopulate"
                            PopulateNodesFromClient="false" ShowLines="true" BackColor="#EDF8E7" NodeWrap="true"
                            BorderStyle="Solid" HoverNodeStyle-BackColor="WhiteSmoke" SelectedNodeStyle-BackColor="Orange"
                            NodeStyle-BorderStyle="None"
                            NodeStyle-HorizontalPadding="5" NodeStyle-ChildNodesPadding="5" NodeStyle-ForeColor="#5D3217">
                            <Nodes>
                                <asp:TreeNode PopulateOnDemand="True" Text="Operations List" Value="" Expanded="false"></asp:TreeNode>
                            </Nodes>
                        </asp:TreeView>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </ContentTemplate>
        </ajaxToolkit:TabPanel>
        <ajaxToolkit:TabPanel runat="server" HeaderText="Table View">
            <ContentTemplate>
                <div style="font-size:xx-small;">Hover mouse over table column headers to see helpful information. &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                    Information on Special Operations can only be viewed in Tree View.</div>
                <div style="overflow:scroll">
                    <asp:GridView ID="gvOps" runat="server"  AutoGenerateColumns="False" ShowHeaderWhenEmpty="true"
                            GridLines="None" CssClass="SoarNPGridStyle"
                            AllowPaging="true" PageSize="50" OnPageIndexChanging="gvOps_PageIndexChanging"
                            Font-Size="Small" EmptyDataText=" --==>> No Data Found <<==-- " >
                            <PagerSettings Mode="Numeric" PageButtonCount="10" Position="TopAndBottom" />
                            <PagerStyle CssClass="SoarNPpaging" />
                            <Columns>
                            <asp:TemplateField ItemStyle-HorizontalAlign="Center" Visible="false">
                                <HeaderTemplate>
                                    <asp:Panel CssClass="text-center" runat="server">
                                        <asp:Label ID="lblHID" runat="server" Text="Internal ID" ToolTip="Unique internal database identifier of a flight operation" />
                                    </asp:Panel>
                                </HeaderTemplate>
                                <ItemTemplate><asp:Label ID="lblID" runat="server" Text='<%# Eval("sOpID") %>' /></ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField ItemStyle-HorizontalAlign="Center" Visible="false">
                                <HeaderTemplate>
                                    <asp:Panel CssClass="text-center" runat="server">
                                        <asp:Label ID="lblHCt" runat="server" Text="Seq #" ToolTip="Sequence number of item in this flight operation" />
                                    </asp:Panel>
                                </HeaderTemplate>
                                <ItemTemplate><asp:Label ID="lblCt" runat="server" Text='<%# Eval("iCount") %>' /></ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField ItemStyle-HorizontalAlign="Center">
                                <HeaderTemplate>
                                    <asp:Panel CssClass="text-center" runat="server">
                                        <asp:Label ID="lblHLM" runat="server" Text="LM" />
                                    </asp:Panel>
                                </HeaderTemplate>
                                <ItemTemplate><asp:Label ID="lblLM" runat="server" Text='<%# Eval("cLaunchMethod") %>' /></ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField>
                                <HeaderTemplate>
                                    <asp:Panel CssClass="text-center" runat="server">
                                        <asp:Label ID="lblHTOL" runat="server" Text="Takeoff Location" ToolTip="The location where the takeoff took place" />
                                    </asp:Panel>
                                </HeaderTemplate>
                                <ItemTemplate><asp:Label ID="lblTOL" runat="server" Text='<%# Server.HtmlDecode((string)Eval("TOLoc")) %>' /></ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField>
                                <HeaderTemplate>
                                    <asp:Panel CssClass="text-center" runat="server">
                                        <asp:Label ID="lblHTOT" runat="server" Text="Takeoff Date Time" ToolTip="The date and time when the takeoff took place" />
                                    </asp:Panel>
                                </HeaderTemplate>
                                <ItemTemplate><asp:Label ID="lblTOT" runat="server" Text='<%# Eval("sDBegin") %>' /></ItemTemplate>
<%--                                <ItemTemplate><asp:Label ID="lblTOT" runat="server" Text='<%# TSoar.CustFmt.sFmtDate((DateTime)Eval("DBegin"),TSoar.CustFmt.enDFmt.DateAndTimeMin) %>' /></ItemTemplate>--%>
                            </asp:TemplateField>
                            <asp:TemplateField ItemStyle-HorizontalAlign="Center">
                                <HeaderTemplate>
                                    <asp:Panel CssClass="text-center" runat="server">
                                        <asp:Label ID="lblHDur" runat="server" Text="Dur Min" ToolTip="Glider flight duration in minutes" />
                                    </asp:Panel>
                                </HeaderTemplate>
                                <ItemTemplate><asp:Label ID="lblDur" runat="server" Text='<%# Eval("sDuratMin") %>' /></ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField>
                                <HeaderTemplate>
                                    <asp:Panel CssClass="text-center" runat="server">
                                        <asp:Label ID="lblHLDL" runat="server" Text="Landing Location" ToolTip="The location where the landing took place" />
                                    </asp:Panel>
                                </HeaderTemplate>
                                <ItemTemplate><asp:Label ID="lblLDL" runat="server" Text='<%# Server.HtmlDecode((string)Eval("LDGLoc")) %>' /></ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField ItemStyle-HorizontalAlign="Center">
                                <HeaderTemplate>
                                    <asp:Panel CssClass="text-center" runat="server">
                                        <asp:Label ID="lblHCC" runat="server" Text="CC" />
                                    </asp:Panel>
                                </HeaderTemplate>
                                <ItemTemplate><asp:Label ID="lblCC" runat="server" Text='<%# Eval("cChargeCode") %>' /></ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField>
                                <HeaderTemplate>
                                    <asp:Panel CssClass="text-center" runat="server">
                                        <asp:Label ID="lblHCom" runat="server" Text="Comments" />
                                    </asp:Panel>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="lblCom" runat="server" Text='<%# Server.HtmlDecode(((Eval("sComment") == DBNull.Value)? "" : (string)Eval("sComment"))) %>' />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField>
                                <HeaderTemplate>
                                    <asp:Panel CssClass="text-center" runat="server">
                                        <asp:Label ID="lblHSpOp" runat="server" Text="Sp Op"
                                            ToolTip="Do any Special Operations records exist for this flight operation?
 If so, use Tree View to see the contents of Special Operations records."/>
                                    </asp:Panel>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <asp:Panel CssClass="text-center" runat="server">
                                        <asp:Label ID="lblSpOp" runat="server" Text='<%# Eval("sSpOp") %>' />
                                    </asp:Panel>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField ItemStyle-HorizontalAlign="Center">
                                <HeaderTemplate>
                                    <asp:Panel CssClass="text-center" runat="server">
                                        <asp:Label ID="lblHER" runat="server" Text="Equipment Role" />
                                    </asp:Panel>
                                </HeaderTemplate>
                                <ItemTemplate><asp:Label ID="lblER" runat="server" Text='<%# Server.HtmlDecode((string)Eval("sEquipmentRole")) %>' /></ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField>
                                <HeaderTemplate>
                                    <asp:Panel CssClass="text-center" runat="server">
                                        <asp:Label ID="lblHEN" runat="server" Text="Equipment Name" />
                                    </asp:Panel>
                                </HeaderTemplate>
                                <ItemTemplate><asp:Label ID="lblEN" runat="server" Text='<%# Server.HtmlDecode((string)Eval("sShortEquipName")) %>' /></ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField ItemStyle-HorizontalAlign="Right">
                                <HeaderTemplate>
                                    <asp:Panel CssClass="text-center" runat="server">
                                        <asp:Label ID="lblHRA" runat="server" Text="Rel Alt" ToolTip="Release Altitude in feet above Mean Sea Level" />
                                    </asp:Panel>
                                </HeaderTemplate>
                                <ItemTemplate><asp:Label ID="lblRA" runat="server" Text='<%# Eval("sReleaseAltitude") %>' /></ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField>
                                <HeaderTemplate>
                                    <asp:Panel CssClass="text-center" runat="server">
                                        <asp:Label ID="lblHAR" runat="server" Text="Aviator Role" />
                                    </asp:Panel>
                                </HeaderTemplate>
                                <ItemTemplate><asp:Label ID="lblAR" runat="server" Text='<%# Server.HtmlDecode((string)Eval("sAviatorRole")) %>' /></ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField>
                                <HeaderTemplate>
                                    <asp:Panel CssClass="text-center" runat="server">
                                        <asp:Label ID="lblHWho" runat="server" Text="Aviator Name" />
                                    </asp:Panel>
                                </HeaderTemplate>
                                <ItemTemplate><asp:Label ID="lblWho" runat="server" Text='<%# Server.HtmlDecode((string)Eval("sDisplayName")) %>' /></ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField ItemStyle-HorizontalAlign="Right">
                                <HeaderTemplate>
                                    <asp:Panel CssClass="text-center" runat="server">
                                        <asp:Label ID="lblHPC" runat="server" Text="% Chg" ToolTip="Percentage of the flight costs for which this aviator is responsible" />
                                    </asp:Panel>
                                </HeaderTemplate>
                                <ItemTemplate><asp:Label ID="lblPC" runat="server" Text='<%# Eval("dPercentCharge") %>' /></ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField>
                                <HeaderTemplate>
                                    <asp:Panel CssClass="text-center" runat="server">
                                        <asp:Label ID="lblH1F" runat="server" Text="1st?" ToolTip="Was this the first flight of this soaring season for this aviator, and was it with an instructor?" />
                                    </asp:Panel>
                                </HeaderTemplate>
                                <ItemTemplate><asp:Label ID="lbl1F" runat="server" Text='<%# ((bool)Eval("b1stFlight"))? "Yes" : "No" %>' /></ItemTemplate>
                            </asp:TemplateField>
                            </Columns>
                    </asp:GridView>
                </div>
            </ContentTemplate>
        </ajaxToolkit:TabPanel>
    </ajaxToolkit:TabContainer>

    <div>
        <%-- ModalPopupExtender, popping up MPE_Panel and dynamically populating lblPopupText --%>
        <asp:LinkButton ID="Target" runat="server" Text="T" CssClass="displayNone" />
        <ajaxToolkit:ModalPopupExtender ID="ModalPopExt" runat="server"
            TargetControlID="Target" PopupControlID="MPE_Panel"
            BackgroundCssClass="background" DropShadow="true" />
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