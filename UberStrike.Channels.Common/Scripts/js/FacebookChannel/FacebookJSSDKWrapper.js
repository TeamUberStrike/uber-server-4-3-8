function FacebookSetSize(canvasSize) {
    FB.Canvas.setSize({ height: canvasSize + "px" });
}

function FacebookScrollTo(x, y) {
    FB.Canvas.scrollTo(x, y);
}

function FacebookNavigate(url) {
    window.top.location.href = url;
}

function FacebookOpen(url) {
    window.open(url);
}

var hasBeenOnEpinTransaction = false;

function RegisterEpinStep() {
    hasBeenOnEpinTransaction = true;
}

function HandleBrowsingEvent() {
    if (hasBeenOnEpinTransaction == true) {
        RefreshWallet();
        hasBeenOnEpinTransaction = false;
    }
}

function EarnCredits() {
    // calling the API ...
    var obj = {
        method: 'pay',
        credits_purchase: true,
        dev_purchase_params: { "shortcut": "offer" }
    };

    if (MenuMainTab_CurrentTab == MenuMainTab_Home) {
        HideUnity();
    }
    FB.ui(obj, LoadGetCreditsPage);
}

function BuyBundle(bundleId) {
    var order_info = {
        "bundleId": bundleId
    }

    var obj = {
        method: 'pay',
        order_info: order_info,
        purchase_type: 'item'
    };

    if (MenuMainTab_CurrentTab == MenuMainTab_Home) {
        HideUnity();
    }
    FB.ui(obj, facebookCreditsCallback);
}

