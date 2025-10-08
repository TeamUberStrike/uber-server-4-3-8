function clearFormElements(ele) {
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
    $(summaryParentId + " .sucessMessage").hide();
}

function DisplaySuccessSummary(summaryParentId, message) {
    $(summaryParentId + " .sucessMessage").html(message);
    $(summaryParentId + " .sucessMessage").show();
    $(summaryParentId + " .errorMessage").hide();
}