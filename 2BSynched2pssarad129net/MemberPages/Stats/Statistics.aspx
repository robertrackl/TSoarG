<%@ Page Title="Statistics" Language="C#" MasterPageFile="~/mTSoar.Master" AutoEventWireup="true" CodeBehind="Statistics.aspx.cs" Inherits="TSoar.MemberPages.Stats.Statistics" %>

<asp:Content ID="HeaderContent" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="server">
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="ContentPlaceHolderMain" runat="server">
    <div class="HelpText" ><%-- SCR 213 --%>
        <ul>
            <li><a href="ClubStats.aspx">Club Statistics in Tree/Table View</a>: Flight Data with Interactive Filtering</li>
            <li><a href="StatsReports.aspx">Club Statistics in Report Format</a>: Flight Data by Pilot/Aviator and by Glider</li>
        </ul>
    </div><%-- SCR 213 --%>
</asp:Content>

<asp:Content ID="FooterContent" ContentPlaceHolderID="ContentPlaceHolderFooter" runat="server">
    <%: Title %>
</asp:Content>