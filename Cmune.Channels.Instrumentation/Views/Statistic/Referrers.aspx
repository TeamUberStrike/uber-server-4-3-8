<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content2" ContentPlaceHolderID="HeadContent" runat="server">
    <% Html.RenderPartial("ChartJavaScriptControl", ViewData); %>
    <script type="text/javascript" src="<%= Url.Content("~/Scripts/js/stats.js")%>"></script>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    
    <script type="text/javascript">
        function updateCharts() {
            updateChartsOnly();
            updateReferrerContributionChart();
        }

        function updateChartsOnly() {
            updateDailyRevenueChart();
            updateDauChart();
            updateMauChart();
            updateDailyPlayRateChart();
            updateReferrerNewMembersChart();
        }

        function updateDailyRevenueChart() {
            updateChart('referrerDailyRevenueChart', '<%= Url.Action("GetDailyRevenue", "StatisticsReferrers") %>?' + $('#StatsCalendar').serialize() + '&' + $('#referrerForm').serialize());
        }

        function updateDauChart() {
            updateChart('referrerDauChart', '<%= Url.Action("GetDau", "StatisticsReferrers") %>?' + $('#StatsCalendar').serialize() + '&' + $('#referrerForm').serialize());
        }

        function updateMauChart() {
            updateChart('referrerMauChart', '<%= Url.Action("GetMau", "StatisticsReferrers") %>?' + $('#StatsCalendar').serialize() + '&' + $('#referrerForm').serialize());
        }

        function updateDailyPlayRateChart() {
            updateChart('referrerDailyPlayRateChart', '<%= Url.Action("GetDailyPlayRate", "StatisticsReferrers") %>?' + $('#StatsCalendar').serialize() + '&' + $('#referrerForm').serialize());
        }

        function updateReferrerContributionChart() {
            updateChart('referrerContributionChart', '<%= Url.Action("GetReferrerContribution", "StatisticsReferrers") %>?' + $('#StatsCalendar').serialize() + '&' + $('#referrerForm').serialize());
        }

        function updateReferrerNewMembersChart() {
            updateChart('referrerNewMembersChart', '<%= Url.Action("GetNewMembers", "StatisticsReferrers") %>?' + $('#StatsCalendar').serialize() + '&' + $('#referrerForm').serialize());
        }

    </script>

    <% Html.RenderPartial("StatsCalendar", ViewData); %>

    <div id="statsDiv" style="width:1300px; text-align:left;">

        <form id="referrerForm" method="post" action="">
            <span style="color:#F06F30; font-weight:bold;">Referrer 1:</span> <%= Html.DropDownList("referrerOne", ViewData["referrerOne"] as SelectList, new { onchange = "updateChartsOnly();" })%><br />
            <span style="color:#63BE6C; font-weight:bold;">Referrer 2:</span> <%= Html.DropDownList("referrerTwo", ViewData["referrerTwo"] as SelectList, new { onchange = "updateChartsOnly();" })%><br />
            <span style="color:#78B3E5; font-weight:bold;">Referrer 3:</span> <%= Html.DropDownList("referrerThree", ViewData["referrerThree"] as SelectList, new { onchange = "updateChartsOnly();" })%>
        </form>

        <div class="chartHolder">
            <%= Html.RenderChart("referrerDailyRevenueChart", Url.Action("GetDailyRevenue", "StatisticsReferrers") + "?referrerOne=" + ((string)ViewBag.SelectedReferrerOne) + "&referrerTwo=" + ((string)ViewBag.SelectedReferrerTwo), 600, 300, Url)%>
            <div id="referrerDailyRevenueChart">
            </div>
        </div>

        <div class="chartHolder">
            <%= Html.RenderChart("referrerContributionChart", Url.Action("GetReferrerContribution", "StatisticsReferrers"), 600, 300, Url)%>
            <div id="referrerContributionChart" style="margin: 0 0 0 0;">
            </div>
        </div>

        <div class="chartHolder">
            <%= Html.RenderChart("referrerDauChart", Url.Action("GetDau", "StatisticsReferrers") + "?referrerOne=" + ((string)ViewBag.SelectedReferrerOne) + "&referrerTwo=" + ((string)ViewBag.SelectedReferrerTwo), 600, 300, Url)%>
            <div id="referrerDauChart">
            </div>
        </div>

        <div class="chartHolder">
            <%= Html.RenderChart("referrerMauChart", Url.Action("GetMau", "StatisticsReferrers") + "?referrerOne=" + ((string)ViewBag.SelectedReferrerOne) + "&referrerTwo=" + ((string)ViewBag.SelectedReferrerTwo), 600, 300, Url)%>
            <div id="referrerMauChart">
            </div>
        </div>

        <div class="chartHolder">
            <%= Html.RenderChart("referrerDailyPlayRateChart", Url.Action("GetDailyPlayRate", "StatisticsReferrers") + "?referrerOne=" + ((string)ViewBag.SelectedReferrerOne) + "&referrerTwo=" + ((string)ViewBag.SelectedReferrerTwo), 600, 300, Url)%>
            <div id="referrerDailyPlayRateChart">
            </div>
        </div>

        <div class="chartHolder">
            <%= Html.RenderChart("referrerNewMembersChart", Url.Action("GetNewMembers", "StatisticsReferrers") + "?referrerOne=" + ((string)ViewBag.SelectedReferrerOne) + "&referrerTwo=" + ((string)ViewBag.SelectedReferrerTwo), 600, 300, Url)%>
            <div id="referrerNewMembersChart">
            </div>
        </div>

    </div>

</asp:Content>