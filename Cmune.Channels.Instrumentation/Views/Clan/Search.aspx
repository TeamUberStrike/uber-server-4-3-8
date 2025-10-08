<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content2" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript" src="<%= Url.Content("~/Scripts/js/clan.js")%>"></script>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <div style="margin-left:auto; margin-right:auto; width:1000px;">
        <div style="border:1px solid black;background-color:#f2f2f2;padding:10px; width:700px; margin:30px 0 30px 0; margin-left:auto; margin-right:auto;">
            <h2>Search a clan</h2><br />
            <form action="/Member/GetClans" method="post" id="searchClanForm">
                <table cellpadding="4" cellspacing="4" style="margin-left:auto; margin-right:auto;">
                    <tr>
                        <td>
                            By name:
                        </td>
                        <td>
                            <%= Html.TextBox("ClanNameTextBox") %>
                        </td>
                        <td>
                            By tag:
                        </td>
                        <td>
                            <%= Html.TextBox("ClanTagTextBox")%>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            By clan member:
                        </td>
                        <td>
                            <%= Html.TextBox("ClanMemberNameTextBox")%>
                        </td>
                        <td colspan="2" style="text-align:center;">
                            <input type="submit" id="searchClanButton" onclick="searchClanGo(); return false;" value="Search" />
                        </td>
                    </tr>
                </table>
            </form>
        </div>
        <div id="clansListGrid" style="width:80%; margin:auto;">
        </div>
    </div>
</asp:Content>