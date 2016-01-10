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
        private clsTiles _tiles;

        public clsMap(string path) 
        {
            _tiles = new clsTiles(path + "tiles.bmp");
        }

        public clsTiles tiles 
        { 
            get 
            { 
                return _tiles; 
            } 
        }
    }
}