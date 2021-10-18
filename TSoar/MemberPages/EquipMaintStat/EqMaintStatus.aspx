<%@ Page Title="Equipment Maintenance Status" Language="C#" MasterPageFile="~/mTSoar.Master" AutoEventWireup="true"
    CodeBehind="EqMaintStatus.aspx.cs" Inherits="TSoar.MemberPages.EquipMaintStat.EqMaintStatus" %>

<asp:Content ID="ContentHeader" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="server">
    <asp:Label runat="server" Text="Display the Status of Equipment Maintenance Actions" Font-Italic="true" />
</asp:Content>

<asp:Content ID="ContentMain" ContentPlaceHolderID="ContentPlaceHolderMain" runat="server">
    <style>
        th[scope=col]{
            text-align:center;
        }
        .localfont {font-size:medium;}
        .localdarkred {color: #c82600;}
        .localorangered {color:orangered;}
        .localblack {color:black;}
        .localblue {color:blue;}
    </style>
    ﻿    <asp:SqlDataSource ID="SqlDataSrc_EqMaintStatus" runat="server" ConnectionString="<%$ ConnectionStrings:SqlConn %>"
        SelectCommand="SELECT * FROM TNPV_EqMaintStatus ORDER BY iEquipment,[Equipment Component],[Aging Item],[Completion Date]" >
    </asp:SqlDataSource>
    <div class="localfont">Color Legend: <span class="localdarkred">Deadline more than a week in the future (action not started)</span>,
        <span class="localorangered">Deadline in the past or less than a week in the future (action not started)</span>,
        <span class="localblack">Action started but not yet completed</span>,
        <span class="localblue">Action completed</span>.
    </div>
    <asp:GridView ID="gvEqMSt" runat="server" AutoGenerateColumns="False"
            GridLines="None" CssClass="SoarNPGridStyle" Font-Size="Small"
            DataSourceID="SqlDataSrc_EqMaintStatus" OnRowCommand="gvEqMSt_RowCommand"
            OnRowDataBound="gvEqMSt_RowDataBound" EmptyDataText="No equipment maintenance/aging action items have been calculated yet."
            OnPageIndexChanging="gvEqMSt_PageIndexChanging" AllowPaging="true" PageSize="25">
        <PagerSettings Mode="NextPreviousFirstLast" Position="TopAndBottom" Visible="true" FirstPageText="&nbsp;First&nbsp;"
            LastPageText="&nbsp;Last&nbsp;" NextPageText="&nbsp;Next&nbsp;" PreviousPageText="&nbsp;Previous&nbsp;" />
        <PagerStyle CssClass="SoarNPpaging" />
        <Columns>
            <asp:TemplateField>
                <HeaderTemplate>
                    <asp:Label runat="server" Text="Internal ID" ToolTip="Action Item identifier in database table EQUIPACTIONITEMS" />
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:Label ID="lblIID" runat="server" Text='<%# Eval("ActionItem ID") %>' />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <HeaderTemplate>
                    <asp:Label runat="server" Text="Equipment / Component" ToolTip="Equipment Component name together with names of component parents, grandparents etc." />
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:Label ID="lblIComp" runat="server" Text='<%# TSoar.Equipment.EqSupport.sExpandedComponentName((int)Eval("iEquipComponent")) %>' />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <HeaderTemplate>
                    <asp:Label runat="server" Text="Aging/Maint. Item" ToolTip="An Aging Item describes an equipment maintenance action for a component." />
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:Label runat="server" Text='<%# Eval("Aging Item") %>' />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:CommandField ButtonType="Button" ShowSelectButton="true" HeaderText="Details" SelectText="Details" />
            <asp:TemplateField>
                <HeaderTemplate>
                    <asp:Label runat="server" Text="Last Updated" ToolTip="When the deadline calculation for this action item was last carried out." />
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:Label runat="server" Text='<%# Eval("Last Updated") %>' />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField ControlStyle-Font-Size="Medium">
                <HeaderTemplate>
                    <asp:Label runat="server" Text="Deadline" ToolTip=" By what date this action is supposed to be completed" />
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:Label runat="server" Text='<%# Eval("Deadline") %>' Font-Bold="true" />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <HeaderTemplate>
                    <asp:Label runat="server" Text="Actual Start" ToolTip="When the work for this action item was started; `% complete` is supposed to be > 0." />
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:Label runat="server" Text='<%# Eval("Actual Start") %>' />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <HeaderTemplate>
                    <asp:Label runat="server" Text="% Complete" ToolTip="A number between 0 and 100 indicating what percentage of the time on the calendar has elapsed
 in order to complete the work." />
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:Label runat="server" Text='<%# Eval("% Complete") %>' />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <HeaderTemplate>
                    <asp:Label runat="server" Text="Completion Date" ToolTip=" When % Complete = 0: completion date is meaningless.
 When 0 < % Complete < 100: completion date is an estimate of when the action item work will be done.
 When % Complete = 100: the actual completion date." />
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:Label runat="server" Text='<%# Eval("Completion Date") %>' />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="Comment" ReadOnly="true" HeaderText="Comment" />
        </Columns>
    </asp:GridView>

    <style>
        table {
            border-collapse:separate;
        }
        td {
            padding: 1px;
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
            <ajaxToolkit:TabContainer ID="tbcCharts" runat="server" ActiveTabIndex="0" >
                <ajaxToolkit:TabPanel ID="tbpDetTables" runat="server">
                    <HeaderTemplate>
                        Textual Table
                    </HeaderTemplate>
                    <ContentTemplate>
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
                                    Point in Time when this record was created or last modified
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
                                    <b>DeadLine</b>
                                </asp:TableCell>
                                <asp:TableCell>
                                    <asp:Label ID="lblDeadLine" runat="server" Text="Deadline" Font-Bold="true" />
                                </asp:TableCell>
                                <asp:TableCell>
                                    The deadline calculated for this action item
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
                        </ContentTemplate>
                </ajaxToolkit:TabPanel>
                <ajaxToolkit:TabPanel ID="tbpIllustr" runat="server">
                    <HeaderTemplate>
                        Action Item Illustration
                    </HeaderTemplate>
                    <ContentTemplate>
                        <asp:Image ID="imgIllustr" runat="server" />
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
                    </ContentTemplate>
                </ajaxToolkit:TabPanel>
            </ajaxToolkit:TabContainer>
            <asp:Button runat="server" Text="Dismiss" />
        </asp:Panel>
    </div>

</asp:Content>

<asp:Content ID="ContentFooter" ContentPlaceHolderID="ContentPlaceHolderFooter" runat="server">
    <%: Title %>
</asp:Content>
