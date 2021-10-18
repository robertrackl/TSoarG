<%@ Page Title="Fleet" Language="C#" MasterPageFile="~/mTSoar.Master" AutoEventWireup="true" CodeBehind="Fleet.aspx.cs" Inherits="TSoar.PublicPages.Fleet" %>

<asp:Content ID="HeaderContent" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="server">
    <asp:Label runat="server" Text="PSSA Aircraft Fleet" Font-Italic="true" />
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="ContentPlaceHolderMain" runat="server">
    <style>
        .tpad {padding:8px;}
    </style>
    <div id="Scrolling-div" style="padding-bottom:0px; padding-top:44px;">
        <div class="container body-content">
            <div class="row tpad" >
                <div class="col-sm-4">Registration</div>
                <div class="col-sm-4">Model and Use</div>
                <div class="col-sm-4"></div>
            </div>
            <div class="row tpad" >
                <div class="col-sm-4">&nbsp;</div>
                <div class="col-sm-4">&nbsp;</div>
                <div class="col-sm-4">&nbsp;</div>
            </div>
            <div class="row tpad" >
                <div class="col-sm-4">N333TM</div>
                <div class="col-sm-4"><asp:HyperLink NavigateUrl="http://en.wikipedia.org/wiki/Piper_PA-18" runat="server">PA-18 Super Cub - Tow plane</asp:HyperLink></div>
                <div class="col-sm-4">
                    <div style="text-align:center">
                        <asp:Image ImageUrl="~/i/Fleet/N333TM.jpg" AlternateText="Tow Plane" runat="server" />
                    </div>
                </div>
            </div>
            <div class="row tpad" >
                <div class="col-sm-4">N157AJ</div>
                <div class="col-sm-4"><asp:HyperLink NavigateUrl="http://en.wikipedia.org/wiki/Politechniki_Warszawskiej_PW-5" runat="server">PW-5 Smyk - Singel Seat Glider</asp:HyperLink><br /><br />
                        <asp:HyperLink NavigateUrl="~/PublicPages/Fleet/PW-5_FlightManual.pdf" runat="server" Target="_blank">Flight Manual</asp:HyperLink>
                    </div>
                <div class="col-sm-4">
                    <div style="text-align:center">
                        <asp:Image ImageUrl="~/i/Fleet/N157AJ.jpg" AlternateText="PW-5 Glider" runat="server" /></div>
                </div>
            </div>
            <div class="row tpad" >
                <div class="col-sm-4">N766PW</div>
                <div class="col-sm-4"><asp:HyperLink NavigateUrl="http://en.wikipedia.org/wiki/PW-6" runat="server">PW-6U - Two Seat Glider</asp:HyperLink><br /><br />
                            <asp:HyperLink NavigateUrl="~/PublicPages/Fleet/PW6UFlightManual.pdf" runat="server" Target="_blank">Flight Manual</asp:HyperLink>
                </div>
                <div class="col-sm-4">
                    <div style="text-align:center">
                        <asp:Image ImageUrl="~/i/Fleet/N766PW.jpg" AlternateText="PW-6U Glider" runat="server" />
                    </div>
                </div>
            </div>
            <div class="row tpad" >
                <div class="col-sm-4">N914B</div>
                <div class="col-sm-4"><asp:HyperLink NavigateUrl="https://en.wikipedia.org/wiki/LET_L-23_Super_Blan%C3%ADk" runat="server">LET L-23 Superblanik - Two Seat Glider</asp:HyperLink><br /><br />
                            <asp:HyperLink NavigateUrl="~/PublicPages/Fleet/L-23_SN_917914_Flight_Manual.pdf" runat="server" Target="_blank">Flight Manual</asp:HyperLink></div>
                <div class="col-sm-4">
                    <div style="text-align:center">
                        <asp:Image ImageUrl="~/i/Fleet/Blanik_3_a.jpg" AlternateText="L-23 Superblanik Glider" runat="server" />
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>

<asp:Content ID="FooterContent" ContentPlaceHolderID="ContentPlaceHolderFooter" runat="server">
    <%: Title %>
</asp:Content>
