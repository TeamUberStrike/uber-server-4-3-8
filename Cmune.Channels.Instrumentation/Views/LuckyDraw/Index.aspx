<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<List<LuckyDrawView>>" %>

<asp:Content ID="Content2" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript" src="<%= Url.Content("~/Scripts/js/luckydraw.js")%>?v=<%= ViewBag.PublishVersion %>"></script>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <div style="width: 1200px">
        <div>
            <br />
            <a onclick="LoadLuckyDrawForm(0);return false;" href="">Add a Lucky Draw</a>
            <br />
            <br />
        </div>
        <% 
            int c = 0;
            foreach (var luckyDraw in Model)
            { %>
        <div class="left" style="width: 360px; margin-bottom: 20px; margin-left: 20px; padding: 10px;
            background: <%= luckyDraw.IsAvailableInShop ? "#CCF45F": "#E8E8E8" %>;">
            <div>
                <div class="right">
                    <a onclick="LoadLuckyDrawForm(<%= luckyDraw.Id %>);return false;" href="">Edit</a>
                </div>
                <h3 style="text-align: left">
                    <%= luckyDraw.Name %>
                </h3>
            </div>
            <br />
            <div style="text-align: left;">
                <div>
                    <span class="left label">
                        <%= "Price" %>
                    </span><span class="smallInfo">
                        <%= luckyDraw.Price.ToString() %>
                        <%= luckyDraw.UberStrikeCurrencyType.ToString() %>s </span>
                </div>
                <div>
                    <span class="left label">
                        <%= "Promo text" %>
                    </span><span class="smallInfo">
                        <%= luckyDraw.Description %>
                    </span>
                </div>
                <div>
                    <span class="left label">
                        <%= "Thumb" %>
                    </span><span class="smallInfo" title="<%= luckyDraw.IconUrl %>">
                        <%= luckyDraw.IconUrl.ShortenText(35, true) %>
                    </span>
                </div>
                <div>
                    <span class="left label">
                        <%= "Category" %>
                    </span><span class="smallInfo">
                        <%=  luckyDraw.Category%>
                    </span>
                </div>
            </div>
            <br />
            <%  int j = 1;
                foreach (var luckyDrawSet in luckyDraw.LuckyDrawSets)
                { %>
            <div>
                <fieldset style="margin-top: 15px;">
                    <legend>Prize
                        <%= j %></legend>
                    <div>
                        <span class="left label">
                            <%= "Weight" %>
                        </span><span class="smallInfo">
                            <%= luckyDrawSet.SetWeight %>
                        </span>
                    </div>
                    <br />
                    <table class="searchResultTable">
                        <tr>
                            <th>
                                Item Id
                            </th>
                            <th>
                                Duration Type
                            </th>
                            <th>
                                Amount
                            </th>
                        </tr>
                        <% 
                    int i = 0;
                    foreach (var luckyDrawSetItem in luckyDrawSet.LuckyDrawSetItems)
                    {  %>
                        <tr <%= i % 2 == 0 ? "":"class=\"mod\"" %>>
                            <td>
                                <%= luckyDrawSetItem.Name %>
                            </td>
                            <td>
                                <%= luckyDrawSetItem.DurationType %>
                            </td>
                            <td>
                                <%= luckyDrawSetItem.Amount %>
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
                                <%= luckyDrawSet.CreditsAttributed %>
                            </span>
                        </div>
                        <div>
                            <span class="left label">
                                <%= "Points Attributed" %>
                            </span><span class="smallInfo">
                                <%= luckyDrawSet.PointsAttributed %>
                            </span>
                        </div>
                        <div>
                            <span class="left label">
                                <%= "Set Weight"%>
                            </span><span class="smallInfo">
                                <%= luckyDrawSet.SetWeight %>
                            </span>
                        </div>
                        <div>
                            <span class="left label">
                                <%= "Image Url" %>
                            </span><span class="smallInfo" title="<%= luckyDrawSet.ImageUrl %>">
                                <%= luckyDrawSet.ImageUrl.ShortenText(35, true)%>
                            </span>
                        </div>
                        <div>
                            <span class="left label">
                                <%= "Expose items" %>
                            </span><span class="smallInfo">
                                <%= Html.IconNotif(Url,luckyDrawSet.ExposeItemsToPlayers) %>
                            </span>
                        </div>
                    </div>
                </fieldset>
            </div>
            <% j++;
                } %>
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
