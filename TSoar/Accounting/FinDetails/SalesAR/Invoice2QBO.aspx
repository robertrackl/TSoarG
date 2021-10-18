<%@ Page Title="Invoice2QBO" Async="true" Language="C#" MasterPageFile="~/mTSoar.Master" AutoEventWireup="true" CodeBehind="Invoice2QBO.aspx.cs" Inherits="TSoar.Accounting.FinDetails.SalesAR.Invoice2QBO" %>

<asp:Content ID="HeaderContent" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="server">
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="ContentPlaceHolderMain" runat="server">
    <style>
        .GridStyle {min-width: 30%; border-color:black;}
    </style>
    <% if (this.dictTokens.ContainsKey("accessToken"))
        {
            Response.Write("<script> window.opener.location.reload();window.close(); </script>");
        }
    %> 
    <h3>Send Flying Activities Invoice to QuickBooks Online via Intuit QBO API secured by OAuth2</h3>
            
    <asp:Table runat="server" BorderStyle="Solid">
        <asp:TableHeaderRow BorderStyle="Solid">
            <asp:TableHeaderCell CssClass="GridStyle">dictTokens[accessToken]</asp:TableHeaderCell>
            <asp:TableHeaderCell CssClass="GridStyle">dictTokens[realmId]</asp:TableHeaderCell>
            <asp:TableHeaderCell CssClass="GridStyle">dictTokens[refreshToken]</asp:TableHeaderCell>
            <asp:TableHeaderCell CssClass="GridStyle">Session[dictTokens]</asp:TableHeaderCell>
            <asp:TableHeaderCell CssClass="GridStyle">Session[oAuthCSRFToken]</asp:TableHeaderCell>
        </asp:TableHeaderRow>
        <asp:TableRow BorderStyle="Solid">
            <asp:TableCell CssClass="GridStyle"><asp:Label ID="lblAccessToken" Text="-0-" runat="server"></asp:Label></asp:TableCell>
            <asp:TableCell CssClass="GridStyle"><asp:Label ID="lblRealmId" Text="-0-" runat="server"></asp:Label></asp:TableCell>
            <asp:TableCell CssClass="GridStyle"><asp:Label ID="lblRefreshToken" Text="-0-" runat="server"></asp:Label></asp:TableCell>
            <asp:TableCell CssClass="GridStyle"><asp:Label ID="lbldictTokens" Text="-0-" runat="server"></asp:Label></asp:TableCell>
            <asp:TableCell CssClass="GridStyle"><asp:Label ID="lbloAuthCSRFToken" Text="-0-" runat="server"></asp:Label></asp:TableCell>
        </asp:TableRow>
    </asp:Table>
    <div id="mainButtons" runat="server" visible ="false">

        <!-- Get App Now -->
        <asp:ImageButton id="btnOpenId" runat="server" AlternateText="Get App Now"
                ImageAlign="left"
                ImageUrl="Images/Get_App.png"
                OnClick="ImgGetAppNow_Click" CssClass="font-size:14px; border: 1px solid grey; padding: 10px; color: red" Height="40px" Width="200px"/>
            <br /><br /><br />
    </div>

    <div id="connected" runat="server" visible ="false">
        <p><asp:label runat="server" id="lblConnected" visible="false">"Your application is connected!"</asp:label></p> 
                 
        <asp:Button id="btnQBOAPICall" runat="server" Text="Call QBO API" OnClick="btnQBOAPICall_Click"/>
            <br />
                
        <p><asp:label runat="server" id="lblQBOCall" visible="false"></asp:label></p>
        <br /><br />

            <asp:Button id="btnUserInfo" runat="server" Text="Get User Info" OnClick="btnUserInfo_Click" />
        <br />

            <p><asp:label runat="server" id="lblUserInfo" visible="false"></asp:label></p>
        <br /><br />

            <asp:Button id="btnRefresh" runat="server" Text="Refresh Tokens" OnClick="btnRefresh_Click"/>
        <br /><br /><br />

        <asp:Button id="btnRevoke" runat="server" Text="Revoke Tokens"  OnClick="btnRevoke_Click"/>
        <br /><br /><br /> 
    </div>
    <br />
    Number of Invoices = <asp:Label ID="lblNumInv" runat="server" Text="-0-" />
    <br />
    <asp:GridView ID="gvInvList" runat="server" >
    </asp:GridView> <br /><br />
    <asp:GridView ID="gvCustList" runat="server" >
    </asp:GridView> <br />
    <asp:Button ID="pbInvoice" runat="server" Text="Invoice Development" OnClick="pbInvoice_Click" /><br />
    Has the Invoice been sent? <asp:Label ID="lblInvoiceSentYesNo" runat="server" Text="NO" />

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

</asp:Content>

<asp:Content ID="FooterContent" ContentPlaceHolderID="ContentPlaceHolderFooter" runat="server">
    <footer>
        <p>TSoar <%: Title %>- &copy; <%: DateTime.Now.Year %>- Rackl and Associates</p>
    </footer>
</asp:Content>