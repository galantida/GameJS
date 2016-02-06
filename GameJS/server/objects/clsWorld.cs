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
        public clsMap map;

        public clsWorld(string name, string password)
        {
            this.db = new clsDatabase(name, password);
            this.map = new clsMap(this.db);
        }

        
    }
}