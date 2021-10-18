<%@ Page Title="Manage Settings" Language="C#" MasterPageFile="~/mTSoar.Master" AutoEventWireup="true" CodeBehind="ManageSettings.aspx.cs" Inherits="TSoar.MemberPages.ManageSettings.ManageSettings" %>

<asp:Content ID="HeaderContent" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="server">
    <br />
    <asp:Label runat="server" Text="Display and Modify User-Selectable Settings" Font-Italic="true" />
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="ContentPlaceHolderMain" runat="server">
    <asp:SqlDataSource ID="SqlDS_Value" runat="server" ConnectionString="<%$ ConnectionStrings:SqlConn %>"  />
    <h4>Manage User-Selectable Settings</h4>
    <div class="gvclass">
        <asp:GridView ID="gvUSS" runat="server" AutoGenerateColumns="False"
            GridLines="None" CssClass="SoarNPGridStyle"
            OnRowDataBound="gvUSS_RowDataBound"
            ShowHeaderWhenEmpty="true"
            Font-Size="Small">
            <Columns>
                <asp:TemplateField>
                    <HeaderTemplate>
                        <asp:Label ID="lblHName" runat="server" Text="Setting Name" />
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label ID="lblIName" runat="server" Text='<%# Eval("sSettingName") %>' />
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField>
                    <HeaderTemplate>
                        <asp:Label ID="lblHExpl" runat="server" Text="Explanation" />
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label ID="lblIExpl" runat="server" Text='<%# Eval("sExplanation") %>' />
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField>
                    <HeaderTemplate>
                        <asp:Label ID="lblHValue" runat="server" Text="Setting Value" />
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label ID="lblIValue" runat="server" Text='<%# Eval("sSettingValue") %>' />
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:TextBox ID="txbValue" runat="server" Text='<%# Eval("sSettingValue") %>' />
                        <asp:RangeValidator ID="RgVValue" runat="server" ControlToValidate="txbValue"
                            Display="Dynamic" ForeColor="Red" BorderStyle="Dotted" BorderColor="Red" BorderWidth="4px" />
                        <asp:RegularExpressionValidator ID="RegExValue" runat="server" ControlToValidate="txbValue"
                            Display="Dynamic" ForeColor="Red" BorderStyle="Dotted" BorderColor="Red" BorderWidth="4px"/>
                        <asp:DropDownList ID="DDLValue" runat="server" Width="180" />
                    </EditItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField>
                    <HeaderStyle HorizontalAlign="Center" />
                    <HeaderTemplate>
                        <asp:Label ID="lblHEdit" runat="server" Text="Edit / Update"></asp:Label>
                    </HeaderTemplate>
                    <ItemStyle HorizontalAlign="Center" />
                    <ItemTemplate>
                        <asp:ImageButton ID="pbEdit" runat="server" ImageUrl="~/i/BlueButton.jpg" OnClick="pbEdit_Click" CssClass="text-center" />
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:ImageButton ID="pbUpdate" runat="server" ImageUrl="~/i/Update.jpg" OnClick="pbUpdate_Click" />
                        <asp:ImageButton ID="pbCancel" runat="server" ImageUrl="~/i/Cancel.jpg" OnClick="pbCancel_Click" />
                    </EditItemTemplate>
                </asp:TemplateField>

            </Columns>
        </asp:GridView>
    </div>

    <div>
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
