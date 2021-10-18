using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TSoar.Accounting;
using TSoar.DB;

namespace TSoar.Equipment
{
    public partial class EqComponents : System.Web.UI.Page
    {
        SCUD_Multi mCRUD = new SCUD_Multi();

        #region Boolean Variables
        private bool bEditExistingRow
        {
            get { return GetbEditExistingRow("bEditExistingRow"); }
            set { ViewState["bEditExistingRow"] = value; }
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
        #region Integer Variables
        private int iNgvRows { get { return iGetInt("iNgvRows"); } set { ViewState["iNgvRows"] = value; } } // number of equipment components + 1
        // iEdRow indexes the row of interest (the one being edited) starting at 0 and ignoring paging
        private int iEdRow { get { return iGetInt("iEdRow"); } set { ViewState["iEdRow"] = value; } }
        private int iIndexOfLastPage { get { return iGetInt("iIndexOfLastPage"); } set { ViewState["iIndexOfLastPage"] = value; } }
        private int iGetInt(string suN)
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
                FillDataTable();
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
            //Global.oLog("Modal popup dismissed with " + btn.ID + ", CommandName=" + btn.CommandName);
            if (btn.ID == "YesButton")
            {
                switch (btn.CommandName)
                {
                    case "Delete":
                        // Delete the Equipment Component record
                        try
                        {
                            mCRUD.DeleteOne(Global.enugInfoType.EquipComponents, btn.CommandArgument);
                        }
                        catch (Global.excToPopup exc)
                        {
                            ProcessPopupException(exc);
                        }
                        gvEqComp.EditIndex = -1; // SCR 231
                        iEdRow = -1; // SCR 231
                        FillDataTable();
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
        
        private void FillDataTable()
        {
            List<DataRow> li = AssistLi.Init(Global.enLL.EquipComponents);
            Session["liEqComp"] = li;
            iNgvRows = li.Count; // number of rows in gvEqComp
            if (iEdRow == -1)
            {
                iEdRow = iNgvRows - 1;
            }
            gvEqComp.PageSize = Int32.Parse(mCRUD.GetSetting("PageSizeEquipComponents"));
            int iNewEditIndex = iEdRow % gvEqComp.PageSize;
            int iIndexOfEditPage = iEdRow / gvEqComp.PageSize;
            iIndexOfLastPage = iNgvRows / gvEqComp.PageSize;
            if ((iNgvRows % gvEqComp.PageSize) == 0)
            {
                iIndexOfLastPage--;
            }
            // Is the row being edited in the current page?
            if (gvEqComp.PageIndex == iIndexOfEditPage)
            {
                GridViewEditEventArgs gvee = new GridViewEditEventArgs(iNewEditIndex);
                gvEqComp_RowEditing(null, gvee);
            }
            else
            {
                gvEqComp.EditIndex = -1;
                bEditExistingRow = false;
                dt_BindTogvEqComp(li.CopyToDataTable());
            }
        }

        #region GridView Event Handlers
        protected void gvEqComp_RowEditing(object sender, GridViewEditEventArgs e)
        {
            GridView gv = gvEqComp;
            gv.EditIndex = e.NewEditIndex;
            iEdRow = gv.PageSize * gv.PageIndex + gv.EditIndex;
            List<DataRow> li = (List<DataRow>)Session["liEqComp"];
            bEditExistingRow = false;
            if (iEdRow < iNgvRows - 1)
            {
                // We are editing an existing row.
                // need to get rid of the last row which would be the New row (but there is no New row)
                li.RemoveAt(li.Count - 1);
                bEditExistingRow = true;
            }
            dt_BindTogvEqComp(li.CopyToDataTable());
        }

        protected void gvEqComp_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            bool bShowEqComponentsLinkBeginEndTimes = AccountProfile.CurrentUser.bShowEqComponentsLinkBeginEndTimes;
            switch (e.Row.RowType)
            {
                case DataControlRowType.Header:
                    ((Label)e.Row.FindControl("lblHBeginTime")).Visible = bShowEqComponentsLinkBeginEndTimes;
                    ((Label)e.Row.FindControl("lblHBeginOffset")).Visible = bShowEqComponentsLinkBeginEndTimes;
                    ((Label)e.Row.FindControl("lblHEndTime")).Visible = bShowEqComponentsLinkBeginEndTimes;
                    ((Label)e.Row.FindControl("lblHEndOffset")).Visible = bShowEqComponentsLinkBeginEndTimes;
                    break;
                case DataControlRowType.DataRow:
                    EquipmentDataContext eqdc = new EquipmentDataContext();
                    if ((e.Row.RowState & DataControlRowState.Edit) != 0)
                    {
                        DropDownList DDLEquipName = (DropDownList)e.Row.FindControl("DDLEquipName");
                        var q0 = from r in eqdc.EQUIPMENTs orderby r.sShortEquipName select r;
                        DDLEquipName.DataSource = q0;
                        DDLEquipName.DataValueField = "ID";
                        DDLEquipName.DataTextField = "sShortEquipName";
                        DDLEquipName.DataBind();

                        DropDownList DDLParent = (DropDownList)e.Row.FindControl("DDLParent");
                        var qc = from c in eqdc.TNPV_EqCompNames orderby c.sComponent select c;
                        DDLParent.DataSource = qc;
                        DDLParent.DataValueField = "ID";
                        DDLParent.DataTextField = "sComponent";
                        DDLParent.DataBind();

                        ((TextBox)e.Row.FindControl("txbEBeginTime")).Width = bShowEqComponentsLinkBeginEndTimes ? 80 : 1;
                        ((TextBox)e.Row.FindControl("txbEBeginOffset")).Width = bShowEqComponentsLinkBeginEndTimes ? 80 : 1;
                        ((TextBox)e.Row.FindControl("txbEEndTime")).Width = bShowEqComponentsLinkBeginEndTimes ? 80 : 1;
                        ((TextBox)e.Row.FindControl("txbEEndOffset")).Width = bShowEqComponentsLinkBeginEndTimes ? 80 : 1;

                        int iColAddButton = gvEqComp.Columns.Count - 2; // Column of the Edit and Add buttons
                        Label lblIIdent = (Label)e.Row.FindControl("lblIIdent");
                        if (!bEditExistingRow && iIndexOfLastPage == gvEqComp.PageIndex)
                        {
                            // Last row has no button except the Edit button:
                            e.Row.Cells[iColAddButton + 1].Visible = false;
                            // Last row has an Add button
                            ImageButton pb = (ImageButton)e.Row.FindControl("ipbEUpdate");
                            pb.ImageUrl = "~/i/YellowAddButton.jpg";
                            // Thick colored box around last row
                            e.Row.BackColor = Color.LightGray;
                            e.Row.BorderStyle = BorderStyle.Ridge;
                            e.Row.BorderWidth = 5;
                            e.Row.BorderColor = Color.Orange;
                            e.Row.Cells[iColAddButton].BorderStyle = BorderStyle.Ridge;
                            e.Row.Cells[iColAddButton].BorderWidth = 5;
                            e.Row.Cells[iColAddButton].BorderColor = Color.Orange;
                            // No Cancel button in the last row
                            ImageButton pbC = (ImageButton)e.Row.FindControl("ipbECancel");
                            pbC.Visible = false;
                            lblIIdent.Text = "New";
                            CheckBox chbIEntire = (CheckBox)e.Row.FindControl("chbIEntire");
                            chbIEntire.Visible = false;
                        }
                        else
                        {
                            // Editing an existing row

                            // Visibility of DDLParent
                            if ((bool)DataBinder.Eval(e.Row.DataItem, "bEntire"))
                            {
                                DDLParent.Visible = false;
                            }
                            else
                            {
                                DDLParent.Visible = true;
                            }
                            string siEqComp = DataBinder.Eval(e.Row.DataItem, "iParentComponent").ToString();
                            Set_DropDown_ByValue(DDLParent, siEqComp);

                            // Visibility of DDLEquipName: not visible if there are children
                            string siEquip = DataBinder.Eval(e.Row.DataItem, "iEquipment").ToString();
                            int iIdent = int.Parse(lblIIdent.Text);
                            int iNumChildren = (from c in eqdc.EQUIPCOMPONENTs where c.iParentComponent == iIdent select c).Count();
                            if (iNumChildren > 0)
                            {
                                DDLEquipName.Visible = false;
                            }
                            else
                            {
                                DDLEquipName.Visible = true;
                            }
                            Set_DropDown_ByValue(DDLEquipName, siEquip);
                        }
                    }
                    else
                    {
                        if (bEditExistingRow)
                        {
                            ImageButton ipbEEdit = (ImageButton)e.Row.FindControl("ipbEEdit");
                            ImageButton ipbEDelete = (ImageButton)e.Row.FindControl("ipbEDelete");
                            ipbEEdit.Visible = false;
                            ipbEDelete.Visible = false;
                        }
                        else
                        {
                            Label lblParent = (Label)e.Row.FindControl("lblParent");
                            int iParent = (int)DataBinder.Eval(e.Row.DataItem, "iParentComponent");
                            if (iParent == 0)
                            {
                                lblParent.Visible = false;
                            }
                            else
                            {
                                lblParent.Visible = true;
                                lblParent.Text = (from p in eqdc.EQUIPCOMPONENTs
                                                  where p.ID == iParent
                                                  select new { sParent = p.ID.ToString() + " - " + p.sComponent }).First().sParent;
                            }
                            if ((DateTimeOffset)DataBinder.Eval(e.Row.DataItem, "DLinkBegin") < Global.DTO_EqAgEarliest)
                            {
                                ((Label)e.Row.FindControl("lblILinkBegin")).ForeColor = Color.YellowGreen;
                                ((Label)e.Row.FindControl("lblIBeginTime")).ForeColor = Color.YellowGreen;
                                ((Label)e.Row.FindControl("lblIBeginOffset")).ForeColor = Color.YellowGreen;
                            }
                            if ((DateTimeOffset)DataBinder.Eval(e.Row.DataItem, "DLinkEnd") > Global.DTO_EqAgLatest)
                            {
                                ((Label)e.Row.FindControl("lblILinkEnd")).ForeColor = Color.YellowGreen;
                                ((Label)e.Row.FindControl("lblIEndTime")).ForeColor = Color.YellowGreen;
                                ((Label)e.Row.FindControl("lblIEndOffset")).ForeColor = Color.YellowGreen;
                            }
                        }
                        ((Label)e.Row.FindControl("lblIBeginTime")).Visible = bShowEqComponentsLinkBeginEndTimes;
                        ((Label)e.Row.FindControl("lblIBeginOffset")).Visible = bShowEqComponentsLinkBeginEndTimes;
                        ((Label)e.Row.FindControl("lblIEndTime")).Visible = bShowEqComponentsLinkBeginEndTimes;
                        ((Label)e.Row.FindControl("lblIEndOffset")).Visible = bShowEqComponentsLinkBeginEndTimes;
                    }
                    break;
            }
        }

        protected void gvEqComp_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            e.Cancel = true;
            string sMsg = "You should probably NOT DELETE this equipment component record because it is referenced from other data tables. " +
                "Those other records would also be wiped out (and quite possibly more data if there are more related tables): ";
            int iCount = 0;
            string sID = ((Label)((GridView)sender).Rows[e.RowIndex].FindControl("lblIIdent")).Text;
            int iID = Int32.Parse(sID);
            EquipmentDataContext eqdc = new EquipmentDataContext();

            iCount = (from c in eqdc.EQUIPCOMPONENTs where c.iParentComponent == iID select c).Count();
            if (iCount > 0)
            {
                ProcessPopupException(new Global.excToPopup("Equipment component with ID=" + sID + " is a parent to one or more children. Cannot delete."));
                return;
            }

            try
            {
                var dtEq = eqdc.spForeignKeyRefs("EQUIPCOMPONENTS", iID);
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

            Label lblItem = (Label)gvEqComp.Rows[e.RowIndex].FindControl("lblIIdent");
            string sItem = lblItem.Text;
            ButtonsClear();
            YesButton.CommandName = "Delete";
            YesButton.CommandArgument = sItem;
            OkButton.CommandArgument = "EqComp";
            if (iCount < 1)
            {
                lblPopupText.Text = "Please confirm deletion of equipment component record with internal Id " + YesButton.CommandArgument;
            }
            else
            {
                lblPopupText.Text = sMsg;
            }
            MPE_Show(Global.enumButtons.NoYes);
        }

        protected void gvEqComp_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvEqComp.EditIndex = -1;
            iEdRow = -1;
            FillDataTable();
        }

        protected void gvEqComp_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            string sThisRoutine = "EqComponents.aspx.cs.gvEqComp_RowUpdating: ";
            EquipmentDataContext dc = new EquipmentDataContext();
            GridViewRow gvRow = gvEqComp.Rows[e.RowIndex];
            DropDownList DDLEquipName = (DropDownList)gvRow.FindControl("DDLEquipName");
            string sComponent = Server.HtmlEncode(((TextBox)gvRow.FindControl("txbDComponent")).Text.Replace("'", "`"));
            if (sComponent.Length < 1)
            {
                ProcessPopupException(new Global.excToPopup(sThisRoutine + "Equipment Component name must not be empty."));
                return;
            }
            bool bEntire = ((CheckBox)gvRow.FindControl("chbIEntire")).Checked;
            string sComment = Server.HtmlEncode(((TextBox)gvRow.FindControl("txbDComments")).Text.Replace("'", "`"));
            int iUser = mCRUD.GetPeopleIDfromWebSiteUserName(HttpContext.Current.User.Identity.Name);
            DropDownList DDLParent = (DropDownList)gvRow.FindControl("DDLParent");

            DateTimeOffset DELinkBegin;
            DateTimeOffset DELinkEnd;
            try
            {
                DELinkBegin = Time_Date.DTO_from3TextBoxes(gvRow, "LinkBegin", "txbELinkBegin", "txbEBeginTime", "txbEBeginOffset");
                DELinkEnd = Time_Date.DTO_from3TextBoxes(gvRow, "LinkEnd", "txbELinkEnd", "txbEEndTime", "txbEEndOffset");
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

            EQUIPCOMPONENT f;
            int iID;
            if (!bEditExistingRow && iIndexOfLastPage == gvEqComp.PageIndex)
            {
                iID = 0;
                f = new EQUIPCOMPONENT
                {
                    PiTRecordEntered = DateTime.UtcNow,
                    iRecordEnteredBy = iUser,
                    iEquipment = Int32.Parse(DDLEquipName.SelectedValue),
                    sComponent = sComponent,
                    bReportOperStatus = ((CheckBox)gvRow.FindControl("chbEReportOpSt")).Checked, // SCR 231
                    sOperStatus = Server.HtmlEncode(((TextBox)gvRow.FindControl("txbEOpSt")).Text.Replace("'", "`")), // SCR 231
                    DLinkBegin = DELinkBegin,
                    DLinkEnd = DELinkEnd,
                    sComment = sComment
                };
                int iParent = 0;
                int.TryParse(DDLParent.SelectedItem.Value, out iParent);
                f.iParentComponent = iParent;
                f.bEntire = iParent == 0;
                if (!bLinkDatesCheck(f.bEntire, DELinkBegin, DELinkEnd)) return;
                dc.EQUIPCOMPONENTs.InsertOnSubmit(f);
            }
            else
            {
                iID = Int32.Parse(((Label)gvRow.FindControl("lblIIdent")).Text);
                f = (from v in dc.EQUIPCOMPONENTs where v.ID == iID select v).First();
                f.PiTRecordEntered = DateTime.UtcNow;
                f.iRecordEnteredBy = iUser;
                f.iEquipment = Int32.Parse(DDLEquipName.SelectedValue);
                f.sComponent = sComponent;
                f.bReportOperStatus = ((CheckBox)gvRow.FindControl("chbEReportOpSt")).Checked; // SCR 231
                f.sOperStatus = Server.HtmlEncode(((TextBox)gvRow.FindControl("txbEOpSt")).Text.Replace("'", "`")); // SCR 231
                f.bEntire = bEntire;
                f.DLinkBegin = DELinkBegin;
                f.DLinkEnd = DELinkEnd;
                if (!bLinkDatesCheck(bEntire, DELinkBegin, DELinkEnd)) return;
                f.sComment = sComment;
                f.iParentComponent = (bEntire) ? 0 : int.Parse(DDLParent.SelectedItem.Value);
                try
                {
                    if ( ! EqSupport.bParentage(iID, f.iParentComponent))
                    {
                        ProcessPopupException(new Global.excToPopup(sThisRoutine + "An Equipment Component cannot be its own parent or grandparent etc."));
                        return;
                    }
                }
                catch (Global.excToPopup exc)
                {
                    ProcessPopupException(exc);
                    return;
                }
            }

            // ***** Validation
            // ----------------

            if (bEditExistingRow)
            {
                // If the component being edited has children, its LinkBegin and LinkEnd must not violate LinkBegin/End of the children:
                var qh = from h in dc.EQUIPCOMPONENTs where h.iParentComponent == iID select h;
                foreach (var h in qh)
                {
                    if (h.DLinkBegin >= new DateTimeOffset(1900, 1, 1, 0, 0, 0, new TimeSpan(0)) && h.DLinkBegin < f.DLinkBegin)
                    {
                        ProcessPopupException(new Global.excToPopup(sThisRoutine + "The new Link Begin date " + f.DLinkBegin.ToString() +
                            " is later than one the children's Link Begin date " + h.DLinkBegin.ToString()));
                        return;
                    }
                    if (h.DLinkEnd <= new DateTimeOffset(2999, 12, 31, 23, 59, 0, new TimeSpan(0)) && h.DLinkEnd > f.DLinkEnd)
                    {
                        ProcessPopupException(new Global.excToPopup(sThisRoutine + "The new Link End date " + f.DLinkEnd.ToString() +
                            " is earlier than one the children's Link End date " + h.DLinkEnd.ToString()));
                        return;
                    }
                }
            }

            if (f.bEntire)
            {
                // Any two 'Entire' components must not refer to the same PoEq
                var qs = (from s in dc.EQUIPCOMPONENTs
                         where s.bEntire && s.iEquipment == f.iEquipment && s.ID != iID
                         select s).ToList();
                if (qs.Count > 0)
                {
                    ProcessPopupException(new Global.excToPopup(sThisRoutine + "The piece of equipment '" + qs[0].EQUIPMENT.sShortEquipName +
                        "' is already associated with 'Entire' component '" + qs[0].sComponent + "'." ));
                    return;
                }
            }
            else // 'Entire' components have no parents; so, this does not apply to them:
            {
                // Children's references to PoEq must be the same as PoEq reference of parent:
                var qq = (from q in dc.EQUIPCOMPONENTs where q.ID == f.iParentComponent select new { q.iEquipment, q.EQUIPMENT.sShortEquipName }).First();
                if (f.iEquipment != qq.iEquipment)
                {
                    string sWrongParent = (from qw in dc.EQUIPMENTs where qw.ID == f.iEquipment select qw.sShortEquipName).First();
                    ProcessPopupException(new Global.excToPopup(sThisRoutine + "A subcomponent must refer to the same piece of equipment '" + 
                        qq.sShortEquipName + "' as its parent component, i.e. not to '" + sWrongParent + "'."));
                    return;
                }

                // Children's Link Begin and End dates must be consistent with those of their parents:
                var qParent = (from d in dc.EQUIPCOMPONENTs where d.ID == f.iParentComponent select d).First();
                if (f.DLinkBegin >= new DateTimeOffset(1900, 1, 1, 0, 0, 0, new TimeSpan(0)))
                {
                    if (f.DLinkBegin < qParent.DLinkBegin)
                    {
                        ProcessPopupException(new Global.excToPopup(sThisRoutine +
                            "A component's Link Begin date must either be earlier than 1900/1/1 00:00 +00:00 or equal to or later than its parent's Link Begin date = " 
                            + qParent.DLinkBegin.ToString()));
                        return;
                    }
                }
                if (f.DLinkEnd <= new DateTimeOffset(2999, 12, 31, 23, 59, 0, new TimeSpan(0)))
                {
                    if (f.DLinkEnd > qParent.DLinkEnd)
                    {
                        ProcessPopupException(new Global.excToPopup(sThisRoutine +
                            "A component's Link End date must either be later than 2999/12/31 23:59 +00:00 or equal to or earlier than its parent's Link End date = "
                            + qParent.DLinkEnd.ToString()));
                        return;
                    }
                }
            }

            // ***** Commit database changes
            // -----------------------------
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
            gvEqComp.EditIndex = -1;
            iEdRow = iNgvRows - 1;
            if (iID == 0) iEdRow = iNgvRows;
            FillDataTable();
        }

        private bool bLinkDatesCheck(bool buEntire, DateTimeOffset DuLinkBegin, DateTimeOffset DuLinkEnd)
        {
            if (DuLinkBegin >= DuLinkEnd)
            {
                ProcessPopupException(new Global.excToPopup("Link Begin " + DuLinkBegin.ToString() + " is not earlier than Link End " + DuLinkEnd.ToString()));
                return false;
            }
            if (buEntire)
            {
                if (DuLinkBegin < new DateTimeOffset(1900, 1, 1, 0, 0, 0, new TimeSpan(0)))
                {
                    ProcessPopupException(new Global.excToPopup("An 'entire' component must have a Link Begin date/time after 1900/01/01 00:00 +00:00"));
                    return false;
                }
                if (DuLinkEnd > new DateTimeOffset(2999, 12, 31, 23, 59, 0, new TimeSpan(0)))
                {
                    ProcessPopupException(new Global.excToPopup("An 'entire' component must have a Link End date/time before 2999/12/31 23:59:00 +00:00"));
                    return false;
                }
            }
            return true;
        }

        protected void gvEqComp_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvEqComp.PageIndex = e.NewPageIndex;
            FillDataTable();
        }
        #endregion

        private void dt_BindTogvEqComp(DataTable dtu)
        {
            GridView GV = gvEqComp;
            GV.Visible = true;
            DataView view = new DataView(dtu)
            {
                Sort = "i1Sort ASC, sShortEquipName ASC"
            };
            GV.DataSource = view;
            GV.DataBind();
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
    }
}