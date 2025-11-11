<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content2" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript" src="<%= Url.Content("~/Scripts/js/calendarDateInput/calendarDateInit.js")%>?v=<%= ViewBag.PublishVersion %>"></script>
    <script type="text/javascript" src="<%= Url.Content("~/Scripts/js/calendarDateInput/calendarDateInput.js")%>?v=<%= ViewBag.PublishVersion %>"></script>
    <script type="text/javascript" src="<%= Url.Content("~/Scripts/js/management.js")%>?v=<%= ViewBag.PublishVersion %>"></script>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <div style="width:1000px; margin:auto;">
        <h1>E-Pins</h1>

        <div style="border:1px solid #f2f2f2; background-color:#f2f2f2; padding:10px; margin:10px; width:300px; border-radius:10px;">
            <h3>Generation</h3>
            <form id="generateEpinsForm" name="generateEpinsForm" method="post" action="">
                <table style="margin:20px 0 15px 0;">
                    <tr>
                        <td>E-Pin Provider:</td>
                        <td><%= Html.DropDownList("EpinProvider", (List<SelectListItem>)ViewData["ProvidersList"], new { onchange = "isEpinAdmin($('#EpinProvider').val());" })%></td>
                    </tr>
                    <tr>
                        <td>Amount to generate:</td>
                        <td><%= Html.TextBox("EpinAmount", null, new { style = "width:58px;" })%></td>
                    </tr>
                    <tr>
                        <td>Credits to assign:</td>
                        <td><%= Html.TextBox("CreditAmount", null, new { style = "width:58px;" })%></td>
                    </tr>
                    <tr id="isAdminTr" style="text-align:center;">
                        <td colspan="2"><label for="IsAdmin">Is admin</label>: <%= Html.CheckBox("IsAdmin")%></td>
                    </tr>
                </table>
            </form>
            <input id="generateEpinsButton" type="button" onclick="generateEpins(); return false;" value="Generate" />
        </div>       

        <div id="epinBatchesContainer"></div>

        <div style="border:1px solid #f2f2f2; background-color:#f2f2f2; padding:10px; margin-top:30px; width:610px; border-radius:10px;">
            <h3 style="margin-bottom:20px;">Search</h3>
            <form id="epinsSearchForm" name="epinsSearchForm" method="post" action="">
                <%= Html.TextBox("EpinsSearch", null, new { style = "width:450px;" })%>
                <input id="epinsSearchButton" type="button" onclick="searchEpins(); return false;" value="Search" />
                <img id="epinsSearchFormat" alt="E-pins search format" src="<%= Url.Content("~/Content/img/question.jpg")%>" />
            </form>
        </div>

        <div id="epinsSearchTooltip" style="display:none; position:absolute; text-align:left; background:#FFFEBA; padding:10px; border:3px solid #7ABBF7; -moz-border-radius:5px; border-radius:5px;">
            Search by:
            <ul>
                <li>Multiple Epin Ids: "1,4,5" (Id 1, 4 and 5)</li>
                <li>Range of Epin Ids: "1-5" (between 1 and 5)</li>
                <li>Pins: "vsc14cd,bvfbggfd" (matching with the strings)</li>
            </ul>
        </div>

        <div id="epinsContainer" style="margin-top:15px;"></div>

        <script type="text/javascript">
            $('#epinsSearchFormat').hover(function () {
                $('#epinsSearchTooltip').css({ top: $('#epinsSearchFormat').offset().top + 'px', left: $('#epinsSearchFormat').offset().left + 50 + 'px' });
                $('#epinsSearchTooltip').fadeIn();
            }, function () {
                $('#epinsSearchTooltip').fadeOut();
            });

            getEpinBatches();
        </script>
    </div>
</asp:Content>