function AddOrEditManagedServer() {
    $.ajax({
        type: 'POST',
        url: applicationPath + 'Servers/AddOrEditManagedServer',
        data: $("#addOrEditManagedServerForm").serialize(),
        success: function (data) {
            if (data.isAddOrEdit) {
                alert(data.message);
                window.location.reload();
            }
            else {
                showSummaryError(data.message);
            }
        }
    });
}

function LoadAddOrEditManagedServerForm(managedServerId) {
    $.ajax({
        type: 'POST',
        url: applicationPath + 'Servers/LoadAddOrEditManagedServerForm',
        data: { "managedServerId": managedServerId },
        success: function (data) {
            Shadowbox.open({
                content: data,
                player: "html",
                title: managedServerId == 0 ? "Add" : "Edit",
                width: 560,
                height: 760,
                options: { onFinish: function () {
                }
                }
            });
        }
    });
}

function DisableOrEnableManagedServer(actionType, managedServerId) {
    $.ajax({
        type: 'POST',
        url: applicationPath + 'Servers/EnableOrDisableManagedServer',
        data: { "actionType": actionType, "managedServerId": managedServerId },
        success: function (data) {
            if (data.isEnabledOrDisabled) {
                window.location.reload(true);
            }
            else {
                showSummaryError(data.message);
            }
        }
    });
}

function DeleteManagedServer(managedServerId) {
    if (confirm("would you delete the server managed with ID " + managedServerId)) {
        $.ajax({
            type: 'POST',
            url: applicationPath + 'Servers/DeleteManagedServer',
            data: { "managedServerId": managedServerId },
            success: function (data) {
                if (!data.isError) {
                    alert("the server has been deleted");
                    window.location.reload();
                }
                else
                    showSummaryError("Error occured");
            }
        });
    }
}
