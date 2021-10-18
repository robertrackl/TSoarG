using System;
using System.Collections.Generic;
using System.Data;
using TSoar.DB;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TSoar.Accounting.FinDetails.ExpVendAP
{
    public partial class ExpFilter : System.Web.UI.Page
    {
        private DataTable dtFilters = new DataTable("ExpFilterSettings", "TSoar.Accounting.FinDetails.ExpVendAP");

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

        }
        private void ProcessPopupException(Global.excToPopup excu)
        {
            ButtonsClear();
            lblPopupText.Text = excu.sExcMsg();
            ActivityLog.oLog(ActivityLog.enumLogTypes.PopupException, 0, lblPopupText.Text);
            MPE_Show(Global.enumButtons.OkOnly);
        }
        #endregion

        #region UpFront Items
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                bCheck4dtFilterReset(dtFilters, lblVersionUpdate);
            }
        }

        public void bCheck4dtFilterReset(DataTable dtu, Label lblu)
        {
            dtu = AccountProfile.CurrentUser.XactFilterSettings;
            bool b = false;
            if (dtu.Rows.Count < 1)
            {
                b = true;
            }
            else
            {
                // The LowLimit property in the very first row of the filter settings DataTable is used to store the
                //     filter settings version
                if (((decimal)dtu.Rows[(int)Global.egXactFilters.Version][(int)Global.egAdvFilterProps.LowLimit]) !=
                        Global.dgcVersionOfXactFilterSettingsDataTable)
                {
                    lblu.Visible = true;
                    b = true;
                }
            }
            if (b)
            {
                // There was no entry in table aspnet_Profile for this user, or a version conflict is forcing a reset of dtu
                dtu = XactFilter.dtDefaultXactFilter();
                // Save filter settings for this user
                AccountProfile.CurrentUser.XactFilterSettings = dtu;
            }
            return;
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                dtFilters = AccountProfile.CurrentUser.XactFilterSettings;
                InitControls();
            }
        }
        private void InitControls()
        {
            // Initialize the controls based upon the previously saved filter settings
            int ix = (int)Global.egXactFilters.EnableFilteringOverall;
            chbEnableFiltering.Checked = (bool)dtFilters.Rows[ix][(int)Global.egAdvFilterProps.Enabled];

            InitListFilter((int)Global.egXactFilters.Vendor, chbVendor, chbVendorIN, txbDDLVendor);
            InitListFilter((int)Global.egXactFilters.TransactionStatus, chbStatus, chbStatusIN, txbDDLStatus);
            InitListFilter((int)Global.egXactFilters.AttachmentCateg, chbAttCat, chbAttCatIN, txbDDLAttCat);
            InitListFilter((int)Global.egXactFilters.AttachmentType, chbAttTyp, chbAttTypIN, txbDDLAttTyp);
            InitListFilter((int)Global.egXactFilters.PaymentMethod, chbPmtMeth, chbPmtMethIN, txbDDLPmtMeth);
            InitListFilter((int)Global.egXactFilters.ExpenseAccount, chbExpAcc, chbExpAccIN, txbDDLExpAcc);
            InitListFilter((int)Global.egXactFilters.PaymentAccount, chbPmtAcc, chbPmtAccIN, txbDDLPmtAcc);

            ix = (int)Global.egXactFilters.XactDate;
            chbXactDate.Checked = (bool)dtFilters.Rows[ix][(int)Global.egAdvFilterProps.Enabled];
            // Special use of 'List' for Xact date:
            string[] sa1 = ((string)dtFilters.Rows[ix][(int)Global.egAdvFilterProps.List]).Split(',');
            string ss = sa1[0].Replace("'", "").Trim();
            string[] sa2 = ss.Split(' ');
            txbXactDateLo.Text = sa2[0];
            txbXactTimeLo.Text = sa2[1];
            txbXactOffsetLo.Text = sa2[2];
            ss = sa1[1].Replace("'", "").Trim();
            sa2 = ss.Split(' ');
            txbXactDateHi.Text = sa2[0];
            txbXactTimeHi.Text = sa2[1];
            txbXactOffsetHi.Text = sa2[2];

            InitRangeFilter((int)Global.egXactFilters.XactAmount, chbAmount, txbAmountLo, txbAmountHi);
            InitRangeFilter((int)Global.egXactFilters.NumAttFiles, chbNAtt, txbNAttLo, txbNAttHi);
        }
        private void InitListFilter(int iux, CheckBox chbu, CheckBox chbuIN, TextBox txbuList)
        {
            chbu.Checked = (bool)dtFilters.Rows[iux][(int)Global.egAdvFilterProps.Enabled];
            txbuList.Text = (string)dtFilters.Rows[iux][(int)Global.egAdvFilterProps.List];
            if (txbuList.Text.Length < 1) { txbuList.Text = "All"; }
            if (txbuList.Text == "All")
            {
                chbuIN.Checked = true;
                chbuIN.Enabled = false;
            }
            else
            {
                chbuIN.Checked = (bool)dtFilters.Rows[iux][(int)Global.egAdvFilterProps.INorEX];
                chbuIN.Enabled = true;
            }
        }
        private void InitRangeFilter(int iux, CheckBox chbu, TextBox txbuLo, TextBox txbuHi)
        {
            chbu.Checked = (bool)dtFilters.Rows[iux][(int)Global.egAdvFilterProps.Enabled];
            txbuLo.Text = ((decimal)dtFilters.Rows[iux][(int)Global.egAdvFilterProps.LowLimit]).ToString();
            txbuHi.Text = ((decimal)dtFilters.Rows[iux][(int)Global.egAdvFilterProps.HighLimit]).ToString();
        }
        #endregion

        #region Vendor
        protected void DDLVendor_SelectedIndexChanged(object sender, EventArgs e)
        {
            DDL_SelectedIndexChanged(DDLVendor, txbDDLVendor, chbVendorIN);
        }
        #endregion
        #region Xact Status
        protected void DDLStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            DDL_SelectedIndexChanged(DDLStatus, txbDDLStatus, chbStatusIN);
        }
        #endregion
        #region Attachment Category
        protected void DDLAttCat_SelectedIndexChanged(object sender, EventArgs e)
        {
            DDL_SelectedIndexChanged(DDLAttCat, txbDDLAttCat, chbAttCatIN);
        }
        #endregion
        #region Attachment Type
        protected void DDLAttTyp_SelectedIndexChanged(object sender, EventArgs e)
        {
            DDL_SelectedIndexChanged(DDLAttTyp, txbDDLAttTyp, chbAttTypIN);
        }
        #endregion
        #region Payment Method
        protected void DDLPmtMeth_SelectedIndexChanged(object sender, EventArgs e)
        {
            DDL_SelectedIndexChanged(DDLPmtMeth, txbDDLPmtMeth, chbPmtMethIN);
        }
        #endregion
        #region Expense Account
        protected void DDLExpAcc_SelectedIndexChanged(object sender, EventArgs e)
        {
            DDL_SelectedIndexChanged(DDLExpAcc, txbDDLExpAcc, chbExpAccIN);
        }
        #endregion
        #region Payment Account
        protected void DDLPmtAcc_SelectedIndexChanged(object sender, EventArgs e)
        {
            DDL_SelectedIndexChanged(DDLPmtAcc, txbDDLPmtAcc, chbPmtAccIN);
        }
        #endregion
        #region Xact Date
        #endregion
        #region Amount
        #endregion
        #region Number of Attached Files
        #endregion
        protected void pbXactOK_Click(object sender, EventArgs e)
        {
            if (!(bFilterListCheck("Vendor", chbVendor, chbVendorIN, txbDDLVendor) &&
                bFilterListCheck("Transaction Status", chbStatus, chbStatusIN, txbDDLStatus) &&
                bFilterListCheck("File Attachment Category", chbAttCat, chbAttCatIN, txbDDLAttCat) &&
                bFilterListCheck("File Attachment Type", chbAttTyp, chbAttTypIN, txbDDLAttTyp) &&
                bFilterListCheck("Payment Method", chbPmtMeth, chbPmtMethIN, txbDDLPmtMeth) &&
                bFilterListCheck("Expense Account", chbExpAcc, chbExpAccIN, txbDDLExpAcc) &&
                bFilterListCheck("Payment Account", chbPmtAcc, chbPmtAccIN, txbDDLPmtAcc)
                ))
            {
                return;
            }
            if (!(bFilterRangeCheck("Transaction Date", chbXactDate, txbXactDateLo, txbXactDateHi) &&
                bFilterRangeCheck("Transaction Amount", chbAmount, txbAmountLo, txbAmountHi) &&
                bFilterRangeCheck("Nummber of Attached Files", chbNAtt, txbNAttLo, txbNAttHi)
                ))
            {
                return;
            }

            // Remember the filter settings for the current user
            dtFilters = AccountProfile.CurrentUser.XactFilterSettings;

            int ix = (int)Global.egXactFilters.EnableFilteringOverall;
            dtFilters.Rows[ix][(int)Global.egAdvFilterProps.Enabled] = chbEnableFiltering.Checked;

            ix = (int)Global.egXactFilters.Vendor;
            dtFilters.Rows[ix][(int)Global.egAdvFilterProps.Enabled] = chbVendor.Checked;
            dtFilters.Rows[ix][(int)Global.egAdvFilterProps.INorEX] = chbVendorIN.Checked;
            dtFilters.Rows[ix][(int)Global.egAdvFilterProps.List] = txbDDLVendor.Text;
            ix = (int)Global.egXactFilters.TransactionStatus;
            dtFilters.Rows[ix][(int)Global.egAdvFilterProps.Enabled] = chbStatus.Checked;
            dtFilters.Rows[ix][(int)Global.egAdvFilterProps.INorEX] = chbStatusIN.Checked;
            dtFilters.Rows[ix][(int)Global.egAdvFilterProps.List] = txbDDLStatus.Text;
            ix = (int)Global.egXactFilters.AttachmentCateg;
            dtFilters.Rows[ix][(int)Global.egAdvFilterProps.Enabled] = chbAttCat.Checked;
            dtFilters.Rows[ix][(int)Global.egAdvFilterProps.INorEX] = chbAttCatIN.Checked;
            dtFilters.Rows[ix][(int)Global.egAdvFilterProps.List] = txbDDLAttCat.Text;
            ix = (int)Global.egXactFilters.AttachmentType;
            dtFilters.Rows[ix][(int)Global.egAdvFilterProps.Enabled] = chbAttTyp.Checked;
            dtFilters.Rows[ix][(int)Global.egAdvFilterProps.INorEX] = chbAttTypIN.Checked;
            dtFilters.Rows[ix][(int)Global.egAdvFilterProps.List] = txbDDLAttTyp.Text;
            ix = (int)Global.egXactFilters.PaymentMethod;
            dtFilters.Rows[ix][(int)Global.egAdvFilterProps.Enabled] = chbPmtMeth.Checked;
            dtFilters.Rows[ix][(int)Global.egAdvFilterProps.INorEX] = chbPmtMethIN.Checked;
            dtFilters.Rows[ix][(int)Global.egAdvFilterProps.List] = txbDDLPmtMeth.Text;
            ix = (int)Global.egXactFilters.ExpenseAccount;
            dtFilters.Rows[ix][(int)Global.egAdvFilterProps.Enabled] = chbExpAcc.Checked;
            dtFilters.Rows[ix][(int)Global.egAdvFilterProps.INorEX] = chbExpAccIN.Checked;
            dtFilters.Rows[ix][(int)Global.egAdvFilterProps.List] = txbDDLExpAcc.Text;
            ix = (int)Global.egXactFilters.PaymentAccount;
            dtFilters.Rows[ix][(int)Global.egAdvFilterProps.Enabled] = chbPmtAcc.Checked;
            dtFilters.Rows[ix][(int)Global.egAdvFilterProps.INorEX] = chbPmtAccIN.Checked;
            dtFilters.Rows[ix][(int)Global.egAdvFilterProps.List] = txbDDLPmtAcc.Text;

            ix = (int)Global.egXactFilters.XactDate;
            dtFilters.Rows[ix][(int)Global.egAdvFilterProps.Enabled] = chbXactDate.Checked;
            // Special use of 'List' for Xact date:
            dtFilters.Rows[ix][(int)Global.egAdvFilterProps.List] =
                txbXactDateLo.Text + " " + txbXactTimeLo.Text + " " + txbXactOffsetLo.Text + "," + 
                txbXactDateHi.Text + " " + txbXactTimeHi.Text + " " + txbXactOffsetHi.Text;

            ix = (int)Global.egXactFilters.XactAmount;
            dtFilters.Rows[ix][(int)Global.egAdvFilterProps.Enabled] = chbAmount.Checked;
            if (txbAmountLo.Text.Length < 1) { txbAmountLo.Text = "0.00"; }
            dtFilters.Rows[ix][(int)Global.egAdvFilterProps.LowLimit] = decimal.Parse(txbAmountLo.Text);
            if (txbAmountHi.Text.Length < 1) { txbAmountHi.Text = "999999999.99"; }
            dtFilters.Rows[ix][(int)Global.egAdvFilterProps.HighLimit] = decimal.Parse(txbAmountHi.Text);
            ix = (int)Global.egXactFilters.NumAttFiles;
            dtFilters.Rows[ix][(int)Global.egAdvFilterProps.Enabled] = chbNAtt.Checked;
            if (txbNAttLo.Text.Length < 1) { txbNAttLo.Text = "0"; }
            dtFilters.Rows[ix][(int)Global.egAdvFilterProps.LowLimit] = Int32.Parse(txbNAttLo.Text);
            if (txbNAttHi.Text.Length < 1) { txbNAttHi.Text = "100"; }
            dtFilters.Rows[ix][(int)Global.egAdvFilterProps.HighLimit] = Int32.Parse(txbNAttHi.Text);

            AccountProfile.CurrentUser.XactFilterSettings = dtFilters; // Save filter settings for current user

            Server.Transfer("~/Accounting/FinDetails/ExpVendAP/Expenses.aspx", true);
        }

        protected void pbExpCancel_Click(object sender, EventArgs e)
        {
            Server.Transfer("~/Accounting/FinDetails/ExpVendAP/Expenses.aspx", true);
        }

        private void DDL_SelectedIndexChanged(DropDownList uddl, TextBox utxb, CheckBox chbuIN)
        {
            // Process the comma-separated list of items
            string sPunct = "";
            if (utxb.Text == "All")
            {
                utxb.Text = "";
            }
            string[] sax = utxb.Text.Split(',');
            if (!(sax.Length < 2 && sax[0].Length < 1))
            {
                foreach (string ss in sax)
                {
                    if (ss.Replace("'", "") == uddl.SelectedItem.Text) return;
                }
                sPunct = ",";
            }
            string s1 = uddl.SelectedItem.Text.Trim();
            if (s1.Length > 0)
            {
                if (uddl == DDLStatus) // Only want the first character of cStatus
                {
                    s1 = s1.Substring(0, 1);
                }
                utxb.Text += sPunct + "'" + s1 + "'";
            }
            if (utxb.Text.Length < 1)
            {
                utxb.Text = "All";
                chbuIN.Enabled = false;
            }
            else
            {
                chbuIN.Enabled = true;
            }
            uddl.SelectedIndex = 0;
        }

        protected void pbReset_Click(object sender, EventArgs e)
        {
            Button pb = (Button)sender;
            switch (pb.ID)
            {
                case "pbResetVendor":
                    txbDDLVendor.Text = "All";
                    chbVendorIN.Checked = true;
                    chbVendorIN.Enabled = false;
                    break;
                case "pbResetStatus":
                    txbDDLStatus.Text = "All";
                    chbStatusIN.Checked = true;
                    chbStatusIN.Enabled = false;
                    break;
                case "pbResetAttCat":
                    txbDDLAttCat.Text = "All";
                    chbAttCatIN.Checked = true;
                    chbAttCatIN.Enabled = false;
                    break;
                case "pbResetAttTyp":
                    txbDDLAttTyp.Text = "All";
                    chbAttTypIN.Checked = true;
                    chbAttTypIN.Enabled = false;
                    break;
                case "pbResetPmtMeth":
                    txbDDLPmtMeth.Text = "All";
                    chbPmtMethIN.Checked = true;
                    chbPmtMethIN.Enabled = false;
                    break;
                case "pbResetExpAcc":
                    txbDDLExpAcc.Text = "All";
                    chbExpAccIN.Checked = true;
                    chbExpAccIN.Enabled = false;
                    break;
                case "pbResetPmtAcc":
                    txbDDLPmtAcc.Text = "All";
                    chbPmtAccIN.Checked = true;
                    chbPmtAccIN.Enabled = false;
                    break;
                case "pbResetXactDate":
                    txbXactDateLo.Text = "2000-01-01";
                    txbXactDateHi.Text = "2099-12-31";
                    break;
                case "pbResetAmount":
                    txbAmountLo.Text = "0.00";
                    txbAmountHi.Text = "999999999.99";
                    break;
                case "pbResetNAtt":
                    txbNAttLo.Text = "0";
                    txbNAttHi.Text = "100";
                    break;
            }
        }

        private bool bFilterListCheck(string suItem, CheckBox chbuItem, CheckBox chbuIN, TextBox utxb)
        {
            if (chbuItem.Checked && chbuIN.Checked && (utxb.Text.Length < 1))
            {
                ProcessPopupException(new Global.excToPopup(suItem + " filter is in use, but its list is empty"));
                return false;
            }
            return true;
        }
        private bool bFilterRangeCheck(string suItem, CheckBox chbuItem, TextBox txbLo, TextBox txbHi)
        {
            if (chbuItem.Checked)
            {
                if (txbLo.Text.Length < 1)
                {
                    ProcessPopupException(new Global.excToPopup(suItem + " Filter is in use, but its lower limit is empty"));
                    return false;
                }
                if (txbHi.Text.Length < 1)
                {
                    ProcessPopupException(new Global.excToPopup(suItem + " Filter is in use, but its upper limit is empty"));
                    return false;
                }
                if (suItem == "Transaction Date")
                {
                    DateTime DLo = DateTime.Parse(txbLo.Text);
                    DateTime DHi = DateTime.Parse(txbHi.Text);
                    if (DLo > DHi)
                    {
                        ProcessPopupException(new Global.excToPopup(suItem + " Filter is in use, but its lower limit is greater than its upper limit"));
                        return false;
                    }
                }
                else
                {
                    decimal dLo = decimal.Parse(txbLo.Text);
                    decimal dHi = decimal.Parse(txbHi.Text);
                    if (dLo > dHi)
                    {
                        ProcessPopupException(new Global.excToPopup(suItem + " Filter is in use, but its lower limit is greater than its upper limit"));
                        return false;
                    }
                }
            }
            return true;
        }
    }
}