<%@ Page Title="Equipment List" Language="C#" MasterPageFile="~/mTSoar.Master" AutoEventWireup="true" CodeBehind="EquipmentList.aspx.cs" Inherits="TSoar.Equipment.EquipmentList" %>

<asp:Content ID="HeaderContent" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="server">
    <asp:Label runat="server" Text="Equipment and Maintenance" Font-Italic="true" />
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="ContentPlaceHolderMain" runat="server">
    <style>
        .cell-padding{padding:5px}
    </style>
    <h4>Maintain List of Equipment</h4>
    <asp:SqlDataSource ID="SqlDataSrc_EquipType" runat="server" ConnectionString="<%$ ConnectionStrings:SqlConn %>"
        SelectCommand="SELECT ID,sEquipmentType FROM EQUIPTYPES" >
    </asp:SqlDataSource>
    <ajaxToolkit:Accordion
        ID="AccordionEquip"
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
                <Header>Add New Equipment</Header>
                <Content>
                    <asp:DetailsView runat="server" ID="dvEquip" AutoGenerateRows="false"
                            DataKeyNames="sShortEquipName" AllowPaging="false" AutoGenerateInsertButton="true"
                            CellPadding="10" DefaultMode="Insert" HorizontalAlign="Center" CssClass="SoarNPGridStyle"
                            OnItemInserting="dvEquip_ItemInserting" OnModeChanging="dvEquip_ModeChanging">
                        <Fields>
                            <asp:TemplateField HeaderText="Equipment Type">
                                <InsertItemTemplate>
                                    <asp:DropDownList ID="DDL_EquipType" runat="server" DataSourceID="SqlDataSrc_EquipType"
                                        DataTextField="sEquipmentType" DataValueField="ID" OnPreRender="DDL_EquipType_PreRender" />
                                </InsertItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Name">
                                <InsertItemTemplate>
                                    <asp:TextBox ID="txb_ShortEquipName" runat="server" ></asp:TextBox>
                                </InsertItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Description">
                                <InsertItemTemplate>
                                    <asp:TextBox ID="txb_Description" runat="server" ></asp:TextBox>
                                </InsertItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Registration Id">
                                <InsertItemTemplate>
                                    <asp:TextBox ID="txb_Registration" runat="server" ></asp:TextBox>
                                </InsertItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Owner">
                                <InsertItemTemplate>
                                    <asp:TextBox ID="txb_Owner" runat="server" ></asp:TextBox>
                                </InsertItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Model Number">
                                <InsertItemTemplate>
                                    <asp:TextBox ID="txb_ModelNum" runat="server" ></asp:TextBox>
                                </InsertItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Ownership Begin">
                                <InsertItemTemplate>
                                    <asp:TextBox ID="txb_OwnBegin" runat="server" TextMode="Date" ></asp:TextBox>
                                </InsertItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Ownership End">
                                <InsertItemTemplate>
                                    <asp:TextBox ID="txb_OwnEnd" runat="server" TextMode="Date" ></asp:TextBox>
                                </InsertItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Acquisition Cost">
                                <InsertItemTemplate>
                                    <asp:TextBox ID="txb_AcqCost" runat="server" TextMode="Number" ></asp:TextBox>
                                </InsertItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Comments/Notes">
                                <InsertItemTemplate>
                                    <asp:TextBox ID="txb_Notes" runat="server" TextMode="MultiLine" ></asp:TextBox>
                                </InsertItemTemplate>
                            </asp:TemplateField>
                        </Fields>
                    </asp:DetailsView>
                </Content>
            </ajaxToolkit:AccordionPane>
            <ajaxToolkit:AccordionPane ID="AccPEquip_BasicList" runat="server" >
                <Header> List of Equipment </Header>
                <Content>
                    <asp:GridView ID="gvEquip" runat="server" AutoGenerateColumns="False"
                            GridLines="None" CssClass="SoarNPGridStyle"
                            OnRowEditing="gvEquip_RowEditing" OnRowDataBound="gvEquip_RowDataBound"
                            OnRowDeleting="gvEquip_RowDeleting" OnRowUpdating="gvEquip_RowUpdating"
                            OnRowCreated="gvEquip_RowCreated" OnRowCancelingEdit="gvEquip_RowCancelingEdit"
                            OnPageIndexChanging="gvEquip_PageIndexChanging" AllowPaging="true" PageSize="8">
                        <PagerSettings Mode="NextPreviousFirstLast" Position="TopAndBottom" Visible="true" FirstPageText="&nbsp;First&nbsp;"
                            LastPageText="&nbsp;Last&nbsp;" NextPageText="&nbsp;Next&nbsp;" PreviousPageText="&nbsp;Previous&nbsp;" />
                        <PagerStyle CssClass="SoarNPpaging" />
                        <Columns>
                            <asp:TemplateField HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                                <HeaderTemplate>
                                    <asp:Label ID="lblHIdent" runat="server" Text="Internal Id" Width="60" ToolTip="Points to a row in database table EQUIPMENT with this ID field contents"></asp:Label>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="lblIIdent" runat="server" Text='<%# Eval("ID") %>' Width="60" ></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField ItemStyle-Width="120" HeaderText="&nbsp;Equipment Type&nbsp;">
                                <ItemTemplate>
                                <asp:Label runat="server" ID="lblEquipType" Text='<%# Bind("sEquipmentType") %>'/>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:DropDownList ID="DDLEquipType" runat="server" DataSourceID="SqlDataSrc_EquipType"
                                        DataTextField="sEquipmentType" DataValueField="ID" />
                                </EditItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField ItemStyle-Width="120" HeaderText="&nbsp;Equipment Name&nbsp;">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblEquipName" Text='<%# Eval("sShortEquipName") %>'/>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:textBox ID="txbEquipName" runat="server" text='<%# Eval("sShortEquipName") %>' Width="120"/>
                                </EditItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField ItemStyle-Width="120" HeaderText="&nbsp;Description&nbsp;">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblDescript" Text='<%# Eval("sDescription") %>'/>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:textBox ID="txbDescript" runat="server" text='<%# Eval("sDescription") %>' Width="120"/>
                                </EditItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField ItemStyle-Width="120" HeaderText="&nbsp;Registration Id&nbsp;">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblRegistr" Text='<%# Eval("sRegistrationId") %>'/>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:textBox ID="txbRegistr" runat="server" text='<%# Eval("sRegistrationId") %>' Width="120"/>
                                </EditItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField ItemStyle-Width="200" HeaderText="&nbsp;Owner&nbsp;">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblOwner" Text='<%# Eval("sOwner") %>'/>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:textBox ID="txbOwner" runat="server" text='<%# Eval("sOwner") %>' Width="200"/>
                                </EditItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField ItemStyle-Width="140" HeaderText="&nbsp;Model Number&nbsp;">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblModelNum" Text='<%# Eval("sModelNum") %>'/>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:textBox ID="txbModelNum" runat="server" text='<%# Eval("sModelNum") %>' Width="140"/>
                                </EditItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField ItemStyle-Width="140">
                                <HeaderTemplate>
                                    <asp:Label ID="lblHOwnBegin" runat="server" Text="Ownership Begin" ToolTip="Date when legal ownership began; not necessarily the same as date of begin of operations (see Components of Equipment)." />
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblOwnBegin" Text='<%# TSoar.CustFmt.sFmtDate((DateTimeOffset)Eval("DOwnershipBegin"),TSoar.CustFmt.enDFmt.DateOnly) %>'/>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:textBox ID="txbOwnBegin" runat="server" text='<%# TSoar.CustFmt.sFmtDate((DateTimeOffset)Eval("DOwnershipBegin"),TSoar.CustFmt.enDFmt.DateOnly).Replace("/","-") %>' TextMode="Date"/>
                                </EditItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField ItemStyle-Width="140" HeaderText="&nbsp;Ownership End&nbsp;">
                                <HeaderTemplate>
                                    <asp:Label ID="lblHOwnEnd" runat="server" Text="Ownership End" ToolTip="Date when legal ownership ended. This may be unknown - use a date far in the future." />
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblOwnEnd" Text='<%# TSoar.CustFmt.sFmtDate((DateTimeOffset)Eval("DOwnershipEnd"),TSoar.CustFmt.enDFmt.DateOnly) %>'/>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:textBox ID="txbOwnEnd" runat="server" text='<%# TSoar.CustFmt.sFmtDate((DateTimeOffset)Eval("DOwnershipEnd"),TSoar.CustFmt.enDFmt.DateOnly).Replace("/","-") %>' TextMode="Date"/>
                                </EditItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField ItemStyle-Width="70" HeaderText="&nbspAcquisition Cost&nbsp;" ItemStyle-HorizontalAlign="Right">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblAcqCost" Text='<%# Eval("AcquisitionCost") %>'/>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:textBox ID="txbAcquisCost" runat="server" text='<%# Eval("AcquisitionCost") %>' Width="70"/>
                                </EditItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField ItemStyle-Width="140" HeaderText="&nbsp;Comment/Notes&nbsp;">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblNotes" Text='<%# Eval("sComment") %>'/>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:textBox ID="txbNotes" runat="server" text='<%# Eval("sComment") %>' Width="140"/>
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
