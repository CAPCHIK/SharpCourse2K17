using System;
using System.Threading;
using System.Collections.Generic;
namespace _23_09
{
    class Lambdas
    {
        public static void Work()
        {
            int[] ar = new int[]{1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11};
            int[] result = Select(ar, IsNeeded);
            System.Console.WriteLine(string.Join(",", result));
            Func<int, bool> functsia = ALALAL => ALALAL % 2 == 1;            
            int[] result2 = Select(ar, functsia);
            System.Console.WriteLine(string.Join(",", result2));

            Func<string, int> length = S => S.Length;
            int len = length("long string");            
        }
        static bool IsNeeded(int value)
        {
            System.Console.WriteLine($"getted {value}");
            return value % 2 == 0;
        }
        static int[] Select(int[] array, Func<int, bool> functsia)
        {
            List<int> result = new List<int>();
            foreach (var item in array)
            {
                System.Console.WriteLine($"Enter iteration for {item}");
                if(functsia(item))
                {
                    System.Console.WriteLine($"predicate returnet true");
                    result.Add(item);
                }
            }
            return result.ToArray();
            //For students
            DoSomething(S => System.Console.WriteLine($"{S} : {S.Length}"));
            var res = calculate(1, 4, (A, B) => A / B);     
        }

        //For Students
        static void DoSomething(Action<string> alalala)
        {
            System.Console.WriteLine("B");
            alalala("WTF");
            System.Console.WriteLine("A");
        }

        static int calculate(int first, int second, Func<int, int, int> oper)
        {
            return oper(first, second);
        }
    }
}