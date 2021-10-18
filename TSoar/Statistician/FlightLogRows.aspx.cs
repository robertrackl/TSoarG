using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TSoar.Accounting;
using TSoar.DB;
using System.Data;

namespace TSoar.Statistician
{
    public partial class FlightLogRows : System.Web.UI.Page
    {
        SCUD_Multi mCRUD = new SCUD_Multi();
        protected Dictionary<char, string> dictStatus = new Dictionary<char, string>()
        {
            {'N', "New"}, {'P', "Processed"}, {'R', "Recalled"}
        };

        #region ViewState Booleans
        private bool bEditExistingFlLogRow
        {
            get { return GetVSBoolean("bEditExistingFlLogRow", false); }
            set { ViewState["bEditExistingFlLogRow"] = value; }
        }
        private bool bWhichDefaultSet
        {
            get { return GetVSBoolean("bWhichDefaultSet", true); }
            set { ViewState["bWhichDefaultSet"] = value; }
        }
        private bool GetVSBoolean(string sub, bool buInitDefault)
        {
            if (ViewState[sub] is null)
            {
                return buInitDefault;
            }
            else
            {
                return (bool)ViewState[sub];
            }
        }
        #endregion
        #region ViewState Integers
        private int iNgvFliNlRows { get { return iGetInteger("iNgvFliNlRows"); } set { ViewState["iNgvFliNlRows"] = value; } }
        private int iFlightLog { get { return iGetInteger("iFlightLog"); } set { ViewState["iFlightLog"] = value; } }
        private int iGetInteger(string suNgvRows)
        {
            if (ViewState[suNgvRows] is null)
            {
                return 0;
            }
            else
            {
                return (int)ViewState[suNgvRows];
            }
        }
        #endregion

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
                        case "FlightLogRow":
                            // Delete one flight log row
                            StatistDailyFlightLogDataContext dc = new StatistDailyFlightLogDataContext();
                            var r = (from w in dc.FLIGHTLOGROWs where w.ID == Int32.Parse(btn.CommandArgument) select w).First();
                            dc.FLIGHTLOGROWs.DeleteOnSubmit(r);
                            try
                            {
                                dc.SubmitChanges();
                            }
                            catch (Exception exc)
                            {
                                Global.excToPopup ex = new Global.excToPopup(exc.Message);
                                ProcessPopupException(ex);
                            }
                            finally
                            {
                                FillFlightLogRowsDataTable();
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

        protected void page_load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["iFlightLog"] is null)
                {
                    ProcessPopupException(new Global.excToPopup("Programming logic error detected in Statistician.FlightLogRows.aspx.cs: Session[" +
                        (char)34 + "iFlightLog" + (char)34 + "] is null."));
                    Server.Transfer("FlightLogInput.aspx");
                    return;
                }
                iFlightLog = Math.Abs((int)Session["iFlightLog"]);
                FillLogListDataTable();
                FillFlightLogRowsDataTable();
            }
            else
            {
                if (!bEditExistingFlLogRow)
                {
                    int iColAddButton = gvFliN.Columns.Count - 2; // Column of the Edit and Add buttons
                    GridViewRow LastRow = gvFliN.Rows[gvFliN.Rows.Count - 1];
                    LastRow.Cells[iColAddButton].Controls.Remove(LastRow.Cells[iColAddButton].Controls[2]);
                    LastRow.Cells[iColAddButton].Controls.Remove(LastRow.Cells[iColAddButton].Controls[1]);
                }
            }
        }

        #region Daily Flight Log Being Edited

        private void FillLogListDataTable()
        {
            StatistDailyFlightLogDataContext stdc = new StatistDailyFlightLogDataContext();
            var qdf = from q in stdc.DAILYFLIGHTLOGs
                      where q.ID == iFlightLog
                      select new
                      {
                          q.ID,
                          q.DFlightOps,
                          q.sFldMgr,
                          q.iMainTowEquip,
                          q.EQUIPMENT.sShortEquipName,
                          q.iMainTowOp,
                          q.PEOPLE.sDisplayName,
                          q.iMainGlider,
                          sMainGliderName = (from g in stdc.EQUIPMENTs where g.ID == q.iMainGlider select g.sShortEquipName).First(),
                          q.iMainLaunchMethod,
                          sMainLaunchMethod = (from l in stdc.LAUNCHMETHODs where l.ID == q.iMainLaunchMethod select l.sLaunchMethod).First(),
                          q.iMainLocation,
                          q.LOCATION.sLocation,
                          mTcoll = ((from t in stdc.FLIGHTLOGROWs where t.iFliteLog == q.ID select t.mAmtCollected).Sum() == null)
                                    ? 0.00m : (from t in stdc.FLIGHTLOGROWs where t.iFliteLog == q.ID select t.mAmtCollected).Sum(),
                          q.sNotes
                      };
            gvDayFL.DataSource = qdf;
            gvDayFL.DataBind();
        }

        protected void rblChooseDefaultSet_SelectedIndexChanged(object sender, EventArgs e)
        {
            RadioButtonList rbl = (RadioButtonList)sender;
            bWhichDefaultSet = rbl.SelectedValue == "0"; // TRUE means take default values from "Main Items in Daily Flight Log Heading"; FALSE means take defaults from "Last Used"
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
        #endregion

        #region Flight Log Rows (Details)
        private void FillFlightLogRowsDataTable()
        {
            List<DataRow> liFlLRows = AssistLi.Init(Global.enLL.FlightLogRows, iFlightLog, bWhichDefaultSet);
            Session["liFlLRows"] = liFlLRows;
            DataTable dtFlLRows = liFlLRows.CopyToDataTable();
            iNgvFliNlRows = dtFlLRows.Rows.Count; // number of rows in gvRewards
            GridViewEditEventArgs gvee = new GridViewEditEventArgs(iNgvFliNlRows - 1);
            gvFliN_RowEditing(null, gvee);
        }

        protected void gvFliN_RowEditing(object sender, GridViewEditEventArgs e)
        {
            List<DataRow> liFlLRows = (List<DataRow>)Session["liFlLRows"];
            gvFliN.EditIndex = e.NewEditIndex;
            bEditExistingFlLogRow = false;
            if (e.NewEditIndex < iNgvFliNlRows - 1)
            {
                // We are editing an existing row; need to get rid of the last row which would be the New row (but there is no New row)
                liFlLRows.RemoveAt(liFlLRows.Count - 1);
                bEditExistingFlLogRow = true;
            }
            DataTable dtFlLRows = liFlLRows.CopyToDataTable();
            gvFliN.DataSource = dtFlLRows;
            gvFliN.DataBind();
        }

        protected void gvFliN_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                int iColAddButton = gvFliN.Columns.Count - 2; // Column of the Edit and Add buttons
                //e.Row.Cells[6].HorizontalAlign = HorizontalAlign.Right;
                if ((e.Row.RowState & DataControlRowState.Edit) != 0)
                {
                    StatistDailyFlightLogDataContext stdc = new StatistDailyFlightLogDataContext();
                    Equipment.EquipmentDataContext eqdc = new Equipment.EquipmentDataContext();
                    ClubMembership.ClubMembershipDataContext cmdc = new ClubMembership.ClubMembershipDataContext();

                    DropDownList DDLDcStatus = (DropDownList)e.Row.FindControl("DDLDcStatus");
                    Set_DropDown_ByValue(DDLDcStatus, DataBinder.Eval(e.Row.DataItem, "cStatus").ToString());

                    DropDownList DDLDiTowEquip = (DropDownList)e.Row.FindControl("DDLDiTowEquip");
                    var q0 = from d in eqdc.sp_EquipTypes4Role("Tow") select d;
                    DDLDiTowEquip.DataSource = q0;
                    DDLDiTowEquip.DataValueField = "ID";
                    DDLDiTowEquip.DataTextField = "sShortEquipName";
                    DDLDiTowEquip.DataBind();
                    Set_DropDown_ByValue(DDLDiTowEquip, DataBinder.Eval(e.Row.DataItem, "iTowEquip").ToString());

                    DateTime DOFliteLog = (from D in stdc.DAILYFLIGHTLOGs where D.ID == iFlightLog select D.DFlightOps).First().Date;
                    DropDownList DDLDiTowOperator = (DropDownList)e.Row.FindControl("DDLDiTowOperator");
                    var q1 = from P in stdc.sp_PeopleWhoCanTow(DOFliteLog) select new { P.ID, sDisplayName = Server.HtmlDecode(P.sDisplayName) };
                    DDLDiTowOperator.DataSource = q1;
                    DDLDiTowOperator.DataValueField = "ID";
                    DDLDiTowOperator.DataTextField = "sDisplayName";
                    DDLDiTowOperator.DataBind();
                    Set_DropDown_ByValue(DDLDiTowOperator, DataBinder.Eval(e.Row.DataItem, "iTowOperator").ToString());

                    DropDownList DDLDiGlider = (DropDownList)e.Row.FindControl("DDLDiGlider");
                    var q2 = from d in eqdc.sp_EquipTypes4Role("Glider") orderby d.sShortEquipName select d;
                    DDLDiGlider.DataSource = q2;
                    DDLDiGlider.DataValueField = "ID";
                    DDLDiGlider.DataTextField = "sShortEquipName";
                    DDLDiGlider.DataBind();
                    Set_DropDown_ByValue(DDLDiGlider, DataBinder.Eval(e.Row.DataItem, "iGlider").ToString());

                    DropDownList DDLDiLaunchMethod = (DropDownList)e.Row.FindControl("DDLDiLaunchMethod");
                    var q7 = from d in stdc.LAUNCHMETHODs orderby d.sLaunchMethod select d;
                    DDLDiLaunchMethod.DataSource = q7;
                    DDLDiLaunchMethod.DataValueField = "ID";
                    DDLDiLaunchMethod.DataTextField = "sLaunchMethod";
                    DDLDiLaunchMethod.DataBind();
                    Set_DropDown_ByValue(DDLDiLaunchMethod, DataBinder.Eval(e.Row.DataItem, "iLaunchMethod").ToString());

                    DropDownList DDLDPilot1 = (DropDownList)e.Row.FindControl("DDLDPilot1");
                    var q6 = from P in cmdc.MEMBERFROMTOs
                             where P.DMembershipBegin.AddDays(-1) <= DOFliteLog && P.DMembershipEnd.AddDays(1) >= DOFliteLog
                             orderby P.PEOPLE.sDisplayName
                             select new { P.PEOPLE.ID, sDisplayName = Server.HtmlDecode(P.PEOPLE.sDisplayName) };
                    DDLDPilot1.DataSource = q6;
                    DDLDPilot1.DataValueField = "ID";
                    DDLDPilot1.DataTextField = "sDisplayName";
                    DDLDPilot1.DataBind();
                    Set_DropDown_ByValue(DDLDPilot1, DataBinder.Eval(e.Row.DataItem, "iPilot1").ToString());

                    DropDownList DDLDAviatorRole1 = (DropDownList)e.Row.FindControl("DDLDAviatorRole1");
                    var q3 = from R in stdc.AVIATORROLEs orderby R.sAviatorRole select R;
                    DDLDAviatorRole1.DataSource = q3;
                    DDLDAviatorRole1.DataValueField = "ID";
                    DDLDAviatorRole1.DataTextField = "sAviatorRole";
                    DDLDAviatorRole1.DataBind();
                    Set_DropDown_ByValue(DDLDAviatorRole1, DataBinder.Eval(e.Row.DataItem, "iAviatorRole1").ToString());

                    DropDownList DDLDPilot2 = (DropDownList)e.Row.FindControl("DDLDPilot2");
                    q6 = from P in cmdc.MEMBERFROMTOs
                         where (P.DMembershipBegin.AddDays(-1) <= DOFliteLog && P.DMembershipEnd.AddDays(1) >= DOFliteLog)
                         orderby P.PEOPLE.sDisplayName
                         select new { P.PEOPLE.ID, sDisplayName = Server.HtmlDecode(P.PEOPLE.sDisplayName) };
                    DDLDPilot2.DataSource = q6;
                    DDLDPilot2.DataValueField = "ID";
                    DDLDPilot2.DataTextField = "sDisplayName";
                    DDLDPilot2.DataBind();
                    int iNone = 0;
                    try
                    {
                        iNone = (from n in cmdc.PEOPLEs where n.sDisplayName == "[none]" select n.ID).First();
                    }catch (Exception exc)
                    {
                        ProcessPopupException(new Global.excToPopup("FlightLogInput.aspx.cs problem: '" + exc.Message + 
                            "'; this most likely means that aviator '[none]' does not exist. Please make sure that a member with display name [none] exists (including the square brackets)."));
                        return;
                    }
                    //ListItem li = new ListItem("[none]", iNone.ToString()); (SCR 113)
                    //DDLDPilot2.Items.Add(li);
                    Set_DropDown_ByValue(DDLDPilot2, DataBinder.Eval(e.Row.DataItem, "iPilot2").ToString());

                    DropDownList DDLDAviatorRole2 = (DropDownList)e.Row.FindControl("DDLDAviatorRole2");
                    q3 = from R in stdc.AVIATORROLEs orderby R.sAviatorRole select R;
                    DDLDAviatorRole2.DataSource = q3;
                    DDLDAviatorRole2.DataValueField = "ID";
                    DDLDAviatorRole2.DataTextField = "sAviatorRole";
                    DDLDAviatorRole2.DataBind();
                    Set_DropDown_ByValue(DDLDAviatorRole2, DataBinder.Eval(e.Row.DataItem, "iAviatorRole2").ToString());

                    DropDownList DDLDiLocTakeOff = (DropDownList)e.Row.FindControl("DDLDiLocTakeOff");
                    var q4 = from L in stdc.LOCATIONs orderby L.sLocation select L;
                    DDLDiLocTakeOff.DataSource = q4;
                    DDLDiLocTakeOff.DataValueField = "ID";
                    DDLDiLocTakeOff.DataTextField = "sLocation";
                    DDLDiLocTakeOff.DataBind();
                    Set_DropDown_ByValue(DDLDiLocTakeOff, DataBinder.Eval(e.Row.DataItem, "iLocTakeOff").ToString());

                    DropDownList DDLDiLocLanding = (DropDownList)e.Row.FindControl("DDLDiLocLanding");
                    q4 = from L in stdc.LOCATIONs orderby L.sLocation select L;
                    DDLDiLocLanding.DataSource = q4;
                    DDLDiLocLanding.DataValueField = "ID";
                    DDLDiLocLanding.DataTextField = "sLocation";
                    DDLDiLocLanding.DataBind();
                    Set_DropDown_ByValue(DDLDiLocLanding, DataBinder.Eval(e.Row.DataItem, "iLocLanding").ToString());

                    DropDownList DDLDiChargeCode = (DropDownList)e.Row.FindControl("DDLDiChargeCode");
                    var q5 = from C in stdc.CHARGECODEs orderby C.sChargeCode select C;
                    DDLDiChargeCode.DataSource = q5;
                    DDLDiChargeCode.DataValueField = "ID";
                    DDLDiChargeCode.DataTextField = "sChargeCode";
                    DDLDiChargeCode.DataBind();
                    Set_DropDown_ByValue(DDLDiChargeCode, DataBinder.Eval(e.Row.DataItem, "iChargeCode").ToString());

                    int iLast = iNgvFliNlRows - 1; // The standard editing row
                    if (!bEditExistingFlLogRow)
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
                }
                else
                {
                    // Dealing with a row that is not being edited
                    if (bEditExistingFlLogRow)
                    {
                        // We don't want edit buttons visible for rows not being edited because
                        //   the row being edited has Update and Cancel buttons. They must be the only ones the user can act upon.
                        e.Row.Cells[iColAddButton].Controls.Remove(e.Row.Cells[iColAddButton].Controls[0]);
                        e.Row.Cells[iColAddButton + 1].Controls.Remove(e.Row.Cells[iColAddButton + 1].Controls[0]);
                    }
                }
            }
        }

        protected void gvFliN_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            Label lblIID = (Label)gvFliN.Rows[e.RowIndex].FindControl("lblIID");
            string sItem = lblIID.Text;
            ButtonsClear();
            YesButton.CommandName = "Delete";
            YesButton.CommandArgument = sItem;
            OkButton.CommandArgument = "FlightLogRow";
            lblPopupText.Text = "Please confirm deletion of flight log row/line with Internal ID = '" + sItem + "'";
            MPE_Show(Global.enumButtons.NoYes);
        }

        protected void gvFliN_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            FillFlightLogRowsDataTable();
        }

        protected void gvFliN_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            string sLog = "";
            ActivityLog.enumLogTypes elt = ActivityLog.enumLogTypes.DataInsert;
            StatistDailyFlightLogDataContext dc = new StatistDailyFlightLogDataContext();
            int iLast = iNgvFliNlRows - 1; // index to last row (regardless of whether a row was trimmed off in FillFlightLogRowsDataTable()).
            Label lblIcStatus = (Label)gvFliN.Rows[e.RowIndex].FindControl("lblIcStatus");

            DropDownList DDLDcStatus = (DropDownList)gvFliN.Rows[e.RowIndex].FindControl("DDLDcStatus");
            char cStatus = DDLDcStatus.SelectedItem.Value[0];

            DropDownList DDLDiTowEquip = (DropDownList)gvFliN.Rows[e.RowIndex].FindControl("DDLDiTowEquip");
            int iTowEquip = Int32.Parse(DDLDiTowEquip.SelectedItem.Value);
            DropDownList DDLDiTowOperator = (DropDownList)gvFliN.Rows[e.RowIndex].FindControl("DDLDiTowOperator");
            int iTowOperator = Int32.Parse(DDLDiTowOperator.SelectedItem.Value);
            DropDownList DDLDiGlider = (DropDownList)gvFliN.Rows[e.RowIndex].FindControl("DDLDiGlider");
            int iGlider = Int32.Parse(DDLDiGlider.SelectedItem.Value);
            DropDownList DDLDiLaunchMethod = (DropDownList)gvFliN.Rows[e.RowIndex].FindControl("DDLDiLaunchMethod");
            int iLaunchMethod = Int32.Parse(DDLDiLaunchMethod.SelectedItem.Value);
            
            DropDownList DDLDPilot1 = (DropDownList)gvFliN.Rows[e.RowIndex].FindControl("DDLDPilot1");
            int iPilot1 = Int32.Parse(DDLDPilot1.SelectedItem.Value);
            DropDownList DDLDAviatorRole1 = (DropDownList)gvFliN.Rows[e.RowIndex].FindControl("DDLDAviatorRole1");
            int iAviatorRole1 = Int32.Parse(DDLDAviatorRole1.SelectedItem.Value);
            TextBox txbDdPctCharge1 = (TextBox)gvFliN.Rows[e.RowIndex].FindControl("txbDdPctCharge1");
            decimal mPctCharge1 = decimal.Parse(txbDdPctCharge1.Text);

            DropDownList DDLDPilot2 = (DropDownList)gvFliN.Rows[e.RowIndex].FindControl("DDLDPilot2");
            int iPilot2 = Int32.Parse(DDLDPilot2.SelectedItem.Value);
            DropDownList DDLDAviatorRole2 = (DropDownList)gvFliN.Rows[e.RowIndex].FindControl("DDLDAviatorRole2");
            int iAviatorRole2 = Int32.Parse(DDLDAviatorRole2.SelectedItem.Value);
            TextBox txbDdPctCharge2 = (TextBox)gvFliN.Rows[e.RowIndex].FindControl("txbDdPctCharge2");
            decimal mPctCharge2 = decimal.Parse(txbDdPctCharge2.Text);

            TextBox txbDdReleaseAltitude = (TextBox)gvFliN.Rows[e.RowIndex].FindControl("txbDdReleaseAltitude");
            string sRelAlt = txbDdReleaseAltitude.Text;
            if (sRelAlt.Length < 1)
            {
                ProcessPopupException(new Global.excToPopup("ERROR: Release Altitude must not be empty"));
                return;
            }
            decimal mReleaseAltitude = decimal.Parse(sRelAlt);
            if (mReleaseAltitude < -1200.0M)
            {
                ProcessPopupException(new Global.excToPopup("ERROR: Release Altitude must not be less than -1200 feet"));
                return;
            }
            TextBox txbDdMaxAltitude = (TextBox)gvFliN.Rows[e.RowIndex].FindControl("txbDdMaxAltitude");
            string sMaxAlt = txbDdMaxAltitude.Text; // (SCR 107)
            decimal mMaxAltitude = -2000.0m; // indicates 'unknown'
            if (sMaxAlt.Length > 0)
            {
                mMaxAltitude = decimal.Parse(sMaxAlt);
            }

            DropDownList DDLDiLocTakeOff = (DropDownList)gvFliN.Rows[e.RowIndex].FindControl("DDLDiLocTakeOff");
            int iLocTakeOff = Int32.Parse(DDLDiLocTakeOff.SelectedItem.Value);

            TextBox txbDTakeOffDate = (TextBox)gvFliN.Rows[e.RowIndex].FindControl("txbDTakeOffDate");
            TextBox txbDTakeOffTime = (TextBox)gvFliN.Rows[e.RowIndex].FindControl("txbDTakeOffTime");
            TextBox txbTakeOffOffset = (TextBox)gvFliN.Rows[e.RowIndex].FindControl("txbTakeOffOffset");
            DateTimeOffset PiTop = DateTimeOffset.MinValue;
            string sPiTop = txbDTakeOffDate.Text + " " + txbDTakeOffTime.Text + " " + txbTakeOffOffset.Text;
            if (!DateTimeOffset.TryParse(sPiTop, out PiTop))
            {
                throw new Global.excToPopup("Internal software error in FlighLogInput.aspx.cs: gvFliN_RowUpdating.sPiTop takeoff: badly formed input to DateTimeOffset.TryParse = '" + sPiTop + "'");
            }
            DateTimeOffset DTakeOff = PiTop;

            DropDownList DDLDiLocLanding = (DropDownList)gvFliN.Rows[e.RowIndex].FindControl("DDLDiLocLanding");
            int iLocLanding = Int32.Parse(DDLDiLocLanding.SelectedItem.Value);

            TextBox txbDLandingDate = (TextBox)gvFliN.Rows[e.RowIndex].FindControl("txbDLandingDate");
            TextBox txbDLandingTime = (TextBox)gvFliN.Rows[e.RowIndex].FindControl("txbDLandingTime");
            TextBox txbLandingOffset = (TextBox)gvFliN.Rows[e.RowIndex].FindControl("txbLandingOffset");
            PiTop = DateTimeOffset.MinValue;
            sPiTop = txbDLandingDate.Text + " " + txbDLandingTime.Text + " " + txbLandingOffset.Text;
            if (!DateTimeOffset.TryParse(sPiTop, out PiTop))
            {
                throw new Global.excToPopup("Internal software error in FlighLogInput.aspx.cs: gvFliN_RowUpdating.sPiTop landing: badly formed input to DateTimeOffset.TryParse = '" + sPiTop + "'");
            }
            DateTimeOffset DLanding = PiTop;

            DropDownList DDLDiChargeCode = (DropDownList)gvFliN.Rows[e.RowIndex].FindControl("DDLDiChargeCode");
            int iChargeCode = Int32.Parse(DDLDiChargeCode.SelectedItem.Value);
            decimal mAmtCollected = decimal.Parse(((TextBox)gvFliN.Rows[e.RowIndex].FindControl("txbDmAmtCollected")).Text);
            string sComments = Server.HtmlEncode(((TextBox)gvFliN.Rows[e.RowIndex].FindControl("txbDsComments")).Text.Replace("'", "`"));

            // Validation
            // Tow Equipment must be able to do tows
            // Tow Equipment Operator must be qualified on the chosen tow equipment
            // Glider equiment must indeed be a glider
            // Pilot 1 role must be compatible with pilot qualifications
            // Pilot 2 role must be compatible with pilot qualifications
            // One of the pilot roles must be qualified for piloting this glider
            // dPctCharge1 + dPctCharge 2 == 100
            if (mPctCharge1 + mPctCharge2 != 100m)
            {
                ProcessPopupException(new Global.excToPopup("ERROR: Sum of percent charge to each pilot is " + (mPctCharge1 + mPctCharge2).ToString() + " which is not = 100"));
                return;
            }
            // dMaxAltitude cannot be less than dReleaseAltitude unless dMaxAltitude is null or <= -2000
            if (mMaxAltitude > -2000.0m && mReleaseAltitude > mMaxAltitude)
            {
                ProcessPopupException(new Global.excToPopup("ERROR: Release altitude cannot be greater than max altitude"));
                return;
            }
            // Date portion of the takeoff date/time must be the same as the date of the flight log
            DateTimeOffset DTOTO = (from d in dc.DAILYFLIGHTLOGs where d.ID == iFlightLog select d.DFlightOps).First();
            if (DTOTO.Date != DTakeOff.Date)
            {
                ProcessPopupException(new Global.excToPopup("ERROR: Date of takoff " + DTakeOff.Date.ToString() + " must be the same as the flightlog's date: " + DTOTO.Date.ToString()));
                return;
            }
            // DTakeoff < DLanding
            if (DTakeOff > DLanding)
            {
                ProcessPopupException(new Global.excToPopup("ERROR: Time of takoff cannot be less than or equal to time of landing"));
                return;
            }
            // DtakeOff and DLanding ought to be on the same day
            if (DTakeOff.Date != DLanding.Date)
            {
                ProcessPopupException(new Global.excToPopup("WARNING: Time of takoff should be on the same day as time of landing"));
            }
            // mAmtCollected >= 0
            if (mAmtCollected < 0.00m)
            {
                ProcessPopupException(new Global.excToPopup("WARNING: Amount of money collected should not be negative."));
            }

            if (e.RowIndex == iLast)
            {
                // Adding a new record
                FLIGHTLOGROW flr = new FLIGHTLOGROW();
                flr.PiTRecordEntered = DateTimeOffset.UtcNow;
                flr.iRecordEnteredBy = mCRUD.GetPeopleIDfromWebSiteUserName(HttpContext.Current.User.Identity.Name);
                flr.iFliteLog = iFlightLog;

                //flr.cStatus = 'N';
                flr.cStatus = cStatus;

                flr.iTowEquip = iTowEquip;
                flr.iTowOperator = iTowOperator;
                flr.iGlider = iGlider;
                flr.iLaunchMethod = iLaunchMethod;
                flr.iPilot1 = iPilot1;
                flr.iAviatorRole1 = iAviatorRole1;
                flr.dPctCharge1 = mPctCharge1;
                flr.iPilot2 = iPilot2;
                flr.iAviatorRole2 = iAviatorRole2;
                flr.dPctCharge2 = mPctCharge2;
                flr.dReleaseAltitude = mReleaseAltitude;
                flr.dMaxAltitude = mMaxAltitude;
                flr.iLocTakeOff = iLocTakeOff;
                flr.DTakeOff = DTakeOff;
                flr.iLocLanding = iLocLanding;
                flr.DLanding = DLanding;
                flr.iChargeCode = iChargeCode;
                flr.mAmtCollected = mAmtCollected;
                flr.sComments = sComments;

                dc.FLIGHTLOGROWs.InsertOnSubmit(flr);
                sLog = "cStatus=" + flr.cStatus + ", iTowEquip=" + flr.iTowEquip.ToString() + ", iTowOperator=" + flr.iTowOperator.ToString() + ", iGlider=" + flr.iGlider.ToString() +
                    ", iLaunchMethod=" + flr.iLaunchMethod.ToString() +
                    ", iPilot1=" + flr.iPilot1.ToString() + ", iAviatorRole1=" + flr.iAviatorRole1.ToString() + ", dPctCharge1=" + flr.dPctCharge1.ToString() + ", iPilot2=" + flr.iPilot2.ToString() +
                    ", iAviatorRole2=" + flr.iAviatorRole2.ToString() + ", dPctCharge2=" + flr.dPctCharge2.ToString() + ", dReleaseAltitude=" + flr.dReleaseAltitude.ToString() + ", dMaxAltitude=" + flr.dMaxAltitude +
                    ", iLocTakeOff=" + flr.iLocTakeOff.ToString() + ", DTakeOff=" + flr.DTakeOff.ToString() + ", iLocLanding=" + flr.iLocLanding.ToString() +
                    ", DLanding=" + flr.DLanding.ToString() + ", iChargeCode=" + flr.iChargeCode.ToString() + ", mAmtCollected=" + flr.mAmtCollected.ToString() +
                    ", sComments=" + flr.sComments;

                DataTable dtLastUsedIn = new DataTable("LastUsedIn");
                string[] saIn = new string[(int)Enum.GetNames(typeof(Global.enPFFlightLogRowItems)).Length];
                saIn[(int)Global.enPFFlightLogRowItems.Version] = "1";
                saIn[(int)Global.enPFFlightLogRowItems.cStatus] = "N";
                saIn[(int)Global.enPFFlightLogRowItems.TowEquip] = DDLDiTowEquip.SelectedItem.Value;
                saIn[(int)Global.enPFFlightLogRowItems.TowOperator] = DDLDiTowOperator.SelectedItem.Value;
                saIn[(int)Global.enPFFlightLogRowItems.Glider] = DDLDiGlider.SelectedItem.Value;
                saIn[(int)Global.enPFFlightLogRowItems.LaunchMethod] = DDLDiLaunchMethod.SelectedItem.Value;
                saIn[(int)Global.enPFFlightLogRowItems.Pilot1] = DDLDPilot1.SelectedItem.Value;
                saIn[(int)Global.enPFFlightLogRowItems.AviatorRole1] = DDLDAviatorRole1.SelectedItem.Value;
                saIn[(int)Global.enPFFlightLogRowItems.PctCharge1] = txbDdPctCharge1.Text;
                saIn[(int)Global.enPFFlightLogRowItems.Pilot2] = DDLDPilot2.SelectedItem.Value;
                saIn[(int)Global.enPFFlightLogRowItems.AviatorRole2] = DDLDAviatorRole2.SelectedItem.Value;
                saIn[(int)Global.enPFFlightLogRowItems.PctCharge2] = txbDdPctCharge2.Text;
                saIn[(int)Global.enPFFlightLogRowItems.ReleaseAltitude] = txbDdReleaseAltitude.Text;
                saIn[(int)Global.enPFFlightLogRowItems.MaxAltitude] = (txbDdMaxAltitude.Text.Length < 1) ? "-2000" : txbDdMaxAltitude.Text; // (SCR 107)
                saIn[(int)Global.enPFFlightLogRowItems.LocTakeOff] = DDLDiLocTakeOff.SelectedItem.Value;
                saIn[(int)Global.enPFFlightLogRowItems.DateTakeOff] = DTakeOff.ToString();
                saIn[(int)Global.enPFFlightLogRowItems.LocLanding] = DDLDiLocLanding.SelectedItem.Value;
                saIn[(int)Global.enPFFlightLogRowItems.DateLanding] = DLanding.ToString();
                saIn[(int)Global.enPFFlightLogRowItems.ChargeCode] = DDLDiChargeCode.SelectedItem.Text;
                saIn[(int)Global.enPFFlightLogRowItems.AmtCollected] = mAmtCollected.ToString();
                saIn[(int)Global.enPFFlightLogRowItems.Comments] = sComments;

                AssistLi.LastUsedInputs_FlightLogRows(saIn, ref dtLastUsedIn);
                AccountProfile.CurrentUser.FlightLogRowsDefaults = dtLastUsedIn;
            }
            else
            {
                // Editing an existing record
                List<DataRow> liFlLRows = (List<DataRow>)Session["liFlLRows"];
                DataRow dr = liFlLRows[e.RowIndex];
                FLIGHTLOGROW flr = (from v in dc.FLIGHTLOGROWs where v.ID == (int)dr.ItemArray[0] select v).First();
                flr.PiTRecordEntered = DateTimeOffset.UtcNow;
                flr.iRecordEnteredBy = mCRUD.GetPeopleIDfromWebSiteUserName(HttpContext.Current.User.Identity.Name);

                flr.iTowEquip = iTowEquip;
                flr.iTowOperator = iTowOperator;

                //flr.cStatus = 'N';
                flr.cStatus = cStatus;

                flr.iGlider = iGlider;
                flr.iLaunchMethod = iLaunchMethod;
                flr.iPilot1 = iPilot1;
                flr.iAviatorRole1 = iAviatorRole1;
                flr.dPctCharge1 = mPctCharge1;
                flr.iPilot2 = iPilot2;
                flr.iAviatorRole2 = iAviatorRole2;
                flr.dPctCharge2 = mPctCharge2;
                flr.dReleaseAltitude = mReleaseAltitude;
                flr.dMaxAltitude = mMaxAltitude;
                flr.iLocTakeOff = iLocTakeOff;
                flr.DTakeOff = DTakeOff;
                flr.iLocLanding = iLocLanding;
                flr.DLanding = DLanding;
                flr.iChargeCode = iChargeCode;
                flr.mAmtCollected = mAmtCollected;
                flr.sComments = sComments;

                sLog = "cStatus=" + flr.cStatus + ", iTowEquip=" + flr.iTowEquip.ToString() + ", iTowOperator=" + flr.iTowOperator.ToString() + ", iGlider=" + flr.iGlider.ToString() +
                    ", iLaunchMethod=" + flr.iLaunchMethod.ToString() +
                    ", iPilot1=" + flr.iPilot1.ToString() + ", iAviatorRole1=" + flr.iAviatorRole1.ToString() + ", dPctCharge1=" + flr.dPctCharge1.ToString() + ", iPilot2=" + flr.iPilot2.ToString() +
                    ", iAviatorRole2=" + flr.iAviatorRole2.ToString() + ", dPctCharge2=" + flr.dPctCharge2.ToString() + ", dReleaseAltitude=" + flr.dReleaseAltitude.ToString() + ", dMaxAltitude=" + flr.dMaxAltitude +
                    ", iLocTakeOff=" + flr.iLocTakeOff.ToString() + ", DTakeOff=" + flr.DTakeOff.ToString() + ", iLocLanding=" + flr.iLocLanding.ToString() +
                    ", DLanding=" + flr.DLanding.ToString() + ", iChargeCode=" + flr.iChargeCode.ToString() + ", mAmtCollected=" + flr.mAmtCollected.ToString() +
                    ", sComments=" + flr.sComments;
                elt = ActivityLog.enumLogTypes.DataUpdate;
            }
            try
            {
                dc.SubmitChanges();
                ActivityLog.oLog(elt, 1, "FlightLogRows: " + sLog);
            }
            catch (Exception exc)
            {
                Global.excToPopup ex = new Global.excToPopup(exc.Message);
                ProcessPopupException(ex);
                return;
            }
            FillFlightLogRowsDataTable();
        }
        #endregion

        protected void pbReturn_MyClick(object sender, EventArgs e)
        {
            Server.Transfer("FlightLogInput.aspx");
        }

        // Removed per SCR147
        //protected void pbPost_MyClick(object sender, EventArgs e)
        //{
        //    Session["iFlightLog"] = -iFlightLog; // negative flight log pointer asks FlightLogInput.aspx.cs to post this flight log to the database tables OPERATIONS, OPDETAILS etc.
        //    Server.Transfer("FlightLogInput.aspx");
        //}
    }
}