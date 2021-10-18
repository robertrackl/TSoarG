<%@ Page Title="Members" Language="C#" MasterPageFile="~/mTSoar.Master" AutoEventWireup="true" CodeBehind="Members.aspx.cs" Inherits="TSoar.Accounting.FinDetails.SalesAR.Members" %>

<asp:Content ID="HeaderContent" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="server">
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="ContentPlaceHolderMain" runat="server">
    <a href="../../../MemberPages/Roster/ClubRoster.aspx">List of Members (Club Roster)</a>
</asp:Content>

<asp:Content ID="FooterContent" ContentPlaceHolderID="ContentPlaceHolderFooter" runat="server">
    <%: Title %>
</asp:Content>