/*******************************************************************
    World View - This is a portal view in to the game world
    the goal is to design it so that you can have more then one running
********************************************************************/

// report version
console.log("=== included clsWorldView.js ver 0.1 ===");

function clsWorldView(screenx, screeny, width, height) {

    // world view properties
    this.screenx = screenx;
    this.screeny = screeny;
    this.width = width;
    this.height = height;


    // world view display panel
    this.displayPanel = new clsDisplayPanel("playerPanel", screenx, screeny, width, height);

    // make a new ground object in the panel the display panel
    this.ground = new clsGround(this.displayPanel);
}

clsWorldView.prototype.jumpToLocation = function (worldx, worldy) {
    this.ground.jumpToLocation(worldx, worldy);
}

clsWorldView.prototype.process = function () {
    
}



