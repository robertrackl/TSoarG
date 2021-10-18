<%@ Page Title="Sales" Language="C#" MasterPageFile="~/mTSoar.Master" AutoEventWireup="true" CodeBehind="Sales.aspx.cs" Inherits="TSoar.Accounting.FinDetails.SalesAR.Sales" %>

<asp:Content ID="HeaderContent" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="server">
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="ContentPlaceHolderMain" runat="server">
    List of Sales from Club Activities
                    <ul>
                        <li><a href="FlyActInvoice.aspx">Create Invoices from Records of Club Members' Flying Activities</a></li>
                        <li><a href="Billing.aspx">Intuit Test</a></li>
                        <li>Sales Receipt</li>
                        <li>Payment Received from Member/Customer</li>
                    </ul>
</asp:Content>

<asp:Content ID="FooterContent" ContentPlaceHolderID="ContentPlaceHolderFooter" runat="server">
    <%: Title %>
</asp:Content>