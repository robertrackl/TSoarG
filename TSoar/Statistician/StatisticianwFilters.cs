using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.UI.WebControls;
using TSoar.DB;

namespace TSoar.Statistician
{
    class PopulateTrV
    {
        // Populates nodes of the TreeView control for displaying flight operations

        private string[] saMonths = new string[] { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };

        public PopulateTrV()
        {
        }

        public void PopulateYears(TreeNode node)
        {
            SqlCommand sqlQuery = new SqlCommand("SELECT DISTINCT DATEPART(year,DBegin) AS year FROM OPERATIONS ORDER BY year");
            DataSet resultSet = RunQuery(sqlQuery);
            if (resultSet.Tables.Count > 0)
            {
                foreach (DataRow row in resultSet.Tables[0].Rows)
                {
                    string sy = row["year"].ToString();
                    TreeNode NewNode = new TreeNode("y " + sy, sy);
                    NewNode.PopulateOnDemand = true;
                    NewNode.SelectAction = TreeNodeSelectAction.Select;
                    NewNode.Expanded = false;
                    node.ChildNodes.Add(NewNode);
                }
            }
        }

        public void PopulateMonths(TreeNode node)
        {
            SqlCommand sqlQuery = new SqlCommand("SELECT DISTINCT DATEPART(month,DBegin) AS month FROM OPERATIONS WHERE DATEPART(year,DBegin) = @Year " +
                "ORDER BY month");
            sqlQuery.Parameters.Add("@Year", SqlDbType.Int).Value = node.Value;
            DataSet resultSet = RunQuery(sqlQuery);
            if (resultSet.Tables.Count > 0)
            {
                foreach (DataRow row in resultSet.Tables[0].Rows)
                {
                    int m = (int)row["month"];
                    TreeNode NewNode = new TreeNode(saMonths[m - 1], m.ToString());
                    NewNode.PopulateOnDemand = true;
                    NewNode.SelectAction = TreeNodeSelectAction.Select;
                    NewNode.Expanded = false;
                    node.ChildNodes.Add(NewNode);
                }
            }
        }

        public void PopulateDays(TreeNode node)
        {
            SqlCommand sqlQuery = new SqlCommand("SELECT DISTINCT DATEPART(day,DBegin) AS day FROM OPERATIONS " +
                "WHERE DATEPART(year,DBegin) = @Year AND DATEPART(month,DBegin) = @Month ORDER BY day");
            sqlQuery.Parameters.Add("@Year", SqlDbType.Int).Value = node.Parent.Value;
            sqlQuery.Parameters.Add("@Month", SqlDbType.Int).Value = node.Value;
            DataSet resultSet = RunQuery(sqlQuery);
            if (resultSet.Tables.Count > 0)
            {
                foreach (DataRow row in resultSet.Tables[0].Rows)
                {
                    string sd = row["day"].ToString();
                    TreeNode NewNode = new TreeNode("d " + sd, sd);
                    NewNode.PopulateOnDemand = true;
                    NewNode.SelectAction = TreeNodeSelectAction.Select;
                    NewNode.Expanded = false;
                    node.ChildNodes.Add(NewNode);
                }
            }
        }

        public void PopulateOperations(TreeNode node, bool buWithOpId)
        {
            SqlCommand sqlQuery = new SqlCommand(
                "SELECT OPERATIONS.ID, LAUNCHMETHODS.sLaunchMethod, LOCATIONS.sLocation AS LocTakeoff, OPERATIONS.DBegin, " +
                    "LOCATIONS_1.sLocation AS LocLand, OPERATIONS.DEnd, OPERATIONS.sComment, CHARGECODES.sChargeCode, OPERATIONS.iInvoices2go " +
                "FROM LAUNCHMETHODS INNER JOIN " +
                    "OPERATIONS ON LAUNCHMETHODS.ID = OPERATIONS.iLaunchMethod INNER JOIN " +
                    "LOCATIONS ON OPERATIONS.iTakeoffLoc = LOCATIONS.ID INNER JOIN " +
                    "LOCATIONS AS LOCATIONS_1 ON OPERATIONS.iLandingLoc = LOCATIONS_1.ID INNER JOIN " +
                    "CHARGECODES ON OPERATIONS.iChargeCode = CHARGECODES.ID " +
                "WHERE CONVERT(date, OPERATIONS.DBegin) = DATEFROMPARTS(" +
                    node.Parent.Parent.Value + "," + node.Parent.Value + "," + node.Value + ") ORDER BY OPERATIONS.DBegin");
            DataSet resultSet;
            resultSet = RunQuery(sqlQuery);
            if (resultSet.Tables.Count > 0)
            {
                foreach (DataRow row in resultSet.Tables[0].Rows)
                {
                    string sID = row["ID"].ToString();
                    TreeNode NewNode = new TreeNode(row["sLaunchMethod"] + " T " + row["LocTakeoff"] + " " +
                        ((DateTime)row["DBegin"]).ToString("yyyy/MM/dd HH:mm") +
                        ", L " + row["LocLand"] + " " + ((DateTime)row["DEnd"]).ToString("yyyy/MM/dd HH:mm") +
                        ", " + row["sComment"] + ", CC " + row["sChargeCode"] + ", " + ((int)row["iInvoices2go"]).ToString(), sID);
                    if (buWithOpId)
                    {
                        NewNode.Text = "Op " + sID + ": " + NewNode.Text;
                    }
                    NewNode.PopulateOnDemand = true;
                    NewNode.SelectAction = TreeNodeSelectAction.Select;
                    NewNode.Expanded = false;
                    node.ChildNodes.Add(NewNode);
                }
            }
        }

        public void PopulateSpecialOpsInfo(TreeNode node)
        {
            SqlCommand sqlQuery = new SqlCommand();
            sqlQuery.CommandText = "SELECT SO.ID, ST.sSpecialOpType, SO.sDescription, SO.iDurationMinutes " +
                "FROM SPECIALOPTYPES ST INNER JOIN " +
                     "SPECIALOPS SO ON ST.ID = SO.iSpecialOpType " +
                "WHERE SO.iOperation = @OperationID";
            sqlQuery.Parameters.Add("@OperationID", SqlDbType.Int).Value = node.Value;
            DataSet resultSet = RunQuery(sqlQuery);
            if (resultSet.Tables.Count > 0)
            {
                foreach (DataRow row in resultSet.Tables[0].Rows)
                {
                    TreeNode NewNode = new TreeNode("Special Operations - " + row["sSpecialOpType"].ToString() + ": " + row["sDescription"].ToString() +
                        ", Duration " + row["iDurationMinutes"] + " minutes", row["ID"].ToString());
                    NewNode.PopulateOnDemand = true;
                    NewNode.SelectAction = TreeNodeSelectAction.Select;
                    NewNode.Expanded = false;
                    node.ChildNodes.Add(NewNode);
                }
            }
        }

        public void PopulateOpDetails(TreeNode node)
        {
            SqlCommand sqlQuery = new SqlCommand();
            sqlQuery.CommandText = "SELECT OD.ID, EQR.sEquipmentRole, EQ.sShortEquipName, OD.dMaxAltitude, OD.dReleaseAltitude " +
                "FROM EQUIPMENT EQ INNER JOIN " +
                     "OPDETAILS OD ON EQ.ID = OD.iEquip INNER JOIN " +
                     "EQUIPMENTROLES EQR ON OD.iEquipmentRole = EQR.ID " +
                "WHERE iOperation = @OperationID";
            sqlQuery.Parameters.Add("@OperationID", SqlDbType.Int).Value = node.Value;
            DataSet resultSet = RunQuery(sqlQuery);
            if (resultSet.Tables.Count > 0)
            {
                foreach (DataRow row in resultSet.Tables[0].Rows)
                {
                    TreeNode NewNode = new TreeNode(row["sEquipmentRole"].ToString() + ": " + row["sShortEquipName"].ToString() +
                        ", Rel Alt " + row["dReleaseAltitude"] + ", Max Alt " + row["dMaxAltitude"], row["ID"].ToString());
                    NewNode.PopulateOnDemand = true;
                    NewNode.SelectAction = TreeNodeSelectAction.Select;
                    NewNode.Expanded = false;
                    node.ChildNodes.Add(NewNode);
                }
            }
        }

        public void PopulateAviators(TreeNode node)
        {
            SqlCommand sqlQuery = new SqlCommand();
            sqlQuery.CommandText = "SELECT AVIATORS.ID AS ID_Av, AVIATORROLES.sAviatorRole, PEOPLE.sDisplayName, " +
                    "AVIATORS.dPercentCharge, AVIATORS.b1stFlight, AVIATORS.mInvoiced, AVIATORS.DInvoiced " +
                "FROM AVIATORROLES INNER JOIN " +
                     "AVIATORS ON AVIATORROLES.ID = AVIATORS.iAviatorRole INNER JOIN " +
                     "PEOPLE ON AVIATORS.iPerson = PEOPLE.ID " +
                "WHERE AVIATORS.iOpDetail = @OpDetailID";
            sqlQuery.Parameters.Add("@OpDetailID", SqlDbType.Int).Value = node.Value;
            DataSet resultSet = RunQuery(sqlQuery);
            if (resultSet.Tables.Count > 0)
            {
                foreach (DataRow row in resultSet.Tables[0].Rows)
                {
                    TreeNode NewNode = new TreeNode(row["sAviatorRole"].ToString() + ": " + row["sDisplayName"].ToString() +
                            ", % Charge = " + row["dPercentCharge"].ToString() +
                            (((bool)row["b1stFlight"]) ? ", First flight of season with instructor" : "") +
                            ", $invoiced = " + ((decimal)row["mInvoiced"]).ToString("F2") +
                            ", Date invoiced = " + CustFmt.sFmtDate((object)row["DInvoiced"],CustFmt.enDFmt.DateOnly),
                        row["ID_Av"].ToString());
                    NewNode.PopulateOnDemand = false;
                    NewNode.SelectAction = TreeNodeSelectAction.Select;
                    node.ChildNodes.Add(NewNode);
                }
            }
        }

        private DataSet RunQuery(SqlCommand sqlQuery)
        {
            using (SqlConnection SqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlConn"].ToString()))
            {
                SqlDataAdapter dbAdapter = new SqlDataAdapter();
                dbAdapter.SelectCommand = sqlQuery;
                sqlQuery.Connection = SqlConn;
                DataSet resultsDataSet = new DataSet();
                try
                {
                    dbAdapter.Fill(resultsDataSet);
                }
                catch (Exception exc)
                {
                    throw new Global.excToPopup("Statistician.cs.RunQuery: " + exc.Message);
                }
                return resultsDataSet;
            }
        }

        public void trv_OpsClosestInTime(TreeView trv_Ops)
        {
            trv_Ops.CollapseAll();
            trv_Ops.Nodes[0].Expand();
            // Find the operation that is closest in time to right now
            trv_Ops.CollapseAll();
            trv_Ops.Nodes[0].Expand();
            StatistDailyFlightLogDataContext dc = new StatistDailyFlightLogDataContext();
            int iOper = (from o in dc.OPERATIONs where o.ID == dc.iTNPF_ClosestOpInTime() select o.ID).First();

            DateTime DT = (DateTime)(from o in dc.OPERATIONs where o.ID == iOper select o.DBegin).First();
            int iCurrDP = DT.Year;
            int iTest;

            string sValuePath = "";
            TreeNode tnc;
            if (trv_Ops.Nodes[0].ChildNodes.Count > 0) // Nodes[0] is the root node
            {
                //Years
                foreach (TreeNode tn in trv_Ops.Nodes[0].ChildNodes)
                {
                    iTest = Int32.Parse(tn.Value);
                    if (iTest == iCurrDP)
                    {
                        sValuePath = tn.ValuePath;
                    }
                }
                tnc = trv_Ops.FindNode(sValuePath);
                tnc.Expand();
                // Months
                iCurrDP = DT.Month;
                if (tnc.ChildNodes.Count > 0)
                {
                    foreach (TreeNode tn in tnc.ChildNodes)
                    {
                        iTest = Int32.Parse(tn.Value);
                        if (iTest == iCurrDP)
                        {
                            sValuePath = tn.ValuePath;
                        }
                    }
                    tnc = trv_Ops.FindNode(sValuePath);
                    tnc.Expand();
                    // Days
                    iCurrDP = DT.Day;
                    if (tnc.ChildNodes.Count > 0)
                    {
                        foreach (TreeNode tn in tnc.ChildNodes)
                        {
                            iTest = Int32.Parse(tn.Value);
                            if (iTest == iCurrDP)
                            {
                                sValuePath = tn.ValuePath;
                            }
                        }
                        tnc = trv_Ops.FindNode(sValuePath);
                        tnc.Expand();
                    }
                }
                tnc.Select();
            }
        }
    }

    class PopulateTrVwFilters
    {
        // Populates nodes of the TreeView control for displaying flight operations; includes filtering of data

        #region Declarations
        private enum eWDatePart { Year, Month, Day}
        private DataTable dtLFilter; // Local copy of filter table
        private const string scErr = "Internal error in StatisticianwFilters.cs.dtDefaultAdvFilterSettings: Inconsistent row count at ";
        private string[] saMonths = new string[] { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };
        private string sSQL;
        private const string scSQL_FROM =
            "FROM EQUIPMENTROLES AS EQUIPMENTROLES_1 INNER JOIN " +
                "AVIATORROLES AS AVIATORROLES_1 INNER JOIN " +
                "AVIATORS AS AVIATORS_1 ON AVIATORROLES_1.ID = AVIATORS_1.iAviatorRole INNER JOIN " +
                "OPDETAILS AS OPDETAILS_1 ON AVIATORS_1.iOpDetail = OPDETAILS_1.ID ON EQUIPMENTROLES_1.ID = OPDETAILS_1.iEquipmentRole INNER JOIN " +
                "EQUIPTYPES AS EQUIPTYPES_1 INNER JOIN " +
                "EQUIPMENT AS EQUIPMENT_1 ON EQUIPTYPES_1.ID = EQUIPMENT_1.iEquipType ON OPDETAILS_1.iEquip = EQUIPMENT_1.ID INNER JOIN " +
                "OPERATIONS AS OPERATIONS_1 INNER JOIN " +
                "CHARGECODES AS CHARGECODES_1 ON OPERATIONS_1.iChargeCode = CHARGECODES_1.ID INNER JOIN " +
                "LOCATIONS AS TOLOCS ON OPERATIONS_1.iTakeoffLoc = TOLOCS.ID ON OPDETAILS_1.iOperation = OPERATIONS_1.ID INNER JOIN " +
                "PEOPLE AS PEOPLE_1 ON AVIATORS_1.iPerson = PEOPLE_1.ID INNER JOIN " +
                "LAUNCHMETHODS AS LAUNCHMETHODS_1 ON OPERATIONS_1.iLaunchMethod = LAUNCHMETHODS_1.ID INNER JOIN " +
                "LOCATIONS AS LDLOCS ON OPERATIONS_1.iLandingLoc = LDLOCS.ID ";
        private const string scSQL_SpecialOps = "LEFT OUTER JOIN " +
                "SPECIALOPS ON OPERATIONS_1.ID = SPECIALOPS.iOperation INNER JOIN " + // (SCR 110)
                "SPECIALOPTYPES ON SPECIALOPS.iSpecialOpType = SPECIALOPTYPES.ID ";
        private const string scSQL_SUBS1 =
                    "FROM OPDETAILS AS OPDETAILS_3 INNER JOIN " +
                         "OPERATIONS AS OPERATIONS_3 ON OPDETAILS_3.iOperation = OPERATIONS_3.ID " +
                    "WHERE " +
                    "(OPDETAILS_3.ID IN " +
                         "(SELECT ID " +
                               "FROM " +
                               "(SELECT OPDETAILS_2.ID, COUNT(AVIATORS_2.ID) AS AVCount " +
                                      "FROM OPDETAILS AS OPDETAILS_2 INNER JOIN " +
                                          "AVIATORS AS AVIATORS_2 ON OPDETAILS_2.ID = AVIATORS_2.iOpDetail " +
                                      "WHERE " +
                                      "(OPDETAILS_2.ID IN " +
                                          "(SELECT OPDETAILS_1.ID " + scSQL_FROM;
        private const string scSQL_SUBS2 = ")" +
                                      ")" +
                                   "GROUP BY OPDETAILS_2.ID " +
                                   "HAVING(COUNT(AVIATORS_2.ID) BETWEEN ";
        private const string scSQL_SUBS3 = ")" +
                               ") AS derivedtbl_1 " +
                          ")" +
                     ")";
        private string sSQL_WHERE = "TBD";
        private string sSubQuery = "(TBD)";
        #endregion

        public PopulateTrVwFilters()
        {
            //ActivityLog.oDiag("Debug", "PopulateTrVwFilters.PopulateTrVwFilters() was called");
        }

        public void bCheck4dtStdFilterReset(DataTable dtu, Label lblu)
        {
            dtu = AccountProfile.CurrentUser.OpsStdFilterSetting;
            bool b = false;
            if (dtu.Rows.Count < 1)
            {
                b = true;
            }
            else
            {
                // The LowLimit property in the very first row of the filter settings DataTable is used to store the
                //     filter settings version
                if (((decimal)dtu.Rows[(int)Global.egOpsFilters.Version][(int)Global.egAdvFilterProps.LowLimit]) !=
                        Global.dgcVersionOfOpsAdvFilterSettingDataTable) // there is only one version for both standard and advanced filters,
                                                                         // ... that's why we use Adv here. Global.dgcVersionOfOpsStdFilterSettingDataTable does not exist.
                {
                    lblu.Visible = true;
                    b = true;
                }
            }
            if (b)
            {
                // There was no entry in table aspnet_Profile for this user, or a version conflict is forcing a reset of dtu
                dtu = dtDefaultAdvFilterSettings();
                // Save filter settings for this user
                AccountProfile.CurrentUser.OpsStdFilterSetting = dtu;
            }
            return;
        }

        public void bCheck4dtAdvFilterReset(DataTable dtu, Label lblu)
        {
            dtu = AccountProfile.CurrentUser.OpsAdvFilterSetting;
            bool b = false;
            if (dtu.Rows.Count < 1)
            {
                b = true;
            }
            else
            {
                // The LowLimit property in the very first row of the filter settings DataTable is used to store the
                //     filter settings version
                if (((decimal)dtu.Rows[(int)Global.egOpsFilters.Version][(int)Global.egAdvFilterProps.LowLimit]) !=
                        Global.dgcVersionOfOpsAdvFilterSettingDataTable)
                {
                    lblu.Visible = true;
                    b = true;
                }
            }
            if (b)
            {
                // There was no entry in table aspnet_Profile for this user, or a version conflict is forcing a reset of dtu
                dtu = dtDefaultAdvFilterSettings();
                // Save filter settings for this user
                AccountProfile.CurrentUser.OpsAdvFilterSetting = dtu;
            }
            return;
        }

        public void BuildAdvSubQuery(DataTable dtu)
        {
            BuildAdvFilters(dtu); // builds the contents of variable sSQL_WHERE

            sSubQuery = "(SELECT DISTINCT OPDETAILS_1.ID " + scSQL_FROM;
            if ((bool)dtu.Rows[(int)Global.egOpsFilters.EnableFilteringOverall][(int)Global.egAdvFilterProps.Enabled] &&
                (bool)dtu.Rows[(int)Global.egOpsFilters.SpecialOps][(int)Global.egAdvFilterProps.Enabled])
            {
                sSubQuery += " " + scSQL_SpecialOps; // Add to the FROM clause
            }
            sSubQuery += " WHERE " + sSQL_WHERE ;
        }

        #region Build Filters
        private void BuildAdvFilters(DataTable udt)
        {
            //ActivityLog.oDiag("Debug", "PopulateTrVwFilters.BuildFilters Entry");
            sSQL_WHERE = "1=1";
            dtLFilter = udt; // Save local copy of filter settings
            if ((bool)udt.Rows[(int)Global.egOpsFilters.EnableFilteringOverall][(int)Global.egAdvFilterProps.Enabled])
            {
                sSQL_WHERE = "(";
                int iFilterCount = 0;
                //for(int ix= (int)Global.egOpsFilters.Person; ix <= (int)Global.egOpsFilters.FirstFlight; ix++)
                foreach (Global.egOpsFilters ex in (Global.egOpsFilters[]) Enum.GetValues(typeof(Global.egOpsFilters)))
                {
                    if (ex != Global.egOpsFilters.Version && ex != Global.egOpsFilters.EnableFilteringOverall)
                    {
                        int ix = (int)ex;
                        if ((bool)udt.Rows[ix][(int)Global.egAdvFilterProps.Enabled])
                        {
                            if (iFilterCount > 0) { sSQL_WHERE += " AND "; }
                            switch ((string)udt.Rows[ix][(int)Global.egAdvFilterProps.FilterType])
                            {
                                case "List":
                                    sSQL_WHERE += "(" + sBuildListFilter(udt, ix) + ")";
                                    break;
                                case "Boolean":
                                    sSQL_WHERE += "(" + sBuildBooleanFilter(udt, ix) + ")";
                                    break;
                                case "DateList":
                                    sSQL_WHERE += "(" + sBuildDateListFilter(udt, ix) + ")";
                                    break;
                                case "Range":
                                    sSQL_WHERE += "(" + sBuildRangeFilter(udt, ix) + ")";
                                    break;
                                default:
                                    throw new Global.excToPopup("StatisticianwFilters.cs.BuildFilters: Filter Type is not List, Boolean, DateList, or Range, but "
                                        + (string)udt.Rows[ix][(int)Global.egAdvFilterProps.FilterType]);
                            }
                            iFilterCount++;
                        }
                    }
                }
                sSQL_WHERE += ")";
                if (iFilterCount < 1)
                {
                    sSQL_WHERE = "1=1";
                }
                //ActivityLog.oDiag("Debug", "PopulateTrVwFilters.BuildFilters Exit with sSQL_WHERE=`" + sSQL_WHERE + "`");
            }
        }
        private string sBuildListFilter(DataTable udt, int iux)
        {
            if (((string)udt.Rows[iux][(int)Global.egAdvFilterProps.List]) == "All")
            {
                return "1=1";
            }
            else
            {
                return (string)udt.Rows[iux][(int)Global.egAdvFilterProps.Field] +
                    (((bool)udt.Rows[iux][(int)Global.egAdvFilterProps.INorEX]) ? "" : " NOT") +
                    " IN (" + (string)udt.Rows[iux][(int)Global.egAdvFilterProps.List] + ")";
            }
        }
        private string sBuildBooleanFilter(DataTable udt, int iux)
        {
            return (string)udt.Rows[iux][(int)Global.egAdvFilterProps.Field] + " = 1";
        }
        private string sBuildDateListFilter(DataTable udt, int iux)
        {
            string[] sa = ((string)udt.Rows[iux][(int)Global.egAdvFilterProps.List]).Split(',');
            if (sa.Length != 2)
            {
                throw new Global.excToPopup("StatisticianwFilters.cs.BuildFilters: DateList does not consist of exactly 2 comma-separated items, but " + sa.Length.ToString());
            }

            return "(" + (string)udt.Rows[iux][(int)Global.egAdvFilterProps.Field] +
                (((bool)udt.Rows[iux][(int)Global.egAdvFilterProps.INorEX]) ? "" : " NOT") +
                " BETWEEN " + sa[0] + " AND " + sa[1].Remove(sa[1].Length - 1) + " 23:59:59')";
        }
        private string sBuildRangeFilter(DataTable udt, int iux)
        {
            switch ((string)udt.Rows[iux][(int)Global.egAdvFilterProps.FilterName])
            {
                case "NumberOfOccupants":
                    // Number of Aviators per OpsDetail - this situation is handled separately by special code in:
                    // dtGlAggregate, and the various treenode population routines: PopulateYear through PopulateAviators.
                    return "1=1";
                case "ReleaseAltitudeFeet": case "TowAltitudeDifferenceFeet": case "DurationInMinutes":
                    return "(" + (string)udt.Rows[iux][(int)Global.egAdvFilterProps.Field] +
                        (((bool)udt.Rows[iux][(int)Global.egAdvFilterProps.INorEX]) ? "" : " NOT") +
                        " BETWEEN " + ((decimal)udt.Rows[iux][(int)Global.egAdvFilterProps.LowLimit]).ToString() + " AND " +
                        ((decimal)udt.Rows[iux][(int)Global.egAdvFilterProps.HighLimit]).ToString() + ")";
                default:
                    return "(1=1)";
            }
        }
        #endregion

        public DataTable dtDefaultAdvFilterSettings()
        {
            //ActivityLog.oDiag("Debug", "PopulateTrVwFilters.dtDefaultAdvFilterSettings() was called");
            DataTable dt = new DataTable("OpsAdvFilterSetting");
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
                //Important Note:
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
            row[(int)Global.egAdvFilterProps.Enabled] = true; // is set to true to be included in filter list in ClubStats.aspx.cs.Page_Load
            row[(int)Global.egAdvFilterProps.INorEX] = false;
            row[(int)Global.egAdvFilterProps.List] = "";
            row[(int)Global.egAdvFilterProps.LowLimit] = 2.0M; // Version number of the filter settings DataTable; compare with Global.dgcVersionOfOpsAdvFilterSettingsDataTable
            row[(int)Global.egAdvFilterProps.HighLimit] = 0M;
            row[(int)Global.egAdvFilterProps.PunctuationMark] = "";
            row[(int)Global.egAdvFilterProps.Field] = "";
            dt.Rows.Add(row);
            if ((dt.Rows.Count - 1) != (int)Global.egOpsFilters.Version)
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
            if ((dt.Rows.Count - 1) != (int)Global.egOpsFilters.EnableFilteringOverall)
            {
                throw new Global.excToPopup(scErr + row[(int)Global.egAdvFilterProps.FilterName]);
            }
            #endregion
            #region Aviator
            row = dt.NewRow();
            row[(int)Global.egAdvFilterProps.FilterName] = "Aviator";
            row[(int)Global.egAdvFilterProps.FilterType] = "List";
            row[(int)Global.egAdvFilterProps.Enabled] = false;
            row[(int)Global.egAdvFilterProps.INorEX] = true;
            row[(int)Global.egAdvFilterProps.List] = "";
            row[(int)Global.egAdvFilterProps.LowLimit] = 0.0;
            row[(int)Global.egAdvFilterProps.HighLimit] = 0.0;
            row[(int)Global.egAdvFilterProps.PunctuationMark] = "'";
            row[(int)Global.egAdvFilterProps.Field] = "PEOPLE_1.sDisplayName";
            dt.Rows.Add(row);
            if ((dt.Rows.Count - 1) != (int)Global.egOpsFilters.Person)
            {
                throw new Global.excToPopup(scErr + row[(int)Global.egAdvFilterProps.FilterName]);
            }
            #endregion
            #region AviatorRole
            row = dt.NewRow();
            row[(int)Global.egAdvFilterProps.FilterName] = "AviatorRole";
            row[(int)Global.egAdvFilterProps.FilterType] = "List";
            row[(int)Global.egAdvFilterProps.Enabled] = false;
            row[(int)Global.egAdvFilterProps.INorEX] = true;
            row[(int)Global.egAdvFilterProps.List] = "";
            row[(int)Global.egAdvFilterProps.LowLimit] = 0.0;
            row[(int)Global.egAdvFilterProps.HighLimit] = 0.0;
            row[(int)Global.egAdvFilterProps.PunctuationMark] = "'";
            row[(int)Global.egAdvFilterProps.Field] = "AVIATORROLES_1.sAviatorRole";
            dt.Rows.Add(row);
            if ((dt.Rows.Count - 1) != (int)Global.egOpsFilters.AviatorRole)
            {
                throw new Global.excToPopup(scErr + row[(int)Global.egAdvFilterProps.FilterName]);
            }
            #endregion
            #region TakeoffLocation
            row = dt.NewRow();
            row[(int)Global.egAdvFilterProps.FilterName] = "TakeoffLocation";
            row[(int)Global.egAdvFilterProps.FilterType] = "List";
            row[(int)Global.egAdvFilterProps.Enabled] = false;
            row[(int)Global.egAdvFilterProps.INorEX] = true;
            row[(int)Global.egAdvFilterProps.List] = "";
            row[(int)Global.egAdvFilterProps.LowLimit] = 0.0;
            row[(int)Global.egAdvFilterProps.HighLimit] = 0.0;
            row[(int)Global.egAdvFilterProps.PunctuationMark] = "'";
            row[(int)Global.egAdvFilterProps.Field] = "TOLOCS.sLocation";
            dt.Rows.Add(row);
            if ((dt.Rows.Count - 1) != (int)Global.egOpsFilters.TakeoffLocation)
            {
                throw new Global.excToPopup(scErr + row[(int)Global.egAdvFilterProps.FilterName]);
            }
            #endregion
            #region LandingLocation
            row = dt.NewRow();
            row[(int)Global.egAdvFilterProps.FilterName] = "LandingLocation";
            row[(int)Global.egAdvFilterProps.FilterType] = "List";
            row[(int)Global.egAdvFilterProps.Enabled] = false;
            row[(int)Global.egAdvFilterProps.INorEX] = true;
            row[(int)Global.egAdvFilterProps.List] = "";
            row[(int)Global.egAdvFilterProps.LowLimit] = 0.0;
            row[(int)Global.egAdvFilterProps.HighLimit] = 0.0;
            row[(int)Global.egAdvFilterProps.PunctuationMark] = "'";
            row[(int)Global.egAdvFilterProps.Field] = "LDLOCS.sLocation";
            dt.Rows.Add(row);
            if ((dt.Rows.Count - 1) != (int)Global.egOpsFilters.LandingLocation)
            {
                throw new Global.excToPopup(scErr + row[(int)Global.egAdvFilterProps.FilterName]);
            }
            #endregion
            #region ChargeCode
            row = dt.NewRow();
            row[(int)Global.egAdvFilterProps.FilterName] = "ChargeCode";
            row[(int)Global.egAdvFilterProps.FilterType] = "List";
            row[(int)Global.egAdvFilterProps.Enabled] = false;
            row[(int)Global.egAdvFilterProps.INorEX] = true;
            row[(int)Global.egAdvFilterProps.List] = "";
            row[(int)Global.egAdvFilterProps.LowLimit] = 0.0;
            row[(int)Global.egAdvFilterProps.HighLimit] = 0.0;
            row[(int)Global.egAdvFilterProps.PunctuationMark] = "'";
            row[(int)Global.egAdvFilterProps.Field] = "CHARGECODES_1.sChargeCode";
            dt.Rows.Add(row);
            if ((dt.Rows.Count - 1) != (int)Global.egOpsFilters.ChargeCode)
            {
                throw new Global.excToPopup(scErr + row[(int)Global.egAdvFilterProps.FilterName]);
            }
            #endregion
            #region LaunchMethod
            row = dt.NewRow();
            row[(int)Global.egAdvFilterProps.FilterName] = "LaunchMethod";
            row[(int)Global.egAdvFilterProps.FilterType] = "List";
            row[(int)Global.egAdvFilterProps.Enabled] = false;
            row[(int)Global.egAdvFilterProps.INorEX] = true;
            row[(int)Global.egAdvFilterProps.List] = "";
            row[(int)Global.egAdvFilterProps.LowLimit] = 0.0;
            row[(int)Global.egAdvFilterProps.HighLimit] = 0.0;
            row[(int)Global.egAdvFilterProps.PunctuationMark] = "'";
            row[(int)Global.egAdvFilterProps.Field] = "LAUNCHMETHODS_1.sLaunchMethod";
            dt.Rows.Add(row);
            if ((dt.Rows.Count - 1) != (int)Global.egOpsFilters.LaunchMethod)
            {
                throw new Global.excToPopup(scErr + row[(int)Global.egAdvFilterProps.FilterName]);
            }
            #endregion
            #region Special Operations (SCR 110)
            row = dt.NewRow();
            row[(int)Global.egAdvFilterProps.FilterName] = "SpecialOps";
            row[(int)Global.egAdvFilterProps.FilterType] = "List";
            row[(int)Global.egAdvFilterProps.Enabled] = false;
            row[(int)Global.egAdvFilterProps.INorEX] = true;
            row[(int)Global.egAdvFilterProps.List] = "";
            row[(int)Global.egAdvFilterProps.LowLimit] = 0.0;
            row[(int)Global.egAdvFilterProps.HighLimit] = 0.0;
            row[(int)Global.egAdvFilterProps.PunctuationMark] = "'";
            row[(int)Global.egAdvFilterProps.Field] = "SPECIALOPTYPES.sSpecialOpType";
            dt.Rows.Add(row);
            if ((dt.Rows.Count - 1) != (int)Global.egOpsFilters.SpecialOps)
            {
                throw new Global.excToPopup(scErr + row[(int)Global.egAdvFilterProps.FilterName]);
            }
            #endregion
            #region Equipment
            row = dt.NewRow();
            row[(int)Global.egAdvFilterProps.FilterName] = "Equipment";
            row[(int)Global.egAdvFilterProps.FilterType] = "List";
            row[(int)Global.egAdvFilterProps.Enabled] = false;
            row[(int)Global.egAdvFilterProps.INorEX] = true;
            row[(int)Global.egAdvFilterProps.List] = "";
            row[(int)Global.egAdvFilterProps.LowLimit] = 0.0;
            row[(int)Global.egAdvFilterProps.HighLimit] = 0.0;
            row[(int)Global.egAdvFilterProps.PunctuationMark] = "'";
            row[(int)Global.egAdvFilterProps.Field] = "EQUIPMENT_1.sShortEquipName";
            dt.Rows.Add(row);
            if ((dt.Rows.Count - 1) != (int)Global.egOpsFilters.Equipment)
            {
                throw new Global.excToPopup(scErr + row[(int)Global.egAdvFilterProps.FilterName]);
            }
            #endregion
            #region EquipmentRole
            row = dt.NewRow();
            row[(int)Global.egAdvFilterProps.FilterName] = "EquipmentRole";
            row[(int)Global.egAdvFilterProps.FilterType] = "List";
            row[(int)Global.egAdvFilterProps.Enabled] = true;
            row[(int)Global.egAdvFilterProps.INorEX] = true;
            row[(int)Global.egAdvFilterProps.List] = "'Glider'";
            row[(int)Global.egAdvFilterProps.LowLimit] = 0.0;
            row[(int)Global.egAdvFilterProps.HighLimit] = 0.0;
            row[(int)Global.egAdvFilterProps.PunctuationMark] = "'";
            row[(int)Global.egAdvFilterProps.Field] = "EQUIPMENTROLES_1.sEquipmentRole";
            dt.Rows.Add(row);
            if ((dt.Rows.Count - 1) != (int)Global.egOpsFilters.EquipmentRole)
            {
                throw new Global.excToPopup(scErr + row[(int)Global.egAdvFilterProps.FilterName]);
            }
            #endregion
            #region EquipmentType
            row = dt.NewRow();
            row[(int)Global.egAdvFilterProps.FilterName] = "EquipmentType";
            row[(int)Global.egAdvFilterProps.FilterType] = "List";
            row[(int)Global.egAdvFilterProps.Enabled] = false;
            row[(int)Global.egAdvFilterProps.INorEX] = true;
            row[(int)Global.egAdvFilterProps.List] = "";
            row[(int)Global.egAdvFilterProps.LowLimit] = 0.0;
            row[(int)Global.egAdvFilterProps.HighLimit] = 0.0;
            row[(int)Global.egAdvFilterProps.PunctuationMark] = "'";
            row[(int)Global.egAdvFilterProps.Field] = "EQUIPTYPES_1.sEquipmentType";
            dt.Rows.Add(row);
            if ((dt.Rows.Count - 1) != (int)Global.egOpsFilters.EquipmentType)
            {
                throw new Global.excToPopup(scErr + row[(int)Global.egAdvFilterProps.FilterName]);
            }
            #endregion
            #region TakeoffDate
            row = dt.NewRow();
            row[(int)Global.egAdvFilterProps.FilterName] = "TakeoffDate";
            row[(int)Global.egAdvFilterProps.FilterType] = "DateList";
            row[(int)Global.egAdvFilterProps.Enabled] = false;
            row[(int)Global.egAdvFilterProps.INorEX] = true;
            row[(int)Global.egAdvFilterProps.List] = "'2000/01/01', '2099/12/31'";
            row[(int)Global.egAdvFilterProps.LowLimit] = 0.0M;
            row[(int)Global.egAdvFilterProps.HighLimit] = 0.0M;
            row[(int)Global.egAdvFilterProps.PunctuationMark] = "'";
            row[(int)Global.egAdvFilterProps.Field] = "OPERATIONS_1.DBegin";
            dt.Rows.Add(row);
            if ((dt.Rows.Count - 1) != (int)Global.egOpsFilters.TakeoffDate)
            {
                throw new Global.excToPopup(scErr + row[(int)Global.egAdvFilterProps.FilterName]);
            }
            #endregion
            #region NumberOfOccupants
            row = dt.NewRow();
            row[(int)Global.egAdvFilterProps.FilterName] = "NumberOfOccupants";
            row[(int)Global.egAdvFilterProps.FilterType] = "Range";
            row[(int)Global.egAdvFilterProps.Enabled] = false;
            row[(int)Global.egAdvFilterProps.INorEX] = true;
            row[(int)Global.egAdvFilterProps.List] = "";
            row[(int)Global.egAdvFilterProps.LowLimit] = 1M;
            row[(int)Global.egAdvFilterProps.HighLimit] = 10M;
            row[(int)Global.egAdvFilterProps.PunctuationMark] = "";
            row[(int)Global.egAdvFilterProps.Field] = "";
            dt.Rows.Add(row);
            if ((dt.Rows.Count - 1) != (int)Global.egOpsFilters.NumOccup)
            {
                throw new Global.excToPopup(scErr + row[(int)Global.egAdvFilterProps.FilterName]);
            }
            #endregion
            #region ReleaseAltitudeFeet
            row = dt.NewRow();
            row[(int)Global.egAdvFilterProps.FilterName] = "ReleaseAltitudeFeet";
            row[(int)Global.egAdvFilterProps.FilterType] = "Range";
            row[(int)Global.egAdvFilterProps.Enabled] = false;
            row[(int)Global.egAdvFilterProps.INorEX] = true;
            row[(int)Global.egAdvFilterProps.List] = "";
            row[(int)Global.egAdvFilterProps.LowLimit] = -1000M;
            row[(int)Global.egAdvFilterProps.HighLimit] = 30000M;
            row[(int)Global.egAdvFilterProps.PunctuationMark] = "";
            row[(int)Global.egAdvFilterProps.Field] = "OPDETAILS_1.dReleaseAltitude";
            dt.Rows.Add(row);
            if ((dt.Rows.Count - 1) != (int)Global.egOpsFilters.ReleaseAltit)
            {
                throw new Global.excToPopup(scErr + row[(int)Global.egAdvFilterProps.FilterName]);
            }
            #endregion
            #region TowAltitudeDifferenceFeet
            row = dt.NewRow();
            row[(int)Global.egAdvFilterProps.FilterName] = "TowAltitudeDifferenceFeet";
            row[(int)Global.egAdvFilterProps.FilterType] = "Range";
            row[(int)Global.egAdvFilterProps.Enabled] = false;
            row[(int)Global.egAdvFilterProps.INorEX] = true;
            row[(int)Global.egAdvFilterProps.List] = "";
            row[(int)Global.egAdvFilterProps.LowLimit] = 0M;
            row[(int)Global.egAdvFilterProps.HighLimit] = 30000M;
            row[(int)Global.egAdvFilterProps.PunctuationMark] = "";
            row[(int)Global.egAdvFilterProps.Field] = "(OPDETAILS_1.dReleaseAltitude - TOLOCS.dRunwayAltitude)";
            dt.Rows.Add(row);
            if ((dt.Rows.Count - 1) != (int)Global.egOpsFilters.TowAltDiff)
            {
                throw new Global.excToPopup(scErr + row[(int)Global.egAdvFilterProps.FilterName]);
            }
            #endregion
            #region DurationInMinutes
            row = dt.NewRow();
            row[(int)Global.egAdvFilterProps.FilterName] = "DurationInMinutes";
            row[(int)Global.egAdvFilterProps.FilterType] = "Range";
            row[(int)Global.egAdvFilterProps.Enabled] = false;
            row[(int)Global.egAdvFilterProps.INorEX] = true;
            row[(int)Global.egAdvFilterProps.List] = "";
            row[(int)Global.egAdvFilterProps.LowLimit] = 0M;
            row[(int)Global.egAdvFilterProps.HighLimit] = 2880M;
            row[(int)Global.egAdvFilterProps.PunctuationMark] = "";
            row[(int)Global.egAdvFilterProps.Field] = "(DATEDIFF(minute, OPERATIONS_1.DBegin, OPERATIONS_1.DEnd))";
            dt.Rows.Add(row);
            if ((dt.Rows.Count - 1) != (int)Global.egOpsFilters.Duration)
            {
                throw new Global.excToPopup(scErr + row[(int)Global.egAdvFilterProps.FilterName]);
            }
            #endregion
            #region FirstFlightOfSeasonWithInstructor
            row = dt.NewRow();
            row[(int)Global.egAdvFilterProps.FilterName] = "FirstFlightOfSeasonWithInstructor";
            row[(int)Global.egAdvFilterProps.FilterType] = "Boolean";
            row[(int)Global.egAdvFilterProps.Enabled] = false;
            row[(int)Global.egAdvFilterProps.INorEX] = true;
            row[(int)Global.egAdvFilterProps.List] = "";
            row[(int)Global.egAdvFilterProps.LowLimit] = 0.0M;
            row[(int)Global.egAdvFilterProps.HighLimit] = 0.0M;
            row[(int)Global.egAdvFilterProps.PunctuationMark] = "'";
            row[(int)Global.egAdvFilterProps.Field] = "AVIATORS_1.b1stFlight";
            dt.Rows.Add(row);
            if ((dt.Rows.Count - 1) != (int)Global.egOpsFilters.FirstFlight)
            {
                throw new Global.excToPopup(scErr + row[(int)Global.egAdvFilterProps.FilterName]);
            }
            #endregion
            #endregion
            return dt;
        }

        public DataTable dtGlAggregate(DataTable dtuFilters, string suNodeFilter)
        {
            // Provides a DataTable with aggregate statistics of number of flights and hours flown.
            //   If suNodeFilter is not an empty string then the data are provided grouped by Equipment Role.

            string sSqlFrom = scSQL_FROM;
            // If a filter is set on Special Operations, the FROM clause in the SQL code needs an addition involving a LEFT OUTER JOIN
            if ((bool)dtuFilters.Rows[(int)Global.egOpsFilters.EnableFilteringOverall][(int)Global.egAdvFilterProps.Enabled] &&
                (bool)dtuFilters.Rows[(int)Global.egOpsFilters.SpecialOps][(int)Global.egAdvFilterProps.Enabled])
            {
                sSqlFrom += " " + scSQL_SpecialOps;
            }

            // If a filter is set on the number of occupants in an aircraft, the SQL code is quite different
            if ((bool)dtuFilters.Rows[(int)Global.egOpsFilters.EnableFilteringOverall][(int)Global.egAdvFilterProps.Enabled] &&
                (bool)dtuFilters.Rows[(int)Global.egOpsFilters.NumOccup][(int)Global.egAdvFilterProps.Enabled])
            {
                if (suNodeFilter.Length > 0)
                {
                    sSQL = "SELECT COUNT(*) AS NumFl, SUM(DATEDIFF(minute, DBegin, DEnd)) AS TotalTime " +
                    "FROM OPDETAILS AS OPDETAILS_3 INNER JOIN " +
                         "OPERATIONS AS OPERATIONS_3 ON OPDETAILS_3.iOperation = OPERATIONS_3.ID " +
                    "WHERE " +
                    "(OPDETAILS_3.ID IN " +
                         "(SELECT ID " +
                               "FROM " +
                               "(SELECT OPDETAILS_2.ID, COUNT(AVIATORS_2.ID) AS AVCount " +
                                      "FROM OPDETAILS AS OPDETAILS_2 INNER JOIN " +
                                          "AVIATORS AS AVIATORS_2 ON OPDETAILS_2.ID = AVIATORS_2.iOpDetail " +
                                      "WHERE " +
                                      "(OPDETAILS_2.ID IN " +
                                          "(SELECT OPDETAILS_1.ID " +
                                            sSqlFrom +
                                           " WHERE " + sSQL_WHERE + " " + suNodeFilter +
                                           ")" +
                                      ")" +
                                   "GROUP BY OPDETAILS_2.ID " +
                                   "HAVING(COUNT(AVIATORS_2.ID) BETWEEN " +
                                           ((decimal)dtuFilters.Rows[(int)Global.egOpsFilters.NumOccup][(int)Global.egAdvFilterProps.LowLimit]).ToString() + " AND " +
                                           ((decimal)dtuFilters.Rows[(int)Global.egOpsFilters.NumOccup][(int)Global.egAdvFilterProps.HighLimit]).ToString() +
                                         ")" +
                               ") AS derivedtbl_1 " +
                          ")" +
                     ")";
                }
                else
                {
                    sSQL = "SELECT EQUIPMENTROLES_3.sEquipmentRole, COUNT(*) AS NumFl, " +
                            "SUM(IIF(EQUIPMENTROLES_3.sEquipmentRole='Glider', DATEDIFF(minute,OPERATIONS_3.DBegin, OPERATIONS_3.DEnd), EQUIPMENTROLES_3.iAvgUseDurationMinutes)) AS TotalTime " +
                    "FROM EQUIPMENTROLES AS EQUIPMENTROLES_3 INNER JOIN OPDETAILS AS OPDETAILS_3 ON EQUIPMENTROLES_3.ID = OPDETAILS_3.iEquipmentRole INNER JOIN " +
                         "OPERATIONS AS OPERATIONS_3 ON OPDETAILS_3.iOperation = OPERATIONS_3.ID " +
                    "WHERE " +
                    "(OPDETAILS_3.ID IN " +
                         "(SELECT ID " +
                               "FROM " +
                               "(SELECT OPDETAILS_2.ID, COUNT(AVIATORS_2.ID) AS AVCount " +
                                      "FROM OPDETAILS AS OPDETAILS_2 INNER JOIN " +
                                          "AVIATORS AS AVIATORS_2 ON OPDETAILS_2.ID = AVIATORS_2.iOpDetail " +
                                      "WHERE " +
                                      "(OPDETAILS_2.ID IN " +
                                          "(SELECT OPDETAILS_1.ID " +
                                            sSqlFrom +
                                           " WHERE " + sSQL_WHERE +
                                           ")" +
                                      ")" +
                                   "GROUP BY OPDETAILS_2.ID " +
                                   "HAVING(COUNT(AVIATORS_2.ID) BETWEEN " +
                                           ((decimal)dtuFilters.Rows[(int)Global.egOpsFilters.NumOccup][(int)Global.egAdvFilterProps.LowLimit]).ToString() + " AND " +
                                           ((decimal)dtuFilters.Rows[(int)Global.egOpsFilters.NumOccup][(int)Global.egAdvFilterProps.HighLimit]).ToString() +
                                         ")" +
                               ") AS derivedtbl_1 " +
                          ")" +
                     ") GROUP BY EQUIPMENTROLES_3.sEquipmentRole";
                }
            }
            else
            {
                // sSubQuery is a class-wide variable
                if (suNodeFilter.Length > 0)
                {
                    sSQL = "SELECT COUNT(*) AS NumFl, " +
                                "SUM(DATEDIFF(minute,OPERATIONS_3.DBegin, OPERATIONS_3.DEnd)) AS TotalTime " +
                           "FROM OPDETAILS AS OPDETAILS_3 INNER JOIN " +
                                "OPERATIONS AS OPERATIONS_3 ON OPDETAILS_3.iOperation = OPERATIONS_3.ID " +
                           "WHERE " +
                           "(OPDETAILS_3.ID IN " + sSubQuery + " " + suNodeFilter + "))";
                }
                else
                {
                    sSQL = "SELECT EQUIPMENTROLES_3.sEquipmentRole, COUNT(*) AS NumFl, " +
                            "SUM(IIF(EQUIPMENTROLES_3.sEquipmentRole='Glider', DATEDIFF(minute,OPERATIONS_3.DBegin, OPERATIONS_3.DEnd), EQUIPMENTROLES_3.iAvgUseDurationMinutes)) AS TotalTime " +
                        "FROM EQUIPMENTROLES AS EQUIPMENTROLES_3 INNER JOIN OPDETAILS AS OPDETAILS_3 ON EQUIPMENTROLES_3.ID = OPDETAILS_3.iEquipmentRole " +
                            "INNER JOIN OPERATIONS AS OPERATIONS_3 ON OPDETAILS_3.iOperation = OPERATIONS_3.ID  " +
                        "WHERE " +
                            "(OPDETAILS_3.ID IN " + sSubQuery;
                    sSQL += ")) " + "GROUP BY EQUIPMENTROLES_3.sEquipmentRole";
                }
            }
            SqlCommand sqlQuery = new SqlCommand(sSQL);
            DataSet resultSet;
            resultSet = RunQuery(sqlQuery);
            if (resultSet.Tables.Count > 0)
            {
                return resultSet.Tables[0];
            }
            else
            {
                return null;
            }
        }

        public DataTable dtGlOps(DataTable dtuFilters)
        {
            // Provides a DataTable with detailed operational flying data.

            string sSqlFrom = scSQL_FROM;
            // If a filter is set on Special Operations, the FROM clause in the SQL code needs an addition involving a LEFT OUTER JOIN
            if ((bool)dtuFilters.Rows[(int)Global.egOpsFilters.EnableFilteringOverall][(int)Global.egAdvFilterProps.Enabled] &&
                (bool)dtuFilters.Rows[(int)Global.egOpsFilters.SpecialOps][(int)Global.egAdvFilterProps.Enabled])
            {
                sSqlFrom += " " + scSQL_SpecialOps;
            }

            sSQL = "SELECT IIF(O_3.ID = LAG(O_3.ID) OVER(ORDER BY O_3.ID), N' ', CAST(O_3.ID AS nvarchar(12))) AS sOpID, " +
                    "ROW_NUMBER() OVER(PARTITION BY O_3.ID ORDER BY O_3.DBegin, LOCFROM.sLocation, O_3.ID) AS iCount, " +
                    "IIF(O_3.ID = LAG(O_3.ID) OVER(ORDER BY O_3.ID), N' ', LAUNCHMETHODS.cLaunchMethod) AS cLaunchMethod, " +
                    "IIF(O_3.ID = LAG(O_3.ID) OVER(ORDER BY O_3.ID), N' ', LOCFROM.sLocation)  AS TOLoc, " +
                    "IIF(O_3.ID = LAG(O_3.ID) OVER(ORDER BY O_3.ID), N' ', FORMAT(O_3.DBegin,'yyyy/MM/dd HH:mm')) AS sDBegin, " +
                    //"IIF(O_3.ID = LAG(O_3.ID) OVER(ORDER BY O_3.ID), N' ', " + SCR 215
                    "IIF(CHARINDEX('Tow', AVIATORROLES.sAviatorRole) < 1, CAST(DATEDIFF(minute, O_3.DBegin, O_3.DEnd) AS nvarchar(12)), N' ') AS sDuratMin, " + // SCR 215
                    "IIF(O_3.ID = LAG(O_3.ID) OVER(ORDER BY O_3.ID), N' ', LOCTO.sLocation) AS LDGLoc, " +
                    "IIF(O_3.ID = LAG(O_3.ID) OVER(ORDER BY O_3.ID), N' ', CHARGECODES.cChargeCode) AS cChargeCode, " +
                    "IIF(O_3.ID = LAG(O_3.ID) OVER(ORDER BY O_3.ID), N' ', O_3.sComment) AS sComment,  " +
                    "IIF(O_3.ID = LAG(O_3.ID) OVER(ORDER BY O_3.ID), N' ', " +
                        "CAST((SELECT COUNT(*) FROM OPERATIONS AS O_4 INNER JOIN SPECIALOPS ON O_4.ID=SPECIALOPS.iOperation WHERE O_4.ID=O_3.ID) AS nvarchar(3))" +
                            ") AS sSpOp, " +
                    "IIF((O_3.ID = LAG(O_3.ID) OVER(ORDER BY O_3.ID)) AND (EQUIPMENTROLES_3.sEquipmentRole = LAG(EQUIPMENTROLES_3.sEquipmentRole) OVER(ORDER BY O_3.ID)), " +
                            "N' ', EQUIPMENTROLES_3.sEquipmentRole) AS sEquipmentRole," +
                    "IIF((O_3.ID = LAG(O_3.ID) OVER(ORDER BY O_3.ID)) AND (EQUIPMENT.sShortEquipName = LAG(EQUIPMENT.sShortEquipName) OVER(ORDER BY O_3.ID)), " +
                            "N' ', EQUIPMENT.sShortEquipName) AS sShortEquipName," +
                    "IIF(O_3.ID = LAG(O_3.ID) OVER(ORDER BY O_3.ID), N' ', CAST(OPDETAILS_3.dReleaseAltitude AS nvarchar(15))) AS sReleaseAltitude, " +
                    "AVIATORROLES.sAviatorRole, PEOPLE.sDisplayName, AVIATORS.dPercentCharge, AVIATORS.b1stFlight " +
            "FROM EQUIPMENTROLES AS EQUIPMENTROLES_3 " +
                    "INNER JOIN OPDETAILS AS OPDETAILS_3   ON EQUIPMENTROLES_3.ID    = OPDETAILS_3.iEquipmentRole " +
                    "INNER JOIN OPERATIONS AS O_3 ON OPDETAILS_3.iOperation = O_3.ID " +
                    "INNER JOIN LOCATIONS AS LOCFROM       ON LOCFROM.ID             = O_3.iTakeOffLoc " +
                    "INNER JOIN LOCATIONS AS LOCTO         ON LOCTO.ID               = O_3.iLandingLoc " +
                    "INNER JOIN LAUNCHMETHODS              ON LAUNCHMETHODS.ID       = O_3.iLaunchMethod " +
                    "INNER JOIN CHARGECODES                ON CHARGECODES.ID         = O_3.iChargeCode " +
                    "INNER JOIN EQUIPMENT                  ON EQUIPMENT.ID           = OPDETAILS_3.iEquip " +
                    "INNER JOIN AVIATORS                   ON OPDETAILS_3.ID         = AVIATORS.iOpDetail " +
                    "INNER JOIN AVIATORROLES               ON AVIATORROLES.ID        = AVIATORS.iAviatorRole " +
                    "INNER JOIN PEOPLE                     ON PEOPLE.ID              = AVIATORS.iPerson ";

            // If a filter is set on the number of occupants in an aircraft, the SQL code is quite different
            if ((bool)dtuFilters.Rows[(int)Global.egOpsFilters.EnableFilteringOverall][(int)Global.egAdvFilterProps.Enabled] &&
                (bool)dtuFilters.Rows[(int)Global.egOpsFilters.NumOccup][(int)Global.egAdvFilterProps.Enabled])
            {
                sSQL += "WHERE " +
                "(OPDETAILS_3.ID IN " +
                        "(SELECT ID " +
                            "FROM " +
                            "(SELECT OPDETAILS_2.ID, COUNT(AVIATORS_2.ID) AS AVCount " +
                                    "FROM OPDETAILS AS OPDETAILS_2 INNER JOIN " +
                                        "AVIATORS AS AVIATORS_2 ON OPDETAILS_2.ID = AVIATORS_2.iOpDetail " +
                                    "WHERE " +
                                    "(OPDETAILS_2.ID IN " +
                                        "(SELECT OPDETAILS_1.ID " +
                                        sSqlFrom +
                                        " WHERE " + sSQL_WHERE +
                                        ")" +
                                    ")" +
                                "GROUP BY OPDETAILS_2.ID " +
                                "HAVING(COUNT(AVIATORS_2.ID) BETWEEN " +
                                        ((decimal)dtuFilters.Rows[(int)Global.egOpsFilters.NumOccup][(int)Global.egAdvFilterProps.LowLimit]).ToString() + " AND " +
                                        ((decimal)dtuFilters.Rows[(int)Global.egOpsFilters.NumOccup][(int)Global.egAdvFilterProps.HighLimit]).ToString() +
                                        ")" +
                            ") AS derivedtbl_1 " +
                        ")" +
                 ")";
            }
            else
            {
                // sSubQuery is a class-wide variable
                sSQL += "WHERE (OPDETAILS_3.ID IN " + sSubQuery + ")) ";
                sSQL += "ORDER BY O_3.DBegin, LOCFROM.sLocation, O_3.ID";
            }
            SqlCommand sqlQuery = new SqlCommand(sSQL);
            DataSet resultSet;
            resultSet = RunQuery(sqlQuery);
            if (resultSet.Tables.Count > 0)
            {
                return resultSet.Tables[0];
            }
            else
            {
                return null;
            }
        }

        #region Populate
        public void PopulateYears(TreeNode node, DataTable dtuFilters)
        {
            if ((bool)dtuFilters.Rows[(int)Global.egOpsFilters.EnableFilteringOverall][(int)Global.egAdvFilterProps.Enabled] &&
                (bool)dtuFilters.Rows[(int)Global.egOpsFilters.NumOccup][(int)Global.egAdvFilterProps.Enabled])
            {
                string sSQL_Subs1 = scSQL_SUBS1;
                if ((bool)dtuFilters.Rows[(int)Global.egOpsFilters.SpecialOps][(int)Global.egAdvFilterProps.Enabled])
                {
                    sSQL_Subs1 += " " + scSQL_SpecialOps;
                }
                sSQL = "SELECT DISTINCT DATEPART(year, OPERATIONS_3.DBegin) AS year " +
                        sSQL_Subs1 + " WHERE " + sSQL_WHERE + scSQL_SUBS2 +
                                        ((decimal)dtuFilters.Rows[(int)Global.egOpsFilters.NumOccup][(int)Global.egAdvFilterProps.LowLimit]).ToString() + " AND " +
                                        ((decimal)dtuFilters.Rows[(int)Global.egOpsFilters.NumOccup][(int)Global.egAdvFilterProps.HighLimit]).ToString() +
                                   scSQL_SUBS3;
            }
            else
            {
                sSQL = "SELECT DISTINCT DATEPART(year, OPERATIONS_3.DBegin) AS year " +
                        "FROM OPDETAILS AS OPDETAILS_3 INNER JOIN " +
                         "OPERATIONS AS OPERATIONS_3 ON OPDETAILS_3.iOperation = OPERATIONS_3.ID " +
                    "WHERE " +
                    "(OPDETAILS_3.ID IN " + sSubQuery + "))";
            }
            sSQL += " ORDER BY year";
            SqlCommand sqlQuery = new SqlCommand(sSQL);
            DataSet resultSet = RunQuery(sqlQuery);
            if (resultSet.Tables.Count > 0)
            {
                foreach (DataRow row in resultSet.Tables[0].Rows)
                {
                    string sy = row["year"].ToString();
                    TreeNode NewNode = new TreeNode("y " + sy + sNodeStats(eWDatePart.Year, sy, "", ""), sy);
                    NewNode.PopulateOnDemand = true;
                    NewNode.SelectAction = TreeNodeSelectAction.Select;
                    NewNode.Expanded = false;
                    node.ChildNodes.Add(NewNode);
                }
            }
        }

        public void PopulateMonths(TreeNode node, DataTable dtu)
        {
            //ActivityLog.oDiag("Debug", "PopulateTrVwFilters.PopulateMonths Entry; sSQL_WHERE=`" + sSQL_WHERE + "`");
            if ((bool)dtu.Rows[(int)Global.egOpsFilters.EnableFilteringOverall][(int)Global.egAdvFilterProps.Enabled] &&
                (bool)dtu.Rows[(int)Global.egOpsFilters.NumOccup][(int)Global.egAdvFilterProps.Enabled])
            {
                string sSQL_Subs1 = scSQL_SUBS1;
                if ((bool)dtu.Rows[(int)Global.egOpsFilters.SpecialOps][(int)Global.egAdvFilterProps.Enabled])
                {
                    sSQL_Subs1 += " " + scSQL_SpecialOps;
                }
                sSQL = "SELECT DISTINCT DATEPART(month,OPERATIONS_3.DBegin) AS month " +
                                            sSQL_Subs1 + " WHERE " + sSQL_WHERE + " AND DATEPART(year,OPERATIONS_3.DBegin) = @Year" + scSQL_SUBS2 +
                                        ((decimal)dtu.Rows[(int)Global.egOpsFilters.NumOccup][(int)Global.egAdvFilterProps.LowLimit]).ToString() + " AND " +
                                        ((decimal)dtu.Rows[(int)Global.egOpsFilters.NumOccup][(int)Global.egAdvFilterProps.HighLimit]).ToString() +
                                   scSQL_SUBS3;
                sSQL += " ORDER BY month";
            }
            else
            {
                sSQL = "SELECT DISTINCT DATEPART(month,OPERATIONS_3.DBegin) AS month " +
                    "FROM OPDETAILS AS OPDETAILS_3 INNER JOIN " +
                         "OPERATIONS AS OPERATIONS_3 ON OPDETAILS_3.iOperation = OPERATIONS_3.ID " +
                    "WHERE " +
                    "(OPDETAILS_3.ID IN " + sSubQuery + ")";
                sSQL += " AND DATEPART(year,DBegin) = @Year" + ")";
                sSQL += " ORDER BY month";
            }
            SqlCommand sqlQuery = new SqlCommand(sSQL);
            sqlQuery.Parameters.Add("@Year", SqlDbType.Int).Value = node.Value;
            DataSet resultSet = RunQuery(sqlQuery);
            if (resultSet.Tables.Count > 0)
            {
                foreach (DataRow row in resultSet.Tables[0].Rows)
                {
                    int m = (int)row["month"];
                    string sm = m.ToString();
                    TreeNode NewNode = new TreeNode(saMonths[m - 1] + sNodeStats(eWDatePart.Month, node.Value, sm, ""), sm);
                    NewNode.PopulateOnDemand = true;
                    NewNode.SelectAction = TreeNodeSelectAction.Select;
                    NewNode.Expanded = false;
                    node.ChildNodes.Add(NewNode);
                }
            }
        }

        public void PopulateDays(TreeNode node, DataTable dtu)
        {
            if ((bool)dtu.Rows[(int)Global.egOpsFilters.EnableFilteringOverall][(int)Global.egAdvFilterProps.Enabled] &&
                (bool)dtu.Rows[(int)Global.egOpsFilters.NumOccup][(int)Global.egAdvFilterProps.Enabled])
            {
                string sSQL_Subs1 = scSQL_SUBS1;
                if ((bool)dtu.Rows[(int)Global.egOpsFilters.SpecialOps][(int)Global.egAdvFilterProps.Enabled])
                {
                    sSQL_Subs1 += " " + scSQL_SpecialOps;
                }
                sSQL = "SELECT DISTINCT DATEPART(day,OPERATIONS_3.DBegin) AS day " +
                         sSQL_Subs1 + " WHERE " + sSQL_WHERE + " AND DATEPART(year,OPERATIONS_3.DBegin) = @Year AND DATEPART(month,OPERATIONS_3.DBegin) = @Month" + scSQL_SUBS2 +
                                        ((decimal)dtu.Rows[(int)Global.egOpsFilters.NumOccup][(int)Global.egAdvFilterProps.LowLimit]).ToString() + " AND " +
                                        ((decimal)dtu.Rows[(int)Global.egOpsFilters.NumOccup][(int)Global.egAdvFilterProps.HighLimit]).ToString() +
                                   scSQL_SUBS3;
                sSQL += " ORDER BY day";
            }
            else
            {
                sSQL = "SELECT DISTINCT DATEPART(day,OPERATIONS_3.DBegin) AS day " +
                    "FROM OPDETAILS AS OPDETAILS_3 INNER JOIN " +
                         "OPERATIONS AS OPERATIONS_3 ON OPDETAILS_3.iOperation = OPERATIONS_3.ID " +
                    "WHERE " +
                    "(OPDETAILS_3.ID IN " + sSubQuery + ")";
                sSQL += " AND DATEPART(year,DBegin) = @Year AND DATEPART(month,DBegin) = @Month" + ")";
                sSQL += " ORDER BY day";
            }
            SqlCommand sqlQuery = new SqlCommand(sSQL);
            sqlQuery.Parameters.Add("@Year", SqlDbType.Int).Value = node.Parent.Value;
            sqlQuery.Parameters.Add("@Month", SqlDbType.Int).Value = node.Value;
            DataSet resultSet = RunQuery(sqlQuery);
            if (resultSet.Tables.Count > 0)
            {
                foreach (DataRow row in resultSet.Tables[0].Rows)
                {
                    string sd = row["day"].ToString();
                    TreeNode NewNode = new TreeNode("day " + sd + sNodeStats(eWDatePart.Day, node.Parent.Value, node.Value, sd), sd);
                    NewNode.PopulateOnDemand = true;
                    NewNode.SelectAction = TreeNodeSelectAction.Select;
                    NewNode.Expanded = false;
                    node.ChildNodes.Add(NewNode);
                }
            }
        }

        private string sNodeStats(eWDatePart euWDP, string suYear, string suMonth, string suDay)
        {
            string sFilt = "";
            DataTable dt;
            switch (euWDP)
            {
                case eWDatePart.Year:
                    sFilt = "AND (DATEPART(year, OPERATIONS_3.DBegin) = " + suYear + ")";
                    break;
                case eWDatePart.Month:
                    sFilt = "AND (DATEPART(year, OPERATIONS_3.DBegin) = " + suYear + ") AND (DATEPART(month, OPERATIONS_3.DBegin) = " + suMonth + ")";
                    break;
                case eWDatePart.Day:
                    sFilt = "AND (DATEPART(year, OPERATIONS_3.DBegin) = " + suYear + ") AND (DATEPART(month, OPERATIONS_3.DBegin) = " + suMonth +
                        ") AND (DATEPART(day, OPERATIONS_3.DBegin) = " + suDay + ")";
                    break;
            }
            dt = dtGlAggregate(dtLFilter, sFilt);
            if (dt != null)
            {
                int iNumFl = (int)dt.Rows[0]["NumFl"];
                decimal dMinutes = 0.0M;
                if (iNumFl > 0)
                {
                    dMinutes = (int)dt.Rows[0]["TotalTime"];
                }
                return " - " + iNumFl.ToString() + " Flts, " + ((dMinutes) / 60.0M).ToString("F2") + " Hrs, " + ((iNumFl > 0) ? (dMinutes / iNumFl).ToString("F0") + " min/Flt" : "");
            }
            return " - 0 Flights";
        }

        public void PopulateOperations(TreeNode node, bool buWithOpId, DataTable dtu)
        {
            if ((bool)dtu.Rows[(int)Global.egOpsFilters.EnableFilteringOverall][(int)Global.egAdvFilterProps.Enabled] &&
                (bool)dtu.Rows[(int)Global.egOpsFilters.NumOccup][(int)Global.egAdvFilterProps.Enabled])
            {
                sSQL = "SELECT DISTINCT OPERATIONS.ID, LAUNCHMETHODS.sLaunchMethod, LOCATIONS.sLocation AS LocTakeoff, OPERATIONS.DBegin, " +
                        "LOCATIONS_1.sLocation AS LocLand, OPERATIONS.DEnd, DateDiff(minute,OPERATIONS.DBegin,OPERATIONS.DEnd) AS Duration, " +
                        "OPERATIONS.sComment, CHARGECODES.sChargeCode " +
                        "FROM CHARGECODES INNER JOIN " +
                             "OPERATIONS ON CHARGECODES.ID = OPERATIONS.iChargeCode INNER JOIN " +
                             "LAUNCHMETHODS ON OPERATIONS.iLaunchMethod = LAUNCHMETHODS.ID INNER JOIN " +
                             "LOCATIONS ON OPERATIONS.iTakeoffLoc = LOCATIONS.ID INNER JOIN " +
                             "LOCATIONS AS LOCATIONS_1 ON OPERATIONS.iLandingLoc = LOCATIONS_1.ID  INNER JOIN " +
                             "OPDETAILS AS OPDETAILS_3 ON OPERATIONS.ID = OPDETAILS_3.iOperation " +
                    "WHERE " +
                    "(OPDETAILS_3.ID IN " +
                         "(SELECT ID " +
                               "FROM " +
                               "(SELECT OPDETAILS_2.ID, COUNT(AVIATORS_2.ID) AS AVCount " +
                                      "FROM OPDETAILS AS OPDETAILS_2 INNER JOIN " +
                                          "AVIATORS AS AVIATORS_2 ON OPDETAILS_2.ID = AVIATORS_2.iOpDetail " +
                                      "WHERE " +
                                      "(OPDETAILS_2.ID IN " +
                                          "(SELECT OPDETAILS_1.ID " +
                                            scSQL_FROM +
                                            (((bool)dtu.Rows[(int)Global.egOpsFilters.SpecialOps][(int)Global.egAdvFilterProps.Enabled]) ? " " + scSQL_SpecialOps : "") +
                                           " WHERE " + sSQL_WHERE +
                             " AND CONVERT(date, OPERATIONS.DBegin) = DATEFROMPARTS(" +
                                        node.Parent.Parent.Value + "," + node.Parent.Value + "," + node.Value + ")" +
                                    scSQL_SUBS2 +
                                        ((decimal)dtu.Rows[(int)Global.egOpsFilters.NumOccup][(int)Global.egAdvFilterProps.LowLimit]).ToString() + " AND " +
                                        ((decimal)dtu.Rows[(int)Global.egOpsFilters.NumOccup][(int)Global.egAdvFilterProps.HighLimit]).ToString() +
                                   scSQL_SUBS3;
                sSQL += " ORDER BY OPERATIONS.DBegin";
            }
            else
            {
                sSQL = "SELECT DISTINCT OPERATIONS.ID, LAUNCHMETHODS.sLaunchMethod, LOCATIONS.sLocation AS LocTakeoff, OPERATIONS.DBegin, " +
                        "LOCATIONS_1.sLocation AS LocLand, OPERATIONS.DEnd, DateDiff(minute,OPERATIONS.DBegin,OPERATIONS.DEnd) AS Duration, " +
                        "OPERATIONS.sComment, CHARGECODES.sChargeCode " +
                        "FROM CHARGECODES INNER JOIN " +
                             "OPERATIONS ON CHARGECODES.ID = OPERATIONS.iChargeCode INNER JOIN " +
                             "LAUNCHMETHODS ON OPERATIONS.iLaunchMethod = LAUNCHMETHODS.ID INNER JOIN " +
                             "LOCATIONS ON OPERATIONS.iTakeoffLoc = LOCATIONS.ID INNER JOIN " +
                             "LOCATIONS AS LOCATIONS_1 ON OPERATIONS.iLandingLoc = LOCATIONS_1.ID " +
                             "INNER JOIN OPDETAILS ON OPERATIONS.ID = OPDETAILS.iOperation " +
                             "WHERE OPDETAILS.ID IN " + sSubQuery;

                sSQL += " AND CONVERT(date, OPERATIONS.DBegin) = DATEFROMPARTS(" +
                    node.Parent.Parent.Value + "," + node.Parent.Value + "," + node.Value + "))";
                sSQL += " ORDER BY OPERATIONS.DBegin";
            }
            SqlCommand sqlQuery = new SqlCommand(sSQL);
            DataSet resultSet;
            resultSet = RunQuery(sqlQuery);
            if (resultSet.Tables.Count > 0)
            {
                foreach (DataRow row in resultSet.Tables[0].Rows)
                {
                    string sID = row["ID"].ToString();
                    TreeNode NewNode = new TreeNode(row["sLaunchMethod"] + " T " + row["LocTakeoff"] + " " +
                        ((DateTime)row["DBegin"]).ToString("yyyy/MM/dd HH:mm") +
                        ", L " + row["LocLand"] + " " + ((DateTime)row["DEnd"]).ToString("yyyy/MM/dd HH:mm") +
                        ", D=" + row["Duration"].ToString() + ", " + row["sComment"] + ", CC " + row["sChargeCode"], sID);
                    if (buWithOpId)
                    {
                        NewNode.Text = "Op " + sID + ": " + NewNode.Text;
                    }
                    NewNode.PopulateOnDemand = true;
                    NewNode.SelectAction = TreeNodeSelectAction.Select;
                    NewNode.Expanded = false;
                    node.ChildNodes.Add(NewNode);
                }
            }
        }

        public void PopulateSpecialOpsInfo(TreeNode node)
        {
            SqlCommand sqlQuery = new SqlCommand();
            sqlQuery.CommandText = "SELECT SO.ID, ST.sSpecialOpType, SO.sDescription, SO.iDurationMinutes " +
                "FROM SPECIALOPTYPES ST INNER JOIN " +
                     "SPECIALOPS SO ON ST.ID = SO.iSpecialOpType " +
                "WHERE SO.iOperation = @OperationID";
            sqlQuery.Parameters.Add("@OperationID", SqlDbType.Int).Value = node.Value;
            DataSet resultSet = RunQuery(sqlQuery);
            if (resultSet.Tables.Count > 0)
            {
                foreach (DataRow row in resultSet.Tables[0].Rows)
                {
                    TreeNode NewNode = new TreeNode("Special Operations - " + row["sSpecialOpType"].ToString() + ": " + row["sDescription"].ToString() +
                        ", Duration " + row["iDurationMinutes"] + " minutes", row["ID"].ToString());
                    NewNode.PopulateOnDemand = true;
                    NewNode.SelectAction = TreeNodeSelectAction.Select;
                    NewNode.Expanded = false;
                    node.ChildNodes.Add(NewNode);
                }
            }
        }

        public void PopulateOpDetails(TreeNode node)
        {
            SqlCommand sqlQuery = new SqlCommand();
            sqlQuery.CommandText = "SELECT OD.ID, EQR.sEquipmentRole, EQ.sShortEquipName, OD.dMaxAltitude, OD.dReleaseAltitude " +
                "FROM EQUIPMENT EQ INNER JOIN " +
                     "OPDETAILS OD ON EQ.ID = OD.iEquip INNER JOIN " +
                     "EQUIPMENTROLES EQR ON OD.iEquipmentRole = EQR.ID " +
                "WHERE iOperation = @OperationID";
            sqlQuery.Parameters.Add("@OperationID", SqlDbType.Int).Value = node.Value;
            DataSet resultSet = RunQuery(sqlQuery);
            if (resultSet.Tables.Count > 0)
            {
                foreach (DataRow row in resultSet.Tables[0].Rows)
                {
                    string sMaxAlt = row["dMaxAltitude"].ToString();
                    sMaxAlt = (sMaxAlt.Length < 1) ? "?" : sMaxAlt;
                    TreeNode NewNode = new TreeNode(row["sEquipmentRole"].ToString() + ": " + row["sShortEquipName"].ToString() +
                        ", Rel Alt " + row["dReleaseAltitude"] + " ft MSL, Max Alt " + sMaxAlt, row["ID"].ToString());
                    NewNode.PopulateOnDemand = true;
                    NewNode.SelectAction = TreeNodeSelectAction.Select;
                    NewNode.Expanded = false;
                    node.ChildNodes.Add(NewNode);
                }
            }
        }

        public void PopulateAviators(TreeNode node)
        {
            SqlCommand sqlQuery = new SqlCommand();
            sqlQuery.CommandText = "SELECT AVIATORS.ID AS ID_Av, AVIATORROLES.sAviatorRole, PEOPLE.sDisplayName, " +
                    "AVIATORS.dPercentCharge, AVIATORS.b1stFlight " +
                "FROM AVIATORROLES INNER JOIN " +
                     "AVIATORS ON AVIATORROLES.ID = AVIATORS.iAviatorRole INNER JOIN " +
                     "PEOPLE ON AVIATORS.iPerson = PEOPLE.ID " +
                "WHERE AVIATORS.iOpDetail = @OpDetailID";
            sqlQuery.Parameters.Add("@OpDetailID", SqlDbType.Int).Value = node.Value;
            DataSet resultSet = RunQuery(sqlQuery);
            if (resultSet.Tables.Count > 0)
            {
                foreach (DataRow row in resultSet.Tables[0].Rows)
                {
                    TreeNode NewNode = new TreeNode(row["sAviatorRole"].ToString() + ": " + row["sDisplayName"].ToString() +
                            ", % Charge = " + row["dPercentCharge"].ToString() +
                            (((bool)row["b1stFlight"]) ? ", First flight of season with instructor" : ""),
                        row["ID_Av"].ToString());
                    NewNode.PopulateOnDemand = false;
                    NewNode.SelectAction = TreeNodeSelectAction.Select;
                    node.ChildNodes.Add(NewNode);
                }
            }
        }

        private DataSet RunQuery(SqlCommand sqlQuery)
        {
            using (SqlConnection SqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlConn"].ToString()))
            {
                SqlDataAdapter dbAdapter = new SqlDataAdapter();
                dbAdapter.SelectCommand = sqlQuery;
                sqlQuery.Connection = SqlConn;
                DataSet resultsDataSet = new DataSet();
                try
                {
                    dbAdapter.Fill(resultsDataSet);
                }
                catch (Exception exc)
                {
                    throw new Global.excToPopup("Statistician.cs.RunQuery: " + exc.Message);
                }
                return resultsDataSet;
            }
        }
        #endregion

        public void trv_OpsClosestInTime(TreeView trv_Ops)
        {
            // Find the operation that is closest in time to right now
            trv_Ops.CollapseAll();
            trv_Ops.Nodes[0].Expand();
            StatistDailyFlightLogDataContext dc = new StatistDailyFlightLogDataContext();
            int iOper = (from o in dc.OPERATIONs where o.ID == dc.iTNPF_ClosestOpInTime() select o.ID).First();

            DateTime DT = (DateTime)(from o in dc.OPERATIONs where o.ID == iOper select o.DBegin).First();
            int iCurrDP = DT.Year;
            int iTest;

            string sValuePath = "";
            TreeNode tnc;
            if (trv_Ops.Nodes[0].ChildNodes.Count > 0) // Nodes[0] is the root node
            {
                //Years
                foreach (TreeNode tn in trv_Ops.Nodes[0].ChildNodes)
                {
                    iTest = Int32.Parse(tn.Value);
                    if (iTest == iCurrDP)
                    {
                        sValuePath = tn.ValuePath;
                    }
                }
                tnc = trv_Ops.FindNode(sValuePath);
                tnc.Expand();
                // Months
                iCurrDP = DT.Month;
                if (tnc.ChildNodes.Count > 0)
                {
                    foreach (TreeNode tn in tnc.ChildNodes)
                    {
                        iTest = Int32.Parse(tn.Value);
                        if (iTest == iCurrDP)
                        {
                            sValuePath = tn.ValuePath;
                        }
                    }
                    tnc = trv_Ops.FindNode(sValuePath);
                    tnc.Expand();
                    // Days
                    iCurrDP = DT.Day;
                    if (tnc.ChildNodes.Count > 0)
                    {
                        foreach (TreeNode tn in tnc.ChildNodes)
                        {
                            iTest = Int32.Parse(tn.Value);
                            if (iTest == iCurrDP)
                            {
                                sValuePath = tn.ValuePath;
                            }
                        }
                        tnc = trv_Ops.FindNode(sValuePath);
                        tnc.Expand();
                    }
                }
                tnc.Select();
            }
        }
    }
}
