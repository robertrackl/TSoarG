using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TSoar.MemberPages.Stats
{
    public partial class StatsReports : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void pbByPilot_Click(object sender, EventArgs e)
        {
            DisplayByPilot();
        }

        protected void gvByPilot_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvByPilot.PageIndex = e.NewPageIndex;
            DisplayByPilot();
        }

        private void DisplayByPilot()
        {
            TSoar.DB.StatistDataContext dc = new TSoar.DB.StatistDataContext();
            var q = (from r in dc.sp_StatsRptByPilot() select r).ToList();
            foreach(DB.sp_StatsRptByPilotResult r in q)
            {
                if (r.Role == null)
                {
                    r.SubGlider = r.Flight_Hours.ToString();
                    r.SubGlider = r.SubGlider.Remove(r.SubGlider.Length - 4);
                    r.Flight_Hours = null;
                    if (r.Glider == null)
                    {
                        r.SubYear = r.SubGlider;
                        r.SubGlider = null;
                        if (r.Year == null)
                        {
                            r.SubAviator = r.SubYear;
                            r.SubYear = null;
                            if (r.Aviator == null)
                            {
                                r.Total = r.SubAviator;
                                r.SubAviator = null;
                            }
                        }
                    }
                }
            }
            gvByPilot.DataSource = q;
            gvByPilot.DataBind();
        }

        protected void pbByAircraft_Click(object sender, EventArgs e)
        {
            DisplayByGlider();
        }

        protected void gvByGlider_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvByGlider.PageIndex = e.NewPageIndex;
            DisplayByGlider();
        }

        private void DisplayByGlider()
        {
            TSoar.DB.StatistDataContext dc = new TSoar.DB.StatistDataContext();
            var q = (from r in dc.sp_StatsRptByGlider() select r).ToList();
            foreach(DB.sp_StatsRptByGliderResult r in q)
            {
                if (r.Year == null)
                {
                    r.SubGlider = r.Flight_Hours.ToString();
                    r.SubGlider = r.SubGlider.Remove(r.SubGlider.Length - 4);
                    r.Flight_Hours = null;
                    if (r.Glider___Owner == null)
                    {
                        r.Total = r.SubGlider;
                        r.SubGlider = null;
                    }
                }
            }
            gvByGlider.DataSource = q;
            gvByGlider.DataBind();
        }
    }
}