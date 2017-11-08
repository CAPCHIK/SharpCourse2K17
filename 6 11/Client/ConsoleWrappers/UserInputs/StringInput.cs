using System;
namespace Client.ConsoleWrappers.UserInputs
{
    //Строка, введенная пользователем
    public class StringInput : UserInput
    {
        //Свойство сама строка, введенная пользователем
        public string InputString { get; }
        //Конструктор принимает эту строку и сохраняет в себе
        public StringInput(string inputString)
        {
            InputString = inputString;
        }
    }
}
