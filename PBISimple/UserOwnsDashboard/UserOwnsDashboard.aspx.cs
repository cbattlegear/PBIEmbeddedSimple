
using System;
using System.Linq;
using System.Web;
using System.Web.UI;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System.Collections.Specialized;
using Newtonsoft.Json;

using Microsoft.PowerBI.Api.V2;
using Microsoft.PowerBI.Api.V2.Models;
using Microsoft.Rest;

namespace PBIEmbeddedSimple
{
    /* NOTE: This sample is to illustrate how to authenticate a Power BI web app. 
    * In a production application, you should provide appropriate exception handling and refactor authentication settings into 
    * a configuration. Authentication settings are hard-coded in the sample to make it easier to follow the flow of authentication. */
    public partial class UserOwnsDashboard : Page
    {
        /******************** The settings you need to change start here ********************/
        //This is your Azure Active Directory Application ID. The instructions to set this up are:
        //https://docs.microsoft.com/en-us/power-bi/developer/embed-sample-for-your-organization#setup-your-embedded-analytics-development-environment
        public static readonly string ApplicationId = "11111111-2222-3333-4444-555555555555";

        //This is a secret just like a password, do not store this in your code! This is a bad idea overall, you created this as part of your settings for the AppId
        public static readonly string ClientSecret = "RandomStringOfCharactersandStuff=";

        //This needs to be set in your AAD application or things will break, Make sure to paste this verbatim
        public static readonly string RedirectUrl = "http://localhost:2785/UserOwnsDashboard/RedirectDashboard.aspx";
        //These are the IDs for your workspace and dashboard, these can easily be found via your Power BI url
        /*
         * https://app.powerbi.com/groups/11111111-2222-3333-4444-555555555555/dashboards/11111111-2222-3333-4444-555555555555
         *                                ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^            ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
         *                                     This is your Workspace ID                        This is your Dashboard ID
         */
        private static readonly string WorkspaceId = "11111111-2222-3333-4444-555555555555";
        //If you leave the DashboardId as blank we will just display the first dashboard in your Workspace
        private static readonly string DashboardId = "11111111-2222-3333-4444-555555555555";
        /******************** The settings you need to change end here **********************/

        //These are how we handle authentication, they will be different for different clouds (Gov, Germany)
        public static readonly string AuthorityUrl = "https://login.windows.net/common/oauth2/authorize/";
        public static readonly string ResourceUrl = "https://analysis.windows.net/powerbi/api";

        //This is the power BI Api URL, this will only be different for different clouds.
        public static readonly string PbiDataSet = "https://api.powerbi.com/v1.0/myorg/";
        public static readonly string PbiAPIUrl = "https://analysis.windows.net/powerbi/api";

        //This is the power BI Api URL, this will only be different for different clouds.
        private static readonly string ApiUrl = "https://api.powerbi.com/";

        //The EmbedConfig class is in EmbedConfig.cs this is just the properties for our embedded dashboard
        public EmbedConfig embedConf;
        public static string DashboardList;

        public const string authResultString = "authResultDashboard";
        public static AuthenticationResult authResult { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {

            //Test for AuthenticationResult
            if (Session[authResultString] != null)
            {
                //Get the authentication result from the session
                authResult = (AuthenticationResult)Session[authResultString];
                embedConf = EmbedDashboard();
            }
            else
            {
                signIn();
            }
        }

        protected void signIn()
        {
            //Create a query string
            //Create a sign-in NameValueCollection for query string
            var @params = new NameValueCollection
            {
                //Azure AD will return an authorization code. 
                //See the Redirect class to see how "code" is used to AcquireTokenByAuthorizationCode
                {"response_type", "code"},

                //Client ID is used by the application to identify themselves to the users that they are requesting permissions from. 
                //You get the client id when you register your Azure app.
                {"client_id", ApplicationId},

                //Resource uri to the Power BI resource to be authorized
                {"resource", PbiAPIUrl},

                //After user authenticates, Azure AD will redirect back to the web app
                {"redirect_uri", RedirectUrl}
            };

            //Create sign-in query string
            var queryString = HttpUtility.ParseQueryString(string.Empty);
            queryString.Add(@params);

            //Redirect authority
            //Authority Uri is an Azure resource that takes a client id to get an Access token
            string authorityUri = AuthorityUrl;
            var authUri = String.Format("{0}?{1}", authorityUri, queryString);
            Response.Redirect(authUri);
        }
        public static EmbedConfig EmbedDashboard()
        {
            var tokenCredentials = new TokenCredentials(authResult.AccessToken, "Bearer");

            // Create a Power BI Client object. It will be used to call Power BI APIs.
            using (var client = new PowerBIClient(new Uri(ApiUrl), tokenCredentials))
            {
                // Get a list of dashboards.
                var dashboards = client.Dashboards.GetDashboardsInGroup(WorkspaceId);

                // Get the first report in the workspace.
                var dashboard = dashboards.Value.FirstOrDefault();

                //This is where we determine if your DashboardID set above is blank, if blank use the queried dashboard. If not just use your setting
                var dashboardid = "";
                if (DashboardId == "")
                {
                    dashboardid = dashboard.Id;
                }
                else
                {
                    dashboardid = DashboardId;
                }

                // Generate Embed Token, a very import part of this is you can change the access level which will let end users edit!
                var generateTokenRequestParameters = new GenerateTokenRequest(accessLevel: "view");
                var tokenResponse = client.Dashboards.GenerateTokenInGroupAsync(WorkspaceId, dashboardid, generateTokenRequestParameters).Result;

                // Generate Embed Configuration.
                var embedConfig = new EmbedConfig()
                {
                    EmbedToken = tokenResponse,
                    EmbedUrl = dashboard.EmbedUrl,
                    Id = dashboardid
                };

                return embedConfig;
            }
        }
    }


}