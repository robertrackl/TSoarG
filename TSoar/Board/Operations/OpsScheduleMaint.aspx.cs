using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TSoar.Board.Operations
{
    public partial class OpsSchedule : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            DB.SCUD_Multi mSCUD = new DB.SCUD_Multi();
            divHRef.InnerHtml = "<h4>Click <a href=" + (char)34 + mSCUD.GetSetting("OpsScheduleSignUpSite") + (char)34 +
                " target=" + (char)34 + "_blank" + (char)34 + "> here </a> to open PSSA's SignUpGenius.</h4>";
        }
    }
}