<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript" src="<%= Url.Content("~/Scripts/js/item.js")%>?v=<%= ViewBag.PublishVersion %>"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <br />
    <p>
        This page allow you to see the differences between the prod and dev items</p>
    <br />
    <div>
        <label for="ItemTypeDp">
            Item Type</label>
        <%= Html.DropDownList("ItemTypeDp", (List<SelectListItem>)ViewBag.ItemTypeList, new { onchange = "LoadTypeComparison($('#ItemTypeDp').val());" })%>
    </div>
    <br />
    <script type="text/javascript">
        LoadTypeComparison($('#ItemTypeDp').val());
    </script>
    <br />
    <div id="ComparisonTable">
    </div>
</asp:Content>
