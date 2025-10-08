// UberStrike.js

function ClearForm(ele) {
    $(ele).find(':input').each(function () {
        switch (this.type) {
            case 'password':
            case 'select-multiple':
            case 'select-one':
            case 'text':
            case 'textarea':
                $(this).val('');
                break;
            case 'checkbox':
            case 'radio':
                this.checked = false;
        }
    });
}

function DisplayErrorSummary(summaryParentId, message) {
    $(summaryParentId + " .errorMessage").html(message);
    $(summaryParentId + " .errorMessage").show();
    $(summaryParentId + " .successMessage").hide();
}

function DisplaySuccessSummary(summaryParentId, message) {
    $(summaryParentId + " .successMessage").html(message);
    $(summaryParentId + " .successMessage").show();
    $(summaryParentId + " .errorMessage").hide();
}

function ToggleSummary(summaryId, success, message) {
    if (success) {
        $(summaryId).attr("class", "");
        $(summaryId).attr("class", "successMessage");
    }
    else {
        $(summaryId).attr("class", "");
        $(summaryId).attr("class", "errorMessage");
    }
    $(summaryId).html(message);
    $(summaryId).show();
    $(summaryId).fadeOut(5000);
}

function StartServerClock() {
    var date = new Date();
    setServerTime(date);
    updateClock('clock');
    setInterval('updateClock(\'clock\')', 1000);
}

function ToggleTopLoader() {
    if ($("#toploader").is(':hidden')) {
        $("#toploader").show();
    }
    else {
        $("#toploader").hide();
    }
}