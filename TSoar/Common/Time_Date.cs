using System;
using System.Web.UI.WebControls;
using TSoar.DB;

namespace TSoar
{
    public static class Time_Date
    {
        public static DateTimeOffset DNthWeekdayOfMonth(int iuYear, int iuMonth, int iuNth, DayOfWeek day_of_week)
        {
            // Determines a DateTimeOffset point in time for the iuNth occurrence of weekday day_of_week in year iuYear and month iuMonth.
            //     The time portion is set to 1 hour and 1 minute after midnight, and the UTC offset is taken from Setting "TimeZoneOffset" in table SETTINGS.

            // In the enumeration DayOfWeek, Sunday has the value 0, and Saturday is 6.

            SCUD_Multi mCrud = new SCUD_Multi();

            if (iuYear < 1 || iuYear > 9999)
            {
                throw new Exception("In Time_Date.DNthWeekdayOfMonth: " + iuYear.ToString() + " is an invalid year value.");
            }

            if (iuMonth < 1 || iuMonth > 12)
            {
                throw new Exception("In Time_Date.DNthWeekdayOfMonth: " + iuMonth.ToString() + " is an invalid month value.");
            }

            if (iuNth < 1 || iuNth > 5)
            {
                throw new Exception("In class Time_Date.DNthWeekdayOfMonth: " + iuNth.ToString() + " is an invalid nth value.");
            }

            // start from the first day of the month
            string sOffset = mCrud.GetSetting("TimeZoneOffset");
            int iOffsetHrs = Int32.Parse(sOffset.Substring(0, 3));
            int iOffsetMin = Int32.Parse(sOffset.Substring(4, 2));
            DateTimeOffset dt1 = new DateTimeOffset(iuYear, iuMonth, 1, 1, 1, 0, new TimeSpan(iOffsetHrs, iOffsetMin, 0));
            DateTimeOffset dt = dt1;
            // loop until we find our first match day of the week
            while (dt.DayOfWeek != day_of_week)
            {
                dt = dt.AddDays(1);
            }

            // Complete the gap to the nth week
            dt = dt.AddDays((iuNth - 1) * 7);

            if (dt.Month != iuMonth)
            {
                // If we skip to the next month, we throw an exception
                throw new Global.excToPopup(string.Format("Month " + dt1.Year.ToString() + "/" + dt1.Month.ToString() +
                    " does not have a " + iuNth.ToString() + "-th " + ((DayOfWeek)day_of_week)).ToString());
            }

            return dt;
        }

        public static DateTimeOffset DTO_from3TextBoxes(GridViewRow ugvRow, string suAnnot,
            string sutxbDate, string sutxbTime, string sutxbOffset)
            // Construct a DateTimeOffset value from three textboxes found in a GridView row.
            // suAnnot is only needed for annotating any thrown exceptions.
        {
            DateTime Date1 = DateTime.MinValue;
            string sD = ((TextBox)ugvRow.FindControl(sutxbDate)).Text.Trim();
            if (sD.Length < 1)
            {
                throw new Global.excToPopup("The " + suAnnot + " date cannot be empty.");
            }
            if (!DateTime.TryParse(sD, out Date1))
            {
                throw new Global.excToPopup("The " + suAnnot + " date `" + sD + "` is not valid.");
            }

            DateTime Time1 = DateTime.MinValue;
            sD = ((TextBox)ugvRow.FindControl(sutxbTime)).Text.Trim();
            if (sD.Length < 1)
            {
                throw new Global.excToPopup("The " + suAnnot + " time cannot be empty.");
            }
            if (!DateTime.TryParse(sD, out Time1))
            {
                throw new Global.excToPopup("The " + suAnnot + " time `" + sD + "` is not valid.");
            }

            string sOffset = ((TextBox)ugvRow.FindControl(sutxbOffset)).Text.Trim();
            if (sOffset.Length < 1)
            {
                throw new Global.excToPopup("The " + suAnnot + " offset cannot be empty.");
            }
            int iOffsetHrs = Int32.Parse(sOffset.Substring(0, 3));
            int iOffsetMin = Int32.Parse(sOffset.Substring(4, 2));
            decimal dOffset = iOffsetHrs + Math.Sign(iOffsetHrs) * iOffsetMin / 60.0M;
            if (Math.Abs(dOffset) > 14.0M)
            {
                throw new Global.excToPopup("The " + suAnnot + " Offset is limited to +/- 14 hours.");
            }
            return new DateTimeOffset(Date1.Year, Date1.Month, Date1.Day, Time1.Hour, Time1.Minute, Time1.Second,
                new TimeSpan(iOffsetHrs, iOffsetMin * Math.Sign(iOffsetHrs), 0));
        }
    }
}