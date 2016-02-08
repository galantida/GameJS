/*******************************************************************
    Pack View - This is a portal view of the users pack
    the goal is to design it so that you can have more then one running
********************************************************************/

// report version
console.log("=== included clsContainerView.js ver 0.1 ===");

function clsContainerView(screenx, screeny, width, height) {
    console.log("Creating Container view...");

    // world view display panel
    this.displayPanel = new clsDisplayPanel("packPanel", screenx, screeny, width, height);
    this.currentTemplateId = 0;
    this.setup();
}

// refresh all tiles
clsContainerView.prototype.setup = function () {
    console.log("update pack view.");
    
    this.requestTemplates();

}

clsContainerView.prototype.requestTemplates = function () {
    // request templates
    wsi.requestJSONInfo({ "callName": "getTemplates" } , receiveTemplates);
}

clsContainerView.prototype.receiveTemplates = function (response) {
    var templates = response.content;

    for (var t = 0; t < templates.length; t++) {

        // get object
        var template = templates[t];

        this.displayPanel.element.appendChild(this.createTemplate(template));
    }
}

clsContainerView.prototype.createTemplate = function (template) {

    // create div 
    var div = document.createElement('div');
    div.className = "clsContainer divDefault";
    div.setAttribute("data", JSON.stringify(template));
    div.setAttribute("data", JSON.stringify(template));

    // create image
    var img = document.createElement('img');
    img.src = "../packs/stone/images/" + template.image + ".png";
    img.className = "clsContainer imgDefault";
    div.appendChild(img);

    div.appendChild(document.createElement("BR"));
    div.appendChild(document.createTextNode(template.name));

    // add events
    div.onmousedown = function () { client.packView.onClickTemplate(this); };
    div.addEventListener("contextmenu", function (e) { e.preventDefault(); });
    
    return div;
}

clsContainerView.prototype.onClickTemplate = function (element) {
    console.log("Clicked template.");

    var template = JSON.parse(element.getAttribute("data")); // get template information

    // which click type was it
    var rightclick;
    if (!e) var e = window.event;
    if (e.which) rightclick = (e.which == 3);
    else if (e.button) rightclick = (e.button == 2);


    if (rightclick == true) {
        // right click


    }
    else {
        // left click
        //set current template
        console.log("setting currentTemplateID = " + this.currentTemplateId);
        this.currentTemplateId = template.id;
    }

    //return false; // don't show default right click menu
    e.preventDefault();
}



