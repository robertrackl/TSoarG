<%@ Page Title="Account Profile TSoar Settings" Language="C#" MasterPageFile="~/mTSoar.Master" AutoEventWireup="true"
    CodeBehind="APTSettings.aspx.cs" Inherits="TSoar.Developer.SWLab.APTSettings" %>

<asp:Content ID="HeaderContent" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="server">
    <asp:Label runat="server" Text="Test and Improve APTSoarSettings" Font-Italic="true" />
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="ContentPlaceHolderMain" runat="server">
    <p>In file AccountProfile.cs there exists a class called APTSoarSettings.
        It provides access to the user-selectable settings whose names and initial values are defined in database table SETTINGS.
        When a website user does not yet have in his profile a table of these settings, then a new instance of class
        APTSoarSettings is created and its internal DataTable APTS is filled from the initial values given in table SETTINGS
        using the fields sSettingName and sSettingValue.
    </p>
    <p>Class APTSoarSettings has several methods:
        <UL>
            <li>
                public string <b>sGetUSSetting</b>(string suSettingName) <br />
                Gets a setting value given the setting name
            </li>
            <li>
                public void <b>SetUSSetting</b>(string suSettingName, string suSettingValue) <br />
                Sets a setting value for the supplied setting name
            </li>
            <li>
                public decimal <b>dGetVersion</b>()<br />
                Gets the version embedded in DataTable APTS
            </li>
            <li>
                public bool <b>bVersionSynch</b>()<br />
                Synchronizes the contents of APTS with database table SETTINGS; if necessary, APTS is modified
                and 'true' is returned; 'false' is returned when no modification was required. Settings not touched
                by this synchronization keep their values; new settings added take their value from field sSettingValue in table SETTINGS.
            </li>
        </UL>
    </p>
    <p>Let's test those methods:</p>
    <br /><asp:Button ID="pbRunGetTest" runat="server" Text="Run Get Test" OnClick="pbRunGetTest_Click" />
    <br />A call to sGetUSSetting(<asp:TextBox ID="txbsNameGet" runat="server" />) results in '<asp:Label ID="lblsValue" runat="server" />'.
    <br /><br />
    <br />Version = <asp:Label ID="lblVersion" runat="server" />, Settings Count = <asp:Label ID="lblCount" runat="server" />
    <br /><br />
    <br /><asp:Button ID="pbRunSetTest" runat="server" Text="Run Set Test" OnClick="pbRunSetTest_Click" />
    <br />Call sSetUSSetting with suSettingName=<asp:TextBox ID="txbsNameSet" runat="server" /> and
        suSettingValue=<asp:TextBox ID="txbsValue" runat="server" />
    <br />Check: sGetUSSetting(<asp:Label ID="lblCheckG" runat="server" />) results in '<asp:Label ID="lblCheckV" runat="server" />'.
</asp:Content>

<asp:Content ID="FooterContent" ContentPlaceHolderID="ContentPlaceHolderFooter" runat="server">
    <%: Title %>
</asp:Content>
