using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TSoar.Accounting.FinDetails.SalesAR
{
    public partial class MinFlyChrg : System.Web.UI.Page
    {
        private const string scDateOnlyPattern = "yyyy-MM-dd";
        public Dictionary<int, string> dictMembCategs;
        public Dictionary<int, string> dictPerson;
        public Dictionary<char, string> dictTypeOfAmount;

        private TSoar.DB.SCUD_Multi mCrud = new TSoar.DB.SCUD_Multi();

        #region Properties
        #region String Properties
        private string sMembCategFCE { get { return (string)ViewState["sMembCategFCE"] ?? mCrud.GetSetting("DefaultMembCategMFC"); } set { ViewState["sMembCategFCE"] = value; } }
        private string sPersonFCE { get { return (string)ViewState["sPersonFCE"] ?? sGetDefaultPersonFCE(); } set { ViewState["sPersonFCE"] = value; } }
        private string sGetDefaultPersonFCE()
        {
            FlyActInvoiceDataContext fdc = new FlyActInvoiceDataContext();
            return (from m in fdc.MEMBERFROMTOs where m.MEMBERSHIPCATEGORy.sMembershipCategory == mCrud.GetSetting("DefaultMembCategMFC") select m.PEOPLE.sDisplayName).First();
        }
        #endregion
        #region Integer Properties
        private int iPersonFCE { get { return iGet("iPersonFCE"); } set { ViewState["iPersonFCE"] = value; } }
        private int ipMembCateg { get { return iGet("ipMembCateg"); } set { ViewState["ipMembCateg"] = value; } }
        private int ivNRows { get { return iGet("ivNRows"); } set { ViewState["ivNRows"] = value; } }
        private int ivEditIndex { get { return iGet("ivEditIndex"); } set { ViewState["ivEditIndex"] = value; } }
        private int iGet(string su)
        {
            if (ViewState[su] == null)
            {
                switch (su)
                {
                    case "iPersonFCE":
                        FlyActInvoiceDataContext fdc = new FlyActInvoiceDataContext();
                        return (from m in fdc.MEMBERFROMTOs where m.MEMBERSHIPCATEGORy.sMembershipCategory == mCrud.GetSetting("DefaultMembCategMFC") select m.PEOPLE.ID).First();
                    case "ipMembCateg":
                        fdc = new FlyActInvoiceDataContext();
                        return (from m in fdc.MEMBERFROMTOs where m.MEMBERSHIPCATEGORy.sMembershipCategory == mCrud.GetSetting("DefaultMembCategMFC") select m.MEMBERSHIPCATEGORy.ID).First();
                    case "ivEditIndex":
                        return -1;
                    default:
                        return 0;
                }
            }
            else
            {
                return (int)ViewState[su];
            }
        }
        #endregion
        #region Boolean Properties
        private bool bManualRefresh
        {
            get { return bGet("bManualRefresh"); }
            set { ViewState["bManualRefresh"] = value; }
        }
        private bool bEditExisting
        {
            get { return bGet("bEditExisting"); }
            set { ViewState["bEditExisting"] = value; }
        }
        private bool bGet(string su)
        {
            if (ViewState[su] == null)
            {
                return false;
            }
            else
            {
                return (bool)ViewState[su];
            }
        }
        #endregion
        #endregion
        public string sDcyfl(string sFirstLast)
        {
            DateTime DJetzt = DateTime.Now;
            DateTime DD = sFirstLast == "first" ? new DateTime(DJetzt.Year, 1, 1) : new DateTime(DJetzt.Year, 12, 31);
            return CustFmt.sFmtDate(DD,CustFmt.enDFmt.DateOnly).Replace("/","-");
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
            try
            {
                if ((btn.ID == "YesButton") && (btn.CommandName == "Delete"))
                {
                    switch (OkButton.CommandArgument)
                    {
                        case "MFCP":
                            // Delete the record
                            FlyActInvoiceDataContext fdc = new FlyActInvoiceDataContext();
                            var f = (from v in fdc.MINFLYCHGPARs where v.ID == Int32.Parse(btn.CommandArgument) select v).First();
                            fdc.MINFLYCHGPARs.DeleteOnSubmit(f);
                            try
                            {
                                fdc.SubmitChanges();
                            }
                            catch (Exception exc)
                            {
                                Global.excToPopup ex = new Global.excToPopup(exc.Message);
                                ProcessPopupException(ex);
                            }
                            finally
                            {
                                FillDataTableMFCPed();
                            }
                            break;
                        case "FCE":
                            // Delete the record
                            fdc = new FlyActInvoiceDataContext();
                            var g = (from v in fdc.FLYINGCHARGEs where v.ID == Int32.Parse(btn.CommandArgument) select v).First();
                            fdc.FLYINGCHARGEs.DeleteOnSubmit(g);
                            try
                            {
                                fdc.SubmitChanges();
                            }
                            catch (Exception exc)
                            {
                                Global.excToPopup ex = new Global.excToPopup(exc.Message);
                                ProcessPopupException(ex);
                            }
                            finally
                            {
                                FillDataTableFCedit(-1);
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

        #region Modal Popup Context Menu for generating MFCs
        private void ButtonsClearCM()
        {
            pbDoMFC.CommandArgument = "";
            pbDoMFC.CommandName = "";
            pbDoMFC.Visible = true;
            pbRemoveMFC.CommandArgument = "";
            pbRemoveMFC.CommandName = "";
            pbRemoveMFC.Visible = true;
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
            int iMFCPID = Int32.Parse(btn.CommandName);
            try
            {
                switch (btn.ID)
                {
                    case "pbDoMFC":
                        DoMFC(iMFCPID);
                        break;
                    case "pbRemoveMFC":
                        RemoveMFC(iMFCPID);
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
        #region Page
        protected void Page_PreInit(object sender, EventArgs e)
        {
            FlyActInvoiceDataContext dc = new FlyActInvoiceDataContext();

            dictMembCategs = new Dictionary<int, string>();
            var qy = from y in dc.MEMBERSHIPCATEGORies select y;
            foreach (var y in qy)
            {
                dictMembCategs.Add(y.ID, y.sMembershipCategory);
            }

            dictPerson = new Dictionary<int, string>();
            var qp = from p in dc.PEOPLEs select p;
            foreach (var p in qp)
            {
                dictPerson.Add(p.ID, p.sDisplayName);
            }

            dictTypeOfAmount = new Dictionary<char, string>();
            dictTypeOfAmount.Add('A', "Actual");
            dictTypeOfAmount.Add('B', "Billed");
            dictTypeOfAmount.Add('M', "Min Fly Chrg MFC");
            dictTypeOfAmount.Add('T', "MFC Tow Pilots/Instructors");

        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {

                gvFCedit.PageSize = Int32.Parse(mCrud.GetSetting("PageSizeMinFlyChrgEdit"));
                FillDataTableMFCPed();
                FillDataTableMFCGen();
                PreviousMonth();
                txbDFromFCE.Text = sDcyfl("first");
                txbDToFCE.Text = sDcyfl("last");
            }
        }

        protected void Page_LoadComplete(object sender, EventArgs e)
        {
            if (bManualRefresh || !IsPostBack)
            {
                SqlDataSrc_Person.SelectCommand = sPersonSelectCommand();
                bManualRefresh = false;
                // Only now should we make this call:
                FillDataTableFCedit(-1);
            }
        }
        #endregion
        private string sPersonSelectCommand()
        {
            return "SELECT DISTINCT P.ID, P.sDisplayName " +
                        "FROM MEMBERSHIPCATEGORIES C " +
                            "INNER JOIN MEMBERFROMTO F ON C.ID = F.iMemberCategory " +
                            "INNER JOIN PEOPLE P ON F.iPerson = P.ID " +
                        "WHERE (C.ID = " + ipMembCateg.ToString() + ") ORDER BY P.sDisplayName";
        }
        #region DropDownLists
        protected void DDL_PreRender(object sender, EventArgs e)
        {
            DropDownList ddl = (DropDownList)sender;
            string sText = "";
            FlyActInvoiceDataContext dc = new FlyActInvoiceDataContext();
            int ii;
            switch (ddl.ID)
            {
                #region in Minimum Monthly Flying Charges - Parameters
                case "DDLMembCateg": // This DropDownList is embedded in one of the rows of GridView gvMFCPars
                    if (gvMFCPars.EditIndex < 0)
                    {
                        sText = "}{"; // no editing is taking place in this GridView
                    }
                    else
                    {
                        ii = Int32.Parse(((Label)gvMFCPars.Rows[gvMFCPars.EditIndex].FindControl("lblIIdent")).Text); // ID of MINFLYCHRGPARS record
                        if (ii == 0)
                        {
                            sText = mCrud.GetSetting("DefaultMembCategMFC");
                        }
                        else
                        {
                            ii = (from f in dc.MINFLYCHGPARs where f.ID == ii select f.iMembershipCategory).First(); // ID of MEMBERSHIPCATEGORIES record
                            sText = dictMembCategs[ii];
                        }
                    }
                    break;
                #endregion

                #region in Flying Charges Editing
                case "DDLMembCategFCE": // This DropDownList is outside of any GridView
                    sText = sMembCategFCE;
                    break;
                case "DDLPerson": // in Manually editing minimum and actual flying charges. This DropDownList is outside of any GridView.
                    sText = sPersonFCE;
                    break;
                case "DDLType": // Type of FLYINGCHARGE amount in Manually editing minimum and actual flying charges. This DropDownList is inside GridView gvFCedit.
                    if (gvFCedit.EditIndex < 0)
                    {
                        sText = "}{"; // no editing is taking place in this GridView
                    }
                    else
                    {
                        ii = Int32.Parse(((Label)gvFCedit.Rows[gvFCedit.EditIndex].FindControl("lblIFCIdent")).Text); // ID of FLYINGCHARGES record
                        if (ii == 0)
                        {
                            sText = "Actual";
                        }
                        else
                        {
                            char cc = (from f in dc.FLYINGCHARGEs where f.ID == ii select f.cTypeOfAmount).First();
                            int ic = "ABMT".IndexOf(cc); // The 'values' in the list items of dropdownlist DDLType
                            sText = ddl.Items[ic].Text;
                        }
                    }
                    break;
                #endregion
            }
            if (sText != "}{")
            {
                SetDropDownByText(ddl, sText);
            }
        }

        private void SetDropDownByText(DropDownList ddl, string sText)
        {
            if (ddl.Items.Count > 0)
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
        }

        protected void DDLMembCategFCE_SelectedIndexChanged(object sender, EventArgs e)
        {
            DropDownList ddl = (DropDownList)sender;
            sMembCategFCE = ddl.SelectedItem.Text;
            ipMembCateg = Int32.Parse(ddl.SelectedValue);
            SqlDataSrc_Person.SelectCommand = sPersonSelectCommand();
            DDLPerson.DataBind();
            if (DDLPerson.Items.Count > 0)
            {
                DDLPerson.SelectedIndex = 0;
                iPersonFCE = Int32.Parse(DDLPerson.SelectedValue);
                sPersonFCE = DDLPerson.SelectedItem.Text;
            }
            bManualRefresh = true;
        }

        protected void DDLPerson_SelectedIndexChanged(object sender, EventArgs e)
        {
            DropDownList ddl = (DropDownList)sender;
            sPersonFCE = ddl.SelectedItem.Text;
            iPersonFCE = Int32.Parse(ddl.SelectedItem.Value);
            FillDataTableFCedit(-1);
        }
        #endregion
        #region Minimum Flying Charges Parameters

        private void FillDataTableMFCPed() // Fill the data table for editing the MFC parameters
        {
            List<DataRow> liMinFlyChrgs = AssistLi.Init(Global.enLL.MinFlyChrgs);
            Session["liMinFlyChrgs"] = liMinFlyChrgs;
            DataTable dtMinFlyChrgs = liMinFlyChrgs.CopyToDataTable();
            GridViewEditEventArgs gvee = new GridViewEditEventArgs(dtMinFlyChrgs.Rows.Count - 1);
            gvMFCPars_RowEditing(null, gvee);
        }

        protected void gvMFCPars_RowDataBound(object sender, GridViewRowEventArgs e)
        {

        }

        protected void gvMFCPars_RowEditing(object sender, GridViewEditEventArgs e)
        {
            gvMFCPars.EditIndex = e.NewEditIndex;
            List<DataRow> liMinFlyChrgs = (List<DataRow>)Session["liMinFlyChrgs"];
            DataTable dtMinFlyChrgs = liMinFlyChrgs.CopyToDataTable();
            dt_BindTogvMFCPars(dtMinFlyChrgs);
        }

        private void dt_BindTogvMFCPars(DataTable dtu)
        {
            GridView GV = gvMFCPars;
            DataView view = new DataView(dtu);
            view.Sort = "i1Sort ASC, DFrom ASC";
            GV.DataSource = view;
            GV.DataBind();
            int iLast = GV.Rows.Count - 1; // The standard editing row
            // Last row has no button except the Edit button:
            GV.Rows[iLast].Cells[GV.Columns.Count - 1].Visible = false;
            // Last row has an Add button
            int iColAddButton = GV.Columns.Count - 2;
            // Execute this code only when a row is being edited
            if (GV.EditIndex > -1)
            {
                if (GV.EditIndex == iLast)
                {
                    ImageButton pb = (ImageButton)GV.Rows[iLast].FindControl("ipbEUpdate");
                    pb.ImageUrl = "~/i/YellowAddButton.jpg";
                    GV.Rows[iLast].BackColor = System.Drawing.Color.LightGray;
                    GV.Rows[iLast].BorderStyle = BorderStyle.Ridge;
                    GV.Rows[iLast].BorderWidth = 5;
                    GV.Rows[iLast].BorderColor = System.Drawing.Color.Orange;
                    GV.Rows[iLast].Cells[iColAddButton].BorderStyle = BorderStyle.Ridge;
                    GV.Rows[iLast].Cells[iColAddButton].BorderWidth = 5;
                    GV.Rows[iLast].Cells[iColAddButton].BorderColor = System.Drawing.Color.Orange;

                    // No Cancel button in the last row
                    ImageButton pbC = (ImageButton)GV.Rows[GV.EditIndex].FindControl("ipbECancel");
                    pbC.Visible = false;
                }
                else
                {
                    // If we are editing a row other than the last, only the Update and Cancel buttons are allowed:
                    for (int iRow = 0; iRow <= iLast; iRow++)
                    {
                        if (iRow != GV.EditIndex)
                        {
                            ((ImageButton)GV.Rows[iRow].FindControl("ipbEEdit")).Visible = false;
                        }
                        ((ImageButton)GV.Rows[iRow].FindControl("ipbEDelete")).Visible = false;
                    }
                }
            }

            CheckOnOverlaps();
        }

        private void CheckOnOverlaps()
        {
            FlyActInvoiceDataContext fdc = new FlyActInvoiceDataContext();
            var qo = (from o in fdc.spCheck4MFCOverlap() select o).ToList();
            if (qo.Count > 0)
            {
                string sErr = qo[0].Membership_Category + ": " + qo[0].From_1 + " to " + qo[0].To_1 + ", and " + qo[0].From_2 + " to " + qo[0].To_2;
                string sMsg = "Error: There are " + qo.Count + " overlapping time intervals for the same membership category; first overlap: " + sErr;
                ProcessPopupException(new Global.excToPopup(sMsg));
            }
        }

        protected void gvMFCPars_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            Label lblItem = (Label)gvMFCPars.Rows[e.RowIndex].FindControl("lblIIdent");
            string sItem = lblItem.Text;
            ButtonsClear();
            YesButton.CommandName = "Delete";
            YesButton.CommandArgument = sItem;
            OkButton.CommandArgument = "MFCP";
            lblPopupText.Text =
                "Please confirm deletion of Minimum Flying Charge Parameters record with ID " + sItem;
            MPE_Show(Global.enumButtons.NoYes);
        }

        protected void gvMFCPars_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvMFCPars.EditIndex = -1;
            FillDataTableMFCPed();
        }

        protected void gvMFCPars_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            FlyActInvoiceDataContext dc = new FlyActInvoiceDataContext();
            int iLast = gvMFCPars.Rows.Count - 1;
            DateTimeOffset DFrom = DateTimeOffset.Parse(((TextBox)gvMFCPars.Rows[e.RowIndex].FindControl("txbDFrom")).Text);
            DateTimeOffset DTo = DateTimeOffset.Parse(((TextBox)gvMFCPars.Rows[e.RowIndex].FindControl("txbDTo")).Text);
            if (DFrom >= DTo)
            {
                ProcessPopupException(new Global.excToPopup("Error: The 'From' date `" + DFrom.ToString() + "` has to be earlier than the 'To' Date `" + DTo.ToString() + "`."));
                return;
            }
            int iMC = Int32.Parse(((DropDownList)gvMFCPars.Rows[e.RowIndex].FindControl("DDLMembCateg")).SelectedItem.Value); // 'Value' contains the ID of a row in table MEMBERSHIPCATEGORIES (not the same as SelectedIndex!!)
            decimal mMinMonthlyFlyChrg = Decimal.Parse(((TextBox)gvMFCPars.Rows[e.RowIndex].FindControl("txbMinMonthlyFlyChrg")).Text);
            string sComments = ((TextBox)gvMFCPars.Rows[e.RowIndex].FindControl("txbDComments")).Text;
            //TSoar.DB.SCUD_Multi mCrud = new TSoar.DB.SCUD_Multi();
            int iUser = mCrud.GetPeopleIDfromWebSiteUserName(HttpContext.Current.User.Identity.Name);
            if (e.RowIndex == iLast)
            {
                MINFLYCHGPAR f = new MINFLYCHGPAR();
                f.PiTRecordEntered = DateTime.UtcNow;
                f.iRecordEnteredBy = iUser;
                f.DFrom = DFrom;
                f.DTo = DTo.Date.AddMinutes(1439);
                f.iMembershipCategory = iMC;
                f.mMinMonthlyFlyChrg = mMinMonthlyFlyChrg;
                f.sComments = sComments.Replace("'", "`");

                dc.MINFLYCHGPARs.InsertOnSubmit(f);
            }
            else
            {
                int iID = Int32.Parse(((Label)gvMFCPars.Rows[e.RowIndex].FindControl("lblIIdent")).Text);
                var f = (from v in dc.MINFLYCHGPARs where v.ID == iID select v).First();
                f.PiTRecordEntered = DateTime.UtcNow;
                f.iRecordEnteredBy = iUser;
                f.DFrom = DFrom;
                f.DTo = DTo.Date.AddMinutes(1439);
                f.iMembershipCategory = iMC;
                f.mMinMonthlyFlyChrg = mMinMonthlyFlyChrg;
                f.sComments = sComments.Replace("'", "`");
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
            gvMFCPars.EditIndex = -1;
            FillDataTableMFCPed();
        }

        #endregion

        #region Minimum Flying Charges Generation
        private void FillDataTableMFCGen()
        {
            FlyActInvoiceDataContext fdc = new FlyActInvoiceDataContext();
            var q = from m in fdc.spGetMFCPars() select m;
            gvGenMFC.DataSource = q.ToList();
            gvGenMFC.DataBind();
        }

        protected void gvGenMFC_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int iRow = 0;
            if (int.TryParse((string)e.CommandArgument, out iRow))
            {
                if ((iRow >= 0) && (iRow < gvGenMFC.Rows.Count))
                {
                    ButtonsClearCM(); // CM means context menu
                    // e.CommandArgument contains the row index of the GridView row clicked, as a string
                    pbDoMFC.CommandArgument = (string)e.CommandArgument;
                    pbRemoveMFC.CommandArgument = (string)e.CommandArgument;
                    pbCancel.CommandArgument = (string)e.CommandArgument;
                    // Find the database table MINFLYCHGPARS row identifier, as a string
                    string sMFCPID = ((Label)gvGenMFC.Rows[iRow].FindControl("lblIIdent")).Text;
                    pbDoMFC.CommandName = sMFCPID;
                    pbRemoveMFC.CommandName = sMFCPID;
                    pbCancel.CommandName = sMFCPID;
                    lblMFCPID.Text = sMFCPID; // Show user the transaction ID in the popup
                    MPE_ShowCM();
                }
                else
                {
                    ProcessPopupException(new Global.excToPopup("MinFlyChrg.aspx.cs.gvGenMFC_RowCommand: row number in e.CommandArgument is not between 0 and " +
                        (gvGenMFC.Rows.Count - 1).ToString() + " (pointer to last row in gvGenMFC)"));
                }
            }
        }

        private void DoMFC(int iuMFCPID)
        {
            FlyActInvoiceDataContext fdc = new FlyActInvoiceDataContext();
            var q = (from c in fdc.MINFLYCHGPARs where c.ID == iuMFCPID select new { c.MEMBERSHIPCATEGORy.sMembershipCategory, c.DFrom, c.DTo}).First();
            //TSoar.DB.SCUD_Multi mCrud = new TSoar.DB.SCUD_Multi();
            int iUser = mCrud.GetPeopleIDfromWebSiteUserName(HttpContext.Current.User.Identity.Name);
            string sStatus = "";
            fdc.spGenMFCs(iuMFCPID, iUser, ref sStatus);
            if (sStatus.Substring(0, 2) != "OK")
            {
                ProcessPopupException(new Global.excToPopup("MinFlyChrg.aspx.cs.DoMFC: " + sStatus));
                return;
            }
            int iMembCount = Int32.Parse(sStatus.Substring(2));
            string sMsg = "Minimum Flying Charges have been generated for " + iMembCount.ToString() + " " +
                q.sMembershipCategory + " member(s) from " + q.DFrom.ToString() + " to " + q.DTo.ToString();
            ProcessPopupException(new Global.excToPopup(sMsg));
            ActivityLog.oLog(ActivityLog.enumLogTypes.DataInsert, 0, sMsg);
        }

        private void RemoveMFC(int iuMFCPID)
        {
            FlyActInvoiceDataContext fdc = new FlyActInvoiceDataContext();
            var q = (from c in fdc.MINFLYCHGPARs where c.ID == iuMFCPID select new { c.MEMBERSHIPCATEGORy.sMembershipCategory, c.DFrom, c.DTo }).First();
            string sStatus = "";
            fdc.spRemoveMFCs(iuMFCPID, ref sStatus);
            if (sStatus.Substring(0, 2) != "OK")
            {
                ProcessPopupException(new Global.excToPopup("MinFlyChrg.aspx.cs.RemoveMFC: " + sStatus));
                return;
            }
            string ss = sStatus.Substring(2);
            string[] s1 = ss.Split(',');
            string sMsg = s1[0] + " Minimum Flying Charges have been removed for " + s1[1] + " " +
                q.sMembershipCategory + " member(s) from " + q.DFrom.ToString() + " to " + q.DTo.ToString();
            ProcessPopupException(new Global.excToPopup(sMsg));
            ActivityLog.oLog(ActivityLog.enumLogTypes.DataInsert, 0, sMsg);
        }
        #endregion

        #region Actual Flying Charges Summaries

        protected void pbISPreviousMonth_Click(object sender, EventArgs e)
        {
            PreviousMonth();
        }
        private void PreviousMonth()
        {
            DateTimeOffset heute = DateTimeOffset.Now.LocalDateTime.Date;
            DateTimeOffset DFirstDayofThisMonth = heute.AddDays(-(heute.Day - 1));
            DateTimeOffset DTo = DFirstDayofThisMonth.AddMinutes(-61); // 61 minutes before end of previous month
            txbISDTo.Text = DTo.ToString(scDateOnlyPattern);
            DateTimeOffset DFrom = DFirstDayofThisMonth.AddMonths(-1).AddMinutes(61); // First Day of previous month
            txbISDFrom.Text = DFrom.ToString(scDateOnlyPattern);
        }

        protected void pbISSummarize_Click(object sender, EventArgs e)
        {
            // Since, by convention in TSoar.Accounting.FinDetails.SalesAR.FlyActInvoice.aspx.cs.DoInvoiceLines,
            // one invoice cannot span two months, i.e., invoices cover flying activities for just a single day,
            // summarization for a month and for one aviator consists of adding up the totals of the appropriate invoices.

            // Check that the start and end dates fall on beginning and end of the same month:
            DateTimeOffset DISStart = DateTimeOffset.MinValue;
            DateTimeOffset DISEnd = DateTimeOffset.MinValue;

            #region Validate start/end dates
            if (!DateTimeOffset.TryParse(txbISDFrom.Text, out DISStart))
            {
                ProcessPopupException(new Global.excToPopup("Illegal start date `" + txbISDFrom.Text + "`"));
                return;
            }
            if (!DateTimeOffset.TryParse(txbISDTo.Text, out DISEnd))
            {
                ProcessPopupException(new Global.excToPopup("Illegal end date `" + txbISDTo.Text + "`"));
                return;
            }
            if (DISStart.Month != DISEnd.Month || DISStart.Year != DISEnd.Year)
            {
                ProcessPopupException(new Global.excToPopup("Start date " + txbISDFrom.Text + " and end date " + txbISDTo.Text + " are not in the same month"));
                return;
            }
            if (DISStart.Day != 1)
            {
                ProcessPopupException(new Global.excToPopup("Start date " + txbISDFrom.Text + " is not on the first day of a month"));
                return;
            }
            DateTimeOffset DlastDM = DISStart.AddMonths(1).AddDays(-1); // last day of month
            if (DISEnd.Day != DlastDM.Day)
            {
                ProcessPopupException(new Global.excToPopup("End date " + txbISDTo.Text + " is not the last day of the month"));
                return;
            }
            DISStart = DISStart.AddHours(1).AddMinutes(1);
            DISEnd = DISEnd.AddHours(22).AddMinutes(59);
            #endregion

            //TSoar.DB.SCUD_Multi mCrud = new TSoar.DB.SCUD_Multi();
            int iUser = mCrud.GetPeopleIDfromWebSiteUserName(HttpContext.Current.User.Identity.Name);
            FlyActInvoiceDataContext fdc = new FlyActInvoiceDataContext();
            string sStatus = "";
            fdc.spInvoiceSummaries(iUser, DISStart, DISEnd, ref sStatus);
            if (sStatus.Substring(0,2) != "OK")
            {
                ProcessPopupException(new Global.excToPopup("Status from stored procedure spInvoiceSummaries: " + sStatus));
            }
            else
            {
                ProcessPopupException(new Global.excToPopup(sStatus.Substring(2)));
            }
        }
        #endregion

        #region Manually Edit Flying Charges
        private void FillDataTableFCedit(int iuEditIndex)
        {
            DateTimeOffset DFrom = DateTimeOffset.MinValue;
            DateTimeOffset DTo = DateTimeOffset.MaxValue;
            if (!DateTimeOffset.TryParse(txbDFromFCE.Text, out DFrom))
            {
                ProcessPopupException(new Global.excToPopup("The 'From' date `" + txbDFromFCE.Text + "` is not in a correct format."));
                bManualRefresh = false;
                return;
            }
            if (!DateTimeOffset.TryParse(txbDToFCE.Text, out DTo))
            {
                ProcessPopupException(new Global.excToPopup("The 'To' date `" + txbDToFCE.Text + "` is not in a correct format."));
                bManualRefresh = false;
                return;
            }
            List<DataRow> liFlyChrgEdit = AssistLi.Init(Global.enLL.FlyChrgEdit, iPersonFCE, DFrom, DTo);
            Session["liFlyChrgEdit"] = liFlyChrgEdit;
            ivNRows = liFlyChrgEdit.Count;
            ivEditIndex = (iuEditIndex > -1) ? iuEditIndex : ivNRows - 1;
            GridView gv = gvFCedit;
            int iNewEditIndex = ivEditIndex;
            if (gv.AllowPaging)
            {
                int iFrom = gv.PageIndex * gv.PageSize;
                int iTo = iFrom + gv.PageSize - 1;
                if (iTo >= ivNRows) iTo = ivNRows;
                if (ivEditIndex >= iFrom && ivEditIndex <= iTo)
                {
                    iNewEditIndex = ivEditIndex % gv.PageSize;
                }
                else
                {
                    iNewEditIndex = -1;
                }
            }
            GridViewEditEventArgs gvee = new GridViewEditEventArgs(iNewEditIndex);
            gvFCedit_RowEditing(null, gvee);
        }

        protected void gvFCedit_RowEditing(object sender, GridViewEditEventArgs e)
        {
            GridView gv = gvFCedit;
            List<DataRow> liFlyChrgEdit = (List<DataRow>)Session["liFlyChrgEdit"];
            gv.EditIndex = e.NewEditIndex; // takes paging into account
            if (!(sender is null))
            {
                ivEditIndex = e.NewEditIndex + gv.PageIndex * gv.PageSize;
            }
            bEditExisting = false;
            if (ivEditIndex < liFlyChrgEdit.Count - 1)
            {
                // We are editing an existing row; need to get rid of the last row which would be the New row (but there is no New row)
                liFlyChrgEdit.RemoveAt(liFlyChrgEdit.Count - 1);
                bEditExisting = true;
            }
            DataTable dtFlyChrgEdit = liFlyChrgEdit.CopyToDataTable();
            dt_BindTogvgvFCedit(dtFlyChrgEdit);
        }

        protected void gvFCedit_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                //e.Row.Cells[6].HorizontalAlign = HorizontalAlign.Right;
                if ((e.Row.RowState & DataControlRowState.Edit) != 0)
                {
                    GridView GV = gvFCedit;
                    int iLast = ivNRows - 1; // The standard editing row
                    int iColAddButton = GV.Columns.Count - 2; // Column of the Edit and Add buttons
                    if (!bEditExisting)
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
                        // No Cancel button in the last row
                        e.Row.Cells[iColAddButton].Controls.Remove(e.Row.Cells[iColAddButton].Controls[2]);
                        e.Row.Cells[iColAddButton].Controls.Remove(e.Row.Cells[iColAddButton].Controls[1]);
                    }
                    else
                    {
                        DDLPerson.Enabled = false;
                        DDLMembCategFCE.Enabled = false;
                    }
                }
            }
        }

        private void dt_BindTogvgvFCedit(DataTable dtu)
        {
            if (iPersonFCE < 1)
            {
                gvFCedit.Visible = false;
            }
            else
            {
                GridView GV = gvFCedit;
                GV.Visible = true;
                DataView view = new DataView(dtu);
                view.Sort = "i1Sort ASC, DateOfAmount ASC";
                GV.DataSource = view;
                GV.DataBind();
            }
        }

        protected void gvFCedit_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            Label lblItem = (Label)gvFCedit.Rows[e.RowIndex].FindControl("lblIFCIdent");
            string sItem = lblItem.Text;
            ButtonsClear();
            YesButton.CommandName = "Delete";
            YesButton.CommandArgument = sItem;
            OkButton.CommandArgument = "FCE";
            lblPopupText.Text =
                "Please confirm deletion of Flying Charge record with ID " + sItem;
            MPE_Show(Global.enumButtons.NoYes);
        }

        protected void gvFCedit_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            FillDataTableFCedit(-1);
            DDLPerson.Enabled = true;
            DDLMembCategFCE.Enabled = true;
        }

        protected void gvFCedit_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            string sLog = "";
            ActivityLog.enumLogTypes elt = ActivityLog.enumLogTypes.DataInsert;
            FlyActInvoiceDataContext dc = new FlyActInvoiceDataContext();
            int iLast = gvFCedit.Rows.Count - 1;
            DateTimeOffset DateOfAmount = DateTimeOffset.Parse(((TextBox)gvFCedit.Rows[e.RowIndex].FindControl("txbDWhen")).Text);
            decimal mAmount = Decimal.Parse(((TextBox)gvFCedit.Rows[e.RowIndex].FindControl("txbAmount")).Text);
            char cTypeOfAmount = ((DropDownList)gvFCedit.Rows[e.RowIndex].FindControl("DDLType")).SelectedItem.Value[0];
            string sComments = Server.HtmlEncode(((TextBox)gvFCedit.Rows[e.RowIndex].FindControl("txbDComments")).Text);
            //TSoar.DB.SCUD_Multi mCrud = new TSoar.DB.SCUD_Multi();
            int iUser = mCrud.GetPeopleIDfromWebSiteUserName(HttpContext.Current.User.Identity.Name);
            Label lblIFCIdent = (Label)gvFCedit.Rows[e.RowIndex].FindControl("lblIFCIdent");
            int iId = Int32.Parse(lblIFCIdent.Text);
            if (iId == 0)
            {
                // Adding a new record
                FLYINGCHARGE f = new FLYINGCHARGE();
                f.PiTRecordEntered = DateTime.UtcNow;
                f.iRecordEnteredBy = iUser;
                f.iPerson = Int32.Parse(DDLPerson.SelectedValue);
                f.mAmount = mAmount;
                f.DateOfAmount = DateOfAmount;
                f.cTypeOfAmount = cTypeOfAmount;
                f.bManuallyModified = true;
                f.sComments = Server.HtmlEncode(sComments.Replace("'", "`"));

                dc.FLYINGCHARGEs.InsertOnSubmit(f);
                sLog = "PiTRecordEntered=" + f.PiTRecordEntered.ToString() + ", iRecordEnteredBy=" + f.iRecordEnteredBy + ", iPerson=" + f.iPerson.ToString() +
                    ", mAmount=" + f.mAmount.ToString() + ", DateOfAmount=" + f.DateOfAmount.ToString() + ", cTypeOfAmount=" + f.cTypeOfAmount +
                    ", bManuallyModified=" + f.bManuallyModified.ToString() + ", sComments=" + f.sComments;
            }
            else
            {
                int iID = Int32.Parse(((Label)gvFCedit.Rows[e.RowIndex].FindControl("lblIFCIdent")).Text);
                var f = (from v in dc.FLYINGCHARGEs where v.ID == iID select v).First();
                f.PiTRecordEntered = DateTime.UtcNow;
                f.iRecordEnteredBy = iUser;
                f.iPerson = Int32.Parse(DDLPerson.SelectedValue);
                f.mAmount = mAmount;
                f.DateOfAmount = DateOfAmount;
                f.cTypeOfAmount = cTypeOfAmount;
                f.bManuallyModified = true;
                f.sComments = sComments.Replace("'", "`");
                sLog = "PiTRecordEntered=" + f.PiTRecordEntered.ToString() + ", iRecordEnteredBy=" + f.iRecordEnteredBy + ", iPerson=" + f.iPerson.ToString() +
                    ", mAmount=" + f.mAmount.ToString() + ", DateOfAmount=" + f.DateOfAmount.ToString() + ", cTypeOfAmount=" + f.cTypeOfAmount +
                    ", bManuallyModified=" + f.bManuallyModified.ToString() + ", sComments=" + f.sComments;
                elt = ActivityLog.enumLogTypes.DataUpdate;
            }
            try
            {
                dc.SubmitChanges();
                ActivityLog.oLog(elt, 1, "MinFlyChrgs.aspx.cs: " + sLog);
            }
            catch (Exception exc)
            {
                Global.excToPopup ex = new Global.excToPopup(exc.Message);
                ProcessPopupException(ex);
                return;
            }
            dc.Dispose();
            gvFCedit.EditIndex = -1;
            FillDataTableFCedit(-1);
            DDLPerson.Enabled = true;
            DDLMembCategFCE.Enabled = true;
        }

        protected void gvFCedit_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridView gv = (GridView)sender;
            gv.PageIndex = e.NewPageIndex; // counts pages starting at 0
            FillDataTableFCedit(ivEditIndex);
        }

        protected void pbRefreshFCE_Click(object sender, EventArgs e)
        {
            bManualRefresh = true;
        }
        #endregion

        #region Help
        protected void pbHelp_Click(object sender, EventArgs e)
        {
            AccordionMFCHelp.SelectedIndex = TabC_MFC.ActiveTabIndex;
            ModPopExt_Help.Show();
        }

        protected void pbFDone_Click(object sender, EventArgs e)
        {
        }
        #endregion
    }
}