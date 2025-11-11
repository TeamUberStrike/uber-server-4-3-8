/*
* serverTime 0.1
* Calculates current time based on input datetime
*/

// Set the current UTC time
var currentTime;

function setServerTime(dateTime) {
    // local time -> let's convert it to UTC
    var localTime = dateTime.getTime();
    var localOffset = dateTime.getTimezoneOffset() * 60000;
    var utc = utc = localTime + localOffset;
    currentTime = new Date(utc);
}

function updateClock() {
    // Since we increment by seconds, add one second to the clock
    currentTime.setSeconds(currentTime.getSeconds() + 1, 0);

    var currentMinutes = currentTime.getMinutes();
    var currentSeconds = currentTime.getSeconds();

    // Pad the minutes and seconds with leading zeros, if required
    currentMinutes = (currentMinutes < 10 ? "0" : "") + currentMinutes;
    currentSeconds = (currentSeconds < 10 ? "0" : "") + currentSeconds;

    // Update the time display
    document.getElementById("clock").innerHTML = currentTime.getHours() + ":" + currentMinutes + ":" + currentSeconds;
}
