<%@ Page Title="Statistician" Language="C#" MasterPageFile="~/mTSoar.Master" AutoEventWireup="true" CodeBehind="Statistician.aspx.cs" Inherits="TSoar.Statistician.Statistician1" %>

<asp:Content ID="ContentHeader" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="server">
    <asp:Label runat="server" Text="The area for the Club Statistician" Font-Italic="true" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderMain" runat="server">
    <ul>
        <li><a href="FlightLogInput.aspx">Manually add and edit Flights in grid view</a></li>
        <li><a href="OpsDataInput.aspx">Manually add and edit Flights in tree view</a></li>
        <li><a href="BulkImport.aspx">Bulk Import</a>: Import bulk operational data from a specially formatted text file.</li>
        <li><a href="TrackFlyingCharges.aspx">Track Flying Charges</a>: with a view of determining minimum monthly flying charges</li>
        <li><a href="TiRewardsEdit.aspx">Rewards Journal</a></li>
        <li><a href="TiRewards1Member.aspx">Show Rewards for 1 Member</a></li>
    </ul>
</asp:Content>

<asp:Content ID="FooterContent" ContentPlaceHolderID="ContentPlaceHolderFooter" runat="server">
    <%: Title %>
</asp:Content>
