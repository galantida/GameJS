function clsWorldView(x, y, width, height) {

    // create display panel 
    this.displayPanel = new clsDisplayPanel("playerPanel", x, y, width, height);

    // add the landscape to the bufer
    this.ground = new clsGround(this.displayPanel);
}

clsWorldView.prototype.screenToWorld = function (screenLocation) {
    result = new clsVector2D(0, 0);
    result.x = display.worldLocation.x + screenLocation.x;
    result.y = display.worldLocation.y + screenLocation.y;
    return result;
}

clsWorldView.prototype.worldToScreen = function (worldLocation) {
    result = new clsVector2D(0, 0);
    result.x = display.worldLocation.x - screenLocation.x;
    result.y = display.worldLocation.y - screenLocation.y;
    return result;
}

clsWorldView.prototype.process = function () {
    this.displayPanel.process();
    this.ground.process();
}

