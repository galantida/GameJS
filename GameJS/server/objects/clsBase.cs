using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Reflection;
using MySql.Data.MySqlClient;

namespace GameJS
{
    public class clsBase
    {
        public int id { get; set; }
        public DateTime created { get; set; }
        public DateTime modified { get; set; }
        private clsDatabase _db;

        protected string tableName
        {
            get
            {
                // get table name
                string typeName = this.GetType().ToString(); // get the matching object name
                int start = typeName.LastIndexOf(".") + 4; // skip the cls
                int length = typeName.Length - start;
                return typeName.Substring(start, length);
            }
        }

        public clsBase(clsDatabase db)
        {
            _db = db;
        }

        public clsBase(clsDatabase db, MySqlDataReader dr) {
            this.populate(dr);
        }

        protected int execute(string sqlQuery)
        {
            _db.open();
            int result = _db.execute(sqlQuery);
            _db.close();
            return result;
        }

        // return an object based on the contents of a SQLDataReader
        protected static object get(MySqlDataReader dr, Type type)
        {
            intBase obj = (intBase)Activator.CreateInstance(type);
            if (dr.HasRows) obj.populate(dr);
            return obj;
        }

        // get an object from database based on query
        protected object get(string sqlQuery, Type type)
        {
            _db.open(); 
            object obj = get(_db.query(sqlQuery), type);
            _db.close();
            return obj;
        }

        // return a list of objects from database based on query
        protected List<object> getList(string sqlQuery, Type type)
        {
            // get sqlDataReader from dbCon and copy it to generic list so we can close the connection
            _db.open();
            List<object> results = getList(_db.query(sqlQuery), type);
            _db.close();
            return results;
        }

        // return a list of objects from database based on the contents of a SQLDataReader
        protected List<object> getList(MySqlDataReader dr, Type type)
        {
            List<object> objects = new List<object>();
            while (dr.Read())
            {
                intBase obj = (intBase)Activator.CreateInstance(type, _db);
                obj.populate(dr);
                objects.Add(obj);
            }
            return objects;
        }

        // load this object with the record in the DB with the same ID
        public bool load(MySqlDataReader dr)
        {
            return this.populate(dr);
        }

        // load this object with the first record in the DB that matches the query
        public bool load(string sqlQuery)
        {
            _db.open();
            bool result = this.load(_db.query(sqlQuery));
            _db.close();
            return result;
        }

        // load this object with the record in the DB with the same ID
        public bool load(int id)
        {
            return this.load("SELECT * FROM " + tableName + "s WHERE id = " + id);
        }

        // save and update
        public int save()
        {
            string sql, names = "", values = "", where = "", delimiter;
            int result = 0;

            _db.open(); // open DB connection

            if (this.id == 0)
            {
                // insert - INSERT INTO table (column1, column2, ... ) VALUES (expression1, expression2, ... );
                // select - SELECT * FROM table WHERE column1 = expression1 and column2 = expression2 and .....

                // set defaults
                this.created = DateTime.Now;
                this.modified = DateTime.Now;

                // build names and value for insert statment
                delimiter = "";
                foreach (var propertyInfo in this.GetType().GetProperties())
                {
                    if (propertyInfo.Name != "ID")
                    {
                        names += delimiter + propertyInfo.Name; // build name list
                        values += delimiter + getProperty(propertyInfo.Name);
                        delimiter = ", ";
                    }
                }

                // build and execute insert query
                sql = "INSERT INTO " + this.tableName + "s";
                sql += " (" + names + ")";
                sql += " VALUES (" + values + ");";
                result = _db.execute(sql);


                // build query string for reselect statment
                delimiter = "";
                foreach (var propertyInfo in this.GetType().GetProperties())
                {
                    if (propertyInfo.Name != "id")
                    {
                        where += delimiter + propertyInfo.Name + "=" + getProperty(propertyInfo.Name);
                        delimiter = " AND ";
                    }
                }

                // build query string for read back statment
                MySqlDataReader dr = _db.query("SELECT * FROM " + this.tableName + "s WHERE " + where + ";");
                this.load(dr);
                dr.Close();
            }
            else
            {
                // update - UPDATE table SET column1 = expression1,column2 = expression2,... WHERE conditions;

                // set defaults
                this.modified = DateTime.Now;

                sql = "UPDATE " + this.tableName + "s SET ";
                delimiter = "";
                foreach (var propertyInfo in this.GetType().GetProperties())
                {
                    if (propertyInfo.Name != "ID")
                    {
                        sql += delimiter + propertyInfo.Name + " = " + getProperty(propertyInfo.Name);
                        delimiter = ", ";
                    }
                }
                sql += " WHERE ID = " + this.id + ";";

                result = _db.execute(sql);

                // build query string for read back statment
                MySqlDataReader dr = _db.query("SELECT * FROM " + this.tableName + "s WHERE id=" + this.id + ";");
                this.load(dr);
                dr.Close();
            }

            _db.close(); // close DB connection

            return result;
        }

        public string getProperty(string name)
        {
            foreach (var propertyInfo in this.GetType().GetProperties())
            {
                if (propertyInfo.Name == name)
                {
                    switch (propertyInfo.PropertyType.ToString())
                    {
                        case "System.Boolean":
                            {
                                // add unquoted bool type property to json string
                                if ((bool)propertyInfo.GetValue(this, null) == true) return "1";
                                else return "0";
                            }
                        case "System.Int16":
                        case "System.Int32":
                            {
                                // add unquoted type property to json string
                                return propertyInfo.GetValue(this, null).ToString();
                            }
                        case "System.DateTime":
                            {
                                // add quoted type property to json string
                                DateTime dt = Convert.ToDateTime(propertyInfo.GetValue(this, null));
                                return "'" + dt.ToString("yyyy-MM-dd HH:mm:ss") + "'";
                            }

                        default:
                            {
                                // add quoted type property to json string
                                string text = propertyInfo.GetValue(this, null).ToString();
                                text = text.Replace("'", "''"); // db friendly
                                return "'" + text + "'";
                            }
                    }
                }
            }
            return null;
        }

        public bool populate(MySqlDataReader dr)
        {
            /// verify
            if (dr.HasRows == false) return false;

            // check if reader has been initialized
            try
            {
                dr.IsDBNull(0);
            }
            catch
            {
                dr.Read();
            }


            foreach (PropertyInfo propertyInfo in this.GetType().GetProperties())
            {
                // might have to add some code to handle DBNull value in dr[propertyInfo.Name]
                switch (propertyInfo.PropertyType.ToString())
                {
                    case "System.Int16":
                    case "System.Int32":
                        {
                            // numbers
                            propertyInfo.SetValue(this, Convert.ToInt32(dr[propertyInfo.Name]));
                            break;
                        }
                    case "System.DateTime":
                        {
                            propertyInfo.SetValue(this, Convert.ToDateTime(dr[propertyInfo.Name]));
                            break;
                        }
                    case "System.Boolean":
                        {
                            propertyInfo.SetValue(this, Convert.ToBoolean(dr[propertyInfo.Name]));
                            break;
                        }
                    default:
                        {
                            // strings
                            propertyInfo.SetValue(this, Convert.ToString(dr[propertyInfo.Name]));
                            break;
                        }
                }
            }
            return true;
        }
        

        // populate this typed object based on a generic object
        protected bool populate(object obj)
        {
            try
            {
                // loop through the typed objects properties and lookup those values in the generic object
                foreach (var thisPropertyInfo in this.GetType().GetProperties()) // loop through the objects properties
                {
                    foreach (var objPropertyInfo in obj.GetType().GetProperties()) // loop through the objects properties
                    {
                        // if you do find a match assign it
                        if (objPropertyInfo.Name == thisPropertyInfo.Name) thisPropertyInfo.SetValue(this, objPropertyInfo.GetValue(obj, null), null);
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public string toJSON(bool abbreviated = false)
        {
            string delimiter = "";
            string result = "{";
            foreach (PropertyInfo propertyInfo in this.GetType().GetProperties())
            {

                switch (propertyInfo.PropertyType.ToString())
                {
                    case "System.Int16":
                    case "System.Int32":
                        {
                            // numbers
                            result += delimiter + "\"" + propertyInfo.Name + "\":" + propertyInfo.GetValue(this, null) + "";
                            break;
                        }
                    default:
                        {
                            if ((abbreviated == true) && ((propertyInfo.Name == "created") || (propertyInfo.Name == "modified"))) break;
                            
                            // strings
                            result += delimiter + "\"" + propertyInfo.Name + "\":\"" + propertyInfo.GetValue(this, null).ToString() + "\"";
                            break;
                        }
                }
                delimiter = ",";
            }
            result += "}";
            return result;
        }
    }
}