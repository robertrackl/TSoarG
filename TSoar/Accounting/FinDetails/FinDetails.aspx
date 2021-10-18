<%@ Page Title="Financial Details" Language="C#" MasterPageFile="~/mTSoar.Master" AutoEventWireup="true"
    CodeBehind="FinDetails.aspx.cs" Inherits="TSoar.Accounting.FinDetails.FinDetails" %>

<asp:Content ID="HeaderContent" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="server">
    <asp:Label runat="server" Text="Accounting - Bookkeeping" Font-Italic="true" />
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="ContentPlaceHolderMain" runat="server">
    <p>Pick one of these areas to work in:</p>
    <ul>
        <li><a href="SalesAR/SalesAR.aspx">Sales and Accounts Receivable</a></li>
            <ul>
                <li><a href="SalesAR/Members.aspx">Members</a></li>
                <li><a href="SalesAR/Sales.aspx">Sales from Club Activities</a>
                    <ul>
                        <li><a href="SalesAR/FlyActInvoice.aspx">Create Invoices from Records of Club Members' Flying Activities</a></li>
                        <li><a href="SalesAR/Billing.aspx">Intuit Test</a></li>
                        <li>Sales Receipt</li>
                        <li>Payment Received from Member/Customer</li>
                    </ul>
                </li>
            </ul>
        <li><a href="ExpVendAP/ExpVendAP.aspx">Expenses, Vendors and Accounts Payable</a></li>
            <ul>
                <li><a href="ExpVendAP/Vendors.aspx">List of Vendors</a></li>
                <li><a href="ExpVendAP/Expenses.aspx">Expense Journal</a></li>
                    <ul>
                        <li><a href="ExpVendAP/XactExpense.aspx">Create a New Expense Transaction</a></li>
                    </ul>
                <li>Bill</li>
                <li>Payment Made to Vendor</li>
            </ul>
        <li>Funds Transfer</li>
        <li>Enter Bank Account Starting Balance</li>
    </ul>
</asp:Content>

<asp:Content ID="FooterContent" ContentPlaceHolderID="ContentPlaceHolderFooter" runat="server">
    <%: Title %>
</asp:Content>
