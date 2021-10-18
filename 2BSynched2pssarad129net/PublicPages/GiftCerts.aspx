<%@ Page Title="TSoar GiftCerts" Language="C#" MasterPageFile="~/mTSoar.Master" AutoEventWireup="true" CodeBehind="GiftCerts.aspx.cs"
    Inherits="TSoar.PublicPages.GiftCerts" %>

<asp:Content ID="HeaderContent" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="server">
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="ContentPlaceHolderMain" runat="server">
    <div class="HelpText" ><%-- SCR 213 --%>
        <p>Introductory Flight Gift Certificates are $150.00 each.  To order, send a check made out to: PSSA Inc, or call the number below to purchase by credit card.
        </p>
        <p>Note: Due to the weight & balance limitations of the gliders, passenger weight is limited to at most 235 lbs.
        </p>
        <p>Mail to:<br />
            PSSA, Inc.<br />
            37302 204th Ave. SE<br />
            Auburn,  WA  98092
        </p>
        <p>To purchase by credit card, call Tim at: 425 466 9201
        </p>
        <p>Using a gift certificate, you purchase an introductory 30-Day membership in our soaring club. It consists of:</p>
        <ul>
            <li>A flight in a two-seat glider with a pilot who holds at least a commercial glider license; flight duration is usually 20 to 30 minutes.</li>
            <li>During this 30-Day membership period, you may fly additional flights in one of our gliders with a PSSA pilot or instructor at regular member rates.</li>
            <li>If you sign up as a PSSA regular/family/youth member during this 30-Day period you receive a $25 credit towards your initiation fee.</li>
        </ul>
    </div><%-- SCR 213 --%>
</asp:Content>

<asp:Content ID="FooterContent" ContentPlaceHolderID="ContentPlaceHolderFooter" runat="server">
    <%: Title %>
</asp:Content>
