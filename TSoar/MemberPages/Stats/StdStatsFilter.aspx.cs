using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using TSoar.Statistician;
using TSoar.DB;

namespace TSoar.MemberPages.Stats
{
    public partial class StdStatsFilter : System.Web.UI.Page
    {
        private DataTable dtFilters = new DataTable("OpsStdFilterSetting", "TSoar.MemberPages.Stats");
        private PopulateTrVwFilters PopulTree = new PopulateTrVwFilters();
        private bool bFilterChanged
        {
            get
            {
                if (ViewState["bFilterChanged"] == null)
                {
                    return false;
                }
                else
                {
                    return (bool)ViewState["bFilterChanged"];
                }
            }
            set
            {
                ViewState["bFilterChanged"] = value;
                if (value)
                {
                    value = true;
                }
                //ActivityLog.oDiag("Debug", "StatsFilter.aspx.cs.dFilterChanged was set to " + value.ToString());
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            //ActivityLog.oDiag("Debug", "StatsFilter.aspx.cs.Page_Load was called");
            PopulTree.bCheck4dtStdFilterReset(dtFilters, lblVersionUpdate);
            bFilterChanged = false;
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            //ActivityLog.oDiag("Debug", "StatsFilter.aspx.cs.Page_PreRender Entry with IsPostBack=" + IsPostBack.ToString());
            if (!IsPostBack)
            {
                InitControls();
                bFilterChanged = false;
            }
            //ActivityLog.oDiag("Debug", "StatsFilter.aspx.cs.Page_PreRender Exit");
        }
        private void InitControls()
        {
            // Initialize the controls based upon the previously saved filter settings
            int ix = (int)Global.egOpsFilters.EnableFilteringOverall;
            dtFilters = AccountProfile.CurrentUser.OpsStdFilterSetting;
            chbEnableFiltering.Checked = (bool)dtFilters.Rows[ix][(int)Global.egAdvFilterProps.Enabled];

            InitListFilter((int)Global.egOpsFilters.Person, chbAviator, chbAviatorIN, txbDDLAviator);
            InitListFilter((int)Global.egOpsFilters.AviatorRole, chbAvRole, chbAvRoleIN, txbDDLAvRole);
            InitListFilter((int)Global.egOpsFilters.TakeoffLocation, chbTOLocation, chbTOLocationIN, txbDDLTOLocation);
            InitListFilter((int)Global.egOpsFilters.LandingLocation, chbLDLocation, chbLDLocationIN, txbDDLLDLocation);
            InitListFilter((int)Global.egOpsFilters.ChargeCode, chbChargeCode, chbChargeCodeIN, txbDDLChargeCode);
            InitListFilter((int)Global.egOpsFilters.LaunchMethod, chbLaunchMethod, chbLaunchMethodIN, txbDDLLaunchMethod);
            InitListFilter((int)Global.egOpsFilters.SpecialOps, chbSpecialOps, chbSpecialOpsIN, txbDDLSpecialOps);
            InitListFilter((int)Global.egOpsFilters.Equipment, chbEquipment, chbEquipmentIN, txbDDLEquipment);
            InitListFilter((int)Global.egOpsFilters.EquipmentRole, chbEquipmentRole, chbEquipmentRoleIN, txbDDLEquipmentRole);
            InitListFilter((int)Global.egOpsFilters.EquipmentType, chbEquipmentType, chbEquipmentTypeIN, txbDDLEquipmentType);

            ix = (int)Global.egOpsFilters.TakeoffDate;
            chbTakeoffDate.Checked = (bool)dtFilters.Rows[ix][(int)Global.egAdvFilterProps.Enabled];
            // Special use of 'List' for takeoff date:
            string[] sa = ((string)dtFilters.Rows[ix][(int)Global.egAdvFilterProps.List]).Split(',');
            txbTakeoffDateLo.Text = sa[0].Replace("'", "");
            txbTakeoffDateHi.Text = sa[1].Replace("'", "").Replace(" ", "");

            InitRangeFilter((int)Global.egOpsFilters.NumOccup, chbNumOccup, txbNumOccupLo, txbNumOccupHi);
            InitRangeFilter((int)Global.egOpsFilters.ReleaseAltit, chbReleaseAltitude, txbReleaseAltitudeLo, txbReleaseAltitudeHi);
            InitRangeFilter((int)Global.egOpsFilters.TowAltDiff, chbTowAltDiff, txbTowAltDiffLo, txbTowAltDiffHi);
            InitRangeFilter((int)Global.egOpsFilters.Duration, chbDuration, txbDurationLo, txbDurationHi);

            ix = (int)Global.egOpsFilters.FirstFlight;
            chb1stFlt.Checked = (bool)dtFilters.Rows[ix][(int)Global.egAdvFilterProps.Enabled];
        }
        private void InitListFilter(int iux, CheckBox chbu, CheckBox chbuIN, TextBox txbuList)
        {
            chbu.Checked = (bool)dtFilters.Rows[iux][(int)Global.egAdvFilterProps.Enabled];
            txbuList.Text = (string)dtFilters.Rows[iux][(int)Global.egAdvFilterProps.List];
            if (txbuList.Text.Length < 1)
            {
                txbuList.Text = "All";
            }
            chbuIN.Checked = true;
            chbuIN.Enabled = false;
        }
        private void InitRangeFilter(int iux, CheckBox chbu, TextBox txbuLo, TextBox txbuHi)
        {
            chbu.Checked = (bool)dtFilters.Rows[iux][(int)Global.egAdvFilterProps.Enabled];
            txbuLo.Text = ((decimal)dtFilters.Rows[iux][(int)Global.egAdvFilterProps.LowLimit]).ToString();
            txbuHi.Text = ((decimal)dtFilters.Rows[iux][(int)Global.egAdvFilterProps.HighLimit]).ToString();
        }

        #region Modal Popup
        //======================
        private void ButtonsClear()
        {
            NoButton.CommandArgument = "";
            NoButton.CommandName = "";
            YesButton.CommandArgument = "";
            YesButton.CommandName = "";
            OkButton.CommandArgument = "";
            OkButton.CommandName = "";
            CancelButton.CommandArgument = "";
            CancelButton.CommandName = "";
        }
        private void MPE_Show(Global.enumButtons eubtns)
        {
            NoButton.CssClass = "displayNone";
            YesButton.CssClass = "displayNone";
            OkButton.CssClass = "displayNone";
            CancelButton.CssClass = "displayNone";
            switch (eubtns)
            {
                case Global.enumButtons.NoYes:
                    NoButton.CssClass = "displayUnset";
                    YesButton.CssClass = "displayUnset";
                    break;
                case Global.enumButtons.OkOnly:
                    OkButton.CssClass = "displayUnset";
                    break;
                case Global.enumButtons.OkCancel:
                    OkButton.CssClass = "displayUnset";
                    CancelButton.CssClass = "displayUnset";
                    break;
            }
            ModalPopExt.Show();
        }
        protected void Button_Click(object sender, EventArgs e)
        {

        }
        private void ProcessPopupException(Global.excToPopup excu)
        {
            ButtonsClear();
            lblPopupText.Text = excu.sExcMsg();
            ActivityLog.oLog(ActivityLog.enumLogTypes.PopupException, 0, lblPopupText.Text);
            MPE_Show(Global.enumButtons.OkOnly);
        }
        #endregion

        protected void DDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            DropDownList ddl = (DropDownList)sender;
            TextBox txb = null;
            CheckBox chb = null;
            switch (ddl.ID)
            {
                case "DDLAviator":
                    txb = txbDDLAviator;
                    chb = chbAviator;
                    break;
                case "DDLAvRole":
                    txb = txbDDLAvRole;
                    chb = chbAvRole;
                    break;
                case "DDLTOLocation":
                    txb = txbDDLTOLocation;
                    chb = chbTOLocation;
                    break;
                case "DDLLDLocation":
                    txb = txbDDLLDLocation;
                    chb = chbLDLocation;
                    break;
                case "DDLChargeCode":
                    txb = txbDDLChargeCode;
                    chb = chbChargeCode;
                    break;
                case "DDLLaunchMethod":
                    txb = txbDDLLaunchMethod;
                    chb = chbLaunchMethod;
                    break;
                case "DDLSpecialOps":
                    txb = txbDDLSpecialOps;
                    chb = chbSpecialOps;
                    break;
                case "DDLEquipment":
                    txb = txbDDLEquipment;
                    chb = chbEquipment;
                    break;
                case "DDLEquipmentRole":
                    txb = txbDDLEquipmentRole;
                    chb = chbEquipmentRole;
                    break;
                case "DDLEquipmentType":
                    txb = txbDDLEquipmentType;
                    chb = chbEquipmentType;
                    break;
            }
            string s1 = ddl.SelectedItem.Text.Trim();
            if (s1.Length > 0)
            {
                txb.Text = "'" + s1 + "'";
                chb.Checked = true;
            }
            else
            {
                txb.Text = "All";
                chb.Checked = false;
            }
            bFilterChanged = true;
        }

        protected void pbOpOK_Click(object sender, EventArgs e)
        {
            if (!(bFilterListCheck("Aviator", chbAviator, chbAviatorIN, txbDDLAviator) &&
                bFilterListCheck("Aviator Role", chbAvRole, chbAvRoleIN, txbDDLAvRole) &&
                bFilterListCheck("Takeoff Location", chbTOLocation, chbTOLocationIN, txbDDLTOLocation) &&
                bFilterListCheck("Landing Location", chbLDLocation, chbLDLocationIN, txbDDLLDLocation) &&
                bFilterListCheck("Charge Code", chbChargeCode, chbChargeCodeIN, txbDDLChargeCode) &&
                bFilterListCheck("Launch Methods", chbLaunchMethod, chbLaunchMethodIN, txbDDLLaunchMethod) &&
                bFilterListCheck("Special Ops", chbSpecialOps, chbSpecialOpsIN, txbDDLSpecialOps) &&
                bFilterListCheck("Equipment", chbEquipment, chbEquipmentIN, txbDDLEquipment) &&
                bFilterListCheck("Equipment Role", chbEquipmentRole, chbEquipmentRoleIN, txbDDLEquipmentRole) &&
                bFilterListCheck("Equipment Type", chbEquipmentType, chbEquipmentTypeIN, txbDDLEquipmentType)
                ))
            {
                return;
            }
            if (!(bFilterRangeCheck("Takeoff Date", chbTakeoffDate, txbTakeoffDateLo, txbTakeoffDateHi) &&
                bFilterRangeCheck("Number of Occupants", chbNumOccup, txbNumOccupLo, txbNumOccupHi) &&
                bFilterRangeCheck("Release Altitude", chbReleaseAltitude, txbReleaseAltitudeLo, txbReleaseAltitudeHi) &&
                bFilterRangeCheck("Tow Altitude Difference", chbTowAltDiff, txbTowAltDiffLo, txbTowAltDiffHi) &&
                bFilterRangeCheck("Duration", chbDuration, txbDurationLo, txbDurationHi)
                ))
            {
                return;
            }

            // Remember the filter settings for the current user
            int ix = (int)Global.egOpsFilters.EnableFilteringOverall;
            dtFilters = AccountProfile.CurrentUser.OpsStdFilterSetting;
            dtFilters.Rows[ix][(int)Global.egAdvFilterProps.Enabled] = chbEnableFiltering.Checked;

            ix = (int)Global.egOpsFilters.Person; // Aviator
            dtFilters.Rows[ix][(int)Global.egAdvFilterProps.Enabled] = chbAviator.Checked;
            dtFilters.Rows[ix][(int)Global.egAdvFilterProps.INorEX] = chbAviatorIN.Checked;
            dtFilters.Rows[ix][(int)Global.egAdvFilterProps.List] = txbDDLAviator.Text;
            ix = (int)Global.egOpsFilters.AviatorRole;
            dtFilters.Rows[ix][(int)Global.egAdvFilterProps.Enabled] = chbAvRole.Checked;
            dtFilters.Rows[ix][(int)Global.egAdvFilterProps.INorEX] = chbAvRoleIN.Checked;
            dtFilters.Rows[ix][(int)Global.egAdvFilterProps.List] = txbDDLAvRole.Text;
            ix = (int)Global.egOpsFilters.TakeoffLocation;
            dtFilters.Rows[ix][(int)Global.egAdvFilterProps.Enabled] = chbTOLocation.Checked;
            dtFilters.Rows[ix][(int)Global.egAdvFilterProps.INorEX] = chbTOLocationIN.Checked;
            dtFilters.Rows[ix][(int)Global.egAdvFilterProps.List] = txbDDLTOLocation.Text;
            ix = (int)Global.egOpsFilters.LandingLocation;
            dtFilters.Rows[ix][(int)Global.egAdvFilterProps.Enabled] = chbLDLocation.Checked;
            dtFilters.Rows[ix][(int)Global.egAdvFilterProps.INorEX] = chbLDLocationIN.Checked;
            dtFilters.Rows[ix][(int)Global.egAdvFilterProps.List] = txbDDLLDLocation.Text;
            ix = (int)Global.egOpsFilters.ChargeCode;
            dtFilters.Rows[ix][(int)Global.egAdvFilterProps.Enabled] = chbChargeCode.Checked;
            dtFilters.Rows[ix][(int)Global.egAdvFilterProps.INorEX] = chbChargeCodeIN.Checked;
            dtFilters.Rows[ix][(int)Global.egAdvFilterProps.List] = txbDDLChargeCode.Text;
            ix = (int)Global.egOpsFilters.LaunchMethod;
            dtFilters.Rows[ix][(int)Global.egAdvFilterProps.Enabled] = chbLaunchMethod.Checked;
            dtFilters.Rows[ix][(int)Global.egAdvFilterProps.INorEX] = chbLaunchMethodIN.Checked;
            dtFilters.Rows[ix][(int)Global.egAdvFilterProps.List] = txbDDLLaunchMethod.Text;
            ix = (int)Global.egOpsFilters.SpecialOps;
            dtFilters.Rows[ix][(int)Global.egAdvFilterProps.Enabled] = chbSpecialOps.Checked;
            dtFilters.Rows[ix][(int)Global.egAdvFilterProps.INorEX] = chbSpecialOpsIN.Checked;
            dtFilters.Rows[ix][(int)Global.egAdvFilterProps.List] = txbDDLSpecialOps.Text;
            ix = (int)Global.egOpsFilters.Equipment;
            dtFilters.Rows[ix][(int)Global.egAdvFilterProps.Enabled] = chbEquipment.Checked;
            dtFilters.Rows[ix][(int)Global.egAdvFilterProps.INorEX] = chbEquipmentIN.Checked;
            dtFilters.Rows[ix][(int)Global.egAdvFilterProps.List] = txbDDLEquipment.Text;
            ix = (int)Global.egOpsFilters.EquipmentRole;
            dtFilters.Rows[ix][(int)Global.egAdvFilterProps.Enabled] = chbEquipmentRole.Checked;
            dtFilters.Rows[ix][(int)Global.egAdvFilterProps.INorEX] = chbEquipmentRoleIN.Checked;
            dtFilters.Rows[ix][(int)Global.egAdvFilterProps.List] = txbDDLEquipmentRole.Text;
            ix = (int)Global.egOpsFilters.EquipmentType;
            dtFilters.Rows[ix][(int)Global.egAdvFilterProps.Enabled] = chbEquipmentType.Checked;
            dtFilters.Rows[ix][(int)Global.egAdvFilterProps.INorEX] = chbEquipmentTypeIN.Checked;
            dtFilters.Rows[ix][(int)Global.egAdvFilterProps.List] = txbDDLEquipmentType.Text;

            ix = (int)Global.egOpsFilters.TakeoffDate;
            dtFilters.Rows[ix][(int)Global.egAdvFilterProps.Enabled] = chbTakeoffDate.Checked;
            // Special use of 'List' for takeoff date:
            dtFilters.Rows[ix][(int)Global.egAdvFilterProps.List] = "'" + txbTakeoffDateLo.Text + "','" + txbTakeoffDateHi.Text + "'";

            ix = (int)Global.egOpsFilters.NumOccup;
            dtFilters.Rows[ix][(int)Global.egAdvFilterProps.Enabled] = chbNumOccup.Checked;
            if (txbNumOccupLo.Text.Length < 1) { txbNumOccupLo.Text = "1"; }
            dtFilters.Rows[ix][(int)Global.egAdvFilterProps.LowLimit] = Int32.Parse(txbNumOccupLo.Text);
            if (txbNumOccupHi.Text.Length < 1) { txbNumOccupHi.Text = "10"; }
            dtFilters.Rows[ix][(int)Global.egAdvFilterProps.HighLimit] = Int32.Parse(txbNumOccupHi.Text);
            ix = (int)Global.egOpsFilters.ReleaseAltit;
            dtFilters.Rows[ix][(int)Global.egAdvFilterProps.Enabled] = chbReleaseAltitude.Checked;
            if (txbReleaseAltitudeLo.Text.Length < 1) { txbReleaseAltitudeLo.Text = "-1000"; }
            dtFilters.Rows[ix][(int)Global.egAdvFilterProps.LowLimit] = Int32.Parse(txbReleaseAltitudeLo.Text);
            if (txbReleaseAltitudeHi.Text.Length < 1) { txbReleaseAltitudeHi.Text = "30000"; }
            dtFilters.Rows[ix][(int)Global.egAdvFilterProps.HighLimit] = Int32.Parse(txbReleaseAltitudeHi.Text);
            ix = (int)Global.egOpsFilters.TowAltDiff;
            dtFilters.Rows[ix][(int)Global.egAdvFilterProps.Enabled] = chbTowAltDiff.Checked;
            if (txbTowAltDiffLo.Text.Length < 1) { txbTowAltDiffLo.Text = "0"; }
            dtFilters.Rows[ix][(int)Global.egAdvFilterProps.LowLimit] = Int32.Parse(txbTowAltDiffLo.Text);
            if (txbTowAltDiffHi.Text.Length < 1) { txbTowAltDiffHi.Text = "30000"; }
            dtFilters.Rows[ix][(int)Global.egAdvFilterProps.HighLimit] = Int32.Parse(txbTowAltDiffHi.Text);
            ix = (int)Global.egOpsFilters.Duration;
            dtFilters.Rows[ix][(int)Global.egAdvFilterProps.Enabled] = chbDuration.Checked;
            if (txbDurationLo.Text.Length < 1) { txbDurationLo.Text = "0"; }
            dtFilters.Rows[ix][(int)Global.egAdvFilterProps.LowLimit] = Int32.Parse(txbDurationLo.Text);
            if (txbDurationHi.Text.Length < 1) { txbDurationHi.Text = "2880"; }
            dtFilters.Rows[ix][(int)Global.egAdvFilterProps.HighLimit] = Int32.Parse(txbDurationHi.Text);

            ix = (int)Global.egOpsFilters.FirstFlight;
            dtFilters.Rows[ix][(int)Global.egAdvFilterProps.Enabled] = chb1stFlt.Checked;

            AccountProfile.CurrentUser.OpsStdFilterSetting = dtFilters; // Save filter settings for current user
            bFilterChanged = false;
            AccountProfile.CurrentUser.OpsFilterSettingSelection = "STANDARD";
            Server.Transfer("~/MemberPages/Stats/ClubStats.aspx", true);
        }

        private bool bFilterListCheck(string suItem, CheckBox chbuItem, CheckBox chbuIN, TextBox utxb)
        {
            if (chbuItem.Checked && chbuIN.Checked && utxb.Text.Length < 1)
            {
                ProcessPopupException(new Global.excToPopup(suItem + " filter is in use, but its list is empty"));
                return false;
            }
            return true;
        }
        private bool bFilterRangeCheck(string suItem, CheckBox chbuItem, TextBox txbLo, TextBox txbHi)
        {
            if (chbuItem.Checked)
            {
                if (txbLo.Text.Length < 1)
                {
                    ProcessPopupException(new Global.excToPopup(suItem + " Filter is in use, but its lower limit is empty"));
                    return false;
                }
                if (txbHi.Text.Length < 1)
                {
                    ProcessPopupException(new Global.excToPopup(suItem + " Filter is in use, but its upper limit is empty"));
                    return false;
                }
                if (suItem == "Takeoff Date")
                {
                    DateTime DLo = DateTime.Parse(txbLo.Text);
                    DateTime DHi = DateTime.Parse(txbHi.Text);
                    if (DLo > DHi)
                    {
                        ProcessPopupException(new Global.excToPopup(suItem + " Filter is in use, but its lower limit is greater than its upper limit"));
                        return false;
                    }
                }
                else
                {
                    decimal dLo = decimal.Parse(txbLo.Text);
                    decimal dHi = decimal.Parse(txbHi.Text);
                    if (dLo > dHi)
                    {
                        ProcessPopupException(new Global.excToPopup(suItem + " Filter is in use, but its lower limit is greater than its upper limit"));
                        return false;
                    }
                }
            }
            return true;
        }

        protected void pbOpCancel_Click(object sender, EventArgs e)
        {
            bFilterChanged = false;
            Server.Transfer("~/MemberPages/Stats/ClubStats.aspx", true);
        }

        protected void txb_TextChanged(object sender, EventArgs e)
        {
            //ActivityLog.oDiag("Debug", "StatsFilter.aspx.cs.txbTakeoffDateLo_TextChanged was called");
            bFilterChanged = true;
        }

        protected void chb_CheckedChanged(object sender, EventArgs e)
        {
            bFilterChanged = true;
        }


        protected void DDL_DataBound(object sender, EventArgs e)
        {
            DropDownList ddl = (DropDownList)sender;
            dtFilters = AccountProfile.CurrentUser.OpsStdFilterSetting;
            string sItem = "";

            switch (ddl.ID)
            {
                case "DDLAviator":
                    sItem = (string)dtFilters.Rows[(int)Global.egOpsFilters.Person][(int)Global.egAdvFilterProps.List];
                    break;
                case "DDLAvRole":
                    sItem = (string)dtFilters.Rows[(int)Global.egOpsFilters.AviatorRole][(int)Global.egAdvFilterProps.List];
                    break;
                case "DDLTOLocation":
                    sItem = (string)dtFilters.Rows[(int)Global.egOpsFilters.TakeoffLocation][(int)Global.egAdvFilterProps.List];
                    break;
                case "DDLLDLocation":
                    sItem = (string)dtFilters.Rows[(int)Global.egOpsFilters.LandingLocation][(int)Global.egAdvFilterProps.List];
                    break;
                case "DDLChargeCode":
                    sItem = (string)dtFilters.Rows[(int)Global.egOpsFilters.ChargeCode][(int)Global.egAdvFilterProps.List];
                    break;
                case "DDLLaunchMethod":
                    sItem = (string)dtFilters.Rows[(int)Global.egOpsFilters.LaunchMethod][(int)Global.egAdvFilterProps.List];
                    break;
                case "DDLSpecialOps":
                    sItem = (string)dtFilters.Rows[(int)Global.egOpsFilters.SpecialOps][(int)Global.egAdvFilterProps.List];
                    break;
                case "DDLEquipment":
                    sItem = (string)dtFilters.Rows[(int)Global.egOpsFilters.Equipment][(int)Global.egAdvFilterProps.List];
                    break;
                case "DDLEquipmentRole":
                    sItem = (string)dtFilters.Rows[(int)Global.egOpsFilters.EquipmentRole][(int)Global.egAdvFilterProps.List];
                    break;
                case "DDLEquipmentType":
                    sItem = (string)dtFilters.Rows[(int)Global.egOpsFilters.EquipmentType][(int)Global.egAdvFilterProps.List];
                    break;
            }
            if (sItem.Length > 0)
            {
                SetDropDownByValue(ddl, sItem);
            }
        }

        private void SetDropDownByValue(DropDownList ddlu, string suText)
        {
            suText = suText.Replace("'", "");
            ddlu.ClearSelection();
            foreach (ListItem li in ddlu.Items)
            {
                if (li.Text == suText)
                {
                    li.Selected = true;
                    break;
                }
            }
            ddlu.SelectedItem.Text = suText;
        }

        protected void pbReset_Click(object sender, EventArgs e)
        {
            Button pb = (Button)sender;
            bFilterChanged = true;
            switch (pb.ID)
            {
                case "pbResetTakeoffDate":
                    txbTakeoffDateLo.Text = "2000-01-01";
                    txbTakeoffDateHi.Text = "2099-12-31";
                    break;
                case "pbResetNumOccup":
                    txbNumOccupLo.Text = "1";
                    txbNumOccupHi.Text = "10";
                    break;
                case "pbResetReleaseAltitude":
                    txbReleaseAltitudeLo.Text = "-1000";
                    txbReleaseAltitudeHi.Text = "30000";
                    break;
                case "pbResetTowAltDiff":
                    txbTowAltDiffLo.Text = "0";
                    txbTowAltDiffHi.Text = "30000";
                    break;
                case "pbResetDuration":
                    txbDurationLo.Text = "0";
                    txbDurationHi.Text = "2880";
                    break;
            }
        }
    }
}