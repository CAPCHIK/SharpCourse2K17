using System;
using Newtonsoft.Json;

namespace ChatClient.Messages
{
    //Класс представляет информацию про отравляемое текстовое сообщение
    public class TextMessage : Message
    {
        //Идентификатор сообщения
        public Guid Id {get; set;}
        //Текст самого сообщения
        public string Text {get; set;}
        //Количество прочтений сообщения, не передается по сети
        //благодоря JsonIgnore
        //Как этот игнор работает - не важно на этом этапе
        //Если интересно - google + C# аттрибуты

        [JsonIgnore]
        public int ReadedCount {get; set;}
        //Данный класс всегда возвращает тип MessageType.TextMessage
        public override MessageType MessageType => MessageType.TextMessage;
    }
}