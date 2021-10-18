<%@ Page Title="FlyActInvoice" Language="C#" MasterPageFile="~/mTSoar.Master" AutoEventWireup="true" CodeBehind="FlyActInvoice.aspx.cs"
    Inherits="TSoar.Accounting.FinDetails.SalesAR.FlyActInvoice" %>

<asp:Content ID="HeaderContent" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="server">
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="ContentPlaceHolderMain" runat="server">

    <div class="row divRowBackgr" >
    <div class="col-sm-6"><h4>Create Invoices from Records of Club Members' Flying Activities</h4></div>
    <div class="col-sm-6"><asp:Button ID="pbHelp" runat="server" Text="Display Help" OnClick="pbHelp_Click" /></div>
    </div>
    <u>Operational Restriction:</u> Make sure that only one user of this website at a time creates invoices. This section is not suitable for simultaneous access by more than one user.
            Different users may use this section at different non-overlapping times.
    <ajaxToolkit:Accordion
        ID="AccordionInvoice"
        runat="Server"
        SelectedIndex="0"
        HeaderCssClass="accordionHeader"
        HeaderSelectedCssClass="accordionHeaderSelected"
        ContentCssClass="accordionContent"
        AutoSize="None"
        FadeTransitions="true"
        TransitionDuration="250"
        FramesPerSecond="40"
        RequireOpenedPane="false" >
        <Panes>
            <ajaxToolkit:AccordionPane ID="AccPGenDat" runat="server" >
                <Header>
                    <asp:Label ID="lblAccPHGenDat" runat="server" Text="General/Control Data"  />
                </Header>
                <Content>
                    <asp:Table runat="server" CssClass="SoarNPGridStyle">
                        <asp:TableHeaderRow BackColor="#D0F0C0">
                            <asp:TableHeaderCell CssClass="text-center">Item</asp:TableHeaderCell>
                            <asp:TableHeaderCell CssClass="text-center">Value or Action</asp:TableHeaderCell>
                        </asp:TableHeaderRow>

<%--                        <asp:TableRow BackColor="#EDF8D7" >
                            <asp:TableCell HorizontalAlign="Right">Date on Newly Generated Invoices</asp:TableCell>
                            <asp:TableCell HorizontalAlign="Center"><asp:TextBox ID="txbInvDate" runat="server" TextMode="Date" AutoPostBack="true" OnTextChanged="txb_TextChanged" /></asp:TableCell>
                        </asp:TableRow>--%>
                        <asp:TableRow BackColor="#EDF8D7">
                            <asp:TableCell HorizontalAlign="Right">Days to Invoices' Due Dates</asp:TableCell>
                            <asp:TableCell HorizontalAlign="Center"><asp:TextBox ID="txbDays2Due" runat="server" TextMode="Number" Style="width: 60px;" AutoPostBack="true"
                                OnTextChanged="txb_TextChanged" /></asp:TableCell>
                        </asp:TableRow>
                        <asp:TableRow BackColor="#D0F0C0">
                            <asp:TableCell HorizontalAlign="Right" ColumnSpan="2"></asp:TableCell>
                        </asp:TableRow>

                        <asp:TableRow BackColor="#D0F0C0">
                            <asp:TableCell HorizontalAlign="Right" ColumnSpan="2" Font-Italic="true">Properties of the <B>Filter</B> for Displaying <B>Flight Operations</B> from which Invoices are to be Generated:</asp:TableCell>
                        </asp:TableRow>
                        <asp:TableRow BackColor="#EDF8D7">
                            <asp:TableCell HorizontalAlign="Right">Flight operations that occurred on or after this "From" Date:</asp:TableCell>
                            <asp:TableCell HorizontalAlign="Center"><asp:TextBox ID="txbDFrom" runat="server" TextMode="Date" AutoPostBack="true" OnTextChanged="txb_TextChanged"
                                ToolTip="Create invoices for flight operation dates starting at this date" /></asp:TableCell>
                        </asp:TableRow>
                        <asp:TableRow BackColor="#EDF8D7">
                            <asp:TableCell HorizontalAlign="Right">Flight operations that occurred up to this "To" Date:</asp:TableCell>
                            <asp:TableCell HorizontalAlign="Center"><asp:TextBox ID="txbDTo" runat="server" TextMode="Date" AutoPostBack="true" OnTextChanged="txb_TextChanged"
                                ToolTip="Create invoices for flight operation dates ending with this date" /></asp:TableCell>
                        </asp:TableRow>
                        <asp:TableRow BackColor="#EDF8D7">
                            <asp:TableCell HorizontalAlign="Right">For which Member(s)?</asp:TableCell>
                            <asp:TableCell HorizontalAlign="Center">
                                <asp:DropDownList ID="DDL_Member" runat="server" AutoPostBack="true" EnableViewState="true" ViewStateMode="Enabled"
                                                    DataTextField="sDisplayName" DataValueField="ID" OnPreRender="DDL_Member_PreRender" OnSelectedIndexChanged="DDL_Member_SelectedIndexChanged" >
                                    <asp:ListItem Value="0" Text=" ALL">ALL</asp:ListItem>
                                </asp:DropDownList>
                            </asp:TableCell>
                        </asp:TableRow>
                        <asp:TableRow BackColor="#EDF8D7">
                            <asp:TableCell HorizontalAlign="Right">Show only those flights for which an invoice still needs to be generated</asp:TableCell>
                            <asp:TableCell HorizontalAlign="Center"><asp:CheckBox ID="chbInvReq" runat="server" Checked="true" AutoPostBack="true"
                                OnCheckedChanged="chbInvReq_CheckedChanged" EnableViewState="true" ViewStateMode="Enabled" /></asp:TableCell>
                        </asp:TableRow>
                        <asp:TableRow BackColor="#EDF8D7">
                            <asp:TableCell HorizontalAlign="Right" ColumnSpan="2">The current settings would generate invoices for <asp:Label ID="lblNumFlOps" runat="server" Text="0" /> flight operation(s) and
                                <asp:Label ID="lblNumAv" runat="server" Text="0" /> aviator(s).</asp:TableCell>
                        </asp:TableRow>
                        <asp:TableRow BackColor="#EDF8D7">
                            <asp:TableCell HorizontalAlign="Right">Generate those invoices:</asp:TableCell>
                            <asp:TableCell HorizontalAlign="Center">
                                <asp:Button ID="pbGenInv" runat="server" Text="Generate Invoices" OnClick="pbGenInv_Click" ControlStyle-Font-Size="X-Small" Height="15" />
                            </asp:TableCell>
                        </asp:TableRow>
                        <asp:TableRow BackColor="#D0F0C0">
                            <asp:TableCell HorizontalAlign="Right" ColumnSpan="2"></asp:TableCell>
                        </asp:TableRow>

                        <asp:TableRow BackColor="#D0F0C0">
                            <asp:TableCell HorizontalAlign="Right" ColumnSpan="2" Font-Italic="true">Properties of the <B>Filter</B> for Displaying <B>Invoices</B> that have already been Generated:</asp:TableCell>
                        </asp:TableRow>
<%--                        <asp:TableRow BackColor="#EDF8D7">
                            <asp:TableCell HorizontalAlign="Right">Invoices dated on or after "From" Date</asp:TableCell>
                            <asp:TableCell HorizontalAlign="Center"><asp:TextBox ID="txbDFromInv" runat="server" TextMode="Date" AutoPostBack="true" OnTextChanged="txb_TextChanged"
                                ToolTip="Display invoices with dates starting with this date" /></asp:TableCell>
                        </asp:TableRow>
                        <asp:TableRow BackColor="#EDF8D7">
                            <asp:TableCell HorizontalAlign="Right">Invoices dated up to "To" Date</asp:TableCell>
                            <asp:TableCell HorizontalAlign="Center"><asp:TextBox ID="txbDToInv" runat="server" TextMode="Date" AutoPostBack="true" OnTextChanged="txb_TextChanged"
                                ToolTip="Display invoices with dates up to this date" /></asp:TableCell>
                        </asp:TableRow>--%>
                        <asp:TableRow BackColor="#EDF8D7">
                            <asp:TableCell HorizontalAlign="Right">Invoices covering flight operations that occurred on or after this "From" Date:</asp:TableCell>
                            <asp:TableCell HorizontalAlign="Center">
                                <asp:TextBox ID="txbDFromInvFO" runat="server" TextMode="Date" AutoPostBack="true" OnTextChanged="txb_TextChanged"
                                    ToolTip="Display invoices with dates starting with this date" />
                            </asp:TableCell>
                        </asp:TableRow>
                        <asp:TableRow BackColor="#EDF8D7">
                            <asp:TableCell HorizontalAlign="Right">Invoices covering flight operations that occurred up to this "To" Date:</asp:TableCell>
                            <asp:TableCell HorizontalAlign="Center"><asp:TextBox ID="txbDToInvFO" runat="server" TextMode="Date" AutoPostBack="true" OnTextChanged="txb_TextChanged"
                                ToolTip="Display invoices with dates up to this date" /></asp:TableCell>
                        </asp:TableRow>
                        <asp:TableRow BackColor="#EDF8D7">
                            <asp:TableCell HorizontalAlign="Right">Which Member(s)?</asp:TableCell>
                            <asp:TableCell HorizontalAlign="Center">
                                <asp:DropDownList ID="DDL_MemberInv" runat="server" AutoPostBack="true" EnableViewState="true" ViewStateMode="Enabled"
                                                    DataTextField="sDisplayName" DataValueField="ID" OnPreRender="DDL_Member_PreRender" OnSelectedIndexChanged="DDL_Member_SelectedIndexChanged" >
                                    <asp:ListItem Value="0" Text=" ALL">ALL</asp:ListItem>
                                </asp:DropDownList>
                            </asp:TableCell>
                        </asp:TableRow>
                        <asp:TableRow BackColor="#EDF8D7">
                            <asp:TableCell HorizontalAlign="Right">Show only Invoices which are still open</asp:TableCell>
                            <asp:TableCell HorizontalAlign="Center"><asp:CheckBox ID="chbInvOpenOnly" runat="server" Checked="true" AutoPostBack="true"
                                OnCheckedChanged="chbInvOpenOnly_CheckedChanged" EnableViewState="true" ViewStateMode="Enabled" /></asp:TableCell>
                        </asp:TableRow>
                        <asp:TableRow BackColor="#EDF8D7">
                            <asp:TableCell HorizontalAlign="Right" ColumnSpan="2">The current settings display <asp:Label ID="lblNumInv" runat="server" Text="0" /> invoice lines.</asp:TableCell>
                        </asp:TableRow>

                        <asp:TableRow BackColor="#D0F0C0">
                            <asp:TableCell HorizontalAlign="Right" ColumnSpan="2">&nbsp;</asp:TableCell>
                        </asp:TableRow>
                        <asp:TableRow BackColor="#EDF8D7">
                            <asp:TableCell HorizontalAlign="Right">Reset General/Control Data to Default Values:</asp:TableCell>
                            <asp:TableCell HorizontalAlign="Center"><asp:Button ID="pbReset" runat="server" Text="Reset" OnClick="pbReset_Click" ControlStyle-Font-Size="X-Small" Height="15" /></asp:TableCell>
                        </asp:TableRow>
                    </asp:Table>
                </Content>
            </ajaxToolkit:AccordionPane>
            <ajaxToolkit:AccordionPane ID="AccPFlyOps" runat="server" >
                <Header>
                    <asp:Label ID="lblAccPFlyOps" runat="server" Text="List of Flight Operations That Could Give Rise to Invoices"  />
                </Header>
                <Content>
                    Page No. <asp:Label ID="lblPageNum" runat="server" Text="1" />
                        <div class="gvclass">
                    <asp:GridView runat="server" ID="gvFlyOps" AutoGenerateColumns="false"
                        GridLines="None" CssClass="SoarNPGridStyle"
                        ShowHeaderWhenEmpty="true" Font-Size="Small"
                        OnRowCommand="gvFlyOps_RowCommand"
                        OnPageIndexChanging="gvFlyOps_PageIndexChanging"
                        OnRowDataBound="gvFlyOps_RowDataBound" >
<%--                        AllowPaging="true" PageSize="20"--%>
<%--                        <PagerSettings Mode="NextPreviousFirstLast" Position="TopAndBottom" Visible="true"
                            FirstPageText="&nbsp;First Page&nbsp;" LastPageText="&nbsp;Last Page&nbsp;"
                            NextPageText="&nbsp;Next Page&nbsp;" PreviousPageText="&nbsp;Previous&nbsp;" />
                        <PagerStyle CssClass="SoarNPpaging" />--%>
                        <Columns>
                            <asp:TemplateField ItemStyle-HorizontalAlign="Center">
                                <HeaderTemplate>
                                    <asp:Label ID="lblHID" runat="server" Text="ID" ToolTip="Internal flight operation identifier" />
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="lblID" runat="server" Text='<%# Eval("ID") %>' /> 
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField ItemStyle-HorizontalAlign="Center">
                                <HeaderTemplate>
                                    <asp:Label ID="lblHDate" runat="server" Text="Date and Takeoff Time" ToolTip="Date and time of takeoff of flight operation" />
                                </HeaderTemplate>
                                <ItemTemplate>
<%--                                    <asp:Label ID="lblDate" runat="server" Text='<%# Eval("Date") %>' />--%>
                                    <asp:Label ID="lblDate" runat="server" Text='<%# TSoar.CustFmt.sFmtDate((DateTime)Eval("Date"),TSoar.CustFmt.enDFmt.DateAndTimeMin) %>' /> 
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField ItemStyle-HorizontalAlign="Center">
                                <HeaderTemplate>
                                    <asp:Label ID="lblHLaunchMethod" runat="server" Text="Launch Method" />
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="lblLaunchMethod" runat="server" Text='<%# Eval("sLaunchMethod") %>' /> 
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField ItemStyle-HorizontalAlign="Center">
                                <HeaderTemplate>
                                    <asp:Label ID="lblHRElAltit" runat="server" Text="Rel. Alt. ft" ToolTip="Release altitude in feet" />
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="lblRelAltit" runat="server" Text='<%# Eval("Rel_Alt_ft") %>' /> 
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField ItemStyle-HorizontalAlign="Center">
                                <HeaderTemplate>
                                    <asp:Label ID="lblHDur" runat="server" Text="Dur. min" ToolTip="Glider flight duration in minutes"/>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="lblDur" runat="server" Text='<%# Eval("Dur_min") %>' /> 
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField ItemStyle-HorizontalAlign="Center">
                                <HeaderTemplate>
                                    <asp:Label ID="lblHAviator" runat="server" Text="Aviator" ToolTip="Name of person in cockpit" />
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="lblAviator" runat="server" Text='<%# Eval("Aviator") %>' /> 
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField ItemStyle-HorizontalAlign="Center">
                                <HeaderTemplate>
                                    <asp:Label ID="lblHRole" runat="server" Text="Role" ToolTip="The aviator's role" />
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="lblRole" runat="server" Text='<%# Eval("Role") %>' /> 
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField ItemStyle-HorizontalAlign="Center">
                                <HeaderTemplate>
                                    <asp:Label ID="lblHPercCharge" runat="server" Text="%" ToolTip="Percent Charge to this Aviator"/>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="lblPercCharge" runat="server" Text='<%# Eval("Perc") %>' /> 
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField ItemStyle-HorizontalAlign="Center">
                                <HeaderTemplate>
                                    <asp:Label ID="lblHCC" runat="server" Text="CC" ToolTip="Charge Code:
&nbsp;A&#9;(free, Aircraft relocation)
&nbsp;C&#9;(introduCtory Membership)
&nbsp;F&#9;(Full charge)
&nbsp;G&#9;(Glider only)
&nbsp;I&#9;(free, Instructor currency)
&nbsp;M&#9;(Manual data entry - ignored during automatic invoice generation)
&nbsp;P&#9;(free, tow pilot introductory flight)
&nbsp;S&#9;(Season start)
&nbsp;T&#9;(Tow only)
&nbsp;X&#9;(free, other - eXplain)
"/>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="lblCC" runat="server" Text='<%# Eval("CC") %>' /> 
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField ItemStyle-HorizontalAlign="Center">
                                <HeaderTemplate>
                                    <asp:Label ID="lblHInv2go" runat="server" Text="Status" ToolTip="Invoice creation status:
&nbsp;-1&#9;Number of potential invoices is yet to be calculated
&nbsp;0, 1, 2&#9;Number of invoices to be created from this flight operation
"/>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="lblInv2go" runat="server" Text='<%# Eval("Inv2go") %>' /> 
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:ButtonField ButtonType="Button" Text="Take Action" CommandName="TakeAction" HeaderText="Take Action" ControlStyle-Font-Size="X-Small" ControlStyle-Height="15" />
                        </Columns>
                    </asp:GridView>
                    </div>
                </Content>
            </ajaxToolkit:AccordionPane>
            <ajaxToolkit:AccordionPane ID="AccPInvDet" runat="server" >
                <Header>
                    <asp:Label ID="lblAccPHInvDet" runat="server" Text="Invoice Details and Transmission of Invoices to QuickBooks Online"  />
                </Header>
                <Content>
                    <div id="div2QBO" runat="server" visible="false"><a href="../../AdminFin/Connect2QBO.aspx">Connect to QuickBooks Online</a></div>
                    <div class="gvclass" id="divgv" runat="server" visible="true">
                        <asp:GridView runat="server" ID="gvInv" AutoGenerateColumns="false"
                            GridLines="None" CssClass="SoarNPGridStyle" RowStyle-Font-Size="X-Small"
                            ShowHeaderWhenEmpty="true" Font-Size="Small"
                            OnRowCommand="gvInv_RowCommand" >
                            <Columns>
                                <asp:TemplateField ItemStyle-HorizontalAlign="Center">
                                    <HeaderTemplate>
                                        Internal ID
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:Label ID="lblID" runat="server" Text='<%# Eval("ID") %>' />
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        Invoice Date
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:Label ID="lblIInvDate" runat="server" Text='<%# TSoar.CustFmt.sFmtDate((DateTimeOffset)Eval("DInvoice"),TSoar.CustFmt.enDFmt.DateOnly) %>' />
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        Member Name
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:Label ID="lblIDisplayName" runat="server" Text='<%# Eval("sDisplayName") %>' />
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <asp:Label runat="server" Text="From Date"
                                            ToolTip="The invoice is intended to cover a time interval starting at 'From Date' and ending at 'To Date'" />
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:Label ID="lblIDFrom" runat="server" Text='<%# TSoar.CustFmt.sFmtDate((DateTimeOffset)Eval("DFrom"),TSoar.CustFmt.enDFmt.DateOnly) %>' />
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <asp:Label runat="server" Text="To Date"
                                            ToolTip="The invoice is intended to cover a time interval starting at 'From Date' and ending at 'To Date'" />
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:Label ID="lblIDTo" runat="server" Text='<%# TSoar.CustFmt.sFmtDate((DateTimeOffset)Eval("DTo"),TSoar.CustFmt.enDFmt.DateOnly) %>' />
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField ItemStyle-HorizontalAlign="Center">
                                    <HeaderTemplate>
                                        <asp:Label runat="server" Text="Closed?"
                                            ToolTip="Unchecked: the invoice is open; checked: the invoice is closed" />
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:CheckBox ID="chbClosed" runat="server" Checked='<%# Eval("bClosed") %>' Enabled="false" />
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:ButtonField ButtonType="Button" Text="Take Action" CommandName="TakeAction" HeaderText="Take Action" ControlStyle-Font-Size="X-Small" ControlStyle-Height="15" />

                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <asp:Label runat="server" Text="Invoice Line Description"
                                            ToolTip="rel = Release Altitude in feet; Dur = Duration in minutes; CC = charge code" />
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:Label ID="lblIDescr" runat="server" Text='<%# Eval("sDescription") %>' />
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField ItemStyle-HorizontalAlign="Right">
                                    <HeaderTemplate>
                                        $ Charge
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:Label ID="lblICharge" runat="server" Text='<%# ((decimal)Eval("mUnitPrice")).ToString("F2") %>' />
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField ItemStyle-HorizontalAlign="Right">
                                    <HeaderTemplate>
                                        $ Invoice Total
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:Label ID="lblITotal" runat="server" Text='<%# ((decimal)Eval("InvTotal")).ToString("F2") %>' />
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </div>
                </Content>
            </ajaxToolkit:AccordionPane>
        </Panes>            
    </ajaxToolkit:Accordion>

    <div id="ModalPopupExtender">
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

    <div id="ModPopExtContextMenu">
        <asp:LinkButton ID="LinkButton1" runat="server" Text="T" CssClass="displayNone" />
        <ajaxToolkit:ModalPopupExtender ID="ModPopExtCM" runat="server"
            TargetControlID="LinkButton1" PopupControlID="MPE_PanelCM"
            BackgroundCssClass="background" />
        <asp:Panel ID="MPE_PanelCM" runat="server" CssClass="popup" style="display:none;" HorizontalAlign="Center">
            For transaction with ID <asp:Label ID="lblMPEOpsID" runat="server" Text="0" />:
            <p> <asp:Button ID="pbDoInvoice" runat="server" Text="Create Invoice(s)" OnClick="pbCM_Click" ToolTip="Create invoice(s) from this flight operation" /><br />
                <asp:Button ID="pbSetFirst" runat="server" Text="Set First Operation" OnClick="pbCM_Click" ToolTip="Use this flight operation's begin date as the 'From' date for creating a series of invoices" /><br />
                <asp:Button ID="pbSetLast" runat="server" Text="Set Last Operation" OnClick="pbCM_Click" ToolTip="Use this flight operation's begin date as the 'To' date for creating a series of invoices" /><br />
                <asp:Button ID="pbCancel" runat="server" Text="Cancel" OnClick="pbCM_Click" ToolTip="Do nothing and return to the list of flight operations." /></p>
        </asp:Panel>
    </div>

    <div id="ModPopExtContextMenuInvoice">
        <asp:LinkButton ID="LinkButton2" runat="server" Text="T" CssClass="displayNone" />
        <ajaxToolkit:ModalPopupExtender ID="ModPopExtCMInv" runat="server"
            TargetControlID="LinkButton2" PopupControlID="MPE_PanelCMInv"
            BackgroundCssClass="background" />
        <asp:Panel ID="MPE_PanelCMInv" runat="server" CssClass="popup" style="display:none;" HorizontalAlign="Center">
            For invoice with ID <asp:Label ID="lblMPEInvId" runat="server" Text="0" />:
            <p> <asp:Button ID="pbXmit" runat="server" Text="Xmit to QBO" OnClick="pbCMInv_Click" ToolTip="Transmit to QuickBooks Online" /><br />
                <asp:Button ID="pbDelete" runat="server" Text="Delete" OnClick="pbCMInv_Click" ToolTip="Delete the entire invoice" /><br />
                <asp:Button ID="pbSet2Open" runat="server" Text="Set to Open" OnClick="pbCMInv_Click" ToolTip="Set invoice open/closed status to 'Open'" /><br />
                <asp:Button ID="pbSet2Closed" runat="server" Text="Set to Closed" OnClick="pbCMInv_Click" ToolTip="Set invoice open/closed status to 'Closed'" /><br />
                <asp:Button ID="pbCancelInv" runat="server" Text="Cancel" OnClick="pbCMInv_Click" ToolTip="Do nothing and return to the list of invoice details." /></p>
        </asp:Panel>
    </div>

    <div id="ModPopExtHelp">
        <%-- ModalPopupExtender for displaying FlyActInvoice Help --%>
        <asp:LinkButton ID="TargetHelp" runat="server" Text="T" CssClass="displayNone" />
        <ajaxToolkit:ModalPopupExtender ID="ModPopExt_Help" runat="server"
            TargetControlID="TargetHelp" PopupControlID="MPE_Pnl_Help"
            BackgroundCssClass="background"
             />
        <asp:Panel ID="MPE_Pnl_Help" runat="server" CssClass="popup" style="display:none;" HorizontalAlign="Center"
                Width="600" Height="850">
            <p><asp:Button ID="pbFDone" runat="server" Text="Done with Help" OnClick="pbFDone_Click" /></p>
            <ajaxToolkit:Accordion
                ID="AccordionRatesHelp"
                runat="Server"
                SelectedIndex="0"
                HeaderCssClass="accordionHeader"
                HeaderSelectedCssClass="accordionHeaderSelected"
                ContentCssClass="accordionContent"
                AutoSize="None"
                FadeTransitions="true"
                TransitionDuration="250"
                FramesPerSecond="40"
                RequireOpenedPane="false" >
                <Panes>
                    <ajaxToolkit:AccordionPane ID="AccPGenDat1" runat="server" >
                        <Header>
                            Invoices Help, Page 1
                        </Header>
                        <Content>
                            <div class="leftAlign">
                                <p>There are three sections:</p>
                                <ol>
                                    <li>General/Control Data</li>
                                    <li>List of Flight Operations That Could Give Rise to Invoices</li>
                                    <li>Invoice Details and Transmission of Invoices to QuickBooks Online</li>
                                </ol>
                                Click on the headers of these sections to open and close them; they behave kind of like an accordion's bellows.
                                <h4>1. General/Control Data</h4>
                                <p>In this section, you control which flight operations are to be included during invoice generation,
                                    by date range and by aviator/club member. Also, you control the date when payment on generated invoices is due by specifying the number of days after the invoice date.</p>
                                <p>One invoice is always associated with one club member. When two aviators split the cost of a flight, the flight will show up in two different invoices.</p>
                                <p>A flight operation causes two line items to be generated in an invoice, one for the launch or tow, and the other for flying/renting the glider.
                                    The line items are added to an open invoice for the pilot if such an invoice already exists; if not then a new invoice is opened.
                                    Here are some conventions regarding invoices for flight operations:
                                </p>
                                <ol>
                                    <li> Generally, one invoice exists for all flight operations for one person and one day.</li>
                                    <li> It is ok for more than one invoice to exist for one person and one day.</li>
                                    <li> It is NOT ok for one invoice to cover more than one day of flight operations
                                         (otherwise we run into problems when figuring out the monthly actual flying charges and comparing them to minimum monthly flying charges)</li>
                                    <li> The date of the invoice is the date of those flight operations (handled automatically by website software).</li>
                                    <li> The date of each invoice line (the 'service date') is also the date of those flight operations (handled automatically by website software).</li>
                                    <li> Generally, there are two invoice lines per flight operation: one for the launch/tow of the glider, and one for the use/rental of the glider (handled automatically by website software).</li>
                                </ol>
                            </div>
                        </Content>
                    </ajaxToolkit:AccordionPane>
                    <ajaxToolkit:AccordionPane ID="AccPGenDat2" runat="server" >
                        <Header>
                            Invoices Help, Page 2: List of Flight Operations
                        </Header>
                        <Content>
                            <div class="leftAlign">
                                <h4>2. List of Flight Operations That Could Give Rise to Invoices</h4>
                                <p>Shown here are the flight operations as filtered by the parameters in Section/Page 1.
                                    Column `Status` indicates the number of invoice entries (1 entry = 2 lines [one for launch, one for glider rental) that still need to be generated for this flight operation:</p>
                                <ul>
                                    <li>-1 - The number of invoice entries still needs to be calculated</li>
                                    <li>0 - all invoice entries have been generated for this flight operation.</li>
                                    <li>1 - either: only one invoice entry will be generated for this flight operation because the glider was occupied by only one person; or: 
                                        the glider was occupied by two aviators - the invoice entry for one aviator has been generated, the invoice entry for the other aviator still needs to be generated.
                                        </li>
                                    <li>2 - two invoice entries will be generated for this flight operation because the glider was occupied by two persons</li>
                                    <li>3 or greater - Should never happen - it would be an error condition. This software does not handle gliders with more than two seats.</li>
                                </ul>
                                <p>In the last column there is an action button for each flight operation. Clicking it shows these options:</p>
                                <ul>
                                    <li>Create Invoice(s): invoice entries are generated only for this flight operation. Most of the time, though, you should generate invoice entries from the
                                        `Generate Invoices` button in the first (General/Control Data) section.
                                    </li>
                                    <li>Set First Operation - The date of this flight operation is used to set the `From Date` in the General/Control Data section.</li>
                                    <li>Set Last Operation - The date of this flight operation is used to set the `To Date` in the General/Control Data section.</li>
                                    <li>Cancel - Do nothing</li>
                                </ul>
                            </div>
                        </Content>
                    </ajaxToolkit:AccordionPane>
                    <ajaxToolkit:AccordionPane ID="AccPGenDat3" runat="server" >
                        <Header>
                            Invoices Help, Page 3: Invoice Details
                        </Header>
                        <Content>
                            <div class="leftAlign">
                            <h4>3. Invoice Details and Transmission of Invoices to QuickBooks Online</h4>
                                <p>After invoices and invoice entries/lines have been generated, those details are shown in this section.  The first line of each invoice has a `Take Action` button
                                which allows you to transmit the invoice to QuickBooks Online (QBO), delete it, or change the invoice's status to open or closed.</p>
                                <p>An open invoice can continue to accept additional invoice entries; a closed invoice cannot. After successfully transmitting an invoice to QBO
                                    its status is set to `closed` by the software. You should not mess with this status unless you are sure that you are not going to cause confusion. For example, you could have
                                    deleted from QuickBooks Online a previously transmitted invoice, and you wish to add entries to it and then retransmit it; before you can add to it, you would have to
                                    set it to open status. Another example would be when you have generated an invoice but you don't want it transmitted to QBO: set its status to 'closed' manually.
                                </p>
                                <p>In order to be able to transmit invoices to QBO, a secure connection from this website to QBO has to be established. What is needed for that to work:</p>
                                <ul>
                                    <li>Intuit QuickBooks Online user credentials (email address and password): can be obtained for example by being added as a user by the PSSA QBO master administrator
                                        (Robert Rackl at the time of this writing in April 2019).</li>
                                    <li>Go to <a href="../../AdminFin/Connect2QBO.aspx">the Connect2QBO page</a>. Once there, click on the `Make Connection` button. Another window will appear
                                        for entering your credentials; when they are accepted, a `Connect` button will appear; click on it. You know that you have connected successfully when
                                        the Connect window disappears and the
                                        'Make Connection' button has been replaced with three other buttons. Return to this (Flying Activities Invoices) page in order to transmit invoices.
                                        </li>
                                    <li>The connection stays alive until it times out, after approximately one hour of non-use. It is also possible that that extra Connect window
                                        shows the 'Connect' button right away without asking for credentials. That happens when you have supplied your credentials a relatively 
                                        short while ago.
                                    </li>
                                </ul>
                            </div>
                            </div>
                        </Content>
                    </ajaxToolkit:AccordionPane>
                </Panes>            
            </ajaxToolkit:Accordion>
        </asp:Panel>
    </div>

</asp:Content>

<asp:Content ID="FooterContent" ContentPlaceHolderID="ContentPlaceHolderFooter" runat="server">
    <%: Title %>
</asp:Content>