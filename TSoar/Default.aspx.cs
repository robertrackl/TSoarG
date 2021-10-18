using System;
using System.Collections;
using System.Linq;
using System.Web;
using System.IO;

namespace TSoar
{
    public partial class Default : System.Web.UI.Page
    {
        private int i_Cnt { get { return (int?)ViewState["i_Cnt"] ?? 0; } set { ViewState["i_Cnt"] = value; } }
        private ArrayList sLHomeShowFiles { get { return (ArrayList)ViewState["sLHomeShowFiles"] ?? new ArrayList(); } set { ViewState["sLHomeShowFiles"] = value; } }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                AdminPages.DBMaint.DataIntegrityDataContext dc = new AdminPages.DBMaint.DataIntegrityDataContext();
                lblAdjective.Text = (from s in dc.SETTINGs where s.sSettingName == "AdjectiveToWebSiteOnHomePage" select s.sSettingValue).First();
                PrepHomeShow();
                SetImagePath();
            }
        }
        private void PrepHomeShow()
        {
            string sHomeShow = Global.scgHomeShow.Replace(@"\", @"/");
            string sDir = HttpContext.Current.Server.MapPath("~").Replace(@"\\", @"\") + Global.scgHomeShow;
            ArrayList AL = new ArrayList();
            foreach (string f in Directory.GetFiles(sDir))
            {
                AL.Add("~" + sHomeShow + Path.GetFileName(f));
            }
            sLHomeShowFiles = AL;
        }

        protected void Timer1_Tick(object sender, EventArgs e)
        {
            SetImagePath();
        }

        private void SetImagePath()
        {
            if (sLHomeShowFiles.Count > 1)
            {
                i_Cnt++;
                if (i_Cnt > sLHomeShowFiles.Count) i_Cnt = 1;
                int i_Img1 = i_Cnt - 1;
                int i_Img2 = i_Img1 + 1;
                if (i_Img2 >= sLHomeShowFiles.Count) i_Img2 = 0;
                string sUrl = (string)sLHomeShowFiles[i_Img1];
                Image1.ImageUrl = sUrl;
                sUrl = (string)sLHomeShowFiles[i_Img2];
                Image2.ImageUrl = sUrl;
            }
        }
    }
}