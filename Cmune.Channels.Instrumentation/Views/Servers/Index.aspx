<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content2" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript" src="<%= Url.Content("~/Scripts/js/servers.js")%>?v=<%= ViewBag.PublishVersion %>"></script>
    <script type="text/javascript" src="<%= Url.Content("~/Scripts/js/calendarDateInput/calendarDateInit.js")%>?v=<%= ViewBag.PublishVersion %>"></script>
    <script type="text/javascript" src="<%= Url.Content("~/Scripts/js/calendarDateInput/calendarDateInput.js")%>?v=<%= ViewBag.PublishVersion %>"></script>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <div style="margin-top:15px; margin-bottom:15px;">
        <a onclick="LoadAddOrEditManagedServerForm();return false;" href="">Add Server</a>
    </div>
    <div style="margin-left: 10px; margin-right: 10px; text-align: left;">
        <table class="searchResultTable" cellspacing="5" style="margin: 0 0 0 0; margin-left: auto;
            margin-right: auto; text-align: center">
            <tr style="text-align: center; font-weight: bold;">
                <th style="width: 20px;">
                    Id
                </th>
                <th style="width: 120px;">
                    Server Name
                </th>
                <th style="width: 120px;">
                    Role
                </th>
                <th style="width: 120px;">
                    Public IP
                </th>
                <th style="width: 120px;">
                    Private IP
                </th>
                <th style="width: 150px;">
                    Deployment Time
                </th>
                <th style="width: 120px;">
                    Server IDC
                </th>
                <th style="width: 120px;">
                    World Region
                </th>
                <th style="width: 80px;">
                    City
                </th>
                <th style="width: 80px;">
                    CPUModel
                </th>
                <th style="width: 80px;">
                    CPUSpeed
                </th>
                <th style="width: 80px;">
                    CPUCore
                </th>
                <th style="width: 20px;">
                    CPUs
                </th>
                <th style="width: 100px;">
                    RAM
                </th>
                <th style="width: 120px;">
                    Disk Space
                </th>
                <th style="width: 120px;">
                    Bandwidth
                </th>
                <th style="width: 120px;">
                    Price
                </th>
                <th style="width: 250px;">
                    Note
                </th>
                <th style="width: 200px;">
                </th>
            </tr>
            <% 
                if (((List<ManagedServerModel>)ViewData["ManagedServersListView"]).Count > 0)
                {
                    int i = 0;

            %>
            <% foreach (var item in (List<ManagedServerModel>)ViewData["ManagedServersListView"])
               {
            %>
            <tr id="serverTr" class="<%=  item.IsDisable ? "disabledDataRow" : ""%> <%= (i % 2) == 1 ? "mod":""%>">
                <td style="width: 20px;">
                    <%= item.ManagedServerId2.ToString() %>
                </td>
                <td style="text-align: center; width: 120px;">
                    <a href="<%= Url.Action("","ServersMonitoring") %>?selectedManagedServer=<%= item.ManagedServerId2 %>" target="_blank"><%= item.ServerName %></a>
                </td>
                <td style="width: 120px;">
                    <%= item.Role%>
                </td>
                <td style="width: 120px">
                   <a href="http://whatismyipaddress.com/ip/<%= item.PublicIp %>"><%= item.PublicIp %></a>
                </td>
                <td style="width: 120px;">
                    <%= item.PrivateIp %>
                </td>
                <td style="width: 150px;">
                    <%= (item.DeploymentTime.HasValue ? item.DeploymentTime.Value.ConvertToBeijingTime().ToString("MM/dd/yyyy HH:mm:ss") : "")%>
                </td>
                <td style="width: 120px;">
                    <%= item.ServerIDC %>
                </td>
                <td style="width: 120px;">
                    <%= ((RegionType)item.Region).ToString() %>
                    <a href=""  ></a>
                </td>
                <td style="width: 80px;">
                    <%= item.City %>
                </td>
                <td style="width: 80px;">
                    <%= item.CPUModel%>
                </td>
                <td style="width: 80px;">
                    <%= item.CPUSpeed%>
                </td>
                <td style="width: 80px;">
                    <%= item.CPUCore%>
                </td>
                <td style="width: 20px;">
                    <%= item.CPUs%>
                </td>
                <td style="width: 80px;">
                    <%= item.RAM%>
                </td>
                <td style="width: 120px;">
                    <%= item.DiskSpace %>
                </td>
                <td style="width: 80px;">
                    <%= item.AllowedBandwidth %>
                </td>
                <td style="width: 120px;">
                    <%= item.Price.HasValue ? item.Price.Value + "$" : "" %>
                </td>
                <td style="width: 250px;">
                    <%= item.Note %>
                </td>
                <td>
                    <% if (item.IsDisable)
                       {  %>
                    <a id="EnableDisableLinkButton" onclick="DisableOrEnableManagedServer('enable', <%= item.ManagedServerId2 %>); return false;"
                        href="">Enable</a>
                    <% }
                       else
                       {%>
                    <a id="EnableDisableLinkButton" onclick="DisableOrEnableManagedServer('disable', <%= item.ManagedServerId2 %>); return false;"
                        href="">Disable</a>
                    <%} %>
                    <a id="DeleteManagedServer" onclick="DeleteManagedServer(<%= item.ManagedServerId2 %>); return false;"
                        href="">Delete</a> <a id="EditServerLinkButton" onclick="LoadAddOrEditManagedServerForm(<%= item.ManagedServerId2 %>);return false;"
                            href="">Edit</a>
                </td>
            </tr>
            <%
                       i++;
               } %>
        </table>
    </div>
    <%} %>
</asp:Content>
