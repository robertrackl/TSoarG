<%@ Page Title="TIRewardsEdit" Language="C#" MasterPageFile="~/mTSoar.Master" AutoEventWireup="true" CodeBehind="TIRewardsEdit.aspx.cs" Inherits="TSoar.Statistician.TIRewardsEdit" %>

<asp:Content ID="HeaderContent" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="server">
    <asp:Label runat="server" Text="Work with Towpilot and Instructor Rewards" Font-Italic="true" />
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="ContentPlaceHolderMain" runat="server">
    <h3>Towpilot and Instructor Rewards Journal</h3>
    <%-- TabContainer added in SCR 223 --%>
    <ajaxToolkit:TabContainer ID="TabC_RewardsJ" runat="server" ActiveTabIndex="0">
        <ajaxToolkit:TabPanel runat="server" HeaderText="Rewards Journal">
            <ContentTemplate>
                <p>A rewards filter <asp:Label runat="server" ID="lbl_is" Text="is" /> currently <asp:Label runat="server" ID="lbl_not" Text="not" /> in effect
                    <asp:Label runat="server" ID="lbl_filter" Text="." /></p>
                <a href="TIRewardsFilter.aspx">Work with Filter Settings</a><br />
                <a href="TIRewards1Member.aspx">Show list of rewards activity for one member</a>
                <asp:panel id="pnlMaxRows" runat="server">Showing at most <asp:Label ID="lblMaxRows" runat="server" Text="25" /> rows from the <asp:Label ID="lblTopBottom" runat="server" Text="Bottom"/>.</asp:panel>
                <asp:panel id="pnlShowRows" runat="server">Showing all rows.</asp:panel>
                <div class="gvclass">
                    <asp:GridView ID="gvRewards" runat="server" AutoGenerateColumns="False"
                        GridLines="None" CssClass="SoarNPGridStyle"
                        OnRowEditing="gvRewards_RowEditing"
                        OnRowDataBound="gvRewards_RowDataBound"
                        OnRowDeleting="gvRewards_RowDeleting" OnRowCancelingEdit="gvRewards_RowCancelingEdit"
                        OnRowUpdating="gvRewards_RowUpdating"
                        ShowHeaderWhenEmpty="true"
                        Font-Size="Small">
                        <Columns>
                            <asp:TemplateField ItemStyle-HorizontalAlign="Right">
                                <HeaderTemplate>
                                    <asp:Label ID="lblHID" runat="server" Text="Internal Id"></asp:Label>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="lblIID" runat="server" Text='<%# Eval("ID") %>' ></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField>
                                <HeaderTemplate>
                                    <asp:Label ID="lblHEC" runat="server" Text="E/C" Width="45"
                                        ToolTip="`Earned` or `Claimed`.
A check mark in the editing row (orange/gold-framed boxes) indicates an 'earned' lot of rewards service points; absence of a check mark there means a 'claimed' lot.
`Reset` or `Clean Up` records are also reported as `Earned` even though they are not."></asp:Label>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="lblIEC" runat="server" Text='<%# ((bool)Eval("bEC")) ? "Earned" : "Claimed" %>' Width="45" ></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:CheckBox ID="chbDEC" runat="server" Checked='<%# Eval("bEC") %>' CssClass="largeCheckBox" AutoPostBack="true"
                                        OnCheckedChanged="chbDEC_CheckedChanged"></asp:CheckBox>
                                </EditItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField>
                                <HeaderTemplate>
                                    <asp:Label ID="lblHDEarn" runat="server" Text="Earn Date" Width="130" ToolTip="Date on which this lot of reward points was earned"></asp:Label>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="lblIDEarn" runat="server" Text='<%# TSoar.CustFmt.sFmtDate((DateTimeOffset)Eval("DEarn"),TSoar.CustFmt.enDFmt.DateOnly) %>' Width="130"></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox ID="txbDDEarn" runat="server" Text='<%# TSoar.CustFmt.sFmtDate((DateTimeOffset)Eval("DEarn"),TSoar.CustFmt.enDFmt.DateOnly).Replace("/","-") %>'
                                        TextMode="Date" Width="130" ></asp:TextBox>
                                </EditItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField>
                                <HeaderTemplate>
                                    <asp:Label ID="lblHDExpiry" runat="server" Text="Expiry Date" Width="130"
                                        ToolTip="Date on which this lot of reward points expires. This column also shows the dates of Reset and Clean Up entries."></asp:Label>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="lblIDExpiry" runat="server" Text='<%# TSoar.CustFmt.sFmtDate((DateTimeOffset)Eval("DExpiry"),TSoar.CustFmt.enDFmt.DateOnly) %>' Width="130"></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox ID="txbDDExpiry" runat="server" Text='<%# TSoar.CustFmt.sFmtDate((DateTimeOffset)Eval("DExpiry"),TSoar.CustFmt.enDFmt.DateOnly).Replace("/","-") %>'
                                        TextMode="Date" Width="130" ></asp:TextBox>
                                </EditItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField>
                                <HeaderTemplate>
                                    <asp:Label ID="lblHDClaim" runat="server" Text="Claim Date" Width="120" ToolTip="Date on which this lot of reward points was claimed"></asp:Label>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="lblIDClaim" runat="server" Text='<%# TSoar.CustFmt.sFmtDate((DateTimeOffset)Eval("DClaim"),TSoar.CustFmt.enDFmt.DateOnly) %>' Width="130"></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox ID="txbDDClaim" runat="server" Text='<%# TSoar.CustFmt.sFmtDate((DateTimeOffset)Eval("DClaim"),TSoar.CustFmt.enDFmt.DateOnly).Replace("/","-") %>' TextMode="Date" Width="130"></asp:TextBox>
                                </EditItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField>
                                <HeaderTemplate>
                                    <asp:Label ID="lblHMember" runat="server" Text="Member" Width="200" ToolTip="The tow pilot/instructor member to whom this lot of service points applies"></asp:Label>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="lblIMember" runat="server" Text='<%# Eval("sDisplayName") %>' Width="200"></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:DropDownList ID="DDLDMember" runat="server" Width="200" />
                                </EditItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField>
                                <HeaderTemplate>
                                    <asp:Label ID="lblHSvcPts" runat="server" Text="Service Points" Width="50"></asp:Label>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="lblISvcPts" runat="server" Text='<%# Eval("iServicePts") %>' Width="50"></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox ID="txbDSvcPts" runat="server" Text='<%# Eval("iServicePts") %>' TextMode="Number" Width="50" Style="min-width:100%;"></asp:TextBox>
                                </EditItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField>
                                <HeaderTemplate>
                                    <asp:Label ID="lblHCode" runat="server" Text="Code" ToolTip="
            &nbsp;C = Claim
            &nbsp;G = Gift Claim
            &nbsp;I = three points for each flight instructed (usually in one day)
            &nbsp;M = manual entry (use only for unusual situation, i.e., hardly ever)
            &nbsp;S = extra points for signing up and then showing up
            &nbsp;T = three points for each tow flown (usually in one day)
            &nbsp;R = reset running cumulative claim sums to zero
            &nbsp;X = summary entry for cleaning up expired points
            ">
                                    </asp:Label>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="lblICode" runat="server" Text='<%# sECCode((char)Eval("cECCode")) %>'></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:DropDownList ID="DDLDECCode" runat="server">
                                        <asp:ListItem Value="C">Claim</asp:ListItem>
                                        <asp:ListItem Value="G">Gift Claim</asp:ListItem>
                                        <asp:ListItem Value="I">Instructor</asp:ListItem>
                                        <asp:ListItem Value="M">Manual</asp:ListItem>
                                        <asp:ListItem Value="S">Showed Up</asp:ListItem>
                                        <asp:ListItem Value="T">Tow Pilot</asp:ListItem>
                                        <asp:ListItem Value="R">Reset Claims</asp:ListItem>
                                        <asp:ListItem Value="X">Clean up Expired</asp:ListItem>
                                    </asp:DropDownList>
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <%-- SCR 223 start --%>
                            <asp:TemplateField>
                                <HeaderTemplate>
                                    <asp:Label ID="lblHForward" runat="server" Text="Forwarded?" ToolTip="Are these service points to be forwarded to next season?
            Should be checked only when the service points accrued during the last 2 months of a season." ></asp:Label>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <asp:CheckBox ID="chbIForward" runat="server" Checked='<%# ((bool)Eval("bForward")) %>' Enabled="false" ></asp:CheckBox>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:CheckBox ID="chbEForward" runat="server" Checked='<%# ((bool)Eval("bForward")) %>' Enabled="true" ></asp:CheckBox>
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <%-- SCR 223 end --%>
                            <asp:TemplateField>
                                <HeaderTemplate>
                                    <asp:Label ID="lblHNotes" runat="server" Text="Notes/Comments" ></asp:Label>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="lblINotes" runat="server" Text='<%# Eval("sComments") %>' ></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox ID="txbDNotes" runat="server" Text='<%# Eval("sComments") %>' ></asp:TextBox>
                                </EditItemTemplate>
                            </asp:TemplateField>

                            <asp:CommandField ButtonType="Image" ShowEditButton="true" HeaderText="Edit" EditImageUrl="~/i/BlueButton.jpg"
                                CancelImageUrl="~/i/Cancel.jpg" UpdateImageUrl="~/i/Update.jpg" >
                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Height="25px" Width="50px" />
                            </asp:CommandField>

                            <asp:CommandField ButtonType="Image" ShowDeleteButton="true" HeaderText="Delete" DeleteImageUrl="~/i/RedButton.jpg">
                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Height="25px" Width="50px" />
                            </asp:CommandField>

                        </Columns>
                    </asp:GridView>
                </div>
    <%-- SCR 223 start --%>
            </ContentTemplate>
        </ajaxToolkit:TabPanel>
        <ajaxToolkit:TabPanel runat="server" HeaderText="Add a Reset entry to the Journal">
            <ContentTemplate>
                <p> A Reset entry is typically entered for the last day of each year of interest.
                    When a listing of a member's service points is generated the Reset entry causes the accumulated claimed and gift-claimed amounts for the year
                    to be set to zero so that the accumulation is restarted.
                </p>
                <p> A Reset entry applies to all members. Therefore, there is no need to enter a member associated with this entry.</p>
                <p> If you need to edit a Reset entry: delete it first in the journal, then come here and re-create it.</p>
                <p> The date of a Reset entry appears in the Earn Date column.</p>
                <p> There should be only one Reset entry at the end of a year.</p>
                <hr />
                <asp:DetailsView ID="dv_Reset" runat="server" AutoGenerateRows="false"
                            AllowPaging="false" AutoGenerateInsertButton="true"
                            CellPadding="10" DefaultMode="Insert" HorizontalAlign="Center" CssClass="SoarNPGridStyle"
                            OnItemInserting="dv_Reset_ItemInserting" OnItemInserted="dv_Reset_ItemInserted"
                            OnModeChanging="dv_Reset_ModeChanging" OnDataBound="dv_Reset_DataBound">
                    <Fields>
                        <asp:TemplateField>
                            <HeaderTemplate>Date: </HeaderTemplate>
                            <InsertItemTemplate>
                                <asp:TextBox ID="txbDate" runat="server" TextMode="Date" />
                            </InsertItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <HeaderTemplate>Comments/Notes: </HeaderTemplate>
                            <InsertItemTemplate>
                                <asp:TextBox ID="txbComments" runat="server" />
                            </InsertItemTemplate>
                        </asp:TemplateField>
                    </Fields>
                </asp:DetailsView>
            </ContentTemplate>
        </ajaxToolkit:TabPanel>
        <ajaxToolkit:TabPanel runat="server" HeaderText="Add a Clean Up entry to the Journal">
            <ContentTemplate>
                A Clean Up entry is typically entered for the last day of each year of interest.
                When a listing of a member's service points is generated the Clean Up entry causes the accumulated service points to be adjusted as follows:
                <ul>
                    <li>Calculate the difference D = (accumulated to-be-forwarded service points) - (accumulated service points)</li>
                    <li>Place D into service points field</li>
                    <li>Set the accumulated to-be-forwarded service points to zero.</li>
                </ul>
                <p>The above procedure gets rid of expired points and starts the accumulation of service points with the amount to be forwarded to the next flying season.</p>
                <p> A Clean Up entry applies to all members. Therefore, there is no need to enter a member associated with this entry.</p>
                <p> If you need to edit a Clean Up entry: delete it first in the journal, then come here and re-create it.</p>
                <p> The date of a Clean Up entry appears in the Earn Date column.</p>
                <p> There should be only one Clean Up entry at the end of a year.</p>
                <hr />
                <asp:DetailsView ID="dv_CleanUp" runat="server" AutoGenerateRows="false"
                            AllowPaging="false" AutoGenerateInsertButton="true"
                            CellPadding="10" DefaultMode="Insert" HorizontalAlign="Center" CssClass="SoarNPGridStyle"
                            OnItemInserting="dv_CleanUp_ItemInserting" OnItemInserted="dv_CleanUp_ItemInserted"
                            OnModeChanging="dv_CleanUp_ModeChanging" OnDataBound="dv_CleanUp_DataBound">
                    <Fields>
                        <asp:TemplateField>
                            <HeaderTemplate>Date: </HeaderTemplate>
                            <InsertItemTemplate>
                                <asp:TextBox ID="txbDate" runat="server" TextMode="Date" />
                            </InsertItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <HeaderTemplate>Comments/Notes: </HeaderTemplate>
                            <InsertItemTemplate>
                                <asp:TextBox ID="txbComments" runat="server" />
                            </InsertItemTemplate>
                        </asp:TemplateField>
                    </Fields>
                </asp:DetailsView>
            </ContentTemplate>
        </ajaxToolkit:TabPanel>
    </ajaxToolkit:TabContainer>
    <%-- SCR 223 end --%>

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

</asp:Content>

<asp:Content ID="FooterContent" ContentPlaceHolderID="ContentPlaceHolderFooter" runat="server">
    <%: Title %>
</asp:Content>
