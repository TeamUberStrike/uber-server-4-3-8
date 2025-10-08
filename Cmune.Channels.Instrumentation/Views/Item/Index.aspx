<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content2" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript" src="<%= Url.Content("~/Scripts/js/item.js")%>?v=<%= ViewBag.PublishVersion %>"></script>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
<script type="text/javascript">
    var shopItemTypeFunctional = <%= ViewData["ShopItemTypeFunctional"] %>;
    var shopItemTypeGear = <%= ViewData["ShopItemTypeGear"] %>;
    var shopItemTypeQuickUse = <%= ViewData["ShopItemTypeQuickUse"] %>;
    var shopItemTypeSpecial = <%= ViewData["ShopItemTypeSpecial"] %>;
    var shopItemTypeWeapon = <%= ViewData["ShopItemTypeWeapon"] %>;
    var shopItemTypeWeaponMod = <%= ViewData["ShopItemTypeWeaponMod"] %>;
</script>
<div>
    <form id="searchItemForm" action="" method="post">
        <div style="width: 80%;">
            <div class="left">
                <%= Html.DropDownList("ApplicationDropDownList", (List<SelectListItem>)ViewData["applicationList"], new { onchange = "getItems();" })%>
                <% if(ViewBag.Environnement == "dev"){  %>
                <a href="" onclick="LoadAddOrEditForm('add', 0); return false;" id="addItemLink">Add a new item</a>
                <%} %>
            </div>
            <div class="right" style="text-align: right;">
                <%= Html.TextBox("itemIdTextBox", null, new { style = "width:40px;" })%>
                <input type="submit" id="searchByIdButton" name="searchByIdButton" value="Search by Id"
                    onclick="getItems(); return false;" />
                <span class="bold" style="color:red;">OR</span> by name:
                <%= Html.TextBox("itemNameTextBox")%>
                <input type="submit" id="searchByNameButton" name="searchByNameButton" value="Search by name"
                    onclick="getItems(); return false;" />
                <span class="bold" style="color:red;">OR</span> by type and class:
                <%= Html.DropDownList("TypeDropDownList", (List<SelectListItem>)ViewData["itemTypeList"], new { onchange = "refreshItemClassDropDownList('#TypeDropDownList','#ClassDropDownList');getItems();" })%>
                <%= Html.DropDownList("ClassDropDownList", (List<SelectListItem>)ViewData["itemClassList"], new { onchange = "getItems();" })%>
                <span class="bold" style="color:red;">OR</span> by:
                <%= Html.DropDownList("itemStatus", (List<SelectListItem>)ViewData["itemStatus"], new { onchange = "getItems();" })%>
            </div>
            <br class="clear" />
        </div>
        <div id="itemsResultContainer">
        </div>
        <div id="addOrEditFormContainer" style="display: none; margin: 0 0 50px 0;">
        </div>
    </form>
</div>

<script type="text/javascript">
    getItems();
</script>

</asp:Content>