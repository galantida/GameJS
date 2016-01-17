/*******************************************************************
    Ground Object  - This object is used to manage the ground tiles
                        displayed in the worldview
********************************************************************/


// report version
console.log("=== included clsGround.js ver 0.1 ===");

function clsGround(displayPanel) {
    console.log("Creating ground display panel...");

    // properties
    this.world = new clsVector2D(0, 0);

    // initializations
    this.tileset = new clsTileset("yarsTileset.png", 64, 10, 12); // load tileset
    this.buffer = this.createBuffer(displayPanel); // create and position buffer element based on screen size and position
    this.createTiles(); // create random map this will be a JSON map load in the future
    this.desiredShift = new clsVector2D(0, 0); // screen coordinates to scroll to
}

/**********************************************
    Initialization functions
***********************************************/
clsGround.prototype.createBuffer = function (displayPanel) {
    console.log("Creating Buffer...");

    var buffer = { "border": 2 }; // minimum border in tiles to load off screen 

    // calculate buffer size (based on display hypotenuse and tilesizes)
    buffer.size = Math.ceil(displayPanel.hypotenuse() / this.tileset.displayHeight) + (buffer.border * 2); // calculate buffer size in tiles
    buffer.displayHeight = buffer.size * this.tileset.displayHeight; // actual vertical pixels on screen
    buffer.displayWidth = buffer.size * this.tileset.displayWidth; // actual horizontal pixels on screen

    // create and position buffer element based on display panel size
    buffer.element = this.createDiv(displayPanel.name + "Buffer");
    buffer.element.style.left = ((displayPanel.width() / 2) - (this.tileset.displayWidth / 2)) + "px"; // move right to the center of the display area
    buffer.element.style.top = ((displayPanel.height() / 2) - (buffer.displayHeight / 2) - (this.tileset.displayHeight / 2)) + "px"; // move down half the display and back up half the buffer
    buffer.element.style.position = "relative";
    //buffer.element.style.border = "1px solid red";
    displayPanel.element.appendChild(buffer.element);
    return buffer;
}

clsGround.prototype.createTiles = function () {
    console.log("Creating ground tiles...");

    // create tile array
    this.tiles = new Array(this.buffer.size); // create row array
    for (var x = 0; x < this.buffer.size; x++) {
        this.tiles[x] = new Array(this.buffer.size); // add column array to each row
        for (var y = 0; y < this.buffer.size; y++) {
            this.tiles[x][y] = {}; // could add other properties to the object array here.
        }
    }

    // create html tile elements
    for (var y = 0; y < this.buffer.size ; y++) {
        for (var x = 0; x < this.buffer.size; x++) {
            // create tile div element
            var ele = this.createDiv(x + "-" + y, "clsGround tile");
            ele.style.left = (((x * .5) - (y * .5)) * this.tileset.displayWidth) + "px";
            ele.style.top = (((x * .5) + (y * .5)) * this.tileset.displayHeight) + "px";
            ele.onmousedown = function () { tileOnClick(this.id); };
            //ele.onmouseover = function () { console.log(this.id); };
            this.buffer.element.appendChild(ele); // add the tile element to the buffer

            // add to array for faster access
            this.tiles[x][y].element = ele;
        }
    }
}

clsGround.prototype.createDiv = function (id, className) {
    var ele = document.createElement('div');
    ele.setAttribute('id', id);
    ele.className = className;
    return ele;
}

/**********************************************
    Conversion functions
***********************************************/
clsGround.prototype.worldToScreen = function (worldLocation) {
    return new clsVector2D(worldLocation.x - this.world.x, worldLocation.y - this.world.y);

    // old code
    var result = new clsVector2D(worldLocation.x - this.world.x, worldLocation.y - this.world.y);
    console.log("world (" + JSON.stringify(worldLocation) + ") to screen (" + JSON.stringify(result) + ")");
    return result;
}

clsGround.prototype.screenToWorld = function (screenLocation) {
    return new clsVector2D(this.world.x + screenLocation.x, this.world.y + screenLocation.y);

    // old code
    var result = new clsVector2D(this.world.x + screenLocation.x, this.world.y + screenLocation.y);
    console.log("screen (" + JSON.stringify(screenLocation) + ") to world (" + JSON.stringify(result) + ")");
    return result;
}


/**********************************************
    screen usage function
***********************************************/

// sets the ground to a specified location
clsGround.prototype.clearTiles = function () {
    // create html tile elements
    for (var y = 0; y < this.buffer.size ; y++) {
        for (var x = 0; x < this.buffer.size; x++) {
            var ele = document.getElementById(x + "-" + y);
            ele.style.backgroundPosition = "0em 0em";
        }
    }
}


// sets the ground to a specified location
clsGround.prototype.jumpToLocation = function (worldx, worldy) {
    console.log("Jumping top left of view to world location (" + worldx + ", " + worldy + ")");

    // set new world location
    this.world.x = worldx;
    this.world.y = worldy;

    // pre calculate bottom right tiles for speed.
    var worldx2 = this.world.x + this.buffer.size - 1;
    var worldy2 = this.world.y + this.buffer.size - 1;

    // request tiles
    wsi.requestJSONInfo({ "callName": "getTiles", "x1": worldx, "y1": worldy, "x2": worldx2, "y2": worldy2 }, JSONResponseHandler);
}

clsGround.prototype.displayTiles = function (tiles) {
    console.log("Updating ground tiles with new tile list...");

    console.log(JSON.stringify(document.styleSheets)); // ask Kevin about this one
    /* transition:background-position linear 1000ms; // recall in */

    // update new ground tiles
    this.clearTiles(); // clear all ground tiles
    for (var t = 0; t < tiles.length; t++) {
        var screenLocation = this.worldToScreen(new clsVector2D(tiles[t].x, tiles[t].y));
        var ele = document.getElementById(screenLocation.x + "-" + screenLocation.y);
        if (ele != null) {
            //console.log("updating " + screenLocation.x + "-" + screenLocation.y + ".");
            // add support for tile sets later
            ele.style.backgroundPosition = "-" + tiles[t].col + "em -" + tiles[t].row + "em";
        }
        else {
            // skipping tiles that are nolonger visible
            //console.log("skipping " + screenLocation.x + "-" + screenLocation.y + ".");
        }
    }
}


clsGround.prototype.shiftTiles = function (shiftx, shifty) {

    console.log("Shifting tiles... (" + shiftx + ", " + shifty + ")");

    // determine the best copy direction based on the y shift direction
    var yfrom = 0;
    var yto = this.buffer.size - 1;
    var yinc = 1;
    if (shifty == -1) {
        yfrom = this.buffer.size - 1;
        yto = 0;
        yinc = -1;
    }

    // determine the best copy direction based on the x shift direction
    var xfrom = 0;
    var xto = this.buffer.size - 1;
    var xinc = 1;
    if (shiftx == -1) {
        xfrom = this.buffer.size - 1;
        xto = 0;
        xinc = -1;
    }

    // loop the the rows(y)
    var y = yfrom;
    while (((y >= yfrom) && (y <= yto)) || ((y <= yfrom) && (y >= yto))) {

        // loop the the columns(x)
        var x = xfrom;
        while (((x >= xfrom) && (x <= xto)) || ((x <= xfrom) && (x >= xto))) {
            var xdes = utils.wrap(x + shiftx, 0, this.buffer.size - 1);
            var ydes = utils.wrap(y + shifty, 0, this.buffer.size - 1);
            this.tiles[x][y].element.style.backgroundPosition = this.tiles[xdes][ydes].element.style.backgroundPosition;
            x += xinc;
        }
        y += yinc;
    }
}

clsGround.prototype.ScrollToTileLocation = function (tileID) {
    var location = tileID.split("-");
    var bufferCenter = Math.floor(this.buffer.size / 2);
    this.desiredShift.x = location[0] - bufferCenter;
    this.desiredShift.y = location[1] - bufferCenter;
}




clsGround.prototype.process = function () {

    if ((this.desiredShift.x != 0) || (this.desiredShift.y != 0)) {
        var shiftx = 0;
        if (this.desiredShift.x > 0) {
            this.desiredShift.x--;
            shiftx = 1;
        } else if (this.desiredShift.x < 0) {
            this.desiredShift.x++;
            shiftx = -1;
        }

        var shifty = 0;
        if (this.desiredShift.y > 0) {
            this.desiredShift.y--;
            shifty = 1;
        } else if (this.desiredShift.y < 0) {
            this.desiredShift.y++;
            shifty = -1;
        }

        this.shiftTiles(shiftx, shifty);
    }
}

