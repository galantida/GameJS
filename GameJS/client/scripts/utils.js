﻿// report version
console.log("=== included utils.js ver 0.1 ===");


var utils = { // utils namespace

    wrap: function (value, min, max) {
        while (value > max) value -= max;
        while (value < min) value += max;
        return value;
    },

    swap: function (valueA, valueB) {
        var tmp = valueA;
        valueA = valueB;
        valueB = tmp;
    },

    wsFriendlyDateTime: function (dateTime) {
        return (dateTime.getMonth() + 1) + '/' + dateTime.getDate() + '/' + dateTime.getFullYear() + " " + dateTime.getHours() + ":" + dateTime.getMinutes() + ":" + dateTime.getSeconds();
    }
};
