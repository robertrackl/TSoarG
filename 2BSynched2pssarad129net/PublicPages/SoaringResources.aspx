<%@ Page Title="Soaring Resources" Language="C#" MasterPageFile="~/mTSoar.Master" AutoEventWireup="true" CodeBehind="SoaringResources.aspx.cs" Inherits="TSoar.PublicPages.SoaringResources" %>

<asp:Content ID="HeaderContent" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="server">
    <asp:Label runat="server" Text="Links to Sites of Soaring and Sailplaning Interest" Font-Italic="true" />
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="ContentPlaceHolderMain" runat="server">
    <div class="HelpText" ><%-- SCR 213 --%>
        <ul>
            <li>Weather and Meteorology
                <ul>
                    <li>NOAA, National Weather Service
                        <ul>
                            <li><asp:HyperLink runat="server" 
                                NavigateUrl="https://forecast.weather.gov/MapClick.php?lon=-121.92116&lat=47.24408#.W2SMCdJKjmF" 
                                Target="_blank">Bergseth Field, Enumclaw, WA US</asp:HyperLink></li>
                            <li><asp:HyperLink runat="server" 
                                NavigateUrl="https://forecast.weather.gov/MapClick.php?lat=47.3187&lon=-119.5512#.W2Tk2dJKjmE" 
                                Target="_blank">Ephrata, WA US</asp:HyperLink></li>
                            <li><asp:HyperLink runat="server" 
                                NavigateUrl="https://forecast.weather.gov/MapClick.php?lat=46.0489&lon=-118.4095#.W2Tl49JKjmE" 
                                Target="_blank">Walla Walla Martin Field, WA US</asp:HyperLink></li>
                            <li><asp:HyperLink runat="server" NavigateUrl="https://www.wrh.noaa.gov/zoa/mwmap.php?map=sew" Target="_blank">
                                Interactive METAR Map, Seattle Area</asp:HyperLink></li>
                        </ul>
                    </li>
                    <li>NOAA, Aviation Weather
                        <ul>
                            <li><asp:HyperLink runat="server" NavigateUrl="https://www.wrh.noaa.gov/sew/aviation.php" Target="_blank">Current Conditions Seattle-Tacoma, WA US</asp:HyperLink></li>
                            <li><asp:HyperLink runat="server" NavigateUrl="https://www.wrh.noaa.gov/sew/soar.php" Target="_blank">Soaring Forecasts near Seattle</asp:HyperLink></li>
                        </ul>
                    </li>
                    <li>usairnet<%-- SCR 229 --%>
                        <ul>
                            <li><asp:HyperLink runat="server" NavigateUrl="http://www.usairnet.com/cgi-bin/launch/code.cgi?Submit=Go&sta=KSEA&state=WA" Target="_blank">General Aviation weather near Seattle</asp:HyperLink></li>
                        </ul>
                    </li>
                    <li>wx to fly
                        <ul>
                            <li><asp:HyperLink runat="server" NavigateUrl="http://wxtofly.net/v2/windgrams.html#Bergseth" Target="_blank">wxtofly for Bergseth Field</asp:HyperLink></li>
                        </ul>
                    </li>
                    <li><asp:HyperLink runat="server" NavigateUrl="http://www.drjack.info/" Target="_blank">Dr. John W. (Jack) Glendening, Meteorologist</asp:HyperLink> - (Establish a free account in order to use the links below)
                        <ul>
                            <li><asp:HyperLink runat="server" NavigateUrl="http://www.drjack.info/BLIP/NAM/NW/FCST/wfpm.curr+1.21z.png" Target="_blank">NAM Thermal Updraft velocity Next Day</asp:HyperLink></li>
                            <li><asp:HyperLink runat="server" NavigateUrl="http://www.drjack.info/BLIP/RAP/NW/FCST/wfpm.21z.png" Target="_blank">Latest RAP Thermal Updraft velocity</asp:HyperLink></li>
                        </ul>
                    </li>
                </ul>
            </li>
            <li>Organizations
                <ul>
                    <li><asp:HyperLink runat="server" NavigateUrl="http://www.ssa.org/" Target="_blank">The Soaring Society of America</asp:HyperLink></li>
                    <li><asp:HyperLink runat="server" NavigateUrl="https://www.faa.gov/about/office_org/field_offices/fsdo/sea/contact/" Target="_blank">Seattle Flight Standards District Office</asp:HyperLink></li>
                    <li><asp:HyperLink runat="server" NavigateUrl="https://www.seattleglidercouncil.org/" Target="_blank">Seattle Glider Council</asp:HyperLink></li>
                    <li><asp:HyperLink runat="server" NavigateUrl="http://www.evergreensoaring.info/home" Target="_blank">Evergreen Soaring</asp:HyperLink></li>
                    <li><asp:HyperLink runat="server" NavigateUrl="http://www.cascadesoaringsociety.com/" Target="_blank">Cascade Soaring Society</asp:HyperLink></li>
                    <li><asp:HyperLink runat="server" NavigateUrl="http://www.spokanesoaring.org/" Target="_blank">Spokane Soaring Society</asp:HyperLink></li>
                    <li><asp:HyperLink runat="server" NavigateUrl="http://wvsc.org/" Target="_blank">Willamette Valley Soaring Club, Oregon</asp:HyperLink></li>
                    <li><asp:HyperLink runat="server" NavigateUrl="http://vancouversoaring.com/" Target="_blank">Vancouver Soaring Association, Hope, British Columbia</asp:HyperLink></li>
                    <li><asp:HyperLink runat="server" NavigateUrl="http://www.soartherockies.com/" Target="_blank">The Invermere Soaring Centre, British Columbia</asp:HyperLink></li>
                    <li><asp:HyperLink runat="server" NavigateUrl="http://www.avsa.ca/" Target="_blank">Alberni Valley Soaring Association, Vancouver Island, British Columbia</asp:HyperLink></li>
                </ul>
            </li>
            <li>Gliders/Tow Planes, Parts, and Supplies
                <ul>
                    <li><asp:HyperLink runat="server" NavigateUrl="https://wingsandwheels.com/" Target="_blank">Wings and Wheels</asp:HyperLink></li>
                    <li><asp:HyperLink runat="server" NavigateUrl="http://www.cumulus-soaring.com/" Target="_blank">Cumulus Soaring Inc.</asp:HyperLink></li>
                    <li><asp:HyperLink runat="server" NavigateUrl="http://spenceraircraft.com/" Target="_blank">Spencer Aircraft</asp:HyperLink></li>
                    <li><asp:HyperLink runat="server" NavigateUrl="http://www.aircraftspruce.com/" Target="_blank">Aircraft Spruce</asp:HyperLink></li>
                    <li><asp:HyperLink runat="server" NavigateUrl="http://www.univair.com/" Target="_blank">Univair Aircraft Corporation</asp:HyperLink></li>
                    <li><asp:HyperLink runat="server" NavigateUrl="http://home.nwi.net/~blanikam/ba/home.htm" Target="_blank">Blanik America</asp:HyperLink></li>
                    <li><asp:HyperLink runat="server" NavigateUrl="http://www.streifly.de/home-e.htm" Target="_blank">Glasfaser Flugzeug-Service Hansjörg Streifeneder</asp:HyperLink></li>
                </ul>
            </li>
            <li>Education, Literature
                <ul>
                    <li><asp:HyperLink runat="server" NavigateUrl="https://www.faa.gov/regulations_policies/handbooks_manuals/aircraft/glider_handbook/media/faa-h-8083-13a.pdf" Target="_blank">FAA Glider Flying Handbook</asp:HyperLink></li>
                    <li><asp:HyperLink runat="server" NavigateUrl="http://www.gliderbooks.com/" Target="_blank">GLIDERBOOKS.com</asp:HyperLink></li>
                    <li><asp:HyperLink runat="server" NavigateUrl="http://www.bobwander.com/Aboutbob.html" Target="_blank">Bob Wander</asp:HyperLink></li>
                </ul>
            </li>
        </ul>
    </div><%-- SCR 213 --%>
</asp:Content>

<asp:Content ID="FooterContent" ContentPlaceHolderID="ContentPlaceHolderFooter" runat="server">
    <%: Title %>
</asp:Content>
