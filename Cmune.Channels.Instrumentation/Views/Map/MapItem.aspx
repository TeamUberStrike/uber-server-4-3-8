<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content2" ContentPlaceHolderID="HeadContent" runat="server">
<script src="<%= Url.MapJsFile() %>" type="text/javascript"></script>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <br />
    <div style="margin:auto; margin:0 0 0 0; width:900px; text-align:left;">
        <span class="bold">Clusters:</span> <%= Html.DropDownList("applicationVersions", (List<SelectListItem>)ViewBag.ApplicationVersions, new { onchange = "GetMapItems('" + Url.Action("GetMapItems", "Map") + "', $(this).val());" })%>
        <hr />
        <div id="mapItemsContainer">
        </div>
    </div>
    <script type="text/javascript">
        GetMapItems('<%= Url.Action("GetMapItems", "Map") %>', $('#applicationVersions').val());
    </script>
</asp:Content>
