function GetUnityWidth() {
    var minW = 760;
    var width = ((screen.width) - 600);
    if (width && width <= minW) {
        width = minW;
    }
    return width;
}

function GetUnityHeight() {
    var minHeight = 590;
    var height = Math.ceil(GetUnityWidth() / parseFloat('1.8'));
    if (height < minHeight) {
        height = minHeight;
    }
    return height;
}

function GetShellWidth() {
    return GetUnityWidth();
}

function GetShellHeight() {
    return GetUnityHeight() + 25;
}

function GetGameFrameWidth() {
    return GetUnityWidth();
}

function GetGameFrameHeight() {
    return GetUnityHeight() + 25;
}

function GetKongregateLoginData() {
    var params = "kongregate_user_id=" + parent.KongregateUserId
    + "&kongregate_username=" + parent.KongregateUserName
    + "&kongregate_game_auth_token=" + parent.KongregateGameAuthToken;

    return params;
}

function ResizeShell(shellWidth, shellHeight) {
    $("#contentdiv").width(shellWidth);
    $("#contentdiv").height(shellHeight);
    $("#content").width(shellWidth);
    $("#content").height(shellHeight);
}

function HideUnity() {
    if ($("#unityPlayer") != null) {
        $("#unityPlayer").height(1);
        $("#unityPlayer").width(1);
        var unityPlayerObject = GetUnity();
        if (unityPlayerObject != null) {
            unityPlayerObject.style.height = '1px';
        }
        return true;
    }
    return false;
}

function ShowUnity() {
    if ($("#unityPlayer") != null) {
        $("#unityPlayer").height(GetUnityHeight());
        $("#unityPlayer").width(GetUnityWidth());
        var unityPlayerObject = GetUnity();
        if (unityPlayerObject != null) {
            unityPlayerObject.style.height = GetUnityHeight() + 'px';
        }
        return true;
    }
    return false;
}

function HideMainContainer() {
    if (HideUnity() == true)
        $("#MainPageContainer").width(1);
    else
        $("#MainPageContainer").hide();
}

function ShowMainContainer() {
    ShowUnity();
    $("#GamePageContainer").css("width", "");

}

function HideAllContent() {
    HideMainContainer();
    $("#OtherPageContainer").hide();
}

function LoadPlayPage() {
    if (MainMenuTab_CurrentTab == MainMenuTab_Play)
        return;
    HideAllContent();
    ShowMainContainer();
    ChangePage(MainMenuTab_Play);
}

function LoadCompleteEmailPage() {
    if (MainMenuTab_CurrentTab == MainMenuTab_CompleteEmail)
        return;
    $.ajax({
        type: 'POST',
        url: ApplicationUrl + '/Account/AccountCompletion' + '?' + GetKongregateLoginData(),
        success: function (data) {
            HideAllContent();
            $("#OtherPageContainer").html(data);
            $("#OtherPageContainer").show();
            ChangePage(MainMenuTab_CompleteEmail);
            /* ToggleTopLoader(); done in iframe */
        }
    });
}

function LoadItemBundlePage() {
    if (MainMenuTab_CurrentTab == MainMenuTab_ItemBundles)
        return;
    $.ajax({
        type: 'POST',
        url: ApplicationUrl + '/Offer/ItemBundles' + '?' + GetKongregateLoginData(),
        success: function (data) {
            HideAllContent();
            $("#OtherPageContainer").html(data);
            $("#OtherPageContainer").show();
            ChangePage(MainMenuTab_ItemBundles);
            /*ToggleTopLoader();  done in iframe */
        }
    });
}

function LoadCreditBundlePage() {
    if (MainMenuTab_CurrentTab == MainMenuTab_CreditBundles)
        return;
    $.ajax({
        type: 'POST',
        url: ApplicationUrl + '/Offer' + '?' + GetKongregateLoginData(),
        success: function (data) {
            HideAllContent();
            $("#OtherPageContainer").html(data);
            $("#OtherPageContainer").show();
            ChangePage(MainMenuTab_CreditBundles);
        }
    });
}


function LoadProfilePage() {
    if (MainMenuTab_CurrentTab == MainMenuTab_Profile)
        return;
    $.ajax({
        type: 'POST',
        url: ApplicationUrl + "/Profile" + '?' + GetKongregateLoginData(),
        success: function (data) {
            HideAllContent();
            $("#OtherPageContainer").html(data);
            $("#OtherPageContainer").show();
            ChangePage(MainMenuTab_Profile);
        }
    });
}

function LoadRankingPage() {
    if (MainMenuTab_CurrentTab == MainMenuTab_Ranking)
        return;
    $.ajax({
        type: 'POST',
        url: ApplicationUrl + "/Ranking" + '?' + GetKongregateLoginData(),
        success: function (data) {
            HideAllContent();
            $("#OtherPageContainer").html(data);
            $("#OtherPageContainer").show();
            ChangePage(MainMenuTab_Ranking);
        }
    });
}

function ChangePage(menuMainTab) {
    MainMenuTab_CurrentTab = menuMainTab;
    SelectMenuTab(MainMenuTab_CurrentTab);
    //ResizeCanvas(MenuMainTab_CurrentTab, true);
}

function SelectMenuTab(MainMenuTab_CurrentTab) {
    $("#UberstrikeMenu a.tab").removeClass("selectedTab");
    $('#' + MainMenuTab_CurrentTab).addClass("selectedTab");
}
