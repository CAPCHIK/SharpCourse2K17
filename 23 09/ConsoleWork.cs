using System;
using System.Threading;
using System.Collections.Generic;
namespace _23_09
{
    class ConsoleWork
    {
        public static void Work()
        {
            Thread redThread = new Thread(
                () => 
                {
                    for (int i = 0; i < 50; i++)
                        PrintText("RED", ConsoleColor.Red);
                }
            );
            Thread yellowThread = new Thread(
                () => 
                {
                    for (int i = 0; i < 50; i++)
                        PrintText("YELLOW", ConsoleColor.Yellow);
                }
            );
            redThread.Start();
            yellowThread.Start();
        }
        private static object syncObject = new object();
        static void PrintText(string text, ConsoleColor color)
        {
            lock(syncObject)
            {
                var previews = Console.ForegroundColor;
                Console.ForegroundColor = color;
                Thread.Sleep(1);
                System.Console.WriteLine(text);
                Console.ForegroundColor = previews;  
            }          
        }
    }
}