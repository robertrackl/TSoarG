using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Web.SessionState;
using System.Web.UI.WebControls;
using TSoar.DB;

namespace TSoar.Accounting
{
    public static class AssistLi
    {
        #region Structures
        #region Expenses
        public struct SRowDataExp // Expenditure
        {
            public int iAcct;
            public string sAcct;
            public string sDescr;
            public string sAmount;
        }
        public struct SRowDataPmt // Payment
        {
            public int iAcct;
            public string sAcct;
            public int iPmtMethod;
            public string sPmtMethod;
            public string sDescr;
            public string sAmount;
        }
        public struct SRowDataAtt // AttachedFile
        {
            public int iCateg;
            public string sCateg;
            public DateTimeOffset DAssoc; //Date to be associated with this document, usually (but not necessarily) the same as the transaction's date
            public int iFile; // ID if row in table SF_DOCS; if = 0: file has not yet been stored in SF_DOCS.
            public string sFile; // file name with path
            public int iThumb;
            public string sThumb;
        }
        #endregion
        #region Vendors
        public struct SRowVendor
        {
            public int ID;
            public string sVendorName;
            public string sNotes;
        }
        #endregion
        #region Rates
        public struct SRowRate
        {
            public int ID;
            public int iEquipType;
            public int iLaunchMethod;
            public decimal mSingleDpUse;
            public int iNoChrg1st;
            public decimal mAltDiffDpFt;
            public decimal mDurationDpMin;
            public string sComment;
        }
        #endregion
        #endregion

        #region Init routines
        static public List<DataRow> Init(Global.enLL euLL)
        {
            //Initialize a linked list of the type given in euLL
            DataTable dtPattern = new DataTable();
            TSoar.DB.SCUD_Multi mCrud = new TSoar.DB.SCUD_Multi();
            switch (euLL)
            {
                #region Expense
                case Global.enLL.Expenditure:
                    List<DataRow> liExpenditure = new List<DataRow>();
                    // Define the schema for DataTable dtPattern
                    dtPattern = dtSchema(Global.enLL.Expenditure);

                    // 'Sum' Row
                    DataRow dtRow = dtPattern.NewRow();
                    dtRow[(int)Global.enPFExp.LineNum] = 0;
                    dtRow[(int)Global.enPFExp.AccountID] = 0;
                    dtRow[(int)Global.enPFExp.AccountName] = " ";
                    dtRow[(int)Global.enPFExp.Descr] = "Sum of Debits:";
                    dtRow[(int)Global.enPFExp.sAmount] = "";
                    liExpenditure.Add(dtRow);

                    // 'New' Row
                    dtRow = dtPattern.NewRow();
                    dtRow[(int)Global.enPFExp.LineNum] = -1;
                    string sSetting = mCrud.GetSetting("DefaultExpenseAccount");
                    string sCode = sSetting.Split(' ')[0];
                    sf_AccountingDataContext dc = new sf_AccountingDataContext();
                    var q0 = from p in dc.SF_ACCOUNTs where p.sCode == sCode select p;
                    SF_ACCOUNT qr = q0.First();
                    dtRow[(int)Global.enPFExp.AccountID] = qr.ID;
                    dtRow[(int)Global.enPFExp.AccountName] = qr.sName;
                    dtRow[(int)Global.enPFExp.Descr] = " ";
                    dtRow[(int)Global.enPFExp.sAmount] = "";
                    liExpenditure.Add(dtRow);

                    return liExpenditure;

                case Global.enLL.Payment:
                    List<DataRow> liPayment = new List<DataRow>();
                    dtPattern = dtSchema(Global.enLL.Payment);

                    // 'Sum' Row
                    dtRow = dtPattern.NewRow();
                    dtRow[(int)Global.enPFPmt.LineNum] = 0;
                    dtRow[(int)Global.enPFPmt.AccountID] = 0;
                    dtRow[(int)Global.enPFPmt.AccountName] = " ";
                    dtRow[(int)Global.enPFPmt.PmtMethodID] = 0;
                    dtRow[(int)Global.enPFPmt.PmtMethodName] = " ";
                    dtRow[(int)Global.enPFPmt.Descr] = "Sum of Credits:";
                    dtRow[(int)Global.enPFPmt.sAmount] = "0.00";
                    liPayment.Add(dtRow);

                    // 'New' Row
                    dtRow = dtPattern.NewRow();
                    dtRow[(int)Global.enPFPmt.LineNum] = -1;
                    sSetting = mCrud.GetSetting("DefaultAssetsAccount");
                    sCode = sSetting.Split(' ')[0];
                    dc = new sf_AccountingDataContext();
                    var qPmt = from p in dc.SF_ACCOUNTs where p.sCode == sCode select p;
                    qr = qPmt.First();
                    dtRow[(int)Global.enPFPmt.AccountID] = qr.ID;
                    dtRow[(int)Global.enPFPmt.AccountName] = qr.sName;
                    sSetting = mCrud.GetSetting("DefaultPaymentMethod");
                    var qMethod = from p in dc.SF_PAYMENTMETHODs where p.sPaymentMethod == sSetting select p;
                    var qp = qMethod.First();
                    dtRow[(int)Global.enPFPmt.PmtMethodID] = qp.ID;
                    dtRow[(int)Global.enPFPmt.PmtMethodName] = qp.sPaymentMethod;
                    dtRow[(int)Global.enPFPmt.Descr] = " ";
                    dtRow[(int)Global.enPFPmt.sAmount] = "";
                    liPayment.Add(dtRow);

                    return liPayment;

                case Global.enLL.AttachedFiles:
                    List<DataRow> liAttachedFile = new List<DataRow>();
                    dtPattern = dtSchema(Global.enLL.AttachedFiles);

                    // 'Sum' Row
                    dtRow = dtPattern.NewRow();
                    dtRow[(int)Global.enPFAtt.LineNum] = 0;
                    dtRow[(int)Global.enPFAtt.AttachCategID] = 0;
                    dtRow[(int)Global.enPFAtt.AttachCategName] = "Number of Attached Files:";
                    dtRow[(int)Global.enPFAtt.AttachAssocDate] = DateTimeOffset.MinValue;
                    dtRow[(int)Global.enPFAtt.AttachedFileID] = 0;
                    dtRow[(int)Global.enPFAtt.AttachedFileName] = "0";
                    dtRow[(int)Global.enPFAtt.AttachedThumbID] = 0;
                    dtRow[(int)Global.enPFAtt.AttachedThumbName] = " ";
                    liAttachedFile.Add(dtRow);

                    // 'New' Row
                    dtRow = dtPattern.NewRow();
                    dtRow[(int)Global.enPFAtt.LineNum] = -1;
                    sSetting = mCrud.GetSetting("DefaultAttachmentCateg");
                    dc = new sf_AccountingDataContext();
                    var qAT = from p in dc.SF_ATTACHMENTCATEGs where p.sAttachmentCateg == sSetting select p;
                    var qt = qAT.First();
                    dtRow[(int)Global.enPFAtt.AttachCategID] = qt.ID;
                    dtRow[(int)Global.enPFAtt.AttachCategName] = qt.sAttachmentCateg;
                    dtRow[(int)Global.enPFAtt.AttachAssocDate] = DateTimeOffset.Now;
                    dtRow[(int)Global.enPFAtt.AttachedFileID] = 0;
                    dtRow[(int)Global.enPFAtt.AttachedFileName] = " ";
                    dtRow[(int)Global.enPFAtt.AttachedThumbID] = 0;
                    dtRow[(int)Global.enPFAtt.AttachedThumbName] = " ";
                    liAttachedFile.Add(dtRow);

                    return liAttachedFile;
                #endregion

                #region Vendors
                case Global.enLL.Vendors:
                    List<DataRow> liVendors = new List<DataRow>();
                    dtPattern = dtSchema(Global.enLL.Vendors);
                    dc = new sf_AccountingDataContext();
                    var qv = from v in dc.SF_VENDORs where v.sVendorName != "(none)" select v;
                    // Existing Rows
                    foreach (SF_VENDOR v in qv)
                    {
                        dtRow = dtPattern.NewRow();
                        dtRow[(int)Global.enPFVend.VendorName] = v.sVendorName;
                        dtRow[(int)Global.enPFVend.Notes] = v.sNotes;
                        liVendors.Add(dtRow);
                    }
                    // New row
                    dtRow = dtPattern.NewRow();
                    dtRow[(int)Global.enPFVend.VendorName] = "";
                    dtRow[(int)Global.enPFVend.Notes] = "";
                    liVendors.Add(dtRow);

                    return liVendors;
                #endregion

                #region Rates
                case Global.enLL.Rates:
                    List<DataRow> liRates = new List<DataRow>();
                    dtPattern = dtSchema(Global.enLL.Rates);
                    dc = new sf_AccountingDataContext();
                    var qa = from a in dc.RATEs select a;
                    // Existing rows
                    foreach (RATE a in qa)
                    {
                        dtRow = dtPattern.NewRow();
                        dtRow[(int)Global.enPFRate.i1Sort] = 0; // ensures that existing rows appear at the top of the gridview
                        dtRow[(int)Global.enPFRate.ID] = a.ID;
                        dtRow[(int)Global.enPFRate.sShortName] = a.sShortName;
                        dtRow[(int)Global.enPFRate.DFrom] = a.Dfrom;
                        dtRow[(int)Global.enPFRate.DTo] = a.DTo;
                        dtRow[(int)Global.enPFRate.iEquipType] = a.iEquipType;
                        dtRow[(int)Global.enPFRate.iLaunchMethod] = a.iLaunchMethod;
                        dtRow[(int)Global.enPFRate.sChargeCodes] = a.sChargeCodes;
                        dtRow[(int)Global.enPFRate.mSingleDpUse] = a.mSingleDpUse;
                        dtRow[(int)Global.enPFRate.iNoChrg1stFt] = a.iNoChrg1stFt;
                        dtRow[(int)Global.enPFRate.mAltDiffDpFt] = a.mAltDiffDpFt;
                        dtRow[(int)Global.enPFRate.iNoChrg1stMin] = a.iNoChrg1stMin;
                        dtRow[(int)Global.enPFRate.mDurationDpMin] = a.mDurationDpMin;
                        dtRow[(int)Global.enPFRate.iDurCapMin] = a.iDurCapMin;
                        dtRow[(int)Global.enPFRate.sComment] = a.sComment;
                        dtRow[(int)Global.enPFRate.iFA_Item] = a.iFA_Item;
                        dtRow[(int)Global.enPFRate.iFA_PmtTerm] = a.iFA_PmtTerm;
                        dtRow[(int)Global.enPFRate.iQBO_ItemName] = a.iQBO_ItemName;
                        liRates.Add(dtRow);
                    }
                    // New Row
                    dtRow = dtPattern.NewRow();
                    dtRow[(int)Global.enPFRate.i1Sort] = 1; // ensures that the new row appears at the bottom of the gridview
                    dtRow[(int)Global.enPFRate.ID] = 0;
                    dtRow[(int)Global.enPFRate.sShortName] = "_";
                    dtRow[(int)Global.enPFRate.DFrom] = DateTimeOffset.Parse("1900/01/01");
                    dtRow[(int)Global.enPFRate.DTo] = DateTimeOffset.Parse("2999/12/31");
                    sSetting = mCrud.GetSetting("DefaultEquipmentType");
                    var qq = from a in dc.EQUIPTYPEs where a.sEquipmentType == sSetting select a.ID;
                    dtRow[(int)Global.enPFRate.iEquipType] = qq.First();
                    sSetting = mCrud.GetSetting("DefaultLaunchMethod");
                    var ql = from l in dc.LAUNCHMETHODs where l.sLaunchMethod == sSetting select l.ID;
                    dtRow[(int)Global.enPFRate.iLaunchMethod] = ql.First();
                    dtRow[(int)Global.enPFRate.sChargeCodes] = "F";
                    dtRow[(int)Global.enPFRate.mSingleDpUse] = 0.00;
                    dtRow[(int)Global.enPFRate.iNoChrg1stFt] = 0;
                    dtRow[(int)Global.enPFRate.mAltDiffDpFt] = 0.00;
                    dtRow[(int)Global.enPFRate.iNoChrg1stMin] = 0;
                    dtRow[(int)Global.enPFRate.mDurationDpMin] = 0.00;
                    dtRow[(int)Global.enPFRate.iDurCapMin] = 0;
                    dtRow[(int)Global.enPFRate.sComment] = "";
                    sSetting = mCrud.GetSetting("DefaultFA_item_code");
                    dtRow[(int)Global.enPFRate.iFA_Item] = (from b in dc.FA_ITEMs where b.sFA_ItemCode == sSetting select b.ID).First();
                    sSetting = mCrud.GetSetting("DefaultFA_PaymentTerm");
                    dtRow[(int)Global.enPFRate.iFA_PmtTerm] = (from b in dc.FA_PMTTERMs where b.iPmtTermsCode == int.Parse(sSetting) select b.ID).First();
                    sSetting = mCrud.GetSetting("DefaultQBO_AccItem");
                    var qi = from a in dc.QBO_ITEMNAMEs where a.sQBO_ItemName == sSetting select a.ID;
                    dtRow[(int)Global.enPFRate.iQBO_ItemName] = qi.First();
                    liRates.Add(dtRow);

                    return liRates;
                #endregion

                #region Minimum Flying Charges Parameters
                case Global.enLL.MinFlyChrgs:
                    List<DataRow> liMFCP = new List<DataRow>();
                    dtPattern = dtSchema(Global.enLL.MinFlyChrgs);
                    FinDetails.SalesAR.FlyActInvoiceDataContext fdc = new FinDetails.SalesAR.FlyActInvoiceDataContext();
                    var qm = from m in fdc.MINFLYCHGPARs select m;
                    // Existing Rows
                    foreach (FinDetails.SalesAR.MINFLYCHGPAR m in qm)
                    {
                        dtRow = dtPattern.NewRow();
                        dtRow[(int)Global.enPFMFC.i1Sort] = 0; // ensures that existing rows appear at the top of the gridview
                        dtRow[(int)Global.enPFMFC.ID] = m.ID;
                        dtRow[(int)Global.enPFMFC.DFrom] = m.DFrom;
                        dtRow[(int)Global.enPFMFC.DTo] = m.DTo;
                        dtRow[(int)Global.enPFMFC.iMembCateg] = m.iMembershipCategory;
                        dtRow[(int)Global.enPFMFC.sMinFlyChrg] = m.mMinMonthlyFlyChrg.ToString("N2", CultureInfo.InvariantCulture);
                        dtRow[(int)Global.enPFMFC.sComment] = m.sComments;

                        liMFCP.Add(dtRow);
                    }
                    // New Row
                    dtRow = dtPattern.NewRow();
                    dtRow[(int)Global.enPFMFC.i1Sort] = 1; // ensures that the new row appears at the bottom of the gridview
                    dtRow[(int)Global.enPFMFC.ID] = 0;
                    DateTimeOffset DD = DateTimeOffset.Now;
                    dtRow[(int)Global.enPFMFC.DFrom] = DateTimeOffset.Parse(DD.Year.ToString() + "/03/01");
                    dtRow[(int)Global.enPFMFC.DTo] = DateTimeOffset.Parse(DD.Year.ToString() + "/10/31 23:59");
                    sSetting = mCrud.GetSetting("DefaultMembCategMFC");
                    var qn = from m in fdc.MEMBERSHIPCATEGORies where m.sMembershipCategory == sSetting select m.ID;
                    dtRow[(int)Global.enPFMFC.iMembCateg] = qn.First();
                    sSetting = mCrud.GetSetting("DefaultMinimumMonthlyFlyingCharge");
                    dtRow[(int)Global.enPFMFC.sMinFlyChrg] = sSetting;
                    dtRow[(int)Global.enPFMFC.sComment] = "";
                    liMFCP.Add(dtRow);

                    return liMFCP;
                #endregion

                #region People Offices
                case Global.enLL.PeopleOffices:
                    List<DataRow> liPeopleOffices = new List<DataRow>();
                    dtPattern = dtSchema(Global.enLL.PeopleOffices);
                    TSoar.DB.TNPV_PeopleContactsDataContext pdc = new TSoar.DB.TNPV_PeopleContactsDataContext();
                    var qo = from o in pdc.PEOPLEOFFICEs select new { o.ID, o.PEOPLE.sDisplayName, o.BOARDOFFICE.sBoardOffice, o.DOfficeBegin, o.DOfficeEnd, o.sAdditionalInfo };
                    // Existing Rows
                    foreach (var o in qo)
                    {
                        dtRow = dtPattern.NewRow();
                        dtRow[(int)Global.enPFPeopleOffices.i1Sort] = 0;
                        dtRow[(int)Global.enPFPeopleOffices.ID] = o.ID;
                        dtRow[(int)Global.enPFPeopleOffices.sDisplayName] = o.sDisplayName;
                        dtRow[(int)Global.enPFPeopleOffices.sBoardOffice] = o.sBoardOffice;
                        dtRow[(int)Global.enPFPeopleOffices.DOfficeBegin] = o.DOfficeBegin;
                        dtRow[(int)Global.enPFPeopleOffices.DOfficeEnd] = o.DOfficeEnd;
                        dtRow[(int)Global.enPFPeopleOffices.sAdditionalInfo] = o.sAdditionalInfo;

                        liPeopleOffices.Add(dtRow);
                    }
                    // New Row
                    dtRow = dtPattern.NewRow();
                    dtRow[(int)Global.enPFPeopleOffices.i1Sort] = 1;
                    dtRow[(int)Global.enPFPeopleOffices.ID] = 0;
                    dtRow[(int)Global.enPFPeopleOffices.sDisplayName] = (from p in pdc.PEOPLEs select p.sDisplayName).First();
                    dtRow[(int)Global.enPFPeopleOffices.sBoardOffice] = (from b in pdc.BOARDOFFICEs select b.sBoardOffice).First();
                    DD = DateTimeOffset.Now;
                    dtRow[(int)Global.enPFPeopleOffices.DOfficeBegin] = DD;
                    dtRow[(int)Global.enPFPeopleOffices.DOfficeEnd] = DD.AddYears(2);
                    dtRow[(int)Global.enPFPeopleOffices.sAdditionalInfo] = "";

                    liPeopleOffices.Add(dtRow);

                    return liPeopleOffices;
                #endregion

                #region People Bridges to Equipment Roles and Types
                case Global.enLL.PeopleEquipRolesTypes:
                    List<DataRow> liPersonEqRoTy = new List<DataRow>();
                    dtPattern = dtSchema(Global.enLL.PeopleEquipRolesTypes);
                    TSoar.Equipment.EquipmentDataContext eqdc = new TSoar.Equipment.EquipmentDataContext();
                    // Existing rows
                    var qpert = from p in eqdc.sp_PeopleEqRoTy() select p;
                    foreach (var p in qpert)
                    {
                        dtRow = dtPattern.NewRow();
                        dtRow[(int)Global.enPFPeopleEquipRolesTypes.ID] = p.ID;
                        dtRow[(int)Global.enPFPeopleEquipRolesTypes.iPerson] = p.iPerson;
                        dtRow[(int)Global.enPFPeopleEquipRolesTypes.sDisplayName] = p.sDisplayName;
                        dtRow[(int)Global.enPFPeopleEquipRolesTypes.iAviatorRole] = p.iAviatorRole;
                        dtRow[(int)Global.enPFPeopleEquipRolesTypes.sAviatorRole] = p.sAviatorRole;
                        dtRow[(int)Global.enPFPeopleEquipRolesTypes.iEqRoleType] = p.iEqRoTyId;
                        dtRow[(int)Global.enPFPeopleEquipRolesTypes.sEqRoleType] = p.sEqRoTy;
                        dtRow[(int)Global.enPFPeopleEquipRolesTypes.sComments] = p.sComments;

                        liPersonEqRoTy.Add(dtRow);
                    }
                    //New Row
                    dtRow = dtPattern.NewRow();
                    var qpret = (from p in eqdc.PEOPLEs select p).First();
                    dtRow[(int)Global.enPFPeopleEquipRolesTypes.ID] = 0;
                    dtRow[(int)Global.enPFPeopleEquipRolesTypes.iPerson] = qpret.ID;
                    dtRow[(int)Global.enPFPeopleEquipRolesTypes.sDisplayName] = qpret.sDisplayName;
                    var qav = (from a in eqdc.AVIATORROLEs select a).First();
                    dtRow[(int)Global.enPFPeopleEquipRolesTypes.iAviatorRole] = qav.ID;
                    dtRow[(int)Global.enPFPeopleEquipRolesTypes.sAviatorRole] = qav.sAviatorRole;
                    var qroty = (from q in eqdc.EQUIPROLESTYPEs select new { q.ID, sEqRoTy = q.EQUIPMENTROLE.sEquipmentRole + " / " + q.EQUIPTYPE.sEquipmentType + " [" + q.sComments + "]" }).First();
                    dtRow[(int)Global.enPFPeopleEquipRolesTypes.iEqRoleType] = qroty.ID;
                    dtRow[(int)Global.enPFPeopleEquipRolesTypes.sEqRoleType] = qroty.sEqRoTy;
                    dtRow[(int)Global.enPFPeopleEquipRolesTypes.sComments] = "";

                    liPersonEqRoTy.Add(dtRow);
                    return liPersonEqRoTy;

                #endregion

                #region Equipment Bridges to Roles and Types
                case Global.enLL.EquipRolesTypes:
                    List<DataRow> liEqRoTy = new List<DataRow>();
                    dtPattern = dtSchema(Global.enLL.EquipRolesTypes);
                    eqdc = new TSoar.Equipment.EquipmentDataContext();
                    var qrt = from q in eqdc.sp_EquipRolesTypes() select q;
                    foreach (var q in qrt)
                    {
                        dtRow = dtPattern.NewRow();
                        dtRow[(int)Global.enPFEqRolesTypes.ID] = q.ID;
                        dtRow[(int)Global.enPFEqRolesTypes.iRole] = q.iRole;
                        dtRow[(int)Global.enPFEqRolesTypes.sRole] = q.sRole;
                        dtRow[(int)Global.enPFEqRolesTypes.iType] = q.iType;
                        dtRow[(int)Global.enPFEqRolesTypes.sType] = q.sType;
                        dtRow[(int)Global.enPFEqRolesTypes.sComments] = q.sComments;

                        liEqRoTy.Add(dtRow);
                    }
                    //New Row
                    dtRow = dtPattern.NewRow();
                    dtRow[(int)Global.enPFEqRolesTypes.ID] = 0;
                    var qrole = (from q in eqdc.EQUIPMENTROLEs orderby q.sEquipmentRole select q).First();
                    dtRow[(int)Global.enPFEqRolesTypes.iRole] = qrole.ID;
                    dtRow[(int)Global.enPFEqRolesTypes.sRole] = qrole.ID.ToString() + " - " + qrole.sEquipmentRole;
                    var qtype = (from q in eqdc.EQUIPTYPEs orderby q.sEquipmentType select q).First();
                    dtRow[(int)Global.enPFEqRolesTypes.iType] = qtype.ID;
                    dtRow[(int)Global.enPFEqRolesTypes.sType] = qtype.ID.ToString() + " - " + qtype.sEquipmentType;
                    dtRow[(int)Global.enPFEqRolesTypes.sComments] = "";

                    liEqRoTy.Add(dtRow);

                    return liEqRoTy;
                #endregion

                #region Bridges between LaunchMethods and Equipment Roles
                case Global.enLL.LaunchMethodsEqRoles:
                    List<DataRow> liLMEqR = new List<DataRow>();
                    dtPattern = dtSchema(Global.enLL.LaunchMethodsEqRoles);
                    Statistician.StatistDailyFlightLogDataContext stdc = new Statistician.StatistDailyFlightLogDataContext();
                    var qL = from L in stdc.BRIDGE_LAUNCHMETH_EQUIPROLEs
                             select new
                             {
                                 iLaunchMethod = L.LAUNCHMETHOD.ID,
                                 sLaunchMethod = L.LAUNCHMETHOD.ID.ToString() + " - " + L.LAUNCHMETHOD.sLaunchMethod,
                                 L.ID,
                                 L.sComment,
                                 iEquipmentRole = L.EQUIPMENTROLE.ID,
                                 sEquipmentRole = L.EQUIPMENTROLE.ID.ToString() + " - " + L.EQUIPMENTROLE.sEquipmentRole
                             };
                    foreach (var q in qL)
                    {
                        dtRow = dtPattern.NewRow();
                        dtRow[(int)Global.enPFLaunchMEqRoles.ID] = q.ID;
                        dtRow[(int)Global.enPFLaunchMEqRoles.iLaunchMethod] = q.iLaunchMethod;
                        dtRow[(int)Global.enPFLaunchMEqRoles.sLaunchMethod] = q.sLaunchMethod;
                        dtRow[(int)Global.enPFLaunchMEqRoles.iEqRole] = q.iEquipmentRole;
                        dtRow[(int)Global.enPFLaunchMEqRoles.sEqRole] = q.sEquipmentRole;
                        dtRow[(int)Global.enPFLaunchMEqRoles.sComments] = q.sComment;

                        liLMEqR.Add(dtRow);
                    }
                    //New Row
                    dtRow = dtPattern.NewRow();
                    dtRow[(int)Global.enPFLaunchMEqRoles.ID] = 0;
                    var qLaunchMethod = (from L in stdc.LAUNCHMETHODs orderby L.sLaunchMethod select L).First();
                    dtRow[(int)Global.enPFLaunchMEqRoles.iLaunchMethod] = qLaunchMethod.ID;
                    dtRow[(int)Global.enPFLaunchMEqRoles.sLaunchMethod] = qLaunchMethod.sLaunchMethod;
                    var qEqRole = (from q in stdc.EQUIPMENTROLEs orderby q.sEquipmentRole select q).First();
                    dtRow[(int)Global.enPFLaunchMEqRoles.iEqRole] = qEqRole.ID;
                    dtRow[(int)Global.enPFLaunchMEqRoles.sEqRole] = qEqRole.sEquipmentRole;
                    dtRow[(int)Global.enPFLaunchMEqRoles.sComments] = "";

                    liLMEqR.Add(dtRow);

                    return liLMEqR;
                #endregion

                #region Bridges between Website User Roles and User-Selectable Settings
                case Global.enLL.UserRolesSettings:
                    List<DataRow> liURS = new List<DataRow>();
                    dtPattern = dtSchema(Global.enLL.UserRolesSettings);
                    AdminPages.DBMaint.DataIntegrityDataContext didc = new AdminPages.DBMaint.DataIntegrityDataContext();
                    var qURS = from u in didc.SETTINGSROLESBRIDGEs
                               orderby u.aspnet_Role.RoleName,u.SETTING.sSettingName
                             select new
                             {
                                 u.uiRole,
                                 u.aspnet_Role.RoleName,
                                 u.ID,
                                 u.sComments,
                                 iSetting = u.SETTING.ID,
                                 u.SETTING.sSettingName
                             };
                    foreach (var q in qURS)
                    {
                        dtRow = dtPattern.NewRow();
                        dtRow[(int)Global.enPFUserRolesSettings.ID] = q.ID;
                        dtRow[(int)Global.enPFUserRolesSettings.uiRole] = q.uiRole;
                        dtRow[(int)Global.enPFUserRolesSettings.RoleName] = q.RoleName;
                        dtRow[(int)Global.enPFUserRolesSettings.iSetting] = q.iSetting;
                        dtRow[(int)Global.enPFUserRolesSettings.sSettingName] = q.sSettingName;
                        dtRow[(int)Global.enPFUserRolesSettings.sComments] = q.sComments;

                        liURS.Add(dtRow);
                    }
                    //New Row
                    dtRow = dtPattern.NewRow();
                    dtRow[(int)Global.enPFUserRolesSettings.ID] = 0;
                    AdminPages.DBMaint.aspnet_Role role;
                    var qRoles = (from u in didc.aspnet_Roles orderby u.RoleName select u).ToList();
                    var LiRole = qRoles.Where(asp_net => asp_net.RoleName == "Member");
                    if (LiRole.Count() < 1)
                    {
                        role = qRoles.First();
                    }
                    else
                    {
                        role = LiRole.First();
                    }
                    dtRow[(int)Global.enPFUserRolesSettings.uiRole] = role.RoleId;
                    dtRow[(int)Global.enPFUserRolesSettings.RoleName] = role.RoleName;
                    var qSettings = (from s in didc.SETTINGs where s.bUserSelectable == true orderby s.sSettingName select s).First();
                    dtRow[(int)Global.enPFUserRolesSettings.iSetting] = qSettings.ID;
                    dtRow[(int)Global.enPFUserRolesSettings.sSettingName] = qSettings.sSettingName;
                    dtRow[(int)Global.enPFUserRolesSettings.sComments] = "";

                    liURS.Add(dtRow);

                    return liURS;
                #endregion

                #region Operational Calendar Names
                case Global.enLL.OpsCalendarNames:
                    List<DataRow> liOpsCalNames = new List<DataRow>();
                    dtPattern = dtSchema(Global.enLL.OpsCalendarNames);
                    eqdc = new TSoar.Equipment.EquipmentDataContext();
                    var qoc = from o in eqdc.OPSCALNAMEs select o;
                    //Existing rows
                    foreach(var o in qoc)
                    {
                        dtRow = dtPattern.NewRow();
                        dtRow[(int)Global.enPFOpsCalNames.i1Sort] = 0;
                        dtRow[(int)Global.enPFOpsCalNames.ID] = o.ID;
                        dtRow[(int)Global.enPFOpsCalNames.sOpsCalName] = o.sOpsCalName;
                        dtRow[(int)Global.enPFOpsCalNames.bStandard] = o.bStandard;

                        liOpsCalNames.Add(dtRow);
                    }
                    //New row
                    dtRow = dtPattern.NewRow();
                    dtRow[(int)Global.enPFOpsCalNames.i1Sort] = 1;
                    dtRow[(int)Global.enPFOpsCalNames.ID] = 0;
                    dtRow[(int)Global.enPFOpsCalNames.sOpsCalName] = "";
                    dtRow[(int)Global.enPFOpsCalNames.bStandard] = false;

                    liOpsCalNames.Add(dtRow);
                    return liOpsCalNames;
                #endregion

                #region Equipment Components
                case Global.enLL.EquipComponents:
                    List<DataRow> liEqComp = new List<DataRow>();
                    dtPattern = dtSchema(Global.enLL.EquipComponents);
                    eqdc = new TSoar.Equipment.EquipmentDataContext();
                    var qe = from q in eqdc.sf_EqCompList()
                             orderby q.Sort
                             select new
                             {
                                 q.ID,
                                 EqId = q.iEquipment,
                                 q.sShortEquipName,
                                 q.sComponent,
                                 q.sCName,
                                 q.bEntire,
                                 q.DLinkBegin,
                                 q.DLinkEnd,
                                 q.bReportOperStatus,
                                 q.sOperStatus,
                                 q.sComment,
                                 q.iParentComponent
                             };
                    //Existing Rows
                    foreach (var q in qe)
                    {
                        dtRow = dtPattern.NewRow();
                        dtRow[(int)Global.enPFEqComponents.i1Sort] = 0;
                        dtRow[(int)Global.enPFEqComponents.ID] = q.ID;
                        dtRow[(int)Global.enPFEqComponents.iEquipment] = q.EqId;
                        dtRow[(int)Global.enPFEqComponents.sShortEquipName] = q.sShortEquipName;
                        dtRow[(int)Global.enPFEqComponents.sComponent] = q.sComponent;
                        dtRow[(int)Global.enPFEqComponents.sCName] = q.sCName;
                        dtRow[(int)Global.enPFEqComponents.bEntire] = q.bEntire;
                        dtRow[(int)Global.enPFEqComponents.DLinkBegin] = q.DLinkBegin;
                        dtRow[(int)Global.enPFEqComponents.DLinkEnd] = q.DLinkEnd;
                        dtRow[(int)Global.enPFEqComponents.bReportOperStatus] = q.bReportOperStatus; // SCR 231
                        dtRow[(int)Global.enPFEqComponents.sOperStatus] = q.sOperStatus; // SCR 231
                        dtRow[(int)Global.enPFEqComponents.sComment] = q.sComment;
                        dtRow[(int)Global.enPFEqComponents.iParentComponent] = q.iParentComponent;

                        liEqComp.Add(dtRow);
                    }
                    //New Row
                    dtRow = dtPattern.NewRow();
                    dtRow[(int)Global.enPFEqComponents.i1Sort] = 1;
                    dtRow[(int)Global.enPFEqComponents.ID] = 0;
                    var qf = (from e in eqdc.EQUIPMENTs select e).FirstOrDefault();
                    if (qf == null)
                    {
                        dtRow[(int)Global.enPFEqComponents.iEquipment] = 0;
                        dtRow[(int)Global.enPFEqComponents.sShortEquipName] = "";
                    }
                    else
                    {
                        dtRow[(int)Global.enPFEqComponents.iEquipment] = qf.ID;
                        dtRow[(int)Global.enPFEqComponents.sShortEquipName] = qf.sShortEquipName;
                    }
                    dtRow[(int)Global.enPFEqComponents.sComponent] = "";
                    dtRow[(int)Global.enPFEqComponents.sCName] = "";
                    dtRow[(int)Global.enPFEqComponents.bEntire] = true;
                    string sOffset = mCrud.GetSetting("TimeZoneOffset");
                    int iOffsetHrs = Int32.Parse(sOffset.Substring(0, 3));
                    int iOffsetMin = Int32.Parse(sOffset.Substring(4, 2));
                    dtRow[(int)Global.enPFEqComponents.DLinkBegin] = new DateTimeOffset(1900, 1, 1, 1, 1, 0, new TimeSpan(iOffsetHrs, iOffsetMin, 0));
                    dtRow[(int)Global.enPFEqComponents.DLinkEnd] = new DateTimeOffset(2999, 12, 31, 22, 59, 0, new TimeSpan(iOffsetHrs, iOffsetMin, 0));
                    dtRow[(int)Global.enPFEqComponents.bReportOperStatus] = true; // SCR 231
                    dtRow[(int)Global.enPFEqComponents.sOperStatus] = "OK"; // SCR 231
                    dtRow[(int)Global.enPFEqComponents.sComment] = "";
                    dtRow[(int)Global.enPFEqComponents.iParentComponent] = 0; // a zero here is good only when bEntire is true
                    liEqComp.Add(dtRow);

                    return liEqComp;
                #endregion

                #region Equipment Status // SCR 218
                case Global.enLL.EquipStatus:
                    List<DataRow> liEqStat = new List<DataRow>();
                    dtPattern = dtSchema(Global.enLL.EquipStatus);
                    eqdc = new TSoar.Equipment.EquipmentDataContext();
                    eqdc.CommandTimeout = 30;
                    var qst = from q in eqdc.tf_EqStatList() orderby q.sSorter select q;
                    //Existing Rows only:
                    foreach(var q in qst)
                    {
                        dtRow = dtPattern.NewRow();
                        dtRow[(int)Global.enPFEqStatus.iLine] = q.iLine;
                        dtRow[(int)Global.enPFEqStatus.ID] = q.ID_Compon;
                        dtRow[(int)Global.enPFEqStatus.sShortEquipName] = q.sShortEquipName;
                        dtRow[(int)Global.enPFEqStatus.sRegistrId] = q.sRegistrationId;
                        dtRow[(int)Global.enPFEqStatus.sCName] = q.sCName;
                        dtRow[(int)Global.enPFEqStatus.sOperStat] = q.sOperStatus;
                        dtRow[(int)Global.enPFEqStatus.sComment] = q.sComment;
                        dtRow[(int)Global.enPFEqStatus.sSorter] = q.sSorter;
                        if (q.sError != "OK")
                        {
                            throw new Global.excToPopup(q.sError);
                        }
                        dtRow[(int)Global.enPFEqStatus.sError] = q.sError;
                        dtRow[(int)Global.enPFEqStatus.DLastUpdated] = q.DUnderlLast;
                        dtRow[(int)Global.enPFEqStatus.sDebug] = q.sDebug;
                        dtRow[(int)Global.enPFEqStatus.dAccumHours] = q.dAccumHours;
                        dtRow[(int)Global.enPFEqStatus.iAccumCycles] = q.iAccumCycles;
                        dtRow[(int)Global.enPFEqStatus.dAccumDist] = q.dAccumDist;
                        dtRow[(int)Global.enPFEqStatus.sActionItem] = q.sActionItem;
                        dtRow[(int)Global.enPFEqStatus.sDeadline] = q.sDeadline;
                        dtRow[(int)Global.enPFEqStatus.cPrevSchedMeth] = q.cPrevailSchedMeth;
                        dtRow[(int)Global.enPFEqStatus.iPercentComplete] = q.iPercentComplete;
                        liEqStat.Add(dtRow);
                    }
                    return liEqStat;
                #endregion

                #region Equipment Parameters
                case Global.enLL.EquipParameters:
                    List<DataRow> liEqPars = new List<DataRow>();
                    dtPattern = dtSchema(Global.enLL.EquipParameters);
                    eqdc = new TSoar.Equipment.EquipmentDataContext();
                    var qs = from q in eqdc.EQUIPAGINGPARs
                             select new
                             {
                                 q.ID,
                                 q.sShortDescript,
                                 iEqAcTy = q.EQUIPACTIONTYPE.ID,
                                 q.EQUIPACTIONTYPE.sEquipActionType,
                                 q.iIntervalElapsed,
                                 q.sTimeUnitsElapsed,
                                 q.cDeadLineMode,
                                 q.iDeadLineSpt1,
                                 q.iDeadLineSpt2,
                                 q.iIntervalOperating,
                                 q.sTimeUnitsOperating,
                                 q.iIntervalDistance,
                                 q.sDistanceUnits,
                                 q.sComment
                             };
                    //Existing Rows
                    foreach (var q in qs)
                    {
                        dtRow = dtPattern.NewRow();
                        dtRow[(int)Global.enPFEqParameters.i1Sort] = 0;
                        dtRow[(int)Global.enPFEqParameters.ID] = q.ID;
                        dtRow[(int)Global.enPFEqParameters.sShortDescript] = q.sShortDescript;
                        dtRow[(int)Global.enPFEqParameters.iEquipActionType] = q.iEqAcTy;
                        dtRow[(int)Global.enPFEqParameters.sEquipActionType] = q.sEquipActionType;
                        dtRow[(int)Global.enPFEqParameters.iIntervalElapsed] = q.iIntervalElapsed;
                        dtRow[(int)Global.enPFEqParameters.sTimeUnitsElapsed] = q.sTimeUnitsElapsed;
                        dtRow[(int)Global.enPFEqParameters.cDeadLineMode] = q.cDeadLineMode;
                        dtRow[(int)Global.enPFEqParameters.iDeadLineSpt1] = q.iDeadLineSpt1;
                        dtRow[(int)Global.enPFEqParameters.iDeadLineSpt2] = q.iDeadLineSpt2;
                        dtRow[(int)Global.enPFEqParameters.iIntervalOperating] = q.iIntervalOperating;
                        dtRow[(int)Global.enPFEqParameters.sTimeUnitsOperating] = q.sTimeUnitsOperating;
                        dtRow[(int)Global.enPFEqParameters.iIntervalDistance] = q.iIntervalDistance;
                        dtRow[(int)Global.enPFEqParameters.sDistanceUnits] = q.sDistanceUnits;
                        dtRow[(int)Global.enPFEqParameters.sComment] = q.sComment;

                        liEqPars.Add(dtRow);
                    }
                    //New Row
                    dtRow = dtPattern.NewRow();
                    dtRow[(int)Global.enPFEqParameters.i1Sort] = 1;
                    dtRow[(int)Global.enPFEqParameters.ID] = 0;
                    dtRow[(int)Global.enPFEqParameters.sShortDescript] = "";
                    Equipment.EQUIPACTIONTYPE qu = null;
                    try
                    {
                        qu = (from e in eqdc.EQUIPACTIONTYPEs select e).First();
                    } catch
                    {
                        throw new Global.excToPopup("Looks like no equipment action types are defined. Check whether table EQUIPACTIONTYPES is empty.");
                    }
                    dtRow[(int)Global.enPFEqParameters.iEquipActionType] = qu.ID;
                    dtRow[(int)Global.enPFEqParameters.sEquipActionType] = qu.sEquipActionType;
                    dtRow[(int)Global.enPFEqParameters.iIntervalElapsed] = -1;
                    dtRow[(int)Global.enPFEqParameters.sTimeUnitsElapsed] = "Months";
                    dtRow[(int)Global.enPFEqParameters.cDeadLineMode] = 'L';
                    dtRow[(int)Global.enPFEqParameters.iDeadLineSpt1] = -1;
                    dtRow[(int)Global.enPFEqParameters.iDeadLineSpt2] = -1;
                    dtRow[(int)Global.enPFEqParameters.iIntervalOperating] = 1000;
                    dtRow[(int)Global.enPFEqParameters.sTimeUnitsOperating] = "Hours";
                    dtRow[(int)Global.enPFEqParameters.iIntervalDistance] = 12000;
                    dtRow[(int)Global.enPFEqParameters.sDistanceUnits] = "Miles";
                    dtRow[(int)Global.enPFEqParameters.sComment] = "";

                    liEqPars.Add(dtRow);

                    return liEqPars;
                #endregion

                #region Equipment Aging Items
                case Global.enLL.EquipAgingItems:
                    List<DataRow> liEqAgItems = new List<DataRow>();
                    dtPattern = dtSchema(Global.enLL.EquipAgingItems);
                    eqdc = new TSoar.Equipment.EquipmentDataContext();
                    var qw = from q in eqdc.EQUIPAGINGITEMs
                             select new
                             {
                                 q.ID,
                                 q.sName,
                                 q.iEquipComponent,
                                 q.EQUIPCOMPONENT.sComponent,
                                 q.EQUIPCOMPONENT.EQUIPMENT.sShortEquipName,
                                 q.iParam,
                                 q.iOpCal,
                                 q.OPSCALNAME.sOpsCalName,
                                 q.EQUIPAGINGPAR.sShortDescript,
                                 q.DStart,
                                 q.DEnd,
                                 q.dEstRunDays,
                                 q.bRunExtrap,
                                 q.dEstCycleDays,
                                 q.bCyclExtrap,
                                 q.dEstDistDays,
                                 q.bDistExtrap,
                                 q.dEstDuration,
                                 q.sComment
                             };
                    //Existing Rows
                    foreach (var q in qw)
                    {
                        dtRow = dtPattern.NewRow();
                        dtRow[(int)Global.enPFEqAgingItems.i1Sort] = 0;
                        dtRow[(int)Global.enPFEqAgingItems.ID] = q.ID;
                        dtRow[(int)Global.enPFEqAgingItems.sName] = q.sName;
                        dtRow[(int)Global.enPFEqAgingItems.iEquipComponent] = q.iEquipComponent;
                        dtRow[(int)Global.enPFEqAgingItems.sComponent] = q.sComponent;
                        dtRow[(int)Global.enPFEqAgingItems.iParam] = q.iParam;
                        dtRow[(int)Global.enPFEqAgingItems.iOpCal] = q.iOpCal;
                        dtRow[(int)Global.enPFEqAgingItems.sOpsCalName] = q.sOpsCalName;
                        dtRow[(int)Global.enPFEqAgingItems.sShortDescript] = q.sShortDescript;
                        dtRow[(int)Global.enPFEqAgingItems.DStart] = q.DStart;
                        dtRow[(int)Global.enPFEqAgingItems.DEnd] = q.DEnd;
                        dtRow[(int)Global.enPFEqAgingItems.dEstRunDays] = q.dEstRunDays;
                        dtRow[(int)Global.enPFEqAgingItems.bRunExtrap] = q.bRunExtrap;
                        dtRow[(int)Global.enPFEqAgingItems.dEstCycleDays] = q.dEstCycleDays;
                        dtRow[(int)Global.enPFEqAgingItems.bCyclExtrap] = q.bCyclExtrap;
                        dtRow[(int)Global.enPFEqAgingItems.dEstDistDays] = q.dEstDistDays;
                        dtRow[(int)Global.enPFEqAgingItems.bDistExtrap] = q.bDistExtrap;
                        dtRow[(int)Global.enPFEqAgingItems.dEstDuration] = q.dEstDuration;
                        dtRow[(int)Global.enPFEqAgingItems.sComment] = q.sComment;

                        liEqAgItems.Add(dtRow);
                    }
                    //New Row
                    dtRow = dtPattern.NewRow();
                    // If there are either no aging parameters sets defined, or no equipment components defined,
                    //     then it is not possible to define any aging items
                    var qwe = (from e in eqdc.EQUIPCOMPONENTs select e).ToList();
                    if (qwe.Count() > 0)
                    {
                        dtRow[(int)Global.enPFEqAgingItems.i1Sort] = 1;
                        dtRow[(int)Global.enPFEqAgingItems.ID] = 0;
                        dtRow[(int)Global.enPFEqAgingItems.sName] = "";
                        dtRow[(int)Global.enPFEqAgingItems.iEquipComponent] = qwe.First().ID;
                        dtRow[(int)Global.enPFEqAgingItems.sComponent] = qwe.First().sComponent;
                        var qwp = (from p in eqdc.EQUIPAGINGPARs select p).ToList();
                        if (qwp.Count() > 0)
                        {
                            //var qwp = (from p in eqdc.EQUIPAGINGPARs select p).First();
                            dtRow[(int)Global.enPFEqAgingItems.iParam] = qwp.First().ID;
                            dtRow[(int)Global.enPFEqAgingItems.sShortDescript] = qwp.First().sShortDescript;
                        }
                        sOffset = mCrud.GetSetting("TimeZoneOffset");
                        iOffsetHrs = Int32.Parse(sOffset.Substring(0, 3));
                        iOffsetMin = Int32.Parse(sOffset.Substring(4, 2));
                        dtRow[(int)Global.enPFEqAgingItems.DStart] = new DateTimeOffset(2001, 1, 1, 1, 1, 0, new TimeSpan(iOffsetHrs, iOffsetMin, 0));
                        dtRow[(int)Global.enPFEqAgingItems.DEnd] = new DateTimeOffset(2999, 12, 31, 22, 59, 0, new TimeSpan(iOffsetHrs, iOffsetMin, 0));
                        dtRow[(int)Global.enPFEqAgingItems.dEstRunDays] = 365.25m;
                        dtRow[(int)Global.enPFEqAgingItems.bRunExtrap] = false;
                        dtRow[(int)Global.enPFEqAgingItems.dEstCycleDays] = 365.25m;
                        dtRow[(int)Global.enPFEqAgingItems.bCyclExtrap] = false;
                        dtRow[(int)Global.enPFEqAgingItems.dEstDistDays] = 365.25m;
                        dtRow[(int)Global.enPFEqAgingItems.bDistExtrap] = false;
                        dtRow[(int)Global.enPFEqAgingItems.dEstDuration] = 30.00m;
                        dtRow[(int)Global.enPFEqAgingItems.sComment] = "";
                        var ocn = (from c in eqdc.OPSCALNAMEs where c.bStandard select c).First();
                        dtRow[(int)Global.enPFEqAgingItems.iOpCal] = ocn.ID;

                        liEqAgItems.Add(dtRow);
                    }
                    return liEqAgItems;
                #endregion

                #region Equipment Action Items
                case Global.enLL.EquipActionItems:
                    List<DataRow> liEqActItems = new List<DataRow>();
                    dtPattern = dtSchema(Global.enLL.EquipActionItems);
                    eqdc = new TSoar.Equipment.EquipmentDataContext();
                    var qy = from q in eqdc.TNPV_EquipActionItems
                             select new
                             {
                                 q.ID,
                                 q.iEquipAgingItem,
                                 q.sName,
                                 q.DeadLine,
                                 q.dDeadlineHrs,
                                 q.iDeadLineCycles,
                                 q.dDeadLineDist,
                                 q.DScheduledStart,
                                 q.DActualStart,
                                 q.iPercentComplete,
                                 q.DComplete,
                                 q.dAtCompletionHrs,
                                 q.iAtCompletionCycles,
                                 q.dAtCompletionDist,
                                 q.sComment,
                                 q.sUpdateStatus
                             };
                    //Existing Rows
                    foreach (var q in qy)
                    {
                        dtRow = dtPattern.NewRow();
                        dtRow[(int)Global.enPFEqActionItems.i1Sort] = 0;
                        dtRow[(int)Global.enPFEqActionItems.ID] = q.ID;
                        dtRow[(int)Global.enPFEqActionItems.iEquipAgingItem] = q.iEquipAgingItem;
                        dtRow[(int)Global.enPFEqActionItems.sName] = q.sName;
                        dtRow[(int)Global.enPFEqActionItems.DeadLine] = q.DeadLine;
                        dtRow[(int)Global.enPFEqActionItems.dDeadlineHrs] = q.dDeadlineHrs;
                        dtRow[(int)Global.enPFEqActionItems.iDeadLineCycles] = q.iDeadLineCycles;
                        dtRow[(int)Global.enPFEqActionItems.dDeadLineDist] = q.dDeadLineDist;
                        dtRow[(int)Global.enPFEqActionItems.DScheduledStart] = q.DScheduledStart;
                        dtRow[(int)Global.enPFEqActionItems.DActualStart] = q.DActualStart;
                        dtRow[(int)Global.enPFEqActionItems.iPercentComplete] = q.iPercentComplete;
                        dtRow[(int)Global.enPFEqActionItems.DComplete] = q.DComplete;
                        dtRow[(int)Global.enPFEqActionItems.dAtCompletionHrs] = q.dAtCompletionHrs;
                        dtRow[(int)Global.enPFEqActionItems.iAtCompletionCycles] = q.iAtCompletionCycles;
                        dtRow[(int)Global.enPFEqActionItems.dAtCompletionDist] = q.dAtCompletionDist;

                        dtRow[(int)Global.enPFEqActionItems.sComment] = q.sComment;
                        dtRow[(int)Global.enPFEqActionItems.sUpdateStatus] = q.sUpdateStatus;

                        liEqActItems.Add(dtRow);
                    }
                    //New Row (even though it is never used; action items do not get created by the user directly)
                    dtRow = dtPattern.NewRow();
                    dtRow[(int)Global.enPFEqActionItems.i1Sort] = 1;
                    dtRow[(int)Global.enPFEqActionItems.ID] = 0;
                    dtRow[(int)Global.enPFEqActionItems.iEquipAgingItem] = 0;
                    dtRow[(int)Global.enPFEqActionItems.sName] = "";
                    dtRow[(int)Global.enPFEqActionItems.DeadLine] = DateTimeOffset.MaxValue;
                    dtRow[(int)Global.enPFEqActionItems.dDeadlineHrs] = 99999999.9999M;
                    dtRow[(int)Global.enPFEqActionItems.iDeadLineCycles] = 99999999;
                    dtRow[(int)Global.enPFEqActionItems.dDeadLineDist] = 99999999.99M;
                    dtRow[(int)Global.enPFEqActionItems.DScheduledStart] = Global.DTO_NotStarted;
                    dtRow[(int)Global.enPFEqActionItems.DActualStart] = Global.DTO_NotStarted;
                    dtRow[(int)Global.enPFEqActionItems.iPercentComplete] = 0;
                    dtRow[(int)Global.enPFEqActionItems.DComplete] = Global.DTO_NotCompleted;
                    dtRow[(int)Global.enPFEqActionItems.dAtCompletionHrs] = -9.99m;
                    dtRow[(int)Global.enPFEqActionItems.iAtCompletionCycles] = -999;
                    dtRow[(int)Global.enPFEqActionItems.dAtCompletionDist] = -9.99m;
                    dtRow[(int)Global.enPFEqActionItems.sComment] = "";
                    dtRow[(int)Global.enPFEqActionItems.sUpdateStatus] = "";

                    liEqActItems.Add(dtRow);

                    return liEqActItems;
                #endregion

                #region Member Equity Shares Journal
                case Global.enLL.MeEquityShJ:
                    List<DataRow> liMeEquityShJ = new List<DataRow>();
                    dtPattern = dtSchema(Global.enLL.MeEquityShJ);
                    // Existing Rows
                    ClubMembership.ClubMembershipDataContext mqdc = new ClubMembership.ClubMembershipDataContext();
                    var qsh = from s in mqdc.EQUITYSHAREs select new { s.ID, s.iOwner, s.PEOPLE.sDisplayName, s.DXaction, s.cDateQuality, s.dNumShares, s.cXactType,
                        s.sInfoSource, s.sComment};
                    foreach (var s in qsh)
                    {
                        dtRow = dtPattern.NewRow();
                        dtRow[(int)Global.enPFMeEquityShJ.ID] = s.ID;
                        dtRow[(int)Global.enPFMeEquityShJ.iOwner] = s.iOwner;
                        dtRow[(int)Global.enPFMeEquityShJ.sDisplayName] = s.sDisplayName;
                        dtRow[(int)Global.enPFMeEquityShJ.DXaction] = s.DXaction;
                        dtRow[(int)Global.enPFMeEquityShJ.cDateQuality] = s.cDateQuality;
                        dtRow[(int)Global.enPFMeEquityShJ.dNumShares] = s.dNumShares;
                        dtRow[(int)Global.enPFMeEquityShJ.cXactType] = s.cXactType;
                        dtRow[(int)Global.enPFMeEquityShJ.sInfoSource] = s.sInfoSource;
                        dtRow[(int)Global.enPFMeEquityShJ.sComment] = s.sComment;

                        liMeEquityShJ.Add(dtRow);
                    }
                    //New Row
                    dtRow = dtPattern.NewRow();
                    dtRow[(int)Global.enPFMeEquityShJ.ID] = 0;
                    dtRow[(int)Global.enPFMeEquityShJ.iOwner] = 0;
                    dtRow[(int)Global.enPFMeEquityShJ.sDisplayName] = "";
                    dtRow[(int)Global.enPFMeEquityShJ.DXaction] = DateTime.Now.Date;
                    dtRow[(int)Global.enPFMeEquityShJ.cDateQuality] = 'E';
                    dtRow[(int)Global.enPFMeEquityShJ.dNumShares] = 10.0M;
                    dtRow[(int)Global.enPFMeEquityShJ.cXactType] = 'P';
                    dtRow[(int)Global.enPFMeEquityShJ.sInfoSource] = "";
                    dtRow[(int)Global.enPFMeEquityShJ.sComment] = "";

                    liMeEquityShJ.Add(dtRow);

                    return liMeEquityShJ;
                #endregion

                #region Flight Operations Schedule Dates
                case Global.enLL.FltOpsSchedDates:
                    List<DataRow> liFOSDates = new List<DataRow>();
                    dtPattern = dtSchema(euLL);
                    TSoar.Operations.OpsSchedDataContext FOSdc = new TSoar.Operations.OpsSchedDataContext();
                    var qFOSDates = from d in FOSdc.FSDATEs select d;
                    //Existing rows
                    foreach(var d in qFOSDates)
                    {
                        dtRow = dtPattern.NewRow();
                        dtRow[(int)Global.enPFFltOpsSchedDates.i1Sort] = 0;
                        dtRow[(int)Global.enPFFltOpsSchedDates.ID] = d.ID;
                        dtRow[(int)Global.enPFFltOpsSchedDates.Date] = d.Date;
                        dtRow[(int)Global.enPFFltOpsSchedDates.bEnabled] = d.bEnabled;
                        dtRow[(int)Global.enPFFltOpsSchedDates.sNote] = d.sNote;

                        liFOSDates.Add(dtRow);
                    }
                    // New Row
                    dtRow = dtPattern.NewRow();
                    dtRow[(int)Global.enPFFltOpsSchedDates.i1Sort] = 1;
                    dtRow[(int)Global.enPFFltOpsSchedDates.ID] = 0;
                    dtRow[(int)Global.enPFFltOpsSchedDates.Date] = DateTime.Today.AddDays(60.0);
                    dtRow[(int)Global.enPFFltOpsSchedDates.bEnabled] = true;
                    dtRow[(int)Global.enPFFltOpsSchedDates.sNote] = "";

                    liFOSDates.Add(dtRow);
                    return liFOSDates;
                #endregion

                default:
                    return null;
            }
        }

        static public List<DataRow> Init(Global.enLL euLL, DataTable udtFilters)
        {
            DataTable dtPattern = new DataTable();
            DataRow dtRow;
            switch (euLL)
            {
                #region Daily Flight Logs
                case Global.enLL.DailyFlightLogs:
                    List<DataRow> liDFlLog = new List<DataRow>();
                    dtPattern = dtSchema(Global.enLL.DailyFlightLogs);
                    TSoar.Statistician.StatistDailyFlightLogDataContext stdc = new TSoar.Statistician.StatistDailyFlightLogDataContext();
                    var qdf = from q in stdc.DAILYFLIGHTLOGs
                              where q.DFlightOps.Year == (int)udtFilters.Rows[0]["Year"] && q.DFlightOps.Month == (int)udtFilters.Rows[0]["Month"]
                              orderby q.DFlightOps
                              select new
                              {
                                  q.ID,
                                  iFlightCount = (from r in stdc.FLIGHTLOGROWs where r.iFliteLog == q.ID select r).Count(),
                                  iFlightsPosted = (from r in stdc.FLIGHTLOGROWs where r.iFliteLog == q.ID && r.cStatus == 'P' select r).Count(),
                                  q.DFlightOps,
                                  q.sFldMgr,
                                  q.iMainTowEquip,
                                  q.EQUIPMENT.sShortEquipName,
                                  q.iMainTowOp,
                                  q.PEOPLE.sDisplayName,
                                  q.iMainGlider,
                                  sMainGliderName = (from g in stdc.EQUIPMENTs where g.ID == q.iMainGlider select g.sShortEquipName).First(),
                                  q.iMainLaunchMethod,
                                  sMainLaunchMethod = (from l in stdc.LAUNCHMETHODs where l.ID == q.iMainLaunchMethod select l.sLaunchMethod).First(),
                                  q.iMainLocation,
                                  q.LOCATION.sLocation,
                                  mTcoll = ((from t in stdc.FLIGHTLOGROWs where t.iFliteLog == q.ID select t.mAmtCollected).Sum() == null)
                                            ? 0.00m : (from t in stdc.FLIGHTLOGROWs where t.iFliteLog == q.ID select t.mAmtCollected).Sum(),
                                  q.sNotes
                              };
                    //Existing rows
                    foreach (var q in qdf)
                    {
                        dtRow = dtPattern.NewRow();
                        dtRow[(int)Global.enPFDailyFlightLogs.ID] = q.ID;
                        dtRow[(int)Global.enPFDailyFlightLogs.iFlightCount] = q.iFlightCount;
                        dtRow[(int)Global.enPFDailyFlightLogs.iFlightsPosted] = q.iFlightsPosted;
                        dtRow[(int)Global.enPFDailyFlightLogs.DFlightOps] = q.DFlightOps;
                        dtRow[(int)Global.enPFDailyFlightLogs.sFldMgr] = q.sFldMgr;
                        dtRow[(int)Global.enPFDailyFlightLogs.iMainTowEquip] = q.iMainTowEquip;
                        dtRow[(int)Global.enPFDailyFlightLogs.sShortEquipName] = q.sShortEquipName;
                        dtRow[(int)Global.enPFDailyFlightLogs.iMainTowOp] = q.iMainTowOp;
                        dtRow[(int)Global.enPFDailyFlightLogs.sDisplayName] = q.sDisplayName;
                        dtRow[(int)Global.enPFDailyFlightLogs.iMainGlider] = q.iMainGlider;
                        dtRow[(int)Global.enPFDailyFlightLogs.sMainGliderName] = q.sMainGliderName;
                        dtRow[(int)Global.enPFDailyFlightLogs.iMainLaunchMethod] = q.iMainLaunchMethod;
                        dtRow[(int)Global.enPFDailyFlightLogs.sMainLaunchMethod] = q.sMainLaunchMethod;
                        dtRow[(int)Global.enPFDailyFlightLogs.iMainLocation] = q.iMainLocation;
                        dtRow[(int)Global.enPFDailyFlightLogs.sLocation] = q.sLocation;
                        dtRow[(int)Global.enPFDailyFlightLogs.mTotalCollected] = q.mTcoll;
                        dtRow[(int)Global.enPFDailyFlightLogs.sNotes] = q.sNotes;

                        liDFlLog.Add(dtRow);
                    }
                    //New row
                    dtRow = dtPattern.NewRow();
                    dtRow[(int)Global.enPFDailyFlightLogs.ID] = 0;
                    dtRow[(int)Global.enPFDailyFlightLogs.iFlightCount] = 0;
                    dtRow[(int)Global.enPFDailyFlightLogs.iFlightsPosted] = 0;
                    dtRow[(int)Global.enPFDailyFlightLogs.DFlightOps] = DateTimeOffset.Now;
                    dtRow[(int)Global.enPFDailyFlightLogs.sFldMgr] = "";

                    // Default strings
                    string sDefTowEq;
                    string sDefTowOp;
                    string sDefGlider;
                    string sDefLaunchMethod;
                    string sDefLoc;
                    if (AccountProfile.CurrentUser.DailyFlightLogsDefaults.Rows.Count > 0)
                    {
                        DataTable dtLastUsedIn = AccountProfile.CurrentUser.DailyFlightLogsDefaults;
                        sDefTowEq = (string)dtLastUsedIn.Rows[0].ItemArray[1];
                        sDefTowOp = (string)dtLastUsedIn.Rows[0].ItemArray[2];
                        sDefGlider = (string)dtLastUsedIn.Rows[0].ItemArray[3];
                        sDefLaunchMethod = (string)dtLastUsedIn.Rows[0].ItemArray[4];
                        sDefLoc = (string)dtLastUsedIn.Rows[0].ItemArray[5];
                    }
                    else
                    {
                        sDefTowEq = (from s in stdc.SETTINGs where s.sSettingName == "DefaultTowEquipment" select s.sSettingValue).First();
                        sDefTowOp = (from s in stdc.SETTINGs where s.sSettingName == "DefaultAviator" select s.sSettingValue).First();
                        sDefGlider = (from s in stdc.SETTINGs where s.sSettingName == "DefaultGlider" select s.sSettingValue).First();
                        sDefLaunchMethod = (from s in stdc.SETTINGs where s.sSettingName == "DefaultLaunchMethod" select s.sSettingValue).First();
                        sDefLoc = (from s in stdc.SETTINGs where s.sSettingName == "DefaultLocation" select s.sSettingValue).First();
                    }

                    //  DefaultTowEquipment
                    try
                    {
                        dtRow[(int)Global.enPFDailyFlightLogs.iMainTowEquip] = (from t in stdc.EQUIPMENTs where t.sShortEquipName == sDefTowEq select t.ID).First();
                    }
                    catch (Exception exc)
                    {
                        throw new Global.excToPopup("Error in AssistLi.Init Daily Flight Logs: '" + exc.Message + "'. Possibly missing '" + sDefTowEq + "' in EQUIPMENT.");
                    }
                    dtRow[(int)Global.enPFDailyFlightLogs.sShortEquipName] = sDefTowEq;

                    //  DefaultTowOperator
                    try
                    {
                        dtRow[(int)Global.enPFDailyFlightLogs.iMainTowOp] = (from t in stdc.PEOPLEs where t.sDisplayName == sDefTowOp select t.ID).First();
                    }
                    catch (Exception exc)
                    {
                        throw new Global.excToPopup("Error in AssistLi.Init Daily Flight Logs: '" + exc.Message + "'. Possibly missing '" + sDefTowOp + "' in PEOPLE.");
                    }
                    dtRow[(int)Global.enPFDailyFlightLogs.sDisplayName] = sDefTowOp;

                    //  DefaultGlider
                    try
                    {
                        dtRow[(int)Global.enPFDailyFlightLogs.iMainGlider] = (from t in stdc.EQUIPMENTs where t.sShortEquipName == sDefGlider select t.ID).First();
                    }
                    catch (Exception exc)
                    {
                        throw new Global.excToPopup("Error in AssistLi.Init Daily Flight Logs: '" + exc.Message + "'. Possibly missing '" + sDefGlider + "' in EQUIPMENT.");
                    }
                    dtRow[(int)Global.enPFDailyFlightLogs.sMainGliderName] = sDefGlider;

                    //  DefaultLaunchMethod
                    try
                    {
                        dtRow[(int)Global.enPFDailyFlightLogs.iMainLaunchMethod] = (from t in stdc.LAUNCHMETHODs where t.sLaunchMethod == sDefLaunchMethod select t.ID).First();
                    }
                    catch (Exception exc)
                    {
                        throw new Global.excToPopup("Error in AssistLi.Init Daily Flight Logs: '" + exc.Message + "'. Possibly missing '" + sDefLaunchMethod + "' in LAUNCHMETHODS.");
                    }
                    dtRow[(int)Global.enPFDailyFlightLogs.sMainGliderName] = sDefLaunchMethod;

                    //  DefaultLocation
                    try
                    {
                        dtRow[(int)Global.enPFDailyFlightLogs.iMainLocation] = (from t in stdc.LOCATIONs where t.sLocation == sDefLoc select t.ID).First();
                    }
                    catch (Exception exc)
                    {
                        throw new Global.excToPopup("Error in AssistLi.Init Daily Flight Logs: '" + exc.Message + "'. Possibly missing '" + sDefLoc + "' in LOCATIONS.");
                    }
                    dtRow[(int)Global.enPFDailyFlightLogs.sLocation] = sDefLoc;

                    dtRow[(int)Global.enPFDailyFlightLogs.mTotalCollected] = 0.00m;
                    dtRow[(int)Global.enPFDailyFlightLogs.sNotes] = "";

                    liDFlLog.Add(dtRow);

                    return liDFlLog;
                #endregion

                default:
                    return null;
            }
        }

        static public List<DataRow> Init(Global.enLL euLL, DataTable udtFilters, DateTimeOffset? DuEarn)
        {
            DataTable dtPattern = new DataTable();
            DataRow dtRow;
            switch (euLL)
            {
                #region Rewards
                case Global.enLL.Rewards:
                    List<DataRow> liRewards = new List<DataRow>();
                    dtPattern = dtSchema(Global.enLL.Rewards);
                    using (DataTable dtRewards = new DataTable())
                    {
                        using (SqlConnection SqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlConn"].ToString()))
                        {
                            using (SqlCommand cmd = new SqlCommand("sf_RewardsJournal"))
                            {
                                SqlParameter[] ap = new SqlParameter[3];
                                ap[0] = new SqlParameter("@iDebug", SqlDbType.Int)
                                {
                                    Value = 0,
                                    Direction = ParameterDirection.Input
                                };
                                ap[1] = new SqlParameter("@sStatus", SqlDbType.NVarChar, 350000)
                                {
                                    Value = "undefined",
                                    Direction = ParameterDirection.InputOutput
                                };
                                ap[2] = new SqlParameter("@taFilter", SqlDbType.Structured)
                                {
                                    Value = udtFilters,
                                    Direction = ParameterDirection.Input
                                };
                                cmd.Parameters.AddRange(ap);
                                using (SqlDataAdapter sda = new SqlDataAdapter())
                                {
                                    cmd.CommandType = CommandType.StoredProcedure;
                                    cmd.Connection = SqlConn;
                                    sda.SelectCommand = cmd;
                                    sda.Fill(dtRewards);
                                    string sMsg = (string)ap[1].Value; // @sStatus // SCR 223
                                    if (sMsg.Substring(0, 2) != "OK")
                                    {
                                        throw new Global.excToPopup(sMsg);
                                    }
                                }
                            }
                        }
                        // Existing Rows
                        //    Are we taking records from the top or from the bottom?
                        bool bReverse = false;
                        if ((int)udtFilters.Rows[6][5] == 2)
                        {
                            // from the bottom
                            bReverse = true;
                        }
                        for (int iIter = 0; iIter < dtRewards.Rows.Count; iIter++)
                        {
                            int iSub = iIter;
                            if (bReverse) iSub = dtRewards.Rows.Count - iIter - 1;
                            DataRow r = dtRewards.Rows[iSub];
                            dtRow = dtPattern.NewRow();
                            dtRow[(int)Global.enPFReward.ID] = r["ID"];
                            dtRow[(int)Global.enPFReward.bEC] = r["bEC"];
                            dtRow[(int)Global.enPFReward.DEarn] = r["DEarn"];
                            dtRow[(int)Global.enPFReward.DExpiry] = r["DExpiry"];
                            dtRow[(int)Global.enPFReward.DClaim] = r["DClaim"];
                            dtRow[(int)Global.enPFReward.iPerson] = r["iPerson"];
                            dtRow[(int)Global.enPFReward.sDisplayName] = r["sDisplayName"];
                            dtRow[(int)Global.enPFReward.iServicePts] = r["iServicePts"];
                            dtRow[(int)Global.enPFReward.cECCode] = r["cECCode"];
                            dtRow[(int)Global.enPFReward.bForward] = r["bForward"];
                            dtRow[(int)Global.enPFReward.sComments] = r["sComments"];
                            liRewards.Add(dtRow);
                        }
                        // New row
                        dtRow = dtPattern.NewRow();
                        dtRow[(int)Global.enPFReward.ID] = 0;
                        dtRow[(int)Global.enPFReward.bEC] = 1;

                        DateTimeOffset DEarn = DateTimeOffset.Now.AddDays(-2);
                        if (DuEarn != null)
                        {
                            DEarn = (DateTimeOffset)DuEarn;
                        }
                        dtRow[(int)Global.enPFReward.DEarn] = DEarn;

                        int iYearExpiry = DEarn.Year;
                        if (DEarn.Month > 8) iYearExpiry++;
                        TSoar.DB.SCUD_Multi mCrud = new TSoar.DB.SCUD_Multi();
                        dtRow[(int)Global.enPFReward.DExpiry] = new DateTimeOffset(iYearExpiry, 10, 31, 22, 59, 0, TimeSpan.Parse(mCrud.GetSetting("TimeZoneOffset")));

                        dtRow[(int)Global.enPFReward.DClaim] = DateTimeOffset.Now;
                        dtRow[(int)Global.enPFReward.iPerson] = 0;
                        dtRow[(int)Global.enPFReward.sDisplayName] = "";
                        dtRow[(int)Global.enPFReward.iServicePts] = 0;
                        dtRow[(int)Global.enPFReward.cECCode] = 'T';
                        dtRow[(int)Global.enPFReward.bForward] = false;
                        dtRow[(int)Global.enPFReward.sComments] = "";
                        liRewards.Add(dtRow);

                        return liRewards;
                    }
                #endregion

                default:
                    return null;
            }
        }

        static public List<DataRow> Init(Global.enLL euLL, int iuID)
        {
            //For purposes of editing, initialize a linked list of the type given in euLL from data in the database
            sf_AccountingDataContext dc = new sf_AccountingDataContext();
            DataTable dtPattern = new DataTable();
            SCUD_Multi mCrud = new SCUD_Multi();
            switch (euLL)
            {
                #region Expenditure
                case Global.enLL.Expenditure:
                    List<DataRow> liExpenditure = new List<DataRow>();
                    // Define the schema for DataTable dtPattern
                    dtPattern = dtSchema(Global.enLL.Expenditure);
                    int iLine = 0;
                    decimal mAmount = 0.00M;
                    decimal mSum = 0.00M;

                    // 'Data' rows
                    DataRow dtRow;
                    var q1 = from n in dc.SF_ENTRies
                             where n.iXactId == iuID && n.SF_ACCOUNT.SF_ACCTTYPE.sAccountType == "Expense"
                             select new { n, ac = n.SF_ACCOUNT.sCode + " " + n.SF_ACCOUNT.sName };
                    foreach (var xp in q1)
                    {
                        dtRow = dtPattern.NewRow();
                        dtRow[(int)Global.enPFExp.LineNum] = ++iLine;
                        dtRow[(int)Global.enPFExp.AccountID] = xp.n.iAccountId;
                        dtRow[(int)Global.enPFExp.AccountName] = xp.ac;
                        dtRow[(int)Global.enPFExp.Descr] = xp.n.sDescription;
                        mAmount = xp.n.mAmount;
                        mSum += mAmount;
                        dtRow[(int)Global.enPFExp.sAmount] = mAmount.ToString("N2", CultureInfo.InvariantCulture);
                        liExpenditure.Add(dtRow);
                    }
                    foreach (DataRow row in liExpenditure)
                    {
                        row[(int)Global.enPFExp.LineNum] = iLine--;
                    }

                    // 'Sum' Row
                    dtRow = dtPattern.NewRow();
                    dtRow[(int)Global.enPFExp.LineNum] = 0;
                    dtRow[(int)Global.enPFExp.AccountID] = 0;
                    dtRow[(int)Global.enPFExp.AccountName] = " ";
                    dtRow[(int)Global.enPFExp.Descr] = "Sum of Debits:";
                    dtRow[(int)Global.enPFExp.sAmount] = mSum.ToString("N2", CultureInfo.InvariantCulture);
                    liExpenditure.Add(dtRow);

                    // 'New' Row
                    dtRow = dtPattern.NewRow();
                    dtRow[(int)Global.enPFExp.LineNum] = -1;
                    string sSetting = mCrud.GetSetting("DefaultExpenseAccount");
                    string sCode = sSetting.Split(' ')[0];
                    var q0 = from p in dc.SF_ACCOUNTs where p.sCode == sCode select p;
                    SF_ACCOUNT qr = q0.First();
                    dtRow[(int)Global.enPFExp.AccountID] = qr.ID;
                    dtRow[(int)Global.enPFExp.AccountName] = qr.sName;
                    dtRow[(int)Global.enPFExp.Descr] = " ";
                    dtRow[(int)Global.enPFExp.sAmount] = "";
                    liExpenditure.Add(dtRow);

                    return liExpenditure;
                #endregion
                #region Payment
                case Global.enLL.Payment:
                    List<DataRow> liPayment = new List<DataRow>();
                    dtPattern = dtSchema(Global.enLL.Payment);
                    iLine = 0;
                    mAmount = 0.00M;
                    mSum = 0.00M;

                    // 'Data' rows
                    var q2 = from n in dc.SF_ENTRies
                             where n.iXactId == iuID && n.SF_ACCOUNT.SF_ACCTTYPE.sAccountType == "Assets"
                             select new { n, ac = n.SF_ACCOUNT.sCode + " " + n.SF_ACCOUNT.sName };
                    foreach (var xp in q2)
                    {
                        dtRow = dtPattern.NewRow();
                        dtRow[(int)Global.enPFPmt.LineNum] = ++iLine;
                        dtRow[(int)Global.enPFPmt.AccountID] = xp.n.iAccountId;
                        dtRow[(int)Global.enPFPmt.AccountName] = xp.ac;
                        dtRow[(int)Global.enPFPmt.PmtMethodID] = xp.n.iPaymentMethod;
                        dtRow[(int)Global.enPFPmt.PmtMethodName] = xp.n.SF_PAYMENTMETHOD.sPaymentMethod;
                        dtRow[(int)Global.enPFPmt.Descr] = xp.n.sDescription;
                        mAmount = xp.n.mAmount;
                        mSum += mAmount;
                        dtRow[(int)Global.enPFPmt.sAmount] = mAmount.ToString("N2", CultureInfo.InvariantCulture);
                        liPayment.Add(dtRow);
                    }
                    foreach (DataRow row in liPayment)
                    {
                        row[(int)Global.enPFExp.LineNum] = iLine--;
                    }

                    // 'Sum' Row
                    dtRow = dtPattern.NewRow();
                    dtRow[(int)Global.enPFPmt.LineNum] = 0;
                    dtRow[(int)Global.enPFPmt.AccountID] = 0;
                    dtRow[(int)Global.enPFPmt.AccountName] = " ";
                    dtRow[(int)Global.enPFPmt.PmtMethodID] = 0;
                    dtRow[(int)Global.enPFPmt.PmtMethodName] = " ";
                    dtRow[(int)Global.enPFPmt.Descr] = "Sum of Credits:";
                    dtRow[(int)Global.enPFPmt.sAmount] = mSum.ToString("N2", CultureInfo.InvariantCulture);
                    liPayment.Add(dtRow);

                    // 'New' Row
                    dtRow = dtPattern.NewRow();
                    dtRow[(int)Global.enPFPmt.LineNum] = -1;
                    sSetting = mCrud.GetSetting("DefaultAssetsAccount");
                    sCode = sSetting.Split(' ')[0];
                    dc = new sf_AccountingDataContext();
                    var qPmt = from p in dc.SF_ACCOUNTs where p.sCode == sCode select p;
                    qr = qPmt.First();
                    dtRow[(int)Global.enPFPmt.AccountID] = qr.ID;
                    dtRow[(int)Global.enPFPmt.AccountName] = qr.sName;
                    sSetting = mCrud.GetSetting("DefaultPaymentMethod");
                    var qMethod = from p in dc.SF_PAYMENTMETHODs where p.sPaymentMethod == sSetting select p;
                    var qp = qMethod.First();
                    dtRow[(int)Global.enPFPmt.PmtMethodID] = qp.ID;
                    dtRow[(int)Global.enPFPmt.PmtMethodName] = qp.sPaymentMethod;
                    dtRow[(int)Global.enPFPmt.Descr] = " ";
                    dtRow[(int)Global.enPFPmt.sAmount] = "";
                    liPayment.Add(dtRow);

                    return liPayment;
                #endregion
                #region Attached Files
                case Global.enLL.AttachedFiles:
                    List<DataRow> liAttachedFile = new List<DataRow>();
                    dtPattern = dtSchema(Global.enLL.AttachedFiles);
                    iLine = 0;
                    int iNumAtt = 0;

                    // 'Data' rows
                    var q4 = from af in dc.SF_XACT_DOCs
                             let d = af.SF_DOC
                             where af.iXactId == iuID
                             select new { af, d };
                    foreach (var a in q4)
                    {
                        dtRow = dtPattern.NewRow();
                        dtRow[(int)Global.enPFAtt.LineNum] = ++iLine;
                        dtRow[(int)Global.enPFAtt.AttachCategID] = a.af.iAttachmentCateg;
                        dtRow[(int)Global.enPFAtt.AttachCategName] = a.af.SF_ATTACHMENTCATEG.sAttachmentCateg;
                        dtRow[(int)Global.enPFAtt.AttachAssocDate] = a.d.DDateOfDoc;
                        dtRow[(int)Global.enPFAtt.AttachedFileID] = a.af.iDocsId;
                        dtRow[(int)Global.enPFAtt.AttachedFileName] = a.d.sPrefix + "_" + a.d.sName;
                        dtRow[(int)Global.enPFAtt.AttachedThumbID] = 0;
                        dtRow[(int)Global.enPFAtt.AttachedThumbName] = " ";
                        liAttachedFile.Add(dtRow);
                    }
                    iNumAtt = iLine;
                    foreach (DataRow row in liAttachedFile)
                    {
                        row[(int)Global.enPFExp.LineNum] = iLine--;
                    }

                    // 'Sum' Row
                    dtRow = dtPattern.NewRow();
                    dtRow[(int)Global.enPFAtt.LineNum] = 0;
                    dtRow[(int)Global.enPFAtt.AttachCategID] = 0;
                    dtRow[(int)Global.enPFAtt.AttachCategName] = "Number of Attached Files:";
                    dtRow[(int)Global.enPFAtt.AttachAssocDate] = DateTimeOffset.MinValue;
                    dtRow[(int)Global.enPFAtt.AttachedFileID] = 0;
                    dtRow[(int)Global.enPFAtt.AttachedFileName] = iNumAtt.ToString();
                    dtRow[(int)Global.enPFAtt.AttachedThumbID] = 0;
                    dtRow[(int)Global.enPFAtt.AttachedThumbName] = " ";
                    liAttachedFile.Add(dtRow);

                    // 'New' Row
                    dtRow = dtPattern.NewRow();
                    dtRow[(int)Global.enPFAtt.LineNum] = -1;
                    sSetting = mCrud.GetSetting("DefaultAttachmentCateg");
                    dc = new sf_AccountingDataContext();
                    var qAT = from p in dc.SF_ATTACHMENTCATEGs where p.sAttachmentCateg == sSetting select p;
                    var qt = qAT.First();
                    dtRow[(int)Global.enPFAtt.AttachCategID] = qt.ID;
                    dtRow[(int)Global.enPFAtt.AttachCategName] = qt.sAttachmentCateg;
                    dtRow[(int)Global.enPFAtt.AttachAssocDate] = DateTimeOffset.Now;
                    dtRow[(int)Global.enPFAtt.AttachedFileID] = 0;
                    dtRow[(int)Global.enPFAtt.AttachedFileName] = " ";
                    dtRow[(int)Global.enPFAtt.AttachedThumbID] = 0;
                    dtRow[(int)Global.enPFAtt.AttachedThumbName] = " ";
                    liAttachedFile.Add(dtRow);

                    return liAttachedFile;
                #endregion

                #region Operational Calendar Times
                case Global.enLL.OpsCalendarTimes:
                    List<DataRow> liOpsCalTimes = new List<DataRow>();
                    dtPattern = dtSchema(Global.enLL.OpsCalendarTimes);
                    TSoar.Equipment.EquipmentDataContext eqdc = new TSoar.Equipment.EquipmentDataContext();
                    var qoc = from o in eqdc.OPSCALTIMEs where o.iOpsCal == iuID select o; // We want all records for just one operational calendar
                    //Existing rows
                    foreach (var o in qoc)
                    {
                        dtRow = dtPattern.NewRow();
                        dtRow[(int)Global.enPFOpsCalTimes.i1Sort] = 0;
                        dtRow[(int)Global.enPFOpsCalTimes.ID] = o.ID;
                        dtRow[(int)Global.enPFOpsCalTimes.iOpsCal] = o.iOpsCal;
                        dtRow[(int)Global.enPFOpsCalTimes.DStart] = o.DStart;
                        dtRow[(int)Global.enPFOpsCalTimes.bOpStatus] = o.bOpStatus;
                        dtRow[(int)Global.enPFOpsCalTimes.sComment] = o.sComment;

                        liOpsCalTimes.Add(dtRow);
                    }
                    //New row
                    dtRow = dtPattern.NewRow();
                    dtRow[(int)Global.enPFOpsCalTimes.i1Sort] = 1;
                    dtRow[(int)Global.enPFOpsCalTimes.ID] = 0;
                    dtRow[(int)Global.enPFOpsCalTimes.iOpsCal] = iuID;
                    dtRow[(int)Global.enPFOpsCalTimes.DStart] = DateTimeOffset.Now;
                    dtRow[(int)Global.enPFOpsCalTimes.bOpStatus] = true;
                    dtRow[(int)Global.enPFOpsCalTimes.sComment] = "";

                    liOpsCalTimes.Add(dtRow);
                    return liOpsCalTimes;
                #endregion

                #region Equipment Operating Data
                case Global.enLL.EquipOpData:
                    List<DataRow> liOpData = new List<DataRow>();
                    dtPattern = dtSchema(Global.enLL.EquipOpData);
                    eqdc = new TSoar.Equipment.EquipmentDataContext();
                    var qh = from q in eqdc.EQUIPOPERDATAs
                             where q.iEquipComponent == iuID
                             orderby q.DFrom
                             select new
                             {
                                 q.ID,
                                 q.iEquipComponent,
                                 q.EQUIPCOMPONENT.sComponent,
                                 q.DFrom,
                                 q.DTo,
                                 q.dHours,
                                 q.iCycles,
                                 q.dDistance,
                                 q.sDistanceUnits,
                                 q.cSource,
                                 q.sComment
                             };
                    //Existing Rows
                    foreach (var q in qh)
                    {
                        dtRow = dtPattern.NewRow();
                        dtRow[(int)Global.enPFEqOpData.i1Sort] = 0;
                        dtRow[(int)Global.enPFEqOpData.ID] = q.ID;
                        dtRow[(int)Global.enPFEqOpData.iEquipComponent] = q.iEquipComponent;
                        dtRow[(int)Global.enPFEqOpData.sComponent] = q.sComponent;
                        dtRow[(int)Global.enPFEqOpData.DFrom] = q.DFrom;
                        dtRow[(int)Global.enPFEqOpData.DTo] = q.DTo;
                        dtRow[(int)Global.enPFEqOpData.dHours] = q.dHours;
                        dtRow[(int)Global.enPFEqOpData.iCycles] = q.iCycles;
                        dtRow[(int)Global.enPFEqOpData.dDistance] = q.dDistance;
                        dtRow[(int)Global.enPFEqOpData.sDistanceUnits] = q.sDistanceUnits;
                        dtRow[(int)Global.enPFEqOpData.cSource] = q.cSource;
                        dtRow[(int)Global.enPFEqOpData.sComment] = q.sComment;

                        liOpData.Add(dtRow);
                    }
                    //New Row
                    dtRow = dtPattern.NewRow();
                    dtRow[(int)Global.enPFEqOpData.i1Sort] = 1;
                    dtRow[(int)Global.enPFEqOpData.ID] = 0;
                    var q5 = (from e in eqdc.EQUIPCOMPONENTs select e).First();
                    dtRow[(int)Global.enPFEqOpData.iEquipComponent] = q5.ID;
                    dtRow[(int)Global.enPFEqOpData.sComponent] = "";

                    string sOffset = mCrud.GetSetting("TimeZoneOffset");
                    int iOffsetHrs = Int32.Parse(sOffset.Substring(0, 3));
                    int iOffsetMin = Int32.Parse(sOffset.Substring(4, 2));
                    int iYear = DateTimeOffset.Now.Year;
                    dtRow[(int)Global.enPFEqOpData.DFrom] = new DateTimeOffset(iYear, 1, 1, 1, 1, 0, new TimeSpan(iOffsetHrs, iOffsetMin, 0));
                    dtRow[(int)Global.enPFEqOpData.DTo] = new DateTimeOffset(iYear, 12, 31, 22, 59, 0, new TimeSpan(iOffsetHrs, iOffsetMin, 0));

                    dtRow[(int)Global.enPFEqOpData.dHours] = 0.01M;
                    dtRow[(int)Global.enPFEqOpData.iCycles] = 0;
                    dtRow[(int)Global.enPFEqOpData.dDistance] = 0.01M;
                    dtRow[(int)Global.enPFEqOpData.sDistanceUnits] = "Miles";
                    dtRow[(int)Global.enPFEqOpData.cSource] = 'M';
                    dtRow[(int)Global.enPFEqOpData.sComment] = "";

                    liOpData.Add(dtRow);

                    return liOpData;
                #endregion

                default:
                    return null;
            }
        }

        static public List<DataRow> Init(Global.enLL euLL, int iuID, bool buWichDefault)
        {
            sf_AccountingDataContext dc = new sf_AccountingDataContext();
            DataTable dtPattern = new DataTable();
            SCUD_Multi mCrud = new SCUD_Multi();
            switch (euLL)
            {

                #region Flight Log Rows
                case Global.enLL.FlightLogRows:
                    List<DataRow> liDFlLogRow = new List<DataRow>();
                    dtPattern = dtSchema(Global.enLL.FlightLogRows);
                    TSoar.Statistician.StatistDailyFlightLogDataContext stdc = new TSoar.Statistician.StatistDailyFlightLogDataContext();
                    var qdfr = from q in stdc.sp_FlightLogRows(iuID)
                               where q.iFliteLog == iuID
                               orderby q.DTakeOff
                               select q;
                    DataRow dtRow;
                    //Existing rows
                    foreach (var q in qdfr)
                    {
                        dtRow = dtPattern.NewRow();
                        dtRow[(int)Global.enPFFlightLogRowsPlus.ID] = q.ID;
                        dtRow[(int)Global.enPFFlightLogRowsPlus.iFliteLog] = q.iFliteLog;
                        dtRow[(int)Global.enPFFlightLogRowsPlus.cStatus] = q.cStatus;
                        dtRow[(int)Global.enPFFlightLogRowsPlus.iTowEquip] = q.iTowEquip;
                        dtRow[(int)Global.enPFFlightLogRowsPlus.sTowEquipName] = q.sTowEquipName;
                        dtRow[(int)Global.enPFFlightLogRowsPlus.iTowOperator] = q.iTowOperator;
                        dtRow[(int)Global.enPFFlightLogRowsPlus.sTowOperName] = q.sTowOperName;
                        dtRow[(int)Global.enPFFlightLogRowsPlus.iGlider] = q.iGlider;
                        dtRow[(int)Global.enPFFlightLogRowsPlus.sGliderName] = q.sGliderName;
                        dtRow[(int)Global.enPFFlightLogRowsPlus.iLaunchMethod] = q.iLaunchMethod;
                        dtRow[(int)Global.enPFFlightLogRowsPlus.sLaunchMethod] = q.sLaunchMethod;
                        dtRow[(int)Global.enPFFlightLogRowsPlus.iPilot1] = q.iPilot1;
                        dtRow[(int)Global.enPFFlightLogRowsPlus.sPilot1] = q.sPilot1;
                        dtRow[(int)Global.enPFFlightLogRowsPlus.iAviatorRole1] = q.iAviatorRole1;
                        dtRow[(int)Global.enPFFlightLogRowsPlus.sAviatorRole1] = q.sAviatorRole1;
                        dtRow[(int)Global.enPFFlightLogRowsPlus.dPctCharge1] = q.dPctCharge1;
                        dtRow[(int)Global.enPFFlightLogRowsPlus.iPilot2] = q.iPilot2;
                        dtRow[(int)Global.enPFFlightLogRowsPlus.sPilot2] = q.sPilot2;
                        dtRow[(int)Global.enPFFlightLogRowsPlus.iAviatorRole2] = q.iAviatorRole2;
                        dtRow[(int)Global.enPFFlightLogRowsPlus.sAviatorRole2] = q.sAviatorRole2;
                        dtRow[(int)Global.enPFFlightLogRowsPlus.dPctCharge2] = q.dPctCharge2;
                        dtRow[(int)Global.enPFFlightLogRowsPlus.dReleaseAltitude] = q.dReleaseAltitude;
                        dtRow[(int)Global.enPFFlightLogRowsPlus.dMaxAltitude] = q.dMaxAltitude;
                        dtRow[(int)Global.enPFFlightLogRowsPlus.iLocTakeOff] = q.iLocTakeOff;
                        dtRow[(int)Global.enPFFlightLogRowsPlus.sLocTakeOff] = q.sLocTakeOff;
                        dtRow[(int)Global.enPFFlightLogRowsPlus.DTakeOff] = q.DTakeOff;
                        dtRow[(int)Global.enPFFlightLogRowsPlus.iLocLanding] = q.iLocLanding;
                        dtRow[(int)Global.enPFFlightLogRowsPlus.sLocLanding] = q.sLocLanding;
                        dtRow[(int)Global.enPFFlightLogRowsPlus.DLanding] = q.DLanding;
                        dtRow[(int)Global.enPFFlightLogRowsPlus.iChargeCode] = q.iChargeCode;
                        dtRow[(int)Global.enPFFlightLogRowsPlus.cChargeCode] = q.cChargeCode;
                        dtRow[(int)Global.enPFFlightLogRowsPlus.sChargeCode] = q.sChargeCode;
                        dtRow[(int)Global.enPFFlightLogRowsPlus.mAmtCollected] = q.mAmtCollected;
                        dtRow[(int)Global.enPFFlightLogRowsPlus.sComments] = q.sComments;

                        liDFlLogRow.Add(dtRow);
                    }

                    //New row
                    dtRow = dtPattern.NewRow();
                    dtRow[(int)Global.enPFFlightLogRowsPlus.ID] = 0;
                    dtRow[(int)Global.enPFFlightLogRowsPlus.iFliteLog] = iuID;
                    dtRow[(int)Global.enPFFlightLogRowsPlus.cStatus] = "N";
                    dtRow[(int)Global.enPFFlightLogRowsPlus.mAmtCollected] = 0.00m;
                    dtRow[(int)Global.enPFFlightLogRowsPlus.sComments] = "";

                    DataTable dtLastUsedIn = new DataTable("LastUsedIn");
                    // If we encounter an error trying to interpret the contents of dtLastUsedIn then the second iteration of the following for loop forces use of the main items in the daily flight log heading
                    for (int iErrCnt = 0; iErrCnt < 2; iErrCnt++) // to make sure we don't execute this more than twice
                    {
                        // buWichDefault: TRUE means take default values from "Main Items in Daily Flight Log Heading"; FALSE means take defaults from "Last Used"
                        if (buWichDefault || AccountProfile.CurrentUser.FlightLogRowsDefaults.Rows.Count < 1 || iErrCnt > 0)
                        { 
                            string[] sa = new string[21];
                            sa[0] = "1";
                            sa[1] = "N";
                            int? iMain = (from d in stdc.DAILYFLIGHTLOGs where d.ID == iuID select d.iMainTowEquip).First();
                            sa[2] = (iMain == null) ? "0" : iMain.ToString();
                            iMain = (from d in stdc.DAILYFLIGHTLOGs where d.ID == iuID select d.iMainTowOp).First();
                            sa[3] = (iMain == null) ? "0" : iMain.ToString();
                            iMain = (from d in stdc.DAILYFLIGHTLOGs where d.ID == iuID select d.iMainGlider).First();
                            sa[4] = (iMain == null) ? "0" : iMain.ToString();
                            iMain = (from d in stdc.DAILYFLIGHTLOGs where d.ID == iuID select d.iMainLaunchMethod).First();
                            sa[5] = (iMain == null) ? "0" : iMain.ToString();
                            sa[6] = (from p in stdc.PEOPLEs where p.sDisplayName == (from j in stdc.SETTINGs where j.sSettingName == "DefaultAviator" select j.sSettingValue).First() select p.ID).First().ToString();
                            sa[7] = (from r in stdc.AVIATORROLEs where r.sAviatorRole == (from j in stdc.SETTINGs where j.sSettingName == "DefaultGliderPilotRole" select j.sSettingValue).First() select r.ID).First().ToString();
                            sa[8] = "100.00";
                            sa[9] = (from p in stdc.PEOPLEs where p.sDisplayName == "[none]" select p.ID).First().ToString(); // Pilot2
                            sa[10] = (from r in stdc.AVIATORROLEs where r.sAviatorRole == "[none]" select r.ID).First().ToString(); ; // Aviatorrole 2
                            sa[11] = "0.00"; // Percent charge 2
                            sa[12] = (from j in stdc.SETTINGs where j.sSettingName == "DefaultReleaseAltitude" select j.sSettingValue).First();
                            sa[13] = "-2000";
                            sa[14] = (from L in stdc.DAILYFLIGHTLOGs where L.ID == iuID && L.LOCATION.ID == L.iMainLocation select L.LOCATION.ID).First().ToString(); // Takeoff Location
                            DateTimeOffset DTO = (from d in stdc.DAILYFLIGHTLOGs where d.ID == iuID select d.DFlightOps).First();
                            string sTZO = mCrud.GetSetting("TimeZoneOffset");
                            TimeSpan TS = new TimeSpan(Int32.Parse(sTZO.Substring(0, 3)), Int32.Parse(sTZO.Substring(4, 2)), 0);
                            DTO = new DateTimeOffset(DTO.Year, DTO.Month, DTO.Day, 12, 0, 0, TS);
                            sa[15] = DTO.ToString();
                            sa[16] = (from L in stdc.DAILYFLIGHTLOGs where L.ID == iuID && L.LOCATION.ID == L.iMainLocation select L.LOCATION.ID).First().ToString(); // Landing Location
                            sa[17] = DTO.AddHours(0.5).ToString();
                            sa[18] = (from j in stdc.SETTINGs where j.sSettingName == "DefaultChargeCode" select j.sSettingValue).First();
                            sa[19] = "0.00";
                            sa[20] = "";
                            LastUsedInputs_FlightLogRows(sa, ref dtLastUsedIn);
                        }
                        else
                        {
                            dtLastUsedIn = AccountProfile.CurrentUser.FlightLogRowsDefaults;
                        }

                        try
                        {
                            dtRow[(int)Global.enPFFlightLogRowsPlus.iTowEquip] = Int32.Parse((string)dtLastUsedIn.Rows[0].ItemArray[(int)Global.enPFFlightLogRowItems.TowEquip]);
                            dtRow[(int)Global.enPFFlightLogRowsPlus.sTowEquipName] = (from j in stdc.EQUIPMENTs where j.ID == (int)dtRow[(int)Global.enPFFlightLogRowsPlus.iTowEquip] select j.sShortEquipName).First();
                            dtRow[(int)Global.enPFFlightLogRowsPlus.iTowOperator] = Int32.Parse((string)dtLastUsedIn.Rows[0].ItemArray[(int)Global.enPFFlightLogRowItems.TowOperator]);
                            dtRow[(int)Global.enPFFlightLogRowsPlus.sTowOperName] = (from p in stdc.PEOPLEs where p.ID == (int)dtRow[(int)Global.enPFFlightLogRowsPlus.iTowOperator] select p.sDisplayName).First();
                            dtRow[(int)Global.enPFFlightLogRowsPlus.iGlider] = Int32.Parse((string)dtLastUsedIn.Rows[0].ItemArray[(int)Global.enPFFlightLogRowItems.Glider]);
                            dtRow[(int)Global.enPFFlightLogRowsPlus.sGliderName] = (from j in stdc.EQUIPMENTs where j.ID == (int)dtRow[(int)Global.enPFFlightLogRowsPlus.iGlider] select j.sShortEquipName).First();
                            dtRow[(int)Global.enPFFlightLogRowsPlus.iLaunchMethod] = Int32.Parse((string)dtLastUsedIn.Rows[0].ItemArray[(int)Global.enPFFlightLogRowItems.LaunchMethod]);
                            dtRow[(int)Global.enPFFlightLogRowsPlus.sLaunchMethod] = (from j in stdc.LAUNCHMETHODs where j.ID == (int)dtRow[(int)Global.enPFFlightLogRowsPlus.iLaunchMethod] select j.sLaunchMethod).First();
                            dtRow[(int)Global.enPFFlightLogRowsPlus.iPilot1] = Int32.Parse((string)dtLastUsedIn.Rows[0].ItemArray[(int)Global.enPFFlightLogRowItems.Pilot1]);
                            dtRow[(int)Global.enPFFlightLogRowsPlus.sPilot1] = (from p in stdc.PEOPLEs where p.ID == (int)dtRow[(int)Global.enPFFlightLogRowsPlus.iPilot1] select p.sDisplayName).First();
                            dtRow[(int)Global.enPFFlightLogRowsPlus.iAviatorRole1] = Int32.Parse((string)dtLastUsedIn.Rows[0].ItemArray[(int)Global.enPFFlightLogRowItems.AviatorRole1]);
                            dtRow[(int)Global.enPFFlightLogRowsPlus.sAviatorRole1] = (from j in stdc.AVIATORROLEs where j.ID == (int)dtRow[(int)Global.enPFFlightLogRowsPlus.iAviatorRole1] select j.sAviatorRole).First();
                            dtRow[(int)Global.enPFFlightLogRowsPlus.dPctCharge1] = 100.00m;

                            // (SCR 113)
                            dtRow[(int)Global.enPFFlightLogRowsPlus.iPilot2] = Int32.Parse((string)dtLastUsedIn.Rows[0].ItemArray[(int)Global.enPFFlightLogRowItems.Pilot2]);
                            dtRow[(int)Global.enPFFlightLogRowsPlus.sPilot2] = (from p in stdc.PEOPLEs where p.ID == (int)dtRow[(int)Global.enPFFlightLogRowsPlus.iPilot2] select p.sDisplayName).First();
                            dtRow[(int)Global.enPFFlightLogRowsPlus.iAviatorRole2] = Int32.Parse((string)dtLastUsedIn.Rows[0].ItemArray[(int)Global.enPFFlightLogRowItems.AviatorRole2]);
                            dtRow[(int)Global.enPFFlightLogRowsPlus.sAviatorRole2] = (from j in stdc.AVIATORROLEs where j.ID == (int)dtRow[(int)Global.enPFFlightLogRowsPlus.iAviatorRole2] select j.sAviatorRole).First();
                            dtRow[(int)Global.enPFFlightLogRowsPlus.dPctCharge2] = 0.00m;
                            dtRow[(int)Global.enPFFlightLogRowsPlus.dReleaseAltitude] = dtLastUsedIn.Rows[0].ItemArray[(int)Global.enPFFlightLogRowItems.ReleaseAltitude];
                            dtRow[(int)Global.enPFFlightLogRowsPlus.dMaxAltitude] = dtLastUsedIn.Rows[0].ItemArray[(int)Global.enPFFlightLogRowItems.MaxAltitude];
                            dtRow[(int)Global.enPFFlightLogRowsPlus.iLocTakeOff] = Int32.Parse((string)dtLastUsedIn.Rows[0].ItemArray[(int)Global.enPFFlightLogRowItems.LocTakeOff]);
                            dtRow[(int)Global.enPFFlightLogRowsPlus.sLocTakeOff] = (from j in stdc.LOCATIONs where j.ID == (int)dtRow[(int)Global.enPFFlightLogRowsPlus.iLocTakeOff] select j.sLocation);
                            dtRow[(int)Global.enPFFlightLogRowsPlus.DTakeOff] = dtLastUsedIn.Rows[0].ItemArray[(int)Global.enPFFlightLogRowItems.DateTakeOff];
                            dtRow[(int)Global.enPFFlightLogRowsPlus.iLocLanding] = Int32.Parse((string)dtLastUsedIn.Rows[0].ItemArray[(int)Global.enPFFlightLogRowItems.LocLanding]);
                            dtRow[(int)Global.enPFFlightLogRowsPlus.sLocLanding] = (from j in stdc.LOCATIONs where j.ID == (int)dtRow[(int)Global.enPFFlightLogRowsPlus.iLocLanding] select j.sLocation);
                            dtRow[(int)Global.enPFFlightLogRowsPlus.DLanding] = dtLastUsedIn.Rows[0].ItemArray[(int)Global.enPFFlightLogRowItems.DateLanding];
                            dtRow[(int)Global.enPFFlightLogRowsPlus.iChargeCode] = (from j in stdc.CHARGECODEs where j.sChargeCode == (string)dtLastUsedIn.Rows[0].ItemArray[(int)Global.enPFFlightLogRowItems.ChargeCode] select j.ID).First();
                            dtRow[(int)Global.enPFFlightLogRowsPlus.sChargeCode] = dtLastUsedIn.Rows[0].ItemArray[(int)Global.enPFFlightLogRowItems.ChargeCode];
                        }
                        catch
                        {
                            continue; // do the second iteration of the for loop. iErrCnt will be incremented
                        }
                        liDFlLogRow.Add(dtRow);

                        return liDFlLogRow; // Note: we jump out of the 'for' loop at this point
                    } //for (int iErrCnt = 0; iErrCnt < 2; iErrCnt++)
                    return null; // this statement should never be reached. It's included to avoid the error "not all code paths return a value"
                #endregion
                default:
                    return null;
            }
        }

        static public List<DataRow> Init(Global.enLL euLL, int iuID, DateTimeOffset DuFrom, DateTimeOffset DuTo)
        {
            #region Flying Charges Editing
            List<DataRow> liFCE = new List<DataRow>();
            DataTable dtPattern = dtSchema(Global.enLL.FlyChrgEdit);
            FinDetails.SalesAR.FlyActInvoiceDataContext fdc = new FinDetails.SalesAR.FlyActInvoiceDataContext();
            var qe = from e in fdc.FLYINGCHARGEs where e.iPerson == iuID && e.DateOfAmount >= DuFrom && e.DateOfAmount <= DuTo select e;
            DataRow dtRow;
            // Existing rows
            foreach (var e in qe)
            {
                dtRow = dtPattern.NewRow();
                dtRow[(int)Global.enPFFlyChrgEdit.i1Sort] = 0; // ensures that existing rows appear at the top of the gridview
                dtRow[(int)Global.enPFFlyChrgEdit.ID] = e.ID;
                dtRow[(int)Global.enPFFlyChrgEdit.mAmount] = e.mAmount;
                dtRow[(int)Global.enPFFlyChrgEdit.DateOfAmount] = e.DateOfAmount;
                dtRow[(int)Global.enPFFlyChrgEdit.cTypeOfAmount] = e.cTypeOfAmount;
                dtRow[(int)Global.enPFFlyChrgEdit.bManuallyModified] = e.bManuallyModified;
                dtRow[(int)Global.enPFFlyChrgEdit.sComments] = e.sComments;
                liFCE.Add(dtRow);
            }
            // New Row
            dtRow = dtPattern.NewRow();
            dtRow[(int)Global.enPFFlyChrgEdit.i1Sort] = 1; // ensures that the new row appears at the bottom of the gridview
            dtRow[(int)Global.enPFFlyChrgEdit.ID] = 0;
            dtRow[(int)Global.enPFFlyChrgEdit.mAmount] = 0.0m;
            dtRow[(int)Global.enPFFlyChrgEdit.DateOfAmount] = DateTimeOffset.Now;
            dtRow[(int)Global.enPFFlyChrgEdit.cTypeOfAmount] = 'A';
            dtRow[(int)Global.enPFFlyChrgEdit.bManuallyModified] = true;
            dtRow[(int)Global.enPFFlyChrgEdit.sComments] = "";
            liFCE.Add(dtRow);

            return liFCE;
            #endregion
        }
        #endregion

        static public DataTable dtSchema(Global.enLL euLL)
        { // Define the columns of various DataTables
            DataColumn[] dcaPF;
            DataTable dt_Pattern = new DataTable();
            switch (euLL)
            {
                #region Expense
                case Global.enLL.Expenditure:
                    dcaPF = new DataColumn[Enum.GetNames(typeof(Global.enPFExp)).Length]; // PF stands for Pattern Field
                    dcaPF[(int)Global.enPFExp.LineNum] = new DataColumn(Global.enPFExp.LineNum.ToString(), Type.GetType("System.Int32"));
                    dcaPF[(int)Global.enPFExp.AccountID] = new DataColumn(Global.enPFExp.AccountID.ToString(), Type.GetType("System.Int32"));
                    dcaPF[(int)Global.enPFExp.AccountName] = new DataColumn(Global.enPFExp.AccountName.ToString(), Type.GetType("System.String"));
                    dcaPF[(int)Global.enPFExp.Descr] = new DataColumn(Global.enPFExp.Descr.ToString(), Type.GetType("System.String"));
                    dcaPF[(int)Global.enPFExp.sAmount] = new DataColumn(Global.enPFExp.sAmount.ToString(), Type.GetType("System.String"));
                    foreach (DataColumn col in dcaPF)
                    {
                        col.AllowDBNull = false;
                        dt_Pattern.Columns.Add(col);
                    }
                    return dt_Pattern;

                case Global.enLL.Payment:
                    dcaPF = new DataColumn[Enum.GetNames(typeof(Global.enPFPmt)).Length]; // PF stands for Pattern Field
                    dcaPF[(int)Global.enPFPmt.LineNum] = new DataColumn(Global.enPFPmt.LineNum.ToString(), Type.GetType("System.Int32"));
                    dcaPF[(int)Global.enPFPmt.AccountID] = new DataColumn(Global.enPFPmt.AccountID.ToString(), Type.GetType("System.Int32"));
                    dcaPF[(int)Global.enPFPmt.AccountName] = new DataColumn(Global.enPFPmt.AccountName.ToString(), Type.GetType("System.String"));
                    dcaPF[(int)Global.enPFPmt.PmtMethodID] = new DataColumn(Global.enPFPmt.PmtMethodID.ToString(), Type.GetType("System.Int32"));
                    dcaPF[(int)Global.enPFPmt.PmtMethodName] = new DataColumn(Global.enPFPmt.PmtMethodName.ToString(), Type.GetType("System.String"));
                    dcaPF[(int)Global.enPFPmt.Descr] = new DataColumn(Global.enPFPmt.Descr.ToString(), Type.GetType("System.String")); // Reference info like check number
                    dcaPF[(int)Global.enPFPmt.sAmount] = new DataColumn(Global.enPFPmt.sAmount.ToString(), Type.GetType("System.String"));
                    foreach (DataColumn col in dcaPF)
                    {
                        col.AllowDBNull = false;
                        dt_Pattern.Columns.Add(col);
                    }
                    return dt_Pattern;

                case Global.enLL.AttachedFiles:
                    dcaPF = new DataColumn[Enum.GetNames(typeof(Global.enPFAtt)).Length]; // PF stands for Pattern Field
                    dcaPF[(int)Global.enPFAtt.LineNum] = new DataColumn(Global.enPFAtt.LineNum.ToString(), Type.GetType("System.Int32"));
                    dcaPF[(int)Global.enPFAtt.AttachCategID] = new DataColumn(Global.enPFAtt.AttachCategID.ToString(), Type.GetType("System.Int32"));
                    dcaPF[(int)Global.enPFAtt.AttachCategName] = new DataColumn(Global.enPFAtt.AttachCategName.ToString(), Type.GetType("System.String"));
                    dcaPF[(int)Global.enPFAtt.AttachAssocDate] = new DataColumn(Global.enPFAtt.AttachAssocDate.ToString(), Type.GetType("System.DateTimeOffset"));
                    dcaPF[(int)Global.enPFAtt.AttachedFileID] = new DataColumn(Global.enPFAtt.AttachedFileID.ToString(), Type.GetType("System.Int32"));
                    dcaPF[(int)Global.enPFAtt.AttachedFileName] = new DataColumn(Global.enPFAtt.AttachedFileName.ToString(), Type.GetType("System.String"));
                    dcaPF[(int)Global.enPFAtt.AttachedThumbID] = new DataColumn(Global.enPFAtt.AttachedThumbID.ToString(), Type.GetType("System.Int32"));
                    dcaPF[(int)Global.enPFAtt.AttachedThumbName] = new DataColumn(Global.enPFAtt.AttachedThumbName.ToString(), Type.GetType("System.String"));
                    foreach (DataColumn col in dcaPF)
                    {
                        col.AllowDBNull = false;
                        dt_Pattern.Columns.Add(col);
                    }
                    return dt_Pattern;
                #endregion

                #region Vendors
                case Global.enLL.Vendors:
                    dcaPF = new DataColumn[Enum.GetNames(typeof(Global.enPFVend)).Length];
                    dcaPF[(int)Global.enPFVend.VendorName] = new DataColumn(Global.enPFVend.VendorName.ToString(), Type.GetType("System.String"))
                    {
                        AllowDBNull = false
                    };
                    dcaPF[(int)Global.enPFVend.Notes] = new DataColumn(Global.enPFVend.Notes.ToString(), Type.GetType("System.String"))
                    {
                        AllowDBNull = true
                    };
                    foreach (DataColumn col in dcaPF)
                    {
                        dt_Pattern.Columns.Add(col);
                    }
                    return dt_Pattern;
                #endregion

                #region Rewards
                case Global.enLL.Rewards:
                    dcaPF = new DataColumn[Enum.GetNames(typeof(Global.enPFReward)).Length];
                    dcaPF[(int)Global.enPFReward.ID] = new DataColumn(Global.enPFReward.ID.ToString(), Type.GetType("System.Int32"));
                    dcaPF[(int)Global.enPFReward.bEC] = new DataColumn(Global.enPFReward.bEC.ToString(), Type.GetType("System.Boolean"));
                    dcaPF[(int)Global.enPFReward.DEarn] = new DataColumn(Global.enPFReward.DEarn.ToString(), Type.GetType("System.DateTimeOffset"));
                    dcaPF[(int)Global.enPFReward.DExpiry] = new DataColumn(Global.enPFReward.DExpiry.ToString(), Type.GetType("System.DateTimeOffset"));
                    dcaPF[(int)Global.enPFReward.DClaim] = new DataColumn(Global.enPFReward.DClaim.ToString(), Type.GetType("System.DateTimeOffset"));
                    dcaPF[(int)Global.enPFReward.iPerson] = new DataColumn(Global.enPFReward.iPerson.ToString(), Type.GetType("System.Int32"));
                    dcaPF[(int)Global.enPFReward.sDisplayName] = new DataColumn(Global.enPFReward.sDisplayName.ToString(), Type.GetType("System.String"));
                    dcaPF[(int)Global.enPFReward.iServicePts] = new DataColumn(Global.enPFReward.iServicePts.ToString(), Type.GetType("System.Int32"));
                    dcaPF[(int)Global.enPFReward.cECCode] = new DataColumn(Global.enPFReward.cECCode.ToString(), Type.GetType("System.Char"));
                    dcaPF[(int)Global.enPFReward.bForward] = new DataColumn(Global.enPFReward.bForward.ToString(), Type.GetType("System.Boolean"));
                    dcaPF[(int)Global.enPFReward.sComments] = new DataColumn(Global.enPFReward.sComments.ToString(), Type.GetType("System.String"));
                    foreach (DataColumn col in dcaPF)
                    {
                        col.AllowDBNull = false;
                        if (col.ColumnName == "sComments") col.AllowDBNull = true;
                        dt_Pattern.Columns.Add(col);
                    }
                    return dt_Pattern;
                #endregion

                #region Rates
                case Global.enLL.Rates:
                    dcaPF = new DataColumn[Enum.GetNames(typeof(Global.enPFRate)).Length];

                    dcaPF[(int)Global.enPFRate.i1Sort] = new DataColumn(Global.enPFRate.i1Sort.ToString(), Type.GetType("System.Int32"));
                    dcaPF[(int)Global.enPFRate.ID] = new DataColumn(Global.enPFRate.ID.ToString(), Type.GetType("System.Int32"));
                    dcaPF[(int)Global.enPFRate.sShortName] = new DataColumn(Global.enPFRate.sShortName.ToString(), Type.GetType("System.String"));
                    dcaPF[(int)Global.enPFRate.DFrom] = new DataColumn(Global.enPFRate.DFrom.ToString(), Type.GetType("System.DateTimeOffset"));
                    dcaPF[(int)Global.enPFRate.DTo] = new DataColumn(Global.enPFRate.DTo.ToString(), Type.GetType("System.DateTimeOffset"));
                    dcaPF[(int)Global.enPFRate.iEquipType] = new DataColumn(Global.enPFRate.iEquipType.ToString(), Type.GetType("System.Int32"));
                    dcaPF[(int)Global.enPFRate.iLaunchMethod] = new DataColumn(Global.enPFRate.iLaunchMethod.ToString(), Type.GetType("System.Int32"));
                    dcaPF[(int)Global.enPFRate.sChargeCodes] = new DataColumn(Global.enPFRate.sChargeCodes.ToString(), Type.GetType("System.String"));
                    dcaPF[(int)Global.enPFRate.mSingleDpUse] = new DataColumn(Global.enPFRate.mSingleDpUse.ToString(), Type.GetType("System.Decimal"));
                    dcaPF[(int)Global.enPFRate.iNoChrg1stFt] = new DataColumn(Global.enPFRate.iNoChrg1stFt.ToString(), Type.GetType("System.Int32"));
                    dcaPF[(int)Global.enPFRate.mAltDiffDpFt] = new DataColumn(Global.enPFRate.mAltDiffDpFt.ToString(), Type.GetType("System.Decimal"));
                    dcaPF[(int)Global.enPFRate.iNoChrg1stMin] = new DataColumn(Global.enPFRate.iNoChrg1stMin.ToString(), Type.GetType("System.Int32"));
                    dcaPF[(int)Global.enPFRate.mDurationDpMin] = new DataColumn(Global.enPFRate.mDurationDpMin.ToString(), Type.GetType("System.Decimal"));
                    dcaPF[(int)Global.enPFRate.iDurCapMin] = new DataColumn(Global.enPFRate.iDurCapMin.ToString(), Type.GetType("System.Int32"));
                    dcaPF[(int)Global.enPFRate.sComment] = new DataColumn(Global.enPFRate.sComment.ToString(), Type.GetType("System.String"));
                    dcaPF[(int)Global.enPFRate.iFA_Item] = new DataColumn(Global.enPFRate.iFA_Item.ToString(), Type.GetType("System.Int32"));
                    dcaPF[(int)Global.enPFRate.iFA_PmtTerm] = new DataColumn(Global.enPFRate.iFA_PmtTerm.ToString(), Type.GetType("System.Int32"));
                    dcaPF[(int)Global.enPFRate.iQBO_ItemName] = new DataColumn(Global.enPFRate.iQBO_ItemName.ToString(), Type.GetType("System.Int32"));
                    foreach (DataColumn col in dcaPF)
                    {
                        col.AllowDBNull = false;
                        if (col.ColumnName == "sComment") col.AllowDBNull = true;
                        dt_Pattern.Columns.Add(col);
                    }
                    return dt_Pattern;
                #endregion

                #region Minimum Flying Charges Parameters
                case Global.enLL.MinFlyChrgs:
                    dcaPF = new DataColumn[Enum.GetNames(typeof(Global.enPFMFC)).Length];

                    dcaPF[(int)Global.enPFMFC.i1Sort] = new DataColumn(Global.enPFMFC.i1Sort.ToString(), Type.GetType("System.Int32"));
                    dcaPF[(int)Global.enPFMFC.ID] = new DataColumn(Global.enPFMFC.ID.ToString(), Type.GetType("System.Int32"));
                    dcaPF[(int)Global.enPFMFC.DFrom] = new DataColumn(Global.enPFMFC.DFrom.ToString(), Type.GetType("System.DateTimeOffset"));
                    dcaPF[(int)Global.enPFMFC.DTo] = new DataColumn(Global.enPFMFC.DTo.ToString(), Type.GetType("System.DateTimeOffset"));
                    dcaPF[(int)Global.enPFMFC.iMembCateg] = new DataColumn(Global.enPFMFC.iMembCateg.ToString(), Type.GetType("System.Int32"));
                    dcaPF[(int)Global.enPFMFC.sMinFlyChrg] = new DataColumn(Global.enPFMFC.sMinFlyChrg.ToString(), Type.GetType("System.Decimal"));
                    dcaPF[(int)Global.enPFMFC.sComment] = new DataColumn(Global.enPFMFC.sComment.ToString(), Type.GetType("System.String"));
                    foreach (DataColumn col in dcaPF)
                    {
                        col.AllowDBNull = false;
                        if (col.ColumnName == "sComment") col.AllowDBNull = true;
                        dt_Pattern.Columns.Add(col);
                    }
                    return dt_Pattern;
                #endregion

                #region Flying Charges Edit
                case Global.enLL.FlyChrgEdit:
                    dcaPF = new DataColumn[Enum.GetNames(typeof(Global.enPFFlyChrgEdit)).Length];

                    dcaPF[(int)Global.enPFFlyChrgEdit.i1Sort] = new DataColumn(Global.enPFFlyChrgEdit.i1Sort.ToString(), Type.GetType("System.Int32"));
                    dcaPF[(int)Global.enPFFlyChrgEdit.ID] = new DataColumn(Global.enPFFlyChrgEdit.ID.ToString(), Type.GetType("System.Int32"));
                    dcaPF[(int)Global.enPFFlyChrgEdit.mAmount] = new DataColumn(Global.enPFFlyChrgEdit.mAmount.ToString(), Type.GetType("System.Decimal"));
                    dcaPF[(int)Global.enPFFlyChrgEdit.DateOfAmount] = new DataColumn(Global.enPFFlyChrgEdit.DateOfAmount.ToString(), Type.GetType("System.DateTimeOffset"));
                    dcaPF[(int)Global.enPFFlyChrgEdit.cTypeOfAmount] = new DataColumn(Global.enPFFlyChrgEdit.cTypeOfAmount.ToString(), Type.GetType("System.Char"));
                    dcaPF[(int)Global.enPFFlyChrgEdit.bManuallyModified] = new DataColumn(Global.enPFFlyChrgEdit.bManuallyModified.ToString(), Type.GetType("System.Boolean"));
                    dcaPF[(int)Global.enPFFlyChrgEdit.sComments] = new DataColumn(Global.enPFFlyChrgEdit.sComments.ToString(), Type.GetType("System.String"));
                    foreach (DataColumn col in dcaPF)
                    {
                        col.AllowDBNull = false;
                        if (col.ColumnName == "sComments") col.AllowDBNull = true;
                        dt_Pattern.Columns.Add(col);
                    }
                    return dt_Pattern;
                #endregion

                #region People Offices
                case Global.enLL.PeopleOffices:
                    dcaPF = new DataColumn[Enum.GetNames(typeof(Global.enPFPeopleOffices)).Length];

                    dcaPF[(int)Global.enPFPeopleOffices.i1Sort] = new DataColumn(Global.enPFPeopleOffices.i1Sort.ToString(), Type.GetType("System.Int32"));
                    dcaPF[(int)Global.enPFPeopleOffices.ID] = new DataColumn(Global.enPFPeopleOffices.ID.ToString(), Type.GetType("System.Int32"));
                    dcaPF[(int)Global.enPFPeopleOffices.sDisplayName] = new DataColumn(Global.enPFPeopleOffices.sDisplayName.ToString(), Type.GetType("System.String"));
                    dcaPF[(int)Global.enPFPeopleOffices.sBoardOffice] = new DataColumn(Global.enPFPeopleOffices.sBoardOffice.ToString(), Type.GetType("System.String"));
                    dcaPF[(int)Global.enPFPeopleOffices.DOfficeBegin] = new DataColumn(Global.enPFPeopleOffices.DOfficeBegin.ToString(), Type.GetType("System.DateTimeOffset"));
                    dcaPF[(int)Global.enPFPeopleOffices.DOfficeEnd] = new DataColumn(Global.enPFPeopleOffices.DOfficeEnd.ToString(), Type.GetType("System.DateTimeOffset"));
                    dcaPF[(int)Global.enPFPeopleOffices.sAdditionalInfo] = new DataColumn(Global.enPFPeopleOffices.sAdditionalInfo.ToString(), Type.GetType("System.String"));
                    foreach (DataColumn col in dcaPF)
                    {
                        col.AllowDBNull = false;
                        if (col.ColumnName == "sAdditionalInfo") col.AllowDBNull = true;
                        dt_Pattern.Columns.Add(col);
                    }
                    return dt_Pattern;
                #endregion

                #region People Bridges to Equipment Roles and Types
                case Global.enLL.PeopleEquipRolesTypes:
                    dcaPF = new DataColumn[Enum.GetNames(typeof(Global.enPFPeopleEquipRolesTypes)).Length];

                    dcaPF[(int)Global.enPFPeopleEquipRolesTypes.ID] = new DataColumn(Global.enPFPeopleEquipRolesTypes.ID.ToString(), Type.GetType("System.Int32"));
                    dcaPF[(int)Global.enPFPeopleEquipRolesTypes.iAviatorRole] = new DataColumn(Global.enPFPeopleEquipRolesTypes.iAviatorRole.ToString(), Type.GetType("System.Int32"));
                    dcaPF[(int)Global.enPFPeopleEquipRolesTypes.sAviatorRole] = new DataColumn(Global.enPFPeopleEquipRolesTypes.sAviatorRole.ToString(), Type.GetType("System.String"));
                    dcaPF[(int)Global.enPFPeopleEquipRolesTypes.iPerson] = new DataColumn(Global.enPFPeopleEquipRolesTypes.iPerson.ToString(), Type.GetType("System.Int32"));
                    dcaPF[(int)Global.enPFPeopleEquipRolesTypes.sDisplayName] = new DataColumn(Global.enPFPeopleEquipRolesTypes.sDisplayName.ToString(), Type.GetType("System.String"));
                    dcaPF[(int)Global.enPFPeopleEquipRolesTypes.iEqRoleType] = new DataColumn(Global.enPFPeopleEquipRolesTypes.iEqRoleType.ToString(), Type.GetType("System.Int32"));
                    dcaPF[(int)Global.enPFPeopleEquipRolesTypes.sEqRoleType] = new DataColumn(Global.enPFPeopleEquipRolesTypes.sEqRoleType.ToString(), Type.GetType("System.String"));
                    dcaPF[(int)Global.enPFPeopleEquipRolesTypes.sComments] = new DataColumn(Global.enPFPeopleEquipRolesTypes.sComments.ToString(), Type.GetType("System.String"));
                    foreach (DataColumn col in dcaPF)
                    {
                        col.AllowDBNull = false;
                        if (col.ColumnName == "sComments") col.AllowDBNull = true;
                        dt_Pattern.Columns.Add(col);
                    }
                    return dt_Pattern;
                #endregion

                #region Equipment Bridges to Roles and Types
                case Global.enLL.EquipRolesTypes:
                    dcaPF = new DataColumn[Enum.GetNames(typeof(Global.enPFEqRolesTypes)).Length];

                    dcaPF[(int)Global.enPFEqRolesTypes.ID] = new DataColumn(Global.enPFEqRolesTypes.ID.ToString(), Type.GetType("System.Int32"));
                    dcaPF[(int)Global.enPFEqRolesTypes.iRole] = new DataColumn(Global.enPFEqRolesTypes.iRole.ToString(), Type.GetType("System.Int32"));
                    dcaPF[(int)Global.enPFEqRolesTypes.sRole] = new DataColumn(Global.enPFEqRolesTypes.sRole.ToString(), Type.GetType("System.String"));
                    dcaPF[(int)Global.enPFEqRolesTypes.iType] = new DataColumn(Global.enPFEqRolesTypes.iType.ToString(), Type.GetType("System.Int32"));
                    dcaPF[(int)Global.enPFEqRolesTypes.sType] = new DataColumn(Global.enPFEqRolesTypes.sType.ToString(), Type.GetType("System.String"));
                    dcaPF[(int)Global.enPFEqRolesTypes.sComments] = new DataColumn(Global.enPFEqRolesTypes.sComments.ToString(), Type.GetType("System.String"));
                    foreach (DataColumn col in dcaPF)
                    {
                        col.AllowDBNull = false;
                        if (col.ColumnName == "sComments") col.AllowDBNull = true;
                        dt_Pattern.Columns.Add(col);
                    }
                    return dt_Pattern;
                #endregion

                #region Bridges between LaunchMethods and Equipment Roles
                case Global.enLL.LaunchMethodsEqRoles:
                    dcaPF = new DataColumn[Enum.GetNames(typeof(Global.enPFLaunchMEqRoles)).Length];

                    dcaPF[(int)Global.enPFLaunchMEqRoles.ID] = new DataColumn(Global.enPFLaunchMEqRoles.ID.ToString(), Type.GetType("System.Int32"));
                    dcaPF[(int)Global.enPFLaunchMEqRoles.iLaunchMethod] = new DataColumn(Global.enPFLaunchMEqRoles.iLaunchMethod.ToString(), Type.GetType("System.Int32"));
                    dcaPF[(int)Global.enPFLaunchMEqRoles.sLaunchMethod] = new DataColumn(Global.enPFLaunchMEqRoles.sLaunchMethod.ToString(), Type.GetType("System.String"));
                    dcaPF[(int)Global.enPFLaunchMEqRoles.iEqRole] = new DataColumn(Global.enPFLaunchMEqRoles.iEqRole.ToString(), Type.GetType("System.Int32"));
                    dcaPF[(int)Global.enPFLaunchMEqRoles.sEqRole] = new DataColumn(Global.enPFLaunchMEqRoles.sEqRole.ToString(), Type.GetType("System.String"));
                    dcaPF[(int)Global.enPFLaunchMEqRoles.sComments] = new DataColumn(Global.enPFLaunchMEqRoles.sComments.ToString(), Type.GetType("System.String"));
                    foreach (DataColumn col in dcaPF)
                    {
                        col.AllowDBNull = false;
                        if (col.ColumnName == "sComments") col.AllowDBNull = true;
                        dt_Pattern.Columns.Add(col);
                    }
                    return dt_Pattern;
                #endregion

                #region Bridges between Website User Roles and User-Selectable Settings
                case Global.enLL.UserRolesSettings:
                    dcaPF = new DataColumn[Enum.GetNames(typeof(Global.enPFUserRolesSettings)).Length];

                    dcaPF[(int)Global.enPFUserRolesSettings.ID] = new DataColumn(Global.enPFUserRolesSettings.ID.ToString(), Type.GetType("System.Int32"));
                    dcaPF[(int)Global.enPFUserRolesSettings.uiRole] = new DataColumn(Global.enPFUserRolesSettings.uiRole.ToString(), Type.GetType("System.Guid"));
                    dcaPF[(int)Global.enPFUserRolesSettings.RoleName] = new DataColumn(Global.enPFUserRolesSettings.RoleName.ToString(), Type.GetType("System.String"));
                    dcaPF[(int)Global.enPFUserRolesSettings.iSetting] = new DataColumn(Global.enPFUserRolesSettings.iSetting.ToString(), Type.GetType("System.Int32"));
                    dcaPF[(int)Global.enPFUserRolesSettings.sSettingName] = new DataColumn(Global.enPFUserRolesSettings.sSettingName.ToString(), Type.GetType("System.String"));
                    dcaPF[(int)Global.enPFUserRolesSettings.sComments] = new DataColumn(Global.enPFUserRolesSettings.sComments.ToString(), Type.GetType("System.String"));
                    foreach (DataColumn col in dcaPF)
                    {
                        col.AllowDBNull = false;
                        if (col.ColumnName == "sComments") col.AllowDBNull = true;
                        dt_Pattern.Columns.Add(col);
                    }
                    return dt_Pattern;
                #endregion

                #region Operational Calendar Names
                case Global.enLL.OpsCalendarNames:
                    dcaPF = new DataColumn[Enum.GetNames(typeof(Global.enPFOpsCalNames)).Length];
                    dcaPF[(int)Global.enPFOpsCalNames.i1Sort] = new DataColumn(Global.enPFOpsCalNames.i1Sort.ToString(), Type.GetType("System.Int32"));
                    dcaPF[(int)Global.enPFOpsCalNames.ID] = new DataColumn(Global.enPFOpsCalNames.ID.ToString(), Type.GetType("System.Int32"));
                    dcaPF[(int)Global.enPFOpsCalNames.sOpsCalName] = new DataColumn(Global.enPFOpsCalNames.sOpsCalName.ToString(), Type.GetType("System.String"));
                    dcaPF[(int)Global.enPFOpsCalNames.bStandard] = new DataColumn(Global.enPFOpsCalNames.bStandard.ToString(), Type.GetType("System.Boolean"));
                    foreach (DataColumn col in dcaPF)
                    {
                        col.AllowDBNull = false;
                        dt_Pattern.Columns.Add(col);
                    }
                    return dt_Pattern;
                #endregion

                #region Operational Calendar Times
                case Global.enLL.OpsCalendarTimes:
                    dcaPF = new DataColumn[Enum.GetNames(typeof(Global.enPFOpsCalTimes)).Length];
                    dcaPF[(int)Global.enPFOpsCalTimes.i1Sort] = new DataColumn(Global.enPFOpsCalTimes.i1Sort.ToString(), Type.GetType("System.Int32"));
                    dcaPF[(int)Global.enPFOpsCalTimes.ID] = new DataColumn(Global.enPFOpsCalTimes.ID.ToString(), Type.GetType("System.Int32"));
                    dcaPF[(int)Global.enPFOpsCalTimes.iOpsCal] = new DataColumn(Global.enPFOpsCalTimes.iOpsCal.ToString(), Type.GetType("System.Int32"));
                    dcaPF[(int)Global.enPFOpsCalTimes.DStart] = new DataColumn(Global.enPFOpsCalTimes.DStart.ToString(), Type.GetType("System.DateTimeOffset"));
                    dcaPF[(int)Global.enPFOpsCalTimes.bOpStatus] = new DataColumn(Global.enPFOpsCalTimes.bOpStatus.ToString(), Type.GetType("System.Boolean"));
                    dcaPF[(int)Global.enPFOpsCalTimes.sComment] = new DataColumn(Global.enPFOpsCalTimes.sComment.ToString(), Type.GetType("System.String"));
                    foreach (DataColumn col in dcaPF)
                    {
                        col.AllowDBNull = false;
                        if (col.ColumnName == "sComment") col.AllowDBNull = true;
                        dt_Pattern.Columns.Add(col);
                    }
                    return dt_Pattern;
                #endregion

                #region Equipment Components
                case Global.enLL.EquipComponents:
                    dcaPF = new DataColumn[Enum.GetNames(typeof(Global.enPFEqComponents)).Length];

                    dcaPF[(int)Global.enPFEqComponents.i1Sort] = new DataColumn(Global.enPFEqComponents.i1Sort.ToString(), Type.GetType("System.Int32"));
                    dcaPF[(int)Global.enPFEqComponents.ID] = new DataColumn(Global.enPFEqComponents.ID.ToString(), Type.GetType("System.Int32"));
                    dcaPF[(int)Global.enPFEqComponents.iEquipment] = new DataColumn(Global.enPFEqComponents.iEquipment.ToString(), Type.GetType("System.Int32"));
                    dcaPF[(int)Global.enPFEqComponents.sShortEquipName] = new DataColumn(Global.enPFEqComponents.sShortEquipName.ToString(), Type.GetType("System.String"));
                    dcaPF[(int)Global.enPFEqComponents.sComponent] = new DataColumn(Global.enPFEqComponents.sComponent.ToString(), Type.GetType("System.String"));
                    dcaPF[(int)Global.enPFEqComponents.sCName] = new DataColumn(Global.enPFEqComponents.sCName.ToString(), Type.GetType("System.String"));
                    dcaPF[(int)Global.enPFEqComponents.bEntire] = new DataColumn(Global.enPFEqComponents.bEntire.ToString(), Type.GetType("System.Boolean"));
                    dcaPF[(int)Global.enPFEqComponents.DLinkBegin] = new DataColumn(Global.enPFEqComponents.DLinkBegin.ToString(), Type.GetType("System.DateTimeOffset"));
                    dcaPF[(int)Global.enPFEqComponents.DLinkEnd] = new DataColumn(Global.enPFEqComponents.DLinkEnd.ToString(), Type.GetType("System.DateTimeOffset"));
                    dcaPF[(int)Global.enPFEqComponents.bReportOperStatus] = new DataColumn(Global.enPFEqComponents.bReportOperStatus.ToString(), Type.GetType("System.Boolean")); // SCR 231
                    dcaPF[(int)Global.enPFEqComponents.sOperStatus] = new DataColumn(Global.enPFEqComponents.sOperStatus.ToString(), Type.GetType("System.String")); // SCR 231
                    dcaPF[(int)Global.enPFEqComponents.sComment] = new DataColumn(Global.enPFEqComponents.sComment.ToString(), Type.GetType("System.String"));
                    dcaPF[(int)Global.enPFEqComponents.iParentComponent] = new DataColumn(Global.enPFEqComponents.iParentComponent.ToString(), Type.GetType("System.Int32"));
                    foreach (DataColumn col in dcaPF)
                    {
                        col.AllowDBNull = false;
                        if (col.ColumnName == "sComment") col.AllowDBNull = true;
                        dt_Pattern.Columns.Add(col);
                    }
                    return dt_Pattern;
                #endregion

                #region Equipment Status // SCR 218 start
                case Global.enLL.EquipStatus:
                    dcaPF = new DataColumn[Enum.GetNames(typeof(Global.enPFEqStatus)).Length];

                    dcaPF[(int)Global.enPFEqStatus.iLine] = new DataColumn(Global.enPFEqStatus.iLine.ToString(), Type.GetType("System.Int32"));
                    dcaPF[(int)Global.enPFEqStatus.ID] = new DataColumn(Global.enPFEqStatus.ID.ToString(), Type.GetType("System.Int32"));
                    dcaPF[(int)Global.enPFEqStatus.sShortEquipName] = new DataColumn(Global.enPFEqStatus.sShortEquipName.ToString(), Type.GetType("System.String"));
                    dcaPF[(int)Global.enPFEqStatus.sRegistrId] = new DataColumn(Global.enPFEqStatus.sRegistrId.ToString(), Type.GetType("System.String"));
                    dcaPF[(int)Global.enPFEqStatus.sCName] = new DataColumn(Global.enPFEqStatus.sCName.ToString(), Type.GetType("System.String"));
                    dcaPF[(int)Global.enPFEqStatus.sOperStat] = new DataColumn(Global.enPFEqStatus.sOperStat.ToString(), Type.GetType("System.String"));
                    dcaPF[(int)Global.enPFEqStatus.sComment] = new DataColumn(Global.enPFEqStatus.sComment.ToString(), Type.GetType("System.String"));
                    dcaPF[(int)Global.enPFEqStatus.sSorter] = new DataColumn(Global.enPFEqStatus.sSorter.ToString(), Type.GetType("System.String"));
                    dcaPF[(int)Global.enPFEqStatus.sError] = new DataColumn(Global.enPFEqStatus.sError.ToString(), Type.GetType("System.String"));
                    dcaPF[(int)Global.enPFEqStatus.DLastUpdated] = new DataColumn(Global.enPFEqStatus.DLastUpdated.ToString(), Type.GetType("System.DateTime"));
                    dcaPF[(int)Global.enPFEqStatus.sDebug] = new DataColumn(Global.enPFEqStatus.sDebug.ToString(), Type.GetType("System.String"));
                    dcaPF[(int)Global.enPFEqStatus.dAccumHours] = new DataColumn(Global.enPFEqStatus.dAccumHours.ToString(), Type.GetType("System.Decimal"));
                    dcaPF[(int)Global.enPFEqStatus.iAccumCycles] = new DataColumn(Global.enPFEqStatus.iAccumCycles.ToString(), Type.GetType("System.Int32"));
                    dcaPF[(int)Global.enPFEqStatus.dAccumDist] = new DataColumn(Global.enPFEqStatus.dAccumDist.ToString(), Type.GetType("System.Decimal"));
                    dcaPF[(int)Global.enPFEqStatus.sActionItem] = new DataColumn(Global.enPFEqStatus.sActionItem.ToString(), Type.GetType("System.String"));
                    dcaPF[(int)Global.enPFEqStatus.sDeadline] = new DataColumn(Global.enPFEqStatus.sDeadline.ToString(), Type.GetType("System.String"));
                    dcaPF[(int)Global.enPFEqStatus.cPrevSchedMeth] = new DataColumn(Global.enPFEqStatus.cPrevSchedMeth.ToString(), Type.GetType("System.Char"));
                    dcaPF[(int)Global.enPFEqStatus.iPercentComplete] = new DataColumn(Global.enPFEqStatus.iPercentComplete.ToString(), Type.GetType("System.Int32"));
                    
                    foreach (DataColumn col in dcaPF)
                    {
                        col.AllowDBNull = false;
                        if (col.ColumnName == "sOperStat" || col.ColumnName == "sComment") col.AllowDBNull = true;
                        dt_Pattern.Columns.Add(col);
                    }
                    return dt_Pattern;
                #endregion // SCR 218 end

                #region Equipment Parameters
                case Global.enLL.EquipParameters:
                    dcaPF = new DataColumn[Enum.GetNames(typeof(Global.enPFEqParameters)).Length];

                    dcaPF[(int)Global.enPFEqParameters.i1Sort] = new DataColumn(Global.enPFEqParameters.i1Sort.ToString(), Type.GetType("System.Int32"));
                    dcaPF[(int)Global.enPFEqParameters.ID] = new DataColumn(Global.enPFEqParameters.ID.ToString(), Type.GetType("System.Int32"));
                    dcaPF[(int)Global.enPFEqParameters.sShortDescript] = new DataColumn(Global.enPFEqParameters.sShortDescript.ToString(), Type.GetType("System.String"));
                    dcaPF[(int)Global.enPFEqParameters.iEquipActionType] = new DataColumn(Global.enPFEqParameters.iEquipActionType.ToString(), Type.GetType("System.Int32"));
                    dcaPF[(int)Global.enPFEqParameters.sEquipActionType] = new DataColumn(Global.enPFEqParameters.sEquipActionType.ToString(), Type.GetType("System.String"));
                    dcaPF[(int)Global.enPFEqParameters.iIntervalElapsed] = new DataColumn(Global.enPFEqParameters.iIntervalElapsed.ToString(), Type.GetType("System.Int32"));
                    dcaPF[(int)Global.enPFEqParameters.sTimeUnitsElapsed] = new DataColumn(Global.enPFEqParameters.sTimeUnitsElapsed.ToString(), Type.GetType("System.String"));
                    dcaPF[(int)Global.enPFEqParameters.cDeadLineMode] = new DataColumn(Global.enPFEqParameters.cDeadLineMode.ToString(), Type.GetType("System.Char"));
                    dcaPF[(int)Global.enPFEqParameters.iDeadLineSpt1] = new DataColumn(Global.enPFEqParameters.iDeadLineSpt1.ToString(), Type.GetType("System.Int32"));
                    dcaPF[(int)Global.enPFEqParameters.iDeadLineSpt2] = new DataColumn(Global.enPFEqParameters.iDeadLineSpt2.ToString(), Type.GetType("System.Int32"));
                    dcaPF[(int)Global.enPFEqParameters.iIntervalOperating] = new DataColumn(Global.enPFEqParameters.iIntervalOperating.ToString(), Type.GetType("System.Int32"));
                    dcaPF[(int)Global.enPFEqParameters.sTimeUnitsOperating] = new DataColumn(Global.enPFEqParameters.sTimeUnitsOperating.ToString(), Type.GetType("System.String"));
                    dcaPF[(int)Global.enPFEqParameters.iIntervalDistance] = new DataColumn(Global.enPFEqParameters.iIntervalDistance.ToString(), Type.GetType("System.Int32"));
                    dcaPF[(int)Global.enPFEqParameters.sDistanceUnits] = new DataColumn(Global.enPFEqParameters.sDistanceUnits.ToString(), Type.GetType("System.String"));
                    dcaPF[(int)Global.enPFEqParameters.sComment] = new DataColumn(Global.enPFEqParameters.sComment.ToString(), Type.GetType("System.String"));
                    foreach (DataColumn col in dcaPF)
                    {
                        col.AllowDBNull = false;
                        if (col.ColumnName == "sComment" || col.ColumnName == "DStart" || col.ColumnName == "DEnd") col.AllowDBNull = true;
                        dt_Pattern.Columns.Add(col);
                    }

                    return dt_Pattern;
                #endregion

                #region Equipment Aging Items
                case Global.enLL.EquipAgingItems:
                    dcaPF = new DataColumn[Enum.GetNames(typeof(Global.enPFEqAgingItems)).Length];

                    dcaPF[(int)Global.enPFEqAgingItems.i1Sort] = new DataColumn(Global.enPFEqAgingItems.i1Sort.ToString(), Type.GetType("System.Int32"));
                    dcaPF[(int)Global.enPFEqAgingItems.ID] = new DataColumn(Global.enPFEqAgingItems.ID.ToString(), Type.GetType("System.Int32"));
                    dcaPF[(int)Global.enPFEqAgingItems.sName] = new DataColumn(Global.enPFEqAgingItems.sName.ToString(), Type.GetType("System.String"));
                    dcaPF[(int)Global.enPFEqAgingItems.iEquipComponent] = new DataColumn(Global.enPFEqAgingItems.iEquipComponent.ToString(), Type.GetType("System.Int32"));
                    dcaPF[(int)Global.enPFEqAgingItems.sComponent] = new DataColumn(Global.enPFEqAgingItems.sComponent.ToString(), Type.GetType("System.String"));
                    dcaPF[(int)Global.enPFEqAgingItems.iParam] = new DataColumn(Global.enPFEqAgingItems.iParam.ToString(), Type.GetType("System.Int32"));
                    dcaPF[(int)Global.enPFEqAgingItems.iOpCal] = new DataColumn(Global.enPFEqAgingItems.iOpCal.ToString(), Type.GetType("System.Int32"));
                    dcaPF[(int)Global.enPFEqAgingItems.sOpsCalName] = new DataColumn(Global.enPFEqAgingItems.sOpsCalName.ToString(), Type.GetType("System.String"));
                    dcaPF[(int)Global.enPFEqAgingItems.sShortDescript] = new DataColumn(Global.enPFEqAgingItems.sShortDescript.ToString(), Type.GetType("System.String"));
                    dcaPF[(int)Global.enPFEqAgingItems.DStart] = new DataColumn(Global.enPFEqAgingItems.DStart.ToString(), Type.GetType("System.DateTimeOffset"));
                    dcaPF[(int)Global.enPFEqAgingItems.DEnd] = new DataColumn(Global.enPFEqAgingItems.DEnd.ToString(), Type.GetType("System.DateTimeOffset"));
                    dcaPF[(int)Global.enPFEqAgingItems.dEstRunDays] = new DataColumn(Global.enPFEqAgingItems.dEstRunDays.ToString(), Type.GetType("System.Decimal"));
                    dcaPF[(int)Global.enPFEqAgingItems.bRunExtrap] = new DataColumn(Global.enPFEqAgingItems.bRunExtrap.ToString(), Type.GetType("System.Boolean"));
                    dcaPF[(int)Global.enPFEqAgingItems.dEstCycleDays] = new DataColumn(Global.enPFEqAgingItems.dEstCycleDays.ToString(), Type.GetType("System.Decimal"));
                    dcaPF[(int)Global.enPFEqAgingItems.bCyclExtrap] = new DataColumn(Global.enPFEqAgingItems.bCyclExtrap.ToString(), Type.GetType("System.Boolean"));
                    dcaPF[(int)Global.enPFEqAgingItems.dEstDistDays] = new DataColumn(Global.enPFEqAgingItems.dEstDistDays.ToString(), Type.GetType("System.Decimal"));
                    dcaPF[(int)Global.enPFEqAgingItems.bDistExtrap] = new DataColumn(Global.enPFEqAgingItems.bDistExtrap.ToString(), Type.GetType("System.Boolean"));
                    dcaPF[(int)Global.enPFEqAgingItems.dEstDuration] = new DataColumn(Global.enPFEqAgingItems.dEstDuration.ToString(), Type.GetType("System.Decimal"));
                    dcaPF[(int)Global.enPFEqAgingItems.sComment] = new DataColumn(Global.enPFEqAgingItems.sComment.ToString(), Type.GetType("System.String"));
                    foreach (DataColumn col in dcaPF)
                    {
                        col.AllowDBNull = false;
                        if (col.ColumnName == "sComment" ) col.AllowDBNull = true;
                        dt_Pattern.Columns.Add(col);
                    }

                    return dt_Pattern;
                #endregion

                #region Equipment Operating Data
                case Global.enLL.EquipOpData:
                    dcaPF = new DataColumn[Enum.GetNames(typeof(Global.enPFEqOpData)).Length];

                    dcaPF[(int)Global.enPFEqOpData.i1Sort] = new DataColumn(Global.enPFEqOpData.i1Sort.ToString(), Type.GetType("System.Int32"));
                    dcaPF[(int)Global.enPFEqOpData.ID] = new DataColumn(Global.enPFEqOpData.ID.ToString(), Type.GetType("System.Int32"));
                    dcaPF[(int)Global.enPFEqOpData.iEquipComponent] = new DataColumn(Global.enPFEqOpData.iEquipComponent.ToString(), Type.GetType("System.Int32"));
                    dcaPF[(int)Global.enPFEqOpData.sComponent] = new DataColumn(Global.enPFEqOpData.sComponent.ToString(), Type.GetType("System.String"));
                    dcaPF[(int)Global.enPFEqOpData.DFrom] = new DataColumn(Global.enPFEqOpData.DFrom.ToString(), Type.GetType("System.DateTimeOffset"));
                    dcaPF[(int)Global.enPFEqOpData.DTo] = new DataColumn(Global.enPFEqOpData.DTo.ToString(), Type.GetType("System.DateTimeOffset"));
                    dcaPF[(int)Global.enPFEqOpData.dHours] = new DataColumn(Global.enPFEqOpData.dHours.ToString(), Type.GetType("System.Decimal"));
                    dcaPF[(int)Global.enPFEqOpData.iCycles] = new DataColumn(Global.enPFEqOpData.iCycles.ToString(), Type.GetType("System.Int32"));
                    dcaPF[(int)Global.enPFEqOpData.dDistance] = new DataColumn(Global.enPFEqOpData.dDistance.ToString(), Type.GetType("System.Decimal"));
                    dcaPF[(int)Global.enPFEqOpData.sDistanceUnits] = new DataColumn(Global.enPFEqOpData.sDistanceUnits.ToString(), Type.GetType("System.String"));
                    dcaPF[(int)Global.enPFEqOpData.cSource] = new DataColumn(Global.enPFEqOpData.cSource.ToString(), Type.GetType("System.Char"));
                    dcaPF[(int)Global.enPFEqOpData.sComment] = new DataColumn(Global.enPFEqOpData.sComment.ToString(), Type.GetType("System.String"));
                    foreach (DataColumn col in dcaPF)
                    {
                        col.AllowDBNull = false;
                        if (col.ColumnName == "sComment") col.AllowDBNull = true;
                        dt_Pattern.Columns.Add(col);
                    }

                    return dt_Pattern;
                #endregion

                #region Equipment Action Items
                case Global.enLL.EquipActionItems:
                    dcaPF = new DataColumn[Enum.GetNames(typeof(Global.enPFEqActionItems)).Length];

                    dcaPF[(int)Global.enPFEqActionItems.i1Sort] = new DataColumn(Global.enPFEqActionItems.i1Sort.ToString(), Type.GetType("System.Int32"));
                    dcaPF[(int)Global.enPFEqActionItems.ID] = new DataColumn(Global.enPFEqActionItems.ID.ToString(), Type.GetType("System.Int32"));
                    dcaPF[(int)Global.enPFEqActionItems.iEquipAgingItem] = new DataColumn(Global.enPFEqActionItems.iEquipAgingItem.ToString(), Type.GetType("System.Int32"));
                    dcaPF[(int)Global.enPFEqActionItems.sName] = new DataColumn(Global.enPFEqActionItems.sName.ToString(), Type.GetType("System.String"));
                    dcaPF[(int)Global.enPFEqActionItems.DeadLine] = new DataColumn(Global.enPFEqActionItems.DeadLine.ToString(), Type.GetType("System.DateTimeOffset"));
                    dcaPF[(int)Global.enPFEqActionItems.dDeadlineHrs] = new DataColumn(Global.enPFEqActionItems.dDeadlineHrs.ToString(), Type.GetType("System.Decimal"));
                    dcaPF[(int)Global.enPFEqActionItems.iDeadLineCycles] = new DataColumn(Global.enPFEqActionItems.iDeadLineCycles.ToString(), Type.GetType("System.Int32"));
                    dcaPF[(int)Global.enPFEqActionItems.dDeadLineDist] = new DataColumn(Global.enPFEqActionItems.dDeadLineDist.ToString(), Type.GetType("System.Decimal"));
                    dcaPF[(int)Global.enPFEqActionItems.DScheduledStart] = new DataColumn(Global.enPFEqActionItems.DScheduledStart.ToString(), Type.GetType("System.DateTimeOffset"));
                    dcaPF[(int)Global.enPFEqActionItems.DActualStart] = new DataColumn(Global.enPFEqActionItems.DActualStart.ToString(), Type.GetType("System.DateTimeOffset"));
                    dcaPF[(int)Global.enPFEqActionItems.iPercentComplete] = new DataColumn(Global.enPFEqActionItems.iPercentComplete.ToString(), Type.GetType("System.Int32"));
                    dcaPF[(int)Global.enPFEqActionItems.DComplete] = new DataColumn(Global.enPFEqActionItems.DComplete.ToString(), Type.GetType("System.DateTimeOffset"));
                    dcaPF[(int)Global.enPFEqActionItems.dAtCompletionHrs] = new DataColumn(Global.enPFEqActionItems.dAtCompletionHrs.ToString(), Type.GetType("System.Decimal"));
                    dcaPF[(int)Global.enPFEqActionItems.iAtCompletionCycles] = new DataColumn(Global.enPFEqActionItems.iAtCompletionCycles.ToString(), Type.GetType("System.Int32"));
                    dcaPF[(int)Global.enPFEqActionItems.dAtCompletionDist] = new DataColumn(Global.enPFEqActionItems.dAtCompletionDist.ToString(), Type.GetType("System.Decimal"));
                    dcaPF[(int)Global.enPFEqActionItems.sComment] = new DataColumn(Global.enPFEqActionItems.sComment.ToString(), Type.GetType("System.String"));
                    dcaPF[(int)Global.enPFEqActionItems.sUpdateStatus] = new DataColumn(Global.enPFEqActionItems.sUpdateStatus.ToString(), Type.GetType("System.String"));
                    foreach (DataColumn col in dcaPF)
                    {
                        col.AllowDBNull = false;
                        if (col.ColumnName == "sComment" || col.ColumnName == "sUpdateStatus") col.AllowDBNull = true;
                        dt_Pattern.Columns.Add(col);
                    }

                    return dt_Pattern;
                #endregion

                #region Daily Flight Logs
                case Global.enLL.DailyFlightLogs:
                    dcaPF = new DataColumn[Enum.GetNames(typeof(Global.enPFDailyFlightLogs)).Length];

                    dcaPF[(int)Global.enPFDailyFlightLogs.ID] = new DataColumn(Global.enPFDailyFlightLogs.ID.ToString(), Type.GetType("System.Int32"));
                    dcaPF[(int)Global.enPFDailyFlightLogs.iFlightCount] = new DataColumn(Global.enPFDailyFlightLogs.iFlightCount.ToString(), Type.GetType("System.Int32"));
                    dcaPF[(int)Global.enPFDailyFlightLogs.iFlightsPosted] = new DataColumn(Global.enPFDailyFlightLogs.iFlightsPosted.ToString(), Type.GetType("System.Int32"));
                    dcaPF[(int)Global.enPFDailyFlightLogs.DFlightOps] = new DataColumn(Global.enPFDailyFlightLogs.DFlightOps.ToString(), Type.GetType("System.DateTimeOffset"));
                    dcaPF[(int)Global.enPFDailyFlightLogs.sFldMgr] = new DataColumn(Global.enPFDailyFlightLogs.sFldMgr.ToString(), Type.GetType("System.String"));
                    dcaPF[(int)Global.enPFDailyFlightLogs.iMainTowEquip] = new DataColumn(Global.enPFDailyFlightLogs.iMainTowEquip.ToString(), Type.GetType("System.Int32"));
                    dcaPF[(int)Global.enPFDailyFlightLogs.sShortEquipName] = new DataColumn(Global.enPFDailyFlightLogs.sShortEquipName.ToString(), Type.GetType("System.String"));
                    dcaPF[(int)Global.enPFDailyFlightLogs.iMainTowOp] = new DataColumn(Global.enPFDailyFlightLogs.iMainTowOp.ToString(), Type.GetType("System.Int32"));
                    dcaPF[(int)Global.enPFDailyFlightLogs.sDisplayName] = new DataColumn(Global.enPFDailyFlightLogs.sDisplayName.ToString(), Type.GetType("System.String"));
                    dcaPF[(int)Global.enPFDailyFlightLogs.iMainGlider] = new DataColumn(Global.enPFDailyFlightLogs.iMainGlider.ToString(), Type.GetType("System.Int32"));
                    dcaPF[(int)Global.enPFDailyFlightLogs.sMainGliderName] = new DataColumn(Global.enPFDailyFlightLogs.sMainGliderName.ToString(), Type.GetType("System.String"));
                    dcaPF[(int)Global.enPFDailyFlightLogs.iMainLaunchMethod] = new DataColumn(Global.enPFDailyFlightLogs.iMainLaunchMethod.ToString(), Type.GetType("System.Int32"));
                    dcaPF[(int)Global.enPFDailyFlightLogs.sMainLaunchMethod] = new DataColumn(Global.enPFDailyFlightLogs.sMainLaunchMethod.ToString(), Type.GetType("System.String"));
                    dcaPF[(int)Global.enPFDailyFlightLogs.iMainLocation] = new DataColumn(Global.enPFDailyFlightLogs.iMainLocation.ToString(), Type.GetType("System.Int32"));
                    dcaPF[(int)Global.enPFDailyFlightLogs.sLocation] = new DataColumn(Global.enPFDailyFlightLogs.sLocation.ToString(), Type.GetType("System.String"));
                    dcaPF[(int)Global.enPFDailyFlightLogs.mTotalCollected] = new DataColumn(Global.enPFDailyFlightLogs.mTotalCollected.ToString(), Type.GetType("System.Decimal"));
                    dcaPF[(int)Global.enPFDailyFlightLogs.sNotes] = new DataColumn(Global.enPFDailyFlightLogs.sNotes.ToString(), Type.GetType("System.String"));
                    foreach (DataColumn col in dcaPF)
                    {
                        col.AllowDBNull = true;
                        if (col.ColumnName == "DFlightOps" || col.ColumnName == "iMainLocation" || col.ColumnName == "iMainLaunchMethod") col.AllowDBNull = false;
                        dt_Pattern.Columns.Add(col);
                    }

                    return dt_Pattern;
                #endregion

                #region Flight Log Rows
                case Global.enLL.FlightLogRows:
                    dcaPF = new DataColumn[Enum.GetNames(typeof(Global.enPFFlightLogRowsPlus)).Length];
                    dcaPF[(int)Global.enPFFlightLogRowsPlus.ID] = new DataColumn(Global.enPFFlightLogRowsPlus.ID.ToString(), Type.GetType("System.Int32"));
                    dcaPF[(int)Global.enPFFlightLogRowsPlus.iFliteLog] = new DataColumn(Global.enPFFlightLogRowsPlus.iFliteLog.ToString(), Type.GetType("System.Int32"));
                    dcaPF[(int)Global.enPFFlightLogRowsPlus.cStatus] = new DataColumn(Global.enPFFlightLogRowsPlus.cStatus.ToString(), Type.GetType("System.Char"));
                    dcaPF[(int)Global.enPFFlightLogRowsPlus.iTowEquip] = new DataColumn(Global.enPFFlightLogRowsPlus.iTowEquip.ToString(), Type.GetType("System.Int32"));
                    dcaPF[(int)Global.enPFFlightLogRowsPlus.sTowEquipName] = new DataColumn(Global.enPFFlightLogRowsPlus.sTowEquipName.ToString(), Type.GetType("System.String"));
                    dcaPF[(int)Global.enPFFlightLogRowsPlus.iTowOperator] = new DataColumn(Global.enPFFlightLogRowsPlus.iTowOperator.ToString(), Type.GetType("System.Int32"));
                    dcaPF[(int)Global.enPFFlightLogRowsPlus.sTowOperName] = new DataColumn(Global.enPFFlightLogRowsPlus.sTowOperName.ToString(), Type.GetType("System.String"));
                    dcaPF[(int)Global.enPFFlightLogRowsPlus.iGlider] = new DataColumn(Global.enPFFlightLogRowsPlus.iGlider.ToString(), Type.GetType("System.Int32"));
                    dcaPF[(int)Global.enPFFlightLogRowsPlus.sGliderName] = new DataColumn(Global.enPFFlightLogRowsPlus.sGliderName.ToString(), Type.GetType("System.String"));
                    dcaPF[(int)Global.enPFFlightLogRowsPlus.iLaunchMethod] = new DataColumn(Global.enPFFlightLogRowsPlus.iLaunchMethod.ToString(), Type.GetType("System.Int32"));
                    dcaPF[(int)Global.enPFFlightLogRowsPlus.sLaunchMethod] = new DataColumn(Global.enPFFlightLogRowsPlus.sLaunchMethod.ToString(), Type.GetType("System.String"));
                    dcaPF[(int)Global.enPFFlightLogRowsPlus.iPilot1] = new DataColumn(Global.enPFFlightLogRowsPlus.iPilot1.ToString(), Type.GetType("System.Int32"));
                    dcaPF[(int)Global.enPFFlightLogRowsPlus.sPilot1] = new DataColumn(Global.enPFFlightLogRowsPlus.sPilot1.ToString(), Type.GetType("System.String"));
                    dcaPF[(int)Global.enPFFlightLogRowsPlus.iAviatorRole1] = new DataColumn(Global.enPFFlightLogRowsPlus.iAviatorRole1.ToString(), Type.GetType("System.Int32"));
                    dcaPF[(int)Global.enPFFlightLogRowsPlus.sAviatorRole1] = new DataColumn(Global.enPFFlightLogRowsPlus.sAviatorRole1.ToString(), Type.GetType("System.String"));
                    dcaPF[(int)Global.enPFFlightLogRowsPlus.dPctCharge1] = new DataColumn(Global.enPFFlightLogRowsPlus.dPctCharge1.ToString(), Type.GetType("System.Decimal"));
                    dcaPF[(int)Global.enPFFlightLogRowsPlus.iPilot2] = new DataColumn(Global.enPFFlightLogRowsPlus.iPilot2.ToString(), Type.GetType("System.Int32"));
                    dcaPF[(int)Global.enPFFlightLogRowsPlus.sPilot2] = new DataColumn(Global.enPFFlightLogRowsPlus.sPilot2.ToString(), Type.GetType("System.String"));
                    dcaPF[(int)Global.enPFFlightLogRowsPlus.iAviatorRole2] = new DataColumn(Global.enPFFlightLogRowsPlus.iAviatorRole2.ToString(), Type.GetType("System.Int32"));
                    dcaPF[(int)Global.enPFFlightLogRowsPlus.sAviatorRole2] = new DataColumn(Global.enPFFlightLogRowsPlus.sAviatorRole2.ToString(), Type.GetType("System.String"));
                    dcaPF[(int)Global.enPFFlightLogRowsPlus.dPctCharge2] = new DataColumn(Global.enPFFlightLogRowsPlus.dPctCharge2.ToString(), Type.GetType("System.Decimal"));
                    dcaPF[(int)Global.enPFFlightLogRowsPlus.dReleaseAltitude] = new DataColumn(Global.enPFFlightLogRowsPlus.dReleaseAltitude.ToString(), Type.GetType("System.Decimal"));
                    dcaPF[(int)Global.enPFFlightLogRowsPlus.dMaxAltitude] = new DataColumn(Global.enPFFlightLogRowsPlus.dMaxAltitude.ToString(), Type.GetType("System.Decimal"));
                    dcaPF[(int)Global.enPFFlightLogRowsPlus.iLocTakeOff] = new DataColumn(Global.enPFFlightLogRowsPlus.iLocTakeOff.ToString(), Type.GetType("System.Int32"));
                    dcaPF[(int)Global.enPFFlightLogRowsPlus.sLocTakeOff] = new DataColumn(Global.enPFFlightLogRowsPlus.sLocTakeOff.ToString(), Type.GetType("System.String"));
                    dcaPF[(int)Global.enPFFlightLogRowsPlus.DTakeOff] = new DataColumn(Global.enPFFlightLogRowsPlus.DTakeOff.ToString(), Type.GetType("System.DateTimeOffset"));
                    dcaPF[(int)Global.enPFFlightLogRowsPlus.iLocLanding] = new DataColumn(Global.enPFFlightLogRowsPlus.iLocLanding.ToString(), Type.GetType("System.Int32"));
                    dcaPF[(int)Global.enPFFlightLogRowsPlus.sLocLanding] = new DataColumn(Global.enPFFlightLogRowsPlus.sLocLanding.ToString(), Type.GetType("System.String"));
                    dcaPF[(int)Global.enPFFlightLogRowsPlus.DLanding] = new DataColumn(Global.enPFFlightLogRowsPlus.DLanding.ToString(), Type.GetType("System.DateTimeOffset"));
                    dcaPF[(int)Global.enPFFlightLogRowsPlus.iChargeCode] = new DataColumn(Global.enPFFlightLogRowsPlus.iChargeCode.ToString(), Type.GetType("System.Int32"));
                    dcaPF[(int)Global.enPFFlightLogRowsPlus.cChargeCode] = new DataColumn(Global.enPFFlightLogRowsPlus.cChargeCode.ToString(), Type.GetType("System.Char"));
                    dcaPF[(int)Global.enPFFlightLogRowsPlus.sChargeCode] = new DataColumn(Global.enPFFlightLogRowsPlus.sChargeCode.ToString(), Type.GetType("System.String"));
                    dcaPF[(int)Global.enPFFlightLogRowsPlus.mAmtCollected] = new DataColumn(Global.enPFFlightLogRowsPlus.mAmtCollected.ToString(), Type.GetType("System.Decimal"));
                    dcaPF[(int)Global.enPFFlightLogRowsPlus.sComments] = new DataColumn(Global.enPFFlightLogRowsPlus.sComments.ToString(), Type.GetType("System.String"));
                    foreach (DataColumn col in dcaPF)
                    {
                        if (col != null)
                        {
                            col.AllowDBNull = false;
                            if (col.ColumnName == "dReleaseAltitude" || col.ColumnName == "dMaxAltitude" ||
                                col.ColumnName == "mAmtCollected" || col.ColumnName == "sComments") col.AllowDBNull = true;
                            dt_Pattern.Columns.Add(col);
                        }
                    }
                    return dt_Pattern;
                #endregion

                #region Member Equity Shares Journal
                case Global.enLL.MeEquityShJ:
                    dcaPF = new DataColumn[Enum.GetNames(typeof(Global.enPFMeEquityShJ)).Length];
                    dcaPF[(int)Global.enPFMeEquityShJ.ID] = new DataColumn(Global.enPFMeEquityShJ.ID.ToString(), Type.GetType("System.Int32"));
                    dcaPF[(int)Global.enPFMeEquityShJ.iOwner] = new DataColumn(Global.enPFMeEquityShJ.iOwner.ToString(), Type.GetType("System.Int32"));
                    dcaPF[(int)Global.enPFMeEquityShJ.sDisplayName] = new DataColumn(Global.enPFMeEquityShJ.sDisplayName.ToString(), Type.GetType("System.String"));
                    dcaPF[(int)Global.enPFMeEquityShJ.DXaction] = new DataColumn(Global.enPFMeEquityShJ.DXaction.ToString(), Type.GetType("System.DateTime"));
                    dcaPF[(int)Global.enPFMeEquityShJ.cDateQuality] = new DataColumn(Global.enPFMeEquityShJ.cDateQuality.ToString(), Type.GetType("System.Char"));
                    dcaPF[(int)Global.enPFMeEquityShJ.dNumShares] = new DataColumn(Global.enPFMeEquityShJ.dNumShares.ToString(), Type.GetType("System.Decimal"));
                    dcaPF[(int)Global.enPFMeEquityShJ.cXactType] = new DataColumn(Global.enPFMeEquityShJ.cXactType.ToString(), Type.GetType("System.Char"));
                    dcaPF[(int)Global.enPFMeEquityShJ.sInfoSource] = new DataColumn(Global.enPFMeEquityShJ.sInfoSource.ToString(), Type.GetType("System.String"));
                    dcaPF[(int)Global.enPFMeEquityShJ.sComment] = new DataColumn(Global.enPFMeEquityShJ.sComment.ToString(), Type.GetType("System.String"));
                    foreach (DataColumn col in dcaPF)
                    {
                        if (col != null)
                        {
                            col.AllowDBNull = false;
                            if (col.ColumnName == "sInfoSource" || col.ColumnName == "sComments") col.AllowDBNull = true;
                            dt_Pattern.Columns.Add(col);
                        }
                    }
                    return dt_Pattern;
                #endregion

                #region  Flight Operations Schedule Dates
                case Global.enLL.FltOpsSchedDates:
                    dcaPF = new DataColumn[Enum.GetNames(typeof(Global.enPFFltOpsSchedDates)).Length];
                    dcaPF[(int)Global.enPFFltOpsSchedDates.i1Sort] = new DataColumn(Global.enPFFltOpsSchedDates.i1Sort.ToString(), Type.GetType("System.Int32"));
                    dcaPF[(int)Global.enPFFltOpsSchedDates.ID] = new DataColumn(Global.enPFFltOpsSchedDates.ID.ToString(), Type.GetType("System.Int32"));
                    dcaPF[(int)Global.enPFFltOpsSchedDates.Date] = new DataColumn(Global.enPFFltOpsSchedDates.Date.ToString(), Type.GetType("System.DateTime"));
                    dcaPF[(int)Global.enPFFltOpsSchedDates.bEnabled] = new DataColumn(Global.enPFFltOpsSchedDates.bEnabled.ToString(), Type.GetType("System.Boolean"));
                    dcaPF[(int)Global.enPFFltOpsSchedDates.sNote] = new DataColumn(Global.enPFFltOpsSchedDates.sNote.ToString(), Type.GetType("System.String"));
                    foreach (DataColumn col in dcaPF)
                    {
                        if (col != null)
                        {
                            col.AllowDBNull = false;
                            if (col.ColumnName == "sNote") col.AllowDBNull = true;
                            dt_Pattern.Columns.Add(col);
                        }
                    }
                    return dt_Pattern;
                #endregion

                default:
                    return null;
            }
        }

        #region makeDataRow
        static private DataRow makeDataRow(SRowDataExp rdu, Global.enLL euL)
        {
            DataTable dt_Pattern = new DataTable();
            dt_Pattern = dtSchema(euL);
            DataRow dtRow = dt_Pattern.NewRow();
            dtRow[(int)Global.enPFExp.LineNum] = 1;
            dtRow[(int)Global.enPFExp.AccountID] = rdu.iAcct;
            dtRow[(int)Global.enPFExp.AccountName] = rdu.sAcct;
            dtRow[(int)Global.enPFExp.Descr] = rdu.sDescr;
            dtRow[(int)Global.enPFExp.sAmount] = rdu.sAmount;
            return dtRow;
        }

        static private DataRow makeDataRow(SRowDataPmt rdu, Global.enLL euL)
        {
            DataTable dt_Pattern = new DataTable();
            dt_Pattern = dtSchema(euL);
            DataRow dtRow = dt_Pattern.NewRow();
            dtRow[(int)Global.enPFPmt.LineNum] = 1;
            dtRow[(int)Global.enPFPmt.PmtMethodID] = rdu.iPmtMethod;
            dtRow[(int)Global.enPFPmt.PmtMethodName] = rdu.sPmtMethod;
            dtRow[(int)Global.enPFPmt.AccountID] = rdu.iAcct;
            dtRow[(int)Global.enPFPmt.AccountName] = rdu.sAcct;
            dtRow[(int)Global.enPFPmt.Descr] = rdu.sDescr;
            dtRow[(int)Global.enPFPmt.sAmount] = rdu.sAmount;
            return dtRow;
        }

        static public DataRow makeDataRow(SRowDataAtt rdu, Global.enLL euL)
        {
            DataTable dt_Pattern = new DataTable();
            dt_Pattern = dtSchema(euL);
            DataRow dtRow = dt_Pattern.NewRow();
            dtRow[(int)Global.enPFAtt.LineNum] = 1;
            dtRow[(int)Global.enPFAtt.AttachCategID] = rdu.iCateg;
            dtRow[(int)Global.enPFAtt.AttachCategName] = rdu.sCateg;
            dtRow[(int)Global.enPFAtt.AttachAssocDate] = rdu.DAssoc;
            dtRow[(int)Global.enPFAtt.AttachedFileID] = rdu.iFile;
            dtRow[(int)Global.enPFAtt.AttachedFileName] = rdu.sFile;
            dtRow[(int)Global.enPFAtt.AttachedThumbID] = rdu.iThumb;
            dtRow[(int)Global.enPFAtt.AttachedThumbName] = rdu.sThumb;
            return dtRow;
        }
        #endregion

        static private void Renumber(List<DataRow> Lu)
        {
            int NRows = Lu.Count - 2; // Number of rows that get renumbered
            for (int iRow = 0; iRow < NRows; iRow++)
            {
                Lu[iRow][0] = iRow + 1;
            }
        }

        #region Manipulate Lists
        #region Add
        #region Expense
        static public void Add(List<DataRow> Lu, Global.enLL euL, SRowDataExp rdu)
        {
            // We add an item by inserting in the position two from the last
            Lu.Insert(Lu.Count - 2, makeDataRow(rdu, euL));
            Renumber(Lu);
        }
        static public void Add(List<DataRow> Lu, Global.enLL euL, SRowDataPmt rdu)
        {
            // We add an item by inserting in the position two from the last
            Lu.Insert(Lu.Count - 2, makeDataRow(rdu, euL));
            Renumber(Lu);
        }
        static public void Add(List<DataRow> Lu, Global.enLL euL, SRowDataAtt rdu)
        {
            // We add an item by inserting in the position two from the last
            Lu.Insert(Lu.Count - 2, makeDataRow(rdu, euL));
            Renumber(Lu);
        }
        #endregion
        //#region Vendors
        //static public void Add(List<DataRow> Lu, Global.enLL euL, SRowVendor rdu)
        //{
        //    // We add an item by inserting in the position next to last
        //    Lu.Insert(Lu.Count - 1, makeDataRow(rdu, euL));
        //}
        //#endregion
        #endregion
        static public void Remove(List<DataRow> Lu, int iRow)
        {
            DataRow dr = Lu[iRow];
            Lu.Remove(dr);
            Renumber(Lu);
        }
        #region Replace
        static public void Replace(List<DataRow> Lu, Global.enLL euL, int iRow, SRowDataExp rdu)
        {
            Lu[iRow] = makeDataRow(rdu, euL);
        }
        static public void Replace(List<DataRow> Lu, Global.enLL euL, int iRow, SRowDataPmt rdu)
        {
            Lu[iRow] = makeDataRow(rdu, euL);
        }
        static public void Replace(List<DataRow> Lu, Global.enLL euL, int iRow, SRowDataAtt rdu)
        {
            Lu[iRow] = makeDataRow(rdu, euL);
        }
        //static public void Replace(List<DataRow> Lu, Global.enLL euL, int iRow, SRowVendor rdu)
        //{
        //    Lu[iRow] = makeDataRow(rdu, euL);
        //}
        #endregion
        static public void Swap(List<DataRow> Lu, int iRow1, int iRow2)
        {
            DataRow rtmp = Lu[iRow1];
            Lu[iRow1] = Lu[iRow2];
            Lu[iRow2] = rtmp;
            Renumber(Lu);
        }

        static public string Sum(List<DataRow> Lu, Global.enLL euL)
        {
            decimal dSum = 0.00m;
            if (euL == Global.enLL.AttachedFiles)
            {
                dSum = Lu.Count - 2;
                return dSum.ToString("N0", CultureInfo.InvariantCulture);
            }
            else
            {
                for (int iNode = 0; iNode < Lu.Count - 2; iNode++)
                {
                    switch (euL)
                    {
                        case Global.enLL.Expenditure:
                            dSum += decimal.Parse((string)Lu[iNode][(int)Global.enPFExp.sAmount]);
                            break;
                        case Global.enLL.Payment:
                            dSum += decimal.Parse((string)Lu[iNode][(int)Global.enPFPmt.sAmount]);
                            break;
                    }
                }
                return dSum.ToString("N2", CultureInfo.InvariantCulture);
            }
        }
        #endregion

        #region Membership AccountProfile Support
        static public DataTable dtGetRewardsFilterSettings(out bool buNewVersionDetected)
        {
            buNewVersionDetected = false;
            DataTable dtrf = AccountProfile.CurrentUser.RewardsFilterSettings;
            bool b = false;
            if (dtrf.Rows.Count < 1)
            {
                b = true;
            }
            else
            {
                // The LowLimit property in the very first row of the filter settings DataTable is used to store the
                //     filter settings version
                if (((int)dtrf.Rows[(int)Global.egRewardsFilters.Version][(int)Global.egRewardsFilterProps.Integ32]) !=
                        Global.dgcVersionOfRewardsFilterSettingsDataTable)
                {
                    buNewVersionDetected = true;
                    b = true;
                }
            }
            if (b)
            {
                // There was no entry in table aspnet_Profile for this user, or a version conflict is forcing a reset of dtu
                dtrf = dtDefaultRewardsFilter();
                // Save filter settings for this user
                AccountProfile.CurrentUser.RewardsFilterSettings = dtrf;
            }
            return dtrf;
        }

        static private DataTable dtDefaultRewardsFilter()
        {
            DataTable dt = new DataTable("RewardsFilterSettings");

            #region columns
            // Each column represents a filter property
            #region 0 Filter Name
            DataColumn column = new DataColumn
            {
                DataType = System.Type.GetType("System.String"),
                ColumnName = "sFilterName",
                Unique = true
            };
            dt.Columns.Add(column);
            #endregion
            #region 1 Filter Type
            column = new DataColumn
            {
                DataType = System.Type.GetType("System.String"),
                ColumnName = "sFilterType",
                Unique = false // Filter Types are: YesNo, DateList, List, Integ32
            };
            dt.Columns.Add(column);
            #endregion
            #region 2 First Boolean - Is the filter Enabled?
            column = new DataColumn
            {
                DataType = System.Type.GetType("System.Boolean"),
                ColumnName = "bEnabled" // is this filter to be used at all?
            };
            dt.Columns.Add(column);
            #endregion
            #region 3 List of comma separated items, could also be just a single item
            column = new DataColumn
            {
                DataType = System.Type.GetType("System.String"),
                ColumnName = "sList"
            };
            dt.Columns.Add(column);
            #endregion
            #region 4 Second Boolean YesNo
            column = new DataColumn
            {
                DataType = System.Type.GetType("System.Boolean"),
                ColumnName = "YesNo" // If true, the bottom N records are displayed
            };
            dt.Columns.Add(column);
            #endregion
            #region 5 Integer
            column = new DataColumn
            {
                DataType = System.Type.GetType("System.Int32"),
                ColumnName = "Integ32"
            };
            dt.Columns.Add(column);
            #endregion
            #endregion
            #region rows
            // Create and add the table rows. Each row represents a particular filter, except the first row is a version number:
            #region 0 FilterVersion
            DataRow row = dt.NewRow();
            row[(int)Global.egRewardsFilterProps.FilterName] = "FilterVersion";
            row[(int)Global.egRewardsFilterProps.FilterType] = "Integ32";
            row[(int)Global.egRewardsFilterProps.Enabled] = true;
            row[(int)Global.egRewardsFilterProps.List] = ""; // not used
            row[(int)Global.egRewardsFilterProps.YesNo] = false; // not used
            row[(int)Global.egRewardsFilterProps.Integ32] = 1; // The actual Filter Verson number
            dt.Rows.Add(row);
            RowCountCheck(dt, (int)Global.egRewardsFilters.Version, (string)row[(int)Global.egRewardsFilterProps.FilterName]);
            #endregion
            #region 1 EnableFilteringOverall
            row = dt.NewRow();
            row[(int)Global.egRewardsFilterProps.FilterName] = "EnableFilteringOverall";
            row[(int)Global.egRewardsFilterProps.FilterType] = "Boolean";
            row[(int)Global.egRewardsFilterProps.Enabled] = true;
            row[(int)Global.egRewardsFilterProps.List] = ""; // not used
            row[(int)Global.egRewardsFilterProps.YesNo] = false; // not used
            row[(int)Global.egRewardsFilterProps.Integ32] = 0; // not used
            dt.Rows.Add(row);
            RowCountCheck(dt, (int)Global.egRewardsFilters.EnableFilteringOverall, (string)row[(int)Global.egRewardsFilterProps.FilterName]);
            #endregion
            #region 2 Member
            row = dt.NewRow();
            row[(int)Global.egRewardsFilterProps.FilterName] = "Member";
            row[(int)Global.egRewardsFilterProps.FilterType] = "Integ32";
            row[(int)Global.egRewardsFilterProps.Enabled] = false;
            row[(int)Global.egRewardsFilterProps.List] = ""; // not used
            row[(int)Global.egRewardsFilterProps.YesNo] = false; // not used
            row[(int)Global.egRewardsFilterProps.Integ32] = 0;
            dt.Rows.Add(row);
            RowCountCheck(dt, (int)Global.egRewardsFilters.Member, (string)row[(int)Global.egRewardsFilterProps.FilterName]);
            #endregion
            #region 3 Earn/Claim Date
            row = dt.NewRow();
            row[(int)Global.egRewardsFilterProps.FilterName] = "EarnClaimDate";
            row[(int)Global.egRewardsFilterProps.FilterType] = "DateList";
            row[(int)Global.egRewardsFilterProps.Enabled] = false;
            row[(int)Global.egRewardsFilterProps.List] = "2000-01-01 00:00 -08:00, 2099-12-31 23:59 -08:00";
            row[(int)Global.egRewardsFilterProps.YesNo] = false; // not used
            row[(int)Global.egRewardsFilterProps.Integ32] = 0; // not used
            dt.Rows.Add(row);
            RowCountCheck(dt, (int)Global.egRewardsFilters.EarnClaimDate, (string)row[(int)Global.egRewardsFilterProps.FilterName]);
            #endregion
            #region 4 Show Expired Entries?
            row = dt.NewRow();
            row[(int)Global.egRewardsFilterProps.FilterName] = "ShowExpired";
            row[(int)Global.egRewardsFilterProps.FilterType] = "Boolean";
            row[(int)Global.egRewardsFilterProps.Enabled] = false;
            row[(int)Global.egRewardsFilterProps.List] = ""; // not used
            row[(int)Global.egRewardsFilterProps.YesNo] = false; // not used
            row[(int)Global.egRewardsFilterProps.Integ32] = 0; // not used
            dt.Rows.Add(row);
            RowCountCheck(dt, (int)Global.egRewardsFilters.ShowExpired, (string)row[(int)Global.egRewardsFilterProps.FilterName]);
            #endregion
            #region 5 Earn/Claim Code
            row = dt.NewRow();
            row[(int)Global.egRewardsFilterProps.FilterName] = "EarnClaimCode";
            row[(int)Global.egRewardsFilterProps.FilterType] = "List";
            row[(int)Global.egRewardsFilterProps.Enabled] = false;
            row[(int)Global.egRewardsFilterProps.List] = "C";
            row[(int)Global.egRewardsFilterProps.YesNo] = false; // not used
            row[(int)Global.egRewardsFilterProps.Integ32] = 0; // not used
            dt.Rows.Add(row);
            RowCountCheck(dt, (int)Global.egRewardsFilters.EarnClaimCode, (string)row[(int)Global.egRewardsFilterProps.FilterName]);
            #endregion
            #region 6 Limit the number of rows displayed; 2 means at the bottom, 1 means at the top
            row = dt.NewRow();
            row[(int)Global.egRewardsFilterProps.FilterName] = "LimitAtTopBottom";
            row[(int)Global.egRewardsFilterProps.FilterType] = "Integ32";
            row[(int)Global.egRewardsFilterProps.Enabled] = true;
            row[(int)Global.egRewardsFilterProps.List] = ""; // not used
            row[(int)Global.egRewardsFilterProps.YesNo] = true; // not used
            row[(int)Global.egRewardsFilterProps.Integ32] = 2; // default is bottom
            dt.Rows.Add(row);
            RowCountCheck(dt, (int)Global.egRewardsFilters.LimitAtTopBottom, (string)row[(int)Global.egRewardsFilterProps.FilterName]);
            #endregion
            #region 7 The actual limit for the row count at top or at bottom (if the LimitAtTopBottom filter is turned on)
            row = dt.NewRow();
            row[(int)Global.egRewardsFilterProps.FilterName] = "LimitRowCount";
            row[(int)Global.egRewardsFilterProps.FilterType] = "Integ32";
            row[(int)Global.egRewardsFilterProps.Enabled] = true; // not used, is handled together with LimitAtTopBottom
            row[(int)Global.egRewardsFilterProps.List] = ""; // not used
            row[(int)Global.egRewardsFilterProps.YesNo] = false; // not used
            row[(int)Global.egRewardsFilterProps.Integ32] = 25; // between 1 and 200
            dt.Rows.Add(row);
            RowCountCheck(dt, (int)Global.egRewardsFilters.LimitRowCount, (string)row[(int)Global.egRewardsFilterProps.FilterName]);
            #endregion
            #endregion
            return dt;
        }

        static public void LastUsedInputs_DailyFlightLogs(string suVersion, string suMainTowEquip, string suMainTowOp, string suMainGlider,
            string suMainLaunchMethod, string suMainTowLoc, ref DataTable dtu)
        {
            dtu.Clear();
            #region Columns
            DataColumn column = new DataColumn
            {
                DataType = System.Type.GetType("System.String"),
                ColumnName = "sVersion",
                Unique = true
            };
            dtu.Columns.Add(column);
            column = new DataColumn
            {
                DataType = System.Type.GetType("System.String"),
                ColumnName = "sMainTowEquipment",
                Unique = false
            };
            dtu.Columns.Add(column);
            column = new DataColumn
            {
                DataType = System.Type.GetType("System.String"),
                ColumnName = "sMainTowOperator",
                Unique = false
            };
            dtu.Columns.Add(column);
            column = new DataColumn
            {
                DataType = System.Type.GetType("System.String"),
                ColumnName = "sMainGlider",
                Unique = false
            };
            dtu.Columns.Add(column);
            column = new DataColumn
            {
                DataType = System.Type.GetType("System.String"),
                ColumnName = "sMainLaunchMethod",
                Unique = false
            };
            dtu.Columns.Add(column);
            column = new DataColumn
            {
                DataType = System.Type.GetType("System.String"),
                ColumnName = "sMainTowLocation",
                Unique = false
            };
            dtu.Columns.Add(column);
            #endregion
            #region Rows (there is just one)
            DataRow row = dtu.NewRow();
            row[0] = suVersion;
            row[1] = suMainTowEquip;
            row[2] = suMainTowOp;
            row[3] = suMainGlider;
            row[4] = suMainLaunchMethod;
            row[5] = suMainTowLoc;
            dtu.Rows.Add(row);
            #endregion
        }

        // The following string array defines data types to go with Global.enPFFlightLogRowItems:
        static readonly string[] saTypes = new string[] { "String", "Char", "String", "String", "String", "String", "String", "String", "Decimal", "String", "String", "Decimal", "Decimal", "Decimal",
                "String", "DateTimeOffset", "String", "DateTimeOffset", "String", "Decimal", "String"};

        static public void LastUsedInputs_FlightLogRows(string[] sau, ref DataTable dtu)
        {
            dtu.Clear();
            #region Columns
            foreach (int i in (int[])Enum.GetValues(typeof(Global.enPFFlightLogRowItems))) {
                DataColumn column = new DataColumn
                {
                    DataType = Type.GetType("System." + saTypes[i]),
                    ColumnName = Enum.GetNames(typeof(Global.enPFFlightLogRowItems))[i]
                };
                dtu.Columns.Add(column);
            }

            #endregion
            #region Rows (there is just one)
            DataRow row = dtu.NewRow();
            // The cells in the single row of this table are filled with items supplied in array sau
            foreach(Global.enPFFlightLogRowItems en in (Global.enPFFlightLogRowItems[])Enum.GetValues(typeof(Global.enPFFlightLogRowItems)))
            {
                switch (saTypes[(int)en])
                {
                    case "Char":
                    case "String":
                        row[(int)en] = sau[(int)en];
                        break;
                    case "Decimal":
                        row[(int)en] = Decimal.Parse(sau[(int)en]);
                        break;
                    case "DateTimeOffset":
                        row[(int)en] = DateTimeOffset.Parse(sau[(int)en]);
                        break;
                }
            }
            dtu.Rows.Add(row);
            #endregion
        }
        #endregion

        static private void RowCountCheck(DataTable udt, int iuRowCount, string suFilterName)
        {
            if ((udt.Rows.Count - 1) != iuRowCount)
            {
                throw new Global.excToPopup("Internal error in AssisLi.cs.dtDefaultRewardsFilter: Inconsistent row count at " + suFilterName);
            }
        }
    }
}