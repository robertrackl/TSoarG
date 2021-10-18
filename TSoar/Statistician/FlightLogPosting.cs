using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Linq;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace TSoar.Statistician
{
    /* Data in tables DAILYFLIGHTLOGS and FLIGHTLOGROWS is to be transferred to tables OPERATIONS, OPDETAILS, AVIATORS and EQUIPMENT
     * We do this in three phases: Flight Operation, Equipment, Aviators.
     * Phase "Flight Operations":
     *      Table OPERATIONS gets these data:
     *              Launch Method
     *              Takeoff and Landing Locations
     *              Begin and End Times
     *              Charge Code
     *              Comments
     * Phase "Equipment":
     *      Table OPDETAILS gets these data for each piece of equipment used in the operation:
     *              Pointer to the operation
     *              Pointer to the equipment
     *              Pointer to the equipment role
     *              Max altitude
     *              Release altitude
     * Phase "Aviators":
     *      Table AVIATORS gets these data:
     *              Pointer to a row in OPDETAILS (which determines the equipment used by this aviator)
     *              Pointer to a row in table PEOPLE for identifying the aviator
     *              Aviator role
     *              Percent charge
     *              whether this was a first flight of the season with an instructor */

    /* Software organization:
     *  a class is defined under namespace TSoar.Statistician which provides this method:
     *       a public routine Post2OPERATIONS() for posting a single flight operation:
     *              writes to Table OPERATIONS
     *              writes to table OPDETAILS for each piece of equipment used in the flight operation
     *              writes to table AVIATORS as many times as there were aviators */

    public class FlightLogPosting
    {
        TSoar.DB.SCUD_Multi mCRUD = new TSoar.DB.SCUD_Multi();

        public string sPost2OPERATIONS(StatistDailyFlightLogDataContext ustdc, int iuRow)
        {
            // A call to this method should be within a transaction so that all database updates
            //   in this routine either succeed together of fail together.

            // Post one flight operation to table OPERATIONS using data in a row of table FLIGHTLOGROWS
            string sReturn = "Problem";
            var qflr = from r in ustdc.FLIGHTLOGROWs where r.ID == iuRow select r;
            int iUser = mCRUD.GetPeopleIDfromWebSiteUserName(HttpContext.Current.User.Identity.Name);
            int iOPER = 0;
            int iLaunchMethod = 0;
            int iOpDetailTow = 0;
            int iOpDetailGlider = 0;
            foreach (var r in qflr) // There is just one item in qflr, but we have to use foreach to get at it.
            {
                // Does an operation like this already exist in table OPERATIONS?
                var ae = from o in ustdc.OPERATIONs
                         where (bool)ustdc.bTNPF_CheckDupOp(r.iLaunchMethod, r.iLocTakeOff, o.DBegin, r.DTakeOff.DateTime)
                         select new { o.DBegin, r.DTakeOff, o.LAUNCHMETHOD.sLaunchMethod, o.LOCATION.sLocation };
                foreach (var b in ae)
                {
                    string ss = "An operation with Launch Method " + b.sLaunchMethod + " at Location " + b.sLocation + " beginning at " + b.DBegin.ToString() +
                        " exists already in table OPERATIONS. The operation to be posted has a takeoff time of " + b.DTakeOff.ToString() +
                        ", i.e., within one minute of the existing operation. Operation not posted.";
                    return ss;
                }
                try
                {
                    OPERATION flop = new OPERATION();
                    flop.PiTRecordEntered = DateTime.UtcNow;
                    flop.iRecordEnteredBy = iUser;
                    iLaunchMethod = r.iLaunchMethod;
                    flop.iLaunchMethod = iLaunchMethod;
                    flop.iTakeoffLoc = r.iLocTakeOff;
                    flop.DBegin = r.DTakeOff.DateTime;
                    flop.iLandingLoc = r.iLocLanding;
                    flop.DEnd = r.DLanding.DateTime;
                    flop.sComment = r.sComments;
                    flop.iChargeCode = r.iChargeCode;
                    flop.iInvoices2go = -1;
                    ustdc.OPERATIONs.InsertOnSubmit(flop);
                    ustdc.SubmitChanges();
                    iOPER = flop.ID;
                }
                catch (Exception exc)
                {
                    throw new Global.excToPopup("Problem in FlightLogPosting.Post2OPERATIONS writing to table OPERATIONS: " + exc.Message);
                }

                // Post flight operation details to table OPDETAILS using data in this row of table FLIGHTLOGROWS
                try
                {
                    //   The tow
                    OPDETAIL flod = new OPDETAIL();
                    flod.PiTRecordEntered = DateTime.UtcNow;
                    flod.iRecordEnteredBy = iUser;
                    flod.iOperation = iOPER;
                    //      Determine the equipment role from the launch method
                    var qe = (from b in ustdc.BRIDGE_LAUNCHMETH_EQUIPROLEs where b.iLaunchMethod == iLaunchMethod select b.EQUIPMENTROLE.ID).ToList();
                    if (qe.Count < 1)
                    {
                        throw new Global.excToPopup("Error in FlightLogPosting.Post2OPERATIONS: iLaunchMethod=" + iLaunchMethod.ToString() +
                            " does not have a related equipment role through table BRIDGE_LAUNCHMETH_EQUIPROLE.");
                    }
                    if (qe.Count > 1)
                    {
                        throw new Global.excToPopup("Error in FlightLogPosting.Post2OPERATIONS: iLaunchMethod=" + iLaunchMethod.ToString() +
                            " has more than one related equipment role through table BRIDGE_LAUNCHMETH_EQUIPROLE.");
                    }
                    flod.iEquipmentRole = qe.First();
                    flod.iEquip = r.iTowEquip; ;
                    flod.dMaxAltitude = r.dReleaseAltitude;
                    flod.dReleaseAltitude = r.dReleaseAltitude;
                    ustdc.OPDETAILs.InsertOnSubmit(flod);
                    ustdc.SubmitChanges();
                    iOpDetailTow = flod.ID;
                }
                catch (Exception exc)
                {
                    throw new Global.excToPopup("Problem in FlightLogPosting.Post2OPERATIONS writing the tow details to table OPDETAILS: " + exc.Message);
                }

                try
                {
                    //   The Glider
                    OPDETAIL flod = new OPDETAIL();
                    flod.PiTRecordEntered = DateTime.UtcNow;
                    flod.iRecordEnteredBy = iUser;
                    flod.iOperation = iOPER;
                    var qe = (from e in ustdc.EQUIPROLESTYPEs
                              where e.EQUIPMENTROLE.sEquipmentRole.Contains("Glider") &&
                                    e.iEquipType == e.EQUIPTYPE.ID &&
                                    e.iEquipRole == e.EQUIPMENTROLE.ID &&
                                    e.EQUIPTYPE.ID == (from u in ustdc.EQUIPMENTs where u.ID == r.iGlider select u.iEquipType).First()
                              select e.EQUIPMENTROLE.ID).ToList();
                    if (qe.Count < 1)
                    {
                        throw new Global.excToPopup("Error in FlightLogPosting.Post2OPERATIONS: Could not find an equipment role that contains 'Glider' " +
                            "and is linked to equipment type " + (from u in ustdc.EQUIPMENTs where u.ID==r.iGlider select u.EQUIPTYPE.sEquipmentType).First());
                    }
                    if (qe.Count > 1)
                    {
                        throw new Global.excToPopup("Error in FlightLogPosting.Post2OPERATIONS: There are more than one equipment roles that contain 'Glider' " +
                            "and are linked to equipment type " + (from u in ustdc.EQUIPMENTs where u.ID == r.iGlider select u.EQUIPTYPE.sEquipmentType).First());
                    }
                    flod.iEquipmentRole = qe.First();
                    flod.iEquip = r.iGlider;
                    flod.dReleaseAltitude = r.dReleaseAltitude;
                    flod.dMaxAltitude = r.dMaxAltitude;
                    ustdc.OPDETAILs.InsertOnSubmit(flod);
                    ustdc.SubmitChanges();
                    iOpDetailGlider = flod.ID;
                }
                catch (Exception exc)
                {
                    throw new Global.excToPopup("Problem in FlightLogPosting.Post2OPERATIONS writing the glider flight details to table OPDETAILS: " + exc.Message);
                }

                // Post flight operation details to table AVIATORS using data in this row of table FLIGHTLOGROWS
                try
                {
                    //   The Tow
                    //      We record here only one tow pilot. If there is another one then he has to be added in OpsDataInput.aspx using the TreeView approach
                    AVIATOR flav = new AVIATOR();
                    flav.PiTRecordEntered = DateTime.UtcNow;
                    flav.iRecordEnteredBy = iUser;
                    flav.iPerson = r.iTowOperator;
                    flav.iOpDetail = iOpDetailTow;
                    var qe = (from o in ustdc.AVIATORROLEs where o.sAviatorRole.Contains("Tow") && o.sAviatorRole.Contains("Pilot") select o.ID).ToList();
                    if (qe.Count < 1)
                    {
                        throw new Global.excToPopup("Error in FlightLogPosting.Post2OPERATIONS: Could not find an aviator role that contains 'Tow' and 'Pilot'.");
                    }
                    if (qe.Count > 1)
                    {
                        throw new Global.excToPopup("Error in FlightLogPosting.Post2OPERATIONS: There are more than one aviator roles that contain 'Tow' and 'Pilot'.");
                    }
                    flav.iAviatorRole = qe.First();
                    flav.dPercentCharge = 0.00m;
                    flav.b1stFlight = false;
                    flav.mInvoiced = 0.00m;
                    flav.DInvoiced = DateTimeOffset.MinValue;
                    ustdc.AVIATORs.InsertOnSubmit(flav);
                    ustdc.SubmitChanges();
                }
                catch (Exception exc)
                {
                    throw new Global.excToPopup("Problem in FlightLogPosting.Post2OPERATIONS writing the tow operator data to table AVIATORS: " + exc.Message);
                }
                try
                {
                    //   Glider Pilot 1
                    AVIATOR flav = new AVIATOR();
                    flav.PiTRecordEntered = DateTime.UtcNow;
                    flav.iRecordEnteredBy = iUser;
                    flav.iPerson = r.iPilot1;
                    flav.iOpDetail = iOpDetailGlider;
                    flav.iAviatorRole = r.iAviatorRole1;
                    flav.dPercentCharge = r.dPctCharge1;
                    flav.b1stFlight = false; // exceptions to be handled in OpsDataInput.aspx
                    flav.mInvoiced = 0.00m;
                    flav.DInvoiced = DateTimeOffset.MinValue;
                    ustdc.AVIATORs.InsertOnSubmit(flav);
                    ustdc.SubmitChanges();
                }
                catch (Exception exc)
                {
                    throw new Global.excToPopup("Problem in FlightLogPosting.Post2OPERATIONS writing the details of glider pilot1 to table AVIATORS: " + exc.Message);
                }
                try { 
                    //   Glider Pilot 2
                    var qs = (from p in ustdc.PEOPLEs where p.ID == r.iPilot2 select p.sDisplayName).First();
                    if (qs != "[none]") // is there a second pilot?
                    {
                        AVIATOR flav = new AVIATOR();
                        flav.PiTRecordEntered = DateTime.UtcNow;
                        flav.iRecordEnteredBy = iUser;
                        flav.iPerson = r.iPilot2;
                        flav.iOpDetail = iOpDetailGlider;
                        flav.iAviatorRole = r.iAviatorRole2;
                        flav.dPercentCharge = r.dPctCharge2;
                        flav.b1stFlight = false; // exceptions to be handled in OpsDataInput.aspx
                        flav.mInvoiced = 0.00m;
                        flav.DInvoiced = DateTimeOffset.MinValue;
                        ustdc.AVIATORs.InsertOnSubmit(flav);
                        ustdc.SubmitChanges();
                    }
                    sReturn = "OK";
                }
                catch (Exception exc)
                {
                    throw new Global.excToPopup("Problem in FlightLogPosting.Post2OPERATIONS writing the details of glider pilot2 to table AVIATORS: " + exc.Message);
                }
            }
            return sReturn;
        }
    }
}