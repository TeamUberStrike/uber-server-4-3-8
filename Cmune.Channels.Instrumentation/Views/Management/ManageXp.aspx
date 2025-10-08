<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content2" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript" src="<%= Url.Content("~/Scripts/js/management.js")%>?v=<%= ViewBag.PublishVersion %>"></script>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div style="width:1200px; margin:auto; padding-top:15px;">
        <h1 onclick="$('#xpEventsTopContainer').toggle();" style="cursor:pointer;">Manage XP Events</h1>
        <div id="xpEventsTopContainer" class="niceDiv" style="display:none;">
            <div id="xpEventsContainer">
            </div>
        </div>
        <script type="text/javascript">
            getXpEvents();
        </script>

        <h1 onclick="$('#reSyncXpLevelContainer').toggle();" style="cursor:pointer;">Resynchronise Levels and XP</h1>

        <div id="reSyncXpLevelContainer" class="niceDiv" style="display:none;">
            This will modify the LEVEL: <input id="resynchroniseLevelsBasedOnXpButton" type="button" onclick="resynchroniseLevelsBasedOnXp(); return false;" value="Resynchronise Levels Based on XP" /><br />
            This will modify the XP: <input id="resynchroniseXpBasedOnLevelButton" type="button" onclick="resynchroniseXpBasedOnLevel(); return false;" value="Resynchronise XP Based on Levels" />
            <div id="syncXpLevelExplanation" style="font-weight:bold; display:none; margin-top:15px; margin-bottom:15px;">Get your favorite dev to execute those queries for you:</div>
            <div id="syncXpLevelQueries" style="text-align:left;">
            </div>
        </div>

        <h1 onclick="$('#levelCapsTopContainer').toggle();" style="cursor:pointer;">Manage Level Caps</h1>

        <div id="levelCapsTopContainer" class="niceDiv" style="display:none;">
            <p style="margin-bottom:15px;">Do not forget that the tutorial attributes <%= ViewBag.XpAttributedOnTutorialCompletion %> XP and it should bring the user close of being level 3.</p>
            <div id="levelCapsContainer">
            </div>
        </div>

        <script type="text/javascript">
            getLevelCaps();
        </script>

        <h1 onclick="$('#itemsAttributedTopContainer').toggle();" style="cursor:pointer;">Items attributed on tutorial</h1>
        <div id="itemsAttributedTopContainer" class="niceDiv" style="display:none;">
            <div id="itemsAttributedContainer">
            </div>
        </div>
        <script type="text/javascript">
            getItemsAttributed();
        </script>
    </div>
</asp:Content>