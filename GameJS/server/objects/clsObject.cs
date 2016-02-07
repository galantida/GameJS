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

        public List<clsObject> contents
        {
            get {
                return this.referenced();
            }
        }

        // returns the contents of this object. passing a modified date will return deleted as well.
        public List<clsObject> referenced(DateTime? modified = null)
        {
            // get every object in a particular area and container. (0 is not in a container)
            string sql = "SELECT * FROM " + this.tableName + "s WHERE containerId = " + this.id;

            // if a modified value is not passed only return objects that have not been deleted
            if (modified == null) sql += " AND deleted = 0";
            else
            {
                // if a modified value has been passed show things that have been recently deleted as well
                // date format for mySQL = '1/19/2016 9:14:03 PM'
                DateTime dt = (DateTime)modified;
                sql += " AND modified >= '" + dt.ToString("yyyy-MM-dd HH:mm:ss") + "'";
            }
            sql += " ORDER BY x, y, z;";
            return this.getList(sql);
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

        // delete an object by marking it as deleted so that other process can update their info
        new public bool delete()
        {
            this.deleted = true;

            // delete contents first
            foreach (clsObject obj in this.contents)
            {
                if (obj.delete() == false) this.deleted = false;
            }

            // only when all of the contents have been deleted delete container
            if (this.deleted == true)
            {
                if (this.save() == 0) this.deleted = false;
            }
            return this.deleted;
        }


        // destroy an object by removing it and every reference to it from the database
        public bool destroy()
        {
            bool result = true;

            // destroy contents first
            foreach (clsObject obj in this.contents)
            {
                if (obj.destroy() == false) result = false;
            }

            // only when all of the contents have been destroyed then destroy container
            if (result == true)
            {
                if (this.execute("DELETE FROM " + this.tableName + "s WHERE id = " + this.id + ";") > 0) this.id = 0;
                else result = false;
            }
            return result;
        }

        // destroy all objects in a particular area
        public int destroyObjects(int x1, int y1, int x2, int y2, int containerId = 0)
        {
            int result = 0;

            // initiate destruction method for each object
            List<clsObject> objs = getObjects(x1, y1, x2, y2, containerId);
            foreach (clsObject obj in this.contents)
            {
                if (obj.destroy() == true) result++;
            }
            return result;
        }

        // delete all objects in a particular area
        public int deleteObjects(int x1, int y1, int x2, int y2, int containerId = 0)
        {
            int result = 0;

            // initiate destruction method for each object
            List<clsObject> objs = getObjects(x1, y1, x2, y2, containerId);
            foreach (clsObject obj in this.contents)
            {
                if (obj.delete() == true) result++;
            }
            return result;
        }

        // returns all objects in a particular area and cotnainer. modifed will return deleted objects as well.
        public List<clsObject> getObjects(int x1, int y1, int x2, int y2, int containerId =0, DateTime? modified = null)
        {
            util.sort(ref x1, ref x2);
            util.sort(ref y1, ref y2);

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