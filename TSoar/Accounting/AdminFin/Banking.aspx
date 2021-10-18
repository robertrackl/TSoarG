<%@ Page Title="Banking" Language="C#" MasterPageFile="~/mTSoar.Master" AutoEventWireup="true"
    CodeBehind="Banking.aspx.cs" Inherits="TSoar.Accounting.Banking" %>

<asp:Content ID="HeaderContent" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="server">
    <asp:Label runat="server" Text="Board - Finance - Accounting System Administration - Banking" Font-Italic="true" />
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="ContentPlaceHolderMain" runat="server">

    <asp:SqlDataSource ID="SqlDataSrc_FinInst" runat="server" ConnectionString="<%$ ConnectionStrings:SqlConn %>"
        SelectCommand="SELECT ID,sFinancialInstitution FROM SF_FININSTITUTIONS" >
    </asp:SqlDataSource>

    <asp:SqlDataSource ID="SqlDataSrc_BankAcctType" runat="server" ConnectionString="<%$ ConnectionStrings:SqlConn %>"
        SelectCommand="SELECT ID,sBankAcctType FROM SF_BANKACCTTYPES" >
    </asp:SqlDataSource>

    <asp:SqlDataSource ID="SqlDataSrc_Acct" runat="server" ConnectionString="<%$ ConnectionStrings:SqlConn %>"
        SelectCommand="SELECT ID,sCode + ' ' + sName as sAccount FROM SF_ACCOUNTS ORDER BY sAccount" >
    </asp:SqlDataSource>

    <p>Define the details of those accounts in the <a href="ChrtOActs.aspx">Chart of Accounts</a>
        which are bank accounts; edit information about financial institutions and bank account types.</p>
    <p><a href="AdminFin.aspx">Back to Accounting System Administration - Overview</a></p>
    <style>
        .cell-padding{padding:5px}
    </style>
    <ajaxToolkit:Accordion
        ID="AccordionBanking"
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
                <Header>Add New Financial Institution</Header>
                <Content>
                    <asp:DetailsView runat="server" ID="dvFinInst" AutoGenerateRows="false"
                            AllowPaging="false" AutoGenerateInsertButton="true"
                            CellPadding="10" DefaultMode="Insert" HorizontalAlign="Center" CssClass="SoarNPGridStyle"
                            OnItemInserted="dvFinInst_ItemInserted" OnItemInserting="dvFinInst_ItemInserting" >
                        <Fields>
                            <asp:TemplateField HeaderText="Name of Financial Institution">
                                <InsertItemTemplate>
                                    <asp:TextBox ID="txbFinInst" runat="server"></asp:TextBox>
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
                <Header>List of Financial Institutions</Header>
                <Content>
                    <asp:GridView ID="gvFinInst" runat="server" AutoGenerateColumns="False"
                            GridLines="None" CssClass="SoarNPGridStyle"
                            OnRowEditing="gvFinInst_RowEditing"
                            OnRowDeleting="gvFinInst_RowDeleting" OnRowUpdating="gvFinInst_RowUpdating"
                            OnRowCreated="gvFinInst_RowCreated" OnRowCancelingEdit="gvFinInst_RowCancelingEdit"
                            OnPageIndexChanging="gvFinInst_PageIndexChanging" AllowPaging="true" PageSize="12">
                        <PagerSettings Mode="NextPreviousFirstLast" Position="TopAndBottom" Visible="true" FirstPageText="&nbsp;First&nbsp;"
                            LastPageText="&nbsp;Last&nbsp;" NextPageText="&nbsp;Next&nbsp;" PreviousPageText="&nbsp;Previous&nbsp;" />
                        <PagerStyle CssClass="SoarNPpaging" />
                        <Columns>
                            <asp:TemplateField HeaderStyle-Width="5" Visible="false" HeaderText="Internal ID">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblID" Text='<%# Eval("ID") %>' />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField ItemStyle-Width="120" HeaderText="&nbsp;Account Code&nbsp;">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblFinInst" Text='<%# Eval("sFinancialInstitution") %>'/>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:textBox ID="txbUFinInst" runat="server" text='<%# Eval("sFinancialInstitution") %>' Width="120"/>
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
            <ajaxToolkit:AccordionPane runat="server">
                <Header>Add New Bank Account Type</Header>
                <Content>
                    <asp:DetailsView runat="server" ID="dvBankAcctType" AutoGenerateRows="false"
                            DataKeyNames="ID" AllowPaging="false" AutoGenerateInsertButton="true"
                            CellPadding="10" DefaultMode="Insert" HorizontalAlign="Center" CssClass="SoarNPGridStyle"
                            OnItemInserted="dvBankAcctType_ItemInserted" OnItemInserting="dvBankAcctType_ItemInserting" >
                        <Fields>
                            <asp:TemplateField HeaderText="Bank Account Type">
                                <InsertItemTemplate>
                                    <asp:TextBox ID="txbBankAcctType" runat="server"></asp:TextBox>
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
                <Header>List of Bank Account Types</Header>
                <Content>
                    <asp:GridView ID="gvBankAcctType" runat="server" AutoGenerateColumns="False"
                            GridLines="None" CssClass="SoarNPGridStyle"
                            OnRowEditing="gvBankAcctType_RowEditing"
                            OnRowDeleting="gvBankAcctType_RowDeleting" OnRowUpdating="gvBankAcctType_RowUpdating"
                            OnRowCreated="gvBankAcctType_RowCreated" OnRowCancelingEdit="gvBankAcctType_RowCancelingEdit"
                            OnPageIndexChanging="gvBankAcctType_PageIndexChanging" AllowPaging="true" PageSize="12">
                        <PagerSettings Mode="NextPreviousFirstLast" Position="TopAndBottom" Visible="true" FirstPageText="&nbsp;First&nbsp;"
                            LastPageText="&nbsp;Last&nbsp;" NextPageText="&nbsp;Next&nbsp;" PreviousPageText="&nbsp;Previous&nbsp;" />
                        <PagerStyle CssClass="SoarNPpaging" />
                        <Columns>
                            <asp:TemplateField HeaderStyle-Width="5" Visible="false" HeaderText="Internal ID">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblID" Text='<%# Eval("ID") %>' />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField ItemStyle-Width="120" HeaderText="&nbsp;Bank Account Type&nbsp;">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblBankAcctType" Text='<%# Eval("sBankAcctType") %>'/>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:textBox ID="txbUBankAcctType" runat="server" text='<%# Eval("sBankAcctType") %>' Width="120"/>
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
            <ajaxToolkit:AccordionPane runat="server">
                <Header>Add New Bank Account</Header>
                <Content>
                    <asp:DetailsView runat="server" ID="dvBankAcct" AutoGenerateRows="false"
                            AllowPaging="false" AutoGenerateInsertButton="true"
                            CellPadding="10" DefaultMode="Insert" HorizontalAlign="Center" CssClass="SoarNPGridStyle"
                            OnItemInserted="dvBankAcct_ItemInserted" OnItemInserting="dvBankAcct_ItemInserting" >
                        <Fields>
                            <asp:TemplateField HeaderText="Financial Institution">
                                <InsertItemTemplate>
                                    <asp:DropDownList ID="DDL_FinInst" runat="server" DataSourceID="SqlDataSrc_FinInst"
                                        DataTextField="sFinancialInstitution" DataValueField="ID" OnPreRender="DDL_FinInst_PreRender" />
                                </InsertItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Bank Account Type">
                                <InsertItemTemplate>
                                    <asp:DropDownList ID="DDL_BankAcctType" runat="server" DataSourceID="SqlDataSrc_BankAcctType"
                                        DataTextField="sBankAcctType" DataValueField="ID" OnPreRender="DDL_BankAcctType_PreRender"/>
                                </InsertItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Account Number">
                                <InsertItemTemplate>
                                    <asp:TextBox ID="txbAcctNum" runat="server"></asp:TextBox>
                                </InsertItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Account Nick Name">
                                <InsertItemTemplate>
                                    <asp:TextBox ID="txbName" runat="server" ></asp:TextBox>
                                </InsertItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Associated with this Account in the Chart of Accounts">
                                <InsertItemTemplate>
                                    <asp:DropDownList ID="DDL_AssocAcct" runat="server" DataSourceID="SqlDataSrc_Acct"
                                        DataTextField="sAccount" DataValueField="ID" OnPreRender="DDL_AssocAcct_PreRender"/>
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
                <Header>List of Bank Accounts</Header>
                <Content>
                    <asp:GridView ID="gvBankAcct" runat="server" AutoGenerateColumns="False"
                            GridLines="None" CssClass="SoarNPGridStyle"
                            OnRowEditing="gvBankAcct_RowEditing" OnRowDataBound="gvBankAcct_RowDataBound"
                            OnRowDeleting="gvBankAcct_RowDeleting" OnRowUpdating="gvBankAcct_RowUpdating"
                            OnRowCreated="gvBankAcct_RowCreated" OnRowCancelingEdit="gvBankAcct_RowCancelingEdit"
                            OnPageIndexChanging="gvBankAcct_PageIndexChanging" AllowPaging="true" PageSize="12">
                        <PagerSettings Mode="NextPreviousFirstLast" Position="TopAndBottom" Visible="true" FirstPageText="&nbsp;First&nbsp;"
                            LastPageText="&nbsp;Last&nbsp;" NextPageText="&nbsp;Next&nbsp;" PreviousPageText="&nbsp;Previous&nbsp;" />
                        <PagerStyle CssClass="SoarNPpaging" />
                        <Columns>
                            <asp:TemplateField HeaderStyle-Width="5" Visible="false" HeaderText="Internal ID">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblID" Text='<%# Eval("ID") %>' />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField ItemStyle-Width="120" HeaderText="&nbsp;Financial Institution&nbsp;">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblFinInst" Text='<%# Eval("sFinancialInstitution") %>'/>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:DropDownList ID="DDLFinInst" runat="server" DataSourceID="SqlDataSrc_FinInst"
                                        DataTextField="sFinancialInstitution" DataValueField="ID" Width="120" />
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField ItemStyle-Width="120" HeaderText="&nbsp;Bank Account Type&nbsp;">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblBankAcctType" Text='<%# Eval("sBankAcctType") %>'/>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:DropDownList ID="DDLBankAcctType" runat="server" DataSourceID="SqlDataSrc_BankAcctType"
                                        DataTextField="sBankAcctType" DataValueField="ID" Width="120" />
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField ItemStyle-Width="120" HeaderText="&nbsp;Account Number&nbsp;">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblAcctNum" Text='<%# Eval("sAccountNum") %>'/>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:textBox ID="txbUAcctNum" runat="server" text='<%# Eval("sAccountNum") %>' Width="120"/>
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField ItemStyle-Width="140" HeaderText="&nbsp;Account Nick Name&nbsp;">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblName" Text='<%# Eval("sAccountName") %>'/>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:textBox ID="txbUName" runat="server" text='<%# Eval("sAccountName") %>'  Width="140"/>
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField ItemStyle-Width="160" HeaderText="&nbsp;Associated Account in Chart of Accounts&nbsp;">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblAssocAcct" Text='<%# Eval("sAccount") %>'/>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:DropDownList ID="DDLAssocAcct" runat="server" DataSourceID="SqlDataSrc_Acct"
                                        DataTextField="sAccount" DataValueField="ID"  Width="160"/>
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
