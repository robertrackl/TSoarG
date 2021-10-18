<%@ Page Title="PathRoot" Language="C#" MasterPageFile="~/mTSoar.Master" AutoEventWireup="true" CodeBehind="PathRoot.aspx.cs" Inherits="TSoar.Developer.PathRoot" %>

<asp:Content ID="HeaderContent" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="server">
    <asp:Label runat="server" Text="Root Directory Information" Font-Italic="true" />
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="ContentPlaceHolderMain" runat="server">
    <style>
        .indented {
            padding-left: 50pt; padding-right: 0;
        }
    </style>
    <p>From this current directory: "<asp:Label ID="lblPath" runat="server" Text="PATH" />", we determine this path root:</p>
    <p class="indented"><asp:Label ID="lblRoot" runat="server" Text="ROOT" /></p>
    <p>However, the current directory is a system-level feature; it returns the directory that the server was launched from. It has nothing to do with the website.</p>
    <p>To get the web application's path use System.Web.HttpRuntime.AppDomainAppPath:</p>
    <p class="indented"><asp:Label ID="lblAppPath1" runat="server" Text="APPATH1" /></p>
    <p>Or, use System.Web.HttpContext.Current.Server.MapPath("~"):</p>
    <p class="indented"><asp:Label ID="lblAppPath2" runat="server" Text="APPATH2" /></p>
</asp:Content>

<asp:Content ID="FooterContent" ContentPlaceHolderID="ContentPlaceHolderFooter" runat="server">
    <%: Title %>
</asp:Content>
