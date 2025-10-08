<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content2" ContentPlaceHolderID="HeadContent" runat="server">
    <% Html.RenderPartial("ChartJavaScriptControl", ViewData); %>
    <script type="text/javascript" src="<%= Url.Content("~/Scripts/js/stats.js")%>"></script>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">

    <script type="text/javascript">
        function updateCharts() {
            updateMapUsage();
            updateChart('dauChart', '<%= Url.Action("GetDau", "Statistic") %>?' + $('#StatsCalendar').serialize());
            updateChart('newVersusReturningChart', '<%= Url.Action("GetNewVsReturning", "Statistic") %>/?' + $('#StatsCalendar').serialize());
            updateChart('mauChart', '<%= Url.Action("GetMau", "Statistic") %>?' + $('#StatsCalendar').serialize());
            updateChart('playRateChart', '<%= Url.Action("GetPlayRate", "Statistic") %>?' + $('#StatsCalendar').serialize());
            updateChart('installFlowNoUnityChart', '<%= Url.Action("GetInstallFlow", "Statistic") %>?' + $('#StatsCalendar').serialize() + '&hasUnity=0');
            updateChart('installFlowNotUnityLineChart', '<%= Url.Action("GetInstallFlowLine", "Statistic") %>?' + $('#StatsCalendar').serialize() + '&hasUnity=0');
            updateChart('installFlowHasUnityChart', '<%= Url.Action("GetInstallFlow", "Statistic") %>?' + $('#StatsCalendar').serialize() + '&hasUnity=1');
            updateChart('installFlowHasUnityLineChart', '<%= Url.Action("GetInstallFlowLine", "Statistic") %>?' + $('#StatsCalendar').serialize() + '&hasUnity=1');
        }

        function updateMapUsage() {
            updateStatsMetrics(['mapOneUsage', 'mapTwoUsage', 'mapThreeUsage'],
                                ['mapUsageForm'],
                                '/Statistic/GetMapUsageSelect',
                                function () { updateMapUsageChart() });
        }

        function updateMapUsageChart() {
            updateChart('mapUsageChart', '<%= Url.Action("GetMapsUsage", "Statistic")%>?' + $('#StatsCalendar').serialize() + '&' + $('#mapUsageForm').serialize());
        }

    </script>

    <% Html.RenderPartial("StatsCalendar", ViewData); %>

    <div id="statsDiv" style="width:1200px;">

        <div class="chartHolder">
            <%= Html.RenderChart("dauChart", Url.Action("GetDau", "Statistic") , 600, 300, Url)%>
            <div id="dauChart" style="margin: 0 0 0 0;">
            </div>
        </div>

        <div class="chartHolder">
            <%= Html.RenderChart("mauChart", Url.Action("GetMau", "Statistic"), 600, 300, Url)%>
            <div id="mauChart" style="margin: 0 0 0 0;">
            </div>
        </div>

        <div class="chartHolder">
            <%= Html.RenderChart("newVersusReturningChart", Url.Action("GetNewVsReturning", "Statistic"), 600, 300, Url)%>
            <div id="newVersusReturningChart" style="margin: 0 0 0 0;">
            </div>
        </div>

        <div class="chartHolder">
            <%= Html.RenderChart("playRateChart", Url.Action("GetPlayRate", "Statistic"), 600, 300, Url)%>
            <div id="playRateChart" style="margin: 0 0 0 0;">
            </div>
        </div>

        <div class="chartHolder">
            <%= Html.RenderChart("installFlowNoUnityChart", Url.Action("GetInstallFlow", "Statistic") + "?hasUnity=0", 600, 300, Url)%>
            <div id="installFlowNoUnityChart">
            </div>
        </div>

        <div class="chartHolder">
            <%= Html.RenderChart("installFlowNotUnityLineChart", Url.Action("GetInstallFlowLine", "Statistic") + "?hasUnity=0", 600, 300, Url)%>
            <div id="installFlowNotUnityLineChart">
            </div>
        </div>

        <div class="chartHolder">
            <%= Html.RenderChart("installFlowHasUnityChart", Url.Action("GetInstallFlow", "Statistic") + "?hasUnity=1", 600, 300, Url)%>
            <div id="installFlowHasUnityChart">
            </div>
        </div>

        <div class="chartHolder">
            <%= Html.RenderChart("installFlowHasUnityLineChart", Url.Action("GetInstallFlowLine", "Statistic") + "?hasUnity=1", 600, 300, Url)%>
            <div id="installFlowHasUnityLineChart">
            </div>
        </div>

        <h1>Map usage</h1>

        <div class="left" style="width:600px; height:410px;">

            <form id="mapUsageForm" method="post" action="">
                <span style="color:#F06F30; font-weight:bold;">Map 1:</span> <%= Html.DropDownList("mapOneUsage", ViewData["mapOneUsage"] as SelectList, new { onchange = "updateMapUsageChart();" })%><br />
                <span style="color:#63BE6C; font-weight:bold;">Map 2:</span> <%= Html.DropDownList("mapTwoUsage", ViewData["mapTwoUsage"] as SelectList, new { onchange = "updateMapUsageChart();" })%><br />
                <span style="color:#78B3E5; font-weight:bold;">Map 3:</span> <%= Html.DropDownList("mapThreeUsage", ViewData["mapThreeUsage"] as SelectList, new { onchange = "updateMapUsageChart();" })%>
            </form>

            <div class="chartHolder">
                <%= Html.RenderChart("mapUsageChart", Url.Action("GetMapsUsage", "Statistic") + "?mapOneUsage=" + ((string)ViewBag.SelectedMapOneUsage), 600, 300, Url)%>
                <div id="mapUsageChart">
                </div>
            </div>

        </div>

    </div>

</asp:Content>