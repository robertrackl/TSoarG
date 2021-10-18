using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Data;
using System.Configuration;
using System.Data.SqlClient;
using Dapper;
using TSoar.DB;
using TSoar.Developer.Debug;

namespace TSoar
{
    public class ActivityLog
    {
        // The numerical values of members of enumLogTypes (except 'None') must correspond
        //    to the values in the ID column of rows in table LOGTYPES
        public enum enumLogTypes
        {
            None, UserLogin, Warning, ErrorContinuable, ErrorSevere, Debug, DataInsert,
            DataUpdate, DataDeletion, ManualLogEntry, QBO, FailedLogin, ApplicationStart,
            ApplicationEnd, StackTrace, ErrorFatal, PopupException, UserLogout, SessionStart,
            SessionEnd, Security, SimulAppStart
        };

        //Logging for display to the website administrator
        public static Boolean bgLogging = true;
        public static void oLog(enumLogTypes euLogType, int iuDbgLvl, string sulogMessage)
        {
            if (bgLogging)
            {
                using (SqlConnection SqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlConn"].ToString()))
                {
                    SqlConn.Execute("INSERT INTO ACTIVITYLOG (DUTC, iLogType, iDbgLvl, sMsg) VALUES (@DUTC, @iLogType, @iDbgLvl, @sMsg)",
                        new { DUTC = DateTime.UtcNow, iLogType = euLogType, iDbgLvl = iuDbgLvl, sMsg = sulogMessage });
                }
            }
        }

        //Diagnostics are for display to the developer
        public static void oDiag(string suKind, string suDescription)
        {
            DateTime DNow = DateTime.UtcNow;
            using (SqlConnection SqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlConn"].ToString()))
            {
                SqlConn.Execute("INSERT INTO DIAGNOSTICS (PiT, sKind, iDbgLvl, sDescription) VALUES (@PiT, @sKind, 0, @sDescription)",
                    new { PiT = DNow, sKind = suKind, sDescription = suDescription });
            }
        }

        public static void oDiag(string suKind, int iuDbgLvl, string suDescription)
        {
            DateTime DNow = DateTime.UtcNow;
            //using (SqlConnection SqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlConn"].ToString()))
            //{
            //    SqlConn.Execute("INSERT INTO DIAGNOSTICS (PiT, sKind, iDbgLvl, sDescription) VALUES (@PiT, @sKind, @iuDbgLvl, @sDescription)",
            //        new { PiT = DNow, sKind = suKind, iDbgLvl = iuDbgLvl, sDescription = suDescription });
            //}
            StatistDataContext dc = new StatistDataContext();
            DIAGNOSTIC dia = new DIAGNOSTIC();
            dia.PiT = DNow;
            dia.sKind = suKind;
            dia.iDbgLvl = iuDbgLvl;
            dia.sDescription = suDescription;
            dc.DIAGNOSTICs.InsertOnSubmit(dia);
            dc.SubmitChanges();

            DebugControlDataContext dcdc = new DebugControlDataContext();
            List<DEVDEBUGCONTROL> q0 = (from d in dcdc.DEVDEBUGCONTROLs where d.sDebugThread == suKind select d).ToList();
            if (q0.Count > 0)
            {
                if (q0.Count > 1)
                {
                    throw new Global.excToPopup("from ActivityLog.cs.oDiag: sDebugThread '" + suKind + "' occurs more than once in table DEVDEBUGCONTROLS.");
                }
                DEVDEBUGCONTROL ddc = q0.First();
                string sDebugLog = ddc.sDebugLog;
                bool bAppend = ddc.bAppendToLog == null ? false : (bool)ddc.bAppendToLog;
                if (sDebugLog.Length > 0 && ddc.iDebugLevel > 0 && ddc.iDebugLevel >= iuDbgLvl)
                {
                    sDebugLog = HostingEnvironment.MapPath("~") + @"AppData\DebugLogs\" + sDebugLog;
                    using (StreamWriter debugfile = new StreamWriter(sDebugLog, bAppend))
                    {
                        debugfile.WriteLine(suDescription);
                    }
                    if ( !bAppend)
                    {
                        ddc.bAppendToLog = true;
                        dcdc.SubmitChanges();
                    }
                }
            }
        }

        public static int iDiag()
        {
            using (SqlConnection SqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlConn"].ToString()))
            {
                return SqlConn.ExecuteScalar<int>("SELECT COUNT(*) FROM DIAGNOSTICS");
            }
        }

        public static void DeleteDiags()
        {
            using (SqlConnection SqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlConn"].ToString()))
            {
                SqlConn.Execute("DELETE FROM DIAGNOSTICS");
            }
        }
    }
}