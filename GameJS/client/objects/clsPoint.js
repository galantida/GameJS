/*******************************************************************
    clsPoint  - object supporting two dimensional math

    plan to make javascript formaula library as needed 
    which will contain this.
********************************************************************/


// report version
console.log("=== included clsPoint.js ver 0.1 ===");

function clsPoint(x, y, z) {
    this.x = x;
    this.y = y;
    this.z = z;
    if (z != null) this.z = z;
    else this.z = 0;
}

// example of a function added to the prototype (this.value works)
clsPoint.prototype.getInfo = function () {
    return this.x;
};

