using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TSoar.Accounting;
using TSoar.DB;

namespace TSoar.Operations
{
    public partial class OpsSchedDates : System.Web.UI.Page
    {
        SCUD_Multi mCRUD = new SCUD_Multi();

        #region ViewState and Session properties
        private bool bEditExiDatesRow
        {
            get { return GetbEditExistingRow("bEditExiDatesRow"); }
            set { ViewState["bEditExiDatesRow"] = value; }
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
        private int iNgvRows { get { return iGetInt("iNgvRows"); } set { ViewState["iNgvRows"] = value; } }
        private int iEdRow { get { return iGetInt("iEdRow"); } set { ViewState["iEdRow"] = value; } }
        private int iIndexOfLastPage { get { return iGetInt("iIndexOfLastPage"); } set { ViewState["iIndexOfLastPage"] = value; } }
        private int iGetInt(string suInt)
        {
            if (ViewState[suInt] is null)
            {
                if (suInt == "iEdRow") return -1;
                return 0;
            }
            else
            {
                return (int)ViewState[suInt];
            }
        }
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                iEdRow = -1;
                FillDatesTable();
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
            if (btn.ID == "YesButton")
            {
                switch (btn.CommandName)
                {
                    case "Delete":
                        switch (OkButton.CommandArgument)
                        {
                            case "FOSDate":
                                // Delete a Flight Operations Schedule date
                                try
                                {
                                    mCRUD.DeleteOne(Global.enugInfoType.FltOpsSchedDates, btn.CommandArgument);
                                }
                                catch (Global.excToPopup exc)
                                {
                                    ProcessPopupException(exc);
                                }
                                iEdRow = -1;
                                FillDatesTable();
                                break;
                        }
                        break;
                    case "AddDates":
                        AddDates();
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

        private void FillDatesTable()
        {
            List<DataRow> liFOSDates = AssistLi.Init(Global.enLL.FltOpsSchedDates);
            Session["liFOSDates"] = liFOSDates;
            iNgvRows = liFOSDates.Count; // number of rows in gvFOSDates
            if (iEdRow == -1)
            {
                iEdRow = iNgvRows - 1;
            }
            gvFOSDates.PageSize = 35;
            int iNewEditIndex = iEdRow % gvFOSDates.PageSize;
            int iIndexOfEditPage = iEdRow / gvFOSDates.PageSize;
            iIndexOfLastPage = iNgvRows / gvFOSDates.PageSize;
            if ((iNgvRows % gvFOSDates.PageSize) == 0)
            {
                iIndexOfLastPage--;
            }
            // Is the row being edited in the current page?
            if (gvFOSDates.PageIndex == iIndexOfEditPage)
            {
                GridViewEditEventArgs gvee = new GridViewEditEventArgs(iNewEditIndex);
                gvFOSDates_RowEditing(null, gvee);
            }
            else
            {
                gvFOSDates.EditIndex = -1;
                bEditExiDatesRow = false;
                dt_BindTogvFOSDates(liFOSDates.CopyToDataTable());
            }
        }

        protected void gvFOSDates_RowEditing(object sender, GridViewEditEventArgs e)
        {
            GridView gv = gvFOSDates;
            gv.EditIndex = e.NewEditIndex;
            iEdRow = gv.PageSize * gv.PageIndex + gv.EditIndex;
            List<DataRow> liFOSDates = (List<DataRow>)Session["liFOSDates"];
            bEditExiDatesRow = false;
            if (iEdRow < iNgvRows - 1)
            {
                // We are editing an existing row; need to get rid of the last row which would be the New row (but there is no New row)
                liFOSDates.RemoveAt(liFOSDates.Count - 1);
                bEditExiDatesRow = true;
            }
            DataTable dtFOSDates = liFOSDates.CopyToDataTable();
            dt_BindTogvFOSDates(dtFOSDates);
        }

        private void dt_BindTogvFOSDates(DataTable dtu)
        {
            GridView gv = gvFOSDates;
            gv.Visible = true;
            DataView view = new DataView(dtu)
            {
                Sort = "i1Sort ASC, Date ASC, sNote ASC"
            };
            gv.DataSource = view;
            gv.DataBind();
        }

        protected void gvFOSDates_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                //EquipmentDataContext eqdc = new EquipmentDataContext();
                if ((e.Row.RowState & DataControlRowState.Edit) != 0)
                {
                    int iLast = iNgvRows - 1; // The standard editing row
                    int iColAddButton = gvFOSDates.Columns.Count - 2; // Column of the Edit and Add buttons
                    if (!bEditExiDatesRow)
                    {
                        // Last row has no button except the Edit button:
                        e.Row.Cells[iColAddButton + 1].Visible = false;
                        // Last row has an Add button
                        ImageButton pb = (ImageButton)e.Row.FindControl("ipbEUpdate");
                        pb.ImageUrl = "~/i/YellowAddButton.jpg";
                        e.Row.BackColor = System.Drawing.Color.LightGray;
                        e.Row.BorderStyle = BorderStyle.Ridge;
                        e.Row.BorderWidth = 5;
                        e.Row.BorderColor = System.Drawing.Color.Orange;
                        e.Row.Cells[iColAddButton].BorderStyle = BorderStyle.Ridge;
                        e.Row.Cells[iColAddButton].BorderWidth = 5;
                        e.Row.Cells[iColAddButton].BorderColor = System.Drawing.Color.Orange;
                        // No Cancel button in the last row
                        ImageButton pbC = (ImageButton)e.Row.FindControl("ipbECancel");
                        pbC.Visible = false;
                        Label lblIIdent = (Label)e.Row.FindControl("lblIIdent");
                        lblIIdent.Text = "New";
                        Label lblIDOW = (Label)e.Row.FindControl("lblIDOW");
                        lblIDOW.Visible = false;
                    }
                }
                else
                {
                    if (bEditExiDatesRow)
                    {
                        ImageButton ipbEEdit = (ImageButton)e.Row.FindControl("ipbEEdit");
                        ImageButton ipbEDelete = (ImageButton)e.Row.FindControl("ipbEDelete");
                        ipbEEdit.Visible = false;
                        ipbEDelete.Visible = false;
                    }
                }
            }
        }

        protected void gvFOSDates_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            e.Cancel = true;
            string sItem = ((Label)gvFOSDates.Rows[e.RowIndex].FindControl("lblIIdent")).Text;
            ButtonsClear();
            YesButton.CommandName = "Delete";
            YesButton.CommandArgument = sItem;
            OkButton.CommandArgument = "FOSDate";
            lblPopupText.Text = "A Flight Operations Schedule Date is not usually deleted, particularly when there are singups associated with it." +
                "Any signups that go with this Date will also be deleted. " +
                " If you still want to delete this Date, please confirm deletion of Flight Operations Schedule Date with internal Id " + YesButton.CommandArgument;
            MPE_Show(Global.enumButtons.NoYes);
        }

        protected void gvFOSDates_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            OpsSchedDataContext dc = new OpsSchedDataContext();
            string sNote = Server.HtmlEncode(((TextBox)gvFOSDates.Rows[e.RowIndex].FindControl("txbEComment")).Text.Replace("'", "`"));
            DateTime DDate = DateTime.Parse(((TextBox)gvFOSDates.Rows[e.RowIndex].FindControl("txbDDate")).Text);
            bool bEnabled = ((CheckBox)gvFOSDates.Rows[e.RowIndex].FindControl("chbDEnabled")).Checked;

            if (!bEditExiDatesRow)
            {
                // Insert a new date
                int iCnt = (from d in dc.FSDATEs where d.Date == DDate select d).Count();
                if (iCnt > 0)
                {
                    ProcessPopupException(new Global.excToPopup("The date " + DDate.ToString("yyyy/MM/dd") +
                        " will occur multiple times in the list of Flight Operations Schedule dates. " +
                        "Make sure you use the 'Notes' to distinguish between them, for example " +
                        "by indicating the location of flight operations."));
                    //gvFOSDates.EditIndex = -1;
                    //iEdRow = -1;
                    //bEditExiDatesRow = false;
                    //FillDatesTable();
                    //return;
                }
                FSDATE f = new FSDATE
                {
                    sNote = sNote,
                    Date = DDate,
                    bEnabled = bEnabled
                };
                dc.FSDATEs.InsertOnSubmit(f);
            }
            else
            {
                // Update an existing date
                int iID = Int32.Parse(((Label)gvFOSDates.Rows[e.RowIndex].FindControl("lblIIdent")).Text);
                var f = (from v in dc.FSDATEs where v.ID == iID select v).First();
                f.sNote = sNote;
                f.Date = DDate;
                f.bEnabled = bEnabled;
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
            gvFOSDates.EditIndex = -1;
            iEdRow = -1;
            bEditExiDatesRow = false;
            FillDatesTable();
        }

        protected void gvFOSDates_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvFOSDates.EditIndex = -1;
            bEditExiDatesRow = false;
            iEdRow = -1;
            FillDatesTable();
        }

        protected void pbAddDates_Click(object sender, EventArgs e)
        {
            OpsSchedDataContext dc = new OpsSchedDataContext();
            DateTime qd = DateTime.Today;
            try
            {
                qd = (from d in dc.FSDATEs orderby d.Date descending select d.Date).First();
            }
            catch
            {

            }
            ButtonsClear();
            lblPopupText.Text = "Are you sure you want to add a year's worth of weekend dates after " + CustFmt.sFmtDate(qd, CustFmt.enDFmt.DateOnly) + "?";
            YesButton.CommandName = "AddDates";
            MPE_Show(Global.enumButtons.NoYes);
        }
        private void AddDates()
        {
            // Get latest date in the list
            OpsSchedDataContext dc = new OpsSchedDataContext();
            DateTime qd = DateTime.Today;
            try
            {
                qd = (from d in dc.FSDATEs orderby d.Date descending select d.Date).First();
            }
            catch
            {

            }
            // Keep adding one day; if it's a weekend day then add it to the list unless it's off-season
            DateTime DEnd = qd.AddYears(1).AddDays(1);
            do
            {
                qd = qd.AddDays(1);
                if ((qd.DayOfWeek == DayOfWeek.Saturday || qd.DayOfWeek == DayOfWeek.Sunday) && (qd.Month > 2) && (qd.Month < 11))
                {
                    FSDATE fSDATE = new FSDATE()
                    {
                        Date = qd,
                        bEnabled = true,
                        sNote = ""
                    };
                    dc.FSDATEs.InsertOnSubmit(fSDATE);
                }
            } while (qd <= DEnd);
            dc.SubmitChanges();
            iEdRow = -1;
            FillDatesTable();
        }

        protected void gvFOSDates_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvFOSDates.PageIndex = e.NewPageIndex;
            FillDatesTable();
        }
    }
}