using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TSoar.DB;

namespace TSoar.Developer.SWLab
{
    public partial class APTSettings : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void pbRunGetTest_Click(object sender, EventArgs e)
        {
            APTSoarSetting USSettings = AccountProfile.CurrentUser.APTSUserSelectableSettings;
            lblsValue.Text = USSettings.sGetUSSetting(txbsNameGet.Text);
            lblVersion.Text = USSettings.dGetVersion().ToString();
            lblCount.Text = USSettings.APTS.Rows.Count.ToString();
        }

        protected void pbRunSetTest_Click(object sender, EventArgs e)
        {
            APTSoarSetting USSettings = AccountProfile.CurrentUser.APTSUserSelectableSettings;
            USSettings.SetUSSetting(txbsNameSet.Text, txbsValue.Text);
            AccountProfile.CurrentUser.APTSUserSelectableSettings = USSettings;
            lblCheckG.Text = txbsNameSet.Text;
            lblCheckV.Text = USSettings.sGetUSSetting(lblCheckG.Text);
        }
    }
}