using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;

//These are all downloaded via NuGet
using Microsoft.PowerBI.Api.V2;
using Microsoft.PowerBI.Api.V2.Models;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Rest;

namespace PBIEmbeddedSimple
{
    public partial class AppOwnsReport : System.Web.UI.Page
    {
        /******************** The settings you need to change start here ********************/
        //This is the username of the user that has access to the Report
        private static readonly string Username = "UserName";
        //And their password
        //DO NOT SAVE THIS IN CODE! You will get burned somehow, make sure to put this somewhere safe in a settings file encryped and hidden deep away from the world
        private static readonly string Password = "Password";

        //This is your Azure Active Directory Application ID. The instructions to set this up are:
        //https://docs.microsoft.com/en-us/power-bi/developer/embed-sample-for-customers#register-an-application-in-azure-active-directory-azure-ad
        private static readonly string ApplicationId = "11111111-2222-3333-4444-555555555555";

        //These are the IDs for your workspace and report, these can easily be found via your Power BI url
        /*
         * https://org.powerbi.com/groups/11111111-2222-3333-4444-555555555555/reports/11111111-2222-3333-4444-555555555555/ReportSection
         *                                ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^        ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
         *                                     This is your Workspace ID                    This is your Report ID
         */
        private static readonly string WorkspaceId = "11111111-2222-3333-4444-555555555555";
        //If you leave the ReportId as blank we will just display the first report in your Workspace
        private static readonly string ReportId = "11111111-2222-3333-4444-555555555555";
        /******************** The settings you need to change end here **********************/


        //These are how we handle authentication, they will be different for different clouds (Gov, Germany)
        private static readonly string AuthorityUrl = "https://login.windows.net/common/oauth2/authorize/";
        private static readonly string ResourceUrl = "https://analysis.windows.net/powerbi/api";

        //This is the power BI Api URL, this will only be different for different clouds.
        private static readonly string ApiUrl = "https://api.powerbi.com/";

        //The EmbedConfig class is in EmbedConfig.cs this is just the properties for our embedded dashboard
        public EmbedConfig embedConf;

        protected void Page_Load(object sender, EventArgs e)
        {
            embedConf = EmbedReport();
        }

        public static EmbedConfig EmbedReport()
        {
            // Create a user password cradentials.
            var credential = new UserPasswordCredential(Username, Password);

            // Authenticate using created credentials
            var authenticationContext = new AuthenticationContext(AuthorityUrl);
            var authenticationResult = authenticationContext.AcquireTokenAsync(ResourceUrl, ApplicationId, credential).Result;

            var tokenCredentials = new TokenCredentials(authenticationResult.AccessToken, "Bearer");

            // Create a Power BI Client object. It will be used to call Power BI APIs.
            using (var client = new PowerBIClient(new Uri(ApiUrl), tokenCredentials))
            {
                // Get a list of reports.
                var reports = client.Reports.GetReportsInGroup(WorkspaceId);

                // Get the first report in the workspace.
                var report = reports.Value.FirstOrDefault();

                //This is where we determine if your DashboardID set above is blank, if blank use the queried dashboard. If not just use your setting
                var reportid = "";
                if(ReportId == "")
                {
                    reportid = report.Id;
                } else
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