using Client.ConsoleWrappers.UserInputs;

namespace Client.ConsoleWrappers
{
    //Форма для считывания пары логин/пароль
    public class LoginPasswordPairWrapper : ConsoleWrappersBase
    {
        //строка, отображающаяся в шапке формы, ее название
        private readonly string content;
        //Поле для хранения логина пользователя
        private string login = null;
        //Поле для хранения пароля
        private string password = null;
        //В конструкторе получаем шапку, которую будем выводить наверху формы
        public LoginPasswordPairWrapper(string content)
        {
            //Устанавливаем шапку
            this.content = content;
            //Отрисовываем форму
            Render();
        }
        //Действий не предпологается, только ввод логина и пароля
        protected override void RenderActionSection()
        {
        }
        //Отрисовка контента
        protected override void RenderContent()
        {
            //Пишем шапку
            System.Console.WriteLine(content);
            //Отступ в строку
            System.Console.WriteLine();
            
            //Пару слов про оператор ??
            //когда пишем переменная2 = переменная1 ?? <значение>
            //В переменная2 запишется переменная1 если переменная1 != null
            //Если же переменная1 == null то в переменная1 запишется <значение>
            //Таким образом используя конструкцию login ?? "..."
            //Мы получаем либо сам логин, если он уже готов, либо заглушку
            //Если логин еще не инициализирован
            //Выводим login: и сам логин, если уже ввели, или 3 точки, если еще не ввели
            System.Console.WriteLine($"login: {login ?? "..."}");
            //Выводим pass(only digits) и сам пароль, если уже ввели, или 3 точки, если еще не ввели
            System.Console.WriteLine($"pass (only digits): {password ?? "..."}");
        }
        //Считываем от пользователя пару логин/пароль
        public LoginPasswordPair GetPair()
        {
            //Первым делом просто читаем строку
            login = ReadString();
            //Рендерим содержимое, чтобы отобразить, что логин уже введен
            Render();
            //Получаем от пользователя число
            var numPass = ReadNumber();
            //В поле пароль записываем это число, только как строку
            //ВАЖНО
            //Если пользователь ввел 0005
            //То его паролем будет 5
            password = numPass.ToString();
            //Рендерим заполненную форму
            Render();
            //Возвращаем объект-контейнер, хранящий в себе логин и пароль
            return new LoginPasswordPair
            {
                Login = login,
                Password = numPass
            };
        }
        //Считывает от пользователя только число. причем целое
        private int ReadNumber()
        {
            //Переменная для результата
            int number;
            //int.TryParse() вернет false, если пользователь введет не число
            //И пока пользователь вводит не число мы рендерим содержимое формы снова и снова
            //Чем отчищаем его видимый буффер ввода
            //Так же, если захотим, можем поместить внутрь цикла метод для добавления
            //Просьбы ввести именно число. Но тут мы делать этого не будем
            while (!int.TryParse(ReadString(), out number))
            {
                Render();
            }
            //Если трайпарс вернул true то условие в while не прошло, и в переменной 
            //number лежит число, введенное пользователем. Возврашаем его.
            return number;
        }
        //Метод для считывания любой строки от пользователя
        private string ReadString()
        {
            //Условно бесконечный цикл
            while (true)
            {
                //Считали значение
                var input = ReadInput();
                //Если это значение строковой ввод
                if (input is StringInput str)
                    //Возвращаем его
                    return str.InputString;
                //Если это не строковый ввод, снова читаем.
                //Пока этот красавец не введет строку. Не надо тут действия
                //Нажимать
            }
        }

    }
}