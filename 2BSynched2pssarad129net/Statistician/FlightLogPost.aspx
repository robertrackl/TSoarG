<%@ Page Title="FlightLogPost" Language="C#" MasterPageFile="~/mTSoar.Master" AutoEventWireup="true" CodeBehind="FlightLogPost.aspx.cs" Inherits="TSoar.Statistician.FlightLogPost" %>

<asp:Content ID="ContentHeader" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="server">
    <asp:Label runat="server" Text="Post one Daily Log Sheet to Database Tables" Font-Italic="true" />
</asp:Content>

<asp:Content ID="ContentMain" ContentPlaceHolderID="ContentPlaceHolderMain" runat="server">

    <div>
        <%-- ModalPopupExtender, popping up UPMPE_Panel and dynamically populating lblPopupText --%>
        <asp:LinkButton ID="UPTarget" runat="server" Text="T" CssClass="displayNone" />
        <ajaxToolkit:ModalPopupExtender ID="UPModalPopExt" runat="server"
            TargetControlID="UPTarget" PopupControlID="UPMPE_Panel"
            BackgroundCssClass="background"
             />
        <asp:Panel ID="UPMPE_Panel" runat="server" CssClass="popup" style="display:none;" HorizontalAlign="Center">
            <asp:Timer ID="UPTimer1" OnTick="UPTimer1_Tick" runat="server" Interval="5" Enabled="false" />
      
            <asp:UpdatePanel ID="UPPanel" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true" >
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="UPTimer1" />
            </Triggers>
            <ContentTemplate>
                <asp:Label id="lblCounter" runat="server" Text="-" /> <asp:Label ID="lblAll" runat="server" Text="-" />
                </br></br>
                <asp:Button ID="pbOK" runat="server" Text="Dismiss" Visible="false" OnClick="pbOK_Click" />
            </ContentTemplate>
            </asp:UpdatePanel>
        </asp:Panel>
    </div>

</asp:Content>

<asp:Content ID="ContentFooter" ContentPlaceHolderID="ContentPlaceHolderFooter" runat="server">
    <%: Title %>
</asp:Content>
