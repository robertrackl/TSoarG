<%@ Page Title="TSoar CMS_Contact" Language="C#" MasterPageFile="~/mTSoar.Master" AutoEventWireup="true" CodeBehind="CMS_Contacts.aspx.cs"
    Inherits="TSoar.ClubMembership.CMS_Contact" EnableEventValidation="false" %>

<asp:Content ID="ContentHeader" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="server">
    <asp:Label runat="server" Text="Display and edit Club Member Contact Information" Font-Italic="true" />
</asp:Content>

<asp:Content ID="ContentMain" ContentPlaceHolderID="ContentPlaceHolderMain" runat="server">
    <style>
        .cell-padding{padding:5px}
    </style>
    <asp:SqlDataSource ID="SqlDataSrc_Member" runat="server" ConnectionString="<%$ ConnectionStrings:SqlConn %>"
        SelectCommand="SELECT 0 AS ID,' ' AS sDisplayName UNION SELECT ID,sDisplayName FROM PEOPLE ORDER BY sDisplayName" >
    </asp:SqlDataSource>
    <div class="container body-content">
        <div class="row" >
            <div class="col-sm-4">
                <asp:CheckBox ID="chbMemberFilter" runat="server" AutoPostBack="true" Checked="true"
                    Text="Should the member filter be enabled?" OnCheckedChanged="chbMemberFilter_CheckedChanged"
                    ToolTip="In order to filter for a particular member, check this checkbox; find a record in the list below for the member. Click on `Action` in right-most column for various options." />
            </div>
            <div class="col-sm-4">
                <div style="text-align:center">
                    <asp:Label runat="server" Text="New Contact Information Entry:&nbsp;" ToolTip="Create a new record for contact information. A separate page will open." />
                    <asp:Button ID="pbNew" runat="server" Text="Create" OnClick="pbNew_Click" ToolTip="Create a new record for contact information. A separate page will open."/>
                </div>
            </div>
            <div class="col-sm-4">
                <div style="text-align:center">
                    <asp:Label runat="server" Text="Select a member for whom to filter:&nbsp;" 
                    ToolTip="After clicking on a member name in the dropdown list, the table of contacts below will be filtered for only the contact data for that member. You may need to enable the member filter (checkbox on the left or above) if it's not enabled already." />
                <asp:DropDownList ID="DDL_Member" runat="server" DataSourceID="SqlDataSrc_Member" AutoPostBack="true"
                    DataTextField="sDisplayName" DataValueField="ID" OnSelectedIndexChanged="DDL_Member_SelectedIndexChanged"
                    ToolTip="After clicking on a member name in the dropdown list, the table of contacts below will be filtered for only the contact data for that member. You may need to enable the member filter (checkbox on the left or above) if it's not enabled already." />
                </div>
            </div>
        </div>
    </div>
    <br />
    
    <div style="margin-left: auto; margin-right: auto; text-align: center;">
        <asp:Label runat="server" ID="lblNoRecords" Text="N O &nbsp; R E C O R D S" Visible="false" />
    </div>
    <asp:GridView ID="gvCMS_Contact" runat="server" AutoGenerateColumns="False"
            GridLines="None" CssClass="SoarNPGridStyle"
            OnPageIndexChanging="gvCMS_Contact_PageIndexChanging" OnRowCreated="gvCMS_Contact_RowCreated" OnRowDataBound="gvCMS_Contact_RowDataBound"
            AllowPaging="true" PageSize="20">
        <PagerSettings Mode="NextPreviousFirstLast" Position="TopAndBottom" Visible="true" FirstPageText="&nbsp;First&nbsp;"
            LastPageText="&nbsp;Last&nbsp;" NextPageText="&nbsp;Next&nbsp;" PreviousPageText="&nbsp;Previous&nbsp;" />
        <PagerStyle CssClass="SoarNPpaging" />
        <Columns>
            <asp:TemplateField HeaderStyle-Width="50" Visible="true">
                <HeaderTemplate>
                    <asp:Label runat="server" Text="Row ID"
                        ToolTip="An internal database record identifier; shown here only for purposes of confirming people contact information deletions, and referencing the table row when clicking in the `Action` column." />
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:Label runat="server" ID="lblID" Text='<%# Eval("ID") %>' />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField ItemStyle-Width="120">
                <HeaderTemplate>
                    <asp:Label runat="server" Text="Display Name" ToolTip="Member Display Name" />
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:Label runat="server" ID="lblDName" Text='<%# Eval("sDisplayName") %>'/>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField ItemStyle-Width="150">
                <HeaderTemplate>
                    <asp:Label runat="server" Text="Contact Type" ToolTip="The type of contact information." />
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:Label runat="server" ID="lblCType" Text='<%# Eval("sPeopleContactType") %>'/>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField ItemStyle-Width="100">
                <HeaderTemplate>
                    <asp:Label runat="server" Text="Valid From Date" ToolTip="From this date forward, the contact information is valid; ok to be empty (i.e., unknown start date)." />
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:Label runat="server" ID="lblDStart" Text='<%# sDateTimeFormat((DateTime?)Eval("DBegin")) %>' />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField ItemStyle-Width="100">
                <HeaderTemplate>
                    <asp:Label runat="server" Text="Valid Until Date" ToolTip="The contact information is valid until this date; ok to be empty (i.e., unknown end date)." />
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:Label runat="server" ID="lblDEnd" Text='<%# sDateTimeFormat((DateTime?)Eval("DEnd")) %>' />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField ItemStyle-Width="50">
                <HeaderTemplate>
                    <asp:Label runat="server" Text="Attached Address" ToolTip="A physical address can only be attached to certain contact types that require a physical address. If so, the `Contact Information` field is not used." />
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:Label ID="lblUAddress" runat="server" Visible="true" Text="None" />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField ItemStyle-Width="200">
                <HeaderTemplate>
                    <asp:Label runat="server" Text="Contact Information" ToolTip="Contains any contact information except physical addresses." />
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:Label runat="server" ID="lblInfo" Text='<%# Eval("sContactInfo") %>'/>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField ItemStyle-Width="60">
                <HeaderTemplate>
                    <asp:Label runat="server" Text="Rank" ToolTip="Used for sorting the display of contact entries for a member by importance or priority rank. Higher rank appears higher on the list." />
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:Label runat="server" ID="lblFName" Text='<%# Eval("dContactPriorityRanking") %>' />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField ItemStyle-Width="50">
                <HeaderTemplate>
                    <asp:Label runat="server" Text="Action" ToolTip="Offers a short list of actions available with any one contact entry"/>
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:Button ID="pbAction" runat="server"  Text="Action" OnClick="pbAction_Click" ControlStyle-Font-Size="X-Small" />
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>

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

    <div>
        <%-- ModalPopupExtender, popping up MPE_PanelAction and populating a RadioButtonList --%>
        <asp:LinkButton ID="Target2" runat="server" Text="T" CssClass="displayNone" />
        <ajaxToolkit:ModalPopupExtender ID="MPEAction" runat="server"
            TargetControlID="Target2" PopupControlID="MPE_PanelAction"
            BackgroundCssClass="background" />
        <asp:Panel ID="MPE_PanelAction" runat="server" CssClass="popup" style="display:none;" HorizontalAlign="Center" Width="350"
            ToolTip="`Show Address`: display a physical address if one is associated with this entry. `Edit`: modify or update the entry. `Enable Member Filter`: for displaying contact data, turn on the member filter for this member.">
            What do you want to do with the contact record with ID '<asp:Label ID="lblActionID" runat="server" Text="TBD" />' ?
            <asp:RadioButtonList ID="rblAction" runat="server" RepeatDirection="Vertical" RepeatLayout="OrderedList" Align="left" TextAlign="Right" >
                <asp:ListItem Text="- Show Address" Selected="False" />
                <asp:ListItem Text="- Edit" Selected="True" />
                <asp:ListItem Text="- Enable Member Filter" Selected="False" />
                <asp:ListItem Text="- Create New Entry for this Member" Selected="False" />
                <asp:ListItem Text="- Delete" Selected="False" />
            </asp:RadioButtonList>
            <p> <asp:Button ID="pbOKAction" runat="server" Text="OK" OnClick="pbMPEAction_Click" ToolTip="Perform the action"/>&nbsp;&nbsp;
                <asp:Button ID="pbCancelAction" runat="server" Text="Cancel" OnClick="pbMPEAction_Click" ToolTip="Return to display of contact data without taking any action"/>&nbsp;&nbsp;
        </asp:Panel>
    </div>

</asp:Content>

<asp:Content ID="ContentFooter" ContentPlaceHolderID="ContentPlaceHolderFooter" runat="server">
    <%: Title %>
</asp:Content>