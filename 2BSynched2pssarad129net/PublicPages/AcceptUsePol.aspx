<%@ Page Title="Acceptable Use Policy" Language="C#" MasterPageFile="~/mTSoar.Master" AutoEventWireup="true" CodeBehind="AcceptUsePol.aspx.cs" Inherits="TSoar.PublicPages.AcceptUsePol" %>

<asp:Content ID="HeaderContent" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="server">
    <asp:Label runat="server" Text="PSSA Acceptable Use Policy" Font-Italic="true" />
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="ContentPlaceHolderMain" runat="server">
    <div class="HelpText" ><%-- SCR 213 --%>
        <h3>Acceptable Use Policy (AUP)</h3>
        <asp:Label ID="lblAgree" runat="server" Text="Please be familiar with this Acceptable Use Policy.
            Then scroll to the bottom of this page and click one of the buttons" Visible="false" ForeColor="Red" Font-Bold="true" />
        <h4><u>Introduction</u></h4>
        <h5><u>Definitions</u></h5>
        <p> PSSA means Puget Sound Soaring Association, Inc., a not-for-profit organization incorporated in the State of Washington.</p>
        <p>For purposes of this AUP, “content” refers to all forms of
            communications including, without limitation, text, graphics (including
            photographs, illustrations, images, drawings, logos), executable
            programs, audiovisual recordings, and audio recordings.
            </p>
        <p>The words "you", "your", and "yours" refer to the reader of this web page, and users of this website.</p>
        <h5><u>Preamble</u></h5>
        <p>This website is owned by PSSA.</p>
        <p>Users of this web site can be either anonymous or authenticated. Anonymous users have access only to the home page ("Default")
            and the Public Pages. A user is authenticated when the user's login attempt succeeds.
            Authenticated users have access to various areas and pages of this website in addition to the home page and Public Pages.
            Which pages and areas depends on the role or roles that the website administrator has assigned to that user.
            Accordingly, the subsequent sections of this AUP apply mostly to authenticated users since the home and public pages
            only display information with no opportunity for the user to input any data except for the login and password recovery dialogs.
            Anonymous users are prohibited from attempting to circumvent the login process.
        </p>
        <h4><u>Prohibited Activities</u></h4>
            <ol type="i">
                <li>Use, possess, post, upload, transmit, disseminate or otherwise
                    make available content that is unlawful or violates the copyright
                    or other intellectual property rights of others;</li>
                <li>Participate in any fraudulent activities, including impersonating
                    any person or entity or forging anyone else’s digital or manual
                    signature. You assume all risks regarding the determination of
                    whether material is in the public domain;</li>
                <li>Invade another person’s privacy, collect or store personal data
                    about other users, or stalk or harass another person or entity; </li>
                <li>Use, reproduce, distribute, sell, resell or otherwise exploit this
                    website or content we provide or which you obtain through this
                    website for any commercial purposes;
                <li>threatening, intimidating, abusive or harassing statements; </li>
                <li>content that violates the privacy rights or intellectual property
                    rights of others;</li>
                <li>content that unlawfully promotes or incites hatred;</li>
                <li>content that is otherwise offensive or objectionable; or</li>
                <li>any transmissions constituting or encouraging conduct that
                    would constitute a criminal offence, give rise to civil liability
                    or otherwise violate any municipal, provincial, federal or
                    international law, order or regulation.</li>
                <li>impersonate any person or entity, including, without limitation,
                    a PSSA official, or falsely state or
                    otherwise misrepresent your affiliation with a person or entity;</li>
            </ol>
        <h4><u>Unlawful or Inappropriate Content</u></h4>
        <p>PSSA reserves the right to move, remove or refuse to post
            any content, in whole or in part, that it, in its sole discretion, decides
            are unacceptable, undesirable or in violation of the terms of this AUP.
            This includes, without limitation:</p>
        <ol type="i">
            <li>obscene, profane, pornographic content;</li>
            <li>defamatory, fraudulent or deceptive statements;</li>
            <li>threatening, intimidating, abusive or harassing statements;</li>
            <li>content that violates the privacy rights or intellectual property
                rights of others;</li>
            <li>content that unlawfully promotes or incites hatred;</li>
            <li>content that is otherwise offensive or objectionable; or</li>
            <li>any transmissions constituting or encouraging conduct that
                would constitute a criminal offence, give rise to civil liability
                or otherwise violate any municipal, provincial, federal or
                international law, order or regulation.</li>
        </ol>
        <h4><u>Security</u></h4>
            <p>As set out above, you are responsible for any misuse of this website,
                by you or by any other person with access to this website through
                your equipment or your account. Therefore, you must take steps to
                ensure that others do not gain unauthorized access to the website.
                This website may not be used to breach the
                security of another user or to attempt to gain access to any other
                person’s equipment, software or data, without the knowledge and
                consent of such person. Additionally, this website may not be used
                in any attempt to circumvent the user authentication or security of
                any account, including, without limitation, accessing
                data not intended for you. This website may not be used in any attempt
                to interfere with computer networking or telecommunications
                services to any user, host or network, including, without limitation,
                denial of service attacks, flooding of a network, overloading a service,
                improper seizing and abuse of operator privileges and attempts to
                “crash” a host. The transmission or dissemination of any information
                or software that contains a virus or other harmful feature is also
                prohibited. You are solely responsible for the security of any device
                you choose to connect to this website, including any data stored
                on that device.  You agree to treat as confidential all access codes, user names, and passwords that we may provide to you for use
                with this website, or which you generate yourself (e.g., through changing your password).
            </p>
        <h4><u>Governing Law</u></h4>
            <p>You agree that all matters relating to your access to or use of this website, including all disputes,
                will be governed by the laws of the United States of America and by the laws of the State of Washington.</p>
        <h4><u>Miscellaneous</u></h4>
            <p>If any of the provisions of this AUP are held by a court of competent jurisdiction to be void or unenforceable,
                such provisions shall be limited or eliminated to the minimum extent necessary and replaced with a valid provision
                that best embodies the intent of this AUP, so that this AUP shall remain in full force and effect.</p>
        <h4><u>Complaints</u></h4>
            <p>Please direct any complaints of violations of this AUP to info@pugetsoundsoaring.org.</p>
        <hr />
        <asp:Button ID="pbAgree" runat="server" Text="I agree with this Acceptable Use Policy" Visible="false" OnClick="pbAgree_Click" />
        <asp:Button ID="pbNotAgree" runat="server" Text="I do not agree with this Acceptable Use Policy" Visible="false" OnClick="pbNotAgree_Click" />
    </div><%-- SCR 213 --%>
</asp:Content>

<asp:Content ID="FooterContent" ContentPlaceHolderID="ContentPlaceHolderFooter" runat="server">
    <%: Title %>
</asp:Content>
