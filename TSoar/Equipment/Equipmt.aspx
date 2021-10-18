<%@ Page Title="" Language="C#" MasterPageFile="~/mTSoar.Master" AutoEventWireup="true" CodeBehind="Equipmt.aspx.cs" Inherits="TSoar.Equipment.Equipment" %>

<asp:Content ID="HeaderContent" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="server">
    <asp:Label runat="server" Text="Equipment and Maintenance" Font-Italic="true" />
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="ContentPlaceHolderMain" runat="server">
    <ul>
        <li><a href="EquipmentList.aspx">List of Equipment</a></li>
        <li><a href="EqComponents.aspx">Components of Equipment</a></li>
        <li><a href="EquipAging\EqAgingIntro.aspx">Equipment Aging Management</a>: Maintenance Schedules, etc.</li>
        <li><a href="EquipRolesTypes.aspx">Bridge between Equipment Roles and Equipment Types</a></li>
        <li><a href="Bridge_EqRoleLaunchMeth.aspx">Bridge between Launch Methods and Equipment Roles</a></li>
    </ul>
</asp:Content>

<asp:Content ID="FooterContent" ContentPlaceHolderID="ContentPlaceHolderFooter" runat="server">
    <%: Title %>
</asp:Content>
