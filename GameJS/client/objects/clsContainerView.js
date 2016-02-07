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

    this.setup();
}

// refresh all tiles
clsContainerView.prototype.setup = function () {
    console.log("update pack view.");

    for (var y = 0; y <= 4; y++) {
        for (var x = 0; x <= 4; x++) {

            // create cube div element
            var ele = document.createElement('div');
            ele.className = "clsContainer item";
            ele.style.backgroundPosition = "-" + x + "em -" + y + "em";

            // set dataset properties
            ele.dataset.x = x;
            ele.dataset.y = y;

            // set element properties
            //ele.style.left = ele.dataset.x * 64 + "px";
            //ele.style.top = ele.dataset.y * 64 + "px";

            // set element events
            //ele.onmousedown = function () { cubeOnClick(this.id); };
            //ele.addEventListener("contextmenu", function (e) { e.preventDefault(); });
            //ele.onmouseover = function () { console.log("(" + this.id + ")"); };
            this.displayPanel.element.appendChild(ele); // add the cube element to the buffer

        }
    }
}

