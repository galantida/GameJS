using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
//using MySql.Data;

namespace GameJS
{
    // world object that contains everything to instance an entire universe
    public class clsWorld
    {
        // map objects
        public clsDatabase db {get; set; }
        public clsMap map;

        public clsWorld(string name, string password)
        {
            this.db = new clsDatabase(name, password);
            this.map = new clsMap(this.db);
        }

        public List<clsTemplate> getTemplates()
        {
            clsTemplate template = new clsTemplate(this.db);
            return template.getTemplates();
        }

        public void BedRock()
        {
            // pack = stone
            // img = bedrock1
            // stackable = true
            // passthrough = false
            // container = false
            // ai = none
            // weight = 100;


            // ingredients 2 piles of stone

        }

        
    }
}