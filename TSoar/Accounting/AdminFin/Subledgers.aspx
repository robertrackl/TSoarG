<%@ Page Title="Subledgers" Language="C#" MasterPageFile="~/mTSoar.Master" AutoEventWireup="true"
    CodeBehind="Subledgers.aspx.cs" Inherits="TSoar.Accounting.Subledgers" %>

<asp:Content ID="HeaderContent" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="server">
    <asp:Label runat="server" Text="Board - Finance - Accounting Administration - Work with Subledger Definitions" Font-Italic="true" />
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="ContentPlaceHolderMain" runat="server">

    <h3>Chart of Accounts - Work with Subledgers</h3>
    <p><a href="AdminFin.aspx">Back to Accounting System Administration - Overview/a></p>
    <p>Show Hierarchical List of Accounts: <a href="ChrtOActs.aspx">Hierarchy of Accounts</a></p>
    <style>
        .cell-padding{padding:5px}
    </style>
    <ajaxToolkit:Accordion
        ID="AccordionCoA"
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
                <Header>Add New Subledger</Header>
                <Content>
                    <asp:DetailsView runat="server" ID="dvSubledgers" AutoGenerateRows="false"
                            DataKeyNames="ID" AllowPaging="false" AutoGenerateInsertButton="true"
                            CellPadding="10" DefaultMode="Insert" HorizontalAlign="Center" CssClass="SoarNPGridStyle"
                            OnItemInserted="dvSubledgers_ItemInserted" OnItemInserting="dvSubledgers_ItemInserting" >
                        <Fields>
                            <asp:TemplateField HeaderText="Subledger Name">
                                <InsertItemTemplate>
                                    <asp:TextBox ID="txbName" runat="server"></asp:TextBox>
                                </InsertItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Name of Fingering Table">
                                <InsertItemTemplate>
                                    <asp:TextBox ID="txbFingeringTable" runat="server" ></asp:TextBox>
                                </InsertItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Name of Descriptive Field in Fingered Table">
                                <InsertItemTemplate>
                                    <asp:TextBox ID="txbFingeredDescr" runat="server" ></asp:TextBox>
                                </InsertItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Notes">
                                <InsertItemTemplate>
                                    <asp:TextBox ID="txbNotes" runat="server" ></asp:TextBox>
                                </InsertItemTemplate>
                            </asp:TemplateField>
                        </Fields>
                    </asp:DetailsView>
                </Content>
            </ajaxToolkit:AccordionPane>
            <ajaxToolkit:AccordionPane runat="server" >
                <Header> Chart of Accounts - Work with Subledgers </Header>
                <Content>
                    <asp:GridView ID="gvSubledger" runat="server" AutoGenerateColumns="False"
                            GridLines="None" CssClass="SoarNPGridStyle"
                            OnRowEditing="gvSubledger_RowEditing"
                            OnRowDeleting="gvSubledger_RowDeleting" OnRowUpdating="gvSubledger_RowUpdating"
                            OnRowCreated="gvSubledger_RowCreated" OnRowCancelingEdit="gvSubledger_RowCancelingEdit"
                            OnPageIndexChanging="gvSubledger_PageIndexChanging" AllowPaging="true" PageSize="12">
                        <PagerSettings Mode="NextPreviousFirstLast" Position="TopAndBottom" Visible="true" FirstPageText="&nbsp;First&nbsp;"
                            LastPageText="&nbsp;Last&nbsp;" NextPageText="&nbsp;Next&nbsp;" PreviousPageText="&nbsp;Previous&nbsp;" />
                        <PagerStyle CssClass="SoarNPpaging" />
                        <Columns>
                            <asp:TemplateField HeaderStyle-Width="5" Visible="false" HeaderText="Internal ID">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblID" Text='<%# Eval("ID") %>' />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField ItemStyle-Width="120" HeaderText="&nbsp;Subledger Name&nbsp;">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblSubledger" Text='<%# Eval("sSubLedgerName") %>'/>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:textBox ID="txbUSubledger" runat="server" text='<%# Eval("sSubLedgerName") %>' Width="120"/>
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField ItemStyle-Width="120" HeaderText="&nbsp;Fingering Table&nbsp;">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblFinderingTable" Text='<%# Eval("sFingeringTable") %>'/>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:textBox ID="txbUFingeringTable" runat="server" text='<%# Eval("sFingeringTable") %>' Width="120"/>
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField ItemStyle-Width="120" HeaderText="&nbsp;Name of Descriptive Field in Fingered Table&nbsp;">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblFingeredDescr" Text='<%# Eval("sFingeredDescrField") %>'/>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:textBox ID="txbUFingeredDescr" runat="server" text='<%# Eval("sFingeredDescrField") %>' Width="120"/>
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField ItemStyle-Width="200" HeaderText="&nbsp;Notes&nbsp;">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblNotes" Text='<%# Eval("sNotes") %>'/>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:textBox ID="txbUNotes" runat="server" text='<%# Eval("sNotes") %>' Width="200"/>
                                </EditItemTemplate>
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

<asp:Content ID="FooterContent" ContentPlaceHolderID="ContentPlaceHolderFooter" runat="server">
    <%: Title %>
</asp:Content>
