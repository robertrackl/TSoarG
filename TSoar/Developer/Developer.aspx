<%@ Page Title="For the Software Developer" Language="C#" MasterPageFile="~/mTSoar.Master" AutoEventWireup="true" CodeBehind="Developer.aspx.cs" Inherits="TSoar.Developer.Developer" %>

<asp:Content ID="HeaderContent" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="server">
    <asp:Label runat="server" Text="For the Website/Software Developer" Font-Italic="true" />
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="ContentPlaceHolderMain" runat="server">
<h3>TSoar Software History</h3>
<p>This is a summary. Details are in Excel workbook <B>TSoar_SCR_Tracking_1dr.xlsm</B> .</p>
<table class="SoarNPGridStyle">
    <thead>
        <tr>
            <th>Date</th>
            <th>Deliveries</th>
            <th>Version</th>
        </tr>
    </thead>
    <tbody>
        <!-- #Include virtual="SWHistoryInclude.html" -->
    </tbody>
</table>
    <hr />
    Links:
    <ul>
        <li><a href="Debug/DebugControl.aspx">Debug Control</a></li>
        <li><a href="BrowserProperties.aspx">Browser Properties</a></li>
        <li><a href="DBProperties.aspx">Database Properties</a></li>
        <li><a href="PathRoot.aspx">Path to Root</a></li>
        <li><a href="SWLab/SWLab.aspx">Software Laboratory</a></li>
    </ul>
</asp:Content>

<asp:Content ID="FooterContent" ContentPlaceHolderID="ContentPlaceHolderFooter" runat="server">
    <%: Title %>
</asp:Content>
