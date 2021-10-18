<%@ Page Title="Bridge User Roles to Settings" Language="C#" MasterPageFile="~/mTSoar.Master" AutoEventWireup="true" CodeBehind="Bridge_UserRoles_Settings.aspx.cs" Inherits="TSoar.AdminPages.DBMaint.Bridge_UserRoles_Settings" %>

<asp:Content ID="HeaderContent" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="server">
    <asp:Label runat="server" Text="User Roles and User-Selectable Settings: Allowable Combinations" Font-Italic="true" />
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="ContentPlaceHolderMain" runat="server">
    <asp:SqlDataSource ID="SqlDS_UserRoles" runat="server" ConnectionString="<%$ ConnectionStrings:SqlConn %>"
        SelectCommand="SELECT RoleName AS Role FROM aspnet_Roles ORDER BY RoleName"  />
    <asp:SqlDataSource ID="SqlDS_Settings" runat="server" ConnectionString="<%$ ConnectionStrings:SqlConn %>"
        SelectCommand="SELECT ID, sSettingName AS Name, sExplanation AS Explanation, sSettingValue AS Value FROM SETTINGS WHERE bUserSelectable = 1 ORDER BY sSettingName" />
    <div class="HelpText" ><%-- SCR 213 --%>
    <h4>Definition of which User Roles are allowed to modify which User-Selectable Settings</h4>
        <p>Only certain combinations of website user roles and user-selectable settings can be allowed, i.e.,
            a particular role is given permission to modify only a certain collection of settings. For example, the 'Member' role may modify,
            among others, the number of rows displayed in membership rosters. The 'Admin' role is allowed to modify all settings; therefore,
            it need not be given permission explicitly here for any setting.
            </p>
    </div><%-- SCR 213 --%>
    <div class="gvclass">
        <asp:Table runat="server" BorderStyle="Solid" BorderWidth="2">
            <asp:TableHeaderRow  BorderStyle="Solid" BorderWidth="2">
                <asp:TableHeaderCell  BorderStyle="Solid" BorderWidth="2">
                    Website User Roles
                </asp:TableHeaderCell>
                <asp:TableHeaderCell  BorderStyle="Solid" BorderWidth="2">
                    Allowable Combinations of User Roles and Settings
                </asp:TableHeaderCell>
                <asp:TableHeaderCell  BorderStyle="Solid" BorderWidth="2">
                    User-Selectable Settings
                </asp:TableHeaderCell>
            </asp:TableHeaderRow>
            <asp:TableRow  BorderStyle="Solid" BorderWidth="2" >
                <asp:TableCell  BorderStyle="Solid" BorderWidth="2">
                    <asp:GridView runat="server" ID="gvUserRoles" DataSourceID="SqlDS_UserRoles" GridLines="None" CssClass="SoarNPGridStyle" ShowHeaderWhenEmpty="true" Font-Size="Small">
                    </asp:GridView>
                </asp:TableCell>
                <asp:TableCell BorderStyle="Solid" BorderWidth="2">
                    <asp:GridView runat="server" ID="gvURolesSettings" GridLines="None" CssClass="SoarNPGridStyle" AutoGenerateColumns="false" ShowHeaderWhenEmpty="true"
                        OnRowEditing="gvURolesSettings_RowEditing"
                        OnRowDataBound="gvURolesSettings_RowDataBound"
                        OnRowDeleting="gvURolesSettings_RowDeleting"
                        OnRowCancelingEdit="gvURolesSettings_RowCancelingEdit"
                        OnRowUpdating="gvURolesSettings_RowUpdating"
                        Font-Size="Small">
                        <Columns>
                            <asp:TemplateField ItemStyle-HorizontalAlign="Center">
                                <HeaderTemplate>
                                    <asp:Label ID="lblHID" runat="server" Text="Internal Id"></asp:Label>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="lblIID" runat="server" Text='<%# Eval("ID") %>' ></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField>
                                <HeaderTemplate>
                                    <asp:Label ID="lblHUserRoled" runat="server" Text="User Role"></asp:Label>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="lblIUserRole" runat="server" Text='<%# Eval("RoleName") %>' ></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:DropDownList ID="DDLDUserRole" runat="server" />
                                </EditItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField>
                                <HeaderTemplate>
                                    <asp:Label ID="lblHsSetting" runat="server" Text="Setting"></asp:Label>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="lblIsSetting" runat="server" Text='<%# Eval("iSetting").ToString() + " - " + Eval("sSettingName") %>' ></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:DropDownList ID="DDLDSetting" runat="server" />
                                </EditItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField>
                                <HeaderTemplate>
                                    <asp:Label ID="lblHsComments" runat="server" Text="Notes/Comments" ></asp:Label>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="lblIsComments" runat="server" Text='<%# Eval("sComments") %>' ></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox ID="txbDsComments" runat="server" Text='<%# Eval("sComments") %>' ></asp:TextBox>
                                </EditItemTemplate>
                            </asp:TemplateField>

                            <asp:CommandField ButtonType="Image" ShowEditButton="true" HeaderText="Edit" EditImageUrl="~/i/BlueButton.jpg"
                                CancelImageUrl="~/i/Cancel.jpg" UpdateImageUrl="~/i/Update.jpg" >
                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Height="25px" Width="50px" />
                            </asp:CommandField>

                            <asp:CommandField ButtonType="Image" ShowDeleteButton="true" HeaderText="Delete" DeleteImageUrl="~/i/RedButton.jpg">
                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Height="25px" Width="50px" />
                            </asp:CommandField>
                        </Columns>
                    </asp:GridView>
                </asp:TableCell>
                <asp:TableCell BorderStyle="Solid" BorderWidth="2">
                    <asp:GridView runat="server" ID="gvSettings" DataSourceID="SqlDS_Settings" GridLines="None" CssClass="SoarNPGridStyle" ShowHeaderWhenEmpty="true" Font-Size="Small">
                    </asp:GridView>
                </asp:TableCell>
            </asp:TableRow>
        </asp:Table>
    </div>

    <div id="ModalPopupExtender">
        <%-- ModalPopupExtender, popping up MPE_Panel and dynamically populating lblPopupText --%>
        <asp:LinkButton ID="Target" runat="server" Text="T" CssClass="displayNone" />
        <ajaxToolkit:ModalPopupExtender ID="ModalPopExt" runat="server"
            TargetControlID="Target" PopupControlID="MPE_Panel"
            BackgroundCssClass="background"
             />
        <asp:Panel ID="MPE_Panel" runat="server" CssClass="popup" style="display:none;" HorizontalAlign="Center">
            <asp:Label ID="lblPopupText" runat="server" BackColor="#eeb5a2" Font-Bold="true" />
            <br /><br />
            <p> <asp:Button ID="OkButton" runat="server" Text="OK" OnClick="Button_Click" />&nbsp;&nbsp;
                <asp:Button ID="CancelButton" runat="server" Text="Cancel" OnClick="Button_Click" />&nbsp;&nbsp;
                <asp:Button ID="NoButton" runat="server" Text="No" OnClick="Button_Click" />&nbsp;&nbsp;
                <asp:Button ID="YesButton" runat="server" Text="Yes" OnClick="Button_Click"/></p>
        </asp:Panel>
    </div>

</asp:Content>

<asp:Content ID="FooterContent" ContentPlaceHolderID="ContentPlaceHolderFooter" runat="server">
    <%: Title %>
</asp:Content>
