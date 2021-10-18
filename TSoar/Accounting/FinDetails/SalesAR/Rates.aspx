<%@ Page Title="Rates" Language="C#" MasterPageFile="~/mTSoar.Master" AutoEventWireup="true" CodeBehind="Rates.aspx.cs" Inherits="TSoar.Accounting.FinDetails.SalesAR.Rates" %>

<asp:Content ID="HeaderContent" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="server">
    Monetary Charging Rates
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="ContentPlaceHolderMain" runat="server">
    <asp:SqlDataSource ID="SqlDataSrc_EquipTypes" runat="server" ConnectionString="<%$ ConnectionStrings:SqlConn %>"
        SelectCommand="SELECT ID,sEquipmentType FROM EQUIPTYPES ORDER BY sEquipmentType" />
    <asp:SqlDataSource ID="SqlDataSrc_LaunchMethods" runat="server" ConnectionString="<%$ ConnectionStrings:SqlConn %>"
        SelectCommand="SELECT ID,sLaunchMethod FROM LAUNCHMETHODS ORDER BY sLaunchMethod" />
    <asp:SqlDataSource ID="SqlDataSrc_AccItems" runat="server" ConnectionString="<%$ ConnectionStrings:SqlConn %>"
        SelectCommand="SELECT ID,sQBO_ItemName FROM QBO_ITEMNAMES ORDER BY sQBO_ItemName" />
    <asp:SqlDataSource ID="SqlDataSrc_FAItems" runat="server" ConnectionString="<%$ ConnectionStrings:SqlConn %>"
        SelectCommand="SELECT ID,sFA_ItemCode FROM FA_ITEMS ORDER BY sFA_ItemCode" />
    <asp:SqlDataSource ID="SqlDataSrc_FAPmtTerms" runat="server" ConnectionString="<%$ ConnectionStrings:SqlConn %>"
        SelectCommand="SELECT ID,iPmtTermsCode FROM FA_PMTTERMS ORDER BY iPmtTermsCode" />
    
    <div class="col-sm-6"><h3>Rates</h3></div><div class="col-sm-6">
    <asp:Button ID="pbHelp" runat="server" Text="Display Help" OnClick="pbHelp_Click" /></div>
    <div class="gvclass">
        <asp:GridView ID="gvRates" runat="server" AutoGenerateColumns="False"
            GridLines="None" CssClass="SoarNPGridStyle"
            OnRowDataBound="gvRates_RowDataBound"
            OnRowEditing="gvRates_RowEditing"
            OnRowDeleting="gvRates_RowDeleting" OnRowCancelingEdit="gvRates_RowCancelingEdit"
            OnRowUpdating="gvRates_RowUpdating"
            AllowPaging="false" PageSize="25" ShowHeaderWhenEmpty="true"
            Font-Size="Small">
            <PagerSettings Mode="NextPreviousFirstLast" Position="TopAndBottom" Visible="true" FirstPageText="&nbsp;First&nbsp;"
                LastPageText="&nbsp;Last&nbsp;" NextPageText="&nbsp;Next&nbsp;" PreviousPageText="&nbsp;Previous&nbsp;" />
            <PagerStyle CssClass="SoarNPpaging" />
            <Columns>
                <asp:TemplateField HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                    <HeaderTemplate>
                        <asp:Label ID="lblHIdent" runat="server" Text="Internal Id" Width="50" ToolTip="Points to a row in database table RATES with this ID field contents"></asp:Label>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label ID="lblIIdent" runat="server" Text='<%# Eval("ID") %>' Width="50" ></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderStyle-HorizontalAlign="Center">
                    <HeaderTemplate>
                        <asp:Label ID="lblHShortName" runat="server" Text="Short Name" Width="100"></asp:Label>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label ID="lblIShortName" runat="server" Text='<%# Eval("sShortName") %>' Width="100"></asp:Label>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:TextBox ID="txbDShortName" runat="server" Text='<%# Eval("sShortName") %>' Width="100" ></asp:TextBox>
                        <asp:RegularExpressionValidator runat="server" ValidationExpression="^.{0,30}$" ControlToValidate="txbDShortName"
                            ErrorMessage="At most 30 characters in Short Name" Display="Dynamic" Font-Bold="true" ForeColor="Red" Font-Size="Medium" />
                    </EditItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                    <HeaderTemplate>
                        <asp:Label ID="lblHDFrom" runat="server" Text="From Date" Width="130" ToolTip="Rates are valid from this date onwards"></asp:Label>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label ID="lblIDFrom" runat="server" Text='<%# TSoar.CustFmt.sFmtDate((DateTimeOffset)Eval("DFrom"),TSoar.CustFmt.enDFmt.DateOnly) %>' Width="130"></asp:Label>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:TextBox ID="txbDFrom" runat="server" Text='<%# TSoar.CustFmt.sFmtDate((DateTimeOffset)Eval("DFrom"),TSoar.CustFmt.enDFmt.DateOnly).Replace("/","-") %>' Width="130" TextMode="Date" ></asp:TextBox>
                    </EditItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                    <HeaderTemplate>
                        <asp:Label ID="lblHDTo" runat="server" Text="To Date" Width="130" ToolTip="Rates are valid up to this date"></asp:Label>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label ID="lblIDTo" runat="server" Text='<%# TSoar.CustFmt.sFmtDate((DateTimeOffset)Eval("DTo"),TSoar.CustFmt.enDFmt.DateOnly) %>' Width="130"></asp:Label>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:TextBox ID="txbDTo" runat="server" Text='<%# TSoar.CustFmt.sFmtDate((DateTimeOffset)Eval("DTo"),TSoar.CustFmt.enDFmt.DateOnly).Replace("/","-") %>' Width="130" TextMode="Date" ></asp:TextBox>
                    </EditItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField>
                    <HeaderTemplate>
                        <asp:Label ID="lblHEquipType" runat="server" Text="Equipment Type" Width="120"></asp:Label>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label ID="lblIEquipType" runat="server" Text='<%# dictEquipTypes[(int)Eval("iEquipType")] %>' Width="120"></asp:Label>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:DropDownList ID="DDLEquipType" runat="server" DataSourceID="SqlDataSrc_EquipTypes" DataTextField="sEquipmentType"
                            DataValueField="ID" Width="120" OnPreRender="DDL_PreRender" ></asp:DropDownList>
                    </EditItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField>
                    <HeaderTemplate>
                        <asp:Label ID="lblHLaunchMethod" runat="server" Text="Launch Method" Width="150"></asp:Label>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label ID="lblILaunchMethod" runat="server" Text='<%# dictLaunchMethods[(int)Eval("iLaunchMethod")]  %>' Width="150"></asp:Label>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:DropDownList ID="DDLLaunchMethod" runat="server" DataSourceID="SqlDataSrc_LaunchMethods" DataTextField="sLaunchMethod"
                            DataValueField="ID" Width="150" OnPreRender="DDL_PreRender" ></asp:DropDownList>
                    </EditItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField>
                    <HeaderTemplate>
                        <asp:Label ID="lblHChargeCodes" runat="server" Text="Applicable Charge Codes" Width="75"></asp:Label>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label ID="lblIChargeCodes" runat="server" Text='<%# s_ChargeCodes((int)Eval("ID"))  %>' Width="75"></asp:Label>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:Button ID="pbEChargeCodes" runat="server" Text='<%# s_ChargeCodes((int)Eval("ID"))  %>' Width="75" OnClick="pbEChargeCodes_Click"></asp:Button>
                    </EditItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Right">
                    <HeaderTemplate>
                        <asp:Label ID="lblHSingleDpUse" runat="server" Text="$/Single Use" Width="60" ToolTip="Amount charged for one use of this equipment type, with this launch method, and with this charge code"></asp:Label>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label ID="lblISingleDpUse" runat="server" Text='<%# Eval("mSingleDpUse") %>' Width="60"></asp:Label>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:TextBox ID="txbSingleDpUse" runat="server" Text='<%# Eval("mSingleDpUse") %>' Width="60" CssClass="rightAlign" ></asp:TextBox>
                        <asp:RegularExpressionValidator runat="server" ValidationExpression="^[+-]?\d*\.?\d*$" ControlToValidate="txbSingleDpUse"
                            ErrorMessage="Must be decimal number like -9.99 or 9" Display="Dynamic" Font-Bold="true" ForeColor="Red" Font-Size="Medium" />
                    </EditItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Right">
                    <HeaderTemplate>
                        <asp:Label ID="lblHNoChrg1stFt" runat="server" Text="No Charge for 1st x Feet <sup>(2)</sup>" Width="60" ToolTip="If positive: Regarding the single use charge, we start charging after the first x feet of altitude difference of the launch.
If negative: The single use charge applies if the altitude difference of the tow is less than or equal to the absolute value in this field."></asp:Label>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label ID="lblINoChrg1stFt" runat="server" Text='<%# Eval("iNoChrg1stFt") %>' Width="60"></asp:Label>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:TextBox ID="txbNoChrg1stFt" runat="server" Text='<%# Eval("iNoChrg1stFt") %>' TextMode="Number" Width="60" CssClass="rightAlign" ></asp:TextBox>
                    </EditItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Right">
                    <HeaderTemplate>
                        <asp:Label ID="lblHAltDiffDpFt" runat="server" Text="$/foot Altit. Diff." Width="60" ToolTip="Charge per foot of altitude difference of a launch"></asp:Label>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label ID="lblIAltDiffDpFt" runat="server" Text='<%# Eval("mAltDiffDpFt") %>' Width="60"></asp:Label>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:TextBox ID="txbAltDiffDpFt" runat="server" Text='<%# Eval("mAltDiffDpFt") %>' Width="60" CssClass="rightAlign" ></asp:TextBox>
                        <asp:RegularExpressionValidator runat="server" ValidationExpression="^[+-]?\d*\.?\d*$" ControlToValidate="txbAltDiffDpFt"
                            ErrorMessage="Must be decimal number like -9.99 or 9" Display="Dynamic" Font-Bold="true" ForeColor="Red" Font-Size="Medium" />
                    </EditItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Right">
                    <HeaderTemplate>
                        <asp:Label ID="lblHNoChrg1stMin" runat="server" Text="No Charge for 1st x minutes" Width="60" ToolTip="Considering the single use charge, we start charging per minute after the first so many minutes of equipment use."></asp:Label>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label ID="lblINoChrg1stMin" runat="server" Text='<%# Eval("iNoChrg1stMin") %>' Width="60"></asp:Label>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:TextBox ID="txbNoChrg1stMin" runat="server" Text='<%# Eval("iNoChrg1stMin") %>' TextMode="Number" Width="60" CssClass="rightAlign" ></asp:TextBox>
                    </EditItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Right">
                    <HeaderTemplate>
                        <asp:Label ID="lblHDurationDpMin" runat="server" Text="$/minute" Width="60" ToolTip="Charge per minute of equipment use"></asp:Label>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label ID="lblIDurationDpMin" runat="server" Text='<%# Eval("mDurationDpMin") %>' Width="60"></asp:Label>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:TextBox ID="txbDurationDpMin" runat="server" Text='<%# Eval("mDurationDpMin") %>' Width="60" CssClass="rightAlign" ></asp:TextBox>
                        <asp:RegularExpressionValidator runat="server" ValidationExpression="^[+-]?\d*\.?\d*$" ControlToValidate="txbDurationDpMin"
                            ErrorMessage="Must be decimal number like -9.99 or 9" Display="Dynamic" Font-Bold="true" ForeColor="Red" Font-Size="Medium" />
                    </EditItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Right">
                    <HeaderTemplate>
                        <asp:Label ID="lblHDurCapMin" runat="server" Text="Duration Cap minutes" Width="60" ToolTip="After this many minutes of glider rental there is no further charge. 0 means no cap."></asp:Label>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label ID="lblIDurCapMin" runat="server" Text='<%# Eval("iDurCapMin") %>' Width="60"></asp:Label>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:TextBox ID="txbDurCapMin" runat="server" Text='<%# Eval("iDurCapMin") %>' TextMode="Number" Width="60" CssClass="rightAlign" ></asp:TextBox>
                    </EditItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderStyle-HorizontalAlign="Center">
                    <HeaderTemplate>
                        <asp:Label ID="lblHComments" runat="server" Text="Comments" Width="120"></asp:Label>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label ID="lblIComments" runat="server" Text='<%# Eval("sComment") %>' Width="120"></asp:Label>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:TextBox ID="txbDComments" runat="server" Text='<%# Eval("sComment") %>' Width="120"></asp:TextBox>
                    </EditItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderStyle-HorizontalAlign="Center">
                    <HeaderTemplate>
                        <asp:Label ID="lblHFA_Item" runat="server" Text="Front Accounting Item"></asp:Label>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label ID="lblIFA_Item" runat="server" Text='<%# dictFAItems[(int)Eval("iFA_Item")] %>'></asp:Label>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:DropDownList ID="DDLFAItem" runat="server" DataSourceId="SqlDataSrc_FAItems" DataTextField="sFA_ItemCode"
                            DataValueField="ID" OnPreRender="DDL_PreRender"></asp:DropDownList>
                    </EditItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderStyle-HorizontalAlign="Center">
                    <HeaderTemplate>
                        <asp:Label ID="lblHFA_PmtTerm" runat="server" Text="Front Accounting Payment Term"></asp:Label>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label ID="lblIFA_PmtTerm" runat="server" Text='<%# dictFAPmtTerms[(int)Eval("iFA_PmtTerm")] %>'></asp:Label>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:DropDownList ID="DDLFAPmtTerm" runat="server" DataSourceId="SqlDataSrc_FAPmtTerms" DataTextField="iPmtTermsCode"
                            DataValueField="ID" OnPreRender="DDL_PreRender"></asp:DropDownList>
                    </EditItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField>
                    <HeaderTemplate>
                        <asp:Label ID="lblHAccItem" runat="server" Text="QBO Accounting Item"></asp:Label>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label ID="lblIAccItem" runat="server" Text='<%# dictAccItems[(int)Eval("iQBO_ItemName")] %>'></asp:Label>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:DropDownList ID="DDLAccItem" runat="server" DataSourceID="SqlDataSrc_AccItems" DataTextField="sQBO_ItemName"
                            DataValueField="ID" OnPreRender="DDL_PreRender" ></asp:DropDownList>
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
        <p>Note (1): Add the applicable charge codes for a new RATE record after adding/creating the record;
            click on its blue Edit button and then on the orange button in the Applicable Charge Codes column; a popup appears; add and delete charge codes as appropriate.
            After returning from that popup, click on the 'Update' button (in the Edit column).<br />
           Note (2): If zero or positive: Regarding the single use charge, we start charging after the first x feet of altitude difference of the launch.
            If negative: The single use charge applies if the altitude difference of the tow is less than or equal to the absolute value in this field.
        </p>
    </div>

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

    <div id="ModPopExtChargeCodes">
        <%-- ModalPopupExtender, popping up MPE_PnlChargeCodes --%>
        <asp:LinkButton ID="TargetCC" runat="server" Text="T" CssClass="displayNone" />
        <ajaxToolkit:ModalPopupExtender ID="ModPopExt_ChargeCodes" runat="server"
            TargetControlID="TargetCC" PopupControlID="MPE_PnlChargeCodes"
            BackgroundCssClass="background"
             />
        <asp:Panel ID="MPE_PnlChargeCodes" runat="server" CssClass="popup" style="display:none;" HorizontalAlign="Center" Width="480" Height="500">
            <asp:Label ID="lblPopChargeCodes" runat="server" BackColor="#eeb5a2" Font-Bold="true" Text="Add or remove charge codes applicable to this RATE record" />
            <br /><br />
            <asp:GridView ID="gvPopChargeCodes" runat="server" AutoGenerateColumns="False" GridLines="None" CssClass="SoarNPGridStyle"
                EmptyDataText="There are currently no charge codes assigned" ShowHeaderWhenEmpty="true"
                OnRowDeleting="gvPopChargeCodes_RowDeleting">
                <Columns>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            Internal ID
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:Label ID="lblIID" runat="server" Text='<%# Eval("ID") %>' />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            Description
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:Label ID="lblIsChargeCode" runat="server" Text='<%# Eval("CHARGECODE.sChargeCode") %>' />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            Code
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:Label ID="lblIcChargeCode" runat="server" Text='<%# Eval("CHARGECODE.cChargeCode") %>' />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            Used for Launch Charge?
                        </HeaderTemplate>
                        <ItemTemplate>
                            <%# Eval("CHARGECODE.bCharge4Launch") %>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            Used for Rental Charge?
                        </HeaderTemplate>
                        <ItemTemplate>
                            <%# Eval("CHARGECODE.bCharge4Rental") %>
                        </ItemTemplate>
                    </asp:TemplateField>
                <asp:CommandField ButtonType="Image" ShowDeleteButton="true" HeaderText="Delete" DeleteImageUrl="~/i/RedButton.jpg">
                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Height="25px" Width="50px" />
                </asp:CommandField>
                </Columns>
            </asp:GridView>
            <hr />
            <asp:DropDownList ID="DDLChargeCodes" runat="server" OnPreRender="DDLChargeCodes_PreRender" ></asp:DropDownList>
            <br />
            To Add the selected charge code: <asp:Button ID="pbMPECC_Add" runat="server" Text="Add" OnClick="pbMPECC_Add_Click" />
            <hr />
            <asp:Button ID="MPECC_OkButton" runat="server" Text="OK" OnClick="MPECC_Button_Click" />
        </asp:Panel>
    </div>

    <div id="ModPopExtHelp">
        <%-- ModalPopupExtender for displaying RATES Help --%>
        <asp:LinkButton ID="TargetHelp" runat="server" Text="T" CssClass="displayNone" />
        <ajaxToolkit:ModalPopupExtender ID="ModPopExt_Help" runat="server"
            TargetControlID="TargetHelp" PopupControlID="MPE_Pnl_Help"
            BackgroundCssClass="background"
             />
        <asp:Panel ID="MPE_Pnl_Help" runat="server" CssClass="popup" style="display:none;" HorizontalAlign="Center"
                Width="600" Height="800">
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
                            Charging Rates Help, Page 1
                        </Header>
                        <Content>
                <div class="leftAlign">
                <p>The amounts charged for flying activities are controlled by charging rates which are stored in records in table RATES.
                    Different rates apply for different kinds of activities. For example, the cost of an aero tow is different from a ground tow;
                    or, different gliders may have different rental rates depending on number of seats or age. Rates depend on the following variables:</p>
                <ul>
                    <li>Equipment Role</li>
                    <li>Equipment Type</li>
                    <li>Launch Method</li>
                    <li>Charge Code</li>
                    <li>Time Interval</li>
                </ul>

                <h4>Equipment Role</h4>
                <p>One equipment role is special, namely 'Glider'; all other equipment roles describe some kind of launch equipment (winch, car, tow plane, ...).
                    Charging rates do not explicitly depend on equipment role. It becomes important only when the software selects the applicable rate record
                    at the time a charge is computed: when the role is "Glider', a rate record for glider rental is selected; when the role is not 'Glider',
                    a rate record for appropriate towing equipment is selected. Different equipment roles are defined in table EQUIPMENTROLES and are managed by the web administrator.</p>

                <h4>Equipment Type</h4>
                <p>Different charging rates apply to different types of towing or launching equipment, such as 'Single Engine Propeller Aircraft', tow car 'Brown Goose',
                    or winch 'South Winch'. And different rental charges apply to different gliders, such as 'Glider Single Seat' versus 'Glider Two Seats'.
                    The equipment Types are defined in table EQUIPTYPES and are managed by the Web administrator.</p>
                </div>
                        </Content>
                    </ajaxToolkit:AccordionPane>
                    <ajaxToolkit:AccordionPane ID="AccPGenDat2" runat="server" >
                        <Header>
                            Charging Rates Help, Page 2
                        </Header>
                        <Content>
                <div class="leftAlign">

                <h4>Launch Method</h4>
                <p>Different charging rates apply to different launch methods; common launch methods are: self launch, aero tow by altitude, aero tow by duration, auto, winch.
                    They are defined in table LAUNCHMETHODS and managed by the web administrator.</p>

                <h4>Charge Codes</h4>
                <p>Different charging rates apply to different charge codes; any number of charge codes may apply to one rate record. Common charge codes are F (full charge),
                    T (charge for tow only, not for glider rental), G (charge for glider rental only, not for launch), E (free ride), C (introductory ride).
                    They are defined in table CHARGECODES and managed by the administrator. Specify which charge codes apply to one rate record after having created the record
                    (see Note(1) at the bottom of the Rates table); add as many as appropriate.</p>
                <p>Each charge code in table CHARGECODES has these two properties: 'Charge for Launch?' and 'Charge for Rental?'. They can only be true or false.
                    The charging rates in table RATES need not take this into account; for example, for charge code 'T' (tow only), 'Charge for Rental?' is set to false.
                    No rental charges will be generated regardless what the rate record may specify for rental charges.</p>

                <h4>Time Interval</h4>
                <h5>From Date</h5>
                <p>The charging rate record starts applying from this date forward.</p>

                <h5>To Date</h5>
                <p>The charging rate applies up to this date. Suggestion: Keep seemingly expired rate records for historical purposes.</p>
                </div>
                        </Content>
                    </ajaxToolkit:AccordionPane>
                    <ajaxToolkit:AccordionPane ID="AccPGenDat3" runat="server" >
                        <Header>
                            Charging Rates Help, Page 3
                        </Header>
                        <Content>
                <div class="leftAlign">

                <h4>$/Single Use</h4>
                <p>Amount charged for one use of this equipment type, with this launch method, and with this charge code. This is sometimes called a 'hook-up charge'.
                    Different hook-up charges may apply to different equipment types/roles and may exist side-by-side.</p>

                <h4>No Charge for 1st x Feet</h4>
                <p>If positive: The first x feet of altitude difference are included in the hook-up charge. We start charging by the foot after that.
                    If negative: intended for simulated rope breaks - the single use charge applies if the altitude difference of the tow is less than or
                    equal to the absolute value in this field. For example: the entry in this field is -500 ft; the altitude difference of a tow is only 400 ft.
                    The tow charge will be whatever is specified for $/Single Use in this rate record.
                </p>

                <h4>$/foot Altitude Difference</h4>
                <p>If charging by altitude difference, this is the applicable rate per foot. This number is zero when not charging by altitude difference.</p>

                <h4>No Charge for 1st x minutes</h4>
                <p>The first x minutes of equipment use/rental are included in the hook-up charge. We start charging by the minute after that. </p>

                <h4>$/minute</h4>
                <p>If charging by the minute, this is the applicable rate per minute. This number is zero when not charging by time used on equipment.</p>
                </div>
                        </Content>
                    </ajaxToolkit:AccordionPane>
                </Panes>            
            </ajaxToolkit:Accordion>
            <br /><br />
            <asp:Button ID="pbDone" runat="server" Text="Done" />
        </asp:Panel>
    </div>

</asp:Content>

<asp:Content ID="FooterContent" ContentPlaceHolderID="ContentPlaceHolderFooter" runat="server">
    <%: Title %>
</asp:Content>