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

    // initializations
    this.cs = { "width":64, "height":64, "displayWidth":64, "displayHeight":32 };
    this.buffer = this.createBuffer(displayPanel); // create and position buffer element based on screen size and position

    // rename to grid!!!!
    this.createTiles(); // create the tile that we will fill with objects

    // request regular updates without user interaction
    this.lastUpdate = new Date("3/19/69");
    this.heartBeat = setInterval(function() { client.worldView.ground.update(); }, 1000);
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

            // set dataset properties (not sure I even need these any more)
            ele.dataset.x = x;
            ele.dataset.y = y;
            ele.dataset.defaultLeft = (((x * .5) - (y * .5)) * this.cs.displayWidth);
            ele.dataset.defaultTop = (((x * .5) + (y * .5)) * this.cs.displayHeight);

            // set element properties
            ele.style.left = ele.dataset.defaultLeft + "px";
            ele.style.top = ele.dataset.defaultTop + "px";

            // set element events
            ele.onmousedown = function () { client.worldView.ground.onClickTile(this); };
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
}

// it may be a fluke but this function did not work last time I used to. (probably should never never need it);
clsGround.prototype.screenToWorld = function (screenLocation) {
    return new clsVector2D(this.world.x + screenLocation.x, this.world.y + screenLocation.y);
}

clsGround.prototype.setWorldLocation = function (worldLocation) {
    // set new world location
    this.world = worldLocation
    this.worldBottomRight = new clsVector2D(this.world.x + this.buffer.size - 1, this.world.y + this.buffer.size - 1);
}


/**********************************************
    screen usage function
***********************************************/

// sets all ground cubes in a specificed area to cube 0,0
clsGround.prototype.clearArea = function (x1, y1, x2, y2) {
    console.log("Clearing tiles (" + x1 + "," + y1 + " - " + x2 + "," + y2 + ")");
    for (var y = y1; y <= y2 ; y++) {
        for (var x = x1; x <= x2; x++) {
            var ele = this.tiles[x][y].element;
            ele.innerHTML = "";
            ele.style.background = "url('../images/tiles/noTile.png')"; // set empty images to noTile background
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
    this.setWorldLocation(new clsVector2D(worldx, worldy));
    this.clearAll(); // clear all existing cubes

    // request objects
    wsi.requestJSONInfo({ "callName": "getArea", "x1": worldx, "y1": worldy, "x2": this.worldBottomRight.x, "y2": this.worldBottomRight.y }, client.worldView.ground.objectsResponse);
}

// not sure why we need this to handle the response but it works
clsGround.prototype.objectsResponse = function (response) {
    client.worldView.ground.updateObjects(response.content);
}


// given a list of world cubes this will refresh their counter parts on the screen if they are still visible
// this is run for all responses from the server that return updated cube information
clsGround.prototype.updateObjects = function (objects) {
    console.log("Updating tiles with new object list...");

    // update new objects
    for (var t = 0; t < objects.length; t++) {

        // get object
        var obj = objects[t];

        // get objects screen tile location
        var screenLocation = this.worldToScreen(new clsVector2D(obj.x, obj.y));
        var tile = this.tiles[screenLocation.x][screenLocation.y].element;
        
        // only update cubes that have not yet scrolled off the buffer
        if (tile != null) {

            if (obj.deleted == true) {
                // erase object

                // loop through the existing elements in this tile.
                for (i = 0; i < tile.children.length; i++) {
                    if (obj.id == tile.children[i].id) {
                        tile.removeChild(tile.children[i]);
                        break; // either way break out of the for loop when done
                    }
                }
            }
            else {

                // this is a modified object erase the old one
                if (obj.created != obj.modified) {
                    // loop through the existing elements in this tile.
                    for (i = 0; i < tile.children.length; i++) {
                        if (obj.id == tile.children[i].id) {
                            tile.removeChild(tile.children[i]); // remove it 
                            break; // either way break out of the for loop when done
                        }
                    }
                }

                // draw new object
                var ele = drawObject(obj);
                ele.id = obj.id;
                tile.appendChild(ele);
                tile.style.backgroundImage = ""; // clear default background
            }
        }
    }
}


function drawObject(obj) {
    // this should be a generic function that creates objects base don a template in the database
        
    // create div 
    var div = document.createElement('div');
    div.className = "object divDefault";
    div.setAttribute("data", JSON.stringify(obj));
    div.style.top = (-(obj.z * 32)) + "px";

    // add events
    div.onmousedown = function () { client.worldView.ground.onClickObject(this); };
    div.addEventListener("contextmenu", function (e) { e.preventDefault(); });

    // create image
    var img = document.createElement('img');
    img.src = "../images/world/" + obj.item + ".png";
    img.className = "object imgDefault";
    div.appendChild(img); // put image in container

    return div;
}


// scroll the entire landscape one cube in any direction
clsGround.prototype.shiftCubes = function(shiftx, shifty) {

    console.log("Shifting cubes... (" + shiftx + ", " + shifty + ")");

    // pre calc last element id
    var last = this.buffer.size - 1; // last row or column. They are the same because its a square.

    // set new world location
    this.world = new clsVector2D(this.world.x += shiftx, this.world.y += shifty);
    this.worldBottomRight = new clsVector2D(this.world.x + this.buffer.size - 1, this.world.y + last);

    // determine the best copy direction based on the y shift direction
    var yfrom, yto, yinc;
    if (shifty == 0) {
        yfrom = 0;
        yto = last;
        yinc = 1;
    } else if (shifty == 1) {
        yfrom = 0;
        yto = last - 1; 
        yinc = 1;
    } else {
        yfrom = last;
        yto = 1;
        yinc = -1;
    }

    // determine the best copy direction based on the x shift direction
    var xfrom, xto, xinc;
    if (shiftx == 0) {
        xfrom = 0;
        xto = last;
        xinc = 1;
    } else if (shiftx == 1) {
        xfrom = 0;
        xto = last - 1;
        xinc = 1;
    } else {
        xfrom = last;
        xto = 1;
        xinc = -1;
    }

    // loop the the rows(y)
    var y = yfrom;
    while (((y >= yfrom) && (y <= yto)) || ((y <= yfrom) && (y >= yto))) {

        // loop the the columns(x)
        var x = xfrom;
        while (((x >= xfrom) && (x <= xto)) || ((x <= xfrom) && (x >= xto))) {

            // calculate the source location for each tiles information
            var sourcex = utils.wrap(x + shiftx, 0, last);
            var sourcey = utils.wrap(y + shifty, 0, last);
            this.copyTile(this.tiles[sourcex][sourcey].element, this.tiles[x][y].element); // copy most tiles
           
            x += xinc;
        }
        y += yinc;
    }

    if (shiftx == 1) {
        // request last row
        var worldRow1 = this.screenToWorld(new clsVector2D(last, 0));
        var worldRow2 = this.screenToWorld(new clsVector2D(last, last));
        wsi.requestJSONInfo({ "callName": "getArea", "x1": worldRow1.x, "y1": worldRow1.y, "x2": worldRow2.x, "y2": worldRow2.y }, client.worldView.ground.objectsResponse);
    } else if (shiftx == -1) {
        // request first row
        var worldRow1 = this.screenToWorld(new clsVector2D(0, 0));
        var worldRow2 = this.screenToWorld(new clsVector2D(0, last));
        wsi.requestJSONInfo({ "callName": "getArea", "x1": worldRow1.x, "y1": worldRow1.y, "x2": worldRow2.x, "y2": worldRow2.y }, client.worldView.ground.objectsResponse);
    }

    if (shifty == 1) {
        // request last col
        var worldCol1 = this.screenToWorld(new clsVector2D(0, last));
        var worldCol2 = this.screenToWorld(new clsVector2D(last, last));
        wsi.requestJSONInfo({ "callName": "getArea", "x1": worldCol1.x, "y1": worldCol1.y, "x2": worldCol2.x, "y2": worldCol2.y }, client.worldView.ground.objectsResponse);
    } else if (shifty == -1) {
        // request first col
        var worldCol1 = this.screenToWorld(new clsVector2D(0, 0));
        var worldCol2 = this.screenToWorld(new clsVector2D(last, 0));
        wsi.requestJSONInfo({ "callName": "getArea", "x1": worldCol1.x, "y1": worldCol1.y, "x2": worldCol2.x, "y2": worldCol2.y }, client.worldView.ground.objectsResponse);
    }

}

clsGround.prototype.copyTile = function (source, destination) {

    destination.innerHTML = ""; // clear destination tile

    // move elements from source tile
    while (source.hasChildNodes()) {
        var ele = source.removeChild(source.childNodes[0]);
        destination.appendChild(ele);
    }

    // copy tile guides
    destination.style.background = source.style.background; // copy cube image and and tile info
}

clsGround.prototype.onClickTile = function(element) {
    console.log("Clicked tile.");

    // get info about the click
    var screenLocation = new clsVector2D(Number(element.dataset.x), Number(element.dataset.y)); // get screen location clicked
    var worldLocation = client.worldView.ground.screenToWorld(screenLocation); // get world location clicked

    // which click type was it
    var rightclick;
    if (!e) var e = window.event;
    if (e.which) rightclick = (e.which == 3);
    else if (e.button) rightclick = (e.button == 2);


    if (rightclick == true) {
        // right click
        //var br = Math.floor(Math.random() * 2);
        //client.setObject(worldLocation.x, worldLocation.y,"cubes", "bedrock" + br);
    }
    else {
        // left click

        // reload with new location as center
        client.playerMoveTarget = new clsVector2D(worldLocation.x, worldLocation.y);
    }

    //return false; // don't show default right click menu
    e.preventDefault();
}

clsGround.prototype.onClickObject = function(element) {
    console.log("Clicked object.");

    // get info about the click
    var obj = JSON.parse(element.getAttribute("data")); // get object information
    var worldLocation = new clsVector2D(obj.x, obj.y);
    var screenLocation = this.worldToScreen(worldLocation); // get screen location clicked
    //var worldLocation = client.worldView.ground.screenToWorld(screenLocation); // get world location clicked
    

    // identify click type (left right middle)
    var rightclick;
    if (!e) var e = window.event;
    if (e.which) rightclick = (e.which == 3);
    else if (e.button) rightclick = (e.button == 2);


    if (rightclick == true) {
        // right click
        console.log("Right click stone @world(" + obj.x + "," + obj.y + ") and @screen(" + screenLocation.x + "," + screenLocation.y + ")");

        // create object based on current template
        client.createObject(obj.x, obj.y, obj.z + 1, client.packView.currentTemplateId);

        // delete and redraw everything in tile            
        //client.deleteObject(obj.id);
    }
    else {
        // left click
        //console.log("Left click stone @world(" + obj.x + "," + obj.y + ") and @screen(" + screenLocation.x + "," + screenLocation.y + ")");
    }

    //return false; // don't show default right click menu
    e.preventDefault();
}

clsGround.prototype.update = function () {

    console.log("Requesting objects modified since " + this.lastUpdate + " in (" + this.world.x + "," + this.world.y + " - " + this.worldBottomRight.x + "," + this.worldBottomRight.y + ")");
    //wsi.requestJSONInfo({ "callName": "getArea", "x1": this.world.x, "y1": this.world.y, "x2": this.worldBottomRight.x, "y2": this.worldBottomRight.y, "modified": utils.wsFriendlyDateTime(this.lastUpdate) }, JSONResponseHandler);
    wsi.requestJSONInfo({ "callName": "getArea", "x1": this.world.x, "y1": this.world.y, "x2": this.worldBottomRight.x, "y2": this.worldBottomRight.y, "modified": utils.wsFriendlyDateTime(this.lastUpdate) }, client.worldView.ground.objectsResponse);
    this.lastUpdate = new Date();
}
