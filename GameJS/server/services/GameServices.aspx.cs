using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Reflection;

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

            string name = "testdatabase";
            string password = "IBMs3666";


            /****** web services ******/
            string callName = getStringParameter("callName");
            if (callName == null) callName = "";

            // record request
            //clsEventLog.entry(EntryType.Request, command + " - (" + sessionUserName + "@" + Request.UserHostAddress + ")", Request.Url.ToString(), clsUtil.fileNameFromPath(Request.PhysicalPath));

            // process command first since that is the only parameter I can be sure of
            switch (callName.ToUpper())
            {
                case "SETTILE":
                    {
                        int x = getNumericParameter("x");
                        int y = getNumericParameter("y");
                        
                        // connect to world db
                        clsWorld world = new clsWorld(name, password);

                        // locate existing can't find tile then create new one
                        clsTile tile = world.getTile(x, y);
                        if (tile == null) tile = new clsTile(world.db);

                        // populate properties
                        int t;
                        foreach (PropertyInfo propertyInfo in tile.GetType().GetProperties())
                        {
                            if (parameterExists(propertyInfo.Name))
                            {
                                t = getNumericParameter(propertyInfo.Name);
                                propertyInfo.SetValue(tile, t);
                            }

                        }
                        tile.save(); // this will save a new or update and exisitng tile

                        // formulate response
                        string JSON = tile.toJSON();
                        sendResponse("setTile", "{x:" + tile.x + ",y:" + tile.y + "}","[" + JSON + "]");
                        break;
                    }
                case "GETTILES":
                    {
                        // read requested coordinate
                        int x1 = getNumericParameter("x1", true);
                        int y1 = getNumericParameter("y1", true);
                        int x2 = getNumericParameter("x2");
                        int y2 = getNumericParameter("y2");
                        DateTime? modified = getDateTimeParameter("modified");

                        clsWorld world = new clsWorld(name,password);
                        string JSON = clsTile.toJSON(world.getTiles(x1, y1, x2, y2, modified));
                        sendResponse("getTiles", "{x1:" + x1 + ",y1:" + y1 + ",x2:" + x2 + ",y2:" + y2 + "}", JSON);
                        break;
                    }
                case "SETTILES":
                    {
                        // read requested coordinate
                        int x1 = getNumericParameter("x1",true);
                        int y1 = getNumericParameter("y1", true);
                        int x2 = getNumericParameter("x2", true);
                        int y2 = getNumericParameter("y2", true);
                        int z = getNumericParameter("z");
                        int tilesetId = getNumericParameter("tilesetId", true);
                        int col = getNumericParameter("col", true);
                        int row = getNumericParameter("row", true);

                        // connect to world db
                        clsWorld world = new clsWorld(name, password);

                        clsTile tile;
                        List<clsTile> tiles = new List<clsTile>();
                        string JSON = "[";
                        string delimiter = "";
                        //Random random = new Random();
                        for (int y = y1; y <= y2; y++)
                        {
                            for (int x = x1; x <= x2; x++)
                            {
                                // locate existing can't find tile then create new one
                                tile = world.getTile(x, y);
                                if (tile == null) tile = new clsTile(world.db);
                                    
                                tile.x = x;
                                tile.y = y;
                                tile.z = x % 3;

                                tile.tilesetId = tilesetId;
                                tile.col = col;
                                tile.row = row;
                                
                                tile.save();
                                tiles.Add(tile);
                                JSON += delimiter + tile.toJSON();
                                delimiter = ",";
                            }
                        }

                        JSON += "]";

                        // formulate response
                        sendResponse("setTiles", "{x1:" + x1 + ",y1:" + y1 + ",x2:" + x2 + ",y2:" + y2 + "}", JSON);
                        break;
                    }
                default:
                    {
                        // handling invalid commands
                        if (callName == "") sendResponse("Error", "callName is a required parameter.");
                        else sendResponse("Error", "'" + callName + "' is not a valid call.");
                        break;
                    }
            }
            Response.End();
        }

        // standard place for all responses to be sent from
        private void sendResponse(string callName, string details, string obj = "null")
        {
            //clsEventLog.entry(EntryType.Response, summary + " (" + sessionUserName + " @ " + Request.UserHostAddress + ")", description, "session.aspx");
            Response.Write("{");
            Response.Write("\"callName\":\"" + callName + "\"");
            Response.Write(",\"details\":\"" + details + "\"");
            Response.Write(",\"compiled\":\"" + DateTime.Now + "\"");
            Response.Write(",\"content\":" + obj);
            Response.Write("}");
            Response.End();
        }

        private bool parameterExists(string name)
        {
            string value = Request.QueryString[name];
            if (value == null) return false;
            else return true;
        }

        private string getStringParameter(string name, bool required = false)
        {
            string value = Request.QueryString[name];
            if ((required == true) && (value == null)) sendResponse("Error", "'" + name + "' parameter is required.");
            return value;
        }

        private DateTime? getDateTimeParameter(string name, bool required = false)
        {
            DateTime? result = null;
            string value = Request.QueryString[name];
            if ((required == true) && (value == null)) sendResponse("Error", "'" + name + "' parameter is required.");
            if (value != null)
            {
                try
                {
                    result = Convert.ToDateTime(value);
                }
                catch (InvalidCastException e) 
                {
                    sendResponse("Error", "Invalid date format in '" + name + "' parameter.");
                }
            }
            return result;
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