// UnityEmbedExtend.js - Portal

// Still in use
function displayPayment(cmid, paymentUrl, channel) {
    var paymentIframeContainer = $("#paymentIframeContainer");
    var paymentIframe = $("#creditBundleIframe");

    if (paymentIframeContainer.length > 0 && paymentIframe.length > 0) {
        HideUnity();
        paymentIframe.attr("src", paymentUrl + '?channel=' + channel + '&cmid=' + cmid);
        paymentIframeContainer.show();
        paymentIframe.show();
    }
}

// Still in use
function hidePayment() {
    var paymentIframeContainer = $("#paymentIframeContainer");
    var paymentIframe = $("#creditBundleIframe");

    if (paymentIframeContainer.length > 0 && paymentIframe.length > 0) {
        paymentIframeContainer.hide();
        paymentIframe.hide();
        ShowUnity();
    }
}

// Still in use
function getCreditsWrapper(cmid) {
    displayPayment(cmid, getCreditsIframeUrl, webChannel);
}

// Still in use but might need refactor
function displayHeader(cmid) {
    $.ajax({
        type: 'POST',
        url: refreshTopMenuUrl,
        success: function (data) {
            $("#littleTopMenuPartial").html(data);
        }
    });
}