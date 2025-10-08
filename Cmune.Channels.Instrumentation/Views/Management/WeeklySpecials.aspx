<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript" src="<%= Url.Content("~/Scripts/js/management.js")%>?v=<%= ViewBag.PublishVersion %>"></script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div style="width:1200px;">
        <div class="niceDiv">
            <h1>Weekly Specials</h1>

            <div id="weeklySpecialsContainer" style="margin-top:25px;">
            </div>

        </div>
    </div>
    <script type="text/javascript">
        getWeeklySpecials();
    </script>
</asp:Content>