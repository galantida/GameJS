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

    wsFriendlyDateTime: function (dateTime) {
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
};
