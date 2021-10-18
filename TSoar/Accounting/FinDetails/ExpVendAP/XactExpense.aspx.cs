using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TSoar.DB;

namespace TSoar.Accounting.FinDetails.ExpVendAP
{
    public partial class XactExpense : System.Web.UI.Page
    {
        private static char cq = Convert.ToChar(34); // double quote

        SCUD_Multi mCRUD = new SCUD_Multi();

        #region About CallMode
        // About CallMode:
        // ===============
        // There exists a Session variable Session["CallMode"] which contains a KeyValuePair<string,int>; it is used to transmit to class
        //     XactExpense from an outside caller what XactExpense is supposed to do:
        //     * CallMode with Key "NEW" causes creation of a new transaction; this is also the default when Session["CallMode"] is not
        //          yet defined. The CallMode integer value of the KeyValuePair should be set to zero.
        //     * CallMode "EDIT" is accompanied by an integer value that equals the ID (in table SF_XACTS) of the transaction to be edited;
        //          XactExpense fills its display page by copying the data from that transaction. The user may modify the data; when the user requests
        //          Save, the modified transaction is saved as a new transaction and the copied transaction is marked 'Replaced' (no data
        //          are deleted).
        //     * CallMode "INSPECT" is also accompanied by an integer value of the ID of the transaction to be inspected; its data is used
        //          to fill the XactExpense display page. The user has no opportunity to modify the data, nor to save anything.
        //     * CallMode "RESUME" is used to resume working with a transaction either in NEW or EDIT mode when that work was interrupted
        //          by temporarily going to page "FilterSortAttFiles" in order to attach an already existing file in table SF_DOCS to the current transaction
        //
        // There exists also a local writable property kvpCallMode which is used to track the CallMode across postbacks of the XactExpense page by
        //     using ViewState["CallMode"]. In the Page_Load event handler, Session["CallMode"] is copied to kvpCallMode. The latter
        //     is then used during the remainder of the page's existence. Right away in Page_Load, Session["CallMode"] is set back to its default value
        //     of "NEW" to prevent any inadvertent repeats of editing or inspection commands.

        // See also comments in button click event handler pbAttFile_Click
        #endregion

        #region Declarations
        private readonly string scNoFileSelected = "No File Selected";
        private string slVendor { get { return (string)ViewState["slVendor"] ?? "(none)"; } set { ViewState["slVendor"] = value; } } // `(none)` better exist in table SF_VENDORS
        private KeyValuePair<string, int> kvpCallMode {
            get { return GetkvpCallMode("CallMode"); }
            set { ViewState["CallMode"] = value; }
        }
        private KeyValuePair<string, int> GetkvpCallMode(string suCallMode)
        {
            if (ViewState[suCallMode] == null)
            {
                return new KeyValuePair<string, int>("NEW", 0);
            }
            else
            {
                return (KeyValuePair<string, int>)ViewState[suCallMode];
            }
        }
        private KeyValuePair<string, int> kvpCallModeFromResume
        {
            get { return GetkvpCallMode("kvpCallModeFromResume"); }
            set { ViewState["kvpCallModeFromResume"] = value; }
        }

        public XExpense pXe { get; private set; }
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
                        case "BadCallMode":
                            Response.Redirect("Expenses.aspx");
                            break;
                        case "CancelClose":
                            DoCancel(false);
                            break;
                        case "CancelNew":
                            DoCancel(true);
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
            if (Session["CallMode"] == null)
            {
                Session["CallMode"] = kvpCallMode;
            }
            if (!IsPostBack)
            {
                bool bShowXactTime = AccountProfile.CurrentUser.bShowXactTime;
                txbXactTime.Visible = bShowXactTime;
                txbXactOffset.Visible = bShowXactTime;
                lblDateAndTime.Text =bShowXactTime ? "Date and time when the expense occurred:" : "Date when the expense occurred:";
                kvpCallMode = (KeyValuePair<string, int>)Session["CallMode"];
                Session["CallMode"] = new KeyValuePair<string, int>("NEW", 0);
                switch (kvpCallMode.Key) {
                    case "NEW":
                        PrepNew();
                        break;
                    case "INSPECT":
                    case "EDIT":
                        PrepInspectEdit();
                        break;
                    case "RESUME":
                        kvpCallModeFromResume = kvpCallMode;
                        PrepResume();
                        break;
                    default:
                        OkButton.CommandArgument = "BadCallMode";
                        ProcessPopupException(new Global.excToPopup("Accounting.FinDetails.ExpVendAP.XactExpense.aspx.cs.Page_Load: bad CallMode " + kvpCallMode.Key));
                        return;
                }
            }
            FocusOn();
        }
        private void PrepNew()
        {
            lblNewEdit.Text = "Create New Expense Transaction";
            DateTime DXact = DateTime.Today;
            txbXactDate.Text = DXact.Year.ToString("0000") + "-" + DXact.Month.ToString("00") + "-" + DXact.Day.ToString("00");
            txbXactOffset.Text = mCRUD.GetSetting("TimeZoneOffset");
            gvInits(AssistLi.Init(Global.enLL.Expenditure),
                    AssistLi.Init(Global.enLL.Payment),
                    AssistLi.Init(Global.enLL.AttachedFiles));
            Label lblAFN = (Label)gvAttach.Rows[gvAttach.Rows.Count - 1].FindControl("lblOldAttFile");
            lblAFN.Text = scNoFileSelected;
        }
        private void PrepInspectEdit()
        {
            lblNewEdit.Text = "Existing Expense Transaction";
            switch (kvpCallMode.Key)
            {
                case "INSPECT":
                    lblNewEdit.Text = "Inspect " + lblNewEdit.Text;
                    break;
                case "EDIT":
                    lblNewEdit.Text = "Modify " + lblNewEdit.Text;
                    break;
            }
            int iXactId = kvpCallMode.Value;
            lblXactId.Text = iXactId.ToString();
            sf_AccountingDataContext dc = new sf_AccountingDataContext();
            var q0 = from x in dc.SF_XACTs where x.ID == iXactId select x;
            var y = q0.First();
            DateTimeOffset DXactOffset = y.D;
            txbXactDate.Text = DXactOffset.Year.ToString("0000") + "-" + DXactOffset.Month.ToString("00") + "-" + DXactOffset.Day.ToString("00");
            txbXactTime.Text = DXactOffset.Hour.ToString("00") + ":" + DXactOffset.Minute.ToString("00");
            txbXactOffset.Text = DXactOffset.Offset.ToString().Substring(0, 6);
            txbMemo.Text = y.sMemo;
            string sStatus = "Unknown";
            switch (y.cStatus)
            {
                case 'A': sStatus = "Active"; break;
                case 'D': sStatus = "Deleted"; break;
                case 'V': sStatus = "Voided"; break;
                case 'R': sStatus = "Replaced"; break;
                case 'T': sStatus = "Template Only"; break;
            }
            lblNewEdit.Text += " with Status `" + sStatus + "`";

            gvInits(AssistLi.Init(Global.enLL.Expenditure, iXactId), 
                    AssistLi.Init(Global.enLL.Payment, iXactId), 
                    AssistLi.Init(Global.enLL.AttachedFiles, iXactId));
            Label lblAFN = (Label)gvAttach.Rows[gvAttach.Rows.Count - 1].FindControl("lblOldAttFile");
            lblAFN.Text = scNoFileSelected;
        }
        private void PrepResume()
        {
            Page lastPage = (Page)Context.Handler;
            if (lastPage is FilterSortAttFiles)
            {
                pXe = ((FilterSortAttFiles)lastPage).pXe;
                slVendor = pXe.sVendor;
            }
            else
            {
                ProcessPopupException(new Global.excToPopup("Accounting.FinDetails.ExpVendAP.XactExpense.PrepResume: Context.Handler did not provide a page of type FilterSortAttFiles"));
                PrepNew();
                return;
            }
            txbXactDate.Text = pXe.Dxact.Date.Year.ToString("0000")+"-"+ pXe.Dxact.Date.Month.ToString("00")+"-"+ pXe.Dxact.Date.Day.ToString("00");
            txbXactTime.Text = pXe.Dxact.TimeOfDay.ToString().Substring(0,5);
            txbXactOffset.Text = pXe.Dxact.Offset.ToString().Substring(0,6);
            lblXactId.Text = pXe.kvpXCallMode.Value.ToString();
            txbMemo.Text = pXe.sMemo;

            gvInits(pXe.liExpenditure, 
                    pXe.liPayment, 
                    pXe.liAttachedFile);
            Label lblAFN = (Label)gvAttach.Rows[gvAttach.Rows.Count - 1].FindControl("lblOldAttFile");
            lblAFN.Text= (string)pXe.liAttachedFile[pXe.liAttachedFile.Count - 1][(int)Global.enPFAtt.AttachedFileName];
            AccordionExpense.SelectedIndex = 4;
        }

        private void gvInits(List<DataRow> liuExp, List<DataRow> liuPmt, List<DataRow> liuAtt)
        {
            Session["liExpenditure"] = liuExp;
            DataTable dtExpenditure = liuExp.CopyToDataTable();
            dt_BindToGV(dtExpenditure, Global.enLL.Expenditure);
            GridViewEditEventArgs gvee = new GridViewEditEventArgs(gvEExp.Rows.Count - 1);
            gvEExp_RowEditing(null, gvee);

            Session["liPayment"] = liuPmt;
            DataTable dtPayment = liuPmt.CopyToDataTable();
            dt_BindToGV(dtPayment, Global.enLL.Payment);
            GridViewEditEventArgs gvep = new GridViewEditEventArgs(gvEPmt.Rows.Count - 1);
            gvEPmt_RowEditing(null, gvep);

            Session["liAttachedFiles"] = liuAtt;
            DataTable dtAttachedFiles = liuAtt.CopyToDataTable();
            dt_BindToGV(dtAttachedFiles, Global.enLL.AttachedFiles);
            GridViewEditEventArgs gvea = new GridViewEditEventArgs(gvAttach.Rows.Count - 1);
            gvAttach_RowEditing(null, gvea);
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                switch (kvpCallMode.Key)
                {
                    case "NEW":
                    case "EDIT":
                    case "RESUME":
                        break;
                    case "INSPECT":
                        tblXactDateVendor.BorderColor = System.Drawing.Color.FromName("Black");
                        tblXactDateVendor.BorderStyle = BorderStyle.Solid;
                        tblXactDateVendor.BorderWidth = 1;
                        txbXactDate.Enabled = false;
                        txbXactTime.Enabled = false;
                        txbXactOffset.Enabled = false;
                        lblNote.Visible = false;
                        txbMemo.Enabled = false;
                        txbMemo.BorderColor = System.Drawing.Color.FromName("Black");
                        txbMemo.BorderStyle = BorderStyle.Solid;
                        txbMemo.BorderWidth = 1;
                        pbSaveClose.Visible = false;
                        pbSaveNew.Visible = false;
                        break;
                }
            }

            // Calculate and display the difference between the sum of credits and the sum of debits
            List<DataRow> liExpenditure = (List<DataRow>)Session["liExpenditure"];
            string sDebits = AssistLi.Sum(liExpenditure, Global.enLL.Expenditure);
            List<DataRow> liPayment = (List<DataRow>)Session["liPayment"];
            string sCredits = AssistLi.Sum(liPayment, Global.enLL.Payment);
            string sDiff = (Convert.ToDecimal(sCredits) - Convert.ToDecimal(sDebits)).ToString("#0.00");
            SetAccordionPaneHeaderText(Global.enLL.DiffCredDeb, sDiff);
        }

        protected void DDL_Vendor_PreRender(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                switch (kvpCallMode.Key)
                {
                    case "NEW":
                        break;
                    case "RESUME":
                        SetVendorDropDown();
                        kvpCallMode = kvpCallModeFromResume;
                        break;
                    case "EDIT":
                        SetVendorDropDown();
                        break;
                    case "INSPECT":
                        SetVendorDropDown();
                        DDL_Vendor.Enabled = false;
                        break;
                }
            }
        }
        private void SetVendorDropDown()
        {
            int iXactId = kvpCallMode.Value;
            if (iXactId > 0)
            {
                sf_AccountingDataContext dc = new sf_AccountingDataContext();
                int iVendor = (from x in dc.SF_XACTs where x.ID == iXactId select x.iVendor).First();
                string sVendor = (from v in dc.SF_VENDORs where v.ID == iVendor select v.sVendorName).First();
                SetDropDownByValue(DDL_Vendor, sVendor);
            }
            else
            {
                SetDropDownByValue(DDL_Vendor, slVendor);
            }
        }

        private void FocusOn()
        {
            GridView GV = null;
            string sControlWFocus = "";
            switch (AccordionExpense.Panes[AccordionExpense.SelectedIndex].ID)
            {
                case "AccPExpenditure":
                    GV = gvEExp;
                    sControlWFocus = "DDLEDAcct";
                    break;
                case "AccPPayment":
                    GV = gvEPmt;
                    sControlWFocus = "DDLPDAcct";
                    break;
                case "AccPMemo":
                    sControlWFocus = "";
                    break;
                case "AccPAttach":
                    GV = gvAttach;
                    sControlWFocus = "DDLACateg";
                    break;
                default:
                    sControlWFocus = "X";
                    break;
            }
            if (sControlWFocus.Length < 1)
            {
                txbMemo.Focus();
            }
            else if (sControlWFocus == "X")
            {
                Global.excToPopup ex = new Global.excToPopup("Internal software error in XactExpense.aspx.cs.FocusOn: Accordion Pane " + AccordionExpense.Panes[AccordionExpense.SelectedIndex].ID + " not found.");
                ProcessPopupException(ex);
            }
            else
            {
                ((DropDownList)GV.Rows[GV.EditIndex].FindControl(sControlWFocus)).Focus();
            }
        }

        private void dt_BindToGV(DataTable dtu, Global.enLL enuLL)
        {
            GridView GV = null;
            string sLblLineNum = "";
            string sLblAcct = ""; // name of label control displaying the account name; non-existent for attached files
            string sLblPmtMeth = "";
            int iNumNotVisSum = 4; // on the Sum line, the number of controls not to be visible on right of the Sum
            string sLblAmount = "";
            int iColAddButton = 4; // This is also the column of the edit button
            int iColSumText = 2; // the column in which the sum annotation text is displayed
            int iColAmount = -1; // In which column appears the $-amount? (not applicable to attached files)
            string sControlWFocus = ""; // when we are done with binding, which control will have the focus?
            string sipbUpdate = ""; // ID of Update image button
            string sipbCancel = ""; // ID of Cancel image button
            switch (enuLL)
            {
                case Global.enLL.Expenditure:
                    GV = gvEExp;
                    sLblLineNum = "lblEDLineNum";
                    sLblAcct = "lblEIAcct";
                    sLblAmount = "lblEIAmount";
                    iColAmount = 3;
                    sControlWFocus = "DDLEDAcct";
                    sipbUpdate = "ipbEUpdate";
                    sipbCancel = "ipbECancel";
                    break;
                case Global.enLL.Payment:
                    GV = gvEPmt;
                    sLblLineNum = "lblPDLineNum";
                    sLblAcct = "lblPIAcct";
                    sLblPmtMeth = "lblPIPmtMeth";
                    sLblAmount = "lblPIAmount";
                    iColAddButton = 5;
                    iColSumText = 3;
                    iColAmount = 4;
                    sControlWFocus = "DDLPDAcct";
                    sipbUpdate = "ipbPUpdate";
                    sipbCancel = "ipbPCancel";
                    break;
                case Global.enLL.AttachedFiles:
                    GV = gvAttach;
                    sLblLineNum = "lblADLineNum";
                    sLblAmount = "lblAIFileName"; // The count of attached files appears in the file name column in the Sum line
                    iNumNotVisSum = 2;
                    iColAddButton = 5;
                    iColSumText = 1;
                    // AttachedFiles has no MoveUp MoveDown buttons
                    sControlWFocus = "DDLACateg";
                    sipbUpdate = "ipbAUpdate";
                    sipbCancel = "ipbACancel";
                    break;
            }
            GV.DataSource = dtu;
            GV.DataBind();

            // Make a bunch of adjustments to the GridView display:

            int iLast = GV.Rows.Count - 1; // The standard editing row
            int iNextToLast = iLast - 1; // The row with the Sum of all credits or all debits

            // Next-to-last (Sum) row has little info in it:
            ((Label)GV.Rows[iNextToLast].FindControl(sLblLineNum)).Text = "Sum";
            if (sLblAcct.Length > 0)
            {
                ((Label)GV.Rows[iNextToLast].FindControl(sLblAcct)).Visible = false;
            }
            if (sLblPmtMeth.Length > 0)
            {
                ((Label)GV.Rows[iNextToLast].FindControl(sLblPmtMeth)).Visible = false;
            }
            // In the next-to-last (Sum) row, there are no controls on the right:
            for (int iCell = GV.Columns.Count - 1; iCell > GV.Columns.Count - iNumNotVisSum - 1; iCell--)
            {
                GV.Rows[iNextToLast].Cells[iCell].Visible = false;
            }

            // Last row has no button except the Edit button:
            for (int iCell = GV.Columns.Count - 1; iCell > GV.Columns.Count - iNumNotVisSum; iCell--)
            {
                GV.Rows[iLast].Cells[iCell].Visible = false;
            }

            // Set text for line number label in last row
            ((Label)GV.Rows[iLast].FindControl(sLblLineNum)).Text = "New";

            // Set things up for the Next-to-last (`sum`) row:
            if (enuLL != Global.enLL.AttachedFiles)
            {
                // control visibility of MoveUp and MovewDown buttons:
                VisPbMoveUpDown(GV);
                // Let's make the Sum bold:
                ((Label)GV.Rows[iNextToLast].FindControl(sLblAmount)).Font.Bold = true;
            }
            else  // for the case of attached files only:
            {
                ((Label)GV.Rows[iNextToLast].FindControl("lblAICateg")).Visible = false;
                Label lblDAssoc = (Label)GV.Rows[iNextToLast].FindControl("lblAIDAssoc");
                lblDAssoc.Text = "Number of files to be attached: &rarr;";
                if (kvpCallMode.Key.Equals("INSPECT"))
                {
                    lblDAssoc.Text = lblDAssoc.Text.Replace("to be ", "");
                }
                GV.Rows[iNextToLast].Cells[2].HorizontalAlign = HorizontalAlign.Right;
                ((TextBox)GV.Rows[iNextToLast].FindControl("txbAIFileName")).Height = 25;
                GV.Rows[iNextToLast].Height = 30;
            }
            // Right-align the text annotating the sum:
            GV.Rows[iNextToLast].Cells[iColSumText].HorizontalAlign = HorizontalAlign.Right;

            // If the amount column exists, right-align all the amounts:
            if (iColAmount >= 0)
            {
                foreach (GridViewRow row in GV.Rows)
                {
                    row.Cells[iColAmount].HorizontalAlign = HorizontalAlign.Right;
                }
            }

            // For the list of attached files, insert a space after the underscore character to make file names more easily readable:
            if (enuLL == Global.enLL.AttachedFiles)
            {
                for (int iRow =0; iRow < GV.Rows.Count - 2; iRow++)
                {
                    TextBox txbFN = (TextBox)GV.Rows[iRow].FindControl("txbAIFileName"); // Textbox is readonly
                    txbFN.Text = txbFN.Text.Replace("_", "_ ");
                }
            }

            // Execute this code only when a row is being edited
            if (GV.EditIndex > -1)
            {
                if (GV.EditIndex == iLast)
                {
                    // Last row has an Add button
                    ImageButton pb = (ImageButton)GV.Rows[iLast].FindControl(sipbUpdate);
                    pb.ImageUrl = "~/i/YellowAddButton.jpg";
                    GV.Rows[iLast].BackColor = System.Drawing.Color.LightGray;
                    GV.Rows[iLast].BorderStyle = BorderStyle.Ridge;
                    GV.Rows[iLast].BorderWidth = 5;
                    GV.Rows[iLast].BorderColor = System.Drawing.Color.Orange;
                    GV.Rows[iLast].Cells[iColAddButton].BorderStyle = BorderStyle.Ridge;
                    GV.Rows[iLast].Cells[iColAddButton].BorderWidth = 5;
                    GV.Rows[iLast].Cells[iColAddButton].BorderColor = System.Drawing.Color.Orange;

                    // No Cancel button in the last row
                    ImageButton pbC = (ImageButton)GV.Rows[GV.EditIndex].FindControl(sipbCancel);
                    pbC.Visible = false;

                    // In the case of AttachedFiles:
                    if (enuLL == Global.enLL.AttachedFiles)
                    {
                        // we do not allow user editing of any rows but the last
                        if (GV.Rows.Count > 2)
                        {
                            // Make Edit button invisible
                            foreach(GridViewRow row in GV.Rows)
                            {
                                if (row.RowIndex < iNextToLast)
                                {
                                    ImageButton ipb = (ImageButton)row.Cells[iColAddButton].FindControl("ipbAEdit");
                                    ipb.Visible = false;
                                }
                            }
                        }

                        // In the last row:
                        FileUpload fup = (FileUpload)GV.Rows[iLast].FindControl("fupAE");
                        Label lblOR = (Label)GV.Rows[iLast].FindControl("lblOR");
                        // The date associated with an attached file:
                        TextBox txbAEDAssoc = (TextBox)GV.Rows[iLast].FindControl("txbAEDAssoc");
                        //   If it's not a previously attached file: defaulted to equal the transaction date
                        if (kvpCallModeFromResume.Value == 0)
                        {
                            txbAEDAssoc.Text = txbXactDate.Text;
                            fup.Visible = true;
                            lblOR.Visible = true;
                        }
                        else
                        {
                            List<DataRow> liAttachedFiles = (List<DataRow>)Session["liAttachedFiles"];
                            //   A previously attached file has been selected: its date must equal that file's associated date
                            string sD = ((DateTimeOffset)liAttachedFiles[iLast][(int)Global.enPFAtt.AttachAssocDate]).ToString();
                            sD = sD.Substring(0, 10).Replace("/", "-");
                            txbAEDAssoc.Text = sD;
                            txbAEDAssoc.Enabled = false; // User cannot change this date
                            //   Similarly for the attachment category:
                            DropDownList ddlA = (DropDownList)GV.Rows[iLast].FindControl("DDLACateg");
                            sD = (string)liAttachedFiles[iLast][(int)Global.enPFAtt.AttachCategName];
                            SetDropDownByValue(ddlA, sD);
                            ddlA.Enabled = false; // user cannot change this category
                            //   Make FileUpload for new attachment disappear:
                            fup.Visible = false;
                            lblOR.Visible = false;
                        }
                    }
                }
                else // not handling the last row
                {
                    if (enuLL == Global.enLL.AttachedFiles)
                    {
                        Global.excToPopup ex = new Global.excToPopup("Internal software error in XactExpense.aspx.cs.dt_BindToGV: for AttachedFiles, EditIndex = " + GV.EditIndex.ToString() + " can only be = iLast = " + iLast.ToString());
                        ProcessPopupException(ex);
                        return;
                    }

                    // If we are editing a row other than the last, only the Update and Cancel buttons are allowed:
                    for (int iRow = 0; iRow <= iLast; iRow++)
                    {
                        if (iRow != GV.EditIndex)
                        {
                            ((ImageButton)GV.Rows[iRow].Cells[iColAddButton].Controls[1]).Visible = false;
                            ((ImageButton)GV.Rows[iRow].Cells[iColAddButton+1].Controls[0]).Visible = false;
                        }
                    }
                }

                ((DropDownList)GV.Rows[GV.EditIndex].FindControl(sControlWFocus)).Focus();

                // We see a lot less when we just inspect the data:
                switch (kvpCallMode.Key)
                {
                    case "INSPECT":
                        GV.Rows[iLast].Visible = false;
                        for (int iCol = iColAddButton; iCol < GV.Columns.Count; iCol++)
                        {
                            GV.Columns[iCol].Visible = false;
                        }
                        break;
                }
            }
        }

        private void VisPbMoveUpDown(GridView gvu) // Visibility of pushbuttons for moving rows up or down
        {
            if (gvu.Rows.Count > 2)
            {
                if (gvu.EditIndex != gvu.Rows.Count - 1)
                    // We are NOT editing the last row but some other row
                {
                    foreach (GridViewRow row in gvu.Rows)
                    {
                        ((Button)row.FindControl("pbEDMoveUp")).Visible = false;
                        ((Button)row.FindControl("pbEDMoveDown")).Visible = false;
                    }
                }
                else
                    // We are editing the last row (standard situation)
                {
                    int iLastRowM2 = gvu.Rows.Count - 3;
                    ((Button)gvu.Rows[0].FindControl("pbEDMoveUp")).Visible = false;
                    ((Button)gvu.Rows[iLastRowM2].FindControl("pbEDMoveDown")).Visible = false;
                }
            }
        }
        private void SetDropDownByValue(DropDownList ddl, string sText)
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
        private void SetAccordionPaneHeaderText(Global.enLL enuL, string sSum)
        {
            switch (enuL)
            {
                case Global.enLL.Expenditure:
                    lblAccPHExp.Text = "Expenditure Detail  --  Sum of Debits = " + sSum;
                    break;
                case Global.enLL.Payment:
                    lblAccPHPmt.Text = "Payment Detail  --  Sum of Credits = " + sSum;
                    break;
                case Global.enLL.AttachedFiles:
                    lblAccPHAtt.Text = "Attached Files  --  Number of Files = " + sSum;
                    break;
                case Global.enLL.DiffCredDeb:
                    lblDiff.Text = sSum;
                    break;
            }

        }
        #region Expenditures
        protected void gvEExp_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if ((e.Row.RowState & DataControlRowState.Edit) > 0)
                {
                    DropDownList ddlAcct = (DropDownList)e.Row.FindControl("DDLEDAcct");
                    string sD = DataBinder.Eval(e.Row.DataItem, "AccountName").ToString();
                    SetDropDownByValue(ddlAcct, sD);
                }
            }
        }

        protected void gvEExp_DataBound(object sender, EventArgs e)
        {
            int NRows = gvEExp.Rows.Count;
            // Separating grid line above Sum row:
            foreach (TableCell tcell in gvEExp.Rows[NRows - 2].Cells)
            {
                tcell.Attributes["style"] = "border-top: 2px solid black;";
            }
        }

        protected void gvEExp_RowEditing(object sender, GridViewEditEventArgs e)
        {
            gvEExp.EditIndex = e.NewEditIndex;
            List<DataRow> liExpenditure = (List<DataRow>)Session["liExpenditure"];
            DataRow dr = liExpenditure[liExpenditure.Count - 2];
            dr[(int)Global.enPFExp.sAmount] = AssistLi.Sum(liExpenditure, Global.enLL.Expenditure);
            DataTable dtExpenditure = liExpenditure.CopyToDataTable();
            dt_BindToGV(dtExpenditure, Global.enLL.Expenditure);
            SetAccordionPaneHeaderText(Global.enLL.Expenditure, dr[(int)Global.enPFExp.sAmount].ToString());
        }

        protected void gvEExp_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "MoveUp" || e.CommandName == "MoveDown")
            {
                int index = Int32.Parse((string)e.CommandArgument);
                List<DataRow> liExpenditure = (List<DataRow>)Session["liExpenditure"];
                if (e.CommandName == "MoveUp")
                {
                    AssistLi.Swap(liExpenditure, index, index - 1);
                }
                else if (e.CommandName == "MoveDown")
                {
                    AssistLi.Swap(liExpenditure, index, index + 1);
                }
                DataTable dtExpenditure = liExpenditure.CopyToDataTable();
                //dtExpenditure_BindToGV(dtExpenditure);
                dt_BindToGV(dtExpenditure, Global.enLL.Expenditure);
            }
        }

        protected void gvEExp_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            int iLast = gvEExp.Rows.Count - 1;
            gvEExp.EditIndex = iLast;
            List<DataRow> liExpenditure = (List<DataRow>)Session["liExpenditure"];
            DataRow dr = liExpenditure[liExpenditure.Count - 2];
            dr[(int)Global.enPFExp.sAmount] = AssistLi.Sum(liExpenditure, Global.enLL.Expenditure);
            DataTable dtExpenditure = liExpenditure.CopyToDataTable();
            //dtExpenditure_BindToGV(dtExpenditure);
            dt_BindToGV(dtExpenditure, Global.enLL.Expenditure);
        }

        protected void gvEExp_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            // Make a DataRow from the user's data and add it to, or replace it in, the list of data rows
            AssistLi.SRowDataExp rowdata;
            // Description
            TextBox txb =(TextBox)gvEExp.Rows[e.RowIndex].FindControl("txbEDDescr");
            string sTxb = Server.HtmlEncode(txb.Text.Trim().Replace("'", "`"));
            if (sTxb.Length < 1)
            {
                Global.excToPopup ex = new Global.excToPopup("Description must not be empty");
                ProcessPopupException(ex);
                return;
            }
            rowdata.sDescr = sTxb;
            // Amount
            txb = (TextBox)gvEExp.Rows[e.RowIndex].FindControl("txbEDAmount");
            sTxb = Server.HtmlEncode(txb.Text.Trim());
            if (sTxb.Length < 1)
            {
                Global.excToPopup ex = new Global.excToPopup("Amount must not be empty");
                ProcessPopupException(ex);
                return;
            }
            decimal dd = 0.0M;
            if (!decimal.TryParse(sTxb, out dd))
            {
                Global.excToPopup ex = new Global.excToPopup("Unable to interpret `" + txb.Text + "` as a decimal number");
                ProcessPopupException(ex);
                return;
            }
            rowdata.sAmount = dd.ToString("N2", CultureInfo.InvariantCulture);
            // Account
            DropDownList ddl = (DropDownList)gvEExp.Rows[e.RowIndex].FindControl("DDLEDAcct");
            rowdata.iAcct = Int32.Parse(ddl.Items[ddl.SelectedIndex].Value);
            rowdata.sAcct = ddl.Items[ddl.SelectedIndex].Text;
            List<DataRow> liExpenditure = (List<DataRow>)Session["liExpenditure"];
            // If we are in the last row, then we add the data. Anywhere else, we replace. Of course, we never edit the next to last row.
            int iLast = gvEExp.Rows.Count - 1;
            if (e.RowIndex == iLast)
            {
                AssistLi.Add(liExpenditure, Global.enLL.Expenditure, rowdata);
            }
            else
            {
                AssistLi.Replace(liExpenditure, Global.enLL.Expenditure, e.RowIndex, rowdata);
            }
            Session["liExpenditure"] = liExpenditure;
            DataRow dr = liExpenditure[liExpenditure.Count - 2];
            dr[(int)Global.enPFExp.sAmount] = AssistLi.Sum(liExpenditure, Global.enLL.Expenditure);
            DataTable dtExpenditure = liExpenditure.CopyToDataTable();
            gvEExp.EditIndex = dtExpenditure.Rows.Count - 1;
            dt_BindToGV(dtExpenditure, Global.enLL.Expenditure);
            SetAccordionPaneHeaderText(Global.enLL.Expenditure, dr[(int)Global.enPFExp.sAmount].ToString());
        }

        protected void gvEExp_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            List<DataRow> liExpenditure = (List<DataRow>)Session["liExpenditure"];
            AssistLi.Remove(liExpenditure, e.RowIndex);
            e.Cancel = true;
            Session["liExpenditure"] = liExpenditure;
            DataRow dr = liExpenditure[liExpenditure.Count - 2];
            dr[(int)Global.enPFExp.sAmount] = AssistLi.Sum(liExpenditure, Global.enLL.Expenditure);
            DataTable dtExpenditure = liExpenditure.CopyToDataTable();
            gvEExp.EditIndex = dtExpenditure.Rows.Count - 1;
            dt_BindToGV(dtExpenditure, Global.enLL.Expenditure);
            SetAccordionPaneHeaderText(Global.enLL.Expenditure, dr[(int)Global.enPFExp.sAmount].ToString());
        }

        protected void DDLEDAcct_PreRender(object sender, EventArgs e)
        {
            SetDropDownByValue((DropDownList)sender, mCRUD.GetSetting("DefaultExpenseAccount"));
        }
#endregion 
        #region Payment
        protected void DDLPDAcct_PreRender(object sender, EventArgs e)
        {
            SetDropDownByValue((DropDownList)sender, mCRUD.GetSetting("DefaultAssetsAccount"));
        }

        protected void DDLPDPmtMeth_PreRender(object sender, EventArgs e)
        {
            SetDropDownByValue((DropDownList)sender, mCRUD.GetSetting("DefaultPaymentMethod"));
        }

        protected void gvEPmt_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if ((e.Row.RowState & DataControlRowState.Edit) > 0)
                {
                    DropDownList ddlAcct = (DropDownList)e.Row.FindControl("DDLPDAcct");
                    string sD = DataBinder.Eval(e.Row.DataItem, "AccountName").ToString();
                    SetDropDownByValue(ddlAcct, sD);
                }
            }
        }

        protected void gvEPmt_DataBound(object sender, EventArgs e)
        {
            int NRows = gvEPmt.Rows.Count;
            // Separating grid line above Sum row:
            foreach (TableCell tcell in gvEPmt.Rows[NRows - 2].Cells)
            {
                tcell.Attributes["style"] = "border-top: 2px solid black;";
            }
        }

        protected void gvEPmt_RowEditing(object sender, GridViewEditEventArgs e)
        {
            gvEPmt.EditIndex = e.NewEditIndex;
            List<DataRow> liPayment = (List<DataRow>)Session["liPayment"];
            DataRow dr = liPayment[liPayment.Count - 2];
            dr[(int)Global.enPFExp.sAmount] = AssistLi.Sum(liPayment, Global.enLL.Payment);
            DataTable dtPayment = liPayment.CopyToDataTable();
            //dtPayment_BindToGV(dtPayment);
            dt_BindToGV(dtPayment, Global.enLL.Payment);
            SetAccordionPaneHeaderText(Global.enLL.Payment, dr[(int)Global.enPFPmt.sAmount].ToString());
        }

        protected void gvEPmt_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "MoveUp" || e.CommandName == "MoveDown")
            {
                int index = Int32.Parse((string)e.CommandArgument);
                List<DataRow> liPayment = (List<DataRow>)Session["liPayment"];
                if (e.CommandName == "MoveUp")
                {
                    AssistLi.Swap(liPayment, index, index - 1);
                }
                else if (e.CommandName == "MoveDown")
                {
                    AssistLi.Swap(liPayment, index, index + 1);
                }
                DataTable dtPayment = liPayment.CopyToDataTable();
                dt_BindToGV(dtPayment, Global.enLL.Payment);
            }
        }

        protected void gvEPmt_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            int iLast = gvEPmt.Rows.Count - 1;
            gvEPmt.EditIndex = iLast;
            List<DataRow> liPayment = (List<DataRow>)Session["liPayment"];
            DataRow dr = liPayment[liPayment.Count - 2];
            dr[(int)Global.enPFPmt.sAmount] = AssistLi.Sum(liPayment, Global.enLL.Payment);
            DataTable dtPayment = liPayment.CopyToDataTable();
            dt_BindToGV(dtPayment, Global.enLL.Payment);
        }

        protected void gvEPmt_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            // Make a DataRow from the user's data and add it to, or replace it in, the linked list of data rows
            AssistLi.SRowDataPmt rowdata;
            // Description (i.e. Reference) (ok to be empty)
            TextBox txb = (TextBox)gvEPmt.Rows[e.RowIndex].FindControl("txbPDDescr");
            rowdata.sDescr = Server.HtmlEncode(txb.Text.Trim().Replace("'", "`"));
            // Amount
            txb = (TextBox)gvEPmt.Rows[e.RowIndex].FindControl("txbPDAmount");
            string sTxb = Server.HtmlEncode(txb.Text.Trim());
            if (sTxb.Length < 1)
            {
                Global.excToPopup ex = new Global.excToPopup("Amount must not be empty");
                ProcessPopupException(ex);
                return;
            }
            decimal dd = 0.0M;
            if (!decimal.TryParse(sTxb, out dd))
            {
                Global.excToPopup ex = new Global.excToPopup("Unable to interpret `" + sTxb + "` as a decimal number");
                ProcessPopupException(ex);
                return;
            }
            rowdata.sAmount = dd.ToString("N2", CultureInfo.InvariantCulture);
            // Account
            DropDownList ddl = (DropDownList)gvEPmt.Rows[e.RowIndex].FindControl("DDLPDAcct");
            rowdata.iAcct = Int32.Parse(ddl.Items[ddl.SelectedIndex].Value);
            rowdata.sAcct = ddl.Items[ddl.SelectedIndex].Text;
            // Payment Method
            ddl = (DropDownList)gvEPmt.Rows[e.RowIndex].FindControl("DDLPDPmtMeth");
            rowdata.iPmtMethod = Int32.Parse(ddl.Items[ddl.SelectedIndex].Value);
            rowdata.sPmtMethod = ddl.Items[ddl.SelectedIndex].Text;
            List<DataRow> liPayment = (List<DataRow>)Session["liPayment"];
            // If we are in the last row, then we add the data. Anywhere else, we replace. Of course, we never edit the next to last row.
            int iLast = gvEPmt.Rows.Count - 1;
            if (e.RowIndex == iLast)
            {
                AssistLi.Add(liPayment, Global.enLL.Payment, rowdata);
            }
            else
            {
                AssistLi.Replace(liPayment, Global.enLL.Payment, e.RowIndex, rowdata);
            }
            Session["liPayment"] = liPayment;
            DataRow dr = liPayment[liPayment.Count - 2];
            dr[(int)Global.enPFPmt.sAmount] = AssistLi.Sum(liPayment, Global.enLL.Payment);
            DataTable dtPayment = liPayment.CopyToDataTable();
            gvEPmt.EditIndex = dtPayment.Rows.Count - 1;
            dt_BindToGV(dtPayment, Global.enLL.Payment);
            SetAccordionPaneHeaderText(Global.enLL.Payment, dr[(int)Global.enPFPmt.sAmount].ToString());
        }

        protected void gvEPmt_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            List<DataRow> liPayment = (List<DataRow>)Session["liPayment"];
            AssistLi.Remove(liPayment, e.RowIndex);
            e.Cancel = true;
            Session["liPayment"] = liPayment;
            DataRow dr = liPayment[liPayment.Count - 2];
            dr[(int)Global.enPFPmt.sAmount] = AssistLi.Sum(liPayment, Global.enLL.Payment);
            DataTable dtPayment = liPayment.CopyToDataTable();
            gvEPmt.EditIndex = dtPayment.Rows.Count - 1;
            dt_BindToGV(dtPayment, Global.enLL.Payment);
            SetAccordionPaneHeaderText(Global.enLL.Payment, dr[(int)Global.enPFPmt.sAmount].ToString());
        }
        #endregion
        #region Memorandum
        protected void txbMemo_TextChanged(object sender, EventArgs e)
        {
            string st = Server.HtmlEncode(txbMemo.Text.Trim().Replace("'", "`"));
            if (st.Length > 30)
            {
                st = st.Substring(0, 30) + " ...";
            }
            lblAccPHMem.Text = "Memorandum  --  " + st;
        }
        #endregion
        #region Attached Files

        protected void gvAttach_DataBound(object sender, EventArgs e)
        {
            int NRows = gvAttach.Rows.Count;
            // Separating grid line above Sum row:
            foreach (TableCell tcell in gvAttach.Rows[NRows - 2].Cells)
            {
                tcell.Attributes["style"] = "border-top: 2px solid black;";
            }
        }

        protected void gvAttach_RowEditing(object sender, GridViewEditEventArgs e)
        {
            gvAttach.EditIndex = e.NewEditIndex;
            List<DataRow> liAttachedFiles = (List<DataRow>)Session["liAttachedFiles"];
            DataRow dr = liAttachedFiles[liAttachedFiles.Count - 2];
            dr[(int)Global.enPFAtt.AttachedFileName] = AssistLi.Sum(liAttachedFiles, Global.enLL.AttachedFiles);
            DataTable dtAttachedFiles = liAttachedFiles.CopyToDataTable();
            dt_BindToGV(dtAttachedFiles, Global.enLL.AttachedFiles);
            SetAccordionPaneHeaderText(Global.enLL.AttachedFiles, dr[(int)Global.enPFAtt.AttachedFileName].ToString());
        }

        protected void DDLACateg_PreRender(object sender, EventArgs e)
        {
            if (kvpCallModeFromResume.Key == "RESUME")
            {
                // In the page life cycle, this is the last use of kvpCallModeFromResume before page is rendered on display screen;
                //    the RESUME process is thereby completed and kvpCallModeFromResume needs to be reset:
                kvpCallModeFromResume = new KeyValuePair<string, int>("NEW", 0);
            }
            else
            {
                SetDropDownByValue((DropDownList)sender, mCRUD.GetSetting("DefaultAttachmentCateg"));
            }
        }

        protected void gvAttach_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            List<DataRow> liAttachedFiles = (List<DataRow>)Session["liAttachedFiles"];
            // The file has already been uploaded; now we need to delete it in turn:
            DataRow dr = liAttachedFiles[e.RowIndex];
            string sMapPath = Server.MapPath(Global.scgRelPathUpload);
            File.Delete(sMapPath + "\\" + dr[(int)Global.enPFAtt.AttachedFileName]);
            AssistLi.Remove(liAttachedFiles, e.RowIndex);
            e.Cancel = true;
            Session["liAttachedFiles"] = liAttachedFiles;
            dr = liAttachedFiles[liAttachedFiles.Count - 2];
            dr[(int)Global.enPFAtt.AttachedFileName] = AssistLi.Sum(liAttachedFiles, Global.enLL.AttachedFiles);
            DataTable dtAttachedFiles = liAttachedFiles.CopyToDataTable();
            gvAttach.EditIndex = dtAttachedFiles.Rows.Count - 1;
            dt_BindToGV(dtAttachedFiles,Global.enLL.AttachedFiles);
            SetAccordionPaneHeaderText(Global.enLL.AttachedFiles, dr[(int)Global.enPFAtt.AttachedFileName].ToString());
        }

        protected void gvAttach_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            // Make a DataRow from the user's data and add it to the list of data rows
            // If we are in the last row, then we add the data. Anywhere else is an error:
            int iLast = gvAttach.Rows.Count - 1;
            if (e.RowIndex != iLast)
            {
                Global.excToPopup ex = new Global.excToPopup("Internal software error in XactExpense.aspx.cs: gvAttach.RowIndex = "
                    + e.RowIndex.ToString() + " != iLast = " + iLast.ToString());
                ProcessPopupException(ex);
                return;
            }
            AssistLi.SRowDataAtt rowdata;
            rowdata.sFile = "";

            // Category of Attachment:
            DropDownList ddl = (DropDownList)gvAttach.Rows[iLast].FindControl("DDLACateg");
            rowdata.iCateg = Int32.Parse(ddl.Items[ddl.SelectedIndex].Value);
            rowdata.sCateg = ddl.Items[ddl.SelectedIndex].Text;

            // Date of Document, or date associated with file:
            TextBox txbD = (TextBox)gvAttach.Rows[iLast].FindControl("txbAEDAssoc"); // Textmode = Date
            // Date associated with file/document has a constant time of day: local noon
            DateTimeOffset DAssoc = DateTimeOffset.Parse(txbD.Text + " 12:00 " + mCRUD.GetSetting("TimeZoneOffset"));
            rowdata.DAssoc = DAssoc;
            DataRow dr;
            List<DataRow> liAttachedFiles = (List<DataRow>)Session["liAttachedFiles"];
            FileUpload fup = (FileUpload)gvAttach.Rows[iLast].FindControl("fupAE");
            Table tblFN = (Table)gvAttach.Rows[iLast].FindControl("tblFN");
            TableRow tblFNRow1 = (TableRow)tblFN.FindControl("tblFNRow1");
            TableCell tblFNRow1Cell1 = (TableCell)tblFNRow1.FindControl("tblFNRow1Cell1");
            Label lblOldAttFile = (Label)tblFNRow1Cell1.FindControl("lblOldAttFile");
            bool bOldAttFile = false;
            if (!fup.HasFile)
            {
                if(lblOldAttFile.Text.Equals(scNoFileSelected, StringComparison.OrdinalIgnoreCase))
                {
                    Global.excToPopup ex = new Global.excToPopup("No file has been specified for uploading or re-attaching");
                    ProcessPopupException(ex);
                    return;
                }
                else
                {
                    bOldAttFile = true;
                }
            }
            if (!bOldAttFile)
            {
                // Validate file name
                string sFup = Server.HtmlEncode(fup.FileName);
                //    Length
                int iL = sFup.Length;
                if (iL > 145)
                {
                    Global.excToPopup ex = new Global.excToPopup("Name of file to be uploaded may not be longer than 145 characters; current length = " + iL.ToString());
                    ProcessPopupException(ex);
                    return;
                }
                //    File name:
                Match result = Regex.Match(Server.HtmlEncode(sFup), "[A-Z]{1}[a-zA-Z0-9_]{1,141}\\.[a-zA-Z0-9]{1,10}");
                if (!result.Success)
                {
                    Global.excToPopup ex = new Global.excToPopup("File name `" + Server.HtmlEncode(fup.PostedFile.FileName) + "` does not follow all of these rules: " +
                        "(1) must start with upper case alphabetic character, " +
                        "(2) allowed characters are English alphabetic lower and upper case, digits 0 through 9, and underscore (but no blanks or spaces), " +
                        "(3) without file type, can be up to 142 characters long, " +
                        "(4) there must be one and only one period, " +
                        "(5) the file type/extension without the period must be from 1 to 10 characters long, no underscore.");
                    ProcessPopupException(ex);
                    return;
                }
                //    File type/extension
                string sFType = Path.GetExtension(sFup).ToLower();
                sf_AccountingDataContext dc = new sf_AccountingDataContext();
                int iCount = (from a in dc.SF_ALLOWEDATTACHTYPEs where a.sAllowedFileType == sFType select a).Count();
                if (iCount != 1)
                {
                    var Q = from t in dc.SF_ALLOWEDATTACHTYPEs select t.sAllowedFileType;
                    string st = "";
                    foreach (string q in Q)
                    {
                        st += "  " + q;
                    }
                    Global.excToPopup ex = new Global.excToPopup("The file type `" + sFType + "` is not among the ones that are allowed for files to be attached: " +
                        st + " (search count=" + iCount.ToString() + ")");
                    ProcessPopupException(ex);
                    return;
                }
                //    File size
                iL = fup.PostedFile.ContentLength;
                var Q1 = (from s in dc.SF_ALLOWEDATTACHTYPEs where sFType == s.sAllowedFileType select new { s.iMinBytes, s.iMaxBytes }).First();
                if (iL < Q1.iMinBytes || iL > Q1.iMaxBytes)
                {
                    Global.excToPopup ex = new Global.excToPopup("The size of file `" + sFup + "` = " + iL.ToString() +
                        " bytes is not between " + Q1.iMinBytes.ToString() + " and " + Q1.iMaxBytes.ToString());
                    ProcessPopupException(ex);
                    return;
                }

                rowdata.sFile = sFup;
                rowdata.iFile = 0; // zero means file has not yet been stored in table SF_DOCS
                rowdata.iThumb = 0; // TBD !!
                rowdata.sThumb = " "; // TBD!
                // Search for duplicate file name within the files to be attached by this Xact:
                foreach (DataRow datrow in liAttachedFiles)
                {
                    if ((int)datrow.ItemArray[0] <= 0) break; // Don't check the bottom two rows
                    string sNameWithPath = (string)datrow.ItemArray[(int)Global.enPFAtt.AttachedFileName];
                    string sName = Path.GetFileName(sNameWithPath);
                    if (sName.Substring(30) == sFup)
                    {
                        //Insert a blank every 30 characters so the string will be split and wrapped over multiple lines
                        string ss = sFup;
                        for (int i = ss.Length - 31; i > 2; i -= 30)
                        {
                            ss = ss.Substring(0, i) + " " + ss.Substring(i);
                        }
                        Global.excToPopup ex = new Global.excToPopup("File `" + ss + "` is already in the list of files to be attached");
                        ProcessPopupException(ex);
                        return;
                    }
                }
                // Validation is complete
                // Prepend time stamp to filename
                string savePath = Server.MapPath(Global.scgRelPathUpload);
                DateTimeOffset Dts = DateTimeOffset.Now;
                string sts = Dts.ToString("yyyyMMddHHmmssK");
                // Hexadecimal user id:
                string sxUser = mCRUD.GetPeopleIDfromWebSiteUserName(HttpContext.Current.User.Identity.Name).ToString("x8");
                sts = "F" + sts.Replace("+", "p").Replace("-", "m").Replace(":", "") + "x" + sxUser + "_" + rowdata.sFile;
                savePath += "\\" + sts;
                // Save on server:
                try
                {
                    fup.SaveAs(savePath);
                }
                catch (PathTooLongException pte)
                {
                    Global.excToPopup ex = new Global.excToPopup(pte.Message);
                    ProcessPopupException(ex);
                    return;
                }
                rowdata.sFile = sts; // file name includes prepended time stamp and hex user id
            }
            else
            {
                // Dealing with a previously attached file
                dr = liAttachedFiles[liAttachedFiles.Count - 1];
                rowdata.iFile = (int)dr[(int)Global.enPFAtt.AttachedFileID];
                rowdata.sFile=(string)dr[(int)Global.enPFAtt.AttachedFileName];
                rowdata.iThumb = 0; // TBD !!
                rowdata.sThumb = " "; // TBD!
            }
            AssistLi.Add(liAttachedFiles, Global.enLL.AttachedFiles, rowdata);
            // Reset the last row
            List<DataRow> tmpLiAtt = AssistLi.Init(Global.enLL.AttachedFiles);
            liAttachedFiles[liAttachedFiles.Count - 1] = tmpLiAtt.Last();

            Session["liAttachedFiles"] = liAttachedFiles;
            dr = liAttachedFiles[liAttachedFiles.Count - 2]; // the 'Sum' row
            dr[(int)Global.enPFAtt.AttachedFileName] = AssistLi.Sum(liAttachedFiles, Global.enLL.AttachedFiles); // number of files to be attached
            DataTable dtAttachedFiles = liAttachedFiles.CopyToDataTable();
            gvAttach.EditIndex = dtAttachedFiles.Rows.Count - 1;
            dt_BindToGV(dtAttachedFiles, Global.enLL.AttachedFiles);
            SetAccordionPaneHeaderText(Global.enLL.AttachedFiles, dr[(int)Global.enPFAtt.AttachedFileName].ToString());
        }
        #endregion
        #region Save and Cancel Buttons
        protected void pbSaveClose_Click(object sender, EventArgs e)
        {
            if (bpbSave())
            {
                Response.Redirect("Expenses.aspx");
            }
        }

        protected void pbSaveNew_Click(object sender, EventArgs e)
        {
            if (bpbSave())
            {
                kvpCallMode = new KeyValuePair<string, int>("NEW", 0);
                Server.TransferRequest(Request.Url.AbsolutePath, false);
            }
        }
        private bool bpbSave()
        {
            XactEng XA;
            try
            {
                pXe = FillXExpense(true);
                XA = new XactEng(pXe);
            }
            catch (Global.excToPopup exctp)
            {
                ProcessPopupException(exctp);
                return false;
            }
            try
            {
                XA.Save(kvpCallMode.Value); // kvpCallMode.Value is > 0 when a transaction is being modified, i.e., replaced
                return true;
            }
            catch (XactEng.excXactEng excX)
            {
                Global.excToPopup ex = new Global.excToPopup(excX.sExcMsg());
                ProcessPopupException(ex);
                return false;
            }
        }
        private XExpense FillXExpense(bool buSave)
        {
            DateTimeOffset DXact = DateTimeOffset.MinValue;
            string sDate = txbXactDate.Text + " " + txbXactTime.Text + " " + txbXactOffset.Text;
            if (!DateTimeOffset.TryParse(sDate, out DXact))
            {
                throw new Global.excToPopup("Internal software error in XactExpense.aspx.cs: pbSaveClose_Click.sDate: badly formed input to DateTimeOffset.TryParse = '" + sDate + "'");
            }
            string sVendor = DDL_Vendor.SelectedItem.Text;
            List<DataRow> liExp = (List<DataRow>)Session["liExpenditure"];
            List<DataRow> liPay = (List<DataRow>)Session["liPayment"];
            string sMemo = txbMemo.Text.Replace("'", "`");
            List<DataRow> liAtt = (List<DataRow>)Session["liAttachedFiles"];
            if (!buSave)
            {
                // When we are not about to save the Expense Xact, then we are about to select a file to attach which already exists in table SF_DOCS;
                //   need to remember contents of the last row of gvAttach:
                int iRow = liAtt.Count - 1;
                DataRow dr = liAtt[iRow];
                DropDownList ddlACateg = (DropDownList)gvAttach.Rows[iRow].FindControl("DDLACateg");
                dr[(int)Global.enPFAtt.AttachCategID] = ddlACateg.SelectedValue;
                dr[(int)Global.enPFAtt.AttachCategName] = ddlACateg.Text;
                TextBox txbD = (TextBox)gvAttach.Rows[iRow].FindControl("txbAEDAssoc");
                // Date associated with file/document has a constant time of day: local noon
                DateTimeOffset DAssoc = DateTimeOffset.Parse(txbD.Text + " 12:00 " + mCRUD.GetSetting("TimeZoneOffset"));
                dr[(int)Global.enPFAtt.AttachAssocDate] = DAssoc;
                liAtt[iRow] = dr;
            }
            XExpense xExp = new XExpense
            {
                kvpXCallMode = kvpCallMode,
                Dxact = DXact,
                sVendor = sVendor,
                liExpenditure = liExp,
                liPayment = liPay,
                sMemo = sMemo,
                liAttachedFile = liAtt
            };
            return xExp;
        }

        protected void pbCancelClose_Click(object sender, EventArgs e)
        {
            if (kvpCallMode.Key != "INSPECT")
            {
                ButtonsClear();
                lblPopupText.Text = "Please confirm canceling work on this expense:";
                OkButton.CommandArgument = "CancelClose";
                YesButton.CommandName = "Delete";
                MPE_Show(Global.enumButtons.NoYes);
            }
            else
            {
                DoCancel(false);
            }
        }

        protected void pbCancelNew_Click(object sender, EventArgs e)
        {
            if (kvpCallMode.Key != "INSPECT")
            {
                ButtonsClear();
                lblPopupText.Text = "Please confirm canceling work on this expense:";
                OkButton.CommandArgument = "CancelNew";
                YesButton.CommandName = "Delete";
                MPE_Show(Global.enumButtons.NoYes);
            }
            else
            {
                DoCancel(true);
            }
        }
        private void DoCancel(bool buNew)
        {
            DeleteAbandonedFiles();
            if (buNew)
            {
                kvpCallMode = new KeyValuePair<string, int>("NEW", 0);
                Server.TransferRequest(Request.Url.AbsolutePath, false);
            }
            else
            {
                Response.Redirect("Expenses.aspx");
            }

        }

        private void DeleteAbandonedFiles()
        {
            if (kvpCallMode.Key != "INSPECT")
            {
                List<DataRow> liAttachedFiles = (List<DataRow>)Session["liAttachedFiles"];
                DataTable dtAttachedFiles = liAttachedFiles.CopyToDataTable();
                foreach (DataRow dr in dtAttachedFiles.Rows)
                {
                    if ((int)dr[(int)Global.enPFAtt.LineNum] > 0)
                    {
                        string sPath = Server.MapPath("~");
                        sPath = sPath.Substring(0, sPath.Length - 1); // remove trailing /
                        sPath += Global.scgRelPathUpload + "/" + (string)dr[Global.enPFAtt.AttachedFileName.ToString()];
                        File.Delete(sPath);
                    }
                }
            }
        }
        #endregion

        protected void pbAttFile_Click(object sender, EventArgs e)
        {
            // In preparation of returning from FilterSortAttFiles without setting CallMode to RESUME:
            Session["CallMode"] = new KeyValuePair<string, int>("NEW", 0);
            // If FilterSortAttFiles does not modify the Session variable CallMode to 'RESUME',
            //    then this page (XactExpense) starts a new Expense transaction, i.e., previously entered data are lost.
            //    This should avoid confusion over left-over lists in Session variables.

            pXe = FillXExpense(false); // pXe being a public property of XactExpense.aspx, it will be available via Context.Handler after a Server.Transfer().
            Session["liExpenditure"] = null;
            Session["liPayment"] = null;
            Session["liAttachedFiles"] = null;

            Server.Transfer("~/Accounting/FinDetails/FilterSortAttFiles.aspx", true);
        }
    }
}