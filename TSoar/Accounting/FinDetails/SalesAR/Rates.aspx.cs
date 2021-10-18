using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TSoar.Accounting.FinDetails.SalesAR
{
    public partial class Rates : System.Web.UI.Page
    {
        public Dictionary<int, string> dictEquipTypes;
        public Dictionary<int, string> dictLaunchMethods;
        public Dictionary<int, string> dictFAItems;
        public Dictionary<int, int> dictFAPmtTerms;
        public Dictionary<int, string> dictAccItems;

        #region ipRate
        private int ipRate { get { return iGetpRate("ipRate"); } set { ViewState["ipRate"] = value; } }
        private int iGetpRate(string supRate)
        {
            if (ViewState[supRate] == null)
            {
                return 0;
            }
            else
            {
                return (int)ViewState[supRate];
            }
        }
        #endregion

        protected void Page_PreInit(object sender, EventArgs e)
        {
            TSoar.DB.sf_AccountingDataContext dc = new TSoar.DB.sf_AccountingDataContext();
            dictEquipTypes = new Dictionary<int, string>();
            var qy = from y in dc.EQUIPTYPEs select y;
            foreach (var y in qy)
            {
                dictEquipTypes.Add(y.ID, y.sEquipmentType);
            }
            dictLaunchMethods = new Dictionary<int, string>();
            var qz = from z in dc.LAUNCHMETHODs select z;
            foreach (var z in qz)
            {
                dictLaunchMethods.Add(z.ID, z.sLaunchMethod);
            }
            dictFAItems = new Dictionary<int, string>();
            var qf = from j in dc.FA_ITEMs select j;
            foreach (var j in qf)
            {
                dictFAItems.Add(j.ID, j.sFA_ItemCode);
            }
            dictFAPmtTerms = new Dictionary<int, int>();
            var qp = from j in dc.FA_PMTTERMs select j;
            foreach (var j in qp)
            {
                dictFAPmtTerms.Add(j.ID, j.iPmtTermsCode);
            }

            dictAccItems = new Dictionary<int, string>();
            var qi = from i in dc.QBO_ITEMNAMEs select i;
            foreach (var i in qi)
            {
                dictAccItems.Add(i.ID, i.sQBO_ItemName);
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
            try
            {
                if ((btn.ID == "YesButton") && (btn.CommandName == "Delete"))
                {
                    switch (OkButton.CommandArgument)
                    {
                        case "Rate":
                            // Delete the Rate record
                            TSoar.DB.sf_AccountingDataContext dc = new TSoar.DB.sf_AccountingDataContext();
                            var r = (from v in dc.RATEs where v.ID == Int32.Parse(btn.CommandArgument) select v).First();
                            dc.RATEs.DeleteOnSubmit(r);
                            try
                            {
                                dc.SubmitChanges();
                            }
                            catch (Exception exc)
                            {
                                Global.excToPopup ex = new Global.excToPopup(exc.Message);
                                ProcessPopupException(ex);
                            }
                            finally
                            {
                                FillDataTable();
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
                FillDataTable();
            }
            foreach (string st in new string[] { "txbDShortName", "txbSingleDpUse", "txbNoChrg1st", "txbAltDiffDpFt", "txbDurationDpMin" })
            {

                TextBox txb = (TextBox)gvRates.Rows[gvRates.Rows.Count - 1].FindControl(st);
                if (txb != null)
                {
                    txb.Attributes.Add("onfocus", "this.select();");
                }
            }
        }

        private void FillDataTable()
        {
            List<DataRow> liRates = AssistLi.Init(Global.enLL.Rates);
            Session["liRates"] = liRates;
            DataTable dtRates = liRates.CopyToDataTable();
            GridViewEditEventArgs gvee = new GridViewEditEventArgs(dtRates.Rows.Count - 1);
            gvRates_RowEditing(null, gvee);
        }

        protected void DDL_PreRender(object sender, EventArgs e)
        {
            DropDownList ddl = (DropDownList)sender;
            if (gvRates.EditIndex > -1)
            {
                string sText = "";
                TSoar.DB.SCUD_Multi mCrud = new TSoar.DB.SCUD_Multi();
                TSoar.DB.sf_AccountingDataContext dc = new TSoar.DB.sf_AccountingDataContext();
                int ii = Int32.Parse(((Label)gvRates.Rows[gvRates.EditIndex].FindControl("lblIIdent")).Text); // ID of RATES record
                switch (ddl.ID)
                {
                    case "DDLEquipType":
                        if (ii == 0)
                        {
                            sText = mCrud.GetSetting("DefaultEquipmentType");
                        }
                        else
                        {
                            ii = (from r in dc.RATEs where r.ID == ii select r.iEquipType).First(); // ID of EQUIPTYPES record
                            sText = dictEquipTypes[ii];
                        }
                        SetDropDownByText(ddl, sText);
                        break;
                    case "DDLLaunchMethod":
                        if (ii == 0)
                        {
                            sText = mCrud.GetSetting("DefaultLaunchMethod");
                        }
                        else
                        {
                            ii = (from r in dc.RATEs where r.ID == ii select r.iLaunchMethod).First(); // ID of LAUNCHMETHODS record
                            sText = dictLaunchMethods[ii];
                        }
                        SetDropDownByText(ddl, sText);
                        break;
                    case "DDLFAItem":
                        if (ii == 0)
                        {
                            sText = mCrud.GetSetting("DefaultFA_item_code");
                        }
                        else
                        {
                            ii = (from r in dc.RATEs where r.ID == ii select r.iFA_Item).First();
                            sText = dictFAItems[ii];
                        }
                        SetDropDownByText(ddl, sText);
                        break;
                    case "DDLFAPmtTerm":
                        if (ii == 0)
                        {
                            sText = mCrud.GetSetting("DefaultFA_PaymentTerm");
                        }
                        else
                        {
                            ii = (from r in dc.RATEs where r.ID == ii select r.iFA_PmtTerm).First();
                            sText = dictFAPmtTerms[ii].ToString();
                        }
                        SetDropDownByText(ddl, sText);
                        break;
                    case "DDLAccItem":
                        if (ii == 0)
                        {
                            sText = mCrud.GetSetting("DefaultQBO_AccItem");
                        }
                        else
                        {
                            ii = (from r in dc.RATEs where r.ID == ii select r.iQBO_ItemName).First(); // ID of QBO_ITEMNAMES record
                            sText = dictAccItems[ii];
                        }
                        SetDropDownByText(ddl, sText);
                        break;
                }
            }
        }
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

        protected void gvRates_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (e.Row.RowIndex == gvRates.Rows.Count - 1)
                {
                    // We are dealing with the last row of gvRates
                    
                }
            }
        }

        protected void gvRates_RowEditing(object sender, GridViewEditEventArgs e)
        {
            gvRates.EditIndex = e.NewEditIndex;
            List<DataRow> liRates = (List<DataRow>)Session["liRates"];
            DataTable dtRates = liRates.CopyToDataTable();
            dt_BindToGV(dtRates);
        }

        private void dt_BindToGV(DataTable dtu)
        {
            GridView GV = gvRates;
            DataView view = new DataView(dtu);
            view.Sort = "i1Sort ASC, sShortName ASC, DFrom ASC";
            GV.DataSource = view;
            GV.DataBind();
            int iLast = GV.Rows.Count - 1; // The standard editing row
            // Last row has no button except the Edit button:
            GV.Rows[iLast].Cells[GV.Columns.Count - 1].Visible = false;
            // Last row has an Add button
            int iColAddButton = GV.Columns.Count - 2;
            ImageButton pb = (ImageButton)GV.Rows[iLast].Cells[iColAddButton].Controls[0];
            pb.ImageUrl = "~/i/YellowAddButton.jpg";
            GV.Rows[iLast].BackColor = System.Drawing.Color.LightGray;
            GV.Rows[iLast].BorderStyle = BorderStyle.Ridge;
            GV.Rows[iLast].BorderWidth = 5;
            GV.Rows[iLast].BorderColor = System.Drawing.Color.Orange;
            GV.Rows[iLast].Cells[iColAddButton].BorderStyle = BorderStyle.Ridge;
            GV.Rows[iLast].Cells[iColAddButton].BorderWidth = 5;
            GV.Rows[iLast].Cells[iColAddButton].BorderColor = System.Drawing.Color.Orange;
            // Execute this code only when a row is being edited
            if (GV.EditIndex > -1)
            {
                if (GV.EditIndex == iLast)
                {
                    // No Cancel button in the last row
                    ImageButton pbC = (ImageButton)GV.Rows[GV.EditIndex].Cells[iColAddButton].Controls[2];
                    pbC.Visible = false;
                    Button pbEChargeCodes = (Button)GV.Rows[iLast].FindControl("pbEChargeCodes");
                    pbEChargeCodes.Text = "See Note (1)";
                    pbEChargeCodes.Font.Size = 8;
                    pbEChargeCodes.Enabled = false;
                }
                else
                {
                    // If we are editing a row other than the last, only the Update and Cancel buttons are allowed:
                    for (int iRow = 0; iRow <= iLast; iRow++)
                    {
                        if (iRow != GV.EditIndex)
                        {
                            ((ImageButton)GV.Rows[iRow].Cells[iColAddButton].Controls[0]).Visible = false;
                            ((ImageButton)GV.Rows[iRow].Cells[iColAddButton + 1].Controls[0]).Visible = false;
                        }
                    }
                }
            }
        }

        protected void gvRates_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            Label lblItem = (Label)gvRates.Rows[e.RowIndex].FindControl("lblIIdent");
            string sItem = lblItem.Text;
            ButtonsClear();
            YesButton.CommandName = "Delete";
            YesButton.CommandArgument = sItem;
            OkButton.CommandArgument = "Rate";
            lblPopupText.Text = 
                "Please confirm deletion of Rates record with ID " + sItem;
            MPE_Show(Global.enumButtons.NoYes);
        }

        protected void gvRates_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvRates.EditIndex = -1;
            FillDataTable();
        }

        protected void gvRates_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            string sLog = "";
            ActivityLog.enumLogTypes elt = ActivityLog.enumLogTypes.DataInsert;
            FlyActInvoiceDataContext dc = new FlyActInvoiceDataContext();
            // Validate inputs: It is the user's responsibility to make sure the rate numbers make sense
            string sShortName = Server.HtmlEncode(((TextBox)gvRates.Rows[e.RowIndex].FindControl("txbDShortName")).Text);
            string st = ((TextBox)gvRates.Rows[e.RowIndex].FindControl("txbDFrom")).Text;
            if (st.Length < 1)
            {
                ProcessPopupException(new Global.excToPopup("You must enter a valid 'From' date"));
                return;
            }
            DateTimeOffset DFrom = DateTimeOffset.Parse(st);
            st = ((TextBox)gvRates.Rows[e.RowIndex].FindControl("txbDTo")).Text;
            if (st.Length < 1)
            {
                ProcessPopupException(new Global.excToPopup("You must enter a valid 'To' date"));
                return;
            }
            DateTimeOffset DTo = DateTimeOffset.Parse(st);
            if (DFrom >= DTo)
            {
                ProcessPopupException(new Global.excToPopup("Error: The 'From' date `" + DFrom.ToString() + "` has to be earlier than the 'To' Date `" + DTo.ToString() + "`."));
                return;
            }
            int iET = Int32.Parse(((DropDownList)gvRates.Rows[e.RowIndex].FindControl("DDLEquipType")).SelectedItem.Value); // 'Value' contains the ID of a row in table EQUIPTYPES (not the same as SelectedIndex!!)
            int iLM = Int32.Parse(((DropDownList)gvRates.Rows[e.RowIndex].FindControl("DDLLaunchMethod")).SelectedItem.Value); // 'Value' contains the ID of a row in table LAUNCHMETHODS
            decimal dSingleDpUse = Decimal.Parse(((TextBox)gvRates.Rows[e.RowIndex].FindControl("txbSingleDpUse")).Text);
            int iNoChrg1stFt = Int32.Parse(((TextBox)gvRates.Rows[e.RowIndex].FindControl("txbNoChrg1stFt")).Text);
            decimal dAltDiffDpFt = Decimal.Parse(((TextBox)gvRates.Rows[e.RowIndex].FindControl("txbAltDiffDpFt")).Text);
            int iNoChrg1stMin = Int32.Parse(((TextBox)gvRates.Rows[e.RowIndex].FindControl("txbNoChrg1stMin")).Text);
            decimal dDurationDpMin = Decimal.Parse(((TextBox)gvRates.Rows[e.RowIndex].FindControl("txbDurationDpMin")).Text);
            st = ((TextBox)gvRates.Rows[e.RowIndex].FindControl("txbDurCapMin")).Text;
            if (st.Length < 1) st = "0";
            int iDurCapMin = Int32.Parse(st);
            if (iDurCapMin < 0)
            {
                ProcessPopupException(new Global.excToPopup("Duration cap cannot be negative"));
                return;
            }
            string sComment = Server.HtmlEncode(((TextBox)gvRates.Rows[e.RowIndex].FindControl("txbDComments")).Text);
            int iQBOI = Int32.Parse(((DropDownList)gvRates.Rows[e.RowIndex].FindControl("DDLAccItem")).SelectedItem.Value); // 'Value' contains the ID of a row in table QBO_ITEMNAMES
            int iFA_Item = Int32.Parse(((DropDownList)gvRates.Rows[e.RowIndex].FindControl("DDLFAItem")).SelectedItem.Value); // 'Value' contains the ID of a row in table FA_ITEMS
            int iFA_PmtTerm = Int32.Parse(((DropDownList)gvRates.Rows[e.RowIndex].FindControl("DDLFAPmtTerm")).SelectedItem.Value); // 'Value' contains the ID of a row in table FA_PMTTERMS

            Label lblIIdent = (Label)gvRates.Rows[e.RowIndex].FindControl("lblIIdent");
            int iId = Int32.Parse(lblIIdent.Text);
            if (iId == 0)
            {
                // Adding a new record
                RATE r = new RATE();
                r.sShortName = sShortName.Replace("'", "`");
                r.Dfrom = DFrom;
                r.DTo = DTo;
                r.iEquipType = iET;
                r.iLaunchMethod = iLM;
                r.sChargeCodes = " ";
                r.mSingleDpUse = dSingleDpUse;
                r.iNoChrg1stFt = iNoChrg1stFt;
                r.mAltDiffDpFt = dAltDiffDpFt;
                r.iNoChrg1stMin = iNoChrg1stMin;
                r.mDurationDpMin = dDurationDpMin;
                r.iDurCapMin = iDurCapMin;
                r.sComment = sComment.Replace("'", "`");
                r.iFA_Item = iFA_Item;
                r.iFA_PmtTerm = iFA_PmtTerm;
                r.iQBO_ItemName = iQBOI;

                dc.RATEs.InsertOnSubmit(r);
                sLog = DateTimeOffset.UtcNow.ToString() + ": sShortName=" + r.sShortName + ", Dfrom=" + r.Dfrom.ToString() + ", DTo=" + r.DTo.ToString() +
                    ", iEquipType=" + r.iEquipType.ToString() + ", iLaunchMethod=" + r.iLaunchMethod.ToString() + ", sChargeCodes=" + r.sChargeCodes +
                    ", mSingleDpUse=" + r.mSingleDpUse.ToString() + ", iNoCharg1stFt=" + r.iNoChrg1stFt.ToString() + ", mAltDiffDpFt=" + r.mAltDiffDpFt +
                    ", iNoChrg1stMin=" + r.iNoChrg1stMin.ToString() + ", mDurationDpMin=" + r.mDurationDpMin.ToString() + ", iDurCapMin=" + r.iDurCapMin.ToString() +
                    ", sComment=" + r.sComment + ", ifa_PmtTerm=" + r.iFA_PmtTerm.ToString() + ", iFA_Item=" + r.iFA_Item.ToString() +
                    ", iQBO_ItemName=" + r.iQBO_ItemName.ToString();
            }
            else
            {
                // Updating existing record
                int iID = Int32.Parse(((Label)gvRates.Rows[e.RowIndex].FindControl("lblIIdent")).Text);
                var r = (from v in dc.RATEs where v.ID == iID select v).First();
                r.sShortName = sShortName.Replace("'", "`");
                r.Dfrom = DFrom;
                r.DTo = DTo;
                r.iEquipType = iET;
                r.iLaunchMethod = iLM;
                r.mSingleDpUse = dSingleDpUse;
                r.iNoChrg1stFt = iNoChrg1stFt;
                r.mAltDiffDpFt = dAltDiffDpFt;
                r.iNoChrg1stMin = iNoChrg1stMin;
                r.mDurationDpMin = dDurationDpMin;
                r.iDurCapMin = iDurCapMin;
                r.sComment = sComment.Replace("'", "`");
                r.iFA_Item = iFA_Item;
                r.iFA_PmtTerm = iFA_PmtTerm;
                r.iQBO_ItemName = iQBOI;
                elt = ActivityLog.enumLogTypes.DataUpdate;
                sLog = DateTimeOffset.UtcNow.ToString() + " Z: sShortName=" + r.sShortName + ", Dfrom=" + r.Dfrom.ToString() + ", DTo=" + r.DTo.ToString() +
                    ", iEquipType=" + r.iEquipType.ToString() + ", iLaunchMethod=" + r.iLaunchMethod.ToString() + ", sChargeCodes=" + r.sChargeCodes +
                    ", mSingleDpUse=" + r.mSingleDpUse.ToString() + ", iNoCharg1stFt=" + r.iNoChrg1stFt.ToString() + ", mAltDiffDpFt=" + r.mAltDiffDpFt +
                    ", iNoChrg1stMin=" + r.iNoChrg1stMin.ToString() + ", mDurationDpMin=" + r.mDurationDpMin.ToString() + ", iDurCapMin=" + r.iDurCapMin.ToString() +
                    ", sComment=" + r.sComment + ", ifa_PmtTerm=" + r.iFA_PmtTerm.ToString() + ", iFA_Item=" + r.iFA_Item.ToString() +
                    ", iQBO_ItemName=" + r.iQBO_ItemName.ToString();
            }
            try
            {
                dc.SubmitChanges();
                ActivityLog.oLog(elt, 1, "Rates.aspx.cs: " + sLog);
            }
            catch (Exception exc)
            {
                Global.excToPopup ex = new Global.excToPopup(exc.Message);
                ProcessPopupException(ex);
                return;
            }
            dc.Dispose();
            gvRates.EditIndex = -1;
            FillDataTable();
        }

        protected void pbEChargeCodes_Click(object sender, EventArgs e)
        {
            Label lblIIdent = (Label)gvRates.Rows[gvRates.EditIndex].FindControl("lblIIdent");
            ipRate = Int32.Parse(lblIIdent.Text);
            MPECC_Show();
        }

        protected void DDLChargeCodes_PreRender(object sender, EventArgs e)
        {
            FlyActInvoiceDataContext dc = new FlyActInvoiceDataContext();
            var qCC = from c in dc.spDDL_ChargeCodes(ipRate) select c;
            DDLChargeCodes.DataSource = qCC;
            DDLChargeCodes.DataValueField = "ID";
            DDLChargeCodes.DataTextField = "sCode";
            DDLChargeCodes.DataBind();
        }

        #region ModPopExt for ChargeCodes
        //======================
        private void MPECC_Show()
        {
            Label lblIIdent = (Label)gvRates.Rows[gvRates.EditIndex].FindControl("lblIIdent");
            int iRate = Int32.Parse(lblIIdent.Text);
            FlyActInvoiceDataContext dc = new FlyActInvoiceDataContext();
            var qac = (from c in dc.CHARGECODESINRATEs where c.iRate == iRate select new { c.ID, c.CHARGECODE }).ToList();
            gvPopChargeCodes.DataSource = qac;
            gvPopChargeCodes.DataBind();
            ModPopExt_ChargeCodes.Show();
        }
        protected void MPECC_Button_Click(object sender, EventArgs e)
        {
        }
        #endregion

        protected void pbMPECC_Add_Click(object sender, EventArgs e)
        {
            CHARGECODESINRATE ci = new CHARGECODESINRATE();
            ci.iRate = ipRate;
            ci.iChargeCode = Int32.Parse(DDLChargeCodes.SelectedValue);
            FlyActInvoiceDataContext dc = new FlyActInvoiceDataContext();
            dc.CHARGECODESINRATEs.InsertOnSubmit(ci);
            dc.SubmitChanges();
        }

        public string s_ChargeCodes(int iuRate)
        {
            FlyActInvoiceDataContext dc = new FlyActInvoiceDataContext();
            var qc = from c in dc.CHARGECODESINRATEs where c.iRate == iuRate select c.CHARGECODE.cChargeCode;
            string sChargeCodes = "";
            string sPunct = "";
            foreach (var c in qc)
            {
                sChargeCodes += sPunct + c;
                sPunct = ", ";
            }
            return sChargeCodes;
        }

        protected void pbHelp_Click(object sender, EventArgs e)
        {
            ModPopExt_Help.Show();
        }

        protected void gvPopChargeCodes_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            Label lblIID = (Label)gvPopChargeCodes.Rows[e.RowIndex].FindControl("lblIID");
            int iCI_ID = Int32.Parse(lblIID.Text);
            FlyActInvoiceDataContext dc = new FlyActInvoiceDataContext();
            CHARGECODESINRATE qci = (from i in dc.CHARGECODESINRATEs where i.ID == iCI_ID select i).First();
            dc.CHARGECODESINRATEs.DeleteOnSubmit(qci);
            dc.SubmitChanges();
        }

        protected void DDLFAItem_PreRender(object sender, EventArgs e)
        {

        }
    }
}