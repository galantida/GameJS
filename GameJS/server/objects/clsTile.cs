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
    public class clsTile : clsBase, intBase
    {
        // properties
        public int x { get; set; }
        public int y { get; set; }
        public int z { get; set; }
        public int tilesetId { get; set; }
        public int col { get; set; }
        public int row { get; set; }
        //protected clsDatabase db {get; set; }

        public clsTile(clsDatabase db) : base(db) {
           // this.db = db;
        }
        public clsTile(clsDatabase db, MySqlDataReader dr) : base(db, dr) { }

        public static string toJSON(List<clsTile> tiles)
        {
            string delimiter = "";
            string JSON = "[";
            foreach (clsTile tile in tiles)
            {
                JSON += delimiter + tile.toJSON();
                delimiter = ",";
            }
            JSON += "]";
            return JSON;
        }

        // convert a generic object to a typed one
        private clsTile get(string sqlQuery)
        {
            return (clsTile)get(sqlQuery, typeof(clsTile));
        }


        // convert a generic list to a typed list
        private List<clsTile> getList(string sqlQuery)
        {
            List<object> objs = getList(sqlQuery, typeof(clsTile));

            List<clsTile> results = new List<clsTile>();
            foreach (object obj in objs)
            {
                clsTile result = (clsTile)obj; // giving this a try
                //clsTile result = new clsTile(_db);
                //result.populateByObject(obj);
                results.Add(result);
            }
            return results;
        }


        // will merge elevation information in later
        public List<clsTile> getTiles(int x1, int y1, int x2, int y2)
        {
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

            return this.getList("SELECT * FROM tiles WHERE x>=" + x1 + " AND y>=" + y1 + " AND x<=" + x2 + " AND y<=" + y2 + " ORDER BY x, y, z;");
        }
    }
}