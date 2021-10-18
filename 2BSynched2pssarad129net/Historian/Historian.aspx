<%@ Page Title="Historian" Language="C#" MasterPageFile="~/mTSoar.Master" AutoEventWireup="true" CodeBehind="Historian.aspx.cs" Inherits="TSoar.Historian" %>

<asp:Content ID="ContentHeader" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="server">
    <asp:Label runat="server" Text="The club historian enters significant events for the public to see" Font-Italic="true" />
</asp:Content>

<asp:Content ID="ContentMain" ContentPlaceHolderID="ContentPlaceHolderMain" runat="server">
    <div class="HelpText" ><%-- SCR 213 --%>
        <p>Not yet operational. Will display a list of club events. Has a means of adding and editing details for club events of
            significance to the club's history, for example purchase of a glider, accidents, reports on flying activities away from home,
            check rides, etc.
        </p>
    </div><%-- SCR 213 --%>
</asp:Content>

<asp:Content ID="ContentFooter" ContentPlaceHolderID="ContentPlaceHolderFooter" runat="server">
    <%: Title %>
</asp:Content>
