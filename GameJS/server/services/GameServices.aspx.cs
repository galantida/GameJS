using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;

namespace GameJS
{
    public partial class GameServices : System.Web.UI.Page
    {
        

        protected void Page_Load(object sender, EventArgs e)
        {
            /****** Session Support ******/
            //clsAccount myAccount = (clsAccount)Session["myAccount"]; // load a session if there is one
            //if (myAccount != null) sessionUserName = myAccount.eMail;
            //else sendResponse("Error", "Logon expired."); 


            /****** web services ******/
            string command = getStringParameter("cmd");
            if (command == null) command = "";

            // record request
            //clsEventLog.entry(EntryType.Request, command + " - (" + sessionUserName + "@" + Request.UserHostAddress + ")", Request.Url.ToString(), clsUtil.fileNameFromPath(Request.PhysicalPath));

            // process command first since that is the only parameter I can be sure of
            switch (command.ToUpper())
            {
                // return tile object - image, elevation
                case "GETTILE":
                    {
                        // read requested coordinate
                        int x = getNumericParameter("x");
                        int y = getNumericParameter("y");

                        clsWorld world = new clsWorld();
                        sendResponse("getTile", "{x:" + x + ",y:" + y + "}", world.map.tiles.getTile(x, y).toJSON());
                        break;
                    }
                case "GETTILES":
                    {
                        // read requested coordinate
                        int x1 = getNumericParameter("x1");
                        int y1 = getNumericParameter("y1");
                        int x2 = getNumericParameter("x2");
                        int y2 = getNumericParameter("y2");

                        clsWorld world = new clsWorld();
                        string JSON = clsTiles.toJSON(world.map.tiles.getTiles(x1, y1, x2, y2));
                        sendResponse("getTiles", "{x1:" + x1 + ",y1:" + y1 + ",x2:" + x2 + ",y2:" + y2 + "}", JSON);
                        break;
                    }
                case "GETTILEROW":
                    {
                        // return tileno and elevation given world coordinate

                        // read requested coordinate
                        int x1 = getNumericParameter("x1");
                        int x2 = getNumericParameter("x2");
                        int y = getNumericParameter("y");

                        // validate input data
                        if (x1 > x2) 
                        {
                            int tmp = x1;
                            x1 = x2;
                            x2 = tmp;
                        }

                        string result = "";
                        result += "{";
                        result += "\"tiles\":";
                        result += "[";

                        clsWorld world = new clsWorld();
                        List<clsTile> tiles = world.map.tiles.getTiles(y, x1, x2, y);

                        string comma = "";
                        foreach (clsTile tile in tiles)
                        {
                            result += comma + tile.toJSON();
                            comma = ",";
                        }

                        result += "]";
                        result += "}";

                        Response.Write(result);
                        break;
                    }
                case "GETTILECOLUMN":
                    {
                        // return tileno and elevation given world coordinate

                        // read requested coordinate
                        int x = getNumericParameter("x", true);
                        int y1 = getNumericParameter("y1", true);
                        int y2 = getNumericParameter("y2", true);

                        // validate input data
                        if (y1 > y2)
                        {
                            int tmp = y1;
                            y1 = y2;
                            y2 = tmp;
                        }

                        string result = "";
                        result += "{";
                        result += "\"tiles\":";
                        result += "[";

                        clsWorld world = new clsWorld();
                        List<clsTile> tiles = world.map.tiles.getTiles(y1, x, x, y2);

                        string comma = "";
                        foreach (clsTile tile in tiles)
                        {
                            result += comma + tile.toJSON();
                            comma = ",";
                        }

                        result += "]";
                        result += "}";

                        Response.Write(result);
                        break;
                    }
                default:
                    {
                        // handling invalid commands
                        if (command == "") sendResponse("Error", "Command is a required parameter.");
                        else sendResponse("Error", "'" + command + "' is not a valid command.");
                        break;
                    }
            }
            Response.End();
        }

        // standard place for all responses to be sent from
        private void sendResponse(string summary, string description, string obj = "null")
        {
            //clsEventLog.entry(EntryType.Response, summary + " (" + sessionUserName + " @ " + Request.UserHostAddress + ")", description, "session.aspx");
            Response.Write("{");
            Response.Write("\"summary\":\"" + summary + "\"");
            Response.Write(",\"details\":\"" + description + "\"");
            Response.Write(",\"compiled\":\"" + DateTime.Now + "\"");
            Response.Write(",\"content\":" + obj);
            Response.Write("}");
            Response.End();
        }

        private string getStringParameter(string name, bool required = false)
        {
            string value = Request.QueryString[name];
            if ((required == true) && (value == null)) sendResponse("Error", "'" + name + "' parameter is required.");
            return value;
        }

        private int getNumericParameter(string name, bool required = false)
        {
            // validate parameters
            int value;
            string svalue = Request.QueryString[name];
            if (!int.TryParse(svalue, out value))
            {
                if (required == true) sendResponse("Error", "' A numeric '" + name + "' parameter is required.");
            }
            return value;
        }
    }
}