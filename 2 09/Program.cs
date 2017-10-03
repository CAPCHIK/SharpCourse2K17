using System;
using System.Text;
using System.Linq;
namespace untitled_folder
{
    class Program
    {
        static void Main(string[] args)
        {
            // System.Console.WriteLine("input first");
            // string first = Console.ReadLine();
            
            // System.Console.WriteLine("input oper");
            // string oper = Console.ReadLine();
            
            // System.Console.WriteLine("input second");
            // string second = Console.ReadLine();
            
            // int f = int.Parse(first);
            // int s = int.Parse(second);

            // double result;

            // switch(oper)
            // {
            //     case "+" : 
            //         result = f + s;
            //         break;
            //     case "-" :  
            //         result = f - s;
            //         break;
            //     case "/" :
            //         result = f / s;
            //         break;
            //     case "*" : 
            //         result = f * s;
            //         break;
            //     default :
            //         result = 0;
            //         break;
            // }


            // System.Console.WriteLine(result);

            var expression = Console.ReadLine();

            int value = GetResult(expression);
            System.Console.WriteLine(value);
            int vara = ;
            int newvar = Meth(vara);
            System.Console.WriteLine(newvar);
            int secvar = WMethg2(VARE);
            System.Console.WriteLine(secvar);
            
            Console.WriteLine("Hello World!");
        }
    static int GetResult(string input)
    {
        int result = 0;
        int i = 0;
        int f = 0;
        string oper;
        for (; i < input.Length; i++)
        {
            var varA = input[i];
            
            if(int.TryParse(input[i].ToString(), out var value))
            {
                f = f * 10 + value;
            }
            else
            {
                break;
            }
        }
        oper = input[i].ToString();
        i++;
        int s = 0;
        for (; i < input.Length; i++)
        {
            var varA = input[i];
            
            if(int.TryParse(input[i].ToString(), out var value))
            {
                s = s * 10 + value;
            }
            else
            {
                break;
            }
        }

        switch(oper)
            {
                case "+" : 
                    result = f + s;
                    break;
                case "-" :  
                    result = f - s;
                    break;
                case "/" :
                    result = f / s;
                    break;
                case "*" : 
                    result = f * s;
                    break;
                default :
                    result = 0;
                    break;
            }
        return result;
    }       
    //0123456789
    //32-678
        
            // long a = long.Parse(args[0]);
            // long b = long.Parse(args[1]);
            // System.Console.WriteLine(a + b);
            // Console.InputEncoding = Encoding.UTF8;
            // Console.OutputEncoding = Encoding.UTF8;

    }

}
