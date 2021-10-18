using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TSoar.DB;

namespace TSoar.Accounting
{
    public partial class Subledgers : System.Web.UI.Page
    {
        const int icSubledgerName = 2;
        const int icFingeringTable = 3;
        const int icFingeredDescrField = 4; // Descriptive field in the fingered table
        const int icNotes = 5;
        SCUD_Multi mCRUD = new SCUD_Multi();
        string[] sa = new string[6];
        private string sMKey { get { return (string)ViewState["sMKey"] ?? ""; } set { ViewState["sMKey"] = value; } }

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
            Button btn = (Button)sender;
            //Global.oLog("Modal popup dismissed with " + btn.ID + ", CommandName=" + btn.CommandName);
            try
            {
                if ((btn.ID == "YesButton") && (btn.CommandName == "Delete"))
                {
                    switch (OkButton.CommandArgument)
                    {
                        case "Subledger":
                            // Delete the Account
                            mCRUD.DeleteOne(Global.enugInfoType.SF_Subledgers, btn.CommandArgument);
                            DisplayInGrid(Global.enugInfoType.SF_Subledgers);
                            break;
                    }
                }
            }
            catch (Global.excToPopup exc)
            {
                ProcessPopupException(exc);
            }
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
            if (!IsPostBack)
            {
                DisplayInGrid(Global.enugInfoType.SF_Subledgers);
            }
        }

        private void DisplayInGrid(Global.enugInfoType euInfoType)
        {
            GridView g = null;
            string ss = "";
            switch (euInfoType)
            {
                case Global.enugInfoType.SF_Subledgers:
                    g = gvSubledger;
                    ss = "PageSizeSubledger";
                    break;
                default:
                    return;
            }
            g.PageSize = Int32.Parse(mCRUD.GetSetting(ss));
            g.DataSource = mCRUD.GetAll(euInfoType);
            g.DataBind();
        }

        protected void dvSubledgers_ItemInserted(object sender, DetailsViewInsertedEventArgs e)
        {
            Response.Redirect(Request.RawUrl);
            //DisplayInGrid(Global.enugInfoType.SF_Subledgers);
        }

        protected void dvSubledgers_ItemInserting(object sender, DetailsViewInsertEventArgs e)
        {
            Global.excToPopup exc = null;
            e.Cancel = true; // because we are handling our own insertion
            sa[0] = DateTime.UtcNow.ToString();
            sa[1] = mCRUD.GetPeopleIDfromWebSiteUserName(HttpContext.Current.User.Identity.Name).ToString();
            sa[icSubledgerName] = Server.HtmlEncode(((TextBox)dvSubledgers.FindControl("txbName")).Text.Trim().Replace("'", "`"));
            sa[icFingeringTable] = Server.HtmlEncode(((TextBox)dvSubledgers.FindControl("txbFingeringTable")).Text.Trim().Replace("'", "`"));
            sa[icFingeredDescrField] = Server.HtmlEncode(((TextBox)dvSubledgers.FindControl("txbFingeredDescr")).Text.Trim().Replace("'", "`"));
            sa[icNotes] = Server.HtmlEncode(((TextBox)dvSubledgers.FindControl("txbNotes")).Text.Trim().Replace("'", "`"));
            int iIdent = 0;
            mCRUD.InsertOne(Global.enugInfoType.SF_Subledgers, sa, out iIdent);
            string sVal = "Record Inserted: ";
            for (int i = 2; i < sa.Count(); i++)
            {
                sVal += ((i > 2) ? ", " : " ") + sa[i];
            }
            exc = new Global.excToPopup(sVal);
            ProcessPopupException(exc);
            DisplayInGrid(Global.enugInfoType.SF_Subledgers);
        }

        protected void gvSubledger_RowEditing(object sender, GridViewEditEventArgs e)
        {
            sMKey = ((Label)gvSubledger.Rows[e.NewEditIndex].FindControl("lblID")).Text;
            gvSubledger.EditIndex = e.NewEditIndex;
            DisplayInGrid(Global.enugInfoType.SF_Subledgers);
        }

        protected void gvSubledger_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            Label lblItem = (Label)gvSubledger.Rows[e.RowIndex].FindControl("lblSubledger");
            string sItem = lblItem.Text;
            ButtonsClear();
            YesButton.CommandName = "Delete";
            YesButton.CommandArgument = ((Label)gvSubledger.Rows[e.RowIndex].FindControl("lblID")).Text;
            OkButton.CommandArgument = "Subledger";
            lblPopupText.Text = "It is quite unusual to delete a subledger; they are usually kept on file for historical recordkeeping. " +
                "If you still want to go ahead, please confirm deletion of the subledger '" + sItem + "'";
            MPE_Show(Global.enumButtons.NoYes);
        }

        protected void gvSubledger_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            GridViewRow row = gvSubledger.Rows[e.RowIndex];
            TextBox txb;
            sa[0] = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.mmm");
            sa[1] = mCRUD.GetPeopleIDfromWebSiteUserName(HttpContext.Current.User.Identity.Name).ToString();
            txb = (TextBox)row.FindControl("txbUSubledger");
            sa[icSubledgerName] = Server.HtmlEncode(txb.Text.Trim().Replace("'", "`"));
            txb = (TextBox)row.FindControl("txbUFingeringTable");
            sa[icFingeringTable] = Server.HtmlEncode(txb.Text.Trim().Replace("'", "`"));
            txb = (TextBox)row.FindControl("txbUFingeredDescr");
            sa[icFingeredDescrField] = Server.HtmlEncode(txb.Text.Trim().Replace("'", "`"));
            txb = (TextBox)row.FindControl("txbUNotes");
            sa[icNotes] = Server.HtmlEncode(txb.Text.Trim().Replace("'", "`"));
            try
            {
                mCRUD.UpdateOne(Global.enugInfoType.SF_Subledgers, sMKey, sa);
                gvSubledger.EditIndex = -1;
            }
            catch (Global.excToPopup exc1)
            {
                ProcessPopupException(exc1);
            }
            DisplayInGrid(Global.enugInfoType.SF_Subledgers);
        }

        protected void gvSubledger_RowCreated(object sender, GridViewRowEventArgs e)
        {
            foreach (TableCell cell in e.Row.Cells) { cell.CssClass = "cell-padding"; }
        }

        protected void gvSubledger_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvSubledger.EditIndex = -1;
            DisplayInGrid(Global.enugInfoType.SF_Subledgers);
        }

        protected void gvSubledger_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvSubledger.PageIndex = e.NewPageIndex;
            DisplayInGrid(Global.enugInfoType.SF_Subledgers);
        }
    }
}