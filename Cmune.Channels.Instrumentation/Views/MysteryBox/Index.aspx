<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<List<MysteryBoxView>>" %>

<asp:Content ID="Content2" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript" src="<%= Url.Content("~/Scripts/js/mysterybox.js")%>?v=<%= ViewBag.PublishVersion %>"></script>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div style="width: 1200px">
        <div>
            <br />
            <a onclick="LoadMysteryBoxForm(0);return false;" href="">Add a Mystery Box</a>
            <br />
            <br />
        </div>
        <% 
            int c = 0;
            foreach (var mysteryBox in Model)
            { %>
        <div class="left" style="width: 360px; margin-bottom: 20px; margin-left: 20px; padding: 10px;
            background: <%= mysteryBox.IsAvailableInShop ? "#F2DE5E": "#E8E8E8" %>;">
            <div>
                <div class="right">
                    <a onclick="LoadMysteryBoxForm(<%= mysteryBox.Id %>);return false;" href="">Edit</a>
                </div>
                <h3 style="text-align: left">
                    <%= mysteryBox.Name%>
                </h3>
            </div>
            <br />
            <div style="text-align: left;">
                <div>
                    <span class="left label">
                        <%= "Price" %>
                    </span><span class="smallInfo">
                        <%= mysteryBox.Price.ToString()%>
                        <%= mysteryBox.UberStrikeCurrencyType.ToString()   %>s </span>
                </div>
                <div>
                    <span class="left label">
                        <%= "Promo text" %>
                    </span><span class="smallInfo">
                        <%= mysteryBox.Description%>
                    </span>
                </div>
                <div>
                    <span class="left label">
                        <%= "Icon" %>
                    </span><span class="smallInfo" title="<%= mysteryBox.IconUrl %>">
                        <%= mysteryBox.IconUrl.ShortenText(35, true) %>
                    </span>
                </div>
                <div>
                    <span class="left label">
                        <%= "Category" %>
                    </span><span class="smallInfo">
                        <%=  mysteryBox.Category%>
                    </span>
                </div>
                <div>
                    <span class="left label">
                        <%= "Items Attributed" %>
                    </span><span class="smallInfo">
                        <%=  mysteryBox.ItemsAttributed%>
                    </span>
                </div>
            </div>
            <br />
            <fieldset>
                <legend>Prize</legend>
                <table class="searchResultTable">
                    <tr>
                        <th>
                            Item Name
                        </th>
                        <th>
                            Duration Type
                        </th>
                        <th>
                            Weight
                        </th>
                        <th>
                            Amount
                        </th>
                    </tr>
                    <%  
                int i = 0;
                foreach (var mysteryBoxItem in mysteryBox.MysteryBoxItems)
                {  %>
                    <tr <%= i % 2 == 0 ? "":"class=\"mod\"" %>>
                        <td>
                            <%=  mysteryBoxItem.Name %>
                        </td>
                        <td>
                            <%=  mysteryBoxItem.DurationType %>
                        </td>
                        <td>
                            <%=  mysteryBoxItem.ItemWeight %>
                        </td>
                        <td>
                            <%=  mysteryBoxItem.Amount %>
                        </td>
                    </tr>
                    <% i++;
                } %>
                </table>
                <br />
                <div style="text-align: left;">
                    <div>
                        <span class="left label">
                            <%= "Credits Attributed" %>
                        </span><span class="smallInfo">
                            <%= mysteryBox.CreditsAttributed %>
                        </span>
                    </div>
                    <div>
                        <span class="left label">
                            <%= "Credits Weight" %>
                        </span><span class="smallInfo">
                            <%= mysteryBox.CreditsAttributedWeight %>
                        </span>
                    </div>
                    <div>
                        <span class="left label">
                            <%= "Points Attributed" %>
                        </span><span class="smallInfo">
                            <%= mysteryBox.PointsAttributed %>
                        </span>
                    </div>
                    <div>
                        <span class="left label">
                            <%= "Points Weight"%>
                        </span><span class="smallInfo">
                            <%= mysteryBox.PointsAttributedWeight %>
                        </span>
                    </div>
                    <div>
                        <span class="left label">
                            <%= "Image Url" %>
                        </span><span class="smallInfo" title="<%= mysteryBox.ImageUrl %>">
                            <%= mysteryBox.ImageUrl.ShortenText(35, true)%>
                        </span>
                    </div>
                    <div>
                        <span class="left label">
                            <%= "Expose items" %>
                        </span><span class="smallInfo">
                            <%= Html.IconNotif(Url,mysteryBox.ExposeItemsToPlayers) %>
                        </span>
                    </div>
                </div>
            </fieldset>
        </div>
        <%
                c++;
                if ((c % 3) == 0)
                {%>
        <div class="clear">
        </div>
        <% }
        %>
        <% } %>
        <div class="clear">
        </div>
    </div>
</asp:Content>
