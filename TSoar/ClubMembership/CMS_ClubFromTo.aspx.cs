using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using TSoar.DB;

namespace TSoar.ClubMembership
{
    public partial class CMS_ClubFromTo : System.Web.UI.Page
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
                            mCRUD.DeleteOne(Global.enugInfoType.PeopleFromTo, btn.CommandArgument);
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

        private void ProcessPopupException(Global.excToPopup excu)
        {
            ButtonsClear();
            lblPopupText.Text = excu.sExcMsg();
            MPE_Show(Global.enumButtons.OkOnly);
        }
        #endregion

        private void DisplayInGrid()
        {
            gvCMS_MbrFromTo.PageSize = Int32.Parse(mCRUD.GetSetting("PageSizeMembersFromTo"));
            gvCMS_MbrFromTo.DataSource = mCRUD.GetAll(Global.enugInfoType.PeopleFromTo);
            gvCMS_MbrFromTo.DataBind();
        }

        protected void dvCMS_MbrFromTo_ItemInserting(object sender, DetailsViewInsertEventArgs e)
        {
            sa = new string[5];
            sa[0] = ((DropDownList)dvCMS_MbrFromTo.FindControl("DDL_Member")).SelectedValue.ToString();
            sa[1] = ((DropDownList)dvCMS_MbrFromTo.FindControl("DDL_MCat")).SelectedValue.ToString();
            // In Sql Server, these dates are DateTimeOffset(0)
            string sOffset = mCRUD.GetSetting("TimeZoneOffset");
            sa[2] = ((TextBox)dvCMS_MbrFromTo.FindControl("txb_MBegan")).Text ;
            if (sa[2].Trim().Length < 1)
            {
                ProcessPopupException(new Global.excToPopup("It looks like you did not enter a begin date"));
                return;
            }
            sa[2] += " 01:01:00 " + sOffset;
            DateTimeOffset Dtest2 = DateTimeOffset.MinValue;
            if (!DateTimeOffset.TryParse(sa[2], out Dtest2))
            {
                ProcessPopupException(new Global.excToPopup("The string `" + sa[2] + "` could not be recognized as date of when membership began"));
                return;
            }
            if (Dtest2 < DateTimeOffset.Parse("1901/01/01"))
            {
                ProcessPopupException(new Global.excToPopup("The string `" + sa[2] + "` is too early as a date when membership began. Must 1901 Jan 1 or later."));
                return;
            }
            sa[3] = ((TextBox)dvCMS_MbrFromTo.FindControl("txb_MEnd")).Text;
            if (sa[3].Trim().Length < 1)
            {
                ProcessPopupException(new Global.excToPopup("It looks like you did not enter an end date"));
                return;
            }
            sa[3] += " 22:59:00 " + sOffset;
            DateTimeOffset Dtest3 = DateTimeOffset.MinValue;
            if (!DateTimeOffset.TryParse(sa[3], out Dtest3))
            {
                ProcessPopupException(new Global.excToPopup("The string `" + sa[3] + "` could not be recognized as date of when membership ended"));
                return;
            }
            if (Dtest2 >= Dtest3)
            {
                ProcessPopupException(new Global.excToPopup("Date of when membership began (" + sa[2] + 
                    ") has to be earlier than date when membership ended ("+ sa[3] + ")"));
                return;
            }
            sa[4] = Server.HtmlEncode(((TextBox)dvCMS_MbrFromTo.FindControl("txb_Notes")).Text);
            int iIdent = 0;
            mCRUD.InsertOne(Global.enugInfoType.PeopleFromTo, sa, out iIdent);
            string sp = "Record Inserted: ";
            sp += ((DropDownList)dvCMS_MbrFromTo.FindControl("DDL_Member")).SelectedItem;
            sp += ", " + ((DropDownList)dvCMS_MbrFromTo.FindControl("DDL_MCat")).SelectedItem;
            for (int i = 2; i < sa.Count(); i++)
            {
                sp += ", " + sa[i];
            }
            Global.excToPopup exc = new Global.excToPopup(sp); // not an exception - just using to display confirmation of insertion
            ProcessPopupException(exc);
            e.Cancel = true; // because we are handling our own insertion
            DisplayInGrid();
        }

        protected void DDL_MCat_PreRender(object sender, EventArgs e)
        {
            SetDropDownByValue((DropDownList)sender, mCRUD.GetSetting("DefaultClubMembershipType"));
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
            lblPopupText.Text = "Please confirm deletion of club membership record with row identifier " + YesButton.CommandArgument;
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
                    sMember = DataBinder.Eval(e.Row.DataItem, "Display_Name").ToString();
                    SetDropDownByValue(ddlMembers, sMember);
                    DropDownList ddlMbCat = (DropDownList)e.Row.FindControl("DDLMbCat");
                    string sD = DataBinder.Eval(e.Row.DataItem, "Membership_Category").ToString();
                    SetDropDownByValue(ddlMbCat, sD);
                    TextBox txbB = (TextBox)e.Row.FindControl("txbDBegin");
                    sD = DataBinder.Eval(e.Row.DataItem, "Date_Began").ToString();
                    if (sD.Length > 0)
                    {
                        DateTime DD = DateTime.Parse(sD);
                        txbB.Text = DD.ToString("yyyy-MM-dd");
                    }
                    else
                    {
                        txbB.Text = sD;
                    }
                    TextBox txbE = (TextBox)e.Row.FindControl("txbDEnd");
                    sD = DataBinder.Eval(e.Row.DataItem, "Date_Ended").ToString();
                    if (sD.Length > 0)
                    {
                        DateTime DD = DateTime.Parse(sD);
                        txbE.Text = DD.ToString("yyyy-MM-dd");
                    }
                    else
                    {
                        txbE.Text = sD;
                    }
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
            sa = new string[5];
            e.Cancel = true;
            GridViewRow row = gvCMS_MbrFromTo.Rows[e.RowIndex];
            DropDownList ddlMembers= (DropDownList)row.FindControl("DDLMember");
            sa[0] = ddlMembers.Items[ddlMembers.SelectedIndex].Value;
            DropDownList ddlMbCat = (DropDownList)row.FindControl("DDLMbCat");
            sa[1] = ddlMbCat.Items[ddlMbCat.SelectedIndex].Value;
            string sOffset = mCRUD.GetSetting("TimeZoneOffset");
            sa[2] = ((TextBox)row.FindControl("txbDBegin")).Text;
            if (sa[2].Trim().Length < 1)
            {
                ProcessPopupException(new Global.excToPopup("It looks like you did not enter a begin date"));
                return;
            }
            sa[2] += " 01:01:00 " + sOffset;
            DateTimeOffset Dtest2 = DateTimeOffset.MinValue;
            if (!DateTimeOffset.TryParse(sa[2], out Dtest2))
            {
                ProcessPopupException(new Global.excToPopup("The string `" + sa[2] + "` could not be recognized as date of when membership began"));
                return;
            }
            if (Dtest2 < DateTimeOffset.Parse("1901/01/01"))
            {
                ProcessPopupException(new Global.excToPopup("The string `" + sa[2] + "` is too early as a date when membership began. Must 1901 Jan 1 or later."));
                return;
            }
            sa[3] = ((TextBox)row.FindControl("txbDEnd")).Text;
            if (sa[3].Trim().Length < 1)
            {
                ProcessPopupException(new Global.excToPopup("It looks like you did not enter an end date"));
                return;
            }
            sa[3] += " 22:59:00 " + sOffset;
            DateTimeOffset Dtest3 = DateTimeOffset.MinValue;
            if (!DateTimeOffset.TryParse(sa[3], out Dtest3))
            {
                ProcessPopupException(new Global.excToPopup("The string `" + sa[3] + "` could not be recognized as date of when membership ended"));
                return;
            }
            DateTimeOffset DBeg = new DateTimeOffset();
            if (!DateTimeOffset.TryParse(sa[2], out DBeg))
            {
                lblPopupText.Text = "Begin date string '" + sa[2] + "' is not in proper date format";
                MPE_Show(Global.enumButtons.OkOnly);
                return;
            }
            DateTimeOffset DEnd = new DateTimeOffset();
            if (sa[3].Length > 0)
            {
                if (!DateTimeOffset.TryParse(sa[3], out DEnd))
                {
                    lblPopupText.Text = "End date string '" + sa[3] + "' is not in proper date format";
                    MPE_Show(Global.enumButtons.OkOnly);
                    return;
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
                return;
            }
            sa[4] = Server.HtmlEncode(((TextBox)row.FindControl("txbNotes")).Text);
            if (ddlMembers.SelectedItem.Text != sMember)
            {
                ButtonsClear();
                YesButton.CommandName = "Update";
                lblPopupText.Text = "It is quite unusual to change the member for a club membership entry; please confirm that you want to do that:";
                MPE_Show(Global.enumButtons.NoYes);
            }
            else
            {
                FromToUpdate();
            }
        }

        private void FromToUpdate()
        {
            try
            {
                mCRUD.UpdateOne(Global.enugInfoType.PeopleFromTo, sKey, sa);
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

        protected void dvCMS_MbrFromTo_ModeChanging(object sender, DetailsViewModeEventArgs e)
        {

        }
    }
}