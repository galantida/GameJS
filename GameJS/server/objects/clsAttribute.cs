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
    public class clsAttribute : clsBase, intBase
    {
        // properties
        public string name { get; set; }
        public string value { get; set; }

        public clsAttribute(clsDatabase db) : base(db) { }
        public clsAttribute(clsDatabase db, int id) : base(db, id) { }
        public clsAttribute(clsDatabase db, MySqlDataReader dr) : base(db, dr) { }

        // convert a generic object to a typed one
        private clsAttribute get(string sqlQuery)
        {
            return (clsAttribute)get(sqlQuery, typeof(clsAttribute));
        }

        // convert a generic list to a typed list
        private List<clsAttribute> getList(string sqlQuery)
        {
            List<object> attributes = base.getList(sqlQuery, typeof(clsAttribute));

            List<clsAttribute> results = new List<clsAttribute>();
            foreach (object attribute in attributes)
            {
                clsAttribute result = (clsAttribute)attribute; // giving this a try
                results.Add(result);
            }
            return results;
        }

        public List<clsAttribute> getObjectAttributes(int objectId)
        {
            return this.getList("SELECT * FROM " + this.tableName + " WHERE objectId = " + objectId);
        }

        public int deleteObjectAttributes(int objectId)
        {
            return this.execute("DELETE FROM " + this.tableName + " WHERE objectId = " + objectId);
        }

        public static string toJSON(List<clsAttribute> attributes)
        {
            string delimiter = "";
            string JSON = "[";
            foreach (clsAttribute prop in attributes)
            {
                JSON += delimiter + prop.toJSON();
                delimiter = ",";
            }
            JSON += "]";
            return JSON;
        }
    }
}