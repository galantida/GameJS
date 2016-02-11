/*******************************************************************
    World View - This is a portal view in to the game world
    the goal is to design it so that you can have more then one running
********************************************************************/

// report version
console.log("=== included clsWorldView.js ver 0.1 ===");

function clsWorldView(screenx, screeny, width, height) {
    console.log("Creating world view...");

    // world view display panel
    this.displayPanel = new clsDisplayPanel("playerPanel", screenx, screeny, width, height);

    // locations
    this.location = new clsVector2D(0, 0); // world coordinate the view is currently centered on

    // make a new ground object in the panel the display panel
    this.ground = new clsGround(this.displayPanel);
}

// refresh all tiles
clsWorldView.prototype.update = function () {
    console.log("update world view.");
    this.ground.update();
}

// center view on a specific location
clsWorldView.prototype.jumpToLocation = function (worldx, worldy) {
    console.log("Jump from (" + this.location.x + "," + this.location.y + ") to (" + worldx + "," + worldy + ")");

    this.location.x = worldx;
    this.location.y = worldy;

    // calculate top, left corner in world since ground does not accept center location
    var worldViewTopLeft = new clsVector2D(worldx - (this.ground.buffer.size / 2), worldy - (this.ground.buffer.size / 2));
    this.ground.setWorldLocation(worldViewTopLeft);
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

    this.ground.shiftGrid(shiftx, shifty);
}





