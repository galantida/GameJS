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
        public List<clsCube> getCubes(int x1, int y1, int x2, int y2, DateTime? modified = null)
        {
            clsCube cube = new clsCube(this.db);
            return cube.getCubes(x1,y1,x2,y2, modified);
        }

        public clsCube getCube(int x1, int y1)
        {
            List<clsCube> cubes = this.getCubes(x1, y1, x1, y1);
            if (cubes.Count > 0) return cubes[0];
            else return null;
        }

        public List<clsObject> getObjects(int x1, int y1, int x2, int y2, DateTime? modified = null)
        {
            clsObject obj = new clsObject(this.db);
            return obj.getObjects(x1, y1, x2, y2, modified);
        }

        public clsObject getObject(int x1, int y1)
        {
            List<clsObject> objects = this.getObjects(x1, y1, x1, y1);
            if (objects.Count > 0) return objects[0];
            else return null;
        }
    }
}