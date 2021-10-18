<%@ Page Title="TSoar CMS_FromTo" Language="C#" MasterPageFile="~/mTSoar.Master" AutoEventWireup="true"
    CodeBehind="CMS_ClubFromTo.aspx.cs" Inherits="TSoar.ClubMembership.CMS_ClubFromTo" %>

<asp:Content ID="ContentHeader" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="server">
    <asp:Label runat="server" Text="Club Members' History and Current Status of Membership" Font-Italic="true" />
</asp:Content>

<asp:Content ID="ContentMain" ContentPlaceHolderID="ContentPlaceHolderMain" runat="server">
    <style>
        .cell-padding{padding:5px}
    </style>
    <asp:SqlDataSource ID="SqlDataSrc_Member" runat="server" ConnectionString="<%$ ConnectionStrings:SqlConn %>"
        SelectCommand="SELECT ID,sDisplayName FROM PEOPLE" >
    </asp:SqlDataSource>
    <asp:SqlDataSource ID="SqlDataSrc_MCat" runat="server" ConnectionString="<%$ ConnectionStrings:SqlConn %>"
        SelectCommand="SELECT ID,sMemberShipCategory FROM MEMBERSHIPCATEGORIES" >
    </asp:SqlDataSource>
    <ajaxToolkit:Accordion
        ID="AccordionCMS_MbrFromTo"
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
                <Header>Add New Club Membership Details</Header>
                <Content>
                    <asp:DetailsView runat="server" ID="dvCMS_MbrFromTo" AutoGenerateRows="false"
                            DataKeyNames="sDisplayName" AllowPaging="false" AutoGenerateInsertButton="true"
                            CellPadding="10" DefaultMode="Insert" HorizontalAlign="Center" CssClass="SoarNPGridStyle"
                            OnItemInserting="dvCMS_MbrFromTo_ItemInserting" OnModeChanging="dvCMS_MbrFromTo_ModeChanging">
                        <Fields>
                            <asp:TemplateField HeaderText="Member Display Name" >
                                <InsertItemTemplate>
                                    <asp:DropDownList ID="DDL_Member" runat="server" DataSourceID="SqlDataSrc_Member"
                                        DataTextField="sDisplayName" DataValueField="ID" />
                                </InsertItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Membership Category">
                                <InsertItemTemplate>
                                    <asp:DropDownList ID="DDL_MCat" runat="server" DataSourceID="SqlDataSrc_MCat"
                                        DataTextField="sMembershipCategory" DataValueField="ID" OnPreRender="DDL_MCat_PreRender" />
                                </InsertItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Date Membership Began">
                                <InsertItemTemplate>
                                    <asp:TextBox ID="txb_MBegan" runat="server" TextMode="Date" ></asp:TextBox>
                                </InsertItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Date Membership Ended">
                                <InsertItemTemplate>
                                    <asp:TextBox ID="txb_MEnd" runat="server" TextMode="Date" ></asp:TextBox>
                                </InsertItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Notes">
                                <InsertItemTemplate>
                                    <asp:TextBox ID="txb_Notes" runat="server" TextMode="MultiLine" ></asp:TextBox>
                                </InsertItemTemplate>
                            </asp:TemplateField>
                        </Fields>
                    </asp:DetailsView>
                </Content>
            </ajaxToolkit:AccordionPane>
            <ajaxToolkit:AccordionPane runat="server" >
                <Header> Club Membership - Past and Present </Header>
                <Content>
                    <asp:GridView ID="gvCMS_MbrFromTo" runat="server" AutoGenerateColumns="False"
                            GridLines="None" CssClass="SoarNPGridStyle"
                            OnRowCreated="gvCMS_MbrFromTo_RowCreated"
                            OnRowDeleting="gvCMS_MbrFromTo_RowDeleting"
                            OnRowEditing="gvCMS_MbrFromTo_RowEditing"
                            OnRowDataBound="gvCMS_MbrFromTo_RowDataBound"
                            OnRowCancelingEdit="gvCMS_MbrFromTo_RowCancelingEdit"
                            OnRowUpdating="gvCMS_MbrFromTo_RowUpdating"
                            AllowPaging="true" PageSize="36" 
                            OnPageIndexChanging="gvCMS_MbrFromTo_PageIndexChanging">
                        <PagerSettings Mode="NextPreviousFirstLast" Position="TopAndBottom" Visible="true" FirstPageText="&nbsp;First&nbsp;"
                            LastPageText="&nbsp;Last&nbsp;" NextPageText="&nbsp;Next&nbsp;" PreviousPageText="&nbsp;Previous&nbsp;" />
                        <PagerStyle CssClass="SoarNPpaging" />
                        <Columns>
                            <asp:BoundField ReadOnly="true" DataField="ID" HeaderText="Row ID" ItemStyle-Width="5" />
                            <asp:TemplateField ItemStyle-Width="120" HeaderText="Display Name">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblDName" Text='<%# Eval("Display_Name") %>'/>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:DropDownList ID="DDLMember" runat="server" DataSourceID="SqlDataSrc_Member"
                                        DataTextField="sDisplayName" DataValueField="ID" />
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField ItemStyle-Width="120" HeaderText="Membership Category">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblMbCat" Text='<%# Eval("Membership_Category") %>'/>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:DropDownList ID="DDLMbCat" runat="server" DataSourceID="SqlDataSrc_MCat"
                                        DataTextField="sMembershipCategory" DataValueField="ID" />
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField ItemStyle-Width="140" HeaderText="Date Began">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblDBegin" Text='<%# TSoar.CustFmt.sFmtDate((DateTimeOffset)Eval("Date_Began"),TSoar.CustFmt.enDFmt.DateOnly) %>'/>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox ID="txbDBegin" runat="server" Width="140"
                                        Text='<%# TSoar.CustFmt.sFmtDate((DateTimeOffset)Eval("Date_Began"),TSoar.CustFmt.enDFmt.DateOnly).Replace("/","-") %>'
                                        TextMode="Date" Font-Size="X-Small" />
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField ItemStyle-Width="140" HeaderText="Date Ended">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblDEnd" Text='<%# TSoar.CustFmt.sFmtDate((DateTimeOffset)Eval("Date_Ended"),TSoar.CustFmt.enDFmt.DateOnly) %>'/>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:textBox ID="txbDEnd" runat="server" Width="140" Text='<%# TSoar.CustFmt.sFmtDate((DateTimeOffset)Eval("Date_Ended"),TSoar.CustFmt.enDFmt.DateOnly).Replace("/","-") %>' TextMode="Date" />
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField ItemStyle-Width="200" HeaderText="Notes">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblNotes" Text='<%# Eval("Notes") %>'/>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:textBox ID="txbNotes" runat="server" text='<%# Bind("Notes") %>' Width="200"/>
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

<asp:Content ID="ContentFooter" ContentPlaceHolderID="ContentPlaceHolderFooter" runat="server">
    <%: Title %>
</asp:Content>
