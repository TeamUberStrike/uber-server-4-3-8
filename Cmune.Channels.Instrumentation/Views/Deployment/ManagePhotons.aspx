<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content2" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript" src="<%= Url.Content("~/Scripts/js/deployment.js")%>?v=<%= ViewBag.PublishVersion %>"></script>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <h1 style="margin-top:10px; margin-bottom:30px">Photons clusters management</h1>

    <div style="margin:auto; margin:0 0 0 0; width:1024px; text-align:left;">
        <span class="bold">Clusters:</span> <%= Html.DropDownList("SelectPhotonsGroupDropDownList", (List<SelectListItem>)ViewData["SelectPhotonsGroupDropDownListData"], new { onchange = "getPhotonsCluster($(this).val());" })%>
        <hr />
        <div id="photonsClusterContainer">
        </div>
    </div>

    <script type="text/javascript">
        getPhotonsCluster($('#SelectPhotonsGroupDropDownList').val());
    </script>

</asp:Content>