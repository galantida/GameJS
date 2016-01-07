<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GamePlay.aspx.cs" Inherits="GameJS.GamePlay" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml" style="background:#000000;">
<head id="Head1" runat="server">
    <title>No Title</title>
    <link href="Content/Site.css" rel="stylesheet" />
</head>
<body onkeydown="keyDownHandler(event);" >
    <form id="frmScreen" >
    </form>
</body>
</html>
<script type="text/javascript">
    // ****************************
    // ***** World Settings *****
    // ****************************
    // player postion in world (will read from service)
    var posx = 200;
    var posy = 200;

    // world size (will read from service if even needed could wrap world on service side)
    var worldWidth = 320;
    var worldHeight = 320;
    var world;

    // ******************************
    // ***** screen settings *****
    // ******************************
    // screen width and height (Always even)
    var screenWidth = 16;
    var screenHeight = 24;

    // globals
    var cache = 3; // tiles to cache
    var screenPositionLeft, screenPositionTop;  // global variables
    var cacheWidth, cacheHeight, cacheWidthHalf, cacheHeightHalf; // global variables

    // constants
    var tileSize = 44;
    var tileSizeHalf = tileSize / 2;

    // quick access to html elements
    var tiles; // array of reference to screen tile divs
    var twoD; // reference to 2d overlay


    function start() {

        hudInit(10, 10);
        consoleInit(10, 600);
        screenInit(35, 35);

        // start game loop
        window.setInterval("heartbeat();", 100);

        //displayItem(3703, (cacheWidth / 2), (cacheHeight / 2), 1);     // barrel

        //displayGump("backpack", 800, 100);
        displayAnimation("walkdl", 500, 300);

        for (var x = 0; x < cacheWidth; x++) {
            requestScreenColumn(x);
        }
        shiftScreen(posx, posy);
    }

    function screenInit(x, y) {

        screenPositionLeft = x;
        screenPositionTop = y;

        // calc cache values based on screen size
        cacheHeight = screenHeight + (cache * 2); // add cache rows top and bottom
        cacheWidth = screenWidth + (cache * 2) + (cacheHeight / 2); // add cache rows left and right and account for ISO columns too
        cacheWidthHalf = cacheWidth / 2;
        cacheHeightHalf = cacheHeight / 2;

        // create div tags to represent screen tiles
        var result = "";
        for (var y = 0; y < cacheHeight; y++) {
            for (var x = 0; x < cacheWidth; x++) {
                result += "<div id=\"" + x + "-" + y + "\" class=\"screenTile\"></div>"
            }
        }
        frmScreen.innerHTML = result + "<div id=\"twoD\" class=\"screenTwoD\"></div>" + frmScreen.innerHTML;

        // links screen div tiles references with an array for faster access
        tiles = new Array(cacheWidth);
        for (var y = 0; y < cacheHeight; y++) {
            for (var x = 0; x < cacheWidth; x++) {
                if (y == 0) tiles[x] = new Array();
                tiles[x][y] = document.getElementById(x + "-" + y);
                tiles[x][y].innerHTML = x + "-" + y; // debug code to display tile id
            }
        }
        twoD = document.getElementById("twoD");
    }

    function hudInit(x, y) {
        var result = "<div id=\"hud\" style=\"position:absolute;top:" + y + "px;left:" + x + "px;\">";
        result += "World Coordinates = <span id=\"wc\"></span><br />";
        result += "Screen Coordinates = <span id=\"sc\"></span><br />";
        result += "Cache Coordinates = <span id=\"cc\"></span><br />";
        result += "</div>";
        frmScreen.innerHTML += result;
    }

    function consoleInit(x, y) {
        var result = "<div id=\"console\" style=\"position:absolute;top:" + y + "px;left:" + x + "px;max-height:100px;overflow-y: scroll;\"></div>";
        frmScreen.innerHTML += result;
    }

    function consoleWrite(text) {
        var objDiv = document.getElementById("console");
        objDiv.innerHTML += text + "<br>";
        objDiv.scrollTop = objDiv.scrollHeight;
    }


    // display the screen shifted 
    function shiftScreen(xShift, yShift) {

        // loop through each and repostion screen tiles based on shift
        for (var y = 0; y < cacheHeight; y++) {
            for (var x = 0; x < cacheWidth; x++) {

                // shift display horizontally with wrap
                var rx = x + xShift;
                while (rx >= cacheWidth) { rx -= cacheWidth; } // wrap negative values
                while (rx < 0) { rx += cacheWidth; } // wrap to great of values

                // shift display vertically with wrap
                var ry = y + yShift;
                while (ry >= cacheHeight) { ry -= cacheHeight; } // wrap negative values
                while (ry < 0) { ry += cacheHeight; } // wrap to great of values

                // display only tiles in screen area
                var result = true;
                var leftSide = ((cacheHeight - y) / 2) + cache;
                //if (x < leftSide) result = false;
                //if (x > (leftSide + screenWidth)) result = false;
                //if ((y < cache) || (y > (cacheHeight - (cache * 2)))) result = false;

                // move div
                if (result == true) {
                    // calculate new screen position for div
                    var px = screenPositionLeft + (x * tileSize) + (y * tileSizeHalf) - (cacheWidthHalf * tileSize);
                    var py = screenPositionTop + (y * tileSizeHalf) - (cache * tileSizeHalf);

                    tiles[rx][ry].style.visibility = "visible";
                    tiles[rx][ry].style.left = px.toString() + "px";
                    tiles[rx][ry].style.top = py.toString() + "px";
                }
                else {
                    tiles[rx][ry].style.left = "0px";
                    tiles[rx][ry].style.top = "0px";
                    tiles[rx][ry].style.visibility = "hidden";
                }
            }
        }
    }

    // recevie screen column return world coordinate
    function screenToWorldx(sx) {
        // we know posx = (cacheWidth / 2) on screen
        var wx = posx + sx;
        while (wx < 0) { wx += worldWidth; }
        while (wx > worldWidth) { wx -= worldWidth; }
        return wx;
    }

    // recevie screen row return world coordinate
    function screenToWorldy(sy) {
        // we know posy = (cacheHeight / 2) on screen
        var wy = posy + sy;
        while (wy < 0) { wy += worldHeight; }
        while (wy > worldHeight) { wy -= worldHeight; }
        return wy;
    }

    function worldToScreenx(wx) {
        // we know (cacheWidth / 2) = posx in world
        var sx = wx - posx;
        while (sx < 0) { sx += cacheWidth; }
        while (sx >= cacheWidth) { sx -= cacheWidth; }
        return sx;
    }

    function worldToScreeny(wy) {
        // we know (cacheWidth / 2) = posx in world
        var sy = wy - posy;
        while (sy < 0) { sy += cacheHeight; }
        while (sy >= cacheHeight) { sy -= cacheHeight; }
        return sy;
    }

    function screenToCachex(sx) {
        // cache has been shifted based on posx
        var result = sx + (posx % cacheWidth); // screenx + minor shift balance
        if (result >= cacheWidth) result -= cacheWidth;
        return result;
    }

    function screenToCachey(sy) {
        // cache has been shifted based on posx
        var result = sy + (posy % cacheHeight); // screeny + minor shift balance
        if (result >= cacheHeight) result -= cacheHeight;
        return result;
    }

    function cacheToScreenx(cx) {
        // cache has been shifted based on posx
        var shiftBalance = (posx % cacheWidth);
        var result = cx - shiftBalance;
        if (result < 0) result += screenWidth;
        return result;
    }

    function cacheToScreeny(cy) {
        // cache has been shifted based on posx
        var shiftBalance = (posy % cacheHeight);
        var result = cy - shiftBalance;
        if (result < 0) result += screenHeight;
        return result;
    }

    function worldToCachex(wx) {
        return screenToCachex(worldToScreenx(wx));
    }

    function worldToCachey(wy) {
        return screenToCachey(worldToScreeny(wy));
    }


    // request tilestrip world y from x1 to x2
    function requestScreenRow(row) {
        // build parameter string 
        var p = "?nocache=" + Math.random();
        p += "&cmd=getTileRow";
        p += "&y=" + screenToWorldy(row);
        p += "&x1=" + screenToWorldx(0);
        p += "&x2=" + screenToWorldx(cacheWidth - 1);

        // build service url
        var requestURL = "GameServices.aspx" + p;

        //consoleWrite("Requesting information... \"" + requestURL + "\"");

        // prepare service request
        var xhr = new XMLHttpRequest(); // communication object
        xhr.open("GET", requestURL, true); // prepare request
        xhr.onreadystatechange = function () { receiveScreenTiles(xhr) };

        xhr.send(); // send
    }

    // request tilestrip world y from x1 to x2
    function requestScreenColumn(column) {

        // build parameter string 
        var p = "?nocache=" + Math.random();
        p += "&cmd=getTileColumn";
        p += "&x=" + screenToWorldx(column);
        p += "&y1=" + screenToWorldy(0);
        p += "&y2=" + screenToWorldy(cacheHeight - 1);

        // build service url
        var requestURL = "GameServices.aspx" + p;

        //consoleWrite("Requesting information... \"" + requestURL + "\"");

        // prepare service request
        var xhr = new XMLHttpRequest(); // communication object
        xhr.open("GET", requestURL, true); // prepare request
        xhr.onreadystatechange = function () { receiveScreenTiles(xhr) };

        xhr.send(); // send
        //alert("Request sent.");  // for debug only
    }

    function receiveScreenTiles(xhr) {
        if (xhr.readyState == 4) {
            if (xhr.status != 200) alert("ERROR: failed to connect to server returned status (" + xhr.status + ").");
            else {
                //consoleWrite("Received response.(" + xhr.responseText + ")");
                console.log("Received response.(" + xhr.responseText + ")");

                try {
                    var JSONResponse = JSON.parse(xhr.responseText);
                }
                catch (err) {
                    var txt = "There was an error on this page.\n\n";
                    txt += "Error description: " + err.message + "\n\n";
                    txt += "Error description: " + xhr.responseText + "\n\n";
                    txt += "Click OK to continue.\n\n";
                    alert(txt);
                }

                // save info to screen
                var tiles = JSONResponse.tiles;
                for (var t = 0; t < tiles.length; t++) {
                    displayTile(tiles[t]);
                }
            }
        }
    }

    function displayTile(tile) {

        // update our view of the world
        //world[tile.x][tile.y].tile = tile;
        
        // get cache coordinates
        var cx = worldToCachex(tile.x);
        var cy = worldToCachey(tile.y);

        // stretched image
        var h = 44;
        var z = tileElevation(tile.x, tile.y - 1);
        if (z != null) h += z - tile.z;

        var w = 44;
        var zz = tileElevation(tile.x - 1, tile.y);
        if (zz != null) w += zz - tile.z;
        
        // stretched
        //tiles[cx][cy].innerHTML = "<img id=\"tile" + tile.x + "-" + tile.y + "\" src=\"/images/tiles/" + tile.no + ".gif\" style=\"position:absolute;bottom:" + tile.z + "px;left:0px;width:" + w + "px;height:" + h + "px;\">";


        // standard image
        tiles[cx][cy].innerHTML = "<img src=\"/images/tiles/" + tile.img + ".gif\" style=\"position:absolute;bottom:" + tile.z + "px;left:0px;\">";
        

        // background image
        //tiles[cx][cy].style.backgroundImage = "url(/images/tiles/" + tile.no + ".gif)";

        //tiles[cx][cy].innerHTML = "<img src=\"/images/tiles/" + tile.no + ".gif\" style=\"position:absolute;bottom:0px;left:0px;\">";
        //tiles[cx][cy].innerHTML = tile.No;
        //tiles[cx][cy].innerHTML += tile.z;
        //tiles[cx][cy].innerHTML = screenToWorldx(worldToScreenx(tile.x)) + "<br>" + screenToWorldy(worldToScreeny(tile.y));
    }

    function tileElevation(x, y) {
        var tile = document.getElementById("tile" + x + "-" + y);
        if (tile != null) {
            var sz = tile.style.bottom;
            return sz.substring(0, sz.length - 2);
        }
        else return null;
    }

    function displayItem(itemNo, x, y, z) {
        tiles[x][y].innerHTML += "<img src=\"/images/items/" + itemNo + ".gif\" style=\"position:absolute;bottom:" + z + "px;left:0px;\">";
    }

    function displayGump(gumpNo, x, y) {
        twoD.innerHTML += "<img src=\"/images/gumps/" + gumpNo + ".gif\" style=\"position:absolute;left:" + x + "px;top:" + y + "px;\">";
    }

    function displayAnimation(animationNo, x, y) {
        twoD.innerHTML += "<img src=\"/images/animations/" + animationNo + ".gif\" style=\"position:absolute;left:" + x + "px;top:" + y + "px;\">";
    }

    function heartbeat() {
        // game loop
        

        // bug display
        document.getElementById("wc").innerHTML = "(" + posx + "-" + posy + ")";
        document.getElementById("sc").innerHTML = "(" + screenWidth + "-" + screenHeight + ") (" + worldToScreenx(posx) + "-" + worldToScreeny(posy) + ")";
        document.getElementById("cc").innerHTML = "(" + cacheWidth + "-" + cacheHeight + ") (" + screenToCachex(worldToScreenx(posx)) + "-" + screenToCachey(worldToScreeny(posy)) + ")";
    }

    function keyDownHandler(event) {

        //alert("event.keyCode" + event.keyCode);
        //alert("event.charCode" + event.charCode);

        switch (event.keyCode) {
            case 36:
                {
                    // up left
                    posy -= 1;
                    posx -= 1;
                    shiftScreen(posx, posy);
                    requestScreenRow(0);
                    break;
                }
            case 38:
                {
                    // up
                    posy -= 1;
                    shiftScreen(posx, posy);
                    requestScreenRow(0);
                    break;
                }
            case 33:
                {
                    // up right
                    posy -= 1;
                    posx += 1;
                    shiftScreen(posx, posy);
                    requestScreenRow(0);
                    break;
                }
            case 37:
                {
                    // left
                    posx -= 1;
                    shiftScreen(posx, posy);
                    requestScreenColumn(0);
                    break;
                }
            case 39:
                {
                    // right
                    posx += 1;
                    shiftScreen(posx, posy);
                    requestScreenColumn(cacheWidth - 1);
                    break;
                }
            case 35:
                {
                    // down left
                    posy += 1;
                    posx -= 1;
                    shiftScreen(posx, posy);
                    requestScreenRow(cacheHeight - 1);
                    break;
                }
            case 40:
                {
                    // down
                    posy += 1;
                    shiftScreen(posx, posy);
                    requestScreenRow(cacheHeight - 1);
                    break;
                }
            case 34:
                {
                    // down right
                    posy += 1;
                    posx += 1;
                    shiftScreen(posx, posy);
                    requestScreenRow(cacheHeight - 1);
                    break;
                }
            default:
                {
                    //alert("event.keyCode" + event.keyCode);
                }
        }
    }

    start();
</script>
