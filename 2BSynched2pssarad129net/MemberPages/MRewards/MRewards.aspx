<%@ Page Title="MRewards" Language="C#" MasterPageFile="~/mTSoar.Master" AutoEventWireup="true" CodeBehind="MRewards.aspx.cs" Inherits="TSoar.MemberPages.MRewards.MRewards" %>

<asp:Content ID="HeaderContent" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="server">
    <br />
    <asp:Label runat="server" Text="Show Rewards Tally for the Signed-in User/Member" Font-Italic="true" />
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="ContentPlaceHolderMain" runat="server">
    <div class="HelpText" ><%-- SCR 213 --%>
        <h3>Towpilot/Instructor Service Points Account for <asp:Label ID="lblUser" runat="server" Text="TBD" /></h3>
        <a href="RewardRules.aspx">Show Towpilot/Instructor Reward Rules</a>
        <br />
        Do not show reward entries with a date earlier than
        <asp:TextBox ID="txbMonths" runat="server" TextMode="Number" Text="15" Width="45px" AutoPostBack="true" OnTextChanged="txbMonths_TextChanged" /> months ago.
    </div><%-- SCR 213 --%>

    <div style="font-size:x-small">[Hover over headers for column explanations]</div>
    <div class="gvclass">
        <asp:GridView ID="gvMRewards" runat="server" AutoGenerateColumns="False"
            GridLines="None" CssClass="SoarNPGridStyle"
            OnRowDataBound="gvMRewards_RowDataBound"
            OnPageIndexChanging="gvMRewards_PageIndexChanging"
            ShowHeaderWhenEmpty="true"
            AllowPaging="true" PageSize="30"
            Font-Size="Small" EmptyDataText="--==>> No data found <<==--"
            >
            <PagerSettings Mode="NextPreviousFirstLast" Position="TopAndBottom" Visible="true" FirstPageText="&nbsp;First&nbsp;"
                LastPageText="&nbsp;Last&nbsp;" NextPageText="&nbsp;Next&nbsp;" PreviousPageText="&nbsp;Previous&nbsp;" />
            <PagerStyle CssClass="SoarNPpaging" />
            <Columns>
                <asp:BoundField DataField="EarnD" HeaderText="Earned" DataFormatString="{0:yyyy/MM/dd}" />
                <asp:BoundField DataField="ExpiryD" HeaderText="Expiry" DataFormatString="{0:yyyy/MM/dd}" />
                <asp:BoundField DataField="ClaimD" HeaderText="Claimed" DataFormatString="{0:yyyy/MM/dd}" />
                <asp:BoundField DataField="ServicePtsi" HeaderText="Svc.Pts." />
                <asp:BoundField DataField="Expiredb" HeaderText="X?" />
                <asp:BoundField DataField="ECCodec" HeaderText="Code" />
                <asp:BoundField DataField="Cumuli" HeaderText="Cumul" />
                <asp:BoundField DataField="CumForwarded" HeaderText="Forward" />
                <asp:BoundField DataField="Claim1yri" HeaderText="C1yr" />
                <asp:BoundField DataField="Claim1yrg" HeaderText="C1yrG" />
                <asp:TemplateField>
                    <HeaderTemplate>Notes</HeaderTemplate>
                    <ItemTemplate><asp:Label ID="lblCommentss" runat="server" Text='<%# Eval("Commentss") %>' /></ItemTemplate>
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
