using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
//using MySql.Data;

namespace GameJS
{
    // world object that contains everything to instance an entire universe
    public class clsWorld
    {
        // map objects
        public clsDatabase db {get; set; }
        private clsMap _map;

        public clsWorld(string name, string password)
        {
            this.db = new clsDatabase(name, password);
        }

        public clsMap map
        {
            get
            {
                return _map;
            }
        }

        // will merge elevation information in later
        public List<clsTile> getTiles(int x1, int y1, int x2, int y2, DateTime? modified = null)
        {
            clsTile tile = new clsTile(this.db);
            return tile.getTiles(x1,y1,x2,y2, modified);
        }

        public clsTile getTile(int x1, int y1)
        {
            List<clsTile> tiles = this.getTiles(x1, y1, x1, y1);
            if (tiles.Count > 0) return tiles[0];
            else return null;
        }
    }
}