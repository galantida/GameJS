/*******************************************************************
    Ground Object  - This object is used to manage the ground cubes
                        displayed in the worldview
********************************************************************/


// report version
console.log("=== included clsObject.js ver 0.1 ===");

var object = { // object namespace

    draw: function (obj) {

        // create div 
        var div = document.createElement('div');
        div.className = "object divDefault";
        div.setAttribute("id", ("obj" + obj.id));

        // set properties
        obj.elementLastUpdated = new Date(); // add an elementCreated date to the object
        div.setAttribute("data", JSON.stringify(obj));
        //div.style.top = (-((obj.z + 1) * 32)) + "px";

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
        img.onload = function () { object.position(this); }; // add onload event to position element after image loaded and size available
        img.src = "../images/world/" + obj.image + ".png";
        img.className = "object imgDefault";

        div.appendChild(img); // put image in container

        return div;
    },

    // this will position the image after it is loaded
    position: function (img) {
        var div = img.parentElement;
        var obj = JSON.parse(div.getAttribute("data"));
        obj.imgWidth = img.clientWidth;
        obj.imgHeight = img.clientHeight;
        div.setAttribute("data", JSON.stringify(obj));
        div.style.left = (32 - obj.imgWidth /2) + "px";
        div.style.top = (-(obj.z + 32) -(obj.imgHeight -64)) + "px"; // this works perfectly
    },

    // this is used to smooth the move over time
    moving: function (element, xOffset, yOffset) {
        var speed = 5;
        var movesPerSecond = 10;

        console.log("moving " + element.id + " (" + xOffset + "," + yOffset + ")");


        // slowly move the image to the desired location
        xOffset -= (xOffset / Math.abs(xOffset) * speed); // this will never end has to be fixed
        yOffset -= (yOffset / Math.abs(yOffset) * speed); // this will never end has to be fixed

        var stillMoving = false;
        if (Math.abs(xOffset) > speed) {
            element.style.left = xOffset + "px";
            stillMoving = true;
        }
        else element.style.left = "0px";

        if (Math.abs(yOffset) > speed) {
            element.style.top = yOffset + "px";
            stillMoving = true;
        }
        else element.style.top = "0px";
        
        // setup next move execution
        if (stillMoving == true) setTimeout(function () { object.moving(element, xOffset, yOffset); }, (1000 / movesPerSecond));

    },

    onClick: function (element) {

        // get the event
        if (!e) var e = window.event;
        e.stopPropagation();

        if (dragflag == false) { // allow for click event on mouse up only if nothing was being dragged

            // get info about the click
            var obj = JSON.parse(element.getAttribute("data")); // get object information
            var worldLocation = new clsVector2D(obj.x, obj.y);
            var screenLocation = client.worldView.worldToScreen(worldLocation); // get screen location clicked

            // which click type was it
            var rightclick;
            if (e.which) rightclick = (e.which == 3);
            else if (e.button) rightclick = (e.button == 2);

            if (rightclick == true) {
                console.log("Right clicked object " + JSON.stringfy(obj) + " at screen(" + screenLocation.x + "," + screenLocation.y + ")");

            }
            else {
                console.log("Left clicked object " + JSON.stringify(obj) + " at screen(" + screenLocation.x + "," + screenLocation.y + ")");

                // move play to new location. screen will follow
                client.playerMoveTarget = new clsVector2D(worldLocation.x - (obj.z / 32), worldLocation.y - (obj.z / 32));
            }
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

        console.log("Drag start " + JSON.stringify(srcObj));
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
        e.preventDefault(); // is this really needed?
        e.stopPropagation();

        dragflag = false; // allow for click event on mouse up if nothing was dragged
        element.classList.remove('over'); // remove the selected class

        // get dragged information
        var srcObj = JSON.parse(e.dataTransfer.getData("text"));

        // get drop location information
        var dstObj = JSON.parse(element.getAttribute("data")); // get object information
        var worldLocation = new clsVector2D(dstObj.x, dstObj.y);
        var screenLocation = client.worldView.worldToScreen(worldLocation); // get screen location clicked
        console.groupCollapsed("dropped " + JSON.stringify(srcObj) + " on " + JSON.stringify(dstObj));

        switch (srcObj.dragType) {
            case "template":
                // create object based on dropped template
                console.log("creating new object based on template " + JSON.stringify(srcObj));
                client.createObject(dstObj.x, dstObj.y, dstObj.z + 32, srcObj.id);
                break;

            case "object":
                // delete original object
                console.log("Deleting element 'obj" + srcObj.id + "' for object " + JSON.stringify(srcObj));
                var srcEle = document.getElementById("obj" + srcObj.id);
                srcEle.parentNode.removeChild(srcEle);

                // create new object in new location
                console.log("Sending update request for object " + JSON.stringify(srcObj));
                client.updateObject(srcObj.id, dstObj.x, dstObj.y, dstObj.z + 32, srcObj.pack, srcObj.item);
                break;
        }
        console.groupEnd();
    }

}




