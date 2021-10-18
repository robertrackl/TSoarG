<%@ Page Title="Privacy Policy" Language="C#" MasterPageFile="~/mTSoar.Master" AutoEventWireup="true" CodeBehind="PrivacyPolicy.aspx.cs" Inherits="TSoar.PublicPages.PrivacyPolicy" %>

<asp:Content ID="HeaderContent" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="server">
    <asp:Label runat="server" Text="PSSA Privacy Policy" Font-Italic="true" />
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="ContentPlaceHolderMain" runat="server">
    <div class="HelpText" ><%-- SCR 213 --%>
        <h3>Privacy Notice</h3>
        <p>This privacy notice discloses the privacy practices for Puget Sound Soaring Association’s website https://pssa.rad129.net.
            This privacy notice applies solely to information collected by this website. It will notify you of the following:</p>
            <ol>
                <li>What personally identifiable information is collected from you through the website, how it is used and with whom it may be shared.</li>
                <li>What choices are available to you regarding the use of your data.</li>
                <li>The security procedures in place to protect the misuse of your information.</li>
                <li>How you can correct any inaccuracies in the information.</li>
            </ol>
        <h4>Information Collection, Use, and Sharing </h4>
        <p>We are the sole owners of the information collected on this site.
            We only have access to/collect information that you voluntarily give us via email or other direct contact from you.
            We will not sell or rent this information to anyone.
            We will use your information to respond to you, regarding the reason you contacted us.
            We will not share your information with any third party outside of our organization, other than as necessary to fulfill your request.</p>
            <p>The Club's financial data (which may include financial and contact data on the account that the Club maintains for you) is kept on QuickBooks Online, a service provided by Intuit;
                their privacy policy is available <a href="https://security.intuit.com/index.php/privacy?bc=OBI-LL1" target="_blank">here</a>.
            </p>
        <h4>Links </h4>
        <p>This website contains links to other sites. 
            Please be aware that we are not responsible for the content or privacy practices of such other sites. 
            We encourage our users to be aware when they leave our site and to read the privacy statements of any other site that collects personally identifiable information.</p>
        <h4>Your Access to and Control Over Information </h4>
            <p>You can do the following at any time by contacting us using the information given on our <a href="Contact.aspx">How to Contact us</a> page.:</p>
                <ul>
                    <li>See what data we have about you, if any.</li>
                    <li>Change/correct any data we have about you.</li>
                    <li>Have us delete any data we have about you.</li>
                    <li>Express any concern you have about our use of your data.</li>
                </ul>
        <h4>Security </h4>
        <p>We take precautions to protect your information. When you submit sensitive information to us, your information is protected both online and offline.</p>
 
        <p>While we use encryption to protect sensitive information transmitted online, we also protect your information offline.
            Only employees or volunuteers who need the information to perform a specific job (for example, billing) are granted access to personally identifiable information.
            The computers/servers in which we store personally identifiable information are kept in a secure environment.</p>
        <p>We may contact you in the future to tell you about changes to this privacy policy.</p>
        <p>If you feel that we are not abiding by this privacy policy, you should contact us immediately as shown on our <a href="Contact.aspx">How to Contact us</a> web page.</p>
    </div><%-- SCR 213 --%>
</asp:Content>

<asp:Content ID="FooterContent" ContentPlaceHolderID="ContentPlaceHolderFooter" runat="server">
    <%: Title %>
</asp:Content>
