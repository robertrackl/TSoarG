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
    public partial class Bridge_EqRoleLaunchMeth : System.Web.UI.Page
    {
        SCUD_Multi mCRUD = new SCUD_Multi();

        #region EditExisting Row
        private bool bEditExisting
        {
            get { return GetbEditExistingRow("bEditExisting"); }
            set { ViewState["bEditExisting"] = value; }
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
        private int iNRows { get { return iGetNgvRows("iNRows"); } set { ViewState["iNRows"] = value; } }
        private int iGetNgvRows(string suNgvRows)
        {
            if (ViewState[suNgvRows] == null)
            {
                return 0;
            }
            else
            {
                return (int)ViewState[suNgvRows];
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
            try
            {
                if ((btn.ID == "YesButton") && (btn.CommandName == "Delete"))
                {
                    switch (OkButton.CommandArgument)
                    {
                        case "LaunchMethodEquipRole":
                            // Delete one
                            Statistician.StatistDailyFlightLogDataContext stdc = new Statistician.StatistDailyFlightLogDataContext();
                            var l = (from B in stdc.BRIDGE_LAUNCHMETH_EQUIPROLEs where B.ID == Int32.Parse(btn.CommandArgument) select B).First();
                            stdc.BRIDGE_LAUNCHMETH_EQUIPROLEs.DeleteOnSubmit(l);
                            try
                            {
                                stdc.SubmitChanges();
                                ActivityLog.oLog(ActivityLog.enumLogTypes.DataDeletion, 1, "Bridge_EqRoleLaunchMeth: deleted record with ID = " + btn.CommandArgument);
                            }
                            catch (Exception exc)
                            {
                                Global.excToPopup ex = new Global.excToPopup(exc.Message);
                                ProcessPopupException(ex);
                            }
                            finally
                            {
                                FillLaunchMethodEqRoleDataTable();
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

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                FillLaunchMethodEqRoleDataTable();
            }
        }

        private void FillLaunchMethodEqRoleDataTable()
        {
            List<DataRow> liLMEqR = AssistLi.Init(Global.enLL.LaunchMethodsEqRoles);
            Session["liLMEqR"] = liLMEqR;
            DataTable dtLMEqR = liLMEqR.CopyToDataTable();
            iNRows = dtLMEqR.Rows.Count; // number of rows in gvLaunchMEqRoles
            GridViewEditEventArgs gvee = new GridViewEditEventArgs(iNRows - 1);
            gvLaunchMEqRoles_RowEditing(null, gvee);
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

        protected void gvLaunchMEqRoles_RowEditing(object sender, GridViewEditEventArgs e)
        {
            List<DataRow> liLMEqR = (List<DataRow>)Session["liLMEqR"];
            gvLaunchMEqRoles.EditIndex = e.NewEditIndex;
            bEditExisting = false;
            if (e.NewEditIndex < iNRows - 1)
            {
                // We are editing an existing row; need to get rid of the last row which would be the New row (but there is no New row)
                liLMEqR.RemoveAt(liLMEqR.Count - 1);
                bEditExisting = true;
            }
            DataTable dtLMEqR = liLMEqR.CopyToDataTable();
            dt_BindToGV(gvLaunchMEqRoles, dtLMEqR);
        }
        
        private void dt_BindToGV(GridView gvu, DataTable dtu)
        {
            gvu.DataSource = dtu;
            gvu.DataBind();
        }

        protected void gvLaunchMEqRoles_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if ((e.Row.RowState & DataControlRowState.Edit) != 0)
                {
                    Statistician.StatistDailyFlightLogDataContext stdc = new Statistician.StatistDailyFlightLogDataContext();

                    DropDownList DDLDLaunchMethod = (DropDownList)e.Row.FindControl("DDLDLaunchMethod");
                    var q0 = from d in stdc.LAUNCHMETHODs  orderby d.sLaunchMethod select new { d.ID, sLaunchMethod = d.ID.ToString() + " - " + d.sLaunchMethod };
                    DDLDLaunchMethod.DataSource = q0;
                    DDLDLaunchMethod.DataValueField = "ID";
                    DDLDLaunchMethod.DataTextField = "sLaunchMethod";
                    DDLDLaunchMethod.DataBind();
                    Set_DropDown_ByText(DDLDLaunchMethod, DataBinder.Eval(e.Row.DataItem, "sLaunchMethod").ToString());

                    DropDownList DDLDEqRole = (DropDownList)e.Row.FindControl("DDLDEqRole");
                    var q1 = from R in stdc.EQUIPMENTROLEs orderby R.sEquipmentRole select new { R.ID, sEqRole = R.ID.ToString() + " - " + R.sEquipmentRole };
                    DDLDEqRole.DataSource = q1;
                    DDLDEqRole.DataValueField = "ID";
                    DDLDEqRole.DataTextField = "sEqRole";
                    DDLDEqRole.DataBind();
                    Set_DropDown_ByText(DDLDEqRole, DataBinder.Eval(e.Row.DataItem, "sEqRole").ToString());

                    int iLast = iNRows - 1; // The standard editing row
                    int iColAddButton = gvLaunchMEqRoles.Columns.Count - 2; // Column of the Edit and Add buttons
                    if (!bEditExisting)
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
                        //ImageButton pbC = (ImageButton)e.Row.Cells[iColAddButton].Controls[2];
                        //pbC.Visible = false;
                    }
                }
            }
        }

        protected void gvLaunchMEqRoles_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            Label lblIID = (Label)gvLaunchMEqRoles.Rows[e.RowIndex].FindControl("lblIID");
            string sItem = lblIID.Text;
            ButtonsClear();
            YesButton.CommandName = "Delete";
            YesButton.CommandArgument = sItem;
            OkButton.CommandArgument = "LaunchMethodEquipRole";
            lblPopupText.Text = "Please confirm deletion of Launch Method / Equipment Role combination in row/line with Internal ID = '" + sItem + "'";
            MPE_Show(Global.enumButtons.NoYes);
        }

        protected void gvLaunchMEqRoles_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            FillLaunchMethodEqRoleDataTable();
        }

        protected void gvLaunchMEqRoles_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            string sLog = "";
            ActivityLog.enumLogTypes elt = ActivityLog.enumLogTypes.DataInsert;
            Statistician.StatistDailyFlightLogDataContext stdc = new Statistician.StatistDailyFlightLogDataContext();
            int iLast = iNRows - 1; // index to last row (regardless of whether a row was trimmed off in FillLaunchMethodEqRoleDataTable()).

            DropDownList DDLDLaunchMethod = (DropDownList)gvLaunchMEqRoles.Rows[e.RowIndex].FindControl("DDLDLaunchMethod");
            int iLaunchMethod = Int32.Parse(DDLDLaunchMethod.SelectedItem.Value);
            DropDownList DDLDEqRole = (DropDownList)gvLaunchMEqRoles.Rows[e.RowIndex].FindControl("DDLDEqRole");
            int iEqRole = Int32.Parse(DDLDEqRole.SelectedItem.Value);
            string sComments = Server.HtmlEncode(((TextBox)gvLaunchMEqRoles.Rows[e.RowIndex].FindControl("txbDsComments")).Text.Replace("'", "`"));

            // Validation

            if (e.RowIndex == iLast)
            {
                // Adding a new record
                Statistician.BRIDGE_LAUNCHMETH_EQUIPROLE lmr = new Statistician.BRIDGE_LAUNCHMETH_EQUIPROLE()
                {
                    PiTRecordEntered = DateTime.UtcNow,
                    iRecordEnteredBy = mCRUD.GetPeopleIDfromWebSiteUserName(HttpContext.Current.User.Identity.Name),
                    iLaunchMethod = iLaunchMethod,
                    iEquipRole = iEqRole,
                    sComment = sComments
                };

                stdc.BRIDGE_LAUNCHMETH_EQUIPROLEs.InsertOnSubmit(lmr);
                sLog = "PiTRecordEntered=" + lmr.PiTRecordEntered.ToString() + ", iRecordEnteredBy=" + lmr.iRecordEnteredBy + ", iLaunchMethod=" + lmr.iLaunchMethod.ToString() +
                    ", iEquipRole=" + lmr.iEquipRole.ToString() + ", sComment=" + lmr.sComment;

            }
            else
            {
                // Editing an existing record
                List<DataRow> liLMEqR = (List<DataRow>)Session["liLMEqR"];
                DataRow dr = liLMEqR[e.RowIndex];
                Statistician.BRIDGE_LAUNCHMETH_EQUIPROLE lmr = (from v in stdc.BRIDGE_LAUNCHMETH_EQUIPROLEs where v.ID == (int)dr.ItemArray[0] select v).First();
                lmr.PiTRecordEntered = DateTime.UtcNow;
                lmr.iRecordEnteredBy = mCRUD.GetPeopleIDfromWebSiteUserName(HttpContext.Current.User.Identity.Name);
                lmr.iLaunchMethod = iLaunchMethod;
                lmr.iEquipRole = iEqRole;
                lmr.sComment = sComments;

                sLog = "PiTRecordEntered=" + lmr.PiTRecordEntered.ToString() + ", iRecordEnteredBy=" + lmr.iRecordEnteredBy + ", iLaunchMethod=" + lmr.iLaunchMethod.ToString() +
                    ", iEquipRole=" + lmr.iEquipRole.ToString() + ", sComment=" + lmr.sComment;
                elt = ActivityLog.enumLogTypes.DataUpdate;
            }
            try
            {
                stdc.SubmitChanges();
                ActivityLog.oLog(elt, 1, "EquipRolesTypes: " + sLog);
            }
            catch (Exception exc)
            {
                Global.excToPopup ex = new Global.excToPopup(exc.Message);
                ProcessPopupException(ex);
                return;
            }
            FillLaunchMethodEqRoleDataTable();
        }
    }
}