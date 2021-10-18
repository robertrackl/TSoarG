using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Security;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TSoar.DB;

namespace TSoar.MemberPages.ManageSettings
{
    public partial class ManageSettings : System.Web.UI.Page
    {
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
                        case "Basic":
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
            if (!Page.IsPostBack)
            {
                FillGrid();
            }
        }

        private void FillGrid()
        {
            ManageSettingsDataContext msdc = new ManageSettingsDataContext();
            var qs = (from s in msdc.sp_ManageSettings(HttpContext.Current.User.Identity.Name) select s).ToList();
            // Replace the setting values with the ones from the user's profile
            APTSoarSetting USSettings = AccountProfile.CurrentUser.APTSUserSelectableSettings;
            foreach (DataRow r in USSettings.APTS.Rows)
            {
                int i = qs.FindIndex(x => x.sSettingName == (string)r[0]);
                if (i > -1)
                {
                    qs[i].sSettingValue = (string)r[1];
                }
            }
            gvUSS.DataSource = qs;
            gvUSS.DataBind();
        }

        protected void gvUSS_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                string sRowIndex = e.Row.RowIndex.ToString();
                if ((e.Row.RowState & DataControlRowState.Edit) == 0)
                {
                    ImageButton pbEdit = (ImageButton)e.Row.FindControl("pbEdit");
                    if (gvUSS.EditIndex == -1)
                    {
                        pbEdit.CommandArgument = sRowIndex;
                    }
                    else
                    {
                        pbEdit.Visible = false;
                    }
                }
                else
                {
                    ImageButton pbUpdate = (ImageButton)e.Row.FindControl("pbUpdate");
                    pbUpdate.CommandArgument = sRowIndex;
                    RangeValidator RgVValue = (RangeValidator)e.Row.FindControl("RgVValue");
                    RgVValue.Visible = false;
                    RgVValue.Enabled = false;
                    RegularExpressionValidator RegExValue = (RegularExpressionValidator)e.Row.FindControl("RegExValue");
                    RegExValue.Visible = false;
                    RegExValue.Enabled = false;
                    TextBox txbValue = (TextBox)e.Row.FindControl("txbValue");
                    txbValue.Visible = false;
                    DropDownList DDLValue = (DropDownList)e.Row.FindControl("DDLValue");
                    DDLValue.Visible = false;
                    switch ((char)DataBinder.Eval(e.Row.DataItem, "cSettingType"))
                    {
                        case 'F':
                            SqlDS_Value.SelectCommand = (string)DataBinder.Eval(e.Row.DataItem,"sSelectStmnt");
                            DDLValue.DataSource = SqlDS_Value;
                            DDLValue.DataValueField = (string)DataBinder.Eval(e.Row.DataItem, "sDataValueField");
                            DDLValue.DataTextField = (string)DataBinder.Eval(e.Row.DataItem, "sDataTextField");
                            DDLValue.DataBind();
                            SetDropDownByText(DDLValue, (string)DataBinder.Eval(e.Row.DataItem, "sSettingValue"));
                            DDLValue.Visible = true;
                            break;
                        case 'I':
                            RgVValue.Enabled = true;
                            RgVValue.Visible = true;
                            RgVValue.MinimumValue = Convert.ToInt32(DataBinder.Eval(e.Row.DataItem, "dLow")).ToString();
                            RgVValue.MaximumValue = Convert.ToInt32(DataBinder.Eval(e.Row.DataItem, "dHigh")).ToString();
                            RgVValue.Type = ValidationDataType.Integer;
                            RgVValue.Text = "Must be between " + RgVValue.MinimumValue + " and " + RgVValue.MaximumValue;
                            txbValue.Visible = true;
                            break;
                        case 'D':
                            RgVValue.Enabled = true;
                            RgVValue.Visible = true;
                            RgVValue.MinimumValue = DataBinder.Eval(e.Row.DataItem, "dLow").ToString();
                            RgVValue.MaximumValue = DataBinder.Eval(e.Row.DataItem, "dHigh").ToString();
                            RgVValue.Type = ValidationDataType.Double;
                            RgVValue.Text = "Must be between " + RgVValue.MinimumValue + " and " + RgVValue.MaximumValue;
                            txbValue.Visible = true;
                            break;
                        case 'S':
                            RegExValue.Enabled = true;
                            RegExValue.Visible = true;
                            // Double use of field sSelectStmnt here as ValidationExpression
                            RegExValue.ValidationExpression = (string)DataBinder.Eval(e.Row.DataItem, "sSelectStmnt");
                            RegExValue.Text = (string)DataBinder.Eval(e.Row.DataItem, "sDataTextField");
                            txbValue.Visible = true;
                            break;
                        case 'N':
                            break;
                    }
                }
            }
        }

        private void SetDropDownByText(DropDownList ddl, string sText)
        {
            if (ddl.Items.Count > 0)
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
        }

        protected void pbEdit_Click(object sender, ImageClickEventArgs e)
        {
            gvUSS.EditIndex = Convert.ToInt32(((ImageButton)sender).CommandArgument);
            FillGrid();
        }

        protected void pbUpdate_Click(object sender, ImageClickEventArgs e)
        {
            int iRow = Convert.ToInt32(((ImageButton)sender).CommandArgument);
            TextBox txbValue = (TextBox)gvUSS.Rows[iRow].FindControl("txbValue");
            Label lblIName = (Label)gvUSS.Rows[iRow].FindControl("lblIName");
            APTSoarSetting USSettings = AccountProfile.CurrentUser.APTSUserSelectableSettings;
            USSettings.SetUSSetting(lblIName.Text, txbValue.Text);
            AccountProfile.CurrentUser.APTSUserSelectableSettings = USSettings;
            gvUSS.EditIndex = -1;
            FillGrid();
        }

        protected void pbCancel_Click(object sender, ImageClickEventArgs e)
        {
            gvUSS.EditIndex = -1;
            FillGrid();
        }
    }
}