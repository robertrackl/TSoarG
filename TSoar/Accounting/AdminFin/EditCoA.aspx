<%@ Page Title="Edit the Chart of Accounts" Language="C#" MasterPageFile="~/mTSoar.Master" AutoEventWireup="true" CodeBehind="EditCoA.aspx.cs"
    Inherits="TSoar.Accounting.EditCoA" %>

<asp:Content ID="HeaderContent" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="server">
    <asp:Label runat="server" Text="Board - Finance - Accounting Administration" Font-Italic="true" />
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="ContentPlaceHolderMain" runat="server">

    <asp:SqlDataSource ID="SqlDataSrc_Type" runat="server" ConnectionString="<%$ ConnectionStrings:SqlConn %>"
        SelectCommand="SELECT ID,sAccountType FROM SF_ACCTTYPES" >
    </asp:SqlDataSource>

    <asp:SqlDataSource ID="SqlDataSrc_Parent" runat="server" ConnectionString="<%$ ConnectionStrings:SqlConn %>"
        SelectCommand="SELECT ID,sCode FROM SF_ACCOUNTS" >
    </asp:SqlDataSource>

    <asp:SqlDataSource ID="SqlDataSrc_Subledger" runat="server" ConnectionString="<%$ ConnectionStrings:SqlConn %>"
        SelectCommand="SELECT ID,sSubLedgerName FROM SF_SUBLEDGERS" >
    </asp:SqlDataSource>

    <h3>Chart of Accounts - Edit</h3>
    <p><a href="AdminFin.aspx">Back to Accounting System Administration - Overview</a></p>
    <p>Reference: <a href="https://www.ifrs-gaap.com/basic-gaap-chart-accounts" target="_blank">GAAP Basic Chart of Accounts</a> </p>
    <p>Show Hierarchical List: <a href="ChrtOActs.aspx">Hierarchy of Accounts</a></p>
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
                <Header>Add New Account</Header>
                <Content>
                    <asp:DetailsView runat="server" ID="dvCoA" AutoGenerateRows="false"
                            DataKeyNames="sCode" AllowPaging="false" AutoGenerateInsertButton="true"
                            CellPadding="10" DefaultMode="Insert" HorizontalAlign="Center" CssClass="SoarNPGridStyle"
                            OnItemInserted="dvCoA_ItemInserted" OnItemInserting="dvCoA_ItemInserting" >
                        <Fields>
                            <asp:TemplateField HeaderText="Account Code">
                                <InsertItemTemplate>
                                    <asp:TextBox ID="txbCode" runat="server" OnTextChanged="txbCode_TextChanged" AutoPostBack="true"></asp:TextBox>
                                </InsertItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Sort Code">
                                <InsertItemTemplate>
                                    <asp:TextBox ID="txbSortCode" runat="server" ></asp:TextBox>
                                </InsertItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Account Name">
                                <InsertItemTemplate>
                                    <asp:TextBox ID="txbName" runat="server" ></asp:TextBox>
                                </InsertItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Account Type">
<%--                                <InsertItemTemplate>
                                    <asp:TextBox ID="txbType" runat="server" ></asp:TextBox>
                                </InsertItemTemplate>--%>
                                <InsertItemTemplate>
                                    <asp:DropDownList ID="DDL_Type" runat="server" DataSourceID="SqlDataSrc_Type"
                                        DataTextField="sAccountType" DataValueField="ID" OnPreRender="DDL_Type_PreRender" />
                                </InsertItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Parent Account Code">
                                <InsertItemTemplate>
                                    <asp:DropDownList ID="DDL_Parent" runat="server" DataSourceID="SqlDataSrc_Parent"
                                        DataTextField="sCode" DataValueField="ID" OnPreRender="DDL_Parent_PreRender" />
                                </InsertItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Notes">
                                <InsertItemTemplate>
                                    <asp:TextBox ID="txbNotes" runat="server" ></asp:TextBox>
                                </InsertItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Subledger">
                                <InsertItemTemplate>
                                    <asp:DropDownList ID="DDL_Subledger" runat="server" DataSourceID="SqlDataSrc_Subledger"
                                        DataTextField="sSubLedgerName" DataValueField="ID" OnPreRender="DDL_Subledger_PreRender" />
                                </InsertItemTemplate>
                            </asp:TemplateField>
                        </Fields>
                    </asp:DetailsView>
                </Content>
            </ajaxToolkit:AccordionPane>
            <ajaxToolkit:AccordionPane runat="server" >
                <Header> Chart of Accounts - Add Accounts, Edit Account Properties </Header>
                <Content>
                    <asp:GridView ID="gvCoA" runat="server" AutoGenerateColumns="False"
                            GridLines="None" CssClass="SoarNPGridStyle"
                            OnRowEditing="gvCoA_RowEditing" OnRowDataBound="gvCoA_RowDataBound"
                            OnRowDeleting="gvCoA_RowDeleting" OnRowUpdating="gvCoA_RowUpdating"
                            OnRowCreated="gvCoA_RowCreated" OnRowCancelingEdit="gvCoA_RowCancelingEdit"
                            OnPageIndexChanging="gvCoA_PageIndexChanging" AllowPaging="true" PageSize="12">
                        <PagerSettings Mode="NextPreviousFirstLast" Position="TopAndBottom" Visible="true" FirstPageText="&nbsp;First&nbsp;"
                            LastPageText="&nbsp;Last&nbsp;" NextPageText="&nbsp;Next&nbsp;" PreviousPageText="&nbsp;Previous&nbsp;" />
                        <PagerStyle CssClass="SoarNPpaging" />
                        <Columns>
                            <asp:TemplateField HeaderStyle-Width="5" Visible="false" HeaderText="Internal ID">
                                <ItemTemplate>
                                    <%# Eval("ID") %>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField ItemStyle-Width="60" HeaderText="&nbsp;Account Code&nbsp;">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblCode" Text='<%# Eval("sCode") %>'/>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:textBox ID="txbUCode" runat="server" text='<%# Eval("sCode") %>' Width="60"/>
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField ItemStyle-Width="90" HeaderText="&nbsp;Sort Code&nbsp;">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblSortCode" Text='<%# Eval("sSortCode") %>'/>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:textBox ID="txbUSortCode" runat="server" text='<%# Eval("sSortCode") %>' Width="90"/>
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField ItemStyle-Width="250" HeaderText="&nbsp;Account Name&nbsp;">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblName" Text='<%# Eval("sName") %>'/>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:textBox ID="txbUName" runat="server" text='<%# Eval("sName") %>' Width="250"/>
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField ItemStyle-Width="80" HeaderText="&nbsp;Account Type&nbsp;">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblType" Text='<%# Eval("Account_Type") %>'/>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:DropDownList ID="DDLType" runat="server" DataSourceID="SqlDataSrc_Type"
                                        DataTextField="sAccountType" DataValueField="ID" />
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField ItemStyle-Width="60" HeaderText="&nbsp;Parent Account Code&nbsp;">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblParent" Text='<%# Eval("Parent_Account_Code") %>'/>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:DropDownList ID="DDLParent" runat="server" DataSourceID="SqlDataSrc_Parent"
                                        DataTextField="sCode" DataValueField="ID" />
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField ItemStyle-Width="60" HeaderText="&nbsp;Subledger&nbsp;">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblSubledger" Text='<%# Eval("SubLedger_Name") %>'/>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:DropDownList ID="DDLSubledger" runat="server" DataSourceID="SqlDataSrc_Subledger"
                                        DataTextField="sSubLedgerName" DataValueField="ID" />
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
