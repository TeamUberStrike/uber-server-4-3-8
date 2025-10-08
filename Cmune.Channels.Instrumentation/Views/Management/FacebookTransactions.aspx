<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript" src="<%= Url.Content("~/Scripts/js/management.js")%>?v=<%= ViewBag.PublishVersion %>"></script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div style="width:1200px;">

        <div class="niceDiv">
            <h1>Facebook reversals</h1>
            <% if ((string)ViewData["EnvironnementLiteral"] == "prod") { %>
            <p style="margin-bottom:15px;">Facebook Id equals to 0 means that the account was deleted by Facebook, no action needs to be taken</p>
            <input type="submit" id="getFacebookReversalsButton" style="margin-bottom:15px;" value="Get Facebook reversals" onclick="getFacebookReversals(); return false;" />
            <div id="facebookReversalsContainer">
            </div>
            <% } else { %>
            <span class="errorMessage">Please view the Facebook reversals in prod only.</span>
            <% } %>
        </div>

        <div class="niceDiv">
            <h1>Refund a disputed transaction</h1>
            <% if ((string)ViewData["EnvironnementLiteral"] == "prod") { %>
            <form id="refundTransactionForm" action="" method="post">
                <%= Html.DropDownList("DisputedTransactionActionType", (List<SelectListItem>)ViewData["DisputedTransactionActionType"])%> Order id: <%= Html.TextBox("OrderId", null, new { style = "width:100px;" })%> with comment: <%= Html.TextBox("Comment", null, new { style = "width:500px;" })%> <input type="submit" id="refundOrderButton" value="Proceed" onclick="refundTransaction(); return false;" />
            </form>
            <% } else { %>
            <span class="errorMessage">Please refund the disputed transactions in prod only.</span>
            <% } %>
        </div>

        <div class="niceDiv">
            <h1>Get a transaction details</h1>
            <% if ((string)ViewData["EnvironnementLiteral"] == "prod") { %>
            <form id="getTransactionForm" action="" method="post">
                Order id: <%= Html.TextBox("OrderId", null, new { style = "width:100px;" })%> <input type="submit" id="getOrderButton" value="Get transaction details" onclick="getTransaction(); return false;" />
            </form>
            <div id="transactionContainer" style="margin:35px 0 0 150px;">
            </div>
            <% } else { %>
            <span class="errorMessage">Please view a transaction details in prod only.</span>
            <% } %>
        </div>

    </div>

</asp:Content>