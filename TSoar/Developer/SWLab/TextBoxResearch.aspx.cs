using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TSoar.Developer.SWLab
{
    public partial class TextBoxResearch : System.Web.UI.Page
    {
        private DataTable dtFlyOpsInvoiceFilterSettings;

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

            if ((btn.ID == "YesButton") && (btn.CommandName == "Delete"))
            {
                //switch (OkButton.CommandArgument)
                //{
                //    case "VOID":
                //        break;
                //}
            }
        }

        private void ProcessPopupException(Global.excToPopup excu)
        {
            ButtonsClear();
            lblPopupText.Text = excu.sExcMsg();
            MPE_Show(Global.enumButtons.OkOnly);
        }
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            dtFlyOpsInvoiceFilterSettings = TSoar.DB.AccountProfile.CurrentUser.FlyOpsInvoiceFilterSettings;
            if (dtFlyOpsInvoiceFilterSettings.Rows.Count < 1)
            { // Initial content for current user
                dtFlyOpsInvoiceFilterSettings = new DataTable("dtFlyOpsInvoiceFilterSettings");
                DataColumn col;
                DataRow row;

                col = new DataColumn();
                col.DataType = System.Type.GetType("System.DateTimeOffset");
                col.ColumnName = "InvDate"; // Date of Invoice
                dtFlyOpsInvoiceFilterSettings.Columns.Add(col);

                col = new DataColumn();
                col.DataType = System.Type.GetType("System.DateTimeOffset");
                col.ColumnName = "DFrom"; // Flight operations from this date
                dtFlyOpsInvoiceFilterSettings.Columns.Add(col);

                col = new DataColumn();
                col.DataType = System.Type.GetType("System.DateTimeOffset");
                col.ColumnName = "DTo"; // FLight operations up to this date
                dtFlyOpsInvoiceFilterSettings.Columns.Add(col);

                col = new DataColumn();
                col.DataType = System.Type.GetType("System.String");
                col.ColumnName = "sMemberDisplayName";
                dtFlyOpsInvoiceFilterSettings.Columns.Add(col);

                col = new DataColumn();
                col.DataType = System.Type.GetType("System.Int32");
                col.ColumnName = "iDays2Due";
                dtFlyOpsInvoiceFilterSettings.Columns.Add(col);

                col = new DataColumn();
                col.DataType = System.Type.GetType("System.Boolean");
                col.ColumnName = "bFO2BInvoicedOnly";
                dtFlyOpsInvoiceFilterSettings.Columns.Add(col);

                row = dtFlyOpsInvoiceFilterSettings.NewRow();
                DateTimeOffset heute = DateTimeOffset.Now.LocalDateTime.Date;
                row["InvDate"] = heute;
                DateTimeOffset DFirstDayofThisMonth = heute.AddDays(-(heute.Day - 1));
                row["DFrom"] = DFirstDayofThisMonth.AddDays(-1);
                row["DTo"] = DFirstDayofThisMonth.AddMonths(-1);
                row["sMemberDisplayName"] = "ALL";
                row["iDays2Due"] = 30;
                row["bFO2BInvoicedOnly"] = true;
                dtFlyOpsInvoiceFilterSettings.Rows.Add(row);

                TSoar.DB.AccountProfile.CurrentUser.FlyOpsInvoiceFilterSettings = dtFlyOpsInvoiceFilterSettings;
            }
            if (!IsPostBack)
            {
                txbInvDate.Text = ((DateTimeOffset)dtFlyOpsInvoiceFilterSettings.Rows[0]["InvDate"]).ToString("yyyy-MM-dd");
                txbDFrom.Text = ((DateTimeOffset)dtFlyOpsInvoiceFilterSettings.Rows[0]["DFrom"]).ToString("yyyy-MM-dd");
                txbDTo.Text = ((DateTimeOffset)dtFlyOpsInvoiceFilterSettings.Rows[0]["DTo"]).ToString("yyyy-MM-dd");
                // sMemberDisplayName is handled in DDL_Member_PreRender
                txbDays2Due.Text = ((int)dtFlyOpsInvoiceFilterSettings.Rows[0]["iDays2Due"]).ToString();
                chbInvReq.Checked = (bool)dtFlyOpsInvoiceFilterSettings.Rows[0]["bFO2BInvoicedOnly"];
            }
        }

        protected void txb_TextChanged(object sender, EventArgs e)
        {
            TextBox txb = (TextBox)sender;
            DateTimeOffset DTrial = DateTimeOffset.MinValue;
            if (txb.ID != "txbDays2Due")
            {
                if (!DateTimeOffset.TryParse(txb.Text, out DTrial)) return;
            }
            switch (txb.ID)
            {
                case "txbInvDate":
                    dtFlyOpsInvoiceFilterSettings.Rows[0]["InvDate"] = DTrial;
                    break;
                case "txbDFrom":
                    dtFlyOpsInvoiceFilterSettings.Rows[0]["DFrom"] = DTrial;
                    break;
                case "txbDTo":
                    dtFlyOpsInvoiceFilterSettings.Rows[0]["DTo"] = DTrial;
                    break;
                case "txbDays2Due":
                    dtFlyOpsInvoiceFilterSettings.Rows[0]["iDays2Due"] = Int32.Parse(txb.Text);
                    break;
            }
            TSoar.DB.AccountProfile.CurrentUser.FlyOpsInvoiceFilterSettings = dtFlyOpsInvoiceFilterSettings;
        }

        #region Modal Popup Context Menu
        private void ButtonsClearCM()
        {
            pbDoInvoice.CommandArgument = "";
            pbDoInvoice.CommandName = "";
            pbDoInvoice.Visible = true;
            pbSetFirst.CommandArgument = "";
            pbSetFirst.CommandName = "";
            pbSetFirst.Visible = true;
            pbSetLast.CommandArgument = "";
            pbSetLast.CommandName = "";
            pbSetLast.Visible = true;
            pbCancel.CommandArgument = "";
            pbCancel.CommandName = "";
            pbCancel.Visible = true;
        }
        private void MPE_ShowCM()
        {
            ModPopExtCM.Show();
        }
        protected void pbCM_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            try
            {
                switch (btn.ID)
                {
                    case "pbDoInvoice":
                        //DoInvoiceLines(btn.CommandName);
                        break;
                    case "pbSetFirst":
                        break;
                    case "pbSetLast":
                        break;
                    case "pbCancel": // Do nothing
                        break;
                }
            }
            catch (Global.excToPopup exc)
            {
                ProcessPopupException(exc);
            }
        }
        #endregion
    }
}