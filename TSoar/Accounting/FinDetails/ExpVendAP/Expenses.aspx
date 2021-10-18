<%@ Page Title="Expense Journal" Language="C#" MasterPageFile="~/mTSoar.Master" AutoEventWireup="true" CodeBehind="Expenses.aspx.cs"
    Inherits="TSoar.Accounting.FinDetails.ExpVendAP.Expenses" %>

<asp:Content ID="HeaderContent" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="server">
    <asp:Label runat="server" Text="Accounting | Bookkeeping | Expenses, Vendors, Accounts Payable | Expense Journal" Font-Italic="true" />
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="ContentPlaceHolderMain" runat="server">
    <style>
        table tr, th {border: 1px solid darkgreen; padding:5px 5px 5px 5px; text-align:center}
        table td {border: 1px solid darkgreen; padding:0px 5px 0px 5px; }
        .fontsmaller {font-size:smaller}
    </style>
    <p>A financial transactions filter <asp:Label runat="server" ID="lbl_is" Text="is" /> currently <asp:Label runat="server" ID="lbl_not" Text="not" /> in effect
        <asp:Label runat="server" ID="lbl_filter" Text="." />
    </p>
        <asp:Label ID="lblVersionUpdate" runat="server"
            Text="Your previous filter settings had to be deleted because of a filter settings data table version update; sorry about that!"
            Font-Bold="true" ForeColor="Red" Visible="false" />
    <asp:Button ID="pbExpFilters" Text="Work with Expense Journal Display Filters" runat="server" OnClick="pbExpFilters_Click" />
    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
    <asp:Button ID="pbXactExpense" Text="Create New Expense Record" runat="server" OnClick="pbXactExpense_Click" />
    <br />
    <h4>Expense Journal</h4>
    <asp:Button ID="pbNotes" runat="server" Text="Display Notes" Font-Size="Smaller" OnClick="pbNotes_Click" />
    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
    <asp:Button ID="pbSort" runat="server" Text="Sorting" Font-Size="Smaller" OnClick="pbSort_Click" />
    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
    <asp:CheckBox ID="chbTimeOfDay" runat="server" Text="Show Transaction Time of Day" OnCheckedChanged="chbTimeOfDay_CheckedChanged" AutoPostBack="true" />
    <br /><br />
    <div class="gvclass">
        <asp:GridView ID="gvExpenses" runat="server" AutoGenerateColumns="false"
            GridLines="None" CssClass="SoarNPGridStyle" PageSize="25" EmptyDataText="(Nothing to display)"
         OnRowDataBound="gvExpenses_RowDataBound"
            OnRowCommand="gvExpenses_RowCommand" AllowPaging="true">
            <PagerSettings Mode="NextPreviousFirstLast" Position="TopAndBottom" Visible="true" FirstPageText="&nbsp;First&nbsp;"
                LastPageText="&nbsp;Last&nbsp;" NextPageText="&nbsp;Next&nbsp;" PreviousPageText="&nbsp;Previous&nbsp;" />
            <PagerStyle CssClass="SoarNPpaging" />
            <Columns>
                <asp:TemplateField>
                    <HeaderTemplate>
                        <asp:Label runat="server" Text="ID" ToolTip="Internal unique numeric identifier of a transaction" />
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label ID="lblID" runat="server" Text='<%# Eval("ID") %>' /> 
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField>
                    <HeaderTemplate>
                        <asp:Label runat="server" Text="Date and Time" ToolTip="Time of Day is unimportant most of the time; last 6 characters are offset from Zulu time indicating a time zone" />
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label runat="server" Text='<%# Eval("Date") %>' />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField>
                    <HeaderTemplate>
                        <asp:Label runat="server" Text="Vendor Name" />
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label runat="server" Text='<%# Eval("sVendorName") %>' />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField>
                    <HeaderTemplate>
                        <asp:Label runat="server" Text="S" ToolTip="Status of the transaction: A (active), D (deleted), V (voided), R (Replaced), or T (template only)" />
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label runat="server" Text='<%# Eval("cStatus") %>' />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField>
                    <HeaderTemplate>
                        <asp:Label runat="server" Text="Total $" ToolTip="Total amount of all expenditure items in a transaction" />
                    </HeaderTemplate>
                    <ItemTemplate>
                        <div class="text-right">
                            <asp:Label runat="server" Text='<%# Eval("XactTotal") %>' />
                        </div>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField>
                    <HeaderTemplate>
                        <asp:Label runat="server" Text=" Expense Account(s)" ToolTip="The first of possibly several expense accounts occurring in expenditure items of a transaction" />
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label runat="server" Text='<%# Eval("Expense Account") %>' />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField>
                    <HeaderTemplate>
                        <asp:Label runat="server" Text="Description(s)" ToolTip="The first of possibly several expenditure descriptions in a transaction" />
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label runat="server" Text='<%# Eval("Description") %>' />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField>
                    <HeaderTemplate>
                        <asp:Label runat="server" Text="NX" ToolTip="The number of expenditure items in an expense transaction" />
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label runat="server" Text='<%# Eval("NX") %>' />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField>
                    <HeaderTemplate>
                        <asp:Label runat="server" Text="Payment Account(s)" ToolTip="The first of possibly several payment accounts occurring in payment items of a transaction" />
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label runat="server" Text='<%# Eval("Payment Account") %>' />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField>
                    <HeaderTemplate>
                        <asp:Label runat="server" Text="NP" ToolTip="The number of payment items in an expense transaction" />
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label runat="server" Text='<%# Eval("NP") %>' />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField>
                    <HeaderTemplate>
                        <asp:Label runat="server" Text="NF" ToolTip="The number of files attached to a transaction" />
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label runat="server" Text='<%# Eval("NumAtt") %>' />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField>
                    <HeaderTemplate>
                        <asp:Label runat="server" Text="Memorandum" />
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label runat="server" Text='<%# Eval("sMemo") %>' />
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:ButtonField ButtonType="Button" Text="Take Action" CommandName="TakeAction" HeaderText="Take Action" ControlStyle-Font-Size="X-Small" />

            </Columns>
        </asp:GridView>
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

    <div id="ModPopExtContextMenu">
        <asp:LinkButton ID="LinkButton1" runat="server" Text="T" CssClass="displayNone" />
        <ajaxToolkit:ModalPopupExtender ID="ModPopExtCM" runat="server"
            TargetControlID="LinkButton1" PopupControlID="MPE_PanelCM"
            BackgroundCssClass="background" />
        <asp:Panel ID="MPE_PanelCM" runat="server" CssClass="popup" style="display:none;" HorizontalAlign="Center">
            For transaction with ID <asp:Label ID="lblMPEXactID" runat="server" Text="0" />:
            <p> <asp:Button ID="pbInspect" runat="server" Text="Inspect Only" OnClick="pbCM_Click" ToolTip="Just look at the transaction's data without modifying it. Note that 'Replaced' transaction cannot be edited, voided, deleted, nor undeleted" /><br />
                <asp:Button ID="pbEdit" runat="server" Text="Edit" OnClick="pbCM_Click" ToolTip="Replace this transaction with a new transaction with modified data based on this transaction." /><br />
                <asp:Button ID="pbVoid" runat="server" Text="Void" OnClick="pbCM_Click" ToolTip="Change all $-amounts to zero; this action cannot be reversed." /><br />
                <asp:Button ID="pbDelete" runat="server" Text="Delete" OnClick="pbCM_Click" ToolTip="Mark the transaction as deleted (will no longer contribute to aggregates); the record is kept for audit trail purposes." /><br />
                <asp:Button ID="pbTemplate" runat="server" Text="Create Template" OnClick="pbCM_Click" ToolTip="Copy this transaction to create a template for entering future similar transactions in less time." /><br />
                <asp:Button ID="pbCancel" runat="server" Text="Cancel" OnClick="pbCM_Click" ToolTip="Do nothing to the transaction and return to List of Expenses." /></p>
        </asp:Panel>
    </div>

    <div id="ModPopExtSorting">
        <asp:LinkButton ID="LinkButton3" runat="server" Text="T" CssClass="displayNone" />
        <ajaxToolkit:ModalPopupExtender ID="ModPopExtSort" runat="server"
            TargetControlID="LinkButton3" PopupControlID="MPE_PanelSort"
            BackgroundCssClass="background" />
        <asp:Panel ID="MPE_PanelSort" runat="server" CssClass="popup" style="display:none;" HorizontalAlign="Center" Width="415">
            <h5>Expense Journal Sorting Control</h5>
            <p class="fontsmaller">By default, the Expense Journal is sorted first by date, then by vendor, then by status, and finally by amount. You can change those sort priorities here.
                You can also control whether items appear in ascending or descending order. The date, for eample, has a default sort order of 'descending' (most recent on top).
            </p>
            <asp:Table id="tblMPEexpSort" runat="server">
                <asp:TableHeaderRow>
                    <asp:TableHeaderCell></asp:TableHeaderCell>
                    <asp:TableHeaderCell>Sort Priority</asp:TableHeaderCell>
                    <asp:TableHeaderCell>Sort Order</asp:TableHeaderCell>
                </asp:TableHeaderRow>
                <asp:TableRow>
                    <asp:TableCell>Date</asp:TableCell>
                    <asp:TableCell><asp:RadioButtonList ID="obl0" runat="server" RepeatColumns="4" RepeatDirection="Horizontal" RepeatLayout="Table" TextAlign="Left">
                            <asp:ListItem Selected="True" Value="1"/>
                            <asp:ListItem Value="2"/>
                            <asp:ListItem Value="3"/>
                            <asp:ListItem Value="4"/>
                        </asp:RadioButtonList>
                    </asp:TableCell>
                    <asp:TableCell><asp:RadioButtonList ID="oblAscDesc0" runat="server" RepeatColumns="2" RepeatDirection="Horizontal" RepeatLayout="Table" TextAlign="Left">
                            <asp:ListItem Value="&nbsp;Ascending&nbsp;" />
                            <asp:ListItem Value="&nbsp;Descending&nbsp;" Selected="True" />
                        </asp:RadioButtonList>
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow>
                    <asp:TableCell>Vendor</asp:TableCell>
                    <asp:TableCell><asp:RadioButtonList ID="obl1" runat="server" RepeatColumns="4" RepeatDirection="Horizontal" RepeatLayout="Table" TextAlign="Left">
                            <asp:ListItem Value="1"/>
                            <asp:ListItem Selected="True" Value="2"/>
                            <asp:ListItem Value="3"/>
                            <asp:ListItem Value="4"/>
                        </asp:RadioButtonList>
                    </asp:TableCell>
                    <asp:TableCell><asp:RadioButtonList ID="oblAscDesc1" runat="server" RepeatColumns="2" RepeatDirection="Horizontal" RepeatLayout="Table" TextAlign="Left">
                            <asp:ListItem Value="&nbsp;Ascending&nbsp;" Selected="True" />
                            <asp:ListItem Value="&nbsp;Descending&nbsp;" />
                        </asp:RadioButtonList>
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow>
                    <asp:TableCell>Status</asp:TableCell>
                    <asp:TableCell><asp:RadioButtonList ID="obl2" runat="server" RepeatColumns="4" RepeatDirection="Horizontal" RepeatLayout="Table" TextAlign="Left">
                            <asp:ListItem Value="1"/>
                            <asp:ListItem Value="2"/>
                            <asp:ListItem Selected="True" Value="3"/>
                            <asp:ListItem Value="4"/>
                        </asp:RadioButtonList>
                    </asp:TableCell>
                    <asp:TableCell><asp:RadioButtonList ID="oblAscDesc2" runat="server" RepeatColumns="2" RepeatDirection="Horizontal" RepeatLayout="Table" TextAlign="Left">
                            <asp:ListItem Value="&nbsp;Ascending&nbsp;" Selected="True" />
                            <asp:ListItem Value="&nbsp;Descending&nbsp;" />
                        </asp:RadioButtonList>
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow>
                    <asp:TableCell>Amount</asp:TableCell>
                    <asp:TableCell><asp:RadioButtonList ID="obl3" runat="server" RepeatColumns="4" RepeatDirection="Horizontal" RepeatLayout="Table" TextAlign="Left">
                            <asp:ListItem Value="1"/>
                            <asp:ListItem Value="2"/>
                            <asp:ListItem Value="3"/>
                            <asp:ListItem Selected="True" Value="4"/>
                        </asp:RadioButtonList>
                    </asp:TableCell>
                    <asp:TableCell><asp:RadioButtonList ID="oblAscDesc3" runat="server" RepeatColumns="2" RepeatDirection="Horizontal" RepeatLayout="Table" TextAlign="Left">
                            <asp:ListItem Value="&nbsp;Ascending&nbsp;" />
                            <asp:ListItem Value="&nbsp;Descending&nbsp;" Selected="True" />
                        </asp:RadioButtonList>
                    </asp:TableCell>
                </asp:TableRow>
            </asp:Table>
            <br />
            <p> <asp:Button ID="pbSortOK" runat="server" Text="OK" OnClick="pbSortOKCancel_Click" />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                <asp:Button ID="pbSortCancel" runat="server" Text="Cancel" OnClick="pbSortOKCancel_Click" />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                <asp:Button ID="pbSortDefault" runat="server" Text="Use Defaults" OnClick="pbSortOKCancel_Click" />
            </p>
        </asp:Panel>
    </div>
<%--    <script>
        var col, el;
        $("input[type=radio]").click(function() {
           el = $(this);
           col = el.data("col");
           $("input[data-col=" + col + "]").prop("checked", false);
           el.prop("checked", true);
        });
    </script>--%>
    <script>
        $("asp:RadioButton").click(function() {
            $("asp:RadioButton." + this.classname).not($(this)).each(function() {
                this.checked = false;
            });
        });
    </script>
<%--            Guided by: https://stackoverflow.com/questions/15292114/html-radiobuttons-in-the-form-of-matrix-and-it-has-to-check-only-one-radio-butto--%>
<%--            <table>
                <tr>
                    <th></th>
                    <th>1</th>
                    <th>2</th>
                    <th>3</th>
                    <th>4</th>
                </tr>
                <tr>
                    <td>Date</td>
                    <td><input type="radio" name="row-1" data-col="1"></td>
                    <td><input type="radio" name="row-1" data-col="2"></td>
                    <td><input type="radio" name="row-1" data-col="3"></td>
                    <td><input type="radio" name="row-1" data-col="4"></td>
                </tr>
                <tr>
                    <td>Vendor</td>
                    <td><input type="radio" name="row-2" data-col="1"></td>
                    <td><input type="radio" name="row-2" data-col="2"></td>
                    <td><input type="radio" name="row-2" data-col="3"></td>
                    <td><input type="radio" name="row-2" data-col="4"></td>
                </tr>
                <tr>
                    <td>Status</td>
                    <td><input type="radio" name="row-3" data-col="1"></td>
                    <td><input type="radio" name="row-3" data-col="2"></td>
                    <td><input type="radio" name="row-3" data-col="3"></td>
                    <td><input type="radio" name="row-3" data-col="4"></td>
                </tr>
                <tr>
                    <td>Amount</td>
                    <td><input type="radio" name="row-4" data-col="1"></td>
                    <td><input type="radio" name="row-4" data-col="2"></td>
                    <td><input type="radio" name="row-4" data-col="3"></td>
                    <td><input type="radio" name="row-4" data-col="4"></td>
                </tr>
                </table>--%>
<%--            Guided by: https://stackoverflow.com/questions/7041003/radio-button-matrix-group-javascript-jquery
            <table>
                <tr>
                    <th></th>
                    <th>1</th>
                    <th>2</th>
                    <th>3</th>
                    <th>4</th>
                </tr>
                <tr>
                    <td>Date</td>
                    <td><asp:RadioButton id="ob11" class="RowDate" runat="server"/></td>
                    <td><asp:RadioButton id="ob12" class="RowDate" runat="server"/></td>
                    <td><asp:RadioButton id="ob13" class="RowDate" runat="server"/></td>
                    <td><asp:RadioButton id="ob14" class="RowDate" runat="server"/></td>
                </tr>
                <tr>
                    <td>Vendor</td>
                    <td><asp:RadioButton id="ob21" class="RowVendor" runat="server"/></td>
                    <td><asp:RadioButton id="ob22" class="RowVendor" runat="server"/></td>
                    <td><asp:RadioButton id="ob23" class="RowVendor" runat="server"/></td>
                    <td><asp:RadioButton id="ob24" class="RowVendor" runat="server"/></td>
                </tr>
                <tr>
                    <td>Status</td>
                    <td><asp:RadioButton id="ob31" class="RowStatus" runat="server"/></td>
                    <td><asp:RadioButton id="ob32" class="RowStatus" runat="server"/></td>
                    <td><asp:RadioButton id="ob33" class="RowStatus" runat="server"/></td>
                    <td><asp:RadioButton id="ob34" class="RowStatus" runat="server"/></td>
                </tr>
                <tr>
                    <td>Amount</td>
                    <td><asp:RadioButton id="ob41" class="RowAmount" runat="server"/></td>
                    <td><asp:RadioButton id="ob42" class="RowAmount" runat="server"/></td>
                    <td><asp:RadioButton id="ob43" class="RowAmount" runat="server"/></td>
                    <td><asp:RadioButton id="ob44" class="RowAmount" runat="server"/></td>
                </tr>
                </table>--%>
</asp:Content>

<asp:Content ID="FooterContent" ContentPlaceHolderID="ContentPlaceHolderFooter" runat="server">
    <%: Title %>
</asp:Content>
