/*******************************************************************
    cubes library which includes both and object that can be instancesed 
    as well as a name space of functoins that can be used.

********************************************************************/

// report version
console.log("=== included stone.js ver 0.1 ===");

function clsStone() {
    

    // properties
    // amount of resources
    

}

// methods - what to do when a tree is clicked moused over etc....
clsStone.prototype.onClick = function() {

}

clsStone.prototype.onMouseOver = function () {


}

clsStone.prototype.process = function() {
    // trees day by day code when visible

}


// functions available without an instace via the items namespace
// access these using nsTree.function()
var stone = {
    import: function () {
        console.log("importing stone pack...");

        // load css
        var link = document.createElement('link');
        link.rel = 'stylesheet';
        link.type = 'text/css';
        link.href = "../packs/stone/stone.css";
        document.getElementsByTagName('head')[0].appendChild(link);
    },

    create: function (obj) {

        // create image
        var img = document.createElement('img');
        img.src = "../packs/stone/images/" + obj.item + ".png";
        img.className = "stone imgDefault";
        
        // create container
        var div = document.createElement('div');
        div.className = "stone divDefault";
        div.setAttribute("data", JSON.stringify(obj));
        div.style.top = (-(obj.z * 32)) + "px";

        // add events
        div.onmousedown = function () { stone.onClick(this); };
        div.addEventListener("contextmenu", function (e) { e.preventDefault(); });

        // put image in container
        div.appendChild(img);
        return div;
    },

    onClick: function (element) {
        console.log("stone click");

        var obj = JSON.parse(element.getAttribute("data"));

        // get info about the click
        var screenLocation = new clsVector2D(Number(obj.x), Number(obj.y)); // get screen location clicked
        var worldLocation = client.worldView.ground.screenToWorld(screenLocation); // get world location clicked

        // which click type was it
        var rightclick;
        if (!e) var e = window.event;
        if (e.which) rightclick = (e.which == 3);
        else if (e.button) rightclick = (e.button == 2);


        if (rightclick == true) {
            // right click
            console.log("right clicked a stone");
            //var br = Math.floor(Math.random() * 2);
            //client.setObject(worldLocation.x, worldLocation.y, "cubes", "bedrock" + br);
        }
        else {
            // left click
            console.log("left clicked a stone");
        }

        //return false; // don't show default right click menu
        e.preventDefault();
    },

    process: function () {

    }
};



