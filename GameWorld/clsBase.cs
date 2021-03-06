﻿using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GameWorld
{
    public class clsBase
    {
        public int id { get; set; }
        public DateTime created { get; set; }
        public DateTime modified { get; set; }
        protected clsDatabase _db;

        protected string tableName
        {
            get
            {
                // get table name
                string typeName = this.GetType().ToString(); // get the matching object name
                int start = typeName.LastIndexOf(".") + 4; // skip the cls
                int length = typeName.Length - start;
                return typeName.Substring(start, length).ToLower();
            }
        }

        public clsBase(clsDatabase db)
        {
            _db = db;
        }

        public clsBase(clsDatabase db, int id)
        {
            _db = db;
            this.load(id);
        }

        public clsBase(clsDatabase db, MySqlDataReader dr) {
            this.populate(dr);
        }

        protected int execute(string sqlQuery)
        {
            //_db.open();
            int result = _db.execute(sqlQuery);
            //_db.close();
            return result;
        }

        protected int delete(string sql)
        {
            int result = 0;

            //_db.open();
            MySqlDataReader dr = _db.query(sql); // execute query to get the record that are to be deleted
            while (dr.Read())
            {
                result += this.execute("DELETE FROM " + this.tableName + "s WHERE id = " + Convert.ToInt32(dr["id"]) + ";");
            }
            dr.Close();
            //_db.close();
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
            //_db.open(); 
            MySqlDataReader dr = _db.query(sqlQuery);
            object obj = get(dr, type);
            dr.Close();
            //_db.close();
            return obj;
        }

        // return a list of objects from database based on query
        protected List<object> getList(string sqlQuery, Type type)
        {
            // get sqlDataReader from dbCon and copy it to generic list so we can close the connection
            //_db.open();
            MySqlDataReader dr = _db.query(sqlQuery);
            List<object> results = getList(dr, type);
            dr.Close();
            //_db.close();
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

        public int destroy()
        {
            int result = 0;
            result += this.execute("DELETE FROM " + this.tableName + "s WHERE id = " + this.id + ";");
            if ( result > 0) this.id = 0;
            return result;
        }

        public int destroyAll()
        {
            return this.execute("DELETE FROM " + this.tableName + "s;");
        }

        // load this object with the record in the DB with the same ID
        public bool load(MySqlDataReader dr)
        {
            return this.populate(dr);
        }

        // load this object with the first record in the DB that matches the query
        public bool load(string sqlQuery)
        {
            //_db.open();
            MySqlDataReader dr = _db.query(sqlQuery);
            bool result = this.load(dr);
            dr.Close();
            //_db.close();
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
            string sql, names = "", values = "", where = "";
            int result = 0;

            //_db.open(); // open DB connection

            if (this.id == 0)
            {
                // insert - INSERT INTO table (column1, column2, ... ) VALUES (expression1, expression2, ... );

                // set defaults
                this.created = DateTime.UtcNow;
                this.modified = DateTime.UtcNow;

                // build names and value for insert statment
                string insertDelimiter = "";
                string whereDelimiter = "";

                foreach (var propertyInfo in this.GetType().GetProperties())
                {
                    if (propertyInfo.Name != "ID")
                    {
                        switch (propertyInfo.PropertyType.ToString())
                        {
                            case "System.Boolean":
                            case "System.Int16":
                            case "System.Int32":
                            case "System.DateTime":
                            case "System.String":
                                {
                                    // build insert
                                    names += insertDelimiter + propertyInfo.Name; // build name list
                                    values += insertDelimiter + getProperty(propertyInfo.Name);
                                    insertDelimiter = ", ";

                                    // build where for look up as well
                                    if (propertyInfo.Name != "id") where += whereDelimiter + propertyInfo.Name + "=" + getProperty(propertyInfo.Name);
                                    whereDelimiter = " AND ";
                                    break;
                                }
                        }
                    }
                }

                // build and execute insert query
                sql = "INSERT INTO " + this.tableName + "s";
                sql += " (" + names + ")";
                sql += " VALUES (" + values + ");";
                int id = _db.executeInsert(sql);

                // old way
                // build query string for read back statment
                // MySqlDataReader dr = _db.query("SELECT * FROM " + this.tableName + "s WHERE " + where + ";");
                // this.load(dr); // get saved record with new ID
                // dr.Close();

                // new way
                this.load(id);
            }
            else
            {
                // update - UPDATE table SET column1 = expression1,column2 = expression2,... WHERE conditions;

                // set defaults
                this.modified = DateTime.UtcNow;

                sql = "UPDATE " + this.tableName + "s SET ";
                string updateDelimiter = "";
                foreach (var propertyInfo in this.GetType().GetProperties())
                {
                    if (propertyInfo.Name != "ID") // ignore unsavable properties
                    {
                        switch (propertyInfo.PropertyType.ToString())
                        {
                            case "System.Boolean":
                            case "System.Int16":
                            case "System.Int32":
                            case "System.DateTime":
                            case "System.String":
                                {
                                    string prop = getProperty(propertyInfo.Name);
                                    if (prop != null) // ignore unsavable property types
                                    {
                                        sql += updateDelimiter + propertyInfo.Name + " = " + prop;
                                        updateDelimiter = ", ";
                                    }
                                    break;
                                }
                        }
                    }
                }
                sql += " WHERE ID = " + this.id + ";";
                result = _db.execute(sql);


                // build query string for read back statment
                MySqlDataReader dr = _db.query("SELECT * FROM " + this.tableName + "s WHERE id=" + this.id + ";");
                this.load(dr);
                dr.Close();
            }

            //_db.close(); // close DB connection

            return result;
        }

        public int save(IEnumerable<intBase> objects)
        {
            int result = 0;
            foreach (intBase o in objects)
            {
                result += o.save(true);
            }
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
                        case "System.String":
                            {
                                // add quoted type property to json string
                                string text = propertyInfo.GetValue(this, null).ToString();
                                text = text.Replace("'", "''"); // db friendly
                                return "'" + text + "'";
                            }
                        default:
                            {
                                // add quoted type property to json string
                                return null;
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
                            propertyInfo.SetValue(this, Convert.ToInt32(dr[propertyInfo.Name]), null);
                            
                            break;
                        }
                    case "System.DateTime":
                        {
                            propertyInfo.SetValue(this, Convert.ToDateTime(dr[propertyInfo.Name]), null);
                            break;
                        }
                    case "System.Boolean":
                        {
                            propertyInfo.SetValue(this, Convert.ToBoolean(dr[propertyInfo.Name]), null);
                            //if (Convert.ToInt16(dr[propertyInfo.Name]) == 0) propertyInfo.SetValue(this, false);
                            //else propertyInfo.SetValue(this, true);
                            break;
                        }
                    case "System.String":
                        {
                            // strings
                            propertyInfo.SetValue(this, Convert.ToString(dr[propertyInfo.Name]), null);
                            break;
                        }
                    default:
                        {
                            // unknowns are not populated
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

        public string toJSON(bool hideDBElements = false)
        {
            string delimiter = "";
            string result = "{";
            foreach (PropertyInfo propertyInfo in this.GetType().GetProperties())
            {
                string name = propertyInfo.Name.ToLower();

                bool displayProperty = true;
                if (hideDBElements == true) {
                    if ((name == "created") || (name == "modified")) displayProperty = false;
                    else if (name.EndsWith("id")) displayProperty = false;
                }

                if ((name == "attributes") || (name == "templateattributes") || (name == "contents")) displayProperty = false;

                // properties that are skipped when full = false
                if (displayProperty == true)
                {
                    string value = propertyInfo.GetValue(this, null).ToString();

                    switch (propertyInfo.PropertyType.ToString())
                    {
                        case "System.Int16":
                        case "System.Int32":
                        case "System.Boolean":
                            {
                                // numbers
                                result += delimiter + "\"" + name + "\":" + value.ToLower() + ""; // the ToLower() is for bool values
                                break;
                            }
                        case "System.String":
                        case "System.DateTime":
                            {
                                // strings
                                result += delimiter + "\"" + name + "\":\"" + value + "\"";
                                break;
                            }
                        default:
                            {
                                // skip unknown property types
                                break;
                            }
                    }
                    delimiter = ",";
                }
            }
            result += "}";
            return result;
        }

        public bool fromJSON(JObject JSONObj)
        {
            bool result = true;

            foreach (PropertyInfo propertyInfo in this.GetType().GetProperties())
            {
                if (JSONObj[propertyInfo.Name] != null)
                {
                    switch (propertyInfo.PropertyType.ToString())
                    {
                        case "System.Int16":
                        case "System.Int32":
                            {
                                propertyInfo.SetValue(this, (Int32)JSONObj[propertyInfo.Name], null);
                                break;
                            }
                        case "System.String":
                            {
                                propertyInfo.SetValue(this, (string)JSONObj[propertyInfo.Name], null);
                                break;
                            }
                        case "System.DateTime":
                            {
                                propertyInfo.SetValue(this, (DateTime)JSONObj[propertyInfo.Name], null);
                                break;
                            }
                        case "System.Boolean":
                            {
                                propertyInfo.SetValue(this, (Boolean)JSONObj[propertyInfo.Name], null);
                                break;
                            }
                        default:
                            {
                                // skip unknown property types
                                result = false;
                                break;
                            }
                    }
                }

            }
            return result;
        }
    }
}