<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content2" ContentPlaceHolderID="HeadContent" runat="server">
    <% Html.RenderPartial("ChartJavaScriptControl", ViewData); %>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">

<script type="text/javascript">
    function updateCharts() {
        updateChart('dauMauChart', '<%= Url.Action("GetDauMau", "Statistic") %>?' + $('#StatsCalendar').serialize());
        updateChart('dailyRevenueChart', '<%= Url.Action("GetDailyRevenue", "Statistic") %>?' + $('#StatsCalendar').serialize());
        updateChart('retentionCohortsChart', '<%= Url.Action("GetRetentionCohorts", "StatisticsRetention") %>?' + $('#StatsCalendar').serialize());
    }
</script>

<% Html.RenderPartial("StatsCalendar", ViewData); %>

<div id="statsDiv" style="width:1200px;">

    <div class="chartHolder">
        <%= Html.RenderChart("dauMauChart", Url.Action("GetDauMau", "Statistic"), 600, 300, Url)%>
        <div id="dauMauChart" style="margin: 0 0 0 0;">
        </div>
    </div>

    <div class="chartHolder">
        <%= Html.RenderChart("dailyRevenueChart", Url.Action("GetDailyRevenue", "Statistic"), 600, 300, Url)%>
        <div id="dailyRevenueChart" style="margin: 0 0 0 0;">
        </div>
    </div>

    <div class="chartHolder">
        <%= Html.RenderChart("retentionCohortsChart", Url.Action("GetRetentionCohorts", "StatisticsRetention"), 600, 300, Url)%>
        <div id="retentionCohortsChart">
        </div>
    </div>

</div>

</asp:Content>