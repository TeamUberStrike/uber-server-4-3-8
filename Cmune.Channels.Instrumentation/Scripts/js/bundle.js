var bundleItemNumber = 0;

function AddBundleItemField(value, dropDownListElement) {
    var htmlElement = "";
    htmlElement += '<div>';
    htmlElement += 'Item Id: <input id="NewItem' + bundleItemNumber + '" name="NewItem' + bundleItemNumber + '" value="' + value + '" style="width:30px;" />';
    htmlElement += ' <select id="NewDuration' + bundleItemNumber + '" name="NewDuration' + bundleItemNumber + '" >';
    for (var i in dropDownListElement) {
        htmlElement += '<option value="' + dropDownListElement[i].Value + '" ' + (dropDownListElement[i].Selected != false ? 'selected="selected"' : '') + ' >' + dropDownListElement[i].Text + '</option>';
    }
    htmlElement += 'Item Id: <input id="NewAmount' + bundleItemNumber + '" name="NewAmount' + bundleItemNumber + '" value="' + 0 + '" style="width:30px;" />';
    htmlElement += '</select>';
    htmlElement += '</div>';
    if (bundleItemNumber < 15) {
        $("#BundleItemsField").append(htmlElement);
        bundleItemNumber++;
    }
}

function LoadAddEditBundleForm(bundleId) {
    bundleItemNumber = 0;
    $.ajax({
        type: 'POST',
        url: applicationPath + 'Management/LoadAddEditBundleForm',
        data: { 'bundleId': bundleId },
        success: function (data) {
            Shadowbox.open({
                content: data,
                player: "html",
                title: bundleId > 0 ? "Edit" : "Add",
                height: 850,
                width: 550
            });
        },
        dataType: "html"
    });
}

function AddEditBundleGo() {

    $('#bundleSubmitButton').hide();
    $('#bundleSubmitButton').before(loadingImage.clone().attr('id', 'addEditBundleLoader'));

    $.ajax({
        type: 'POST',
        url: applicationPath + 'Management/AddEditBundle',
        data: $('#addEditBundleForm').serialize(),
        success: function (data) {

            if (data.isAddOrEdit) {
                $('#addEditBundleLoader').remove();
                Shadowbox.close();
                showSummarySuccess('Edited bundle');
                getBundles();
            }
            else {
                $('#addEditBundleLoader').remove();
                $('#bundleSubmitButton').show();
                showSummaryError(data.message);
            }
        },
        error: function (data) {
            $('#addEditBundleLoader').remove();
            Shadowbox.close();
            showSummaryError('Error :(');
        }
    });
}

function DeleteBundleGo(bundleId) {
    if (confirm("Are you sure to delete this pack ?")) {
        $.ajax({
            type: 'POST',
            url: applicationPath + 'Management/DeleteBundle',
            data: { "bundleId": bundleId },
            success: function (data) {
                if (data.isDeleted) {
                    showSummarySuccess('Deleted bundle');
                    getBundles();
                }
                else {
                    showSummaryError("Error occured");
                }
            }
        });
    }
}

function getBundlesOnly() {
    if ($('#IsPack').attr('checked')) {
        $('#IsPack').attr('checked', false)
    }

    getBundles();
}

function getPacksOnly() {
    if ($('#IsBundle').attr('checked')) {
        $('#IsBundle').attr('checked', false)
    }

    getBundles();
}

function getBundles() {

    $('#bundleContainer').hide();
    $('#bundleContainer').before(loadingImage.clone().attr('id', 'getBundlesLoader'));

    $.ajax({
        type: 'POST',
        url: applicationPath + "Management/GetBundles",
        data: { 'onlyOnSale': $('#IsOnSale').attr('checked'), 'onlyBundle': $('#IsBundle').attr('checked'), 'onlyPack': $('#IsPack').attr('checked') },
        success: function (data) {
            $('#bundleContainer').html(data);
            $('#bundleContainer').show();
            $('#getBundlesLoader').remove();
        },
        error: function (data) {
            showSummaryError('We couldn\'t get the bundles :(');
            $('#bundleContainer').show();
            $('#getBundlesLoader').remove();
        }
    });
}

function selectAllChannels() {
    $('input[id][id$=ActiveOnChannel]').attr('checked', true);
}