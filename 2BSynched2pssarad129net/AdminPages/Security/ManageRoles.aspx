<%@ Page Title="TSoar Manage Roles" Language="C#" MasterPageFile="~/mTSoar.Master" AutoEventWireup="true" CodeBehind="ManageRoles.aspx.cs"
    Inherits="TSoar.AdminPages.Security.ManageRoles" %>
<%@ MasterType VirtualPath="~/mTSoar.Master" %>

<asp:Content ID="HeaderContent" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="server">
    <asp:Label runat="server" Text="Website Administration" Font-Italic="true" />
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="ContentPlaceHolderMain" runat="server">
    <div class="text-center">
        <b>Create a New Role: </b>
        <asp:TextBox ID="RoleName" runat="server"></asp:TextBox>
        <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ErrorMessage="Role names must not contain a comma [,]"
            ValidationExpression="^((?!,).)*$" ControlToValidate="RoleName" ForeColor="Red" >
        </asp:RegularExpressionValidator>
        <br />
        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="Role names must not be empty"
            ControlToValidate="RoleName" ForeColor="Red" >
        </asp:RequiredFieldValidator>
        <br />
        <asp:Button ID="CreateRoleButton" runat="server" Text="Create Role" OnClick="CreateRoleButton_Click" />
        <br /><br />
        Table of Currently Defined Roles:
        <br />
        <asp:GridView ID="RoleList" runat="server" AutoGenerateColumns="False" OnRowDeleting="RoleList_RowDeleting"
            OnRowDataBound="RoleList_RowDataBound" HorizontalAlign="Center" >
             <Columns>    
                 <asp:CommandField DeleteText="Delete Role" ShowDeleteButton="True" />
                 <asp:TemplateField HeaderText="Role">    
                     <ItemTemplate>    
                        <asp:Label runat="server" ID="RoleNameLabel" Text='<%# Container.DataItem %>' />    
                     </ItemTemplate>    
                 </asp:TemplateField>    
             </Columns>    
        </asp:GridView>
    </div>
</asp:Content>

<asp:Content ID="FooterContent" ContentPlaceHolderID="ContentPlaceHolderFooter" runat="server">
    <%: Title %>
</asp:Content>
