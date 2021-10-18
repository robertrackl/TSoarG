using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TSoar.ErrorPages
{
    public partial class Resp2Err : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            DateTimeOffset dto = new DateTimeOffset();
            dto = DateTimeOffset.UtcNow;
            lblDate.Text = CustFmt.sFmtDate(dto,CustFmt.enDFmt.DateOnly);
            lblTime.Text = CustFmt.sFmtDate(dto,CustFmt.enDFmt.TimeOnlySec);
            if (Global.sgMigrationLevelDevIntProd == "Dev" || Global.sgMigrationLevelDevIntProd == "Int")
            {
                pnlOutText.Visible = true;
            }
        }
    }
}