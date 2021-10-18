<%@ Page Title="Members" Language="C#" MasterPageFile="~/mTSoar.Master" AutoEventWireup="true"
    CodeBehind="Members.aspx.cs" Inherits="TSoar.MemberPages.Members" %>

<asp:Content ID="HeaderContent" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="server">
    <asp:Label runat="server" Text="The PSSA Members Pages" Font-Italic="true" />
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="ContentPlaceHolderMain" runat="server">
    <div class="HelpText" ><%-- SCR 213 --%>
        <h2>Main Members-Only Page</h2>
        <h3>Here is a list of what you can do:</h3>
        <div>
            <a href="MyMembership/MyMembership.aspx">My Membership Data</a><br />
            <a href="ChangePassword.aspx">Change password</a><br />
            <hr />
            <a href="OpsScheduleSignup/OpsScheduleSignup.aspx">Signups in Operations Schedule, Large Screen</a><br />
            <a href="OpsScheduleSignup/OpsSchedSigSMALL.aspx">Signups in Operations Schedule, Small Screen</a><br />
            <hr />
            <a href="Roster/ClubRoster.aspx">Club Roster</a><br />
            <a href="Stats/Statistics.aspx">Flying Statistics</a><br />
            <a href="EquipMaintStat/EqMaintStatus.aspx">Equipment Aging/Maintenance Status</a><br />
            <a href="EquipMaintStat/EqOpStat.aspx">Equipment Operational Status</a><br />
            <hr />
            <a href="FieldManager/FieldMgr.aspx">Info for Field Managers</a><br />
            <a href="MRewards/Mrewards.aspx">Towpilot/Instructor Rewards</a><br />
            <a href="MRewards/RewardRules.aspx">Towpilot/Instructor Reward Rules</a><br />
            <a href="TrainInstruct/TrainInstruct.aspx">Training and Instruction</a><br />
            <a href="TowPilots/TowPilots.aspx">Tow Pilots</a><br />
            <hr />
            <a href="Fin/MemberFinStat.aspx">Member Financial Status</a><br />
            <a href="Fin/EquitySharesReport.aspx">Member Equity Shares Report</a><br />
            <a href="Fin/MaintenanceReserves.aspx">Maintenance Reserves</a><br />
            <hr />
            <a href="ManageSettings/ManageSettings.aspx">Manage Settings</a><br />
        </div>
    </div><%-- SCR 213 --%>
</asp:Content>

<asp:Content ID="FooterContent" ContentPlaceHolderID="ContentPlaceHolderFooter" runat="server">
    <%: Title %>
</asp:Content>
