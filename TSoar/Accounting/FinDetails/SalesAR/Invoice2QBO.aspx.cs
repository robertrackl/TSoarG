using System;
using System.Collections.Generic;
using System.Net;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Data;
using System.Web;
using Intuit.Ipp.OAuth2PlatformClient;
using System.Threading.Tasks;
using Intuit.Ipp.Core;
using Intuit.Ipp.Data;
using Intuit.Ipp.DataService;
using Intuit.Ipp.LinqExtender;
using Intuit.Ipp.QueryFilter;
using Intuit.Ipp.Security;
using Intuit.Ipp.Exception;
using System.Linq;
using Intuit.Ipp.ReportService;

namespace TSoar.Accounting.FinDetails.SalesAR
{
    public enum FlAcInvoiceStatus { New, InProcess, Done}
    public class clFlAcInvoice
    {
        public FlAcInvoiceStatus flAcInvoiceStatus; 
        public string sCustomerId;
        public string sDescription;
        public decimal dAmount;

        public clFlAcInvoice(FlAcInvoiceStatus euf)
        {
            flAcInvoiceStatus = euf;
            sCustomerId = "";
            sDescription = "";
            dAmount = 0.0M;
        }
    }

    public partial class Invoice2QBO : System.Web.UI.Page
    {
        #region Definitions

        private clFlAcInvoice lFlAcInvoice = new clFlAcInvoice(FlAcInvoiceStatus.Done); // local instance

        // OAuth2 client configuration
        static string clientID = ConfigurationManager.AppSettings["clientID"];
        static string clientSecret = ConfigurationManager.AppSettings["clientSecret"];
        static string logPath = ConfigurationManager.AppSettings["logPath"];
        static string appEnvironment = ConfigurationManager.AppSettings["appEnvironment"];

        private string authCode { get { return (string)Session["authCode"] ?? ""; } set { Session["authCode"] = value; } }
        private string idToken { get { return (string)Session["idToken"] ?? ""; } set { Session["idToken"] = value; } }

        public Dictionary<string, string> dictTokens
        {
            get { return GetDict("dictTokens"); }
            set { Session["dictTokens"] = value; }
        }
        private Dictionary<string, string> GetDict(string sudict)
        {
            if (Session[sudict] == null)
            {
                Session[sudict] = new Dictionary<string, string>();
            }
            return (Dictionary<string, string>)Session[sudict];
        }

        private OAuth2Client CreateOAuth2Client()
        {
            var redirectUri = $"{Request.Url.Scheme}://{Request.Url.Host}:{Request.Url.Port}/Accounting/FinDetails/SalesAR/Invoice2QBO.aspx";
            OAuth2Client client = new OAuth2Client(clientID, clientSecret, redirectUri, appEnvironment);
            if (Session["oAuthCSRFToken"] is string csrfToken)
            {
                client.CSRFToken = csrfToken;
            }
            else
            {
                Session["oAuthCSRFToken"] = client.CSRFToken = client.GenerateCSRFToken();
            }
            return client;
        }

        private static OAuth2Client _oauth2Client;
        //private OAuth2Client oauthClient
        //{
        //    get { return (OAuth2Client)Session["oauthClient"] ?? CreateOAuth2Client(); }
        //    set { Session["oauthClient"] = value; }
        //}
        private OAuth2Client oauthClient => _oauth2Client ?? (_oauth2Client = CreateOAuth2Client());

        #endregion Definitions

        #region Page Events

        protected void Page_Load(object sender, EventArgs e)
        {
            AsyncMode = true;
            if (!dictTokens.ContainsKey("accessToken"))
            {
                mainButtons.Visible = true;
                connected.Visible = false;
                ActivityLog.oLog(ActivityLog.enumLogTypes.Debug, 20, "Page_Load: no access token");
                if (Request.QueryString.Count > 0)
                {
                    ActivityLog.oLog(ActivityLog.enumLogTypes.Debug, 21, "Page_Load: Request.QueryString.Count > 0");
                    var response = new AuthorizeResponse(Request.QueryString.ToString());
                    if (response.State != null)
                    {
                        ActivityLog.oLog(ActivityLog.enumLogTypes.Debug, 22, "Page_Load: no access token");
                        if (oauthClient.CSRFToken == response.State)
                        {
                            ActivityLog.oLog(ActivityLog.enumLogTypes.Debug, 23, "Page_Load: oauthClient.CSRFToken == response.State");
                            if (response.RealmId != null)
                            {
                                ActivityLog.oLog(ActivityLog.enumLogTypes.Debug, 24, "Page_Load: response.RealmId != null");
                                if (!dictTokens.ContainsKey("realmId"))
                                {
                                    ActivityLog.oLog(ActivityLog.enumLogTypes.Debug, 25, "Page_Load: !dictTokens.ContainsKey(realmId)");
                                    dictTokens.Add("realmId", response.RealmId);
                                }
                            }

                            if (response.Code != null)
                            {
                                authCode = response.Code;
                                ActivityLog.oLog(ActivityLog.enumLogTypes.Debug, 24, "Page_Load: Authorization code obtained; -> performCodeExchange");
                                PageAsyncTask t = new PageAsyncTask(performCodeExchange);
                                Page.RegisterAsyncTask(t);
                                Page.ExecuteRegisteredAsyncTasks();
                            }
                        }
                        else
                        {
                            ActivityLog.oLog(ActivityLog.enumLogTypes.Debug, 23, "Page_Load: Invalid State");
                            dictTokens.Clear();
                        }
                    }
                }
            }
            else
            {
                mainButtons.Visible = false;
                connected.Visible = true;
                if (Session["FlAcInvoice"] != null)
                {
                    lFlAcInvoice = (clFlAcInvoice)Session["FlAcInvoice"];
                }
                switch (lFlAcInvoice.flAcInvoiceStatus)
                {
                    case FlAcInvoiceStatus.Done: return;
                    case FlAcInvoiceStatus.InProcess:
                        ProcessPopupException(new Global.excToPopup("Invoice2QBO.aspx.cs.Page_Load: lFlAcInvoice.flAcInvoiceStatus = FlAcInvoiceStatus.InProcess is illegal"));
                        return;
                    case FlAcInvoiceStatus.New:
                        break;
                }
                genInvoice(lFlAcInvoice);
                Response.Redirect("FlyActInvoice.aspx");
            }
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            lblAccessToken.Text = dictTokens.ContainsKey("accessToken") ? dictTokens["accessToken"].Length.ToString() : "-0-";
            lblRealmId.Text = dictTokens.ContainsKey("realmId") ? dictTokens["realmId"].Length.ToString() : "-0-";
            lblRefreshToken.Text = dictTokens.ContainsKey("refreshToken") ? dictTokens["refreshToken"].Length.ToString() : "-0-";
            int iNdict = -1;
            if ((Dictionary<string, string>)Session["dictTokens"] != null)
            {
                iNdict = ((Dictionary<string, string>)Session["dictTokens"]).Count;
            }
            lbldictTokens.Text = iNdict.ToString();
            lbloAuthCSRFToken.Text = ((string)Session["oAuthCSRFToken"] != null) ? "-1-" : "-0-";
            ActivityLog.oLog(ActivityLog.enumLogTypes.Debug, 20, "Page_PreRender: accessToken:" + lblAccessToken.Text + "; realmId:" + lblRealmId.Text + "; refreshToken:" + lblRefreshToken.Text + "; dictTokens:" + lbldictTokens.Text + "; oAuthCSRFToken:" + lbloAuthCSRFToken.Text);
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

        #endregion Page Events

        #region button click events

        protected void ImgGetAppNow_Click(object sender, ImageClickEventArgs e)
        {
            ActivityLog.oLog(ActivityLog.enumLogTypes.Debug, 20, "ImgGetAppNow_Click: Initiating Get App Now call.");
            try
            {
                if (!dictTokens.ContainsKey("accessToken"))
                {
                    List<OidcScopes> scopes = new List<OidcScopes>();
                    scopes.Add(OidcScopes.Accounting);
                    scopes.Add(OidcScopes.OpenId);
                    scopes.Add(OidcScopes.Phone);
                    scopes.Add(OidcScopes.Profile);
                    scopes.Add(OidcScopes.Address);
                    scopes.Add(OidcScopes.Email);

                    var authorizationRequest = oauthClient.GetAuthorizationURL(scopes);
                    ActivityLog.oLog(ActivityLog.enumLogTypes.Debug, 21, "ImgGetAppNow_Click: authorizationRequest=" + authorizationRequest);
                    Response.Redirect(authorizationRequest, "_blank", "menubar=0,scrollbars=1,width=780,height=900,top=10");
                }
            }
            catch (Exception ex)
            {
                ActivityLog.oLog(ActivityLog.enumLogTypes.Debug, 20, "ImgGetAppNow_Click: " + ex.Message);
            }
        }

        protected async void btnQBOAPICall_Click(object sender, EventArgs e)
        {
            if (dictTokens.ContainsKey("accessToken") && dictTokens.ContainsKey("realmId"))
            {
                await QboApiCall();
            }
            else
            {
                ActivityLog.oLog(ActivityLog.enumLogTypes.Debug, 21, "btnQBOAPICall_Click: Access token not found.");
                lblQBOCall.Visible = true;
                lblQBOCall.Text = "btnQBOAPICall_Click: Access token not found.";
            }
        }

        protected async void btnUserInfo_Click(object sender, EventArgs e)
        {
            if (idToken != null)
            {
                var userInfoResp = await oauthClient.GetUserInfoAsync(dictTokens["accessToken"]);
                lblUserInfo.Visible = true;
                lblUserInfo.Text = userInfoResp.Raw;
                ActivityLog.oLog(ActivityLog.enumLogTypes.Debug, 21, "btnUserInfo_Click: lblUserInfo.Text=" + lblUserInfo.Text);
            }
            else
            {
                lblUserInfo.Visible = true;
                lblUserInfo.Text = "UserInfo call is available through OpenId/GetAppNow flow first.";
                ActivityLog.oLog(ActivityLog.enumLogTypes.Debug, 21, "btnUserInfo_Click: Go through OpenId flow first.");
            }
        }

        protected async void btnRefresh_Click(object sender, EventArgs e)
        {
            if ((dictTokens.ContainsKey("accessToken")) && (dictTokens.ContainsKey("refreshToken")))
            {
                ActivityLog.oLog(ActivityLog.enumLogTypes.Debug, 21, "btnRefresh_Click: Exchanging refresh token for access token.");
                var tokenResp = await oauthClient.RefreshTokenAsync(dictTokens["refreshToken"]);
            }
        }

        protected async void btnRevoke_Click(object sender, EventArgs e)
        {
            ActivityLog.oLog(ActivityLog.enumLogTypes.Debug, 20, "btnRevoke_Click: Performing Revoke tokens.");
            if ((dictTokens.ContainsKey("accessToken")) && (dictTokens.ContainsKey("refreshToken")))
            {
                var revokeTokenResp = await oauthClient.RevokeTokenAsync(dictTokens["refreshToken"]);
                if (revokeTokenResp.HttpStatusCode == HttpStatusCode.OK)
                {
                    dictTokens.Clear();
                    if (Request.Url.Query == "")
                        Response.Redirect(Request.RawUrl);
                    else
                        Response.Redirect(Request.RawUrl.Replace(Request.Url.Query, ""));
                }
                ActivityLog.oLog(ActivityLog.enumLogTypes.Debug, 21, "btnRevoke_Click: Token revoked.");
            }
        }
        #endregion

        public async System.Threading.Tasks.Task performCodeExchange()
        {
            ActivityLog.oLog(ActivityLog.enumLogTypes.Debug, 20, "performCodeExchange: Exchanging code for tokens.");
            try
            {
                var tokenResp = await oauthClient.GetBearerTokenAsync(authCode);
                if (!dictTokens.ContainsKey("accessToken"))
                    dictTokens.Add("accessToken", tokenResp.AccessToken);
                else
                    dictTokens["accessToken"] = tokenResp.AccessToken;

                if (!dictTokens.ContainsKey("refreshToken"))
                    dictTokens.Add("refreshToken", tokenResp.RefreshToken);
                else
                    dictTokens["refreshToken"] = tokenResp.RefreshToken;

                if (tokenResp.IdentityToken != null)
                    idToken = tokenResp.IdentityToken;
                if (Request.Url.Query == "")
                {
                    Response.Redirect(Request.RawUrl);
                }
                else
                {
                    Response.Redirect(Request.RawUrl.Replace(Request.Url.Query, ""));
                }
            }
            catch (Exception ex)
            {
                ActivityLog.oLog(ActivityLog.enumLogTypes.Debug, 21, "performCodeExchange: Problem while getting bearer tokens: " + ex.Message);
            }
        }

        public async System.Threading.Tasks.Task QboApiCall()
        {
            try
            {
                if ((dictTokens.ContainsKey("accessToken")) && (dictTokens.ContainsKey("realmId")))
                {
                    ActivityLog.oLog(ActivityLog.enumLogTypes.Debug, 20, "QboApiCall: Making QBO API Call.");
                    OAuth2RequestValidator oauthValidator = new OAuth2RequestValidator(dictTokens["accessToken"]);
                    ServiceContext serviceContext = new ServiceContext(dictTokens["realmId"], IntuitServicesType.QBO, oauthValidator);
                    serviceContext.IppConfiguration.BaseUrl.Qbo = "https://sandbox-quickbooks.api.intuit.com/";
                    //serviceContext.IppConfiguration.BaseUrl.Qbo = "https://quickbooks.api.intuit.com/";//prod
                    serviceContext.IppConfiguration.MinorVersion.Qbo = "29";
                    ReportService reportService = new ReportService(serviceContext);

                    //Date should be in the format YYYY-MM-DD 
                    //Response format should be JSON as that is the only supported right now for reports 
                    reportService.accounting_method = "Accrual";
                    reportService.start_date = "2018-01-01";
                    reportService.end_date = "2018-07-01";
                    ////reportService.classid = "2800000000000634813"; 
                    //reportService.date_macro = "Last Month"; 
                    reportService.summarize_column_by = "Month";


                    //List<String> columndata = new List<String>();
                    //columndata.Add("tx_date");
                    //columndata.Add("dept_name");
                    //string coldata = String.Join(",", columndata);
                    //reportService.columns = coldata;

                    var report1 = reportService.ExecuteReport("TrialBalance");

                    DataService commonServiceQBO = new DataService(serviceContext);
                    //Item item = new Item();
                    //List<Item> results = commonServiceQBO.FindAll<Item>(item, 1, 1).ToList<Item>();
                    QueryService<Invoice> inService = new QueryService<Invoice>(serviceContext);
                    int In = inService.ExecuteIdsQuery("SELECT count(*) FROM Invoice").Count();
                    lblNumInv.Text = In.ToString();
                    serviceContext.RequestId = Helper.GetGuid();
                    List<Invoice> returnedInvoices = Helper.FindAll<Invoice>(serviceContext, new Invoice());
                    DataTable dtinv = new DataTable();
                    DataRow dr = null;
                    dtinv.Columns.Add("Id");
                    dtinv.Columns.Add("DueDate");
                    dtinv.Columns.Add("Balance");
                    dtinv.Columns.Add("NameAndId");
                    dtinv.Columns.Add("status");
                    dtinv.Columns.Add("TotalAmt");
                    foreach (Invoice iv in returnedInvoices)
                    {
                        dr = dtinv.NewRow();
                        dr[0] = iv.Id;
                        dr[1] = iv.DueDate;
                        dr[2] = iv.Balance;
                        dr[3] = iv.NameAndId;
                        dr[4] = iv.status;
                        dr[5] = iv.TotalAmt;
                        dtinv.Rows.Add(dr);
                    }
                    gvInvList.DataSource = dtinv;
                    gvInvList.DataBind();

                    serviceContext.RequestId = Helper.GetGuid();
                    List<Customer> returnedCustomers = Helper.FindAll<Customer>(serviceContext, new Customer(), 1, 500);
                    DataTable dtpr = new DataTable();
                    dtpr.Columns.Add("Company Name");
                    dtpr.Columns.Add("Display Name");
                    dtpr.Columns.Add("Family Name");
                    dtpr.Columns.Add("Id");
                    dtpr.Columns.Add("Balance");
                    dtpr.Columns.Add("IntuitId");
                    dtpr.Columns.Add("UserId");
                    foreach (Customer c in returnedCustomers)
                    {
                        dr = dtpr.NewRow();
                        dr[0] = c.CompanyName;
                        dr[1] = c.DisplayName;
                        dr[2] = c.FamilyName;
                        dr[3] = c.Id;
                        dr[4] = c.Balance;
                        dr[5] = c.IntuitId;
                        dr[6] = c.UserId;
                        dtpr.Rows.Add(dr);
                    }
                    gvCustList.DataSource = dtpr;
                    gvCustList.DataBind();

                    Batch batch = commonServiceQBO.CreateNewBatch();


                    batch.Add("select count(*) from Account", "queryAccount");
                    batch.Execute();

                    if (batch.IntuitBatchItemResponses != null && batch.IntuitBatchItemResponses.Count() > 0)
                    {
                        ActivityLog.oLog(ActivityLog.enumLogTypes.Debug, 21, "batch.IntuitBatchItemResponses.Count()=" + batch.IntuitBatchItemResponses.Count().ToString());
                        IntuitBatchResponse res = batch.IntuitBatchItemResponses.FirstOrDefault();
                        List<Account> acc = res.Entities.ToList().ConvertAll(item => item as Account);
                    };
                    ActivityLog.oLog(ActivityLog.enumLogTypes.Debug, 20, "QboApiCall: QBO call successful.");
                    lblQBOCall.Visible = true;
                    lblQBOCall.Text = "QboApiCall: QBO Call successful";
                }
            }
            catch (IdsException ex)
            {
                if (ex.Message == "Unauthorized-401")
                {
                    ActivityLog.oLog(ActivityLog.enumLogTypes.Debug, 21, "QboApiCall: Invalid/Expired Access Token.");

                    var tokenResp = await oauthClient.RefreshTokenAsync(dictTokens["refreshToken"]);
                    if (tokenResp.AccessToken != null && tokenResp.RefreshToken != null)
                    {
                        dictTokens["accessToken"] = tokenResp.AccessToken;
                        dictTokens["refreshToken"] = tokenResp.RefreshToken;
                        await QboApiCall();
                    }
                    else
                    {
                        ActivityLog.oLog(ActivityLog.enumLogTypes.Debug, 22, "QboApiCall: Error while refreshing tokens: " + tokenResp.Raw);
                    }
                }
                else
                {
                    ActivityLog.oLog(ActivityLog.enumLogTypes.Debug, 21, "QboApiCall: " + ex.Message);
                }
            }
            catch (Exception ex)
            {
                ActivityLog.oLog(ActivityLog.enumLogTypes.Debug, 20, "QboApiCall: Invalid/Expired Access Token.");
            }
        }

        protected void pbInvoice_Click(object sender, EventArgs e)
        {
            genTInvoice();
        }

        private void genTInvoice()
        {
            lblInvoiceSentYesNo.Text = "NO";
            // Copied in part from:
            //   https://developer.intuit.com/app/developer/qbo/docs/concepts/invoicing#how-to-implement

            // Step 1: Compose serviceContext
            ActivityLog.oLog(ActivityLog.enumLogTypes.Debug, 20, "pbInvoice_Click: Entry");
            OAuth2RequestValidator oauthValidator = new OAuth2RequestValidator(dictTokens["accessToken"]);
            ServiceContext serviceContext = new ServiceContext(dictTokens["realmId"], IntuitServicesType.QBO, oauthValidator);
            serviceContext.IppConfiguration.BaseUrl.Qbo = "https://sandbox-quickbooks.api.intuit.com/";
            //serviceContext.IppConfiguration.BaseUrl.Qbo = "https://quickbooks.api.intuit.com/";//prod
            serviceContext.IppConfiguration.MinorVersion.Qbo = "29";

            //customerTest.CustomerFindbyIdTestUsingoAuth(qboContextoAuth);

            // Step 2: Create a customer object and put the customer of interest into it
            Customer customer = QBOHelper.CreateCustomer(serviceContext);
            customer.Id = "29";
            Customer custFound = Helper.FindById<Customer>(serviceContext, customer);

            // Step 3: Create a US Invoice object
            //TaxCode taxCode = Helper.FindOrAdd<TaxCode>(serviceContext, new TaxCode());
            //Account account = Helper.FindOrAddAccount(serviceContext, AccountTypeEnum.AccountsReceivable, AccountClassificationEnum.Liability);


            //Item item = Helper.FindOrAddItem(serviceContext, ItemTypeEnum.Service);
            Item item;
            try
            {
                item = FindByNameItem(serviceContext, ItemTypeEnum.Service, "Flying Charges");
            }
            catch (Global.excToPopup etp)
            {
                ProcessPopupException(etp);
                return;
            }
            Invoice invoice = new Invoice();
            //invoice.Deposit = 0.0M;
            invoice.DepositSpecified = false;

            // Step 4: Attach the customer to the invoice
            invoice.CustomerRef = new ReferenceType()
            {
                name = custFound.DisplayName,
                Value = custFound.Id
            };

            // Step 5: define miscellaneous data to go with the invoice
            invoice.DueDate = DateTime.UtcNow.Date;
            invoice.DueDate = invoice.DueDate.AddDays(15);
            invoice.DueDateSpecified = true;

            //invoice.TotalAmt = new Decimal(101.01);
            invoice.TotalAmtSpecified = false;

            //invoice.Balance = new Decimal(101.01);
            invoice.BalanceSpecified = false;

            invoice.TxnDate = DateTime.UtcNow.Date.AddDays(-1);
            invoice.TxnDateSpecified = true;

            // Step 6: Create the list of line items
            List<Line> lineList = new List<Line>();

            // Step 7: Create at least one line item
            Line line = new Line();
            //line.LineNum = "LineNum";
            line.Description = "Tow to 1017 ft AGL";
            //line.Amount = new Decimal(101.02);
            //line.AmountSpecified = true;
            /*
            Intuit.Ipp.Data.Class myClass = new Class();
            myClass.Id = "5000000000000031049";
            myClass.Name = "PW - 6";
            line.AnyIntuitObject = myClass; */

            //line.DetailType = LineDetailTypeEnum.DescriptionOnly;
            line.DetailType = LineDetailTypeEnum.SalesItemLineDetail;
            line.DetailTypeSpecified = true;
            line.Amount = 101.17M;
            line.AmountSpecified = true;
            line.AnyIntuitObject = new SalesItemLineDetail()
            {
                Qty = 1,
                QtySpecified = true,

                //AnyIntuitObject = 101.10M,
                //ItemElementName = ItemChoiceType.UnitPrice,

                ItemRef = new ReferenceType()
                {
                    name = item.Name,
                    Value = item.Id
                },
                //TaxCodeRef = new ReferenceType()
                //{
                //    name = taxCode.Name,
                //    Value = taxCode.Id //US has TAX or NON. Global needs actual TaxCode Id
                //},
            };

            // Step 8: Add the line item to the list of line items
            lineList.Add(line);
            // Step 9: Attach the line item list to the invoice
            invoice.Line = lineList.ToArray();

            // Step 10: Add sales tax data
            invoice.TxnTaxDetail = new TxnTaxDetail()
            {
                TotalTax = 0.0M,
                TotalTaxSpecified = true
            };

            // Step 11: Transmit the invoice
            DataService service = new DataService(serviceContext);
            try
            {
                Invoice addedInvoice = service.Add<Invoice>(invoice);
            }
            catch (Intuit.Ipp.Exception.IdsException IdsExc)
            {
                ProcessPopupException(new Global.excToPopup("Inv2Qbo.aspx.cs.genInvoice: IdsException Message=" + IdsExc.Message + ", inner exception message=" + IdsExc.InnerException.Message));
            }
            catch (Exception exc)
            {
                ProcessPopupException(new Global.excToPopup("Inv2Qbo.aspx.cs.genInvoice: general exception, Message=" + exc.Message));
            }
            lblInvoiceSentYesNo.Text = "YES";
            ActivityLog.oLog(ActivityLog.enumLogTypes.Debug, 20, "Inv2Qbo.aspx.cs.genInvoice: Exit");
        }


        private void genInvoice(clFlAcInvoice uFlAcInvoice)
        {
            uFlAcInvoice.flAcInvoiceStatus = FlAcInvoiceStatus.InProcess;
            Session["FlAcInvoice"] = uFlAcInvoice;
            lblInvoiceSentYesNo.Text = "NO";
            // Copied in part from:
            //   https://developer.intuit.com/app/developer/qbo/docs/concepts/invoicing#how-to-implement

            // Step 1: Compose serviceContext
            ActivityLog.oLog(ActivityLog.enumLogTypes.Debug, 20, "pbInvoice_Click: Entry");
            OAuth2RequestValidator oauthValidator = new OAuth2RequestValidator(dictTokens["accessToken"]);
            ServiceContext serviceContext = new ServiceContext(dictTokens["realmId"], IntuitServicesType.QBO, oauthValidator);
            serviceContext.IppConfiguration.BaseUrl.Qbo = "https://sandbox-quickbooks.api.intuit.com/";
            //serviceContext.IppConfiguration.BaseUrl.Qbo = "https://quickbooks.api.intuit.com/";//prod
            serviceContext.IppConfiguration.MinorVersion.Qbo = "29";

            //customerTest.CustomerFindbyIdTestUsingoAuth(qboContextoAuth);

            // Step 2: Create a customer object and put the customer of interest into it
            Customer customer = QBOHelper.CreateCustomer(serviceContext);
            customer.Id = uFlAcInvoice.sCustomerId;
            Customer custFound = Helper.FindById<Customer>(serviceContext, customer);

            // Step 3: Create a US Invoice object
            //TaxCode taxCode = Helper.FindOrAdd<TaxCode>(serviceContext, new TaxCode());
            //Account account = Helper.FindOrAddAccount(serviceContext, AccountTypeEnum.AccountsReceivable, AccountClassificationEnum.Liability);


            //Item item = Helper.FindOrAddItem(serviceContext, ItemTypeEnum.Service);
            Item item;
            try
            {
                item = FindByNameItem(serviceContext, ItemTypeEnum.Service, "Flying Charges");
            }
            catch (Global.excToPopup etp)
            {
                ProcessPopupException(etp);
                return;
            }
            Invoice invoice = new Invoice();
            //invoice.Deposit = 0.0M;
            invoice.DepositSpecified = false;

            // Step 4: Attach the customer to the invoice
            invoice.CustomerRef = new ReferenceType()
            {
                name = custFound.DisplayName,
                Value = custFound.Id
            };

            // Step 5: define miscellaneous data to go with the invoice
            invoice.DueDate = DateTime.UtcNow.Date;
            invoice.DueDate = invoice.DueDate.AddDays(15);
            invoice.DueDateSpecified = true;

            //invoice.TotalAmt = new Decimal(101.01);
            invoice.TotalAmtSpecified = false;

            //invoice.Balance = new Decimal(101.01);
            invoice.BalanceSpecified = false;

            invoice.TxnDate = DateTime.UtcNow.Date.AddDays(-1);
            invoice.TxnDateSpecified = true;

            // Step 6: Create the list of line items
            List<Line> lineList = new List<Line>();

            // Step 7: Create at least one line item
            Line line = new Line();
            //line.LineNum = "LineNum";
            line.Description = "Tow to 1015 ft AGL";
            //line.Amount = new Decimal(101.02);
            //line.AmountSpecified = true;
            /*
            Intuit.Ipp.Data.Class myClass = new Class();
            myClass.Id = "5000000000000031049";
            myClass.Name = "PW - 6";
            line.AnyIntuitObject = myClass; */

            //line.DetailType = LineDetailTypeEnum.DescriptionOnly;
            line.DetailType = LineDetailTypeEnum.SalesItemLineDetail;
            line.DetailTypeSpecified = true;
            line.Amount = 101.15M;
            line.AmountSpecified = true;
            line.AnyIntuitObject = new SalesItemLineDetail()
            {
                Qty = 1,
                QtySpecified = true,

                //AnyIntuitObject = 101.10M,
                //ItemElementName = ItemChoiceType.UnitPrice,

                ItemRef = new ReferenceType()
                {
                    name = item.Name,
                    Value = item.Id
                },
                //TaxCodeRef = new ReferenceType()
                //{
                //    name = taxCode.Name,
                //    Value = taxCode.Id //US has TAX or NON. Global needs actual TaxCode Id
                //},
            };

            // Step 8: Add the line item to the list of line items
            lineList.Add(line);
            // Step 9: Attach the line item list to the invoice
            invoice.Line = lineList.ToArray();

            // Step 10: Add sales tax data
            invoice.TxnTaxDetail = new TxnTaxDetail()
            {
                TotalTax = 0.0M,
                TotalTaxSpecified = true
            };

            // Step 11: Transmit the invoice
            DataService service = new DataService(serviceContext);
            try
            {
                Invoice addedInvoice = service.Add<Invoice>(invoice);
            }
            catch (Intuit.Ipp.Exception.IdsException IdsExc)
            {
                ProcessPopupException(new Global.excToPopup("Inv2Qbo.aspx.cs.genInvoice: IdsException Message=" + IdsExc.Message + ", inner exception message=" + IdsExc.InnerException.Message));
            }
            catch (Exception exc)
            {
                ProcessPopupException(new Global.excToPopup("Inv2Qbo.aspx.cs.genInvoice: general exception, Message=" + exc.Message));
            }
            lblInvoiceSentYesNo.Text = "YES";
            uFlAcInvoice.flAcInvoiceStatus = FlAcInvoiceStatus.Done;
            Session["FlAcInvoice"] = uFlAcInvoice;
            ActivityLog.oLog(ActivityLog.enumLogTypes.Debug, 20, "Inv2Qbo.aspx.cs.genInvoice: Exit");
        }

        private Item FindByNameItem(ServiceContext context, ItemTypeEnum itemType, string suItemName)
        {
            List<Item> liItem = Helper.FindAll<Item>(context, new Item(), 1, 500).Where(i => i.status != EntityStatusEnum.SyncError).ToList();
            if (liItem.Count > 0)
            {
                foreach (Item item in liItem)
                {
                    if (item.Name == suItemName)
                    {
                        return item;
                    }
                }
                throw new Global.excToPopup("Inv2Qbo.aspx.cs.FindByNameItem: no item found with name `" + suItemName + "`");
            }
            else
            {
                throw new Global.excToPopup("Inv2Qbo.aspx.cs.FindByNameItem: list of items is empty");
            }
        }
    }

    public static class ResponseHelper
    {
        public static void Redirect(this HttpResponse response, string url, string target, string windowFeatures)
        {
            if ((String.IsNullOrEmpty(target) || target.Equals("_self", StringComparison.OrdinalIgnoreCase)) && String.IsNullOrEmpty(windowFeatures))
            {
                response.Redirect(url);
            }
            else
            {
                Page page = (Page)HttpContext.Current.Handler;
                if (page == null)
                {
                    throw new InvalidOperationException("Cannot redirect to new window outside Page context.");
                }
                url = page.ResolveClientUrl(url);
                string script;
                if (!String.IsNullOrEmpty(windowFeatures))
                {
                    script = @"window.open(""{0}"", ""{1}"", ""{2}"");";
                }
                else
                {
                    script = @"window.open(""{0}"", ""{1}"");";
                }
                script = String.Format(script, url, target, windowFeatures);
                ScriptManager.RegisterStartupScript(page, typeof(Page), "Redirect", script, true);
            }
        }
    }
}