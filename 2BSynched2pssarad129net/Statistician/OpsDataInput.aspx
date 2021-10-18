<%@ Page Title="OpsDataInput" Language="C#" MasterPageFile="~/mTSoar.Master" AutoEventWireup="true"
    CodeBehind="OpsDataInput.aspx.cs" Inherits="TSoar.Statistician.OpsDataInput" %>

<asp:Content ID="ContentHeader" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="server">
    <asp:Label runat="server" Text="The club statistician enters details of each flight; display of flight details" Font-Italic="true" />
</asp:Content>

<asp:Content ID="ContentMain" ContentPlaceHolderID="ContentPlaceHolderMain" runat="server">
    <div title="SqlDataSources">
        <asp:SqlDataSource ID="SqlDataSrc_LaunchMethod" runat="server" ConnectionString="<%$ ConnectionStrings:SqlConn %>"
            SelectCommand="SELECT ID,sLaunchMethod FROM LAUNCHMETHODS" >
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="SqlDataSrc_Location" runat="server" ConnectionString="<%$ ConnectionStrings:SqlConn %>"
            SelectCommand="SELECT ID,sLocation FROM LOCATIONS" >
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="SqlDataSrc_Equip" runat="server" ConnectionString="<%$ ConnectionStrings:SqlConn %>"
            SelectCommand="SELECT ID,sShortEquipName FROM EQUIPMENT" >
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="SqlDataSrc_EquipmentRole" runat="server" ConnectionString="<%$ ConnectionStrings:SqlConn %>"
            SelectCommand="SELECT ID,sEquipmentRole FROM EQUIPMENTROLES" >
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="SqlDataSrc_Av" runat="server" ConnectionString="<%$ ConnectionStrings:SqlConn %>"
            SelectCommand="SELECT ID,sDisplayName FROM PEOPLE" >
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="SqlDataSrc_AvRole" runat="server" ConnectionString="<%$ ConnectionStrings:SqlConn %>"
            SelectCommand="SELECT ID,sAviatorRole FROM AVIATORROLES" >
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="SqlDataSrc_ChargeCode" runat="server" ConnectionString="<%$ ConnectionStrings:SqlConn %>"
            SelectCommand="SELECT ID,sChargeCode FROM CHARGECODES" >
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="SqlDataSrc_SpecialOps" runat="server" ConnectionString="<%$ ConnectionStrings:SqlConn %>"
            SelectCommand="SELECT ID,sSpecialOpType FROM SPECIALOPTYPES" >
        </asp:SqlDataSource>
    </div>
        <asp:Panel runat="server" BorderStyle="Solid" >
            <asp:Table runat="server" HorizontalAlign="Center" GridLines="Both">
                <asp:TableRow BackColor="YellowGreen" >
                    <asp:TableCell HorizontalAlign="Center" ForeColor="DarkGreen" Font-Bold="true">Add New Data:</asp:TableCell>
                    <asp:TableCell HorizontalAlign="Center">&nbsp;&nbsp;<asp:Button runat="server" ID="pbAddOp" Text="Add an operation" Enabled="true" OnClick="pbAddOp_Click" />&nbsp;&nbsp;</asp:TableCell>
                    <asp:TableCell HorizontalAlign="Center">&nbsp;&nbsp;<asp:Button runat="server" ID="pbAddSpOpInfo" Text="Add Special Operations Info to operation" Enabled="false" OnClick="pbAddSpOpInfo_Click" />&nbsp;&nbsp;</asp:TableCell>
                    <asp:TableCell HorizontalAlign="Center">&nbsp;&nbsp;<asp:Button runat="server" ID="pbAddEquip" Text="Add equipment/aircraft to operation" Enabled="false" OnClick="pbAddEquip_Click" />&nbsp;&nbsp;</asp:TableCell>
                    <asp:TableCell HorizontalAlign="Center">&nbsp;&nbsp;<asp:Button runat="server" ID="pbAddAviator" Text="Add operator/aviator to equipment" Enabled="false" OnClick="pbAddAviator_Click"/>&nbsp;&nbsp;</asp:TableCell>
                </asp:TableRow>
                <asp:TableRow BackColor="#ff6699" >
                    <asp:TableCell HorizontalAlign="Center" ForeColor="DarkMagenta" Font-Bold="true">&nbsp;Edit/Delete Existing Data:&nbsp;</asp:TableCell>
                    <asp:TableCell HorizontalAlign="Center">&nbsp;&nbsp;<asp:Button runat="server" ID="pbEditOp" Text="Edit highlighted operation" Enabled="false" OnClick="pbEditOp_Click"/>&nbsp;&nbsp;</asp:TableCell>
                    <asp:TableCell HorizontalAlign="Center">&nbsp;&nbsp;<asp:Button runat="server" ID="pbEditSpOpInfo" Text="Edit highlighted Special Operations Info" Enabled="false" OnClick="pbEditSpOpInfo_Click"/>&nbsp;&nbsp;</asp:TableCell>
                    <asp:TableCell HorizontalAlign="Center">&nbsp;&nbsp;<asp:Button runat="server" ID="pbEditEquip" Text="Edit highlighted equipment/aircraft" Enabled="false" OnClick="pbEditEquip_Click"/>&nbsp;&nbsp;</asp:TableCell>
                    <asp:TableCell HorizontalAlign="Center">&nbsp;&nbsp;<asp:Button runat="server" ID="pbEditAviator" Text="Edit highlighted operator/aviator" Enabled="false" OnClick="pbEditAviator_Click"/>&nbsp;&nbsp;</asp:TableCell>
                </asp:TableRow>
            </asp:Table>
        </asp:Panel>
        <p style="color:red; font-weight:bold; text-align:center;">Hover over red entries to see concern or warning</p>
        <asp:TreeView ID="trv_Ops" runat="server" MaxDataBindDepth="6" PathSeparator="|"
            OnSelectedNodeChanged="trv_Ops_SelectedNodeChanged" OnTreeNodePopulate="trv_Ops_TreeNodePopulate"
            PopulateNodesFromClient="false" ShowLines="true" OnPreRender="trv_Ops_PreRender"
            BorderStyle="Solid" HoverNodeStyle-BackColor="WhiteSmoke" SelectedNodeStyle-BackColor="Orange"
            NodeStyle-BorderStyle="Solid" NodeStyle-BorderWidth="1" NodeStyle-BorderColor="WhiteSmoke"
            NodeStyle-HorizontalPadding="5" NodeStyle-ChildNodesPadding="5" NodeStyle-ForeColor="Black">
            <Nodes>
                <asp:TreeNode PopulateOnDemand="True" Text="Operations List" Value="" Expanded="false"></asp:TreeNode>
            </Nodes>
        </asp:TreeView>

    <div title="ModalPopExt">
        <%-- ModalPopupExtender, popping up MPE_Panel and dynamically populating lblPopupText --%>
        <asp:LinkButton ID="Target" runat="server" Text="T" CssClass="displayNone" />
        <ajaxToolkit:ModalPopupExtender ID="ModalPopExt" runat="server"
            TargetControlID="Target" PopupControlID="MPE_Panel"
            BackgroundCssClass="background" DropShadow="true" />
        <asp:Panel ID="MPE_Panel" runat="server" CssClass="popup" style="display:none;" HorizontalAlign="Center">
            <asp:Label ID="lblPopupText" runat="server" BackColor="#eeb5a2" Font-Bold="true" />
            <br /><br />
            <p> <asp:Button ID="OkButton" runat="server" Text="OK" OnClick="Button_Click" />&nbsp;&nbsp;
                <asp:Button ID="CancelButton" runat="server" Text="Cancel" OnClick="Button_Click" />&nbsp;&nbsp;
                <asp:Button ID="NoButton" runat="server" Text="No" OnClick="Button_Click" />&nbsp;&nbsp;
                <asp:Button ID="YesButton" runat="server" Text="Yes" OnClick="Button_Click"/></p>
        </asp:Panel>
    </div>

    <div title="ModalPopExt4Op">
        <%-- ModalPopupExtender, popping up MPE_PnlOp and dynamically populating dv_Op --%>
        <asp:LinkButton ID="lbTarget" runat="server" Text="T" CssClass="displayNone" />
        <ajaxToolkit:ModalPopupExtender ID="ModalPopExt4Op" runat="server"
            TargetControlID="lbTarget" PopupControlID="MPE_PnlOp"
            BackgroundCssClass="background" DropShadow="true" />
        <asp:Panel ID="MPE_PnlOp" runat="server" CssClass="popup" style="display:none;" HorizontalAlign="Center" Width="450">
            <h4><asp:Label ID="lblOpTitle" runat="server" Text="Add/Edit Operation" /></h4>
            <asp:DetailsView ID="dv_Op" runat="server" BackColor="#eeb5a2" Font-Bold="true" AutoGenerateRows="false"
                            AllowPaging="false"
                            CellPadding="10" DefaultMode="Insert" HorizontalAlign="Center" CssClass="SoarNPGridStyle">
                <Fields>
                    <asp:TemplateField HeaderText="Launch Method">
                        <InsertItemTemplate>
                            <asp:DropDownList ID="DDL_LaunchMethod" runat="server" DataSourceID="SqlDataSrc_LaunchMethod"
                                DataTextField="sLaunchMethod" DataValueField="ID" OnPreRender="DDL_LaunchMethod_PreRender" />
                        </InsertItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Takeoff Location" >
                        <InsertItemTemplate>
                            <asp:DropDownList ID="DDL_TLoc" runat="server" DataSourceID="SqlDataSrc_Location"
                                DataTextField="sLocation" DataValueField="ID" OnPreRender="DDL_TLoc_PreRender" />
                        </InsertItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Takeoff Date" >
                        <InsertItemTemplate>
                            <asp:TextBox ID="txbTDate" runat="server" OnPreRender="txbTDate_PreRender" TextMode="Date" />
                        </InsertItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Takeoff Hour (24 Hr Military Time)" >
                        <InsertItemTemplate>
                            <ajaxToolkit:SliderExtender ID="sliderTakeoffHour" TargetControlID="txbTTHour" BoundControlID="txbTHour"
                                EnableHandleAnimation="true" runat="server" Minimum="0" Maximum="23" EnableKeyboard="true" Length="220" />
                            <asp:TextBox ID="txbTTHour" runat="server" OnPreRender="txbTTHour_PreRender"/>
                            Hour of Takeoff: &nbsp;&nbsp;
                            <asp:TextBox ID="txbTHour" runat="server" Width="30" CssClass="text-center"/>
                        </InsertItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Takeoff Minute" >
                        <InsertItemTemplate>
                            <ajaxToolkit:SliderExtender ID="sliderTakeoffMinute" TargetControlID="txbTTMinute" BoundControlID="txbTMinute"
                                EnableHandleAnimation="true" runat="server" Minimum="0" Maximum="59" EnableKeyboard="true" Length="220" />
                            <asp:TextBox ID="txbTTMinute" runat="server" OnPreRender="txbTTMinute_PreRender"/>
                            Minute of Takeoff: &nbsp;&nbsp;
                            <asp:TextBox ID="txbTMinute" runat="server" Width="30" CssClass="text-center"/>
                        </InsertItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Landing Location" >
                        <InsertItemTemplate>
                            <asp:DropDownList ID="DDL_LLoc" runat="server" DataSourceID="SqlDataSrc_Location"
                                DataTextField="sLocation" DataValueField="ID" OnPreRender="DDL_LLoc_PreRender"/>
                        </InsertItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Landing Date" >
                        <InsertItemTemplate>
                            <asp:TextBox ID="txbLDate" runat="server" OnPreRender="txbLDate_PreRender" TextMode="Date" />
                        </InsertItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Landing Hour (24 Hr Military Time)" >
                        <InsertItemTemplate>
                            <ajaxToolkit:SliderExtender ID="sliderLandingHour" TargetControlID="txbTLHour" BoundControlID="txbLHour"
                                EnableHandleAnimation="true" runat="server" Minimum="0" Maximum="23" EnableKeyboard="true" Length="220" />
                            <asp:TextBox ID="txbTLHour" runat="server" OnPreRender="txbTLHour_PreRender"/>
                            Hour of Landing: &nbsp;&nbsp;
                            <asp:TextBox ID="txbLHour" runat="server" Width="30" CssClass="text-center"/>
                        </InsertItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Landing Minute" >
                        <InsertItemTemplate>
                            <ajaxToolkit:SliderExtender ID="sliderLandingMinute" TargetControlID="txbTLMinute" BoundControlID="txbLMinute"
                                EnableHandleAnimation="true" runat="server" Minimum="0" Maximum="59" EnableKeyboard="true" Length="220" />
                            <asp:TextBox ID="txbTLMinute" runat="server" OnPreRender="txbTLMinute_PreRender"/>
                            Minute of Landing: &nbsp;&nbsp;
                            <asp:TextBox ID="txbLMinute" runat="server" Width="30" CssClass="text-center"/>
                        </InsertItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Charge Code">
                        <InsertItemTemplate>
                            <asp:DropDownList ID="DDL_ChargeCode" runat="server" DataSourceID="SqlDataSrc_ChargeCode"
                                DataTextField="sChargeCode" DataValueField="ID" OnPreRender="DDL_ChargeCode_PreRender" />
                        </InsertItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Comments/Notes" >
                        <InsertItemTemplate>
                            <asp:TextBox ID="txb_Notes" runat="server" OnPreRender="txb_Notes_PreRender"/>
                        </InsertItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Invoices To Go">
                        <InsertItemTemplate>
                            <asp:TextBox ID="txbInvoices2go" runat="server" OnPreRender="txbInvoices2go_PreRender" TextMode="Number" />
                        </InsertItemTemplate>
                    </asp:TemplateField>
                </Fields>
            </asp:DetailsView>
            <br />
            <p> <asp:Button ID="pbOpOK" runat="server" Text="OK" OnClick="pbOpOK_Click" />&nbsp;&nbsp;
                <asp:Button ID="pbOpDelete" runat="server" Text="Delete" OnClick="pbOpDelete_Click" />&nbsp;&nbsp;
                <asp:Button ID="pbOpCancel" runat="server" Text="Cancel" OnClick="pbOpCancel_Click" />
            </p>
        </asp:Panel>
    </div>

    <div title="ModalPopupExt4NewSpOp">
        <%-- ModalPopupExtender, popping up MPE_PnlNewSpOp and dynamically populating dv_NewSpOp where SpOp means Special Operations Information --%>
        <asp:LinkButton ID="lblTSpOp" runat="server" Text="T" CssClass="displayNone" />
        <ajaxToolkit:ModalPopupExtender ID="ModalPopupExt4NewSpOp" runat="server"
            TargetControlID="lblTSpOp" PopupControlID="MPE_PnlNewSpOp"
            BackgroundCssClass="background" DropShadow="true" />
        <asp:Panel ID="MPE_PnlNewSpOp" runat="server" CssClass="popup" style="display:none;" HorizontalAlign="Center" Width="450">
            <h4><asp:Label ID="lblNewSpOpTitle" Text="Add Special Operations Info to Operation " runat="server" /></h4>
            <asp:Label ID="lblNewSpOpop" runat="server" Text="TBD" />
            <asp:DetailsView ID="dv_NewSpOp" runat="server" BackColor="#eeb5a2" Font-Bold="true" AutoGenerateRows="false"
                            AllowPaging="false"
                            CellPadding="10" DefaultMode="Insert" HorizontalAlign="Center" CssClass="SoarNPGridStyle">
                <Fields>

                    <asp:TemplateField HeaderText="Special Operations Type">
                        <InsertItemTemplate>
                            <asp:DropDownList ID="DDL_SpOpType" runat="server" DataSourceID="SqlDataSrc_SpecialOps"
                                DataTextField="sSpecialOpType" DataValueField="ID" OnPreRender="DDL_SpOpType_PreRender" />
                        </InsertItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Description" >
                        <InsertItemTemplate>
                            <asp:TextBox ID="txbDescr" runat="server" Width="200" CssClass="text-center"
                                 OnPreRender="txbDescr_PreRender"/>
                        </InsertItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Duration in Minutes" >
                        <InsertItemTemplate>
                            <asp:TextBox ID="txbDuration" runat="server" Width="80" CssClass="text-center"
                                TextMode="Number" MaxLength="6" OnPreRender="txbDuration_PreRender"/>
                        </InsertItemTemplate>
                    </asp:TemplateField>

                </Fields>
            </asp:DetailsView>
            <br />
            <p> <asp:Button ID="pbSpOpOK" runat="server" Text="OK" OnClick="pbSpOpOK_Click" />&nbsp;&nbsp;
                <asp:Button ID="pbSpOpDelete" runat="server" Text="Delete" OnClick="pbSpOpDelete_Click" />&nbsp;&nbsp;
                <asp:Button ID="pbSpOpCancel" runat="server" Text="Cancel" OnClick="pbSpOpCancel_Click" />
            </p>
        </asp:Panel>
    </div>

    <div title="ModalPopupExt4NewOD">
        <%-- ModalPopupExtender, popping up MPE_PnlNewOD and dynamically populating dv_NewOD where OD means Operationes Details --%>
        <asp:LinkButton ID="lblTOD" runat="server" Text="T" CssClass="displayNone" />
        <ajaxToolkit:ModalPopupExtender ID="ModalPopupExt4NewOD" runat="server"
            TargetControlID="lblTOD" PopupControlID="MPE_PnlNewOD"
            BackgroundCssClass="background" DropShadow="true" />
        <asp:Panel ID="MPE_PnlNewOD" runat="server" CssClass="popup" style="display:none;" HorizontalAlign="Center" Width="450">
            <h4><asp:Label ID="lblNewODTitle" Text="Add Equipment/Aircraft to Operation " runat="server" /></h4>
            <asp:Label ID="lblNewODop" runat="server" Text="TBD" />
            <asp:DetailsView ID="dv_NewOD" runat="server" BackColor="#eeb5a2" Font-Bold="true" AutoGenerateRows="false"
                            AllowPaging="false"
                            CellPadding="10" DefaultMode="Insert" HorizontalAlign="Center" CssClass="SoarNPGridStyle">
                <Fields>

                    <asp:TemplateField HeaderText="Equipment Role">
                        <InsertItemTemplate>
                            <asp:DropDownList ID="DDL_EquipmentRole" runat="server" DataSourceID="SqlDataSrc_EquipmentRole"
                                DataTextField="sEquipmentRole" DataValueField="ID" OnPreRender="DDL_EquipmentRole_PreRender" />
                        </InsertItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Equipment/Aircraft">
                        <InsertItemTemplate>
                            <asp:DropDownList ID="DDL_OpDetail" runat="server" DataSourceID="SqlDataSrc_Equip"
                                DataTextField="sShortEquipName" DataValueField="ID" OnPreRender="DDL_OpDetail_PreRender" />
                        </InsertItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Release Altitude, ft MSL" >
                        <InsertItemTemplate>
                            <asp:TextBox ID="txbRelAlt" runat="server" Width="80" CssClass="text-center"
                                TextMode="Number" MaxLength="6" OnPreRender="txbRelAlt_PreRender"/>
                        </InsertItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Maximum Altitude, ft MSL" >
                        <InsertItemTemplate>
                            <asp:TextBox ID="txbMaxAlt" runat="server" Width="80" CssClass="text-center"
                                TextMode="Number" MaxLength="6" OnPreRender="txbMaxAlt_PreRender"/>
                        </InsertItemTemplate>
                    </asp:TemplateField>

                </Fields>
            </asp:DetailsView>
            <br />
            <p> <asp:Button ID="pbODOK" runat="server" Text="OK" OnClick="pbODOK_Click" />&nbsp;&nbsp;
                <asp:Button ID="pbODDelete" runat="server" Text="Delete" OnClick="pbODDelete_Click" />&nbsp;&nbsp;
                <asp:Button ID="pbODCancel" runat="server" Text="Cancel" OnClick="pbODCancel_Click" />
            </p>
        </asp:Panel>
    </div>

    <div title="ModalPopupExt4NewAv">
        <%-- ModalPopupExtender, popping up MPE_PnlNewAv and dynamically populating dv_NewAv where Av means Aviator --%>
        <asp:LinkButton ID="lblTAv" runat="server" Text="T" CssClass="displayNone" />
        <ajaxToolkit:ModalPopupExtender ID="ModalPopupExt4NewAv" runat="server"
            TargetControlID="lblTAv" PopupControlID="MPE_PnlNewAv"
            BackgroundCssClass="background" DropShadow="true" />
        <asp:Panel ID="MPE_pnlNewAv" runat="server" CssClass="popup" style="display:none;" HorizontalAlign="Center" Width="450">
            <h4><asp:Label ID="lblAvTitle" Text="Default Popup Title" runat="server"/></h4>
            <asp:Label ID="lblAvSubtitle" runat="server" Text="Default Popup Subtitle"/>
            <asp:DetailsView ID="dv_NewAv" runat="server" BackColor="#eeb5a2" Font-Bold="true" AutoGenerateRows="false"
                            AllowPaging="false"
                            CellPadding="10" DefaultMode="Insert" HorizontalAlign="Center" CssClass="SoarNPGridStyle">
                <Fields>
                    <asp:TemplateField HeaderText="Aviator/Pilot/Passenger">
                        <InsertItemTemplate>
                            <asp:DropDownList ID="DDL_Av" runat="server" DataSourceID="SqlDataSrc_Av"
                                DataTextField="sDisplayName" DataValueField="ID" OnPreRender="DDL_Av_PreRender" />
                        </InsertItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Aviator Role">
                        <InsertItemTemplate>
                            <asp:DropDownList ID="DDL_AvRole" runat="server" DataSourceID="SqlDataSrc_AvRole"
                                DataTextField="sAviatorRole" DataValueField="ID" OnPreRender="DDL_AvRole_PreRender" />
                        </InsertItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Percent Charge" >
                        <InsertItemTemplate>
                            <asp:TextBox ID="txbPercChrg" runat="server" Width="80" CssClass="text-center" EnableViewState="true"
                                OnPreRender="txbPercChrg_PreRender" />
                            <asp:RegularExpressionValidator runat="server" ControlToValidate="txbPercChrg" ValidationExpression="^[-+]?\d*\.?\d*$"
                                ErrorMessage="Must be a decimal number (optional sign, optional decimal point)" Display="Dynamic" ForeColor="Red" BorderStyle="Dotted" BorderColor="Red" BorderWidth="4px" />
                        </InsertItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="First Flight of Season with Instructor?" >
                        <InsertItemTemplate>
                            <asp:CheckBox ID="chb1stFlt" runat="server" EnableViewState="true" OnPreRender="chb1stFlt_PreRender" />
                        </InsertItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Amount Invoiced" >
                        <InsertItemTemplate>
                            <asp:TextBox ID="txbmInvoiced" runat="server" Width="80" CssClass="text-center" EnableViewState="true"
                                OnPreRender="txbmInvoiced_PreRender" />
                            <asp:RegularExpressionValidator runat="server" ControlToValidate="txbmInvoiced" ValidationExpression="^[-+]?\d*\.?\d*$"
                                ErrorMessage="Must be a decimal number (optional sign, optional decimal point)" Display="Dynamic" ForeColor="Red" BorderStyle="Dotted" BorderColor="Red" BorderWidth="4px" />
                        </InsertItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Date Invoiced" >
                        <InsertItemTemplate>
                            <asp:TextBox ID="txbDInvoiced" runat="server" Width="135" CssClass="text-center" EnableViewState="true" TextMode="Date"
                                OnPreRender="txbDInvoiced_PreRender" />
                        </InsertItemTemplate>
                    </asp:TemplateField>
                </Fields>
            </asp:DetailsView>
            <br />
            <p> <asp:Button ID="pbAvOK" runat="server" Text="OK" OnClick="pbAvOK_Click" />&nbsp;&nbsp;
                <asp:Button ID="pbAvDelete" runat="server" Text="Delete" OnClick="pbAvDelete_Click" />&nbsp;&nbsp;
                <asp:Button ID="pbAvCancel" runat="server" Text="Cancel" OnClick="pbAvCancel_Click" />
            </p>
        </asp:Panel>
    </div>

</asp:Content>

<asp:Content ID="ContentFooter" ContentPlaceHolderID="ContentPlaceHolderFooter" runat="server">
    <%: Title %>
</asp:Content>
