/*******************************************************************
    tree library which includes both and object that can be instancesed 
    as well as a name space of functoins that can be used.

********************************************************************/

// report version
console.log("=== included tress.js ver 0.1 ===");

function clsTree() {
    

    // properties
    // amount of resources
    

}

// methods - what to do when a tree is clicked moused over etc....
clsTree.prototype.onClick = function() {

}

clsTree.prototype.onMouseOver = function () {


}

clsTree.prototype.process = function() {
    // trees day by day code when visible

}


// functions available without an instace via the items namespace
// access these using nsTree.function()
var tree = {
    import: function () {
        console.log("importing tree pack...");

        // load css
        var link = document.createElement('link');
        link.rel = 'stylesheet';
        link.type = 'text/css';
        link.href = "../packs/trees/trees.css";
        document.getElementsByTagName('head')[0].appendChild(link);
    },

    create: function () {
        var img = document.createElement('img');
        img.className = "trees oak";
        return img;
    },

    process: function () {

    }
};



