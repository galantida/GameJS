﻿/*******************************************************************
    Ground Object  - This object is used to manage the ground cubes
                        displayed in the worldview
********************************************************************/


// report version
console.log("=== included clsGrid.js ver 0.1 ===");

function clsGrid(displayPanel) {
    console.log("Creating grid...");

    // properties
    this.world = new clsVector2D(0, 0);
    this.worldBottomRight = new clsVector2D(0, 0);

    // initializations
    this.cs = { "width":64, "height":64, "displayWidth":64, "displayHeight":32 };
    this.buffer = this.createBuffer(displayPanel); // create and position buffer element based on screen size and position

    // rename to grid!!!!
    this.createGrid(); // create the tile that we will fill with objects

    // request regular updates without user interaction
    this.lastUpdate = new Date("3/19/69");
    this.heartBeat = setInterval(function() { client.worldView.grid.update(); }, 1000);
}

/**********************************************
    Initialization functions
***********************************************/
clsGrid.prototype.createBuffer = function (displayPanel) {
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
    buffer.element.className = "clsGrid buffer"
    //buffer.element.style.border = "1px solid red";
    displayPanel.element.appendChild(buffer.element);
    return buffer;
}

clsGrid.prototype.createGrid = function () {
    console.log("Creating Grid...");

    // create cube array for faster access
    this.grid = new Array(this.buffer.size); // create row array with global scope
    for (var x = 0; x < this.buffer.size; x++) {
        this.grid[x] = new Array(this.buffer.size); // add column array to each row
        for (var y = 0; y < this.buffer.size; y++) {
            this.grid[x][y] = {}; // could add other properties to the object array here.
        }
    }

    // create html grid elements
    for (var y = 0; y < this.buffer.size ; y++) {
        for (var x = 0; x < this.buffer.size; x++) {

            // create cube div element
            var ele = document.createElement('div');
            ele.className = "clsGrid grid";

            // set dataset properties
            ele.dataset.x = x;
            ele.dataset.y = y;
            //ele.dataset.defaultLeft = (((x * .5) - (y * .5)) * this.cs.displayWidth);
            //ele.dataset.defaultTop = (((x * .5) + (y * .5)) * this.cs.displayHeight);

            // set element properties
            ele.style.left = (((x * .5) - (y * .5)) * this.cs.displayWidth) + "px";
            ele.style.top = (((x * .5) + (y * .5)) * this.cs.displayHeight) + "px";

            // set element events
            // add events
            ele.onmouseup = function () { client.worldView.grid.onClick(this); };
            ele.ondragover = function () { client.worldView.grid.onDragOver(this); };
            ele.ondrop = function () { client.worldView.grid.onDrop(this); };
            ele.addEventListener("contextmenu", function (e) { e.preventDefault(); });
            this.buffer.element.appendChild(ele); // add the cube element to the buffer

            // add element to array for faster access
            this.grid[x][y].element = ele;
        }
    }
}



clsGrid.prototype.setWorldLocation = function (worldLocation) {
    // set new world location
    this.world = worldLocation
    this.worldBottomRight = new clsVector2D(this.world.x + this.buffer.size - 1, this.world.y + this.buffer.size - 1);
}


/**********************************************
    screen usage function
***********************************************/

// sets all ground cubes in a specificed area to cube 0,0
clsGrid.prototype.clearArea = function (x1, y1, x2, y2) {
    console.log("Clearing Grid (" + x1 + "," + y1 + " - " + x2 + "," + y2 + ")");
    for (var y = y1; y <= y2 ; y++) {
        for (var x = x1; x <= x2; x++) {
            var ele = this.grid[x][y].element;
            ele.innerHTML = "";
            ele.style.background = "url('../images/tiles/noTile.png')"; // set empty images to noTile background
        }
    }
}

clsGrid.prototype.clearAll = function (x1, y1, x2, y2) {
    this.clearArea(0, 0, this.buffer.size-1, this.buffer.size-1);
}


// sets the top left of the ground ground to a specified world location
clsGrid.prototype.jumpToLocation = function (worldx, worldy) {
    console.log("Jumping top left of view to world location (" + worldx + ", " + worldy + ")");

    // set new world location
    this.setWorldLocation(new clsVector2D(worldx, worldy));
    this.clearAll(); // clear all existing cubes

    // request objects
    wsi.requestJSONInfo({ "callName": "getArea", "x1": worldx, "y1": worldy, "x2": this.worldBottomRight.x, "y2": this.worldBottomRight.y }, client.worldView.ground.objectsResponse);
}

// not sure why we need this to handle the response but it works
clsGrid.prototype.objectsResponse = function (response) {
    client.worldView.grid.updateObjects(response.content);
}


// given a list of world cubes this will refresh their counter parts on the screen if they are still visible
// this is run for all responses from the server that return updated cube information
clsGrid.prototype.updateObjects = function (objects) {
    console.log("Updating grid with new object list...");

    // update new objects
    for (var t = 0; t < objects.length; t++) {

        // get object
        var obj = objects[t];

        // get objects screen grid location
        var screenLocation = client.worldView.worldToScreen(new clsVector2D(obj.x, obj.y));
        var tile = this.grid[screenLocation.x][screenLocation.y].element;
        
        // only update cubes that have not yet scrolled off the buffer
        if (tile != null) {

            if (obj.deleted == true) {
                // erase object

                // loop through the existing elements in this tile.
                for (i = 0; i < tile.children.length; i++) {
                    if (tile.children[i].id == ("obj" + obj.id)) {
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
                        if (tile.children[i].id == ("obj" + obj.id)) {
                            tile.removeChild(tile.children[i]); // remove it 
                            break; // either way break out of the for loop when done
                        }
                    }
                }

                // draw new object
                var ele = object.draw(obj);
                tile.appendChild(ele);
                tile.style.backgroundImage = ""; // clear default background
            }
        }
    }
}

// scroll the entire landscape one cube in any direction
clsGrid.prototype.shiftGrid = function(shiftx, shifty) {

    console.log("Shifting grid... (" + shiftx + ", " + shifty + ")");

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
            this.copyTileContents(this.grid[sourcex][sourcey].element, this.grid[x][y].element); // copy most tiles
           
            x += xinc;
        }
        y += yinc;
    }

    if (shiftx == 1) {
        // request last row
        var worldRow1 = client.worldView.screenToWorld(new clsVector2D(last, 0));
        var worldRow2 = client.worldView.screenToWorld(new clsVector2D(last, last));
        wsi.requestJSONInfo({ "callName": "getArea", "x1": worldRow1.x, "y1": worldRow1.y, "x2": worldRow2.x, "y2": worldRow2.y }, client.worldView.grid.objectsResponse);
    } else if (shiftx == -1) {
        // request first row
        var worldRow1 = client.worldView.screenToWorld(new clsVector2D(0, 0));
        var worldRow2 = client.worldView.screenToWorld(new clsVector2D(0, last));
        wsi.requestJSONInfo({ "callName": "getArea", "x1": worldRow1.x, "y1": worldRow1.y, "x2": worldRow2.x, "y2": worldRow2.y }, client.worldView.grid.objectsResponse);
    }

    if (shifty == 1) {
        // request last col
        var worldCol1 = client.worldView.screenToWorld(new clsVector2D(0, last));
        var worldCol2 = client.worldView.screenToWorld(new clsVector2D(last, last));
        wsi.requestJSONInfo({ "callName": "getArea", "x1": worldCol1.x, "y1": worldCol1.y, "x2": worldCol2.x, "y2": worldCol2.y }, client.worldView.grid.objectsResponse);
    } else if (shifty == -1) {
        // request first col
        var worldCol1 = client.worldView.screenToWorld(new clsVector2D(0, 0));
        var worldCol2 = client.worldView.screenToWorld(new clsVector2D(last, 0));
        wsi.requestJSONInfo({ "callName": "getArea", "x1": worldCol1.x, "y1": worldCol1.y, "x2": worldCol2.x, "y2": worldCol2.y }, client.worldView.grid.objectsResponse);
    }

}

clsGrid.prototype.copyTileContents = function (source, destination) {

    destination.innerHTML = ""; // clear destination tile

    // move elements from source tile
    while (source.hasChildNodes()) {
        var ele = source.removeChild(source.childNodes[0]);
        destination.appendChild(ele);
    }

    // copy tile guides
    destination.style.background = source.style.background; // copy cube image and and tile info
}

clsGrid.prototype.onClick = function (element) {

    // which click type was it
    var rightclick;
    if (!e) var e = window.event;
    if (e.which) rightclick = (e.which == 3);
    else if (e.button) rightclick = (e.button == 2);

    if (dragflag == false) { // allow for click event on mouse up if nothing was dragged
        console.log("Clicked tile.");

        // get info about the click
        var screenLocation = new clsVector2D(Number(element.dataset.x), Number(element.dataset.y)); // get screen location clicked
        var worldLocation = client.worldView.screenToWorld(screenLocation); // get world location clicked


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
    }

    //return false; // don't show default right click menu
    e.preventDefault();
}


clsGrid.prototype.onDragOver = function (element) {
    if (!e) var e = window.event;
    e.preventDefault();
}

clsGrid.prototype.onDrop = function (element) {

    // get event information
    if (!e) var e = window.event; // get event
    e.preventDefault();
    //e.stopPropagation();

    dragflag = false; // allow for click event on mouse up if nothing was dragged

    // get dragged information
    var srcObj = JSON.parse(e.dataTransfer.getData("text"));

    // get drop location information
    var screenLocation = new clsVector2D(Number(element.dataset.x), Number(element.dataset.y)); // get screen location
    var worldLocation = client.worldView.screenToWorld(screenLocation); // get world location

    console.log("dropped " + JSON.stringify(srcObj) + " on Grid " + worldLocation.x + "," + worldLocation.y)

    switch (srcObj.dragType) {
        case "template":
            {
                // create object based on dropped template
                client.createObject(worldLocation.x, worldLocation.y, 0, srcObj.id);
                break;
            }
        case "object":
            {
                // delete original object
                //console.log("deleting " + "obj" + srcObj.id);
                var srcEle = document.getElementById("obj" + srcObj.id);
                srcEle.parentNode.removeChild(srcEle);

                // move object to new location
                client.updateObject(srcObj.id, worldLocation.x, worldLocation.y, 0, srcObj.pack, srcObj.item);
                break;
            }
    }
}

clsGrid.prototype.update = function () {

    console.log("Requesting objects modified since " + this.lastUpdate + " in (" + this.world.x + "," + this.world.y + " - " + this.worldBottomRight.x + "," + this.worldBottomRight.y + ")");
    //wsi.requestJSONInfo({ "callName": "getArea", "x1": this.world.x, "y1": this.world.y, "x2": this.worldBottomRight.x, "y2": this.worldBottomRight.y, "modified": utils.wsFriendlyDateTime(this.lastUpdate) }, JSONResponseHandler);
    wsi.requestJSONInfo({ "callName": "getArea", "x1": this.world.x, "y1": this.world.y, "x2": this.worldBottomRight.x, "y2": this.worldBottomRight.y, "modified": utils.wsFriendlyDateTime(this.lastUpdate) }, client.worldView.grid.objectsResponse);
    this.lastUpdate = new Date();
}
