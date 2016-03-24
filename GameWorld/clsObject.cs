using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace GameWorld
{
    // the tile object is a container for information pertaining to a single ground tile collected from multiple files
    public class clsObject : clsBase, intBase
    {
        // properties
        public string name { get; set; }
        public int x { get; set; }
        public int y { get; set; }
        public int z { get; set; }
        public string type { get; set; }
        public string image { get; set; }
        public int weight { get; set; }
        public bool stackable { get; set; }
        public bool blocking { get; set; }
        public int containerId { get; set; }
        public bool deleted { get; set; }
        

        // children
        private List<clsAttribute> _attributes = null;
        private List<clsObject> _contents = null;

        public clsObject(clsDatabase db) : base(db) { }
        public clsObject(clsDatabase db, int id) : base(db, id) { }
        public clsObject(clsDatabase db, MySqlDataReader dr) : base(db, dr) { }

        // create an object from a template
        public clsObject(clsDatabase db, int x,int y, int z, clsTemplate template): base(db)
        {
            this.x = x;
            this.y = y;
            this.z = z;

            this.type = template.type;
            this.name = template.name; // this should be the attribute name not the template name
            this.image = template.image;
            this.weight = template.weight;
            this.stackable = template.stackable;
            this.blocking = template.blocking;
            this.save();

            // loop through template attibutes and use them to create object
            foreach (clsTemplateAttribute ta in template.templateAttributes)
            {
                // copy paste right now but could have random numbers, names etc....
                clsAttribute a = new clsAttribute(_db);
                a.objectId = this.id;
                a.name = ta.name;
                a.value = ta.value;
                a.save();
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

        // returns all objects in this table
        public List<clsObject> getAll()
        {
            // get every object in a particular area and container. (0 is not in a container)
            return this.getList("SELECT * FROM " + this.tableName + "s WHERE deleted = 0 ORDER BY name DESC");
        }

        // returns all objects in this table
        public List<clsObject> getByType(string objectType)
        {
            // get every object ov a certain type
            return this.getList("SELECT * FROM " + this.tableName + "s WHERE deleted = 0 AND type = '" + objectType +  "' ORDER BY name DESC");
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
        public List<clsObject> contents(DateTime? modified = null)
        {
            if (modified == null)
            {
                if (_contents == null) _contents = this.getList("SELECT * FROM " + this.tableName + "s WHERE containerId = " + this.id);
                return _contents;
            }
            else
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
        public int delete()
        {
            int result = 0;

            // mark contents for delete first
            result += this.delete(this.contents()); // mark its children as deleted 

            // mark this object as deleted
            this.deleted = true;
            result += this.save(); 

            return result;
        }

        // deletes all objects in a particular area and container. (0 is not in a container)
        public int deleteArea(int x1, int y1, int x2, int y2, int containerId = 0)
        {
            util.sort(ref x1, ref x2);
            util.sort(ref y1, ref y2);

            return this.execute("UPDATE " + this.tableName + "s SET deleted = true WHERE x>=" + x1 + " AND y>=" + y1 + " AND x<=" + x2 + " AND y<=" + y2 + " AND containerId = " + containerId);
        }

        public int delete(List<clsObject> objects)
        {
            // destroy contents
            int result = 0;
            foreach (clsObject obj in this.contents())
            {
                result += obj.delete();
            }
            return result;
        }

        public int deleteAll()
        {
            return this.execute("UPDATE " + this.tableName + "s SET deleted = true;");
        }


        // destroy an object by removing it and every reference to it from the database
        public new int destroy()
        {
            int result = 0;

            // destroy contents
            result += this.destroyContents();

            // destroy atributes
            result += this.execute("DELETE FROM attributes WHERE objectId = " + this.id + ";");
            
            // destroy object
            result += base.destroy();

            return result;
        }

        public int destroy(List<clsObject> objects)
        {
            int result = 0;
            foreach (clsObject obj in objects) 
            {
                result += this.destroy(obj.contents()); // call the same method to destroy this objects contents
                result += base.destroy();
            }
            return result;
        }

        public int destroyContents(bool internalUseOnly = true)
        {
            int result = 0;

            // execute this same function for the imeadiate contents of each of this objects imeadiate contents
            List<clsObject> objects = this.getList("SELECT * FROM " + this.tableName + "s WHERE containerId = " + this.id + ";");
            foreach (clsObject obj in objects)
            {
                result += obj.destroyContents(false);
            }

            // delete this objects imeadiate contents in one call
            result += this.execute("DELETE FROM " + this.tableName + "s WHERE containerId = " + this.id + ";");

            // once all objects and their contents are destroyed then destroy all disconnected attributes in one call
            if (internalUseOnly == true)
            {
                clsAttribute attribute = new clsAttribute(_db);
                attribute.destroyDisconnected();
            }

            return result;
        }

        // destroys all objects in a particular area and container. (0 is not in a container)
        public int destroyArea(int x1, int y1, int x2, int y2, int containerId = 0)
        {
            util.sort(ref x1, ref x2);
            util.sort(ref y1, ref y2);

            int result = 0;

            // destroy objects
            result += this.execute("DELETE FROM " + this.tableName + "s WHERE x>=" + x1 + " AND y>=" + y1 + " AND x<=" + x2 + " AND y<=" + y2 + " AND containerId = " + containerId);

            // destroy disconnected objects
            result += this.destroyDisconnected();

            // destroy disconnected attributes
            clsAttribute attribute = new clsAttribute(_db);
            result += attribute.destroyDisconnected();

            return result;
        }

        public int destroyDisconnected()
        {
            return this.delete("SELECT * FROM " + this.tableName + "s WHERE containerid <> 0 AND containerid NOT IN (SELECT id FROM objects);");
        }

        public static string toJSON(List<clsObject> objects, bool children = false, bool hideDBElements = false)
        {
            string delimiter = "";
            string JSON = "[";
            foreach (clsObject obj in objects)
            {
                JSON += delimiter + obj.toJSON(children, hideDBElements);
                delimiter = ",";
            }
            JSON += "]";
            return JSON;
        }

        public string toJSON(bool children = false, bool hideDBElements = false)
        {
            string JSONString = base.toJSON(hideDBElements);
            if (children == true)
            {
                JSONString = JSONString.Substring(0, JSONString.Length - 1);

                JSONString += ",\"attributes\":" + clsAttribute.toJSON(this.attributes, hideDBElements);
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
            this.save();

            // check for templateAtributes
            if (JSONObj["attributes"] != null)
            {
                JArray JSONArray = (JArray)JSONObj["attributes"];

                // convert to attribute objects
                clsAttribute attribute = new clsAttribute(_db);
                attribute.objectId = this.id;
                List<clsAttribute> attributes = attribute.fromJSON(JSONArray);

                foreach (clsAttribute a in attributes)
                {
                    a.objectId = this.id; // we will not know our template id until was save our template
                    a.save();
                }
            }

            return result;
        }
    }
}