using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TSoar.Accounting;
using TSoar.DB;
using System.Data;

namespace TSoar.Statistician
{
    public partial class FlightLogInput : System.Web.UI.Page
    {
        private enum Enbuttons { None, Edit, Select, Post, Delete }
        SCUD_Multi mCRUD = new SCUD_Multi();

        #region ViewState Booleans
        private bool bEditExistingDailyFLRow
        {
            get { return GetVSBoolean("bEditExistingDailyFLRow", false); }
            set { ViewState["bEditExistingDailyFLRow"] = value; }
        }
        private bool bEditExistingFlLogRow
        {
            get { return GetVSBoolean("bEditExistingFlLogRow", false); }
            set { ViewState["bEditExistingFlLogRow"] = value; }
        }
        private bool bWhichDefaultSet
        {
            get { return GetVSBoolean("bWhichDefaultSet", true); }
            set { ViewState["bWhichDefaultSet"] = value; }
        }
        private bool GetVSBoolean(string sub, bool buInitDefault)
        {
            if (ViewState[sub] is null)
            {
                return buInitDefault;
            }
            else
            {
                return (bool)ViewState[sub];
            }
        }
        #endregion
        #region ViewState Integers
        private int iNgvDailyFlRows { get { return iGetNgvRows("iNgvDailyFlRows"); } set { ViewState["iNgvDailyFlRows"] = value; } }
        private int iNgvFliNlRows { get { return iGetNgvRows("iNgvFliNlRows"); } set { ViewState["iNgvFliNlRows"] = value; } }
        private int iFlightLog { get { return iGetNgvRows("iFlightLog"); } set { ViewState["iFlightLog"] = value; } }
        private int igvFliNRowIndex { get { return iGetNgvRows("igvFliNRowIndex"); } set { ViewState["igvFliNRowIndex"] = value; } }
        private int iGetNgvRows(string suNgvRows)
        {
            if (ViewState[suNgvRows] is null)
            {
                return 0;
            }
            else
            {
                return (int)ViewState[suNgvRows];
            }
        }
        #endregion
        private string sYearMonth { get { return (string)ViewState["sYearMonth"] ?? ""; } set { ViewState["sYearMonth"] = value; } }

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
                        case "DailyFlightLog":
                            // Delete one daily flight log
                            StatistDailyFlightLogDataContext dc = new StatistDailyFlightLogDataContext();
                            var l = (from w in dc.DAILYFLIGHTLOGs where w.ID == Int32.Parse(btn.CommandArgument) select w).First();
                            dc.DAILYFLIGHTLOGs.DeleteOnSubmit(l);
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
                                Fill_gvYearMonths();
                                //FillLogListDataTable();
                            }
                            break;
                    }
                }
                if ((btn.ID == "OkButton") && (btn.CommandName == "AfterPosting"))
                {
                    Session["iFlightLog"] = iFlightLog;
                    Server.Transfer("FlightLogRows.aspx");
                }
            }
            catch (Exception exc)
            {
                ProcessPopupException(new Global.excToPopup(exc.Message));
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
                if (!(Session["iFlightLog"] is null))
                {
                    iFlightLog = (int)Session["iFlightLog"];
                    if (iFlightLog < 0)
                    {
                        // FlightLogRows.aspx.cs signalled that a flight log should be posted
                        iFlightLog = Math.Abs(iFlightLog);
                        PostAFlightLog();
                    }
                }
                Fill_gvYearMonths();
            }
        }

        private void PostAFlightLog()
        {
            Session["iFlightLog"] = iFlightLog;
            Response.Redirect("FlightLogPost.aspx");
        }

        private void Fill_gvYearMonths()
        {
            StatistDailyFlightLogDataContext dc = new StatistDailyFlightLogDataContext();
            var qf = (from f in dc.TNPV_FlightLogsYearMonths select f).ToList();
            gvYearMonths.DataSource = qf;
            gvYearMonths.DataBind();
            if (qf.Count > 0 && sYearMonth.Length < 1)
            {
                sYearMonth = qf.Last().YearMonth;
            }
            if (iFlightLog > 0)
            {
                DateTimeOffset Df = (from d in dc.DAILYFLIGHTLOGs where d.ID == iFlightLog select d.DFlightOps).First();
                sYearMonth = CustFmt.sFmtDate(Df, CustFmt.enDFmt.YearSlashMonth);
            }
            FillLogListDataTable();
            AccordionFlightLogInput.SelectedIndex = 2;
        }

        protected void gvYearMonths_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            GridViewRow row = gvYearMonths.Rows[Convert.ToInt32(e.CommandArgument)];
            sYearMonth = ((Label)row.FindControl("lblIYearMonth")).Text;
            FillLogListDataTable();
            AccordionFlightLogInput.SelectedIndex = 2;
        }

        #region Daily Flight Logs

        private void FillLogListDataTable()
        {
            DataTable dtFilters = new DataTable();
            DataColumn DatCol = new DataColumn("Year", Type.GetType("System.Int32"));
            dtFilters.Columns.Add(DatCol);
            DatCol = new DataColumn("Month", Type.GetType("System.Int32"));
            dtFilters.Columns.Add(DatCol);
            DataRow DatRow = dtFilters.NewRow();
            if (sYearMonth.Length > 0)
            {
                string[] saYearMonth = sYearMonth.Split('/');
                DatRow["Year"] = Int32.Parse(saYearMonth[0]);
                DatRow["Month"] = Int32.Parse(saYearMonth[1]);
            }
            else
            {
                DatRow["Year"] = 2020;
                DatRow["Month"] = 1;
            }
            dtFilters.Rows.Add(DatRow);
            List<DataRow> liLogList = null;
            try
            {
                liLogList = AssistLi.Init(Global.enLL.DailyFlightLogs, dtFilters);
            }
            catch (Global.excToPopup exc)
            {
                ProcessPopupException(exc);
                return;
            }
            Session["liLogList"] = liLogList;
            DataTable dtLogList = liLogList.CopyToDataTable();
            iNgvDailyFlRows = dtLogList.Rows.Count; // number of rows in gvDailyFL
            ImageClickEventArgs gvee = new ImageClickEventArgs(iNgvDailyFlRows - 1, -1);
            pbEdit_Click(null, gvee);
        }

        private void dt_BindToGV(GridView gvu, DataTable dtu)
        {
            gvu.DataSource = dtu;
            gvu.DataBind(); // Causes calls to gvRewards_RowDataBound for each GridViewRow, including the header row
        }

        protected void gvDailyFL_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if ((e.Row.RowState & DataControlRowState.Edit) != 0)
                //Prepare one row for editing
                {
                    StatistDailyFlightLogDataContext stdc = new StatistDailyFlightLogDataContext();

                    DropDownList DDLDiMainTowEquip = (DropDownList)e.Row.FindControl("DDLDiMainTowEquip");
                    var q0 = from d in stdc.EQUIPMENTs orderby d.sShortEquipName select d;
                    DDLDiMainTowEquip.DataSource = q0;
                    DDLDiMainTowEquip.DataValueField = "ID";
                    DDLDiMainTowEquip.DataTextField = "sShortEquipName";
                    DDLDiMainTowEquip.DataBind();
                    Set_DropDown_ByValue(DDLDiMainTowEquip, DataBinder.Eval(e.Row.DataItem, "iMainTowEquip").ToString());

                    DropDownList DDLDiMainTowOp = (DropDownList)e.Row.FindControl("DDLDiMainTowOp");
                    DateTime DOFliteLog = DateTime.Now;
                    var q1 = from P in stdc.sp_PeopleWhoCanTow(DOFliteLog) select new { P.ID, sDisplayName = Server.HtmlDecode(P.sDisplayName) };
                    DDLDiMainTowOp.DataSource = q1;
                    DDLDiMainTowOp.DataValueField = "ID";
                    DDLDiMainTowOp.DataTextField = "sDisplayName";
                    DDLDiMainTowOp.DataBind();
                    Set_DropDown_ByValue(DDLDiMainTowOp, DataBinder.Eval(e.Row.DataItem, "iMainTowOp").ToString());

                    DropDownList DDLDiMainGlider = (DropDownList)e.Row.FindControl("DDLDiMainGlider");
                    var q3 = from d in stdc.EQUIPMENTs orderby d.sShortEquipName select d;
                    DDLDiMainGlider.DataSource = q3;
                    DDLDiMainGlider.DataValueField = "ID";
                    DDLDiMainGlider.DataTextField = "sShortEquipName";
                    DDLDiMainGlider.DataBind();
                    Set_DropDown_ByValue(DDLDiMainGlider, DataBinder.Eval(e.Row.DataItem, "iMainGlider").ToString());

                    DropDownList DDLDiMainLaunchMethod = (DropDownList)e.Row.FindControl("DDLDiMainLaunchMethod");
                    var q4 = from d in stdc.LAUNCHMETHODs orderby d.sLaunchMethod select d;
                    DDLDiMainLaunchMethod.DataSource = q4;
                    DDLDiMainLaunchMethod.DataValueField = "ID";
                    DDLDiMainLaunchMethod.DataTextField = "sLaunchMethod";
                    DDLDiMainLaunchMethod.DataBind();
                    Set_DropDown_ByValue(DDLDiMainLaunchMethod, DataBinder.Eval(e.Row.DataItem, "iMainLaunchMethod").ToString());
                    
                    DropDownList DDLDiMainLocation = (DropDownList)e.Row.FindControl("DDLDiMainLocation");
                    var q2 = from L in stdc.LOCATIONs orderby L.sLocation select L;
                    DDLDiMainLocation.DataSource = q2;
                    DDLDiMainLocation.DataValueField = "ID";
                    DDLDiMainLocation.DataTextField = "sLocation";
                    DDLDiMainLocation.DataBind();
                    Set_DropDown_ByValue(DDLDiMainLocation, DataBinder.Eval(e.Row.DataItem, "iMainLocation").ToString());

                    // Only the Update and Cancel buttons are visible; Edit ImageButton already not visible because it only appears in the ItemTemplate - here we are in the EditItemTemplate
                    Button pbSelect = (Button)e.Row.FindControl("pbSelect");
                    pbSelect.Visible = false;
                    Button pbPost = (Button)e.Row.FindControl("pbPost");
                    pbPost.Visible = false;
                    Button pbDelete = (Button)e.Row.FindControl("pbDelete");
                    pbDelete.Visible = false;

                }
                else
                {
                    // Preparing a row that is not being edited

                    // Handle visibility of the Post button
                    int iFlCt = Convert.ToInt32(((Label)e.Row.FindControl("lblIFlCt")).Text);
                    int iFLPosted = Convert.ToInt32(((Label)e.Row.FindControl("lblIFlPosted")).Text);
                    if ((iFLPosted < iFlCt) && (iFlCt > 0))
                    {
                        // The Post button is visible
                        foreach (Control ctrl in e.Row.Cells[gvDailyFL.Columns.Count - 2].Controls)
                        {
                            ctrl.Visible = true;
                        }
                    }
                    else
                    {
                        // The Post button is invisible
                        foreach(Control ctrl in e.Row.Cells[gvDailyFL.Columns.Count - 2].Controls)
                        {
                            ctrl.Visible = false;
                        }
                    }

                    if (bEditExistingDailyFLRow)
                    {
                        // No buttons are visible in rows that are not being edited
                        ImageButton pbEdit = (ImageButton)e.Row.FindControl("pbEdit");
                        pbEdit.Visible = false;
                        Button pbSelect = (Button)e.Row.FindControl("pbSelect");
                        pbSelect.Visible = false;
                        Button pbPost = (Button)e.Row.FindControl("pbPost");
                        pbPost.Visible = false;
                        Button pbDelete = (Button)e.Row.FindControl("pbDelete");
                        pbDelete.Visible = false;
                    }
                }
            }
        }

        protected void gvDailyFL_DataBound(object sender, EventArgs e)
        {
            int iColAddButton = gvDailyFL.Columns.Count - 4; // Column of the Edit and Add buttons
            if (!bEditExistingDailyFLRow)
            {
                // Last row has no button except the Edit button:
                GridViewRow gvr = gvDailyFL.Rows[gvDailyFL.Rows.Count - 1];
                // Last row has an Add button
                ImageButton pb = (ImageButton)gvr.FindControl("pbUpdate");
                pb.ImageUrl = "~/i/YellowAddButton.jpg";

                // Make the last row (the Add row) stand out
                gvr.BackColor = System.Drawing.Color.LightGray;
                gvr.BorderStyle = BorderStyle.Ridge;
                gvr.BorderWidth = 5;
                gvr.BorderColor = System.Drawing.Color.Orange;
                gvr.Cells[iColAddButton].BorderStyle = BorderStyle.Ridge;
                gvr.Cells[iColAddButton].BorderWidth = 5;
                gvr.Cells[iColAddButton].BorderColor = System.Drawing.Color.Orange;

                // No other buttons are visible in the last row
                ImageButton pbCancel = (ImageButton)gvr.FindControl("pbCancel");
                pbCancel.Visible = false;
                Button pbSelect = (Button)gvr.FindControl("pbSelect");
                pbSelect.Visible = false;
                Button pbPost = (Button)gvr.FindControl("pbPost");
                pbPost.Visible = false;
                Button pbDelete = (Button)gvr.FindControl("pbDelete");
                pbDelete.Visible = false;
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

        protected void pbEdit_Click(object sender, ImageClickEventArgs e)
        {
            int iRowIndex;
            if (sender is null)
            {
                iRowIndex = e.X;
            }
            else
            {
                ImageButton pbEdit = (ImageButton)sender;
                GridViewRow selectedRow = (GridViewRow)pbEdit.NamingContainer;
                iRowIndex = selectedRow.RowIndex;
            }
            List<DataRow> liLogList = (List<DataRow>)Session["liLogList"];
            gvDailyFL.EditIndex = iRowIndex;
            bEditExistingDailyFLRow = false;
            if (iRowIndex < iNgvDailyFlRows - 1)
            {
                // We are editing an existing row; need to get rid of the last row which would be the New row (but there is no New row)
                liLogList.RemoveAt(liLogList.Count - 1);
                bEditExistingDailyFLRow = true;
            }
            DataTable dtLogList = liLogList.CopyToDataTable();
            dt_BindToGV(gvDailyFL, dtLogList);
        }

        protected void pbUpdate_Click(object sender, ImageClickEventArgs e)
        {
            ImageButton pbUpdate = (ImageButton)sender;
            GridViewRow selectedRow = (GridViewRow)pbUpdate.NamingContainer;
            int iRowIndex = selectedRow.RowIndex;
            string sLog = "";
            DAILYFLIGHTLOG dfl = new DAILYFLIGHTLOG();
            ActivityLog.enumLogTypes elt = ActivityLog.enumLogTypes.DataInsert;
            StatistDailyFlightLogDataContext dc = new StatistDailyFlightLogDataContext();
            int iLast = iNgvDailyFlRows - 1; // index to last row (regardless of whether a row was trimmed off in FillDataTable()).
            TextBox txbDDFlightOps = (TextBox)gvDailyFL.Rows[iRowIndex].FindControl("txbDDFlightOps");
            TextBox txbsFldMgrs = (TextBox)gvDailyFL.Rows[iRowIndex].FindControl("txbsFldMgrs");
            DropDownList DDLDiMainTowEquip = (DropDownList)gvDailyFL.Rows[iRowIndex].FindControl("DDLDiMainTowEquip");
            int iMainTowEquip = Int32.Parse(DDLDiMainTowEquip.SelectedItem.Value);
            DropDownList DDLDiMainTowOp = (DropDownList)gvDailyFL.Rows[iRowIndex].FindControl("DDLDiMainTowOp");
            int iMainTowOp = Int32.Parse(DDLDiMainTowOp.SelectedItem.Value);
            DropDownList DDLDiMainGlider = (DropDownList)gvDailyFL.Rows[iRowIndex].FindControl("DDLDiMainGlider");
            int iMainGlider = Int32.Parse(DDLDiMainGlider.SelectedItem.Value);
            DropDownList DDLDiMainLaunchMethod = (DropDownList)gvDailyFL.Rows[iRowIndex].FindControl("DDLDiMainLaunchMethod");
            int iMainLaunchMethod = Int32.Parse(DDLDiMainLaunchMethod.SelectedItem.Value);
            DropDownList DDLDiMainLocation = (DropDownList)gvDailyFL.Rows[iRowIndex].FindControl("DDLDiMainLocation");
            int iMainLocation = Int32.Parse(DDLDiMainLocation.SelectedItem.Value);
            TextBox txbDNotes = (TextBox)gvDailyFL.Rows[iRowIndex].FindControl("txbDNotes");
            if (iRowIndex == iLast)
            {
                // Adding a new record
                dfl.PiTRecordEntered = DateTimeOffset.UtcNow;
                dfl.iRecordEnteredBy = mCRUD.GetPeopleIDfromWebSiteUserName(HttpContext.Current.User.Identity.Name);
                dfl.DFlightOps = DateTimeOffset.Parse(txbDDFlightOps.Text);
                dfl.sFldMgr = txbsFldMgrs.Text.Trim().Replace("'", "`");
                dfl.iMainTowEquip = iMainTowEquip;
                dfl.iMainGlider = iMainGlider;
                dfl.iMainLaunchMethod = iMainLaunchMethod;
                dfl.iMainTowOp = iMainTowOp;
                dfl.iMainLocation = iMainLocation;
                sLog = Server.HtmlEncode(txbDNotes.Text.Trim().Replace("'", "`"));
                dfl.sNotes = sLog;
                dc.DAILYFLIGHTLOGs.InsertOnSubmit(dfl);

                DataTable dt = AssistLi.dtSchema(Global.enLL.DailyFlightLogs);
                DataRow dtRow = dt.NewRow();
                dtRow[(int)Global.enPFDailyFlightLogs.ID] = 0;
                dtRow[(int)Global.enPFDailyFlightLogs.DFlightOps] = dfl.DFlightOps;
                dtRow[(int)Global.enPFDailyFlightLogs.sFldMgr] = dfl.sFldMgr;
                dtRow[(int)Global.enPFDailyFlightLogs.iMainTowEquip] = dfl.iMainTowEquip;
                dtRow[(int)Global.enPFDailyFlightLogs.iMainTowOp] = dfl.iMainTowOp;
                dtRow[(int)Global.enPFDailyFlightLogs.iMainGlider] = dfl.iMainGlider;
                dtRow[(int)Global.enPFDailyFlightLogs.iMainLaunchMethod] = dfl.iMainLaunchMethod;
                dtRow[(int)Global.enPFDailyFlightLogs.iMainLocation] = dfl.iMainLocation;
                dtRow[(int)Global.enPFDailyFlightLogs.sNotes] = dfl.sNotes;

                DataTable dtLastUsedIn = new DataTable("LastUsedIn");
                AssistLi.LastUsedInputs_DailyFlightLogs("1", DDLDiMainTowEquip.SelectedItem.Text, DDLDiMainTowOp.SelectedItem.Text,
                    DDLDiMainGlider.SelectedItem.Text, DDLDiMainLaunchMethod.SelectedItem.Text, DDLDiMainLocation.SelectedItem.Text, ref dtLastUsedIn);
                AccountProfile.CurrentUser.DailyFlightLogsDefaults = dtLastUsedIn;
            }
            else
            {
                // Editing an existing record
                List<DataRow> liLogList = (List<DataRow>)Session["liLogList"];
                DataRow dr = liLogList[iRowIndex];
                dfl = (from v in dc.DAILYFLIGHTLOGs where v.ID == (int)dr.ItemArray[0] select v).First();
                dfl.PiTRecordEntered = DateTimeOffset.UtcNow;
                dfl.iRecordEnteredBy = mCRUD.GetPeopleIDfromWebSiteUserName(HttpContext.Current.User.Identity.Name);
                dfl.DFlightOps = DateTimeOffset.Parse(txbDDFlightOps.Text);
                dfl.sFldMgr = txbsFldMgrs.Text.Trim().Replace("'", "`");
                dfl.iMainTowEquip = iMainTowEquip;
                dfl.iMainTowOp = iMainTowOp;
                dfl.iMainGlider = iMainGlider;
                dfl.iMainLaunchMethod = iMainLaunchMethod;
                dfl.iMainLocation = iMainLocation;
                sLog = Server.HtmlEncode(txbDNotes.Text.Trim().Replace("'", "`"));
                dfl.sNotes = sLog;
                elt = ActivityLog.enumLogTypes.DataUpdate;
            }
            try
            {
                dc.SubmitChanges();
                sLog = "DFlightOps=" + TSoar.CustFmt.sFmtDate(dfl.DFlightOps, CustFmt.enDFmt.DateOnly) + ", sFldMgr=" + dfl.sFldMgr + ", iMainTowEquip=" +
                    dfl.iMainTowEquip.ToString() + ", iMainTowOp=" + dfl.iMainTowOp.ToString() + ", iMainGlider=" + dfl.iMainGlider.ToString()
                    + ", iMainLaunchMethod=" + dfl.iMainLaunchMethod.ToString() + ", iMainLocation=" + dfl.iMainLocation.ToString() + ", sNotes=" + sLog;
                ActivityLog.oLog(elt, 1, "DailyFlightLogs: " + sLog);
            }
            catch (Exception exc)
            {
                Global.excToPopup ex = new Global.excToPopup(exc.Message);
                ProcessPopupException(ex);
                return;
            }
            Fill_gvYearMonths();
            //FillLogListDataTable();
        }

        protected void pbCancel_Click(object sender, ImageClickEventArgs e)
        {
            FillLogListDataTable();
        }

        protected void pbSelect_Click(object sender, EventArgs e)
        {
            Button pbSelect = (Button)sender;
            GridViewRow selectedRow = (GridViewRow)pbSelect.NamingContainer;
            string sFlID = ((Label)selectedRow.FindControl("lblIID")).Text;
            iFlightLog = Convert.ToInt32(sFlID);
            Session["iFlightLog"] = iFlightLog;
            Server.Transfer("FlightLogRows.aspx");
        }

        protected void pbPost_Click(object sender, EventArgs e)
        {
            Button pbPost = (Button)sender;
            GridViewRow selectedRow = (GridViewRow)pbPost.NamingContainer;
            string sFlID = ((Label)selectedRow.FindControl("lblIID")).Text;
            iFlightLog = Convert.ToInt32(sFlID);
            PostAFlightLog();
        }

        protected void pbDelete_Click(object sender, EventArgs e)
        {
            Button pbDelete = (Button)sender;
            GridViewRow selectedRow = (GridViewRow)pbDelete.NamingContainer;
            Label lblItem = (Label)selectedRow.FindControl("lblIID");
            string sItem = lblItem.Text;
            ButtonsClear();
            YesButton.CommandName = "Delete";
            YesButton.CommandArgument = sItem;
            OkButton.CommandArgument = "DailyFlightLog";
            lblPopupText.Text = "Please confirm deletion of daily flight log with ID = '" + sItem +
                "' (This action deletes all the detail rows in this flight log as well, and cannot be reversed)";
            MPE_Show(Global.enumButtons.NoYes);
        }

        #endregion
    }
}