<%@ Page Title="CompletedDocs" Language="C#" MasterPageFile="~/mTSoar.Master" AutoEventWireup="true" CodeBehind="CompletedDocs.aspx.cs" Inherits="TSoar.Board.Docs.Completed.CompletedDocs" %>

<asp:Content ID="HeaderContent" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="server">
    <asp:Label runat="server" Text="Completed Board Documents" Font-Italic="true" />
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="ContentPlaceHolderMain" runat="server">
    Not yet operational: Area for presenting completed and approved Board Documents (including minutes), not exclusively related to finance
</asp:Content>

<asp:Content ID="FooterContent" ContentPlaceHolderID="ContentPlaceHolderFooter" runat="server">
    <%: Title %>
</asp:Content>
