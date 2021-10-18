using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TSoar.DB;

namespace TSoar.Statistician
{
    public partial class TIRewards1Member : System.Web.UI.Page
    {
        private enum enumGVColumns { D_Earn, D_Expiry, D_Claim, i_ServicePts, b_Expired, c_ECCode, i_Cumul, i_Forwarded, i_C1yr, i_C1yrG, s_Comments }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                sf_AccountingDataContext dc = new sf_AccountingDataContext();
                DateTimeOffset D = DateTimeOffset.MinValue;
                var q0 = from r in dc.TNPF_EligibleRewardMembers(D, false) orderby r.sDisplayName select r;
                DDLMember.DataSource = q0;
                DDLMember.DataValueField = "ID";
                DDLMember.DataTextField = "sDisplayName";
                DDLMember.DataBind();
                if (string.IsNullOrWhiteSpace(Session["RewardsMonthsCutoff"] as string))
                {
                    Session["RewardsMonthsCutoff"] = "15";
                }
                txbMonths.Text = (string)Session["RewardsMonthsCutoff"];
            }
            DisplayGrid();
        }

        private void DisplayGrid()
        {
            sf_AccountingDataContext dc = new sf_AccountingDataContext();
            DateTimeOffset Dref = DateTimeOffset.Now;
            int iMonths = Int32.Parse(txbMonths.Text);
            DateTimeOffset DCutoff = Dref.AddMonths(-iMonths);
            var q0 = from r in dc.TNPF_MRewards2(DDLMember.SelectedItem.Text, true, Dref) where r.OrderByD > DCutoff orderby r.OrderByD select r;
            gvMRewards.DataSource = q0;
            gvMRewards.DataBind();
        }

        protected void DDLMember_SelectedIndexChanged(object sender, EventArgs e)
        {
            DisplayGrid();
        }

        protected void gvMRewards_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
            {
                e.Row.Cells[(int)enumGVColumns.D_Earn].ToolTip = "Date on which service points were earned";
                e.Row.Cells[(int)enumGVColumns.D_Expiry].ToolTip = "Date on which service points expire(d)";
                e.Row.Cells[(int)enumGVColumns.D_Claim].ToolTip = "Date on which service points were claimed";
                e.Row.Cells[(int)enumGVColumns.i_ServicePts].ToolTip = "Number of service points: positive for earned points, negative for claimed points";
                e.Row.Cells[(int)enumGVColumns.b_Expired].ToolTip = "X means eXpired; blank means not expired";
                e.Row.Cells[(int)enumGVColumns.c_ECCode].ToolTip =
                    " C = points claimed\n" +
                    " G = points claimed for gifting\n" +
                    " I = points for each flight instructed (usually in one day)\n" +
                    " M = manual entry(used only in unusual situations, i.e., hardly ever)\n" +
                    " R = Causes the values in columns C1yr (claimed in one year) and C1yrG (gift-claimed in one year) to be reset to zero" +
                    " S = extra points for signing up and then showing up\n" +
                    " T = points for each tow flown (usually in one day)\n" +
                    " X = an entry to deduct any expired and unclaimed points";
                e.Row.Cells[(int)enumGVColumns.i_Cumul].ToolTip = "Cumulative number of service points";
                e.Row.Cells[(int)enumGVColumns.i_Forwarded].ToolTip = "Cumulative number of service points forwarded to next season";
                e.Row.Cells[(int)enumGVColumns.i_C1yr].ToolTip = "Cumulative number of service points claimed in one calendar year";
                e.Row.Cells[(int)enumGVColumns.i_C1yrG].ToolTip = "Cumulative number of service points claimed for gifting in one calendar year";
            }

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.Cells[(int)enumGVColumns.i_ServicePts].HorizontalAlign = HorizontalAlign.Right;
                e.Row.Cells[(int)enumGVColumns.i_Cumul].HorizontalAlign = HorizontalAlign.Right;
                e.Row.Cells[(int)enumGVColumns.i_Forwarded].HorizontalAlign = HorizontalAlign.Right;
                e.Row.Cells[(int)enumGVColumns.i_C1yr].HorizontalAlign = HorizontalAlign.Right;
                e.Row.Cells[(int)enumGVColumns.i_C1yrG].HorizontalAlign = HorizontalAlign.Right; // (SCR 114)

                if (e.Row.Cells[(int)enumGVColumns.c_ECCode].Text == "C" || e.Row.Cells[(int)enumGVColumns.c_ECCode].Text == "G")
                {
                    e.Row.Cells[(int)enumGVColumns.D_Earn].Text = "";
                    e.Row.Cells[(int)enumGVColumns.D_Expiry].Text = "";
                }
                else
                {
                    e.Row.Cells[(int)enumGVColumns.D_Claim].Text = "";
                }

                if (e.Row.Cells[(int)enumGVColumns.b_Expired].Text == "True")
                {
                    e.Row.Cells[(int)enumGVColumns.b_Expired].Text = "X";
                }
                else
                {
                    e.Row.Cells[(int)enumGVColumns.b_Expired].Text = "";
                }
                e.Row.Cells[(int)enumGVColumns.b_Expired].HorizontalAlign = HorizontalAlign.Center;

                string sCode = "ERROR!";
                switch (e.Row.Cells[(int)enumGVColumns.c_ECCode].Text)
                {
                    case "C":
                        sCode = "C - Claim";
                        break;
                    case "G":
                        sCode = "G - Gift Claim";
                        break;
                    case "I":
                        sCode = "I - Instructor";
                        break;
                    case "M":
                        sCode = "M - Manual";
                        break;
                    case "R":
                        sCode = "R - Reset Claims";
                        break;
                    case "S":
                        sCode = "S - Showed Up";
                        break;
                    case "T":
                        sCode = "T - Towpilot";
                        break;
                    case "X":
                        sCode = "X - Apply Expired";
                        break;
                    default:
                        break;
                }
                e.Row.Cells[(int)enumGVColumns.c_ECCode].Text = sCode;
                e.Row.Cells[(int)enumGVColumns.c_ECCode].HorizontalAlign = HorizontalAlign.Left;
            }
        }

        protected void gvMRewards_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvMRewards.PageIndex = e.NewPageIndex;
            DisplayGrid();
        }

        protected void txbMonths_TextChanged(object sender, EventArgs e)
        {
            Session["RewardsMonthsCutoff"] = ((TextBox)sender).Text;
        }
    }
}