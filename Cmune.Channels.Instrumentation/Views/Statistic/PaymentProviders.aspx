<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content2" ContentPlaceHolderID="HeadContent" runat="server">
    <% Html.RenderPartial("ChartJavaScriptControl", ViewData); %>
    <script type="text/javascript" src="<%= Url.Content("~/Scripts/js/stats.js")%>?v=<%= ViewBag.PublishVersion %>"></script>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <script type="text/javascript">
        var providerId = <%= ViewBag.PaymentProviderId %>;
        var latestPaymentCount = 120;

        function updateCharts() {
            getTotalRevenueProvider(providerId);
            updateChart('dailyRevenueChart', '<%= Url.Action("GetDailyRevenue", "StatisticsPaymentProviders") %>?' + $('#StatsCalendar').serialize() + '&providerId=<%= ViewBag.PaymentProviderId %>');
            updateChart('dailyTransactionsChart', '<%= Url.Action("GetDailyTransactions", "StatisticsPaymentProviders") %>?' + $('#StatsCalendar').serialize() + '&providerId=<%= ViewBag.PaymentProviderId %>');

            <% if (ViewBag.ShowPackageContribution) { %>
            
            updateChart('packageContributionByRevenueChart', '<%= Url.Action("GetPackagesContributionByRevenue", "StatisticsPaymentProviders") %>?' + $('#StatsCalendar').serialize() + '&providerId=<%= ViewBag.PaymentProviderId %>');
            updateChart('packageContributionByVolumeChart', '<%= Url.Action("GetPackagesContributionByVolume", "StatisticsPaymentProviders") %>?' + $('#StatsCalendar').serialize() + '&providerId=<%= ViewBag.PaymentProviderId %>');
            updateChart('bundleContributionByRevenueChart', '<%= Url.Action("GetBundleContributionByRevenue", "StatisticsPaymentProviders") %>?' + $('#StatsCalendar').serialize() + '&providerId=<%= ViewBag.PaymentProviderId %>');
            updateChart('bundleContributionByVolumeChart', '<%= Url.Action("GetBundleContributionByVolume", "StatisticsPaymentProviders") %>?' + $('#StatsCalendar').serialize() + '&providerId=<%= ViewBag.PaymentProviderId %>');
            updateChart('creditsSalesChart', '<%= Url.Action("GetCreditsSales", "StatisticsPaymentProviders") %>?' + $('#StatsCalendar').serialize() + '&providerId=<%= ViewBag.PaymentProviderId %>');
            updateChart('bundlesSalesChart', '<%= Url.Action("GetBundlesSales", "StatisticsPaymentProviders") %>?' + $('#StatsCalendar').serialize() + '&providerId=<%= ViewBag.PaymentProviderId %>');

            <% } %>
        }
    </script>

    <div class="Menu">
        <% foreach (PaymentProviderType paymentProvider in AdminConfig.ActiveStatsPaymentProviders) { %>
        <a href="<%= Url.Action("PaymentProviders", "Statistic") %>?providerId=<%= (int) paymentProvider %>" class="<%= (int) ViewBag.PaymentProviderId == (int) paymentProvider ? "submenuActiveTab":"" %>"><%: CommonConfig.PaymentProviderName[paymentProvider]%></a>
        <% } %>
    </div>

    <% Html.RenderPartial("StatsCalendar", ViewData); %>

    <div id="statsDiv" style="width:1200px; text-align:left;">

        <h1><%= ViewBag.PaymentProviderName %></h1>

        <p style="margin:0 0 25px 25px;">Total payment over the period: US$<span id="totalPaymentSpan"></span></p>

        <script type="text/javascript">
            getTotalRevenueProvider(<%= ((int)ViewBag.PaymentProviderId) %>);
        </script>

        <div class="chartHolder">
            <%= Html.RenderChart("dailyRevenueChart", Url.Action("GetDailyRevenue", "StatisticsPaymentProviders") + "?providerId=" + ((int)ViewBag.PaymentProviderId).ToString(), 600, 300, Url)%>
            <div id="dailyRevenueChart">
            </div>
        </div>

        <div class="chartHolder">
            <%= Html.RenderChart("dailyTransactionsChart", Url.Action("GetDailyTransactions", "StatisticsPaymentProviders") + "?providerId=" + ((int)ViewBag.PaymentProviderId).ToString(), 600, 300, Url)%>
            <div id="dailyTransactionsChart">
            </div>
        </div>

        <% if (ViewBag.ShowPackageContribution) { %>

        <div class="chartHolder">
            <%= Html.RenderChart("packageContributionByRevenueChart", Url.Action("GetPackagesContributionByRevenue", "StatisticsPaymentProviders") + "?providerId=" + ((int)ViewBag.PaymentProviderId).ToString(), 600, 300, Url)%>
            <div id="packageContributionByRevenueChart">
            </div>
        </div>

        <div class="chartHolder">
            <%= Html.RenderChart("packageContributionByVolumeChart", Url.Action("GetPackagesContributionByVolume", "StatisticsPaymentProviders") + "?providerId=" + ((int)ViewBag.PaymentProviderId).ToString(), 600, 300, Url)%>
            <div id="packageContributionByVolumeChart">
            </div>
        </div>

        <div class="chartHolder">
            <%= Html.RenderChart("bundleContributionByRevenueChart", Url.Action("GetBundleContributionByRevenue", "StatisticsPaymentProviders") + "?providerId=" + ((int)ViewBag.PaymentProviderId).ToString(), 600, 300, Url)%>
            <div id="bundleContributionByRevenueChart">
            </div>
        </div>

        <div class="chartHolder">
            <%= Html.RenderChart("bundleContributionByVolumeChart", Url.Action("GetBundleContributionByVolume", "StatisticsPaymentProviders") + "?providerId=" + ((int)ViewBag.PaymentProviderId).ToString(), 600, 300, Url)%>
            <div id="bundleContributionByVolumeChart">
            </div>
        </div>

        <div class="clear"></div>

        <div class="chartHolder">
            <%= Html.RenderChart("creditsSalesChart", Url.Action("GetCreditsSales", "StatisticsPaymentProviders") + "?providerId=" + ((int)ViewBag.PaymentProviderId).ToString(), 1200, 600, Url)%>
            <div id="creditsSalesChart" style="margin: 0 0 0 0;">
            </div>
        </div>

        <div class="chartHolder">
            <%= Html.RenderChart("bundlesSalesChart", Url.Action("GetBundlesSales", "StatisticsPaymentProviders") + "?providerId=" + ((int)ViewBag.PaymentProviderId).ToString(), 1200, 600, Url)%>
            <div id="bundlesSalesChart" style="margin: 0 0 0 0;">
            </div>
        </div>

        <% } %>

        <div class="left" style="width:450px;">
            <input type="submit" id="getLatestPaymentsButton" onclick='getLatestPayment(providerId, latestPaymentCount); return false;' value="Reload latest payments" /> <img id="latestPaymentsLoadingImg" style="display:none;" src="<%= Url.Content("~/Content/img/ajax-loader-pac-man.gif")%>" alt="Loader" />

            <div id="latestPaymentDiv">
            </div>

            <script type="text/javascript">
                getLatestPayment(providerId, latestPaymentCount);
            </script>
        </div>

        <div class="left" style="margin:0 0 0 100px;">
            Transaction key: <%= Html.TextBox("transactionId", "", new { style="width:300px" })%> <input type="submit" id="getCreditDepositButton" onclick='getCreditDeposit(providerId, $("#transactionId").val()); return false;' value="Search" /> <img id="creditDepositLoadingImg" style="display:none;" src="<%= Url.Content("~/Content/img/ajax-loader-pac-man.gif")%>" alt="Loader" />

            <div id="creditDepositDiv" style="height:1800px;">
            </div>

        </div>

    </div>

</asp:Content>