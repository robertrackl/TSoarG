<%@ Page Title="TSoar Edit User" Language="C#" MasterPageFile="~/mTSoar.Master" AutoEventWireup="true" CodeBehind="EditUsers.aspx.cs"
    Inherits="TSoar.AdminPages.Security.EditUsers" %>

<asp:Content ID="HeaderContent" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="server">
    <asp:Label runat="server" Text="Website Administration - Edit User" Font-Italic="true" />
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="ContentPlaceHolderMain" runat="server">
    <asp:GridView ID="UserGrid" runat="server" AutoGenerateColumns="False" DataKeyNames="UserName" HorizontalAlign="Center"
        OnRowEditing="UserGrid_RowEditing" OnRowCancelingEdit="UserGrid_RowCancelingEdit" OnRowUpdating="UserGrid_RowUpdating"
        OnRowDeleting="UserGrid_RowDeleting" >
        <Columns>
            <asp:CommandField ShowDeleteButton="True" ShowEditButton="True" />
            <asp:BoundField DataField="UserName" HeaderText="User Name" ReadOnly="True" />
            <asp:BoundField DataField="LastLoginDate" DataFormatString="{0:d}" HeaderText="Last Login Date" HtmlEncode="False" ReadOnly="True" />

            <asp:TemplateField HeaderText="Email">    
                 <ItemTemplate>    
                      <asp:Label runat="server" ID="Label1" Text='<%# Eval("Email") %>'></asp:Label>    
                 </ItemTemplate>    
                 <EditItemTemplate>    
                      <asp:TextBox runat="server" ID="Email" Text='<%# Bind("Email") %>'></asp:TextBox>    

                      <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server"    
                           ControlToValidate="Email" Display="Dynamic"    
                           ErrorMessage="You must provide an email address." 
                           SetFocusOnError="True">*</asp:RequiredFieldValidator>    

                      <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server"    
                           ControlToValidate="Email" Display="Dynamic"    
                           ErrorMessage="The email address you have entered is not valid. Please fix 
                           this and try again."    
                           SetFocusOnError="True"    
                           ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*">*
                      </asp:RegularExpressionValidator>    
                 </EditItemTemplate>    
            </asp:TemplateField>

            <asp:TemplateField HeaderText="Comment">    
                 <ItemTemplate>    
                      <asp:Label runat="server" ID="Label2" Text='<%# Eval("Comment") %>'></asp:Label>    
                 </ItemTemplate>    
                 <EditItemTemplate>    
                      <asp:TextBox runat="server" ID="Comment" TextMode="MultiLine"
                           Columns="40" Rows="4" Text='<%# Bind("Comment") %>'>
                      </asp:TextBox>    
                 </EditItemTemplate>    
            </asp:TemplateField>

        </Columns>
    </asp:GridView>
    <asp:ValidationSummary ID="ValidationSummary1"
               runat="server"
               ShowMessageBox="True"
               ShowSummary="False" />
</asp:Content>

<asp:Content ID="FooterContent" ContentPlaceHolderID="ContentPlaceHolderFooter" runat="server">
    <%: Title %>
</asp:Content>

