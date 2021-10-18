<%@ Page Title="TSoar Users and Roles" Language="C#" MasterPageFile="~/mTSoar.Master" AutoEventWireup="true" CodeBehind="UsersAndRoles.aspx.cs"
    Inherits="TSoar.AdminPages.Security.UsersAndRoles" %>
<%@ MasterType VirtualPath="~/mTSoar.Master" %>

<asp:Content ID="HeaderContent" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="server">
    <asp:Label runat="server" Text="Website Administration - Users and Roles" Font-Italic="true" />
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="ContentPlaceHolderMain" runat="server">
    <div class="row">
        <div class="col-sm-6 text-center">
            <asp:Label ID="ActionStatus" runat="server" Text=" " Font-Size="Larger" ForeColor="Red"></asp:Label> 
            <b>Select a User:</b>
            <asp:DropDownList ID="UserList" runat="server" AutoPostBack="True" 
                 DataTextField="UserName" DataValueField="UserName" OnSelectedIndexChanged="UserList_SelectedIndexChanged">
            </asp:DropDownList> 
            <br />  <br />
            <asp:Repeater ID="UsersRoleList" runat="server"> 
                 <ItemTemplate> 
                      <asp:CheckBox runat="server" ID="RoleCheckBox" AutoPostBack="true" 
                            Text='<%# Container.DataItem %>' OnCheckedChanged="RoleCheckBox_CheckChanged" /> 
                      <br /> 
                  </ItemTemplate> 
            </asp:Repeater>
        </div>
<%--        <hr style="color:darkgrey" />--%>
        <div class="col-sm-6 text-center">
            <b>Select a Role:</b>
            <asp:DropDownList ID="RoleList" runat="server" AutoPostBack="true" OnSelectedIndexChanged="RoleList_SelectedIndexChanged" >
            </asp:DropDownList>
        </div>
        <br /><br />
        <asp:GridView ID="RolesUserList" runat="server" AutoGenerateColumns="False" SelectedRowStyle-Height="25px"
            HeaderStyle-Height="30px" Height="80px"
            EmptyDataText="No users belong to this role." HorizontalAlign="Center" OnRowDeleting="RolesUserList_RowDeleting"
            BorderWidth="1px" RowStyle-BorderWidth="1px" GridLines="Both" SortedAscendingCellStyle-Height="25px"
            SortedAscendingHeaderStyle-Height="30px" SortedDescendingCellStyle-Height="25px" SortedDescendingHeaderStyle-Height="30px">
            <RowStyle Height="25px"/>
            <Columns>
                <asp:CommandField DeleteText="Remove" ShowDeleteButton="True" />
                <asp:TemplateField HeaderText="Users"> 
                    <ItemTemplate> 
                        <asp:Label runat="server" id="UserNameLabel" 
                            Text='<%# Container.DataItem %>'></asp:Label> 
                    </ItemTemplate> 
                </asp:TemplateField> 
            </Columns> 
         </asp:GridView>
     </div>
</asp:Content>

<asp:Content ID="FooterContent" ContentPlaceHolderID="ContentPlaceHolderFooter" runat="server">
    <%: Title %>
</asp:Content>
