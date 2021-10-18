<%@ Page Title="ExpVendAP - Expenses, Vendors, Accounts Payable" Language="C#" MasterPageFile="~/mTSoar.Master" AutoEventWireup="true" CodeBehind="ExpVendAP.aspx.cs" Inherits="TSoar.Accounting.FinDetails.ExpVendAP.ExpVendAP" %>

<asp:Content ID="HeaderContent" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="server">
    <asp:Label runat="server" Text="Accounting | Bookkeeping | Expenses, Vendors, Accounts Payable" Font-Italic="true" />
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="ContentPlaceHolderMain" runat="server">
    <ul>
        <li><a href="Expenses.aspx">Expense Journal</a></li>
        <li><a href="Vendors.aspx">List of Vendors</a></li>
        <li>Accounts Payable</li>
    </ul>
</asp:Content>

<asp:Content ID="FooterContent" ContentPlaceHolderID="ContentPlaceHolderFooter" runat="server">
    <%: Title %>
</asp:Content>
