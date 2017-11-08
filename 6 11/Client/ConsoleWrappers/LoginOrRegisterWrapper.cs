using Client.ConsoleWrappers.UserInputs;

namespace Client.ConsoleWrappers
{
    //Форма для выбора дальнейшего пути. Либо логин в систему либо регстрация
    public class LoginOrRegisterWrapper : ConsoleWrappersBase
    {
        //Секция действий, пишем что нажав /1 мы будеи входить в систему
        //А при вводе /2 будем регистрироваться
        protected override void RenderActionSection()
        {
            System.Console.WriteLine("/1 login /2 register");
        }
        //Контент, поясняющее сообщение, показывающее, что надо сделать
        protected override void RenderContent()
        {
            System.Console.WriteLine("Hi! enter action for login ot register!");
        }
        //Получение пользовательского действия
        public UserInput GetUserChoose()
        {
            //Считали действие
            var input = ReadInput();
            switch (input)
            {
                //Если юзер ввел строку, снова пытаемся взять у него действие
                //Новым вызовом этого же метода
                case StringInput str:
                    Render();
                    return GetUserChoose();
                //Если юзер ввел действие 1 или действие 2 то возвращаем это действие 
                case ActionInput action when (action.ActionIndex == 1) :
                    return input;
                case ActionInput action when (action.ActionIndex == 2) :
                    return input;
                //Какое-то непонятное действие - снова просим ввести нормальное действие
                //Выховом этого же метода
                default :
                    Render();
                    return GetUserChoose();
            }
        }
    }
}