using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TSoar.Accounting;
using TSoar.DB;

namespace TSoar.ClubMembership
{
    public partial class CMS_PeopleEquipRolesTypes : System.Web.UI.Page
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
        #region Integer Properties
        private int ivNRows { get { return iGetNgvRows("ivNRows"); } set { ViewState["ivNRows"] = value; } }
        private int ivEditIndex { get { return iGetNgvRows("ivEditIndex"); } set { ViewState["ivEditIndex"] = value; } }
        private int iGetNgvRows(string su)
        {
            if (ViewState[su] == null)
            {
                switch (su)
                {
                    case "ivEditIndex":
                        return -1;
                    default:
                        return 0;
                }
            }
            else
            {
                return (int)ViewState[su];
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
                        case "PeopleEqRoTy":
                            // Delete one
                            TSoar.Equipment.EquipmentDataContext eqdc = new TSoar.Equipment.EquipmentDataContext();
                            var l = (from w in eqdc.PEOPLEEQUIPROLESTYPEs where w.ID == Int32.Parse(btn.CommandArgument) select w).First();
                            eqdc.PEOPLEEQUIPROLESTYPEs.DeleteOnSubmit(l);
                            try
                            {
                                eqdc.SubmitChanges();
                            }
                            catch (Exception exc)
                            {
                                Global.excToPopup ex = new Global.excToPopup(exc.Message);
                                ProcessPopupException(ex);
                            }
                            finally
                            {
                                ActivityLog.oLog(ActivityLog.enumLogTypes.DataDeletion, 1, "CMS_PeopleEquipRolesTypes: deleted record with ID = " + btn.CommandArgument);
                                FillPeopleEqRoTyDataTable(-1);
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
                gvPeopleEqRolesTypes.PageSize = Int32.Parse(mCRUD.GetSetting("PageSizePeopleEqRolesTypes"));
                FillPeopleEqRoTyDataTable(-1);
            }
            TSoar.Equipment.EquipmentDataContext eqdc = new Equipment.EquipmentDataContext();
            lblMembersCount.Text = (from m in eqdc.PEOPLEs select m).Count().ToString();
        }

        private void FillPeopleEqRoTyDataTable(int iuEditIndex)
        {
            // When iuEditIndex is -1 then we edit the last row, i.e., the one with potentially new data should it be added

            List<DataRow> liPersonEqRoTy = null;
            try
            {
                liPersonEqRoTy = AssistLi.Init(Global.enLL.PeopleEquipRolesTypes);
            }
            catch (Exception exc)
            {
                string sMsg = (char)34 + "../Equipment/EquipRolesTypes.aspx" + (char)34;
                sMsg = "Error in CMS_PeopleEquipRolesTypes.FillPeopleEqRoTyDataTable: '" + exc.Message +
                    "' which most likely means that you have not yet defined any allowable combinations of " +
                    "<a href=" + sMsg + ">Equipment Roles and Types</a> (website user requires the Equipment Membership Role). " +
                    "Possibly, but less likely, no aviator roles exist.";
                ProcessPopupException(new Global.excToPopup(sMsg));
                return;
            }
            Session["liPersonEqRoTy"] = liPersonEqRoTy;
            ivNRows = liPersonEqRoTy.Count;
            ivEditIndex = (iuEditIndex > -1) ? iuEditIndex : ivNRows - 1;
            GridView gv = gvPeopleEqRolesTypes;
            int iNewEditIndex = ivEditIndex;
            if (gv.AllowPaging)
            {
                int iFrom = gv.PageIndex * gv.PageSize;
                int iTo = iFrom + gv.PageSize - 1;
                if (iTo >= ivNRows) iTo = ivNRows;
                if (ivEditIndex >= iFrom && ivEditIndex <= iTo)
                {
                    iNewEditIndex=ivEditIndex % gv.PageSize;
                }
                else
                {
                    iNewEditIndex = -1;
                }
            }
            GridViewEditEventArgs gvee = new GridViewEditEventArgs(iNewEditIndex);
            gvPeopleEqRolesTypes_RowEditing(null, gvee);
        }

        protected void gvPeopleEqRolesTypes_RowEditing(object sender, GridViewEditEventArgs e)
        {
            GridView gv = gvPeopleEqRolesTypes;
            List<DataRow> liPersonEqRoTy = (List<DataRow>)Session["liPersonEqRoTy"];
            gv.EditIndex = e.NewEditIndex; // takes paging into account
            if (!(sender is null))
            {
                ivEditIndex = e.NewEditIndex + gv.PageIndex * gv.PageSize;
            }
            bEditExisting = false;
            if (ivEditIndex < liPersonEqRoTy.Count - 1)
            {
                // We are editing an existing row; need to get rid of the last row which would be the New row (but there is no New row)
                liPersonEqRoTy.RemoveAt(liPersonEqRoTy.Count - 1);
                bEditExisting = true;
            }
            DataTable dtPersonEqRoTy = liPersonEqRoTy.CopyToDataTable();
            dt_BindToGV(gvPeopleEqRolesTypes, dtPersonEqRoTy);
        }

        private void dt_BindToGV(GridView gvu, DataTable dtu)
        {
            gvu.DataSource = dtu;
            gvu.DataBind(); // Causes calls to gvRewards_RowDataBound for each GridViewRow, including the header row
        }

        protected void gvPeopleEqRolesTypes_PreRender(object sender, EventArgs e)
        {
            GridView gv = (GridView)sender;
            GridViewRow gvrb = gv.BottomPagerRow;
            if (gvrb != null)
            {
                gvrb.Visible = true;
            }
            GridViewRow gvrt = gv.TopPagerRow;
            if (gvrt != null)
            {
                gvrt.Visible = true;
            }
            lbl_ivEditIndex.Text = "ivEditIndex=" + ivEditIndex.ToString() + ", Rows.Count=" + gv.Rows.Count.ToString() +
                ", EditIndex=" + gv.EditIndex.ToString() + ", PageIndex=" + gv.PageIndex.ToString() + ", PageSize=" + gv.PageSize.ToString() +
                ", PageCount=" + gv.PageCount.ToString() + ((gv.DataSource is null)? "" : ", total rows=" + (gv.DataSource as DataTable).Rows.Count.ToString());
        }

        protected void DDL_DataBound(object sender, EventArgs e)
        {
            foreach(ListItem li in ((DropDownList)sender).Items)
            {
                li.Text = Server.HtmlDecode(li.Text);
            }
        }

        protected void gvPeopleEqRolesTypes_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                //e.Row.Cells[6].HorizontalAlign = HorizontalAlign.Right;
                if ((e.Row.RowState & DataControlRowState.Edit) != 0)
                {
                    TSoar.Equipment.EquipmentDataContext eqdc = new TSoar.Equipment.EquipmentDataContext();

                    DropDownList DDLDAvRole = (DropDownList)e.Row.FindControl("DDLDAvRole");
                    var qA = from A in eqdc.AVIATORROLEs orderby A.sAviatorRole select A;
                    DDLDAvRole.DataSource = qA;
                    DDLDAvRole.DataValueField = "ID";
                    DDLDAvRole.DataTextField = "sAviatorRole";
                    DDLDAvRole.DataBind();
                    Set_DropDown_ByText(DDLDAvRole, DataBinder.Eval(e.Row.DataItem, "sAviatorRole").ToString());

                    DropDownList DDLDPerson = (DropDownList)e.Row.FindControl("DDLDPerson");
                    var qP = from P in eqdc.PEOPLEs orderby P.sDisplayName select P;
                    DDLDPerson.DataSource = qP;
                    DDLDPerson.DataValueField = "ID";
                    DDLDPerson.DataTextField = "sDisplayName";
                    DDLDPerson.DataBind();
                    Set_DropDown_ByText(DDLDPerson, Server.HtmlDecode(DataBinder.Eval(e.Row.DataItem, "sDisplayName").ToString()));

                    DropDownList DDLDEqRoTy = (DropDownList)e.Row.FindControl("DDLDEqRoTy");
                    var q0 = from d in eqdc.sp_EquipRolesTypes() select new { d.ID, sEqRoTy = d.sRole + " / " + d.sType + " [" + d.sComments + "]" };
                    DDLDEqRoTy.DataSource = q0;
                    DDLDEqRoTy.DataValueField = "ID";
                    DDLDEqRoTy.DataTextField = "sEqRoTy";
                    DDLDEqRoTy.DataBind();
                    Set_DropDown_ByText(DDLDEqRoTy, DataBinder.Eval(e.Row.DataItem, "sEqRoleType").ToString());

                    int iLast = ivNRows - 1; // The standard editing row
                    int iColAddButton = gvPeopleEqRolesTypes.Columns.Count - 2; // Column of the Edit and Add buttons
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
            if (e.Row.RowType == DataControlRowType.Footer)
            {
                e.Row.Cells[0].Text = "Page " + (gvPeopleEqRolesTypes.PageIndex + 1) + " of " + gvPeopleEqRolesTypes.PageCount;
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

        protected void gvPeopleEqRolesTypes_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            Label lblIID = (Label)gvPeopleEqRolesTypes.Rows[e.RowIndex].FindControl("lblIID");
            string sItem = lblIID.Text;
            ButtonsClear();
            YesButton.CommandName = "Delete";
            YesButton.CommandArgument = sItem;
            OkButton.CommandArgument = "PeopleEqRoTy";
            lblPopupText.Text = "Please confirm deletion of People-AviatiorRole-Equipment-Role/Type combination in row/line with Internal ID = '" + sItem + "'";
            MPE_Show(Global.enumButtons.NoYes);
        }

        protected void gvPeopleEqRolesTypes_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            FillPeopleEqRoTyDataTable(-1);
        }

        protected void gvPeopleEqRolesTypes_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            string sLog = "";
            ActivityLog.enumLogTypes elt = ActivityLog.enumLogTypes.DataInsert;
            TSoar.Equipment.EquipmentDataContext eqdc = new TSoar.Equipment.EquipmentDataContext();

            DropDownList DDLDAvRole = (DropDownList)gvPeopleEqRolesTypes.Rows[e.RowIndex].FindControl("DDLDAvRole");
            int iAvRole = Int32.Parse(DDLDAvRole.SelectedItem.Value);
            DropDownList DDLDPerson = (DropDownList)gvPeopleEqRolesTypes.Rows[e.RowIndex].FindControl("DDLDPerson");
            int iPerson = Int32.Parse(DDLDPerson.SelectedItem.Value);
            DropDownList DDLDEqRoTy = (DropDownList)gvPeopleEqRolesTypes.Rows[e.RowIndex].FindControl("DDLDEqRoTy");
            int iEqRoTy = Int32.Parse(DDLDEqRoTy.SelectedItem.Value);
            string sComments = Server.HtmlEncode(((TextBox)gvPeopleEqRolesTypes.Rows[e.RowIndex].FindControl("txbDsComments")).Text.Replace("'", "`"));

            // We are adding a record if the internal Id displayed in the first column is 0
            Label lblIID = (Label)gvPeopleEqRolesTypes.Rows[e.RowIndex].FindControl("lblIID");
            int iId = Int32.Parse(lblIID.Text);
            if (iId == 0)
            {
                // Adding a new record
                TSoar.Equipment.PEOPLEEQUIPROLESTYPE ert = new TSoar.Equipment.PEOPLEEQUIPROLESTYPE
                {
                    PiTRecordEntered = DateTime.UtcNow,
                    iRecordEnteredBy = mCRUD.GetPeopleIDfromWebSiteUserName(HttpContext.Current.User.Identity.Name),
                    iAviatorRole = iAvRole,
                    iPerson = iPerson,
                    iRoleType = iEqRoTy,
                    sComments = sComments
                };

                eqdc.PEOPLEEQUIPROLESTYPEs.InsertOnSubmit(ert);
                sLog = "PiTRecordEntered=" + ert.PiTRecordEntered.ToString() + ", iRecordEnteredBy=" + ert.iRecordEnteredBy + ", iAviatorRole=" + ert.iAviatorRole.ToString() +
                    ", iPerson=" + ert.iPerson + ", iRoleType=" + ert.iRoleType.ToString() + ", sComments=" + ert.sComments;
            }
            else
            {
                // Editing an existing record
                TSoar.Equipment.PEOPLEEQUIPROLESTYPE ert = (from v in eqdc.PEOPLEEQUIPROLESTYPEs where v.ID == iId select v).First();
                ert.PiTRecordEntered = DateTime.UtcNow;
                ert.iRecordEnteredBy = mCRUD.GetPeopleIDfromWebSiteUserName(HttpContext.Current.User.Identity.Name);
                ert.iAviatorRole = iAvRole;
                ert.iPerson = iPerson;
                ert.iRoleType = iEqRoTy;
                ert.sComments = sComments;

                sLog = "PiTRecordEntered=" + ert.PiTRecordEntered.ToString() + ", iRecordEnteredBy=" + ert.iRecordEnteredBy + ", iAviatorRole=" + ert.iAviatorRole.ToString() +
                    ", iPerson=" + ert.iPerson + ", iRoleType=" + ert.iRoleType.ToString() + ", sComments=" + ert.sComments;
                elt = ActivityLog.enumLogTypes.DataUpdate;
            }
            try
            {
                eqdc.SubmitChanges();
                ActivityLog.oLog(elt, 1, "CMS_PeopleEquipRolesTypes: " + sLog);
            }
            catch (Exception exc)
            {
                Global.excToPopup ex = new Global.excToPopup(exc.Message);
                ProcessPopupException(ex);
                return;
            }
            FillPeopleEqRoTyDataTable(-1);
        }

        protected void gvPeopleEqRolesTypes_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridView gv = (GridView)sender;
            gv.PageIndex = e.NewPageIndex; // counts pages starting at 0
            FillPeopleEqRoTyDataTable(ivEditIndex);
        }
    }
}