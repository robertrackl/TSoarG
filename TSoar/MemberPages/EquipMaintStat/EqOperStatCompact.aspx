<%@ Page Title="EqOperStatCompact" Language="C#" MasterPageFile="~/mTSoar.Master" AutoEventWireup="true" 
    CodeBehind="EqOperStatCompact.aspx.cs" Inherits="TSoar.MemberPages.EquipMaintStat.EqOperStatCompact" %>

<asp:Content ID="HeaderContent" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="server">
    <asp:Label runat="server" Text="Equipment Operational Status - Compact List" Font-Italic="true" />
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="ContentPlaceHolderMain" runat="server">
    <a href="EqOperStatus.aspx">Show Detailed List of Equipment Status</a>
    <div>
        <asp:GridView ID="gvEqOpStC" runat="server" AutoGenerateColumns="False" ShowHeaderWhenEmpty="true" HorizontalAlign="Center"
            GridLines="None" CssClass="SoarNPGridStyle"
            AllowPaging="true" PageSize="18" OnPageIndexChanging="gvEqOpStC_PageIndexChanging"
            Font-Size="Small">
            <PagerSettings Mode="NextPreviousFirstLast" Position="TopAndBottom" Visible="true" FirstPageText="&nbsp;First&nbsp;"
                LastPageText="&nbsp;Last&nbsp;" NextPageText="&nbsp;Next&nbsp;" PreviousPageText="&nbsp;Previous&nbsp;" />
            <PagerStyle CssClass="SoarNPpaging" />
            <Columns>

                <asp:TemplateField HeaderStyle-CssClass="text-center">
                    <HeaderTemplate>
                        <asp:Label ID="lblHEquipName" runat="server" Text="Equipment"></asp:Label>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label runat="server" ID="lblEquipName" Text='<%# Eval("sPoEq") %>'/>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField ItemStyle-Width="120" HeaderStyle-CssClass="text-center">
                    <HeaderTemplate>
                        <asp:Label ID="lblHRegistr" runat="server" Text="Registration Id" ToolTip="FAA N-number, License Plate Number"></asp:Label>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label runat="server" ID="lblIRegistr" Text='<%# Eval("sReg") %>'/>
                    </ItemTemplate>
                </asp:TemplateField>
<%--                SCR 231 start --%>
                <asp:TemplateField ItemStyle-Width="120" HeaderStyle-CssClass="text-center">
                    <HeaderTemplate>
                        <asp:Label ID="lblHComponent" runat="server" Text="Equipment Component"></asp:Label>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label runat="server" ID="lblIComponent" Text='<%# Eval("sComponent") %>'/>
                    </ItemTemplate>
                </asp:TemplateField>
<%--                SCR 231 end --%>
                <asp:TemplateField ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="text-center">
                    <HeaderTemplate>
                        <asp:Label ID="lblHOperStat" runat="server" Text="Operational Status"
                            ToolTip="The operational status as entered by a web site user whose list of website roles includes 'Equipment'."></asp:Label>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label ID="lblIOperStat" runat="server" Text='<%# Eval("sOperStatus") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderStyle-CssClass="text-center">
                    <HeaderTemplate>
                        <asp:Label ID="lblHComment" runat="server" Text="Comment" ></asp:Label>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label ID="lblIComment" runat="server" Text='<%# Eval("sComment") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>

            </Columns>
        </asp:GridView>
    </div>

</asp:Content>

<asp:Content ID="FooterContent" ContentPlaceHolderID="ContentPlaceHolderFooter" runat="server">
    <%: Title %>
</asp:Content>
