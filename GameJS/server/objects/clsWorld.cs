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
        private clsDatabase _db;
        private clsMap _map;

        public clsWorld()
        {
            _db = new clsDatabase("testdatabase","IBMs3666");
            _map = new clsMap(_db);
        }

        public clsMap map
        {
            get
            {
                return _map;
            }
        }

    }
}