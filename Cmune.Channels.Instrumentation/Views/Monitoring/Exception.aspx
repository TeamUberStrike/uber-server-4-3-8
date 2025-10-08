<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Cmune.Instrumentation.Monitoring.Common.Entities.UnityExceptionView>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div style="width:1200px;">

        <h1>Exception</h1>

        <p style="margin:0 0 50px 0; text-align:left;">
            <a href="<%= Url.Action("UnityExceptions", "Monitoring") %>">View all the exception groups</a><br />
            <% if (Model != null) { %>
            <a href="<%= Url.Action("ExceptionGroup", "Monitoring") %>?stacktraceHash=<%= Model.StacktraceHash %>">View the exception group</a>
            <% } %>
        </p>

        <% if (Model != null) { %>
        <table>
            <tr>
                <td class="bold" style="width:140px;">Exception type:</td>
                <td style="padding-bottom:15px;"><%= Model.ExceptionType %></td>
            </tr>
            <tr>
                <td class="bold">Exception message:</td>
                <td style="padding-bottom:15px;"><%= Model.ExceptionMessage %></td>
            </tr>
            <tr>
                <td class="bold">Cmid:</td>
                <td style="padding-bottom:15px;"><%= Model.Cmid %></td>
            </tr>
            <tr>
                <td class="bold">Channel:</td>
                <td style="padding-bottom:15px;"><%= Model.Channel %></td>
            </tr>
            <tr>
                <td class="bold">Build:</td>
                <td style="padding-bottom:15px;"><%= Model.Build %></td>
            </tr>
            <tr>
                <td class="bold">Build number:</td>
                <td style="padding-bottom:15px;"><%= Model.BuildNumber%></td>
            </tr>
            <tr>
                <td class="bold">Exception time:</td>
                <td style="padding-bottom:15px;"><%= Model.ExceptionTime.ToString("MM/dd/yyyy HH:mm:ss")%></td>
            </tr>
            <tr>
                <td class="bold">Faultive function:</td>
                <td style="padding-bottom:15px;"><%= Model.FaultiveFunction%></td>
            </tr>
            <tr>
                <td class="bold">Stacktrace:</td>
                <td style="padding-bottom:15px;"><%= Model.Stacktrace%></td>
            </tr>
            <tr>
                <td class="bold">Exception data:</td>
                <td style="padding-bottom:15px;"><%= Model.ExceptionData%></td>
            </tr>
        </table>
        <% } else { %>
        <p>This exception doesn't exist.</p>
        <% } %>
    </div>
</asp:Content>