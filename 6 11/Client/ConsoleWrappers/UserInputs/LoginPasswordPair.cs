using System;
namespace Client.ConsoleWrappers.UserInputs
{
    //Пользовательский ввод, содержащий логин и пароль
    public class LoginPasswordPair : UserInput
    {
        //Введенный юзером логин
        public string Login {get; set;}
        //Введенный юзером пароль
        public int Password {get; set;}
    }
}