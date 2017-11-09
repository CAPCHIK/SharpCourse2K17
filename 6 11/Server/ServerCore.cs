using System.Net.Sockets;
using System.Net;
using System.Threading.Tasks;
using System;
using System.Threading;
using System.Text;
using Server.Models;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Shared.Models.Messages;
using System.Linq;
using Shared.Models.Responses;
using Shared.Models.Users;

namespace Server
{
    //Класс ядро сервера содержит всю логику работы сервера
    public class ServerCore
    {
        //Сервер должен иметь доступ к хранилищю пользователей
        private DataStorage storage;
        //Сервер должен иметь возможность работать с сетью
        private NetworkWrapper networkManager;
        //Сервер может быть выключен, и мы будем использовать
        //токен отмены работы для остановки потоков
        private CancellationTokenSource tokenSource = new CancellationTokenSource();
        //Список уже подключенных конечных устройств
        //Это могут быть как авторизованные пользователи
        //Так и еще не авторизованные пк
        private List<RemoteClient> clients = new List<RemoteClient>();
        //Конструктор ядра, выполняем подготовительные работы
        public ServerCore()
        {
            //Создаем хранилище пользователей
            storage = new DataStorage();
            //Создаем объект для доступа к сети
            networkManager = new NetworkWrapper(tokenSource.Token);
            //Подписываемся на событие о подключении некого клиента
            //Теперь, если у нам по сети подключился новый клиент
            //У нас выполнится метод ClientAccepted
            networkManager.ClientAccepted += ClientAccepted;
        }
        //Метод для запуска работы
        public void StartWork()
        {
            //Запускаем сетевой менеджер
            networkManager.Start();
        }
        //Метод для остановки сервера
        public void Stop()
        {
            //Переключаем токены выполнения
            tokenSource.Cancel();
        }
        //Выполняется, когда сетевой менеджер присоединил к себе
        //Нового клиента
        private void ClientAccepted(RemoteClient client)
        {
            //Добавляем клиента в список клиентов
            clients.Add(client);
            //При получении сообщения от этого клиента будем выполнять
            //Метод MessageEwceived
            client.MessageReceived += T => MessageReceived(client, T);
        }
        //Получено сообщение от клиента client сообщение находится в token
        private void MessageReceived(RemoteClient client, JToken token)
        {
            //КОнвертируем данные сообщения в объект типа Message
            var message = token.ToObject<Message>();
            //Логируем информацию о том, что мы получили сообщение
            Console.WriteLine($"Message type is {message.MessageType}");
            //Проверяем, какой тип у полкченного сообщения
            switch(message.MessageType)
            {
                //Клиент пытается войти в систему
                case MessageType.Login:
                    //Обрабатываем этот запрос, отражая данные 
                    //На тип LoginMessage
                    HandleLogin(client, token.ToObject<LoginMessage>());
                    break;
                //Клиент пытается зарегистрироваться
                case MessageType.Register:
                    //Обрабатываем этот запрос, отражая данные 
                    //На тип RegistrationMessage
                    HandleRegistration(client, token.ToObject<RegistrationMessage>());
                    break;
                //Клиент прислал сообщение, и он авторизован
                case MessageType.Text when (client.UserView != null):
                    //Обрабатываем этот запрос, отражая данные 
                    //На тип TextMessage
                    SendTextMessage(client, token.ToObject<TextMessage>());
                    break;
                //Клиент зпросил список пользоватей онлайн
                case MessageType.OnlineRequest:
                    //Отсылаем список имен обратно клиенту
                    SendUsersOnline(client);
                    break;
            }
        }
        //отправляем список имен пользоваьелей онлайн 
        private void SendUsersOnline(RemoteClient client)
        {
            //Получаем список имен, для этого берем клиентов
            var usernames = clients
                //Берем из них только тех, у которых
                //имеется отображение аккаунта, то есть они авторизованы
                .Where(C => C.UserView != null)
                //Получаем имена этих авторизованных тользователей
                .Select(C => C.UserView.Name)
                //Приводим эту последовательность к списку элементов
                .ToList();
            //Отправляем клиенту этот список, обернув его в OnlineUsersResponse
            client.Send(new OnlineUsersResponse(usernames));
        }
        //Обработка запрососа на логин
        private void HandleLogin(RemoteClient client, LoginMessage loginMessage)
        {
            //Находим пользователя в базе, у которого имя и пароль имеют переданные логин и пароль
            var targetaccount = storage.Find(U => U.Name == loginMessage.Name && U.Password == loginMessage.Password);
            //Если пользователя не найдено
            if(targetaccount == null)
            {
                //отправляем ответ, с инфой о том, что пара логин пароль неверна
                client.Send(new Response(StatusCode.IncorrectLoginOrPassword));
                //Ну и больше ничего не делаем
                return;
            }
            //нашли пользователя, который подходит по переданным параметрам
            client.UserView = targetaccount;
            //Отправляем сообщение что всё хорошо, пользотель вошел
            client.Send(Response.GoodResponse);
        }
        //Обработка запроса на регистрацию
        private void HandleRegistration(RemoteClient client, RegistrationMessage registrationMessage)
        {
            //проверяем, есть ли в базе пользователь с таким имемен
            var userWithSomwName = storage.Find(U => U.Name == registrationMessage.UserName);
            //Если пользователь  с таким именем есть в базе
            if (userWithSomwName != null)
            {
                //Сообщаем клиенту, что такое имя занято
                client.Send(new Response(StatusCode.UserNameBusy));
                //И больше ничего не делаем
                return;
            }
            //Имя пользователя не занято, создаем новый аккаунт
            User user = new User
            {
                Name = registrationMessage.UserName,
                Password = registrationMessage.UserPassword
            };
            //Сохраняем его в хранилище
            storage.RegisterUser(user);
            //И привызываем его к соединению, по которому пришел запрос на регистрацию
            client.UserView = user;
            //Отправяем клиенту инфу о том что он успешно зарегистрировался
            client.Send(Response.GoodResponse);
        }
        //Отправляем текстовое сообщение от клиента всем остальным
        private void SendTextMessage(RemoteClient client, TextMessage textMessage)
        {
            //Формируем объект хранящиу в себе имя отправителя и текст сообщения
            var response = new TextResponse(client.UserView.Name, textMessage.Content);
            //Проходимся цоклом по
            foreach(var targetClient in 
                    //Тем подключенным клиентам
                    clients
                    //Которые авторизованы в системе и их имя не совпадает с именем отпраителя
                    .Where(U => U.UserView?.Name != client.UserView.Name))
            {
                //Каждому подходящему под выборку клиента отправляем сообщение
                targetClient.Send(response);
            }
        }
    }
}