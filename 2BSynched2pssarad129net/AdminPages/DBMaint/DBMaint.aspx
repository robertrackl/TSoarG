<%@ Page Title="TSoar Database Maintenance" Language="C#" MasterPageFile="~/mTSoar.Master"
    AutoEventWireup="true" CodeBehind="DBMaint.aspx.cs" Inherits="TSoar.AdminPages.DBMaint.DBMaint" Debug="true" %>

<asp:Content ID="HeaderContent" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="server">
    <asp:Label runat="server" Text="Database Maintenance Operations" Font-Italic="true" />
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="ContentPlaceHolderMain" runat="server">
    <script>
        $(document).ready(function () {
            $(document).keypress(function (event) {
                if (event.which == 13) {
                    event.preventDefault();
                }
            });
        });
    </script>

    <h4>Editing of Lists that Change Infrequently:</h4>

    <ajaxToolkit:Accordion
        ID="AccordionDBMaint"
        runat="Server"
        SelectedIndex="0"
        HeaderCssClass="accordionHeader"
        HeaderSelectedCssClass="accordionHeaderSelected"
        ContentCssClass="accordionContent"
        AutoSize="None"
        FadeTransitions="true"
        TransitionDuration="250"
        FramesPerSecond="40"
        RequireOpenedPane="false">
        <Panes>
            <ajaxToolkit:AccordionPane ID="AccPLoc" runat="server" >
                <Header> Locations </Header>
                <Content>
                    <div class="text-center">
                        <b>Create a New Location: </b><br />
                        <asp:Table runat="server" BorderStyle="None" HorizontalAlign="Center">
                            <asp:TableRow>
                                <asp:TableCell>Name:</asp:TableCell>
                                <asp:TableCell><asp:TextBox ID="txbNewLoc" runat="server" AutoCompleteType="Disabled" ></asp:TextBox></asp:TableCell>
                            </asp:TableRow>
                            <asp:TableRow>
                                <asp:TableCell>Airfield Abbreviation:</asp:TableCell>
                                <asp:TableCell><asp:TextBox ID="txbNewAbbrev" runat="server" AutoCompleteType="Disabled"></asp:TextBox></asp:TableCell>
                            </asp:TableRow>
                            <asp:TableRow>
                                <asp:TableCell>Runway Altitude in feet MSL:</asp:TableCell>
                                <asp:TableCell><asp:TextBox ID="txbNewAlt" runat="server" TextMode="Number" AutoCompleteType="Disabled" /></asp:TableCell>
                            </asp:TableRow>
                        </asp:Table>
                        <br />
                        <asp:Button ID="pbCreateLoc" runat="server" Text="Create Location" OnClick="pbCreateLoc_Click" />
                        <br /><br />
                        Table of Currently Defined Locations:
                        <br />
                        <asp:GridView ID="gvLoc" runat="server" AutoGenerateColumns="False"
                            GridLines="None" CssClass="SoarNPGridStyle"
                            OnRowDeleting="gvLoc_RowDeleting" OnRowUpdating="gvLoc_RowUpdating" OnRowEditing="gvLoc_RowEditing"
                            OnRowCancelingEdit="gvLoc_RowCancelingEdit" HorizontalAlign="Center">
                             <Columns>
                                 <asp:TemplateField HeaderText="&nbsp;- - - - Location - - - -&nbsp;">    
                                     <ItemTemplate>    
                                        <asp:Label runat="server" ID="lblLoc" Text=' <%# Bind("sLocation") %> ' />    
                                     </ItemTemplate>
                                     <EditItemTemplate>
                                        <asp:textBox ID="txbLoc" runat="server" text='<%# Bind("sLocation") %>' AutoCompleteType="Disabled" />
                                     </EditItemTemplate>
                                 </asp:TemplateField>    
                                 <asp:TemplateField HeaderText="&nbsp;Airfield Abbreviation&nbsp;">    
                                     <ItemTemplate>    
                                        <asp:Label runat="server" ID="lblAbbrev" Text=' <%# Bind("sAbbrev") %> ' />    
                                     </ItemTemplate>
                                     <EditItemTemplate>
                                        <asp:textBox ID="txbAbbrev" runat="server" text='<%# Bind("sAbbrev") %>' AutoCompleteType="Disabled" />
                                     </EditItemTemplate>
                                 </asp:TemplateField>    
                                 <asp:TemplateField HeaderText="&nbsp;Runway Altitude, ft&nbsp;">    
                                     <ItemTemplate>    
                                        <asp:Label runat="server" ID="lblAlt" Text=' <%# Bind("dRunwayAltitude") %> ' />    
                                     </ItemTemplate>
                                     <EditItemTemplate>
                                        <asp:textBox ID="txbAlt" runat="server" text='<%# Bind("dRunwayAltitude") %>' TextMode="Number" AutoCompleteType="Disabled" />
                                     </EditItemTemplate>
                                 </asp:TemplateField>    
                                 <asp:CommandField ShowEditButton="true" EditText="&nbsp;Modify&nbsp;" HeaderStyle-CssClass="text-center" />
                                 <asp:CommandField DeleteText="&nbsp;Delete&nbsp;" ShowDeleteButton="True" HeaderStyle-CssClass="text-center"/>
                             </Columns>    
                        </asp:GridView>
                    </div>
                </Content>
            </ajaxToolkit:AccordionPane>  
            
            <ajaxToolkit:AccordionPane ID="AccPQualif" runat="server" >
                <Header> Qualifications </Header>
                <Content>
                    <div class="text-center">
                        <b>Create a New Qualification: </b>
                        <p>`Qualifications` normally arise from requirements in association bylaws and operating rules, and in insurance policies, as opposed to 
                            ratings and certifications which are derived from Federal Aviation Administration rules.
                        </p>
                            <asp:TextBox ID="txbNewQualif" runat="server" AutoCompleteType="Disabled" ></asp:TextBox>
                            <br />
                            <asp:Button ID="pbCreateQualif" runat="server" Text="Create Qualification" OnClick="pbCreateQualif_Click" />
                        <br /><br />
                        Table of Currently Defined Qualifications:
                        <br />
                        <asp:GridView ID="gvQualif" runat="server" AutoGenerateColumns="False"
                            GridLines="None" CssClass="SoarNPGridStyle"
                            OnRowDeleting="gvQualif_RowDeleting" OnRowUpdating="gvQualif_RowUpdating" OnRowEditing="gvQualif_RowEditing"
                            OnRowCancelingEdit="gvQualif_RowCancelingEdit" HorizontalAlign="Center">
                             <Columns>
                                 <asp:TemplateField HeaderText="&nbsp;- - - - Qualification - - - -&nbsp;">    
                                     <ItemTemplate>    
                                        <asp:Label runat="server" ID="lblQualif" Text=' <%# Bind("sQualification") %> ' />    
                                     </ItemTemplate>
                                     <EditItemTemplate>
                                        <asp:textBox ID="txbQualif" runat="server" text='<%# Bind("sQualification") %>' AutoCompleteType="Disabled" />
                                     </EditItemTemplate>
                                 </asp:TemplateField>    
                                 <asp:CommandField ShowEditButton="true" EditText="&nbsp;Modify&nbsp;" HeaderStyle-CssClass="text-center" />
                                 <asp:CommandField DeleteText="&nbsp;Delete&nbsp;" ShowDeleteButton="True" HeaderStyle-CssClass="text-center"/>
                             </Columns>    
                        </asp:GridView>
                        <p>This website's software requires that a qualification called 'None' exists!</p>
                    </div>
                </Content>
            </ajaxToolkit:AccordionPane> 
            
            <ajaxToolkit:AccordionPane ID="AccPRating" runat="server" >
                <Header> Ratings </Header>
                <Content>
                    <div class="text-center">
                        <b>Create a New Rating: </b>
                        <p>`Ratings` and `Certifications` arise from requirements in Federal Aviation Administration rules, as opposed to qualifications which derive from other sources.</p>
                            <asp:TextBox ID="txbNewRating" runat="server" AutoCompleteType="Disabled"></asp:TextBox>
                            <br />
                            <asp:Button ID="pbCreateRating" runat="server" Text="Create Rating" OnClick="pbCreateRating_Click" />
                        <br /><br />
                        Table of Currently Defined Ratings:
                        <br />
                        <asp:GridView ID="gvRating" runat="server" AutoGenerateColumns="False"
                            GridLines="None" CssClass="SoarNPGridStyle"
                            OnRowDeleting="gvRating_RowDeleting" OnRowUpdating="gvRating_RowUpdating" OnRowEditing="gvRating_RowEditing"
                            OnRowCancelingEdit="gvRating_RowCancelingEdit" HorizontalAlign="Center">
                             <Columns>
                                 <asp:TemplateField HeaderText="&nbsp;- - - - - - - - Rating - - - - - - - -&nbsp;">    
                                     <ItemTemplate>    
                                        <asp:Label runat="server" ID="lblRating" Text=' <%# Bind("sRating") %> ' />    
                                     </ItemTemplate>
                                     <EditItemTemplate>
                                        <asp:textBox ID="txbRating" runat="server" text='<%# Bind("sRating") %>' AutoCompleteType="Disabled" />
                                     </EditItemTemplate>
                                 </asp:TemplateField>    
                                 <asp:CommandField ShowEditButton="true" EditText="&nbsp;Modify&nbsp;" HeaderStyle-CssClass="text-center" />
                                 <asp:CommandField DeleteText="&nbsp;Delete&nbsp;" ShowDeleteButton="True" HeaderStyle-CssClass="text-center"/>
                             </Columns>    
                        </asp:GridView>
                        <p>This website's software requires that a rating called 'None' exists!</p>
                    </div>
                </Content>
            </ajaxToolkit:AccordionPane>  
            
            <ajaxToolkit:AccordionPane runat="server" >
                <Header> Certifications </Header>
                <Content>
                    <div class="text-center">
                        <b>Create a New Certification: </b>
                        <p>`Ratings` and `Certifications` arise from requirements in Federal Aviation Administration rules, as opposed to qualifications which derive from other sources.</p>
                            <asp:TextBox ID="txbNewCert" runat="server" AutoCompleteType="Disabled" ></asp:TextBox>
                            <br />
                            <asp:Button ID="pbCreateCertif" runat="server" Text="Create Certification" OnClick="pbCreateCertif_Click" />
                        <br /><br />
                        Table of Currently Defined Certifications:
                        <br />
                        <asp:GridView ID="gvCertif" runat="server" AutoGenerateColumns="False"
                            GridLines="None" CssClass="SoarNPGridStyle"
                            OnRowDeleting="gvCertif_RowDeleting" OnRowUpdating="gvCertif_RowUpdating" OnRowEditing="gvCertif_RowEditing"
                            OnRowCancelingEdit="gvCertif_RowCancelingEdit" HorizontalAlign="Center">
                             <Columns>
                                 <asp:TemplateField HeaderText="&nbsp;- - - - Certification - - - -&nbsp;">    
                                     <ItemTemplate>    
                                        <asp:Label runat="server" ID="lblCertif" Text=' <%# Bind("sCertification") %> ' />    
                                     </ItemTemplate>
                                     <EditItemTemplate>
                                        <asp:textBox ID="txbCertif" runat="server" text='<%# Bind("sCertification") %>' AutoCompleteType="Disabled" />
                                     </EditItemTemplate>
                                 </asp:TemplateField>    
                                 <asp:CommandField ShowEditButton="true" EditText="&nbsp;Modify&nbsp;" HeaderStyle-CssClass="text-center" />
                                 <asp:CommandField DeleteText="&nbsp;Delete&nbsp;" ShowDeleteButton="True" HeaderStyle-CssClass="text-center"/>
                             </Columns>    
                        </asp:GridView>
                        <p>This website's software requires that a certification called 'None' exists!</p>
                    </div>
                </Content>
            </ajaxToolkit:AccordionPane> 
            
            <ajaxToolkit:AccordionPane ID="AccPMCat" runat="server">
                <Header> Club Membership Categories </Header>
                <Content>
                    <div class="text-center">
                        <b>Create a New Club Membership Category: </b>
                            <asp:TextBox ID="txbNewMembCat" runat="server" AutoCompleteType="Disabled"></asp:TextBox>
                            <br />
                            <asp:Button ID="pbCreateMembCat" runat="server" Text="Create Membership Category" OnClick="pbCreateMembCat_Click" />
                        <br /><br />
                        Table of Currently Defined Club Membership Categories:
                        <br />
                        <asp:GridView ID="gvMembCat" runat="server" AutoGenerateColumns="False"
                            GridLines="None" CssClass="SoarNPGridStyle"
                             OnRowDeleting="gvMembCat_RowDeleting" OnRowUpdating="gvMembCat_RowUpdating" OnRowEditing="gvMembCat_RowEditing"
                             OnRowCancelingEdit="gvMembCat_RowCancelingEdit" HorizontalAlign="Center">
                             <Columns>    
                                 <asp:TemplateField HeaderText="&nbsp;- - - - Membership Category - - - -&nbsp;">    
                                     <ItemTemplate>    
                                        <asp:Label runat="server" ID="lblMembCat" Text='<%# Bind("sMembershipCategory") %>'/>    
                                     </ItemTemplate>
                                     <EditItemTemplate>
                                        <asp:textBox ID="txbMembCat" runat="server" text='<%# Bind("sMembershipCategory") %>' AutoCompleteType="Disabled" />
                                     </EditItemTemplate>
                                 </asp:TemplateField>
                                 <asp:CommandField ShowEditButton="true" EditText="&nbsp;Modify&nbsp;" HeaderStyle-CssClass="text-center" />
                                 <asp:CommandField DeleteText="&nbsp;Delete&nbsp;" ShowDeleteButton="True" HeaderStyle-CssClass="text-center"/>
                             </Columns>    
                        </asp:GridView>
                    </div>
                </Content>
            </ajaxToolkit:AccordionPane> 
            
            <ajaxToolkit:AccordionPane ID="AccP_SSA_MCat" runat="server">
                <Header> SSA (Soaring Society of America) Member Categories </Header>
                <Content>
                    <div class="text-center">
                        <b>Create a New SSA Member Category: </b>
                            <asp:TextBox ID="txbNewSSAMC" runat="server" AutoCompleteType="Disabled"></asp:TextBox>
                            <br />
                            <asp:Button ID="pbCreateSSAMC" runat="server" Text="Create SSA Member Category" OnClick="pbCreateSSAMC_Click" />
                        <br /><br />
                        Table of Currently Defined SSA Member Categories:
                        <br />
                        <asp:GridView ID="gvSSA_MC" runat="server" AutoGenerateColumns="False"
                            GridLines="None" CssClass="SoarNPGridStyle"
                             OnRowDeleting="gvSSA_MC_RowDeleting" OnRowUpdating="gvSSA_MC_RowUpdating" OnRowEditing="gvSSA_MC_RowEditing"
                             OnRowCancelingEdit="gvSSA_MC_RowCancelingEdit" HorizontalAlign="Center">
                             <Columns>    
                                 <asp:TemplateField HeaderText="&nbsp;- - - - SSA Member Category - - - -&nbsp;">    
                                     <ItemTemplate>    
                                        <asp:Label runat="server" ID="lblSSAMC" Text='<%# Bind("sSSA_MemberCategory") %>'/>    
                                     </ItemTemplate>
                                     <EditItemTemplate>
                                        <asp:textBox ID="txbSSAMC" runat="server" text='<%# Bind("sSSA_MemberCategory") %>' AutoCompleteType="Disabled" />
                                     </EditItemTemplate>
                                 </asp:TemplateField>
                                 <asp:CommandField ShowEditButton="true" EditText="&nbsp;Modify&nbsp;" HeaderStyle-CssClass="text-center" />
                                 <asp:CommandField DeleteText="&nbsp;Delete&nbsp;" ShowDeleteButton="True" HeaderStyle-CssClass="text-center"/>
                             </Columns>    
                        </asp:GridView>
                    </div>
                </Content>
            </ajaxToolkit:AccordionPane>    
            
            <ajaxToolkit:AccordionPane ID="AccP_AvRoles" runat="server">
                <Header> Aviator Roles </Header>
                <Content>
                    <div class="text-center">
                        <b>Create a New Aviator Role: </b>
                            <asp:TextBox ID="txbNewAvRole" runat="server" AutoCompleteType="Disabled"></asp:TextBox>
                            <br />
                            <asp:Button ID="pbCreateAvRole" runat="server" Text="Create Aviator Role" OnClick="pbCreateAvRole_Click" />
                        <br /><br />
                        <asp:TextBox runat="server" Enabled="false" Width="350px" Height="110px" TextMode="MultiLine"
                            Text="NOTE: Aviator Roles that have to do with towing or launching must have the string `tow` or `Tow` in the role's name. Aviator Roles that do NOT have to do with towing or launching must NOT have the string `tow` or `Tow` in the role's name." />
                        <br />Table of Currently Defined Aviator Roles:
                        <br />
                        <asp:GridView ID="gvAvRole" runat="server" AutoGenerateColumns="False"
                            GridLines="None" CssClass="SoarNPGridStyle"
                            OnRowDeleting="gvAvRole_RowDeleting" OnRowUpdating="gvAvRole_RowUpdating" OnRowEditing="gvAvRole_RowEditing"
                            OnRowCancelingEdit="gvAvRole_RowCancelingEdit" HorizontalAlign="Center">
                             <Columns>    
                                 <asp:TemplateField HeaderText="&nbsp;- - - - Aviator Role - - - -&nbsp;">    
                                     <ItemTemplate>    
                                        <asp:Label runat="server" ID="lblAvRole" Text='<%# Bind("sAviatorRole") %>'/>    
                                     </ItemTemplate>
                                     <EditItemTemplate>
                                        <asp:textBox ID="txbAvRole" runat="server" text='<%# Bind("sAviatorRole") %>' AutoCompleteType="Disabled" />
                                     </EditItemTemplate>
                                 </asp:TemplateField>
                                 <asp:CommandField ShowEditButton="true" EditText="&nbsp;Modify&nbsp;" HeaderStyle-CssClass="text-center" />
                                 <asp:CommandField DeleteText="&nbsp;Delete&nbsp;" ShowDeleteButton="True" HeaderStyle-CssClass="text-center"/>
                             </Columns>    
                        </asp:GridView>
                    </div>
                </Content>
            </ajaxToolkit:AccordionPane>  
            
            <ajaxToolkit:AccordionPane ID="AccPEquipRoles" runat="server">
                <Header> Equipment Roles </Header>
                <Content>
                    <div class="text-center">
                        <b>Create a New Equipment Role: </b>
<%--                            <asp:TextBox ID="txbNewEqRole" runat="server" AutoCompleteType="Disabled"></asp:TextBox>--%>
                        <asp:Table runat="server" BorderStyle="None" HorizontalAlign="Center">
                            <asp:TableRow>
                                <asp:TableCell>Name of Equipment Role:</asp:TableCell>
                                <asp:TableCell><asp:TextBox ID="txbNewEqRName" runat="server" AutoCompleteType="Disabled" ></asp:TextBox></asp:TableCell>
                            </asp:TableRow>
                            <asp:TableRow>
                                <asp:TableCell>Average Duration per Use in Minutes:</asp:TableCell>
                                <asp:TableCell><asp:TextBox ID="txbNewEqRDur" runat="server" TextMode="Number"></asp:TextBox></asp:TableCell>
                            </asp:TableRow>
                            <asp:TableRow>
                                <asp:TableCell>Comment:</asp:TableCell>
                                <asp:TableCell><asp:TextBox ID="txbNewEqRComment" runat="server" AutoCompleteType="Disabled" /></asp:TableCell>
                            </asp:TableRow>
                        </asp:Table>
                            <br />
                            <asp:Button ID="pbCreateEqRole" runat="server" Text="Create Equipment Role" OnClick="pbCreateEqRole_Click" />
                        <br /><br />
                        <asp:TextBox runat="server" Enabled="false" Width="350px" Height="110px" TextMode="MultiLine"
                            Text="NOTE: Equipment Roles that have to do with towing or launching must have the string `tow` or `Tow` in the role's name. Equipment Roles that do NOT have to do with towing or launching must NOT have the string `tow` or `Tow` in the role's name." />
                        <br />Table of Currently Defined Equipment Roles:
                        <br />
                        <asp:GridView ID="gvEqRole" runat="server" AutoGenerateColumns="False"
                            GridLines="None" CssClass="SoarNPGridStyle"
                            OnRowDeleting="gvEqRole_RowDeleting" OnRowUpdating="gvEqRole_RowUpdating" OnRowEditing="gvEqRole_RowEditing"
                            OnRowCancelingEdit="gvEqRole_RowCancelingEdit" HorizontalAlign="Center">
                             <Columns>    
                                 <asp:TemplateField HeaderText="&nbsp;- - - - Equipment Role - - - -&nbsp;">    
                                     <ItemTemplate>    
                                        <asp:Label runat="server" ID="lblEqRole" Text='<%# Bind("sEquipmentRole") %>'/>    
                                     </ItemTemplate>
                                     <EditItemTemplate>
                                        <asp:textBox ID="txbEqRole" runat="server" text='<%# Bind("sEquipmentRole") %>' AutoCompleteType="Disabled" />
                                     </EditItemTemplate>
                                 </asp:TemplateField>
                                 <asp:TemplateField HeaderText="&nbsp;Avg Use Duration, min&nbsp;">    
                                     <ItemTemplate>    
                                        <asp:Label runat="server" ID="lblDur" Text=' <%# Bind("iAvgUseDurationMinutes") %> ' />    
                                     </ItemTemplate>
                                     <EditItemTemplate>
                                        <asp:textBox ID="txbDur" runat="server" text='<%# Bind("iAvgUseDurationMinutes") %>' TextMode="Number" AutoCompleteType="Disabled" />
                                     </EditItemTemplate>
                                 </asp:TemplateField>    
                                 <asp:TemplateField HeaderText="&nbsp;Comment&nbsp;">    
                                     <ItemTemplate>    
                                        <asp:Label runat="server" ID="lblComment" Text='<%# Bind("sComment") %>'/>    
                                     </ItemTemplate>
                                     <EditItemTemplate>
                                        <asp:textBox ID="txbComment" runat="server" text='<%# Bind("sComment") %>' AutoCompleteType="Disabled" Width="300" />
                                     </EditItemTemplate>
                                 </asp:TemplateField>
                                 <asp:CommandField ShowEditButton="true" EditText="&nbsp;Modify&nbsp;" HeaderStyle-CssClass="text-center" />
                                 <asp:CommandField DeleteText="&nbsp;Delete&nbsp;" ShowDeleteButton="True" HeaderStyle-CssClass="text-center"/>
                             </Columns>
                        </asp:GridView>
                    </div>
                </Content>
            </ajaxToolkit:AccordionPane>

            <ajaxToolkit:AccordionPane ID="AccPSpecialOpTypes" runat="server">
                <Header> Types of Special Operations </Header>
                <Content>
                    <div class="text-center">
                        <b>Create a New Special Operation Type: </b>
                            <asp:TextBox ID="txbNewSpOpTy" runat="server" AutoCompleteType="Disabled"></asp:TextBox>
                            <br />
                            <asp:Button ID="pbCreateSpOpTy" runat="server" Text="Create Special Operation Type" OnClick="pbCreateSpOpTy_Click" />
                        <br /><br />
                        Table of Currently Defined Special Operation Types:
                        <br />
                        <asp:GridView ID="gvSpOpTy" runat="server" AutoGenerateColumns="False"
                            GridLines="None" CssClass="SoarNPGridStyle"
                            OnRowDeleting="gvSpOpTy_RowDeleting" OnRowUpdating="gvSpOpTy_RowUpdating" OnRowEditing="gvSpOpTy_RowEditing"
                            OnRowCancelingEdit="gvSpOpTy_RowCancelingEdit" HorizontalAlign="Center">
                             <Columns>    
                                 <asp:TemplateField HeaderText="&nbsp;- - - - - Special Operation Type - - - - -&nbsp;">    
                                     <ItemTemplate>    
                                        <asp:Label runat="server" ID="lblSpOpTy" Text='<%# Bind("sSpecialOpType") %>'/>    
                                     </ItemTemplate>
                                     <EditItemTemplate>
                                        <asp:textBox ID="txbSpOpTy" runat="server" text='<%# Bind("sSpecialOpType") %>' AutoCompleteType="Disabled" />
                                     </EditItemTemplate>
                                 </asp:TemplateField>
                                 <asp:CommandField ShowEditButton="true" EditText="&nbsp;Modify&nbsp;" HeaderStyle-CssClass="text-center" />
                                 <asp:CommandField DeleteText="&nbsp;Delete&nbsp;" ShowDeleteButton="True" HeaderStyle-CssClass="text-center"/>
                             </Columns>    
                        </asp:GridView>
                    </div>
                </Content>
            </ajaxToolkit:AccordionPane>        

            <ajaxToolkit:AccordionPane ID="AccPLaunchMethods" runat="server">
                <Header> Launch Methods </Header>
                <Content>
                    <div class="text-center">
                        <b>Create a New Launch Method: </b>
                            <asp:TextBox ID="txbNewLaunchM" runat="server" AutoCompleteType="Disabled"></asp:TextBox>
                            <br />
                        Single Character Abbreviation (must be unique among all Launch Method abbreviations):
                            <asp:TextBox ID="txbLMAbbrev" runat="server" AutoCompleteType="Disabled"></asp:TextBox>
                                <asp:RegularExpressionValidator runat="server" ValidationExpression="^[A-Z]|[a-z]$" ControlToValidate="txbLMAbbrev"
                                    ErrorMessage="Must be a single alphabetic character" Display="Dynamic" Font-Bold="true" ForeColor="Red" Font-Size="Medium" />
                            <br />
                            <asp:Button ID="pbCreateLaunchM" runat="server" Text="Create Launch Method" OnClick="pbCreateLaunchM_Click" />
                        <br /><br />
                        Table of Currently Defined Launch Methods:
                        <br />
                        <asp:GridView ID="gvLaunchM" runat="server" AutoGenerateColumns="False"
                            GridLines="None" CssClass="SoarNPGridStyle"
                            OnRowDeleting="gvLaunchM_RowDeleting" OnRowUpdating="gvLaunchM_RowUpdating" OnRowEditing="gvLaunchM_RowEditing"
                            OnRowCancelingEdit="gvLaunchM_RowCancelingEdit" HorizontalAlign="Center">
                             <Columns>
                                 <asp:TemplateField HeaderText="Internal ID">    
                                     <ItemTemplate>    
                                        <asp:Label runat="server" ID="lblID" Text='<%# Bind("ID") %>'/>
                                     </ItemTemplate>
                                 </asp:TemplateField>
  
                                 <asp:TemplateField HeaderText="&nbsp;- - - - Launch Method - - - -&nbsp;">    
                                     <ItemTemplate>    
                                        <asp:Label runat="server" ID="lblLaunchM" Text='<%# Bind("sLaunchMethod") %>'/>    
                                     </ItemTemplate>
                                     <EditItemTemplate>
                                        <asp:textBox ID="txbLaunchM" runat="server" text='<%# Bind("sLaunchMethod") %>' AutoCompleteType="Disabled" />
                                     </EditItemTemplate>
                                 </asp:TemplateField>

                                 <asp:TemplateField HeaderText="&nbsp;Abbreviation&nbsp;">
                                     <ItemTemplate>
                                         <asp:Label runat="server" ID="lblAbbrev" Text='<%# Bind("cLaunchMethod") %>'></asp:Label>
                                     </ItemTemplate>
                                     <EditItemTemplate>
                                        <asp:textBox ID="txbAbbrev" runat="server" text='<%# Bind("cLaunchMethod") %>' AutoCompleteType="Disabled" />
                                     </EditItemTemplate>
                                 </asp:TemplateField>
                                 <asp:CommandField ShowEditButton="true" EditText="&nbsp;Modify&nbsp;" HeaderStyle-CssClass="text-center" />
                                 <asp:CommandField DeleteText="&nbsp;Delete&nbsp;" ShowDeleteButton="True" HeaderStyle-CssClass="text-center"/>
                             </Columns>    
                        </asp:GridView>
                    </div>
                </Content>
            </ajaxToolkit:AccordionPane>
            
            <ajaxToolkit:AccordionPane ID="AccP_EquipTypes" runat="server">
                <Header> Equipment Types </Header>
                <Content>
                    <div class="text-center">
                        <b>Create a New Equipment Type: </b>
                            <asp:TextBox ID="txbNewEqType" runat="server" AutoCompleteType="Disabled"></asp:TextBox>
                            <br />
                            <asp:Button ID="pbCreateEqType" runat="server" Text="Create Equipment Type" OnClick="pbCreateEqType_Click" />
                        <br /><br />
                        Table of Currently Defined Equipment Types:
                        <br />
                        <asp:GridView ID="gvEqType" runat="server" AutoGenerateColumns="False"
                            GridLines="None" CssClass="SoarNPGridStyle"
                            OnRowDeleting="gvEqType_RowDeleting" OnRowUpdating="gvEqType_RowUpdating" OnRowEditing="gvEqType_RowEditing"
                            OnRowCancelingEdit="gvEqType_RowCancelingEdit" HorizontalAlign="Center">
                             <Columns>    
                                 <asp:TemplateField HeaderText="&nbsp;- - - - Equipment Type - - - -&nbsp;">    
                                     <ItemTemplate>    
                                        <asp:Label runat="server" ID="lblEqType" Text='<%# Bind("sEquipmentType") %>'/>    
                                     </ItemTemplate>
                                     <EditItemTemplate>
                                        <asp:textBox ID="txbEqType" runat="server" text='<%# Bind("sEquipmentType") %>' AutoCompleteType="Disabled" />
                                     </EditItemTemplate>
                                 </asp:TemplateField>
                                 <asp:CommandField ShowEditButton="true" EditText="&nbsp;Modify&nbsp;" HeaderStyle-CssClass="text-center" />
                                 <asp:CommandField DeleteText="&nbsp;Delete&nbsp;" ShowDeleteButton="True" HeaderStyle-CssClass="text-center"/>
                             </Columns>    
                        </asp:GridView>
                    </div>
                </Content>
            </ajaxToolkit:AccordionPane>  
            
            <ajaxToolkit:AccordionPane ID="AccP_EquipActionTypes" runat="server">
                <Header> Equipment Action Types </Header>
                <Content>
                    <div class="text-center">
                        <b>Create a New Equipment Action Type: </b>
                            <asp:TextBox ID="txbNewEqActionType" runat="server" AutoCompleteType="Disabled"></asp:TextBox>
                            <br />
                            <asp:Button ID="pbCreateNewEqActionType" runat="server" Text="Create Equipment Action Type" OnClick="pbCreateNewEqActionType_Click" />
                        <br /><br />
                        Table of Currently Defined Equipment Action Types:
                        <br />
                        <asp:GridView ID="gvEqActType" runat="server" AutoGenerateColumns="False"
                            GridLines="None" CssClass="SoarNPGridStyle"
                            OnRowDeleting="gvEqActType_RowDeleting" OnRowUpdating="gvEqActType_RowUpdating" OnRowEditing="gvEqActType_RowEditing"
                            OnRowCancelingEdit="gvEqActType_RowCancelingEdit" HorizontalAlign="Center">
                             <Columns>    
                                 <asp:TemplateField HeaderText="&nbsp;- - Equipment Action Type - -&nbsp;">    
                                     <ItemTemplate>    
                                        <asp:Label runat="server" ID="lblEqActType" Text='<%# Bind("sEquipActionType") %>'/>    
                                     </ItemTemplate>
                                     <EditItemTemplate>
                                        <asp:textBox ID="txbEqActType" runat="server" text='<%# Bind("sEquipActionType") %>' AutoCompleteType="Disabled" />
                                     </EditItemTemplate>
                                 </asp:TemplateField>
                                 <asp:CommandField ShowEditButton="true" EditText="&nbsp;Modify&nbsp;" HeaderStyle-CssClass="text-center" />
                                 <asp:CommandField DeleteText="&nbsp;Delete&nbsp;" ShowDeleteButton="True" HeaderStyle-CssClass="text-center"/>
                             </Columns>    
                        </asp:GridView>
                    </div>
                </Content>
            </ajaxToolkit:AccordionPane>    
            
            <ajaxToolkit:AccordionPane ID="AccP_ContactTypes" runat="server">
                <Header> People Contact Types </Header>
                <Content>
                    <div class="text-center">
                        <b>Create a New Contact Type: </b>
                        <asp:Table runat="server" BorderStyle="None" HorizontalAlign="Center">
                            <asp:TableRow>
                                <asp:TableCell>Contact Type:</asp:TableCell>
                                <asp:TableCell><asp:TextBox ID="txbNewContTy" runat="server" AutoCompleteType="Disabled"></asp:TextBox></asp:TableCell>
                            </asp:TableRow>
                            <asp:TableRow>
                                <asp:TableCell>Does this contact type require a physical address?</asp:TableCell>
                                <asp:TableCell><asp:CheckBox ID="chbHasPA" runat="server" Checked="false" /></asp:TableCell>
                            </asp:TableRow>
                            <asp:TableRow>
                                <asp:TableCell>Default Priority Rank (0 to 100):</asp:TableCell>
                                <asp:TableCell><asp:TextBox ID="txbDefPrioRank" runat="server" AutoCompleteType="Disabled" TextMode="Number" Text="50"></asp:TextBox></asp:TableCell>
                            </asp:TableRow>
                        </asp:Table>
                        <br />
                        <asp:Button ID="pbCreateContTy" runat="server" Text="Create Contact Type" OnClick="pbCreateContTy_Click" />
                        <br /><br />
                        Table of Currently Defined Contact Types:
                        <br />
                        <asp:GridView ID="gvContTy" runat="server" AutoGenerateColumns="False"
                            GridLines="None" CssClass="SoarNPGridStyle"
                            OnRowDeleting="gvContTy_RowDeleting" OnRowUpdating="gvContTy_RowUpdating" OnRowEditing="gvContTy_RowEditing"
                            OnRowCancelingEdit="gvContTy_RowCancelingEdit" HorizontalAlign="Center">
                             <Columns>    
                                 <asp:TemplateField HeaderText="&nbsp;- - - - People Contact Type - - - -&nbsp;">    
                                     <ItemTemplate>    
                                        <asp:Label runat="server" ID="lblContTy" Text='<%# Bind("sPeopleContactType") %>'/>    
                                     </ItemTemplate>
                                     <EditItemTemplate>
                                        <asp:textBox ID="txbContTy" runat="server" text='<%# Bind("sPeopleContactType") %>' AutoCompleteType="Disabled" />
                                     </EditItemTemplate>
                                 </asp:TemplateField>
                                 <asp:TemplateField HeaderText="Physical Address Required?" >
                                     <ItemTemplate>
                                         <asp:CheckBox ID="chbIHasPA" runat="server" Checked='<%# Bind("bHasPhysAddr") %>' Enabled="false"/>
                                     </ItemTemplate>
                                     <EditItemTemplate>
                                         <asp:CheckBox ID="chbUHasPA" runat="server" Checked='<%# Bind("bHasPhysAddr") %>' Enabled="true"/>
                                     </EditItemTemplate>
                                 </asp:TemplateField>
                                 <asp:TemplateField HeaderText="&nbsp;Default Priority Rank&nbsp;">    
                                     <ItemTemplate>    
                                        <asp:Label runat="server" ID="lblDefRank" Text='<%# Bind("dDefaultRank") %>'/>    
                                     </ItemTemplate>
                                     <EditItemTemplate>
                                        <asp:textBox ID="txbDefRank" runat="server" text='<%# Bind("dDefaultRank") %>' AutoCompleteType="Disabled" TextMode="Number"/>
                                     </EditItemTemplate>
                                 </asp:TemplateField>
                                 <asp:CommandField ShowEditButton="true" EditText="&nbsp;Modify&nbsp;" HeaderStyle-CssClass="text-center" />
                                 <asp:CommandField DeleteText="&nbsp;Delete&nbsp;" ShowDeleteButton="True" HeaderStyle-CssClass="text-center"/>
                             </Columns>    
                        </asp:GridView>
                    </div>
                </Content>
            </ajaxToolkit:AccordionPane>   
            
            <ajaxToolkit:AccordionPane ID="AccP_BoardOffices" runat="server">
                <Header> What Offices are there on the Board of Directors? </Header>
                <Content>
                    <div class="text-center">
                        <b>Create a New Office: </b>
                            <asp:TextBox ID="txbNewOffice" runat="server" AutoCompleteType="Disabled"></asp:TextBox>
                            <br />
                            <asp:Button ID="pbCreateOffice" runat="server" Text="Create Office" OnClick="pbCreateOffice_Click" />
                        <br /><br />
                        Table of Currently Defined Board of Directors Offices:
                        <br />
                        <asp:GridView ID="gvOffice" runat="server" AutoGenerateColumns="False"
                            GridLines="None" CssClass="SoarNPGridStyle"
                            OnRowDeleting="gvOffice_RowDeleting" OnRowUpdating="gvOffice_RowUpdating" OnRowEditing="gvOffice_RowEditing"
                            OnRowCancelingEdit="gvOffice_RowCancelingEdit" HorizontalAlign="Center">
                             <Columns>    
                                 <asp:TemplateField HeaderText="&nbsp;- - - - - - - - - - Office - - - - - - - - - -&nbsp;">    
                                     <ItemTemplate>    
                                        <asp:Label runat="server" ID="lblOffice" Text='<%# Bind("sBoardOffice") %>'/>    
                                     </ItemTemplate>
                                     <EditItemTemplate>
                                        <asp:textBox ID="txbOffice" runat="server" text='<%# Bind("sBoardOffice") %>' AutoCompleteType="Disabled" />
                                     </EditItemTemplate>
                                 </asp:TemplateField>
                                 <asp:CommandField ShowEditButton="true" EditText="&nbsp;Modify&nbsp;" HeaderStyle-CssClass="text-center" />
                                 <asp:CommandField DeleteText="&nbsp;Delete&nbsp;" ShowDeleteButton="True" HeaderStyle-CssClass="text-center"/>
                             </Columns>    
                        </asp:GridView>
                    </div>
                </Content>
            </ajaxToolkit:AccordionPane>   
            
            <ajaxToolkit:AccordionPane ID="AccP_ChargeCodes" runat="server">
                <Header> Charge Codes </Header>
                <Content>
                    <div class="text-center">
                        <b>Create a Charge Code: </b>
                        <asp:Table runat="server" BorderStyle="None" HorizontalAlign="Center">
                            <asp:TableRow>
                                <asp:TableCell>Charge Code Description:</asp:TableCell>
                                <asp:TableCell><asp:TextBox ID="txbsNewCC" runat="server" AutoCompleteType="Disabled" ></asp:TextBox>
                                <asp:RegularExpressionValidator runat="server" ValidationExpression="^\s*.{1,30}\s*$" ControlToValidate="txbsNewCC"
                                    ErrorMessage="Must be between 1 and 30 characters long" Display="Dynamic" Font-Bold="true" ForeColor="Red" Font-Size="Medium" />
                                </asp:TableCell>
                            </asp:TableRow>
                            <asp:TableRow>
                                <asp:TableCell>Unique Single Alpha Upper Case Character Charge Code:</asp:TableCell>
                                <asp:TableCell><asp:TextBox ID="txbcNewCC" runat="server" Text="C" Width="15px"></asp:TextBox>
                                <asp:RegularExpressionValidator runat="server" ValidationExpression="^[A-Z]{1}$" ControlToValidate="txbcNewCC"
                                    ErrorMessage="Must be a single upper case alphabetic ASCII character" Display="Dynamic" Font-Bold="true" ForeColor="Red" Font-Size="Medium" />
                                </asp:TableCell>
                            </asp:TableRow>
                            <asp:TableRow>
                                <asp:TableCell>Charge for flight operation launch:</asp:TableCell>
                                <asp:TableCell><asp:CheckBox ID="chbNChrg4Launch" runat="server" Checked="true" /></asp:TableCell>
                            </asp:TableRow>
                            <asp:TableRow>
                                <asp:TableCell>Charge for equipment rental:</asp:TableCell>
                                <asp:TableCell><asp:CheckBox ID="chbNChrg4Rental" runat="server" Checked="true" /></asp:TableCell>
                            </asp:TableRow>
                        </asp:Table>
                        <br />
                        <asp:Button ID="pbCreateChargeCode" runat="server" Text="Create Charge Code" OnClick="pbCreateChargeCode_Click" />
                        <br /><br />
                        Table of Currently Defined Charge Codes:
                        <br />
                        <asp:GridView ID="gvChargeCode" runat="server" AutoGenerateColumns="False"
                            GridLines="None" CssClass="SoarNPGridStyle"
                            OnRowDeleting="gvChargeCode_RowDeleting" OnRowUpdating="gvChargeCode_RowUpdating" OnRowEditing="gvChargeCode_RowEditing"
                            OnRowCancelingEdit="gvChargeCode_RowCancelingEdit" HorizontalAlign="Center">
                             <Columns>    
                                 <asp:TemplateField HeaderText="Internal ID">    
                                     <ItemTemplate>    
                                        <asp:Label runat="server" ID="lblID" Text='<%# Bind("ID") %>'/>
                                     </ItemTemplate>
                                 </asp:TemplateField>
                                 <asp:TemplateField>
                                     <HeaderTemplate>
                                         <asp:Label runat="server" Text="&nbsp;- - - - -Charge Code - - - - -&nbsp;" ToolTip="Charge Code Description, from 1 to 30 characters long" />
                                     </HeaderTemplate>
                                     <ItemTemplate>    
                                        <asp:Label runat="server" ID="lblsChargeCode" Text='<%# Bind("sChargeCode") %>'/>
                                     </ItemTemplate>
                                     <EditItemTemplate>
                                        <asp:textBox ID="txbsChargeCode" runat="server" text='<%# Bind("sChargeCode") %>' AutoCompleteType="Disabled" />
                                        <asp:RegularExpressionValidator runat="server" ValidationExpression="^\s*.{1,30}\s*$" ControlToValidate="txbsChargeCode"
                                            ErrorMessage="Must be between 1 and 30 characters long" Display="Dynamic" Font-Bold="true" ForeColor="Red" Font-Size="Medium" />
                                     </EditItemTemplate>
                                 </asp:TemplateField>
                                 <asp:TemplateField>
                                     <HeaderTemplate>
                                         <asp:Label runat="server" ID="lblHChargeCode" Text="CC" ToolTip="Single Character Charge Code" />
                                     </HeaderTemplate>
                                     <ItemTemplate>    
                                        <asp:Label runat="server" ID="lblcChargeCode" Text='<%# Bind("cChargeCode") %>'/>
                                     </ItemTemplate>
                                     <EditItemTemplate>
                                        <asp:textBox ID="txbcChargeCode" runat="server" text='<%# Bind("cChargeCode") %>' AutoCompleteType="Disabled" Width="15px"/>
                                        <asp:RegularExpressionValidator runat="server" ValidationExpression="^[A-Z]{1}$" ControlToValidate="txbcChargeCode"
                                             ErrorMessage="Must be a single upper case alphabetic ASCII character A - Z" Display="Dynamic" Font-Bold="true" ForeColor="Red" Font-Size="Medium" />
                                     </EditItemTemplate>
                                 </asp:TemplateField>
                                 <asp:TemplateField>
                                     <HeaderTemplate>
                                         <asp:Label runat="server" ID="lblHChrg4Launch" Text="Charge for Launch?" />
                                     </HeaderTemplate>
                                     <ItemTemplate>    
                                        <asp:CheckBox runat="server" ID="chbIChrg4Launch" Checked='<%# Bind("bCharge4Launch") %>' Enabled="false"/>
                                     </ItemTemplate>
                                     <EditItemTemplate>
                                        <asp:CheckBox ID="chbEChrg4Launch" runat="server" Checked='<%# Bind("bCharge4Launch") %>' />
                                     </EditItemTemplate>
                                 </asp:TemplateField>
                                 <asp:TemplateField>
                                     <HeaderTemplate>
                                         <asp:Label runat="server" ID="lblHChrg4Rental" Text="Charge for Rental?" />
                                     </HeaderTemplate>
                                     <ItemTemplate>    
                                        <asp:CheckBox runat="server" ID="chbIChrg4Rental" Checked='<%# Bind("bCharge4Rental") %>' Enabled="false"/>
                                     </ItemTemplate>
                                     <EditItemTemplate>
                                        <asp:CheckBox ID="chbEChrg4Rental" runat="server" Checked='<%# Bind("bCharge4Rental") %>' />
                                     </EditItemTemplate>
                                 </asp:TemplateField>
                                 <asp:CommandField ShowEditButton="true" EditText="&nbsp;Modify&nbsp;" HeaderStyle-CssClass="text-center" />
                                 <asp:CommandField DeleteText="&nbsp;Delete&nbsp;" ShowDeleteButton="True" HeaderStyle-CssClass="text-center"/>
                             </Columns>    
                        </asp:GridView>
                    </div>
                </Content>
            </ajaxToolkit:AccordionPane>  
            
            <ajaxToolkit:AccordionPane ID="AccPSettings" runat="server">
                <Header>Preferences and Settings for this Web Site</Header>
                <Content>
                    <div>
                        For each user, a collection of settings and their values is created; those values are copied from the master list seen here.
                        From then on, the user controls his own settings to the extent that he is given permission via role assignments.
                        When you modify the value of a setting in the master list here, existing users' settings are not affected.
                    </div>
                    <asp:GridView ID="gvSettings" runat="server" OnRowCreated="gvSettings_RowCreated" AutoGenerateColumns="False"
                            GridLines="None" CssClass="SoarNPGridStyle"
                            OnRowEditing="gvSettings_RowEditing" OnRowDataBound="gvSettings_RowDataBound" OnRowCancelingEdit="gvSettings_RowCancelingEdit"
                            OnRowUpdating="gvSettings_RowUpdating" OnPageIndexChanging="gvSettings_PageIndexChanging"
                            AllowPaging="true" PageSize="20">
                        <PagerSettings Mode="NextPreviousFirstLast" Position="TopAndBottom" Visible="true" FirstPageText="&nbsp;First&nbsp;"
                            LastPageText="&nbsp;Last&nbsp;" NextPageText="&nbsp;Next&nbsp;" PreviousPageText="&nbsp;Previous&nbsp;" />
                        <PagerStyle CssClass="SoarNPpaging" />
                        <Columns>
                            <asp:TemplateField Visible="False" >
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblID" Text='<%# Eval("ID") %>' Width="5" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField ItemStyle-Width="100" HeaderText="&nbsp;Setting Name&nbsp;">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblSName" Text='<%# Eval("sSettingName") %>'/>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField ItemStyle-Width="200" HeaderText="&nbsp;Explanation&nbsp;">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblExplanation" Text='<%# Eval("sExplanation") %>'/>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField ItemStyle-Width="100" HeaderText="&nbsp;Setting Value&nbsp;"  HeaderStyle-Width="100" ControlStyle-Width="100" >
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblsVal" Text='<%# Eval("sSettingValue") %>'/>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:textBox ID="txbSVal" runat="server" text='<%# Eval("sSettingValue") %>' style="min-width:100%;" />
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField ItemStyle-Width="200" HeaderStyle-Width="200" ControlStyle-Width="200">
                                <HeaderTemplate>
                                    <asp:Label ID="lblHInTable" runat="server" Text="&nbsp;In TABLE.Field&nbsp;"
                                        ToolTip="Comma-separated list; each item is a pair: a TABLENAME and Field descriptor, separated by a period;
they determine where the sSettingValue needs to appear; this is used in database integrity checks."></asp:Label>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblsInTable" Text='<%# Eval("sInTable") %>'/>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:textBox ID="txbSInTable" runat="server" text='<%# Eval("sInTable") %>' style="min-width:100%;"/>
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField ItemStyle-Width="160" HeaderText="&nbsp;Comments&nbsp;" HeaderStyle-Width="160" ControlStyle-Width="160">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblComments" Text='<%# Eval("sComments") %>'/>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:textBox ID="txbComments" runat="server" text='<%# Eval("sComments") %>' style="min-width:100%;" />
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField ItemStyle-Width="100" HeaderText="&nbsp;User Selectable&nbsp;">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblUserSelectable" Text='<%# Eval("bUserSelectable") %>'/>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:CommandField ShowEditButton="true" EditText="Modify" HeaderStyle-CssClass="text-center" />
                        </Columns>
                    </asp:GridView>
                </Content>
            </ajaxToolkit:AccordionPane>

            <ajaxToolkit:AccordionPane ID="AccPFAItems" runat="server">
                <Header> Front Accounting (FA) Items </Header>
                <Content>
                    <div class="text-center">
                        <b>Create a New FA Item (must match an item code in FA): </b>
                            <asp:TextBox ID="txbFAItem" runat="server" AutoCompleteType="Disabled"></asp:TextBox>
                            <br />
                        FA Item Description (should but need not match item description in FA):
                            <asp:TextBox ID="txbFADescr" runat="server" AutoCompleteType="Disabled"></asp:TextBox>
                            <br />
                            <asp:Button ID="pbCreateFAItem" runat="server" Text="Create FA Item" OnClick="pbCreateFAItem_Click" />
                        <br /><br />
                        Table of Currently Defined Front Accounting Items:
                        <br />
                        <asp:GridView ID="gvFAItems" runat="server" AutoGenerateColumns="False"
                            GridLines="None" CssClass="SoarNPGridStyle"
                            OnRowDeleting="gvFAItems_RowDeleting" OnRowUpdating="gvFAItems_RowUpdating" OnRowEditing="gvFAItems_RowEditing"
                            OnRowCancelingEdit="gvFAItems_RowCancelingEdit" HorizontalAlign="Center">
                             <Columns>
                                 <asp:TemplateField HeaderText="Internal ID">    
                                     <ItemTemplate>    
                                        <asp:Label runat="server" ID="lblFAItemID" Text='<%# Bind("ID") %>'/>
                                     </ItemTemplate>
                                 </asp:TemplateField>
  
                                 <asp:TemplateField HeaderText="&nbsp;- - - - FA Item - - - -&nbsp;" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Left">    
                                     <ItemTemplate>    
                                        <asp:Label runat="server" ID="lblFAItemCode" Text='<%# Bind("sFA_ItemCode") %>'/>    
                                     </ItemTemplate>
                                     <EditItemTemplate>
                                        <asp:textBox ID="txbFAItemCode" runat="server" text='<%# Bind("sFA_ItemCode") %>' AutoCompleteType="Disabled" />
                                     </EditItemTemplate>
                                 </asp:TemplateField>

                                 <asp:TemplateField HeaderText="&nbsp;Description&nbsp;" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Left">
                                     <ItemTemplate>
                                         <asp:Label runat="server" ID="lblFAItemDescr" Text='<%# Bind("sItemDescription") %>'></asp:Label>
                                     </ItemTemplate>
                                     <EditItemTemplate>
                                        <asp:textBox ID="txbFAItemDescr" runat="server" text='<%# Bind("sItemDescription") %>' AutoCompleteType="Disabled" />
                                     </EditItemTemplate>
                                 </asp:TemplateField>

                                 <asp:CommandField ShowEditButton="true" EditText="&nbsp;Modify&nbsp;" HeaderStyle-CssClass="text-center" />
                                 <asp:CommandField DeleteText="&nbsp;Delete&nbsp;" ShowDeleteButton="True" HeaderStyle-CssClass="text-center"/>
                             </Columns>    
                        </asp:GridView>
                    </div>
                </Content>
            </ajaxToolkit:AccordionPane>

            <ajaxToolkit:AccordionPane ID="AccP_FA_PmtTerms" runat="server">
                <Header> Front Accounting (FA) Payment Terms </Header>
                <Content>
                    <div class="text-center">
                        <b>Create a New FA Payment Term (must match a payment term in FA): </b>
                            <asp:TextBox ID="txbPmtTerm" runat="server" AutoCompleteType="Disabled" TextMode="Number"></asp:TextBox>
                            <br />
                        FA Payment Term Description (should but need not match payment term description in FA):
                            <asp:TextBox ID="txbPTDescr" runat="server" AutoCompleteType="Disabled"></asp:TextBox>
                            <br />
                            <asp:Button ID="pbFAPmtTerm" runat="server" Text="Create FA Payment Term" OnClick="pbFAPmtTerm_Click" />
                        <br /><br />
                        Table of Currently Defined Front Accounting Payment Terms:
                        <br />
                        <asp:GridView ID="gvFAPmtTerms" runat="server" AutoGenerateColumns="False"
                            GridLines="None" CssClass="SoarNPGridStyle"
                            OnRowDeleting="gvFAPmtTerms_RowDeleting" OnRowUpdating="gvFAPmtTerms_RowUpdating" OnRowEditing="gvFAPmtTerms_RowEditing"
                            OnRowCancelingEdit="gvFAPmtTerms_RowCancelingEdit" HorizontalAlign="Center">
                             <Columns>
                                 <asp:TemplateField HeaderText="Internal ID">    
                                     <ItemTemplate>    
                                        <asp:Label runat="server" ID="lblFAPTID" Text='<%# Bind("ID") %>'/>
                                     </ItemTemplate>
                                 </asp:TemplateField>
  
                                 <asp:TemplateField HeaderText="&nbsp; FA Payment Term &nbsp;" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Left">    
                                     <ItemTemplate>    
                                        <asp:Label runat="server" ID="lblFAPmtTermsCode" Text='<%# Bind("iPmtTermsCode") %>'/>    
                                     </ItemTemplate>
                                     <EditItemTemplate>
                                        <asp:textBox ID="txbFAPmtTermsCode" runat="server" text='<%# Bind("iPmtTermsCode") %>' TextMode="Number" AutoCompleteType="Disabled" />
                                     </EditItemTemplate>
                                 </asp:TemplateField>

                                 <asp:TemplateField HeaderText="&nbsp;Description&nbsp;" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Left">
                                     <ItemTemplate>
                                         <asp:Label runat="server" ID="lblFAPTDescr" Text='<%# Bind("sDescription") %>'></asp:Label>
                                     </ItemTemplate>
                                     <EditItemTemplate>
                                        <asp:textBox ID="txbFAPTDescr" runat="server" text='<%# Bind("sDescription") %>' AutoCompleteType="Disabled" />
                                     </EditItemTemplate>
                                 </asp:TemplateField>

                                 <asp:CommandField ShowEditButton="true" EditText="&nbsp;Modify&nbsp;" HeaderStyle-CssClass="text-center" />
                                 <asp:CommandField DeleteText="&nbsp;Delete&nbsp;" ShowDeleteButton="True" HeaderStyle-CssClass="text-center"/>
                             </Columns>    
                        </asp:GridView>
                    </div>
                </Content>
            </ajaxToolkit:AccordionPane>

            <ajaxToolkit:AccordionPane ID="AccPItems" runat="server">
                <Header> Accounting Items in QuickBooks Online </Header>
                <Content>
                    <div class="text-center">
                        <b>Create a New Accounting Item: </b>
                            <asp:TextBox ID="txbNewAccItem" runat="server" AutoCompleteType="Disabled"></asp:TextBox>
                            <br />
                            <asp:Button ID="pbCreateAccItem" runat="server" Text="Create Accounting Item" OnClick="pbCreateAccItem_Click" />
                        <br /><br />
                        Table of Currently Defined QuickBooks Online Accounting Items (Note: they must exist in QuickBooks Online):
                        <br />
                        <asp:GridView ID="gvAccItem" runat="server" AutoGenerateColumns="False"
                            GridLines="None" CssClass="SoarNPGridStyle"
                            OnRowDeleting="gvAccItem_RowDeleting" OnRowUpdating="gvAccItem_RowUpdating" OnRowEditing="gvAccItem_RowEditing"
                            OnRowCancelingEdit="gvAccItem_RowCancelingEdit" HorizontalAlign="Center">
                             <Columns>    
                                 <asp:TemplateField HeaderText="&nbsp;- - - - QBO Accounting Item - - - -&nbsp;">    
                                     <ItemTemplate>    
                                        <asp:Label runat="server" ID="lblAccItem" Text='<%# Bind("sQBO_ItemName") %>'/>    
                                     </ItemTemplate>
                                     <EditItemTemplate>
                                        <asp:textBox ID="txbAccItem" runat="server" text='<%# Bind("sQBO_ItemName") %>' AutoCompleteType="Disabled" />
                                     </EditItemTemplate>
                                 </asp:TemplateField>
                                 <asp:CommandField ShowEditButton="true" EditText="&nbsp;Modify&nbsp;" HeaderStyle-CssClass="text-center" />
                                 <asp:CommandField DeleteText="&nbsp;Delete&nbsp;" ShowDeleteButton="True" HeaderStyle-CssClass="text-center"/>
                             </Columns>    
                        </asp:GridView>
                    </div>
                </Content>
            </ajaxToolkit:AccordionPane>    
            
            <ajaxToolkit:AccordionPane ID="AccPInvSrc" runat="server">
                <Header> Invoice Sources </Header>
                <Content>
                    <div class="text-center">
                        <b>Create a New Invoice Source: </b>
                            <asp:TextBox ID="txbNewInvSrc" runat="server" AutoCompleteType="Disabled"></asp:TextBox>
                            <br />
                            <asp:Button ID="pbCreateInvSrc" runat="server" Text="Create Invoice Source" OnClick="pbCreateInvSrc_Click"/>
                        <br /><br />
                        Table of Currently Defined Invoice Sources:
                        <br />
                        <asp:GridView ID="gvInvSrc" runat="server" AutoGenerateColumns="False"
                            GridLines="None" CssClass="SoarNPGridStyle"
                            OnRowDeleting="gvInvSrc_RowDeleting" OnRowUpdating="gvInvSrc_RowUpdating" OnRowEditing="gvInvSrc_RowEditing"
                            OnRowCancelingEdit="gvInvSrc_RowCancelingEdit" HorizontalAlign="Center">
                             <Columns>    
                                 <asp:TemplateField HeaderText="&nbsp;- - - - Invoice Source - - - -&nbsp;">    
                                     <ItemTemplate>    
                                        <asp:Label runat="server" ID="lblInvSrc" Text='<%# Bind("sInvoiceSource") %>'/>    
                                     </ItemTemplate>
                                     <EditItemTemplate>
                                        <asp:textBox ID="txbInvSrc" runat="server" text='<%# Bind("sInvoiceSource") %>' AutoCompleteType="Disabled" />
                                     </EditItemTemplate>
                                 </asp:TemplateField>
                                 <asp:CommandField ShowEditButton="true" EditText="&nbsp;Modify&nbsp;" HeaderStyle-CssClass="text-center" />
                                 <asp:CommandField DeleteText="&nbsp;Delete&nbsp;" ShowDeleteButton="True" HeaderStyle-CssClass="text-center"/>
                             </Columns>    
                        </asp:GridView>
                    </div>
                </Content>
            </ajaxToolkit:AccordionPane>

            <ajaxToolkit:AccordionPane ID="AccPFSCategs" runat="server" >
                <Header> Flight Operations Scheduling (FS) Categories of Signups </Header>
                <Content>
                    <div class="text-center">
                        <p>Flight Operations Scheduling Categories appear in <a href="../../Operations/OpsSchedule.aspx">Flight Operations Schedule</a>.
                            For the large screen version, they appear in the top row. For the small screen version, they appear in the left-most column.
                            There are three kinds of categories: Role, Equipment, Activity. The order in which categories appear within one kind is determined
                            by the integral values set for the variable 'Order'.
                        </p>
                        <b>Create a New FS Category: </b><br />
                        <asp:Table runat="server" BorderStyle="None" HorizontalAlign="Center">
                            <asp:TableRow>
                                <asp:TableCell>Name:</asp:TableCell>
                                <asp:TableCell><asp:TextBox ID="txbFSCNameNew" runat="server" AutoCompleteType="Disabled" ></asp:TextBox></asp:TableCell>
                            </asp:TableRow>
                            <asp:TableRow>
                                <asp:TableCell>The kind of FS Category:</asp:TableCell>
                                <asp:TableCell>
                                    <asp:DropDownList ID="DDLFSCKindNew" runat="server">
                                        <asp:ListItem Text="Role" Value="R" Selected="True" />
                                        <asp:ListItem Text="Equipment" Value="E" />
                                        <asp:ListItem Text="Activity" Value="A" />
                                    </asp:DropDownList>
                                </asp:TableCell>
                            </asp:TableRow>
                            <asp:TableRow>
                                <asp:TableCell>Order:</asp:TableCell>
                                <asp:TableCell><asp:TextBox ID="txbFSCOrderNew" runat="server" TextMode="Number" Width="50px" AutoCompleteType="Disabled" /></asp:TableCell>
                            </asp:TableRow>
                            <asp:TableRow>
                                <asp:TableCell>Notes:</asp:TableCell>
                                <asp:TableCell><asp:TextBox ID="txbFSCNotesNew" runat="server" Width="300px" AutoCompleteType="Disabled" /></asp:TableCell>
                            </asp:TableRow>
                        </asp:Table>
                        <br />
                        <asp:Button ID="pbCrCateg" runat="server" Text="Create FS Category" OnClick="pbCrCateg_Click" />
                        <br /><br />
                        Table of Currently Defined Flight Operations Scheduling (FS) Categories of Signups:
                        <br />
                        <asp:GridView ID="gvFSCategs" runat="server" AutoGenerateColumns="False"
                            GridLines="None" CssClass="SoarNPGridStyle"
                            OnRowDeleting="gvFSCategs_RowDeleting" OnRowUpdating="gvFSCategs_RowUpdating" OnRowEditing="gvFSCategs_RowEditing"
                            OnRowCancelingEdit="gvFSCategs_RowCancelingEdit" OnRowDataBound="gvFSCategs_RowDataBound" HorizontalAlign="Center">
                             <Columns>
                                 <asp:TemplateField HeaderText="&nbsp; - FS Category - &nbsp;">    
                                     <ItemTemplate>    
                                        <asp:Label runat="server" ID="lblFSCName" Text=' <%# Eval("sCateg") %> ' />    
                                     </ItemTemplate>
                                     <EditItemTemplate>
                                        <asp:textBox ID="txbFSCCateg" runat="server" text='<%# Eval("sCateg") %>' AutoCompleteType="Disabled" />
                                     </EditItemTemplate>
                                 </asp:TemplateField>    
                                 <asp:TemplateField HeaderText="&nbsp; - Kind of Category - &nbsp;">    
                                     <ItemTemplate>    
                                        <asp:Label runat="server" ID="lblFSCKind" Text=' <%# Eval("cKind") %> ' />    
                                     </ItemTemplate>
                                     <EditItemTemplate>
                                        <asp:DropDownList ID="DDLFSCKind" runat="server" AutoCompleteType="Disabled">
                                            <asp:ListItem Text="Role" Value="R" Selected="True" />
                                            <asp:ListItem Text="Equipment" Value="E" />
                                            <asp:ListItem Text="Activity" Value="A" />
                                        </asp:DropDownList>
                                     </EditItemTemplate>
                                 </asp:TemplateField>    
                                 <asp:TemplateField HeaderText="&nbsp; - Order - &nbsp;">    
                                     <ItemTemplate>    
                                        <asp:Label runat="server" ID="lblFSCOrder" Text=' <%# Eval("iOrder") %> ' />    
                                     </ItemTemplate>
                                     <EditItemTemplate>
                                        <asp:textBox ID="txbFSCOrder" runat="server" text='<%# Eval("iOrder") %>' TextMode="Number" Width="50px" AutoCompleteType="Disabled" />
                                     </EditItemTemplate>
                                 </asp:TemplateField>    
                                 <asp:TemplateField HeaderText="&nbsp; - Notes - &nbsp;">    
                                     <ItemTemplate>    
                                        <asp:Label runat="server" ID="lblFSCNotes" Text=' <%# Eval("sNotes") %> ' />    
                                     </ItemTemplate>
                                     <EditItemTemplate>
                                        <asp:textBox ID="txbFSCNotes" runat="server" text='<%# Eval("sNotes") %>' Width="300px" AutoCompleteType="Disabled" />
                                     </EditItemTemplate>
                                 </asp:TemplateField>    
                                 <asp:CommandField ShowEditButton="true" EditText="&nbsp;Modify&nbsp;" HeaderStyle-CssClass="text-center" />
                                 <asp:CommandField  ShowDeleteButton="True" DeleteText="&nbsp;Delete&nbsp;" HeaderStyle-CssClass="text-center"/>
                             </Columns>    
                        </asp:GridView>
                    </div>
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
