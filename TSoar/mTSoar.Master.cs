using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;

namespace TSoar
{
    public partial class mTSoar : System.Web.UI.MasterPage
    {
        protected void Page_Init(object sender, EventArgs e)
        {
            this.Page.Error += new System.EventHandler(this.Page_Error);
        }

        protected void Page_Error(object sender, EventArgs e)
        {
            lblPage_Error.Visible = true;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Global.sgVersion == "00")
            {
                int rowcounter = 0;
                int colcounter = 0;
                int iStart = 0;
                int iStop = 0;
                string line;
                StreamReader SWHist = new StreamReader(HttpRuntime.AppDomainAppPath + @"Developer\SWHistoryInclude.html");
                while ((line = SWHist.ReadLine()) != null)
                {
                    if (line.IndexOf("<tr>") == 0)
                    {
                        rowcounter++;
                        colcounter = 0;
                    }
                    else
                    {
                        if (line.IndexOf("<td>") >= 0)
                        {
                            colcounter++;
                            if (colcounter > 2)
                            {
                                iStart = line.IndexOf(">") + 1;
                                iStop = line.IndexOf("<", iStart);
                                Global.sgVersion = line.Substring(iStart, iStop - iStart);
                            }
                        }
                    }
                }
                SWHist.Close();
            }
            lblVersion.Text = Global.sgVersion;
            DateTimeOffset DExp = DateTimeOffset.Now;
            lblLastLoaded.Text = DExp.ToString();
            // DExp will hold the website server's time. 
            //    For development work, that is the local development system time.
            //    For production, that is the time of the server where the website runs. Often, that is UTC.
            lblMinToTimeOut.Text = FormsAuthentication.Timeout.TotalMinutes.ToString();
        }

        protected void LoginStatus1_LoggedOut(object sender, EventArgs e)
        {
            ActivityLog.oLog(ActivityLog.enumLogTypes.UserLogout, 0, "User=" + HttpContext.Current.User.Identity.Name);
        }
    }
}