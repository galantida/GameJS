using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GameJS
{
    // ground tiles for the entire world can be accessed via this object
    public class clsTiles
    {
        private clsTileFile _tileFile;
        

        public clsTiles(string path)
        {
            _tileFile = new clsTileFile(path);
        }

        // will merge elevation information in later
        public clsTile getTile(int x, int y)
        {
            return _tileFile.getTile(x,y);
        }

        // will merge elevation information in later
        public Dictionary<int, clsTile> getTileRow(int y, int x1, int x2)
        {
            return _tileFile.getTileRow(y,x1,x2);
        }

        // will merge elevation information in later
        public Dictionary<int, clsTile> getTileColumn(int x, int y1, int y2)
        {
            return _tileFile.getTileRow(x, y1, y2);
        }

    }
}