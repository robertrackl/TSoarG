using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TSoar.Operations;
using TSoar.DB;

namespace TSoar.MemberPages.OpsScheduleSignup
{
    public partial class OpsSchedSigSMALL : System.Web.UI.Page
    {
        SCUD_Multi mCRUD = new SCUD_Multi(); 
        private int ilCateg // ID of row in table PEOPLE
        {
            get { return iGetInt("ilCateg"); }
            set { ViewState["ilCateg"] = value; }
        }
        private int ilPerson // ID of row in table PEOPLE
        {
            get { return iGetInt("ilPerson"); }
            set { ViewState["ilPerson"] = value; }
        }
        private int ilDate // ID of row in table FSDATES
        {
            get { return iGetInt("ilDate"); }
            set { ViewState["ilDate"] = value; }
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

        private DateTime DCurr
        {
            get { return DGet("DCurr"); }
            set { ViewState["DCurr"] = value; }
        }
        private DateTime DGet(string suN)
        {
            if (ViewState[suN] == null)
            {
                return DateTime.Now.Date;
            }
            else
            {
                return (DateTime)ViewState[suN];
            }
        }

        public Dictionary<char, string> dictCategKinds = new Dictionary<char, string>();

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
            //Button btn = (Button)sender;
            //if (btn.ID == "YesButton")
            //{
            //    switch (btn.CommandName)
            //    {
            //        case "Delete":
            //            switch (OkButton.CommandArgument)
            //            {
            //                case "FOSDate":
            //                    // Delete a Flight Operations Schedule date
            //                    try
            //                    {
            //                        mCRUD.DeleteOne(Global.enugInfoType.FltOpsSchedDates, btn.CommandArgument);
            //                    }
            //                    catch (Global.excToPopup exc)
            //                    {
            //                        ProcessPopupException(exc);
            //                    }
            //                    iEdRow = -1;
            //                    FillDatesTable();
            //                    break;
            //            }
            //            break;
            //        case "AddDates":
            //            AddDates();
            //            break;
            //    }
            //}
        }

        private void ProcessPopupException(Global.excToPopup excu)
        {
            ButtonsClear();
            lblPopupText.Text = excu.sExcMsg();
            MPE_Show(Global.enumButtons.OkOnly);
        }
        #endregion

        protected void Page_PreInit(object sender, EventArgs e)
        {
            dictCategKinds.Add('R', "Role");
            dictCategKinds.Add('E', "Equipment");
            dictCategKinds.Add('A', "Activity");
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ComingUp();
                FillOSTables();
            }
        }

        private void ComingUp()
        {
            // Choose an initial date: the one that is in the future, closest to today, including today
            OpsSchedDataContext OSdc = new OpsSchedDataContext();
            var qd = OSdc.sfOpsSchedInitDate().First();
            ilDate = (int)qd.iDate;
            DCurr = (DateTime)qd.Dresult;
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

        protected void pbMemPgs_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/MemberPages/Members.aspx");
        }

        protected void OneDay_Click(object sender, EventArgs e)
        {
            Button pb = (Button)sender;
            int iSelIndex = DDLDate.SelectedIndex;
            int iCntM1 = DDLDate.Items.Count - 1;
            bool bUpdate = true;
            switch (pb.ID)
            {
                case "pbFirst":
                    if (iSelIndex > 0)
                    {
                        iSelIndex = 0;
                    }
                    else
                    {
                        bUpdate = false;
                    }
                    break;
                case "pbEarlier":
                    iSelIndex--;
                    if (iSelIndex < 0)
                    {
                        iSelIndex = 0;
                        bUpdate = false;
                    }
                    break;
                case "pbToday":
                    ComingUp();
                    Set_DropDown_ByValue(DDLDate, ilDate.ToString());
                    FillOSTables();
                    bUpdate = false;
                    break;
                case "pbLater":
                    iSelIndex++;
                    if (iSelIndex > iCntM1)
                    {
                        iSelIndex = iCntM1;
                        bUpdate = false;
                    }
                    break;
                case "pbLast":
                    if (iSelIndex < iCntM1)
                    {
                        iSelIndex = iCntM1;
                    }
                    else
                    {
                        bUpdate = false;
                    }
                    break;
            }
            if (bUpdate)
            {
                DDLDate.SelectedIndex = iSelIndex;
                DDLDate_SelectedIndexChanged(DDLDate, new EventArgs());
            }
        }

        private void FillOSTables()
        {
            DataTable dt = new DataTable("OpsSchedSMALL");
            DataColumn dc = new DataColumn("sCateg", System.Type.GetType("System.String"));
            dt.Columns.Add(dc);
            dc.AllowDBNull = false;
            dc = new DataColumn("iCateg", System.Type.GetType("System.Int32"));
            dc.AllowDBNull = false;
            dt.Columns.Add(dc);
            dc = new DataColumn("cKind", System.Type.GetType("System.Char"));
            dc.AllowDBNull = false;
            dt.Columns.Add(dc);
            dc = new DataColumn("iOrder", System.Type.GetType("System.Int32"));
            dc.AllowDBNull = false;
            dt.Columns.Add(dc);
            dc = new DataColumn("bEnabled", System.Type.GetType("System.Boolean"));
            dc.AllowDBNull = false;
            dc.DefaultValue = true;
            dt.Columns.Add(dc);
            dc = new DataColumn("iPerson", System.Type.GetType("System.Int32"));
            dc.AllowDBNull = true;
            dc.DefaultValue = 0;
            dt.Columns.Add(dc);
            dc = new DataColumn("sDisplayName", System.Type.GetType("System.String")); // from PEOPLE
            dc.AllowDBNull = true;
            dc.DefaultValue = "";
            dt.Columns.Add(dc);
            dc = new DataColumn("sNameInSchedule", System.Type.GetType("System.String")); // SCR 221
            dc.AllowDBNull = true; // SCR 221
            dc.DefaultValue = ""; // SCR 221
            dt.Columns.Add(dc); // SCR 221
            dc = new DataColumn("sRemarks", System.Type.GetType("System.String"));
            dc.AllowDBNull = true;
            dc.DefaultValue = "";
            dt.Columns.Add(dc);

            OpsSchedDataContext OSdc = new OpsSchedDataContext();
            var qCat = from c in OSdc.FSCATEGs orderby c.cKind descending, c.sCateg ascending select c;
            string sUserDisplayName = mCRUD.GetPeopleDisplayNamefromWebSiteUserName(System.Web.HttpContext.Current.User.Identity.Name);
            string sCateg = "";
            foreach (var c in qCat)
            {
                DataRow dr;
                string sRepeat = new String((char)160, 8) + " - " + (char)34 + " -";
                var qs = (from s in OSdc.FSSIGNUPs where s.iDate == ilDate && s.iCateg == c.ID orderby s.PEOPLE.sDisplayName descending select s).ToList();
                bool bEmptyRow = true; // Don't want to offer the user the opportunity to add himself again if he's already signed up
                if (qs.Count > 0)
                {
                    foreach (var s in qs)
                    {
                        dr = dt.NewRow();
                        dr["sCateg"] = (c.sCateg == sCateg) ? sRepeat : c.sCateg;
                        sCateg = c.sCateg;
                        dr["iCateg"] = c.ID;
                        dr["cKind"] = c.cKind;
                        dr["iOrder"] = c.iOrder;
                        dr["bEnabled"] = true;
                        dr["iPerson"] = s.iPerson;
                        if (s.PEOPLE.sDisplayName == sUserDisplayName) bEmptyRow = false;
                        dr["sDisplayName"] = s.PEOPLE.sDisplayName;
                        dr["sNameInSchedule"] = s.sNameInSchedule; // SCR 221
                        dr["sRemarks"] = s.sRemarks;
                        dt.Rows.Add(dr);
                    }
                }
                if (bEmptyRow)
                {
                    dr = dt.NewRow();
                    dr["sCateg"] = (c.sCateg == sCateg) ? sRepeat : c.sCateg;
                    sCateg = c.sCateg;
                    dr["iCateg"] = c.ID;
                    dr["cKind"] = c.cKind;
                    dr["iOrder"] = c.iOrder;
                    dr["bEnabled"] = true;
                    dt.Rows.Add(dr);
                }
            }
            DataView view = new DataView(dt);
            view.Sort = "cKind DESC, iOrder ASC, sDisplayName DESC";
            gvOSSmall.DataSource = view;
            gvOSSmall.DataBind();
        }

        protected void gvOSSmall_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                ImageButton ipbAddEdit = (ImageButton)e.Row.FindControl("ipbAddEdit");
                Label lblIPerson = (Label)e.Row.FindControl("lblIPerson");
                if (lblIPerson.Text.Length > 0 && 
                    mCRUD.GetPeopleDisplayNamefromWebSiteUserName(System.Web.HttpContext.Current.User.Identity.Name) != lblIPerson.Text)
                {
                    ipbAddEdit.Enabled = false;
                    ipbAddEdit.ImageUrl = "~/i/GrayButton.jpg";
                }
                Label lblIKind = (Label)e.Row.FindControl("lblIKind");
                if (lblIKind.Text == dictCategKinds['E'])
                {
                    e.Row.Cells[0].BackColor = Color.YellowGreen;
                }
            }
        }

        protected void ipbAddEdit_Click(object sender, ImageClickEventArgs e)
        {
            ImageButton pb = (ImageButton)sender;
            ilCateg = Int32.Parse(pb.CommandArgument);
            ilPerson = Int32.Parse(pb.CommandName);

            OpsSchedDataContext OSdc = new OpsSchedDataContext();
            lblMPEDate.Text = CustFmt.sFmtDate((from d in OSdc.FSDATEs where d.ID==ilDate select d.Date).First(),CustFmt.enDFmt.DateOnly);
            var qc = (from c in OSdc.FSCATEGs where c.ID == ilCateg select c).ToList().First();
            lblMPEKind.Text = dictCategKinds[qc.cKind];
            lblMPECateg.Text = qc.sCateg;
            var qp = (from p in OSdc.PEOPLEs where p.ID == ilPerson select p.sDisplayName).ToList();
            if (qp.Count > 0)
            {
                lblMPEMember.Text = qp.First();
            }
            else
            {
                lblMPEMember.Text = mCRUD.GetPeopleDisplayNamefromWebSiteUserName(System.Web.HttpContext.Current.User.Identity.Name);
                ilPerson = mCRUD.GetPeopleIDfromWebSiteUserName(System.Web.HttpContext.Current.User.Identity.Name);
            }

            // Does table FSSIGNUPS hold any data at the intersection of a Date and a Category?
            var q2 = (from s in OSdc.FSSIGNUPs
                      where s.iDate == ilDate && s.iCateg == ilCateg && s.iPerson == ilPerson
                      select new { s, s.FSDATE.Date, s.FSCATEG.sCateg }).ToList();
            if (q2.Count() > 0)
            {
                pbAdd.Enabled = false;
                pbUpdate.Enabled = true;
                pbRemove.Enabled = true;
                txbOther.Text = q2.First().s.sNameInSchedule;
                txbRemarks.Text = q2.First().s.sRemarks;
            }
            else
            {
                txbOther.Text = "";
                txbRemarks.Text = "";
                pbAdd.Enabled = true;
                pbUpdate.Enabled = false;
                pbRemove.Enabled = false;
            }
            ModPopExt.Show();
        }

        protected void MPE_Click(object sender, EventArgs e)
        {
            Button pb = (Button)sender;
            if (pb.ID == "pbDismiss") return;
            OpsSchedDataContext OSdc = new OpsSchedDataContext();
            FSSIGNUP fss = new FSSIGNUP();
            var qm = (from s in OSdc.FSSIGNUPs
                      where s.iDate == ilDate && s.iCateg == ilCateg && s.iPerson == ilPerson
                      select s).ToList();
            if (qm.Count() > 0)
            {
                fss = qm.First();
                switch (pb.ID)
                {
                    case "pbUpdate":
                        fss.sNameInSchedule = Server.HtmlEncode(txbOther.Text); // SCR 221
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
                    fss.iPerson = ilPerson;
                    fss.sNameInSchedule = Server.HtmlEncode(txbOther.Text); // SCR 221
                    fss.sRemarks = Server.HtmlEncode(txbRemarks.Text); // SCR 221
                    OSdc.FSSIGNUPs.InsertOnSubmit(fss);
                }
            }
            OSdc.SubmitChanges();
            FillOSTables();
        }

        protected void DDLDate_SelectedIndexChanged(object sender, EventArgs e)
        {
            ilDate = Int32.Parse(((DropDownList)sender).SelectedValue);
            OpsSchedDataContext OSdc = new OpsSchedDataContext();
            DCurr = (from d in OSdc.FSDATEs where d.ID == ilDate select d.Date).First();
            FillOSTables();
        }

        protected void DDLDate_PreRender(object sender, EventArgs e)
        {
            Set_DropDown_ByValue(DDLDate, ilDate.ToString());
        }
    }
}