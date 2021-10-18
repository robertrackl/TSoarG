<%@ Page Title="Board" Language="C#" MasterPageFile="~/mTSoar.Master" AutoEventWireup="true"
    CodeBehind="Board.aspx.cs" Inherits="TSoar.Board.Board" %>

<asp:Content ID="HeaderContent" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="server">
    <asp:Label runat="server" Text="Board Documents" Font-Italic="true" />
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="ContentPlaceHolderMain" runat="server">
    Board Pages:
    <asp:LoginView ID="LoginView2" runat="server" >
        <RoleGroups>
            <asp:RoleGroup Roles="Finance">
                <ContentTemplate>
                    <ul>
                        <li>Meeting Minutes</li>
                        <li>Working Documents</li>
                        <li>Financial Reports</li>
                        <li>Accounting</li>
                    </ul>
                </ContentTemplate>
            </asp:RoleGroup>
            <asp:RoleGroup Roles="BoardMember">
                <ContentTemplate>
                    <ul>
                        <li>Meeting Minutes</li>
                        <li>Working Documents</li>
                        <li>Financial Reports</li>
                    </ul>
                </ContentTemplate>
            </asp:RoleGroup>
            <asp:RoleGroup Roles="Member">
                <ContentTemplate>
                    <ul>
                        <li>Meeting Minutes</li>
                        <li>Financial Reports</li>
                    </ul>
                </ContentTemplate>
            </asp:RoleGroup>
        </RoleGroups>
    </asp:LoginView>
</asp:Content>

<asp:Content ID="FooterContent" ContentPlaceHolderID="ContentPlaceHolderFooter" runat="server">
    <%: Title %>
</asp:Content>
