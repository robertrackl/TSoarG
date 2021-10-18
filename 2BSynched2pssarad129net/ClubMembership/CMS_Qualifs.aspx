<%@ Page Title="TSoar CMS_Qualifs" Language="C#" MasterPageFile="~/mTSoar.Master" AutoEventWireup="true"
    CodeBehind="CMS_Qualifs.aspx.cs" Inherits="TSoar.ClubMembership.CMS_Qualifs" %>

<asp:Content ID="ContentHeader" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="server">
    <asp:Label runat="server" Text="List of Members' Qualifications (mostly piloting)" Font-Italic="true" />
</asp:Content>

<asp:Content ID="ContentMain" ContentPlaceHolderID="ContentPlaceHolderMain" runat="server">

    <asp:SqlDataSource ID="SqlDataSrc_Member" runat="server" ConnectionString="<%$ ConnectionStrings:SqlConn %>"
        SelectCommand="SELECT ID,sDisplayName FROM PEOPLE" >
    </asp:SqlDataSource>
    <asp:SqlDataSource ID="SqlDataSrc_Certif" runat="server" ConnectionString="<%$ ConnectionStrings:SqlConn %>"
        SelectCommand="SELECT ID,sCertification FROM CERTIFICATIONS" >
    </asp:SqlDataSource>
    <asp:SqlDataSource ID="SqlDataSrc_Rating" runat="server" ConnectionString="<%$ ConnectionStrings:SqlConn %>"
        SelectCommand="SELECT ID,sRating FROM RATINGS" >
    </asp:SqlDataSource>
    <asp:SqlDataSource ID="SqlDataSrc_Qualif" runat="server" ConnectionString="<%$ ConnectionStrings:SqlConn %>"
        SelectCommand="SELECT ID,sQualification FROM QUALIFICATIONS" >
    </asp:SqlDataSource>

    <p>We distinguish between 'Certifications', 'Ratings', and 'Qualifications'.
       Certifications and Ratings arise out of Federal Aviation Administration (FAA) rules for airmen.
       Qualifications arise out of other requirements, such as aircraft insurance policies, and club bylaws, and operations rules.
    </p>
    <asp:Button ID="pbQualifs" runat="server" Text="Show/Edit Qualifications" OnClick="pbQCR_Click" />
    <asp:Button ID="pbCertifs" runat="server" Text="Show/Edit Certifications" OnClick="pbQCR_Click" />
    <asp:Button ID="pbRatings" runat="server" Text="Show/Edit Ratings" OnClick="pbQCR_Click" />
    <div id="divQualifs">
        <ajaxToolkit:Accordion
            ID="AccordionCMS_Qualifs"
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
                    <Header>Add New Club Member Qualification Details</Header>
                    <Content>
                        <asp:DetailsView runat="server" ID="dvCMS_Qualifs" AutoGenerateRows="false"
                                DataKeyNames="sDisplayName" AllowPaging="false" AutoGenerateInsertButton="true"
                                CellPadding="10" DefaultMode="Insert" HorizontalAlign="Center" CssClass="SoarNPGridStyle"
                                OnItemInserting="dv_ItemInserting" OnModeChanging="dvCMS_Qualifs_ModeChanging">
                            <Fields>
                                <asp:TemplateField HeaderText="Member Display Name" >
                                    <InsertItemTemplate>
                                        <asp:DropDownList ID="DDL_Member" runat="server" DataSourceID="SqlDataSrc_Member"
                                            DataTextField="sDisplayName" DataValueField="ID" />
                                    </InsertItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Qualification">
                                    <InsertItemTemplate>
                                        <asp:DropDownList ID="DDL_Qualif" runat="server" DataSourceID="SqlDataSrc_Qualif"
                                            DataTextField="sQualification" DataValueField="ID" OnPreRender="DDL_Qualif_PreRender" />
                                    </InsertItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Since">
                                    <InsertItemTemplate>
                                        <asp:TextBox ID="txb_QBegan" runat="server" TextMode="Date" ></asp:TextBox>
                                    </InsertItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Expires">
                                    <InsertItemTemplate>
                                        <asp:TextBox ID="txb_QEnd" runat="server" TextMode="Date" ></asp:TextBox>
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
                    <Header> Club Member Qualifications </Header>
                    <Content>
                        <asp:GridView ID="gvCMS_Qualifs" runat="server" AutoGenerateColumns="False" 
                                GridLines="None" CssClass="SoarNPGridStyle"
                                OnRowCreated="gv_RowCreated"
                                OnRowDeleting="gv_RowDeleting"
                                OnRowEditing="gv_RowEditing"
                                OnRowDataBound="gv_RowDataBound"
                                OnRowCancelingEdit="gv_RowCancelingEdit"
                                OnRowUpdating="gv_RowUpdating"
                                AllowPaging="true" PageSize="20" 
                                OnPageIndexChanging="gvCMS_PageIndexChanging">
                            <PagerSettings Mode="NextPreviousFirstLast" Position="TopAndBottom" Visible="true" FirstPageText="&nbsp;First&nbsp;"
                                LastPageText="&nbsp;Last&nbsp;" NextPageText="&nbsp;Next&nbsp;" PreviousPageText="&nbsp;Previous&nbsp;" />
                            <PagerStyle CssClass="SoarNPpaging" />
                            <Columns>
                                <asp:BoundField ReadOnly="true" DataField="ID" HeaderText="Row ID" ItemStyle-Width="5" />
                                <asp:TemplateField ItemStyle-Width="120" HeaderText="&nbsp;Display Name&nbsp;">
                                    <ItemTemplate>
                                    <asp:Label runat="server" ID="lblDName" Text='<%# Eval("Display_Name") %>'/>
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <asp:DropDownList ID="DDLMember" runat="server" DataSourceID="SqlDataSrc_Member"
                                            DataTextField="sDisplayName" DataValueField="ID" />
                                    </EditItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField ItemStyle-Width="120" HeaderText="&nbsp;Qualification&nbsp;">
                                    <ItemTemplate>
                                    <asp:Label runat="server" ID="lblQualif" Text='<%# Eval("Qualification") %>'/>
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <asp:DropDownList ID="DDLQualif" runat="server" DataSourceID="SqlDataSrc_Qualif"
                                            DataTextField="sQualification" DataValueField="ID" />
                                    </EditItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField ItemStyle-Width="135" HeaderText="&nbsp;Since&nbsp;">
                                    <ItemTemplate>
                                    <asp:Label runat="server" ID="lblSince" Text='<%# TSoar.CustFmt.sFmtDate((DateTimeOffset)Eval("DSince"),TSoar.CustFmt.enDFmt.DateOnly) %>'/>
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                    <asp:textBox ID="txbSince" runat="server" Width="135" text='<%# TSoar.CustFmt.sFmtDate((DateTimeOffset)Eval("DSince"),TSoar.CustFmt.enDFmt.DateOnly).Replace("/","-") %>' TextMode="Date"/>
                                    </EditItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField ItemStyle-Width="135" HeaderText="&nbsp;Expires&nbsp;">
                                    <ItemTemplate>
                                    <asp:Label runat="server" ID="lblExpires" Text='<%# TSoar.CustFmt.sFmtDate((DateTimeOffset)Eval("DExpiry"),TSoar.CustFmt.enDFmt.DateOnly) %>'/>
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                    <asp:textBox ID="txbExpires" runat="server" Width="135" text='<%# TSoar.CustFmt.sFmtDate((DateTimeOffset)Eval("DExpiry"),TSoar.CustFmt.enDFmt.DateOnly).Replace("/","-") %>' TextMode="Date"/>
                                    </EditItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField ItemStyle-Width="200" HeaderText="&nbsp;Notes&nbsp;">
                                    <ItemTemplate>
                                    <asp:Label runat="server" ID="lblNotes" Text='<%# Eval("Notes") %>'/>
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                    <asp:textBox ID="txbNotes" runat="server" text='<%# Eval("Notes") %>' TextMode="MultiLine"/>
                                    </EditItemTemplate>
                                </asp:TemplateField>
                                <asp:CommandField ShowEditButton="true" EditText="&nbsp;Modify&nbsp;" HeaderStyle-CssClass="text-center" />
                                <asp:CommandField DeleteText="&nbsp;Delete&nbsp;" ShowDeleteButton="True" HeaderStyle-CssClass="text-center"/>
                            </Columns>
                        </asp:GridView>
                    </Content>
                </ajaxToolkit:AccordionPane>
            </Panes>        
        </ajaxToolkit:Accordion>
    </div>
    <div id="divCertifs">
        <ajaxToolkit:Accordion
            ID="AccordionCertifs"
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
                    <Header>Add New Club Member Certification Details</Header>
                    <Content>
                        <asp:DetailsView runat="server" ID="dvCMS_Certifs" AutoGenerateRows="false"
                                DataKeyNames="sDisplayName" AllowPaging="false" AutoGenerateInsertButton="true"
                                CellPadding="10" DefaultMode="Insert" HorizontalAlign="Center" CssClass="SoarNPGridStyle"
                                OnItemInserting="dv_ItemInserting" OnModeChanging="dvCMS_Certifs_ModeChanging">
                            <Fields>
                                <asp:TemplateField HeaderText="Member Display Name" >
                                    <InsertItemTemplate>
                                        <asp:DropDownList ID="DDL_Member" runat="server" DataSourceID="SqlDataSrc_Member"
                                            DataTextField="sDisplayName" DataValueField="ID" />
                                    </InsertItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Certification">
                                    <InsertItemTemplate>
                                        <asp:DropDownList ID="DDL_Certif" runat="server" DataSourceID="SqlDataSrc_Certif"
                                            DataTextField="sCertification" DataValueField="ID" OnPreRender="DDL_Certif_PreRender" />
                                    </InsertItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Since">
                                    <InsertItemTemplate>
                                        <asp:TextBox ID="txb_QBegan" runat="server" TextMode="Date" ></asp:TextBox>
                                    </InsertItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Expires">
                                    <InsertItemTemplate>
                                        <asp:TextBox ID="txb_QEnd" runat="server" TextMode="Date" ></asp:TextBox>
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
                    <Header> Club Member Certifications </Header>
                    <Content>
                        <asp:GridView ID="gvCMS_Certifs" runat="server" AutoGenerateColumns="False" 
                                GridLines="None" CssClass="SoarNPGridStyle"
                                OnRowCreated="gv_RowCreated"
                                OnRowDeleting="gv_RowDeleting"
                                OnRowEditing="gv_RowEditing"
                                OnRowDataBound="gv_RowDataBound"
                                OnRowCancelingEdit="gv_RowCancelingEdit"
                                OnRowUpdating="gv_RowUpdating"
                                AllowPaging="true" PageSize="20" 
                                OnPageIndexChanging="gvCMS_PageIndexChanging">
                            <PagerSettings Mode="NextPreviousFirstLast" Position="TopAndBottom" Visible="true" FirstPageText="&nbsp;First&nbsp;"
                                LastPageText="&nbsp;Last&nbsp;" NextPageText="&nbsp;Next&nbsp;" PreviousPageText="&nbsp;Previous&nbsp;" />
                            <PagerStyle CssClass="SoarNPpaging" />
                            <Columns>
                                <asp:BoundField ReadOnly="true" DataField="ID" HeaderText="Row ID" ItemStyle-Width="5" />
                                <asp:TemplateField ItemStyle-Width="120" HeaderText="&nbsp;Display Name&nbsp;">
                                    <ItemTemplate>
                                    <asp:Label runat="server" ID="lblDName" Text='<%# Eval("Display_Name") %>'/>
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <asp:DropDownList ID="DDLMember" runat="server" DataSourceID="SqlDataSrc_Member"
                                            DataTextField="sDisplayName" DataValueField="ID" />
                                    </EditItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField ItemStyle-Width="120" HeaderText="&nbsp;Certification&nbsp;">
                                    <ItemTemplate>
                                    <asp:Label runat="server" ID="lblCertif" Text='<%# Eval("sCertification") %>'/>
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <asp:DropDownList ID="DDLCertif" runat="server" DataSourceID="SqlDataSrc_Certif"
                                            DataTextField="sCertification" DataValueField="ID" />
                                    </EditItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField ItemStyle-Width="135" HeaderText="&nbsp;Since&nbsp;">
                                    <ItemTemplate>
                                    <asp:Label runat="server" ID="lblSince" Text='<%# TSoar.CustFmt.sFmtDate((DateTimeOffset)Eval("DSince"),TSoar.CustFmt.enDFmt.DateOnly) %>'/>
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                    <asp:textBox ID="txbSince" runat="server" Width="135" text='<%# TSoar.CustFmt.sFmtDate((DateTimeOffset)Eval("DSince"),TSoar.CustFmt.enDFmt.DateOnly).Replace("/","-") %>' TextMode="Date"/>
                                    </EditItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField ItemStyle-Width="135" HeaderText="&nbsp;Expires&nbsp;">
                                    <ItemTemplate>
                                    <asp:Label runat="server" ID="lblExpires" Text='<%# TSoar.CustFmt.sFmtDate((DateTimeOffset)Eval("DExpiry"),TSoar.CustFmt.enDFmt.DateOnly) %>'/>
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                    <asp:textBox ID="txbExpires" runat="server" Width="135" text='<%# TSoar.CustFmt.sFmtDate((DateTimeOffset)Eval("DExpiry"),TSoar.CustFmt.enDFmt.DateOnly).Replace("/","-") %>' TextMode="Date"/>
                                    </EditItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField ItemStyle-Width="200" HeaderText="&nbsp;Notes&nbsp;">
                                    <ItemTemplate>
                                    <asp:Label runat="server" ID="lblNotes" Text='<%# Eval("sAdditionalInfo") %>'/>
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                    <asp:textBox ID="txbNotes" runat="server" text='<%# Eval("sAdditionalInfo") %>' TextMode="MultiLine"/>
                                    </EditItemTemplate>
                                </asp:TemplateField>
                                <asp:CommandField ShowEditButton="true" EditText="&nbsp;Modify&nbsp;" HeaderStyle-CssClass="text-center" />
                                <asp:CommandField DeleteText="&nbsp;Delete&nbsp;" ShowDeleteButton="True" HeaderStyle-CssClass="text-center"/>
                            </Columns>
                        </asp:GridView>
                    </Content>
                </ajaxToolkit:AccordionPane>
            </Panes>
        </ajaxToolkit:Accordion>
    </div>
    <div id="divRatings">
        <ajaxToolkit:Accordion
            ID="AccordionRatings"
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
                    <Header>Add New Club Member Rating Details</Header>
                    <Content>
                        <asp:DetailsView runat="server" ID="dvCMS_Ratings" AutoGenerateRows="false"
                                DataKeyNames="sDisplayName" AllowPaging="false" AutoGenerateInsertButton="true"
                                CellPadding="10" DefaultMode="Insert" HorizontalAlign="Center" CssClass="SoarNPGridStyle"
                                OnItemInserting="dv_ItemInserting" OnModeChanging="dvCMS_Ratings_ModeChanging">
                            <Fields>
                                <asp:TemplateField HeaderText="Member Display Name" >
                                    <InsertItemTemplate>
                                        <asp:DropDownList ID="DDL_Member" runat="server" DataSourceID="SqlDataSrc_Member"
                                            DataTextField="sDisplayName" DataValueField="ID" />
                                    </InsertItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Rating">
                                    <InsertItemTemplate>
                                        <asp:DropDownList ID="DDL_Rating" runat="server" DataSourceID="SqlDataSrc_Rating"
                                            DataTextField="sRating" DataValueField="ID" OnPreRender="DDL_Rating_PreRender" />
                                    </InsertItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Since">
                                    <InsertItemTemplate>
                                        <asp:TextBox ID="txb_QBegan" runat="server" TextMode="Date" ></asp:TextBox>
                                    </InsertItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Expires">
                                    <InsertItemTemplate>
                                        <asp:TextBox ID="txb_QEnd" runat="server" TextMode="Date" ></asp:TextBox>
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
                    <Header> Club Member Ratings </Header>
                    <Content>
                        <asp:GridView ID="gvCMS_Ratings" runat="server" AutoGenerateColumns="False" 
                                GridLines="None" CssClass="SoarNPGridStyle"
                                OnRowCreated="gv_RowCreated"
                                OnRowDeleting="gv_RowDeleting"
                                OnRowEditing="gv_RowEditing"
                                OnRowDataBound="gv_RowDataBound"
                                OnRowCancelingEdit="gv_RowCancelingEdit"
                                OnRowUpdating="gv_RowUpdating"
                                AllowPaging="true" PageSize="20" 
                                OnPageIndexChanging="gvCMS_PageIndexChanging">
                            <PagerSettings Mode="NextPreviousFirstLast" Position="TopAndBottom" Visible="true" FirstPageText="&nbsp;First&nbsp;"
                                LastPageText="&nbsp;Last&nbsp;" NextPageText="&nbsp;Next&nbsp;" PreviousPageText="&nbsp;Previous&nbsp;" />
                            <PagerStyle CssClass="SoarNPpaging" />
                            <Columns>
                                <asp:BoundField ReadOnly="true" DataField="ID" HeaderText="Row ID" ItemStyle-Width="5" />
                                <asp:TemplateField ItemStyle-Width="120" HeaderText="&nbsp;Display Name&nbsp;">
                                    <ItemTemplate>
                                    <asp:Label runat="server" ID="lblDName" Text='<%# Eval("Display_Name") %>'/>
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <asp:DropDownList ID="DDLMember" runat="server" DataSourceID="SqlDataSrc_Member"
                                            DataTextField="sDisplayName" DataValueField="ID" />
                                    </EditItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField ItemStyle-Width="120" HeaderText="&nbsp;Rating&nbsp;">
                                    <ItemTemplate>
                                    <asp:Label runat="server" ID="lblRating" Text='<%# Eval("sRating") %>'/>
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <asp:DropDownList ID="DDLRating" runat="server" DataSourceID="SqlDataSrc_Rating"
                                            DataTextField="sRating" DataValueField="ID" />
                                    </EditItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField ItemStyle-Width="135" HeaderText="&nbsp;Since&nbsp;">
                                    <ItemTemplate>
                                    <asp:Label runat="server" ID="lblSince" Text='<%# TSoar.CustFmt.sFmtDate((DateTimeOffset)Eval("DSince"),TSoar.CustFmt.enDFmt.DateOnly) %>'/>
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                    <asp:textBox ID="txbSince" runat="server" Width="135" text='<%# TSoar.CustFmt.sFmtDate((DateTimeOffset)Eval("DSince"),TSoar.CustFmt.enDFmt.DateOnly).Replace("/","-") %>' TextMode="Date"/>
                                    </EditItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField ItemStyle-Width="135" HeaderText="&nbsp;Expires&nbsp;">
                                    <ItemTemplate>
                                    <asp:Label runat="server" ID="lblExpires" Text='<%# TSoar.CustFmt.sFmtDate((DateTimeOffset)Eval("DExpiry"),TSoar.CustFmt.enDFmt.DateOnly) %>'/>
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                    <asp:textBox ID="txbExpires" runat="server" Width="135" text='<%# TSoar.CustFmt.sFmtDate((DateTimeOffset)Eval("DExpiry"),TSoar.CustFmt.enDFmt.DateOnly).Replace("/","-") %>' TextMode="Date"/>
                                    </EditItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField ItemStyle-Width="200" HeaderText="&nbsp;Notes&nbsp;">
                                    <ItemTemplate>
                                    <asp:Label runat="server" ID="lblNotes" Text='<%# Eval("sAdditionalInfo") %>'/>
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                    <asp:textBox ID="txbNotes" runat="server" text='<%# Eval("sAdditionalInfo") %>' TextMode="MultiLine"/>
                                    </EditItemTemplate>
                                </asp:TemplateField>
                                <asp:CommandField ShowEditButton="true" EditText="&nbsp;Modify&nbsp;" HeaderStyle-CssClass="text-center" />
                                <asp:CommandField DeleteText="&nbsp;Delete&nbsp;" ShowDeleteButton="True" HeaderStyle-CssClass="text-center"/>
                            </Columns>
                        </asp:GridView>
                    </Content>
                </ajaxToolkit:AccordionPane>        
            </Panes>            
        </ajaxToolkit:Accordion>
    </div>

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
