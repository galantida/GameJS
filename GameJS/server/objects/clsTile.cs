using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Reflection;


namespace GameJS
{
    // the tile object is a container for information pertaining to a single ground tile collected from multiple files
    public class clsTile
    {
        // properties
        public int x { get; set; }
        public int y { get; set; }
        public int z { get; set; }
        public string tileset { get; set; }
        public int column { get; set; }
        public int row { get; set; }

        public clsTile()
        {
            
        }

        

        public string toJSON()
        {
            string delimiter = "";
            string result = "{";
            foreach (PropertyInfo propertyInfo in this.GetType().GetProperties())
            {
                
                switch (propertyInfo.PropertyType.ToString())
                {
                    case "System.Int16":
                    case "System.Int32":
                        {
                            // numbers
                            result += delimiter + "\"" + propertyInfo.Name + "\":" + propertyInfo.GetValue(this, null) + "";
                            break;
                        }
                    default:
                        {
                            // strings
                            result += delimiter + "\"" + propertyInfo.Name + "\":\"" + propertyInfo.GetValue(this, null).ToString() + "\"";
                            break;
                        }
                }
                delimiter = ",";
            }
            result += "}";
            return result;
        }
    }
}