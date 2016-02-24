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

            // process command first since that is the only parameter I can be sure of
            switch (callName.ToUpper())
            {
                case "CREATEOBJECT":
                    {
                        // required properties
                        int x = getNumericParameter("x", true);
                        int y = getNumericParameter("y", true);
                        int z = getNumericParameter("z", true);
                        int templateId = getNumericParameter("templateId", true);

                        // connect to worl
                        clsWorld world = new clsWorld(name, password);

                        // load template
                        clsTemplate template = world.getTemplate(templateId);
                        if (template == null) sendResponse("Error", "Could not locate template 'templateId=" + templateId + "' to create new object.");

                        // create object based on template
                        clsObject obj = world.map.createObject(x, y, z);
                        obj.item = template.image;
                        obj.pack = "stone";
                        obj.save(); 

                        // formulate response
                        sendResponse("createObject", "{id:" + obj.id + ",x:" + obj.x + ",y:" + obj.y + ",z:" + obj.z + "}", "[" + obj.toJSON() + "]");
                        break;
                    }
                case "UPDATEOBJECT":
                    {
                        int id = getNumericParameter("id", true);

                        // locate an existing object
                        clsWorld world = new clsWorld(name, password);
                        clsObject obj = world.map.getObject(id);
                        if (obj == null) sendResponse("Error", "Could not locate object 'id=" + id + "' to update.");


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

                        // formulate response
                        sendResponse("updateObject", "{id:" + obj.id + ",x:" + obj.x + ",y:" + obj.y + ",z:" + obj.z + "}", "[" + obj.toJSON() + "]");
                        break;
                    }
                case "DELETEOBJECT":
                    {
                        int id = getNumericParameter("id", true);

                        clsWorld world = new clsWorld(name, password);
                        clsObject obj = world.map.deleteObject(id);
                        if (obj == null) sendResponse("Error", "'Could not locate object 'id=" + id + "' to delete.");
                        else sendResponse("deleteObject", "{id:" + obj.id + "}", "[" + obj.toJSON() + "]");
                        break;
                    }
                case "GETAREA":
                    {
                        // read requested coordinate
                        int x1 = getNumericParameter("x1", true);
                        int y1 = getNumericParameter("y1", true);
                        int x2 = getNumericParameter("x2");
                        int y2 = getNumericParameter("y2");
                        DateTime? modified = getDateTimeParameter("modified");

                        clsWorld world = new clsWorld(name, password);
                        string JSON = clsObject.toJSON(world.map.getArea(x1, y1, x2, y2, modified));
                        sendResponse("getArea", "{x1:" + x1 + ",y1:" + y1 + ",x2:" + x2 + ",y2:" + y2 + "}", JSON);
                        break;
                    }
                case "CREATEAREA":
                    {
                        // read requested coordinate
                        int x1 = getNumericParameter("x1", true);
                        int y1 = getNumericParameter("y1", true);
                        int x2 = getNumericParameter("x2", true);
                        int y2 = getNumericParameter("y2", true);

                        // connect to world db
                        clsWorld world = new clsWorld(name, password);
                        //List<clsObject> result = world.map.createArea(x1,y1,x2,y2);
                        string JSON = world.map.createArea(x1,y1,x2 - x1);

                        // formulate response
                        sendResponse("setCubes", "{x1:" + x1 + ",y1:" + y1 + ",x2:" + x2 + ",y2:" + y2 + "}", JSON);
                        break;
                    }
                case "GETTEMPLATES":
                    {
                        clsWorld world = new clsWorld(name, password);
                        string JSON = clsTemplate.toJSON(world.getTemplates());
                        sendResponse("getTemplates", "{}", JSON);
                        break;
                    }
                case "TEST":
                    {
                        clsWorld world = new clsWorld(name, password);
                        clsTemplate template = new clsTemplate(world.db);
                        template.fromJSON("{\"name\":\"nameTest\",\"image\":\"imageTest\"}");
                        sendResponse("getTemplates", "{}", template.toJSON());
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