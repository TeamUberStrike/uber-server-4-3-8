<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content2" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript" src="<%= Url.Content("~/Scripts/js/member.js")%>?v=<%= ViewBag.PublishVersion %>"></script>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">

<div style="width:700px; margin-top:35px; margin-bottom:35px;">
    <div style="border:1px solid black; background-color:#F2F2F2; padding:10px; margin-top:15px;">
        <h2>Search a user</h2>
        <form id="searchUserForm" method="post" action="">
            <table class="userSearchTable">
                <tr>
                    <td>Email: <%= Html.TextBox("memberEmail", null, new { style = "width:200px;" })%></td>
                    <td>
                        Name:  <%= Html.TextBox("memberName")%>
                        <%= Html.CheckBox("byExactName", true)%>
                        <label for="byExactName">By exact name</label>
                    </td>
                </tr>
                <tr>
                    <td>Cmid: <%= Html.TextBox("cmid", null, new { style = "width:60px;" })%></td>
                    <td>SNS: <%= Html.DropDownList("esnsType")%> Id <%= Html.TextBox("memberHandle") %></td>
                </tr>
            </table>
            <input type="submit" id="searchUsersButton" onclick="searchUsers(); return false;" value="Search" />
        </form>
        <hr />
        <form id="searchByPreviousEmailForm" method="post" action="">
            <p style="text-align:left;">
                Previous email: <%= Html.TextBox("previousEmail", null, new { style = "width:200px;" })%> <input type="submit" id="searchUsersByPreviousEmailButton" onclick="searchUsersByPreviousEmail(); return false;" value="Search" />
            </p>
        </form>
        <form id="searchByPreviousNameForm" method="post" action="">
            <p style="text-align:left;">
                Previous name: <%= Html.TextBox("previousName", null, new { style = "width:200px;" })%> <input type="submit" id="searchUsersByPreviousNameButton" onclick="searchUsersByPreviousName(); return false;" value="Search" />
            </p>
        </form>

        <%--<script type="text/javascript">
            // TODO proper key up
            $("#memberName").keyup(function (event) {
                if (event.keyCode == 13) {
                    $("#searchUsersButton").click();
                }
            });
            $("#previousEmail").keyup(function (event) {
                if (event.keyCode == 13) {
                    $("#searchUsersByPreviousEmailButton").click();
                }
            });
            $("#previousName").keyup(function (event) {
                if (event.keyCode == 13) {
                    $("#searchUsersByPreviousNameButton").click();
                }
            });
        </script>--%>

    </div>
</div>

<div style="width:600px; margin:auto;">
    <div id="loadingRes" style="display:none; text-align:center;">Results are loading... <br /><img src="<%= Url.Content("~/Content/img/ajax-loader-pac-man.gif")%>" alt="Loader" /></div>
    <div id="noResultLabel" style="display:none; text-align:center;" >Your search didn't return any result.</div>
</div>
        
<div id="searchUsersResultContainer" style="width:80%; margin:auto;">
</div>

<div id="searchUsersByPreviousEmailResultContainer" style="width:80%; margin:auto;">
</div>

<div id="searchUsersByPreviousNameResultContainer" style="width:80%; margin:auto;">
</div>

</asp:Content>