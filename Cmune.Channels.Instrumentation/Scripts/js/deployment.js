/* Application Versions */

function loadApplicationVersionForm(url, title, applicationVersionId) {
    $.ajax({
        type: 'POST',
        url: url,
        data: { 'applicationVersionId': applicationVersionId },
        success: function (data) {
            Shadowbox.open({
                content: data,
                player: "html",
                title: title,
                height: 220,
                width: 600,
                options: {
                    onFinish: function () {
                        checkApplicationVersionRestriction($("select[name=ChannelType]").val());
                    }
                }
            });
        },
        error: function (data) {
            showSummaryError('We couldn\'t load the form :(');
        }
    });
}

function saveApplicationVersion(url, form) {
        var formValues = $("#" + form).serialize();
        $('select[disabled]').each(function () {
            formValues = formValues + '&' + this.name + '=' + $(this).val();
        });
        $.ajax({
            type: 'POST',
            url: url,
            data: formValues,
            success: function (data) {
                if (data.errorMessage == "") {
                    Shadowbox.close();
                    getApplicationVersions();
                }
                else {
                    showSummaryError(data.errorMessage);
                }
            },
            error: function (data) {
                showSummaryError('We could\'nt save :(');
            }
        });
}

function deleteApplicationVersion(url, applicationVersionId) {
    if (confirm("Delete this application version?")) {
        $.ajax({
            type: 'POST',
            url: url,
            data: { 'applicationVersionId': applicationVersionId },
            success: function (data) {
                getApplicationVersions();
            },
            error: function (data) {
                showSummaryError('Couldn\'t do the operation :(');
            }
        });
    }
}

function checkApplicationVersionRestriction(selectedChannel) {
    selectedChannel = parseInt(selectedChannel);

    if ($.inArray(selectedChannel, standaloneChannels, 0) > -1) {
        $(".webPlayerFileNameInput").removeAttr("disabled");
        $(".channelWarning").html("");
    }
    else if ($.inArray(selectedChannel, webChannels, 0) > -1) {
        $(".webPlayerFileNameInput").removeAttr("disabled");
        $(".channelWarning").html("Enabling this version will disable all the other ones");
    }
    else {
        $(".webPlayerFileNameInput").attr("disabled", "disabled");
        $(".channelWarning").html("");
    }
}

function getApplicationVersions() {
    $('#applicationVersionsContainer').hide();
    $('#applicationVersionsContainer').after(loadingImage.clone().attr('id', 'getApplicationVersionsLoader'));

    $.ajax({
        type: 'POST',
        url: applicationPath + "Deployment/GetApplicationVersions",
        success: function (data) {
            $('#applicationVersionsContainer').html(data);
            $('#applicationVersionsContainer').show();
            $('#getApplicationVersionsLoader').remove();
        },
        error: function (data) {
            showSummaryError('We couldn\'t get the applications versions :(');
            $('#applicationVersionsContainer').show();
            $('#getApplicationVersionsLoader').remove();
        }
    });
}

function updateApplicationVersionIsEnabled(applicationVersionId) {
    var checkBoxId = '#isEnabled' + applicationVersionId;

    $(checkBoxId).hide();
    $(checkBoxId).before(loadingImage.clone().attr('id', 'isEnabledLoader'));

    $.ajax({
        type: "POST",
        url: applicationPath + "Deployment/UpdateApplicationVersionIsEnabled",
        data: { 'applicationVersionId': applicationVersionId, 'isEnabled': $(checkBoxId).attr('checked') },
        success: function (data) {
            $("#isEnabledLoader").remove();

            if (data.IsOk) {
                showSummarySuccess('Updated successfully');
                getApplicationVersions();
            }
            else {
                $(checkBoxId).val() = !$(checkBoxId).val();
                $(checkBoxId).show();
                showSummaryError('Couldn\'t update the IsEnabled status');
            }
        },
        error: function (data) {
            showSummaryError('Error :(');
            $("#isEnabledLoader").remove();
            $(checkBoxId).show();
        }
    });
}

function updateApplicationVersionWarnPlayer(applicationVersionId) {
    var checkBoxId = '#warnPlayer' + applicationVersionId;

    $(checkBoxId).hide();
    $(checkBoxId).before(loadingImage.clone().attr('id', 'warnPlayerLoader'));

    $.ajax({
        type: "POST",
        url: applicationPath + "Deployment/UpdateApplicationVersionWarnPlayer",
        data: { 'applicationVersionId': applicationVersionId, 'warnPlayer': $(checkBoxId).attr('checked') },
        success: function (data) {
            $("#warnPlayerLoader").remove();

            if (data.IsOk) {
                showSummarySuccess('Updated successfully');
                getApplicationVersions();
            }
            else {
                $(checkBoxId).val() = !$(checkBoxId).val();
                $(checkBoxId).show();
                showSummaryError('Couldn\'t update the WarnPlayer status');
            }
        },
        error: function (data) {
            showSummaryError('Error :(');
            $("#warnPlayerLoader").remove();
            $(checkBoxId).show();
        }
    });
}

/* End Application Versions */

/* Photon clusters */

function getPhotonsCluster(photonGroupId) {

    $("#photonsClusterContainer").hide();
    $("#photonsClusterContainer").before(loadingImage.clone().attr('id', 'photonsClusterLoader'));

    $.ajax({
        type: "GET",
        url: applicationPath + "Deployment/GetPhotonsCluster",
        data: { 'photonGroupId': photonGroupId },
        success: function (data) {
            $('#photonsClusterContainer').html(data);
            $("#photonsClusterContainer").show();
            $('#photonsClusterLoader').remove();
        },
        error: function (data) {
            $("#photonsClusterContainer").show();
            $('#photonsClusterLoader').remove();
        }
    });
}

function getPhotonsClustersDropDownListAndLatestPhotonsCluster(photonsClusterId) {
    var callbackFunction = function () { getPhotonsCluster($('#SelectPhotonsGroupDropDownList').val()) };

    if (photonsClusterId !== undefined) {
        callbackFunction = function () {
            $('#SelectPhotonsGroupDropDownList').val(photonsClusterId);
            getPhotonsCluster(photonsClusterId);
        };
    }

    updateDropDownList('SelectPhotonsGroupDropDownList', '/Deployment/GetPhotonClustersDropDownList', '', callbackFunction);
}

function savePhotonsCluster() {

    hideClusterSubmitButtons();

    $.ajax({
        type: 'POST',
        url: applicationPath + 'Deployment/SavePhotonCluster',
        data: $('#editPhotonGroupsAndPhotonServers').serialize(),
        success: function (data) {
            if (data.IsSaved) {
                showSummarySuccess(data.Message);
                getPhotonsClustersDropDownListAndLatestPhotonsCluster();
            }
            else {
                showSummaryError(data.Message);
                showClusterSubmitButtons();
            }
        },
        error: function (data) {
            showSummaryError('error');
            showClusterSubmitButtons();
        }
    });
}

function deletePhotonsCluster(photonsClusterId) {
    if (confirm("Are you sure to delete this photon cluster? ")) {
        $.ajax({
            type: 'POST',
            url: applicationPath + 'Deployment/DeletePhotonCluster',
            data: { "photonsClusterId": photonsClusterId },
            success: function (data) {
                if (data.isSuccess) {
                    showSummarySuccess(data.message);
                    window.location.reload(true);
                }
                else {
                    showSummarySuccess(data.message);
                }
            }
        });
    }
}

function hideClusterSubmitButtons() {
    $("#savePhotonClusterButton").hide();
    $("#displaySavePhotonClusterAsButton").hide();
    $("#savePhotonClusterButton").before(loadingImage.clone().attr('id', 'savePhotonsClusterLoader'));
}

function showClusterSubmitButtons() {
    $("#savePhotonClusterButton").show();
    $("#displaySavePhotonClusterAsButton").show();
    $("#savePhotonsClusterLoader").remove();
}

function loadSavePhotonsClusterAs() {

    hideClusterSubmitButtons();

    $.ajax({
        type: 'GET',
        url: applicationPath + 'Deployment/LoadSavePhotonsClusterAs',
        success: function (data) {
            Shadowbox.open({
                content: data,
                player: "html",
                title: "Save cluster as:",
                options: {
                    onClose: function () {
                    },
                    onFinish: function () {
                        showClusterSubmitButtons();
                        $("#NameMpeTextBox").val($("#NameTextBox").val());
                    }
                }
            });
        },
        error: function (data) {
            showSummaryError('error');
        }
    });
}

function savePhotonsClusterAs() {
    $("#NameTextBox").val($("#NameMpeTextBox").val());

    $.ajax({
        type: 'POST',
        url: applicationPath + 'Deployment/SavePhotonClusterAs',
        data: $('#editPhotonGroupsAndPhotonServers').serialize(),
        success: function (data) {
            if (data.IsSaved) {
                Shadowbox.close();
                showSummarySuccess(data.Message);
                getPhotonsClustersDropDownListAndLatestPhotonsCluster(data.PhotonClusterId);
            }
            else {
                showSummaryError(data.Message);
            }
        },
        error: function (data) {
            showSummaryError('error');
        }
    });
}

/* End Photons clusters */

/* Maps */

function getMapsCluster(applicationVersion) {

    if (applicationVersion == null) {
        applicationVersion = '';
    }

    $("#mapsContainer").hide();
    $("#mapsContainer").before(loadingImage.clone().attr('id', 'mapsClusterLoader'));

    $.ajax({
        type: "POST",
        url: applicationPath + "Deployment/GetMapsCluster",
        data: { 'applicationVersion': applicationVersion },
        success: function (data) {
            $('#mapsContainer').html(data);
            $("#mapsContainer").show();
            $('#mapsClusterLoader').remove();
        },
        error: function (data) {
            showSummaryError('Couldn\'t refresh the map cluster');
            $('#mapsContainer').html('');
            $("#mapsContainer").show();
            $('#mapsClusterLoader').remove();
        }
    });
}

function loadAddMap() {

    $("#addMapLink").hide();
    $("#addMapLink").before(loadingImage.clone().attr('id', 'addMapLoader'));

    $.ajax({
        type: 'POST',
        url: applicationPath + 'Deployment/LoadAddMap',
        data: { 'applicationVersion': $("#appVersion").val() },
        success: function (data) {
            Shadowbox.open({
                content: data,
                player: "html",
                title: "Add a map",
                width: 550,
                height: 380,
                options: {
                    onClose: function () {
                    },
                    onFinish: function () {
                        $("#addMapLink").show();
                        $("#addMapLoader").remove();
                    }
                }
            });
        },
        error: function (data) {
            showSummaryError('error');
            $("#addMapLink").show();
            $("#addMapLoader").remove();
        }
    });
}

function addMap() {
    $("#addButton").hide();
    $("#addButton").before(loadingImage.clone().attr('id', 'addMapLoader'));

    $.ajax({
        type: "POST",
        url: applicationPath + "Deployment/AddMap",
        data: $("#mapForm").serialize(),
        success: function (data) {
            $("#addMapLoader").remove();

            if (data.IsOk) {
                Shadowbox.close();
                getMapsClustersAndSelect(data.AppVersion);
            }
            else {
                $("#addButton").show();
                showSummaryError(data.StatusMessage);
            }
        },
        error: function (data) {
            showSummaryError('Couldn\'t add the map');
            $("#addButton").show();
            $("#addMapLoader").remove();
        }
    });
}

function updateMapVersion() {
    $("#mapVersionButton").hide();
    $("#mapVersionButton").before(loadingImage.clone().attr('id', 'addMapVersionLoader'));

    $.ajax({
        type: "POST",
        url: applicationPath + "Deployment/UpdateMapVersion",
        data: $("#mapVersionForm").serialize(),
        success: function (data) {
            $("#addMapVersionLoader").remove();

            if (data.IsUpdated) {
                getMapsClustersAndSelect(data.AppVersion);
            }
            else {
                $("#mapVersionButton").show();
                showSummaryError('The map version was not updated');
            }
        },
        error: function (data) {
            showSummaryError('Couldn\'t update the map version');
            $("#mapVersionButton").show();
            $("#addMapVersionLoader").remove();
        }
    });
}

function loadEditMap(mapId) {
    var linkId = '#editMap' + mapId + 'Link';

    $(linkId).hide();
    $(linkId).before(loadingImage.clone().attr('id', 'editMapLoader'));

    $.ajax({
        type: 'POST',
        url: applicationPath + 'Deployment/LoadEditMap',
        data: { 'applicationVersion': $("#appVersion").val(), 'mapId': mapId },
        success: function (data) {
            Shadowbox.open({
                content: data,
                player: "html",
                title: "Edit a map",
                width: 550,
                height: 270,
                options: {
                    onClose: function () {
                    },
                    onFinish: function () {
                        $(linkId).show();
                        $("#editMapLoader").remove();
                    }
                }
            });
        },
        error: function (data) {
            showSummaryError('error');
            $(linkId).show();
            $("#editMapLoader").remove();
        }
    });
}

function editMap() {
    $("#editButton").hide();
    $("#editButton").before(loadingImage.clone().attr('id', 'editMapLoader'));

    $.ajax({
        type: "POST",
        url: applicationPath + "Deployment/EditMap",
        data: $("#mapForm").serialize(),
        success: function (data) {
            $("#addMapLoader").remove();

            if (data.IsOk) {
                Shadowbox.close();
                getMapsCluster(data.AppVersion)
            }
            else {
                $("#editButton").show();
                showSummaryError(data.StatusMessage);
            }
        },
        error: function (data) {
            showSummaryError('Couldn\'t edit the map');
            $("#editButton").show();
            $("#editMapLoader").remove();
        }
    });
}

function getMapsClustersAndSelect(appVersion) {
    var callbackFunction = function () { getMapsCluster($('#applicationVersions').val()) };

    if (appVersion !== undefined) {
        callbackFunction = function () {
            $('#applicationVersions').val(appVersion);
            getMapsCluster(appVersion);
        };
    }

    updateDropDownList('applicationVersions', '/Deployment/GetMapApplicationVersionsSelect', '', callbackFunction);
}

function updateMapInUse(mapId) {
    var checkBoxId = '#inUse' + mapId;

    $(checkBoxId).hide();
    $(checkBoxId).before(loadingImage.clone().attr('id', 'inUseLoader'));

    $.ajax({
        type: "POST",
        url: applicationPath + "Deployment/UpdateMapInUse",
        data: { 'appVersion': $("#appVersion").val(), 'mapId': mapId, 'isInUse': $(checkBoxId).attr('checked') },
        success: function (data) {
            $("#inUseLoader").remove();

            if (data.IsOk) {
                showSummarySuccess('Updated successfully');
                $(checkBoxId).show();
            }
            else {
                $(checkBoxId).val() = !$(checkBoxId).val();
                $(checkBoxId).show();
                showSummaryError(data.StatusMessage);
            }
        },
        error: function (data) {
            showSummaryError('Couldn\'t update the InUse status');
            $("#inUseLoader").remove();
            $(checkBoxId).show();
        }
    });
}

function updateIsBlueBox(mapId) {
    var checkBoxId = '#isBlueBox' + mapId;

    $(checkBoxId).hide();
    $(checkBoxId).before(loadingImage.clone().attr('id', 'isBlueBoxLoader'));

    $.ajax({
        type: "POST",
        url: applicationPath + "Deployment/UpdateIsBlueBox",
        data: { 'appVersion': $("#appVersion").val(), 'mapId': mapId, 'isBlueBox': $(checkBoxId).attr('checked') },
        success: function (data) {
            $("#isBlueBoxLoader").remove();

            if (data.IsOk) {
                showSummarySuccess('Updated successfully');
                $(checkBoxId).show();
            }
            else {
                $(checkBoxId).val() = !$(checkBoxId).val();
                $(checkBoxId).show();
                showSummaryError(data.StatusMessage);
            }
        },
        error: function (data) {
            showSummaryError('Couldn\'t update the IsBlueBox status');
            $("#isBlueBoxLoader").remove();
            $(checkBoxId).show();
        }
    });
}

function deleteMap(mapId) {
    if (confirm("Are your sure to delete this map?")) {
        var deleteLinkId = '#deleteMap' + mapId + 'Link';

        $(deleteLinkId).hide();
        $(deleteLinkId).before(loadingImage.clone().attr('id', 'deleteLoader'));

        $.ajax({
            type: "POST",
            url: applicationPath + "Deployment/DeleteMap",
            data: { 'appVersion': $("#appVersion").val(), 'mapId': mapId },
            success: function (data) {
                $("#deleteLoader").remove();

                if (data.IsOk) {
                    showSummarySuccess('Deleted successfully');
                    getMapsCluster($('#applicationVersions').val());
                }
                else {
                    showSummaryError(data.StatusMessage);
                }
            },
            error: function (data) {
                showSummaryError('Couldn\'t delete the map');
                $("#deleteLoader").remove();
                $(deleteLinkId).show();
            }
        });
    }
}

function hideMapsClusterSubmitButtons() {
    $("#displaySaveAsMapsClusterButton").hide();
    $("#deleteMapsClusterButton").hide();
    $("#displaySaveAsMapsClusterButton").before(loadingImage.clone().attr('id', 'mapClusterSubmitLoader'));
}

function showMapsClusterSubmitButtons() {
    $("#displaySaveAsMapsClusterButton").show();
    $("#deleteMapsClusterButton").show();
    $("#mapClusterSubmitLoader").remove();
}

function saveAsMapCluster() {
    if (confirm("Are your sure to create a new map cluster named '" + $("#clusterName").val() + "'?")) {
        hideMapsClusterSubmitButtons();

        $.ajax({
            type: 'POST',
            url: applicationPath + 'Deployment/SaveAsMapCluster',
            data: { 'appVersion': $('#applicationVersions').val(), 'clusterName': $("#clusterName").val() },
            success: function (data) {
                if (data.IsOk) {
                    showSummarySuccess('Created successfully');
                    getMapsClustersAndSelect($("#clusterName").val());
                }
                else {
                    showSummaryError(data.StatusMessage);
                    showMapsClusterSubmitButtons();
                }
            },
            error: function (data) {
                showSummaryError('error');
                showMapsClusterSubmitButtons();
            }
        });
    }
}

function deleteMaps() {
    if (confirm("Do you want to delete this map cluster?")) {
        hideMapsClusterSubmitButtons();

        $.ajax({
            type: 'POST',
            url: applicationPath + 'Deployment/DeleteMaps',
            data: { 'appVersion': $('#applicationVersions').val() },
            success: function (data) {
                if (data.IsOk) {
                    showSummarySuccess('Deleted successfully');
                    getMapsClustersAndSelect();
                }
                else {
                    showSummaryError(data.StatusMessage);
                    showMapsClusterSubmitButtons();
                }
            },
            error: function (data) {
                showSummaryError('error');
                showMapsClusterSubmitButtons();
            }
        });
    }
}

/* End Maps */

/* Application milestones */

function getMilestones() {
    $('#milestonesDiv').hide();
    $('#milestonesDiv').before(loadingImage.clone().attr('id', 'getMilestonesLoadingImg'));

    $.ajax({
        type: 'POST',
        url: applicationPath + 'Deployment/GetApplicationMilestones',
        success: function (data) {
            $('#milestonesDiv').html(data);
            $('#getMilestonesLoadingImg').remove();
            $('#milestonesDiv').show();
        },
        error: function (data) {
            $('#getMilestonesLoadingImg').remove();
            $('#milestonesDiv').show();
        }
    });

    return false;
}

function createApplicationMilestone() {
    $('#createMilestoneButton').hide();
    $('#createMilestoneButton').after(loadingImage.clone().attr('id', 'createMilestoneLoadingImg'));

    $.ajax({
        type: 'POST',
        url: applicationPath + 'Deployment/CreateApplicationMilestone',
        data: $('#createMilestoneForm').serialize(),
        success: function (data) {
            if (data.IsMilestoneCreated) {
                $('#createMilestoneLoadingImg').remove();
                $('#createMilestoneButton').show();
                $('#MilestoneDateTextBox').val('');
                $('#MilestoneDescriptionTextBox').val('');
                showSummarySuccess(data.Message);
                getMilestones();
            }
            else {
                $('#createMilestoneLoadingImg').remove();
                $('#createMilestoneButton').show();
                showSummaryError(data.Message);
            }
        },
        error: function (data) {
            $('#createMilestoneLoadingImg').remove();
            $('#createMilestoneButton').show();
        }
    });

    return false;
}

function displayEditMilestone(milestoneId, date, description) {
    $('#milestoneDate' + milestoneId).hide();
    $('#milestoneDate' + milestoneId).after('<input id="milestoneDate' + milestoneId + 'Input" name="milestoneDate' + milestoneId + 'Input" type="text" value="' + date + '" />');
    $('#milestoneDescription' + milestoneId).hide();
    $('#milestoneDescription' + milestoneId).after('<input id="milestoneDescription' + milestoneId + 'Input" name="milestoneDescription' + milestoneId + 'Input" type="text" value="' + description + '" />');
    $('#displayEditMilestone' + milestoneId + 'Link').hide();
    $('#displayEditMilestone' + milestoneId + 'Link').after('<div id="confirmEditMilestone' + milestoneId + 'Div"><a href="#" onclick="editMilestone(' + milestoneId + ');">Ok</a> <a href="#" onclick="cancelEditMilestone(' + milestoneId + ');">Cancel</a></div>');

    return false;
}

function cancelEditMilestone(milestoneId) {
    $('#milestoneDate' + milestoneId + 'Input').remove();
    $('#milestoneDate' + milestoneId).show();
    $('#milestoneDescription' + milestoneId + 'Input').remove();
    $('#milestoneDescription' + milestoneId).show();
    $('#displayEditMilestone' + milestoneId + 'Link').show();
    $('#confirmEditMilestone' + milestoneId + 'Div').remove();

    return false;
}

function editMilestone(milestoneId) {

    $('#confirmEditMilestone' + milestoneId + 'Div').hide();
    $('#confirmEditMilestone' + milestoneId + 'Div').after(loadingImage.clone().attr('id', 'editMilestone' + milestoneId + 'LoadingImg'));

    $.ajax({
        type: 'POST',
        url: applicationPath + "Deployment/EditApplicationMilestone",
        data: { 'milestoneId': milestoneId,
            'description': $('#milestoneDescription' + milestoneId + 'Input').val(),
            'creationDate': $('#milestoneDate' + milestoneId + 'Input').val()
        },
        success: function (data) {
            if (data.IsMilestoneEdited) {
                getMilestones();
                showSummarySuccess(data.Message);
            }
            else {
                $('#confirmEditMilestone' + milestoneId + 'Div').show();
                $('#editMilestone' + milestoneId + 'LoadingImg').remove();
                showSummaryError(data.Message);
            }
        },
        error: function (data) {
            $('#confirmEditMilestone' + milestoneId + 'Div').show();
            $('#editMilestone' + milestoneId + 'LoadingImg').remove();
        }
    });
}

function deleteMilestone(milestoneId) {
    if (confirm("Are your sure you want to delete this milestone?")) {

        $('#deleteMilestone' + milestoneId + 'Link').hide();
        $('#deleteMilestone' + milestoneId + 'Link').after(loadingImage.clone().attr('id', 'deleteMilestoneLoadingImg'));

        $.ajax({
            type: 'POST',
            url: applicationPath + "Deployment/DeleteApplicationMilestone",
            data: { 'milestoneId': milestoneId },
            success: function (data) {
                if (data.IsMilestoneDeleted) {
                    getMilestones();
                    showSummarySuccess(data.Message);
                }
                else {
                    $('#deleteMilestone' + milestoneId + 'Link').show();
                    $('#deleteMilestoneLoadingImg').remove();
                    showSummaryError(data.Message);
                }
            },
            error: function (data) {
                $('#deleteMilestone' + milestoneId + 'Link').show();
                $('#deleteMilestoneLoadingImg').remove();
            }
        });
    }
}

/* End application milestones */