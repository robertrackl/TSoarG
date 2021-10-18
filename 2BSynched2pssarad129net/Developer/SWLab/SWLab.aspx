<%@ Page Title="TextBoxResearch" Language="C#" MasterPageFile="~/mTSoar.Master" AutoEventWireup="true" CodeBehind="SWLab.aspx.cs" Inherits="TSoar.Developer.SWLab.SWLab" %>

<asp:Content ID="HeaderContent" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="server">
    <asp:Label runat="server" Text="The Developer's Software Laboratory" Font-Italic="true" />
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="ContentPlaceHolderMain" runat="server">
    <ul>
        <li><a href="TextBoxResearch.aspx">Research on TextBox Behavior</a></li>
        <li><a href="TimeAndDate.aspx">Research on TimeAndDate Behavior</a></li>
        <li><a href="PSSA_MySQL.aspx">Interact with PSSA MySQL database</a></li>
        <li><a href="AjaxPractice.aspx">AJAX Practice with ModalPopupExtender and Timers</a></li>
    </ul>
</asp:Content>

<asp:Content ID="FooterContent" ContentPlaceHolderID="ContentPlaceHolderFooter" runat="server">
    <%: Title %>
</asp:Content>
