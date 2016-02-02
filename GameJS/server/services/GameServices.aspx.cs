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
                case "CREATEOBJECT":
                case "UPDATEOBJECT":
                    {
                        // connect to world db
                        clsWorld world;
                        clsObject obj;
                        if (callName.ToUpper() == "CREATEOBJECT") {

                            int x = getNumericParameter("x", true);
                            int y = getNumericParameter("y", true);
                            int z = getNumericParameter("z", true);

                            // create new object
                            world = new clsWorld(name, password);
                            obj = world.createObject(x, y, z);
                        }
                        else {
                            int id = getNumericParameter("id", true);

                            // locate an existing object
                            world = new clsWorld(name, password);
                            obj = world.getObject(id);
                            if (obj == null) sendResponse("Error", "Could not locate object 'id=" + id + "' to update.");
                        }

                        // populate properties
                        foreach (PropertyInfo propertyInfo in obj.GetType().GetProperties())
                        {
                            if (parameterExists(propertyInfo.Name))
                            {
                                switch (propertyInfo.PropertyType.ToString())
                                {
                                    case "System.Boolean":
                                        {
                                            // add unquoted bool type property to json string
                                            //if ((bool)propertyInfo.GetValue(this, null) == true) return "1";
                                            //else return "0";
                                            break;
                                        }
                                    case "System.Int16":
                                    case "System.Int32":
                                        {
                                            int v = getNumericParameter(propertyInfo.Name);
                                            propertyInfo.SetValue(obj, v);
                                            break;
                                        }
                                    case "System.DateTime":
                                        {
                                            break;
                                        }

                                    default:
                                        {
                                            string v = getStringParameter(propertyInfo.Name);
                                            propertyInfo.SetValue(obj, v);
                                            break;
                                        }
                                }
                            }
                        }
                        obj.save(); // this will save a new or update and exisitng tile

                        // formulate response
                        string JSON = obj.toJSON();
                        sendResponse(callName, "{id:" + obj.id + ",x:" + obj.x + ",y:" + obj.y + ",z:" + obj.z + "}", "[" + JSON + "]");
                        break;
                    }
                case "DELETEOBJECT":
                    {
                        int id = getNumericParameter("id", true);

                        // connect to world db
                        clsWorld world = new clsWorld(name, password);

                        // locate an existing object
                        clsObject obj = world.getObject(id);
                        if (obj == null) sendResponse("Error", "'Could not locate object 'id=" + id + "' to delete.");
                        obj.delete();

                        // return the objects that are still at that location
                        string JSON = clsObject.toJSON(world.getObjects(obj.x, obj.y, obj.x, obj.y), true);

                        sendResponse("deleteObject", "{id:" + id + "}", JSON);
                        break;
                    }
                case "GETOBJECTS":
                    {
                        // read requested coordinate
                        int x1 = getNumericParameter("x1", true);
                        int y1 = getNumericParameter("y1", true);
                        int x2 = getNumericParameter("x2");
                        int y2 = getNumericParameter("y2");
                        DateTime? modified = getDateTimeParameter("modified");

                        clsWorld world = new clsWorld(name, password);
                        string JSON = clsObject.toJSON(world.getObjects(x1, y1, x2, y2, modified), true);
                        sendResponse("getObjects", "{x1:" + x1 + ",y1:" + y1 + ",x2:" + x2 + ",y2:" + y2 + "}", JSON);
                        break;
                    }
                case "CREATEOBJECTS":
                    {
                        // read requested coordinate
                        int x1 = getNumericParameter("x1", true);
                        int y1 = getNumericParameter("y1", true);
                        int x2 = getNumericParameter("x2", true);
                        int y2 = getNumericParameter("y2", true);
                        int z = getNumericParameter("z");
                        string pack = getStringParameter("pack", true);
                        string item = getStringParameter("item", true);

                        // connect to world db
                        clsWorld world = new clsWorld(name, password);

                        clsObject obj;
                        List<clsObject> objects = new List<clsObject>();
                        string JSON = "[";
                        string delimiter = "";
                        Random random = new Random();
                        for (int y = y1; y <= y2; y++)
                        {
                            for (int x = x1; x <= x2; x++)
                            {
                                // new object
                                obj = new clsObject(world.db);
                                obj.x = x;
                                obj.y = y;
                                obj.z = random.Next(0, 2);
                                obj.pack = "stone";
                                switch (random.Next(0, 2))
                                {
                                    case 0:
                                        {
                                            obj.item = "bedrock1";
                                            break;
                                        }
                                    case 1:
                                        {
                                            obj.item = "bedrock2";
                                            break;
                                        }
                                }

                                obj.save();
                                objects.Add(obj);
                                JSON += delimiter + obj.toJSON(true);
                                delimiter = ",";

                                if (obj.z == 0)
                                {
                                    // put some warter on top
                                    obj = new clsObject(world.db);
                                    obj.x = x;
                                    obj.y = y;
                                    obj.z = 1;
                                    obj.pack = "stone";
                                    obj.item = "wetrock1";
                                    obj.save();
                                    objects.Add(obj);
                                    JSON += delimiter + obj.toJSON(true);
                                }

                                /*
                                // put some dirt on top
                                obj = new clsObject(world.db);
                                obj.x = x;
                                obj.y = y;
                                obj.z = 2;
                                obj.pack = "stone";
                                obj.item = "bedrockl";
                                obj.save();
                                objects.Add(obj);
                                JSON += delimiter + obj.toJSON(true);
                                 */
                            }
                        }

                        JSON += "]";

                        // formulate response
                        sendResponse("setCubes", "{x1:" + x1 + ",y1:" + y1 + ",x2:" + x2 + ",y2:" + y2 + "}", JSON);
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