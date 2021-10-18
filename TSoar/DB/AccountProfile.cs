using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web.Profile;
using System.Web.Security;
using System.Configuration;
using System.Data.SqlClient;
//using Dapper;

namespace TSoar.DB
{
    public class APTSoarSetting
    {
        public DataTable APTS;
        // The abbreviation 'US' means User Selectable
        public string sGetUSSetting(string suSettingName)
        {
            DataRow[] rows = APTS.Select("sSettingName = '" + suSettingName + "'");
            int iSize = rows.GetLength(0);
            if (iSize < 1)
            { 
                throw new Global.excToPopup("APTSoarSetting.sGetUSSetting error: row with '" + suSettingName + "' not found");
            }
            if (iSize > 1)
            {
                throw new Global.excToPopup("APTSoarSetting.sGetUSSetting error: row with '" + suSettingName + "' found " + iSize.ToString() + " times.");
            }
            return (string)rows[0][1];
        }

        public decimal dGetVersion()
        {
            return Convert.ToDecimal(sGetUSSetting("Version"));
        }

        public void SetUSSetting(string suSettingName, string suSettingValue)
        {
            DataRow[] rows = APTS.Select("sSettingName = '" + suSettingName + "'");
            int iSize = rows.GetLength(0);
            if (iSize < 1)
            {
                throw new Global.excToPopup("APTSoarSetting.sSetUSSetting error: row with '" + suSettingName + "' not found");
            }
            if (iSize > 1)
            {
                throw new Global.excToPopup("APTSoarSetting.sSetUSSetting error: row with '" + suSettingName + "' found " + iSize.ToString() + " times.");
            }
            rows[0][1] = suSettingValue;
        }

        public bool bVersionSynch()
        {
            AdminPages.DBMaint.DataIntegrityDataContext didc = new AdminPages.DBMaint.DataIntegrityDataContext();
            bool bret = false; // no synchronization was required
            // Check that version in APTS equals the latest version setting in Global.asax.cs
            // Assumption: any change in the list of sSettingName in table SETTINGS results in a different version, i.e.,
            //    a different sSettingValue for sSettingName = 'Version', and a corresponding change in the constant Global.dgcVersionUserSelectableSettingsDataTable.
            if (dGetVersion() != Global.dgcVersionUserSelectableSettingsDataTable)
            {
                // Synchronize the settings in DataTable APTS with the contents of table SETTINGS
                //   Items not in APTS but in SETTINGS: add item to APTS with value taken from SETTINGS
                //   Items not in SETTINGS but in APTS: remove from APTS

                DataTable dtemp = APTS.Copy();
                DataColumn col = new DataColumn
                {
                    DataType = System.Type.GetType("System.Char"),
                    ColumnName = "cStatus",
                    Unique = false,
                    DefaultValue = 'U' // unknown
                };
                dtemp.Columns.Add(col);
                int iCnt = 0;
                var o = (from s in didc.SETTINGs where s.bUserSelectable || s.sSettingName == "Version" orderby s.sSettingName descending select s).ToList();
                iCnt = o.Count();
                foreach (var s in o)
                {
                    DataRow[] rows = dtemp.Select("sSettingName = '" + s.sSettingName + "'");
                    int iCount = rows.GetLength(0);
                    if (iCount < 1)
                    {
                        DataRow drow = dtemp.NewRow();
                        drow[0] = s.sSettingName;
                        drow[1] = s.sSettingValue;
                        drow[2] = 'O'; // OK
                        dtemp.Rows.Add(drow);
                        bret = true;
                    }
                    else if (iCount == 1)
                    {
                        rows[0][2] = 'O'; // OK
                        if ((string)rows[0][0] == "Version")
                        {
                            rows[0][1] = Global.dgcVersionUserSelectableSettingsDataTable.ToString();
                            bret = true;
                        }
                    }
                    else
                    {
                        throw new Global.excToPopup("APTSoarSetting.VersionSynch error: Setting Name `" + s.sSettingName + "` found more than once.");
                    }
                }
                // Any rows left with 'U' in the status column need to be removed because the setting name was not found in table SETTINGS
                int iRow = dtemp.Rows.Count;
                for (int i = iRow - 1; i >= 0; i--)
                {
                    if ((char)dtemp.Rows[i][2] == 'U')
                    {
                        dtemp.Rows.Remove(dtemp.Rows[i]);
                        bret = true;
                     }
                }
                // Remove the status column
                dtemp.Columns.Remove(dtemp.Columns[2]);
                APTS = dtemp.Copy();
                // Quality assurance:
                if (APTS.Rows.Count != iCnt)
                {
                    throw new Global.excToPopup("APTSoarSetting.VersionSynch error: APTS.Rows.Count=" + APTS.Rows.Count + " does not equal " + iCnt + "=Settings count");
                }
            }
            return bret;
        }
    }

    public class AccountProfile : ProfileBase
    {
        // Reference: https://stackoverflow.com/questions/426609/how-to-assign-profile-values , answer by Joel Spolsky, edited by Sky Sanders in 2010

        static public AccountProfile CurrentUser
        {
            get { return (AccountProfile)ProfileBase.Create(Membership.GetUser().UserName); }
        }

        #region User-Selectable Settings
        // These settings can be altered by the website user. Table SETTINGS provides initial values.
        //    All entries in table SETTINGS which have bUserSelectable==true should have a counterpart here.

        public APTSoarSetting APTSUserSelectableSettings
        {
            get
            {
                try
                {
                    APTSoarSetting apt = (APTSoarSetting)base["APTSUserSelectableSettings"];
                    if (apt==null || apt.APTS == null)
                    {
                        throw new System.Configuration.SettingsPropertyNotFoundException("null");
                    }
                    else
                    {
                        if (apt.bVersionSynch())
                        {
                            // A synchronization action took place; need to replace the Settings in the user profile:
                            base["APTSUserSelectableSettings"] = apt; Save();
                        }
                        return apt;
                    }
                }
                catch (System.Configuration.SettingsPropertyNotFoundException)
                {
                    // Create a new DataTable. Its initial contents is taken from database table SETTINGS:
                    DataTable dt = new DataTable("APTS");
                    DataColumn column = new DataColumn
                    {
                        DataType = System.Type.GetType("System.String"),
                        ColumnName = "sSettingName",
                        Unique = true
                    };
                    dt.Columns.Add(column);
                    column = new DataColumn
                    {
                        DataType = System.Type.GetType("System.String"),
                        ColumnName = "sSettingValue",
                        Unique = false
                    };
                    dt.Columns.Add(column);

                    bool bVersionFound = false;
                    AdminPages.DBMaint.DataIntegrityDataContext didc = new AdminPages.DBMaint.DataIntegrityDataContext();
                    var q = from s in didc.SETTINGs where s.bUserSelectable || s.sSettingName == "Version" orderby s.sSettingName descending select s;
                    foreach (var s in q)
                    {
                        if (s.sSettingName == "Version")
                        {
                            bVersionFound = true;
                            string sV = Global.dgcVersionUserSelectableSettingsDataTable.ToString();
                            if (s.sSettingValue != sV)
                            {
                                throw new Global.excToPopup("Settings Version `" + s.sSettingValue + "` is not the same as `" + sV +
                                    "` in Global.dgcVersionUserSelectableSettingsDataTable");
                            }
                        }
                        DataRow dr = dt.NewRow();
                        dr[0] = s.sSettingName;
                        dr[1] = s.sSettingValue;
                        dt.Rows.Add(dr);
                    }
                    if (!bVersionFound)
                    {
                        throw new Global.excToPopup("AccountProfile.APTSoarSetting.get: a row with sSettingName = `Version` was not found in table SETTINGS");
                    }
                    APTSoarSetting apt = new APTSoarSetting();
                    apt.APTS = dt;
                    return apt;
                }
            }

            set { base["APTSUserSelectableSettings"] = value; Save(); }
        }
        #endregion

        #region Various Filter Settings
        public string OpsFilterSettingSelection
        {
            get { return (string)base["OpsFilterSettingSelection"]; }
            set { base["OpsFilterSettingSelection"] = value; Save(); }
        }

        public DataTable OpsStdFilterSetting
        {
            get { return (DataTable)base["OpsStdFilterSetting"]; }
            set { base["OpsStdFilterSetting"] = value; Save(); }
        }

        public DataTable OpsAdvFilterSetting
        {
            get { return (DataTable)base["OpsAdvFilterSetting"]; }
            set { base["OpsAdvFilterSetting"] = value; Save(); }
        }

        public Global.strgMbrContactsFilter MCFsettings // Member Contacts Filter settings, used in CMS_Contacts.aspx.cs
        {
            get { return (Global.strgMbrContactsFilter)base["MCFsettings"]; }
            set { base["MCFsettings"] = value; Save(); }
        }

        public DataTable XactFilterSettings
        {
            get { return (DataTable)base["XactFilterSettings"]; }
            set { base["XactFilterSettings"] = value; Save(); }
        }

        public DataTable XactSortSettings
        {
            get { return (DataTable)base["XactSortSettings"]; }
            set { base["XactSortSettings"] = value; Save(); }
        }

        public bool bShowXactTime
        {
            get { return (bool)base["bShowXactTime"]; }
            set { base["bShowXactTime"] = value; Save(); }
        }

        public DataTable RewardsFilterSettings
        {
            get { return (DataTable)base["RewardsFilterSettings"]; }
            set { base["RewardsFilterSettings"] = value; Save(); }
        }

        public DataTable FlyOpsInvoiceFilterSettings
        {
            get { return (DataTable)base["FlyOpsInvoiceFilterSettings"]; }
            set { base["FlyOpsInvoiceFilterSettings"] = value; Save(); }
        }
        #endregion

        #region Equipment-related settings
        public bool bShowEquipMgtOpDataStartEndTimes
        {
            get { return (bool)base["bShowEquipMgtOpDataStartEndTimes"]; }
            set { base["bShowEquipMgtOpDataStartEndTimes"] = value; Save(); }
        }

        public bool bShowEqAgItemsStartEndTimes
        {
            get { return (bool)base["bShowEqAgItemsStartEndTimes"]; }
            set { base["bShowEqAgItemsStartEndTimes"] = value; Save(); }
        }

        public bool bShowEqActItemsStartEndTimes
        {
            get { return (bool)base["bShowEqActItemsStartEndTimes"]; }
            set { base["bShowEqActItemsStartEndTimes"] = value; Save(); }
        }

        public bool bShowEqComponentsLinkBeginEndTimes
        {
            get { return (bool)base["bShowEqComponentsLinkBeginEndTimes"]; }
            set { base["bShowEqComponentsLinkBeginEndTimes"] = value; Save(); }
        }

        public int? iLastEquipComponent
        {
            get { return (int?)base["iLastEquipComponent"]; }
            set { base["iLastEquipComponent"] = value; Save(); }
        }
        #endregion

        #region Settings related to Daily Flight Logs
        // Default values for entering main tow equipment, main tow operator, and main location into daily flight logs
        public DataTable DailyFlightLogsDefaults
        {
            get { return (DataTable)base["DailyFlightLogsDefaults"]; }
            set { base["DailyFlightLogsDefaults"] = value; Save(); }
        }

        // Default values for entering data into flight log rows
        public DataTable FlightLogRowsDefaults
        {
            get { return (DataTable)base["FlightLogRowsDefaults"]; }
            set { base["FlightLogRowsDefaults"] = value; Save(); }
        }
        #endregion

    }
}
