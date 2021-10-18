using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TSoar.DB;
using TSoar.Accounting;

namespace TSoar.ClubMembership
{
    public partial class CMS_Offices : System.Web.UI.Page
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
        private int iNgvRows { get { return iGetNgvRows("iNgvRows"); } set { ViewState["iNgvRows"] = value; } } // number of equipment components + 1
        // iEdRow indexes the row of interest (the one being edited) starting at 0 and ignoring paging
        private int iEdRow { get { return iGetNgvRows("iEdRow"); } set { ViewState["iEdRow"] = value; } }
        private int iIndexOfLastPage { get { return iGetNgvRows("iIndexOfLastPage"); } set { ViewState["iIndexOfLastPage"] = value; } }
        private int iGetNgvRows(string suN)
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
                        if (OkButton.CommandArgument == "PeopleOffices")
                        {
                            // Delete the People Offices FromTo record
                            try
                            {
                                mCRUD.DeleteOne(Global.enugInfoType.PeopleOffices, btn.CommandArgument);
                            }
                            catch (Global.excToPopup exc)
                            {
                                ProcessPopupException(exc);
                            }
                            bEditExistingRow = false;
                            iEdRow = -1;
                            gvCMS_Offices.EditIndex = -1;
                            FillDataTable();
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

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                FillDataTable();
            }
        }

        private void FillDataTable()
        {
            GridView gv = gvCMS_Offices;
            List<DataRow> li = AssistLi.Init(Global.enLL.PeopleOffices);
            Session["liPeopleOffices"] = li;
            iNgvRows = li.Count; // number of rows in gvEqComp
            if (iEdRow == -1)
            {
                iEdRow = iNgvRows - 1;
            }
            int iNewEditIndex = iEdRow % gv.PageSize;
            int iIndexOfEditPage = iEdRow / gv.PageSize;
            iIndexOfLastPage = iNgvRows / gv.PageSize;
            if ((iNgvRows % gv.PageSize) == 0)
            {
                iIndexOfLastPage--;
            }
            // Is the row being edited in the current page?
            if (gv.PageIndex == iIndexOfEditPage)
            {
                GridViewEditEventArgs gvee = new GridViewEditEventArgs(iNewEditIndex);
                gvCMS_Offices_RowEditing(null, gvee);
            }
            else
            {
                gv.EditIndex = -1;
                bEditExistingRow = false;
                dt_BindToGV(li.CopyToDataTable());
            }
        }

        //protected void DDL_PreRender(object sender, EventArgs e)
        //{
        //    DropDownList ddl = (DropDownList)sender;
        //    if (gvCMS_Offices.EditIndex > -1)
        //    {
        //        string sText = "";
        //        TNPV_PeopleContactsDataContext pdc = new TNPV_PeopleContactsDataContext();
        //        int ii = Int32.Parse(((Label)gvCMS_Offices.Rows[gvCMS_Offices.EditIndex].FindControl("lblIIdent")).Text); // ID of record in table PEOPLEOFFICES
        //        switch (ddl.ID)
        //        {
        //            case "DDLPerson":
        //                if (ii> 0)
        //                {
        //                    sText = (from r in pdc.PEOPLEOFFICEs where r.ID == ii select r.PEOPLE.sDisplayName).First();
        //                }
        //                break;
        //            case "DDLBoardOffice":
        //                if (ii > 0)
        //                {
        //                    sText = (from r in pdc.PEOPLEOFFICEs where r.ID == ii select r.BOARDOFFICE.sBoardOffice).First();
        //                }
        //                break;
        //        }
        //        if (sText.Length > 0)
        //        {
        //            SetDropDownByText(ddl, sText);
        //        }
        //    }
        //}
        private void SetDropDownByText(DropDownList ddl, string sText)
        {
            ddl.ClearSelection();
            foreach (ListItem li in ddl.Items)
            {
                if (li.Text == sText)
                {
                    li.Selected = true;
                    break;
                }
            }
            ddl.SelectedItem.Text = sText;
        }

        protected void gvCMS_Offices_RowEditing(object sender, GridViewEditEventArgs e)
        {
            GridView gv = gvCMS_Offices;
            gv.EditIndex = e.NewEditIndex;
            iEdRow = gv.PageSize * gv.PageIndex + gv.EditIndex;
            List<DataRow> li = (List<DataRow>)Session["liPeopleOffices"];
            bEditExistingRow = false;
            if (iEdRow < iNgvRows - 1)
            {
                // We are editing an existing row.
                // need to get rid of the last row which would be the New row (but there is no New row)
                li.RemoveAt(li.Count - 1);
                bEditExistingRow = true;
            }
            dt_BindToGV(li.CopyToDataTable());
        }

        private void dt_BindToGV(DataTable dtu)
        {
            GridView GV = gvCMS_Offices;
            DataView view = new DataView(dtu);
            view.Sort = "i1Sort ASC, sDisplayName ASC, DOfficeBegin ASC";
            GV.DataSource = view;
            GV.DataBind();
        }

        protected void gvCMS_Offices_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                TNPV_PeopleContactsDataContext pdc = new TNPV_PeopleContactsDataContext();
                if ((e.Row.RowState & DataControlRowState.Edit) != 0)
                {
                    DropDownList DDLPerson = (DropDownList)e.Row.FindControl("DDLPerson");
                    var q0 = from p in pdc.PEOPLEs orderby p.sDisplayName select p;
                    DDLPerson.DataSource = q0;
                    DDLPerson.DataValueField = "ID";
                    DDLPerson.DataTextField = "sDisplayName";
                    DDLPerson.DataBind();

                    DropDownList DDLBoardOffice = (DropDownList)e.Row.FindControl("DDLBoardOffice");
                    var qb = from b in pdc.BOARDOFFICEs orderby b.sBoardOffice select b;
                    DDLBoardOffice.DataSource = qb;
                    DDLBoardOffice.DataValueField = "ID";
                    DDLBoardOffice.DataTextField = "sBoardOffice";
                    DDLBoardOffice.DataBind();

                    int iColAddButton = gvCMS_Offices.Columns.Count - 2; // Column of the Edit and Add buttons
                    Label lblIIdent = (Label)e.Row.FindControl("lblIIdent");
                    if (!bEditExistingRow && iIndexOfLastPage == gvCMS_Offices.PageIndex)
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
                    }
                    else
                    {
                        // Editing an existing row
                        string sT = DataBinder.Eval(e.Row.DataItem, "sDisplayName").ToString();
                        SetDropDownByText(DDLPerson, sT);
                        sT = DataBinder.Eval(e.Row.DataItem, "sBoardOffice").ToString();
                        SetDropDownByText(DDLBoardOffice, sT);
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
                }
            }
        }

        protected void gvCMS_Offices_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            Label lblItem = (Label)gvCMS_Offices.Rows[e.RowIndex].FindControl("lblIIdent");
            string sItem = lblItem.Text;
            ButtonsClear();
            YesButton.CommandName = "Delete";
            YesButton.CommandArgument = sItem;
            OkButton.CommandArgument = "PeopleOffices";
            lblPopupText.Text =
                "Please confirm deletion of PeopleOffices record with ID " + sItem;
            MPE_Show(Global.enumButtons.NoYes);
        }

        protected void gvCMS_Offices_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvCMS_Offices.EditIndex = -1;
            bEditExistingRow = false;
            iEdRow = -1;
            FillDataTable();
        }

        protected void gvCMS_Offices_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            TNPV_PeopleContactsDataContext pdc = new TNPV_PeopleContactsDataContext();
            int iLast = gvCMS_Offices.Rows.Count - 1;

            DropDownList DDLPerson = (DropDownList)gvCMS_Offices.Rows[e.RowIndex].FindControl("DDLPerson");
            int iPerson = Int32.Parse(DDLPerson.SelectedValue);
            DropDownList DDLBoardOffice = (DropDownList)gvCMS_Offices.Rows[e.RowIndex].FindControl("DDLBoardOffice");
            int iBoardOffice = Int32.Parse(DDLBoardOffice.SelectedValue);
            DateTimeOffset DOfficeBegin = DateTimeOffset.Parse(((TextBox)gvCMS_Offices.Rows[e.RowIndex].FindControl("txbDBegan")).Text);
            DateTimeOffset DOfficeEnd = DateTimeOffset.Parse(((TextBox)gvCMS_Offices.Rows[e.RowIndex].FindControl("txbDEnded")).Text);
            string sAdditionalInfo = Server.HtmlEncode(((TextBox)gvCMS_Offices.Rows[e.RowIndex].FindControl("txbNotes")).Text);

            if (e.RowIndex == iLast)
            {
                PEOPLEOFFICE po = new PEOPLEOFFICE();
                po.iPerson = iPerson;
                po.iBoardOffice = iBoardOffice;
                po.DOfficeBegin = DOfficeBegin;
                po.DOfficeEnd = DOfficeEnd;
                po.sAdditionalInfo = sAdditionalInfo;

                pdc.PEOPLEOFFICEs.InsertOnSubmit(po);
            }
            else
            {
                int iID = Int32.Parse(((Label)gvCMS_Offices.Rows[e.RowIndex].FindControl("lblIIdent")).Text);
                var po = (from v in pdc.PEOPLEOFFICEs where v.ID==iID select v).First();
                po.iPerson = iPerson;
                po.iBoardOffice = iBoardOffice;
                po.DOfficeBegin = DOfficeBegin;
                po.DOfficeEnd = DOfficeEnd;
                po.sAdditionalInfo = sAdditionalInfo;
            }
            try
            {
                pdc.SubmitChanges();
            }
            catch (Exception exc)
            {
                Global.excToPopup ex = new Global.excToPopup(exc.Message);
                ProcessPopupException(ex);
                return;
            }
            pdc.Dispose();
            gvCMS_Offices.EditIndex = -1;
            bEditExistingRow = false;
            iEdRow = -1;
            FillDataTable();
        }

        protected void gvCMS_Offices_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvCMS_Offices.PageIndex = e.NewPageIndex;
            FillDataTable();
        }
    }
}