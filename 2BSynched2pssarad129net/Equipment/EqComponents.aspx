<%@ Page Title="" Language="C#" MasterPageFile="~/mTSoar.Master" AutoEventWireup="true" CodeBehind="EqComponents.aspx.cs" Inherits="TSoar.Equipment.EqComponents" %>

<asp:Content ID="HeaderContent" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="server">
    <asp:Label runat="server" Text="Equipment and Maintenance" Font-Italic="true" />
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="ContentPlaceHolderMain" runat="server">
    <div class="row">
        <div class="col-sm-3">Links:</div>
        <div class="col-sm-3"><a href="EquipAging\EqAgingIntro.aspx">Introduction to / Help for Equipment Aging</a></div>
    </div>
    <div class="row">
        <div class="col-sm-2">Components of Equipment</div>
        <div class="col-sm-2"><a href="EquipAging\EqAgingParSets.aspx">Work with Aging Parameter Sets</a></div>
        <div class="col-sm-2"><a href="EquipAging\OpsCalendars.aspx">Work with Operational Calendars</a></div>
        <div class="col-sm-2"><a href="EquipAging\EqAgingItems.aspx">Work with Equipment Aging Items</a></div>
        <div class="col-sm-2"><a href="EquipAging\EqAgingOperDat.aspx">Work with Operating Data</a></div>
        <div class="col-sm-2"><a href="EquipAging\EqAgingActionItems.aspx">Work with the Equipment Aging Register (List of Action Items)</a></div>
    </div>
    <div class="divXSmallFont"><a href="EquipAging\EqAgingIntro.aspx">To control whether or not the time of day is included in start/from and end/to dates in Equipment Components</a></div>
    <h4>List of Components of Equipment</h4>
        <div class="gvclass">
        <asp:GridView ID="gvEqComp" runat="server" AutoGenerateColumns="False"
            GridLines="None" CssClass="SoarNPGridStyle"
            OnRowDataBound="gvEqComp_RowDataBound"
            OnRowEditing="gvEqComp_RowEditing"
            OnRowDeleting="gvEqComp_RowDeleting" OnRowCancelingEdit="gvEqComp_RowCancelingEdit"
            OnRowUpdating="gvEqComp_RowUpdating"
            OnPageIndexChanging="gvEqComp_PageIndexChanging"
            AllowPaging="true" PageSize="30" ShowHeaderWhenEmpty="true"
            Font-Size="Small">
            <PagerSettings Mode="NextPreviousFirstLast" Position="TopAndBottom" Visible="true" FirstPageText="&nbsp;First&nbsp;"
                LastPageText="&nbsp;Last&nbsp;" NextPageText="&nbsp;Next&nbsp;" PreviousPageText="&nbsp;Previous&nbsp;" />
            <PagerStyle CssClass="SoarNPpaging" />
            <Columns>
                <asp:TemplateField HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                    <HeaderTemplate>
                        <asp:Label ID="lblHIdent" runat="server" Text="Internal Id" Width="60" ToolTip="Points to a row in database table EQUIPCOMPONENTS with this ID field contents"></asp:Label>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label ID="lblIIdent" runat="server" Text='<%# Eval("ID") %>' Width="60" ></asp:Label>
                    </ItemTemplate>
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

                <asp:TemplateField HeaderStyle-HorizontalAlign="Center">
                    <HeaderTemplate>
                        <asp:Label ID="lblHComponent" runat="server" Text="Component"></asp:Label>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label ID="lblIComponent" runat="server" Text='<%# Eval("sCName") %>'></asp:Label>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:TextBox ID="txbDComponent" runat="server" Text='<%# Eval("sComponent") %>'></asp:TextBox>
                    </EditItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" >
                    <HeaderTemplate>
                        <asp:Label ID="lblHEntire" runat="server" Text="Entire?" Width="50"
                            ToolTip=" - CHECKED: This component represents the entire piece of equipment; parent component must be [none].
 - UNCHECKED: This is a subcomponent; parent component must NOT be [none].
This property cannot be edited; if you must change it, delete the entry and recreate it." />
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:CheckBox ID="chbIEntire" runat="server" Checked='<%# Eval("bEntire") %>' Enabled="false" />
                    </ItemTemplate>
<%-- Cannot change bEntire property ever; must delete entry and recreate.
                    <EditItemTemplate>
                        <asp:CheckBox ID="chbDEntire" runat="server" Checked='<%# Eval("bEntire") %>' Enabled="true" />
                    </EditItemTemplate>--%>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Linked to Parent Component">
                    <ItemTemplate>
                        <asp:Label runat="server" ID="lblParent" Text='<%# Eval("iParentComponent") %>'/>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:DropDownList ID="DDLParent" runat="server" />
                    </EditItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField >
                    <HeaderTemplate>
                        <asp:Label ID="lblHLinkBegin" runat="server" Text="Link Begin Date"
                            ToolTip="Begin date of link to parent component.
 For an 'entire' component, it is the date when operations began which is
 not necessarily the same as start of legal ownership of the piece of equipment (see List of Equipment).
 If < 1900/01/01: use the parent component's Link Begin date." />
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label runat="server" ID="lblILinkBegin" Text='<%# TSoar.Equipment.EqSupport.sComponentDFromTo((int)Eval("ID"), "From", (char)68) %>'/>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:textBox ID="txbELinkBegin" runat="server" text='<%# TSoar.Equipment.EqSupport.sComponentDFromTo((int)Eval("ID"), "From", (char)68).Replace("/","-") %>'
                            TextMode="Date"/>
                    </EditItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField >
                    <HeaderTemplate>
                        <asp:Label ID="lblHBeginTime" runat="server" Text="Link Begin Time" 
                            ToolTip="Time of day in hours and minutes of the start of the link to parent component time interval."></asp:Label>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label runat="server" ID="lblIBeginTime" Text='<%# TSoar.Equipment.EqSupport.sComponentDFromTo((int)Eval("ID"), "From", (char)84) %>'/>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:TextBox ID="txbEBeginTime" runat="server" Text='<%# TSoar.Equipment.EqSupport.sComponentDFromTo((int)Eval("ID"), "From", (char)84).Replace("/","-") %>'
                            TextMode="Time"></asp:TextBox>
                    </EditItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField >
                    <HeaderTemplate>
                        <asp:Label ID="lblHBeginOffset" runat="server" Text="Link Begin Offset"
                            ToolTip="Time offset in hours and minutes to UTC of the start of the link to parent component time interval. Limited to +/- 14:00."></asp:Label>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label runat="server" ID="lblIBeginOffset" Text='<%# TSoar.Equipment.EqSupport.sComponentDFromTo((int)Eval("ID"), "From", (char)79) %>' />
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:TextBox ID="txbEBeginOffset" runat="server"
                            Text='<%# TSoar.Equipment.EqSupport.sComponentDFromTo((int)Eval("ID"), "From", (char)79) %>'
                            TextMode="SingleLine" Width="55" ></asp:TextBox>
                        <asp:RegularExpressionValidator runat="server" ControlToValidate="txbEBeginOffset" ValidationExpression="^[-+]((1[0-4])|(0\d)):[0-5]\d$"
                            ErrorMessage="Must be a time offset from local to UTC like  -08:00  or  +03:30" Display="Dynamic" ForeColor="Red" BorderStyle="Dotted" BorderColor="Red" BorderWidth="4px"/>
                    </EditItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField >
                    <HeaderTemplate>
                        <asp:Label ID="lblHLinkEnd" runat="server" Text="Link End Date"
                            ToolTip="End date of link to parent component.
 For an 'entire' component, it is the date when operations ended.
 This date may be unknown - use a date far in the future.
 Not necessarily the same as end of legal ownership of the piece of equipment.
 if > 2999/12/31: use the parent component's Link End date." />
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label runat="server" ID="lblILinkEnd" Text='<%# TSoar.Equipment.EqSupport.sComponentDFromTo((int)Eval("ID"), "To", (char)68) %>'/>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:textBox ID="txbELinkEnd" runat="server" text='<%# TSoar.Equipment.EqSupport.sComponentDFromTo((int)Eval("ID"), "To", (char)68).Replace("/","-") %>' TextMode="Date"/>
                    </EditItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField >
                    <HeaderTemplate>
                        <asp:Label ID="lblHEndTime" runat="server" Text="Link End Time" 
                            ToolTip="Time of day in hours and minutes of the end of the link to parent component time interval."></asp:Label>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label runat="server" ID="lblIEndTime" Text='<%# TSoar.Equipment.EqSupport.sComponentDFromTo((int)Eval("ID"), "To", (char)84) %>'/>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:TextBox ID="txbEEndTime" runat="server" Text='<%# TSoar.Equipment.EqSupport.sComponentDFromTo((int)Eval("ID"), "To", (char)84).Replace("/","-") %>'
                            TextMode="Time"></asp:TextBox>
                    </EditItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField >
                    <HeaderTemplate>
                        <asp:Label ID="lblHEndOffset" runat="server" Text="Link End Offset"
                            ToolTip="Time offset in hourse and minutes to UTC of the end of the link to parent component time interval. Limited to +/- 14:00."></asp:Label>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label runat="server" ID="lblIEndOffset" Text='<%# TSoar.Equipment.EqSupport.sComponentDFromTo((int)Eval("ID"), "To", (char)79) %>' />
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:TextBox ID="txbEEndOffset" runat="server"
                            Text='<%# TSoar.Equipment.EqSupport.sComponentDFromTo((int)Eval("ID"), "To", (char)79) %>'
                            TextMode="SingleLine" Width="55" ></asp:TextBox>
                        <asp:RegularExpressionValidator runat="server" ControlToValidate="txbEEndOffset" ValidationExpression="^[-+]((1[0-4])|(0\d)):[0-5]\d$"
                            ErrorMessage="Must be a time offset from local to UTC like  -08:00  or  +03:30" Display="Dynamic" ForeColor="Red" BorderStyle="Dotted" BorderColor="Red" BorderWidth="4px"/>
                    </EditItemTemplate>
                </asp:TemplateField>
<%--                // SCR 231 start --%>
                <asp:TemplateField HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" >
                    <HeaderTemplate>
                        <asp:Label ID="lblHReportOpSt" runat="server" Text="Report OpSt?"
                            ToolTip="Should the operational status of this component be reported to Club members
in the compact list of equipment operational status?"></asp:Label>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:CheckBox ID="chbIReportOpSt" runat="server" Checked='<%# (bool)Eval("bReportOperStatus") %>' Enabled="false"></asp:CheckBox>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:CheckBox ID="chbEReportOpSt" runat="server" Checked='<%# (bool)Eval("bReportOperStatus") %>'></asp:CheckBox>
                    </EditItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderStyle-HorizontalAlign="Center">
                    <HeaderTemplate>
                        <asp:Label ID="lblHOpSt" runat="server" Text="Operational Status"></asp:Label>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label ID="lblIOpSt" runat="server" Text='<%# Eval("sOperStatus") %>'></asp:Label>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:TextBox ID="txbEOpSt" runat="server" Text='<%# Eval("sOperStatus") %>'></asp:TextBox>
                    </EditItemTemplate>
                </asp:TemplateField>
<%--               // SCR 231 end --%>
                <asp:TemplateField HeaderStyle-HorizontalAlign="Center">
                    <HeaderTemplate>
                        <asp:Label ID="lblHComments" runat="server" Text="Comments"></asp:Label>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label ID="lblIComments" runat="server" Text='<%# Eval("sComment") %>'></asp:Label>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:TextBox ID="txbDComments" runat="server" Text='<%# Eval("sComment") %>'></asp:TextBox>
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
