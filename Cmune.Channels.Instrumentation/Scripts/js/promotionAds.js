function LoadAddEditPromotionContentForm(promotionContentId) {
    $.ajax({
        type: 'GET',
        url: applicationPath + 'PromotionContent/LoadAddEditPromotionContentForm',
        data: { "promotionContentId" : promotionContentId}, 
        success: function (data) {
            Shadowbox.open({
                content: data,
                player: "html",
                title: "Promotion ads management",
                width:900,
                height: 800
            });
        }
    });
}

var promotionAdsItemNumber = 0;

function AddPromotionAdsItemField(value, dropDownListChannelElement, dropDownListChannelType) {
    var htmlElement = "";
    htmlElement += '<div>Pic: ';

    htmlElement += '<select id="NewChannelType' + promotionAdsItemNumber + '" name="NewChannelType' + promotionAdsItemNumber + '" >';
    for (var i in dropDownListChannelType) {
        htmlElement += '<option value="' + dropDownListChannelType[i].Value + '" ' + (dropDownListChannelType[i].Selected != false ? 'selected="selected"' : '') + ' >' + dropDownListChannelType[i].Text + '</option>';
    }

    htmlElement += '</select>';
    htmlElement += '<select id="NewChannelElement' + promotionAdsItemNumber + '" name="NewChannelElement' + promotionAdsItemNumber + '" >';
    for (var i in dropDownListChannelElement) {
        htmlElement += '<option value="' + dropDownListChannelElement[i].Value + '" ' + (dropDownListChannelElement[i].Selected != false ? 'selected="selected"' : '') + ' >' + dropDownListChannelElement[i].Text + '</option>';
    }

    htmlElement += '</select>';

    htmlElement += '<input type="text" name="NewFilename' + promotionAdsItemNumber + '" value="">';
    htmlElement += '<input type="text" name="NewFilenameTitle' + promotionAdsItemNumber + '" value="">';
    htmlElement += '<input type="text" name="NewAnchorLink' + promotionAdsItemNumber + '" value="">';
    
    htmlElement += '</div>';
    if (promotionAdsItemNumber < 15) {
        $("#promotionAdsItems").append(htmlElement);
        promotionAdsItemNumber++;
    }
}

function AddEditPromotion() {
    $.ajax({
        type: 'POST',
        url: applicationPath + 'PromotionContent/AddEditPromotionContent',
        data: $("#addEditPromotionAds").serialize(),
        success: function (data) {
            if (data.isAddOrEdit) {
                alert(data.message);
                window.location.reload();
            }
            else {
                if (data.isJson) {
                    for (var i in data.message) {
                        showSummaryError(data.message[i]);
                    }
                }
                else
                    showSummaryError(data.message);

            }
        }
    });
}

function DeletePromotionContent(promotionContentId) {
    if (confirm("Are you sure to delete this pack ?")) {
        $.ajax({
            type: 'POST',
            url: applicationPath + 'PromotionContent/DeletePromotionContent',
            data: { "promotionContentId": promotionContentId },
            success: function (data) {
                if (data.isDeleted) {
                    window.location.reload();
                }
                else {
                    showSummaryError("Error occured");
                }
            }
        });
    }
}