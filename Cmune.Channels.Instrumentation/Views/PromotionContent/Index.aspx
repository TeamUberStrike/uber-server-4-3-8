<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<List<PromotionContentViewModel>>" %>

<%@ Import Namespace="Cmune.Channels.Instrumentation.Extensions" %>
<%@ Import Namespace="Cmune.DataCenter.Utils" %>


<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript" src="<%= Url.Content("~/Scripts/js/promotionAds.js")%>"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div>
        <br />
        <a href="" onclick="LoadAddEditPromotionContentForm(0);return false;">Add Promotion
            ads</a>
        <br />
        <br />
        <table class="searchResultTable" style="text-align: center">
            <tr>
                <th style="width: 30px;">
                </th>
                <th style="width: 200px;">
                    Name
                </th>
                <th style="width: 200px;">
                    Start Date
                </th>
                <th style="width: 200px;">
                    End Date
                </th>
                <th style="width: 200px;">
                    IsPermanent
                </th>
                <th style="width: 200px;">
                </th>
            </tr>
            <% 
                int i = 0;
                foreach (var promotionContent in Model.OrderByDescending(d => d.StartDate))
                { %>
            <tr class="<%= (i % 2) == 1 ? "mod":""%>">
                <td style="width: 30px; cursor: pointer; font-weight: bold;" onclick="$('#promotionContentElements<%= promotionContent.PromotionContentId %>').toggle()">
                    <% if (promotionContent.PromotionContentElements.Count > 0)
                       { %>
                    <img src="<%= Url.Content("~/Content/img/next.gif") %>" />
                    <% } %>
                </td>
                <td style="width: 200px; cursor: pointer; font-weight: bold;" onclick="$('#promotionContentElements<%= promotionContent.PromotionContentId %>').toggle()">
                    <div style="">
                        <%= promotionContent.Name%></div>
                </td>
                <td style="width: 200px;">
                    <%= promotionContent.StartDate.ToNiceDisplay()%>
                </td>
                <td style="width: 200px;">
                    <%= promotionContent.EndDate.ToNiceDisplay()%>
                </td>
                <td style="width: 200px;">
                    <%= promotionContent.IsPermanent ? "Permanent" : ""%>
                </td>
                <td style="width: 200px;">
                    <a href="" onclick="LoadAddEditPromotionContentForm(<%= promotionContent.PromotionContentId %>);return false;">
                        Edit</a> | <a href="" onclick="DeletePromotionContent(<%= promotionContent.PromotionContentId %>); return false;">
                            Delete</a>
                </td>
            </tr>
            <% if (promotionContent.PromotionContentElements.Count > 0)
               { %>
            <tr style="display: none;" id="promotionContentElements<%= promotionContent.PromotionContentId %>">
                <td colspan="6">
                    <div>
                        <div class="left" style="width: 150px">
                            Channel Type</div>
                        <div class="left" style="width: 150px">
                            Channel Element</div>
                        <div class="left" style="width: 150px">
                            Filename Title</div>
                        <div class="left">
                            Filename</div>
                        <div class="left">
                            Anchor Link</div>
                        <div class="clear">
                        </div>
                        <br />
                        <% foreach (var promoElement in promotionContent.PromotionContentElements.OrderBy(d => d.ChannelType).ToList())
                           { %>
                        <div class="left" style="width: 150px">
                            <%= ((ChannelType)promoElement.ChannelType).ToString() %></div>
                        <div class="left" style="width: 150px">
                            <%= ((ChannelElement)promoElement.ChannelElement).ToString()%></div>
                        <div class="left" style="width: 150px">
                            <%= promoElement.FilenameTitle %>
                        </div>
                        <div class="left" style="overflow: hidden; width: 1000px;">
                            <img src="<%= ConfigurationUtilities.ReadConfigurationManager("CommonImagesRoot") %><%= promoElement.Filename %>" alt="<%= promoElement.FilenameTitle %>" />
                        </div>
                        <div class="left">
                            <%= promoElement.AnchorLink %></div>
                        <div class="clear">
                        </div>
                        <% } %>
                    </div>
                </td>
            </tr>
            <% } %>
            <% 
               i++;
                } %>
        </table>
    </div>
</asp:Content>
