<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
    <% Html.RenderPartial("ChartJavaScriptControl", ViewData); %>
    <script type="text/javascript" src="<%= Url.Content("~/Scripts/js/stats.js")%>"></script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <script type="text/javascript">
        function updateCharts() {
            updateItemCreditsContribution();
            updateChart('averagePointsBalanceChart', '<%= Url.Action("GetAveragePointsBalance", "StatisticsItems")%>?' + $('#StatsCalendar').serialize());
            updateChart('pointsMovementChart', '<%= Url.Action("GetPointsMovement", "StatisticsItems")%>?' + $('#StatsCalendar').serialize());
            updateChart('averageCreditsBalanceChart', '<%= Url.Action("GetAverageCreditsBalance", "StatisticsItems")%>?' + $('#StatsCalendar').serialize());
            updateChart('creditsMovementChart', '<%= Url.Action("GetCreditsMovement", "StatisticsItems")%>?' + $('#StatsCalendar').serialize());
            updateChart('pointsDepositTypeDistributionChart', '<%= Url.Action("GetPointDepositTypesDistribution", "StatisticsItems")%>?' + $('#StatsCalendar').serialize());
        }

        function updateItemCreditsContribution() {
            updateStatsMetrics(['itemOneCreditsContribution', 'itemTwoCreditsContribution', 'itemThreeCreditsContribution'],
                                ['itemCreditsContributionForm'],
                                '/StatisticsItems/GetItemCreditSaleDropDownList',
                                function () { updateItemCreditsContributionChart() });
        }

        function updateItemCreditsContributionChart() {
            updateChart('itemCreditsContributionChart', '<%= Url.Action("GetCreditItemSale", "StatisticsItems")%>?' + $('#StatsCalendar').serialize() + '&' + $('#itemCreditsContributionForm').serialize());
        }

        function updateItemPriceAvailability(itemIndex, itemId) {
            $("#item" + itemIndex + "InnerDiv").hide();
            $("#item" + itemIndex + "LoadingImg").show();

            $.ajax({
                type: 'POST',
                dataType: 'json',
                url: applicationPath + 'StatisticsItems/GetItemPricingAvailability',
                data: 'itemId=' + itemId,
                success: function (data) {

                    if (data == "null") {
                        $("#item" + itemIndex + "InnerDiv").html('');
                        $("#item" + itemIndex + "TitleSpan").html("Item " + itemIndex);
                    }
                    else {
                        var shopDailyCredits = "-";
                        var shopPermanentCredits = "-";
                        var undergroundDailyCredits = "-";
                        var undergroundPermanentCredits = "-";
                        var isAvailableForOneDay = "No";
                        var isAvailableForSevenDays = "No";
                        var isAvailableForThirtyDays = "No";
                        var isAvailableForNinetyDays = "No";

                        if (data.IsAvailableInShop) {

                            if (data.ShopDailyCredits != -1) {
                                shopDailyCredits = data.ShopDailyCredits;
                            }

                            if (data.ShopPermanentCredits != -1) {
                                shopPermanentCredits = data.ShopPermanentCredits;
                            }

                        }

                        if (data.IsAvailableInUnderground) {

                            if (data.UndergroundDailyCredits != -1) {
                                undergroundDailyCredits = data.UndergroundDailyCredits;
                            }

                            if (data.UndergroundPermamentCredits != -1) {
                                undergroundPermanentCredits = data.UndergroundPermamentCredits;
                            }

                        }

                        if (data.IsAvailableForOneDay) {
                            isAvailableForOneDay = "Yes";
                        }

                        if (data.IsAvailableForSevenDays) {
                            isAvailableForSevenDays = "Yes";
                        }

                        if (data.IsAvailableForThirtyDays) {
                            isAvailableForThirtyDays = "Yes";
                        }

                        if (data.IsAvailableForNinetyDays) {
                            isAvailableForNinetyDays = "Yes";
                        }

                        var htmlContent = '<div style="width:80%; margin:auto;"><span style="display:block; width:100%; background-color:#B3B3B3; text-align:center; padding:7px 0 7px; margin:7px 0 7px 0;">Shop</span>Daily: ' + shopDailyCredits + '<br />Permanent: ' + shopPermanentCredits + '</div><div style="width:80%; margin:auto;"><span style="display:block; width:100%; background-color:#B3B3B3; text-align:center; padding:7px 0 7px; margin:7px 0 7px 0;">Underground</span>Daily: ' + undergroundDailyCredits + '<br />Permanent: ' + undergroundPermanentCredits + '</div><div style="width:80%; margin:auto;"><span style="display:block; width:100%; background-color:#B3B3B3; text-align:center; padding:7px 0 7px; margin:7px 0 7px 0;">Duration</span>1 day: ' + isAvailableForOneDay + '<br />7 days: ' + isAvailableForSevenDays + '<br />30 days: ' + isAvailableForThirtyDays + '<br />90 days: ' + isAvailableForNinetyDays + '</div>';

                        $("#item" + itemIndex + "InnerDiv").html(htmlContent);
                        $("#item" + itemIndex + "InnerDiv").show();
                        $("#item" + itemIndex + "TitleSpan").html("Item " + itemIndex + " - " + itemId);
                    }

                    $("#item" + itemIndex + "LoadingImg").hide();
                },
                error: function (data) {

                    $("#item" + itemIndex + "LoadingImg").hide();
                }
            });
        }
    </script>

    <% Html.RenderPartial("StatsCalendar", ViewData); %>

    <div id="statsDiv" style="width:1300px; text-align:left;">

        <div class="left" style="width:600px; height:410px;">

            <form id="itemCreditsContributionForm" method="post" action="">
                <span class="it">If an item is not in the list it means it didn't generate any revenue over the selected duration.</span><br />
                <span style="color:#F06F30; font-weight:bold;">Item 1:</span> <%= Html.DropDownList("itemOneCreditsContribution", ViewData["itemOneCreditsContribution"] as SelectList, new { onchange = "updateItemCreditsContributionChart(); updateItemPriceAvailability(1, $('#itemOneCreditsContribution').val());" })%><br />
                <span style="color:#63BE6C; font-weight:bold;">Item 2:</span> <%= Html.DropDownList("itemTwoCreditsContribution", ViewData["itemTwoCreditsContribution"] as SelectList, new { onchange = "updateItemCreditsContributionChart(); updateItemPriceAvailability(2, $('#itemTwoCreditsContribution').val());" })%><br />
                <span style="color:#78B3E5; font-weight:bold;">item 3:</span> <%= Html.DropDownList("itemThreeCreditsContribution", ViewData["itemThreeCreditsContribution"] as SelectList, new { onchange = "updateItemCreditsContributionChart(); updateItemPriceAvailability(3, $('#itemThreeCreditsContribution').val());" })%>
            </form>

            <div class="chartHolder">
                <%= Html.RenderChart("itemCreditsContributionChart", Url.Action("GetCreditItemSale", "StatisticsItems") + "?itemOneCreditsContribution=" + ((string)ViewBag.SelectedItemOneCreditsContribution) + "&itemTwoCreditsContribution=0&itemThreeCreditsContribution=0", 600, 300, Url)%>
                <div id="itemCreditsContributionChart">
                </div>
            </div>

        </div>

        <div class="left" style="width:690px; height:410px;">

            <script type="text/javascript">
                updateItemPriceAvailability(1, <%= ViewBag.SelectedItemOneCreditsContribution %>);
            </script>

            <div class="left" style="width:200px; border:1px solid #F06F30; margin:0 5px 0 5px; padding:5px;">
                <span id="item1TitleSpan" style="display:block; width:100%; text-align:center; color:#F06F30; font-weight:bold;">Item 1</span>
                <div id="item1InnerDiv">
                </div>
                <img id="item1LoadingImg" style="display:none;" src="<%= Url.Content("~/Content/img/ajax-loader-pac-man.gif")%>" alt="Loader" />
            </div>

            <div class="left" style="width:200px; border:1px solid #63BE6C; margin:0 5px 0 5px; padding:5px;">
                <span id="item2TitleSpan" style="display:block; width:100%; text-align:center; color:#63BE6C; font-weight:bold;">Item 2</span>
                <div id="item2InnerDiv">
                </div>
                <img id="item2LoadingImg" style="display:none;" src="<%= Url.Content("~/Content/img/ajax-loader-pac-man.gif")%>" alt="Loader" />
            </div>

            <div class="left" style="width:200px; border:1px solid #78B3E5; margin:0 5px 0 5px; padding:5px;">
                <span id="item3TitleSpan" style="display:block; width:100%; text-align:center; color:#78B3E5; font-weight:bold;">Item 3</span>
                <div id="item3InnerDiv">
                </div>
                <img id="item3LoadingImg" style="display:none;" src="<%= Url.Content("~/Content/img/ajax-loader-pac-man.gif")%>" alt="Loader" />
            </div>

        </div>

        <div class="clear"></div>

        <h1>Points Economy</h1>

        <div class="clear"></div>

        <div class="chartHolder">
            <%= Html.RenderChart("averagePointsBalanceChart", Url.Action("GetAveragePointsBalance", "StatisticsItems"), 600, 300, Url)%>
            <div id="averagePointsBalanceChart">
            </div>
            <span style="width:600px; display:block;">The average points balance is computed over the members that connected during the stats period and are level 5 or more.</span>
        </div>

        <div class="chartHolder">
            <%= Html.RenderChart("pointsMovementChart", Url.Action("GetPointsMovement", "StatisticsItems"), 600, 300, Url)%>
            <div id="pointsMovementChart">
            </div>
        </div>

        <div class="clear"></div>

        <div class="chartHolder">
            <%= Html.RenderChart("pointsDepositTypeDistributionChart", Url.Action("GetPointDepositTypesDistribution", "StatisticsItems"), 600, 300, Url)%>
            <div id="pointsDepositTypeDistributionChart">
            </div>
        </div>

        <div class="clear"></div>

        <h1>Credits Economy</h1>

        <div class="clear"></div>

        <div class="chartHolder">
            <%= Html.RenderChart("averageCreditsBalanceChart", Url.Action("GetAverageCreditsBalance", "StatisticsItems"), 600, 300, Url)%>
            <div id="averageCreditsBalanceChart">
            </div>
            <span style="width:600px; display:block;">The average credits balance is computed over the members that connected during the stats period and have a credits balance superior to 0 currently.</span>
        </div>

        <div class="chartHolder">
            <%= Html.RenderChart("creditsMovementChart", Url.Action("GetCreditsMovement", "StatisticsItems"), 600, 300, Url)%>
            <div id="creditsMovementChart">
            </div>
        </div>

    </div>

</asp:Content>