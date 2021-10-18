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
    public partial class EqAgingItems : System.Web.UI.Page
    {
        SCUD_Multi mCRUD = new SCUD_Multi();

        #region ViewState Items
        private bool bEditExistingEqAgItemsRow
        {
            get { return GetbEditExisting("bEditExistingEqAgItemsRow"); }
            set { ViewState["bEditExistingEqAgItemsRow"] = value; }
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
        private int iNgvEqAgItemsRows { get { return iGetN("iNgvEqAgItemsRows"); } set { ViewState["iNgvEqAgItemsRows"] = value; } }
        private int iEdRow { get { return iGetN("iEdRow"); } set { ViewState["iEdRow"] = value; } }
        private int iIndexOfLastPage { get { return iGetN("iIndexOfLastPage"); } set { ViewState["iIndexOfLastPage"] = value; } }
        private int iGetN(string suN)
        {
            if (ViewState[suN] == null)
            {
                if (suN == "iEdRow") return -1;
                return 0;
            }
            else
            {
                return (int)ViewState[suN];
            }
        }
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                iEdRow = -1;
                FillDataTableEqAgItems();
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
                            case "AgItems":
                                // Delete the Equipment Aging Item record
                                try
                                {
                                    mCRUD.DeleteOne(Global.enugInfoType.EquipAgingItems, btn.CommandArgument);
                                }
                                catch (Global.excToPopup exc)
                                {
                                    ProcessPopupException(exc);
                                }
                                FillDataTableEqAgItems();
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

        #region Related to gvAgItems
        private void FillDataTableEqAgItems()
        {
            GridView GV = gvAgItems;
            EquipmentDataContext eqdc = new EquipmentDataContext();
            if ((from c in eqdc.EQUIPCOMPONENTs select c).Count() < 1)
            {
                ProcessPopupException(new Global.excToPopup("It looks like you have not defined any equipment components. Cannot define Aging Items without components."));
                return;
            }
            if ((from o in eqdc.OPSCALNAMEs select o).Count() < 1)
            {
                ProcessPopupException(new Global.excToPopup("It looks like you have not defined any operational calendars. Cannot define Aging Items without operational calendars."));
                return;
            }
            if ((from p in eqdc.EQUIPAGINGPARs select p).Count() < 1)
            {
                ProcessPopupException(new Global.excToPopup("It looks like you have not defined any aging parameter sets. Cannot define Aging Items without parameter sets."));
                return;
            }
            List<DataRow> li = AssistLi.Init(Global.enLL.EquipAgingItems);
            Session["liEqAgItems"] = li;
            iNgvEqAgItemsRows = li.Count;
            if (iEdRow == -1)
            {
                iEdRow = iNgvEqAgItemsRows - 1;
            }

            GV.PageSize = Int32.Parse(mCRUD.GetSetting("PageSizeEquipAgingItems"));
            int iNewEditIndex = iEdRow % GV.PageSize;
            int iIndexOfEditPage = iEdRow / GV.PageSize;
            iIndexOfLastPage = iNgvEqAgItemsRows / GV.PageSize;
            if ((iNgvEqAgItemsRows % GV.PageSize) == 0)
            {
                iIndexOfLastPage--;
            }
            // Is the row being edited in the current page?
            if (GV.PageIndex == iIndexOfEditPage)
            {
                GridViewEditEventArgs gvee = new GridViewEditEventArgs(iNewEditIndex);
                gvAgItems_RowEditing(null, gvee);
            }
            else
            {
                GV.EditIndex = -1;
                bEditExistingEqAgItemsRow = false;
                dt_BindTogvEqAgItems(li.CopyToDataTable());
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

        protected void gvAgItems_RowEditing(object sender, GridViewEditEventArgs e)
        {
            GridView gv = gvAgItems;
            gv.EditIndex = e.NewEditIndex;
            iEdRow = gv.PageSize * gv.PageIndex + gv.EditIndex;
            List<DataRow> li = (List<DataRow>)Session["liEqAgItems"];
            bEditExistingEqAgItemsRow = false;
            if (iEdRow < iNgvEqAgItemsRows - 1)
            {
                // We are editing an existing row.
                // need to get rid of the last row which would be the New row (but there is no New row)
                li.RemoveAt(li.Count - 1);
                bEditExistingEqAgItemsRow = true;
            }
            if (li.Count() > 0)
            {
                dt_BindTogvEqAgItems(li.CopyToDataTable());
            }
        }

        private void dt_BindTogvEqAgItems(DataTable dtu)
        {
            GridView GV = gvAgItems;
            GV.Visible = true;
            DataView view = new DataView(dtu)
            {
                Sort = "i1Sort ASC, sComponent ASC, sName ASC"
            };
            GV.DataSource = view;
            GV.DataBind();
        }

        protected void gvAgItems_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            bool bShowEqAgItemsStartEndTimes = AccountProfile.CurrentUser.bShowEqAgItemsStartEndTimes;
            switch (e.Row.RowType)
            {
                case DataControlRowType.Header:
                    foreach (string s in new string[] { "lblHStartTime", "lblHStartOffset", "lblHEndTime", "lblHEndOffset" })
                    {
                        ((Label)e.Row.FindControl(s)).Visible = bShowEqAgItemsStartEndTimes;
                    }
                    break;
                case DataControlRowType.DataRow:
                    if ((e.Row.RowState & DataControlRowState.Edit) != 0)
                    {
                        foreach (string s in new string[] { "txbDStartTime", "txbDStartOffset", "txbDEndTime", "txbDEndOffset" })
                        {
                            ((TextBox)e.Row.FindControl(s)).Width = bShowEqAgItemsStartEndTimes ? 80 : 1;
                        }

                        int iLast = iNgvEqAgItemsRows - 1; // The standard editing row
                        int iColAddButton = gvAgItems.Columns.Count - 2; // Column of the Edit and Add buttons

                        EquipmentDataContext eqdc = new EquipmentDataContext();
                        DropDownList DDLCompName = (DropDownList)e.Row.FindControl("DDLCompName");
                        var qc = from c in eqdc.EQUIPCOMPONENTs orderby c.sComponent select new { c.ID, sName = EqSupport.sExpandedComponentName(c.ID) };
                        DDLCompName.DataSource = qc;
                        DDLCompName.DataValueField = "ID";
                        DDLCompName.DataTextField = "sName";
                        DDLCompName.DataBind();

                        DropDownList DDLEOpsCal = (DropDownList)e.Row.FindControl("DDLEOpsCal");
                        var qo = from o in eqdc.OPSCALNAMEs orderby o.sOpsCalName select o;
                        DDLEOpsCal.DataSource = qo;
                        DDLEOpsCal.DataValueField = "ID";
                        DDLEOpsCal.DataTextField = "sOpsCalName";
                        DDLEOpsCal.DataBind();

                        if (!bEditExistingEqAgItemsRow)
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
                            string sDDLitem = DataBinder.Eval(e.Row.DataItem, "sOpsCalName").ToString();
                            Set_DropDown_ByText(DDLEOpsCal, sDDLitem);

                            sDDLitem = DataBinder.Eval(e.Row.DataItem, "iEquipComponent").ToString();
                            Set_DropDown_ByValue(DDLCompName, sDDLitem);

                            DropDownList DDLParSets = (DropDownList)e.Row.FindControl("DDLParSets");
                            sDDLitem = DataBinder.Eval(e.Row.DataItem, "iParam").ToString();
                            Set_DropDown_ByValue(DDLParSets, sDDLitem);
                        }
                    }
                    else
                    {
                        if (bEditExistingEqAgItemsRow)
                        {
                            ImageButton ipbEEdit = (ImageButton)e.Row.FindControl("ipbEEdit");
                            ImageButton ipbEDelete = (ImageButton)e.Row.FindControl("ipbEDelete");
                            ipbEEdit.Visible = false;
                            ipbEDelete.Visible = false;
                        }
                        foreach (string s in new string[] { "lblIStartTime", "lblIStartOffset", "lblIEndTime", "lblIEndOffset" })
                        {
                            ((Label)e.Row.FindControl(s)).Visible = bShowEqAgItemsStartEndTimes;
                        }
                        int iParam = (int)DataBinder.Eval(e.Row.DataItem, "iParam");
                        EquipmentDataContext eqdc = new EquipmentDataContext();
                        var q = (from p in eqdc.EQUIPAGINGPARs where p.ID == iParam select new { p.iIntervalOperating, p.iIntervalDistance, p.iIntervalCycles }).First();

                        if (q.iIntervalOperating == -1)
                        {
                            Label lblIEstRun = (Label)e.Row.FindControl("lblIEstRun");
                            lblIEstRun.ForeColor = System.Drawing.Color.LightGray;
                        }

                        if (q.iIntervalDistance == -1)
                        {
                            Label lblIEstDist = (Label)e.Row.FindControl("lblIEstDist");
                            lblIEstDist.ForeColor = System.Drawing.Color.LightGray;
                        }

                        if (q.iIntervalCycles == -1)
                        {
                            Label lblIEstCycl = (Label)e.Row.FindControl("lblIEstCycl");
                            lblIEstCycl.ForeColor = System.Drawing.Color.LightGray;
                        }
                    }
                    break;
            }
        }

        protected void gvAgItems_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            e.Cancel = true;
            string sMsg = "You should probably NOT DELETE this equipment aging item record because it is referenced from other data tables. " +
                "Those other records would also be wiped out (and quite possibly more data if there are more related tables): ";
            int iCount = 0;
            EquipmentDataContext eqdc = new EquipmentDataContext();
            try
            {
                var dtEq = eqdc.spForeignKeyRefs("EQUIPAGINGITEMS", Int32.Parse(((Label)((GridView)sender).Rows[e.RowIndex].FindControl("lblIIdent")).Text));
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

            Label lblItem = (Label)gvAgItems.Rows[e.RowIndex].FindControl("lblIIdent");
            string sItem = lblItem.Text;
            ButtonsClear();
            YesButton.CommandName = "Delete";
            YesButton.CommandArgument = sItem;
            OkButton.CommandArgument = "AgItems";
            if (iCount < 1)
            {
                lblPopupText.Text = "Please confirm deletion of equipment aging item record with internal Id " + YesButton.CommandArgument;
            }
            else
            {
                lblPopupText.Text = sMsg;
            }
            MPE_Show(Global.enumButtons.NoYes);
        }

        protected void gvAgItems_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            iEdRow = -1;
            gvAgItems.EditIndex = -1;
            FillDataTableEqAgItems();
        }

        protected void gvAgItems_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            EquipmentDataContext dc = new EquipmentDataContext();
            int iLast = gvAgItems.Rows.Count - 1;
            GridViewRow gvRow = gvAgItems.Rows[e.RowIndex];

            DateTimeOffset DStart;
            DateTimeOffset DEnd;
            try
            {
                DStart = Time_Date.DTO_from3TextBoxes(gvRow, "Start", "txbDStartDate", "txbDStartTime", "txbDStartOffset");
                DEnd = Time_Date.DTO_from3TextBoxes(gvRow, "End", "txbDEndDate", "txbDEndTime", "txbDEndOffset");
                if (DEnd < DStart)
                {
                    throw new Global.excToPopup("The End Date " + DEnd.ToString() + " must be later than or equal to the Start Date " + DStart.ToString() + ".");
                }
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

            decimal dEstRunDays = decimal.Parse(((TextBox)gvRow.FindControl("txbDEstRun")).Text);
            bool bRunExtrap = ((CheckBox)gvRow.FindControl("chbEbRun")).Checked;
            decimal dEstCycleDays = decimal.Parse(((TextBox)gvRow.FindControl("txbDEstCycl")).Text);
            bool bCyclExtrap = ((CheckBox)gvRow.FindControl("chbEbCycl")).Checked;
            decimal dEstDistDays = decimal.Parse(((TextBox)gvRow.FindControl("txbDEstDist")).Text);
            bool bDistExtrap = ((CheckBox)gvRow.FindControl("chbEbDist")).Checked;
            decimal dEstDuration = decimal.Parse(((TextBox)gvRow.FindControl("txbDEstDur")).Text);

            string sComment = Server.HtmlEncode(((TextBox)gvRow.FindControl("txbDComments")).Text.Replace("'", "`").Trim());

            string sName = Server.HtmlEncode(((TextBox)gvRow.FindControl("txbDName")).Text.Replace("'", "`").Trim());
            if (sName.Length < 1)
            {
                ProcessPopupException(new Global.excToPopup("Aging item name cannot be empty."));
                return;
            }
            var q0 = (from p in dc.EQUIPAGINGITEMs where p.sName == sName select p).ToList();
            if (q0.Count() > 1)
            {
                ProcessPopupException(new Global.excToPopup("Software/database error: Aging Item name `" + sName + "` exists more than once; contact support personnel."));
                return;
            }

            DropDownList DDLEOpsCal = (DropDownList)gvRow.FindControl("DDLEOpsCal");
            int iUser = mCRUD.GetPeopleIDfromWebSiteUserName(HttpContext.Current.User.Identity.Name);
            if (!bEditExistingEqAgItemsRow)
            {
                if (q0.Count() > 0)
                {
                    ProcessPopupException(new Global.excToPopup("An Aging Item called `" + sName + "` exists already."));
                    return;
                }
                EQUIPAGINGITEM f = new EQUIPAGINGITEM
                {
                    PiTRecordEntered = DateTime.UtcNow,
                    iRecordEnteredBy = iUser,
                    sName = sName,
                    iEquipComponent = Int32.Parse(((DropDownList)gvRow.FindControl("DDLCompName")).SelectedValue),
                    iParam = Int32.Parse(((DropDownList)gvRow.FindControl("DDLParSets")).SelectedValue),
                    iOpCal = Int32.Parse(DDLEOpsCal.SelectedValue),
                    DStart = DStart,
                    DEnd = DEnd,
                    dEstRunDays = dEstRunDays,
                    bRunExtrap = bRunExtrap,
                    dEstCycleDays = dEstCycleDays,
                    bCyclExtrap = bCyclExtrap,
                    dEstDistDays = dEstDistDays,
                    bDistExtrap = bDistExtrap,
                    dEstDuration = dEstDuration,
                    sComment = sComment
                };

                dc.EQUIPAGINGITEMs.InsertOnSubmit(f);
            }
            else
            {
                int iID = Int32.Parse(((Label)gvRow.FindControl("lblIIdent")).Text);
                if (q0.Count() > 0)
                {
                    if (q0[0].ID != iID)
                    {
                        ProcessPopupException(new Global.excToPopup("An Aging Item called `" + sName + "` exists already."));
                        return;
                    }
                }
                var f = (from v in dc.EQUIPAGINGITEMs where v.ID == iID select v).First();
                f.PiTRecordEntered = DateTime.UtcNow;
                f.iRecordEnteredBy = iUser;
                f.sName = sName;
                f.iEquipComponent = Int32.Parse(((DropDownList)gvRow.FindControl("DDLCompName")).SelectedValue);
                f.iParam = Int32.Parse(((DropDownList)gvRow.FindControl("DDLParSets")).SelectedValue);
                f.iOpCal = Int32.Parse(DDLEOpsCal.SelectedValue);
                f.DStart = DStart;
                f.DEnd = DEnd;
                f.dEstRunDays = dEstRunDays;
                f.bRunExtrap = bRunExtrap;
                f.dEstCycleDays = dEstCycleDays;
                f.bCyclExtrap = bCyclExtrap;
                f.dEstDistDays = dEstDistDays;
                f.bDistExtrap = bDistExtrap;
                f.dEstDuration = dEstDuration;
                f.sComment = sComment;
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
            iEdRow = -1;
            gvAgItems.EditIndex = -1;
            FillDataTableEqAgItems();
        }

        protected void gvAgItems_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvAgItems.PageIndex = e.NewPageIndex;
            FillDataTableEqAgItems();
        }
        #endregion

        //public static string sExpandedComponentName(int iuEqComp)
        //{
        //    // Assemble a string starting with the component's name and then adding its parent / grandparent / etc.
        //    string sRet = "";
        //    string sConn = "";
        //    EquipmentDataContext dc = new EquipmentDataContext();
        //    int iContinue = 100;
        //    int iParent = iuEqComp;
        //    string sEquip = "";
        //    do
        //    {
        //        if (iContinue > 1)
        //        {
        //            var qc = (from c in dc.EQUIPCOMPONENTs where c.ID == iParent select new { c.sComponent, c.iParentComponent, c.iEquipment, c.EQUIPMENT.sShortEquipName }).First();
        //            sRet += sConn + (char)39 + qc.sComponent + (char)39;
        //            sConn = " of ";
        //            iParent = qc.iParentComponent;
        //            if (iParent == 0)
        //            {
        //                iContinue = 2;
        //                sEquip = qc.sShortEquipName;
        //            }
        //        }
        //        else
        //        {
        //            sRet += sConn + (char)39 + sEquip + (char)39;
        //        }

        //    } while (--iContinue > 0);
        //    return sRet;
        //}
    }
}