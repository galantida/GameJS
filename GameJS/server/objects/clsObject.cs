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
    public class clsObject : clsBase, intBase
    {
        // properties
        public int x { get; set; }
        public int y { get; set; }
        public int z { get; set; }
        public string pack { get; set; }
        public string item { get; set; }
        public int containerId { get; set; }
        public bool deleted { get; set; }

        public clsObject(clsDatabase db) : base(db) { }
        public clsObject(clsDatabase db, int id) : base(db, id) { }
        public clsObject(clsDatabase db, MySqlDataReader dr) : base(db, dr) { }

        // convert a generic object to a typed one
        private clsObject get(string sqlQuery)
        {
            return (clsObject)get(sqlQuery, typeof(clsObject));
        }

        // convert a generic list to a typed list
        private List<clsObject> getList(string sqlQuery)
        {
            List<object> objs = getList(sqlQuery, typeof(clsObject));

            List<clsObject> results = new List<clsObject>();
            foreach (object obj in objs)
            {
                clsObject result = (clsObject)obj; // giving this a try
                //clsTile result = new clsTile(_db);
                //result.populateByObject(obj);
                results.Add(result);
            }
            return results;
        }

        public static string toJSON(List<clsObject> objects)
        {
            string delimiter = "";
            string JSON = "[";
            foreach (clsObject obj in objects)
            {
                JSON += delimiter + obj.toJSON();
                delimiter = ",";
            }
            JSON += "]";
            return JSON;
        }

        // must create a recursive contents deleter
        new public bool delete()
        {
            bool result = false;
            this.deleted = true;
            if (this.save() > 0) result = true;
            return result;
        }


        // must create a recursive contents deleter
        public bool destroy()
        {
            bool result = false;
            if (this.execute("DELETE FROM " + this.tableName + "s WHERE id = " + this.id + ";") > 0)
            {
                this.id = 0;
                result = true;
            }
            return result;
        }

        // returns the contents of this object. passing a modified date will return deleted as well.
        public List<clsObject> contents(DateTime? modified = null)
        {
            clsObject obj = new clsObject(base._db);
            return obj.getObjects(0, 0, 1000, 1000, containerId, modified);
        }

        public int destroyObjects(int x1, int y1, int x2 = 0, int y2 = 0, int containerId = 0)
        {
            if (x2 == 0) x2 = x1;
            if (y2 == 0) y2 = y1;

            if (x1 > x2)
            {
                int x = x1;
                x1 = x2;
                x2 = x;
            }

            if (y1 > y2)
            {
                int y = y1;
                y1 = y2;
                y2 = y;
            }

            // get every object in a particular area and container. (0 is not in a container)
            string sql = "DELETE FROM " + this.tableName + "s WHERE x>=" + x1 + " AND y>=" + y1 + " AND x<=" + x2 + " AND y<=" + y2 + " AND containerId = " + containerId;
            return this.execute(sql);
        }

        // returns all objects in a particular area and cotnainer. modifed will return deleted objects as well.
        public List<clsObject> getObjects(int x1, int y1, int x2 = 0, int y2 = 0, int containerId =0, DateTime? modified = null)
        {
            if (x2 == 0) x2 = x1;
            if (y2 == 0) y2 = y1;

            if (x1 > x2)
            {
                int x = x1;
                x1 = x2;
                x2 = x;
            }

            if (y1 > y2)
            {
                int y = y1;
                y1 = y2;
                y2 = y;
            }

            // get every object in a particular area and container. (0 is not in a container)
            string sql = "SELECT * FROM " + this.tableName + "s WHERE x>=" + x1 + " AND y>=" + y1 + " AND x<=" + x2 + " AND y<=" + y2 + " AND containerId = " + containerId;

            // if a modified value is not passed only return objects that have not been deleted
            if (modified == null) sql += " AND deleted = 0";
            else {
                // if a modified value has been passed show things that have been recently deleted as well
                // date format for mySQL = '1/19/2016 9:14:03 PM'
                DateTime dt = (DateTime)modified;
                sql += " AND modified >= '" + dt.ToString("yyyy-MM-dd HH:mm:ss") + "'";
            }
            sql += " ORDER BY x, y, z;";
            return this.getList(sql);
        }
    }
}