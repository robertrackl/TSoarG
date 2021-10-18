<%@ Page Title="The PSSA Public Contact Page" Language="C#" MasterPageFile="~/mTSoar.Master" AutoEventWireup="true" CodeBehind="Contact.aspx.cs"
    Inherits="TSoar.PublicPages.Contact" %>

<asp:Content ID="HeaderContent" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="server">
    <br />
    <%: Title %>
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="ContentPlaceHolderMain" runat="server">
    <div class="HelpText" ><%-- SCR 213 --%>
        <address class="text-center">
            Puget Sound Soaring Association, Inc.<br />
            P.O. Box 941<br />
            Enumclaw, WA 98022 USA<br /><br />
            <abbr title="Phone">Phone:</abbr>
            206 660 0019
        </address>
        <address class="text-center">
            <strong>email:</strong>   <a href="mailto:info@pugetsoundsoaring.org">info@pugetsoundsoaring.org</a><br />
        </address>
    </div><%-- SCR 213 --%>
</asp:Content>

<asp:Content ID="FooterContent" ContentPlaceHolderID="ContentPlaceHolderFooter" runat="server">
    <%: Title %>
</asp:Content>
