<%@ Page Title="Bylaws and Operation Rules" Language="C#" MasterPageFile="~/mTSoar.Master" AutoEventWireup="true" CodeBehind="ByLawsOpsRules.aspx.cs" Inherits="TSoar.PublicPages.ByLawsOpsRules" %>

<asp:Content ID="HeaderContent" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="server">
    <asp:Label runat="server" Text="PSSA Bylaws and Operation Rules" Font-Italic="true" />
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="ContentPlaceHolderMain" runat="server">
    <div class="HelpText" ><%-- SCR 213 --%>
        <p>Links to Important Club Documents:</p>
        <ul>
            <li><asp:HyperLink NavigateUrl="bylaws.pdf" runat="server" Target="_blank">Bylaws</asp:HyperLink></li>
            <li><asp:HyperLink NavigateUrl="operations.pdf" runat="server" Target="_blank">Operation Rules</asp:HyperLink></li>
        </ul>
    </div><%-- SCR 213 --%>
</asp:Content>

<asp:Content ID="FooterContent" ContentPlaceHolderID="ContentPlaceHolderFooter" runat="server">
    <%: Title %>
</asp:Content>
