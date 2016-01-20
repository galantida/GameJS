/*******************************************************************
    Client Object  - This object in the master container for 
                        all other objects in the interface


********************************************************************/

// report version
console.log("=== included clsClient.js ver 0.1 ===");

function clsClient() {

    // settings
    this.player = new clsVector2D(0, 0);

    this.worldView = new clsWorldView(350, 150, 320, 200);
    // playerPanel2 = new clsWorldView(250, 250, 320, 200); testing only

    this.worldView.jumpToLocation(this.player.x, this.player.y);
}

clsClient.prototype.playerMoveRequest = function(worldx, worldy) {
    console.log("Player move request to world location (" + worldx + ", " + worldy + ")");

    this.player.x = worldx;
    this.player.y = worldy;

    // jump view to center this world location
    var worldViewx = this.player.x - (this.worldView.ground.buffer.size / 2)
    var worldViewy = this.player.y - (this.worldView.ground.buffer.size / 2)

    this.worldView.jumpToLocation(worldViewx, worldViewy);
}

// sets the ground to a specified location
clsClient.prototype.setTile = function (worldx, worldy, tilesetId, col, row) {

    console.log("Saving tile...");

    // update/create a tile
    // {"id ":1,"x":0,"y":0,"z":0,"tilesetId ":0,"col":0,"row ":0}
    var tile = {"x":worldx, "y":worldy, "tileset": tilesetId, "col": col, "row": row }

    var paramObj = tile;
    paramObj.callName = "setTile";

    // request tiles
    wsi.requestJSONInfo(paramObj, JSONResponseHandler);
}

clsClient.prototype.process = function () {
    this.worldView.process();
}



