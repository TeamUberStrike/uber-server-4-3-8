<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content2" ContentPlaceHolderID="HeadContent" runat="server">
    <% Html.RenderPartial("ChartJavaScriptControl", ViewData); %>
    <script type="text/javascript" src="<%= Url.Content("~/Scripts/js/stats.js")%>?v=<%= ViewBag.PublishVersion %>"></script>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <script type="text/javascript">
        function updateCharts() {
            updateChart('tutorialStepsChart', '<%= Url.Action("GetTutorialStepsChart", "Statistic") %>?' + $('#StatsCalendar').serialize());
            updateChart('tutorialStepsLineChart', '<%= Url.Action("GetTutorialStepsLineChart", "Statistic") %>?' + $('#StatsCalendar').serialize());
        }
    </script>

    <% Html.RenderPartial("StatsCalendar", ViewData); %>

    <div id="statsDiv" style="width:1200px;">

        <div class="chartHolder">
            <%= Html.RenderChart("tutorialStepsChart", Url.Action("GetTutorialStepsChart", "Statistic"), 600, 300, Url)%>
            <div id="tutorialStepsChart" style="margin: 0 0 0 0;">
            </div>
        </div>

        <div class="chartHolder">
            <%= Html.RenderChart("tutorialStepsLineChart", Url.Action("GetTutorialStepsLineChart", "Statistic"), 600, 300, Url)%>
            <div id="tutorialStepsLineChart" style="margin: 0 0 0 0;">
            </div>
        </div>

        <div class="clear"></div>

        <h1>Tracking testing</h1>
        <div style="text-align:left;">
            <p>Ignore this section if you're just looking at the stats :)</p>
            <div style="margin:30px 0 35px 25px;">
                Cmid: <%= Html.TextBox("cmid", null, new { style = "width:60px;" }) %> <input type="submit" id="getTutorialStepsButton" onclick="return getTutorialSteps();" value="Search" />
            </div>
            <div id="tutorialStepsContainer">
            </div>
        </div>
    </div>
</asp:Content>