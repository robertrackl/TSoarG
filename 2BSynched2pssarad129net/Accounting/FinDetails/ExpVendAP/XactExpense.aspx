<%@ Page Title="XactExpense - Expense Transaction " Language="C#" MasterPageFile="~/mTSoar.Master" AutoEventWireup="true"
    CodeBehind="XactExpense.aspx.cs" Inherits="TSoar.Accounting.FinDetails.ExpVendAP.XactExpense" ViewStateEncryptionMode="Always" %>

<asp:Content ID="HeaderContent" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="server">
    
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="ContentPlaceHolderMain" runat="server">

<div id="DataSources">
    <asp:SqlDataSource ID="SqlDataSrc_EDAcct" runat="server" ConnectionString="<%$ ConnectionStrings:SqlConn %>"
        SelectCommand="SELECT A.ID, A.sCode + ' ' + A.sName AS sAccount FROM SF_ACCOUNTS AS A INNER JOIN SF_ACCTTYPES AS T ON A.iSF_Type = T.ID
            WHERE (T.sAccountType = N'Expense') ORDER BY sAccount" >
    </asp:SqlDataSource>

    <asp:SqlDataSource ID="SqlDataSrc_EDVendor" runat="server" ConnectionString="<%$ ConnectionStrings:SqlConn %>"
        SelectCommand="SELECT V.ID, V.sVendorName FROM SF_VENDORS V ORDER BY sVendorName" >
    </asp:SqlDataSource>

    <asp:SqlDataSource ID="SqlDataSrc_PDAcct" runat="server" ConnectionString="<%$ ConnectionStrings:SqlConn %>"
        SelectCommand="SELECT A.ID, A.sCode + ' ' + A.sName AS sAccount FROM SF_ACCOUNTS AS A INNER JOIN SF_ACCTTYPES AS T ON A.iSF_Type = T.ID
            WHERE (T.sAccountType = N'Assets') ORDER BY sAccount" >
    </asp:SqlDataSource>

    <asp:SqlDataSource ID="SqlDataSrc_PDPmtMeth" runat="server" ConnectionString="<%$ ConnectionStrings:SqlConn %>"
        SelectCommand="SELECT P.ID, P.sPaymentMethod FROM SF_PAYMENTMETHODS P ORDER BY ID" >
    </asp:SqlDataSource>

    <asp:SqlDataSource ID="SqlDataSrc_ACateg" runat="server" ConnectionString="<%$ ConnectionStrings:SqlConn %>"
        SelectCommand="SELECT T.ID, T.sAttachmentCateg FROM SF_ATTACHMENTCATEGS T ORDER BY sAttachmentCateg" >
    </asp:SqlDataSource>
    </div>
    <style>
        .cell-padding{padding:5px}
        .panel_with_padding
        {    padding-top:5px;
             padding-left:5px;
             padding-right:5px;
             padding-bottom:5px; 
             width: 580px;
             margin-left: auto;
             margin-right: auto;
             background-color: white;
        }
        .hinweis {font-size:x-small; background-color:powderblue; color:navy;}
        .fontXsmall {font-size:x-small;}
    </style>
    <div class="container">
        <div class="row">
            <div class="col-sm-12">
<%--            ! NOTE: Control lblNewEdit is referenced in javascript at the end of this code ! --%>
                <asp:Label ID="lblNewEdit" runat="server" Text="Expense" Font-Size="Larger" Font-Bold="true" CssClass="FloatLeft" />
                <asp:Label ID="lblXactId" runat="server" Text="000" CssClass="FloatRight"/>
            </div>
        </div>
    </div>
    <p style="font-size:smaller"><asp:Label ID="lblNote" runat="server" Text="Note: When you prepare input data here but don't save it and do nothing on this page for more than an hour, the data may (and probably will) be lost."></asp:Label></p>
    <asp:Table ID="tblXactDateVendor" runat="server" CssClass="SoarNPGridStyle" BorderColor="Orange" BorderStyle="Ridge" BorderWidth="5px">
        <asp:TableRow >
            <asp:TableCell HorizontalAlign="Right"><asp:Label ID="lblDateAndTime" runat="server" Text="Date and time when the expense occurred:" /></asp:TableCell>
            <asp:TableCell HorizontalAlign="Center"><asp:TextBox ID="txbXactDate" runat="server" Textmode="Date" Width="135px"></asp:TextBox>

            </asp:TableCell><asp:TableCell HorizontalAlign="Center">
                <asp:TextBox ID="txbXactTime" runat="server" Textmode="Time" Text="12:00:00" Width="80px"
                ToolTip="Defaults to noon; change only if time of day is important (usually it is not)"></asp:TextBox>
                <asp:TextBox ID="txbXactOffset" runat="server" TextMode="SingleLine" Text="-08:00" Width="55px"
                    ToolTip="Time offset from Universal Time Coordinated. Change only if it is important (it rarely is). Default comes from Setting 'TimeZoneOffset'. For Pacific Standard Time use -08:00. Limited to +/-14:00."></asp:TextBox>
                <asp:RegularExpressionValidator runat="server" ControlToValidate="txbXactOffset" ValidationExpression="^[-+]((1[0-4])|(0\d)):[0-5]\d$"
                    ErrorMessage="Must be a time offset from local to UTC like -08:00" Display="Dynamic" ForeColor="Red" BorderStyle="Dotted" BorderColor="Red" BorderWidth="4px"/>
            </asp:TableCell>

        </asp:TableRow><asp:TableRow>
            <asp:TableCell HorizontalAlign="Right">Vendor:</asp:TableCell><asp:TableCell HorizontalAlign="Center" ColumnSpan="2">
                <asp:DropDownList ID="DDL_Vendor" runat="server" DataSourceID="SqlDataSrc_EDVendor" Width="300px"
                        DataTextField="sVendorName" DataValueField="ID" OnPreRender="DDL_Vendor_PreRender"/>
            </asp:TableCell></asp:TableRow></asp:Table>
    <span class="hinweis">[To navigate the data in this expense, click on one of the items below in blue print with light blue background.]</span>
    <ajaxToolkit:Accordion
        ID="AccordionExpense"
        runat="Server"
        SelectedIndex="0"
        HeaderCssClass="accordionHeader"
        HeaderSelectedCssClass="accordionHeaderSelected"
        ContentCssClass="accordionContent"
        AutoSize="None"
        FadeTransitions="true"
        TransitionDuration="250"
        FramesPerSecond="40"
        RequireOpenedPane="false"
        >
        <Panes>
            <ajaxToolkit:AccordionPane ID="AccPExpenditure" runat="server">
                <Header>
                    <asp:Label ID="lblAccPHExp" runat="server" Text="Expenditure Detail"  />
                </Header>
                <Content>
                    <div class="gvclass">
                    <asp:GridView ID="gvEExp" runat="server" AutoGenerateColumns="False"
                        GridLines="None" CssClass="SoarNPGridStyle" OnRowCommand="gvEExp_RowCommand"
                        OnRowDataBound="gvEExp_RowDataBound" OnRowEditing="gvEExp_RowEditing"
                        OnRowDeleting="gvEExp_RowDeleting" OnRowCancelingEdit="gvEExp_RowCancelingEdit"
                        OnRowUpdating="gvEExp_RowUpdating" OnDataBound="gvEExp_DataBound"
                        AllowPaging="false" ShowHeaderWhenEmpty="true"
                        Font-Size="Small">
                        <Columns>
                            <asp:TemplateField>
                                <HeaderTemplate>
                                    <asp:Label ID="lblEHLineNum" runat="server" Text="#" Width="30"></asp:Label></HeaderTemplate><ItemTemplate>
                                    <asp:Label ID="lblEDLineNum" runat="server" Text='<%# Eval("LineNum") %>' Width="30"  style="text-align:center"></asp:Label></ItemTemplate></asp:TemplateField><asp:TemplateField>
                                <HeaderTemplate>
                                    <asp:Label ID="lblEHAcct" runat="server" Text="Debit: Expense Account" Width="200"></asp:Label></HeaderTemplate><ItemTemplate>
                                    <asp:Label ID="lblEIAcct" runat="server" Text='<%# Eval("AccountName") %>' Width="200"></asp:Label></ItemTemplate><EditItemTemplate>
                                    <asp:DropDownList ID="DDLEDAcct" runat="server" DataSourceID="SqlDataSrc_EDAcct"
                                        DataTextField="sAccount" DataValueField="ID" Width="200" OnPreRender="DDLEDAcct_PreRender" />
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField>
                                <HeaderTemplate>
                                    <asp:Label ID="lblEHDescr" runat="server" Text="Description" Width="209"></asp:Label></HeaderTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="lblEIDescr" runat="server" Text='<%# Eval("Descr") %>' Width="209"></asp:Label></ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox ID="txbEDDescr" runat="server" Text='<%# Eval("Descr") %>' Width="209"></asp:TextBox></EditItemTemplate></asp:TemplateField>
                            <asp:TemplateField>
                                <HeaderTemplate>
                                    <asp:Label ID="lblEHAmount" runat="server" Text="Amount" Width="100"></asp:Label></HeaderTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="lblEIAmount" runat="server" Text='<%# Eval("sAmount") %>' Width="100" ></asp:Label></ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox ID="txbEDAmount" runat="server" Text='<%# Eval("sAmount") %>' Width="100" TextMode="SingleLine" style="text-align: right"></asp:TextBox>
                                    <asp:RegularExpressionValidator runat="server" ControlToValidate="txbEDAmount" ValidationExpression="^[-+]?((\d{1,3}(,\d{3})*)|(\d*))(\.|\.\d*)?$"
                                        ErrorMessage="Must be a decimal number (optional sign, optional decimal point)" Display="Dynamic" ForeColor="Red" BorderStyle="Dotted" BorderColor="Red" BorderWidth="4px" />
                                </EditItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField ItemStyle-HorizontalAlign="Center">
                                <HeaderTemplate>
                                    Edit

                                </HeaderTemplate>
                                <ItemTemplate>
                                    <asp:ImageButton ID="ipbEEdit" ImageUrl="~/i/BlueButton.jpg" runat="server" CommandName="Edit" OnClientClick="oktoSubmit = true;" />
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:ImageButton ID="ipbEUpdate" ImageUrl="~/i/Update.jpg" runat="server" CommandName="Update" OnClientClick="oktoSubmit = true;" />
                                    <asp:ImageButton ID="ipbECancel" ImageUrl="~/i/Cancel.jpg" runat="server" CommandName="Cancel" OnClientClick="oktoSubmit = true;" />
                                </EditItemTemplate>
                            </asp:TemplateField>

                            <asp:CommandField ButtonType="Image" ShowDeleteButton="true" HeaderText="Delete" DeleteImageUrl="~/i/RedButton.jpg">
                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Height="25px" Width="50px" />
                            </asp:CommandField>

                            <asp:TemplateField>
                                <HeaderTemplate>
                                    <asp:Label ID="lblEHMoveUp" runat="server" Text="Move Up"></asp:Label></HeaderTemplate>
                                <ItemTemplate>
                                    <asp:Button ID="pbEDMoveUp" runat="server" Height="25" Width="25" Text="&#x21DE;" Font-Bold="true" CssClass="center-block" OnClientClick="oktoSubmit = true;"
                                        CommandName="MoveUp" CommandArgument="<%# ((GridViewRow) Container).RowIndex %>" ToolTip="Move the line up with respect to the other expense lines"/>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField>
                                <HeaderTemplate>
                                    <asp:Label ID="lblEHMoveDown" runat="server" Text="Move Down"></asp:Label></HeaderTemplate>
                                <ItemTemplate>
                                    <asp:Button ID="pbEDMoveDown" runat="server" Height="25" Width="25" Text="&#x21DF;" Font-Bold="true" CssClass="center-block" OnClientClick="oktoSubmit = true;"
                                        CommandName="MoveDown" CommandArgument="<%# ((GridViewRow) Container).RowIndex %>" ToolTip="Move the line down with respect to the other expense lines"/>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                    </div>
                </Content>
            </ajaxToolkit:AccordionPane>
            <ajaxToolkit:AccordionPane ID="AccPDiffDebCre" runat="server" >
                <Header>
                    <asp:Label ID="lblDiffText" runat="server" Text="Difference of Credits minus Debits = " Font-Bold="true" />
                    <asp:Label ID="lblDiff" runat="server" Text="0" Font-Bold="true" Font-Underline="true" />
                </Header>
            </ajaxToolkit:AccordionPane>
            <ajaxToolkit:AccordionPane ID="AccPPayment" runat="server" >
                <Header>
                    <asp:Label ID="lblAccPHPmt" runat="server" Text="Payment Detail" />
                </Header>
                <Content>
                    <div class="gvclass">
                    <asp:GridView ID="gvEPmt" runat="server" AutoGenerateColumns="False"
                            GridLines="None" CssClass="SoarNPGridStyle" OnRowCommand="gvEPmt_RowCommand"
                            OnRowDataBound="gvEPmt_RowDataBound" OnRowEditing="gvEPmt_RowEditing"
                            OnRowDeleting="gvEPmt_RowDeleting" OnRowCancelingEdit="gvEPmt_RowCancelingEdit"
                            OnRowUpdating="gvEPmt_RowUpdating" OnDataBound="gvEPmt_DataBound"
                            AllowPaging="false" ShowHeaderWhenEmpty="true"
                            Font-Size="Small">
                        <Columns>
                            <asp:TemplateField>
                                <HeaderTemplate>
                                    <asp:Label ID="lblPHLineNum" runat="server" Text="#" Width="30"></asp:Label></HeaderTemplate><ItemTemplate>
                                    <asp:Label ID="lblPDLineNum" runat="server" Text='<%# Eval("LineNum") %>' Width="30" style="text-align:center"></asp:Label></ItemTemplate></asp:TemplateField><asp:TemplateField>
                                <HeaderTemplate>
                                    <asp:Label ID="lblPHAcct" runat="server" Text="Credit: Asset Account" Width="160"></asp:Label></HeaderTemplate><ItemTemplate>
                                    <asp:Label ID="lblPIAcct" runat="server" Text='<%# Eval("AccountName") %>' Width="160"></asp:Label></ItemTemplate><EditItemTemplate>
                                    <asp:DropDownList ID="DDLPDAcct" runat="server" DataSourceID="SqlDataSrc_PDAcct"
                                        DataTextField="sAccount" DataValueField="ID" Width="160" OnPreRender="DDLPDAcct_PreRender"/>
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField>
                                <HeaderTemplate>
                                    <asp:Label ID="lblPHPmtMeth" runat="server" Text="Payment Method" Width="100"></asp:Label></HeaderTemplate><ItemTemplate>
                                    <asp:Label ID="lblPIPmtMeth" runat="server" Text='<%# Eval("PmtMethodName") %>' Width="100"></asp:Label></ItemTemplate><EditItemTemplate>
                                    <asp:DropDownList ID="DDLPDPmtMeth" runat="server" DataSourceID="SqlDataSrc_PDPmtMeth"
                                        DataTextField="sPaymentMethod" DataValueField="ID" Width="100" OnPreRender="DDLPDPmtMeth_PreRender" />
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField>
                                <%-- Descr field is used here for handling 'Reference' info (like a check number) --%>
                                <HeaderTemplate>
                                    <asp:Label ID="lblPHDescr" runat="server" Text="Reference" Width="140"></asp:Label></HeaderTemplate><ItemTemplate>
                                    <asp:Label ID="lblPIDescr" runat="server" Text='<%# Eval("Descr") %>' Width="140"></asp:Label></ItemTemplate><EditItemTemplate>
                                    <asp:TextBox ID="txbPDDescr" runat="server" Text='<%# Eval("Descr") %>' Width="140"></asp:TextBox></EditItemTemplate></asp:TemplateField>
                            <asp:TemplateField>
                                <HeaderTemplate>
                                    <asp:Label ID="lblPHAmount" runat="server" Text="Amount" Width="100"></asp:Label>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="lblPIAmount" runat="server" Text='<%# Eval("sAmount") %>' Width="100" ></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox ID="txbPDAmount" runat="server" Text='<%# Eval("sAmount") %>' Width="100" TextMode="SingleLine" style="text-align: right">
                                    </asp:TextBox>
                                    <asp:RegularExpressionValidator runat="server" ControlToValidate="txbPDAmount" ValidationExpression="^[-+]?((\d{1,3}(,\d{3})*)|(\d*))(\.|\.\d*)?$"
                                        ErrorMessage="Must be a decimal number (optional sign, optional decimal point)" Display="Dynamic" ForeColor="Red" BorderStyle="Dotted" BorderColor="Red" BorderWidth="4px" />
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField ItemStyle-HorizontalAlign="Center">
                                <HeaderTemplate>
                                    Edit</HeaderTemplate><ItemTemplate>
                                    <asp:ImageButton ID="ipbPEdit" ImageUrl="~/i/BlueButton.jpg" runat="server" CommandName="Edit" OnClientClick="oktoSubmit = true;" />
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:ImageButton ID="ipbPUpdate" ImageUrl="~/i/Update.jpg" runat="server" CommandName="Update" OnClientClick="oktoSubmit = true;" />
                                    <asp:ImageButton ID="ipbPCancel" ImageUrl="~/i/Cancel.jpg" runat="server" CommandName="Cancel" OnClientClick="oktoSubmit = true;" />
                                </EditItemTemplate>
                            </asp:TemplateField>

                            <asp:CommandField ButtonType="Image" ShowDeleteButton="true" HeaderText="Delete" DeleteImageUrl="~/i/RedButton.jpg" >
                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Height="25px" Width="50px" />
                            </asp:CommandField>

                            <asp:TemplateField>
                                <HeaderTemplate>
                                    <asp:Label ID="lblPHMoveUp" runat="server" Text="Move Up"></asp:Label></HeaderTemplate>
                                <ItemTemplate>
                                    <asp:Button ID="pbEDMoveUp" runat="server" Height="25" Width="25" Text="&#x21DE;" Font-Bold="true" CssClass="center-block" OnClientClick="oktoSubmit = true;"
                                        CommandName="MoveUp" CommandArgument="<%# ((GridViewRow) Container).RowIndex %>" ToolTip="Move the line up with respect to the other payment lines"/>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField>
                                <HeaderTemplate>
                                    <asp:Label ID="lblPHMoveDown" runat="server" Text="Move Down"></asp:Label></HeaderTemplate>
                                <ItemTemplate>
                                    <asp:Button ID="pbEDMoveDown" runat="server" Height="25" Width="25" Text="&#x21DF;" Font-Bold="true" CssClass="center-block" OnClientClick="oktoSubmit = true;"
                                        CommandName="MoveDown" CommandArgument="<%# ((GridViewRow) Container).RowIndex %>" ToolTip="Move the line down with respect to the other payment lines"/>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                    </div>
                </Content>
            </ajaxToolkit:AccordionPane>        
            <ajaxToolkit:AccordionPane ID="AccPMemo" runat="server" >
                <Header>
                    <asp:Label ID="lblAccPHMem" runat="server" Text="Memorandum" />
                </Header>
                <Content>
                    &nbsp;&nbsp;<asp:TextBox ID="txbMemo" runat="server" Textmode="MultiLine" Style="min-width:90%; min-height:120px" OnTextChanged="txbMemo_TextChanged"
                        BorderColor="Orange" BorderStyle="Ridge" BorderWidth="5px"></asp:TextBox></Content></ajaxToolkit:AccordionPane>
            <ajaxToolkit:AccordionPane ID="AccPAttach" runat="server" >
                <Header>
                    <asp:Label ID="lblAccPHAtt" runat="server" Text="Attached Files" />
                </Header>
                <Content>
                    <div class="gvclass">
                    <asp:GridView ID="gvAttach" runat="server" AutoGenerateColumns="False"
                        GridLines="None" CssClass="SoarNPGridStyle"
                        OnRowDeleting="gvAttach_RowDeleting"
                        OnRowUpdating="gvAttach_RowUpdating"
                        OnDataBound="gvAttach_DataBound"
                        AllowPaging="false" ShowHeaderWhenEmpty="true"
                        Font-Size="Small">
                    <Columns>
                        <asp:TemplateField>
                            <HeaderTemplate>
                                <asp:Label ID="lblAHLineNum" runat="server" Text="#" Width="30"></asp:Label></HeaderTemplate><ItemTemplate>
                                <asp:Label ID="lblADLineNum" runat="server" Text='<%# Eval("LineNum") %>' Width="30" style="text-align:center"></asp:Label></ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <HeaderTemplate>
                                <asp:Label runat="server" Text="Attachment Category" Width="140"></asp:Label></HeaderTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblAICateg" runat="server" Text='<%# Eval("AttachCategName") %>' Width="140"></asp:Label></ItemTemplate>
                            <EditItemTemplate>
                                <asp:DropDownList ID="DDLACateg" runat="server" DataSourceID="SqlDataSrc_ACateg"
                                    DataTextField="sAttachmentCateg" DataValueField="ID" Width="140" OnPreRender="DDLACateg_PreRender" />
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <HeaderTemplate>
                                <asp:Label ID="lblAHDAssoc" runat="server" Text="Date" Width="125px" ToolTip="A date associated with the document you are attaching." />
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblAIDAssoc" runat="server" Text='<%# Eval("AttachAssocDate") %>' Width="125px" />
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txbAEDAssoc" runat="server" Text='<%# Eval("AttachAssocDate") %>' Width="125px" TextMode="Date" />
                            </EditItemTemplate>

                        </asp:TemplateField>
                        <asp:TemplateField>
                            <HeaderTemplate>
                                <asp:Label runat="server" Text="Name of File to be Attached" Width="300px"></asp:Label>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <div style="overflow:hidden; white-space:nowrap">
                                    <asp:TextBox ID="txbAIFileName" runat="server" Text='<%# Eval("AttachedFileName") %>' Width="300px" Height="50px"
                                        ReadOnly="true" TextMode="MultiLine" Wrap="true" BackColor="LightGray" ForeColor="DarkGreen" />
                                </div>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:Table ID="tblFN" runat="server" BorderWidth="3px" BorderColor="OrangeRed" BorderStyle="Inset" BackColor="Orange" Width="210px" >
                                    <asp:TableRow ID="tblFNRow0" HorizontalAlign="Center">
                                        <asp:TableCell><span class="fontXsmall">New Upload:</span></asp:TableCell><asp:TableCell>
                                            <asp:FileUpload ID="fupAE" runat="server" Width="300px" onchange="oktoSubmit = true;" ClientIDMode="Static" BorderWidth="3px" BorderColor="OrangeRed" BorderStyle="Inset" BackColor="Orange"
                                                ToolTip="A file name prefix will be generated containing the date and time of upload; certain file name rules apply; if violated, an explanation will pop up."/>
                                        </asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow ID="tblFNRow1" HorizontalAlign="Center">
                                        <asp:TableCell>
                                            <asp:Label ID="lblOR" runat="server" Text="OR:" Font-Bold="true" />
                                        </asp:TableCell>
                                        <asp:TableCell ID="tblFNRow1Cell1">
                                            <asp:Button ID="pbAttFile" runat="server" Text="Re-use a previously uploaded file" OnClick="pbAttFile_Click" OnClientClick="oktoSubmit = true;"
                                                    ToolTip="You may attach a file to this transaction that has previously been attached to another transaction.
It is ok to attach the same file to any number of transactions.
The file needs to be uploaded just once with the first transaction that needs it."/>
                                            <br />
                                            <asp:Label ID="lblOldAttFile" runat="server" Text="No File selected" />
                                        </asp:TableCell>
                                    </asp:TableRow>
                                </asp:Table>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <HeaderTemplate>
                                <asp:Label runat="server" Text="Thumb Print" Width="129"></asp:Label></HeaderTemplate><ItemTemplate>
                                <asp:Image ID="imgThumb" runat="server" Width="129" />
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField ItemStyle-HorizontalAlign="Center">
                            <HeaderTemplate>
                                <span class="fontXsmall">(Cannot Edit)</span></HeaderTemplate>
                            <ItemTemplate>
                                <asp:ImageButton ID="ipbAEdit" ImageUrl="~/i/BlueButton.jpg" runat="server" CommandName="Edit" OnClientClick="oktoSubmit = true;" />
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:ImageButton ID="ipbAUpdate" ImageUrl="~/i/Update.jpg" runat="server" CommandName="Update" OnClientClick="oktoSubmit = true;" />
                                <asp:ImageButton ID="ipbACancel" ImageUrl="~/i/Cancel.jpg" runat="server" CommandName="Cancel" OnClientClick="oktoSubmit = true;" />
                            </EditItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField ItemStyle-HorizontalAlign="Center">
                            <HeaderTemplate>
                                Delete
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:ImageButton ID="ipbADelete" ImageUrl="~/i/RedButton.jpg" runat="server" CommandName="Delete" OnClientClick="oktoSubmit = true;" />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    </asp:GridView>
                    </div>
                </Content>
            </ajaxToolkit:AccordionPane>
        </Panes>            
    </ajaxToolkit:Accordion>
    <hr />
    <asp:Panel runat="server" BorderStyle="Solid" BorderWidth="4px" BorderColor="PowderBlue"  Width="580px" BackColor="Navy" HorizontalAlign="Center" Wrap="true" CssClass="panel_with_padding">
        &nbsp;&nbsp; <asp:Button ID="pbSaveClose" runat="server" Text="Save and Close" OnClick="pbSaveClose_Click"
            ToolTip="Save this Expense Transaction, and go back to the List of Expenses" OnClientClick="oktoSubmit = true;" />
        &nbsp;&nbsp;&nbsp;&nbsp; <asp:Button ID="pbSaveNew" runat="server" Text="Save and New" OnClick="pbSaveNew_Click"
            ToolTip="Save this Expense Transaction, and open another new expense transaction form" OnClientClick="oktoSubmit = true;" />
        &nbsp;&nbsp;&nbsp;&nbsp; <asp:Button ID="pbCancelClose" runat="server" Text="Cancel and Close" OnClick="pbCancelClose_Click"
            ToolTip="Abandon any additions or changes to this expense transaction, and go back to the List of Expenses"
            OnClientClick="oktoSubmit = true;"/>
        &nbsp;&nbsp;&nbsp;&nbsp; <asp:Button ID="pbCancelNew" runat="server" Text="Cancel and New" OnClick="pbCancelNew_Click"
            ToolTip="Abandon any additions or changes to this expense transaction, and open another new expense transaction form"
            OnClientClick="oktoSubmit = true;"/>
        &nbsp;&nbsp; 
    </asp:Panel>
    
    <div id="ModalPopupExtender">
<%--         ModalPopupExtender, popping up MPE_Panel and dynamically populating lblPopupText --%>
        <asp:LinkButton ID="Target" runat="server" Text="T" CssClass="displayNone" />
        <ajaxToolkit:ModalPopupExtender ID="ModalPopExt" runat="server"
            TargetControlID="Target" PopupControlID="MPE_Panel"
            BackgroundCssClass="background"
             />
        <asp:Panel ID="MPE_Panel" runat="server" CssClass="popup" style="display:none;" HorizontalAlign="Center">
            <asp:Label ID="lblPopupText" runat="server" BackColor="#eeb5a2" Font-Bold="true" />
            <br /><br />
            <p> <asp:Button ID="OkButton" runat="server" Text="OK" OnClick="Button_Click" OnClientClick="oktoSubmit = true;" />&nbsp;&nbsp;
                <asp:Button ID="CancelButton" runat="server" Text="Cancel" OnClick="Button_Click" OnClientClick="oktoSubmit = true;" />&nbsp;&nbsp; 
                <asp:Button ID="NoButton" runat="server" Text="No" OnClick="Button_Click" OnClientClick="oktoSubmit = true;" />&nbsp;&nbsp;
                <asp:Button ID="YesButton" runat="server" Text="Yes" OnClick="Button_Click" OnClientClick="oktoSubmit = true;" />
            </p>
        </asp:Panel>
    </div>
    
<script type="text/javascript" >
    var oktoSubmit = false; // 
    var isFiredTwice = false;

    window.onload = QInspect;
    function QInspect() {
        var slblText = document.getElementById('<%= lblNewEdit.ClientID %>').innerHTML;
        if (slblText.indexOf("Inspect") > -1) {
            oktoSubmit = true;
        }
    }

    window.onbeforeunload = confirmExit;
    function confirmExit() {
        if (!oktoSubmit) {
            if (navigator.appName == "Microsoft Internet Explorer") {
                if (!isFiredTwice) {
                    event.returnValue = "If you have any unsaved data in the current page, it will be lost.";
                    isFiredTwice = true;
                    setTimeout("isFiredTwice = false;", 0);
                }
            }
            else {
                // For other browsers: (tested with Chrome, but Chrome uses its own message, not the one specified in the next statement)
                event.returnValue = "If you have any unsaved data in the current page, it will be lost.";
            }
        }  
    }
</script>

</asp:Content>

<asp:Content ID="FooterContent" ContentPlaceHolderID="ContentPlaceHolderFooter" runat="server">
    <%: Title %>
</asp:Content>