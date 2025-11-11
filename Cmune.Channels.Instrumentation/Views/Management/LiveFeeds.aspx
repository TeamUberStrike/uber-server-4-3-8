<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content2" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript" src="<%= Url.Content("~/Scripts/js/management.js")%>?v=<%= ViewBag.PublishVersion %>"></script>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <div style="width:852px;">
        <h1>Live feeds</h1>
        <p style="text-align:left; margin-bottom:20px;">
            <a href="#" onclick="loadAddLiveFeedForm(<%= UberStrikeCommonConfig.LiveFeedNormalPriority %>); return false;">Add</a>
        </p>
        <div id="liveFeedsContainer">
        </div>
        <script type="text/javascript">
            getLiveFeeds();
        </script>
    </div>
</asp:Content>