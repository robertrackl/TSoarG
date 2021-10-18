using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TSoar.TestEngineer.TE_Equipment
{
    public partial class TE_Equipment : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string sMP = HttpContext.Current.Server.MapPath("~");
            string sLevel = File.ReadAllText(sMP + "/MigrationLevelDevIntProd.txt");
            divHRef.Visible = true;
            divLbl.Visible = false;
            if (sLevel.Length > 3)
            {
                if (sLevel.Substring(0, 4) == "Prod")
                {
                    divHRef.Visible = false;
                    divLbl.Visible = true;
                }
            }
        }
    }
}