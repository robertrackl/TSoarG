using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TSoar.DB;

namespace TSoar.Accounting.AdminFin
{
    public partial class ChrtOActs : System.Web.UI.Page
    {
        public class FlatAccount
        {
            public string sCode;
            public string sSortCode;
            public string sName;
            public string sAccountType;
            public int? ID;
            public int? iSF_ParentAcct;
            public string sNotes;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                List<FlatAccount> lstFlAc = new List<FlatAccount>();
                sf_AccountingDataContext dc = new sf_AccountingDataContext();
                var q = from p in dc.sf_AccountsFlatList() orderby p.sSortCode select new { p.sCode, p.sSortCode, p.sName, p.sAccountType, p.ID, p.iSF_ParentAcct, p.sNotes };
                foreach (var row in q)
                {
                    FlatAccount flAc = new FlatAccount()
                    {
                        sCode = row.sCode,
                        sSortCode = row.sSortCode,
                        sName = row.sName,
                        sAccountType = row.sAccountType,
                        ID = row.ID,
                        iSF_ParentAcct = row.iSF_ParentAcct,
                        sNotes = row.sNotes
                    };
                    lstFlAc.Add(flAc);
                }
                BindTree(lstFlAc, null);
            }
        }

        private void BindTree(IEnumerable<FlatAccount> lstuFlAc, TreeNode parentNode)
        {
            var nodes = lstuFlAc.Where(x => parentNode == null ? x.iSF_ParentAcct == null : x.iSF_ParentAcct == int.Parse(parentNode.Value));
            foreach (var node in nodes)
            {
                TreeNode newNode = new TreeNode(node.sCode.ToString() + " " + node.sName + " [" + node.sAccountType +
                    ((node.sNotes == null) ? "" : ". ") + node.sNotes + "]", node.ID.ToString());
                if (parentNode == null)
                {
                    trv_Acc.Nodes.Add(newNode);
                }
                else
                {
                    parentNode.ChildNodes.Add(newNode);
                }
                BindTree(lstuFlAc, newNode);
            }
        }
    }
}