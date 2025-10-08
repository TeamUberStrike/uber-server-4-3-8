/*
 * dropDownLists: array of the dropdownlist id to refresh
 * formIds: array of forms id containing the dropdownlist (will be hidden while refeshing, should contain dropDownLists to hide them too)
 * endpointUrl: endpoint to get new data for the dropDownLists (example: '/StatisticsRegions/GetCountriesByRevenueDropDownList')
 * updateChartFunction: functions to be called after refreshing (for example display charts)
 */
function updateStatsMetrics(dropDownLists, formIds, endpointUrl, updateChartFunction) {
    var selectedIndexes = new Array();

    for (var i = 0; i < dropDownLists.length; i++) {
        selectedIndexes[i] = $("#" + dropDownLists[i]).val();
    }

    for (var i = 0; i < formIds.length; i++) {
        $("#" + formIds[i]).hide();
        $("#" + formIds[i]).before(loadingImage.clone().attr('id', formIds[i] + 'Loader'));
    }

    $.ajax({
        type: 'POST',
        dataType: 'json',
        url: applicationPath + endpointUrl,
        data: $("#StatsCalendar").serialize(),
        success: function (data) {

            for (var i = 0; i < dropDownLists.length; i++) {
                $("#" + dropDownLists[i]).empty();
            }

            $.each(data, function (index, item) {

                for (var i = 0; i < dropDownLists.length; i++) {
                    $("#" + dropDownLists[i]).append("<option value='" + item.Value + "'>" + item.Text + "</option>");
                }

            });

            for (var i = 0; i < dropDownLists.length; i++) {
                if ($('#' + dropDownLists[i] + ' option[value=' + selectedIndexes[i] + ']').length > 0) {
                    $("#" + dropDownLists[i]).val(selectedIndexes[i]);
                }
                else if ($("#" + dropDownLists[i] + ' option').length > 1) {
                    $('#' + dropDownLists[i] + ' option:eq(1)').attr('selected', 'selected')
                }
                else if ($("#" + dropDownLists[i] + ' option').length == 1) {
                    $('#' + dropDownLists[i] + ' option:eq(0)').attr('selected', 'selected')
                }
            }

            for (var i = 0; i < formIds.length; i++) {
                $("#" + formIds[i]).show();
                $("#" + formIds[i] + 'Loader').remove();
            }

            updateChartFunction();
        },
        error: function (data) {

            for (var i = 0; i < formIds.length; i++) {
                $("#" + formIds[i]).show();
                $("#" + formIds[i] + 'Loader').remove();
            }

            updateChartFunction();
        }
    });
}

/*
 * Works only for Json returns with two properties called ColumnOne and ColumnTwo
 *
 * tableDiv: the div that will contain the generated DOM (will be hidden during update)
 * loadingImage: the image id that will be displayed while updating
 * endpointUrl: endpoint to get new data for the table (example: '/StatisticsRegions/GetCountriesByRevenueList')
 * columnOneTitle: the title of the first column
 * columnTwoTitle: the title of the second column
 */
function updateStatsMetricsTable(tableDiv, loadingImage, endpointUrl, columnOneTitle, columnTwoTitle) {
    $('#' + tableDiv).hide();
    $('#' + loadingImage).show();

    $.ajax({
        type: 'POST',
        dataType: 'json',
        url: applicationPath + endpointUrl,
        data: $("#StatsCalendar").serialize(),
        success: function (data) {

            $("#" + tableDiv).html('');

            var totalCount = 0;

            $.each(data, function (index, item) {
                totalCount += Number(item.ColumnTwo.replace(/[^0-9\.]+/g, ""));
            });

            var tableHeader = '<table class="left" style="margin:0 10px 0 10px;"><tr><td></td><td style="font-weight:bold;">' + columnOneTitle + '</td><td style="font-weight:bold;">' + columnTwoTitle + '</td><td style="font-weight:bold;">%</td></tr>';
            var tableContent = '';

            var columnCount = 4;
            var newTableIndex = Math.round(data.length / columnCount);

            $.each(data, function (index, item) {
                if (index % newTableIndex == 0) {
                    if (index != 0) {
                        tableContent += '</table>';
                    }
                    tableContent += tableHeader;
                }

                tableContent += '<tr><td>' + (index + 1) + '</td><td>' + item.ColumnOne + '</td><td style="text-align:right;">' + item.ColumnTwo + '</td><td style="text-align:right;">' + roundNumber(Number(item.ColumnTwo.replace(/[^0-9\.]+/g, "")) / totalCount * 100, 2) + '</td></tr>';
            });

            tableContent += '</table>';

            $('#' + tableDiv).html(tableContent);

            $('#' + loadingImage).hide();
            $('#' + tableDiv).show();
        },
        error: function (data) {
            $('#' + loadingImage).hide();
            $('#' + tableDiv).show();
        }
    });
}

/* Unity Conversion Funnel */

function getTrackingStepsFromCookieOrField(trackingCookieName) {
    var cookieTrackingId = getCookie(trackingCookieName);

    if (cookieTrackingId.length > 0 && $("#trackingId").val() == '') {
        readTrackingCookie(trackingCookieName)
    }
    else if ($("#trackingId").val() != '') {
        getTrackingSteps($("#trackingId").val())
    }

    return false;
}

function getTrackingSteps(trackingId) {

    $("#trackingStepsDiv").html('');
    $("#getTrackingStepsButton").hide();
    $("#trackingIdLoadingImg").show();

    $.ajax({
        type: 'POST',
        url: applicationPath + 'StatisticsUnityConversionFunnel/GetSteps',
        data: { 'trackingId': trackingId },
        success: function (data) {
            $("#trackingStepsDiv").html(data);
            $("#getTrackingStepsButton").show();
            $("#trackingIdLoadingImg").hide();
        },
        error: function (data) {
            $("#getTrackingStepsButton").show();
            $("#trackingIdLoadingImg").hide();
        }
    });

    return false;
}

function readTrackingCookie(trackingCookieName) {
    var cookieTrackingId = getCookie(trackingCookieName);
    if (cookieTrackingId.length > 0) {
        $("#cookieDetectedDiv").show();
        $("#trackingId").val(cookieTrackingId);
        $("#getTrackingStepsButton").val("Reload");
        getTrackingSteps(cookieTrackingId);
    }
    else {
        $("#readTrackingCookieSpan").show();
    }

    return false;
}

function deleteTrackingCookies(trackingCookieName, trackingStateCookieName) {
    var cookieTrackingId = getCookie(trackingCookieName);

    setCookie(trackingCookieName, "", -1);
    setCookie(trackingStateCookieName, "", -1);

    if (cookieTrackingId.length > 0 && cookieTrackingId == $("#trackingId").val()) {
        $("#trackingId").val('');
        $("#trackingStepsDiv").html('');
        $("#readTrackingCookieSpan").show();
    }
}

/* End Unity Conversion Funnel */

/* Tutorial Conversion Funnel */

function getTutorialSteps() {

    $("#tutorialStepsContainer").html('');
    $("#getTutorialStepsButton").hide();
    $("#getTutorialStepsButton").before(loadingImage.clone().attr('id', 'tutorialStepsLoader'));

    $.ajax({
        type: 'POST',
        url: applicationPath + 'Statistic/GetTutorialSteps',
        data: { 'cmid': $("#cmid").val() },
        success: function (data) {
            $("#tutorialStepsContainer").html(data);
            $("#getTutorialStepsButton").show();
            $("#tutorialStepsLoader").remove();
        },
        error: function (data) {
            showSummaryError('Error getting the tutorial steps');
            $("#getTutorialStepsButton").show();
            $("#tutorialStepsLoader").remove();
        }
    });

    return false;
}

/* End Tutorial Conversion Funnel */

function getLatestPayment(providerId, latestPaymentCount) {

    $("#latestPaymentDiv").html('');
    $("#getLatestPaymentsButton").hide();
    $("#latestPaymentsLoadingImg").show();

    $.ajax({
        type: 'POST',
        url: applicationPath + 'StatisticsPaymentProviders/GetLatestPayments',
        data: { 'providerId': providerId, 'paymentsCount': latestPaymentCount },
        success: function (data) {
            $("#latestPaymentDiv").html(data);
            $("#getLatestPaymentsButton").show();
            $("#latestPaymentsLoadingImg").hide();
        },
        error: function (data) {
            $("#getLatestPaymentsButton").show();
            $("#latestPaymentsLoadingImg").hide();
        }
    });
}

function getCreditDeposit(providerId, transactionId) {

    $("#creditDepositDiv").html('');
    $("#getCreditDepositButton").hide();
    $("#creditDepositLoadingImg").show();

    $.ajax({
        type: 'POST',
        url: applicationPath + 'StatisticsPaymentProviders/GetCreditDeposit',
        data: { 'providerId': providerId, 'transactionId': transactionId },
        success: function (data) {
            $("#creditDepositDiv").html(data);
            $("#getCreditDepositButton").show();
            $("#creditDepositLoadingImg").hide();
        },
        error: function (data) {
            $("#getCreditDepositButton").show();
            $("#creditDepositLoadingImg").hide();
        }
    });
}

function getTotalRevenue() {

    $("#totalPaymentSpan").html('');

    $.ajax({
        type: 'POST',
        url: applicationPath + 'Statistic/GetTotalRevenue',
        data: $("#StatsCalendar").serialize(),
        success: function (data) {
            $("#totalPaymentSpan").html(data);
        },
        error: function (data) {
        }
    });
}

function getTotalRevenueProvider(providerId) {

    $("#totalPaymentSpan").html('');

    $.ajax({
        type: 'POST',
        url: applicationPath + 'StatisticsPaymentProviders/GetTotalRevenue',
        data: $("#StatsCalendar").serialize() + '&providerId=' + providerId,
        success: function (data) {
            $("#totalPaymentSpan").html(data);
        },
        error: function (data) {
        }
    });
}