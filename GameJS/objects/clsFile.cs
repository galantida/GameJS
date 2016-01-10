using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

namespace GameJS
{
    public class clsFile
    {
        private string _path;

        public clsFile(string path)
        {
            _path = path;
        }

        public int read(int fileLocation, int size)
        {
            // reserve file stream
            FileStream fs;
            int[] bytes = {0,0,0,0};

            // OpenFile file and read bytes
            fs = File.OpenRead(_path);
            fs.Seek(fileLocation, SeekOrigin.Begin);
            for (int t = 0; t < size; t++)
            {
                bytes[t] = fs.ReadByte();
            }
            fs.Close();

            // return bytes
            return (bytes[3] * 16777216) + (bytes[2] * 65536) + (bytes[1] * 256) + bytes[0];
        }
    }
}