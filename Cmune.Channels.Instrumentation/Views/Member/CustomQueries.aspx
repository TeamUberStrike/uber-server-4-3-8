<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content2" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript" src="<%= Url.Content("~/Scripts/js/member.js")%>?v=<%=ViewBag.PublishVersion %>"></script>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div style="width:1200px; margin:auto; text-align:left; padding-top:15px;">
        

        <div class="niceDiv">
            <h1>Who bought an item?</h1>
            <p><span class="it">Does not take into account gifted items and items part of bundles</span> - Date format should be 2011-06-11 (11 of June 2011)</p>
            <form id="whoBoughtItemForm" method="post" action="">
                Who bought the itemId <%= Html.TextBox("itemId") %> between <%= Html.TextBox("from") %> (included) and <%= Html.TextBox("to") %> (not included) for <%= Html.DropDownList("whoBoughtItemDuration")%> 
                <input type="submit" id="whoBoughtItemButton" onclick="whoBoughtItem(); return false;" value="Tell me" /><img id="whoBoughtItemLoadingImg" style="display:none;" src="<%= Url.Content("~/Content/img/ajax-loader-pac-man.gif")%>" alt="Loader" />
                <p id="whoBoughtItemResultP" style="display:none;"><span id="whoBoughtItemResultCount"></span> Cmids: <span id="whoBoughtItemResult"></span></p>
            </form>
        </div>

        <div class="niceDiv">
            <h1>Vanity stats</h1>
            <form id="biggestCmidForm" method="post" action="">
                <input type="submit" id="biggestCmidButton" onclick="retrieveMembersCount(); return false;" value="What is the biggest Cmid (number of registered members)?" /><img id="biggestCmidLoadingImg" style="display:none;" src="<%= Url.Content("~/Content/img/ajax-loader-pac-man.gif")%>" alt="Loader" />
                <p id="biggestCmidResultP" style="display:none;">We have <span id="biggestCmidResult"></span> members registered (yes Nad I did this just for you :P)</p>
            </form>
            <form id="esnsMembersCountForm" style="margin-top:25px;" method="post" action="">
                <%= Html.DropDownList("membersCountEsns")%> <input type="submit" id="esnsMembersCountButton" onclick="retrieveEsnsMembersCount(); return false;" value="How many users from this esns accessed UberStrike?" /><img id="esnsMembersCountLoadingImg" style="display:none;" src="<%= Url.Content("~/Content/img/ajax-loader-pac-man.gif")%>" alt="Loader" />
                <p id="esnsMembersCountResultP" style="display:none;">We have <span id="esnsMembersCountResult"></span> <span id="esnsMembersCountEsnsName"></span> id</p>
            </form>
        </div>

        <div class="niceDiv">
            <h1>XP Farmers</h1>
            <form id="xpFarmersForm" method="post" action="">
                Day of Month (starting from yesterday, we keep 5 days of data): <%= Html.TextBox("xpFarmerDayOfMonth", ViewData["xpFarmerDayOfMonth"], new { style = "width:20px;" })%> (Above Kills: <%= Html.TextBox("xpFarmerKills", ViewData["xpFarmerKills"], new { style = "width:35px;" })%> Above KDR: <%= Html.TextBox("xpFarmerKdr", ViewData["xpFarmerKdr"], new { style = "width:20px;" })%>) <input type="submit" id="getXpFarmersButton" onclick="getDailyFarmers(); return false;" value="Get XP farmers" />
            </form>
            <div id="xpFarmersContainer"></div>
        </div>

        <div class="niceDiv">
            <h1>IP Tools</h1>
            <form id="ipConversionForm" method="post" action="">
                IP: <%= Html.TextBox("ipToConvert", null, new { style = "width:100px;" })%> <input type="submit" id="convertIpButton" onclick="convertIp(); return false;" value="Convert IP" />
            </form>
            <span id="ipConverterContainer"></span>
        </div>

    </div>
</asp:Content>