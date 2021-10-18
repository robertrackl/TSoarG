using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TSoar.AdminPages.Security
{
    public partial class ResetPwd : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void DDLUsers_SelectedIndexChanged(object sender, EventArgs e)
        {
            pnlErr.Visible = false;
            pnlNewPwd.Visible = false;
            pnlSure.Visible = false;
        }

        protected void pbResetPwd_Click(object sender, EventArgs e)
        {
            pnlErr.Visible = false;
            pnlNewPwd.Visible = false;
            lblUser.Text = DDLUsers.SelectedItem.Text;
            pnlSure.Visible = true;
        }

        protected void pbOK_Click(object sender, EventArgs e)
        {
            pnlSure.Visible = false;
            string sNewPwd;
            string sUser = DDLUsers.SelectedItem.Text;
            MembershipUser u = Membership.GetUser(sUser);
            if (u == null)
            {
                pnlErr.Visible = true;
                lblErr.Text = "User name `" + sUser + "` not found";
                return;
            }
            try
            {
                sNewPwd = u.ResetPassword();
                lblNewPwd.Text = sNewPwd;
                pnlNewPwd.Visible = true;
                ActivityLog.oLog(ActivityLog.enumLogTypes.DataUpdate, 0, "New password for user `" + sUser + "` = " + sNewPwd);
            }
            catch (MembershipPasswordException exc)
            {
                pnlErr.Visible = true;
                lblErr.Text = "Membership Password Exception says `" + exc.Message + "`";
                return;
            }
            catch (Exception exc)
            {
                pnlErr.Visible = true;
                lblErr.Text = exc.Message;
                return;
            }
        }

        protected void pbCancel_Click(object sender, EventArgs e)
        {
            pnlErr.Visible = false;
            pnlNewPwd.Visible = false;
            pnlSure.Visible = false;
        }
    }
}