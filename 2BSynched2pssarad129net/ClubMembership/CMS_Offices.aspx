<%@ Page Title="TSoar CMS_Offices" Language="C#" MasterPageFile="~/mTSoar.Master" AutoEventWireup="true"
    CodeBehind="CMS_Offices.aspx.cs" Inherits="TSoar.ClubMembership.CMS_Offices" %>

<asp:Content ID="ContentHeader" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="server">
    <asp:Label runat="server" Text="List of current and past Club Officers" Font-Italic="true" />
</asp:Content>

<asp:Content ID="ContentMain" ContentPlaceHolderID="ContentPlaceHolderMain" runat="server">
<%--    <asp:SqlDataSource ID="SqlDataSrc_People" runat="server" ConnectionString="<%$ ConnectionStrings:SqlConn %>"
        SelectCommand="SELECT ID, sDisplayName FROM PEOPLE ORDER BY sDisplayName" />
    <asp:SqlDataSource ID="SqlDataSrc_BoardOffice" runat="server" ConnectionString="<%$ ConnectionStrings:SqlConn %>"
        SelectCommand="SELECT ID, sBoardOffice FROM BOARDOFFICES ORDER BY sBoardOffice" />--%>
    <asp:GridView ID="gvCMS_Offices" runat="server" AutoGenerateColumns="False"
            GridLines="None" CssClass="SoarNPGridStyle" ShowHeaderWhenEmpty="true"
            OnRowDeleting="gvCMS_Offices_RowDeleting"
            OnRowEditing="gvCMS_Offices_RowEditing"
            OnRowCancelingEdit="gvCMS_Offices_RowCancelingEdit"
            OnRowUpdating="gvCMS_Offices_RowUpdating"
            OnRowDataBound="gvCMS_Offices_RowDataBound"
            AllowPaging="true" PageSize="7"
            OnPageIndexChanging="gvCMS_Offices_PageIndexChanging">
        <PagerSettings Mode="NextPreviousFirstLast" Position="TopAndBottom" Visible="true" FirstPageText="&nbsp;First&nbsp;"
            LastPageText="&nbsp;Last&nbsp;" NextPageText="&nbsp;Next&nbsp;" PreviousPageText="&nbsp;Previous&nbsp;" />
        <PagerStyle CssClass="SoarNPpaging" />
        <Columns>
            <asp:TemplateField HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                <HeaderTemplate>
                    <asp:Label ID="lblHIdent" runat="server" Text="Internal Id" Width="50" ToolTip="Points to a row in database table PEOPLEOFFICES with this ID field contents"></asp:Label>
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:Label ID="lblIIdent" runat="server" Text='<%# Eval("ID") %>' Width="50" ></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>

            <asp:TemplateField HeaderText="&nbsp;Person&nbsp;">
                <ItemTemplate>
                    <asp:Label runat="server" ID="lblFName" Text='<%# Eval("sDisplayName") %>' Width="150"/>
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:DropDownList ID="DDLPerson" runat="server" />
                </EditItemTemplate>
            </asp:TemplateField>

            <asp:TemplateField HeaderText="&nbsp;Board Office&nbsp;">
                <ItemTemplate>
                    <asp:Label runat="server" ID="lblMName" Text='<%# Eval("sBoardOffice") %>' Width="150" />
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:DropDownList ID="DDLBoardOffice" runat="server" />
                </EditItemTemplate>
            </asp:TemplateField>

            <asp:TemplateField HeaderText="&nbsp;Date Began&nbsp;">
                <ItemTemplate>
                    <asp:Label runat="server" ID="lblLName" Text='<%# TSoar.CustFmt.sFmtDate((DateTimeOffset)Eval("DOfficeBegin"),TSoar.CustFmt.enDFmt.DateOnly) %>' TextMode="Date" Width="140" />
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:textBox ID="txbDBegan" runat="server" Text='<%# TSoar.CustFmt.sFmtDate((DateTimeOffset)Eval("DOfficeBegin"),TSoar.CustFmt.enDFmt.DateOnly).Replace("/","-") %>' TextMode="Date" Width="140" />
                </EditItemTemplate>
            </asp:TemplateField>

            <asp:TemplateField HeaderText="&nbsp;Date Ended&nbsp;">
                <ItemTemplate>
                    <asp:Label runat="server" ID="lblDName" Text='<%# TSoar.CustFmt.sFmtDate((DateTimeOffset)Eval("DOfficeEnd"),TSoar.CustFmt.enDFmt.DateOnly) %>' TextMode="Date" Width="140" />
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:textBox ID="txbDEnded" runat="server" Text='<%# TSoar.CustFmt.sFmtDate((DateTimeOffset)Eval("DOfficeEnd"),TSoar.CustFmt.enDFmt.DateOnly).Replace("/","-") %>' TextMode="Date" Width="140" />
                </EditItemTemplate>
            </asp:TemplateField>

            <asp:TemplateField HeaderText="&nbsp;Notes&nbsp;">
                <ItemTemplate>
                    <asp:Label runat="server" ID="lblNotes" Text='<%# Eval("sAdditionalInfo") %>' Width="200" />
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:textBox ID="txbNotes" runat="server" Text='<%# Eval("sAdditionalInfo") %>' Width="200" />
                </EditItemTemplate>
            </asp:TemplateField>

            <asp:TemplateField ItemStyle-HorizontalAlign="Center">
                <HeaderTemplate>
                    Edit
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:ImageButton ID="ipbEEdit" ImageUrl="~/i/BlueButton.jpg" runat="server" CommandName="Edit" OnClientClick="oktoSubmit = true;" />
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:ImageButton ID="ipbEUpdate" ImageUrl="~/i/Update.jpg" runat="server" CommandName="Update" OnClientClick="oktoSubmit = true;" />
                    <asp:ImageButton ID="ipbECancel" ImageUrl="~/i/Cancel.jpg" runat="server" CommandName="Cancel" OnClientClick="oktoSubmit = true;" />
                </EditItemTemplate>
            </asp:TemplateField>

            <asp:TemplateField ItemStyle-HorizontalAlign="Center">
                <HeaderTemplate>
                    Delete
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:ImageButton ID="ipbEDelete" ImageUrl="~/i/RedButton.jpg" runat="server" CommandName="Delete" OnClientClick="oktoSubmit = true;" />
                </ItemTemplate>
                <EditItemTemplate>
                    &nbsp;
                </EditItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>

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
