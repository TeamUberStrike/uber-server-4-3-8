/* Search user */

function searchUsers() {
    
    var selectedPage =  $("#selectedPageUsers").val();
    $('#searchUsersResultContainer').html('');
    $('#loadingRes').show();
    $('#noResultLabel').hide();
    $("#searchUsersButton").hide();

    $.ajax({
        type: 'POST',
        url: applicationPath + 'Member/SearchUsers',
        data: $("#searchUserForm").serialize() + '&selectedPage=' + selectedPage,
        success: function (data) {
            if (data.Redirect !== undefined && data.Redirect == true) {
                showSummarySuccess('User was found, loading her profile...');
                window.location = applicationPath + 'Member/See?Cmid=' + data.Cmid;
            }
            else if (data == "" || data == undefined) {
                $('#loadingRes').hide();
                $("#searchUsersButton").show();
                $('#noResultLabel').show();
            }
            else {
                $('#loadingRes').hide();
                $("#searchUsersButton").show
                $('#searchUsersResultContainer').html(data);
            }
        },
        error: function (data) {
            showSummaryError('Sorry');
            $('#loadingRes').hide();
            $("#searchUserButton").show();
        }
    });
}

function searchUsersByPreviousEmail() {

    $('#searchUsersByPreviousEmailResultContainer').html('');
    $("#searchUsersByPreviousEmailButton").before(loadingImage.clone().attr('id', 'searchUsersByPreviousEmailLoader'));
    $("#searchUsersByPreviousEmailButton").hide();

    $.ajax({
        type: 'POST',
        url: applicationPath + 'Member/SearchUsersByPreviousEmail',
        data: $("#searchByPreviousEmailForm").serialize(),
        success: function (data) {
            if (data.Redirect !== undefined && data.Redirect == true) {
                showSummarySuccess('User was found, loading his profile...');
                window.location = applicationPath + 'Member/See?Cmid=' + data.Cmid;
            }
            else {
                $('#searchUsersByPreviousEmailLoader').remove();
                $("#searchUsersByPreviousEmailButton").show();
                $('#searchUsersByPreviousEmailResultContainer').html(data);
            }
        },
        error: function (data) {
            showSummaryError('Error :(');
            $('#searchUsersByPreviousEmailLoader').remove();
            $("#searchUsersByPreviousEmailButton").show();
        }
    });
}

function searchUsersByPreviousName() {

    $('#searchUsersByPreviousNameResultContainer').html('');
    $("#searchUsersByPreviousNameButton").before(loadingImage.clone().attr('id', 'searchUsersByPreviousNameLoader'));
    $("#searchUsersByPreviousNameButton").hide();

    $.ajax({
        type: 'POST',
        url: applicationPath + 'Member/SearchUsersByPreviousName',
        data: $("#searchByPreviousNameForm").serialize(),
        success: function (data) {
            if (data.Redirect !== undefined && data.Redirect == true) {
                showSummarySuccess('User was found, loading his profile...');
                window.location = applicationPath + 'Member/See?Cmid=' + data.Cmid;
            }
            else {
                $('#searchUsersByPreviousNameLoader').remove();
                $("#searchUsersByPreviousNameButton").show();
                $('#searchUsersByPreviousNameResultContainer').html(data);
            }
        },
        error: function (data) {
            showSummaryError('Error :(');
            $('#searchUsersByPreviousNameLoader').remove();
            $("#searchUsersByPreviousNameButton").show();
        }
    });
}

/* End of search user */

/* Custom actions */

function attributeCurrency() {
    $('#attributeCurrencyResult').hide();
    $('#attributeCurrencyButton').hide();
    $('#attributeCurrencyLoadingImg').show();
    $.ajax({
        type: 'POST',
        url: applicationPath + 'Member/AttributeCurrency',
        data: $("#attributeCurrencyForm").serialize(),
        success: function (data) {
            $('#attributeCurrencyLoadingImg').hide();
            $('#attributeCurrencyButton').show();
            $('#attributeCurrencyResult').html('Success :)');
            $('#attributeCurrencyResult').attr("class", "successMessage");
            $('#attributeCurrencyResult').show();
        },
        error: function (data) {
            $('#attributeCurrencyLoadingImg').hide();
            $('#attributeCurrencyButton').show();
            $('#attributeCurrencyResult').html('Error :(');
            $('#attributeCurrencyResult').attr("class", "errorMessage");
            $('#attributeCurrencyResult').show();
        }
    });
}

function banPermanently() {

    $("#banCmidsForm").hide();
    $("#banCmidsContainer").hide();
    $("#banCmidsForm").before(loadingImage.clone().attr('id', 'banCmidsLoader').attr('style', 'display:block; margin:auto;'));

    $.ajax({
        type: 'POST',
        url: applicationPath + 'Member/BanCheaters',
        data: $("#banCmidsForm").serialize(),
        success: function (data) {
            $("#banCmidsContainer").html(data);
            $("#banCmidsContainer").show();
            $('#banCmidsLoader').remove();
            $("#banCmidsForm").show();
        },
        error: function (data) {
            $('#banCmidsLoader').remove();
            $("#banCmidsForm").show();
            showSummaryError('An error happened');
        }
    });
}



function whoBoughtItem() {
    $('#whoBoughtItemResultP').hide();
    $('#whoBoughtItemButton').hide();
    $('#whoBoughtItemLoadingImg').show();
    $.ajax({
        type: 'POST',
        url: applicationPath + 'Member/WhoBoughtItem',
        data: $("#whoBoughtItemForm").serialize(),
        success: function (data) {
            $('#whoBoughtItemLoadingImg').hide();
            $('#whoBoughtItemButton').show();
            $('#whoBoughtItemResult').html(data);

            var dataCount = 0;

            if (data.length > 0) {
                dataCount = data.split(",").length;
            }

            $('#whoBoughtItemResultCount').html(dataCount);
            $('#whoBoughtItemResultP').show();
        },
        error: function (data) {
            $('#whoBoughtItemLoadingImg').hide();
            $('#whoBoughtItemButton').show();
            $('#whoBoughtItemResult').html('Error :(');
            $('#whoBoughtItemResult').attr("class", "errorMessage");
            $('#whoBoughtItemResultCount').html('');
            $('#whoBoughtItemResultP').show();
        }
    });
}

function retrieveMembersCount() {
    $('#biggestCmidResultP').hide();
    $('#biggestCmidButton').hide();
    $('#biggestCmidLoadingImg').show();
    $.ajax({
        type: 'POST',
        url: applicationPath + 'Member/RetrieveMembersCount',
        data: $("#biggestCmidForm").serialize(),
        success: function (data) {
            $('#biggestCmidLoadingImg').hide();
            $('#biggestCmidButton').show();
            $('#biggestCmidResult').html(data);
            $('#biggestCmidResultP').show();
        },
        error: function (data) {
            $('#biggestCmidLoadingImg').hide();
            $('#biggestCmidButton').show();
            $('#biggestCmidResult').html('Error :(');
            $('#biggestCmidResult').attr("class", "errorMessage");
            $('#biggestCmidResultP').show();
        }
    });
}

function retrieveEsnsMembersCount() {

    $('#esnsMembersCountResultP').hide();
    $('#esnsMembersCountButton').hide();
    $('#esnsMembersCountLoadingImg').show();

    $('#esnsMembersCountEsnsName').html($('#membersCountEsns option:selected').text());

    $.ajax({
        type: 'POST',
        url: applicationPath + 'Member/RetrieveEsnsMembersCount',
        data: $("#esnsMembersCountForm").serialize(),
        success: function (data) {
            $('#esnsMembersCountLoadingImg').hide();
            $('#esnsMembersCountButton').show();
            $('#esnsMembersCountResult').html(data);
            $('#esnsMembersCountResultP').show();
        },
        error: function (data) {
            $('#esnsMembersCountLoadingImg').hide();
            $('#esnsMembersCountButton').show();
            $('#esnsMembersCountResult').html('Error :(');
            $('#esnsMembersCountResult').attr("class", "errorMessage");
            $('#esnsMembersCountResultP').show();
        }
    });
}

function convertIp() {

    $('#convertIpButton').hide();
    $('#ipConverterContainer').html('');
    $('#convertIpButton').after(loadingImage.clone().attr('id', 'ipConverterLoader'));

    $.ajax({
        type: 'POST',
        url: applicationPath + 'Member/ConvertIp',
        data: { 'ip': $("#ipToConvert").val() },
        success: function (data) {
            $('#ipConverterLoader').remove();
            $('#convertIpButton').show();
            $('#ipConverterContainer').html(data);
        },
        error: function (data) {
            $('#ipConverterLoader').remove();
            $('#convertIpButton').show();
            showSummaryError('Error converting the IP');
        }
    });
}

/* End of custom actions */

function LoadDelete(){
    $.ajax({
        type: 'GET',
        url: applicationPath + 'Member/LoadDelete',
        data: { 'memberCmid': $('#memberCmid').val(),
                'memberEmail': $('#memberEmail').val(),
                'memberName': $('#memberName').val()
        },
        success: function (data) {
            Shadowbox.open({
                content: data,
                player: "html",
                title: "Delete a member"
            });
        }
    });
}

function deleteMemberGo() {
    $.ajax({
        type: 'POST',
        url: applicationPath + 'Member/Delete',
        data: $('#deleteMemberGo').serialize(),
        success: function (data) {
            if (data.isDeleted == true) {
                $('#DeleteUserUpdatePanel').html(data.message);
                $('#MemberSeeContent').html('<a href="' + applicationPath + 'Member/Search">Go Back</a>');
            }
            else {
                ShowShadowSummaryErrorWithFadeOut(data.message);
            }
        }
    });  
}

function LoadCompleteMemberForm()
{
    $.ajax({
        type: 'GET',
        url: applicationPath + 'Member/LoadCompleteMemberForm',
        data: { 'cmid': $('#memberCmid').val(),
                'memberEmail': $('#memberEmail').val(),
                'memberName': $('#memberName').val()
        },
        success: function (data) {
            Shadowbox.open({
                content: data,
                player: "html",
                title: "Complete member"
            });
        }
    });
}

function LoadBan() {
    $.ajax({
        type: 'GET',
        url: applicationPath + 'Member/LoadBan',
        data: { 'cmid': $('#memberCmid').val(),
                'memberAccessIsAccountDisabled': $('#memberAccessIsAccountDisabled').val(),
                'banUtil': $('#banStatusLabel').html()
        },
        success: function (data) {
            Shadowbox.open({
                content: data,
                player: "html",
                title: "Ban account",
                height: 340,
                options: {
                    onClose: function () {
                    },
                    onFinish: function () {
                        selectCurrentAdmin('adminNames');
                    }
                }
            });
        }
    });
}

function LoadChatBan()
{
    $.ajax({
        type: 'GET',
        url: applicationPath + 'Member/LoadChatBan',
        data: { 'cmid': $('#memberCmid').val(),
                'memberAccessIsChatDisabled': $('#memberAccessIsChatDisabled').val(),
                'banUtil': $('#chatBanStatusLabel').html()
        },
        success: function (data) {
            Shadowbox.open({
                content: data,
                player: "html",
                title: "Ban from chat",
                options: {
                    onClose: function () {
                    },
                    onFinish: function () {
                        selectCurrentAdmin('adminNames');
                    }
                }
            });
        }
    });
}

function LoadCreditsAndPoints() {
    $.ajax({
        type: 'GET',
        url: applicationPath + 'Member/LoadCreditsAndPoints',
        data: { 'cmid': $('#memberCmid').val() },
        success: function (data) {
            Shadowbox.open({
                content: data,
                player: "html",
                title: "Add Credits and points",
                options: {
                    onClose: function () {
                    },
                    onFinish: function () {
                        selectCurrentAdmin('adminNames');
                    }
                }
            });
        }
    });
}

function getPreviousClans() {

    $("#previousClansContainer").hide();
    $("#viewPreviousClansButton").hide();
    $("#previousClansContainer").before(loadingImage.clone().attr('id', 'previousClansLoader'));

    $.ajax({
        type: 'POST',
        url: applicationPath + 'Member/GetPreviousClans',
        data: { 'cmid': $('#memberCmid').val() },
        success: function (data) {
            $('#previousClansContainer').html(data);
            $('#previousClansLoader').remove();
            $('#previousClansContainer').show();
            $('#viewPreviousClansButton').show();
        },
        error: function (data) {
            $('#previousClansLoader').remove();
            $('#previousClansContainer').show();
            $('#viewPreviousClansButton').show();
        }
    });

    return false;
}

function getItems() {

    $("#itemsContainer").hide();
    $("#itemsContainer").before(loadingImage.clone().attr('id', 'itemsLoader'));

    $.ajax({
        type: 'POST',
        url: applicationPath + 'Member/GetItems',
        data: { 'cmid': $('#memberCmid').val() },
        success: function (data) {
            $('#itemsContainer').html(data);
            $('#itemsLoader').remove();
            $('#itemsContainer').show();
        },
        error: function (data) {
            $('#itemsLoader').remove();
            $('#itemsContainer').show();
        }
    });

    return false;
}

function deleteMemberItem(itemId) {
    if (confirm("Are your sure to delete this member's item ?")) {

        $("#itemsContainer").hide();
        $("#itemsContainer").before(loadingImage.clone().attr('id', 'itemsLoader'));

        $.ajax({
            type: 'POST',
            url: applicationPath + "Member/DeleteMemberItem",
            data: { 'cmid': $('#memberCmid').val(), 'itemId': itemId },
            success: function (data) {
                if (data.IsModified == false) {
                    showSummaryError(data.Message);
                    $("#itemsContainer").show();
                    $("#itemsLoader").remove();
                }
                else {
                    showSummarySuccess(data.Message);
                    $("#itemsLoader").remove();
                    getItems();
                }
            },
            error: function (data) {
                $("#itemsContainer").show();
                $("#itemsLoader").remove();
            }
        });
    }

    return false;
}

function updateAccountToUberStrike() {
    $("#itemsContainer").hide();
    $("#itemsContainer").before(loadingImage.clone().attr('id', 'itemsLoader'));

    $.ajax({
        type: 'POST',
        url: applicationPath + "Member/UpdateAccountToUberStrike",
        data: { 'cmid': $('#memberCmid').val() },
        success: function (data) {
            if (data.IsUpdated == false) {
                showSummaryError('We couldn\'t update the account to UberStrike.');
                $("#itemsContainer").show();
                $("#itemsLoader").remove();
            }
            else {
                showSummarySuccess('The account was updated to UberStrike');
                $("#itemsLoader").remove();
                getItems();
            }
        },
        error: function (data) {
            $("#itemsContainer").show();
            $("#itemsLoader").remove();
        }
    });

    return false;
}

function LoadTransactionsHistory() {
    $.ajax({
        type: 'GET',
        url: applicationPath + 'Member/LoadTransactionsHistory',
        data: { 'cmid': $('#memberCmid').val(),
                'selectedPage': $('#selectedPageTransactionHistory').val()
        },
        success: function (data) {
            $("#TransactionsHistoryContainer").html(data);
        }
    });
}

function getCurrencyDeposits() {

    $("#currencyDepositsContainer").before(loadingImage.clone().attr('id', 'currencyDepositsLoader'));

    var selectedPage = 1;

    if ($("#selectedPageCurrencyDeposits").length > 0) {
        selectedPage = $('#selectedPageCurrencyDeposits').val()
    }

    $("#currencyDepositsContainer").html('');

    $.ajax({
        type: 'GET',
        url: applicationPath + 'Member/GetCurrencyDeposits',
        data: { 'cmid': $('#memberCmid').val(), 'selectedPage': selectedPage },
        success: function (data) {
            $("#currencyDepositsLoader").remove();
            $("#currencyDepositsContainer").html(data);
        },
        error: function (data) {
            $("#currencyDepositsLoader").remove();
        }
    });
}

function getTotalCurrencyDeposits() {
    $("#totalCurrencyDepositsSpan").before(loadingImage.clone().attr('id', 'totalCurrencyDepositsLoader'));
    $("#totalCurrencyDepositsSpan").html('');
    $("#banSpentSpan").before(loadingImage.clone().attr('id', 'totalCurrencyDepositsBanLoader'));
    $("#banSpentSpan").html('');

    $.ajax({
        type: 'GET',
        url: applicationPath + 'Member/GetTotalCurrencyDeposits',
        data: { 'cmid': $('#memberCmid').val() },
        success: function (data) {
            $("#totalCurrencyDepositsLoader").remove();
            $("#totalCurrencyDepositsSpan").html(data);
            $("#totalCurrencyDepositsBanLoader").remove();
            $("#banSpentSpan").html(data);
        },
        error: function (data) {
            $("#totalCurrencyDepositsLoader").remove();
            $("#totalCurrencyDepositsSpan").html('error');
            $("#totalCurrencyDepositsBanLoader").remove();
            $("#banSpentSpan").html('error');
        }
    });
}

function getItemTransactions() {

    $("#itemTransactionsContainer").before(loadingImage.clone().attr('id', 'itemTransactionsLoader'));

    var selectedPage = 1;

    if ($("#selectedPageItemTransactions").length > 0) {
        selectedPage = $('#selectedPageItemTransactions').val()
    }

    $("#itemTransactionsContainer").html('');

    $.ajax({
        type: 'GET',
        url: applicationPath + 'Member/GetItemTransactions',
        data: { 'cmid': $('#memberCmid').val(), 'selectedPage': selectedPage },
        success: function (data) {
            $("#itemTransactionsLoader").remove();
            $("#itemTransactionsContainer").html(data);
        },
        error: function (data) {
            $("#itemTransactionsLoader").remove();
        }
    });
}

function getPointsDeposits() {

    $("#pointsDepositsContainer").before(loadingImage.clone().attr('id', 'pointsDepositsLoader'));

    var selectedPage = 1;

    if ($("#selectedPagePointsDeposits").length > 0) {
        selectedPage = $('#selectedPagePointsDeposits').val()
    }

    $("#pointsDepositsContainer").html('');

    $.ajax({
        type: 'GET',
        url: applicationPath + 'Member/GetPointsDeposits',
        data: { 'cmid': $('#memberCmid').val(), 'selectedPage': selectedPage },
        success: function (data) {
            $("#pointsDepositsLoader").remove();
            $("#pointsDepositsContainer").html(data);
        },
        error: function (data) {
            $("#pointsDepositsLoader").remove();
        }
    });
}

function getContacts() {

    $("#viewContactsButton").hide();
    $("#contactsContainer").before(loadingImage.clone().attr('id', 'contactsLoader'));
    $("#contactsContainer").hide();
    
    var selectedPage = 1;

    if ($("#selectedPageContacts").length > 0) {
        selectedPage = $('#selectedPageContacts').val()
    }

    $.ajax({
        type: 'POST',
        url: applicationPath + 'Member/GetContacts',
        data: { 'cmid': $('#memberCmid').val(), 'selectedPage': selectedPage },
        success: function (data) {
            $("#contactsLoader").remove();
            $("#contactsContainer").html(data);
            $("#contactsContainer").show();
        },
        error: function (data) {
            $("#contactsLoader").remove();
            $("#contactsContainer").show();
        }
    });

    return false;
}

function getLoadout() {

    $("#getLoadoutButton").hide();
    $("#loadoutContainer").before(loadingImage.clone().attr('id', 'loadoutLoader'));
    $("#loadoutContainer").hide();

    $.ajax({
        type: 'POST',
        url: applicationPath + 'Member/GetLoadout',
        data: { 'cmid': $('#memberCmid').val() },
        success: function (data) {
            $("#loadoutLoader").remove();
            $("#loadoutContainer").html(data);
            $("#loadoutContainer").show();
            $("#getLoadoutButton").show();
        },
        error: function (data) {
            $("#loadoutLoader").remove();
            $("#loadoutContainer").show();
            $("#getLoadoutButton").show();
        }
    });

    return false;
}

function getDailyStats() {

    $("#dailyStatisticsContainer").before(loadingImage.clone().attr('id', 'dailyStatsLoader'));

    $.ajax({
        type: 'GET',
        url: applicationPath + 'Member/GetDailyStatistics',
        data: { 'cmid': $('#memberCmid').val() },
        success: function (data) {
            $("#dailyStatsLoader").remove();
            $("#dailyStatisticsContainer").html(data);
        },
        error: function (data) {
            $("#dailyStatsLoader").remove();
        }
    });

    return false;
}

function loadIpsAndOtherAccounts() {

    $("#IpsAndOtherAccountsContainer").hide();
    $("#viewIpAndOtherAccountsLoadingImg").show();
    $("#viewIpAndOtherAccountsButton").hide();

    $.ajax({
        type: 'POST',
        url: applicationPath + 'Member/LoadIpsAndOtherAccounts',
        data: { 'cmid': $('#memberCmid').val() },
        success: function (data) {
            $("#viewIpAndOtherAccountsLoadingImg").hide();
            $("#viewIpAndOtherAccountsButton").show();
            $("#IpsAndOtherAccountsContainer").html(data);
            $("#IpsAndOtherAccountsContainer").show();
        },
        error: function (data) {
            $("#viewIpAndOtherAccountsLoadingImg").hide();
            $("#viewIpAndOtherAccountsButton").show();
            $("#IpsAndOtherAccountsContainer").html('');
            $("#IpsAndOtherAccountsContainer").show();
        }
    });
}

function ChangeMemberPointsAndCredits() {
    $.ajax({
        url: applicationPath + "Member/EditMemberPointsAndCredits",
        type: "Post",
        data: $('#editMemberForm').serialize() + '&' + $('#editMemberFormComplement').serialize(),
        success: function (data) {
            if (data.isModified == false) {
                ShowShadowSummaryErrorWithFadeOut(data.message);
            }
            else {
                Shadowbox.close();
                showSummarySuccess(data.message);
                $('#memberCreditsLabel').html(data.newCredits);
                $('#memberPointsLabel').html(data.newPoints);
                getMemberNotes();
            }
        }
    });
}

function setAccessLevel() {

    $("#memberAccessAccessLevel").hide();
    $("#memberAccessAccessLevel").after(loadingImage.clone().attr('id', 'accessLevelLoader'));

    $.ajax({
        url: applicationPath + "Member/SetAccessLevel",
        type: "POST",
        data: { 'cmid': $('#memberCmid').val(), 'accessLevel': +$('#memberAccessAccessLevel').val() },
        success: function (data) {

            $("#memberAccessAccessLevel").show();
            $("#accessLevelLoader").remove();

            if (data.IsModified) {
                showSummarySuccess(data.Message);
            }
            else {
                showSummaryError(data.Message);
            }
        },
        error: function (data) {
            $("#memberAccessAccessLevel").show();
            $("#accessLevelLoader").remove();
            showSummaryError('Error editing the access level.');
        }
    });
}

function ChangeMemberAccess(action){
    $('#action').val(action);
    $.ajax({
        url: applicationPath + "Member/EditMemberAccess",
        type: "POST",
        data: $('#editMemberForm').serialize() + '&' + $('#editMemberFormComplement').serialize(),
        success: function (data) {
            if (data.isModified == false) {
                // mean data.message contains a error message
                if (data.message != undefined && data.message != '') {
                    switch (action) {
                        case 'changeIsAccountDisabled':
                            showSummaryError(data.message);
                            break;
                        case 'changeIsChatDisabled':
                            showSummaryError(data.message);
                            break;
                    }
                }
            }
            else {
                // clear the form edit value
                switch (action) {
                    case 'changeIsAccountDisabled':
                        $('#memberAccessIsAccountDisabled').val(data.newValue);
                        if (data.banUntil != '')
                            $('#memberAccessAccountDisabledUntil').val(data.banUntil);
                        Shadowbox.close();
                        showSummarySuccess(data.message);
                        $('#banStatusLabel').html(data.banUntil);
                        if (data.errorCss == true)
                            $('#banStatusLabel').attr('class', 'error');
                        else
                            $('#banStatusLabel').attr('class', '');
                        getMemberNotes()
                        break;
                    case 'changeIsChatDisabled':
                        $('#memberAccessIsChatDisabled').val(data.newValue);
                        if (data.banUntil != '')
                            $('#memberAccessChatDisabledUntil').val(data.banUntil);
                        Shadowbox.close();
                        showSummarySuccess(data.message);
                        $('#chatBanStatusLabel').html(data.banUntil);
                        if (data.errorCss == true)
                            $('#chatBanStatusLabel').attr('class', 'error');
                        else
                            $('#chatBanStatusLabel').attr('class', '');
                        getMemberNotes();
                        break;
                }
            }
        },
        error: function (data) {
        }
    });

    return false;
}

function ChangeMemberInfo(action) {

    var buttonChangeId = '';
    var buttonCancelId = '';

    switch (action) {
        case 'changeEmail':
            buttonChangeId = '#editEmailChangeButton';
            buttonCancelId = '#editEmailCancelButton';
            break;
        case 'changeName':
            buttonChangeId = '#editMemberNameChangeButton';
            buttonCancelId = '#editMemberNameCancelButton';
            break;
        case 'changePassword':
            break;
        case 'changeIsStatsFrozen':
            break;
        case 'changeEmailForCompletion':
            break;
    }

    if (buttonChangeId != '') {
        $(buttonChangeId).hide();

        if (buttonCancelId != '') {
            $(buttonCancelId).hide();
        }

        $(buttonChangeId).before(loadingImage.clone().attr('id', 'changeMemberInfoLoading'));
    }

    $('#action').val(action);
    var editMemberFormComplementData = '';

    if ($('#editMemberFormComplement').length > 0) {
        editMemberFormComplementData = '&' + $('#editMemberFormComplement').serialize();
    }

    $.ajax({
        url: applicationPath + "Member/Edit",
        type: "POST",
        data: $('#editMemberForm').serialize() + editMemberFormComplementData,
        success: function (data) {

            if (buttonChangeId != '') {
                $(buttonChangeId).show();

                if (buttonCancelId != '') {
                    $(buttonCancelId).show();
                }

                $('#changeMemberInfoLoading').remove();
            }

            if (data.isModified == false) {
                // mean data.message contains a error message
                if (data.message != undefined && data.message != '') {

                    switch (action) {
                        case 'changeEmail':
                            showSummaryError(data.message);
                            break;
                        case 'changeName':
                            showSummaryError(data.message);
                            break;
                        case 'changePassword':
                            showSummaryError(data.message);
                            break;
                        case 'changeIsStatsFrozen':
                            ShowShadowSummaryErrorWithFadeOut(data.message);
                            break;
                        case 'changeEmailForCompletion':
                            ShowShadowSummaryErrorWithFadeOut(data.message);
                            break;

                    }
                }
            }
            else {
                // clear the form edit value
                switch (action) {
                    case 'changeEmail':
                        showSummarySuccess(data.message);
                        var newMemberEmail = htmlEncode($('#newMemberEmail').val());
                        $('#memberEmail').val(newMemberEmail);
                        $('.memberEmailLabel').html(newMemberEmail);
                        $('#explanationMemberEmail').val('');
                        TogglePanel('#EditEmailPanel');
                        getMemberNotes();
                        break;
                    case 'changeEmailForCompletion':
                        showSummarySuccess(data.message);
                        window.setTimeout('location.reload()', 1000);
                        break;
                    case 'changeName':
                        showSummarySuccess(data.message);
                        var newMemberName = htmlEncode($('#newMemberName').val());
                        $('#memberName').val(newMemberName);
                        $('.memberNameLabel').html(newMemberName);
                        $('#memberNameHeaderH2').html(newMemberName);
                        $('#memberNameStats').html(newMemberName);
                        $('#explanationMemberName').val('');
                        TogglePanel('#EditUsernamePanel');
                        getMemberNotes();
                        break;
                    case 'changePassword':
                        showSummarySuccess(data.message);
                        $('#pwd').val('');
                        $('#pwd2').val('');
                        TogglePanel('#newPwd');
                        break;
                }
            }
        },
        error: function (data) {
            showSummaryError('error :(');

            if (buttonChangeId != '') {
                $(buttonChangeId).show();

                if (buttonCancelId != '') {
                    $(buttonCancelId).show();
                }

                $('#changeMemberInfoLoading').remove();
            }
        }
    });

    return false;
}

function resetPassword() {
    $('#resetPasswordLoadingImg').show();
    $.ajax({
        type: 'POST',
        url: applicationPath + 'Member/ResetPassword',
        data: $("#editMemberForm").serialize(),
        success: function (data) {
            $('#resetPasswordLoadingImg').hide();
            showSummarySuccess(data.message);
        },
        error: function (data) {
            $('#resetPasswordLoadingImg').hide();
            showSummarySuccess("The password couldn't be reset.");
        }
    });
}

function selectCurrentAdmin(adminNameDdl) {
    var adminCmid = getAdminCmid();

    if (adminCmid != '') {
        $('#' + adminNameDdl).val(adminCmid);
    }
}

function getMemberNotes() {
    $('#memberNotesSubDiv').hide();
    $('#memberNotesLoadingImg').show();

    $.ajax({
        type: 'POST',
        url: applicationPath + 'Member/GetMemberNotes',
        data: $("#editMemberForm").serialize(),
        success: function (data) {
            $('#memberNotesSubDiv').html(data);
            $('#memberNotesLoadingImg').hide();
            $('#memberNotesSubDiv').show();
        },
        error: function (data) {
            $('#memberNotesLoadingImg').hide();
            $('#memberNotesSubDiv').show();
        }
    });
}

function addMemberNote() {
    $('#addNoteDiv').hide();
    $('#addNoteLoadingImg').show();

    $.ajax({
        type: 'POST',
        url: applicationPath + "Member/AddMemberNote",
        data: $('#editMemberForm').serialize() + '&' + $('#addNoteForm').serialize(),
        success: function (data) {
            $('#addNoteLoadingImg').hide();
            $("#addNoteDescription").val('');
            getMemberNotes();
        },
        error: function (data) {
            $('#addNoteLoadingImg').hide();
            $("#addNoteDescription").val('');
        }
    });
}

function deleteMemberNote(noteId, loaderImage) {
    if (confirm("Are your sure you want to delete this note?")) {

        $('#deleteMemberNote' + noteId + 'Link').hide();
        $('#deleteMemberNote' + noteId + 'Td').append(loaderImage.attr('id', 'memberNoteLoading' + noteId + 'Img'));

        $.ajax({
            type: 'POST',
            url: applicationPath + "Member/DeleteMemberNote",
            data: { 'noteId': noteId },
            success: function (data) {
                showSummarySuccess('The note was deleted successfully');
                getMemberNotes();
            },
            error: function (data) {
                showSummaryError('An error happened while deleting the note');
                $('#deleteMemberNote' + noteId + 'Link').show();
                $('#memberNoteLoading' + noteId + 'Img').remove();
            }
        });
    }
}

function displayEditMemberNote(noteId, sourceCmid, actionTypeId, reason, adminNamesDdl, actionTypesDdl) {
    $('#reason' + noteId + 'Td').hide();
    $('#reason' + noteId + 'Td').after('<td id="editReason' + noteId + 'Td"><textarea id="reason' + noteId + '" maxlength="1000" rows="5" cols="40">' + reason + '</textarea></td>');
    $('#sourceName' + noteId + 'Td').hide();
    $('#sourceName' + noteId + 'Td').after('<td id="editSourceName' + noteId + 'Td"></td>');
    $('#editSourceName' + noteId + 'Td').html(adminNamesDdl.clone().attr('id', 'adminName' + noteId).val(sourceCmid));
    $('#actionType' + noteId + 'Td').hide();
    $('#actionType' + noteId + 'Td').after('<td id="editActionType' + noteId + 'Td"></td>');
    $('#editActionType' + noteId + 'Td').html(actionTypesDdl.clone().attr('id', 'actionType' + noteId).val(actionTypeId));
    $('#deleteMemberNote' + noteId + 'Td').hide();
    $('#editMemberNote' + noteId + 'Td').attr('colspan', '2');
    $('#editMemberNote' + noteId + 'Link').hide();
    $('#editMemberNote' + noteId + 'Span').html('<a href="#" onclick="editMemberNote(' + noteId + ', memberNoteLoadingImage); return false;">Ok</a> <a href="#" onclick="cancelEditMemberNote(' + noteId + '); return false;">Cancel</a>');
}

function cancelEditMemberNote(noteId) {
    $('#editReason' + noteId + 'Td').remove();
    $('#reason' + noteId + 'Td').show();
    $('#editSourceName' + noteId + 'Td').remove();
    $('#sourceName' + noteId + 'Td').show();
    $('#editActionType' + noteId + 'Td').remove();
    $('#actionType' + noteId + 'Td').show();
    $('#deleteMemberNote' + noteId + 'Td').show();
    $('#editMemberNote' + noteId + 'Td').attr('colspan', '1');
    $('#editMemberNote' + noteId + 'Link').show();
    $('#editMemberNote' + noteId + 'Span').html('');
}

function editMemberNote(noteId, loaderImage) {

    $('#editMemberNote' + noteId + 'Span').hide();
    $('#editMemberNote' + noteId + 'Td').append(loaderImage.attr('id', 'editMemberNoteLoading' + noteId + 'Img'));

    $.ajax({
        type: 'POST',
        url: applicationPath + "Member/EditMemberNote",
        data: { 'noteId': noteId,
                'sourceCmid': $('#adminName' + noteId).val(),
                'actionType': $('#actionType' + noteId).val(),
                'reason': $('#reason' + noteId).val()
        },
        success: function (data) {
            showSummarySuccess('The note was edited successfully');
            getMemberNotes();
        },
        error: function (data) {
            showSummaryError('An error happened while editing the note');
            $('#editMemberNote' + noteId + 'Span').show();
            $('#editMemberNoteLoading' + noteId + 'Img').remove();
        }
    });
}

function displayBanIpPopUp(ipAddressToBan, targetCmid, clickedElement, banMode, actionSuccessCallback) {

    $("#" + clickedElement).hide();
    $("#" + clickedElement).after(loadingImage.clone().attr('id', 'displayBanIpPopUpLoader'));

    var title = 'Ban IP';

    if (banMode == 'modify') {
        title = 'Unban IP';
    }

    $.ajax({
        type: 'GET',
        url: applicationPath + 'Member/LoadIpBan',
        success: function(data) {
            openInShadowbox(data, title, function () { }, function () { prepareFormToBanIp(ipAddressToBan, targetCmid, banMode, actionSuccessCallback); });
            $("#displayBanIpPopUpLoader").remove();
            $("#" + clickedElement).show();
        },
        error: function () {
            $("#displayBanIpPopUpLoader").remove();
            $("#" + clickedElement).show();
        }
    });
}

function prepareFormToBanIp(ipAddressToBan, targetCmid, banMode, actionSuccessCallback) {

    $("#ipBanIpToBan").val(ipAddressToBan);
    $("#ipBanTargetCmid").val(targetCmid);
    $("#ipToBanSpan").html(ipAddressToBan);

    $('#ipBanButton').click(function () {
        banIp(actionSuccessCallback);
        return false;
    });

    if (banMode == 'ban') {
        $("#unbanSpan").html('ban');
        $("#ModifyIpBanP").hide();
        $('#unbanIpCheckbox').removeAttr('checked');
        $("#banP").show();
        $("#ipBanButton").val('Ban IP');
    }
    else if (banMode == 'modify') {
        $("#unbanSpan").html('modify the ban of');
        $("#ModifyIpBanP").show();
        $('#unbanIpCheckbox').removeAttr('checked');
        $("#banP").show();
        $("#ipBanButton").val('Ban IP');
    }
    else if (banMode == 'unban') {
        $("#unbanSpan").html('unban');
        $("#ModifyIpBanP").hide();
        $('#unbanIpCheckbox').attr('checked', 'checked');
        $("#banP").hide();
        $("#ipBanButton").val('Unban IP');
    }

    selectCurrentAdmin('ipBanAdminNames');
}

function switchIpBanMode() {
    if ($('#unbanIpCheckbox').attr('checked')) {
        $("#banP").hide();
        $("#unbanSpan").html('unban');
        $("#ipBanButton").val('Unban IP');
    }
    else {
        $("#banP").show();
        $("#unbanSpan").html('modify the ban of');
        $("#ipBanButton").val('Ban IP');
    }
}

function banIp(actionSuccessCallback) {
    $('#ipBanButton').hide();
    $("#ipBanButton").after(loadingImage.clone().attr('id', 'ipBanLoader'));

    $.ajax({
        type: 'POST',
        url: applicationPath + "Member/BanIp",
        data: $('#banIpForm').serialize(),
        success: function (data) {
            Shadowbox.close();
            showSummarySuccess('The action on the IP was taken successfuly.');
            actionSuccessCallback();
        },
        error: function (data) {
            Shadowbox.close();
            showSummaryError('An error happened');
        }
    });
}

function loadBannedIps() {

    $("#BannedIpsContainer").before(loadingImage.clone().attr('id', 'bannedIpsLoader'));

    var selectedPage = 1;

    if ($("#selectedPageBannedIps").length > 0) {
        selectedPage = $('#selectedPageBannedIps').val()
    }

    $.ajax({
        type: 'POST',
        url: applicationPath + 'Member/LoadBannedIps',
        data: $('#loadBannedIpsForm').serialize() + '&selectedPage=' + selectedPage,
        success: function (data) {
            $("#bannedIpsLoader").remove();
            $("#BannedIpsContainer").html(data);
        },
        error: function (data) {
            $("#bannedIpsLoader").remove();
        }
    });
}

function loadMember() {

    $("#searchAnotherUserButton").hide();
    $("#searchAnotherUserButton").before(loadingImage.clone().attr('id', 'searchAnotherUserLoader'));

    $.ajax({
        type: 'POST',
        url: applicationPath + 'Member/GetUserCmid',
        data: { 'userName' : $("#searchByNameTextBox").val(),
                'userEmail' : $("#searchByEmailTextBox").val(),
                'facebookId' : $("#searchByFacebookIdTextBox").val()
        },
        success: function (data) {

            if (data.MemberCmid == '0') {
                showSummaryError('No user matching');
                $('#searchAnotherUserLoader').remove();
                $('#searchAnotherUserButton').show();
                $("#searchByEmailTextBox").val('');
                $("#searchByFacebookIdTextBox").val('');
            }
            else {
                $('#searchAnotherUserLoader').remove();
                showSummarySuccess('User found, reloading the page now...');
                window.location = applicationPath + 'Member/See?Cmid=' + data.MemberCmid;
            }
        },
        error: function (data) {
            showSummaryError('An error happened');
            $('#searchAnotherUserLoader').remove();
            $('#searchAnotherUserButton').show();
        }
    });

    return false;
}

function loadPrivateMessageSender() {
    $.ajax({
        type: 'GET',
        url: applicationPath + 'Member/LoadPrivateMessageSender',
        data: { 'memberCmid': $('#memberCmid').val() },
        success: function (data) {
            Shadowbox.open({
                content: data,
                player: "html",
                title: "Send a Private Message",
                height: 450
            });
        }
    });
}

function displayPrivateMessageTemplate(templateId) {
    var subject = '';
    var content = '';

    if (templateId == 1) {
        subject = 'Speed hacking warning';
        content = 'Our anti cheat technology detected that you used speed hacking. This is our final warning, if we ever catch you again glitching or cheating we\'ll ban your account permanently.';
    }
    if (templateId == 2) {
        subject = 'Getting into the walls warning';
        content = 'You got into the walls of Gideon Towers. This is our final warning, if we ever catch you again glitching or cheating we\'ll ban your account permanently.';
    }
    else if (templateId == 3) {
        subject = 'Inappropriate name';
        content = 'Your name was deemed inappropriate and action was taken by an admin to change it to something guaranteed to be non offensive. If you wish to choose your name there is a \'Name Change\' item available in-game in the shop.';
    }
    else if (templateId == 4) {
        subject = 'Payment reversal';
        content = 'We noticed that you reverted a $INSERT-AMOUNT-HERE payment. Unless you settle this payment we\'ll have to ban your account permanently.';
    }
    else if (templateId == 5) {
        subject = 'Map mixing warning';
        content = 'You mixed map. This is our final warning, if we ever catch you again glitching or cheating we\'ll ban your account permanently.';
    }
    if (templateId == 6) {
        subject = 'xp farming warning';
        content = 'Our anti cheat technology detected that you used xp farming with your account XXX. This is our final warning, if we ever catch you again glitching or cheating we\'ll ban your account permanently.';
    }

    $("#subjectTextBox").val(subject);
    $("#contentTextArea").val(content);
}

function sendPrivateMessage() {

    $("#sendPrivateMessageButton").hide();
    $("#cancelSendPrivateMessageButton").hide();
    $("#sendPrivateMessageButton").before(loadingImage.clone().attr('id', 'sendPrivateMessageLoader'));

    $.ajax({
        type: 'POST',
        url: applicationPath + 'Member/SendPrivateMessage',
        data: $('#sendPrivateMessagForm').serialize(),
        success: function (data) {
            if (data.IsSent) {
                $('#sendPrivateMessageLoader').remove();
                Shadowbox.close();
                showSummarySuccess('The message was sent');
            }
            else {
                $('#sendPrivateMessageLoader').remove();
                Shadowbox.close();
                showSummaryError('The message was not sent');
            }
        },
        error: function (data) {
            $('#sendPrivateMessageLoader').remove();
            Shadowbox.close();
            showSummaryError('An error happened');
        }
    });
}

function displayNoteTemplates() {
    var noteType = $("#addNoteType").val();

    $("#warningTemplates").hide();

    if (noteType == "4") {
        $("#warningTemplates").show();
    }
}

function displayNoteTemplate(templateId) {
    var description = '';

    if (templateId == 1) {
        description = 'Speed hacking';
    }
    if (templateId == 2) {
        description = 'Map mixing';
    }
    if (templateId == 3) {
        description = 'Xp farming';
    }

    $("#addNoteDescription").val(description);
}

function displayBanTemplate(templateId) {
    var description = '';

    if (templateId == 1) {
        description = 'Speed hacking';
        $('#newNemberAccessIsAccountDisabled3').attr('checked', true);
    }
    else if (templateId == 2) {
        description = 'Hiding in the walls of GT';
        $('#newNemberAccessIsAccountDisabled2').attr('checked', true);
        $('#disablingDuration').val('7');
        displayBanDate($('#disablingDuration').val());
    }
    else if (templateId == 3) {
        description = 'Crude language';
    }
    else if (templateId == 4) {
        description = 'Map mixing';
        $('#newNemberAccessIsAccountDisabled2').attr('checked', true);
        $('#disablingDuration').val('7');
        displayBanDate($('#disablingDuration').val());
    }
    else if (templateId == 5) {
        description = 'xp farming';
        $('#newNemberAccessIsAccountDisabled3').attr('checked', true);
    }
    else if (templateId == 6) {
        description = 'payment reversal';
        $('#newNemberAccessIsAccountDisabled3').attr('checked', true);
    }

    $("#explanationTextBox").val(description);
}

function displayBanDate(banDuration) {
    var d = new Date();
    var newDate = d.getDate() + parseInt(banDuration);
    d.setDate(newDate);
    $("#banUntilSpan").html(d.getUTCMonth() + 1 + "/" + d.getUTCDate());
}

function displayNameChangeTemplate(editMemberNameType) {
    var description = '';

    if (editMemberNameType == 6) {
        description = 'Per user request';
    }
    else if (editMemberNameType == 7) {
        description = 'Your name was deemed inappropriate and action was taken by an admin to change it to something guaranteed to be non offensive. If you wish to choose your name there is a \'Name Change\' item available in-game in the shop.';
    }

    $("#explanationMemberName").val(description);
}

/* Custom queries */

function getDailyFarmers() {

    $("#xpFarmersForm").hide();
    $("#xpFarmersContainer").hide();
    $("#xpFarmersForm").before(loadingImage.clone().attr('id', 'getXpFarmersLoader').attr('style', 'display:block; margin:auto;'));

    $.ajax({
        type: 'POST',
        url: applicationPath + 'Member/GetXpFarmers',
        data: $("#xpFarmersForm").serialize(),
        success: function (data) {
            $("#xpFarmersContainer").html(data);
            $("#xpFarmersContainer").show();
            $('#getXpFarmersLoader').remove();
            $("#xpFarmersForm").show();
        },
        error: function (data) {
            $('#getXpFarmersLoader').remove();
            $("#xpFarmersForm").show();
            showSummaryError('An error happened');
        }
    });
}

/* Custom queries end */

/* See user */

function setLevel(cmid, newLevel) {

    $("#setLevelButton").hide();
    $("#setLevelButton").before(loadingImage.clone().attr('id', 'setLevelLoader'));

    $.ajax({
        type: 'POST',
        url: applicationPath + 'Member/SetLevel',
        data: { 'cmid': cmid, 'level': newLevel },
        success: function (data) {

            $("#setLevelButton").show();
            $("#setLevelLoader").remove();

            if (data.IsModified) {
                $("#xpSpan").html(data.NewXp);
                $("#levelSpan").html(newLevel);
                showSummarySuccess('The player level was updated');
            }
            else {
                showSummaryError('The player level was not updated (you probbaly used a non existing level)');
            }
        },
        error: function (data) {
            $("#setLevelButton").show();
            $("#setLevelLoader").remove();
            showSummaryError('An error happened');
        }
    });
}

function toggleMoreStatistics() {
    if (typeof $('#PlayerStatisticPanel').attr('oldHeight') === "undefined") {
        $('#PlayerStatisticPanel').attr('oldHeight', $('#PlayerStatisticPanel').height());
        $('#PlayerStatisticPanel').height('');
    }
    else {
        $('#PlayerStatisticPanel').height($('#PlayerStatisticPanel').attr('oldHeight') + 'px');
        $('#PlayerStatisticPanel').removeAttr("oldHeight")
    }
}

/* See user end */