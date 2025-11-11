<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content2" ContentPlaceHolderID="HeadContent" runat="server">
    <% Html.RenderPartial("ChartJavaScriptControl", ViewData); %>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript">
        function updateCharts() {
            updateLevelDistributionChart();

            <% foreach (ChannelType channel in AdminConfig.ActiveStatsChannels)
           { %>
            updateChart('<%: String.Format("{0}FriendsCountChart", channel) %>', '<%= Url.Action("GetFriendsCount", "Statistic") %>?' + $('#StatsCalendar').serialize() + '&channelId=<%: (int) channel %>');
            updateChart('<%: String.Format("{0}ActiveFriendsCountChart", channel) %>', '<%= Url.Action("GetActiveFriendsCount", "Statistic") %>?' + $('#StatsCalendar').serialize() + '&channelId=<%: (int) channel %>');
           <% } %>
        }

        function updateLevelDistributionChart() {
            var additionalArgs = '';

            if ($('#activeOnly').attr('checked')) {
                additionalArgs = '&activeOnly=true';
            }

            updateChart('playerLevelDistributionChart', '<%= Url.Action("GetPlayerLevelDistribution", "Statistic") %>?' + $('#StatsCalendar').serialize() + additionalArgs);
        }
    </script>
    <% Html.RenderPartial("StatsCalendar", ViewData); %>
    <div id="statsDiv" style="width: 1200px;">
        <div class="chartHolder">
            <%= Html.RenderChart("playerLevelDistributionChart", Url.Action("GetPlayerLevelDistribution", "Statistic") + "?activeOnly=true", 600, 300, Url)%>
            <div id="playerLevelDistributionChart">
            </div>
            <div class="clear">
            </div>
            <input id="activeOnly" type="checkbox" onclick="updateLevelDistributionChart();"
                checked="checked" /><label for="activeOnly">Get only active players</label>
            <p style="text-align: left; margin: 15px 0 0 0;">
                We started to record historical data on the 10 of August 2011.</p>
        </div>
        <div class="clear">
        </div>
        <h1>
            Friends</h1>
        <div class="clear">
        </div>
        <% foreach (ChannelType channel in AdminConfig.ActiveStatsChannels)
           { %>
        <div class="chartHolder">
            <%= Html.RenderChart(String.Format("{0}FriendsCountChart", channel), Url.Action("GetFriendsCount", "Statistic") + "?channelId=" + (int) channel, 600, 300, Url)%>
            <div id="<%: String.Format("{0}FriendsCountChart", channel) %>">
            </div>
        </div>
        <%} %>
        <div class="clear">
        </div>
        <h1>
            Active Friends</h1>
        <div class="clear">
        </div>
        <% foreach (ChannelType channel in AdminConfig.ActiveStatsChannels)
           { %>
        <div class="chartHolder">
            <%= Html.RenderChart(String.Format("{0}ActiveFriendsCountChart", channel), Url.Action("GetActiveFriendsCount", "Statistic") + "?channelId=" + (int)channel, 600, 300, Url)%>
            <div id="<%: String.Format("{0}ActiveFriendsCountChart", channel) %>">
            </div>
        </div>
        <%} %>
    </div>
    </div>
</asp:Content>
