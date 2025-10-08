/*** Photons health Page ***/

function getPhotonsHealth(version) {

    $("#photonsHealthContainer").hide();
    $("#photonsHealthContainer").before(loadingImage.clone().attr('id', 'photonsHealthLoader'));

    $.ajax({
        type: 'GET',
        url: applicationPath + 'Monitoring/GetPhotonsHealthCharts',
        data: { 'version': version },
        success: function (data) {
            $('#photonsHealthContainer').html(data);
            $('#photonsHealthLoader').remove();
            $('#photonsHealthContainer').show();
        },
        error: function (data) {
            $('#photonsHealthLoader').remove();
            $('#photonsHealthContainer').show();
        }
    });

    return false;
}

/*** Unity exceptions Page ***/

/*******/

function getExceptionGroups() {

    $("#unityExceptionsContainer").hide();
    $("#unityExceptionsContainer").before(loadingImage.clone().attr('id', 'unityExceptionsLoader'));

    $.ajax({
        type: 'POST',
        url: applicationPath + 'Monitoring/GetExceptionGroups',
        success: function (data) {
            $('#unityExceptionsContainer').html(data);
            $('#unityExceptionsLoader').remove();
            $('#unityExceptionsContainer').show();
        },
        error: function (data) {
            $('#unityExceptionsLoader').remove();
            $('#unityExceptionsContainer').show();
        }
    });

    return false;
}

function deleteExceptionGroup(stacktraceHash) {

    if (confirm("This will delete ALL the exceptions in this group")) {

        $("#unityExceptionsContainer").hide();
        $("#unityExceptionsContainer").before(loadingImage.clone().attr('id', 'unityExceptionsLoader'));

        $.ajax({
            type: 'POST',
            url: applicationPath + 'Monitoring/DeleteExceptionGroup',
            data: { 'stacktraceHash': stacktraceHash },
            success: function (data) {

                if (data.IsDeleted) {
                    $('#unityExceptionsLoader').remove();
                    showSummarySuccess('The exceptions were deleted.');
                    getExceptionGroups();
                }
                else {
                    showSummaryError('The exceptions group couldn\'t be deleted.');
                    $('#unityExceptionsLoader').remove();
                    $('#unityExceptionsContainer').show();
                }
            },
            error: function (data) {
                showSummaryError('The exceptions group couldn\'t be deleted.');
                $('#unityExceptionsLoader').remove();
                $('#unityExceptionsContainer').show();
            }
        });
    }

    return false;
}

function deleteAllExceptions() {

    if (confirm("This will delete ALL the exceptions")) {

        $("#unityExceptionsContainer").hide();
        $("#unityExceptionsContainer").before(loadingImage.clone().attr('id', 'unityExceptionsLoader'));

        $.ajax({
            type: 'POST',
            url: applicationPath + 'Monitoring/DeleteAllExceptions',
            success: function (data) {

                if (data.IsDeleted) {
                    $('#unityExceptionsLoader').remove();
                    showSummarySuccess('The exceptions were deleted.');
                    getExceptionGroups();
                }
                else {
                    showSummaryError('The exceptions couldn\'t be deleted.');
                    $('#unityExceptionsLoader').remove();
                    $('#unityExceptionsContainer').show();
                }
            },
            error: function (data) {
                showSummaryError('The exceptions couldn\'t be deleted.');
                $('#unityExceptionsLoader').remove();
                $('#unityExceptionsContainer').show();
            }
        });
    }

    return false;
}