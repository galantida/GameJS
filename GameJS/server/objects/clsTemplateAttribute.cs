using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Reflection;
using MySql.Data.MySqlClient;


namespace GameJS
{
    // the tile object is a container for information pertaining to a single ground tile collected from multiple files
    public class clsTemplateAttribute : clsBase, intBase
    {
        // properties
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
            return this.getList("SELECT * FROM " + this.tableName + " WHERE objectId = " + templateId);
        }

        public int deleteTemplateAttributes(int templateId)
        {
            return this.execute("DELETE FROM " + this.tableName + " WHERE objectId = " + templateId);
        }

        public static string toJSON(List<clsTemplateAttribute> attributes)
        {
            string delimiter = "";
            string JSON = "[";
            foreach (clsTemplateAttribute prop in attributes)
            {
                JSON += delimiter + prop.toJSON();
                delimiter = ",";
            }
            JSON += "]";
            return JSON;
        }
    }
}