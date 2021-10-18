using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TSoar.MemberPages.EquipMaintStat
{
    public partial class EqOperStatCompact : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            FillDataTable();
        }

        private void FillDataTable()
        {
            Equipment.EquipmentDataContext dc = new Equipment.EquipmentDataContext();
            var q0 = from ec in dc.EQUIPCOMPONENTs
                     where ec.bReportOperStatus // SCR 231
                     orderby ec.EQUIPMENT.sShortEquipName
                     select new { sPoEq = ec.EQUIPMENT.sShortEquipName, sReg = ec.EQUIPMENT.sRegistrationId,
                         ec.sComponent, ec.sOperStatus, ec.sComment }; // SCR 231
            gvEqOpStC.DataSource = q0;
            gvEqOpStC.DataBind();
        }

        protected void gvEqOpStC_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvEqOpStC.PageIndex = e.NewPageIndex;
            FillDataTable();
        }
    }
}