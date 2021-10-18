using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TSoar.MemberPages.MyMembership
{
    public partial class MyMembership : System.Web.UI.Page
    {
        private Dictionary<char, string> dictDQual = new Dictionary<char, string> { { 'P', "aPproximate" }, { 'X', "eXact" }, { 'A', "After" }, { 'B', "Before" },
                    { 'G', "educated Guess" }, {'W', "Wild ass guess"} };
        private Dictionary<char, string> dictXType = new Dictionary<char, string> { { 'P', "Purchase by member" },{ 'S', "Sale by member" },{ 'D', "Donation by member" },
                    {'R',"Reinstatement after having donated" } };

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                    DisplayInGrid();
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
            //Button btn = (Button)sender;
        }
        private void ProcessPopupException(Global.excToPopup excu)
        {
            ButtonsClear();
            lblPopupText.Text = excu.sExcMsg();
            MPE_Show(Global.enumButtons.OkOnly);
        }
        #endregion

        private void DisplayInGrid()
        {
            string sDisplayName;
            MyMembershipDataContext d = new MyMembershipDataContext();

            try
            {
                var q0 = from m in d.PEOPLEs where m.sUserName == HttpContext.Current.User.Identity.Name select m.sDisplayName;
                sDisplayName = q0.First();
            }
            catch (Exception exc)
            {
                ProcessPopupException(new Global.excToPopup("MyMembership.aspx.cs.DisplayInGrid: Could not find a row in table PEOPLE where sUserName is = Current.User.Identity.Name = `" +
                    HttpContext.Current.User.Identity.Name + "`. Exception message text: " + exc.Message));
                return;
            }

            try
            {
                var qClubMembership = from m in d.MEMBERFROMTOs
                                      where m.PEOPLE.sDisplayName == sDisplayName
                                      orderby m.DMembershipBegin
                                      select new
                                      {
                                          Name = m.PEOPLE.sDisplayName,
                                          Category = m.MEMBERSHIPCATEGORy.sMembershipCategory,
                                          Begin = m.DMembershipBegin,
                                          End = m.DMembershipEnd,
                                          Note = m.sAdditionalInfo
                                      };
                gvClubMembership.DataSource = qClubMembership;
                gvClubMembership.DataBind();
            }catch (Exception exc)
            {
                ProcessPopupException(new Global.excToPopup("MyMembership.aspx.cs.DisplayInGrid: ClubMembership - " + exc.Message));
                return;
            }

            try
            {
                // Could not make the following commented out Linq code work; kept running into 'cast is not valid' error on DMembershipBegin
                //var qSSAMembership = (from m in d.SSA_MEMBERFROMTOs
                //                     where m.PEOPLE.sDisplayName == sDisplayName
                //                     orderby m.DMembershipBegin
                //                     select new
                //                     {
                //                         Name = m.PEOPLE.sDisplayName,
                //                         SSA_ID = m.iSSA_ID,
                //                         Category = m.SSA_MEMBERCATEGORy.sSSA_MemberCategory,
                //                         Begin = m.DMembershipBegin
                //                         End = m.DMembershipEnd,
                //                         Expires = m.DMembershipExpires,
                //                         RenewsWithChapter = m.bRenewsWithChapter ? "Yes" : "No",
                //                         ChapterAffiliation = m.sChapterAffiliation,
                //                         Notes = m.sAdditionalInfo
                //                     }).ToList();
                //gvSSAMembership.DataSource = qSSAMembership;
                //gvSSAMembership.DataBind();
                using (DataTable dt = new DataTable())
                {
                    using (SqlConnection SqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlConn"].ToString()))
                    {
                        using (SqlCommand cmd = new SqlCommand("SELECT P.sDisplayName AS [Name], M.iSSA_ID AS SSA_ID, C.sSSA_MemberCategory AS Category, M.DMembershipBegin AS [Begin], M.DMembershipEnd AS [End], " +
                             "M.DMembershipExpires AS Expires, M.bRenewsWithChapter RenewsWithChapter, M.sChapterAffiliation AS ChapterAffiliation, M.sAdditionalInfo AS Notes " +
                             "FROM PEOPLE P INNER JOIN " +
                             "SSA_MEMBERFROMTO M ON P.ID = M.iPerson INNER JOIN " +
                             "SSA_MEMBERCATEGORIES C ON M.iSSA_MemberCategory = C.ID " +
                             "WHERE(P.sDisplayName = N'" + sDisplayName + "')"))
                        {
                            using (SqlDataAdapter sda = new SqlDataAdapter())
                            {
                                cmd.CommandType = CommandType.Text;
                                cmd.Connection = SqlConn;
                                sda.SelectCommand = cmd;
                                sda.Fill(dt);
                            }
                        }
                    }
                    gvSSAMembership.DataSource = dt;
                    gvSSAMembership.DataBind();
                }
            }
            catch (Exception exc)
            {
                ProcessPopupException(new Global.excToPopup("MyMembership.aspx.cs.DisplayInGrid: SSAMembership - " + exc.Message));
                return;
            }

            try
            {
                var qContacts = from m in d.PEOPLECONTACTs
                                where m.PEOPLE.sDisplayName == sDisplayName && !m.CONTACTTYPE.bHasPhysAddr
                                orderby m.CONTACTTYPE.sPeopleContactType, m.dContactPriorityRanking, m.DBegin
                                select new
                                {
                                    Name = m.PEOPLE.sDisplayName,
                                    ContactType = m.CONTACTTYPE.sPeopleContactType,
                                    Begin = m.DBegin,
                                    End = m.DEnd,
                                    Info = m.sContactInfo
                                };
                gvContacts.DataSource = qContacts;
                gvContacts.DataBind();
            }
            catch (Exception exc)
            {
                ProcessPopupException(new Global.excToPopup("MyMembership.aspx.cs.DisplayInGrid: Contacts - " + exc.Message));
                return;
            }

            try
            {
                var qPhysAddr = from m in d.PEOPLECONTACTs
                                join p in d.PHYSADDRESSes on m.iPhysAddress equals p.ID into PAs
                                from p in PAs
                                where m.PEOPLE.sDisplayName == sDisplayName && m.CONTACTTYPE.bHasPhysAddr
                                orderby m.CONTACTTYPE.sPeopleContactType, m.dContactPriorityRanking, m.DBegin
                                select new
                                {
                                    Name = m.PEOPLE.sDisplayName,
                                    ContactType = m.CONTACTTYPE.sPeopleContactType,
                                    Begin = m.DBegin,
                                    End = m.DEnd,
                                    Address_1 = p.sAddress1,
                                    Address_2 = p.sAddress2,
                                    City = p.sCity,
                                    State_Prov = p.sStateProv,
                                    PostalCode = p.sZipPostal,
                                    Country = p.sCountry
                                };
                gvPhysAddr.DataSource = qPhysAddr;
                gvPhysAddr.DataBind();
            }
            catch (Exception exc)
            {
                ProcessPopupException(new Global.excToPopup("MyMembership.aspx.cs.DisplayInGrid: Physical Addresses - " + exc.Message));
                return;
            }

            try
            {
                var qCertifics = from m in d.PEOPLECERTIFICs
                                 where (m.PEOPLE.sDisplayName == sDisplayName && m.CERTIFICATION.sCertification != "None")
                                 select new
                                 {
                                     Name = sDisplayName,
                                     Q_R = "Certification",
                                     Qu_Rat = m.CERTIFICATION.sCertification,
                                     Since = m.DSince,
                                     Expires = m.DExpiry,
                                     Notes = m.sAdditionalInfo
                                 };
                gvCertifics.DataSource = qCertifics;
                gvCertifics.DataBind();
            }
            catch (Exception exc)
            {
                ProcessPopupException(new Global.excToPopup("MyMembership.aspx.cs.DisplayInGrid: Certifications - " + exc.Message));
                return;
            }

            try
            {
                var qRatings = from m in d.PEOPLERATINGs
                               where (m.PEOPLE.sDisplayName == sDisplayName && m.RATING.sRating != "None")
                               select new
                               {
                                   Name = sDisplayName,
                                   Q_R = "Rating",
                                   Qu_Rat = m.RATING.sRating,
                                   Since = m.DSince,
                                   Expires = m.DExpiry,
                                   Notes = m.sAdditionalInfo
                               };
                gvRatings.DataSource = qRatings;
                gvRatings.DataBind();
            }
            catch (Exception exc)
            {
                ProcessPopupException(new Global.excToPopup("MyMembership.aspx.cs.DisplayInGrid: Ratings - " + exc.Message));
                return;
            }

            try
            {
                var qQualifics = from m in d.PEOPLEQUALIFICs
                                 where (m.PEOPLE.sDisplayName == sDisplayName && m.QUALIFICATION.sQualification != "None")
                                 select new
                                 {
                                     Name = sDisplayName,
                                     Q_R = "Qualification",
                                     Qu_Rat = m.QUALIFICATION.sQualification,
                                     Since = m.DSince,
                                     Expires = m.DExpiry,
                                     Notes = m.sAdditionalInfo
                                 };
                gvQualifics.DataSource = qQualifics;
                gvQualifics.DataBind();
            }
            catch (Exception exc)
            {
                ProcessPopupException(new Global.excToPopup("MyMembership.aspx.cs.DisplayInGrid: Qualifications - " + exc.Message));
                return;
            }

            try
            {
                var qBoardPos = from m in d.PEOPLEOFFICEs
                                where m.PEOPLE.sDisplayName == sDisplayName
                                orderby m.DOfficeBegin
                                select new
                                {
                                    Name = m.PEOPLE.sDisplayName,
                                    Office = m.BOARDOFFICE.sBoardOffice,
                                    Begin = m.DOfficeBegin,
                                    End = m.DOfficeEnd,
                                    Notes = m.sAdditionalInfo
                                };
                gvBoardPos.DataSource = qBoardPos;
                gvBoardPos.DataBind();
            }
            catch (Exception exc)
            {
                ProcessPopupException(new Global.excToPopup("MyMembership.aspx.cs.DisplayInGrid: BoardPos - " + exc.Message));
                return;
            }

            try
            {
                var qEquShares = from m in d.EQUITYSHAREs
                                 where m.PEOPLE.sDisplayName == sDisplayName
                                 orderby m.DXaction
                                 select new
                                 {
                                     Date = m.DXaction,
                                     Qual = dictDQual[m.cDateQuality],
                                     NumberOfShares = m.dNumShares,
                                     Type=dictXType[m.cXactType],
                                     Info=m.sInfoSource,
                                     Notes = m.sComment
                                 };
                gvEquityShares.DataSource = qEquShares;
                gvEquityShares.DataBind();
            }
            catch (Exception exc)
            {
                ProcessPopupException(new Global.excToPopup("MyMembership.aspx.cs.DisplayInGrid: EquShares - " + exc.Message));
                return;
            }
            try
            {
                lblSum.Text = (from s in d.EQUITYSHAREs
                          where s.PEOPLE.sDisplayName == sDisplayName
                          select s.dNumShares).Sum().ToString();
            }
            catch (Exception exc)
            {
                if (exc.Message == "The null value cannot be assigned to a member with type System.Decimal which is a non-nullable value type.")
                {
                    lblSum.Text = "0.0000";
                }
                else
                {
                    ProcessPopupException(new Global.excToPopup("MyMembership.aspx.cs.DisplayInGrid: Sum of EquShares - " + exc.Message));
                    return;
                }
            }
        }
    }
}