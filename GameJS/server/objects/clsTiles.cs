using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MySql.Data.MySqlClient;
using System.Reflection;

namespace GameJS
{
    // ground tiles for the entire world can be accessed via this object
    public class clsTiles
    {
        private clsDatabase _db;

        public clsTiles(clsDatabase db)
        {
            _db = db;
        }

        public List<clsTile> query(string queryString)
        {
            List<clsTile> tiles = new List<clsTile>();


            _db.open();
            MySqlDataReader dr = _db.query(queryString);


            while (dr.Read()) // loop through rows
            {
                clsTile tile = new clsTile();

                foreach (PropertyInfo propertyInfo in tile.GetType().GetProperties())
                {
                    switch (propertyInfo.PropertyType.ToString())
                    {
                        case "System.Int16":
                        case "System.Int32":
                            {
                                // numbers
                                propertyInfo.SetValue(tile, Convert.ToInt32(dr[propertyInfo.Name]));
                                break;
                            }
                        default:
                            {
                                // strings
                                propertyInfo.SetValue(tile, Convert.ToString(dr[propertyInfo.Name]));
                                break;
                            }
                    }
                }
                tiles.Add(tile);
            }
            _db.close();

            return tiles;
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

            return this.query("SELECT * FROM tiles WHERE x>=" + x1 + " AND y>=" + y1 + " AND x<=" + x2 + " AND y<=" + y2 + " ORDER BY x, y, z;");
        }

        public clsTile getTile(int x1, int y1)
        {
            List<clsTile> tiles = this.getTiles(x1, y1, x1, y1);
            if (tiles.Count > 0) return tiles[0];
            else return null;
        }

        public List<clsTile> getRow(int x1, int x2, int y1)
        {
            return this.getTiles(x1, y1, x2, y1);
        }

        public List<clsTile> getCol(int x1, int y1, int y2)
        {
            return this.getTiles(x1, y1, x1, y1);
        }

        // could add functions for getting columns row and single tiles

    }
}