using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TSoar.DB;

namespace TSoar.Equipment.EquipAging
{
    public partial class EqAgingIntro : System.Web.UI.Page
    {
        private int iHelpPage { get { return iGet("iHelpPage"); } set { Session["iHelpPage"] = value; } }
        private int iGet(string suN)
        {
            if (Session[suN] == null)
            {
                return 0;
            }
            else
            {
                return (int)Session[suN];
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                chbShowEqComp.Checked = AccountProfile.CurrentUser.bShowEqComponentsLinkBeginEndTimes;
                chbShowTimes.Checked = AccountProfile.CurrentUser.bShowEqAgItemsStartEndTimes;
                chbShowEqOpDataTimes.Checked = AccountProfile.CurrentUser.bShowEquipMgtOpDataStartEndTimes;
                chbShowEqActionItemsTimes.Checked = AccountProfile.CurrentUser.bShowEqActItemsStartEndTimes;
                TabContainer1.ActiveTabIndex = iHelpPage;
            }
        }

        protected void chbShowTimes_CheckedChanged(object sender, EventArgs e)
        {
            AccountProfile.CurrentUser.bShowEqAgItemsStartEndTimes = ((CheckBox)sender).Checked;
        }

        protected void chbShowEqOpDataTimes_CheckedChanged(object sender, EventArgs e)
        {
            AccountProfile.CurrentUser.bShowEquipMgtOpDataStartEndTimes = ((CheckBox)sender).Checked;
        }

        protected void chbShowEqActionItemsTimes_CheckedChanged(object sender, EventArgs e)
        {
            AccountProfile.CurrentUser.bShowEqActItemsStartEndTimes = ((CheckBox)sender).Checked;
        }

        protected void chbShowEqComp_CheckedChanged(object sender, EventArgs e)
        {
            AccountProfile.CurrentUser.bShowEqComponentsLinkBeginEndTimes = ((CheckBox)sender).Checked;
        }

        protected void TabContainer1_ActiveTabChanged(object sender, EventArgs e)
        {
            AjaxControlToolkit.TabContainer TabContainer1 = (AjaxControlToolkit.TabContainer)sender;
            iHelpPage = TabContainer1.ActiveTabIndex;
        }
    }
}