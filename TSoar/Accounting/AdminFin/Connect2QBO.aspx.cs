using System;
using System.Collections.Generic;
using System.Net;
using System.Web.UI;
using System.Configuration;
using System.Web;
using Intuit.Ipp.OAuth2PlatformClient;

namespace TSoar.Accounting.AdminFin
{
    public partial class Connect2QBO : System.Web.UI.Page
    {
        #region Definitions

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
            var redirectUri = $"{Request.Url.Scheme}://{Request.Url.Host}:{Request.Url.Port}/Accounting/AdminFin/Connect2QBO.aspx";
            ActivityLog.oLog(ActivityLog.enumLogTypes.Debug, 25, "Connect2QBO.aspx.OAuth2Client.redirectURI='" + redirectUri + "'");
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

        protected void Page_Load(object sender, EventArgs e)
        {
            AsyncMode = true;
            if (!dictTokens.ContainsKey("accessToken"))
            {
                mainButtons.Visible = true;
                connected.Visible = false;
                //ActivityLog.oLog(ActivityLog.enumLogTypes.Debug, 20, "Page_Load: no access token");
                if (Request.QueryString.Count > 0)
                {
                    //ActivityLog.oLog(ActivityLog.enumLogTypes.Debug, 21, "Page_Load: Request.QueryString.Count > 0");
                    var response = new AuthorizeResponse(Request.QueryString.ToString());
                    if (response.State != null)
                    {
                        //ActivityLog.oLog(ActivityLog.enumLogTypes.Debug, 22, "Page_Load: no access token");
                        if (oauthClient.CSRFToken == response.State)
                        {
                            //ActivityLog.oLog(ActivityLog.enumLogTypes.Debug, 23, "Page_Load: oauthClient.CSRFToken == response.State");
                            if (response.RealmId != null)
                            {
                                //ActivityLog.oLog(ActivityLog.enumLogTypes.Debug, 24, "Page_Load: response.RealmId != null");
                                if (!dictTokens.ContainsKey("realmId"))
                                {
                                    //ActivityLog.oLog(ActivityLog.enumLogTypes.Debug, 25, "Page_Load: !dictTokens.ContainsKey(realmId)");
                                    dictTokens.Add("realmId", response.RealmId);
                                }
                            }

                            if (response.Code != null)
                            {
                                authCode = response.Code;
                                //ActivityLog.oLog(ActivityLog.enumLogTypes.Debug, 24, "Page_Load: Authorization code obtained; -> performCodeExchange");
                                PageAsyncTask t = new PageAsyncTask(performCodeExchange);
                                Page.RegisterAsyncTask(t);
                                Page.ExecuteRegisteredAsyncTasks();
                            }
                        }
                        else
                        {
                            ActivityLog.oLog(ActivityLog.enumLogTypes.Debug, 23, "Connect2QBO.aspx.cs.Page_Load: Invalid State");
                            dictTokens.Clear();
                        }
                    }
                }
            }
            else
            {
                mainButtons.Visible = false;
                connected.Visible = true;
                lblConnected.Visible = true;
            }
        }

        #region button click events

        protected void pbConnect_Click(object sender, EventArgs e)
        {
            //ActivityLog.oLog(ActivityLog.enumLogTypes.Debug, 20, "ImgGetAppNow_Click: Initiating Get App Now call.");
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
                    //ActivityLog.oLog(ActivityLog.enumLogTypes.Debug, 21, "ImgGetAppNow_Click: authorizationRequest=" + authorizationRequest);
                    Response.Redirect(authorizationRequest, "_blank", "menubar=0,scrollbars=1,width=780,height=900,top=10");
                }
            }
            catch (Exception ex)
            {
                ActivityLog.oLog(ActivityLog.enumLogTypes.Debug, 20, "Connect2QBO.aspx.cs.ImgGetAppNow_Click: " + ex.Message);
            }
        }

        protected async void btnUserInfo_Click(object sender, EventArgs e)
        {
            lblUserInfo.Visible = true;
            if (idToken != null)
            {
                var userInfoResp = await oauthClient.GetUserInfoAsync(dictTokens["accessToken"]);
                lblUserInfo.Text = userInfoResp.Raw;
                ActivityLog.oLog(ActivityLog.enumLogTypes.Debug, 21, "btnUserInfo_Click: lblUserInfo.Text=" + lblUserInfo.Text);
            }
            else
            {
                lblUserInfo.Text = "UserInfo call is available through OpenId/GetAppNow flow first.";
                ActivityLog.oLog(ActivityLog.enumLogTypes.Debug, 21, "btnUserInfo_Click: Go through OpenId flow first.");
            }
        }

        protected async void btnRefresh_Click(object sender, EventArgs e)
        {
            if ((dictTokens.ContainsKey("accessToken")) && (dictTokens.ContainsKey("refreshToken")))
            {
                ActivityLog.oLog(ActivityLog.enumLogTypes.Debug, 21, "Connect2QBO.aspx.cs.btnRefresh_Click: Exchanging refresh token for access token.");
                var tokenResp = await oauthClient.RefreshTokenAsync(dictTokens["refreshToken"]);
            }
        }

        protected async void btnRevoke_Click(object sender, EventArgs e)
        {
            ActivityLog.oLog(ActivityLog.enumLogTypes.Debug, 20, "Connect2QBO.aspx.cs.btnRevoke_Click: Performing Revoke tokens.");
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
                ActivityLog.oLog(ActivityLog.enumLogTypes.Debug, 21, "Connect2QBO.aspx.cs.btnRevoke_Click: Token revoked.");
            }
        }
        #endregion

        public async System.Threading.Tasks.Task performCodeExchange()
        {
            //ActivityLog.oLog(ActivityLog.enumLogTypes.Debug, 20, "performCodeExchange: Exchanging code for tokens.");
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
                ActivityLog.oLog(ActivityLog.enumLogTypes.Debug, 21, "Connect2QBO.aspx.cs.performCodeExchange: Problem while getting bearer tokens: " + ex.Message);
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