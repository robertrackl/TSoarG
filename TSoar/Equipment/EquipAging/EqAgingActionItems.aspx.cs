using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TSoar.Accounting;
using TSoar.DB;
using System.Web.UI.DataVisualization.Charting;

namespace TSoar.Equipment.EquipAging
{
    public partial class EqAgingActionItems : System.Web.UI.Page
    {
        SCUD_Multi mCRUD = new SCUD_Multi();

        #region ViewState Variables
        private bool bEditExistingEqActItems
        {
            get { return GetbEditExisting("bEditExistingEqActItems"); }
            set { ViewState["bEditExistingEqActItems"] = value; }
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
        private int iNgvEqActItems { get { return iGetN("iNgvEqActItems"); } set { ViewState["iNgvEqActItems"] = value; } }
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

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                EquipmentDataContext dc = new EquipmentDataContext();
                DateTimeOffset DLastUpd = DateTimeOffset.MinValue;
                if (DateTimeOffset.TryParse((from l in dc.TNP_SETTINGs where l.sSettingName == "DateOfLastUpdateEquipmentActionItems" select l.sSettingValue).First(), out DLastUpd))
                {
                    lblLastUpd.Text = CustFmt.sFmtDate(DLastUpd, CustFmt.enDFmt.DateAndTimeMin);
                }
                else
                {
                    lblLastUpd.Text = "Never";
                }
                FillDataTableEqActItems();
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
            bool bTry = true;
            if (btn.ID == "YesButton")
            {
                char cKind = 'T';
                switch (btn.CommandName)
                {
                    case "pbRemoveIll":
                        cKind = 'I';
                        break;
                    case "pbRemoveH":
                        cKind = 'H';
                        break;
                    case "pbRemoveC":
                        cKind = 'C';
                        break;
                    case "pbRemoveD":
                        cKind = 'D';
                        break;
                    case "DelActionItem":
                        cKind = 'A'; // delete ALL
                        break;
                    default:
                        bTry = false;
                        break;
                }
                if (bTry)
                {
                    // Delete an action item illustration or chart
                    try
                    {
                        int iActionItem = int.Parse(btn.CommandArgument);
                        EquipmentDataContext eqdc = new EquipmentDataContext();
                        var ee = from o in eqdc.EQACITAUXes
                                 where o.iEqActionItem == iActionItem && (o.cKind == cKind || o.cKind == 'A')
                                 select o;
                        foreach (var o in ee)
                        {
                            if (o.cKind == 'I')
                            {
                                if (o.sIllustrFile.Length > 0)
                                {
                                    File.Delete(System.Web.Hosting.HostingEnvironment.MapPath(o.sIllustrFile));
                                }
                            }
                            eqdc.EQACITAUXes.DeleteOnSubmit(o);
                        }
                        if (cKind == 'A')
                        {
                            var qa = from c in eqdc.EQUIPACTIONITEMs where c.ID == iActionItem select c;
                            foreach (var c in qa)
                            {
                                eqdc.EQUIPACTIONITEMs.DeleteOnSubmit(c);
                            }
                        }
                        eqdc.SubmitChanges();
                        eqdc.Dispose();
                    }
                    catch (Global.excToPopup exc)
                    {
                        ProcessPopupException(exc);
                    }
                    catch (Exception gExc)
                    {
                        ProcessPopupException(new Global.excToPopup(gExc.Message));
                    }
                }
                ButtonsClear();
                FillDataTableEqActItems();
            }
        }

        private void ProcessPopupException(Global.excToPopup excu)
        {
            ButtonsClear();
            lblPopupText.Text = excu.sExcMsg();
            MPE_Show(Global.enumButtons.OkOnly);
        }
        #endregion

        protected void pbRemove_Click(object sender, EventArgs e)
        {
            Button B = (Button)sender;
            lblPopupText.Text="Really remove one of the charts that go with action item with internal ID " + ((Button)sender).CommandArgument + " ?";
            ButtonsClear();
            YesButton.CommandName = B.ID;
            YesButton.CommandArgument = B.CommandArgument;
            MPE_Show(Global.enumButtons.NoYes);
        }

        #region Action Items
        private void FillDataTableEqActItems()
        {
            EquipmentDataContext eqdc = new EquipmentDataContext();
            var qs = from s in eqdc.TNPF_GetSetting("DateOfLastUpdateEquipmentActionItems") select s;
            foreach (var s in qs)
            {
                lblLastUpd.Text = s.sSettingValue;
            }
            List<DataRow> liEqActItems = AssistLi.Init(Global.enLL.EquipActionItems);
            Session["liEqActItems"] = liEqActItems;
            iNgvEqActItems = liEqActItems.Count;
            GridViewEditEventArgs gvee = new GridViewEditEventArgs(iNgvEqActItems - 1);
            gvActItems_RowEditing(null, gvee);
        }

        protected void pbUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                EqSupport.ActionItemUpdate(0); // Update all action/aging items
            }
            catch (Global.excToPopup exctp)
            {
                ProcessPopupException(exctp);
                return;
            }
            try
            { 
                EquipmentDataContext eqdc = new EquipmentDataContext();
                var qs = from s in eqdc.TNP_SETTINGs where s.sSettingName == "DateOfLastUpdateEquipmentActionItems" select s;
                var t = qs.First();
                t.sSettingValue = CustFmt.sFmtDate(DateTimeOffset.Now, CustFmt.enDFmt.DateAndTimeMin);
                eqdc.SubmitChanges();
                FillDataTableEqActItems();
            }
            catch (Global.excToPopup exctp)
            {
                ProcessPopupException(exctp);
            }
            catch (Exception exc)
            {
                ProcessPopupException(new Global.excToPopup(exc.Message));
            }
        }

        protected void gvActItems_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            bool bShowEqActItemsStartEndTimes = AccountProfile.CurrentUser.bShowEqActItemsStartEndTimes;
            switch (e.Row.RowType)
            {
                case DataControlRowType.Header:
                    foreach (string s in new string[] { "lblHSchedStartTime", "lblHSchedStartOffset", "lblHActStartTime", "lblHActStartOffset",
                        "lblHComplTime", "lblHComplOffset", "lblHDeadLineTime", "lblHDeadLineOffset" })
                    {
                        ((Label)e.Row.FindControl(s)).Visible = bShowEqActItemsStartEndTimes;
                    }
                    break;
                case DataControlRowType.DataRow:
                    if ((e.Row.RowState & DataControlRowState.Edit) != 0)
                    {
                        foreach (string s in new string[] { "txbDActStartTime", "txbDActStartOffset", "txbDComplTime", "txbDComplOffset" })
                        {
                            ((TextBox)e.Row.FindControl(s)).Width = bShowEqActItemsStartEndTimes ? 80 : 1;
                        }
                        Button pbIOptions = (Button)e.Row.FindControl("pbIOptions");
                        pbIOptions.Visible = false;
                    }
                    else
                    {
                        if (bEditExistingEqActItems)
                        {
                            Button pbIOptions = (Button)e.Row.FindControl("pbIOptions");
                            pbIOptions.Visible = false;
                        }
                        System.Drawing.Color foreColor;
                        switch ((int)DataBinder.Eval(e.Row.DataItem, "iPercentComplete"))
                        {
                            case 0:
                                foreColor = System.Drawing.Color.FromArgb(200, 38, 0);
                                if ((DateTimeOffset)DataBinder.Eval(e.Row.DataItem, "DeadLine") < DateTimeOffset.Now.Subtract(new TimeSpan(7, 0, 0, 0)))
                                {
                                    foreColor = System.Drawing.Color.OrangeRed;
                                }
                                break;
                            case 100:
                                foreColor = System.Drawing.Color.Blue;
                                break;
                            default:
                                foreColor = System.Drawing.Color.Black;
                                break;
                        }
                        Label lblIIdent = (Label)e.Row.FindControl("lblIIdent");
                        lblIIdent.ForeColor = foreColor;
                        ((Label)e.Row.FindControl("lblIAgingItem")).ForeColor = foreColor;
                        System.Drawing.Color tmpColor = foreColor;
                        if ((DateTimeOffset)DataBinder.Eval(e.Row.DataItem, "DeadLine") == DateTimeOffset.MinValue)
                        {
                            tmpColor = System.Drawing.Color.LightGray;
                        }
                        ((Label)e.Row.FindControl("lblIDeadLine")).ForeColor = tmpColor;
                        ((Label)e.Row.FindControl("lblIDeadLineTime")).ForeColor = tmpColor;
                        ((Label)e.Row.FindControl("lblIDeadLineOffset")).ForeColor = tmpColor;

                        ((Label)e.Row.FindControl("lblIDeadLineHrs")).ForeColor = foreColor;
                        if ((decimal)DataBinder.Eval(e.Row.DataItem, "dDeadlineHrs") == -9.99m)
                        {
                            ((Label)e.Row.FindControl("lblIDeadLineHrs")).ForeColor = System.Drawing.Color.LightGray;
                        }

                        ((Label)e.Row.FindControl("lblIDeadLineCycl")).ForeColor = foreColor;
                        if ((int)DataBinder.Eval(e.Row.DataItem, "iDeadLineCycles") == -999)
                        {
                            ((Label)e.Row.FindControl("lblIDeadLineCycl")).ForeColor = System.Drawing.Color.LightGray;
                        }

                        ((Label)e.Row.FindControl("lblIDeadLineDist")).ForeColor = foreColor;
                        if ((decimal)DataBinder.Eval(e.Row.DataItem, "dDeadLineDist") == -9.99m)
                        {
                            ((Label)e.Row.FindControl("lblIDeadLineDist")).ForeColor = System.Drawing.Color.LightGray;
                        }

                        ((Label)e.Row.FindControl("lblISchedStart")).ForeColor = tmpColor;
                        ((Label)e.Row.FindControl("lblISchedStartTime")).ForeColor = tmpColor;
                        ((Label)e.Row.FindControl("lblISchedStartOffset")).ForeColor = tmpColor;

                        tmpColor = foreColor;
                        if ((DateTimeOffset)DataBinder.Eval(e.Row.DataItem, "DActualStart") == Global.DTO_NotStarted)
                        {
                            tmpColor = System.Drawing.Color.LightGray;
                        }
                        ((Label)e.Row.FindControl("lblIActStart")).ForeColor = tmpColor;
                        ((Label)e.Row.FindControl("lblIActStartTime")).ForeColor = tmpColor;
                        ((Label)e.Row.FindControl("lblIActStartOffset")).ForeColor = tmpColor;

                        tmpColor = foreColor;
                        if ((DateTimeOffset)DataBinder.Eval(e.Row.DataItem, "DComplete") == Global.DTO_NotCompleted)
                        {
                            tmpColor = System.Drawing.Color.LightGray;
                        }
                        ((Label)e.Row.FindControl("lblIComplDate")).ForeColor = tmpColor;
                        ((Label)e.Row.FindControl("lblIComplTime")).ForeColor = tmpColor;
                        ((Label)e.Row.FindControl("lblIComplOffset")).ForeColor = tmpColor;

                        ((Label)e.Row.FindControl("lblIPerc")).ForeColor = foreColor;
                        ((Label)e.Row.FindControl("lblIComments")).ForeColor = foreColor;
                        ((Label)e.Row.FindControl("lblIUpdStat")).ForeColor = foreColor;

                        Label lblIAtComplHrs = (Label)e.Row.FindControl("lblIAtComplHrs");
                        if (lblIAtComplHrs.Text == "-9.9900")
                        {
                            lblIAtComplHrs.ForeColor = System.Drawing.Color.LightGray;
                        }
                        else
                        {
                            lblIAtComplHrs.ForeColor = tmpColor;
                        }
                        Label lblIAtComplCycl = (Label)e.Row.FindControl("lblIAtComplCycl");
                        if (lblIAtComplCycl.Text == "-999")
                        {
                            lblIAtComplCycl.ForeColor = System.Drawing.Color.LightGray;
                        }
                        else
                        {
                            lblIAtComplCycl.ForeColor = tmpColor;
                        }
                        Label lblIAtComplDist = (Label)e.Row.FindControl("lblIAtComplDist");
                        if (lblIAtComplDist.Text == "-9.9900")
                        {
                            lblIAtComplDist.ForeColor = System.Drawing.Color.LightGray;
                        }
                        else
                        {
                            lblIAtComplDist.ForeColor = tmpColor;
                        }

                        foreach (string s in new string[] { "lblISchedStartTime", "lblISchedStartOffset", "lblIActStartTime", "lblIActStartOffset",
                            "lblIComplTime", "lblIComplOffset", "lblIDeadLineTime", "lblIDeadLineOffset" })
                        {
                            ((Label)e.Row.FindControl(s)).Visible = bShowEqActItemsStartEndTimes;
                        }

                        //EquipmentDataContext eqdc = new EquipmentDataContext();
                        //int iEqActionItem = int.Parse(lblIIdent.Text);
                        //int ih = (from h in eqdc.EQACITAUXes where h.iEqActionItem == iEqActionItem && (h.serChart.Length > 0 || h.sIllustrFile.Length > 0) select h).Count();
                        //Button pbIChart = (Button)e.Row.FindControl("pbIChart");
                        //pbIChart.Visible = (ih > 0) && !bEditExistingEqActItems;
                    }
                    break;
            }
        }

        protected void gvActItems_RowEditing(object sender, GridViewEditEventArgs e)
        {
            gvActItems.EditIndex = e.NewEditIndex;
            List<DataRow> liEqActItems = (List<DataRow>)Session["liEqActItems"];
            bEditExistingEqActItems = false;
            if (e.NewEditIndex < iNgvEqActItems - 1)
            {
                // We are editing an existing row; need to get rid of the last row which would be the New row (but there is no New row)
                liEqActItems.RemoveAt(liEqActItems.Count - 1);
                bEditExistingEqActItems = true;
            }
            DataTable dt = liEqActItems.CopyToDataTable();
            dt_BindTogvEqActItems(dt);
        }

        // Action Items should not be deleted. They result from Aging Items which can be edited, created, and deleted.
        //protected void gvActItems_RowDeleting(object sender, GridViewDeleteEventArgs e)
        //{
        //    Label lblItem = (Label)gvActItems.Rows[e.RowIndex].FindControl("lblIIdent");
        //    string sItem = lblItem.Text;
        //    ButtonsClear();
        //    YesButton.CommandName = "Delete";
        //    YesButton.CommandArgument = sItem;
        //    OkButton.CommandArgument = "EqActItems";
        //    lblPopupText.Text =
        //        "Please confirm deletion of equipment Action Item record with ID " + sItem;
        //    MPE_Show(Global.enumButtons.NoYes);
        //}

        protected void gvActItems_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvActItems.EditIndex = -1;
            FillDataTableEqActItems();
        }

        protected void gvActItems_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            EquipmentDataContext dc = new EquipmentDataContext();
            GridViewRow gvRow = gvActItems.Rows[e.RowIndex];

            try
            {
                DateTimeOffset DoActualStart = Time_Date.DTO_from3TextBoxes(gvRow, "Actual Start", "txbDActStart", "txbDActStartTime", "txbDActStartOffset");

                TextBox txbDPerc = (TextBox)gvRow.FindControl("txbDPerc");
                int iPercCompl = Int32.Parse(txbDPerc.Text);
                DateTimeOffset DCompletion = Time_Date.DTO_from3TextBoxes(gvRow, "Completion", "txbDComplDate", "txbDComplTime", "txbDComplOffset");
                DateTimeOffset DoTomorrow = DateTimeOffset.Now.AddDays(1);
                if (DoActualStart > DoTomorrow)
                {
                    throw new Global.excToPopup("The Actual Start Date must not be in the future.");
                }
                if (DCompletion != Global.DTO_NotCompleted && DoActualStart == Global.DTO_NotStarted)
                {
                    throw new Global.excToPopup("The Completion Date " + CustFmt.sFmtDate(DCompletion, CustFmt.enDFmt.DateAndTimeMinOffset) +
                        " implies that work has started or even been completed, but the Actual Start Date "
                        + CustFmt.sFmtDate(DoActualStart, CustFmt.enDFmt.DateAndTimeMinOffset) + " implies that work has not been started.");
                }
                if (iPercCompl > 0 && DCompletion == Global.DTO_NotCompleted)
                {
                    throw new Global.excToPopup("Please enter an actual or estimated completion date");
                }
                if (DCompletion < DoActualStart)
                {
                    throw new Global.excToPopup("The Completion Date " + CustFmt.sFmtDate(DCompletion, CustFmt.enDFmt.DateAndTimeMinOffset) +
                        " must be later than or equal to the Actual Start Date " + CustFmt.sFmtDate(DoActualStart, CustFmt.enDFmt.DateAndTimeMinOffset) + ".");
                }
                if (DCompletion < DateTimeOffset.Now && iPercCompl < 100)
                {
                    throw new Global.excToPopup("The Completion Date " + CustFmt.sFmtDate(DCompletion, CustFmt.enDFmt.DateAndTimeMinOffset) +
                        " is in the past. In that case, Percent Complete has to be 100.");
                }
                if (iPercCompl > 99 && DCompletion > DoTomorrow)
                {
                    throw new Global.excToPopup("The Completion Date " + CustFmt.sFmtDate(DCompletion, CustFmt.enDFmt.DateAndTimeMinOffset) +
                        " is in the future but Percent Complete is already 100.");
                }

                string ss = ((TextBox)gvRow.FindControl("txbEAtComplHrs")).Text;
                decimal dAtCompletionHrs = (ss.Length < 1 || (ss.Length > 4 && ss.Substring(0, 5) == "-9.99")) ? -9.99m : decimal.Parse(ss);
                ss = ((TextBox)gvRow.FindControl("txbEAtComplCycl")).Text;
                int iAtCompletionCycles = (ss.Length < 1 || (ss.Length > 3 && ss == "-999")) ? -999 : int.Parse(ss);
                ss = ((TextBox)gvRow.FindControl("txbEAtComplDist")).Text;
                decimal dAtCompletionDist = (ss.Length < 1 || (ss.Length > 4 && ss.Substring(0, 5) == "-9.99")) ? -9.99m : decimal.Parse(ss);

                string sComment = Server.HtmlEncode(((TextBox)gvRow.FindControl("txbDComments")).Text.Replace("'", "`").Trim());

                int iUser = mCRUD.GetPeopleIDfromWebSiteUserName(HttpContext.Current.User.Identity.Name);
                if (!bEditExistingEqActItems)
                {
                    throw new Global.excToPopup("Software Error: Attempt to create a new Action Item manually in EquipAging.aspx.cs. Contact Website developer.");
                }
                else
                {
                    int id = Int32.Parse(((Label)gvRow.FindControl("lblIIdent")).Text);
                    var qai = (from ai in dc.EQUIPACTIONITEMs where ai.ID == id select ai).First();
                    qai.PiTRecordEntered = DateTime.UtcNow;
                    qai.iRecordEnteredBy = iUser;
                    qai.DActualStart = DoActualStart;
                    qai.iPercentComplete = (byte)iPercCompl;
                    qai.DComplete = DCompletion;
                    qai.dAtCompletionHrs = dAtCompletionHrs;
                    qai.iAtCompletionCycles = iAtCompletionCycles;
                    qai.dAtCompletionDist = dAtCompletionDist;
                    qai.sComment = sComment;
                    try
                    {
                        dc.SubmitChanges();
                    }
                    catch (Exception exc)
                    {
                        throw new Global.excToPopup(exc.Message);
                    }
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
            dc.Dispose();
            gvActItems.EditIndex = -1;
            FillDataTableEqActItems();
        }

        private void dt_BindTogvEqActItems(DataTable dtu)
        {
            GridView GV = gvActItems;
            GV.Visible = true;
            if (!bEditExistingEqActItems)
            {
                dtu.Rows[dtu.Rows.Count - 1].Delete(); // Delete last row (user cannot add a new row of data for action items)
            }
            DataView view = new DataView(dtu)
            {
                Sort = "sName ASC, DeadLine ASC"
            };
            GV.DataSource = view;
            GV.PageSize = Int32.Parse(mCRUD.GetSetting("PageSizeEquipActionItems"));
            GV.DataBind();
        }
        #endregion

        #region Modal Popup Options Menu
        private void ButtonsClearOM()
        {
            pbCharts.CommandArgument = "";
            pbCharts.CommandName = "";
            pbCharts.Visible = true;
            pbDetails.CommandArgument = "";
            pbDetails.CommandName = "";
            pbDetails.Visible = true;
            pbEdit.CommandArgument = "";
            pbEdit.CommandName = "";
            pbEdit.Visible = true;
            pbRefresh.CommandArgument = "";
            pbRefresh.CommandName = "";
            pbRefresh.Visible = true;
            pbDelete.CommandArgument = "";
            pbDelete.CommandName = "";
            pbDelete.Visible = true;
            pbCancel.CommandArgument = "";
            pbCancel.CommandName = "";
            pbCancel.Visible = true;
        }
        private void ButtonsSetArg(string suCmdArg, string suCmdName)
        {
            pbCharts.CommandArgument = suCmdArg;
            pbDetails.CommandArgument = suCmdArg;
            pbEdit.CommandArgument = suCmdArg;
            pbRefresh.CommandArgument = suCmdArg;
            pbDelete.CommandArgument = suCmdArg;
            pbCharts.CommandName = suCmdName;
            pbDetails.CommandName = suCmdName;
            pbEdit.CommandName = suCmdName;
            pbRefresh.CommandName = suCmdName;
            pbDelete.CommandName = suCmdName;
        }
        private void MPE_ShowOM()
        {
            ModPopExtOptionsMenu.Show();
        }
        protected void pbOM_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            try
            {
                switch (btn.ID)
                {
                    case "pbCharts":
                        vShowCharts(btn.CommandArgument);
                        break;
                    case "pbDetails":
                        vDetails(btn.CommandArgument);
                        break;
                    case "pbEdit":
                        GridViewEditEventArgs gvee = new GridViewEditEventArgs(int.Parse(btn.CommandName));
                        gvActItems_RowEditing(null, gvee);
                        break;
                    case "pbRefresh":
                        vUpdate(btn.CommandArgument);
                        break;
                    case "pbDelete":
                        vDelete(btn.CommandArgument);
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

        protected void pbIOptions_Click(object sender, EventArgs e)
        {
            ButtonsClearOM();
            Button pbIOptions = (Button)sender;
            GridViewRow row = (GridViewRow)pbIOptions.NamingContainer;
            string sActionItemID = ((Label)row.FindControl("lblIIdent")).Text;
            ButtonsSetArg(sActionItemID, row.RowIndex.ToString());
            lblActID.Text = sActionItemID;
            MPE_ShowOM();
        }

        private void vShowCharts(string suCmdArg)
        {
            EquipmentDataContext eqdc = new EquipmentDataContext();
            int iEqActionItem = int.Parse(suCmdArg);
            tbpIllustr.Visible = false;
            tbpChartsH.Visible = false;
            tbpChartsC.Visible = false;
            tbpChartsD.Visible = false;
            lblNothing.Visible = true;
            pbNothing.Visible = true;
            ButtonsClear();

            foreach (EqSupport.enSchedMeth s in Enum.GetValues(typeof(EqSupport.enSchedMeth)))
            {
                switch (s)
                {
                    case EqSupport.enSchedMeth.Illustration:
                        var qh = from h in eqdc.EQACITAUXes
                                 where h.iEqActionItem == iEqActionItem && h.cKind == 'I'
                                 select h;
                        foreach (var h in qh)
                        {
                            if (h.sIllustrFile.Length > 0)
                            {
                                imgIllustr.ImageUrl = h.sIllustrFile;
                                tbpIllustr.Visible = true;
                                lblNothing.Visible = false;
                            }
                        }
                        pbRemoveIll.CommandArgument = suCmdArg;
                        break;
                    case EqSupport.enSchedMeth.ElapsedTime:
                        break;
                    case EqSupport.enSchedMeth.OperatingHours:
                        qh = from h in eqdc.EQACITAUXes
                             where h.iEqActionItem == iEqActionItem && h.cKind == 'H'
                             select h;
                        foreach (var h in qh)
                        {
                            lblPopTxtChartsH.Text = h.sTitle;
                            StringReader reader = new StringReader(h.serChart);
                            chartExtrapH.Serializer.Content = SerializationContents.Default;
                            chartExtrapH.Serializer.Load(reader);
                            reader.Close();
                            chartExtrapH.Width = 900;
                            chartExtrapH.Height = 660;
                            pbRemoveH.CommandArgument = suCmdArg;
                            tbpChartsH.Visible = true;
                            lblNothing.Visible = false;
                        }
                        break;
                    case EqSupport.enSchedMeth.Cycles:
                        qh = from h in eqdc.EQACITAUXes
                             where h.iEqActionItem == iEqActionItem && h.cKind == 'C'
                             select h;
                        foreach (var h in qh)
                        {
                            lblPopTxtChartsC.Text = h.sTitle;
                            StringReader reader = new StringReader(h.serChart);
                            chartExtrapC.Serializer.Content = SerializationContents.Default;
                            chartExtrapC.Serializer.Load(reader);
                            reader.Close();
                            chartExtrapC.Width = 900;
                            chartExtrapC.Height = 660;
                            pbRemoveC.CommandArgument = suCmdArg;
                            tbpChartsC.Visible = true;
                            lblNothing.Visible = false;
                        }
                        break;
                    case EqSupport.enSchedMeth.DistanceTraveled:
                        qh = from h in eqdc.EQACITAUXes
                             where h.iEqActionItem == iEqActionItem && h.cKind == 'D'
                             select h;
                        foreach (var h in qh)
                        {
                            lblPopTxtChartsD.Text = h.sTitle;
                            StringReader reader = new StringReader(h.serChart);
                            chartExtrapD.Serializer.Content = SerializationContents.Default;
                            chartExtrapD.Serializer.Load(reader);
                            reader.Close();
                            chartExtrapD.Width = 900;
                            chartExtrapD.Height = 660;
                            pbRemoveD.CommandArgument = suCmdArg;
                            tbpChartsD.Visible = true;
                            lblNothing.Visible = false;
                        }
                        break;
                }
            }
            pbNothing.Visible = lblNothing.Visible;
            ModalPopExtCharts.Show();
        }

        private void vDetails(string suActionItemID)
        {
            int iActionItem = int.Parse(suActionItemID);
            EquipmentDataContext eqdc = new EquipmentDataContext();
            int iAgingItem = (from a in eqdc.EQUIPACTIONITEMs where a.ID == iActionItem select a.iEquipAgingItem).First();
            var qAgingItem = (from w in eqdc.EQUIPAGINGITEMs
                     where w.ID == iAgingItem select w).First();
            var qEquipCompon = (from c in eqdc.EQUIPCOMPONENTs
                                where c.ID == qAgingItem.iEquipComponent
                                select c).First();
            var qPoEq = (from e in eqdc.EQUIPMENTs
                         where e.ID == qEquipCompon.iEquipment
                         select new {e.sShortEquipName,
                             e.DOwnershipBegin,
                             e.DOwnershipEnd,
                             e.EQUIPTYPE.sEquipmentType
                         }).First();
            var qParSet = (from p in eqdc.EQUIPAGINGPARs
                           where p.ID == qAgingItem.iParam
                           select new { p.EQUIPACTIONTYPE.sEquipActionType, p }).First();
            var qOpsCal = (from o in eqdc.OPSCALNAMEs
                           where o.ID == qAgingItem.iOpCal
                           select new {o.sOpsCalName,
                               iOpsCal = o.ID
                           }).First();
            var qOpsCalTimes = (from m in eqdc.OPSCALTIMEs where m.iOpsCal == qOpsCal.iOpsCal orderby m.DStart select m).ToList(); // SCR 214
            var qOperDat = (from d in eqdc.EQUIPOPERDATAs
                           where d.iEquipComponent == qAgingItem.iEquipComponent
                           orderby d.DFrom
                           select d).ToList();
            var qFlyOpsDat = (from f in eqdc.OPDETAILs
                              where f.iEquip == qEquipCompon.iEquipment
                              orderby f.OPERATION.DBegin
                              select f).ToList();
            lblActItemID.Text = suActionItemID;
            lblEquipName.Text = qPoEq.sShortEquipName;
            lblEquipFrom.Text = CustFmt.sFmtDate(qPoEq.DOwnershipBegin, CustFmt.enDFmt.DateAndTimeMinOffset);
            lblEquipTo.Text = CustFmt.sFmtDate(qPoEq.DOwnershipEnd, CustFmt.enDFmt.DateAndTimeMinOffset);
            lblEquipProps.Text = qPoEq.sEquipmentType;
            lblComponName.Text = EqSupport.sExpandedComponentName(qAgingItem.iEquipComponent);
            lblComponFrom.Text = CustFmt.sFmtDate(qEquipCompon.DLinkBegin, CustFmt.enDFmt.DateAndTimeMinOffset);
            lblComponTo.Text = CustFmt.sFmtDate(qEquipCompon.DLinkEnd, CustFmt.enDFmt.DateAndTimeMinOffset);
            lblComponProps.Text = "bEntire = " + qEquipCompon.bEntire.ToString();
            lblAgingItemName.Text = qAgingItem.sName;
            lblAgingItemFrom.Text = CustFmt.sFmtDate(qAgingItem.DStart, CustFmt.enDFmt.DateAndTimeMinOffset);
            lblAgingItemTo.Text = CustFmt.sFmtDate(qAgingItem.DEnd, CustFmt.enDFmt.DateAndTimeMinOffset);
            lblAgingItemProps.Text = qAgingItem.sComment;
            lblAgingItemProps2.Text = "dEstDuration = " + qAgingItem.dEstDuration.ToString() + " days";
            lblParSetName.Text = qParSet.p.sShortDescript;
            lblParSetProps.Text = qParSet.sEquipActionType;
            if (qParSet.p.iIntervalElapsed > -1)
            {
                lblParSetProps.Text += ", iIntervalElapsed = " + qParSet.p.iIntervalElapsed.ToString() + " " + qParSet.p.sTimeUnitsElapsed +
                    ", cDeadLineMode = " + qParSet.p.cDeadLineMode + ", iDeadLineSpt1 = " + qParSet.p.iDeadLineSpt1.ToString() +
                    ", iDeadLineSpt2 = " + qParSet.p.iDeadLineSpt2.ToString();
            }
            if (qParSet.p.iIntervalOperating > -1)
            {
                lblParSetProps.Text += ", iIntervalOperating = " + qParSet.p.iIntervalOperating.ToString() + " " + qParSet.p.sTimeUnitsOperating;
                lblAgingItemProps2.Text += ", dEstRunDays = " + qAgingItem.dEstRunDays.ToString() + ", bRunExtrap = " + qAgingItem.bRunExtrap.ToString();
            }
            if (qParSet.p.iIntervalCycles > -1)
            {
                lblParSetProps.Text += ", iIntervalCycles = " + qParSet.p.iIntervalCycles.ToString();
                lblAgingItemProps2.Text += ", dEstCycleDays = " + qAgingItem.dEstCycleDays.ToString() + ", bCyclExtrap = " + qAgingItem.bCyclExtrap.ToString();
            }
            if (qParSet.p.iIntervalDistance > -1)
            {
                lblParSetProps.Text += ", iIntervalDistance = " + qParSet.p.iIntervalDistance.ToString() + " " + qParSet.p.sDistanceUnits;
                lblAgingItemProps2.Text += ", dEstDistDays = " + qAgingItem.dEstDistDays.ToString() + ", bDistExtrap = " + qAgingItem.bDistExtrap.ToString();
            }
            if (qParSet.p.iIntervalElapsed == -1 && qParSet.p.iIntervalOperating == -1 && qParSet.p.iIntervalCycles == -1 && qParSet.p.iIntervalDistance == -1)
            {
                lblAgingItemProps2.Text += ", unique event";
            }
            if (!(qParSet.p.sComment is null))
            {
                if (qParSet.p.sComment.Length > 0) lblParSetProps.Text += ", " + qParSet.p.sComment;
            }
            lblOpsCalName.Text = qOpsCal.sOpsCalName;
            lblOpsCalFrom.Text = CustFmt.sFmtDate(qOpsCalTimes.First().DStart, CustFmt.enDFmt.DateAndTimeMinOffset);
            lblOpsCalTo.Text = CustFmt.sFmtDate(qOpsCalTimes.Last().DStart, CustFmt.enDFmt.DateAndTimeMinOffset);
            lblOpsCalProps.Text = "Number of segments = " + qOpsCalTimes.Count.ToString();
            if (qOperDat.Count > 0)
            {
                lblManOpsFrom.Text = CustFmt.sFmtDate(qOperDat.First().DFrom, CustFmt.enDFmt.DateAndTimeMinOffset);
                lblManOpsTo.Text = CustFmt.sFmtDate(qOperDat.Last().DTo, CustFmt.enDFmt.DateAndTimeMinOffset);
                lblManOpsProps.Text = qOperDat.Count.ToString() + " data points";
            }
            else { lblManOpsFrom.Text = ""; lblManOpsTo.Text = ""; lblManOpsProps.Text = ""; }
            if (qFlyOpsDat.Count > 0)
            {
                lblFlyOpsFrom.Text = CustFmt.sFmtDate((DateTime)qFlyOpsDat.First().OPERATION.DBegin, CustFmt.enDFmt.DateAndTimeMinOffset);
                lblFlyOpsTo.Text = CustFmt.sFmtDate((DateTime)qFlyOpsDat.Last().OPERATION.DBegin, CustFmt.enDFmt.DateAndTimeMinOffset);
                lblFlyOpsProps.Text = qFlyOpsDat.Count.ToString() + " flights";
            }
            else { lblFlyOpsProps.Text = ""; lblFlyOpsFrom.Text = ""; lblFlyOpsTo.Text = ""; }

            var qAct = (from c in eqdc.EQUIPACTIONITEMs where c.ID == iActionItem select c).First();
            lblPiTRecordEntered.Text = qAct.PiTRecordEntered.ToString();
            lblRecordEnteredBy.Text = (from p in eqdc.PEOPLEs where p.ID == qAct.iRecordEnteredBy select p.sDisplayName).First();
            lblDLastAction.Text = CustFmt.sFmtDate(qAct.DLastAction, CustFmt.enDFmt.DateAndTimeMinOffset);
            lblWhenceDLastAction.Text = EqSupport.sWhenceDLastAction;
            lblDTOdeadlineElapsed.Text = CustFmt.sFmtDate(qAct.DTOdeadlineElapsed, CustFmt.enDFmt.DateAndTimeMinOffset);
            lblDTOdeadlineHrs.Text = CustFmt.sFmtDate(qAct.DTOdeadlineHrs, CustFmt.enDFmt.DateAndTimeMinOffset);
            lblDTOdeadlineCycles.Text = CustFmt.sFmtDate(qAct.DTOdeadlineCycles, CustFmt.enDFmt.DateAndTimeMinOffset);
            lblDTOdeadlineDist.Text = CustFmt.sFmtDate(qAct.DTOdeadlineDist, CustFmt.enDFmt.DateAndTimeMinOffset);
            lblDTOdeadlineUnique.Text = CustFmt.sFmtDate(qAct.DTOdeadlineUnique, CustFmt.enDFmt.DateAndTimeMinOffset);
            lblDeadLine.Text = CustFmt.sFmtDate(qAct.DeadLine, CustFmt.enDFmt.DateAndTimeMinOffset);
            lblDeadlineHrs.Text = qAct.dDeadlineHrs.ToString() + " hours";
            lblDeadLineCycles.Text = qAct.iDeadLineCycles.ToString() + " cycles";
            lblDeadLineDist.Text = qAct.dDeadLineDist.ToString() + " " + qParSet.p.sDistanceUnits;
            lblScheduledStart.Text = CustFmt.sFmtDate(qAct.DScheduledStart, CustFmt.enDFmt.DateAndTimeMinOffset);
            lblActualStart.Text = CustFmt.sFmtDate(qAct.DActualStart, CustFmt.enDFmt.DateAndTimeMinOffset);
            lblPercentComplete.Text = qAct.iPercentComplete.ToString();
            lblAtCompletionHrs.Text = qAct.dAtCompletionHrs.ToString();
            lblAtCompletionCycles.Text = qAct.iAtCompletionCycles.ToString();
            lblAtCompletionDist.Text = qAct.dAtCompletionDist.ToString();
            lblDComplete.Text = CustFmt.sFmtDate(qAct.DComplete, CustFmt.enDFmt.DateAndTimeMinOffset);
            lblComment.Text = qAct.sComment;
            lblUpdateStatus.Text = qAct.sUpdateStatus;

            ModPopExtDetails.Show();
        }

        protected void vUpdate(string suCmdArg)
        {
            try
            {
                EqSupport.ActionItemUpdate(int.Parse(suCmdArg)); // Update a single aging items
            }
            catch (Global.excToPopup exctp)
            {
                ProcessPopupException(exctp);
                return;
            }
            FillDataTableEqActItems();
        }

        private void vDelete(string suCmdArg)
        {
            EquipmentDataContext eqdc = new EquipmentDataContext();
            int iActionItem = int.Parse(suCmdArg);
            EQUIPACTIONITEM acti = (from c in eqdc.EQUIPACTIONITEMs where c.ID == iActionItem select c).First();
            if (acti.iPercentComplete > 0)
            {
                lblPopupText.Text = "The action item with internal ID = " + suCmdArg + " has Percent Complete = " + acti.iPercentComplete.ToString()
                    + " which is greater than 0. Cannot delete.";
                ButtonsClear();
                MPE_Show(Global.enumButtons.OkOnly);
                return;
            }
            lblPopupText.Text = "Action Items are not normally deleted.<br />" +
                "If you want to delete all action items that belong to an aging item you can delete the aging item.<br />" +
                "You could then recreate the aging item.<br />" +
                "So, you should answer NO here unless you really know what you are doing.<br />" +
                "Be aware that any of this action item's illustrations or charts will also be deleted.<br />" +
                "Do you really want to delete this action item with internal ID " + suCmdArg + " ?";
            ButtonsClear();
            YesButton.CommandName = "DelActionItem";
            YesButton.CommandArgument = suCmdArg;
            MPE_Show(Global.enumButtons.NoYes);
            return;
        }

        protected void gvActItems_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvActItems.PageIndex = e.NewPageIndex;
            FillDataTableEqActItems();
        }
    }
}