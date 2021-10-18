<%@ Page Title="Member Financial Status [MemberFinStat]" Language="C#" MasterPageFile="~/mTSoar.Master" AutoEventWireup="true" 
    CodeBehind="MemberFinStat.aspx.cs" Inherits="TSoar.MemberPages.Fin.MemberFinStat" %>

<%@ Register Assembly="System.Web.DataVisualization, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" 
    Namespace="System.Web.UI.DataVisualization.Charting" TagPrefix="datvis" %>

<asp:Content ID="HeaderContent" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="server">
    <asp:Label runat="server" Text="PSSA Member Financial Status" Font-Italic="true" />
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="ContentPlaceHolderMain" runat="server">
	<asp:Table runat="server" class="SoarNPGridStyle">
        <asp:Tablerow>
            <asp:TableCell style="text-align:center" >
                Member Name: <asp:Label ID="lblMName" runat="server" /><br />
                <asp:Label ID="lblEmptyData" runat="server" Text="There is no data for the current user" Visible="true" />
            </asp:TableCell>
        </asp:Tablerow>
        <asp:Tablerow>
            <asp:TableCell style="text-align:center" >
                <h4>Tracking the Charges for Flying Activities</h4>
                <br />
                from <asp:TextBox ID="txbDFrom" runat="server" TextMode="Date" />, 
                to <asp:TextBox ID="txbDTo" runat="server" TextMode="Date" />, 
                for this member: <asp:DropDownList ID="DDLMember" runat="server" DataTextField="sDisplayName"
                    DataValueField="ID" Width="180" OnPreRender="DDLMember_PreRender" />
                &nbsp;&nbsp;&nbsp;&nbsp;
                <asp:Button ID="pbUpdate" runat="server" Text="Update" OnClick="pbUpdate_Click" />
            </asp:TableCell>
        </asp:Tablerow>
        <asp:Tablerow>
            <asp:TableCell >
                <asp:GridView runat="server" ID="gvTFC" RowStyle-HorizontalAlign="Right">
                </asp:GridView>
            </asp:TableCell>
        </asp:Tablerow>
		<asp:Tablerow>
			<asp:TableCell style="text-align:center" >
                <h4>Chart of Cumulative Flying Charges</h4>
            </asp:TableCell>
        </asp:Tablerow>
		<asp:Tablerow>
            <asp:TableCell style="text-align:center" >
                <datvis:CHART id="Chart1"  runat="server" Palette="BrightPastel" BackColor="#F3DFC1" ImageType="Png" ImageLocation="~/TempImages/ChartPic_#SEQ(300,3)" 
                        Width="660px" Height="400px" BorderlineDashStyle="Solid" BackGradientStyle="TopBottom" BorderWidth="2" BorderColor="181, 64, 1">
					<legends>
						<asp:Legend Enabled="true" IsTextAutoFit="False" Name="Default" BackColor="Transparent" Font="Trebuchet MS, 8.25pt, style=Bold" Alignment="Near" Docking="Top"></asp:Legend>
					</legends>
					<borderskin SkinStyle="Emboss"></borderskin>
					<chartareas>
						<asp:ChartArea Name="ChartArea1" BorderColor="64, 64, 64, 64" BorderDashStyle="Solid" BackSecondaryColor="White" BackColor="OldLace" ShadowColor="Transparent" BackGradientStyle="TopBottom">
							<axisy LineColor="64, 64, 64, 64" Title="Dollars">
								<LabelStyle Font="Trebuchet MS, 8.25pt, style=Bold" />
								<MajorGrid LineColor="64, 64, 64, 64" />
							</axisy>
							<axisx LineColor="64, 64, 64, 64" IsMarginVisible="false" Title="Date" >
								<LabelStyle Font="Trebuchet MS, 8.25pt, style=Bold" />
								<MajorGrid LineColor="64, 64, 64, 64" IntervalType="Months" Interval="1" Enabled="true" />
                                <MinorGrid LineColor="64, 64, 64, 64" Enabled="true" IntervalType="Months" Interval="1" />
							</axisx>
						</asp:ChartArea>
					</chartareas>
				</datvis:CHART>
			</asp:TableCell>
		</asp:Tablerow>
	</asp:Table>

    <br />

    <div>
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

</asp:Content>

<asp:Content ID="FooterContent" ContentPlaceHolderID="ContentPlaceHolderFooter" runat="server">
    <%: Title %>
</asp:Content>