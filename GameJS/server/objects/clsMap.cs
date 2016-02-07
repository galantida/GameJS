using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

namespace GameJS
{
    // a map is a colmanation of multiple files
    //  tile information from the tile file
    //  elevation information from the elevation file
    //  path information from the path file
    public class clsMap
    {
        // map objects
        public clsDatabase db { get; set; }

        public clsMap(clsDatabase db) 
        {
            this.db = db;
        }

        public clsObject createObject(int x, int y, int z)
        {
            clsObject obj = new clsObject(this.db);
            obj.x = x;
            obj.y = y;
            obj.z = z;
            return obj;
        }

        public clsObject getObject(int id)
        {
            clsObject obj = new clsObject(this.db);
            if (obj.load(id) == true) return obj;
            else return null;
        }

        public clsObject deleteObject(int id)
        {
            clsObject obj = new clsObject(this.db, id);
            obj.delete();
            return obj;
        }

        public bool destroyObject(int id)
        {
            clsObject obj = new clsObject(this.db);
            return obj.destroy();
        }

        public List<clsObject> getArea(int x1, int y1, int x2, int y2, DateTime? modified = null)
        {
            clsObject obj = new clsObject(this.db);
            return obj.getArea(x1, y1, x2, y2, 0, modified);
        }

        public int deleteArea(int x1, int y1, int x2, int y2)
        {
            clsObject obj = new clsObject(this.db);
            return obj.deleteArea(x1, y1, x2, y2, 0);
        }

        public int destroyArea(int x1, int y1, int x2, int y2)
        {
            clsObject obj = new clsObject(this.db);
            return obj.destroyArea(x1, y1, x2, y2, 0);
        }

        public int createArea(int x1, int y1, int x2, int y2)
        {
            clsObject obj = new clsObject(this.db);
            return obj.destroyArea(x1, y1, x2, y2, 0);

            // map generator



        }
    }
}