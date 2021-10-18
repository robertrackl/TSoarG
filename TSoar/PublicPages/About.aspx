<%@ Page Title="The PSSA Public About Page" Language="C#" MasterPageFile="~/mTSoar.Master" AutoEventWireup="true" CodeBehind="About.aspx.cs"
    Inherits="TSoar.PublicPages.About" %>

<asp:Content ID="HeaderContent" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="server">
    <br /><%: Title %>
</asp:Content>

<asp:Content ID="BodyContent" ContentPlaceHolderID="ContentPlaceHolderMain" runat="server">
    <div class="HelpText" >
        <p>
            <B>Puget Sound Soaring Association</B> is a soaring club that operates out of Bergseth Airfield just NorthEast of Enumclaw, Washington.
            We offer instruction and glider tows and rentals for our membership.
            Introductory 30-day memberships are available to the general public for $150 which includes an introductory glider flight with a qualified pilot
            in one of our two seat gliders.
        </p><p>
            Note: Due to the weight & balance limitations of the gliders, passenger weight is limited to approximately 235 lbs.
        </p><p>
            For those who haven't yet experienced the joy of soaring follow along with this description of an introductory ride in a glider:
        </p>
        <UL>
            <li>After a brief explanation of the glider along with its instruments and controls by one of our members you will be comfortably seated in the front seat. 
            Our instructor or commercial rated pilot will be sitting behind you and will describe the different phases of the flight. 
            Both seats have all the required controls to pilot the glider. Foot pedals control the rudder and a control stick is used to control pitch (nose up and down)
            and bank for turning. All controls are linked together so it is possible to observe and feel what your pilot is doing.</li>
        <li>
            The glider is connected to the tow plane with a 200 foot rope and is pulled aloft in about 10 minutes to around 4000 feet. 
            The pilot (or you) releases the glider from the towrope and gently banks to the right with the tow plane banking to the left. 
            You notice now that it has become quieter as all you can hear is the air rushing past.
        </li><li>
            In the right conditions the glider is capable of staying aloft until sundown if your pilot locates sources of rising air which we call lift. 
            If the wind is strong enough, the ridges to the east will provide a continuous source of lift. 
            Other times we might locate some thermals which are invisible columns of warm rising air. 
            Keep a lookout for hawks or turkey vultures. They are expert pilots and many times show us where the thermals are. 
            It is a wonderful experience to actually be circling in a thermal with one of these birds. 
            Your introductory flight will last about 30 minutes if the pilot finds some lift. 
            Without lift the flight will be a little shorter but still highly enjoyable. 
            At your pilot's discretion he or she may let you control the glider to let you get the full experience of flying and hopefully generate continued interest
            on your part in this wonderful sport.
        </li><li>
            As the glider gets lower the pilot will enter the landing pattern and set up for a soft touchdown at about the same spot from where your flight started. 
            After the flight either the pilot or field manager will answer any questions you might have.
        </li>
        </UL>
        <p>
            We do have <a href="GiftCerts.aspx"> gift certificates </a> available if you would like to provide a friend or relative with an opportunity to enjoy soaring.</p>
    </div>
</asp:Content>

<asp:Content ID="FooterContent" ContentPlaceHolderID="ContentPlaceHolderFooter" runat="server">
    <%: Title %>
</asp:Content>
