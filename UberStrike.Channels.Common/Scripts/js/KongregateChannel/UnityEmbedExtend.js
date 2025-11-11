// Still in use but might need refactor
function displayHeader() {

}


function GetTopMenu() {
    $.ajax({
        type: 'POST',
        url: RefreshTopMenuUrl + "?" + GetKongregateLoginData(),
        success: function (data) {
            $("#UberstrikeMenu").html(data);
        }
    });
}

// Still in use
function getCreditsWrapper(cmid) {
    LoadCreditBundlePage();
}