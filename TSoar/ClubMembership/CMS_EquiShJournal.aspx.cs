using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TSoar.DB;
using TSoar.Accounting;

namespace TSoar.ClubMembership
{
    public partial class CMS_EquiShJournal : System.Web.UI.Page
    {
        SCUD_Multi mCRUD = new SCUD_Multi();

        #region ViewState Booleans
        private bool bEditExisting
        {
            get { return GetVSBoolean("bEditExisting", false); }
            set { ViewState["bEditExisting"] = value; }
        }
        private bool GetVSBoolean(string sub, bool buInitDefault)
        {
            if (ViewState[sub] == null)
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
        private int iNgvCMS_EquiSh { get { return iGetInteger("iNgvCMS_EquiSh"); } set { ViewState["iNgvCMS_EquiSh"] = value; } }
        private int iGetInteger(string suNgvRows)
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
                        case "EquityShare":
                            // Delete one equity share transaction row
                            ClubMembershipDataContext dc = new ClubMembershipDataContext();
                            var r = (from w in dc.EQUITYSHAREs where w.ID == Int32.Parse(btn.CommandArgument) select w).First();
                            dc.EQUITYSHAREs.DeleteOnSubmit(r);
                            try
                            {
                                dc.SubmitChanges();
                                ActivityLog.oLog(ActivityLog.enumLogTypes.DataDeletion, 1, "Deleted from table EQUITYSHARES record with ID " + btn.CommandArgument);
                            }
                            catch (Exception exc)
                            {
                                Global.excToPopup ex = new Global.excToPopup(exc.Message);
                                ProcessPopupException(ex);
                            }
                            finally
                            {
                                DisplayInGrid();
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
                DisplayInGrid();
            }
        }

        private void DisplayInGrid()
        {
            List<DataRow> liMeEquityShJ = AssistLi.Init(Global.enLL.MeEquityShJ);
            Session["liMeEquityShJ"] = liMeEquityShJ;
            iNgvCMS_EquiSh = liMeEquityShJ.Count;
            GridViewEditEventArgs gvee = new GridViewEditEventArgs(iNgvCMS_EquiSh - 1);
            gvCMS_EquiSh_RowEditing(null, gvee);
        }

        protected void gvCMS_EquiSh_RowEditing(object sender, GridViewEditEventArgs e)
        {
            List<DataRow> liMeEquityShJ = (List<DataRow>)Session["liMeEquityShJ"];
            gvCMS_EquiSh.EditIndex = e.NewEditIndex;
            bEditExisting = false;
            if (e.NewEditIndex < iNgvCMS_EquiSh - 1)
            {
                // We are editing an existing row; need to get rid of the last row which would be the New row (but there is no New row)
                liMeEquityShJ.RemoveAt(liMeEquityShJ.Count - 1);
                bEditExisting = true;
            }
            DataTable dtMeEquityShJ = liMeEquityShJ.CopyToDataTable();
            gvCMS_EquiSh.DataSource = dtMeEquityShJ;
            gvCMS_EquiSh.DataBind();
        }

        protected void gvCMS_EquiSh_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                //e.Row.Cells[6].HorizontalAlign = HorizontalAlign.Right;
                if ((e.Row.RowState & DataControlRowState.Edit) != 0)
                {
                    ClubMembershipDataContext cmdc = new ClubMembershipDataContext();
                    DropDownList DDLsDisplayName = (DropDownList)e.Row.FindControl("DDLsDisplayName");
                    var qP = from p in cmdc.PEOPLEs where p.sDisplayName != "[none]" select new { p.ID, sDisplayName=Server.HtmlDecode(p.sDisplayName) };
                    DDLsDisplayName.DataSource = qP;
                    DDLsDisplayName.DataValueField = "ID";
                    DDLsDisplayName.DataTextField = "sDisplayName";
                    DDLsDisplayName.DataBind();
                    Set_DropDown_ByValue(DDLsDisplayName, Server.HtmlDecode(DataBinder.Eval(e.Row.DataItem, "iOwner").ToString()));

                    TextBox txbDXaction = (TextBox)e.Row.FindControl("txbDXaction");
                    DateTime DXaction = (DateTime)DataBinder.Eval(e.Row.DataItem, "DXaction");
                    txbDXaction.Text = DXaction.ToString("yyyy-MM-dd");

                    DropDownList DDLcDateQuality = (DropDownList)e.Row.FindControl("DDLcDateQuality");
                    Set_DropDown_ByValue(DDLcDateQuality, DataBinder.Eval(e.Row.DataItem, "cDateQuality").ToString());

                    DropDownList DDLcXactType = (DropDownList)e.Row.FindControl("DDLcXactType");
                    Set_DropDown_ByValue(DDLcXactType, DataBinder.Eval(e.Row.DataItem, "cXactType").ToString());

                    int iLast = iNgvCMS_EquiSh - 1; // The standard editing row
                    int iColAddButton = gvCMS_EquiSh.Columns.Count - 2; // Column of the Edit and Add buttons
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

        protected void gvCMS_EquiSh_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            Label lblIID = (Label)gvCMS_EquiSh.Rows[e.RowIndex].FindControl("lblIID");
            string sItem = lblIID.Text;
            ButtonsClear();
            YesButton.CommandName = "Delete";
            YesButton.CommandArgument = sItem;
            OkButton.CommandArgument = "EquityShare";
            lblPopupText.Text = "Please confirm deletion of member equity share transaction row/line with Internal ID = '" + sItem + "'";
            MPE_Show(Global.enumButtons.NoYes);
        }

        protected void gvCMS_EquiSh_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            DisplayInGrid();
        }

        protected void gvCMS_EquiSh_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            string sLog = "";
            ActivityLog.enumLogTypes elt = ActivityLog.enumLogTypes.DataInsert;
            ClubMembershipDataContext dc = new ClubMembershipDataContext();

            DropDownList DDLsDisplayName = (DropDownList)gvCMS_EquiSh.Rows[e.RowIndex].FindControl("DDLsDisplayName");
            int iPerson = Int32.Parse(DDLsDisplayName.SelectedItem.Value);

            TextBox txbDXaction = (TextBox)gvCMS_EquiSh.Rows[e.RowIndex].FindControl("txbDXaction");
            DateTime DXaction = DateTime.Parse(txbDXaction.Text).Date;

            DropDownList DDLcDateQuality = (DropDownList)gvCMS_EquiSh.Rows[e.RowIndex].FindControl("DDLcDateQuality");
            char cDateQuality = DDLcDateQuality.SelectedItem.Value[0];

            TextBox txbdNumShares = (TextBox)gvCMS_EquiSh.Rows[e.RowIndex].FindControl("txbdNumShares");
            decimal dNumShares = Decimal.Parse(txbdNumShares.Text);

            DropDownList DDLcXactType = (DropDownList)gvCMS_EquiSh.Rows[e.RowIndex].FindControl("DDLcXactType");
            char cXactType = DDLcXactType.SelectedItem.Value[0];

            TextBox txbsInfoSource = (TextBox)gvCMS_EquiSh.Rows[e.RowIndex].FindControl("txbsInfoSource");
            string sInfoSource = txbsInfoSource.Text;

            TextBox txbsComment = (TextBox)gvCMS_EquiSh.Rows[e.RowIndex].FindControl("txbsComment");
            string sComment = txbsComment.Text;

            // Validation

            if (DXaction < new DateTime(1976, 1, 1))
            {
                ProcessPopupException(new Global.excToPopup("ERROR: Equity share transaction date must be later than December 31, 1975"));
                return;
            }
            if (DXaction > new DateTime(2149, 12, 31))
            {
                ProcessPopupException(new Global.excToPopup("ERROR: Equity share transaction date must be earlier than January 1, 2150"));
                return;
            }

            if (cXactType == 'P' || cXactType == 'R')
            {
                if (dNumShares < 0.0m)
                {
                    ProcessPopupException(new Global.excToPopup("ERROR: The number of shares purchased by a member (or reinstated after donation) must be zero or positive"));
                    return;
                }
            }
            else
            {
                if (dNumShares > 0.0m)
                {
                    ProcessPopupException(new Global.excToPopup("ERROR: The number of shares sold or donated by a member must be zero or negative"));
                    return;
                }
            }

            if (e.RowIndex == iNgvCMS_EquiSh - 1)
            {
                // Adding a new record
                EQUITYSHARE eqs = new EQUITYSHARE();
                eqs.PiTRecordEntered = DateTime.UtcNow;
                eqs.iRecordEnteredBy = mCRUD.GetPeopleIDfromWebSiteUserName(HttpContext.Current.User.Identity.Name);
                eqs.iOwner = iPerson;
                eqs.DXaction = DXaction;
                eqs.cDateQuality = cDateQuality;
                eqs.dNumShares = dNumShares;
                eqs.cXactType = cXactType;
                eqs.sInfoSource = Server.HtmlEncode(sInfoSource);
                eqs.sComment = Server.HtmlEncode(sComment);

                dc.EQUITYSHAREs.InsertOnSubmit(eqs);
                sLog = "iOwner=" + eqs.iOwner + ", DXaction=" + eqs.DXaction + ", cDateQuality=" + eqs.cDateQuality + ", dNumShares=" + eqs.dNumShares +
                    ", cXactType" + eqs.cXactType + ", sInfoSource=" + eqs.sInfoSource + ", sComment=" + eqs.sComment;
            }
            else
            {
                // Editing an existing record
                List<DataRow> liMeEquityShJ = (List<DataRow>)Session["liMeEquityShJ"];
                DataRow dr = liMeEquityShJ[e.RowIndex];
                EQUITYSHARE eqs = (from v in dc.EQUITYSHAREs where v.ID == (int)dr.ItemArray[0] select v).First();
                eqs.PiTRecordEntered = DateTime.UtcNow;
                eqs.iRecordEnteredBy = mCRUD.GetPeopleIDfromWebSiteUserName(HttpContext.Current.User.Identity.Name);
                eqs.iOwner = iPerson;
                eqs.DXaction = DXaction;
                eqs.cDateQuality = cDateQuality;
                eqs.dNumShares = dNumShares;
                eqs.cXactType = cXactType;
                eqs.sInfoSource = Server.HtmlEncode(sInfoSource);
                eqs.sComment = Server.HtmlEncode(sComment);
                sLog = "iOwner=" + eqs.iOwner + ", DXaction=" + eqs.DXaction + ", cDateQuality=" + eqs.cDateQuality + ", dNumShares=" + eqs.dNumShares +
                    ", cXactType" + eqs.cXactType + ", sInfoSource=" + eqs.sInfoSource + ", sComment=" + eqs.sComment;
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
            DisplayInGrid();
        }
    }
}