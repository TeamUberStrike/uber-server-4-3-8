<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
    <% Html.RenderPartial("ChartJavaScriptControl", ViewData); %>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <script type="text/javascript">
        function updateCharts() {
            updateChart('dauChart', '<%= Url.Action("GetDau", "StatisticsChannels") %>?' + $('#StatsCalendar').serialize() + '&channel=<%= ViewBag.Channel %>');
            updateChart('mauChart', '<%= Url.Action("GetMau", "StatisticsChannels") %>?' + $('#StatsCalendar').serialize() + '&channel=<%= ViewBag.Channel %>');
            updateChart('newVersusReturningChart', '<%= Url.Action("GetNewVersusReturning", "StatisticsChannels") %>/?' + $('#StatsCalendar').serialize() + '&channel=<%= ViewBag.Channel %>');
            updateChart('dailyPlayRateChart', '<%= Url.Action("GetDailyPlayRate", "StatisticsChannels") %>?' + $('#StatsCalendar').serialize() + '&channel=<%= ViewBag.Channel %>');
            updateChart('revenueChart', '<%= Url.Action("GetRevenue", "StatisticsChannels") %>?' + $('#StatsCalendar').serialize() + '&channel=<%= ViewBag.Channel %>');
            updateChart('darpuChart', '<%= Url.Action("GetDarpu", "StatisticsChannels") %>?' + $('#StatsCalendar').serialize() + '&channel=<%= ViewBag.Channel %>');
            updateChart('dailyTransactionsChart', '<%= Url.Action("GetDailyTransactions", "StatisticsChannels") %>?' + $('#StatsCalendar').serialize() + '&channel=<%= ViewBag.Channel %>');
            updateChart('dailyConversionToPayingChart', '<%= Url.Action("GetDailyConversionToPaying", "StatisticsChannels") %>?' + $('#StatsCalendar').serialize() + '&channel=<%= ViewBag.Channel %>');
            updateChart('darppuChart', '<%= Url.Action("GetDarppu", "StatisticsChannels") %>?' + $('#StatsCalendar').serialize() + '&channel=<%= ViewBag.Channel %>');
            updateChart('packagesContributionByRevenueChart', '<%= Url.Action("GetPackagesContributionByRevenue", "StatisticsChannels") %>?' + $('#StatsCalendar').serialize() + '&channel=<%= ViewBag.Channel %>');
            updateChart('packagesContributionByVolumeChart', '<%= Url.Action("GetPackagesContributionByVolume", "StatisticsChannels") %>?' + $('#StatsCalendar').serialize() + '&channel=<%= ViewBag.Channel %>');
            updateChart('bundleContributionByRevenueChart', '<%= Url.Action("GetBundleContributionByRevenue", "StatisticsChannels") %>?' + $('#StatsCalendar').serialize() + '&channel=<%= ViewBag.Channel %>');
            updateChart('bundleContributionByVolumeChart', '<%= Url.Action("GetBundleContributionByVolume", "StatisticsChannels") %>?' + $('#StatsCalendar').serialize() + '&channel=<%= ViewBag.Channel %>');
            updateChart('creditsSalesChart', '<%= Url.Action("GetCreditsSales", "StatisticsChannels") %>?' + $('#StatsCalendar').serialize() + '&channel=<%= ViewBag.Channel %>');
            updateChart('bundlesSalesChart', '<%= Url.Action("GetBundlesSales", "StatisticsChannels") %>?' + $('#StatsCalendar').serialize() + '&channel=<%= ViewBag.Channel %>');

            <% if ((ChannelType) ViewBag.Channel != ChannelType.OSXStandalone) { %>

            updateChart('installTrackingNoUnityChart', '<%= Url.Action("GetInstallTracking", "StatisticsChannels") %>?' + $('#StatsCalendar').serialize() + '&channel=<%= ViewBag.Channel %>&hasUnity=0');
            updateChart('installTrackingNoUnityLineChart', '<%= Url.Action("GetInstallTrackingLine", "StatisticsChannels") %>?' + $('#StatsCalendar').serialize() + '&channel=<%= ViewBag.Channel %>&hasUnity=0');
            updateChart('installTrackingHasUnityChart', '<%= Url.Action("GetInstallTracking", "StatisticsChannels") %>?' + $('#StatsCalendar').serialize() + '&channel=<%= ViewBag.Channel %>&hasUnity=1');
            updateChart('installTrackingHasUnityLineChart', '<%= Url.Action("GetInstallTrackingLine", "StatisticsChannels") %>?' + $('#StatsCalendar').serialize() + '&channel=<%= ViewBag.Channel %>&hasUnity=1');

            <%} %>
        }
    </script>

    <div class="Menu">
        <% foreach (ChannelType channel in AdminConfig.ActiveStatsChannels)
           { %>
           <a href="<%= Url.Action("Channels","Statistic") %>?channel=<%= (int) channel  %>" class="<%= (int) ViewBag.Channel == (int) channel ? "submenuActiveTab":"" %>"><%: channel %></a>
        <%} %>
    </div>

    <% Html.RenderPartial("StatsCalendar", ViewData); %>

    <div id="statsDiv" style="width:1200px; text-align:left;">

        <h1>Activity</h1>

        <div class="chartHolder">
            <%= Html.RenderChart("dauChart", Url.Action("GetDau", "StatisticsChannels") + "?channel=" + ((int) ViewBag.Channel).ToString(), 600, 300, Url)%>
            <div id="dauChart">
            </div>
        </div>

        <div class="chartHolder">
            <%= Html.RenderChart("mauChart", Url.Action("GetMau", "StatisticsChannels") + "?channel=" + ((int)ViewBag.Channel).ToString(), 600, 300, Url)%>
            <div id="mauChart">
            </div>
        </div>

        <div class="chartHolder">
            <%= Html.RenderChart("newVersusReturningChart", Url.Action("GetNewVersusReturning", "StatisticsChannels") + "?channel=" + ((int)ViewBag.Channel).ToString(), 600, 300, Url)%>
            <div id="newVersusReturningChart">
            </div>
        </div>

        <div class="chartHolder">
            <%= Html.RenderChart("dailyPlayRateChart", Url.Action("GetDailyPlayRate", "StatisticsChannels") + "?channel=" + ((int)ViewBag.Channel).ToString(), 600, 300, Url)%>
            <div id="dailyPlayRateChart">
            </div>
        </div>

        <h1>Revenue</h1>

        <div class="chartHolder">
            <%= Html.RenderChart("revenueChart", Url.Action("GetRevenue", "StatisticsChannels") + "?channel=" + ((int)ViewBag.Channel).ToString(), 600, 300, Url)%>
            <div id="revenueChart">
            </div>
        </div>

        <div class="chartHolder">
            <%= Html.RenderChart("dailyTransactionsChart", Url.Action("GetDailyTransactions", "StatisticsChannels") + "?channel=" + ((int)ViewBag.Channel).ToString(), 600, 300, Url)%>
            <div id="dailyTransactionsChart">
            </div>
        </div>

        <div class="chartHolder">
            <%= Html.RenderChart("darpuChart", Url.Action("GetDarpu", "StatisticsChannels") + "?channel=" + ((int)ViewBag.Channel).ToString(), 600, 300, Url)%>
            <div id="darpuChart">
            </div>
        </div>

        <div class="chartHolder">
            <%= Html.RenderChart("dailyConversionToPayingChart", Url.Action("GetDailyConversionToPaying", "StatisticsChannels") + "?channel=" + ((int)ViewBag.Channel).ToString(), 600, 300, Url)%>
            <div id="dailyConversionToPayingChart">
            </div>
        </div>

        <div class="chartHolder">
            <%= Html.RenderChart("darppuChart", Url.Action("GetDarppu", "StatisticsChannels") + "?channel=" + ((int)ViewBag.Channel).ToString(), 600, 300, Url)%>
            <div id="darppuChart">
            </div>
        </div>

        <div class="clear"></div>

        <div class="chartHolder">
            <%= Html.RenderChart("packagesContributionByRevenueChart", Url.Action("GetPackagesContributionByRevenue", "StatisticsChannels") + "?channel=" + ((int)ViewBag.Channel).ToString(), 600, 300, Url)%>
            <div id="packagesContributionByRevenueChart">
            </div>
        </div>

        <div class="chartHolder">
            <%= Html.RenderChart("packagesContributionByVolumeChart", Url.Action("GetPackagesContributionByVolume", "StatisticsChannels") + "?channel=" + ((int)ViewBag.Channel).ToString(), 600, 300, Url)%>
            <div id="packagesContributionByVolumeChart">
            </div>
        </div>

        <div class="chartHolder">
            <%= Html.RenderChart("bundleContributionByRevenueChart", Url.Action("GetBundleContributionByRevenue", "StatisticsChannels") + "?channel=" + ((int)ViewBag.Channel).ToString(), 600, 300, Url)%>
            <div id="bundleContributionByRevenueChart">
            </div>
        </div>

        <div class="chartHolder">
            <%= Html.RenderChart("bundleContributionByVolumeChart", Url.Action("GetBundleContributionByVolume", "StatisticsChannels") + "?channel=" + ((int)ViewBag.Channel).ToString(), 600, 300, Url)%>
            <div id="bundleContributionByVolumeChart">
            </div>
        </div>

        <div class="clear"></div>

        <div class="chartHolder">
            <%= Html.RenderChart("creditsSalesChart", Url.Action("GetCreditsSales", "StatisticsChannels") + "?channel=" + ((int)ViewBag.Channel).ToString(), 1200, 600, Url)%>
            <div id="creditsSalesChart" style="margin: 0 0 0 0;">
            </div>
        </div>

        <div class="chartHolder">
            <%= Html.RenderChart("bundlesSalesChart", Url.Action("GetBundlesSales", "StatisticsChannels") + "?channel=" + ((int)ViewBag.Channel).ToString(), 1200, 600, Url)%>
            <div id="bundlesSalesChart" style="margin: 0 0 0 0;">
            </div>
        </div>

        <div class="clear"></div>

        <% if ((ChannelType)ViewBag.Channel != ChannelType.MacAppStore)
           { %>

        <h1>Unity conversion funnel</h1>

        <div class="chartHolder">
            <%= Html.RenderChart("installTrackingNoUnityChart", Url.Action("GetInstallTracking", "StatisticsChannels") + "?channel=" + ((int)ViewBag.Channel).ToString() + "&hasUnity=0", 600, 300, Url)%>
            <div id="installTrackingNoUnityChart">
            </div>
        </div>

        <div class="chartHolder">
            <%= Html.RenderChart("installTrackingNoUnityLineChart", Url.Action("GetInstallTrackingLine", "StatisticsChannels") + "?channel=" + ((int)ViewBag.Channel).ToString() + "&hasUnity=0", 600, 300, Url)%>
            <div id="installTrackingNoUnityLineChart">
            </div>
        </div>

        <div class="chartHolder">
            <%= Html.RenderChart("installTrackingHasUnityChart", Url.Action("GetInstallTracking", "StatisticsChannels") + "?channel=" + ((int)ViewBag.Channel).ToString() + "&hasUnity=1", 600, 300, Url)%>
            <div id="installTrackingHasUnityChart">
            </div>
        </div>

        <div class="chartHolder">
            <%= Html.RenderChart("installTrackingHasUnityLineChart", Url.Action("GetInstallTrackingLine", "StatisticsChannels") + "?channel=" + ((int)ViewBag.Channel).ToString() + "&hasUnity=1", 600, 300, Url)%>
            <div id="installTrackingHasUnityLineChart">
            </div>
        </div>

        <% } %>

    </div>

</asp:Content>