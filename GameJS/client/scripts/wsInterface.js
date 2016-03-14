// *************************************************************************************************
// The purpose of this file is to facilitate properly formatted communications with the server.
// Goals one place for server names and paths
// simplify and standardize communications with web services
// *************************************************************************************************

// report version
console.log("=== included wsInterface.js ver 0.2 ===");

var wsi = { // wsi namespace

    debug: false,

    // decide which service to use base on the domain in the URL
    wsURL: function () {
        var url = "";

        switch (window.location.host) {
            case "localhost:7973":
                {
                    url = "http://localhost:7973/server/services/gameservices.aspx";
                    break;
                }
            case "www.codestudiocreation.com":
                {
                    url = "http://www.codestudiocreation.com/GameJS/server/services/gameservices.aspx";
                    break;
                }
            default:
                {
                    alert("Error: Unrecognised deployment location '" + window.location.host + "'");
                    break;
                }
        }
        return url;
    },
        
    // ***********************************************************************************************************************
    // requestJSONInfo - multithreaded handler for requesting information in the form of JSON objects
    // usage - Pass a one word command and a javascript object with need parameters as properties
    // 
    requestJSONInfo: function (paramObj, callBack) {

        // verify paramters are passed
        if (paramObj == null) paramObj = {};

        // add additional parameters
        paramObj.nocache = Math.random();

        // verify required parameter
        if (paramObj.callName == null) console.log("ERROR: Required parameter callName is 'null'.");
	
        // build parameter string
        var delimiter = "";
        var paramString = "?"
        for (var key in paramObj) { // loop through properties
            if (paramObj.hasOwnProperty(key)) {
                paramString += delimiter + key + "=" + paramObj[key];
                delimiter = "&";
            }
        }

        // log sending request attempt to server
        console.log("JSON Request: " + paramObj.callName + " - " + wsi.wsURL() + paramString); // display the request in the console
    
        // prepare service request
        var xhr = new XMLHttpRequest(); // communication object    
        xhr.withCredentials = true; 
        // you must set access control and allow credentials on web service server HTTP response headers as well.
        // Access-Control-Allow-Credentials = true
        // Access-Control-Allow-Headers = Content-Type
        // Access-Control-Allow-Methods = GET,POST,OPTIONS
        // Access-Control-Allow-Origin = http://tcgportal1:1212
	
        xhr.open("GET", wsi.wsURL() + paramString, true); // create a post in the com object
        xhr.timeout = 60 * 1000; // seconds	
        xhr.setRequestHeader("Content-type", "application/x-www-form-urlencoded; charset=UTF-8");   
        xhr.onreadystatechange = function () { wsi.receiveJSONInfo(paramObj.callName, xhr, callBack) };
        try {
            xhr.send(encodeURI(paramString)); // send
        } catch (err) {
            console.log("error encountered while sending request.");
        }
    },

    // ***********************************************************************************************************************
    // receiveJSONInfo - multi-threaded handler for JSON object responses received from the server
    // usage - this function is only called from "requestJSONInfo". just make sure to add a case statement to redirect
    // responses to their appropriate pages
    // 
    receiveJSONInfo: function(callName, xhr, callBack) {
        if (xhr.readyState == 4) {
            // report connection failure
            if (xhr.status != 200){
                console.log("ERROR: failed to execute '" + callName + "' on server '" + wsi.wsURL() + "' returned status '" + xhr.status + "'.");
            } 
            else {
                try {
                    var JSONResponse = JSON.parse(xhr.responseText);
                }
                catch (err) {
                    console.log("ERROR: JSON parse error '" + err.message + "'. responseText (" + xhr.responseText + ")");
                }

                // log the response from the server
                var JSONString = JSON.stringify(JSONResponse);
                if (JSONString.length > 100) JSONString = JSONString.substring(0, 100) + "...";
                console.log("JSON Response: " + callName + " - " + JSONString + "\n\n"); // display the response in the console
			
                // generic error handling
                /*
                if (JSONResponse.errorCode != null) {
                    switch (JSONResponse.errorCode)
                    {
                        case "-2146233086":
                            {
                                alert("Argument out of range. (" + JSONResponse.errorMessgae + ")");
                                break;
                            }
                        default:
                            {
                                alert("Unhandled error. (" + JSONResponse.errorMessgae + ")");
                                break;
                            }
                    }
				
                }
                */
	
                var fn;
                if (callBack != null) fn = callBack;
                else {
                    var fnName = productName + "_" + callName + "Reply";
                    var fn = window[fnName];
                }
            
                if (typeof fn === 'function') fn(JSONResponse);
                else alert("ERROR: a Function with the name  '" + fnName + "(xhr)' does not exists to handle the server response.\n\n");
            }
        } 
    }


};
