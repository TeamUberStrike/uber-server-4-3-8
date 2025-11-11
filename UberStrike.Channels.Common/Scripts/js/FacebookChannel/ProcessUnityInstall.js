// ProcessUnityInstall.js - Facebook

/*
* UnityEmbed 0.1
*
* Embed customized Unity Webplayer into the page
* Enable/Disable page scrolling
*
*/

var unityInstallFlowEnUs = {
    "windowTitle": "Setup UberStrike",
    "welcomeSentence": "Welcome to UberStrike! Facebook's largest 3D First Person Shooter",
    "sentence2": "The installation won't be long, we promise!",
    "continueInstall": "Click <strong>Continue</strong> to setup Unity3D",
    "continueInstallError": "If your download didn't start please ",
    "clickHere": "click here",
    "or": "or",
    "openDownloadPage": "open the download page",
    "supportLink": "Need Help? <a target=\"_blank\" href=\"http://support.uberstrike.com/\">Support center</a>"
};

var unityInstallFlowKoKr = {
    "windowTitle": "Setup UberStrike",
    "welcomeSentence": "Welcome to UberStrike! Facebook's largest 3D First Person Shooter",
    "sentence2": "The installation won't be long, we promise!",
    "continueInstall": "Click <strong>Continue</strong> to setup Unity 3D Web Player",
    "continueInstallError": "If your download didn't start please ",
    "clickHere": "click here",
    "or": "or",
    "openDownloadPage": "open the download page",
    "supportLink": "Need Help? <a target=\"_blank\" href=\"http://support.uberstrike.com/\">Support center</a>"
};

var unityInstallFlowLang = {
    "en-US": unityInstallFlowEnUs,
    "ko_kr": unityInstallFlowKoKr
};

var unityInstallFlow = unityInstallFlowEnUs;

if (typeof locale !== 'undefined' && unityInstallFlowLang[locale] !== undefined) {
    unityInstallFlow = unityInstallFlowLang[locale];
}

function IsOneClickPluginEnabled() {
    return (unityObject.ua.java) || (unityObject.ua.co);
}

function IsMacPlateform() {
    return unityObject.ua.installNav.platform == "MacIntel" || unityObject.ua.installNav.platform == "MacPPC";
}

function IsAutoDownloadWebBrowser() {
    return (unityObject.ua.ff || unityObject.ua.ch);
}

function GenerateInstallPluginPopUpButton(name) {
    var htmlcontent = '';
 
    htmlcontent = '<a class="facebookButton" href="' + unityObject.ua.installUrl + '" onclick="' + unityObject.ua.installOnclick + ';setTimeout(\'wsTracking(2)\', 800); return true;">' + name + '</a>';
    return htmlcontent;
}

function GetInstallPluginPopUpUpperBody() {
    var dialogBody = '';
    dialogBody += unityInstallFlow['continueInstall'];
    return dialogBody;
}

function GetUnityDownloadLink() {
    if (unityObject.ua.win) {
        return "http://webplayer.unity3d.com/download_webplayer-3.x/UnityWebPlayer.exe";
    }
    else if (unityObject.ua.mac) {
        return "http://webplayer.unity3d.com/download_webplayer-3.x/webplayer-mini.dmg";
    }
    else
        return "";
}

function GetInstallPluginPopUpLowerBody() {
    var dialogBody = '';
    if (IsOneClickPluginEnabled() || (unityObject.ua.win || IsMacPlateform())) {
        dialogBody += unityInstallFlow["continueInstallError"];
        dialogBody += ' <a href="' + GetUnityDownloadLink() + '">' + unityInstallFlow["clickHere"] + '</a>';
    }
    
    return dialogBody;
}

function GetInstallPluginPopUpHeight() { // the size depends of the content returned by GetInstallPluginPopUpImage() function
    var initialHeight = 220;
    if (IsOneClickPluginEnabled()) {
        initialHeight += 50;
    }
    else if (unityObject.ua.win || IsMacPlateform()) {
        initialHeight += 50;
    }
    else {
        initialHeight += 50;
    }
    initialHeight += 250;
    return initialHeight;
}

function loadUnityInstall(data) {
    window.setTimeout(function(){
    Shadowbox.open({
        content: data,
        player: "html",
        height: GetInstallPluginPopUpHeight()
    })}, 1000);
}

function BuildDialogButton() {
    var dialogButtonHTML = '';
    dialogButtonHTML += '<a href="';
    dialogButtonHTML += 'http://unity3d.com/';
    dialogButtonHTML += '" target="_blank" ';
    dialogButtonHTML += '><img src="' + imageUnityLogo + '" height="23px" /></a>';

    dialogButtonHTML += '<a href="';
    dialogButtonHTML += 'http://www.cmune.com/';
    dialogButtonHTML += '" target="_blank" ';
    dialogButtonHTML += '><img src="' + imageCmuneLogo + '" height="23px" /></a>';

    return dialogButtonHTML;
}

function BuildDialog() {
    var dialogBody = '<div class="install_step1_lower"><br/>';
    dialogBody += '<p>' + unityInstallFlow['welcomeSentence'] + '</p>';
    dialogBody += '<p>';
    dialogBody += GetInstallPluginPopUpUpperBody();
    dialogBody += '</p>';
    dialogBody += '<p>';
    dialogBody += GenerateInstallPluginPopUpButton("Continue");
    dialogBody += '</p>';
    dialogBody += '<p>';
    dialogBody += GetInstallPluginPopUpLowerBody();
    dialogBody += '</p>';
    dialogBody += '<p><a class="macAfeeButton" href="http://www.siteadvisor.com/sites/unity3d.com" target="_blank"></a></p>';
    dialogBody += '<p><div class="installPluginMonitor"><div style="height:187px; overflow:hidden">';
    dialogBody += '<object width="320" height="212" type="application/x-shockwave-flash" data="https://www.youtube.com/v/4pryvDbzKbY&amp;loop=1&amp;modestbranding=1&amp;autoplay=1" wmode="transparent">';
    dialogBody += '<param name="movie" value="https://www.youtube.com/v/4pryvDbzKbY&amp;loop=1&amp;modestbranding=1&amp;autoplay=1">';
	dialogBody += '<param name="wmode" value="transparent">';
    dialogBody += '</object>';
    dialogBody += '</div></div></p>';
    dialogBody += '</div>';
    return dialogBody;
}

//generate html content
function processUnityInstallFlow() {
    var newHTML = '';

    // Container
    newHTML += '<h2 class="dlgHeader">';
    newHTML += '<p>' + unityInstallFlow["windowTitle"] + '</p>';
    newHTML += '</h2>';
    newHTML += '<div class="dlgContent">';
    newHTML += '<div class="dlgBody">';
    newHTML += BuildDialog();
    newHTML += '</div>';
    newHTML += '<div class="dlgButtons">';
    newHTML += '<div class="left">' + unityInstallFlow["supportLink"];
    newHTML += '</div>';
    newHTML += BuildDialogButton();
    newHTML += '</div>';
    newHTML += '</div>';
    
    loadUnityInstall(newHTML);
}