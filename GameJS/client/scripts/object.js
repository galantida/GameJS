/*******************************************************************
    Ground Object  - This object is used to manage the ground cubes
                        displayed in the worldview
********************************************************************/


// report version
console.log("=== included clsObject.js ver 0.1 ===");

var object = { // utils namespace

    draw: function (obj) {

        // this should be a generic function that creates objects based on a template in the database

        // create div 
        var div = document.createElement('div');
        div.className = "object divDefault";
        div.setAttribute("id", ("obj" + obj.id));
        div.setAttribute("data", JSON.stringify(obj));
        div.style.top = (-((obj.z + 1) * 32)) + "px";

        // add events
        //div.onmousedown = function () { object.onClick(this); };
        div.onmouseup = function () { object.onClick(this); };

        // add dragable events
        div.ondragstart = function () { object.onDragStart(this); };
        div.ondrag = function () { object.onDrag(this); };
        div.ondragend = function () { object.onDragEnd(this); };

        // add drag target events
        div.ondragenter = function () { object.onDragEnter(this); };
        div.ondragover = function () { object.onDragOver(this); };
        div.ondragleave = function () { object.onDragLeave(this); };
        div.ondrop = function () { object.onDrop(this); };
        
        
        div.addEventListener("contextmenu", function (e) { e.preventDefault(); });

        // not selectable
        //div.style = "-moz-user-select: none; -webkit-user-select: none; -ms-user-select:none; user-select:none;-o-user-select:none;"
        //div.setAttribute("unselectable", "on");
        //div.onselectstart = function () { return false; };

        // create image
        var img = document.createElement('img');
        img.src = "../images/world/" + obj.item + ".png";
        img.className = "object img64Default";
        div.appendChild(img); // put image in container

        return div;
    },

    onClick: function (element) {

        // identify click type (left right middle)
        var rightclick;
        if (!e) var e = window.event;
        e.stopPropagation();

        if (e.which) rightclick = (e.which == 3);
        else if (e.button) rightclick = (e.button == 2);

        if (dragflag == false) { // allow for click event on mouse up if nothing was dragged

            console.log("Clicked object.");

            // get info about the click
            var obj = JSON.parse(element.getAttribute("data")); // get object information
            var worldLocation = new clsVector2D(obj.x, obj.y);
            var screenLocation = client.worldView.worldToScreen(worldLocation); // get screen location clicked
            //var worldLocation = client.worldView.ground.screenToWorld(screenLocation); // get world location clicked


            if (rightclick == true) {
                // right click
                console.log("Right click stone @world(" + obj.x + "," + obj.y + ") and @screen(" + screenLocation.x + "," + screenLocation.y + ")");

                // create object based on current template
                //client.createObject(obj.x, obj.y, obj.z + 1, client.packView.currentTemplateId);

                // delete and redraw everything in tile            
                //client.deleteObject(obj.id);
            }
            else {
                // left click
                //console.log("Left click stone @world(" + obj.x + "," + obj.y + ") and @screen(" + screenLocation.x + "," + screenLocation.y + ")");

                // reload with new location as center
                client.playerMoveTarget = new clsVector2D(worldLocation.x - obj.z, worldLocation.y - obj.z);
            }

            //return false; // don't show default right click menu
        }
        e.preventDefault();
    },

    // drag events drag target
    onDragStart: function (element) {
        if (!e) var e = window.event; // get the event
        dragflag = true; // allow for click event on mouse up if nothing was dragged

        // get object and add an identifier
        var srcObj = JSON.parse(element.getAttribute("data"));
        srcObj.dragType = "object";
        e.dataTransfer.setData("text", JSON.stringify(srcObj)); // pass json string of object to drag

        console.log("dragging " + JSON.stringify(srcObj))
    },


    onDrag: function (element) {

    },

    onDragEnd: function (element) {

    },

    //drag events drop target
    onDragEnter: function (element) {
        element.classList.add('over');
    },

    onDragOver: function (element) {
        if (!e) var e = window.event;
        e.preventDefault();
    },

    onDragLeave: function (element) {
        element.classList.remove('over');
    },

    onDrop: function (element) {

        // get event information
        if (!e) var e = window.event; // get event
        e.preventDefault();
        e.stopPropagation();

        dragflag = false; // allow for click event on mouse up if nothing was dragged
        element.classList.remove('over');

        // get dragged information
        var srcObj = JSON.parse(e.dataTransfer.getData("text"));

        // get drop location information
        var dstObj = JSON.parse(element.getAttribute("data")); // get object information
        var worldLocation = new clsVector2D(dstObj.x, dstObj.y);
        var screenLocation = client.worldView.worldToScreen(worldLocation); // get screen location clicked

        console.log("dropped " + JSON.stringify(srcObj) + " on " +  JSON.stringify(dstObj))

        switch (srcObj.dragType) {
            case "template":
                {
                    // create object based on dropped template
                    client.createObject(dstObj.x, dstObj.y, dstObj.z + 1, srcObj.id);
                    break;
                }
            case "object":
                {
                    // delete original object
                    //console.log("deleting " + "obj" + srcObj.id);
                    var srcEle = document.getElementById("obj" + srcObj.id);
                    srcEle.parentNode.removeChild(srcEle);

                    // move object to new location
                    client.updateObject(srcObj.id, dstObj.x, dstObj.y, dstObj.z + 1, srcObj.pack, srcObj.item);
                    break;
                }
        }
    }

}




