using System.Net.Sockets;
using System.Net;
using System;
using Client.ConsoleWrappers;
using Client.ConsoleWrappers.UserInputs;
using Shared.Models.Messages;
using Newtonsoft.Json;
using System.Text;
using Shared.Models.Responses;

namespace Client
{
    //Класс, опиывающий основную логику нашей программы
    //Подключение, отправка прием сообщений, прыжки меж формами
    public class ClientCore
    {
        //Поле networkManager, этот объект скрывает в себе
        //Методы для непосредственной работы с сетью, чтобы не захламлять основную логику
        private readonly NetworkWrapper networkManager = new NetworkWrapper();


        //Вся работа нашей программы сожержится в методе Work
        public void Work()
        {
            //Первое что надо сделать - найти сервер в локальной сети
            //Эту работу выполнит networkManager
            networkManager.FindServer();
            //Входим/регистрируемся в системе
            EnterInSystem();
            //Создаем форму чата, которая будет отображать сообщения текущего чата
            var chatForm = new ChatWrapper();
            //Вешаем слушателя на событие получения сообщения
            //То есть при получении networkManagerом сообщения из сети,
            //будет выполнен код, находящийся в лямбде. 
            //Согласно сигнатуре события внутрь лямбды приходят JToken и StatusCode
            networkManager.ResponseRecieved += (token, status) => {
                //Анализируем тип пришедшего сообщения
                switch(status)
                {
                    //Если это текстовое сообщение от некоторого клиента
                    case StatusCode.TextMessage:
                        //Добавляем это сообщение на нашу форму чата
                        chatForm.AddMessage(token.ToObject<TextResponse>());
                        break;
                    //Сдесь не реализована логика приема других типов сообщения, на подобие
                    //Пользователь такой-то подключился или отключился
                    //Но это не сложно добавить, расширив switch
                }
            };
            //Бесконечно читаем пользовательский ввод
            while(true)
            {
                //Анализируем то, что ввел пользователь в форму чата
                switch(chatForm.ReadInput())
                {
                    //Если он ввел строку - то есть метод ReadInput()
                    //Вернул объект типа StringInput
                    case StringInput str:
                        //Добавляем в форму чата сообщение, что мы ввели такое-то сообщение
                        chatForm.AddSelfMessage(str.InputString);
                        //Отправляем это сообщение на сервер.
                        networkManager.SendMessage(new TextMessage(str.InputString));
                        break;
                    //Если пользователь ввел действие (/цифра)
                    //И ввел именно /1, у нас на форме чата только одно доступное действие
                    case ActionInput act when (act.ActionIndex == 1):
                        //Останавливаем рендеринг окна чата, чтобы приходящие с сервера
                        //сообщения не мешали отображению списка пользователей
                        chatForm.StopRendering();
                        //Показываем список пользователей
                        ShowOnlineUsers();
                        //Снова запускаем работу формы чата
                        chatForm.StartRendering();
                        break;
                }

            }
        }

        //Отображает на экране список пользователей, находящихся онлайн
        private void ShowOnlineUsers()
        {
            //Создаем специальную форму, которая умеет выводит список пользователей
            var usersOnline = new OnlineUsersWrapper();
            //получаем из networkManagerа список пользователей, и заполняем им 
            //Нашу форму
            usersOnline.SetUsersList(networkManager.GetUsersList());
            //Ждем хоть какое-то действие пользователя, чтобы прекратить метод
            //Показа пользователей онлайн
            usersOnline.WaitInput();
        }
        //Метод позволяет пользователю или войти в систему под уже существующим аккаунтом
        //Или зарегистрировать новый аккаунт
        private void EnterInSystem()
        {
            //Читаем первый выбор пользователя
            //Для этого создаем форму LoginOrRegister, которая предоставляет пользователю
            //Выбор, логин или регистрация. Получаем выбор пользователя, и приводим его к
            //Типу ActionInput
            var firstChoose = new LoginOrRegisterWrapper().GetUserChoose() as ActionInput;
            //Смотрим, что именно выбрал пользователь
            switch (firstChoose.ActionIndex)
            {
                //Первое действие - логин. окей, логиним пользователя
                case 1:
                    HandleLogin();
                    break;
                //Второе действие. Значин он хочет регистрироваться
                case 2:
                    HandleRegister();
                    break;
            }
        }
        //Регистрация пользователя
        //Аргумент message будет выводиться в начале формы регистрации
        //По умолчанию он равен Registration, и мы можем вызвать метод  
        //HandleRegister без аргументов.
        //Если произошла какая-то ошибка при регистрации, это сообщение заменится на 
        //Информацию о произошедшей ошибке
        private void HandleRegister(string message = "Registration")
        {
            //Создаем форму логина или регистрации
            //У нас имеется одна форма, так как и для логина и для регистрации
            //Нужно только два поля, ник и пароль
            //Как аргумент - шапка формы
            var registerWrapper = new LoginPasswordPairWrapper(message);
            //Читаем пару логин/пароль, которые ввел пользователь
            var pair = registerWrapper.GetPair();
            //записываем эти данные в сообщение RegistrationMessage
            //Для отправки на сервер
            var registerMessage = new RegistrationMessage()
            {
                UserName = pair.Login,
                UserPassword = pair.Password
            };
            //Отправляем сообщение на сервер
            networkManager.SendMessage(registerMessage);
            //Ждем от сервера ответа. На данном этапе нам мог прийти ответ только на это сообщение
            var response = networkManager.GetResponse();
            //В зависимости от кода результата делаем разные вещи
            switch (response.StatusCode)
            {
                //Всё хорошо, юзер зареган, заканчиваем работу по регистрации в системе
                case StatusCode.OK:
                    return;
                //Произошла ошибка, введенное пользователем имя уже занято
                case StatusCode.UserNameBusy:
                    //Снова вызываем метод для регистрации, но уже с новым сообщением в шапке
                    //Уже этот вызов должен будет решить проблему
                    HandleRegister($"Name {pair.Login} is busy");
                    return;
                //Пришла какая-то дичь, роняем программу
                default:
                    throw new Exception($"Status code is bad :( {response.StatusCode}");
            }
        }
        //Обработка входа пользователя в существующий аккаунт. В целом похоже на регистрацию
        private void HandleLogin(string message = "Login")
        {
            //Снова форма для ввода пары логин/пароль
            var loginWrapper = new LoginPasswordPairWrapper(message);
            //Читаем эту пару
            var pair = loginWrapper.GetPair();
            //Записываем данные в специальное сообщение
            var LoginMessage = new LoginMessage()
            {
                Name = pair.Login,
                Password = pair.Password
            };
            //Отправляем его на сервер
            networkManager.SendMessage(LoginMessage);
            //Получаем ответ от сервера
            var response = networkManager.GetResponse();
            //Проверяем результат
            switch (response.StatusCode)
            {
                //Вошли в систему, всё ок
                case StatusCode.OK:
                    return;
                //Ввели неправильную пару логин/пароль
                case StatusCode.IncorrectLoginOrPassword:
                    //Снова выполняем действия для входа в систему, с новой шапкой
                    HandleLogin($"incorrect login or password");
                    return;
                //Пришла дичь, роняем прогу
                default:
                    throw new Exception($"Status code is bad :( {response.StatusCode}");
            }
        }
    }
}