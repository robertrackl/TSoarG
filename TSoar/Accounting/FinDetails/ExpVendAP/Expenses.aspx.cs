using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TSoar.DB;

namespace TSoar.Accounting.FinDetails.ExpVendAP
{
    public partial class Expenses : System.Web.UI.Page
    {
        private DataTable dtFilters = new DataTable("ExpFilterSettings", "TSoar.Accounting.FinDetails.ExpVendAP");

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
                switch (OkButton.CommandArgument)
                {
                    case "VOID":
                        XactVoidAtRow(Convert.ToInt32(OkButton.CommandName), OkButton.CommandArgument);
                        Response.Redirect(Request.RawUrl); // Page Refresh
                        break;
                }
            }
        }

        private void ProcessPopupException(Global.excToPopup excu)
        {
            ButtonsClear();
            lblPopupText.Text = excu.sExcMsg();
            MPE_Show(Global.enumButtons.OkOnly);
        }
        #endregion

        #region Modal Popup Context Menu
        private void ButtonsClearCM()
        {
            pbEdit.CommandArgument = "";
            pbEdit.CommandName = "";
            pbEdit.Visible=true;
            pbVoid.CommandArgument = "";
            pbVoid.CommandName = "";
            pbVoid.Visible = true;
            pbDelete.CommandArgument = "";
            pbDelete.CommandName = "";
            pbDelete.Text = "Delete";
            pbDelete.Visible = true;
            pbCancel.CommandArgument = "";
            pbCancel.CommandName = "";
            pbCancel.Visible = true;
            pbTemplate.CommandArgument = "";
            pbTemplate.CommandName = "";
            pbTemplate.Visible = true;
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
                    case "pbInspect":
                        Session["CallMode"] = new KeyValuePair<string, int>("INSPECT", Convert.ToInt32(btn.CommandName));
                        Response.Redirect("XactExpense.aspx");
                        break;
                    case "pbEdit":
                        // btn.CommandName contains the transaction's database table row ID as a string
                        Session["CallMode"] = new KeyValuePair<string, int>("EDIT", Convert.ToInt32(btn.CommandName));
                        Response.Redirect("XactExpense.aspx");
                        break;
                    case "pbVoid":
                        XactVoidAtRow(Convert.ToInt32(btn.CommandName), "INIT");
                        break;
                    case "pbDelete":
                        // btn.CommandArgument contains the row index as a string
                        XactDeleteAtRow(Convert.ToInt32(btn.CommandArgument), btn.Text == "Undelete");
                        break;
                    case "pbTemplate":
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

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                chbTimeOfDay.Checked = AccountProfile.CurrentUser.bShowXactTime;

                DisplayGrid();

                // Show user what filtering is being used
                bool bFilterInUse = false; // Is there any filtering being used? False if no.
                if ((bool)dtFilters.Rows[(int)Global.egXactFilters.EnableFilteringOverall][(int)Global.egAdvFilterProps.Enabled])
                {
                    for (int ix = (int)Global.egXactFilters.EnableFilteringOverall + 1; ix < dtFilters.Rows.Count - 1; ix++)
                    {
                        if ((bool)dtFilters.Rows[ix][(int)Global.egAdvFilterProps.Enabled])
                        {
                            bFilterInUse = true;
                            break;
                        }
                    }
                }
                if (bFilterInUse)
                { // a filter is in use
                    lbl_is.Font.Bold = true;
                    lbl_not.Text = "";
                    lbl_filter.Text = " [ ";
                    foreach (DataRow row in dtFilters.Rows)
                    {
                        if ((bool)row[(int)Global.egAdvFilterProps.Enabled])
                        {
                            string sName = (string)row[(int)Global.egAdvFilterProps.FilterName];
                            switch ((string)row[(int)Global.egAdvFilterProps.FilterType])
                            {
                                case ("Range"):
                                    if ((string)row[(int)Global.egAdvFilterProps.FilterName] == "FilterVersion")
                                    {
                                        lbl_filter.Text += "Version " + ((decimal)row[(int)Global.egAdvFilterProps.LowLimit]).ToString();
                                    }
                                    else
                                    {
                                        lbl_filter.Text += "; " + sName + " between " + ((decimal)row[(int)Global.egAdvFilterProps.LowLimit]).ToString() +
                                            " and " + ((decimal)row[(int)Global.egAdvFilterProps.HighLimit]).ToString();
                                    }
                                    break;
                                case ("List"):
                                    lbl_filter.Text += "; " + sName + ((bool)row[(int)Global.egAdvFilterProps.INorEX] ? "" : " NOT ") +
                                        " In (" + (string)row[(int)Global.egAdvFilterProps.List] + ")";
                                    break;
                                case ("DateList"):
                                    string[] sa = ((string)row[(int)Global.egAdvFilterProps.List]).Split(',');
                                    lbl_filter.Text += "; " + sName + " between " + sa[0] + " and " + sa[1];
                                    break;
                                case ("Boolean"):
                                    lbl_filter.Text += "; " + sName + " turned on";
                                    break;
                            }
                        }
                    }
                    lbl_filter.Text += " ].";
                }
                else
                { // filter is not being used
                    lbl_is.Font.Bold = false;
                    lbl_not.Text = "not";
                    lbl_not.Font.Bold = true;
                    lbl_filter.Text = ".";
                }
            }
        }

        protected void chbTimeOfDay_CheckedChanged(object sender, EventArgs e)
        {
            AccountProfile.CurrentUser.bShowXactTime = chbTimeOfDay.Checked;
            DisplayGrid();
        }

        private void DisplayGrid()
        {
            string sMsg="(empty message)";
            try
            {
                dtFilters = AccountProfile.CurrentUser.XactFilterSettings;
                if (dtFilters.Rows.Count < 1)
                {
                    dtFilters = XactFilter.dtDefaultXactFilter();
                    AccountProfile.CurrentUser.XactFilterSettings = dtFilters;
                }

                DataTable dtTSort = AccountProfile.CurrentUser.XactSortSettings;
                if (dtTSort.Rows.Count < 1)
                {
                    dtTSort = XactSort.DefaultXactSort();
                    AccountProfile.CurrentUser.XactSortSettings = dtTSort;
                }

                // Generate the list of expenses from table SF_XACTS (and adjoining ones) subject to info in dtTFilter and dtTSort
                using (DataTable dt = new DataTable())
                {
                    using (SqlConnection SqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlConn"].ToString()))
                    {
                        using (SqlCommand cmd = new SqlCommand("sf_ExpenseJournal"))
                        {
                            SqlParameter[] ap = new SqlParameter[4];
                            ap[0] = new SqlParameter("@sStatus", SqlDbType.NVarChar, 350000)
                            {
                                Value = "undefined",
                                Direction = ParameterDirection.InputOutput
                            };
                            ap[1] = new SqlParameter("@bShowXactTime", SqlDbType.Bit)
                            {
                                Value = AccountProfile.CurrentUser.bShowXactTime,
                                Direction = ParameterDirection.Input
                            };
                            ap[2] = new SqlParameter("@taFilter", SqlDbType.Structured)
                            {
                                Value = dtFilters,
                                Direction = ParameterDirection.Input
                            };
                            ap[3] = new SqlParameter("@taSort", SqlDbType.Structured)
                            {
                                Value = dtTSort,
                                Direction = ParameterDirection.Input
                            };
                            cmd.Parameters.AddRange(ap);
                            using (SqlDataAdapter sda = new SqlDataAdapter())
                            {
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Connection = SqlConn;
                                sda.SelectCommand = cmd;
                                sda.Fill(dt);
                                sMsg = (string)ap[0].Value;
                                if (sMsg.Substring(0,2) != "OK")
                                {
                                    throw new Global.excToPopup(sMsg);
                                }
                            }
                        }
                    }
                    gvExpenses.DataSource = dt;
                    gvExpenses.DataBind();
                }
            }
            catch (Global.excToPopup exc)
            {
                ActivityLog.oLog(ActivityLog.enumLogTypes.Debug, 2, sMsg);
                ProcessPopupException(new Global.excToPopup(exc.Message + ": Stored Procedure sf_ExpenseJournal debug written to table ACTIVITYLOG at about " + DateTimeOffset.UtcNow.ToString() +
                    ". Inspection requires administrator access."));
            }
            catch (Exception gexc)
            {
                ProcessPopupException(new Global.excToPopup(gexc.Message));
            }
        }

        private void XactVoidAtRow(int iuXactId, string sMode)
        {
            // Void the financial transaction (Xact) with internal Id iuXactId.
            // To void means to set all expenditures and payments to zero for this Xact.
            // Files that may be attached are left undisturbed.

            switch (sMode) {
                case "INIT":
                    // Start with making sure that the user really wants to void this Xact
                    lblPopupText.Text = "Are you sure that you want to void the financial transaction with Id " +
                        iuXactId.ToString() + "? (This action cannot be undone!)";
                    ButtonsClear();
                    YesButton.CommandName = "Delete";
                    OkButton.CommandArgument = "VOID";
                    OkButton.CommandName = iuXactId.ToString();
                    MPE_Show(Global.enumButtons.NoYes);
                    break;
                case "VOID":
                    // Now perform the voiding itself
                    SCUD_Multi mCRUD = new SCUD_Multi();
                    int iUser = mCRUD.GetPeopleIDfromWebSiteUserName(HttpContext.Current.User.Identity.Name);
                    using (TransactionScope Tscope = new TransactionScope())
                    {
                        using (sf_AccountingDataContext dc = new sf_AccountingDataContext())
                        {
                            string sJold = XactEng.sXSerialize(iuXactId, dc);
                            var y = (from x in dc.SF_XACTs where x.ID == iuXactId select x).First();
                            y.cStatusPrev = y.cStatus;
                            y.cStatus = 'V';
                            var q0 = from e in dc.SF_ENTRies where e.iXactId == iuXactId select e;
                            foreach (var e in q0)
                            {
                                e.mAmount = 0.0M;
                            }
                            dc.SubmitChanges();

                            SF_AUDITTRAIL _AUDITTRAIL = new SF_AUDITTRAIL();
                            _AUDITTRAIL.DTimeStamp = DateTimeOffset.Now;
                            _AUDITTRAIL.iRecordEnteredBy = iUser;
                            _AUDITTRAIL.cAction = 'V';
                            _AUDITTRAIL.Jold = sJold;
                            _AUDITTRAIL.Jnew = XactEng.sXSerialize(iuXactId, dc);
                            dc.SF_AUDITTRAILs.InsertOnSubmit(_AUDITTRAIL);
                            dc.SubmitChanges();
                        }
                        Tscope.Complete();
                    }
                    break;
            }

        }

        protected void gvExpenses_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
            {
                e.Row.Cells[1].Text = chbTimeOfDay.Checked ? "Date, Time, and Offset to UTC" : "Date";
            }
        }

        protected void gvExpenses_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            ButtonsClearCM(); // CM means context menu
            // e.CommandArgument contains the row index of the GridView row clicked as a string
            pbInspect.CommandArgument = (string)e.CommandArgument;
            pbEdit.CommandArgument = (string)e.CommandArgument;
            pbVoid.CommandArgument = (string)e.CommandArgument;
            pbDelete.CommandArgument = (string)e.CommandArgument;
            pbTemplate.CommandArgument = (string)e.CommandArgument;
            // Find the database table SF_XACTS row identifier of this transaction, as a string
            string sXactID = ((Label)gvExpenses.Rows[Convert.ToInt32(e.CommandArgument)].FindControl("lblID")).Text;
            pbInspect.CommandName = sXactID;
            pbEdit.CommandName = sXactID;
            pbVoid.CommandName = sXactID;
            pbDelete.CommandName = sXactID;
            pbTemplate.CommandName = sXactID;
            // What is the current status of this transaction?
            sf_AccountingDataContext dc = new sf_AccountingDataContext();
            int iXactID = Convert.ToInt32(sXactID);
            var r = (from t in dc.SF_XACTs where t.ID == iXactID select t).First();
            switch (r.cStatus)
            {
                case 'D':
                    // A 'deleted' transaction can be undeleted (made active again)
                    pbDelete.Text = "Undelete";
                    break;
                case 'R':
                case 'V':
                    // A 'replaced' or 'voided' transaction can only be inspected, or made into a template
                    pbEdit.Enabled = false;
                    pbVoid.Enabled = false;
                    pbDelete.Enabled = false;
                    break;

            }
            lblMPEXactID.Text = iXactID.ToString(); // Show user the transaction ID in the popup
            MPE_ShowCM();
        }

        private void XactDeleteAtRow(int iRowIndex, bool buUnDelete)
        {
            // The transaction record is not deleted, merely marked as deleted
            int iXactId = Convert.ToInt32(((Label)gvExpenses.Rows[iRowIndex].FindControl("lblID")).Text);
            sf_AccountingDataContext dc = new sf_AccountingDataContext();
            var r = (from t in dc.SF_XACTs where t.ID == iXactId select t).First();
            if (buUnDelete)
            {
                r.cStatus = (char)r.cStatusPrev;
                r.cStatusPrev = 'D';
            }
            else
            {
                r.cStatusPrev = r.cStatus;
                r.cStatus = 'D'; // this is how we mark as deleted
            }
            dc.SubmitChanges();
            DisplayGrid();
        }

        protected void pbNotes_Click(object sender, EventArgs e)
        {
            MPE_Panel.Width =450;
            MPE_Panel.HorizontalAlign = HorizontalAlign.Left;
            MPE_Panel.BackColor = System.Drawing.Color.FromName("White");
            lblPopupText.BackColor = System.Drawing.Color.FromName("White");
            lblPopupText.Font.Bold = false;
            char cdq = Convert.ToChar(34);
            lblPopupText.Text = "Notes on Status of a Transaction:" +
                "<span style=" + cdq + "font-size: smaller !important;" + cdq + ">" +
                "<ul>" +
                    "<li>A - Active; can be ... " +
                        "<ul> " +
                            "<li>marked deleted,</li> " +
                            "<li>voided($-amounts changed to zeroes),</li> " +
                            "<li>copied to become a template. </li>" +
                        "</ul> " +
                    "</li> " +
                    "<li>D - marked Deleted" +
                        "<ul> " +
                            "<li>A transaction marked deleted can be undeleted to become active again.</li> " +
                        "</ul> " +
                    "</li>" +
                    "<li>V - Voided; </li>" +
                        "<ul> " +
                            "<li>cannot be unvoided; once voided it stays that way.</li> " +
                        "</ul> " +
                    "</li>" +
                    "<li>R - Replaced:" +
                        "<ul> " +
                            "<li>When an active transaction is edited (modified) a new active transaction is created that replaces the original one.</li> " +
                            "<li>Once in Replaced status, the transaction's status stays that way.</li> " +
                        "</ul> " +
                    "</li>" +
                    "<li>T - used as Template for new transaction</li></span>" +
                "</ul>" +
                "Notes on Transaction Filters:" +
                "<br />" +
                "<span style=" + cdq + "font-size: smaller !important;" + cdq + ">" +
                    "By default, all active and voided transactions are displayed. To change that, click on the button marked `" +
                        pbExpFilters.Text + "`." +
                "</span>" +
                "<br />" +
                "Notes on Sorting:" +
                "<br />" +
                "<span style=" + cdq + "font-size: smaller !important;" + cdq + ">" +
                    "By default, transactions are sorted by Date (descending, most recent first), then by Vendor name (ascending), " +
                    "status (ascending), and amount (descending, largest first). " +
                    "To change those sort priorities and sort orders, click on the button marked `" + pbSort.Text + "`." + 
                "</span>";

            ButtonsClear();
            OkButton.Text = "Dismiss";
            MPE_Show(Global.enumButtons.OkOnly);
        }

        protected void pbSort_Click(object sender, EventArgs e)
        {
            RadioButtonList[] obla = new RadioButtonList[4];
            obla[0] = obl0;
            obla[1] = obl1;
            obla[2] = obl2;
            obla[3] = obl3;
            DataTable dtTSort = AccountProfile.CurrentUser.XactSortSettings;
            int rowIndex = 0;
            foreach(DataRow dr in dtTSort.Rows)
            {
                obla[rowIndex].ClearSelection();
                obla[rowIndex].Items[(int)dr["SortPriority"] - 1].Selected = true;
                rowIndex++;
            }
            ModPopExtSort.Show();
        }
        
        protected void pbSortOKCancel_Click(object sender, EventArgs e)
        {
            Button pb = (Button)sender;
            switch (pb.ID)
            {
                case "pbSortOK":
                    SaveSortSettings();
                    break;
                case "pbSortDefault":
                    obl0.SelectedIndex = 0;
                    obl1.SelectedIndex = 1;
                    obl2.SelectedIndex = 2;
                    obl3.SelectedIndex = 3;
                    oblAscDesc0.SelectedIndex = 1;
                    oblAscDesc1.SelectedIndex = 0;
                    oblAscDesc2.SelectedIndex = 0;
                    oblAscDesc3.SelectedIndex = 1;
                    SaveSortSettings();
                    break;
                default:
                    break;
            }
        }
        private void SaveSortSettings()
        {
            RadioButtonList[] obla;
            RadioButtonList[] oblaAscDesc;
            oblaAscDesc = new RadioButtonList[4];
            obla = new RadioButtonList[4];
            obla[0] = obl0;
            obla[1] = obl1;
            obla[2] = obl2;
            obla[3] = obl3;
            oblaAscDesc[0] = oblAscDesc0;
            oblaAscDesc[1] = oblAscDesc1;
            oblaAscDesc[2] = oblAscDesc2;
            oblaAscDesc[3] = oblAscDesc3;
            int iSum = 0;
            foreach (XactSort.eSortBy es in Enum.GetValues(typeof(XactSort.eSortBy)))
            {
                for (int j = 1; j < 5; j++)
                {
                    if (obla[(int)es].Items[j - 1].Selected)
                    {
                        iSum += j * (int)(Math.Pow(10.0, j - 1) + 0.1);
                    }
                }
            }
            if (iSum != 4321)
            {
                ProcessPopupException(new Global.excToPopup("Invalid Sort Priorities - each of Date, Vendor, Status, and Amount " +
                    "must be assigned a different sort priority between 1 and 4. [Checksum=" + iSum.ToString() + "]"));
            }
            else
            {
                DataTable dtTSort = AccountProfile.CurrentUser.XactSortSettings;
                foreach (XactSort.eSortBy es in Enum.GetValues(typeof(XactSort.eSortBy)))
                {
                    for (int j = 1; j < 5; j++)
                    {
                        if (obla[(int)es].Items[j - 1].Selected)
                        {
                            dtTSort.Rows[(int)es]["SortPriority"] = j;
                        }
                    }
                    dtTSort.Rows[(int)es]["SortOrder"] = (oblaAscDesc[(int)es].SelectedIndex == 0) ? "Asc" : "Desc";
                }
                AccountProfile.CurrentUser.XactSortSettings = dtTSort;
                DisplayGrid();
            }
        }

        protected void pbExpFilters_Click(object sender, EventArgs e)
        {
            Server.Transfer("~/Accounting/FinDetails/ExpVendAP/ExpFilter.aspx", true);
        }

        protected void pbXactExpense_Click(object sender, EventArgs e)
        {
            Session["CallMode"] = new KeyValuePair<string, int>("NEW", 0);
            Server.Transfer("~/Accounting/FinDetails/ExpVendAP/XactExpense.aspx", true);
        }
    }
}
