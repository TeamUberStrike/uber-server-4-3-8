<%@ Page Title="" MasterPageFile="~/Views/Shared/Site.Master" Language="C#" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content2" ContentPlaceHolderID="HeadContent" runat="server">
    <% Html.RenderPartial("ChartJavaScriptControl", ViewData); %>
    <script type="text/javascript" src="<%= Url.ServersMonitoringJsFile() %>"></script>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript">
        function updateCharts() {
            updateChart('dailyCPUUsage', '<%= Url.Action("GetCPUUsage", "ServersMonitoring") %>?managedServerId=' + $("#ManagedServersDropDownList").val() + '&' + $('#StatsCalendar').serialize());
            updateChart('dailyRamUsage', '<%= Url.Action("GetRamUsage", "ServersMonitoring") %>?managedServerId=' + $("#ManagedServersDropDownList").val() + '&' + $('#StatsCalendar').serialize());
            updateChart('dailyBandwidthUsage', '<%= Url.Action("GetBandwidthUsage", "ServersMonitoring") %>?managedServerId=' + $("#ManagedServersDropDownList").val() + '&' + $('#StatsCalendar').serialize());
            updateChart('dailyDiskSpaceUsage', '<%= Url.Action("GetDiskSpaceUsage", "ServersMonitoring") %>?managedServerId=' + $("#ManagedServersDropDownList").val() + '&' + $('#StatsCalendar').serialize());
            RefreshManagedServerServices($("#ManagedServersDropDownList").val());
        }
    </script>
    <br />
    <h1>
        Server Monitoring</h1>
    <br />
    <div style="width: 868px;">
        <div class="left" style="margin-top: 18px">
            <%= Html.DropDownList("ManagedServersDropDownList", (List<SelectListItem>)Html.ManagedServersList((List<ManagedServerModel>)ViewData["ManagedServers"], (int)ViewBag.ManagedServerId), new { onchange = "updateCharts()" })%>
            <div style="width:350px;" id="ManagedServerServices">
                <%= Html.Action("GetManagedServerServices", "ServersMonitoring", new { ManagedServerId = (int)ViewBag.ManagedServerId}) %>
            </div>
        </div>
        <div class="left">
            <% Html.RenderPartial("StatsCalendar", ViewData); %>
        </div>
        <div class="clear">
        </div>
    </div>
    <br /><br />
    <div id="statsDiv" style="width: 1200px;">
        <div class="chartHolder">
            <%= Html.RenderChart("dailyCPUUsage", Url.Action("GetCPUUsage", "ServersMonitoring") + "?managedServerId=" + (int)ViewBag.ManagedServerId, 1024, 300, Url)%>
            <div id="dailyCPUUsage" style="margin: 0 0 0 0;">
            </div>
        </div>
        <div class="chartHolder">
            <%= Html.RenderChart("dailyRamUsage", Url.Action("GetRamUsage", "ServersMonitoring") + "?managedServerId=" + (int)ViewBag.ManagedServerId, 1024, 300, Url)%>
            <div id="dailyRamUsage" style="margin: 0 0 0 0;">
            </div>
        </div>
        <div class="chartHolder">
            <%= Html.RenderChart("dailyBandwidthUsage", Url.Action("GetBandwidthUsage", "ServersMonitoring") + "?managedServerId=" + (int)ViewBag.ManagedServerId, 1024, 300, Url)%>
            <div id="dailyBandwidthUsage" style="margin: 0 0 0 0;">
            </div>
        </div>
        <div class="chartHolder">
            <%= Html.RenderChart("dailyDiskSpaceUsage", Url.Action("GetDiskSpaceUsage", "ServersMonitoring") + "?managedServerId=" + (int)ViewBag.ManagedServerId, 1024, 300, Url)%>
            <div id="dailyDiskSpaceUsage" style="margin: 0 0 0 0;">
            </div>
        </div>
    </div>
</asp:Content>
