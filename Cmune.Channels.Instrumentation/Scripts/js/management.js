/*** Live Feeds Page ***/

function getLiveFeeds() {

    $("#liveFeedsContainer").hide();
    $("#liveFeedsContainer").before(loadingImage.clone().attr('id', 'liveFeedsLoader'));

    $.ajax({
        type: 'POST',
        url: applicationPath + 'Management/GetLiveFeeds',
        success: function (data) {
            $("#liveFeedsContainer").html(data);
            $("#liveFeedsLoader").remove();
            $("#liveFeedsContainer").show();
        },
        error: function (data) {
            $("#liveFeedsLoader").remove();
            $("#liveFeedsContainer").show();
            showSummaryError('Error getting the live feeds');
        }
    });
}

function loadAddLiveFeedForm(defaultPriority) {
    loadEditLiveFeedForm(0, '', '', defaultPriority);
}

function loadEditLiveFeedForm(liveFeedId, description, url, priority) {

    var formTitle = 'Add a live feed';

    if (liveFeedId != 0) {
        formTitle = 'Edit a live feed';
    }

    $("#liveFeedId").val(liveFeedId);
    $("#description").val(description);
    $("#url").val(url);
    $("#priority").val(priority);

    $.ajax({
        type: 'POST',
        url: applicationPath + 'Management/LoadAddEditLiveFeedForm',
        data: $("#liveFeedForm").serialize(),
        success: function (data) {
            Shadowbox.open({
                content: data,
                player: "html",
                title: formTitle,
                height: '180px'
            });
        },
        error: function (data) {
            showSummaryError('We were unable to load the form :(');
        }
    });
}

function AddEditLiveFeedGo() {
    $.ajax({
        type: 'POST',
        url: applicationPath + 'Management/AddEditLiveFeed',
        data: $('#addEditLiveFeedForm').serialize(),
        success: function (data) {
            if (data.IsModified) {
                Shadowbox.close();
                getLiveFeeds();
                showSummarySuccess(data.Message);
            }
            else {
                alert(data.Message);
            }
        },
        error: function (data) {
            alert(data.Message);
        }
    });
}

function deleteLiveFeed(liveFeedId) {
    if (confirm("Would you like to delete this live feed ?")) {
        $.ajax({
            type: 'POST',
            url: applicationPath + 'Management/DeleteLiveFeed',
            data: { 'liveFeedId': liveFeedId },
            success: function (data) {
                if (data.IsDeleted) {
                    getLiveFeeds();
                    showSummarySuccess(data.Message);
                }
                else {
                    alert(data.Message);
                }
            },
            error: function (data) {
                alert(data.Message);
            }
        });
    }
}

/******/

/*** E-PINS ***/

function generateEpins() {
    $("#generateEpinsButton").hide();
    $("#generateEpinsButton").before(loadingImage.clone().attr('id', 'generateEpinsLoader'));

    $.ajax({
        type: 'POST',
        url: applicationPath + 'Management/GenerateEpins',
        data: $("#generateEpinsForm").serialize(),
        success: function (data) {
            $("#generateEpinsButton").show();
            $("#generateEpinsLoader").remove();

            if (data.IsGenerated) {
                showSummarySuccess(data.Message);
                getEpinBatches();
            }
            else {
                showSummaryError(data.Message);
            }
        },
        error: function (data) {
            showSummaryError('error');
            $("#generateEpinsButton").show();
            $("#generateEpinsLoader").remove();
        }
    });
}

function isEpinAdmin(epinProvider) {

    if (epinProvider == '1') {
        $("#isAdminTr").hide();
    }
    else {
        $("#isAdminTr").show();
    }
}

function getEpinBatches() {

    $("#epinBatchesContainer").hide();
    $("#epinBatchesContainer").before(loadingImage.clone().attr('id', 'epinBatchesLoader'));

    var selectedPage = 1;

    if ($("#selectedPageEpinBatches").length > 0) {
        selectedPage = $('#selectedPageEpinBatches').val()
    }

    $.ajax({
        type: 'POST',
        url: applicationPath + 'Management/GetEpinBatches',
        data: { 'selectedPage': selectedPage },
        success: function (data) {
            $("#epinBatchesLoader").remove();
            $("#epinBatchesContainer").html(data);
            $("#epinBatchesContainer").show();
        },
        error: function (data) {
            $("#epinBatchesLoader").remove();
            $("#epinBatchesContainer").show();
            showSummaryError('error');
        }
    });
}

function changeBatchRetirementStatus(batchId) {
    var linkId = "#retire" + batchId + "Link";
    var loaderId = "retire" + batchId + "Loader";

    $(linkId).hide();
    $(linkId).before(loadingImage.clone().attr('id', loaderId));

    $.ajax({
        type: 'POST',
        url: applicationPath + 'Management/ChangeBatchRetirementStatus',
        data: { 'batchId': batchId },
        success: function (data) {

            if (data.IsStatusChanged) {
                getEpinBatches();
            }
            else {
                $(linkId).show();
                $("#" + loaderId).remove();
                showSummaryError('error');
            }
        },
        error: function (data) {
            $(linkId).show();
            $("#" + loaderId).remove();
            showSummaryError('error');
        }
    });
}

function sendMessageToAllUsers() {
    $.ajax({
        type: 'POST',
        url: applicationPath + 'Management/SendMessageToAllUsers',
        data: $("#sendMessageForm").serialize(),
        success: function (data) {
            alert("The Message was sent to " + data + " users!");
        },
        error: function (data) {
            alert("Something went wrong!");
        }
    });
}

function searchEpins() {
    $("#epinsContainer").hide();
    $("#epinsSearchButton").hide();
    $("#epinsSearchButton").before(loadingImage.clone().attr('id', 'epinsLoader'));

    $.ajax({
        type: 'POST',
        url: applicationPath + 'Management/GetEpins',
        data: $("#epinsSearchForm").serialize(),
        success: function (data) {
            $("#epinsLoader").remove();
            $("#epinsContainer").html(data);
            $("#epinsContainer").show();
            $("#epinsSearchButton").show();
        },
        error: function (data) {
            $("#epinsLoader").remove();
            $("#epinsContainer").show();
            $("#epinsSearchButton").show();
            showSummaryError('error');
        }
    });
}

function changeEpinRetirementStatus(epinId) {
    var linkId = "#retireEpin" + epinId + "Link";
    var loaderId = "retireEpin" + epinId + "Loader";

    $(linkId).hide();
    $(linkId).before(loadingImage.clone().attr('id', loaderId));

    $.ajax({
        type: 'POST',
        url: applicationPath + 'Management/ChangeEpinRetirementStatus',
        data: { 'epinId': epinId },
        success: function (data) {

            if (data.IsStatusChanged) {
                searchEpins();
            }
            else {
                $(linkId).show();
                $("#" + loaderId).remove();
                showSummaryError('error');
            }
        },
        error: function (data) {
            $(linkId).show();
            $("#" + loaderId).remove();
            showSummaryError('error');
        }
    });
}

function retireEpins() {
    $("#epinsContainer").hide();
    $("#retireEpinsLink").hide();
    $("#retireEpinsLink").before(loadingImage.clone().attr('id', 'retireEpinsLoader'));

    $.ajax({
        type: 'POST',
        url: applicationPath + 'Management/RetireEpins',
        data: $("#retireEpinsForm").serialize(),
        success: function (data) {

            if (data.IsStatusChanged) {
                $("#epinsContainer").html(data);
                $("#epinsContainer").show();
                searchEpins();
            }
            else {
                $("#retireEpinsLoader").remove();
                $('#retireEpinsLink').show();
                $("#retireEpinsLoader").remove();
                $("#epinsContainer").show();
                showSummaryError('error');
            }

        },
        error: function (data) {
            $("#retireEpinsLoader").remove();
            $("#epinsContainer").show();
            showSummaryError('error');
        }
    });
}

/******/

/*** Facebook reversals ***/

function getFacebookReversals() {

    $("#facebookReversalsContainer").hide();
    $("#getFacebookReversalsButton").hide();
    $("#getFacebookReversalsButton").before(loadingImage.clone().attr('id', 'facebookReversalsLoader'));
    $("#facebookReversalsLoader").after('<br /><span id="facebookReversalsLoaderText">We\'re loading the reversals, this will take a while.</span>');

    $.ajax({
        type: 'GET',
        url: applicationPath + 'Management/GetFacebookReversals',
        success: function (data) {
            $('#facebookReversalsContainer').html(data);
            $('#facebookReversalsLoader').remove();
            $('#facebookReversalsLoaderText').remove();
            $('#facebookReversalsContainer').show();
            $("#getFacebookReversalsButton").show();
        },
        error: function (data) {
            $('#facebookReversalsLoader').remove();
            $('#facebookReversalsLoaderText').remove();
            $("#getFacebookReversalsButton").show();
        }
    });

    return false;
}

function refundTransaction() {
    $("#refundOrderButton").hide();
    $("#refundOrderButton").before(loadingImage.clone().attr('id', 'refundTransactionLoader'));

    $.ajax({
        type: 'POST',
        url: applicationPath + 'Management/RefundTransaction',
        data: $("#refundTransactionForm").serialize(),
        success: function (data) {
            $("#refundTransactionLoader").remove();

            if (data.IsRefunded) {
                showSummarySuccess('The transaction was refunded');
                $("#refundOrderButton").show();
            }
            else {
                $('#refundOrderButton').show();
                showSummaryError('We couldn\'t refund the transaction');
            }

        },
        error: function (data) {
            $("#refundTransactionLoader").remove();
            $("#refundOrderButton").show();
            showSummaryError('error');
        }
    });

    return false;
}

function getTransaction() {
    $("#getOrderButton").hide();
    $("#transactionContainer").html('');
    $("#getOrderButton").before(loadingImage.clone().attr('id', 'getOrderLoader'));

    $.ajax({
        type: 'POST',
        url: applicationPath + 'Management/GetTransaction',
        data: $("#getTransactionForm").serialize(),
        success: function (data) {
            $("#getOrderLoader").remove();
            $("#getOrderButton").show();
            $("#transactionContainer").html(data);
        },
        error: function (data) {
            $("#getOrderLoader").remove();
            $("#getOrderButton").show();
            showSummaryError('error');
        }
    });

    return false;
}

/******/

/*** Manage XP ***/

function getXpEvents() {

    $("#xpEventsContainer").hide();
    $("#xpEventsContainer").before(loadingImage.clone().attr('id', 'xpEventsLoader'));

    $.ajax({
        type: 'POST',
        url: applicationPath + 'Management/GetXpEvents',
        success: function (data) {
            $('#xpEventsContainer').html(data);
            $('#xpEventsLoader').remove();
            $('#xpEventsContainer').show();
        },
        error: function (data) {
            $('#xpEventsLoader').remove();
            $('#xpEventsContainer').show();
        }
    });

    return false;
}

function editXpEvents() {

    $("#xpEventsContainer").hide();
    $("#xpEventsContainer").before(loadingImage.clone().attr('id', 'editXpEventsLoader'));

    $.ajax({
        type: 'POST',
        url: applicationPath + 'Management/EditXpEvents',
        data: $("#xpEventsForm").serialize(),
        success: function (data) {

            $('#editXpEventsLoader').remove();

            if (data.AreModified) {
                showSummarySuccess('XP events modified successfuly, reloading them now');
                getXpEvents();
            }
            else {
                showSummaryError('The XP events were not modified :(');
                $('#xpEventsContainer').show();
            }
        },
        error: function (data) {
            $('#editXpEventsLoader').remove();
            showSummaryError('error');
            $('#xpEventsContainer').show();
        }
    });

    return false;
}

function getLevelCaps() {

    $("#levelCapsContainer").hide();
    $("#levelCapsContainer").before(loadingImage.clone().attr('id', 'levelCapsLoader'));

    $.ajax({
        type: 'POST',
        url: applicationPath + 'Management/GetLevelCaps',
        success: function (data) {
            $('#levelCapsContainer').html(data);
            $('#levelCapsLoader').remove();
            $('#levelCapsContainer').show();
        },
        error: function (data) {
            $('#levelCapsLoader').remove();
            $('#levelCapsContainer').show();
        }
    });

    return false;
}

function editLevelCaps() {

    $("#levelCapsContainer").hide();
    $("#levelCapsContainer").before(loadingImage.clone().attr('id', 'editLevelCapsLoader'));

    $.ajax({
        type: 'POST',
        url: applicationPath + 'Management/EditLevelCaps',
        data: $("#levelCapsForm").serialize(),
        success: function (data) {

            $('#editLevelCapsLoader').remove();

            if (data.AreModified) {
                showSummarySuccess('Level caps modified successfuly, reloading them now');
                getLevelCaps();
            }
            else {
                showSummaryError('The Level caps were not modified :(');
                $('#levelCapsContainer').show();
            }
        },
        error: function (data) {
            $('#editLevelCapsLoader').remove();
            showSummaryError('error');
            $('#levelCapsContainer').show();
        }
    });

    return false;
}

function getItemsAttributed() {

    $("#itemsAttributedContainer").hide();
    $("#itemsAttributedContainer").before(loadingImage.clone().attr('id', 'itemsAttributedLoader'));

    $.ajax({
        type: 'POST',
        url: applicationPath + 'Management/GetItemsAttributed',
        success: function (data) {
            $('#itemsAttributedContainer').html(data);
            $('#itemsAttributedLoader').remove();
            $('#itemsAttributedContainer').show();
        },
        error: function (data) {
            $('#itemsAttributedLoader').remove();
            $('#itemsAttributedContainer').show();
        }
    });

    return false;
}

function setItemsAttributed() {

    $("#itemsAttributedContainer").hide();
    $("#itemsAttributedContainer").before(loadingImage.clone().attr('id', 'setItemsAttributedLoader'));

    $.ajax({
        type: 'POST',
        url: applicationPath + 'Management/SetItemsAttributed',
        data: $("#itemsAttributedForm").serialize(),
        success: function (data) {

            $('#setItemsAttributedLoader').remove();

            if (data.AreModified) {
                showSummarySuccess('Items attributed modified successfuly, reloading them now');
                getItemsAttributed();
            }
            else {
                showSummaryError('The items attributed were not modified :(');
                $('#itemsAttributedContainer').show();
            }
        },
        error: function (data) {
            $('#setItemsAttributedLoader').remove();
            showSummaryError('error');
            $('#itemsAttributedContainer').show();
        }
    });

    return false;
}

function getItemId() {

    $("#itemIdResultSpan").html('');
    $("#itemNameButton").hide();
    $("#itemNameButton").before(loadingImage.clone().attr('id', 'getItemIdLoader'));

    $.ajax({
        type: 'POST',
        url: applicationPath + 'Item/GetItemId',
        data: { 'itemName': $("#itemName").val() },
        success: function (data) {

            if (data.ItemId == '0') {
                $("#itemIdResultSpan").html('No item matching this name');
            }
            else {
                $("#itemIdResultSpan").html('The itemId is ' + data.ItemId);
            }

            $('#getItemIdLoader').remove();
            $('#itemNameButton').show();
        },
        error: function (data) {
            showSummaryError('An error happened');
            $('#getItemIdLoader').remove();
            $('#itemNameButton').show();
        }
    });

    return false;
}

function resynchroniseLevelsBasedOnXp() {

    $("#resynchroniseLevelsBasedOnXpButton").hide();
    $("#resynchroniseLevelsBasedOnXpButton").before(loadingImage.clone().attr('id', 'resynchroniseLevelsBasedOnXpLoader'));
    $("#syncLevelCapsQueries").html('');
    $("#syncXpLevelExplanation").hide();


    $.ajax({
        type: 'POST',
        url: applicationPath + 'Management/ResynchroniseLevelsBasedOnXp',
        success: function (data) {

            $("#syncXpLevelExplanation").show();
            $("#syncXpLevelQueries").html(data.Queries);

            $('#resynchroniseLevelsBasedOnXpLoader').remove();
            $('#resynchroniseLevelsBasedOnXpButton').show();
        },
        error: function (data) {
            showSummaryError('Error :(');

            $('#resynchroniseLevelsBasedOnXpLoader').remove();
            $('#resynchroniseLevelsBasedOnXpButton').show();
        }
    });

    return false;
}

function resynchroniseXpBasedOnLevel() {

    $("#resynchroniseXpBasedOnLevelButton").hide();
    $("#resynchroniseXpBasedOnLevelButton").before(loadingImage.clone().attr('id', 'resynchroniseXpBasedOnLevelLoader'));
    $("#syncLevelCapsQueries").html('');
    $("#syncXpLevelExplanation").hide();

    $.ajax({
        type: 'POST',
        url: applicationPath + 'Management/ResynchroniseXpBasedOnLevel',
        success: function (data) {

            $("#syncXpLevelExplanation").show();
            $("#syncXpLevelQueries").html(data.Queries);

            $('#resynchroniseXpBasedOnLevelLoader').remove();
            $('#resynchroniseXpBasedOnLevelButton').show();
        },
        error: function (data) {
            showSummaryError('Error :(');

            $('#resynchroniseXpBasedOnLevelLoader').remove();
            $('#resynchroniseXpBasedOnLevelButton').show();
        }
    });

    return false;
}

/******/

/*** Weekly Specials ***/

function getWeeklySpecials() {

    $("#weeklySpecialsContainer").hide();
    $("#weeklySpecialsContainer").before(loadingImage.clone().attr('id', 'weeklySpecialsLoader'));

    $.ajax({
        type: 'POST',
        url: applicationPath + 'Management/GetWeeklySpecials',
        success: function (data) {
            $('#weeklySpecialsContainer').html(data);
            $('#weeklySpecialsLoader').remove();
            $('#weeklySpecialsContainer').show();
        },
        error: function (data) {
            $('#weeklySpecialsLoader').remove();
            $('#weeklySpecialsContainer').show();
        }
    });

    return false;
}

function loadAddWeeklySpecialForm() {
    return loadEditWeeklySpecialForm(0);
}

function loadEditWeeklySpecialForm(weeklySpecialId) {

    var formTitle = 'Add a weekly special';
    var linkSelector = '';
    var loaderId = '';

    if (weeklySpecialId != 0) {
        formTitle = 'Edit a weekly special';
        linkSelector = '#editWeeklySpecial' + weeklySpecialId;
        loaderId = 'addWeeklySpecialLoader' + weeklySpecialId;
    }
    else {
        linkSelector = '#addWeeklySpecialLink';
        loaderId = 'addWeeklySpecialLoader'
    }

    var loaderSelector = '#' + loaderId;

    $(linkSelector).hide();
    $(linkSelector).before(loadingImage.clone().attr('id', loaderId));

    $.ajax({
        type: 'POST',
        url: applicationPath + 'Management/LoadAddEditWeeklySpecialForm',
        data: { 'id': weeklySpecialId },
        success: function (data) {
            $(linkSelector).show();
            $(loaderSelector).remove();

            if (data.HasError !== undefined && data.HasError == true) {
                showErrorSuccess('This weekly special does not exist');
            }
            else {
                Shadowbox.open({
                    content: data,
                    player: "html",
                    title: formTitle,
                    width: '700px',
                    height: '300px'
                });
            }
        },
        error: function (data) {
            showSummaryError('We were unable to load the form :(');
        }
    });
}

function addEditWeeklySpecial() {
    $("#addEditWeeklySpecialButton").hide();
    $("#addEditWeeklySpecialButton").before(loadingImage.clone().attr('id', 'addEditWeeklySpecialLoader'));

    $.ajax({
        type: 'POST',
        url: applicationPath + 'Management/AddEditWeeklySpecial',
        data: $('#addEditWeeklySpecialForm').serialize(),
        success: function (data) {
            $("#addEditWeeklySpecialButton").show();
            $("#addEditWeeklySpecialLoader").remove();

            if (data.IsModified == true) {
                Shadowbox.close();
                getWeeklySpecials();
            }
            else {
                showSummaryError(data.ErrorMessage);
            }
        },
        error: function (data) {
            $("#addEditWeeklySpecialButton").show();
            $("#addEditWeeklySpecialLoader").remove();

            showSummaryError('We were unable add/edit the weekly special');
        }
    });
}

function endWeeklySpecial(weeklySpecialId) {
    if (confirm("Are you sure you want to end this weekly special?")) {
        var linkSelector = "#endWeeklySpecial" + weeklySpecialId;
        var loaderId = "endWeeklySpecialLoader" + weeklySpecialId;
        var loaderSelector = "#" + loaderId;
        $(linkSelector).hide();
        $(linkSelector).before(loadingImage.clone().attr('id', loaderId));

        $.ajax({
            type: 'POST',
            url: applicationPath + 'Management/EndWeeklySpecial',
            data: { 'id': weeklySpecialId },
            success: function (data) {

                if (data.IsModified == true) {
                    getWeeklySpecials();
                }
                else {
                    $(linkSelector).show();
                    $(loaderSelector).remove();
                    showSummaryError('This weekly special does not exist anymore, please reload the page');
                }
            },
            error: function (data) {
                showSummaryError('We were unable to load the form :(');
            }
        });
    }
}