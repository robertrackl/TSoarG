<%@ Page Title="Flight Log Input" Language="C#" MasterPageFile="~/mTSoar.Master" AutoEventWireup="true" CodeBehind="FlightLogInput.aspx.cs" Inherits="TSoar.Statistician.FlightLogInput" %>

<asp:Content ID="ContentHeader" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="server">
    <asp:Label runat="server" Text="Input of Flight Operations Data from Daily Log Sheets" Font-Italic="true" />
</asp:Content>

<asp:Content ID="ContentMain" ContentPlaceHolderID="ContentPlaceHolderMain" runat="server">
NOTE: Some rarely occurring situations need to be handled in <a href="OpsDataInput.aspx">OpsDataInput</a>, such as: 
Special Operations data; two tow pilots in tow plane; more than two people in a glider.
    <ajaxToolkit:Accordion
        ID="AccordionFlightLogInput"
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
            <ajaxToolkit:AccordionPane ID="AccPFLHelp" runat="server" >
                <Header> Help </Header>
                <Content>
                    <h3>Help for Daily Flight Logs</h3>
                    <p>These web pages (this one and its child page 'FlightLogRows') are designed for easy transfer of flight operations data
                        from the daily log sheets (on legal size paper) to the database attached to this website. Here is a link to the daily log sheet form:</p>
                    <p><a href="../i/FlightlogLegalSize.pdf" target="_blank">Flight Log Form</a> - designed to be printed double-sided (the back has a field manager check list).</p>
                    <p>All flights that occur on a given day are in one daily flight log record on this page (use continuation sheets if the number of flights in one day exceeds
                        the capacity of one paper log sheet). Those daily flight log records are organized by the month in the next (second) accordion pane: Flight Log Index by Year and Month.
                        When you click on 'Select' to the left of a month, all daily flight log records for that month are listed in the third accordion pane: List of Daily Flight Logs for One Month.
                        Add a new daily flight log by filling in the data in the gold/orange box at the bottom of the list, and then clicking on the yellow 'Add' button on the right (you may need to scroll over there).
                    </p>
                    <p>
                        <ul>
                            <li>Field Manager: Preferably a name, or list of names; the software does not check the list for anything.</li>
                            <li>Main Tow Equipment: Any piece of equipment. The software does not check whether it is capable of towing.</li>
                            <li>Main Tow Operator: This dropdownlist contains the names of people who can tow. This is determined as follows:
                                <ul>
                                    <li>Either: The person is currently a club member in a membership category with a name containing the word 'Tow' (such as Dues-Paying Tow Pilot);</li>
                                    <li>Or: The person currently qualifies in a qualification with a name containing the word 'Tow', but does not end with the word 'Tow'.
                                        This excludes qualifications 'Aero Tow' or 'Ground Tow' (they are glider pilot-related), but includes 'PSSA Tow Pilot' or 'Tow Winch Operator'.
                                    </li>
                                </ul>
                            </li>
                            <li>Main Glider: Any piece of equipment. The user must be sure that it is a glider.</li>
                            <li>Total $ Collected: Is calculated from flight data input as the sum of moneys collected during the day of flight operations. This is not connected to the accounting system.</li>
                            <li>Notes/Comments: Anything you like.</li>
                        </ul>
                    </p>
                    <p>The List of Daily Flight Logs for One Month has several columns with buttons:</p>
                    <p>
                        <ul>
                            <li>Square blue button - allows you to edit/modify/update the data in this row.</li>
                            <li>Select: The 'FlightLogRows' page opens and lists all the flights on that day. The bottom line is for entering new flight data.</li>
                            <li>Post: All the flights in this Daily Flight Log are posted to appropriate database tables so that the flights become visible in the
                                <a href="OpsDataInput.aspx">Tree View</a>, and show up in the <a href="../MemberPages/Stats/ClubStats.aspx">Flight Statistics</a>.</li>
                            <li>Delete: Delete this flight log together with any individual flights that may have been entered.</li>
                        </ul>
                    </p>
                </Content>
            </ajaxToolkit:AccordionPane>        
            <ajaxToolkit:AccordionPane ID="AccPFLIndex" runat="server" >
                <Header> Flight Log Index by Year and Month </Header>
                <Content>
                    <div class="gvclass">
                        <asp:GridView ID="gvYearMonths" runat="server" AutoGenerateColumns="False"
                            GridLines="None" CssClass="SoarNPGridStyle"
                            ShowHeaderWhenEmpty="true" EmptyDataText="== No Data Found ==" AutoGenerateSelectButton="true"
                            OnRowCommand="gvYearMonths_RowCommand"
                            Font-Size="Small">
                            <SelectedRowStyle BackColor="YellowGreen" />
                            <Columns>
                                <asp:TemplateField ItemStyle-HorizontalAlign="Center">
                                    <HeaderTemplate>
                                        Year/Month
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:Label  ID="lblIYearMonth" runat="server" Text='<%# Eval("YearMonth") %>' ></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField ItemStyle-HorizontalAlign="Center">
                                    <HeaderTemplate>
                                        Number of Daily Logs
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:Label  ID="lblICount" runat="server" Text='<%# Eval("FlightLogCount") %>' ></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </div>
                </Content>
            </ajaxToolkit:AccordionPane>        
            <ajaxToolkit:AccordionPane ID="AccPFlogs" runat="server" >
                <Header> List of Daily Flight Logs for One Month </Header>
                <Content>
                    <div class="gvclass">
                        <asp:GridView ID="gvDailyFL" runat="server" AutoGenerateColumns="False"
                            GridLines="None" CssClass="SoarNPGridStyle"
                            OnRowDataBound="gvDailyFL_RowDataBound"
                            OnDataBound="gvDailyFL_DataBound"
                            ShowHeaderWhenEmpty="true"
                            Font-Size="Small">
                            <SelectedRowStyle BackColor="YellowGreen" />
                            <Columns>
                                <asp:TemplateField ItemStyle-HorizontalAlign="Center">
                                    <HeaderTemplate>
                                        <asp:Label ID="lblHID" runat="server" Text="Internal Id"></asp:Label>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:Label ID="lblIID" runat="server" Text='<%# Eval("ID") %>' ></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField ItemStyle-HorizontalAlign="Center">
                                    <HeaderTemplate>
                                        <asp:Label ID="lblHFlCt" runat="server" Text="Flight Count"></asp:Label>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:Label ID="lblIFlCt" runat="server" Text='<%# Eval("iFlightCount") %>' ></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField ItemStyle-HorizontalAlign="Center">
                                    <HeaderTemplate>
                                        <asp:Label ID="lblHFlPosted" runat="server" Text="Posted Flights"></asp:Label>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:Label ID="lblIFlPosted" runat="server" Text='<%# Eval("iFlightsPosted") %>' ></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <asp:Label ID="lblHDFlightOps" runat="server" Text="Daily Flight Log Date" Width="108" ToolTip="Date on which the flights in this log occurred"></asp:Label>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:Label ID="lblIDFlightOps" runat="server" Text='<%# TSoar.CustFmt.sFmtDate((DateTimeOffset)Eval("DFlightOps"),TSoar.CustFmt.enDFmt.DateOnly) %>' Width="108"></asp:Label>
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <asp:TextBox ID="txbDDFlightOps" runat="server" Text='<%# TSoar.CustFmt.sFmtDate((DateTimeOffset)Eval("DFlightOps"),TSoar.CustFmt.enDFmt.DateOnly).Replace("/","-") %>'
                                            TextMode="Date" Width="108" ></asp:TextBox>
                                    </EditItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <asp:Label ID="lblHsFldMgr" runat="server" Text="Field Manager(s)" Width="160" ></asp:Label>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:Label ID="lblIsFldMgr" runat="server" Text='<%# Eval("sFldMgr") %>' Width="160" ></asp:Label>
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <asp:TextBox ID="txbsFldMgrs" runat="server" Text='<%# Eval("sFldMgr") %>' Width="160" ></asp:TextBox>
                                    </EditItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <asp:Label ID="lblHiMainTowEquip" runat="server" Text="Main Tow Equip" Width="150" ToolTip="The tow plane, tow car, or winch used for most of the flights"></asp:Label>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:Label ID="lblIiMainTowEquip" runat="server" Text='<%# Eval("sShortEquipName") %>' Width="150"></asp:Label>
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <asp:DropDownList ID="DDLDiMainTowEquip" runat="server" Width="150" />
                                    </EditItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <asp:Label ID="lblHiMainTowOp" runat="server" Text="Main Tow Operator" Width="150"
                                            ToolTip="The person that operated towing equipment most of the time.
 To show up in this list, person must be qualified as PSSA Tow Pilot, and
 must have the aviator role Tow Pilot in the list of persons related to equipment roles and types"></asp:Label>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:Label ID="lblIiMainTowOp" runat="server" Text='<%# Eval("sDisplayName") %>' Width="150"></asp:Label>
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <asp:DropDownList ID="DDLDiMainTowOp" runat="server" Width="150" />
                                    </EditItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <asp:Label ID="lblHiMainGlider" runat="server" Text="Main Glider" Width="150" ToolTip="The glider used for most of the flights"></asp:Label>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:Label ID="lblIiMainGlider" runat="server" Text='<%# Eval("sMainGliderName") %>' Width="150"></asp:Label>
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <asp:DropDownList ID="DDLDiMainGlider" runat="server" Width="150" />
                                    </EditItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <asp:Label ID="lblHiMainLaunchMethod" runat="server" Text="Main Launch Method" Width="150" ToolTip="The launch method used for most of the flights"></asp:Label>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:Label ID="lblIiMainLaunchMethod" runat="server" Text='<%# Eval("sMainLaunchMethod") %>' Width="150"></asp:Label>
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <asp:DropDownList ID="DDLDiMainLaunchMethod" runat="server" Width="150" />
                                    </EditItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <asp:Label ID="lblHiMainLocation" runat="server" Text="Main Location" Width="120" ToolTip="The takeoff location used for most of the flights"></asp:Label>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:Label ID="lblIiMainLocation" runat="server" Text='<%# Eval("sLocation") %>' Width="120"></asp:Label>
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <asp:DropDownList ID="DDLDiMainLocation" runat="server" Width="120" />
                                    </EditItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField ItemStyle-HorizontalAlign="Right">
                                    <HeaderTemplate>
                                        <asp:Label ID="lblHAmtCollected" runat="server" Text="Total $ Collected" ></asp:Label>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:Label ID="lblIAmtCollected" runat="server" Text='<%# ((decimal)Eval("mTotalCollected")).ToString("N2") %>'  ></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <asp:Label ID="lblHNotes" runat="server" Text="Notes/Comments" Width="200" ></asp:Label>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:Label ID="lblINotes" runat="server" Text='<%# Eval("sNotes") %>' Width="200" ></asp:Label>
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <asp:TextBox ID="txbDNotes" runat="server" Text='<%# Eval("sNotes") %>' Width="200" ></asp:TextBox>
                                    </EditItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField ItemStyle-HorizontalAlign="Center">
                                    <HeaderTemplate>
                                        <asp:Label ID="lblHEdit" runat="server" Text="Edit / Update" ToolTip="Edit the flight log's overall parameters to the left of this button"></asp:Label>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:ImageButton ID="pbEdit" runat="server" ImageUrl="~/i/BlueButton.jpg" OnClick="pbEdit_Click" CssClass="text-center" />
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <asp:ImageButton ID="pbUpdate" runat="server" ImageUrl="~/i/Update.jpg" OnClick="pbUpdate_Click" />
                                        <asp:ImageButton ID="pbCancel" runat="server" ImageUrl="~/i/Cancel.jpg" OnClick="pbCancel_Click" />
                                    </EditItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <asp:Label ID="lblHSelect" runat="server" Text="Select" ToolTip="Work with the flight operations details in this daily flight log" />
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:Button ID="pbSelect" runat="server" Text="Select" OnClick="pbSelect_Click" />
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <asp:Label ID="lblHPost" runat="server" Text="Post" ToolTip="Post to database tables the flight operation details in this daily flight log" />
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:Button ID="pbPost" runat="server" Text="Post" OnClick="pbPost_Click" />
                                        <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Height="25px" Width="50px" />
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <asp:Label ID="lblDelete" runat="server" Text="Delete" ToolTip="Delete this daily flight operations log INCLUDING any associated flight operations details" />
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:Button ID="pbDelete" runat="server" Text="Delete" OnClick="pbDelete_Click" />
                                        <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Height="25px" Width="50px" />
                                    </ItemTemplate>
                                </asp:TemplateField>

                            </Columns>
                        </asp:GridView>
                    </div>
                </Content>
            </ajaxToolkit:AccordionPane>        
        </Panes>            
    </ajaxToolkit:Accordion>

    <div id="ModalPopupExtender">
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
