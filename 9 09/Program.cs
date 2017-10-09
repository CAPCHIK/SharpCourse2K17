using System;
using System.Collections.Generic;
using System.Linq;

namespace LOL
{
    // )(())(
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Input");
            string expression = Console.ReadLine();   
            string[] tokens = expression.Split(" ");
            foreach (var item in tokens)
                System.Console.WriteLine(item);
            //3 5 4 2 - * +
            var stack = new Stack<double>();


            foreach (var token in tokens)
            {
                double value;
                if (double.TryParse(token, out value))
                {
                    stack.Push(value);
                }
                else
                {
                    var second = stack.Pop();
                    var first = stack.Pop();
                    stack.Push(Calculate(token, first, second));
                }
                PrintStack(stack);
            }
            System.Console.WriteLine("Result");
            System.Console.WriteLine(stack.Pop());
            
        }

        static void PrintStack(Stack<double> stack)
        {
            string[] values = new string[stack.Count];
            int i = 0;
            foreach (var item in stack)
            {
                values[i] = item.ToString();
                i = i + 1;
            } 
            System.Console.WriteLine($"Count : {values.Length}");
            //System.Console.WriteLine("Count : " + values.Length);
            System.Console.WriteLine(string.Join(" -- ", values.Reverse()));
        }

        static double Calculate(string op, double a, double b)
        {
            switch (op)
            {
                case "+" :
                    return a + b;
                case "-" :
                    return a - b;
                case "*" :
                    return a * b;
                case "/" :
                    return a / b;
                //default : 
                //    return -1;
            }
            return -1;
        }

        static void WithStack()
        {
            Stack<char> stack = new Stack<char>();
            Console.WriteLine("Input");
            string skobki = Console.ReadLine();
            bool success = true;
            foreach(char currentSymbol in skobki)
            {
                if (currentSymbol == '(')
                {
                    stack.Push(currentSymbol);
                } else if (currentSymbol == ')' && stack.Count > 0)
                    stack.Pop();
                else 
                {
                    success = false;
                    break;
                }
            }
            if (stack.Count != 0 || !success)
                System.Console.WriteLine("NO");
            else
                System.Console.WriteLine("YES");
        }
        static void WithoutStack()
        {
            Console.WriteLine("Input");
            string skobki = Console.ReadLine();
            int opens = 0;
            for (int i = 0; i < skobki.Length; i++)
            {
                if (skobki[i] == '(')
                {
                    opens++;
                } else if (skobki[i] == ')')
                {
                    opens--;
                }
                if (opens < 0)
                    break;
            }
            if (opens == 0)
                System.Console.WriteLine("YES");
            else 
                System.Console.WriteLine("NO");
                
            Console.WriteLine("Hello World!");
        }
        //Далее идет два метода, написанных во время ответов на вопросы

        static bool IsPowerOfFive(string value)
        {
            var number = long.Parse(value);
            long testNum = 1;
            double res = 0;
            while ((res = Math.Pow(5, testNum)) <= number)
            {
                if (number == res)
                    return true;
                testNum++;
            }
            return false;
        }

        static void Test()
        {
            int [] ints = new int[]{1, 2, 3, 4, 5};


            foreach (var item in ints)
            {

                System.Console.WriteLine(item);
            }
            for(int i = 0; i < ints.Length; i++)
            {
                System.Console.WriteLine(ints[i-1]);
            }
        }
    }
}
