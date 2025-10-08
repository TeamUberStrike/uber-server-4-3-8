<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript" src="<%= Url.Content("~/Scripts/js/bundle.js")%>?v=<%= ViewBag.PublishVersion %>"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div style="width: 1200px">
        <p style="margin-top:15px; margin-bottom:15px;" class="italic">
            The price is used as identifier for transaction. Decimal price doesn't work on Facebook for now.<br />
            Images should be uploaded to: client.cloud.cmune.com/UberStrike/CommonChannel/images/bundle/
        </p>
        <a href="" onclick="LoadAddEditBundleForm(0); return false;">Add bundle / pack</a>
        <form id="getBundlesForm" action="" method="post" style="margin-top:15px;">
            <%= Html.CheckBox("IsOnSale", true, new { onchange = "getBundles();" })%> <label for="IsOnSale">Only on sale</label>
            <%= Html.CheckBox("IsBundle", false, new { onchange = "getBundlesOnly();" })%> <label for="IsBundle" style="color:#83D5EC; font-weight:bold;">Only items/points bundles</label>
            <%= Html.CheckBox("IsPack", false, new { onchange = "getPacksOnly();" })%> <label for="IsPack" style="color:#F5B289; font-weight:bold;">Only credits packs</label>
        </form>
        <div id="bundleContainer">
            <% Html.RenderPartial("Partial/Bundles", (List<BundleView>) ViewBag.Bundles); %>
        </div>
    </div>
</asp:Content>