<%@ Page Title="DBIntegrity" Language="C#" MasterPageFile="~/mTSoar.Master" AutoEventWireup="true" CodeBehind="DBIntegrity.aspx.cs" Inherits="TSoar.AdminPages.DBMaint.DBIntegrity" %>

<asp:Content ID="HeaderContent" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="server">
    <br />
    <asp:Label runat="server" Text="Run Checks in order to ensure the database's integrity" Font-Italic="true" />
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="ContentPlaceHolderMain" runat="server">
    <div class="row">
        <div class="col-sm-3"><asp:Button ID="pbSettings" runat="server" Text="Settings" OnClick="pbSettings_Click"
            ToolTip="Check consistency of values in table SETTINGS in conjunction with the contents of other tables" /></div>
        <div class="col-sm-3"><asp:Button ID="pbRoles" runat="server" Text="Website Security Roles" OnClick="pbRoles_Click"
            ToolTip="Check for presence of certain user Roles required by the software underlying this website" /></div>
        <div class="col-sm-3"><asp:Button ID="pbEquip" runat="server" Text="Equipment" OnClick="pbEquip_Click"
            ToolTip="Check for data consistency in the tables in the Equipment area" /></div>
        <div class="col-sm-3"><asp:Button ID="pbMiscell" runat="server" Text="Miscellaneous" OnClick="pbMiscell_Click"
            ToolTip="Check for the presence of several miscellaneous values" /></div>
    </div>

    <div id="ModalPopupExtender">
        <%-- ModalPopupExtender, popping up MPE_Panel and dynamically populating lblPopupText --%>
        <asp:LinkButton ID="Target" runat="server" Text="T" CssClass="displayNone" />
        <ajaxToolkit:ModalPopupExtender ID="ModalPopExt" runat="server"
            TargetControlID="Target" PopupControlID="MPE_Panel"
            BackgroundCssClass="background"
             />
        <asp:Panel ID="MPE_Panel" runat="server" CssClass="popup" style="display:none;" HorizontalAlign="Center" Width="750px">
            <asp:TextBox ID="txbPopup" runat="server" Text="" ReadOnly="true" TextMode="MultiLine" style="min-width: 100%;" Height="700px" CssClass="rightAlign" />
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
