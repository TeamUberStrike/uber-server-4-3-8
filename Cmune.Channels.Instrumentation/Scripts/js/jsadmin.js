function JsonArrayToString(array) {
    var string = "";
    for (i in array) {
        string += array[i];
    }
    return string;
}

function TogglePanel(element) {
    if ($(element).is(":visible"))
        $(element).hide();
    else
        $(element).show();
}

var ajaxFeedbackDiv = "#operationFeedbackDiv";

function showSummaryError(message) {
    showSummary(message, 'ajaxFeedbackError');
}

function showSummarySuccess(message) {
    showSummary(message, 'ajaxFeedbackSuccess');
}

function showSummary(message, cssClass) {
    $(ajaxFeedbackDiv).html(message);
    $(ajaxFeedbackDiv).css('margin-left', '-' + (Math.ceil($(ajaxFeedbackDiv).width() / 2) + 50) + 'px');
    $(ajaxFeedbackDiv).addClass(cssClass);
    $(ajaxFeedbackDiv).fadeIn();

    var displayDuration = 5000;

    setTimeout(function () {
        $(ajaxFeedbackDiv).fadeOut();
    }, displayDuration);
    setTimeout(function () {
        $(ajaxFeedbackDiv).removeClass(cssClass);
    }, displayDuration + 500);
}

function OpenInShadowbox(element, title) {
    Shadowbox.open({
        content: $(element).html(),
        player: "html",
        title: title
    });
}

function openInShadowbox(data, title, onCloseCallback, onFinishCallback) {
    Shadowbox.open({
        content: data,
        player: "html",
        title: title,
        options: {
            onClose: function () {

                if (onCloseCallback !== undefined) {
                    onCloseCallback();
                }
            },
            onFinish: function () {

                if (onFinishCallback !== undefined) {
                    onFinishCallback();
                }
            }
        }
    });
}

function CopyShadowBoxDataToOriginal(object) {
    for (var i in object) {
        $(object[i]).val($('#sb-player ' + object[i]).val());
    }
}

function DisplayGreenLight() {
    $("#RedLight").hide();
    $("#GreenLight").show();
}

function DisplayRedLight() {
    $("#GreenLight").hide();
    $("#RedLight").show();
}

function roundNumber(num, dec) {
    var result = Math.round(num * Math.pow(10, dec)) / Math.pow(10, dec);
    return result;
}

function setAdmin(cmid, name) {
    setCookie("adminCmid", cmid, 365);
    setCookie("adminName", name, 365);
}

function getAdminCmid() {
    return getCookie("adminCmid");
}

function getAdminName() {
    return getCookie("adminName");
}

function setCookie(cookieName, value, expireInDays) {
    var exdate = new Date();
    exdate.setDate(exdate.getDate() + expireInDays);

    var localCookieDomainName = ".cmune.com";

    if (typeof (cookieDomainName) != 'undefined') {
        localCookieDomainName = cookieDomainName;
    }

    document.cookie = cookieName + "=" + escape(value) + ((expireInDays == null) ? "" : ";expires=" + exdate.toUTCString()) + ((localCookieDomainName == '') ? "" : ";domain=" + localCookieDomainName) + ";path=/";
}

function getCookie(cookieName) {

    if (document.cookie.length > 0) {
        cookieStart = document.cookie.indexOf(cookieName + "=");

        if (cookieStart != -1) {
            cookieStart = cookieStart + cookieName.length + 1;
            cookieEnd = document.cookie.indexOf(";", cookieStart);

            if (cookieEnd == -1) {
                cookieEnd = document.cookie.length;
            }

            return unescape(document.cookie.substring(cookieStart, cookieEnd));
        }
    }

    return "";
}

function updateDropDownList(dropDownList, endpointUrl, parameters, callback) {
    var selectedIndex = $("#" + dropDownList).val();
    $("#" + dropDownList).hide();
    $("#" + dropDownList).after(loadingImage.clone().attr('id', dropDownList + 'LoadingImg'));

    $.ajax({
        type: 'POST',
        dataType: 'json',
        url: applicationPath + endpointUrl,
        data: parameters,
        success: function (data) {

            $("#" + dropDownList).empty();

            $.each(data, function (index, item) {
                $("#" + dropDownList).append("<option value='" + item.Value + "'>" + item.Text + "</option>");
            });

            if ($('#' + dropDownList + ' option[value=' + selectedIndex + ']').length > 0) {
                $("#" + dropDownList).val(selectedIndex);
            }
            else if ($("#" + dropDownList + ' option').length > 1) {
                $('#' + dropDownList + ' option:eq(1)').attr('selected', 'selected')
            }
            else if ($("#" + dropDownList + ' option').length == 1) {
                $('#' + dropDownList + ' option:eq(0)').attr('selected', 'selected')
            }

            $("#" + dropDownList + 'LoadingImg').remove();
            $("#" + dropDownList).show();

            if (callback !== undefined) {
                callback();
            }
        },
        error: function (data) {
            $("#" + dropDownList + 'LoadingImg').remove();

            if (callback !== undefined) {
                callback();
            }
        }
    });
}

function htmlEncode(value) {
    return $('<div/>').text(value).html();
}

function htmlDecode(value) {
    return $('<div/>').html(value).text();
}