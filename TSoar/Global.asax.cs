using System;
using System.IO;
using System.Net.Mail;
using System.Web;
using System.Threading;
using System.Globalization;
using TSoar.DB;

namespace TSoar
{
    public class Global : HttpApplication
    {
        private static readonly int[] iaLast = new int[] { 0, 0, -1, -1, -1, -1 }; // First two array elements intentionally never used

        public const string sgcAppName = "TSoar";
        public const string scgHomeShow = @"\i\HomeShow\";
        public const string scgRelPathUpload = "/Accounting/UploadedFiles";
        public static string sgMigrationLevelDevIntProd = "Dev";
        public static string sgVersion = "00";
        public static DateTimeOffset DTO_NotStarted = new DateTimeOffset(2001, 1, 1, 1, 1, 0, new TimeSpan(0));
        public static DateTimeOffset DTO_NotCompleted = new DateTimeOffset(2999, 12, 31, 22, 59, 0, new TimeSpan(0));
        public static DateTimeOffset DTO_EqAgEarliest = new DateTimeOffset(1900, 1, 1, 1, 1, 0, new TimeSpan(0)); // Earliest valid date in equipment aging
        public static DateTimeOffset DTO_EqAgLatest = new DateTimeOffset(2999, 12, 31, 22, 59, 0, new TimeSpan(0)); // Latest valid date in equipment aging
        //public static int igRequestCount = 0;
        public static string[] sagRoles = new string[] { "Accountant", "Admin", "Board", "Bookkeeper", "Developer", "Test_Engineer", "Equipment", "Historian", "Instructor",
                "Media", "Member", "Membership", "Operations", "Statistician", "Towpilot", "Treasury"};
        public enum engRoles { Accountant, Admin, Board, Bookkeeper, Developer, Test_Engineer, Equipment, Historian, Instructor,
            Media, Member, Membership, Operations, Statistician, Towpilot, Treasury };
        public enum enugSingleMFF { Qualifications, Ratings, Certifications, MembershipCategories, SSA_MemberCategories, AviatorRoles, EquipmentRoles, SpecialOpTypes,
            LaunchMethods, EquipmentTypes, EquipmentActionTypes, ContactTypes, Offices, Locations, ChargeCodes, FA_AccItems, FA_PmtTerms, QBO_AccItem, InvoiceSources,
            FSCategories };
        public enum enugInfoType { Members, Settings, PeopleOffices, PeopleFromTo, SSA_FromTo, Qualifics, Certifics, Ratings, EquityShares,
            Equipment, Operations, SpecialOp, OpDetail, Aviators, FlyingCharges, Contacts, ContactTypes, PhysicalAddresses, SF_Accounts, SF_Subledgers,
            SF_FinInstitutions, SF_BankAcctTypes, SF_BankAccts, SF_AuditTrail, SF_FiscalPeriods, TIRewards, OpsCalNames, OpsCalTimes, EquipComponents, EquipParameters,
            EquipOpData, EquipAgingItems, EquipActionItems, FltOpsSchedDates, FltOpsSignups, DevDebugControls };
        public enum enumButtons { NoYes, OkOnly, OkCancel };
        public enum enumDepths { Root, Year, Month, Day, Operation, OpDetail, Aviator }; // TreeView display depths
        public const decimal dgcVersionOfOpsAdvFilterSettingDataTable = 2.0M; // in Statistician/StatisticianwFilters.cs.dtDefaultAdvFilterSettings - region FilterVersion
                        // Version 2.0M: Added SpecialOps (SCR 110)
        public const decimal dgcVersionOfXactFilterSettingsDataTable = 1.1M;
        public const int dgcVersionOfRewardsFilterSettingsDataTable = 1;
        public const decimal dgcVersionUserSelectableSettingsDataTable = 1.08M; // SCR 216 // SCR 222
        public const int igcNCategs = 15; // The number of categories of signups; must be the same as the number of rows in table FSCATEGS. // SCR 222

        public enum egOpsFilters
        {
            Version, EnableFilteringOverall, Person, AviatorRole, TakeoffLocation, LandingLocation, ChargeCode, LaunchMethod, SpecialOps, Equipment, 
            EquipmentRole, EquipmentType, TakeoffDate, NumOccup, ReleaseAltit, TowAltDiff, Duration, FirstFlight
        };
        public enum egXactFilters
        {
            Version, EnableFilteringOverall, TransactionType, Vendor, TransactionStatus, AttachmentCateg, AttachmentType, PaymentMethod,
            ExpenseAccount, PaymentAccount, XactDate, XactAmount, NumAttFiles
        };
        public enum egRewardsFilters
        {
            Version, EnableFilteringOverall, Member, EarnClaimDate, ShowExpired, EarnClaimCode, LimitAtTopBottom, LimitRowCount
        };
        //public enum egStdFilterProps { FilterName, FilterType, Enabled, Item, LowLimit, HighLimit, Field };
        public enum egAdvFilterProps { FilterName, FilterType, Enabled, INorEX, List, LowLimit, HighLimit, PunctuationMark, Field };
        public enum egRewardsFilterProps { FilterName, FilterType, Enabled, List, YesNo, Integ32 };
        public enum egContactProps { PiT, EnteredBy, Member, ContactType, DBegin, DEnd, PhysAddr, ContactInfo, PriorityRank };
        public enum enPFExp { LineNum, AccountID, AccountName, Descr, sAmount }; // PF stands for Pattern Field; used in Finance/Accounting
        public enum enPFPmt { LineNum, AccountID, AccountName, PmtMethodID, PmtMethodName, Descr, sAmount }; // PF stands for Pattern Field; used in Finance/Accounting
        public enum enPFAtt { LineNum, AttachCategID, AttachCategName, AttachAssocDate, AttachedFileID, AttachedFileName, AttachedThumbID, AttachedThumbName };
        public enum enPFVend { VendorName, Notes };
        public enum enPFReward { ID, bEC, DEarn, DExpiry, DClaim, iPerson, sDisplayName, iServicePts, cECCode, bForward, sComments };
        public enum enPFRate { i1Sort, ID, sShortName, DFrom, DTo, iEquipType, iLaunchMethod, sChargeCodes, mSingleDpUse, iNoChrg1stFt, mAltDiffDpFt,
            iNoChrg1stMin, mDurationDpMin, iDurCapMin, sComment, iFA_Item, iFA_PmtTerm, iQBO_ItemName };
        public enum enPFMFC { i1Sort, ID, DFrom, DTo, iMembCateg, sMinFlyChrg, sComment } // minimum flying charges
        public enum enPFFlyChrgEdit { i1Sort, ID, mAmount, DateOfAmount, cTypeOfAmount, bManuallyModified, sComments };
        public enum enPFPeopleOffices { i1Sort, ID, sDisplayName, sBoardOffice, DOfficeBegin, DOfficeEnd, sAdditionalInfo };
        public enum enPFPeopleEquipRolesTypes { ID, iAviatorRole, sAviatorRole, iPerson, sDisplayName, iEqRoleType, sEqRoleType, sComments };
        public enum enPFEqRolesTypes { ID, iRole, sRole, iType, sType, sComments };
        public enum enPFLaunchMEqRoles { ID, iLaunchMethod, sLaunchMethod, iEqRole, sEqRole, sComments };
        public enum enPFUserRolesSettings { ID, uiRole, RoleName, iSetting, sSettingName, sComments };
        public enum enPFOpsCalNames { i1Sort, ID, sOpsCalName, bStandard };
        public enum enPFOpsCalTimes { i1Sort, ID, iOpsCal, DStart, bOpStatus, sComment };
        public enum enPFEqComponents { i1Sort, ID, iEquipment, sShortEquipName, sComponent, sCName, bEntire, DLinkBegin, DLinkEnd,
            bReportOperStatus, sOperStatus, sComment, iParentComponent };// SCR 231
        public enum enPFEqStatus { iLine, ID, sShortEquipName, sRegistrId, sCName, sOperStat, sComment, sSorter, sError, DLastUpdated, sDebug, // SCR 218
            dAccumHours, iAccumCycles, dAccumDist, sActionItem, sDeadline, cPrevSchedMeth, iPercentComplete }; // SCR 218
        public enum enPFEqParameters { i1Sort, ID, sShortDescript, iEquipActionType, sEquipActionType, iIntervalElapsed, sTimeUnitsElapsed, cDeadLineMode,
            iDeadLineSpt1, iDeadLineSpt2, iIntervalOperating, sTimeUnitsOperating, iIntervalDistance, sDistanceUnits, sComment };
        public enum enPFEqAgingItems { i1Sort, ID, sName, iEquipComponent, sComponent, iParam, iOpCal, sOpsCalName, sShortDescript, DStart, DEnd,
            dEstRunDays, bRunExtrap, dEstCycleDays, bCyclExtrap, dEstDistDays, bDistExtrap, dEstDuration, sComment }
        public enum enPFEqOpData { i1Sort, ID, iEquipComponent, sComponent, DFrom, DTo, dHours, iCycles, dDistance, sDistanceUnits, cSource, sComment };
        public enum enPFEqActionItems { i1Sort, ID, iEquipAgingItem, sName, DeadLine, dDeadlineHrs, iDeadLineCycles, dDeadLineDist,
            DScheduledStart, DActualStart, iPercentComplete, DComplete, dAtCompletionHrs, iAtCompletionCycles, dAtCompletionDist, sComment, sUpdateStatus };
        public enum enPFDailyFlightLogs { ID, DFlightOps, iFlightCount, iFlightsPosted, sFldMgr, iMainTowEquip, sShortEquipName, iMainTowOp, sDisplayName, iMainGlider, sMainGliderName, iMainLaunchMethod, sMainLaunchMethod,
            iMainLocation, sLocation, mTotalCollected, sNotes};
        public enum enPFFlightLogRowsPlus
        {
            ID, iFliteLog, cStatus, iTowEquip, sTowEquipName, iTowOperator, sTowOperName, iGlider, sGliderName, iLaunchMethod, sLaunchMethod,
            iPilot1, sPilot1, iAviatorRole1, sAviatorRole1, dPctCharge1, iPilot2, sPilot2, iAviatorRole2, dPctCharge2, sAviatorRole2, dReleaseAltitude, dMaxAltitude,
            iLocTakeOff, sLocTakeOff, DTakeOff, iLocLanding, sLocLanding, DLanding, iChargeCode, cChargeCode, sChargeCode, mAmtCollected, sComments
        };
        public enum enPFFlightLogRowItems {Version, cStatus, TowEquip, TowOperator, Glider, LaunchMethod, Pilot1, AviatorRole1, PctCharge1, Pilot2, AviatorRole2, PctCharge2, ReleaseAltitude, MaxAltitude,
            LocTakeOff, DateTakeOff, LocLanding, DateLanding, ChargeCode, AmtCollected, Comments };
        public enum enPFMeEquityShJ { ID, iOwner, sDisplayName, DXaction, cDateQuality, dNumShares, cXactType, sInfoSource, sComment}; // Member Equity Shares Journal

        public enum enPFFltOpsSchedDates { i1Sort, ID, Date, bEnabled, sNote};

        // LL stands for List (i.e., 'generic' List in the c# sense); used in Finance/Accounting for enumerating the types of lists plus other uses:
        public enum enLL { Expenditure, Payment, AttachedFiles, Vendors, DiffCredDeb, Rates, Rewards, MinFlyChrgs, FlyChrgEdit, PeopleOffices,
            PeopleEquipRolesTypes, EquipRolesTypes, LaunchMethodsEqRoles, EquipComponents, EquipParameters, OpsCalendarNames, OpsCalendarTimes,
            EquipOpData, EquipAgingItems, EquipActionItems, DailyFlightLogs, FlightLogRows, MeEquityShJ, UserRolesSettings, FltOpsSchedDates,
            EquipStatus
        }; 

        public struct strgMbrContactsFilter
        {
            public bool bFilterOn;
            public string sFilterMemberName;
            public strgMbrContactsFilter(bool bpu, string spu)
            {
                bFilterOn = bpu;
                sFilterMemberName = spu;
            }
        }

        public class excToPopup : Exception
        {
            private string sp;
            public excToPopup(string su) : base(su) { sp = su; }
            public excToPopup(string su, System.Exception inner) : base(su, inner) { }
            protected excToPopup(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
            public string sExcMsg() { return sp; }
        }

        protected void Application_Start(object sender, EventArgs e)
        {
            ActivityLog.oLog(ActivityLog.enumLogTypes.ApplicationStart, 0, sgcAppName + " " + sgMigrationLevelDevIntProd);
            AppStart();
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            //igRequestCount++;
            CultureInfo ci = new CultureInfo("en-US", true);
            ci.DateTimeFormat.ShortDatePattern = "yyyy/MM/dd";
            ci.DateTimeFormat.ShortTimePattern = "HH:mm:ss";
            ci.DateTimeFormat.LongTimePattern = "HH:mm:ss";
            Thread.CurrentThread.CurrentCulture = ci;
            //ActivityLog.oLog(ActivityLog.enumLogTypes.Debug, 23, "Application_BeginRequest has been called; igRequestCount = " + igRequestCount.ToString() +
            //    ", Thread.CurrentThread.CurrentCulture short date and time formats = " + Thread.CurrentThread.CurrentCulture.DateTimeFormat.ShortDatePattern + ", " +
            //    Thread.CurrentThread.CurrentCulture.DateTimeFormat.ShortTimePattern);
        }

        public static void AppStart()
        {
            sgMigrationLevelDevIntProd = File.ReadAllText(HttpRuntime.AppDomainAppPath + @"\MigrationLevelDevIntProd.txt");
            //SCUD_Multi mCRUD = new SCUD_Multi();
            //string sOffset = mCRUD.GetSetting("TimeZoneOffset", 1);
            //int iOffsetHrs = Int32.Parse(sOffset.Substring(0, 3));
            //int iOffsetMin = Int32.Parse(sOffset.Substring(4, 2));
            //TimeSpan ts = new TimeSpan(iOffsetHrs, iOffsetMin, 0);
            //DTO_NotStarted = new DateTimeOffset(2001, 1, 1, 1, 1, 0, ts);
            //DTO_NotCompleted = new DateTimeOffset(2999, 12, 31, 22, 59, 0, ts);
            //DTO_EqAgEarliest = new DateTimeOffset(1900, 1, 1, 1, 1, 0, ts);
            //DTO_EqAgLatest = new DateTimeOffset(2999, 12, 31, 22, 59, 0, ts);
        }

        protected void Application_End(object sender, EventArgs e)
        {
            ActivityLog.oLog(ActivityLog.enumLogTypes.ApplicationEnd, 0, sgcAppName);
        }

        protected void Session_Start(object sender, EventArgs e)
        {
            ActivityLog.oLog(ActivityLog.enumLogTypes.SessionStart, 0, "ID = " + Session.SessionID.ToString());
            Session["iaLast"] = iaLast;
            Session["sLastLaunchMethod"] = "";
            Session["saLastLoc"] = new string[] { "", "" };
            Session["DaLast"] = new DateTime[] { new DateTime(2001, 1, 1), new DateTime(2001, 1, 1) };
        }
        protected void Session_End(object sender, EventArgs e)
        {
            ActivityLog.oLog(ActivityLog.enumLogTypes.SessionEnd, 0, "ID = " + Session.SessionID.ToString());
        }

        void Application_Error(object sender, EventArgs e)
        {
            // Code that runs when an unhandled error occurs
            // Get the exception object.
            Exception exc = Server.GetLastError();

            // Handle HTTP errors
            if (exc.GetType() == typeof(HttpUnhandledException))
            {
                if (exc.InnerException != null)
                {
                    exc = exc.InnerException;
                }
            }

            // Log the exception and notify system operators
            ExceptionUtility.LogException(exc);
            ExceptionUtility.NotifySystemOps(exc);

            // Clear the error from the server
            Server.ClearError();
            Server.Transfer("~/ErrorPages/Resp2Err.aspx");
        }

        public sealed class ExceptionUtility
        {
            // All methods are static, so constructor can be private
            private ExceptionUtility()
            { }

            // Log an Exception 
            public static void LogException(Exception exc)
            {
                ActivityLog.oLog(ActivityLog.enumLogTypes.ErrorSevere, 9, "Global.Application_Error reports: " + exc.ToString());
            }

            // Notify Developer about an exception
            public static void NotifySystemOps(Exception uexc)
            {
                SmtpClient client = new SmtpClient("smtp.centurylink.net", 587)
                {
                    EnableSsl = true,
                    Credentials = new System.Net.NetworkCredential("robertrackl@centurylink.net", "Puergg53~"),
                    DeliveryMethod = SmtpDeliveryMethod.Network
                };
                MailMessage mailMessage = new MailMessage("robertrackl@rad129.net", "robertrackl@rad129.net", "Error in TSoar " + sgMigrationLevelDevIntProd,
                    "Error logged at " + DateTimeOffset.UtcNow.ToString() + ": " + uexc.ToString());
                client.Timeout = 20000;
                client.Send(mailMessage);
                mailMessage.Dispose();
            }
        }
    }
}