<%@ Page Title="TSoar Website Security" Language="C#" MasterPageFile="~/mTSoar.Master" AutoEventWireup="true" CodeBehind="Security.aspx.cs"
    Inherits="TSoar.AdminPages.Security.Security" %>
<%@ MasterType VirtualPath="~/mTSoar.Master" %>

<asp:Content ID="HeaderContent" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="server">
    <asp:Label runat="server" Text="Website Administration - Security" Font-Italic="true" />
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="ContentPlaceHolderMain" runat="server">
    <br />
    <asp:Label ID="Label1" runat="server" Font-Size="Larger" Text="Website Security (Users and Roles)"></asp:Label>
    <br />
    <a href="UsersAndRoles.aspx">Users and Roles</a>
    <br />
    <a href="CreateUserWizardWithRoles.aspx">Create new user</a>
    <br />
    <a href="EditUsers.aspx">List, update, or delete (edit) users</a>
    <br />
    <a href="ManageRoles.aspx">List, add, or delete roles</a>
    <br />
    <a href="ResetPwd.aspx">Reset the password for a user</a>
</asp:Content>

<asp:Content ID="FooterContent" ContentPlaceHolderID="ContentPlaceHolderFooter" runat="server">
    <%: Title %>
</asp:Content>
