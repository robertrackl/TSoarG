<%@ Page Title="Map for Finding Bergseth Field" Language="C#" MasterPageFile="~/mTSoar.Master" AutoEventWireup="true" CodeBehind="Directions.aspx.cs" Inherits="TSoar.PublicPages.Directions" %>

<asp:Content ID="HeaderContent" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="server">
    <br />
    <%: Title %>, Puget Sound Soaring Association's Home Base
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="ContentPlaceHolderMain" runat="server">
<!DOCTYPE html>
    <div class="HelpText" ><%-- SCR 213 --%>
        <p>Physical Address of Puget Sound Soaring Association's home field, Bergseth Field:<br />
        31500 SE 408th Street, Enumclaw, Washington, 98022</p>
        <p>GPS Coordinates: Latitude 47.243686, Longitude -121.923686; or 47&deg;14&#39;37.3&#34;N 121&deg;55&#39;25.3&#34;W</p>
        <p>The mailing address is different - find it <a href="Contact.aspx">here</a>.</p>
        <%--    <!--The div element for the map -->
        <div id="map"></div>
        <script>
            // Initialize and add the map
            function initMap() {
              // The location of Bergseth FIeld
              var bergseth = {lat: 47.243686, lng: -121.923686};
              // The map, centered at bergseth
              var map = new google.maps.Map(document.getElementById('map'), {zoom: 10, center: bergseth, mapTypeId: google.maps.MapTypeId.ROADMAP});
              // The marker, positioned at bergseth
                var marker = new google.maps.Marker({ position: bergseth, map: map, title: "Bergseth FIeld" });
            }
        </script>
        <!--Load the API from the specified URL
        * The async attribute allows the browser to render the page while the API loads
        * The key parameter will contain your own API key (which is not needed for this tutorial)
        * The callback parameter executes the initMap() function
        -->
        <script async defer
        src="https://maps.googleapis.com/maps/api/js?key=AIzaSyDZoeorP__kT9TnUI-qFpoIixcygUzAM6c&callback=initMap">
        </script>--%>
        <a href="https://www.google.com/maps/place/Puget+Sound+Soaring+Association+Inc./@47.2441706,-121.9254077,17z/data=!3m1!4b1!4m5!3m4!1s0x5490f2c63ce6c9ff:0x56e2cf4af5c02b7d!8m2!3d47.244167!4d-121.923219?hl=en"
            target="_blank">Click here for a <u>Map</u> to Puget Sound Soaring Association</a>
    </div><%-- SCR 213 --%>
</asp:Content>

<asp:Content ID="FooterContent" ContentPlaceHolderID="ContentPlaceHolderFooter" runat="server">
    <%: Title %>
</asp:Content>
