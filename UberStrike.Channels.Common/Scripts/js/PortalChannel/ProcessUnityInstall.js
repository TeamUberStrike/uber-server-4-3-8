// ProcessUnityInstall.js - Portal

var unityInstallFlowEnUs = {
    "windowTitle": "Setup UberStrike",
    "continueStep1Button": "Click <strong>Continue</strong> to run UberStrike.",
    "continueStep1Sentence": "Accepting will improve the game and give you kick-ass 3D graphics!",
    "javaInstall": "After you click <strong>Continue</strong> please choose <strong>Run</strong> in the next window.",
    "firefoxInstall": "After you click <strong>Continue</strong> please choose <strong>Save File</strong> in the next window (as indicated below).",
    "internetExplorerInstall": "After you click <strong>Continue</strong> please choose <strong>Run</strong> in the next window (as indicated below).",
    "chromeInstall": "After you click <strong>Continue</strong> please choose <strong>Save</strong> from pop-up at the bottom of your screen (as indicated below).",
    "safariInstallWindows": "After you click <strong>Continue</strong> please choose <strong>Run</strong> in the next window (as indicated below).",
    "safariInstallMac": "Click <strong>Continue</strong> to start the download of <i>Unity Player</i>...",
    "supportLink": "Need Help? Contact Support",
    "continueStep2Button": "<strong>Continue</strong>",
    "cancelStep2Button": "Cancel",
    "settingUp": "Setting up...",
    "automaticStart": "The game will automatically start after the download has completed.",
    "doubleClickWindowsFirefox": "Once the download has finished <strong>Double Click</strong> to continue...",
    "doubleClickMacFirefox": "Click <strong>Save</strong>, then once downloaded <strong>Double Click</strong> the download to continue...",
    "noPopUpFirefox": "Don't see the pop-up?",
    "restartDownloadFirefox": "Click here to restart the download",
    "finishedDownloadChrome": "Once the download has finished <strong>Click</strong> to continue...",
    "automaticStartWindowsSafari": "The game will automatically start after the download has completed.",
    "downloadStartMacSafari": "The game will automatically start after the download has completed.",
    "runWindows": "If prompted, click <strong>Run</strong> to start the game."
};

var unityInstallFlowKoKr = {
    "windowTitle": "Unity3D Web Player 설치",
    "continueStep1Button": '<strong>"계속하기"</strong> 버튼을 클릭해 주세요!',
    "continueStep1Sentence": "세계 최초로 웹에서 즐기는 FPS! 위버스트라이크!",
    "javaInstall": '<strong>"계속하기"</strong> 버튼을 클릭하시고 설치를 진행해 주세요!',
    "firefoxInstall": '<strong>"계속하기"</strong> 버튼을 클릭하시고 설치를 진행해 주세요!',
    "internetExplorerInstall": '<strong>"계속하기"</strong> 버튼을 클릭하시고 설치를 진행해 주세요!',
    "chromeInstall": '<strong>"계속하기"</strong> 버튼을 클릭하시고 설치를 진행해 주세요!',
    "safariInstallWindows": '<strong>"계속하기"</strong> 버튼을 클릭하시고 설치를 진행해 주세요!',
    "safariInstallMac": '<strong>"계속하기"</strong> 버튼을 클릭하시고 설치를 진행해 주세요!',
    "supportLink": "문의하기",
    "continueStep2Button": "<strong>계속하기</strong>",
    "cancelStep2Button": "취소",
    "settingUp": "세팅중…",
    "automaticStart": "엔진 설치가 완료되면 자동으로 게임이 실행됩니다.",
    "doubleClickWindowsFirefox": "다운로드가 완료되면 <strong>더블 클릭</strong>해 실행시켜 주세요!",
    "doubleClickMacFirefox": "<strong>저장</strong>을 선택하시고, 다운받은 파일을 <strong>더블 클릭</strong>해 계속 진행해 주세요!",
    "noPopUpFirefox": "팝업이 안보이시나요?",
    "restartDownloadFirefox": "여기를 클릭하시면 다시 다운로드됩니다!",
    "finishedDownloadChrome": "다운로드가 완료되면 파일을 <strong>실행</strong>해 주세요~!",
    "automaticStartWindowsSafari": "엔진 설치가 완료되면 자동으로 게임이 실행됩니다.",
    "downloadStartMacSafari": "엔진 설치가 완료되면 자동으로 게임이 실행됩니다.",
    "runWindows": '설치 창이 뜨면 <strong>"Run"</strong> 버튼을 클릭해 주세요!'
};

var unityInstallFlowLang = {
    "en-US": unityInstallFlowEnUs,
    "ko_kr": unityInstallFlowKoKr
};

var unityInstallFlow = unityInstallFlowEnUs;

if (typeof locale !== 'undefined' && unityInstallFlowLang[locale] !== undefined) {
    unityInstallFlow = unityInstallFlowLang[locale];
}


//generate html content
function processUnityInstallFlow() {
    var newHTML = '';

    // Container
    newHTML += '<div id="installflow" class="install_container">';

    // Box Header
    newHTML += '<div style="height:27px; background:#cc6600; vertical-align:middle; text-align:center; font-size:16px; line-height:27px; color:#fff"><strong>' + unityInstallFlow['windowTitle'] + '</strong></div>';

    newHTML += '<div style="float:left; position:relative; max-width:400px; height:182px">';

    // Logo
    newHTML += '<img src="' + imagesRoot + 'unityinstall/logo.jpg" alt="UberStrike" style="padding:7px 0px 0px 7px" />';

    newHTML += '<p style="font-size:16px; padding-left:10px">' + unityInstallFlow['continueStep1Button'] + '</p>';
    newHTML += '<p style="font-size:11px; padding-left:10px">' + unityInstallFlow['continueStep1Sentence'] + '</p>';

    newHTML += '</div>';

    // Flash Video
    newHTML += '<div style="float:right">';
    newHTML += '<object classid="clsid:d27cdb6e-ae6d-11cf-96b8-444553540000" codebase="http://download.macromedia.com/pub/shockwave/cabs/flash/swflash.cab#version=10,0,0,0" width="316" height="182" id="UberStrike_Preview">';
    newHTML += '<param name="movie" value="' + commonVideoRoot + 'preview.swf" />';
    if (!unityObject.ua.ie)
        newHTML += '<object type="application/x-shockwave-flash" data="' + commonVideoRoot + 'preview.swf" width="316" height="182" align="left"><param name="movie" value="' + commonVideoRoot + 'preview.swf" allowScriptAccess="always" allowFullScreen="false" background="#000000" />';
    if (!unityObject.ua.ie)
        newHTML += '</object>';
    newHTML += '</object>';
    newHTML += '</div>';

    var os;

    if (unityObject.ua.win)
        os = "vista"
    else if (unityObject.ua.mac)
        os = "mac"

    newHTML += '<div style="clear:left; text-align:center; height:282px;">';

    if (unityObject.ua.java) {
        newHTML += '<p>' + unityInstallFlow['javaInstall'] + '</p>';

        if (channel != 6) {
            newHTML += '<img src="' + imagesRoot + 'unityinstall/java-' + os + '.jpg" alt="Install Info" title="Install Info" /><br />';
        }
    }
    else if (unityObject.ua.ff) {
        if (os == 'vista')
            newHTML += '<p></p>';
        else
            newHTML += '<p>' + unityInstallFlow['firefoxInstall'] + '</p>';

        if (channel != 6) {
            newHTML += '<img src="' + imagesRoot + 'unityinstall/ff-' + os + '.jpg" alt="Install Info" title="Install Info" /><br />';
        }
    }
    else if (unityObject.ua.ie) {
        window.onerror = noError;
        newHTML += '<p>' + unityInstallFlow['internetExplorerInstall'] + '</p>';

        if (channel != 6) {
            newHTML += '<img src="' + imagesRoot + 'unityinstall/ie-' + os + '.jpg" alt="Install Info" title="Install Info" />';
        }
    }
    else if (unityObject.ua.ch) {
        newHTML += '<p style="padding-top:50px">' + unityInstallFlow['chromeInstall'] + '</p>';

        if (channel != 6) {
            newHTML += '<img src="' + imagesRoot + 'unityinstall/ch.jpg" alt="Install Info" title="Install Info" /><br /><br />';
        }
    }
    else if (unityObject.ua.wk) {
        if (os == 'vista') {
            newHTML += '<p style="padding-top:30px">' + unityInstallFlow['safariInstallWindows'] + '</p>';

            if (channel != 6) {
                newHTML += '<img src="' + imagesRoot + 'unityinstall/saf-vista.jpg" alt="Install Info" title="Install Info" />';
            }
        }
        else {
            newHTML += '<p>' + unityInstallFlow['safariInstallMac'] + '</p>';
        }
    }

    newHTML += '</div>';

    newHTML += '<div class="install_bottom">';
    newHTML += '<a href="';

    if (channel != 6) {
        newHTML += 'http://www.cmune.com/index.php/contact/';
    }
    else {
        newHTML += 'http://club.cyworld.com/clubV1/Home.cy/54629955';
    }

    newHTML += '" target="_blank" ';
    newHTML += 'style="font-size:11px; float:left; padding:6px 0 0 5px">' + unityInstallFlow['supportLink'] + '</a>';

    newHTML += '<a href="' + unityObject.ua.installUrl + '" onclick="setTimeout(\'wsTracking(2)\', 800); setTimeout(\'nextInstallPage()\', 400); return true;" class="darkGray" style="float:right; font-size: 15px; padding:3px 8px 3px 8px">' + unityInstallFlow['continueStep2Button'] + '</a>';

    newHTML += '<a href=';
    if (channel == 1)
        newHTML += '"http://www.facebook.com/apps/application.php?id=155171167829381&v=info" target="_top"';
    else if (channel == 6) {
        newHTML += '"http://appstore.nate.com/" target="_blank"';
    }
    else
        newHTML += '"' + appBaseUrl + '"';
    newHTML += ' class="darkGray" onclick="wsTracking(5); return true;" style="float:right; font-size: 15px; padding:3px 8px 3px 8px">' + unityInstallFlow['cancelStep2Button'] + '</a>';

    newHTML += '</div></div></div>';

    replaceHTML("unityPlayer", newHTML);
}

function nextInstallPage() {

    var newHTML = '';

    // Container
    newHTML += '<div class="install_container">';

    newHTML += '<center><div style="font-size:large; clear:left; padding-top:30px">' + unityInstallFlow['settingUp'] + '</div>';

    if (channel != 6) {
        newHTML += '<img src="' + imagesRoot + 'unityinstall/logo.jpg" alt="UberStrike" title="Uberstrike" />';
    }

    if (unityObject.ua.win)
        os = "vista"
    else if (unityObject.ua.mac)
        os = "mac"

    if (unityObject.ua.ie) {
        newHTML += '<p>' + unityInstallFlow['automaticStart'] + '</p>';
    }
    else if (unityObject.ua.ff) {
        if (unityObject.ua.win)
            newHTML += '<p style="font-size:17px">' + unityInstallFlow['doubleClickWindowsFirefox'] + '</p>';
        else
            newHTML += '<p style="font-size:17px">' + unityInstallFlow['doubleClickMacFirefox'] + '</p>';

        if (channel != 6) {
            newHTML += '<img src="' + imagesRoot + 'unityinstall/ff-' + os + '-2.jpg" alt="Install" title="Install" />';
        }

        newHTML += '<p>' + unityInstallFlow['noPopUpFirefox'] + '</p>';
        newHTML += '<a href="' + unityObject.ua.installUrl + '" onclick="nextInstallPage(); return true;">' + unityInstallFlow['restartDownloadFirefox'] + '</a>';
    }
    else if (unityObject.ua.ch) {
        newHTML += '<p style="font-size:17px">' + unityInstallFlow['finishedDownloadChrome'] + '</p>';

        if (channel != 6) {
            newHTML += '<img src="' + imagesRoot + 'unityinstall/ch-2.jpg" alt="Install Flow" title="Install Flow" />';
        }
    }
    else if (unityObject.ua.wk) {
        if (os == 'vista') {
            newHTML += '<p>' + unityInstallFlow['automaticStartWindowsSafari'] + '</p>';
        }
        else {
            newHTML += '<p>' + unityInstallFlow['downloadStartMacSafari'] + '</p>';

            if (channel != 6) {
                newHTML += '<img src="' + imagesRoot + 'unityinstall/saf-mac.jpg" alt="Install Info" title="Install Info" />';
            }
        }
    }

    if (unityObject.ua.win)
        newHTML += '<p>' + unityInstallFlow['runWindows'] + '</p>';

    newHTML += '</center></div>';

    replaceHTML("unityPlayer", newHTML);
}

function replaceHTML(divID, setText) {
    // Prevent IE from complaining about its own inability to handle WC3 standards
    if (unityObject.ua.ie)
        window.onerror = noError;

    var container = document.getElementById(divID), child;
    while (child = container.firstChild)
        container.removeChild(child);

    var newdiv = document.createElement("div");
    newdiv.innerHTML = setText;
    container.appendChild(newdiv);
}