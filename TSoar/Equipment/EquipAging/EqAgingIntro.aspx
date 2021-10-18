<%@ Page Title="Equipment Aging Introduction" Language="C#" MasterPageFile="~/mTSoar.Master" AutoEventWireup="true" CodeBehind="EqAgingIntro.aspx.cs" Inherits="TSoar.Equipment.EquipAging.EqAgingIntro" %>

<asp:Content ID="HeaderContent" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="server">
    <asp:Label runat="server" Text="Equipment Aging Introduction and Help Pages" Font-Italic="true" />
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="ContentPlaceHolderMain" runat="server">

    <ajaxToolkit:Accordion
        ID="AccordionAgingIntro"
        runat="Server"
        SelectedIndex="3"
        HeaderCssClass="accordionHeader"
        HeaderSelectedCssClass="accordionHeaderSelected"
        ContentCssClass="accordionContent"
        AutoSize="None"
        FadeTransitions="true"
        TransitionDuration="250"
        FramesPerSecond="40"
        RequireOpenedPane="false">
        <Panes>
            <ajaxToolkit:AccordionPane ID="AccPIntro" runat="server" >
                <Header> Introduction to Equipment Aging </Header>
                <Content>
                    <div class="HelpText" ><%-- SCR 213 --%>
                        <p>The word "Aging" is used to refer to activities that are required to keep pieces of equipment and their components in good and serviceable shape,
                            physically as well as legally, i.e., mechanical maintenance as well as paperwork including licenses and registrations.
                        </p>
                        <p>Consists of:
                            <ul>
                                <li>Physical Maintenance of Equipment
                                    <ul>
                                        <li>History of maintenance performed, by piece of equipment and its components</li>
                                        <li>Schedule of upcoming maintenance items</li>
                                    </ul>
                                </li>
                                <li>Tracking and renewing required Documentation and Licenses, such as:
                                    <ul>
                                        <li>FAA Registrations</li>
                                        <li>State Aircraft Registrations</li>
                                        <li>License plate renewals for trailers</li>
                                    </ul>
                                </li>
                            </ul>
                        </p>
                        <p>In addition to the <a href="../EquipmentList.aspx">List of Equipment</a>, we use these tools (see 'Links' below):<%-- SCR 218 start --%>
                            <ul>
                                <li>Equipment Components
                                    <ul>
                                        <li>A hierarchical list of items, each representing a physical component of a piece of equipment. Each component has a parent,
                                            and each component may have any number of children.</li>
                                    </ul>
                                </li>
                                <li>Equipment Aging Parameter Sets
                                    <ul>
                                        <li>For a particular item to be tracked, the user specifies parameters such as what kind of action is to be taken, and the time interval between occurrences.
                                            One set of parameters can be attached to any number of equipment components; for example, one parameter set for all annual inspections.
                                        </li>
                                    </ul>
                                </li>
                                <li>Operational Calendars
                                    <ul>
                                        <li>In certain situations, an operational calendar (abbreviated to ops cal) is required. It specifies time intervals (usually measured
                                            in months but can be to the day) when operations with equipment
                                            can take place and when they cannot, such as when aircraft are put into storage over the Winter months. Ops cals are required
                                            when forecasting the next time an equipment aging/maintenance action is required using an extrapolation of past operational data. Each equipment aging item
                                            references one of the ops cals defined by the user.
                                        </li>
                                    </ul>
                                </li>
                                <li>Equipment Aging Items
                                    <ul>
                                        <li>List of items that are to be tracked in terms of their maintenance or renewal.</li>
                                        <li>Each item refers to: (1) an equipment component, (2) an equipment aging parameter set, and (3) an operational calendar.</li>
                                        <li>Each item has beginning and end times.</li>
                                    </ul>
                                </li>
                                <li>Equipment Operating Hours, Operating Cycles, and/or Distance Traveled
                                    <ul>
                                        <li>For equipment that requires keeping track of time in operation (or running time), cycles (one cycle is usually the combination of one takeoff and one landing),
                                            and/or distance traveled, that tracking is done in one of two ways:
                                            <ul>
                                                <li>Automatically from the records of flying activities (running/operating hours and number of cycles only)</li>
                                                <li>Manually by user input: manually input data is in addition to the data from records of flying activities.
                                                    When there are no flying activities data, the manually provided data can be the sole operational data.
                                                </li>
                                            </ul>
                                        </li>
                                    </ul>
                                </li>
                                <li>Equipment Aging Register = List of Action Items
                                    <ul>
                                        <li>Aging Items give rise to Action Items for scheduling required actions and for keeping a historical record of completed actions.</li>
                                        <li>Lists all tracked items with their history of actions with completion status, and occurrences in the future.</li>
                                        <li>Has the button "Update Aging Items": For bringing up to date the status of each action item associated with each aging item.
                                            An update for just one individual aging item can also be performed.
                                        </li>
                                    </ul>
                                </li>
                            </ul>
                        </p>
                        <p>Notes:
                            <ul>
                                <li>Equipment operating hours, cycles, and distance travelled (mileage) are tracked by equipment component. One component for a piece of equipment must always be
                                    that entire piece in addition to subcomponents, for example 'Entire towplane PA-18' (parent component) and 'Engine for PA-18' (child component).
                                    Certain operating data for a parent component automatically flow to the child component (do not enter duplicate data).
                                </li>
                                <li>You may find it convenient to display this introduction (including the help pages below) in another instance of this website in a separate browser
                                    window as you work with equipment aging.</li>
                                <%-- SCR 214 start --%>
                                <li>A summary of equipment actions is available to the "Member" role from the main menu under 
                                    <a href="../../MemberPages/EquipMaintStat/EqMaintStatus.aspx"> Member Pages / Status of Equipment Maintenance Actions</a>.</li>
                                <%-- SCR 214 end --%>
                            </ul>
                        </p>
                        <p>Limitation:
                            <ul>
                                <li>Only one person should make changes to equipment data at any one time. Different persons may edit equipment data at non-overlapping times.
                                    The website administrator can enforce this by giving only one person the role of 'Equipment'.
                                </li>
                            </ul>
                        </p>
                    </div><%-- SCR 213 --%>
                </Content>
            </ajaxToolkit:AccordionPane>

            <ajaxToolkit:AccordionPane ID="AccPHelp" runat="server" >
                <Header> Detailed Help Pages/Tabs </Header>
                <Content>
                    <ajaxToolkit:TabContainer ID="TabContainer1" runat="server" ScrollBars="Both" BackColor="#ffffcc" OnActiveTabChanged="TabContainer1_ActiveTabChanged" AutoPostBack="true" >
                        <ajaxToolkit:TabPanel runat="server" HeaderText="Page 1: Data Flow Chart" ScrollBars="Both" BackColor="#ffffcc">
                            <ContentTemplate>
                                <asp:Image ID="Image1" runat="server" ImageUrl="~/i/EquipAging_Concepts.jpg" />
                            </ContentTemplate>
                        </ajaxToolkit:TabPanel>
                        <ajaxToolkit:TabPanel runat="server" HeaderText="Page 2: Flowchart Boxes; Dates/Times; Scheduling Methods" ScrollBars="Both" BackColor="#ffffcc">
                            <ContentTemplate>
                                <div class="HelpText" ><%-- SCR 213 --%>
                                    <div class="leftAlign">
                                        <h5><b>Boxes in the Flowchart</b> on the First Help Page</h5>
                                        <ul>
                                            <li>The <a href="../EquipmentList.aspx">List of Equipment</a> defines each piece of equipment that you wish to work with at a high level,
                                                such as an aircraft, a trailer, or a mower.
                                                The most important property is its name (only up to 25 characters long).</li>
                                            <li>The <a href="../EqComponents.aspx">List of Equipment Components</a> breaks down pieces of Equipment into those parts
                                                that you need to track in terms of maintenance/documentation schedules. Only those components need exist in the list of equipment components for which
                                                equipment aging processing is desired.
                                                Note that one of those parts (and only one) should represent the entire piece of equipment. For example, a towplane could be called "N333TM";
                                                its components could be "entire Supercub" and "Supercub engine". Another example would be that a glider could be called "N766PW",
                                                and its components could be "entire PW-6U" and "PW-6U rudder cables" where the latter is a child of the former.</li>
                                            <li>The nature of what kind of actions can or need to be taken on a regular basis can be chosen from the <b>List of Types of Action</b>
                                                which can be edited only by the website administrator. Examples: "Annual Inspection", "Replace", "Renew", "Documentation", ...</li>
                                            <li>Other flowchart boxes are discussed on other help pages.</li>
                                        </ul>
                                        <h5><b>Dates and Times</b></h5>
                                        <p>Most dates and points in time are expressed as `DateTimeOffset` objects: such an object holds the calendar date,
                                        the time of day, and the time offset to Universal Time Coordinated (UTC, or Greenwich Mean Time GMT). Most of the time, the user needs to be
                                        concerned only with the date portion. Occasionally, you need to work with time of day; in that case go to the
                                        bottom of this web page where the display and editing of times and offsets can be turned on and off: click on "Is Time Of Day Important?".
                                        This website remembers those settings separately for each user.</p>
                                        <p>DateTimeOffset objects are representing like this: YYYY/MM/DD HH:mm:ss +hh:mm where:
                                            <ul>
                                                <li>YYYY = 4-digit year</li>
                                                <li>MM = 2-digit month</li>
                                                <li>DD = 2-digit day of month</li>
                                                <li>HH = 2-digit 24-hour hour of day</li>
                                                <li>mm = 2-digit minute of hour</li>
                                                <li>ss = 2-digit second of minute</li>
                                                <li> +  must be either + or -</li>
                                                <li>hh = 2-digit hours of offset from GMT; cannot be greater than 14; cannot be less than -14.</li>
                                            </ul>
                                            For example: 2001/09/11 09:02:00 -04:00 - September 11, 2001, 9:02 am, Northamerican Eastern Daylight Savings Time.
                                            The GMT offset for Central Europe Standard Time is +01:00. The GMT offset for Northamerican Pacific Standard time is -08:00.
                                            The GMT offset for Northamerican Pacific Daylight Savings time is -07:00. The offset for GMT is, of course, +00:00 (do not use a leading minus sign here).
                                        </p>
                                        <p>There are five ways that maintenance/aging items can be <b>scheduled</b>:</p>
                                        <ul>
                                            <li>By elapsed time: for example, an annual inspection needs to happen every 12 months.</li>
                                            <li>By operating or running time: for example, an airplane engine needs to be overhauled every 2000 hours of running time.</li>
                                            <li>By number of cycles: for example, a landing gear inspection needs to occur every 500 takeoff/landing cycles.</li>
                                            <li>By distance traveled: for example, trailer wheel bearings need to repacked every 12,000 miles.</li>
                                            <li>By unique event: for example, the sale of a major piece of equipment, i.e., an activity for which no periodicity can be given.</li>
                                        </ul>
                                        <p>Note that more than one scheduling method can be combined, for example, trailer wheel bearings need to be repacked every
                                            12,000 miles or 3 years, whichever occurs earlier. The unique event method cannot be combined with any others.</p>
                                    </div>
                                </div><%-- SCR 213 --%>
                            </ContentTemplate>
                        </ajaxToolkit:TabPanel>
                        <ajaxToolkit:TabPanel runat="server" HeaderText="Page 3: Components of Equipment" ScrollBars="Both" BackColor="#ffffcc" >
                            <ContentTemplate>
                                <div class="HelpText" ><%-- SCR 213 --%>
                                    <div class="leftAlign">
                                        <h5><b>Equipment Components Hierarchy</b></h5>
                                        Reference: <a href="../EqComponents.aspx">web page for maintaining components</a>
                                        <p>For purposes of tracking equipment maintenance items, each piece of major equipment can be broken down into as many components
                                            as needed to perform the tracking. A quick introduction appears on the previous help page, including the fact that a major piece
                                            of equipment also needs to appear as an 'Entire' component in the list of components. That entire component can then have
                                            any number of child components; each of those can have any number of child components of their own, and so on to any desired depth
                                            of descendants.
                                        </p>
                                        <h5><b>Equipment Component Properties</b></h5>
                                        <ul>
                                            <li>Each component and subcomponent has a unique name, i.e., different from the name of any other component.</li>
                                            <li>Each component and subcomponent maintains a reference to the major piece of equipment to which it belongs.</li>
                                            <li>The 'Entire' property is either true or false (table column contains a checkmark, or does not).</li>
                                            <li>Parent component: identifies the component to which the component in this table row belongs</li>
                                            <li>Parent component for an 'Entire' component: must be set to '[none]'.</li>
                                            <li>Link Begin and Link End dates and times: When does the association of this component with its parent begin and end?
                                                There exist several special conditions:
                                                <ul>
                                                    <li>The component is an 'Entire' one:
                                                        <ul>
                                                            <li>the Link Begin and Link End dates need not be the same as ownership begin
                                                                and ownership end dates specified for the corresponding <a href="../EquipmentList.aspx"> major piece of equipment</a>.
                                                                The latter dates are intended for legal ownership, and the former for physical presence.
                                                                They can, of course, be the same.
                                                            </li>
                                                            <li>The Link Begin date must be equal to greater than 1900/01/01 01:01:00 +00:00</li>
                                                            <li>The Link End date must be equal to less than 2999/12/31 22:59:00 +00:00. Choose a date far in the future
                                                                when the end date is not known.
                                                            </li>
                                                        </ul>
                                                    <li>The component is a subcomponent ('Entire' property is false or unchecked):
                                                        <ul>
                                                            <li>You signal that the subcomponent's Link Begin date is the same as the Link Begin date of its parent
                                                            by entering a Link Begin date that is earlier than 1900/01/01 01:01:00 +00:00.
                                                            </li>
                                                            <li>You signal that the subcomponent's Link End date is the same as the Link End date of its parent
                                                                by entering a Link End date that is later than 2999/12/31 22:59:00 +00:00.
                                                            </li>
                                                            <li>When a subcomponent's Link Begin or End dates are not the same as its parent's take care to enter
                                                                Begin dates later than, and End dates earlier than the parent's.
                                                            </li>
                                                        </ul>
                                                    </li>
                                                </ul>
                                            </li>
                                        </ul>
                                    </div>
                                </div><%-- SCR 213 --%>
                            </ContentTemplate>
                        </ajaxToolkit:TabPanel>
                        <ajaxToolkit:TabPanel runat="server" HeaderText="Page 4: Operational Calendars" ScrollBars="Both" BackColor="#ffffcc">
                            <ContentTemplate>
                                <div class="HelpText" ><%-- SCR 213 --%>
                                    <div class="leftAlign">
                                        <p><b>Operational Calendars</b></p>
                                        <p>It is often the case that flight operations are interrupted for one reason or another, for example, a glider club may not be operating during the winter months. Or, 
                                        one of the pieces of equipment needs extensive down time due to a maintenance requirement. For purposes of least squares extrapolation of accumulated flight hours
                                        into the future, an operational calendar should be used in order to obtain a reliable extrapolation result. That calendar specifies when flight operations are
                                        possible, and when they are not allowed or when they did not occur.</p>
                                        <p>One of the operational calendars is designated as 'Standard'. It will be offered as the default calendar when creating a new aging item.</p>
                                        <p>A secondary use of operational calendars concerns the time span that is displayed in action item illustrations (see a subsequent help page):
                                            from about one month before the first date to about one month after the last date in the operational calendar.
                                        </p>
                                        <%-- SCR 214 start --%>
                                        <p>Generally, it is best to start an operational calendar with an non-operational interval; its start date is early enough to cover any actions that may have been taken
                                            regarding an aging item of interest. However, it should not be too early to avoid meaningless illustrations
                                            because they start with the start date of the first interval in the operational calendar.
                                        </p>
                                        <%-- SCR 214 end --%>
                                    </div>
                                </div><%-- SCR 213 --%>
                            </ContentTemplate>
                        </ajaxToolkit:TabPanel>
                        <ajaxToolkit:TabPanel runat="server" HeaderText="Page 5: Parameter Sets" ScrollBars="Both" BackColor="#ffffcc" >
                            <ContentTemplate>
                                <div class="HelpText" ><%-- SCR 213 --%>
                                    <div class="leftAlign">
                                    <p>Referring to the data flow chart on help page 1, and the <a href="EqAgingParSets.aspx"> web page for equipment aging "Parameter Sets"</a>:
                                        <ul>
                                            <li>Instead of repeatedly specifying aging parameters for each aging item, we pre-define <b>sets of aging parameters</b> so that one set can be used over
                                                and over for different equipment components. Each set has a short description and combines these items:
                                                <ul>
                                                    <li>"<b>sEquipActionType</b>": Type of action</li>
                                                    <li>"<b>iIntervalElapsed</b>": If we schedule by elapsed time, the calendar time interval, an integral number. Use -1 if this scheduling method is not used.</li>
                                                    <li>"<b>sTimeUnitsElapsed</b>": The units of elapsed time; choose from: Hours, Days, Weeks, Months, Quarters, Years. Default: Months</li>
                                                    <li>"<b>cDeadLineMode</b>": The deadline mode when scheduling by elapsed time (deadline support variables iDeadLineSpt1 and iDeadLineSpt2 have different
                                                        roles depending on the deadline mode - see also next help tab):
                                                        <ul>
                                                            <li><b>Y</b> = Action Item occurs every iIntervalElapsed years on month/day, where month is specified by iDeadLineSpt1, and day by iDeadLineSpt2.
                                                                If iDeadLineSpt2 is greater than the number of days in the month, then the action occurs on the last day of the month.
                                                                sTimeUnitsElapsed must be 'Years'.
                                                            </li>
                                                            <li><b>M</b> = Action occurs every iIntervalElapsed months on the Day specified by iDeadLineSpt2.
                                                                If iDeadLineSpt2 is greater than the number of days in the month, then the action occurs on the last day of the month.
                                                                sTimeUnitsElapsed must be 'Months'. iDeadLineSpt1 is not used and should be set to -1.
                                                            </li>
                                                            <li><b>W</b> = Action occurs every iIntervalElapsed weeks on the weekday specified by iDeadLineSpt2:  0 = Sunday, 1 = Monday, … 6 = Saturday. 
                                                                sTimeUnitsElapsed must be 'Weeks'. iDeadLineSpt1 is not used and should be set to -1.
                                                            </li>
                                                            <li><b>N</b> = Action occurs every iIntervalElapsed months on the Nth (=iDeadLineSpt1) occurrence of the weekday specified by iDeadLineSpt2.
                                                                Special case: when iIntervalElapsed is zero and iDeadLineSpt1=5, action occurs on the next 5-th occurrence of this weekday, whichever month this may be.</li>
                                                            <li><b>L</b> = Action is scheduled from the last completion date of the action associated with this aging item, counting iIntervalElapsed sTimeUnitsElapsed.
                                                                iDeadLineSpt1 and iDeadLineSpt2 are not used and should be set to -1.
                                                            </li>
                                                            <li><b>C</b> = Like L, but for sTimeUnitsElapsed=(Years, Quarters, or Months) the deadline is at the end of the time interval into which the calculated deadline falls.
                                                                No deadline adjustment when sTimeUnitsElapsed=(Weeks, Days, or Hours). iDeadLineSpt1 and iDeadLineSpt2 are not used and should be set to -1.</li>
                                                        </ul>
                                                    </li>
                                                </ul>
                                            </li>
                                        </ul>
                                    </p>
                                    (continued on next page)
                                    </div>
                                </div><%-- SCR 213 --%>
                            </ContentTemplate>
                        </ajaxToolkit:TabPanel>
                        <ajaxToolkit:TabPanel runat="server" HeaderText="Page 6: Parameter Sets" ScrollBars="Both" BackColor="#ffffcc" >
                            <ContentTemplate>
                                <div class="HelpText" ><%-- SCR 213 --%>
                                    <div class="leftAlign">
                                    <p>
                                        <ul>
                                            <li>(continued from previous page)
                                                <ul>
                                                    <li>"<b>iDeadLineSpt1</b>": support variable 1 for elapsed time deadline calculations (for how to use see previous page and table below)</li>
                                                    <li>"<b>iDeadLineSpt2</b>": support variable 2 for elapsed time deadline calculations (for how to use see previous page and table below)</li>
                                                </ul>
                                                <h4 class="text-center">How to use Deadline Support Variables under Various Elapsed Time Deadline Modes</h4>
                                                <div class="text-center">- -1 - means 'not used'. Set the value to -1.</div>
                                                <table class="SoarNPGridStyle" style="margin-left:auto;margin-right:auto;text-align:center">
                                                    <tr>
                                                        <td><b>Deadline Mode Code</b></td><td><b>iDeadLineSpt1</b></td><td><b>iDeadLineSpt2</b></td>
                                                    </tr>
                                                    <tr>
                                                        <td>Y</td><td>Month (1 - 12)</td><td>Day of Month (1 - 31)</td>
                                                    </tr>
                                                    <tr>
                                                        <td>M</td><td> - -1 - </td><td>Day of Month (1 - 31)</td>
                                                    </tr>
                                                    <tr>
                                                        <td>W</td><td> - -1 - </td><td>Day of Week (0 - 6)</td>
                                                    </tr>
                                                    <tr>
                                                        <td>N</td><td>N-th Occurrence (1 through 5) in a Month</td><td>Day of Week (0 - 6)</td>
                                                    </tr>
                                                    <tr>
                                                        <td>L</td><td> - -1 - </td><td> - -1 -</td>
                                                    </tr>
                                                    <tr>
                                                        <td>C</td><td> - -1 - </td><td> - -1 -</td>
                                                    </tr>
                                                </table>
                                                <ul>
                                                    <li>"<b>iIntervalOperating</b>": If we schedule by operating time, the operating time interval after which the action is due counting from the last time the action was taken; an integral number.
                                                        Set to -1 if this scheduling method is not used.</li>
                                                    <li>"<b>sTimeUnitsOperating</b>": The units of measure for operating time: Hours, Days, Weeks, Months, Quarters, Years. Default: Hours.</li>
                                                    <li>"<b>iIntervalCycles</b>": If we schedule by number of cycles, the number of cycles after which the action is due counting from the last time the action was taken;
                                                        an integral number. -1 means not scheduling by number of cycles.
                                                    </li>
                                                    <li>"<b>iIntervalDistance</b>": The number of distance intervals to use when scheduling by distance travelled. -1 means not scheduling by distance.</li>
                                                    <li>"<b>sDistanceUnits</b>": The units of distance to use when scheduling by distance; default is ‘Miles’. The only other choice is ‘Km’ (kilometers).</li>
                                                    <li>You may add any <b>comment</b> to the parameter set, practically of any length.</li>
                                                    <li>Any or all of the first four scheduling methods may be used at the same time.
                                                        <b>Special</b> case: All four intervals are equal to -1.
                                                        This signals that no scheduling method is being used. This kind of a parameter set can be used
                                                        for one-time occurrences of single or <b>unique</b> events (the fifth scheduling method) associated with an equipment component.
                                                    </li>
                                                </ul>
                                            </li>
                                        </ul>
                                    </p>
                                    </div>
                                </div><%-- SCR 213 --%>
                            </ContentTemplate>
                        </ajaxToolkit:TabPanel>
                        <ajaxToolkit:TabPanel runat="server" HeaderText="Page 7: Aging Items" ScrollBars="Both" BackColor="#ffffcc">
                            <ContentTemplate>
                                <div class="HelpText" ><%-- SCR 213 --%>
                                    <div class="leftAlign">
                                    <p>Referring to the data flow chart on the help page 1:
                                        <ul>
                                            <li>The combination of an equipment component, a parameter set, and an ops calendar, is an <b>Aging Item</b>:
                                                This is how you specify for a particular component how we track action items in order to fulfill
                                                maintenance or documentation items that are to occur at regular intervals.</li>
                                            <li>The following are input data that keep changing as time goes by:</li>
                                            <ul>
                                                <li><b>Statistics of Flying Activities</b>: They are collected on an ongoing basis by the Club statistician via this website.
                                                    For each aircraft, we collect for each flight the time of takeoff and the time of landing, so that the time in flight
                                                    can be calculated. This allows us to collect the number of cycles (takeoff/landing combination) as well.
                                                </li>
                                                <li><b>Manually collected operating hours</b>: for equipment and components for which operating hours are not collected through
                                                    flight statistics, such as a mower. This method may need to be used for aircraft for which detailed takeoff and landing times
                                                    are not available.
                                                </li> 
                                            </ul>
                                        </ul>
                                    </p>
                                    <p>Referring to the table on the web page <a href="EqAgingItems.aspx">"Equipment Component Aging Items"</a>, description of columns:
                                        <ul>
                                            <li>Internal Id: Identifier of a row in database table EQUIPAGINGITEMS</li>
                                            <li><b>Component Name</b>: the name of the component with which this aging item is associated</li>
                                            <li><b>Aging Item Name</b>: user-supplied name of this aging item</li>
                                            <li><b>Parameter Set</b>: the name of the aging parameter set with which this aging item is associated</li>
                                            <li><b>Operational Calendar</b>: the name of the operational calendar that this aging item uses</li>
                                            <li><b>Start Date</b>: user-supplied point in time when scheduling of this aging item begins. This could be when the component was put into service.
                                                If this date is earlier than the "Link Begin" date specified for the component, then this Link Begin date is used as the start date for scheduling purposes.
                                                It can provide an anchor point for the scheduling algorithm.</li>
                                            <li><b>End Date</b>: user-supplied point in time beyond which no scheduling takes place; this date is usually not known and can be set far into the future.
                                                If this date is later than the "Link End" date specified for the component, then the Link End date is used as the end date for scheduling purposes.
                                            </li>
                                            <li><b>Estimated Operations Elapsed Time</b>: user-supplied estimate of the elapsed time it will take until 'iIntervalOperating' (see parameter set) will have run out;
                                                required when scheduling by time in operation/running time; units of Days. This is ignored when variable bRunExtrap (column <b>"Extr?"</b>
                                                to the right of column "Estim. Op. El. Time") is set to true (checkbox is checked) indicating that flight operations data are to be used for
                                                estimating this value by least squares fitting and extrapolation.
                                            </li>
                                            <li><b>Estimated Cycles Elapsed Time</b>: user-supplied estimate of the elapsed time it will take until 'iIntervalCycles' (see parameter set) will have run out;
                                                required when scheduling by number of cycles. This is ignored when variable bCyclExtrap (column <b>"Extr?"</b> to the right of column "Estim. Cycles El. Time")
                                                is set to true (checkbox is checked) indicating that flight operations data are to be used for estimating this value
                                                by least squares fitting and extrapolation.</li>
                                            <li><b>Estimated Distance Traveled Elapsed Time</b>: user-supplied estimate of the elapsed time it will take until the 'iIntervalDistance' (see parameter set) will have run out;
                                                required when scheduling by distance traveled; units of Days. In a future version of this software, variable bDistExtrap (column <b>"Extr?"</b> to the right of
                                                column "Estim. Dist. El. Time") can be set to true indicating that vehicle operational data (in tables OPERATIONS and OPDETAILS) are to be used for estimating this value
                                                by least squares fitting and extrapolation. Extrapolation for distance traveled is not functional at this time (value of bDistExtrap is ignored).</li>
                                            <li><b>Comments</b>: Any helpful notes</li>
                                        </ul>
                                    </p>
                                    </div>
                                </div><%-- SCR 213 --%>
                            </ContentTemplate>
                        </ajaxToolkit:TabPanel>
                        <ajaxToolkit:TabPanel runat="server" HeaderText="Page 8: Operating Data" ScrollBars="Both" BackColor="#ffffcc">
                            <ContentTemplate>
                                <div class="HelpText" ><%-- SCR 213 --%>
                                    <div class="leftAlign">
                                    <p>Referring to the diagram on the first help tab, and the table in the web page <a href="EqAgingOperDat.aspx">"Equipment Component Operating Data"</a>:
                                        <ul>
                                            <li><b>Combination of Operating Data (hours of operation and/or distance/mileage traveled) by Component and Time Interval</b>:
                                                Distinguish between Flight Operations data and Manually Entered data:
                                                <ul>
                                                    <li>Flight Operations data: entered by the Statistician in <a href="../../Statistician/FlightLogInput.aspx">grid view</a>
                                                        or <a href="../../Statistician/OpsDataInput.aspx">tree view</a>. While this is also a manual process, we hope to automate this
                                                        a little in the future. In the database, tables OPERATIONS and OPDETAILS hold these data. It is associated with pieces of equipment
                                                        in the <a href="../EquipmentList.aspx">equipment list</a>, i.e., expressly <u>not</u> with components of equipment. As implied by the name,
                                                        Flight Operations data cannot be used for "Distance Traveled", only for flight times and cycles (combination of one takeoff and one landing).
                                                    </li>
                                                    <li>Manually Entered data: entered by the Equipment role in <a href="EqAgingOperDat.aspx">Operational data</a>. In the database, table EQUIPOPERDATA
                                                        holds these data which <u>are</u> associated with <a href="../EqComponents.aspx">components of equipment</a>.
                                                    </li>
                                                </ul>
                                                All operational data are cumulative, meaning that flight operations data and manually entered data are added together should both of them
                                                exist for a component. Furthermore, manually entered data for a component cascade down to its child components for a particular time 
                                                period of interest. Expressed in a different way: a subcomponent's hours of operations (or cycles, or distance traveled) are taken from its
                                                parent component (and from there from its grandparents etc.) and are added to the hours that may have been entered for the subcomponent itself.
                                                When the component is an 'Entire' component (representing in the list of components an entire piece of equipment in the list of equipment)
                                                the flight operations data for that piece of equipment cascade down to that 'entire' component and from there to its children and grandchildren, etc.
                                                <br />
                                                All operating times are expressed in hours and decimal fractions of an hour. Distances can be expressed in miles or kilometers.
                                                Each row in this table holds the operating data for a time interval ("from" - "to").
                                                Certain conditions have to be met regarding interval non-overlap:
                                                <ul>
                                                    <li>Time intervals must not overlap for the same component: Status column 'St' displays a red <b><span style="color:red">X</span></b>.</li>
                                                    <li>Beginning of a time interval is more than three hours after the end of the preceding interval: column 'St' displays a dark magenta
                                                        <b><span style="color:darkmagenta">G</span></b> (for 'Gap').</li>
                                                    <li>Otherwise, a green <b><span style="color:green">O</span></b> is displayed in column 'St' (for 'Ok').</li>
                                                </ul>
                                                The 'Source' of operating data is characterized by one of two codes:
                                                <ul>
                                                    <li>M - The record's data was entered manually by the user</li>
                                                    <li>R - This is a Reset or Baseline record (also entered manually, see next help tab)</li>
                                                </ul>
                                                The default source code is M.
                                            </li>
                                        </ul>
                                    </p>
                                    </div>
                                </div><%-- SCR 213 --%>
                            </ContentTemplate>
                        </ajaxToolkit:TabPanel>
                        <ajaxToolkit:TabPanel runat="server" HeaderText="Page 9: Operating Data - Reset Records" ScrollBars="Both" BackColor="#ffffcc">
                            <ContentTemplate>
                                <div class="HelpText" ><%-- SCR 213 --%>
                                    <div class="leftAlign">
                                    <p>Continued from previous page:
                                        <ul>
                                            <li><b>Baseline Records:</b> (also referred to as 'Reset Records')
                                                <ul>
                                                    <li>When the From-date and time is set exactly the same as the To-date and time,
                                                        then that record represents a 'baseline'; also, the code for 'Source' needs to be 'R'.
                                                        The operating hours, number of cycles, and distance in this record are the cumulative operating hours, number of cycles, and distance for this component from its start of operation.
                                                        For example, a new piece of equipment starts with 0 hours. Or, a used piece of equipment placed in operation with this organization has 15500 miles already accumulated.
                                                        At least one baseline record should exist for each component; if none exists, the software assumes that operating hours, number of cycles, and distances start at zero.</li>
                                                    <li>A baseline record holds baseline data for operating hours, number of cycles, and operating distance (they cannot be baselined separately).</li>
                                                    <li>When flying activities records are used for accumulating operating data, only those flights that occurred
                                                        after the last baseline record are taken into account.</li>
                                                    <li>Records of Operating Data with To-dates before or equal to the last baseline record for a component are ignored in the scheduling algorithm for action items.</li>
                                                    <li>It is an error when the point in time of a baseline record falls between the From-date and time, and To-date and time, of another record for the same component.</li>
                                                    <li>In order to ensure that the From- and To-times are indeed exactly the same, you need to be able to see
                                                        the time of day: if necessary, go to the bottom of this page and check the checkbox labeled
                                                        <i>`In Aging Management, working with operating data records, include Start and End Times of Day instead of showing just the date`</i>.
                                                        Make sure that not only From-time and To-time are the same to the minute, but also From-Offset and To-Offset.</li>
                                                    <li>Recommendation: set the time of day of a baseline record late in the day, for example 22:59:00. For the offset to universal time coordinated use
                                                        your local offset; for example, for Pacific Daylight Time use -07:00; for Pacific Standard Time it is -08:00.</li>
                                                </ul>
                                            </li>
                                            <li>Further Elaboration on <b>Reset Records</b>:
                                                <ul>
                                                    <li>The data in Reset records do not cascade down the component hierarchy (other data with the 'M' source code do cascade down).</li>
                                                    <li>Therefore, each component and subcomponent should have its own Reset record(s).</li>
                                                    <li>If there is no Reset record it's as if there was one with all zeroes for the operating data at the Link Begin date of the component.</li>
                                                </ul>
                                            </li>
                                        </ul>
                                    </p>
                                    </div>
                                </div><%-- SCR 213 --%>
                            </ContentTemplate>
                        </ajaxToolkit:TabPanel>
                        <ajaxToolkit:TabPanel runat="server" HeaderText="Page 10: Action Items - Basics" ScrollBars="Both" BackColor="#ffffcc">
                            <ContentTemplate>
                                <div class="HelpText" ><%-- SCR 213 --%>
                                    <div class="leftAlign">
                                    <p>Referring to the data flow chart on help page 1, and the table in the web page <a href="EqAgingActionItems.aspx">"Equipment Aging Register (List of Action Items)"</a>:<br />
                                        <b>List of Action Items</b>: also referred to as the 'Equipment Aging Register'. These action items are derived from the Aging Items
                                            by applying a computational algorithm which takes as input:</p>
                                            <ol>
                                                <li>The list of Aging Items</li>
                                                <li>Each component's operating data (operating hours, cycles, and/or miles traveled)</li>
                                                <li>An operational calendar</li>
                                                <li>The current status of each action item</li>
                                            </ol>
                                        <p>As mentioned on an earlier help page, there are five ways of scheduling:
                                            by calendar elapsed time, by hours of operation, by number of cycles, by ground distance traveled, and by the date of a unique event.
                                            Any or all of these ways may be used for an aging item. For a unique event, the deadline is simply the date of the event;
                                            it cannot be combined with any of the first four options.
                                            When we ignore the unique event option, the algorithm's output consists of one deadline and up to four limits:</p>
                                            <ul>
                                                <li>A deadline date: the earliest of the up to four deadlines that are calculated for each of the four ways of scheduling</li>
                                                <li>If scheduling by elapsed time: the limit is the point in time when the next action is due on the calendar</li>
                                                <li>If scheduling by operating hours, the limit of hours of operation: when the next action is due in terms of hours of operation</li>
                                                <li>If scheduling by number of cycles, the limit number of cycles: when the next action is due in terms of number of cycles</li>
                                                <li>If scheduling by distance traveled, the limit of miles or kilometers: when the next action is due in terms of distance traveled</li>
                                            </ul>
                                        <p>One Aging Item can have any number of Action Items associated with it: historical ones that have been completed, zero or one current one that is in progress,
                                            and one future one that has not been started.
                                        The result is a list of action items grouped together by aging item, and sorted by deadline date:</p>
                                        <ol>
                                            <li><span style="color:blue">Completed (blue):</span> 'percent complete' is 100%, and the start date and date of completion have been given.</li>
                                            <li>Current (black): 'percent complete' is greater than 0% and less than 100%; the start date exists. If a date of completion exists, it is an estimated date.</li>
                                            <li><span style="color: orangered">Upcoming (orange):</span> 'percent complete' = 0%; Deadline is in the past of less than a week in the future. Shows when the action item is due; if start and completion dates exist, they are estimated dates.</li>
                                            <li><span style="color: brown">Future (brown):</span> 'percent complete' = 0%; Deadline is more than a week in the future.</li>
                                        </ol>
                                        <p>Two special points in time are used:</p>
                                        <ol>
                                            <li>2001/01/01 01:01:00 +00:00 : indicates that the action has not been started</li>
                                            <li>2999/12/31 22:59:00 +00:00 : indicates that the action has not been completed</li>
                                        </ol>
                                        <p>... Continued on next page ...</p>
                                    </div>
                                </div><%-- SCR 213 --%>
                            </ContentTemplate>
                        </ajaxToolkit:TabPanel>
                        <ajaxToolkit:TabPanel runat="server" HeaderText="Page 11: Action Items - Details" ScrollBars="Both" BackColor="#ffffcc">
                            <ContentTemplate>
                                <div class="HelpText" ><%-- SCR 213 --%>
                                    <div class="leftAlign">
                                        <p>... Continued from previous page ...</p>
                                        Many of the columns in the list of <b>Action Items</b> have explanations in their 'Tool Tips' which appear when hovering
                                        the mouse cursor over the column header.<br />
                                        There is no automatic updating of action items; you can click on the "Update Aging Items" button as often as you wish.
                                        When you do, all actions associated with all aging items are updated by re-examining and recalculating all deadlines.
                                        <br />
                                        The user cannot make changes to the data in columns that are calculated by the computational algorithm, i.e.,
                                        all columns starting from the left up to and including the last deadline.
                                        Available for editing are these columns:
                                        <ul>
                                            <li><b>Actual Start</b>: Point in time when work on this action item started</li>
                                            <li><b>Percent Complete</b>: an integral number between 0 and 100. The percentage estimate should be in terms of elapsed time.</li>
                                            <li><b>Completed Date</b>: a known date when Percent Complete is 100; an estimated date when Percent Complete is between 1 and 99</li>
                                            <li><b>Hrs @ Completion</b>: Number of operating hours at completion of the action item if scheduling includes by operating hours or
                                                running time; -9.99 otherwise. </li>
                                            <li><b>Cycles @ Completion</b>: Number of cycles at completion of the action item if scheduling includes by cycles; -999 otherwise. </li>
                                            <li><b>Distance @ Completion</b>: Distance traveled at completion of the action item if scheduling includes by distance; -9.99 otherwise. </li>
                                            <li><b>Comments</b>: could be used to make notes about who is doing the work and where</li>
                                        </ul>
                                        Regarding the "xxx @ Completion" items:
                                        <%-- SCR 218 start --%>
                                        <ul>
                                            <li>They are similar to the "Reset Records" discussed earlier in that we start accumulating operational data
                                                from that point in time on for purposes of calculating the next action item deadline(s) for this aging item.</li>
                                            <li>All three kinds of operational data (hours, cycles, distance) must be entered together for each action item completion
                                                unless one or two of them are of no interest; those should always have a zero value.
                                            </li>
                                        </ul>
                                        <%-- SCR 218 end --%>
                                        <b>Options Buttons and Options Menu</b><br />
                                        The right-most column of the Action Item page holds buttons entitled 'Options'. Clicking on one of them opens an options menu
                                        for the action item in that grid row:
                                        <ul>
                                            <li><b>Charts</b>: Shows illustrations that help understand how the deadline of this action item was calculated (see next help page).</li>
                                            <li><b>Details</b>: Shows two tables with details about input to and output from the action item deadline algorithm.</li>
                                            <li><b>Edit</b>: Allows entering user-modifiable action item data.</li>
                                            <li><b>Update</b>: Causes deadlines to be recalculated for all action items that belong to the aging item associated with the action item
                                                identified at the top of the list of options.</li>
                                            <li><b>Delete</b>: Rarely used. Allows deletion of the action item identified at the top of the list of options.</li>
                                            <li><b>Cancel</b>: Take no action/choose no option.</li>
                                        </ul>
                                        <p>... Continued on next page ...</p>
                                    </div>
                                </div><%-- SCR 213 --%>
                            </ContentTemplate>
                        </ajaxToolkit:TabPanel>
                        <ajaxToolkit:TabPanel runat="server" HeaderText="Page 12: Action Item Illustrations" ScrollBars="Both" BackColor="#ffffcc">
                            <ContentTemplate>
                                <div class="HelpText" ><%-- SCR 213 --%>
                                    <div class="leftAlign">
                                        <p>... Continued from previous page ...</p>
                                        <p><b>Action Item Illustrations and Charts</b></p>
                                        During an update of an action item one or more pictures are generated: 
                                        <ol>
                                            <li><b>Action Item Illustration</b>: shows the influences upon an action item's deadline in terms of a data flow logic diagram, and a time line
                                                of the various sources of data. This picture is always generated.
                                            </li>
                                            <li><b>Operating Hours</b>: a chart showing flying activity and accumulated flight hours as a function of elapsed time. Explains how the
                                                action item's deadline was obtained through least squares extrapolation. Only generated when operating hours from records of
                                                flying activities are used. See also <b>**</b> paragraph below.<%-- SCR 214 --%>
                                            </li>
                                            <li><b>Cycles</b>: a chart showing flying activity and accumulated flight cycles as a function of elapsed time where a 'cycle' is the
                                                combination of one takeoff and one landing. Explains how the action item's deadline was obtained through least squares
                                                extrapolation. Only generated when cycle data from records of flying activities are used. See also <b>**</b> paragraph below.<%-- SCR 214 --%>
                                            </li>
                                            <li><b>Distance Traveled</b>: (this feature is not yet available) a chart showing distance traveled and accumulated distance traveled
                                                as a function of elapsed time. Explains how the action item's deadline was obtained through least squares extrapolation.
                                                Only generated when distance traveled data are used. 
                                            </li>
                                        </ol>
                                        <%-- SCR 214 start --%>
                                        <p>
                                            <b>**</b> You may use the extrapolation illustrations to judge the <U>goodness of fit</U> of the least squares extrapolation
                                            by comparing the accumulated flight data to the fitted broken line. It may happen that recent flight activity does not follow the
                                            predicted (fitted) line well enough to result in a useful deadline, particularly when that deadline is in the near future.
                                            The remedy for this situation is to define a different operational calendar where one or more operational intervals span only
                                            recent flying activities so that older flight data does not influence the fitted line. Use that new operational calendar
                                            in the aging item and repeat the extrapolation calculation for the action item associated with the aging item.
                                        </p>
                                        <%-- SCR 214 end --%>
                                        <p><b>Two tables with Action Item Details</b></p>
                                        This is another tool for clarifying the inputs used for calculating an action item's deadline, as well as the resulting output data
                                        and user input stored with the action item. The computational algorithm uses an important piece of data called <b>DLastAction</b>, the date of last action:
                                        the point in time where the calculation of the next action item deadline starts. DLastAction is the latest of the following:
                                        <ul>
                                            <li>The latest date of completion among the action items associated with an aging item; there may not be one in existence yet.</li>
                                            <li>The aging item's start date</li>
                                            <li>The equipment component's Link Begin date</li>
                                        </ul>
                                    </div>
                                </div><%-- SCR 213 --%>
                            </ContentTemplate>
                        </ajaxToolkit:TabPanel>
                        <ajaxToolkit:TabPanel runat="server" HeaderText="Page 13: Computational Algorithm" ScrollBars="Both" BackColor="#ffffcc">
                            <ContentTemplate>
                                <div class="HelpText" ><%-- SCR 213 --%>
                                    <div class="leftAlign">
                                        <b>Computational Algorithm</b>: figures out when the next action items are due.
                                        <p>This description is <b>not</b> required reading for operating the Equipment Aging portion of this website.</p>
                                        <p>A detailed description appears in the document "The TSoar Project". The software source code contains a lot of comments explaining what goes on.
                                            We just give an overview here.
                                        </p>
                                        <p>Running the algorithm is initiated by the user in the Equipment Aging Register (List of Action Items) either by clicking on
                                            'Update Aging Items' or clicking on 'Options' in any one of the Action Item rows and then clicking on 'Update' in the context menu.
                                            In the former case, all action items are updated at once. In the latter case, only those action items are updated that are
                                            associated with the aging item to which the action item belongs that was clicked. There is no automatic running of the algorithm.
                                        </p>
                                        <p>The handlers of the click events are in file EqAgingActionItems.aspx.cs. From there, routines in file EqSupport.cs are called, in particular
                                            <b>ActionItemUpdate()</b>. When all items are being updated, that routine loops over all aging items. Note that aging items are independent
                                            of each other even when they refer to the same equipment component, whereas there exists some interdependency among action items belonging
                                            to the same aging item. The algorithm is designed in such a way that running it multiple times for the same aging item (or all aging items)
                                            does no harm except for the waste of computing resources in repeating the calculations.
                                        </p>
                                        <p>Within one loop over an aging item, we first determine the point in time when the last action took place; call it DLastAction.
                                            This is the latest of the following dates/times:
                                            <ul>
                                                <li>If a previous action item exists with a percent complete > 0, the date/time of that action</li>
                                                <li>The start date/time of the aging item</li>
                                                <li>The Link Begin date/time of the equipment component</li>
                                            </ul>
                                        </p>
                                        <p>Next, we determine the accumulated operating data (running times/cycles/distance traveled) up to DLastAction
                                            if we are scheduling by running times, cycles, or distance traveled.
                                        </p>
                                        <p>This is followed by calculating the <b>deadlines</b> for each of the scheduling methods specified by the chosen parameter set:
                                            <ul>
                                                <li>By elapsed time: in routine DTO_ElapsedNext().</li>
                                                <li>By number of operating hours or running time:
                                                    <ul>
                                                        <li>Either by simply adding the Estimated Operations Elapsed Time (see 'Aging Items' help tab),</li>
                                                        <li>or, if extrapolation was requested, by calling DLSExtrapHrs() where the 'LS' refers to least squares fitting.</li>
                                                    </ul>
                                                </li>
                                                <li>By number of cycles:
                                                    <ul>
                                                        <li>Either by simply adding the Estimated Cycles Elapsed Time (see 'Aging Items' help tab),</li>
                                                        <li>or, if extrapolation was requested, by calling DLSExtrapCycles().</li>
                                                    </ul>
                                                </li>
                                                <li>By distance traveled:
                                                    <ul>
                                                        <li>By simply adding the Estimated Distance Traveled Elapsed Time (see 'Aging Items' help tab). An extrapolation is not available at this time.</li>
                                                    </ul>
                                                </li>
                                                <li>By unique event: the deadline is simply the date of the event.</li>
                                            </ul>
                                        </p>
                                        <p>For a unique event, the deadline is already known. Otherwise, if more than one scheduling method was specified, the <b>deadline
                                            to be chosen</b> is the earliest of the up to four calculated ones.
                                        </p>
                                    </div>
                                </div><%-- SCR 213 --%>
                            </ContentTemplate>
                        </ajaxToolkit:TabPanel>
                        <ajaxToolkit:TabPanel runat="server" HeaderText="Page 14: Computational Algorithm - Least Squares Extrapolation" ScrollBars="Both" BackColor="#ffffcc">
                            <ContentTemplate>
                                <div class="HelpText" ><%-- SCR 213 --%>
                                    <div class="leftAlign">
                                        <p>This description is <b>not</b> required reading for operating the Equipment Aging portion of this website.</p>
                                        <p>For two of the scheduling methods (by running time/operating hours and by cycles) the user may ask for extrapolation of existing
                                            flight data into the future in order to estimate when the next deadline will occur. Here is an outline of how this happens
                                            (we discuss the approach for running time; cycles is quite similar):
                                            <ul>
                                                <li>From the database, read the operational calendar (specified in the Aging Item) into a list.</li>
                                                <li>From the database, read the relevant piece of equipment's flight-by-flight data into a list.</li>
                                                <li>Aggregate the raw by-flight data into daily operations in terms of hours flown per day; call it LiRaw.</li>
                                                <li>For purposes of fitting a single straight line through the data, and using the operational calendar,
                                                    remove from LiRaw the time gaps when no operations can take place; call the result LiShift.</li>
                                                <li>Loop over all list items in LiShift
                                                    <ul>
                                                        <li>Calculate the cumulative daily operational hours from LiShift; call the result ILiShift because it is the
                                                            integral of LiShift.
                                                        </li>
                                                        <li>Calculate the five sums that are required for the weighted least squares fit where 'weighted'
                                                            means that more recent data (later on the calendar) is given more importance so that the fit should turn out better
                                                            for the more recent past than the more distant one.
                                                        </li>
                                                        <li>The slope and intercept of the fitted straight line is calculated from those five sums. The least squares
                                                            method consists of minimizing the sum of the squares of the vertical deviations of the fitted line
                                                            from the given data. The minimization is done by setting up formulas for the sum S of those squares as a function
                                                            of slope k and intercept c. Taking the derivatives with respect to k and c and setting them to 0 results in two
                                                            linear equations in the two unknowns k and c which can now be solved for k and c.
                                                        </li>
                                                    </ul>
                                                </li>
                                                <li>The operational calendar is used once more to re-introduce the time intervals when no operations take place
                                                    to the straight line we just found, resulting in a broken line, and to ILiShift, resulting in the cumulative
                                                    unshifted daily operations. The latter are used for display purposes in illustrations. The former (the broken line)
                                                    is used to determine where it intersects with the horizontal line that represents the number of accumulated flight hours
                                                    at which the next action is to take place. That intersection happens at a certain date/time, and that is our deadline.
                                                </li>
                                                <li>Much of the above procedure is summarized in illustrations or charts that are made available for the user to inspect.
                                                    In particular, the goodness of the fit of the broken line with the accumulated flight data can be judged.
                                                </li>
                                            </ul>
                                        </p>
                                    </div>
                                </div><%-- SCR 213 --%>
                            </ContentTemplate>
                        </ajaxToolkit:TabPanel>
                    </ajaxToolkit:TabContainer>
                </Content>
            </ajaxToolkit:AccordionPane>

            <ajaxToolkit:AccordionPane ID="AccPLinks" runat="server" >
                <Header> Links to Other Equipment Aging Pages </Header>
                <Content>
                    <div class="HelpText" ><%-- SCR 213 --%>
                        <a href="../EqComponents.aspx">Work with Equipment Components</a><br />
                        <a href="EqAgingParSets.aspx">Work with Aging Parameter Sets</a><br />
                        <a href="OpsCalendars.aspx">Work with Operational Calendars</a><br />
                        <a href="EqAgingItems.aspx">Work with Equipment Aging Items</a><br />
                        <a href="EqAgingOperDat.aspx">Work with Operating Data</a><br />
                        <a href="EqAgingActionItems.aspx">Work with the Equipment Aging Register (List of Action Items)</a><br />
                        <br />
                        <a href="../../MemberPages/EquipMaintStat/EqMaintStatus.aspx">How the 'Member' Role sees the results of Equipment Aging Activities</a>
                    </div><%-- SCR 213 --%>
                </Content>
            </ajaxToolkit:AccordionPane>  
            
            <ajaxToolkit:AccordionPane ID="AccPTimeOfDay" runat="server" >
                <Header> Is Time Of Day Important? </Header>
                <Content>
                    <div class="HelpText" ><%-- SCR 213 --%>
                        <div class="divXSmallFont">To control whether or not the time of day is included in start/from and end/to dates in Equipment Aging:</div>
                        <p>Is Time Of Day important? (usually not - checkboxes are usually unchecked, but there are exceptions; see Introduction pages/tabs above):
                            <ul>
                                <li><asp:CheckBox ID="chbShowEqComp" runat="server" Checked="false" AutoPostBack="true" OnCheckedChanged="chbShowEqComp_CheckedChanged"
                                    Text="In Equipment Components, include LinkBegin and LinkEnd Times of Day instead of showing just the date." /></li>
                                <li><asp:CheckBox ID="chbShowTimes" runat="server" Checked="false" AutoPostBack="true" OnCheckedChanged="chbShowTimes_CheckedChanged"
                                    Text="In Aging Management, definition of Aging Items, include Start and End Times of Day instead of showing just the date." /></li>
                                <li><asp:CheckBox ID="chbShowEqOpDataTimes" runat="server" Checked="false" AutoPostBack="true" OnCheckedChanged="chbShowEqOpDataTimes_CheckedChanged"
                                    Text="In Aging Management, working with operating data records, include Start and End Times of Day instead of showing just the date." /></li>
                                <li><asp:CheckBox ID="chbShowEqActionItemsTimes" runat="server" Checked="false" AutoPostBack="true" OnCheckedChanged="chbShowEqActionItemsTimes_CheckedChanged"
                                    Text="In Aging Management, working with Action Item records, include Times of Day instead of showing just the date." /></li>
                            </ul>
                        </p>
                    </div><%-- SCR 213 --%>
                </Content>
            </ajaxToolkit:AccordionPane> 
            
        </Panes>            
    </ajaxToolkit:Accordion>

</asp:Content>

<asp:Content ID="FooterContent" ContentPlaceHolderID="ContentPlaceHolderFooter" runat="server">
    <%: Title %>
</asp:Content>
