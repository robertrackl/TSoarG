using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Web.Util;
using System.Web.UI;
using System.Web.UI.WebControls;
using TSoar.DB;

namespace TSoar.Operations
{
    public partial class OpsSchedule : System.Web.UI.Page
    {
        SCUD_Multi mCRUD = new SCUD_Multi();
        //public const int icNCategs = 15; // The number of categories of signups; must be the same as the number of rows in table FSCATEGS. // SCR 222
        public const string scFilled = "<{[Filled]}>"; // To help with blanking out cells that were filled in to make all signup lists the same for one day of operations
        public struct SCateg
        {
            public char cKind;
            public int iCateg;
            public string sCateg;
            public string sNotes;
            public SCateg(char cuKind, int iuCateg, string suCateg, string suNotes)
            {
                cKind = cuKind;
                iCateg = iuCateg;
                sCateg = suCateg;
                sNotes = suNotes;
            }
        }
        // SCR 221 start
        public struct SIntStr
        {
            public int iID;
            public string sName;
            public SIntStr(int iuID, string suStr)
            {
                iID = iuID;
                sName = suStr;
            }
        }
        // SCR 221 end
        public Dictionary<int, SCateg> dictColNames = new Dictionary<int, SCateg>();
        public Dictionary<char, string> dictCategKinds = new Dictionary<char, string>();
        private const int icFirstCategCol = 4; // Pointer to the first column in gvOpsSch that contains a signup category

        #region Properties
        private int ilDate { get { return iGetInt("ilDate"); } set { ViewState["ilDate"] = value; } } // ID of row in table FSDATES
        private int ilCateg { get { return iGetInt("ilCateg"); } set { ViewState["ilCateg"] = value; } } // ID of row in table FSCATEGS
        private int ilPerson { get { return iGetInt("ilPerson"); } set { ViewState["ilPerson"] = value; } } // ID of row in table PEOPLE
        private int iGetInt(string suN)
        {
            if (ViewState[suN] == null)
            {
                return 0;
            }
            else
            {
                return (int)ViewState[suN];
            }
        }
        #endregion

        protected void Page_PreInit(object sender, EventArgs e)
        {
            OpsSchedDataContext OSdc = new OpsSchedDataContext();
            var qc = from c in OSdc.FSCATEGs orderby c.cKind descending, c.iOrder ascending select c;
            int iCol = 0;
            foreach (var c in qc)
            {
                iCol++;
                dictColNames.Add(iCol, new SCateg(c.cKind, c.ID, c.sCateg, c.sNotes)); // iCol is 1-based
            }
            if (iCol != Global.igcNCategs) // SCR 222
            {
                throw new Global.excToPopup("Operations.OpsSchedule.Page_Preinit: The number of categories in table FSCATEGS = " + iCol.ToString() +
                    " is not the same as constant igcNCategs = " + Global.igcNCategs.ToString()); // SCR 222
            }
            dictCategKinds.Add('R', "Role");
            dictCategKinds.Add('E', "Equipment");
            dictCategKinds.Add('A', "Activity");
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                if (Session["DFrom"] is null)
                {
                    Session["DFrom"] = DateTime.Now.Subtract(new TimeSpan(3, 0, 0, 0));
                }
                txbDFrom.Text = CustFmt.sFmtDate((DateTime)Session["DFrom"], CustFmt.enDFmt.DateOnly).Replace("/", "-");

                if (Session["DTo"] is null)
                {
                    OpsSchedDataContext OSdc = new OpsSchedDataContext();
                    try
                    {
                        Session["DTo"] = (from d in OSdc.FSDATEs orderby d.Date descending select d.Date).First();
                        txbDTo.Text = CustFmt.sFmtDate((DateTime)Session["DTo"], CustFmt.enDFmt.DateOnly).Replace("/", "-");
                    }
                    catch
                    {
                        txbDTo.Text = "2199-12-31";
                        Session["DTo"] = DateTime.Parse(txbDTo.Text);
                    }
                }
                ilPerson = mCRUD.GetPeopleIDfromWebSiteUserName(System.Web.HttpContext.Current.User.Identity.Name); // SCR 221
                FillFOSTable();
            }
        }

        private void FillFOSTable()
        {
            int iPageSize = Int32.Parse(mCRUD.GetSetting("PageSizeOpsSchedule"));
            DataTable td = new DataTable("OpsSched");
            DataColumn dc = new DataColumn("ID", System.Type.GetType("System.Int32")); // ID in table FSDATES
            dc.Unique = false;
            td.Columns.Add(dc);
            dc = new DataColumn("DDate", System.Type.GetType("System.DateTime"));
            dc.Unique = false;
            td.Columns.Add(dc);
            dc = new DataColumn("bEnabled", System.Type.GetType("System.Boolean"));
            dc.Unique = false;
            td.Columns.Add(dc);
            dc = new DataColumn("sNote", System.Type.GetType("System.String"));
            dc.Unique = false;
            td.Columns.Add(dc);

            OpsSchedDataContext OSdc = new OpsSchedDataContext();
            for (int i = 1; i <= Global.igcNCategs; i++) // SCR 222
            {
                dc = new DataColumn("I" + dictColNames[i].sCateg, System.Type.GetType("System.Int32")); // SCR 221
                td.Columns.Add(dc); // SCR 221
                dc = new DataColumn(dictColNames[i].sCateg, System.Type.GetType("System.String")); // SCR 221
                td.Columns.Add(dc);
            }

            DateTime DFrom = (DateTime)Session["DFrom"];
            DateTime DTo = (DateTime)Session["DTo"];
            var qr = from r in OSdc.FSDATEs where r.bEnabled && r.Date >= DFrom && r.Date <= DTo orderby r.Date, r.sNote select r;
            int iCol = 0;
            SIntStr intStr1; // SCR 221
            foreach (var r in qr) // Loop over the dates
            {
                iCol = -1;
                int iSameDateSameCategSignups = 0;
                List<List<SIntStr>> LiLi = new List<List<SIntStr>>(); // outer list // SCR 221
                int iSignupCountMax = 0;
                for (int i = 0; i < Global.igcNCategs; i++) // SCR 222
                {
                    iCol++;
                    // Does table FSSIGNUPS hold any data at the intersection of a Date and a Category? If so, how many?
                    // SCR 221 start
                    var q1 = (from s in OSdc.FSSIGNUPs
                              where s.iDate == r.ID && s.iCateg == dictColNames[i + 1].iCateg
                              select new
                              { iFSSignup = s.ID,
                                sShownName = ((s.sNameInSchedule == null) || (s.sNameInSchedule.Trim().Length < 1))
                                        ? s.PEOPLE.sDisplayName : Server.HtmlDecode(s.sNameInSchedule)
                              } // If sNameInSchedule is null or empty then use sDisplayName
                             ).ToList();
                    // SCR 221 end
                    iSameDateSameCategSignups = q1.Count();
                    int iSignupCount = 0;
                    List<SIntStr> Li = new List<SIntStr>(); // inner list // SCR 221
                    if (iSameDateSameCategSignups > 0)
                    {
                        foreach (var s in q1)
                        {
                            SIntStr intStr2 = new SIntStr(s.iFSSignup, s.sShownName); // SCR 221
                            Li.Add(intStr2); // SCR 221
                            iSignupCount++;
                        }
                    }
                    intStr1 = new SIntStr(0, ""); // SCR 221
                    Li.Add(intStr1); // We need an empty row at the bottom of a day of operations with signups // SCR 221
                    iSignupCount++;
                    iSignupCountMax = Math.Max(iSignupCount, iSignupCountMax);
                    LiLi.Add(Li);
                }
                // Make all inner lists the same length as the longest inner list
                foreach (List<SIntStr> innerLi in LiLi) // SCR 221
                {
                    int innerLiCount = innerLi.Count;
                    if (innerLiCount < iSignupCountMax)
                    {
                        intStr1 = new SIntStr(0, scFilled); // SCR 221
                        for (int j = 0; j < iSignupCountMax - innerLiCount; j++)
                        {
                            innerLi.Add(intStr1); // SCR 221
                        }
                    }
                }
                // Create as many new rows in td as there are items in one of the inner lists
                for (int j = 0; j < iSignupCountMax; j++)
                {
                    DataRow dr = td.NewRow();
                    dr["ID"] = r.ID * 100 + j; // ID of row in table FSDATES
                    dr["DDate"] = r.Date;
                    dr["bEnabled"] = r.bEnabled;
                    dr["sNote"] = r.sNote;
                    for (int i = 0; i < Global.igcNCategs; i++) // SCR 222
                    {
                        dr["I" + dictColNames[i + 1].sCateg] = LiLi[i][j].iID; // SCR 221
                        dr[dictColNames[i + 1].sCateg] = LiLi[i][j].sName; // SCR 221
                    }
                    if (r.bEnabled)
                    {
                        td.Rows.Add(dr);
                    }
                }
            }
            // Make sure that all the signups for a day of operations are on the same page.
            // Method: insert empty rows at the bottom of a page if necessary
            int iPageIndex = 0;
            int iNumRows = td.Rows.Count;
            // Go to the next page break and see whether rows before and after the break have the same date
            bool bKeepPaging = true;
            do
            {
                // Row at bottom of current page:
                int iRow = (iPageIndex + 1) * iPageSize - 1;
                if (iRow < iNumRows - 1)
                {
                    DataRow drB = td.Rows[iRow];
                    DateTime theDate = (DateTime)drB["DDate"];
                    // Row at top of next page:
                    bool bInsertBlanks = true;
                    DataRow drT = td.Rows[iRow + 1];
                    if (theDate == (DateTime)drT["DDate"])
                    {
                        // We do have the situation where a date breaks over two pages
                        bool bContinue = true;
                        int iDecrCnt = 1;
                        do
                        {
                            // Where do rows with this date start?
                            if ((DateTime)td.Rows[iRow - 1]["DDate"] == theDate)
                            {
                                iRow--;
                                iDecrCnt++;
                                if (iDecrCnt > 8)
                                {
                                    bContinue = false;
                                    bInsertBlanks = false; // we don't bother fixing the end of a page if the number of signups is large
                                }
                            }
                            else
                            {
                                bContinue = false;
                            }

                        } while (bContinue);
                        if (bInsertBlanks)
                        {
                            for (int j = 1; j <= iDecrCnt; j++)
                            {
                                DataRow dr = td.NewRow();
                                dr["ID"] = 0; // ID of row in table FSDATES
                                dr["DDate"] = DateTime.MinValue;
                                dr["bEnabled"] = false;
                                dr["sNote"] = "";
                                td.Rows.InsertAt(dr, iRow);
                            }
                        }
                    }
                }
                else
                {
                    bKeepPaging = false;
                }
                iPageIndex++;
            } while (bKeepPaging);

            gvOpsSch.DataSource = td;
            gvOpsSch.PageSize = iPageSize;
            gvOpsSch.DataBind();
        }

        protected void ipb_Click(object sender, ImageClickEventArgs e)
        {
            ImageButton pb = (ImageButton)sender;
            GridViewRow row = (GridViewRow)pb.NamingContainer;
            Label lbliDate = (Label)row.FindControl("lbliDate"); // holds string form of ID of row in table FSDATES
            string sdictCateg = pb.ID.Substring(3, 2); // string form of index into member of dictCateg
            int idictCateg = Int32.Parse(sdictCateg); // index into member of dictCateg
            ilDate = Int32.Parse(lbliDate.Text) / 100; // ID of tuple in table FSDATES
            Label lbl = (Label)row.FindControl("lbl" + sdictCateg); // Find labels of ID format lbl01, lbl02, ...
            string sSignUp = lbl.Text; // SCR 221
            Label lblI = (Label)row.FindControl("lblI" + sdictCateg); // Find labels of ID format lblI01, lblI02, ... // SCR 221
            int iSignUp = Int32.Parse(lblI.Text); // SCR 221
            OpsSchedDataContext OSdc = new OpsSchedDataContext();
            ilCateg = (from c in OSdc.FSCATEGs where c.sCateg == dictColNames[idictCateg].sCateg select c.ID).First(); // ID of tuple in table FSCATEGS
            lblMPEKind.Text = (from c in OSdc.FSCATEGs where c.ID == ilCateg select dictCategKinds[c.cKind]).First();
            ilPerson = 0;
            // Does table FSSIGNUPS hold any data at the intersection of a Date and a Category?
            lblMPEDate.Text = CustFmt.sFmtDate((from d in OSdc.FSDATEs where d.ID==ilDate select d.Date).First(), CustFmt.enDFmt.DateOnly);
            lblMPECateg.Text = dictColNames[idictCateg].sCateg;

            var q2 = (from s in OSdc.FSSIGNUPs
                      where s.ID == iSignUp // SCR 221
                      select new { s, s.FSDATE.Date, s.FSCATEG.sCateg }).ToList();
            if (q2.Count() > 0)
            {
                pbAdd.Enabled = false;
                pbUpdate.Enabled = true;
                pbRemove.Enabled = true;
                ilPerson = q2.First().s.iPerson;
                Set_DropDown_ByValue(DDLMPEMembers, ilPerson);
                txbDiffName.Text = Server.HtmlDecode(q2.First().s.sNameInSchedule); // SCR 221
                txbRemarks.Text = Server.HtmlDecode(q2.First().s.sRemarks); // SCR 221
                lbliSignup.Text = iSignUp.ToString(); // SCR 221
            }
            else
            {
                txbDiffName.Text = ""; // SCR 221
                txbRemarks.Text = "";
                pbAdd.Enabled = true;
                pbUpdate.Enabled = false;
                pbRemove.Enabled = false;
                lbliSignup.Text = "0"; // SCR 221
            }
            ModPopExt.Show();
        }
        // SCR 221 start
        //private void Set_DropDown_ByText(DropDownList ddl, string suText)
        //{
        //    ddl.ClearSelection();
        //    foreach (ListItem li in ddl.Items)
        //    {
        //        if (li.Text == Server.HtmlDecode(suText))
        //        {
        //            li.Selected = true;
        //            break;
        //        }
        //    }
        //}

        private void Set_DropDown_ByValue(DropDownList ddl, int iu)
        {
            ddl.ClearSelection();
            foreach (ListItem li in ddl.Items)
            {
                if (Int32.Parse(li.Value) == iu)
                {
                    li.Selected = true;
                    break;
                }
            }
        }
        // SCR 221 end
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
            if (btn.ID == "YesButton")
            {
            }
        }

        private void ProcessPopupException(Global.excToPopup excu)
        {
            ButtonsClear();
            lblPopupText.Text = excu.sExcMsg();
            MPE_Show(Global.enumButtons.OkOnly);
        }
        #endregion

        protected void ModPopExt_Load(object sender, EventArgs e)
        {
            foreach (ListItem li in DDLMPEMembers.Items)
            {
                li.Text = Server.HtmlDecode(li.Text);
            }
        }

        protected void MPE_Click(object sender, EventArgs e)
        {
            Button pb = (Button)sender;
            if (pb.ID == "pbDismissTop" || pb.ID == "pbDismissBottom") return;
            int iSignUp = Int32.Parse(lbliSignup.Text); // SCR 221
            OpsSchedDataContext OSdc = new OpsSchedDataContext();
            FSSIGNUP fss = new FSSIGNUP();
            var qm = (from s in OSdc.FSSIGNUPs
                      where s.ID == iSignUp // SCR 221
                      select s).ToList();
            if (qm.Count() > 0)
            {
                fss = qm.First();
                switch (pb.ID)
                {
                    case "pbUpdate":
                        fss.iPerson = Int32.Parse(DDLMPEMembers.SelectedValue);
                        fss.sNameInSchedule = Server.HtmlEncode(txbDiffName.Text); // SCR 221
                        fss.sRemarks = Server.HtmlEncode(txbRemarks.Text); // SCR 221
                        break;
                    case "pbRemove":
                        OSdc.FSSIGNUPs.DeleteOnSubmit(fss);
                        break;
                }
            }
            else
            {
                if (pb.ID == "pbAdd")
                {
                    fss.DEntered = DateTimeOffset.Now;
                    fss.DModified = fss.DEntered;
                    fss.iDate = ilDate;
                    fss.iCateg = ilCateg;
                    int iPerson = Int32.Parse(DDLMPEMembers.SelectedValue);
                    if (iPerson < 1)
                    {
                        ProcessPopupException(new Global.excToPopup("You must select a member for this signup"));
                        return;
                    }
                    // Duplicate signups are not allowed:
                    var qd = (from s in OSdc.FSSIGNUPs
                              where s.iPerson == iPerson && s.iDate == ilDate && s.iCateg == ilCateg
                              select new { sDisplayName = s.PEOPLE.sDisplayName, Date = s.FSDATE.Date, sCateg = s.FSCATEG.sCateg }).ToList();
                    if (qd.Count > 0)
                    {
                        var qq = qd.First();
                        ProcessPopupException(new Global.excToPopup("Member " + qq.sDisplayName + " is already signed up for " +
                            CustFmt.sFmtDate(qq.Date, CustFmt.enDFmt.DateOnly) + " in category " + qq.sCateg));
                        return;
                    }
                    fss.iPerson = iPerson;
                    fss.sNameInSchedule = Server.HtmlEncode(txbDiffName.Text); // SCR 221
                    fss.sRemarks = Server.HtmlEncode(txbRemarks.Text); // SCR 221
                    OSdc.FSSIGNUPs.InsertOnSubmit(fss);
                }
            }
            OSdc.SubmitChanges();
            FillFOSTable();
        }

        protected void gvOpsSch_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvOpsSch.PageIndex = e.NewPageIndex;
            FillFOSTable();
        }
        protected void gvOpsSch_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
            {
                for (int i = icFirstCategCol; i < e.Row.Cells.Count; i++)
                {
                    char cKind = dictColNames[i - icFirstCategCol + 1].cKind;
                    if (cKind == 'E')
                    {
                        e.Row.Cells[i].BackColor = Color.YellowGreen;
                    }
                    e.Row.Cells[i].ToolTip = "Signup Category `" + dictColNames[i - icFirstCategCol + 1].sCateg + "`" + Environment.NewLine
                         + Environment.NewLine + "     Kind: " + dictCategKinds[cKind];
                    string sNotes = dictColNames[i - icFirstCategCol + 1].sNotes;
                    if (sNotes.Length > 0)
                    {
                        e.Row.Cells[i].ToolTip+= Environment.NewLine + "     Notes: " + sNotes; 
                    }
                }
            }
            else if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (!(bool)DataBinder.Eval(e.Row.DataItem, "bEnabled"))
                {
                    // Blank out the row
                    foreach (DataControlFieldCell c in e.Row.Cells)
                    {
                        foreach(Control v in c.Controls)
                        {
                            v.Visible = false;
                        }
                    }
                }
                else
                {
                    OpsSchedDataContext OSdc = new OpsSchedDataContext();
                    for (int i = icFirstCategCol; i < e.Row.Cells.Count; i++)
                    {
                        Label lblDate = (Label)e.Row.FindControl("lblDate");
                        int iDictIndex = i - icFirstCategCol + 1;
                        Label lblIDate = (Label)e.Row.FindControl("lblIDate");
                        int iDate = Int32.Parse(lblIDate.Text);
                        int iDat = iDate / 100;
                        string sdictCateg = ((100 + iDictIndex).ToString()).Substring(1, 2); // string form of index into member of dictCateg
                        Label lbl = (Label)e.Row.FindControl("lbl" + sdictCateg); // Find labels of ID format lbl01, lbl02, ...
                        Label lblI = (Label)e.Row.FindControl("lblI" + sdictCateg); // Find labels of ID format lblI01, lblI02, ... // SCR 221
                        string sSignUp = Server.HtmlDecode(lbl.Text); // SCR 221
                        int iSignUp = Int32.Parse(lblI.Text); // SCR 221
                        if (sSignUp == scFilled)
                        {
                            foreach (Control ctrl in e.Row.Cells[i].Controls)
                            {
                                ctrl.Visible = false;
                            }
                        }
                        else
                        {
                            e.Row.Cells[i].ToolTip = lblDate.Text + Environment.NewLine + "Category: " + dictColNames[iDictIndex].sCateg; // SCR 221
                            var q0 = (from s in OSdc.FSSIGNUPs
                                      where s.ID == iSignUp // SCR 221
                                      select s).ToList();
                            if (q0.Count() > 0)
                            {   // SCR 221 start
                                string sNameInSchedule = q0.First().sNameInSchedule;
                                if (sNameInSchedule != null && sNameInSchedule.Trim().Length > 0)
                                {
                                    e.Row.Cells[i].ToolTip += Environment.NewLine + "Signup Owner: " + Server.HtmlDecode(q0.First().PEOPLE.sDisplayName);
                                }
                                // SCR 221 end
                                string sRemarks = q0.First().sRemarks;
                                e.Row.Cells[i].ToolTip += Environment.NewLine + sSignUp; // SCR 221
                                if (sRemarks.Length > 0)
                                {
                                    e.Row.Cells[i].ToolTip += Environment.NewLine + "Remarks: " + sRemarks; // SCR 221
                                    lbl.Font.Bold = true;
                                    lbl.ForeColor = Color.DarkMagenta;
                                }
                            }
                            else
                            {
                                e.Row.Cells[i].ToolTip += Environment.NewLine + "Vacant";
                            }
                        }
                        // Blank out duplicate data at start of row
                        if ((iDate % 100) != 0)
                        {
                            for (int j = 1; j < icFirstCategCol; j++)
                            {
                                foreach (Control ctrl in e.Row.Cells[j].Controls)
                                {
                                    ctrl.Visible = false;
                                }
                            }
                        }
                    }
                }
            }
        }

        protected void pbDateRangeUpdate_Click(object sender, EventArgs e)
        {
            DateTime DFrom = DateTime.Parse(txbDFrom.Text);
            DateTime DTo = DateTime.Parse(txbDTo.Text);
            if (DFrom > DTo)
            {
                ProcessPopupException(new Global.excToPopup("'From' date cannot be later than 'To' date."));
                return;
            }
            Session["DFrom"] = DFrom;
            Session["DTo"] = DTo;
            FillFOSTable();
        }
    }
}