<%@ Page Title="SalesAR" Language="C#" MasterPageFile="~/mTSoar.Master" AutoEventWireup="true" CodeBehind="SalesAR.aspx.cs" Inherits="TSoar.Accounting.FinDetails.SalesAR.SalesAR" %>

<asp:Content ID="HeaderContent" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="server">
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="ContentPlaceHolderMain" runat="server">
    <p>Pick one of these areas to work in:</p>
            <ul>
                <li><a href="Members.aspx">Members</a></li>
                <li><a href="Sales.aspx">Sales from Club Activities</a>
                    <ul>
                        <li><a href="FlyActInvoice.aspx">Create Invoices from Records of Club Members' Flying Activities</a></li>
                        <li><a href="Billing.aspx">Intuit Test</a></li>
                        <li>Sales Receipt</li>
                        <li>Payment Received from Member/Customer</li>
                    </ul>
                </li>
            </ul>
</asp:Content>

<asp:Content ID="FooterContent" ContentPlaceHolderID="ContentPlaceHolderFooter" runat="server">
    <%: Title %>
</asp:Content>