
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
    public partial class UserOwnsReport : Page
    {
        //This is your Azure Active Directory Application ID. The instructions to set this up are:
        //https://docs.microsoft.com/en-us/power-bi/developer/embed-sample-for-customers#register-an-application-in-azure-active-directory-azure-ad
        public static readonly string ApplicationId = "11111111-2222-3333-4444-555555555555";

        //This is a secret just like a password, do not store this in your code! This is a bad idea overall, you created this as part of your settings for the AppId
        public static readonly string ClientSecret = "RandomStringOfCharactersandStuff=";

        public static readonly string RedirectUrl = "http://localhost:2785/UserOwnsReport/RedirectReport.aspx";
        //These are the IDs for your workspace and report, these can easily be found via your Power BI url
        /*
         * https://app.powerbi.com/groups/11111111-2222-3333-4444-555555555555/reports/11111111-2222-3333-4444-555555555555/ReportSection
         *                                ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^        ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
         *                                     This is your Workspace ID                    This is your Report ID
         */
        private static readonly string WorkspaceId = "11111111-2222-3333-4444-555555555555";
        //If you leave the ReportId as blank we will just display the first report in your Workspace
        private static readonly string ReportId = "11111111-2222-3333-4444-555555555555";
        //This is the power BI Api URL, this will only be different for different clouds.
        private static readonly string ApiUrl = "https://api.powerbi.com/";

        //This is the power BI Api URL, this will only be different for different clouds.
        public static readonly string PbiDataSet = "https://api.powerbi.com/v1.0/myorg/";
        public static readonly string PbiAPIUrl = "https://analysis.windows.net/powerbi/api";


        //These are how we handle authentication, they will be different for different clouds (Gov, Germany)
        public static readonly string AuthorityUrl = "https://login.windows.net/common/oauth2/authorize/";
        public static readonly string ResourceUrl = "https://analysis.windows.net/powerbi/api";

        //The EmbedConfig class is in EmbedConfig.cs this is just the properties for our embedded dashboard
        public EmbedConfig embedConf;

        public const string authResultString = "authResultReport";
        public static AuthenticationResult authResult { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {

            //Check if we are authenticated, if we are, pull the token from the session, if not follow the sign in process
            if (Session[authResultString] != null)
            {
                //Get the authentication result from the session
                authResult = (AuthenticationResult)Session[authResultString];
                embedConf = EmbedReport();
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

            //Once all this is created, redirect to our login system, which will then redirect to RedirectReport.aspx
            Response.Redirect(authUri);
        }
        public static EmbedConfig EmbedReport()
        {
            var tokenCredentials = new TokenCredentials(authResult.AccessToken, "Bearer");

            // Create a Power BI Client object. It will be used to call Power BI APIs.
            using (var client = new PowerBIClient(new Uri(ApiUrl), tokenCredentials))
            {
                // Get a list of reports.
                var reports = client.Reports.GetReportsInGroup(WorkspaceId);

                // Get the first report in the workspace.
                var report = reports.Value.FirstOrDefault();

                //This is where we determine if your DashboardID set above is blank, if blank use the queried dashboard. If not just use your setting
                var reportid = "";
                if (ReportId == "")
                {
                    reportid = report.Id;
                }
                else
                {
                    reportid = ReportId;
                }

                // Generate Embed Token, a very import part of this is you can change the access level which will let end users edit!
                var generateTokenRequestParameters = new GenerateTokenRequest(accessLevel: "view");
                var tokenResponse = client.Reports.GenerateTokenInGroupAsync(WorkspaceId, reportid, generateTokenRequestParameters).Result;

                // Generate Embed Configuration.
                var embedConfig = new EmbedConfig()
                {
                    EmbedToken = tokenResponse,
                    EmbedUrl = report.EmbedUrl,
                    Id = reportid
                };

                return embedConfig;
            }
        }
    }


}