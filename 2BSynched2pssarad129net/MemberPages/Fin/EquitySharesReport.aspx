<%@ Page Title="Equity Shares Report" Language="C#" MasterPageFile="~/mTSoar.Master" AutoEventWireup="true" CodeBehind="EquitySharesReport.aspx.cs" Inherits="TSoar.MemberPages.Fin.EquitySharesReport" %>

<asp:Content ID="ContentHeader" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="server">
    <asp:Label runat="server" Text="Listing of Equity Shares Transactions; nominal share value is $50" Font-Italic="true" />
</asp:Content>

<asp:Content ID="ContentMain" ContentPlaceHolderID="ContentPlaceHolderMain" runat="server">
    <%-- SCR 227 start --%>
    <%-- <div class="gvclass">
        <asp:GridView ID="gvEquiShRpt" runat="server" AutoGenerateColumns="True"
            GridLines="None" CssClass="SoarNPGridStyle"
            emptydatatext="<<==-- no data found --==>>"
            showheaderwhenempty="true"
            OnRowDataBound="gvEquiShRpt_RowDataBound"
            Font-Size="Small">
        </asp:GridView>
    </div>--%>
    <div class="HelpText" ><%-- SCR 213 start --%>
        <h4>Click below to be taken to a listing of the status of each member's equity shares.</h4>
        <a href="https://1drv.ms/x/s!AvoFL8QrGVaTtSk78U1glDdzyawK?e=X2X7Tx" target="_blank">Open Equity Shares Register (MS Excel Workbook) in browser</a>
        <div style="font-size:smaller">Note: In the upper left corner of the Equity Shares Register worksheet, you may see "#NAME?"
            where the date and/or time is supposed to appear when the workbook was last modified; this is not visible in a browser.
        </div>
        <%-- SCR 227 end --%>
    </div><%-- SCR 213 end --%>
</asp:Content>

<asp:Content ID="ContentFooter" ContentPlaceHolderID="ContentPlaceHolderFooter" runat="server">
    <%: Title %>
</asp:Content>
