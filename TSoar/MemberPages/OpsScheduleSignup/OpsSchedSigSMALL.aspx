<%@ Page Title="Operations Schedule, Small Screen" Language="C#" AutoEventWireup="true" MasterPageFile="~/mOpsSched.Master"
    CodeBehind="OpsSchedSigSMALL.aspx.cs" Inherits="TSoar.MemberPages.OpsScheduleSignup.OpsSchedSigSMALL" %>

<%--<asp:Content ID="HeaderContent" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="server">
    <asp:Label runat="server" Text="Flight Operations Schedule with Signups (Small Screen)" Font-Italic="true" />
</asp:Content>--%>

<asp:Content ID="MainContent" ContentPlaceHolderID="ContentPlaceHolderMain" runat="server">
    <asp:SqlDataSource ID="SqlDS_Dates" runat="server" ConnectionString="<%$ ConnectionStrings:SqlConn %>"
        SelectCommand="SELECT FORMAT(Date,'dddd') + ', ' + FORMAT(Date, 'yyyy/MM/dd') + ' ' + sNote AS DDate, ID AS iDate FROM FSDATES WHERE bEnabled=1 ORDER BY Date, sNote" />

    <div class="gvclass">
        Go back to <asp:Button ID="pbMembPgs" runat="server" Text="Member Pages" OnClick="pbMemPgs_Click" />
        <br /><br />
        &nbsp;&nbsp;&nbsp; <asp:Button ID="pbFirst" runat="server" Text="First" OnClick="OneDay_Click" />
        &nbsp;&nbsp;&nbsp; <asp:Button ID="pbEarlier" runat="server" Text="Previous" OnClick="OneDay_Click" />
        &nbsp;&nbsp;&nbsp; <asp:Button ID="pbToday" runat="server" Text="Today/Coming Up" OnClick="OneDay_Click" />
        &nbsp;&nbsp;&nbsp; <asp:Button ID="pbLater" runat="server" Text="Next" OnClick="OneDay_Click" />
        &nbsp;&nbsp;&nbsp; <asp:Button ID="pbLast" runat="server" Text="Last" OnClick="OneDay_Click" />
        <br /><br />
        <asp:DropDownList ID="DDLDate" runat="server" DataSourceID="SqlDS_Dates" DataTextField="DDate" DataValueField="iDate" Font-Bold="true"
            OnSelectedIndexChanged="DDLDate_SelectedIndexChanged" OnPreRender="DDLDate_PreRender" AutoPostBack="true" />
        <br />
        <asp:GridView ID="gvOSSmall" runat="server" AutoGenerateColumns="False"
            GridLines="None" CssClass="SoarNPGridStyle"
            OnRowDataBound="gvOSSmall_RowDataBound"
            AllowPaging="false">
<%--            <PagerSettings Mode="NextPreviousFirstLast" Position="TopAndBottom" Visible="true" FirstPageText="&nbsp;First&nbsp;"
                LastPageText="&nbsp;Last&nbsp;" NextPageText="&nbsp;Next&nbsp;" PreviousPageText="&nbsp;Previous&nbsp;" />
            <PagerStyle CssClass="SoarNPpaging" />--%>
            <Columns>
                <asp:TemplateField HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                    <HeaderTemplate>
                        Kind
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label ID="lblIKind" runat="server" Text='<%# dictCategKinds[(char)Eval("cKind")] %>' />
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField>
                    <HeaderTemplate>
                        Category
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label ID="lblICateg" runat="server" Text='<%# Eval("sCateg") %>' />
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                    <HeaderTemplate>
                        Add / Edit
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:ImageButton ID="ipbAddEdit" runat="server" ImageUrl="~/i/BlueButton.jpg" OnClick="ipbAddEdit_Click"
                            CommandArgument='<%# Eval("iCateg") %>' CommandName='<%# Eval("iPerson") %>' />
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderStyle-HorizontalAlign="Center">
                    <HeaderTemplate>
                        Member Name
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label ID="lblIPerson" runat="server" Text='<%# Eval("sDisplayName") %>' />
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderStyle-HorizontalAlign="Center">
                    <HeaderTemplate>
                        Other Name(s)
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label ID="lblIOther" runat="server" Text='<%# Eval("sNameInSchedule") %>' />
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderStyle-HorizontalAlign="Center">
                    <HeaderTemplate>
                        Remarks
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label ID="lblIRem" runat="server" Text='<%# Eval("sRemarks") %>' />
                    </ItemTemplate>
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

    <div id="ModPopExtDiv">
        <%-- ModalPopupExtender ModPopExt, popping up MPE_Pnl --%>
        <asp:SqlDataSource ID="SqlDS_Members" runat="server" ConnectionString="<%$ ConnectionStrings:SqlConn %>"
         SelectCommand="SELECT ID, sDisplayName FROM PEOPLE WHERE LEN(sUserName) > 0
            UNION SELECT 0 AS ID, '[none]' AS sDisplayName ORDER BY sDisplayName" />
        <asp:LinkButton ID="LinkButton1" runat="server" Text="T" CssClass="displayNone" />
        <ajaxToolkit:ModalPopupExtender ID="ModPopExt" runat="server"
            TargetControlID="LinkButton1" PopupControlID="MPE_Pnl" BackgroundCssClass="background" />
        <asp:Panel ID="MPE_Pnl" runat="server" CssClass="popup" style="display:none;" HorizontalAlign="Center">
            Date: <asp:Label  ID="lblMPEDate" runat="server"   Text="Date" BackColor="#eeb5a2" Font-Bold="true" />
            <br />
            Signup Kind of Category: <asp:Label ID="lblMPEKind" runat="server" Text="Kind" Font-Bold="true" />
            <br />
            Signup Category: <asp:Label ID="lblMPECateg" runat="server" Text="Signup Category" Font-Bold="true" />
            <br />
            Member Name: <asp:Label ID="lblMPEMember" runat="server" Text="Member Name" Font-Bold="true" />
            <br />
            Other Name(s): <asp:TextBox ID="txbOther" runat="server" Text="Other Name(s)" TextMode="SingleLine" />
            <br />
            Remarks:
            <br />
            <asp:TextBox ID="txbRemarks" runat="server" Text="_" Width="250px" Height="60px" TextMode="MultiLine"  />
            <br /><br />
            Signup Action: &nbsp;&nbsp;
            <asp:Button  ID="pbAdd"           runat="server" OnClick="MPE_Click" Text="Add" />&nbsp;&nbsp;
            <asp:Button  ID="pbUpdate"        runat="server" OnClick="MPE_Click" Text="Update" />&nbsp;&nbsp;
            <asp:Button  ID="pbRemove"        runat="server" OnClick="MPE_Click" Text="Remove" />
            <br /><br />
            <asp:Button  ID="pbDismiss" runat="server" OnClick="MPE_Click" Text="Cancel" />
        </asp:Panel>
    </div>

</asp:Content>

<%--<asp:Content ID="FooterContent" ContentPlaceHolderID="ContentPlaceHolderFooter" runat="server">
    <%: Title %>
</asp:Content>--%>
