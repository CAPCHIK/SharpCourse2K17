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
         //Точка входа в программу
        static void Main(string[] args)
        {
            //Создаем ядро работы чата
            var core = new ClientCore();
            //Запускаем его работу
            core.Work();
            //Если работа завершена - выводим про это сообщение
            System.Console.WriteLine("The END");
        }
    }
}
