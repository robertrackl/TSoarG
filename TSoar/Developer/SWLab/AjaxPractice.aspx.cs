using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TSoar.Developer.SWLab
{
    public partial class AjaxPractice : System.Web.UI.Page
    {
        #region ViewState
        private string st { get { return (string)ViewState["st"] ?? "}"; } set { ViewState["st"] = value; } }
        private char ct { get { return Getchar("ct"); } set { ViewState["ct"] = value; } }
        private char Getchar(string suc)
        {
            if (ViewState[suc] == null)
            {
                return '}';
            }
            else
            {
                return (char)ViewState[suc];
            }
        }
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            OriginalTime.Text = DateTime.Now.ToLongTimeString();
            UPButtonsClear();
            lblUPPopupText.Text = "Test Starting . . .";
            UPTimer1.Enabled = true;
            UPMPE_Show(Global.enumButtons.OkOnly);
        }

        //protected void pbStart_Click(object sender, EventArgs e)
        //{
        //    UPButtonsClear();
        //    lblUPPopupText.Text = "Test Starting . . .";
        //    UPTimer1.Enabled = true;
        //    UPMPE_Show(Global.enumButtons.OkOnly);
        //}

        protected void UPTimer1_Tick(object sender, EventArgs e)
        {
            UPTimer1.Enabled = false;
            lblAlpha.Text = st;
            lblCurrTime.Text = DateTime.Now.ToLongTimeString();
            WasteSomeTime(500);
            UPTimer2.Enabled = true;
        }

        private void WasteSomeTime(int iuSms)
        {
            System.Threading.Thread.Sleep(iuSms);
        }

        protected void UPTimer2_Tick(object sender, EventArgs e)
        {
            UPTimer2.Enabled = false;
            int it = (int)ct;
            it++;
            if (it % 32 == 0)
            {
                it = 32;
                st = "";
            }
            ct = (char)it;
            st += ct.ToString();
            if (it == 42)
            {
                Response.Redirect("SWLab.aspx");
            }
            UPTimer1.Enabled = true;
        }

        #region Modal Popup
        //======================
        private void UPButtonsClear()
        {
            OkUPButton.CommandArgument = "";
            OkUPButton.CommandName = "";
        }
        private void UPMPE_Show(Global.enumButtons eubtns)
        {
            OkUPButton.CssClass = "displayNone";
            switch (eubtns)
            {
                case Global.enumButtons.OkOnly:
                    OkUPButton.CssClass = "displayUnset";
                    break;
            }
            UPModalPopExt.Show();
        }
        protected void UPButton_Click(object sender, EventArgs e)
        {
            UPTimer2.Enabled = false;
            UPTimer1.Enabled = false;
            Response.Redirect("SWLab.aspx");
        }

        #endregion
    }
}