#undef IllustrationDEDBUG
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using TSoar.DB;
using System.Web.UI.DataVisualization.Charting;

namespace TSoar.Equipment
{
    public class EqSupport
    {
        public static string sWhenceDLastAction = "Unknown";

        public enum enSchedMeth { ElapsedTime, OperatingHours, Cycles, DistanceTraveled, Illustration}; // where Illustration is not really a scheduling method

        public static string sComponentDFromTo(int iuID, string suFromTo, char cuc)
        {
            EquipmentDataContext eqdc = new EquipmentDataContext();
            string sRet = "";
            if (iuID == 0)
            {
                switch (suFromTo)
                {
                    case "From":
                        switch (cuc)
                        {
                            case 'D':
                                sRet = "1900/01/01";
                                break;
                            case 'T':
                                sRet = "01:01";
                                break;
                            case 'O':
                                sRet = (from s in eqdc.SETTINGs where s.sSettingName == "TimeZoneOffset" select s.sSettingValue).First();
                                break;
                            default:
                                throw new Global.excToPopup("Error in Equipment.EqSupport.sComponentDFromTo: cuc is not 'D', 'T', or 'O', but '" + cuc + "' (From, iuID=0).");
                        }
                        break;
                    case "To":
                        switch (cuc)
                        {
                            case 'D':
                                sRet = "2999-12-31";
                                break;
                            case 'T':
                                sRet = "22:59";
                                break;
                            case 'O':
                                sRet = (from s in eqdc.SETTINGs where s.sSettingName == "TimeZoneOffset" select s.sSettingValue).First();
                                break;
                            default:
                                throw new Global.excToPopup("Error in Equipment.EqSupport.sComponentDFromTo: cuc is not 'D', 'T', or 'O', but '" + cuc + "' (To, iuID=0).");
                        }
                        break;
                    default:
                        throw new Global.excToPopup("Error in Equipment.EqSupport.sComponentDFromTo: suFromTo is not 'From' or 'To' but " + suFromTo + "(iuID=0)");
                }
            }
            else
            {
                var q = (from c in eqdc.EQUIPCOMPONENTs
                         where c.ID == iuID
                         select new
                         {
                             c.bEntire,
                             DCompBegin = c.DLinkBegin,
                             DCompEnd = c.DLinkEnd,
                         }).First();
                switch (suFromTo)
                {
                    case "From":
                        switch (cuc)
                        {
                            case 'D':
                                sRet = CustFmt.sFmtDate(q.DCompBegin, CustFmt.enDFmt.DateOnly);
                                break;
                            case 'T':
                                sRet = CustFmt.sFmtDate(q.DCompBegin, CustFmt.enDFmt.TimeOnlyMin);
                                break;
                            case 'O':
                                sRet = CustFmt.sFmtDate(q.DCompBegin, CustFmt.enDFmt.OffsetOnly);
                                break;
                            default:
                                throw new Global.excToPopup("Error in Equipment.EqSupport.sComponentDFromTo: cuc is not 'D', 'T', or 'O', but '" + cuc + "' (From, iuID=" + iuID.ToString() + ").");
                        }
                        break;
                    case "To":
                        switch (cuc)
                        {
                            case 'D':
                                sRet = CustFmt.sFmtDate(q.DCompEnd, CustFmt.enDFmt.DateOnly);
                                break;
                            case 'T':
                                sRet = CustFmt.sFmtDate(q.DCompEnd, CustFmt.enDFmt.TimeOnlyMin);
                                break;
                            case 'O':
                                sRet = CustFmt.sFmtDate(q.DCompEnd, CustFmt.enDFmt.OffsetOnly);
                                break;
                            default:
                                throw new Global.excToPopup("Error in Equipment.EqSupport.sComponentDFromTo: cuc is not 'D', 'T', or 'O', but '" + cuc + "' (To, iuID=" + iuID.ToString() + ").");
                        }
                        break;
                    default:
                        throw new Global.excToPopup("Error in Equipment.EqSupport.sComponentDFromTo: suFromTo is not 'From' or 'To' but " + suFromTo + "iuID=" + iuID.ToString());
                }
            }
            return sRet;
        }

        public static void ActionItemUpdate(int iuActionItem)
        {
            SCUD_Multi mCRUD = new SCUD_Multi();

            EquipmentDataContext eqdc = new EquipmentDataContext();
            // One aging item can have multiple action item records associated with it:
            //      Any number of historical action items (percent complete = 100)
            //      None or one current action item (percent complete = 1 to 99)
            //      None or one upcoming action item (percent complete = 0)
            //   where "percent complete" is in terms of elapsed time so that a completion date can be estimated.
            //      percent complete = 0 means the work has not started.
            //      percent complete = 100 means the work is completed and the completion date is an actual date.

            // Each action item has between 1 to 4 limits:
            //      a date on the calendar, and/or
            //      a limit on the number of operating hours and/or,
            //      a limit on the number of cycles and/or
            //      a limit on the number of miles/Km traveled in distance
            // In the last three cases, a due date for the deadline can be estimated based on historical operational data using least squares fitting and extrapolation.
            //      However, least squares extrapolation for distance traveled is not yet implemented.

            // There are five ways of scheduling; they are independent of each other; all may be used in parallel (except the unique event), or just one at a time:
            //      (1) E = Elapsed time, by calendar - according to several deadline modes: // SCR 218
            //              mode Y: deadline is exactly at a specified date specified by month and day
            //              mode M: Action occurs every iIntervalElapsed months on the Day specified. If iDeadLineSpt1 is greater than the number of days in the month, then the action occurs on the last day of the month.
            //              mode W: 0 = Sunday, 1 = Monday, … 6 = Saturday. Action occurs every iIntervalElapsed weeks on the specified weekday.
            //              mode N: Action occurs every iIntervalElapsed months on the Nth occurrence of the specified weekday. Special case: when iIntervalElapsed is zero and iDeadLineSpt1=5, action occurs on the next 5-th occurrence of this weekday, whichever month this may be.
            //              mode L: a new deadline is calculated starting from the last completed date of an action item of the same nature
            //              mode C: like L, but the reported deadline is at the end of the calendar interval into which the calculated deadline falls
            //      (2) H = By operating hours or running time: // SCR 218
            //              the new operating hours limit is obtained by adding the iIntervalOperating (in units of sTimeUnitsOperating) to the number of running hours accumulated
            //              up to when the last action item of this nature was completed.
            //      (3) C = By number of cycles: // SCR 218
            //              the new number of cycles limit is obtained by adding iIntervalCycles number of cycles to the number of cycles accumulated
            //              up to when the last action item of this nature was completed.
            //      (4) D = By operating distance: // SCR 218
            //              the new distance traveled limit is obtained by adding the iIntervalDistance (in units of sDistanceUnits) to the number of miles/Km accumulated
            //              up to when the last action item of this nature was completed.
            //      (5) U = By unique event: happens on a fixed date; this scheduling method can only be used by itself as it excludes all the others. // SCR 218
            // When several ways of scheduling are in use for an aging item, the earliest deadline prevails.
            // SCR 218 removed comment lines here
            // When scheduling by operating/running hours, by number of cycles, or by distance traveled, there are two ways of arriving at a deadline datetime:
            //      (1) The user supplies an estimate of the elapsed time it will take between required actions; those estimates are stored in Table EQUIPAGINGITEMS
            //          in fields dEstRunDays, dEstCycleDays, and dEstDistDays. For example, the user estimates that it will take 10 years ~= 3650 days for the
            //          tow plane engine to accumulate 2000 hours which is the maximum operating time between engine replacements (the required action).
            //      (2) Historical operational data are used for predicting the next time that an action is required. Past data are fitted by a least squares method
            //          with a straight line. That line is extrapolated into the future and intersected with the known limit in operations, either running hours,
            //          cycles, or distance traveled. Table EQUIPAGINGITEMS contains boolean fields (bRunExtrap, bCyclExtrap, bDistExtrap)
            //          for instructing TSoar to attempt or not to attempt this extrapolation. This method is not  yet available for distance traveled. // SCR 218

            // Field dEstDuration in table EQUIPAGINGITEMS is an estimate of the elapsed time required from start of the action to its completion. This value is used
            //      to arrive at the start date of the action by backing up dEstDuration days from the calculated deadline.

            string sUpdateStatus = "OK"; // optimistic
            string[] saChart = new string[] { "", "", "", "" }; // Temporarily stores charts' serialized representation; indexed by first 4 values of enSchedMeth
            char[] caKindOfChart = new char[] { 'E', 'H', 'C', 'D', 'I' }; // indexed by enSchedMeth (all 5 values)
            string sOffset = mCRUD.GetSetting("TimeZoneOffset");
            int iOffsetHrs = Int32.Parse(sOffset.Substring(0, 3));
            int iOffsetMin = Int32.Parse(sOffset.Substring(4, 2));
            TimeSpan tsOffset = new TimeSpan(iOffsetHrs, iOffsetMin, 0);
            int iUser = mCRUD.GetPeopleIDfromWebSiteUserName(HttpContext.Current.User.Identity.Name);
            char cPrevailSchedMeth = 'N'; // Can be [CDEHNU]; N is default and means 'none'. See paragraphs above. // SCR 218

            int iAgingItemSingle = 0; // Loop through all aging items if this stays 0
            if (iuActionItem > 0)
            {
                // User asked for update of the action items associated with just one aging item
                iAgingItemSingle = (from a in eqdc.EQUIPACTIONITEMs where a.ID == iuActionItem select a.iEquipAgingItem).First();
            }

            // Loop over all Aging Items (there is no interaction between aging items, i.e., they are independent of each other):
            var qi = from agingItem in eqdc.TNPV_EquipAgingItems orderby agingItem.sName select agingItem;
            int iAgingItemCount = 0;
            foreach (var agingItem in qi)
            {
                iAgingItemCount++;
                if (iAgingItemSingle == 0 || iAgingItemSingle == agingItem.ID) // Doing all aging items or just one of them?
                { 
                    decimal mHrsPerUnit = 1.0m;
                    switch (agingItem.sTimeUnitsOperating)
                    {
                        case "Days": mHrsPerUnit = 24.0m; break;
                        case "Weeks": mHrsPerUnit = 24.0m * 7.0m; break;
                        case "Months": mHrsPerUnit = 24.0m * 30.0m; break; // An Approximation!
                        case "Quarters": mHrsPerUnit = 24.0m * 30.0m * 3.0m; break; // An Approximation!
                        case "Years": mHrsPerUnit = 24.0m * 365.25m; break; // An approximation!
                    }

                    decimal? mPrevHrs = 0.0m; // The number of operating hours accumulated up to the last action of this nature
                    int iPrevCycles = 0; // the number of cycles accumulated up to the last action of this nature
                    decimal mPrevDist = 0.0m; // The distance traveled accumulated up to the last action of this nature
                    DateTimeOffset DnewDeadLine = DateTimeOffset.MaxValue.AddYears(-1);
                    DateTimeOffset DLocDeadLine = DateTimeOffset.MinValue;
                    DateTimeOffset DScheduledStart = DateTimeOffset.MaxValue.AddYears(-1);

                    // For this Aging Item, do any action items of the same nature already exist? (where 'of the same nature' really just means 'associated with this aging item')
                    int i_aExists = 0; // counts number of action items of the same nature that have not been started
                    // iActionItem = usually ID of row in table EQUIPACTIONITEMS
                    //     = -1: No update action is required for this aging item
                    //     = 0: we expect to create a new action item for aging item pointed to by variable agingItem
                    //     > 0: pointer to record in table EQUIPACTIONITEMS
                    int iActionItem = 0;
                    DateTimeOffset DLastAction = DateTimeOffset.MinValue;
                    sWhenceDLastAction = "DateTimeOffset.MinValue";

                    // bIsUnique is a computed property of the aging item:
                    bool bIsUnique = agingItem.iIntervalDistance == -1 && agingItem.iIntervalOperating == -1 && agingItem.iIntervalCycles == -1 && agingItem.iIntervalElapsed == -1;

                    // Examine all already existing action items that go with this aging item, and start finding the date of last action:
                    //             ================  
                    var qa = (from actionItem in eqdc.TNPV_EquipActionItems
                              where actionItem.iEquipAgingItem == agingItem.ID
                              orderby actionItem.iPercentComplete descending, actionItem.DComplete ascending
                              select actionItem).ToList();
                    foreach (var actionItem in qa)
                    {
                        // Does an action item of the same nature exist that has not been started? If one exists, it is subject to update
                        if (actionItem.iPercentComplete < 1)
                        {
                            i_aExists++;
                            if (i_aExists > 1)
                            {
                                throw new Global.excToPopup("More than 1 action items exist that have not been started for Aging Item index " +
                                    actionItem.iEquipAgingItem + " (`" + actionItem.sName + "`, component `" + actionItem.sComponent + "`, equipment `" + actionItem.sShortEquipName);
                            }
                            if (actionItem.DActualStart != Global.DTO_NotStarted && actionItem.DActualStart < DateTimeOffset.Now)
                            {
                                throw new Global.excToPopup("Data inconsistency in Aging Item `" + actionItem.sName + "`, component `" + actionItem.sComponent + 
                                    "`, equipment `" + actionItem.sShortEquipName + ": When the actual start time is in the past the value for Percent Complete should be at least 1, not 0. " +
                                    "Please adjust Actual Start time or Percent Complete.");
                            }
                            iActionItem = actionItem.ID; // needed below for updating the action item
                        }
                        else
                        {
                            // Find the latest completed or partially completed action item of this nature:
                            if (actionItem.DComplete > DLastAction)
                            {
                                DLastAction = actionItem.DComplete;
                                sWhenceDLastAction = "Latest completed or partially completed action item for this aging item";
                            }
                            if (bIsUnique) // For an aging item that consists of one single unique event, we should never have more than 1 action item in existence.
                            {
                                i_aExists++;
                                if (i_aExists > 1)
                                {
                                    throw new Global.excToPopup("Aging Item `" + actionItem.sName + "` (index " +  actionItem.iEquipAgingItem + "), component `"
                                        + actionItem.sComponent + "`, equipment `" + actionItem.sShortEquipName + "`, is `unique` having only a single fixed event associated with it, " +
                                        "i.e., there may exist only one action item for it, but more than one exist. Contact website administrator or developer.");
                                }
                                if (actionItem.iPercentComplete == 100)
                                {
                                    iActionItem = -1; // Don't touch this record since it's completed
                                }
                                else
                                {
                                    iActionItem = actionItem.ID;
                                }
                            }
                        }
                    } // foreach (var actionItem in qa)
                    // DLastAction now contains the date when the last action item of this nature was either completed or estimated to be completed.
                    //      If none had ever been completed then DLastAction still contains DateTimeOffset.MinValue.

                    sUpdateStatus = "";

                    // Continue and complete establishing the date of last action:
                    //                                        ===================

                    // Apply Start/End times of Aging Item constraints to DLastAction
                    if (DLastAction < agingItem.DStart)
                    {
                        DLastAction = agingItem.DStart;
                        sWhenceDLastAction = "Start datetime of aging item";
                    }
                    if (DLastAction > agingItem.DEnd)
                    {
                        throw new Global.excToPopup("Equipment.EqSupport.ActionItemUpdate Error: Date of Last Action "
                            + CustFmt.sFmtDate(DLastAction, CustFmt.enDFmt.DateAndTimeMinOffset)
                            + " is past end of aging item time interval " + CustFmt.sFmtDate(agingItem.DEnd, CustFmt.enDFmt.DateAndTimeMinOffset) + ". iEquipComponent = "
                            + agingItem.iEquipComponent.ToString() + ". Aging item ID = " + agingItem.ID.ToString());
                    }

                    // Apply DlinkBegin/DlinkEnd (from equipment component) constraint to DLastAction:
                    var qbe = (from c in eqdc.TNPF_LinkBeginEnd(agingItem.iEquipComponent) select c).First();
                    if (qbe.DBegin <= DateTimeOffset.MinValue.AddDays(1) || qbe.DEnd >= DateTimeOffset.MaxValue.AddYears(-1))
                    {
                        throw new Global.excToPopup("Equipment.EqSupport.ActionItemUpdate Error: Infinite Loop detected in TNPF_LinkBeginEnd.SQL. iEquipComponent = "
                            + agingItem.iEquipComponent.ToString() + ".");
                    }
                    if (DLastAction < qbe.DBegin)
                    {
                        // We come here when there is no previous action item that has been completed
                        DLastAction = (DateTimeOffset)qbe.DBegin;
                        sWhenceDLastAction = "DLinkBegin of Component";
                        //sUpdateStatus += "; DLastAction adjusted for DLinkBegin to " + CustFmt.sFmtDate(DLastAction,CustFmt.enDFmt.DateAndTimeMinOffset);
                    }
                    if (DLastAction > qbe.DEnd) // Doubtful that this would ever happen
                    {
                        DLastAction = (DateTimeOffset)qbe.DEnd;
                        sWhenceDLastAction = "DLinkEnd of Component";
                        //sUpdateStatus += "; DLastAction adjusted for DLinkEnd to " + CustFmt.sFmtDate(DLastAction, CustFmt.enDFmt.DateAndTimeMinOffset);
                    }

                    // Find the operating data up to the date of last action:
                    //          ==============
                    string sDbg = "";
                    int? iDbgCtrl = 0;
                    if (agingItem.iIntervalOperating > -1)
                    {
                        // What are the operating hours up to DLastAction?
                        eqdc.spOperHours(agingItem.iEquipComponent, DLastAction, iDbgCtrl, ref mPrevHrs, ref sDbg);
                    }

                    if (agingItem.iIntervalCycles > -1)
                    {
                        // What is the number of cycles up to DLastAction?
                        iPrevCycles = (int)eqdc.TNPF_OperCycles(agingItem.iEquipComponent, DLastAction);
                    }

                    if (agingItem.iIntervalDistance > -1)
                    {
                        // What is the distance traveled up to DLastAction?
                        mPrevDist = (decimal)eqdc.TNPF_OperDist(agingItem.iEquipComponent, DLastAction);
                    }
                    if (iActionItem > -1) // Completed 'Unique' aging items need no further treatment
                    {

                        // Determine the deadline for the next action on this aging item:
                        //               ========

                        EQUIPACTIONITEM eqa = new EQUIPACTIONITEM
                        {
                            iEquipAgingItem = agingItem.ID,
                            iUsedElapsed = 0, // SCR 218
                            iUsedHours = 0, // SCR 218
                            iUsedCycles = 0, // SCR 218
                            iUsedDist = 0, // SCR 218
                            iUsedUnique = 0, // SCR 218
                            dAtCompletionHrs = 0.0m, // SCR 218
                            dAtCompletionDist = 0.0m, // SCR 218
                            iAtCompletionCycles = 0, // SCR 218
                            cPrevailSchedMeth = 'N', // SCR 218
                            DTOdeadlineElapsed = Global.DTO_NotCompleted,
                            DTOdeadlineHrs = Global.DTO_NotCompleted,
                            DTOdeadlineCycles = Global.DTO_NotCompleted,
                            DTOdeadlineDist = Global.DTO_NotCompleted,
                            DTOdeadlineUnique = Global.DTO_NotCompleted
                        };
                        if (i_aExists > 0)
                        {
                            // We are updating an existing action item
                            eqa = (from va in eqdc.EQUIPACTIONITEMs where va.ID == iActionItem select va).First();
                        }
                        eqa.PiTRecordEntered = DateTime.UtcNow;
                        eqa.iUsedElapsed = 0; // SCR 218
                        eqa.iUsedHours = 0; // SCR 218
                        eqa.iUsedCycles = 0; // SCR 218
                        eqa.iUsedDist = 0; // SCR 218
                        eqa.iUsedUnique = 0; // SCR 218
                        eqa.cPrevailSchedMeth = 'N'; // SCR 218
                        eqa.iRecordEnteredBy = iUser;
                        eqa.DLastAction = DLastAction;
                        eqa.DTOdeadlineElapsed = Global.DTO_NotCompleted;
                        eqa.DTOdeadlineHrs = Global.DTO_NotCompleted;
                        eqa.DTOdeadlineCycles = Global.DTO_NotCompleted;
                        eqa.DTOdeadlineDist = Global.DTO_NotCompleted;
                        eqa.DTOdeadlineUnique = Global.DTO_NotCompleted;
                        string sAnnot = "";

                        // Scheduling by elapsed time
                        // ==========================
                        if (agingItem.iIntervalElapsed > 0)
                        {
                            DLocDeadLine = DTO_ElapsedNext(agingItem, DLastAction, tsOffset);
                            eqa.iUsedElapsed = 1; // SCR218
                            eqa.DTOdeadlineElapsed = DLocDeadLine;
                            if (DLocDeadLine < agingItem.DStart || DLocDeadLine > agingItem.DEnd)
                            {
                                sUpdateStatus += "; Elapsed: Deadline " + CustFmt.sFmtDate(DLocDeadLine, CustFmt.enDFmt.DateAndTimeMin) + " is outside of Aging Item Start/End";
                            }
                            else
                            {
                                sUpdateStatus += "; Elapsed: OK" + sAnnot;
                                cPrevailSchedMeth = 'E'; // SCR 218
                                DnewDeadLine = DLocDeadLine;
                            }
                        }
                        else
                        {
                            DnewDeadLine = DateTimeOffset.MaxValue.AddYears(-1);
                        }

                        // Scheduling by Operating hours
                        // =============================
                        sAnnot = "";
                        if (agingItem.iIntervalOperating > 0)
                        {
                            //  Add up all that we know up until today, then add the scheduling interval(s):
                            eqa.dDeadlineHrs = (decimal)mPrevHrs + agingItem.iIntervalOperating * mHrsPerUnit;
                            eqa.iUsedHours = 1; // SCR 218
                            DLocDeadLine = DLastAction.AddDays((double)agingItem.dEstRunDays);
                            if (agingItem.bRunExtrap)
                            {
                                DLocDeadLine = DLSExtrapHrs(agingItem.ID, DLastAction, (decimal)mPrevHrs, (double)eqa.dDeadlineHrs, ref saChart[(int)enSchedMeth.OperatingHours]);
                                sAnnot = " (extrapolated)";
                            }
                            eqa.DTOdeadlineHrs = DLocDeadLine;
                            if (DLocDeadLine < agingItem.DStart || DLocDeadLine > agingItem.DEnd)
                            {
                                sUpdateStatus += "; Operating Hours: Deadline " + CustFmt.sFmtDate(DLocDeadLine, CustFmt.enDFmt.DateAndTimeMin) + " is outside of Aging Item Start _/_ End " +
                                    CustFmt.sFmtDate(agingItem.DStart, CustFmt.enDFmt.DateAndTimeMin) + " _/_ " + CustFmt.sFmtDate(agingItem.DEnd, CustFmt.enDFmt.DateAndTimeMin);
                            }
                            else
                            {
                                // SCR 218 Start
                                if (DnewDeadLine > DLocDeadLine)
                                {
                                    DnewDeadLine = DLocDeadLine;
                                    cPrevailSchedMeth = 'H';
                                }
                                // SCR 218 End
                                sUpdateStatus += "; Operating Hours: OK" + sAnnot;
                            }
                        }
                        else
                        {
                            eqa.dDeadlineHrs = 0.0M;
                        }

                        // Scheduling by number of cycles
                        // ==============================
                        sAnnot = "";
                        if (agingItem.iIntervalCycles > 0)
                        {
                            eqa.iUsedCycles = 1; // SCR 218
                            eqa.iDeadLineCycles = iPrevCycles + agingItem.iIntervalCycles;
                            DLocDeadLine = DLastAction.AddDays((double)agingItem.dEstCycleDays);
                            if (agingItem.bCyclExtrap)
                            {
                                DLocDeadLine = DLSExtrapCycles(agingItem.ID, DLastAction, iPrevCycles, eqa.iDeadLineCycles, ref saChart[(int)enSchedMeth.Cycles]);
                                sAnnot = " (extrapolated)";
                            }
                            eqa.DTOdeadlineCycles = DLocDeadLine;
                            if (DLocDeadLine < agingItem.DStart || DLocDeadLine > agingItem.DEnd)
                            {
                                sUpdateStatus += "; Number of Cycles: Deadline " + CustFmt.sFmtDate(DLocDeadLine, CustFmt.enDFmt.DateAndTimeMin) + " is outside of Aging Item Start/End";
                            }
                            else
                            {
                                // SCR 218 Start
                                if (DnewDeadLine > DLocDeadLine)
                                {
                                    DnewDeadLine = DLocDeadLine;
                                    cPrevailSchedMeth = 'C';
                                }
                                // SCR 218 End
                                sUpdateStatus += "; Number of Cycles: OK" + sAnnot;
                            }
                        }
                        else
                        {
                            eqa.iDeadLineCycles = 0; // SCR 218
                        }

                        // Scheduling by distance traveled
                        // ===============================
                        sAnnot = "";
                        if (agingItem.iIntervalDistance > 0)
                        {
                            eqa.iUsedDist = 1;
                            eqa.dDeadLineDist = mPrevDist + agingItem.iIntervalDistance;
                            DLocDeadLine = DLastAction.AddDays((double)agingItem.dEstDistDays);
                            if (agingItem.bDistExtrap)
                            {
                                throw new Global.excToPopup("Apologies from the website Developer: deadline extrapolation for distance traveled is not yet available.");
                            }
                            eqa.DTOdeadlineDist = DLocDeadLine;
                            if (DLocDeadLine < agingItem.DStart || DLocDeadLine > agingItem.DEnd)
                            {
                                sUpdateStatus += ": Distance Traveled: Deadline " + CustFmt.sFmtDate(DLocDeadLine, CustFmt.enDFmt.DateAndTimeMin) + " is outside of Aging Item Start/End";
                            }
                            else
                            {
                                // SCR 218 Start
                                if (DnewDeadLine > DLocDeadLine)
                                {
                                    DnewDeadLine = DLocDeadLine;
                                    cPrevailSchedMeth = 'D';
                                }
                                // SCR 218 End
                                sUpdateStatus += "; Distance Traveled: OK" + sAnnot;
                            }
                        }
                        else
                        {
                            eqa.dDeadLineDist = 0.0m; // SCR 218
                        }

                        // Are we dealing with a single unique event?
                        if (bIsUnique)
                        {
                            eqa.iUsedUnique = 1;
                            sAnnot = "";
                            // SCR 218 Start
                            DLocDeadLine = agingItem.DStart; 
                            if (DnewDeadLine > DLocDeadLine)
                            {
                                DnewDeadLine = DLocDeadLine;
                                cPrevailSchedMeth = 'U';
                            }
                            // SCR 218 End
                            sUpdateStatus += "; Unique: OK" + sAnnot;
                            eqa.DTOdeadlineUnique = DnewDeadLine;
                        }

                        if (sUpdateStatus.Substring(0, 2) == "; ") sUpdateStatus = sUpdateStatus.Substring(2); // trim off leading semicolon

                        // Save results
                        // ============

                        eqa.DeadLine = DnewDeadLine;
                        eqa.sUpdateStatus = sUpdateStatus;
                        eqa.cPrevailSchedMeth = cPrevailSchedMeth; // SCR 218
                        eqa.DScheduledStart = DnewDeadLine.AddDays(-(double)agingItem.dEstDuration);
                        if (i_aExists < 1)
                        {
                            eqa.DActualStart = Global.DTO_NotStarted;
                            eqa.iPercentComplete = 0;
                            eqa.DComplete = Global.DTO_NotCompleted;
                            eqa.sComment = "";
                            eqdc.EQUIPACTIONITEMs.InsertOnSubmit(eqa);
                        }
                        eqdc.SubmitChanges();
                        iActionItem = eqa.ID;

                        // Create illustration of how Action Item deadline was obtained
                        ActIllustr actill = new ActIllustr();
                        actill.ipAgingItem = agingItem.ID;
                        string sIllustr = actill.sIllustration(DLastAction);

                        foreach (enSchedMeth s in Enum.GetValues(typeof(enSchedMeth)))
                        {
                            string sTitle = "Generated on " + CustFmt.sFmtDate(DateTimeOffset.Now, CustFmt.enDFmt.DateAndTimeMinOffset);
                            int iSchedMeth = (int)s;
                            switch (s)
                            {
                                case enSchedMeth.Illustration:
                                    if (sIllustr.Length > 0)
                                    {
                                        var qcc = (from cc in eqdc.EQACITAUXes
                                                    where cc.iEqActionItem == iActionItem &&
                                                            cc.cKind == caKindOfChart[iSchedMeth]
                                                    select cc).ToList();
                                        if (qcc.Count < 1)
                                        {
                                            EQACITAUX eaia = new EQACITAUX();
                                            eaia.sIllustrFile = sIllustr;
                                            eaia.cKind = caKindOfChart[iSchedMeth];
                                            eaia.sTitle = sTitle;
                                            eaia.iEqActionItem = iActionItem;
                                            eqdc.EQACITAUXes.InsertOnSubmit(eaia);
                                        }
                                        else
                                        {
                                            EQACITAUX eaia = qcc[0];
                                            string sFile = eaia.sIllustrFile;
                                            if (!(sFile is null || sFile.Length < 1))
                                            {
                                                File.Delete(System.Web.Hosting.HostingEnvironment.MapPath(sFile));
                                            }
                                            eaia.sIllustrFile = sIllustr;
                                            eaia.sTitle = sTitle;
                                        }
                                        eqdc.SubmitChanges();
                                    }
                                    break;
                                default:
                                    if (saChart[iSchedMeth].Length > 0)
                                    {
                                        var qcc = (from cc in eqdc.EQACITAUXes
                                                    where cc.iEqActionItem == iActionItem &&
                                                            cc.cKind == caKindOfChart[iSchedMeth]
                                                    select cc).ToList();
                                        if (qcc.Count < 1)
                                        {
                                            EQACITAUX eaia = new EQACITAUX();
                                            eaia.serChart = saChart[iSchedMeth];
                                            eaia.cKind = caKindOfChart[iSchedMeth];
                                            eaia.sTitle = sTitle;
                                            eaia.iEqActionItem = iActionItem;
                                            eqdc.EQACITAUXes.InsertOnSubmit(eaia);
                                        }
                                        else
                                        {
                                            EQACITAUX eaia = qcc[0];
                                            eaia.serChart = saChart[iSchedMeth];
                                            eaia.cKind = caKindOfChart[iSchedMeth];
                                            eaia.sTitle = sTitle;
                                        }
                                        eqdc.SubmitChanges();
                                    }
                                    break;
                            }
                        }
                    }
                }
                saChart = new string[] { "", "", "", "" };
            }
            eqdc.Dispose();
            if (iAgingItemCount < 1)
            {
                throw new Global.excToPopup("There are no aging items defined; cannot calculate any action items.");
            }
        }

        private static DateTimeOffset DTO_ElapsedNext(TNPV_EquipAgingItem uagingItem, DateTimeOffset DuLast, TimeSpan utsOffset)
        {
            if (uagingItem.iIntervalElapsed < 0)
            {
                // Not scheduling by elapsed calendar time, therefore:
                return DateTimeOffset.MinValue;
            }
            DateTimeOffset DnewDL = DateTimeOffset.MinValue; // the new deadline
            if (DuLast == Global.DTO_NotStarted)
            {
                throw new Global.excToPopup("EqSupport.cs.DTO_ElapsedNext: Aging Item `" + uagingItem.sName + "`: date of last action (DuLast = " + CustFmt.sFmtDate(DuLast, CustFmt.enDFmt.DateAndTimeMinOffset) + ") indicates `Not Started`; that's not allowed.");
            }
            switch (uagingItem.cDeadLineMode)
            {
                case 'Y': // Deadline is exacly at a specified calendar date (for example April 15, 2020, end of day)
                    int iMonth = uagingItem.iDeadLineSpt1;
                    int iDay = uagingItem.iDeadLineSpt2;
                    int iYearCount = uagingItem.iIntervalElapsed;
                    // The deadline is the first occurrence of the date in month iMonth with day iDay that happens after the date in DuLast,
                    //    plus the number of years specified in iIntervalElapsed.
                    DnewDL = new DateTimeOffset(DuLast.Year, iMonth, iDay, 23, 59, 0, utsOffset);
                    if (DnewDL < DuLast)
                    {
                        DnewDL = DnewDL.AddYears(1);
                        if (iYearCount > 0) iYearCount--;
                    }
                    DnewDL = DnewDL.AddYears(iYearCount);
                    break;
                case 'M': // Deadline is every iIntervalElapsed months on the day specified by iDeadLineSpt2
                    DnewDL = new DateTimeOffset(DuLast.Year, DuLast.Month + uagingItem.iIntervalElapsed, uagingItem.iDeadLineSpt2, 23, 59, 0, utsOffset);
                    break;
                case 'W': // Deadline is iIntervalElapsed weeks later on the weekday specified by iDeadLineSpt2
                          // Add a day until we find the desired weekday
                    DnewDL = DuLast;
                    int iWeeksCount = uagingItem.iIntervalElapsed;
                    do
                    {
                        DnewDL = DnewDL.AddDays(1);
                    } while ((int)DnewDL.DayOfWeek != uagingItem.iDeadLineSpt2);
                    // 
                    if (iWeeksCount > 0) iWeeksCount--;
                    DnewDL = DnewDL.AddDays(7 * iWeeksCount);
                    break;
                case 'N':
                    DnewDL = DuLast;
                    int iMonthsCount = uagingItem.iIntervalElapsed;
                    int iMonthsDiff = 0;
                    do
                    {
                        try
                        {
                            DnewDL = Time_Date.DNthWeekdayOfMonth(DuLast.Year, DuLast.Month + iMonthsDiff,
                                uagingItem.iDeadLineSpt1, (DayOfWeek)uagingItem.iDeadLineSpt2);
                        }
                        catch (Global.excToPopup excTP)
                        {
                            if (uagingItem.iIntervalElapsed == 0 && uagingItem.iDeadLineSpt1 == 5)
                            {
                                iMonthsCount = 2; // try again next month
                            }
                            else
                            {
                                throw new Global.excToPopup(excTP.Message);
                            }
                        }
                        catch (Exception exc)
                        {
                            throw new Global.excToPopup(exc.Message);
                        }
                        iMonthsDiff++;
                        if (DnewDL > DuLast) iMonthsCount--;
                    } while (DnewDL <= DuLast || iMonthsCount > 0);
                    break;
                case 'L':
                case 'C':
                    switch (uagingItem.sTimeUnitsElapsed)
                    {
                        case "Hours":
                            DnewDL = DuLast.AddHours(uagingItem.iIntervalElapsed);
                            break;
                        case "Days":
                            DnewDL = DuLast.AddDays(uagingItem.iIntervalElapsed);
                            break;
                        case "Weeks":
                            DnewDL = DuLast.AddDays(7 * uagingItem.iIntervalElapsed);
                            break;
                        case "Months":
                            DnewDL = DuLast.AddMonths(uagingItem.iIntervalElapsed);
                            if (uagingItem.cDeadLineMode == 'C')
                            {
                                // Change to last day of same month
                                DnewDL = DnewDL.AddDays(-DnewDL.Day + 1).AddMonths(1).AddDays(-1);
                            }
                            break;
                        case "Quarters":
                            DnewDL = DuLast.AddMonths(4 * uagingItem.iIntervalElapsed);
                            if (uagingItem.cDeadLineMode == 'C')
                            {
                                // Change to last day of same quarter
                                // Wich quarter is DnewDL in?
                                int iquarter = (DuLast.Month - 1) / 3 + 1;
                                DateTimeOffset DFirst4 = new DateTimeOffset(DuLast.Year, (iquarter - 1) * 3 + 1, 1, 23, 59, 0, utsOffset); // 1st day of quarter
                                DnewDL = DFirst4.AddMonths(3).AddDays(-1);
                            }
                            break;
                        case "Years":
                            DnewDL = DuLast.AddYears(uagingItem.iIntervalElapsed);
                            if (uagingItem.cDeadLineMode == 'C')
                            {
                                // Change to last day of same year
                                DnewDL = new DateTimeOffset(DuLast.Year, 12, 31, 23, 59, 0, utsOffset);
                            }
                            break;
                    }
                    break;
            }
            return DnewDL;
        }

        public class InopOpIntv
        {
            // Operations Interval
            private DateTimeOffset DpStart;
            private bool bpOpStat; // False = an Inoperative Interval; True = an Interval when operations can take place
            // Instances of InopOpIntv occur one after the other in a list. The end of this interval is given by the start of the next interval in the list.
            // The last interval gives the end of the entire set of Operations Intervals: times beyond its DStart are not considered inside this list.
            public DateTimeOffset DStart { get { return DpStart; } set { DStart = value; } }
            public bool bOpStat { get { return bpOpStat; } set { bOpStat = value; } }

            public InopOpIntv(DateTimeOffset DuStart, bool buOpStat) // constructor
            {
                DpStart = DuStart;
                bpOpStat = buOpStat;
            }
        }

        public class DailyOp
        {
            private DateTimeOffset DpDay; // Calendar date of a day of operations
            private double dpHrs; // Hours of operation on this day
            public DateTimeOffset DDay { get { return DpDay; } set { DpDay = value; } }
            public double dHrs { get { return dpHrs; } set { dpHrs = value; } }

            public DailyOp(DateTimeOffset DuDay, double duHrs) // constructor
            {
                DpDay = DuDay;
                dpHrs = duHrs;
            }
        }

        public class DailyCycles
        {
            private DateTimeOffset DpDay; // Calendar date of a day of operations
            private int ipCycles; // Cycles on this day
            public DateTimeOffset DDay { get { return DpDay; } set { DpDay = value; } }
            public int iCycles { get { return ipCycles; } set { ipCycles = value; } }

            public DailyCycles(DateTimeOffset DuDay, int iuCycles) // constructor
            {
                DpDay = DuDay;
                ipCycles = iuCycles;
            }
        }

        private enum enSeries { HoursFlownPerDay, CumulHoursFlown, TimeShiftedCumulHoursFlown, TimeDiffWeights, CumulHoursLimit, BrokenLine, Deadline }
        // The above directly applies to hours of operation. The same enumeration is also used for number of cycles and distance traveled;
        //  Use these translations: HoursFlownPerDay           = CyclesPerDay             = DistanceTraveledPerDay;
        //                          CumulHoursFlown            = AccumulatedCycles        = CumulDistanceTraveled;
        //                          TimeShiftedCumulHoursFlown = TimeShiftedAccumulCycles = TimeShiftedDistancesTraveled
        //                          TimeDiffWeights            = TimeDiffWeights          = TimeDiffWeights
        //                          CumulHoursLimit            = CumulCyclesLimit         = CumulDistanceLimit
        //                          BrokenLine                 = BrokenLine               = BrokenLine
        //                          Deadline                   = Deadline                 = Deadline

        private static DateTimeOffset DLSExtrapHrs(int iuAgingItem, DateTimeOffset DuLastAction, decimal muPrevHrs, double udTAlimit, ref string suChart)
        {
            // Performs Least Squares Extrapolation for number of hours of running/operating time

            // DuLastAction = Date and Time of last (=latest) action taken with respect to this aging item
            // muPrevHrs = accumulated flight time hours up to DuLastAction
            // udTAlimit = the target limit of operating hours for which an extrapolation in time is desired
            List<InopOpIntv> InopOpLi = new List<InopOpIntv>(); // List of inoperative/operative intervals
            List<DailyOp> DailyOpsLi = new List<DailyOp>(); // Daily operations list, flight hours per day
            DateTimeOffset t0; // Reference start time
            DateTimeOffset tDeadLine = DateTimeOffset.MaxValue.AddYears(-1);
            EquipmentDataContext eqdc = new EquipmentDataContext();
            // Pointer to the operational calendar
            int iOpCal = (from a in eqdc.EQUIPAGINGITEMs where a.ID == iuAgingItem select a.iOpCal).First();
            var qo = from o in eqdc.OPSCALTIMEs where o.iOpsCal == iOpCal orderby o.DStart select o;
            // SCR 214 start
            // The interval for which extrapolation data is acquired:
            DateTime DInopOpStart = DateTime.MinValue;
            DateTime DInopOpStop = DateTime.MinValue;
            // SCR 214 end
            foreach (var o in qo)
            {
                // SCR 214 start
                if (DInopOpStart == DateTime.MinValue && o.bOpStatus)
                {
                    DInopOpStart = o.DStart.DateTime;
                    DInopOpStop = DateTime.MaxValue;
                }
                else if (!o.bOpStatus)
                {
                    DInopOpStop = o.DStart.Date;
                }
                // SCR 214 end
                InopOpLi.Add(new InopOpIntv(o.DStart.DateTime, o.bOpStatus));
            }

            // Use actual takeoff and landing times for the piece of equipment pointed to by the aging item:
            int iEquip = (from ec in eqdc.EQUIPAGINGITEMs where ec.ID == iuAgingItem select ec.EQUIPCOMPONENT.iEquipment).First();
            var qw = from ww in eqdc.OPDETAILs
                     where ww.EQUIPMENT.ID == iEquip && ww.OPERATION.DBegin >= DInopOpStart && ww.OPERATION.DBegin <= DInopOpStop // SCR 214
                     orderby ww.OPERATION.DBegin
                     select new { ww.OPERATION.DBegin, ww.OPERATION.DEnd };
            List<DailyOp> OpsLiRaw = new List<DailyOp>(); // Hours flown in each flight operation of piece of equipment pointed to by iEquip
            foreach (var ww in qw)
            {
                OpsLiRaw.Add(new DailyOp((DateTime)ww.DBegin, ((DateTime)ww.DEnd).Subtract((DateTime)ww.DBegin).TotalHours));
            }
            // Aggregate the raw by-flight data into daily operations in terms of hours flown per day
            DateTime tDay = DateTime.MinValue;
            int iDOLi = -1; // index into daily operations list
            for (int i = 0; i < OpsLiRaw.Count; i++)
            {
                if (tDay != OpsLiRaw[i].DDay.Date)
                {
                    iDOLi++;
                    tDay = OpsLiRaw[i].DDay.Date;
                    DailyOpsLi.Add(new DailyOp(tDay, 0));
                }
                DailyOpsLi[iDOLi].dHrs += OpsLiRaw[i].dHrs;
            }
            if (DailyOpsLi.Count < 2)
            {
                throw new Global.excToPopup("EqSupport.DLSExtrapHrs Error: The number of days of flying activity is " +
                    DailyOpsLi.Count.ToString() + " which is too small to allow a least squares fit. Internal Aging Item ID = " + iuAgingItem.ToString());
            }

            // For purposes of fitting a line through the data, remove the time gaps when no operations can take place
            int iInopOpRefListIndex = -999;
            List<DailyOp> DOLiSd = ShiftTS(InopOpLi, DailyOpsLi, iuAgingItem, ref iInopOpRefListIndex); // Shifted time sequence
            t0 = InopOpLi[iInopOpRefListIndex].DStart; // DOLiSd[0].DDay;

            List<Series> serli = new List<Series>();
            foreach (enSeries s in Enum.GetValues(typeof(enSeries)))
            {
                serli.Add(new Series());
            }

            serli[(int)enSeries.HoursFlownPerDay].ChartType = SeriesChartType.Candlestick; // this is 's'
            serli[(int)enSeries.CumulHoursFlown].ChartType = SeriesChartType.StepLine; // the integral of s
            serli[(int)enSeries.TimeShiftedCumulHoursFlown].ChartType = SeriesChartType.StepLine; // the integral of time shifted s
            serli[(int)enSeries.TimeDiffWeights].ChartType = SeriesChartType.StepLine; // weights in terms of time differences
            serli[(int)enSeries.CumulHoursLimit].ChartType = SeriesChartType.Line; // the flight time limit
            serli[(int)enSeries.BrokenLine].ChartType = SeriesChartType.Line; // Extrapolated line
            serli[(int)enSeries.Deadline].ChartType = SeriesChartType.Line; // the deadline corresponding to the flight time limit

            // Least squares fit: see K:\DataAndDocs\07_Science&Technology\02_ComputerScience\Software\Projects\TSoar\TheTSoarProject.docx,
            //     Chapter on Design of Equipment Maintenance Support, subsection on linear extrapolation
            double v = 0.0; // accumulator of hours flown
            double[] Sm = new double[] { 0.0, 0.0, 0.0, 0.0, 0.0 };
            double w = 0.0; // weight
            for (int i = 0; i < DailyOpsLi.Count; i++)
            {
                serli[(int)enSeries.HoursFlownPerDay].Points.AddXY(DailyOpsLi[i].DDay.DateTime, DailyOpsLi[i].dHrs);
                v += DailyOpsLi[i].dHrs; // Accumulated hours flown
                serli[(int)enSeries.CumulHoursFlown].Points.AddXY(DailyOpsLi[i].DDay.DateTime, v); // the integral of DailyOpsLi
                serli[(int)enSeries.TimeShiftedCumulHoursFlown].Points.AddXY(DOLiSd[i].DDay.DateTime, v); // the integral of time shifted s

                w = DOLiSd[i].DDay.Subtract(t0).TotalDays;
                serli[(int)enSeries.TimeDiffWeights].Points.AddXY(DOLiSd[i].DDay.DateTime, w);

                Sm[0] += w * w * w;
                Sm[1] += w * w;
                Sm[2] += w;
                Sm[3] += w * w * v;
                Sm[4] += w * v;
            }
            // Slope and intercept of extrapolation line:
            double k = (Sm[1] * Sm[4] - Sm[2] * Sm[3]) / (Sm[1] * Sm[1] - Sm[0] * Sm[2]);
            double c = (Sm[3] - Sm[0] * k) / Sm[1];

            Chart Chart1 = new Chart();
            Chart1.Series.Clear();
            Chart1.Series.Add(serli[(int)enSeries.HoursFlownPerDay]); // 0 - Flight hours per day
            Chart1.Series.Add(serli[(int)enSeries.CumulHoursFlown]); // 1 - Accumulated flight hours, integral of s
            Chart1.Series.Add(serli[(int)enSeries.TimeShiftedCumulHoursFlown]); // 2 - Time shifted accumulated flight hours
            Chart1.Series.Add(serli[(int)enSeries.TimeDiffWeights]); // 3 - Time differences after shifting

            DateTimeOffset tEnd = InopOpLi[InopOpLi.Count - 1].DStart;
            Chart1.Series.Add(serli[(int)enSeries.CumulHoursLimit]); // 4 - Flight time limit

            bool bArbitr = false;
            // Show the least-squares-fitted line with the inoperative intervals re-introduced:
            serli[(int)enSeries.BrokenLine] = BrokenLine(InopOpLi, k, c, t0, DuLastAction, muPrevHrs, udTAlimit, ref tDeadLine,
                ref bArbitr, iInopOpRefListIndex, DailyOpsLi[DailyOpsLi.Count - 1].DDay, iuAgingItem, serli[(int)enSeries.CumulHoursFlown]);
            serli[(int)enSeries.BrokenLine].ChartType = SeriesChartType.Line; // the extrapolation line
            Chart1.Series.Add(serli[(int)enSeries.BrokenLine]); // 5

            Chart1.Series[(int)enSeries.HoursFlownPerDay].XValueType  = ChartValueType.Date;
            Chart1.Series[(int)enSeries.HoursFlownPerDay].BorderColor = Color.Blue;
            Chart1.Series[(int)enSeries.HoursFlownPerDay].BorderWidth = 2;
            Chart1.Series[(int)enSeries.CumulHoursFlown].XValueType   = ChartValueType.Date;
            Chart1.Series[(int)enSeries.TimeShiftedCumulHoursFlown].XValueType = ChartValueType.Date;
            Chart1.Series[(int)enSeries.TimeDiffWeights].XValueType   = ChartValueType.Date;
            Chart1.Series[(int)enSeries.CumulHoursLimit].XValueType   = ChartValueType.Date;
            Chart1.Series[(int)enSeries.BrokenLine].XValueType        = ChartValueType.Date;

            string sTtl = (from a in eqdc.EQUIPAGINGITEMs where a.ID == iuAgingItem select a.sName).First();
            Title Ttl = new Title(sTtl + " - Flight Time versus Elapsed Calendar Time", Docking.Top,
                new Font("Arial", 14, FontStyle.Bold), Color.Black);

            Chart1.Titles.Clear();
            Chart1.Titles.Add(Ttl);

            Chart1.ChartAreas.Add(new ChartArea());
            ChartArea CA = Chart1.ChartAreas[0];

            CA.BackColor = Color.LightYellow;

            CA.AxisX.LineWidth = 2;
            CA.AxisX.LabelStyle.Format = "yyyy-MM-dd";
            CA.AxisX.Interval = 61;
            CA.AxisX.IntervalOffset = 1;
            CA.AxisX.Title = "Calendar Elapsed Time";
            CA.AxisX.TitleFont = new Font("Arial", 12, FontStyle.Regular, GraphicsUnit.Point);
            CA.AxisX.MajorGrid.LineDashStyle = ChartDashStyle.DashDotDot;

            DateTimeOffset minDate = InopOpLi[0].DStart.AddSeconds(-1);
            DateTimeOffset maxDate = InopOpLi[InopOpLi.Count - 1].DStart.AddSeconds(1);
            tEnd = DailyOpsLi.Last().DDay;
            if (tEnd > maxDate)
            {
                maxDate = tEnd;
            }
            if (tDeadLine > maxDate)
            {
                maxDate = tDeadLine;
            }
            maxDate = maxDate.AddDays(2);
            CA.AxisX.Minimum = minDate.Date.ToOADate();
            CA.AxisX.Maximum = maxDate.Date.ToOADate();
            CA.AxisX.IntervalType = DateTimeIntervalType.Auto;
            int iYears = (int)(maxDate.Subtract(minDate).TotalDays / 365.25);
            if (iYears > 4)
            {
                CA.AxisX.IntervalType = DateTimeIntervalType.Years;
                CA.AxisX.Interval = 1 + iYears / 17;
            }

            serli[(int)enSeries.CumulHoursLimit].Points.AddXY(InopOpLi[0].DStart.DateTime, udTAlimit);
            serli[(int)enSeries.CumulHoursLimit].Points.AddXY(maxDate.DateTime, udTAlimit);
            Chart1.Series[(int)enSeries.CumulHoursLimit] = serli[(int)enSeries.CumulHoursLimit]; // 4 - Flight time limit

            CA.AxisY.Title = "Flight Time, Hours per Day";
            Color myBlue = Color.FromArgb(50, 130, 255);
            CA.AxisY.TitleForeColor = myBlue;
            CA.AxisY.TitleFont = new Font("Arial", 12, FontStyle.Regular, GraphicsUnit.Point);
            CA.AxisY.LineWidth = 2;
            CA.AxisY.LineColor = myBlue;
            CA.AxisY.LabelStyle.ForeColor = myBlue;
            CA.AxisY.LabelStyle.Font = new Font("Arial", 12, FontStyle.Regular, GraphicsUnit.Point);
            CA.AxisY.MajorGrid.LineColor = myBlue;
            CA.AxisY.MajorTickMark.LineColor = myBlue;
            CA.AxisY.MajorGrid.LineDashStyle = ChartDashStyle.DashDotDot;

            CA.AxisY2.Enabled = AxisEnabled.True;
            CA.AxisY2.Title = "Accumulated Flight Time, Hours";
            CA.AxisY2.TitleForeColor = Color.Brown;
            CA.AxisY2.TitleFont = new Font("Arial", 12, FontStyle.Regular, GraphicsUnit.Point);
            CA.AxisY2.LineWidth = 2;
            CA.AxisY2.LineColor = Color.Brown;
            CA.AxisY2.LabelStyle.ForeColor = Color.Brown;
            CA.AxisY2.LabelStyle.Font = new Font("Arial", 12, FontStyle.Regular, GraphicsUnit.Point);
            CA.AxisY2.MajorGrid.LineColor = Color.Brown;
            CA.AxisY2.MajorTickMark.LineColor = Color.Brown;
            CA.AxisY2.MajorGrid.LineDashStyle = ChartDashStyle.DashDotDot;
            CA.AxisY2.IsStartedFromZero = false;

            Chart1.Series[(int)enSeries.CumulHoursFlown].YAxisType = AxisType.Secondary;
            Chart1.Series[(int)enSeries.CumulHoursFlown].Color = Color.Brown;
            Chart1.Series[(int)enSeries.CumulHoursFlown].BorderWidth = 3;

            Chart1.Series[(int)enSeries.TimeShiftedCumulHoursFlown].YAxisType = AxisType.Secondary;
            Chart1.Series[(int)enSeries.TimeShiftedCumulHoursFlown].Color = Color.Maroon;
            Chart1.Series[(int)enSeries.TimeShiftedCumulHoursFlown].BorderWidth = 2;
            Chart1.Series[(int)enSeries.TimeShiftedCumulHoursFlown].Enabled = false;

            Chart1.Series[(int)enSeries.TimeDiffWeights].YAxisType = AxisType.Secondary;
            Chart1.Series[(int)enSeries.TimeDiffWeights].Color = Color.Black;
            Chart1.Series[(int)enSeries.TimeDiffWeights].BorderWidth = 2;
            Chart1.Series[(int)enSeries.TimeDiffWeights].BorderDashStyle = ChartDashStyle.Solid;
            Chart1.Series[(int)enSeries.TimeDiffWeights].Enabled = false;

            Chart1.Series[(int)enSeries.CumulHoursLimit].YAxisType = AxisType.Secondary;
            Chart1.Series[(int)enSeries.CumulHoursLimit].Color = Color.Red;
            Chart1.Series[(int)enSeries.CumulHoursLimit].BorderWidth = 2;
            Chart1.Series[(int)enSeries.CumulHoursLimit].BorderDashStyle = ChartDashStyle.Solid;

            Chart1.Series[(int)enSeries.BrokenLine].YAxisType = AxisType.Secondary;
            Chart1.Series[(int)enSeries.BrokenLine].Color = Color.ForestGreen;
            Chart1.Series[(int)enSeries.BrokenLine].BorderWidth = 3;
            Chart1.Series[(int)enSeries.BrokenLine].BorderDashStyle = ChartDashStyle.Dash;

            //// Remove negative values from the start of the broken line
            //if (Chart1.Series[(int)enSeries.BrokenLine].Points.Count > 2)
            //{
            //    for (int iAt = Chart1.Series[(int)enSeries.BrokenLine].Points.Count - 1; iAt >= 0; iAt--)
            //    {
            //        if (Chart1.Series[(int)enSeries.BrokenLine].Points[iAt].YValues[0] < 0.0)
            //        {
            //            Chart1.Series[(int)enSeries.BrokenLine].Points.RemoveAt(iAt);
            //        }
            //    }
            //}

            double dY2AxisMinimum = double.MaxValue;
            double dY2AxisMaximum = double.MinValue;
            foreach (Series ser in Chart1.Series)
            {
                if (ser.Enabled && ser.YAxisType == AxisType.Secondary)
                {
                    double dymin = double.MaxValue;
                    double dymax = double.MinValue;
                    foreach (DataPoint dpt in ser.Points)
                    {
                        if (dymin > dpt.YValues[0]) dymin = dpt.YValues[0];
                        if (dymax < dpt.YValues[0]) dymax = dpt.YValues[0];
                    }
                    if (dY2AxisMinimum > dymin) dY2AxisMinimum = dymin;
                    if (dY2AxisMaximum < dymax) dY2AxisMaximum = dymax;
                }
            }
            if (Math.Abs(dY2AxisMinimum) < 2.0)
            {
                if (dY2AxisMinimum >= 0.0) { dY2AxisMinimum = 0.0; } else { dY2AxisMinimum = -2.0; };
            }
            else if (dY2AxisMinimum > 0.0)
            {
                double du = Math.Pow(10.0, Math.Floor(Math.Log10(dY2AxisMinimum)));
                dY2AxisMinimum = du * Math.Floor(dY2AxisMinimum / du);
            }
            else if (dY2AxisMinimum < 0.0)
            {
                double du = Math.Pow(10.0, Math.Ceiling(Math.Log10(-dY2AxisMinimum)) - 1);
                dY2AxisMinimum = -du * Math.Ceiling(-dY2AxisMinimum / du);
            }
            CA.AxisY2.Minimum = dY2AxisMinimum;

            double dux = Math.Pow(10.0, Math.Ceiling(Math.Log10(dY2AxisMaximum)) - 1);
            dY2AxisMaximum = dux * Math.Ceiling(dY2AxisMaximum / dux);
            CA.AxisY2.Maximum = dY2AxisMaximum;

            TextAnnotation TAaft = new TextAnnotation();
            TAaft.Text = "Accumulated" + Environment.NewLine + "Flight Time";
            TAaft.ForeColor = Color.Brown;
            TAaft.Font = new Font("Arial", 12, FontStyle.Bold, GraphicsUnit.Point);
            TAaft.SetAnchor(Chart1.Series[(int)enSeries.CumulHoursFlown].Points[Chart1.Series[(int)enSeries.CumulHoursFlown].Points.Count-1]);
            TAaft.AnchorAlignment = ContentAlignment.BottomRight;
            Chart1.Annotations.Add(TAaft);

            TextAnnotation TAlim = new TextAnnotation();
            TAlim.Text = "Accumulated Flight Time Limit " + udTAlimit.ToString("F2") + " hours";
            TAlim.ForeColor = Color.Red;
            TAlim.Font = new Font("Arial", 12, FontStyle.Regular, GraphicsUnit.Point);
            TAlim.SetAnchor(Chart1.Series[(int)enSeries.CumulHoursLimit].Points[1]);
            TAlim.AnchorAlignment = ContentAlignment.BottomRight;
            Chart1.Annotations.Add(TAlim);

            if (Chart1.Series[(int)enSeries.BrokenLine].Points.Count > 1)
            {
                TextAnnotation TAx = new TextAnnotation();
                TAx.Text = "Extrapolated" + Environment.NewLine + "Accumulated Flight Time";
                TAx.Alignment = ContentAlignment.TopLeft;
                TAx.ForeColor = Color.ForestGreen;
                TAx.Font = new Font("Arial", 12, FontStyle.Regular, GraphicsUnit.Point);
                TAx.SetAnchor(Chart1.Series[(int)enSeries.BrokenLine].Points[Chart1.Series[(int)enSeries.BrokenLine].Points.Count - 2]);
                TAx.AnchorAlignment = ContentAlignment.TopLeft;
                Chart1.Annotations.Add(TAx);
            }

            if (tDeadLine < DateTimeOffset.MaxValue.AddYears(-1))
            {
                serli[(int)enSeries.Deadline].Points.AddXY(tDeadLine.DateTime, CA.AxisY2.Maximum);
                serli[(int)enSeries.Deadline].Points.AddXY(tDeadLine.AddSeconds(1).DateTime, (CA.AxisY2.Maximum + CA.AxisY2.Minimum) / 2.0);
                serli[(int)enSeries.Deadline].Points.AddXY(tDeadLine.AddSeconds(2).DateTime, CA.AxisY2.Minimum);
                Chart1.Series.Add(serli[(int)enSeries.Deadline]); // 6 - Deadline
                int iSeriesDeadLine = Chart1.Series.Count - 1; // index to the just added Series
                Chart1.Series[iSeriesDeadLine].XValueType = ChartValueType.Date;
                Chart1.Series[iSeriesDeadLine].YAxisType = AxisType.Secondary;
                Chart1.Series[iSeriesDeadLine].BorderColor = Color.DarkMagenta;
                Chart1.Series[iSeriesDeadLine].BorderWidth = 2;

                TextAnnotation TADeadline = new TextAnnotation();
                TADeadline.Text = "Deadline" + (bArbitr ? " (arbitrarily extended)" : "") + Environment.NewLine + "~ " + tDeadLine.ToString("yyyy/MM/dd");
                TADeadline.ForeColor = Color.DarkMagenta;
                TADeadline.Font = new Font("Arial", 12, FontStyle.Bold, GraphicsUnit.Point);
                TADeadline.SetAnchor(Chart1.Series[(int)enSeries.Deadline].Points[1]);
                TADeadline.AnchorAlignment = ContentAlignment.MiddleCenter;
                Chart1.Annotations.Add(TADeadline);
            }

            StringWriter wr = new StringWriter();
            Chart1.Serializer.Content = SerializationContents.Default;
            Chart1.Serializer.Save(wr);
            suChart = wr.ToString();
            wr.Close();

            return tDeadLine;
        }

        private static DateTimeOffset DLSExtrapCycles(int iuAgingItem, DateTimeOffset DLastAction, int iuPrevCycles, int uiTAlimit, ref string suChart)
        {
            // Perform Least Squares Extrapolation for the number of operational cycles

            // DuLastAction = Date and Time of last (=latest) action taken with respect to this aging item
            // uiTAlimit = the target limit of accumulated cycles for which an extrapolation in time is desired
            List<InopOpIntv> InopOpLi = new List<InopOpIntv>(); // List of inoperative/operative intervals
            List<DailyCycles> DailyCyclesLi = new List<DailyCycles>(); // Daily operations list, cycles per day
            DateTimeOffset t0; // Reference start time
            DateTimeOffset tDeadLine = DateTimeOffset.MaxValue.AddYears(-1);
            EquipmentDataContext eqdc = new EquipmentDataContext();
            // Pointer to the operational calendar
            int iOpCal = (from a in eqdc.EQUIPAGINGITEMs where a.ID == iuAgingItem select a.iOpCal).First();
            var qo = from o in eqdc.OPSCALTIMEs where o.iOpsCal == iOpCal orderby o.DStart select o;
            // SCR 214 start
            // The interval for which extrapolation data is acquired:
            DateTime DInopOpStart = DateTime.MinValue;
            DateTime DInopOpStop = DateTime.MinValue;
            // SCR 214 end
            foreach (var o in qo)
            {
                // SCR 214 start
                if (DInopOpStart == DateTime.MinValue && o.bOpStatus)
                {
                    DInopOpStart = o.DStart.DateTime;
                    DInopOpStop = DateTime.MaxValue;
                }
                else if (!o.bOpStatus)
                {
                    DInopOpStop = o.DStart.Date;
                }
                // SCR 214 end
                InopOpLi.Add(new InopOpIntv(o.DStart.DateTime, o.bOpStatus));
            }

            // Use existence of flights for the piece of equipment pointed to by the aging item:
            int iEquip = (from ec in eqdc.EQUIPAGINGITEMs where ec.ID == iuAgingItem select ec.EQUIPCOMPONENT.iEquipment).First();
            var qw = from ww in eqdc.OPDETAILs
                     where ww.EQUIPMENT.ID == iEquip && ww.OPERATION.DBegin >= DInopOpStart && ww.OPERATION.DBegin <= DInopOpStop // SCR 214
                     orderby ww.OPERATION.DBegin
                     select new { ww.OPERATION.DBegin };
            List<DailyCycles> CyclesLiRaw = new List<DailyCycles>();
            foreach (var ww in qw)
            {
                CyclesLiRaw.Add(new DailyCycles((DateTime)ww.DBegin, 1));
            }
            // Aggregate the raw by-flight data into daily operations in terms of cycles flown per day
            DateTime tDay = DateTime.MinValue;
            int iDOLi = -1;
            for (int i = 0; i < CyclesLiRaw.Count; i++)
            {
                if (tDay != CyclesLiRaw[i].DDay.Date)
                {
                    iDOLi++;
                    tDay = CyclesLiRaw[i].DDay.Date;
                    DailyCyclesLi.Add(new DailyCycles(tDay, 0));
                }
                DailyCyclesLi[iDOLi].iCycles += CyclesLiRaw[i].iCycles;
            }
            if (DailyCyclesLi.Count < 2)
            {
                throw new Global.excToPopup("EqSupport.DLSExtrapCycles Error: The number of days of flying activity is " +
                    DailyCyclesLi.Count.ToString() + " which is too small to allow a least squares fit. Internal Aging Item ID = " + iuAgingItem.ToString());
            }

            // For purposes of fitting a line through the data, remove the time gaps when no operations can take place
            int iInopOpRefListIndex = -999;
            List<DailyCycles> DCLiSd = ShiftTSCycles(InopOpLi, DailyCyclesLi, iuAgingItem, ref iInopOpRefListIndex); // Shifted time sequence
            t0 = InopOpLi[iInopOpRefListIndex].DStart; // DCLiSd[0].DDay;

            List<Series> serli = new List<Series>();
            foreach (enSeries s in Enum.GetValues(typeof(enSeries)))
            {
                serli.Add(new Series());
            }

            // In the code below, think of 'hours' as 'cycles'
            serli[(int)enSeries.HoursFlownPerDay].ChartType = SeriesChartType.Candlestick; // this is 's'
            serli[(int)enSeries.CumulHoursFlown].ChartType = SeriesChartType.StepLine; // the integral of s
            serli[(int)enSeries.TimeShiftedCumulHoursFlown].ChartType = SeriesChartType.StepLine; // the integral of time shifted s
            serli[(int)enSeries.TimeDiffWeights].ChartType = SeriesChartType.StepLine; // weights in terms of time differences
            serli[(int)enSeries.CumulHoursLimit].ChartType = SeriesChartType.Line; // the flight time limit in number of cycles (not hours)
            serli[(int)enSeries.BrokenLine].ChartType = SeriesChartType.Line; // Extrapolated line
            serli[(int)enSeries.Deadline].ChartType = SeriesChartType.Line; // the deadline corresponding to the number of cycles limit

            // Least squares fit: see K:\DataAndDocs\07_Science&Technology\02_ComputerScience\Software\Projects\TSoar\TheTSoarProject.docx,
            //     Chapter on Design of Equipment Maintenance Support, subsection on linear extrapolation
            double v = 0.0;
            double[] Sm = new double[] { 0.0, 0.0, 0.0, 0.0, 0.0 };
            double w = 0.0; // weight
            for (int i = 0; i < DailyCyclesLi.Count; i++)
            {
                // Interpret 'HoursFlownPerDay' as 'CyclesFlownPerDay'
                serli[(int)enSeries.HoursFlownPerDay].Points.AddXY(DailyCyclesLi[i].DDay.DateTime, (double)DailyCyclesLi[i].iCycles);
                v += (double)DailyCyclesLi[i].iCycles; // Accumulated cycles flown
                serli[(int)enSeries.CumulHoursFlown].Points.AddXY(DailyCyclesLi[i].DDay.DateTime, v); // the integral of s = DailyCyclesLi
                serli[(int)enSeries.TimeShiftedCumulHoursFlown].Points.AddXY(DCLiSd[i].DDay.DateTime, v); // the integral of time shifted s

                w = DCLiSd[i].DDay.Subtract(t0).TotalDays;
                serli[(int)enSeries.TimeDiffWeights].Points.AddXY(DCLiSd[i].DDay.DateTime, w);

                Sm[0] += w * w * w;
                Sm[1] += w * w;
                Sm[2] += w;
                Sm[3] += w * w * v;
                Sm[4] += w * v;
            }
            // Slope and intercept of extrapolation line:
            double k = (Sm[1] * Sm[4] - Sm[2] * Sm[3]) / (Sm[1] * Sm[1] - Sm[0] * Sm[2]);
            double c = (Sm[3] - Sm[0] * k) / Sm[1];

            Chart Chart1 = new Chart();
            Chart1.Series.Clear();
            Chart1.Series.Add(serli[(int)enSeries.HoursFlownPerDay]); // 0 - Cycles per day
            Chart1.Series.Add(serli[(int)enSeries.CumulHoursFlown]); // 1 - Accumulated cycles, integral of s
            Chart1.Series.Add(serli[(int)enSeries.TimeShiftedCumulHoursFlown]); // 2 - Time shifted accumulated cycles
            Chart1.Series.Add(serli[(int)enSeries.TimeDiffWeights]); // 3 - Time differences after shifting

            DateTimeOffset tEnd = InopOpLi[InopOpLi.Count - 1].DStart;
            Chart1.Series.Add(serli[(int)enSeries.CumulHoursLimit]); // 4 - Number of cycles limit

            bool bArbitr = false;
            // Show the least-squares-fitted line with the inoperative intervals re-introduced:
            serli[(int)enSeries.BrokenLine] = BrokenLine(InopOpLi, k, c, t0, DLastAction, iuPrevCycles, (double)uiTAlimit, ref tDeadLine,
                ref bArbitr, iInopOpRefListIndex, DailyCyclesLi[DailyCyclesLi.Count - 1].DDay, iuAgingItem, serli[(int)enSeries.CumulHoursFlown]);
            serli[(int)enSeries.BrokenLine].ChartType = SeriesChartType.Line; // the extrapolation line
            Chart1.Series.Add(serli[(int)enSeries.BrokenLine]); // 5

            Chart1.Series[(int)enSeries.HoursFlownPerDay].XValueType = ChartValueType.Date; // Interpret as 'Cycles Flown per Day'
            Chart1.Series[(int)enSeries.HoursFlownPerDay].BorderColor = Color.Blue;
            Chart1.Series[(int)enSeries.HoursFlownPerDay].BorderWidth = 2;
            Chart1.Series[(int)enSeries.CumulHoursFlown].XValueType = ChartValueType.Date; // Interpret as 'Cumulative Cycles Flown'
            Chart1.Series[(int)enSeries.TimeShiftedCumulHoursFlown].XValueType = ChartValueType.Date;
            Chart1.Series[(int)enSeries.TimeDiffWeights].XValueType = ChartValueType.Date;
            Chart1.Series[(int)enSeries.CumulHoursLimit].XValueType = ChartValueType.Date;
            Chart1.Series[(int)enSeries.BrokenLine].XValueType = ChartValueType.Date;

            string sTtl = (from a in eqdc.EQUIPAGINGITEMs where a.ID == iuAgingItem select a.sName).First();
            Title Ttl = new Title(sTtl + " - Number of Cycles versus Elapsed Calendar Time", Docking.Top,
                new Font("Arial", 14, FontStyle.Bold), Color.Black);

            Chart1.Titles.Clear();
            Chart1.Titles.Add(Ttl);

            Chart1.ChartAreas.Add(new ChartArea());
            ChartArea CA = Chart1.ChartAreas[0];

            CA.BackColor = Color.FromArgb(255, 242, 253);

            CA.AxisX.LineWidth = 2;
            CA.AxisX.LabelStyle.Format = "yyyy-MM-dd";
            CA.AxisX.Interval = 61;
            CA.AxisX.IntervalOffset = 1;
            CA.AxisX.Title = "Calendar Elapsed Time";
            CA.AxisX.TitleFont = new Font("Arial", 12, FontStyle.Regular, GraphicsUnit.Point);
            CA.AxisX.MajorGrid.LineDashStyle = ChartDashStyle.DashDotDot;

            DateTimeOffset minDate = InopOpLi[0].DStart.AddSeconds(-1);
            DateTimeOffset maxDate = InopOpLi[InopOpLi.Count - 1].DStart.AddSeconds(1);
            tEnd = DailyCyclesLi.Last().DDay;
            if (tEnd > maxDate)
            {
                maxDate = tEnd;
            }
            if (tDeadLine > maxDate)
            {
                maxDate = tDeadLine;
            }
            maxDate = maxDate.AddDays(2);
            CA.AxisX.Minimum = minDate.Date.ToOADate();
            CA.AxisX.Maximum = maxDate.Date.ToOADate();
            CA.AxisX.IntervalType = DateTimeIntervalType.Auto;
            int iYears = (int)(maxDate.Subtract(minDate).TotalDays / 365.25);
            if (iYears > 4)
            {
                CA.AxisX.IntervalType = DateTimeIntervalType.Years;
                CA.AxisX.Interval = 1 + iYears / 17;
            }

            serli[(int)enSeries.CumulHoursLimit].Points.AddXY(InopOpLi[0].DStart.DateTime, (double)uiTAlimit);
            serli[(int)enSeries.CumulHoursLimit].Points.AddXY(maxDate.DateTime, (double)uiTAlimit);
            Chart1.Series[(int)enSeries.CumulHoursLimit] = serli[(int)enSeries.CumulHoursLimit]; // 4 - Number of Cycles limit

            CA.AxisY.Title = "Cycles per Day";
            Color myBlue = Color.FromArgb(50, 130, 255);
            CA.AxisY.TitleForeColor = myBlue;
            CA.AxisY.TitleFont = new Font("Arial", 12, FontStyle.Regular, GraphicsUnit.Point);
            CA.AxisY.LineWidth = 2;
            CA.AxisY.LineColor = myBlue;
            CA.AxisY.LabelStyle.ForeColor = myBlue;
            CA.AxisY.LabelStyle.Font = new Font("Arial", 12, FontStyle.Regular, GraphicsUnit.Point);
            CA.AxisY.MajorGrid.LineColor = myBlue;
            CA.AxisY.MajorTickMark.LineColor = myBlue;
            CA.AxisY.MajorGrid.LineDashStyle = ChartDashStyle.DashDotDot;

            CA.AxisY2.Enabled = AxisEnabled.True;
            CA.AxisY2.Title = "Accumulated Cycles";
            CA.AxisY2.TitleForeColor = Color.Brown;
            CA.AxisY2.TitleFont = new Font("Arial", 12, FontStyle.Regular, GraphicsUnit.Point);
            CA.AxisY2.LineWidth = 2;
            CA.AxisY2.LineColor = Color.Brown;
            CA.AxisY2.LabelStyle.ForeColor = Color.Brown;
            CA.AxisY2.LabelStyle.Font = new Font("Arial", 12, FontStyle.Regular, GraphicsUnit.Point);
            CA.AxisY2.MajorGrid.LineColor = Color.Brown;
            CA.AxisY2.MajorTickMark.LineColor = Color.Brown;
            CA.AxisY2.MajorGrid.LineDashStyle = ChartDashStyle.DashDotDot;

            Chart1.Series[(int)enSeries.CumulHoursFlown].YAxisType = AxisType.Secondary; // Interpret as 'Accumulated Cycles Flown'
            Chart1.Series[(int)enSeries.CumulHoursFlown].Color = Color.Brown;
            Chart1.Series[(int)enSeries.CumulHoursFlown].BorderWidth = 3;

            Chart1.Series[(int)enSeries.TimeShiftedCumulHoursFlown].YAxisType = AxisType.Secondary;
            Chart1.Series[(int)enSeries.TimeShiftedCumulHoursFlown].Color = Color.Maroon;
            Chart1.Series[(int)enSeries.TimeShiftedCumulHoursFlown].BorderWidth = 2;
            Chart1.Series[(int)enSeries.TimeShiftedCumulHoursFlown].Enabled = false;

            Chart1.Series[(int)enSeries.TimeDiffWeights].YAxisType = AxisType.Secondary;
            Chart1.Series[(int)enSeries.TimeDiffWeights].Color = Color.Black;
            Chart1.Series[(int)enSeries.TimeDiffWeights].BorderWidth = 2;
            Chart1.Series[(int)enSeries.TimeDiffWeights].BorderDashStyle = ChartDashStyle.Solid;
            Chart1.Series[(int)enSeries.TimeDiffWeights].Enabled = false;

            Chart1.Series[(int)enSeries.CumulHoursLimit].YAxisType = AxisType.Secondary;
            Chart1.Series[(int)enSeries.CumulHoursLimit].Color = Color.Red;
            Chart1.Series[(int)enSeries.CumulHoursLimit].BorderWidth = 2;
            Chart1.Series[(int)enSeries.CumulHoursLimit].BorderDashStyle = ChartDashStyle.Solid;

            Chart1.Series[(int)enSeries.BrokenLine].YAxisType = AxisType.Secondary;
            Chart1.Series[(int)enSeries.BrokenLine].Color = Color.ForestGreen;
            Chart1.Series[(int)enSeries.BrokenLine].BorderWidth = 3;
            Chart1.Series[(int)enSeries.BrokenLine].BorderDashStyle = ChartDashStyle.Dash;

            //// Remove negative values from the start of the broken line
            //if (Chart1.Series[(int)enSeries.BrokenLine].Points.Count > 2)
            //{
            //    for (int iAt = Chart1.Series[(int)enSeries.BrokenLine].Points.Count - 1; iAt >= 0; iAt--)
            //    {
            //        if (Chart1.Series[(int)enSeries.BrokenLine].Points[iAt].YValues[0] < 0.0)
            //        {
            //            Chart1.Series[(int)enSeries.BrokenLine].Points.RemoveAt(iAt);
            //        }
            //    }
            //}

            double dY2AxisMinimum = double.MaxValue;
            double dY2AxisMaximum = double.MinValue;
            foreach (Series ser in Chart1.Series)
            {
                if (ser.Enabled && ser.YAxisType == AxisType.Secondary)
                {
                    double dymin = double.MaxValue;
                    double dymax = double.MinValue;
                    foreach (DataPoint dpt in ser.Points)
                    {
                        if (dymin > dpt.YValues[0]) dymin = dpt.YValues[0];
                        if (dymax < dpt.YValues[0]) dymax = dpt.YValues[0];
                    }
                    if (dY2AxisMinimum > dymin) dY2AxisMinimum = dymin;
                    if (dY2AxisMaximum < dymax) dY2AxisMaximum = dymax;
                }
            }
            if (Math.Abs(dY2AxisMinimum) < 2.0)
            {
                if (dY2AxisMinimum >= 0.0) { dY2AxisMinimum = 0.0; } else { dY2AxisMinimum = -2.0; };
            }
            else if (dY2AxisMinimum > 0.0)
            {
                double du = Math.Pow(10.0, Math.Floor(Math.Log10(dY2AxisMinimum)));
                dY2AxisMinimum = du * Math.Floor(dY2AxisMinimum / du);
            }
            else if (dY2AxisMinimum < 0.0)
            {
                double du = Math.Pow(10.0, Math.Ceiling(Math.Log10(-dY2AxisMinimum)) - 1);
                dY2AxisMinimum = -du * Math.Ceiling(-dY2AxisMinimum / du);
            }
            CA.AxisY2.Minimum = dY2AxisMinimum;

            double dux = Math.Pow(10.0, Math.Ceiling(Math.Log10(dY2AxisMaximum)) - 1);
            dY2AxisMaximum = dux * Math.Ceiling(dY2AxisMaximum / dux);
            CA.AxisY2.Maximum = dY2AxisMaximum;

            TextAnnotation TAaft = new TextAnnotation();
            TAaft.Text = "Accumulated" + Environment.NewLine + "Flight Cycles";
            TAaft.ForeColor = Color.Brown;
            TAaft.Font = new Font("Arial", 12, FontStyle.Bold, GraphicsUnit.Point);
            TAaft.SetAnchor(Chart1.Series[(int)enSeries.CumulHoursFlown].Points[Chart1.Series[(int)enSeries.CumulHoursFlown].Points.Count - 1]);
            TAaft.AnchorAlignment = ContentAlignment.BottomRight;
            Chart1.Annotations.Add(TAaft);

            TextAnnotation TAlim = new TextAnnotation();
            TAlim.Text = "Accumulated Cycles Limit " + uiTAlimit.ToString();
            TAlim.ForeColor = Color.Red;
            TAlim.Font = new Font("Arial", 12, FontStyle.Regular, GraphicsUnit.Point);
            TAlim.SetAnchor(Chart1.Series[(int)enSeries.CumulHoursLimit].Points[1]);
            TAlim.AnchorAlignment = ContentAlignment.BottomRight;
            Chart1.Annotations.Add(TAlim);

            if (Chart1.Series[(int)enSeries.BrokenLine].Points.Count > 1)
            {
                TextAnnotation TAx = new TextAnnotation();
                TAx.Text = "Extrapolated" + Environment.NewLine + "Accumulated Cycles";
                TAx.Alignment = ContentAlignment.TopLeft;
                TAx.ForeColor = Color.ForestGreen;
                TAx.Font = new Font("Arial", 12, FontStyle.Regular, GraphicsUnit.Point);
                TAx.SetAnchor(Chart1.Series[(int)enSeries.BrokenLine].Points[Chart1.Series[(int)enSeries.BrokenLine].Points.Count - 2]);
                TAx.AnchorAlignment = ContentAlignment.TopLeft;
                Chart1.Annotations.Add(TAx);
            }

            if (tDeadLine < DateTimeOffset.MaxValue.AddYears(-1))
            {
                serli[(int)enSeries.Deadline].Points.AddXY(tDeadLine.DateTime, CA.AxisY2.Maximum);
                serli[(int)enSeries.Deadline].Points.AddXY(tDeadLine.AddSeconds(1).DateTime, (CA.AxisY2.Maximum + CA.AxisY2.Minimum) / 2.0);
                serli[(int)enSeries.Deadline].Points.AddXY(tDeadLine.AddSeconds(2).DateTime, CA.AxisY2.Minimum);
                Chart1.Series.Add(serli[(int)enSeries.Deadline]); // 6 - Deadline
                int iSeriesDeadLine = Chart1.Series.Count - 1; // index to the just added Series
                Chart1.Series[iSeriesDeadLine].XValueType = ChartValueType.Date;
                Chart1.Series[iSeriesDeadLine].YAxisType = AxisType.Secondary;
                Chart1.Series[iSeriesDeadLine].BorderColor = Color.DarkMagenta;
                Chart1.Series[iSeriesDeadLine].BorderWidth = 2;

                TextAnnotation TADeadline = new TextAnnotation();
                TADeadline.Text = "Deadline" + (bArbitr ? " (arbitrarily extended)" : "") + Environment.NewLine + "~ " + tDeadLine.ToString("yyyy/MM/dd");
                TADeadline.ForeColor = Color.DarkMagenta;
                TADeadline.Font = new Font("Arial", 12, FontStyle.Bold, GraphicsUnit.Point);
                TADeadline.SetAnchor(Chart1.Series[(int)enSeries.Deadline].Points[1]);
                TADeadline.AnchorAlignment = ContentAlignment.MiddleCenter;
                Chart1.Annotations.Add(TADeadline);
            }

            StringWriter wr = new StringWriter();
            Chart1.Serializer.Content = SerializationContents.Default;
            Chart1.Serializer.Save(wr);
            suChart = wr.ToString();
            wr.Close();

            return tDeadLine;
        }

        private static List<DailyOp> ShiftTS(List<InopOpIntv> InopOpLi, List<DailyOp> uDailyOpsLi, int iuAgingItem, ref int iuInopOpRefListIndex)
        {
            // InopOpLi = list of intervals of Inoperative/Operative times
            // uDailyOpsLi  = Daily Operations List; one element in the list holds the day's date and the number of flight hours on that day
            string sErrMsg = "Aging Item internal ID " + iuAgingItem.ToString() + " EqSupport.ShiftTS Error: ";

            // Shift the times of members of the input list such as to pretend that the gaps of inoperability do not exist.

            if (InopOpLi.Count < 2)
            {
                throw new Global.excToPopup(sErrMsg + "List InopOpLi.Count = " + InopOpLi.Count.ToString() + " < 2. i.e., too few time points in the operational calendar.");
            }
            // Find the point in InopOpLi with a calendar time that just precedes the first point in uDailyOpsLi. That index will become iuInopOpRefListIndex.
            int iInopOpListCounter = -1;
            foreach (InopOpIntv v in InopOpLi)
            {
                iInopOpListCounter++;
                if (v.DStart > uDailyOpsLi[0].DDay)
                {
                    if (iInopOpListCounter < 1)
                    {
                        throw new Global.excToPopup(sErrMsg + "The first point in the operational intervals list at " +  CustFmt.sFmtDate(v.DStart, CustFmt.enDFmt.DateAndTimeMinOffset) +
                            " occurs after the first point of the flight data at " +  CustFmt.sFmtDate(uDailyOpsLi[0].DDay, CustFmt.enDFmt.DateAndTimeMinOffset));
                    }
                    iInopOpListCounter--;
                    break;
                }
            }
            if (!InopOpLi[iInopOpListCounter].bOpStat)
            {
                throw new Global.excToPopup(sErrMsg + "The first point of the flight data at " +  CustFmt.sFmtDate(uDailyOpsLi[0].DDay, CustFmt.enDFmt.DateAndTimeMinOffset) +
                    " falls into an inoperative time interval starting at " +  CustFmt.sFmtDate(InopOpLi[iInopOpListCounter].DStart, CustFmt.enDFmt.DateAndTimeMinOffset));
            }
            iuInopOpRefListIndex = iInopOpListCounter;

            List<DailyOp> dol = new List<DailyOp>();
            TimeSpan ts = new TimeSpan(0, 0, 0, 0); // the amount of time shift
            DailyOp o;
            foreach (var y in uDailyOpsLi)
            {
                if (iInopOpListCounter < InopOpLi.Count - 1)
                {
                    if (y.DDay > InopOpLi[iInopOpListCounter + 1].DStart) // Crossing boundary between Inop/Op intervals?
                    {
                        iInopOpListCounter++; // cross into the next InopOpLi interval
                        bool bContinue = false;
                        do
                        {
                            if (!InopOpLi[iInopOpListCounter].bOpStat)
                            {
                                // This member of InopOpLi is an interval where no operations take place
                                ts += InopOpLi[iInopOpListCounter + 1].DStart - InopOpLi[iInopOpListCounter].DStart;
                                iInopOpListCounter++;
                                bContinue = true;
                            }
                            else
                            {
                                bContinue = false;
                            }
                        } while (bContinue && iInopOpListCounter < InopOpLi.Count - 1);
                    }
                    if (y.DDay >= InopOpLi[iInopOpListCounter].DStart)
                    {
                        o = new DailyOp(y.DDay - ts, y.dHrs); // perform the shift in time
                    }
                    else
                    {
                        throw new Global.excToPopup(sErrMsg + "The day of flight operations " + CustFmt.sFmtDate(y.DDay, CustFmt.enDFmt.DateAndTimeMinOffset)
                            + " falls into an inoperational interval " +
                            CustFmt.sFmtDate(InopOpLi[iInopOpListCounter - 1].DStart, CustFmt.enDFmt.DateAndTimeMinOffset) + " to " +
                            CustFmt.sFmtDate(InopOpLi[iInopOpListCounter].DStart, CustFmt.enDFmt.DateAndTimeMinOffset));
                    }
                }
                else
                {
                    o = new DailyOp(y.DDay - ts, y.dHrs);
                }
                dol.Add(o);
            }
            return dol;
        }

        private static List<DailyCycles> ShiftTSCycles(List<InopOpIntv> InopOpLi, List<DailyCycles> uDailyCyclesLi, int iuAgingItem, ref int iuInopOpRefListIndex)
        {
            // InopOpLi = list of intervals of Inoperative/Operative times
            // uDailyCyclesLi  = Daily Cycles List; one element in the list holds the day's date and the number of cycles on that day
            string sErrMsg = "Aging Item internal ID " + iuAgingItem.ToString() + " EqSupport.ShiftTSCycles Error: ";

            // Shift the times of members of the input list such as to pretend that the gaps of inoperability do not exist.

            // If there are no operational intervals where no operations are allowed, then there is no shifting to do:
            EquipmentDataContext eqdc = new EquipmentDataContext();
            int iCnt = (from c in InopOpLi where !c.bOpStat select c).Count();
            if (iCnt < 1)
            {
                iuInopOpRefListIndex = 0;
                return uDailyCyclesLi;
            }

            if (InopOpLi.Count < 2)
            {
                throw new Global.excToPopup(sErrMsg + "List InopOpLi.Count = " + InopOpLi.Count.ToString() + " < 2. i.e., too few time points in the operational calendar.");
            }

            // Find the point in InopOpLi with a calendar time that just precedes the first point in uDailyCyclesLi. That index will become iuInopOpRefListIndex.
            int iInopOpListCounter = -1;
            foreach (InopOpIntv v in InopOpLi)
            {
                iInopOpListCounter++;
                if (v.DStart > uDailyCyclesLi[0].DDay)
                {
                    if (iInopOpListCounter < 1)
                    {
                        throw new Global.excToPopup(sErrMsg + "The first point in the operational intervals list at " + CustFmt.sFmtDate(v.DStart, CustFmt.enDFmt.DateAndTimeMinOffset) +
                            " occurs after the first point of the flight data at " + CustFmt.sFmtDate(uDailyCyclesLi[0].DDay, CustFmt.enDFmt.DateAndTimeMinOffset));
                    }
                    iInopOpListCounter--;
                    break;
                }
            }
            if (!InopOpLi[iInopOpListCounter].bOpStat)
            {
                throw new Global.excToPopup(sErrMsg + "The first point of the flight data at " + CustFmt.sFmtDate(uDailyCyclesLi[0].DDay, CustFmt.enDFmt.DateAndTimeMinOffset) +
                    " falls into an inoperative time interval starting at " + CustFmt.sFmtDate(InopOpLi[iInopOpListCounter].DStart, CustFmt.enDFmt.DateAndTimeMinOffset));
            }
            iuInopOpRefListIndex = iInopOpListCounter;

            List<DailyCycles> dol = new List<DailyCycles>();
            TimeSpan ts = new TimeSpan(0, 0, 0, 0); // the amount of time shift
            DailyCycles o;
            foreach (var y in uDailyCyclesLi)
            {
                if (iInopOpListCounter < InopOpLi.Count - 1)
                {
                    if (y.DDay > InopOpLi[iInopOpListCounter + 1].DStart) // Crossing boundary between Inop/Op intervals?
                    {
                        iInopOpListCounter++; // cross into the next InopOpLi interval
                        bool bContinue = false;
                        do
                        {
                            if (!InopOpLi[iInopOpListCounter].bOpStat)
                            {
                                // This member of InopOpLi is an interval where no operations take place
                                ts += InopOpLi[iInopOpListCounter + 1].DStart - InopOpLi[iInopOpListCounter].DStart;
                                iInopOpListCounter++;
                                bContinue = true;
                            }
                            else
                            {
                                bContinue = false;
                            }
                        } while (bContinue && iInopOpListCounter < InopOpLi.Count - 1);
                    }
                    if (y.DDay >= InopOpLi[iInopOpListCounter].DStart)
                    {
                        o = new DailyCycles(y.DDay - ts, y.iCycles); // perform the shift in time
                    }
                    else
                    {
                        throw new Global.excToPopup(sErrMsg + "The day of flight operations " + CustFmt.sFmtDate(y.DDay, CustFmt.enDFmt.DateAndTimeMinOffset) + " falls into an inoperational interval" +
                            CustFmt.sFmtDate(InopOpLi[iInopOpListCounter - 1].DStart, CustFmt.enDFmt.DateAndTimeMinOffset) + " to " + 
                            CustFmt.sFmtDate(InopOpLi[iInopOpListCounter].DStart, CustFmt.enDFmt.DateAndTimeMinOffset));
                    }
                }
                else
                {
                    o = new DailyCycles(y.DDay - ts, y.iCycles);
                }
                dol.Add(o);
            }
            return dol;
        }

        static Series BrokenLine(List<InopOpIntv> uInopOpLi, double uk, double uc, DateTimeOffset ut0, DateTimeOffset DuLastAction, decimal muPrevHrs, double uTAlimit,
            ref DateTimeOffset utDeadLine, ref bool ubArbitr, int iuInopOpRefListIndex, DateTimeOffset DuLatest, int iuAgingItem, Series seruAccumHrs)
        {
            // Main output is a datavisualization series with points that represent a broken line on the chart
            Series serRet = new Series();
            // uInopOpLi = List of operative/inoperative time intervals
            // uk = slope of unbroken line
            // uc = intercept of unbroken line
            // ut0 = reference DateTimeOffset
            // The unbroken line applies unmodified at index iuInopOpRefListIndex of uInopOpLi
            // Point of broken line with index iuInopOpRefListIndex is at the time reference point ut0
            // DuLastAction = Date and time when the last action was taken on the aging item of interest
            // muPrevHrs = flight time accumulated up to DuLastAction
            // uTAlimit = the target limit of operating hours for which an extrapolation in time is desired,
            //      i.e., when does the broken line reach this value?
            // utDeadLine - calculated by this routine
            // ubArbitr - if true then the deadline calculation ran into difficulties; deadline is arbitrary: 1 year past end of ops calendar
            // iuInopOpRefListIndex = Index to the point in the list of Inoperative/Operative time intervals (uInopOpLi) that just precedes
            //      the first point in the list of flight hours versus day of operations.
            // DuLatest = point in time when chart ends

            // intercept on vertical axis
            double dvLast = uc;

            DateTimeOffset tLast = ut0;
            bool bPrevStat = false;
            bool bDeadLineFound = false;
            int iInopOpListCounter = -1;
            Series serTemp1 = new Series();
            // Going backwards from iuInopOpRefListIndex:
            if (iuInopOpRefListIndex > 0)
            {
                for (iInopOpListCounter = iuInopOpRefListIndex; iInopOpListCounter >= 0; iInopOpListCounter--)
                {
                    // Broken line segment has slope uk when bOpStat is true. Slope = 0 for bOpStat = false.
                    if (uInopOpLi[iInopOpListCounter].bOpStat)
                    {
                        dvLast += uk * (uInopOpLi[iInopOpListCounter].DStart - tLast).TotalDays;
                    }
                    serTemp1.Points.AddXY(uInopOpLi[iInopOpListCounter].DStart.DateTime, dvLast);
                    tLast = uInopOpLi[iInopOpListCounter].DStart;
                }
            }
            // Add the points from going backwards in the proper going forward order:
            Series serTemp2 = new Series();
            for (iInopOpListCounter = serTemp1.Points.Count - 1; iInopOpListCounter > 0; iInopOpListCounter--)
            {
                serTemp2.Points.Add(serTemp1.Points[iInopOpListCounter]);
            }

            // Now let's go forward starting at iuInopOpRefListIndex:
            dvLast = uc;
            bPrevStat = false;
            iInopOpListCounter = -1;
            foreach (InopOpIntv g in uInopOpLi)
            {
                if (++iInopOpListCounter >= iuInopOpRefListIndex)
                {
                    // End of broken line segment is start of next segment
                    if (bPrevStat)
                    {
                        dvLast += uk * g.DStart.Subtract(tLast).TotalDays;
                    }
                    serTemp2.Points.AddXY(g.DStart.DateTime, dvLast);
                    bPrevStat = g.bOpStat;
                    tLast = g.DStart;
                }
            }
            // serTemp2 now contains all the broken line segments in proper sequence, but not yet with the desired vertical shift.

            // What is the value of the broken line serTemp2 at time = DuLastAction ?
            double dHours = 0.0;
            double dLastAct = DuLastAction.DateTime.ToOADate();
            DataPoint PtPrevLow = serTemp2.Points[0];
            if (dLastAct < PtPrevLow.XValue)
            {
                EquipmentDataContext eqdc = new EquipmentDataContext();
                string sAgingItem = (from a in eqdc.EQUIPAGINGITEMs where a.ID == iuAgingItem select a.sName).First();
                throw new Global.excToPopup("EqSupport.cs.BrokenLine Error: Date of Last Action " +
                    CustFmt.sFmtDate(DuLastAction,CustFmt.enDFmt.DateAndTimeMinOffset) +
                    " is earlier than first date of operational calendar " +
                    CustFmt.sFmtDate(uInopOpLi[0].DStart, CustFmt.enDFmt.DateAndTimeMinOffset) +
                    ". Aging Item index =" + iuAgingItem.ToString() + " ('" + sAgingItem + "').");
            }
            int iSeg = -1;
            foreach (var pt in serTemp2.Points)
            {
                iSeg++;
                if (iSeg > 0)
                {
                    if (dLastAct < pt.XValue)
                    {
                        // Found the segment into which dLastAct falls
                        if (iSeg < serTemp2.Points.Count - 1)
                        {
                            dHours = PtPrevLow.YValues[0] + (dLastAct - PtPrevLow.XValue) * (pt.YValues[0] - PtPrevLow.YValues[0]) / (pt.XValue - PtPrevLow.XValue);
                        }
                        else
                        {
                            dHours = pt.YValues[0] + uk * (dLastAct - pt.XValue);
                        }
                        break;
                    }
                }
                PtPrevLow = pt;
            }
            // At DuLastAction (=dLastAct) the broken line value is supposed to be muPrevHrs; so, we need to shift broken line serTemp2 up by (muPrevHrs - dHours):
            double diff = (double)muPrevHrs - dHours;
            foreach (var pt in serTemp2.Points)
            {
                serRet.Points.AddXY(pt.XValue, pt.YValues[0] + diff);
            }
            dvLast += diff;
            iSeg = -1;
            foreach (var pt in seruAccumHrs.Points)
            {
                iSeg++;
                seruAccumHrs.Points[iSeg].YValues[0] = pt.YValues[0] + diff;
            }

            //What is the time on broken line SerRet at YValue uTAlimit ?
            double dDaysDiff = 0.0;
            for (iInopOpListCounter = 1; iInopOpListCounter < serRet.Points.Count; iInopOpListCounter++)
            { 
                if (uTAlimit < serRet.Points[iInopOpListCounter].YValues[0])
                {
                    dDaysDiff = (uTAlimit - serRet.Points[iInopOpListCounter - 1].YValues[0]) / uk;
                    utDeadLine = uInopOpLi[iInopOpListCounter - 1].DStart.AddDays(dDaysDiff);
                    bDeadLineFound = true;
                    break;
                }
            }
            ubArbitr = false;
            if (!bDeadLineFound)
            {
                // Try to extend the last InopOp segment if bOpStat is true, i.e. it is an operational segment:
                iInopOpListCounter = uInopOpLi.Count - 1;
                InopOpIntv ge = uInopOpLi[iInopOpListCounter];
                if (ge.bOpStat)
                {
                    utDeadLine = tLast.AddDays((uTAlimit - dvLast) / uk);
                    if (utDeadLine > DuLatest) DuLatest = utDeadLine;
                    dvLast += uk * DuLatest.Subtract(ge.DStart).TotalDays;
                    serRet.Points.AddXY(DuLatest.DateTime, dvLast);
                }
                else
                {
                    ubArbitr = true;
                    utDeadLine = ge.DStart.AddYears(1); // arbitrarily add one year
                    serRet.Points.AddXY(utDeadLine.DateTime, dvLast);
                }
            }
            return serRet;
        }

        public static bool bParentage(int iuID, int iuParent)
        {
            // iuParent = ID of record in table EQUIPCOMPONENTS that is supposed to become the parent of record with iuID.
            // What we need to make sure is that iuParent does not show up in the hierarchical tree below or at iuID.
            // Therefore, we start at iuParent and go up the hierarchy until either of two conditions:
            //    (1) we reach a record with ID = iuID which means that we started below iuID;
            //        that's bad because a record cannot be its own parent or grandparent or greastgrandparent etc.
            //           -=> return false
            //    (2) we reach a record with iParent = 0, i.e., a record that does not have a parent;
            //        that's good because we did not encounter a record with ID = iuID which means that we
            //        did not attempt to assign as parent a record under iuID.
            //           -=> return true

            if (iuParent == 0) return true;

            EquipmentDataContext dc = new EquipmentDataContext();
            int iID = iuParent;
            if (iuID == iID) return false; // A record cannot be its own parent
            // What is the parent (or UpLine) of iID?
            int iUpLine = -1;
            do // Traverse the hierarchy
            {
                var li = (from r in dc.EQUIPCOMPONENTs where r.ID == iID select r.iParentComponent).ToList();
                if (li.Count < 1)
                {
                    throw new Global.excToPopup("EqSupport.bParentage ERROR: empty list when iuID=" + iuID.ToString() + ", iuParent=" + iuParent.ToString());
                }
                iUpLine = li.First();
                if (iUpLine == iuID)
                {
                    return false;
                }
                else
                {
                    if (iUpLine == 0)
                    {
                        return true;
                    }
                }
                iID = iUpLine;
            } while (true);
        }

        public static string sExpandedComponentName(int iuEqComp)
        {
            // Assemble a string starting with the component's name and then adding its parent / grandparent / etc.
            string sRet = "";
            string sConn = "";
            EquipmentDataContext dc = new EquipmentDataContext();
            int iContinue = 100;
            int iParent = iuEqComp;
            string sEquip = "";
            do
            {
                if (iContinue > 1)
                {
                    var qc = (from c in dc.EQUIPCOMPONENTs where c.ID == iParent select new { c.sComponent, c.iParentComponent, c.iEquipment, c.EQUIPMENT.sShortEquipName }).First();
                    sRet += sConn + (char)39 + qc.sComponent + (char)39;
                    sConn = " of ";
                    iParent = qc.iParentComponent;
                    if (iParent == 0)
                    {
                        iContinue = 2;
                        sEquip = qc.sShortEquipName;
                    }
                }
                else
                {
                    sRet += sConn + (char)39 + sEquip + (char)39;
                }

            } while (--iContinue > 0);
            return sRet;
        }
    }

    class DBeginEnd
    {
        public DateTimeOffset DBegin { get; set; } = DateTimeOffset.MinValue;
        public DateTimeOffset DEnd { get; set; } = new DateTimeOffset(2199, 12, 31, 23, 59, 59, new TimeSpan(0));
        public DBeginEnd ()
        {
            DBegin = DateTimeOffset.MinValue;
            DEnd = new DateTimeOffset(2199, 12, 31, 23, 59, 59, new TimeSpan(0));
        }
        public DBeginEnd (DateTimeOffset DuBegin, DateTimeOffset DuEnd)
        {
            DBegin = DuBegin;
            DEnd = DuEnd;
        }
    }

    public class ActIllustr
    {
        // Action Item Illustration
        // ========================

        public enum enSeries
        {
            EntireEquipment, Component, ParameterSet, AgingItem, ManualOpsDataReset, ManualOpsDataBegin, ManualOpsDataEnd, FlightOpsData, ActionItem,
            LastAction, OpsCalendar
        }
        #region Properties
        // Public Properties
        // -----------------
        public Bitmap pBitm { get; set; } = new Bitmap(901, 651);
        public int ipAgingItem { get; set; } = 0; // pointer to record in table EQUIPAGINGITEMS

        // Private Properties
        // ------------------
        //     Index by enSeries:
        private string[] saBoxedText = new string[] {"Entire Equipment", "Component", "Parameter Set",
                "Aging Item", "Manually Entered Operational Data", "", "", "Flight Operations Data", "Action Item: Completions and Deadlines", "", "Operational Calendar"};
        private int[] iaVert = new int[] { 8, 7, 6, 4, 3, 3, 3, 2, 1, 4, 5 };
        private bool[] baDrawArrowPattern = new bool[] { true, true, true, true, false, false, false, false, false, false, false };
        private bool[] baDrawArrow = new bool[0];
        //     Not indexed:
        private DBeginEnd DBEComponent = new DBeginEnd(); // DLinkBegin and DLinkEnd of the equipment component
        private DBeginEnd DBEEntireComp = new DBeginEnd(); // DLinkBegin and DLinkEnd of the Entire equipment component
        private DBeginEnd DBEAgingItem = new DBeginEnd(); // DStart and DEnd of the aging item
        private List<DateTime> LiDeadLines = new List<DateTime>(); // Action Item deadlines
        private string sParSet = "TBD"; // string that characterizes the parameterset; for display in the illustration
        private List<DBeginEnd> liDBE_OpsCal = new List<DBeginEnd>(); // List of Begin/End dates of operational calendar
        private List<DBeginEnd> liDBE_OpsDatReset = new List<DBeginEnd>(); // List of Begin/End dates of manually entered operational reset data
        private List<DBeginEnd> liDBE_OpsDatOngoing = new List<DBeginEnd>(); // List of Begin/End dates of manually entered operational data
        private List<DateTime> liDFlightOps = new List<DateTime>(); // List of begin date/times of flight operations
        #endregion
        public string sIllustration(DateTimeOffset DuLastAction)
        {
#if (IllustrationDEDBUG)
            StreamWriter sw = new StreamWriter(HttpContext.Current.Server.MapPath("~/AppData/DebugLog.txt"), true);
            sw.WriteLine(DateTime.Now.ToString() + " Debugging sIllustration");
#endif
            // Create timeline chart
            // ---------------------
#region Gather Chart Data
            // Section A - Gather the data to be plotted
            // .........................................

            List<Series> serli = new List<Series>();
            foreach (enSeries s in Enum.GetValues(typeof(enSeries)))
            {
                serli.Add(new Series());
            }

            serli[(int)enSeries.EntireEquipment].ChartType = SeriesChartType.Line;
            serli[(int)enSeries.EntireEquipment].BorderWidth = 5;
            serli[(int)enSeries.EntireEquipment].Color = Color.Blue;
            serli[(int)enSeries.EntireEquipment].MarkerStyle = MarkerStyle.Diamond;
            serli[(int)enSeries.EntireEquipment].MarkerSize = 10;
            serli[(int)enSeries.EntireEquipment].MarkerColor = Color.Blue;
            serli[(int)enSeries.Component].ChartType = SeriesChartType.Line;
            serli[(int)enSeries.Component].BorderWidth = 5;
            serli[(int)enSeries.Component].Color = Color.CornflowerBlue;
            serli[(int)enSeries.Component].MarkerStyle = MarkerStyle.Triangle;
            serli[(int)enSeries.Component].MarkerSize = 10;
            serli[(int)enSeries.Component].MarkerColor = Color.CornflowerBlue;
            serli[(int)enSeries.ParameterSet].ChartType = SeriesChartType.Line;
            serli[(int)enSeries.AgingItem].ChartType = SeriesChartType.Line;
            serli[(int)enSeries.AgingItem].BorderWidth = 5;
            serli[(int)enSeries.AgingItem].Color = Color.Green;
            serli[(int)enSeries.AgingItem].MarkerStyle = MarkerStyle.Circle;
            serli[(int)enSeries.AgingItem].MarkerSize = 8;
            serli[(int)enSeries.AgingItem].MarkerColor = Color.Green;
            serli[(int)enSeries.ManualOpsDataReset].ChartType = SeriesChartType.Point;
            serli[(int)enSeries.ManualOpsDataReset].MarkerStyle = MarkerStyle.Square;
            serli[(int)enSeries.ManualOpsDataReset].MarkerSize = 6;
            serli[(int)enSeries.ManualOpsDataReset].MarkerColor = Color.Purple;
            serli[(int)enSeries.ManualOpsDataBegin].ChartType = SeriesChartType.Point;
            serli[(int)enSeries.ManualOpsDataBegin].MarkerStyle = MarkerStyle.Square;
            serli[(int)enSeries.ManualOpsDataBegin].MarkerSize = 6;
            serli[(int)enSeries.ManualOpsDataBegin].MarkerColor = Color.Purple;
            serli[(int)enSeries.ManualOpsDataEnd].ChartType = SeriesChartType.Point;
            serli[(int)enSeries.ManualOpsDataEnd].MarkerStyle = MarkerStyle.Square;
            serli[(int)enSeries.ManualOpsDataEnd].MarkerSize = 6;
            serli[(int)enSeries.ManualOpsDataEnd].MarkerColor = Color.Purple;
            serli[(int)enSeries.FlightOpsData].ChartType = SeriesChartType.Point;
            serli[(int)enSeries.FlightOpsData].MarkerStyle = MarkerStyle.Cross;
            serli[(int)enSeries.FlightOpsData].MarkerSize = 6;
            serli[(int)enSeries.FlightOpsData].MarkerColor = Color.LightBlue;
            serli[(int)enSeries.ActionItem].ChartType = SeriesChartType.Point;
            serli[(int)enSeries.ActionItem].MarkerStyle = MarkerStyle.Star5;
            serli[(int)enSeries.ActionItem].MarkerSize = 10;
            serli[(int)enSeries.ActionItem].MarkerColor = Color.Red;
            serli[(int)enSeries.LastAction].ChartType = SeriesChartType.Point;
            serli[(int)enSeries.LastAction].BorderWidth = 2;
            serli[(int)enSeries.LastAction].Color = Color.Green;
            serli[(int)enSeries.LastAction].MarkerStyle = MarkerStyle.Star5;
            serli[(int)enSeries.LastAction].MarkerSize = 8;
            serli[(int)enSeries.LastAction].MarkerColor = Color.Green;
            serli[(int)enSeries.OpsCalendar].ChartType = SeriesChartType.Line;
            serli[(int)enSeries.OpsCalendar].BorderWidth = 5;
            serli[(int)enSeries.OpsCalendar].Color = Color.Gray;
            serli[(int)enSeries.OpsCalendar].MarkerStyle = MarkerStyle.Cross;
            serli[(int)enSeries.OpsCalendar].MarkerSize = 10;
            serli[(int)enSeries.OpsCalendar].MarkerColor = Color.Gray;

            EquipmentDataContext dc = new EquipmentDataContext();
            EQUIPAGINGITEM eqg = (from g in dc.EQUIPAGINGITEMs where g.ID == ipAgingItem select g).First();
            int igEquipComponent = eqg.iEquipComponent;
            int igParameterSet = eqg.iParam;
            var eqp = (from p in dc.EQUIPAGINGPARs where p.ID == igParameterSet select new { p, p.EQUIPACTIONTYPE.sEquipActionType }).First();
            int igOpCalNames = eqg.iOpCal;
            foreach (enSeries s in Enum.GetValues(typeof(enSeries)))
            {
#if (IllustrationDEDBUG)
                sw.WriteLine("Starting Section A.01 - s = " + s.ToString());
#endif
                int iEquipComponent = igEquipComponent;
                EQUIPCOMPONENT eqc;
                switch (s)
                {
                    case enSeries.EntireEquipment:
                        do
                        {
                            eqc = (from c in dc.EQUIPCOMPONENTs where c.ID == iEquipComponent select c).First();
                            iEquipComponent = eqc.iParentComponent;
                        }
                        while (!eqc.bEntire);
                        DBEEntireComp.DBegin = eqc.DLinkBegin;
                        DBEEntireComp.DEnd = eqc.DLinkEnd;
                        saBoxedText[(int)s] += ": " + eqc.sComponent;
                        break;
                    case enSeries.Component:
                        bool bBegin = false; // Have we found the component's DLinkBegin?
                        bool bEnd = false; // Have we found the component's DLinkEnd?
                        int iLevel = 0;
                        do
                        {
#if (IllustrationDEDBUG)
                            sw.WriteLine("Starting Section A.01.Component: iEquipComponent = " + iEquipComponent.ToString() +
                                ", Global.DTO_EqAgEarliest = " + Global.DTO_EqAgEarliest.ToString() + ", Global.DTO_EqAgLatest = " + Global.DTO_EqAgLatest.ToString());
#endif
                            if (iEquipComponent < 1)
                            {
                                throw new Global.excToPopup("EqSupport.cs.sIllustration, Section A.01.Component Error: Trying to find a tuple in table EQUIPCOMPONENTS with ID = " +
                                    iEquipComponent.ToString() + " which is not > 0. Possible cause: Equipment components` DLinKBegin <= " + 
                                    CustFmt.sFmtDate(Global.DTO_EqAgEarliest, CustFmt.enDFmt.DateAndTimeMinOffset) +
                                    " and/or DLinkEnd >= " + CustFmt.sFmtDate(Global.DTO_EqAgLatest,CustFmt.enDFmt.DateAndTimeMinOffset));
                            }
                            eqc = (from c in dc.EQUIPCOMPONENTs where c.ID == iEquipComponent select c).First();
                            if (iLevel == 0)
                            {
                                saBoxedText[(int)s] += ": " + eqc.sComponent;
                            }
                            iLevel--;
#if (IllustrationDEDBUG)
                            sw.WriteLine("Starting Section A.02.Component: bEntire = " + eqc.bEntire.ToString() +
                                ", iParentComponent = " + eqc.iParentComponent.ToString() +
                                ", DLinkBegin = " + eqc.DLinkBegin.ToString() +
                                ", DLinkEnd = " + eqc.DLinkEnd.ToString() + Environment.NewLine +
                                "        bBegin = " + bBegin.ToString() + ", bEnd = " + bEnd.ToString() +
                                ", Global.DTO_EqAgEarliest = " + Global.DTO_EqAgEarliest.ToString() + 
                                ", Global.DTO_EqAgLatest = " + Global.DTO_EqAgLatest.ToString());
#endif
                            if (!bBegin)
                            {
                                if (eqc.DLinkBegin < Global.DTO_EqAgEarliest)
                                {
                                    // Use the parent component's DLinkBegin
                                    iEquipComponent = eqc.iParentComponent;
                                }
                                else
                                {
                                    bBegin = true;
                                    DBEComponent.DBegin = eqc.DLinkBegin;
                                }
                            }
                            if (!bEnd)
                            {
                                if (eqc.DLinkEnd > Global.DTO_EqAgLatest)
                                {
                                    // Use the parent component's DLinkEnd
                                    iEquipComponent = eqc.iParentComponent;
                                }
                                else
                                {
                                    bEnd = true;
                                    DBEComponent.DEnd = eqc.DLinkEnd;
                                }
                            }
                        } while (!bBegin || !bEnd);
#if (IllustrationDEDBUG)
                        sw.WriteLine("End of Section A.01.Component");
#endif
                        break;
                    case enSeries.ParameterSet:
                        sParSet = "Action Type: ";
                        sParSet += eqp.sEquipActionType;
                        string sComma = ", ";
                        baDrawArrow = baDrawArrowPattern;
                        if (eqp.p.iIntervalElapsed > -1)
                        {
                            sParSet += sComma + "iIntervalElapsed = " + eqp.p.iIntervalElapsed.ToString() + " " + eqp.p.sTimeUnitsElapsed +
                                ", cDeadLineMode = " + eqp.p.cDeadLineMode + ", iDeadLineSpt1 = " + eqp.p.iDeadLineSpt1.ToString() +
                                ", iDeadLineSpt2 = " + eqp.p.iDeadLineSpt2.ToString();
                        }
                        if (eqp.p.iIntervalOperating > -1)
                        {
                            sParSet += sComma + "iIntervalOperating = " + eqp.p.iIntervalOperating.ToString() + " " + eqp.p.sTimeUnitsOperating +
                                ", extrapolate: " + (eqg.bRunExtrap ? "Yes" : "No");
                            baDrawArrow[(int)enSeries.FlightOpsData] = eqg.bRunExtrap;
                            baDrawArrow[(int)enSeries.OpsCalendar] = eqg.bRunExtrap;
                        }
                        if (eqp.p.iIntervalCycles > -1)
                        {
                            sParSet += sComma + "iIntervalCycles = " + eqp.p.iIntervalCycles.ToString() + ", extrapolate: " + (eqg.bCyclExtrap ? "Yes" : "No");
                            baDrawArrow[(int)enSeries.FlightOpsData] = eqg.bCyclExtrap;
                            baDrawArrow[(int)enSeries.OpsCalendar] = eqg.bCyclExtrap;
                        }
                        if (eqp.p.iIntervalDistance > -1)
                        {
                            sParSet += sComma + "iIntervalDistance = " + eqp.p.iIntervalDistance.ToString() + " " + eqp.p.sDistanceUnits +
                                ", extrapolate: " + (eqg.bDistExtrap ? "Yes" : "No");
                            baDrawArrow[(int)enSeries.ManualOpsDataReset] = eqg.bDistExtrap;
                        }
                        if (!(eqp.p.sComment is null))
                        {
                            if (eqp.p.sComment.Length > 0)
                            {
                                sParSet += sComma + " Comment: " + eqp.p.sComment + ".";
                            }
                        }
                        saBoxedText[(int)s] += ": " + eqp.p.sShortDescript;
                        break;
                    case enSeries.OpsCalendar:
                        var eqo = (from o in dc.OPSCALNAMEs where o.ID == igOpCalNames select o).First();
                        saBoxedText[(int)s] += ": " + eqo.sOpsCalName;
                        DBeginEnd DBE = new DBeginEnd();
                        var eqt = from t in dc.OPSCALTIMEs where t.iOpsCal == igOpCalNames orderby t.DStart select t;
                        bool bStarted = false;
                        bool bEnded = false;
                        DateTimeOffset Dtemp = DateTimeOffset.MaxValue.AddYears(-1);
                        foreach (var t in eqt)
                        {
                            Dtemp = t.DStart;
                            if (t.bOpStatus)
                            {
                                if (!bStarted)
                                {
                                    DBE.DBegin = Dtemp;
                                    bStarted = true;
                                    bEnded = false;
                                }
                            }
                            else
                            {
                                if (bStarted)
                                {
                                    DBE.DEnd = Dtemp;
                                    liDBE_OpsCal.Add(DBE);
                                    DBE = new DBeginEnd();
                                    bStarted = false;
                                    bEnded = true;
                                }
                            }
                        }
                        if (bStarted)
                        {
                            if (!bEnded)
                            {
                                DBE.DEnd = Dtemp;
                            }
                            liDBE_OpsCal.Add(DBE);
                        }
                        break;
                    case enSeries.AgingItem:
                        saBoxedText[(int)s] += ": " + eqg.sName;
                        DBEAgingItem.DBegin = eqg.DStart;
                        DBEAgingItem.DEnd = eqg.DEnd;
                        break;
                    case enSeries.ManualOpsDataReset:
                    case enSeries.ManualOpsDataBegin:
                        if (eqp.p.iIntervalOperating > -1 || eqp.p.iIntervalCycles > -1 || eqp.p.iIntervalDistance > -1)
                        {
                            bool bContinue = true;
                            do
                            {
                                var eqco = (from co in dc.EQUIPCOMPONENTs where co.ID == iEquipComponent select co).First();
                                switch (s)
                                {
                                    case enSeries.ManualOpsDataReset:
                                        var eqm = from m in dc.EQUIPOPERDATAs where m.iEquipComponent == iEquipComponent && m.cSource == 'R' select m;
                                        foreach (var m in eqm)
                                        {
                                            liDBE_OpsDatReset.Add(new DBeginEnd(m.DFrom, m.DTo));
                                        }
                                        break;
                                    case enSeries.ManualOpsDataBegin:
                                        eqm = from m in dc.EQUIPOPERDATAs where m.iEquipComponent == iEquipComponent && m.cSource == 'M' select m;
                                        foreach (var m in eqm)
                                        {
                                            liDBE_OpsDatOngoing.Add(new DBeginEnd(m.DFrom, m.DTo));
                                        }
                                        break;
                                }
                                iEquipComponent = eqco.iParentComponent;
                                bContinue = !eqco.bEntire;
                            } while (bContinue);
                        }
                        break;
                    case enSeries.ManualOpsDataEnd: // no action required
                        break;
                    case enSeries.FlightOpsData:
                        if ((eqp.p.iIntervalOperating > -1 && eqg.bRunExtrap) || (eqp.p.iIntervalCycles > -1 && eqg.bCyclExtrap))
                        {
                            var eqf = from f in dc.OPDETAILs where f.iEquip == eqg.EQUIPCOMPONENT.iEquipment select f.OPERATION.DBegin;
                            foreach (var f in eqf)
                            {
                                liDFlightOps.Add(f.Value);
                            }
                        }
                        break;
                    case enSeries.ActionItem:
                        var eqa = from a in dc.EQUIPACTIONITEMs where a.iEquipAgingItem == ipAgingItem select a;
                        foreach (var a in eqa)
                        {
                            LiDeadLines.Add(a.DeadLine.Date);
                        }
                        break;
                    case enSeries.LastAction:
                        // no action required; just us DuLastAction
                        break;
                }
            }
#endregion
#if (IllustrationDEDBUG)
            sw.WriteLine("Starting Section B");
#endif
#region Chart Points
            // Section B - Use the above data to define the points in the chart series to be plotted
            // .....................................................................................

            int iVSep = 89; // vertical separation
            int iy = 0;
            int j = 0;
            int jMaxP1 = (int)Enum.GetValues(typeof(enSeries)).Cast<enSeries>().Max() + 1;
            MemoryStream memStr = new MemoryStream();
            using (Chart ChartA = new Chart())
            {
                ChartA.Width = 625;
                ChartA.Height = 650;
                ChartA.Series.Clear();
                DateTime DMidAxisX;

                foreach (enSeries s in Enum.GetValues(typeof(enSeries)))
                {
#if (IllustrationDEDBUG)
                    sw.WriteLine("Starting Section B.01 - s = " + s.ToString());
#endif
                    j = (int)s;
                    iy = iaVert[j] * iVSep - 57;
                    switch (s)
                    {
                        case enSeries.EntireEquipment:
                            serli[j].Points.AddXY(DBEEntireComp.DBegin.DateTime, iy);
                            serli[j].Points.AddXY(DBEEntireComp.DEnd.DateTime, iy);
                            ChartA.Series.Add(serli[j]);
                            break;
                        case enSeries.Component:
                            serli[j].Points.AddXY(DBEComponent.DBegin.DateTime, iy);
                            serli[j].Points.AddXY(DBEComponent.DEnd.DateTime, iy);
                            ChartA.Series.Add(serli[j]);
                            break;
                        case enSeries.AgingItem:
                            serli[j].Points.AddXY(DBEAgingItem.DBegin.DateTime, iy);
                            serli[j].Points.AddXY(DBEAgingItem.DEnd.DateTime, iy);
                            ChartA.Series.Add(serli[j]);
                            break;
                        case enSeries.ManualOpsDataReset:
                            iy += 25;
                            foreach (var d in liDBE_OpsDatReset)
                            {
                                serli[j].Points.AddXY(d.DBegin.DateTime, iy);
                                baDrawArrow[j] = true;
                            }
                            ChartA.Series.Add(serli[j]);
                            break;
                        case enSeries.ManualOpsDataBegin:
                            foreach (var d in liDBE_OpsDatOngoing)
                            {
                                serli[j].Points.AddXY(d.DBegin.DateTime, iy);
                                baDrawArrow[j] = true;
                            }
                            ChartA.Series.Add(serli[j]);
                            break;
                        case enSeries.ManualOpsDataEnd:
                            iy -= 25;
                            foreach (var d in liDBE_OpsDatOngoing)
                            {
                                serli[j].Points.AddXY(d.DEnd.DateTime, iy);
                                baDrawArrow[j] = true;
                            }
                            ChartA.Series.Add(serli[j]);
                            break;
                        case enSeries.FlightOpsData:
                            foreach (var D in liDFlightOps)
                            {
                                serli[j].Points.AddXY(D, iy);
                            }
                            ChartA.Series.Add(serli[j]);
                            break;
                        case enSeries.ActionItem:
                            foreach (var D in LiDeadLines)
                            {
                                serli[j].Points.AddXY(D, iy);
                            }
                            ChartA.Series.Add(serli[j]);
                            break;
                        case enSeries.LastAction:
                            serli[j].Points.AddXY(DuLastAction.DateTime, iy - 25);
                            ChartA.Series.Add(serli[j]);
                            break;
                        case enSeries.OpsCalendar:
                            int k = j;
                            foreach (var DBE in liDBE_OpsCal)
                            {
                                if (k > j)
                                {
                                    serli.Add(new Series());
                                    serli[k].ChartType = SeriesChartType.Line;
                                    serli[k].BorderWidth = 5;
                                    serli[k].Color = Color.Gray;
                                    serli[k].MarkerStyle = MarkerStyle.Cross;
                                    serli[k].MarkerSize = 10;
                                    serli[k].MarkerColor = Color.Gray;
                                    serli[k].XValueType = ChartValueType.Date;
                                }
                                serli[k].Points.AddXY(DBE.DBegin.DateTime, iy);
                                serli[k].Points.AddXY(DBE.DEnd.DateTime, iy);
                                ChartA.Series.Add(serli[k]);
                                k++;
                            }
                            break;
                    }
                }
                #endregion
#if (IllustrationDEDBUG)
                sw.WriteLine("Starting Section C");
#endif
                #region Save the Chart
                // Section C -Save the Chart
                // .........................
                ChartA.ChartAreas.Clear();
                ChartA.ChartAreas.Add(new ChartArea());
                ChartArea CA = ChartA.ChartAreas[0];
                CA.BackColor = Color.LightYellow;
                CA.AxisX.LabelStyle.Format = "yyyy-MM-dd";
                CA.AxisX.LabelStyle.Angle = -90;
                CA.AxisX.MajorGrid.LineColor = Color.DarkGray;
                CA.AxisX.MajorGrid.LineDashStyle = ChartDashStyle.Dash;
                CA.AxisX.MinorGrid.Enabled = true;
                CA.AxisX.MinorGrid.LineDashStyle = ChartDashStyle.Dot;
                CA.AxisX.MinorGrid.LineColor = Color.LightGray;
                DateTime Dt = liDBE_OpsCal.First().DBegin.DateTime.AddMonths(-1);
                Dt = new DateTime(Dt.Year, Dt.Month, 1, 0, 0, 0, DateTimeKind.Local);
                CA.AxisX.Minimum = Dt.ToOADate();

                Dt = liDBE_OpsCal.Last().DEnd.DateTime.AddMonths(2);
                Dt = new DateTime(Dt.Year, Dt.Month, 1, 23, 59, 59, DateTimeKind.Local).AddDays(-1);
                CA.AxisX.Maximum = Dt.ToOADate();

                CA.AxisY.Crossing = 0;
                CA.AxisY.Interval = 100;
                CA.AxisY.Maximum = 700;
                CA.AxisY.Minimum = 0;
                CA.AxisY.Enabled = AxisEnabled.False;

                if (ipAgingItem == 302)
                {
                    CA.AxisY.Minimum = 0;
                }
#if (IllustrationDEDBUG)
                sw.WriteLine("Starting Section C.02");
#endif
                TextAnnotation TAx = new TextAnnotation();
                DateTime Ddl = LiDeadLines.Last();
                TAx.Text = Ddl.ToString("yyyy-MM-dd");
                TAx.ForeColor = Color.Red;
                TAx.Font = new Font("Arial", 14, FontStyle.Bold, GraphicsUnit.Point);
                j = (int)enSeries.ActionItem;
                if (Ddl.ToOADate() < CA.AxisX.Minimum || Ddl.ToOADate() > CA.AxisX.Maximum)
                {
                    TAx.AxisX = CA.AxisX;
                    TAx.AxisY = CA.AxisY;
                    TAx.AnchorY = iaVert[(int)enSeries.ActionItem] * iVSep - 57;
                    if (Ddl.ToOADate() > CA.AxisX.Maximum)
                    {
                        TAx.AnchorX = CA.AxisX.Maximum;
                        TAx.AnchorAlignment = ContentAlignment.BottomRight;
                    }
                    else
                    {
                        TAx.AnchorX = CA.AxisX.Minimum;
                        TAx.AnchorAlignment = ContentAlignment.BottomLeft;
                    }
                }
                else
                {
                    TAx.SetAnchor(serli[j].Points[serli[j].Points.Count - 1]);
                    TimeSpan TS = liDBE_OpsCal.Last().DEnd - liDBE_OpsCal.First().DBegin;
                    DMidAxisX = liDBE_OpsCal.First().DBegin.Date.AddDays(TS.Days / 2.0);
                    if (LiDeadLines.Last() > DMidAxisX) TAx.AnchorAlignment = ContentAlignment.BottomRight; else TAx.AnchorAlignment = ContentAlignment.BottomLeft;
                }
                ChartA.Annotations.Add(TAx);

                TextAnnotation TAn = new TextAnnotation();
                TAn.Text = "Last Action: " + CustFmt.sFmtDate(DuLastAction,CustFmt.enDFmt.DateAndTimeMinOffset);
                Ddl = DuLastAction.DateTime;
                TAn.ForeColor = Color.Green;
                TAn.Font = new Font("Arial", 9, FontStyle.Bold, GraphicsUnit.Point);
                j = (int)enSeries.LastAction;
                if (Ddl.ToOADate() < CA.AxisX.Minimum || Ddl.ToOADate() > CA.AxisX.Maximum)
                {
                    TAn.AxisX = CA.AxisX;
                    TAn.AxisY = CA.AxisY;
                    TAn.AnchorY = iaVert[j] * iVSep - 100;
                    if (Ddl.ToOADate() > CA.AxisX.Maximum)
                    {
                        TAn.AnchorX = CA.AxisX.Maximum;
                        TAn.AnchorAlignment = ContentAlignment.BottomRight;
                    }
                    else
                    {
                        TAn.AnchorX = CA.AxisX.Minimum;
                        TAn.AnchorAlignment = ContentAlignment.BottomLeft;
                    }
                }
                else
                {
                    TAn.SetAnchor(serli[j].Points[serli[j].Points.Count - 1]);
                    TimeSpan TS = liDBE_OpsCal.Last().DEnd - liDBE_OpsCal.First().DBegin;
                    DMidAxisX = liDBE_OpsCal.First().DBegin.Date.AddDays(TS.Days / 2.0);
                    if (LiDeadLines.Last() > DMidAxisX) TAn.AnchorAlignment = ContentAlignment.MiddleRight; else TAn.AnchorAlignment = ContentAlignment.MiddleLeft;
                }
                ChartA.Annotations.Add(TAn);

                ChartA.SaveImage(memStr, ChartImageFormat.Bmp);
                ChartA.Dispose();
            }
#endregion
#if (IllustrationDEDBUG)
            sw.WriteLine("Starting Section D");
#endif
#region DataFlow Chart
            // Create the Data Flow Chart
            // --------------------------
            // Text Colors:
            Brush[] aBrush = new Brush[]
                {new SolidBrush(Color.Blue), // Entire Equipment
            new SolidBrush(Color.CornflowerBlue), // Component
            new SolidBrush(Color.OrangeRed), // Parameter Set
            new SolidBrush(Color.Green), // AgingItem
            new SolidBrush(Color.Maroon), //Manual Ops Data Reset
            new SolidBrush(Color.Maroon), //Manual Ops Data Begin
            new SolidBrush(Color.Maroon), //Manual Ops Data End
            new SolidBrush(Color.Blue), // Flight Ops Data
            new SolidBrush(Color.Red), // Action Item
            new SolidBrush(Color.Green), // Date of last action
            new SolidBrush(Color.Black)}; // Ops Calendar
                                            // Rectangle Fill Colors:
            SolidBrush[] aSolBrLight = new SolidBrush[]
                {new SolidBrush(Color.LightBlue), // Entire Equipment
                new SolidBrush(Color.LightBlue), // Component
                new SolidBrush(Color.FromArgb(255,230,230)), // Parameter Set
                new SolidBrush(Color.LightGreen), // AgingItem
                new SolidBrush(Color.LightCoral), //Manual Ops Data Reset
                new SolidBrush(Color.LightCoral), //Manual Ops Data Begin
                new SolidBrush(Color.LightCoral), //Manual Ops Data End
                new SolidBrush(Color.LightBlue), // Flight Ops Data
                new SolidBrush(Color.FromArgb(255,230,230)), // Action Item
                new SolidBrush(Color.LightGreen), // Date of Last Action
                new SolidBrush(Color.LightGray)}; // Ops Calendar
            int[] iaXOffset = new int[] { 10, 10, 10, 100, 10, 10, 10, 10, 150, 10, 10 }; // units: pixels
            int[] iaYOffset = new int[] { 2, 3, 4, 6, 7, -1, -1, 8, 9, -1, 5 }; // units: iVSep*pixels
            int iRWidth = 110;
            int[] iaArrOffset = new int[11];
            iaArrOffset[0] = 0;
            iaArrOffset[1] = (int)(0.6 * iRWidth);
            iaArrOffset[2] = (int)(0.4 * iRWidth);
            iaArrOffset[3] = (int)(0.2 * iRWidth);
            iaArrOffset[4] = (int)(0.75 * iRWidth);
            iaArrOffset[5] = (int)(0.8 * iRWidth);
            iaArrOffset[6] = (int)(0.8 * iRWidth);
            iaArrOffset[7] = (int)(0.48 * iRWidth);
            iaArrOffset[8] = (int)(0.4 * iRWidth);
            iaArrOffset[9] = (int)(0.4 * iRWidth);
            iaArrOffset[10] = (int)(0.2 * iRWidth);
            iVSep = 66; // vertical separation

            Graphics graf = Graphics.FromImage(pBitm);
            graf.Clear(Color.White);

            Image chartImg = Image.FromStream(memStr);
            memStr.Dispose();
            float x = 276.0F;
            float y = iVSep - 50;
            Rectangle chartRect = new Rectangle(0, 0, 625, 650);
            graf.DrawImage(chartImg, x, y, chartRect, GraphicsUnit.Pixel);

            Pen penDBlue = new Pen(Color.DarkBlue, 3);
            Pen pen = new Pen(Color.Black, 2);
            RectangleF rectF;
            using (Font font1 = new Font("Calibri", 12, FontStyle.Regular, GraphicsUnit.Pixel))
            {
                Pen penArr = new Pen(Color.Gray, 5);
                penArr.EndCap = LineCap.ArrowAnchor;
                Point[] aPts = new Point[3];
                iy = 0;
                int ix = 0;
                int iTopAgingItem = 6 * iVSep - 83;
                int iTopActionItem = 9 * iVSep - 78;
                int iTop = 0;
                int ixDateBegin = 280;
                int ixDateEnd = 830;
                string sDateFormat = "yyyy-MM-dd";
                foreach (enSeries s in Enum.GetValues(typeof(enSeries)))
                {
#if (IllustrationDEDBUG)
                    sw.WriteLine("Starting Section D.03 - s = " + s.ToString());
#endif
                    int i = (int)s;
                    if (iaYOffset[i] > -1)
                    {
                        // Boxes and Text
                        iy = iaYOffset[i] * iVSep - 33;
                        if (i == 3)
                        {
                            rectF = new RectangleF(iaXOffset[i] - 50, iy - 50, iRWidth + 50, 50);
                        }
                        else
                        {
                            rectF = new RectangleF(iaXOffset[i], (i == 8) ? iy - 45 : iy - 50, iRWidth, 50);
                        }
                        graf.FillRectangle(aSolBrLight[i], rectF);
                        graf.DrawString(saBoxedText[i], font1, aBrush[i], rectF);
                        graf.DrawRectangle(pen, Rectangle.Round(rectF));
                        switch (s)
                        {
                            case enSeries.EntireEquipment:
                                graf.DrawString(DBEEntireComp.DBegin.Date.ToString(sDateFormat), font1, aBrush[i], new PointF(ixDateBegin, iy - 45));
                                graf.DrawString(DBEEntireComp.DEnd.Date.ToString(sDateFormat), font1, aBrush[i], new PointF(ixDateEnd, iy - 50));
                                break;
                            case enSeries.Component:
                                graf.DrawString(DBEComponent.DBegin.Date.ToString(sDateFormat), font1, aBrush[i], new PointF(ixDateBegin, iy - 45));
                                graf.DrawString(DBEComponent.DEnd.Date.ToString(sDateFormat), font1, aBrush[i], new PointF(ixDateEnd, iy - 50));
                                break;
                            case enSeries.ParameterSet:
                                Rectangle rectPar = new Rectangle(ixDateBegin, iy - 50, 550, 50);
                                graf.DrawString(sParSet, new Font("Calibri", 14, FontStyle.Regular, GraphicsUnit.Pixel), Brushes.Red, rectPar);
                                break;
                            case enSeries.AgingItem:
                                graf.DrawString(DBEAgingItem.DBegin.Date.ToString(sDateFormat), font1, aBrush[i], new PointF(ixDateBegin, iy - 45));
                                graf.DrawString(DBEAgingItem.DEnd.Date.ToString(sDateFormat), font1, aBrush[i], new PointF(ixDateEnd, iy - 50));
                                break;
                        }

                        // Flowchart arrows
                        if (baDrawArrow[i])
                        {
                            if (i == 0)
                            {
                                ix = iaXOffset[1] + iRWidth / 2;
                                graf.DrawLine(penArr, ix, iy, ix, iy + 16);
                            }
                            else
                            {
                                ix = iaXOffset[i] + iRWidth;
                                aPts[0] = new Point(ix, iy - 25);
                                aPts[1] = new Point(ix + iaArrOffset[i], iy - 25);
                                if (i < 3 || i == 10)
                                {
                                    iTop = iTopAgingItem;
                                }
                                else
                                {
                                    iTop = iTopActionItem;
                                }
                                if (i != 8 && i != 9)
                                {
                                    aPts[2] = new Point(ix + iaArrOffset[i], iTop);
                                    graf.DrawLines(penArr, aPts);
                                }
                            }
                        }
                    }
                }
            }
#endregion
#if (IllustrationDEDBUG)
            sw.WriteLine("Starting Section E");
#endif
            // Title
            graf.DrawString("Equipment Maintenance/Aging: Influences Upon an Action Item",
                new Font("Calibri", 24, FontStyle.Bold, GraphicsUnit.Pixel), Brushes.DarkBlue, 130, 4);
            DateTime DNow = DateTime.Now;

            graf.DrawString("Generated by TSoar on " + DNow.ToString("yyyy-MM-dd") + " at " + DNow.ToString("HH:mm:ss"),
                new Font("Calibri", 14, FontStyle.Regular, GraphicsUnit.Pixel), Brushes.DarkBlue, 10, 615);
            graf.DrawRectangle(penDBlue, 1, 1, pBitm.Width - 3, pBitm.Height - 3);

            string sFile = "~/AppData/ActionItems/" + Guid.NewGuid().ToString("N") + ".jpg";
            string sPath = System.Web.Hosting.HostingEnvironment.MapPath(sFile);
            pBitm.Save(sPath, ImageFormat.Jpeg);
            pen.Dispose();
            graf.Dispose();
            pBitm.Dispose();
#if (IllustrationDEDBUG)
            sw.WriteLine("End of debug log");
#endif

            return sFile;
        }
    }
}
