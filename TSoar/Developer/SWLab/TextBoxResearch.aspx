<%@ Page Title="TextBoxResearch" Language="C#" MasterPageFile="~/mTSoar.Master" AutoEventWireup="true" CodeBehind="TextBoxResearch.aspx.cs" Inherits="TSoar.Developer.SWLab.TextBoxResearch" %>

<asp:Content ID="HeaderContent" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="server">
    <asp:Label runat="server" Text="Research on TextBox Behavior" Font-Italic="true" />
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="ContentPlaceHolderMain" runat="server">
        <ajaxToolkit:Accordion
        ID="AccordionInvoice"
        runat="Server"
        SelectedIndex="0"
        HeaderCssClass="accordionHeader"
        HeaderSelectedCssClass="accordionHeaderSelected"
        ContentCssClass="accordionContent"
        AutoSize="None"
        FadeTransitions="true"
        TransitionDuration="250"
        FramesPerSecond="40"
        RequireOpenedPane="false" >
        <Panes>
            <ajaxToolkit:AccordionPane ID="AccPGenDat" runat="server" >
                <Header>
                    <asp:Label ID="lblAccPHGenDat" runat="server" Text="General Data"  />
                </Header>
                <Content>
                    <asp:Table runat="server" CssClass="SoarNPGridStyle">
                        <asp:TableHeaderRow>
                            <asp:TableHeaderCell CssClass="text-center">Item</asp:TableHeaderCell>
                            <asp:TableHeaderCell CssClass="text-center">Value</asp:TableHeaderCell>
                        </asp:TableHeaderRow>
                        <asp:TableRow>
                            <asp:TableCell HorizontalAlign="Right">Date on Invoices</asp:TableCell>
                            <asp:TableCell HorizontalAlign="Center"><asp:TextBox ID="txbInvDate" runat="server" TextMode="Date" AutoPostBack="true" OnTextChanged="txb_TextChanged"
                                EnableViewState="true" ViewStateMode="Enabled" /></asp:TableCell>
                        </asp:TableRow>
                        <asp:TableRow>
                            <asp:TableCell HorizontalAlign="Right">Flight Ops "From" Date</asp:TableCell>
                            <asp:TableCell HorizontalAlign="Center"><asp:TextBox ID="txbDFrom" runat="server" TextMode="SingleLine" AutoPostBack="true" OnTextChanged="txb_TextChanged"
                                EnableViewState="true" ViewStateMode="Enabled" ToolTip="Create invoices for flight operation dates starting at this date" /></asp:TableCell>
                        </asp:TableRow>
                        <asp:TableRow>
                            <asp:TableCell HorizontalAlign="Right">Flight Ops "To" Date</asp:TableCell>
                            <asp:TableCell HorizontalAlign="Center"><asp:TextBox ID="txbDTo" runat="server" TextMode="Date" AutoPostBack="true" OnTextChanged="txb_TextChanged"
                                EnableViewState="true" ViewStateMode="Enabled" ToolTip="Create invoices for flight operation dates ending with this date" /></asp:TableCell>
                        </asp:TableRow>
                        <asp:TableRow>
                            <asp:TableCell HorizontalAlign="Right">Days to Invoices' Due Dates</asp:TableCell>
                            <asp:TableCell HorizontalAlign="Center"><asp:TextBox ID="txbDays2Due" runat="server" TextMode="Number" Style="width: 60px;"
                                OnTextChanged="txb_TextChanged" EnableViewState="true" ViewStateMode="Enabled" /></asp:TableCell>
                        </asp:TableRow>
                        <asp:TableRow>
                            <asp:TableCell HorizontalAlign="Right">Create Invoices for This Member</asp:TableCell>
                            <asp:TableCell HorizontalAlign="Center">
                                <asp:DropDownList ID="DDL_Member" runat="server" AutoPostBack="true" EnableViewState="true" ViewStateMode="Enabled"
                                                    DataTextField="sDisplayName" DataValueField="ID" />
                            </asp:TableCell>
                        </asp:TableRow>
                        <asp:TableRow>
                            <asp:TableCell HorizontalAlign="Right">Show only those flights for which an invoice still needs to be generated</asp:TableCell>
                            <asp:TableCell HorizontalAlign="Center"><asp:CheckBox ID="chbInvReq" runat="server" Checked="true" AutoPostBack="true"
                                EnableViewState="true" ViewStateMode="Enabled" /></asp:TableCell>
                        </asp:TableRow>
                    </asp:Table>
                </Content>
            </ajaxToolkit:AccordionPane>
        </Panes>            
    </ajaxToolkit:Accordion>
        <br />
    <asp:Label ID="lblT" runat="server" />
    
    <div id="ModalPopupExtender">
        <%-- ModalPopupExtender, popping up MPE_Panel and dynamically populating lblPopupText --%>
        <asp:LinkButton ID="Target" runat="server" Text="T" CssClass="displayNone" />
        <ajaxToolkit:ModalPopupExtender ID="ModalPopExt" runat="server"
            TargetControlID="Target" PopupControlID="MPE_Panel"
            BackgroundCssClass="background"
             />
        <asp:Panel ID="MPE_Panel" runat="server" CssClass="popup" style="display:none;" HorizontalAlign="Center">
            <asp:Label ID="lblPopupText" runat="server" BackColor="#eeb5a2" Font-Bold="true" />
            <br /><br />
            <p> <asp:Button ID="OkButton" runat="server" Text="OK" OnClick="Button_Click" />&nbsp;&nbsp;
                <asp:Button ID="CancelButton" runat="server" Text="Cancel" OnClick="Button_Click" />&nbsp;&nbsp;
                <asp:Button ID="NoButton" runat="server" Text="No" OnClick="Button_Click" />&nbsp;&nbsp;
                <asp:Button ID="YesButton" runat="server" Text="Yes" OnClick="Button_Click"/></p>
        </asp:Panel>
    </div>

    <div id="ModPopExtContextMenu">
        <asp:LinkButton ID="LinkButton1" runat="server" Text="T" CssClass="displayNone" />
        <ajaxToolkit:ModalPopupExtender ID="ModPopExtCM" runat="server"
            TargetControlID="LinkButton1" PopupControlID="MPE_PanelCM"
            BackgroundCssClass="background" />
        <asp:Panel ID="MPE_PanelCM" runat="server" CssClass="popup" style="display:none;" HorizontalAlign="Center">
            For transaction with ID <asp:Label ID="lblMPEOpsID" runat="server" Text="0" />:
            <p> <asp:Button ID="pbDoInvoice" runat="server" Text="Create Invoice(s)" OnClick="pbCM_Click" ToolTip="Create invoice(s) from this flight operation" /><br />
                <asp:Button ID="pbSetFirst" runat="server" Text="Set First Operation" OnClick="pbCM_Click" ToolTip="Set this flight operation as the first to use for creating a series of invoices" /><br />
                <asp:Button ID="pbSetLast" runat="server" Text="Set Last Operation" OnClick="pbCM_Click" ToolTip="Set this flight operation as the last to use for creating a series of invoices" /><br />
                <asp:Button ID="pbCancel" runat="server" Text="Cancel" OnClick="pbCM_Click" ToolTip="Do nothing and return to the list of flight operations." /></p>
        </asp:Panel>
    </div>

</asp:Content>

<asp:Content ID="FooterContent" ContentPlaceHolderID="ContentPlaceHolderFooter" runat="server">
    <%: Title %>
</asp:Content>
