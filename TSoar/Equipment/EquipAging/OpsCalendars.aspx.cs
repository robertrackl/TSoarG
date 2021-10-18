using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TSoar.Accounting;
using TSoar.DB;

namespace TSoar.Equipment
{
    public partial class OpsCalendars : System.Web.UI.Page
    {
        SCUD_Multi mCRUD = new SCUD_Multi();

        #region boolean variables
        private bool bEditExiNamesRow
        {
            get { return GetbEditExistingRow("bEditExiNamesRow"); }
            set { ViewState["bEditExiNamesRow"] = value; }
        }
        private bool bEditExiTimesRow
        {
            get { return GetbEditExistingRow("bEditExiTimesRow"); }
            set { ViewState["bEditExiTimesRow"] = value; }
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
        #region Integers
        private int iNgvNamesRows { get { return iGetInt("iNgvNamesRows"); } set { ViewState["iNgvNamesRows"] = value; } }
        private int iNgvTimesRows { get { return iGetInt("iNgvTimesRows"); } set { ViewState["iNgvTimesRows"] = value; } }
        private int iGetInt(string suInt)
        {
            if (ViewState[suInt] is null)
            {
                return -1;
            }
            else
            {
                return (int)ViewState[suInt];
            }
        }
        #endregion
        private int iSelectedOpsCal { get { return iGetSessionInt("iSelectedOpsCal", -1); } set { Session["iSelectedOpsCal"] = value; } }
        private int iGetSessionInt(string suInt, int iuIfNull )
        {
            if (Session[suInt] is null)
            {
                return iuIfNull;
            }
            else
            {
                return (int)Session[suInt];
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
                if (btn.CommandName == "Delete")
                {

                    switch (OkButton.CommandArgument)
                    {
                        case "OpsCal":
                            // Delete the operational calendar names record
                            try
                            {
                                mCRUD.DeleteOne(Global.enugInfoType.OpsCalNames, btn.CommandArgument);
                            }
                            catch (Global.excToPopup exc)
                            {
                                ProcessPopupException(exc);
                            }
                            iSelectedOpsCal = -1;
                            FillNamesTable();
                            break;
                        case "OpsTim":
                            // Delete the operational calendar times record
                            try
                            {
                                mCRUD.DeleteOne(Global.enugInfoType.OpsCalTimes, btn.CommandArgument);
                            }
                            catch (Global.excToPopup exc)
                            {
                                ProcessPopupException(exc);
                            }
                            EquipmentDataContext dc = new EquipmentDataContext();
                            if ((from v in dc.OPSCALTIMEs where v.iOpsCal == iSelectedOpsCal select v).Count() < 2)
                            {
                                ProcessPopupException(new Global.excToPopup("WARNING: An operational calendar must have at least two entries in order to specify at least one time interval."));
                            }
                            FillTimesTable();
                            break;
                    }
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

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                EquipmentDataContext eqdc = new EquipmentDataContext();
                if (iSelectedOpsCal < 0)
                {
                    var q = (from o in eqdc.OPSCALNAMEs where o.bStandard select o).ToList();
                    if (q.Count > 0)
                    {
                        iSelectedOpsCal = q.First().ID;
                    }
                }
                FillNamesTable();
            }
        }

        private void FillNamesTable()
        {
            List<DataRow> liOpsCalNames = AssistLi.Init(Global.enLL.OpsCalendarNames);
            Session["liOpsCalNames"] = liOpsCalNames;
            iNgvNamesRows = liOpsCalNames.Count; // number of rows in gvOpsCalNames
            GridViewEditEventArgs gvee = new GridViewEditEventArgs(iNgvNamesRows - 1);
            gvOpsCalNames_RowEditing(null, gvee);
        }

        private void FillTimesTable()
        {
            List<DataRow> liOpsCalTimes = AssistLi.Init(Global.enLL.OpsCalendarTimes, iSelectedOpsCal);
            Session["liOpsCalTimes"] = liOpsCalTimes;
            iNgvTimesRows = liOpsCalTimes.Count; // number of rows in gvOpsCalTimes
            GridViewEditEventArgs gvee = new GridViewEditEventArgs(iNgvTimesRows - 1);
            gvOpsCalTimes_RowEditing(null, gvee);
        }

        protected void gvOpsCalNames_RowEditing(object sender, GridViewEditEventArgs e)
        {
            gvOpsCalNames.EditIndex = e.NewEditIndex;
            List<DataRow> liOpsCalNames = (List<DataRow>)Session["liOpsCalNames"];
            bEditExiNamesRow = false;
            if (e.NewEditIndex < iNgvNamesRows - 1)
            {
                // We are editing an existing row; need to get rid of the last row which would be the New row (but there is no New row)
                liOpsCalNames.RemoveAt(liOpsCalNames.Count - 1);
                bEditExiNamesRow = true;
            }
            DataTable dtOpsCalNames = liOpsCalNames.CopyToDataTable();
            dt_BindTogvOpsCalNames(dtOpsCalNames);
            FillTimesTable();
        }

        private void dt_BindTogvOpsCalNames(DataTable dtu)
        {
            GridView GV = gvOpsCalNames;
            GV.Visible = true;
            DataView view = new DataView(dtu)
            {
                Sort = "i1Sort ASC, sOpsCalName ASC"
            };
            GV.DataSource = view;
            GV.DataBind();
            if (iSelectedOpsCal < 0)
            {
                // No ops calendar has been selected. Let's select the first one
                if (GV.Rows.Count > 1)
                {
                    iSelectedOpsCal = int.Parse(((Label)GV.Rows[0].FindControl("lblIIdent")).Text);
                }
                else
                {
                    iSelectedOpsCal = -1;
                }
            }
            if (GV.Rows.Count > 1)
            {
                // Highlight the selected row
                foreach(GridViewRow r in GV.Rows)
                {
                    try
                    {
                        if (iSelectedOpsCal == int.Parse(((Label)r.FindControl("lblIIdent")).Text))
                        {
                            r.BackColor = System.Drawing.Color.Yellow;
                            break;
                        }
                    }
                    catch { } // Ignore any errors
                }
            }
            else
            {
                iSelectedOpsCal = -1;
            }
            HeaderText();
        }

        private void HeaderText()
        {
            if (iSelectedOpsCal > 0)
            {
                EquipmentDataContext eqdc = new EquipmentDataContext();
                lblTHOC.Text = "Contents of Selected Operational Calendar " + (char)34 +
                    (from c in eqdc.OPSCALNAMEs where c.ID == iSelectedOpsCal select c.sOpsCalName).First() +
                    (char)34;
            }
        }

        protected void gvOpsCalNames_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                EquipmentDataContext eqdc = new EquipmentDataContext();
                if ((e.Row.RowState & DataControlRowState.Edit) != 0)
                {
                    int iLast = iNgvNamesRows - 1; // The standard editing row
                    int iColAddButton = gvOpsCalNames.Columns.Count - 2; // Column of the Edit and Add buttons
                    if (!bEditExiNamesRow)
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
                    }
                }
                else
                {
                    if (bEditExiNamesRow)
                    {
                        ImageButton ipbEEdit = (ImageButton)e.Row.FindControl("ipbEEdit");
                        ImageButton ipbEDelete = (ImageButton)e.Row.FindControl("ipbEDelete");
                        ipbEEdit.Visible = false;
                        ipbEDelete.Visible = false;
                    }
                }
            }
        }

        protected void gvOpsCalNames_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            e.Cancel = true;
            string sMsg = "You should probably NOT DELETE this operational calendar record because it is referenced from other data tables. " +
                "Those other records would also be wiped out (and quite possibly more data if there are more related tables): ";
            int iCount = 0;
            string sID = ((Label)((GridView)sender).Rows[e.RowIndex].FindControl("lblIIdent")).Text;
            int iID = Int32.Parse(sID);
            EquipmentDataContext eqdc = new EquipmentDataContext();

            iCount = (from c in eqdc.EQUIPAGINGITEMs where c.iOpCal == iID select c).Count();
            if (iCount > 0)
            {
                ProcessPopupException(new Global.excToPopup("Operational calendar with ID=" + sID + " is is referenced by one or more aging items. Cannot delete."));
                return;
            }

            try
            {
                var dtEq = eqdc.spForeignKeyRefs("OPSCALNAMES", iID);
                foreach (var row in dtEq)
                {
                    iCount += (int)row.iNumFKRefs;
                    sMsg += row.iNumFKRefs.ToString() + " times in " + row.sFKTable + ", ";
                }
                sMsg = sMsg.Substring(0, sMsg.Length - 2) + ". You should click on `No` unless you really know what you are doing!";
            }
            catch (Exception exc)
            {
                ProcessPopupException(new Global.excToPopup(exc.Message));
                return;
            }

            Label lblItem = (Label)gvOpsCalNames.Rows[e.RowIndex].FindControl("lblIIdent");
            string sItem = lblItem.Text;
            ButtonsClear();
            YesButton.CommandName = "Delete";
            YesButton.CommandArgument = sItem;
            OkButton.CommandArgument = "OpsCal";
            if (iCount < 1)
            {
                lblPopupText.Text = "Please confirm deletion of operational calendar with internal Id " + YesButton.CommandArgument;
            }
            else
            {
                lblPopupText.Text = sMsg;
            }
            MPE_Show(Global.enumButtons.NoYes);
        }

        protected void gvOpsCalNames_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            EquipmentDataContext dc = new EquipmentDataContext();
            string sOpsCalName = Server.HtmlEncode(((TextBox)gvOpsCalNames.Rows[e.RowIndex].FindControl("txbOCName")).Text.Replace("'", "`"));
            bool bStandard = ((CheckBox)gvOpsCalNames.Rows[e.RowIndex].FindControl("chbEStd")).Checked;

            if (!bEditExiNamesRow)
            {
                OPSCALNAME f = new OPSCALNAME
                {
                    sOpsCalName = sOpsCalName,
                    bStandard = bStandard
                };
                dc.OPSCALNAMEs.InsertOnSubmit(f);
            }
            else
            {
                int iID = Int32.Parse(((Label)gvOpsCalNames.Rows[e.RowIndex].FindControl("lblIIdent")).Text);
                var f = (from v in dc.OPSCALNAMEs where v.ID == iID select v).First();
                f.sOpsCalName = sOpsCalName;
                f.bStandard = bStandard;
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
            int iCount = (from c in dc.OPSCALNAMEs where c.bStandard select c).Count();
            if (iCount != 1)
            {
                ProcessPopupException(new Global.excToPopup("WARNING: The number of operational calendars marked 'Standard' is not 1 but " + iCount.ToString()));
            }

            dc.Dispose();
            gvOpsCalNames.EditIndex = -1;
            bEditExiNamesRow = false;
            FillNamesTable();
        }

        protected void gvOpsCalNames_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvOpsCalNames.EditIndex = -1;
            bEditExiNamesRow = false;
            FillNamesTable();
        }

        protected void gvOpsCalNames_SelectedIndexChanged(object sender, EventArgs e)
        {
            iSelectedOpsCal = int.Parse(((Label)gvOpsCalNames.Rows[gvOpsCalNames.SelectedIndex].FindControl("lblIIdent")).Text);
            foreach (GridViewRow r in gvOpsCalNames.Rows)
            {
                r.BackColor = System.Drawing.Color.FromArgb(0xEDF8D7);
            }
            gvOpsCalNames.Rows[gvOpsCalNames.SelectedIndex].BackColor = System.Drawing.Color.Yellow;
            EquipmentDataContext dc = new EquipmentDataContext();
            if ((from v in dc.OPSCALTIMEs where v.iOpsCal == iSelectedOpsCal select v).Count() < 2)
            {
                ProcessPopupException(new Global.excToPopup("WARNING: An operational calendar must have at least two entries in order to specify at least one time interval."));
            }
            HeaderText();
            FillTimesTable();
        }

        protected void gvOpsCalTimes_RowEditing(object sender, GridViewEditEventArgs e)
        {
            gvOpsCalTimes.EditIndex = e.NewEditIndex;
            List<DataRow> liOpsCalTimes = (List<DataRow>)Session["liOpsCalTimes"];
            bEditExiTimesRow = false;
            if (e.NewEditIndex < iNgvTimesRows - 1)
            {
                // We are editing an existing row; need to get rid of the last row which would be the New row (but there is no New row)
                liOpsCalTimes.RemoveAt(liOpsCalTimes.Count - 1);
                bEditExiTimesRow = true;
            }
            DataTable dtOpsCalTimes = liOpsCalTimes.CopyToDataTable();
            dt_BindTogvOpsCalTimes(dtOpsCalTimes);
        }

        private void dt_BindTogvOpsCalTimes(DataTable dtu)
        {
            GridView GV = gvOpsCalTimes;
            GV.Visible = true;
            DataView view = new DataView(dtu)
            {
                Sort = "i1Sort ASC, DStart ASC"
            };
            GV.DataSource = view;
            GV.DataBind();
        }

        protected void gvOpsCalTimes_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                EquipmentDataContext eqdc = new EquipmentDataContext();
                if ((e.Row.RowState & DataControlRowState.Edit) != 0)
                {
                    int iLast = iNgvTimesRows - 1; // The standard editing row
                    int iColAddButton = gvOpsCalTimes.Columns.Count - 2; // Column of the Edit and Add buttons
                    if (!bEditExiTimesRow)
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
                    }
                }
                else
                {
                    if (bEditExiTimesRow)
                    {
                        ImageButton ipbEEdit = (ImageButton)e.Row.FindControl("ipbEEdit");
                        ImageButton ipbEDelete = (ImageButton)e.Row.FindControl("ipbEDelete");
                        ipbEEdit.Visible = false;
                        ipbEDelete.Visible = false;
                    }
                }
            }
        }

        protected void gvOpsCalTimes_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            e.Cancel = true;
            string sItem = ((Label)gvOpsCalTimes.Rows[e.RowIndex].FindControl("lblIIdent")).Text;
            ButtonsClear();
            YesButton.CommandName = "Delete";
            YesButton.CommandArgument = sItem;
            OkButton.CommandArgument = "OpsTim";
            lblPopupText.Text = "Please confirm deletion of operational calendar record with internal Id " + YesButton.CommandArgument;
            MPE_Show(Global.enumButtons.NoYes);
        }

        protected void gvOpsCalTimes_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            EquipmentDataContext dc = new EquipmentDataContext();
            string sComment = Server.HtmlEncode(((TextBox)gvOpsCalTimes.Rows[e.RowIndex].FindControl("txbEComment")).Text.Replace("'", "`"));
            bool bOpStatus = ((CheckBox)gvOpsCalTimes.Rows[e.RowIndex].FindControl("chbEOpSt")).Checked;
            DateTimeOffset DStart = Time_Date.DTO_from3TextBoxes(gvOpsCalTimes.Rows[e.RowIndex], "Start", "txbDFromDate", "txbDFromTime", "txbDFromOffset");

            if (!bEditExiTimesRow)
            {
                OPSCALTIME f = new OPSCALTIME
                {
                    sComment = sComment,
                    bOpStatus = bOpStatus,
                    DStart = DStart,
                    iOpsCal = iSelectedOpsCal
                };
                dc.OPSCALTIMEs.InsertOnSubmit(f);
            }
            else
            {
                int iID = Int32.Parse(((Label)gvOpsCalTimes.Rows[e.RowIndex].FindControl("lblIIdent")).Text);
                var f = (from v in dc.OPSCALTIMEs where v.ID == iID select v).First();
                f.sComment = sComment;
                f.bOpStatus = bOpStatus;
                f.DStart = DStart;
                f.iOpsCal = iSelectedOpsCal;
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
            if ((from v in dc.OPSCALTIMEs where v.iOpsCal==iSelectedOpsCal select v).Count() < 2)
            {
                ProcessPopupException(new Global.excToPopup("WARNING: An operational calendar must have at least two entries in order to specify at least one time interval."));
            }
            dc.Dispose();
            gvOpsCalTimes.EditIndex = -1;
            bEditExiTimesRow = false;
            FillTimesTable();
        }

        protected void gvOpsCalTimes_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvOpsCalTimes.EditIndex = -1;
            bEditExiTimesRow = false;
            FillTimesTable();
        }
    }
}