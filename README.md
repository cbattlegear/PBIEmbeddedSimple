# Simple Power BI Embedded Examples

This is an attempt to create a super simple Power BI Embedded .Net example.
The goal was to have a self contained example for both App Owns Data and User Owns Data embedding methods.
Each example is heavily documented in code comments but I will give a quick rundown for each on how to get going here

## Apps Owns Dashboard

This is a dashboard embedded using credentials that are in the code
### Items you need to change:
* Username
	* This is the username of the account with access to the dashboard
* Password
	* This is the password of the account with access to the dashboard
* ApplicationId
	* This is the Application ID of the AAD App you created follow the instructions here https://docs.microsoft.com/en-us/power-bi/developer/embed-sample-for-customers#register-an-application-in-azure-active-directory-azure-ad
* WorkspaceId
	* This is the workspace ID for your Power BI Workspace this is the first GUID in your Power BI dashboard url https://app.powerbi.com/groups/11111111-2222-3333-4444-555555555555/dashboards/11111111-2222-3333-4444-555555555555
* DashboardId
	* This is the dashboard ID for your Power BI Workspace this is the second GUID in your Power BI dashboard url https://app.powerbi.com/groups/11111111-2222-3333-4444-555555555555/dashboards/11111111-2222-3333-4444-555555555555

## Apps Owns Report

This is a report embedded using credentials that are in the code. The settings are the same as the Dashboard setup except for ReportId
### Items you need to change:
* ReportId
	* This is the report ID for your Power BI Workspace this is the second GUID in your Power BI report url https://app.powerbi.com/groups/11111111-2222-3333-4444-555555555555/reports/11111111-2222-3333-4444-555555555555/ReportSection

## User Owns Dashboard

This is a dashboard that the user will log in with their own credentials. One important item to note is there are two pages, the RedirectDashboard.aspx page is strictly to get the Auth Token and put it in the session.
### Items you need to change:
* ApplicationId
	* This is the Application ID of the AAD App you created follow the instructions here https://docs.microsoft.com/en-us/power-bi/developer/embed-sample-for-your-organization#setup-your-embedded-analytics-development-environment
* ClientSecret
	* This is the Client Secret created as part of the instructions above
* RedirectUrl
	* This is the Redirect URL for the auth to land on, you can copy/paste this from the code
* WorkspaceId
	* This is the workspace ID for your Power BI Workspace this is the first GUID in your Power BI dashboard url https://app.powerbi.com/groups/11111111-2222-3333-4444-555555555555/dashboards/11111111-2222-3333-4444-555555555555
* DashboardId
	* This is the dashboard ID for your Power BI Workspace this is the second GUID in your Power BI dashboard url https://app.powerbi.com/groups/11111111-2222-3333-4444-555555555555/dashboards/11111111-2222-3333-4444-555555555555

## User Owns Report

This is a report that the user will log in with their own credentials. One important item to note is there are two pages, the RedirectReport.aspx page is strictly to get the Auth Token and put it in the session.
The only difference is we set the ReportId instead of the DashboardId
### Items you need to change:
* ReportId
	* This is the report ID for your Power BI Workspace this is the second GUID in your Power BI report url https://app.powerbi.com/groups/11111111-2222-3333-4444-555555555555/reports/11111111-2222-3333-4444-555555555555/ReportSection