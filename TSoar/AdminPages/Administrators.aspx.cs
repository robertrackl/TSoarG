using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Configuration;
using System.Data.SqlClient;
using TSoar.DB;

namespace TSoar.AdminPages
{
    public partial class Administrators : System.Web.UI.Page
    {
        SCUD_Multi mCRUD = new SCUD_Multi();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string sPageSize = mCRUD.GetSetting("PageSizeActivityLog");
                int iPageSize = Int32.Parse(sPageSize);
                gvActLog.PageSize = iPageSize;
                lblPageSize.Text = sPageSize;
                lblNumEntries.Text = GetNumEntries().ToString();
                DisplayInGrid();
            }
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
        }

        private void ProcessPopupException(Global.excToPopup excu)
        {
            ButtonsClear();
            lblPopupText.Text = excu.sExcMsg();
            MPE_Show(Global.enumButtons.OkOnly);
        }
        #endregion

        private void DisplayInGrid()
        {
            using (DataTable dt = new DataTable())
            {
                using (SqlConnection SqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlConn"].ToString()))
                {
                    using (SqlCommand cmd = new SqlCommand("SELECT TOP 500 * FROM TNPV_ActivityLog Order By ID DESC"))
                    {
                        using (SqlDataAdapter sda = new SqlDataAdapter())
                        {
                            cmd.CommandType = CommandType.Text;
                            cmd.Connection = SqlConn;
                            sda.SelectCommand = cmd;
                            sda.Fill(dt);
                        }
                    }
                }
                gvActLog.DataSource = dt;
                gvActLog.DataBind();
            }
        }

        private int GetNumEntries()
        {
            using (SqlConnection SqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlConn"].ToString()))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM ACTIVITYLOG"))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = SqlConn;
                    SqlConn.Open();
                    return (int)cmd.ExecuteScalar();
                }
            }

        }

        protected void gvActLog_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvActLog.PageIndex = e.NewPageIndex;
            DisplayInGrid();
        }

        protected void pbUnloadAppDomain_Click(object sender, EventArgs e)
        {
            System.Web.HttpRuntime.UnloadAppDomain();
        }

        //protected void pbSimulateAppStart_Click(object sender, EventArgs e)
        //{
        //    ActivityLog.oLog(ActivityLog.enumLogTypes.SimulAppStart, 0, Global.sgcAppName + " " + Global.sgMigrationLevelDevIntProd);
        //    Global.AppStart();
        //}

        protected void pbShowGlobals_Click(object sender, EventArgs e)
        {
            lblPopupText.Text = "Global Variables";
            lblsgcAppName.Text = Global.sgcAppName;
            lblscgHomeShow.Text = Global.scgHomeShow;
            lblscgRelPathUpload.Text = Global.scgRelPathUpload;
            lblDevIntProd.Text = Global.sgMigrationLevelDevIntProd;
            lblsgVersion.Text = Global.sgVersion;
            lblDTO_NotStarted.Text = CustFmt.sFmtDate(Global.DTO_NotStarted, CustFmt.enDFmt.DateAndTimeMinOffset);
            lblDTO_NotCompleted.Text = CustFmt.sFmtDate(Global.DTO_NotCompleted, CustFmt.enDFmt.DateAndTimeMinOffset);
            lblDTO_EqAgEarliest.Text = CustFmt.sFmtDate(Global.DTO_EqAgEarliest, CustFmt.enDFmt.DateAndTimeMinOffset);
            lblDTO_EqAgLatest.Text = CustFmt.sFmtDate(Global.DTO_EqAgLatest, CustFmt.enDFmt.DateAndTimeMinOffset);
            lblSETTINGS_Version.Text = Global.dgcVersionUserSelectableSettingsDataTable.ToString();
            MPE_Show(Global.enumButtons.OkOnly);
        }
    }
}