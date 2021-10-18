<%@ Page Title="Connect2QBO" Async="true" Language="C#" MasterPageFile="~/mTSoar.Master" AutoEventWireup="true" CodeBehind="Connect2QBO.aspx.cs" Inherits="TSoar.Accounting.AdminFin.Connect2QBO" %>

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
    <h4>Establish a Connection over the Internet to Intuit QuickBooks Online Secured by OAuth2</h4>

    <div id="mainButtons" runat="server" visible ="false">
        <asp:Button ID ="pbConnect" runat="server" Text="Make Connection" OnClick="pbConnect_Click" />
    </div>

    <div id="connected" runat="server" visible ="false">
        <br />
        <p><asp:label runat="server" id="lblConnected" visible="false">The connection to QBO has been established!</asp:label></p> 
        <br />
        <asp:Button id="btnUserInfo" runat="server" Text="Get User Info" OnClick="btnUserInfo_Click" />
        <br />
        <p><asp:label runat="server" id="lblUserInfo" visible="false"></asp:label></p>
        <br />
        <asp:Button id="btnRefresh" runat="server" Text="Refresh Tokens" OnClick="btnRefresh_Click"/>
        <br /><br />
        <asp:Button id="btnRevoke" runat="server" Text="Revoke Tokens"  OnClick="btnRevoke_Click"/>
    </div>

</asp:Content>

<asp:Content ID="FooterContent" ContentPlaceHolderID="ContentPlaceHolderFooter" runat="server">
    <%: Title %>
</asp:Content>