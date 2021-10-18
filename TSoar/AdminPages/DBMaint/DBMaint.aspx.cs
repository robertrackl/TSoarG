using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Configuration;
using System.Data.SqlClient;

namespace TSoar.AdminPages.DBMaint
{
    // Database Maintenance
    public partial class DBMaint : System.Web.UI.Page
    {
        #region Declarations
        //======================
        private TextBox txbItemName;
        private string stxbEditItemName;
        private Global.enugSingleMFF enumMFF;
        private int index;
        private string sIdItemNameLabel;
        private GridView gvItem;

        private string sTextBeforeUpdate { get { return (string)ViewState["sTextBeforeUpdate"] ?? ""; } set { ViewState["sTextBeforeUpdate"] = value; } }
        private string sOKCmdArg { get { return (string)ViewState["sOKCmdArg"] ?? ""; } set { ViewState["sOKCmdArg"] = value; } }
        private string sItemName { get { return (string)ViewState["sItemName"] ?? ""; } set { ViewState["sItemName"] = value; } }
        private string sSKey {
            get {
                return (string)ViewState["sSKey"] ?? "";
            }
            set {
                ViewState["sSKey"] = value;
            }
        }
        private string sRowID { get { return (string)ViewState["sRowID"] ?? ""; } set { ViewState["sRowID"] = value; } }

        TSoar.DB.SCUD_Multi mCRUD = new TSoar.DB.SCUD_Multi();
        TSoar.DB.SCUD_single lCRUD = new TSoar.DB.SCUD_single();
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                foreach(Global.enugSingleMFF ei in Enum.GetValues(typeof(Global.enugSingleMFF)))
                {
                    DisplayInGrid(ei);
                }
                DisplayInGrid(Global.enugInfoType.Settings);
            }
        }

        private void DisplayInGrid(Global.enugSingleMFF enugMFF)
        {
            GridView gv = new GridView();

            switch (enugMFF)
            {
                case Global.enugSingleMFF.Qualifications:
                    gv = gvQualif;
                    break;
                case Global.enugSingleMFF.Ratings:
                    gv = gvRating;
                    break;
                case Global.enugSingleMFF.Certifications:
                    gv = gvCertif;
                    break;
                case Global.enugSingleMFF.MembershipCategories:
                    gv = gvMembCat;
                    break;
                case Global.enugSingleMFF.SSA_MemberCategories:
                    gv = gvSSA_MC;
                    break;
                case Global.enugSingleMFF.AviatorRoles:
                    gv = gvAvRole;
                    break;
                case Global.enugSingleMFF.EquipmentRoles:
                    gv = gvEqRole;
                    break;
                case Global.enugSingleMFF.SpecialOpTypes:
                    gv = gvSpOpTy;
                    break;
                case Global.enugSingleMFF.LaunchMethods:
                    gv = gvLaunchM;
                    break;
                case Global.enugSingleMFF.EquipmentTypes:
                    gv = gvEqType;
                    break;
                case Global.enugSingleMFF.EquipmentActionTypes:
                    gv = gvEqActType;
                    break;
                case Global.enugSingleMFF.ContactTypes:
                    gv = gvContTy;
                    break;
                case Global.enugSingleMFF.Offices:
                    gv = gvOffice;
                    break;
                case Global.enugSingleMFF.Locations:
                    gv = gvLoc;
                    break;
                case Global.enugSingleMFF.ChargeCodes:
                    gv = gvChargeCode;
                    break;
                case Global.enugSingleMFF.FA_AccItems:
                    gv = gvFAItems;
                    break;
                case Global.enugSingleMFF.FA_PmtTerms:
                    gv = gvFAPmtTerms;
                    break;
                case Global.enugSingleMFF.QBO_AccItem:
                    gv = gvAccItem;
                    break;
                case Global.enugSingleMFF.InvoiceSources:
                    gv = gvInvSrc;
                    break;
                case Global.enugSingleMFF.FSCategories:
                    gv = gvFSCategs;
                    break;
            }
            try
            {
                DataView dv = new DataView(lCRUD.GetAll(enugMFF));
                switch (enugMFF)
                {
                    case Global.enugSingleMFF.FSCategories:
                        dv.Sort = "cKind DESC, iOrder ASC";
                        break;
                }
                gv.DataSource = dv;
                gv.DataBind();
            }
            catch (Global.excToPopup exc)
            {
                ProcessPopupException(exc);
            }
        }

        private void DisplayInGrid(Global.enugInfoType euInfoType)
        {
            GridView g = null;
            string ss = "";
            switch (euInfoType)
            {
                case Global.enugInfoType.Settings:
                    g = gvSettings;
                    ss = "PageSizeSettings";
                    break;
                default:
                    return;
            }
            g.PageSize = Int32.Parse(mCRUD.GetSetting(ss));
            g.DataSource = mCRUD.GetAll(euInfoType);
            g.DataBind();
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
                if ((btn.ID == "YesButton") && (btn.CommandName == "NumFKRefs"))
                {
                    int iNumFKRefs = -1;
                    switch (OkButton.CommandArgument)
                    {
                        case "Loc":
                            // Count foreign key references for the Location
                            iNumFKRefs = lCRUD.iNumForeignKeyRefs(Global.enugSingleMFF.Locations, btn.CommandArgument);
                            break;
                        case "Qualif":
                            // Count foreign key references for the Qualification
                            iNumFKRefs = lCRUD.iNumForeignKeyRefs(Global.enugSingleMFF.Qualifications, btn.CommandArgument);
                            break;
                        case "Rating":
                            // Count foreign key references for the Rating
                            iNumFKRefs = lCRUD.iNumForeignKeyRefs(Global.enugSingleMFF.Ratings, btn.CommandArgument);
                            break;
                        case "Certif":
                            // Count foreign key references for the Qualification
                            iNumFKRefs = lCRUD.iNumForeignKeyRefs(Global.enugSingleMFF.Certifications, btn.CommandArgument);
                            break;
                        case "MembCat":
                            // Count foreign key references for the Membership Category
                            iNumFKRefs = lCRUD.iNumForeignKeyRefs(Global.enugSingleMFF.MembershipCategories, btn.CommandArgument);
                            break;
                        case "SSAMC":
                            // Count foreign key references for the SSA Member Category
                            iNumFKRefs = lCRUD.iNumForeignKeyRefs(Global.enugSingleMFF.SSA_MemberCategories, btn.CommandArgument);
                            break;
                        case "AvRole":
                            // Count foreign key references for the Aviator Role
                            iNumFKRefs = lCRUD.iNumForeignKeyRefs(Global.enugSingleMFF.AviatorRoles, btn.CommandArgument);
                            break;
                        case "EqRole":
                            // Count foreign key references for the Equipment Role
                            iNumFKRefs = lCRUD.iNumForeignKeyRefs(Global.enugSingleMFF.EquipmentRoles, btn.CommandArgument);
                            break;
                        case "SpecialOpTy":
                            // Count Foreign Key References for Special Operations Types
                            iNumFKRefs = lCRUD.iNumForeignKeyRefs(Global.enugSingleMFF.SpecialOpTypes, btn.CommandArgument);
                            break;
                        case "LaunchM":
                            // Count foreign key references for the Launch Method
                            iNumFKRefs = lCRUD.iNumForeignKeyRefs(Global.enugSingleMFF.LaunchMethods, btn.CommandArgument);
                            break;
                        case "EqType":
                            // Count foreign key references for the Equipment Type
                            iNumFKRefs = lCRUD.iNumForeignKeyRefs(Global.enugSingleMFF.EquipmentTypes, btn.CommandArgument);
                            break;
                        case "EqActType":
                            // Count foreign key references for the Equipment Type
                            iNumFKRefs = lCRUD.iNumForeignKeyRefs(Global.enugSingleMFF.EquipmentActionTypes, btn.CommandArgument);
                            break;
                        case "ContTy":
                            // Count foreign key references for the Contact Type
                            iNumFKRefs = lCRUD.iNumForeignKeyRefs(Global.enugSingleMFF.ContactTypes, btn.CommandArgument);
                            break;
                        case "Office":
                            // Count foreign key references for the Board Office
                            iNumFKRefs = lCRUD.iNumForeignKeyRefs(Global.enugSingleMFF.Offices, btn.CommandArgument);
                            break;
                        case "ChargeCode":
                            // Count foreign key references for the Charge Code
                            iNumFKRefs = lCRUD.iNumForeignKeyRefs(Global.enugSingleMFF.ChargeCodes, btn.CommandArgument);
                            break;
                        case "FA_Item":
                            // Count foreign key references for the QBO Accounting Item
                            iNumFKRefs = lCRUD.iNumForeignKeyRefs(Global.enugSingleMFF.FA_AccItems, btn.CommandArgument);
                            break;
                        case "FA_PmtTerm":
                            // Count foreign key references for the QBO Accounting Item
                            iNumFKRefs = lCRUD.iNumForeignKeyRefs(Global.enugSingleMFF.FA_PmtTerms, btn.CommandArgument);
                            break;
                        case "AccItem":
                            // Count foreign key references for the QBO Accounting Item
                            iNumFKRefs = lCRUD.iNumForeignKeyRefs(Global.enugSingleMFF.QBO_AccItem, btn.CommandArgument);
                            break;
                        case "InvSrc":
                            // Count foreign key references for the Invoice Source
                            iNumFKRefs = lCRUD.iNumForeignKeyRefs(Global.enugSingleMFF.InvoiceSources, btn.CommandArgument);
                            break;
                        case "Categ":
                            // Count foreign key references for the Invoice Source
                            iNumFKRefs = lCRUD.iNumForeignKeyRefs(Global.enugSingleMFF.FSCategories, btn.CommandArgument);
                            break;
                    }
                    if (iNumFKRefs < 0)
                    {
                        ProcessPopupException(new Global.excToPopup("SCUD_single.cs software error: Did not obtain number of foreign key references"));
                        return;
                    }
                    else if (iNumFKRefs > 0)
                    {
                        YesButton.CommandName = "delete";
                        string sItem = YesButton.CommandArgument;
                        lblPopupText.Text = "ATTENTION !! You are about to delete at least " + iNumFKRefs.ToString() +
                            " related records (there may be more!). Are you sure that you want to get rid of all of these? " + 
                            " ** This is NOT usually done - Better answer NO unless you know what you are doing! ** Confirm deletion of " + sItemName + " '" + sItem + "'";
                        ActivityLog.oLog(ActivityLog.enumLogTypes.Warning, 0, lblPopupText.Text);
                        MPE_Show(Global.enumButtons.NoYes);
                        return;
                    }
                    else
                    {
                        // No foreign key reference exists -> ok to delete
                        btn.CommandName = "delete";
                    }
                }
                if ((btn.ID == "YesButton") && (btn.CommandName == "delete"))
                { 
                    switch (OkButton.CommandArgument)
                    {
                        case "Loc":
                            // Delete the Location
                            lCRUD.DeleteOne(Global.enugSingleMFF.Locations, btn.CommandArgument);
                            DisplayInGrid(Global.enugSingleMFF.Locations);
                            break;
                        case "Qualif":
                            // Delete the Qualification
                            lCRUD.DeleteOne(Global.enugSingleMFF.Qualifications, btn.CommandArgument);
                            DisplayInGrid(Global.enugSingleMFF.Qualifications);
                            break;
                        case "Rating":
                            // Delete the Rating
                            lCRUD.DeleteOne(Global.enugSingleMFF.Ratings, btn.CommandArgument);
                            DisplayInGrid(Global.enugSingleMFF.Ratings);
                            break;
                        case "Certif":
                            // Delete the Certification
                            lCRUD.DeleteOne(Global.enugSingleMFF.Certifications, btn.CommandArgument);
                            DisplayInGrid(Global.enugSingleMFF.Certifications);
                            break;
                        case "MembCat":
                            // Delete the Membership Category
                            lCRUD.DeleteOne(Global.enugSingleMFF.MembershipCategories, btn.CommandArgument);
                            DisplayInGrid(Global.enugSingleMFF.MembershipCategories);
                            break;
                        case "SSAMC":
                            // Delete the SSA Member Category
                            lCRUD.DeleteOne(Global.enugSingleMFF.SSA_MemberCategories, btn.CommandArgument);
                            DisplayInGrid(Global.enugSingleMFF.SSA_MemberCategories);
                            break;
                        case "AvRole":
                            // Delete the Aviator Role
                            lCRUD.DeleteOne(Global.enugSingleMFF.AviatorRoles, btn.CommandArgument);
                            DisplayInGrid(Global.enugSingleMFF.AviatorRoles);
                            break;
                        case "EqRole":
                            // Delete the Equipment Role
                            lCRUD.DeleteOne(Global.enugSingleMFF.EquipmentRoles, btn.CommandArgument);
                            DisplayInGrid(Global.enugSingleMFF.EquipmentRoles);
                            break;
                        case "SpecialOpTy":
                            // Delete the Special Operation Type
                            lCRUD.DeleteOne(Global.enugSingleMFF.SpecialOpTypes, btn.CommandArgument);
                            DisplayInGrid(Global.enugSingleMFF.SpecialOpTypes);
                            break;
                        case "LaunchM":
                            // Delete the Launch Method
                            lCRUD.DeleteOne(Global.enugSingleMFF.LaunchMethods, btn.CommandArgument);
                            DisplayInGrid(Global.enugSingleMFF.LaunchMethods);
                            break;
                        case "EqType":
                            // Delete the Equipment Type
                            lCRUD.DeleteOne(Global.enugSingleMFF.EquipmentTypes, btn.CommandArgument);
                            DisplayInGrid(Global.enugSingleMFF.EquipmentTypes);
                            break;
                        case "EqActType":
                            // Delete the Equipment Type
                            lCRUD.DeleteOne(Global.enugSingleMFF.EquipmentActionTypes, btn.CommandArgument);
                            DisplayInGrid(Global.enugSingleMFF.EquipmentActionTypes);
                            break;
                        case "ContTy":
                            // Delete the Contact Type
                            lCRUD.DeleteOne(Global.enugSingleMFF.ContactTypes, btn.CommandArgument);
                            DisplayInGrid(Global.enugSingleMFF.ContactTypes);
                            break;
                        case "Office":
                            // Delete the Office
                            lCRUD.DeleteOne(Global.enugSingleMFF.Offices, btn.CommandArgument);
                            DisplayInGrid(Global.enugSingleMFF.Offices);
                            break;
                        case "ChargeCode":
                            // Delete the Charge Code
                            lCRUD.DeleteOne(Global.enugSingleMFF.ChargeCodes, btn.CommandArgument);
                            DisplayInGrid(Global.enugSingleMFF.ChargeCodes);
                            break;
                        case "FA_Item":
                            // Delete the FA Item Code
                            lCRUD.DeleteOne(Global.enugSingleMFF.FA_AccItems, btn.CommandArgument);
                            DisplayInGrid(Global.enugSingleMFF.FA_AccItems);
                            break;
                        case "FA_PmtTerm":
                            // Delete the FA Payment Term
                            lCRUD.DeleteOne(Global.enugSingleMFF.FA_PmtTerms, btn.CommandArgument);
                            DisplayInGrid(Global.enugSingleMFF.FA_PmtTerms);
                            break;
                        case "AccItem":
                            // Delete the QBO Accounting Item
                            lCRUD.DeleteOne(Global.enugSingleMFF.QBO_AccItem, btn.CommandArgument);
                            DisplayInGrid(Global.enugSingleMFF.QBO_AccItem);
                            break;
                        case "InvSrc":
                            // Delete the Invoice Source
                            lCRUD.DeleteOne(Global.enugSingleMFF.InvoiceSources, btn.CommandArgument);
                            DisplayInGrid(Global.enugSingleMFF.InvoiceSources);
                            break;
                        case "Categ":
                            // Delete the Flight Scheduling Activity Category
                            lCRUD.DeleteOne(Global.enugSingleMFF.FSCategories, btn.CommandArgument);
                            DisplayInGrid(Global.enugSingleMFF.FSCategories);
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
            ActivityLog.oLog(ActivityLog.enumLogTypes.PopupException, 0, lblPopupText.Text);
            MPE_Show(Global.enumButtons.OkOnly);
        }
        #endregion

        #region Locations
        //====================
        protected void pbCreateLoc_Click(object sender, EventArgs e)
        {
            enumMFF = Global.enugSingleMFF.Locations;
            string sLoc = Server.HtmlEncode(txbNewLoc.Text.Trim().Replace("'","`"));
            string sAbbrev = Server.HtmlEncode(txbNewAbbrev.Text.Trim().Replace("'", "`"));
            string sRunwayAlt = Server.HtmlEncode(txbNewAlt.Text.Trim());
            if (sLoc.Length < 1)
            {
                Global.excToPopup excP = new Global.excToPopup("Location name must not be empty");
                ProcessPopupException(excP);
                return;
            }
            if (sLoc.IndexOf(",") > -1)
            {
                Global.excToPopup excP = new Global.excToPopup("Location name must not contain a comma");
                ProcessPopupException(excP);
                return;
            }
            if (sRunwayAlt.Length < 1)
            {
                sRunwayAlt = "0";
            }
            decimal dAlt = decimal.Parse(sRunwayAlt);
            if (dAlt < -1000.0M || dAlt > 30000.0M)
            {
                ProcessPopupException(new Global.excToPopup("Runway altitude must be between -1000 and +30000 feet"));
                return;
            }
            using (SqlConnection SqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlConn"].ToString()))
            {
                SqlConn.Open();
                string sCmd = "SELECT COUNT(*) FROM LOCATIONS WHERE sLocation='" + sLoc + "'";
                if (sAbbrev.Length > 0)
                {
                    sCmd += " OR sAbbrev='" + sAbbrev + "'";
                }
                using (SqlCommand cmd = new SqlCommand(sCmd))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = SqlConn;
                    int iCount = (int)cmd.ExecuteScalar();
                    if (iCount > 0)
                    {
                        sCmd = "` is already in use in this table";
                        if (sAbbrev.Length > 0)
                        {
                            sCmd = "Either `" + sLoc + "` or `" + sAbbrev + sCmd;
                        }
                        else
                        {
                            sCmd = "`" + sLoc + sCmd;
                        }
                        Global.excToPopup excP = new Global.excToPopup(sCmd);
                        ProcessPopupException(excP);
                        return;
                    }
                }
                using (SqlCommand cmd = new SqlCommand("INSERT INTO LOCATIONS (sLocation, sAbbrev, dRunwayAltitude) VALUES ('"
                    + sLoc + "','" + sAbbrev + "'," + sRunwayAlt + ")"))
                {
                    try
                    {
                        cmd.CommandTimeout = 600;
                        cmd.CommandType = CommandType.Text;
                        cmd.Connection = SqlConn;
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception exc)
                    {
                        Global.excToPopup excP = new Global.excToPopup(exc.Message);
                        ProcessPopupException(excP);
                        return;
                    }
                }
            }
            ActivityLog.oLog(ActivityLog.enumLogTypes.DataInsert, 1, Enum.GetName(typeof(Global.enugSingleMFF), (int)Global.enugSingleMFF.Locations) + 
                " | " + sLoc + " | " + sAbbrev + " | " + sRunwayAlt);
            txbNewLoc.Text = "";
            txbNewAbbrev.Text = "";
            txbNewAlt.Text = "";
            DisplayInGrid(enumMFF);
        }

        protected void gvLoc_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            sIdItemNameLabel = "lblLoc";
            index = e.RowIndex;
            gvItem = gvLoc;
            sOKCmdArg = "Loc";
            sItemName = "Location";
            gv_RowDeleting();
        }

        protected void gvLoc_RowEditing(object sender, GridViewEditEventArgs e)
        {
            index = e.NewEditIndex;
            GridViewRow row = gvLoc.Rows[index];
            Label lbl = (Label)row.FindControl("lblLoc");
            sTextBeforeUpdate = lbl.Text;
            gvLoc.EditIndex = index;
            DisplayInGrid(Global.enugSingleMFF.Locations);
        }

        protected void gvLoc_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvItem = gvLoc;
            enumMFF = Global.enugSingleMFF.Locations;
            gv_RowCancelingEdit();
        }

        protected void gvLoc_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            string[] sa = new string[3];
            GridViewRow row = gvLoc.Rows[e.RowIndex];
            TextBox txbL = (TextBox)row.FindControl("txbLoc");
            sa[0] = Server.HtmlEncode(txbL.Text.Trim().Replace("'", "`"));
            if (sa[0].IndexOf(",") > -1)
            {
                Global.excToPopup excP = new Global.excToPopup("Location name must not contain a comma");
                ProcessPopupException(excP);
                return;
            }
            TextBox txbA = (TextBox)row.FindControl("txbAbbrev");
            sa[1] = Server.HtmlEncode(txbA.Text.Trim().Replace("'", "`"));
            TextBox txbH = (TextBox)row.FindControl("txbAlt");
            txbH.Text = Server.HtmlEncode(txbH.Text);
            sa[2] = (txbH.Text.Length > 0) ? txbH.Text : "0";
            using (SqlConnection SqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlConn"].ToString()))
            {
                using (SqlCommand cmd = new SqlCommand("UPDATE LOCATIONS SET sLocation='" + sa[0] + "', sAbbrev='" + sa[1] +
                        "', dRunwayAltitude=" + sa[2] + " WHERE sLocation='" + sTextBeforeUpdate + "'" ))
                {
                    try
                    {
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = SqlConn;
                    cmd.CommandTimeout = 600;
                    SqlConn.Open();
                    cmd.ExecuteNonQuery();
                    SqlConn.Close();
                    }
                    catch (Exception exc)
                    {
                        Global.excToPopup excP = new Global.excToPopup(exc.Message);
                        ProcessPopupException(excP);
                        return;
                    }
                    ActivityLog.oLog(ActivityLog.enumLogTypes.DataUpdate, 1, Enum.GetName(typeof(Global.enugInfoType), 
                        (int)Global.enugSingleMFF.Locations) + " | " + string.Join(" | ", sa));
                }
            }
            gvLoc.EditIndex = -1;
            DisplayInGrid(Global.enugSingleMFF.Locations);
        }
        #endregion

        #region People Contact Types
        //====================
        protected void pbCreateContTy_Click(object sender, EventArgs e)
        {
            using (SqlConnection SqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlConn"].ToString()))
            {
                string sContactType = Server.HtmlEncode(txbNewContTy.Text.Trim().Replace("'", "`"));
                string sHasPhysAddr = chbHasPA.Checked ? "1" : "0";
                string sDefPrioRank = txbDefPrioRank.Text;
                SqlConn.Open();
                using (SqlCommand cmd = new SqlCommand("INSERT INTO CONTACTTYPES (sPeopleContactType, bHasPhysAddr, dDefaultRank) VALUES ('"
                    + sContactType + "'," + sHasPhysAddr + "," + sDefPrioRank + ")"))
                {
                    try
                    {
                        cmd.CommandTimeout = 600;
                        cmd.CommandType = CommandType.Text;
                        cmd.Connection = SqlConn;
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception exc)
                    {
                        Global.excToPopup excP = new Global.excToPopup(exc.Message);
                        ProcessPopupException(excP);
                    }
                }
            }
            txbNewContTy.Text = "";
            chbHasPA.Checked = false;
            DisplayInGrid(Global.enugSingleMFF.ContactTypes);
        }

        protected void gvContTy_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            sIdItemNameLabel = "lblContTy";
            index = e.RowIndex;
            gvItem = gvContTy;
            sOKCmdArg = "ContTy";
            sItemName = "Contact Type";
            gv_RowDeleting();
        }

        protected void gvContTy_RowEditing(object sender, GridViewEditEventArgs e)
        {
            index = e.NewEditIndex;
            gvItem = gvContTy;
            GridViewRow row = gvItem.Rows[e.NewEditIndex];
            Label lbl = (Label)row.FindControl("lblContTy");
            sTextBeforeUpdate = lbl.Text;
            gvItem.EditIndex = index;
            DisplayInGrid(Global.enugSingleMFF.ContactTypes);
        }

        protected void gvContTy_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvItem = gvContTy;
            enumMFF = Global.enugSingleMFF.ContactTypes;
            gv_RowCancelingEdit();
        }

        protected void gvContTy_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            string[] sa = new string[3];
            GridViewRow row = gvContTy.Rows[e.RowIndex];
            TextBox txbC = (TextBox)row.FindControl("txbContTy");
            sa[0] = Server.HtmlEncode(txbC.Text.Trim().Replace("'", "`"));
            if (sa[0].IndexOf(",") > -1)
            {
                Global.excToPopup excP = new Global.excToPopup("Contact Type must not contain a comma");
                ProcessPopupException(excP);
                return;
            }
            CheckBox chb = (CheckBox)row.FindControl("chbUHasPA");
            sa[1] = chb.Checked ? "1" : "0";
            sa[2] = ((TextBox)row.FindControl("txbDefRank")).Text;
            using (SqlConnection SqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlConn"].ToString()))
            {
                using (SqlCommand cmd = new SqlCommand("UPDATE CONTACTTYPES SET sPeopleContactType='" + sa[0] +
                    "', bHasPhysAddr=" + sa[1] + ", dDefaultRank=" + sa[2] +
                        " WHERE sPeopleContactType='" + sTextBeforeUpdate + "'"))
                {
                    try
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Connection = SqlConn;
                        cmd.CommandTimeout = 600;
                        SqlConn.Open();
                        cmd.ExecuteNonQuery();
                        SqlConn.Close();
                    }
                    catch (Exception exc)
                    {
                        Global.excToPopup excP = new Global.excToPopup(exc.Message);
                        ProcessPopupException(excP);
                        return;
                    }
                    ActivityLog.oLog(ActivityLog.enumLogTypes.DataUpdate, 1, Enum.GetName(typeof(Global.enugInfoType),
                        (int)Global.enugSingleMFF.ContactTypes) + " | " + string.Join(" | ", sa));
                }
            }
            gvContTy.EditIndex = -1;
            DisplayInGrid(Global.enugSingleMFF.ContactTypes);
        }
        #endregion

        #region Qualifications
        //====================
        protected void pbCreateQualif_Click(object sender, EventArgs e)
        {
            txbItemName = txbNewQualif;
            enumMFF = Global.enugSingleMFF.Qualifications;
            CreateButton();
        }

        protected void gvQualif_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            sIdItemNameLabel = "lblQualif";
            index = e.RowIndex;
            gvItem = gvQualif;
            sOKCmdArg = "Qualif";
            sItemName = "Qualification";
            gv_RowDeleting();
        }

        protected void gvQualif_RowEditing(object sender, GridViewEditEventArgs e)
        {
            sIdItemNameLabel = "lblQualif";
            index = e.NewEditIndex;
            gvItem = gvQualif;
            enumMFF = Global.enugSingleMFF.Qualifications;
            gv_RowEditing();
        }

        protected void gvQualif_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvItem = gvQualif;
            enumMFF = Global.enugSingleMFF.Qualifications;
            gv_RowCancelingEdit();
        }

        protected void gvQualif_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            index = e.RowIndex;
            gvItem = gvQualif;
            stxbEditItemName = "txbQualif";
            enumMFF = Global.enugSingleMFF.Qualifications;
            gv_RowUpdating();
        }
        #endregion

        #region Ratings
        protected void pbCreateRating_Click(object sender, EventArgs e)
        {
            txbItemName = txbNewRating;
            enumMFF = Global.enugSingleMFF.Ratings;
            CreateButton();
        }

        protected void gvRating_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            sIdItemNameLabel = "lblRating";
            index = e.RowIndex;
            gvItem = gvRating;
            sOKCmdArg = "Rating";
            sItemName = "Rating";
            gv_RowDeleting();
        }

        protected void gvRating_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            index = e.RowIndex;
            gvItem = gvRating;
            stxbEditItemName = "txbRating";
            enumMFF = Global.enugSingleMFF.Ratings;
            gv_RowUpdating();
        }

        protected void gvRating_RowEditing(object sender, GridViewEditEventArgs e)
        {
            sIdItemNameLabel = "lblRating";
            index = e.NewEditIndex;
            gvItem = gvRating;
            enumMFF = Global.enugSingleMFF.Ratings;
            gv_RowEditing();
        }

        protected void gvRating_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvItem = gvRating;
            enumMFF = Global.enugSingleMFF.Ratings;
            gv_RowCancelingEdit();
        }
        #endregion

        #region Certifications
        protected void pbCreateCertif_Click(object sender, EventArgs e)
        {
            txbItemName = txbNewCert;
            enumMFF = Global.enugSingleMFF.Certifications;
            CreateButton();
        }

        protected void gvCertif_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            sIdItemNameLabel = "lblCertif";
            index = e.RowIndex;
            gvItem = gvCertif;
            sOKCmdArg = "Certif";
            sItemName = "Certification";
            gv_RowDeleting();
        }

        protected void gvCertif_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            index = e.RowIndex;
            gvItem = gvCertif;
            stxbEditItemName = "txbCertif";
            enumMFF = Global.enugSingleMFF.Certifications;
            gv_RowUpdating();
        }

        protected void gvCertif_RowEditing(object sender, GridViewEditEventArgs e)
        {
            sIdItemNameLabel = "lblCertif";
            index = e.NewEditIndex;
            gvItem = gvCertif;
            enumMFF = Global.enugSingleMFF.Certifications;
            gv_RowEditing();
        }

        protected void gvCertif_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvItem = gvCertif;
            enumMFF = Global.enugSingleMFF.Certifications;
            gv_RowCancelingEdit();
        }
        #endregion

        #region Club Membership Categories
        //===========================
        protected void pbCreateMembCat_Click(object sender, EventArgs e)
        {
            txbItemName = txbNewMembCat;
            enumMFF = Global.enugSingleMFF.MembershipCategories;
            CreateButton();
        }

        protected void gvMembCat_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            sIdItemNameLabel = "lblMembCat";
            index = e.RowIndex;
            gvItem = gvMembCat;
            sOKCmdArg = "MembCat";
            sItemName = "Membership Category";
            gv_RowDeleting();
        }

        protected void gvMembCat_RowEditing(object sender, GridViewEditEventArgs e)
        {
            sIdItemNameLabel = "lblMembCat";
            index = e.NewEditIndex;
            gvItem = gvMembCat;
            enumMFF = Global.enugSingleMFF.MembershipCategories;
            gv_RowEditing();
        }

        protected void gvMembCat_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvItem = gvMembCat;
            enumMFF = Global.enugSingleMFF.MembershipCategories;
            gv_RowCancelingEdit();
        }

        protected void gvMembCat_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            index = e.RowIndex;
            gvItem = gvMembCat;
            stxbEditItemName = "txbMembCat";
            enumMFF = Global.enugSingleMFF.MembershipCategories;
            gv_RowUpdating();
        }
        #endregion

        #region SSA Member Categories
        protected void pbCreateSSAMC_Click(object sender, EventArgs e)
        {
            txbItemName = txbNewSSAMC;
            enumMFF = Global.enugSingleMFF.SSA_MemberCategories;
            CreateButton();
        }

        protected void gvSSA_MC_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            sIdItemNameLabel = "lblSSAMC";
            index = e.RowIndex;
            gvItem = gvSSA_MC;
            sOKCmdArg = "SSAMC";
            sItemName = "SSA Member Category";
            gv_RowDeleting();
        }

        protected void gvSSA_MC_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            index = e.RowIndex;
            gvItem = gvSSA_MC;
            stxbEditItemName = "txbSSAMC";
            enumMFF = Global.enugSingleMFF.SSA_MemberCategories;
            gv_RowUpdating();
        }

        protected void gvSSA_MC_RowEditing(object sender, GridViewEditEventArgs e)
        {
            sIdItemNameLabel = "lblSSAMC";
            index = e.NewEditIndex;
            gvItem = gvSSA_MC;
            enumMFF = Global.enugSingleMFF.SSA_MemberCategories;
            gv_RowEditing();
        }

        protected void gvSSA_MC_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvItem = gvSSA_MC;
            enumMFF = Global.enugSingleMFF.SSA_MemberCategories;
            gv_RowCancelingEdit();
        }
        #endregion

        #region Aviator Roles
        //===================
        protected void pbCreateAvRole_Click(object sender, EventArgs e)
        {
            txbItemName = txbNewAvRole;
            enumMFF = Global.enugSingleMFF.AviatorRoles;
            CreateButton();
        }

        protected void gvAvRole_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            sIdItemNameLabel = "lblAvRole";
            index = e.RowIndex;
            gvItem = gvAvRole;
            sOKCmdArg = "AvRole";
            sItemName = "Aviator Role";
            gv_RowDeleting();
        }

        protected void gvAvRole_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            index = e.RowIndex;
            gvItem = gvAvRole;
            stxbEditItemName = "txbAvRole";
            enumMFF = Global.enugSingleMFF.AviatorRoles;
            gv_RowUpdating();
        }

        protected void gvAvRole_RowEditing(object sender, GridViewEditEventArgs e)
        {
            sIdItemNameLabel = "lblAvRole";
            index = e.NewEditIndex;
            gvItem = gvAvRole;
            enumMFF = Global.enugSingleMFF.AviatorRoles;
            gv_RowEditing();
        }

        protected void gvAvRole_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvItem = gvAvRole;
            enumMFF = Global.enugSingleMFF.AviatorRoles;
            gv_RowCancelingEdit();
        }
        #endregion

        #region Equipment Roles
        //=====================
        protected void pbCreateEqRole_Click(object sender, EventArgs e)
        {
            //txbItemName = txbNewEqRName;
            //enumMFF = Global.enugSingleMFF.EquipmentRoles;
            //CreateButton();
            enumMFF = Global.enugSingleMFF.EquipmentRoles;
            string sEquipmentRole = Server.HtmlEncode(txbNewEqRName.Text.Trim().Replace("'", "`"));
            int iAvgUseDurationMinutes = 10;
            Int32.TryParse(txbNewEqRDur.Text, out iAvgUseDurationMinutes);
            string sComment = Server.HtmlEncode(txbNewEqRComment.Text.Trim().Replace("'", "`"));
            if (sEquipmentRole.Length < 1)
            {
                Global.excToPopup excP = new Global.excToPopup("Equipment Role name must not be empty");
                ProcessPopupException(excP);
                return;
            }
            if (sEquipmentRole.IndexOf(",") > -1)
            {
                Global.excToPopup excP = new Global.excToPopup("Equipment Role name must not contain a comma");
                ProcessPopupException(excP);
                return;
            }
            if (iAvgUseDurationMinutes < 1)
            {
                Global.excToPopup excP = new Global.excToPopup("Average Use Duration must not be less than 1 minute");
                ProcessPopupException(excP);
                return;
            }
            if (iAvgUseDurationMinutes > 1440)
            {
                Global.excToPopup excP = new Global.excToPopup("Average Use Duration must not be greater than 1440 minutes (1 day)");
                ProcessPopupException(excP);
                return;
            }
            using (SqlConnection SqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlConn"].ToString()))
            {
                SqlConn.Open();
                string sCmd = "SELECT COUNT(*) FROM EQUIPMENTROLES WHERE sEquipmentRole='" + sEquipmentRole + "'";
                using (SqlCommand cmd = new SqlCommand(sCmd))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = SqlConn;
                    int iCount = (int)cmd.ExecuteScalar();
                    if (iCount > 0)
                    {
                        sCmd = "`" + sEquipmentRole + "` is already in use in this table";
                        Global.excToPopup excP = new Global.excToPopup(sCmd);
                        ProcessPopupException(excP);
                        return;
                    }
                }
                using (SqlCommand cmd = new SqlCommand("INSERT INTO EQUIPMENTROLES (sEquipmentRole, iAvgUseDurationMinutes, sComment) VALUES ('"
                    + sEquipmentRole + "'," + iAvgUseDurationMinutes + ",'" + sComment + "')"))
                {
                    try
                    {
                        cmd.CommandTimeout = 600;
                        cmd.CommandType = CommandType.Text;
                        cmd.Connection = SqlConn;
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception exc)
                    {
                        Global.excToPopup excP = new Global.excToPopup(exc.Message);
                        ProcessPopupException(excP);
                        return;
                    }
                }
            }
            ActivityLog.oLog(ActivityLog.enumLogTypes.DataInsert, 1, Enum.GetName(typeof(Global.enugSingleMFF), (int)Global.enugSingleMFF.EquipmentRoles) +
                " | " + sEquipmentRole + " | " + iAvgUseDurationMinutes.ToString() + " | " + sComment);
            txbNewEqRName.Text = "";
            txbNewEqRDur.Text = "";
            txbNewEqRComment.Text = "";
            DisplayInGrid(enumMFF);
        }

        protected void gvEqRole_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            sIdItemNameLabel = "lblEqRole";
            index = e.RowIndex;
            gvItem = gvEqRole;
            sOKCmdArg = "EqRole";
            sItemName = "Equipment Role";
            gv_RowDeleting();
        }

        protected void gvEqRole_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            string[] sa = new string[3];
            GridViewRow row = gvEqRole.Rows[e.RowIndex];
            TextBox txbEqR = (TextBox)row.FindControl("txbEqRole");
            sa[0] = Server.HtmlEncode(txbEqR.Text.Trim().Replace("'", "`"));
            if (sa[0].IndexOf(",") > -1)
            {
                Global.excToPopup excP = new Global.excToPopup("Equipment Role name must not contain a comma");
                ProcessPopupException(excP);
                return;
            }
            TextBox txbDur = (TextBox)row.FindControl("txbDur");
            int iDur = Int32.Parse(txbDur.Text);
            if (iDur < 1 || iDur > 1440)
            {
                Global.excToPopup excP = new Global.excToPopup("Average Use Duration must be greater than 0 and less than 1441");
                ProcessPopupException(excP);
                return;
            }
            sa[1] = iDur.ToString();
            TextBox txbC = (TextBox)row.FindControl("txbComment");
            sa[2] = Server.HtmlEncode(txbC.Text.Trim().Replace("'", "`"));
            using (SqlConnection SqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlConn"].ToString()))
            {
                using (SqlCommand cmd = new SqlCommand("UPDATE EQUIPMENTROLES SET sEquipmentRole='" + sa[0] + "', iAvgUseDurationMinutes=" + sa[1] +
                        ", sComment='" + sa[2] + "' WHERE sEquipmentRole='" + sTextBeforeUpdate + "'"))
                {
                    try
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Connection = SqlConn;
                        cmd.CommandTimeout = 600;
                        SqlConn.Open();
                        cmd.ExecuteNonQuery();
                        SqlConn.Close();
                    }
                    catch (Exception exc)
                    {
                        Global.excToPopup excP = new Global.excToPopup(exc.Message);
                        ProcessPopupException(excP);
                        return;
                    }
                    ActivityLog.oLog(ActivityLog.enumLogTypes.DataUpdate, 1, Enum.GetName(typeof(Global.enugInfoType),
                        (int)Global.enugSingleMFF.EquipmentRoles) + " | " + string.Join(" | ", sa));
                }
            }
            gvEqRole.EditIndex = -1;
            DisplayInGrid(Global.enugSingleMFF.EquipmentRoles);
        }

        protected void gvEqRole_RowEditing(object sender, GridViewEditEventArgs e)
        {
            index = e.NewEditIndex;
            GridViewRow row = gvEqRole.Rows[index];
            Label lbl = (Label)row.FindControl("lblEqRole");
            sTextBeforeUpdate = lbl.Text;
            gvEqRole.EditIndex = index;
            DisplayInGrid(Global.enugSingleMFF.EquipmentRoles);
        }

        protected void gvEqRole_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvItem = gvEqRole;
            enumMFF = Global.enugSingleMFF.EquipmentRoles;
            gv_RowCancelingEdit();
        }
        #endregion

        #region Launch Methods
        //====================
        protected void pbCreateLaunchM_Click(object sender, EventArgs e)
        {
            //txbItemName = txbNewLaunchM;
            //enumMFF = Global.enugSingleMFF.LaunchMethods;
            //CreateButton();


            enumMFF = Global.enugSingleMFF.LaunchMethods;
            string sLM = Server.HtmlEncode(txbNewLaunchM.Text.Trim().Replace("'", "`"));
            if (sLM.Length < 1)
            {
                Global.excToPopup excP = new Global.excToPopup("Launch method description must not be empty");
                ProcessPopupException(excP);
                return;
            }
            if (txbLMAbbrev.Text.Length < 1)
            {
                Global.excToPopup excP = new Global.excToPopup("Launch method abbreviation must not be empty");
                ProcessPopupException(excP);
                return;
            }
            char cLM = txbLMAbbrev.Text[0];
            using (SqlConnection SqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlConn"].ToString()))
            {
                SqlConn.Open();
                using (SqlCommand cmd = new SqlCommand("INSERT INTO LAUNCHMETHODS VALUES ('"
                    + sLM + "','" + cLM + "')"))
                {
                    try
                    {
                        cmd.CommandTimeout = 600;
                        cmd.CommandType = CommandType.Text;
                        cmd.Connection = SqlConn;
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception exc)
                    {
                        Global.excToPopup excP = new Global.excToPopup(exc.Message);
                        ProcessPopupException(excP);
                        return;
                    }
                }
            }
            ActivityLog.oLog(ActivityLog.enumLogTypes.DataInsert, 1, Enum.GetName(typeof(Global.enugSingleMFF), (int)Global.enugSingleMFF.LaunchMethods) +
                " | " + sLM + " | " + cLM);
            txbNewLaunchM.Text = "";
            txbLMAbbrev.Text = "";
            gvLaunchM.EditIndex = -1;
            DisplayInGrid(enumMFF);
        }

        protected void gvLaunchM_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            sIdItemNameLabel = "lblLaunchM";
            index = e.RowIndex;
            gvItem = gvLaunchM;
            sOKCmdArg = "LaunchM";
            sItemName = "Launch Method";
            gv_RowDeleting();
        }

        protected void gvLaunchM_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            //index = e.RowIndex;
            //gvItem = gvLaunchM;
            //stxbEditItemName = "txbLaunchM";
            //enumMFF = Global.enugSingleMFF.LaunchMethods;
            //gv_RowUpdating();

            enumMFF = Global.enugSingleMFF.LaunchMethods;
            TextBox txbLaunchM = (TextBox)gvLaunchM.Rows[e.RowIndex].FindControl("txbLaunchM");
            string sCC = Server.HtmlEncode(txbLaunchM.Text.Trim().Replace("'", "`"));
            if (sCC.Length < 1)
            {
                Global.excToPopup excP = new Global.excToPopup("Launch Method description must not be empty");
                ProcessPopupException(excP);
                return;
            }
            TextBox txbAbbrev = (TextBox)gvLaunchM.Rows[e.RowIndex].FindControl("txbAbbrev");
            if (txbAbbrev.Text.Length < 1)
            {
                Global.excToPopup excP = new Global.excToPopup("Launch method abbreviation must not be empty, must be one of A - Z or a - z");
                ProcessPopupException(excP);
                return;
            }
            char cCC = txbAbbrev.Text[0];
            using (SqlConnection SqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlConn"].ToString()))
            {
                SqlConn.Open();
                using (SqlCommand cmd = new SqlCommand("UPDATE LAUNCHMETHODS SET " +
                    "sLaunchMethod='" + sCC +
                    "', cLaunchMethod='" + cCC +
                    "' WHERE ID=" + sRowID)) // sRowID is set in gvChargeCode_RowEditing
                {
                    try
                    {
                        cmd.CommandTimeout = 600;
                        cmd.CommandType = CommandType.Text;
                        cmd.Connection = SqlConn;
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception exc)
                    {
                        Global.excToPopup excP = new Global.excToPopup(exc.Message);
                        ProcessPopupException(excP);
                        return;
                    }
                }
            }
            ActivityLog.oLog(ActivityLog.enumLogTypes.DataUpdate, 1, Enum.GetName(typeof(Global.enugSingleMFF), (int)Global.enugSingleMFF.LaunchMethods) +
                " | " + sCC + " | " + cCC );
            gvLaunchM.EditIndex = -1;
            DisplayInGrid(enumMFF);
        }

        protected void gvLaunchM_RowEditing(object sender, GridViewEditEventArgs e)
        {
            enumMFF = Global.enugSingleMFF.LaunchMethods;
            gvLaunchM.EditIndex = e.NewEditIndex;
            Label lbl = (Label)gvLaunchM.Rows[e.NewEditIndex].FindControl("lblID");
            sRowID = lbl.Text;
            DisplayInGrid(enumMFF);
        }

        protected void gvLaunchM_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvItem = gvLaunchM;
            enumMFF = Global.enugSingleMFF.LaunchMethods;
            gv_RowCancelingEdit();
        }
        #endregion

        #region Equipment Types
        //=====================
        protected void pbCreateEqType_Click(object sender, EventArgs e)
        {
            txbItemName = txbNewEqType;
            enumMFF = Global.enugSingleMFF.EquipmentTypes;
            CreateButton();
        }

        protected void gvEqType_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            sIdItemNameLabel = "lblEqType";
            index = e.RowIndex;
            gvItem = gvEqType;
            sOKCmdArg = "EqType";
            sItemName = "Equipment Type";
            gv_RowDeleting();
        }

        protected void gvEqType_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            index = e.RowIndex;
            gvItem = gvEqType;
            stxbEditItemName = "txbEqType";
            enumMFF = Global.enugSingleMFF.EquipmentTypes;
            gv_RowUpdating();
        }

        protected void gvEqType_RowEditing(object sender, GridViewEditEventArgs e)
        {
            sIdItemNameLabel = "lblEqType";
            index = e.NewEditIndex;
            gvItem = gvEqType;
            enumMFF = Global.enugSingleMFF.EquipmentTypes;
            gv_RowEditing();
        }

        protected void gvEqType_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvItem = gvEqType;
            enumMFF = Global.enugSingleMFF.EquipmentTypes;
            gv_RowCancelingEdit();
        }
        #endregion

        #region Equipment Action Types
        //=====================
        protected void pbCreateNewEqActionType_Click(object sender, EventArgs e)
        {
            txbItemName = txbNewEqActionType;
            enumMFF = Global.enugSingleMFF.EquipmentActionTypes;
            CreateButton();
        }

        protected void gvEqActType_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            sIdItemNameLabel = "lblEqActType";
            index = e.RowIndex;
            gvItem = gvEqActType;
            sOKCmdArg = "EqActType";
            sItemName = "Equipment Action Type";
            gv_RowDeleting();
        }

        protected void gvEqActType_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            index = e.RowIndex;
            gvItem = gvEqActType;
            stxbEditItemName = "txbEqActType";
            enumMFF = Global.enugSingleMFF.EquipmentActionTypes;
            gv_RowUpdating();
        }

        protected void gvEqActType_RowEditing(object sender, GridViewEditEventArgs e)
        {
            sIdItemNameLabel = "lblEqActType";
            index = e.NewEditIndex;
            gvItem = gvEqActType;
            enumMFF = Global.enugSingleMFF.EquipmentActionTypes;
            gv_RowEditing();
        }

        protected void gvEqActType_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvItem = gvEqActType;
            enumMFF = Global.enugSingleMFF.EquipmentActionTypes;
            gv_RowCancelingEdit();
        }
        #endregion

        #region Board of Director Offices
        protected void pbCreateOffice_Click(object sender, EventArgs e)
        {
            txbItemName = txbNewOffice;
            enumMFF = Global.enugSingleMFF.Offices;
            CreateButton();
        }

        protected void gvOffice_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            sIdItemNameLabel = "lblOffice";
            index = e.RowIndex;
            gvItem = gvOffice;
            sOKCmdArg = "Office";
            sItemName = "Board Offices";
            gv_RowDeleting();
        }

        protected void gvOffice_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            index = e.RowIndex;
            gvItem = gvOffice;
            stxbEditItemName = "txbOffice";
            enumMFF = Global.enugSingleMFF.Offices;
            gv_RowUpdating();
        }

        protected void gvOffice_RowEditing(object sender, GridViewEditEventArgs e)
        {
            sIdItemNameLabel = "lblOffice";
            index = e.NewEditIndex;
            gvItem = gvOffice;
            enumMFF = Global.enugSingleMFF.Offices;
            gv_RowEditing();
        }

        protected void gvOffice_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvItem = gvOffice;
            enumMFF = Global.enugSingleMFF.Offices;
            gv_RowCancelingEdit();
        }
        #endregion

        #region Front Accounting (FA) Items

        protected void pbCreateFAItem_Click(object sender, EventArgs e)
        {
            enumMFF = Global.enugSingleMFF.FA_AccItems;
            string sCC = Server.HtmlEncode(txbFAItem.Text.Trim().Replace("'", "`"));
            if (sCC.Length < 1)
            {
                Global.excToPopup excP = new Global.excToPopup("FA Item Code must not be empty");
                ProcessPopupException(excP);
                return;
            }
            DataIntegrityDataContext dbmdc = new DataIntegrityDataContext();
            FA_ITEM fai = new FA_ITEM();
            fai.sFA_ItemCode = sCC;
            fai.sItemDescription = Server.HtmlEncode(txbFADescr.Text.Trim().Replace("'", "`"));
            dbmdc.FA_ITEMs.InsertOnSubmit(fai);
            dbmdc.SubmitChanges();
            DisplayInGrid(enumMFF);
        }

        protected void gvFAItems_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            sIdItemNameLabel = "lblFAItemCode";
            index = e.RowIndex;
            gvItem = gvFAItems;
            sOKCmdArg = "FA_Item";
            sItemName = "FA Item";
            gv_RowDeleting();
        }

        protected void gvFAItems_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            enumMFF = Global.enugSingleMFF.FA_AccItems;
            TextBox txbFAItemCode = (TextBox)gvFAItems.Rows[e.RowIndex].FindControl("txbFAItemCode");
            string sCC = Server.HtmlEncode(txbFAItemCode.Text.Trim().Replace("'", "`"));
            if (sCC.Length < 1)
            {
                Global.excToPopup excP = new Global.excToPopup("FA Item Code must not be empty");
                ProcessPopupException(excP);
                return;
            }
            Label lblFAItemID = (Label)gvFAItems.Rows[e.RowIndex].FindControl("lblFAItemID");
            int iFAItem = int.Parse(lblFAItemID.Text);
            DataIntegrityDataContext dbmdc = new DataIntegrityDataContext();
            FA_ITEM fai = (from f in dbmdc.FA_ITEMs where f.ID == iFAItem select f).First();
            fai.sFA_ItemCode = sCC;
            TextBox txbFAItemDescr = (TextBox)gvFAItems.Rows[e.RowIndex].FindControl("txbFAItemDescr");
            fai.sItemDescription = Server.HtmlEncode(txbFAItemDescr.Text.Trim().Replace("'", "`"));
            dbmdc.SubmitChanges();

            gvFAItems.EditIndex = -1;
            DisplayInGrid(enumMFF);
        }

        protected void gvFAItems_RowEditing(object sender, GridViewEditEventArgs e)
        {
            enumMFF = Global.enugSingleMFF.FA_AccItems;
            gvFAItems.EditIndex = e.NewEditIndex;
            Label lbl = (Label)gvFAItems.Rows[e.NewEditIndex].FindControl("lblFAItemID");
            sRowID = lbl.Text;
            DisplayInGrid(enumMFF);
        }

        protected void gvFAItems_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvItem = gvFAItems;
            enumMFF = Global.enugSingleMFF.FA_AccItems;
            gv_RowCancelingEdit();
        }
        #endregion

        #region Front Accounting Payment Terms

        protected void pbFAPmtTerm_Click(object sender, EventArgs e)
        {
            enumMFF = Global.enugSingleMFF.FA_PmtTerms;
            string sCC = txbPmtTerm.Text;
            if (sCC.Length < 1)
            {
                Global.excToPopup excP = new Global.excToPopup("FA Payment Terms Code must not be empty");
                ProcessPopupException(excP);
                return;
            }
            DataIntegrityDataContext dbmdc = new DataIntegrityDataContext();
            FA_PMTTERM fapt = new FA_PMTTERM();
            fapt.iPmtTermsCode = int.Parse(sCC);
            fapt.sDescription = Server.HtmlEncode(txbPTDescr.Text.Trim().Replace("'", "`"));
            dbmdc.FA_PMTTERMs.InsertOnSubmit(fapt);
            dbmdc.SubmitChanges();
            DisplayInGrid(enumMFF);
        }

        protected void gvFAPmtTerms_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            sIdItemNameLabel = "lblFAPTID";
            index = e.RowIndex;
            gvItem = gvFAPmtTerms;
            sOKCmdArg = "FA_PmtTerm";
            sItemName = "FA_PmtTerm";
            gv_RowDeleting();
        }

        protected void gvFAPmtTerms_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            enumMFF = Global.enugSingleMFF.FA_PmtTerms;
            TextBox txbFAPmtTermsCode = (TextBox)gvFAPmtTerms.Rows[e.RowIndex].FindControl("txbFAPmtTermsCode");
            string sCC = txbFAPmtTermsCode.Text;
            if (sCC.Length < 1)
            {
                Global.excToPopup excP = new Global.excToPopup("FA Payment Term Code must not be empty");
                ProcessPopupException(excP);
                return;
            }
            Label lblFAPTID = (Label)gvFAPmtTerms.Rows[e.RowIndex].FindControl("lblFAPTID");
            int iFAPmtTerm = int.Parse(lblFAPTID.Text);
            DataIntegrityDataContext dbmdc = new DataIntegrityDataContext();
            FA_PMTTERM fapt = (from f in dbmdc.FA_PMTTERMs where f.ID == iFAPmtTerm select f).First();
            fapt.iPmtTermsCode = int.Parse(sCC);
            TextBox txbFAPTDescr = (TextBox)gvFAPmtTerms.Rows[e.RowIndex].FindControl("txbFAPTDescr");
            fapt.sDescription = Server.HtmlEncode(txbFAPTDescr.Text.Trim().Replace("'", "`"));
            dbmdc.SubmitChanges();

            gvFAPmtTerms.EditIndex = -1;
            DisplayInGrid(enumMFF);
        }

        protected void gvFAPmtTerms_RowEditing(object sender, GridViewEditEventArgs e)
        {
            enumMFF = Global.enugSingleMFF.FA_PmtTerms;
            gvFAPmtTerms.EditIndex = e.NewEditIndex;
            Label lbl = (Label)gvFAPmtTerms.Rows[e.NewEditIndex].FindControl("lblFAPTID");
            sRowID = lbl.Text;
            DisplayInGrid(enumMFF);
        }

        protected void gvFAPmtTerms_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvItem = gvFAPmtTerms;
            enumMFF = Global.enugSingleMFF.FA_PmtTerms;
            gv_RowCancelingEdit();
        }
        #endregion

        #region QBO Accounting Items

        protected void pbCreateAccItem_Click(object sender, EventArgs e)
        {
            txbItemName = txbNewAccItem;
            enumMFF = Global.enugSingleMFF.QBO_AccItem;
            CreateButton();
        }

        protected void gvAccItem_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            sIdItemNameLabel = "lblAccItem";
            index = e.RowIndex;
            gvItem = gvAccItem;
            sOKCmdArg = "AccItem";
            sItemName = "Accounting Item Name";
            gv_RowDeleting();
        }

        protected void gvAccItem_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            index = e.RowIndex;
            gvItem = gvAccItem;
            stxbEditItemName = "txbAccItem";
            enumMFF = Global.enugSingleMFF.QBO_AccItem;
            gv_RowUpdating();
        }

        protected void gvAccItem_RowEditing(object sender, GridViewEditEventArgs e)
        {
            sIdItemNameLabel = "lblAccItem";
            index = e.NewEditIndex;
            gvItem = gvAccItem;
            enumMFF = Global.enugSingleMFF.QBO_AccItem;
            gv_RowEditing();
        }

        protected void gvAccItem_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvItem = gvAccItem;
            enumMFF = Global.enugSingleMFF.QBO_AccItem;
            gv_RowCancelingEdit();
        }
        #endregion

        #region Charge Codes
        protected void pbCreateChargeCode_Click(object sender, EventArgs e)
        {
            enumMFF = Global.enugSingleMFF.ChargeCodes;
            string sCC = Server.HtmlEncode(txbsNewCC.Text.Trim().Replace("'", "`"));
            if (sCC.Length < 1)
            {
                Global.excToPopup excP = new Global.excToPopup("Charge Code description must not be empty");
                ProcessPopupException(excP);
                return;
            }
            if (txbcNewCC.Text.Length < 1)
            {
                Global.excToPopup excP = new Global.excToPopup("Charge Code character must not be empty, must be one of A - Z");
                ProcessPopupException(excP);
                return;
            }
            char cCC = txbcNewCC.Text[0];
            bool bChrg4Launch = chbNChrg4Launch.Checked;
            bool bChrg4Rental = chbNChrg4Rental.Checked;
            using (SqlConnection SqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlConn"].ToString()))
            {
                SqlConn.Open();
                using (SqlCommand cmd = new SqlCommand("INSERT INTO CHARGECODES VALUES ('"
                    + sCC + "','" + cCC + "','" + (bChrg4Launch ? "true" : "false") + "','" + (bChrg4Rental ? "true" : "false") + "')"))
                {
                    try
                    {
                        cmd.CommandTimeout = 600;
                        cmd.CommandType = CommandType.Text;
                        cmd.Connection = SqlConn;
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception exc)
                    {
                        Global.excToPopup excP = new Global.excToPopup(exc.Message);
                        ProcessPopupException(excP);
                        return;
                    }
                }
            }
            ActivityLog.oLog(ActivityLog.enumLogTypes.DataInsert, 1, Enum.GetName(typeof(Global.enugSingleMFF), (int)Global.enugSingleMFF.ChargeCodes) +
                " | " + sCC + " | " + cCC + " | " + (bChrg4Launch ? "true" : "false") + " | " + (bChrg4Rental ? "true" : "false"));
            txbcNewCC.Text = "";
            txbsNewCC.Text = "";
            gvChargeCode.EditIndex = -1;
            DisplayInGrid(enumMFF);

            //txbItemName = txbNewChargeCode;
            //enumMFF = Global.enugSingleMFF.ChargeCodes;
            //CreateButton();
        }

        protected void gvChargeCode_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            sIdItemNameLabel = "lblsChargeCode";
            index = e.RowIndex;
            gvItem = gvChargeCode;
            sOKCmdArg = "ChargeCode";
            sItemName = "Charge Code";
            gv_RowDeleting();
        }

        protected void gvChargeCode_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            enumMFF = Global.enugSingleMFF.ChargeCodes;
            TextBox txbsChargeCode = (TextBox)gvChargeCode.Rows[e.RowIndex].FindControl("txbsChargeCode");
            string sCC = Server.HtmlEncode(txbsChargeCode.Text.Trim().Replace("'", "`"));
            if (sCC.Length < 1)
            {
                Global.excToPopup excP = new Global.excToPopup("Charge Code description must not be empty");
                ProcessPopupException(excP);
                return;
            }
            TextBox txbcChargeCode = (TextBox)gvChargeCode.Rows[e.RowIndex].FindControl("txbcChargeCode");
            if (txbcChargeCode.Text.Length < 1)
            {
                Global.excToPopup excP = new Global.excToPopup("Charge Code character must not be empty, must be one of A - Z");
                ProcessPopupException(excP);
                return;
            }
            char cCC = txbcChargeCode.Text[0];
            CheckBox chbEChrg4Launch = (CheckBox)gvChargeCode.Rows[e.RowIndex].FindControl("chbEChrg4Launch");
            bool bChrg4Launch = chbEChrg4Launch.Checked;
            CheckBox chbEChrg4Rental = (CheckBox)gvChargeCode.Rows[e.RowIndex].FindControl("chbEChrg4Rental");
            bool bChrg4Rental = chbEChrg4Rental.Checked;
            using (SqlConnection SqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlConn"].ToString()))
            {
                SqlConn.Open();
                using (SqlCommand cmd = new SqlCommand("UPDATE CHARGECODES SET " +
                    "sChargeCode='" + sCC +
                    "', cChargeCode='" + cCC +
                    "', bCharge4Launch='" + (bChrg4Launch ? "true" : "false") +
                    "', bCharge4Rental='" + (bChrg4Rental ? "true" : "false") +
                    "' WHERE ID=" + sRowID )) // sRowID is set in gvChargeCode_RowEditing
                {
                    try
                    {
                        cmd.CommandTimeout = 600;
                        cmd.CommandType = CommandType.Text;
                        cmd.Connection = SqlConn;
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception exc)
                    {
                        Global.excToPopup excP = new Global.excToPopup(exc.Message);
                        ProcessPopupException(excP);
                        return;
                    }
                }
            }
            ActivityLog.oLog(ActivityLog.enumLogTypes.DataUpdate, 1, Enum.GetName(typeof(Global.enugSingleMFF), (int)Global.enugSingleMFF.ChargeCodes) +
                " | " + sCC + " | " + cCC + " | " + (bChrg4Launch ? "true" : "false") + " | " + (bChrg4Rental ? "true" : "false"));
            gvChargeCode.EditIndex = -1;
            DisplayInGrid(enumMFF);
        }

        protected void gvChargeCode_RowEditing(object sender, GridViewEditEventArgs e)
        {
            enumMFF = Global.enugSingleMFF.ChargeCodes;
            gvChargeCode.EditIndex = e.NewEditIndex;
            Label lbl = (Label)gvChargeCode.Rows[e.NewEditIndex].FindControl("lblID");
            sRowID = lbl.Text;
            DisplayInGrid(enumMFF);
        }

        protected void gvChargeCode_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvItem = gvChargeCode;
            enumMFF = Global.enugSingleMFF.ChargeCodes;
            gv_RowCancelingEdit();
        }
        #endregion

        #region Invoice Sources
        protected void pbCreateInvSrc_Click(object sender, EventArgs e)
        {
            txbItemName = txbNewInvSrc;
            enumMFF = Global.enugSingleMFF.InvoiceSources;
            CreateButton();
        }

        protected void gvInvSrc_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            sIdItemNameLabel = "lblInvSrc";
            index = e.RowIndex;
            gvItem = gvInvSrc;
            sOKCmdArg = "InvSrc";
            sItemName = "Invoice Source";
            gv_RowDeleting();
        }

        protected void gvInvSrc_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            index = e.RowIndex;
            gvItem = gvInvSrc;
            stxbEditItemName = "txbInvSrc";
            enumMFF = Global.enugSingleMFF.InvoiceSources;
            gv_RowUpdating();
        }

        protected void gvInvSrc_RowEditing(object sender, GridViewEditEventArgs e)
        {
            sIdItemNameLabel = "lblInvSrc";
            index = e.NewEditIndex;
            gvItem = gvInvSrc;
            enumMFF = Global.enugSingleMFF.InvoiceSources;
            gv_RowEditing();
        }

        protected void gvInvSrc_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvItem = gvInvSrc;
            enumMFF = Global.enugSingleMFF.InvoiceSources;
            gv_RowCancelingEdit();
        }
        #endregion

        #region Generic routines
        //======================
        private void CreateButton()
        {
            try
            {
                string ss = Server.HtmlEncode(txbItemName.Text.Trim());
                lCRUD.InsertOne(enumMFF, ss);
                txbItemName.Text = "";
                DisplayInGrid(enumMFF);
            }
            catch (Global.excToPopup exc)
            {
                ProcessPopupException(exc);
            }
        }
        private void gv_RowDeleting()
        {
            Label lblItem = (Label)gvItem.Rows[index].FindControl(sIdItemNameLabel);
            string sItem = lblItem.Text;
            ButtonsClear();
            YesButton.CommandName = "NumFKRefs";
            YesButton.CommandArgument = sItem;
            OkButton.CommandArgument = sOKCmdArg;
            lblPopupText.Text = "Confirm deletion of " + sItemName + " '" + sItem + "'";
            MPE_Show(Global.enumButtons.NoYes);
        }

        private void gv_RowEditing()
        {
            GridViewRow row = gvItem.Rows[index];
            Label lbl = (Label)row.FindControl(sIdItemNameLabel);
            sTextBeforeUpdate = lbl.Text;
            gvItem.EditIndex = index;
            DisplayInGrid(enumMFF);
        }

        private void gv_RowCancelingEdit()
        {
            gvItem.EditIndex = -1;
            DisplayInGrid(enumMFF);
        }

        private void gv_RowUpdating()
        {
            GridViewRow row = gvItem.Rows[index];
            TextBox txb = (TextBox)row.FindControl(stxbEditItemName);
            try
            {
                lCRUD.UpdateOne(enumMFF, sTextBeforeUpdate, Server.HtmlEncode(txb.Text));
                gvItem.EditIndex = -1;
                DisplayInGrid(enumMFF);
            }
            catch (Global.excToPopup exc)
            {
                ProcessPopupException(exc);
            }
        }
        #endregion

        #region Settings events
        protected void gvSettings_RowCreated(object sender, GridViewRowEventArgs e)
        {
            foreach (TableCell cell in e.Row.Cells) { cell.CssClass = "cell-padding"; }
        }

        protected void gvSettings_RowEditing(object sender, GridViewEditEventArgs e)
        {
            gvSettings.EditIndex = e.NewEditIndex;
            DisplayInGrid(Global.enugInfoType.Settings);
        }

        protected void gvSettings_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if ((e.Row.RowState & DataControlRowState.Edit) > 0)
                {
                    string ss = ((Label)e.Row.FindControl("lblID")).Text;
                    sSKey = ss;
                    ss = ((Label)e.Row.FindControl("lblUserSelectable")).Text;
                    if (ss == "False")
                    {
                        // remove update button in the last cell so that user cannot modify this row
                        e.Row.Cells[e.Row.Cells.Count - 1].Controls.Remove(e.Row.Cells[e.Row.Cells.Count - 1].Controls[0]);
                    }
                }
            }
        }

        protected void gvSettings_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvSettings.EditIndex = -1;
            DisplayInGrid(Global.enugInfoType.Settings);
        }

        protected void gvSettings_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            GridViewRow row = gvSettings.Rows[e.RowIndex];
            Label lbl;
            TextBox txb;
            string[] sa = new string[12];
            lbl = (Label)row.FindControl("lblSName");
            sa[0] = lbl.Text;
            lbl = (Label)row.FindControl("lblExplanation");
            sa[1] = lbl.Text;
            txb = (TextBox)row.FindControl("txbSVal");
            sa[2] = txb.Text;
            txb = (TextBox)row.FindControl("txbSInTable");
            sa[3] = txb.Text;
            txb = (TextBox)row.FindControl("txbComments");
            sa[4] = txb.Text;
            lbl = (Label)row.FindControl("lblUserSelectable");
            sa[5] = (lbl.Text == "False") ? "0" : "1";
            
            for (int i = 2; i < sa.Count(); i++)
            {
                if (sa[i] != null)
                {
                    sa[i] = Server.HtmlEncode(sa[i].ToString());
                }
            }
            try
            {
                int iSKey = int.Parse(sSKey);
                DataIntegrityDataContext didc = new DataIntegrityDataContext();
                var sett = (from s in didc.SETTINGs where s.ID == iSKey select s).First();
                sett.sSettingValue = sa[2];
                sett.sInTable = sa[3];
                sett.sComments = sa[4];
                didc.SubmitChanges();
            }
            catch (Exception exc)
            {
                ProcessPopupException(new Global.excToPopup(exc.Message));
            }
            finally
            {
                gvSettings.EditIndex = -1;
                DisplayInGrid(Global.enugInfoType.Settings);
            }
        }

        protected void gvSettings_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvSettings.PageIndex = e.NewPageIndex;
            DisplayInGrid(Global.enugInfoType.Settings);
        }
        #endregion

        #region Special Operations Types
        protected void pbCreateSpOpTy_Click(object sender, EventArgs e)
        {
            txbItemName = txbNewSpOpTy;
            enumMFF = Global.enugSingleMFF.SpecialOpTypes;
            CreateButton();
        }

        protected void gvSpOpTy_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            sIdItemNameLabel = "lblSpOpTy";
            index = e.RowIndex;
            gvItem = gvSpOpTy;
            sOKCmdArg = "SpecialOpTy";
            sItemName = "Special Ops Types";
            gv_RowDeleting();
        }

        protected void gvSpOpTy_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            index = e.RowIndex;
            gvItem = gvSpOpTy;
            stxbEditItemName = "txbSpOpTy";
            enumMFF = Global.enugSingleMFF.SpecialOpTypes;
            gv_RowUpdating();
        }

        protected void gvSpOpTy_RowEditing(object sender, GridViewEditEventArgs e)
        {
            sIdItemNameLabel = "lblSpOpTy";
            index = e.NewEditIndex;
            gvItem = gvSpOpTy;
            enumMFF = Global.enugSingleMFF.SpecialOpTypes;
            gv_RowEditing();
        }

        protected void gvSpOpTy_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvItem = gvSpOpTy;
            enumMFF = Global.enugSingleMFF.SpecialOpTypes;
            gv_RowCancelingEdit();
        }
        #endregion

        #region Flight Operations Scheduling Categories of Signups
        //====================
        protected void pbCrCateg_Click(object sender, EventArgs e)
        {
            enumMFF = Global.enugSingleMFF.FSCategories;
            string sCateg = Server.HtmlEncode(txbFSCNameNew.Text.Trim().Replace("'", "`"));
            char cKind = DDLFSCKindNew.SelectedValue[0];
            string sOrder = txbFSCOrderNew.Text;
            string sNotes = Server.HtmlEncode(txbFSCNotesNew.Text.Trim().Replace("'", "`"));
            if (sCateg.Length < 1)
            {
                Global.excToPopup excP = new Global.excToPopup("Category name must not be empty");
                ProcessPopupException(excP);
                return;
            }
            using (SqlConnection SqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlConn"].ToString()))
            {
                SqlConn.Open();
                string sCmd = "SELECT COUNT(*) FROM FSCATEGS WHERE sCateg='" + sCateg + "'";
                using (SqlCommand cmd = new SqlCommand(sCmd))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = SqlConn;
                    int iCount = (int)cmd.ExecuteScalar();
                    if (iCount > 0)
                    {
                        sCmd = "`" + sCateg + "` is already in use in this table";
                        Global.excToPopup excP = new Global.excToPopup(sCmd);
                        ProcessPopupException(excP);
                        return;
                    }
                }
                using (SqlCommand cmd = new SqlCommand("INSERT INTO FSCATEGS (sCateg, cKind, iOrder, sNotes) VALUES ('"
                    + sCateg + "','" + cKind + "'," + sOrder + ",'" + sNotes + "')"))
                {
                    try
                    {
                        cmd.CommandTimeout = 600;
                        cmd.CommandType = CommandType.Text;
                        cmd.Connection = SqlConn;
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception exc)
                    {
                        Global.excToPopup excP = new Global.excToPopup(exc.Message);
                        ProcessPopupException(excP);
                        return;
                    }
                }
            }
            ActivityLog.oLog(ActivityLog.enumLogTypes.DataInsert, 1, Enum.GetName(typeof(Global.enugSingleMFF), (int)Global.enugSingleMFF.FSCategories) +
                " | " + sCateg + " | " + cKind + " | " + sOrder + " | " + sNotes);
            txbFSCNameNew.Text = "";
            txbFSCNotesNew.Text = "";
            DisplayInGrid(enumMFF);
        }

        protected void gvFSCategs_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                DropDownList DDLFSCKind = (DropDownList)e.Row.FindControl("DDLFSCKind");
                char cKind = ((string)DataBinder.Eval(e.Row.DataItem, "cKind"))[0];
                if ((e.Row.RowState & DataControlRowState.Edit) > 0)
                {
                    SetDropDownByValue(DDLFSCKind, cKind.ToString());
                }
                else
                {
                    Dictionary<char, string> dictCateg = new Dictionary<char, string>();
                    dictCateg.Add('R', "Role");
                    dictCateg.Add('E', "Equipment");
                    dictCateg.Add('A', "Activity");
                    Label lblFSCKind = (Label)e.Row.FindControl("lblFSCKind");
                    lblFSCKind.Text = dictCateg[cKind];
                }
            }
        }

        protected void gvFSCategs_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            sIdItemNameLabel = "lblFSCName";
            index = e.RowIndex;
            gvItem = gvFSCategs;
            sOKCmdArg = "Categ";
            sItemName = "FSCategory";
            gv_RowDeleting();
        }

        protected void gvFSCategs_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            string[] sa = new string[4];
            GridViewRow row = gvFSCategs.Rows[e.RowIndex];
            TextBox txbC = (TextBox)row.FindControl("txbFSCCateg");
            sa[0] = Server.HtmlEncode(txbC.Text.Trim().Replace("'", "`"));

            DropDownList DDLFSCKind = (DropDownList)row.FindControl("DDLFSCKind");
            sa[1] = DDLFSCKind.SelectedValue;
            TextBox txbFSCOrder = (TextBox)row.FindControl("txbFSCOrder");
            sa[2] = txbFSCOrder.Text;
            TextBox txbFSCNotes = (TextBox)row.FindControl("txbFSCNotes");
            sa[3] = Server.HtmlEncode(txbFSCNotes.Text.Trim().Replace("'", "`"));
            using (SqlConnection SqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlConn"].ToString()))
            {
                using (SqlCommand cmd = new SqlCommand("UPDATE FSCATEGS SET sCateg='" + sa[0] + "', cKind='" + sa[1] +
                    "', iOrder=" + sa[2] + ", sNotes='" + sa[3] + "' WHERE sCateg='" + sTextBeforeUpdate + "'"))
                {
                    try
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Connection = SqlConn;
                        cmd.CommandTimeout = 600;
                        SqlConn.Open();
                        cmd.ExecuteNonQuery();
                        SqlConn.Close();
                    }
                    catch (Exception exc)
                    {
                        Global.excToPopup excP = new Global.excToPopup(exc.Message);
                        ProcessPopupException(excP);
                        return;
                    }
                    ActivityLog.oLog(ActivityLog.enumLogTypes.DataUpdate, 1, Enum.GetName(typeof(Global.enugInfoType),
                        (int)Global.enugSingleMFF.FSCategories) + " | " + string.Join(" | ", sa));
                }
            }
            gvFSCategs.EditIndex = -1;
            DisplayInGrid(Global.enugSingleMFF.FSCategories);
        }

        protected void gvFSCategs_RowEditing(object sender, GridViewEditEventArgs e)
        {
            index = e.NewEditIndex;
            GridViewRow row = gvFSCategs.Rows[index];
            Label lbl = (Label)row.FindControl("lblFSCName");
            sTextBeforeUpdate = lbl.Text;
            gvFSCategs.EditIndex = index;
            DisplayInGrid(Global.enugSingleMFF.FSCategories);
        }

        protected void gvFSCategs_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvItem = gvFSCategs;
            enumMFF = Global.enugSingleMFF.FSCategories;
            gv_RowCancelingEdit();
        }
        #endregion
        public void SetDropDownByValue(DropDownList ddl, string su)
        {
            ddl.ClearSelection();
            foreach (ListItem li in ddl.Items)
            {
                if (li.Value == su)
                {
                    li.Selected = true;
                    break;
                }
            }
        }
    }
}
