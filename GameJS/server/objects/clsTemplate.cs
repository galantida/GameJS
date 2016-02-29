using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Reflection;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace GameJS
{
    // the tile object is a container for information pertaining to a single ground tile collected from multiple files
    public class clsTemplate : clsBase, intBase
    {
        // properties
        public string name { get; set; }
        public string image { get; set; }

        private List<clsTemplateAttribute> _templateAttributes = null;

        public clsTemplate(clsDatabase db) : base(db) { }
        public clsTemplate(clsDatabase db, int id) : base(db, id) { }
        public clsTemplate(clsDatabase db, MySqlDataReader dr) : base(db, dr) { }

        public clsTemplate(clsDatabase db, string name): base(db)
        {
            this.load("SELECT * FROM " + this.tableName + "s WHERE name = '" + name + "'");
        }

        // convert a generic object to a typed one
        private clsTemplate get(string sqlQuery)
        {
            return (clsTemplate)get(sqlQuery, typeof(clsTemplate));
        }

        // query database and return typed list.
        private List<clsTemplate> getList(string sqlQuery)
        {
            List<object> templates = base.getList(sqlQuery, typeof(clsTemplate));

            List<clsTemplate> results = new List<clsTemplate>();
            foreach (object template in templates)
            {
                clsTemplate result = (clsTemplate)template; // giving this a try
                results.Add(result);
            }
            return results;
        }

        public List<clsTemplateAttribute> templateAttributes
        {
            get
            {
                if (_templateAttributes == null)
                {
                    clsTemplateAttribute templateAttribute = new clsTemplateAttribute(_db);
                    _templateAttributes = templateAttribute.getTemplateAttributes(this.id);
                }
                return _templateAttributes;
            }
        }

        public int save(bool children = false) {

            int result = base.save(); // save the base object

            // ifchildren is true then save all children as well
            if (children == true)
            {
                foreach (clsTemplateAttribute templateAttribute in this.templateAttributes)
                {
                    templateAttribute.templateId = this.id; // verify that each child actually has its parent id.
                    result += templateAttribute.save();
                }
            }
            return result;
        }

        public static string toJSON(List<clsTemplate> templates, bool children = false)
        {
            string delimiter = "";
            string JSON = "[";
            foreach (clsTemplate obj in templates)
            {
                JSON += delimiter + obj.toJSON(children);
                delimiter = ",";
            }
            JSON += "]";
            return JSON;
        }

        public string toJSON(bool children = false)
        {
            string JSONString = base.toJSON();
            if (children == true) 
            {
                JSONString = JSONString.Substring(0, JSONString.Length - 1);
                JSONString += ",\"templateAttributes\":" + clsTemplateAttribute.toJSON(this.templateAttributes) + "}";
            }
            return JSONString;
        }

        public List<clsTemplate> fromJSON(JArray JSONArray)
        {
            List<clsTemplate> results = new List<clsTemplate>();

            // loop thorugh all the JSON obects in the JSON array
            foreach (JObject JSONObject in JSONArray)
            {
                clsTemplate template = new clsTemplate(_db); // create new blank template
                template.fromJSON(JSONObject); // load based on JSON Object
                results.Add(template); // add new to results
            }
            return results;
        }

        public new bool fromJSON(JObject JSONObj)
        {
            bool result = base.fromJSON(JSONObj);
            this.id = 0; // the id in JSON is from previous db. clear it.

            // check for templateAtributes
            if (JSONObj["templateAttributes"] != null)
            {
                JArray JSONArray = (JArray)JSONObj["templateAttributes"];

                // convert to template attribute objects
                clsTemplateAttribute templateAttribute = new clsTemplateAttribute(_db);
                _templateAttributes = templateAttribute.fromJSON(JSONArray);

                foreach (clsTemplateAttribute ta in this.templateAttributes)
                {
                    ta.id = 0; // the id in JSON is from previous db. clear it.
                    ta.templateId = 0; // we will not know our template id until was save our template
                }
            }

            return result;
        }

        // returns all objects in this table
        public List<clsTemplate> getAllTemplates()
        {
            // get every object in a particular area and container. (0 is not in a container)
            return this.getList("SELECT * FROM " + this.tableName + "s order by name desc");
        }
    }
}