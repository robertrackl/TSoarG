<%@ Page Title="TSoar CMS_BasicList" Language="C#" MasterPageFile="~/mTSoar.Master" AutoEventWireup="true"
    CodeBehind="CMS_BasicList.aspx.cs" Inherits="TSoar.ClubMembership.CMS_BasicList" %>

<asp:Content ID="ContentHeader" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="server">
    <asp:Label runat="server" Text="List of Club Members with Basic Data; Control of Settings Values" Font-Italic="true" />
</asp:Content>

<asp:Content ID="ContentMain" ContentPlaceHolderID="ContentPlaceHolderMain" runat="server">
    <style>
        .cell-padding{padding:5px}
    </style>
    <ajaxToolkit:Accordion
        ID="AccordionCMS"
        runat="Server"
        SelectedIndex="1"
        HeaderCssClass="accordionHeader"
        HeaderSelectedCssClass="accordionHeaderSelected"
        ContentCssClass="accordionContent"
        AutoSize="None"
        FadeTransitions="true"
        TransitionDuration="250"
        FramesPerSecond="40"
        RequireOpenedPane="false">
        <Panes>
            <ajaxToolkit:AccordionPane runat="server">
                <Header>Add New Member</Header>
                <Content>
                    <asp:DetailsView runat="server" ID="dvCMS_BasicList" AutoGenerateRows="false"
                            DataKeyNames="sDisplayName" AllowPaging="false" AutoGenerateInsertButton="true"
                            CellPadding="10" DefaultMode="Insert" HorizontalAlign="Center" CssClass="SoarNPGridStyle"
                            OnItemInserted="dvCMS_BasicList_ItemInserted" OnItemInserting="dvCMS_BasicList_ItemInserting"
                            OnModeChanging="dvCMS_BasicList_ModeChanging">
                        <Fields>
                            <asp:TemplateField HeaderText="Title">
                                <InsertItemTemplate>
                                    <asp:TextBox ID="txbTitle" runat="server" ></asp:TextBox>
                                </InsertItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="First Name">
                                <InsertItemTemplate>
                                    <asp:TextBox ID="txbFirstName" runat="server" ></asp:TextBox>
                                </InsertItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Middle Name/Init">
                                <InsertItemTemplate>
                                    <asp:TextBox ID="txbMiddleName" runat="server" ></asp:TextBox>
                                </InsertItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Last Name">
                                <InsertItemTemplate>
                                    <asp:TextBox ID="txbLastName" runat="server" ></asp:TextBox>
                                </InsertItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Suffix">
                                <InsertItemTemplate>
                                    <asp:TextBox ID="txbSuffix" runat="server" ></asp:TextBox>
                                </InsertItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Display Name">
                                <InsertItemTemplate>
                                    <asp:TextBox ID="txbDisplayName" runat="server" ></asp:TextBox>
                                </InsertItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Notes">
                                <InsertItemTemplate>
                                    <asp:TextBox ID="txbNotes" runat="server" ></asp:TextBox>
                                </InsertItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Date Of Birth">
                                <InsertItemTemplate>
                                    <asp:TextBox ID="txbDOB" runat="server" TextMode="Date"></asp:TextBox>
                                </InsertItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="This Web Site Login User Name">
                                <InsertItemTemplate>
                                    <asp:TextBox ID="txbUserName" runat="server" ></asp:TextBox>
                                    <asp:RegularExpressionValidator runat="server" ValidationExpression="^[a-zA-Z0-9]{6,256}$" ControlToValidate="txbUserName"
                                        ErrorMessage="User name must be at least 6 chars long and at most 256 chars. Allowed chars are a-z, A-Z, 0-9."
                                        Display="Dynamic" Font-Bold="true" ForeColor="Red" Font-Size="Medium" />
                                </InsertItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Customer/Member Identifier in PSSA's QuickBooks Online Company">
                                <InsertItemTemplate>
                                    <asp:TextBox ID="txbIdQBO" runat="server" ></asp:TextBox>
                                </InsertItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Debtor Number in PSSA's FrontAccounting Company">
                                <InsertItemTemplate>
                                    <asp:TextBox ID="txbIDebtorNo" runat="server" TextMode="Number" ></asp:TextBox>
                                </InsertItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Branch Code in PSSA's FrontAccounting Company">
                                <InsertItemTemplate>
                                    <asp:TextBox ID="txbIBranchCode" runat="server" TextMode="Number" ></asp:TextBox>
                                </InsertItemTemplate>
                            </asp:TemplateField>
                        </Fields>
                    </asp:DetailsView>
                </Content>
            </ajaxToolkit:AccordionPane>
            <ajaxToolkit:AccordionPane ID="AccPCMS_BasicList" runat="server">
                <Header> Basic List of Members </Header>
                <Content>
                    <asp:CheckBox ID="chbFilter" runat="server" Text="Filter this list by Display Name containing:" />
                    <asp:TextBox ID="txbFilter" runat="server" Text="" />
                    <asp:Button ID="pbFilter" runat="server" Text="Re-display the List" OnClick="pbFilter_Click" />
                    <asp:GridView ID="gvMembers" runat="server" AutoGenerateColumns="False"
                            GridLines="None" CssClass="SoarNPGridStyle"
                            OnRowEditing="gvMembers_RowEditing"
                            OnRowDeleting="gvMembers_RowDeleting" OnRowUpdating="gvMembers_RowUpdating"
                            OnRowCreated="gvMembers_RowCreated" OnRowCancelingEdit="gvMembers_RowCancelingEdit"
                            OnPageIndexChanging="gvMembers_PageIndexChanging" AllowPaging="true" PageSize="8">
                        <PagerSettings Mode="NextPreviousFirstLast" Position="TopAndBottom" Visible="true" FirstPageText="&nbsp;First&nbsp;"
                            LastPageText="&nbsp;Last&nbsp;" NextPageText="&nbsp;Next&nbsp;" PreviousPageText="&nbsp;Previous&nbsp;" />
                        <PagerStyle CssClass="SoarNPpaging" />
                        <Columns>
                            <asp:TemplateField HeaderStyle-Width="5" Visible="false">
                                <ItemTemplate>
                                    <%# Eval("ID") %>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField ItemStyle-Width="60" HeaderText="&nbsp;Title&nbsp;">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblTitle" Text='<%# sDecodeWNull(Eval("sTitle")) %>'/>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:textBox ID="txbUTitle" runat="server" Text='<%# sDecodeWNull(Eval("sTitle")) %>' Width="60"/>
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField ItemStyle-Width="60" HeaderText="&nbsp;First Name&nbsp;">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblFName" Text='<%# sDecodeWNull(Eval("sFirstName")) %>'/>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:textBox ID="txbFName" runat="server" Text='<%# sDecodeWNull(Eval("sFirstName")) %>' Width="60"/>
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField ItemStyle-Width="60" HeaderText="&nbsp;Middle Name/Initial&nbsp;">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblMName" Text='<%# sDecodeWNull(Eval("sMiddleName")) %>'/>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:textBox ID="txbMName" runat="server" Text='<%# sDecodeWNull(Eval("sMiddleName")) %>' Width="60"/>
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField ItemStyle-Width="60" HeaderText="&nbsp;Last Name&nbsp;">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblLName" Text='<%# sDecodeWNull(Eval("sLastName")) %>'/>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:textBox ID="txbLName" runat="server" Text='<%# sDecodeWNull(Eval("sLastName")) %>' Width="60"/>
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField ItemStyle-Width="60" HeaderText="&nbsp;Suffix&nbsp;">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblSuffix" Text='<%# sDecodeWNull(Eval("sSuffix")) %>'/>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:textBox ID="txbUSuffix" runat="server" Text='<%# sDecodeWNull(Eval("sSuffix")) %>' Width="60"/>
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField ItemStyle-Width="120" HeaderText="&nbsp;Display Name&nbsp;">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblDName" Text='<%# sDecodeWNull(Eval("sDisplayName")) %>'/>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:textBox ID="txbDName" runat="server" Text='<%# sDecodeWNull(Eval("sDisplayName"), true) %>' Width="120"/>
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField ItemStyle-Width="200" HeaderText="&nbsp;Notes&nbsp;">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblNotes" Text='<%# sDecodeWNull(Eval("sNotes")) %>'/>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:textBox ID="txbNotes" runat="server" Text='<%# sDecodeWNull(Eval("sNotes")) %>' Width="200"/>
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField ItemStyle-Width="140" HeaderText="&nbsp;Date of Birth yyyy/mm/dd&nbsp;">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblDOB" Text='<%# Eval("DateOfBirth") %>'/>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:textBox ID="txbDOB" runat="server" Text='<%# Eval("DateOfBirth") %>' Width="140"/>
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField ItemStyle-Width="100" HeaderText="&nbsp;Web Site User Name&nbsp;">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblUserName" Text='<%# Eval("sUserName") %>'/>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:textBox ID="txbUserName" runat="server" Text='<%# Eval("sUserName") %>' Width="100"/>
                                    <asp:RegularExpressionValidator runat="server" ValidationExpression="^[a-zA-Z0-9]{6,256}$" ControlToValidate="txbUserName"
                                        ErrorMessage="User name must be at least 6 chars long and at most 256 chars. Allowed chars are a-z, A-Z, 0-9."
                                        Display="Dynamic" Font-Bold="true" ForeColor="Red" Font-Size="Medium" />
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField ItemStyle-Width="100" HeaderText="&nbsp;QBO Id&nbsp;">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblIdQBO" Text='<%# sDecodeWNull(Eval("sIdQBO")) %>'/>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:textBox ID="txbIdQBO" runat="server" Text='<%# sDecodeWNull(Eval("sIdQBO")) %>' Width="100"/>
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField ItemStyle-Width="100" HeaderText="&nbsp;FA Debtor Number&nbsp;">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblIFADebtorNo" Text='<%# Eval("iFA_debtor_no") %>'/>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:textBox ID="txbIFADebtorNo" runat="server" Text='<%# Eval("iFA_debtor_no") %>' TextMode="Number" Width="100"/>
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField ItemStyle-Width="100" HeaderText="&nbsp;FA Branch Code&nbsp;">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblIFABranchCode" Text='<%# Eval("iFA_branch_code") %>'/>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:textBox ID="txbIFAbranchcode" runat="server" Text='<%# Eval("iFA_branch_code") %>' TextMode="Number" Width="100"/>
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField>
                                <HeaderTemplate>
                                    <asp:Label runat="server" Text="Last AUP Accept" />
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="lblDAcceptedAUP" runat="server" Text='<%# Eval("DAcceptedAUP") %>' />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:CommandField ShowEditButton="true" EditText="Modify" HeaderStyle-CssClass="text-center" />
                            <asp:CommandField DeleteText="Delete" ShowDeleteButton="True" HeaderStyle-CssClass="text-center"/>
                        </Columns>
                    </asp:GridView>
                </Content>
            </ajaxToolkit:AccordionPane>        
        </Panes>            
    </ajaxToolkit:Accordion>

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

<asp:Content ID="ContentFooter" ContentPlaceHolderID="ContentPlaceHolderFooter" runat="server">
    <%: Title %>
</asp:Content>
