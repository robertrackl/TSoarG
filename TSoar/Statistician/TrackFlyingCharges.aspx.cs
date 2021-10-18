using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TSoar.DB;

namespace TSoar.Statistician
{
    public partial class TrackFlyingCharges : System.Web.UI.Page
    {
        //private string sKey {
        //    get {
        //        string ss = (string)ViewState["sKey"] ?? "";
        //        return ss;
        //    }
        //    set {
        //        ViewState["sKey"] = value;
        //    }
        //}
        //private string sMember { get { return (string)ViewState["sMember"] ?? ""; } set { ViewState["sMember"] = value; } }
        //private SCUD_Multi mCRUD = new SCUD_Multi();

        //protected void Page_Load(object sender, EventArgs e)
        //{
        //    if (!IsPostBack)
        //    {
        //        DisplayInGrid();
        //    }
        //}

        //#region Modal Popup
        ////======================
        //private void ButtonsClear()
        //{
        //    NoButton.CommandArgument = "";
        //    NoButton.CommandName = "";
        //    YesButton.CommandArgument = "";
        //    YesButton.CommandName = "";
        //    OkButton.CommandArgument = "";
        //    OkButton.CommandName = "";
        //    CancelButton.CommandArgument = "";
        //    CancelButton.CommandName = "";
        //}
        //private void MPE_Show(Global.enumButtons eubtns)
        //{
        //    NoButton.CssClass = "displayNone";
        //    YesButton.CssClass = "displayNone";
        //    OkButton.CssClass = "displayNone";
        //    CancelButton.CssClass = "displayNone";
        //    switch (eubtns)
        //    {
        //        case Global.enumButtons.NoYes:
        //            NoButton.CssClass = "displayUnset";
        //            YesButton.CssClass = "displayUnset";
        //            break;
        //        case Global.enumButtons.OkOnly:
        //            OkButton.CssClass = "displayUnset";
        //            break;
        //        case Global.enumButtons.OkCancel:
        //            OkButton.CssClass = "displayUnset";
        //            CancelButton.CssClass = "displayUnset";
        //            break;
        //    }
        //    ModalPopExt.Show();
        //}
        //protected void Button_Click(object sender, EventArgs e)
        //{
        //    Button btn = (Button)sender;
        //    try
        //    {
        //        if ((btn.ID == "YesButton") && (btn.CommandName == "Delete"))
        //        {
        //            switch (OkButton.CommandArgument)
        //            {
        //                case "Track Flying Charges":
        //                    mCRUD.DeleteOne(Global.enugInfoType.FlyingCharges, btn.CommandArgument);
        //                    DisplayInGrid();
        //                    break;
        //            }
        //        }
        //    }
        //    catch (Global.excToPopup exc)
        //    {
        //        ProcessPopupException(exc);
        //    }
        //}

        //private void ProcessPopupException(Global.excToPopup excu)
        //{
        //    ButtonsClear();
        //    lblPopupText.Text = excu.sExcMsg();
        //    MPE_Show(Global.enumButtons.OkOnly);
        //}
        //#endregion

        //private void DisplayInGrid()
        //{
        //    GridView g = gvTFC;
        //    g.PageSize = 20;
        //    DataTable dt = mCRUD.GetAll(Global.enugInfoType.FlyingCharges);
        //    g.DataSource = dt;
        //    g.PageSize = Int32.Parse(mCRUD.GetSetting("PageSizeTrackFlyingCharges"));
        //    g.DataBind();
        //    g.Columns[4].ItemStyle.HorizontalAlign = HorizontalAlign.Right;
        //    g.Columns[5].ItemStyle.HorizontalAlign = HorizontalAlign.Center;
        //    g.Columns[6].ItemStyle.HorizontalAlign = HorizontalAlign.Center;
        //}

        //protected void DDLMember_PreRender(object sender, EventArgs e)
        //{
        //    SetDropDownByValue((DropDownList)sender, "Robert Rackl");
        //}

        //protected void dvTFC_PreRender(object sender, EventArgs e)
        //{
        //    TextBox tb = (TextBox)dvTFC.FindControl("txbDAmount");
        //    tb.Text = DateTime.Today.AddDays(-2).ToString("yyyy-MM-dd");
        //}

        //protected void dvTFC_ModeChanging(object sender, DetailsViewModeEventArgs e)
        //{
        //    // This method must be present even though it is empty.
        //}

        //protected void dvTFC_ItemInserting(object sender, DetailsViewInsertEventArgs e)
        //{
        //    string [] sa = new string[6];
        //    sa[0] = DateTime.UtcNow.ToString();
        //    sa[1] = mCRUD.GetPeopleIDfromWebSiteUserName(HttpContext.Current.User.Identity.Name).ToString();
        //    sa[2] = ((DropDownList)dvTFC.FindControl("DDL_Member")).SelectedValue.ToString();
        //    sa[3] = ((TextBox)dvTFC.FindControl("txbAmount")).Text.Trim();
        //    sa[4] = ((TextBox)dvTFC.FindControl("txbDAmount")).Text;
        //    //sa[5] = ((RadioButtonList)dvTFC.FindControl("rblTFC")).Items[0].Selected ? "C" : "B";
        //    sa[5] = ((RadioButtonList)dvTFC.FindControl("rblTFC")).SelectedValue;
        //    int iIdent = 0;
        //    mCRUD.InsertOne(Global.enugInfoType.FlyingCharges, sa, out iIdent);
        //    string sp = "Record Inserted: ";
        //    sp += ((DropDownList)dvTFC.FindControl("DDL_Member")).SelectedItem;
        //    for (int i = 1; i < sa.Count(); i++)
        //    {
        //        sp += ", " + sa[i];
        //    }
        //    Global.excToPopup exc = new Global.excToPopup(sp); // not an exception - just using to display confirmation of insertion
        //    ProcessPopupException(exc);
        //    e.Cancel = true; // because we are handling our own insertion
        //    DisplayInGrid();
        //}

        //protected void gvTFC_RowEditing(object sender, GridViewEditEventArgs e)
        //{
        //    gvTFC.EditIndex = e.NewEditIndex;
        //    DisplayInGrid();
        //}

        //protected void gvTFC_RowDataBound(object sender, GridViewRowEventArgs e)
        //{
        //    if (e.Row.RowType == DataControlRowType.DataRow)
        //    {
        //        if ((e.Row.RowState & DataControlRowState.Edit) > 0)
        //        {
        //            sKey = e.Row.Cells[0].Text;
        //            DropDownList ddlMembers = (DropDownList)e.Row.FindControl("DDLMember");
        //            sMember = DataBinder.Eval(e.Row.DataItem, "sMDisplayName").ToString();
        //            SetDropDownByValue(ddlMembers, sMember);
        //            TextBox txbDOA = (TextBox)e.Row.FindControl("txbDOA");
        //            string sD = DataBinder.Eval(e.Row.DataItem, "DateOfAmount").ToString();
        //            if (sD.Length > 0)
        //            {
        //                DateTime DD = DateTime.Parse(sD);
        //                txbDOA.Text = DD.ToString("yyyy-MM-dd");
        //            }
        //            else
        //            {
        //                txbDOA.Text = sD;
        //            }
        //            RadioButtonList rbl = (RadioButtonList)e.Row.FindControl("rblUpdTFC");
        //            Dictionary<string, int> dictis = new Dictionary<string, int>() { { "A", 0 }, { "B", 1 }, { "M", 2 }, { "T", 3 } };
        //            rbl.Items[dictis[DataBinder.Eval(e.Row.DataItem, "cTypeOfAmount").ToString()]].Selected = true;
        //            //sD = DataBinder.Eval(e.Row.DataItem, "cTypeOfAmount").ToString();
        //            //switch (sD)
        //            //{
        //            //    case "A":
        //            //        rbl.Items[0].Selected = true;
        //            //        break;
        //            //    case "B":
        //            //        rbl.Items[1].Selected = true;
        //            //        break;
        //            //    case "M":
        //            //        rbl.Items[2].Selected = true;
        //            //        break;
        //            //    case "T":
        //            //        rbl.Items[3].Selected = true;
        //            //        break;
        //            //}
        //        }
        //    }
        //}
        //public void SetDropDownByValue(DropDownList ddl, string sText)
        //{
        //    ddl.ClearSelection();
        //    foreach (ListItem li in ddl.Items)
        //    {
        //        if (li.Text == sText)
        //        {
        //            li.Selected = true;
        //            break;
        //        }
        //    }
        //    ddl.SelectedItem.Text = sText;
        //}

        //protected void gvTFC_RowDeleting(object sender, GridViewDeleteEventArgs e)
        //{
        //    ButtonsClear();
        //    OkButton.CommandArgument = "Track Flying Charges";
        //    YesButton.CommandName = "Delete";
        //    YesButton.CommandArgument = e.Values[0].ToString();
        //    lblPopupText.Text = "Please confirm deletion of " + OkButton.CommandArgument + " record with row identifier " + YesButton.CommandArgument;
        //    MPE_Show(Global.enumButtons.NoYes);
        //    e.Cancel = true;
        //}

        //protected void gvTFC_RowUpdating(object sender, GridViewUpdateEventArgs e)
        //{
        //    string[] sa = new string[6];
        //    e.Cancel = true;
        //    sa[0] = DateTime.UtcNow.ToString();
        //    sa[1] = mCRUD.GetPeopleIDfromWebSiteUserName(HttpContext.Current.User.Identity.Name).ToString();
        //    GridViewRow row = gvTFC.Rows[e.RowIndex];
        //    DropDownList ddlMembers = (DropDownList)row.FindControl("DDLMember");
        //    sa[2] = ddlMembers.Items[ddlMembers.SelectedIndex].Value;
        //    sa[3] = ((TextBox)row.FindControl("txbAmount")).Text;
        //    sa[4] = ((TextBox)row.FindControl("txbDOA")).Text;
        //    DateTime DOA = new DateTime();
        //    if (!DateTime.TryParse(sa[4], out DOA))
        //    {
        //        lblPopupText.Text = "Date of Amount string '" + sa[4] + "' is not in proper date format";
        //        MPE_Show(Global.enumButtons.OkOnly);
        //        return;
        //    }
        //    RadioButtonList rl = (RadioButtonList)row.FindControl("rblUpdTFC");
        //    sa[5] = (rl).Items[0].Value;
        //    mCRUD.UpdateOne(Global.enugInfoType.FlyingCharges, sKey, sa);
        //    gvTFC.EditIndex = -1;
        //    DisplayInGrid();
        //}

        //protected void gvTFC_RowCreated(object sender, GridViewRowEventArgs e)
        //{
        //    foreach (TableCell cell in e.Row.Cells) { cell.CssClass = "cell-padding"; }
        //}

        //protected void gvTFC_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        //{
        //    gvTFC.EditIndex = -1;
        //    DisplayInGrid();
        //}

        //protected void gvTFC_PageIndexChanging(object sender, GridViewPageEventArgs e)
        //{
        //    gvTFC.PageIndex = e.NewPageIndex;
        //    DisplayInGrid();
        //}
    }
}