<%@ Page Title="Database Properties" Language="C#" MasterPageFile="~/mTSoar.Master" AutoEventWireup="true" CodeBehind="DBProperties.aspx.cs" Inherits="TSoar.Developer.DBProperties" %>

<asp:Content ID="HeaderContent" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="server">
    <asp:Label runat="server" Text="For the Website/Software Developer" Font-Italic="true" />
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="ContentPlaceHolderMain" runat="server">
    <h4>SQL Server Instance:</h4>
    <asp:GridView ID="gvSQLProps" runat="server" GridLines="None" CssClass="SoarNPGridStyle" />
    <hr />
    <h4>Database Files:</h4>
    <asp:GridView ID="gvDBProps" runat="server" GridLines="None" CssClass="SoarNPGridStyle" />
</asp:Content>

<asp:Content ID="FooterContent" ContentPlaceHolderID="ContentPlaceHolderFooter" runat="server">
    <%: Title %>
</asp:Content>
