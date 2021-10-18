using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Linq;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
//using TSoar.DB;

namespace TSoar.Equipment
{
    public partial class EquipmentList : System.Web.UI.Page
    {
        private string sKey { get { return (string)ViewState["sKey"] ?? ""; } set { ViewState["sKey"] = value; } }

        TSoar.DB.SCUD_Multi mCRUD = new TSoar.DB.SCUD_Multi();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DisplayInGrid(Global.enugInfoType.Equipment);
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
            Button btn = (Button)sender;
            //Global.oLog("Modal popup dismissed with " + btn.ID + ", CommandName=" + btn.CommandName);
            if (btn.ID == "YesButton")
            {
                switch (btn.CommandName)
                {
                    case "Delete":
                        // Delete the Equipment record
                        try
                        {
                            mCRUD.DeleteOne(Global.enugInfoType.Equipment, btn.CommandArgument);
                        }
                        catch (Global.excToPopup exc)
                        {
                            ProcessPopupException(exc);
                        }
                        DisplayInGrid(Global.enugInfoType.Equipment);
                        break;
                }
            }
        }

        private void ProcessPopupException(Global.excToPopup excu)
        {
            ButtonsClear();
            lblPopupText.Text = excu.sExcMsg();
            MPE_Show(Global.enumButtons.OkOnly);
        }
        #endregion

        private void DisplayInGrid(Global.enugInfoType enugMultiMFF)
        {
            gvEquip.DataSource = mCRUD.GetAll(enugMultiMFF);
            gvEquip.PageSize = Int32.Parse(mCRUD.GetSetting("PageSizeEquipmentList"));
            gvEquip.DataBind();
        }

        protected void dvEquip_ItemInserting(object sender, DetailsViewInsertEventArgs e)
        {
            Global.excToPopup exc;
            e.Cancel = true; // because we are handling our own insertion
            string sp = "Equipment Name must not be empty";
            string[] sa = new string[12];
            sa[0] = ((DropDownList)dvEquip.FindControl("DDL_EquipType")).SelectedValue.ToString();
            sa[1] = CustFmt.sFmtDate(DateTime.UtcNow, CustFmt.enDFmt.DateAndTimeSec);
            sa[2] = mCRUD.GetPeopleIDfromWebSiteUserName(HttpContext.Current.User.Identity.Name).ToString();
            sa[3] = Server.HtmlEncode(((TextBox)dvEquip.FindControl("txb_ShortEquipName")).Text.Trim());
            if (sa[3].Length < 1)
            {
                exc = new Global.excToPopup(sp);
            }
            else
            {
                sa[4] = Server.HtmlEncode(((TextBox)dvEquip.FindControl("txb_Description")).Text.Trim());
                sa[5] = Server.HtmlEncode(((TextBox)dvEquip.FindControl("txb_Registration")).Text.Trim());
                sa[6] = Server.HtmlEncode(((TextBox)dvEquip.FindControl("txb_Owner")).Text.Trim());
                sa[7] = Server.HtmlEncode(((TextBox)dvEquip.FindControl("txb_ModelNum")).Text.Trim());
                sa[8] = ((TextBox)dvEquip.FindControl("txb_AcqCost")).Text.Trim();
                sa[9] = ((TextBox)dvEquip.FindControl("txb_OwnBegin")).Text;
                sa[10] = ((TextBox)dvEquip.FindControl("txb_OwnEnd")).Text;
                sa[11] = Server.HtmlEncode(((TextBox)dvEquip.FindControl("txb_Notes")).Text.Trim());
                if (mCRUD.Exists(Global.enugInfoType.Equipment, sa[3]) > 0)
                {
                    sp = "Equipment named '" + sa[3] + "' already exists";
                    exc = new Global.excToPopup(sp);
                }
                else
                {
                    int iIdent = 0;
                    mCRUD.InsertOne(Global.enugInfoType.Equipment, sa, out iIdent);
                    sp = "Record Inserted: ";
                    sp += ((DropDownList)dvEquip.FindControl("DDL_EquipType")).SelectedItem;
                    for (int i = 1; i < sa.Count(); i++)
                    {
                        sp += ", " + sa[i];
                    }
                    exc = new Global.excToPopup(sp); // not an exception - just using to display confirmation of insertion
                }
            }
            ProcessPopupException(exc);
            DisplayInGrid(Global.enugInfoType.Equipment);
        }

        protected void DDL_EquipType_PreRender(object sender, EventArgs e)
        {
            SetDropDownByValue((DropDownList)sender, mCRUD.GetSetting("DefaultEquipmentType"));
        }

        public void SetDropDownByValue(DropDownList ddl, string sText)
        {
            ddl.ClearSelection();
            foreach (ListItem li in ddl.Items)
            {
                if (li.Text == sText)
                {
                    li.Selected = true;
                    break;
                }
            }
            ddl.SelectedItem.Text = sText;
        }

        private void EquipmentUpdate(string[] sau)
        {
            try
            {
                mCRUD.UpdateOne(Global.enugInfoType.Equipment, sKey, sau);
                gvEquip.EditIndex = -1;
            }
            catch (Global.excToPopup exc)
            {
                ProcessPopupException(exc);
            }
            DisplayInGrid(Global.enugInfoType.Equipment);
        }

        protected void gvEquip_RowCreated(object sender, GridViewRowEventArgs e)
        {
            foreach (TableCell cell in e.Row.Cells) { cell.CssClass = "cell-padding"; }
        }

        protected void gvEquip_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            e.Cancel = true;
            GridView gv = (GridView)sender;
            string sID = ((Label)gv.Rows[e.RowIndex].FindControl("lblIIdent")).Text;
            string sMsg = "You should probably NOT DELETE this equipment record because it is referenced from other data tables. " +
                "Those other records would also be wiped out (and quite possibly more data if there are more related tables): ";
            int iCount = 0;
            EquipmentDataContext eqdc = new EquipmentDataContext();
            try
            {
                var dtEq = eqdc.spForeignKeyRefs("EQUIPMENT", Int32.Parse(sID));
                foreach (var row in dtEq)
                {
                    iCount += (int)row.iNumFKRefs;
                    sMsg += row.iNumFKRefs.ToString() + " times in " + row.sFKTable + ", ";
                }
                sMsg = sMsg.Substring(0, sMsg.Length - 2) + ". You should click on `No` unless you really know what you are doing!";
            }
            catch (Exception exc)
            {
                ProcessPopupException(new Global.excToPopup(exc.Message));
                return;
            }
            ButtonsClear();
            OkButton.CommandArgument = "Equip";
            YesButton.CommandName = "Delete";
            YesButton.CommandArgument = sID;
            if (iCount < 1)
            {
                lblPopupText.Text = "Please confirm deletion of equipment with row identifier " + sID;
            }
            else
            {
                lblPopupText.Text = sMsg;
            }
            MPE_Show(Global.enumButtons.NoYes);
        }

        protected void gvEquip_RowEditing(object sender, GridViewEditEventArgs e)
        {
            gvEquip.EditIndex = e.NewEditIndex;
            DisplayInGrid(Global.enugInfoType.Equipment);
        }

        protected void gvEquip_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if ((e.Row.RowState & DataControlRowState.Edit) > 0)
                {
                    sKey = e.Row.Cells[0].Text;
                    DropDownList ddlEquipType = (DropDownList)e.Row.FindControl("DDLEquipType");
                    string sD = DataBinder.Eval(e.Row.DataItem, "sEquipmentType").ToString();
                    SetDropDownByValue(ddlEquipType, sD);
                }
            }
        }

        protected void gvEquip_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvEquip.EditIndex = -1;
            DisplayInGrid(Global.enugInfoType.Equipment);
        }

        protected void gvEquip_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            string[] sa = new string[12];
            e.Cancel = true;
            GridViewRow row = gvEquip.Rows[e.RowIndex];
            Label lblIIdent = (Label)row.FindControl("lblIIdent");
            sKey = lblIIdent.Text;
            DropDownList ddlEquipType = (DropDownList)row.FindControl("DDLEquipType");
            sa[0] = ddlEquipType.Items[ddlEquipType.SelectedIndex].Value;
            sa[1] = CustFmt.sFmtDate(DateTime.UtcNow, CustFmt.enDFmt.DateAndTimeSec);
            sa[2] = mCRUD.GetPeopleIDfromWebSiteUserName(HttpContext.Current.User.Identity.Name).ToString();
            sa[3] = Server.HtmlEncode(((TextBox)row.FindControl("txbEquipName")).Text);
            sa[4] = Server.HtmlEncode(((TextBox)row.FindControl("txbDescript")).Text);
            sa[5] = Server.HtmlEncode(((TextBox)row.FindControl("txbRegistr")).Text);
            sa[6] = Server.HtmlEncode(((TextBox)row.FindControl("txbOwner")).Text);
            sa[7] = Server.HtmlEncode(((TextBox)row.FindControl("txbModelNum")).Text);
            sa[8] = ((TextBox)row.FindControl("txbAcquisCost")).Text;
            sa[9] = ((TextBox)row.FindControl("txbOwnBegin")).Text;
            sa[10] = ((TextBox)row.FindControl("txbOwnEnd")).Text;
            sa[11] = Server.HtmlEncode(((TextBox)row.FindControl("txbNotes")).Text);
            EquipmentUpdate(sa);
        }

        protected void gvEquip_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvEquip.PageIndex = e.NewPageIndex;
            DisplayInGrid(Global.enugInfoType.Equipment);
        }

        protected void dvEquip_ModeChanging(object sender, DetailsViewModeEventArgs e)
        {
            // Fires when user clicks on 'cancel'
            // Display the equipment list
            AccordionEquip.SelectedIndex = 1;
        }
    }
}