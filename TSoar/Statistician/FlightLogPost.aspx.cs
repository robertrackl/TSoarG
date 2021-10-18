using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TSoar.Statistician
{
    public partial class FlightLogPost : System.Web.UI.Page
    {
        FlightLogPosting flpo = new FlightLogPosting();
        StatistDailyFlightLogDataContext stdc = new StatistDailyFlightLogDataContext();

        #region ViewState
        private int iFlightLog { get { return iGet("iFlightLog"); } set { ViewState["iFlightLog"] = value; } }
        private int iOpsCount { get { return iGet("iOpsCount"); } set { ViewState["iOpsCount"] = value; } }
        private int NOps { get { return iGet("NOps"); } set { ViewState["NOps"] = value; } }
        private int iGet(string su)
        {
            if (ViewState[su] is null)
            {
                return 0;
            }
            else
            {
                return (int)ViewState[su];
            }
        }
        private string sPStatus { get { return (string)ViewState["sPStatus"] ?? "}"; } set { ViewState["sPStatus"] = value; } }
        private List<int> iaqflr { get { return GetListInt("ListInt"); } set { ViewState["ListInt"] = value; } }
        private List<int> GetListInt(string sLu)
        {
            if (ViewState[sLu] == null)
            {
                return new List<int>(); // return an empty list
            }
            else
            {
                return (List<int>)ViewState[sLu];
            }
        }
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                iFlightLog = (int)Session["iFlightLog"];
                // Post all the flight log rows in one Daily Flight Log
                sPStatus = "OK"; // optimistic
                                 // iFlightLog points to the ID column in a row of table DAILYFLIGHTLOGS
                iaqflr = (from r in stdc.FLIGHTLOGROWs where r.iFliteLog == iFlightLog && r.cStatus != 'P' select r.ID).ToList();
                iOpsCount = 0;
                NOps = iaqflr.Count;
                if (NOps > 0)
                {
                    UPTimer1.Enabled = true;
                    UPModalPopExt.Show();
                }
                else
                {
                    DateTimeOffset Dfl = (from l in stdc.DAILYFLIGHTLOGs where l.ID == iFlightLog select l.DFlightOps).First();
                    string sDfl = CustFmt.sFmtDate(Dfl, CustFmt.enDFmt.DateOnly);
                    lblCounter.Text = "There are no flights to be posted in flight log for " + sDfl + " with internal Id " + iFlightLog.ToString();
                    lblAll.Text = "";
                    pbOK.Visible = true;
                    UPModalPopExt.Show();
                }
            }
        }

        protected void UPTimer1_Tick(object sender, EventArgs e)
        {
            UPTimer1.Enabled = false;
            using (var transaction = new TransactionScope())
            {
                try
                {
                    int iRow = iaqflr[iOpsCount];
                    string sStatus = flpo.sPost2OPERATIONS(stdc, iRow);
                    if (sStatus == "OK")
                    {
                        FLIGHTLOGROW flr = (from r in stdc.FLIGHTLOGROWs where r.ID == iRow select r).First();
                        flr.cStatus = 'P'; // mark as 'Processed'
                        stdc.SubmitChanges();
                        transaction.Complete();
                        iOpsCount++;
                    }
                    else
                    {
                        sPStatus = sStatus;
                    }
                    transaction.Dispose();
                }
                catch (Global.excToPopup exTP)
                {
                    sPStatus = "Problem in FlightLogPosting.sPost2OPERATIONS: '" + exTP.Message +
                        "' -- the processing of daily flight log with ID = " + iFlightLog.ToString() +
                        " has been interrupted after successfully processing " + iOpsCount.ToString() +
                        " flight operations. Please check status of each flight operation in this daily flight log.";
                    transaction.Dispose();
                }
            }

            lblCounter.Text = "Processed " + iOpsCount.ToString();
            lblAll.Text = " flights out of " + NOps.ToString();
            if (sPStatus.Substring(0,2) != "OK")
            {
                pbOK.Visible = true;
                lblCounter.Text = sPStatus + "</br></br>" + lblCounter.Text;
            }
            else
            {
                if (iOpsCount >= NOps)
                {
                    Response.Redirect("FlightLogInput.aspx");
                }
                UPTimer1.Enabled = true;
            }
        }

        protected void pbOK_Click(object sender, EventArgs e)
        {
            Response.Redirect("FlightLogInput.aspx");
        }
    }
}