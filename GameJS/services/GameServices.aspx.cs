using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;

namespace GameJS
{
    public partial class GameServices : System.Web.UI.Page
    {
        

        protected void Page_Load(object sender, EventArgs e)
        {

            // Get Command
            string Command = "" + Request.QueryString["cmd"].ToUpper();

            // process command first since that is the only parameter I can be sure of
            switch (Command)
            {
                // return tile object - image, elevation
                case "GETTILE":
                    {
                        // read requested coordinate
                        int x = Convert.ToInt16("0" + Request.QueryString["x"]);
                        int y = Convert.ToInt16("0" + Request.QueryString["y"]);

                        clsWorld world = new clsWorld();
                        Response.Write(world.map.tiles.getTile(x, y).toJSON);
                        break;
                    }
                case "GETTILEROW":
                    {
                        // return tileno and elevation given world coordinate
                        int y = Convert.ToInt16("0" + Request.QueryString["y"]);
                        int x1 = Convert.ToInt16("0" + Request.QueryString["x1"]);
                        int x2 = Convert.ToInt16("0" + Request.QueryString["x2"]);

                        // validate input data
                        if (x1 > x2) 
                        {
                            int tmp = x1;
                            x1 = x2;
                            x2 = tmp;
                        }

                        string result = "";
                        result += "{";
                        result += "\"tiles\":";
                        result += "[";

                        clsWorld world = new clsWorld();
                        Dictionary<int, clsTile> tiles = world.map.tiles.getTileRow(y, x1, x2);

                        string comma = "";
                        foreach (KeyValuePair<int, clsTile> kvp in tiles)
                        {
                            result += comma + kvp.Value.toJSON;
                            comma = ",";
                        }

                        result += "]";
                        result += "}";

                        Response.Write(result);
                        break;
                    }
                case "GETTILECOLUMN":
                    {
                        // return tileno and elevation given world coordinate
                        int x = Convert.ToInt16("0" + Request.QueryString["x"]);
                        int y1 = Convert.ToInt16("0" + Request.QueryString["y1"]);
                        int y2 = Convert.ToInt16("0" + Request.QueryString["y2"]);

                        // validate input data
                        if (y1 > y2)
                        {
                            int tmp = y1;
                            y1 = y2;
                            y2 = tmp;
                        }

                        string result = "";
                        result += "{";
                        result += "\"tiles\":";
                        result += "[";

                        clsWorld world = new clsWorld();
                        Dictionary<int, clsTile> tiles = world.map.tiles.getTileColumn(x, y1, y2);

                        string comma = "";
                        foreach (KeyValuePair<int, clsTile> kvp in tiles)
                        {
                            result += comma + kvp.Value.toJSON;
                            comma = ",";
                        }

                        result += "]";
                        result += "}";

                        Response.Write(result);
                        break;
                    }
                default:
                    {
                        Response.Write("ERROR: Invalid Web Service Call");
                        break;
                    }
            }
            Response.End();
        }
    }
}