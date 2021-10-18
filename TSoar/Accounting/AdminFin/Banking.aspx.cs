using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TSoar.DB;

namespace TSoar.Accounting
{
    public partial class Banking : System.Web.UI.Page
    {
        #region Declarations
        const int icPiTRecordEntered = 0;
        const int icRecordEnteredBy = 1;
        const int icFinInstit = 2;
        const int icBankAcctType = 2;
        const int icNotesBI = 3;
        SCUD_Multi mCRUD = new SCUD_Multi();
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
                        case "FinInstitution":
                            // Delete the financial institution
                            mCRUD.DeleteOne(Global.enugInfoType.SF_FinInstitutions, btn.CommandArgument);
                            DisplayInGrid(Global.enugInfoType.SF_FinInstitutions);
                            break;
                        case "BankAcctType":
                            // Delete the bank account type
                            mCRUD.DeleteOne(Global.enugInfoType.SF_BankAcctTypes, btn.CommandArgument);
                            DisplayInGrid(Global.enugInfoType.SF_BankAcctTypes);
                            break;
                        case "BankAccount":
                            // Delete the Bank Account
                            mCRUD.DeleteOne(Global.enugInfoType.SF_BankAccts, btn.CommandArgument);
                            DisplayInGrid(Global.enugInfoType.SF_BankAccts);
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

        private void DisplayInGrid(Global.enugInfoType euInfoType)
        {
            GridView g = null;
            string ss = "";
            switch (euInfoType)
            {
                case Global.enugInfoType.SF_FinInstitutions:
                    g = gvFinInst;
                    ss = "PageSizeFinInstitutions";
                    break;
                case Global.enugInfoType.SF_BankAcctTypes:
                    g = gvBankAcctType;
                    ss = "PageSizeBankAcctTypes";
                    break;
                case Global.enugInfoType.SF_BankAccts:
                    g = gvBankAcct;
                    ss = "PageSizeBankAccts";
                    break;
                default:
                    return;
            }
            g.PageSize = Int32.Parse(mCRUD.GetSetting(ss));
            g.DataSource = mCRUD.GetAll(euInfoType);
            g.DataBind();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DisplayInGrid(Global.enugInfoType.SF_FinInstitutions);
                DisplayInGrid(Global.enugInfoType.SF_BankAcctTypes);
                DisplayInGrid(Global.enugInfoType.SF_BankAccts);
                AccordionBanking.SelectedIndex = 5;
            }
        }
        #region Financial Institutions
        protected void dvFinInst_ItemInserted(object sender, DetailsViewInsertedEventArgs e)
        {
            Response.Redirect(Request.RawUrl);
        }

        protected void dvFinInst_ItemInserting(object sender, DetailsViewInsertEventArgs e)
        {
            string[] sa = new string[4];
            Global.excToPopup exc = null;
            e.Cancel = true; // because we are handling our own insertion
            sa[icPiTRecordEntered] = DateTime.UtcNow.ToString();
            sa[icRecordEnteredBy] = mCRUD.GetPeopleIDfromWebSiteUserName(HttpContext.Current.User.Identity.Name).ToString();
            sa[icFinInstit] = Server.HtmlEncode(((TextBox)dvFinInst.FindControl("txbFinInst")).Text.Trim().Replace("'", "`"));
            sa[icNotesBI] = Server.HtmlEncode(((TextBox)dvFinInst.FindControl("txbNotes")).Text.Trim().Replace("'", "`"));
            int iIdent = 0;
            mCRUD.InsertOne(Global.enugInfoType.SF_FinInstitutions, sa, out iIdent);
            string sVal = "Record Inserted: ";
            for (int i = 2; i < sa.Count(); i++)
            {
                sVal += ((i > 2) ? ", " : " ") + sa[i];
            }
            exc = new Global.excToPopup(sVal);
            ProcessPopupException(exc);
            DisplayInGrid(Global.enugInfoType.SF_FinInstitutions);
        }

        protected void gvFinInst_RowEditing(object sender, GridViewEditEventArgs e)
        {
            sMKey = ((Label)gvFinInst.Rows[e.NewEditIndex].FindControl("lblID")).Text;
            gvFinInst.EditIndex = e.NewEditIndex;
            DisplayInGrid(Global.enugInfoType.SF_FinInstitutions);
        }

        protected void gvFinInst_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            Label lblItem = (Label)gvFinInst.Rows[e.RowIndex].FindControl("lblFinInst");
            string sItem = lblItem.Text;
            ButtonsClear();
            YesButton.CommandName = "Delete";
            YesButton.CommandArgument = ((Label)gvFinInst.Rows[e.RowIndex].FindControl("lblID")).Text;
            OkButton.CommandArgument = "FinInstitution";
            lblPopupText.Text = "It is quite unusual to delete a financial institution; they are usually kept on file for historical recordkeeping. " +
                "If you still want to go ahead, please confirm deletion of the financial institution '" + sItem + "'";
            MPE_Show(Global.enumButtons.NoYes);
        }

        protected void gvFinInst_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            string[] sa = new string[4];
            GridViewRow row = gvFinInst.Rows[e.RowIndex];
            TextBox txb;
            sa[icPiTRecordEntered] = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.mmm");
            sa[icRecordEnteredBy] = mCRUD.GetPeopleIDfromWebSiteUserName(HttpContext.Current.User.Identity.Name).ToString();
            txb = (TextBox)row.FindControl("txbUFinInst");
            sa[icFinInstit] = Server.HtmlEncode(txb.Text.Trim().Replace("'", "`"));
            txb = (TextBox)row.FindControl("txbUNotes");
            sa[icNotesBI] = Server.HtmlEncode(txb.Text.Trim().Replace("'", "`"));
            try
            {
                mCRUD.UpdateOne(Global.enugInfoType.SF_FinInstitutions, sMKey, sa);
                gvFinInst.EditIndex = -1;
            }
            catch (Global.excToPopup exc1)
            {
                ProcessPopupException(exc1);
            }
            DisplayInGrid(Global.enugInfoType.SF_FinInstitutions);
        }

        protected void gvFinInst_RowCreated(object sender, GridViewRowEventArgs e)
        {
            foreach (TableCell cell in e.Row.Cells) { cell.CssClass = "cell-padding"; }
        }

        protected void gvFinInst_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvFinInst.EditIndex = -1;
            DisplayInGrid(Global.enugInfoType.SF_FinInstitutions);
        }

        protected void gvFinInst_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvFinInst.PageIndex = e.NewPageIndex;
            DisplayInGrid(Global.enugInfoType.SF_FinInstitutions);
        }
        #endregion

        #region Bank Account Types
        protected void dvBankAcctType_ItemInserted(object sender, DetailsViewInsertedEventArgs e)
        {
            Response.Redirect(Request.RawUrl);
        }

        protected void dvBankAcctType_ItemInserting(object sender, DetailsViewInsertEventArgs e)
        {
            string[] sa = new string[4];
            Global.excToPopup exc = null;
            e.Cancel = true; // because we are handling our own insertion
            sa[icPiTRecordEntered] = DateTime.UtcNow.ToString();
            sa[icRecordEnteredBy] = mCRUD.GetPeopleIDfromWebSiteUserName(HttpContext.Current.User.Identity.Name).ToString();
            sa[icFinInstit] = Server.HtmlEncode(((TextBox)dvBankAcctType.FindControl("txbBankAcctType")).Text.Trim().Replace("'", "`"));
            sa[icNotesBI] = Server.HtmlEncode(((TextBox)dvBankAcctType.FindControl("txbNotes")).Text.Trim().Replace("'", "`"));
            int iIdent = 0;
            mCRUD.InsertOne(Global.enugInfoType.SF_BankAcctTypes, sa, out iIdent);
            string sVal = "Record Inserted: ";
            for (int i = 2; i < sa.Count(); i++)
            {
                sVal += ((i > 2) ? ", " : " ") + sa[i];
            }
            exc = new Global.excToPopup(sVal);
            ProcessPopupException(exc);
            DisplayInGrid(Global.enugInfoType.SF_BankAcctTypes);
        }

        protected void gvBankAcctType_RowEditing(object sender, GridViewEditEventArgs e)
        {
            sMKey = ((Label)gvBankAcctType.Rows[e.NewEditIndex].FindControl("lblID")).Text;
            gvBankAcctType.EditIndex = e.NewEditIndex;
            DisplayInGrid(Global.enugInfoType.SF_BankAcctTypes);
        }

        protected void gvBankAcctType_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            Label lblItem = (Label)gvBankAcctType.Rows[e.RowIndex].FindControl("lblBankAcctType");
            string sItem = lblItem.Text;
            ButtonsClear();
            YesButton.CommandName = "Delete";
            YesButton.CommandArgument = ((Label)gvFinInst.Rows[e.RowIndex].FindControl("lblID")).Text;
            OkButton.CommandArgument = "BankAcctType";
            lblPopupText.Text = "It is quite unusual to delete a bank account type; they are usually kept on file for historical recordkeeping. " +
                "If you still want to go ahead, please confirm deletion of the bank account type '" + sItem + "'";
            MPE_Show(Global.enumButtons.NoYes);
        }

        protected void gvBankAcctType_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            string[] sa = new string[4];
            GridViewRow row = gvBankAcctType.Rows[e.RowIndex];
            TextBox txb;
            sa[icPiTRecordEntered] = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.mmm");
            sa[icRecordEnteredBy] = mCRUD.GetPeopleIDfromWebSiteUserName(HttpContext.Current.User.Identity.Name).ToString();
            txb = (TextBox)row.FindControl("txbUBankAcctType");
            sa[icBankAcctType] = Server.HtmlEncode(txb.Text.Trim().Replace("'", "`"));
            txb = (TextBox)row.FindControl("txbUNotes");
            sa[icNotesBI] = Server.HtmlEncode(txb.Text.Trim().Replace("'", "`"));
            try
            {
                mCRUD.UpdateOne(Global.enugInfoType.SF_BankAcctTypes, sMKey, sa);
                gvBankAcctType.EditIndex = -1;
            }
            catch (Global.excToPopup exc1)
            {
                ProcessPopupException(exc1);
            }
            DisplayInGrid(Global.enugInfoType.SF_BankAcctTypes);
        }

        protected void gvBankAcctType_RowCreated(object sender, GridViewRowEventArgs e)
        {
            foreach (TableCell cell in e.Row.Cells) { cell.CssClass = "cell-padding"; }
        }

        protected void gvBankAcctType_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvBankAcctType.EditIndex = -1;
            DisplayInGrid(Global.enugInfoType.SF_BankAcctTypes);
        }

        protected void gvBankAcctType_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvBankAcctType.PageIndex = e.NewPageIndex;
            DisplayInGrid(Global.enugInfoType.SF_BankAcctTypes);
        }
        #endregion

        #region Bank Accounts
        protected void dvBankAcct_ItemInserted(object sender, DetailsViewInsertedEventArgs e)
        {
            Response.Redirect(Request.RawUrl);
        }

        protected void dvBankAcct_ItemInserting(object sender, DetailsViewInsertEventArgs e)
        {
            string[] sa = new string[8];
            Global.excToPopup exc = null;
            e.Cancel = true; // because we are handling our own insertion
            sa[icPiTRecordEntered] = DateTime.UtcNow.ToString();
            sa[icRecordEnteredBy] = mCRUD.GetPeopleIDfromWebSiteUserName(HttpContext.Current.User.Identity.Name).ToString();
            sa[icFinInstit] = ((DropDownList)dvBankAcct.FindControl("DDL_FinInst")).SelectedValue.ToString();
            sa[3] = ((DropDownList)dvBankAcct.FindControl("DDL_BankAcctType")).SelectedValue.ToString();
            sa[4] = Server.HtmlEncode(((TextBox)dvBankAcct.FindControl("txbAcctNum")).Text.Trim().Replace("'", "`"));
            sa[5] = Server.HtmlEncode(((TextBox)dvBankAcct.FindControl("txbName")).Text.Trim().Replace("'", "`"));
            sa[6] = ((DropDownList)dvBankAcct.FindControl("DDL_AssocAcct")).SelectedValue.ToString();
            sa[7] = Server.HtmlEncode(((TextBox)dvBankAcct.FindControl("txbNotes")).Text.Trim().Replace("'", "`"));
            string sVal = sValid(sa);
            if (sVal == "OK")
            {
                int iIdent = 0;
                mCRUD.InsertOne(Global.enugInfoType.SF_BankAccts, sa, out iIdent);
                sVal = "Record Inserted: ";
                for (int i = 2; i < sa.Count(); i++)
                {
                    sVal += ((i > 2) ? ", " : " ") + sa[i];
                }
            }
            exc = new Global.excToPopup(sVal);
            ProcessPopupException(exc);
            DisplayInGrid(Global.enugInfoType.SF_BankAccts);
        }
        private string sValid(string[] sau)
        {
            string sRet = "OK";
            if (sau[5].Length < 1)
            {
                sRet = "Bank Account Name must not be empty";
            }
            return sRet;
        }

        protected void DDL_FinInst_PreRender(object sender, EventArgs e)
        {
            SetDropDownByValue((DropDownList)sender, mCRUD.GetSetting("DefaultFinancialInstitution"));
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

        protected void DDL_BankAcctType_PreRender(object sender, EventArgs e)
        {
            SetDropDownByValue((DropDownList)sender, mCRUD.GetSetting("DefaultBankAccountType"));
        }

        protected void DDL_AssocAcct_PreRender(object sender, EventArgs e)
        {
            SetDropDownByValue((DropDownList)sender, mCRUD.GetSetting("DefaultAssociatedAccountCode"));
        }

        protected void gvBankAcct_RowEditing(object sender, GridViewEditEventArgs e)
        {
            sMKey = ((Label)gvBankAcct.Rows[e.NewEditIndex].FindControl("lblId")).Text;
            gvBankAcct.EditIndex = e.NewEditIndex;
            DisplayInGrid(Global.enugInfoType.SF_BankAccts);
        }

        protected void gvBankAcct_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if ((e.Row.RowState & DataControlRowState.Edit) > 0)
                {
                    DropDownList DDLFinInst = (DropDownList)e.Row.FindControl("DDLFinInst");
                    string sD = DataBinder.Eval(e.Row.DataItem, "sFinancialInstitution").ToString();
                    SetDropDownByValue(DDLFinInst, sD);
                    DropDownList DDLBankAcctType = (DropDownList)e.Row.FindControl("DDLBankAcctType");
                    sD = DataBinder.Eval(e.Row.DataItem, "sBankAcctType").ToString();
                    SetDropDownByValue(DDLBankAcctType, sD);
                    DropDownList DDLAssocAcct = (DropDownList)e.Row.FindControl("DDLAssocAcct");
                    sD = DataBinder.Eval(e.Row.DataItem, "sAccount").ToString();
                    SetDropDownByValue(DDLAssocAcct, sD);
                }
            }
        }

        protected void gvBankAcct_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            Label lblFinInst = (Label)gvBankAcct.Rows[e.RowIndex].FindControl("lblFinInst");
            Label lblBankAcctType = (Label)gvBankAcct.Rows[e.RowIndex].FindControl("lblBankAcctType");
            Label lblAcctNum = (Label)gvBankAcct.Rows[e.RowIndex].FindControl("lblAcctNum");
            Label lblName = (Label)gvBankAcct.Rows[e.RowIndex].FindControl("lblName");
            string sItem = lblFinInst.Text + " " + lblBankAcctType.Text + " " + lblAcctNum.Text + " " + lblName.Text;
            ButtonsClear();
            YesButton.CommandName = "Delete";
            YesButton.CommandArgument = ((Label)gvBankAcct.Rows[e.RowIndex].FindControl("lblID")).Text;
            OkButton.CommandArgument = "BankAccount";
            lblPopupText.Text = "It is quite unusual to delete a bank account; they are usually kept on file for historical recordkeeping. " +
                "If you still want to go ahead, please confirm deletion of the bank account '" + sItem + "'";
            MPE_Show(Global.enumButtons.NoYes);
        }

        protected void gvBankAcct_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            string[] sa = new string[8];
            Global.excToPopup exc = null;
            GridViewRow row = gvBankAcct.Rows[e.RowIndex];
            TextBox txb;
            sa[icPiTRecordEntered] = DateTime.UtcNow.ToString();
            sa[icRecordEnteredBy] = mCRUD.GetPeopleIDfromWebSiteUserName(HttpContext.Current.User.Identity.Name).ToString();
            txb = (TextBox)row.FindControl("txbUCode");
            DropDownList DDLFinInst = (DropDownList)row.FindControl("DDLFinInst");
            sa[icFinInstit] = DDLFinInst.Items[DDLFinInst.SelectedIndex].Value;
            DropDownList DDLBankAcctType = (DropDownList)row.FindControl("DDLBankAcctType");
            sa[3] = DDLBankAcctType.Items[DDLBankAcctType.SelectedIndex].Value;
            txb = (TextBox)row.FindControl("txbUAcctNum");
            sa[4] = Server.HtmlEncode(txb.Text.Trim().Replace("'", "`"));
            txb = (TextBox)row.FindControl("txbUName");
            sa[5] = Server.HtmlEncode(txb.Text.Trim().Replace("'", "`"));
            DropDownList DDLAssocAcct = (DropDownList)row.FindControl("DDLAssocAcct");
            sa[6] = DDLAssocAcct.Items[DDLAssocAcct.SelectedIndex].Value;
            txb = (TextBox)row.FindControl("txbUNotes");
            sa[7] = Server.HtmlEncode(txb.Text.Trim().Replace("'", "`"));
            string sVal = sValid(sa);
            if (sVal == "OK")
            {
                try
                {
                    mCRUD.UpdateOne(Global.enugInfoType.SF_BankAccts, sMKey, sa);
                    gvBankAcct.EditIndex = -1;
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
            DisplayInGrid(Global.enugInfoType.SF_BankAccts);
        }

        protected void gvBankAcct_RowCreated(object sender, GridViewRowEventArgs e)
        {
            foreach (TableCell cell in e.Row.Cells) { cell.CssClass = "cell-padding"; }
        }

        protected void gvBankAcct_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvBankAcct.EditIndex = -1;
            DisplayInGrid(Global.enugInfoType.SF_BankAccts);
        }

        protected void gvBankAcct_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvBankAcct.PageIndex = e.NewPageIndex;
            DisplayInGrid(Global.enugInfoType.SF_BankAccts);
        }
        #endregion
    }
}