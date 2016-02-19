/*******************************************************************
    Display Panel - The display panel is used as a generic portal container
    the plan is add functionality here once 
    so that other portals can benifit
********************************************************************/


// report version
console.log("=== included clsDisplayPanel.js ver 0.1 ===");

function clsDisplayPanel(id, width, height) {
    // create display element
    this.element = document.createElement("div");
    this.element.setAttribute("id", id);
    this.element.style.width = width + "px";
    this.element.style.height = height + "px";
    this.element.className = "clsDisplayPanel_panel";
    document.body.appendChild(this.element);
}

// example of a function added to the prototype (this.value works)
clsDisplayPanel.prototype.width = function () {
    return this.element.offsetWidth;
};

clsDisplayPanel.prototype.height = function () {
    return this.element.offsetHeight;
};

clsDisplayPanel.prototype.hypotenuse = function () {
    return Math.sqrt(Math.pow(this.width(), 2) + Math.pow(this.height(), 2));
}


