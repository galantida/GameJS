using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GameJS
{
    public class clsElevationFile
    {
        private int _width = 0;
        private int _heigth = 0;
        private clsFile elevationFile;

        public clsElevationFile(string path)
        {
            elevationFile = new clsFile(path);
        }

        public int width
        {
            get {
                if (_width == 0)
                {
                   _width = elevationFile.read(18, 4);
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
                    _heigth = elevationFile.read(22, 4);
                }
                return _heigth;
            }
        }

        public int elevation(int x, int y)
        {
            return 0;
        }

        public Dictionary<int, int> getElevationsRow(int y, int x1, int x2)
        {
            return new Dictionary<int, int>();
        }

        public Dictionary<int, int> getElevationColumn(int x, int y1, int y2)
        {
            return new Dictionary<int, int>();
        }


    }
}