<%@ Page Title="Finance" Language="C#" MasterPageFile="~/mTSoar.Master" AutoEventWireup="true"
    CodeBehind="Finance.aspx.cs" Inherits="TSoar.Board.Finance.Finance" %>

<asp:Content ID="HeaderContent" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="server">
    <asp:Label runat="server" Text="Board - Finance" Font-Italic="true" />
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="ContentPlaceHolderMain" runat="server">
    Area for Finance / Accounting / Treasury
    <br />
    Has an area where members can inspect the organization's financial status.
    <br />
    Has another area for editing financial/accounting data if member is duly authorized.
</asp:Content>

<asp:Content ID="FooterContent" ContentPlaceHolderID="ContentPlaceHolderFooter" runat="server">
    <%: Title %>
</asp:Content>
