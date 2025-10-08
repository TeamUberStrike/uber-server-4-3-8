// BrowserDetectWrapper.js

function IsWindowBrowser() {
    return BrowserDetect.OS == "Windows";
}

function IsFirefoxBrowser() {
    return BrowserDetect.browser == "Firefox";
}

function IsIE7Browser() {
    return BrowserDetect.browser == "Explorer" && BrowserDetect.version == "7";
}

function IsIE8Browser() {
    return BrowserDetect.browser == "Explorer" && BrowserDetect.version == "8";
}

function IsIE9Browser() {
    return BrowserDetect.browser == "Explorer" && BrowserDetect.version == "9";
}