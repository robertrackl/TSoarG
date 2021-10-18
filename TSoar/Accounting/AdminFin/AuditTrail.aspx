<%@ Page Title="Audit Trail" Language="C#" MasterPageFile="~/mTSoar.Master" AutoEventWireup="true"
    CodeBehind="AuditTrail.aspx.cs" Inherits="TSoar.Accounting.AuditTrail" %>

<asp:Content ID="HeaderContent" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="server">
    <asp:Label runat="server" Text="Board - Finance - Administration - Audit Trail" Font-Italic="true" />
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="ContentPlaceHolderMain" runat="server">
    <asp:GridView ID="gvAuditTrail" runat="server" AutoGenerateColumns="True"
        GridLines="None" CssClass="SoarNPGridStyle"
        OnRowCreated="gvAuditTrail_RowCreated"
        OnPageIndexChanging="gvAuditTrail_PageIndexChanging" AllowPaging="true" PageSize="25"
        RowStyle-Font-Size="X-Small" HeaderStyle-Font-Size="Smaller">
        <PagerSettings Mode="NextPreviousFirstLast" Position="TopAndBottom" Visible="true" FirstPageText="&nbsp;First&nbsp;"
            LastPageText="&nbsp;Last&nbsp;" NextPageText="&nbsp;Next&nbsp;" PreviousPageText="&nbsp;Previous&nbsp;" />
        <PagerStyle CssClass="SoarNPpaging" />
    </asp:GridView>
</asp:Content>

<asp:Content ID="FooterContent" ContentPlaceHolderID="ContentPlaceHolderFooter" runat="server">
    <%: Title %>
</asp:Content>
