<%@ Page Title="Expense Journal Filter" Language="C#" MasterPageFile="~/mTSoar.Master" AutoEventWireup="true" CodeBehind="ExpFilter.aspx.cs" Inherits="TSoar.Accounting.FinDetails.ExpVendAP.ExpFilter" %>

<asp:Content ID="HeaderContent" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="server">
    <asp:Label runat="server" Text="Set a Filter for Display of the Expense Journal" Font-Italic="true" />
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="ContentPlaceHolderMain" runat="server">
        <asp:SqlDataSource ID="SqlDataSrc_Vendors" runat="server" ConnectionString="<%$ ConnectionStrings:SqlConn %>"
            SelectCommand="SELECT ID,sVendorName FROM SF_VENDORS UNION ALL SELECT '0',' ' ORDER BY sVendorName" />
        <asp:SqlDataSource ID="SqlDataSrc_AttCat" runat="server" ConnectionString="<%$ ConnectionStrings:SqlConn %>"
            SelectCommand="SELECT ID,sAttachmentCateg FROM SF_ATTACHMENTCATEGS UNION ALL SELECT '0',' ' ORDER BY sAttachmentCateg" />
        <asp:SqlDataSource ID="SqlDataSrc_AttTyp" runat="server" ConnectionString="<%$ ConnectionStrings:SqlConn %>"
            SelectCommand="SELECT ID,sAllowedFileType FROM SF_ALLOWEDATTACHTYPES UNION ALL SELECT '0',' ' ORDER BY sAllowedFileType" />
        <asp:SqlDataSource ID="SqlDataSrc_PmtMeth" runat="server" ConnectionString="<%$ ConnectionStrings:SqlConn %>"
            SelectCommand="SELECT ID,sPaymentMethod FROM SF_PAYMENTMETHODS UNION ALL SELECT '0',' ' ORDER BY sPaymentMethod" />
        <asp:SqlDataSource ID="SqlDataSrc_ExpAcc" runat="server" ConnectionString="<%$ ConnectionStrings:SqlConn %>"
            SelectCommand="SELECT A.ID,(sCode + ' ' + sName) as sAccName FROM SF_ACCOUNTS A INNER JOIN SF_ACCTTYPES T ON A.iSF_Type=T.ID
                WHERE T.sAccountType='Expense' UNION ALL SELECT '0',' ' ORDER BY sAccName" />
        <asp:SqlDataSource ID="SqlDataSrc_PmtAcc" runat="server" ConnectionString="<%$ ConnectionStrings:SqlConn %>"
            SelectCommand="SELECT A.ID,(sCode + ' ' + sName) as sAccName FROM SF_ACCOUNTS A INNER JOIN SF_ACCTTYPES T ON A.iSF_Type=T.ID
                WHERE T.sAccountType='Assets' UNION ALL SELECT '0',' ' ORDER BY sAccName" />
    <div>
        <div style="text-align:center">
            <p><B>Note:</B> It is easy to lose any filter setting changes by leaving this page without clicking on the 
                '<B>Save these filter settings</B>' button at the bottom.</p>
            <p>Use filtering in order to show only those expense transactions which satisfy certain criteria. For example, you might want to
                display only the expenses for a certain vendor within a date range. It is possible to build simple and fairly complex filters. 
                The system remembers the last filter you built.
            </p>
            <hr />
            <asp:CheckBox ID="chbEnableFiltering" runat="server" Text="Check to turn on filtering; uncheck to not use any filters" 
                Checked="true" TextAlign="Right" Font-Size="Large" BorderColor="Orange" BorderStyle="Ridge" BorderWidth="5px"/><br />
            <asp:Label ID="lblVersionUpdate" runat="server"
                Text="Your previous filter settings had to be deleted because of a filter settings data table version update; sorry about that!"
                Font-Bold="true" ForeColor="Red" Visible="false" />
        </div>
        <h4>Filters Using a List of Items</h4>
        <div style="font-size:smaller">
            <ul>
            <li>The function of a list of items is to allow a match on any one or more of the items.</li>
            <li>The overall filter will include only those transactions for which each filter marked in 'Use' has found at least one match.</li>
            <li>Lists cannot be edited directly; you can reset a list to just 'All', and you can add to the list from the drop down in the right-most column.</li>
            <li>Pay attention to the tool tips: floating text that appears when you hover over many of the elements below.</li>
            </ul>
        </div>
        <asp:Table runat="server" ID="tblIN"  GridLines="Both" HorizontalAlign="Center"  CssClass="SoarNPGridStyle" BorderColor="Orange" BorderStyle="Ridge" BorderWidth="5px" >
            <asp:TableHeaderRow>
                <asp:TableHeaderCell runat="server" Text="Filter Name" CssClass="text-center" />
                <asp:TableHeaderCell runat="server" Text="Use <span style='color:red; font-weight:bold'>(1)</span>" CssClass="text-center" />
                <asp:TableHeaderCell runat="server" Text="In <span style='color:red; font-weight:bold'>(2)</span>" CssClass="text-center" />
                <asp:TableHeaderCell runat="server" Text="<span style='font-size:smaller'>Reset List</span>" CssClass="text-center" ToolTip="Click a button in this column to change the contents of the list to 'All'" />
                <asp:TableHeaderCell runat="server" CssClass="text-center"  >
                    <asp:Label Text="List of comma-separated items" runat="server" BackColor="White" Width="280" BorderColor="Gray" BorderStyle="Solid" BorderWidth="1" ToolTip="Lists can be wiped out by clicking the corresponding round RESET button" />
                    <asp:Label Text="Selector (adds to list)" runat="server" BackColor="White" Width="280" BorderColor="Gray" BorderStyle="Solid" BorderWidth="1" ToolTip="Click on a filter's drop down list, then make a selection and click to add it to the list to the left" />
                </asp:TableHeaderCell>
            </asp:TableHeaderRow>
            <asp:TableRow>
                <asp:TableCell Text="Vendor" />
                <asp:TableCell HorizontalAlign="Center">
                    <asp:CheckBox runat="server" ID="chbVendor" Checked="false" ToolTip="Check to turn the vendor filter on" />
                </asp:TableCell><asp:TableCell HorizontalAlign="Center">
                    <asp:CheckBox runat="server" ID="chbVendorIN" Checked="true" />
                </asp:TableCell><asp:TableCell HorizontalAlign="Center">
                    <asp:Button ID="pbResetVendor" runat="server" Text="" OnClick="pbReset_Click" Height="15" CssClass="roundedbutton" ToolTip="Click to change the filter list of Vendor names to just 'All'" />
                </asp:TableCell><asp:TableCell HorizontalAlign="Center">
                    <asp:TextBox ID="txbDDLVendor" runat="server" ReadOnly="true" Font-Size="X-Small" Text="All" CssClass="box_width" ToolTip="Wipe out this list by clicking Reset to the left; add to the list by selecting from the dropdown on the right" />
                    <asp:DropDownList ID="DDLVendor" runat="server" Width="300" DataSourceID="SqlDataSrc_Vendors" AutoPostBack="true" onchange="oktoSubmit = true;"
                        DataTextField="sVendorName" DataValueField="ID" OnSelectedIndexChanged="DDLVendor_SelectedIndexChanged" ToolTip="Select an item from this drop down to add to the list to the left" />
                </asp:TableCell>
            </asp:TableRow>
            <asp:TableRow>
                <asp:TableCell Text="Transaction Status" />
                <asp:TableCell HorizontalAlign="Center">
                    <asp:CheckBox runat="server" ID="chbStatus" Checked="false" ToolTip="Check to turn the status filter on" />
                </asp:TableCell><asp:TableCell HorizontalAlign="Center">
                    <asp:CheckBox runat="server" ID="chbStatusIN" Checked="true" />
                </asp:TableCell><asp:TableCell HorizontalAlign="Center">
                    <asp:Button ID="pbResetStatus" runat="server" Text="" OnClick="pbReset_Click" Height="15" CssClass="roundedbutton" ToolTip="Click to change the filter list of Status to just 'All'" />
                </asp:TableCell><asp:TableCell HorizontalAlign="Center">
                    <asp:TextBox ID="txbDDLStatus" runat="server" ReadOnly="true" Font-Size="X-Small" Text="Active, Voided" CssClass="box_width" ToolTip="Wipe out this list by clicking Reset to the left; add to the list by selecting from the dropdown on the right" />
                    <asp:DropDownList ID="DDLStatus" runat="server" Width="300" AutoPostBack="true" onchange="oktoSubmit = true;"
                        OnSelectedIndexChanged="DDLStatus_SelectedIndexChanged" ToolTip="Select an item from this drop down to add to the list to the left">
                        <asp:ListItem></asp:ListItem>
                        <asp:ListItem>Active</asp:ListItem>
                        <asp:ListItem>Voided</asp:ListItem>
                        <asp:ListItem>Deleted</asp:ListItem>
                        <asp:ListItem>Replaced</asp:ListItem>
                        <asp:ListItem>Template Only</asp:ListItem>
                    </asp:DropDownList>
                </asp:TableCell>
            </asp:TableRow>
            <asp:TableRow>
                <asp:TableCell Text="File Attachment Category" />
                <asp:TableCell HorizontalAlign="Center">
                    <asp:CheckBox runat="server" ID="chbAttCat" Checked="false" ToolTip="Check to turn the file attachment category filter on" />
                </asp:TableCell><asp:TableCell HorizontalAlign="Center">
                    <asp:CheckBox runat="server" ID="chbAttCatIN" Checked="true" />
                </asp:TableCell><asp:TableCell HorizontalAlign="Center">
                    <asp:Button ID="pbResetAttCat" runat="server" Text="" OnClick="pbReset_Click" Height="15" CssClass="roundedbutton" ToolTip="Click to change the filter list of File Attachment Categories to just 'All'" />
                </asp:TableCell><asp:TableCell HorizontalAlign="Center">
                    <asp:TextBox ID="txbDDLAttCat" runat="server" ReadOnly="true" Font-Size="X-Small" Text="All" CssClass="box_width" ToolTip="Wipe out this list by clicking Reset to the left; add to the list by selecting from the dropdown on the right (or just below)" />
                    <asp:DropDownList ID="DDLAttCat" runat="server" Width="300" DataSourceID="SqlDataSrc_AttCat" AutoPostBack="true" onchange="oktoSubmit = true;"
                        DataTextField="sAttachmentCateg" DataValueField="ID" OnSelectedIndexChanged="DDLAttCat_SelectedIndexChanged" ToolTip="Select an item from this drop down to add to the list to the left (or just above)" />
                </asp:TableCell>
            </asp:TableRow>
            <asp:TableRow>
                <asp:TableCell Text="File Attachment Type" />
                <asp:TableCell HorizontalAlign="Center">
                    <asp:CheckBox runat="server" ID="chbAttTyp" Checked="false" ToolTip="Check to turn the file attachment type filter on" />
                </asp:TableCell><asp:TableCell HorizontalAlign="Center">
                    <asp:CheckBox runat="server" ID="chbAttTypIN" Checked="true" />
                </asp:TableCell><asp:TableCell HorizontalAlign="Center">
                    <asp:Button ID="pbResetAttTyp" runat="server" Text="" OnClick="pbReset_Click" Height="15" CssClass="roundedbutton" ToolTip="Click to change the filter list of File Attachment Types to just 'All'" />
                </asp:TableCell><asp:TableCell HorizontalAlign="Center">
                    <asp:TextBox ID="txbDDLAttTyp" runat="server" ReadOnly="true" Font-Size="X-Small" Text="All" CssClass="box_width" ToolTip="Wipe out this list by clicking Reset to the left; add to the list by selecting from the dropdown on the right (or just below)" />
                    <asp:DropDownList ID="DDLAttTyp" runat="server" Width="300" DataSourceID="SqlDataSrc_AttTyp" AutoPostBack="true" onchange="oktoSubmit = true;"
                        DataTextField="sAllowedFileType" DataValueField="ID" OnSelectedIndexChanged="DDLAttTyp_SelectedIndexChanged" ToolTip="Select an item from this drop down to add to the list to the left (or just above)" />
                </asp:TableCell>
            </asp:TableRow>
            <asp:TableRow>
                <asp:TableCell Text="Payment Method" />
                <asp:TableCell HorizontalAlign="Center">
                    <asp:CheckBox runat="server" ID="chbPmtMeth" Checked="false" ToolTip="Check to turn the Payment Method filter on" />
                </asp:TableCell><asp:TableCell HorizontalAlign="Center">
                    <asp:CheckBox runat="server" ID="chbPmtMethIN" Checked="true" />
                </asp:TableCell><asp:TableCell HorizontalAlign="Center">
                    <asp:Button ID="pbResetPmtMeth" runat="server" Text="" OnClick="pbReset_Click" Height="15" CssClass="roundedbutton" ToolTip="Click to change the filter list of Payment Methods to just 'All'" />
                </asp:TableCell><asp:TableCell HorizontalAlign="Center">
                    <asp:TextBox ID="txbDDLPmtMeth" runat="server" ReadOnly="true" Font-Size="X-Small" Text="All" CssClass="box_width" ToolTip="Wipe out this list by clicking Reset to the left; add to the list by selecting from the dropdown on the right (or just below)" />
                    <asp:DropDownList ID="DDLPmtMeth" runat="server" Width="300" DataSourceID="SqlDataSrc_PmtMeth" AutoPostBack="true" onchange="oktoSubmit = true;"
                        DataTextField="sPaymentMethod" DataValueField="ID" OnSelectedIndexChanged="DDLPmtMeth_SelectedIndexChanged" ToolTip="Select an item from this drop down to add to the list to the left (or just above)" />
                </asp:TableCell>
            </asp:TableRow>
            <asp:TableRow>
                <asp:TableCell Text="Expense Account (in any of the transaction's entries)" />
                <asp:TableCell HorizontalAlign="Center">
                    <asp:CheckBox runat="server" ID="chbExpAcc" Checked="false" ToolTip="Check to turn the Expense Account filter on" />
                </asp:TableCell><asp:TableCell HorizontalAlign="Center">
                    <asp:CheckBox runat="server" ID="chbExpAccIN" Checked="true" />
                </asp:TableCell><asp:TableCell HorizontalAlign="Center">
                    <asp:Button ID="pbResetExpAcc" runat="server" Text="" OnClick="pbReset_Click" Height="15" CssClass="roundedbutton" ToolTip="Click to change the filter list of Expense Account to just 'All'" />
                </asp:TableCell><asp:TableCell HorizontalAlign="Center">
                    <asp:TextBox ID="txbDDLExpAcc" runat="server" ReadOnly="true" Font-Size="X-Small" Text="All" CssClass="box_width" ToolTip="Wipe out this list by clicking Reset to the left; add to the list by selecting from the dropdown on the right (or just below)" />
                    <asp:DropDownList ID="DDLExpAcc" runat="server" Width="300" DataSourceID="SqlDataSrc_ExpAcc" AutoPostBack="true" onchange="oktoSubmit = true;"
                        DataTextField="sAccName" DataValueField="ID" OnSelectedIndexChanged="DDLExpAcc_SelectedIndexChanged" ToolTip="Select an item from this drop down to add to the list to the left (or just above)" />
                </asp:TableCell>
            </asp:TableRow>
            <asp:TableRow>
                <asp:TableCell Text="Payment Account (in any of the transaction's entries)" />
                <asp:TableCell HorizontalAlign="Center">
                    <asp:CheckBox runat="server" ID="chbPmtAcc" Checked="false" ToolTip="Check to turn the Payment Account filter on" />
                </asp:TableCell><asp:TableCell HorizontalAlign="Center">
                    <asp:CheckBox runat="server" ID="chbPmtAccIN" Checked="true" />
                </asp:TableCell><asp:TableCell HorizontalAlign="Center">
                    <asp:Button ID="pbResetPmtAcc" runat="server" Text="" OnClick="pbReset_Click" Height="15" CssClass="roundedbutton" ToolTip="Click to change the filter list of Payment Account to just 'All'" />
                </asp:TableCell><asp:TableCell HorizontalAlign="Center">
                    <asp:TextBox ID="txbDDLPmtAcc" runat="server" ReadOnly="true" Font-Size="X-Small" Text="All" CssClass="box_width" ToolTip="Wipe out this list by clicking Reset to the left; add to the list by selecting from the dropdown on the right (or just below)" />
                    <asp:DropDownList ID="DDLPmtAcc" runat="server" Width="300" DataSourceID="SqlDataSrc_PmtAcc" AutoPostBack="true" onchange="oktoSubmit = true;"
                        DataTextField="sAccName" DataValueField="ID" OnSelectedIndexChanged="DDLPmtAcc_SelectedIndexChanged" ToolTip="Select an item from this drop down to add to the list to the left (or just above)" />
                </asp:TableCell>
            </asp:TableRow>
        </asp:Table>
        <p><span style='color:red; font-weight:bold'>
                    (1)</span>: Put checkmarks in this column to 'Use' a filter, uncheck to ignore<br /> <span style='color:red; font-weight:bold'>
                    (2)</span>: Put checkmarks in this column to include only those transactions which have matching items 'In' the list;
                        uncheck to do the opposite: exclude all transactions which have matching items 'In' the list</p>
        <h4>Filters Using a Range of Values</h4>
        <asp:Table runat="server" ID="tblRange" GridLines="Both" HorizontalAlign="Center"  CssClass="SoarNPGridStyle" BorderColor="Orange" BorderStyle="Ridge" BorderWidth="5px" >
            <asp:TableHeaderRow>
                <asp:TableHeaderCell runat="server" Text="Filter Name" CssClass="text-center" />
                <asp:TableHeaderCell runat="server" Text="Use <span style='color:red; font-weight:bold'>(1)</span>" CssClass="text-center" />
                <asp:TableHeaderCell runat="server" Text="Reset" CssClass="text-center" />
                <asp:TableHeaderCell runat="server" Text="Lower Limit" CssClass="text-center" />
                <asp:TableHeaderCell runat="server" Text="Upper Limit" CssClass="text-center" />
            </asp:TableHeaderRow>
            <asp:TableRow>
                <asp:TableCell Text="Transaction Date" />
                <asp:TableCell HorizontalAlign="Center">
                    <asp:CheckBox runat="server" ID="chbXactDate" ToolTip="Check to turn on the transaction date filter" />
                </asp:TableCell><asp:TableCell HorizontalAlign="Center">
                    <asp:Button ID="pbResetXactDate" runat="server" Text="" OnClick="pbReset_Click" Height="15" CssClass="roundedbutton" ToolTip="Click to change the filter limits to include all transaction dates" />
                </asp:TableCell><asp:TableCell HorizontalAlign="Center">
                    <asp:TextBox runat="server" ID="txbXactDateLo" TextMode="Date"
                        ToolTip="Date portion of lower limit in format YYYY/MM/DD" Width="130px" />
                    <asp:TextBox runat="server" ID="txbXactTimeLo" TextMode="Time" Width="75px"
                        ToolTip="Time of Day portion of lower limit in format HH:mm (usually 00:00)" />
                    <asp:TextBox runat="server" ID="txbXactOffsetLo" Width="50px"
                        ToolTip="Time offset portion from local to UTC like -08:00 (sHH:mm where s is + or -, HH is hours of offset, and mm is minutes (usually 00)); for Pacific Standard Time use -08:00. Limited to +/-14:00." />
                    <asp:RegularExpressionValidator runat="server" ControlToValidate="txbXactOffsetLo" ValidationExpression="^[-+]((1[0-4])|(0\d)):[0-5]\d$"
                        ErrorMessage="Must be a time offset from local to UTC like -08:00" Display="Dynamic" ForeColor="Red" BorderStyle="Dotted" BorderColor="Red" BorderWidth="4px"/>
                </asp:TableCell><asp:TableCell HorizontalAlign="Center">
                    <asp:TextBox runat="server" ID="txbXactDateHi" TextMode="Date"
                        ToolTip="Date portion of upper limit in format YYYY/MM/DD" Width="130px" />
                    <asp:TextBox runat="server" ID="txbXactTimeHi" TextMode="Time" Width="75px"
                        ToolTip="Time of Day portion of upper limit in format HH:mm (usually 23:59)" />
                    <asp:TextBox runat="server" ID="txbXactOffsetHi" Width="50px"
                        ToolTip="Time offset portion from local to UTC like -08:00 (sHH:mm where s is + or -, HH is hours of offset, and mm is minutes (usually 00)); for Pacific Standard Time use -08:00. Limited to +/-14:00." />
                    <asp:RegularExpressionValidator runat="server" ControlToValidate="txbXactOffsetHi" ValidationExpression="^[-+]((1[0-4])|(0\d)):[0-5]\d$"
                        ErrorMessage="Must be a time offset from local to UTC like -08:00" Display="Dynamic" ForeColor="Red" BorderStyle="Dotted" BorderColor="Red" BorderWidth="4px"/>
                </asp:TableCell>
            </asp:TableRow>
            <asp:TableRow>
                <asp:TableCell Text="Transaction Amount" />
                <asp:TableCell HorizontalAlign="Center">
                    <asp:CheckBox runat="server" ID="chbAmount" ToolTip="Check to turn on the amount filter" />
                </asp:TableCell><asp:TableCell HorizontalAlign="Center">
                    <asp:Button ID="pbResetAmount" runat="server" Text="" OnClick="pbReset_Click" Height="15" CssClass="roundedbutton" ToolTip="Click to change the filter limits to include practically any amount" />
                </asp:TableCell><asp:TableCell HorizontalAlign="Center">
                    <asp:TextBox runat="server" ID="txbAmountLo" Text="0" />
                    <asp:RegularExpressionValidator runat="server" ControlToValidate="txbAmountLo" ValidationExpression="^[-+]?\d*\.?\d*$"
                        ErrorMessage="Must be a decimal number (optional sign, optional decimal point)" Display="Dynamic" ForeColor="Red" BorderStyle="Dotted" BorderColor="Red" BorderWidth="4px" />
                </asp:TableCell><asp:TableCell HorizontalAlign="Center">
                    <asp:TextBox runat="server" ID="txbAmountHi" Text="999999999.99" />
                    <asp:RegularExpressionValidator runat="server" ControlToValidate="txbAmountHi" ValidationExpression="^[-+]?\d*\.?\d*$"
                        ErrorMessage="Must be a decimal number (optional sign, optional decimal point)" Display="Dynamic" ForeColor="Red" BorderStyle="Dotted" BorderColor="Red" BorderWidth="4px" />
                </asp:TableCell>
            </asp:TableRow>
            <asp:TableRow>
                <asp:TableCell Text="Number of Attached Files" />
                <asp:TableCell HorizontalAlign="Center">
                    <asp:CheckBox runat="server" ID="chbNAtt" ToolTip="Check to turn on the number of attached files filter" />
                </asp:TableCell><asp:TableCell HorizontalAlign="Center">
                    <asp:Button ID="pbResetNAtt" runat="server" Text="" OnClick="pbReset_Click" Height="15" CssClass="roundedbutton" ToolTip="Click to change the filter limits to include transactions with up to 100 attached files" />
                </asp:TableCell><asp:TableCell HorizontalAlign="Center">
                    <asp:TextBox runat="server" ID="txbNAttLo" TextMode="Number" Text="0" />
                </asp:TableCell><asp:TableCell HorizontalAlign="Center">
                    <asp:TextBox runat="server" ID="txbNAttHi" TextMode="Number" Text="100" />
                </asp:TableCell>
            </asp:TableRow>
        </asp:Table>
    </div>
        <asp:Panel runat="server" BorderStyle="Solid" BorderWidth="4px" BorderColor="PowderBlue" Width="460px" BackColor="Navy" HorizontalAlign="Center" Wrap="true" CssClass="panel_with_padding">
        &nbsp;&nbsp;
            <asp:Button ID="pbXactOK" runat="server" Text="Save these filter settings" OnClick="pbXactOK_Click" OnClientClick="oktoSubmit=true;"/>
            &nbsp;&nbsp; <asp:Button ID="pbExpCancel" runat="server" Text="Abandon any filter setting changes" OnClick="pbExpCancel_Click" OnClientClick="oktoSubmit=true;"/>
        &nbsp;&nbsp;
        </asp:Panel>

    <div>
        <%-- ModalPopupExtender, popping up MPE_Panel and dynamically populating lblPopupText --%>
        <asp:LinkButton ID="Target" runat="server" Text="T" CssClass="displayNone" />
        <ajaxToolkit:ModalPopupExtender ID="ModalPopExt" runat="server"
            TargetControlID="Target" PopupControlID="MPE_Panel"
            BackgroundCssClass="background" DropShadow="true" />
        <asp:Panel ID="MPE_Panel" runat="server" CssClass="popup" style="display:none;" HorizontalAlign="Center">
            <asp:Label ID="lblPopupText" runat="server" BackColor="#eeb5a2" Font-Bold="true" />
            <br /><br />
            <p> <asp:Button ID="OkButton" runat="server" Text="OK" OnClick="Button_Click" OnClientClick="oktoSubmit = true;" />&nbsp;&nbsp;
                <asp:Button ID="CancelButton" runat="server" Text="Cancel" OnClick="Button_Click" OnClientClick="oktoSubmit = true;" />&nbsp;&nbsp;
                <asp:Button ID="NoButton" runat="server" Text="No" OnClick="Button_Click" OnClientClick="oktoSubmit = true;" />&nbsp;&nbsp;
                <asp:Button ID="YesButton" runat="server" Text="Yes" OnClick="Button_Click" OnClientClick="oktoSubmit = true;" /></p>
        </asp:Panel>
    </div>

<script type="text/javascript" >
    var oktoSubmit = false; // 
    var isFiredTwice = false;

    window.onbeforeunload = confirmExit;
    function confirmExit() {
        if (!oktoSubmit) {
            if (navigator.appName == "Microsoft Internet Explorer") {
                if (!isFiredTwice) {
                    event.returnValue = "If you have any unsaved data in the current page, it will be lost.";
                    isFiredTwice = true;
                    setTimeout("isFiredTwice = false;", 0);
                }
            }
            else {
                // For other browsers: (tested with Chrome, but Chrome uses its own message, not the one specified in the next statement)
                event.returnValue = "If you have any unsaved data in the current page, it will be lost.";
            }
        }  
    }
</script>

</asp:Content>

<asp:Content ID="FooterContent" ContentPlaceHolderID="ContentPlaceHolderFooter" runat="server">
    <%: Title %>
</asp:Content>