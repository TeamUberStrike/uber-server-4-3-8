<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript" src="<%= Url.Content("~/Scripts/js/monitoring.js")%>?v=<%= ViewBag.PublishVersion %>"></script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div style="width:1200px;">
        <h1>Unity exceptions</h1>

        <p style="margin:0 0 15px 0; text-align:left;">Those exceptions were raised during the last seven days. <a href="#" onclick="deleteAllExceptions(); return false;">Delete all exceptions</a></p>

        <div id="unityExceptionsContainer">
        </div>
    </div>

    <script type="text/javascript">
        getExceptionGroups();
    </script>

</asp:Content>