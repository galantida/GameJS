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
        private string _tileset;
        private int _column;
        private int _row;

        public clsTile(int x, int y, int z, string tileset, int column, int row)
        {
            _x = x;
            _y = y;
            _z = z;
            _tileset = tileset;
            _column = column;
            _row = row;
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

        public string tileset
        {
            get
            {
                return _tileset;
            }
        }

        public int column
        {
            get
            {
                return _column;
            }
        }
        public int row
        {
            get
            {
                return _row;
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
                result += ",\"tileset\":\"" + _tileset + "\"";
                result += ",\"column\":" + _column + "";
                result += ",\"row\":" + _row + "";
                result += "}";
                return result;
            }
        }


    }
}