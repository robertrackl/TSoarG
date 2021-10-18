<%@ Page Title="Reset Password" Language="C#" MasterPageFile="~/mTSoar.Master" AutoEventWireup="true" CodeBehind="ResetPwd.aspx.cs" Inherits="TSoar.AdminPages.Security.ResetPwd" %>

<asp:Content ID="HeaderContent" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="server">
    <asp:Label runat="server" Text="Website Administration - Reset Password for a Membership User" Font-Italic="true" />
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="ContentPlaceHolderMain" runat="server">
    <asp:SqlDataSource ID="SqlDataSrc_Users" runat="server"  ConnectionString="<%$ ConnectionStrings:SqlConn %>"
        SelectCommand="SELECT UserName FROM aspnet_Users ORDER BY UserName" />
    Reset password for which user?:
    <br />
    <asp:DropDownList ID="DDLUsers" runat="server" DataSourceID="SqlDataSrc_Users" DataTextField="UserName"
                      Width="120" OnSelectedIndexChanged="DDLUsers_SelectedIndexChanged" AutoPostBack="true" />
    <br />
    <asp:Button ID="pbResetPwd" runat="server" Text="Reset the Password" OnClick="pbResetPwd_Click" />
    <asp:Panel id="pnlSure" runat="server" Visible="false">
        Are you sure you want to have a new password created for website user `<asp:Label ID="lblUser" runat="server" />`?
        <br />
        <asp:Button ID="pbCancel" runat="server" Text="Cancel" OnClick="pbCancel_Click" />
        <asp:Button ID="pbOK" runat="server" Text="OK" OnClick="pbOK_Click" />
    </asp:Panel>
    <asp:Panel ID="pnlNewPwd" runat="server" Visible="false">
        The new password is: <asp:Label ID="lblNewPwd" runat="server" Text="" />
    </asp:Panel>
    <asp:Panel ID="pnlErr" runat="server" Visible="false">
        <asp:Label ID="lblErr" runat="server" Text="none" />
    </asp:Panel>
</asp:Content>

<asp:Content ID="FooterContent" ContentPlaceHolderID="ContentPlaceHolderFooter" runat="server">
    <%: Title %>
</asp:Content>

