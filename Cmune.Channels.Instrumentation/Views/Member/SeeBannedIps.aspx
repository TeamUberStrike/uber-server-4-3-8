<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript" src="<%= Url.Content("~/Scripts/js/member.js")%>"></script>

    <script type="text/javascript">
        var loadingImage = $('<img />').attr('src', '<%= Url.Content("~/Content/img/ajax-loader-pac-man.gif")%>');
        loadBannedIps();
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div style="width:900px; margin:auto;">

        <div style="text-align:left; margin:25px 0 5px 0;">
            <form id="loadBannedIpsForm" action="" method="post">
                Search banned IP: <input id="searchIpTextBox" name="searchIpTextBox" type="text" /> <input id="searchIpButton" onclick="loadBannedIps(); return false;" value="Search" type="submit" /><br />
                <input id="loadPermanentBanOnly" name="loadPermanentBanOnly" type="checkbox" onchange="loadBannedIps();" /> <label for="loadPermanentBanOnly">Permanent ban only (not used when searching by IP)</label>
            </form>
        </div>

        <div id="BannedIpsContainer">
        </div>
    </div>
</asp:Content>