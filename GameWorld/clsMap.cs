using System;
using System.Collections.Generic;

namespace GameWorld
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
        private Random r = new Random(); // seed the random

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
            return obj.getAll();
        }

        public List<clsObject> getObjectsByType(string objectType)
        {
            clsObject obj = new clsObject(_db);
            return obj.getByType(objectType);
        }

        // delete all objects in a particular area
        public int deleteArea(int x1, int y1, int x2, int y2, int containerId = 0)
        {
            // get all objects in the area and mark each one for delete
            int result = 0;
            clsObject obj = new clsObject(_db);
            result += obj.deleteArea(x1, y1, x2, y2, containerId);
            return result;
        }

        public int deleteAll()
        {
            clsObject obj = new clsObject(_db);
            return obj.deleteAll();
        }

        // destroy all objects in a particular area
        public int destroyArea(clsPoint topLeft, clsPoint bottomRight, int containerId = 0)
        {
            // initiate destruction method for each object
            int result = 0;
            clsObject obj = new clsObject(_db);
            result += obj.destroyArea((int)topLeft.x, (int)topLeft.y, (int)bottomRight.x, (int)bottomRight.y, containerId);
            return result;
        }

        public int destroyArea(int x1, int y1, int x2, int y2, int containerId = 0)
        {
            return this.destroyArea(new clsPoint(x1, y1), new clsPoint(x2, y2));
        }

        public int destroyAll()
        {
            int result = 0;

            clsObject obj = new clsObject(_db);
            result += obj.destroyAll();

            clsAttribute attribute = new clsAttribute(_db);
            result += attribute.destroyAll();

            return result;
        }

        public List<clsObject> getArea(int x1, int y1, int x2, int y2, int containerId = 0, DateTime? modified = null)
        {
            clsObject obj = new clsObject(_db);
            return obj.getArea(x1, y1, x2, y2, containerId, modified);
        }


        public string createWorld(int blocks)
        {
            string result = "";
            int blockSize = 17;

            // create the height array
            heights = new int[blockSize, blockSize];

            for (int y = 0; y < blocks; y++) 
            {
                for (int x = 0; x < blocks; x++) 
                {
                    result += createBlock(new clsPoint(x, y));
                }
            }
            return result;
        }

        public string createBlock(clsPoint block)
        {
            int size = heights.GetLength(0);

            // clear the height array
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    heights[x, y] = 0;
                }
            }

            int blockSize = size;
            clsPoint arrayMax = new clsPoint(blockSize - 1, blockSize - 1);
            clsPoint worldTopLeft = new clsPoint(block.x * blockSize, block.y * blockSize);
            clsPoint worldBotRight = new clsPoint(worldTopLeft.x + blockSize - 1, worldTopLeft.y + blockSize - 1);

            // seed the corners
            heights[0, 0] = getSeed(worldTopLeft);
            heights[blockSize - 1, 0] = getSeed(new clsPoint(worldBotRight.x, worldTopLeft.y));
            heights[0, blockSize - 1] = getSeed(new clsPoint(worldTopLeft.x, worldBotRight.y));
            heights[blockSize - 1, blockSize - 1] = getSeed(worldBotRight);

            // mid point displacement loop // http://minecraft.gamepedia.com/
            calculateMidPoints(size);

            return SaveMap(worldTopLeft);
        }

        public int getSeed(clsPoint worldLocation)
        {
            int maxRandomHeight = 6;
            return r.Next(0, maxRandomHeight);
        }

        public string SaveMap(clsPoint worldLocation)
        {
            int size = heights.GetLength(0);

            // clear block
            clsPoint worldLocationBotRight = new clsPoint(worldLocation.x + heights.Length - 1, worldLocation.y + heights.Length - 1);
            this.destroyArea(worldLocation, worldLocationBotRight);

            // load templates
            clsTemplate template = new clsTemplate(_db);
            List<clsTemplate> templates = template.getAllTemplates();

            int waterLevel = 0;

            // save results to database
            clsObject obj = new clsObject(_db);
            List<clsObject> results = new List<clsObject>();
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    obj = this.createObject((int)worldLocation.x + x, (int)worldLocation.y + y, heights[x, y] * 32, templates.Find(i => i.name.Contains("MC Grass")));

                    if (heights[x, y] < waterLevel)
                    {
                        for (int z = heights[x, y] + 1; z <= waterLevel; z++)
                        {
                            obj = this.createObject((int)worldLocation.x + x, (int)worldLocation.y + y, z * 32, templates.Find(i => i.name.Contains("MC Water")));
                        }
                    }

                }
            }

            // height map to JSON
            string JSON;
            JSON = "{";
            JSON = "\"objectsCreated\":" + results.Count + "";
            JSON += "}";
            return JSON;

        }

        public void calculateMidPoints(int subBlockSize) 
        {
            // http://www.playfuljs.com/realistic-terrain-in-130-lines/
            float roughness = 0.2F;


            int half = subBlockSize / 2;
            float scale = roughness * (float)subBlockSize;
            if (half < 1) return;
            int max = subBlockSize;

            // seed the random
            Random r = new Random();

            for (int y = half; y < max; y += subBlockSize) 
            {
                for (int x = half; x < max; x += subBlockSize) 
                {
                    square(x, y, half, r.Next(0, 3) * (int)(scale * 2 - scale));
                }
            }

            for (int y = 0; y <= max; y += half) 
            {
                for (int x = (y + half) % subBlockSize; x <= max; x += subBlockSize) 
                {
                    diamond(x, y, half, r.Next(0, 3) * (int)(scale * 2 - scale));
                }
            }

            calculateMidPoints(subBlockSize / 2);
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
            int min = 0;
            int max = heights.GetLength(0);

            if ((x >= min) && (x < max) && (y >= min) && (y < max))
            {
                if (value > 6) value = 6;
                if (value < 0) value = 0;
                heights[x, y] = value;
            }
        }

        public int getHeight(int x, int y)
        {
            int min = 0;
            int max = heights.GetLength(0);

            if ((x >= min) && (x < max) && (y >= min) && (y < max))
            {
                return heights[x, y];
            }
            else return -5000;
        }

    }
}