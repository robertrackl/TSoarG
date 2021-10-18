using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Configuration;
using System.Data.SqlClient;

namespace TSoar
{
    public static class CustFmt
    {
        public enum enDFmt { DateOnly, TimeOnlyMin, TimeOnlySec, DateAndTimeMin, DateAndTimeSec, DateAndTimeMinOffset, YearSlashMonth, OffsetOnly }

        public static string sFmtDate(DateTime Du, enDFmt uenDFmt)
        {
            string sDate = Du.Year.ToString("D4") + "/" + Du.Month.ToString("D2") + "/" + Du.Day.ToString("D2");
            string sTime = Du.Hour.ToString("D2") + ":" + Du.Minute.ToString("D2");
            string sSec = Du.Second.ToString("D2");
            return sDT(sDate, sTime, sSec, "", uenDFmt);
        }
        public static string sFmtDate(DateTimeOffset Du, enDFmt uenDFmt)
        {
            string sDate = Du.Year.ToString("D4") + "/" + Du.Month.ToString("D2") + "/" + Du.Day.ToString("D2");
            string sTime = Du.Hour.ToString("D2") + ":" + Du.Minute.ToString("D2");
            string sSec = Du.Second.ToString("D2");
            string sOffset = Du.Offset.ToString().Substring(0,6);
            if (sOffset.Substring(0, 1) != "-")
            {
                sOffset = "+" + sOffset.Substring(0, 5);
            }
            return sDT(sDate, sTime, sSec, sOffset, uenDFmt);
        }
        public static string sFmtDate(object ou, enDFmt uenDFmt)
        {
            if (ou == System.DBNull.Value || ou is null)
            {
                return "";
            }
            else
            {
                DateTimeOffset Du = (DateTimeOffset)ou;
                string sDate = Du.Year.ToString("D4") + "/" + Du.Month.ToString("D2") + "/" + Du.Day.ToString("D2");
                string sTime = Du.Hour.ToString("D2") + ":" + Du.Minute.ToString("D2");
                string sSec = Du.Second.ToString("D2");
                string sOffset = Du.Offset.ToString().Substring(0,6);
                if (sOffset.Substring(0, 1) != "-")
                {
                    sOffset = "+" + sOffset.Substring(0, 5);
                }
                return sDT(sDate, sTime, sSec, sOffset, uenDFmt);
            }
        }
        private static string sDT(string suDate, string suTimeMin, string suTimeSec, string suOffset, enDFmt uenDFmt)
        { 
            switch (uenDFmt)
            {
                case enDFmt.DateOnly:
                    return suDate;
                case enDFmt.TimeOnlyMin:
                    return suTimeMin;
                case enDFmt.TimeOnlySec:
                    return suTimeMin + ":" + suTimeSec;
                case enDFmt.DateAndTimeMin:
                    return suDate + " " + suTimeMin;
                case enDFmt.DateAndTimeSec:
                    return suDate + " " + suTimeMin + ":" + suTimeSec + " " + suOffset;
                case enDFmt.DateAndTimeMinOffset:
                    return suDate + " " + suTimeMin + " " + suOffset;
                case enDFmt.YearSlashMonth:
                    return suDate.Substring(0, 7);
                case enDFmt.OffsetOnly:
                    return suOffset;
                default:
                    return "Error in CustFmt.sFmtDate.sDT";
            }
        }
    }
}