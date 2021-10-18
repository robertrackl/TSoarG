using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TSoar.Developer.SWLab
{
    public partial class TimeAndDate : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
            {
                txbOut.Text += Environment.NewLine;
            }
            else
            {
                lblDateTime.Text = DateTimeOffset.Now.ToString();
            }
        }

        protected void pbGo_Click(object sender, EventArgs e)
        {
            int iYear = Int32.Parse(txbYear.Text);
            int iMonth = Int32.Parse(txbMonth.Text);
            int Nth = Int32.Parse(txbNth.Text);
            int iDayOW = Int32.Parse(DDLweekday.SelectedValue);
            DateTimeOffset DD = DateTimeOffset.MinValue;
            try
            {
                DD = Time_Date.DNthWeekdayOfMonth(iYear, iMonth, Nth, (DayOfWeek)iDayOW);
            }catch (Exception exc)
            {
                txbOut.Text = exc.Message + Environment.NewLine;
            }
            txbOut.Text += DD.ToString();
        }
    }
}