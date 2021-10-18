<%@ Page Title="" Language="C#" MasterPageFile="~/mTSoar.Master" AutoEventWireup="true" CodeBehind="ThrowException.aspx.cs" Inherits="TSoar.Developer.ThrowException" %>

<asp:Content ID="HeaderContent" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="server">
    <asp:Label runat="server" Text="Work with Towpilot and Instructor Rewards" Font-Italic="true" />
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="ContentPlaceHolderMain" runat="server">
    <asp:Button ID="pbGeneral" runat="server" Text="Throw general exception" OnClick="pbGeneral_Click" />
    <hr />
    <asp:Button ID="pbexcToPopup" runat="server" Text="Throw the `excToPopup` exception" OnClick="pbexcToPopup_Click" />
    <hr />
    <asp:Button ID="pbexcToPopupInner" runat="server" Text="Throw 'excToPopup' with an inner exception" OnClick="pbexcToPopupInner_Click" />
    <hr />
    <asp:Button ID="pbHttpExc" runat="server" Text="Throw 'HttpException' exception" OnClick="pbHttpExc_Click" />
</asp:Content>

<asp:Content ID="FooterContent" ContentPlaceHolderID="ContentPlaceHolderFooter" runat="server">
    <%: Title %>
</asp:Content>
