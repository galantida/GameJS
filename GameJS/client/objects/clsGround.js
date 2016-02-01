/*******************************************************************
    Ground Object  - This object is used to manage the ground cubes
                        displayed in the worldview
********************************************************************/


// report version
console.log("=== included clsGround.js ver 0.1 ===");

function clsGround(displayPanel) {
    console.log("Creating ground display panel...");

    // properties
    this.world = new clsVector2D(0, 0);
    this.worldBottomRight = new clsVector2D(0, 0);

    // inner workings
    this.lastUpdate = new Date();

    // initializations
    this.cs = new clsImageset("cubes.png", 64, 10, 12); // load cs (not really used)
    this.buffer = this.createBuffer(displayPanel); // create and position buffer element based on screen size and position
    this.createTiles(); // create random map this will be a JSON map load in the future
}

clsGround.prototype.worldBottomRight = function () {
    
}

/**********************************************
    Initialization functions
***********************************************/
clsGround.prototype.createBuffer = function (displayPanel) {
    console.log("Creating Buffer...");

    var buffer = { "border": 2 }; // minimum border in cubes to load off screen 

    // calculate buffer size (based on display hypotenuse and cubesizes)
    buffer.size = Math.ceil(displayPanel.hypotenuse() / this.cs.displayHeight) + (buffer.border * 2); // calculate buffer size in cubes
    buffer.displayHeight = buffer.size * this.cs.displayHeight; // actual vertical pixels on screen
    buffer.displayWidth = buffer.size * this.cs.displayWidth; // actual horizontal pixels on screen

    // create and position buffer element based on display panel size
    buffer.element = document.createElement('div');
    buffer.element.setAttribute('id', "Buffer");
    buffer.element.style.left = ((displayPanel.width() / 2) - (this.cs.displayWidth / 2)) + "px"; // move right to the center of the display area
    buffer.element.style.top = ((displayPanel.height() / 2) - (buffer.displayHeight / 2) - (this.cs.displayHeight / 2)) + "px"; // move down half the display and back up half the buffer
    buffer.element.style.position = "relative";
    //buffer.element.style.border = "1px solid red";
    displayPanel.element.appendChild(buffer.element);
    return buffer;
}

clsGround.prototype.createTiles = function () {
    console.log("Creating tiles...");

    // create cube array for faster access
    this.tiles = new Array(this.buffer.size); // create row array with global scope
    for (var x = 0; x < this.buffer.size; x++) {
        this.tiles[x] = new Array(this.buffer.size); // add column array to each row
        for (var y = 0; y < this.buffer.size; y++) {
            this.tiles[x][y] = {}; // could add other properties to the object array here.
        }
    }

    // create html tile elements
    for (var y = 0; y < this.buffer.size ; y++) {
        for (var x = 0; x < this.buffer.size; x++) {

            // create cube div element
            var ele = document.createElement('div');
            ele.className = "clsGround tile";

            // set dataset properties
            ele.dataset.x = x;
            ele.dataset.y = y;
            ele.dataset.defaultLeft = (((x * .5) - (y * .5)) * this.cs.displayWidth);
            ele.dataset.defaultTop = (((x * .5) + (y * .5)) * this.cs.displayHeight);

            // set element properties
            ele.style.left = ele.dataset.defaultLeft + "px";
            ele.style.top = ele.dataset.defaultTop + "px";

            // set element events
            ele.onmousedown = function () { tileOnClick(this); };
            ele.addEventListener("contextmenu", function (e) { e.preventDefault(); });
            this.buffer.element.appendChild(ele); // add the cube element to the buffer

            // add element to array for faster access
            this.tiles[x][y].element = ele;
        }
    }
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

// sets all ground cubes in a specificed area to cube 0,0
clsGround.prototype.clearArea = function (x1, y1, x2, y2) {
    console.log("Clearing tiles (" + x1 + "," + y1 + " - " + x2 + ", " + y2 + ")");
    for (var y = y1; y <= y2 ; y++) {
        for (var x = x1; x <= x2; x++) {
            var ele = this.tiles[x][y].element;
            ele.innerHTML = "";
            ele.style.top = ele.dataset.defaultTop; // may not need this now that we don't move tiles around
            ele.style.left = ele.dataset.defaultLeft; // may not need this now that we don't move tiles around
            ele.dataset.z = 0; // my not need this now that we don't move tiles around

            // set empty images to noTile
            ele.style.background = "url('../images/tiles/noTile.png')";
        }
    }
}

clsGround.prototype.clearAll = function (x1, y1, x2, y2) {
    this.clearArea(0, 0, this.buffer.size-1, this.buffer.size-1);
}


// sets the top left of the ground ground to a specified world location
clsGround.prototype.jumpToLocation = function (worldx, worldy) {
    console.log("Jumping top left of view to world location (" + worldx + ", " + worldy + ")");

    // set new world location
    this.world = new clsVector2D(worldx, worldy);
    this.worldBottomRight = new clsVector2D(this.world.x + this.buffer.size - 1, this.world.y + this.buffer.size - 1);

    this.clearAll(); // clear all existing cubes

    // request cubes
    wsi.requestJSONInfo({ "callName": "getObjects", "x1": worldx, "y1": worldy, "x2": this.worldBottomRight.x, "y2": this.worldBottomRight.y }, JSONResponseHandler);
}


// given a list of world cubes this will refresh their counter parts on the screen if they are still visible
// this is run for all responses from the server that return updated cube information
clsGround.prototype.updateObjects = function (objects) {
    console.log("Updating tiles with new object list...");

    console.log(JSON.stringify(document.styleSheets)); // ask Kevin about this one
    /* transition:background-position linear 1000ms; // recall in */

    // update new objects
    for (var t = 0; t < objects.length; t++) {

        // get object
        var obj = objects[t];

        // get objects screen tile location
        var screenLocation = this.worldToScreen(new clsVector2D(obj.x, obj.y));
        var tile = this.tiles[screenLocation.x][screenLocation.y].element;
        
        // only update cubes that have not yet scrolled off the buffer
        if (tile != null) {

            // clear default background
            tile.style.backgroundImage = "";

            // clear deleted elements - careful modified only returned new or updates ones

            // place new elements
            var ele = window[obj.pack]["create"](obj);
            tile.appendChild(ele);

            // add support for elevations
        }
    }
}


// scroll the entire landscape one cube in any direction
clsGround.prototype.shiftCubes = function(shiftx, shifty) {

    console.log("Shifting cubes... (" + shiftx + ", " + shifty + ")");

    // set new world location
    this.world = new clsVector2D(this.world.x += shiftx, this.world.y += shifty);
    this.worldBottomRight = new clsVector2D(this.world.x + this.buffer.size - 1, this.world.y + this.buffer.size - 1);

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

            // tile to edit
            var tile = this.tiles[x][y].element;
            tile.innerHTML = ""; // clear destination tile

            // calculate the source location for each tiles information
            var sourcex = utils.wrap(x + shiftx, 0, this.buffer.size - 1);
            var sourcey = utils.wrap(y + shifty, 0, this.buffer.size - 1);
            var source = this.tiles[sourcex][sourcey].element;
            

            // move elements from source tile
            while (source.hasChildNodes()) {
                var ele = source.removeChild(source.childNodes[0]);
                tile.appendChild(ele);
            }

            // copy tile guides
            tile.style.background = source.style.background; // copy cube image and and tile info
            x += xinc;
        }
        y += yinc;
    }

    // request update
    var last = this.buffer.size - 1; // last row or column. They are the same because its a square.

    if (shiftx == 1) {
        // request last row
        var worldRow1 = this.screenToWorld(new clsVector2D(last, 0));
        var worldRow2 = this.screenToWorld(new clsVector2D(last, last));
        this.clearArea(last, 0, last, last);
        wsi.requestJSONInfo({ "callName": "getObjects", "x1": worldRow1.x, "y1": worldRow1.y, "x2": worldRow2.x, "y2": worldRow2.y }, JSONResponseHandler);
        //wsi.requestJSONInfo({ "callName": "getObjects", "x1": worldRow1.x, "y1": worldRow1.y, "x2": worldRow2.x-3, "y2": worldRow2.y }, JSONResponseHandler);
    } else if (shiftx == -1) {
        // request first row
        var worldRow1 = this.screenToWorld(new clsVector2D(0, 0));
        var worldRow2 = this.screenToWorld(new clsVector2D(0, last));
        this.clearArea(0, 0, 0, last);
        wsi.requestJSONInfo({ "callName": "getObjects", "x1": worldRow1.x, "y1": worldRow1.y, "x2": worldRow2.x, "y2": worldRow2.y }, JSONResponseHandler);
        //wsi.requestJSONInfo({ "callName": "getObjects", "x1": worldRow1.x, "y1": worldRow1.y, "x2": worldRow2.x + 3, "y2": worldRow2.y }, JSONResponseHandler);
    }

    if (shifty == 1) {
        // request last col
        var worldCol1 = this.screenToWorld(new clsVector2D(0, last));
        var worldCol2 = this.screenToWorld(new clsVector2D(last, last));
        this.clearArea(0, last, last, last);
        wsi.requestJSONInfo({ "callName": "getObjects", "x1": worldCol1.x, "y1": worldCol1.y, "x2": worldCol2.x, "y2": worldCol2.y }, JSONResponseHandler);
        //wsi.requestJSONInfo({ "callName": "getObjects", "x1": worldCol1.x, "y1": worldCol1.y, "x2": worldCol2.x, "y2": worldCol2.y - 3 }, JSONResponseHandler);
    } else if (shifty == -1) {
        // request first col
        var worldCol1 = this.screenToWorld(new clsVector2D(0, 0));
        var worldCol2 = this.screenToWorld(new clsVector2D(last, 0));
        this.clearArea(0, 0, last, 0);
        wsi.requestJSONInfo({ "callName": "getObjects", "x1": worldCol1.x, "y1": worldCol1.y, "x2": worldCol2.x, "y2": worldCol2.y }, JSONResponseHandler);
        //wsi.requestJSONInfo({ "callName": "getObjects", "x1": worldCol1.x, "y1": worldCol1.y, "x2": worldCol2.x, "y2": worldCol2.y + 3 }, JSONResponseHandler);
    }

}

clsGround.prototype.update = function () {
    console.log("Requesting objects modified since " + this.lastUpdate + " in (" + this.world.x + "," + this.world.y + " - " + this.worldBottomRight.x + "," + this.worldBottomRight.y + ")");
    // update example
    wsi.requestJSONInfo({ "callName": "getObjects", "x1": this.world.x, "y1": this.world.y, "x2": this.worldBottomRight.x, "y2": this.worldBottomRight.y, "modified": utils.wsFriendlyDateTime(this.lastUpdate) }, JSONResponseHandler);
    // key frame example
    //wsi.requestJSONInfo({ "callName": "getObjects", "x1": this.world.x, "y1": this.world.y, "x2": this.worldBottomRight.x, "y2": this.worldBottomRight.y}, JSONResponseHandler);
    this.lastUpdate = new Date();

    // you may want keyframes to avoid anomolies
}


clsGround.prototype.process = function () {

    // refresh modified cubes since last update
    if ((new Date() - this.lastUpdate) > 3000) {
        this.update(); // remember cube deletion is not an update since it will not have a modified record.
    }
}

