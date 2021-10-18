using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Linq;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.UI.WebControls;
using Newtonsoft.Json;
using TSoar.DB;

namespace TSoar.Accounting
{
    public class XExpense // used while during user interaction preparing a new or editing an existing Xact
    {
        public KeyValuePair<string,int> kvpXCallMode;
        public DateTimeOffset Dxact;
        public string sVendor;
        public List<DataRow> liExpenditure;
        public List<DataRow> liPayment;
        public string sMemo;
        public List<DataRow> liAttachedFile;
    }
    public class JExpense // used while saving a new Xact
    {
        public struct expend
        {
            public string sAccountName;
            public string sDescr;
            public decimal mAmount;
        }
        public struct payment
        {
            public string sPmtMethod;
            public string sAccountName;
            public string sReference;
            public decimal mAmount;
        }
        public struct attach
        {
            public string sAttachCateg;
            public DateTimeOffset DAssoc;
            public string sFile;
            public string sThumb;
        }

        public string sVersion = "1";
        public int iSF_XACT_ID;
        public DateTimeOffset Dxact;
        public string sVendor;
        public expend[] aExpenditures;
        public payment[] aPayments;
        public string sMemo;
        public attach[] aAttachFiles;
    }

    public class XactEng
    {
        #region ReadMe
        // Financial Transaction Engine
        // ============================
        // (Differentiate between 'financial' transactions and 'database procedural' transactions (that can be committed or rolled back).)
        // Recognize various types of financial transactions (abbreviated as Xact / Xacts): table SF_XACTTYPES
        // Offer a number of methods for handling xacts: validate, create new, mark existing as deleted, update existing, provide reporting support, ...
        // Translate the user-supplied Xact data to JSON text
        // Translate JSON text into database actions using Linq-to-SQL
        // Use database transactions to ensure that all db modifications succeed as one block, or fail as one block
        // Use robust error checking throughout; throw exception(s) when errors are detected
        #endregion

        #region Declarations
        private string sXactType = ""; // To match SF_XACTTYPES.sTransactionType
        private XExpense xExpense;
        #endregion

        #region Constructors
        public XactEng() { }
        public XactEng(XExpense uXactExpense)
        {
            xExpense = uXactExpense;
            sXactType = "Expense";
        }
        #endregion

        #region Exceptions
        public class excXactEng : Exception
        {
            private string sp;
            public excXactEng(string su) { sp = su; }
            public string sExcMsg() { return sp; }
        }
        #endregion

        public void Save(int iuXactId)
        {
            switch (sXactType) {
                case "Expense":
                    //string sDbgMsg = "XactEng.cs.Save case Expense - calling sXSerialize";
                    //ActivityLog.oLog(ActivityLog.enumLogTypes.Debug, 14, sDbgMsg);
                    string sJ = sXSerialize();
                    // This is where we might hook in when accepting data from the audit log (which is in JSON text),
                    //    or from external sources which would need to be in JSON text.
                    //sDbgMsg = "XactEng.cs.Save case Expense - calling DeserializeObject";
                    //ActivityLog.oLog(ActivityLog.enumLogTypes.Debug, 14, sDbgMsg);
                    JExpense Xse = JsonConvert.DeserializeObject<JExpense>(sJ);
                    //sDbgMsg = "XactEng.cs.Save case Expense - calling XValidate";
                    //ActivityLog.oLog(ActivityLog.enumLogTypes.Debug, 14, sDbgMsg);
                    XValidate(Xse);
                    //sDbgMsg = "XactEng.cs.Save case Expense - calling XSave";
                    //ActivityLog.oLog(ActivityLog.enumLogTypes.Debug, 14, sDbgMsg);
                    XSave(Xse, iuXactId);
                    break;
                default:
                    throw new excXactEng("Internal error in XactEng.cs.Save: Financial transaction type not known: `" + sXactType + "`");
            }
        }
        private string sXSerialize()
        {
            JExpense Xse = new JExpense();
            Xse.iSF_XACT_ID = xExpense.kvpXCallMode.Value;
            Xse.Dxact = xExpense.Dxact;
            Xse.sVendor = xExpense.sVendor;
            int nx = xExpense.liExpenditure.Count - 2;
            if (nx < 1)
            {
                throw new excXactEng("You have not entered any expenditure data");
            }
            JExpense.expend[] ax = new JExpense.expend[nx];
            for (int iRow = 0; iRow < nx; iRow++)
            {
                ax[iRow].sAccountName = (string)xExpense.liExpenditure[iRow].ItemArray[2];
                ax[iRow].sDescr = (string)xExpense.liExpenditure[iRow].ItemArray[3];
                ax[iRow].mAmount = decimal.Parse((string)xExpense.liExpenditure[iRow].ItemArray[4]);
            }
            Xse.aExpenditures = ax;
            nx = xExpense.liPayment.Count - 2;
            if (nx < 1)
            {
                throw new excXactEng("You have not entered any payment data");
            }
            JExpense.payment[] px = new JExpense.payment[nx];
            for (int iRow = 0; iRow < nx; iRow++)
            {
                px[iRow].sPmtMethod = (string)xExpense.liPayment[iRow].ItemArray[4];
                px[iRow].sAccountName = (string)xExpense.liPayment[iRow].ItemArray[2];
                px[iRow].sReference = (string)xExpense.liPayment[iRow].ItemArray[5];
                px[iRow].mAmount = decimal.Parse((string)xExpense.liPayment[iRow].ItemArray[6]);
            }
            Xse.aPayments = px;
            Xse.sMemo = xExpense.sMemo;
            nx = xExpense.liAttachedFile.Count - 2;
            JExpense.attach[] attach = new JExpense.attach[nx];
            for (int iRow = 0; iRow < nx; iRow++)
            {
                attach[iRow].sAttachCateg = (string)xExpense.liAttachedFile[iRow].ItemArray[2];
                attach[iRow].DAssoc = (DateTimeOffset)xExpense.liAttachedFile[iRow].ItemArray[3];
                attach[iRow].sFile = (string)xExpense.liAttachedFile[iRow].ItemArray[5];
                attach[iRow].sThumb = (string)xExpense.liAttachedFile[iRow].ItemArray[7];
            }
            Xse.aAttachFiles = attach;
            return JsonConvert.SerializeObject(Xse, Formatting.Indented);
        }

        public static string sXSerialize(int iuXactId, sf_AccountingDataContext udc)
        {
            // Because sXSerialize is called from within a TransactionScope, we pass the data connection udc
            //    rather than create a new one in order to avoid having SQL Server try to use 'distributed connections'
            //    which is expensive in terms of computing resources (and MSDTC distributed transaction coordinator is therefore usually disabled).
            JExpense Xse = new JExpense();
            
            Xse.iSF_XACT_ID = iuXactId;
            var m = (from e in udc.SF_XACTs where e.ID == iuXactId select e).First();
            Xse.Dxact = m.D;
            Xse.sVendor = ((from v in udc.SF_VENDORs where v.ID == m.iVendor select v).First()).sVendorName;

            List<JExpense.expend> ax = new List<JExpense.expend>();
            var x = from e in udc.SF_ENTRies
                    where e.iXactId == iuXactId && e.SF_PAYMENTMETHOD.sPaymentMethod == "(none)"
                    select new { e.SF_ACCOUNT.sName, e.sDescription, e.mAmount };
            foreach (var e in x)
            {
                JExpense.expend y = new JExpense.expend
                {
                    sAccountName = e.sName,
                    sDescr = e.sDescription,
                    mAmount = e.mAmount
                };
                ax.Add(y);
            }
            Xse.aExpenditures = ax.ToArray();

            List<JExpense.payment> px = new List<JExpense.payment>();
            var p = from e in udc.SF_ENTRies
                    where e.iXactId == iuXactId && e.SF_PAYMENTMETHOD.sPaymentMethod != "(none)"
                    select new {e.SF_PAYMENTMETHOD.sPaymentMethod, e.SF_ACCOUNT.sName, e.sDescription, e.mAmount };
            foreach (var e in p)
            {
                JExpense.payment y = new JExpense.payment
                {
                    sPmtMethod=e.sPaymentMethod,
                    sAccountName = e.sName,
                    sReference = e.sDescription,
                    mAmount = e.mAmount
                };
                px.Add(y);
            }
            Xse.aPayments = px.ToArray();

            Xse.sMemo = m.sMemo;

            List<JExpense.attach> attach = new List<JExpense.attach>();
            var a = from xd in udc.SF_XACT_DOCs
                    let d = xd.SF_DOC
                    where xd.iXactId == iuXactId
                    select new { xd.SF_ATTACHMENTCATEG.sAttachmentCateg, d.DDateOfDoc, d.sName };
            foreach (var e in a)
            {
                JExpense.attach y = new JExpense.attach
                {
                    sAttachCateg = e.sAttachmentCateg,
                    DAssoc = e.DDateOfDoc,
                    sFile = e.sName,
                    sThumb = ""
                };
                attach.Add(y);
            }
            Xse.aAttachFiles = attach.ToArray();

            return JsonConvert.SerializeObject(Xse);
        }

        private void XValidate(JExpense uXse)
        {
            // Some validations have already occurred to this point:
            //    No duplicate attached files
            //    No empty expenditure or payment lists
            //    Expenditure description must not be empty
            //    Amounts of expenditure and payments must be numbers and must not be empty, although they are allowed to be zero.
            //    Dates must be well formed
            //    Don't try to add a file to be uploaded when none has been specified
            // Sum of credits must equal sum of debits
            decimal mSumDebits = 0.0m;
            decimal mSumCredits = 0.0m;
            foreach (JExpense.expend Xexpend in uXse.aExpenditures)
            {
                mSumDebits += Xexpend.mAmount;
            }
            foreach (JExpense.payment Xpayment in uXse.aPayments)
            {
                mSumCredits += Xpayment.mAmount;
            }
            if (mSumDebits != mSumCredits)
            {
                throw new excXactEng("Sum of Credits = " + mSumCredits.ToString() + " does not equal " + mSumDebits.ToString() + " = Sum of Debits");
            }

            // Account names must exist, must be unique, and accounts must be of the appropriate type. And, payment methods must exist:
            sf_AccountingDataContext dc = new sf_AccountingDataContext();
            foreach (JExpense.expend Xexpend in uXse.aExpenditures)
            {
                string sAcc = Xexpend.sAccountName;
                AcctCheck(Global.enLL.Expenditure, dc, sAcc, "");
            }
            foreach (JExpense.payment Xpay in uXse.aPayments)
            {
                string sAcc = Xpay.sAccountName;
                AcctCheck(Global.enLL.Payment, dc, sAcc, Xpay.sPmtMethod);
            }

            // Attached Files:
            //    At this point, the files to be attached have been uploaded to directory ~/Accounting/UploadedFiles.
            //    We check that they are indeed there
            FileAttCheck(uXse.aAttachFiles);
        }

        private void AcctCheck(Global.enLL euLL, sf_AccountingDataContext udc, string suAcc, string suPayMeth)
        {
            // Split account name into code portion and name portion:
            //    Assumes that there are no spaces in the field sCode of table SF_ACCOUNTS
            char[] cas = { ' ' };
            string[] saAcc = suAcc.Split(cas, 2); //subscript 0: Code, subscript 1: Name
            int iCount = (from a in udc.SF_ACCOUNTs where saAcc[0] == a.sCode select a).Count();
            string sText = "The code (leading) portion of account named `" + suAcc + "` ";
            if (iCount < 1)
            {
                throw new excXactEng(sText + "does not exist in the Chart of Accounts");
            }
            if (iCount > 1)
            {
                throw new excXactEng(sText + "occurs more than once in the Chart of Accounts");
            }
            iCount = (from a in udc.SF_ACCOUNTs where (saAcc[1] == a.sName) && (saAcc[0] == a.sCode) select a).Count();
            if (iCount < 1)
            {
                throw new excXactEng("Account `" + suAcc + "` not found in Chart of Accounts");
            }
            // Is the account type appropriate?
            string sAccTyp = "";
            switch (euLL)
            {
                case Global.enLL.Expenditure:
                    sAccTyp = "Expense"; // Debit account type must be "Expense" and must exist in table SF_ACCTTYPES
                    break;
                case Global.enLL.Payment:
                    iCount=(from a in udc.SF_PAYMENTMETHODs where a.sPaymentMethod == suPayMeth select a).Count();
                    if (iCount != 1)
                    {
                        throw new excXactEng("Payment method `" + suPayMeth + "` not found in table SF_PAYMENTMETHODS");
                    }
                    sAccTyp = "Assets"; // Credit account type must by "Assets" and must exist in table SF_ACCTTYPES
                    break;
            }
            iCount = (from a in udc.SF_ACCOUNTs join t in udc.SF_ACCTTYPEs on a.iSF_Type equals t.ID where a.sCode == saAcc[0] && t.sAccountType == sAccTyp select t).Count();
            if (iCount != 1)
            {
                throw new excXactEng(euLL.ToString() + " Account `" + suAcc + "` is not of type " + sAccTyp);
            }
        }

        private void FileAttCheck(JExpense.attach[] auAttach)
        {
            string savePath = HttpContext.Current.Server.MapPath(Global.scgRelPathUpload);
            foreach (JExpense.attach att in auAttach)
            {
                FileInfo f = new FileInfo(savePath + "\\" + att.sFile);
                if (!f.Exists)
                {
                    // Or maybe it already exists in SF_DOCS?
                    using (sf_AccountingDataContext dc = new sf_AccountingDataContext())
                    {
                        SF_DOC _DOC = new SF_DOC();
                        char[] cau = new char[] { '_' };
                        string[] safPN = att.sFile.Split(cau, 2); // File name prefix and main name
                        var qF = from d in dc.SF_DOCs where d.sPrefix == safPN[0] && d.sName == safPN[1] select d;
                        if (1 > qF.Count())
                        {
                            string ss = att.sFile.Substring(30);
                            for (int i = ss.Length - 31; i > 2; i -= 30)
                            {
                                ss = ss.Substring(0, i) + " " + ss.Substring(i);
                            }
                            string sMsg = "Accounting.FinDetails.ExpVendAP.XactEng.cs.FileAttCheck: Internal Software error: Could not find on the server the file to be attached `" + 
                                att.sFile.Substring(0,30) + " " + ss + "`, nor does it exist already as a saved file.";
                            throw new excXactEng(sMsg);
                        }
                    }
                }
            }
        }

        private void XSave(JExpense uXse, int iuOldXactId)
        {
            string sCodeLocation = "Start";
            //ActivityLog.oLog(ActivityLog.enumLogTypes.Debug, 16, "XactEng.cs.XSave sCodeLocation=" + sCodeLocation);
            SCUD_Multi mCRUD = new SCUD_Multi();
            sCodeLocation = "calling  mCRUD.GetPeopleIDfromWebSiteUserName";
            //ActivityLog.oLog(ActivityLog.enumLogTypes.Debug, 16, "XactEng.cs.XSave sCodeLocation=" + sCodeLocation);
            int iUser = mCRUD.GetPeopleIDfromWebSiteUserName(HttpContext.Current.User.Identity.Name);
            sCodeLocation = "calling new sf_AccountingDataContext";
            //ActivityLog.oLog(ActivityLog.enumLogTypes.Debug, 16, "XactEng.cs.XSave sCodeLocation=" + sCodeLocation);
            using (sf_AccountingDataContext dc = new sf_AccountingDataContext()) {
                sCodeLocation = "Next is 'Try' before 'new TransactionScope()'";
                //ActivityLog.oLog(ActivityLog.enumLogTypes.Debug, 16, "XactEng.cs.XSave sCodeLocation=" + sCodeLocation);
                try
                {
                    // Start with defining the database transaction within which all storing needs to take place,
                    //    and which has to succeed as a whole
                    sCodeLocation = "Calling 'new TransactionScope()'";
                    //ActivityLog.oLog(ActivityLog.enumLogTypes.Debug, 16, "XactEng.cs.XSave sCodeLocation=" + sCodeLocation);
                    using (TransactionScope Tscope = new TransactionScope())
                    {
                        DateTimeOffset DNow = DateTimeOffset.Now;

                        // Store items to table SF_XACTS, basic transaction info:
                        sCodeLocation = "Prepare to store to SF_XACTS";
                        //ActivityLog.oLog(ActivityLog.enumLogTypes.Debug, 18, "XactEng.cs.XSave sCodeLocation=" + sCodeLocation);
                        SF_XACT _XACT = new SF_XACT();
                        _XACT.D = uXse.Dxact;
                        _XACT.iType = (from t in dc.SF_XACTTYPEs where t.sTransactionType == "Expense" select t.ID).First();
                        _XACT.sMemo = uXse.sMemo;
                        _XACT.cStatus = 'A';
                        _XACT.iVendor = (from v in dc.SF_VENDORs where v.sVendorName == uXse.sVendor select v.ID).First();
                        _XACT.iMember = 0;
                        _XACT.iReplaces = iuOldXactId; // zero when nothing is being replaced
                        dc.SF_XACTs.InsertOnSubmit(_XACT);
                        sCodeLocation = "Storing to SF_XACTS";
                        //ActivityLog.oLog(ActivityLog.enumLogTypes.Debug, 18, "XactEng.cs.XSave sCodeLocation=" + sCodeLocation + ", _Xact.ID=" + _XACT.ID.ToString());
                        dc.SubmitChanges();
                        //  is now known
                        uXse.iSF_XACT_ID = _XACT.ID;
                        // Store items to table SF_ENTRIES, the debits and credits:
                        sCodeLocation = "Prepare to store to SF_ENTRIES";
                        //ActivityLog.oLog(ActivityLog.enumLogTypes.Debug, 18, "XactEng.cs.XSave sCodeLocation=" + sCodeLocation);
                        //    Expenditures
                        string sCodePortion;
                        char[] cab = new char[] { ' ' };
                        foreach (JExpense.expend x in uXse.aExpenditures)
                        {
                            SF_ENTRy _ENTRy = new SF_ENTRy();
                            _ENTRy.iXactId = _XACT.ID;
                            sCodePortion = x.sAccountName.Split(cab, 2)[0];
                            _ENTRy.iAccountId = (from a in dc.SF_ACCOUNTs where a.sCode == sCodePortion select a.ID).First();
                            _ENTRy.mAmount = x.mAmount;
                            _ENTRy.sDescription = x.sDescr;
                            _ENTRy.iPaymentMethod = (from m in dc.SF_PAYMENTMETHODs where m.sPaymentMethod == "(none)" select m.ID).First();
                            dc.SF_ENTRies.InsertOnSubmit(_ENTRy);
                        }
                        //    Payments
                        foreach (JExpense.payment p in uXse.aPayments)
                        {
                            SF_ENTRy _ENTRy = new SF_ENTRy();
                            _ENTRy.iXactId = _XACT.ID;
                            sCodePortion = p.sAccountName.Split(cab, 2)[0];
                            _ENTRy.iAccountId = (from a in dc.SF_ACCOUNTs where a.sCode == sCodePortion select a.ID).First();
                            _ENTRy.mAmount = p.mAmount;
                            _ENTRy.sDescription = p.sReference;
                            _ENTRy.iPaymentMethod = (from m in dc.SF_PAYMENTMETHODs where m.sPaymentMethod == p.sPmtMethod select m.ID).First();
                            dc.SF_ENTRies.InsertOnSubmit(_ENTRy);
                        }
                        sCodeLocation = "Storing to SF_ENTRIES";
                        //ActivityLog.oLog(ActivityLog.enumLogTypes.Debug, 18, "XactEng.cs.XSave sCodeLocation=" + sCodeLocation);
                        dc.SubmitChanges();

                        //    The files to be attached to this financial transaction (Xact)
                        string savePath = HttpContext.Current.Server.MapPath(Global.scgRelPathUpload);
                        foreach (JExpense.attach a in uXse.aAttachFiles)
                        {
                            // Store file to SF_DOCS with its metadata
                            sCodeLocation = "Prepare to store to SF_DOCS";
                            //ActivityLog.oLog(ActivityLog.enumLogTypes.Debug, 18, "XactEng.cs.XSave sCodeLocation=" + sCodeLocation);
                            SF_DOC _DOC = new SF_DOC();
                            string sFType = Path.GetExtension(a.sFile).ToLower();
                            _DOC.iFileType = (from t in dc.SF_ALLOWEDATTACHTYPEs where t.sAllowedFileType == sFType select t.ID).First();
                            char[] cau = new char[] { '_' };
                            string[] safPN = a.sFile.Split(cau, 2); // File name prefix and main name
                            // Does this file exist already in SF_DOCS?
                            var qF = from d in dc.SF_DOCs where d.sPrefix == safPN[0] && d.sName == safPN[1] select d;
                            if (1 > qF.Count())
                            {
                                // File does not yet exist
                                _DOC.sPrefix = safPN[0];
                                _DOC.sName = safPN[1];
                                _DOC.DDateOfDoc = a.DAssoc;
                                _DOC.Dinserted = DNow;
                                //    Represent the file contents as a blob
                                string fullPathName = savePath + "\\" + a.sFile;
                                byte[] file_byte = File.ReadAllBytes(fullPathName);
                                Binary file_binary = new Binary(file_byte);
                                _DOC.blobDoc = file_binary;
                                dc.SF_DOCs.InsertOnSubmit(_DOC);
                                sCodeLocation = "Storing to SF_DOCS";
                                //ActivityLog.oLog(ActivityLog.enumLogTypes.Debug, 18, "XactEng.cs.XSave sCodeLocation=" + sCodeLocation);
                                dc.SubmitChanges();
                                // _DOC.ID is now known

                                // Write file to ZIP archive
                                sCodeLocation = "Writing to ZIP archive";
                                //ActivityLog.oLog(ActivityLog.enumLogTypes.Debug, 18, "XactEng.cs.XSave sCodeLocation=" + sCodeLocation);
                                using (FileStream zipToOpen = new FileStream(savePath + "\\" + "UploadedFilesArchive.zip", FileMode.Open))
                                {
                                    using (ZipArchive archive = new ZipArchive(zipToOpen, ZipArchiveMode.Update))
                                    {
                                        ZipArchiveEntry z = archive.CreateEntry(a.sFile, CompressionLevel.Optimal);
                                        using (BinaryWriter bw = new BinaryWriter(z.Open()))
                                        {
                                            bw.Write(file_byte);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                // File already exists
                                _DOC = qF.First();
                            }

                            // Make the connection between file in SF_DOCS and transaction in SF_XACTS via table SF_XACT_DOCS
                            // Note that it is possible to have more than one Xact in SF_XACT connected to one file in SF_DOCS; this happens
                            //   when a Xact is edited that replaces an older Xact. The older Xact is not deleted, just replaced,
                            //   and it keeps its connection to the attached file; the replacing Xact possibly (not necessarily) 
                            //   connects to the same file.
                            sCodeLocation = "Prepare to store to SF_XACT_DOCS";
                            //ActivityLog.oLog(ActivityLog.enumLogTypes.Debug, 18, "XactEng.cs.XSave sCodeLocation=" + sCodeLocation);
                            SF_XACT_DOC _XACT_DOC = new SF_XACT_DOC();
                            _XACT_DOC.iDocsId = _DOC.ID;
                            _XACT_DOC.iAttachmentCateg = (from c in dc.SF_ATTACHMENTCATEGs where a.sAttachCateg == c.sAttachmentCateg select c.ID).First();
                            _XACT_DOC.iXactId = _XACT.ID;
                            dc.SF_XACT_DOCs.InsertOnSubmit(_XACT_DOC);
                            sCodeLocation = "Storing to SF_XACT_DOCS";
                            //ActivityLog.oLog(ActivityLog.enumLogTypes.Debug, 18, "XactEng.cs.XSave sCodeLocation=" + sCodeLocation);
                            dc.SubmitChanges();
                        }

                        // If a Xact is being replaced, mark it as such:
                        if (iuOldXactId > 0)
                        {
                            SF_XACT _XACT_old = (from x in dc.SF_XACTs where x.ID == iuOldXactId select x).First();
                            _XACT_old.iReplacedBy = _XACT.ID;
                            _XACT_old.cStatus = 'R';
                            dc.SubmitChanges();
                        }

                        // Write to audit trail table
                        sCodeLocation = "Prepare to store to SF_AUDITTRAIL";
                        //ActivityLog.oLog(ActivityLog.enumLogTypes.Debug, 18, "XactEng.cs.XSave sCodeLocation=" + sCodeLocation);
                        SF_AUDITTRAIL _AUDITTRAIL = new SF_AUDITTRAIL();
                        _AUDITTRAIL.DTimeStamp = DNow;
                        _AUDITTRAIL.iRecordEnteredBy = iUser;
                        _AUDITTRAIL.cAction = 'N';
                        _AUDITTRAIL.Jold = null;
                        _AUDITTRAIL.Jnew = JsonConvert.SerializeObject(uXse, Formatting.Indented);
                        dc.SF_AUDITTRAILs.InsertOnSubmit(_AUDITTRAIL);
                        sCodeLocation = "Storing to SF_AUDITTRAIL";
                        //ActivityLog.oLog(ActivityLog.enumLogTypes.Debug, 18, "XactEng.cs.XSave sCodeLocation=" + sCodeLocation);
                        dc.SubmitChanges();

                        sCodeLocation = "Calling Transactionscope.Complete()";
                        //ActivityLog.oLog(ActivityLog.enumLogTypes.Debug, 18, "XactEng.cs.XSave sCodeLocation=" + sCodeLocation);
                        Tscope.Complete();

                        // Delete the uploaded files (they have been saved to database table SF_DOCS and to a zip archive)
                        sCodeLocation = "Deleting uploaded files";
                        //ActivityLog.oLog(ActivityLog.enumLogTypes.Debug, 18, "XactEng.cs.XSave sCodeLocation=" + sCodeLocation);
                        foreach (JExpense.attach a in uXse.aAttachFiles)
                        {
                            File.Delete(savePath + "\\" + a.sFile);
                        }
                    } // end of TransactionScope Tscope
                    sCodeLocation = "After end of Transactionscope Tscope";
                    //ActivityLog.oLog(ActivityLog.enumLogTypes.Debug, 16, "XactEng.cs.XSave sCodeLocation=" + sCodeLocation);
                }
                catch (TransactionAbortedException TrAbExc)
                {
                    throw new excXactEng("In XactEng.cs at " + sCodeLocation + ": TransactionAbortedException Message: " + TrAbExc.Message);
                }
                catch (Exception exc)
                {
                    throw new excXactEng("In XactEng.cs at " + sCodeLocation + ": General exception Message: " + exc.Message);
                }
            }
        }
    }

    public static class XactFilter
    {
        private const string scErr = "Internal error in XactEng.cs.dtDefaultXactFilter: Inconsistent row count at ";
        public enum eStatus { Active, Voided, Deleted, Template};

        public static DataTable dtDefaultXactFilter()
        {
            DataTable dt = new DataTable("XactFilterSettings");

            #region columns
            // Each column repesents a filter property
            #region Filter Name
            DataColumn column = new DataColumn
            {
                DataType = System.Type.GetType("System.String"),
                ColumnName = "sFilterName",
                Unique = true
            };
            dt.Columns.Add(column);
            #endregion
            #region Filter Type
            column = new DataColumn
            {
                DataType = System.Type.GetType("System.String"),
                ColumnName = "sFilterType",
                Unique = false // Filter Types are: List, Boolean, DateList, Range
            };
            dt.Columns.Add(column);
            #endregion
            #region Is the filter Enabled?
            column = new DataColumn
            {
                DataType = System.Type.GetType("System.Boolean"),
                ColumnName = "bEnabled" // is this filter to be used at all?
            };
            dt.Columns.Add(column);
            #endregion
            #region IN the list or NOT IN the list?
            column = new DataColumn
            {
                DataType = System.Type.GetType("System.Boolean"),
                ColumnName = "bINorEX" // Look for items that occur in the list (bINorEX is true), or rather look for items that do not match any in the list (bINorEX is false)
            };
            dt.Columns.Add(column);
            #endregion
            #region List of comma separated items
            column = new DataColumn
            {
                DataType = System.Type.GetType("System.String"),
                ColumnName = "sList"
            };
            dt.Columns.Add(column);
            #endregion
            #region Lower range limit
            column = new DataColumn
            {
                DataType = System.Type.GetType("System.Decimal"),
                ColumnName = "dLow"
            };
            dt.Columns.Add(column);
            #endregion
            #region High range limit
            column = new DataColumn
            {
                DataType = System.Type.GetType("System.Decimal"),
                ColumnName = "dHigh"
            };
            dt.Columns.Add(column);
            #endregion
            #region Punctuation Mark
            column = new DataColumn
            {
                DataType = System.Type.GetType("System.String"),
                ColumnName = "sPunctuationMark"
            };
            dt.Columns.Add(column);
            #endregion
            #region Associated database field
            column = new DataColumn
            {
                DataType = System.Type.GetType("System.String"),
                ColumnName = "sField"
                //===============
                //Important Note:  ##### Is this needed for accounting ?? #####
                //===============
                // In each row of this table, the contents of this field needs to be coordinated carefully with the contents of the constant scSQL_FROM:
                // The tables need to be referenced by their alias as defined in scSQL_FROM, for example:
                //  Table OPERATIONS is referred to as OPERATIONS_1 not just OPERATIONS; and
                //  Table LOCATIONS is referred to as TOLOCS for takeoff locations, and as LDLOCS for landing locations.
            };
            dt.Columns.Add(column);
            #endregion
            #endregion
            #region rows
            // Create and add the table rows. Each row represents a particular filter, except the first row is a version number:
            #region FilterVersion
            DataRow row = dt.NewRow();
            row[(int)Global.egAdvFilterProps.FilterName] = "FilterVersion";
            row[(int)Global.egAdvFilterProps.FilterType] = "Range";
            row[(int)Global.egAdvFilterProps.Enabled] = true; // is set to true to be included in filter list in Expenses.aspx.cs.Page_Load
            row[(int)Global.egAdvFilterProps.INorEX] = false;
            row[(int)Global.egAdvFilterProps.List] = "";
            row[(int)Global.egAdvFilterProps.LowLimit] = 1.1M; // Version number of the filter settings DataTable; compare with Global.dgcVersionOfFilterSettingsDataTable
            row[(int)Global.egAdvFilterProps.HighLimit] = 0M;
            row[(int)Global.egAdvFilterProps.PunctuationMark] = "";
            row[(int)Global.egAdvFilterProps.Field] = "";
            dt.Rows.Add(row);
            if ((dt.Rows.Count - 1) != (int)Global.egXactFilters.Version)
            {
                throw new Global.excToPopup(scErr + row[(int)Global.egAdvFilterProps.FilterName]);
            }
            #endregion
            #region EnableFilteringOverall
            row = dt.NewRow();
            row[(int)Global.egAdvFilterProps.FilterName] = "EnableFilteringOverall";
            row[(int)Global.egAdvFilterProps.FilterType] = "Boolean";
            row[(int)Global.egAdvFilterProps.Enabled] = true;
            row[(int)Global.egAdvFilterProps.INorEX] = false;
            row[(int)Global.egAdvFilterProps.List] = "";
            row[(int)Global.egAdvFilterProps.LowLimit] = 0;
            row[(int)Global.egAdvFilterProps.HighLimit] = 0;
            row[(int)Global.egAdvFilterProps.PunctuationMark] = "";
            row[(int)Global.egAdvFilterProps.Field] = "";
            dt.Rows.Add(row);
            if ((dt.Rows.Count - 1) != (int)Global.egXactFilters.EnableFilteringOverall)
            {
                throw new Global.excToPopup(scErr + row[(int)Global.egAdvFilterProps.FilterName]);
            }
            #endregion
            #region Transaction Type
            row = dt.NewRow();
            row[(int)Global.egAdvFilterProps.FilterName] = "TransactionType"; // Ignored for list of expenses since transaction type is 'Expense'
            row[(int)Global.egAdvFilterProps.FilterType] = "List";
            row[(int)Global.egAdvFilterProps.Enabled] = false;
            row[(int)Global.egAdvFilterProps.INorEX] = true;
            row[(int)Global.egAdvFilterProps.List] = "All";
            row[(int)Global.egAdvFilterProps.LowLimit] = 0.0;
            row[(int)Global.egAdvFilterProps.HighLimit] = 0.0;
            row[(int)Global.egAdvFilterProps.PunctuationMark] = "'";
            row[(int)Global.egAdvFilterProps.Field] = "";
            dt.Rows.Add(row);
            if ((dt.Rows.Count - 1) != (int)Global.egXactFilters.TransactionType)
            {
                throw new Global.excToPopup(scErr + row[(int)Global.egAdvFilterProps.FilterName]);
            }
            #endregion
            #region Vendor
            row = dt.NewRow();
            row[(int)Global.egAdvFilterProps.FilterName] = "Vendor";
            row[(int)Global.egAdvFilterProps.FilterType] = "List";
            row[(int)Global.egAdvFilterProps.Enabled] = false;
            row[(int)Global.egAdvFilterProps.INorEX] = true;
            row[(int)Global.egAdvFilterProps.List] = "All";
            row[(int)Global.egAdvFilterProps.LowLimit] = 0.0;
            row[(int)Global.egAdvFilterProps.HighLimit] = 0.0;
            row[(int)Global.egAdvFilterProps.PunctuationMark] = "'";
            row[(int)Global.egAdvFilterProps.Field] = "";
            dt.Rows.Add(row);
            if ((dt.Rows.Count - 1) != (int)Global.egXactFilters.Vendor)
            {
                throw new Global.excToPopup(scErr + row[(int)Global.egAdvFilterProps.FilterName]);
            }
            #endregion
            #region Transaction Status
            row = dt.NewRow();
            row[(int)Global.egAdvFilterProps.FilterName] = "TransactionStatus";
            row[(int)Global.egAdvFilterProps.FilterType] = "List";
            row[(int)Global.egAdvFilterProps.Enabled] = true;
            row[(int)Global.egAdvFilterProps.INorEX] = true;
            row[(int)Global.egAdvFilterProps.List] = "'A','V'";
            row[(int)Global.egAdvFilterProps.LowLimit] = 0.0;
            row[(int)Global.egAdvFilterProps.HighLimit] = 0.0;
            row[(int)Global.egAdvFilterProps.PunctuationMark] = "'";
            row[(int)Global.egAdvFilterProps.Field] = "";
            dt.Rows.Add(row);
            if ((dt.Rows.Count - 1) != (int)Global.egXactFilters.TransactionStatus)
            {
                throw new Global.excToPopup(scErr + row[(int)Global.egAdvFilterProps.FilterName]);
            }
            #endregion
            #region File Attachment Category
            row = dt.NewRow();
            row[(int)Global.egAdvFilterProps.FilterName] = "AttachmentCateg";
            row[(int)Global.egAdvFilterProps.FilterType] = "List";
            row[(int)Global.egAdvFilterProps.Enabled] = false;
            row[(int)Global.egAdvFilterProps.INorEX] = true;
            row[(int)Global.egAdvFilterProps.List] = "All";
            row[(int)Global.egAdvFilterProps.LowLimit] = 0.0;
            row[(int)Global.egAdvFilterProps.HighLimit] = 0.0;
            row[(int)Global.egAdvFilterProps.PunctuationMark] = "'";
            row[(int)Global.egAdvFilterProps.Field] = "";
            dt.Rows.Add(row);
            if ((dt.Rows.Count - 1) != (int)Global.egXactFilters.AttachmentCateg)
            {
                throw new Global.excToPopup(scErr + row[(int)Global.egAdvFilterProps.FilterName]);
            }
            #endregion
            #region File Attachment Type
            row = dt.NewRow();
            row[(int)Global.egAdvFilterProps.FilterName] = "AttachmentType";
            row[(int)Global.egAdvFilterProps.FilterType] = "List";
            row[(int)Global.egAdvFilterProps.Enabled] = false;
            row[(int)Global.egAdvFilterProps.INorEX] = true;
            row[(int)Global.egAdvFilterProps.List] = "All";
            row[(int)Global.egAdvFilterProps.LowLimit] = 0.0;
            row[(int)Global.egAdvFilterProps.HighLimit] = 0.0;
            row[(int)Global.egAdvFilterProps.PunctuationMark] = "'";
            row[(int)Global.egAdvFilterProps.Field] = "";
            dt.Rows.Add(row);
            if ((dt.Rows.Count - 1) != (int)Global.egXactFilters.AttachmentType)
            {
                throw new Global.excToPopup(scErr + row[(int)Global.egAdvFilterProps.FilterName]);
            }
            #endregion
            #region Payment Method
            row = dt.NewRow();
            row[(int)Global.egAdvFilterProps.FilterName] = "PaymentMethod";
            row[(int)Global.egAdvFilterProps.FilterType] = "List";
            row[(int)Global.egAdvFilterProps.Enabled] = false;
            row[(int)Global.egAdvFilterProps.INorEX] = true;
            row[(int)Global.egAdvFilterProps.List] = "All";
            row[(int)Global.egAdvFilterProps.LowLimit] = 0.0;
            row[(int)Global.egAdvFilterProps.HighLimit] = 0.0;
            row[(int)Global.egAdvFilterProps.PunctuationMark] = "'";
            row[(int)Global.egAdvFilterProps.Field] = "";
            dt.Rows.Add(row);
            if ((dt.Rows.Count - 1) != (int)Global.egXactFilters.PaymentMethod)
            {
                throw new Global.excToPopup(scErr + row[(int)Global.egAdvFilterProps.FilterName]);
            }
            #endregion
            #region Expense Account (in any of the transaction's entries)
            row = dt.NewRow();
            row[(int)Global.egAdvFilterProps.FilterName] = "ExpenseAccount";
            row[(int)Global.egAdvFilterProps.FilterType] = "List";
            row[(int)Global.egAdvFilterProps.Enabled] = false;
            row[(int)Global.egAdvFilterProps.INorEX] = true;
            row[(int)Global.egAdvFilterProps.List] = "All";
            row[(int)Global.egAdvFilterProps.LowLimit] = 0.0;
            row[(int)Global.egAdvFilterProps.HighLimit] = 0.0;
            row[(int)Global.egAdvFilterProps.PunctuationMark] = "'";
            row[(int)Global.egAdvFilterProps.Field] = "";
            dt.Rows.Add(row);
            if ((dt.Rows.Count - 1) != (int)Global.egXactFilters.ExpenseAccount)
            {
                throw new Global.excToPopup(scErr + row[(int)Global.egAdvFilterProps.FilterName]);
            }
            #endregion
            #region Payment Account (in any of the transaction's entries)
            row = dt.NewRow();
            row[(int)Global.egAdvFilterProps.FilterName] = "PaymentAccount";
            row[(int)Global.egAdvFilterProps.FilterType] = "List";
            row[(int)Global.egAdvFilterProps.Enabled] = false;
            row[(int)Global.egAdvFilterProps.INorEX] = true;
            row[(int)Global.egAdvFilterProps.List] = "";
            row[(int)Global.egAdvFilterProps.LowLimit] = 0.0;
            row[(int)Global.egAdvFilterProps.HighLimit] = 0.0;
            row[(int)Global.egAdvFilterProps.PunctuationMark] = "'";
            row[(int)Global.egAdvFilterProps.Field] = "";
            dt.Rows.Add(row);
            if ((dt.Rows.Count - 1) != (int)Global.egXactFilters.PaymentAccount)
            {
                throw new Global.excToPopup(scErr + row[(int)Global.egAdvFilterProps.FilterName]);
            }
            #endregion
            #region Transaction Date
            row = dt.NewRow();
            row[(int)Global.egAdvFilterProps.FilterName] = "XactDate";
            row[(int)Global.egAdvFilterProps.FilterType] = "DateList";
            row[(int)Global.egAdvFilterProps.Enabled] = false;
            row[(int)Global.egAdvFilterProps.INorEX] = true;
            // In the following line, there must be exactly one space between the date, time, and offset portions.
            //    The format of these times is rigid: YYYY-MM-DD HH:mm shh:mm, where s is + or -, hh between 00 and 14, and mm between 00 and 59.
            //    For the offset portion in the Pacific Standard Time zone use -08:00; 'offset' means time interval between UTC (Universal Time Coordinated) and local time.
            //    The comma separates the lower default limit from the upper default limit. Spaces surrounding the comma are optional.
            row[(int)Global.egAdvFilterProps.List] = "2000-01-01 00:00 -08:00, 2099-12-31 23:59 -08:00";
            row[(int)Global.egAdvFilterProps.LowLimit] = 0.0M;
            row[(int)Global.egAdvFilterProps.HighLimit] = 0.0M;
            row[(int)Global.egAdvFilterProps.PunctuationMark] = "'";
            row[(int)Global.egAdvFilterProps.Field] = "";
            dt.Rows.Add(row);
            if ((dt.Rows.Count - 1) != (int)Global.egXactFilters.XactDate)
            {
                throw new Global.excToPopup(scErr + row[(int)Global.egAdvFilterProps.FilterName]);
            }
            #endregion
            #region Transaction Amount
            row = dt.NewRow();
            row[(int)Global.egAdvFilterProps.FilterName] = "XactAmount";
            row[(int)Global.egAdvFilterProps.FilterType] = "Range";
            row[(int)Global.egAdvFilterProps.Enabled] = false;
            row[(int)Global.egAdvFilterProps.INorEX] = true;
            row[(int)Global.egAdvFilterProps.List] = "";
            row[(int)Global.egAdvFilterProps.LowLimit] = 0M;
            row[(int)Global.egAdvFilterProps.HighLimit] = 999999999.99M;
            row[(int)Global.egAdvFilterProps.PunctuationMark] = "";
            row[(int)Global.egAdvFilterProps.Field] = "";
            dt.Rows.Add(row);
            if ((dt.Rows.Count - 1) != (int)Global.egXactFilters.XactAmount)
            {
                throw new Global.excToPopup(scErr + row[(int)Global.egAdvFilterProps.FilterName]);
            }
            #endregion
            #region Number of Attached Files
            row = dt.NewRow();
            row[(int)Global.egAdvFilterProps.FilterName] = "NumAttFiles";
            row[(int)Global.egAdvFilterProps.FilterType] = "Range";
            row[(int)Global.egAdvFilterProps.Enabled] = false;
            row[(int)Global.egAdvFilterProps.INorEX] = true;
            row[(int)Global.egAdvFilterProps.List] = "";
            row[(int)Global.egAdvFilterProps.LowLimit] = 0M;
            row[(int)Global.egAdvFilterProps.HighLimit] = 100M;
            row[(int)Global.egAdvFilterProps.PunctuationMark] = "";
            row[(int)Global.egAdvFilterProps.Field] = "";
            dt.Rows.Add(row);
            if ((dt.Rows.Count - 1) != (int)Global.egXactFilters.NumAttFiles)
            {
                throw new Global.excToPopup(scErr + row[(int)Global.egAdvFilterProps.FilterName]);
            }
            #endregion
            #endregion
            return dt;
        }
    }

    public static class XactSort
    {
        public enum eSortBy { Date, sVendorName, cStatus, XT};

        public static DataTable DefaultXactSort()
        {
            DataTable dt = new DataTable("XactSortSettings");

            //Columns

            DataColumn[] dca = new DataColumn[3];
            dca[0] = new DataColumn("OrderBy", Type.GetType("System.String"));
            dca[0].MaxLength = 25;
            dca[0].Unique = true;
            dca[1] = new DataColumn("SortPriority", Type.GetType("System.Int32"));
            dca[1].Unique = false;
            dca[2] = new DataColumn("SortOrder", Type.GetType("System.String"));
            dca[2].MaxLength = 4;
            dca[2].Unique = false;
            foreach (DataColumn col in dca)
            {
                col.AllowDBNull = false;
                dt.Columns.Add(col);
            }

            //Rows

            // Date
            DataRow dtRow = dt.NewRow();
            dtRow["OrderBy"] = eSortBy.Date.ToString();
            dtRow["SortPriority"] = 1;
            dtRow["SortOrder"] = "desc";
            dt.Rows.Add(dtRow);
            // Vendor
            dtRow = dt.NewRow();
            dtRow["OrderBy"] = eSortBy.sVendorName.ToString();
            dtRow["SortPriority"] = 2;
            dtRow["SortOrder"] = "asc";
            dt.Rows.Add(dtRow);
            // Status
            dtRow = dt.NewRow();
            dtRow["OrderBy"] = eSortBy.cStatus.ToString();
            dtRow["SortPriority"] = 3;
            dtRow["SortOrder"] = "asc";
            // Amount
            dt.Rows.Add(dtRow);
            dtRow = dt.NewRow();
            dtRow["OrderBy"] = eSortBy.XT.ToString();
            dtRow["SortPriority"] = 4;
            dtRow["SortOrder"] = "desc";
            dt.Rows.Add(dtRow);

            return dt;
        }
    }
}