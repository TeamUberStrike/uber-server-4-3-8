<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content2" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript" src="<%= Url.Content("~/Scripts/js/item.js")%>?v=<%= ViewBag.PublishVersion %>"></script>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <div style="width:900px;">
        
        <div class="niceDiv">
            <h1>Items synchronization</h1>
            <% if (((bool) ViewData["isSynchronizable"]) == true) { %>
            <div id="itemsSynchronizationButtonsDiv" style="width:60%; margin:auto; font-weight:bold;">
                <p id="syncErrorContainer" style="display:none;color:Red;"></p>
                <input type="submit" id="SyncItemsToStagingButton" name="SyncItemsToStagingButton" value="Push To <%= ViewData["synchronizationType"] %>" onclick="synchronizeGo($(this).val()); return false;" />
            </div>
            <%} %>
            <%else { %>
            <span class="errorMessage">You can synchronize items only from a dev environnement</span>
            <%} %>
        </div>
        
        <div id="applyGlobalDiscountDiv" class="niceDiv" style="margin:30px 0 30px 0;">
            <h1>Apply global discount</h1>
            <% if (ViewBag.Environnement != "dev")
               {  %>
            <form id="applyGlobalDiscountForm" method="post" action="">
                <p style="text-align:left;">
                    Apply a discount of <%= Html.TextBox("DiscountTextBox", null, new { style = "width:30px;" })%>% to all the items <input type="submit" id="applyDiscountButton" name="applyDiscountButton" onclick="applyGlobalDiscount(); return false;" value="Apply" />
                </p>
                <p style="text-align:left; margin-top:15px;">
                    <span class="errorMessage">Note</span>: this will apply the discount to all the items in sales and make them available for one day and permanently only. Users will be able to buy the items for a maximum duration of one day.
                </p>
            </form>
            <% } else { %>
            <span class="errorMessage" >You can apply a global discount in prod and staging only</span>
            <% } %>
        </div>

        <div class="niceDiv">
            <h1>XLXS export</h1>
            <a href="<%= Url.Action("GetShop", "Item")%>">Get Shop</a>
        </div>
<%--
        <div class="niceDiv">
            <h1>Disable Item</h1>
            <form id="disableItemForm" action="" method="post">
                Disable ItemId <%= Html.TextBox("itemIdToDisable", null, new { style = "width:40px" })%> AND replace it in the inventories and loadouts by the ItemId <%= Html.TextBox("replacementItemId", null, new { style = "width:40px" })%> 
                <input id="disableItemButton" type="submit" onclick="disableItem(); return false;" value="Disable" />
            </form>
        </div>--%>

       <% Html.RenderPartial("~/Views/ItemDeprecation/Partial/ItemDeprecation.cshtml"); %>

    </div>
</asp:Content>