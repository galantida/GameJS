using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GameJS
{
    // world object that contains everything to instance an entire universe
    public class clsWorld
    {
        private string _path = "C:\\Users\\Home\\Documents\\Visual Studio 2012\\Projects\\GameJS\\GameJS\\World\\";

        // map objects
        private clsMap _map;

        public clsWorld()
        {
            _map = new clsMap(_path);
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