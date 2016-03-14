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
    public class clsTemplateAttribute : clsBase, intBase
    {
        // properties
        public int templateId { get; set; }
        public string name { get; set; }
        public string value { get; set; }

        public clsTemplateAttribute(clsDatabase db) : base(db) { }
        public clsTemplateAttribute(clsDatabase db, int id) : base(db, id) { }
        public clsTemplateAttribute(clsDatabase db, MySqlDataReader dr) : base(db, dr) { }

        // convert a generic object to a typed one
        private clsTemplateAttribute get(string sqlQuery)
        {
            return (clsTemplateAttribute)get(sqlQuery, typeof(clsTemplateAttribute));
        }

        // convert a generic list to a typed list
        private List<clsTemplateAttribute> getList(string sqlQuery)
        {
            List<object> attributes = base.getList(sqlQuery, typeof(clsTemplateAttribute));

            List<clsTemplateAttribute> results = new List<clsTemplateAttribute>();
            foreach (object attribute in attributes)
            {
                clsTemplateAttribute result = (clsTemplateAttribute)attribute; // giving this a try
                results.Add(result);
            }
            return results;
        }

        public List<clsTemplateAttribute> getTemplateAttributes(int templateId)
        {
            return this.getList("SELECT * FROM " + this.tableName + "s WHERE templateId = " + templateId);
        }

        public int save(bool children = false)
        {
            return base.save();
        }

        public int destroyTemplateAttributes(int templateId)
        {
            return this.execute("DELETE FROM " + this.tableName + "s WHERE templateId = " + templateId);
        }

        public int destroyDisconnected()
        {
            return this.delete("SELECT * FROM " + this.tableName + "s WHERE templateId NOT IN (SELECT id FROM templates);");
        }

        public static string toJSON(List<clsTemplateAttribute> TemplateAttributes, bool hideDBElements = false)
        {
            string delimiter = "";
            string JSON = "[";
            foreach (clsTemplateAttribute attribute in TemplateAttributes)
            {
                JSON += delimiter + attribute.toJSON(hideDBElements);
                delimiter = ",";
            }
            JSON += "]";
            return JSON;
        }

        public List<clsTemplateAttribute> fromJSON(JArray JSONArray)
        {
            List<clsTemplateAttribute> results = new List<clsTemplateAttribute>();

            // loop thorugh all the JSON obects in the JSON array
            foreach (JObject JSONObject in JSONArray)
            {
                clsTemplateAttribute templateAttribute = new clsTemplateAttribute(_db); // create new blank template
                templateAttribute.fromJSON(JSONObject); // load template based on JSON Object
                results.Add(templateAttribute); // add new template to results
            }
            return results;
        }
    }
}