using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Configuration;
using System.Data.SqlClient;

namespace TSoar.DB
{
    public class SCUD_single
    {
        #region Declarations
        private const string scStoredProc = "SoarSingleList_SCUD";
        private enum enuSCUD { SELECT, SELECTONE, EXISTS, INSERT, UPDATE, DELETE, NUMFOREIGNKEYREFS };
        private string[] saSCUD = { "SELECT", "SELECTONE", "EXISTS", "INSERT", "UPDATE", "DELETE", "NUMFKREFS" }; // where INSERT is used for Create [in 'SCUD', the C stands for 'Create']. Using INSERT is in keeping with SQL syntax.
        private enum enuParTy { eStatus, eAction, eMFF, eInput1, eInput2 }; // Parameter Type
        // The following table must be harmonized with: (1) Global.asax.cs.enugSingleMFF, (2) List of tables in Stored Procedure SoarSingleList_SCUD
        private string[] saMFF = { "Qualifications", "Ratings", "Certifications", "Membership Categories", "SSA Member Categories", "Aviator Roles",
            "Equipment Roles", "Special Ops Types", "Launch Methods", "Equipment Types", "Equipment Action Types",
            "Contact Types", "Board Offices", "Locations", "ChargeCodes", "FA Accounting Items", "FA Payment Terms", "QBO Accounting Items", "Invoice Sources",
            "FlightOpsSchedCategs"};
        #endregion

        public SCUD_single()
        {
        }

        private SqlParameter[] apars(string suAction, string suMFF, string suInput1, string suInput2)
        {
            SqlParameter[] ap = new SqlParameter[5];

            int i = (int)enuParTy.eStatus;
            ap[i] = new SqlParameter("@sStatus", SqlDbType.NVarChar, 350);
            ap[i].Value = "undefined";
            ap[i].Direction = ParameterDirection.InputOutput;

            i = (int)enuParTy.eAction;
            ap[i] = new SqlParameter("@sAction", SqlDbType.NVarChar, 10);
            ap[i].Value = suAction;

            i = (int)enuParTy.eMFF;
            ap[i] = new SqlParameter("@sMFF", SqlDbType.NVarChar, 25);
            ap[i].Value = suMFF;

            i = (int)enuParTy.eInput1;
            ap[i] = new SqlParameter("@sInput1", SqlDbType.NVarChar, 1024);
            ap[i].Value = suInput1;

            i = (int)enuParTy.eInput2;
            ap[i] = new SqlParameter("@sInput2", SqlDbType.NVarChar, 1024);
            ap[i].Value = suInput2;

            return ap;
        }

        public DataTable GetAll(Global.enugSingleMFF euMFF)
        {
            using (DataTable dt = new DataTable())
            {
                using (SqlConnection SqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlConn"].ToString()))
                {
                    using (SqlCommand cmd = new SqlCommand("SoarSingleList_SCUD"))
                    {
                        SqlParameter[] ap = apars(saSCUD[(int)enuSCUD.SELECT], saMFF[(int)euMFF], "", "");
                        cmd.Parameters.AddRange(ap);
                        using (SqlDataAdapter sda = new SqlDataAdapter())
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Connection = SqlConn;
                            sda.SelectCommand = cmd;
                            sda.Fill(dt);
                            if ((string)ap[(int)enuParTy.eStatus].Value != "OK")
                            {
                                throw new Global.excToPopup((string)ap[(int)enuParTy.eStatus].Value);
                            }
                        }
                    }
                }
                return dt;
            }
        }

        public string sGetOne(Global.enugSingleMFF euMFF, string suID)
        {
            using (SqlConnection SqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlConn"].ToString()))
            {
                using (SqlCommand cmd = new SqlCommand("SoarSingleList_SCUD"))
                {
                    SqlParameter[] ap = apars(saSCUD[(int)enuSCUD.SELECTONE], saMFF[(int)euMFF], suID, "");
                    cmd.Parameters.AddRange(ap);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Connection = SqlConn;
                    SqlConn.Open();
                    string sRet = (string)cmd.ExecuteScalar();
                    if ((string)ap[(int)enuParTy.eStatus].Value != "OK")
                    {
                        throw new Global.excToPopup((string)ap[(int)enuParTy.eStatus].Value);
                    }
                    return sRet;
                }
            }
        }

        public int iExists(Global.enugSingleMFF euMFF, string su)
        {
            using (SqlConnection SqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlConn"].ToString()))
            {
                using (SqlCommand cmd = new SqlCommand("SoarSingleList_SCUD"))
                {
                    SqlParameter[] ap = apars(saSCUD[(int)enuSCUD.EXISTS], saMFF[(int)euMFF], su, "");
                    cmd.Parameters.AddRange(ap);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Connection = SqlConn;
                    SqlConn.Open();
                    int iRet;
                    try
                    {
                        iRet = (int)cmd.ExecuteScalar();
                    }
                    catch (Exception exc)
                    {
                        throw new Global.excToPopup(exc.Message + " / " + (string)ap[(int)enuParTy.eStatus].Value);
                    }
                    if ((string)ap[(int)enuParTy.eStatus].Value != "OK")
                    {
                        throw new Global.excToPopup((string)ap[(int)enuParTy.eStatus].Value);
                    }
                    return iRet;
                }
            }
        }

        public void InsertOne(Global.enugSingleMFF euMFF, string suInput)
        {
            if (suInput.IndexOf(",") > -1)
            {
                throw new Global.excToPopup("A comma `,` is not allowed here: " + suInput);
            }
            suInput = suInput.Replace("'", "`").Trim();
            using (SqlConnection SqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlConn"].ToString()))
            {
                using (SqlCommand cmd = new SqlCommand(scStoredProc))
                {
                    SqlParameter[] ap = apars(saSCUD[(int)enuSCUD.INSERT], saMFF[(int)euMFF], suInput, "");
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddRange(ap);
                    cmd.Connection = SqlConn;
                    SqlConn.Open();
                    cmd.ExecuteNonQuery();
                    SqlConn.Close();
                    if ((string)ap[(int)enuParTy.eStatus].Value != "OK")
                    {
                        throw new Global.excToPopup((string)ap[(int)enuParTy.eStatus].Value);
                    }
                    else
                    {
                        ActivityLog.oLog(ActivityLog.enumLogTypes.DataInsert, 1, "SCUD_single.cs: " + Enum.GetName(typeof(Global.enugSingleMFF),
                            (int)euMFF) + " | " + suInput);
                    }
                }
            }
        }

        public int iNumForeignKeyRefs(Global.enugSingleMFF euMFF, string suInput)
        {
            using (SqlConnection SqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlConn"].ToString()))
            {
                SqlParameter[] ap = apars(saSCUD[(int)enuSCUD.NUMFOREIGNKEYREFS], saMFF[(int)euMFF], suInput, "");
                using (SqlCommand cmd = new SqlCommand(scStoredProc))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddRange(ap);
                    cmd.Connection = SqlConn;
                    SqlConn.Open();
                    cmd.ExecuteNonQuery();
                    SqlConn.Close();
                    if (((string)ap[(int)enuParTy.eStatus].Value).Substring(0,2) != "OK")
                    {
                        throw new Global.excToPopup((string)ap[(int)enuParTy.eStatus].Value);
                    }
                    else
                    {
                        Int32 iN;
                        string sN = ((string)ap[(int)enuParTy.eStatus].Value).Substring(2);
                        if (Int32.TryParse(sN, out iN))
                        {
                            return iN;
                        }
                        else
                        {
                            throw new Global.excToPopup("SCUD_single.cs.iNumForeignKeyRefs: " + sN + " cannot be converted to an integer");
                        }
                    }
                }
            }
        }

        public void DeleteOne(Global.enugSingleMFF euMFF, string suInput)
        {
            using (SqlConnection SqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlConn"].ToString()))
            {
                SqlParameter[] ap = apars(saSCUD[(int)enuSCUD.DELETE], saMFF[(int)euMFF], suInput,"");
                using (SqlCommand cmd = new SqlCommand(scStoredProc))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddRange(ap);
                    cmd.Connection = SqlConn;
                    SqlConn.Open();
                    cmd.ExecuteNonQuery();
                    SqlConn.Close();
                    if ((string)ap[(int)enuParTy.eStatus].Value != "OK")
                    {
                        throw new Global.excToPopup((string)ap[(int)enuParTy.eStatus].Value);
                    }
                    else
                    {
                        ActivityLog.oLog(ActivityLog.enumLogTypes.DataDeletion, 1, "SCUD_single.cs: " + Enum.GetName(typeof(Global.enugSingleMFF), (int)euMFF) + " | " + suInput);
                    }
                }
            }
        }

        public void UpdateOne(Global.enugSingleMFF euMFF, string suInputOld, string suInputNew)
        {
            if (suInputNew.IndexOf(",") > 1)
            {
                throw new Global.excToPopup("A comma `,` is not allowed here: " + suInputNew);
            }
            suInputNew = suInputNew.Replace("'", "`").Trim();
            using (SqlConnection SqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlConn"].ToString()))
            {
                SqlParameter[] ap = apars(saSCUD[(int)enuSCUD.UPDATE], saMFF[(int)euMFF], suInputOld, suInputNew);
                using (SqlCommand cmd = new SqlCommand(scStoredProc))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddRange(ap);
                    cmd.Connection = SqlConn;
                    SqlConn.Open();
                    cmd.ExecuteNonQuery();
                    SqlConn.Close();
                    if ((string)ap[(int)enuParTy.eStatus].Value != "OK")
                    {
                        throw new Global.excToPopup((string)ap[(int)enuParTy.eStatus].Value);
                    }
                    else
                    {
                        ActivityLog.oLog(ActivityLog.enumLogTypes.DataUpdate, 1, "SCUD_single.cs: " + Enum.GetName(typeof(Global.enugSingleMFF), (int)euMFF) + " | " + suInputOld + " | " + suInputNew);
                    }
                }
            }
        }
    }
}