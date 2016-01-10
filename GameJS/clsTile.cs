using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

namespace GameJS
{
    // the tile object is a container for information pertaining to a single ground tile collected from multiple files
    public class clsTile
    {
        private int _x;
        private int _y;
        private int _z;
        private int _tileset;
        private int _tile;

        public clsTile(int x, int y, int z, int tileset, int tile)
        {
            _x = x;
            _y = y;
            _z = z;
            _tileset = tileset;
            _tile = tile;
        }

        public int x
        {
            get
            {
                return _x;
            }
        }

        public int y
        {
            get
            {
                return _y;
            }
        }

        public int z
        {
            get
            {
                return _z;
            }
        }

        public int tileset
        {
            get
            {
                return _tileset;
            }
        }

        public int tile
        {
            get
            {
                return _tile;
            }
        }

        public string toJSON
        {
            get
            {
                string result = "";
                result += "{";
                result += "\"x\":" + _x + "";
                result += ",\"y\":" + _y + "";
                result += ",\"z\":" + _z + "";
                result += ",\"tileset\":" + _tileset + "";
                result += ",\"tile\":" + _tile + "";
                result += "}";
                return result;
            }
        }


    }
}