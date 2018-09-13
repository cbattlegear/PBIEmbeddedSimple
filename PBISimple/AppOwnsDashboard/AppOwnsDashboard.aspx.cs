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
    public partial class AppOwnsDashboard : System.Web.UI.Page
    {
        /******************** The settings you need to change start here ********************/
        //This is the username of the user that has access to the Dashboard
        private static readonly string Username = "UserName";
        //And their password
        //DO NOT SAVE THIS IN CODE! You will get burned somehow, make sure to put this somewhere safe in a settings file encryped and hidden deep away from the world
        private static readonly string Password = "Password";
        //This is your Azure Active Directory Application ID. The instructions to set this up are:
        //https://docs.microsoft.com/en-us/power-bi/developer/embed-sample-for-customers#register-an-application-in-azure-active-directory-azure-ad
        private static readonly string ApplicationId = "11111111-2222-3333-4444-555555555555";
        //These are the IDs for your workspace and dashboard, these can easily be found via your Power BI url
        /*
         * https://org.powerbi.com/groups/11111111-2222-3333-4444-555555555555/dashboards/11111111-2222-3333-4444-555555555555
         *                                ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^            ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
         *                                     This is your Workspace ID                        This is your Dashboard ID
         */
        private static readonly string WorkspaceId = "11111111-2222-3333-4444-555555555555";
        //If you leave the DashboardId as blank we will just display the first dashboard in your Workspace
        private static readonly string DashboardId = "11111111-2222-3333-4444-555555555555";
        /******************** The settings you need to change end here **********************/

        //These are how we handle authentication, they will be different for different clouds (Gov, Germany)
        private static readonly string AuthorityUrl = "https://login.windows.net/common/oauth2/authorize/";
        private static readonly string ResourceUrl = "https://analysis.windows.net/powerbi/api";

        //This is the power BI Api URL, this will only be different for different clouds.
        private static readonly string ApiUrl = "https://api.powerbi.com/";

        //The EmbedConfig class is in EmbedConfig.cs this is just the properties for our embedded dashboard
        public EmbedConfig embedConf;
        public static string DashboardList;

        protected void Page_Load(object sender, EventArgs e)
        {
            embedConf = EmbedDashboard();
        }

        public static EmbedConfig EmbedDashboard()
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
                // Get a list of dashboards.
                var dashboards = client.Dashboards.GetDashboardsInGroup(WorkspaceId);

                // Get the first report in the workspace.
                var dashboard = dashboards.Value.FirstOrDefault();

                //This is where we determine if your DashboardID set above is blank, if blank use the queried dashboard. If not just use your setting
                var dashboardid = "";
                if(DashboardId == "")
                {
                    dashboardid = dashboard.Id;
                } else
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