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

        public List<clsObject> getObjects(int x1, int y1, int x2, int y2, DateTime? modified = null)
        {
            clsObject obj = new clsObject(this.db);
            return obj.getObjects(x1, y1, x2, y2, modified);
        }

        public clsObject getObject(int id)
        {
            clsObject obj = new clsObject(this.db);
            if (obj.load(id) == true) return obj;
            else return null;
        }

        public clsObject createObject(int x, int y, int z)
        {
            clsObject obj = new clsObject(this.db);
            obj.x = x;
            obj.y = y;
            obj.z = z;
            return obj;
        }

        public bool deleteObject(int id)
        {
            bool result = false;
            clsObject obj = new clsObject(this.db);
            if (obj.load(id) == true)
            {
                result = obj.delete();
            }
            return result;
        }
    }
}