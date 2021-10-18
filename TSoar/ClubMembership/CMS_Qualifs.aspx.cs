using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TSoar.DB;

namespace TSoar.ClubMembership
{
    // Qualifs stands for 'Certifications', 'Ratings', as well as 'Qualifications'.
    // Certifications and Ratings arise out of Federal Aviation Administration (FAA) rules for airmen.
    // Qualifications arise out of other requirements, such as aircraft insurance policies, and club bylaws and operations rules.

    public partial class CMS_Qualifs : System.Web.UI.Page
    {
        #region Declarations
        private string sKey { get { return (string)ViewState["sKey"] ?? ""; } set { ViewState["sKey"] = value; } }
        private string sMember { get { return (string)ViewState["sMember"] ?? ""; } set { ViewState["sMember"] = value; } }
        private string[] sa { get { return (string[])ViewState["sa"] ?? new string[1] { "" }; } set { ViewState["sa"] = value; } }

        private Dictionary<string, Global.enugInfoType> dictgvit = new Dictionary<string, Global.enugInfoType>();
        private Dictionary<Global.enugInfoType, string> dictitddl = new Dictionary<Global.enugInfoType, string>();
        private Dictionary<Global.enugInfoType, string> dictitQCR = new Dictionary<Global.enugInfoType, string>();
        private Dictionary<Global.enugInfoType, GridView> dictitgv = new Dictionary<Global.enugInfoType, GridView>();

        SCUD_Multi mCRUD = new SCUD_Multi();
        #endregion

        protected void Page_PreInit(Object sender, EventArgs e)
        {
            dictgvit.Add("gvCMS_Qualifs", Global.enugInfoType.Qualifics);
            dictgvit.Add("gvCMS_Certifs", Global.enugInfoType.Certifics);
            dictgvit.Add("gvCMS_Ratings", Global.enugInfoType.Ratings);
            dictitddl.Add(Global.enugInfoType.Qualifics, "Qualif");
            dictitddl.Add(Global.enugInfoType.Certifics, "Certif");
            dictitddl.Add(Global.enugInfoType.Ratings, "Rating");
            dictitQCR.Add(Global.enugInfoType.Qualifics, "Qualification");
            dictitQCR.Add(Global.enugInfoType.Certifics, "sCertification");
            dictitQCR.Add(Global.enugInfoType.Ratings, "sRating");
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            dictitgv.Add(Global.enugInfoType.Qualifics, gvCMS_Qualifs);
            dictitgv.Add(Global.enugInfoType.Certifics, gvCMS_Certifs);
            dictitgv.Add(Global.enugInfoType.Ratings, gvCMS_Ratings);
            if (!IsPostBack)
            {
                DisplayInGrid(Global.enugInfoType.Qualifics);
                DisplayInGrid(Global.enugInfoType.Certifics);
                DisplayInGrid(Global.enugInfoType.Ratings);
                AccordionCMS_Qualifs.Visible = true;
                AccordionCertifs.Visible = false;
                AccordionRatings.Visible = false;
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
                        // Delete the Qualification/Certification/Rating FromTo record
                        try
                        {
                            mCRUD.DeleteOne(dictgvit[OkButton.CommandArgument], btn.CommandArgument);
                        }
                        catch (Global.excToPopup exc)
                        {
                            ProcessPopupException(exc);
                        }
                        DisplayInGrid(dictgvit[OkButton.CommandArgument]);
                        break;
                    case "Update":
                        QCRUpdate(dictgvit[OkButton.CommandArgument]);
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
            switch (enugMultiMFF)
            {
                case Global.enugInfoType.Qualifics:
                    gvCMS_Qualifs.PageSize = Int32.Parse(mCRUD.GetSetting("PageSizeCMSQualif"));
                    gvCMS_Qualifs.DataSource = mCRUD.GetAll(enugMultiMFF);
                    gvCMS_Qualifs.DataBind();
                    break;
                case Global.enugInfoType.Certifics:
                    gvCMS_Certifs.PageSize = Int32.Parse(mCRUD.GetSetting("PageSizeCMSCertifics"));
                    gvCMS_Certifs.DataSource = mCRUD.GetAll(enugMultiMFF);
                    gvCMS_Certifs.DataBind();
                    break;
                case Global.enugInfoType.Ratings:
                    gvCMS_Ratings.PageSize = Int32.Parse(mCRUD.GetSetting("PageSizeCMSRatings"));
                    gvCMS_Ratings.DataSource = mCRUD.GetAll(enugMultiMFF);
                    gvCMS_Ratings.DataBind();
                    break;
            }
        }

        #region DetailsViews
        protected void dv_ItemInserting(object sender, DetailsViewInsertEventArgs e)
        {
            DetailsView dv = (DetailsView)sender;
            string sDDL="";
            Global.enugInfoType enugit = Global.enugInfoType.Qualifics;
            switch (dv.ID)
            {
                case "dvCMS_Qualifs":
                    sDDL = "DDL_Qualif";
                    break;
                case "dvCMS_Certifs":
                    sDDL = "DDL_Certif";
                    enugit = Global.enugInfoType.Certifics;
                    break;
                case "dvCMS_Ratings":
                    sDDL = "DDL_Rating";
                    enugit = Global.enugInfoType.Ratings;
                    break;
            }
            sa = new string[5];
            sa[0] = ((DropDownList)dv.FindControl("DDL_Member")).SelectedValue.ToString();
            sa[1] = ((DropDownList)dv.FindControl(sDDL)).SelectedValue.ToString();
            string sOffset = mCRUD.GetSetting("TimeZoneOffset");
            string stmp = ((TextBox)dv.FindControl("txb_QBegan")).Text;
            if (stmp.Length < 1)
            {
                stmp = "1900/01/01";
            }
            sa[2] = stmp.Substring(0,10) + " 01:01:00 " + sOffset;
            stmp = ((TextBox)dv.FindControl("txb_QEnd")).Text;
            if (stmp.Length < 1)
            {
                stmp = "2999/12/31";
            }
            sa[3] = stmp.Substring(0, 10) + " 22:59:00 " + sOffset;
            sa[4] = Server.HtmlEncode(((TextBox)dv.FindControl("txb_Notes")).Text);
            int iIdent = 0;
            mCRUD.InsertOne(enugit, sa, out iIdent);
            string sp = "Record Inserted: ";
            sp += ((DropDownList)dv.FindControl("DDL_Member")).SelectedItem;
            sp += ", " + ((DropDownList)dv.FindControl(sDDL)).SelectedItem;
            for (int i = 2; i < sa.Count(); i++)
            {
                sp += ", " + sa[i];
            }
            Global.excToPopup exc = new Global.excToPopup(sp); // not an exception - just using to display confirmation of insertion
            ProcessPopupException(exc);
            e.Cancel = true; // because we are handling our own insertion
            DisplayInGrid(enugit);
        }
        #endregion

        #region DropDownLists
        protected void DDL_Qualif_PreRender(object sender, EventArgs e)
        {
            SetDropDownByValue((DropDownList)sender, mCRUD.GetSetting("DefaultQualification"));
        }

        protected void DDL_Rating_PreRender(object sender, EventArgs e)
        {
            SetDropDownByValue((DropDownList)sender, mCRUD.GetSetting("DefaultRating"));
        }

        protected void DDL_Certif_PreRender(object sender, EventArgs e)
        {
            SetDropDownByValue((DropDownList)sender, mCRUD.GetSetting("DefaultCertification"));
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
        #endregion

        #region Gridviews
        protected void gv_RowCreated(object sender, GridViewRowEventArgs e)
        {
            foreach (TableCell cell in e.Row.Cells) { cell.CssClass = "cell-padding"; }
        }

        protected void gv_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            GridView gv = (GridView)sender;
            ButtonsClear();
            OkButton.CommandArgument = gv.ID;
            YesButton.CommandName = "Delete";
            YesButton.CommandArgument = e.Values[0].ToString();
            lblPopupText.Text = "Please confirm deletion of qual/certification/rating record with row identifier " + YesButton.CommandArgument;
            MPE_Show(Global.enumButtons.NoYes);
            e.Cancel = true;
        }

        protected void gv_RowEditing(object sender, GridViewEditEventArgs e)
        {
            GridView gv = (GridView)sender;
            gv.EditIndex = e.NewEditIndex;
            DisplayInGrid(dictgvit[gv.ID]);
        }

        protected void gv_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if ((e.Row.RowState & DataControlRowState.Edit) > 0)
                {
                    GridView gv = (GridView)sender;
                    sKey = e.Row.Cells[0].Text;
                    DropDownList ddlMembers = (DropDownList)e.Row.FindControl("DDLMember");
                    sMember = DataBinder.Eval(e.Row.DataItem, "Display_Name").ToString();
                    SetDropDownByValue(ddlMembers, sMember);
                    DropDownList ddlQualif = (DropDownList)e.Row.FindControl("DDL" + dictitddl[dictgvit[gv.ID]]);
                    string sD = DataBinder.Eval(e.Row.DataItem, dictitQCR[dictgvit[gv.ID]]).ToString();
                    SetDropDownByValue(ddlQualif, sD);
                    TextBox txbB = (TextBox)e.Row.FindControl("txbSince");
                    sD = DataBinder.Eval(e.Row.DataItem, "DSince").ToString();
                    if (sD.Length > 0)
                    {
                        DateTime DD = DateTime.Parse(sD);
                        txbB.Text = DD.ToString("yyyy-MM-dd");
                    }
                    else
                    {
                        txbB.Text = sD;
                    }
                    TextBox txbE = (TextBox)e.Row.FindControl("txbExpires");
                    sD = DataBinder.Eval(e.Row.DataItem, "DExpiry").ToString();
                    if (sD.Length > 0)
                    {
                        DateTime DD = DateTime.Parse(sD);
                        txbE.Text = DD.ToString("yyyy-MM-dd");
                    }
                    else
                    {
                        txbE.Text = sD;
                    }
                }
            }
        }

        protected void gv_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            GridView gv = (GridView)sender;
            gv.EditIndex = -1;
            DisplayInGrid(dictgvit[gv.ID]);
        }

        protected void gv_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            GridView gv = (GridView)sender;
            sa = new string[5];
            e.Cancel = true;
            GridViewRow row = gv.Rows[e.RowIndex];
            DropDownList ddlMembers = (DropDownList)row.FindControl("DDLMember");
            sa[0] = ddlMembers.Items[ddlMembers.SelectedIndex].Value;
            DropDownList ddlQCR = (DropDownList)row.FindControl("DDL" + dictitddl[dictgvit[gv.ID]]);
            sa[1] = ddlQCR.Items[ddlQCR.SelectedIndex].Value;
            sa[2] = ((TextBox)row.FindControl("txbSince")).Text;
            sa[3] = ((TextBox)row.FindControl("txbExpires")).Text;
            DateTime DBeg = new DateTime();
            if (!DateTime.TryParse(sa[2], out DBeg))
            {
                lblPopupText.Text = "Begin date string '" + sa[2] + "' is not in proper date format";
                MPE_Show(Global.enumButtons.OkOnly);
                return;
            }
            DateTime DEnd = new DateTime();
            if (sa[3].Length > 0)
            {
                if (!DateTime.TryParse(sa[3], out DEnd))
                {
                    lblPopupText.Text = "End date string '" + sa[3] + "' is not in proper date format";
                    MPE_Show(Global.enumButtons.OkOnly);
                    return;
                }
            }
            else
            {
                DEnd = DateTime.MaxValue;
            }
            if (DEnd < DBeg)
            {
                lblPopupText.Text = "End date must be greater or equal to begin date";
                MPE_Show(Global.enumButtons.OkOnly);
                return;
            }
            sa[4] = Server.HtmlEncode(((TextBox)row.FindControl("txbNotes")).Text);
            if (ddlMembers.SelectedItem.Text != sMember)
            {
                ButtonsClear();
                YesButton.CommandName = "Update";
                OkButton.CommandArgument = gv.ID;
                lblPopupText.Text = "It is quite unusual to change the member for a qual/certification/rating entry; please confirm that you want to do that:";
                MPE_Show(Global.enumButtons.NoYes);
            }
            else
            {
                QCRUpdate(dictgvit[gv.ID]);
            }
        }

        private void QCRUpdate(Global.enugInfoType enugit)
        {
            try
            {
                mCRUD.UpdateOne(enugit, sKey, sa);
                dictitgv[enugit].EditIndex = -1;
            }
            catch (Global.excToPopup exc)
            {
                ProcessPopupException(exc);
            }
            DisplayInGrid(enugit);
        }

        protected void gvCMS_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridView gv = (GridView)sender;
            gv.PageIndex = e.NewPageIndex;
            DisplayInGrid(dictgvit[gv.ID]);
        }
        #endregion

        protected void pbQCR_Click(object sender, EventArgs e)
        {
            Button pbQCR = (Button)sender;
            switch (pbQCR.ID)
            {
                case "pbQualifs":
                    AccordionCMS_Qualifs.Visible = true;
                    AccordionCertifs.Visible = false;
                    AccordionRatings.Visible = false;
                    break;
                case "pbCertifs":
                    AccordionCMS_Qualifs.Visible = false;
                    AccordionCertifs.Visible = true;
                    AccordionRatings.Visible = false;
                    break;
                case "pbRatings":
                    AccordionCMS_Qualifs.Visible = false;
                    AccordionCertifs.Visible = false;
                    AccordionRatings.Visible = true;
                    break;
            }
        }

        protected void dvCMS_Qualifs_ModeChanging(object sender, DetailsViewModeEventArgs e)
        {

        }

        protected void dvCMS_Certifs_ModeChanging(object sender, DetailsViewModeEventArgs e)
        {

        }

        protected void dvCMS_Ratings_ModeChanging(object sender, DetailsViewModeEventArgs e)
        {

        }
    }
}