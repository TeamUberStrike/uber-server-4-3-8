/* Email sending */

function sendSingleEmail() {

    $("#sendSingleEmailButton").hide();
    $("#sendSingleEmailButton").before(loadingImage.clone().attr('id', 'sendSingleEmailLoader'));

    $.ajax({
        type: 'POST',
        url: applicationPath + 'Utility/SendSingleEmail',
        data: $("#sendSingleEmailForm").serialize(),
        success: function (data) {
            $('#sendSingleEmailLoader').remove();
            $("#sendSingleEmailButton").show();

            if (data.IsSent) {
                showSummarySuccess('Go check your inbox');
            }
            else {
                showSummaryError(data.Message);
            }
        },
        error: function (data) {
            $('#sendSingleEmailLoader').remove();
            $("#sendSingleEmailButton").show();
            showSummaryError('An error happened');
        }
    });
}

function sendMultipleEmails() {

    $("#sendMultipleEmailsButton").hide();
    $("#sendMultipleEmailsButton").before(loadingImage.clone().attr('id', 'sendMultipleEmailsLoader'));

    $.ajax({
        type: 'POST',
        url: applicationPath + 'Utility/SendMultipleEmails',
        data: $("#sendMultipleEmailsForm").serialize(),
        success: function (data) {
            $('#sendMultipleEmailsLoader').remove();
            $("#sendMultipleEmailsButton").show();

            if (data.IsSent) {
                showSummarySuccess('Go check your inboxes');
            }
            else {
                showSummaryError(data.Message);
            }
        },
        error: function (data) {
            $('#sendMultipleEmailsLoader').remove();
            $("#sendMultipleEmailsButton").show();
            showSummaryError('An error happened');
        }
    });
}

/* End Email sending */