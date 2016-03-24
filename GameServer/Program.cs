using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameWorld;
using System.Timers;

namespace GameServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Timer heart = new Timer(1000); // in miliseconds
            heart.Elapsed += new ElapsedEventHandler(heartBeat);
            heart.AutoReset = true;
            heart.Start();
            Console.Read();
        }

        static void heartBeat(object sender, EventArgs e)
        {
            string conString = "server=127.0.0.1;uid=root;database=testdatabase;pwd=IBMs3666;pooling=false;";
            clsWorld world = new clsWorld(conString);

            Console.WriteLine("Connecting to World...");
            world.open();

            // run AI
            processDogs(world);
            processChickens(world);

            Console.WriteLine("Disconnecting from World...");
            world.close();
        }



        static void processDogs(clsWorld world)
        {
            Console.Write("Processing Dogs");
            Random random = new Random();

            List<clsObject> dogs = world.map.getObjectsByType("dog");

            foreach (clsObject dog in dogs)
            {
                Console.Write(".");
                // chance of moving
                if (random.Next(0, 10) == 5)
                {
                    int randomX = random.Next(-3, 4);
                    int randomY = random.Next(-3, 4);

                    dog.x += randomX;
                    dog.y += randomY;
                    dog.save();
                }
            }

            Console.WriteLine(dogs.Count + " processed.");
            Console.WriteLine();
        }

        static void processChickens(clsWorld world)
        {
            Console.Write("Processing Chicken");
            Random random = new Random();

            List<clsObject> chickens = world.map.getObjectsByType("chicken");

            foreach (clsObject chicken in chickens)
            {
                Console.Write(".");
                // chance of moving
                if (random.Next(0, 5) == 3)
                {
                    int randomX = random.Next(-1, 2);
                    int randomY = random.Next(-1, 2);

                    chicken.x += randomX;
                    chicken.y += randomY;
                    chicken.save();
                }
            }

            Console.WriteLine(chickens.Count + " processed.");
            Console.WriteLine();
        }
    }
}
