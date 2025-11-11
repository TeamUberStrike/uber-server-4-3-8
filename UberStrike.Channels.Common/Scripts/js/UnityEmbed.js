// UnityEmbed.js - Common

function GetUnityEmbedParams() {
    var params = {
        backgroundcolor: "000000",
        bordercolor: "000000",
        textcolor: "FFFFFF",
        logoimage: imagesRoot + "unityloader/_logo.png",
        progressbarimage: imagesRoot + "unityloader/_progressbar.png",
        progressframeimage: imagesRoot + "unityloader/_progressframe.png",
        disableContextMenu: true,
        autoInstall: true,
        autoHideShow: true
    };
    return params;
}

function noError() { return true; }

function UnityPlayerElement() {
    if ($("#unityPlayer").length > 0)
        return $("#unityPlayer");
    else
        throw "div #unityPlayer is missing";
}

function GetUnity() {
    if (typeof unityObject != "undefined") {
        if (document.getElementById('unityPlayer') !== "undefined")
            return unityObject.getObjectById("unityPlayer");
        else
            throw "div #unityPlayer is missing";
    }
    else
        throw "unityObject undefined";
    return null;
}

function EmbedUnity(sourceFile, width, height, sourceChannel) {
    channel = sourceChannel;

    if (typeof unityObject != "undefined") {
        unityObject.enableGoogleAnalytics = false;
        unityObject.useOwnInstallScreen = true;
        unityObject.enableAutoInstall = true;
        unityObject.enableClickOnceInstall = true;
        unityObject.enableJavaInstall = true;
        unityObject.embedUnity("unityPlayer", sourceFile, width, height, GetUnityEmbedParams(), null, unityLoaded);
    }
    else
        throw "unityObject undefined";
}

/** called by unity player **/

function unityLoaded(result) {
   
    if (result.success) {
        var unity = result.ref;
        var version = unity.GetUnityVersion("3.x.x");
        // alert("Unity Web Player loaded!\nId: " + result.id + "\nVersion: " + version);
        if ((typeof UnityEvent !== "undefined") && typeof UnityEvent.OnLoadSuccess === "function") {
            UnityEvent.OnLoadSuccess();
        }

        if ((typeof Shadowbox !== "undefined") && (typeof Shadowbox.close === "function")) {
            Shadowbox.close();
        }

        if (hadUnityBefore()) {
            wsTracking(8);
        }
        else {
            wsTracking(3);
        }
    }
    else {
        // Record no Unity
        wsTracking(1);
       
        if ((typeof UnityEvent !== "undefined") && typeof UnityEvent.OnLoadFailure === "function") {
            UnityEvent.OnLoadFailure();
        }
        if (typeof processUnityInstallFlow === "function") {
            processUnityInstallFlow();
        }
        else {
            throw "function processUnityInstallFlow is undefined";
        }
    }
}

// Called by Unity to disable scrolling
function disableWheel() {
    document.body.style.overflow = 'hidden';
}

// Called by Unity to enable scrolling
function enableWheel() {
    document.body.style.overflow = '';
}

function RefreshWallet() {
    try {
        GetUnity().SendMessage("/Managers/ApplicationDataManager", "RefreshWallet", "");
    }
    catch (error) {
        callWebService('Stats/ReportIssue?type=1&data=RefreshWallet@@@' + navigator.userAgent);
    }
}

function displayMessage(messageContent, messageUrl) {
    window.open(messageUrl);
//    if (document.getElementById('messageDiv') === 'undefined'
//        || document.getElementById('messageContentSpan') === 'undefined'
//        || document.getElementById('messageUrlSpan') === 'undefined'
//        || document.getElementById('messageLinkA') === 'undefined')
//        throw "div #messageDiv #messageContentSpan or #messageUrlSpan or #messageLinkA is missing"; 
//    document.getElementById('messageContentSpan').innerHTML = messageContent;
//    document.getElementById('messageUrlSpan').innerHTML = messageUrl;
//    document.getElementById('messageLinkA').onclick = function () { var newWindow = window.open(messageUrl); hideMessage(); return false; };

//    messageLinkUrl = messageUrl;
//    var unityPlayerObject = GetUnity();

//    if (unityPlayerObject != null) {
//        unityPlayerObject.style.height = '1px';
//    }
//    else {
//        callWebService('Stats/ReportIssue?type=1&data=displayMessageHelper@@@' + navigator.userAgent);
//    }

//    document.getElementById('messageDiv').style.display = '';
}

function hideMessage() {
    if (document.getElementById('messageDiv') === 'undefined') {
        throw "div #messageDiv is missing"; 
    }
    var unityOjectHeight = 560;
    var unityPlayerObject = GetUnity();

    if (unityPlayerObject != null) {
        unityPlayerObject.style.height = unityOjectHeight.toString() + 'px';
    }
    else {
        callWebService('Stats/ReportIssue?type=1&data=hideMessageHelper@@@' + navigator.userAgent);
    }
    document.getElementById('messageDiv').style.display = 'none';
}

/***/

function HideUnity() {
    UnityPlayerElement().height(1);
    var unityPlayerObject = GetUnity();
    if (unityPlayerObject != null) {
        unityPlayerObject.style.height = '1px';
    }
}

function ShowUnity() {
    UnityPlayerElement().height(560);
    var unityPlayerObject = GetUnity();
    if (unityPlayerObject != null) {
        unityPlayerObject.style.height = '560px';
    }
}

function IsBrowserCompatibleWithContentOverUnity() {
    var isCompatible = true;
    var userAgent = navigator.userAgent.toLowerCase();

    if (userAgent.indexOf("macintosh") != -1 || userAgent.indexOf("opera") != -1 || userAgent.indexOf("safari") != 1) {
        isCompatible = false;
    }

    return isCompatible;
}

var UnityEvent = function () {
    var OnLoadSuccess;
    var OnLoadFailure;
};