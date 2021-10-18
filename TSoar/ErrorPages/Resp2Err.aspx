<%@ Page Title="Resp2Err" Language="C#" MasterPageFile="~/mTSoar.Master" AutoEventWireup="true"
    CodeBehind="Resp2Err.aspx.cs" Inherits="TSoar.ErrorPages.Resp2Err" %>

<asp:Content ID="HeaderContent" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="server">
    <asp:Label runat="server" Text="Error Indication" Font-Italic="true" />
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="ContentPlaceHolderMain" runat="server">
<p>A software error occurred in this website. For security reasons, no further information can be disclosed here, except that the error occurred on 
    <asp:Label ID="lblDate" runat="server" /> at <asp:Label ID="lblTime" runat="server" /> UTC. The webmaster has been notified of this error via email.
    <br />
    <asp:Panel ID="pnlOutText" runat="server" Visible="false">
        <asp:Label ID="lblOutText" runat="server" Text="A website administrator may look at the activity log and see the text of this error." />
    </asp:Panel>
</p>

</asp:Content>

<asp:Content ID="FooterContent" ContentPlaceHolderID="ContentPlaceHolderFooter" runat="server">
    <%: Title %>
</asp:Content>
