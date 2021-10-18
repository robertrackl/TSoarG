<%@ Page Title="BulkImport" Language="C#" MasterPageFile="~/mTSoar.Master" AutoEventWireup="true" CodeBehind="BulkImport.aspx.cs" Inherits="TSoar.Statistician.BulkImport" %>

<asp:Content ID="ContentHeader" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="server">
    <asp:Label runat="server" Text="Import of Flight Data from a text file" Font-Italic="true" />
</asp:Content>

<asp:Content ID="ContentMain" ContentPlaceHolderID="ContentPlaceHolderMain" runat="server">
    <h4>Import Operational Data from a Specially Formatted Tab-Delimited Text File</h4>
    <asp:Button ID="pbReset" Text="Reset to Initial State" runat="server" Enabled="true" CssClass="buttonEnabled" OnClick="pbReset_Click" />
    <br />
    <asp:Label runat="server" Text="Type debug code (integer):" />
    <asp:TextBox ID="txbDebug" runat="server" Text="0" TextMode="Number" />
    <br /><br />
    <asp:FileUpload ID="FileUploadBulk" runat="server" />
    <br />
    <asp:Button ID="pbOpen" Text="Read from the selected file" runat="server" Enabled="true" CssClass="buttonEnabled" OnClick="pbOpen_Click" />
    <br /><br />
    Comment about the result of import: <asp:Label ID="lblResult" Text="Nothing imported yet ..." runat="server" />
    <br /><br />
    <asp:Button ID="pbCheck" Text="Check the data to be imported from the table below" Enabled="false" CssClass="buttonDisabled" runat="server" OnClick="pbCheck_Click" />
    <br />
    <asp:Button ID="pbAddData" Text="Finally - import the data from the table below" runat="server" Enabled="false" CssClass="buttonDisabled" OnClick="pbAddData_Click" />
    
    <ajaxToolkit:Accordion ID="AccordionBulkImp" runat="server" EnableViewState="true" ViewStateMode="Enabled">
        <Panes>
            <ajaxToolkit:AccordionPane runat="server" EnableViewState="true" ViewStateMode="Enabled">
                <Header>
                    Click here to see the data that has been read from the text file:
                </Header>
                <Content>
                    <asp:Label ID="lblDataRead" runat="server" ViewStateMode="Enabled" EnableViewState="true" Text="Nothing yet" />
                    <asp:GridView ID="gvBulk" runat="server" ViewStateMode="Enabled" EnableViewState="true"/>
                </Content>
            </ajaxToolkit:AccordionPane>
            <ajaxToolkit:AccordionPane runat="server">
                <Header>
                    Click here to see diagnostics regarding imported data:
                </Header>
                <Content>
                    <asp:Label ID="lblDiagnostics" runat="server" ViewStateMode="Enabled" EnableViewState="true" Text="Nothing yet" />
                    <asp:GridView ID="gvDiag" runat="server" ViewStateMode="Enabled" EnableViewState="true" />
                </Content>
            </ajaxToolkit:AccordionPane>
        </Panes>
    </ajaxToolkit:Accordion>

    <div title="ModalPopExt">
        <%-- ModalPopupExtender, popping up MPE_Panel and dynamically populating lblPopupText --%>
        <asp:LinkButton ID="Target" runat="server" Text="T" CssClass="displayNone" />
        <ajaxToolkit:ModalPopupExtender ID="ModalPopExt" runat="server"
            TargetControlID="Target" PopupControlID="MPE_Panel"
            BackgroundCssClass="background" DropShadow="true" />
        <asp:Panel ID="MPE_Panel" runat="server" CssClass="popup" style="display:none;" HorizontalAlign="Center">
            <asp:Label ID="lblPopupText" runat="server" BackColor="#eeb5a2" Font-Bold="true" />
            <br /><br />
            <p> <asp:Button ID="OkButton" runat="server" Text="OK" OnClick="Button_Click" />&nbsp;&nbsp;
                <asp:Button ID="CancelButton" runat="server" Text="Cancel" OnClick="Button_Click" />&nbsp;&nbsp;
                <asp:Button ID="NoButton" runat="server" Text="No" OnClick="Button_Click" />&nbsp;&nbsp;
                <asp:Button ID="YesButton" runat="server" Text="Yes" OnClick="Button_Click"/></p>
        </asp:Panel>
    </div>

</asp:Content>

<asp:Content ID="ContentFooter" ContentPlaceHolderID="ContentPlaceHolderFooter" runat="server">
    <%: Title %>
</asp:Content>
