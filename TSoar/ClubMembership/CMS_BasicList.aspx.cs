using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TSoar.DB;

namespace TSoar.ClubMembership
{
    public partial class CMS_BasicList : System.Web.UI.Page
    {
        #region Declarations
        const int icDisplayName = 7;
        const int icDateOfBirth = 9;
        SCUD_Multi mCRUD = new SCUD_Multi();
        string[] sa = new string[15];
        private string sMKey { get { return (string)ViewState["sMKey"] ?? ""; } set { ViewState["sMKey"] = value; } }

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
            //Global.oLog("Modal popup dismissed with " + btn.ID + ", CommandName=" + btn.CommandName);
            try
            {
                if ((btn.ID == "YesButton") && (btn.CommandName == "Delete"))
                {
                    switch (OkButton.CommandArgument)
                    {
                        case "Basic":
                            // Delete the Member
                            mCRUD.DeleteOne(Global.enugInfoType.Members, btn.CommandArgument);
                            DisplayInGrid(Global.enugInfoType.Members);
                            break;
                    }
                }
            }
            catch (Global.excToPopup exc)
            {
                ProcessPopupException(exc);
            }
        }
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DisplayInGrid(Global.enugInfoType.Members);
            }
        }

        private void DisplayInGrid(Global.enugInfoType euInfoType)
        {
            GridView g=null;
            string ss="";
            switch (euInfoType)
            {
                case Global.enugInfoType.Members:
                    g = gvMembers;
                    ss = "PageSizeMembers";
                    break;
                default:
                    return;
            }
            g.PageSize = Int32.Parse(mCRUD.GetSetting(ss));
            if (chbFilter.Checked)
            {
                ClubMembershipDataContext dc = new ClubMembershipDataContext();
                g.DataSource = from b in dc.PEOPLEs where b.sDisplayName.Contains(Server.HtmlEncode(txbFilter.Text)) select b;
            }
            else
            {
                g.DataSource = mCRUD.GetAll(euInfoType);
            }
            g.DataBind();
        }

        #region Add Member events
        protected void dvCMS_BasicList_ItemInserting(object sender, DetailsViewInsertEventArgs e)
        {
            Global.excToPopup exc;
            e.Cancel = true; // because we are handling our own insertion
            string sp = "Display Name must not be empty";
            sa[0] = DateTime.UtcNow.ToString();
            sa[1] = mCRUD.GetPeopleIDfromWebSiteUserName(HttpContext.Current.User.Identity.Name).ToString();
            sa[2] = Server.HtmlEncode(((TextBox)dvCMS_BasicList.FindControl("txbTitle")).Text.Trim().Replace("'", "`"));
            sa[3] = Server.HtmlEncode(((TextBox)dvCMS_BasicList.FindControl("txbFirstName")).Text.Trim().Replace("'", "`"));
            sa[4] = Server.HtmlEncode(((TextBox)dvCMS_BasicList.FindControl("txbMiddleName")).Text.Trim().Replace("'", "`"));
            sa[5] = Server.HtmlEncode(((TextBox)dvCMS_BasicList.FindControl("txbLastName")).Text.Trim().Replace("'", "`"));
            sa[6] = Server.HtmlEncode(((TextBox)dvCMS_BasicList.FindControl("txbSuffix")).Text.Trim().Replace("'", "`"));
            sa[icDisplayName] = Server.HtmlEncode(((TextBox)dvCMS_BasicList.FindControl("txbDisplayName")).Text.Trim().Replace("'", "`"));
            sa[8] = Server.HtmlEncode(((TextBox)dvCMS_BasicList.FindControl("txbNotes")).Text.Trim().Replace("'", "`"));
            sa[icDateOfBirth] = ((TextBox)dvCMS_BasicList.FindControl("txbDOB")).Text.Trim();
            sa[10] = Server.HtmlEncode(((TextBox)dvCMS_BasicList.FindControl("txbUserName")).Text.Trim().Replace("'", "`"));
            sa[11] = Server.HtmlEncode(((TextBox)dvCMS_BasicList.FindControl("txbIdQBO")).Text.Trim().Replace("'", "`"));
            sa[12] = ((TextBox)dvCMS_BasicList.FindControl("txbIBranchCode")).Text;
            sa[13] = ((TextBox)dvCMS_BasicList.FindControl("txbIDebtorNo")).Text;
            sa[14] = "1900-01-01 00:00:00.0 +00:00";
            if (sa[icDisplayName].Length < 1)
            {
                exc = new Global.excToPopup(sp);
            }
            else
            {
                if (mCRUD.Exists(Global.enugInfoType.Members, sa[icDisplayName]) > 0)
                {
                    exc = new Global.excToPopup("Member with display name `" + sa[icDisplayName] + "` already exists");
                }
                else
                {
                    if (sa[icDateOfBirth].Length > 0 && !IsDate(sa[icDateOfBirth]))
                    {
                        exc = new Global.excToPopup("Date of Birth does not have a valid date");
                    }
                    else
                    {
                        int iIdent = 0;
                        mCRUD.InsertOne(Global.enugInfoType.Members, sa, out iIdent);
                        sp = "Record Inserted: ";
                        for (int i = 2; i < sa.Count(); i++)
                        {
                            sp += ((i > 2) ? ", " : " ") + sa[i];
                        }
                        exc = new Global.excToPopup(sp); // not an exception - just using to display confirmation of insertion
                    }
                }
            }
            ProcessPopupException(exc);
            DisplayInGrid(Global.enugInfoType.Members);
        }

        protected void dvCMS_BasicList_ItemInserted(object sender, DetailsViewInsertedEventArgs e)
        {
            DisplayInGrid(Global.enugInfoType.Members);
        }
#endregion

        private void ProcessPopupException(Global.excToPopup excu)
        {
            ButtonsClear();
            lblPopupText.Text = excu.sExcMsg();
            MPE_Show(Global.enumButtons.OkOnly);
        }

        public string sDecodeWNull(object ou)
        {
            if (ou is null || ou is DBNull)
            {
                return "";
            }
            else
            {
                return Server.HtmlDecode((string)ou);
            }
        }

        public string sDecodeWNull(object ou, bool bKey)
        {
            if (ou is null || ou is DBNull)
            {
                return "";
            }
            else
            {
                sMKey = (string)ou;
                return Server.HtmlDecode(sMKey);
            }
        }

        #region Members basic list events
        protected void gvMembers_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            Label lblItem = (Label)gvMembers.Rows[e.RowIndex].FindControl("lblDName");
            string sItem = Server.HtmlEncode(lblItem.Text);
            ButtonsClear();
            YesButton.CommandName = "Delete";
            YesButton.CommandArgument = sItem;
            OkButton.CommandArgument = "Basic";
            lblPopupText.Text = "It is quite unusual to delete a member; they are usually kept on file for historical recordkeeping. " +
                "If you still want to go ahead, please confirm deletion of '" + sItem +
                "'. All records associated with this member will also be deleted - so, better don't do it!";
            MPE_Show(Global.enumButtons.NoYes);
        }

        protected void gvMembers_RowEditing(object sender, GridViewEditEventArgs e)
        {
            gvMembers.EditIndex = e.NewEditIndex;
            DisplayInGrid(Global.enugInfoType.Members);
        }

        protected void gvMembers_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvMembers.EditIndex = -1;
            DisplayInGrid(Global.enugInfoType.Members);
        }

        protected void gvMembers_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            GridViewRow row = gvMembers.Rows[e.RowIndex];
            TextBox txb;
            string[] sa = new string[15];
            sa[0] = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.mmm");
            sa[1] = mCRUD.GetPeopleIDfromWebSiteUserName(HttpContext.Current.User.Identity.Name).ToString();
            txb = (TextBox)row.FindControl("txbUTitle");
            sa[2] = Server.HtmlEncode(txb.Text.Trim().Replace("'", "`"));
            txb = (TextBox)row.FindControl("txbFName");
            sa[3] = Server.HtmlEncode(txb.Text.Trim().Replace("'", "`"));
            txb = (TextBox)row.FindControl("txbMName");
            sa[4] = Server.HtmlEncode(txb.Text.Trim().Replace("'", "`"));
            txb = (TextBox)row.FindControl("txbLName");
            sa[5] = Server.HtmlEncode(txb.Text.Trim().Replace("'", "`"));
            txb = (TextBox)row.FindControl("txbUSuffix");
            sa[6] = Server.HtmlEncode(txb.Text.Trim().Replace("'", "`"));
            txb = (TextBox)row.FindControl("txbDName");
            sa[icDisplayName] = Server.HtmlEncode(txb.Text.Trim().Replace("'", "`"));
            if (sa[icDisplayName].Length < 1)
            {
                Global.excToPopup exc = new Global.excToPopup("Display Name must not be empty");
                ProcessPopupException(exc);
                return;
            }
            if (sa[icDisplayName] != sMKey)
            {
                Global.excToPopup exc;
                if (mCRUD.Exists(Global.enugInfoType.Members, sa[icDisplayName]) > 0)
                {
                    exc = new Global.excToPopup("A Member with display name `" + sa[icDisplayName] + "` already exists");
                    ProcessPopupException(exc);
                    return;
                }
            }
            txb = (TextBox)row.FindControl("txbNotes");
            sa[8] = Server.HtmlEncode(txb.Text.Trim().Replace("'", "`"));
            txb = (TextBox)row.FindControl("txbDOB");
            sa[icDateOfBirth] = txb.Text.Trim();
            if (sa[icDateOfBirth].Length > 0 && !IsDate(sa[icDateOfBirth]))
            {
                Global.excToPopup exc = new Global.excToPopup("Date of Birth does not have a valid date");
                ProcessPopupException(exc);
                return;
            }
            txb = (TextBox)row.FindControl("txbUserName");
            sa[10] = Server.HtmlEncode(txb.Text.Trim().Replace("'", "`"));
            txb = (TextBox)row.FindControl("txbIdQBO");
            sa[11] = Server.HtmlEncode(txb.Text.Trim().Replace("'", "`"));
            txb = (TextBox)row.FindControl("txbIFAbranchcode");
            sa[12] = txb.Text;
            txb = (TextBox)row.FindControl("txbIFADebtorNo");
            sa[13] = txb.Text;
            Label lblDAcceptedAUP = (Label)row.FindControl("lblDAcceptedAUP");
            sa[14] = lblDAcceptedAUP.Text;
            //for (int i = 2; i < sa.Count(); i++)
            //{
            //    if (sa[i] != null)
            //    {
            //        sa[i] = Server.HtmlEncode(sa[i].ToString());
            //    }
            //}
            try
            {
                mCRUD.UpdateOne(Global.enugInfoType.Members, sMKey, sa);
            }
            catch (Global.excToPopup exc)
            {
                ProcessPopupException(exc);
            }
            finally
            {
                gvMembers.EditIndex = -1;
                DisplayInGrid(Global.enugInfoType.Members);
            }
        }

        protected void gvMembers_RowCreated(object sender, GridViewRowEventArgs e)
        {
            foreach (TableCell cell in e.Row.Cells) { cell.CssClass = "cell-padding"; }
        }

        public static bool IsDate(Object obj)
        {
            string strDate = obj.ToString();
            try
            {
                DateTime dt = DateTime.Parse(strDate);
                if (dt != DateTime.MinValue && dt != DateTime.MaxValue)
                {
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        protected void gvMembers_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvMembers.PageIndex = e.NewPageIndex;
            DisplayInGrid(Global.enugInfoType.Members);
        }
        #endregion

        protected void pbFilter_Click(object sender, EventArgs e)
        {
            DisplayInGrid(Global.enugInfoType.Members);
        }

        protected void gvMembers_RowDataBound(object sender, GridViewRowEventArgs e)
        {

        }

        protected void dvCMS_BasicList_ModeChanging(object sender, DetailsViewModeEventArgs e)
        {

        }
    }
}