<%@ Page Title="Equipment Aging Action Items" Language="C#" MasterPageFile="~/mTSoar.Master" AutoEventWireup="true" CodeBehind="EqAgingActionItems.aspx.cs" Inherits="TSoar.Equipment.EquipAging.EqAgingActionItems" %>

<asp:Content ID="HeaderContent" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="server">
    <asp:Label runat="server" Text="Equipment Aging - Action Items" Font-Italic="true" />
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="ContentPlaceHolderMain" runat="server">

    <div class="row">
        <div class="col-sm-3">Links:</div>
        <div class="col-sm-3"><a href="EqAgingIntro.aspx">Introduction to / Help for Equipment Aging</a></div>
    </div>
    <div class="row">
        <div class="col-sm-2"><a href="../EqComponents.aspx">Work with Equipment Components</a></div>
        <div class="col-sm-2"><a href="EqAgingParSets.aspx">Work with Aging Parameter Sets</a></div>
        <div class="col-sm-2"><a href="OpsCalendars.aspx">Work with Operational Calendars</a></div>
        <div class="col-sm-2"><a href="EqAgingItems.aspx">Work with Equipment Aging Items</a></div>
        <div class="col-sm-2"><a href="EqAgingOperDat.aspx">Work with Operating Data</a></div>
        <div class="col-sm-2">Working with the Equipment Aging Register (List of Action Items)</div>
    </div>
    Date and Time of Last Update of all Action Items Associated with All Aging Items: <asp:Label ID="lblLastUpd" runat="server" Text="Never" />
    <asp:Button ID="pbUpdate" runat="server" Text="Update Aging Items" OnClick="pbUpdate_Click"
        ToolTip="Update the entire list of equipment aging items and their associated action items.
 You can update a single aging item by clicking on one of the 'Options' buttons of an associated action item in the right-most column of the table below." />
    <div class="divXSmallFont"><a href="EqAgingIntro.aspx">Click here to control whether or not the time of day is included in start/from and end/to dates in Equipment Aging</a>
        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Special Dates: 2001/01/01 01:01:00 +00:00 means 'not started'; 2999/12/31 22:59:00 +00:00 means 'not completed'.
    </div>
    <asp:GridView ID="gvActItems" runat="server" AutoGenerateColumns="False"
        GridLines="None" CssClass="SoarNPGridStyle"
        OnRowDataBound="gvActItems_RowDataBound"
        OnRowEditing="gvActItems_RowEditing"
        OnRowCancelingEdit="gvActItems_RowCancelingEdit"
        OnRowUpdating="gvActItems_RowUpdating"
        AllowPaging="true" PageSize="35" OnPageIndexChanging="gvActItems_PageIndexChanging" ShowHeaderWhenEmpty="true"
        Font-Size="Small">
        <PagerSettings Mode="NextPreviousFirstLast" Position="TopAndBottom" Visible="true" FirstPageText="&nbsp;First&nbsp;"
            LastPageText="&nbsp;Last&nbsp;" NextPageText="&nbsp;Next&nbsp;" PreviousPageText="&nbsp;Previous&nbsp;" />
        <PagerStyle CssClass="SoarNPpaging" />
        <Columns>
            <asp:TemplateField HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                <HeaderTemplate>
                    <asp:Label ID="lblHIdent" runat="server" Text="Internal Action Item Id" Width="60"
                        ToolTip="Points to a row in database table EQUIPACTIONITEMS with this ID field contents"></asp:Label>
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:Label ID="lblIIdent" runat="server" Text='<%# Eval("ID") %>' Width="60" ></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>

            <asp:TemplateField HeaderStyle-HorizontalAlign="Center">
                <HeaderTemplate>
                    <asp:Label ID="lblHAgingItem" runat="server" Text="Aging Item Name" Width="200" ToolTip="the name assigned in 'Equipment Aging Items'."></asp:Label>
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:Label ID="lblIAgingItem" runat="server" Text='<%# Eval("sName") %>' Width="200"></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>

            <asp:TemplateField >
                <HeaderTemplate>
                    <asp:Label ID="lblHDeadLine" runat="server" Text="Deadline Date" ToolTip="The deadline calculated for this aging item" ></asp:Label>
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:Label runat="server" ID="lblIDeadLine" Text='<%# TSoar.CustFmt.sFmtDate(((DateTimeOffset)Eval("DeadLine")),TSoar.CustFmt.enDFmt.DateOnly) %>'/>
                </ItemTemplate>
            </asp:TemplateField>

            <asp:TemplateField >
                <HeaderTemplate>
                    <asp:Label ID="lblHDeadLineTime" runat="server" Text="DL Time" 
                        ToolTip="Time of day of deadline; it is usually of minor importance."></asp:Label>
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:Label runat="server" ID="lblIDeadLineTime" Text='<%# TSoar.CustFmt.sFmtDate(((DateTimeOffset)Eval("DeadLine")),TSoar.CustFmt.enDFmt.TimeOnlyMin) %>'/>
                </ItemTemplate>
            </asp:TemplateField>

            <asp:TemplateField >
                <HeaderTemplate>
                    <asp:Label ID="lblHDeadLineOffset" runat="server" Text="DL Offset"
                        ToolTip="Time offset to UTC of the dealine time; almost always of very little importance. Often equals the contents of Setting 'TimeZoneOffset' (-08:00 in the Pacific Standard Time Zone)."></asp:Label>
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:Label runat="server" ID="lblIDeadLineOffset" Text='<%# TSoar.CustFmt.sFmtDate(((DateTimeOffset)Eval("DeadLine")),TSoar.CustFmt.enDFmt.DateAndTimeSec).Substring(20,6) %>' />
                </ItemTemplate>
            </asp:TemplateField>

            <asp:TemplateField HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Right">
                <HeaderTemplate>
                    <asp:Label ID="lblHDeadLineHrs" runat="server" Text="Op Hrs Limit" Width="80"
                        ToolTip="The limit in terms of operating hours. -9.9900 signifies 'not used'. The 'hours' deadline date occurs when this amount of running time/operating hours is reached."></asp:Label>
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:Label ID="lblIDeadLineHrs" runat="server" Text='<%# Eval("dDeadlineHrs") %>' Width="80"></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>

            <asp:TemplateField HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Right">
                <HeaderTemplate>
                    <asp:Label ID="lblHDeadLineCycl" runat="server" Text="Cycles Limit" Width="80"
                        ToolTip="The limit in terms of number of cycles. -999 signifies 'not used'. The 'cycles' deadline date occurs when this number of cycles is reached."></asp:Label>
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:Label ID="lblIDeadLineCycl" runat="server" Text='<%# Eval("iDeadlineCycles") %>' Width="80"></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>

            <asp:TemplateField HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Right">
                <HeaderTemplate>
                    <asp:Label ID="lblHDeadLineDist" runat="server" Text="Op Dist Limit" Width="80"
                        ToolTip="The limit in terms of distance traveled. -9.9900 signifies 'not used'. The 'distance' deadline date occurs when this amount of distance traveled has been reached."></asp:Label>
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:Label ID="lblIDeadLineDist" runat="server" Text='<%# Eval("dDeadLineDist") %>' Width="80"></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>

            <asp:TemplateField >
                <HeaderTemplate>
                    <asp:Label ID="lblHSchedStart" runat="server" Text="Scheduled Start Date"  ></asp:Label>
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:Label runat="server" ID="lblISchedStart" Text='<%# TSoar.CustFmt.sFmtDate(((DateTimeOffset)Eval("DScheduledStart")),TSoar.CustFmt.enDFmt.DateOnly) %>' />
                </ItemTemplate>
            </asp:TemplateField>

            <asp:TemplateField >
                <HeaderTemplate>
                    <asp:Label ID="lblHSchedStartTime" runat="server" Text="Sched. Time" 
                        ToolTip="Time of day of scheduled start time; it is usually of minor importance. Default is 1 hour and 1 minute after midnight."></asp:Label>
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:Label runat="server" ID="lblISchedStartTime" Text='<%# TSoar.CustFmt.sFmtDate(((DateTimeOffset)Eval("DScheduledStart")),TSoar.CustFmt.enDFmt.TimeOnlyMin) %>'/>
                </ItemTemplate>
            </asp:TemplateField>

            <asp:TemplateField >
                <HeaderTemplate>
                    <asp:Label ID="lblHSchedStartOffset" runat="server" Text="Sched. Offset"
                        ToolTip="Time offset to UTC of scheduled start time; almost always of very little importance. Default is the Setting called 'TimeZoneOffset' (-08:00 in the Pacific Standard Time Zone)"></asp:Label>
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:Label runat="server" ID="lblISchedStartOffset" Text='<%# TSoar.CustFmt.sFmtDate(((DateTimeOffset)Eval("DScheduledStart")),TSoar.CustFmt.enDFmt.DateAndTimeSec).Substring(20,6) %>' />
                </ItemTemplate>
            </asp:TemplateField>

            <asp:TemplateField ItemStyle-Width="115" >
                <HeaderTemplate>
                    <asp:Label ID="lblHActStart" runat="server" Text="Actual Start Date" Width="115" ToolTip=
                        "The date when the action actually started. 2001/01/01 is a special date signifying that the action has not been started."></asp:Label>
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:Label runat="server" ID="lblIActStart" Text='<%# TSoar.CustFmt.sFmtDate(((DateTimeOffset)Eval("DActualStart")),TSoar.CustFmt.enDFmt.DateOnly) %>' Width="115"/>
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:TextBox ID="txbDActStart" runat="server"
                        Text='<%# TSoar.CustFmt.sFmtDate(((DateTimeOffset)Eval("DActualStart")),TSoar.CustFmt.enDFmt.DateOnly).Replace("/","-") %>'
                        TextMode="Date" Font-Size="X-Small" Width="115"></asp:TextBox>
                </EditItemTemplate>
            </asp:TemplateField>

            <asp:TemplateField >
                <HeaderTemplate>
                    <asp:Label ID="lblHActStartTime" runat="server" Text="Actual Time" 
                        ToolTip="Time of day of actual start time; it is usually of minor importance. Default is 1 hour and 1 minute after midnight."></asp:Label>
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:Label runat="server" ID="lblIActStartTime" Text='<%# TSoar.CustFmt.sFmtDate(((DateTimeOffset)Eval("DActualStart")),TSoar.CustFmt.enDFmt.TimeOnlyMin) %>'/>
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:TextBox ID="txbDActStartTime" runat="server" Text='<%# TSoar.CustFmt.sFmtDate(((DateTimeOffset)Eval("DActualStart")),TSoar.CustFmt.enDFmt.TimeOnlyMin) %>' TextMode="Time"></asp:TextBox>
                </EditItemTemplate>
            </asp:TemplateField>

            <asp:TemplateField >
                <HeaderTemplate>
                    <asp:Label ID="lblHActStartOffset" runat="server" Text="Actual Offset"
                        ToolTip="Time offset to UTC of actual start time; almost always of very little importance. Default is the Setting called 'TimeZoneOffset' (-08:00 in the Pacific Standard Time Zone). Limited to +/-14:00."></asp:Label>
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:Label runat="server" ID="lblIActStartOffset" Text='<%# TSoar.CustFmt.sFmtDate(((DateTimeOffset)Eval("DActualStart")),TSoar.CustFmt.enDFmt.DateAndTimeSec).Substring(20,6) %>' />
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:TextBox ID="txbDActStartOffset" runat="server" Text='<%# TSoar.CustFmt.sFmtDate(((DateTimeOffset)Eval("DActualStart")),TSoar.CustFmt.enDFmt.DateAndTimeSec).Substring(20,6) %>' TextMode="SingleLine" ></asp:TextBox>
                    <asp:RegularExpressionValidator runat="server" ControlToValidate="txbDActStartOffset" ValidationExpression="^[-+]((1[0-4])|(0\d)):[0-5]\d$"
                        ErrorMessage="Must be a time offset from local to UTC like -08:00" Display="Dynamic" ForeColor="Red" BorderStyle="Dotted" BorderColor="Red" BorderWidth="4px"/>
                </EditItemTemplate>
            </asp:TemplateField>

            <asp:TemplateField HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                <HeaderTemplate>
                    <asp:Label ID="lblHPerc" runat="server" Text="Percent Complete" Width="60"
                        ToolTip="An estimate of the state of completion of the action item:
0 = not started; a scheduled start date exists
> 0, < 100 = started but incomplete; an actual start date must exist; completion date is an estimate
100 = action item completed; Action Item Completed date exists.">
                    </asp:Label>
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:Label ID="lblIPerc" runat="server" Text='<%# Eval("iPercentComplete") %>' Width="60"></asp:Label>
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:TextBox ID="txbDPerc" runat="server" Text='<%# Eval("iPercentComplete") %>' TextMode="Number" Width="60"></asp:TextBox>
                    <asp:RegularExpressionValidator runat="server" ControlToValidate="txbDPerc" ValidationExpression="^0*((\d\d?)|(100))$"
                        ErrorMessage="Must be an integer between 0 and 100" Display="Dynamic" ForeColor="Red" BorderStyle="Dotted"
                        BorderColor="Red" BorderWidth="4px" />
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="txbDPerc" ErrorMessage="Percent Complete must not be empty"
                        Display="Dynamic" ForeColor="Red" BorderStyle="Dotted" BorderColor="Red" BorderWidth="4px" />
                </EditItemTemplate>
            </asp:TemplateField>

            <asp:TemplateField ItemStyle-Width="115" >
                <HeaderTemplate>
                    <asp:Label ID="lblHComplDate" runat="server" Text="Completion Date" Width="115"
                        ToolTip="The date on which the action item was completed. If it exists and if the action is not yet completed, this is an estimated date. 2999/12/31 is a special date signifying that the action has not been completed." ></asp:Label>
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:Label runat="server" ID="lblIComplDate" Text='<%# TSoar.CustFmt.sFmtDate(((DateTimeOffset)Eval("DComplete")),TSoar.CustFmt.enDFmt.DateOnly) %>' Width="115"/>
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:TextBox ID="txbDComplDate" runat="server"
                        Text='<%# TSoar.CustFmt.sFmtDate(((DateTimeOffset)Eval("DComplete")),TSoar.CustFmt.enDFmt.DateOnly).Replace("/","-") %>'
                        TextMode="Date" Font-Size="X-Small" Width="115"></asp:TextBox>
                </EditItemTemplate>
            </asp:TemplateField>

            <asp:TemplateField >
                <HeaderTemplate>
                    <asp:Label ID="lblHComplTime" runat="server" Text="Compl. Time"
                        ToolTip="Time of day when action was completed; it is usually of minor importance. Default is 1 hour and 1 minute before midnight."></asp:Label>
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:Label runat="server" ID="lblIComplTime" Text='<%# TSoar.CustFmt.sFmtDate(((DateTimeOffset)Eval("DComplete")),TSoar.CustFmt.enDFmt.TimeOnlyMin) %>' />
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:TextBox ID="txbDComplTime" runat="server" Text='<%# TSoar.CustFmt.sFmtDate(((DateTimeOffset)Eval("DComplete")),TSoar.CustFmt.enDFmt.TimeOnlyMin) %>' TextMode="Time"></asp:TextBox>
                </EditItemTemplate>
            </asp:TemplateField>

            <asp:TemplateField >
                <HeaderTemplate>
                    <asp:Label ID="lblHComplOffset" runat="server" Text="Compl. Offset"
                        ToolTip="Time offset to UTC when action was completed; it is almost always of very little importance. Default is the Setting called 'TimeZoneOffset' (-08:00 in the Pacific Standard Time Zone). Limited to +/-14:00."></asp:Label>
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:Label runat="server" ID="lblIComplOffset" Text='<%# TSoar.CustFmt.sFmtDate(((DateTimeOffset)Eval("DComplete")),TSoar.CustFmt.enDFmt.DateAndTimeSec).Substring(20,6) %>' />
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:TextBox ID="txbDComplOffset" runat="server" Text='<%# TSoar.CustFmt.sFmtDate(((DateTimeOffset)Eval("DComplete")),TSoar.CustFmt.enDFmt.DateAndTimeSec).Substring(20,6) %>' TextMode="SingleLine" ></asp:TextBox>
                    <asp:RegularExpressionValidator runat="server" ControlToValidate="txbDComplOffset" ValidationExpression="^[-+]((1[0-4])|(0\d)):[0-5]\d$"
                        ErrorMessage="Must be a time offset from local to UTC like -08:00" Display="Dynamic" ForeColor="Red" BorderStyle="Dotted" BorderColor="Red" BorderWidth="4px"/>
                </EditItemTemplate>
            </asp:TemplateField>
                                            
            <asp:TemplateField HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Right">
                <HeaderTemplate>
                    <asp:Label ID="lblHAtComplHrs" runat="server" Text="Hrs @ Completion"
                        ToolTip="Accumulated operating hours at time of action item completion. Empty or -9.99 means `unknown`. 0.0 (zero) is a legitimate number."></asp:Label>
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:Label ID="lblIAtComplHrs" runat="server" Text='<%# Eval("dAtCompletionHrs") %>'></asp:Label>
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:TextBox ID="txbEAtComplHrs" runat="server" Text='<%# Eval("dAtCompletionHrs") %>' />
                    <asp:RegularExpressionValidator runat="server" ControlToValidate="txbEAtComplHrs" ValidationExpression="^((-9\.990{0,2})?|\+?((\d{1,8}(\.\d{0,4})?)|(\.\d{1,4})))?$"
                        ErrorMessage="Must be empty or -9.99, or >= 0, up to 12 digits, up to 4 decimal places" Display="Dynamic" ForeColor="Red" BorderStyle="Dotted" BorderColor="Red" BorderWidth="4px"/>
                </EditItemTemplate>
            </asp:TemplateField>
                                            
            <asp:TemplateField HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Right">
                <HeaderTemplate>
                    <asp:Label ID="lblHAtComplCycl" runat="server" Text="Cycles @ Completion"
                        ToolTip="Accumulated cycles at time of action item completion. Empty or -999 means `unknown`. 0 (zero) is a legitimate number."></asp:Label>
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:Label ID="lblIAtComplCycl" runat="server" Text='<%# Eval("iAtCompletionCycles") %>'></asp:Label>
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:TextBox ID="txbEAtComplCycl" runat="server" Text='<%# Eval("iAtCompletionCycles") %>' TextMode="Number" />
                    <asp:RegularExpressionValidator runat="server" ControlToValidate="txbEAtComplCycl" ValidationExpression="^((-999)|(\+?\d{1,11}))?$"
                        ErrorMessage="Must be empty or -999, or >= 0, up to 11 digits" Display="Dynamic" ForeColor="Red" BorderStyle="Dotted" BorderColor="Red" BorderWidth="4px"/>
                </EditItemTemplate>
            </asp:TemplateField>
                                            
            <asp:TemplateField HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Right">
                <HeaderTemplate>
                    <asp:Label ID="lblHAtComplDist" runat="server" Text="Distance @ Completion"
                        ToolTip="Accumulated distance traveled at time of action item completion. Empty or -9.99 means `unknown`. 0.0 (zero) is a legitimate number."></asp:Label>
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:Label ID="lblIAtComplDist" runat="server" Text='<%# Eval("dAtCompletionDist") %>'></asp:Label>
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:TextBox ID="txbEAtComplDist" runat="server" Text='<%# Eval("dAtCompletionDist") %>' />
                    <asp:RegularExpressionValidator runat="server" ControlToValidate="txbEAtComplDist" ValidationExpression="^((-9\.990{0,2})?|\+?((\d{1,8}(\.\d{0,4})?)|(\.\d{1,4})))?$"
                        ErrorMessage="Must be empty or -9.99, or >= 0, up to 12 digits, up to 4 decimal places" Display="Dynamic" ForeColor="Red" BorderStyle="Dotted" BorderColor="Red" BorderWidth="4px"/>
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
                    <asp:TextBox ID="txbDComments" runat="server" Text='<%# Eval("sComment") %>' Wrap="true" TextMode="MultiLine"></asp:TextBox>
                </EditItemTemplate>
            </asp:TemplateField>

            <asp:TemplateField HeaderStyle-HorizontalAlign="Center">
                <HeaderTemplate>
                    <asp:Label ID="lblHUpdStat" runat="server" Text="Update Status"></asp:Label>
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:Label ID="lblIUpdStat" runat="server" Text='<%# Eval("sUpdateStatus") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>

            <asp:TemplateField ItemStyle-HorizontalAlign="Center">
                <HeaderTemplate>
                    Ed
                </HeaderTemplate>
                <ItemTemplate>
<%--                    <asp:ImageButton ID="ipbEEdit" ImageUrl="~/i/BlueButton.jpg" runat="server" CommandName="Edit" OnClientClick="oktoSubmit = true;" />--%>
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:ImageButton ID="ipbEUpdate" ImageUrl="~/i/Update.jpg" runat="server" CommandName="Update" OnClientClick="oktoSubmit = true;" />
                    <asp:ImageButton ID="ipbECancel" ImageUrl="~/i/Cancel.jpg" runat="server" CommandName="Cancel" OnClientClick="oktoSubmit = true;" />
                </EditItemTemplate>
            </asp:TemplateField>

            <asp:TemplateField ItemStyle-HorizontalAlign="Center">
                <HeaderTemplate>
                    <asp:Label ID="lblHOptions" runat="server" Text="Options" ToolTip="Click on a button to see options for actions" />
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:Button ID="pbIOptions" runat="server" Text="Options" Font-Size="Smaller" OnClick="pbIOptions_Click" />
                </ItemTemplate>
            </asp:TemplateField>

<%--            <asp:TemplateField ItemStyle-HorizontalAlign="Center">
                <HeaderTemplate>
                    <asp:Label ID="lblHUpdate" runat="server" Text="Update" ToolTip="Click on a button to update one action item" />
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:Button ID="pbIUpdate" runat="server" Text="Update" Font-Size="Smaller" OnClick="pbIUpdate_Click" />
                </ItemTemplate>
            </asp:TemplateField>--%>

        </Columns>
    </asp:GridView>
    <style>
        table {
            border-collapse:separate;
        }
        td {
            padding: 0px 5px 0px 5px;
            border: 1px solid;
        }
    </style>
    <div>
        <%-- ModalPopupExtender for displaying details about an action item --%>
        <asp:LinkButton ID="LinkButton2" runat="server" Text="U" CssClass="displayNone" />
        <ajaxToolkit:ModalPopupExtender ID="ModPopExtDetails" runat="server" PopupDragHandleControlID="MPE_PanelDet"
            TargetControlID="LinkButton2" PopupControlID="MPE_PanelDet" BackgroundCssClass="background"
            RepositionMode="RepositionOnWindowResizeAndScroll" />
        <asp:Panel ID="MPE_PanelDet" runat="server" CssClass="popup" style="display:none;" HorizontalAlign="Center">
            <asp:Button runat="server" Text="Dismiss" />
            <p><h4>Details for Action Item with Internal ID <asp:Label ID="lblActItemID" runat="server" Text="??" /></h4></p>
            <asp:Table runat="server" GridLines="Both" Font-Size="X-Small" >
                <asp:TableHeaderRow BorderWidth="2">
                    <asp:TableHeaderCell CssClass="text-center">
                        &nbsp;
                    </asp:TableHeaderCell>
                    <asp:TableHeaderCell CssClass="text-center">
                        Name
                    </asp:TableHeaderCell>
                    <asp:TableHeaderCell CssClass="text-center">
                        From
                    </asp:TableHeaderCell>
                    <asp:TableHeaderCell CssClass="text-center">
                        To
                    </asp:TableHeaderCell>
                    <asp:TableHeaderCell CssClass="text-center">
                        Other Properties
                    </asp:TableHeaderCell>
                </asp:TableHeaderRow>
                <asp:TableRow>
                    <asp:TableCell HorizontalAlign="Right">
                        Equipment
                    </asp:TableCell>
                    <asp:TableCell>
                        <asp:Label ID="lblEquipName" runat="server" Text="EquipName" />
                    </asp:TableCell>
                    <asp:TableCell>
                        <asp:Label ID="lblEquipFrom" runat="server" Text="EquipFrom" />
                    </asp:TableCell>
                    <asp:TableCell>
                        <asp:Label ID="lblEquipTo" runat="server" Text="EquipTo" />
                    </asp:TableCell>
                    <asp:TableCell HorizontalAlign="Left">
                        <asp:Label ID="lblEquipProps" runat="server" Text="EquipProps" />
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow>
                    <asp:TableCell HorizontalAlign="Right">
                        Component
                    </asp:TableCell>
                    <asp:TableCell>
                        <asp:Label ID="lblComponName" runat="server" Text="ComponName" />
                    </asp:TableCell>
                    <asp:TableCell>
                        <asp:Label ID="lblComponFrom" runat="server" Text="ComponFrom" />
                    </asp:TableCell>
                    <asp:TableCell>
                        <asp:Label ID="lblComponTo" runat="server" Text="ComponTo" />
                    </asp:TableCell>
                    <asp:TableCell HorizontalAlign="Left">
                        <asp:Label ID="lblComponProps" runat="server" Text="ComponProps" />
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow>
                    <asp:TableCell HorizontalAlign="Right">
                        Aging Item
                    </asp:TableCell>
                    <asp:TableCell>
                        <asp:Label ID="lblAgingItemName" runat="server" Text="AgingItemName" />
                    </asp:TableCell>
                    <asp:TableCell>
                        <asp:Label ID="lblAgingItemFrom" runat="server" Text="AgingItemFrom" />
                    </asp:TableCell>
                    <asp:TableCell>
                        <asp:Label ID="lblAgingItemTo" runat="server" Text="AgingItemTo" />
                    </asp:TableCell>
                    <asp:TableCell HorizontalAlign="Left">
                        <asp:Label ID="lblAgingItemProps" runat="server" Text="AgingItemProps" />
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow>
                    <asp:TableCell HorizontalAlign="Right">
                        Aging Item
                    </asp:TableCell>
                    <asp:TableCell ColumnSpan="4">
                        <asp:Label ID="lblAgingItemProps2" runat="server" Text="AgingItemProps2" />
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow>
                    <asp:TableCell HorizontalAlign="Right">
                        Parameter Set
                    </asp:TableCell>
                    <asp:TableCell>
                        <asp:Label ID="lblParSetName" runat="server" Text="ParSetName" />
                    </asp:TableCell>
                    <asp:TableCell HorizontalAlign="Left" ColumnSpan="3">
                        <asp:Label ID="lblParSetProps" runat="server" Text="ParSetProps" />
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow>
                    <asp:TableCell HorizontalAlign="Right">
                        Operational Calendar
                    </asp:TableCell>
                    <asp:TableCell>
                        <asp:Label ID="lblOpsCalName" runat="server" Text="OpsCalName" />
                    </asp:TableCell>
                    <asp:TableCell>
                        <asp:Label ID="lblOpsCalFrom" runat="server" Text="OpsCalFrom" />
                    </asp:TableCell>
                    <asp:TableCell>
                        <asp:Label ID="lblOpsCalTo" runat="server" Text="OpsCalTo" />
                    </asp:TableCell>
                    <asp:TableCell HorizontalAlign="Left" ColumnSpan="3">
                        <asp:Label ID="lblOpsCalProps" runat="server" Text="OpsCalProps" />
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow>
                    <asp:TableCell HorizontalAlign="Right">
                        Manually entered Operational Data
                    </asp:TableCell>
                    <asp:TableCell>
                        
                    </asp:TableCell>
                    <asp:TableCell>
                        <asp:Label ID="lblManOpsFrom" runat="server" Text="ManOpsFrom" />
                    </asp:TableCell>
                    <asp:TableCell>
                        <asp:Label ID="lblManOpsTo" runat="server" Text="ManOpsTo" />
                    </asp:TableCell>
                    <asp:TableCell HorizontalAlign="Left">
                        <asp:Label ID="lblManOpsProps" runat="server" Text="ManOpsProps" />
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow>
                    <asp:TableCell HorizontalAlign="Right">
                        Flying Activities Data
                    </asp:TableCell>
                    <asp:TableCell>

                    </asp:TableCell>
                    <asp:TableCell>
                        <asp:Label ID="lblFlyOpsFrom" runat="server" Text="FlyOpsFrom" />
                    </asp:TableCell>
                    <asp:TableCell>
                        <asp:Label ID="lblFlyOpsTo" runat="server" Text="FlyOpsTo" />
                    </asp:TableCell>
                    <asp:TableCell HorizontalAlign="Left" ColumnSpan="3">
                        <asp:Label ID="lblFlyOpsProps" runat="server" Text="FlyOpsProps" />
                    </asp:TableCell>
                </asp:TableRow>
            </asp:Table><br />
            Action Item Properties
            <div class="divXSmallFont">Meaning of negative numbers (not as part of a date/time field): the value was not used in this action item deadline calculation.</div>
            <asp:Table runat="server" GridLines="Both" Font-Size="X-Small" >
                <asp:TableHeaderRow BorderWidth="2">
                    <asp:TableHeaderCell CssClass="text-center">
                        Variable Name
                    </asp:TableHeaderCell>
                    <asp:TableHeaderCell CssClass="text-center">
                        Variable Value
                    </asp:TableHeaderCell>
                    <asp:TableHeaderCell CssClass="text-center">
                        Explanation
                    </asp:TableHeaderCell>
                </asp:TableHeaderRow>
                <asp:TableRow>
                    <asp:TableCell HorizontalAlign="Right">
                        PiTRecordEntered
                    </asp:TableCell>
                    <asp:TableCell>
                        <asp:Label ID="lblPiTRecordEntered" runat="server" Text="PiTRecordEntered" />
                    </asp:TableCell>
                    <asp:TableCell>
                        Point in Time (GMT) when this record was created or last modified
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow>
                    <asp:TableCell HorizontalAlign="Right">
                        RecordEnteredBy
                    </asp:TableCell>
                    <asp:TableCell>
                        <asp:Label ID="lblRecordEnteredBy" runat="server" Text="RecordEnteredBy" />
                    </asp:TableCell>
                    <asp:TableCell>
                        Name of user who created or modified this action item last
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow>
                    <asp:TableCell HorizontalAlign="Right">
                        DLastAction
                    </asp:TableCell>
                    <asp:TableCell>
                        <asp:Label ID="lblDLastAction" runat="server" Text="DLastAction" />
                    </asp:TableCell>
                    <asp:TableCell>
                        The Date of Last Action on this action item
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow>
                    <asp:TableCell HorizontalAlign="Right">
                        sWhenceDLastAction
                    </asp:TableCell>
                    <asp:TableCell>
                        DLastAction controlled by:
                    </asp:TableCell>
                    <asp:TableCell>
                        <asp:Label ID="lblWhenceDLastAction" runat="server" Text="sWhenceDLastAction" />
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow>
                    <asp:TableCell HorizontalAlign="Right">
                        DeadLine 'Elapsed'
                    </asp:TableCell>
                    <asp:TableCell>
                        <asp:Label ID="lblDTOdeadlineElapsed" runat="server" Text="Deadline" />
                    </asp:TableCell>
                    <asp:TableCell>
                        (1) The deadline calculated due an elapsed time criterion; a 2999 date means 'not calculated'
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow>
                    <asp:TableCell HorizontalAlign="Right">
                        DeadLine 'Operating Hours/Running Time'
                    </asp:TableCell>
                    <asp:TableCell>
                        <asp:Label ID="lblDTOdeadlineHrs" runat="server" Text="Deadline" />
                    </asp:TableCell>
                    <asp:TableCell>
                        (2) The deadline calculated due an operating hours criterion; a 2999 date means 'not calculated'
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow>
                    <asp:TableCell HorizontalAlign="Right">
                        DeadLine 'Operating Cycles'
                    </asp:TableCell>
                    <asp:TableCell>
                        <asp:Label ID="lblDTOdeadlineCycles" runat="server" Text="Deadline" />
                    </asp:TableCell>
                    <asp:TableCell>
                        (3) The deadline calculated due an operating cycles criterion; a 2999 date means 'not calculated'
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow>
                    <asp:TableCell HorizontalAlign="Right">
                        DeadLine 'Distance traveled'
                    </asp:TableCell>
                    <asp:TableCell>
                        <asp:Label ID="lblDTOdeadlineDist" runat="server" Text="Deadline" />
                    </asp:TableCell>
                    <asp:TableCell>
                        (4) The deadline calculated due a distance traveled criterion; a 2999 date means 'not calculated'
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow>
                    <asp:TableCell HorizontalAlign="Right">
                        DeadLine 'Unique Event'
                    </asp:TableCell>
                    <asp:TableCell>
                        <asp:Label ID="lblDTOdeadlineUnique" runat="server" Text="Deadline" />
                    </asp:TableCell>
                    <asp:TableCell>
                        (5) The deadline calculated from the date of a unique event; a 2999 date means 'not calculated'
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow>
                    <asp:TableCell HorizontalAlign="Right">
                        DeadLine
                    </asp:TableCell>
                    <asp:TableCell>
                        <asp:Label ID="lblDeadLine" runat="server" Text="Deadline" />
                    </asp:TableCell>
                    <asp:TableCell>
                        The deadline calculated for this action item as the earliest of the above 5 deadlines
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow>
                    <asp:TableCell HorizontalAlign="Right">
                        dDeadlineHrs
                    </asp:TableCell>
                    <asp:TableCell>
                        <asp:Label ID="lblDeadlineHrs" runat="server" Text="DeadlineHrs" />
                    </asp:TableCell>
                    <asp:TableCell>
                        The number of operating hours (running time) since the last time this action item was performed that may have influenced the deadline calculation
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow>
                    <asp:TableCell HorizontalAlign="Right">
                        iDeadLineCycles
                    </asp:TableCell>
                    <asp:TableCell>
                        <asp:Label ID="lblDeadLineCycles" runat="server" Text="DeadLineCycles" />
                    </asp:TableCell>
                    <asp:TableCell>
                        The number of operating cycles since the last time this action item was performed that may have influenced the deadline calculation
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow>
                    <asp:TableCell HorizontalAlign="Right">
                        dDeadLineDist
                    </asp:TableCell>
                    <asp:TableCell>
                        <asp:Label ID="lblDeadLineDist" runat="server" Text="DeadLineDist" />
                    </asp:TableCell>
                    <asp:TableCell>
                        The distance traveled since the last time this action item was performed may have influenced the deadline calculation
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow>
                    <asp:TableCell HorizontalAlign="Right">
                        DScheduledStart
                    </asp:TableCell>
                    <asp:TableCell>
                        <asp:Label ID="lblScheduledStart" runat="server" Text="ScheduledStart" />
                    </asp:TableCell>
                    <asp:TableCell>
                        Point in Time when the work on this action item should start = DeadLine - dEstDuration
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow>
                    <asp:TableCell HorizontalAlign="Right">
                        DActualStart
                    </asp:TableCell>
                    <asp:TableCell>
                        <asp:Label ID="lblActualStart" runat="server" Text="ActualStart" />
                    </asp:TableCell>
                    <asp:TableCell>
                        Point in Time when the work on this action item was actually started [user input]. 2001/01/01 means 'unknown'.
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow>
                    <asp:TableCell HorizontalAlign="Right">
                        iPercentComplete
                    </asp:TableCell>
                    <asp:TableCell>
                        <asp:Label ID="lblPercentComplete" runat="server" Text="PercentComplete" />
                    </asp:TableCell>
                    <asp:TableCell>
                        Work completion percentage [user input]
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow>
                    <asp:TableCell HorizontalAlign="Right">
                        dAtCompletionHrs
                    </asp:TableCell>
                    <asp:TableCell>
                        <asp:Label ID="lblAtCompletionHrs" runat="server" Text="AtCompletionHrs" />
                    </asp:TableCell>
                    <asp:TableCell>
                        The number of operating hours (running time) at the time of completion of this action item [user input]. Value may be required in the next deadline calculation.
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow>
                    <asp:TableCell HorizontalAlign="Right">
                        iAtCompletionCycles
                    </asp:TableCell>
                    <asp:TableCell>
                        <asp:Label ID="lblAtCompletionCycles" runat="server" Text="AtCompletionCycles" />
                    </asp:TableCell>
                    <asp:TableCell>
                        The number of operating cycles at the time of completion of this action item [user input]. Value may be required in the next deadline calculation.
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow>
                    <asp:TableCell HorizontalAlign="Right">
                        dAtCompletionDist
                    </asp:TableCell>
                    <asp:TableCell>
                        <asp:Label ID="lblAtCompletionDist" runat="server" Text="AtCompletionDist" />
                    </asp:TableCell>
                    <asp:TableCell>
                        The distance traveled at the time of completion of this action item [user input]. Value may be required in the next deadline calculation.
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow>
                    <asp:TableCell HorizontalAlign="Right">
                        DComplete
                    </asp:TableCell>
                    <asp:TableCell>
                        <asp:Label ID="lblDComplete" runat="server" Text="DComplete" />
                    </asp:TableCell>
                    <asp:TableCell>
                        Point in Time when the work on this action item was completed (iPercentComplete=100), or is estimated to be completed (iPercentComplete<100) [user input]
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow>
                    <asp:TableCell HorizontalAlign="Right">
                        sComment
                    </asp:TableCell>
                    <asp:TableCell>
                        <asp:Label ID="lblComment" runat="server" Text="Comment" />
                    </asp:TableCell>
                    <asp:TableCell>
                        Any comment [user input]
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow>
                    <asp:TableCell HorizontalAlign="Right">
                        sUpdateStatus
                    </asp:TableCell>
                    <asp:TableCell>
                        <asp:Label ID="lblUpdateStatus" runat="server" Text="UpdateStatus" />
                    </asp:TableCell>
                    <asp:TableCell>
                        Status of the last update on this action item: a clue re how deadline was arrived at [generated by the software]
                    </asp:TableCell>
                </asp:TableRow>
            </asp:Table><br />
            <asp:Button runat="server" Text="Dismiss" />
        </asp:Panel>
    </div>

    <div>
        <%-- ModalPopupExtender, popping up MPE_Panel and dynamically populating lblPopupText --%>
        <asp:LinkButton ID="Target" runat="server" Text="T" CssClass="displayNone" />
        <ajaxToolkit:ModalPopupExtender ID="ModalPopExt" runat="server"
            TargetControlID="Target" PopupControlID="MPE_Panel" RepositionMode="RepositionOnWindowResizeAndScroll"
            BackgroundCssClass="background" />
        <asp:Panel ID="MPE_Panel" runat="server" CssClass="popup" style="display:none;" HorizontalAlign="Center">
            <asp:Label ID="lblPopupText" runat="server" BackColor="#eeb5a2" Font-Bold="true" />
            <br /><br />
            <p> <asp:Button ID="OkButton" runat="server" Text="OK" OnClick="Button_Click" />&nbsp;&nbsp;
                <asp:Button ID="CancelButton" runat="server" Text="Cancel" OnClick="Button_Click" />&nbsp;&nbsp;
                <asp:Button ID="NoButton" runat="server" Text="No" OnClick="Button_Click" />&nbsp;&nbsp;
                <asp:Button ID="YesButton" runat="server" Text="Yes" OnClick="Button_Click"/></p>
        </asp:Panel>
    </div>

    <div>
        <asp:LinkButton ID="LinkButton1" runat="server" Text="T" CssClass="displayNone" />
        <ajaxToolkit:ModalPopupExtender ID="ModPopExtOptionsMenu" runat="server"
            TargetControlID="LinkButton1" PopupControlID="MPE_PanelOM"
            BackgroundCssClass="background" />
        <asp:Panel ID="MPE_PanelOM" runat="server" CssClass="popup" style="display:none;" HorizontalAlign="Center">
            <p>For action item with ID <asp:Label ID="lblActID" runat="server" Text="0" />:</p>
            <p> <asp:Button ID="pbCharts" runat="server" Text="Charts" OnClick="pbOM_Click" ToolTip="Show any charts illustrating this action item." /></p>
            <p> <asp:Button ID="pbDetails" runat="server" Text="Details" OnClick="pbOM_Click" ToolTip="Show more details of the aging and action items' data without modifying." /></p>
            <p> <asp:Button ID="pbEdit" runat="server" Text="Edit" OnClick="pbOM_Click" ToolTip="Modify some of the data in this action item." /></p>
            <p> <asp:Button ID="pbRefresh" runat="server" Text="Update" OnClick="pbOM_Click" ToolTip="Run the Update procedure on the aging item with which this action item is associated." /></p>
            <p> <asp:Button ID="pbDelete" runat="server" Text="Delete" OnClick="pbOM_Click" ToolTip="Attempt to delete this action item." /></p>
            <p> <asp:Button ID="pbCancel" runat="server" Text="Cancel" OnClick="pbOM_Click" ToolTip="Do nothing; return to list of action items." /></p>
        </asp:Panel>
    </div>

    <div>
        <%-- ModalPopupExtender, popping up MPE_PanelCharts and dynamically populating lblPopTxtChartsx ... --%>
        <asp:LinkButton ID="lpb" runat="server" Text="T" CssClass="displayNone" />
        <ajaxToolkit:ModalPopupExtender ID="ModalPopExtCharts" runat="server" X="50" Y="50" PopupDragHandleControlID="MPE_PanelCharts"
            TargetControlID="lpb" PopupControlID="MPE_PanelCharts" RepositionMode="RepositionOnWindowResizeAndScroll"
            BackgroundCssClass="background" />
        <asp:Panel ID="MPE_PanelCharts" runat="server" CssClass="popup" style="display:none;" HorizontalAlign="Center">
            <asp:Label ID="lblNothing" runat="server" Text="Sorry, no charts are available to show here." Font-Bold="true" Font-Size="Larger" /><br />
            <asp:Button ID="pbNothing" runat="server" Text="Dismiss" />
            <ajaxToolkit:TabContainer ID="tbcCharts" runat="server" ActiveTabIndex="0" >
                <ajaxToolkit:TabPanel ID="tbpIllustr" runat="server">
                    <HeaderTemplate>
                        Action Item Illustration
                    </HeaderTemplate>
                    <ContentTemplate>
                        <asp:Image ID="imgIllustr" runat="server" />
                        <br /><br />
                        <div class="row">
                            <div class="col-sm-6"><asp:Button ID="pbDismissIll" runat="server" Text="Dismiss" /></div>
                            <div class="col-sm-6"><asp:Button ID="pbRemoveIll" runat="server" Text="Delete This Illustration" OnClick="pbRemove_Click" /></div>
                        </div>
                    </ContentTemplate>
                </ajaxToolkit:TabPanel>
                <ajaxToolkit:TabPanel ID="tbpChartsH" runat="server">
                    <HeaderTemplate>
                        Operating Hours
                    </HeaderTemplate>
                    <ContentTemplate>
                        <asp:Label ID="lblPopTxtChartsH" runat="server" BackColor="#eeb5a2" Font-Bold="true" />
                        <br />
                        <asp:Chart ID="chartExtrapH" runat="server" />
                        <br /><br />
                        <div class="row">
                            <div class="col-sm-6"><asp:Button ID="pbDismissH" runat="server" Text="Dismiss" /></div>
                            <div class="col-sm-6"><asp:Button ID="pbRemoveH" runat="server" Text="Delete This Chart" OnClick="pbRemove_Click" /></div>
                        </div>
                    </ContentTemplate>
                </ajaxToolkit:TabPanel>
                <ajaxToolkit:TabPanel ID="tbpChartsC" runat="server">
                    <HeaderTemplate>
                        Cycles
                    </HeaderTemplate>
                    <ContentTemplate>
                        <asp:Label ID="lblPopTxtChartsC" runat="server" BackColor="#eeb5a2" Font-Bold="true" />
                        <br />
                        <asp:Chart ID="chartExtrapC" runat="server" />
                        <br /><br />
                        <div class="row">
                            <div class="col-sm-6"><asp:Button ID="pbDismissC" runat="server" Text="Dismiss" /></div>
                            <div class="col-sm-6"><asp:Button ID="pbRemoveC" runat="server" Text="Delete This Chart" OnClick="pbRemove_Click" /></div>
                        </div>
                    </ContentTemplate>
                </ajaxToolkit:TabPanel>
                <ajaxToolkit:TabPanel ID="tbpChartsD" runat="server">
                    <HeaderTemplate>
                        Distance Traveled
                    </HeaderTemplate>
                    <ContentTemplate>
                        <asp:Label ID="lblPopTxtChartsD" runat="server" BackColor="#eeb5a2" Font-Bold="true" />
                        <br />
                        <asp:Chart ID="chartExtrapD" runat="server" />
                        <br /><br />
                        <div class="row">
                            <div class="col-sm-6"><asp:Button ID="pbDismissD" runat="server" Text="Dismiss" /></div>
                            <div class="col-sm-6"><asp:Button ID="pbRemoveD" runat="server" Text="Delete This Chart" OnClick="pbRemove_Click" /></div>
                        </div>
                    </ContentTemplate>
                </ajaxToolkit:TabPanel>
            </ajaxToolkit:TabContainer>
        </asp:Panel>
    </div>
</asp:Content>

<asp:Content ID="FooterContent" ContentPlaceHolderID="ContentPlaceHolderFooter" runat="server">
    <%: Title %>
</asp:Content>
