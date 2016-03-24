using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Configuration;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using GameWorld;

namespace GameJS
{
    public partial class GameServices : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //sendResponse("Error", "I got this far");
            //return;

            /****** web services ******/
            string callName = getStringParameter("callName");
            if (callName == null) callName = "";

            string dbConString = WebConfigurationManager.AppSettings["dbConString"];

            // process command first since that is the only parameter I can be sure of
            switch (callName.ToLower())
            {
                case "createobject":
                    {
                        // required properties
                        int x = getNumericParameter("x", true);
                        int y = getNumericParameter("y", true);
                        int z = getNumericParameter("z", true);
                        int templateId = getNumericParameter("templateId", true);

                        // connect to world
                        clsWorld world = new clsWorld(dbConString);
                        world.open();

                        // load template
                        clsTemplate template = world.getTemplate(templateId);
                        if (template == null) sendResponse("Error", "Could not locate template 'templateId=" + templateId + "' to create new object.");

                        // create object based on template
                        clsObject obj = world.map.createObject(x, y, z, template);
                        obj.save();
                        world.close();

                        // formulate response
                        sendResponse("createObject", "{id:" + obj.id + ",x:" + obj.x + ",y:" + obj.y + ",z:" + obj.z + "}", "[" + obj.toJSON() + "]");
                        break;
                    }
                case "updateobject":
                    {
                        int id = getNumericParameter("id", true);

                        // locate an existing object
                        clsWorld world = new clsWorld(dbConString);
                        world.open();
                        clsObject obj = new clsObject(world.db, id);
                        if (obj == null) sendResponse("Error", "Could not locate object 'id=" + id + "' to update.");


                        // populate properties based on passed paramters
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
                                            propertyInfo.SetValue(obj, v, null);
                                            break;
                                        }
                                    case "System.DateTime":
                                        {
                                            break;
                                        }

                                    default:
                                        {
                                            string v = getStringParameter(propertyInfo.Name);
                                            propertyInfo.SetValue(obj, v, null);
                                            break;
                                        }
                                }
                            }
                        }
                        obj.save();
                        world.close();

                        // formulate response
                        sendResponse("updateObject", "{id:" + obj.id + ",x:" + obj.x + ",y:" + obj.y + ",z:" + obj.z + "}", "[" + obj.toJSON() + "]");
                        break;
                    }
                case "deleteobject":
                    {
                        int id = getNumericParameter("id", true);

                        clsWorld world = new clsWorld(dbConString);
                        world.open();
                        clsObject obj = new clsObject(world.db, id);
                        obj.delete();
                        world.close();
                        if (obj == null) sendResponse("Error", "'Could not locate object 'id=" + id + "' to delete.");
                        else sendResponse("deleteObject", "{id:" + obj.id + "}", "[" + obj.toJSON() + "]");
                        break;
                    }
                case "getarea":
                    {
                        // read requested coordinate
                        int x1 = getNumericParameter("x1", true);
                        int y1 = getNumericParameter("y1", true);
                        int x2 = getNumericParameter("x2");
                        int y2 = getNumericParameter("y2");
                        DateTime? modified = getDateTimeParameter("modified");

                        clsWorld world = new clsWorld(dbConString);
                        world.open();
                        string JSON = clsObject.toJSON(world.map.getArea(x1, y1, x2, y2, 0, modified));
                        world.close();
                        sendResponse("getArea", "{x1:" + x1 + ",y1:" + y1 + ",x2:" + x2 + ",y2:" + y2 + "}", JSON);
                        break;
                    }
                case "createarea":
                    {
                        // read requested coordinate
                        int x1 = getNumericParameter("x1", true);
                        int y1 = getNumericParameter("y1", true);
                        int x2 = getNumericParameter("x2", true);
                        int y2 = getNumericParameter("y2", true);

                        // connect to world db
                        clsWorld world = new clsWorld(dbConString);
                        world.open();
                        //List<clsObject> result = world.map.createArea(x1,y1,x2,y2);
                        string JSON = world.map.createArea(x1,y1,x2 - x1);
                        world.close();

                        // formulate response
                        sendResponse("createArea", "{x1:" + x1 + ",y1:" + y1 + ",x2:" + x2 + ",y2:" + y2 + "}", JSON);
                        break;
                    }
                case "gettemplates":
                    {
                        clsWorld world = new clsWorld(dbConString);
                        world.open();
                        string JSON = clsTemplate.toJSON(world.getAllTemplates());
                        world.close();
                        sendResponse("getTemplates", "{}", JSON);
                        break;
                    }
                case "savetemplates":
                    {
                        // load the world templates from the DB
                        clsWorld world = new clsWorld(dbConString);
                        world.open();
                        List<clsTemplate> templates = world.getAllTemplates();

                        // convert to formatted JSON
                        string JSONString = clsTemplate.toJSON(templates, true, true);
                        string JSONFormatted = JValue.Parse(JSONString).ToString(Formatting.Indented);

                        // save to file
                        string path = Server.MapPath("..") + "\\files\\templates.json";
                        File.WriteAllText(@path, JSONFormatted);
                        world.close();

                        // respond
                        sendResponse("saveTemplates", "{}", JSONString);
                        break;
                    }
                case "loadtemplates":
                    {
                        // destroy existing templates & their attributes from the database
                        clsWorld world = new clsWorld(dbConString);
                        world.open();
                        int recordsDeleted = world.destroyAll();

                        // load the world templates from the file
                        string path = Server.MapPath("..") + "\\files\\templates.json";
                        string JSONString = File.ReadAllText(@path);
                        JArray JSONArray = (JArray)JsonConvert.DeserializeObject(JSONString);

                        // convert to template objects
                        clsTemplate template = new clsTemplate(world.db);
                        List<clsTemplate> templates = template.fromJSON(JSONArray);
                        int recordsCreated = template.save(templates.Cast<intBase>());
                        world.close();

                        // respond
                        string results = clsTemplate.toJSON(templates, true);
                        sendResponse("loadTemplates", "{\"recordsCreated\":" + recordsCreated + "}", results);
                        break;
                    }
                case "saveobjects":
                    {
                        // load the world templates from the DB
                        clsWorld world = new clsWorld(dbConString);
                        world.open();
                        List<clsObject> objects = world.map.getAllObjects();

                        // convert to formatted JSON
                        string JSONString = clsObject.toJSON(objects, true, true);
                        string JSONFormatted = JValue.Parse(JSONString).ToString(Formatting.Indented);

                        // save to file
                        string path = Server.MapPath("..") + "\\files\\objects.json";
                        File.WriteAllText(@path, JSONFormatted);
                        world.close();

                        // respond
                        sendResponse("saveObjects", "{}", JSONString);
                        break;
                    }
                case "loadobjects":
                    {
                        // destroy existing templates & their attributes from the database
                        clsWorld world = new clsWorld(dbConString);
                        world.open();
                        world.map.destroyAll();

                        // load the world templates from the file
                        string path = Server.MapPath("..") + "\\files\\objects.json";
                        string JSONString = File.ReadAllText(@path);
                        JArray JSONArray = (JArray)JsonConvert.DeserializeObject(JSONString);

                        // convert to template objects
                        clsObject obj = new clsObject(world.db);
                        List<clsObject> objects = obj.fromJSON(JSONArray);
                        int recordsCreated = obj.save(objects.Cast<intBase>());
                        world.close();

                        // respond
                        string results = clsObject.toJSON(objects, true);
                        sendResponse("loadObjects", "{\"recordsCreated\":" + recordsCreated + "}", results);
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
            Response.Write("{");
            Response.Write("\"callName\":\"" + callName + "\"");
            Response.Write(",\"details\":\"" + details + "\"");
            Response.Write(",\"compiled\":\"" + DateTime.UtcNow + "\"");
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