<%@ Page Title="Equipment Aging Operating Data" Language="C#" MasterPageFile="~/mTSoar.Master" AutoEventWireup="true"
    CodeBehind="EqAgingOperDat.aspx.cs" Inherits="TSoar.Equipment.EquipAging.EqAgingOperDat" %>

<asp:Content ID="HeaderContent" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="server">
    <asp:Label runat="server" Text="Equipment Aging - Underlying Operating Data" Font-Italic="true" />
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
        <div class="col-sm-2">Working with Operating Data</div>
        <div class="col-sm-2"><a href="EqAgingActionItems.aspx">Work with the Equipment Aging Register (List of Action Items)</a></div>
    </div>
    The operating data created and edited here exists for supplementing the flight operations data which can be viewed <a href="../../MemberPages/Stats/ClubStats.aspx">here</a>,
    and can be manipulated <a href="../../Statistician/Statistician.aspx">here</a>.
    <div class="divXSmallFont"><a href="EqAgingIntro.aspx">To control whether or not the time of day is included in start/from and end/to dates in Equipment Aging</a></div>
    <div class="gvclass">
    <asp:Table ID="tblOD" runat="server" BorderStyle="Solid" BorderColor="Brown" GridLines="Both" BorderWidth="3" CellPadding="5" CellSpacing="5" BackColor="WhiteSmoke">
        <asp:TableHeaderRow BorderStyle="Solid" BorderWidth="3">
            <asp:TableHeaderCell BorderStyle="Solid" BorderWidth="3" HorizontalAlign="Center">
                List of Equipment Components
            </asp:TableHeaderCell>
            <asp:TableHeaderCell BorderStyle="Solid" BorderWidth="3" HorizontalAlign="Center">
                <asp:Label ID="lblTHOC" runat="server" Text="Manually Entered Operational Data for the Selected Equipment Component" />
            </asp:TableHeaderCell>
        </asp:TableHeaderRow>
        <asp:TableRow BorderStyle="Solid" BorderWidth="3">
            <asp:TableCell BorderStyle="Solid" BorderWidth="3">
                <asp:GridView ID="gvEqComps" runat="server" AutoGenerateColumns="False"
                    GridLines="None" CssClass="SoarNPGridStyle"
                    OnRowDataBound="gvEqComps_RowDataBound" OnSelectedIndexChanged="gvEqComps_SelectedIndexChanged">
                    <Columns>
                        <asp:TemplateField HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" Visible="false">
                            <HeaderTemplate>
                                <asp:Label ID="lblHIdent" runat="server" Text="Internal Id" Width="60" 
                                    ToolTip="Points to a row in database table EQUIPCOMPONENTS with this ID field contents"></asp:Label>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblIIdent" runat="server" Text='<%# Eval("ID") %>' Width="60" ></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                            <HeaderTemplate>
                                <asp:Label ID="lblHSel" runat="server" Text="Select" 
                                    ToolTip="Click in this column on the row of the equipment component for which you want to have the manually entered operational data displayed on the right." />
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:ImageButton ID="ipbISel" ImageUrl="~/i/GreenButton.jpg" runat="server" CommandName="Select" />
                            </ItemTemplate>
                            <EditItemTemplate>
                                &nbsp;
                            </EditItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField>
                            <HeaderTemplate>
                                <asp:Label ID="lblHEquipName" runat="server" Text="Equipment" ToolTip="To which piece of equipment does this component belong?"></asp:Label>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lblEquipName" Text='<%# Bind("sShortEquipName") %>'/>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:DropDownList ID="DDLEquipName" runat="server" />
                            </EditItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderStyle-HorizontalAlign="Center" >
                            <HeaderTemplate>
                                <asp:Label ID="lblHComp" runat="server" Text="Component Name" ToolTip="Name of an Equipment Component" />
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lblIComp" Text='<%# Eval("sCName") %>'/>
<%--                                <asp:Label runat="server" ID="lblIComp" Text='<%# TSoar.Equipment.EquipAging.EqAgingItems.sExpandedComponentName((int)Eval("ID")) %>'/>--%>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:DropDownList ID="DDLCompName" runat="server" Width="200" DataValueField="ID" DataTextField="Component"/>
                            </EditItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField  HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Right">
                            <HeaderTemplate>
                                <asp:Label ID="lblHCount" runat="server" Text="Count" ToolTip="Number of operational data records" />
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblICount" runat="server" Text="0" />
                            </ItemTemplate>
                        </asp:TemplateField>

                    </Columns>
                </asp:GridView>
            </asp:TableCell>
            <asp:TableCell BorderStyle="Solid" BorderWidth="3">
                <asp:GridView ID="gvEqOperDat" runat="server" AutoGenerateColumns="False"
                    GridLines="None" CssClass="SoarNPGridStyle"
                    OnRowEditing="gvEqOperDat_RowEditing" OnRowCancelingEdit="gvEqOperDat_RowCancelingEdit" OnRowUpdating="gvEqOperDat_RowUpdating"
                    OnRowDataBound="gvEqOperDat_RowDataBound" OnRowDeleting="gvEqOperDat_RowDeleting">
                    <Columns>
                        <asp:TemplateField HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                            <HeaderTemplate>
                                <asp:Label ID="lblHIdent" runat="server" Text="Internal Id" Width="60" 
                                    ToolTip="Points to a row in database table EQUIPOPERDATA with this ID field contents"></asp:Label>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblIIdent" runat="server" Text='<%# Eval("ID") %>' Width="60" ></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField>
                            <HeaderTemplate>
                                <asp:Label ID="lblHDFromDate" runat="server" Text="From Date"
                                    ToolTip="Date of the start of the operational data time interval."></asp:Label>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lblIDFromDate" Text='<%# TSoar.CustFmt.sFmtDate(((DateTimeOffset)Eval("DFrom")),TSoar.CustFmt.enDFmt.DateOnly) %>'/>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txbEDFromDate" runat="server"
                                    Text='<%# TSoar.CustFmt.sFmtDate(((DateTimeOffset)Eval("DFrom")),TSoar.CustFmt.enDFmt.DateOnly).Replace("/","-") %>'
                                    TextMode="Date" Font-Size="X-Small"></asp:TextBox>
                            </EditItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField >
                            <HeaderTemplate>
                                <asp:Label ID="lblHDFromTime" runat="server" Text="From Time" 
                                    ToolTip="Time of day in hours and minutes of the start of the operational data time interval."></asp:Label>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lblIDFromTime" Text='<%# TSoar.CustFmt.sFmtDate(((DateTimeOffset)Eval("DFrom")),TSoar.CustFmt.enDFmt.TimeOnlyMin) %>'/>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txbDFromTime" runat="server" Text='<%# TSoar.CustFmt.sFmtDate(((DateTimeOffset)Eval("DFrom")),TSoar.CustFmt.enDFmt.TimeOnlyMin) %>'
                                    TextMode="Time"></asp:TextBox>
                            </EditItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField >
                            <HeaderTemplate>
                                <asp:Label ID="lblHDFromOffset" runat="server" Text="From Offset"
                                    ToolTip="Time offset in hourse and minutes to UTC of the start of the operational data time interval. Limited to +/-14:00."></asp:Label>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lblIDFromOffset" Text='<%# TSoar.CustFmt.sFmtDate(((DateTimeOffset)Eval("DFrom")),TSoar.CustFmt.enDFmt.DateAndTimeSec).Substring(20,6) %>' />
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txbDFromOffset" runat="server"
                                    Text='<%# TSoar.CustFmt.sFmtDate(((DateTimeOffset)Eval("DFrom")),TSoar.CustFmt.enDFmt.DateAndTimeSec).Substring(20,6) %>'
                                    TextMode="SingleLine" Width="55" ></asp:TextBox>
                                <asp:RegularExpressionValidator runat="server" ControlToValidate="txbDFromOffset" ValidationExpression="^[-+]((1[0-4])|(0\d)):[0-5]\d$"
                                    ErrorMessage="Must be a time offset from local to UTC like  -08:00  or  +03:30" Display="Dynamic" ForeColor="Red" BorderStyle="Dotted" BorderColor="Red" BorderWidth="4px"/>
                            </EditItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField>
                            <HeaderTemplate>
                                <asp:Label ID="lblHToDate" runat="server" Text="To Date"
                                    ToolTip="Date of the end of the time interval for which operating hours are given."></asp:Label>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lblIToDate" Text='<%# TSoar.CustFmt.sFmtDate(((DateTimeOffset)Eval("DTo")),TSoar.CustFmt.enDFmt.DateOnly) %>'/>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txbDToDate" runat="server"
                                    Text='<%# TSoar.CustFmt.sFmtDate(((DateTimeOffset)Eval("DTo")),TSoar.CustFmt.enDFmt.DateOnly).Replace("/","-") %>'
                                    TextMode="Date" Font-Size="X-Small"></asp:TextBox>
                            </EditItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField >
                            <HeaderTemplate>
                                <asp:Label ID="lblHToTime" runat="server" Text="To Time"
                                    ToolTip="Time of day in hours and minutes of end of operating hours time interval; it is usually of minor importance. Default is 1 hour and 1 minute before midnight."></asp:Label>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lblIToTime" Text='<%# TSoar.CustFmt.sFmtDate(((DateTimeOffset)Eval("DTo")),TSoar.CustFmt.enDFmt.TimeOnlyMin) %>' />
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txbDToTime" runat="server" Text='<%# TSoar.CustFmt.sFmtDate(((DateTimeOffset)Eval("DTo")),TSoar.CustFmt.enDFmt.TimeOnlyMin) %>' TextMode="Time"></asp:TextBox>
                            </EditItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField >
                            <HeaderTemplate>
                                <asp:Label ID="lblHToOffset" runat="server" Text="To Offset"
                                    ToolTip="Time offset in hours and minutes to UTC of end of operating hours time interval; almost always of very little importance.
            Default is the Setting called 'TimeZoneOffset' (-08:00 in the Pacific Standard Time Zone). A positive offset must start with a +-sign. Limited to +/-14:00."></asp:Label>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lblIToOffset" Text='<%# TSoar.CustFmt.sFmtDate(((DateTimeOffset)Eval("DTo")),TSoar.CustFmt.enDFmt.DateAndTimeSec).Substring(20,6) %>' />
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txbDToOffset" runat="server" Text='<%# TSoar.CustFmt.sFmtDate(((DateTimeOffset)Eval("DTo")),TSoar.CustFmt.enDFmt.DateAndTimeSec).Substring(20,6) %>' TextMode="SingleLine" ></asp:TextBox>
                                <asp:RegularExpressionValidator runat="server" ControlToValidate="txbDToOffset" ValidationExpression="^[-+]((1[0-4])|(0\d)):[0-5]\d$"
                                    ErrorMessage="Must be a time offset from local to UTC like -08:00" Display="Dynamic" ForeColor="Red" BorderStyle="Dotted" BorderColor="Red" BorderWidth="4px"/>
                            </EditItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField ItemStyle-HorizontalAlign="Center">
                            <HeaderTemplate>
                                <asp:Label ID="lblHStat" runat="server" Text="St"
                                    ToolTip="Sequencing Status: X = Overlap not allowed; O = OK; G = Gap > 3 hours" />
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblIStat" runat="server" Text="" Font-Bold="true" />
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Right" >
                            <HeaderTemplate>
                                <asp:Label ID="lblHdHours" runat="server" Text="Ops Hours"
                                    ToolTip="Number of operational hours or running time, with fractional hours." />
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lblIdHours" Text='<%# Eval("dHours") %>'/>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txbEdHours" runat="server" Text='<%# Eval("dHours") %>' Width="60"/>
                                <asp:RegularExpressionValidator runat="server" ControlToValidate="txbEdHours" ValidationExpression="^+?((\d{1,8}(\.\d{0,4})?)|(\.\d{1,4}))$"
                                    ErrorMessage="Must be an integer like '76', or a decimal like '+0.54'" Display="Dynamic" ForeColor="Red" BorderStyle="Dotted"
                                    BorderColor="Red" BorderWidth="4px" />
                                <asp:RequiredFieldValidator runat="server" ControlToValidate="txbEdHours" ErrorMessage="Operating hours must not be empty"
                                    Display="Dynamic" ForeColor="Red" BorderStyle="Dotted" BorderColor="Red" BorderWidth="4px" />
                            </EditItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" >
                            <HeaderTemplate>
                                <asp:Label ID="lblHCycl" runat="server" Text="Cycles"
                                    ToolTip="Number of operational cycles." />
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lblICycl" Text='<%# Eval("iCycles") %>'/>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txbECycl" runat="server" Text='<%# Eval("iCycles") %>' Width="60"/>
                                <asp:RegularExpressionValidator runat="server" ControlToValidate="txbECycl" ValidationExpression="^\d*$"
                                    ErrorMessage="Must be a positive integer" Display="Dynamic" ForeColor="Red" BorderStyle="Dotted"
                                    BorderColor="Red" BorderWidth="4px" />
                                <asp:RequiredFieldValidator runat="server" ControlToValidate="txbECycl" ErrorMessage="Number of Cycles must not be empty"
                                    Display="Dynamic" ForeColor="Red" BorderStyle="Dotted" BorderColor="Red" BorderWidth="4px" />
                            </EditItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Right">
                            <HeaderTemplate>
                                <asp:Label ID="lblHOpDist" runat="server" Text="Operating Distance"
                                    ToolTip="Operating distance as a positive decimal number with up to 10 digits before the decimal point, and up to 2 decimal places."></asp:Label>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblIOpDist" runat="server" Text='<%# Eval("dDistance") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txbDOpDist" runat="server" Text='<%# Eval("dDistance") %>' TextMode="SingleLine" Width="60"></asp:TextBox>
                                <asp:RegularExpressionValidator runat="server" ControlToValidate="txbDOpDist" ValidationExpression="^+?((\d{1,8}(\.\d{0,4})?)|(\.\d{1,4}))$"
                                    ErrorMessage="Must be an integer like '76', or a decimal like '+0.54'" Display="Dynamic" ForeColor="Red" BorderStyle="Dotted"
                                    BorderColor="Red" BorderWidth="4px" />
                                <asp:RequiredFieldValidator runat="server" ControlToValidate="txbDOpDist" ErrorMessage="Operating distance must not be empty"
                                    Display="Dynamic" ForeColor="Red" BorderStyle="Dotted" BorderColor="Red" BorderWidth="4px" />
                            </EditItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField ItemStyle-HorizontalAlign="Center">
                            <HeaderTemplate>
                                <asp:Label ID="lblHUnitsDist" runat="server" Text="Distance Units - Mileage" 
                                    ToolTip="Distance units: Miles or Kilometers; default is Miles."></asp:Label>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lblIUnitsDist" Text='<%# Eval("sDistanceUnits") %>' />
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:DropDownList ID="DDLDUnitsDist" runat="server" >
                                    <asp:ListItem Value="0" Text="Miles" Selected="True" />
                                    <asp:ListItem Value="1" Text="Km" />
                                </asp:DropDownList>
                            </EditItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center">
                            <HeaderTemplate>
                                <asp:Label ID="lblHSource" runat="server" Text="Source"
                                    ToolTip="Source of data: M=Manual entry; R=Reset to given value (From Date should equal To Date, including times of day and offsets)."></asp:Label>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lblIUnitsElapsed" Text='<%# ((char)Eval("cSource") == (char)82)? "Reset":((char)Eval("cSource") == (char)77)? "Manual":"Unknown" %>' />
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:DropDownList ID="DDLDSource" runat="server">
                                    <asp:ListItem Value="M" Text="Manual" Selected="True" />
                                    <asp:ListItem Value="R" Text="Reset" />
                                </asp:DropDownList>
                            </EditItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField ItemStyle-Width="120" HeaderStyle-HorizontalAlign="Center" >
                            <HeaderTemplate>
                                <asp:Label ID="lblHComment" runat="server" Text="Notes" />
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lbliComment" Text='<%# (Eval("sComment") is DBNull)? "" : Server.HtmlDecode((string)Eval("sComment")) %>'/>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txbEComment" runat="server" Text='<%# (Eval("sComment") is DBNull)? "" : Server.HtmlDecode((string)Eval("sComment")) %>'>
                                </asp:TextBox>
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
            </asp:TableCell>
        </asp:TableRow>
    </asp:Table>
    </div>

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
