using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.DataVisualization.Charting;
using System.Data;
using System.Configuration;
using System.Data.SqlClient;

namespace TSoar.MemberPages.Fin
{
    public partial class MemberFinStat : System.Web.UI.Page
    {
        private enum enColIx { Date, Actual, ACumul, Min, MCumul, Diff, Q, Billed, BCumul}; // Column Index
        private DB.SCUD_Multi mCRUD = new DB.SCUD_Multi();
        private string sMName { get { return (string)ViewState["sMName"] ?? mCRUD.GetPeopleDisplayNamefromWebSiteUserName(HttpContext.Current.User.Identity.Name); }
                                set { ViewState["sMName"] = value; }
                              }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                lblMName.Text = sMName;
                DateTimeOffset DNow = DateTimeOffset.Now;
                txbDFrom.Text = DNow.Year.ToString() + "-01-01";
                txbDTo.Text = DNow.Year.ToString() + "-12-31";
                FillTableAndGraph();
            }
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
        }

        private void ProcessPopupException(Global.excToPopup excu)
        {
            ButtonsClear();
            lblPopupText.Text = excu.sExcMsg();
            MPE_Show(Global.enumButtons.OkOnly);
        }
        #endregion

        private void FillTableAndGraph()
        {
            DataTable dt = new DataTable();
            Filldt(dt);
            if (dt == null) return;
            Billdt(dt);
            DisplayInGrid(dt);
            DisplayInGraph(dt);
        }

        private void Filldt(DataTable dtu)
        {
            // Fills DataTable dtu with minimum and actual flying charges from table FLYINGCHARGES for the logged-in user
            string sTypeOfAmount = "M"; // minimum monthly charges
            string sMembCat = mCRUD.GetMembershipCategoryfromWebSiteUserName(HttpContext.Current.User.Identity.Name);
            if (sMembCat == null)
            {
                sTypeOfAmount = "T";
            }
            else
            {
                if (sMembCat.IndexOf("Tow Pilot") > -1 || sMembCat.IndexOf("Instructor") > -1 || sMembCat.IndexOf("Temporary") > -1)
                {
                    sTypeOfAmount = "T";
                }
            }
            int iID = mCRUD.GetPeopleIDfromWebSiteUserName(HttpContext.Current.User.Identity.Name);
            if (iID == 0)
            {
                ButtonsClear();
                lblPopupText.Text = "The Web Site User Name `" + HttpContext.Current.User.Identity.Name + "` does not correspond to any organization member.";
                MPE_Show(Global.enumButtons.OkOnly);
                dtu = null;
                return;
            }
            string sID = iID.ToString();
            DateTimeOffset DFrom = DateTimeOffset.Parse(txbDFrom.Text);
            DateTimeOffset DTo = DateTimeOffset.Parse(txbDTo.Text);
            string sDFrom = DFrom.ToString("yyyy/MM/dd");
            string sDTo = DTo.ToString("yyyy/MM/dd");
            string sSQL = "SELECT DateOfAmount, Actual, ACumul = SUM(Actual) OVER(ORDER BY DateOfAmount), " +
                        "Min, MCumul=SUM(Min) OVER(ORDER BY DateOfAmount), $0.0 AS Diff, $0.0 AS Q, $0.0 AS Billed, $0.0 AS BCumul " +
                   "FROM ( SELECT CONVERT(datetime2(3),'" + sDFrom + "') AS DateOfAmount, $0.0 AS Actual, $0.0 AS Min " +
                          "UNION " +
                          "SELECT CONVERT(datetime2(3),F.DateOfAmount), F.mAmount AS Actual, $0.0 AS Min " +
                                "FROM FLYINGCHARGES F INNER JOIN PEOPLE P ON F.iPerson = P.ID " +
                                "WHERE(P.sDisplayName = '" + sMName + "') AND (F.cTypeOfAmount = 'A') AND (F.DateOfAmount >= '" + sDFrom +
                                "') AND (F.DateOfAmount <= '" + sDTo + "') " +
                          "UNION " +
                          "SELECT TOP 100 PERCENT CONVERT(datetime2(3),F2.DateOfAmount), $0.0 AS Actual, F2.mAmount AS Min " +
                                "FROM FLYINGCHARGES AS F2 INNER JOIN PEOPLE P2 ON F2.iPerson = P2.ID " +
                                "WHERE(P2.sDisplayName = '" + sMName + "') AND (F2.cTypeOfAmount = '" + sTypeOfAmount + "') AND (F2.DateOfAmount >= '" + sDFrom +
                                "') AND (F2.DateOfAmount <= '" + sDTo + "') " +
                          "ORDER BY DateOfAmount" +
                        ") AS derivedtbl_1";

            using (SqlConnection SqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlConn"].ToString()))
            {
                using (SqlCommand cmd = new SqlCommand(sSQL))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = SqlConn;
                    cmd.CommandTimeout = 600;
                    SqlConn.Open();
                    using (SqlDataAdapter sda = new SqlDataAdapter())
                    {
                        sda.SelectCommand = cmd;
                        try
                        {
                            sda.Fill(dtu);
                        }
                        catch (Exception exc)
                        {
                            throw new Global.excToPopup(exc.Message);
                        }
                    }
                }
            }
        }

        private void Billdt(DataTable dtu)
        {
            // Determine the amounts to be billed based on actual and minimum flying charges.

            // Consolidate entries for the same month into a single entry
            bool bAgain = true;
            do
            {
                bAgain = false;
                DateTime Dprev = DateTime.MinValue;
                for (int i = 0; i < dtu.Rows.Count; i++)
                {
                    DateTime Di = (DateTime)dtu.Rows[i][(int)enColIx.Date];
                    if ((Dprev.Year == Di.Year) && (Dprev.Month == Di.Month))
                    {
                        // Add the actual and monthly minimum values together and store in the i-th row
                        dtu.Rows[i][(int)enColIx.Actual] = (decimal)dtu.Rows[i][(int)enColIx.Actual] + (decimal)dtu.Rows[i - 1][(int)enColIx.Actual];
                        dtu.Rows[i][(int)enColIx.Min] = (decimal)dtu.Rows[i][(int)enColIx.Min] + (decimal)dtu.Rows[i - 1][(int)enColIx.Min];
                        // Get rid of the (i-1)-th row
                        dtu.Rows.RemoveAt(i - 1);
                        bAgain = true;
                        break;
                    }
                    Dprev = Di;
                }
            } while (bAgain);

            // According to Appendix C (Accounting for Minimum Flying Charges - Theory) of "PSSA Accounting/Bookkeeping/Statistics Procedures":
            for (int i = 0; i < dtu.Rows.Count; i++)
            {
                dtu.Rows[i][(int)enColIx.Diff] = (decimal)dtu.Rows[i][(int)enColIx.ACumul] - (decimal)dtu.Rows[i][(int)enColIx.MCumul];
                dtu.Rows[i][(int)enColIx.BCumul] = (decimal)(((decimal)dtu.Rows[i][(int)enColIx.Diff] < 0.0M) ? dtu.Rows[i][(int)enColIx.MCumul] : dtu.Rows[i][(int)enColIx.ACumul]);
                dtu.Rows[i][(int)enColIx.Q] = (decimal)dtu.Rows[i][(int)enColIx.BCumul] -
                    ((i > 0) ? (decimal)dtu.Rows[i - 1][(int)enColIx.BCumul] : 0.0M) -
                    (decimal)dtu.Rows[i][(int)enColIx.Actual];
                dtu.Rows[i][(int)enColIx.Billed] = (decimal)dtu.Rows[i][(int)enColIx.Actual] + (decimal)dtu.Rows[i][(int)enColIx.Q];
            }
        }

        protected void DDLMember_PreRender(object sender, EventArgs e)
        {
            DB.TNPV_PeopleContactsDataContext dc = new DB.TNPV_PeopleContactsDataContext();
            var q = from p in dc.PEOPLEs select new { p.ID, sDisplayName = Server.HtmlDecode(p.sDisplayName) };
            DDLMember.DataSource = q;
            DDLMember.DataBind();
            SetDropDownByValue(DDLMember, Server.HtmlDecode(sMName));
            string[] roles = Roles.GetRolesForUser();
            if (roles.Contains(Global.sagRoles[(int)Global.engRoles.Admin])
                || roles.Contains(Global.sagRoles[(int)Global.engRoles.Accountant])
                || roles.Contains(Global.sagRoles[(int)Global.engRoles.Bookkeeper])
                || roles.Contains(Global.sagRoles[(int)Global.engRoles.Developer])
                || roles.Contains(Global.sagRoles[(int)Global.engRoles.Test_Engineer])
                || roles.Contains(Global.sagRoles[(int)Global.engRoles.Treasury]) )
            {
                DDLMember.Enabled = true;
            }
            else
            {
                DDLMember.Enabled = false;
            }
        }
        private void SetDropDownByValue(DropDownList ddl, string sText)
        {
            ddl.ClearSelection();
            foreach (ListItem li in ddl.Items)
            {
                if (li.Text == sText)
                {
                    li.Selected = true;
                    break;
                }
            }
            ddl.SelectedItem.Text = sText;
        }

        private void DisplayInGrid(DataTable dtu)
        {
            if (dtu.Rows.Count < 1)
            {
                lblEmptyData.Visible = true;
            }
            else
            {
                DataTable dtl = dtu.Copy();
                lblEmptyData.Visible = false;
                dtl.Columns[0].ColumnName = "Date";
                dtl.Columns[1].ColumnName = "Actual Charge";
                dtl.Columns[2].ColumnName = "Cumulative Actual Charge";
                dtl.Columns[3].ColumnName = "Required Minimum";
                dtl.Columns[4].ColumnName = "Cumulative Required Minimum";
                dtl.Columns[5].ColumnName = "Diff";
                dtl.Columns[6].ColumnName = "Q";
                dtl.Columns[7].ColumnName = "Amount Billed";
                dtl.Columns[8].ColumnName = "Cumulative Amount Billed";
                gvTFC.DataSource = dtl;
                gvTFC.DataBind();
            }
        }

        private void DisplayInGraph(DataTable dtu)
        {
            Chart1.DataSource = dtu;
            string sSer = "$ Charges for Actual Flying";
            Chart1.Series.Add(sSer);
            Chart1.Series[sSer].ChartType = SeriesChartType.Line;
            Chart1.Series[sSer].MarkerStyle = MarkerStyle.Triangle;
            Chart1.Series[sSer].XValueType = ChartValueType.DateTime;
            Chart1.Series[sSer].XValueMember = "DateOfAmount";
            Chart1.Series[sSer].YValueMembers = "ACumul";
            sSer = "$ Minimum Monthly Amount";
            Chart1.Series.Add(sSer);
            Chart1.Series[sSer].ChartType = SeriesChartType.StepLine;
            Chart1.Series[sSer].XValueType = ChartValueType.DateTime;
            Chart1.Series[sSer].XValueMember = "DateOfAmount";
            Chart1.Series[sSer].YValueMembers = "MCumul";
            sSer = "$ Billed Amount";
            Chart1.Series.Add(sSer);
            Chart1.Series[sSer].ChartType = SeriesChartType.Line;
            Chart1.Series[sSer].XValueType = ChartValueType.DateTime;
            Chart1.Series[sSer].XValueMember = "DateOfAmount";
            Chart1.Series[sSer].YValueMembers = "BCumul";
            Chart1.DataBind();
        }

        protected void pbUpdate_Click(object sender, EventArgs e)
        {
            sMName = DDLMember.SelectedItem.Text;
            FillTableAndGraph();
        }
    }
}