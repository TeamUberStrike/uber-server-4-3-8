<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content2" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript" src="<%= Url.Content("~/Scripts/js/management.js")%>?v=<%= ViewBag.PublishVersion %>"></script>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <div style="width:1000px; margin:auto;">
        <h1>Messaging</h1>

        <div style="border:1px solid #f2f2f2; background-color:#f2f2f2; padding:10px; margin-top:30px; width:610px; border-radius:10px;">
            <h3 style="margin-bottom:20px;">Send Message to all users who logged in the last 30 days</h3>
            <form id="sendMessageForm" name="" method="post" action="">
                <%= Html.TextArea("message", ViewData["Message"]) %>
                <input id="messageSubmitButton" type="button" onClick="sendMessageToAllUsers();" value="Send" />
            </form>
        </div>
    </div>
</asp:Content>