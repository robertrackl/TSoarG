<%@ Page Title="Filter Sort Select Attached Files" Language="C#" MasterPageFile="~/mTSoar.Master" AutoEventWireup="true"
    CodeBehind="FilterSortAttFiles.aspx.cs" Inherits="TSoar.Accounting.FinDetails.FilterSortAttFiles" %>

<asp:Content ID="HeaderContent" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="server">
    <asp:Label runat="server" Text="Filter, Sort, and Select from List of Attached Files" Font-Italic="true" />
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="ContentPlaceHolderMain" runat="server">
    <h4>Here is a list of files that have already been uploaded and attached to some transaction. These files are available for attaching to the transaction you are currently working on as well. Please select one:</h4>
    <asp:Button ID="pbCancel" runat="server" Text="Cancel" Font-Size="Smaller" OnClick="pbCancel_Click" />
    <br /><br />
    <asp:GridView ID="gvFSSA" runat="server" AutoGenerateColumns="False"
                GridLines="None" CssClass="SoarNPGridStyle"
                PageSize="25" EmptyDataText="(Nothing to display)"
                AllowPaging="true" ShowHeaderWhenEmpty="true"
                Font-Size="Small"
                OnRowCommand="gvFSSA_RowCommand">
        <PagerSettings Mode="NextPreviousFirstLast" Position="TopAndBottom" Visible="true" FirstPageText="&nbsp;First&nbsp;"
            LastPageText="&nbsp;Last&nbsp;" NextPageText="&nbsp;Next&nbsp;" PreviousPageText="&nbsp;Previous&nbsp;" />
        <PagerStyle CssClass="SoarNPpaging" />
        <Columns>
            <asp:TemplateField>
                <HeaderTemplate> ID </HeaderTemplate>
                <ItemTemplate> <asp:Label ID="lblId" runat="server" Text='<%# Eval("ID") %>' /> </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <HeaderTemplate>Associated Date</HeaderTemplate>
                <ItemTemplate><asp:Label ID="lblDDateOfDoc" runat="server" Text='<%# Eval("DDateOfDoc") %>' /></ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <HeaderTemplate>File Name Prefix</HeaderTemplate>
                <ItemTemplate><asp:Label ID="lblsPrefix" runat="server" Text='<%# Eval("sPrefix") %>' /></ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <HeaderTemplate>File Name</HeaderTemplate>
                <ItemTemplate><asp:Label ID="lblsName" runat="server" Text='<%# Eval("sName") %>' /></ItemTemplate>
            </asp:TemplateField>
            <asp:ButtonField ButtonType="Button" Text="Select" CommandName="Select" ControlStyle-Font-Size="Smaller" HeaderText="Select" />
            <asp:TemplateField>
                <HeaderTemplate>Attachment Category</HeaderTemplate>
                <ItemTemplate><asp:Label ID="lblsAttachmentCateg" runat="server" Text='<%# Eval("sAttachmentCateg") %>' /></ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
    <br />
    <asp:Button ID="pbCancel2" runat="server" Text="Cancel" Font-Size="Smaller" OnClick="pbCancel_Click" />

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
            <p> <asp:Button ID="OkButton" runat="server" Text="OK" OnClick="Button_Click" OnClientClick="oktoSubmit = true;" />&nbsp;&nbsp;
                <asp:Button ID="CancelButton" runat="server" Text="Cancel" OnClick="Button_Click" OnClientClick="oktoSubmit = true;" />&nbsp;&nbsp;
                <asp:Button ID="NoButton" runat="server" Text="No" OnClick="Button_Click" OnClientClick="oktoSubmit = true;" />&nbsp;&nbsp;
                <asp:Button ID="YesButton" runat="server" Text="Yes" OnClick="Button_Click" OnClientClick="oktoSubmit = true;" /></p>
        </asp:Panel>
    </div>

</asp:Content>

<asp:Content ID="FooterContent" ContentPlaceHolderID="ContentPlaceHolderFooter" runat="server">
    <%: Title %>
</asp:Content>
