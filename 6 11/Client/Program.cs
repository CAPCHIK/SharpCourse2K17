using System;
using Shared.Models.Messages;
using Newtonsoft.Json;
using System.Threading;
using Client.ConsoleWrappers;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Client
{
    class Program
    {
        private static SimpleWrapper reader;
         
        static void Main(string[] args)
        {
            reader = new SimpleWrapper();
            reader.ReadLine();
            reader.Dispose();
        }
    }
}
