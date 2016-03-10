using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;


namespace GameJS
{
    // world object that contains everything to instance an entire universe
    public class clsWorld
    {
        // map objects
        public clsDatabase db {get; set; }
        public clsMap map;

        public clsWorld()
        {
            string dbConString = WebConfigurationManager.AppSettings["dbConString"];
            this.db = new clsDatabase(dbConString);
            this.map = new clsMap(this.db);
        }

        public clsTemplate getTemplate(int templateId)
        {
            return new clsTemplate(this.db, templateId);
        }

        public List<clsTemplate> getAllTemplates()
        {
            clsTemplate template = new clsTemplate(this.db);
            return template.getAllTemplates();
        }

        

        
    }
}