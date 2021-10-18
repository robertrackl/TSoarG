using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;
using System.Data;
using System.Configuration;
using System.Data.SqlClient;

namespace TSoar
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            SetFocus(Login2.FindControl("UserName"));
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
            Button btn = (Button)sender;
            //Global.oLog("Modal popup dismissed with " + btn.ID + ", CommandName=" + btn.CommandName);
            //if (btn.ID == "YesButton")
            //{
            //    switch (btn.CommandName)
            //    {
            //    }
            //}
        }

        private void ProcessPopupException(Global.excToPopup excu)
        {
            ButtonsClear();
            lblPopupText.Text = excu.sExcMsg();
            MPE_Show(Global.enumButtons.OkOnly);
        }
        #endregion

        protected void Login2_LoggedIn(object sender, EventArgs e)
        {
            System.Web.UI.WebControls.Login L = (System.Web.UI.WebControls.Login)sender;
            string sWUsername = L.UserName;
            ActivityLog.oLog(ActivityLog.enumLogTypes.UserLogin, 0, "User=" + sWUsername);
            using (DataTable dt = new DataTable())
            {
                using (SqlConnection SqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlConn"].ToString()))
                {
                    using (SqlCommand cmd = new SqlCommand("SELECT DAcceptedAUP FROM PEOPLE WHERE sUserName='" + sWUsername + "'"))
                    {
                        using (SqlDataAdapter sda = new SqlDataAdapter())
                        {
                            cmd.CommandType = CommandType.Text;
                            cmd.Connection = SqlConn;
                            sda.SelectCommand = cmd;
                            sda.Fill(dt);
                            if (dt.Rows.Count != 1)
                            {
                                ProcessPopupException(new Global.excToPopup("User name `" + sWUsername +
                                    "` either was not found or found more than once. This is a software error; please contact technical support."));
                                return;
                            }
                            else
                            {
                                DateTimeOffset DAcceptedAUP = (DateTimeOffset)dt.Rows[0][0];
                                if (DAcceptedAUP < DateTimeOffset.Now.AddYears(-1))
                                {
                                    // The last time this user accepted the Acceptable Use Policy is more than a year ago
                                    Session["AUP Prompt"] = sWUsername;
                                    Page.Response.Redirect("~/PublicPages/AcceptUsePol.aspx", true);
                                }
                            }
                        }
                    }
                }
            }
        }

        protected void Login2_LoginError(object sender, EventArgs e)
        {
            ActivityLog.oLog(ActivityLog.enumLogTypes.FailedLogin, 0, "");
        }
    }
}