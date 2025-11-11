function searchClanGo() {
    $("#searchClanButton").hide();
    $("#searchClanButton").before(loadingImage.clone().attr('id', 'searchCLansLoader'));

    $.ajax({
        type: 'POST',
        url: applicationPath + 'Clan/GetClans',
        data: $("#searchClanForm").serialize(),
        success: function (data) {
            $("#searchClanButton").show();
            $("#searchCLansLoader").remove();
            $('#clansListGrid').html(data);
        },
        error: function (data) {
            showSummaryError('Error :(');
            $("#searchClanButton").show();
            $("#searchCLansLoader").remove();
        }
    });
}

function ChangeClan(action) {
    $('#action').val(action);
    $.ajax({
        url: applicationPath + "Clan/Edit",
        type: "POST",
        data: $('#editClanForm').serialize(),
        success: function (data) {
            if (data.isModified == false) {
                // mean data.message contains a error message
                if (data.message != undefined && data.message != '') {
                    switch (action) {
                        case 'changeTag':
                            showSummaryError(data.message);
                            break;
                        case 'changeName':
                            showSummaryError(data.message);
                            break;
                        case 'changeMotto':
                            showSummaryError(data.message);
                            break;
                    }
                }
            }
            else {
                // clear the form edit value
                switch (action) {
                    case 'changeTag':
                        showSummarySuccess(data.message);
                        window.setTimeout("window.location.reload()", 1000);
                        break;
                    case 'changeName':
                        showSummarySuccess(data.message);
                        window.setTimeout("window.location.reload()", 1000);
                        break;
                    case 'changeMotto':
                        showSummarySuccess(data.message);
                        window.setTimeout("window.location.reload()", 1000);
                        break;
                }
            }
        }
    });
    return false;
}

function deleteClan() {
    if (confirm("Are you sure you want to delete this clan?")) {
        $("#disbandClanDiv").hide();
        $("#disbandClanDiv").before(loadingImage.clone().attr('id', 'disbandCLanLoader'));

        $.ajax({
            url: applicationPath + "Clan/Delete",
            type: "POST",
            data: { 'cmid': $('#memberCmid').val(), 'clanId': $('#clanId').val() },
            success: function (data) {
                if (data.IsModified == false) {
                    showSummaryError(data.Message);
                    $("#disbandCLanLoader").remove();
                }
                else {
                    showSummarySuccess(data.Message);
                    window.setTimeout("window.location='" + data.ReturnUrl + "';", 1000);
                }
            }
        });
    }
}

/* See Clan */

function makeLeader(clanId, oldLeaderCmid, newLeaderCmid) {
    var linkId = '#makeLeader' + newLeaderCmid + 'Link';
    $(linkId).hide();
    $(linkId).before(loadingImage.clone().attr('id', 'makeLeaderLoader'));

    $.ajax({
        type: 'POST',
        url: applicationPath + 'Clan/MakeLeader',
        data: { 'clandId': clanId, 'oldLeaderCmid': oldLeaderCmid, 'newLeaderCmid': newLeaderCmid },
        success: function (data) {

            if (data.IsOk) {
                showSummarySuccess('New leader! Reloading the clan now...');
                setTimeout("window.location.reload(true)", 500);
            }
            else {
                $(linkId).show();
                $('#makeLeaderLoader').show();
                showSummaryError('error :(');
            }
        },
        error: function (data) {
            showSummaryError('Error :(');
            $(linkId).show();
            $("#makeLeaderLoader").remove();
        }
    });

    return false;
}

function kickFromClan(clanId, cmid) {
    var linkId = '#kick' + cmid + 'Link';
    $(linkId).hide();
    $(linkId).before(loadingImage.clone().attr('id', 'kickLoader'));

    $.ajax({
        type: 'POST',
        url: applicationPath + 'Clan/KickFromClan',
        data: { 'clandId': clanId, 'cmid': cmid },
        success: function (data) {

            if (data.IsOk) {
                showSummarySuccess('Kicked from clan! Reloading the clan now...');
                setTimeout("window.location.reload(true)", 500);
            }
            else {
                $(linkId).show();
                $('#kickLoader').show();
                showSummaryError('error :(');
            }
        },
        error: function (data) {
            showSummaryError('Error :(');
            $(linkId).show();
            $("#kickLoader").remove();
        }
    });

    return false;
}

/* End See Clan */