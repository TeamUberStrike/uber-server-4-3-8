function BuyBundle(bundleId) {
    $.ajax({
        type: 'POST',
        url: ApplicationUrl + '/Offer/GetBundleUniqueId' + "?" + GetKongregateLoginData(),
        data: { "bundleId": bundleId },
        success: function (data) {
            var itemIdentifier = [data.bundleUniqueId];
            purchaseBundle(itemIdentifier);
        }
    });
}

function purchaseBundle(itemIdentifier) {
    kongregate.mtx.purchaseItems(itemIdentifier, purchaseItemsCallback);
}

function purchaseItemsCallback(data) {
    if (data.success) {
        LoadPlayPage();
        setTimeout(RefreshWallet, 2000);
    }
}
