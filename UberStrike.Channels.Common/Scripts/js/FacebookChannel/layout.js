function updateBanner(xsellFormElementId) {
    // 6waves
    var sixWavesForm = document.getElementById(xsellFormElementId);

    if (sixWavesForm != null) {
        document.getElementById("xsell_form").submit();
    }

    if (document.getElementById("applifier_bar") != null) {
        (function () {
            window.applifierAsyncInit = function () {
                Applifier.init({ applicationId: applifierApplicationId, thirdPartyId: fbThirdPartyId });
                var bar = new Applifier.Bar({ barType: "bar", barContainer: "#applifier_bar" });
            };
            var a = document.createElement('script'); a.type = 'text/javascript'; a.async = true;
            a.src = (('https:' == document.location.protocol) ? 'https://secure' : 'http://cdn') + '.applifier.com/applifier.min.js';
            var s = document.getElementsByTagName('script')[0]; s.parentNode.insertBefore(a, s);
        })();
    }
}


/* MENU */

function GetUnityWidth() {
    var minW = 760;
    var width = ((window.innerWidth) ? window.innerWidth : document.documentElement.clientWidth);

    if (width && width <= minW) {
        width = minW;
    }
    return width;
}

function GetUnityHeight() {
    var minH = 590;
    var height = Math.ceil(GetUnityWidth() / parseFloat('2'));
    if (height < minH) {
        height = minH;
    }
    return height;
}


function HideUnity() {
    $("#unityPlayer").height(1);
    $("#unityPlayer").width(1);
    var unityPlayerObject = GetUnity();
    if (unityPlayerObject != null) {
        unityPlayerObject.style.height = '1px';
    }
}

function ShowUnity() {
    $("#unityPlayer").height(GetUnityHeight());
    $("#unityPlayer").width(GetUnityWidth());
    var unityPlayerObject = GetUnity();
    if (unityPlayerObject != null) {
        unityPlayerObject.style.height = GetUnityHeight() + 'px';
    }
}

function ResizeCanvas(MenuMainTab_CurrentTab) {
    var canvasSize = 300;

    switch (MenuMainTab_CurrentTab) {
        case MenuMainTab_Home:
            if (GetUnityWidth() == 760) //not in fluid mode
                canvasSize += 745;
            else
                canvasSize += 920;
            break;
        case MenuMainTab_Friend:
            canvasSize += 600;
            break;
        case MenuMainTab_Credit:
            canvasSize += 600;
            break;
        case MenuMainTab_Offer:
            canvasSize += 600;
            break;
        case MenuMainTab_Bundle:
            canvasSize += 800;
            break;
        case MenuMainTab_RedeemEPin:
            canvasSize += 600;
            break;
        case MenuMainTab_Profile:
            canvasSize += 1000;
            break;
        case MenuMainTab_Ranking:
            canvasSize += 2000;
            break;
        default:
            canvasSize += 500;
            break;
    }
    canvasSize += additionalCanvasSize;
    FacebookSetSize(canvasSize);
//    FB.Canvas.setAutoResize();
}

var canvasResizingAttempt = 5;

function IntializeCanvas(MenuMainTab_CurrentTab) {
    ResizeCanvas(MenuMainTab_CurrentTab);
    canvasResizingAttempt--;
    if (canvasResizingAttempt > 0) {
        setTimeout("IntializeCanvas(MenuMainTab_CurrentTab)", 1000);
    }
}

function ResizeUnity() {
    if (MenuMainTab_CurrentTab == MenuMainTab_Home)
        ShowUnity();
}

function FocusOnGame() {
    FB.Canvas.getPageInfo(function (info) {
        if (info.clientHeight > 840) {
            FacebookScrollTo(0, advBannerHeight + additionalCanvasSize + 32); //35 for like pluging
        }
        else {
            FacebookScrollTo(0, advBannerHeight + additionalCanvasSize + 70); //35 for like pluging
        }
    });
}

function SelectMenuTab(MenuMainTab_CurrentTab) {
    $("#uberstrikeMenu a.tab").removeClass("selectedTab");
    $('#' + MenuMainTab_CurrentTab).addClass("selectedTab");
}

function ChangePage(menuMainTab) {
    MenuMainTab_CurrentTab = menuMainTab;
    SelectMenuTab(MenuMainTab_CurrentTab);
    ResizeCanvas(MenuMainTab_CurrentTab, true);
}

function HideGameContainer() {
    HideUnity();
    $("#GamePageContainer").width(1);
}

function DisplayGameContainer() {
    $("#GamePageContainer").css("width", "");
    ShowUnity();
}

function HideAllContent() {
    HideGameContainer();
    $("#InviteFriendsPageContainer").hide();
    $("#GetCreditsPageContainer").hide();
    $("#ProfilePageContainer").hide();
    $("#BundlePageContainer").hide();
    $("#OfferPageContainer").hide();
    $("#RankingPageContainer").hide();
    $("#TestPageContainer").hide();
}

function LoadHomePage() {
    if ($("#unityPlayer").length < 1) {
        FacebookNavigate(FacebookApplicationUrl);
        return;
    }
    if (MenuMainTab_CurrentTab == MenuMainTab_Home)
        return;
    HideAllContent();
    DisplayGameContainer();
    ChangePage(MenuMainTab_Home);
    HandleBrowsingEvent();
}

function LoadInviteFriendsPage() {
    if (MenuMainTab_CurrentTab == MenuMainTab_Friend)
        return;
    ToggleTopLoader();
    $.ajax({
        type: 'POST',
        url: FriendPageUrl,
        success: function (data) {
            HideAllContent();
            $("#InviteFriendsPageContainer").html(data);
            $("#InviteFriendsPageContainer").show();
            ChangePage(MenuMainTab_Friend);
            ToggleTopLoader();
        }
    });
}

function LoadGetCreditsPage() {
    if (MenuMainTab_CurrentTab == MenuMainTab_Credit)
        return;
    ToggleTopLoader();
    $.ajax({
        type: 'POST',
        url: GetCreditsPageUrl,
        success: function (data) {
            HideAllContent();
            $("#GetCreditsPageContainer").html(data);
            $("#GetCreditsPageContainer").show();
            ChangePage(MenuMainTab_Credit);
            /* ToggleTopLoader(); done in iframe */
        }
    });
}

function LoadBundlePage() {
    if (MenuMainTab_CurrentTab == MenuMainTab_Bundle) {
        return;
    }
    ToggleTopLoader();
    $.ajax({
        type: 'POST',
        url: BundlePageUrl,
        success: function (data) {
            HideAllContent();
            $("#BundlePageContainer").html(data);
            $("#BundlePageContainer").show();
            ChangePage(MenuMainTab_Bundle);
            /*ToggleTopLoader();  done in iframe */
        }
    });
}

function LoadOfferPage() {
    if (MenuMainTab_CurrentTab == MenuMainTab_Offer) {
        return;
    }
    ToggleTopLoader();
    $.ajax({
        type: 'POST',
        url: OfferPageUrl,
        success: function (data) {
            HideAllContent();
            $("#OfferPageContainer").html(data);
            $("#OfferPageContainer").show();
            ChangePage(MenuMainTab_Offer);
            ToggleTopLoader();
        }
    });
}

function LoadProfilePage(cmid) {

    if (MenuMainTab_CurrentTab == MenuMainTab_Profile)
        return;
    ToggleTopLoader();
    $.ajax({
        type: 'POST',
        url: ProfilePageUrl,
        data: (cmid != "undefined") ? "cmid=" + cmid : "",
        success: function (data) {

            HideAllContent();
            $("#ProfilePageContainer").html(data);
            $("#ProfilePageContainer").show();
            ChangePage(MenuMainTab_Profile);
            ToggleTopLoader();
        }
    });
}

function LoadRankingPage() {
    if (MenuMainTab_CurrentTab == MenuMainTab_Ranking)
        return;
    ToggleTopLoader();
    $.ajax({
        type: 'POST',
        url: RankingPageUrl,
        success: function (data) {
            HideAllContent();
            $("#RankingPageContainer").html(data);
            $("#RankingPageContainer").show();
            ChangePage(MenuMainTab_Ranking);
            ToggleTopLoader();
        }
    });
}

function LoadRedeemEpinPage() {
    if (MenuMainTab_CurrentTab == MenuMainTab_RedeemEPin)
        return;
    ToggleTopLoader();
    $.ajax({
        type: 'POST',
        url: RedeemEpinPageUrl,
        success: function (data) {
            HideAllContent();
            $("#OfferPageContainer").html(data);
            $("#OfferPageContainer").show();
            ChangePage(MenuMainTab_RedeemEPin);
            ToggleTopLoader();
        }
    });
}

function LoadTestPage() {
    if (MenuMainTab_CurrentTab == MenuMainTab_Test)
        return;
    ToggleTopLoader();
    $.ajax({
        type: 'POST',
        url: TestPageUrl,
        success: function (data) {
            HideAllContent();
            $("#TestPageContainer").html(data);
            $("#TestPageContainer").show();
            ChangePage(MenuMainTab_Test);
            ToggleTopLoader();
        }
    });
}

function ConfirmationFading(text, callback) {
    $("#confirmationFading").html(text);
    $("#confirmationFading").show();
    setTimeout(function () {
        $("#confirmationFading").fadeOut('slow', callback);
    }, 1000);
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
                FacebookNavigate(FacebookApplicationUrl);
            }
        }
    });
}

var ItemBundleIframeInitialHeight = 0;
function ResizeGetItemBundlePage(height) {
    ItemBundleIframeInitialHeight = document.getElementById("itemBundleIframe").height;
    document.getElementById("itemBundleIframe").height = height;
}

function ResetGetItemBundlePage() {
    document.getElementById("itemBundleIframe").height = ItemBundleIframeInitialHeight;
}