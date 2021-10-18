<%@ Page Title="EqOperStatus" Language="C#" MasterPageFile="~/mTSoar.Master" AutoEventWireup="true" CodeBehind="EqOperStatus.aspx.cs"
    Inherits="TSoar.MemberPages.EquipMaintStat.EqOperStatus" %>

<asp:Content ID="HeaderContent" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="server">
    <asp:Label runat="server" Text="Equipment Operational Status - Detailed List" Font-Italic="true" />
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="ContentPlaceHolderMain" runat="server">
    <style>
        .maxWidthGrid {
            max-width: 350px;
            overflow: hidden;
        }
    </style>
    <div>
        <a href="EqOperStatCompact.aspx">Show Compact List of Equipment Status</a>
        <asp:GridView ID="gvEqOpSt" runat="server" AutoGenerateColumns="False" ShowHeaderWhenEmpty="true" HorizontalAlign="Center"
            GridLines="None" CssClass="SoarNPGridStyle"
            OnRowDataBound="gvEqOpSt_RowDataBound"
            OnRowEditing="gvEqOpSt_RowEditing"
            OnRowCancelingEdit="gvEqOpSt_RowCancelingEdit"
            OnRowUpdating="gvEqOpSt_RowUpdating"
            AllowPaging="true" PageSize="35" OnPageIndexChanging="gvEqOpSt_PageIndexChanging"
            Font-Size="Small">
            <PagerSettings Mode="NextPreviousFirstLast" Position="TopAndBottom" Visible="true" FirstPageText="&nbsp;First&nbsp;"
                LastPageText="&nbsp;Last&nbsp;" NextPageText="&nbsp;Next&nbsp;" PreviousPageText="&nbsp;Previous&nbsp;" />
            <PagerStyle CssClass="SoarNPpaging" />
            <Columns>
                <asp:TemplateField HeaderStyle-CssClass="text-center" ItemStyle-HorizontalAlign="Center" Visible="true">
                    <HeaderTemplate>
                        <asp:Label ID="lblHiLine" runat="server" Text="iLine" Width="60" ></asp:Label>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label ID="lblIiLine" runat="server" Text='<%# Eval("iLine") %>' Width="60" ></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderStyle-CssClass="text-center" ItemStyle-HorizontalAlign="Center" Visible="false">
                    <HeaderTemplate>
                        <asp:Label ID="lblHIdent" runat="server" Text="Internal Id" Width="60" ToolTip="Points to a row in database table EQUIPCOMPONENTS with this ID field contents"></asp:Label>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label ID="lblIIdent" runat="server" Text='<%# Eval("ID") %>' Width="60" ></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderStyle-CssClass="text-center">
                    <HeaderTemplate>
                        <asp:Label ID="lblHEquipName" runat="server" Text="Equipment" ToolTip="To which piece of equipment does this component belong?"></asp:Label>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label runat="server" ID="lblEquipName" Text='<%# Bind("sShortEquipName") %>'/>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField ItemStyle-Width="120" HeaderStyle-CssClass="text-center">
                    <HeaderTemplate>
                        <asp:Label ID="lblHRegistr" runat="server" Text="Registration Id" ToolTip="FAA N-number, License Plate Number"></asp:Label>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label runat="server" ID="lblIRegistr" Text='<%# Eval("sRegistrId") %>'/>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderStyle-CssClass="text-center">
                    <HeaderTemplate>
                        <asp:Label ID="lblHComponent" runat="server" Text="Component"
                            ToolTip="The name of the component or subcomponent"></asp:Label>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label ID="lblIComponent" runat="server" Text='<%# Eval("sCName") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="text-center">
                    <HeaderTemplate>
                        <asp:Label ID="lblHOperStat" runat="server" Text="Operational Status"
                            ToolTip="The operational status as entered by a web site user whose list of website roles includes 'Equipment'."></asp:Label>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label ID="lblIOperStat" runat="server" Text='<%# Eval("sOperStat") %>'></asp:Label>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:TextBox ID="txbDOperStat" runat="server" Text='<%# Eval("sOperStat") %>'></asp:TextBox>
                    </EditItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="text-center">
                    <HeaderTemplate>
                        <asp:Label runat="server" Text="Edit Status"
                            ToolTip="Click a blue button in this column to modify the text that explains equipment operational status.
If buttons are greyed out your website user Id does not have the required 'Equipment' role assigned." ></asp:Label>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:ImageButton ID="ipbEEdit" ImageUrl="~/i/BlueButton.jpg" runat="server" CommandName="Edit" OnClientClick="oktoSubmit = true;" />
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:ImageButton ID="ipbEUpdate" ImageUrl="~/i/Update.jpg" runat="server" CommandName="Update" OnClientClick="oktoSubmit = true;" />
                        <asp:ImageButton ID="ipbECancel" ImageUrl="~/i/Cancel.jpg" runat="server" CommandName="Cancel" OnClientClick="oktoSubmit = true;" />
                    </EditItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderStyle-CssClass="text-center" ItemStyle-CssClass="maxWidthGrid">
                    <HeaderTemplate>
                        <asp:Label ID="lblHComment" runat="server" Text="Comment" ></asp:Label>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label ID="lblIComment" runat="server" Text='<%# Eval("sComment") %>'></asp:Label>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:TextBox ID="txbDComment" runat="server" Text='<%# Eval("sComment") %>'></asp:TextBox>
                    </EditItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="text-center">
                    <HeaderTemplate>
                        <asp:Label runat="server" Text="Underlying Data Last Updated"
                            ToolTip="The point in time when a user of this website last updated the underlying operational data for this component"></asp:Label>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label ID="lblILastUpdated" runat="server" Text='<%# TSoar.CustFmt.sFmtDate((DateTime)Eval("DLastUpdated"), TSoar.CustFmt.enDFmt.DateAndTimeMin) %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField ItemStyle-HorizontalAlign="Right" HeaderStyle-CssClass="text-center">
                    <HeaderTemplate>
                        <asp:Label runat="server" Text="Accumulated Hours"
                            ToolTip="The number of operating hours that this component has accumulated up to the time of last update"></asp:Label>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label ID="lblIDAccumHours" runat="server" Text='<%# Eval("dAccumHours") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField ItemStyle-HorizontalAlign="Right" HeaderStyle-CssClass="text-center">
                    <HeaderTemplate>
                        <asp:Label runat="server" Text="Accumulated Cycles"
                            ToolTip="The number of cycles (one takeoff and one landing) that this component has accumulated up to the time of last update"></asp:Label>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label ID="lblIAccumCycles" runat="server" Text='<%# Eval("iAccumCycles") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField ItemStyle-HorizontalAlign="Right" HeaderStyle-CssClass="text-center">
                    <HeaderTemplate>
                        <asp:Label runat="server" Text="Accumulated Distance"
                            ToolTip="The number of units of surface travel (miles or kilometers) that this component has accumulated up to the time of last update"></asp:Label>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label ID="lblIAccumDist" runat="server" Text='<%# Eval("dAccumDist") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField ItemStyle-HorizontalAlign="Left" HeaderStyle-CssClass="text-center" ItemStyle-CssClass="maxWidthGrid">
                    <HeaderTemplate>
                        <asp:Label runat="server" Text="Action Item"
                            ToolTip="Description of the required action consisting of two parts separated by a dash: Aging Item Name - Name of Parameter Set"></asp:Label>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label ID="lblIActionItem" runat="server" Text='<%# Eval("sActionItem") %>' ></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField ItemStyle-HorizontalAlign="Left" HeaderStyle-CssClass="text-center">
                    <HeaderTemplate>
                        <asp:Label runat="server" Text="Deadline"
                            ToolTip="DeadLine can be a date, number of ops hours, number of cycles, or number of miles"></asp:Label>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label ID="lblIDeadline" runat="server" Text='<%# Eval("sDeadline") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="text-center">
                    <HeaderTemplate>
                        <asp:Label ID="lblHPrevSM" runat="server" Text="PSM"
                            ToolTip="Prevailing Scheduling Method - the one that produced the earliest deadline:
 E - Elapsed time
 H - operating Hours/running time
 C - number of Cycles
 D - Distance traveled
 U - Unique event
 N - None
"></asp:Label>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label ID="lblIPrevSM" runat="server" Text='<%# Eval("cPrevSchedMeth") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField ItemStyle-HorizontalAlign="Right"  HeaderStyle-CssClass="text-center">
                    <HeaderTemplate>
                        <asp:Label runat="server" Text="% Complete"
                            ToolTip="Percent complete, a number between 0 and 100"></asp:Label>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label ID="lblIPercentComplete" runat="server" Text='<%# Eval("iPercentComplete") %>'></asp:Label>
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

</asp:Content>

<asp:Content ID="FooterContent" ContentPlaceHolderID="ContentPlaceHolderFooter" runat="server">
    <%: Title %>
</asp:Content>
