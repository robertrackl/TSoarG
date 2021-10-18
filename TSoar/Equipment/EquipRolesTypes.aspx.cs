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
    public partial class EquipRolesTypes : System.Web.UI.Page
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
                        case "RoleType":
                            // Delete one daily flight log
                            EquipmentDataContext eqdc = new EquipmentDataContext();
                            var l = (from w in eqdc.EQUIPROLESTYPEs where w.ID == Int32.Parse(btn.CommandArgument) select w).First();
                            eqdc.EQUIPROLESTYPEs.DeleteOnSubmit(l);
                            try
                            {
                                eqdc.SubmitChanges();
                                ActivityLog.oLog(ActivityLog.enumLogTypes.DataDeletion, 1, "EquipRolesTypes: deleted record with ID = " + btn.CommandArgument);
                            }
                            catch (Exception exc)
                            {
                                Global.excToPopup ex = new Global.excToPopup(exc.Message);
                                ProcessPopupException(ex);
                            }
                            finally
                            {
                                FillRolesTypesDataTable();
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
                FillRolesTypesDataTable();
            }
        }

        private void FillRolesTypesDataTable()
        {
            List<DataRow> liEqRoTy = AssistLi.Init(Global.enLL.EquipRolesTypes);
            Session["liEqRoTy"] = liEqRoTy;
            DataTable dtEqRoTy = liEqRoTy.CopyToDataTable();
            iNRows = dtEqRoTy.Rows.Count; // number of rows in gvRolesTypes
            GridViewEditEventArgs gvee = new GridViewEditEventArgs(iNRows - 1);
            gvRolesTypes_RowEditing(null, gvee);
        }

        protected void gvRolesTypes_RowEditing(object sender, GridViewEditEventArgs e)
        {
            List<DataRow> liEqRoTy = (List<DataRow>)Session["liEqRoTy"];
            gvRolesTypes.EditIndex = e.NewEditIndex;
            bEditExisting = false;
            if (e.NewEditIndex < iNRows - 1)
            {
                // We are editing an existing row; need to get rid of the last row which would be the New row (but there is no New row)
                liEqRoTy.RemoveAt(liEqRoTy.Count - 1);
                bEditExisting = true;
            }
            DataTable dtEqRoTy = liEqRoTy.CopyToDataTable();
            dt_BindToGV(gvRolesTypes, dtEqRoTy);
        }

        private void dt_BindToGV(GridView gvu, DataTable dtu)
        {
            gvu.DataSource = dtu;
            gvu.DataBind();
        }

        protected void gvRolesTypes_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                //e.Row.Cells[6].HorizontalAlign = HorizontalAlign.Right;
                if ((e.Row.RowState & DataControlRowState.Edit) != 0)
                {
                    EquipmentDataContext eqdc = new EquipmentDataContext();

                    DropDownList DDLDRole = (DropDownList)e.Row.FindControl("DDLDRole");
                    var q0 = from d in eqdc.EQUIPMENTROLEs orderby d.sEquipmentRole select new { d.ID, sRole = d.ID.ToString() + " - " + d.sEquipmentRole };
                    DDLDRole.DataSource = q0;
                    DDLDRole.DataValueField = "ID";
                    DDLDRole.DataTextField = "sRole";
                    DDLDRole.DataBind();
                    Set_DropDown_ByText(DDLDRole, DataBinder.Eval(e.Row.DataItem, "sRole").ToString());

                    DropDownList DDLDType = (DropDownList)e.Row.FindControl("DDLDType");
                    var q1 = from T in eqdc.EQUIPTYPEs orderby T.sEquipmentType select new { T.ID, sType = T.ID.ToString() + " - " + T.sEquipmentType };
                    DDLDType.DataSource = q1;
                    DDLDType.DataValueField = "ID";
                    DDLDType.DataTextField = "sType";
                    DDLDType.DataBind();
                    Set_DropDown_ByText(DDLDType, DataBinder.Eval(e.Row.DataItem, "sType").ToString());

                    int iLast = iNRows - 1; // The standard editing row
                    int iColAddButton = gvRolesTypes.Columns.Count - 2; // Column of the Edit and Add buttons
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

        protected void gvRolesTypes_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            Label lblIID = (Label)gvRolesTypes.Rows[e.RowIndex].FindControl("lblIID");
            string sItem = lblIID.Text;
            ButtonsClear();
            YesButton.CommandName = "Delete";
            YesButton.CommandArgument = sItem;
            OkButton.CommandArgument = "RoleType";
            lblPopupText.Text = "Please confirm deletion of Role/Type combination in row/line with Internal ID = '" + sItem + "'";
            MPE_Show(Global.enumButtons.NoYes);
        }

        protected void gvRolesTypes_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            FillRolesTypesDataTable();
        }

        protected void gvRolesTypes_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            string sLog = "";
            ActivityLog.enumLogTypes elt = ActivityLog.enumLogTypes.DataInsert;
            EquipmentDataContext eqdc = new EquipmentDataContext();
            int iLast = iNRows - 1; // index to last row (regardless of whether a row was trimmed off in FillRolesTypesDataTable()).

            DropDownList DDLDRole = (DropDownList)gvRolesTypes.Rows[e.RowIndex].FindControl("DDLDRole");
            int iRole = Int32.Parse(DDLDRole.SelectedItem.Value);
            DropDownList DDLDType = (DropDownList)gvRolesTypes.Rows[e.RowIndex].FindControl("DDLDType");
            int iType = Int32.Parse(DDLDType.SelectedItem.Value);
            string sComments = Server.HtmlEncode(((TextBox)gvRolesTypes.Rows[e.RowIndex].FindControl("txbDsComments")).Text.Replace("'", "`"));

            // Validation

            if (e.RowIndex == iLast)
            {
                // Adding a new record
                EQUIPROLESTYPE ert = new EQUIPROLESTYPE
                {
                    PiTRecordEntered = DateTime.UtcNow,
                    iRecordEnteredBy = mCRUD.GetPeopleIDfromWebSiteUserName(HttpContext.Current.User.Identity.Name),
                    iEquipRole = iRole,
                    iEquipType = iType,
                    sComments = sComments
                };

                eqdc.EQUIPROLESTYPEs.InsertOnSubmit(ert);
                sLog = "PiTRecordEntered=" + ert.PiTRecordEntered.ToString() + ", iRecordEnteredBy=" + ert.iRecordEnteredBy + ", iEquipRole=" + ert.iEquipRole.ToString() +
                    ", iEquipType=" + ert.iEquipType.ToString() + ", sComments=" + ert.sComments;

            }
            else
            {
                // Editing an existing record
                List<DataRow> liEqRoTy = (List<DataRow>)Session["liEqRoTy"];
                DataRow dr = liEqRoTy[e.RowIndex];
                EQUIPROLESTYPE ert = (from v in eqdc.EQUIPROLESTYPEs where v.ID == (int)dr.ItemArray[0] select v).First();
                ert.PiTRecordEntered = DateTime.UtcNow;
                ert.iRecordEnteredBy = mCRUD.GetPeopleIDfromWebSiteUserName(HttpContext.Current.User.Identity.Name);
                ert.iEquipRole = iRole;
                ert.iEquipType = iType;
                ert.sComments = sComments;

                sLog = "PiTRecordEntered=" + ert.PiTRecordEntered.ToString() + ", iRecordEnteredBy=" + ert.iRecordEnteredBy + ", iEquipRole=" + ert.iEquipRole.ToString() +
                    ", iEquipType=" + ert.iEquipType.ToString() + ", sComments=" + ert.sComments;
                elt = ActivityLog.enumLogTypes.DataUpdate;
            }
            try
            {
                eqdc.SubmitChanges();
                ActivityLog.oLog(elt, 1, "EquipRolesTypes: " + sLog);
            }
            catch (Exception exc)
            {
                Global.excToPopup ex = new Global.excToPopup(exc.Message);
                ProcessPopupException(ex);
                return;
            }
            FillRolesTypesDataTable();
        }
    }
}