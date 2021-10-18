using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Configuration;
using System.Data.SqlClient;
using System.Runtime.InteropServices;

namespace TSoar.DB
{
    public class SCUD_Multi
    {
        #region Declarations
        private const string scStoredProc = "SoarMultiList_SCUD";
        private enum EnuParmTyp { eStatus, eAction, eInfoType, eKey, eInput }; // Parameter Type
        public struct SInfoType
        {
            public string sInfTyp;
            public int iSize; // Size of input string array when Action is INSERT or UPDATE
        }
        public SInfoType[] SaInfTyp = new SInfoType[36];
        #endregion

        public SCUD_Multi() // constructor
        {
            SaInfTyp[(int)Global.enugInfoType.Members].sInfTyp = "Member Data Basic";
            SaInfTyp[(int)Global.enugInfoType.Members].iSize = 15;
            SaInfTyp[(int)Global.enugInfoType.Settings].sInfTyp = "Settings";
            SaInfTyp[(int)Global.enugInfoType.Settings].iSize = 12;
            SaInfTyp[(int)Global.enugInfoType.SSA_FromTo].sInfTyp = "SSA Member FromTo";
            SaInfTyp[(int)Global.enugInfoType.SSA_FromTo].iSize = 11;
            SaInfTyp[(int)Global.enugInfoType.PeopleOffices].sInfTyp = "People Offices";
            SaInfTyp[(int)Global.enugInfoType.PeopleOffices].iSize = 5;
            SaInfTyp[(int)Global.enugInfoType.PeopleFromTo].sInfTyp = "Membership FromTo";
            SaInfTyp[(int)Global.enugInfoType.PeopleFromTo].iSize = 5;
            SaInfTyp[(int)Global.enugInfoType.Qualifics].sInfTyp = "People Qualifs";
            SaInfTyp[(int)Global.enugInfoType.Qualifics].iSize = 5;
            SaInfTyp[(int)Global.enugInfoType.Certifics].sInfTyp = "People Certifs";
            SaInfTyp[(int)Global.enugInfoType.Certifics].iSize = 5;
            SaInfTyp[(int)Global.enugInfoType.Ratings].sInfTyp = "People Ratings";
            SaInfTyp[(int)Global.enugInfoType.Ratings].iSize = 5;
            SaInfTyp[(int)Global.enugInfoType.EquityShares].sInfTyp = "Equity Shares";
            SaInfTyp[(int)Global.enugInfoType.EquityShares].iSize = 8;
            SaInfTyp[(int)Global.enugInfoType.Equipment].sInfTyp = "Equipment";
            SaInfTyp[(int)Global.enugInfoType.Equipment].iSize = 12;
            SaInfTyp[(int)Global.enugInfoType.OpsCalNames].sInfTyp = "OpsCalNames";
            SaInfTyp[(int)Global.enugInfoType.OpsCalNames].iSize = 2;
            SaInfTyp[(int)Global.enugInfoType.OpsCalTimes].sInfTyp = "OpsCalTimes";
            SaInfTyp[(int)Global.enugInfoType.OpsCalTimes].iSize = 6;
            SaInfTyp[(int)Global.enugInfoType.EquipComponents].sInfTyp = "Equipment Components";
            SaInfTyp[(int)Global.enugInfoType.EquipComponents].iSize = 9;
            SaInfTyp[(int)Global.enugInfoType.EquipParameters].sInfTyp = "Equipment Parameters";
            SaInfTyp[(int)Global.enugInfoType.EquipParameters].iSize = 12;
            SaInfTyp[(int)Global.enugInfoType.EquipAgingItems].sInfTyp = "EquipAging Items";
            SaInfTyp[(int)Global.enugInfoType.EquipAgingItems].iSize = 12;
            SaInfTyp[(int)Global.enugInfoType.EquipOpData].sInfTyp = "Equipment OpData";
            SaInfTyp[(int)Global.enugInfoType.EquipOpData].iSize = 10;
            SaInfTyp[(int)Global.enugInfoType.EquipActionItems].sInfTyp = "EquipAction Items";
            SaInfTyp[(int)Global.enugInfoType.EquipActionItems].iSize = 12;
            SaInfTyp[(int)Global.enugInfoType.Operations].sInfTyp = "Operations";
            // SaInfTyp[(int)Global.enugInfoType.Operations].iSize = 10; SCR 217
            SaInfTyp[(int)Global.enugInfoType.Operations].iSize = 11; // SCR 217
            SaInfTyp[(int)Global.enugInfoType.SpecialOp].sInfTyp = "SpecialOps";
            SaInfTyp[(int)Global.enugInfoType.SpecialOp].iSize = 6;
            SaInfTyp[(int)Global.enugInfoType.OpDetail].sInfTyp = "OpDetails";
            // SaInfTyp[(int)Global.enugInfoType.OpDetail].iSize = 7; SCR 217
            SaInfTyp[(int)Global.enugInfoType.OpDetail].iSize = 8; // SCR 217
            SaInfTyp[(int)Global.enugInfoType.Aviators].sInfTyp = "Aviators";
            SaInfTyp[(int)Global.enugInfoType.Aviators].iSize = 9;
            SaInfTyp[(int)Global.enugInfoType.FlyingCharges].sInfTyp = "FlyingCharges";
            SaInfTyp[(int)Global.enugInfoType.FlyingCharges].iSize = 6;
            SaInfTyp[(int)Global.enugInfoType.Contacts].sInfTyp = "Contacts";
            SaInfTyp[(int)Global.enugInfoType.Contacts].iSize = 9;
            SaInfTyp[(int)Global.enugInfoType.ContactTypes].sInfTyp = "ContactTypes";
            SaInfTyp[(int)Global.enugInfoType.ContactTypes].iSize = 3;
            SaInfTyp[(int)Global.enugInfoType.PhysicalAddresses].sInfTyp = "PhysicalAddresses";
            SaInfTyp[(int)Global.enugInfoType.PhysicalAddresses].iSize = 8;
            SaInfTyp[(int)Global.enugInfoType.SF_Accounts].sInfTyp = "SF_Accounts";
            SaInfTyp[(int)Global.enugInfoType.SF_Accounts].iSize = 9;
            SaInfTyp[(int)Global.enugInfoType.SF_Subledgers].sInfTyp = "SF_Subledgers";
            SaInfTyp[(int)Global.enugInfoType.SF_Subledgers].iSize = 6;
            SaInfTyp[(int)Global.enugInfoType.SF_FinInstitutions].sInfTyp = "SF_FinInstitutions";
            SaInfTyp[(int)Global.enugInfoType.SF_FinInstitutions].iSize = 4;
            SaInfTyp[(int)Global.enugInfoType.SF_BankAcctTypes].sInfTyp = "SF_BankAcctTypes";
            SaInfTyp[(int)Global.enugInfoType.SF_BankAcctTypes].iSize = 4;
            SaInfTyp[(int)Global.enugInfoType.SF_BankAccts].sInfTyp = "SF_BankAccts";
            SaInfTyp[(int)Global.enugInfoType.SF_BankAccts].iSize = 8;
            SaInfTyp[(int)Global.enugInfoType.SF_AuditTrail].sInfTyp = "SF_AuditTrail";
            SaInfTyp[(int)Global.enugInfoType.SF_AuditTrail].iSize = 11;
            SaInfTyp[(int)Global.enugInfoType.SF_FiscalPeriods].sInfTyp = "SF_FiscalPeriods";
            SaInfTyp[(int)Global.enugInfoType.SF_FiscalPeriods].iSize = 3;
            SaInfTyp[(int)Global.enugInfoType.TIRewards].sInfTyp = "TIRewards";
            SaInfTyp[(int)Global.enugInfoType.TIRewards].iSize = 9;
            SaInfTyp[(int)Global.enugInfoType.FltOpsSchedDates].sInfTyp = "FltOpsSchedDates";
            SaInfTyp[(int)Global.enugInfoType.FltOpsSchedDates].iSize = 3;
            SaInfTyp[(int)Global.enugInfoType.FltOpsSignups].sInfTyp = "FltOpsSignups";
            SaInfTyp[(int)Global.enugInfoType.FltOpsSignups].iSize = 4;
            SaInfTyp[(int)Global.enugInfoType.DevDebugControls].sInfTyp = "DevDebugControls"; // SCR 223
            SaInfTyp[(int)Global.enugInfoType.DevDebugControls].iSize = 3; // SCR 223
        }

        private SqlParameter[] Apars(string suAction, Global.enugInfoType euInfTyp, [Optional] string sKey, [Optional] string[] sau)
        {
            SqlParameter[] ap = new SqlParameter[5];

            int i = (int)EnuParmTyp.eStatus;
            ap[i] = new SqlParameter("@sStatus", SqlDbType.NVarChar, 2048)
            {
                Value = "undefined",
                Direction = ParameterDirection.InputOutput
            };

            i = (int)EnuParmTyp.eAction;
            ap[i] = new SqlParameter("@sAction", SqlDbType.NVarChar, 10)
            {
                Value = suAction
            };

            i = (int)EnuParmTyp.eInfoType;
            ap[i] = new SqlParameter("@sInfoType", SqlDbType.NVarChar, 25)
            {
                Value = SaInfTyp[(int)euInfTyp].sInfTyp
            };

            i = (int)EnuParmTyp.eKey;
            ap[i] = new SqlParameter("@sKey", SqlDbType.NVarChar, 1024)
            {
                Value = sKey.Replace("'", "`")
            };

            DataTable tvp = new DataTable("saInput");
            if (sau == null)
            {
                if ((suAction != "SELECT") && (suAction != "SELECTONE") && (suAction != "DELETE"))
                {
                    throw new Global.excToPopup("SCUD_Multi.apars: array of input data cannot be null if action is " + suAction);
                }
                sau = new string[1];
            }
            if (suAction != "UPDATEKEY")
            {
                if (suAction == "INSERT" || suAction == "UPDATE")
                {
                    if (sau.Count() != SaInfTyp[(int)euInfTyp].iSize)
                    {
                        throw new Global.excToPopup("SCUD_Multi.apars: array size " + sau.Count() + " does not match required size " +
                            SaInfTyp[(int)euInfTyp].iSize + " for " + SaInfTyp[(int)euInfTyp].sInfTyp);
                    }
                }
            }

            DataColumn column;
            DataRow row;

            // Create new DataColumn, set DataType, 
            // ColumnName and add to DataTable.    
            column = new DataColumn
            {
                DataType = System.Type.GetType("System.Int32"),
                ColumnName = "ix",
                AutoIncrement = false,
                ReadOnly = false,
                Unique = true
            };
            // Add the Column to the table
            tvp.Columns.Add(column);

            // Create second column.
            column = new DataColumn
            {
                DataType = System.Type.GetType("System.String"),
                ColumnName = "sInput",
                AutoIncrement = false,
                Caption = "sInput",
                ReadOnly = false,
                Unique = false
            };
            // Add the column to the table.
            tvp.Columns.Add(column);

            // Create and add the rows of the table
            int iCount = 0;
            foreach (string s in sau)
            {
                row = tvp.NewRow();
                row["ix"] = iCount + 2;
                if (sau[iCount] != null && sau[iCount].Length>1)
                {
                    sau[iCount]=sau[iCount].Replace("'", "`");
                }
                row["sInput"] = sau[iCount];
                tvp.Rows.Add(row);
                iCount++;
            }
            i = (int)EnuParmTyp.eInput;
            ap[i] = new SqlParameter("@saInput", SqlDbType.Structured, 1024)
            {
                Value = tvp
            };

            return ap;
        }

        public DataTable GetOne(Global.enugInfoType euMFF, string suKey)
        {
            using (DataTable dt = new DataTable())
            {
                using (SqlConnection SqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlConn"].ToString()))
                {
                    using (SqlCommand cmd = new SqlCommand(scStoredProc))
                    {
                        SqlParameter[] ap = Apars("SELECTONE", euMFF, suKey);
                        cmd.Parameters.AddRange(ap);
                        using (SqlDataAdapter sda = new SqlDataAdapter())
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Connection = SqlConn;
                            sda.SelectCommand = cmd;
                            sda.Fill(dt);
                            if ((string)ap[(int)EnuParmTyp.eStatus].Value != "OK")
                            {
                                throw new Global.excToPopup("SCUD_Multi.GetOne: " + (string)ap[(int)EnuParmTyp.eStatus].Value);
                            }
                        }
                    }
                }
                return dt;
            }
        }

        public DataTable GetAll(Global.enugInfoType euMFF)
        {
            using (DataTable dt = new DataTable())
            {
                using (SqlConnection SqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlConn"].ToString()))
                {
                    using (SqlCommand cmd = new SqlCommand(scStoredProc))
                    {
                        SqlParameter[] ap = null;
                        if (euMFF == Global.enugInfoType.SF_FinInstitutions || euMFF==Global.enugInfoType.SF_Subledgers)
                        {
                            ap = Apars("SELECT", euMFF, "DEBUG_99");
                        }
                        else
                        {
                            ap = Apars("SELECT", euMFF, "");
                        }
                        cmd.Parameters.AddRange(ap);
                        using (SqlDataAdapter sda = new SqlDataAdapter())
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Connection = SqlConn;
                            sda.SelectCommand = cmd;
                            sda.Fill(dt);
                            if ((string)ap[(int)EnuParmTyp.eStatus].Value != "OK")
                            {
                                throw new Global.excToPopup("SCUD_Multi.GetAll: " + (string)ap[(int)EnuParmTyp.eStatus].Value);
                            }
                        }
                    }
                }
                return dt;
            }
        }

        public DataTable GetSome(Global.enugInfoType euMFF, string[] sau)
        {
            using (DataTable dt = new DataTable())
            {
                using (SqlConnection SqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlConn"].ToString()))
                {
                    using (SqlCommand cmd = new SqlCommand(scStoredProc))
                    {
                        SqlParameter[] ap = Apars("SELECTSOME", euMFF, "", sau);
                        cmd.Parameters.AddRange(ap);
                        using (SqlDataAdapter sda = new SqlDataAdapter())
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Connection = SqlConn;
                            sda.SelectCommand = cmd;
                            sda.Fill(dt);
                            if ((string)ap[(int)EnuParmTyp.eStatus].Value != "OK")
                            {
                                throw new Global.excToPopup("SCUD_Multi.GetSome: " + (string)ap[(int)EnuParmTyp.eStatus].Value);
                            }
                        }
                    }
                }
                return dt;
            }
        }

        public int GetPeopleIDfromWebSiteUserName(string suUserName)
        {
            using (SqlConnection SqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlConn"].ToString()))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT ID FROM PEOPLE WHERE sUserName='" + suUserName + "'"))
                {
                    cmd.CommandType = CommandType.Text;
                    SqlConn.Open();
                    cmd.Connection = SqlConn;
                    try
                    {
                        return (int)cmd.ExecuteScalar();
                    }
                    catch
                    {
                        return 0;
                    }
                }
            }
        }

        public string GetPeopleDisplayNamefromWebSiteUserName(string suUserName)
        {
            using (SqlConnection SqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlConn"].ToString()))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT sDisplayName FROM PEOPLE WHERE sUserName='" + suUserName + "'"))
                {
                    cmd.CommandType = CommandType.Text;
                    SqlConn.Open();
                    cmd.Connection = SqlConn;
                    string ss = (string)cmd.ExecuteScalar();
                    SqlConn.Close();
                    return ss;
                }
            }
        }

        public string GetMembershipCategoryfromWebSiteUserName(string suUserName)
        {
            using (SqlConnection SqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlConn"].ToString()))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT TOP (1) MEMBERSHIPCATEGORIES.sMembershipCategory " +
                    "FROM MEMBERFROMTO INNER JOIN MEMBERSHIPCATEGORIES ON MEMBERFROMTO.iMemberCategory = MEMBERSHIPCATEGORIES.ID " +
                        "INNER JOIN PEOPLE ON MEMBERFROMTO.iPerson = PEOPLE.ID " +
                    "WHERE (MEMBERFROMTO.DMembershipBegin > '2000/01/01') AND (MEMBERFROMTO.DMembershipEnd < '2999/12/31') AND(PEOPLE.sUserName = N'" + suUserName + "')"))
                {
                    cmd.CommandType = CommandType.Text;
                    SqlConn.Open();
                    cmd.Connection = SqlConn;
                    return (string)cmd.ExecuteScalar();
                }
            }
        }

        public int Exists(Global.enugInfoType euMFF, string suExists)
        {
            using (SqlConnection SqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlConn"].ToString()))
            {
                using (SqlCommand cmd = new SqlCommand(scStoredProc))
                {
                    string[] sa=new string[1];
                    sa[0] = suExists;
                    SqlParameter[] ap = Apars("EXISTS", euMFF, "", sa);
                    cmd.Parameters.AddRange(ap);
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlConn.Open();
                    cmd.Connection = SqlConn;
                    cmd.ExecuteScalar();
                    SqlConn.Close();
                    string sres = (string)ap[(int)EnuParmTyp.eStatus].Value;
                    if (sres.Substring(0, 2) != "OK")
                    {
                        throw new Global.excToPopup("SCUD_Multi.Exists: " + (string)ap[(int)EnuParmTyp.eStatus].Value);
                    }
                    else
                    {
                        int ires = Int32.Parse(sres.Substring(2));
                        return ires;
                    }
                }
            }
        }

        public string GetSetting(string suSettingName, int iu)
        {
            using (SqlConnection SqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlConn"].ToString()))
            {
                SqlConn.Open();
                string sCmd = "SELECT sSettingValue FROM SETTINGS WHERE sSettingName='" + suSettingName + "'";
                using (SqlCommand cmd = new SqlCommand(sCmd))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = SqlConn;
                    object ob = cmd.ExecuteScalar();
                    if (!(ob.GetType()).Equals(typeof(string)))
                    {
                        return "!NOT A STRING!";
                    }
                    else
                    {
                        return (string)ob;
                    }
                }
            }
        }

        public string GetSetting(string suSettingName)
        {
            AdminPages.DBMaint.DataIntegrityDataContext didc = new AdminPages.DBMaint.DataIntegrityDataContext();
            var qs = (from s in didc.SETTINGs where s.sSettingName == suSettingName select s).First();
            if (qs.bUserSelectable)
            {
                APTSoarSetting A = AccountProfile.CurrentUser.APTSUserSelectableSettings;
                return A.sGetUSSetting(suSettingName);
            }
            else
            {
                return qs.sSettingValue;
            }
        }

        public void InsertOne(Global.enugInfoType euMFF, string[] sau, out int iuIdent)
        {
            string sDbg = "";
            DoInsertOne(0, euMFF, sau, out iuIdent, out sDbg);
        }
        public void InsertOne(int luDebug, Global.enugInfoType euMFF, string[] sau, out int iuIdent, out string suDbg)
        {
            DoInsertOne(luDebug, euMFF, sau, out iuIdent, out suDbg);
        }
        public void DoInsertOne(int luDebug, Global.enugInfoType euMFF, string[] sau, out int iuIdent, out string suDbg)
        {
            suDbg = "";
            using (SqlConnection SqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlConn"].ToString()))
            {
                SqlConn.Open();
                using (SqlCommand cmd = new SqlCommand(scStoredProc))
                {
                    cmd.CommandTimeout = 600;
                    string sKey = "";
                    if (luDebug > 0)
                    {
                        sKey = "DEBUG_" + luDebug.ToString();
                    }
                    SqlParameter[] ap = Apars("INSERT", euMFF, sKey, sau);

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddRange(ap);
                    cmd.Connection = SqlConn;
                    if (luDebug == 7)
                    {
                        suDbg = "SCUD_Multi.cs.DoInsertOne: before cmd.ExecuteNonQuery()";
                        iuIdent = -1;
                        return;
                    }
                    cmd.ExecuteNonQuery();
                    if (luDebug == 8)
                    {
                        suDbg = "SCUD_Multi.cs.DoInsertOne: after cmd.ExecuteNonQuery()";
                        iuIdent = -1;
                        return;
                    }
                    string sRet = (string)ap[(int)EnuParmTyp.eStatus].Value;
                    if (luDebug > 0)
                    {
                        suDbg = sRet;
                        iuIdent = -1;
                        return;
                    }
                    if (sRet.Substring(0,2) != "OK")
                    {
                        throw new Global.excToPopup("SCUD_Multi.DoInsertOne: " + (string)ap[(int)EnuParmTyp.eStatus].Value);
                    }
                    else
                    {
                        iuIdent = Int32.Parse(sRet.Substring(2));
                        ActivityLog.oLog(ActivityLog.enumLogTypes.DataInsert, 1, "SCUD_Multi.cs: " + Enum.GetName(typeof(Global.enugInfoType), (int)euMFF) + " | " + string.Join(" | ", sau) + " | " + sRet);
                    }
                }
            }
        }

        public void UpdateOne(Global.enugInfoType euMFF, string suKey, string[] sau)
        {
            using (SqlConnection SqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlConn"].ToString()))
            {
                using (SqlCommand cmd = new SqlCommand(scStoredProc))
                {
                    SqlParameter[] ap = Apars("UPDATE", euMFF, suKey, sau);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddRange(ap);
                    cmd.Connection = SqlConn;
                    cmd.CommandTimeout = 600;
                    SqlConn.Open();
                    cmd.ExecuteNonQuery();
                    SqlConn.Close();
                    string sret = (string)ap[(int)EnuParmTyp.eStatus].Value;
                    if ( sret != "OK")
                    {
                        if (sret.Contains("Error 2601"))
                        {
                            sret = "Did you modify the name of the equipment? The new name may already exist!";
                        }
                        throw new Global.excToPopup("SCUD_Multi.UpdateOne: " + sret);
                    }
                    else
                    {
                        ActivityLog.oLog(ActivityLog.enumLogTypes.DataUpdate, 1, Enum.GetName(typeof(Global.enugInfoType), (int)euMFF) + " | " + string.Join(" | ", sau));
                    }
                }
            }
        }

        public void DeleteOne(Global.enugInfoType euMFF, string suDelKey)
        {
            // In the special case of People Contacts, we need to check whether a physical address is attached and delete that first:
            if (euMFF == Global.enugInfoType.Contacts)
            {
                TNPV_PeopleContactsDataContext d = new TNPV_PeopleContactsDataContext();
                var q0 = from n in d.PEOPLECONTACTs where n.ID == Int32.Parse(suDelKey) select n.iPhysAddress;
                int iPhysAddr = (int)q0.First();
                if (iPhysAddr > 0)
                {
                    var q = from m in d.PHYSADDRESSes where m.ID == iPhysAddr select m;
                    foreach (var m in q) { d.PHYSADDRESSes.DeleteOnSubmit(m); }
                    d.SubmitChanges();
                }
            }

            // Delete a record with a delete key of suDelKey. For example, in table PEOPLE, the delete key must match an entry
            //  in the unique column sDisplayName before the record is deleted.
            using (SqlConnection SqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlConn"].ToString()))
            {
                SqlParameter[] ap = Apars("DELETE", euMFF, suDelKey);
                using (SqlCommand cmd = new SqlCommand(scStoredProc))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddRange(ap);
                    cmd.Connection = SqlConn;
                    SqlConn.Open();
                    cmd.ExecuteNonQuery();
                    SqlConn.Close();
                    if ((string)ap[(int)EnuParmTyp.eStatus].Value != "OK")
                    {
                        throw new Global.excToPopup("SCUD_Multi.DeleteOne: " + (string)ap[(int)EnuParmTyp.eStatus].Value);
                    }
                    else
                    {
                        ActivityLog.oLog(ActivityLog.enumLogTypes.DataDeletion, 1, Enum.GetName(typeof(Global.enugInfoType),(int)euMFF) + " | " + suDelKey);
                    }
                }
            }
        }
    }
}