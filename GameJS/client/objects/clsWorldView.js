/*******************************************************************
    World View - This is a portal view in to the game world
    the goal is to design it so that you can have more then one running
********************************************************************/

// report version
console.log("=== included clsWorldView.js ver 0.1 ===");

function clsWorldView(screenx, screeny, width, height) {

    // world view display panel
    this.displayPanel = new clsDisplayPanel("playerPanel", screenx, screeny, width, height);

    // make a new ground object in the panel the display panel
    this.ground = new clsGround(this.displayPanel);

    // locations
    this.location = new clsVector2D(0, 0); // world coordinate the view is currently centered on
}

// center view on a specific location
clsWorldView.prototype.jumpToLocation = function (worldx, worldy) {
    console.log("Jump from (" + this.location.x + "," + this.location.y + ") to (" + worldx + "," + worldy + ")");

    this.location.x = worldx;
    this.location.y = worldy;

    // calculate top, left corner in world since ground does not accept center location
    var tmpx = worldx - (this.ground.buffer.size / 2);
    var tmpy = worldy - (this.ground.buffer.size / 2);
    this.ground.jumpToLocation(tmpx, tmpy);
}

// refresh all tiles
clsWorldView.prototype.refresh = function () {
    console.log("refresh using jump.");
    this.ground.jumpToLocation(this.ground.world.x, this.ground.world.y);
}


// scroll center view on a specific location
clsWorldView.prototype.moveTowardLocation = function (worldx, worldy) {
    console.log("Move from (" + this.location.x + "," + this.location.y + ") toward (" + worldx + "," + worldy + ")");

    // shift tiles in the right direction but no more then one tile
    var shiftx = 0;
    if (worldx > this.location.x) shiftx = 1;
    if (worldx < this.location.x) shiftx = -1;

    var shifty = 0;
    if (worldy > this.location.y) shifty = 1;
    if (worldy < this.location.y) shifty = -1;

    this.location.x += shiftx;
    this.location.y += shifty;

    this.ground.shiftTiles(shiftx, shifty);
}


clsWorldView.prototype.process = function () {
   
    
}



