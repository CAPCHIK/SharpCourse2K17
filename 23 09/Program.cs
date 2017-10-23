using System;
using System.Threading;

namespace _23_09
{

    class Cat
    {
        private static int count;
        public static int Count => count;
        public Cat()
        {
            count++;
        }
    }
    class Program
    {
        private static int value = 0;
        static void Main(string[] args)
        {
            PingPong.Work();
            // new Cat();
            // new Cat();
            // new Cat();
            // new Cat();
            // Console.WriteLine(Cat.Count);

            // Thread thread1 = new Thread(IncrementMany);
            // thread1.Name = "First_";
            // Thread thread2 = new Thread(IncrementMany);
            // thread2.Name = "Second";
            // thread1.Start();
            // thread2.Start();
            // System.Console.WriteLine("Before");
            // Thread.Sleep(1000);
            // System.Console.WriteLine("After");
            // System.Console.WriteLine(value);
        }

        static void IncrementMany()
        {
            for(int i = 0; i < 500; i++)
                Increment();
        }

        static void Increment()
        {
            System.Console.WriteLine($"{Thread.CurrentThread.Name} Enter");
            int a = value;
            Thread.Sleep(1);
            System.Console.WriteLine($"{Thread.CurrentThread.Name} readed {a}");            
            a++;
            value = a;
            System.Console.WriteLine($"{Thread.CurrentThread.Name} settet value{a}");
            System.Console.WriteLine($"{value}");
        }
    }
}
