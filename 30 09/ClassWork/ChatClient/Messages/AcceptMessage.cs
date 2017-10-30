using System;

namespace ChatClient.Messages
{
    //ПРедставляет данные о сообщении, подтверждающем получение какого-то другого сообщения
    public class AcceptMessage : Message
    {
        //Идентификатор полученного и прочтенного ранее сообщения
        public Guid ReceivedMessageId {get; set;}
        //Данный класс всегда возвращяет тип сообщения MessageType.AcceprMessage
        public override MessageType MessageType => MessageType.AcceptMessage;
    }
}