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

    console.log("Move Top Left cornor of World View to location (" + worldViewx + ", " + worldViewy + ")");

    this.worldView.jumpToLocation(worldViewx, worldViewy);
}

clsClient.prototype.process = function () {
    this.worldView.process();
}



