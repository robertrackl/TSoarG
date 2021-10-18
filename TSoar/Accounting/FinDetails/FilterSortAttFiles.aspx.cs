using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TSoar.DB;

namespace TSoar.Accounting.FinDetails
{
    public partial class FilterSortAttFiles : System.Web.UI.Page
    {
        public XExpense pXe { get; private set; }

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
            try
            {
                if ((btn.ID == "YesButton") && (btn.CommandName == "Delete"))
                {
                    switch (OkButton.CommandArgument)
                    {
                        case "BadCallMode":
                            Response.Redirect("Expenses.aspx");
                            break;
                    }
                }
            }
            catch (Global.excToPopup exc)
            {
                ProcessPopupException(exc);
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
            if (!IsPostBack)
            {
                Page lastPage = (Page)Context.Handler;
                if (lastPage is ExpVendAP.XactExpense)
                {
                    pXe = ((ExpVendAP.XactExpense)lastPage).pXe;
                    Session["pXe"] = pXe;
                }
                else
                {
                    ProcessPopupException(new Global.excToPopup("Accounting.FinDetails.FilterSortAttFiles.Page_Load: Context.Handler did not provide a page of type XactExpense"));
                    return;
                }

                sf_AccountingDataContext dc = new sf_AccountingDataContext();
                var q0 = from f in dc.SFV_AttachedFiles select f;
                gvFSSA.DataSource = q0;
                gvFSSA.DataBind();
            }
        }

        protected void gvFSSA_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            // e.CommandArgument contains the index to the row where Select was clicked
            int iRow = Convert.ToInt32((string)e.CommandArgument);
            Label lblId = (Label)gvFSSA.Rows[iRow].FindControl("lblId");
            int iSF_DOC_ID = Convert.ToInt32(lblId.Text);
            pXe = (XExpense)Session["pXe"];

            // Make new DataRow
            AssistLi.SRowDataAtt row;

            using (sf_AccountingDataContext dc = new sf_AccountingDataContext())
            {
                var r = (from x in dc.SF_XACT_DOCs
                         let d = x.SF_DOC
                         let c = x.SF_ATTACHMENTCATEG
                         where d.ID == iSF_DOC_ID
                         select new { c.ID, c.sAttachmentCateg, d.DDateOfDoc, d.sPrefix, d.sName }
                        ).First();
                row.iCateg = r.ID;
                row.sCateg = r.sAttachmentCateg;
                row.DAssoc = r.DDateOfDoc;
                row.iFile = iSF_DOC_ID; // is never 0; signals that the file already exists in SF_DOCS
                row.sFile = r.sPrefix + "_" + r.sName;
                row.iThumb = 0;
                row.sThumb = "";
            }
            DataRow dr = AssistLi.makeDataRow(row, Global.enLL.AttachedFiles);
            // Replace last (`add`) row of liAttachedFiles
            int iSize = pXe.liAttachedFile.Count; // it's gonna be at least 2 because the list includes the `sum` and `add` rows.
            pXe.liAttachedFile[iSize - 1] = dr;

            // Transfer back to page XactExpense
            LeavePage(iSF_DOC_ID);
        }

        protected void pbCancel_Click(object sender, EventArgs e)
        {
            pXe = (XExpense)Session["pXe"];
            LeavePage(0);
        }

        private void LeavePage(int iuSF_DOC_ID)
        {
            Session["CallMode"] = new KeyValuePair<string, int>("RESUME", iuSF_DOC_ID);
            Server.Transfer("~/Accounting/FinDetails/ExpVendAP/XactExpense.aspx", true);
        }
    }
}