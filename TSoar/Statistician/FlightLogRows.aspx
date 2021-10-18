<%@ Page Title="Flight Log Input" Language="C#" MasterPageFile="~/mTSoar.Master" AutoEventWireup="true" CodeBehind="FlightLogRows.aspx.cs" Inherits="TSoar.Statistician.FlightLogRows" %>

<asp:Content ID="ContentHeader" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="server">
    <asp:Label runat="server" Text="Input of Flight Operations Data from Daily Log Sheets" Font-Italic="true" />
</asp:Content>

<asp:Content ID="ContentMain" ContentPlaceHolderID="ContentPlaceHolderMain" runat="server">

    <asp:Button ID="pbReturnT" runat="server" Text="Return to List of Flight Logs" OnClick="pbReturn_MyClick" />

    <ajaxToolkit:Accordion
        ID="AccordionFlightLogRows"
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
            <ajaxToolkit:AccordionPane ID="AccFLRHelp" runat="server" >
                <Header> Help </Header>
                <Content>
                    <h3>Flight By Flight Details</h3>
                    <p>The next accordion pane 'Flights for One Day' first shows the data that has been entered that applies to the whole day ('Daily Flight Log Being Edited'). Below that appears the list
                        of flights on that day. To begin with it is empty. You add flights by filling in the data in the gold/orange box and clicking on the 'Add' button (far on the right; you may need to
                        scroll over there).
                        You do not need to add all flights in one session.
                    </p>
                    <p>When you are done entering all the flights for one day you should 'post' them to the appropriate database tables so that the flights become visible in the
                                <a href="OpsDataInput.aspx">Tree View</a>, and show up in the <a href="../MemberPages/Stats/ClubStats.aspx">Flight Statistics</a>:
                        <ul>
                            <li>Return to the FlightLogInput page (there are two buttons to do so, one at the top of this page, and one at the bottom of the next accordion pane);</li>
                            <li>Look for 'Post' button in the row with the daily flight log you have been working with.</li>
                        </ul>
                    </p>
                    <p>Notes regarding selected columns:
                        <ul>
                            <li>The second column in the flight log grid shows 'Status':
                                <ul>
                                    <li>New: Flight data has been entered. No posting has taken place.</li>
                                    <li>Processed: The flight has been posted. A flight in this status cannot be posted again.</li>
                                    <li>Recalled: The flight had been processed/posted, but its status has been changed so that it can be posted again. It is the user's responsibility
                                        to delete the flight operation first in the <a href="OpsDataInput.aspx">Tree View</a>.
                                    </li>
                                </ul>
                                Normally, the user need not manipulate this status; but it is possible if mistakes need to be corrected.
                            </li>
                            <li>Towing Equipment: The dropdownlist contains those pieces of equipment that have an equipment type
                                that can perform a role with a name that contains the word 'Tow'.
                                The relationship between equipment roles and equipment types is established here: <a href="../Equipment/EquipRolesTypes.aspx">Equipment Roles and Types</a>. </li>
                            <li>Tow Operator:  This dropdownlist contains the names of people who can tow. This is determined as follows:
                                <ul>
                                    <li>Either: The person is currently a club member in a membership category with a name containing the word 'Tow' (such as Dues-Paying Tow Pilot);</li>
                                    <li>Or: The person currently qualifies in a qualification with a name containing the word 'Tow', but does not end with the word 'Tow'.
                                        This excludes qualifications 'Aero Tow' or 'Ground Tow' (they are glider pilot-related), but includes 'PSSA Tow Pilot' or 'Tow Winch Operator'.
                                    </li>
                                </ul>
                            </li>
                            <li>Glider: Choose from a list of pieces of equipment that have been given the equipment type of 'glider' as well as the equipment role 'glider'.
                                The relationship between equipment roles and equipment types is established here: <a href="../Equipment/EquipRolesTypes.aspx">Equipment Roles and Types</a>.
                            </li>
                            <li>Pilot 1: The dropdownlist presents a list of people who are currently club members. Those club members must exist in the
                                <a href="../ClubMembership/CMS_BasicList.aspx"> list of people </a>, and must have an unexpired
                                <a href="../ClubMembership/CMS_ClubFromTo.aspx"> club membership determined here </a>.</li>
                            <li>Pilot 2: same as for Pilot 1. Pilot 1 and Pilot 2 must not be the same person.</li>
                            <li>Release Altitude: the altitude at which the glider separated from the towing equipment.</li>
                            <li>Max Altitude: the maximum altitude achieved by the glider. It is often not known; in that case use a large negative number like -2000
                                (anything less than or equal to -2000). </li>
                            <li>Charge Code: see the <a href="../AdminPages/DBMaint/DBMaint.aspx"> definition of charge codes </a>, and
                                <a href="../Accounting/FinDetails/SalesAR/Rates.aspx"> how charge codes relate to charging rates </a>.</li>
                            <li>The Duration column contains the duration of the glider flight; it is not entered by the user but is computed as the difference in minutes
                                of landing and takeoff times. It can be used for double-checking the glider flight duration that is often indicated in the
                                daily log sheet.
                            </li>
                        </ul>
                    </p>
                    <p>
                    </p>
                    <p>An error is indicated when an attempt is made to post a flight whose takeoff time (within 2 minutes) and location is the same as a flight that has already been posted, i.e., already exists in the database.</p>
                </Content>
            </ajaxToolkit:AccordionPane>        
            <ajaxToolkit:AccordionPane ID="AccFLRData" runat="server" >
                <Header> Flights for One Day </Header>
                <Content>
                    <h4>Daily Flight Log Being Edited</h4>
                    <div class="gvclass">
                        <asp:GridView ID="gvDayFL" runat="server" AutoGenerateColumns="False"
                            GridLines="None" CssClass="SoarNPGridStyle"
                            ShowHeaderWhenEmpty="true"
                            Font-Size="Small">
                            <SelectedRowStyle BackColor="YellowGreen" />
                            <Columns>
                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <asp:Label ID="lblHDFlightOps" runat="server" Text="Daily Flight Log Date" Width="105" ToolTip="Date on which the flights in this log occurred"></asp:Label>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:Label ID="lblIDFlightOps" runat="server" Text='<%# TSoar.CustFmt.sFmtDate((DateTimeOffset)Eval("DFlightOps"),TSoar.CustFmt.enDFmt.DateOnly) %>' Width="105"></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <asp:Label ID="lblHsFldMgr" runat="server" Text="Field Manager(s)" Width="160" ></asp:Label>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:Label ID="lblIsFldMgr" runat="server" Text='<%# Eval("sFldMgr") %>' Width="160" ></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <asp:Label ID="lblHiMainTowEquip" runat="server" Text="Main Tow Equip" Width="150" ToolTip="The tow plane, tow car, or winch used for most of the flights"></asp:Label>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:Label ID="lblIiMainTowEquip" runat="server" Text='<%# Eval("sShortEquipName") %>' Width="150"></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <asp:Label ID="lblHiMainTowOp" runat="server" Text="Main Tow Operator" Width="150" ToolTip="The person that operated towing equipment most of the time"></asp:Label>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:Label ID="lblIiMainTowOp" runat="server" Text='<%# Eval("sDisplayName") %>' Width="150"></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <asp:Label ID="lblHiMainGlider" runat="server" Text="Main Glider" Width="150" ToolTip="The glider used for most of the flights"></asp:Label>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:Label ID="lblIiMainGlider" runat="server" Text='<%# Eval("sMainGliderName") %>' Width="150"></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <asp:Label ID="lblHiMainLaunchMethod" runat="server" Text="Main Launch Method" Width="150" ToolTip="The launch method used for most of the flights"></asp:Label>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:Label ID="lblIiMainLaunchMethod" runat="server" Text='<%# Eval("sMainLaunchMethod") %>' Width="150"></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <asp:Label ID="lblHiMainLocation" runat="server" Text="Main Location" Width="120" ToolTip="The takeoff location used for most of the flights"></asp:Label>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:Label ID="lblIiMainLocation" runat="server" Text='<%# Eval("sLocation") %>' Width="120"></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField ItemStyle-HorizontalAlign="Right">
                                    <HeaderTemplate>
                                        <asp:Label ID="lblHAmtCollected" runat="server" Text="Total $ Collected" ></asp:Label>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:Label ID="lblIAmtCollected" runat="server" Text='<%# ((decimal)Eval("mTColl")).ToString("N2") %>' ></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <asp:Label ID="lblHNotes" runat="server" Text="Notes/Comments" Width="200" ></asp:Label>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:Label ID="lblINotes" runat="server" Text='<%# Eval("sNotes") %>' Width="200" ></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>

                            </Columns>
                        </asp:GridView>
                    </div>
                    <h4>List of Flights in Above Daily Flight Log</h4>
                    Choose which default set to use for adding new flight operations: 
                    <asp:RadioButtonList ID="rblChooseDefaultSet" runat="server" AutoPostBack="true"
                            OnSelectedIndexChanged="rblChooseDefaultSet_SelectedIndexChanged" RepeatDirection="Horizontal" >
                        <asp:ListItem Value="0" Selected="True">Main Items from Flight Log Headings</asp:ListItem>
                        <asp:ListItem Value="1">Last Used</asp:ListItem>
                    </asp:RadioButtonList>
                    <div class="gvclass">
                        <asp:GridView ID="gvFliN" runat="server" AutoGenerateColumns="False"
                            GridLines="None" CssClass="SoarNPGridStyle"
                            OnRowEditing="gvFliN_RowEditing"
                            OnRowDataBound="gvFliN_RowDataBound"
                            OnRowDeleting="gvFliN_RowDeleting" OnRowCancelingEdit="gvFliN_RowCancelingEdit"
                            OnRowUpdating="gvFliN_RowUpdating"
                            ShowHeaderWhenEmpty="true"
                            Font-Size="Small">
                            <Columns>
                                <asp:TemplateField ItemStyle-HorizontalAlign="Center">
                                    <HeaderTemplate>
                                        <asp:Label ID="lblHID" runat="server" Text="Internal Id"></asp:Label>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:Label ID="lblIID" runat="server" Text='<%# Eval("ID") %>' ></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField ItemStyle-HorizontalAlign="Center">
                                    <HeaderTemplate>
                                        <asp:Label ID="lblHcStatus" runat="server" Text="Status"
                                            ToolTip="The status of this row/line in the daily flight log:
N = New (data entered, not yet processed)
P = Processed (data transferred to flight operations tables in the database)
R = Recalled (same as N, but had been status P before)" >
                                        </asp:Label>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:Label ID="lblIcStatus" runat="server" Text='<%# dictStatus[(char)Eval("cStatus")] %>' ></asp:Label>
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <asp:DropDownList ID="DDLDcStatus" runat="server">
                                            <asp:ListItem Text="New" Value="N" Selected="True" />
                                            <asp:ListItem Text="Processed" Value="P" Selected="False" />
                                            <asp:ListItem Text="Recalled" Value="R" Selected="False" />
                                        </asp:DropDownList>
                                    </EditItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <asp:Label ID="lblHiTowEquip" runat="server" Text="Towing Equip" Width="100" ToolTip="The tow plane, tow car, or winch used for launching the glider"></asp:Label>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:Label ID="lblIiTowEquip" runat="server" Text='<%# Eval("sTowEquipName") %>' Width="100"></asp:Label>
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <asp:DropDownList ID="DDLDiTowEquip" runat="server" Width="100" />
                                    </EditItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <asp:Label ID="lblHiTowOperator" runat="server" Text="Tow Operator" Width="100" ToolTip="The person that operated towing equipment"></asp:Label>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:Label ID="lblIiTowOperator" runat="server" Text='<%# Eval("sTowOperName") %>' Width="100"></asp:Label>
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <asp:DropDownList ID="DDLDiTowOperator" runat="server" Width="100" />
                                    </EditItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <asp:Label ID="lblHiGlider" runat="server" Text="Glider" Width="100" ToolTip="The glider/sailplane in this flight operation"></asp:Label>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:Label ID="lblIiGlider" runat="server" Text='<%# Eval("sGliderName") %>' Width="100"></asp:Label>
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <asp:DropDownList ID="DDLDiGlider" runat="server" Width="100" />
                                    </EditItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <asp:Label ID="lblHiLaunchMethod" runat="server" Text="Launch Method" Width="100" ToolTip="The launch method used in this flight operation"></asp:Label>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:Label ID="lblIiLaunchMethod" runat="server" Text='<%# Eval("sLaunchMethod") %>' Width="100"></asp:Label>
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <asp:DropDownList ID="DDLDiLaunchMethod" runat="server" Width="100" />
                                    </EditItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <asp:Label ID="lblHPilot1" runat="server" Text="Pilot 1" Width="100" ToolTip="One of the pilots in the glider; cannot be empty"></asp:Label>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:Label ID="lblIPilot1" runat="server" Text='<%# Eval("sPilot1") %>' Width="100"></asp:Label>
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <asp:DropDownList ID="DDLDPilot1" runat="server" Width="100" />
                                    </EditItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <asp:Label ID="lblHAviatorRole1" runat="server" Text="Role of Pilot 1" Width="100" ToolTip="The role of Pilot 1"></asp:Label>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:Label ID="lblIAviatorRole1" runat="server" Text='<%# Eval("sAviatorRole1") %>' Width="100"></asp:Label>
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <asp:DropDownList ID="DDLDAviatorRole1" runat="server" Width="100" />
                                    </EditItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField ItemStyle-HorizontalAlign="Right" >
                                    <HeaderTemplate>
                                        <asp:Label ID="lblHdPctCharge1" runat="server" Text="% Charge Pilot 1" Width="50"
                                            ToolTip="What percentage of flight charges does Pilot 1 pay? Up to 2 decimal places. No +/- signs. Don't use the percent sign. No leading/trailing spaces."></asp:Label>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:Label ID="lblIdPctCharge1" runat="server" Text='<%# Eval("dPctCharge1") %>' Width="50"></asp:Label>
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <asp:TextBox ID="txbDdPctCharge1" runat="server" Text='<%# Eval("dPctCharge1") %>' TextMode="SingleLine" Width="50" Style="min-width:100%; text-align:right"></asp:TextBox>
                                         <asp:RegularExpressionValidator runat="server" ControlToValidate="txbDdPctCharge1" ValidationExpression="^(?!\s*$)(((100)(\.0{0,2})?)|(^\d{0,2}(\.\d{0,2})?))$"
                                              ErrorMessage="Must be a decimal number between 0 and 100" Display="Dynamic" ForeColor="Red" BorderStyle="Dotted" BorderColor="Red" BorderWidth="4px" />
                                    </EditItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <asp:Label ID="lblHPilot2" runat="server" Text="Pilot 2" Width="100" ToolTip="One of the pilots in the glider; ok to be [none]"></asp:Label>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:Label ID="lblIPilot2" runat="server" Text='<%# Eval("sPilot2") %>' Width="100"></asp:Label>
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <asp:DropDownList ID="DDLDPilot2" runat="server" Width="100" />
                                    </EditItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <asp:Label ID="lblHAviatorRole2" runat="server" Text="Role of Pilot 2" Width="100" ToolTip="The role of Pilot 2; ok to be [none]"></asp:Label>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:Label ID="lblIAviatorRole2" runat="server" Text='<%# Eval("sAviatorRole2") %>' Width="100"></asp:Label>
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <asp:DropDownList ID="DDLDAviatorRole2" runat="server" Width="100" />
                                    </EditItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField ItemStyle-HorizontalAlign="Right" >
                                    <HeaderTemplate>
                                        <asp:Label ID="lblHdPctCharge2" runat="server" Text="% Charge Pilot 2" Width="50"
                                            ToolTip="What percentage of flight charges does Pilot 2 pay? Up to 2 decimal places. No +/- signs. Don't use the percent sign. No leading/trailing spaces."></asp:Label>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:Label ID="lblIdPctCharge2" runat="server" Text='<%# Eval("dPctCharge2") %>' Width="50"></asp:Label>
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <asp:TextBox ID="txbDdPctCharge2" runat="server" Text='<%# Eval("dPctCharge2") %>' TextMode="SingleLine" Width="50" Style="min-width:100%; text-align:right"></asp:TextBox>
                                         <asp:RegularExpressionValidator runat="server" ControlToValidate="txbDdPctCharge2" ValidationExpression="^(?!\s*$)(((100)(\.0{0,2})?)|(^\d{0,2}(\.\d{0,2})?))$"
                                              ErrorMessage="Must be a decimal number between 0 and 100" Display="Dynamic" ForeColor="Red" BorderStyle="Dotted" BorderColor="Red" BorderWidth="4px" />
                                    </EditItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField ItemStyle-HorizontalAlign="Right" >
                                    <HeaderTemplate>
                                        <asp:Label ID="lblHdReleaseAltitude" runat="server" Text="Release Altitude (MSL)" Width="50"
                                            ToolTip="At which altitude did the glider release from the tow? Can sometimes be empty."></asp:Label>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:Label ID="lblIdReleaseAltitude" runat="server" Text='<%# Eval("dReleaseAltitude") %>' Width="50"></asp:Label>
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <asp:TextBox ID="txbDdReleaseAltitude" runat="server" Text='<%# Eval("dReleaseAltitude") %>' TextMode="Number" Width="50" Style="min-width:100%;"></asp:TextBox>
                                         <asp:RegularExpressionValidator runat="server" ControlToValidate="txbDdReleaseAltitude" ValidationExpression="^[-+]?\d{1,9}$"
                                              ErrorMessage="Must be an integer number, optional sign, up to 9 digits" Display="Dynamic" ForeColor="Red" BorderStyle="Dotted" BorderColor="Red" BorderWidth="4px" />
                                    </EditItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField ItemStyle-HorizontalAlign="Right" >
                                    <HeaderTemplate>
                                        <asp:Label ID="lblHdMaxAltitude" runat="server" Text="Max Altitude (MSL)" Width="50"
                                            ToolTip="The glider's maximum altitude during this flight. A large negative number indicates 'unknown'."></asp:Label>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:Label ID="lblIdMaxAltitude" runat="server" Text='<%# Eval("dMaxAltitude") %>' Width="50"></asp:Label>
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <asp:TextBox ID="txbDdMaxAltitude" runat="server" Text='<%# Eval("dMaxAltitude") %>' TextMode="Number" Width="50" Style="min-width:100%;"></asp:TextBox>
                                         <asp:RegularExpressionValidator runat="server" ControlToValidate="txbDdMaxAltitude" ValidationExpression="^[-+]?\d{1,9}$"
                                              ErrorMessage="Must be an integer number, optional sign, up to 9 digits" Display="Dynamic" ForeColor="Red" BorderStyle="Dotted" BorderColor="Red" BorderWidth="4px" />
                                    </EditItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <asp:Label ID="lblHiLocTakeOff" runat="server" Text="Takeoff Location" Width="100" ToolTip="The takeoff location for this flight"></asp:Label>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:Label ID="lblIiLocTakeOff" runat="server" Text='<%# Eval("sLocTakeOff") %>' Width="100"></asp:Label>
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <asp:DropDownList ID="DDLDiLocTakeOff" runat="server" Width="100" />
                                    </EditItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <asp:Label ID="lblHDTakeOff" runat="server" Text="Takeoff Time / Offset to UTC" ToolTip="Time and UTC Offset of Takeoff (to the minute). Offset limited to +/-14:00."></asp:Label>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:Label ID="lblIDTakeOff" runat="server" Text='<%# TSoar.CustFmt.sFmtDate((DateTimeOffset)Eval("DTakeOff"),TSoar.CustFmt.enDFmt.DateAndTimeMinOffset).Substring(11) %>' ></asp:Label>
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <asp:TextBox ID="txbDTakeOffDate" runat="server" Text='<%# TSoar.CustFmt.sFmtDate((DateTimeOffset)Eval("DTakeOff"),TSoar.CustFmt.enDFmt.DateOnly).Replace("/","-") %>'
                                            TextMode="Date" Width="108" Visible="false" ></asp:TextBox>
                                        <asp:TextBox ID="txbDTakeOffTime" runat="server" Text='<%# TSoar.CustFmt.sFmtDate((DateTimeOffset)Eval("DTakeOff"),TSoar.CustFmt.enDFmt.TimeOnlyMin) %>'
                                            TextMode="Time" Width="65" ></asp:TextBox>
                                        <asp:TextBox ID="txbTakeOffOffset" runat="server" TextMode="SingleLine" Text='<%# ((DateTimeOffset)Eval("DTakeOff")).Offset.ToString().Substring(0, 6) %>' Width="38px" Style="text-align:right"
                                            ToolTip="Time offset from Universal Time Coordinated. Change only if it is important (it rarely is). Default comes from Setting 'TimeZoneOffset'. For Pacific Daylight Savings Time use -07:00."></asp:TextBox>
                                        <asp:RegularExpressionValidator runat="server" ControlToValidate="txbTakeOffOffset" ValidationExpression="^[-+]((1[0-4])|(0\d)):[0-5]\d$"
                                            ErrorMessage="Must be a time offset from local to UTC like -08:00" Display="Dynamic" ForeColor="Red" BorderStyle="Dotted" BorderColor="Red" BorderWidth="4px"/>
                                    </EditItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <asp:Label ID="lblHiLocLanding" runat="server" Text="Landing Location" Width="100" ToolTip="The landing location for this flight"></asp:Label>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:Label ID="lblIiLocLanding" runat="server" Text='<%# Eval("sLocLanding") %>' Width="100"></asp:Label>
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <asp:DropDownList ID="DDLDiLocLanding" runat="server" Width="100" />
                                    </EditItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <asp:Label ID="lblHDLanding" runat="server" Text="Landing Time / Offset to UTC" ToolTip="Time and UTC Offset of Landing (to the minute). Offset limited to +/-14:00."></asp:Label>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:Label ID="lblIDLanding" runat="server" Text='<%# TSoar.CustFmt.sFmtDate((DateTimeOffset)Eval("DLanding"),TSoar.CustFmt.enDFmt.DateAndTimeMinOffset).Substring(11) %>' ></asp:Label>
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <asp:TextBox ID="txbDLandingDate" runat="server" Text='<%# TSoar.CustFmt.sFmtDate((DateTimeOffset)Eval("DLanding"),TSoar.CustFmt.enDFmt.DateOnly).Replace("/","-") %>'
                                            TextMode="Date" Width="108" Visible="false" ></asp:TextBox>
                                        <asp:TextBox ID="txbDLandingTime" runat="server" Text='<%# TSoar.CustFmt.sFmtDate((DateTimeOffset)Eval("DLanding"),TSoar.CustFmt.enDFmt.TimeOnlyMin) %>'
                                            TextMode="Time" Width="65" ></asp:TextBox>
                                        <asp:TextBox ID="txbLandingOffset" runat="server" TextMode="SingleLine" Text='<%# ((DateTimeOffset)Eval("DLanding")).Offset.ToString().Substring(0, 6) %>' Width="38px" Style="text-align:right"
                                            ToolTip="Time offset from Universal Time Coordinated. Change only if it is important (it rarely is). Default comes from Setting 'TimeZoneOffset'. For Pacific Daylight Savings Time use -07:00."></asp:TextBox>
                                        <asp:RegularExpressionValidator runat="server" ControlToValidate="txbLandingOffset" ValidationExpression="^[-+]((1[0-4])|(0\d)):[0-5]\d$"
                                            ErrorMessage="Must be a time offset from local to UTC like -08:00" Display="Dynamic" ForeColor="Red" BorderStyle="Dotted" BorderColor="Red" BorderWidth="4px"/>
                                    </EditItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <asp:Label ID="lblHDur" runat="server" Text="Duration" ToolTip="Difference in landing and takeoff times for the glider, in minutes" />
                                    </HeaderTemplate>
                                    <HeaderStyle HorizontalAlign="Center" />
                                    <ItemStyle HorizontalAlign="Right" />
                                    <ItemTemplate>
                                        <asp:Label ID="lblIDur" runat="server" Text='<%# ((TimeSpan)((DateTimeOffset)Eval("DLanding") - (DateTimeOffset)Eval("DTakeOff"))).TotalMinutes.ToString() %>' />
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        - -
                                    </EditItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <asp:Label ID="lblHiChargeCode" runat="server" Text="Charge Code" Width="100" ToolTip="A code for how to charge for this flight"></asp:Label>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:Label ID="lblIiChargeCode" runat="server" Text='<%# Eval("cChargeCode") + " - " + Eval("sChargeCode") %>' Width="100"></asp:Label>
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <asp:DropDownList ID="DDLDiChargeCode" runat="server" Width="100" />
                                    </EditItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField ItemStyle-HorizontalAlign="Right" >
                                    <HeaderTemplate>
                                        <asp:Label ID="lblHmAmtCollected" runat="server" Text="$ Collected" Width="50"
                                            ToolTip="The amount collected for this flight. Up to 2 decimal places. Don't use any currency signs like $. No +/- signs. 3-digits grouping comma ok."></asp:Label>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:Label ID="lblImAmtCollected" runat="server" Text='<%# ((decimal)Eval("mAmtCollected")).ToString("N2")%>' Style="text-align:right"></asp:Label>
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <asp:TextBox ID="txbDmAmtCollected" runat="server" Text='<%# ((decimal)Eval("mAmtCollected")).ToString("N2") %>' TextMode="SingleLine" Width="50" Style="min-width:100%; text-align:right" ></asp:TextBox>
                                        <asp:RegularExpressionValidator runat="server" ControlToValidate="txbDmAmtCollected" ValidationExpression="^((\d{1,3}(,\d{3})*)|(\d*))(\.|\.\d{0,4})?$"
                                            ErrorMessage="Must be a decimal number (optional sign, optional decimal point)" Display="Dynamic" ForeColor="Red" BorderStyle="Dotted" BorderColor="Red" BorderWidth="4px" />
                                    </EditItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <asp:Label ID="lblHsComments" runat="server" Text="Notes/Comments" ></asp:Label>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:Label ID="lblIsComments" runat="server" Text='<%# Eval("sComments") %>' ></asp:Label>
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <asp:TextBox ID="txbDsComments" runat="server" Text='<%# Eval("sComments") %>' ></asp:TextBox>
                                    </EditItemTemplate>
                                </asp:TemplateField>

                                <asp:CommandField ButtonType="Image" ShowEditButton="true" HeaderText="Edit" EditImageUrl="~/i/BlueButton.jpg"
                                    CancelImageUrl="~/i/Cancel.jpg" UpdateImageUrl="~/i/Update.jpg" >
                                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Height="25px" Width="50px" />
                                </asp:CommandField>

                                <asp:CommandField ButtonType="Image" ShowDeleteButton="true" HeaderText="Delete" DeleteImageUrl="~/i/RedButton.jpg">
                                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Height="25px" Width="50px" />
                                </asp:CommandField>

                            </Columns>
                        </asp:GridView>
                    </div>
<%-- Removed per SCR 147                    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; 
                    <asp:Button ID="pbPost" runat="server" Text="Post" OnClick="pbPost_MyClick" OnClientClick="document.location.reload(true)"
                        ToolTip="Post/transfer the flights in this Daily Flight Log to the database tables for flight operations."/>--%>
                </Content>
            </ajaxToolkit:AccordionPane>        
        </Panes>            
    </ajaxToolkit:Accordion>
    <asp:Button ID="pbReturnB" runat="server" Text="Return to List of Flight Logs" OnClick="pbReturn_MyClick" />

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

</asp:Content>

<asp:Content ID="ContentFooter" ContentPlaceHolderID="ContentPlaceHolderFooter" runat="server">
    <%: Title %>
</asp:Content>
