// report version
console.log("=== included utils.js ver 0.1 ===");


var utils = { // utils namespace

    wrap: function (value, min, max) {
        var dif = (max - min) + 1;
        while (value > max) value -= dif;
        while (value < min) value += dif;
        return value;
    },

    swap: function (valueA, valueB) {
        var tmp = valueA;
        valueA = valueB;
        valueB = tmp;
    },

    mySQLFriendlyGMTDateTime: function (dateTime) {
        if (dateTime == null) dateTime = new Date(); // if you don't pass a datetime the current datetime will be used

        // create a GMT string
        var GMTString = dateTime.toUTCString(); // convert local time to GMT
        GMTString = GMTString.substring(0, GMTString.length - 3); // chop the timezone off the string

        // create a spoofed GMT date object
        GMTDateTime = new Date(GMTString); // this date time will says it is eastern but we know better

        // format the date time to a mySQL friendly format without timezone
        return utils.mySQLFriendlyDateTime(GMTDateTime);
    },

    mySQLFriendlyDateTime: function (dateTime) {
        if (dateTime == null) dateTime = new Date(); // if you don't pass a datetime the current datetime will be used
        return (dateTime.getMonth() + 1) + '/' + dateTime.getDate() + '/' + dateTime.getFullYear() + " " + dateTime.getHours() + ":" + dateTime.getMinutes() + ":" + dateTime.getSeconds();
    },

    loadScript: function (url, callback) {

        var script = document.createElement("script")
        script.type = "text/javascript";

        if (script.readyState) { //IE
            script.onreadystatechange = function () {
                if (script.readyState == "loaded" || script.readyState == "complete") {
                    script.onreadystatechange = null;
                    callback();
                }
            };
        } else { //Others
            script.onload = function () {
                callback();
            };
        }
        script.src = url;
        document.getElementsByTagName("head")[0].appendChild(script);
    }
}