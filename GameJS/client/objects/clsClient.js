/*******************************************************************
    Client Object  - This object in the master container for 
                        all other objects in the interface


********************************************************************/

// report version
console.log("=== included clsClient.js ver 0.1 ===");

function clsClient() {

    // settings
    this.player = new clsPoint(0, 0);
    this.playerMoveTarget = new clsPoint(0, 0); // world coordinates to scroll

    //this.worldView = new clsWorldView(640, 480);
    this.worldView = new clsWorldView(320, 200);
    this.packView = new clsContainerView(284, 480);

    
    // set the initial player location
    this.setPlayerLocation(this.player.x, this.player.y);
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
clsClient.prototype.createObject = function (worldx, worldy, worldz, templateId) {

    console.log("Creating object...");

    // update/create a tile
    // {"id ":1,"x":0,"y":0,"z":0,"tilesetId ":0,"col":0,"row ":0}
    var obj = { "x": worldx, "y": worldy, "z": worldz, "templateId":templateId }

    var paramObj = obj;
    paramObj.callName = "createObject";

    // request cube
    wsi.requestJSONInfo(paramObj, client.worldView.grid.objectsResponse);
}

// sets a tile to a specific graphic
clsClient.prototype.deleteObject = function (id) {

    console.log("Deleting object...");

    // update/create a tile
    // {"id ":1,"x":0,"y":0,"z":0,"tilesetId ":0,"col":0,"row ":0}
    var obj = { "id": id }

    var paramObj = obj;
    paramObj.callName = "deleteObject";

    // request cube
    wsi.requestJSONInfo(paramObj, client.worldView.grid.objectsResponse);
}

// sets a tile to a specific graphic
clsClient.prototype.updateObject = function (id, worldx, worldy, worldz) {

    console.log("Updating object...");

    // update/create a tile
    // {"id ":1,"x":0,"y":0,"z":0,"tilesetId ":0,"col":0,"row ":0}
    var obj = { "id": id, "x": worldx, "y": worldy, "z": worldz }

    var paramObj = obj;
    paramObj.callName = "updateObject";

    // request cube
    wsi.requestJSONInfo(paramObj, client.worldView.grid.objectsResponse);
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
}



