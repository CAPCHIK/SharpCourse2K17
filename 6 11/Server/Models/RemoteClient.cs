using System;
using System.Net.Sockets;
using Newtonsoft.Json.Linq;
using Shared.Models.Responses;
using Shared.Models.Users;

namespace Server.Models
{
    //Класс обертка над
    //моделью клиента и над TCP соединением
    //Используется для связи конкретного соединения
    //И пользователя, который подключен по этому соединению
    public class RemoteClient
    {
        //Приватное поле действие, как именно можно этому клиенту
        //Отправить сообщение
        //Передается при создании
        private readonly Action<Response> sendResponse;
        //Модель пользователя, который подключен по данному соединению
        //Если пользователь еще не авторизован равно null
        public User UserView { get; set; }
        //Событие, что от данного клиента пришло какое-то сообщение
        public event Action<JToken> MessageReceived;
        //Конструктор принимает в себя метод, который умеет отправлять сообщение 
        //Именно жтому клиенту
        public RemoteClient(Action<Response> sendResponse)
        {
            //И сохраняет его в приаватный объект
            this.sendResponse = sendResponse;
        }
        //Отправляет сообщение, прочто вызывая переданный в конструкторе метод
        public void Send(Response response) =>
                    sendResponse(response);
        //Когда от пользователя пришло некое сообщение
        //Вызывается этот метод
        public void Notify(JToken token)
        {
            //Он в свою очередь вызывает событие о получении сообщения
            //Передавая в него данные сообщения
            MessageReceived?.Invoke(token);
        }
    }
}
