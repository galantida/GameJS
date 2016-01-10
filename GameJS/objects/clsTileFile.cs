using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GameJS
{
    // the tile file contains the images used to represent the ground tiles
    public class clsTileFile
    {
        private int _width = 0;
        private int _heigth = 0;
        private clsFile tileFile;

        public clsTileFile(string path)
        {
            tileFile = new clsFile(path);
        }

        public int width
        {
            get {
                if (_width == 0)
                {
                   _width = tileFile.read(18, 4);
                }
                return _width;
            }
        }

        public int height
        {
            get
            {
                if (_heigth == 0)
                {
                    _heigth = tileFile.read(22, 4);
                }
                return _heigth;
            }
        }

        public clsTile getTile(int x, int y)
        {
            clsTile newTile;
            int imgstart = 1974; // remember firt byte area 0,0,0 for black
            int seek = imgstart + ((this.width * 3) * y) + (x * 3);
            int rgb = tileFile.read(seek, 3);
            int z = (rgb >> 16) & 0xFF;
            string tileset = "yars";
            int column = (rgb) & 0xFF;
            int row = (rgb) & 0xFF;
            newTile = new clsTile(x, y, z, tileset, column, row);
            return newTile;
        }

        // this will need to be optimzed later
        public Dictionary<int, clsTile> getTileRow(int y, int x1, int x2)
        {
            Dictionary<int, clsTile> tiles = new Dictionary<int, clsTile>();
            for (int x = x1; x <= x2; x++)
            {
                clsTile tile = this.getTile(x, y);
                tiles.Add(tile.x, tile);
            }
            return tiles;
        }

        // this will need to be optimzed later
        public Dictionary<int, clsTile> getTileColumn(int x, int y1, int y2)
        {
            Dictionary<int, clsTile> tiles = new Dictionary<int, clsTile>();
            for (int y = y1; y <= y2; y++)
            {
                clsTile tile = this.getTile(x, y);
                tiles.Add(tile.y, tile);
            }
            return tiles;
        }
    }
}