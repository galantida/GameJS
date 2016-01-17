// report version
console.log("=== included clsTileset.js ver 0.1 ===");

function clsTileset(name, tileSize, width, height) {
    this.name = name;
    this.tileSize = tileSize;
    this.width = width;
    this.height = height;
    this.displayWidth = tileSize;
    this.displayHeight = tileSize / 2;
}

// example of a function added to the prototype (this.value works)
clsTileset.prototype.example = function () {
    return this.x;
};

