using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TSoar.PublicPages
{
    public partial class AcceptUsePol : System.Web.UI.Page
    {
        private string sWUsername { get { return (string)ViewState["sWUserName"] ?? ""; } set { ViewState["sWUserName"] = value; } }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["AUP Prompt"] != null)
                {
                    sWUsername = (string)Session["AUP Prompt"];
                    Session["AUP Prompt"] = null;
                    lblAgree.Visible = true;
                    pbAgree.Visible = true;
                    pbNotAgree.Visible = true;
                }
            }
        }

        protected void pbAgree_Click(object sender, EventArgs e)
        {
            ActivityLog.oLog(ActivityLog.enumLogTypes.Security, 0, "User " + sWUsername + " clicked on `I agree with AUP`");
            using (SqlConnection SqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlConn"].ToString()))
            {
                string sCmd = "UPDATE PEOPLE SET DAcceptedAUP='" + CustFmt.sFmtDate(DateTimeOffset.Now, CustFmt.enDFmt.DateAndTimeSec) +
                    "' WHERE sUserName='" + sWUsername + "'";
                using (SqlCommand cmd = new SqlCommand(sCmd))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = SqlConn;
                    cmd.CommandTimeout = 600;
                    SqlConn.Open();
                    cmd.ExecuteNonQuery();
                    SqlConn.Close();
                }
            }
            FormsAuthentication.RedirectToLoginPage();
        }

        protected void pbNotAgree_Click(object sender, EventArgs e)
        {
            ActivityLog.oLog(ActivityLog.enumLogTypes.Security, 0, "User " + sWUsername + " clicked on `I do not agree with AUP`");
            FormsAuthentication.SignOut();
            FormsAuthentication.RedirectToLoginPage();
        }
    }
}