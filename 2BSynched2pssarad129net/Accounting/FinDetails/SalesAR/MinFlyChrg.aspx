<%@ Page Title="Actual/Minimum Flying Charges" Language="C#" MasterPageFile="~/mTSoar.Master" AutoEventWireup="true" CodeBehind="MinFlyChrg.aspx.cs"
    Inherits="TSoar.Accounting.FinDetails.SalesAR.MinFlyChrg" %>

<asp:Content ID="HeaderContent" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="server">
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="ContentPlaceHolderMain" runat="server">
    <asp:SqlDataSource ID="SqlDataSrc_MembCateg" runat="server" ConnectionString="<%$ ConnectionStrings:SqlConn %>"
        SelectCommand="SELECT ID,sMembershipCategory FROM MEMBERSHIPCATEGORIES ORDER BY sMembershipCategory" />
    <asp:SqlDataSource ID="SqlDataSrc_Person" runat="server"  ConnectionString="<%$ ConnectionStrings:SqlConn %>"
        SelectCommand="SELECT ID,sDisplayName FROM PEOPLE ORDER BY sDisplayName" />
    <asp:SqlDataSource ID="SqlDataSrc_MembCategFCE" runat="server" ConnectionString="<%$ ConnectionStrings:SqlConn %>"
        SelectCommand="SELECT ID,sMembershipCategory FROM MEMBERSHIPCATEGORIES ORDER BY sMembershipCategory" />
    
    <ajaxToolkit:TabContainer ID="TabC_MFC" runat="server">
        <ajaxToolkit:TabPanel runat="server" HeaderText="Edit MFC Parameters">
            <ContentTemplate>
                <div class="row">
                    <div class="col-sm-6">
                        <h4>Minimum Monthly Flying Charges - Parameters</h4>
                    </div>
                    <div class="col-sm-6">
                        <asp:Button ID="pbHelp" runat="server" Text="Display Help" OnClick="pbHelp_Click" />
                    </div>
                </div>
                <div class="gvclass">
                    <asp:GridView ID="gvMFCPars" runat="server" AutoGenerateColumns="False"
                        GridLines="None" CssClass="SoarNPGridStyle"
                        OnRowDataBound="gvMFCPars_RowDataBound"
                        OnRowEditing="gvMFCPars_RowEditing"
                        OnRowDeleting="gvMFCPars_RowDeleting" OnRowCancelingEdit="gvMFCPars_RowCancelingEdit"
                        OnRowUpdating="gvMFCPars_RowUpdating"
                        AllowPaging="false" PageSize="5" ShowHeaderWhenEmpty="true"
                        Font-Size="Small">
                        <PagerSettings Mode="NextPreviousFirstLast" Position="TopAndBottom" Visible="true" FirstPageText="&nbsp;First&nbsp;"
                            LastPageText="&nbsp;Last&nbsp;" NextPageText="&nbsp;Next&nbsp;" PreviousPageText="&nbsp;Previous&nbsp;" />
                        <PagerStyle CssClass="SoarNPpaging" />
                        <Columns>
                            <asp:TemplateField HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                                <HeaderTemplate>
                                    <asp:Label ID="lblHIdent" runat="server" Text="Internal Id" Width="60" ToolTip="Points to a row in database table MINFLYCHGPARS with this ID field contents"></asp:Label>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="lblIIdent" runat="server" Text='<%# Eval("ID") %>' Width="60" ></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                                <HeaderTemplate>
                                    <asp:Label ID="lblHDFrom" runat="server" Text="From Date" Width="130" ToolTip="Minimum Flying Charge parameters are valid from this date onwards"></asp:Label>
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
                                    <asp:Label ID="lblHDTo" runat="server" Text="To Date" Width="130" ToolTip="Minimum Flying Charge parameters are valid up to and including this date"></asp:Label>
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
                                    <asp:Label ID="lblHMembCateg" runat="server" Text="Membership Category" Width="120"></asp:Label>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="lblIMembCateg" runat="server" Text='<%# dictMembCategs[(int)Eval("iMembCateg")] %>' Width="120"></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:DropDownList ID="DDLMembCateg" runat="server" DataSourceID="SqlDataSrc_MembCateg" DataTextField="sMembershipCategory"
                                        DataValueField="ID" Width="120" OnPreRender="DDL_PreRender" ></asp:DropDownList>
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Right">
                                <HeaderTemplate>
                                    <asp:Label ID="lblHMinMonthlyFlyChrg" runat="server" Text="$" Width="60" ToolTip="Minimum monthly flying charge for this membership category"></asp:Label>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="lblIMinMonthlyFlyChrg" runat="server" Text='<%# Eval("sMinFlyChrg") %>' Width="60"></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox ID="txbMinMonthlyFlyChrg" runat="server" Text='<%# Eval("sMinFlyChrg") %>' Width="60" CssClass="rightAlign" ></asp:TextBox>
                                    <asp:RegularExpressionValidator runat="server" ValidationExpression="^[+-]?\d*\.?\d*$" ControlToValidate="txbMinMonthlyFlyChrg"
                                        ErrorMessage="Must be a decimal number like 99.99 or 9" Display="Dynamic" Font-Bold="true" ForeColor="Red" Font-Size="Medium" />
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderStyle-HorizontalAlign="Center">
                                <HeaderTemplate>
                                    <asp:Label ID="lblHComments" runat="server" Text="Comments" Width="200"></asp:Label>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="lblIComments" runat="server" Text='<%# Eval("sComment") %>' Width="200"></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox ID="txbDComments" runat="server" Text='<%# Eval("sComment") %>' Width="200"></asp:TextBox>
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
                            </asp:TemplateField>

                        </Columns>
                    </asp:GridView>
                </div>
            </ContentTemplate>
        </ajaxToolkit:TabPanel>
        <ajaxToolkit:TabPanel runat="server" HeaderText="Generate Min Fly Chrg">
            <ContentTemplate>
                <div class="row">
                    <div class="col-sm-6">
                        <h4>Generation of Minimum Monthly Flying Charges</h4>
                        <h5>Using the parameters in the following table:</h5>
                    </div>
                    <div class="col-sm-6">
                        <asp:Button ID="pbHelp2" runat="server" Text="Display Help" OnClick="pbHelp_Click" />
                    </div>
                </div>
                <div class="gvclass">
                    <asp:GridView ID="gvGenMFC" runat="server" AutoGenerateColumns="False"
                        GridLines="None" CssClass="SoarNPGridStyle"
                        OnRowCommand="gvGenMFC_RowCommand"
                        AllowPaging="true" PageSize="25" ShowHeaderWhenEmpty="true"
                        Font-Size="Small">
                        <PagerSettings Mode="NextPreviousFirstLast" Position="TopAndBottom" Visible="true" FirstPageText="&nbsp;First&nbsp;"
                            LastPageText="&nbsp;Last&nbsp;" NextPageText="&nbsp;Next&nbsp;" PreviousPageText="&nbsp;Previous&nbsp;" />
                        <PagerStyle CssClass="SoarNPpaging" />
                        <Columns>
                            <asp:TemplateField HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                                <HeaderTemplate>
                                    <asp:Label ID="lblHIdent" runat="server" Text="Internal Id" Width="60" ToolTip="Points to a row in database table MINFLYCHGPARS with this ID field contents"></asp:Label>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="lblIIdent" runat="server" Text='<%# Eval("ID") %>' Width="60" ></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                                <HeaderTemplate>
                                    <asp:Label ID="lblHDFrom" runat="server" Text="From Date" Width="130" ToolTip="Minimum Flying Charge parameters are valid from this date onwards"></asp:Label>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="lblIDFrom" runat="server" Text='<%# TSoar.CustFmt.sFmtDate((DateTimeOffset)Eval("DFrom"),TSoar.CustFmt.enDFmt.DateOnly) %>' Width="130"></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                                <HeaderTemplate>
                                    <asp:Label ID="lblHDTo" runat="server" Text="To Date" Width="130" ToolTip="Minimum Flying Charge parameters are valid up to and including this date"></asp:Label>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="lblIDTo" runat="server" Text='<%# TSoar.CustFmt.sFmtDate((DateTimeOffset)Eval("DTo"),TSoar.CustFmt.enDFmt.DateOnly) %>' Width="130"></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField>
                                <HeaderTemplate>
                                    <asp:Label ID="lblHMembCateg" runat="server" Text="Membership Category" Width="120"></asp:Label>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="lblIMembCateg" runat="server" Text='<%# dictMembCategs[(int)Eval("iMembershipCategory")] %>' Width="120"></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Right">
                                <HeaderTemplate>
                                    <asp:Label ID="lblHMinMonthlyFlyChrg" runat="server" Text="$" Width="60" ToolTip="Minimum monthly flying charge for this membership category"></asp:Label>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="lblIMinMonthlyFlyChrg" runat="server" Text='<%# Eval("sMinFlyChrg") %>' Width="60"></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderStyle-HorizontalAlign="Center">
                                <HeaderTemplate>
                                    <asp:Label ID="lblHComments" runat="server" Text="Comments" Width="200"></asp:Label>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="lblIComments" runat="server" Text='<%# Eval("sComments") %>' Width="200"></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:ButtonField ButtonType="Button" CommandName="TakeAction" HeaderText="Take Action" Text="Take Action" ControlStyle-Font-Size="X-Small" ControlStyle-Height="15" />
                        </Columns>
                    </asp:GridView>
                </div>
            </ContentTemplate>
        </ajaxToolkit:TabPanel>
        <ajaxToolkit:TabPanel runat="server" HeaderText="Generate Actuals">
            <ContentTemplate>
                <div class="row">
                    <div class="col-sm-6">
                        <h4>Generate actuals by summarizing the actual monthly flying charges</h4>
                    </div>
                    <div class="col-sm-6">
                        <asp:Button ID="pbHelp3" runat="server" Text="Display Help" OnClick="pbHelp_Click" />
                    </div>
                </div>
                <asp:Table runat="server" CssClass="SoarNPGridStyle">
                    <asp:TableHeaderRow BackColor="#D0F0C0">
                        <asp:TableHeaderCell CssClass="text-center">Item</asp:TableHeaderCell>
                        <asp:TableHeaderCell CssClass="text-center">Value/Action</asp:TableHeaderCell>
                    </asp:TableHeaderRow>
                    <asp:TableRow BackColor="#D0F0C0">
                        <asp:TableCell HorizontalAlign="Right" ColumnSpan="2" Font-Italic="true">Specify the time interval for which invoices are to be summarized:</asp:TableCell>
                    </asp:TableRow>
                    <asp:TableRow BackColor="#EDF8D7">
                        <asp:TableCell HorizontalAlign="Right">Set the dates below to the beginning and end of the previous month:</asp:TableCell>
                        <asp:TableCell HorizontalAlign="Center"><asp:Button ID="pbISPreviousMonth" runat="server" Text="Reset" OnClick="pbISPreviousMonth_Click" 
                            ControlStyle-Font-Size="X-Small" Height="15" /></asp:TableCell>
                    </asp:TableRow>
                    <asp:TableRow BackColor="#EDF8D7">
                        <asp:TableCell HorizontalAlign="Right"> Start Date = Invoices dated on or after this "From" Date:</asp:TableCell>
                        <asp:TableCell HorizontalAlign="Center"><asp:TextBox ID="txbISDFrom" runat="server" TextMode="Date" AutoPostBack="true"
                            ToolTip="Summarize invoices with dates later than or on this date" /></asp:TableCell>
                    </asp:TableRow>
                    <asp:TableRow BackColor="#EDF8D7">
                        <asp:TableCell HorizontalAlign="Right">End Date = Invoices dated earlier or on this "To" Date:</asp:TableCell>
                        <asp:TableCell HorizontalAlign="Center"><asp:TextBox ID="txbISDTo" runat="server" TextMode="Date" AutoPostBack="true"
                            ToolTip="Summarize invoices with dates earlier than or equal to this date" /></asp:TableCell>
                    </asp:TableRow>
                    <asp:TableRow BackColor="#EDF8D7">
                        <asp:TableCell HorizontalAlign="Right" ColumnSpan="2">The current settings affect <asp:Label ID="lblISNumInv" runat="server" Text="0" /> invoices.</asp:TableCell>
                    </asp:TableRow>
                    <asp:TableRow BackColor="#D0F0C0">
                        <asp:TableCell HorizontalAlign="Right" ColumnSpan="2">&nbsp;</asp:TableCell>
                    </asp:TableRow>
                    <asp:TableRow BackColor="#EDF8D7">
                        <asp:TableCell HorizontalAlign="Right">Summarize or Re-summarize actual flying charges for above time interval:</asp:TableCell>
                        <asp:TableCell HorizontalAlign="Center"><asp:Button ID="pbISSummarize" runat="server" Text="Summarize Actuals" OnClick="pbISSummarize_Click" 
                            ControlStyle-Font-Size="X-Small" Height="15" /></asp:TableCell>
                    </asp:TableRow>
                    <asp:TableRow BackColor="#EDF8D7">
                        <asp:TableCell HorizontalAlign="Right" ColumnSpan="2">The last point in time when invoices were summarized in order to obtain actual monthly flying charges was:
                            <asp:Label ID="lblISPitLastSum" runat="server" Text="0" /></asp:TableCell>
                    </asp:TableRow>
                </asp:Table>
            </ContentTemplate>
        </ajaxToolkit:TabPanel>
        <ajaxToolkit:TabPanel runat="server" HeaderText="Manually Edit the Data">
            <ContentTemplate>
                <div class="row">
                    <div class="col-sm-6">
                        <h4>Manually editing minimum and actual flying charges</h4>
                    </div>
                    <div class="col-sm-6">
                        <asp:Button ID="pbHelp4" runat="server" Text="Display Help" OnClick="pbHelp_Click" />
                    </div>
                </div>
                Filters: Membership Category:
                <asp:DropDownList ID="DDLMembCategFCE" runat="server" DataSourceID="SqlDataSrc_MembCategFCE" DataTextField="sMembershipCategory" DataValueField="ID" 
                    OnPreRender="DDL_PreRender" OnSelectedIndexChanged="DDLMembCategFCE_SelectedIndexChanged" AutoPostBack="true" />
                , Member:
                <asp:DropDownList ID="DDLPerson" runat="server" DataSourceID="SqlDataSrc_Person" DataTextField="sDisplayName"
                    DataValueField="ID" Width="120" OnPreRender="DDL_PreRender" OnSelectedIndexChanged="DDLPerson_SelectedIndexChanged"
                    AutoPostBack="true"/>
                , From: &nbsp; <asp:TextBox ID="txbDFromFCE" runat="server" Width="135" TextMode="Date" ></asp:TextBox>
                , To: &nbsp; <asp:TextBox ID="txbDToFCE" runat="server" Width="135" TextMode="Date" ></asp:TextBox>
                <asp:Button ID="pbRefreshFCE" runat="server" Text="Refresh" OnClick="pbRefreshFCE_Click" />
                <div class="gvclass">
                    <asp:GridView ID="gvFCedit" runat="server" AutoGenerateColumns="False" ShowHeaderWhenEmpty="true"
                        GridLines="None" CssClass="SoarNPGridStyle"
                        OnRowDataBound="gvFCedit_RowDataBound"
                        OnRowEditing="gvFCedit_RowEditing"
                        OnRowDeleting="gvFCedit_RowDeleting" OnRowCancelingEdit="gvFCedit_RowCancelingEdit"
                        OnRowUpdating="gvFCedit_RowUpdating"
                        AllowPaging="true" PageSize="25" OnPageIndexChanging="gvFCedit_PageIndexChanging"
                        Font-Size="Small">
                        <PagerSettings Mode="Numeric" PageButtonCount="10" Position="TopAndBottom" />
<%--                        <PagerSettings Mode="NextPreviousFirstLast" Position="TopAndBottom" Visible="true" FirstPageText="&nbsp;First&nbsp;"
                            LastPageText="&nbsp;Last&nbsp;" NextPageText="&nbsp;Next&nbsp;" PreviousPageText="&nbsp;Previous&nbsp;" />--%>
                        <PagerStyle CssClass="SoarNPpaging" />
                        <Columns>
                            <asp:TemplateField HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                                <HeaderTemplate>
                                    <asp:Label ID="lblHFCIdent" runat="server" Text="Internal Id" Width="60" ToolTip="Points to a row in database table FLYINGCHARGES with this ID field contents"></asp:Label>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="lblIFCIdent" runat="server" Text='<%# Eval("ID") %>' Width="60" ></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Right">
                                <HeaderTemplate>
                                    <asp:Label ID="lblHAmount" runat="server" Text="$" Width="60" ToolTip="Amount of the flying charge" ></asp:Label>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="lblIAmount" runat="server" Text='<%# ((decimal)Eval("mAmount")).ToString("N2") %>' Width="60" CssClass="rightAlign"></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox ID="txbAmount" runat="server" Text='<%# ((decimal)Eval("mAmount")).ToString("N2") %>' Width="60" CssClass="rightAlign" ></asp:TextBox>
                                    <asp:RegularExpressionValidator runat="server" ValidationExpression="^[+-]?\d*\.?\d*$" ControlToValidate="txbAmount"
                                        ErrorMessage="Must be a decimal number like 99.99 or 9" Display="Dynamic" Font-Bold="true" ForeColor="Red" Font-Size="Medium" />
                                </EditItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                                <HeaderTemplate>
                                    <asp:Label ID="lblHDWhen" runat="server" Text="Date Of Amount" Width="135" ToolTip="Flying Charge Date;
actual flying charges are usually summarized for a month and dated on the last day of a month;
minimum monthly flying charges always occur on the last day of a month"></asp:Label>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="lblIDWhen" runat="server" Text='<%# TSoar.CustFmt.sFmtDate((DateTimeOffset)Eval("DateOfAmount"),TSoar.CustFmt.enDFmt.DateOnly) %>' Width="140"></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox ID="txbDWhen" runat="server" Text='<%# TSoar.CustFmt.sFmtDate((DateTimeOffset)Eval("DateOfAmount"),TSoar.CustFmt.enDFmt.DateOnly).Replace("/","-") %>' Width="140" TextMode="Date" ></asp:TextBox>
                                </EditItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField>
                                <HeaderTemplate>
                                    <asp:Label ID="lblHType" runat="server" Text="Type Of Amount" Width="120" ToolTip="
'A' for an actual flight charge (derived from flight operations data); 
'B' for billed amount;
'M' for minimum monthly flying charge (MFC);
'T' for MFC for tow pilots, instructors, and temporary members"></asp:Label>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="lblIType" runat="server" Text='<%# dictTypeOfAmount[(char)Eval("cTypeOfAmount")] %>' Width="120"></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <!-- The values in these dropdown list items must be the same and in the same order as in 'protected void DDL_PreRender(object sender, EventArgs e)',
                                            switch case "DDLType", string "ABMT" -->
                                    <asp:DropDownList ID="DDLType" runat="server" OnPreRender="DDL_PreRender" >
                                        <asp:ListItem Text="Actual" Value="A" />
                                        <asp:ListItem Text="Billed" Value="B" />
                                        <asp:ListItem Text="Minimum" Value="M" />
                                        <asp:ListItem Text="TowPilot/Instr Min" Value="T" />
                                    </asp:DropDownList>
                                </EditItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderStyle-HorizontalAlign="Center">
                                <HeaderTemplate>
                                    <asp:Label ID="lblHManuallyModified" runat="server" Text="Status" Width="50"
                                        ToolTip="Auto: item was created automatically either in the 'Generate Actuals' tab, or the 'Generate Min Fly Chrg' tab;
Man: item was created or modified manually here in the 'Manually Edit the Data' tab."></asp:Label>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="lblIManuallyModified" runat="server" Text='<%# (bool)Eval("bManuallyModified") ? "Man" : "Auto" %>' Width="50"></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderStyle-HorizontalAlign="Center">
                                <HeaderTemplate>
                                    <asp:Label ID="lblHComments" runat="server" Text="Comments" Width="200"></asp:Label>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="lblIComments" runat="server" Text='<%# Eval("sComments") %>' Width="200"></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox ID="txbDComments" runat="server" Text='<%# Eval("sComments") %>' Width="200"></asp:TextBox>
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
            </ContentTemplate>
        </ajaxToolkit:TabPanel>
    </ajaxToolkit:TabContainer>

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

    <div id="ModPopExtContextMenu">
        <asp:LinkButton ID="LinkButton1" runat="server" Text="T" CssClass="displayNone" />
        <ajaxToolkit:ModalPopupExtender ID="ModPopExtCM" runat="server"
            TargetControlID="LinkButton1" PopupControlID="MPE_PanelCM"
            BackgroundCssClass="background" />
        <asp:Panel ID="MPE_PanelCM" runat="server" CssClass="popup" style="display:none;" HorizontalAlign="Center">
            For MFC parameters with ID <asp:Label ID="lblMFCPID" runat="server" Text="0" />:
            <p> <asp:Button ID="pbDoMFC" runat="server" Text="Generate MFCs" OnClick="pbCM_Click" ToolTip="Create minimum flying charges for this time interval and this membership category" /><br />
                <asp:Button ID="pbRemoveMFC" runat="server" Text="Remove MFCs" OnClick="pbCM_Click" ToolTip="Remove any existing minimum flying charges for this time interval and this membership category" /><br />
                <asp:Button ID="pbCancel" runat="server" Text="Cancel" OnClick="pbCM_Click" ToolTip="Do nothing" /></p>
        </asp:Panel>
    </div>

    <div id="ModPopExtHelp">
        <%-- ModalPopupExtender for displaying Minimum Flying Charges Help for Parameter definition --%>
        <asp:LinkButton ID="TargetHelp" runat="server" Text="T" CssClass="displayNone" />
        <ajaxToolkit:ModalPopupExtender ID="ModPopExt_Help" runat="server"
            TargetControlID="TargetHelp" PopupControlID="MPE_Pnl_Help"
            BackgroundCssClass="background"
             />
        <asp:Panel ID="MPE_Pnl_Help" runat="server" CssClass="popup" style="display:none;" HorizontalAlign="Center"
                Width="600" Height="850">
            <p><asp:Button ID="pbFDone" runat="server" Text="Done with Help" OnClick="pbFDone_Click" /></p>
            <ajaxToolkit:Accordion
                ID="AccordionMFCHelp"
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
                            Flying Charges Help, Page 1: Minimum Flying Charge Parameters
                        </Header>
                        <Content>
                            <div class="leftAlign">
                                <h4>What are Flying Charges?</h4>
                                <p> Flying charges consist of charges for launching and/or towing a glider from the ground into the air, and for renting a glider from the Club.
                                    Those result in actual charges to the club members' accounts.
                                </p>
                                <h4>1. Minimum Flying Charge ("MFC") Parameters</h4>
                                <p> Club members in certain membership categories are required to spend a minimum monthly amount on flying activities during the months of the flying season.
                                    The process of generating and keeping track of MFC amounts has been largely automated in this section of the website's software.
                                    Required input consists of specifying for which time intervals and for which membership categories such an MFC exists, and its amount.
                                    The data are referred to as MFC parameters; you work with those in the tab called "Edit MFC Parameters".
                                    Months and membership categories not covered by any of the MFC parameters do not give rise to MFCs,
                                    i.e., it is not necessary to specify zero MFCs for months and membership categories when there are no MFCs.</p>

                                <p> While not required, it is recommended that `from` dates specify the first day of a month, and `To` dates specify the last day of a month.
                                    Default values for dates for new MFC parameter values are the first day of March, the last day of October of the current year, and the value given
                                    in the Setting called "DefaultMinimumMonthlyFlyingCharge" (72.00 at the time of this writing) for the amount of the MFC.
                                </p>

                                <p> For one membership category, the specified time intervals must not overlap. The software checks for this error condition.
                                </p>
                            </div>
                        </Content>
                    </ajaxToolkit:AccordionPane>
                    <ajaxToolkit:AccordionPane ID="AccPGenDat2" runat="server" >
                        <Header>
                            Flying Charges Help, Page 2: Generate or Remove Minimum Flying Charges
                        </Header>
                        <Content>
                            <div class="leftAlign">
                                <h4>2. Minimum Flying Charge Generation and Removal</h4>
                                <p>This happens in the tab called "Generate Min Fly Chrg". Clicking on one of the Take Action buttons allows you to generate (or remove)
                                    minimum flying charges (MFCs) according to the parameters in that table row,
                                    i.e., for that membership category and for the months in the given From-To time interval.
                                </p>
                                <p>The dates of MFCs are always forced to fall on the last day of a month.</p>
                                <p>When MFCs are generated on top of already existing ones, those latter ones are first deleted before the new MFCs are recorded in order to make sure that there
                                    are no duplicate MFCs, i.e., no more than one MFC may exist for one member and one month.
                                </p>
                            </div>
                        </Content>
                    </ajaxToolkit:AccordionPane>
                    <ajaxToolkit:AccordionPane ID="AccPGenDat3" runat="server" >
                        <Header>
                            Flying Charges Help, Page 3: Actual Flying Charges
                        </Header>
                        <Content>
                            <div class="leftAlign">
                                <h4>3. Actual Flying Charges</h4>
                                <p>Actual Flying Charges (AFCs) are calculated from club members' records of flying activities.
                                    First, those records result in invoices: one invoice per member per day of flying activities.
                                    To generate invoices click <a href="FlyActInvoice.aspx">here</a>.
                                </p>
                                <p>After all invoices have been generated for a month you can come to this page and generate a summary of actual flying charges.
                                    The purpose is to compare actuals and minimum flying charges to see whether the club member is to be charged for actuals or minima.
                                </p>
                                <p>AFCs can be generated as often as you like. Existing records are updated when an updated summary is requested.</p>
                            </div>

                        </Content>
                    </ajaxToolkit:AccordionPane>
                    <ajaxToolkit:AccordionPane ID="AccPGenDat4" runat="server" >
                        <Header>
                            Flying Charges Help, Page 4: Editing
                        </Header>
                        <Content>
                            <div class="leftAlign">
                                <h4>4. Modifying Flying Charges</h4>
                                <p>In order to handle exceptions from the automatically generated data, the flying charges (minimum and actual) can be modified,
                                    including adding new ones, and updating and deleting existing ones.</p>
                                <p>Between the heading and the data table, there are two dropdown lists so you can choose for which membership category and for which
                                    Club member to display and edit flying charges data. You can further filter the data by the From - To date range; after you modify one or both date(s)
                                    click the Refresh button; you may have to click it twice in some cases.
                                </p>
                                <p>Add new records by entering data in the bottom line of the last page with internal Id '0' (surrounded by orange/gold lines). Modify existing records
                                    by clicking one of the blue Edit buttons. Delete a record by clicking one of the red Delete buttons; you will be asked for confirmation.
                                </p>
                            </div>
                            </div>
                        </Content>
                    </ajaxToolkit:AccordionPane>
                </Panes>            
            </ajaxToolkit:Accordion>
        </asp:Panel>
    </div>

</asp:Content>

<asp:Content ID="FooterContent" ContentPlaceHolderID="ContentPlaceHolderFooter" runat="server">
    <%: Title %>
</asp:Content>