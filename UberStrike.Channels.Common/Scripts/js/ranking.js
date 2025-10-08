// ranking.js

function getDailyRanking() {
    getRanking('daily', $("#selectedPageDailyRanking").val());
}

function getAllTimeRanking() {
    getRanking('allTime', $("#selectedPageAllTimeRanking").val());
}

function getWeeklyRanking() {
    getRanking('weekly', $("#selectedPageWeeklyRanking").val());
}

function getRanking(rankingType, page) {

    var containerId = "dailyRankingContainer";
    var rankingEndpoint = "GetDailyRanking";

    if (rankingType == 'weekly') {
        containerId = "weeklyRankingContainer";
        rankingEndpoint = "GetWeeklyRanking";
    }
    if (rankingType == 'allTime') {
        containerId = "allTimeRankingContainer";
        rankingEndpoint = "GetAllTimeRanking";
    }

    $("#" + containerId).hide();
    $("#" + containerId).before(loadingImage.clone().attr('id', rankingType + 'Loader').attr('style', 'display:block; margin-top:20px; margin-bottom:20px; margin-left:auto; margin-right:auto;'));

    $.ajax({
        type: 'POST',
        url: applicationUrl + 'Ranking/' + rankingEndpoint,
        data: 'page=' + page + '&appCallbackUrl=' + appCallbackUrl,
        success: function (data) {
            $('#' + rankingType + 'Loader').remove();
            $("#" + containerId).html(data);
            $("#" + containerId).show();
        },
        error: function (data) {
            $('#' + rankingType + 'Loader').remove();
            $("#" + containerId).show();
        }
    });

    return false;
}

function displayRanking(rankingType) {
    $("#allTimeRanking").hide();
    $("#dailyRanking").hide();
    $("#weeklyRanking").hide();

    $("#dailyLink").unbind('click');
    $("#weeklyLink").unbind('click');
    $("#allTimeLink").unbind('click');

    $("#dailyLink").attr('class', 'rankingMenu');
    $("#weeklyLink").attr('class', 'rankingMenu');
    $("#allTimeLink").attr('class', 'rankingMenu');

    if (rankingType == 'daily') {
        $("#dailyRanking").show();
        $("#dailyLink").attr('class', 'rankingMenuSelected');
        $("#allTimeLink").click(function () { displayRanking('all'); });
        $("#weeklyLink").click(function () { displayRanking('weekly'); });
    }
    else if (rankingType == 'weekly') {
        $("#weeklyRanking").show();
        $("#weeklyLink").attr('class', 'rankingMenuSelected');
        $("#allTimeLink").click(function () { displayRanking('all'); });
        $("#dailyLink").click(function () { displayRanking('daily'); });
    }
    else {
        $("#allTimeRanking").show();
        $("#allTimeLink").attr('class', 'rankingMenuSelected');
        $("#dailyLink").click(function () { displayRanking('daily'); });
        $("#weeklyLink").click(function () { displayRanking('weekly'); });
    }
}