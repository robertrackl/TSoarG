using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Configuration;
using System.Data.SqlClient;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TSoar.DB;

namespace TSoar.Accounting.AdminFin
{
    public partial class AdminFin : System.Web.UI.Page
    {
        private string sTextBeforeUpdate { get { return (string)ViewState["sKey"] ?? ""; } set { ViewState["sKey"] = value; } }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                gvFiscPer_Bind(-2);
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
            Button btn = (Button)sender;
            //Global.oLog("Modal popup dismissed with " + btn.ID + ", CommandName=" + btn.CommandName);
            try
            {
                if ((btn.ID == "YesButton") && (btn.CommandName == "Delete"))
                {
                    switch (OkButton.CommandArgument)
                    {
                        case "FiscalPeriod":
                            // Delete the Fiscal Period
                            SCUD_Multi mCRUD = new SCUD_Multi();
                            mCRUD.DeleteOne(Global.enugInfoType.SF_FiscalPeriods, btn.CommandArgument);
                            gvFiscPer_Bind(-2);
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

        protected void gvFiscPer_RowEditing(object sender, GridViewEditEventArgs e)
        {
            sTextBeforeUpdate = ((Label)((GridView)sender).Rows[e.NewEditIndex].FindControl("lblFiscPer")).Text;
            gvFiscPer_Bind(e.NewEditIndex);
        }

        protected void gvFiscPer_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if ((e.Row.RowState & DataControlRowState.Edit) > 0)
                {
                    TextBox txbB = (TextBox)e.Row.FindControl("txbDStart");
                    string sD = DataBinder.Eval(e.Row.DataItem, "DStart").ToString();
                    if (sD.Length > 0)
                    {
                        sD = sD.Replace('/', '-');
                        sD = sD.Substring(0, 10);
                        txbB.Text = sD;
                    }
                    else
                    {
                        txbB.Text = sD;
                    }
                    TextBox txbE = (TextBox)e.Row.FindControl("txbDEnd");
                    sD = DataBinder.Eval(e.Row.DataItem, "DEnd").ToString();
                    if (sD.Length > 0)
                    {
                        sD = sD.Replace('/', '-');
                        sD = sD.Substring(0, 10);
                        txbE.Text = sD;
                    }
                    else
                    {
                        txbE.Text = sD;
                    }
                }
            }
        }

        protected void gvFiscPer_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvFiscPer_Bind(-2);
        }

        protected void gvFiscPer_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            ButtonsClear();
            OkButton.CommandArgument = "FiscalPeriod";
            YesButton.CommandName = "Delete";
            YesButton.CommandArgument = ((Label)((GridView)sender).Rows[e.RowIndex].FindControl("lblFiscPer")).Text;
            lblPopupText.Text = "Please confirm deletion of Fiscal Period '" + YesButton.CommandArgument + "'";
            MPE_Show(Global.enumButtons.NoYes);
            e.Cancel = true;
        }

        protected void gvFiscPer_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            try
            {
                SCUD_Multi mCRUD = new SCUD_Multi();
                string[] sa = new string[3];
                GridViewRow row = gvFiscPer.Rows[e.RowIndex];
                TextBox txbDescr = (TextBox)row.FindControl("txbFiscPer");
                sa[0] = Server.HtmlEncode(txbDescr.Text.Trim().Replace("'", "`"));
                TextBox txbDStart = (TextBox)row.FindControl("txbDStart");
                string sTOffset = mCRUD.GetSetting("TimeZoneOffset");
                sa[1] = txbDStart.Text + "T00:00:00" + sTOffset;
                DateTimeOffset Dst = DateTimeOffset.Parse(sa[1]);
                TextBox txbDEnd = (TextBox)row.FindControl("txbDEnd");
                sa[2] = txbDEnd.Text + "T23:59:59" + sTOffset;
                DateTimeOffset Dend = DateTimeOffset.Parse(sa[2]);
                if (Dend <= Dst)
                {
                    Global.excToPopup exc = new Global.excToPopup("End date must be later than start date");
                    ProcessPopupException(exc);
                    return;
                }
                if (e.RowIndex == gvFiscPer.Rows.Count - 1) // If last row, then we are inserting
                {
                    int iIdent = 0;
                    if (mCRUD.Exists(Global.enugInfoType.SF_FiscalPeriods, sa[0]) > 0)
                    {
                        Global.excToPopup exc = new Global.excToPopup("Fiscal Period '" + sa[0] + "' exists already");
                        ProcessPopupException(exc);
                        return;
                    }
                    mCRUD.InsertOne(Global.enugInfoType.SF_FiscalPeriods, sa, out iIdent);
                }
                else // If not last row, we are updating
                {
                    mCRUD.UpdateOne(Global.enugInfoType.SF_FiscalPeriods, sTextBeforeUpdate, sa);
                }
                gvFiscPer_Bind(-2);
            }
            catch (Exception exc)
            {
                Global.excToPopup excP = new Global.excToPopup(exc.Message);
                ProcessPopupException(excP);
            }
        }

        private void gvFiscPer_Bind(int iuEditIndex)
        {
            SCUD_Multi mCRUD = new SCUD_Multi();
            DataTable dtAll = mCRUD.GetAll(Global.enugInfoType.SF_FiscalPeriods);
            dtAll.Rows.Add(dtAll.NewRow());
            dtAll.Rows.Add(dtAll.NewRow());
            gvFiscPer.DataSource = dtAll;
            if (iuEditIndex == -2)
            {
                gvFiscPer.EditIndex = dtAll.Rows.Count - 1; // Editing last row
            }
            else
            {
                gvFiscPer.EditIndex = iuEditIndex;
            }
            gvFiscPer.DataBind();
            mCRUD = null;
            int iLast = gvFiscPer.Rows.Count - 1; // The standard editing row
            int iNextToLast = iLast - 1; // The row with the Sum

            // Sum row
            gvFiscPer.Rows[iNextToLast].Cells[0].Text = "Number of Defined Fiscal Periods:";
            gvFiscPer.Rows[iNextToLast].Cells[1].Text = (gvFiscPer.Rows.Count - 2).ToString();
            for (int icol = 2; icol < gvFiscPer.Columns.Count; icol++) 
            {
                gvFiscPer.Rows[iNextToLast].Cells[icol].Visible = false;
            }
            gvFiscPer.Rows[iNextToLast].Cells[0].HorizontalAlign = HorizontalAlign.Right;

            // Edit row (= last row)
            // Last row has an Add button
            ImageButton pb = (ImageButton)gvFiscPer.Rows[iLast].Cells[3].Controls[0];
            pb.ImageUrl = "~/i/YellowAddButton.jpg";

            //  Last row only has edit button
            gvFiscPer.Rows[iLast].Cells[gvFiscPer.Columns.Count - 1].Visible = false;

            // Execute this code only when a row is being edited
            if (gvFiscPer.EditIndex > -1)
            {
                if (gvFiscPer.EditIndex == iLast)
                {
                    // No Cancel button in the last row
                    ImageButton pbC = (ImageButton)gvFiscPer.Rows[gvFiscPer.EditIndex].Cells[3].Controls[2];
                    pbC.Visible = false;
                }
                else
                {
                    // If we are editing a row other than the last, only the Update and Cancel buttons are allowed:
                    for (int iRow = 0; iRow <= iLast; iRow++)
                    {
                        if (iRow != gvFiscPer.EditIndex)
                        {
                            ((ImageButton)gvFiscPer.Rows[iRow].Cells[3].Controls[0]).Visible = false;
                            ((ImageButton)gvFiscPer.Rows[iRow].Cells[4].Controls[0]).Visible = false;
                        }
                    }
                }
            }

        }
    }
}