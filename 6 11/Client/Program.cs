using System;
using Shared.Models.Messages;
using Newtonsoft.Json;
using System.Threading;
using Client.ConsoleWrappers;
using Client.ConsoleWrappers.UserInputs;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;

namespace Client
{
    class Program
    {
         
        static void Main(string[] args)
        {
            var core = new ClientCore();
            core.Work();
            System.Console.WriteLine("The END");
        }
    }
}
