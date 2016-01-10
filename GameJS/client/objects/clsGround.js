function clsGround(displayPanel) {
    this.desiredShift = new clsVector2D(12, 5); // screen coordinates to scroll to
    this.tileset = new clsTileset("yarsTileset.png", 64, 10, 12); // load tileset
    this.buffer = this.createBuffer(displayPanel); // load buffer
    this.createTiles(); // create random map this will be a JSON map load in the future
}

clsGround.prototype.createBuffer = function (displayPanel) {
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

    // create tile array
    this.tiles = new Array(this.buffer.size); // create row array
    for (var x = 0; x < this.buffer.size; x++) {
        this.tiles[x] = new Array(this.buffer.size); // add column array to each row
    }

    // create tile div elements
    for (var y = 0; y < this.buffer.size ; y++) {
        for (var x = 0; x < this.buffer.size; x++) {
            // create tile div element
            var ele = this.createDiv(x + "-" + y, "clsGround tile");
            ele.style.left = (((x * .5) - (y * .5)) * this.tileset.displayWidth) + "px";
            ele.style.top = (((x * .5) + (y * .5)) * this.tileset.displayHeight) + "px";
            ele.onmousedown = function() { playerPanel.ground.ScrollToTileLocation(this.id); };
            this.tiles[x][y] = ele; // add new elements to to ground tile array
            this.buffer.element.appendChild(ele); // add the tile element to the buffer
        }
    }

    // add tile elements to the array
    for (var y = 0; y < this.buffer.size; y++) {
        for (var x = 0; x < this.buffer.size; x++) {
            // display tile
            var tile = {};
            tile.x = 1;
            tile.y = 1;
            tile.x = Math.floor((Math.random() * 9));
            tile.y = Math.floor((Math.random() * 3));
            this.tiles[x][y].style.backgroundPosition = -tile.x + "em " + -tile.y + "em"; // display tile
        }
    }
}

clsGround.prototype.ScrollToTileLocation = function (tileID) {
    var location = tileID.split("-");
    var bufferCenter = Math.floor(this.buffer.size / 2);
    this.desiredShift.x = location[0] - bufferCenter;
    this.desiredShift.y = location[1] - bufferCenter;
}

clsGround.prototype.shiftTiles = function (shiftx, shifty) {

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
            this.tiles[x][y].style.backgroundPosition = this.tiles[xdes][ydes].style.backgroundPosition;
            x += xinc;
        }
        y += yinc;
    }
}

clsGround.prototype.createDiv = function (id, className) {
    var ele = document.createElement('div');
    ele.setAttribute('id', id);
    ele.className = className;
    return ele;
}


// not yet functioning
clsGround.prototype.tileRequest = function (from, to) {

    if (from.x > to.x) {
        var x = from.x;
        from.x = to.x;
        to.x = x;
    }

    if (from.y > to.y) {
        var y = from.y;
        from.y = to.y;
        to.y = y;
    }

    // add tile elements to the array
    for (var y = y1; y <= y2; y++) {
        for (var x = x1; x <= x2; x++) {
            // display tile
            var tile = {};
            tile.x = 1;
            tile.y = 1;
            tile.x = Math.floor((Math.random() * 7));
            tile.y = Math.floor((Math.random() * 4));
            display.tiles[x][y].style.backgroundPosition = -tile.x + "em " + -tile.y + "em"; // display tile
        }
    }
}

clsGround.prototype.tileResponse = function (xhr) {
    // loop through received tile array updating the appropriate screen tiles
    // tiles are in world coordinates (need a conversion function)
}

clsGround.prototype.process = function () {
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

