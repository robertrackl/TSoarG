<%@ Page Title="TSoar Administrators Page" Language="C#" MasterPageFile="~/mTSoar.Master" AutoEventWireup="true" CodeBehind="Administrators.aspx.cs"
    Inherits="TSoar.AdminPages.Administrators" %>

<asp:Content ID="HeaderContent" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="server">
    <asp:Label runat="server" Text="Website Administration" Font-Italic="true" />
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="ContentPlaceHolderMain" runat="server">
    <div style="text-align:center">
       <h4> Area for Administrators of this Website</h4>
        <div class="row">
            <div class="col-sm-4">
                Terminate this Web Application: <asp:Button ID="pbUnloadAppDomain" runat="server" Text="Terminate Web App" OnClick="pbUnloadAppDomain_Click"
                    ToolTip="You may try this in an attempt to restart this web application so that the Application_Start event handler runs.
 Experience shows that it works on the development system, but not once the application is deployed to the World Wide Web.
 In that case it's more advisable to simulate the application startup (next button ...)" />
            </div>
<%--            <div class="col-sm-4">
                Simulate Application Startup: <asp:Button ID="pbSimulateAppStart" runat="server" Text="Simulate Web App Start"
                    OnClick="pbSimulateAppStart_Click"
                    ToolTip="Execute the same code that runs when the Application_Start event fires.
 The scope is 'entire application', i.e., not user specific; sets some global variables.
 Use this after changes are made to the website's underlying executable code." />
            </div>--%>
            <div class="col-sm-4">
                Show Values of Global Variables: <asp:Button ID="pbShowGlobals" runat="server" Text="Show Globals"
                 OnClick="pbShowGlobals_Click" />
            </div>
        </div>
    </div>

    <asp:Table runat="server" BorderStyle="Solid">
        <asp:TableRow BorderStyle="Solid">
            <asp:TableCell BorderStyle="Solid">&nbsp;&nbsp;<a href="Security/Security.aspx">Jump to Website Security</a>&nbsp;&nbsp;</asp:TableCell>
            <asp:TableCell BorderStyle="Solid">&nbsp;&nbsp;<a href="DBMaint/DBMaint.aspx">Jump to Database Maintenance</a>&nbsp;&nbsp;</asp:TableCell>
            <asp:TableCell BorderStyle="Solid">&nbsp;&nbsp;<a href="DBMaint/DBIntegrity.aspx">Jump to Database Integrity Check</a>&nbsp;&nbsp;</asp:TableCell>
        </asp:TableRow>
    </asp:Table>
    <h4>Activity Log: Showing up to the last 500 entries, <asp:Label ID="lblPageSize" runat="server" /> at a time, latest First. There are <asp:Label ID="lblNumEntries" runat="server"/> log entries.</h4>
    <style type="text/css">
        th { text-align:center; border-style:solid; padding-left:5px; padding-right:5px;}
        td { border-style:solid; border-width:1px; padding-left:5px; padding-right:5px;}
    </style>
    <asp:GridView ID="gvActLog" runat="server" CssClass="SoarNPGridStyle" AutoGenerateColumns="false"
        OnPageIndexChanging="gvActLog_PageIndexChanging" AllowPaging="true" PageSize="25">
        <PagerSettings Mode="NextPreviousFirstLast" Position="TopAndBottom" Visible="true" FirstPageText="&nbsp;First&nbsp;"
            LastPageText="&nbsp;Last&nbsp;" NextPageText="&nbsp;Next&nbsp;" PreviousPageText="&nbsp;Previous&nbsp;" />
        <PagerStyle CssClass="SoarNPpaging" />
        <Columns>
            <asp:BoundField DataField="DUTC" HeaderText="Zulu" DataFormatString="{0:yyyy'/'MM'/'dd HH:mm:ss.fff}" ItemStyle-Font-Size="X-Small" />
            <asp:BoundField DataField="sLogType" HeaderText="Log Type" ItemStyle-Font-Size="X-Small" />
            <asp:BoundField DataField="iDbgLvl" HeaderText="Dbg Lvl" ItemStyle-Font-Size="X-Small" />
            <asp:BoundField DataField="sMsg" HeaderText="Log Message" />
        </Columns>
    </asp:GridView>

    <div>
        <%-- ModalPopupExtender, popping up MPE_Panel and dynamically populating lblPopupText --%>
        <asp:LinkButton ID="Target" runat="server" Text="T" CssClass="displayNone" />
        <ajaxToolkit:ModalPopupExtender ID="ModalPopExt" runat="server" PopupDragHandleControlID="MPE_Panel"
            TargetControlID="Target" PopupControlID="MPE_Panel" RepositionMode="RepositionOnWindowResizeAndScroll"
            BackgroundCssClass="background" />
        <asp:Panel ID="MPE_Panel" runat="server" CssClass="popup" style="display:none;" HorizontalAlign="Center">
            <asp:Label ID="lblPopupText" runat="server" BackColor="#eeb5a2" Font-Bold="true" />
            <asp:Table runat="server">
                <asp:TableHeaderRow>
                    <asp:TableHeaderCell>Variable Name</asp:TableHeaderCell>
                    <asp:TableHeaderCell>Variable Value</asp:TableHeaderCell>
                    <asp:TableHeaderCell>Explanation</asp:TableHeaderCell>
                </asp:TableHeaderRow>
                <asp:TableRow>
                    <asp:TableCell>sgcAppName</asp:TableCell>
                    <asp:TableCell><asp:Label ID="lblsgcAppName" runat="server" Text="sgcAppName" /></asp:TableCell>
                    <asp:TableCell>The name of the web application underlying this website</asp:TableCell>
                </asp:TableRow>
                <asp:TableRow>
                    <asp:TableCell>scgHomeShow</asp:TableCell>
                    <asp:TableCell><asp:Label ID="lblscgHomeShow" runat="server" Text="scgHomeShow" /></asp:TableCell>
                    <asp:TableCell>The path to the collection of pictures that are shown on the home page</asp:TableCell>
                </asp:TableRow>
                <asp:TableRow>
                    <asp:TableCell>scgRelPathUpload</asp:TableCell>
                    <asp:TableCell><asp:Label ID="lblscgRelPathUpload" runat="server" Text="scgRelPathUpload" /></asp:TableCell>
                    <asp:TableCell>The path to the location for uploaded files, relative to this website's root directory</asp:TableCell>
                </asp:TableRow>
                <asp:TableRow>
                    <asp:TableCell>sgMigrationLevelDevIntProd</asp:TableCell>
                    <asp:TableCell><asp:Label ID="lblDevIntProd" runat="server" Text="sgMigrationLevelDevIntProd" /></asp:TableCell>
                    <asp:TableCell>The level of software migration during development work: Development, Integration, Production</asp:TableCell>
                </asp:TableRow>
                <asp:TableRow>
                    <asp:TableCell>sgVersion</asp:TableCell>
                    <asp:TableCell><asp:Label ID="lblsgVersion" runat="server" Text="sgVersion" /></asp:TableCell>
                    <asp:TableCell>The software version that is running this website; also same as delivery number</asp:TableCell>
                </asp:TableRow>
                <asp:TableRow>
                    <asp:TableCell>DTO_NotStarted</asp:TableCell>
                    <asp:TableCell><asp:Label ID="lblDTO_NotStarted" runat="server" Text="DTO_NotStarted" /></asp:TableCell>
                    <asp:TableCell>The date and time that is used to signal that something has not started such as the work on an action item</asp:TableCell>
                </asp:TableRow>
                <asp:TableRow>
                    <asp:TableCell>DTO_NotCompleted</asp:TableCell>
                    <asp:TableCell><asp:Label ID="lblDTO_NotCompleted" runat="server" Text="DTO_NotCompleted" /></asp:TableCell>
                    <asp:TableCell>The date and time that is used to signal that something has not been completed such as the work on an action item</asp:TableCell>
                </asp:TableRow>
                <asp:TableRow>
                    <asp:TableCell>DTO_EqAgEarliest</asp:TableCell>
                    <asp:TableCell><asp:Label ID="lblDTO_EqAgEarliest" runat="server" Text="DTO_EqAgEarliest" /></asp:TableCell>
                    <asp:TableCell>If a DLinkBegin date and time of an equipment subcomponent is earlier than this date then the DLinkBegin date of its parent component is to be used instead.</asp:TableCell>
                </asp:TableRow>
                <asp:TableRow>
                    <asp:TableCell>DTO_EqAgLatest</asp:TableCell>
                    <asp:TableCell><asp:Label ID="lblDTO_EqAgLatest" runat="server" Text="DTO_EqAgLatest" /></asp:TableCell>
                    <asp:TableCell>If a DLinkEnd date and time of an equipment subcomponent is later than this date then the DLinkEnd date of its parent component is to be used instead. </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow>
                    <asp:TableCell>dgcVersionUserSelectableSettingsDataTable</asp:TableCell>
                    <asp:TableCell><asp:Label ID="lblSETTINGS_Version" runat="server" Text="dgcVersionUserSelectableSettingsDataTable" /></asp:TableCell>
                    <asp:TableCell>The 'Version' number recorded in the SETTINGS table. </asp:TableCell>
                </asp:TableRow>
                
            </asp:Table>
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
