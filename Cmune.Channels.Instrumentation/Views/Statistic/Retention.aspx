<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <% Html.RenderPartial("ChartJavaScriptControl", ViewData); %>
    <script type="text/javascript" src="<%= Url.Content("~/Scripts/js/stats.js")%>?v=<%= ViewBag.PublishVersion %>"></script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <script type="text/javascript">
        function updateCharts() {
            updateChart('retentionCohortsChart', '<%= Url.Action("GetRetentionCohorts", "StatisticsRetention") %>?' + $('#StatsCalendar').serialize());

            <% foreach (ChannelType channel in AdminConfig.ActiveStatsChannels) { %>
            updateChart('<%: String.Format("{0}RetentionCohortsChart", channel) %>', '<%= Url.Action("GetRetentionCohorts", "StatisticsRetention") %>?' + $('#StatsCalendar').serialize() + '&channel=<%: (int) channel %>');
            <% } %>

            updateRegionRetentionChart();
            updateChart('retentionCohortsApplifierChart', '<%= Url.Action("GetRetentionCohortsByReferrer", "StatisticsRetention") %>?' + $('#StatsCalendar').serialize() + '&referrerId=5');
        }

        function updateRegionRetentionChart() {
            updateChart('retentionCohortsRegionChart', '<%= Url.Action("GetRetentionCohortsByRegion", "StatisticsRetention") %>?' + $('#StatsCalendar').serialize() + '&regionId=' + $('#regionRetentionCohorts').val());
        }

    </script>

    <% Html.RenderPartial("StatsCalendar", ViewData); %>

    <div id="statsDiv" style="width:1200px; text-align:left;">

        <p style="margin:0 0 15px 0;">The data started to be collected on the 5th of September 2011.</p>

        <div style="width:600px; margin:auto;">
            <%= Html.RenderChart("retentionCohortsChart", Url.Action("GetRetentionCohorts", "StatisticsRetention"), 600, 300, Url)%>
            <div id="retentionCohortsChart">
            </div>
        </div>

        <div class="clear"></div>

        <h1>Channels</h1>

        <% foreach (ChannelType channel in AdminConfig.ActiveStatsChannels) { %>
        <div class="chartHolder">
            <%= Html.RenderChart(String.Format("{0}RetentionCohortsChart", channel), Url.Action("GetRetentionCohortsByChannel", "StatisticsRetention") + "?channel=" + (int)channel, 600, 300, Url)%>
            <div id="<%: String.Format("{0}RetentionCohortsChart", channel) %>">
            </div>
        </div>
        <% } %>

        <div class="clear"></div>

        <h1>Retention cohorts by region</h1>

        <div style="width:600px; margin:auto;">
            <form id="retentionCohortsByRegionForm" method="post" action="" style="margin:0 0 15px 0;">
                <span style="font-weight:bold;">Retention cohorts for region:</span> <%= Html.DropDownList("regionRetentionCohorts", ViewData["regionRetentionCohorts"] as SelectList, new { onchange = "updateRegionRetentionChart();" })%>
            </form>

            <%= Html.RenderChart("retentionCohortsRegionChart", Url.Action("GetRetentionCohortsByRegion", "StatisticsRetention") + "?regionId=" + (string) ViewBag.BiggestCountry, 600, 300, Url)%>
            <div id="retentionCohortsRegionChart">
            </div>
        </div>

        <div class="clear"></div>

        <h1>Retention cohorts by referrer</h1>

        <div class="chartHolder">
            <%= Html.RenderChart("retentionCohortsApplifierChart", Url.Action("GetRetentionCohortsByReferrer", "StatisticsRetention") + "?referrerId=5", 600, 300, Url)%>
            <div id="retentionCohortsApplifierChart">
            </div>
        </div>

        <div class="clear"></div>

    </div>

</asp:Content>