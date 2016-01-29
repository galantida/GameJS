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
    public class clsCube : clsBase, intBase
    {
        // properties
        public int x { get; set; }
        public int y { get; set; }
        public int z { get; set; }
        public int csId { get; set; }
        public int csCol { get; set; }
        public int csRow { get; set; }
        public int tsId { get; set; }
        public int tsCol { get; set; }
        public int tsRow { get; set; }

        public clsCube(clsDatabase db) : base(db) { }
        public clsCube(clsDatabase db, MySqlDataReader dr) : base(db, dr) { }

        public static string toJSON(List<clsCube> cubes)
        {
            string delimiter = "";
            string JSON = "[";
            foreach (clsCube cube in cubes)
            {
                JSON += delimiter + cube.toJSON();
                delimiter = ",";
            }
            JSON += "]";
            return JSON;
        }

        // convert a generic object to a typed one
        private clsCube get(string sqlQuery)
        {
            return (clsCube)get(sqlQuery, typeof(clsCube));
        }


        // convert a generic list to a typed list
        private List<clsCube> getList(string sqlQuery)
        {
            List<object> objs = getList(sqlQuery, typeof(clsCube));

            List<clsCube> results = new List<clsCube>();
            foreach (object obj in objs)
            {
                clsCube result = (clsCube)obj; // giving this a try
                //clsTile result = new clsTile(_db);
                //result.populateByObject(obj);
                results.Add(result);
            }
            return results;
        }


        // will merge elevation information in later
        public List<clsCube> getCubes(int x1, int y1, int x2 = 0, int y2 = 0, DateTime? modified = null)
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

            // date format for mySQL = '1/19/2016 9:14:03 PM'
            string sql = "SELECT * FROM cubes WHERE x>=" + x1 + " AND y>=" + y1 + " AND x<=" + x2 + " AND y<=" + y2;
            if (modified != null)
            {
                DateTime dt = (DateTime)modified;
                sql += " AND modified >= '" + dt.ToString("yyyy-MM-dd HH:mm:ss") + "'";
            }
            sql += " ORDER BY x, y, z;";
            return this.getList(sql);
        }
    }
}