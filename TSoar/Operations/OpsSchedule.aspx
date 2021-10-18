<%@ Page Title="Operations Schedule" Language="C#" MasterPageFile="~/mTSoar.Master" AutoEventWireup="true" CodeBehind="OpsSchedule.aspx.cs" Inherits="TSoar.Operations.OpsSchedule" %>

<asp:Content ID="HeaderContent" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="server">
    <asp:Label runat="server" Text="Schedule of Operations" Font-Italic="true" />
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="ContentPlaceHolderMain" runat="server">
    <ul>
        <li><a href="Operations.aspx">Go to 'Operations'</a></li>
        <li><a href="OpsSchedDates.aspx">Work with Flight Operations List of Flying Days</a></li>
    </ul>
    <ajaxToolkit:Accordion
        ID="AccordionOpsSchedule"
        runat="Server"
        SelectedIndex="1"
        HeaderCssClass="accordionHeader"
        HeaderSelectedCssClass="accordionHeaderSelected"
        ContentCssClass="accordionContent"
        AutoSize="None"
        FadeTransitions="true"
        TransitionDuration="250"
        FramesPerSecond="40"
        RequireOpenedPane="false">
        <Panes>
            <ajaxToolkit:AccordionPane ID="AccOpsSchHelp" runat="server" >
                <Header> Help </Header>
                <Content>
                    <p>The <b>Schedule of Signups</b> is organized in a <b>table</b> as follows:</p>
                    <ul>
                        <li><u>Vertical</u> direction going down: table rows with dates of days of operations in chronological ascending order</li>
                        <li><u>Horizontal</u> direction going across: table columns:
                            <ul>
                                <li>First three columns: the <u>date</u>, the day of the week of that date, and Notes pertaining to the day of operations</li>
                                <li>Remaining columns: Signup Categories. There are three <u>kinds</u> of categories:
                                    <ol>
                                        <li><b>Role</b>, such as Field Manager, Tow Pilot, Instructor, ...</li>
                                        <li><b>Equipment</b>, such as name of glider or towing equipment, ...</li>
                                        <li><b>Activity</b>, such as instruction, local or mountain flying, introductory ride, ...</li>
                                    </ol>
                                </li>
                            </ul>
                        </li>
                    </ul>
                    <p>The blue squares in table cells are clickable <b>buttons</b>. When you click one a window pops up:
                        <ul>
                            <li>If the cell you clicked does not yet have any data in it, then you can <u>add</u> a new signup.</li>
                            <li>If the cell you clicked already has data in it, then you can update or <u>edit</u> it, or you can <u>delete</u> the signup altogether.</li>
                        </ul>
                    </p>
                    <p>When a name in a signup is in ordinary black font there are no remarks that go with it. When a name in a signup is in
                        <span style="font-weight:bold;color:darkmagenta">bold magenta</span> font
                        there are <b>remarks</b> associated; see the next paragraph to see them.
                    </p>
                    <p><b>Hovering</b> the mouse pointer ...
                        <ul>
                            <li> ... over table cells with a blue button provides signup data in <u>"tool tips"</u> (temporarily popped-up textbox):
                                date, category, person's name, any remarks.</li>
                            <li> ... over table header cells to the right of the three date columns: the tool tip includes the <u>Kind</u> of signup category.</li>
                            <li>Some items in popped up windows also respond to mouse hovers with explanatory tool tips.</li> <%-- // SCR 221 --%>
                        </ul>
                    </p>
                    <p>The <b>granularity</b> of the operations schedule is one day. Please put hourly schedule information into the remarks for the signup.</p>
                    <p>One person may sign up only once for one date in the same category. One person may sign up for one date for as many different categories as desired.</p>
                    <p> Use the <b>remarks</b> for a signup for anything relevant, such as:
                        <ul>
                            <li>Time of arrival at the airfield, time of departure</li>
                            <li>Names of non-members or visitors coming to the airfield</li>
                            <li>Times of scheduled demonstration flights</li>
                            <li>etc., etc., ...</li>
                        </ul>
                    </p>
                </Content>
            </ajaxToolkit:AccordionPane>
            <ajaxToolkit:AccordionPane ID="AccOpsSchedule" runat="server" >
                <Header> Schedule of Signups </Header>
                <Content>
                    <div style="font-size:x-small">Show schedule for this date range: 
                        <b>From</b> <asp:TextBox ID="txbDFrom" runat="server" TextMode="Date" Width="105px" />
                        <b>To</b> <asp:TextBox ID="txbDTo" runat="server" TextMode="Date" Width="105px" />
                        &nbsp;&nbsp;&nbsp; <asp:Button ID="pbDateRangeUpdate" runat="server" Text="Update" OnClick="pbDateRangeUpdate_Click"
                            ToolTip="Click this button after you have changed the 'From' and/or 'To' date to update the schedule table" />
                    </div>
                    <div class="gvclass">
                        <asp:GridView ID="gvOpsSch" runat="server" AutoGenerateColumns="False"
                                GridLines="None" CssClass="SoarNPGridStyle" EmptyDataText="--==>> No Data Found <<==--"
                                OnRowDataBound="gvOpsSch_RowDataBound"
                                OnPageIndexChanging="gvOpsSch_PageIndexChanging" AllowPaging="true" PageSize="35">
                            <PagerSettings Mode="NextPreviousFirstLast" Position="TopAndBottom" Visible="true" FirstPageText="&nbsp;First&nbsp;"
                                LastPageText="&nbsp;Last&nbsp;" NextPageText="&nbsp;Next&nbsp;" PreviousPageText="&nbsp;Previous&nbsp;" />
                            <PagerStyle CssClass="SoarNPpaging" />
                            <Columns>
                                <asp:TemplateField Visible="false">
                                    <HeaderTemplate>
                                        Internal Date ID
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:Label ID="lblIDate" runat="server" Text='<%# Eval("ID") %>' />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <asp:Label ID="lblHDate" runat="server" Text="Date" ToolTip="The Date of the Day of Operations" />
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:Label ID="lblDate" runat="server" Text='<%# TSoar.CustFmt.sFmtDate(((DateTime)Eval("DDate")),TSoar.CustFmt.enDFmt.DateOnly) %>' />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <asp:Label ID="lblHDoW" runat="server" Text="Day of Week" ToolTip="The day of the week of the date to the left" />
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:Label ID="lblDoW" runat="server" Text='<%# ((DateTime)Eval("DDate")).DayOfWeek %>' />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <asp:Label ID="lblHNotes" runat="server" Text="Notes" ToolTip="Notes for Day of Operations" />
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:Label ID="lblNotes" runat="server" Text='<%# Eval("sNote") %>' ToolTip="All flights from and to Bergseth Field, Enumclaw, Washington, except when noted otherwise" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <%# dictColNames[1].sCateg %>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:ImageButton ID="ipb01" ImageUrl="~/i/BlueButton.jpg" runat="server" OnClick="ipb_Click" />
                                        <asp:Label ID="lblI01" runat="server" Visible="false" Text='<%# Eval("I" + dictColNames[1].sCateg) %>' /> <%-- // SCR 221 --%>
                                        <asp:Label ID="lbl01" runat="server" Text='<%# Eval(dictColNames[1].sCateg) %>' />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <%# dictColNames[2].sCateg %>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:ImageButton ID="ipb02" ImageUrl="~/i/BlueButton.jpg" runat="server" OnClick="ipb_Click" />
                                        <asp:Label ID="lblI02" runat="server" Visible="false" Text='<%# Eval("I" + dictColNames[2].sCateg) %>' /> <%-- // SCR 221 --%>
                                        <asp:Label ID="lbl02" runat="server" Text='<%# Eval(dictColNames[2].sCateg) %>' />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <%# dictColNames[3].sCateg %>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:ImageButton ID="ipb03" ImageUrl="~/i/BlueButton.jpg" runat="server" OnClick="ipb_Click" />
                                        <asp:Label ID="lblI03" runat="server" Visible="false" Text='<%# Eval("I" + dictColNames[3].sCateg) %>' /> <%-- // SCR 221 --%>
                                        <asp:Label ID="lbl03" runat="server" Text='<%# Eval(dictColNames[3].sCateg) %>' />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <%# dictColNames[4].sCateg %>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:ImageButton ID="ipb04" ImageUrl="~/i/BlueButton.jpg" runat="server" OnClick="ipb_Click" />
                                        <asp:Label ID="lblI04" runat="server" Visible="false" Text='<%# Eval("I" + dictColNames[4].sCateg) %>' /> <%-- // SCR 221 --%>
                                        <asp:Label ID="lbl04" runat="server" Text='<%# Eval(dictColNames[4].sCateg) %>' />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <%# dictColNames[5].sCateg %>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:ImageButton ID="ipb05" ImageUrl="~/i/BlueButton.jpg" runat="server" OnClick="ipb_Click" />
                                        <asp:Label ID="lblI05" runat="server" Visible="false" Text='<%# Eval("I" + dictColNames[5].sCateg) %>' /> <%-- // SCR 221 --%>
                                        <asp:Label ID="lbl05" runat="server" Text='<%# Eval(dictColNames[5].sCateg) %>' />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <%# dictColNames[6].sCateg %>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:ImageButton ID="ipb06" ImageUrl="~/i/BlueButton.jpg" runat="server" OnClick="ipb_Click" />
                                        <asp:Label ID="lblI06" runat="server" Visible="false" Text='<%# Eval("I" + dictColNames[6].sCateg) %>' /> <%-- // SCR 221 --%>
                                        <asp:Label ID="lbl06" runat="server" Text='<%# Eval(dictColNames[6].sCateg) %>' />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <%# dictColNames[7].sCateg %>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:ImageButton ID="ipb07" ImageUrl="~/i/BlueButton.jpg" runat="server" OnClick="ipb_Click" />
                                        <asp:Label ID="lblI07" runat="server" Visible="false" Text='<%# Eval("I" + dictColNames[7].sCateg) %>' /> <%-- // SCR 221 --%>
                                        <asp:Label ID="lbl07" runat="server" Text='<%# Eval(dictColNames[7].sCateg) %>' />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <%# dictColNames[8].sCateg %>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:ImageButton ID="ipb08" ImageUrl="~/i/BlueButton.jpg" runat="server" OnClick="ipb_Click" />
                                        <asp:Label ID="lblI08" runat="server" Visible="false" Text='<%# Eval("I" + dictColNames[8].sCateg) %>' /> <%-- // SCR 221 --%>
                                        <asp:Label ID="lbl08" runat="server" Text='<%# Eval(dictColNames[8].sCateg) %>' />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <%# dictColNames[9].sCateg %>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:ImageButton ID="ipb09" ImageUrl="~/i/BlueButton.jpg" runat="server" OnClick="ipb_Click" />
                                        <asp:Label ID="lblI09" runat="server" Visible="false" Text='<%# Eval("I" + dictColNames[9].sCateg) %>' /> <%-- // SCR 221 --%>
                                        <asp:Label ID="lbl09" runat="server" Text='<%# Eval(dictColNames[9].sCateg) %>' />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <%# dictColNames[10].sCateg %>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:ImageButton ID="ipb10" ImageUrl="~/i/BlueButton.jpg" runat="server" OnClick="ipb_Click" />
                                        <asp:Label ID="lblI10" runat="server" Visible="false" Text='<%# Eval("I" + dictColNames[10].sCateg) %>' /> <%-- // SCR 221 --%>
                                        <asp:Label ID="lbl10" runat="server" Text='<%# Eval(dictColNames[10].sCateg) %>' />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <%# dictColNames[11].sCateg %>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:ImageButton ID="ipb11" ImageUrl="~/i/BlueButton.jpg" runat="server" OnClick="ipb_Click" />
                                        <asp:Label ID="lblI11" runat="server" Visible="false" Text='<%# Eval("I" + dictColNames[11].sCateg) %>' /> <%-- // SCR 221 --%>
                                        <asp:Label ID="lbl11" runat="server" Text='<%# Eval(dictColNames[11].sCateg) %>' />
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <%# dictColNames[12].sCateg %>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:ImageButton ID="ipb12" ImageUrl="~/i/BlueButton.jpg" runat="server" OnClick="ipb_Click" />
                                        <asp:Label ID="lblI12" runat="server" Visible="false" Text='<%# Eval("I" + dictColNames[12].sCateg) %>' /> <%-- // SCR 221 --%>
                                        <asp:Label ID="lbl12" runat="server" Text='<%# Eval(dictColNames[12].sCateg) %>' />
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <%# dictColNames[13].sCateg %> <%-- // SCR 222 --%>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:ImageButton ID="ipb13" ImageUrl="~/i/BlueButton.jpg" runat="server" OnClick="ipb_Click" /> <%-- // SCR 222 --%>
                                        <asp:Label ID="lblI13" runat="server" Visible="false" Text='<%# Eval("I" + dictColNames[13].sCateg) %>' /> <%-- // SCR 221 --%> <%-- // SCR 222 --%>
                                        <asp:Label ID="lbl13" runat="server" Text='<%# Eval(dictColNames[13].sCateg) %>' /> <%-- // SCR 222 --%>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <%# dictColNames[14].sCateg %> <%-- // SCR 222 --%>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:ImageButton ID="ipb14" ImageUrl="~/i/BlueButton.jpg" runat="server" OnClick="ipb_Click" /> <%-- // SCR 222 --%>
                                        <asp:Label ID="lblI14" runat="server" Visible="false" Text='<%# Eval("I" + dictColNames[14].sCateg) %>' /> <%-- // SCR 221 --%> <%-- // SCR 222 --%>
                                        <asp:Label ID="lbl14" runat="server" Text='<%# Eval(dictColNames[14].sCateg) %>' /> <%-- // SCR 222 --%>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <%# dictColNames[15].sCateg %> <%-- // SCR 222 --%>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:ImageButton ID="ipb15" ImageUrl="~/i/BlueButton.jpg" runat="server" OnClick="ipb_Click" /> <%-- // SCR 222 --%>
                                        <asp:Label ID="lblI15" runat="server" Visible="false" Text='<%# Eval("I" + dictColNames[15].sCateg) %>' /> <%-- // SCR 221 --%> <%-- // SCR 222 --%>
                                        <asp:Label ID="lbl15" runat="server" Text='<%# Eval(dictColNames[15].sCateg) %>' /> <%-- // SCR 222 --%>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </div>
                </Content>
            </ajaxToolkit:AccordionPane>
        </Panes>
    </ajaxToolkit:Accordion>

    <div id="ModalPopExtDiv">
        <%-- ModalPopupExtender, popping up MPE_Panel and dynamically populating lblPopupText --%>
        <asp:LinkButton ID="Target" runat="server" Text="T" CssClass="displayNone" />
        <ajaxToolkit:ModalPopupExtender ID="ModalPopExt" runat="server"
            TargetControlID="Target" PopupControlID="MPE_Panel" BackgroundCssClass="background" />
        <asp:Panel ID="MPE_Panel" runat="server" CssClass="popup" style="display:none;" HorizontalAlign="Center">
            <asp:Label ID="lblPopupText" runat="server" BackColor="#eeb5a2" Font-Bold="true" />
            <br /><br />
            <p> <asp:Button ID="OkButton"     runat="server" OnClick="Button_Click" Text="OK" />&nbsp;&nbsp;
                <asp:Button ID="CancelButton" runat="server" OnClick="Button_Click" Text="Cancel" />&nbsp;&nbsp;
                <asp:Button ID="NoButton"     runat="server" OnClick="Button_Click" Text="No" />&nbsp;&nbsp;
                <asp:Button ID="YesButton"    runat="server" OnClick="Button_Click" Text="Yes" /></p>
        </asp:Panel>
    </div>

    <div id="ModPopExtDiv">
        <%-- ModalPopupExtender ModPopExt, popping up MPE_Pnl --%>
        <asp:SqlDataSource ID="SqlDS_Members" runat="server" ConnectionString="<%$ ConnectionStrings:SqlConn %>"
         SelectCommand="SELECT ID, sDisplayName FROM PEOPLE WHERE LEN(sUserName) > 0 ORDER BY sDisplayName" />
        <asp:LinkButton ID="LinkButton1" runat="server" Text="T" CssClass="displayNone" />
        <ajaxToolkit:ModalPopupExtender ID="ModPopExt" runat="server"
            TargetControlID="LinkButton1" PopupControlID="MPE_Pnl" BackgroundCssClass="background" OnLoad="ModPopExt_Load" />
        <asp:Panel ID="MPE_Pnl" runat="server" CssClass="popup" style="display:none;" HorizontalAlign="Center">
            <br />
            <asp:Label ID="lbliSignup" runat="server" Text="iSignup" Visible="false" /> <%-- // SCR 221 --%>
            <asp:Button ID="pbDismissTop"       runat="server" OnClick="MPE_Click" Text="Cancel" />
            <hr />
            Date: <asp:Label  ID="lblMPEDate" runat="server"   Text="Date" BackColor="#eeb5a2" Font-Bold="true" />
            <br />
            Signup Kind of Category: <asp:Label ID="lblMPEKind" runat="server" Text="Kind" Font-Bold="true" />
            <br />
            Signup Category: <asp:Label ID="lblMPECateg" runat="server" Text="Signup Category" Font-Bold="true" />
            <br />
            Member Name: <asp:DropDownList ID="DDLMPEMembers" runat="server" DataSourceID="SqlDS_Members" DataTextField="sDisplayName"
                DataValueField="ID" ViewStateMode="Enabled" />
<%-- // SCR 221 start --%>
            <hr />
            Name In Schedule: <asp:TextBox ID="txbDiffName" runat="server" TextMode="SingleLine"
                ToolTip="Optionally specify a different name to appear in the schedule instead of the member to whom
 this signup belongs. The different name could even be a list of several comma-separated names." />
<%-- // SCR 221 end --%>
            <br />
            Remarks:
            <br />
            <asp:TextBox ID="txbRemarks" runat="server" Text="_" Width="300px" Height="100px" TextMode="MultiLine" />
            <br /><br />
            Signup Action: &nbsp;&nbsp;
            <asp:Button  ID="pbAdd"           runat="server" OnClick="MPE_Click" Text="Add" />&nbsp;&nbsp;
            <asp:Button  ID="pbUpdate"        runat="server" OnClick="MPE_Click" Text="Update" />&nbsp;&nbsp;
            <asp:Button  ID="pbRemove"        runat="server" OnClick="MPE_Click" Text="Remove" />
            <hr />
            <asp:Button  ID="pbDismissBottom" runat="server" OnClick="MPE_Click" Text="Cancel" />
        </asp:Panel>
    </div>

</asp:Content>

<asp:Content ID="FooterContent" ContentPlaceHolderID="ContentPlaceHolderFooter" runat="server">
    <%: Title %>
</asp:Content>
