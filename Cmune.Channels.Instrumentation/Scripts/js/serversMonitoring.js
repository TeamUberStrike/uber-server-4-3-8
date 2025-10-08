/*function RefreshManagedServerMonitoring(managedServerId) {
    managedServerId = managedServerId != undefined ? managedServerId : '';
    $.ajax({
        type: 'POST',
        url: applicationPath + 'ServersMonitoring/GetServerMonitoring',
        data:  $('#StatsCalendar').serialize() + '&managedServerId=' + managedServerId,
        success: function (data) {
            $("#managedServerContainer").html(data);
        }
    });
}*/

function RefreshManagedServerServices(managedServerId) {
    managedServerId = managedServerId != undefined ? managedServerId : '';
    $("#ManagedServerServices").html('')
    $.ajax({
        type: 'POST',
        url: applicationPath + 'ServersMonitoring/GetManagedServerServices',
        data: {managedServerId : managedServerId},
        success: function (data) {
            $("#ManagedServerServices").html(data);
        }
    });
}