// Tracking.js

var InstallStepType = {
    InvalidWsCall: 0,
    NoUnity: 1,
    ClickDownload: 2,
    UnityInstalled: 3,
    FullGameLoaded: 4,
    ClickCancel: 5,
    UnityInitialized: 6,
    AccountCreated: 7,
    HasUnity: 8
};

function installationConversionTracking(step) {
    // If we reach the end of tracking we should clear the cookies (for the moment 4 - FullGameLoaded - or 5 - ClickCancel)
    var track_id = 0;
    var track_java = "0";
    var track_referrerid = 0;
    var bCallWs = true;

    if (typeof (fbRefPartnerId) !== 'undefined')
        track_referrerid = fbRefPartnerId;

    if (unityObject.ua.java)
        track_java = "1";

    // Get/Set local tracking cookie..
    track_id = getCookie("uberstrike_install")

    if (track_id == "") {
        track_id = cmid;
        setCookie("uberstrike_install", track_id, null);
    }

    // Only call the webservice if the first install tracking step was previously recorded (ie steps 3, 4 & 6 are always called even if Unity was previously installed)
    if (step != InstallStepType.NoUnity && step != InstallStepType.HasUnity) {
        if (getCookie("uberstrike_install_step") == "" || typeof (getCookie("uberstrike_install_step")) === 'undefined')
            bCallWs = false;
    }

    if (bCallWs) {
        wsC = 'stepId=' + step + '&channel=' + webChannel + '&referrerPartnerId=' + track_referrerid + '&isJavaInstall=' + track_java + '&operatingSystem=' + BrowserDetect.OS + '&tracking=' + track_id + '&browsername=' + BrowserDetect.browser + '&browserversion=' + BrowserDetect.version;

        $.ajax({
            url: webServicesBaseURL + 'Stats/SetInstallTracking',
            data: wsC,
            dataType: 'jsonp',
            jsonp: 'callback',
            jsonpCallback: 'jsonpCallback'
        });

        // Lets track the install progress locally too
        setCookie("uberstrike_install_step", step, null);
    }

    // If that was the last step (full game loaded or clicked cancel) lets clean up cookies
    if (step == 4 || step == 5) {
        setCookie("uberstrike_install", "", 0);
        setCookie("uberstrike_install_step", "", 0);
    }
}

function jsonpCallback(data) {
}

function hadUnityBefore() {
    currentStep = getCookie("uberstrike_install_step");

    var hasUnity = false;

    if (currentStep == "") {
        hasUnity = true;
    }
    else if (currentStep == InstallStepType.NoUnity || currentStep == InstallStepType.ClickDownload || currentStep == InstallStepType.UnityInstalled || currentStep == InstallStepType.ClickCancel) {
        hasUnity = false;
    }
    else if (currentStep == InstallStepType.HasUnity) {
        hasUnity = true;
    }

    return hasUnity;
}

function shouldCallTracking(currentStep) {

    var previousStep = getCookie("uberstrike_install_step");

    if (previousStep == "" && (currentStep == InstallStepType.NoUnity || currentStep == InstallStepType.HasUnity)) {
        return true;
    }

    if (previousStep == InstallStepType.NoUnity && currentStep == InstallStepType.ClickDownload) {
        return true;
    }

    if ((previousStep == InstallStepType.ClickDownload || previousStep == InstallStepType.UnityInstalled) && currentStep == InstallStepType.UnityInstalled) {
        return true;
    }

    if (currentStep == InstallStepType.ClickCancel || currentStep == InstallStepType.FullGameLoaded || currentStep == InstallStepType.UnityInitialized || currentStep == InstallStepType.AccountCreated) {
        return true;
    }

    return false;
}

function setCookie(c_name, value, expiredays) {
    var exdate = new Date();
    exdate.setDate(exdate.getDate() + expiredays);

    var localCookieDomainName = ".cmune.com";

    if (typeof (cookieDomainName) != 'undefined') {
        localCookieDomainName = cookieDomainName;
    }
    document.cookie = c_name + "=" + escape(value) + ((expiredays == null) ? "" : ";expires=" + exdate.toUTCString()) + ((localCookieDomainName == '') ? "" : ";domain=" + localCookieDomainName) + ";path=/";
}

function getCookie(c_name) {
    if (document.cookie.length > 0) {
        c_start = document.cookie.indexOf(c_name + "=");
        if (c_start != -1) {
            c_start = c_start + c_name.length + 1;
            c_end = document.cookie.indexOf(";", c_start);
            if (c_end == -1) c_end = document.cookie.length;
            return unescape(document.cookie.substring(c_start, c_end));
        }
    }
    return "";
}

function wsTracking(step) {
    if (typeof IsKongregateStatisticEnabled != "undefined" && IsKongregateStatisticEnabled === true) {
        if (step == InstallStepType.UnityInitialized) {
            kongregate.stats.submit("initialized", 1);
        }
    }
    if (IsTrackingEnabled === "undefined")
        throw "var IsTrackingEnabled undefined";
    if (IsTrackingEnabled && shouldCallTracking(step)) {
        installationConversionTracking(step);
    }
}