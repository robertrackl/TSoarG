using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Globalization;
using TSoar.DB;
using Dapper;

namespace TSoar.Statistician
{
    public partial class BulkImport : System.Web.UI.Page
    {
        private enum eMode { Init, Reading, Checking, Importing };
        private SCUD_Multi mCRUD = new SCUD_Multi();
        private SCUD_single sCRUD = new SCUD_single();

        private eMode eM { get { return (eMode?)ViewState["eM"] ?? eMode.Init; } set { ViewState["eM"] = value; } }
        private Guid Gg { get { return (Guid)ViewState["Gg"]; } set { ViewState["Gg"] = value; } }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack) { EnableButtons(); }
        }

        private void EnableButtons()
        {
            FileUploadBulk.Enabled = false;
            FileUploadBulk.CssClass = "buttonDisabled";
            pbOpen.Enabled = false;
            pbOpen.CssClass = "buttonDisabled";
            pbCheck.Enabled = false;
            pbCheck.CssClass = "buttonDisabled";
            pbAddData.Enabled = false;
            pbAddData.CssClass = "buttonDisabled";
            switch (eM)
            {
                case eMode.Init:
                    FileUploadBulk.Enabled = true;
                    FileUploadBulk.CssClass = "buttonEnabled";
                    pbOpen.Enabled = true;
                    pbOpen.CssClass = "buttonEnabled";
                    break;
                case eMode.Reading:
                    pbOpen.Enabled = true;
                    pbOpen.CssClass = "buttonEnabled";
                    break;
                case eMode.Checking:
                    pbCheck.Enabled = true;
                    pbCheck.CssClass = "buttonEnabled";
                    break;
                case eMode.Importing:
                    pbAddData.Enabled = true;
                    pbAddData.CssClass = "buttonEnabled";
                    break;
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
                if ((btn.ID == "YesButton"))
                {
                    switch (OkButton.CommandArgument)
                    {
                        case "DeleteDiags":
                            ActivityLog.DeleteDiags();
                            Import();
                            break;
                    }
                }
                if ((btn.ID == "NoButton"))
                {
                    switch (OkButton.CommandArgument)
                    {
                        case "DeleteDiags":
                            Import();
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

        protected void pbReset_Click(object sender, EventArgs e)
        {
            eM = eMode.Init;
            lblResult.Text = "Nothing imported yet ...";
            gvBulk.DataSource = null;
            gvBulk.DataBind();
            lblDataRead.Text = "Nothing yet";
            gvDiag.DataSource = null;
            gvDiag.DataBind();
            lblDiagnostics.Text = "Nothing yet";
            EnableButtons();
        }

        protected void pbOpen_Click(object sender, EventArgs e)
        {
            if (FileUploadBulk.HasFile)
            {
                //ActivityLog.oDiag("Debug", "Statistician.BulkImport.aspx.cs.pbOpen_Click: after `if (FileUploadBulk.HasFile)`");
                eM = eMode.Reading;
                Gg = Guid.NewGuid();
                using (SqlConnection SqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlConn"].ToString()))
                {
                    SqlConn.Execute("DELETE FROM BULKIMPORTS");
                }
                //ActivityLog.oDiag("Debug", "Statistician.BulkImport.aspx.cs.pbOpen_Click: after `SqlConn.Execute(^DELETE FROM BULKIMPORTS^)`");
                int iCount = 0;
                string line;
                StreamReader file = new StreamReader(FileUploadBulk.FileContent);
                //ActivityLog.oDiag("Debug", "Statistician.BulkImport.aspx.cs.pbOpen_Click: before `while ((line = file.ReadLine()) != null)`");
                while ((line = file.ReadLine()) != null)
                {
                    //ActivityLog.oDiag("Debug", "Statistician.BulkImport.aspx.cs.pbOpen_Click: after `while ((line = file.ReadLine()) != null)`");
                    iCount++;
                    string[] sa = line.Split('\t');
                    DateTime DOp = DateTime.Parse(sa[2]);
                    DateTime DTkO = DateTime.Parse(sa[15]);
                    DateTime DLdg = DateTime.Parse(sa[16]);
                    DTkO = DOp.AddMinutes(DTkO.Hour * 60 + DTkO.Minute);
                    DLdg = DOp.AddMinutes(DLdg.Hour * 60 + DLdg.Minute);
                    using (SqlConnection SqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlConn"].ToString()))
                    {
                        try {
                            SqlConn.Execute("INSERT INTO BULKIMPORTS (BulkID, iRow, sLaunchMethod, DOperation, sRevision, sGliderRegId, " +
                                "sTowPlaneRegId, sTowPilot, sInstructor, s1stPilot, s2ndPilot, iNumOccup, sSpecialChrg, sLocation, dRunwayAlt, " +
                                "dReleaseAlt, DTkOTime, DLdgTime, sChargeCode, dPercChrg1stPilot, dPercChrg2ndPilot,dPercChrgSpecial, bFirstFlt, sComment " +
                                ") VALUES " +
                                "(@BulkID, @iRow, @sLaunchMethod, @DOperation, @sRevision, @sGliderRegId, " +
                                "@sTowPlaneRegId, @sTowPilot, @sInstructor, @s1stPilot, @s2ndPilot, @iNumOccup, @sPecialChrg, @sLocation, @dRunwayAlt, " +
                                "@dReleaseAlt, @DTkOTime, @DLdgTime, @sChargeCode, @dPercChrg1stPilot, @dPercChrg2ndPilot,@dPercChrgSpecial, @bFirstFlt, @sComment " +
                                ")",
                                new
                                {
                                    @BulkID = Gg,
                                    @iRow = iCount,
                                    @sLaunchMethod = sa[1],
                                    @DOperation = sa[2],
                                    @sRevision = sa[3],
                                    @sGliderRegId = sa[4],
                                    @sTowPlaneRegId = sa[5],
                                    @sTowPilot = sa[6],
                                    @sInstructor = sa[7],
                                    @s1stPilot = sa[8],
                                    @s2ndPilot = sa[9],
                                    @iNumOccup = sa[10],
                                    @sPecialChrg = sa[11],
                                    @sLocation = sa[12],
                                    @dRunwayAlt = sa[13],
                                    @dReleaseAlt = sa[14],
                                    @DTkOTime = DTkO,
                                    @DLdgTime = DLdg,
                                    @sChargeCode = sa[17],
                                    @dPercChrg1stPilot = sa[18],
                                    @dPercChrg2ndPilot = sa[19],
                                    @dPercChrgSpecial = sa[20],
                                    @bFirstFlt = (sa[21] == "Y") ? 1 : 0,
                                    @sComment = sa[22]
                                });
                        }
                        catch (Exception exc)
                        {
                            Global.excToPopup exctp = new Global.excToPopup(exc.Message);
                            ProcessPopupException(exctp);
                            ActivityLog.oLog(ActivityLog.enumLogTypes.Debug, 10, "More info: BulkImport.aspx.cs/pbOpen_Click, iCount=" + iCount.ToString() +
                                "\r\nline=" + line.Replace('\t','^'));
                            return;
                        }
                    }
                }
                ActivityLog.oDiag("Debug", "Statistician.BulkImport.aspx.cs.pbOpen_Click: before `file.Close()`");
                file.Close();
                lblResult.Text = "Data have been read from the text file";
                using (SqlConnection SqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlConn"].ToString()))
                {
                    using (SqlCommand cmd = new SqlCommand("SELECT * FROM BULKIMPORTS"))
                    {
                        using (SqlDataAdapter sda = new SqlDataAdapter())
                        {
                            cmd.CommandType = CommandType.Text;
                            cmd.Connection = SqlConn;
                            sda.SelectCommand = cmd;
                            DataTable dt = new DataTable();
                            sda.Fill(dt);
                            gvBulk.DataSource = dt;
                            gvBulk.DataBind();
                            if (gvBulk.Rows.Count > 0) { lblDataRead.Visible = false; } else { lblDataRead.Visible = true; }
                        }
                    }
                }
                eM = eMode.Checking;
                EnableButtons();
            }
        }

        protected void pbCheck_Click(object sender, EventArgs e)
        {
            int iDia = ActivityLog.iDiag();
            if (iDia > 0)
            {
                lblPopupText.Text = "Delete existing " + iDia.ToString() + " diagnostics records?";
                ButtonsClear();
                OkButton.CommandArgument = "DeleteDiags";
                MPE_Show(Global.enumButtons.NoYes);
                return;
            }

            Import();
        }
        protected void pbAddData_Click(object sender, EventArgs e)
        {
            Import();
        }
        private void Import()
        {
            int iCount = 0;
            int iID = 0;

            DataTable dt = new DataTable();
            using (SqlConnection SqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlConn"].ToString()))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM BULKIMPORTS ORDER BY iRow"))
                {
                    using (SqlDataAdapter sda = new SqlDataAdapter())
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Connection = SqlConn;
                        sda.SelectCommand = cmd;
                        sda.Fill(dt);
                    }
                }
            }

            foreach (DataRow row in dt.Rows)
            {
                string sNeighbrhd = "";
                string sDebugHelp = "";
                object obj;
                iCount++;
                try
                {
                    using (SqlConnection SqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlConn"].ToString()))
                    {
                        sNeighbrhd = "Init";
                        int iRow = (int)row["iRow"];
                        if (iCount != iRow)
                        {
                            lblPopupText.Text = "foreach counter=" + iCount + ", but iRow counter=" + iRow;
                            ButtonsClear();
                            MPE_Show(Global.enumButtons.OkOnly);
                            return;
                        }
                        string[] saOp = new string[10];
                        string[] saOpDetGlider = new string[7];
                        string[] saOpDetTowPlane = new string[7];
                        string[] saPeople = new string[12];
                        string[] saAvTowPilot = new string[9];
                        saAvTowPilot[0] = "";
                        saAvTowPilot[1] = "";
                        saAvTowPilot[5] = "0";
                        saAvTowPilot[6] = "0";
                        saAvTowPilot[7] = "0";
                        saAvTowPilot[8] = "1900/1/1";
                        string[] saAvInstructor = new string[9];
                        saAvInstructor[0] = "";
                        saAvInstructor[1] = "";
                        saAvInstructor[5] = "0";
                        saAvInstructor[6] = "0";
                        saAvInstructor[7] = "0";
                        saAvInstructor[8] = "1900/1/1";
                        string[] saAvFirstPilot = new string[9];
                        saAvFirstPilot[0] = "0";
                        saAvFirstPilot[1] = "0";
                        saAvFirstPilot[5] = "0";
                        saAvFirstPilot[6] = "0";
                        saAvFirstPilot[7] = "0";
                        saAvFirstPilot[8] = "1900/1/1";
                        string[] saAvSecondPilot = new string[9];
                        saAvSecondPilot[0] = "";
                        saAvSecondPilot[1] = "";
                        saAvSecondPilot[5] = "0";
                        saAvSecondPilot[6] = "0";
                        saAvSecondPilot[7] = "0";
                        saAvSecondPilot[8] = "1900/1/1";
                        string[] saAvSpecialChrg = new string[9];
                        saAvSpecialChrg[0] = "";
                        saAvSpecialChrg[1] = "";
                        saAvSpecialChrg[5] = "0";
                        saAvSpecialChrg[6] = "0";
                        saAvSpecialChrg[7] = "0";
                        saAvSpecialChrg[8] = "1900/1/1";
                        // Time stamp and user signature
                        saOp[0] = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.mmm");
                        saOp[1] = mCRUD.GetPeopleIDfromWebSiteUserName(HttpContext.Current.User.Identity.Name).ToString();

                        // Launch Method
                        sNeighbrhd = "LaunchMethod";
                        string ss = (string)row["sLaunchMethod"];
                        obj = sCRUD.iExists(Global.enugSingleMFF.LaunchMethods, ss);
                        if (obj == null)
                        {
                            string sDiag = "Row #" + iCount.ToString() + ": " + "Launch method " + ss + " does not exist in table LAUNCHMETHODS";
                            switch (eM)
                            {
                                case eMode.Checking:
                                    ActivityLog.oDiag("Missing Item",sDiag);
                                    break;
                                case eMode.Importing:
                                    lblPopupText.Text = sDiag;
                                    ButtonsClear();
                                    MPE_Show(Global.enumButtons.OkOnly);
                                    return;
                            }
                            iID = 0;
                        } else
                        {
                            iID = (int)obj;
                        }
                        saOp[2] = iID.ToString();

                        // Date is ignored

                        //if (iCount > 212)
                        //{
                        //    iID = 0;
                        //}
                        SqlConn.Open();

                        //Glider
                        sNeighbrhd = "Glider";
                        ss = (string)row["sGliderRegId"];
                        using (SqlCommand cmd = new SqlCommand("SELECT ID FROM EQUIPMENT WHERE sRegistrationId='" + ss + "'"))
                        {
                            cmd.CommandType = CommandType.Text;
                            cmd.Connection = SqlConn;
                            obj = cmd.ExecuteScalar();
                        }
                        if (obj == null)
                        {
                            string sDiag = "Row #" + iCount.ToString() + ": " + "Glider " + ss + " does not exist in table EQUIPMENT";
                            switch (eM)
                            {
                                case eMode.Checking:
                                    ActivityLog.oDiag("Missing Item", sDiag);
                                    break;
                                case eMode.Importing:
                                    lblPopupText.Text = sDiag;
                                    ButtonsClear();
                                    MPE_Show(Global.enumButtons.OkOnly);
                                    return;
                            }
                            iID = 0;
                        }
                        else
                        {
                            iID = (int)obj;
                        }
                        saOpDetGlider[2] = iID.ToString(); // EQUIPMENT.ID for glider

                        // Tow Plane
                        sNeighbrhd = "Tow Plane";
                        ss = (string)row["sTowPlaneRegId"];
                        using (SqlCommand cmd = new SqlCommand("SELECT ID FROM EQUIPMENT WHERE sRegistrationId='" + ss + "'"))
                        {
                            cmd.CommandType = CommandType.Text;
                            cmd.Connection = SqlConn;
                            obj = cmd.ExecuteScalar();
                        }
                        if (obj == null)
                        {
                            string sDiag = "Row #" + iCount.ToString() + ": Tow plane " + ss + " does not exist in table EQUIPMENT";
                            switch (eM)
                            {
                                case eMode.Checking:
                                    ActivityLog.oDiag("Missing Item", sDiag);
                                    break;
                                case eMode.Importing:
                                    lblPopupText.Text = sDiag;
                                    ButtonsClear();
                                    MPE_Show(Global.enumButtons.OkOnly);
                                    return;
                            }
                            iID = 0;
                        }
                        else
                        {
                            iID = (int)obj;
                        }
                        saOpDetTowPlane[2] = iID.ToString(); // EQUIPMENT.ID for tow plane

                        // Aviator Role - tow pilot
                        ss = "Tow Pilot";
                        iID = sCRUD.iExists(Global.enugSingleMFF.AviatorRoles, ss); // AVIATORROLES.ID for tow pilot

                        // Aviator record
                        saAvTowPilot[0] = saOp[0];
                        saAvTowPilot[1] = saOp[1];
                        saAvTowPilot[4] = iID.ToString(); // AVIATORS.iAviatorRole for Tow Pilot

                        // Tow Pilot
                        sNeighbrhd = "Tow Pilot";
                        ss = (string)row["sTowPilot"];
                        sDebugHelp = "Tow Pilot 1";
                        iID = mCRUD.Exists(Global.enugInfoType.Members, ss);
                        if (iID < 1)
                        {
                            sDebugHelp = "Tow Pilot 4";
                            string sDiag = "Row #" + iCount.ToString() + ": Tow pilot " + ss + " does not exist in table PEOPLE";
                            switch (eM)
                            {
                                case eMode.Checking:
                                    ActivityLog.oDiag("Will create", sDiag);
                                    break;
                                case eMode.Importing:
                                    saPeople = new string[13];
                                    saPeople[0] = saOp[0];
                                    saPeople[1] = saOp[1];
                                    saPeople[2] = "^_^"; // ^_^ indicates to stored procedure SoarMultiList_SCUD to store NULL in this field
                                    saPeople[3] = "^_^";
                                    saPeople[4] = "^_^";
                                    saPeople[5] = "^_^";
                                    saPeople[6] = "^_^";
                                    saPeople[7] = ss;
                                    saPeople[8] = "Tow pilot created during bulk import";
                                    saPeople[9] = "^_^";
                                    saPeople[10] = "^_^";
                                    saPeople[11] = "^_^";
                                    saPeople[12] = "0001-01-01 00:00:00.0 +00:00";
                                    int lDebug = Int32.Parse(txbDebug.Text);
                                    sDebugHelp = "Tow Pilot 24";
                                    string sDbg = "";
                                    mCRUD.InsertOne(lDebug, Global.enugInfoType.Members, saPeople, out iID, out sDbg);
                                    sDebugHelp = "Tow Pilot 25";
                                    if (lDebug > 0)
                                    {
                                        lblPopupText.Text = "Statistician.BulkImport.aspx.cs.Import: Tow Pilot InsertOne debug: " + sDbg;
                                        ButtonsClear();
                                        MPE_Show(Global.enumButtons.OkOnly);
                                        EnableButtons();
                                        return;
                                    }
                                    break;
                            }
                        }
                        saAvTowPilot[2] = iID.ToString(); // PEOPLE.ID for tow pilot
                        sDebugHelp = "";

                        // Instructor
                        sNeighbrhd = "Instructor";
                        ss = (string)row["sInstructor"];
                        iID = 0;
                        if (ss.Length > 0)
                        {
                            iID = sCRUD.iExists(Global.enugSingleMFF.AviatorRoles, "Instructor"); // AVIATORROLES.ID for instructor
                            saAvInstructor[0] = saOp[0];
                            saAvInstructor[1] = saOp[1];
                            saAvInstructor[4] = iID.ToString(); // AVIATORS.iAviatorRole for Instructor
                            saAvInstructor[5] = "0";
                            saAvInstructor[6] = "0";

                            iID = mCRUD.Exists(Global.enugInfoType.Members, ss);
                            if (iID < 1)
                            {
                                string sDiag = "Row #" + iCount.ToString() + ": Instructor " + ss + " does not exist in table PEOPLE";
                                switch (eM)
                                {
                                    case eMode.Checking:
                                        ActivityLog.oDiag("Will create", sDiag);
                                        break;
                                    case eMode.Importing:
                                        saPeople = new string[13];
                                        saPeople[0] = saOp[0];
                                        saPeople[1] = saOp[1];
                                        saPeople[2] = "^_^";
                                        saPeople[3] = "^_^";
                                        saPeople[4] = "^_^";
                                        saPeople[5] = "^_^";
                                        saPeople[6] = "^_^";
                                        saPeople[7] = ss;
                                        saPeople[8] = "Instructor created during bulk import";
                                        saPeople[9] = "^_^";
                                        saPeople[10] = "^_^";
                                        saPeople[11] = "^_^";
                                        saPeople[12] = "0001-01-01 00:00:00.0 +00:00";
                                        mCRUD.InsertOne(Global.enugInfoType.Members, saPeople, out iID);
                                        break;
                                }
                            }
                        }
                        saAvInstructor[2] = iID.ToString(); // PEOPLE.ID for Instructor

                        // Aviator Role - First Pilot
                        ss = "First Pilot";
                        iID = sCRUD.iExists(Global.enugSingleMFF.AviatorRoles, ss); // AVIATORROLES.ID for First Pilot

                        // Aviator record
                        saAvFirstPilot[0] = saOp[0];
                        saAvFirstPilot[1] = saOp[1];
                        saAvFirstPilot[4] = iID.ToString(); // AVIATORS.iAviatorRole for First Pilot

                        // First Pilot
                        sNeighbrhd = "First Pilot";
                        ss = (string)row["s1stPilot"];
                        iID = mCRUD.Exists(Global.enugInfoType.Members, ss);
                        if (iID < 1)
                        {
                            string sDiag = "Row #" + iCount.ToString() + ": First pilot " + ss + " does not exist in table PEOPLE";
                            switch (eM)
                            {
                                case eMode.Checking:
                                    ActivityLog.oDiag("Will create", sDiag);
                                    break;
                                case eMode.Importing:
                                    saPeople = new string[13];
                                    saPeople[0] = saOp[0];
                                    saPeople[1] = saOp[1];
                                    saPeople[2] = "^_^";
                                    saPeople[3] = "^_^";
                                    saPeople[4] = "^_^";
                                    saPeople[5] = "^_^";
                                    saPeople[6] = "^_^";
                                    saPeople[7] = ss;
                                    saPeople[8] = "First pilot created during bulk import";
                                    saPeople[9] = "^_^";
                                    saPeople[10] = "^_^";
                                    saPeople[11] = "^_^";
                                    saPeople[12] = "0001-01-01 00:00:00.0 +00:00";
                                    mCRUD.InsertOne(Global.enugInfoType.Members, saPeople, out iID);
                                    break;
                            }
                        }
                        saAvFirstPilot[2] = iID.ToString(); // PEOPLE.ID for First Pilot

                        // Second Pilot
                        sNeighbrhd = "Second Pilot";
                        ss = (string)row["s2ndPilot"];
                        iID = 0;
                        if (ss.Length > 0)
                        {
                            iID = sCRUD.iExists(Global.enugSingleMFF.AviatorRoles, "Second Pilot"); // AVIATORROLES.ID for second pilot
                            saAvSecondPilot[0] = saOp[0];
                            saAvSecondPilot[1] = saOp[1];
                            saAvSecondPilot[4] = iID.ToString(); // AVIATORS.iAviatorRole for second pilot

                            iID = mCRUD.Exists(Global.enugInfoType.Members, ss);
                            if (iID < 1)
                            {
                                string sDiag = "Row #" + iCount.ToString() + ": Second Pilot " + ss + " does not exist in table PEOPLE";
                                switch (eM)
                                {
                                    case eMode.Checking:
                                        ActivityLog.oDiag("Will create", sDiag);
                                        break;
                                    case eMode.Importing:
                                        saPeople = new string[13];
                                        saPeople[0] = saOp[0];
                                        saPeople[1] = saOp[1];
                                        saPeople[2] = "^_^";
                                        saPeople[3] = "^_^";
                                        saPeople[4] = "^_^";
                                        saPeople[5] = "^_^";
                                        saPeople[6] = "^_^";
                                        saPeople[7] = ss;
                                        saPeople[8] = "Second Pilot created during bulk import";
                                        saPeople[9] = "^_^";
                                        saPeople[10] = "^_^";
                                        saPeople[11] = "^_^";
                                        saPeople[12] = "0001-01-01 00:00:00.0 +00:00";
                                        mCRUD.InsertOne(Global.enugInfoType.Members, saPeople, out iID);
                                        break;
                                }
                            }
                        }
                        saAvSecondPilot[2] = iID.ToString(); // PEOPLE.ID for second pilot

                        // Special Charge To
                        sNeighbrhd = "Special Charge to";
                        ss = (string)row["sSpecialChrg"];
                        iID = 0;
                        if (ss.Length > 0)
                        {
                            iID = sCRUD.iExists(Global.enugSingleMFF.AviatorRoles, "ChargeTo"); // AVIATORROLES.ID for special charge person
                            saAvSpecialChrg[0] = saOp[0];
                            saAvSpecialChrg[1] = saOp[1];
                            saAvSpecialChrg[4] = iID.ToString(); // AVIATORS.iAviatorRole for special charge person

                            iID = mCRUD.Exists(Global.enugInfoType.Members, ss);
                            if (iID < 1)
                            {
                                string sDiag = "Row #" + iCount.ToString() + ": Special Charge Person " + ss + " does not exist in table PEOPLE";
                                switch (eM)
                                {
                                    case eMode.Checking:
                                        ActivityLog.oDiag("Will create", sDiag);
                                        break;
                                    case eMode.Importing:
                                        saPeople = new string[13];
                                        saPeople[0] = saOp[0];
                                        saPeople[1] = saOp[1];
                                        saPeople[2] = "^_^";
                                        saPeople[3] = "^_^";
                                        saPeople[4] = "^_^";
                                        saPeople[5] = "^_^";
                                        saPeople[6] = "^_^";
                                        saPeople[7] = ss;
                                        saPeople[8] = "Special Charge created during bulk import";
                                        saPeople[9] = "^_^";
                                        saPeople[10] = "^_^";
                                        saPeople[11] = "^_^";
                                        saPeople[12] = "0001-01-01 00:00:00.0 +00:00";
                                        mCRUD.InsertOne(Global.enugInfoType.Members, saPeople, out iID);
                                        break;
                                }
                            }
                        }
                        saAvSpecialChrg[2] = iID.ToString(); // PEOPLE.ID for special charge person

                        // Takeoff Location
                        sNeighbrhd = "Takeoff Location";
                        ss = (string)row["sLocation"];

                        using (SqlCommand cmd = new SqlCommand("SELECT ID FROM LOCATIONS WHERE sLocation='" + ss + "'"))
                        {
                            cmd.CommandType = CommandType.Text;
                            cmd.Connection = SqlConn;
                            obj = cmd.ExecuteScalar();
                        }
                        if (obj == null)
                        {
                            string sDiag = "Row #" + iCount.ToString() + ": " + "Location " + ss + " does not exist in table LOCATIONS";
                            switch (eM)
                            {
                                case eMode.Checking:
                                    ActivityLog.oDiag("Missing Item", sDiag);
                                    break;
                                case eMode.Importing:
                                    lblPopupText.Text = sDiag;
                                    ButtonsClear();
                                    MPE_Show(Global.enumButtons.OkOnly);
                                    return;
                            }
                            iID = 0;
                        }
                        else
                        {
                            iID = (int)obj;
                        }
                        saOp[3] = iID.ToString(); // OPERATIONS.iTakeoffLoc
                        saOp[5] = saOp[3]; // OPERATIONS.iLandingLoc (for Bulk import, takeoff and landing locations are assumed the same)

                        // Takeoff runway altitude is ignored

                        // Equipment roles are fixed for bulk import:
                        iID = sCRUD.iExists(Global.enugSingleMFF.EquipmentRoles, "Glider");
                        saOpDetGlider[4] = iID.ToString();
                        iID = sCRUD.iExists(Global.enugSingleMFF.EquipmentRoles, "Tow Plane");
                        saOpDetTowPlane[4] = iID.ToString();

                        // Release altitude
                        sNeighbrhd = "Release Altitude";
                        ss = ((Decimal)row["dReleaseAlt"]).ToString();
                        saOpDetTowPlane[6] = ss;
                        saOpDetGlider[6] = ss;
                        // Max altitude is unknown in bulk import
                        saOpDetTowPlane[5] = "^_^";
                        saOpDetGlider[5] = "^_^";

                        // Takeoff Time
                        sNeighbrhd = "Takeoff Time";
                        saOp[4] = ((DateTime)row["DTkOTime"]).ToString("yyyy-MM-dd HH:mm");

                        // Landing Time
                        saOp[6] = ((DateTime)row["DLdgTime"]).ToString("yyyy-MM-dd HH:mm");

                        // Charge Code
                        sNeighbrhd = "Charge Code";
                        ss = (string)row["sChargeCode"];
                        switch (ss)
                        {
                            case "F":
                                ss = "Full Charge, Tow by Altitude";
                                break;
                            case "D":
                                ss = "Full Charge, Tow by Duration";
                                break;
                            case "G":
                                ss = "Glider Only";
                                break;
                            case "T":
                                ss = "Tow Only";
                                break;
                            case "I":
                                ss = "Free (Instructor Currency)";
                                break;
                            case "A":
                                ss = "Free (Aircraft Relocation)";
                                break;
                            case "P":
                                ss = "Free (Tow Pilot Intro)";
                                break;
                            case "X":
                                ss = "Free (Other - explain)";
                                break;
                            case "C":
                                ss = "Introductory Ride";
                                break;
                            case "M":
                                ss = "Manual Charge Entry";
                                break;
                            default:
                                lblPopupText.Text = "Row #" + iCount.ToString() + ": Unknown Charge Code " + ss;
                                MPE_Show(Global.enumButtons.OkOnly);
                                return;
                        }
                        using (SqlCommand cmd = new SqlCommand("SELECT ID FROM CHARGECODES WHERE sChargeCode='" + ss + "'"))
                        {
                            cmd.CommandType = CommandType.Text;
                            cmd.Connection = SqlConn;
                            obj = cmd.ExecuteScalar();
                        }
                        if (obj == null)
                        {
                            lblPopupText.Text = "Row #" + iCount.ToString() + ": Charge Code " + ss + " does not exist in table CHARGECODES";
                            MPE_Show(Global.enumButtons.OkOnly);
                            return;
                        }
                        iID = (int)obj;
                        saOp[8] = iID.ToString();

                        // Percent Charge 1st Pilot
                        sNeighbrhd = "Percent Charge";
                        saAvFirstPilot[5] = ((Decimal)row["dPercChrg1stPilot"]).ToString();
                        // Percent Charge 2nd Pilot
                        saAvSecondPilot[5] = ((Decimal)row["dPercChrg2ndPilot"]).ToString();
                        // Percent Charge Special Charge To
                        saAvSpecialChrg[5] = ((Decimal)row["dPercChrgSpecial"]).ToString();

                        // First flight of season with instructor?
                        sNeighbrhd = "First Flight";
                        Boolean b1 = (Boolean)row["bFirstFlt"];
                        if (saAvInstructor[0] != null)
                        {
                            if (saAvInstructor[0].Length > 0)
                            { // An instructor exists for this operation.
                                if (b1)
                                {
                                    // It's a first flight of season with instructor
                                    saAvFirstPilot[6] = "1";
                                }
                                else
                                {
                                    saAvFirstPilot[6] = "0";
                                }
                            }
                        }
                        else
                        {
                            saAvFirstPilot[6] = "0";
                        }

                        // Comments
                        ss = (string)row["sComment"];
                        if (ss.Length > 0)
                        {
                            saOp[7] = ss;
                        }
                        else
                        {
                            saOp[7] = "^_^";
                        }
                        saOp[9] = "-1"; // Field iInvoices2go
                        sNeighbrhd = "`switch (eM)`";
                        switch (eM)
                        {
                            case eMode.Checking:
                                // Done checking
                                break;
                            case eMode.Importing:

                                // Create Operation record, but only if it does not already exist
                                sNeighbrhd = "Create Operation Record";
                                using (SqlCommand cmd = new SqlCommand(
                                    "SELECT ID FROM OPERATIONS " +
                                    "WHERE iLaunchMethod = " + saOp[2] +
                                        " AND iTakeOffLoc = " + saOp[3] +
                                        " AND DBegin = '" + saOp[4] +
                                        "' AND iLandingLoc = " + saOp[5] +
                                        " AND DEnd = '" + saOp[6] +
                                        "' AND iChargeCode =" + saOp[8])) // ignoring field sComment
                                {
                                    cmd.CommandType = CommandType.Text;
                                    cmd.Connection = SqlConn;
                                    obj = cmd.ExecuteScalar();
                                }
                                if (obj != null)
                                { // This operation exists already
                                    iID = (int)obj;
                                }
                                else
                                {
                                    mCRUD.InsertOne(Global.enugInfoType.Operations, saOp, out iID);
                                }

                                // Create OpDetail record for glider if it doesn't already exist
                                sNeighbrhd = "Create OpDetail Record";
                                saOpDetGlider[3] = iID.ToString(); // to which operation does this OpDetail belong?
                                int iIDOpDetGlider = 0;
                                using (SqlCommand cmd = new SqlCommand(
                                    "SELECT ID FROM OPDETAILS " +
                                    "WHERE iEquip = " + saOpDetGlider[2] +
                                        " AND iOperation = " + saOpDetGlider[3] +
                                        " AND iEquipmentRole = " + saOpDetGlider[4] +
                                        " AND dReleaseAltitude = " + saOpDetGlider[6]
                                        ))
                                {
                                    cmd.CommandType = CommandType.Text;
                                    cmd.Connection = SqlConn;
                                    obj = cmd.ExecuteScalar();
                                }
                                if (obj != null)
                                { // This OpDetail exists already
                                    iIDOpDetGlider = (int)obj;
                                }
                                else
                                {
                                    saOpDetGlider[0] = saOp[0];
                                    saOpDetGlider[1] = saOp[1];
                                    mCRUD.InsertOne(Global.enugInfoType.OpDetail, saOpDetGlider, out iIDOpDetGlider);
                                }

                                // Create OpDetail record for tow plane if it doesn't already exist
                                saOpDetTowPlane[3] = iID.ToString(); // to which operation does this OpDetail belong?
                                int iIDOpDetTowPlane = 0;
                                using (SqlCommand cmd = new SqlCommand(
                                    "SELECT ID FROM OPDETAILS " +
                                    "WHERE iEquip = " + saOpDetTowPlane[2] +
                                        " AND iOperation = " + saOpDetTowPlane[3] +
                                        " AND iEquipmentRole = " + saOpDetTowPlane[4] +
                                        " AND dReleaseAltitude = " + saOpDetTowPlane[6]
                                        ))
                                {
                                    cmd.CommandType = CommandType.Text;
                                    cmd.Connection = SqlConn;
                                    obj = cmd.ExecuteScalar();
                                }
                                if (obj != null)
                                { // This OpDetail exists already
                                    iIDOpDetTowPlane = (int)obj;
                                }
                                else
                                {
                                    saOpDetTowPlane[0] = saOp[0];
                                    saOpDetTowPlane[1] = saOp[1];
                                    mCRUD.InsertOne(Global.enugInfoType.OpDetail, saOpDetTowPlane, out iIDOpDetTowPlane);
                                }
                                sNeighbrhd = "Insert Aviator";
                                saAvTowPilot[3] = iIDOpDetTowPlane.ToString();
                                InsertAviator(saAvTowPilot, SqlConn);
                                saAvFirstPilot[3] = iIDOpDetGlider.ToString();
                                InsertAviator(saAvFirstPilot, SqlConn);

                                if (saAvInstructor[0].Length > 0)
                                {
                                    saAvInstructor[3] = iIDOpDetGlider.ToString();
                                    InsertAviator(saAvInstructor, SqlConn);
                                }
                                
                                if (saAvSecondPilot[0].Length > 0)
                                {
                                    saAvSecondPilot[3] = iIDOpDetGlider.ToString();
                                    InsertAviator(saAvSecondPilot, SqlConn);
                                }
                                
                                if (saAvSpecialChrg[0].Length > 0)
                                {
                                    saAvSpecialChrg[3] = iIDOpDetGlider.ToString();
                                    InsertAviator(saAvSpecialChrg, SqlConn);
                                }
                                break;
                        }
                        SqlConn.Close();
                    }
                }
                catch (Global.excToPopup excTP) when (excTP.InnerException != null)
                {
                    lblPopupText.Text = "excToPopup in Row " + iCount.ToString() + ", neighborhood=^" + sNeighbrhd + "^, ^" + sDebugHelp + "^: " + excTP.sExcMsg() + (char)10;
                    lblPopupText.Text += "Inner Exception:" + excTP.InnerException.Message + (char)10 + excTP.StackTrace;
                    ButtonsClear();
                    MPE_Show(Global.enumButtons.OkOnly);
                    return;
                }
                catch (Global.excToPopup excTP)
                {
                    lblPopupText.Text = "excToPopup in Row " + iCount.ToString() + ", neighborhood=^" + sNeighbrhd + "^, ^" + sDebugHelp + "^: " + excTP.sExcMsg();
                    ButtonsClear();
                    MPE_Show(Global.enumButtons.OkOnly);
                    return;
                }
                catch (Exception exc)
                {
                    lblPopupText.Text = "Exception in Row " + iCount.ToString() + ", neighborhood=^" + sNeighbrhd + "^, ^" + sDebugHelp + "^: " + exc.Message;
                    ButtonsClear();
                    MPE_Show(Global.enumButtons.OkOnly);
                    return;
                }
            } // foreach row in dt
            switch (eM)
            {
                case eMode.Checking:
                    // display the diagnostics
                    StatistDataContext d = new StatistDataContext();
                    var q = from m in d.DIAGNOSTICs orderby m.PiT select m;
                    gvDiag.DataSource = q;
                    gvDiag.DataBind();
                    eM = eMode.Importing; lblResult.Text = "Check and Diagnosis phase done";
                    break;
                case eMode.Importing:
                    eM = eMode.Init; lblResult.Text = "Import Complete";
                    break;
            }
            EnableButtons();
        }

        private void InsertAviator(string[] sauAv, SqlConnection SqlConn)
        {
            // Create aviator record if it does not already exist
            int iIDAv = 0;
            object obj = null;
            using (SqlCommand cmd = new SqlCommand(
                "SELECT ID FROM AVIATORS " +
                "WHERE iPerson = " + sauAv[2] +
                    " AND iOpDetail = " + sauAv[3] +
                    " AND iAviatorRole = " + sauAv[4] +
                    " AND dPercentCharge = " + sauAv[5] +
                    " AND b1stFlight = " + sauAv[6]
                )
            )
            {
                cmd.CommandType = CommandType.Text;
                cmd.Connection = SqlConn;
                obj = cmd.ExecuteScalar();
            }
            if (obj == null)
            { // Aviator record does not exist
                mCRUD.InsertOne(Global.enugInfoType.Aviators, sauAv, out iIDAv);
            }

        }
    }
}