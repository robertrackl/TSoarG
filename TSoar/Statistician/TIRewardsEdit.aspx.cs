using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TSoar.DB;
using TSoar.Accounting;

namespace TSoar.Statistician
{
    public partial class TIRewardsEdit : System.Web.UI.Page
    {
        private DataTable dtFilters = new DataTable("ExpFilterSettings", "TSoar.Statistician.TIRewardsFilter");
        SCUD_Multi mCRUD = new SCUD_Multi();

        #region Properties
        #region EditExistingRows
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
        #region iNgvRows
        private int iNgvRows { get { return iGetNgvRows("iNgvRows"); } set { ViewState["iNgvRows"] = value; } }
        private int iGetNgvRows(string suNgvRows)
        {
            if (ViewState[suNgvRows] == null)
            {
                return 0;
            }
            else
            {
                return (int)ViewState[suNgvRows];
            }
        }
        #endregion

        //private string slECCode { get { return (string)ViewState["slECCode"] ?? ""; } set { ViewState["slECCode"] = value; } }
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
                        case "Reward":
                            // Delete the TIReward record
                            sf_AccountingDataContext dc = new sf_AccountingDataContext();
                            var r = (from w in dc.TIREWARDs where w.ID == Int32.Parse(btn.CommandArgument) select w).First();
                            dc.TIREWARDs.DeleteOnSubmit(r);
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
                // Show user what filtering is being used
                bool bFilterInUse = false; // Is there any filtering being used? False if no.
                if ((bool)dtFilters.Rows[(int)Global.egRewardsFilters.EnableFilteringOverall][(int)Global.egRewardsFilterProps.Enabled])
                {
                    for (int ix = (int)Global.egRewardsFilters.EnableFilteringOverall + 1; ix < dtFilters.Rows.Count - 1; ix++)
                    {
                        if ((bool)dtFilters.Rows[ix][(int)Global.egRewardsFilterProps.Enabled])
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
                        if ((bool)row[(int)Global.egRewardsFilterProps.Enabled])
                        {
                            string sName = (string)row[(int)Global.egRewardsFilterProps.FilterName];
                            switch ((string)row[(int)Global.egRewardsFilterProps.FilterType])
                            {
                                case ("Integ32"):
                                    switch (sName)
                                    {
                                        case "FilterVersion":
                                            lbl_filter.Text += "Version " + ((int)row[(int)Global.egRewardsFilterProps.Integ32]).ToString();
                                            break;
                                        case "Member":
                                            int iMember = (int)row[(int)Global.egRewardsFilterProps.Integ32];
                                            using (SqlConnection SqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlConn"].ToString()))
                                            {
                                                TNPV_PeopleContactsDataContext dc = new TNPV_PeopleContactsDataContext(SqlConn);
                                                string sDisplayName = (from p in dc.PEOPLEs where p.ID == iMember select p.sDisplayName).First();
                                                lbl_filter.Text += "; " + sName + " = " + sDisplayName;
                                            }
                                            break;
                                    }
                                    break;
                                case ("List"):
                                    lbl_filter.Text += "; " + sName + " = " + (string)row[(int)Global.egRewardsFilterProps.List];
                                    break;
                                case ("DateList"):
                                    string[] sa = ((string)row[(int)Global.egRewardsFilterProps.List]).Split(',');
                                    lbl_filter.Text += "; " + sName + " between " + sa[0] + " and " + sa[1];
                                    break;
                                case ("Boolean"):
                                    lbl_filter.Text += "; " + sName + " turned on";
                                    break;
                            }
                        }
                        else
                        {
                            string sName = (string)row[(int)Global.egRewardsFilterProps.FilterName];
                            switch ((string)row[(int)Global.egRewardsFilterProps.FilterType])
                            {
                                case ("Boolean"):
                                    lbl_filter.Text += "; " + sName + " turned off, but expired entries are shown if they expired less than 90 days ago";
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
                    lbl_filter.Text = "[Expired entries are shown if they expired less than 90 days ago].";
                }
                int iRowCount = dtFilters.Rows.Count;
                lblMaxRows.Text = ((int)dtFilters.Rows[(int)Global.egRewardsFilters.LimitRowCount][(int)Global.egRewardsFilterProps.Integ32]).ToString();
                lblTopBottom.Text = ((int)dtFilters.Rows[(int)Global.egRewardsFilters.LimitAtTopBottom][(int)Global.egRewardsFilterProps.Integ32] == 2) ? "bottom" : "top";
                pnlMaxRows.Visible = (bool)dtFilters.Rows[(int)Global.egRewardsFilters.LimitAtTopBottom][(int)Global.egRewardsFilterProps.Enabled];
                pnlShowRows.Visible = !pnlMaxRows.Visible;
            }
        }

        private void FillDataTable()
        {
            bool bNewVersion = false;
            dtFilters = AssistLi.dtGetRewardsFilterSettings(out bNewVersion);
            List<DataRow> liRewards = AssistLi.Init(Global.enLL.Rewards, dtFilters, (DateTimeOffset?)Session["TiRewardsEdit.D_Earn"]); // Gets data from tables TIREWARDS and PEOPLE
            if (Session["TIrewardsLastRow"] != null)
            {
                // Replace last row with contents of previous last row:
                liRewards[liRewards.Count - 1] = (DataRow)Session["TIrewardsLastRow"];
                Session["TIrewardsLastRow"] = null; // don't want to use it again
            }
            Session["liRewards"] = liRewards;
            DataTable dtRewards = liRewards.CopyToDataTable();
            iNgvRows = dtRewards.Rows.Count; // number of rows in gvRewards
            GridViewEditEventArgs gvee = new GridViewEditEventArgs(iNgvRows - 1);
            gvRewards_RowEditing(null, gvee);
        }

        protected void gvRewards_RowEditing(object sender, GridViewEditEventArgs e)
        {
            List<DataRow> liRewards = (List<DataRow>)Session["liRewards"];
            gvRewards.EditIndex = e.NewEditIndex;
            bEditExistingRow = false;
            if (e.NewEditIndex < iNgvRows - 1)
            {
                // We are editing an existing row; need to get rid of the last row which would be the New row (but there is no New row)
                liRewards.RemoveAt(liRewards.Count - 1);
                bEditExistingRow = true;
            }
            DataTable dtRewards = liRewards.CopyToDataTable();
            dt_BindToGV(dtRewards);
        }

        private void dt_BindToGV(DataTable dtu)
        {
            GridView GV = gvRewards;
            GV.DataSource = dtu;
            GV.DataBind(); // Causes calls to gvRewards_RowDataBound for each GridViewRow, including the header row
        }

        protected void gvRewards_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            int iColAddButton = gvRewards.Columns.Count - 2; // Column of the Edit and Add buttons // SCR 223
            if (e.Row.RowType == DataControlRowType.Header)
            {
            }
            else if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.Cells[6].HorizontalAlign = HorizontalAlign.Right;
                if ((e.Row.RowState & DataControlRowState.Edit) != 0)
                {
                    CheckBox chbDEC = (CheckBox)e.Row.FindControl("chbDEC");
                    TextBox txbDDEarn = (TextBox)e.Row.FindControl("txbDDEarn");
                    TextBox txbDDExpiry = (TextBox)e.Row.FindControl("txbDDExpiry");
                    TextBox txbDDClaim = (TextBox)e.Row.FindControl("txbDDClaim");
                    bool bEarnClaim = chbDEC.Checked;

                    DropDownList DDLDMember = (DropDownList)e.Row.FindControl("DDLDMember");
                    DateTimeOffset D = DateTimeOffset.MinValue;
                    if (DateTimeOffset.TryParse(txbDDEarn.Text, out D))
                    {
                        sf_AccountingDataContext dc = new sf_AccountingDataContext();
                        var q0 = from r in dc.TNPF_EligibleRewardMembers(D, true) orderby r.sDisplayName select r;
                        DDLDMember.DataSource = q0;
                        DDLDMember.DataValueField = "ID";
                        DDLDMember.DataTextField = "sDisplayName";
                        DDLDMember.DataBind();
                    }

                    string siPerson = DataBinder.Eval(e.Row.DataItem, "iPerson").ToString();
                    Set_DropDown_ByValue(DDLDMember, siPerson);

                    txbDDEarn.Visible = bEarnClaim;
                    txbDDClaim.Visible = !bEarnClaim;

                    DropDownList DDLDECCode = (DropDownList)e.Row.FindControl("DDLDECCode");
                    string sD = DataBinder.Eval(e.Row.DataItem, "cECCode").ToString();
                    FillDDLDECCode(DDLDECCode, bEarnClaim);
                    Set_DropDown_ByValue(DDLDECCode, sD);

                    int iLast = iNgvRows - 1; // The standard editing row
                    //int iColAddButton = gvRewards.Columns.Count - 2; // Column of the Edit and Add buttons // SCR 223
                    if (!bEditExistingRow)
                    {
                        // Last row has no button except the Edit button:
                        e.Row.Cells[iColAddButton + 1].Visible = false;
                        // Last row has an Add button
                        ImageButton pb = (ImageButton)e.Row.Cells[iColAddButton].Controls[0];
                        pb.ImageUrl = "~/i/YellowAddButton.jpg";
                        e.Row.BackColor = System.Drawing.Color.LightGray;
                        e.Row.BorderStyle = BorderStyle.Ridge;
                        e.Row.BorderWidth = 5;
                        e.Row.BorderColor = System.Drawing.Color.Orange;
                        e.Row.Cells[iColAddButton].BorderStyle = BorderStyle.Ridge;
                        e.Row.Cells[iColAddButton].BorderWidth = 5;
                        e.Row.Cells[iColAddButton].BorderColor = System.Drawing.Color.Orange;
                        //txbDDEarn.AutoPostBack = true;
                        //txbDDExpiry.AutoPostBack = true;
                        //txbDDClaim.AutoPostBack = true;
                        // Working on new data being entered
                        // Expiry date will be calculated
                        txbDDExpiry.Visible = false;
                        // No Cancel button in the last row
                        ImageButton pbC = (ImageButton)e.Row.Cells[iColAddButton].Controls[2];
                        pbC.Visible = false;
                        Label lblIID = (Label)e.Row.FindControl("lblIID");
                        lblIID.Text = "New";
                    }
                    else
                    {
                        chbDEC.Enabled = false; // cannot change between non-claim and claim when editing an existing row
                    }
                }
                else
                {
                    if (bEditExistingRow)
                    {
                        //int iColAddButton = gvRewards.Columns.Count - 2; // Column of the Edit and Add buttons // SCR 223
                        ((ImageButton)e.Row.Cells[iColAddButton].Controls[0]).Visible = false;
                        ((ImageButton)e.Row.Cells[iColAddButton + 1].Controls[0]).Visible = false;
                    }
                    Label lblIEC = (Label)e.Row.FindControl("lblIEC");
                    Label lblIDEarn = (Label)e.Row.FindControl("lblIDEarn");
                    Label lblIDExpiry = (Label)e.Row.FindControl("lblIDExpiry");
                    Label lblIDClaim = (Label)e.Row.FindControl("lblIDClaim");

                    bool bEdCl = lblIEC.Text == "Earned";
                    lblIDEarn.Visible = bEdCl;
                    lblIDExpiry.Visible = bEdCl;
                    lblIDClaim.Visible = ! bEdCl;

                    switch ((char)DataBinder.Eval(e.Row.DataItem, "cECCode"))
                    {
                        case 'X':
                        case 'R':
                            foreach (Control ctrl in e.Row.Cells[iColAddButton].Controls)
                            {
                                ctrl.Visible = false;
                            }
                            break;
                    }
                }
            }
        }

        private void Set_DropDown_ByValue(DropDownList ddl, string suText)
        {
            ddl.ClearSelection();
            foreach (ListItem li in ddl.Items)
            {
                if (li.Value == suText)
                {
                    li.Selected = true;
                    break;
                }
            }
        }

        protected void gvRewards_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            Label lblItem = (Label)gvRewards.Rows[e.RowIndex].FindControl("lblIID");
            string sItem = lblItem.Text;
            ButtonsClear();
            YesButton.CommandName = "Delete";
            YesButton.CommandArgument = sItem;
            OkButton.CommandArgument = "Reward";
            lblPopupText.Text = "Please confirm deletion of record with ID = '" + sItem + "' (This action cannot be reversed)";
            MPE_Show(Global.enumButtons.NoYes);
        }

        protected void gvRewards_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            FillDataTable();
        }

        protected void gvRewards_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            Session["TIrewardsLastRow"] = null;
            string sLog = "";
            ActivityLog.enumLogTypes elt = ActivityLog.enumLogTypes.DataInsert;
            sf_AccountingDataContext dc = new sf_AccountingDataContext();
            int iLast = iNgvRows - 1; // index to last row (regardless of whether a row was trimmed off in FillDataTable()).
            CheckBox chbDEC = (CheckBox)gvRewards.Rows[e.RowIndex].FindControl("chbDEC");
            CheckBox chbEForward = (CheckBox)gvRewards.Rows[e.RowIndex].FindControl("chbEForward"); // SCR 223
            bool bForward = chbEForward.Checked; // SCR 223
            TextBox txbDDEarn = (TextBox)gvRewards.Rows[e.RowIndex].FindControl("txbDDEarn");
            TextBox txbDDExpiry = (TextBox)gvRewards.Rows[e.RowIndex].FindControl("txbDDExpiry");
            TextBox txbDDClaim = (TextBox)gvRewards.Rows[e.RowIndex].FindControl("txbDDClaim");
            DateTimeOffset D_Earn = DateTimeOffset.Parse(txbDDEarn.Text);
            DateTimeOffset D_Expiry = DateTimeOffset.Parse(txbDDExpiry.Text);
            DateTimeOffset D_Claim = DateTimeOffset.Parse(txbDDClaim.Text);
            DropDownList DDLDMember = (DropDownList)gvRewards.Rows[e.RowIndex].FindControl("DDLDMember");
            //string sDisplayName = DDLDMember.SelectedItem.Text;
            int iPerson = Int32.Parse(DDLDMember.SelectedItem.Value);
            TextBox txbDSvcPts = (TextBox)gvRewards.Rows[e.RowIndex].FindControl("txbDSvcPts");
            int iServicePts = Int32.Parse(txbDSvcPts.Text);
            DropDownList DDLDECCode = (DropDownList)gvRewards.Rows[e.RowIndex].FindControl("DDLDECCode");
            char cECCode = DDLDECCode.SelectedItem.Text.ToCharArray()[0];
            if (cECCode == 'C' || cECCode == 'G') D_Expiry = DateTimeOffset.MaxValue; // Claims never expire
            TextBox txbNotes = (TextBox)gvRewards.Rows[e.RowIndex].FindControl("txbDNotes");
            TimeSpan hOffset = TimeSpan.Parse(mCRUD.GetSetting("TimeZoneOffset")); // The Setting is quite format sensitive here; for Pacific Standard Time: -08:00
            if (e.RowIndex == iLast)
            {
                // Adding a new record
                if (chbDEC.Checked)
                {
                    // This is a reward-earned record
                    Session["TiRewardsEdit.D_Earn"] = D_Earn;
                    switch (cECCode)
                    {
                        case 'C':
                        case 'G':
                            ProcessPopupException(new Global.excToPopup("Earn/Claim code must not be C or G for a reward-earned entry, but I, M, S, or T"));
                            return;
                        default:
                            // Calculate Expiry Date
                            int iYearExpiry = D_Earn.Year;
                            // SCR 223 start
                            if (D_Earn.Month > 8) 
                            {
                                iYearExpiry++;
                                bForward = true;
                            }
                            // SCR 223 end
                            D_Expiry = new DateTimeOffset(iYearExpiry, 10, 31, 22, 59, 0, hOffset);
                            break;
                    }
                    if (D_Earn < DateTimeOffset.Parse("2017/1/1"))
                    {
                        ProcessPopupException(new Global.excToPopup("Earn date must be after start of 2017"));
                        return;
                    }
                    if (D_Expiry < DateTimeOffset.Parse("2017/1/1"))
                    {
                        ProcessPopupException(new Global.excToPopup("Expiry date must be after start of 2017"));
                        return;
                    }
                    D_Claim = DateTimeOffset.Now;
                    if (iServicePts < 0)
                    {
                        ProcessPopupException(new Global.excToPopup("For a reward-earned entry, the amount of service points must be greater than or equal to 0"));
                        return;
                    }
                }
                else
                {
                    // This is a reward-claimed record
                    if (cECCode != 'C' && cECCode != 'G')
                    {
                        ProcessPopupException(new Global.excToPopup("Earn/Claim code must be C or G for a reward-claimed entry"));
                        return;
                    }
                    if (D_Claim < DateTimeOffset.Parse("2017/1/1"))
                    {
                        ProcessPopupException(new Global.excToPopup("Claim date must be after start of 2017"));
                        return;
                    }
                    D_Earn = DateTimeOffset.Now;
                    if (iServicePts > 0)
                    {
                        ProcessPopupException(new Global.excToPopup("For a reward-claimed entry, the amount of service points must be less than or equal to 0"));
                        return;
                    }
                }
                TIREWARD reward = new TIREWARD();
                reward.PiTRecordEntered = DateTimeOffset.UtcNow;
                reward.iRecordEnteredBy = mCRUD.GetPeopleIDfromWebSiteUserName(HttpContext.Current.User.Identity.Name);
                reward.DEarn = D_Earn ;
                reward.DExpiry = D_Expiry;
                reward.DClaim = D_Claim;
                reward.iPerson = iPerson;
                reward.iServicePts = iServicePts;
                reward.cECCode = cECCode;
                reward.bForward = bForward; // SCR 223
                sLog = Server.HtmlEncode(txbNotes.Text.Trim().Replace("'", "`"));
                reward.sComments = sLog;
                dc.TIREWARDs.InsertOnSubmit(reward);
                sLog = "D_Earn=" + D_Earn.ToString("yyyy/MM/dd") + ", D_Expiry=" + D_Expiry.ToString("yyyy/MM/dd") + ", D_Claim=" + D_Claim.ToString("yyyy/MM/dd") + // SCR 223
                    ", iPerson=" + iPerson.ToString() + ", iServicePts=" + iServicePts.ToString() + ", cECCode=" + cECCode + ", bForward=" + bForward.ToString() +
                    ", sComments=" + sLog;
                DataTable dt = AssistLi.dtSchema(Global.enLL.Rewards);
                DataRow dtRow = dt.NewRow();
                dtRow[(int)Global.enPFReward.ID] = 0;
                dtRow[(int)Global.enPFReward.bEC] = chbDEC.Checked;
                dtRow[(int)Global.enPFReward.DEarn] = D_Earn;
                dtRow[(int)Global.enPFReward.DExpiry] = D_Expiry;
                dtRow[(int)Global.enPFReward.DClaim] = D_Claim;
                dtRow[(int)Global.enPFReward.iPerson] = iPerson;
                dtRow[(int)Global.enPFReward.sDisplayName] = "";
                dtRow[(int)Global.enPFReward.iServicePts] = 0;
                dtRow[(int)Global.enPFReward.cECCode] = cECCode;
                dtRow[(int)Global.enPFReward.bForward] = bForward; // SCR 223
                dtRow[(int)Global.enPFReward.sComments] = "";
                Session["TIrewardsLastRow"] = dtRow;
            }
            else
            {
                // Editing an existing record
                List<DataRow> liRewards = (List<DataRow>)Session["liRewards"];
                DataRow dr = liRewards[e.RowIndex];
                var reward = (from v in dc.TIREWARDs where v.ID == (int)dr.ItemArray[0] select v).First();
                reward.PiTRecordEntered = DateTimeOffset.UtcNow;
                reward.iRecordEnteredBy = mCRUD.GetPeopleIDfromWebSiteUserName(HttpContext.Current.User.Identity.Name);
                if (chbDEC.Checked)
                {
                    if (D_Earn < DateTimeOffset.Parse("2017/1/1"))
                    {
                        ProcessPopupException(new Global.excToPopup("Earn date must be after start of 2017"));
                        return;
                    }
                    if (D_Expiry < DateTimeOffset.Parse("2017/1/1"))
                    {
                        ProcessPopupException(new Global.excToPopup("Expiry date must be after start of 2017"));
                        return;
                    }
                    if (iServicePts < 0)
                    {
                        ProcessPopupException(new Global.excToPopup("For a reward-earned entry, the amount of service points must be greater than or equal to 0"));
                        return;
                    }
                }
                else
                {
                    if (D_Claim < DateTimeOffset.Parse("2017/1/1"))
                    {
                        ProcessPopupException(new Global.excToPopup("Claim date must be after start of 2017"));
                        return;
                    }
                    if (iServicePts > 0)
                    {
                        ProcessPopupException(new Global.excToPopup("For a reward-claimed entry, the amount of service points must be less than or equal to 0"));
                        return;
                    }
                }
                reward.DEarn = D_Earn;
                reward.DExpiry = D_Expiry;
                reward.DClaim = D_Claim;
                reward.iPerson = iPerson;
                reward.iServicePts = iServicePts;
                reward.cECCode = cECCode;
                reward.bForward = bForward;
                sLog = Server.HtmlEncode(txbNotes.Text.Trim().Replace("'", "`"));
                reward.sComments = sLog;
                sLog = "D_Earn=" + D_Earn.ToString("yyyy/MM/dd") + ", D_Expiry=" + D_Expiry.ToString("yyyy/MM/dd") + ", D_Claim=" + D_Claim.ToString("yyyy/MM/dd") +
                    ", iPerson=" + iPerson.ToString() + ", iServicePts=" + iServicePts.ToString() + ", cECCode=" + cECCode + ", bForward=" + bForward.ToString() + // SCR 223
                    ", sComments=" + sLog;
                elt = ActivityLog.enumLogTypes.DataUpdate;
            }
            try
            {
                dc.SubmitChanges();
                ActivityLog.oLog(elt, 1, "TIRewardsEdit: " + sLog);
            }
            catch (Exception exc)
            {
                Global.excToPopup ex = new Global.excToPopup(exc.Message);
                ProcessPopupException(ex);
                return;
            }
            FillDataTable();
        }

        protected void chbDEC_CheckedChanged(object sender, EventArgs e)
        {
            // The checkbox for Earned or Claimed was clicked; this can only happen in the last row of gvRewards
            bool bEarnClaim = ((CheckBox)sender).Checked;
            GridViewRow row = gvRewards.Rows[gvRewards.Rows.Count - 1]; // last row
            TextBox txbDDEarn = (TextBox)row.FindControl("txbDDEarn");
            TextBox txbDDExpiry = (TextBox)row.FindControl("txbDDExpiry");
            TextBox txbDDClaim = (TextBox)row.FindControl("txbDDClaim");
            DropDownList DDLDECCode = (DropDownList)row.FindControl("DDLDECCode"); // Selection of Earn/Claim code
            txbDDExpiry.Visible = false;
            txbDDEarn.Visible = bEarnClaim;
            txbDDClaim.Visible = !bEarnClaim;
            FillDDLDECCode(DDLDECCode, bEarnClaim);
        }

        private void FillDDLDECCode(DropDownList uDDL, bool buEarned)
        {
            // Fill the list items of dropdown control DDLDECCode
            uDDL.Items.Clear();
            if (!buEarned)
            {
                uDDL.Items.Add(new ListItem("Claim", "C"));
                uDDL.Items.Add(new ListItem("Gift Claim", "G"));
            }
            else
            {
                uDDL.Items.Add(new ListItem("Instructor", "I"));
                uDDL.Items.Add(new ListItem("Manual", "M"));
                uDDL.Items.Add(new ListItem("Showed Up", "S"));
                uDDL.Items.Add(new ListItem("Tow Pilot", "T"));
            }
        }

        public string sECCode(char cu)
        {
            switch (cu)
            {
                case 'C':
                    return "Claim";
                case 'I':
                    return "Instructor";
                case 'M':
                    return "Manual";
                case 'S':
                    return "Showed Up";
                case 'T':
                    return "Tow Pilot";
                case 'G':
                    return "Gift Claim";
                case 'R':
                    return "Reset";
                case 'X':
                    return "Clean Up";
                default:
                    return "ERROR!";
            }
        }
        // SCR 223 start
        protected void dv_Reset_DataBound(object sender, EventArgs e)
        {
            DateTime D = DateTime.Today;
            D = D.AddDays(-D.DayOfYear);
            DetailsView dv_Reset = (DetailsView)sender;
            TextBox txbDate = (TextBox)dv_Reset.FindControl("txbDate");
            txbDate.Text = CustFmt.sFmtDate(D, CustFmt.enDFmt.DateOnly).Replace("/", "-");
        }

        protected void dv_Reset_ItemInserting(object sender, DetailsViewInsertEventArgs e)
        {
            DateTimeOffset DEarn = DateTimeOffset.Parse(((TextBox)dv_Reset.FindControl("txbDate")).Text);
            string sComments = Server.HtmlEncode(((TextBox)dv_Reset.FindControl("txbComments")).Text.Trim().Replace("'", "`"));
            TIREWARD reward = new TIREWARD();
            sf_AccountingDataContext dc = new sf_AccountingDataContext();
            reward.PiTRecordEntered = DateTimeOffset.Now;
            reward.iRecordEnteredBy = mCRUD.GetPeopleIDfromWebSiteUserName(HttpContext.Current.User.Identity.Name);
            reward.DEarn = DEarn;
            reward.DExpiry = DEarn;
            reward.DClaim = DEarn;
            reward.iPerson = reward.iRecordEnteredBy;
            reward.iServicePts = 0;
            reward.cECCode = 'R';
            reward.bForward = false;
            reward.sComments = sComments;
            dc.TIREWARDs.InsertOnSubmit(reward);
            string sLog = "DEarn=" + reward.DEarn.ToString("yyyy/MM/dd") + ", DExpiry=" + reward.DExpiry.ToString("yyyy/MM/dd") + ", D_Claim=" + 
                reward.DClaim.ToString("yyyy/MM/dd") + ", iPerson=" + reward.iPerson.ToString() + ", iServicePts=" + reward.iServicePts.ToString() + 
                ", cECCode=" + reward.cECCode + ", bForward=" + reward.bForward.ToString() + ", sComments=" + reward.sComments;
            try
            {
                dc.SubmitChanges();
                ActivityLog.oLog(ActivityLog.enumLogTypes.DataInsert, 1, "TIRewardsEdit: " + sLog);
            }
            catch (Exception exc)
            {
                Global.excToPopup ex = new Global.excToPopup(exc.Message);
                ProcessPopupException(ex);
                return;
            }
        }

        protected void dv_Reset_ItemInserted(object sender, DetailsViewInsertedEventArgs e)
        {
            FillDataTable();
        }

        protected void dv_Reset_ModeChanging(object sender, DetailsViewModeEventArgs e)
        {

        }

        protected void dv_CleanUp_ItemInserting(object sender, DetailsViewInsertEventArgs e)
        {
            DateTimeOffset DEarn = DateTimeOffset.Parse(((TextBox)dv_CleanUp.FindControl("txbDate")).Text);
            string sComments = Server.HtmlEncode(((TextBox)dv_CleanUp.FindControl("txbComments")).Text.Trim().Replace("'", "`"));
            TIREWARD reward = new TIREWARD();
            sf_AccountingDataContext dc = new sf_AccountingDataContext();
            reward.PiTRecordEntered = DateTimeOffset.Now;
            reward.iRecordEnteredBy = mCRUD.GetPeopleIDfromWebSiteUserName(HttpContext.Current.User.Identity.Name);
            reward.DEarn = DEarn;
            reward.DExpiry = DEarn;
            reward.DClaim = DEarn;
            reward.iPerson = reward.iRecordEnteredBy;
            reward.iServicePts = 0;
            reward.cECCode = 'X';
            reward.bForward = false;
            reward.sComments = sComments;
            dc.TIREWARDs.InsertOnSubmit(reward);
            string sLog = "DEarn=" + reward.DEarn.ToString("yyyy/MM/dd") + ", DExpiry=" + reward.DExpiry.ToString("yyyy/MM/dd") + ", D_Claim=" +
                reward.DClaim.ToString("yyyy/MM/dd") + ", iPerson=" + reward.iPerson.ToString() + ", iServicePts=" + reward.iServicePts.ToString() +
                ", cECCode=" + reward.cECCode + ", bForward=" + reward.bForward.ToString() + ", sComments=" + reward.sComments;
            try
            {
                dc.SubmitChanges();
                ActivityLog.oLog(ActivityLog.enumLogTypes.DataInsert, 1, "TIRewardsEdit: " + sLog);
            }
            catch (Exception exc)
            {
                Global.excToPopup ex = new Global.excToPopup(exc.Message);
                ProcessPopupException(ex);
                return;
            }
        }

        protected void dv_CleanUp_ItemInserted(object sender, DetailsViewInsertedEventArgs e)
        {
            FillDataTable();
        }

        protected void dv_CleanUp_ModeChanging(object sender, DetailsViewModeEventArgs e)
        {

        }

        protected void dv_CleanUp_DataBound(object sender, EventArgs e)
        {
            DateTime D = DateTime.Today;
            D = D.AddDays(-D.DayOfYear);
            DetailsView dv_CleanUp = (DetailsView)sender;
            TextBox txbDate = (TextBox)dv_CleanUp.FindControl("txbDate");
            txbDate.Text = CustFmt.sFmtDate(D, CustFmt.enDFmt.DateOnly).Replace("/", "-");
        }
    }
}