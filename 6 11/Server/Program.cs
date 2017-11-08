﻿using System;

namespace Server
{
    //программа сервера
    class Program
    {
        //Точка входа в программу. Код внутри метода Main будет выполнен этой программой
        static void Main(string[] args)
        {
            //объект core(ядро) будет управлять работой сервера, принимать сообщения, управлять пользователями и тд
            var core = new ServerCore();
            //ядро работает параллельно с основной программой, для начала работы надо вызвать метод StartWork()
            core.StartWork();
            //Немного логирования чтобы понять, что сервер запустился
            Console.WriteLine("STARTED!");
            //Если в консоли сервера мы нежмем Enter то выполнится метод ReadLine()
            Console.ReadLine();
            //И мы остановим работу сервера
            core.Stop();
        }
    }
}
