﻿using System;
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
        protected clsDatabase _db { get; set; }
        private int[,] heights;

        public clsMap(clsDatabase db) 
        {
            _db = db;
        }

        // only the worl object has access to template this may get promoted
        public clsObject createObject(int x, int y, int z, clsTemplate template)
        {
            return new clsObject(_db, x, y, z, template);
        }

        public List<clsObject> getAllObjects()
        {
            clsObject obj = new clsObject(_db);
            return obj.getAllObjects();
        }

        // delete all objects in a particular area
        public int deleteArea(int x1, int y1, int x2, int y2, int containerId = 0)
        {
            int result = 0;

            // get all objects in the area and mark each one for delete
            List<clsObject> objs = getArea(x1, y1, x2, y2, containerId);
            foreach (clsObject obj in objs)
            {
                if (obj.delete() == true) result++;
            }
            return result;
        }

        // destroy all objects in a particular area
        public int destroyArea(int x1, int y1, int x2, int y2, int containerId = 0)
        {
            int result = 0;

            // initiate destruction method for each object
            List<clsObject> objs = getArea(x1, y1, x2, y2, containerId);
            foreach (clsObject obj in objs)
            {
                if (obj.destroy() == true) result++;
            }
            return result;
        }

        public List<clsObject> getArea(int x1, int y1, int x2, int y2, int containerId = 0, DateTime? modified = null)
        {
            clsObject obj = new clsObject(_db);
            return obj.getArea(x1, y1, x2, y2, containerId, modified);
        }

        public string createArea(int x1, int y1, int size)
        {
            // make sure it a calculable size
            int sqrt = (int)Math.Sqrt(size);
            size = (sqrt * sqrt) + 1;

            // clear the build area
            clsObject obj = new clsObject(_db);
            this.destroyArea(-1000, -1000, 1000, 1000, 0);

            // seed the random
            Random r = new Random();

            // create the array
            heights = new int[size, size];
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    heights[x, y] = -1000;
                }
            }

            // set the corners
            heights[0, 0] = r.Next(0, 3);
            heights[size - 1, 0] = r.Next(0, 3);
            heights[0, size - 1] = r.Next(0, 3);
            heights[size - 1, size - 1] = r.Next(0, 3);
            

            // mid point displacement loop
            divide(heights.GetLength(0));

            // http://minecraft.gamepedia.com/

            int waterLevel = 2;

            // save results to database
            List<clsObject> results = new List<clsObject>();
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    for (int z = 0; z <= 25; z++)
                    {
                        if (z <= heights[x, y])
                        {
                            // land
                            if (z == 0)
                            {
                                obj = this.createObject(x, y, z * 32, new clsTemplate(_db, "MC Stone"));
                            }
                            else if (z == heights[x, y])
                            {
                                obj = this.createObject(x, y, z * 32, new clsTemplate(_db, "MC Grass"));
                            }
                            else
                            {
                                obj = this.createObject(x, y, z * 32, new clsTemplate(_db, "MC Dirt"));
                            }

                            obj.save();
                            results.Add(obj);
                        }
                        else
                        {
                            if (z <= waterLevel)
                            {
                                obj = this.createObject(x, y, z * 32, new clsTemplate(_db, "MC Water"));
                                obj.save();
                                results.Add(obj);
                            }
                        }

                        
                    }
                }
            }


            // height map to JSON
            string JSON, rowDelimiter, colDelimiter;

            JSON = "[";
            rowDelimiter = "";
            for (int y = 0; y < size; y++)
            {
                JSON += rowDelimiter + "{\"" + y + "\":";
                rowDelimiter = ", ";
                
                colDelimiter = "";
                JSON += "[";
                for (int x = 0; x < size; x++)
                {
                    JSON += colDelimiter + heights[x, y];
                    colDelimiter = ", ";
                }
                JSON += "]";

                JSON += "}";
                
            }
            JSON += "]";

            return JSON;
        }

        public void divide(int size) 
        {
            // http://www.playfuljs.com/realistic-terrain-in-130-lines/
            float roughness = 0.2F;
            
            
            int half = size / 2;
            float scale = roughness * size;
            if (half < 1) return;
            int max = heights.GetLength(0);

            // seed the random
            Random r = new Random();

            for (int y = half; y < max; y += size) 
            {
                for (int x = half; x < max; x += size) 
                {
                    square(x, y, half, r.Next(0, 3) * (int)(scale * 2 - scale));
                }
            }

            for (int y = 0; y <= max; y += half) 
            {
                for (int x = (y + half) % size; x <= max; x += size) 
                {
                    diamond(x, y, half, r.Next(0, 3) * (int)(scale * 2 - scale));
                }
            }

            divide(size / 2);
        }

        public void diamond(int x, int y, int size, int offset) 
        {
            int[] list = {getHeight(x, y - size), getHeight(x + size, y),getHeight(x, y + size),getHeight(x - size, y)};
            int ave = average(list);
            setHeight(x, y, ave + offset);
        }

        public void square(int x, int y, int size, int offset)
        {
            int[] list = { getHeight(x - size, y - size), getHeight(x + size, y - size), getHeight(x + size, y + size), getHeight(x - size, y + size) };
            int ave = average(list);
            setHeight(x, y, ave + offset);
        }

        public int average(int[] values)
        {
            int total = 0;
            int count = 0;

            for (int t = 0; t < values.GetLength(0); t++)
            {
                if (values[t] >= 0)
                {
                    total += values[t];
                    count++;
                }
            }

            if (count > 0) return (total / count);
            else return 0;
        }

        public void setHeight(int x, int y, int value) {
            if ((x >= 0) && (x < heights.GetLength(0)) && (y >= 0) && (y < heights.GetLength(1)))
            {
                if (value > 6) value = 6;
                if (value < 0) value = 0;
                heights[x, y] = value;
            }
        }

        public int getHeight(int x, int y)
        {
            if ((x >= 0) && (x < heights.GetLength(0)) && (y >= 0) && (y < heights.GetLength(1)))
            {
                return heights[x, y];
            }
            else return -5000;
        }

    }
}