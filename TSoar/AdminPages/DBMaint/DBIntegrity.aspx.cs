using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.Security;
using System.Web.UI.WebControls;

namespace TSoar.AdminPages.DBMaint
{
    public partial class DBIntegrity : System.Web.UI.Page
    {
        private Dictionary<string, bool> dictReqS = new Dictionary<string, bool>();
        public DBIntegrity() 
        {
            dictReqS.Add("DefaultQualification", false); //0
            dictReqS.Add("DefaultClubMembershipType", false); //1
            dictReqS.Add("PageSizeMembersFromTo", false); //2
            dictReqS.Add("PageSizeMembers", false); //3
            dictReqS.Add("PageSizeSettings", false); //4
            dictReqS.Add("DefaultRating", false); //5
            dictReqS.Add("PageSizeActivityLog", false); //6
            dictReqS.Add("DefaultEquipmentType", false); //7
            dictReqS.Add("DefaultLaunchMethod", false); //8
            dictReqS.Add("DefaultLocation", false); //9
            dictReqS.Add("DefaultEquipment", false); //10
            dictReqS.Add("DefaultEquipmentRole", false); //11
            dictReqS.Add("DefaultAviator", false); //12
            dictReqS.Add("DefaultAviatorRole", false); //13
            dictReqS.Add("PageSizeTrackFlyingCharges", false); //14
            dictReqS.Add("PageSizeContacts", false); //15
            dictReqS.Add("DefaultCertification", false); //16
            dictReqS.Add("PageSizeCoA", false); //17
            dictReqS.Add("DefaultAccountType", false); //18
            dictReqS.Add("DefaultParentAccountCode", false); //19
            dictReqS.Add("DefaultSubLedgerName", false); //20
            dictReqS.Add("PageSizeSubledger", false); //21
            dictReqS.Add("PageSizeFinInstitutions", false); //22
            dictReqS.Add("PageSizeBankAcctTypes", false); //23
            dictReqS.Add("PageSizeBankAccts", false); //24
            dictReqS.Add("DefaultFinancialInstitution", false); //25
            dictReqS.Add("DefaultBankAccountType", false); //26
            dictReqS.Add("DefaultAssociatedAccountCode", false); //27
            dictReqS.Add("PageSizeAuditTrail", false); //28
            dictReqS.Add("DefaultExpenseAccount", false); //29
            dictReqS.Add("DefaultAssetsAccount", false); //30
            dictReqS.Add("DefaultPaymentMethod", false); //31
            dictReqS.Add("DefaultAttachmentCateg", false); //32
            dictReqS.Add("TimeZoneOffset", false); //33
            dictReqS.Add("DefaultFA_item_code", false); //34
            dictReqS.Add("DefaultFA_PaymentTerm", false); //35
            dictReqS.Add("DefaultQBO_AccItem", false); //36
            dictReqS.Add("DefaultMembCategMFC", false); //37
            dictReqS.Add("DefaultMinimumMonthlyFlyingCharge", false); //38
            dictReqS.Add("PageSizeOfficesFromTo", false); //39
            dictReqS.Add("PageSizeTelephRoster", false); //40
            dictReqS.Add("PageSizeEmailRoster", false); //41
            dictReqS.Add("PageSizePhysicalAddressRoster", false); //42
            dictReqS.Add("PageSizeQualifRoster", false); //43
            dictReqS.Add("PageSizeEquipmentList", false); //44
            dictReqS.Add("DefaultTowEquipment", false); //45
            dictReqS.Add("DefaultTowOperator", false); //46
            dictReqS.Add("DefaultGlider", false); //47
            dictReqS.Add("DefaultGliderPilotRole", false); //48
            dictReqS.Add("DefaultReleaseAltitude", false); //49
            dictReqS.Add("DefaultChargeCode", false); //50
            dictReqS.Add("AdjectiveToWebSiteOnHomePage", false); //51
            dictReqS.Add("DefaultPilot2", false); //52
            dictReqS.Add("DefaultAviatorRolePilot2", false); //53
            dictReqS.Add("PageSizePeopleEqRolesTypes", false); //54
            dictReqS.Add("PageSizeMinFlyChrgEdit", false); //55
            dictReqS.Add("DefaultSpecialOperationType", false); //56
            dictReqS.Add("DefaultSSAMembershipType", false); //57
            dictReqS.Add("Version", false); //58
            dictReqS.Add("PageSizeEquipComponents", false); //59
            dictReqS.Add("PageSizeEquipMaintStatus", false); //60
            dictReqS.Add("OpsScheduleSignUpSite", false); //61
            dictReqS.Add("PageSizeCMSQualif", false); //62
            dictReqS.Add("PageSizeCMSCertifics", false); //63
            dictReqS.Add("PageSizeCMSRatings", false); //64
            dictReqS.Add("PageSizeEquipActionItems", false); //65 
            dictReqS.Add("PageSizeEquipAgingItems", false); //66
            dictReqS.Add("PageSizeOpsSchedule", false); //67
            dictReqS.Add("PageSizeOpsScheduleSignup", false); //68 // SCR 221
            dictReqS.Add("PageSizeClubStats", false); //69 // SCR 216
            dictReqS.Add("PageSizeEquipStatus", false); //70 // SCR 230
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
        private void MPE_Show(Global.enumButtons eubtns, string suMode)
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
            switch (suMode)
            {
                case "Settings":
                    txbPopup.Height = 700;
                    txbPopup.CssClass = "rightAlign";
                    break;
                case "Zero":
                    txbPopup.Height = 300;
                    txbPopup.CssClass = "";
                    break;
                case "Exception":
                    txbPopup.Height = 100;
                    txbPopup.CssClass = "";
                    break;
                default:
                    break;
            }
            ModalPopExt.Show();
        }
        protected void Button_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            try
            {
                if ((btn.ID == "YesButton") && (btn.CommandName == "Delete"))
                {
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
            txbPopup.Text = excu.sExcMsg();
            MPE_Show(Global.enumButtons.OkOnly, "Exception");
        }
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        { }

        protected void pbSettings_Click(object sender, EventArgs e)
        {
            ButtonsClear();
            txbPopup.Text = "LIST OF COMPLETED CHECKS" + Environment.NewLine + "{Success: when all checks score 1 hit}" + Environment.NewLine;
            DataIntegrityDataContext dc = new DataIntegrityDataContext();
            var qt = (from s in dc.SETTINGs select s).ToList();
            // Check for number of settings:
            int it = qt.Count();
            if (it != dictReqS.Count())
            {
                ProcessPopupException(new Global.excToPopup(Server.MapPath(".") + ": the number of settings in database table SETTINGS = "
                    + it.ToString() + " does not equal " + dictReqS.Count().ToString() + ", the number expected by this checking software."));
                //return;
            }
            // Reset the values in the entire dictionary
            List<string> saKeys = new List<string>(dictReqS.Keys);
            foreach(string skey in saKeys)
            {
                dictReqS[skey] = false;
            }

            bool bFound = false;
            foreach (var s in qt)
            {
                bFound = false;
                foreach (var d in dictReqS)
                {
                    //ActivityLog.oDiag("DBIntegrity", 10, s.sSettingName + ":-|-:" + d.Key); // SCR 230
                    if (d.Key == s.sSettingName)
                    {
                        dictReqS[s.sSettingName] = true;
                        bFound = true;
                        break;
                    }
                }
                if (!bFound) {
                    string sL = String.Join(", ", dictReqS.Select(kvp => String.Format("{0}", kvp.Key)));
                    ProcessPopupException(new Global.excToPopup(Server.MapPath(".") + ": database table SETTINGS has setting `" + s.sSettingName +
                        "` which was not found in the list of required setting names: " + sL));
                    return;
                }
            }
            bFound = true;
            string sMiss = "";
            foreach(var d in dictReqS)
            {
                if (!d.Value)
                {
                    bFound = false;
                    sMiss = d.Key;
                    break;
                }
            }
            if (!bFound)
            {
                ProcessPopupException(new Global.excToPopup(Server.MapPath(".") + ": database table SETTINGS is missing a required setting named `" + sMiss + "`"));
                return;
            }

            foreach (var s in qt)
            {
                if (s.sInTable != null && s.sInTable.Length > 0)
                {
                    string[] saIT = s.sInTable.Split(',');
                    foreach(string sIT in saIT)
                    {
                        string[] saTF = sIT.Split('.');
                        int? iHits = 0;
                        string sMsg = "";
                        try
                        {
                            dc.spSettingsCheck(saTF[0], saTF[1], s.sSettingValue, ref iHits, ref sMsg);
                        }
                        catch (Exception exc)
                        {
                            ProcessPopupException(new Global.excToPopup(exc.Message));
                        }
                        if (iHits < 0)
                        {
                            txbPopup.Text += Environment.NewLine + "From spSettingsCheck: " + sMsg;
                        }
                        txbPopup.Text += Environment.NewLine + s.sSettingName + " '" + s.sSettingValue + "' in " + sIT + " - " + iHits.ToString() + " hit" + ((iHits == 1)? "" : "s");
                    }
                }
            }
            MPE_Show(Global.enumButtons.OkOnly, "Settings");
        }

        protected void pbRoles_Click(object sender, EventArgs e)
        {
            int iNumRoles = Global.sagRoles.Count();
            string[] saDefRoles = Roles.GetAllRoles(); // List of roles defined by the administrator
            int iNumDefRoles = saDefRoles.Count();
            if (iNumRoles != iNumDefRoles)
            {
                ProcessPopupException(new Global.excToPopup("The number of required website security roles = " + iNumRoles.ToString() +
                    " does not equal " + iNumDefRoles + " = the number of roles defined by the website administrator"));
                return;
            }
            string[] saeng = Enum.GetNames(typeof(Global.engRoles));
            if (iNumRoles != saeng.Count())
            {
                ProcessPopupException(new Global.excToPopup("The number of required website security roles = " + iNumRoles.ToString() +
                    " does not equal " + saeng.Count().ToString() + " = the number of roles defined in enumeration Global.engRoles"));
                return;
            }
            int i = 0;
            foreach (string srole in Global.sagRoles)
            {
                if (saeng[i] != srole)
                {
                    ProcessPopupException(new Global.excToPopup("The " + i.ToString() + "-th Website security role `" + srole + 
                        "` in array Global.SagRoles does not equal the " + i.ToString() + "-th entry in enum Global.engRoles `" + saeng[i] + "`." ));
                    return;
                }
                if (!Roles.RoleExists(srole))
                {
                    ProcessPopupException(new Global.excToPopup("Website security role `" + srole + "` does not exist in the list of security roles"));
                    return;
                }
                i++;
            }
            ProcessPopupException(new Global.excToPopup("Database integrity checking for website security roles has been completed."));
        }

        protected void pbEquip_Click(object sender, EventArgs e)
        {
            DataIntegrityDataContext dc = new DataIntegrityDataContext();

            #region In table EQUIPOPERDATA, From date must be earlier or equal to To date
            var qo = from o in dc.EQUIPOPERDATAs select o;
            foreach (var o in qo)
            {
                if (o.DFrom > o.DTo)
                {
                    ProcessPopupException(new Global.excToPopup("Table EQUIPOPERDATA has a row with ID = " + o.ID.ToString() + " where DFrom = " + o.DFrom + " is later than Dto = " + o.DTo + " for component " + o.EQUIPCOMPONENT.sComponent));
                    return;
                }
            }
            #endregion

            #region In table EQUIPOPERDATA, Reset records must have both cSource='R' and From and To dates the same
            var q7 = from o in dc.EQUIPOPERDATAs where o.cSource == 'R' && o.DFrom != o.DTo select o;
            foreach (var o in q7)
            {
                ProcessPopupException(new Global.excToPopup("Table EQUIPOPERDATA contains a 'RESET' record, but the From and To dates are not the same: " +
                    "record ID is " + o.ID.ToString() + ", component `" + o.EQUIPCOMPONENT.sComponent + "`, equipment `" + o.EQUIPCOMPONENT.EQUIPMENT.sShortEquipName + "`"));
                return;
            }
            q7 = from o in dc.EQUIPOPERDATAs where o.cSource != 'R' && o.DFrom == o.DTo select o;
            foreach (var o in q7)
            {
                ProcessPopupException(new Global.excToPopup("Table EQUIPOPERDATA contains a record where the From and To dates are the same, but it is not marked as a 'RESET' record: " +
                    "record ID is " + o.ID.ToString() + ", component `" + o.EQUIPCOMPONENT.sComponent + "`, equipment `" + o.EQUIPCOMPONENT.EQUIPMENT.sShortEquipName + "`"));
                return;
            }
            #endregion

            #region Table EQUIPOPERDATA: Reset records for one component must have different DFrom
            var qif = from d in dc.EQUIPOPERDATAs where d.cSource == 'R' group d by d.iEquipComponent;
            foreach(var d in qif)
            {
                SortedList<DateTime, int> sortLi = new SortedList<DateTime, int>();
                foreach (var d2 in d)
                {
                    try
                    {
                        sortLi.Add(d2.DFrom.Date, d2.iEquipComponent); // Adding an already existing key throws an ArgumentException
                    }
                    catch (ArgumentException)
                    {
                        ProcessPopupException(new Global.excToPopup("Table EQUIPOPERDATA has 'Reset' times with date portions that are not different from each other by at least one day for component '" +
                            d.First().EQUIPCOMPONENT.sComponent + "'."));
                        return;
                    }
                }
            }
            #endregion

            #region Table EQUIPOPERDATA: Intervals formed by DFrom and DTo must have a minimum time span of one day
            qo = from o in dc.EQUIPOPERDATAs where o.cSource == 'M' select o;
            foreach (var o in qo)
            {
                if (o.DTo.Subtract(o.DFrom).TotalDays < 1.0)
                {
                    ProcessPopupException(new Global.excToPopup("Table EQUIPOPERDATA interval DFrom " + o.DFrom.ToString() +
                        " DTo " + o.DTo.ToString() + " is shorter than one day for equipment component '" + o.EQUIPCOMPONENT.sComponent + "'."));
                    return;
                }
            }
            #endregion

            #region Table EQUIPOPERDATA: DFrom and DTo must be within DLinkBegin and DLinkEnd of the component pointed to by iEquipComponent
            var qp = from p in dc.EQUIPOPERDATAs orderby p.iEquipComponent select new { p, p.EQUIPCOMPONENT.sComponent };
            int iEquipCompPrev = 0;
            int iEquipComp = 0;
            TNPF_LinkBeginEndResult qq = null;
            foreach (var p in qp)
            {
                iEquipComp = p.p.iEquipComponent;
                if (iEquipComp != iEquipCompPrev)
                {
                    qq = (from q in dc.TNPF_LinkBeginEnd(iEquipComp) select q).First();
                    if (qq.DBegin <= DateTimeOffset.MinValue.AddDays(1) || qq.DEnd >= DateTimeOffset.MaxValue.AddDays(-1))
                    {
                        ProcessPopupException(new Global.excToPopup("AdminPages.DBMaint.DBIntegrit.pbEquip_Click Error while checking table EQUIPOPERDATA: " +
                            " Infinite Loop detected in TNPF_LinkBeginEnd.SQL. iEquipComp = "
                            + iEquipComp.ToString() + "."));
                        return;
                    }
                    iEquipCompPrev = iEquipComp;
                }
                if (p.p.DFrom < qq.DBegin)
                {
                    ProcessPopupException(new Global.excToPopup("Table EQUIPOPERDATA field DFrom = " + p.p.DFrom.ToString() +
                        " for equipment component '" + p.sComponent + "' is earlier than its DLinkBegin " + qq.DBegin.ToString()));
                    return;
                }
                if (p.p.DTo > qq.DEnd)
                {
                    ProcessPopupException(new Global.excToPopup("Table EQUIPOPERDATA field DTo = " + p.p.DTo.ToString() +
                        " for equipment component '" + p.sComponent + "' is later than its DLinkEnd " + qq.DEnd.ToString()));
                    return;
                }
            }
            #endregion

            #region Table EQUIPOPERDATA: Intervals formed by DFrom and DTo for one component must not overlap
            qo = from o in dc.EQUIPOPERDATAs orderby o.DFrom select o;
            foreach (var o in qo)
            {
                var qv = from v in dc.EQUIPOPERDATAs
                         orderby v.DFrom
                         where v.DFrom > o.DFrom && v.iEquipComponent == o.iEquipComponent
                         select v;
                foreach (var v in qv)
                {
                    if (v.DFrom < o.DTo && v.DTo > o.DFrom)
                    {
                        ProcessPopupException(new Global.excToPopup("Table EQUIPOPERDATA interval DFrom " + o.DFrom.ToString() +
                            " DTo " + o.DTo.ToString() + " overlaps interval DFrom " + v.DFrom.ToString() +
                            " DTo " + v.DTo.ToString() + " for component '" + o.EQUIPCOMPONENT.sComponent + "'."));
                        return;
                    }
                }
            }
            #endregion

            #region Table OPSCALNAMES: one and only one tuple must have bStandard = true
            int iCount = (from o in dc.OPSCALNAMEs where o.bStandard select o).Count();
            if (iCount != 1)
            {
                ProcessPopupException(new Global.excToPopup("Table OPSCALNAMES is supposed to have one and only one record with bStandard = true but the count is " + iCount.ToString()));
                return;
            }
            #endregion

            #region Table OPSCALTIMES: There must be at least two segments in OPSCALTIMES for each entry in OPSCALNAMES
            var qn = from n in dc.OPSCALNAMEs select n;
            foreach (var n in qn)
            {
                iCount = (from t in dc.OPSCALTIMEs where t.iOpsCal == n.ID select t).Count();
                if (iCount < 2)
                {
                    ProcessPopupException(new Global.excToPopup("The number of segments in an operational calendar must be at least 2. '" +
                        n.sOpsCalName + "' has only " + iCount.ToString() + "."));
                    return;
                }
            }
            #endregion

            #region Table EQUIPCOMPONENTS: two 'Entire' components cannot refer to the same piece of equipment
            var qf = (from f in dc.EQUIPCOMPONENTs where f.bEntire select f.iEquipment).ToList();
            var iaDistinct = new HashSet<int>(qf);
            bool bAllDifferent = iaDistinct.Count == qf.Count;
            if (!bAllDifferent)
            {
                for (int i = 0; i < qf.Count - 1; i++)
                {
                    for (int j = i + 1; j < qf.Count; j++)
                    {
                        if (qf[i] == qf[j])
                        {
                            string sName = (from g in dc.EQUIPMENTs where g.ID == qf[i] select g.sShortEquipName).First();
                            ProcessPopupException(new Global.excToPopup("Table EQUIPCOMPONENTS field iEquipment " + qf[i].ToString() + " ('" +
                                sName + "') occurs more than once among the 'Entire' components."));
                            return;
                        }
                    }
                }
            }
            #endregion

            #region Table EQUIPCOMPONENTS: 'Entire' entries must have 'real' DLInkBegin/End dates; and must refer to an existing piece of equipment
            var qD = from D in dc.EQUIPCOMPONENTs select D;
            foreach (var D in qD)
            {
                if (D.bEntire)
                {
                    if (D.DLinkBegin < Global.DTO_EqAgEarliest)
                    {
                        ProcessPopupException(new Global.excToPopup("Table EQUIPCOMPONENTS field DLinkBegin = " + D.DLinkBegin.ToString() +
                            " but must be >= " + Global.DTO_EqAgEarliest.ToString() + " for 'Entire' component " + D.sComponent + "."));
                        return;
                    }
                    if (D.DLinkEnd > Global.DTO_EqAgLatest)
                    {
                        ProcessPopupException(new Global.excToPopup("Table EQUIPCOMPONENTS field DLinkEnd = " + D.DLinkEnd.ToString() +
                            " but must be <= " + Global.DTO_EqAgLatest.ToString() + " for 'Entire' component " + D.sComponent + "."));
                        return;
                    }
                }
                else
                {
                    if (D.iParentComponent < 1)
                    {
                        ProcessPopupException(new Global.excToPopup("Table EQUIPCOMPONENTS field iParentComponent = " + D.iParentComponent.ToString() +
                            " but must be > 0 and refer to another equipment component's ID for subcomponent " + D.sComponent + "."));
                        return;
                    }
                    var q8 = (from C in dc.EQUIPCOMPONENTs where C.ID == D.iParentComponent select C).ToList();
                    if (q8.Count != 1)
                    {
                        ProcessPopupException(new Global.excToPopup("Table EQUIPCOMPONENTS field iParentComponent = " + D.iParentComponent.ToString() +
                            " must refer to another equipment component's ID but it does not for subcomponent " + D.sComponent + "."));
                        return;
                    }
                }
                // The requirement that a component must refer to an existing piece of equipment is handled in the database by the foreign key constraint
                //    FK_EQUIPCOMPONENTS_EQUIPMENT
            }
            #endregion

            #region Table EQUIPCOMPONENTS: DLinkBegin and DLinkEnd for subcomponents must be within DLinkBegin/End of their parent components
            var q9 = from q in dc.EQUIPCOMPONENTs where !q.bEntire select q;
            foreach (var q in q9)
            {
                var qu = (from u in dc.TNPF_LinkBeginEnd(q.iParentComponent) select u).First();
                if (qu.DBegin <= DateTimeOffset.MinValue.AddDays(1) || qu.DEnd >= DateTimeOffset.MaxValue.AddDays(-1))
                {
                    ProcessPopupException(new Global.excToPopup("AdminPages.DBMaint.DBIntegrity.pbEquip_Click Error while checking table EQUIPCOMPONENTS: " +
                        " Infinite Loop detected in TNPF_LinkBeginEnd.SQL. iEquipComponent = "
                        + q.iParentComponent.ToString() + "."));
                    return;
                }
                if (q.DLinkBegin >= Global.DTO_EqAgEarliest)
                {
                    if (q.DLinkBegin < qu.DBegin)
                    {
                        ProcessPopupException(new Global.excToPopup("Table EQUIPCOMPONENTS field DLinkBegin = " +
                            CustFmt.sFmtDate(q.DLinkBegin, CustFmt.enDFmt.DateAndTimeMinOffset) +
                            " for subcomponent '" + q.sComponent + "' is earlier than DLinkBegin " +
                            CustFmt.sFmtDate(qu.DBegin, CustFmt.enDFmt.DateAndTimeMinOffset) +
                            " of its parent, and later than or equal to " +
                            CustFmt.sFmtDate(Global.DTO_EqAgEarliest, CustFmt.enDFmt.DateAndTimeMinOffset) +
                            " [Earlier than that is allowed: signals that this component's DLinkBegin is the same as its parent's]."));
                        return;
                    }
                }
                if (q.DLinkEnd <= Global.DTO_EqAgLatest)
                {
                    if (q.DLinkEnd > qu.DEnd)
                    {
                        ProcessPopupException(new Global.excToPopup("Table EQUIPCOMPONENTS field DLinkEnd = " +
                            CustFmt.sFmtDate(q.DLinkEnd, CustFmt.enDFmt.DateAndTimeMinOffset) +
                            " for subcomponent '" + q.sComponent + "' is later than DLinkEnd " +
                            CustFmt.sFmtDate(qu.DEnd, CustFmt.enDFmt.DateAndTimeMinOffset) +
                            " of its parent, and earlier than or equal to " +
                            CustFmt.sFmtDate(Global.DTO_EqAgLatest, CustFmt.enDFmt.DateAndTimeMinOffset) +
                            " [Later than that is allowed: signals that this component's DLinkEnd is the same as its parent's]."));
                        return;
                    }
                }
            }
            #endregion

            ProcessPopupException(new Global.excToPopup("Database integrity checking for tables in the Equipment area has been completed."));
        }

        protected void pbMiscell_Click(object sender, EventArgs e)
        {
            DataIntegrityDataContext dc = new DataIntegrityDataContext();
            int iCount;

            #region Table AVIATORROLES must contain roles 'Tow Pilot' and 'Instructor'
            foreach (string sRole in new string[] { "Tow Pilot", "Instructor"})
            {
                iCount = (from qt in dc.AVIATORROLEs where qt.sAviatorRole == sRole select qt).Count();
                if (iCount == 0)
                {
                    ProcessPopupException(new Global.excToPopup("Table AVIATORROLES does not contain a record for sAviatorRole = '" + sRole + "'; this is required for generation of invoices for flying activities."));
                    return;
                }
                if (iCount > 1)
                {
                    ProcessPopupException(new Global.excToPopup("Table AVIATORROLES contains more than one record for sAviatorRole = '" + sRole + "' (case-insensitive); there should be only one."));
                    return;
                }
            }
            #endregion

            #region Table EQUIPTYPES must contain "Any" for one of the equipment types
            iCount = (from qt in dc.EQUIPTYPEs where qt.sEquipmentType == "Any" select qt).Count();
            if (iCount == 0)
            {
                ProcessPopupException(new Global.excToPopup("Table EQUIPTYPES does not contain a record for sEquipmentType = 'Any'; this is required for generation of invoices for flying activities."));
                return;
            }
            if (iCount > 1)
            {
                ProcessPopupException(new Global.excToPopup("Table EQUIPTYPES contains more than one record for sEquipmentType = 'Any' (case-insensitive); there should be only one."));
                return;
            }
            #endregion

            #region Table LAUNCHMETHODS must contain "Any" for one of the launch methods
            var ql = from l in dc.LAUNCHMETHODs where l.sLaunchMethod == "Any" select l.ID;
            iCount = ql.Count();
            if (iCount == 0)
            {
                ProcessPopupException(new Global.excToPopup("Table LAUNCHMETHODS does not contain a record for sLaunchMethod = 'Any'; this is required for generation of invoices for flying activities."));
                return;
            }
            if (iCount > 1)
            {
                ProcessPopupException(new Global.excToPopup("Table LAUNCHMETHODS contains more than one record for sLaunchMethod = 'Any' (case-insensitive); there should be only one."));
                return;
            }
            int iLaunchMethod = ql.First();
            #endregion

            #region Table Rates must have charge amounts that are 0 or greater:
            var q2 = from r in dc.RATEs select r;
            foreach (var r in q2)
            {
                if (r.mSingleDpUse < 0.0M)
                {
                    ShowProblem(dc, r.ID, "$/Single Use");
                    return;
                }
                //if (r.iNoChrg1stFt < 0.0M)
                //{
                //    ShowProblem(dc, r.ID, "No charge for first x Feet");
                //    return;
                //}
                if (r.mAltDiffDpFt < 0.0M)
                {
                    ShowProblem(dc, r.ID, "$/foot Altit. Diff.");
                    return;
                }
                if (r.iNoChrg1stMin < 0.0M)
                {
                    ShowProblem(dc, r.ID, "No charge for first x minutes");
                    return;
                }
                if (r.mDurationDpMin < 0.0M)
                {
                    ShowProblem(dc, r.ID, "$/minute");
                    return;
                }
            }
            #endregion

            #region Table RATES must have a record for not charging anything with sShortName = 'Zero'
            iCount = (from qr in dc.RATEs
                      where qr.sShortName == "Zero"
                      select qr).Count();
            if (iCount == 0)
            {
                ProcessPopupException(new Global.excToPopup("Table RATES does not contain a record with Short Name = 'Zero'; this is required for generation of invoices for flying activities."));
                return;
            }
            if (iCount > 1)
            {
                ProcessPopupException(new Global.excToPopup("Table RATES contains more than one record for Short Name = 'Zero' (case-insensitive); there should be only one."));
                return;
            }

            var qz = from qr in dc.RATEs
                      where qr.sShortName == "Zero"
                        && qr.Dfrom < DateTimeOffset.Parse("2001-01-01")
                        && qr.DTo > DateTimeOffset.Parse("2998-12-31")
                        && qr.EQUIPTYPE.sEquipmentType == "Any"
                        && qr.iLaunchMethod == iLaunchMethod // Points to launch method 'Any'
                        && qr.mSingleDpUse == 0.0M
                        && qr.iNoChrg1stFt == 0
                        && qr.mAltDiffDpFt == 0.0M
                        && qr.iNoChrg1stMin == 0
                        && qr.mDurationDpMin == 0.0M
                      select qr;
            iCount = qz.Count();

            iLaunchMethod = (from r in dc.RATEs where r.sShortName == "Zero" select r.iLaunchMethod).First(); // iLaunchMethod now points to whatever the RATES Zero record specifies for launch method

            if (iCount == 0)
            {
                ButtonsClear();
                txbPopup.Text = "At least one of the following conditions is NOT fulfilled for the 'Zero' record in table RATES:" + Environment.NewLine;
                var qy = from qr in dc.RATEs
                         where qr.sShortName == "Zero"
                         select qr;

                string sLaunchMethod = (from l in dc.LAUNCHMETHODs where l.ID == iLaunchMethod select l.sLaunchMethod).First();

                foreach (var qr in qy)
                {
                    txbPopup.Text += Environment.NewLine + "`From` Date " + qr.Dfrom + " < 2001/01/01";
                    txbPopup.Text += Environment.NewLine + "`To` Date " + qr.DTo + " > 2998/12/31";
                    txbPopup.Text += Environment.NewLine + "Equipment type `" + qr.EQUIPTYPE.sEquipmentType + "` is `Any`";
                    txbPopup.Text += Environment.NewLine + "Launch method `" + sLaunchMethod + "` is `Any`";
                    txbPopup.Text += Environment.NewLine + "$/Single Use = " + qr.mSingleDpUse + " is zero";
                    txbPopup.Text += Environment.NewLine + "No charge for first x Feet = " + qr.iNoChrg1stFt + " is zero";
                    txbPopup.Text += Environment.NewLine + "$/foot Altit. Diff. = " + qr.mAltDiffDpFt + " is zero";
                    txbPopup.Text += Environment.NewLine + "No charge for first x minutes = " + qr.iNoChrg1stMin + " is zero";
                    txbPopup.Text += Environment.NewLine + "$/minute = " + qr.mDurationDpMin + " is zero";
                }
                MPE_Show(Global.enumButtons.OkOnly, "Zero");
                return;
            }
            #endregion

            #region Table INVOICESOURCES must contain "Flying Activities"
            iCount = (from qt in dc.INVOICESOURCEs where qt.sInvoiceSource == "Flying Activities" select qt).Count();
            if (iCount == 0)
            {
                ProcessPopupException(new Global.excToPopup("Table INVOICESOURCES does not contain a record for sInvoiceSource = 'Flying Activities'; this is required for generation of invoices for flying activities."));
                return;
            }
            if (iCount > 1)
            {
                ProcessPopupException(new Global.excToPopup("Table INVOICESOURCES contains more than one record for sInvoiceSource = 'Flying Activities' (case-insensitive); there should be only one."));
                return;
            }
            #endregion

            #region Table PEOPLE must contain an entry for "[none]", and it must be a long-term member
            // where long term means from before 1970/1/1 to after 2999/12/31
            iCount = (from p in dc.PEOPLEs where p.sDisplayName == "[none]" select p).Count();
            if (iCount == 0)
            {
                ProcessPopupException(new Global.excToPopup("Table PEOPLE does not contain a record for sDisplayName = '[none]'; this is required for input of daily flight logs."));
                return;
            }
            if (iCount > 1)
            {
                ProcessPopupException(new Global.excToPopup("Table PEOPLE contains more than one record for sDisplayName = '[none]' (case-insensitive); there should be one and only one."));
                return;
            }
            var qm = (from m in dc.MEMBERFROMTOs where m.PEOPLE.sDisplayName == "[none]" select m).ToList();
            if (qm.Count < 1)
            {
                ProcessPopupException(new Global.excToPopup("Member [none] exists in table PEOPLE, but it has no corresponding entry in table MEMBERFROMTO."));
                return;
            }
            if (qm.Count > 1)
            {
                ProcessPopupException(new Global.excToPopup("Member [none] exists in table PEOPLE, but it has more than one corresponding entry in table MEMBERFROMTO."));
                return;
            }
            if (qm.First().DMembershipBegin > new DateTimeOffset(1970,1,1,0,0,0, new TimeSpan(0)))
            {
                ProcessPopupException(new Global.excToPopup("Member [none] exists in table PEOPLE, but membership start time is later than start of 1970."));
                return;
            }
            if (qm.First().DMembershipEnd < new DateTimeOffset(2999,12,31,0,0,0, new TimeSpan(0)))
            {
                ProcessPopupException(new Global.excToPopup("Member [none] exists in table PEOPLE, but membership end time is earlier than end of year 2999."));
                return;
            }
            #endregion

            #region Tables MEMBERFROMTO, SSA_MEMBERFROMTO, PEOPLEQUALIFICS, PEOPLECERTIFICS, PEOPLERATINGS, and PEOPLEOFFICES must not have overlapping time intervals for the same person
            string sStatus = "";
            dc.spCheck4PeopleFromToOverlaps(ref sStatus);
            if ("OK" != sStatus.Substring(0, 2))
            {
                ProcessPopupException(new Global.excToPopup(sStatus));
                return;
            }
            #endregion

            #region Table QUALIFICATIONS must contain a record with 'PSSA Tow Pilot'
            iCount = (from qq in dc.QUALIFICATIONs where qq.sQualification == "PSSA Tow Pilot" select qq).Count();
            if (iCount == 0)
            {
                ProcessPopupException(new Global.excToPopup("Table QUALIFICATIONS does not contain a record for sQualification = 'PSSA Tow Pilot'; this is required for instructor/tow pilot service points and rewards."));
                return;
            }
            if (iCount > 1)
            {
                ProcessPopupException(new Global.excToPopup("Table QUALIFICATIONS contains more than one record for sQualification = 'PSSA Tow Pilot' (case-insensitive); there should be only one."));
                return;
            }
            #endregion

            #region Table CERTIFICATIONS must contain a record with 'Instructor'
            iCount = (from qq in dc.CERTIFICATIONs where qq.sCertification == "Instructor" select qq).Count();
            if (iCount == 0)
            {
                ProcessPopupException(new Global.excToPopup("Table CERTIFICATIONS does not contain a record for sCertification = 'Instructor'; this is required for instructor/tow pilot service points and rewards."));
                return;
            }
            if (iCount > 1)
            {
                ProcessPopupException(new Global.excToPopup("Table CERTIFICATIONS contains more than one record for sCertification = 'Instructor' (case-insensitive); there should be only one."));
                return;
            }
            #endregion

            #region Table RATINGS must contain a record with 'Instructor'
            iCount = (from qq in dc.RATINGs where qq.sRating == "Glider" select qq).Count();
            if (iCount == 0)
            {
                ProcessPopupException(new Global.excToPopup("Table RATINGS does not contain a record for sRating = 'Glider'; this is required for instructor/tow pilot service points and rewards."));
                return;
            }
            if (iCount > 1)
            {
                ProcessPopupException(new Global.excToPopup("Table RATINGS contains more than one record for sRating = 'Glider' (case-insensitive); there should be only one."));
                return;
            }
            #endregion

            ProcessPopupException(new Global.excToPopup("Database integrity checking for miscellaneous items has been completed."));
        }

        private void ShowProblem(DataIntegrityDataContext udc ,int iuID, string suItem)
        {
            var r = (from d in udc.RATEs where d.ID == iuID select new { SI = d, d.EQUIPTYPE.sEquipmentType}).First();
            string sLaunchMethod = (from e in udc.LAUNCHMETHODs where e.ID == r.SI.iLaunchMethod select e.sLaunchMethod).First();
            ProcessPopupException(new Global.excToPopup("Monetary Charging RATES for `" + r.SI.sShortName + "`, from " + r.SI.Dfrom.ToString() + " to "
                + r.SI.DTo.ToString() + ", equipment type `" + r.SI.EQUIPTYPE.sEquipmentType + "`, launch methd `" + sLaunchMethod + "`: item `" + suItem + "` is not >= 0"));
        }
    }
}