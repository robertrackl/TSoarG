using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TSoar.DB;
using TSoar.Accounting;

namespace TSoar.Statistician
{
    public partial class TIRewardsFilter : System.Web.UI.Page
    {
        private DataTable dtFilters = new DataTable("ExpFilterSettings", "TSoar.Statistician.TIRewardsFilter");

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
            MPE_Show(Global.enumButtons.OkOnly);
        }
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                bCheck4dtFilterReset(dtFilters, lblVersionUpdate);
                DateTimeOffset D = DateTimeOffset.Now;
                txbAsOfDate.Text = D.Year.ToString() + "-" + (D.Month + 100).ToString().Substring(1) + "-" + (D.Day + 100).ToString().Substring(1);
            }
        }

        private void bCheck4dtFilterReset(DataTable dtu, Label lblu)
        {
            bool bNewVersion = false;
            dtu = AssistLi.dtGetRewardsFilterSettings(out bNewVersion);
            lblu.Visible = bNewVersion;
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                dtFilters = AccountProfile.CurrentUser.RewardsFilterSettings;
                InitControls();
            }
        }
        private void InitControls()
        {
            // Initialize the controls based upon the previously saved filter settings
            int ix = (int)Global.egRewardsFilters.EnableFilteringOverall;
            chbEnableFiltering.Checked = (bool)dtFilters.Rows[ix][(int)Global.egRewardsFilterProps.Enabled];

            ix = (int)Global.egRewardsFilters.Member;
            chbMember.Checked= (bool)dtFilters.Rows[ix][(int)Global.egRewardsFilterProps.Enabled];
            sf_AccountingDataContext dc = new sf_AccountingDataContext();
            DateTimeOffset D = DateTimeOffset.Parse(txbAsOfDate.Text);
            var q0 = from r in dc.TNPF_EligibleRewardMembers(D, true) orderby r.sDisplayName select r;
            DDLMember.DataSource = q0;
            DDLMember.DataValueField = "ID";
            DDLMember.DataTextField = "sDisplayName";
            DDLMember.DataBind();
            string sMemberName = (string)dtFilters.Rows[ix][(int)Global.egRewardsFilterProps.List];
            DDLMember.SelectedValue=dtFilters.Rows[ix][(int)Global.egRewardsFilterProps.Integ32].ToString();

            ix = (int)Global.egRewardsFilters.EarnClaimDate;
            chbEarnDate.Checked = (bool)dtFilters.Rows[ix][(int)Global.egRewardsFilterProps.Enabled];
            // Special use of 'List' for Earn/Claim date:
            string[] sa1 = ((string)dtFilters.Rows[ix][(int)Global.egRewardsFilterProps.List]).Split(',');
            string ss = sa1[0].Replace("'", "").Trim();
            string[] sa2 = ss.Split(' ');
            txbEarnDateLo.Text = sa2[0];
            ss = sa1[1].Replace("'", "").Trim();
            sa2 = ss.Split(' ');
            txbEarnDateHi.Text = sa2[0];

            ix = (int)Global.egRewardsFilters.ShowExpired;
            chbShowExpired.Checked = (bool)dtFilters.Rows[ix][(int)Global.egRewardsFilterProps.Enabled];

            ix = (int)Global.egRewardsFilters.EarnClaimCode;
            chbECCode.Checked = (bool)dtFilters.Rows[ix][(int)Global.egRewardsFilterProps.Enabled];
            DDLECCode.SelectedValue= (string)dtFilters.Rows[ix][(int)Global.egRewardsFilterProps.List];

            ix = (int)Global.egRewardsFilters.LimitAtTopBottom;
            chbN.Checked = (bool)dtFilters.Rows[ix][(int)Global.egRewardsFilterProps.Enabled];
            rblN.SelectedValue=((int)dtFilters.Rows[ix][(int)Global.egRewardsFilterProps.Integ32]).ToString(); // either 1 or 2
            ix = (int)Global.egRewardsFilters.LimitRowCount;
            txbN.Text= ((int)dtFilters.Rows[ix][(int)Global.egRewardsFilterProps.Integ32]).ToString(); // between 1 and 200
        }

        protected void pbFilterOK_Click(object sender, EventArgs e)
        {
            dtFilters = AccountProfile.CurrentUser.RewardsFilterSettings;

            int ix = (int)Global.egRewardsFilters.EnableFilteringOverall;
            dtFilters.Rows[ix][(int)Global.egRewardsFilterProps.Enabled] = chbEnableFiltering.Checked;

            ix = (int)Global.egRewardsFilters.Member;
            if (DDLMember.SelectedValue.Length < 1)
            {
                chbMember.Checked = false;
                dtFilters.Rows[ix][(int)Global.egRewardsFilterProps.Enabled] = false;
            }
            else
            {
                dtFilters.Rows[ix][(int)Global.egRewardsFilterProps.Enabled] = chbMember.Checked;
                dtFilters.Rows[ix][(int)Global.egRewardsFilterProps.Integ32] = Int32.Parse(DDLMember.SelectedValue);
            }

            ix = (int)Global.egRewardsFilters.EarnClaimDate;
            dtFilters.Rows[ix][(int)Global.egRewardsFilterProps.Enabled] = chbEarnDate.Checked;
            dtFilters.Rows[ix][(int)Global.egRewardsFilterProps.List] =
                txbEarnDateLo.Text + "," +  txbEarnDateHi.Text;

            ix = (int)Global.egRewardsFilters.ShowExpired;
            dtFilters.Rows[ix][(int)Global.egRewardsFilterProps.Enabled] = chbShowExpired.Checked;

            ix = (int)Global.egRewardsFilters.EarnClaimCode;
            dtFilters.Rows[ix][(int)Global.egRewardsFilterProps.Enabled] = chbECCode.Checked;
            dtFilters.Rows[ix][(int)Global.egRewardsFilterProps.List] = DDLECCode.Text.Substring(0, 1);

            ix = (int)Global.egRewardsFilters.LimitAtTopBottom;
            dtFilters.Rows[ix][(int)Global.egRewardsFilterProps.Enabled] = chbN.Checked;
            dtFilters.Rows[ix][(int)Global.egRewardsFilterProps.Integ32] = Int32.Parse(rblN.SelectedValue);
            ix = (int)Global.egRewardsFilters.LimitRowCount;
            dtFilters.Rows[ix][(int)Global.egRewardsFilterProps.Integ32] = Int32.Parse(txbN.Text);

            AccountProfile.CurrentUser.RewardsFilterSettings = dtFilters; // Save filter setting for current user
            Server.Transfer("~/Statistician/TIRewardsEdit.aspx", true);
        }

        protected void pbExpCancel_Click(object sender, EventArgs e)
        {
            Server.Transfer("~/Statistician/TIRewardsEdit.aspx", true);
        }
    }
}