<%@ Page Title="Equipment" Language="C#" MasterPageFile="~/mTSoar.Master" AutoEventWireup="true" CodeBehind="EqMemberPages.aspx.cs" Inherits="TSoar.MemberPages.EquipMaintStat.EqMemberPages" %>

<asp:Content ID="HeaderContent" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="server">
    <asp:Label runat="server" Text="Equipment Operational Status" Font-Italic="true" />
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="ContentPlaceHolderMain" runat="server">
    <div class="HelpText" ><%-- SCR 213 --%>
        <a href="EqMaintStatus.aspx">Equipment Maintenance Status</a>
        <br />
        <a href="EqOpStat.aspx">Equipment Operational Status</a>
    </div><%-- SCR 213 --%>
</asp:Content>

<asp:Content ID="FooterContent" ContentPlaceHolderID="ContentPlaceHolderFooter" runat="server">
    <%: Title %>
</asp:Content>
