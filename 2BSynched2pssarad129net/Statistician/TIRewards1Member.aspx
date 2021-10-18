<%@ Page Title="TIRewards1Member" Language="C#" MasterPageFile="~/mTSoar.Master" AutoEventWireup="true" CodeBehind="TIRewards1Member.aspx.cs" Inherits="TSoar.Statistician.TIRewards1Member" %>

<asp:Content ID="HeaderContent" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="server">
    <br />
    <asp:Label runat="server" Text="Rewards Journal for one Member" Font-Italic="true" />
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="ContentPlaceHolderMain" runat="server">
    <h3>Towpilot/Instructor Service Points Account for </h3> <asp:DropDownList ID="DDLMember" runat="server"
        OnSelectedIndexChanged="DDLMember_SelectedIndexChanged" AutoPostBack="true" />
    <br />
    Do not show reward entries with a date earlier than
    <asp:TextBox ID="txbMonths" runat="server" TextMode="Number" Text="15" Width="45px" AutoPostBack="true" OnTextChanged="txbMonths_TextChanged" /> months ago.
    <br />
    <a href="TIRewardsEdit.aspx">Go to Rewards Journal</a>
    <br />
    <div style="font-size:x-small">[Hover over headers for column explanations]</div>
    <div class="gvclass">
        <asp:GridView ID="gvMRewards" runat="server" AutoGenerateColumns="False"
            GridLines="None" CssClass="SoarNPGridStyle"
            OnRowDataBound="gvMRewards_RowDataBound"
            OnPageIndexChanging="gvMRewards_PageIndexChanging"
            ShowHeaderWhenEmpty="true"
            Font-Size="Small" EmptyDataText="--==>> No data found <<==--"
            AllowPaging="true" PageSize="30"
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
</asp:Content>

<asp:Content ID="FooterContent" ContentPlaceHolderID="ContentPlaceHolderFooter" runat="server">
    <%: Title %>
</asp:Content>
