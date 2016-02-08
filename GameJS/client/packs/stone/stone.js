/*******************************************************************
    cubes library which includes both and object that can be instancesed 
    as well as a name space of functoins that can be used.

********************************************************************/

// report version
console.log("=== included stone.js ver 0.1 ===");

// functions available without an instace via the items namespace
// access these using nsTree.function()
var stone = {
    import: function () {
        console.log("importing stone pack...");


    },

    create_old: function (obj) {
        // this should be a generic function that creates objects base don a template in the database

        // create image
        var img = document.createElement('img');
        img.src = "../packs/stone/images/" + obj.item + ".png";
        img.className = "object imgDefault";
        
        // create div 
        var div = document.createElement('div');
        div.className = "object divDefault";
        div.setAttribute("data", JSON.stringify(obj));
        div.style.top = (-(obj.z * 32)) + "px";

        // add events
        div.onmousedown = function () { client.worldView.ground.onClickObject(this); };
        div.addEventListener("contextmenu", function (e) { e.preventDefault(); });

        // put image in container
        div.appendChild(img);
        return div;
    },

    process: function() {

    }
};



