<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<%@ Import Namespace="Cmune.Channels.Instrumentation.Extensions" %>

<asp:Content ID="Content2" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript" src="<%= Url.Content("~/Scripts/js/clan.js")%>"></script>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <div style="width:1000px;">
        <h1>See clan</h1>
        <p style="text-align:left;">
            <a href="<%= Url.Action("Search", "Clan")%>">Search another clan</a>
        </p>
        <% if ((bool)ViewData["IsClanExisting"])
           { %>
        <h2 style="text-align:left; margin-top:20px;"><%= ViewBag.ClanTag%></h2>
        <div class="clear"></div>
        <hr />
        <form id="editClanForm" action="" method="post">
            <%= Html.Hidden("action")%>
            <%= Html.Hidden("clanId")%>
            <%= Html.Hidden("memberCmid")%>
            <%= Html.Hidden("memberName")%>
            <%= Html.Hidden("memberEmail")%>
            <table class="left">
                <tr style="height:30px;">
                    <td style="width:70px;">
                        Tag
                    </td>
                    <td>
                        <div id="tagDiv" style="margin:-4px 0 0 0;">
                            <%: ViewData["ClanTag"]%>
                            <%= Html.Hidden("clanTag")%>
                            <%= Html.ChangeFieldTogglePanel("editTagPanel", "ClanTag", false, "ChangeClan('changeTag'); return false;", Url.Content("~/Content/img/b_edit.png"))%>
                        </div>
                    </td>
                </tr>
                <tr style="height:30px;">
                    <td>
                        Name
                    </td>
                    <td>
                        <div id="nameDiv" runat="server" style="margin:-4px 0 0 0;">
                            <span id="clanNameLabel"><%: ViewData["ClanName"] %></span>
                            <%= Html.Hidden("clanName")%>
                            <%= Html.ChangeFieldTogglePanel("editNamePanel", "ClanName", false, "ChangeClan('changeName'); return false;", Url.Content("~/Content/img/b_edit.png"))%>
                        </div>
                    </td>
                </tr>
                <tr style="height:30px;">
                    <td>
                        Motto
                    </td>
                    <td>
                        <div id="mottoDiv" runat="server" style="margin:-4px 0 0 0;">
                            <span id="clanMottoLabel"><%: ViewData["clanMotto"]%></span>
                            <%= Html.Hidden("clanMotto")%>
                            <%= Html.ChangeFieldTogglePanel("editMottoLink", "ClanMotto", false, "ChangeClan('changeMotto'); return false;", Url.Content("~/Content/img/b_edit.png"))%>
                        </div>
                    </td>
                </tr>
            </table>
            <div class="right">
                <div id="disbandClanDiv">
                    <a href="#" onclick="deleteClan(); return false">Disband clan</a>
                </div>
            </div>
            <div class="clear"></div>
            <table class="searchResultTable" style="margin-top:40px;">
                <tr>
                    <th></th>
                    <th>Name</th>
                    <th>Position</th>
                    <th>Joining date</th>
                    <th>Last login</th>
                    <th></th>
                    <th></th>
                </tr>
                <% 
               int i = 0;
               foreach (ClanMemberView clanMember in (List<Cmune.DataCenter.Common.Entities.ClanMemberView>)ViewData["clanMembers"])
               {
                        %>
                <tr class="<%= (i % 2) == 1 ? "mod":""%>">
                    <td>
                        <a href='<%= Url.Action("See","Member") %>?cmid=<%= clanMember.Cmid %>' target="_blank">See profile</a>
                    </td>
                    <td>
                        <%: clanMember.Name%>
                    </td>
                    <td>
                        <%= clanMember.Position%>
                    </td>
                    <td>
                        <%= clanMember.JoiningDate.ToNiceDisplay() %>
                    </td>
                    <td>
                        <%= clanMember.Lastlogin.ToNiceDisplay()%>
                    </td>
                    <td>
                        <% if (clanMember.Position != GroupPosition.Leader) { %>
                        <a id="makeLeader<%= clanMember.Cmid%>Link" href="#" onclick="makeLeader(<%= ViewData["clanId"]%>, <%= ViewData["memberCmid"]%>, <%= clanMember.Cmid%>); return false;">Make leader</a>
                        <% } %>
                    </td>
                    <td>
                        <% if (clanMember.Position != GroupPosition.Leader) { %>
                        <a id="kick<%= clanMember.Cmid%>Link" href="#" onclick="kickFromClan(<%= ViewData["clanId"]%>, <%= clanMember.Cmid%>); return false;">Kick from clan</a>
                        <% } %>
                    </td>
                </tr>
                <%
                   i++;
               }  %>
        </table>
        </form>
        <% }
           else
           { %>
        <p style="margin-top:30px;">This clan doesn't exist.</p>
        <% } %>
    </div>
</asp:Content>