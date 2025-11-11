function LoadLuckyDrawForm(luckyDrawId) {
    $.ajax({
        type: 'POST',
        url: applicationPath + 'LuckyDraw/GetLuckyDrawForm',
        data: { "luckyDrawId": luckyDrawId },
        success: function (data) {
            Shadowbox.open({
                content: data,
                player: "html",
                title: "Lucky Draw Form",
                width: 1200,
                height: 800
            });
        }
    });
}

function AddLuckyDrawSetItem(x, dropDownListElementOne, dropDownListElement) {
    var luckyItemValue = $("#ItemIdCount_" + x).val();
    if (parseInt(luckyItemValue) < 12) {

        var htmlElement = "<select name='LuckyDrawSets[" + x + "].LuckyDrawSetItems[" + luckyItemValue + "].ItemId' >";
        for (var i in dropDownListElementOne) {
            htmlElement += '<option value="' + dropDownListElementOne[i].Value + '" ' + (dropDownListElementOne[i].Selected != false ? 'selected="selected"' : '') + '>' + dropDownListElementOne[i].Text + '</option>';
        }
        htmlElement += '</select>';
        htmlElement += '<select name="LuckyDrawSets[' + x + '].LuckyDrawSetItems[' + luckyItemValue + '].DurationType" >';
        for (var i in dropDownListElement) {
            htmlElement += '<option value="' + dropDownListElement[i].Value + '" ' + (dropDownListElement[i].Selected != false ? 'selected="selected"' : '') + ' >' + dropDownListElement[i].Text + '</option>';
        }
        htmlElement += '</select>';
        htmlElement += '<input id="LuckyDrawSets[' + x + '].LuckyDrawSetItems[' + luckyItemValue + '].Amount" name="LuckyDrawSets[' + x + '].LuckyDrawSetItems[' + luckyItemValue + '].Amount" style="width:20px;" type="text" value="0">';
        $("#LuckyDrawSetItemContainer" + x).append(htmlElement);
        $("#ItemIdCount_" + x).val(parseInt(luckyItemValue) + 1);
    }
}

function SubmitLuckyDrawForm() {
    $.ajax({
        type: 'POST',
        url: applicationPath + 'LuckyDraw/SaveLuckyDrawForm',
        data: $("#LuckyDrawForm").serialize(),
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

function TestLuckyDraw() {
    $.ajax({
        type: 'POST',
        url: applicationPath + 'LuckyDraw/TestLuckyDraw',
        data: $("#LuckyDrawForm").serialize(),
        success: function (data) {
            $("#TestLuckyDrawResultContainer").html(data);
        }
    });
}
