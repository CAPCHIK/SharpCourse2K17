using System;
using Newtonsoft.Json;

namespace Shared.Models.Responses
{
    //Ответ от сервера, текстовое сообщение от кого либо
    public class TextResponse : Response
    {
        //Имя отправителя сообщения
        public string Sender { get; }
        //Текст, набранный отправителем
		public string Content { get; }
        //Конструктор для инициализации свойств
        //Аргументы подаются или вручную, или JSON библиотекой
        //При десериализации

        [JsonConstructor]
        public TextResponse(string sender, string content) 
            //Вызываем базовый конструктор, передавая тип данного отета
            //А именно - текстовое сообщение
            : base(StatusCode.TextMessage)
        {
            Sender = sender;
            Content = content;
        }

    }
}
