<%@ Page Title="AdvStatsFilter" Language="C#" MasterPageFile="~/mTSoar.Master" AutoEventWireup="true"
    CodeBehind="AdvStatsFilter.aspx.cs" Inherits="TSoar.MemberPages.Stats.AdvStatsFilter" %>

<asp:Content ID="HeaderContent" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="server">
    <asp:Label runat="server" Text="Set Flight Operations Statistics Advanced Filter Properties for the Current User" Font-Italic="true" />
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="ContentPlaceHolderMain" runat="server">
    <div title="SqlDataSources">
<%--        In the SelectCommand below, "UNION ALL SELECT ' ','0'" puts a blank item into the first position of the dropdown list--%>
        <asp:SqlDataSource ID="SqlDataSrc_Aviators" runat="server" ConnectionString="<%$ ConnectionStrings:SqlConn %>"
            SelectCommand="SELECT DISTINCT PEOPLE.sDisplayName, CONVERT(varchar(12), PEOPLE.ID) AS ID
                FROM AVIATORS INNER JOIN 
                OPDETAILS ON AVIATORS.iOpDetail = OPDETAILS.ID INNER JOIN 
                OPERATIONS ON OPDETAILS.iOperation = OPERATIONS.ID INNER JOIN 
                PEOPLE ON AVIATORS.iPerson = PEOPLE.ID
                UNION ALL SELECT ' ','0' 
                ORDER BY PEOPLE.sDisplayName"/>
        <asp:SqlDataSource ID="SqlDataSrc_AvRole" runat="server" ConnectionString="<%$ ConnectionStrings:SqlConn %>"
            SelectCommand="SELECT ID,sAviatorRole FROM AVIATORROLES UNION ALL SELECT '0',' ' ORDER BY sAviatorRole" />
        <asp:SqlDataSource ID="SqlDataSrc_Location" runat="server" ConnectionString="<%$ ConnectionStrings:SqlConn %>"
            SelectCommand="SELECT ID,sLocation FROM LOCATIONS UNION ALL SELECT '0',' ' ORDER BY sLocation" />
        <asp:SqlDataSource ID="SqlDataSrc_ChargeCode" runat="server" ConnectionString="<%$ ConnectionStrings:SqlConn %>"
            SelectCommand="SELECT ID,sChargeCode FROM CHARGECODES UNION ALL SELECT '0',' ' ORDER BY sChargeCode" />
        <asp:SqlDataSource ID="SqlDataSrc_LaunchMethod" runat="server" ConnectionString="<%$ ConnectionStrings:SqlConn %>"
            SelectCommand="SELECT ID,sLaunchMethod FROM LAUNCHMETHODS UNION ALL SELECT '0',' ' ORDER BY sLaunchMethod" />
        <asp:SqlDataSource ID="SqlDataSrc_SpecialOpTypes" runat="server" ConnectionString="<%$ ConnectionStrings:SqlConn %>"
            SelectCommand="SELECT ID,sSpecialOpType FROM SPECIALOPTYPES UNION ALL SELECT '0',' ' ORDER BY sSpecialOpType" />
        <asp:SqlDataSource ID="SqlDataSrc_Equip" runat="server" ConnectionString="<%$ ConnectionStrings:SqlConn %>"
            SelectCommand="SELECT ID,sShortEquipName FROM EQUIPMENT UNION ALL SELECT '0',' ' ORDER BY sShortEquipName" />
        <asp:SqlDataSource ID="SqlDataSrc_EquipmentRole" runat="server" ConnectionString="<%$ ConnectionStrings:SqlConn %>"
            SelectCommand="SELECT ID,sEquipmentRole FROM EQUIPMENTROLES UNION ALL SELECT '0',' ' ORDER BY sEquipmentRole" />
        <asp:SqlDataSource ID="SqlDataSrc_EquimentType" runat="server" ConnectionString="<%$ ConnectionStrings:SqlConn %>"
            SelectCommand="SELECT ID,sEquipmentType FROM EQUIPTYPES UNION ALL SELECT '0',' ' ORDER BY sEquipmentType" />
    </div>
    <div>
        <div style="text-align:center">
        <p><B>Note:</B> It is easy to lose any filter setting changes by leaving this page without clicking on the 
            '<B>Save these filter settings</B>' button at the bottom.</p>
        <p>Use filtering in order to show only those flight operations which satisfy certain criteria. For example, you might want to
            display only the flights where you were a glider pilot. In addition, you might want to restrict the display to the flights in July 2017 only.
            In addition, you might want to show only your flights in the PW-6U. It is possible to build simple and fairly complex filters. 
            The system remembers the last filter you built.
        </p>
        <hr />
        <asp:CheckBox ID="chbEnableFiltering" runat="server" Text="Check to turn on filtering; uncheck to not use any filters" 
            Checked="true" TextAlign="Right" OnCheckedChanged="chb_CheckedChanged" Font-Size="Large" /><br />
        <asp:Label runat="server"
            Text="Your previous filter settings had to be deleted because of a filter settings data table version update; sorry about that!"
            ID="lblVersionUpdate" Font-Bold="true" ForeColor="Red" Visible="false" />
            <h4>Filters Using a List of Items</h4>
            <ul>
            <li>The function of a list of items is to allow a match on any one or more of the items.</li>
            <li>The overall filter will include only those flight operations for which each filter marked in 'Use' has found at least one match.</li>
            <li>Lists cannot be edited directly; you can reset a list to just 'All', and you can add to the list from the drop down in the right-most column.</li>
            <li>Pay attention to the tool tips: floating text that appears when your mouse cursor hovers over many of the elements below.</li>
            <li>Tow plane flights are separate from glider flights; to not show tow plane flights set the equipment role filter to 'Glider' with the 'Use' and 'In' checkboxes checked.</li>
            </ul>
        </div>
        <asp:Table runat="server" ID="tblIN" BorderStyle="Solid" GridLines="Both" HorizontalAlign="Center" CssClass="SoarNPGridStyle" >
            <asp:TableHeaderRow>
                <asp:TableHeaderCell runat="server" Text="Filter Name" CssClass="text-center" />
                <asp:TableHeaderCell runat="server" Text="Use <span style='color:red; font-weight:bold'>(1)</span>" CssClass="text-center" />
                <asp:TableHeaderCell runat="server" Text="In <span style='color:red; font-weight:bold'>(2)</span>" CssClass="text-center" />
                <asp:TableHeaderCell runat="server" Text="<span style='font-size:smaller'>Reset List</span>" CssClass="text-center" ToolTip="Click a button in this column to change the contents of the list to 'All'" />
                <asp:TableHeaderCell runat="server" CssClass="text-center"  >
                    <asp:Label Text="List of comma-separated items" runat="server" BackColor="White" Width="280" BorderColor="Gray" BorderStyle="Solid" BorderWidth="1" ToolTip="Lists can be wiped out by clicking the corresponding RESET button one column to the left" />
                    <asp:Label Text="Selector (adds to list)" runat="server" BackColor="White" Width="280" BorderColor="Gray" BorderStyle="Solid" BorderWidth="1" ToolTip="Click on a filter's drop down list, then make a selection and click to add it to the list to the left" />
                </asp:TableHeaderCell></asp:TableHeaderRow>
            <asp:TableRow>
                <asp:TableCell Text="Aviator" />
                <asp:TableCell HorizontalAlign="Center">
                    <asp:CheckBox runat="server" ID="chbAviator" Checked="false" OnCheckedChanged="chb_CheckedChanged" ToolTip="Check to turn the aviator filter on" />
                </asp:TableCell><asp:TableCell HorizontalAlign="Center">
                    <asp:CheckBox runat="server" ID="chbAviatorIN" Checked="true" OnCheckedChanged="chb_CheckedChanged" />
                </asp:TableCell><asp:TableCell HorizontalAlign="Center">
                    <asp:Button ID="pbResetAv" runat="server" Text="" OnClick="pbReset_Click" Height="15" CssClass="roundedbutton" ToolTip="Click to change the filter list of aviator names to just 'All'" />
                </asp:TableCell><asp:TableCell HorizontalAlign="Center">
                    <asp:TextBox ID="txbDDLAviator" runat="server" ReadOnly="true" Font-Size="X-Small" Text="All" CssClass="box_width" ToolTip="Wipe out this list by clicking Reset to the left; add to the list by selecting from the dropdown on the right" />
                    <asp:DropDownList ID="DDLAviator" runat="server" Width="300" DataSourceID="SqlDataSrc_Aviators" AutoPostBack="true"
                        DataTextField="sDisplayName" DataValueField="ID" OnSelectedIndexChanged="DDLAviator_SelectedIndexChanged" ToolTip="Select from this drop down an item on which to filter" />
                </asp:TableCell>

            </asp:TableRow>
            <asp:TableRow>
                <asp:TableCell Text="Aviator Role" />
                <asp:TableCell HorizontalAlign="Center">
                    <asp:CheckBox runat="server" ID="chbAvRole" Checked="false" OnCheckedChanged="chb_CheckedChanged" ToolTip="Check to turn the aviator role filter on" />
                </asp:TableCell><asp:TableCell HorizontalAlign="Center">
                    <asp:CheckBox runat="server" ID="chbAvRoleIN" Checked="true" OnCheckedChanged="chb_CheckedChanged" />
                </asp:TableCell><asp:TableCell HorizontalAlign="Center">
                    <asp:Button ID="pbResetAvRole" runat="server" Text="" OnClick="pbReset_Click" Height="15" CssClass="roundedbutton" ToolTip="Click to change the filter list of aviator roles to just 'All'" />
                </asp:TableCell><asp:TableCell HorizontalAlign="Center">
                    <asp:TextBox ID="txbDDLAvRole" runat="server" ReadOnly="true" Font-Size="X-Small" Text="All" CssClass="box_width" ToolTip="Wipe out this list by clicking Reset to the left; add to the list by selecting from the dropdown on the right" />
                    <asp:DropDownList ID="DDLAvRole" runat="server" Width="300" DataSourceID="SqlDataSrc_AvRole" AutoPostBack="true"
                        DataTextField="sAviatorRole" DataValueField="ID" OnSelectedIndexChanged="DDLAvRole_SelectedIndexChanged" ToolTip="Select from this drop down an item on which to filter" />
                </asp:TableCell>

            </asp:TableRow>
            <asp:TableRow>
                <asp:TableCell Text="Takeoff Location" />
                <asp:TableCell HorizontalAlign="Center">
                    <asp:CheckBox runat="server" ID="chbTOLocation" Checked="false" OnCheckedChanged="chb_CheckedChanged" ToolTip="Check to turn the takeoff location filter on" />
                </asp:TableCell><asp:TableCell HorizontalAlign="Center">
                    <asp:CheckBox runat="server" ID="chbTOLocationIN" Checked="true" OnCheckedChanged="chb_CheckedChanged" />
                </asp:TableCell><asp:TableCell HorizontalAlign="Center">
                    <asp:Button ID="pbResetTOLocation" runat="server" Text="" OnClick="pbReset_Click" Height="15" CssClass="roundedbutton" ToolTip="Click to change the filter list of takeoff locations to just 'All'" />
                </asp:TableCell><asp:TableCell HorizontalAlign="Center">
                    <asp:TextBox ID="txbDDLTOLocation" runat="server" ReadOnly="true" Font-Size="X-Small" Text="All" CssClass="box_width" ToolTip="Wipe out this list by clicking Reset to the left; add to the list by selecting from the dropdown on the right" />
                    <asp:DropDownList ID="DDLTOLocation" runat="server" Width="300" DataSourceID="SqlDataSrc_Location" AutoPostBack="true"
                        DataTextField="sLocation" DataValueField="ID" OnSelectedIndexChanged="DDLTOLocation_SelectedIndexChanged" ToolTip="Select from this drop down an item on which to filter" />
                </asp:TableCell>

            </asp:TableRow>
            <asp:TableRow>
                <asp:TableCell Text="Landing Location" />
                <asp:TableCell HorizontalAlign="Center">
                    <asp:CheckBox runat="server" ID="chbLDLocation" Checked="false" OnCheckedChanged="chb_CheckedChanged" ToolTip="Check to turn the landing location filter on" />
                </asp:TableCell><asp:TableCell HorizontalAlign="Center">
                    <asp:CheckBox runat="server" ID="chbLDLocationIN" Checked="true" OnCheckedChanged="chb_CheckedChanged" />
                </asp:TableCell><asp:TableCell HorizontalAlign="Center">
                    <asp:Button ID="pbResetLDLocation" runat="server" Text="" OnClick="pbReset_Click" Height="15" CssClass="roundedbutton" ToolTip="Click to change the filter list of landing locations to just 'All'"/>
                </asp:TableCell><asp:TableCell HorizontalAlign="Center">
                    <asp:TextBox ID="txbDDLLDLocation" runat="server" ReadOnly="true" Font-Size="X-Small" Text="All" CssClass="box_width" ToolTip="Wipe out this list by clicking Reset to the left; add to the list by selecting from the dropdown on the right" />
                    <asp:DropDownList ID="DDLLDLocation" runat="server" Width="300" DataSourceID="SqlDataSrc_Location" AutoPostBack="true"
                        DataTextField="sLocation" DataValueField="ID" OnSelectedIndexChanged="DDLLDLocation_SelectedIndexChanged" ToolTip="Select from this drop down an item on which to filter" />
                </asp:TableCell>

            </asp:TableRow>
            <asp:TableRow>
                <asp:TableCell Text="Charge Code" />
                <asp:TableCell HorizontalAlign="Center">
                    <asp:CheckBox runat="server" ID="chbChargeCode" Checked="false" OnCheckedChanged="chb_CheckedChanged" ToolTip="Check to turn the charge code filter on" />
                </asp:TableCell><asp:TableCell HorizontalAlign="Center">
                    <asp:CheckBox runat="server" ID="chbChargeCodeIN" Checked="true" OnCheckedChanged="chb_CheckedChanged" />
                </asp:TableCell><asp:TableCell HorizontalAlign="Center">
                    <asp:Button ID="pbResetChargeCode" runat="server" Text="" OnClick="pbReset_Click" Height="15" CssClass="roundedbutton" ToolTip="Click to change the filter list of charge codes to just 'All'"/>
                </asp:TableCell><asp:TableCell HorizontalAlign="Center">
                    <asp:TextBox ID="txbDDLChargeCode" runat="server" ReadOnly="true" Font-Size="X-Small" Text="All" CssClass="box_width" ToolTip="Wipe out this list by clicking Reset to the left; add to the list by selecting from the dropdown on the right" />
                    <asp:DropDownList ID="DDLChargeCode" runat="server" Width="300" DataSourceID="SqlDataSrc_ChargeCode" AutoPostBack="true"
                        DataTextField="sChargeCode" DataValueField="ID" OnSelectedIndexChanged="DDLChargeCode_SelectedIndexChanged" ToolTip="Select from this drop down an item on which to filter" />
                </asp:TableCell>
            </asp:TableRow>
            <asp:TableRow>
                <asp:TableCell Text="Launch Method" />
                <asp:TableCell HorizontalAlign="Center">
                    <asp:CheckBox runat="server" ID="chbLaunchMethod" Checked="false" OnCheckedChanged="chb_CheckedChanged" ToolTip="Check to turn the launch method filter on" />
                </asp:TableCell><asp:TableCell HorizontalAlign="Center">
                    <asp:CheckBox runat="server" ID="chbLaunchMethodIN" Checked="true" OnCheckedChanged="chb_CheckedChanged" />
                </asp:TableCell><asp:TableCell HorizontalAlign="Center">
                    <asp:Button ID="pbResetLaunchMethod" runat="server" Text="" OnClick="pbReset_Click" Height="15" CssClass="roundedbutton" ToolTip="Click to change the filter list of Launch methods to just 'All'"/>
                </asp:TableCell><asp:TableCell HorizontalAlign="Center">
                    <asp:TextBox ID="txbDDLLaunchMethod" runat="server" ReadOnly="true" Font-Size="X-Small" Text="All" CssClass="box_width" ToolTip="Wipe out this list by clicking Reset to the left; add to the list by selecting from the dropdown on the right" />
                    <asp:DropDownList ID="DDLLaunchMethod" runat="server" Width="300" DataSourceID="SqlDataSrc_LaunchMethod" AutoPostBack="true"
                        DataTextField="sLaunchMethod" DataValueField="ID" OnSelectedIndexChanged="DDLLaunchMethod_SelectedIndexChanged" ToolTip="Select from this drop down an item on which to filter" />
                </asp:TableCell>

            </asp:TableRow>
            <asp:TableRow>
                <asp:TableCell Text="Special Operation Types" />
                <asp:TableCell HorizontalAlign="Center">
                    <asp:CheckBox runat="server" ID="chbSpecialOps" Checked="false" OnCheckedChanged="chb_CheckedChanged" ToolTip="Check to turn the special operations filter on" />
                </asp:TableCell>
                <asp:TableCell HorizontalAlign="Center">
                    <asp:CheckBox runat="server" ID="chbSpecialOpsIN" Checked="true" OnCheckedChanged="chb_CheckedChanged" />
                </asp:TableCell>
                <asp:TableCell HorizontalAlign="Center">
                    <asp:Button ID="pbResetSpecialOps" runat="server" Text="" OnClick="pbReset_Click" Height="15" CssClass="roundedbutton" ToolTip="Click to change the filter list of Special Operations to just 'All'"/>
                </asp:TableCell>
                <asp:TableCell HorizontalAlign="Center">
                    <asp:TextBox ID="txbDDLSpecialOps" runat="server" ReadOnly="true" Font-Size="X-Small" Text="All" CssClass="box_width" ToolTip="Wipe out this list by clicking Reset to the left; add to the list by selecting from the dropdown on the right" />
                    <asp:DropDownList ID="DDLSpecialOps" runat="server" Width="300" DataSourceID="SqlDataSrc_SpecialOpTypes" AutoPostBack="true"
                        DataTextField="sSpecialOpType" DataValueField="ID" OnSelectedIndexChanged="DDLSpecialOps_SelectedIndexChanged" ToolTip="Select from this drop down an item on which to filter" />
                </asp:TableCell>

            </asp:TableRow>
            <asp:TableRow>
                <asp:TableCell Text="Equipment/Aircraft" />
                <asp:TableCell HorizontalAlign="Center">
                    <asp:CheckBox runat="server" ID="chbEquipment" Checked="false" OnCheckedChanged="chb_CheckedChanged" ToolTip="Check to turn the equipment/aircraft filter on" />
                </asp:TableCell><asp:TableCell HorizontalAlign="Center">
                    <asp:CheckBox runat="server" ID="chbEquipmentIN" Checked="true" OnCheckedChanged="chb_CheckedChanged" />
                </asp:TableCell><asp:TableCell HorizontalAlign="Center">
                    <asp:Button ID="pbResetEquipment" runat="server" Text="" OnClick="pbReset_Click" Height="15" CssClass="roundedbutton" ToolTip="Click to change the filter list of aircraft/equipment to just 'All'" />
                </asp:TableCell><asp:TableCell HorizontalAlign="Center">
                    <asp:TextBox ID="txbDDLEquipment" runat="server" ReadOnly="true" Font-Size="X-Small" Text="All" CssClass="box_width" ToolTip="Wipe out this list by clicking Reset to the left; add to the list by selecting from the dropdown on the right" />
                    <asp:DropDownList ID="DDLEquipment" runat="server" Width="300" DataSourceID="SqlDataSrc_Equip" AutoPostBack="true"
                        DataTextField="sShortEquipName" DataValueField="ID" OnSelectedIndexChanged="DDLEquipment_SelectedIndexChanged" ToolTip="Select from this drop down an item on which to filter" />
                </asp:TableCell>

            </asp:TableRow>
            <asp:TableRow>
                <asp:TableCell Text="Equipment Role" />
                <asp:TableCell HorizontalAlign="Center">
                    <asp:CheckBox runat="server" ID="chbEquipmentRole" Checked="false" OnCheckedChanged="chb_CheckedChanged" ToolTip="Check to turn the equipment role filter on" />
                </asp:TableCell><asp:TableCell HorizontalAlign="Center">
                    <asp:CheckBox runat="server" ID="chbEquipmentRoleIN" Checked="true" OnCheckedChanged="chb_CheckedChanged" />
                </asp:TableCell><asp:TableCell HorizontalAlign="Center">
                    <asp:Button ID="pbResetEquipmentRole" runat="server" Text="" OnClick="pbReset_Click" Height="15" CssClass="roundedbutton" ToolTip="Click to change the filter list of equipment roles to just 'All'" />
                </asp:TableCell><asp:TableCell HorizontalAlign="Center">
                    <asp:TextBox ID="txbDDLEquipmentRole" runat="server" ReadOnly="true" Font-Size="X-Small" Text="All" CssClass="box_width" ToolTip="Wipe out this list by clicking Reset to the left; add to the list by selecting from the dropdown on the right" />
                    <asp:DropDownList ID="DDLEquipmentRole" runat="server" Width="300" DataSourceID="SqlDataSrc_EquipmentRole" AutoPostBack="true"
                        DataTextField="sEquipmentRole" DataValueField="ID" OnSelectedIndexChanged="DDLEquipmentRole_SelectedIndexChanged" ToolTip="Select from this drop down an item on which to filter" />
                </asp:TableCell>

            </asp:TableRow>
            <asp:TableRow>
                <asp:TableCell Text="Equipment Type" />
                <asp:TableCell HorizontalAlign="Center">
                    <asp:CheckBox runat="server" ID="chbEquipmentType" Checked="false" OnCheckedChanged="chb_CheckedChanged" ToolTip="Check to turn the equipment type filter on" />
                </asp:TableCell><asp:TableCell HorizontalAlign="Center">
                    <asp:CheckBox runat="server" ID="chbEquipmentTypeIN" Checked="true" OnCheckedChanged="chb_CheckedChanged" />
                </asp:TableCell><asp:TableCell HorizontalAlign="Center">
                    <asp:Button ID="pbResetEquipmentType" runat="server" Text="" OnClick="pbReset_Click" Height="15" CssClass="roundedbutton" ToolTip="Click to change the filter list of equipment types to just 'All'" />
                </asp:TableCell><asp:TableCell HorizontalAlign="Center">
                    <asp:TextBox ID="txbDDLEquipmentType" runat="server" ReadOnly="true" Font-Size="X-Small" Text="All" CssClass="box_width" ToolTip="Wipe out this list by clicking Reset to the left; add to the list by selecting from the dropdown on the right" />
                    <asp:DropDownList ID="DDLEquipmentType" runat="server" Width="300" DataSourceID="SqlDataSrc_EquimentType" AutoPostBack="true"
                        DataTextField="sEquipmentType" DataValueField="ID" OnSelectedIndexChanged="DDLEquipmentType_SelectedIndexChanged" ToolTip="Select from this drop down an item on which to filter" />
                </asp:TableCell>

            </asp:TableRow>

        </asp:Table><div style="text-align:center">
        <p><span style='color:red; font-weight:bold'>
                    (1)</span>: Put checkmarks in this column to 'Use' a filter, uncheck to ignore<br /> <span style='color:red; font-weight:bold'>
                    (2)</span>: Put checkmarks in this column to include only those flight operations which have matching items 'In' the list;
                        uncheck to do the opposite: exclude all flight operations which have matching items 'In' the list</p>
                    <h4>Filters Using a Range of Values</h4></div><asp:Table runat="server" ID="tblRange" BorderStyle="Solid" GridLines="Both" HorizontalAlign="Center" CssClass="SoarNPGridStyle" >
            <asp:TableHeaderRow>
                <asp:TableHeaderCell runat="server" Text="Filter Name" CssClass="text-center" />
                <asp:TableHeaderCell runat="server" Text="Use <span style='color:red; font-weight:bold'>(1)</span>" CssClass="text-center" />
                <asp:TableHeaderCell runat="server" Text="Reset" CssClass="text-center" />
                <asp:TableHeaderCell runat="server" Text="Lower Limit" CssClass="text-center" />
                <asp:TableHeaderCell runat="server" Text="Upper Limit" CssClass="text-center" />
            </asp:TableHeaderRow>
            <asp:TableRow>
                <asp:TableCell Text="Takeoff Date" />
                <asp:TableCell HorizontalAlign="Center">
                    <asp:CheckBox runat="server" ID="chbTakeoffDate" OnCheckedChanged="chb_CheckedChanged" ToolTip="Check to turn the takeoff date filter on" />
                </asp:TableCell><asp:TableCell HorizontalAlign="Center">
                    <asp:Button ID="pbResetTakeoffDate" runat="server" Text="" OnClick="pbReset_Click" Height="15" CssClass="roundedbutton" ToolTip="Click to change the filter limits to include all years between 2000 and 2099" />
                </asp:TableCell><asp:TableCell HorizontalAlign="Center">
                    <asp:TextBox runat="server" ID="txbTakeoffDateLo" TextMode="Date" OnTextChanged="txb_TextChanged" />
                </asp:TableCell><asp:TableCell HorizontalAlign="Center">
                    <asp:TextBox runat="server" ID="txbTakeoffDateHi" TextMode="Date" OnTextChanged="txb_TextChanged" />
                </asp:TableCell></asp:TableRow><asp:TableRow>
                <asp:TableCell Text="Number of Occupants in one Aircraft" />
                <asp:TableCell HorizontalAlign="Center">
                    <asp:CheckBox runat="server" ID="chbNumOccup" OnCheckedChanged="chb_CheckedChanged" ToolTip="Check to turn the number of occupants filter on" />
                </asp:TableCell><asp:TableCell HorizontalAlign="Center">
                    <asp:Button ID="pbResetNumOccup" runat="server" Text="" OnClick="pbReset_Click" Height="15" CssClass="roundedbutton" ToolTip="Click to change the filter limits to include all number of occupants between 1 and 10" />
                </asp:TableCell><asp:TableCell HorizontalAlign="Center">
                    <asp:TextBox runat="server" ID="txbNumOccupLo" TextMode="Number" OnTextChanged="txb_TextChanged" />
                </asp:TableCell><asp:TableCell HorizontalAlign="Center">
                    <asp:TextBox runat="server" ID="txbNumOccupHi" TextMode="Number" OnTextChanged="txb_TextChanged" />
                </asp:TableCell></asp:TableRow><asp:TableRow>
                <asp:TableCell Text="Release Altitude, Feet MSL" />
                <asp:TableCell HorizontalAlign="Center">
                    <asp:CheckBox runat="server" ID="chbReleaseAltitude" OnCheckedChanged="chb_CheckedChanged" ToolTip="Check to turn the release altitude filter on" />
                </asp:TableCell><asp:TableCell HorizontalAlign="Center">
                    <asp:Button ID="pbResetReleaseAltitude" runat="server" Text="" OnClick="pbReset_Click" Height="15" CssClass="roundedbutton" ToolTip="Click to change the filter limits to include all release altitudes between -1000 and 30000 feet" />
                </asp:TableCell><asp:TableCell HorizontalAlign="Center">
                    <asp:TextBox runat="server" ID="txbReleaseAltitudeLo" TextMode="Number" OnTextChanged="txb_TextChanged" />
                </asp:TableCell><asp:TableCell HorizontalAlign="Center">
                    <asp:TextBox runat="server" ID="txbReleaseAltitudeHi" TextMode="Number" OnTextChanged="txb_TextChanged" />
                </asp:TableCell></asp:TableRow><asp:TableRow>
                <asp:TableCell Text="Tow Altitude Difference, Feet" />
                <asp:TableCell HorizontalAlign="Center">
                    <asp:CheckBox runat="server" ID="chbTowAltDiff" OnCheckedChanged="chb_CheckedChanged" ToolTip="Check to turn the tow altitude difference filter on" />
                </asp:TableCell><asp:TableCell HorizontalAlign="Center">
                    <asp:Button ID="pbResetTowAltDiff" runat="server" Text="" OnClick="pbReset_Click" Height="15" CssClass="roundedbutton" ToolTip="Click to change the filter limits to include all tow altitude differences between 0 and 30000 feet" />
                </asp:TableCell><asp:TableCell HorizontalAlign="Center">
                    <asp:TextBox runat="server" ID="txbTowAltDiffLo" TextMode="Number" OnTextChanged="txb_TextChanged" />
                </asp:TableCell><asp:TableCell HorizontalAlign="Center">
                    <asp:TextBox runat="server" ID="txbTowAltDiffHi" TextMode="Number" OnTextChanged="txb_TextChanged" />
                </asp:TableCell></asp:TableRow><asp:TableRow>
                <asp:TableCell Text="Duration, Minutes" />
                <asp:TableCell HorizontalAlign="Center">
                    <asp:CheckBox runat="server" ID="chbDuration" OnCheckedChanged="chb_CheckedChanged" ToolTip="Check to turn the flight duration filter on" />
                </asp:TableCell><asp:TableCell HorizontalAlign="Center">
                    <asp:Button ID="pbResetDuration" runat="server" Text="" OnClick="pbReset_Click" Height="15" CssClass="roundedbutton" ToolTip="Click to change the filter limits to include all durations between 0 and 2880 minutes" />
                </asp:TableCell><asp:TableCell HorizontalAlign="Center">
                    <asp:TextBox runat="server" ID="txbDurationLo" TextMode="Number" OnTextChanged="txb_TextChanged" />
                </asp:TableCell><asp:TableCell HorizontalAlign="Center">
                    <asp:TextBox runat="server" ID="txbDurationHi" TextMode="Number" OnTextChanged="txb_TextChanged" />
                </asp:TableCell></asp:TableRow></asp:Table><br />
        <div style="text-align:center">
            <hr />
        <asp:CheckBox ID="chb1stFlt" runat="server" Text="Show only first of season flights with instructor?" style="align-self:center"
            OnCheckedChanged="chb_CheckedChanged"/>
        <hr />
        <p> <asp:Button ID="pbOpOK" runat="server" Text="Display filtered data" OnClick="pbOpOK_Click" />&nbsp;&nbsp; <asp:Button ID="pbOpCancel" runat="server" Text="Abandon any filter setting changes" OnClick="pbOpCancel_Click" />
        </p></div>
    </div>

    <div>
        <%-- ModalPopupExtender, popping up MPE_Panel and dynamically populating lblPopupText --%>
        <asp:LinkButton ID="Target" runat="server" Text="T" CssClass="displayNone" />
        <ajaxToolkit:ModalPopupExtender ID="ModalPopExt" runat="server"
            TargetControlID="Target" PopupControlID="MPE_Panel"
            BackgroundCssClass="background" DropShadow="true" />
        <asp:Panel ID="MPE_Panel" runat="server" CssClass="popup" style="display:none;" HorizontalAlign="Center">
            <asp:Label ID="lblPopupText" runat="server" BackColor="#eeb5a2" Font-Bold="true" />
            <br /><br />
            <p> <asp:Button ID="OkButton" runat="server" Text="OK" OnClick="Button_Click" />&nbsp;&nbsp; <asp:Button ID="CancelButton" runat="server" Text="Cancel" OnClick="Button_Click" />&nbsp;&nbsp; <asp:Button ID="NoButton" runat="server" Text="No" OnClick="Button_Click" />&nbsp;&nbsp; <asp:Button ID="YesButton" runat="server" Text="Yes" OnClick="Button_Click"/></p>
        </asp:Panel>
    </div>

</asp:Content>

<asp:Content ID="FooterContent" ContentPlaceHolderID="ContentPlaceHolderFooter" runat="server">
    <%: Title %>
</asp:Content>