using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TSoar.Accounting;
using TSoar.DB;

namespace TSoar.Equipment.EquipAging
{
    public partial class EqAgingOperDat : System.Web.UI.Page
    {
        SCUD_Multi mCRUD = new SCUD_Multi();

        #region ViewState Variables
        #region Booleans
        private bool bEditExistingOpDataRow
        {
            get { return GetbEditExisting("bEditExistingOpDataRow"); }
            set { ViewState["bEditExistingOpDataRow"] = value; }
        }
        private bool GetbEditExisting(string su)
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
        #region Integers
        private int iNgvOpDataRows { get { return iGetN("iNgvOpDataRows"); } set { ViewState["iNgvOpDataRows"] = value; } }
        private int iSelectedEqComp { get { return iGetN("iSelectedEqComp"); } set {ViewState["iSelectedEqComp"]=value;} }
        private int iGetN(string su)
        {
            if (ViewState[su] == null)
            {
                return 0;
            }
            else
            {
                return (int)ViewState[su];
            }
        }
        #endregion
        #region DateTimeOffset ViewStates
        private DateTimeOffset DToLast { get { return DGetToLast("DToLast"); } set { ViewState["DToLast"] = value; } }
        private DateTimeOffset DGetToLast(string su)
        {
            if (ViewState[su] == null)
            {
                return DateTimeOffset.MinValue;
            }
            else
            {
                return (DateTimeOffset)ViewState[su];
            }
        }
        #endregion
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                FillEqCompTable();
                FillDataTableOpData();
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
                            case "EqOpData":
                                // Delete the Equipment Operating Hours Record
                                try
                                {
                                    mCRUD.DeleteOne(Global.enugInfoType.EquipOpData, btn.CommandArgument);
                                }
                                catch (Global.excToPopup exc)
                                {
                                    ProcessPopupException(exc);
                                }
                                FillEqCompTable();
                                FillDataTableOpData();
                                break;
                        }
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

        #region Operating Data
        private void FillEqCompTable()
        {
            EquipmentDataContext eqdc = new EquipmentDataContext();
            int iCount = (from c in eqdc.EQUIPCOMPONENTs select c).Count();
            if (iCount < 1)
            {
                ProcessPopupException(new Global.excToPopup("There are no equipment components defined; cannot work with operating/running times data."));
                return;
            }

            List<DataRow> liEqComp = AssistLi.Init(Global.enLL.EquipComponents);
            // Throw away the last row because we don't add or edit any components here:
            liEqComp.RemoveAt(liEqComp.Count - 1);
            Session["liEqComp"] = liEqComp;
            DataView view = new DataView(liEqComp.CopyToDataTable())
            {
                Sort = "i1Sort ASC, sShortEquipName ASC"
            };
            gvEqComps.DataSource = view;
            gvEqComps.DataBind();
            if (iSelectedEqComp < 1)
            {
                // No equipment component has been selected. Let's select the first one
                if (gvEqComps.Rows.Count > 0)
                {
                    iSelectedEqComp = int.Parse(((Label)gvEqComps.Rows[0].FindControl("lblIIdent")).Text);
                }
                else
                {
                    iSelectedEqComp = -1;
                }
            }
            if (gvEqComps.Rows.Count > 0)
            {
                // Highlight the selected row
                foreach (GridViewRow r in gvEqComps.Rows)
                {
                    if (iSelectedEqComp == int.Parse(((Label)r.FindControl("lblIIdent")).Text))
                    {
                        r.BackColor = System.Drawing.Color.Yellow;
                        break;
                    }
                }
            }
        }

        private void Set_DropDown_ByText(DropDownList ddl, string suText)
        {
            ddl.ClearSelection();
            foreach (ListItem li in ddl.Items)
            {
                if (li.Text == suText)
                {
                    li.Selected = true;
                    break;
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

        private void FillDataTableOpData()
        {
            List<DataRow> liEqOpData = AssistLi.Init(Global.enLL.EquipOpData, iSelectedEqComp);
            Session["liEqOpData"] = liEqOpData;
            iNgvOpDataRows = liEqOpData.Count;
            GridViewEditEventArgs gvee = new GridViewEditEventArgs(iNgvOpDataRows - 1);
            gvEqOperDat_RowEditing(null, gvee);
        }

        protected void gvEqOperDat_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            bool bShowEquipMgtOpDataStartEndTimes = AccountProfile.CurrentUser.bShowEquipMgtOpDataStartEndTimes;
            switch (e.Row.RowType)
            {
                case DataControlRowType.Header:
                    Label lblHDFromTime = (Label)e.Row.FindControl("lblHDFromTime");
                    lblHDFromTime.Visible = bShowEquipMgtOpDataStartEndTimes;
                    Label lblHDFromOffset = (Label)e.Row.FindControl("lblHDFromOffset");
                    lblHDFromOffset.Visible = bShowEquipMgtOpDataStartEndTimes;
                    Label lblHToTime = (Label)e.Row.FindControl("lblHToTime");
                    lblHToTime.Visible = bShowEquipMgtOpDataStartEndTimes;
                    Label lblHToOffset = (Label)e.Row.FindControl("lblHToOffset");
                    lblHToOffset.Visible = bShowEquipMgtOpDataStartEndTimes;
                    break;
                case DataControlRowType.DataRow:
                    Label lblIStat = (Label)e.Row.FindControl("lblIStat");
                    if ((e.Row.RowState & DataControlRowState.Edit) != 0)
                    {
                        DropDownList DDLDUnitsDist = (DropDownList)e.Row.FindControl("DDLDUnitsDist");
                        DropDownList DDLDSource = (DropDownList)e.Row.FindControl("DDLDSource");
                        TextBox txbDFromTime = (TextBox)e.Row.FindControl("txbDFromTime");
                        txbDFromTime.Width = bShowEquipMgtOpDataStartEndTimes ? 80 : 1;
                        TextBox txbDFromOffset = (TextBox)e.Row.FindControl("txbDFromOffset");
                        txbDFromOffset.Width = bShowEquipMgtOpDataStartEndTimes ? 80 : 1;
                        TextBox txbDToTime = (TextBox)e.Row.FindControl("txbDToTime");
                        txbDToTime.Width = bShowEquipMgtOpDataStartEndTimes ? 80 : 1;
                        TextBox txbDToOffset = (TextBox)e.Row.FindControl("txbDToOffset");
                        txbDToOffset.Width = bShowEquipMgtOpDataStartEndTimes ? 80 : 1;

                        int iLast = iNgvOpDataRows - 1; // The standard editing row
                        int iColAddButton = gvEqOperDat.Columns.Count - 2; // Column of the Edit and Add buttons
                        if (!bEditExistingOpDataRow)
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
                        else
                        {
                            string sDDLitem = (string)DataBinder.Eval(e.Row.DataItem, "sDistanceUnits");
                            Set_DropDown_ByText(DDLDUnitsDist, sDDLitem);
                            Set_DropDown_ByValue(DDLDSource, DataBinder.Eval(e.Row.DataItem, "cSource").ToString());
                        }
                        lblIStat.Text = "";
                    }
                    else
                    {
                        if (bEditExistingOpDataRow)
                        {
                            ImageButton ipbEEdit = (ImageButton)e.Row.FindControl("ipbEEdit");
                            ImageButton ipbEDelete = (ImageButton)e.Row.FindControl("ipbEDelete");
                            ipbEEdit.Visible = false;
                            ipbEDelete.Visible = false;
                        }
                        else
                        {
                            // The following assumes that GridView is traversed from top to bottom as each RowDataBound event fires
                            if (e.Row.RowIndex > 0) // for all but the first row ...
                            {
                                DateTimeOffset DFrom = (DateTimeOffset)DataBinder.Eval(e.Row.DataItem, "DFrom");
                                TimeSpan tsp = DFrom - DToLast;
                                if (tsp < TimeSpan.Zero)
                                {
                                    lblIStat.Text = "X";
                                    lblIStat.ForeColor = System.Drawing.Color.Red; // warns of a problem (start time of an intervasl cannot be earlier than end time of preceding interval)
                                }
                                else if (tsp > new TimeSpan(3, 0, 0))
                                {
                                    lblIStat.Text = "G";
                                    lblIStat.ForeColor = System.Drawing.Color.DarkMagenta; // warns of a large gap
                                }
                                else
                                {
                                    lblIStat.Text = "O";
                                    lblIStat.ForeColor = System.Drawing.Color.Green; // interval boundaries are ok
                                }
                            }
                            DToLast = (DateTimeOffset)DataBinder.Eval(e.Row.DataItem, "DTo");
                        }
                        Label lblIFromTime = (Label)e.Row.FindControl("lblIDFromTime");
                        lblIFromTime.Visible = bShowEquipMgtOpDataStartEndTimes;
                        Label lblIFromOffset = (Label)e.Row.FindControl("lblIDFromOffset");
                        lblIFromOffset.Visible = bShowEquipMgtOpDataStartEndTimes;
                        Label lblIToTime = (Label)e.Row.FindControl("lblIToTime");
                        lblIToTime.Visible = bShowEquipMgtOpDataStartEndTimes;
                        Label lblIToOffset = (Label)e.Row.FindControl("lblIToOffset");
                        lblIToOffset.Visible = bShowEquipMgtOpDataStartEndTimes;
                    }
                    break;
            }
        }

        protected void gvEqOperDat_RowEditing(object sender, GridViewEditEventArgs e)
        {
            gvEqOperDat.EditIndex = e.NewEditIndex;
            List<DataRow> liEqOpData = (List<DataRow>)Session["liEqOpData"];
            bEditExistingOpDataRow = false;
            if (e.NewEditIndex < iNgvOpDataRows - 1)
            {
                // We are editing an existing row; need to get rid of the last row which would be the New row (but there is no New row)
                liEqOpData.RemoveAt(liEqOpData.Count - 1);
                bEditExistingOpDataRow = true;
            }
            DataTable dtOpData = liEqOpData.CopyToDataTable();
            DataView view = new DataView(dtOpData)
            {
                Sort = "i1Sort ASC, DFrom ASC"
            };
            gvEqOperDat.DataSource = view;
            gvEqOperDat.DataBind();
        }

        protected void gvEqOperDat_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            e.Cancel = true;
            Label lblItem = (Label)gvEqOperDat.Rows[e.RowIndex].FindControl("lblIIdent");
            string sItem = lblItem.Text;
            ButtonsClear();
            YesButton.CommandName = "Delete";
            YesButton.CommandArgument = sItem;
            OkButton.CommandArgument = "EqOpData";
            lblPopupText.Text = "Please confirm deletion of equipment operating data record with internal Id " + YesButton.CommandArgument;
            MPE_Show(Global.enumButtons.NoYes);
        }

        protected void gvEqOperDat_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvEqOperDat.EditIndex = -1;
            FillDataTableOpData();
        }

        protected void gvEqOperDat_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            EquipmentDataContext dc = new EquipmentDataContext();
            GridViewRow gvRow = gvEqOperDat.Rows[e.RowIndex];
            DropDownList DDLDUnitsDist = (DropDownList)gvRow.FindControl("DDLDUnitsDist");
            DateTimeOffset DFrom;
            DateTimeOffset DTo;
            try
            {
                DFrom = Time_Date.DTO_from3TextBoxes(gvRow, "From", "txbEDFromDate", "txbDFromTime", "txbDFromOffset");
                DTo = Time_Date.DTO_from3TextBoxes(gvRow, "To", "txbDToDate", "txbDToTime", "txbDToOffset");
            }
            catch (Global.excToPopup exctp)
            {
                ProcessPopupException(exctp);
                return;
            }
            catch (Exception exc)
            {
                ProcessPopupException(new Global.excToPopup(exc.Message));
                return;
            }

            if (DTo < DFrom)
            {
                ProcessPopupException(new Global.excToPopup("The 'To' Date " + DTo.ToString() + " must be later than or equal to the 'From' Date " + DFrom.ToString() + "."));
                return;
            }

            TextBox txbDDist = (TextBox)gvRow.FindControl("txbDOpDist");
            decimal dDistance = decimal.Parse(txbDDist.Text);

            DropDownList DDLDSource = (DropDownList)gvRow.FindControl("DDLDSource");
            char cSource = DDLDSource.SelectedItem.Value[0];
            if (cSource == 'R' && DFrom != DTo)
            {
                ProcessPopupException(new Global.excToPopup("A Reset record (Source = R) must have From and To dates the same, including time of day and offset."));
                return;
            }
            if (DFrom == DTo && cSource != 'R')
            {
                ProcessPopupException(new Global.excToPopup("When From and To dates are the same then this is a Reset record; the code for the Source must be set to 'R'."));
                return;
            }

            TextBox txbDOpHrs = (TextBox)gvRow.FindControl("txbEdHours");
            decimal dHours = decimal.Parse(txbDOpHrs.Text);

            TextBox txbECycl = (TextBox)gvRow.FindControl("txbECycl");
            int iCycles = int.Parse(txbECycl.Text);

            TimeSpan tsp = DTo - DFrom;
            if ((cSource != 'R') && ((double)dHours > tsp.TotalHours))
            {
                ProcessPopupException(new Global.excToPopup("Warning: Operating Hours = " + dHours.ToString() + " exceed the time between DFrom and Dto."));
            }

            string sComment = Server.HtmlEncode(((TextBox)gvRow.FindControl("txbEComment")).Text.Replace("'", "`").Trim());

            int iUser = mCRUD.GetPeopleIDfromWebSiteUserName(HttpContext.Current.User.Identity.Name);
            if (!bEditExistingOpDataRow)
            {
                EQUIPOPERDATA f = new EQUIPOPERDATA
                {
                    PiTRecordEntered = DateTime.UtcNow,
                    iRecordEnteredBy = iUser,
                    iEquipComponent = iSelectedEqComp,
                    DFrom = DFrom,
                    DTo = DTo,
                    dHours = dHours,
                    iCycles = iCycles,
                    dDistance = dDistance,
                    sDistanceUnits = DDLDUnitsDist.SelectedItem.Text,
                    cSource = cSource,
                    sComment = Server.HtmlEncode(sComment)
                };

                dc.EQUIPOPERDATAs.InsertOnSubmit(f);
            }
            else
            {
                int iID = Int32.Parse(((Label)gvRow.FindControl("lblIIdent")).Text);
                var f = (from v in dc.EQUIPOPERDATAs where v.ID == iID select v).First();
                f.PiTRecordEntered = DateTime.UtcNow;
                f.iRecordEnteredBy = iUser;
                f.iEquipComponent = iSelectedEqComp;
                f.DFrom = DFrom;
                f.DTo = DTo;
                f.dHours = dHours;
                f.iCycles = iCycles;
                f.dDistance = dDistance;
                f.sDistanceUnits = DDLDUnitsDist.SelectedItem.Text;
                f.cSource = cSource;
                f.sComment = Server.HtmlEncode(sComment);
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
            gvEqOperDat.EditIndex = -1;
            FillDataTableOpData();
            FillEqCompTable();
        }
        #endregion

        protected void gvEqComps_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                // How many ops data rows are associated with this component in table EQUIPOPERDATA?
                int iID = int.Parse(((Label)e.Row.FindControl("lblIIdent")).Text);
                EquipmentDataContext eqdc = new EquipmentDataContext();
                int iCount = (from c in eqdc.EQUIPOPERDATAs where c.iEquipComponent == iID select c).Count();
                Label lblICount = (Label)e.Row.FindControl("lblICount");
                lblICount.Text = iCount.ToString();
            }
        }

        protected void gvEqComps_SelectedIndexChanged(object sender, EventArgs e)
        {
            iSelectedEqComp = int.Parse(((Label)gvEqComps.Rows[gvEqComps.SelectedIndex].FindControl("lblIIdent")).Text);
            foreach (GridViewRow r in gvEqComps.Rows)
            {
                r.BackColor = System.Drawing.Color.FromArgb(0xEDF8D7);
            }
            gvEqComps.Rows[gvEqComps.SelectedIndex].BackColor = System.Drawing.Color.Yellow;
            FillDataTableOpData();
        }

    }
}