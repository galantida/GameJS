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

        public clsTemplate(clsDatabase db) : base(db) { }
        public clsTemplate(clsDatabase db, int id) : base(db, id) { }
        public clsTemplate(clsDatabase db, MySqlDataReader dr) : base(db, dr) { }

        // convert a generic object to a typed one
        private clsTemplate get(string sqlQuery)
        {
            return (clsTemplate)get(sqlQuery, typeof(clsTemplate));
        }

        // convert a generic list to a typed list
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
                clsTemplateAttribute templateAttribute = new clsTemplateAttribute(_db);
                return templateAttribute.getTemplateAttributes(this.id);
                // remember to delete these in the destroy function
            }
        }

        // returns all objects in a particular area and cotnainer. modifed will return deleted objects as well.
        public List<clsTemplate> getTemplates()
        {
            // get every object in a particular area and container. (0 is not in a container)
            string sql = "SELECT * FROM " + this.tableName + "s order by name desc";
            return this.getList(sql);
        }

        public static string toJSON(List<clsTemplate> templates)
        {
            string delimiter = "";
            string JSON = "[";
            foreach (clsTemplate prop in templates)
            {
                JSON += delimiter + prop.toJSON();
                delimiter = ",";
            }
            JSON += "]";
            return JSON;
        }

        public new void fromJSON(string JSON)
        {
            // put this in base and flip it on its head
            JObject JSONObj = base.fromJSON(JSON);

            this.name = (string)JSONObj["name"];
            this.image = (string)JSONObj["image"];
            this.created = (DateTime)JSONObj["created"]; // need if exists
            this.modified = (DateTime)JSONObj["modified"]; // need if exists
        }
    }
}