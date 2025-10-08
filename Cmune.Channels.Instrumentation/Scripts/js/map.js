function GetMapItems(url, applicationVersion) {
    $.ajax({
        type: 'POST',
        url: url,
        data: { 'applicationVersion': applicationVersion },
        success: function (data) {
            $("#mapItemsContainer").html(data);
        },
            error: function (data) {
        }
    });
}

function LoadEditMapItem(url, applicationVersion, mapId) {
    $.ajax({
        type: 'POST',
        url: url,
        data: { 'applicationVersion' : applicationVersion, 'mapId': mapId },
        success : function (data) {
            Shadowbox.open({
                content: data,
                player: "html",
                title: "",
                height:100
            });
        },
        error: function (data) {
            
        }
    });
}

function SaveMapItem(url, urlRefresh, applicationVersion, mapId, recommendedItemId) {
    $.ajax({
        type: 'POST',
        url: url,
        data: { 'applicationVersion': applicationVersion, 'mapId': mapId, 'recommendedItemId': recommendedItemId },
        success: function (data) {
            if (data.success) {
                showSummarySuccess(data.message);
                Shadowbox.close();
                GetMapItems(urlRefresh, applicationVersion);
            }
            else {
                showSummaryError(data.message);
            }
        },
        error: function (data) {
            showSummaryError("ajax error occured");
        }
    });
}
