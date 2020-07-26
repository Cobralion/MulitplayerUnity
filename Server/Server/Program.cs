using System;
using System.Threading;
using System.Threading.Tasks;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Game Server";

            Server.Start(2, 4200);

            Console.ReadKey();
            Task.Delay(100);
            Console.ReadKey();
        }
    }
}
