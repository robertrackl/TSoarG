<%@ Page Title="Rewards Edit Filter" Language="C#" MasterPageFile="~/mTSoar.Master" AutoEventWireup="true" CodeBehind="TIRewardsFilter.aspx.cs" Inherits="TSoar.Statistician.TIRewardsFilter" %>

<asp:Content ID="HeaderContent" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="server">
    <asp:Label runat="server" Text="Set a Filter for Display of the Rewards Journal" Font-Italic="true" />
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="ContentPlaceHolderMain" runat="server">
    <div style="text-align:center">
        <p><B>Note:</B> It is easy to lose any filter setting changes by leaving this page without clicking on the 
            '<B>Save these filter settings</B>' button at the bottom.</p>
        <p>Use filtering in order to show only those service point records which satisfy certain criteria. For example, you might want to
            display only the service points for a certain member within an earn date range. It is possible to build simple and fairly complex filters. 
            The system remembers the last filter you built.
        </p>
        <hr />
        <asp:Label ID="lblVersionUpdate" runat="server"
            Text="Your previous filter settings had to be deleted because of a filter settings data table version update; sorry about that!"
            Font-Bold="true" ForeColor="Red" Visible="false" />
    </div>
        <asp:Table runat="server" GridLines="Both" HorizontalAlign="Center"  CssClass="SoarNPGridStyle" BorderColor="Orange" BorderStyle="Ridge" BorderWidth="5px" >
            <asp:TableRow>
                <asp:TableCell Text="Limit the Number of Displayed Data Rows" />
                <asp:TableCell HorizontalAlign="Center">
                    <asp:CheckBox runat="server" ID="chbN"
                        ToolTip="Check to limit the number of data rows displayed, uncheck to show all rows (subject to other filter settings below)" />
                </asp:TableCell>
                <asp:TableCell>
                    <asp:RadioButtonList ID="rblN" runat="server"
                        ToolTip="Show the limited number of rows counting from the top or counting from the bottom" >
                        <asp:ListItem Text="Top" Value="1" Selected="False" />
                        <asp:ListItem Text ="Bottom" Value="2" Selected="True" />
                    </asp:RadioButtonList>
                </asp:TableCell>
                <asp:TableCell HorizontalAlign="Center">
                    <asp:TextBox ID="txbN" runat="server" Text="25" TextMode="Number" style="width: 50px; text-align: right;" ToolTip="Maximum number of data rows to display" />
                    <asp:RegularExpressionValidator runat="server" ControlToValidate="txbN" ValidationExpression="^ *0*([1-9]|[1-9]\d|200|1\d\d) *$"
                        ErrorMessage="Must be an integer between 1 and 200" Display="Dynamic" ForeColor="Red" BorderStyle="Dotted" BorderColor="Red" BorderWidth="4px" />
                </asp:TableCell>
            </asp:TableRow>
        </asp:Table>
        <hr />
    <div class="text-center">
        <asp:CheckBox ID="chbEnableFiltering" runat="server" Text="Check to turn on filtering below; uncheck to not use those filters" 
            Checked="true" TextAlign="Right" Font-Size="Small" BorderColor="Orange" BorderStyle="Ridge" BorderWidth="5px" HorizontalAlign="Center" /><br />
        <asp:Table runat="server" GridLines="Both" HorizontalAlign="Center"  CssClass="SoarNPGridStyle" BorderColor="Orange" BorderStyle="Ridge" BorderWidth="5px" >
            <asp:TableHeaderRow>
                <asp:TableHeaderCell runat="server" Text="Filter Name" CssClass="text-center" />
                <asp:TableHeaderCell runat="server" Text="Use" ToolTip="Turn filters on or off individually" CssClass="text-center" />
                <asp:TableHeaderCell runat="server" Text="" />
                <asp:TableHeaderCell runat="server" Text="" />
            </asp:TableHeaderRow>
            <asp:TableRow>
                <asp:TableCell Text="Member Name" />
                <asp:TableCell HorizontalAlign="Center">
                    <asp:CheckBox runat="server" ID="chbMember" ToolTip="Check to turn on the member filter, uncheck to disable" />
                </asp:TableCell>
                <asp:TableCell HorizontalAlign="Center">
                    <asp:DropDownList ID="DDLMember" runat="server" ToolTip="Select a member for whom to show rewards data" />
                </asp:TableCell><asp:TableCell>
                    <asp:TextBox ID="txbAsOfDate" runat="server" TextMode="Date" AutoPostBack="true"
                        ToolTip="The members in the list to the left are chosen based upon their piloting qualifications as of this date" />
                </asp:TableCell>
            </asp:TableRow>
            <asp:TableRow>
                <asp:TableCell Text="Earn/Claim Date" />
                <asp:TableCell HorizontalAlign="Center">
                    <asp:CheckBox runat="server" ID="chbEarnDate" ToolTip="Check to turn on the service points earn/claim date filter, uncheck to disable" />
                </asp:TableCell>
                <asp:TableCell HorizontalAlign="Center">
                    <asp:TextBox runat="server" ID="txbEarnDateLo" TextMode="Date"
                        ToolTip="Lower limit in format YYYY/MM/DD" Width="135px" />
                </asp:TableCell>
                <asp:TableCell HorizontalAlign="Center">
                    <asp:TextBox runat="server" ID="txbEarnDateHi" TextMode="Date"
                        ToolTip="Upper limit in format YYYY/MM/DD" Width="135px" />
                </asp:TableCell>
            </asp:TableRow>
            <asp:TableRow>
                <asp:TableCell Text="Show Entries that expired more than 90 days ago" />
                <asp:TableCell HorizontalAlign="Center" ColumnSpan="3">
                    <asp:CheckBox runat="server" ID="chbShowExpired" ToolTip="Check to show expired entries, uncheck to hide them" />
                </asp:TableCell>
            </asp:TableRow>
            <asp:TableRow>
                <asp:TableCell Text="Earn/Claim Code" />
                <asp:TableCell HorizontalAlign="Center">
                    <asp:CheckBox runat="server" ID="chbECCode" ToolTip="Check to turn on the Earn/Claim Code filter, uncheck to disable" />
                </asp:TableCell>
                <asp:TableCell HorizontalAlign="Center">
                    <asp:DropDownList ID="DDLECCode" runat="server" >
                        <asp:ListItem Value="C">Claim</asp:ListItem>
                        <asp:ListItem Value="G">Gift Claim</asp:ListItem>
                        <asp:ListItem Value="I">Instructor</asp:ListItem>
                        <asp:ListItem Value="M">Manual</asp:ListItem>
                        <asp:ListItem Value="S">Showed Up</asp:ListItem>
                        <asp:ListItem Value="T">Tow Pilot</asp:ListItem>
                    </asp:DropDownList>
                </asp:TableCell><asp:TableCell>
                    
                </asp:TableCell>
            </asp:TableRow>
        </asp:Table>

        <asp:Panel runat="server" BorderStyle="Solid" BorderWidth="4px" BorderColor="PowderBlue" Width="460px" BackColor="Navy" Wrap="true" CssClass="panel_with_padding">
        &nbsp;&nbsp;
            <asp:Button ID="pbFilterOK" runat="server" Text="Save these filter settings" OnClick="pbFilterOK_Click" OnClientClick="oktoSubmit=true;"/>
            &nbsp;&nbsp;
            <asp:Button ID="pbExpCancel" runat="server" Text="Abandon any filter setting changes" OnClick="pbExpCancel_Click" OnClientClick="oktoSubmit=true;"/>
        &nbsp;&nbsp;
        </asp:Panel>
    </div>
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