<%@ Page Title="Track Flying Charges" Language="C#" MasterPageFile="~/mTSoar.Master" AutoEventWireup="true" CodeBehind="TrackFlyingCharges.aspx.cs" Inherits="TSoar.Statistician.TrackFlyingCharges" %>

<asp:Content ID="HeaderContent" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="server">
    <asp:Label runat="server" Text="Manage Data for Tracking Flying Charges (TFC)" Font-Italic="true" />
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="ContentPlaceHolderMain" runat="server">
    Flying Charges are tracked in the Accounting section -- <a href="../Accounting/FinDetails/SalesAR/MinFlyChrg.aspx">go to Track Flying Charges</a>
</asp:Content>

<asp:Content ID="FooterContent" ContentPlaceHolderID="ContentPlaceHolderFooter" runat="server">
    <%: Title %>
</asp:Content>