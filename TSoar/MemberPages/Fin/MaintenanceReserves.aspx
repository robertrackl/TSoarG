<%@ Page Title="Maintenance Reserves" Language="C#" MasterPageFile="~/mTSoar.Master" AutoEventWireup="true" CodeBehind="MaintenanceReserves.aspx.cs" Inherits="TSoar.MemberPages.Fin.MaintenanceReserves" %>

<asp:Content ID="ContentHeader" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="server">
    <asp:Label runat="server" Text="Link to the Maintenance Reserves Excel Workbook" Font-Italic="true" />
</asp:Content>

<asp:Content ID="ContentMain" ContentPlaceHolderID="ContentPlaceHolderMain" runat="server">
    <div class="HelpText" ><%-- SCR 213 --%>
        <p>The history and forecast of the PSSA Maintenance Reserves is kept in an Excel Workbook here:</p>
        <p><a href="https://1drv.ms/x/s!AvoFL8QrGVaTjUnxY7QtcVYDry-L?e=gaVqms" target="_blank">Link to Maintenance Reserves Workbook</a> (opens a new browser tab)</p>
        <p>The link provides read-only access. If you want to play with what-if situations you can download the workbook to your computer and plug in your own values
            as long as you have the Microsoft Excel software.</p>
        <p>The maintenance reserves workbook contains three worksheets:
            <ul>
                <li>Introduction: Explanations about the workbook and the maintenance reserves in general</li>
                <li>Working Ledger: the main worksheet</li>
                <li>Original Ledger: the worksheet used prior to 2017</li>
            </ul>
        </p>
        <p>Author: Robert Rackl</p>
    </div><%-- SCR 213 --%>
</asp:Content>

<asp:Content ID="ContentFooter" ContentPlaceHolderID="ContentPlaceHolderFooter" runat="server">
    <%: Title %>
</asp:Content>
