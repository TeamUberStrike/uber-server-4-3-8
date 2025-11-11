// layout.js

function SetStep(currentStep) {
    $("div.worflowTree .step").removeClass("selected");
    $('#Step' + currentStep).addClass("selected");
}

function ToggleTopLoader() {
    if ($("#toploader").is(':hidden')) {
        $("#toploader").show();
    }
    else {
        $("#toploader").hide();
    }
}

function facebooklogin(ch, hash) {
    FB.login(function (response) {
        if (response.authResponse) {
            // user authorized
            window.location.href = applicationUrl + "/Account/ConnectWithFacebookGo?channel=" + ch + "&h=" + hash + "&facebookLogin=1&returnUrl=" + $("#ReturnUrl").val();
        } else {
            // user cancelled
        }
    }, { scope: 'publish_stream,email,publish_actions' });
};

function facebooklogout() {
    FB.logout(function (response) { window.location.href = logOffUrl; });
}

