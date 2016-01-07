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
        private string _img;
        // private bool _blocked;

        public clsTile(int x, int y, int z, int bytes)
        {
            _x = x;
            _y = y;
            _z = z;
            _img = (bytes.ToString("X") + "-00").PadLeft(9, '0');
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

        public string img
        {
            get
            {
                return _img;
            }
        }

        public string toJSON
        {
            get
            {
                string result = "";
                result += "{";
                result += "\"x\":\"" + _x + "\"";
                result += ",\"y\":\"" + _y + "\"";
                result += ",\"z\":\"" + _z + "\"";
                result += ",\"img\":\"" + _img + "\"";
                result += "}";
                return result;
            }
        }


    }
}