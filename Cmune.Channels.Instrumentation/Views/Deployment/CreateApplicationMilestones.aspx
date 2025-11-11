<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content2" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript" src="<%= Url.Content("~/Scripts/js/deployment.js")%>"></script>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">

<div id="statsDiv" style="width:1200px; text-align:left;">

    <h1>Application Milestones</h1>

    <form action="" method="post" id="createMilestoneForm">
        <h3>Add a new application milestone</h3>
        <p style="text-align:left; margin:auto;">
            <table>
                <tr>
                    <td>Release date:</td>
                    <td><%= Html.TextBox("MilestoneDateTextBox")%></td>
                    <td><img id="dateFormat" alt="Date format" src="<%= Url.Content("~/Content/img/question.jpg")%>" /></td>
                </tr>
                <tr>
                    <td>Description:</td>
                    <td><%= Html.TextBox("MilestoneDescriptionTextBox")%></td>
                    <td><input type="submit" name="createMilestoneButton" id="createMilestoneButton" value="Create" onclick="createApplicationMilestone(); return false;" /></td>
                </tr>
            </table>
        </p>
    </form>

    <div id="dateTooltip" style="display:none; position:absolute; text-align:center; background:#FFFEBA; padding:10px; border:3px solid #7ABBF7; -moz-border-radius:50px; border-radius:35px;">Leave empty to use the current date.<br />"2011-08-18 12:25:33" stands for 18 of August 2011 12:25:33.<br />you can use a short version also "2011-08-18".</div>

    <div id="milestonesDiv" style="margin:50px 0 0 0;">
    </div>

    <script type="text/javascript">
        $('#dateFormat').hover(function () {
            $('#dateTooltip').css({ top: $('#dateFormat').offset().top + 'px', left: $('#dateFormat').offset().left + 50 + 'px' });
            $('#dateTooltip').fadeIn();
        }, function () {
            $('#dateTooltip').fadeOut();
        });

        getMilestones();
    </script>

</div>

</asp:Content>