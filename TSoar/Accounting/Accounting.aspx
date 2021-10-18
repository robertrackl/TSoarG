<%@ Page Title="Accounting Overview" Language="C#" MasterPageFile="~/mTSoar.Master" AutoEventWireup="true" CodeBehind="Accounting.aspx.cs" Inherits="TSoar.Accounting.Accounting" %>

<asp:Content ID="HeaderContent" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="server">
    <asp:Label runat="server" Text="Accounting Overview" Font-Italic="true" />
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="ContentPlaceHolderMain" runat="server">
    <ul>
        <li><a href="AdminFin/AdminFin.aspx">Accounting Administration</a></li>
        <li><a href="FinDetails/FinDetails.aspx">Bookkeeping</a></li>
        <li><a href="FinReports/FinReports.aspx">Reporting</a></li>
    </ul>
</asp:Content>

<asp:Content ID="FooterContent" ContentPlaceHolderID="ContentPlaceHolderFooter" runat="server">
    <%: Title %>
</asp:Content>
