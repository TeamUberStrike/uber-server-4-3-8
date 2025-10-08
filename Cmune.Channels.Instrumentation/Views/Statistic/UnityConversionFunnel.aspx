<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content2" ContentPlaceHolderID="HeadContent" runat="server">
    <% Html.RenderPartial("ChartJavaScriptControl", ViewData); %>
    <script type="text/javascript" src="<%= Url.Content("~/Scripts/js/stats.js")%>?v=<%= ViewBag.PublishVersion %>"></script>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <script type="text/javascript">
        function updateCharts() {
            updateChart('facebookOsDistributionChart', '<%= Url.Action("GetOsDistribution", "StatisticsUnityConversionFunnel") %>?' + $('#StatsCalendar').serialize() + '&channel=1');
            updateChart('portalOsDistributionChart', '<%= Url.Action("GetOsDistribution", "StatisticsUnityConversionFunnel") %>?' + $('#StatsCalendar').serialize() + '&channel=0');
            updateChart('facebookWindowsBrowserDistributionChart', '<%= Url.Action("GetBrowserDistribution", "StatisticsUnityConversionFunnel") %>?' + $('#StatsCalendar').serialize() + '&channel=1&os=Windows');
            updateChart('portalWindowsBrowserDistributionChart', '<%= Url.Action("GetBrowserDistribution", "StatisticsUnityConversionFunnel") %>?' + $('#StatsCalendar').serialize() + '&channel=0&os=Windows');
            updateChart('portalMacBrowserDistributionChart', '<%= Url.Action("GetBrowserDistribution", "StatisticsUnityConversionFunnel") %>?' + $('#StatsCalendar').serialize() + '&channel=0&os=Mac');
            updateFacebookBrowsersCharts();
            updatePortalBrowsersCharts();
            updateCustomQuery();
        }

        function updateFacebookBrowsersCharts() {
            updateChart('facebookBrowsersNoUnityFunnelChart', '<%= Url.Action("GetInstallFlow", "StatisticsUnityConversionFunnel") %>?' + $('#StatsCalendar').serialize() + '&channel=1&selectedBrowser=' + $("#facebookBrowsers").val() + '&hasUnity=0');
            updateChart('facebookBrowsersNoUnityFunnelLineChart', '<%= Url.Action("GetInstallFlowLine", "StatisticsUnityConversionFunnel") %>?' + $('#StatsCalendar').serialize() + '&channel=1&selectedBrowser=' + $("#facebookBrowsers").val() + '&hasUnity=0');
            updateChart('facebookBrowsersHasUnityFunnelChart', '<%= Url.Action("GetInstallFlow", "StatisticsUnityConversionFunnel") %>?' + $('#StatsCalendar').serialize() + '&channel=1&selectedBrowser=' + $("#facebookBrowsers").val() + '&hasUnity=1');
            updateChart('facebookBrowsersHasUnityFunnelLineChart', '<%= Url.Action("GetInstallFlowLine", "StatisticsUnityConversionFunnel") %>?' + $('#StatsCalendar').serialize() + '&channel=1&selectedBrowser=' + $("#facebookBrowsers").val() + '&hasUnity=1');
        }

        function updatePortalBrowsersCharts() {
            updateChart('portalBrowsersNoUnityFunnelChart', '<%= Url.Action("GetInstallFlow", "StatisticsUnityConversionFunnel") %>?' + $('#StatsCalendar').serialize() + '&channel=0&selectedBrowser=' + $("#portalBrowsers").val() + '&hasUnity=0');
            updateChart('portalBrowsersNoUnityFunnelLineChart', '<%= Url.Action("GetInstallFlowLine", "StatisticsUnityConversionFunnel") %>?' + $('#StatsCalendar').serialize() + '&channel=0&selectedBrowser=' + $("#portalBrowsers").val() + '&hasUnity=0');
            updateChart('portalBrowsersHasUnityFunnelChart', '<%= Url.Action("GetInstallFlow", "StatisticsUnityConversionFunnel") %>?' + $('#StatsCalendar').serialize() + '&channel=0&selectedBrowser=' + $("#portalBrowsers").val() + '&hasUnity=1');
            updateChart('portalBrowsersHasUnityFunnelLineChart', '<%= Url.Action("GetInstallFlowLine", "StatisticsUnityConversionFunnel") %>?' + $('#StatsCalendar').serialize() + '&channel=0&selectedBrowser=' + $("#portalBrowsers").val() + '&hasUnity=1');
        }

        function updateCustomQuery() {
            updateCustomQueryOsName(updateCustomQueryOsVersionCallback(true));
        }

        function updateCustomQueryCharts() {
            updateChart('customQueryNoUnityFunnelChart', '<%= Url.Action("GetCustomInstallFlow", "StatisticsUnityConversionFunnel") %>?' + $('#StatsCalendar').serialize() + '&' + $('#customQueryUnityConversionFunnelForm').serialize() + '&hasUnity=0');
            updateChart('customQueryNoUnityFunnelLineChart', '<%= Url.Action("GetCustomInstallFlowLine", "StatisticsUnityConversionFunnel") %>?' + $('#StatsCalendar').serialize() + '&' + $('#customQueryUnityConversionFunnelForm').serialize() + '&hasUnity=0');
            updateChart('customQueryHasUnityFunnelChart', '<%= Url.Action("GetCustomInstallFlow", "StatisticsUnityConversionFunnel") %>?' + $('#StatsCalendar').serialize() + '&' + $('#customQueryUnityConversionFunnelForm').serialize() + '&hasUnity=1');
            updateChart('customQueryHasUnityFunnelLineChart', '<%= Url.Action("GetCustomInstallFlowLine", "StatisticsUnityConversionFunnel") %>?' + $('#StatsCalendar').serialize() + '&' + $('#customQueryUnityConversionFunnelForm').serialize() + '&hasUnity=1');
        }

        function updateCustomQueryOsName(callback) {
            updateDropDownList('customQueryOsName', '/StatisticsUnityConversionFunnel/GetOsNameDropDownList', $('#StatsCalendar').serialize() + '&' + $('#customQueryUnityConversionFunnelForm').serialize(), callback);
        }

        function updateCustomQueryOsVersionCallback(shouldDisplayChart) {
            updateCustomQueryOsVersion(updateCustomQueryBrowserNameCallback(shouldDisplayChart));
        }

        function updateCustomQueryOsVersion(callback) {
            updateDropDownList('customQueryOsVersion', '/StatisticsUnityConversionFunnel/GetOsVersionDropDownList', $('#StatsCalendar').serialize() + '&' + $('#customQueryUnityConversionFunnelForm').serialize(), callback);
        }

        function updateCustomQueryBrowserNameCallback(shouldDisplayChart) {
            updateCustomQueryBrowserName(updateCustomQueryBrowserVersionCallback(shouldDisplayChart));
        }

        function updateCustomQueryBrowserName(callback) {
            updateDropDownList('customQueryBrowserName', '/StatisticsUnityConversionFunnel/GetBrowserNameDropDownList', $('#StatsCalendar').serialize() + '&' + $('#customQueryUnityConversionFunnelForm').serialize(), callback);
        }

        function updateCustomQueryBrowserVersionCallback(shouldDisplayChart) {
            var callback = null;

            if (shouldDisplayChart !== undefined && shouldDisplayChart) {
                callback = updateCustomQueryCharts;
            }

            updateCustomQueryBrowserVersion(callback);
        }

        function updateCustomQueryBrowserVersion(callback) {
            updateDropDownList('customQueryBrowserVersion', '/StatisticsUnityConversionFunnel/GetBrowserVersionDropDownList', $('#StatsCalendar').serialize() + '&' + $('#customQueryUnityConversionFunnelForm').serialize(), callback);
        }

    </script>

    <% Html.RenderPartial("StatsCalendar", ViewData); %>

    <div id="statsDiv" style="width:1200px;">

        <h1>Facebook</h1>

        <div class="chartHolder">
            <%= Html.RenderChart("facebookOsDistributionChart", Url.Action("GetOsDistribution", "StatisticsUnityConversionFunnel") + "?channel=1", 600, 300, Url)%>
            <div id="facebookOsDistributionChart" style="margin: 0 0 0 0;">
            </div>
        </div>

        <div class="chartHolder">
            <%= Html.RenderChart("facebookWindowsBrowserDistributionChart", Url.Action("GetBrowserDistribution", "StatisticsUnityConversionFunnel") + "?channel=1&os=Windows", 600, 300, Url)%>
            <div id="facebookWindowsBrowserDistributionChart" style="margin: 0 0 0 0;">
            </div>
        </div>

        <div class="clear"></div>
        
        <form id="facebookBrowsersForm" method="post" action="" style="text-align:left; margin:15px 0 25px 0;">
            <span style="font-weight:bold;">Browser:</span> <%= Html.DropDownList("facebookBrowsers", ViewData["facebookBrowsers"] as SelectList, new { onchange = "updateFacebookBrowsersCharts();" })%>
        </form>

        <div class="chartHolder">
            <%= Html.RenderChart("facebookBrowsersNoUnityFunnelChart", Url.Action("GetInstallFlow", "StatisticsUnityConversionFunnel") + "?channel=1&selectedBrowser=" + ((string)ViewBag.SelectedFacebookBrowser) + "&hasUnity=0", 600, 300, Url)%>
            <div id="facebookBrowsersNoUnityFunnelChart">
            </div>
        </div>

        <div class="chartHolder">
            <%= Html.RenderChart("facebookBrowsersNoUnityFunnelLineChart", Url.Action("GetInstallFlowLine", "StatisticsUnityConversionFunnel") + "?channel=1&selectedBrowser=" + ((string)ViewBag.SelectedFacebookBrowser) + "&hasUnity=0", 600, 300, Url)%>
            <div id="facebookBrowsersNoUnityFunnelLineChart">
            </div>
        </div>

        <div class="chartHolder">
            <%= Html.RenderChart("facebookBrowsersHasUnityFunnelChart", Url.Action("GetInstallFlow", "StatisticsUnityConversionFunnel") + "?channel=1&selectedBrowser=" + ((string)ViewBag.SelectedFacebookBrowser) + "&hasUnity=1", 600, 300, Url)%>
            <div id="facebookBrowsersHasUnityFunnelChart">
            </div>
        </div>

        <div class="chartHolder">
            <%= Html.RenderChart("facebookBrowsersHasUnityFunnelLineChart", Url.Action("GetInstallFlowLine", "StatisticsUnityConversionFunnel") + "?channel=1&selectedBrowser=" + ((string)ViewBag.SelectedFacebookBrowser) + "&hasUnity=1", 600, 300, Url)%>
            <div id="facebookBrowsersHasUnityFunnelLineChart">
            </div>
        </div>

        <div class="clear"></div>

        <h1>Portal</h1>

        <div class="chartHolder">
            <%= Html.RenderChart("portalOsDistributionChart", Url.Action("GetOsDistribution", "StatisticsUnityConversionFunnel") + "?channel=0", 600, 300, Url)%>
            <div id="portalOsDistributionChart" style="margin: 0 0 0 0;">
            </div>
        </div>

        <div class="chartHolder">
            <%= Html.RenderChart("portalWindowsBrowserDistributionChart", Url.Action("GetBrowserDistribution", "StatisticsUnityConversionFunnel") + "?channel=0&os=Windows", 600, 300, Url)%>
            <div id="portalWindowsBrowserDistributionChart" style="margin: 0 0 0 0;">
            </div>
        </div>

        <div class="chartHolder">
            <%= Html.RenderChart("portalMacBrowserDistributionChart", Url.Action("GetBrowserDistribution", "StatisticsUnityConversionFunnel") + "?channel=0&os=Mac", 600, 300, Url)%>
            <div id="portalMacBrowserDistributionChart" style="margin: 0 0 0 0;">
            </div>
        </div>

        <div class="clear"></div>

        <form id="portalBrowsersForm" method="post" action="" style="text-align:left; margin:15px 0 25px 0;">
            <span style="font-weight:bold;">Browser:</span> <%= Html.DropDownList("portalBrowsers", ViewData["portalBrowsers"] as SelectList, new { onchange = "updatePortalBrowsersCharts();" })%>
        </form>

        <div class="chartHolder">
            <%= Html.RenderChart("portalBrowsersNoUnityFunnelChart", Url.Action("GetInstallFlow", "StatisticsUnityConversionFunnel") + "?channel=0&selectedBrowser=" + ((string)ViewBag.SelectedPortalBrowser) + "&hasUnity=0", 600, 300, Url)%>
            <div id="portalBrowsersNoUnityFunnelChart">
            </div>
        </div>

        <div class="chartHolder">
            <%= Html.RenderChart("portalBrowsersNoUnityFunnelLineChart", Url.Action("GetInstallFlowLine", "StatisticsUnityConversionFunnel") + "?channel=0&selectedBrowser=" + ((string)ViewBag.SelectedPortalBrowser) + "&hasUnity=0", 600, 300, Url)%>
            <div id="portalBrowsersNoUnityFunnelLineChart">
            </div>
        </div>

        <div class="chartHolder">
            <%= Html.RenderChart("portalBrowsersHasUnityFunnelChart", Url.Action("GetInstallFlow", "StatisticsUnityConversionFunnel") + "?channel=0&selectedBrowser=" + ((string)ViewBag.SelectedPortalBrowser) + "&hasUnity=1", 600, 300, Url)%>
            <div id="portalBrowsersHasUnityFunnelChart">
            </div>
        </div>

        <div class="chartHolder">
            <%= Html.RenderChart("portalBrowsersHasUnityFunnelLineChart", Url.Action("GetInstallFlowLine", "StatisticsUnityConversionFunnel") + "?channel=0&selectedBrowser=" + ((string)ViewBag.SelectedPortalBrowser) + "&hasUnity=1", 600, 300, Url)%>
            <div id="portalBrowsersHasUnityFunnelLineChart">
            </div>
        </div>

        <div class="clear"></div>

        <h1>Custom query</h1>

        <form id="customQueryUnityConversionFunnelForm" method="post" action="">
            <table style="margin:0 0 25px 0;">
                <tr>
                    <td>Channel</td>
                    <td>Os name</td>
                    <td>Os version</td>
                    <td>Browser name</td>
                    <td>Browser version</td>
                    <td>Referrer</td>
                    <td>Java install</td>
                    <td></td>
                </tr>
                <tr>
                    <td><%= Html.DropDownList("customQueryChannel", ViewData["customQueryChannel"] as SelectList, new { onchange = "updateCustomQueryOsName(updateCustomQueryOsVersionCallback(false));", style = "110px" })%></td>
                    <td><%= Html.DropDownList("customQueryOsName", ViewData["customQueryOsName"] as SelectList, new { onchange = "updateCustomQueryOsVersion(updateCustomQueryBrowserNameCallback(false));", style = "width:115px;" })%></td>
                    <td><%= Html.DropDownList("customQueryOsVersion", ViewData["customQueryOsVersion"] as SelectList, new { onchange = "updateCustomQueryBrowserName(updateCustomQueryBrowserVersionCallback(false));", style = "width:70px;" })%></td>
                    <td><%= Html.DropDownList("customQueryBrowserName", ViewData["customQueryBrowserName"] as SelectList, new { onchange = "updateCustomQueryBrowserVersion();", style = "width:123px;" })%></td>
                    <td><%= Html.DropDownList("customQueryBrowserVersion", ViewData["customQueryBrowserVersion"] as SelectList, new { style = "width:90px;" })%></td>
                    <td><%= Html.DropDownList("customQueryReferrer", ViewData["customQueryReferrer"] as SelectList, new { style = "width:60px;" })%></td>
                    <td><%= Html.DropDownList("customQueryJava", ViewData["customQueryJava"] as SelectList, new { style = "width:50px;" })%></td>
                    <td><input type="submit" id="getCustomQueryButton" onclick='updateCustomQueryCharts(); return false;' value="Search" /></td>
                </tr>
            </table>
        </form>

        <div class="clear"></div>

        <div class="chartHolder">
            <%= Html.RenderChart("customQueryNoUnityFunnelChart", Url.Action("GetCustomInstallFlow", "StatisticsUnityConversionFunnel") + ((string)ViewBag.CustomQueryChartBaseUrl) + "&hasUnity=0", 600, 300, Url)%>
            <div id="customQueryNoUnityFunnelChart">
            </div>
        </div>

        <div class="chartHolder">
            <%= Html.RenderChart("customQueryNoUnityFunnelLineChart", Url.Action("GetCustomInstallFlowLine", "StatisticsUnityConversionFunnel") + ((string)ViewBag.CustomQueryChartBaseUrl) + "&hasUnity=0", 600, 300, Url)%>
            <div id="customQueryNoUnityFunnelLineChart">
            </div>
        </div>

        <div class="chartHolder">
            <%= Html.RenderChart("customQueryHasUnityFunnelChart", Url.Action("GetCustomInstallFlow", "StatisticsUnityConversionFunnel") + ((string)ViewBag.CustomQueryChartBaseUrl) + "&hasUnity=1", 600, 300, Url)%>
            <div id="customQueryHasUnityFunnelChart">
            </div>
        </div>

        <div class="chartHolder">
            <%= Html.RenderChart("customQueryHasUnityFunnelLineChart", Url.Action("GetCustomInstallFlowLine", "StatisticsUnityConversionFunnel") + ((string)ViewBag.CustomQueryChartBaseUrl) + "&hasUnity=1", 600, 300, Url)%>
            <div id="customQueryHasUnityFunnelLineChart">
            </div>
        </div>

        <div class="clear"></div>

        <h1>Tracking testing</h1>

        <div style="text-align:left;">

            <p>Ignore this section if you're just looking at the stats :)<span id="readTrackingCookieSpan" style="display:none;"> <a href="#" onclick="readTrackingCookie(trackingCookieName); return false;">Click here to read the tracking cookie</a>.</span></p>

            <p style="margin:30px 0 0 0;">Remember that you need to test with a <span class="bold">new</span> user in order to record the steps.</p>

            <div id="cookieDetectedDiv" style="display:none; margin:30px 0 0 0;">
                <p>We detected a tracking Id on your browser and will display automatically the steps linked to it (<a href="#" onclick="deleteTrackingCookies(trackingCookieName, trackingStateCookieName); return false;">click here to delete the tracking cookies</a>). You can still use another tracking Id thanks to the texbox below:</p>
            </div>

            <div style="margin:30px 0 35px 25px;">
                Tracking Id: <%= Html.TextBox("trackingId", "", new { style="width:300px" })%> <input type="submit" id="getTrackingStepsButton" onclick='getTrackingStepsFromCookieOrField(trackingCookieName); return false;' value="Search" /> <img id="trackingIdLoadingImg" style="display:none;" src="<%= Url.Content("~/Content/img/ajax-loader-pac-man.gif")%>" alt="Loader" />
            </div>

            <div id="trackingStepsDiv" style="height:350px;">
            </div>

            <script type="text/javascript">
                var trackingCookieName = '<%=ViewBag.TrackingCookieName %>';
                var trackingStateCookieName = '<%=ViewBag.TrackingStateCookieName %>';

                readTrackingCookie(trackingCookieName);
            </script>

        </div>

    </div>

</asp:Content>