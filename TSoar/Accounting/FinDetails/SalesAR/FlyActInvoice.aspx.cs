using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using Intuit.Ipp.Core;
using Intuit.Ipp.Data;
using Intuit.Ipp.DataService;
using Intuit.Ipp.Security;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TSoar.DB;

namespace TSoar.Accounting.FinDetails.SalesAR
{
    public partial class FlyActInvoice : System.Web.UI.Page
    {
        private const string scspALL = " ALL"; // The leading blank space is important
        private const string scDateOnlyPattern = "yyyy-MM-dd";
        private const string scLaunch = "Launch";
        private const string scRental = "Rental";
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
            if (btn.ID == "OkButton")
            {
                switch (OkButton.CommandArgument)
                {
                    case "RESET":
                        Reset();
                        break;
                    case "XMITINVOICE":
                        // OkButton.CommandName contains the invoice ID in string form
                        int? iIID = Int32.Parse(OkButton.CommandName);
                        XmitInvoice(iIID);
                        break;
                    case "DELETEINVOICE":
                        // OkButton.CommandName contains the invoice ID in string form
                        iIID = Int32.Parse(OkButton.CommandName);
                        FlyActInvoiceDataContext dc = new FlyActInvoiceDataContext();
                        var qi = (from i in dc.INVOICEs where i.ID == iIID select i).First();
                        // Find all the invoice lines that go with this invoice and update the flight operations records to reverse the number of 'invoices2go'
                        var ql = from l in dc.spFlyOpsFromInvoice(iIID) select l;
                        foreach(var l in ql)
                        {
                            var qo = (from o in dc.OPERATIONs where o.ID == l.iOperation select o).First();
                            qo.iInvoices2go++;
                        }
                        // Now we are ready to delete the invoice row; deletion of related rows in INVLINES is automatic.
                        dc.INVOICEs.DeleteOnSubmit(qi);
                        dc.SubmitChanges();
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

        #region Modal Popup Context Menu for invoice creation
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
            int iOpsID = Int32.Parse(btn.CommandName);
            try
            {
                switch (btn.ID)
                {
                    case "pbDoInvoice":
                        try
                        {
                            DoInvoiceLines(iOpsID);
                        }
                        catch (Global.excToPopup pxc)
                        {
                            ProcessPopupException(pxc);
                        }
                        break;
                    case "pbSetFirst":
                        SetFirstLastDate(iOpsID, "From");
                        break;
                    case "pbSetLast":
                        SetFirstLastDate(iOpsID, "To");
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

        #region Modal Popup Context Menu for Managing Existing Invoices
        private void ButtonsClearCMInv()
        {
            pbXmit.CommandArgument = "";
            pbXmit.CommandName = "";
            pbXmit.Visible = true;
            pbDelete.CommandArgument = "";
            pbDelete.CommandName = "";
            pbDelete.Visible = true;
            pbSet2Open.CommandArgument = "";
            pbSet2Open.CommandName = "";
            pbSet2Open.Visible = true;
            pbSet2Closed.CommandArgument = "";
            pbSet2Closed.CommandName = "";
            pbSet2Closed.Visible = true;
            pbCancelInv.CommandArgument = "";
            pbCancelInv.CommandName = "";
            pbCancelInv.Visible = true;
        }
        private void MPE_ShowCMInv()
        {
            ModPopExtCMInv.Show();
        }
        protected void pbCMInv_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            int iInvID = Int32.Parse(btn.CommandName);
            try
            {
                switch (btn.ID)
                {
                    case "pbXmit":
                        ButtonsClear();
                        lblPopupText.Text = "Please confirm transmission to QuickBooks Online of invoice with ID " + btn.CommandName;
                        OkButton.CommandArgument = "XMITINVOICE";
                        OkButton.CommandName = btn.CommandName; // copy invoice ID from Delete button to OK button
                        MPE_Show(Global.enumButtons.OkCancel);
                        break;
                    case "pbDelete":
                        ButtonsClear();
                        lblPopupText.Text = "Please confirm deletion of invoice with ID " + btn.CommandName;
                        OkButton.CommandArgument = "DELETEINVOICE";
                        OkButton.CommandName = btn.CommandName; // copy invoice ID from Delete button to OK button
                        MPE_Show(Global.enumButtons.OkCancel);
                        break;
                    case "pbSet2Open":
                        InvSet2(iInvID, "Open");
                        break;
                    case "pbSet2Closed":
                        InvSet2(iInvID, "Closed");
                        break;
                    case "pbCancelInv":
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
            dtFlyOpsInvoiceFilterSettings = TSoar.DB.AccountProfile.CurrentUser.FlyOpsInvoiceFilterSettings;
            if (dtFlyOpsInvoiceFilterSettings.Rows.Count < 1)
            {
                // Initial content for current user
                dtFlyOpsInvoiceFilterSettings = new DataTable("dtFlyOpsInvoiceFilterSettings");
                DataColumn col;
                DataRow row;

                //Columns
                //col = new DataColumn();
                //col.DataType = System.Type.GetType("System.DateTimeOffset");
                //col.ColumnName = "InvDate"; // Date of Invoice
                //dtFlyOpsInvoiceFilterSettings.Columns.Add(col);

                col = new DataColumn();
                col.DataType = System.Type.GetType("System.Int32");
                col.ColumnName = "iDays2Due";
                dtFlyOpsInvoiceFilterSettings.Columns.Add(col);

                col = new DataColumn();
                col.DataType = System.Type.GetType("System.DateTimeOffset");
                col.ColumnName = "DFrom"; // Flight operations from this date
                dtFlyOpsInvoiceFilterSettings.Columns.Add(col);

                col = new DataColumn();
                col.DataType = System.Type.GetType("System.DateTimeOffset");
                col.ColumnName = "DTo"; // Flight operations up to this date
                dtFlyOpsInvoiceFilterSettings.Columns.Add(col);

                col = new DataColumn();
                col.DataType = System.Type.GetType("System.String");
                col.ColumnName = "sMemberDisplayName"; // Show Flight Ops for this member
                dtFlyOpsInvoiceFilterSettings.Columns.Add(col);

                col = new DataColumn();
                col.DataType = System.Type.GetType("System.Boolean");
                col.ColumnName = "bFO2BInvoicedOnly"; // Only display flight operations that are still to be invoiced
                dtFlyOpsInvoiceFilterSettings.Columns.Add(col);

                //col = new DataColumn();
                //col.DataType = System.Type.GetType("System.DateTimeOffset");
                //col.ColumnName = "DFromInv"; // Display invoices from this date
                //dtFlyOpsInvoiceFilterSettings.Columns.Add(col);

                //col = new DataColumn();
                //col.DataType = System.Type.GetType("System.DateTimeOffset");
                //col.ColumnName = "DToInv"; // Display Invoices up to this date
                //dtFlyOpsInvoiceFilterSettings.Columns.Add(col);

                col = new DataColumn();
                col.DataType = System.Type.GetType("System.DateTimeOffset");
                col.ColumnName = "DFromInvFO"; // Display invoices covering flight operations from this date
                dtFlyOpsInvoiceFilterSettings.Columns.Add(col);

                col = new DataColumn();
                col.DataType = System.Type.GetType("System.DateTimeOffset");
                col.ColumnName = "DToInvFO"; // Display Invoices covering flight operations up to this date
                dtFlyOpsInvoiceFilterSettings.Columns.Add(col);

                col = new DataColumn();
                col.DataType = System.Type.GetType("System.String");
                col.ColumnName = "sMemberDisplayNameInv"; // Show invoices for this member
                dtFlyOpsInvoiceFilterSettings.Columns.Add(col);

                col = new DataColumn();
                col.DataType = System.Type.GetType("System.Boolean");
                col.ColumnName = "bOpenInvoicesOnly"; // Show only invoices that are still open
                dtFlyOpsInvoiceFilterSettings.Columns.Add(col);

                //Rows
                row = dtFlyOpsInvoiceFilterSettings.NewRow();
                DateTimeOffset heute = DateTimeOffset.Now.LocalDateTime.Date; // eliminates time of day portion
                //row["InvDate"] = heute;
                row["iDays2Due"] = 30;
                DateTimeOffset DFirstDayofThisMonth = heute.AddDays(-(heute.Day - 1));
                DateTimeOffset DTo = DFirstDayofThisMonth.AddMinutes(-1); // Last minute of previous month
                row["DTo"] = DTo;
                DateTimeOffset DFrom = DFirstDayofThisMonth.AddMonths(-1); // First Day of previous month
                row["DFrom"] = DFrom;
                row["sMemberDisplayName"] = scspALL;
                row["bFO2BInvoicedOnly"] = true;
                //row["DFromInv"] = DFrom;
                //row["DToInv"] = heute;
                row["DFromInvFO"] = DFrom;
                row["DToInvFO"] = DTo;
                row["sMemberDisplayNameInv"] = scspALL;
                row["bOpenInvoicesOnly"] = false;

                dtFlyOpsInvoiceFilterSettings.Rows.Add(row);

                TSoar.DB.AccountProfile.CurrentUser.FlyOpsInvoiceFilterSettings = dtFlyOpsInvoiceFilterSettings;
            }
            if (!IsPostBack)
            {
                txbDFrom.Text = ((DateTimeOffset)dtFlyOpsInvoiceFilterSettings.Rows[0]["DFrom"]).ToString(scDateOnlyPattern);
                txbDTo.Text = ((DateTimeOffset)dtFlyOpsInvoiceFilterSettings.Rows[0]["DTo"]).ToString(scDateOnlyPattern);
                // sMemberDisplayName and sMemberDisplayNameInv are handled in DDL_Member_PreRender
                txbDays2Due.Text = ((int)dtFlyOpsInvoiceFilterSettings.Rows[0]["iDays2Due"]).ToString();
                chbInvReq.Checked = (bool)dtFlyOpsInvoiceFilterSettings.Rows[0]["bFO2BInvoicedOnly"];
                //txbDFromInv.Text = ((DateTimeOffset)dtFlyOpsInvoiceFilterSettings.Rows[0]["DFromInv"]).ToString(scDateOnlyPattern);
                //txbDToInv.Text = ((DateTimeOffset)dtFlyOpsInvoiceFilterSettings.Rows[0]["DToInv"]).ToString(scDateOnlyPattern);
                txbDFromInvFO.Text = ((DateTimeOffset)dtFlyOpsInvoiceFilterSettings.Rows[0]["DFromInvFO"]).ToString(scDateOnlyPattern);
                txbDToInvFO.Text = ((DateTimeOffset)dtFlyOpsInvoiceFilterSettings.Rows[0]["DToInvFO"]).ToString(scDateOnlyPattern);
                chbInvOpenOnly.Checked = (bool)dtFlyOpsInvoiceFilterSettings.Rows[0]["bOpenInvoicesOnly"];
            }
        }

        protected void DDL_Member_PreRender(object sender, EventArgs e)
        {
            FlyActInvoiceDataContext dc = new FlyActInvoiceDataContext();
            DropDownList ddl = (DropDownList)sender;
            string sDisplayName = "";
            switch (ddl.ID)
            {
                case "DDL_Member":
                    var qm = (from m in dc.spMemberDisplayNames((DateTimeOffset)dtFlyOpsInvoiceFilterSettings.Rows[0]["DFrom"], (DateTimeOffset)dtFlyOpsInvoiceFilterSettings.Rows[0]["DTo"])
                              select new { m.ID, m.sDisplayName }).ToList();
                    ddl.DataSource = qm;
                    ddl.DataBind();
                    sDisplayName = (string)dtFlyOpsInvoiceFilterSettings.Rows[0]["sMemberDisplayName"];
                    if (sDisplayName == scspALL)
                    {
                        lblNumAv.Text = "up to " + qm.Count().ToString();
                    }
                    else
                    {
                        lblNumAv.Text = "1";
                    }
                    SetDropDownByText(ddl, sDisplayName);

                    DisplayGridFiltOps();
                    break;
                case "DDL_MemberInv":
                    var qi = (from m in dc.spMemberDisplayNamesInv((DateTimeOffset)dtFlyOpsInvoiceFilterSettings.Rows[0]["DFromInvFO"],
                                (DateTimeOffset)dtFlyOpsInvoiceFilterSettings.Rows[0]["DToInvFO"])
                              select new { m.ID, m.sDisplayName }).ToList();
                    ddl.DataSource = qi;
                    ddl.DataBind();
                    sDisplayName = (string)dtFlyOpsInvoiceFilterSettings.Rows[0]["sMemberDisplayNameInv"];
                    SetDropDownByText(ddl, sDisplayName);

                    DisplayInvoiceDetails();
                    break;
            }
        }

        private void SetDropDownByText(DropDownList ddl, string sText)
        {
            ddl.ClearSelection();
            foreach (ListItem li in ddl.Items)
            {
                if (li.Text == sText)
                {
                    li.Selected = true;
                    break;
                }
            }
            ddl.SelectedItem.Text = sText;
        }

        protected void txb_TextChanged(object sender, EventArgs e)
        {
            DateTimeOffset DTrial = DateTimeOffset.MinValue;
            TextBox txb = (TextBox)sender;
            string sTxb = Server.HtmlEncode(txb.Text);
            if (txb.ID != "txbDays2Due")
            {
                if (! DateTimeOffset.TryParse(sTxb, out DTrial))
                {
                    ProcessPopupException(new Global.excToPopup("'" + sTxb + "' cannot be read as a date."));
                    return;
                }
            }
            string sm = txb.ID.Substring(3); // Take away the leading 'txb' from the name/ID of the textbox
            string sFT = sm.Substring(0, 2);
            switch (sFT)
            {
                case "Da":
                    int iDays2Due = 30;
                    sm = "i" + sm; // Days2Due becomes iDays2Due
                    sTxb = Server.HtmlEncode(txbDays2Due.Text);
                    if (! Int32.TryParse(sTxb, out iDays2Due)){
                        ProcessPopupException(new Global.excToPopup("'" + sTxb + "' cannot be read as an integer value. Days to invoice due date has been set to 30."));
                        iDays2Due = 30;
                        txb.Text = iDays2Due.ToString();
                    } else if (iDays2Due <0 || iDays2Due > 120)
                    {
                        ProcessPopupException(new Global.excToPopup("'" + sTxb + "' must be between 0 and 120. Days to invoice due date has been set to 30."));
                        iDays2Due = 30;
                        txb.Text = iDays2Due.ToString();
                    }
                    dtFlyOpsInvoiceFilterSettings.Rows[0][sm] = iDays2Due;
                    break;
                case "DF":
                    DateTimeOffset DCompare = (DateTimeOffset)dtFlyOpsInvoiceFilterSettings.Rows[0][sm.Replace("From", "To")];
                    if (DCompare < DTrial)
                    {
                        ProcessPopupException(new Global.excToPopup("`From` date must not be later than `To` date; `From` date has been set equal to `To` date."));
                        DTrial = DCompare;
                        txb.Text = DTrial.ToString(scDateOnlyPattern);
                    }
                    dtFlyOpsInvoiceFilterSettings.Rows[0][sm] = DTrial;
                    break;
                case "DT":
                    DCompare = (DateTimeOffset)dtFlyOpsInvoiceFilterSettings.Rows[0][sm.Replace("To", "From")];
                    if (DCompare > DTrial)
                    {
                        ProcessPopupException(new Global.excToPopup("`To` date must not be earlier than `From` date; `To` date has been set equal to `From` date."));
                        DTrial = DCompare;
                        txb.Text = DTrial.ToString(scDateOnlyPattern);
                    }
                    dtFlyOpsInvoiceFilterSettings.Rows[0][sm] = DTrial;
                    break;
            }
            TSoar.DB.AccountProfile.CurrentUser.FlyOpsInvoiceFilterSettings = dtFlyOpsInvoiceFilterSettings;
        }

        protected void chbInvReq_CheckedChanged(object sender, EventArgs e)
        {
            dtFlyOpsInvoiceFilterSettings.Rows[0]["bFO2BInvoicedOnly"] = chbInvReq.Checked;
            TSoar.DB.AccountProfile.CurrentUser.FlyOpsInvoiceFilterSettings = dtFlyOpsInvoiceFilterSettings;
        }

        protected void DDL_Member_SelectedIndexChanged(object sender, EventArgs e)
        {
            DropDownList ddl = (DropDownList)sender;
            switch (ddl.ID)
            {
                case "DDL_Member":
                    dtFlyOpsInvoiceFilterSettings.Rows[0]["sMemberDisplayName"] = ddl.SelectedItem.Text;
                    break;
                case "DDL_MemberInv":
                    dtFlyOpsInvoiceFilterSettings.Rows[0]["sMemberDisplayNameInv"] = ddl.SelectedItem.Text;
                    break;
            }
            TSoar.DB.AccountProfile.CurrentUser.FlyOpsInvoiceFilterSettings = dtFlyOpsInvoiceFilterSettings;
        }

        private void DisplayGridFiltOps() // Display the table of filtered flight operations
        {
            DateTimeOffset DFrom = DateTimeOffset.MinValue;
            if (!DateTimeOffset.TryParse(txbDFrom.Text, out DFrom)) return;
            DateTimeOffset DTo = DateTimeOffset.MinValue;
            if (!DateTimeOffset.TryParse(txbDTo.Text, out DTo)) return;
            DTo = DTo.AddDays(1).AddMinutes(-1); // Go to end of that day less one minute

            using (DataTable dtfiltOps = new DataTable())
            {
                using (SqlConnection SqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlConn"].ToString()))
                {
                    using (SqlCommand cmd = new SqlCommand("spFiltOps"))
                    {
                        SqlParameter[] ap = new SqlParameter[4];
                        ap[0] = new SqlParameter("@DFrom", SqlDbType.DateTimeOffset)
                        {
                            Value = DFrom,
                            Direction = ParameterDirection.Input
                        };
                        ap[1] = new SqlParameter("@DTo", SqlDbType.DateTimeOffset)
                        {
                            Value = DTo,
                            Direction = ParameterDirection.Input
                        };
                        if (DDL_Member.SelectedItem == null)
                        {
                            DDL_Member.SelectedIndex = 0;
                        }
                        ap[2] = new SqlParameter("@sMemberDisplayName", SqlDbType.NVarChar, 55)
                        {
                            Value = DDL_Member.SelectedItem.Text,
                            Direction = ParameterDirection.Input
                        };
                        ap[3] = new SqlParameter("@bFO2BInvoicedOnly", SqlDbType.Bit)
                        {
                            Value = (chbInvReq.Checked) ? 1 : 0,
                            Direction = ParameterDirection.Input
                        };
                        cmd.Parameters.AddRange(ap);
                        using (SqlDataAdapter sda = new SqlDataAdapter())
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Connection = SqlConn;
                            sda.SelectCommand = cmd;
                            sda.Fill(dtfiltOps);
                        }
                    }
                }
                lblNumFlOps.Text = "up to " + dtfiltOps.Rows.Count.ToString();
                gvFlyOps.DataSource = dtfiltOps;
                gvFlyOps.DataBind();
            }
        }

        protected void gvFlyOps_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            lblPageNum.Text = (e.NewPageIndex + 1).ToString();
            gvFlyOps.PageIndex = e.NewPageIndex;
            gvFlyOps.DataBind();
        }

        protected void gvFlyOps_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                Label lblInv2go = (Label)e.Row.FindControl("lblInv2go");
                int iInv2Go = Int32.Parse(lblInv2go.Text);
                string sOpsID = ((Label)e.Row.FindControl("lblID")).Text;
                int iOpsID = Convert.ToInt32(sOpsID);
                switch (iInv2Go)
                {
                    case -1:
                        // Number of invoices is yet to be determined - so let's do it:
                        // Must figure out the number of people to be charged for this flight: can be 1 or 2
                        // If it's a free flight, then nobody gets charged, but we still create an invoice just for the record, with zero amount
                        // How many records in table AVIATORS are there belonging to this flight operation, and with a dPercentCharge > 0?
                        TSoar.DB.StatistDataContext dc = new TSoar.DB.StatistDataContext();
                        int? iAv2bChd = dc.TNPF_Aviators2bCharged(iOpsID);
                        if (iAv2bChd == null)
                        {
                            ProcessPopupException(new Global.excToPopup(
                                "FlyActInvoice.aspx.cs.gvFlyOps_RowDataBound: Scalar-valued SQL function TNPF_Aviators2bCharged returned null; iOpsID=" +
                                sOpsID));
                            return;
                        }
                        lblInv2go.Text = iAv2bChd.ToString();
                        var q = (from o in dc.OPERATIONs where o.ID == iOpsID select o).First();
                        q.iInvoices2go = (int)iAv2bChd;
                        dc.SubmitChanges();
                        break;
                    case 0:
                    case 1:
                    case 2:
                        break;
                    default:
                        ProcessPopupException(new Global.excToPopup("FlyActInvoice.aspx.cs.gvFlyOps_RowDataBound: number of invoices for operation with ID " +
                            sOpsID + " is not between -1 and 2, but " + lblInv2go.Text));
                        return;
                }
            }
        }

        protected void gvFlyOps_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int iRow = 0;
            if (int.TryParse((string)e.CommandArgument, out iRow))
            {
                if ((iRow >= 0) && (iRow < gvFlyOps.Rows.Count))
                { 
                    ButtonsClearCM(); // CM means context menu
                    // e.CommandArgument contains the row index of the GridView row clicked as a string
                    pbDoInvoice.CommandArgument = (string)e.CommandArgument;
                    pbSetFirst.CommandArgument = (string)e.CommandArgument;
                    pbSetLast.CommandArgument = (string)e.CommandArgument;
                    pbCancel.CommandArgument = (string)e.CommandArgument;
                    int iInv2Go = Int32.Parse(((Label)gvFlyOps.Rows[iRow].FindControl("lblInv2go")).Text);
                    pbDoInvoice.Visible = (iInv2Go != 0) ? true : false;
                    // Find the database table OPERATIONS row identifier of this flight op, as a string
                    string sOpsID = ((Label)gvFlyOps.Rows[iRow].FindControl("lblID")).Text;
                    pbDoInvoice.CommandName = sOpsID;
                    pbSetFirst.CommandName = sOpsID;
                    pbSetLast.CommandName = sOpsID;
                    pbCancel.CommandName = sOpsID;
                    // What is the current status of this flight operation?
                    lblMPEOpsID.Text = sOpsID; // Show user the transaction ID in the popup
                    MPE_ShowCM();
                }
                else
                {
                    ProcessPopupException(new Global.excToPopup("FlyActInvoice.aspx.cs.gvFlyOps_RowCommand: row number in e.CommandArgument is not between 0 and " +
                        (gvFlyOps.Rows.Count - 1).ToString() + " (pointer to last row in gvFlyOps)"));
                }
            }
        }

        private void DoInvoiceLines(int iuOpsID)
        {
            // For the flight operation with id iuOpsID, create one or more invoice lines to be stored in table INVLINES
            // One line is associated with this operation and one aviator and one launchmethod and one charge type (launch charge or glider rental charge)

            // Conventions re Invoices for flight operations:
            // +---------------------------------------------------------------------------------------
            // | 1. Generally, one invoice exists for all flight operations for one person and one day.
            // | 2. It is ok for more than one invoice to exist for one person and one day.
            // | 3. It is NOT ok for one invoice to cover more than one day of flight operations
            // |      (otherwise we run into problems when figuring out the monthly actual flying charges and comparing them to minimum monthly flying charges)
            // | 4. The date of the invoice (DInvoice) is the date of those flight operations.
            // | 5. The date of each invoice line (the 'service date' DService [see InvLineCharges()]) is also the date of those flight operations.
            // | 6. Generally, there are two invoice lines per flight operation: one for the launch/tow of the glider, and one for the use/rental of the glider.
            // +---------------------------------------------------------------------------------------

            //ActivityLog.oLog(ActivityLog.enumLogTypes.Debug, 16, "FlyAtInvoice.aspx.cs.DoInvoiceLines: Entry with iuOpsID=" + iuOpsID.ToString());
            FlyActInvoiceDataContext dc = new FlyActInvoiceDataContext();
            var op = (from o in dc.OPERATIONs where o.ID==iuOpsID select new { o, o.CHARGECODE.cChargeCode}).First();
            DateTimeOffset DBegin = (DateTime)op.o.DBegin; // when flight operation began
            int iLaunchMethod = op.o.iLaunchMethod;
            char cChargeCode = op.o.CHARGECODE.cChargeCode;
            var qAv = from av in dc.AVIATORs where av.iOpDetail == av.OPDETAIL.ID && av.OPDETAIL.iOperation == iuOpsID select new { av.ID, av.dPercentCharge, av.iPerson };
            foreach(var Aviator in qAv)
            {
                if (Aviator.dPercentCharge > 0.0M) // invoice only if there is a charge
                {
                    // We must have an invoice that the lines for this flight operation can belong to
                    //                 *******
                    int iInvoice = 0;
                    //   Look for an appropriate invoice that may already exist and has not been closed
                    var qInv = from inv in dc.INVOICEs
                               where inv.DFrom <= DBegin && inv.DTo >= DBegin && inv.iPerson == Aviator.iPerson && !inv.bClosed
                               select inv.ID;
                    int iCount = qInv.Count();
                    if (iCount > 1)
                    {
                        throw new Global.excToPopup("FlyActInvoice.DoInvoiceLines: Number of invoices eligible to contain charges for flying operation " +
                            iuOpsID.ToString() + " is " + iCount.ToString() + ", not 0 or 1");
                    }
                    else if (iCount == 0)
                    {
                        // Create new invoice
                        INVOICE inv = new INVOICE();
                        inv.PiTRecordEntered = DateTime.UtcNow;
                        TSoar.DB.SCUD_Multi mCRUD = new TSoar.DB.SCUD_Multi();
                        inv.iRecordEnteredBy = mCRUD.GetPeopleIDfromWebSiteUserName(HttpContext.Current.User.Identity.Name);
                        inv.DInvoice = DBegin.Date;
                        inv.iPerson= Aviator.iPerson;
                        inv.iDaysToDue = Int32.Parse(txbDays2Due.Text);
                        inv.DFrom = DBegin.Date;
                        inv.DTo = inv.DFrom.AddDays(1).AddMinutes(-1);
                        inv.mTotalAmt = 0.0M;
                        inv.mBalance = 0.0M;
                        inv.bClosed = false;
                        SCUD_single sCrud = new SCUD_single();
                        inv.iInvoiceSource = sCrud.iExists(Global.enugSingleMFF.InvoiceSources, "Flying Activities");
                        dc.INVOICEs.InsertOnSubmit(inv);
                        dc.SubmitChanges();
                        iInvoice = inv.ID;
                    }
                    else // one appropriate invoice found
                    {
                        iInvoice = qInv.First();
                    }
                    if (iInvoice == 0)
                    {
                        throw new Global.excToPopup("FlyActInvoice.DoInvoiceLines: Failed to find or create an Invoice for aviator ID=" + Aviator.ID.ToString());
                    }
                    else
                    {
                        // Charge for launch
                        //            ******
                        InvLineCharges(dc, scLaunch, iuOpsID, iInvoice, cChargeCode, Aviator.dPercentCharge, Aviator.iPerson);

                        // Charge for rental
                        //            ******
                        InvLineCharges(dc, scRental, iuOpsID, iInvoice, cChargeCode, Aviator.dPercentCharge, Aviator.iPerson);

                        // Update the record in OPERATIONS to say that the invoicing has taken place:
                        var qo = (from o in dc.OPERATIONs where o.ID==iuOpsID select o).First();
                        qo.iInvoices2go--;

                        // InvLineCharges calls InsertOnSubmit(). We do the SubmitChanges() in this routine right here.
                        dc.SubmitChanges();

                        // Update the total for the invoice from all invoice lines:
                        dc.spInvoiceTotal(iInvoice);
                    }
                } // if (Aviator.dPercentCharge > 0.0M)
            } // foreach(var Aviator in qAv)
            //ActivityLog.oLog(ActivityLog.enumLogTypes.Debug, 16, "FlyAtInvoice.aspx.cs.DoInvoiceLines: Exit");
        }

        private void InvLineCharges(FlyActInvoiceDataContext udc, string suMode, int iuOpsID, int iuInvoice, char cuChargeCode, decimal duAvPerc, int iuAvPerson) // creates an invoice line with flying activity charges
        {
            //ActivityLog.oLog(ActivityLog.enumLogTypes.Debug, 18, "FlyActInvoice.aspx.cs.InvLineCharges Entry with suMode=" + suMode + ", iuOpsID=" + iuOpsID.ToString() +
            //    ", cuChargeCode=" + cuChargeCode + ", duAvPerc=" + duAvPerc.ToString() + ", iuAvPerson=" + iuAvPerson.ToString());
           
            //    Determine which rate record is appropriate
            var qr = (from ra in udc.spSelectRate(iuOpsID, suMode) select ra).ToList();
            int iCount = qr.Count();
            if (iCount != 1)
            {
                string sMsg1 = "FlyActInvoice.InvLineCharges: number of " + suMode + " RATES records for flying operation " + iuOpsID.ToString() + " is " + iCount.ToString() +
                    ", not 1. [For the administrator, there is more detail in the log file.]";
                ProcessPopupException(new Global.excToPopup(sMsg1));
                var ql = from l in udc.OPDETAILs
                         where l.OPERATION.ID == iuOpsID
                         select new
                         {
                             l.OPERATION.DBegin,
                             l.OPERATION.CHARGECODE.cChargeCode,
                             l.EQUIPMENTROLE.sEquipmentRole,
                             l.EQUIPMENT.sShortEquipName,
                             l.EQUIPMENT.EQUIPTYPE.sEquipmentType
                         };
                foreach(var l in ql)
                {
                    string sMsg2 = "DBegin=" + l.DBegin +
                        ", ChargeCode=" + l.cChargeCode +
                        ", sEquipmentRole=" + l.sEquipmentRole +
                        ", EquipentDescription=" + l.sShortEquipName +
                        ", sEquipmentType=" + l.sEquipmentType;
                    ActivityLog.oLog(ActivityLog.enumLogTypes.Debug, 1, sMsg1 + Environment.NewLine + sMsg2);
                }
                return;
            }
            var r = qr.First();

            string sOtherOccup = ""; // Other occupant(s) in glider (or other payer)

            //    Input data for launch and rental charge calculations:
            //       Altitude difference: from OPDETAILS.dReleaseAltitude and LOCATIONS.dRunwayAltitude
            //       Duration: OPERATIONS.DEnd - OPERATIONS.DBegin
            var q0 = (from a in udc.OPDETAILs
                          where a.OPERATION.ID == iuOpsID && a.EQUIPMENTROLE.sEquipmentRole != "Glider" // we want the info regarding the launch equipment
                          select new
                          {
                              DTakeOff = a.OPERATION.DBegin,
                              dRelAlt = a.dReleaseAltitude,
                              sEquip = a.EQUIPMENT.sShortEquipName,
                              dAltdiff = a.dReleaseAltitude - a.OPERATION.LOCATION.dRunwayAltitude,
                              dDuration = ((TimeSpan)((DateTimeOffset)a.OPERATION.DEnd - (DateTimeOffset)a.OPERATION.DBegin)).Minutes
                          }).ToList();
            if (suMode != scLaunch) // throw away the previous definition of q0
            {
                q0 = (from a in udc.OPDETAILs
                          where a.OPERATION.ID == iuOpsID && a.EQUIPMENTROLE.sEquipmentRole == "Glider" // we want the info regarding the glider rental equipment
                          select new
                          {
                              DTakeOff = a.OPERATION.DBegin,
                              dRelAlt = a.dReleaseAltitude,
                              sEquip = a.EQUIPMENT.sShortEquipName,
                              dAltdiff = a.dReleaseAltitude - a.OPERATION.LOCATION.dRunwayAltitude,
                              dDuration = ((TimeSpan)((DateTimeOffset)a.OPERATION.DEnd - (DateTimeOffset)a.OPERATION.DBegin)).Minutes
                          }).ToList();
            }
            iCount = q0.Count();
            if (iCount != 1)
            {
                throw new Global.excToPopup("FlyActInvoice.InvLineCharges: number of records found for Ops ID=" + iuOpsID.ToString() + " for " + suMode + " data is not 1 but " + iCount.ToString());
            }

            // Other occupant in glider? Or does somebody help pay for the flight?
            if (suMode != scLaunch)
            {
                var qv = from v in udc.AVIATORs
                         where v.OPDETAIL.iOperation == iuOpsID && v.OPDETAIL.EQUIPMENTROLE.sEquipmentRole == "Glider"
                         select new { v.AVIATORROLE.sAviatorRole, v.iPerson, v.PEOPLE.sDisplayName };
                foreach(var v in qv)
                {
                    if (v.iPerson != iuAvPerson) // exclude the aviator for whom this invoice line is being prepared
                    {
                        string sRole = v.sAviatorRole;
                        if ((bool)udc.TNPF_IsInstructor(v.iPerson)) sRole = "Instructor";
                        sOtherOccup += " with " + sRole + " " + v.sDisplayName;
                    }
                }
            }

            DateTimeOffset DTakeOff = (DateTimeOffset)q0.First().DTakeOff;
            // Annotations:
            string sAnnot = "Flight on " + CustFmt.sFmtDate(DTakeOff, CustFmt.enDFmt.DateAndTimeMin) + sOtherOccup;
            if (suMode == scLaunch)
            {
                sAnnot += ", tow by ";
            }
            else
            {
                sAnnot += ", glider ";
            }
            sAnnot += q0.First().sEquip + ", rel @ " + q0.First().dRelAlt + " ft, Dur " + q0.First().dDuration + " min, CC " + cuChargeCode + ". ";

            // $ - charge calculation
            decimal dD = 0.0M;
            decimal dAltDiff = (decimal)q0.First().dAltdiff;
            decimal dDuration = (decimal)q0.First().dDuration;
            string sMCharge = suMode + " $: ";
            decimal dCharge = 0.0M;

            dCharge += r.mSingleDpUse;
            sMCharge += " " + r.mSingleDpUse.ToString("F2");

            decimal dChargeAltit = 0.0M;
            if (r.mAltDiffDpFt > 0.0M)
            {
                dD = dAltDiff - (decimal)r.iNoChrg1stFt;
                if (dD < 0.0M) dD = 0.0M;
                dChargeAltit += dD * r.mAltDiffDpFt;
                if (dChargeAltit > 0.0M)
                {
                    sMCharge += " + ( " + dAltDiff.ToString() +
                            " - " + r.iNoChrg1stFt.ToString() + " = " + dD.ToString("F0") + " ft) x " +
                        r.mAltDiffDpFt.ToString();
                }
            }
            dCharge += dChargeAltit;

            decimal dChargeDuration = 0.0M;
            if (r.mDurationDpMin > 0.0M)
            {
                dD = dDuration - (decimal)r.iNoChrg1stMin;
                if (dD < 0.0M) dD = 0.0M;
                dChargeDuration += dD * r.mDurationDpMin;
                if (dChargeDuration > 0.0M)
                {
                    sMCharge += " + (" + dDuration.ToString() + " - " + r.iNoChrg1stMin.ToString() + " = " + dD.ToString() + " min) x " + r.mDurationDpMin.ToString();
                }
            }

            dCharge += dChargeDuration;
            sMCharge += " = " + dCharge.ToString("F2");

            // Round up to next integer dollar
            decimal dChargeCeil = Math.Ceiling(dCharge);
            if (dChargeCeil != dCharge)
            {
                sMCharge += " rounded up to " + dChargeCeil.ToString("F2");
            }

            // Apply percentage (in case flight costs are being split)
            dChargeCeil *= duAvPerc / 100.0M;
            if (duAvPerc < 100.0M)
            {
                sMCharge += ", " + duAvPerc.ToString("F0") + " % split -> " + dChargeCeil.ToString("F2");
            }

            // Write to database
            INVLINE invl = new INVLINE();
            invl.iInvoice = iuInvoice;
            invl.DService = DTakeOff;
            invl.sDescription = sAnnot + sMCharge;
            invl.dQuantity = 1.00M;
            invl.mUnitPrice = dChargeCeil;
            invl.iQBO_ItemName = r.iQBO_ItemName;
            invl.iOperation = iuOpsID;
            udc.INVLINEs.InsertOnSubmit(invl);

            // Submitchanges is called in the routine that calls this one
            //ActivityLog.oLog(ActivityLog.enumLogTypes.Debug, 18, "FlyActInvoice.aspx.cs.InvLineCharges Exit");
        }

        protected void pbReset_Click(object sender, EventArgs e)
        {
            ButtonsClear();
            lblPopupText.Text = "Please confirm reset of all General/Control data items:";
            OkButton.CommandArgument = "RESET";
            MPE_Show(Global.enumButtons.OkCancel);
        }
        private void Reset() {
            DateTimeOffset heute = DateTimeOffset.Now.LocalDateTime.Date;

            txbDays2Due.Text = "30";
            dtFlyOpsInvoiceFilterSettings.Rows[0]["iDays2Due"] = 30;

            DateTimeOffset DFirstDayofThisMonth = heute.AddDays(-(heute.Day - 1));
            DateTimeOffset DTo = DFirstDayofThisMonth.AddMinutes(-1); // Last minute of previous month
            txbDTo.Text = DTo.ToString(scDateOnlyPattern);
            dtFlyOpsInvoiceFilterSettings.Rows[0]["DTo"] = DTo;

            DateTimeOffset DFrom = DFirstDayofThisMonth.AddMonths(-1); // First Day of previous month
            txbDFrom.Text = DFrom.ToString(scDateOnlyPattern);
            dtFlyOpsInvoiceFilterSettings.Rows[0]["DFrom"] = DFrom;

            SetDropDownByText(DDL_Member, scspALL);
            dtFlyOpsInvoiceFilterSettings.Rows[0]["sMemberDisplayName"] = scspALL;

            chbInvReq.Checked = true;
            dtFlyOpsInvoiceFilterSettings.Rows[0]["bFO2BInvoicedOnly"] = true;

            //txbDFromInv.Text = txbDFrom.Text;
            //dtFlyOpsInvoiceFilterSettings.Rows[0]["DFromInv"] = DFrom;

            //txbDToInv.Text = txbDTo.Text;
            //dtFlyOpsInvoiceFilterSettings.Rows[0]["DToInv"] = heute;

            txbDFromInvFO.Text = txbDFrom.Text;
            dtFlyOpsInvoiceFilterSettings.Rows[0]["DFromInvFO"] = DFrom;

            txbDToInvFO.Text = txbDTo.Text;
            dtFlyOpsInvoiceFilterSettings.Rows[0]["DToInvFO"] = DTo;

            SetDropDownByText(DDL_MemberInv, scspALL);
            dtFlyOpsInvoiceFilterSettings.Rows[0]["sMemberDisplayNameInv"] = scspALL;

            chbInvOpenOnly.Checked = true;
            dtFlyOpsInvoiceFilterSettings.Rows[0]["bOpenInvoicesOnly"] = true;

            TSoar.DB.AccountProfile.CurrentUser.FlyOpsInvoiceFilterSettings = dtFlyOpsInvoiceFilterSettings;
        }

        private void SetFirstLastDate(int iuOpsID, string suFromTo)
        {
            FlyActInvoiceDataContext dc = new FlyActInvoiceDataContext();
            DateTimeOffset DBegin = (DateTimeOffset)(from d in dc.OPERATIONs where d.ID == iuOpsID select d.DBegin).First();
            switch (suFromTo)
            {
                case "From":
                    DBegin = DBegin.Date;
                    txbDFrom.Text = DBegin.ToString(scDateOnlyPattern);
                    break;
                case "To":
                    DBegin = DBegin.Date.AddDays(1).AddMinutes(-1);
                    txbDTo.Text = DBegin.ToString(scDateOnlyPattern);
                    break;
                default:
                    ProcessPopupException(new Global.excToPopup("Accounting.FinDetails.SalesAR.FlyActInvoice.SetFirstLastDate: parameter suFromTo is not 'From' or 'To' but '" + suFromTo + "'"));
                    return;
            }
            dtFlyOpsInvoiceFilterSettings.Rows[0]["D" + suFromTo] = DBegin;
            TSoar.DB.AccountProfile.CurrentUser.FlyOpsInvoiceFilterSettings = dtFlyOpsInvoiceFilterSettings;

        }

        protected void pbGenInv_Click(object sender, EventArgs e)
        {
            DateTimeOffset DFrom = DateTimeOffset.MinValue;
            if (!DateTimeOffset.TryParse(txbDFrom.Text, out DFrom)) return;
            DateTimeOffset DTo = DateTimeOffset.MinValue;
            if (!DateTimeOffset.TryParse(txbDTo.Text, out DTo)) return;
            DTo = DTo.AddDays(1).AddMinutes(-1); // Go to end of that day less one minute

            using (DataTable dtfiltOps = new DataTable())
            {
                using (SqlConnection SqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlConn"].ToString()))
                {
                    using (SqlCommand cmd = new SqlCommand("spFiltOpsDistinct")) // each flight operation must occur only once
                    {
                        SqlParameter[] ap = new SqlParameter[3];
                        ap[0] = new SqlParameter("@DFrom", SqlDbType.DateTimeOffset)
                        {
                            Value = DFrom,
                            Direction = ParameterDirection.Input
                        };
                        ap[1] = new SqlParameter("@DTo", SqlDbType.DateTimeOffset)
                        {
                            Value = DTo,
                            Direction = ParameterDirection.Input
                        };
                        if (DDL_Member.SelectedItem == null)
                        {
                            DDL_Member.SelectedIndex = 0;
                        }
                        ap[2] = new SqlParameter("@sMemberDisplayName", SqlDbType.NVarChar, 55)
                        {
                            Value = DDL_Member.SelectedItem.Text,
                            Direction = ParameterDirection.Input
                        };
                        cmd.Parameters.AddRange(ap);
                        using (SqlDataAdapter sda = new SqlDataAdapter())
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Connection = SqlConn;
                            sda.SelectCommand = cmd;
                            sda.Fill(dtfiltOps);
                        }
                    }
                }
                foreach(DataRow row in dtfiltOps.Rows)
                {
                    try
                    {
                        DoInvoiceLines((int)row["ID"]);
                    }
                    catch (Global.excToPopup pxc)
                    {
                        ProcessPopupException(pxc);
                        return;
                    }
                }
            }
        }

        private void DisplayInvoiceDetails()
        {
            FlyActInvoiceDataContext dc = new FlyActInvoiceDataContext();
            //var qi = (from i in dc.spInvoiceDetails((DateTimeOffset?)dtFlyOpsInvoiceFilterSettings.Rows[0]["DFromInv"],
            //                                        (DateTimeOffset?)dtFlyOpsInvoiceFilterSettings.Rows[0]["DToInv"],
            //                                        (DateTimeOffset?)dtFlyOpsInvoiceFilterSettings.Rows[0]["DFromInvFO"],
            //                                        (DateTimeOffset?)dtFlyOpsInvoiceFilterSettings.Rows[0]["DToInvFO"],
            //                                        (string)dtFlyOpsInvoiceFilterSettings.Rows[0]["sMemberDisplayNameInv"],
            //                                        !(bool?)dtFlyOpsInvoiceFilterSettings.Rows[0]["bOpenInvoicesOnly"])
            //          orderby i.ID, i.sDescription
            //          select i).ToList();
            var qi = (from i in dc.spInvoiceDetails((DateTimeOffset?)dtFlyOpsInvoiceFilterSettings.Rows[0]["DFromInvFO"],
                                                    (DateTimeOffset?)dtFlyOpsInvoiceFilterSettings.Rows[0]["DToInvFO"],
                                                    (string)dtFlyOpsInvoiceFilterSettings.Rows[0]["sMemberDisplayNameInv"],
                                                    !(bool?)dtFlyOpsInvoiceFilterSettings.Rows[0]["bOpenInvoicesOnly"])
                      orderby i.ID, i.sDescription select i).ToList();
            gvInv.DataSource = qi;
            gvInv.DataBind();
            lblNumInv.Text = qi.Count().ToString();
            int iDprev = 0;
            int iD = 0;
            int iC1 = 0xD0F0C0;
            int iC2 = 0xEDF8D7;
            int iBackColor = iC2;
            int iLine = 0; // Counts lines in one invoice
            foreach (GridViewRow row in gvInv.Rows)
            {
                iLine++;
                iD = Int32.Parse(((Label)row.FindControl("lblID")).Text);
                if (iD != iDprev)
                {
                    iLine = 1;
                    iDprev = iD;
                    iBackColor = (iBackColor == iC2) ? iC1 : iC2; // Display successive invoices with different background colors
                }
                row.BackColor = System.Drawing.Color.FromArgb(iBackColor);
                if (iLine > 1)
                {
                    for (int iCell = 0; iCell < 10; iCell++)
                    {
                        if (iCell < 7 || iCell > 8)
                        {
                            foreach (Control con in row.Cells[iCell].Controls)
                            {
                                con.Visible = false; // suppress display of repetitive info; show it just in the top line of the invoice.
                            }
                        }
                    }
                }
            }
        }

        protected void gvInv_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int iRow = 0;
            if (int.TryParse((string)e.CommandArgument, out iRow))
            {
                if ((iRow >= 0) && (iRow < gvInv.Rows.Count))
                {
                    ButtonsClearCMInv(); // CMInv means context menu for invoice details
                    // e.CommandArgument contains the row index of the GridView row clicked, as a string
                    pbXmit.CommandArgument = (string)e.CommandArgument;
                    pbDelete.CommandArgument = (string)e.CommandArgument;
                    pbSet2Open.CommandArgument = (string)e.CommandArgument;
                    pbSet2Closed.CommandArgument = (string)e.CommandArgument;
                    pbCancelInv.CommandArgument = (string)e.CommandArgument;
                    // Find the database table INVOICES row identifier of this invoice, as a string
                    string sInvID = ((Label)gvInv.Rows[iRow].FindControl("lblID")).Text;
                    pbXmit.CommandName = sInvID;
                    pbDelete.CommandName = sInvID;
                    pbSet2Open.CommandName = sInvID;
                    pbSet2Closed.CommandName = sInvID;
                    pbCancelInv.CommandName = sInvID;
                    lblMPEInvId.Text = sInvID; // Show user the invoice ID in the popup
                    MPE_ShowCMInv();
                }
                else
                {
                    ProcessPopupException(new Global.excToPopup("FlyActInvoice.aspx.cs.gvInv_RowCommand: row number in e.CommandArgument is not between 0 and " +
                        (gvInv.Rows.Count - 1).ToString() + " (pointer to last row in gvInv)"));
                }
            }
        }

        private void InvSet2(int iuInvID, string suOpenClosed) // set an invoice to open/closed status
        {
            FlyActInvoiceDataContext dc = new FlyActInvoiceDataContext();
            INVOICE inv = (from i in dc.INVOICEs where i.ID == iuInvID select i).First();
            inv.bClosed = (suOpenClosed == "Open") ? false : true;
            dc.SubmitChanges();
        }

        protected void chbInvOpenOnly_CheckedChanged(object sender, EventArgs e)
        {
            dtFlyOpsInvoiceFilterSettings.Rows[0]["bOpenInvoicesOnly"] = chbInvOpenOnly.Checked;
            TSoar.DB.AccountProfile.CurrentUser.FlyOpsInvoiceFilterSettings = dtFlyOpsInvoiceFilterSettings;
        }

        private void XmitInvoice(int? iuID) // iuID = ID of row in table INVOICES; QBO means Intuit QuickBooks Online
        {
            //ActivityLog.oLog(ActivityLog.enumLogTypes.Debug, 20, "FlyActInvoice.aspx.cs.XmitInvoice: Entry");
            // Copied in part from:
            //   https://developer.intuit.com/app/developer/qbo/docs/concepts/invoicing#how-to-implement

            // Step 1: Are we connected to QuickBooks Online?
            if (Session["dictTokens"] == null)
            {
                Session["dictTokens"] = new Dictionary<string, string>();
            }
            Dictionary<string, string> dictTokens = (Dictionary<string, string>)Session["dictTokens"];
            if (!dictTokens.ContainsKey("accessToken"))
            {
                ProcessPopupException(new Global.excToPopup("A session with QuickBooks Online has not been established via OAuth2."));
                divgv.Visible = false;
                div2QBO.Visible = true;
                return;
            }

            // Step 2: Compose serviceContext
            OAuth2RequestValidator oauthValidator;
            try
            {
                oauthValidator = new OAuth2RequestValidator(dictTokens["accessToken"]);
            }
            catch (Exception exc)
            {
                ProcessPopupException(new Global.excToPopup("Problem with OAuth2 connection to QuickBooks Online: " + exc.Message));
                divgv.Visible = false;
                div2QBO.Visible = true;
                return;
            }
            ServiceContext serviceContext = new ServiceContext(dictTokens["realmId"], IntuitServicesType.QBO, oauthValidator);
            serviceContext.IppConfiguration.MinorVersion.Qbo = "29";

            // Step 3: Find QBO customer Id in table PEOPLE
            FlyActInvoiceDataContext dc = new FlyActInvoiceDataContext();
            string sIdQBO = (from i in dc.INVOICEs where i.ID == iuID select i.PEOPLE.sIdQBO).First();
            if (sIdQBO == null)
            {
                ProcessPopupException(new Global.excToPopup("FlyActInvoice.aspx.cs.XmitInvoice: QBO Customer identifier is null. Are you missing that identifier in the member's properties? " +
                    "(Go to Club Membership/Club Members Basic List, column `QBO Id`)"));
                return;
            }
            if (sIdQBO.Length < 1)
            {
                ProcessPopupException(new Global.excToPopup("FlyActInvoice.aspx.cs.XmitInvoice: QBO Customer identifier exists but has no contents. Are you missing that identifier in the member's properties? " +
                    "(Go to Club Membership/Club Members Basic List, column `QBO Id`)"));
                return;
            }

            // Step 4: Create a customer object and put the customer of interest into it
            Customer customer = QBOHelper.CreateCustomer(serviceContext);
            customer.Id = sIdQBO;
            Customer custFound;
            try
            {
                custFound = Helper.FindById<Customer>(serviceContext, customer);
            }
            catch (Exception iexc)
            {
                ProcessPopupException(new Global.excToPopup("FlyActInvoice.aspx.cs.XmitInvoice: Problem finding QBO customer with Id=" + sIdQBO + ": " + iexc.Message));
                return;
            }

            // Step 5: Create a QBO US Invoice object
            Invoice QBOinvoice = new Invoice();

            // Step 6: Attach the customer to the invoice
            QBOinvoice.CustomerRef = new ReferenceType()
            {
                name = custFound.DisplayName,
                Value = custFound.Id
            };

            // Step 7: define miscellaneous data to go with the invoice
            QBOinvoice.DepositSpecified = false;
            QBOinvoice.TotalAmtSpecified = false;
            QBOinvoice.BalanceSpecified = false;

            // Step 8: define invoice transaction ("Txn") and due dates
            DateTimeOffset? DInvoice = (from i in dc.INVOICEs where i.ID == iuID select i.DInvoice).First();

            QBOinvoice.TxnDate = ((DateTimeOffset)DInvoice).DateTime; //Transaction Date; date of invoice
            QBOinvoice.TxnDateSpecified = true;

            QBOinvoice.DueDate = QBOinvoice.TxnDate.AddDays(15);
            QBOinvoice.DueDateSpecified = true;

            // Step 9: Create the list of line items
            List<Line> lineList = new List<Line>();

            // Step 10: Define the lines
            var q1 = from l in dc.INVLINEs where l.INVOICE.ID == iuID select l;
            int iLineNum = 0;
            foreach (var l in q1)
            {

                Line QBOinvLineItem = new Line();
                QBOinvLineItem.LineNum = (++iLineNum).ToString();
                QBOinvLineItem.Description = l.sDescription;
                QBOinvLineItem.DetailType = LineDetailTypeEnum.SalesItemLineDetail;
                QBOinvLineItem.DetailTypeSpecified = true;
                QBOinvLineItem.Amount = l.mUnitPrice;
                QBOinvLineItem.AmountSpecified = true;

                Item QboPSitem; // a QBO Products & Services item
                try
                {
                    QboPSitem = FindByNameItem(serviceContext, (int)l.iQBO_ItemName, dc);
                }
                catch (Global.excToPopup etp)
                {
                    ProcessPopupException(etp);
                    return;
                }

                QBOinvLineItem.AnyIntuitObject = new SalesItemLineDetail()
                {
                    Qty = 1,
                    QtySpecified = true,
                    ItemRef = new ReferenceType()
                    {
                        name = QboPSitem.Name,
                        Value = QboPSitem.Id
                    },
                    ServiceDate = l.DService.DateTime // date of service, i.e., date of takeoff
                };

                // Step 11: Add the line item to the list of line items
                lineList.Add(QBOinvLineItem);
            }

            // Step 12: Attach the line item list to the invoice
            QBOinvoice.Line = lineList.ToArray();

            // Step 13: Transmit the invoice
            DataService service = new DataService(serviceContext);
            try
            {
                Invoice addedInvoice = service.Add<Invoice>(QBOinvoice);
            }
            catch (Intuit.Ipp.Exception.IdsException IdsExc)
            {
                ProcessPopupException(new Global.excToPopup("Inv2Qbo.aspx.cs.genInvoice: IdsException Message=" + IdsExc.Message + ", inner exception message=" + IdsExc.InnerException.Message));
                return;
            }
            catch (Exception exc)
            {
                ProcessPopupException(new Global.excToPopup("Inv2Qbo.aspx.cs.genInvoice: general exception, Message=" + exc.Message));
                return;
            }

            // Step 14: Mark the invoice as closed in table INVOICES, i.e., we cannot add any more lines to it
            INVOICE invo = (from i in dc.INVOICEs where i.ID == iuID select i).First();
            invo.bClosed = true;
            dc.SubmitChanges();
            //ActivityLog.oLog(ActivityLog.enumLogTypes.Debug, 20, "FlyActInvoice.aspx.cs.XmitInvoice: Exit");
        }

        private Item FindByNameItem(ServiceContext context, int iuQBO_ItemName, FlyActInvoiceDataContext udc)
        {
            string sItemName = (from i in udc.QBO_ITEMNAMEs where i.ID == iuQBO_ItemName select i.sQBO_ItemName).First();
            List < Item > liItem = Helper.FindAll<Item>(context, new Item(), 1, 500).Where(i => i.status != EntityStatusEnum.SyncError).ToList();
            if (liItem.Count > 0)
            {
                foreach (Item item in liItem)
                {
                    if (item.Name == sItemName)
                    {
                        return item;
                    }
                }
                throw new Global.excToPopup("Inv2Qbo.aspx.cs.FindByNameItem: no item found with name `" + sItemName + "`");
            }
            else
            {
                throw new Global.excToPopup("Inv2Qbo.aspx.cs.FindByNameItem: list of items is empty");
            }
        }

        protected void pbHelp_Click(object sender, EventArgs e)
        {
            ModPopExt_Help.Show();
        }

        protected void pbFDone_Click(object sender, EventArgs e)
        {

        }
    }
}