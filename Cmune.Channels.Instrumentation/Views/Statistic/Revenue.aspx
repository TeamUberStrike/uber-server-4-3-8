<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
    <% Html.RenderPartial("ChartJavaScriptControl", ViewData); %>
    <script type="text/javascript" src="<%= Url.Content("~/Scripts/js/stats.js")%>?v=<%= ViewBag.PublishVersion %>"></script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <script type="text/javascript">
        function updateCharts() {
            getTotalRevenue();
            updateChart('dailyRevenueChart', '<%= Url.Action("GetDailyRevenue", "Statistic") %>?' + $('#StatsCalendar').serialize());
            updateChart('dailyTransactionsChart', '<%= Url.Action("GetDailyTransactions", "Statistic") %>?' + $('#StatsCalendar').serialize());
            updateChart('dailyAverageRevenuePerUserChart', '<%= Url.Action("GetDailyAverageRevenuePerUser", "Statistic") %>?' + $('#StatsCalendar').serialize());
            updateChart('dailyAverageRevenuePerPayingUserChart', '<%= Url.Action("GetDailyAverageRevenuePerPayingUser", "Statistic") %>?' + $('#StatsCalendar').serialize());
            updateChart('dailyConversionToPayingChart', '<%= Url.Action("GetDailyConversionToPaying", "Statistic") %>?' + $('#StatsCalendar').serialize());            
            updateChart('dailyRevenueByPaymentProviderChart', '<%= Url.Action("GetDailyRevenueByPaymentProvider", "Statistic") %>?' + $('#StatsCalendar').serialize());
            updateChart('dailyRevenueByChannelChart', '<%= Url.Action("GetDailyRevenueByChannel", "Statistic") %>?' + $('#StatsCalendar').serialize());
            updateChart('paymentProviderContributionChart', '<%= Url.Action("GetPaymentProviderContribution", "Statistic") %>?' + $('#StatsCalendar').serialize());
            updateChart('channelContributionChart', '<%= Url.Action("GetChannelContribution", "Statistic") %>?' + $('#StatsCalendar').serialize());
            updateChart('packageContributionByRevenueChart', '<%= Url.Action("GetPackageContributionByRevenue", "Statistic") %>?' + $('#StatsCalendar').serialize());
            updateChart('packageContributionByVolumeChart', '<%= Url.Action("GetPackageContributionByVolume", "Statistic") %>?' + $('#StatsCalendar').serialize());
            updateChart('bundleContributionByRevenueChart', '<%= Url.Action("GetBundleContributionByRevenue", "Statistic") %>?' + $('#StatsCalendar').serialize());
            updateChart('bundleContributionByVolumeChart', '<%= Url.Action("GetBundleContributionByVolume", "Statistic") %>?' + $('#StatsCalendar').serialize());
            updateChart('creditsSalesChart', '<%= Url.Action("GetCreditsSales", "Statistic") %>?' + $('#StatsCalendar').serialize());
            updateChart('bundlesSalesChart', '<%= Url.Action("GetBundlesSales", "Statistic") %>?' + $('#StatsCalendar').serialize());
        }
    </script>
    
    <% Html.RenderPartial("StatsCalendar", ViewData); %>

     <div id="statsDiv" style="width:1200px;">
        
        <p style="margin:0 0 25px 25px; text-align:left;">Total payment over the period: US$<span id="totalPaymentSpan"></span></p>

        <script type="text/javascript">
            getTotalRevenue();
        </script>

        <div class="chartHolder">
            <%= Html.RenderChart("dailyRevenueChart", Url.Action("GetDailyRevenue", "Statistic"), 600, 300, Url)%>
            <div id="dailyRevenueChart" style="margin: 0 0 0 0;">
            </div>
        </div>

        <div class="chartHolder">
            <%= Html.RenderChart("dailyTransactionsChart", Url.Action("GetDailyTransactions", "Statistic"), 600, 300, Url)%>
            <div id="dailyTransactionsChart" style="margin: 0 0 0 0;">
            </div>
        </div>

        <div class="chartHolder">
            <%= Html.RenderChart("dailyAverageRevenuePerUserChart", Url.Action("GetDailyAverageRevenuePerUser", "Statistic"), 600, 300, Url)%>
            <div id="dailyAverageRevenuePerUserChart" style="margin: 0 0 0 0;">
            </div>
        </div>
        
        <div class="chartHolder">
            <%= Html.RenderChart("dailyAverageRevenuePerPayingUserChart", Url.Action("GetDailyAverageRevenuePerPayingUser", "Statistic"), 600, 300, Url)%>
            <div id="dailyAverageRevenuePerPayingUserChart" style="margin: 0 0 0 0;">
            </div>
        </div>

        <div class="chartHolder">
             <%= Html.RenderChart("dailyConversionToPayingChart", Url.Action("GetDailyConversionToPaying", "Statistic"), 600, 300, Url)%>
            <div id="dailyConversionToPayingChart" style="margin: 0 0 0 0;">
            </div>
        </div>

        <div class="chartHolder">
            <%= Html.RenderChart("dailyRevenueByPaymentProviderChart", Url.Action("GetDailyRevenueByPaymentProvider", "Statistic"), 600, 300, Url)%>
            <div id="dailyRevenueByPaymentProviderChart" style="margin: 0 0 0 0;">
            </div>
        </div>

        <div class="chartHolder">
            <%= Html.RenderChart("dailyRevenueByChannelChart", Url.Action("GetDailyRevenueByChannel", "Statistic"), 600, 300, Url)%>
            <div id="dailyRevenueByChannelChart" style="margin: 0 0 0 0;">
            </div>
        </div>

        <div class="clear"></div>

        <div class="chartHolder">
            <%= Html.RenderChart("paymentProviderContributionChart", Url.Action("GetPaymentProviderContribution", "Statistic"), 600, 300, Url)%>
            <div id="paymentProviderContributionChart" style="margin: 0 0 0 0;">
            </div>
        </div>

        <div class="chartHolder">
            <%= Html.RenderChart("channelContributionChart", Url.Action("GetChannelContribution", "Statistic"), 600, 300, Url)%>
            <div id="channelContributionChart" style="margin: 0 0 0 0;">
            </div>
        </div>

        <div class="chartHolder">
            <%= Html.RenderChart("packageContributionByRevenueChart", Url.Action("GetPackageContributionByRevenue", "Statistic"), 600, 300, Url)%>
            <div id="packageContributionByRevenueChart" style="margin: 0 0 0 0;">
            </div>
        </div>

        <div class="chartHolder">
            <%= Html.RenderChart("packageContributionByVolumeChart", Url.Action("GetPackageContributionByVolume", "Statistic"), 600, 300, Url)%>
            <div id="packageContributionByVolumeChart" style="margin: 0 0 0 0;">
            </div>
        </div>

        <div class="chartHolder">
            <%= Html.RenderChart("bundleContributionByRevenueChart", Url.Action("GetBundleContributionByRevenue", "Statistic"), 600, 300, Url)%>
            <div id="bundleContributionByRevenueChart" style="margin: 0 0 0 0;">
            </div>
        </div>

        <div class="chartHolder">
            <%= Html.RenderChart("bundleContributionByVolumeChart", Url.Action("GetBundleContributionByVolume", "Statistic"), 600, 300, Url)%>
            <div id="bundleContributionByVolumeChart" style="margin: 0 0 0 0;">
            </div>
        </div>

        <div class="clear"></div>

        <div class="chartHolder">
            <%= Html.RenderChart("creditsSalesChart", Url.Action("GetCreditsSales", "Statistic"), 1200, 600, Url)%>
            <div id="creditsSalesChart" style="margin: 0 0 0 0;">
            </div>
        </div>

        <div class="chartHolder">
            <%= Html.RenderChart("bundlesSalesChart", Url.Action("GetBundlesSales", "Statistic"), 1200, 600, Url)%>
            <div id="bundlesSalesChart" style="margin: 0 0 0 0;">
            </div>
        </div>

     </div>

</asp:Content>