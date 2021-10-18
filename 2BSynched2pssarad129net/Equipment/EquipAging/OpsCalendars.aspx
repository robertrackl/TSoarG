<%@ Page Title="OpsCalendars" Language="C#" MasterPageFile="~/mTSoar.Master" AutoEventWireup="true" CodeBehind="OpsCalendars.aspx.cs" Inherits="TSoar.Equipment.OpsCalendars" %>

<asp:Content ID="HeaderContent" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="server">
    <asp:Label runat="server" Text="Operational Calendars" Font-Italic="true" />
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="ContentPlaceHolderMain" runat="server">
    <div class="row">
        <div class="col-sm-3">Links:</div>
        <div class="col-sm-3"><a href="EqAgingIntro.aspx">Introduction to / Help for Equipment Aging</a></div>
    </div>
    <div class="row">
        <div class="col-sm-2"><a href="../EqComponents.aspx">Work with Equipment Components</a></div>
        <div class="col-sm-2"><a href="EqAgingParSets.aspx">Work with Aging Parameter Sets</a></div>
        <div class="col-sm-2">Working with Operational Calendars</div>
        <div class="col-sm-2"><a href="EqAgingItems.aspx">Work with Equipment Aging Items</a></div>
        <div class="col-sm-2"><a href="EqAgingOperDat.aspx">Work with Operating Data</a></div>
        <div class="col-sm-2"><a href="EqAgingActionItems.aspx">Work with the Equipment Aging Register (List of Action Items)</a></div>
    </div>
    <div class="gvclass">
    <asp:Table ID="tblOC" runat="server" BorderStyle="Solid" BorderColor="Brown" GridLines="Both" BorderWidth="3" CellPadding="5" CellSpacing="5" BackColor="WhiteSmoke">
        <asp:TableHeaderRow BorderStyle="Solid" BorderWidth="3">
            <asp:TableHeaderCell BorderStyle="Solid" BorderWidth="3" HorizontalAlign="Center">
                List of Names of Operational Calendars
            </asp:TableHeaderCell>
            <asp:TableHeaderCell BorderStyle="Solid" BorderWidth="3" HorizontalAlign="Center">
                <asp:Label ID="lblTHOC" runat="server" Text="Contents of Selected Operational Calendar" />
            </asp:TableHeaderCell>
        </asp:TableHeaderRow>
        <asp:TableRow BorderStyle="Solid" BorderWidth="3">
            <asp:TableCell BorderStyle="Solid" BorderWidth="3">
                <asp:GridView ID="gvOpsCalNames" runat="server" AutoGenerateColumns="False"
                    GridLines="None" CssClass="SoarNPGridStyle"
                    OnRowEditing="gvOpsCalNames_RowEditing" OnRowDataBound="gvOpsCalNames_RowDataBound"
                    OnRowDeleting="gvOpsCalNames_RowDeleting" OnRowUpdating="gvOpsCalNames_RowUpdating"
                    OnRowCancelingEdit="gvOpsCalNames_RowCancelingEdit" OnSelectedIndexChanged="gvOpsCalNames_SelectedIndexChanged">
                    <Columns>
                        <asp:TemplateField HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                            <HeaderTemplate>
                                <asp:Label ID="lblHSel" runat="server" Text="Select" 
                                    ToolTip="Click in this column on the operational calendar for which you want to have the calendar details displayed on the right" />
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:ImageButton ID="ipbISel" ImageUrl="~/i/GreenButton.jpg" runat="server" CommandName="Select" />
                            </ItemTemplate>
                            <EditItemTemplate>
                                &nbsp;
                            </EditItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                            <HeaderTemplate>
                                <asp:Label ID="lblHIdent" runat="server" Text="Internal Id" Width="60" 
                                    ToolTip="Points to a row in database table OPSCALNAMES with this ID field contents"></asp:Label>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblIIdent" runat="server" Text='<%# Eval("ID") %>' Width="60" ></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField ItemStyle-Width="120" HeaderStyle-HorizontalAlign="Center" >
                            <HeaderTemplate>
                                <asp:Label ID="lblHOCName" runat="server" Text="Ops Calendar Name" ToolTip="Name of Operational Calendar" />
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lblOCName" Text='<%# Eval("sOpsCalName") %>'/>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txbOCName" runat="server" Text='<%# Eval("sOpsCalName") %>'>
                                </asp:TextBox>
                            </EditItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" >
                            <HeaderTemplate>
                                <asp:Label ID="lblHStd" runat="server" Text="Standard?"
                                    ToolTip="Is this a 'standard' operational calendar? If yes: appears as default wherever a selection of an operational calendar is required." />
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:CheckBox runat="server" ID="chbIStd" Checked='<%# (bool)Eval("bStandard") %>' Enabled="false" />
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:CheckBox ID="chbEStd" runat="server" Checked='<%# (bool)Eval("bStandard") %>' />
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
            <asp:TableCell BorderStyle="Solid" BorderWidth="3">
                <asp:GridView ID="gvOpsCalTimes" runat="server" AutoGenerateColumns="False"
                    GridLines="None" CssClass="SoarNPGridStyle"
                    OnRowEditing="gvOpsCalTimes_RowEditing" OnRowDataBound="gvOpsCalTimes_RowDataBound"
                    OnRowDeleting="gvOpsCalTimes_RowDeleting" OnRowUpdating="gvOpsCalTimes_RowUpdating"
                    OnRowCancelingEdit="gvOpsCalTimes_RowCancelingEdit">
                    <Columns>
                        <asp:TemplateField HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                            <HeaderTemplate>
                                <asp:Label ID="lblHIdent" runat="server" Text="Internal Id" Width="60" 
                                    ToolTip="Points to a row in database table OPSCALTIMES with this ID field contents"></asp:Label>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblIIdent" runat="server" Text='<%# Eval("ID") %>' Width="60" ></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField ItemStyle-Width="135" >
                            <HeaderTemplate>
                                <asp:Label ID="lblHFromD" runat="server" Text="From Date" Width="115" 
                                    ToolTip="Date of the start of the operational calendar time interval."></asp:Label>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lblIFromD" Text='<%# TSoar.CustFmt.sFmtDate(((DateTimeOffset)Eval("DStart")),TSoar.CustFmt.enDFmt.DateOnly) %>' Width="125"/>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txbDFromDate" runat="server"
                                    Text='<%# TSoar.CustFmt.sFmtDate(((DateTimeOffset)Eval("DStart")),TSoar.CustFmt.enDFmt.DateOnly).Replace("/","-") %>'
                                    TextMode="Date" Font-Size="X-Small" Width="115"></asp:TextBox>
                            </EditItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField >
                            <HeaderTemplate>
                                <asp:Label ID="lblHFromTime" runat="server" Text="From Time" 
                                    ToolTip="Time of day in hours and minutes of the start of the operational calendar time interval.
The time of day can become significant if the difference in start times between adjacent operational calendar time intervals is small.
You probably want the time of day of a non-operational interval near the end of the day in order to include that day in the immediately preceding operational interval."></asp:Label> <%-- SCR 214 --%>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lblIFromTime" Text='<%# TSoar.CustFmt.sFmtDate(((DateTimeOffset)Eval("DStart")),TSoar.CustFmt.enDFmt.TimeOnlyMin) %>'/>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txbDFromTime" runat="server" Text='<%# TSoar.CustFmt.sFmtDate(((DateTimeOffset)Eval("DStart")),TSoar.CustFmt.enDFmt.TimeOnlyMin) %>'
                                    TextMode="Time"></asp:TextBox>
                            </EditItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField >
                            <HeaderTemplate>
                                <asp:Label ID="lblHFromOffset" runat="server" Text="From Offset"
                                    ToolTip="Time offset in hourse and minutes to UTC of the start of the operational calendar time interval. Limited to +/-14:00."></asp:Label>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lblIFromOffset" Text='<%# TSoar.CustFmt.sFmtDate(((DateTimeOffset)Eval("DStart")),TSoar.CustFmt.enDFmt.DateAndTimeSec).Substring(20,6) %>' />
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txbDFromOffset" runat="server"
                                    Text='<%# TSoar.CustFmt.sFmtDate(((DateTimeOffset)Eval("DStart")),TSoar.CustFmt.enDFmt.DateAndTimeSec).Substring(20,6) %>'
                                    TextMode="SingleLine" Width="55" ></asp:TextBox>
                                <asp:RegularExpressionValidator runat="server" ControlToValidate="txbDFromOffset" ValidationExpression="^[-+]((1[0-4])|(0\d)):[0-5]\d$"
                                    ErrorMessage="Must be a time offset from local to UTC like  -08:00  or  +03:30" Display="Dynamic" ForeColor="Red" BorderStyle="Dotted" BorderColor="Red" BorderWidth="4px"/>
                            </EditItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" >
                            <HeaderTemplate>
                                <asp:Label ID="lblHStat" runat="server" Text="Ops Status"
                                    ToolTip="Operational status of this calendar segment. True or Checked: Operational. False or Unchecked: Non-Operational." />
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lblIOpSt" Text='<%# ((bool)Eval("bOpStatus"))? "Operational" : "Non-Operational" %>'/>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:CheckBox ID="chbEOpSt" runat="server" Checked='<%# Eval("bOpStatus") %>' ToolTip="Checked = Operational; unchecked = Non-Operational" />
                            </EditItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField ItemStyle-Width="120" HeaderStyle-HorizontalAlign="Center" >
                            <HeaderTemplate>
                                <asp:Label ID="lblHComment" runat="server" Text="Notes" />
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lbliComment" Text='<%# Eval("sComment") %>'/>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txbEComment" runat="server" Text='<%# Eval("sComment") %>'>
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
