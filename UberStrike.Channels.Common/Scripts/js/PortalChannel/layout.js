var CreditBundleIframeInitialHeight = 0;
var ItemBundleIframeInitialHeight = 0;
function ResizeGetCreditBundlePage(height) {
CreditBundleIframeInitialHeight = document.getElementById("creditBundleIframe").height;
    document.getElementById("creditBundleIframe").height = height;
}

function ResetGetCreditBundlePage()
{
    document.getElementById("creditBundleIframe").height = CreditBundleIframeInitialHeight;
}

function ResizeGetItemBundlePage(height) {
    ItemBundleIframeInitialHeight = document.getElementById("itemBundleIframe").height;
    document.getElementById("itemBundleIframe").height = height;
}

function ResetGetItemBundlePage() {
    document.getElementById("itemBundleIframe").height = ItemBundleIframeInitialHeight;
}

function portallogin() {
    $.ajax({
        type: "POST",
        url: applicationUrl + "/Account/Login",
        data: $('#loginForm').serialize(),
        success: function (data) {
            if (data.isSuccessfull) {
                window.location.href = applicationUrl;
            }
            else {
                $("#uberstrikeValidationSummary").html(data.message);
                $("#uberstrikeValidationSummary").attr("class", "errorMessage");
            }
        }
    });
}

function externalLogin() {
    $.ajax({
        type: "POST",
        url: applicationUrl + "/Account/LoginFromExternal",
        data: $('#loginForm').serialize(),
        success: function (data) {
            if (data.IsSuccessfull) {
                NavigateToUrl(data.Url);
            }
            else {
                $("#uberstrikeValidationSummary").html(data.ErrorMessage);
                $("#uberstrikeValidationSummary").attr("class", "errorMessage");
            }
        }
    });
}

function updateAccount() {
    $("#resultMessageContainer").html('');
    $("#resultMessageContainer").hide();

    $("#updateAccountButton").before(loadingImage.clone().attr('id', 'updateAccountLoader'));
    $("#updateAccountButton").hide();

    $.ajax({
        type: "POST",
        url: applicationUrl + "/Account/UpdateAccount",
        data: $('#updateForm').serialize(),
        success: function (data) {
            if (data.ErrorMessage == '') {
                $("#updateForm").hide();
                $("#successDiv").show();
                $("#updateAccountButton").show();
                $("#updateAccountLoader").remove();
            }
            else {
                $("#resultMessageContainer").html(data.ErrorMessage);
                $("#resultMessageContainer").attr("class", "errorMessage");
                $("#resultMessageContainer").show();
                $("#updateAccountButton").show();
                $("#updateAccountLoader").remove();
            }
        }
    });
}

function loginFacebookExternal(returnUrl, channel) {
    FB.login(function (response) {
        if (response.authResponse) {
            // user authorized
            window.location.href = applicationUrl + "?facebookExternalLogin=1&returnUrl=" + returnUrl + "&channel=" + channel;
        } else {
            // user cancelled
        }
    }, { scope: 'publish_stream,email,publish_actions' });
};

function facebooklogin() {
    FB.login(function (response) {
        if (response.authResponse) {
            // user authorized
            window.location.href = applicationUrl + "?facebookLogin=1&returnUrl=" + $("#ReturnUrl").val();
        } else {
            // user cancelled
        }
    }, { scope: 'publish_stream,email,publish_actions' });
};

function facebooklogout() {
    FB.logout(function (response) { window.location.href = logOffUrl; });
}

function GoLinkFacebookAccount() {
    $.ajax({
        type: "POST",
        url: ApplicationPath + "/Account/LinkFacebookAccount",
        data: $('#linkFacebookAccountForm').serialize(),
        success: function (data) {
            $('#linkingPassword').val('');
            if (data.isError) {
                $("#linkSummaryMessage").html(data.message);
                $("#linkSummaryMessage").attr("class", "errorMessage");
            }
            else {
                window.location.href = ApplicationPath + "Play";
            }
        }
    });
}

function NavigateToUrl(url) {
    window.location.href = url;
}

function setMarketing(accountUrl) {
    $.ajax({
        type: 'POST',
        url: accountUrl + '/SetMarketing',
        data: { 'status': $('#emailMarketingStatus').attr('checked') },
        success: function (data) {
        }
    });
}