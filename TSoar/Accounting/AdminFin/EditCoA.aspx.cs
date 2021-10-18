using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;
using TSoar.DB;

namespace TSoar.Accounting
{
    public partial class EditCoA : System.Web.UI.Page
    {
        const int icAccountCode = 2;
        const int icSortCode = 3;
        const int icAccountName = 4;
        string[] sa = new string[9];
        SCUD_Multi mCRUD = new SCUD_Multi();
        private string sMKey { get { return (string)ViewState["sMKey"] ?? ""; } set { ViewState["sMKey"] = value; } }

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
                        case "Account":
                            // Delete the Account
                            mCRUD.DeleteOne(Global.enugInfoType.SF_Accounts, btn.CommandArgument);
                            DisplayInGrid(Global.enugInfoType.SF_Accounts);
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
                DisplayInGrid(Global.enugInfoType.SF_Accounts);
            }
        }

        private void DisplayInGrid(Global.enugInfoType euInfoType)
        {
            GridView g = null;
            string ss = "";
            switch (euInfoType)
            {
                case Global.enugInfoType.SF_Accounts:
                    g = gvCoA;
                    ss = "PageSizeCoA";
                    break;
                default:
                    return;
            }
            g.PageSize = Int32.Parse(mCRUD.GetSetting(ss));
            g.DataSource = mCRUD.GetAll(euInfoType);
            g.DataBind();
        }
        #region Add Account section
        protected void dvCoA_ItemInserting(object sender, DetailsViewInsertEventArgs e)
        {
            Global.excToPopup exc = null;
            e.Cancel = true; // because we are handling our own insertion
            sa[0] = DateTime.UtcNow.ToString();
            sa[1] = mCRUD.GetPeopleIDfromWebSiteUserName(HttpContext.Current.User.Identity.Name).ToString();
            sa[icAccountCode] = Server.HtmlEncode(((TextBox)dvCoA.FindControl("txbCode")).Text.Trim().Replace("'", "`"));
            sa[icSortCode] = Server.HtmlEncode(((TextBox)dvCoA.FindControl("txbSortCode")).Text.Trim().Replace("'", "`"));
            sa[icAccountName] = Server.HtmlEncode(((TextBox)dvCoA.FindControl("txbName")).Text.Trim().Replace("'", "`"));
            sa[5] = ((DropDownList)dvCoA.FindControl("DDL_Type")).SelectedValue.ToString();
            sa[6] = ((DropDownList)dvCoA.FindControl("DDL_Parent")).SelectedValue.ToString();
            sa[7] = ((DropDownList)dvCoA.FindControl("DDL_Subledger")).SelectedValue.ToString();
            sa[8] = Server.HtmlEncode(((TextBox)dvCoA.FindControl("txbNotes")).Text.Trim().Replace("'", "`"));
            string sVal = sValid(sa, "");
            if (sVal == "OK")
            {
                int iIdent = 0;
                mCRUD.InsertOne(Global.enugInfoType.SF_Accounts, sa, out iIdent);
                sVal = "Record Inserted: ";
                for (int i = 2; i < sa.Count(); i++)
                {
                    sVal += ((i > 2) ? ", " : " ") + sa[i];
                }
            }
            exc = new Global.excToPopup(sVal);
            ProcessPopupException(exc);
            DisplayInGrid(Global.enugInfoType.SF_Accounts);
        }

        private string sValid(string[] sau, string suCode)
        {
            string sRet = "OK";
            bool bContinue = true;
            string sp = " must not be empty";
            string[] asp = new string[] { "", "", "Account Code", "Sort Code", "Account Name" };
            for (int iFld = icAccountCode; iFld <= icAccountName; iFld++)
            {
                if (sau[iFld].Length < 1)
                {
                    sRet = asp[iFld] + sp;
                    bContinue = false;
                    break;
                }
            }
            if (bContinue)
            {
                if (sau[icAccountName].Length > 128)
                {
                    sRet = "Account Name too long; it is limited to 128 characters";
                    bContinue = false;
                }
            }
            if (bContinue)
            {
                if (sau[icAccountCode].Length > 25)
                {
                    sRet = "Account Code too long; it is limited to 25 characters";
                    bContinue = false;
                }
            }
            if (bContinue)
            {
                if (sau[icSortCode].Length > 40)
                {
                    sRet = "Sort Code too long; it is limited to 40 characters";
                    bContinue = false;
                }
            }
            if (bContinue)
            {   // When we add a new account, suCode is always "", i.e., we always check for existence;
                // when we update an existing account, we only check for existence if the account code differs:
                if (sau[icAccountCode] != suCode)
                {
                    if (mCRUD.Exists(Global.enugInfoType.SF_Accounts, sau[icAccountCode]) > 0)
                    {
                        sRet = "Account with Code `" + sau[icAccountCode] + "` already exists";
                        bContinue = false;
                    }
                }
            }
            if (bContinue)
            {
                // Account code must look like 1.11.3
                Match m = Regex.Match(sau[icAccountCode], @"^\d{1,2}(\.\d{1,2})*$");
                if (!m.Success)
                {
                    sRet = "Account Code `" + sau[icAccountCode] + "` must follow a pattern somewhat like 9.99.9 ...";
                    bContinue = false;
                }
            }
            if (bContinue)
            {
                // Sort code must look like 01.11.03
                Match m = Regex.Match(sau[icSortCode], @"^\d{2}(\.\d{2})*$");
                if (!m.Success)
                {
                    sRet = "Sort Code `" + sau[icSortCode] + "` must follow a pattern somewhat like 09.99.09 ...";
                    bContinue = false;
                }
            }
            return sRet;
        }

        protected void txbCode_TextChanged(object sender, EventArgs e)
        {
            string sCode = Server.HtmlEncode(((TextBox)sender).Text);
            string[] saCode = sCode.Split('.');
            string sSortCode = "";
            foreach(string s in saCode)
            {
                sSortCode += (100 + Int32.Parse(s)).ToString().Substring(1) + ".";
            }
            sSortCode = sSortCode.Substring(0, sSortCode.Length - 1);
            ((TextBox)dvCoA.FindControl("txbSortCode")).Text = sSortCode;
        }

        protected void dvCoA_ItemInserted(object sender, DetailsViewInsertedEventArgs e)
        {
            Response.Redirect(Request.RawUrl);
            DisplayInGrid(Global.enugInfoType.SF_Accounts);
        }
        #endregion

        #region Account List Events
        protected void gvCoA_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            
            Label lblItem = (Label)gvCoA.Rows[e.RowIndex].FindControl("lblSubledger");
            if (lblItem.Text != "(none)")
            {
                lblPopupText.Text = "This account cannot be deleted because subledger '" + lblItem.Text + "' is associated with it";
                MPE_Show(Global.enumButtons.OkOnly);
                return;
            }
            lblItem = (Label)gvCoA.Rows[e.RowIndex].FindControl("lblCode");
            string sItem = lblItem.Text;
            ButtonsClear();
            YesButton.CommandName = "Delete";
            YesButton.CommandArgument = sItem;
            OkButton.CommandArgument = "Account";
            lblPopupText.Text = "It is quite unusual to delete an account; they are usually kept on file for historical recordkeeping. " +
                "If you still want to go ahead, please confirm deletion of the account with code '" + sItem + "' (All entries and transactions associated with this account will also be deleted.)";
            MPE_Show(Global.enumButtons.NoYes);
        }

        protected void gvCoA_RowEditing(object sender, GridViewEditEventArgs e)
        {
            sMKey = ((Label)gvCoA.Rows[e.NewEditIndex].FindControl("lblCode")).Text;
            gvCoA.EditIndex = e.NewEditIndex;
            DisplayInGrid(Global.enugInfoType.SF_Accounts);
        }

        protected void gvCoA_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvCoA.EditIndex = -1;
            DisplayInGrid(Global.enugInfoType.SF_Accounts);
        }

        protected void gvCoA_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            Global.excToPopup exc = null;
            GridViewRow row = gvCoA.Rows[e.RowIndex];
            TextBox txb;
            sa[0] = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.mmm");
            sa[1] = mCRUD.GetPeopleIDfromWebSiteUserName(HttpContext.Current.User.Identity.Name).ToString();
            txb = (TextBox)row.FindControl("txbUCode");
            sa[icAccountCode] = Server.HtmlEncode(txb.Text.Trim().Replace("'", "`"));
            txb = (TextBox)row.FindControl("txbUSortCode");
            sa[icSortCode] = Server.HtmlEncode(txb.Text.Trim().Replace("'", "`"));
            txb = (TextBox)row.FindControl("txbUName");
            sa[icAccountName] = Server.HtmlEncode(txb.Text.Trim().Replace("'", "`"));
            DropDownList ddlType = (DropDownList)row.FindControl("DDLType");
            sa[5] = ddlType.Items[ddlType.SelectedIndex].Value;
            DropDownList ddlParent = (DropDownList)row.FindControl("DDLParent");
            sa[6] = ddlParent.Items[ddlParent.SelectedIndex].Value;
            DropDownList ddlSubledger = (DropDownList)row.FindControl("DDLSubledger");
            sa[7] = ddlSubledger.Items[ddlSubledger.SelectedIndex].Value;
            txb = (TextBox)row.FindControl("txbUNotes");
            sa[8] = Server.HtmlEncode(txb.Text.Trim().Replace("'", "`"));
            string sVal = sValid(sa,sMKey);
            if (sVal == "OK")
            {
                try
                {
                    mCRUD.UpdateOne(Global.enugInfoType.SF_Accounts, sMKey, sa);
                    gvCoA.EditIndex = -1;
                }
                catch (Global.excToPopup exc1)
                {
                    ProcessPopupException(exc1);
                }
            }
            else
            {
                exc = new Global.excToPopup(sVal);
                ProcessPopupException(exc);
            }
            DisplayInGrid(Global.enugInfoType.SF_Accounts);
        }

        protected void gvCoA_RowCreated(object sender, GridViewRowEventArgs e)
        {
            foreach (TableCell cell in e.Row.Cells) { cell.CssClass = "cell-padding"; }
        }

        protected void gvCoA_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvCoA.PageIndex = e.NewPageIndex;
            DisplayInGrid(Global.enugInfoType.SF_Accounts);
        }

        #endregion

        protected void DDL_Type_PreRender(object sender, EventArgs e)
        {
            SetDropDownByValue((DropDownList)sender, mCRUD.GetSetting("DefaultAccountType"));
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

        protected void gvCoA_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if ((e.Row.RowState & DataControlRowState.Edit) > 0)
                {
                    DropDownList ddlType = (DropDownList)e.Row.FindControl("DDLType");
                    string sD = DataBinder.Eval(e.Row.DataItem, "Account_Type").ToString();
                    SetDropDownByValue(ddlType, sD);
                    DropDownList ddlParent = (DropDownList)e.Row.FindControl("DDLParent");
                    sD = DataBinder.Eval(e.Row.DataItem, "Parent_Account_Code").ToString();
                    SetDropDownByValue(ddlParent, sD);
                    DropDownList ddlSubledger = (DropDownList)e.Row.FindControl("DDLSubledger");
                    sD = DataBinder.Eval(e.Row.DataItem, "Subledger_Name").ToString();
                    SetDropDownByValue(ddlSubledger, sD);
                }
            }
        }

        protected void DDL_Parent_PreRender(object sender, EventArgs e)
        {
            SetDropDownByValue((DropDownList)sender, mCRUD.GetSetting("DefaultParentAccountCode"));
        }

        protected void DDL_Subledger_PreRender(object sender, EventArgs e)
        {
            SetDropDownByValue((DropDownList)sender, mCRUD.GetSetting("DefaultSubLedgerName"));
        }
    }
}