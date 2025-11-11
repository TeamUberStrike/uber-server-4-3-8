// UnityEmbedExtend.js - Facebook

// Still in use
function facebookCreditsCallback(data) {
    if (MenuMainTab_CurrentTab == MenuMainTab_Home)
    {
        ShowUnity();
    }

    if (data.status != undefined && data.status == "settled") {
        LoadHomePage();
        setTimeout(function () { RefreshWallet() }, 300);
    }
};

// Still in use
function getCreditsWrapper(cmid) {
    HideUnity();
    LoadGetCreditsPage();
}

// Still in use
function showFBInviteFriendsLightbox() {
    LoadInviteFriendsPage();
}


// Still in use but might need refactor
function displayHeader(cmid) {
    $.ajax({
        type: 'POST',
        url: refreshTopMenuUrl,
        success: function (data) {
            $("#TopPartialMenu").html(data);
        }
    });
}

// Still in use but might need refactor -> common with portal
function getFacebookUsername() {
    return fbName;
}