using System;
using System.Collections.Generic;
using System.Linq;
using Client.ConsoleWrappers.UserInputs;

namespace Client.ConsoleWrappers
{
    //Форма, предназначенная для показа функционала обертки
    //Данная форма просто запоминает все введенное и отображает
    //Пользователю. Почти как простая консоль
    public class SimpleWrapper : ConsoleWrappersBase
    {
        //Список введенных пользователем строк
        private List<string> rows = new List<string>();

        //При создании формы, рендерим ее содержимое
        public SimpleWrapper() : base()
        {
            Render();
        }

        //Контент формы - есть все сообщения, хранимые в списке введенных сообщени
        protected override void RenderContent()
        {
            //Проходимся по введенным строкам и выводим их
            foreach (var row in rows)
                Console.WriteLine(row);
        }
        //Метод для считывания строки
        public string ReadLine()
        {
            //Получаем ввод пользователя
            var readedString = ReadInput();
            switch (readedString)
            {
                //Если он ввел строку
                case StringInput str:
                    //Добавляем жту строку в наш буффер строк
                    AddString(str.InputString);
                    //И возвращаем эту строку вызывающему коду
                    return str.InputString;
                //Если пользователь ввел действие 1
                case ActionInput action when (action.ActionIndex == 1):
                    //Добавляем в буффер три строки
                    //первая из 1 запятой, другие две с 2-мя и 3-мя запятыми
                    foreach (var i in Enumerable.Range(1, 3))
                        AddString(new string(',', i));
                    break;
                //Ввел второе действие - ничего не делаем
                case ActionInput action when (action.ActionIndex == 2):
                    break;
                //обработка действий просто для примера
            }
            //Если ввели действие - воза=вращаем пустую строку
            return "";
        }
        //Метод добавления считанной строки в буффер
        public void AddString(string content)
        {
            //Добавляем строку в список строк
            rows.Add(content);
            //Рендерим содержимое
            Render();
        }
        //Секция действий отображает, что при действии 1 будет добавлено
        //3 строки в буфер когнсоли
        protected override void RenderActionSection()
        {
            Console.WriteLine("/1: write 3 lines");
        }
    }
}