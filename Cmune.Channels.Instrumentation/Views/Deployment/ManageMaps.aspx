<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content2" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript" src="<%= Url.Content("~/Scripts/js/deployment.js")%>?v=<%= ViewBag.PublishVersion %>"></script>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
     <h1 style="margin-top:10px; margin-bottom:30px">Maps management</h1>

    <div style="margin:auto; margin:0 0 0 0; width:900px; text-align:left;">
        <span class="bold">Clusters:</span> <%= Html.DropDownList("applicationVersions", (List<SelectListItem>)ViewData["applicationVersions"], new { onchange = "getMapsCluster($(this).val());" })%>
        <hr />
        <div id="mapsContainer">
        </div>
    </div>

    <script type="text/javascript">
        getMapsCluster($('#applicationVersions').val());
    </script>
</asp:Content>