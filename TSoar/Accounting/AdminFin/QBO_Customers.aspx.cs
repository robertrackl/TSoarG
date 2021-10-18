using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Intuit.Ipp.Core;
using Intuit.Ipp.Data;
using Intuit.Ipp.Security;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TSoar.Accounting.AdminFin
{
    public partial class QBO_Customers : System.Web.UI.Page
    {
        private DataTable dtcust = new DataTable();
        private bool bOK { get { return GetbOK(); } set { ViewState["bOK"] = value; } }
        private bool GetbOK()
        {
            if (ViewState["bOK"] == null)
            {
                return true;
            }else
            {
                return (bool)ViewState["bOK"];
            }
        }

        #region Modal Popup
        //======================
        private void ButtonsClear()
        {
            NoButton.CommandArgument = "";
            NoButton.CommandName = "";
            YesButton.CommandArgument = "";
            YesButton.CommandName = "";
            OkButton.CommandArgument = "";
            OkButton.CommandName = "";
            CancelButton.CommandArgument = "";
            CancelButton.CommandName = "";
        }
        private void MPE_Show(Global.enumButtons eubtns)
        {
            NoButton.CssClass = "displayNone";
            YesButton.CssClass = "displayNone";
            OkButton.CssClass = "displayNone";
            CancelButton.CssClass = "displayNone";
            switch (eubtns)
            {
                case Global.enumButtons.NoYes:
                    NoButton.CssClass = "displayUnset";
                    YesButton.CssClass = "displayUnset";
                    break;
                case Global.enumButtons.OkOnly:
                    OkButton.CssClass = "displayUnset";
                    break;
                case Global.enumButtons.OkCancel:
                    OkButton.CssClass = "displayUnset";
                    CancelButton.CssClass = "displayUnset";
                    break;
            }
            ModalPopExt.Show();
        }
        protected void Button_Click(object sender, EventArgs e)
        {
        }

        private void ProcessPopupException(Global.excToPopup excu)
        {
            ButtonsClear();
            lblPopupText.Text = excu.sExcMsg();
            MPE_Show(Global.enumButtons.OkOnly);
        }
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (bOK)
            {
                divConnect2QBO.Visible = false;
                gvQBOcust.Visible = true;
                try
                {
                    GetCustomers();
                    DisplayInGrid();
                }
                catch (Global.excToPopup exc)
                {
                    ProcessPopupException(exc);
                    bOK = false;
                }
            }
            else
            {
                gvQBOcust.Visible = false;
                divConnect2QBO.Visible = true;
            }
        }

        protected void gvQBOcust_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvQBOcust.PageIndex = e.NewPageIndex;
            DisplayInGrid();
        }

        private void GetCustomers()
        {
            if (Session["dictTokens"] == null)
            {
                Session["dictTokens"] = new Dictionary<string, string>();
            }
            Dictionary<string, string> dictTokens = (Dictionary<string, string>)Session["dictTokens"];
            if (!dictTokens.ContainsKey("accessToken"))
            {
                throw new Global.excToPopup("A session with QuickBooks Online has not been established via OAuth2.");
            }

            // Compose serviceContext
            OAuth2RequestValidator oauthValidator;
            try
            {
                oauthValidator = new OAuth2RequestValidator(dictTokens["accessToken"]);
            }
            catch (Exception exc)
            {
                throw new Global.excToPopup("Problem with OAuth2 connection to QuickBooks Online: " + exc.Message);
            }
            ServiceContext serviceContext = new ServiceContext(dictTokens["realmId"], IntuitServicesType.QBO, oauthValidator);
            serviceContext.IppConfiguration.MinorVersion.Qbo = "29";

            // Define table columns
            DataColumn[] dcaPF = new DataColumn[4];
            dcaPF[0] = new DataColumn("Id", Type.GetType("System.String"));
            dcaPF[1] = new DataColumn("DisplayName", Type.GetType("System.String"));
            dcaPF[2] = new DataColumn("Balance", Type.GetType("System.Decimal"));
            dcaPF[3] = new DataColumn("Active", Type.GetType("System.Boolean"));
            foreach (DataColumn col in dcaPF)
            {
                col.AllowDBNull = false;
                dtcust.Columns.Add(col);
            }

            // Get the list of customers and stuff them into a table
            List<Customer> liCust = Helper.FindAll<Customer>(serviceContext, new Customer(), 1, 500).Where(i => i.status != EntityStatusEnum.SyncError).ToList();
            foreach (Customer c in liCust)
            {
                DataRow dr = dtcust.NewRow();
                dr[0] = c.Id;
                dr[1] = c.DisplayName;
                dr[2] = c.Balance;
                dr[3] = c.Active;
                dtcust.Rows.Add(dr);
            }
        }

        private void DisplayInGrid()
        {
            gvQBOcust.DataSource = dtcust;
            gvQBOcust.DataBind();
        }
    }
}