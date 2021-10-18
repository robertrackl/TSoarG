<%@ Page Title="Flight Operations" Language="C#" MasterPageFile="~/mTSoar.Master" AutoEventWireup="true" CodeBehind="Operations.aspx.cs" Inherits="TSoar.Operations.Operations" %>

<asp:Content ID="HeaderContent" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="server">
    <asp:Label runat="server" Text="Flight Operations" Font-Italic="true" />
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="ContentPlaceHolderMain" runat="server">
    <div class="HelpText" ><%-- SCR 213 --%>
        <ul>
            <li><a href="OpsSchedule.aspx">Work with Flight Operations Signups</a></li>
            <li><a href="OpsSchedDates.aspx">Work with Flight Operations List of Dates</a></li>
        </ul>
    </div><%-- SCR 213 --%>
</asp:Content>

<asp:Content ID="FooterContent" ContentPlaceHolderID="ContentPlaceHolderFooter" runat="server">
    <%: Title %>
</asp:Content>
