function LoadMysteryBoxForm(mysteryBoxId) {
    $.ajax({
        type: 'POST',
        url: applicationPath + 'MysteryBox/GetMysteryBoxForm',
        data: { "mysteryBoxId": mysteryBoxId },
        success: function (data) {
            Shadowbox.open({
                content: data,
                player: "html",
                title: "Mystery Box Form",
                width: 700,
                height: 800
            });
        }
    });
}

function AddMysteryBoxItem(dropDownListItemElement, dropDownListElement) {
    var luckyItemValue = $("#ItemIdCount").val();
    if (parseInt(luckyItemValue) < 12) {
        var htmlElement = "<select name='MysteryBoxItems[" + luckyItemValue + "].ItemId' >";
        for (var i in dropDownListItemElement) {
            htmlElement += '<option value="' + dropDownListItemElement[i].Value + '">' + dropDownListItemElement[i].Text + '</option>';
        }     
        htmlElement += '</select>';
        htmlElement += '<select name="MysteryBoxItems[' + luckyItemValue + '].DurationType" >';
        for (var i in dropDownListElement) {
            htmlElement += '<option value="' + dropDownListElement[i].Value + '" ' + (dropDownListElement[i].Selected != false ? 'selected="selected"' : '') + ' >' + dropDownListElement[i].Text + '</option>';
        }
        htmlElement += '</select>';
        htmlElement += " <input type='text' name='MysteryBoxItems[" + luckyItemValue + "].ItemWeight' value='' style='width:50px;' type='text'><br/>";
        htmlElement += " <input type='text' name='MysteryBoxItems[" + luckyItemValue + "].Amount' value='' style='width:50px;' type='text'><br/>";
        $("#MysteryBoxItemContainer").append(htmlElement);
        $("#ItemIdCount").val(parseInt(luckyItemValue) + 1);
    }
}

function SubmitMysteryBoxForm() {
    $.ajax({
        type: 'POST',
        url: applicationPath + 'MysteryBox/SaveMysteryBoxForm',
        data: $("#MysteryBoxForm").serialize(),
        success: function (data) {
            if (data.success) {
                window.location.reload(true);
            }
            else {
                showSummaryError(data.message);
            }
        }
    });
}

function TestMysteryBox() {
    $.ajax({
        type: 'POST',
        url: applicationPath + 'MysteryBox/TestMysteryBox',
        data: $("#MysteryBoxForm").serialize(),
        success: function (data) {
            $("#TestMysteryBoxResultContainer").html(data);
        }
    });
}