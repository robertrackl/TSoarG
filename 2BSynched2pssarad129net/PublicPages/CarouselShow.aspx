<%@ Page Title="TSoar Carousel Show" Language="C#" MasterPageFile="~/mTSoar.Master" AutoEventWireup="true" CodeBehind="CarouselShow.aspx.cs"
    Inherits="TSoar.PublicPages.CarouselShow" %>

<asp:Content ID="HeaderContent" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="server">
    <asp:Label runat="server" Text="Slide Show" Font-Italic="true" />
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="ContentPlaceHolderMain" runat="server">
    <style>
        .container {
            margin-left: auto;
            margin-right: auto;
        }
        .carousel-inner {
            margin-bottom:100px;
        }
    </style>
    <div class="container">
        <div id="myCarousel" class="carousel slide" data-ride="carousel">
            <asp:Panel runat="server" BorderColor="Turquoise" BorderWidth="15px" BorderStyle="Inset" Width="95%" >
                <!-- Images-->
                <div class="carousel-inner" role="listbox">
                    <asp:Literal ID="ltlCarouselImages" runat="server" />
                </div>
                <!-- Indicators -->
                <ol class="carousel-indicators">
                <asp:Literal ID="ltlCarouselIndicators" runat="server"  />
                </ol>
                <!-- Left Right Arrows -->
                <a class="left carousel-control morebottom" href="#myCarousel" role="button" data-slide="prev">
                    <span class="glyphicon glyphicon-chevron-left" aria-hidden="true"></span>
                    <span class="sr-only">Previous</span>
                </a>
                <a class="right carousel-control morebottom" href="#myCarousel" role="button" data-slide="next">
                    <span class="glyphicon glyphicon-chevron-right" aria-hidden="true"></span>
                    <span class="sr-only">Next</span>
                </a>
            </asp:Panel>
        </div>
    </div>
</asp:Content>

<asp:Content ID="FooterContent" ContentPlaceHolderID="ContentPlaceHolderFooter" runat="server">
    <%: Title %>
</asp:Content>
