using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TSoar.DB;

namespace TSoar.MemberPages.Roster
{
    public partial class ClubRoster : System.Web.UI.Page
    {
        private enum enRoster { Telephone, Email, PhysAddr, Qualif};
        SCUD_Multi mCRUD = new SCUD_Multi();

        protected void Page_Load(object sender, EventArgs e)
        {
            DisplayInGrid();
        }

        private void DisplayInGrid()
        {
            DisplayInGrid(enRoster.Telephone);
            DisplayInGrid(enRoster.Email);
            DisplayInGrid(enRoster.PhysAddr);
            DisplayInGrid(enRoster.Qualif);
        }
        private void DisplayInGrid(enRoster enuRoster)
        {
            TNPV_PeopleContactsDataContext dc = new TNPV_PeopleContactsDataContext();
            string ss = "";

            switch (enuRoster) {
                case enRoster.Telephone:
                    ss = "PageSizeTelephRoster";
                    gvTelNumbers.PageSize = Int32.Parse(mCRUD.GetSetting(ss));
                    gvTelNumbers.DataSource = from r in dc.TNPV_TelephRosters orderby r.sDisplayName select r;
                    gvTelNumbers.DataBind();
                    break;
                case enRoster.Email:
                    ss = "PageSizeEmailRoster";
                    gvEmails.PageSize = Int32.Parse(mCRUD.GetSetting(ss));
                    gvEmails.DataSource = from r in dc.TNPV_EmailRosters orderby r.sDisplayName select r;
                    gvEmails.DataBind();
                    break;
                case enRoster.PhysAddr:
                    ss = "PageSizePhysicalAddressRoster";
                    gvPhysAddr.PageSize = Int32.Parse(mCRUD.GetSetting(ss));
                    gvPhysAddr.DataSource = from r in dc.TNPV_PhysAddrRosters orderby r.sDisplayName select r;
                    gvPhysAddr.DataBind();
                    break;
                case enRoster.Qualif:
                    ss = "PageSizeQualifRoster";
                    gvQualifs.PageSize = Int32.Parse(mCRUD.GetSetting(ss));
                    gvQualifs.DataSource = from r in dc.TNPV_QualifRosters orderby r.sDisplayName,r.Since select r;
                    gvQualifs.DataBind();
                    break;
            }
        }

        protected void gvTelNumbers_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvTelNumbers.PageIndex = e.NewPageIndex;
            DisplayInGrid(enRoster.Telephone);
        }

        protected void gvEmails_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvEmails.PageIndex = e.NewPageIndex;
            DisplayInGrid(enRoster.Email);
        }

        protected void gvPhysAddr_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvPhysAddr.PageIndex = e.NewPageIndex;
            DisplayInGrid(enRoster.PhysAddr);
        }

        protected void gvQualifs_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvQualifs.PageIndex = e.NewPageIndex;
            DisplayInGrid(enRoster.Qualif);
        }
    }
}