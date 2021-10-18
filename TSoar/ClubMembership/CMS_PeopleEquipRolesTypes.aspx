<%@ Page Title="People and Equipment Roles and Types" Language="C#" MasterPageFile="~/mTSoar.Master" AutoEventWireup="true"
    CodeBehind="CMS_PeopleEquipRolesTypes.aspx.cs" Inherits="TSoar.ClubMembership.CMS_PeopleEquipRolesTypes" %>

<asp:Content ID="HeaderContent" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="server">
    <asp:Label runat="server" Text="Create/modify authorizations for club members to operate equipment" Font-Italic="true" />
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="ContentPlaceHolderMain" runat="server">
    <asp:SqlDataSource ID="SqlDS_RolesTypes" runat="server" ConnectionString="<%$ ConnectionStrings:SqlConn %>"
        SelectCommand="SELECT E.ID, R.sEquipmentRole + ' / ' + T.sEquipmentType + ' [' + E.sComments + ']' AS [Equipment Roles / Types]
                       FROM EQUIPTYPES AS T
                            INNER JOIN EQUIPROLESTYPES AS E ON T.ID = E.iEquipType
                            INNER JOIN EQUIPMENTROLES AS R ON E.iEquipRole = R.ID
                       ORDER BY [Equipment Roles / Types]" />
    <h4>Allowable Combinations of Club Members and Equipment Roles and Types</h4>
    <p>Not all club members are authorized to operate all equipment. This is especially applicable to flying equipment.
        </p>
    <p>
        What combinations make sense of equipment roles and equipment types is defined here:
        <a href="../Equipment/EquipRolesTypes.aspx">Equipment Roles and Types</a> (website user requires the Equipment Membership Role).
        Those need to be defined first; and then you can come back here and specify who is authorized
        to operate each of these equipment role/type combinations.
        </p>
    <p>The reason we do this is to reduce
        the amount of time required when entering daily flight log sheet data here:
        <a href="../Statistician/FlightLogInput.aspx">Input of Flight Log Data</a> (website user requires the Statistician Membership Role).
    </p>
    <div class="gvclass">
        <asp:Table runat="server" BorderStyle="Solid" BorderWidth="2">
            <asp:TableHeaderRow  BorderStyle="Solid" BorderWidth="2">
                <asp:TableHeaderCell  BorderStyle="Solid" BorderWidth="2">
                    Members
                </asp:TableHeaderCell>
                <asp:TableHeaderCell  BorderStyle="Solid" BorderWidth="2">
                    Allowable Combinations of Members, Aviator/Operator Roles, and Equipment Roles/Types
                </asp:TableHeaderCell>
                <asp:TableHeaderCell  BorderStyle="Solid" BorderWidth="2">
                    Equipment Roles and Types
                </asp:TableHeaderCell>
            </asp:TableHeaderRow>
            <asp:TableRow  BorderStyle="Solid" BorderWidth="2" >
                <asp:TableCell  BorderStyle="Solid" BorderWidth="2">
                    There are <asp:Label ID="lblMembersCount" runat="server" Text="0" /> members.
                </asp:TableCell>
                <asp:TableCell BorderStyle="Solid" BorderWidth="2">
                    <asp:Label ID="lbl_ivEditIndex" runat="server" Text="ivEditIndex Debug" /><br />
                    <asp:GridView runat="server" ID="gvPeopleEqRolesTypes" GridLines="None" CssClass="SoarNPGridStyle" AutoGenerateColumns="false" ShowHeaderWhenEmpty="true"
                        ShowFooter="true"
                        OnPreRender="gvPeopleEqRolesTypes_PreRender"
                        OnRowEditing="gvPeopleEqRolesTypes_RowEditing"
                        OnRowDataBound="gvPeopleEqRolesTypes_RowDataBound"
                        OnRowDeleting="gvPeopleEqRolesTypes_RowDeleting"
                        OnRowCancelingEdit="gvPeopleEqRolesTypes_RowCancelingEdit"
                        OnRowUpdating="gvPeopleEqRolesTypes_RowUpdating"
                        Font-Size="Small"
                        OnPageIndexChanging="gvPeopleEqRolesTypes_PageIndexChanging" AllowPaging="true" PageSize="12">
                        <PagerSettings Mode="Numeric" PageButtonCount="10" Position="TopAndBottom" />
<%--                        <PagerSettings Mode="NextPreviousFirstLast" Position="TopAndBottom" Visible="true" FirstPageText="&nbsp;First&nbsp;"
                            LastPageText="&nbsp;Last&nbsp;" NextPageText="&nbsp;Next&nbsp;" PreviousPageText="&nbsp;Previous&nbsp;" />--%>
                        <PagerStyle CssClass="SoarNPpaging" />
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
                                    <asp:Label ID="lblHAvRole" runat="server" Text="Aviator Role"></asp:Label>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="lblIAvRolen" runat="server" Text='<%# Eval("sAviatorRole") %>' ></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:DropDownList ID="DDLDAvRole" runat="server" />
                                </EditItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField>
                                <HeaderTemplate>
                                    <asp:Label ID="lblHPerson" runat="server" Text="Member"></asp:Label>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="lblIPerson" runat="server" Text='<%# Server.HtmlDecode((string)Eval("sDisplayName")) %>' ></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:DropDownList ID="DDLDPerson" runat="server" OnDataBound="DDL_DataBound" />
                                </EditItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField>
                                <HeaderTemplate>
                                    <asp:Label ID="lblHEqRoTy" runat="server" Text="Equipment Role / Type"></asp:Label>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="lblIEqRoTy" runat="server" Text='<%# Eval("sEqRoleType") %>' ></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:DropDownList ID="DDLDEqRoTy" runat="server" />
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
                    <asp:GridView runat="server" ID="gvRolesTypes" DataSourceID="SQLDS_RolesTypes" GridLines="None" CssClass="SoarNPGridStyle"
                        AutoGenerateColumns="false" ShowHeaderWhenEmpty="true" Font-Size="Small">
                        <Columns>
                            <asp:TemplateField ItemStyle-HorizontalAlign="Center">
                                <HeaderTemplate>
                                    Internal ID
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <%# Eval("ID") %>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField>
                                <HeaderTemplate>
                                    Equipment Roles / Types
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <%# Eval("Equipment Roles / Types") %>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
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
