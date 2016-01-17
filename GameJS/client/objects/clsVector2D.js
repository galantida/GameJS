/*******************************************************************
    clsVector2D  - vector object supporting two dimensional math

    plan to make javascript formaula library as needed 
    which will contain this.
********************************************************************/


// report version
console.log("=== included clsVector2D.js ver 0.1 ===");

function clsVector2D(x, y) {
    this.x = x;
    this.y = y;
}

// example of a function added to the prototype (this.value works)
clsVector2D.prototype.getInfo = function () {
    return this.x;
};

