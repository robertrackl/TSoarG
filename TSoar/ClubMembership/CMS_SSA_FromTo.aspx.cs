using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TSoar.DB;

namespace TSoar.ClubMembership
{
    public partial class CMS_SSA_FromTo : System.Web.UI.Page
    {
        private string sKey { get { return (string)ViewState["sKey"] ?? ""; } set { ViewState["sKey"] = value; } }
        private string sMember { get { return (string)ViewState["sMember"] ?? ""; } set { ViewState["sMember"] = value; } }
        private string[] sa { get { return (string[])ViewState["sa"] ?? new string[1] { "" }; } set { ViewState["sa"] = value; } }

        SCUD_Multi mCRUD = new SCUD_Multi();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DisplayInGrid();
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
                        // Delete the Membership FromTo record
                        try
                        {
                            mCRUD.DeleteOne(Global.enugInfoType.SSA_FromTo, btn.CommandArgument);
                        }
                        catch (Global.excToPopup exc)
                        {
                            ProcessPopupException(exc);
                        }
                        DisplayInGrid();
                        break;
                    case "Update":
                        FromToUpdate();
                        break;
                }
            }
        }
        #endregion

        private void ProcessPopupException(Global.excToPopup excu)
        {
            ButtonsClear();
            lblPopupText.Text = excu.sExcMsg();
            MPE_Show(Global.enumButtons.OkOnly);
        }

        protected void dvCMS_MbrFromTo_PreRender(object sender, EventArgs e)
        {
            string sOffset = mCRUD.GetSetting("TimeZoneOffset");
            char[] ca = new char[] { ':' };
            string[] sa = sOffset.Split(ca);
            TimeSpan Tint = new TimeSpan(Int32.Parse(sa[0]), Int32.Parse(sa[1]), 0);
            DateTimeOffset DToday = DateTimeOffset.Now;

            DateTimeOffset D1NextMonth = new DateTimeOffset(DToday.Year, DToday.Month, 1, 1, 1, 0, Tint).AddMonths(1); // First Day of next month, one hour and 1 minute after midnight
            TextBox txb_MBegan = (TextBox)dvCMS_MbrFromTo.FindControl("txb_MBegan");
            txb_MBegan.Text = TSoar.CustFmt.sFmtDate(D1NextMonth, CustFmt.enDFmt.DateOnly).Replace("/","-");

            TextBox txb_MEnd = (TextBox)dvCMS_MbrFromTo.FindControl("txb_MEnd");
            txb_MEnd.Text = "2999-12-31";

            TextBox txb_MExpiry= (TextBox)dvCMS_MbrFromTo.FindControl("txb_MExpiry");
            int iYear = DToday.Year;
            txb_MExpiry.Text = iYear.ToString("D4") + "-12-31";
        }

        private void DisplayInGrid()
        {
            gvCMS_MbrFromTo.PageSize = Int32.Parse(mCRUD.GetSetting("PageSizeMembersFromTo"));
            try
            {
                gvCMS_MbrFromTo.DataSource = mCRUD.GetAll(Global.enugInfoType.SSA_FromTo);
            }catch (Global.excToPopup pexc)
            {
                ProcessPopupException(pexc);
            }
            gvCMS_MbrFromTo.DataBind();
        }

        protected void dvCMS_MbrFromTo_ItemInserting(object sender, DetailsViewInsertEventArgs e)
        {
            string sOffset = mCRUD.GetSetting("TimeZoneOffset");
            sa = new string[11];
            sa[0] = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.mmm");
            sa[1] = mCRUD.GetPeopleIDfromWebSiteUserName(HttpContext.Current.User.Identity.Name).ToString();
            sa[2] = ((DropDownList)dvCMS_MbrFromTo.FindControl("DDL_Member")).SelectedValue.ToString();
            sa[3] = ((TextBox)dvCMS_MbrFromTo.FindControl("txb_SSA_ID")).Text;
            sa[4] = ((DropDownList)dvCMS_MbrFromTo.FindControl("DDL_MCat")).SelectedValue.ToString();
            sa[5] = ((TextBox)dvCMS_MbrFromTo.FindControl("txb_MBegan")).Text + " 01:01:00 " + sOffset;
            string st = ((TextBox)dvCMS_MbrFromTo.FindControl("txb_MEnd")).Text;
            if (st.Length > 0) st += " 22:59:00 " + sOffset;
            sa[6] = st;
            sa[7] = ((TextBox)dvCMS_MbrFromTo.FindControl("txb_MExpiry")).Text + " 22:59:00 " + sOffset;
            sa[8] = ((CheckBox)dvCMS_MbrFromTo.FindControl("chbRenewsWChapter")).Checked ? "1" : "0";
            sa[9] = Server.HtmlEncode(((TextBox)dvCMS_MbrFromTo.FindControl("txb_ChapterAffil")).Text);
            sa[10] = Server.HtmlEncode(((TextBox)dvCMS_MbrFromTo.FindControl("txb_Notes")).Text);
            if (bValidate_sa())
            {
                int iIdent = 0;
                mCRUD.InsertOne(Global.enugInfoType.SSA_FromTo, sa, out iIdent);
                string sp = "Record Inserted: ";
                sp += ((DropDownList)dvCMS_MbrFromTo.FindControl("DDL_Member")).SelectedItem;
                sp += ", " + sa[3];
                sp += ", " + ((DropDownList)dvCMS_MbrFromTo.FindControl("DDL_MCat")).SelectedItem;
                for (int i = 5; i < sa.Count(); i++)
                {
                    sp += ", " + sa[i];
                }
                Global.excToPopup exc = new Global.excToPopup(sp); // not an exception - just using to display confirmation of insertion
                ProcessPopupException(exc);
                e.Cancel = true; // because we are handling our own insertion
                DisplayInGrid();
            }
        }

        protected void DDL_MCat_PreRender(object sender, EventArgs e)
        {
            SetDropDownByValue((DropDownList)sender, mCRUD.GetSetting("DefaultSSAMembershipType"));
        }

        protected void gvCMS_MbrFromTo_RowCreated(object sender, GridViewRowEventArgs e)
        {
            foreach (TableCell cell in e.Row.Cells) { cell.CssClass = "cell-padding"; }
        }

        protected void gvCMS_MbrFromTo_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            ButtonsClear();
            OkButton.CommandArgument = "MemberFromTo";
            YesButton.CommandName = "Delete";
            YesButton.CommandArgument = e.Values[0].ToString();
            lblPopupText.Text = "Please confirm deletion of SSA membership record with row identifier " + YesButton.CommandArgument;
            MPE_Show(Global.enumButtons.NoYes);
            e.Cancel = true;
        }

        protected void gvCMS_MbrFromTo_RowEditing(object sender, GridViewEditEventArgs e)
        {
            gvCMS_MbrFromTo.EditIndex = e.NewEditIndex;
            DisplayInGrid();
        }

        protected void gvCMS_MbrFromTo_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if ((e.Row.RowState & DataControlRowState.Edit) > 0)
                {
                    sKey = e.Row.Cells[0].Text;
                    DropDownList ddlMembers = (DropDownList)e.Row.FindControl("DDLMember");
                    sMember = DataBinder.Eval(e.Row.DataItem, "sDisplayName").ToString();
                    SetDropDownByValue(ddlMembers, sMember);
                    int iSSA_ID = (int)DataBinder.Eval(e.Row.DataItem, "iSSA_ID");
                    TextBox txbUSSA_ID = (TextBox)e.Row.FindControl("txbUSSA_ID");
                    txbUSSA_ID.Text = iSSA_ID.ToString();
                    DropDownList ddlMbCat = (DropDownList)e.Row.FindControl("DDLMbCat");
                    string sD = DataBinder.Eval(e.Row.DataItem, "sSSA_MemberCategory").ToString();
                    SetDropDownByValue(ddlMbCat, sD);

                    TextBox txbB = (TextBox)e.Row.FindControl("txbDBegin");
                    DateTimeOffset DD = (DateTimeOffset)DataBinder.Eval(e.Row.DataItem, "Date_Began");
                    txbB.Text = TSoar.CustFmt.sFmtDate(DD, CustFmt.enDFmt.DateOnly).Replace("/","-");

                    TextBox txbE = (TextBox)e.Row.FindControl("txbDEnd");
                    txbE.Text = TSoar.CustFmt.sFmtDate((object)DataBinder.Eval(e.Row.DataItem, "Date_End"), CustFmt.enDFmt.DateOnly).Replace("/", "-");

                    TextBox txbX = (TextBox)e.Row.FindControl("txbDExpiry");
                    DD = (DateTimeOffset)DataBinder.Eval(e.Row.DataItem, "Date_Expiry");
                    txbX.Text = TSoar.CustFmt.sFmtDate(DD, CustFmt.enDFmt.DateOnly).Replace("/", "-");
                }
            }
        }
        public void SetDropDownByValue(DropDownList ddl, string sText)
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

        protected void gvCMS_MbrFromTo_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvCMS_MbrFromTo.EditIndex = -1;
            DisplayInGrid();
        }

        protected void gvCMS_MbrFromTo_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            string sOffset = mCRUD.GetSetting("TimeZoneOffset");
            sa = new string[11];
            e.Cancel = true; // because we are handling updates right here
            GridViewRow row = gvCMS_MbrFromTo.Rows[e.RowIndex];
            sa[0] = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.mmm");
            sa[1] = mCRUD.GetPeopleIDfromWebSiteUserName(HttpContext.Current.User.Identity.Name).ToString();
            DropDownList ddlMembers = (DropDownList)row.FindControl("DDLMember");
            sa[2] = ddlMembers.Items[ddlMembers.SelectedIndex].Value;
            sa[3] = ((TextBox)row.FindControl("txbUSSA_ID")).Text;
            DropDownList ddlMbCat = (DropDownList)row.FindControl("DDLMbCat");
            sa[4] = ddlMbCat.Items[ddlMbCat.SelectedIndex].Value;
            sa[5] = ((TextBox)row.FindControl("txbDBegin")).Text + " 01:01:00 " + sOffset;
            string st = ((TextBox)row.FindControl("txbDEnd")).Text;
            if (st.Length > 0) st += " 22:59:00 " + sOffset;
            sa[6] = st;
            sa[7] = ((TextBox)row.FindControl("txbDExpiry")).Text + " 22:59:00 " + sOffset;
            sa[8] = ((CheckBox)row.FindControl("chbURenewsWChapter")).Checked ? "1" : "0";
            sa[9] = Server.HtmlEncode(((TextBox)row.FindControl("txbUChapterAffil")).Text);
            sa[10] = Server.HtmlEncode(((TextBox)row.FindControl("txbUNotes")).Text);
            if (bValidate_sa())
            {
                if (ddlMembers.SelectedItem.Text != sMember)
                {
                    ButtonsClear();
                    YesButton.CommandName = "Update";
                    lblPopupText.Text = "It is unusual to change the member for an SSA membership entry; please confirm that you want to do that:";
                    MPE_Show(Global.enumButtons.NoYes);
                }
                else
                {
                    FromToUpdate();
                }
            }
        }
        private bool bValidate_sa() { 
            DateTimeOffset DBeg = new DateTimeOffset();
            if (!DateTimeOffset.TryParse(sa[5], out DBeg))
            {
                lblPopupText.Text = "Begin date string '" + sa[5] + "' is not in proper date format";
                MPE_Show(Global.enumButtons.OkOnly);
                return false;
            }
            DateTimeOffset DEnd = new DateTimeOffset();
            if (sa[6].Length > 0)
            {
                if (!DateTimeOffset.TryParse(sa[6], out DEnd))
                {
                    lblPopupText.Text = "End date string '" + sa[6] + "' is not in proper date format";
                    MPE_Show(Global.enumButtons.OkOnly);
                    return false;
                }
            }
            else
            {
                DEnd = DateTimeOffset.MaxValue;
            }
            if (DEnd < DBeg)
            {
                lblPopupText.Text = "End date must be greater or equal to begin date";
                MPE_Show(Global.enumButtons.OkOnly);
                return false;
            }
            DateTimeOffset DExpiry = new DateTimeOffset();
            if (sa[7].Length > 0)
            {
                if (!DateTimeOffset.TryParse(sa[7], out DExpiry))
                {
                    lblPopupText.Text = "Expiry date string '" + sa[7] + "' is not in proper date format";
                    MPE_Show(Global.enumButtons.OkOnly);
                    return false;
                }
            }
            else
            {
                lblPopupText.Text = "SSA membership expiry date must be specified";
                MPE_Show(Global.enumButtons.OkOnly);
                return false;
            }
            if (DExpiry < DBeg)
            {
                lblPopupText.Text = "Expiry date must be greater or equal to begin date";
                MPE_Show(Global.enumButtons.OkOnly);
                return false;
            }
            return true;
        }

        private void FromToUpdate()
        {
            try
            {
                mCRUD.UpdateOne(Global.enugInfoType.SSA_FromTo, sKey, sa);
                gvCMS_MbrFromTo.EditIndex = -1;
            }
            catch (Global.excToPopup exc)
            {
                ProcessPopupException(exc);
            }
            DisplayInGrid();
        }

        protected void gvCMS_MbrFromTo_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvCMS_MbrFromTo.PageIndex = e.NewPageIndex;
            DisplayInGrid();
        }
    }
}
