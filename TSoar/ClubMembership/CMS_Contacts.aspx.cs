using System;
using System.Collections.Generic;
using System.Data;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TSoar.DB;
using TSoar.MemberPages.Stats;

namespace TSoar.ClubMembership
{
    public partial class CMS_Contact : System.Web.UI.Page
    {
        SCUD_Multi mCRUD = new SCUD_Multi();
        private string sFilterMemberName { get { return (string)ViewState["sFilterMemberName"] ?? ""; } set { ViewState["sFilterMemberName"] = value; } }
        private Global.strgMbrContactsFilter MCFsettings = new Global.strgMbrContactsFilter(true, "");

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
                        case "Contact":
                            // Delete the Member Contact
                            // First delete the physical address record if one is associated:
                            TNPV_PeopleContactsDataContext d = new TNPV_PeopleContactsDataContext();
                            var q = from m in d.PEOPLECONTACTs where m.ID == Int32.Parse(btn.CommandArgument) select m.iPhysAddress;
                            int iPhysAddr = (int)q.First();
                            if (iPhysAddr > 0)
                            {
                                mCRUD.DeleteOne(Global.enugInfoType.PhysicalAddresses, iPhysAddr.ToString());
                            }
                            mCRUD.DeleteOne(Global.enugInfoType.Contacts, btn.CommandArgument);
                            DisplayInGrid();
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

        protected void Page_PreRender(object sender, EventArgs e)
        {
                MCFsettings = AccountProfile.CurrentUser.MCFsettings;
                sFilterMemberName = MCFsettings.sFilterMemberName;
                chbMemberFilter.Checked = MCFsettings.bFilterOn;
                DisplayInGrid();
        }

        private void DisplayInGrid()
        {
            GridView g = gvCMS_Contact;
            string ss = "PageSizeContacts";
            g.PageSize = Int32.Parse(mCRUD.GetSetting(ss));
            if (chbMemberFilter.Checked)
            {
                TNPV_PeopleContactsDataContext d = new TNPV_PeopleContactsDataContext();
                if (sFilterMemberName.Length < 1)
                {
                    string sWebSiteUserName = HttpContext.Current.User.Identity.Name;
                    try
                    {
                        sFilterMemberName = mCRUD.GetPeopleDisplayNamefromWebSiteUserName(sWebSiteUserName).ToString();
                    }
                    catch (Exception exc)
                    {
                        ActivityLog.oLog(ActivityLog.enumLogTypes.ErrorContinuable, 0, "CMS_Contact.aspx.cs.DisplayInGrid: Website user name `" +
                            sWebSiteUserName + "` does not have a club member associated with it; exception message=" + exc.Message);
                        // Pick some club member who already has a contact record:
                        var q2 = from m in d.PEOPLECONTACTs select m.PEOPLE.sDisplayName;
                        int iCount = q2.Count();
                        if (iCount > 0)
                        {
                            sFilterMemberName = q2.First();
                        }
                        else
                        {
                            sFilterMemberName = "";
                        }
                    }
                }
                var q = from m in d.TNPV_PeopleContacts where m.sDisplayName == sFilterMemberName orderby m.dContactPriorityRanking descending select m;
                g.DataSource = q;
                g.DataBind();
                ss = "for " + sFilterMemberName;
            }
            else
            {
                TNPV_PeopleContactsDataContext d = new TNPV_PeopleContactsDataContext();
                var q = from m in d.TNPV_PeopleContacts orderby m.sDisplayName,m.dContactPriorityRanking descending select m;
                g.DataSource = q;
                g.DataBind();
                ss = "";
            }
            if (g.Rows.Count < 1)
            {
                lblNoRecords.Visible = true;
            }
            else
            {
                lblNoRecords.Visible = false;
            }
            lblNoRecords.Text = "N O &nbsp; R E C O R D S &nbsp; " + ss;
            if (sFilterMemberName.Length > 0)
            {
                SetDropDownByValue(DDL_Member, sFilterMemberName);
            }
        }

        private void SetDropDownByValue(DropDownList ddl, string sText)
        {
            try
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
            catch { }
        }

        protected void gvCMS_Contact_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvCMS_Contact.PageIndex = e.NewPageIndex;
            DisplayInGrid();
        }

        protected void gvCMS_Contact_RowCreated(object sender, GridViewRowEventArgs e)
        {
            foreach (TableCell cell in e.Row.Cells) { cell.CssClass = "cell-padding"; }
        }

        protected void chbMemberFilter_CheckedChanged(object sender, EventArgs e)
        {
            SetMemberFilter();
        }
        private void SetMemberFilter()
        {
            MCFsettings.bFilterOn = chbMemberFilter.Checked;
            MCFsettings.sFilterMemberName = sFilterMemberName;
            AccountProfile.CurrentUser.MCFsettings = MCFsettings;
            DisplayInGrid();
        }

        protected void pbNew_Click(object sender, EventArgs e)
        {
            Session["PeopleID"] = "0";
            Session["PeopleContactID"] = "0";
            Response.Redirect("~/ClubMembership/CMS_ContactEdit.aspx");
        }

        protected void gvCMS_Contact_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                Label lblDStart = (Label)e.Row.FindControl("lblDStart");
                if (lblDStart.Text.Length > 9)
                {
                    lblDStart.Text = lblDStart.Text.Substring(0, 10);
                }
                Label lblDEnd = (Label)e.Row.FindControl("lblDEnd");
                if (lblDEnd.Text.Length > 9)
                {
                    lblDEnd.Text = lblDEnd.Text.Substring(0, 10);
                }
                Label lblCType = (Label)e.Row.FindControl("lblCType");
                TNPV_PeopleContactsDataContext d = new TNPV_PeopleContactsDataContext();
                var q = from m in d.CONTACTTYPEs where m.sPeopleContactType == lblCType.Text select m.bHasPhysAddr;
                if (q.First())
                {
                    ((Label)e.Row.FindControl("lblUAddress")).Text = "Yes";
                }
                Button pb = ((Button)e.Row.FindControl("pbAction"));
                pb.CommandArgument = ((int)DataBinder.Eval(e.Row.DataItem, "ID")).ToString();
            }
        }

        protected void pbAction_Click(object sender, EventArgs e)
        {
            lblActionID.Text = ((Button)sender).CommandArgument;
            MPEAction.Show();
        }
        protected void pbMPEAction_Click(object sender, EventArgs e)
        {
            Button Sender = (Button)sender;
            if (Sender.ID == "pbOKAction")
            {
                foreach (ListItem item in rblAction.Items)
                {
                    if (item.Selected)
                    {
                        switch (item.Text)
                        {
                            case "- Show Address":
                                int iPeopleContactID = Int32.Parse(lblActionID.Text);
                                string ss = "";
                                TNPV_PeopleContactsDataContext d0 = new TNPV_PeopleContactsDataContext();
                                var q0 = from n in d0.PEOPLECONTACTs where n.ID == iPeopleContactID select n.iPhysAddress;
                                int iPhysAddr = (int)q0.First();
                                if (iPhysAddr > 0)
                                {
                                    var q = from m in d0.PHYSADDRESSes where m.ID == iPhysAddr select m;
                                    var r = q.First();
                                    ss += r.sAddress1;
                                    ss += ", " + r.sAddress2;
                                    ss += ", " + r.sCity;
                                    ss += ", " + r.sStateProv;
                                    ss += ", " + r.sZipPostal;
                                    ss += ", " + r.sCountry;
                                }
                                else
                                {
                                    ss = "There is no physical address record associated with this contact type";
                                }
                                ProcessPopupException(new Global.excToPopup(ss));
                                break;
                            case "- Edit":
                                Session["PeopleContactID"] = lblActionID.Text;
                                Session["PeopleID"] = "0";
                                Response.Redirect("~/ClubMembership/CMS_ContactEdit.aspx");
                                break;
                            case "- Enable Member Filter":
                                iPeopleContactID = Int32.Parse(lblActionID.Text);
                                TNPV_PeopleContactsDataContext d1 = new TNPV_PeopleContactsDataContext();
                                var q1 = from n1 in d1.PEOPLECONTACTs
                                         where n1.ID == iPeopleContactID
                                         select n1.PEOPLE.sDisplayName;
                                MCFsettings.bFilterOn = true;
                                MCFsettings.sFilterMemberName = q1.First();
                                AccountProfile.CurrentUser.MCFsettings = MCFsettings;
                                break;
                            case "- Create New Entry for this Member":
                                TNPV_PeopleContactsDataContext d2 = new TNPV_PeopleContactsDataContext();
                                var q2 = from n2 in d2.PEOPLECONTACTs where n2.ID == Int32.Parse(lblActionID.Text) select n2.iPerson;
                                Session["PeopleID"] = q2.First().ToString();
                                Session["PeopleContactID"] = "0";
                                Response.Redirect("~/ClubMembership/CMS_ContactEdit.aspx");
                                break;
                            case "- Delete":
                                ButtonsClear();
                                YesButton.CommandName = "Delete";
                                YesButton.CommandArgument = lblActionID.Text;
                                OkButton.CommandArgument = "Contact";
                                lblPopupText.Text = "Please confirm deletion of member contact data with row ID '" + lblActionID.Text + "'";
                                MPE_Show(Global.enumButtons.NoYes);
                                break;
                        }
                        break;
                    }
                }
            }
        }

        protected void DDL_Member_SelectedIndexChanged(object sender, EventArgs e)
        {
            sFilterMemberName = DDL_Member.SelectedItem.Text;
            SetMemberFilter();
        }
        public string sDateTimeFormat(DateTime? uD)
        {
            if (uD == null)
            {
                return " ";
            }
            else
            {
                DateTime D = (DateTime)uD;
                return D.ToString("yyyy/MM/dd");
            }
        }
    }
}