using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TSoar.DB;
using TSoar.MemberPages.Stats;

namespace TSoar.ClubMembership
{
    public partial class CMS_ContactEdit : System.Web.UI.Page
    {
        private string[] sa; // holds the data for one row of table PEOPLECONTACTS
        private string[] saPA; // holds the data for one row of table PHYSADDRESSES
        private string[] saPAcontrols = { "txbAddress1", "txbAddress2", "txbCity", "txbState", "txbPostalCode", "txbCountry" }; // Names of physical address text boxes
        private string[] saPAtext = { "Address 1", "Address 2", "City", "State/Province", "Postal Code", "Country" };
        private int iPeopleContactID;
        private Global.strgMbrContactsFilter MCFsettings = new Global.strgMbrContactsFilter(true, "");
        SCUD_Multi mCRUD = new SCUD_Multi();

        public CMS_ContactEdit()
        {
            sa = new string[mCRUD.SaInfTyp[(int)Global.enugInfoType.Contacts].iSize];
            saPA = new string[mCRUD.SaInfTyp[(int)Global.enugInfoType.PhysicalAddresses].iSize];
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

        protected void dvCMS_Contact_PreRender(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                int iPeopleContactID = Int32.Parse((string)Session["PeopleContactID"]);
                if (iPeopleContactID == 0)
                {
                    dvCMS_Contact.AutoGenerateInsertButton = true;
                    dvCMS_Contact.AutoGenerateEditButton = false;
                    InitializeContactControls();
                }
                else
                {
                    dvCMS_Contact.AutoGenerateInsertButton = false;
                    dvCMS_Contact.AutoGenerateEditButton = true;
                    SetContactControls(iPeopleContactID);
                }
            }
        }
        private void InitializeContactControls()
        {
            TNPV_PeopleContactsDataContext d = new TNPV_PeopleContactsDataContext();
            int iPeoplID = Int32.Parse((string)Session["PeopleID"]);
            if (iPeoplID > 0)
            {
                var q0 = from n in d.PEOPLEs where n.ID == iPeoplID select n.sDisplayName;
                DropDownList DDL_Member = (DropDownList)dvCMS_Contact.FindControl("DDL_Member");
                SetDropDownByValue(DDL_Member, q0.First());
            }
            DropDownList ddl = (DropDownList)dvCMS_Contact.FindControl("DDL_ContactType");
            SetDropDownByValue(ddl, "Email Address");
            TextBox txb = (TextBox)dvCMS_Contact.FindControl("txb_CBegin");
            txb.Text = "";
            txb = (TextBox)dvCMS_Contact.FindControl("txb_CEnd");
            txb.Text = "";
            SetVisibility(false);
            txb = (TextBox)dvCMS_Contact.FindControl("txbContactInfo");
            txb.Text = "";
            txb = (TextBox)dvCMS_Contact.FindControl("txbPriorityRank");
            var q = from m in d.CONTACTTYPEs where m.sPeopleContactType == "Email Address" select m.dDefaultRank;
            txb.Text = q.First().ToString();
        }
        private void SetContactControls(int iuPeopleContactID)
        {
            TNPV_PeopleContactsDataContext d = new TNPV_PeopleContactsDataContext();
            var qm = from m in d.PEOPLECONTACTs where m.ID == iuPeopleContactID select m.PEOPLE.sDisplayName;
            DropDownList DDL_Member = (DropDownList)dvCMS_Contact.FindControl("DDL_Member");
            SetDropDownByValue(DDL_Member, qm.First());
            DDL_Member.Enabled = false;
            var qt = from n in d.PEOPLECONTACTs where n.ID == iuPeopleContactID select n;
            DropDownList DDL_ContactType = (DropDownList)dvCMS_Contact.FindControl("DDL_ContactType");
            var qq = qt.First();
            SetDropDownByValue(DDL_ContactType, qq.CONTACTTYPE.sPeopleContactType);
            DDL_ContactType.Enabled = false;
            TextBox txb = (TextBox)dvCMS_Contact.FindControl("txb_CBegin");
            string st = qq.DBegin.ToString();
            if (st.Length > 0)
            {
                txb.Text = st.Replace("/", "-").Substring(0, 10);
            }
            txb = (TextBox)dvCMS_Contact.FindControl("txb_CEnd");
            st = qq.DEnd.ToString();
            if (st.Length > 0)
            {
                txb.Text = st.Replace("/", "-").Substring(0, 10);
            }
            int iPhysAddr = (int) qq.iPhysAddress;
            if (iPhysAddr > 0)
            {
                SetVisibility(true);
                var qp = from p in d.PHYSADDRESSes where p.ID == iPhysAddr select p;
                foreach (var p in qp)
                {
                    DetailsView dv = (DetailsView)dvCMS_Contact.FindControl("dv_PhysAddr");
                    txb = (TextBox)dv.FindControl("txbAddress1");
                    txb.Text = p.sAddress1;
                    txb = (TextBox)dv.FindControl("txbAddress2");
                    txb.Text = p.sAddress2;
                    txb = (TextBox)dv.FindControl("txbCity");
                    txb.Text = p.sCity;
                    txb = (TextBox)dv.FindControl("txbState");
                    txb.Text = p.sStateProv;
                    txb = (TextBox)dv.FindControl("txbPostalCode");
                    txb.Text = p.sZipPostal;
                    txb = (TextBox)dv.FindControl("txbCountry");
                    txb.Text = p.sCountry;
                }
            }
            else
            {
                SetVisibility(false);
            }
            txb = (TextBox)dvCMS_Contact.FindControl("txbContactInfo");
            txb.Text = qq.sContactInfo;
            txb = (TextBox)dvCMS_Contact.FindControl("txbPriorityRank");
            txb.Text = qq.dContactPriorityRanking.ToString();
        }

        private void SetDropDownByValue(DropDownList ddl, string sText)
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

        protected void DDL_ContactType_SelectedIndexChanged(object sender, EventArgs e)
        {
            DropDownList ddl = (DropDownList)sender;
            TNPV_PeopleContactsDataContext d = new TNPV_PeopleContactsDataContext();
            var q0 = from c in d.CONTACTTYPEs where c.ID == Int32.Parse(ddl.SelectedValue) select c;
            SetVisibility(q0.First().bHasPhysAddr);
            TextBox txb = (TextBox)dvCMS_Contact.FindControl("txbPriorityRank");
            txb.Text = q0.First().dDefaultRank.ToString();
        }

        private void SetVisibility(bool bu)
        {
            DetailsView dvPA = (DetailsView)dvCMS_Contact.FindControl("dv_PhysAddr");
            Label lblReqE = (Label)dvCMS_Contact.FindControl("lblReqE");
            Label lblHPA = (Label)dvCMS_Contact.FindControl("lblHPA");
            Label lblContactInfo = (Label)dvCMS_Contact.FindControl("lblContactInfo");
            TextBox txb = (TextBox)dvCMS_Contact.FindControl("txbContactInfo");
            if (bu)
            {
                dvPA.Visible = true;
                lblReqE.Visible = true;
                lblHPA.Visible = true;
                lblContactInfo.Visible = false;
                txb.Visible = false;
            }
            else
            {
                dvPA.Visible = false;
                lblReqE.Visible = false;
                lblHPA.Visible = false;
                lblContactInfo.Visible = true;
                txb.Visible = true;
            }
        }

        private void ItemInserting()
        {
            Global.excToPopup exc;
            DropDownList ddl = (DropDownList)dvCMS_Contact.FindControl("DDL_ContactType");
            int iPhysAddr = 0;
            TNPV_PeopleContactsDataContext d = new TNPV_PeopleContactsDataContext();
            var q0 = from c in d.CONTACTTYPEs where c.ID == Int32.Parse(ddl.SelectedValue) select c.bHasPhysAddr;
            bool bPhysAddrRequired = q0.First();
            if (bPhysAddrRequired)
            {
                // A physical address is required
                saPA[(int)Global.egContactProps.PiT] = DateTime.UtcNow.ToString();
                saPA[(int)Global.egContactProps.EnteredBy] = mCRUD.GetPeopleIDfromWebSiteUserName(HttpContext.Current.User.Identity.Name).ToString();
                DetailsView dv_PhysAddr = (DetailsView)dvCMS_Contact.FindControl("dv_PhysAddr");
                for (int iRow = 0; iRow < dv_PhysAddr.Rows.Count; iRow++)
                {
                    saPA[iRow + 2] = Server.HtmlEncode(((TextBox)dv_PhysAddr.Rows[iRow].FindControl(saPAcontrols[iRow])).Text.Replace("'", "`"));
                    if (iRow != 1 && iRow != 4)
                    {
                        if (saPA[iRow + 2].Length < 1)
                        {

                            ProcessPopupException(new Global.excToPopup("Input Field `" + saPAtext[iRow] + "` must not be empty"));
                            return;
                        }
                    }
                }
                int jPhysAddr = 0;
                mCRUD.InsertOne(Global.enugInfoType.PhysicalAddresses, saPA, out jPhysAddr);
                iPhysAddr = jPhysAddr;
            }
            sa[(int)Global.egContactProps.PiT] = DateTime.UtcNow.ToString();
            sa[(int)Global.egContactProps.EnteredBy] = mCRUD.GetPeopleIDfromWebSiteUserName(HttpContext.Current.User.Identity.Name).ToString();
            sa[(int)Global.egContactProps.Member] = ((DropDownList)dvCMS_Contact.FindControl("DDL_Member")).SelectedValue.ToString();
            sa[(int)Global.egContactProps.ContactType] = ((DropDownList)dvCMS_Contact.FindControl("DDL_ContactType")).SelectedValue.ToString();
            sa[(int)Global.egContactProps.DBegin] = ((TextBox)dvCMS_Contact.FindControl("txb_CBegin")).Text.Trim();
            sa[(int)Global.egContactProps.DEnd] = ((TextBox)dvCMS_Contact.FindControl("txb_CEnd")).Text.Trim();
            sa[(int)Global.egContactProps.PhysAddr] = iPhysAddr.ToString();
            sa[(int)Global.egContactProps.ContactInfo] = Server.HtmlEncode(((TextBox)dvCMS_Contact.FindControl("txbContactInfo")).Text.Trim().Replace("'", "`"));
            sa[(int)Global.egContactProps.PriorityRank] = ((TextBox)dvCMS_Contact.FindControl("txbPriorityRank")).Text.Trim();
            // Validation
            if (!bIsValid()) return;
            // Insertion proper
            string sp;
            try
            {
                int iIdent = 0;
                mCRUD.InsertOne(Global.enugInfoType.Contacts, sa, out iIdent);
                iPeopleContactID = iIdent;
                sp = "Record Inserted: ";
                for (int i = 2; i < sa.Count(); i++)
                {
                    sp += ((i > 2) ? ", " : " ") + sa[i];
                }
                exc = new Global.excToPopup(sp); // not an exception - just using to display confirmation of insertion
                ProcessPopupException(exc);
            }
            catch (Global.excToPopup exc1)
            {
                ProcessPopupException(exc1);
            }
            MCFsettings.bFilterOn = true;
            MCFsettings.sFilterMemberName = ((DropDownList)dvCMS_Contact.FindControl("DDL_Member")).SelectedItem.Text;
            AccountProfile.CurrentUser.MCFsettings = MCFsettings;
            Response.Redirect("~/ClubMembership/CMS_Contacts.aspx");
        }

        private bool bIsValid()
        {
            string sp = "";
            DateTime DBegin = DateTime.MinValue;
            if (sa[(int)Global.egContactProps.DBegin].Length > 0)
            {
                DBegin = DateTime.Parse(sa[(int)Global.egContactProps.DBegin]);
            }
            DateTime DEnd = DateTime.MaxValue;
            if (sa[(int)Global.egContactProps.DEnd].Length > 0)
            {
                DEnd = DateTime.Parse(sa[(int)Global.egContactProps.DEnd]);
            }
            if (DEnd < DBegin)
            {
                sp = "'Valid Until' = " + DEnd.ToString() + " is earlier than 'Valid From' =" + DBegin.ToString();
                ProcessPopupException(new Global.excToPopup(sp));
                return false;
            }
            decimal dPrR = Decimal.Parse(sa[(int)Global.egContactProps.PriorityRank]);
            if (dPrR < 0.0M || dPrR > 100.0M)
            {
                sp = "Contact priority ranking must be between 0 and 100, but it is " + dPrR.ToString();
                ProcessPopupException(new Global.excToPopup(sp));
                return false;
            }
            return true;
        }

        private void ItemUpdating()
        {
            DropDownList ddl = (DropDownList)dvCMS_Contact.FindControl("DDL_ContactType");
            int iPhysAddr = 0;
            TNPV_PeopleContactsDataContext d = new TNPV_PeopleContactsDataContext();
            string sPeopleContactID = (string)Session["PeopleContactID"];
            var qp = from m in d.PEOPLECONTACTs where m.ID == Int32.Parse(sPeopleContactID) select m.iPhysAddress;
            iPhysAddr = (int)qp.First();
            if (iPhysAddr > 0)
            {
                // A physical address exists
                saPA[(int)Global.egContactProps.PiT] = DateTime.UtcNow.ToString();
                saPA[(int)Global.egContactProps.EnteredBy] = mCRUD.GetPeopleIDfromWebSiteUserName(HttpContext.Current.User.Identity.Name).ToString();
                DetailsView dv_PhysAddr = (DetailsView)dvCMS_Contact.FindControl("dv_PhysAddr");
                for (int iRow = 0; iRow < dv_PhysAddr.Rows.Count; iRow++)
                {
                    saPA[iRow + 2] = Server.HtmlEncode(((TextBox)dv_PhysAddr.Rows[iRow].FindControl(saPAcontrols[iRow])).Text.Replace("'", "`"));
                    if (iRow != 1 && iRow != 4)
                    {
                        if (saPA[iRow + 2].Length < 1)
                        {

                            ProcessPopupException(new Global.excToPopup("Input Field `" + saPAtext[iRow] + "` must not be empty"));
                            return;
                        }
                    }
                }
                mCRUD.UpdateOne(Global.enugInfoType.PhysicalAddresses, iPhysAddr.ToString(), saPA);
            }
            sa[(int)Global.egContactProps.PiT] = DateTime.UtcNow.ToString();
            sa[(int)Global.egContactProps.EnteredBy] = mCRUD.GetPeopleIDfromWebSiteUserName(HttpContext.Current.User.Identity.Name).ToString();
            sa[(int)Global.egContactProps.Member] = ((DropDownList)dvCMS_Contact.FindControl("DDL_Member")).SelectedValue.ToString();
            sa[(int)Global.egContactProps.ContactType] = ((DropDownList)dvCMS_Contact.FindControl("DDL_ContactType")).SelectedValue.ToString();
            sa[(int)Global.egContactProps.DBegin] = ((TextBox)dvCMS_Contact.FindControl("txb_CBegin")).Text.Trim();
            sa[(int)Global.egContactProps.DEnd] = ((TextBox)dvCMS_Contact.FindControl("txb_CEnd")).Text.Trim();
            sa[(int)Global.egContactProps.PhysAddr] = iPhysAddr.ToString();
            sa[(int)Global.egContactProps.ContactInfo] = Server.HtmlEncode(((TextBox)dvCMS_Contact.FindControl("txbContactInfo")).Text.Trim().Replace("'", "`"));
            sa[(int)Global.egContactProps.PriorityRank] = ((TextBox)dvCMS_Contact.FindControl("txbPriorityRank")).Text.Trim();
            // Validation
            if (!bIsValid()) return;
            // Update operation
            try
            {
                mCRUD.UpdateOne(Global.enugInfoType.Contacts, sPeopleContactID, sa);
            }
            catch (Global.excToPopup exc1)
            {
                ProcessPopupException(exc1);
            }
            MCFsettings.bFilterOn = true;
            MCFsettings.sFilterMemberName = ((DropDownList)dvCMS_Contact.FindControl("DDL_Member")).SelectedItem.Text;
            AccountProfile.CurrentUser.MCFsettings = MCFsettings;
            Response.Redirect("~/ClubMembership/CMS_Contacts.aspx");
        }

        protected void pbSave_Click(object sender, EventArgs e)
        {
            if (Int32.Parse((string)Session["PeopleContactID"]) < 1)
            {
                ItemInserting();
            }
            else
            {
                ItemUpdating();
            }
        }

        protected void pbCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/ClubMembership/CMS_Contacts.aspx");
        }
    }
}