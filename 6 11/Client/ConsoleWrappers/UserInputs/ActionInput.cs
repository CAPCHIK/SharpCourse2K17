using System;
namespace Client.ConsoleWrappers.UserInputs
{
    //Класс, представляющий собой некое действике пользователя
    public class ActionInput : UserInput
    {
        //Имеет только индекс действия, некое число
        public byte ActionIndex { get; }
        //Конструктор принимает внутрь себя этот индекс
        public ActionInput(byte actionIndex)
        {
            ActionIndex = actionIndex;
        }
    }
}
