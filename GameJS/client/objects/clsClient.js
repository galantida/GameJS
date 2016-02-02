/*******************************************************************
    Client Object  - This object in the master container for 
                        all other objects in the interface


********************************************************************/

// report version
console.log("=== included clsClient.js ver 0.1 ===");

function clsClient() {

    // settings
    this.player = new clsVector2D(0, 0);
    this.playerMoveTarget = new clsVector2D(0, 0); // world coordinates to scroll

    this.worldView = new clsWorldView(350, 150, 640, 480);
    //this.worldView = new clsWorldView(350, 150, 320, 200);
    this.packView = new clsContainerView(350, 150, 240, 480);

    

    this.worldView.jumpToLocation(this.player.x, this.player.y);
}

clsClient.prototype.setPlayerLocation = function (worldx, worldy) {
    console.log("Setting player to location (" + worldx + ", " + worldy + ")");

    this.player.x = worldx;
    this.player.y = worldy;

    this.playerMoveTarget.x = worldx;
    this.playerMoveTarget.y = worldy;

    // display new player location
    this.worldView.jumpToLocation(this.player.x, this.player.y);
}


// sets a tile to a specific graphic
clsClient.prototype.setCube = function (worldx, worldy, csId, csCol, csRow, tsId, tsCol, tsRow) {

    console.log("Saving cube...");

    // update/create a tile
    // {"id ":1,"x":0,"y":0,"z":0,"tilesetId ":0,"col":0,"row ":0}
    var tile = { "x": worldx, "y": worldy, "csId": csId, "csCol": csCol, "csRow": csRow, "tsId": tsId, "tsCol": tsRow, "tsRow": tsCol }

    var paramObj = tile;
    paramObj.callName = "setCube";

    // request cube
    wsi.requestJSONInfo(paramObj, JSONResponseHandler);
}

// sets a tile to a specific graphic
clsClient.prototype.setObject = function (worldx, worldy, pack, item) {

    console.log("Saving object...");

    // update/create a tile
    // {"id ":1,"x":0,"y":0,"z":0,"tilesetId ":0,"col":0,"row ":0}
    var obj = { "x": worldx, "y": worldy, "pack": pack, "item": item }

    var paramObj = obj;
    paramObj.callName = "setObject";

    // request cube
    wsi.requestJSONInfo(paramObj, JSONResponseHandler);
}

clsClient.prototype.process = function () {

    // process player moving
    if ((this.player.x != this.playerMoveTarget.x) || (this.player.y != this.playerMoveTarget.y)) {

        console.log("Moving player from (" + this.player.x + "," + this.player.y + ") toward (" + this.playerMoveTarget.x + "," + this.playerMoveTarget.y + ")");

        // move player in the right direction
        var shiftx = this.player.x - this.playerMoveTarget.x;
        if (shiftx > 0) shiftx = -1;
        else if (shiftx < 0) shiftx = 1;

        var shifty = this.player.y - this.playerMoveTarget.y;
        if (shifty > 0) shifty = -1;
        else if (shifty < 0) shifty = 1;

        this.player.x += shiftx;
        this.player.y += shifty;
    }
    

    // set world view target
    if ((this.worldView.location.x != this.player.x) || (this.worldView.location.y != this.player.y)) {
        // move player toward the desired location
        this.worldView.moveTowardLocation(this.player.x, this.player.y);
    }

    this.worldView.process();
}



