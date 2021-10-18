<%@ Page Title="Test Engineer" Language="C#" MasterPageFile="~/mTSoar.Master" AutoEventWireup="true" CodeBehind="TestEngineer.aspx.cs" Inherits="TSoar.TestEngineer.TestEngineer" %>

<asp:Content ID="HeaderContent" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="server">
    <asp:Label runat="server" Text="Entry Point for the Test Engineer" Font-Italic="true" />
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="ContentPlaceHolderMain" runat="server">
    <div id="divHRef" runat="server">
        <a href="TE_Equipment/TE_Equipment.aspx">Equipment</a>
    </div>
    <div id="divLbl" runat="server">
        The testing function is not available in the production version of this website.
    </div>
</asp:Content>

<asp:Content ID="FooterContent" ContentPlaceHolderID="ContentPlaceHolderFooter" runat="server">
    <%: Title %>
</asp:Content>
