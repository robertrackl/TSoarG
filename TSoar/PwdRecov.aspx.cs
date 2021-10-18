using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TSoar
{
    public partial class PwdRecov : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void PasswordRecovery1_SendMailError(object sender, SendMailErrorEventArgs e)
        {
            tbMailError.Visible = true;
            tbMailError.Text = e.Exception.ToString();
            e.Handled = true;
            PasswordRecovery1.SuccessText = "Could not send email with password due to error";
        }

        protected void PasswordRecovery1_SendingMail(object sender, MailMessageEventArgs e)
        {
            e.Message.IsBodyHtml = false;
            string s_s = "New info you requested";
            e.Message.Subject = s_s;
            PasswordRecovery1.SuccessText = "An email message with password with subject '" + s_s + "' has been sent to '" + e.Message.To + "'";
            tbMailError.Text = "";
            tbMailError.Visible = false;
        }
    }
}