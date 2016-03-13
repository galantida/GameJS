/*******************************************************************
    Ground Object  - This object is used to manage the ground cubes
                        displayed in the worldview
********************************************************************/


// report version
console.log("=== included clsGrid.js ver 0.1 ===");

function clsGrid(displayPanel) {
    console.log("Creating Display Grid...");

    // properties
    this.world = new clsVector2D(0, 0);
    this.worldBottomRight = new clsVector2D(0, 0);

    // initializations
    this.cs = { "width":64, "height":64, "displayWidth":64, "displayHeight":32 };
    this.createBuffer(displayPanel); // create and position buffer element based on display panel size

    // request regular updates without user interaction
    this.lastUpdate = new Date("3/19/69");
    this.heartBeat = setInterval(function () { client.worldView.grid.update(); }, 1000);
    //this.heartBeat = setTimeout(function () { client.worldView.grid.update(); }, 1000); // singel call for debug only
}

/**********************************************
    Initialization functions
***********************************************/
clsGrid.prototype.createBuffer = function (displayPanel) {
    console.log("Creating Buffer...");

    this.buffer = { "border": 2 }; // minimum border in cubes to load off screen 

    // calculate buffer size (based on display hypotenuse and cubesizes)
    this.buffer.size = Math.ceil(displayPanel.hypotenuse() / this.cs.displayHeight) + (this.buffer.border * 2); // calculate buffer size in cubes
    this.buffer.displayHeight = this.buffer.size * this.cs.displayHeight; // actual vertical pixels on screen
    this.buffer.displayWidth = this.buffer.size * this.cs.displayWidth; // actual horizontal pixels on screen

    // create and position buffer element based on display panel size
    var ele = document.createElement('div');
    ele.setAttribute('id', "Buffer");
    ele.className = "clsGrid buffer"
    ele.style.width = this.buffer.displayWidth + "px";
    ele.style.height = this.buffer.displayHeight + "px";
    //ele.style.left = ((displayPanel.width() / 2) - (this.cs.displayWidth / 2)) + "px"; // move right to the center of the display area
    //ele.style.top = ((displayPanel.height() / 2) - (this.buffer.displayHeight / 2) - (this.cs.displayHeight / 2)) + "px"; // move down half the display and back up half the buffer
    ele.style.left = ((displayPanel.width() / 2) - (this.buffer.displayWidth / 2)) + "px"; // move right to the center of the display area
    ele.style.top = ((displayPanel.height() / 2) - (this.buffer.displayHeight / 2)) + "px"; // move down half the display and back up half the buffer
    this.buffer.element = ele;

    displayPanel.element.appendChild(this.buffer.element);

    // create the grid that we will fill with objects
    this.createGrid((this.buffer.element.offsetWidth / 2), +64); 
}

clsGrid.prototype.createGrid = function (posx, posy) {
    console.log("Creating Actual Grid...");

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

            // set element properties
            ele.style.left = ((((x * .5) - (y * .5)) * this.cs.displayWidth) + posx) + "px";
            ele.style.top = ((((x * .5) + (y * .5)) * this.cs.displayHeight) + posy) + "px";

            // add elements events
            ele.onmouseup = function () { client.worldView.grid.onClick(this); };

            // add dragable events
            ele.ondragstart = function () { client.worldView.grid.onDragStart(this); };
            ele.ondrag = function () { client.worldView.grid.onDrag(this); };
            ele.ondragend = function () { client.worldView.grid.onDragEnd(this); };

            // add drag target events
            ele.ondragenter = function () { client.worldView.grid.onDragEnter(this); };
            ele.ondragover = function () { client.worldView.grid.onDragOver(this); };
            ele.ondragleave = function () { client.worldView.grid.onDragLeave(this); };
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
    console.groupCollapsed("Updating grid with a " + objects.length + " object list...");

    // update new objects
    for (var t = 0; t < objects.length; t++) {

        // get object
        var obj = objects[t];

        // get objects screen grid location
        var screenLocation = client.worldView.worldToScreen(new clsVector2D(obj.x, obj.y));
        var tile = this.grid[screenLocation.x][screenLocation.y].element;

        console.dir(obj);
        
        // only update cubes that have not yet scrolled off the buffer
        if (tile == null) console("Object is now off the screen");
        else {
            if (obj.deleted == true) {
                // erase object
                console.log("Delete existing element.");

                // loop through the existing elements in this tile.
                for (i = 0; i < tile.children.length; i++) {
                    if (tile.children[i].id == ("obj" + obj.id)) {
                        console.log("Deleting existing element 'obj" + obj.id + "'");
                        tile.removeChild(tile.children[i]);
                        break; // either way break out of the for loop when done
                    }
                }
            }
            else {

                var ele = document.getElementById("obj" + obj.id);
                if (ele != null) {

                    // existing element already exists
                    var eleObj = JSON.parse(ele.getAttribute("data"));

                    // if this update for this object newer then our last  
                    if (new Date(eleObj.elementLastUpdated) < new Date(obj.modified)) {

                        // modifed existing element
                        console.log("Modify existing element.");
                        obj.elementLastUpdated = new Date(); // add an elementCreated date to the object
                        ele.setAttribute("data", JSON.stringify(obj));
                        object.position(ele.firstElementChild);
                        //ele.style.top = (-((obj.z + 1) * 32)) + "px";
                        tile.appendChild(ele); // move element to its new tile location
                    }
                    else {
                        console.log("Already updated element.");
                    }

                }
                else {
                    // create new element
                    console.log("Create new element.");
                    var ele = object.draw(obj);
                    tile.appendChild(ele);
                    tile.style.backgroundImage = ""; // clear default background
                }
            }
        }
    }
    console.groupEnd();
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

    // loop through the rows(y)
    var y = yfrom;
    while (((y >= yfrom) && (y <= yto)) || ((y <= yfrom) && (y >= yto))) {

        // loop through the columns(x)
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

    // request new row information
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

    // get the event
    if (!e) var e = window.event;
    e.stopPropagation();

    if (dragflag == false) { // allow for click event on mouse up only if nothing was being dragged

        // get info about the click
        var screenLocation = new clsVector2D(Number(element.dataset.x), Number(element.dataset.y)); // get screen location clicked
        var worldLocation = client.worldView.screenToWorld(screenLocation); // get world location clicked

        // which click type was it
        var rightclick;
        if (e.which) rightclick = (e.which == 3);
        else if (e.button) rightclick = (e.button == 2);

        if (rightclick == true) {
            console.log("Right clicked grid at world (" + worldLocation.x + "," + worldLocation.y + ") at screen(" + screenLocation.x + "," + screenLocation.y + ")");
            // right click
            //var br = Math.floor(Math.random() * 2);
            //client.setObject(worldLocation.x, worldLocation.y,"cubes", "bedrock" + br);
        }
        else {
            // left click
            console.log("Left clicked grid at world (" + worldLocation.x + "," + worldLocation.y + ") at screen(" + screenLocation.x + "," + screenLocation.y + ")");

            // move play to new location. screen will follow
            client.playerMoveTarget = new clsVector2D(worldLocation.x, worldLocation.y);
        }
    }
    e.preventDefault();
}

// drag events drag target
clsGrid.prototype.onDragStart = function (element) {

}

clsGrid.prototype.onDrag = function (element) {

}

clsGrid.prototype.onDragEnd = function (element) {

}

//drag events drop target
clsGrid.prototype.onDragEnter = function (element) {
    // add selected class
    element.classList.add('over');

}

clsGrid.prototype.onDragLeave = function (element) {
    // remove selected class
    element.classList.remove('over');

}

clsGrid.prototype.onDragOver = function (element) {
    if (!e) var e = window.event;
    e.preventDefault();
}

clsGrid.prototype.onDrop = function (element) {

    // get event information
    if (!e) var e = window.event; // get event
    e.preventDefault();
    //e.stopPropagation(); // not nessisary

    dragflag = false; // allow for click event on mouse up if nothing was dragged
    element.classList.remove('over'); // remove class

    // get dragged information
    var srcObj = JSON.parse(e.dataTransfer.getData("text"));

    // get drop location information
    var screenLocation = new clsVector2D(Number(element.dataset.x), Number(element.dataset.y)); // get screen location
    var worldLocation = client.worldView.screenToWorld(screenLocation); // get world location
    console.groupCollapsed("dropped " + JSON.stringify(srcObj) + " on grid " + screenLocation.x + "," + screenLocation.y)

    switch (srcObj.dragType) {
        case "template":
            // create object based on dropped template
            console.log("creating new object at world (" + worldLocation.x + ", " + worldLocation.y + ", " + worldLocation.z + ") based on template " + JSON.stringify(srcObj));
            client.createObject(worldLocation.x, worldLocation.y, 0, srcObj.id);
            break;

        case "object":
            // delete original object
            console.log("Deleting element 'obj" + srcObj.id + "' for object " + JSON.stringify(srcObj));
            var srcEle = document.getElementById("obj" + srcObj.id);
            srcEle.parentNode.removeChild(srcEle);

            // move object to new location
            console.log("Creating new element 'obj" + srcObj.id + "' for object " + JSON.stringify(srcObj));
            client.updateObject(srcObj.id, worldLocation.x, worldLocation.y, 0, srcObj.pack, srcObj.item);
            break;
    }
    console.groupEnd();
}

clsGrid.prototype.update = function () {

    console.log("Requesting objects modified since " + this.lastUpdate + " in (" + this.world.x + "," + this.world.y + " - " + this.worldBottomRight.x + "," + this.worldBottomRight.y + ")");
    wsi.requestJSONInfo({ "callName": "getArea", "x1": this.world.x, "y1": this.world.y, "x2": this.worldBottomRight.x, "y2": this.worldBottomRight.y, "modified": utils.wsFriendlyDateTime(this.lastUpdate) }, client.worldView.grid.objectsResponse);
    this.lastUpdate = new Date();
}
