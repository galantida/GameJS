// report version
console.log("=== included clsImageset.js ver 0.1 ===");

function clsImageset(name, tileSize, width, height) {
    this.name = name;
    this.tileSize = tileSize;
    this.width = width;
    this.height = height;
    this.displayWidth = tileSize;
    this.displayHeight = tileSize / 2;
}

// example of a function added to the prototype (this.value works)
clsImageset.prototype.example = function () {
    return this.x;
};

