<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content2" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript" src="<%= Url.Content("~/Scripts/js/deployment.js") %>?v=<%= ViewBag.PublishVersion %>"></script>
    <script type="text/javascript">
    var webChannels = <%= Html.ChannelTypesToJson((List<ChannelType>)ViewBag.WebChannels) %>;
    var standaloneChannels = <%= Html.ChannelTypesToJson((List<ChannelType>)ViewBag.StandaloneChannels) %>;
    </script>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <br />
    <div>
        <a onclick="loadApplicationVersionForm('<%= Url.Action("LoadApplicationVersionAddForm") %>', 'Add Application Version'); return false;" href="">Add application version</a>
    </div>
    <br />
    
    <div id="applicationVersionsContainer">
        <% Html.RenderPartial("Partial/ApplicationVersions", (List<ApplicationVersionViewModel>) ViewBag.ApplicationVersions); %>
    </div>
    
</asp:Content>