using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TSoar.DB;

namespace TSoar.Accounting.FinDetails.ExpVendAP
{
    public partial class Vendors : System.Web.UI.Page
    {

        #region Boolean Variables
        private bool bEditExistingRow
        {
            get { return GetbEditExistingRow("bEditExistingRow"); }
            set { ViewState["bEditExistingRow"] = value; }
        }
        private bool GetbEditExistingRow(string suEditExistingRow)
        {
            if (ViewState[suEditExistingRow] == null)
            {
                return false;
            }
            else
            {
                return (bool)ViewState[suEditExistingRow];
            }
        }
        #endregion
        #region Integer Variables
        private int iNgvRows { get { return iGetNgvRows("iNgvRows"); } set { ViewState["iNgvRows"] = value; } } // number of equipment components + 1
        // iEdRow indexes the row of interest (the one being edited) starting at 0 and ignoring paging
        private int iEdRow { get { return iGetNgvRows("iEdRow"); } set { ViewState["iEdRow"] = value; } }
        private int iIndexOfLastPage { get { return iGetNgvRows("iIndexOfLastPage"); } set { ViewState["iIndexOfLastPage"] = value; } }
        private int iGetNgvRows(string suN)
        {
            if (ViewState[suN] == null)
            {
                if (suN == "iEdRow") return -1;
                return 0;
            }
            else
            {
                return (int)ViewState[suN];
            }
        }
        #endregion

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
            try
            {
                if ((btn.ID == "YesButton") && (btn.CommandName == "Delete"))
                {
                    switch (OkButton.CommandArgument)
                    {
                        case "Vendor":
                            // Delete the Vendor
                            sf_AccountingDataContext dc = new sf_AccountingDataContext();
                            var r = (from v in dc.SF_VENDORs where v.sVendorName == btn.CommandArgument select v).First();
                            dc.SF_VENDORs.DeleteOnSubmit(r);
                            try
                            {
                                dc.SubmitChanges();
                            }
                            catch (Exception exc)
                            {
                                Global.excToPopup ex = new Global.excToPopup(exc.Message);
                                ProcessPopupException(ex);
                            }
                            finally
                            {
                                bEditExistingRow = false;
                                gvVendors.EditIndex = -1;
                                iEdRow = -1;
                                FillDataTable();
                            }
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
                FillDataTable();
            }
        }
        private void FillDataTable()
        {
            List<DataRow> liVendors = AssistLi.Init(Global.enLL.Vendors);
            Session["liVendors"] = liVendors;
            GridView gv = gvVendors;
            iNgvRows = liVendors.Count; // number of rows in gv
            if (iEdRow == -1)
            {
                iEdRow = iNgvRows - 1;
            }
            //gvVendors.PageSize = Int32.Parse(mCRUD.GetSetting("PageSizeVendors")); is a constant 25 rows per page
            int iNewEditIndex = iEdRow % gv.PageSize;
            int iIndexOfEditPage = iEdRow / gv.PageSize;
            iIndexOfLastPage = iNgvRows / gv.PageSize;
            if ((iNgvRows % gv.PageSize) == 0)
            {
                iIndexOfLastPage--;
            }
            // Is the row being edited in the current page?
            if (gv.PageIndex == iIndexOfEditPage)
            {
                GridViewEditEventArgs gvee = new GridViewEditEventArgs(iNewEditIndex);
                gvVendors_RowEditing(null, gvee);
            }
            else
            {
                gv.EditIndex = -1;
                bEditExistingRow = false;
                dt_BindToGV(liVendors.CopyToDataTable());
            }
        }

        protected void gvVendors_RowEditing(object sender, GridViewEditEventArgs e)
        {
            gvVendors.EditIndex = e.NewEditIndex;
            iEdRow = gvVendors.PageSize * gvVendors.PageIndex + gvVendors.EditIndex;
            List<DataRow> li = (List<DataRow>)Session["liVendors"];
            bEditExistingRow = false;
            if (iEdRow < iNgvRows - 1)
            {
                // We are editing an existing row.
                // need to get rid of the last row which would be the New row (but there is no New row)
                li.RemoveAt(li.Count - 1);
                bEditExistingRow = true;
            }
            dt_BindToGV(li.CopyToDataTable());
        }

        private void dt_BindToGV(DataTable dtu)
        {
            GridView GV = gvVendors;
            GV.Visible = true;

            GV.DataSource = dtu;
            GV.DataBind();
        }

        protected void gvVendors_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            Label lblItem = (Label)gvVendors.Rows[e.RowIndex].FindControl("lblIVendorName");
            string sItem = lblItem.Text;
            ButtonsClear();
            YesButton.CommandName = "Delete";
            YesButton.CommandArgument = sItem;
            OkButton.CommandArgument = "Vendor";
            lblPopupText.Text = "It is quite unusual to delete a vendor; they are usually kept on file for historical recordkeeping. " +
                "Do you have a recent backup of your data? " +
                "If you still want to go ahead, please confirm deletion of '" + sItem + "'";
            MPE_Show(Global.enumButtons.NoYes);
        }

        protected void gvVendors_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                GridView gv = gvVendors;
                if ((e.Row.RowState & DataControlRowState.Edit) != 0)
                {
                    int iColAddButton = gv.Columns.Count - 2; // Column of the Edit and Add buttons
                    if (!bEditExistingRow && iIndexOfLastPage == gv.PageIndex)
                    {
                        // Last row has no button except the Edit button:
                        e.Row.Cells[iColAddButton + 1].Visible = false;
                        // Last row has an Add button
                        ImageButton pb = (ImageButton)e.Row.FindControl("ipbEUpdate");
                        pb.ImageUrl = "~/i/YellowAddButton.jpg";
                        // Thick colored box around last row
                        e.Row.BackColor = Color.LightGray;
                        e.Row.BorderStyle = BorderStyle.Ridge;
                        e.Row.BorderWidth = 5;
                        e.Row.BorderColor = Color.Orange;
                        e.Row.Cells[iColAddButton].BorderStyle = BorderStyle.Ridge;
                        e.Row.Cells[iColAddButton].BorderWidth = 5;
                        e.Row.Cells[iColAddButton].BorderColor = Color.Orange;
                        // No Cancel button in the last row
                        ImageButton pbC = (ImageButton)e.Row.FindControl("ipbECancel");
                        pbC.Visible = false;
                    }
                }
                else
                {
                    if (bEditExistingRow)
                    {
                        ImageButton ipbEEdit = (ImageButton)e.Row.FindControl("ipbEEdit");
                        ImageButton ipbEDelete = (ImageButton)e.Row.FindControl("ipbEDelete");
                        ipbEEdit.Visible = false;
                        ipbEDelete.Visible = false;
                    }
                }
            }
        }

        protected void gvVendors_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            bEditExistingRow = false;
            gvVendors.EditIndex = -1;
            iEdRow = -1;
            FillDataTable();
        }

        protected void gvVendors_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            sf_AccountingDataContext dc = new sf_AccountingDataContext();
            int iLast = gvVendors.Rows.Count - 1;
            TextBox txbVendorName = (TextBox)gvVendors.Rows[e.RowIndex].FindControl("txbDVendorName");
            if (txbVendorName.Text.Trim().Length < 1)
            {
                Global.excToPopup ex = new Global.excToPopup("Vendor name must not be empty");
                ProcessPopupException(ex);
                return;
            }
            TextBox txbNotes = (TextBox)gvVendors.Rows[e.RowIndex].FindControl("txbDNotes");
            if (e.RowIndex == iLast)
            {
                SF_VENDOR vend = new SF_VENDOR();
                vend.sVendorName = Server.HtmlEncode(txbVendorName.Text.Trim().Replace("'", "`"));
                vend.sNotes = Server.HtmlEncode(txbNotes.Text.Trim().Replace("'", "`"));
                dc.SF_VENDORs.InsertOnSubmit(vend);
            }
            else
            {
                List<DataRow> liVendors = (List<DataRow>)Session["liVendors"];
                DataRow dr = liVendors[e.RowIndex];
                var r = (from v in dc.SF_VENDORs where v.sVendorName == (string)dr.ItemArray[0] select v).First();
                r.sVendorName = Server.HtmlEncode(txbVendorName.Text.Trim().Replace("'", "`"));
                r.sNotes = Server.HtmlEncode(txbNotes.Text.Trim().Replace("'", "`"));
            }
            try
            {
                dc.SubmitChanges();
            }
            catch (Exception exc)
            {
                Global.excToPopup ex = new Global.excToPopup(exc.Message);
                ProcessPopupException(ex);
                return;
            }
            dc.Dispose();
            bEditExistingRow = false;
            gvVendors.EditIndex = -1;
            iEdRow = -1;
            FillDataTable();
        }

        protected void gvVendors_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvVendors.PageIndex = e.NewPageIndex;
            FillDataTable();
        }
    }
}