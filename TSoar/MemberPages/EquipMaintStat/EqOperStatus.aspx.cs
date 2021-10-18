using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using TSoar.Accounting;
using TSoar.DB;
using TSoar.Equipment;

namespace TSoar.MemberPages.EquipMaintStat
{
    public partial class EqOperStatus : System.Web.UI.Page
    {
        SCUD_Multi mCRUD = new SCUD_Multi();

        #region Boolean Variables
        private bool bUserHasEquipmentRole
        {
            get { return GetbEditExistingRow("bUserHasEquipmentRole"); }
            set { ViewState["bUserHasEquipmentRole"] = value; }
        }
        private bool GetbEditExistingRow(string suBool)
        {
            if (ViewState[suBool] == null)
            {
                return false;
            }
            else
            {
                return (bool)ViewState[suBool];
            }
        }
        #endregion
        //#region Integer Variables
        //private int iNgvRows { get { return iGetInt("iNgvRows"); } set { ViewState["iNgvRows"] = value; } } // number of equipment components + 1
        //// iEdRow indexes the row of interest (the one being edited) starting at 0 and ignoring paging
        //private int iEdRow { get { return iGetInt("iEdRow"); } set { ViewState["iEdRow"] = value; } }
        //private int iIndexOfLastPage { get { return iGetInt("iIndexOfLastPage"); } set { ViewState["iIndexOfLastPage"] = value; } }
        //private int iGetInt(string suN)
        //{
        //    if (ViewState[suN] == null)
        //    {
        //        if (suN == "iEdRow") return -1;
        //        return 0;
        //    }
        //    else
        //    {
        //        return (int)ViewState[suN];
        //    }
        //}
        //#endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string[] saRoles = Roles.GetRolesForUser();
                // Can user edit equipment status?
                bUserHasEquipmentRole = Array.Exists(saRoles,
                    element => element == Global.engRoles.Equipment.ToString() || element == Global.engRoles.Admin.ToString());
                FillDataTable();
            }
        }

        private void FillDataTable()
        {
            List<DataRow> li;
            try
            {
                li = AssistLi.Init(Global.enLL.EquipStatus);
            }
            catch (Global.excToPopup exctp)
            {
                ProcessPopupException(exctp);
                return;
            }
            Session["liEqStat"] = li;
            dt_BindTogvEqOpSt(li.CopyToDataTable());
        }

        private void dt_BindTogvEqOpSt(DataTable dtu)
        {
            GridView GV = gvEqOpSt;
            DataView view = new DataView(dtu)
            {
                Sort = "sSorter ASC, iLine ASC" // SCR 218
            };
            GV.DataSource = view;
            GV.DataBind();
        }

        protected void gvEqOpSt_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if ((e.Row.RowState & DataControlRowState.Edit) == 0)
                {
                    ImageButton ipbEEdit = (ImageButton)e.Row.FindControl("ipbEEdit");
                    ipbEEdit.Enabled = bUserHasEquipmentRole;
                    if (!bUserHasEquipmentRole)
                    {
                        ipbEEdit.ImageUrl = "~/i/GrayButton.jpg";
                    }
                    ActivityLog.oDiag("EqOperStatus", 5, (string)DataBinder.Eval(e.Row.DataItem, "sDebug"));
                }
                // SCR 218 Start
                int iLine = (int)DataBinder.Eval(e.Row.DataItem, "iLine");
                if (iLine >= 20000) 
                {
                    for (int iCell = 2; iCell < 11; iCell++)
                    {
                        foreach (Control ctrl in e.Row.Cells[iCell].Controls)
                        {
                            ctrl.Visible = false;
                        }
                    }
                }
                if ((int)DataBinder.Eval(e.Row.DataItem, "iPercentComplete") < 0) // iPercentComplete is set to -1 in tfEqCumData.sql when it is irrelevant
                {
                    Label lblIPercentComplete = (Label)e.Row.FindControl("lblIPercentComplete");
                    lblIPercentComplete.Text = "";
                }
                // SCR 218 End
            }
        }

        protected void gvEqOpSt_RowEditing(object sender, GridViewEditEventArgs e)
        {
            GridView gv = gvEqOpSt;
            gv.EditIndex = e.NewEditIndex;
            FillDataTable();
        }

        protected void gvEqOpSt_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvEqOpSt.PageIndex = e.NewPageIndex;
            FillDataTable();
        }

        protected void gvEqOpSt_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvEqOpSt.EditIndex = -1;
            FillDataTable();
        }

        protected void gvEqOpSt_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            Equipment.EquipmentDataContext eqdc = new Equipment.EquipmentDataContext();
            GridViewRow gvRow = gvEqOpSt.Rows[e.RowIndex];
            string sStatus = Server.HtmlEncode(((TextBox)gvRow.FindControl("txbDOperStat")).Text.Replace("'", "`"));
            string sComment = Server.HtmlEncode(((TextBox)gvRow.FindControl("txbDComment")).Text.Replace("'", "`"));
            int iUser = mCRUD.GetPeopleIDfromWebSiteUserName(HttpContext.Current.User.Identity.Name);
            int iID = Int32.Parse(((Label)gvRow.FindControl("lblIIdent")).Text);

            EQUIPCOMPONENT f =(from v in eqdc.EQUIPCOMPONENTs where v.ID == iID select v).First();
            f.PiTRecordEntered = DateTime.UtcNow;
            f.iRecordEnteredBy = iUser;
            f.sOperStatus = sStatus;
            f.sComment = sComment;
            try
            {
                eqdc.SubmitChanges();
            }
            catch (Exception exc)
            {
                Global.excToPopup ex = new Global.excToPopup(exc.Message);
                ProcessPopupException(ex);
                return;
            }
            eqdc.Dispose();
            gvEqOpSt.EditIndex = -1;
            FillDataTable();
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
            // Button btn = (Button)sender;
        }

        private void ProcessPopupException(Global.excToPopup excu)
        {
            ButtonsClear();
            lblPopupText.Text = excu.sExcMsg();
            MPE_Show(Global.enumButtons.OkOnly);
        }
        #endregion
    }
}