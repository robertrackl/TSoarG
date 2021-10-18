<%@ Page Title="TSoar OpsSchedule" Language="C#" MasterPageFile="~/mTSoar.Master" AutoEventWireup="true" CodeBehind="OpsScheduleMaint.aspx.cs" Inherits="TSoar.Board.Operations.OpsSchedule" %>

<asp:Content ID="ContentHeader" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="server">
    <asp:Label runat="server" Text="Maintenance of the Schedule of Operations and Events" Font-Italic="true" />
</asp:Content>

<asp:Content ID="ContentMain" ContentPlaceHolderID="ContentPlaceHolderMain" runat="server">
    <div id="divHRef" runat="server"></div>
</asp:Content>

<asp:Content ID="ContentFooter" ContentPlaceHolderID="ContentPlaceHolderFooter" runat="server">
    <%: Title %>
</asp:Content>
