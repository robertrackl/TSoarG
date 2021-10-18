<%@ Page Title="CMS_ContactEdit" Language="C#" MasterPageFile="~/mTSoar.Master" AutoEventWireup="true" CodeBehind="CMS_ContactEdit.aspx.cs" Inherits="TSoar.ClubMembership.CMS_ContactEdit" %>

<asp:Content ID="ContentHeader" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="server">
    <asp:Label runat="server" Text="Edit Club Member Contact Information" Font-Italic="true" />
</asp:Content>

<asp:Content ID="ContentMain" ContentPlaceHolderID="ContentPlaceHolderMain" runat="server">
    <asp:SqlDataSource ID="SqlDataSrc_Member" runat="server" ConnectionString="<%$ ConnectionStrings:SqlConn %>"
        SelectCommand="SELECT ID,sDisplayName FROM PEOPLE" >
    </asp:SqlDataSource>
    <asp:SqlDataSource ID="SqlDataSrc_Contact" runat="server" ConnectionString="<%$ ConnectionStrings:SqlConn %>"
        SelectCommand="SELECT ID,sPeopleContactType FROM CONTACTTYPES" >
    </asp:SqlDataSource>
    <asp:Panel ID="pnlCMScontact" runat="server" CssClass="popup" HorizontalAlign="Center" Width="550">
        <asp:DetailsView runat="server" ID="dvCMS_Contact" AutoGenerateRows="false"
                DataKeyNames="sDisplayName" AllowPaging="false"
                CellPadding="10" DefaultMode="Insert" HorizontalAlign="Center" CssClass="SoarNPGridStyle"
                OnPreRender="dvCMS_Contact_PreRender">
            <Fields>
                <asp:TemplateField HeaderText="Member Display Name" >
                    <InsertItemTemplate>
                        <asp:DropDownList ID="DDL_Member" runat="server" DataSourceID="SqlDataSrc_Member"
                            DataTextField="sDisplayName" DataValueField="ID" />
                    </InsertItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Type of Contact Data" >
                    <InsertItemTemplate>
                        <asp:DropDownList ID="DDL_ContactType" runat="server" DataSourceID="SqlDataSrc_Contact" AutoPostBack="true"
                            DataTextField="sPeopleContactType" DataValueField="ID" OnSelectedIndexChanged="DDL_ContactType_SelectedIndexChanged" />
                    </InsertItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Valid From Date">
                    <InsertItemTemplate>
                        <asp:TextBox ID="txb_CBegin" runat="server" TextMode="Date" ></asp:TextBox>
                    </InsertItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Valid Until Date">
                    <InsertItemTemplate>
                        <asp:TextBox ID="txb_CEnd" runat="server" TextMode="Date" ></asp:TextBox>
                    </InsertItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField>
                    <HeaderTemplate>
                        <asp:Label runat="server" ID="lblHPA" Text="Physical Address" />
                    </HeaderTemplate>
                    <InsertItemTemplate>
                        <asp:DetailsView ID="dv_PhysAddr" runat="server" BackColor="#eeb5a2" Font-Bold="true" AutoGenerateRows="false"
                                        AllowPaging="false"
                                        CellPadding="10" DefaultMode="Insert" HorizontalAlign="Center" CssClass="SoarNPGridStyle">
                            <Fields>
                                <asp:TemplateField HeaderText="Address 1 *">
                                    <InsertItemTemplate>
                                        <asp:TextBox ID="txbAddress1" runat="server"></asp:TextBox>
                                    </InsertItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Address 2">
                                    <InsertItemTemplate>
                                        <asp:TextBox ID="txbAddress2" runat="server"></asp:TextBox>
                                    </InsertItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="City *">
                                    <InsertItemTemplate>
                                        <asp:TextBox ID="txbCity" runat="server"></asp:TextBox>
                                    </InsertItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="State/Province *">
                                    <InsertItemTemplate>
                                        <asp:TextBox ID="txbState" runat="server"></asp:TextBox>
                                    </InsertItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Postal Code">
                                    <InsertItemTemplate>
                                        <asp:TextBox ID="txbPostalCode" runat="server"></asp:TextBox>
                                    </InsertItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Country *">
                                    <InsertItemTemplate>
                                        <asp:TextBox ID="txbCountry" runat="server"></asp:TextBox>
                                    </InsertItemTemplate>
                                </asp:TemplateField>
                            </Fields>
                        </asp:DetailsView>
                        <p><asp:Label runat="server" ID="lblReqE" Text="* Required Entries" /></p>
                    </InsertItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField>
                    <HeaderTemplate>
                        <asp:Label runat="server" ID="lblContactInfo" Text="Contact Information" />
                    </HeaderTemplate>
                    <InsertItemTemplate>
                        <asp:TextBox ID="txbContactInfo" runat="server" Width ="240" ></asp:TextBox>
                    </InsertItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Ranking of Contact Priority">
                    <InsertItemTemplate>
                        0 <
                        <asp:TextBox ID="txbPriorityRank" runat="server" TextMode="Number" Width="60" CssClass="text-center"
                            ToolTip="Used for sorting the display of contact entries for a member by importance or priority rank. Higher rank appears higher on the list." ></asp:TextBox>
                        < 100
                    </InsertItemTemplate>
                </asp:TemplateField>
            </Fields>
        </asp:DetailsView>
        <br />
        <asp:Button ID="pbSave" runat="server" Text="Save" OnClick="pbSave_Click" />&nbsp;&nbsp;
        <asp:Button ID="pbCancel" runat="server" Text="Cancel" OnClick="pbCancel_Click" />
    </asp:Panel>

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

<asp:Content ID="ContentFooter" ContentPlaceHolderID="ContentPlaceHolderFooter" runat="server">
    <%: Title %>
</asp:Content>
