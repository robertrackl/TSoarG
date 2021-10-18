<%@ Page Title="Login" Language="C#" MasterPageFile="~/mTSoar.Master" AutoEventWireup="true"
    CodeBehind="Login.aspx.cs" Inherits="TSoar.Login" %>
<%@ MasterType VirtualPath="~/mTSoar.Master" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="ContentPlaceHolderHeader">
    <asp:Label runat="server" Text="The PSSA Login Page" Font-Italic="true" />
</asp:Content>

<asp:Content ID="bodyContent" ContentPlaceHolderID="ContentPlaceHolderMain" runat="server">
    <div style="text-align:center">
      <div style="width: 400px; margin-left: auto; margin-right:auto;">
        <asp:Login ID="Login2" runat="server" Font-Size="12pt" Font-Bold="True" TitleText="Enter Login Credentials"
            MembershipProvider="SqlMembershipProvider" RenderOuterTable="False" DestinationPageUrl="~/Default.aspx"
            OnLoggedIn="Login2_LoggedIn" OnLoginError="Login2_LoginError">
            <TitleTextStyle BackColor="#6B696B" Font-Bold="True" ForeColor="#FFFFFF" />
            <TextBoxStyle BackColor="#F7F7DE" BorderColor="#CCCC99" BorderStyle="Solid" BorderWidth="1px" Font-Names="Verdana" Font-Size="12pt" />
            <LabelStyle BackColor="#F7F7DE" BorderColor="#CCCC99" BorderStyle="Solid" BorderWidth="1px" Font-Names="Verdana" Font-Size="12pt" />
            <CheckBoxStyle BackColor="#F7F7DE" BorderColor="#CCCC99" BorderStyle="Solid" BorderWidth="1px" Font-Names="Verdana" Font-Size="12pt" />
            <LoginButtonStyle BackColor="#F7F7DE" BorderColor="#CCCC99" BorderStyle="Solid" BorderWidth="1px" Font-Names="Verdana" Font-Size="12pt" />
        </asp:Login>
      </div>
        <asp:ValidationSummary ID="ValidationSummary2" runat="server" ValidationGroup="Login2" />
        <br />
        In order to obtain a login user name and password, please contact <a href="mailto:robertrackl@rad129.net">robertrackl@rad129.net</a> .<br />
        <br />
        Click
        <asp:HyperLink ID="HyperLink3" runat="server" NavigateUrl="~/PwdRecov.aspx">here if you forgot your password</asp:HyperLink>
        &nbsp;and want a new one emailed to you (you will need your user name for this web site).<br />
        <br />
        <asp:HyperLink ID="HyperLink4" runat="server" NavigateUrl="~/Default.aspx">Go to Home page</asp:HyperLink>
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
