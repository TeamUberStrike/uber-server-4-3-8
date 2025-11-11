<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content2" ContentPlaceHolderID="HeadContent" runat="server">
    <% Html.RenderPartial("ChartJavaScriptControl", ViewData); %>
    <script type="text/javascript" src="<%= Url.Content("~/Scripts/js/stats.js")%>?v=<%= ViewBag.PublishVersion %>"></script>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <script type="text/javascript">
        function updateCharts() {
            updateDailyRevenue();
            updateActivityMetrics();
        }

        function updateDailyRevenue() {
            updateStatsMetrics(['regionOneDailyRevenue', 'regionTwoDailyRevenue', 'regionThreeDailyRevenue'],
                                ['dailyRevenueForm'],
                                '/StatisticsRegions/GetCountriesByRevenueDropDownList',
                                function () { updateDailyRevenueChart() });

            updateDailyRevenueTable();
        }

        function updateDailyRevenueChart() {
            updateChart('dailyRevenueChart', '<%= Url.Action("GetDailyRevenue", "StatisticsRegions") %>?' + $('#StatsCalendar').serialize() + '&' + $('#dailyRevenueForm').serialize());
        }

        function updateDailyRevenueTable() {
            updateStatsMetricsTable('dailyRevenueListDiv', 'dailyRevenueListLoadingImg', '/StatisticsRegions/GetCountriesByRevenueList', 'Name', 'Revenue (USD)');
        }

        function updateActivityMetrics() {
            updateStatsMetrics(['regionDau', 'regionMau', 'regionDailyPlayRate', 'regionDarpu'],
                                ['dauForm', 'mauForm', 'dailyPlayRateForm', 'darpuForm'],
                                '/StatisticsRegions/GetCountriesByMauDropDownList',
                                function () { updateDauChart(); updateMauChart(); updateDailyPlayRateChart(); updateDarpuChart(); });

            updateMauTable();
        }

        function updateDauChart() {
            updateChart('dauChart', '<%= Url.Action("GetDau", "StatisticsRegions") %>?' + $('#StatsCalendar').serialize() + '&' + $('#dauForm').serialize());
        }

        function updateMauChart() {
            updateChart('mauChart', '<%= Url.Action("GetMau", "StatisticsRegions") %>?' + $('#StatsCalendar').serialize() + '&' + $('#mauForm').serialize());
        }

        function updateMauTable() {
            updateStatsMetricsTable('mauListDiv', 'mauListLoadingImg', '/StatisticsRegions/GetCountriesByMauList', 'Name', 'MAU');
        }

        function updateDailyPlayRateChart() {
            updateChart('dailyPlayRateChart', '<%= Url.Action("GetDailyPlayRate", "StatisticsRegions") %>?' + $('#StatsCalendar').serialize() + '&' + $('#dailyPlayRateForm').serialize());
        }

        function updateDarpuChart() {
            updateChart('darpuChart', '<%= Url.Action("GetDarpu", "StatisticsRegions") %>?' + $('#StatsCalendar').serialize() + '&' + $('#darpuForm').serialize());
        }

    </script>
    
    <% Html.RenderPartial("StatsCalendar", ViewData); %>

    <div id="statsDiv" style="width:1300px; text-align:left;">
        
        <div style="width:600px; height:410px;">

            <form id="dailyRevenueForm" method="post" action="">
                <span style="color:#F06F30; font-weight:bold;">Region 1:</span> <%= Html.DropDownList("regionOneDailyRevenue", ViewData["regionOneDailyRevenue"] as SelectList, new { onchange = "updateDailyRevenueChart();" })%><br />
                <span style="color:#63BE6C; font-weight:bold;">Region 2:</span> <%= Html.DropDownList("regionTwoDailyRevenue", ViewData["regionTwoDailyRevenue"] as SelectList, new { onchange = "updateDailyRevenueChart();" })%><br />
                <span style="color:#78B3E5; font-weight:bold;">Region 3:</span> <%= Html.DropDownList("regionThreeDailyRevenue", ViewData["regionThreeDailyRevenue"] as SelectList, new { onchange = "updateDailyRevenueChart();" })%>
            </form>

            <div class="chartHolder">
                <%= Html.RenderChart("dailyRevenueChart", Url.Action("GetDailyRevenue", "StatisticsRegions") + "?regionOneDailyRevenue=" + ((string)ViewBag.SelectedRegionOneDailyRevenue) + "&regionTwoDailyRevenue=-1&regionThreeDailyRevenue=-1", 600, 300, Url)%>
                <div id="dailyRevenueChart">
                </div>
            </div>

        </div>

        <div class="clear"></div>

        <div>
            <div id="dailyRevenueListDiv" style="display:none;">
            </div>
            <img id="dailyRevenueListLoadingImg" style="display:none;" src="<%= Url.Content("~/Content/img/ajax-loader-pac-man.gif")%>" alt="Loader" />
            <script type="text/javascript">
                updateDailyRevenueTable();
            </script>
        </div>

        <div class="clear"></div>

        <div style="margin:100px 0 0 0;">

            <div class="left" style="width:600px; height:410px;">
            
                <form id="dauForm" method="post" action="">
                    <span style="font-weight:bold;">Dau for region:</span> <%= Html.DropDownList("regionDau", ViewData["regionDau"] as SelectList, new { onchange = "updateDauChart();" })%>
                </form>

                <div class="chartHolder">
                    <%= Html.RenderChart("dauChart", Url.Action("GetDau", "StatisticsRegions") + "?regionDau=" + ((string)ViewBag.SelectedDauRegion), 600, 300, Url)%>
                    <div id="dauChart">
                    </div>
                </div>

            </div>

            <div class="left" style="width:600px; height:410px;">

                <form id="mauForm" method="post" action="">
                    <span style="font-weight:bold;">Mau for region:</span> <%= Html.DropDownList("regionMau", ViewData["regionMau"] as SelectList, new { onchange = "updateMauChart();" })%>
                </form>

                <div class="chartHolder">
                    <%= Html.RenderChart("mauChart", Url.Action("GetMau", "StatisticsRegions") + "?regionMau=" + ((string)ViewBag.SelectedMauRegion), 600, 300, Url)%>
                    <div id="mauChart">
                    </div>
                </div>

            </div>

        </div>

        <div class="clear"></div>

        <div>
            <div id="mauListDiv" style="display:none;">
            </div>
            <img id="mauListLoadingImg" style="display:none;" src="<%= Url.Content("~/Content/img/ajax-loader-pac-man.gif")%>" alt="Loader" />
            <script type="text/javascript">
                updateMauTable();
            </script>
        </div>

        <div class="clear"></div>

        <div style="margin:100px 0 0 0;">

            <div class="left" style="width:600px; height:410px;">

                <form id="dailyPlayRateForm" method="post" action="">
                    <span style="font-weight:bold;">Daily play rate for region:</span> <%= Html.DropDownList("regionDailyPlayRate", ViewData["regionDailyPlayRate"] as SelectList, new { onchange = "updateDailyPlayRateChart();" })%>
                </form>

                <div class="chartHolder">
                    <%= Html.RenderChart("dailyPlayRateChart", Url.Action("GetDailyPlayRate", "StatisticsRegions") + "?regionDailyPlayRate=" + ((string)ViewBag.SelectedDailyPlayRateRegion), 600, 300, Url)%>
                    <div id="dailyPlayRateChart">
                    </div>
                </div>

            </div>

            <div class="left" style="width:600px; height:410px;">

                <form id="darpuForm" method="post" action="">
                    <span style="font-weight:bold;">Darpu for region:</span> <%= Html.DropDownList("regionDarpu", ViewData["regionDarpu"] as SelectList, new { onchange = "updateDarpuChart();" })%>
                </form>

                <div class="chartHolder">
                    <%= Html.RenderChart("darpuChart", Url.Action("GetDarpu", "StatisticsRegions") + "?regionDarpu=" + ((string)ViewBag.SelectedDarpuRegion), 600, 300, Url)%>
                    <div id="darpuChart">
                    </div>
                </div>

            </div>

        </div>

     </div>

</asp:Content>