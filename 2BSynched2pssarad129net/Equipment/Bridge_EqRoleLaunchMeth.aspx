<%@ Page Title="Bridge Equipment Roles to Launch Methods" Language="C#" MasterPageFile="~/mTSoar.Master" AutoEventWireup="true" CodeBehind="Bridge_EqRoleLaunchMeth.aspx.cs" Inherits="TSoar.Equipment.Bridge_EqRoleLaunchMeth" %>

<asp:Content ID="HeaderContent" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="server">
    <asp:Label runat="server" Text="Equipment Roles and Types: Allowable Combinations" Font-Italic="true" />
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="ContentPlaceHolderMain" runat="server">
    <asp:SqlDataSource ID="SqlDS_Roles" runat="server" ConnectionString="<%$ ConnectionStrings:SqlConn %>"
        SelectCommand="SELECT ID, sEquipmentRole AS Role FROM EQUIPMENTROLES ORDER BY sEquipmentRole"  />
    <asp:SqlDataSource ID="SqlDS_LaunchMethods" runat="server" ConnectionString="<%$ ConnectionStrings:SqlConn %>"
        SelectCommand="SELECT ID, sLaunchMethod AS Method FROM LAUNCHMETHODS ORDER BY sLaunchMethod"  />
    <div class="HelpText" ><%-- SCR 213 --%>
        <h4>The Allowable Combinations of Equipment Roles and Launch Methods</h4>
        <p>Only certain combinations of equipment role and launch method make sense. For example, a tow truck cannot be used for aero tow;
            a tow plane cannot be used for auto tow. A glider equipment role would only make sense in conjunction with a self-launching or motor glider.
            By explicitly defining which combinations are allowable ("bridges"), a reduction is achieved
            in the amount of time required searching for equipment while
            <a href="../Statistician/FlightLogInput.aspx">inputting daily flight log data</a> (Statistician website role required). We also reduce the chance of making input data mistakes.
            </p>
        <p>If you are a website administrator, then you can jump to editing the lists of <a href="../AdminPages/DBMaint/DBMaint.aspx">Equipment Roles and Launch Methods</a>.
        </p>
    </div><%-- SCR 213 --%>
    <div class="gvclass">
        <asp:Table runat="server" BorderStyle="Solid" BorderWidth="2" BorderColor="Brown" GridLines="Both" CellPadding="5" CellSpacing="5" BackColor="WhiteSmoke">
            <asp:TableHeaderRow  BorderStyle="Solid" BorderWidth="2">
                <asp:TableHeaderCell  BorderStyle="Solid" BorderWidth="2">
                    Launch Methods
                </asp:TableHeaderCell>
                <asp:TableHeaderCell  BorderStyle="Solid" BorderWidth="2">
                    Allowable Combinations of Launch Methods and Equipment Roles
                </asp:TableHeaderCell>
                <asp:TableHeaderCell  BorderStyle="Solid" BorderWidth="2">
                    Equipment Roles
                </asp:TableHeaderCell>
            </asp:TableHeaderRow>
            <asp:TableRow  BorderStyle="Solid" BorderWidth="2" >
                <asp:TableCell  BorderStyle="Solid" BorderWidth="2">
                    <asp:GridView runat="server" ID="gvLaunchMethods" DataSourceID="SqlDS_LaunchMethods" GridLines="None" CssClass="SoarNPGridStyle" ShowHeaderWhenEmpty="true" Font-Size="Small">
                    </asp:GridView>
                </asp:TableCell>
                <asp:TableCell BorderStyle="Solid" BorderWidth="2">
                    <asp:GridView runat="server" ID="gvLaunchMEqRoles" GridLines="None" CssClass="SoarNPGridStyle" AutoGenerateColumns="false" ShowHeaderWhenEmpty="true"
                        OnRowEditing="gvLaunchMEqRoles_RowEditing"
                        OnRowDataBound="gvLaunchMEqRoles_RowDataBound"
                        OnRowDeleting="gvLaunchMEqRoles_RowDeleting"
                        OnRowCancelingEdit="gvLaunchMEqRoles_RowCancelingEdit"
                        OnRowUpdating="gvLaunchMEqRoles_RowUpdating"
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
                                    <asp:Label ID="lblHsLaunchMethod" runat="server" Text="Launch Method"></asp:Label>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="lblIsLaunchMethod" runat="server" Text='<%# Eval("sLaunchMethod") %>' ></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:DropDownList ID="DDLDLaunchMethod" runat="server" />
                                </EditItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField>
                                <HeaderTemplate>
                                    <asp:Label ID="lblHsEqRole" runat="server" Text="Equipment Role"></asp:Label>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="lblIsEqRole" runat="server" Text='<%# Eval("sEqRole") %>' ></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:DropDownList ID="DDLDEqRole" runat="server" />
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
                    <asp:GridView runat="server" ID="gvEqRoles" DataSourceID="SqlDS_Roles" GridLines="None" CssClass="SoarNPGridStyle" ShowHeaderWhenEmpty="true" Font-Size="Small">
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
