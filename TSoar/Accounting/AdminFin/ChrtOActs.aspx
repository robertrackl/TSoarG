<%@ Page Title="Chart Of Accounts" Language="C#" MasterPageFile="~/mTSoar.Master" AutoEventWireup="true"
    CodeBehind="ChrtOActs.aspx.cs" Inherits="TSoar.Accounting.AdminFin.ChrtOActs" %>

<asp:Content ID="HeaderContent" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="server">
    <asp:Label runat="server" Text="Board - Finance - Accounting Administration" Font-Italic="true" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderMain" runat="server">
    <h3>Chart of Accounts - Hierarchical Listing</h3>
    <p><a href="AdminFin.aspx">Back to Administer Accounting System - Overview</a></p>
    <p>Reference: <a href="https://www.ifrs-gaap.com/basic-gaap-chart-accounts" target="_blank">GAAP Basic Chart of Accounts</a> </p>
    <p>Edit the Chart of Accounts: <a href="EditCOA.aspx">Edit CoA</a>. Define Subledgers: <a href="Subledgers.aspx">Edit Subledgers</a>.</p>
    <asp:UpdatePanel ID="UpdatePanelTrV" UpdateMode="Conditional" runat="server">
        <ContentTemplate>
            <asp:TreeView ID="trv_Acc" runat="server" MaxDataBindDepth="6" PathSeparator="|"
                PopulateNodesFromClient="false" ShowLines="true" BackColor="#EDF8E7" NodeWrap="true"
                BorderStyle="Solid" HoverNodeStyle-BackColor="WhiteSmoke" SelectedNodeStyle-BackColor="Orange"
                NodeStyle-BorderStyle="Solid" NodeStyle-BorderWidth="1" NodeStyle-BorderColor="Green"
                NodeStyle-HorizontalPadding="5" NodeStyle-ChildNodesPadding="5" NodeStyle-ForeColor="Black">
            </asp:TreeView>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

<asp:Content ID="FooterContent" ContentPlaceHolderID="ContentPlaceHolderFooter" runat="server">
    <%: Title %>
</asp:Content>
