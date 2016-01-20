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
                // return tile object - image, elevation
                case "GETTILE":
                    {
                        // read requested coordinate
                        int x = getNumericParameter("x");
                        int y = getNumericParameter("y");

                        
                        clsWorld world = new clsWorld(name, password);
                        world.start();
                        clsTile tile = world.getTile(x, y);
                        string JSON = "null";
                        if (tile != null) JSON = tile.toJSON();
                        world.stop();
                        sendResponse("getTile", "{x:" + x + ",y:" + y + "}", JSON);
                        break;
                    }
                case "SETTILE":
                    {
                        // connect to world db
                        clsWorld world = new clsWorld(name, password);
                        world.start();
                        clsTile tile = new clsTile(world.db);

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

                        tile.save();
                        world.stop(); // close world db

                        // formulate response
                        string JSON = tile.toJSON();
                        sendResponse("setTile", "{x:" + tile.x + ",y:" + tile.y + "}", JSON);
                        break;
                    }
                case "GETTILES":
                    {
                        // read requested coordinate
                        int x1 = getNumericParameter("x1");
                        int y1 = getNumericParameter("y1");
                        int x2 = getNumericParameter("x2");
                        int y2 = getNumericParameter("y2");

                        clsWorld world = new clsWorld(name,password);
                        world.start();
                        string JSON = clsTile.toJSON(world.getTiles(x1, y1, x2, y2));
                        world.stop();
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
                        
                        clsWorld world = new clsWorld(name, password);
                        world.start();
                        string JSON = clsTile.toJSON(world.getRow(x1, x2, y));
                        world.stop();
                        sendResponse("getRow", "{x1:" + x1 + ",x2:" + x2 + ",y:" + y + "}", JSON);
                        break;
                    }
                case "GETTILECOL":
                    {
                        // return tileno and elevation given world coordinate

                        // read requested coordinate
                        int x = getNumericParameter("x", true);
                        int y1 = getNumericParameter("y1", true);
                        int y2 = getNumericParameter("y2", true);

                        clsWorld world = new clsWorld(name, password);
                        world.start();
                        string JSON = clsTile.toJSON(world.getCol(x, y1, y2));
                        world.stop();
                        sendResponse("getCol", "{x:" + x + ",y1:" + y1 + ",y2:" + y2 + "}", JSON); break;
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