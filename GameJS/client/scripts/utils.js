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
    }
};
