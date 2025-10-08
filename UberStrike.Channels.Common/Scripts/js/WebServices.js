// WebServices.js

// Eg: callWebService('Type/Function?var1=a&var2=b');
function callWebService(serviceUrl) {
    var s = document.createElement("script");
    s.src = webServicesBaseURL + serviceUrl;
    s.type = "text/javascript";
    document.getElementsByTagName("head")[0].appendChild(s)
}