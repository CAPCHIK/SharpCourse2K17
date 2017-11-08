using Newtonsoft.Json;

namespace Shared.Models.Messages
{
    //Базовый класс сообщения
    public class Message
    {
        //По умолчанию тип не известен
        public virtual MessageType MessageType { get; } =  MessageType.Unkwnown;

        //Конструктор, аргумент или руками или библиотекой JSON
        [JsonConstructor]
        public Message(MessageType messageType)
        {
            MessageType = messageType;
        }
        //Пустой конструктор, не используется
        public Message()
        {

        }
    }
}