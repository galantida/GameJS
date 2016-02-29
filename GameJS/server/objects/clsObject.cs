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
    public class clsObject : clsBase, intBase
    {
        // properties
        public string name { get; set; }
        public int x { get; set; }
        public int y { get; set; }
        public int z { get; set; }
        public string image { get; set; }
        public int containerId { get; set; }
        public bool deleted { get; set; }

        private List<clsAttribute> _attributes = null;

        public clsObject(clsDatabase db) : base(db) { }
        public clsObject(clsDatabase db, int id) : base(db, id) { }
        public clsObject(clsDatabase db, MySqlDataReader dr) : base(db, dr) { }

        // create an object from a temple
        public clsObject(clsDatabase db, int x,int y, int z, clsTemplate template): base(db)
        {
            this.x = x;
            this.y = y;
            this.z = z;

            this.name = template.name; // this should be the attribute name not the template name
            this.image = template.image;

            // look through template attibutes and use them to create object
            foreach (clsTemplateAttribute ta in template.templateAttributes)
            {
                // copy paste right now but could have random numbers, names etc....
                clsAttribute a = new clsAttribute(_db);
                a.name = ta.name;
                a.value = ta.value;
                this.attributes.Add(a);
            }
        }

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

        // returns all objects in a particular area and cotnainer. modifed will return deleted objects as well.
        public List<clsObject> getArea(int x1, int y1, int x2, int y2, int containerId = 0, DateTime? modified = null)
        {
            util.sort(ref x1, ref x2);
            util.sort(ref y1, ref y2);

            // get every object in a particular area and container. (0 is not in a container)
            string sql = "SELECT * FROM " + this.tableName + "s WHERE x>=" + x1 + " AND y>=" + y1 + " AND x<=" + x2 + " AND y<=" + y2 + " AND containerId = " + containerId;

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

        public List<clsObject> contents
        {
            get {
                return this.referenced();
            }
        }

        public List<clsAttribute> attributes
        {
            get
            {
                if (_attributes == null)
                {
                    clsAttribute attribute = new clsAttribute(_db);
                    _attributes = attribute.getAttributes(this.id);
                }
                return _attributes;
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

        public int save(bool children = false)
        {

            int result = base.save(); // save the base object

            // ifchildren is true then save all children as well
            if (children == true)
            {
                foreach (clsAttribute attribute in this.attributes)
                {
                    attribute.objectId = this.id; // verify that each child actually has its parent id.
                    result += attribute.save();
                }
            }
            return result;
        }

        // delete an object by marking it as deleted so that other processes can update their info
        public bool delete()
        {
            this.deleted = true;

            // mark contents for delete first
            foreach (clsObject obj in this.contents)
            {
                if (obj.delete() == false) this.deleted = false;
            }

            // only when all of the contents have been marked for delete then mark this object for delete
            if (this.deleted == true)
            {
                if (this.save() == 0) this.deleted = false;
            }
            return this.deleted;
        }


        // destroy an object by removing it and every reference to it from the database
        public new bool destroy()
        {
            bool result = true;

            // destroy attributes
            foreach (clsAttribute attribute in this.attributes)
            {
                if (attribute.destroy() == false) result = false;
            }

            // destroy contents
            foreach (clsObject obj in this.contents)
            {
                if (obj.destroy() == false) result = false;
            }

            // only when all of the reference to this object have been destroyed then destroy object
            if (result == true)
            {
                if (this.execute("DELETE FROM " + this.tableName + "s WHERE id = " + this.id + ";") > 0) this.id = 0;
                else result = false;
            }
            return result;
        }

        public static string toJSON(List<clsObject> objects, bool children = false)
        {
            string delimiter = "";
            string JSON = "[";
            foreach (clsObject obj in objects)
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

                JSONString += ",\"attributes\":" + clsAttribute.toJSON(this.attributes);
                //JSONString += ",\"contents\":" + clsObject.toJSON(this.contents); // lets let each object stand on its own

                JSONString += "}";
            }
            return JSONString;
        }

        public List<clsObject> fromJSON(JArray JSONArray)
        {
            List<clsObject> results = new List<clsObject>();

            // loop thorugh all the JSON obects in the JSON array
            foreach (JObject JSONObject in JSONArray)
            {
                clsObject obj = new clsObject(_db); // create new blank template
                obj.fromJSON(JSONObject); // load based on JSON Object
                results.Add(obj); // add new to results
            }
            return results;
        }

        public new bool fromJSON(JObject JSONObj)
        {
            bool result = base.fromJSON(JSONObj);
            this.id = 0; // the id in JSON is from previous db. clear it.

            // check for templateAtributes
            if (JSONObj["attributes"] != null)
            {
                JArray JSONArray = (JArray)JSONObj["attributes"];

                // convert to attribute objects
                clsAttribute attribute = new clsAttribute(_db);
                _attributes = attribute.fromJSON(JSONArray);

                foreach (clsAttribute a in this.attributes)
                {
                    a.id = 0; // the id in JSON is from previous db. clear it.
                    a.objectId = 0; // we will not know our template id until was save our template
                }
            }

            return result;
        }

        // returns all objects in this table
        public List<clsObject> getAllObjects()
        {
            // get every object in a particular area and container. (0 is not in a container)
            return this.getList("SELECT * FROM " + this.tableName + "s WHERE deleted = 0 ORDER BY name DESC");
        }
    }
}