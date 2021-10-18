using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using TSoar.DB;

namespace TSoar.MemberPages.MRewards
{
    // SCR 223 -- lots of changes, not marked line by line
    public partial class MRewards : System.Web.UI.Page
    {
        private enum enumGVColumns { D_Earn, D_Expiry, D_Claim, i_ServicePts, b_Expired, c_ECCode, i_Cumul, i_Forwarded, i_C1yr, i_C1yrG, s_Comments}
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                try // (SCR 109)
                {
                    TNPV_PeopleContactsDataContext pdc = new TNPV_PeopleContactsDataContext();
                    var q0 = from p in pdc.PEOPLEs where p.sUserName == User.Identity.Name select p.sDisplayName;
                    lblUser.Text = q0.First();
                }
                catch (Exception exc)
                {
                    ProcessPopupException(new Global.excToPopup("Problem in MRewards.aspx.cs.Page_Load: `" + exc.Message +
                        "` - most likely because `" + User.Identity.Name + "` does not exist in PEOPLE.sUserName."));
                    return;
                }
                if (string.IsNullOrWhiteSpace(Session["RewardsMonthsCutoff"] as string))
                {
                    Session["RewardsMonthsCutoff"] = "15";
                }
                txbMonths.Text = (string)Session["RewardsMonthsCutoff"];
            }
            DisplayGrid();
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
            //Button btn = (Button)sender;
        }
        private void ProcessPopupException(Global.excToPopup excu)
        {
            ButtonsClear();
            lblPopupText.Text = excu.sExcMsg();
            MPE_Show(Global.enumButtons.OkOnly);
        }
        #endregion

        private void DisplayGrid()
        {
            gvMRewards.DataSource = null;
            sf_AccountingDataContext dc = new sf_AccountingDataContext();
            try
            {
                DateTimeOffset Dref = DateTimeOffset.Now;
                int iMonths = Int32.Parse(txbMonths.Text);
                DateTimeOffset DCutoff = Dref.AddMonths(-iMonths);
                var q0 = from r in dc.TNPF_MRewards2(User.Identity.Name, false, Dref) where r.OrderByD > DCutoff orderby r.OrderByD select r;
                gvMRewards.DataSource = q0;
            }
            catch (Exception exc)
            {
                ProcessPopupException(new Global.excToPopup("MemberPages/MRewards/MRewards.aspx.cs.DisplayGrid() Exception: `" + exc.Message + "`. User: " + User.Identity.Name));
            }
            finally
            {
                gvMRewards.DataBind();
            }
        }

        protected void gvMRewards_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
            {
                e.Row.Cells[(int)enumGVColumns.D_Earn].ToolTip = "Date on which service points were earned";
                e.Row.Cells[(int)enumGVColumns.D_Expiry].ToolTip = "Date on which service points expire";
                e.Row.Cells[(int)enumGVColumns.D_Claim].ToolTip = "Date on which service points were claimed";
                e.Row.Cells[(int)enumGVColumns.i_ServicePts].ToolTip = "Number of service points: positive for earned points, negative for claimed points";
                e.Row.Cells[(int)enumGVColumns.b_Expired].ToolTip = "X means eXpired; blank means not expired";
                e.Row.Cells[(int)enumGVColumns.c_ECCode].ToolTip =
                    " C = points claimed\n" + 
                    " I = points for each flight instructed (usually in one day)\n" + 
                    " M = manual entry(used only in unusual situations, i.e., hardly ever)\n" +
                    " R = Causes the values in columns C1yr (claimed in one year) and C1yrG (gift-claimed in one year) to be reset to zero" +
                    " S = extra points for signing up and then showing up\n" +
                    " T = points for each tow flown (usually in one day)\n" +
                    " X = an entry to deduct any expired and unclaimed points";
                e.Row.Cells[(int)enumGVColumns.i_Cumul].ToolTip = "Cumulative number of unexpired service points";
                e.Row.Cells[(int)enumGVColumns.i_Forwarded].ToolTip = "Cumulative number of service points forwarded to next season";
                e.Row.Cells[(int)enumGVColumns.i_C1yr].ToolTip = "Cumulative number of service points claimed in one calendar year";
                e.Row.Cells[(int)enumGVColumns.i_C1yrG].ToolTip = "Cumulative number of service points gift-claimed in one calendar year";
            }

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.Cells[(int)enumGVColumns.i_ServicePts].HorizontalAlign = HorizontalAlign.Right;
                e.Row.Cells[(int)enumGVColumns.i_Cumul].HorizontalAlign = HorizontalAlign.Right;
                e.Row.Cells[(int)enumGVColumns.i_Forwarded].HorizontalAlign = HorizontalAlign.Right;
                e.Row.Cells[(int)enumGVColumns.i_C1yr].HorizontalAlign = HorizontalAlign.Right;
                e.Row.Cells[(int)enumGVColumns.i_C1yrG].HorizontalAlign = HorizontalAlign.Right;

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
                    case "G":
                        sCode = "G - Gift Claim";
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