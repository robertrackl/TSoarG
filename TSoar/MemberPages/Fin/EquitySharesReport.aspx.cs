using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TSoar.MemberPages.Fin
{
    public partial class EquitySharesReport : System.Web.UI.Page
    {
        //Dictionary<char, string> dictDQ = new Dictionary<char, string>()
        //{
        //    {'P', "aPproximate" },
        //    {'X', "eXact" },
        //    {'B', "Before" },
        //    {'A', "After" },
        //    {'G', "educated Guess" },
        //    {'W', "Wild ass guess" }
        //};
        //Dictionary<char, string> dictTT = new Dictionary<char, string>()
        //{
        //    {'P', "Purchase" },
        //    {'S', "Sale" },
        //    {'D', "Donation" },
        //    {'R', "Reinstatement" }
        //};

        protected void Page_Load(object sender, EventArgs e)
        {
            //ClubMembership.ClubMembershipDataContext dc = new ClubMembership.ClubMembershipDataContext();
            //var q = from s in dc.TNPV_EquityShares
            //        select new
            //        {
            //            Member_Name = Server.HtmlDecode(s.sDisplayName),
            //            Date = s.DXaction.ToString().Substring(0, 4) + "/" + s.DXaction.ToString().Substring(5, 2) + "/" + s.DXaction.ToString().Substring(8, 2),
            //            Date_Quality = dictDQ[s.cDateQuality],
            //            Shares = s.dNumShares,
            //            Transaction_Type = dictTT[s.cXactType],
            //            Info_Source = Server.HtmlDecode(s.sInfoSource),
            //            Notes = Server.HtmlDecode(s.sComment),
            //            Sub_Count = s.SubCount,
            //            Running_SubTotal=s.runningSubtotal,
            //            s.Overall_Count,
            //            Running_Total = s.RunningTotal
            //        };
            //gvEquiShRpt.DataSource = q;
            //gvEquiShRpt.DataBind();
        }

        //protected void gvEquiShRpt_RowDataBound(object sender, GridViewRowEventArgs e)
        //{
        //    if (e.Row.RowType == DataControlRowType.DataRow)
        //    {
        //        e.Row.Cells[3].HorizontalAlign = HorizontalAlign.Right;
        //        e.Row.Cells[7].HorizontalAlign = HorizontalAlign.Center;
        //        e.Row.Cells[8].HorizontalAlign = HorizontalAlign.Right;
        //        e.Row.Cells[9].HorizontalAlign = HorizontalAlign.Center;
        //        e.Row.Cells[10].HorizontalAlign = HorizontalAlign.Right;
        //    }
        //}
    }
}