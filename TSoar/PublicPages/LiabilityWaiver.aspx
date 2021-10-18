<%@ Page Title="Liability Waiver" Language="C#" MasterPageFile="~/mTSoar.Master" AutoEventWireup="true" CodeBehind="LiabilityWaiver.aspx.cs" Inherits="TSoar.PublicPages.LiabilityWaiver" %>

<asp:Content ID="HeaderContent" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="server">
    <asp:Label runat="server" Text="PSSA Privacy Policy" Font-Italic="true" />
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="ContentPlaceHolderMain" runat="server">
    <div class="HelpText" ><%-- SCR 213 --%>
        <h2>Liability Waiver Form</h2>
        <p>This form has two pages; please also fill in emergency contact information on second page. Please print this form and fill and sign.
            Give to Field Manager or your pilot.
        </p>
        <asp:HyperLink NavigateUrl="waiver.pdf" runat="server" Target="_blank">Liability Waiver</asp:HyperLink>
    </div><%-- SCR 213 --%>
</asp:Content>

<asp:Content ID="FooterContent" ContentPlaceHolderID="ContentPlaceHolderFooter" runat="server">
    <%: Title %>
</asp:Content>
