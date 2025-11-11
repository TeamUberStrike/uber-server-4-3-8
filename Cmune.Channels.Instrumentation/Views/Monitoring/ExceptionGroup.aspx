<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Cmune.Instrumentation.Monitoring.Common.Entities.UnityExceptionGroupDetailedView>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div style="width:1200px;">
        <h1>Exception group</h1>

        <p style="margin:0 0 50px 0; text-align:left;"><a href="<%= Url.Action("UnityExceptions", "Monitoring") %>">View all the exception groups</a></p>

        <% if (Model != null) { %>
        <table>
            <tr>
                <td class="bold" style="width:140px;">Exception type:</td>
                <td style="padding-bottom:15px;"><%= Model.Group.ExceptionType %></td>
            </tr>
            <tr>
                <td class="bold">Exception message:</td>
                <td style="padding-bottom:15px;"><%= Model.Group.ExceptionMessage %></td>
            </tr>
            <tr>
                <td class="bold">Faultive function:</td>
                <td style="padding-bottom:15px;"><%= Model.Group.FaultiveFunction %></td>
            </tr>
            <tr>
                <td class="bold">Stacktrace:</td>
                <td><%= Model.Stacktrace %></td>
            </tr>
        </table>
        <table class="searchResultTable" style="margin-top:50px;">
            <tr>
                <th>Build number</th>
                <th>Date</th>
                <th>Cmid</th>
                <th>Channel</th>
                <th></th>
            </tr>
        <% int i = 0; 
           foreach (var exception in Model.Exceptions)
           { %>
            <tr class="<%= (i % 2) == 1 ? "mod":""%>">
                <td><%= exception.BuildNumber %></td>
                <td><%= exception.ExceptionTime.ToString("MM/dd/yyyy HH:mm:ss")%></td>
                <td><%= exception.Cmid %></td>
                <td><%= exception.Channel %></td>
                <td><a href="<%= Url.Action("Exception", "Monitoring") %>?exceptionId=<%= exception.UnityExceptionId %>" title="View exception"><img src="<%= Url.Content("~/Content/img/eye.png")%>" alt="View exceptions" /></a></td>
            </tr>
        <% i++;
           } %>
        </table>
        <% } else { %>
        <p>This group doesn't exist.</p>
        <% } %>
    </div>
</asp:Content>