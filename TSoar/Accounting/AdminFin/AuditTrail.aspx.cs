using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TSoar.DB;

namespace TSoar.Accounting
{
    public partial class AuditTrail : System.Web.UI.Page
    {
        SCUD_Multi mCRUD = new SCUD_Multi();
        protected void Page_Load(object sender, EventArgs e)
        {
            DisplayInGrid(Global.enugInfoType.SF_AuditTrail);
        }

        private void DisplayInGrid(Global.enugInfoType euInfoType)
        {
            GridView g = null;
            string ss = "";
            switch (euInfoType)
            {
                case Global.enugInfoType.SF_AuditTrail:
                    g = gvAuditTrail;
                    ss = "PageSizeAuditTrail";
                    break;
                default:
                    return;
            }
            g.PageSize = Int32.Parse(mCRUD.GetSetting(ss));
            DataView dv = mCRUD.GetAll(euInfoType).DefaultView;
            dv.Sort = "DTimeStamp desc";
            g.DataSource = dv;
            g.DataBind();
        }

        protected void gvAuditTrail_RowCreated(object sender, GridViewRowEventArgs e)
        {
            foreach (TableCell cell in e.Row.Cells) { cell.CssClass = "cell-padding"; }
        }

        protected void gvAuditTrail_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvAuditTrail.PageIndex = e.NewPageIndex;
            DisplayInGrid(Global.enugInfoType.SF_AuditTrail);
        }
    }
}