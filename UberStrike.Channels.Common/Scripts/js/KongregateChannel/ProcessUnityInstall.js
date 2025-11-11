function processUnityInstallFlow() {
    $.ajax({
        type: 'POST',
        url: processUnityInstallFlowUrl,
        data: {
            unityObjectUAInstallUrl: unityObject.ua.installUrl,
            unityObjectUAInstallonclick: unityObject.ua.installOnclick,
            unityObjectUAJava: unityObject.ua.java,
            unityObjectUACo: unityObject.ua.co,
            unityObjectUAWin: unityObject.ua.win,
            unityObjectUAMac: unityObject.ua.mac,
            unityObjectUAInstallNavPlatform: unityObject.ua.installNav.platform
        },
        success: function (data) {
            $("#unityPlayer").html(data);
        }
    });
}
