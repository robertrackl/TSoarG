using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TSoar.MemberPages
{
    public partial class ChangePassword : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void ChangePassword1_ChangedPassword(object sender, EventArgs e)
        {
            ActivityLog.oLog(ActivityLog.enumLogTypes.Security, 0, "User `" + HttpContext.Current.User.Identity.Name + "` changed password");
        }
    }
}