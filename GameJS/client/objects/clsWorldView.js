﻿/*******************************************************************
    World View - This is a portal view in to the game world
    the goal is to design it so that you can have more then one running
********************************************************************/

// report version
console.log("=== included clsWorldView.js ver 0.1 ===");

function clsWorldView(width, height) {
    console.log("Creating world view...");

    // world view display panel
    this.displayPanel = new clsDisplayPanel("playerPanel", width, height);

    // create grid view inside of the display panel
    this.grid = new clsGrid(this.displayPanel);

    // set the intial location
    this.location = new clsPoint(0, 0); // world coordinate the view is currently centered on
}

// center view on a specific location
clsWorldView.prototype.jumpToLocation = function (worldx, worldy) {
    console.log("Jump from (" + this.location.x + "," + this.location.y + ") to (" + worldx + "," + worldy + ")");

    this.location.x = worldx;
    this.location.y = worldy;

    // calculate top, left corner in world since ground does not accept center location
    var worldViewTopLeft = new clsPoint(worldx - (this.grid.buffer.size / 2), worldy - (this.grid.buffer.size / 2));
    this.grid.setWorldLocation(worldViewTopLeft);
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

    this.grid.shiftGrid(shiftx, shifty);
}

/**********************************************
    Conversion functions
***********************************************/
clsWorldView.prototype.worldToScreen = function (worldLocation) {
    return new clsPoint(worldLocation.x - this.grid.world.x, worldLocation.y - this.grid.world.y);
}

// it may be a fluke but this function did not work last time I used to. (probably should never never need it);
clsWorldView.prototype.screenToWorld = function (screenLocation) {
    return new clsPoint(this.grid.world.x + screenLocation.x, this.grid.world.y + screenLocation.y);
}





