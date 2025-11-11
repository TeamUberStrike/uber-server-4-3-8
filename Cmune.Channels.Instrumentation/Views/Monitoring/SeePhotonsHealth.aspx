<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <% Html.RenderPartial("ChartJavaScriptControl", ViewData); %>
    <script type="text/javascript" src="<%= Url.Content("~/Scripts/js/monitoring.js")%>?v=<%= ViewBag.PublishVersion %>"></script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <%= Html.DropDownList("Versions", ViewData["Versions"] as SelectList, new { onchange = "getPhotonsHealth($(this).val());", style = "margin-top:50px;" })%>

    <div id="photonsHealthContainer">
    </div>

    <script type="text/javascript">
        getPhotonsHealth($('#Versions').val());
    </script>

</asp:Content>