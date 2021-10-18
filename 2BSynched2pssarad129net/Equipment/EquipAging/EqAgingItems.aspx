<%@ Page Title="Equipment Aging Items" Language="C#" MasterPageFile="~/mTSoar.Master" AutoEventWireup="true" CodeBehind="EqAgingItems.aspx.cs" Inherits="TSoar.Equipment.EquipAging.EqAgingItems" %>

<asp:Content ID="HeaderContent" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="server">
    <asp:Label runat="server" Text="Equipment Aging Items" Font-Italic="true" />
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="ContentPlaceHolderMain" runat="server">

    <asp:SqlDataSource ID="SqlDataSrc_EqAgPars" runat="server" ConnectionString="<%$ ConnectionStrings:SqlConn %>"
        SelectCommand="SELECT EQUIPAGINGPARS.ID, EQUIPAGINGPARS.sShortDescript + ' - [' + dbo.EQUIPACTIONTYPES.sEquipActionType + ']' AS sParSet
            FROM EQUIPAGINGPARS INNER JOIN EQUIPACTIONTYPES ON EQUIPAGINGPARS.iEquipActionType = EQUIPACTIONTYPES.ID
            ORDER BY EQUIPAGINGPARS.sShortDescript, EQUIPACTIONTYPES.sEquipActionType">
    </asp:SqlDataSource>

    <div class="row">
        <div class="col-sm-3">Links:</div>
        <div class="col-sm-3"><a href="EqAgingIntro.aspx">Introduction to / Help for Equipment Aging</a></div>
    </div>
    <div class="row">
        <div class="col-sm-2"><a href="../EqComponents.aspx">Work with Equipment Components</a></div>
        <div class="col-sm-2"><a href="EqAgingParSets.aspx">Work with Aging Parameter Sets</a></div>
        <div class="col-sm-2"><a href="OpsCalendars.aspx">Work with Operational Calendars</a></div>
        <div class="col-sm-2">Working with Equipment Aging Items</div>
        <div class="col-sm-2"><a href="EqAgingOperDat.aspx">Work with Operating Data</a></div>
        <div class="col-sm-2"><a href="EqAgingActionItems.aspx">Work with the Equipment Aging Register (List of Action Items)</a></div>
    </div>
    An "Aging Item" is the combination of an Aging Parameter Set, an Equipment Component, and an Operational Calendar. It is the item for which maintenance schedules and action items are determined.
    <div class="divXSmallFont"><a href="EqAgingIntro.aspx">To control whether or not the time of day is included in start/from and end/to dates in Equipment Aging</a></div>
    <asp:GridView ID="gvAgItems" runat="server" AutoGenerateColumns="False"
        GridLines="None" CssClass="SoarNPGridStyle"
        OnRowDataBound="gvAgItems_RowDataBound"
        OnRowEditing="gvAgItems_RowEditing"
        OnRowDeleting="gvAgItems_RowDeleting" OnRowCancelingEdit="gvAgItems_RowCancelingEdit"
        OnRowUpdating="gvAgItems_RowUpdating"
        OnPageIndexChanging="gvAgItems_PageIndexChanging"
        AllowPaging="true" PageSize="35" ShowHeaderWhenEmpty="true" EmptyDataText="At least one equipment component and at least one set of aging parameters must exist in order for aging items to make sense."
        Font-Size="Small">
        <PagerSettings Mode="NextPreviousFirstLast" Position="TopAndBottom" Visible="true" FirstPageText="&nbsp;First&nbsp;"
            LastPageText="&nbsp;Last&nbsp;" NextPageText="&nbsp;Next&nbsp;" PreviousPageText="&nbsp;Previous&nbsp;" />
        <PagerStyle CssClass="SoarNPpaging" />
        <Columns>
            <asp:TemplateField HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                <HeaderTemplate>
                    <asp:Label ID="lblHIdent" runat="server" Text="Internal Id" Width="60" ToolTip="Points to a row in database table EQUIPAGINGITEMS with this ID field contents"></asp:Label>
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:Label ID="lblIIdent" runat="server" Text='<%# Eval("ID") %>' Width="60" ></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>

            <asp:TemplateField>
                <HeaderTemplate>
                    <asp:Label ID="lblHCompName" runat="server" Text="Component Name" ToolTip="The component with which this aging item is associated."></asp:Label>
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:Label runat="server" ID="lblCompName" Text='<%# TSoar.Equipment.EqSupport.sExpandedComponentName((int)Eval("iEquipComponent")) %>' />
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:DropDownList ID="DDLCompName" runat="server" Width="200" DataValueField="ID"
                        DataTextField="Component"/>
                </EditItemTemplate>
            </asp:TemplateField>

            <asp:TemplateField HeaderStyle-HorizontalAlign="Center">
                <HeaderTemplate>
                    <asp:Label ID="lblHName" runat="server" Text="Aging Item Name" Width="150"></asp:Label>
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:Label ID="lblIName" runat="server" Text='<%# Eval("sName") %>' Width="150"></asp:Label>
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:TextBox ID="txbDName" runat="server" Text='<%# Eval("sName") %>' Width="150"></asp:TextBox>
                </EditItemTemplate>
            </asp:TemplateField>

            <asp:TemplateField>
                <HeaderTemplate>
                    <asp:Label ID="lblHParSet" runat="server" Text="Parameter Set" ToolTip="The aging parameter set used by this aging item."></asp:Label>
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:Label runat="server" ID="lblIParSet" Text='<%# Bind("sShortDescript") %>' />
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:DropDownList ID="DDLParSets" runat="server" DataSourceID="SqlDataSrc_EqAgPars" DataValueField="ID"
                        DataTextField="sParSet"/>
                </EditItemTemplate>
            </asp:TemplateField>

            <asp:TemplateField>
                <HeaderTemplate>
                    <asp:Label ID="lblHOpsCal" runat="server" Text="Operational Calendar"
                        ToolTip="Which operational calendar does this component use?
 Only meaningful when using the extrapolation feature for determining a future action item deadline.
 Certainly irrelevant for 'unique' single event aging items."></asp:Label>
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:Label runat="server" ID="lblIOpsCal" Text='<%# Eval("sOpsCalName") %>'/>
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:DropDownList ID="DDLEOpsCal" runat="server"/>
                </EditItemTemplate>
            </asp:TemplateField>

            <asp:TemplateField ItemStyle-Width="115" >
                <HeaderTemplate>
                    <asp:Label ID="lblHStartDate" runat="server" Text="Start Date" Width="115" 
                        ToolTip="Date when aging event tracking starts; default is a long time ago, i.e., start is unknown.
However, this start date is assumed to be the point in time when the action required by this aging item was first performed;
in other words: in the absence of a previous action item associated with this aging item,
a new action item deadline is calculated using this start time as a baseline."></asp:Label>
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:Label runat="server" ID="lblIStartDate" Text='<%# TSoar.CustFmt.sFmtDate(((DateTimeOffset)Eval("DStart")),TSoar.CustFmt.enDFmt.DateOnly) %>' Width="115"/>
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:TextBox ID="txbDStartDate" runat="server" Text='<%# TSoar.CustFmt.sFmtDate(((DateTimeOffset)Eval("DStart")),TSoar.CustFmt.enDFmt.DateOnly).Replace("/","-") %>'
                        TextMode="Date"  Font-Size="X-Small" Width="115"></asp:TextBox>
                </EditItemTemplate>
            </asp:TemplateField>

            <asp:TemplateField >
                <HeaderTemplate>
                    <asp:Label ID="lblHStartTime" runat="server" Text="Start Time" 
                        ToolTip="Time of day when aging event tracking starts; it is usually of minor importance. Default is 1 hour and 1 minute after midnight."></asp:Label>
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:Label runat="server" ID="lblIStartTime" Text='<%# TSoar.CustFmt.sFmtDate(((DateTimeOffset)Eval("DStart")),TSoar.CustFmt.enDFmt.TimeOnlyMin) %>'/>
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:TextBox ID="txbDStartTime" runat="server" Text='<%# TSoar.CustFmt.sFmtDate(((DateTimeOffset)Eval("DStart")),TSoar.CustFmt.enDFmt.TimeOnlyMin) %>'
                        TextMode="Time" Font-Size="X-Small"></asp:TextBox>
                </EditItemTemplate>
            </asp:TemplateField>

            <asp:TemplateField >
                <HeaderTemplate>
                    <asp:Label ID="lblHStartOffset" runat="server" Text="Start Offset"
                        ToolTip="Time offset to UTC when aging event tracking starts; almost always of very little importance. Default is the Setting called 'TimeZoneOffset' (-08:00 in the Pacific Standard Time Zone). Limited to +/-14:00."></asp:Label>
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:Label runat="server" ID="lblIStartOffset" Text='<%# TSoar.CustFmt.sFmtDate(((DateTimeOffset)Eval("DStart")),TSoar.CustFmt.enDFmt.DateAndTimeSec).Substring(20,6) %>' />
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:TextBox ID="txbDStartOffset" runat="server" Text='<%# TSoar.CustFmt.sFmtDate(((DateTimeOffset)Eval("DStart")),TSoar.CustFmt.enDFmt.DateAndTimeSec).Substring(20,6) %>' TextMode="SingleLine" ></asp:TextBox>
                    <asp:RegularExpressionValidator runat="server" ControlToValidate="txbDStartOffset" ValidationExpression="^[-+]((1[0-4])|(0\d)):[0-5]\d$"
                        ErrorMessage="Must be a time offset from local to UTC like -08:00" Display="Dynamic" ForeColor="Red" BorderStyle="Dotted" BorderColor="Red" BorderWidth="4px"/>
                </EditItemTemplate>
            </asp:TemplateField>

            <asp:TemplateField ItemStyle-Width="115" >
                <HeaderTemplate>
                    <asp:Label ID="lblHEndDate" runat="server" Text="End Date" Width="115" 
                        ToolTip="Date when aging event tracking ends; default is a long time in the future, i.e., tracking practically never ends."></asp:Label>
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:Label runat="server" ID="lblIEndDate" Text='<%# TSoar.CustFmt.sFmtDate(((DateTimeOffset)Eval("DEnd")),TSoar.CustFmt.enDFmt.DateOnly) %>' Width="115"/>
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:TextBox ID="txbDEndDate" runat="server" Text='<%# TSoar.CustFmt.sFmtDate(((DateTimeOffset)Eval("DEnd")),TSoar.CustFmt.enDFmt.DateOnly).Replace("/","-") %>'
                        TextMode="Date" Font-Size="X-Small" Width="115"></asp:TextBox>
                </EditItemTemplate>
            </asp:TemplateField>

            <asp:TemplateField >
                <HeaderTemplate>
                    <asp:Label ID="lblHEndTime" runat="server" Text="End Time"
                        ToolTip="Time of day when aging event tracking ends; it is usually of minor importance. Default is 1 hour and 1 minute before midnight."></asp:Label>
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:Label runat="server" ID="lblIEndTime" Text='<%# TSoar.CustFmt.sFmtDate(((DateTimeOffset)Eval("DEnd")),TSoar.CustFmt.enDFmt.TimeOnlyMin) %>' />
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:TextBox ID="txbDEndTime" runat="server" Text='<%# TSoar.CustFmt.sFmtDate(((DateTimeOffset)Eval("DEnd")),TSoar.CustFmt.enDFmt.TimeOnlyMin) %>' TextMode="Time"></asp:TextBox>
                </EditItemTemplate>
            </asp:TemplateField>

            <asp:TemplateField >
                <HeaderTemplate>
                    <asp:Label ID="lblHEndOffset" runat="server" Text="End Offset"
                        ToolTip="Time offset to UTC when aging event tracking ends; it is almost always of very little importance. Default is the Setting called 'TimeZoneOffset' (-08:00 in the Pacific Standard Time Zone). Limited to +/-14:00."></asp:Label>
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:Label runat="server" ID="lblIEndOffset" Text='<%# TSoar.CustFmt.sFmtDate(((DateTimeOffset)Eval("DEnd")),TSoar.CustFmt.enDFmt.DateAndTimeSec).Substring(20,6) %>' />
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:TextBox ID="txbDEndOffset" runat="server" Text='<%# TSoar.CustFmt.sFmtDate(((DateTimeOffset)Eval("DEnd")),TSoar.CustFmt.enDFmt.DateAndTimeSec).Substring(20,6) %>' TextMode="SingleLine" ></asp:TextBox>
                    <asp:RegularExpressionValidator runat="server" ControlToValidate="txbDEndOffset" ValidationExpression="^[-+]((1[0-4])|(0\d)):[0-5]\d$"
                        ErrorMessage="Must be a time offset from local to UTC like -08:00" Display="Dynamic" ForeColor="Red" BorderStyle="Dotted" BorderColor="Red" BorderWidth="4px"/>
                </EditItemTemplate>
            </asp:TemplateField>

            <asp:TemplateField HeaderStyle-HorizontalAlign="Center">
                <HeaderTemplate>
                    <asp:Label ID="lblHEstRun" runat="server" Text="Estim. Op. El. Time"
                        ToolTip="Estimated Operating Elapsed Time: Used only when scheduling by operating time (greyed out when not needed): the estimated elapsed time on the calendar (in units of days) between required actions;
 aids in projecting the scheduled start of the next action item. For example, how long will it take for the towplane's engine to run 2000 hours, the running time between replacement or overhaul?
 Say, 10 years; the number to enter here would be 365.25 days/year x 10 years = 3652.5 days.
 NOT USED if extrapolation based on operational data is successful."></asp:Label>
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:Label ID="lblIEstRun" runat="server" Text='<%# Eval("dEstRunDays") %>'></asp:Label>
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:TextBox ID="txbDEstRun" runat="server" Text='<%# Eval("dEstRunDays") %>' TextMode="SingleLine" Width="60"></asp:TextBox>
                    <asp:RegularExpressionValidator runat="server" ControlToValidate="txbDEstRun" ValidationExpression="^+?((\d{1,8}(\.\d{0,4})?)|(\.\d{1,4}))$"
                        ErrorMessage="Must be > 0, up to 12 digits, up to 4 decimal places" Display="Dynamic" ForeColor="Red" BorderStyle="Dotted" BorderColor="Red" BorderWidth="4px"/>
                </EditItemTemplate>
            </asp:TemplateField>

            <asp:TemplateField HeaderStyle-HorizontalAlign="Center">
                <HeaderTemplate>
                    <asp:Label ID="lblHbRun" runat="server" Text="Extr?"
                        ToolTip="Checked if an attempt should be made to predict the next deadline using extrapolated operational/running time data" />
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:CheckBox ID="chbIbRun" runat="server" Checked='<%# Eval("bRunExtrap") %>' Enabled="false" />
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:CheckBox ID="chbEbRun" runat="server" Checked='<%# Eval("bRunExtrap") %>' Enabled="true" />
                </EditItemTemplate>
            </asp:TemplateField>

            <asp:TemplateField HeaderStyle-HorizontalAlign="Center">
                <HeaderTemplate>
                    <asp:Label ID="lblHEstCycl" runat="server" Text="Estim. Cycles El. Time"
                        ToolTip="Estimated Cycles Elapsed Time: Used only when scheduling by number of cycles (greyed out when not needed): the estimated elapsed time on the calendar (in units of days) between required actions;
 aids in projecting the scheduled start of the next action item. For example, how many days does it it take for a tow release to be activated 6000 times?
 NOT USED if extrapolation based on operational data is successful."></asp:Label>
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:Label ID="lblIEstCycl" runat="server" Text='<%# Eval("dEstCycleDays") %>'></asp:Label>
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:TextBox ID="txbDEstCycl" runat="server" Text='<%# Eval("dEstCycleDays") %>' Width="60"></asp:TextBox>
                    <asp:RegularExpressionValidator runat="server" ControlToValidate="txbDEstDist" ValidationExpression="^+?((\d{1,8}(\.\d{0,4})?)|(\.\d{1,4}))$"
                        ErrorMessage="Must be > 0, up to 12 digits, up to 4 decimal places" Display="Dynamic" ForeColor="Red" BorderStyle="Dotted" BorderColor="Red" BorderWidth="4px"/>
                </EditItemTemplate>
            </asp:TemplateField>

            <asp:TemplateField HeaderStyle-HorizontalAlign="Center">
                <HeaderTemplate>
                    <asp:Label ID="lblHbCycl" runat="server" Text="Extr?"
                        ToolTip="Checked if an attempt should be made to predict the next deadline using extrapolated operational cycle data" />
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:CheckBox ID="chbIbCycl" runat="server" Checked='<%# Eval("bCyclExtrap") %>' Enabled="false" />
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:CheckBox ID="chbEbCycl" runat="server" Checked='<%# Eval("bCyclExtrap") %>' Enabled="true" />
                </EditItemTemplate>
            </asp:TemplateField>

            <asp:TemplateField HeaderStyle-HorizontalAlign="Center">
                <HeaderTemplate>
                    <asp:Label ID="lblHEstDist" runat="server" Text="Estim. Dist. El. Time"
                        ToolTip="Estimated Distance Elapsed Time: Used only when scheduling by distance traveled (greyed out when not needed): the estimated elapsed time on the calendar (in units of days) between required actions;
 aids in projecting the scheduled start of the next action item. For example, how long will it take for the tow truck to travel 12500 miles, the mileage between gear box oil changes?
 Say, 1.5 years; the number to enter here would be 365.25 days/year x 1.5 years ~= 548 days.
 NOT USED if extrapolation based on operational data is successful."></asp:Label>
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:Label ID="lblIEstDist" runat="server" Text='<%# Eval("dEstDistDays") %>'></asp:Label>
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:TextBox ID="txbDEstDist" runat="server" Text='<%# Eval("dEstDistDays") %>' TextMode="SingleLine" Width="60"></asp:TextBox>
                    <asp:RegularExpressionValidator runat="server" ControlToValidate="txbDEstDist" ValidationExpression="^+?((\d{1,8}(\.\d{0,4})?)|(\.\d{1,4}))$"
                        ErrorMessage="Must be > 0, up to 12 digits, up to 4 decimal places" Display="Dynamic" ForeColor="Red" BorderStyle="Dotted" BorderColor="Red" BorderWidth="4px"/>
                </EditItemTemplate>
            </asp:TemplateField>

            <asp:TemplateField HeaderStyle-HorizontalAlign="Center">
                <HeaderTemplate>
                    <asp:Label ID="lblHbDist" runat="server" Text="Extr?"
                        ToolTip="Checked if an attempt should be made to predict the next deadline using extrapolated distance traveled data" />
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:CheckBox ID="chbIbDist" runat="server" Checked='<%# Eval("bDistExtrap") %>' Enabled="false" />
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:CheckBox ID="chbEbDist" runat="server" Checked='<%# Eval("bDistExtrap") %>' Enabled="true" />
                </EditItemTemplate>
            </asp:TemplateField>

            <asp:TemplateField HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Right">
                <HeaderTemplate>
                    <asp:Label ID="lblHEstDur" runat="server" Text="Estim. Work Duration"
                        ToolTip="Estimated elapsed calendar time it will take to complete the work of the action, in units of days"></asp:Label>
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:Label ID="lblIEstDur" runat="server" Text='<%# Eval("dEstDuration") %>'></asp:Label>
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:TextBox ID="txbDEstDur" runat="server" Text='<%# Eval("dEstDuration") %>' TextMode="SingleLine" Width="60"></asp:TextBox>
                    <asp:RegularExpressionValidator runat="server" ControlToValidate="txbDEstDur" ValidationExpression="^+?((\d{1,8}(\.\d{0,4})?)|(\.\d{1,4}))$"
                        ErrorMessage="Must be > 0, up to 12 digits, up to 4 decimal places" Display="Dynamic" ForeColor="Red" BorderStyle="Dotted" BorderColor="Red" BorderWidth="4px"/>
                </EditItemTemplate>
            </asp:TemplateField>

            <asp:TemplateField HeaderStyle-HorizontalAlign="Center">
                <HeaderTemplate>
                    <asp:Label ID="lblHComments" runat="server" Text="Comments"></asp:Label>
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:Label ID="lblIComments" runat="server" Text='<%# Eval("sComment") %>'></asp:Label>
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:TextBox ID="txbDComments" runat="server" Text='<%# Eval("sComment") %>' Width="150"></asp:TextBox>
                </EditItemTemplate>
            </asp:TemplateField>

            <asp:TemplateField ItemStyle-HorizontalAlign="Center">
                <HeaderTemplate>
                    Edit
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:ImageButton ID="ipbEEdit" ImageUrl="~/i/BlueButton.jpg" runat="server" CommandName="Edit" OnClientClick="oktoSubmit = true;" />
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:ImageButton ID="ipbEUpdate" ImageUrl="~/i/Update.jpg" runat="server" CommandName="Update" OnClientClick="oktoSubmit = true;" />
                    <asp:ImageButton ID="ipbECancel" ImageUrl="~/i/Cancel.jpg" runat="server" CommandName="Cancel" OnClientClick="oktoSubmit = true;" />
                </EditItemTemplate>
            </asp:TemplateField>

            <asp:TemplateField ItemStyle-HorizontalAlign="Center">
                <HeaderTemplate>
                    Delete
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:ImageButton ID="ipbEDelete" ImageUrl="~/i/RedButton.jpg" runat="server" CommandName="Delete" OnClientClick="oktoSubmit = true;" />
                </ItemTemplate>
                <EditItemTemplate>
                    &nbsp;
                </EditItemTemplate>
            </asp:TemplateField>

        </Columns>
    </asp:GridView>

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
