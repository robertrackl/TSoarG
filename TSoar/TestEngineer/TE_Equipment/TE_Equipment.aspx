<%@ Page Title="Test Engineer - Equipment" Language="C#" MasterPageFile="~/mTSoar.Master" AutoEventWireup="true" CodeBehind="TE_Equipment.aspx.cs"
    Inherits="TSoar.TestEngineer.TE_Equipment.TE_Equipment" %>

<asp:Content ID="HeaderContent" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="server">
    <asp:Label runat="server" Text="Entry Point for the Test Engineer" Font-Italic="true" />
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="ContentPlaceHolderMain" runat="server">
    <div id="divHRef" runat="server">
        <a href="TEEq_DataSetup.aspx">Equipment Test Data Setup</a>
    </div>
    <div id="divLbl" runat="server">
        The testing function is not available in the production version of this website.
    </div>
</asp:Content>

<asp:Content ID="FooterContent" ContentPlaceHolderID="ContentPlaceHolderFooter" runat="server">
    <%: Title %>
</asp:Content>
