/*******************************************************************
    Pack View - This is a portal view of the users pack
    the goal is to design it so that you can have more then one running
********************************************************************/

// report version
console.log("=== included clsContainerView.js ver 0.1 ===");

function clsContainerView(width, height) {
    console.log("Creating Container view...");

    // world view display panel
    this.displayPanel = new clsDisplayPanel("packPanel",width, height);

    // create back buffer div
    this.buffer = document.createElement('div');
    this.buffer.className = "clsContainer buffer";
    this.displayPanel.element.appendChild(this.buffer);

    this.requestTemplates();
}

clsContainerView.prototype.requestTemplates = function () {
    // request templates
    wsi.requestJSONInfo({ "callName": "getTemplates" } , receiveTemplates);
}

clsContainerView.prototype.receiveTemplates = function (response) {
    var templates = response.content;
    for (var t = 0; t < templates.length; t++) {
        var template = templates[t]; // get object
        this.buffer.appendChild(this.createTemplate(template));
    }
}

clsContainerView.prototype.createTemplate = function (template) {

    // create div 
    var div = document.createElement('div');
    div.className = "clsContainer divDefault";
    div.setAttribute("data", JSON.stringify(template));
    div.setAttribute("draggable", "true");

    // create image
    var img = document.createElement('img');
    img.src = "../images/world/" + template.image + ".png";
    img.className = "clsContainer imgDefault";
    div.appendChild(img);

    div.appendChild(document.createElement("BR"));
    div.appendChild(document.createTextNode(template.name));

    // add events
    //div.onmousedown = function () { client.packView.onClickTemplate(this); };
    div.ondragstart = function () { client.packView.onDragStartTemplate(this); };
    div.ondragover = function () { client.packView.onDragOverTemplate(this); };
    div.ondrop = function () { client.packView.onDropTemplate(this); };
    div.addEventListener("contextmenu", function (e) { e.preventDefault(); });
    
    return div;
}

clsContainerView.prototype.onClickTemplate = function (element) {

    // get the event
    if (!e) var e = window.event;
    e.stopPropagation();

    if (dragflag == false) { // allow for click event on mouse up only if nothing was being dragged

        // get infro about this click
        var template = JSON.parse(element.getAttribute("data"));

        // which click type was it
        var rightclick;
        if (e.which) rightclick = (e.which == 3);
        else if (e.button) rightclick = (e.button == 2);

        if (rightclick == true) {
            // right click

        }
        else {
            // left click

        }
    }
    e.preventDefault();
}

clsContainerView.prototype.onDragStartTemplate = function (element) {
    if (!e) var e = window.event; // get the event
    dragflag = true; // allow for click event on mouse up if nothing was dragged

    // get object and add an identifier
    var obj = JSON.parse(element.getAttribute("data"));
    obj.dragType = "template";
    e.dataTransfer.setData("text", JSON.stringify(obj)); // pass json string of object to drag
}


clsContainerView.prototype.onDragOverTemplate = function (element) {
    if (!e) var e = window.event;
    e.preventDefault();
}

clsContainerView.prototype.onDropTemplate = function (element) {

    // get event information
    if (!e) var e = window.event; // get event
    e.preventDefault();

    dragflag = false; // allow for click event on mouse up if nothing was dragged

    // get dragged information
    var srcObj = JSON.parse(e.dataTransfer.getData("text")); // source template

    // get drop location information
    var dstObj = JSON.parse(element.getAttribute("data")); // get object information

    console.log("dropped " + JSON.stringify(srcObj) + " on " + JSON.stringify(dstObj))

    switch (srcObj.dragType) {
        case "template":
            {
                // do noting for template on template
                //client.createObject(dstObj.x, dstObj.y, dstObj.z + 1, srcObj.id);
                break;
            }
        case "object":
            {
                // delete original object
                //console.log("deleting " + "obj" + srcObj.id);
                //var srcEle = document.getElementById("obj" + srcObj.id);
                //srcEle.parentNode.removeChild(srcEle);

                // delete object 
                client.deleteObject(srcObj.id);
                break;
            }
    }
}



