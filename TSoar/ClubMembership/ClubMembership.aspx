<%@ Page Title="TSoar Club Membership" Language="C#" MasterPageFile="~/mTSoar.Master" AutoEventWireup="true" CodeBehind="ClubMembership.aspx.cs" Inherits="TSoar.ClubMembership.ClubMembership" %>

<asp:Content ID="ContentHeader" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="server">
    <asp:Label runat="server" Text="The PSSA Club Membership Page" Font-Italic="true" />
</asp:Content>

<asp:Content ID="ContentMain" ContentPlaceHolderID="ContentPlaceHolderMain" runat="server">
    <h3>Perform membership operations:</h3>
    <ul>
        <li><a href="CMS_BasicList.aspx"> Show/edit a list of club members with basic data; add new member</a></li>
        <li><a href="CMS_Contacts.aspx"> Show/edit list of club members with their contact data</a></li>
        <li><a href="CMS_ClubFromTo.aspx"> Show/edit list of club members with their current and historical membership status</a></li>
        <li><a href="CMS_SSA_FromTo.aspx"> Show/edit list of club members with their current and historical SSA member status</a></li>
        <li><a href="CMS_Offices.aspx"> Show/edit list of club members with their current and past Board of Directors Offices</a></li>
        <li><a href="CMS_PeopleEquipRolesTypes.aspx"> Show/edit list of club members with their relation to Equipment Roles and Types</a></li>
        <li><a href="CMS_Qualifs.aspx"> Show/edit list of club members with their piloting qualifications and ratings</a></li>
        <li><a href="CMS_EquiSh.aspx"> Show/edit list of club members with their history of equity shares purchased, sold, or donated</a></li>
    </ul>
</asp:Content>

<asp:Content ID="ContentFooter" ContentPlaceHolderID="ContentPlaceHolderFooter" runat="server">
    <%: Title %>
</asp:Content>
