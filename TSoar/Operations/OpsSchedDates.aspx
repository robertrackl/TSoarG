<%@ Page Title="Flight Ops Schedule Dates" Language="C#" MasterPageFile="~/mTSoar.Master" AutoEventWireup="true" CodeBehind="OpsSchedDates.aspx.cs" Inherits="TSoar.Operations.OpsSchedDates" %>

<asp:Content ID="HeaderContent" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="server">
    <asp:Label runat="server" Text="Dates for Flight Operations Schedule" Font-Italic="true" />
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="ContentPlaceHolderMain" runat="server">
    <div class="HelpText" ><%-- SCR 213 --%>
        <p>Use this web page to define the dates for which signups are solicited.</p>
        <p><a href="Operations.aspx">Go to 'Operations'</a></p>
        <p><a href="OpsSchedule.aspx"> Work with Signups </a></p>
        <p>Add a year's worth of weekend dates from March to October at the end of the existing list: 
            <asp:Button ID="pbAddDates" runat="server" Text="Add Dates" OnClick="pbAddDates_Click" /></p>
    </div><%-- SCR 213 --%>
    <div class="gvclass">
        <asp:GridView ID="gvFOSDates" runat="server" AutoGenerateColumns="False"
            GridLines="None" CssClass="SoarNPGridStyle"
            OnRowEditing="gvFOSDates_RowEditing" OnRowDataBound="gvFOSDates_RowDataBound"
            OnRowDeleting="gvFOSDates_RowDeleting" OnRowUpdating="gvFOSDates_RowUpdating"
            OnRowCancelingEdit="gvFOSDates_RowCancelingEdit" OnPageIndexChanging="gvFOSDates_PageIndexChanging"
            AllowPaging="true" PageSize="35">
            <PagerSettings Mode="NextPreviousFirstLast" Position="TopAndBottom" Visible="true" FirstPageText="&nbsp;First&nbsp;"
                LastPageText="&nbsp;Last&nbsp;" NextPageText="&nbsp;Next&nbsp;" PreviousPageText="&nbsp;Previous&nbsp;" />
            <PagerStyle CssClass="SoarNPpaging" />
            <Columns>
                <asp:TemplateField HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                    <HeaderTemplate>
                        <asp:Label ID="lblHIdent" runat="server" Text="Internal Id" Width="60" 
                            ToolTip="Points to a row in database table FSDATES with this ID field contents"></asp:Label>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label ID="lblIIdent" runat="server" Text='<%# Eval("ID") %>' Width="60" ></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField >
                    <HeaderTemplate>
                        <asp:Label ID="lblHDate" runat="server" Text="Date"
                            ToolTip="Date of the day of flight operations."></asp:Label>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label runat="server" ID="lblIDate" Text='<%#  TSoar.CustFmt.sFmtDate(((DateTime)Eval("Date")),TSoar.CustFmt.enDFmt.DateOnly) %>' />
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:TextBox ID="txbDDate" runat="server" Text='<%# TSoar.CustFmt.sFmtDate(((DateTime)Eval("Date")),TSoar.CustFmt.enDFmt.DateOnly).Replace("/","-") %>'
                            TextMode="Date" Font-Size="X-Small" Width="110px" ></asp:TextBox>
                    </EditItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField >
                    <HeaderTemplate>
                        <asp:Label ID="lblHDOW" runat="server"
                            ToolTip="Day of Week."></asp:Label>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label runat="server" ID="lblIDOW" Text='<%# ((DateTime)Eval("Date")).ToString("dddd") %>' />
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" >
                    <HeaderTemplate>
                        <asp:Label ID="lblHEnabled" runat="server" Text="Enabled?" />
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:CheckBox runat="server" ID="chbIEnabled" Checked='<%# Eval("bEnabled") %>' Enabled="false" />
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:CheckBox runat="server" ID="chbDEnabled" Checked='<%# Eval("bEnabled") %>' Enabled="true" />
                    </EditItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField ItemStyle-Width="120" HeaderStyle-HorizontalAlign="Center" >
                    <HeaderTemplate>
                        <asp:Label ID="lblHComment" runat="server" Text="Notes" />
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label runat="server" ID="lbliComment" Text='<%# Eval("sNote") %>'/>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:TextBox ID="txbEComment" runat="server" Text='<%# Eval("sNote") %>'>
                        </asp:TextBox>
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
