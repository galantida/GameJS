function clsVector2D(x,y) {
    this.x = x;
    this.y = y;
}

// example of a function added to the prototype (this.value works)
clsVector2D.prototype.getInfo = function () {
    return this.x;
};

