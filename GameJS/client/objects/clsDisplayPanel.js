function clsDisplayPanel(id, x, y, width, height) {
    // create display element
    this.element = document.createElement("div");
    this.element.setAttribute("id", id);
    //this.element.setAttribute("draggable", true);
    //this.element.ondragstart = "drag(event)";
    this.element.style.left = x + "px";
    this.element.style.top = y + "px";
    this.element.style.width = width + "px";
    this.element.style.height = height + "px";
    this.element.className = "clsDisplayPanel_panel";
    document.body.appendChild(this.element);
}

// example of a function added to the prototype (this.value works)
clsDisplayPanel.prototype.x = function () {
    return this.element.offsetLeft;
};

clsDisplayPanel.prototype.y = function () {
    return this.element.offsetTop;
};

clsDisplayPanel.prototype.width = function () {
    return this.element.offsetWidth;
};

clsDisplayPanel.prototype.height = function () {
    return this.element.offsetHeight;
};

clsDisplayPanel.prototype.hypotenuse = function () {
    return Math.sqrt(Math.pow(this.width(), 2) + Math.pow(this.height(), 2));
}

clsDisplayPanel.prototype.process = function () {
    
}

