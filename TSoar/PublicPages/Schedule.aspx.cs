using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TSoar.DB;
using TSoar.Operations;

namespace TSoar.PublicPages
{
    public partial class Schedule : System.Web.UI.Page
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
        public Dictionary<int, SCateg> dictColNames = new Dictionary<int, SCateg>();
        public Dictionary<char, string> dictCategKinds = new Dictionary<char, string>();
        private const int icFirstCategCol = 4; // Pointer to the first column in gvOpsSch that contains a signup category

        #region Properties
        private int ilDate { get { return iGetInt("ilDate"); } set { ViewState["ilDate"] = value; } } // ID of row in table FSDATES
        private int ilCateg { get { return iGetInt("ilCateg"); } set { ViewState["ilCateg"] = value; } } // ID of row in table FSCATEGS
        private int ilPerson // ID of row in table PEOPLE
        {
            get { return iGetInt("ilPerson"); }
            set { ViewState["ilPerson"] = value; }
        }
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
            DB.SCUD_Multi mSCUD = new DB.SCUD_Multi();
            if (!this.IsPostBack)
            {
                FillFOSTable();
            }
        }

        private void FillFOSTable()
        {
            int iPageSize = 35;
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
                dc = new DataColumn(dictColNames[i].sCateg, System.Type.GetType("System.String")); // Label
                td.Columns.Add(dc);
            }

            var qr = from r in OSdc.FSDATEs where r.bEnabled orderby r.Date select r;
            int iCol = 0;
            foreach (var r in qr) // Loop over the dates
            {
                iCol = -1;
                int iSameDateSameCategSignups = 0;
                List<List<string>> LiLi = new List<List<string>>(); // outer list
                int iSignupCountMax = 0;
                for (int i = 0; i < Global.igcNCategs; i++) // SCR 222
                {
                    iCol++;
                    // Does table FSSIGNUPS hold any data at the intersection of a Date and a Category? If so, how many?
                    var q1 = (from s in OSdc.FSSIGNUPs where s.iDate == r.ID && s.iCateg == dictColNames[i + 1].iCateg
                              // SCR 221 start
                              select new 
                              { sShownName = ((s.sNameInSchedule == null) || (s.sNameInSchedule.Trim().Length < 1))
                                ? s.PEOPLE.sDisplayName : Server.HtmlDecode(s.sNameInSchedule)
                              }
                             ).ToList();
                    // SCR 221 end
                    iSameDateSameCategSignups = q1.Count();
                    int iSignupCount = 0;
                    List<string> Li = new List<string>(); // inner list
                    if (iSameDateSameCategSignups > 0)
                    {
                        foreach (var s in q1)
                        {
                            Li.Add(s.sShownName); // SCR 221
                            iSignupCount++;
                        }
                    }
                    if (iSignupCount < 1)
                    {
                        Li.Add(""); // We need an empty row for a day of operations without signups
                        iSignupCount++;
                    }
                    iSignupCountMax = Math.Max(iSignupCount, iSignupCountMax);
                    LiLi.Add(Li);
                }
                // Make all inner lists the same length as the longest inner list
                foreach (List<string> innerLi in LiLi)
                {
                    int innerLiCount = innerLi.Count;
                    if (innerLiCount < iSignupCountMax)
                    {
                        for (int j = 0; j < iSignupCountMax - innerLiCount; j++)
                        {
                            innerLi.Add(scFilled);
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
                        dr[dictColNames[i + 1].sCateg] = LiLi[i][j];
                    }
                    TimeSpan ts = new TimeSpan(3, 0, 0, 0);
                    if (r.bEnabled && r.Date > DateTime.Now.Subtract(ts)) // include only very recent past
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
        // SCR 221 end
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
                        e.Row.Cells[i].ToolTip += Environment.NewLine + "     Notes: " + sNotes;
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
                        foreach (Control v in c.Controls)
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
                        // Visibility of controls
                        Label lblDate = (Label)e.Row.FindControl("lblDate");
                        int iDictIndex = i - icFirstCategCol + 1;
                        Label lblIDate = (Label)e.Row.FindControl("lblIDate");
                        int iDate = Int32.Parse(lblIDate.Text);
                        int iDat = iDate / 100;
                        string sdictCateg = ((100 + iDictIndex).ToString()).Substring(1, 2); // string form of index into member of dictCateg
                        Label lbl = (Label)e.Row.FindControl("lbl" + sdictCateg); // Find labels of ID format lbl01, lbl02, ...
                        string sSignUp = lbl.Text;
                        if (sSignUp == scFilled)
                        {
                            foreach (Control ctrl in e.Row.Cells[i].Controls)
                            {
                                ctrl.Visible = false;
                            }
                        }
                        // Blank out duplicate data at start of row
                        if ((iDate % 100) != 0)
                        {
                            for (int j = 1; j < icFirstCategCol; j++)
                            {
                                foreach (Control ctrl in e.Row.Cells[j].Controls)
                                {
                                    //ctrl.Visible = false;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}