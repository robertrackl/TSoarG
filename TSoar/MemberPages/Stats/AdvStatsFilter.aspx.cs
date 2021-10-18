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
    public partial class AdvStatsFilter : System.Web.UI.Page
    {
        private DataTable dtFilters = new DataTable("OpsAdvFilterSetting", "TSoar.MemberPages.Stats");
        private PopulateTrVwFilters PopulTree = new PopulateTrVwFilters();
        // bFilterChanged is left over from an attempt to keep track of whether any filter settings were changed.
        //    So far, the attempt is unsuccessful because, mysteriously, txbTakeoffDateHi_TextChanged() sometimes runs between
        //    Page_Load and Page_PreRender which sets bFilterChanged to true when it is supposed to stay false.
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
            PopulTree.bCheck4dtAdvFilterReset(dtFilters, lblVersionUpdate);
            //dtFilters = AccountProfile.CurrentUser.OpsAdvFilterSetting;
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
            dtFilters = AccountProfile.CurrentUser.OpsAdvFilterSetting;
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
            txbTakeoffDateHi.Text = sa[1].Replace("'", "").Replace(" ","");

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
            if (txbuList.Text.Length < 1) { txbuList.Text = "All"; }
            if (txbuList.Text == "All")
            {
                chbuIN.Checked = true;
                chbuIN.Enabled = false;
            }
            else
            {
                chbuIN.Checked = (bool)dtFilters.Rows[iux][(int)Global.egAdvFilterProps.INorEX];
                chbuIN.Enabled = true;
            }
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

        #region DDL_SelectedIndexChanged
        private void DDL_SelectedIndexChanged(DropDownList uddl, TextBox utxb, CheckBox chbuIN)
        {
            string sPunct = "";
            if (utxb.Text == "All")
            {
                utxb.Text = "";
            }
            string[] sax = utxb.Text.Split(',');
            if (!(sax.Length < 2 && sax[0].Length < 1))
            {
                foreach (string ss in sax)
                {
                    if (ss.Replace("'","") == uddl.SelectedItem.Text) return;
                }
                sPunct = ",";
            }
            string s1 = uddl.SelectedItem.Text.Trim();
            if (s1.Length > 0)
            {
                utxb.Text += sPunct + "'" + s1 + "'";
            }
            if (utxb.Text.Length < 1)
            {
                utxb.Text = "All";
                chbuIN.Enabled = false;
            }
            else
            {
                chbuIN.Enabled = true;
            }
            uddl.SelectedIndex = 0;
            bFilterChanged = true;
        }
        protected void DDLAviator_SelectedIndexChanged(object sender, EventArgs e)
        {
            DDL_SelectedIndexChanged(DDLAviator, txbDDLAviator, chbAviatorIN);
        }

        protected void DDLAvRole_SelectedIndexChanged(object sender, EventArgs e)
        {
            DDL_SelectedIndexChanged(DDLAvRole, txbDDLAvRole, chbAvRoleIN);
        }

        protected void DDLTOLocation_SelectedIndexChanged(object sender, EventArgs e)
        {
            DDL_SelectedIndexChanged(DDLTOLocation, txbDDLTOLocation, chbTOLocationIN);
        }

        protected void DDLLDLocation_SelectedIndexChanged(object sender, EventArgs e)
        {
            DDL_SelectedIndexChanged(DDLLDLocation, txbDDLLDLocation, chbLDLocationIN);
        }

        protected void DDLChargeCode_SelectedIndexChanged(object sender, EventArgs e)
        {
            DDL_SelectedIndexChanged(DDLChargeCode, txbDDLChargeCode, chbChargeCodeIN);
        }

        protected void DDLLaunchMethod_SelectedIndexChanged(object sender, EventArgs e)
        {
            DDL_SelectedIndexChanged(DDLLaunchMethod, txbDDLLaunchMethod, chbLaunchMethodIN);
        }

        protected void DDLSpecialOps_SelectedIndexChanged(object sender, EventArgs e)
        {
            DDL_SelectedIndexChanged(DDLSpecialOps, txbDDLSpecialOps, chbSpecialOpsIN);
        }

        protected void DDLEquipment_SelectedIndexChanged(object sender, EventArgs e)
        {
            DDL_SelectedIndexChanged(DDLEquipment, txbDDLEquipment, chbEquipmentIN);
        }

        protected void DDLEquipmentRole_SelectedIndexChanged(object sender, EventArgs e)
        {
            DDL_SelectedIndexChanged(DDLEquipmentRole, txbDDLEquipmentRole, chbEquipmentRoleIN);
        }

        protected void DDLEquipmentType_SelectedIndexChanged(object sender, EventArgs e)
        {
            DDL_SelectedIndexChanged(DDLEquipmentType, txbDDLEquipmentType, chbEquipmentTypeIN);
        }
        #endregion
        protected void pbOpOK_Click(object sender, EventArgs e)
        {
            if (!(bFilterListCheck("Aviator", chbAviator, chbAviatorIN, txbDDLAviator) &&
                bFilterListCheck("Aviator Role", chbAvRole, chbAvRoleIN, txbDDLAvRole) &&
                bFilterListCheck("Takeoff Location", chbTOLocation, chbTOLocationIN, txbDDLTOLocation) &&
                bFilterListCheck("Landing Location", chbLDLocation, chbLDLocationIN, txbDDLLDLocation) &&
                bFilterListCheck("Charge Code", chbChargeCode, chbChargeCodeIN, txbDDLChargeCode) &&
                bFilterListCheck("Launch Methods", chbLaunchMethod, chbLaunchMethodIN, txbDDLLaunchMethod) &&
                bFilterListCheck("Special Op Types", chbSpecialOps, chbSpecialOpsIN, txbDDLSpecialOps) &&
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
            dtFilters = AccountProfile.CurrentUser.OpsAdvFilterSetting;
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

            AccountProfile.CurrentUser.OpsAdvFilterSetting = dtFilters; // Save filter settings for current user
            bFilterChanged = false;
            AccountProfile.CurrentUser.OpsFilterSettingSelection = "ADVANCED";
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

        #region Reset List(s) to "All", and range filters to wide open
        protected void pbReset_Click(object sender, EventArgs e)
        {
            Button pb = (Button)sender;
            bFilterChanged = true;
            switch (pb.ID)
            {
                case "pbResetAv":
                    txbDDLAviator.Text = "All";
                    chbAviatorIN.Checked = true;
                    chbAviatorIN.Enabled = false;
                    break;
                case "pbResetAvRole":
                    txbDDLAvRole.Text = "All";
                    chbAvRoleIN.Checked = true;
                    chbAvRoleIN.Enabled = false;
                    break;
                case "pbResetTOLocation":
                    txbDDLTOLocation.Text = "All";
                    chbTOLocationIN.Checked = true;
                    chbTOLocationIN.Enabled = false;
                    break;
                case "pbResetLDLocation":
                    txbDDLLDLocation.Text = "All";
                    chbLDLocationIN.Checked = true;
                    chbLDLocationIN.Enabled = false;
                    break;
                case "pbResetChargeCode":
                    txbDDLChargeCode.Text = "All";
                    chbChargeCodeIN.Checked = true;
                    chbChargeCodeIN.Enabled = false;
                    break;
                case "pbResetLaunchMethod":
                    txbDDLLaunchMethod.Text = "All";
                    chbLaunchMethodIN.Checked = true;
                    chbLaunchMethodIN.Enabled = false;
                    break;
                case "pbResetSpecialOps":
                    txbDDLSpecialOps.Text = "All";
                    chbSpecialOpsIN.Checked = true;
                    chbSpecialOpsIN.Enabled = false;
                    break;
                case "pbResetEquipment":
                    txbDDLEquipment.Text = "All";
                    chbEquipmentIN.Checked = true;
                    chbEquipmentIN.Enabled = false;
                    break;
                case "pbResetEquipmentRole":
                    txbDDLEquipmentRole.Text = "All";
                    chbEquipmentRoleIN.Checked = true;
                    chbEquipmentRoleIN.Enabled = false;
                    break;
                case "pbResetEquipmentType":
                    txbDDLEquipmentType.Text = "All";
                    chbEquipmentTypeIN.Checked = true;
                    chbEquipmentTypeIN.Enabled = false;
                    break;
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
        #endregion

        protected void txb_TextChanged(object sender, EventArgs e)
        {
            //ActivityLog.oDiag("Debug", "StatsFilter.aspx.cs.txb_TextChanged was called");
            bFilterChanged = true;
        }

        protected void chb_CheckedChanged(object sender, EventArgs e)
        {
            bFilterChanged = true;
        }
    }
}