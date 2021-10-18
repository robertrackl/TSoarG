using System;
using System.Configuration;
using System.Data;
using System.Data.Linq;
using System.Data.SqlClient;
using System.Web.UI.WebControls;
using TSoar.Statistician;
using TSoar.DB;

namespace TSoar.MemberPages.Stats
{
    public partial class ClubStats : System.Web.UI.Page
    {
        private const string scSTD = "STANDARD";
        private const string scADV = "ADVANCED";
        private DataTable dtStdFilters = new DataTable("OpsStdFilterSetting", "TSoar.MemberPages.Stats");
        private DataTable dtAdvFilters = new DataTable("OpsAdvFilterSetting", "TSoar.MemberPages.Stats");
        DataTable dtFilters = null;
        private PopulateTrVwFilters PopulTree = new PopulateTrVwFilters();
        SCUD_Multi mCRUD = new SCUD_Multi();
        SCUD_single lCRUD = new SCUD_single();
        private string sOpID = string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {
            //ActivityLog.oDiag("Debug", "ClubStats.Page_Load: Entry, IsPostBack=" + IsPostBack.ToString());

            #region Filter Selection and Average Tow Plane Flight Duration
            string sOpsFilterSel = AccountProfile.CurrentUser.OpsFilterSettingSelection ?? ""; // Can be STANDARD or ADVANCED
            if (sOpsFilterSel.Length < 1)
            {
                sOpsFilterSel = scSTD;
            }

            if (!IsPostBack)
            {
                switch (sOpsFilterSel)
                {
                    case scSTD:
                        PopulTree.bCheck4dtStdFilterReset(dtStdFilters, lblVersionUpdate);
                        break;
                    case scADV:
                        PopulTree.bCheck4dtAdvFilterReset(dtAdvFilters, lblVersionUpdate);
                        break;
                    default:
                        ProcessPopupException(new Global.excToPopup("Operations Filter Setting Selection is not " + scSTD + " or " + scADV + " but " + sOpsFilterSel));
                        break;
                }

                string sDur;
                using (SqlConnection SqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlConn"].ToString()))
                {
                    SqlConn.Open();
                    string sCmd = "SELECT iAvgUseDurationMinutes FROM EQUIPMENTROLES WHERE sEquipmentRole='Tow Plane'";
                    using (SqlCommand cmd = new SqlCommand(sCmd))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Connection = SqlConn;
                        sDur = ((int)cmd.ExecuteScalar()).ToString();
                    }
                }
                lblTowPlaneFltDur.Text = sDur;
            }

            switch (sOpsFilterSel)
            {
                case scSTD:
                    dtFilters = AccountProfile.CurrentUser.OpsStdFilterSetting;
                    break;
                case scADV:
                    dtFilters = AccountProfile.CurrentUser.OpsAdvFilterSetting;
                    break;
            }
            #endregion

            #region Assemble text for displaying standard or advanced filter properties
            if (!IsPostBack)
            {
                lbl_filter.Text = sFilterLabel(sOpsFilterSel, dtFilters);
            }
            #endregion

            #region Populate TreeView and GridView
            // This call defines in PopulTree the variable sSubQuery wich is then used in the call to GlAggregates below calling PopulTree.dtGlAggregates,
            //   and in the call PopulTree.dtGlOps inside of DisplayInGrid.
            PopulTree.BuildAdvSubQuery(dtFilters); 
            if (!IsPostBack)
            {
                // Apply the filter
                trv_Ops.Nodes.Clear();
                TreeNode tnRoot = new TreeNode("Operations List", "");
                tnRoot.PopulateOnDemand = true;
                trv_Ops.Nodes.Add(tnRoot);
                PopulTree.trv_OpsClosestInTime(trv_Ops); // Causes partial expansion of TreeView trv_Ops

                GlAggregates(dtFilters);
                DisplayInGrid();
            }
            #endregion

            // SCR 216 start
            //#region GridView headers tooltips

            //DataTable dt = lCRUD.GetAll(Global.enugSingleMFF.LaunchMethods);
            //string sTT = "Launch Methods Legend:";
            //foreach (DataRow row in dt.Rows)
            //{
            //    sTT += Environment.NewLine + (string)row.ItemArray[2] + "  " + (string)row.ItemArray[1];
            //}
            //Label lblHLM = (Label)gvOps.HeaderRow.FindControl("lblHLM");
            //lblHLM.ToolTip = sTT;

            //dt = lCRUD.GetAll(Global.enugSingleMFF.ChargeCodes);
            //sTT = "Charge Codes Legend:";
            //foreach (DataRow row in dt.Rows)
            //{
            //    sTT += Environment.NewLine + (string)row.ItemArray[2] + "  " + (string)row.ItemArray[1];
            //}
            //Label lblHCC = (Label)gvOps.HeaderRow.FindControl("lblHCC");
            //lblHCC.ToolTip = sTT;

            //#endregion
            // SCR 216 end
            //ActivityLog.oDiag("Debug", "ClubStats.Page_Load: Exit");
        }

        // SCR 216 start
        private void genToolTips()
        {
            DataTable dt = lCRUD.GetAll(Global.enugSingleMFF.LaunchMethods);
            string sTT = "Launch Methods Legend:";
            foreach (DataRow row in dt.Rows)
            {
                sTT += Environment.NewLine + (string)row.ItemArray[2] + "  " + (string)row.ItemArray[1];
            }
            Label lblHLM = (Label)gvOps.HeaderRow.FindControl("lblHLM");
            lblHLM.ToolTip = sTT;

            dt = lCRUD.GetAll(Global.enugSingleMFF.ChargeCodes);
            sTT = "Charge Codes Legend:";
            foreach (DataRow row in dt.Rows)
            {
                sTT += Environment.NewLine + (string)row.ItemArray[2] + "  " + (string)row.ItemArray[1];
            }
            Label lblHCC = (Label)gvOps.HeaderRow.FindControl("lblHCC");
            lblHCC.ToolTip = sTT;
        }
        // SCR 216 end
        private string sFilterLabel(string suOpsFilterSelection, DataTable dtu)
        {
            bool bInEffect = false;
            string sFilter = "";

            switch (suOpsFilterSelection)
            {
                case scSTD:
                    sFilter = "A ";
                    break;
                case scADV:
                    sFilter = "An ";
                    break;
            }
            if ((bool)dtu.Rows[(int)Global.egOpsFilters.EnableFilteringOverall][(int)Global.egAdvFilterProps.Enabled])
            {
                if ((bool)dtu.Rows[(int)Global.egOpsFilters.FirstFlight][(int)Global.egAdvFilterProps.Enabled])
                {
                    bInEffect = true;
                }
                else
                {
                    for (int ix = (int)Global.egOpsFilters.EnableFilteringOverall + 1; ix < dtu.Rows.Count - 1; ix++)
                    {
                        if ((bool)dtu.Rows[ix][(int)Global.egAdvFilterProps.Enabled])
                        {
                            bInEffect = true;
                            break;
                        }
                    }
                }
            }
            if (bInEffect)
            {
                // a filter is in use
                sFilter += suOpsFilterSelection + " flight operations filter is currently in effect: [ ";
                string sOther = "";
                foreach (DataRow row in dtu.Rows)
                {
                    switch (suOpsFilterSelection)
                    {
                        case scSTD:
                            sOther = "An " + scADV;
                            break;
                        case scADV:
                            sOther = "A " + scSTD;
                            break;
                    }
                    sOther += " flight operations filter is currently NOT in effect.";
                    if ((bool)row[(int)Global.egAdvFilterProps.Enabled])
                    {
                        string sName = (string)row[(int)Global.egAdvFilterProps.FilterName];
                        switch ((string)row[(int)Global.egAdvFilterProps.FilterType])
                        {
                            case ("Range"):
                                if ((string)row[(int)Global.egAdvFilterProps.FilterName] == "FilterVersion")
                                {
                                    sFilter += "Version " + ((decimal)row[(int)Global.egAdvFilterProps.LowLimit]).ToString();
                                }
                                else
                                {
                                    sFilter += "; " + sName + " between " + ((decimal)row[(int)Global.egAdvFilterProps.LowLimit]).ToString() +
                                        " and " + ((decimal)row[(int)Global.egAdvFilterProps.HighLimit]).ToString();
                                }
                                break;
                            case ("List"):
                                sFilter += "; " + sName + ((bool)row[(int)Global.egAdvFilterProps.INorEX] ? "" : " NOT ") +
                                    " In (" + (string)row[(int)Global.egAdvFilterProps.List] + ")";
                                break;
                            case ("DateList"):
                                string[] sa = ((string)row[(int)Global.egAdvFilterProps.List]).Split(',');
                                sFilter += "; " + sName + " between " + sa[0] + " and " + sa[1];
                                break;
                            case ("Boolean"):
                                sFilter += "; " + sName + " turned on";
                                break;
                        }
                    }
                }
                sFilter += "]<br />" + sOther;
            }
            else
            {
                sFilter = "Neither a " + scSTD + " nor an " + scADV + " flight operations filter is currently in effect.";
            }
            return sFilter;
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
        private void MPE_Show(Global.enumButtons eubtns)
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
            ModalPopExt.Show();
        }
        protected void Button_Click(object sender, EventArgs e)
        {
            Button pb = (Button)sender;
        }

        private void ProcessPopupException(Global.excToPopup excu)
        {
            ButtonsClear();
            lblPopupText.Text = excu.sExcMsg();
            ActivityLog.oLog(ActivityLog.enumLogTypes.PopupException, 0, lblPopupText.Text);
            MPE_Show(Global.enumButtons.OkOnly);
        }
        #endregion

        private void GlAggregates(DataTable dtuFilters)
        {
            DataTable dtAggr = PopulTree.dtGlAggregate(dtuFilters, ""); // Class PopulateTrVwFilters is defined in file StatisticianwFilters.cs
                                                                        // Before the above call makes sense, PopulTree.BuildAdvSubQuery must have been called for defining sSubquery; that's done in the Page_Load event handler.
            dtAggr.Columns.Add("TotalTimeHrs", Type.GetType("System.String"));
            dtAggr.Columns.Add("AvgFltDur", Type.GetType("System.String"));
            foreach (DataRow row in dtAggr.Rows)
            {
                row["AvgFltDur"] = ((int)row["NumFl"] < 1) ? "0" : ((decimal)((int)row["TotalTime"]) / (int)row["NumFl"]).ToString("F0");
                row["TotalTimeHrs"] = ((int)row["TotalTime"] / 60.0M).ToString("F2");
            }
            gvStatsByEqR.DataSource = dtAggr;
            gvStatsByEqR.DataBind(); // Display aggregate statistics
        }

        private void DisplayInGrid()
        {
            gvOps.DataSource = PopulTree.dtGlOps(dtFilters);
            gvOps.PageSize = Int32.Parse(mCRUD.GetSetting("PageSizeClubStats")); // SCR 216
            gvOps.DataBind();
            genToolTips(); // SCR 216
        }

        protected void trv_Ops_TreeNodePopulate(object sender, TreeNodeEventArgs e)
        {
            //ActivityLog.oDiag("Debug", "ClubStats.trv_Ops_TreeNodePopulate() was called with depth=" + e.Node.Depth.ToString());
            TreeNodePopulate(e.Node);
        }
        private void TreeNodePopulate(TreeNode tn)
        {
            // Makes use of PopulTree which is an instance of class PopulateTrVwFilters in file StatisticianwFilters.cs.
            //    Instance PopulTree is created when this page loads; PopulTree will be around until this page is unloaded.
            //    In particular, variable sSubQuery inside of PopulTree is important.
            if (tn.ChildNodes.Count == 0)
            {
                switch (tn.Depth)
                {
                    case (int)Global.enumDepths.Root:
                        PopulTree.PopulateYears(tn, dtFilters);
                        break;
                    case (int)Global.enumDepths.Year:
                        PopulTree.PopulateMonths(tn, dtFilters);
                        break;
                    case (int)Global.enumDepths.Month:
                        PopulTree.PopulateDays(tn, dtFilters);
                        break;
                    case (int)Global.enumDepths.Day:
                        PopulTree.PopulateOperations(tn, false, dtFilters);
                        break;
                    case (int)Global.enumDepths.Operation:
                        PopulTree.PopulateSpecialOpsInfo(tn);
                        PopulTree.PopulateOpDetails(tn);
                        break;
                    case (int)Global.enumDepths.OpDetail:
                        PopulTree.PopulateAviators(tn);
                        break;
                }
            }
        }

        protected void pbOpCancel_Click(object sender, EventArgs e)
        {
            // Even though this method is empty, it still must exist.
        }

        protected void pbAdvOpsFilters_Click(object sender, EventArgs e)
        {
            Server.Transfer("~/MemberPages/Stats/AdvStatsFilter.aspx", true);
        }

        protected void pbStdOpsFilters_Click(object sender, EventArgs e)
        {
            Server.Transfer("~/MemberPages/Stats/StdStatsFilter.aspx", true);
        }

        protected void gvOps_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvOps.PageIndex = e.NewPageIndex;
            DisplayInGrid();
        }
    }
}