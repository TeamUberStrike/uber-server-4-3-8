function updateChart(swfId, urlToGetData) {
    var tmp = findSWF(swfId);
    x = tmp.reload(urlToGetData);
}

function findSWF(swfId) {
    if (navigator.appName.indexOf("Microsoft") != -1) {
        return window[swfId];
    }
    else {
        return document[swfId];
    }
}