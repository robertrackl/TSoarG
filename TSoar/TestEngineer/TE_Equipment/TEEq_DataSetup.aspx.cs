using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TSoar.TestEngineer.TE_Equipment
{
    public partial class TEEq_DataSetup : System.Web.UI.Page
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

        protected void pbCleanup_Click(object sender, EventArgs e)
        {
            TEEqDataContext dc = new TEEqDataContext();
            string sStatus = "";
            dc.TEEq_Cleanup(ref sStatus);
            lblCleanUpStat.Text = sStatus;
            Session["iSelectedOpsCal"] = -1;
            dc.Dispose();
        }

        protected void pbEquip_Click(object sender, EventArgs e)
        {
            TEEqDataContext dc = new TEEqDataContext();
            Button pb = (Button)sender;
            if (pb.ID == "pbAll")
            {
                EquipCall(dc, "pbOpsCal");
                EquipCall(dc, "pbEquip");
                EquipCall(dc, "pbCompon");
                EquipCall(dc, "pbPars");
                EquipCall(dc, "pbAgIt");
                EquipCall(dc, "pbOpDat");
            }
            else
            {
                EquipCall(dc, pb.ID);
            }
            dc.Dispose();
        }
        private void EquipCall(TEEqDataContext udc, string suPbId)
        {
            string sStatus = "";
            Label lbl = null;
            string sWhat = "";
            switch (suPbId)
            {
                case "pbOpsCal":
                    sWhat = "OpsCal";
                    lbl = lblOpsCalStat;
                    break;
                case "pbEquip":
                    sWhat = "Equipment";
                    lbl = lblEqStat;
                    break;
                case "pbCompon":
                    sWhat = "Components";
                    lbl = lblComponStat;
                    break;
                case "pbPars":
                    sWhat = "Parameters";
                    lbl = lblParsStat;
                    break;
                case "pbAgIt":
                    sWhat = "Aging Items";
                    lbl = lblAgItStat;
                    break;
                case "pbOpDat":
                    sWhat = "Oper Data";
                    lbl = lblOpDatStat;
                    break;
            }
            udc.TEEq_DataSetup(sWhat, ref sStatus);
            lbl.Text = sStatus;
        }
    }
}