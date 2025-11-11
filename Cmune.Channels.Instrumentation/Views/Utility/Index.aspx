<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content2" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript" src="<%= Url.Content("~/Scripts/js/utility.js")%>"></script>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
<div style="width:900px; margin:auto;">
    <h1>Utilities</h1>

    <div class="niceDiv">
        <h1>Send single email</h1>
        <form id="sendSingleEmailForm" method="post" action="">
            <p style="text-align:left;">
                <span class="bold">From</span> email address: <%= Html.TextBox("fromEmailAddress") %> and name: <%= Html.TextBox("fromName") %><br />
                <span class="bold">To</span> email address: <%= Html.TextBox("toEmailAddress") %> and name: <%= Html.TextBox("toName") %>
            </p>
            <p style="text-align:left; text-indent:110px; margin:15px 0 15px 0;">
                <span class="bold">Subject</span>: <%= Html.TextBox("subject", null, new { style = "width:300px;" })%> <span class="italic">(can't contain HTML)</span><br />
            </p>
            <span class="bold">Html</span> body:<br />
            <%= Html.TextArea("htmlBody", null, new { cols = "80", rows = "15", style = "margin:10px 0 10px 0;" })%><br />
            <span class="bold">Text</span> body:<br />
            <span class="italic">(can't contain HTML, replacement of Html body for non HTML clients)</span><br />
            <%= Html.TextArea("textBody", null, new { cols = "80", rows = "15", style = "margin:10px 0 15px 0;" })%><br />
            <input id="sendSingleEmailButton" type="submit" onclick="sendSingleEmail(); return false;" value="Send" />
        </form>
    </div>

    <div class="niceDiv">
        <h1>Send multiple emails with variables replacement</h1>
        <form id="sendMultipleEmailsForm" method="post" action="">
            <input id="sendMultipleEmailsButton" type="submit" onclick="sendMultipleEmails(); return false;" value="Send" />
        </form>
    </div>

</div>
</asp:Content>