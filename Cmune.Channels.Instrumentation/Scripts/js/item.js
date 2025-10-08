function getItems() {
    $.ajax({
        type: 'POST',
        url: applicationPath + 'Item/GetItems',
        data: $("#searchItemForm").serialize(),
        success: function (data) {
            $('#itemsResultContainer').html(data);
        }
    });
}

function refreshItemClassDropDownList(sourceType, destClass, withAllSelection) {
    if (withAllSelection == undefined)
        withAllSelection = true;
    var val = $(sourceType).val();
    $.ajax({
        type: 'GET',
        url: applicationPath + 'Item/HttpGetItemClassList',
        data: { 'typeIdstr': val, 'withAllSelection': withAllSelection },
        success: function (data) {
            var itemClassDropDownListHtml = '';
            for (i in data.itemClassList) {
                itemClassDropDownListHtml += '<option value="' + data.itemClassList[i].Value + '" ' + (data.itemClassList[i].selected == true ? 'selected="selected"' : '') + '>'
                 + data.itemClassList[i].Text + '</option>';
            }
            $(destClass).html(itemClassDropDownListHtml);
        }
    });
}

function LoadAddOrEditForm(action, itemId) {
    $.ajax({
        type: 'GET',
        url: applicationPath + 'Item/LoadAddOrEditForm',
        data: { 'actionTodo': action, 'itemId': itemId },
        success: function (data) {
            Shadowbox.open({
                content: data,
                player: "html",
                title: action + " item",
                width: 900,
                height: 630, // we set this height a little bigger for Google Chrome
                options: { onFinish: function () {
                    displayCorrectComplementaryForm("#ItemTypeId");
                } 
                }
            });
        }
    });
}

function AddItem() {
    $.ajax({
        type: "POST",
        url: applicationPath + "Item/Add",
        data: $('#addOrEditItemForm').serialize(),
        success: function (data) {
            if (!data.isInserted) {
                ShowShadowSummaryError(data.message);
            }
            else {
                Shadowbox.close();
                showSummarySuccess(data.message);
                getItems();
            }
        },
        error: function (data) {
            Shadowbox.close();
            showSummaryError(data.message);
        }
    });
}

function EditItem() {
    $.ajax({
        type: "POST",
        url: applicationPath + "Item/Edit",
        data: $('#addOrEditItemForm').serialize(),
        success: function (data) {
            if (!data.isModified) {
                ShowShadowSummaryError(data.message);
            }
            else {
                Shadowbox.close();
                showSummarySuccess(data.message);
                getItems();
            }
        },
        error: function (data) {
            Shadowbox.close();
            showSummaryError(data.message);
        }
    });
}

function checkIfFreeItem() {
    if ($('#PointsPerDayShopTextBox').val() == '0') {
        $('#freeItemTr').show();
    }
    else {
        $('#freeItemTr').hide();
    }
}


var customPropertyI;
function displayCustomProperties(customProperties, divArea) {
    customPropertyI = 0;
    if (customProperties != "") {
        splittedCustomProperties = customProperties.split('&')
        for (var i in splittedCustomProperties) {
            property = splittedCustomProperties[i].split('=');
            divArea.append("<div><input type='text' name='CustomPropertyName" + customPropertyI + "' value='" + property[0] + "' class='grey' readonly='readonly'/><input type='text'  name='CustomPropertyValue" + customPropertyI + "' value='" + property[1] + "' /></div>");
            customPropertyI++;
        }
    }
}

function addCustomPropertyWeapon() {
    $("#customPropertiesFormWeapon").append("<div><input type='text' name='CustomPropertyName" + customPropertyI + "' value='' /><input type='text'  name='CustomPropertyValue" + customPropertyI + "' value='' /></div>");
    customPropertyI++;
}

function addCustomPropertyQuickItem() {
    $("#customPropertiesFormQuick").append("<div><input type='text' name='CustomPropertyName" + customPropertyI + "' value='' /><input type='text'  name='CustomPropertyValue" + customPropertyI + "' value='' /></div>");
    customPropertyI++;
}

function displayCorrectComplementaryForm(elementItemTypeId) {
    // hide all complementary form
    $('#weaponComplementaryFormContainer').hide();
    $('#gearComplementaryFormContainer').hide();
    $('#quickUseComplementaryFormContainer').hide();
    var selectedTypeValue = parseInt($(elementItemTypeId).val());
    switch (selectedTypeValue) {
        case shopItemTypeFunctional:
            break;
        case shopItemTypeGear:
            $('#gearComplementaryFormContainer').show();
            displayCustomProperties($("#CustomPropertiesHidden").val(), $("#customPropertiesFormGear"));
            break;
        case shopItemTypeQuickUse:
            $('#quickUseComplementaryFormContainer').show();
            displayCustomProperties($("#CustomPropertiesHidden").val(), $("#customPropertiesFormQuick"));
            break;
        case shopItemTypeSpecial:
            break;
        case shopItemTypeWeapon:
            $('#weaponComplementaryFormContainer').show();
            displayCustomProperties($("#CustomPropertiesHidden").val(), $("#customPropertiesFormWeapon"));
            break;
        case shopItemTypeWeaponMod:
            break;
    }
}

function attributeItemsToMembersGo() {
    $.ajax({
        type: "POST",
        url: applicationPath + "Item/AttributeItemsToMembers",
        data: $('#itemAttributionForm').serialize(),
        success: function (data) {
            if (data.isAttributed == true) {
                showSummarySuccess(data.message);
                $('#ItemsIDTextBox').val('');
            }
            else {
                showSummaryError(data.message);
            }
        }
    });
}

function synchronizeGo(action) {
    if (confirm("Would you like to " + action + "?")) {
        $("#syncErrorContainer").html('');
        $("#syncErrorContainer").hide();
        $.ajax({
            type: "POST",
            url: applicationPath + "Item/SynchronizeGo",
            success: function (data) {
                if (data.isFullSynchronized == true) {
                    showSummarySuccess(data.message);
                }
                else {
                    $("#syncErrorContainer").html(data.message);
                    $("#syncErrorContainer").show();
                }
            }
        });
    }
}

/* Management */

function getMemberCmid() {

    $("#memberCmidSpan").html('');
    $("#getMemberCmidButton").hide();
    $("#getMemberCmidButton").before(loadingImage.clone().attr('id', 'getMemberCmidLoader'));

    $.ajax({
        type: 'POST',
        url: applicationPath + 'Member/GetUserCmid',
        data: { 'userName': $("#MemberNameTextBox").val() },
        success: function (data) {

            if (data.MemberCmid == '0') {
                $("#memberCmidSpan").html('No member matching this name');
            }
            else {
                var separator = '';

                if ($("#CmidsTextBox").val().length > 0) {
                    separator = ',';
                }

                $("#CmidsTextBox").addClass('modifiedField');
                setTimeout(function () {
                    $("#CmidsTextBox").removeClass('modifiedField');
                }, 800);

                $("#CmidsTextBox").val($("#CmidsTextBox").val() + separator + data.MemberCmid);
                $("#MemberNameTextBox").val('');

                $("#memberCmidSpan").html('The cmid ' + data.MemberCmid + ' has been added.');
            }

            $('#getMemberCmidLoader').remove();
            $('#getMemberCmidButton').show();
        },
        error: function (data) {
            showSummaryError('An error happened');
            $('#getMemberCmidLoader').remove();
            $('#getMemberCmidButton').show();
        }
    });

    return false;
}

function getItemId() {

    $("#itemIdSpan").html('');
    $("#getItemIdButton").hide();
    $("#getItemIdButton").before(loadingImage.clone().attr('id', 'getItemIdLoader'));

    $.ajax({
        type: 'POST',
        url: applicationPath + 'Item/GetItemId',
        data: { 'itemName': $("#ItemNameTextBox").val() },
        success: function (data) {

            if (data.ItemId == '0') {
                $("#itemIdSpan").html('No item matching this name');
            }
            else {
                var separator = '';

                if ($("#ItemsIDTextBox").val().length > 0) {
                    separator = ',';
                }

                $("#ItemsIDTextBox").addClass('modifiedField');
                setTimeout(function () {
                    $("#ItemsIDTextBox").removeClass('modifiedField');
                }, 800);

                $("#ItemsIDTextBox").val($("#ItemsIDTextBox").val() + separator + data.ItemId);
                $("#ItemNameTextBox").val('');

                $("#itemIdSpan").html('The item Id ' + data.ItemId + ' has been added.');
            }

            $('#getItemIdLoader').remove();
            $('#getItemIdButton').show();
        },
        error: function (data) {
            showSummaryError('An error happened');
            $('#getItemIdLoader').remove();
            $('#getItemIdButton').show();
        }
    });

    return false;
}

function applyGlobalDiscount() {

    $("#applyDiscountButton").hide();
    $("#applyDiscountButton").before(loadingImage.clone().attr('id', 'applyGlobalDiscountLoader'));

    $.ajax({
        type: 'POST',
        url: applicationPath + 'Item/ApplyGlobalDiscount',
        data: { 'discount': $("#DiscountTextBox").val() },
        success: function (data) {

            if (data.IsApplied == true) {
                showSummarySuccess('The global discount has been applied.');
            }
            else {
                showSummaryError('The global discount has not been applied.');
            }

            $('#applyGlobalDiscountLoader').remove();
            $('#applyDiscountButton').show();
        },
        error: function (data) {
            showSummaryError('An error happened');
            $('#applyGlobalDiscountLoader').remove();
            $('#applyDiscountButton').show();
        }
    });

    return false;
}

/* End Management */

/* item comparison */

function LoadTypeComparison(itemTypeId) {
    $.ajax({
        type: 'POST',
        url: applicationPath + 'Item/ComparisonPerType',
        data: { 'itemTypeId': itemTypeId },
        success: function (data) {
            $("#ComparisonTable").html(data);
        }
    });
}

/* item deprecation */

function LoadItemPricing(itemId) {
    if (itemId > 0) {
        $.ajax({
            type: 'POST',
            url: applicationPath + 'ItemDeprecation/GetItemPricing',
            data: { 'ItemId': itemId },
            success: function (data) {
                $("#ItemDeprecationPricing").html(data);
            }
        });
    }
}

function DeprecateItem(itemToDeprecateData, dailyPrice, permanentPrice) {
    $("#pricingDefinitionError").html('');
    if (dailyPrice > 0 && permanentPrice > 0) {
        if (confirm("Are you sure to deprecate this item ?")) {
            $.ajax({
                type: 'POST',
                url: applicationPath + 'ItemDeprecation/Deprecate',
                data: itemToDeprecateData,
                success: function (data) {
                    if (data.success) {
                        showSummarySuccess(data.message);
                    }
                    else {
                        showSummaryError(data.message);
                    }
                }
            });
        }
    }
    else {
        $("#pricingDefinitionError").html("A daily and permanent price must be informed to deprecate this item");
    }
}