using System;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            var core = new ServerCore();
            core.StartWork();
            System.Console.WriteLine("STARTED!");
            Console.ReadLine();
            core.Stop();
        }
    }
}
