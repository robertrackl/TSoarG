<%@ Page Title="AJAX Practice" Language="C#" MasterPageFile="~/mTSoar.Master" AutoEventWireup="true" CodeBehind="AjaxPractice.aspx.cs" Inherits="TSoar.Developer.SWLab.AjaxPractice" %>

<asp:Content ID="HeaderContent" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="server">
    <asp:Label runat="server" Text="Practice with Asynchronous JavaScript And XML" Font-Italic="true" />
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="ContentPlaceHolderMain" runat="server">

<%--        <asp:Button ID="pbStart" runat="server" Text="Start" OnClick="pbStart_Click" />--%>


    <div>
        <%-- ModalPopupExtender, popping up UPMPE_Panel and dynamically populating lblPopupText --%>
        <asp:LinkButton ID="UPTarget" runat="server" Text="T" CssClass="displayNone" />
        <ajaxToolkit:ModalPopupExtender ID="UPModalPopExt" runat="server"
            TargetControlID="UPTarget" PopupControlID="UPMPE_Panel"
            BackgroundCssClass="background"
             />
        <asp:Panel ID="UPMPE_Panel" runat="server" CssClass="popup" style="display:none;" HorizontalAlign="Center">
            <asp:Label ID="lblUPPopupText" runat="server" BackColor="#eeb5a2" Font-Bold="true" />
            <br />
            <asp:Timer ID="UPTimer1" OnTick="UPTimer1_Tick" runat="server" Interval="5" Enabled="false" />
            <asp:Timer ID="UPTimer2" OnTick="UPTimer2_Tick" runat="server" Interval="5" Enabled="false" />
      
            <asp:UpdatePanel ID="UPPanel" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true" >
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="UPTimer1" />
                <asp:AsyncPostBackTrigger ControlID="UPTimer2" />
            </Triggers>
            <ContentTemplate>
                Characters: <asp:Label ID="lblAlpha" runat="server" Text="!" />
                Time is <asp:Label id="lblCurrTime" runat="server"></asp:Label><BR />
            </ContentTemplate>
            </asp:UpdatePanel>
            <div>
            Page loaded at <asp:Label ID="OriginalTime" runat="server"></asp:Label>
            </div>
            <br />
            <p> <asp:Button ID="OkUPButton" runat="server" Text="OK" OnClick="UPButton_Click" />&nbsp;&nbsp;
        </asp:Panel>
    </div>

</asp:Content>

<asp:Content ID="FooterContent" ContentPlaceHolderID="ContentPlaceHolderFooter" runat="server">
    <%: Title %>
</asp:Content>
