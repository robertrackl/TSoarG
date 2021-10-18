<%@ Page Title="TSoar Public Pages" Language="C#" MasterPageFile="~/mTSoar.Master" AutoEventWireup="true" CodeBehind="Public.aspx.cs"
    Inherits="TSoar.PublicPages.Public" %>

<asp:Content ID="HeaderContent" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="server">
    <asp:Label runat="server" Text="PSSA Public Pages" Font-Italic="true" />
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="ContentPlaceHolderMain" runat="server">
    <div class="HelpText" ><%-- SCR 213 --%>
        <div class="text-center">
            <h3>The Public Area</h3>
            <p><a href="About.aspx">About Puget Sound Soaring Association</a></p>
            <p><a href="Contact.aspx">How to Contact Us</a></p>
            <p><a href="Directions.aspx">How to Find Us</a></p>
            <p><a href="Fleet/Fleet.aspx">Fleet (PSSA Aircraft)</a></p>
            <p><a href="ByLawsOpsRules.aspx">Bylaws and Operation Rules</a></p>
            <p><a href="RateSheet.aspx">Rate Sheet</a></p>
            <p><a href="GiftCerts.aspx">Gift Certificates</a></p>
            <p><a href="CarouselShow.aspx">Slide Show</a></p>
            <p><a href="Schedule.aspx">Schedule of Operations and Events</a></p>
            <p><a href="SoaringResources.aspx">Soaring Resources</a></p>
            <p><a href="ClubHistory.aspx">Club History</a></p>
            <p><a href="LiabilityWaiver.aspx">Liability Waiver</a></p>
            <p><a href="PrivacyPolicy.aspx">Privacy Policy</a></p>
            <p><a href="AcceptUsePol.aspx">Acceptable Use Policy</a></p>
        </div>
    </div><%-- SCR 213 --%>
</asp:Content>

<asp:Content ID="FooterContent" ContentPlaceHolderID="ContentPlaceHolderFooter" runat="server">
    <%: Title %>
</asp:Content>
