<%@ Page Title="Home Page [Default]" Language="C#" MasterPageFile="~/mTSoar.Master" AutoEventWireup="true"
    CodeBehind="Default.aspx.cs" Inherits="TSoar.Default" %>

<asp:Content ID="HeaderContent" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="server">
    <asp:Label runat="server" Text="The PSSA Public Home Page" Font-Italic="true" />
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="ContentPlaceHolderMain" runat="server">
    <div class="row">
        <div class="col-sm-4">
            <p>This is Puget Sound Soaring Association's (PSSA) <asp:Label ID="lblAdjective" runat="server" /> web site. Navigate to our
                <a href="PublicPages/Public.aspx"><b>public pages</b></a>
                to find out more about our organization and the sport of soaring.</p>
        </div>
        <div class="col-sm-4">
            <p>PSSA operates weekends March through October just Northeast of Enumclaw, Washington,
                adjacent to the Western slopes of the Cascade mountain range.</p>
        </div>
        <div class="col-sm-4">
            <p>Before you come visit us, <span style="color:red"> be sure to call our operations
                message line at 206-660-0019 after 9:30am </span> to get the latest status on the day's schedule.</p>
        </div>
    </div>
    <div style="text-align:center">
        <asp:UpdatePanel ID="UpdPnlHomeShow" runat="server">
            <ContentTemplate>
                <asp:Timer ID="Timer1" runat="server" Interval="4000" OnTick="Timer1_Tick" Enabled="true" ></asp:Timer>
                <asp:ImageButton ID="Image1" AlternateText="Glider1" runat="server" Width="240px" />
                &nbsp;
                <asp:ImageButton ID="Image2" AlternateText="Glider2" runat="server" Width="240px" />
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
</asp:Content>

<asp:Content ID="FooterContent" ContentPlaceHolderID="ContentPlaceHolderFooter" runat="server">
    <%: Title %>
</asp:Content>
