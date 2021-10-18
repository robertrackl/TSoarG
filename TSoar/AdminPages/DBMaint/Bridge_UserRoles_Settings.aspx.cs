using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TSoar.Accounting;

namespace TSoar.AdminPages.DBMaint
{
    public partial class Bridge_UserRoles_Settings : System.Web.UI.Page
    {
        DB.SCUD_Multi mCRUD = new DB.SCUD_Multi();

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
                        case "UserRoleSetting":
                            // Delete one
                            DataIntegrityDataContext didc = new DataIntegrityDataContext();
                            var l = (from B in didc.SETTINGSROLESBRIDGEs where B.ID == Int32.Parse(btn.CommandArgument) select B).First();
                            didc.SETTINGSROLESBRIDGEs.DeleteOnSubmit(l);
                            try
                            {
                                didc.SubmitChanges();
                                ActivityLog.oLog(ActivityLog.enumLogTypes.DataDeletion, 1, "Bridge_UserRoles_Settings: deleted record with ID = " + btn.CommandArgument);
                            }
                            catch (Exception exc)
                            {
                                Global.excToPopup ex = new Global.excToPopup(exc.Message);
                                ProcessPopupException(ex);
                            }
                            finally
                            {
                                FillUserRoleSettingDataTable();
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
                FillUserRoleSettingDataTable();
            }
        }

        private void FillUserRoleSettingDataTable()
        {
            List<DataRow> liURS = AssistLi.Init(Global.enLL.UserRolesSettings);
            Session["liURS"] = liURS;
            iNRows = liURS.Count; // number of rows in gvLaunchMEqRoles
            GridViewEditEventArgs gvee = new GridViewEditEventArgs(iNRows - 1);
            gvURolesSettings_RowEditing(null, gvee);
        }

        protected void gvURolesSettings_RowEditing(object sender, GridViewEditEventArgs e)
        {
            List<DataRow> liURS = (List<DataRow>)Session["liURS"];
            gvURolesSettings.EditIndex = e.NewEditIndex;
            bEditExisting = false;
            if (e.NewEditIndex < iNRows - 1)
            {
                // We are editing an existing row; need to get rid of the last row which would be the New row (but there is no New row)
                liURS.RemoveAt(liURS.Count - 1);
                bEditExisting = true;
            }
            DataTable dtURS = liURS.CopyToDataTable();
            dt_BindToGV(gvURolesSettings, dtURS);
        }

        private void dt_BindToGV(GridView gvu, DataTable dtu)
        {
            gvu.DataSource = dtu;
            gvu.DataBind();
        }

        protected void gvURolesSettings_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if ((e.Row.RowState & DataControlRowState.Edit) != 0)
                {
                    DataIntegrityDataContext didc = new DataIntegrityDataContext();

                    DropDownList DDLDUserRole = (DropDownList)e.Row.FindControl("DDLDUserRole");
                    var q0 = from r in didc.aspnet_Roles orderby r.RoleName select new { r.RoleId, r.RoleName };
                    DDLDUserRole.DataSource = q0;
                    DDLDUserRole.DataValueField = "RoleId";
                    DDLDUserRole.DataTextField = "RoleName";
                    DDLDUserRole.DataBind();
                    Set_DropDown_ByText(DDLDUserRole, DataBinder.Eval(e.Row.DataItem, "RoleName").ToString());

                    DropDownList DDLDSetting = (DropDownList)e.Row.FindControl("DDLDSetting");
                    var q1 = from s in didc.SETTINGs
                             where s.bUserSelectable == true
                             orderby s.sSettingName
                             select new { s.ID, sSetting = s.ID.ToString() + " - " + s.sSettingName };
                    DDLDSetting.DataSource = q1;
                    DDLDSetting.DataValueField = "ID";
                    DDLDSetting.DataTextField = "sSetting";
                    DDLDSetting.DataBind();
                    Set_DropDown_ByText(DDLDSetting, DataBinder.Eval(e.Row.DataItem, "iSetting").ToString() + " - "
                        + DataBinder.Eval(e.Row.DataItem, "sSettingName").ToString());

                    int iLast = iNRows - 1; // The standard editing row
                    int iColAddButton = gvURolesSettings.Columns.Count - 2; // Column of the Edit and Add buttons
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
                    }
                }
            }
        }

        protected void gvURolesSettings_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            Label lblIID = (Label)gvURolesSettings.Rows[e.RowIndex].FindControl("lblIID");
            string sItem = lblIID.Text;
            ButtonsClear();
            YesButton.CommandName = "Delete";
            YesButton.CommandArgument = sItem;
            OkButton.CommandArgument = "UserRoleSetting";
            lblPopupText.Text = "Please confirm deletion of website user role / user-selectable setting combination in row/line with Internal ID = '" + sItem + "'";
            MPE_Show(Global.enumButtons.NoYes);
        }

        protected void gvURolesSettings_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            FillUserRoleSettingDataTable();
        }

        protected void gvURolesSettings_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            string sLog = "";
            ActivityLog.enumLogTypes elt = ActivityLog.enumLogTypes.DataInsert;
            DataIntegrityDataContext didc = new DataIntegrityDataContext();
            int iLast = iNRows - 1; // index to last row (regardless of whether a row was trimmed off in FillUserRoleSettingDataTable()).

            DropDownList DDLDUserRole = (DropDownList)gvURolesSettings.Rows[e.RowIndex].FindControl("DDLDUserRole");
            Guid uiRole = Guid.Parse(DDLDUserRole.SelectedItem.Value);
            DropDownList DDLDSetting = (DropDownList)gvURolesSettings.Rows[e.RowIndex].FindControl("DDLDSetting");
            int iSetting = Int32.Parse(DDLDSetting.SelectedItem.Value);
            string sComments = Server.HtmlEncode(((TextBox)gvURolesSettings.Rows[e.RowIndex].FindControl("txbDsComments")).Text.Replace("'", "`"));

            if (e.RowIndex == iLast)
            {
                // Adding a new record
                SETTINGSROLESBRIDGE urss = new SETTINGSROLESBRIDGE()
                {
                    PiTRecordEntered = DateTime.UtcNow,
                    iRecordEnteredBy = mCRUD.GetPeopleIDfromWebSiteUserName(HttpContext.Current.User.Identity.Name),
                    uiRole = uiRole,
                    iSetting = iSetting,
                    sComments = sComments
                };

                didc.SETTINGSROLESBRIDGEs.InsertOnSubmit(urss);
                sLog = "PiTRecordEntered=" + urss.PiTRecordEntered.ToString() + ", iRecordEnteredBy=" + urss.iRecordEnteredBy + ", uiRole=" + urss.uiRole.ToString() +
                    ", iSetting=" + urss.iSetting.ToString() + ", sComments=" + urss.sComments;

            }
            else
            {
                // Editing an existing record
                List<DataRow> liURS = (List<DataRow>)Session["liURS"];
                DataRow dr = liURS[e.RowIndex];
                SETTINGSROLESBRIDGE urss = (from v in didc.SETTINGSROLESBRIDGEs where v.ID == (int)dr.ItemArray[0] select v).First();
                urss.PiTRecordEntered = DateTime.UtcNow;
                urss.iRecordEnteredBy = mCRUD.GetPeopleIDfromWebSiteUserName(HttpContext.Current.User.Identity.Name);
                urss.uiRole = uiRole;
                urss.iSetting = iSetting;
                urss.sComments = sComments;

                sLog = "PiTRecordEntered=" + urss.PiTRecordEntered.ToString() + ", iRecordEnteredBy=" + urss.iRecordEnteredBy + ", uiRole=" + urss.uiRole.ToString() +
                    ", iSetting=" + urss.iSetting.ToString() + ", sComments=" + urss.sComments;
                elt = ActivityLog.enumLogTypes.DataUpdate;
            }
            try
            {
                didc.SubmitChanges();
                ActivityLog.oLog(elt, 1, "Bridge_UserRoles_Settings: " + sLog);
            }
            catch (Exception exc)
            {
                Global.excToPopup ex = new Global.excToPopup(exc.Message);
                ProcessPopupException(ex);
                return;
            }
            FillUserRoleSettingDataTable();
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

    }
}