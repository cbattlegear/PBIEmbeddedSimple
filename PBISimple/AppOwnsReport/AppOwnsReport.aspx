<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AppOwnsReport.aspx.cs" Inherits="PBIEmbeddedSimple.AppOwnsReport" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Apps Owns Data Report</title>
    <!-- These are the required scripts for Power BI embedding -->
    <script src="https://code.jquery.com/jquery-3.2.1.min.js" integrity="sha256-hwg4gsxgFZhOsEEamdOYGBf13FyQuiTwlAQgxVSNgt4=" crossorigin="anonymous"></script>
    <script src="https://npmcdn.com/es6-promise@3.2.1"></script>
    <script src="/Public/scripts/powerbi.js"></script>
    <style>
        /* Force the Power BI Report to fill the screen */
        html, body, #form1, #reportContainer {
            height: 100%;
            margin: 0;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div id="reportContainer">
        </div>

        <!-- This script area is where Power BI does the actual work to embed the report -->
        <script>
            // Read embed application token from Model
            var accessToken = "<%= embedConf.EmbedToken.Token %>";

            // Read embed URL from Model
            var embedUrl = "<%= embedConf.EmbedUrl %>";

            // Read report Id from Model
            var embedReportId = "<%= embedConf.Id %>";

            // Get models. models contains enums that can be used.
            var models = window['powerbi-client'].models;

            // Embed configuration used to describe the what and how to embed.
            // This object is used when calling powerbi.embed.
            // This also includes settings and options such as filters.
            // You can find more information at https://github.com/Microsoft/PowerBI-JavaScript/wiki/Embed-Configuration-Details.
            var config = {
                type: 'report',
                tokenType: models.TokenType.Embed,
                accessToken: accessToken,
                embedUrl: embedUrl,
                id: embedReportId,
                permissions: models.Permissions.All,
                settings: {
                    filterPaneEnabled: true,
                    navContentPaneEnabled: true
                }
            };

            // Get a reference to the embedded report HTML element
            var reportContainer = $('#reportContainer')[0];

            // Embed the report and display it within the div container.
            var report = powerbi.embed(reportContainer, config);
        </script>
    </form>
</body>
</html>
